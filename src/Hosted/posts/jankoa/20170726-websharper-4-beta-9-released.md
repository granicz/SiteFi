---
title: "WebSharper 4 beta-9 released"
categories: "csharp,fsharp,javascript,websharper"
abstract: "Compiler and output optimizations, auto-versioning scripts"
identity: "5376,83030"
---
Lates WebSharper 4 beta has compiler and output optimizations, auto-versioning scripts.

Latest Visual Studio template installers compatible with VS2015/2017 is [available here](http://websharper.com/Zafir.vsix).

# Fixes and Improvements

* [#722](https://github.com/intellifactory/websharper/issues/722) Auto-hash WebSharper-generated output and files included as `WebResource`, use this hash on generating links to avoid browsers caching an outdated version. This is an automatic feature, no configuration currently.
* [#725](https://github.com/intellifactory/websharper/issues/725) Client-side JSON can de/serialize custom classes. (`Serializable` attribute and default constructor are not required as on the server.)
* [#717](https://github.com/intellifactory/websharper/issues/717) Fixed exponential compile time on chained method/property calls
* [#716](https://github.com/intellifactory/websharper/issues/716) C# helper for constructing a custom plain JavaScript object: `new JSObject() { { "background-color", "#666" } }`
* [#709](https://github.com/intellifactory/websharper/issues/709) Remove unnecessary variables around inlines. Also stronger optimizations for conditionals, function `.apply` and array `.concat` to reduce translated code size and make it more readable.
* [#726](https://github.com/intellifactory/websharper/issues/726) Not creating .js file for an assembly containing only inlined code
* [#715](https://github.com/intellifactory/websharper/issues/715) Client-side JSON module is visible as `WebSharper.TypedJson` from C# to avoid name conflict.
* [#723](https://github.com/intellifactory/websharper/issues/723) `JavaScript(false)` on an interface opts-out of default interface handling on client-side, allows for more divergence of client/server behavior when needed.
* [#724](https://github.com/intellifactory/websharper/issues/724) optional analyzer for closures, helping to avoid some classes of memory leaks in JavaScript.

> There is an inconvenient source of memory leaks in most JavaScript engines: captured variables are tied to a Context object and even functions that do not use some (or any) variables from a Context still retain the whole object. This is described well [here](http://point.davidglasser.net/2013/06/27/surprising-javascript-memory-leak.html).
>
> Adding a `WebSharperAnalyzeClosures` project property gives warnings on these kinds of captures, helping to eliminate memory leaks.
>
> Usage: `<WebSharperAnalyzeClosures>True</WebSharperAnalyzeClosures>` or `<WebSharperAnalyzeClosures>MoveToTop</WebSharperAnalyzeClosures>`. The latter additionally moves all non-capturing lambdas to top level automatically (experimental).

# Breaking change

* Previously, having `[<Inline "myLibrary.doSomething()">]` and `[<Inline "$global.myLibrary.doSomething()">]` was working equivalently. Now access to global scope without the `$global` object is assumed to be initialized before the current script starts running (WebSharper itself takes care of this, if you use the `Require` attribute) and will not change, so it is safe to shorten. Whenever you want the exact call on the current global scope (`window` object), be sure to use `$global`.