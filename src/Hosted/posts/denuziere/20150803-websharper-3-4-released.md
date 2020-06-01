---
title: "WebSharper 3.4 released"
categories: "fsharp,javascript,web,websharper"
abstract: "WebSharper 3.4 is out with a slew of features: new F# 4.0 collection functions, revamped sitelets API, anti-CSRF protection for RPC functions, and a reworked HTML language in UI.Next with server-side capability."
identity: "4422,79881"
---
We are happy to announce the availability of WebSharper 3.4, which you can download [here](http://websharper.com/downloads). Here are the main highlights of this release.

## Revamped Sitelets API

The sitelets API has received a well-needed cleanup and simplification.

* Content is now always asynchronous: constructors and combinators take and return values of type `Async<Content<_>>`.

* Context is now always passed from the sitelet to the content, rather than as a callback when constructing the content.

* Content creation functions have been renamed:
    * `Content.PageContent[Async] --> Content.Page` overloaded with named arguments instead of a record
    * `Content.JsonContent[Async] --> Content.Json`
    * `Content.CustomContent[Async] --> Content.Custom`
    * `Content.Text` was added.
    * `Content.File` was added.

* A new module `WebSharper.Application` contains functions to create sitelets:
    * `MultiPage` is equivalent to `Sitelet.Infer`.
    * `SinglePage` creates a sitelet with a single endpoint (action) and HTML content.
    * `Text` creates a sitelet with a single endpoint and text content.

Here is a small before/after comparison on a small 2-page application:

```fsharp
open WebSharper
open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /api">] Api

// WebSharper 3.3

let HomePage =
    Content.PageContent <| fun ctx ->
        { Page.Default with
            Title = Some "Home"
            Body = [A [HRef (ctx.Link Action.Home)] -< [Text "This is the home page."]]
        }

let ApiPage =
    Content.JsonContent <| fun ctx ->
        ["a value"; "serialized"; "to json"]

[<Website>]
let MyWebsite =
    Sitelet.Infer <| function
        | Action.Home -> HomePage
        | Action.Api -> ApiPage

// WebSharper 3.4

let HomePage (ctx: Context<EndPoint>) =
    Content.Page(
        Title = "Home",
        Body = [A [HRef (ctx.Link Action.Home)] -< [Text "This is the home page."]]
    )

let ApiPage =
    Content.Json ["a value"; "serialized"; "to json"]

[<Website>]
let MyWebsite =
    Application.MultiPage <| fun ctx -> function
        | Action.Home -> HomePage ctx
        | Action.Api -> ApiPage ctx
```

A number of these new functions are equivalent to functions that were in WebSharper.Warp, which are now marked as obsolete.

## New HTML language for UI.Next

We completely redesigned the HTML embedded language for WebSharper UI.Next. We will post another blog entry soon to fully describe the new design, but here are the main take-aways:

* Element and attribute combinators are now lowercase, for a hopefully more natural-looking syntax.
* The type `Doc` can be used for both client-side and server-side markup. Some features are only available on one side, such as UI.Next reactive elements for the client and conversion to a `Content<_>` for the server. To use these features, you need to open `WebSharper.UI.Next.Client` or `WebSharper.UI.Next.Server`, respectively.  
    A template for client-server UI.Next applications has also been added to the Visual Studio and Xamarin Studio extensions.

Our goal for WebSharper 4.0 is to obsolete the current `Html.Client` and `Html.Server`, and to merge WebSharper.UI.Next into WebSharper under a new name. `Doc` will be the unique way to deal with HTML content, both on the client and the server side.

## F# 4.0 proxies

WebSharper 3.4 provides JavaScript proxies for the functions that were added to the standard library in F# 4.0. You can now call functions such as `Seq.mapFold` and `Array.last` from the client-side.

## Cross-site features for RPC functions

WebSharper 3.4 integrates several features related to cross-site requests to RPC functions:

* Protection against Cross-Site Request Forgery using a cookie-to-header token. This protection is active by default and completely automatic.  
If you need to deactivate it, in particular to be able to call RPC functions from a PhoneGap mobile application, you can simply call `DisableCsrfProtection()` from module `WebSharper.Web.Remoting` at the top-level on the server side.
* Management of allowed CORS origins. In module `WebSharper.Web.Remoting`, the functions `AddAllowedOrigin` and associated can be called at the top-level on the server side to add CORS origins accepted by the RPC handler. In particular, PhoneGap applications need to add `file://` as an allowed origin.

## Other changes

Here are the minor changes and bug fixes for WebSharper 3.4:

* Fix [#453](https://github.com/intellifactory/websharper/issues/453): whitespace around the title in `Content.Page`.
* Fix [#459](https://github.com/intellifactory/websharper/issues/459): Error 500 when replying to an RPC from a page with url "file://..."

Happy coding!