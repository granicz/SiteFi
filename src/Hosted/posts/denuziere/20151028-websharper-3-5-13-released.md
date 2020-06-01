---
title: "WebSharper 3.5.13 released"
categories: "fsharp,javascript,web,websharper"
abstract: "This release adds Visual Studio 2015 Enterprise Edition support, and several bug fixes and minor features."
identity: "4618,80568"
---
We are happy to announce the release of WebSharper 3.5.13, with minor features and bug fixes. Here is the full change log:

### Visual Studio Extension

* [#2](https://github.com/intellifactory/websharper.visualstudio/issues/2): Add supported product: Visual Studio 2015 Enterprise Edition.

### WebSharper

* The bundle command (a.k.a Single-Page Application) now extracts non-JS, non-CSS WebResources into `/Content`.

* [#482](https://github.com/intellifactory/websharper/issues/482): Fix the serialization of `None` and other values represented as `null` in .NET when passed to inline controls (`ClientSide <@ ... @>` in WebSharper.Html, `client <@ ... @>` in UI.Next).

* Don't fail silently when failing to initialize a Sitelet from a `[<Website>]`-annotated static property.

### WebSharper.UI.Next

* `Doc` is now an abstract class, rather than an interface.

* Most extension methods have been changed to C#-style extension methods.

* Add `Doc.Async : Async<#Doc> -> Doc`.

* [#52](https://github.com/intellifactory/websharper.ui.next/issues/52): Fix over-eager loading of assemblies in the templating type provider, which could cause conflicts due to file locking.

### WebSharper.UI.Next.Piglets

* Add `Doc.ShowErrors : View<Result<'T>> -> (list<ErrorMessage> -> Doc) -> Doc`, also as an extension method on `View<Result<'T>>`.

* Add `Doc.ShowSuccess : View<Result<'T>> -> ('T -> Doc) -> Doc`, also as an extension method on `View<Result<'T>>`.

Happy coding!