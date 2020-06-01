---
title: "WebSharper 3.2.22 released"
categories: "warp,fsharp,javascript,web,websharper"
abstract: "This is a bugfix release including numerous fixes for compilation to JavaScript in Warp applications."
identity: "4418,79766"
---
This new WebSharper version is a bugfix release. The main fixes concern dynamic compilation to JavaScript, which includes Warp applications.

#### Change log:

* More verbose error message when failing to compile a function to JavaScript.
* Fix dynamic compilation to JavaScript of generic methods on generic types.
* Fix [#444](https://github.com/intellifactory/websharper/issues/444): dynamic compilation to JavaScript of union type field access.
* Fix various proxies whose types didn't exactly match with the original function, and were accepted by standard compilation but not by dynamic compilation.
* Make TypeScript declaration output optional in FrontEnd.
* Fix [#445](https://github.com/intellifactory/websharper/issues/445): equality and comparison on arbitrary objects.
* Fix `Runtime.Inherit` for when the parent class isn't included in dependencies.
* Fix `[<Website>]` not recognized on Sitelet values.

Happy coding!