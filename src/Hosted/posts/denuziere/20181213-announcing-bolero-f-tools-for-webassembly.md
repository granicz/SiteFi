---
title: "Announcing Bolero: F# tools for WebAssembly"
categories: "bolero,webassembly,fsharp,websharper"
abstract: "A new library to create F# web applications in WebAssembly via Blazor."
identity: "5695,86199"
---
We are happy to announce the release of Bolero 0.1. Bolero is a brand new library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using [Blazor](https://blazor.net/). It is designed for Model-View-Update (Elmish) style development, and provides features such as HTML templating, remote server calls and advanced routing.

Bolero is designed to be familiar for people who have used [WebSharper](https://websharper.com) to write client-side applications, but also easy to learn if you have no such experience.

Bolero is released under the Apache 2.0 license and its source code is available [on GitHub](https://github.com/fsbolero/bolero).

## Getting started with Bolero

Creating a Bolero application is easy using `dotnet new`. First, install the Bolero project templates on your computer:

```
dotnet new -i Bolero.Templates
```

And then, create your project:

```
dotnet new bolero-app -o MyApp
```

[See here](https://github.com/fsbolero/Template) to learn more about this basic application.

## Features

Bolero comes with a number of features, many of which will be familiar if you have done F# web development using WebSharper or Fable.

### HTML in F# ###

Bolero uses a common list-based syntax to write HTML directly in F#. When opening the module `Bolero.Html`, HTML elements are plain functions, attributes are functions in the `attr` module, and event handlers are functions in the `on` module.

```fsharp
let sayHelloTo (who: string) =
    div [attr.id "greeting"] [
        textf "Click this button to say hello to %s:" who
        button [on.click (fun _ -> printfn "Hello, %s!" who)] [
            text "Click me!"
        ]
    ]
```

### HTML templates

If you prefer, you can write actual HTML in separate files, and fill its contents using the `Bolero.Template` type provider.

```html
<!-- main.html -->
<div id="greeting">
  Click this button to say hello to ${Who}:
  <button onclick="${SayHello}">Click me!</button>
</div>
```

```fsharp
type Main = Template<"main.html">

let sayHelloTo (who: string) =
    Main()
        .Who(who)
        .SayHello(fun _ -> printfn "Hello, %s!" who)
        .Elt()
```

### Model-View-Update with Elmish

Bolero integrates the [Elmish](https://elmish.github.io/elmish/) library to provide Model-View-Update (MVU) application architecture.

MVU is a way to structure applications that makes it easy to reason about the current state and its transitions. The state is centralized in an immutable value, the *Model*, which can only be changed by dispatching *Messages* to an update function.

For example, here is a simple text input Elmish program:

```fsharp
/// Model: define the state of the application.

type Model = { value: string }

let initModel = { value = "" }

/// Update: define the transitions of this state.

type Message =
    | SetValue of string
    
let update message model =
    match message with
    | SetValue v -> { model with value = v }
    
/// View: define the display of the application.

let view model dispatch =
    div [] [
        input [
            attr.value model.value
            on.input (fun e -> dispatch (SetValue (e.Value :?> string)))
        ]
        textf "You typed: %s" model.value
    ]

/// Program: put everything together.

let myProgram = Program.mkSimple (fun _ -> initModel) update view
```

It can be simply turned into a Blazor component:

```fsharp
type MyProgram() =
    inherit ProgramComponent<Model, Message>()
    
    override this.Program = myProgram
```

### Advanced routing

You can define the routes provided by your application as an F# union type, and Bolero will automatically parse the current page and generate URLs for links. This parsed route is easy to integrate into the Elmish application state.

In the following example:
* Whenever the page URL changes (for example by clicking a link), the new URL is parsed into a value of type `Page`, and a message `SetPage` is dispatched to Elmish.
* Whenever the `currentPage` field of the model changes, the page's URL is updated accordingly.

```fsharp
// An application with 3 URLs: "/", "/search" and "/article/123".
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/search">] Search
    | [<EndPoint "/article">] Article of id: int

// The Elmish model contains the current URL value.
type Model =
    { currentPage: Page
      // ...
    }

// The Elmish update sets the model accordingly.
type Message =
    | SetPage of Page
    // ...

// The router is simply created from the message and model...
let router = Router.infer SetPage (fun model -> model.currentPage)

// ...and passed to Elmish.
let program =
    Program.mkSimple model update view
    |> Program.withRouter router
```

### Remote server calls

Bolero provides a simple way to write server-side asynchronous functions that can be called directly from the client side. These server-side functions run on ASP.NET Core.

A set of remote functions is defined as a record of functions, each served at a specific URL:

```fsharp
type People =
    {
        create: Person -> Async<unit>          // Served at /people/create
        getById: int -> Async<option<Person>>  // Served at /people/getById
    }

    interface IRemoteService with
        member this.BasePath = "/people"
```

These remote functions can then be:
* implemented on the server side by providing an instance of `People` to ASP.NET Core;
* requested from the client side by calling these functions on an instance of `People` provided by Bolero.

Bolero will take care of serializing the arguments and result type to JSON and requesting the correct URL.

### F#-specific optimizations

In addition to Blazor's build infrastructure optimized for WebAssembly, Bolero performs additional F#-specific optimizations. Namely, it reduces the size of the files that will be downloaded by the browser by stripping out from all assemblies the F# signature data and optimization data, which is only necessary during compilation and can be dropped at runtime.

### ... And more

You can learn more about Bolero's capabilities in [the documentation](https://github.com/fsbolero/bolero/wiki).

See it in action with the classic [TodoMVC application](https://github.com/fsbolero/TodoMVC/).

[![TodoMVC with Bolero](https://i.imgur.com/UemGiYd.png)](https://github.com/fsbolero/TodoMVC/)

## Get involved!

Bolero is a community project, and we're happy to hear your feedback and receive your contributions! You can submit all of your suggestions and bug reports to [the issue tracker](https://github.com/fsbolero/bolero/issues).

Happy coding!