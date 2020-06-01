---
title: "WebSharper.UI 4.2.4 released"
categories: "ui,csharp,fsharp,websharper"
abstract: "This is a minor feature release of WebSharper.UI, out-of-band from WebSharper."
identity: "5517,84872"
---
We are happy to announce the release of [WebSharper.UI 4.2.4](https://www.nuget.org/packages/WebSharper.UI/4.2.4.114).

This is a minor feature release of WebSharper.UI, out-of-band from WebSharper.

# Features

* [#163](https://github.com/dotnet-websharper/ui/issues/163) Allow using the `on.afterRender` attribute, the `Elt.OnAfterRender()` method and the `ws-onafterrender` template hole from the server side just like other event handlers.

* [#165](https://github.com/dotnet-websharper/ui/issues/165) Add variadic overload for `Attr` and `Doc`-typed hole instantiation. For example, the following:

    ```fsharp
    MyTemplate()
        .Body(
            [
                h1 [] [text "Hello!"]
                p [] [text "Welcome to my page."]
            ]
        )
        .Doc()
    ```

    can now be written as:

    ```fsharp
    MyTemplate()
        .Body(
            h1 [] [text "Hello!"],
            p [] [text "Welcome to my page."]
        )
        .Doc()
    ```

# Fixes

* [#138](https://github.com/dotnet-websharper/ui/issues/138) Fix an exception thrown when editing a template file while the application is running and `serverLoad` is set to `ServerLoad.WhenChanged`.

Happy coding!