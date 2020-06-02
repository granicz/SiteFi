---
title: "Introducing Try WebSharper"
categories: "snippets,f#,websharper"
abstract: "We are thrilled to announce the availability of Try WebSharper, a simple and fun way to create and share WebSharper snippets with others!"
identity: "4424,79902"
---
We are thrilled to announce the availability of [Try WebSharper](http://try.websharper.com), a simple and fun way to create and share WebSharper snippets with others!

[![](http://i.imgur.com/s0TUbvdl.png)](http://i.imgur.com/s0TUbvd.png)

The code you share can be **run** (=>Run) right in your browser, and you get basic type checking with warnings and errors showing up as red/yellow squiggles.  Currently, this takes a couple seconds to complete so it's not the most convenient way to experiment, but it will get a lot better soon.

We aim to keep this site current with upcoming WebSharper releases, so you can use it to experiment with even the newest WebSharper features, including reactive markup and templating via UI.Next.

### Converting F# snippets to WebSharper

While [Try WebSharper](http://try.websharper.com) is intended primarily for sharing WebSharper snippets (e.g. F# snippets that produce web output), you can also convert simple F# snippets by following a few simple steps.

Say, you wanted to run the following:

```fsharp
open System

sprintf "Time now is %s" (DateTime.Now.ToShortTimeString())
```

First, it helps to understand that snippets are not simply run as written in F#; they are converted to JavaScript with WebSharper and executed in your browser.  For this, we use the single-page application (SPA) WebSharper template.  This takes a master HTML document (`index.html` - and you can write to this file on the second tab) that references the JavaScript code generated from your F# code.

#### Using `JS.Alert`

In order for this to have a visible effect, you could simply raise a JavaScript popup with your output:

```fsharp
open System
open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
let Main =
    sprintf "Time now is %s" (DateTime.Now.ToShortTimeString())
    |> JS.Alert
```

Note that you will always need to open `WebSharper` for the `[<JavaScript>]` attribute and other WebSharper pervasives, and it's also handy to open `WebSharper.JavaScript` for client-side/EcmaScript functions like `JS.Alert`.

#### Caveat
You may be tempted to throw away the name of the top-level binding, but this will yield **no output** due to `let _ = ...` being interpreted as `do ...` - which acts as a module initializer (instead of a top-level binding) that currently is not translated to JavaScript by WebSharper.

```fsharp
[<JavaScript>]
let _ =  ...
```

#### Using `WebSharper.Html.Client`

Another thing you can do is to show it in the markup that is rendered from `index.html` as your output.  Note, that by default this document has a `div` node with `id=main`, which you can write to using the `AppendTo` member on any `HTML.Client` element.

```fsharp
open System
open WebSharper
open WebSharper.JavaScript
open WebSharper.Html.Client

[<JavaScript>]
let Main =
    Div([Text (sprintf "Time now is %s" (DateTime.Now.ToLongTimeString()))])
       .AppendTo "main"
```

Here, you are opening `WebSharper.Html.Client` for accessing client-side HTML functions like `Div`.

#### Using `WebSharper.UI.Next`

If you followed our [recent announcements for WebSharper 3.4](http://websharper.com/blog-entry/4422/websharper-3-4-released), you will likely prefer to do the above via the more flexible [reactive HTML language](http://websharper.com/blog-entry/4423/websharper-ui-next-3-4-the-new-html-syntax) introduced via UI.Next.  Here is how that goes:

```fsharp
open System
open WebSharper    
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

[<JavaScript>]
let Main =
    div [text (sprintf "Time now is %s" (DateTime.Now.ToLongTimeString()))]
    |> Doc.RunById "main"
```

So all in all, these should give you three different strategies to convert F# snippets into WebSharper ones.

### Site features and plans

As a code snippets site, [Try WebSharper](http://try.websharper.com) has the usual toolset for snippets: you can **create a new snippet** by hitting the big red plus (+) button in the bottom right corner, **fork an existing snippet** (=>Fork), or **save the one you are working on** (=>Save).

We also added a basic set of examples (with more being moved from [WebSharper examples](http://websharper.com/samples)), which you can find under the hamburger icon, along with any saved snippets you may have:

[![](http://i.imgur.com/13mqital.png)](http://i.imgur.com/13mqita.png)

### Coming up

In future releases, we will be introducing additional functionality to help with **"templating" basic snippets** using any one of the above methods, **spinning up snippets via GitHub gists**, and lighting up **much more refined code assistance services** (code completion, type checking as you type, hover comments and signatures, etc.)

Happy sharing!