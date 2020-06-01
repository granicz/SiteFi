---
title: "Bolero 0.3 released with HTML template hot reloading"
categories: "bolero,webassembly,fsharp,websharper"
abstract: "Designing a Bolero page is much quicker with HTML content live editing."
identity: "5728,86414"
---
We are happy to announce the release of Bolero version 0.3. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using Blazor.

Install the latest project template with:

```
dotnet new -i Bolero.Templates
```

# HTML Hot reloading

Version 0.3 brings HTML hot reloading to Bolero. You can now run your application in debug mode and see changes to your HTML templates instantly.

![Hot reloading in action](https://raw.githubusercontent.com/wiki/fsbolero/Bolero/hotreload.gif)

Hot reloading works both in client-side mode (WebAssembly) and in server-side mode. When in client-side mode, it still requires a server to be present: file changes are propagated to the application via SignalR.

Hot reloading is enabled by default in projects created with Bolero.Templates 0.3. You can opt out by calling:

```
dotnet new bolero-app --hotreload=false
```

To add hot reloading to an older project, or if you opted out when creating your project, [check the documentation](https://github.com/fsbolero/Bolero/wiki/Templating#hot-reloading) to learn how to add it. It requires adding NuGet packages and a bit of code to both the client-side and the server-side projects.

Happy coding!