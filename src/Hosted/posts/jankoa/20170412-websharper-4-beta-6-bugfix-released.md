---
title: "WebSharper 4 beta-6 bugfix released"
categories: "c#,javascript,f#,websharper"
abstract: "Fixes for tail recursion optimizations, lighter representation of F# unions/records with no extra members"
identity: "5283,82728"
---
`Zafir.4.0.160.40-beta6` Is now available on NuGet, `.vsix` installers at [WebSharper downloads](http://websharper.com/downloads) ("Other versions" section).

Thanks to @cgravill and @amieres for submitting bug reports.

See this release on [GitHub](https://github.com/intellifactory/websharper/releases/tag/Zafir.4.0.160.40-beta6).

# Fixes
* #681 Return was missing so infinite loop was generated on a module-level `let rec` returning `unit`
* #682 Mark arguments transformed in tail recursion optimization as mutable to avoid incorrect inlining which could result in invalid JS code
* #680 Do not eta-reduce to expressions that has side effects so needs the closure
* #666 Ignore duplicate references when initializing Sitelets in non-ASP.NET contexts, fail only on WS assemblies with different versions.
* #684 Fix for JavaScript statements in `Inline` to not drop first statement if it was an expression
* #685 Different `Name` and `AssemblyName` in F# does not cause errors


# Improvements
* #683 You can now opt-out of dead code elimination for SPA projects. Use `<WebSharperDeadCodeElimination>False</WebSharperDeadCodeElimination>` in the project file or add `--dce-` to the `wsfsc.exe` command line.
* #686 Case test for erased unions with a case which is a plain object in JS now compiles, if there is no previous case that is also a plain object, then it gives a more specific error

# Breaking changes
* #670 By default, classes (including F# records and unions) with no methods translated to JS instance methods and having no base class are now translated not to have a prototype. JSON serializer was fixed to handle this. This is a performance optimization, but disallows type checks or inheriting from a class like this. So a new attribute `Prototype` was added to opt-in for generating a prototype for the type in the translation. Extra features: `Prototype(false)` forces the type to have no prototype, converting instance methods to static in translation.