---
title: "WebSharper 4.4.1 released"
categories: "csharp,fsharp,javascript,websharper"
abstract: "This is a minor release for WebSharper and WebSharper.UI."
identity: "5623,85627"
---
This is a minor release for WebSharper and WebSharper.UI.

Install templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.4.1.261`

Download templates for Visual Studio 2017: http://websharper.com/installers/WebSharper.4.4.1.261.vsix

# WebSharper

## Features

* [#988](https://github.com/dotnet-websharper/core/issues/988) Add proxy for the JavaScript `fetch` API.
* [#988](https://github.com/dotnet-websharper/core/issues/988) Add `Router.Fetch` as a fetch-based equivalent to `Router.Ajax`.

    ```fsharp
    type EndPoint =
        | [<EndPoint "GET /article-data">] GetArticleData of id: int
        
    type ArticleData = { id: int; (* ... *) }

    let router = Router.Infer<EndPoint>()

    promise {
        // Fetch /article-data/12...
        let! resp = Router.Fetch router (GetArticleData 12)
        // ... get its body as JSON...
        let! respJson = resp.Json()
        // ... and decode it as an instance of ArticleData.
        let article = Json.Decode<ArticleData> respJson
        Console.Log(article)
    }
    ```


## Fixes

* [#985](https://github.com/dotnet-websharper/core/issues/985) Fix reading C# tuple type signatures, this also fixes remoting with tuples in C#.
* [#989](https://github.com/dotnet-websharper/core/issues/989) RPC signature verification works as intended: compiler attempts to create JSON de/serializers, if there is any unsupported types found, a compile-time error is given.
* [#990](https://github.com/dotnet-websharper/core/issues/990) The `runngen.ps1` scripts in the `tools` folder in both `WebSharper.FSharp` and `WebSharper.CSharp` packages now correctly installs the compiler tools in the `net461` folder. Run this script in administrator mode to speed up WebSharper compilation.
* [#991](https://github.com/dotnet-websharper/core/issues/991) Fix regression in overriding implementation methods, there is no more incorrect `Instance member name conflict` errors.
* [#993](https://github.com/dotnet-websharper/core/issues/993) Fix scoping of "this" in function conversions. This notably fixes C# event handlers in WebSharper.UI.
* [#994](https://github.com/dotnet-websharper/core/issues/994) Fix control flow for C# try/catch in asynchronous methods.
* [#996](https://github.com/dotnet-websharper/core/issues/996) Don't use `jQuery.on('ready', ...)` for the web control activator. This removes the dependency on jQuery for many sitelets.
* [#997](https://github.com/dotnet-websharper/core/issues/997) Add proxy for `Printf.kprintf`.
* [#998](https://github.com/dotnet-websharper/core/issues/998) Don't serialize System.UI.Web.Control's private fields. Since the Serializable attribute requirement was dropped in 4.4.0, web controls would output a number of null fields in the `<meta>` tag; this is now fixed.

# WebSharper UI

## Fixes

* [#181](https://github.com/dotnet-websharper/ui/issues/181) `Elt.WithAttrs` is no longer public, it was intended to be internal.
* [#181](https://github.com/dotnet-websharper/ui/issues/181) `Elt.RemoveClass` removes unneeded spaces from `class` attribute value.
* [#182](https://github.com/dotnet-websharper/ui/issues/182) `jQuery` is now not used internally for `Elt.AddClass`/`RemoveClass` and `Router.Install`/`InstallInto`.
* [#184](https://github.com/dotnet-websharper/ui/issues/184) In sitelets using a template with `ClientLoad.FromDocument`, when outputting a `ws-replace` tag, make the tag name depend on the parent tag (eg. if the parent is `<tbody>`, output `<tr>`) instead of always `<div>`, so that the HTML is parsed correctly.

Happy coding!