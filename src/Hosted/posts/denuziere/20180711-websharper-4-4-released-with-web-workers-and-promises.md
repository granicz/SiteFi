---
title: "WebSharper 4.4 released with Web Workers and Promises"
categories: "webworker,promise,csharp,fsharp,javascript,websharper"
abstract: "This release improves client-side parallel and asynchronous programming in F# and C#."
identity: "5618,85525"
---
We are happy to announce the release of WebSharper 4.4.0.

You can try it by installing the project templates for .NET Core SDK by running: `dotnet new -i WebSharper.Templates::4.4.0.255`  
or by downloading the templates for Visual Studio 2017 [here](http://websharper.com/installers/WebSharper.4.4.0.255.vsix).

The main highlight of this release is the new Web Workers feature.

Web Workers are a browser feature that allows running JavaScript code in parallel of the main UI thread, taking advantage of concurrency for resource-intensive computations.

Normally, the source code for a Web Worker is a separate JavaScript file that must be passed by URL to the Worker constructor:

```javascript
/// index.js:

var worker = new Worker("worker.js");

// This code runs in the main thread:
worker.postMessage("Hello, worker!");
```

```javascript
/// worker.js:

self.onmessage = e => console.log("Received message from main thread: " + e.data);
```

WebSharper completely automates the creation of this file, which means you can create a worker directly in code like so:

In F#:

```fsharp
let worker = new Worker(fun self ->
    // This code runs in the worker:
    self.Onmessage <- fun e -> Console.Log("Received message from main thread: " + string e.Data)
)

// This code runs in the main thread:
worker.PostMessage("Hello, worker!")
```

In C#:

```fsharp
var worker = new Worker(self =>
{
    // This code runs in the worker:
    self.Onmessage = e => Console.Log("Received message from main thread: " + (string)e.Data);
});
    
// This code runs in the main thread:
worker.PostMessage("Hello, worker!");
```

You can learn more about this feature in [the documentation](http://developers.websharper.com/docs/v4.x/fs/web-workers).

Another addition is the proxy for the JavaScript `Promise` type. In addition to the type definition, we added a few bells and whistles:

* Conversion functions from and to F# `Async` and C# `Task` values: `Promise.AsAsync`, `Promise.OfAsync`, `Promise.AsTask` and `Promise.OfTask`.
* A `promise` computation expression to easily combine promises in F#.

Here is the full change log:

## Features

* [#641](https://github.com/dotnet-websharper/core/issues/641), [#970](https://github.com/dotnet-websharper/core/issues/970): Add support for Web Workers.
    * Add the `Worker` API to `WebSharper.JavaScript`.
    * Add a set of special constructors for `Worker` which takes a function as argument. The compiler automatically compiles this function into a minimal bundle script and compiles the constructor call to reference this script by URL. See [the documentation](http://developers.websharper.com/docs/v4.x/fs/web-workers) for more details.
    * Add a new compiler setting `scriptBaseUrl` which determines the base URL under which the worker scripts are located. The full URL is `<scriptBaseUrl>/<assemblyname>/<assemblyname>.<scriptname>.js`, where `<scriptname>` is `worker` unless specified in the constructor call.
* [#807](https://github.com/dotnet-websharper/core/issues/907): Merge the class `WebSharper.JavaScript.JS` from the assembly `WebSharper.JavaScript` with the module of the same name from the assembly `WebSharper.Main`. This mean that all uses of `JSModule` in C# must be replaced with `JS`.
* [#939](https://github.com/dotnet-websharper/core/issues/939): Add JavaScript Promises support. This includes:
    * Add a proxy for the `Promise<'T>` class.
    * Add a `Promise` module with functions `AsAysnc`, `OfAsync`, `AsTask` and `OfTask`.  
        The `As*` functions handle rejected promises as follows: if the rejection value is an `Error`, then it is used directly as the exception raised from the Async / Task. Otherwise, it is wrapped as the `Reason` property of a `NonStandardPromiseRejectionException`.
    * Add the above functions as extension methods to their respective relevant classes.
    * Add a `promise` computation expression.
* [#975](https://github.com/dotnet-websharper/core/issues/975): Drop the `[Serializable]` requirement for classes used in JSON serialization (RPC, Sitelets, `WebSharper.Json`).
* [#976](https://github.com/dotnet-websharper/core/issues/976): Allow Bundle projects to reference other Bundle projects that have an EntryPoint.
* [#979](https://github.com/dotnet-websharper/core/issues/979): Change signature of `Router.Create` to parse to option.
* [#980](https://github.com/dotnet-websharper/core/issues/980): Sitelets: add `Content.FromContent(Content<obj>)` to convert an F# content into a C# content.
* [#981](https://github.com/dotnet-websharper/core/issues/981): Add `Sitelet.MapContent`.
    * For F#: module function `Sitelet.MapContent : (Async<Content<'T>> -> Async<Content<'T>>) -> Sitelet<'T> -> Sitelet<'T>`
    * For C#: extension method `Sitelet<obj> Sitelet<obj>.MapContent(Func<Task<Content>, Task<Content>>)`
* [#982](https://github.com/dotnet-websharper/core/issues/982): Make all exceptions inherit from the JavaScript `Error` type. This affects exceptions declared with the F# keyword `exception` as well as classes inheriting from `System.Exception`. This makes it possible to perform runtime type tests (F# `:?`, C# `is`) against `System.Exception`.

## Fixes

* [#971](https://github.com/dotnet-websharper/core/issues/971): WIG: properly fail when trying to declare that a class implements a class, rather than inherits from it.
* [#977](https://github.com/dotnet-websharper/core/issues/977): Give a better error if there is a conflict where two projects include a server-side compiled quotation from the exact same file and position.
* [#978](https://github.com/dotnet-websharper/core/issues/978): Give a better error if a Bundle is missing a project reference.
* [#983](https://github.com/dotnet-websharper/core/issues/983): Fix `dotnet build` on a C# project failing to find FSharp.Core.
* WebSharper.Testing: reference QUnit via `https`.
* [#987](https://github.com/dotnet-websharper/core/issues/987): Change the assemblies `WebSharper.Compiler`, `WebSharper.Compiler.CSharp` and `WebSharper.Compiler.FSharp` to reference `FSharp.Core` 4.4.1.0 rather than 4.5.0.0. This fixes assembly loading issues in the Roslyn analyzer.  
    As a consequence, we reduced the minimum dependency of the WebSharper.Compiler packages on FSharp.Core from >= 4.5.0 to >= 4.1.0.

Happy coding!