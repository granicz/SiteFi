---
title: "Bolero 0.5 released for Blazor 3.0-preview"
categories: "blazor,bolero,webassembly,fsharp,websharper"
abstract: "Updated to the latest and greatest Blazor and .NET Core"
identity: "5789,86911"
---
We are happy to announce the release of [Bolero](https://fsbolero.io) version 0.5. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using [Blazor](https://blazor.net).

Install the latest project template with:

```
dotnet new -i Bolero.Templates
```

## Blazor 3.0-preview

![Blazor 3.0](https://github.com/fsbolero/website/raw/3.0/src/Website/img/blazor-icon.png)

Bolero has been upgraded to be based on the Blazor 3.0 preview. This means that it now requires .NET Core 3.0-preview5, which you can install [here](https://dotnet.microsoft.com/download/dotnet-core/3.0).

If you have an existing Bolero 0.4 project, you can check [the upgrade guide](https://fsbolero.io/docs/Upgrade#from-v04-to-v05) to learn how to update your project for Bolero 0.5.

## dotnet template

We have also made changes to the dotnet project template. It now uses plain NuGet `PackageReference`s instead of Paket. You can still create a Paket-based solution by passing the `--paket` toggle:

```
dotnet new bolero-app --paket -o HelloWorld
```

## Other changes

* Removed a use of a `ThreadLocal` variable in `Remote.withContext`.

Happy coding!