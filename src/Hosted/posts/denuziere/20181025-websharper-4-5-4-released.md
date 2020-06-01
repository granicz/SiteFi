---
title: "WebSharper 4.5.4 released"
categories: "cors,csharp,fsharp,javascript,websharper"
abstract: "This minor release adds sitelet CORS support, drops jQuery from UI's dependencies and fixes a number of issues."
identity: "5627,86006"
---
We are happy to announce the release of WebSharper 4.5.4.

Download the project templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.5.4.278`

Or use the extension for Visual Studio 2017: http://websharper.com/installers/WebSharper.4.5.4.278.vsix

## CORS handling

The main highlight of this release is the new API to handle [Cross-Origin Resource Sharing (CORS)](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS) headers in Sitelets. This allows you to manage what requests are allowed to be performed by a browser from a different domain.

This API is fully integrated with inferred sitelets. It automatically handles preflight `OPTIONS` requests, and adds the necessary headers to the main response.

For example, here is a JSON API as you would typically define it:

```fsharp
/// The endpoint type for our full website.
type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "/api">] Api of ApiEndPoint

/// The endpoint type for our API subsite.
and ApiEndPoint =
    | [<EndPoint "GET /users">] GetUsers
    | [<EndPoint "GET /users">] GetUser of id: int
    | [<EndPoint "POST /users"; Json "data">] PostUser of data: UserData
    | [<EndPoint "DELETE /users">] DeleteUser of id: int

and UserData = // ...

/// Handle API requests.
let HandleApi (ctx: Context<EndPoint>) (endpoint: ApiEndPoint) =
    match endpoint with
    | GetUsers -> Content.Json "retrieve and return the list of users..."
    | GetUser id -> Content.Json "retrieve and return the user..."
    | PostUser data -> Content.Json "store the user..."
    | DeleteUser id -> Content.Json "delete the user..."

/// Handle all requests, including the main site and the API subsite.
let Website = Application.MultiPage(fun (ctx: Context<EndPoint>) (endpoint: EndPoint) ->
    match endpoint with
    | Home -> Content.Text "Home page..."
    | Api endpoint -> HandleApi ctx endpoint
)
```

This website handles requests such as `GET /api/users` or `DELETE /api/users/123`.

Adding CORS support is done in two steps:

1. Wrap the affected endpoint type in `Cors<_>`:

    ```fsharp
    type EndPoint =
        | [<EndPoint "GET /">] Home
        | [<EndPoint "/api">] Api of Cors<ApiEndPoint> // <- ApiEndPoint needs CORS handling
    ```
    
2. Use `Content.Cors` to indicate the CORS headers to serve:

    ```fsharp
    let Website = Application.MultiPage(fun ctx endpoint ->
        match endpoint with
        | Home -> Content.Text "Home page..."
        | Api endpoint ->
            Content.Cors endpoint
                (fun corsAllows ->
                    // Add the headers you need here:
                    { corsAllows with
                        // Allow requests from these origins:
                        Origins = ["http://example.com"; "https://example.com"]
                        // Allow these requests to send user credentials (including cookies):
                        Credentials = true
                        // Enforce a new preflight request after 24 hours:
                        MaxAge = Some 86400 })
                (HandleApi ctx)
    )
    ```
    
And that's it! Note that we didn't need to specify the `Methods` to allow, because WebSharper is able to infer which methods need to be allowed based on the endpoint type definition. In this case, the inferred header value is `GET, POST, DELETE`.

Now, here is the full changelog for this release:

## WebSharper

### Breaking changes

* [#1028](https://github.com/dotnet-websharper/core/issues/1028)  Simplified and unified class names in `WebSharper.JavaScript.Dom`, stripping `DOM` prefix where there was one.

### Features

* [#999](https://github.com/dotnet-websharper/core/issues/999): Added `Content.Cors`, a helper for checking for Cross-Origin Resource Sharing headers.
* [#1025](https://github.com/dotnet-websharper/core/issues/1025) Added `Router.FetchWith`and `Router.AjaxWith` that are taking a `RequestOptions` to customize the request sent to an automatically routed WebSharper endpoint.

### Fixes

* [#1019](https://github.com/dotnet-websharper/core/issues/1019) Show a compile-time warning when an `Inline` or `Direct` contains a `$variable` that doesn't correspond to an argument. Additionally, added a property `UseDollarVariables : string` on both `InlineAttribute` and `DirectAttribute` to indicate that a given `$variable` is not an argument typo and should not cause a warning.

    For example, the following warns that `$unexpected` doesn't correspond to an argument, but doesn't warn about `$expected1` and `$expected2`:

    ```fsharp
    [<Inline("$this.myMethod($theArg, $unexpected, $expected1, $expected2)",
             UseDollarVariables = "$expected1, $expected2")>]
    member this.MyMethod(theArg: int) = X<unit>
    ```
* [#1021](https://github.com/dotnet-websharper/core/issues/1021) Fixed parsing FormData when using `Router.Infer`.
* [#1027](https://github.com/dotnet-websharper/core/issues/1077) Added `classList` on `Dom.Element`.
* [#1030](https://github.com/dotnet-websharper/core/issues/1030) Use invariant number format for floats in routing. Fixes failing to parse decimal point character in URLs created on the client when the server system settings use another character instead of point.
* [#1031](https://github.com/dotnet-websharper/core/issues/1031) Handle NoWarn+TreatWarningsAsErrors well for F# compilation.
* [#1032](https://github.com/dotnet-websharper/core/issues/1032) Fix skipping pre-translating some server-side quoted event handlers. In particular, this caused issues in server-side WebSharper.UI templates where one could not use both `ws-onxxx` and `.OnYyy()` with the same template.
* [#1033](https://github.com/dotnet-websharper/core/issues/1033) Fix inconsistent behavior of `HtmlTextWriter` between netfx and netstandard. Attributes written using `HtmlTextWriter.WriteAttribute`, which includes WebSharper.UI attributes, are now always automatically escaped.


## WebSharper.UI

### Enhancements

* [#182](https://github.com/dotnet-websharper/ui/issues/182) Eliminate the last uses of jQuery: `parseHTML` in templating and `Doc.Verbatim`, and `on` in `Input.Keyboard`.
* [#189](https://github.com/dotnet-websharper/ui/issues/189) C# templating: correctly pass along custom `ClientLoad` and `ServerLoad` from template comments and use `ClientLoad = Inline` as default.
* [#190](https://github.com/dotnet-websharper/ui/issues/190) C# templating: fix generated code for `<input type="number">`
* [#195](https://github.com/dotnet-websharper/ui/issues/195): Add `V`-enabled `Attr.Prop` overload.

### Fixes

* [#190](https://github.com/dotnet-websharper/ui/issues/190) C# templating: generate full namespace for `CheckedInput`.
* [#194](https://github.com/dotnet-websharper/ui/issues/194): Ensure `Router.Install` and `Router.InstallInto` initialize their state from the current URL.
* [#198](https://github.com/dotnet-websharper/ui/issues/198)  Ensure `Elt.OnAfterRender()` depends on `OnAfterRenderControl` for the Sitelet runtime to include the correct `.js` link needed.
* [#199](https://github.com/dotnet-websharper/ui/issues/199) Don't double-encode generated event attributes, causing server-defined event handlers to fail on .NET core.

Happy coding!