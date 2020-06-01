---
title: "WebSharper 4.3 released with pure .NET Core support"
categories: "netcore,csharp,fsharp,javascript,websharper"
abstract: "With this release, mono is not required during compile time anymore."
identity: "5617,85409"
---
We are happy to announce the release of WebSharper 4.3.

The main highlight of this release is the **dropped requirement of Mono during compilation** on Linux and OSX. Both the F# and C# compilers are now compiled for .NET Core 2.0, in addition to the existing .NET Framework 4.6.1 versions, and therefore **the .NET Core SDK is now the only requirement.**

By default, compiling a .NET Framework project uses the .NET Framework version of the compiler, and compiling a .NET Core or .NET Standard project uses the .NET Core version of the compiler. To override this default, you can set the project property `<WebSharperUseNetFxCompiler>` to `True` (to use the .NET Framework compiler) or `False` (to use the .NET Core compiler).

Here is how to obtain WebSharper 4.3:

* Install the templates for .NET Core SDK: 

   ```
   dotnet new -i WebSharper.Templates::4.3.1.249
   ```

* Templates for Visual Studio 2017: [Download here.](http://websharper.com/installers/WebSharper.4.3.1.249.vsix)

Here are the full release notes:

# WebSharper Core

## Features

* [#941](https://github.com/dotnet-websharper/core/issues/941): Run the compiler on .NET Core.

* [#964](https://github.com/dotnet-websharper/core/issues/964): Add proxies for the new functions in F# 4.5:
    * `Async.StartImmediateAsTask`
    * `Seq/List/Array.transpose`
    * `ValueOption` type
    * `Map.TryGetValue`
    * `FuncConvert.FromFunc/FromAction`

* [#965](https://github.com/dotnet-websharper/core/issues/965): Add support for [C# 7.3 language features](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7-3):
    * `ref` local reassign
    * Expression variables in initializers
    * `==` and `!=` for tuple types

* [#929](https://github.com/dotnet-websharper/core/issues/929): The `WebSharper.Compiler` NuGet package, which provides the WebSharper compiler as a library, has been split in order to better handle the specific F# and C# use cases.

    * `WebSharper.Compiler.Common` contains the common compiler libraries.
    * `WebSharper.Compiler.FSharp` and `WebSharper.Compiler.CSharp` contain the compilers for their respective languages. In addition to both depending on `WebSharper.Compiler.Common`, they also have properly versioned dependencies on the F# Compiler Service and Roslyn, respectively.
    * `WebSharper.Compiler` still exists, but it is now a meta-package with no content of its own but which depends on both `WebSharper.Compiler.FSharp` and `WebSharper.Compiler.CSharp`. If you were using `WebSharper.Compiler` to compile only one language, then you are encouraged to switch to the corresponding language-specific package.

    Note that although the core WebSharper libraries are compatible with F# 4.1, the compiler requires the latest FSharp.Core 4.5.

* [#938](https://github.com/dotnet-websharper/core/issues/938): Allow macros to resolve `let`-bound variables.

    In an expression like `let y = x + 1 in macroedFunction y`, this allows the macro to retrieve and manipulate the expression `x + 1`, rather than just seeing `y`.

## Fixes

* [#963](https://github.com/dotnet-websharper/core/issues/963) Add missing proxies for F# numeric conversion functions: `int8`, `byte`, `int16`, `uint16`, `uint32`, `uint64`, `double`.

* [#966](https://github.com/dotnet-websharper/core/issues/966): Support the `Method` sitelet endpoint attribute on a class.

* [#968](https://github.com/dotnet-websharper/core/issues/968): `Task.Result` throws if accessed before the task is completed.

* [#969](https://github.com/dotnet-websharper/core/issues/969): Include inherited C# auto-property backing field in remoting.

# WebSharper UI

## Features

* Enable the templating type provider to run on the new .NET Core-compiled WebSharper compiler.

* [#180](https://github.com/dotnet-websharper/ui/issues/180): F# templating: it is now possible to use `.V` directly inside a `string`-typed hole or a `ws-var` hole.

    For example, given the following HTML template:

    ```html
    <input ws-var="FirstName" />
    <div>${LastName}</div>
    ```
    
    You can bind it using the following F# code:

    ```fsharp
    type Name = { First: string; Last: string }

    let myVar = Var.Create { First = "John"; Last = "Doe" }

    let myDoc =
        MyTemplate()
            .FirstName(myVar.V.First) // .V lensing into a Var<string>
            .LastName(myVar.V.Last)   // .V mapping into a View<string>
            // In WebSharper 4.2, this would have been written as:
            // .FirstName(Lens(myVar.V.First))
            // .LastName(V(myVar.V.Last))
            .Doc()
    ```

Happy coding!