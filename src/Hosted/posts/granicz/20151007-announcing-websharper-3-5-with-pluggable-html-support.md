---
title: "Announcing WebSharper 3.5 with pluggable HTML support"
categories: "ui.next,fsharp,websharper"
abstract: "We are happy to announce the availability of WebSharper 3.5, bringing a couple important changes and numerous bug fixes. Most notably, this release brings alternate HTML implementations to your WebSharper applications, and while there are only two at the moment, new flavors and variations (both in syntax and capabilities), and integrations with third-party markup engines are coming your way soon."
identity: "4584,80431"
---
We are happy to announce the availability of WebSharper 3.5, bringing a couple important changes and numerous bug fixes.

Most notably, this release brings **alternate HTML implementations** to your WebSharper applications, and while there are only two at the moment, new flavors and variations (both in syntax and capabilities), and integrations with third-party markup engines are coming your way soon.

### Swappable HTML language support

[![](http://i.imgur.com/ny0JoTwl.png)](http://i.imgur.com/ny0JoTw.png)

WebSharper always shipped with standard HTML support in `Html.Client` and `Html.Server` for constructing client/server markup. In an effort to bring **additional markup functionality** and **new markup alternatives** for your applications, WebSharper 3.5 introduces a less tighly coupled setup and leaves the actual markup implementations to be chosen freely.

This means that WebSharper 3.5+ applications will reference their actual choice(s) of HTML implementations (won't be auto-referenced via WebSharper any longer), and these in turn can evolve separately from the main WebSharper toolset, giving a way to incorporate novel ideas and new capabilities in constructing markup.

#### Reactive applications with UI.Next

Going forward, you are encouraged to try the reactive HTML implementation shipped with [WebSharper.UI.Next](http://github.com/IntelliFactory/websharper.ui.next).  UI.Next also brings **enhanced templating capabilities** beyond those in the standard `Html.Server` templates, giving you **type-safe, reactive HTML templates** that are stunningly easy to work with. You can read about UI.Next [here](http://websharper.com/docs/ui.next), while the documentation for UI.Next templating will be added shortly.

You can also find a number of UI.Next-based project templates shipped in the main VSIX installer and the MonoDevelop/Xamarin Studio integration, to help you get started with developing reactive WebSharper applications.

#### Standard HTML support now available as a separate Nuget

The previous HTML support (`Html.Client` and `Html.Server`) has moved to a [separate repository](https://github.com/intellifactory/websharper.html) and Nuget package called `WebSharper.Html`, so to continue using this implementation you will need to reference `WebSharper.Html` in your 3.5+ applications.

### WebSharper + Suave template

Next to the recent [WebSharper.Suave announcement](http://websharper.com/blog-entry/4556/announcing-websharper-suave), WebSharper 3.5 now also ships a new project template for Visual Studio, MonoDevelop, and Xamarin Studio for running WebSharper applications on [Suave](http://suave.io).  This template implements the standard client-server WebSharper "sample" application, and is easy to modify to fit your needs for any full-stack F# web application running on Suave.

![](http://i.imgur.com/sTlLbObl.png)

### Full change log

The list of other changes and bug fixes to WebSharper and components since the previous release are the following:

#### WebSharper

 * [#476](https://github.com/intellifactory/websharper/issues/476): Add proxies for static members FSharpOption.Some and FSharpOption.None
 * [#474](https://github.com/intellifactory/websharper/issues/474): Add ability to set content type directly in Content.File
 
#### WebSharper.UI.Next

 * [#45](https://github.com/intellifactory/websharper.ui.next/issues/45): Rename Content.Doc to Content.Page
 * [#46](https://github.com/intellifactory/websharper.ui.next/issues/46): Restore Attr.ClassDynPred
 * [#47](https://github.com/intellifactory/websharper.ui.next/issues/47): Add Doc.RunAppend, RunPrepend
 * [#48](https://github.com/intellifactory/websharper.ui.next/issues/48): Templating: add .Elt() when template is a single element
 * [#49](https://github.com/intellifactory/websharper.ui.next/issues/49): Templating: pure-whitespace nodes are ignored

The released bits on are Nuget for you to try - and your feedback is welcome. The easiest way to get in touch is joining the [WebSharper chat room on Gitter](https://gitter.im/intellifactory/websharper).  See you there!

Happy coding!
