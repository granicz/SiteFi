---
title: "Simple reactive scenarios with WebSharper"
categories: "ui.next,websharper"
abstract: "One of the most fundamental design considerations any developer must deal with is handling change. In this article, we are primarily concerned with client-side state and changes to it. Change can be brought about by various external factors (user input such as mouse or keyboard events, server push messages, etc.) or by means internal to the application itself."
identity: "5226,82407"
---
(This post is part of [F# Advent 2016](https://sergeytihon.wordpress.com/2016/10/23/f-advent-calendar-in-english-2016/) - Happy 2017!)

One of the most fundamental design considerations any developer must deal with is handling change. In this article, we are primarily concerned with client-side state and changes to it. Change can be brought about by various **external** factors (user input such as mouse or keyboard events, server push messages, etc.) or by means **internal** to the application itself.

For a long time, handling client-side changes was tied to an object-oriented, event-driven programming style that required a user-initiated action/event connected to an UI control "instance". You may remember adding event handlers for buttons that triggered an action and possibly a change in the state of the user interface by refreshing relevant controls.

While the range of events to which event handlers can be attached typically grows with the maturity (and complexity) of the encompassing UI framework (ASP.NET, WinForms, WPF, etc.), they themselves will never be enough to handle change as we would like to.

### Two-way binding with reactive variables

WebSharper long introduced [UI.Next](https://github.com/intellifactory/websharper.ui.next) to supersede [WebSharper.Html](http://websharper.com/docs#web_development/html_combinators) as its main DOM construction library.

Buried deep in the WebSharper documentation, you can find a [comprehensive tutorial](http://websharper.com/docs#web_development/ui.next_-_f-sharp) on how to use UI.Next for basic two-way data binding with reactive markup. The drill is pretty simple: next to composing with ordinary HTML combinators, you can use various HTML input controls whose values are automatically synched with a **reactive variable** (typically of type `Var<string>`).

Here we have a plain input box bound to `v`:

```fsharp
open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Notation

[<JavaScript>]
module SimpleInput =

    let Main =
        let v = Var.Create ""
        div [
            Doc.Input [] v
        ]
        |> Doc.RunById "main"
```

You can now update the input box easily by writing to `v`. Consider the addition of a simple Clear button:

```fsharp
    ...
    div [
        buttonAttr [on.click (fun _ _ -> v := "")] [text "Clear"]
        Doc.Input [] v
    ]
```

The fun starts when there is dependent markup in your page computed from the value of the input box. Say, you wanted to echo what's typed into the input box, using all caps:

```fsharp
    ...
    div [
        buttonAttr [on.click (fun _ _ -> v := "")] [text "Clear"]
        Doc.Input [] v
        p [textView (v.View.Map (fun s -> s.ToUpper()))]
    ]
```

Here, `v.View` returns the current value of `v`, and `textView` converts it to an HTML text node. You can also react to keyboard and mouse input ([API](https://github.com/intellifactory/websharper.ui.next/blob/master/docs/Input.md)) equally easily:

```fsharp
    div [
        ...
        p [textView (Input.Mouse.Position.Map (fun (x,y) -> sprintf "%d:%d" y x))]
    ]
```

To sum up the basics a somewhat more elaborate [live snippet](http://try.websharper.com/snippet/adam.granicz/00001u) is below:

<div style="width:100%;min-height:550px;position:relative"><iframe style="position:absolute;border:none;width:100%;height:100%" src="http://try.websharper.com/embed/adam.granicz/00001u"></iframe></div>

### List models

Now that you can gather and manipulate user input via reactive variables, and reflect computed/derived values in reactive markup, you can also bind composite data in your web pages; and this is where the real fun begins.  All we need is `Models` ([API](https://github.com/intellifactory/websharper.ui.next/blob/master/docs/Model.md)), and in particular `ListModels` ([API](https://github.com/intellifactory/websharper.ui.next/blob/master/docs/ListModel.md)).

`ListModels` associate values with keys in a time-varying collection.  This association can be implicit, or as in most cases explicit. Below is a `ListModel` that stores simple names - here the names themselves act as their own key (note the `id` function used as the value->key map):

```fsharp
let names = ListModel.Create id ["John"; "James"]
names.Add "Jonathan"
```

### Example - System messages

We are only scratching the surface here, but assume we want to display a set of system messages, each coming with one of the usual Info/Warning/Error flavor.  For added complexity, we also want an Add and a Remove button to play with test data.

```fsharp
namespace Samples

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Notation

[<JavaScript>]
module SimpleInput =
    let counter = ref 0
    let removedCounter = ref 0
    let GetNextCounter() = incr counter; !counter
    
    type MessageType = Info | Warning | Error
    type Message =
        {
            Id: int
            Title: string
            Type: MessageType
        }
        
    let RandomMessage() =
        let id = GetNextCounter()
        {
            Id = id
            Title = sprintf "Message #%d" id
            Type = if id % 3 = 0 then Info elif id % 3 = 1 then Warning else Error
        }
        
    let Main =
        let messages = ListModel.Create (fun msg -> msg.Id) [RandomMessage()]

        div [
            buttonAttr [
                on.click (fun _ _ -> messages.Add(RandomMessage()))
            ] [text "Add"]

            buttonAttr [
                on.click (fun _ _ ->
                    if !removedCounter < !counter then
                        incr removedCounter
                        messages.RemoveByKey(!removedCounter)
                )
            ] [text "Remove"]
            
            messages.View.DocSeqCached (fun msg ->
                let ty, bg =
                    match msg.Type with
                    | Info -> "info", "bg-success"
                    | Warning -> "warning", "bg-warning"
                    | Error -> "error", "bg-danger"
                pAttr [attr.``class`` bg] [text (ty + ": " + msg.Title)]
            )
        ]
        |> Doc.RunById "main"
```

Here, `messages` is initialized with a random system message, and `messages.View.DocSeqCached` is a fancy way of reflecting each message to markup - and note we use Bootstrap class names, so be sure to include the main Bootstrap CSS in the template you are serving for your application.  A [live snippet](http://try.websharper.com/snippet/adam.granicz/0000Be) is below:

<div style="width:100%;min-height:300px;position:relative"><iframe style="position:absolute;border:none;width:100%;height:100%" src="http://try.websharper.com/embed/adam.granicz/0000Be"></iframe></div>

### Example - Grouping system messages

Suppose that in the previous example, we want to group the system messages based on their type.  We can introduce a helper function to filter the subset we are interested in:

```fsharp
    ...
    let Main =
        let messages = ListModel.Create (fun msg -> msg.Id) [RandomMessage()]

        let filter pred =
            messages.View.DocSeqCached (fun msg ->
                if pred msg then
                    let ty, bg =
                        match msg.Type with
                        | Info -> "info", "bg-success"
                        | Warning -> "warning", "bg-warning"
                        | Error -> "error", "bg-danger"
                    pAttr [attr.``class`` bg] [text (ty + ": " + msg.Title)] :> Doc
                else
                    Doc.Empty
            )
		...
```

Using `filter`, we can compute the info/warning/error set separately and display them after each other:

```fsharp
        let infos    = filter (fun msg -> match msg.Type with | Info    -> true | _ -> false)
        let warnings = filter (fun msg -> match msg.Type with | Warning -> true | _ -> false)
        let errors   = filter (fun msg -> match msg.Type with | Error   -> true | _ -> false)

        div [
            buttonAttr [
                on.click (fun _ _ -> messages.Add(RandomMessage()))
            ] [text "Add"]

            buttonAttr [
                on.click (fun _ _ ->
                    if !removedCounter < !counter then
                        incr removedCounter
                        messages.RemoveByKey(!removedCounter)
                )
            ] [text "Remove"]
        
            h3 [text "Info-level messages"]
            infos
            h3 [text "Warnings"]
            warnings
            h3 [text "Errors"]
            errors
        ]
        |> Doc.RunById "main"
```

So with a few lines adjustment we have message grouping under control - feel free to play with the [live snippet](http://try.websharper.com/snippet/adam.granicz/0000Bf) below:

<div style="width:100%;min-height:300px;position:relative"><iframe style="position:absolute;border:none;width:100%;height:100%" src="http://try.websharper.com/embed/adam.granicz/0000Bf"></iframe></div>

### Conclusion

In this brief article, we looked at a few basic reactive scenarios with WebSharper and saw how UI.Next makes it easy to work with two-way binding, reactive markup, and aggregate client-side models. In upcoming articles, I will further examine `ListModels` and their more advanced capabilities, including client-side and client-server persistence.

Happy 2017 and happy coding!
