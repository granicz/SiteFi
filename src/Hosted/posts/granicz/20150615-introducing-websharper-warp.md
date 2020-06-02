---
title: "Introducing WebSharper Warp"
categories: "warp,f#,websharper"
abstract: "WebSharper Warp is a friction-less web development library for building scripted and standalone full-stack F# client-server applications. Warp is built on top of WebSharper and is designed to help you become more productive and benefit from the rich WebSharper features more quickly and more directly.  While Warp shorthands target the most typical applications (text, SPAs, multi-page) and easy exploration, you can extend your Warp applications with the full WebSharper capabilities at any time."
identity: "4409,79675"
---
[WebSharper Warp](https://github.com/intellifactory/websharper.warp) is a friction-less web development library for building scripted and standalone **full-stack** F# client-server applications. Warp is built on top of WebSharper and is designed to help you become more productive and benefit from the rich WebSharper features more quickly and more directly.  While Warp shorthands target the most typical applications (text, SPAs, multi-page) and easy exploration, you can extend your Warp applications with the full WebSharper capabilities at any time.

### Installing

To get started with Warp is super-easy, all you need is to open a new F# Console Application (or any other F# project type if you want to script applications), and add `WebSharper.Warp` to it:

```
Install-Package WebSharper.Warp
```

Or if you use [Paket](http://fsprojects.github.io/Paket/):

```
paket init
paket add nuget WebSharper.Warp
```

### Hello world!

The simplest Warp site just serves text and consist of a single endpoint (`/`), by default listening on `http://localhost:9000`.

```fsharp
open WebSharper

let MyApp = Warp.Text "Hello world!"

[<EntryPoint>]
do Warp.RunAndWaitForInput(MyApp) |> ignore
```

![](http://i.imgur.com/fZgqeKjl.png)

### Single Page Applications

While serving text is fun and often useful, going beyond isn't any complicated. Warp also helps constructing HTML.  In the most basic form, you can create single page applications (SPAs) using `Warp.CreateSPA` and WebSharper's server-side HTML combinators:

```fsharp
open WebSharper.Html.Server

let MySite =
    Warp.CreateSPA (fun ctx ->
        [H1 [Text "Hello world!"]])

[<EntryPoint>]
do Warp.RunAndWaitForInput(MySite) |> ignore
```

![](http://i.imgur.com/xYITvCql.png)

### Multi-page applications

Using multiple `EndPoints` and `Warp.CreateApplication`, you can define multi-page Warp applications.  When constructing the actual pages, `Warp.Page` comes handy - allowing you to fill the `Title`, `Head`, and the `Body` parts on demand.  `Warp.Page` pages are fully autonomous and will **automatically contain the dependencies of any client-side code used on the page**.

```fsharp
type Endpoints =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /about">] About

let MySite =
    Warp.CreateApplication (fun ctx endpoint ->
        let (=>) label endpoint = A [HRef (ctx.Link endpoint)] -< [Text label]
        match endpoint with
        | Endpoints.Home ->
            Warp.Page(
                Body =
                    [
                        H1 [Text "Hello world!"]
                        "About" => Endpoints.About
                    ]
            )
        | Endpoints.About ->
            Warp.Page(
                Body =
                    [
                        P [Text "This is a simple app"]
                        "Home" => Endpoints.Home
                    ]
            )
    )

[<EntryPoint>]
do Warp.RunAndWaitForInput(MySite) |> ignore
```

![](http://i.imgur.com/WMnmzIPl.png)

### Adding client-side functionality

Warp applications can easily incorporate client-side content and functionality, giving an absolute edge over any web development library. The example below is reimplemented from [Deploying WebSharper apps to Azure via GitHub](http://websharper.com/blog-entry/4368), and although it omits the more advanced templating in that approach (which is straightforward to add to this implementation), it greatly simplifies constructing and running the application.

```fsharp
module Server =
    [<Server>]
    let DoWork (s: string) = 
        async {
            return System.String(List.ofSeq s |> List.rev |> Array.ofList)
        }

[<Client>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.Html.Client

    let Main () =
        let input = Input [Attr.Value ""]
        let output = H1 []
        Div [
            input
            Button([Text "Send"])
                .OnClick (fun _ _ ->
                    async {
                        let! data = Server.DoWork input.Value
                        output.Text <- data
                    }
                    |> Async.Start
                )
            HR []
            H4 [Class "text-muted"] -- Text "The server responded:"
            Div [Class "jumbotron"] -< [output]
        ]

let MySite =
    Warp.CreateSPA (fun ctx ->
        [
            H1 [Text "Say Hi to the server"]
            Div [ClientSide <@ Client.Main() @>]
        ])

[<EntryPoint>]
do Warp.RunAndWaitForInput(MySite) |> ignore
```

![](http://i.imgur.com/9sPa4lzl.png)

### Taking things further

Creating RESTful applications, using client-side visualizations is just as easy. For a quick example, here is a Chart.js-based visualization using the `WebSharper.ChartJs` WebSharper extension:

```fsharp
[<Client>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.Html.Client
    open WebSharper.ChartJs

    let RadarChart () =
        Div [
            H3 [Text "Activity Chart"]
            Canvas [Attr.Width  "450"; Attr.Height "300"]
            |>! OnAfterRender (fun canvas ->
                let canvas = As<CanvasElement> canvas.Dom
                RadarChartData(
                    Labels   = [| "Eating"; "Drinking"; "Sleeping";
                                  "Designing"; "Coding"; "Cycling"; "Running" |],
                    Datasets = [|
                        RadarChartDataset(
                            FillColor   = "rgba(151, 187, 205, 0.2)",
                            StrokeColor = "rgba(151, 187, 205, 1)",
                            PointColor  = "rgba(151, 187, 205, 1)",
                            Data        = [|28.0; 48.0; 40.0; 19.0; 96.0; 27.0; 100.0|]
                        )
                        RadarChartDataset(
                            FillColor   = "rgba(220, 220, 220, 0.2)",
                            StrokeColor = "rgba(220, 220, 220, 1)",
                            PointColor  = "rgba(220,220,220,1)",
                            Data        = [|65.0; 59.0; 90.0; 81.0; 56.0; 55.0; 40.0|]
                        )
                    |]
                )
                |> Chart(canvas.GetContext "2d").Radar
                |> ignore
            )
        ]

let MySite =
    Warp.CreateSPA (fun ctx ->
        [
            H1 [Text "Charts are easy with WebSharper Warp!"]
            Div [ClientSide <@ Client.RadarChart() @>]
        ])

[<EntryPoint>]
do Warp.RunAndWaitForInput(MySite) |> ignore
```

![](http://i.imgur.com/9o7x2b1l.png)

### Scripting with Warp

When you add the `WebSharper.Warp` NuGet package to your project in Visual Studio, a new document tab will open giving the necessary boilerplate for using Warp in scripted applications.

For instance, the SPA example above can be written as an F# script and executed in F# Interative:

```fsharp
#I "../packages/Owin.1.0/lib/net40"
#I "../packages/Microsoft.Owin.3.0.1/lib/net45"
#I "../packages/Microsoft.Owin.Host.HttpListener.3.0.1/lib/net45"
#I "../packages/Microsoft.Owin.Hosting.3.0.1/lib/net45"
#I "../packages/Microsoft.Owin.FileSystems.3.0.1/lib/net45"
#I "../packages/Microsoft.Owin.StaticFiles.3.0.1/lib/net45"
#I "../packages/WebSharper.3.2.8.170/lib/net40"
#I "../packages/WebSharper.Compiler.3.2.4.170/lib/net40"
#I "../packages/WebSharper.Owin.3.2.6.83/lib/net45"
#load "../packages/WebSharper.Warp.3.2.10.13/tools/reference.fsx"

open WebSharper
open WebSharper.Html.Server

let MySite =
    Warp.CreateSPA (fun ctx ->
        [H1 [Text "Hello world!"]])

do Warp.RunAndWaitForInput(MySite) |> ignore
```

If you use Paket, then you should replace the `#`-lines above with this one:

```fsharp
#load "../packages/WebSharper.Warp/tools/reference-nover.fsx"
```

In FSI, you should see:

```
--> Added 'c:\sandbox\test\Library1\HelloWorld\../packages/Owin.1.0/lib/net40' to library include path
[... more lines ...]

[Loading c:\sandbox\test\Library1\packages\WebSharper.Warp.3.2.10.13\tools\reference.fsx]

namespace FSI_0004

Serving http://localhost:9000/, press Enter to stop.
```

You can then test this application as before:

![](http://i.imgur.com/xYITvCql.png)

### Getting help

Warp now has a chat room where you can ask questions, feel free to drop by:

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/intellifactory/websharper.warp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Happy coding!
