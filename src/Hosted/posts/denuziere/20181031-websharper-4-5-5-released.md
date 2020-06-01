---
title: "WebSharper 4.5.5 released"
categories: "ui,csharp,fsharp,websharper"
abstract: "Introducing dynamic templates for WebSharper.UI"
identity: "5678,86100"
---
We are happy to announce the release of WebSharper 4.5.5.

Download the project templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.5.5.283`

Or use the extension for Visual Studio 2017: http://websharper.com/installers/WebSharper.4.5.5.283.vsix

The main highlight of this release is the addition of dynamic templating: the capability to fill template holes using their name as a string, rather than a strongly typed method. This is done using the `.With()` method.

```fsharp
open WebSharper.UI.Templating

type MyTemplate = Template<"<div>${SayWhat} ${ToWhom}!</div>">

let helloWorld =
    MyTemplate()
        .SayWhat("Hello")        // Usual strongly typed fill
        .With("ToWhom", "world") // New dynamic fill
        .Doc()
```

Accompanying this is a new type `DynamicTemplate` which is not a provided type, and therefore can only be filled using `.With()`.

```fsharp
open WebSharper.UI.Templating

let helloWorld =
    DynamicTemplate("<div>${SayWhat} ${ToWhom}!</div>")
        .With("SayWhat", "Hello")
        .With("ToWhom", "world")
        .Doc()
```

Note that this feature is only available on the server side for now.

Full release notes:

## WebSharper

### Fixes

* [#1034](https://github.com/dotnet-websharper/core/issues/1034) Router: accept an empty final fragment if a route expects a string.

## WebSharper.UI

## Features

* [#175](https://github.com/dotnet-websharper/ui/issues/175) `<ws-*>` template instantiation is now implemented on the server side.
* [#200](https://github.com/dotnet-websharper/ui/issues/200) Server-side templating: `Var<string>` holes are fully bound on the client side, including dynamically bound to `${Text}` holes with the same name in the same template.
* [#201](https://github.com/dotnet-websharper/ui/issues/201) Add dynamic holes for server-side templates. The method `.With("holeName", value)` now fills the given hole with the given value. An error is raised at runtime if the type of `value` is incompatible with the hole.
* [#201](https://github.com/dotnet-websharper/ui/issues/201) Add `DynamicTemplate` as a non-provided type similar to the provided `Template`, with the following differences:
    * `DynamicTemplate` is (for now) server-side only.
    * `DynamicTemplate` must be instantiated with a string argument.
    * `DynamicTemplate` holes can only be filled with `.With()`.
    * `DynamicTemplate` instantiation can only be finished with `.Doc()`.

## Fixes


* [#187](https://github.com/dotnet-websharper/ui/issues/187) C# templating: the build task doesn't write the generated C# file if it would be identical to the existing file. This makes it possible to use a file system watcher such as `dotnet watch` without running into an infinite loop.

Happy coding!