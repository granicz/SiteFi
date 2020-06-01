---
title: "WebSharper 3.1 published"
categories: "razor,fsharp,javascript,aspnetmvc,web,websharper"
abstract: "The main highlights of this release are ASP.NET MVC support (including Razor pages), and a lighter syntax to embed client-side controls in sitelets."
identity: "4372,79484"
---
It's barely been a month since we released WebSharper 3.0, and we are now at it again with the release of WebSharper 3.1. Without further ado, here are the main highlights.

## ASP.NET MVC support

WebSharper has supported including client-side controls in ASPX pages since version 1.0, and running sitelets alongside ASP.NET since version 2.0. With version 3.1 and [WebSharper.AspNetMvc](http://www.nuget.org/packages/WebSharper.AspNetMvc), WebSharper now has extended support for running together with ASP.NET MVC in the following scenarios:

* Including WebSharper client-side controls in pages that use the Razor view engine. Here is an example:

    ```xml
    @{
        // Create a web control
        var myControl = WebSharper.AspNetMvc.ScriptManager.Register(new MyWebSharperControl())
    }
    <!DOCTYPE html>
    <html>
      <head>
        <title>Testing WebSharper with ASP.NET MVC!</title>
        <!-- Render the needed css and script tags -->
        @WebSharper.AspNetMvc.ScriptManager.Head()
      </head>
      <body>
        <h1>Here is my control:</h1>
        <!-- Render the control -->
        @myControl
      </body>
    </html>
    ```
    
    Remoting is also supported, you can simply call server-side `[<Rpc>]`-annotated functions from `MyWebSharperControl`.

* Running WebSharper sitelets alongside ASP.NET MVC, serving pages and APIs from both. You can decide which one takes priority when their URL spaces overlap.

See [the WebSharper ASP.NET documentation page](http://websharper.com/docs/aspnet) to learn how to integrate WebSharper.AspNetMvc into an ASP.NET MVC application.

## Lightweight syntax to embed client-side elements in sitelets markup

Until now, in order to include client-side generated markup (whether using `Html.Client` or `UI.Next`) inside `Html.Server` markup, you had to create a new class inheriting from `Web.Control` and override its `Body` property. There is now a much easier syntax that you can use thanks to the `ClientSide` function:

```fsharp
[<JavaScript>]
module Client =
    open WebSharper.Html.Client

    let myContent text1 text2 = I [Text (text1 + "; " + text2)]

module Server =
    open WebSharper.Html.Server
    
    // Old Web.Control style:

    type MyControl(text1, text2) =
        inherit Web.Control()
        [<JavaScript>]
        override this.Body = Client.myContent text1 text2 :> _
        
    let OldBody =
        let t = "a local variable"
        Div [ new MyControl(t, "a literal") ]

    // New ClientSide style:

    let Body =
        let t = "a local variable"
        Div [ ClientSide <@ Client.myContent t "a literal" @> ]
```

Unlike the presence of a quotation suggests, this doesn't run any F#-to-JavaScript compilation at runtime. The quotation is only here to allow looking up the fully qualified name of the function you are calling (`Client.myContent` in the above example) and inlining it in the resulting page, alongside its JSON-serialized arguments.

You can read more about `ClientSide` [in the documentation](http://websharper.com/html-combinators).

## Sitelets routing enhancements

WebSharper 3.1 includes several enhancements to sitelets routing.

### Wildcard paths

You can add the `[<Wildcard>]` attribute to an action union case to make its last argument match the remainder of the path. This argument must be of type list, array, or string.

```fsharp
type Action =
    | [<Wildcard>] Articles of pageId: int * tags: list<string>
    | [<Wildcard>] Articles2 of tags: string[]
    | [<Wildcard>] GetFile of path: string

// GET /Articles/12/fsharp/websharper   -->   Articles(12, ["fsharp";"websharper"])
// GET /Articles2                       -->   Articles2 [||]
// Get /GetFile/Content/css/main.css    -->   GetFile "Content/css/main.css"
```

### Multiple actions with the same prefix

You can now create an action with several cases that parse the same prefix (ie. have the same `[<CompiledName>]`) on the same method. They will be tried in the order in which they are declared, until one of them matches. This is very convenient for REST-style URLs where additional information can be added with additional URL fragments.

```fsharp
type Action =
    | [<Method "GET"; CompiledName "blog">] ListBlogEntries
    | [<Method "GET"; CompiledName "blog">] BlogEntry of id: int
    | [<Method "GET"; CompiledName "blog">] BlogEntryWithSlug of id: int * slug: string

// GET /blog                               -->   ListBlogEntries
// GET /blog/123                           -->   BlogEntry 123
// GET /blog/123/websharper-31-published   -->   BlogEntryWithSlug(12, "websharper-31-published")
```

### Parsing form post data

Web forms using `method="post"` send their data in the request body with a format determined by `enctype` (generally either `application/x-www-form-urlencoded` or `multipart/form-data`). This data is available in the sitelet context as `Request.Post`. It is now also possible to directly parse it in the action using `[<FormData>]`. This attribute is used in a similar way as `[<Query>]`.

```fsharp
type Action =
    | [<Method "POST"; FormData("firstName","lastName")>]
        Register of firstName: string; lastName: string

// POST /Register
// Content-Type: application/x-www-form-urlencoded
//
// firstName=Loic&lastName=Denuziere                 --> Register("Loic", "Denuziere")
```

### Disposable enumerators

Perviously WebSharper translation didn't dispose enumerator objects used internally in `for ... in` loops. Also, enumerators created by Seq module functions didn't dispose the wrapped enumerators. These all now follow semantics of their .NET counterparts.

## The future

As you can see, we are now committing to a higher turn-around of WebSharper releases. Among enhancements you can expect in the future, we will be adding proxies for the [new library functions](https://github.com/fsharp/FSharpLangDesign/blob/master/FSharp-4.0/ListSeqArrayAdditions.md) as soon as F# 4.0 hits the shelves.

We are also preparing a service to try WebSharper F#-to-JavaScript compilation online. You will be able to experiment with F#-based web applications quicker and more easily than ever!

[![TryWebSharper screenshot](https://pbs.twimg.com/media/CEVnLnfWMAMPsNS.png)](https://pbs.twimg.com/media/CEVnLnfWMAMPsNS.png:large)

We cannot wait to make this available for you to try. In the meantime, happy coding!