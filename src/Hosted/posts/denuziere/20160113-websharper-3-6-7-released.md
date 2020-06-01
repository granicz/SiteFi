---
title: "WebSharper 3.6.7 released"
categories: "suave,fsharp,javascript,web,websharper"
abstract: "This is a minor release with bug fixes and updates for dependent libraries."
identity: "4676,81085"
---
We are pleased to announce the release of WebSharper 3.6.7. This is a minor release with bug fixes and updates for dependent libraries. Here is the full change log.

### WebSharper

* [#512](https://github.com/intellifactory/websharper/issues/512): Fix JSON deserialization of `[<NamedUnionCases>]` union types in some situations.

### WebSharper.Suave

* [#8](https://github.com/intellifactory/websharper.suave/issues/8): Update to Suave 1.0.0.

### WebSharper.UI.Next

* [#65](https://github.com/intellifactory/websharper.ui.next/issues/65): Fix initial selected index in `Doc.Select`.
* [#66](https://github.com/intellifactory/websharper.ui.next/issues/66): Fix exception when calling a client function from server in `on.*`.

### WebSharper.JQueryUI

* [#8](https://github.com/intellifactory/websharper.jqueryui/pull/8): Fix `Autocomplete.Source`'s variants to use a union case. Thanks @juselius for your contribution!

Happy coding!