---
title: "WebSharper 4.1 released"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.1 introduces multiple new client-server features"
identity: "5483,84130"
---
WebSharper 4.1 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](http://websharper.com/downloads).

New and updated documentation: [WebSharper 4.1 for C#](http://developers-test.websharper.io/docs/v4.1/cs) and [WebSharper 4.1 for F#](http://developers-test.websharper.io/docs/v4.1/fs).

See some new features live at Try WebSharper: [in C#](http://try.websharper.com/snippet/JankoA/0000Hb) and [in F#](http://try.websharper.com/snippet/JankoA/0000Ha).

The release notes are also on [GitHub](https://github.com/dotnet-websharper/websharper/releases/tag/4.1.0.171).

# New features
* Server-side event handlers and `ctx.ClientSide` can take any fixed F# quotation now and translates it to JavaScript.
* New implementation for `WebSharper.Sitelets.Router` type, fully composable and also working on the client-side. `WebSharper.UI` adds helpers to use it for client-side routing. This allows generating safe links on the client for server-side endpoints, also on the server to states handled by the client.

See the details of Sitelet and Router features in the [C#](https://developers.websharper.com/docs/v4.1/cs/sitelets) and [F#](https://developers.websharper.com/docs/v4.1/fs/sitelets) documentation.

# WebSharper.UI
* `WebSharper.UI.Next` has been renamed to `WebSharper.UI`, affecting both package and namespace naming. For C# projects, an additional package reference for `WebSharper.UI.CSharp` is needed, and `.CSharp` has been removed from all namespace names, so now no extra `using`s are needed to get the C#-oriented extension methods. A compatibility package `WebSharper.UI.Next 4.1` still exists for immediate back-compatibility, switching to `WebSharper.UI` is recommended for new features and fixes.
* For composing HTML in F#, the `...Attr` family of functions have been renamed to match just the name of the tag, while the previous functions taking no Attr list have been removed. So previous `div [text "Hello"]` becomes `div [] [text "Hello"]` with `WebSharper.UI`.
* An nice and easy syntax is available for creating reactive values. If `x` and `y` are both `Var<int>` or `View<int>` then `V(x.V + x.Y)` is eqivalent to using `View.Map2 (+)`. Also within F# `text` or `Attr.` functions and any C# HTML combinators, the logic for `V(...)` is applied automatically, so no extra wrapping is needed.
* Template types (created either by the C# code generator or the F# type provider) have a new `.Vars` property, exposing all the variables that has been bound to inputs in the template. If you specify no binding, a `Var` will be implicitly created for the input.
* The `IRef` type has been removed and `Var` is now an abstract class.

See the details of UI features in the [C#](https://developers.websharper.com/docs/v4.1/cs/ui) and [F#](https://developers.websharper.com/docs/v4.1/fs/ui) documentation.

# Fixes/improvements
* Fix using `<UseDownloadedResources>True</UseDownloadedResources>` in `web.config`, now local script links are correct for any sub-page.
* Expanded `WebSharper.Testing` to expose more equality test alternatives provided by `QUnit`.
* Better handling of F# trait calls (using a member constraint inside an `inline` function). Now an overloaded method is resolved based on signature.
* Optimization of remoting protocol: array of types are not included when it is empty.

# Breaking changes

* Attributes previously in `WebSharper.Sitelets` namespace and assembly are now in `WebSharper` namespace and in `WebSharper.Core.dll`.
* A `Sitelet`'s `Router` field is now of type `IRouter`, compatible in capabilities with the old `Router` record. The new `Router` type offers more combinators and is implementing `IRouter`.
