---
title: "WebSharper 4 beta-7 released"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4 beta now has F#4.1 and C#7 language support"
identity: "5330,82881"
---
WebSharper 4 beta now has F# 4.1 and C# 7 language support.

Latest Visual Studio template installers compatible with VS2015/2017 is [available here](http://websharper.com/Zafir.vsix).

# Enhancements
* [#689](https://github.com/intellifactory/websharper/pull/689) Added support for F# 4.1 and C# 7 new language features.
* [#596](https://github.com/intellifactory/websharper/issues/596) Automatic download of remote web resources to serve from locally. Set `<WebSharperDownloadResources>True</WebSharperDownloadResources>` in your project file to have WebSharper download all remote js/css defined in current project and all references. Add 
`<add key="UseDownloadedResources" value="True" />` to your `<appSettings>` section in `web.config`.
* [#696](https://github.com/intellifactory/websharper/issues/696) Parameterless union cases of a JavaScript-annotated union translate to a singleton instance for performance. Equality checks are optimized for these cases to just check the tag.
* [#638](https://github.com/intellifactory/websharper/issues/638) Added `Warn` attribute, calls to annotated member will generate a warning wherever it is called from client-side code.
* [#693](https://github.com/intellifactory/websharper/issues/693) F# multi-type trait calls translate but still are not supporting overloads. F# `+` and `-` operators properly translate to the operator defined by the type.
* [#703](https://github.com/intellifactory/websharper/issues/703) Stub classes with a base class do not emit a new class overwriting outside one.

## UI.Next

* [#117](https://github.com/intellifactory/websharper.ui.next/issues/117) IE compatibility: use DomElement.parentNode, not parentElement.
* Implement the [revised templating engine](https://github.com/intellifactory/websharper.ui.next/blob/master/docs/Templates.md) for C#.

# Fixes
* [#697](https://github.com/intellifactory/websharper/issues/697) Union with `Constant` case will have no prototype implicitly (instance methods compiled to static)
* [#598](https://github.com/intellifactory/websharper/issues/598) RPC signature hash computing is now platform-independent.
* [#702](https://github.com/intellifactory/websharper/issues/702) C# conditional compilation symbols are properly respected for translation with WebSharper.
* [#705](https://github.com/intellifactory/websharper/issues/705) JQuery is linked from `https`