---
title: "WebSharper 4.2.4 released"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.1.6 has F# query on the client and net4x builds not depending on netstandard"
identity: "5510,84735"
---
WebSharper 4.2.4 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](https://websharper.com/downloads).

It contains new client-side support for F# `query` computation expressions and fixes to C# analyzer, using templates in Visual Studio 2017.6, and cleaned builds when targeting .NET 4.6.1+.

Documentation: [WebSharper 4.x for C#](https://developers.websharper.com/docs/v4.x/cs) and [WebSharper 4.x for F#](https://developers.websharper.com/docs/v4.x/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/core/releases/tag/4.2.4.247).

# Improvements

* You can now use `query` expressions in client-side F# code.

```fsharp
    [<JavaScript>]
    let countOddAndEven numbers =
        query {
            for x in numbers do
            groupBy (x % 2) into g
            select (g.Key, g.Count())
        }
```

# Fixes

* Libraries created with Interface Generator targeting .NET 4.x are no longer referencing `netstandard.dll`.
* A full stack of WebSharper extensions has been released with this fix, so any latest WebSharper binaries targeting .NET 4.6.1 are no longer depending on anything targeting .NET Standard.
* Fixed C# analyzers for WebSharper errors and UI template code generation running immediately on template changes in Visual Studio 2017 (Update 5 or 6 needed).

	![Roslyn Analyzer in Visual Studio](https://i.imgur.com/ZGX0XmF.png)
* F# templates for Visual Studio now use `FSharp.Core` from NuGet, and runs out of the box for Visual Studio 2017 Update 6.