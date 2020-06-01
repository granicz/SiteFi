---
title: "WebSharper 4.2-beta released with .NET Core/Standard support"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.2-beta adds support for .NET Core and compiler configuration via json"
identity: "5502,84552"
---
WebSharper 4.2-beta is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](http://websharper.com/downloads).

It brings .NET Standard/Core 2.0 support, templates for `dotnet new`, and compiler options configurable by a `wsconfig.json` file.

Documentation pages has been updated for 4.2:

* [Install VS or dotnet templates](https://developers.websharper.com/docs/v4.x/fs/install)
* [Project templates overview](https://developers.websharper.com/docs/v4.x/fs/templates)
* [Configure WebSharper compilation via wsconfig.json](https://developers.websharper.com/docs/v4.x/fs/project-variables)

Full documentation:
[WebSharper 4.x for C#](http://developers-test.websharper.io/docs/v4.x/cs) and [WebSharper 4.x for F#](http://developers-test.websharper.io/docs/v4.x/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/core/releases/tag/4.2.0.214-beta).

[Questions](https://forums.websharper.com/forum/all), [issue reports](https://github.com/dotnet-websharper/core/issues) and [contributions](https://github.com/dotnet-websharper/core/pulls) are welcome.
You can also report issues and contribute to the documentation at the [dotnet-websharper/docs](https://github.com/dotnet-websharper/docs) repository.

# Enhancements

## .NET Standard and .NET Core support

This is the first version of WebSharper compiled for .NET Standard 2.0, and therefore compatible with .NET Core 2.0.

The compiler itself still runs on .NET Framework, due to the F# compiler's current limitations in particular when dealing with type providers. So having .NET 4.6.1 or Mono installed on development machine is still required; however the runtime is 100% .NET Standard 2.0 compatible.

## ASP.NET Core support

WebSharper can now run on top of ASP.NET Core thanks to [WebSharper.AspNetCore](https://github.com/dotnet-websharper/aspnetcore). The server-side runtime, including Sitelets as well as RPC functions, can be run on top of ASP.NET Core. 

## C# and F# `dotnet` project templates

Templates for .NET Core are now available for both C# and F#. They can be installed with the following command:

```
dotnet new -i WebSharper.Templates
```

There are four templates for each language (add `-lang c#` or `-lang f#` to choose, default is C#):

* `dotnet new websharper-web` creates an ASP.NET Core application with a WebSharper client-server site.

* `dotnet new websharper-spa` creates an ASP.NET Core application containing a WebSharper Single-Page Application.

* `dotnet new websharper-html` creates a WebSharper-generated static HTML website.

* `dotnet new websharper-lib` creates a WebSharper library, compiling C# or F# code to JavaScript.

By default, the project is named based on current folder. Add the `-n ProjectName` flag to override this.

Use `dotnet run` after creating a web, SPA or Html template to immediately see it compile and run. Web and SPA uses Kestrel, while Html opens up an Explorer window for the output folder. 

## Build configuration via `wsconfig.json` file

If a `wsconfig.json` file is found next to a WebSharper project, build settings are read from it, overriding project variable setting if one exists in both places. Existing project variables all have equivalents: these are

* `"Project"`: WebSharper project type (see [documentation](https://developers.websharper.com/docs/v4.x/fs/project-variables)).
* `"OutputDir"`: Output directory for web, SPA and Html projects.
* `"DCE"`: dead code elimination on/off (shortened from `WebSharperDeadCodeElimination`).
* `"SourceMap"`: source map generation on/off.
* `"WarnOnly"`: give WebSharper errors only as warnings (shortened from `WebSharperErrorsAsWarnings`).
* `"DownloadResources"`: download all remote js/css resources for web projects.
* `"AnalyzeClosures"`: give warnings on unintended captures in JS closures.

New settings:

* `"JavaScript"`: emulate usage of `JavaScript` attribute on assembly level, allowing compiling full projects/files without annotating in source.
* `"JSOutput"`/`"MinJSOutput"`: write out `.js`/`.min.js` to given file paths.

# Breaking changes

## .NET 4.6.1

In parallel to the new .NET Standard 2.0, the minimum requirement for running WebSharper on the .NET Framework has been bumped to v4.6.1.