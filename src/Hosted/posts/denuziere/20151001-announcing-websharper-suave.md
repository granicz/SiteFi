---
title: "Announcing WebSharper.Suave"
categories: "suave,fsharp,web,websharper"
abstract: "We are happy to announce the release of WebSharper.Suave, an adapter to run WebSharper applications on Suave."
identity: "4556,80384"
---
We are happy to announce the release of WebSharper.Suave, an adapter to run WebSharper applications on [Suave](http://suave.io).

With WebSharper.Suave, creating a Suave `WebPart` from a WebSharper `Sitelet` is as simple as a single function call.

[![](http://i.imgur.com/9X5yr7Ym.png)](http://i.imgur.com/9X5yr7Y.png)

```fsharp
module Main

open WebSharper
open WebSharper.UI.Next.Html

[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.UI.Next.Client
    open WebSharper.Charting

    let RadarChart () =
        let labels =    
            [| "Eating"; "Drinking"; "Sleeping";
               "Designing"; "Coding"; "Cycling"; "Running" |]
        let data1 = [|28.0; 48.0; 40.0; 19.0; 96.0; 27.0; 100.0|]
        let data2 = [|65.0; 59.0; 90.0; 81.0; 56.0; 55.0; 40.0|]

        let ch =
            Chart.Combine [
                Chart.Radar(Seq.zip labels data1)
                    .WithFillColor(Color.Rgba(151, 187, 205, 0.2))
                    .WithStrokeColor(Color.Rgba(151, 187, 205, 1.))
                    .WithPointColor(Color.Rgba(151, 187, 205, 1.))

                Chart.Radar(Seq.zip labels data2)
                    .WithFillColor(Color.Rgba(220, 220, 220, 0.2))
                    .WithStrokeColor(Color.Rgba(220, 220, 220, 1.))
                    .WithPointColor(Color.Rgba(220, 220, 220, 1.))
            ]
        Renderers.ChartJs.Render(ch, Size = Size(400, 400))

open WebSharper.Sitelets
open WebSharper.UI.Next.Server

let MySite =
    Application.SinglePage (fun ctx ->
        Content.Page(
            Body = [
                h1 [text "Charting on Suave is easy with WebSharper!"]
                div [client <@ Client.RadarChart() @>]
            ])
    )

open Suave.Web
open WebSharper.Suave

startWebServer defaultConfig (WebSharperAdapter.ToWebPart MySite)
```

This opens up awesome ways to combine Suave and WebSharper applications, and we are excited about the synergies this will bring to the F# web programming community.

Happy coding!