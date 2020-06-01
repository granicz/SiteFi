---
title: "Bolero 0.7 released"
categories: "blazor,bolero,webassembly,fsharp,websharper"
abstract: "Updated for Blazor 3.0-preview7"
identity: "5790,87155"
---
We are happy to announce the release of [Bolero](https://fsbolero.io) version 0.7. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using [Blazor](https://blazor.net).

This release requires the .NET Core SDK version 3.0.100-preview7, which you can [download here for Windows, OSX or Linux](https://dotnet.microsoft.com/download/dotnet-core/3.0).

Install the latest project template with:

```shell
dotnet new -i Bolero.Templates
```

If you have an existing Bolero project, you can check [the upgrade guide](https://fsbolero.io/docs/Upgrade) to learn how to update your project for Bolero 0.7.

## Changes

In addition to the API changes inherent to the upgrade to Blazor 3.0-preview7, which you can read about in [the upgrade guide](https://fsbolero.io/docs/Upgrade), here is what's new in Bolero 0.7:

* Bolero.HotReload, the HTML template hot reload library, had been blocked from upgrade by a dependency; it is now available again for the latest Bolero.

* `Cmd.ofRemote` and its cousin `Cmd.performRemote` have been deprecated. We felt that these function names were misleading, because they are only useful when calling *authorized* remote functions. Remote functions without user authorization can be simply called with `Cmd.ofAsync` or `Cmd.performAsync`.  
    The new functions `Cmd.ofAuthorized` or `Cmd.performAuthorized` should now be used instead. They are identical to the previous `*Remote`, except that instead of passing the response as a custom type `RemoteResponse<'T>`, they use a simple `option<'T>`, which is `Some` on success and `None` in case of authorization failure.

Happy coding!