---
title: "WebSharper 3.6.6 released"
categories: "fsharp,javascript,web,websharper"
abstract: "This release adds a small client-side cookies library, a function to require resources in client-side markup, and various bug fixes."
identity: "4636,80943"
---
We are happy to announce the release of WebSharper 3.6.6. Here are the main highlights.

### Cookies library

We integrated a library to facilitate the management of cookies on the client side. It is available under `WebSharper.JavaScript.Cookies`. Here is a small example:

```fsharp
open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
let testCookies() =

    // Set a cookie
    Cookies.Set("key", "value")

    // Set a secure cookie which expires in a day
    Cookies.Set("key", "value",
        Cookies.Options(
            Secure = true,
            Expires = System.DateTime.Now.AddDays(1.).JS))

    // Get a cookie
    let value = Cookies.Get("key")
    
    // Delete a cookie by setting it to expire immediately
    Cookies.Expire("key")
```

### Require resources in server-side markup

It can sometimes be useful to depend on a resource, such as a CSS file, without having any client-side markup. This is now possible using a `WebSharper.Web.Require` control.

```fsharp
open WebSharper

// Let's say you have a resource defined:

type MyResource() =
    inherit Resources.BaseResource("style.css")

[<assembly: System.Web.UI.WebResource("style.css", "text/css")>]
do ()

// And you want to include it in your page that doesn't contain client-side code.

// Using UI.Next:
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Server

let styledElement() =
    div [
        text "A page containing this element will include style.css"
        Doc.WebControl (Web.Require<MyResource>())
        // Or equivalently:
        Doc.WebControl (Web.Require(typeof<MyResource>))
    ]

// Using WebSharper.Html:
open WebSharper.Html.Server

let styledElement() =
    Div [
        Text "A page containing this element will include style.css"
        Web.Require<MyResource>()
        // Or equivalently:
        Web.Require(typeof<MyResource>)
    ]
```

That's it for the main new features; here is the full change log.

### WebSharper

* [#491](https://github.com/intellifactory/websharper/issues/491): Remove reference from `WebSharper.Sitelets.dll` to `WebSharper.Compiler.dll`
* [#498](https://github.com/intellifactory/websharper/issues/498): Add `WebSharper.Web.Require` (see above).
* [#502](https://github.com/intellifactory/websharper/issues/502): Add client-side Cookies library (see above).
* [#503](https://github.com/intellifactory/websharper/issues/503): `Window` inherits from `EventTarget`, allowing the use of `AddEventHandler` on `JS.Window`.
* [#504](https://github.com/intellifactory/websharper/issues/504): MSBuild task: force loading the right version of FSharp.Core in the appdomain.
* [#506](https://github.com/intellifactory/websharper/issues/): In unpack task, use `$(OutDir)` if set.
* [#507](https://github.com/intellifactory/websharper/issues/507): Honor optional arguments `Async.AwaitEvent(?cancelAction)` and `Async.StartChild(?timeout)`.
* [#508](https://github.com/intellifactory/websharper/issues/508): bind multiple-argument versions of `JSON.Stringify`.
* [#509](https://github.com/intellifactory/websharper/issues/): Fix `JSON.Serialize` macro for recursive types.
* Add missing proxies for `querySelector` and `querySelectorAll` on `Window` and `Dom.Element`.
* Add proxy for `System.TimeoutException`.
* Always extract resources when `WebSharperProject` is `Site`, thus fixing WebSharper.Suave [#7](https://github.com/intellifactory/websharper.suave/issues/7).

### WebSharper.UI.Next

* [#60](https://github.com/intellifactory/websharper.ui.next/issues/60): Templating: allow using the same simple text hole in multiple places.
* [#65](https://github.com/intellifactory/websharper.ui.next/issues/65): Make sure to set the selected element after rendering the select box.

Happy coding!