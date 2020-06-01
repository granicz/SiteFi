---
title: "WebSharper 3.5.16 released"
categories: "fsharp,javascript,web,websharper"
abstract: "WebSharper 3.5.16 splits off the WebSharper.Testing framework into a separate package and fixes a few bugs."
identity: "4625,80630"
---
We are happy to announce the release of WebSharper 3.5.16 available through the usual channels. Here is the change log:

### WebSharper.Testing

The unit testing framework WebSharper.Testing has been split into a separate NuGet package.

This fixes the issue where QUnit was included as a script in Single-Page Applications even when it wasn't actually used.

### WebSharper

* [#486](https://github.com/intellifactory/websharper/issues/486): Reference the correct version of FSharp.Core in the MSBuild task. This fixes issues such as `Json.Serialize` failing to compile when running MSBuild from the command line.

* [#489](https://github.com/intellifactory/websharper/issues/489): Null argument exception when building on mono.

### WebSharper.UI.Next

* [#17](https://github.com/intellifactory/websharper.ui.next/issues/17): Add `try...with`, `try...finally` and `return!` to the `View.Do` computation expression.

* [#19](https://github.com/intellifactory/websharper.ui.next/issues/19): Add `View.Sequence : seq<View<'T>> -> View<seq<'T>>`.

* [#55](https://github.com/intellifactory/websharper.ui.next/issues/55): In the templating type provider, translate `<[CDATA[...]]>` blocks to `Doc.Verbatim`.

* [#56](https://github.com/intellifactory/websharper.ui.next/issues/56): Don't delete some text nodes when updating the view.

* [#57](https://github.com/intellifactory/websharper.ui.next/issues/57): Fix genericity error on the `<~` update operator in Notation.

Happy coding!