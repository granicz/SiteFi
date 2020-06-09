---
title: "Serving SPAs"
categories: "templating,ui,routing,f#,websharper"
abstract: "Consider a very typical scenario: you have a web application that serves an SPA. The SPA itself has several \"pages,\" each with its own client-side routed URLs (think of Gmail, for instance.) Then people copy some of these URLs from their \"session\" and send them to others, who in turn expect to find exactly what the sender saw. Except with most SPAs, the server-side knows nothing about the URLs generated on the client and usually throws you back to some initial SPA page. Implementing this properly requires you to share all or a subset of the URLs both on the server and on the client. With WebSharper, this is easy."
identity: "5488,84145"
---
(This article is part of [F# Advent 2017](https://sergeytihon.com/2017/10/22/f-advent-calendar-in-english-2017/) - thanks to [Sergey Tihon](https://sergeytihon.com/) for organizing!)

Consider a very typical scenario: you have a web application that serves an SPA. The SPA itself has several "pages," each with its own client-side routed URLs (think of Gmail, for instance.) Then people copy some of these URLs from their "session" and send them to others, who in turn expect to find exactly what the sender saw. Except with most SPAs, the server-side knows nothing about the URLs generated on the client and usually throws you back to some initial SPA page. Implementing this properly requires you to share all or a subset of the URLs both on the server and on the client. With WebSharper, this is easy.

### Routing

One fundamental difference between WebSharper and other server-side capable F# frameworks such as Giraffe or Suave, is that WebSharper supports **safe URLs**, i.e. URLs computed from the serving context (as opposed to encoding them as ordinary strings) that are guaranteed to be correct.

Consider the following simple app in Giraffe:

```fsharp
open Giraffe

let webApp =
    choose [
        route "/ping"     >=> text "pong"
        routef "/echo/%s" (fun s -> text s)
        route "/"         >=> htmlFile "/pages/index.html" ]

type Startup() =
    member __.Configure (app : IApplicationBuilder)
                        (env : IHostingEnvironment)
                        (loggerFactory : ILoggerFactory) =

        app.UseGiraffe webApp
```

The same app would look like this in WebSharper:

```fsharp
open WebSharper
open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "/">]     Home
    | [<EndPoint "/ping">] Ping
    | [<EndPoint "/echo">] Echo of string

[<Website>]
let Main =
    Application.MultiPage (fun (ctx: Context<EndPoint>) -> function
        | EndPoint.Home   -> Content.File "pages/index.html"
        | EndPoint.Ping   -> Content.Text "pong"
        | EndPoint.Echo s -> Content.Text s
    )
```

With this declaration, **you can obtain the correct URL to any page** (or "endpoint", and fully formed with any of its page arguments) from the serving context, which itself is parameterized over the (typically) discriminated union type that represents all entry points to the application.

For instance, you can get the URL to a particular instantiation of the "echo" page as follows:

```fsharp
   ctx.Link (EndPoint.Echo "hello")
```

Now, clearly, there is a lot you would/could want to do with URLs/endpoints. Making them accessible via POST instead of GET, adding authentication, passing or posting structured values, etc. (You can find the relevant [WebSharper documentation here](https://developers.websharper.com/docs/v4.1/fs/sitelets).) You can do all of these in WebSharper and Giraffe, equally. The main difference, however, remains that Giraffe (or Suave for that matter) builds these up from smaller functions/combinators, whereas WebSharper enables you to do these declaratively (attached to the type that represents your endpoints, i.e. `EndPoint` above) and generates a similar pipeline of combinators under the cover (you can also construct routers manually, similar to Giraffe). This gains not only safe URLs, but also a number of other benefits.

One such benefit is exactly what we need: sharing the URL space between the server and the client side. In pure SPAs, this means sharing the full endpoint type, but more often than that you would want your client-side URL space as a proper subset of the full endpoint set.

### A barebones approach

[![](https://i.imgur.com/Dk73D2jm.png)](https://i.imgur.com/Dk73D2j.png)
[![](https://i.imgur.com/K2PnK5Im.png)](https://i.imgur.com/K2PnK5I.png)
[![](https://i.imgur.com/XYPtC8Rm.png)](https://i.imgur.com/XYPtC8R.png)

(In the screeshots above, the city pages are routed on the client and no server roundtrip (and thus no page refresh) is taking place. The home page is re-requested from the server every time you visit it, you can add other server-based pages easily. However, you can refresh each page, including the two kinds of SPA pages, and you will correctly see the same content.)

Say you want an app (shown above, grab it via a gist [here](https://gist.github.com/granicz/eeaf2140a3f1fbccc9ebb4af9deadaa1)) to report things on various cities using the following URLs:

| URL | .....|
|:----------|:--------|
| `/`  | The root/home page     |
| `/spa/cities` | The page listing our cities, the main SPA |
| `/spa/cities/XYZ` | A page for showing info for a given city |

You can define your endpoint type as follows:

```fsharp
type SPA =
    | [<EndPoint "/cities">] Cities
    | [<EndPoint "/cities">] City of string

type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/spa">] SPA of SPA
```

The intention is that **`/spa/*` URLs will be routed and their content generated on the client** (i.e. without a server roundtrip). You can do this easily by combining `Router.Infer<...>` (to create a router for an endpoint type automatically), `Router.Slice` (to split the part of it you want to make available on the client), and `Router.Install` (to create a client-side router to listen to URL changes in the browser) - alternatively, you can also use `Router.InstallHash` for a hash-based version:

```fsharp
    let router = Router.Infer<EndPoint>()
    let location =
        router
        |> Router.Slice (function | SPA spa -> Some spa | _ -> None) EndPoint.SPA
        |> Router.Install SPA.Cities
```

The type of `location` is `Var<SPA>`, a reactive variable that you can observe the value of (to fetch the current SPA page) or set directly (to route to that page). Note the sharp contrast between this type-safe approach and other ad-hoc methods that perform these via ordinary strings.

```fsharp
open WebSharper.UI.Notation

location := SPA.Cities
```

You can now reactively construct the individual SPA pages based on the value of `location` and a simple city store:

```fsharp
[<JavaScript>]
module Client =
    open WebSharper.UI.Client
    
    let store =
        [
            "Budapest", "awesome"
            "Paris", "famous"
            "San Francisco", "expensive"
            "London", "cosmopolitan"
            "Singapore", "crowded"
        ]
        
    let Main () =
        ...
        location.View.Doc(function
            | SPA.Cities ->
                Doc.Concat [
                    ul [] (
                        store |> List.map (fun (city, _) ->
                            a [attr.href <| router.Link (EndPoint.SPA (SPA.City city))] [
                                li [] [text city]
                            ] :> Doc)
                    )
                ]
            | SPA.City city ->
                let message =
                    match List.tryFind (fun (cty, _) -> city=cty) store with
                    | None ->
                        p [] [text "I don't know your city :("]
                    | Some (_, adjective) ->
                        p [] [text <| "Your city is " + adjective + "!"]
                Doc.Concat [
                    h1 [] [text city]
                    message
                    p [] [
                        text "Click "
                        a [attr.href (router.Link (EndPoint.SPA SPA.Cities))] [text "here"]
                        text " to go back."
                    ]
                ]
        )
```

All that remains is serving this SPA and an empty home page from the server-side:

```fsharp
module Site =
    let HomePage (ctx: Context<EndPoint>) =
        Content.Page(
            Title = "Home",
            Body = [
                p [] [
                    text "There is nothing here, go to the "
                    a [attr.href <| ctx.Link (EndPoint.SPA SPA.Cities)] [text "Cities page"]
                ]
            ]
        )

    let CitiesPage ctx =
        Content.Page(
            Title = "Cities",
            Body = [
                div [] [client <@ Client.Main () @> ]
            ]

        )

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home ->
                HomePage ctx
            | EndPoint.SPA SPA.Cities
            | EndPoint.SPA (SPA.City _) ->
                CitiesPage ctx
        )
```

This app, which can be hosted in any ASP.NET- or OWIN-compatible container, including Suave, or self-hosted) guarantees that we can serve any one of the three types of URLs we specified earlier, and it correctly routes the SPA (`/spa/*`) accordingly as well. (Note: you can also accomplish this also by combining Suave or Giraffe for the server-side and Fable.Elmish on the client-side, but **you won't get any integration or safe URL representation between the two tiers**.)

### Adding templating

[![](https://i.imgur.com/uBAdweUm.png)](https://i.imgur.com/uBAdweU.png)
[![](https://i.imgur.com/mOuYbuLm.png)](https://i.imgur.com/mOuYbuL.png)
[![](https://i.imgur.com/eKE3u0Dm.png)](https://i.imgur.com/eKE3u0D.png)

Another very common need is **to be able to modify an application, or at least its appearance, at runtime**. For instance, you may want to touch up some of the markup you generate in your application, change its structure, add new attributes here and there, etc. On the server-side, both Suave and Giraffe provide a way to serve templated content via DotLiquid. They can also detect modifications to these templates and reload them before serving new content based on them. However, support for templating on the client-side from an F# perspective is severely limited, and you typically fall back to using Mustache, Handlebars, Underscore, or whatever JavaScript-based templating engine you prefer, via inlined JS code, or no support at all.

Not with WebSharper. Although you can easily adapt DotLiquid and other server-side templating engines to plug directly into WebSharper content, WebSharper UI also comes with its own templating engine that you can read about [here](https://developers.websharper.com/docs/v4.1/fs/ui). The nice thing about this templating engine is that **it works both on the server and on the client, unlike anything else**.

What we want is to remove as much of the inlined HTML from our F# code as possible and replace it with HTML templates that we can then modify external to the application. And don't even think about trying to push CSS and other silliness into your F# code, as it will limit you severely. For this example, I cooked up a straightforward template (`Main.html`) that uses Bulma, which you can find in [this gist](https://gist.github.com/granicz/9cd31128496fda6c0f6c228cce55d246).

For instance, consider the following fragment, which defines a `Banner` placeholder and two inner "sub" templates `HomeBanner` and `CitiesBanner`, that can be readily plugged into `Banner`.

```xml
  <div ws-replace="Banner">
    <section class="hero is-info" ws-template="HomeBanner">
      <div class="hero-body">
        <p class="title">Home</p>
      </div>
    </section>
    <section class="hero is-info" ws-template="CitiesBanner">
      <div class="hero-body">
        <p class="title">Cities</p>
        <p class="subtitle">Pick your favorite city.</p>
      </div>
    </section>
  </div>
```
Or consider the nested templates for the SPA content:

```xml
      <div class="content">
        <div ws-replace="Body">
          <ul ws-template="CityLinks" ws-hole="Links">
            <li ws-template="CityLink"><a href="${Link}">${Title}</a></li>
          </ul>
          <div ws-template="CityPage">
            <h1>${Name}</h1>
            <div ws-replace="Message">
              <div ws-template="NotFound">
                <p>I don't know your city :(</p>
              </div>
              <div ws-template="Found">
                <p>Your city is <b>${Kind}</b></p>
              </div>
            </div>
            <p>Click <a href="${BackLink}">here</a> to return.</p>
          </div>
        </div>
```

All you have to do now is to bring the template and all the good stuff in it into scope:

```fsharp
    open WebSharper.UI.Templating

    type MainTemplate = Template<"Main.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>
```

This says "go through `Main.html` and find all placeholders, templates and nested templates in it, and make them available under `MainTemplate`, and if the file changes refresh it automatically when used from the server, and reload it automatically from the main document on the client (since we are in an SPA that is based on the very same file.)" **This one line performs a LOT if you think about it, and all that complexity is taken care of and hidden from you, so you can concentrate on your code and logic instead**.

Contrast what happens inside `Client.Main()` now with templating:

```fsharp
[<JavaScript>]
module Client =
    ...
    let Main () =
        ...
        location.View.Doc(function
            | SPA.Cities ->
                MainTemplate.CityLinks()
                    .Links(
                        store |> List.map (fun (city, _) ->
                            MainTemplate.CityLink()
                                .Link(router.Link (EndPoint.SPA (SPA.City city)))
                                .Title(city)
                                .Doc()
                        )
                    )
                    .Doc()
            | SPA.City city ->
                let message =
                    match List.tryFind (fst >> (=) city) store with
                    | None ->
                        MainTemplate.NotFound().Doc()
                    | Some (_, adjective) ->
                        MainTemplate.Found().Kind(adjective).Doc()
                MainTemplate.CityPage()
                    .Name(city)
                    .Message(message)
                    .BackLink(router.Link (EndPoint.SPA SPA.Cities))
                    .Doc()
        )
```

Not a single in-code HTML combinator in sight - so we are good to go. The final F# code is 110 LOC, and it serves our SPA and all of its links when visited directly as well, all dressed up in a nice Bulma-based master template. Hurray!

### Summing up

In this article, we looked at two fundamental problems you are likely to encounter in any serious web development: serving SPAs with their URLs understood both on the server and the client, and externalizing your entire presentation layer so it can be modified at runtime.

If you want to try the gists, start with a Client-Server Application template after installing WebSharper into your Visual Studio (you can grab the VSIX installer from the [main website](https://websharper.com), or download that template from the developer site (from the Apps menu on the top.)

Guess what? You can do all this in C# as well, but I'll save that for another article! So stay tuned and don't hesitate to ask about WebSharper on the [forums](https://forums.websharper.com), and be sure to check out the [documentation](https://developers.websharper.com) - both 100% WebSharper SPAs in case you wondered ;)

#### Forums

[![](https://i.imgur.com/2DkBX2ol.png)](https://forums.websharper.com)

#### Documentation

[![](https://i.imgur.com/ANg1Fual.png)](https://developers.websharper.com)

#### Quick links:

 * documentation on [routing requests](http://developers.websharper.com/docs/v4.1/fs/sitelets)
 * documentation on [WebSharper UI and templating](http://developers.websharper.com/docs/v4.1/fs/ui)
 * barebones example in [this gist](https://gist.github.com/granicz/eeaf2140a3f1fbccc9ebb4af9deadaa1), and
 * the templated version in [this gist](https://gist.github.com/granicz/9cd31128496fda6c0f6c228cce55d246)
 
Happy holidays and happy coding!
