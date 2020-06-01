---
title: "The road to WebSharper 4"
categories: "c#,javascript,f#,websharper"
abstract: "We started work on WebSharper 4 more than a year ago, open-sourced it in May and published first beta packages in August. It is a project with a big scope and still some features are planned before we would call it stable. This article is an introduction and also a status report."
identity: "5203,82343"
---
We started work on WebSharper 4 more than a year ago, open-sourced it in May and published first beta packages in August.
It is a project with a big scope and still some features are planned before we would call it stable.
This article is an introduction and also a status report.

## What is WebSharper 4?

WebSharper is a toolset consisting of a .NET-to-JavaScript compiler, web framework and libraries.
WebSharper 4 adds support for tranlating C# and expands F# language and .NET main library coverage for client-side availability.
Main features are: 

* Write client and server code in a single language, or now with a mix of C# and F#. Communication between the client and server is transparent and type-safe.
* You can also specify exact translation of method with [attributes](http://websharper.com/docs#getting_started/attribute_reference/heading-1-3), this JavaScript code is checked for validity at compile-time or in code service.
* Lightweight JavaScript runtime and generated code. Simple .NET types translate to JavaScript built-in types. No reflection support and some types cannot be checked against in client-side code but these give compile-time warnings or errors.
* Metaprogramming: compile-time type information is used to generate output. Generating or transforming output code with custom logic is also possible with just a type definition and an [attribute](http://websharper.com/docs#getting_started/attribute_reference/heading-1-5).

### What is currently available?

WebSharper 4 beta packages are available under the codename `Zafir` on NuGet.
Go to [try.websharper.com](http://try.websharper.com/) to explore, write, test and share code snippets and mini-applications easily in both C# and F#.
New documentation is under work, hosted on [GitHub](https://github.com/intellifactory/websharper.docs) and browsable at [websharper.com/docs](http://websharper.com/docs). First full tutorial for C# newcomers presents a small [CRUD application](http://websharper.com/tutorials#web_development/book_collection_(c-sharp)).

### Releases

You can find previous change log of all beta releases on GitHub: 
[beta1](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.115.7-beta1), 
[beta2](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.119.10-beta2), 
[beta3](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.129.193-beta3), 
[beta3-bugfix](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.131.14-beta3),
[beta4](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.133.15-beta4),
[beta5](https://github.com/intellifactory/websharper/releases/tag/Zafir-4.0.151.28-beta5).

Current `vsix` installers are available under [Downloads]() in the "Other versions" section and here: [Zafir.FSharp.vsix](http://websharper.com/Zafir.FSharp.vsix),
[Zafir.CSharp.vsix](http://websharper.com/Zafir.CSharp.vsix)

## New features

* C#-to-JavaScript compiler fully compatible both ways with F# libraries. Code analyzer for giving you WebSharper-specific warnings and errors as you type.
* Many new .NET framework features are usable client-side, including delegates, Tasks (usable for remote calls too), Linq methods.
* Code dependency exploration for smaller output for single-page applications, with optional source mapping.

Track new releases on [GitHub](Track new releases at WebSharper releases on GitHub).

### F#-specific new features

* Not relying on ReflectedDefinition produces smaller .dll files and have improved compilation running time. `JavaScript` attribute now can be set on assembly level too, `[<JavaScript(false)>]` can remove a member or type from the compilation scope.
* All F# language features are now supported, including object expressions, `byref` and `&` operator, inner generic functions, pattern matching on arrays, statically resolved type parameters.
* Correct object-oriented behavior in JavaScript translation. WebSharper now fully supports method overrides, interface implementations, static constructors, base calls, constructor chaining, having no implicit constructor, self identifier on constructors.
* Module `let` values now work as in .NET, not all initialized in arbitrary order on page load, only on first access of a value from a single file.
* Better error reporting, translation failures are reported at the exact location of the expression.

For upgrading your WebSharper 3 projects, check out the [update guide](http://websharper.com/docs/WS4UpdateGuide.md).

## The future

There are a couple major features planned for the final release sometime in 2017, and also general quality improvements like API cleanup, more documentation and tutorials.

### Planned features

* C# 7 and F# 4.1 support. These include better interoperability between the two languages (newly added implicit conversions) which would simplify using from C# even those libraries in the WebSharper ecosystem or older projects which were not updated by hand to have C#-friendly overloads.
* Support for .NET Core by the WebSharper server runtime and libraries.
* TypeScript interoperability, including a code generator to generate C# or F# code from `.d.ts`, and `.d.ts` output for WebSharper projects.

### Planned optimizations

* Generated code optimizations for performance. For example transforming curried and tupled F# function arguments into multi-argument functions. The proxies for standard .NET classes are implemented in F# in WebSharper, so less function object creation by them would benefit C# WebSharper projects too.
* Compiler and server runtime performance. For example better metadata format that allows partial deserialization which would reduce compiler running time and server startup time.
* More .NET coverage, including client-side support for collection interfaces like `IDictionary` and better translation of type checks and conversions of value types.

Feedback and questions are welcome at the [WebSharper forums](http://websharper.com/questions) and issue reports on [GitHub](https://github.com/intellifactory/websharper/issues).

#### Happy coding!