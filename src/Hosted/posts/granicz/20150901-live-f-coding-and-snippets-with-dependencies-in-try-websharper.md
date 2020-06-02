---
title: "Live F# coding and snippets with dependencies in Try WebSharper"
categories: "trywebsharper,fsharp,websharper"
abstract: "Try WebSharper reached an important milestone today: we just released the first bits of on-the-fly typechecking and code completion, and you can now develop F# web snippets, without any installation, online more easily than ever."
identity: "4546,80161"
---
[Try WebSharper](http://try.websharper.com) reached an important milestone today: we just released the first bits of on-the-fly typechecking and code completion, and you can now develop F# web snippets, without any installation, online more easily than ever.

Here is what it looks like:

[![](http://i.imgur.com/XAoctQZl.png)](http://i.imgur.com/XAoctQZ.png)

You don't have to do anything fancy, just start typing and the type checker will guide you, including code completion with `Ctrl+Space` as you would expect.  When you are ready to run your snippet, hit Run and you will see your snippet run in the Result panel.

### Snippets with dependencies

You may also notice the little gear icon in the F# source tab, with that, now you can set up dependencies for your snippet.  Currently, we support a wide range of WebSharper extensions, with more coming soon:

|Type     | Packages     |
|:--------|:-------|
|**Charting**|`WebSharper.Charting`, `WebSharper.ChartJs`|
|**Visualization**|`WebSharper.D3`, `WebSharper.Google.Visualization`|
|**3D graphics**|`WebSharper.GlMatrix`, `WebSharper.O3D`|
|**Mobile UIs/apps**|`WebSharper.SenchaTouch`, `WebSharper.MaterialUI`|
|**WebSharper abstractions**|`WebSharper.Formlets`, `WebSharper.Piglets`|
|**Reactive development**|`WebSharper.React`, `WebSharper.UI.Next`|

We will add third-party extensions shortly, including a variant of `FSharp.Data` to enable data-aware snippets that communicate with web services, etc.

[![](http://i.imgur.com/35rYVevl.png)](http://i.imgur.com/35rYVev.png)

Even more enhancements will be coming shortly, until then happy coding!
