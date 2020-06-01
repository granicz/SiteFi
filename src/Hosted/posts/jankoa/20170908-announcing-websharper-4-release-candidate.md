---
title: "Announcing WebSharper 4 Release Candidate"
categories: "c#,javascript,f#,websharper"
abstract: "Full stack of `WebSharper.4.0-rc` packages are now available on NuGet."
identity: "5414,83295"
---
WebSharper 4 was a long journey.
Started from the initial idea to add C# support and move to read F# source code directly for covering F# language features better,
a lot of other new features requested by the community or needed for ongoing projects were added to make development with WebSharper even more powerful.

Today we are publishing the full stack of `WebSharper` libraries, compilers and extensions with the `4.0-rc` version.
Feature-wise these are the same as the latest `Zafir` (WebSharper 4 codename) releases.
WebSharper 4 is open source under the Apache 2.0 License. Contributions are welcome at the [GitHub repository](https://github.com/intellifactory/websharper/).

For updating previous WebSharper 3 projects and breaking changes, look for the [upgrade guide](https://developers.websharper.com/docs/ws36to40).

We are continuing to round out the [documentation](https://developers.websharper.com/docs) with more help for beginners.
Your questions are welcome on the [forums](https://forums.websharper.com/forum/questions).
All the source for the documentation browser are also available on [GitHub](https://github.com/intellifactory/websharper.docs), please feel free to submit issues and pull requests.

[Try WebSharper](http://try.websharper.com/) has also been updated, test out WebSharper for both languages right in your browser.

You can download the [VSIX installer](http://websharper.com/downloads) for Visual Studio 2015/17 or download a template straight from
the [documentation subsite](https://developers.websharper.com/docs), look for "Create a project" in the top-right drop-down menu.

Big thanks to everyone who has been testing WebSharper 4 beta and submitted bug reports, questions and feature ideas!

# Biggest new features

* A C#-to-JavaScript compiler, up to date with C# 7.1, fully compatible both ways with F# libraries. Code analyzer for WebSharper-specific errors as you code (Requires VS 2017 Update 3).

```csharp
    [Remote] // Runs on the server
    public static Task<string> GetMessage() => Task.FromResult("Hello world!");

    [JavaScript] // Runs on the client, seamless type-safe asynchronous call
    public static async Task SayHello() => WebSharper.JavaScript.Console.Log(await GetMessage());
```

* Many new .NET framework features are usable client-side, including delegates, Tasks (usable for remote calls too), Linq methods. 

```csharp
    ul( // Create HTML list from a query. Syntax is reusable for both server and client
        (from p in people
        where p.Age > 18
        select li(attr.@class("person-name"), p.Name)).ToArray()
    );
```

* All F# language features are now supported, up to date with F# 4.1, including object expressions, `byref` and `&` operator, inner generic functions, pattern matching on arrays, statically resolved type parameters, struct unions and records.
    While WebSharper 3 focused on the functional programming aspects of F#, with the introduction of C# support, WebSharper 4 embraces object-oriented programming too for both languages.

```fsharp
	// Passing byref to C# and consuming a byref return
    // Now all working in JavaScript translation, so you can mix C# and F# anywhere
    let mutable myArr = [| 0; 1 |]
    let res : byref<int> = MyCSharpLibrary.ByRefReturn(&myArr.[0])
```

* Dead code elimination to create standalone libraries and single-page applications: when you compile a single-page application, WebSharper automatically removes all unused library functions from the JavaScript output,
which reduces greatly the download size.

# New templating engine

UI.Next has been updated with a newer templating syntax and engine that allows declaratively reusing sub-templates and has more flexibility for filling in holes while keeping strong typing.
For details, see the [documentation](https://developers.websharper.com/docs/ui.next-templating).

Here is a sample todo application with beautifully small code required [in C#](https://try.websharper.com/example/todo-list-csharp) and [in F#](https://try.websharper.com/example/todo-list).

# Other new features

* Erased union and option types have been added to `WebSharper.JavaScript` namespace, named `Union` and `Optional`. These are similar to `Choice` and `Option` types of F#, but work as TypeScript's erased unions in the JS translation. Conversion functions are available under modules `Optional` and `Union`.
* Classes now have reference equality by default (you can override Equals as in .NET). Arrays, tuples, F# records and unions still use structural equality.
* Automatic download of remote web resources to serve from locally. Set <WebSharperDownloadResources>True</WebSharperDownloadResources> in your project file to have WebSharper download all remote js/css defined in current project and all references. Add
<add key="UseDownloadedResources" value="True" /> to your <appSettings> section in web.config.
* Auto-hash WebSharper-generated output and files included as WebResource, use this hash on generating links to avoid browsers caching an outdated version.
* Many code translation optimizations. For example: functions taking F# tupled or curried functions as parameters and only fully applying them (in the case of curried parameter) now translates to taking a flat JavaScript function. This means less closure creation and better performance when using functions like `List.map2`. This is done by analyzing uses of the function parameter, not making any assumptions.
* And many more, see [list of beta releases](https://github.com/intellifactory/websharper/releases) on GitHub for full details.

# Coming soon

For the 4.0 stable release, we will publish a site to browse what .NET types and methods WebSharper translation cover via proxies. We will use this ourselves to expand coverage where lacking.
We will also address bug reports coming in for the RC release.

High on our priority list for 4.x versions:

* .NET Core and Standard support
* Web workers with a type-safe communication
* TypeScript 2.5 interoperability: generate .NET wrapper classes for `.d.ts` declarations, output `.d.ts` for WebSharper code
* WebAssembly interoperability: generate .NET wrapper classes for `.wasm` files and compile a subset of the languages to WebAssembly

Discuss upcoming features and make feature requests on the [WebSharper forums](http://forums.websharper.com/forum/featured).

## Happy coding!