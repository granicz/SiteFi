---
title: "WebSharper 3.5.9 released"
categories: "fsharp,javascript,web,websharper"
abstract: "This is mostly a bugfix release, with a few shorthands added to the UI.Next API."
identity: "4601,80518"
---
We just released version 3.5.9 of the WebSharper stack. This is mostly a bugfix release, with a few shorthands added to the UI.Next API.

## Change log

### WebSharper

* [#477](https://github.com/intellifactory/websharper/issues/477): Fix translation to JavaScript of mutable variables used from a closure (F# 4.0 feature).

* [#478](https://github.com/intellifactory/websharper/issues/478): Fix encoding of web controls by the ASP.NET ScriptManager.

* [#479](https://github.com/intellifactory/websharper/issues/479): Fix currying of local functions in some edge cases.

* [#480](https://github.com/intellifactory/websharper/issues/480): Fix division involving large negative integers.

* [#481](https://github.com/intellifactory/websharper/issues/481): Include "Z" UTC indicator in server-side System.DateTime JSON serialization.

* Ignore final slash in table-based Sitelets routing, thus fixing WebSharper.Suave [#1](https://github.com/intellifactory/websharper/issues/1).

### WebSharper.Html

* [#1](https://github.com/intellifactory/websharper.html/issues/1): Restore Html.Server.Web.Control and fix the JSON encoding of any client-side controls it contains.

### WebSharper.UI.Next

* [#43](https://github.com/intellifactory/websharper.ui.next/issues/43): Use `keypress` instead of `keyup` for IE8-compatible input detection in the `Attr.Value` and `Doc.Input` families of functions. This makes it possible to use `on.keyup` for other purposes, such as catching Enter to submit the input.

* Add `Submitter.CreateOption : View<'T> -> Submitter<option<'T>>`. This creates a Submitter with initial value `None`, that maps the input view through `Some` when triggered.

* Rename the `View.Convert` and `Doc.Convert` families of functions as follows:

    ```
    View.Convert      -> View.MapSeqCached
    View.ConvertBy    -> View.MapSeqCachedBy
    View.ConvertSeq   -> View.MapSeqCachedView
    View.ConvertSeqBy -> View.MapSeqCachedViewBy
     Doc.Convert      -> Doc.BindSeqCached
     Doc.ConvertBy    -> Doc.BindSeqCachedBy
     Doc.ConvertSeq   -> Doc.BindSeqCachedView
     Doc.ConvertSeqBy -> Doc.BindSeqCachedViewBy
    ```
    
    The old versions still exist for backward-compatibility, but are marked obsolete.

* Add extension method equivalents on `View<'T>` for all of the above. They are overloaded: all variants of `View.MapSeqCached` have an equivalent `v.MapSeqCached()`, and all variants of `Doc.BindSeqCached` have an equivalent `v.DocSeqCached()`.  
    Additionally, `Doc.BindView` has an equivalent `v.Doc()`.

### Project Templates

The UI.Next project templates in the Visual Studio extension and the Xamarin Studio addin have been updated:

* The SPA project uses `People.View.DocSeqCached()` instead of `ListModel.View People |> Doc.Convert`.

* The Client-Server projects now use a reactive style for the small client-side code sample, instead of the previous imperative style.

Happy coding!