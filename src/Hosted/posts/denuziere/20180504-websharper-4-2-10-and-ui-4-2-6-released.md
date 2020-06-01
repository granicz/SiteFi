---
title: "WebSharper 4.2.10 and UI 4.2.6 released"
categories: "ui,csharp,fsharp,websharper"
abstract: "This minor release brings bug fixes and a few features to WebSharper and UI."
identity: "5569,85081"
---
This minor release brings bug fixes and a few features to WebSharper and UI.

Install templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.2.10.234`

Download templates for Visual Studio 2017: http://websharper.com/installers/WebSharper.4.2.10.234.vsix

# WebSharper Core

## Improvements

* [#943](https://github.com/dotnet-websharper/core/issues/943) New option for `DownloadResources`: `WarnOnly`.
    * `true` tries to download resources, and throws an error if it fails.
    * `WarnOnly` tries to download resources, and shows a warning if it fails.
    * `false` does not download resources.

## Fixes

* [#940](https://github.com/dotnet-websharper/core/issues/940) Inferred sitelet: "GET /" union case prevents other cases from being parsed

# WebSharper UI

## Improvements

* [#171](https://github.com/dotnet-websharper/ui/issues/171) Templating: Support writing `<ws-*>` template instantiations from the server side with `.Doc(keepUnfilled = true)`, so that these instantiations can be applied on the client side.

# Fixes

* [#170](https://github.com/dotnet-websharper/ui/issues/170) Templating: Fix issue that prevented applying `<ws-*>` instantiation for a template declared later in the file in `ClientLoad.FromDocument` mode.
* [#176](https://github.com/dotnet-websharper/ui/issues/176) Routing: Recognize relative `href` path when parsing clicked links to apply client-side routing.
* [#177](https://github.com/dotnet-websharper/ui/issues/177) Fix code output when using `.V` on a `Var<Doc>`.

Happy coding!