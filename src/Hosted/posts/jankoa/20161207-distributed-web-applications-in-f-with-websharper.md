---
title: "Distributed web applications in F# with WebSharper"
categories: "remoting,javascript,f#,websharper"
abstract: "Introducing new and improved WebSharper 4 beta features for RPCs: customizing both client and server side."
identity: "5204,82344"
---
This article is part of [F# Advent 2016](https://sergeytihon.wordpress.com/2016/10/23/f-advent-calendar-in-english-2016/)

## Introduction

Often some functionality of a web application is split between the server and client layers.
For example you want a separate component on the page which needs server interaction and package it to reuse in multiple projects.
Sometimes you need to talk to the server to be which is hosting the current application, sometimes another one that hosts a service for multiple web applications.

We will look at the tools that WebSharper provides to achieve all this and some more using only F#.

There is just a [fresh release](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.151.28-beta5) of WebSharper 4 beta out which contains fixes for these features, be sure to grab `Zafir beta-5` packages from NuGet or get the Visual Studio installer for project templates [here](http://websharper.com/Zafir.FSharp.vsix).
For a short introduction, read [The road to WebSharper 4](http://websharper.com/blog-entry/5203/the-road-to-websharper-4).

## WebSharper remoting

We will go into a couple new features of WebSharper 4 beta later, but start with a long-time basis of WebSharper: RPCs.
Just having a `Remote` annotated method exposes it to be callable from client-side code which is translated to JavaScript:

```fsharp
[<Remote>] // runs on the server
let GetData key = async { return DataStore.[key] } 

[<JavaScript>] // runs in the browser
let DataView key =
	async { 
		let! data = GetData key // calls server asynchronously
		return div [ text data ] // creates the view when response has arrived
	} |> Doc.Async // converts to an `Async<Doc>` to `Doc` value that can be embedded
				   // on a page and displays content as soon as it is available
```

This is transparent and lightweigth way of communicating with the server, no larger state is sent back to the server, only what you explicitly pass and a user session cookie.

Let's take a look at what is happening here behind our backs:

* The client-side gets constructs an instance of a "RemotingProvider" object. By default it is the `AjaxRemotingProvider` defined in WebSharper.
* The `Async` method of the RemotingProvider is called with with the RPC method handle (auto-generated) and arguments. There are separate functions for calling Rpcs that return an `async` a `Task` and `unit`, but all use a method `AsyncBase` for common logic. The method handle is something like `MyApp:MyApp.Server.GetData:-1287498065`, containing the assembly name, full path of method and a hash of the method's signature.
* The default implementation of `AsyncBase` sends a `XMLHttpRequest` to the client with the JSON-serialized form of the arguments.
* Server handles the request: looks up the method based on the handle, deserializes the arguments to .NET values and executes the method.
* Server serializes result based on metadata information which tells how they are represented in the JavaScript translation, and sends this back in the response.
* Client deserializes JSON response into objects and applies prototypes.
* Async continuation is called, or in case of a server error, the Error value will be propagated as in the `async` workflow.	

So calling a remote is not a safe operation as it can throw an exception, but we can catch it:
	
```fsharp
	async {  
		try 
			let! data = GetData key 
			return div [ text data ]  
		with e ->
			Console.Log("GetData error:", e)
			return div [ text "Error" ]
	} |> Doc.Async
```

But it is a not a nice functional approach to rely on exceptions.
There is a way to catch the error around every RPC call automatically.

## Customizing the client request

We can inherit from the default `AjaxRemotingProvider` and override the `AsyncBase` member which is has the common logic to handle calling RPC methods :

```fsharp
[<JavaScript>]
type SafeRemotingProvider() =
    inherit Remoting.AjaxRemotingProvider()

	override this.AsyncBase(handle, data) =
        async.TryWith(base.AsyncBase(handle, data), 
            fun e -> 
                Console.Log("Remoting exception", handle, e)
                async.Return(box None)
```
				
This does not knows about the RPC method is actually returning, so it is just assuming that it is an option so `None` is a correct value.
So we still need to be a bit careful to apply it only to `Remote` methods which are indeed returning an option.

```fsharp
[<Remote; RemotingProvider(typeof<SafeRemotingProvider>)>]
let GetData key =
    async { 
		match DataStore.TryGetValue(key) with 
		| true, c -> return Some c
		| _ -> return NOne
	} 
```

A good practice would be to have all RPC methods return an `option` (or `Result` or similar union) value and use the `RemotingProvider` attribute on the module defining the server-side functions, or even on assembly level.

## Setting the target server

The default `AsyncBase` looks up the `Endpoint` property on the same object, which by default reads a module static value `WebSharper.Remoting.Endpoint`.
This gives us a couple possibilities:

* If we want all remoting to target a server with a known URL, we can just set `WebSharper.Remoting.Endpoint` in the client side startup.
* If we want to host some server-side functionality as a separate service used by multiple web applications, we can put the it in a library project and apply a custom `RemotingProvider` class for the assembly that overrides the `Endpoint` property. You can use the `WebSharper.Web.Remoting.AddAllowedOrigin` method on server startup to allow access to the RPCs from other origins (sites which will be using your service).
 
## Decouple server-side implementation

WebSharper allows a fully type-checked communication between your web appication server and client code but if you have it all in one project then there is more coupling between the two layers than desired.
It comes in handy to define your RPC methods on abstract classes so that you can implement it in another project.
Then you can have your communication protocol defined in a common project referenced by a server-side and client-side project not depending directly on each other.

```fsharp
namespace Model
[<AbstractClass>]
type ServerDataStore() =
	[<Remote>]
	abstract GetData : string -> Async<option<string>>
```

Note that now we are putting the `Remote` attribute on an instance method.
There is no restriction on how many `Remote` methods you can have on a class, and you can mix abstract and non-abstract members of course.
In another project we can implement this with a subclass, its methods do not need the `Remote` attribute as they are not directly exposed but will be called through its virtual slot.

```fsharp
type ServerDataStoreImpl(store: Dictionary<string, string>) = 
	inherit Model.ServerDataStore()
	
	override this.GetData(key) =
		async { 
			match store.TryGetValue(key) with 
			| true, c -> return Some c
			| _ -> return NOne
		}
```

Then we need to provide the server runtime at startup with an instance of the `ServerDataStore` class, which will execute its implementations of the abstract remote methods:

```fsharp
    let store = Dictionary()
    store.Add("greeting", "Hello world!")
	AddRpcHandler typeof<Model.ServerDataStore> (ServerDataStoreImpl(store))
```

There should be only one handler object per type. As now the RPC is an instance method, we need a small helper to call it from the client code: 

```fsharp
	async {
		let! greet = Remote<Model.ServerDataStore>.GetData("greeting")
		greet |> Option.iter Console.Log
	}
```

## Putting it together

These features are all combinable.
So you can write a library that houses both server-side and client-side functionality for a feature, customize what happens on the client when the application calls one of the RPCs, and make the implementation of some or all RPCs overridable.
We are using these possibilities actively in our current projects, we hope that WebSharper can bring joy to even more developers in the future.