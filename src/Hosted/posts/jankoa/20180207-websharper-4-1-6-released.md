---
title: "WebSharper 4.1.6 released"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.1.6 has enhancements for bundle projects and bug fixes"
identity: "5500,84533"
---
WebSharper 4.1.6 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](http://websharper.com/downloads).

It contains performance enhancements to the Bundle project type, as well as a new BundleOnly project type for when only `.js`/`.css` output are needed for a project and a `.dll` is not. Also serialization support for `DateTimeOffset`.

Documentation: [WebSharper 4.1 for C#](http://developers-test.websharper.io/docs/v4.1/cs) and [WebSharper 4.1 for F#](http://developers-test.websharper.io/docs/v4.1/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/websharper/releases/tag/4.1.6.207).

# Bundling 
* Bundle project output is now generated quicker.
* Also, if you have source mapping turned off (as default) and dead code elimination too (with `<WebSharperDeadCodeElimination>False</WebSharperDeadCodeElimination>`) then bundling will just concatenate already compiled JavaScript output for referenced assemblies instead of rewriting to a single scope, resulting in faster compilation speed.
* You can use the new project type value `<WebSharperProject>BundleOnly</WebSharperProject>` to have only the `.js`/`.css`/`.html` output for bundle projects. In the case of F#, this means that only a dummy `.dll` is created. For C#, the `.dll` is just not touched, it will contain no WebSharper-specific resources. This allows faster iterative development on bundle projects. Do not use `BundleOnly` if any other projects are referencing the current project. Most time is gained if that project itself (and not just references) contain large amount of code.

# Fixes/improvements
* More `DateTimeOffset` members now usable client-side. Both RPCs and custom JSON serialization are supporting `DateTimeOffset` values, in a way that is also cross-compatible with serialized `DateTime` values.
* F# module-level pattern matching with `let` now translates successfully. For example: `let a, b = 1, 2`
* `Sitelet.InferWithCustomErrors` and `Router.InferWithCustomErrors` work as intended (previously was throwing a null exception).
