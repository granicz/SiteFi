---
title: "WebSharper 3.0 RC released"
categories: "fsharp,javascript,web,websharper"
abstract: "We are finally closing in on the final WebSharper 3.0 with this release candidate."
identity: "4247,79011"
---
Today we just pushed out our release candidate for WebSharper 3.0. This means that WebSharper 3.0 is now in **feature freeze**, and we will only be releasing bug fixes until the stable release of WebSharper. So if you were holding out on switching to 3.0 because of continuous breaking changes, now is the time to try it out!

WebSharper 3.0-rc1 is [available for download](//websharper.com/downloads) right now.

We also made large enhancements to the online documentation; check out the sections on [Sitelets](//websharper.com/docs/sitelets), [HTML combinators](//websharper.com/docs/html-combinators), [WIG](//websharper.com/docs/wig) and [UI.Next](//websharper.com/docs/ui.next). More will be coming until the stable release.

## Change log

Here is the list of changes:

* Homogenize `WebSharper.Html.Client`, `WebSharper.Html.Server` and `WebSharper.UI.Next.Html`: the HTML combinators are now generated from a list of standard HTML tag names and attributes, to ensure consistency between the client and server libraries.

* [#337](https://github.com/intellifactory/websharper/issues/337): Exception when building WebSharper itself in debug mode.

* Html.Client:
    * Mouse and keyboard events have an `Event` field to access the underlying `Dom.Event`.
    * Add `OnEvent` to bind a custom event by name.

* UI.Next:
    * `Doc` now implements `WebSharper.Web.IControlBody`. This means that it can be used directly as the `Body` of a `Web.Control`:

    ```fsharp
    type MyControl() =
        inherit Web.Control()

        [<JavaScript>]
        override this.Body =
            let rvText = Var.Create ""
            Doc.Concat [
                Doc.Input rvText
                Label [] [Doc.TextView rvText.View]
            ]
            :> _
    ```

* JQuery:
    * Make the types of `.on()`, `.off()` and `.one()` more consistent with other event handlers such as `.click()`, passing the element and the event to the callback and returning the `JQuery`:
    
    ```fsharp
    JQuery.Of("#my-input")
        .On("paste", fun el ev ->
            JS.Alert ("Text pasted, value is now: " + JQuery.Of(el).Val()))
        .Ignore
    ```

    * Add `Event.AsDomEvent`.
    * Fix `JQuery.GetJSON` inline.

* WebSharper.JavaScript:
    * More accurate `Dom.Event` class hierarchy.
    * [#206](https://github.com/intellifactory/websharper/issues/206): Add `JS.Prompt()`.
    * [#356](https://github.com/intellifactory/websharper/issues/356): Move `|>!` from `WebSharper.JavaScript` to `WebSharper`, as it can be useful in some server-side scenarios.

* Sitelets:
    * [#327](https://github.com/intellifactory/websharper/issues/327): Allow using all standard integer, float and decimal types in `Sitelet.Infer`.
    * [#355](https://github.com/intellifactory/websharper/issues/355): Fix `Sitelet.Embed` deadlock.
    * [#357](https://github.com/intellifactory/websharper/issues/357): In JSON deserialization, `null` can be used to represent `None`.
    * [#363](https://github.com/intellifactory/websharper/issues/363): Fix `Template.LoadFrequency.WhenChanged`.

* Remoting:
    * [#361](https://github.com/intellifactory/websharper/issues/361): Show a warning when using a synchronous `[<Rpc>]` function.
    * [#362](https://github.com/intellifactory/websharper/issues/362): Throw an error when an `[<Rpc>]` function has a generic type.

* WIG:
    * Removed access modifiers, as only public makes sense in a WIG definition anyway.
    * Deprecated `Type.New()`. It was mainly used for two purposes:
        * Self-reference: you can use `TSelf` instead.
        * Mutually recursive types: the recommended way is to name classes first and add members later:

    ```fsharp
    let Chicken' = Class "Chicken"
    let Egg =
        Class "Egg"
        |+> Instance [
            "hatch" => T<unit> ^-> Chicken'
        ]
    let Chicken =
        Chicken'
        |+> Instance [
            "lay" => T<unit> ^-> Egg
        ]
    ```

    * Added `Pattern.RequiredFields`, `Pattern.OptionalFields` and `Pattern.ObsoleteFields` to create configuration objects in a more compositional way:

    ```fsharp
    // Existing Pattern.Config:
    let MyConfig : Class =		     
        Pattern.Config "classname" {
            Required =        
            [		
                "name", T<string>	
            ]
            Optional = 
            [		
                "width", T<int>	
                "height", T<int>	
            ]
        }
        
    // New API:
    let MyConfig : Class =
        Class "classname"
        |+> Pattern.RequiredFields [
            "name", T<string>
        ]
        |+> Pattern.OptionalFields [
            "width", T<int>
            "height", T<int>
        ]
    ```

## Future plans

This release candidate means that the 3.0 stable will be coming soon. In fact, unless a big issue holds us up, we will be releasing WebSharper 3.0 as early as **next week**.

After that, here is a peek at what you can expect from future releases:

* TypeScript declarations: we will be stabilizing the support for outputting `.d.ts` declarations for generated JavaScript modules, which was an experimental feature so far.
* F# 4.0 support: as soon as F# 4.0 is finalized, we will provide proxies for the extended standard library.
* Source mapping: we will improve the support for [source maps](//websharper.com/blog-entry/4146), in particular reduce the need for many-to-many mapping which several browsers have trouble with.

Happy coding!