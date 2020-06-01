---
title: "WebSharper 2.1 Beta 5 available"
categories: "f#,websharper"
abstract: "Today we are happy to announce WebSharper 2.1 Beta 5, with substantial new features and a major redesign of WebSharper sitelets, among others.  Now with all major features in place and available, we are ramping up to ship the final WebSharper 2.x Professional in the coming weeks."
identity: "998,74579"
---
(**Update**: there was a glitch in 2.1.18 - please upgrade to the latest version, and please make sure you remove all previous 2.x installations)

Today we are happy to announce WebSharper 2.1 Beta 5, with substantial new features and a major redesign of WebSharper sitelets, among others. Now with all major features in place and available, we are ramping up to ship the final WebSharper 2.x Professional in the coming weeks.

You can get download Beta 5 from the [WebSharper home page](http://www.websharper.com/), or directly from [this link](http://www.websharper.com/latest/ws2).

### Pure HTML/JavaScript Applications

In addition to ASP.NET, ASP.NET MVC, and client-server sitelets-based applications, WebSharper 2.1 Beta 5 ships with the tool set and Visual Studio templates that enable you to develop pure HTML/JavaScript applications. These applications execute entirely on the client and can be deployed in any web container not just IIS. They can be developed elegantly with the new sitelets API which enables you to express static HTML pages with dynamic JavaScript functionality more easily than ever before.

### Sitelets on Steroids

The new sitelets API has gone through a major redesign and we will be blogging extensively about the new features in the coming days. If you can't wait to hear all the details and happen to be in Paris next week, come to my [TechDays Paris 2011](http://www.microsoft.com/france/mstechdays/) talk on Feb 8. We will also be hosting a web cast on sitelets shortly - stay tuned for the date and further details.
Besides being able to represent compositional websites, express RESTful services, abstract over any response content type, sitelets also allow automating all URL-related chores:

 * Infer URLs from Actions directly
 * Provide type-safe access to parts of a URL (stay tuned for some awesome examples!) - extracting typed data has never been easier!
 * Always yield safe URLs - e.g. managed URLs that are guaranteed to be valid.

Here is an all-inclusive example of a three-page website (an index page with links to the other pages, a static page, and another static page with some dynamic content) with explicit URLs. The resulting sitelet can be served from IIS, composed with legacy ASP.NET applications seamlessly, or distilled into HTML/JS files that are ready to be deployed in any web server.

```fsharp
namespace SampleWebsite

open System
open System.IO
open System.Web
open IntelliFactory.WebSharper.Sitelets

/// Defines a sample HTML site with nested pages
module SampleSite =
    open IntelliFactory.WebSharper
    open IntelliFactory.Html
    
    // Action type to represent requests
    type Action =
        | Index
        | Page1
        | Page2

    // Module containing JavaScript controls
    module Client =
        open IntelliFactory.WebSharper.Html

        type MyControl() =
            inherit IntelliFactory.WebSharper.Web.Control ()

            [<JavaScript>]
            override this.Body =
                I [Text "Client control"] :> _

    
    let Template title body : Content<Action> =
        PageContent <| fun context ->
            { Page.Default with 
                Title = Some title
                Body = body context
            }

    let Index =
        Template "Index page" <| fun ctx ->
            [
                H1 [Text "Pages"]
                UL [
                    LI [A [HRef (ctx.Link Action.Page1)] -< [Text "Page 1"]]
                    LI [A [HRef (ctx.Link Action.Page2)] -< [Text "Page 2"]]
                ]
            ]

    // Page with static HTML
    let Page1 =
        Template "Title of Page1" <| fun ctx ->
            [
                H1 [Text "Page 1"]
                A [Action.Page1 |> ctx.Link |> HRef] -< [Text "Page 2"]
            ]

    // Page with a dynamic JavaScript control
    let Page2 =
        Template "Title of Page2" <| fun ctx ->
            [
                H1 [Text "Page 2"]
                A [Action.Page1 |> ctx.Link |> HRef] -< [Text "Page 1"]
                Div [new Client.MyControl ()]
            ]
        
    // Putting the site together
    let MySitelet =
        [
            Sitelet.Content "/index" Action.Index Index
            Sitelet.Folder "/pages" [
                Sitelet.Content "/page1" Action.Page1 Page1
                Sitelet.Content "/page2" Action.Page2 Page2
            ]
        ]
        |> Sitelet.Sum
    
    // Actions to generate pages from
    let MyActions = 
        [
            Action.Index
            Action.Page1
            Action.Page2
        ]

/// The class that contains the website
type MySampleWebsite() =
    interface IWebsite<SampleSite.Action> with
        member this.Sitelet =
            SampleSite.MySitelet
        member this.Actions =
            SampleSite.MyActions

[<assembly: WebsiteAttribute(typeof<MySampleWebsite>)>]
do ()
```
