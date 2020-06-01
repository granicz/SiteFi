---
title: "WebSharper 4 beta-6 released"
categories: "csharp,fsharp,javascript,websharper"
abstract: "WebSharper 4 beta-6 contains updates to DOM and JQuery bindings, erased union types, JS output optimizations and fixes"
identity: "5278,82676"
---
WebSharper 4 beta-6 contains updates to DOM and JQuery bindings, erased union types, JS output optimizations and fixes. 

Visual Studio 2017, F# 4.1 and C# 7 support are coming soon.

# Improvements
- using FSharp.Compiler.Service `11.0.4`. To have optimal compilation speeds, go to a folder in a solution at `packages\Zafir.FSharp.4.0.155.11-beta6\tools` and run `./runngen` in PowerShell as administrator.
- [#650](https://github.com/intellifactory/websharper/issues/650) Bindings to DOM have been updated to current ECMA specification
- [#652](https://github.com/intellifactory/websharper/issues/652) Bindings to JQuery have benn updated to version 3.1.1
- [#660](https://github.com/intellifactory/websharper/issues/660) Erased union and option types have been added to `WebSharper.JavaScript` namespace, named `Union` and `Optional`. These are similar to `Choice` and `Option` types of F#, but work as TypeScript's erased unions in the JS translation. Conversion functions are available under modules `Optional` and `Union`.
- [#651](https://github.com/intellifactory/websharper/issues/651) output is using JavaScript strict mode
- [#550](https://github.com/intellifactory/websharper/issues/550) custom structs can now be used in RPC calls
- [#644](https://github.com/intellifactory/websharper/issues/644) F# unions with `Constant null` case can now be used in RPC calls
- [#642](https://github.com/intellifactory/websharper/issues/642) Local generic functions in F# compile when the compiler does not need the type parameter for custom code generation (macros). If it does, you get an error "Macro [name] would use a local type parameter. Make the inner function non-generic or move it to module level and mark it with the Inline attribute"
- [#648](https://github.com/intellifactory/websharper/issues/648) JavaScript code output optimizations: 
  - Functions taking F# tupled or curried functions as parameters and only fully applying them (in the case of curried parameter) now translates to taking a flat JavaScript function. This means less closure creation and better performance when using functions like `List.map2`
  - Local F# tupled and curried functions are now converted to flat functions in JavaScript where possible
- [#649](https://github.com/intellifactory/websharper/issues/649) Tail call optimization for F#:
  - `let rec` expressions with single or mutual recursion
  - Module or class (in default constructor) `let rec` expressions with single recursive function
  - Class members, calling itself but no other members of the same class annotated with `JavaScript` (so inlines do not opt-out of optimization). For instance members, the call must be on the current instance.
- [#655](https://github.com/intellifactory/websharper/issues/655) `Require` and `RemotingProvider` attributes can take additional object parameters. These will be passed on to the constructors of the resource class and the client-side `RemotingProvider` instance respectively.
- `WebSharper.Resources.BaseResource` is not an abstract class any more. This allows using it with the `Require` attribute without defining an extra type:
  
  ``` fsharp
  [<Require(typeof<Resources.BaseResource>, "//myurl.com/mylib.js")>]
  ```

# Fixes
- [#645](https://github.com/intellifactory/websharper/issues/645) Name conflict in `WebSharper.Sitelets.Content` from C#
- [#657](https://github.com/intellifactory/websharper/issues/657) Using `Name` attribute on properties with getter and setter now adds `set_` to the name of the setter to disambiguate them.
- [#633](https://github.com/intellifactory/websharper/issues/633) WIG-defined interfaces now can be inherited from with F#/C# code if they contain only method definitions without custom inline annotations. Calling interface members from `.d.ts`-based bindings now translate properly, but these interfaces can't be inherited from in your code (as they are using special call semantics).
- [#665](https://github.com/intellifactory/websharper/issues/665) Unpacking Scripts/Contents for web projects required the `WebProjectOutputDir` property, although previously assumed project root as default. This default has been restored for back-compatibility.
* [#668](https://github.com/intellifactory/websharper/issues/668) Printing F# compilation errors and warnings as compiling with `fsc.exe` would, respecting `nowarn`, `warn`, `warnon`, `warneserror` flags
* [#667](https://github.com/intellifactory/websharper/issues/667) Fix C# analyzer giving failure warnings on build
* [#669](https://github.com/intellifactory/websharper/issues/669) `Async.StartImmediate` and `Async.StartWithContinuations` now start the `async` immediately as in .NET, possibly finishing synchronously if they contain no bind/combine.
* [#671](https://github.com/intellifactory/websharper/issues/671) Fix a translation bug in optimization around calling curried instance members
* [#673](https://github.com/intellifactory/websharper/issues/673) integer type `Parse` and `TryParse` methods follow .NET semantics, checking boundaries and disallowing fractions
* [#677](https://github.com/intellifactory/websharper/issues/677) Fix using `Inline` on constructors calling into other constructors
* [#678](https://github.com/intellifactory/websharper/issues/678) Fix inlining JavaScript statements, and using `return` to produce a value
* [#679](https://github.com/intellifactory/websharper/issues/679) exception properties `Message` and `InnerException` should work for all exception types
* [#676](https://github.com/intellifactory/websharper/issues/676) `Queue` and `Stack` constructors taking a `seq<'T>` (`IEnumerable<T>`)

# Breaking changes
- Macro API: `MacroResult.MacroNeedsResolvedTypeArg` now needs the offending type parameter as a field. You can decide if a type is a type parameter of any kind by using the new `IsParameter` property.
