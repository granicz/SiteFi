---
title: "WebSharper.UI.Next 3.4.19 with OnAfterRender"
categories: "ui.next,fsharp,javascript,reactive,web,websharper"
abstract: "This is a minor release of WebSharper UI.Next which provides the OnAfterRender feature from Html.Client."
identity: "4550,80189"
---
We just released WebSharper.UI.Next version 3.4.19 to NuGet. This minor release includes a functionality that is widely used with Html.Client and is now also available in UI.Next: OnAfterRender.

This handler allows you to register a callback to be called right after an element has been inserted into the DOM. This is very useful to run functionality from libraries that require an element to be rendered. For example, Google Maps require the container element to have its dimensions already computed. It is easy to create a map using the afterRender handler:

```fsharp
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.Google.Maps

let MyMap () =
    divAttr [
        attr.style "width: 500px; height: 500px;"
        on.afterRender (fun div ->
            let options =
                MapOptions(
                    Center = LatLng(47.4968222, 19.0548256),
                    Zoom = 12)
            let m = Map(div, options)
            ()
        )
    ] [text "Loading Google Maps..."]
```

![Rendered page.](http://i.imgur.com/SyfH98F.png)

Like all event handlers, afterRender is available both as an attribute `on.afterRender` and as a method `Elt.OnAfterRender`.

This is one more step accomplished towards our ultimate goal to completely replace Html.Client with UI.Next. Happy coding!