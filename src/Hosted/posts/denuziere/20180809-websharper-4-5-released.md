---
title: "WebSharper 4.5 released"
categories: "csharp,fsharp,javascript,websharper"
abstract: "This release simplifies the use of HTML functions in WebSharper UI."
identity: "5624,85693"
---
We are happy to announce the release of WebSharper 4.5!

Install templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.5.0.265`

Download templates for Visual Studio 2017: [http://websharper.com/installers/WebSharper.4.5.0.265.vsix](http://websharper.com/installers/WebSharper.4.5.0.265.vsix)

---

The main highlight is a simplification of the HTML construction functions: `div`, `span`, `p`, etc. Until now, these functions returned type `Elt`, a subtype of the more general `Doc`. This often required upcasts to `Doc`, for example:

```fsharp
if someCondition then
    Doc.Empty
else
    div [] [text "test"] :> Doc
```

These functions now return type `Doc`, which means that the above can now be written as:

```fsharp
if someCondition then
    Doc.Empty
else
    div [] [text "test"]
```

If you do need to use the `Elt` API on an element, such as `.Dom` to retrieve the underlying DOM element, you can create it with eg. `Elt.div`, which returns `Elt` like `div` used to.

---

Full change list:

# WebSharper

## Features

* [#1001](https://github.com/dotnet-websharper/core/issues/1001): Strongly-name the .NET Standard core libraries.
* [#1002](https://github.com/dotnet-websharper/core/issues/1002): Added `--jsoutput` and `--minjsoutput` command line options, as well as `WebSharperJsOutput` and `WebSharperMinJsOutput` project properties, as equivalents to `jsOutput` and `minJsOutput` `wsconfig.json` options.

## Fixes

* [#1000](https://github.com/dotnet-websharper/core/issues/1000): Compiling in `BundleOnly` mode correctly detects compile errors.

# WebSharper UI

## Features and breaking changes

* [#183](https://github.com/dotnet-websharper/ui/issues/183): [F#] In order to minimize the number of upcasts needed in user code, the following functions now return values of type `Doc` instead of `Elt`:

    * Functions in the module `WebSharper.UI.Html`, such as `div`, `span`, etc.
    * Functions in the module `WebSharper.UI.Html.Tags`, such as `option`, `object`, etc.
    * Functions in the module `WebSharper.UI.Html.SvgElements`, such as `g`, `rect`, etc.
    * Functions in the module `WebSharper.UI.Client.Doc`, such as `Input`, `Button`, etc.

    For users who do need values of type `Elt`, the following were added:

    * A new module `WebSharper.UI.Html.Elt`, containing `Elt`-returning equivalents to the functions in `WebSharper.UI.Html` and `WebSharper.UI.Html.Tags`.
    * A new module `WebSharper.UI.Html.SvgElements.Elt`, containing `Elt`-returning equivalents to the functions in `WebSharper.UI.Html.SvgElements`.
    * A new module `WebSharper.UI.Client.Elt`, containing `Elt`-returning equivalents to the functions in `WebSharper.UI.Client.Doc`.

# Fixes

* [#112](https://github.com/dotnet-websharper/ui/issues/112): Pass the correct `Dom.Element` as argument to an `.OnAfterRender()` called on a template's `.Elt()`.
* [#185](https://github.com/dotnet-websharper/ui/issues/185): Fix setting the class of an SVG element.

Happy coding!