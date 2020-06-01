---
title: "Deploying WebSharper apps to Azure via GitHub"
categories: "github,azure,websharper"
abstract: "This article describes a basic WebSharper client-server application template that you can deploy to Azure via GitHub commits."
identity: "4368,79468"
---
More and more .NET developers come to appreciate functional programming, and this is really good news for F#.  A couple weeks ago Scott Hanselman ran a [short article](http://www.hanselman.com/blog/RunningSuaveioAndFWithFAKEInAzureWebAppsWithGitAndTheDeployButton.aspx) on running [Suave.io](http://suave.io) apps as Azure web apps deployed via GitHub.  His [setup](https://github.com/shanselman/suavebootstrapper) was the following:

 1. You configure your Azure web app to be sourced from a GitHub repository. This sets up a GitHub hook that notifies Azure on every commit.
 2. Your suave.io app consists of an F# script (`site\webserver.fsx`) inside an empty ASP.NET project with a `web.config`.
 3. On a commit, Azure executes a deployment script (`deploy.cmd` via `.deployment`) found in your project.
 4. This script uses Paket to restore FAKE and Suave, and uses FAKE to execute an F# script (`build.fsx`) by passing it to `fsi.exe` shipped with FAKE.
 5. This FAKE script can optionally execute various build tasks, if necessary, to build a site - in Scott's example it was just used as a placeholder script.
 6. Finally, the web app is copied to the right folder in your Azure web app.
 7. When starting, the app executes a "setup" step, configured as an `<httpPlatform>` directive. This in turn uses FAKE to execute `site\webserver.fsx`, which starts Suave.io and listens on a given port that is then mapped to the standard HTTP port.
 
This sequence is more complicated than it needs to be because it has to work around the lack of the F# tools and core libraries when setting up a new Azure web app deployment. Since Scott's article, I filed a ticket to [bundle the F# compiler and tools in a Nuget](https://github.com/fsharp/fsharp/issues/408), and thanks to Don Syme and Steffen Forkmann, that Nuget was out within a couple days.

Armed with that, we quickly put together a similar deployment setup for WebSharper client-server apps (other types of WebSharper apps are equally possible.) This has the obvious advantage that:

 * It uses WebSharper, so you can add **client-side** (e.g. JavaScript) **functionality** as well and **write it in F#**.
 * It produces an ASP.NET app that serves fine in Azure **without the need to run a separate server process** like Suave.io (although you can also create [OWIN-based self-hosted WebSharper applications](http://www.websharper.com/blog-entry/4098) and run them as in Scott's scenario.)
 * It no longer needs FAKE to compile F# apps or run F# scripts.

Loic blogged the technical details in his [WebSharper: From Zero to an Azure-deployed Web Application](http://websharper.com/blog-entry/4367) article, using a WebSharper implementation/clone of the "2048" game.  I also put together a simpler template that you can use for client-server applications, and it's available in...

## this [GitHub](https://github.com/intellifactory/ClientServer.Azure) repository

The template uses [Paket](http://fsprojects.github.io/Paket/) instead of Nuget, and you can build your project with `build.cmd` (you will need [curl](http://curl.haxx.se/download.html) installed and on your path). Run `build.cmd` before opening in Visual Studio as well.

You can also get started quickly by clicking here:

[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/?repository=https://github.com/intellifactory/ClientServer.Azure)

There is a single `main.fs` file, and it contains both the server and the client functionality:

```fsharp
namespace MyApplication

open WebSharper
open WebSharper.Sitelets

type Action =
    | [<CompiledName "">] Home
    | [<CompiledName "about">] About

module Server =
    [<Rpc>]
    let DoSomething input =
        let R (s: string) = System.String(List.ofSeq s |> List.rev |> Array.ofList)
        async {
            return R input
        }

[<JavaScript>]
module Client =
    open WebSharper.Html.Client

    let Main () =
        let input = Input [Attr.Value ""]
        let output = H1 []
        Div [
            input
            Button [Text "Send"]
            |>! OnClick (fun _ _ ->
                async {
                    let! data = Server.DoSomething input.Value
                    output.Text <- data
                }
                |> Async.Start
            )
            HR []
            H4 [Class "text-muted"] -- Text "The server responded:"
            Div [Class "jumbotron"] -< [output]
        ]

open WebSharper.Html.Server

module Skin =
    open System.Web

    type Page =
        {
            Title : string
            Menubar : Element list
            Body : Element list
        }

    let MainTemplate =
        Content.Template<Page>("~/Main.html")
            .With("title", fun x -> x.Title)
            .With("menubar", fun x -> x.Menubar)
            .With("body", fun x -> x.Body)

    let Menubar (ctx: Context<Action>) action =
        let ( => ) text act =
            LI [if action = act then yield Class "active"] -< [
                A [HRef (ctx.Link act)] -< [Text text]
            ]
        [
            LI ["Home" => Action.Home]
            LI ["About" => Action.About]
        ]

    let WithTemplate action title body : Content<Action> =
        Content.WithTemplate MainTemplate <| fun ctx ->
            {
                Title = title
                Menubar = Menubar ctx action
                Body = body ctx
            }

module Site =
    module Pages =
        let Home =
            Skin.WithTemplate Action.Home "Home" <| fun ctx ->
                [
                    H1 [Text "Say Hi to Azure"]
                    Div [ClientSide <@ Client.Main() @>]
                ]

        let About =
            Skin.WithTemplate Action.About "About" <| fun ctx ->
                [
                    H1 [Text "About"]
                    P [Text "This is a template WebSharper client-server application
                             that you can easily deploy to Azure from source control."]
                ]

    let Main =
        Sitelet.Infer (function
            | Action.Home -> Pages.Home
            | Action.About -> Pages.About
        )

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = [Action.Home; Action.About]

[<assembly: Website(typeof<Website>)>]
do ()
```

This uses basic WebSharper templating and a master `main.html` template and implements a two page sitelet application:

![](http://i.imgur.com/jaRxS2Q.png)

You can run it directly [here](http://websharper-clientserver.azurewebsites.net/).

Happy coding!
