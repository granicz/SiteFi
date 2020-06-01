---
title: "WebSharper 3.0.59 released"
categories: "fsharp,javascript,web,websharper"
abstract: "This minor release adds Sitelets-related functionality and bug fixes."
identity: "4356,79382"
---
This is the first minor release since WebSharper 3.0 went live. Here is the change log:

* The types `Web.IContext` and `Sitelets.Context<_>` have a new member `Environment : IDictionary<string, obj>`. This property is used to pass host-specific data to sitelets and Rpc functions. Currently, this dictionary contains:
    * When running as an ASP.NET module (eg. Client-Server Application project template), `"HttpContext"` contains `System.Web.HttpContext.Current`.
    * When running on OWIN (eg. Self-Hosted Client-Server Application project template), `"OwinContext"` contains the current `IOwinContext`.

    Thanks to Catalin Bocirnea for this contribution on this!

* Added a `Sitelets.Content` creation helper: `Content.FromAsync : Async<Content<'T>> -> Content<'T>`

* Fixed [#391](https://github.com/intellifactory/websharper/issues/391): `Sitelet.Infer` would incorrectly match URLs longer than prescribed, for example a union case such as:

    ```fsharp
    | Article of id: int
    ```
    
    would not only accept urls such as:

    ```text
    /Article/123
    ```
    
    but also urls with any extraneous fragments afterwards:
    
    ```text
    /Article/123/something-extra
    ```
    
    Now it only accepts urls with the following formats:
    
    ```text
    /Article/123
    /Article/123/
    ```

As always, WebSharper 3.0.59 is available [on NuGet](http://www.nuget.org/packages/WebSharper/), and the installers for Visual Studio and Xamarin Studio / MonoDevelop are available [on the official website](http://websharper.com/downloads) and on the respective update channels.

Happy coding!