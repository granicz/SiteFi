---
title: "WebSharper 4.5.8 released with Visual Studio 2019 support"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.5.8 has client-side support for FSharp.Core 4.6 functions and templates for VS2019"
identity: "5729,86428"
---
WebSharper 4.5.8 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](https://websharper.com/downloads).

It contains client-side support for functions added to `FSharp.Core 4.6`, and allows installing templates for Visual Studio 2019 Preview.

Documentation: [WebSharper 4.x for C#](https://developers.websharper.com/docs/v4.x/cs) and [WebSharper 4.x for F#](https://developers.websharper.com/docs/v4.x/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/core/releases/tag/4.5.8.327).

# Fixes
* #1041 Add missing constructor to `WebSharper.JavaScript.Dom.MutationObserver`.
* #1029 No more build warnings about `An error occurred while reading the F# metadata node...`

# Enhancements
* #1043 Support new FSharp.Core 4.6 functions on the client side (`ValueOption` type and module, `List/Seq/Array.tryExactlyOne`).
* Updated to use `FSharp.Compiler.Service` version `26.0.1`. Anonymous records are not supported yet on the client side.
* Visual Studio templates now support 2019 Preview.

**Happy coding!**

![WebSharper templates in VS2019](https://i.imgur.com/Tn08xr6.jpg)