---
title: "New WebSharper templates"
categories: "templates,f#,websharper"
abstract: "The recent release of WebSharper 3.4 also brought an update to most of the project templates shipped for Visual Studio, MonoDevelop, and Xamarin Studio."
identity: "4425,79908"
---
The [recent release of WebSharper 3.4](http://websharper.com/blog-entry/4422/websharper-3-4-released) also brought an update to most of the project templates shipped for Visual Studio, MonoDevelop, and Xamarin Studio.

As a quick glance, here is the template client-server (both UI.Next and the older Html type) application, with two simple pages (Home and About), demonstrating how to use master templates and how to make client-server calls.

[![](http://i.imgur.com/t2s8LTJl.png)](http://i.imgur.com/t2s8LTJ.png)

While the templates organize client and server aspects into separate files (`Client.fs`, `Remoting.fs`, `Main.fs`), here is all the code you need in a single file for the above application (the master template `Main.html` is not listed here):

```fsharp
namespace MyApplication

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About

module Server =

    [<Rpc>]
    let DoSomething input =
        let R (s: string) = System.String(Array.rev(s.ToCharArray()))
        async {
            return R input
        }

[<JavaScript>]
module Client =
    open WebSharper.UI.Next.Client
    open WebSharper.UI.Next.Html

    let Start input k =
        async {
            let! data = Server.DoSomething input
            return k data
        }
        |> Async.Start

    let Main () =
        let input = inputAttr [attr.value ""] []
        let output = h1 []
        div [
            input
            buttonAttr [
                on.click (fun _ _ ->
                    async {
                        let! data = Server.DoSomething input.Value
                        output.Text <- data
                    }
                    |> Async.Start
                )
            ] [text "Send"]
            hr []
            h4Attr [attr.``class`` "text-muted"] [text "The server responded:"]
            divAttr [attr.``class`` "jumbotron"] [output]
        ]

module Templating =
    open WebSharper.UI.Next.Html

    type MainTemplate = Templating.Template<"Main.html">

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
             liAttr [if endpoint = act then yield attr.``class`` "active"] [
                aAttr [attr.href (ctx.Link act)] [text txt]
             ]
        [
            li ["Home" => EndPoint.Home]
            li ["About" => EndPoint.About]
        ]

    let Main ctx action title body =
        Content.Doc(
            MainTemplate.Doc(
                title = title,
                menubar = MenuBar ctx action,
                body = body
            )
        )

module Site =
    open WebSharper.UI.Next.Html

    let HomePage ctx =
        Templating.Main ctx EndPoint.Home "Home" [
            h1 [text "Say Hi to the server!"]
            div [client <@ Client.Main() @>]
        ]

    let AboutPage ctx =
        Templating.Main ctx EndPoint.About "About" [
            h1 [text "About"]
            p [text "This is a template WebSharper client-server application."]
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.About -> AboutPage ctx
        )
```

### Future plans

We are planning to introduce additional tools to create WebSharper projects via these standard and other templates **without** needing the corresponding Visual Studio or Xamarin Studio plugins/extensions installed.  This will make it **super-easy to create WebSharper projects from within any context, IDE, or OS**. Keep an eye on this blog for upcoming announcements.

Happy coding!