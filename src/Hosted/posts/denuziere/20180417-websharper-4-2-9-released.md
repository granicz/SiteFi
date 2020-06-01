---
title: "WebSharper 4.2.9 released"
categories: "ui,csharp,fsharp,websharper"
abstract: "This is a minor release for WebSharper and WebSharper.UI."
identity: "5565,84998"
---
This is a minor release for WebSharper and WebSharper.UI.

Install templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.2.9.232`

Download templates for Visual Studio 2017: [http://websharper.com/installers/WebSharper.4.2.9.232.vsix](http://websharper.com/installers/WebSharper.4.2.9.232.vsix)

# WebSharper Core

## Enhancement

* [#921](https://github.com/dotnet-websharper/core/issues/921) Now JSON deserialization can handle the `System.Object` type, implemented as follows:
  * deserializing on the client side just returns the parsed value (ie. one of: `null`, a boolean, a string, a number, an array or a plain object).
  * deserializing on the server side produces a boxed value:
    * null, boolean, string return a value of the corresponding type.
    * number: returns a `float` (`System.Double`)
    * array: returns a recursively parsed `obj[]`
    * object: returns a recursively parsed `Dictionary<string, obj>`
  * serializing from `System.Object` is _not_ supported. This is to avoid accidentally upcasting the argument of `Json.Serialize` to `System.Object` when a type annotation is missing.

## Fixes

* [#935](https://github.com/dotnet-websharper/core/issues/935) `Seq.chunkBySize` and other similar functions does not fail erroneously on some inputs.
* [#934](https://github.com/dotnet-websharper/core/issues/934) If for a website project, there is no `"outputdir"` in `ws.config` or `WebSharperOutputDir` in project properties or a `web.config` file in the project folder, an explicit error is given that the unpack output directory cannot be determined.

# WebSharper UI

## Enhancement

* Add `Router.InstallInto` and `Router.InstallHashInto`, which are similar to `Router.Install` and `Router.InstallHash` respectively except that they take a `Var<'EndPoint>` as argument rather than creating and returning one.

## Fixes

* [#164](https://github.com/dotnet-websharper/ui/issues/164) Remove `click` event handler on `Doc.Checkbox`, only keeping `change`, to avoid setting the Var twice.
* [#166](https://github.com/dotnet-websharper/ui/issues/166) Add proxy for quotation-based `.OnXyz(...)` event handlers.
* [#167](https://github.com/dotnet-websharper/ui/issues/167) Fix initial value of `Var.Make` whenever an initial value is available from the `View`.
* [#168](https://github.com/dotnet-websharper/ui/issues/168) Add `View.TryGet : View<'T> -> option<'T>` which returns the current value of a `View` if available, or `None` if not (eg if a `View.MapAsync` hasn't returned yet).

Happy coding!