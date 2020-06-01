---
title: "Bolero 0.2 released with improved routing"
categories: "bolero,webassembly,fsharp,websharper"
abstract: "You can now specify full path templates for routers."
identity: "5715,86366"
---
We are happy to announce the release of Bolero version 0.2. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using Blazor.

Install the latest project template with:

```shell
dotnet new -i Bolero.Templates
```

## Features

### Router: full path specification

In Bolero 0.1, you could customize the path for a router using `[<EndPoint>]`:

```fsharp
type Page =
    | [<EndPoint "/">]
      Home
    | [<EndPoint "/article">]
      Article of id: int
    | [<EndPoint "/list">]
      List of page: int * tag: string
```

In the above example, the value `Article 42` represents the URL `/article/42`, and the value `List(1, "bolero")` represents the URL `/list/1/bolero`.

Bolero 0.2 introduces full path specification. You can now indicate the exact shape of the path with `[<EndPoint>]`, with parameters represented between `{braces}`. This makes it possible to have several paths that share a common prefix. The `{*asterisk}` syntax is also supported to indicate the rest of the path.

```fsharp
type Page =
    | [<EndPoint "/">]
      Home
    | [<EndPoint "/article/{id}">]
      Article of id: int
    | [<EndPoint "/list/{page}">]
      List of page: int
    | [<EndPoint "/list/{page}/tagged/{*tags}">]
      ListTagged of page: int * tags: list<string>
```

In the above example, the value `List 2` represents the URL `/list/2`, and the value `ListTagged(2, ["bolero"; "fsharp"])` represents the URL `/list/2/tagged/bolero/fsharp`.

Bolero 0.1-style simple prefix specification is still supported. See [the documentation](https://github.com/fsbolero/bolero/wiki/Routing#format) for more details.

### DOM element references

It is sometimes necessary to call JavaScript functions on the DOM element corresponding to a Bolero node. Bolero 0.2 now provides the attribute `attr.bindRef` to retrieve a reference to the corresponding element. It is typically used in a component. For example, given this JavaScript function:

```javascript
function focus(elt) {
    elt.focus();
}
```

You can call it on your own elements like so:

```fsharp
open Bolero
open Bolero.Html
open Microsoft.JSInterop // Use Blazor's interop features

/// Interop binding to the JavaScript function
let focus (elt: ElementRef) =
    JSRuntime.Current.InvokeAsync("focus", elt) |> ignore

type MyComponent() =
    inherit ElmishComponent<Model, Message>()

    /// Create an element ref for our input tag
    let inputRef = ElementRefBinder()

    override this.View model dispatch =
        concat [
            // Bind the input element
            input [attr.bindRef inputRef]
            button [
                // Call the JavaScript function on the bound element
                on.click (fun _ -> focus inputRef.ref)
            ] [text "Focus the input"]
        ]
```

## Try F# on WebAssembly

I will take this occasion to talk about the improvements that have been done to the Bolero-based [Try F# on WebAssembly](http://fsbolero.io/) since its announcement; they've been out for a few weeks, but haven't been mentioned on this blog yet.

* We added support for asynchronous code, and in particular HTTP requests.

    To run asynchronous code, create a function `AsyncMain : unit -> Async<unit>`.

    To perform HTTP requests within `AsyncMain`, use `Env.Http : System.Net.Http.HttpClient`.

    See for example [the JSON type provider snippet](http://fsbolero.io/?snippet=TP_Json), which queries the GitHub API to list the repositories in the fsbolero organization.
    
* You can now invoke code completion using Ctrl+Space. It can take several seconds to appear on a bare identifier, but it is quite fast when invoked right after a dot.

![Code completion](https://i.imgur.com/KKZnf1y.jpg)

Happy coding!