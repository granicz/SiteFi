---
title: "WebSharper UI.Next 3.4: the new HTML syntax"
categories: "ui.next,fsharp,html,javascript,reactive,websharper"
abstract: "In version 3.4, WebSharper UI.Next's markup has been overhauled in order to prepare for the upcoming merger into WebSharper in version 4.0."
identity: "4423,79893"
---
As its name suggests, UI.Next was created to be the next-generation standard library for UI programming in WebSharper. As a consequence, as of WebSharper 4.0, **UI.Next will be merged into WebSharper itself** under a less "codenamey" moniker that we haven't decided yet. It will completely obsolete the current Html.Client and Html.Server.

The [recently released version 3.4](http://websharper.com/blog-entry/4422/websharper-3-4-released) prepares the terrain for this merger. It streamlines the embedded markup syntax, introduces server-side capability and adds client-side functionality.

## Streamlined syntax

WebSharper UI.Next 3.4 overhauls the syntax for embedding HTML elements in F# for enhanced readability and familiarity.

The most visible change is the switch to **lowercase elements and attributes**, making UI.Next code more similar to what you would write in an HTML file. For elements, the function `Div0` which creates a `<div>` tag with child elements becomes simply `div`, and `Div` which creates a `<div>` tag with attributes and child elements becomes `divAttr`. For attributes, the class `attr` contains static methods to create standard attributes and `on` creates event handlers. Here is a list of the new constructors available under `WebSharper.UI.Next.Html`:

|:-|:-|:-|
| New syntax | Old / Verbose syntax | |
| `divAttr [attrs...] [children...]` | `Div [attrs...] [children...]` | |
| `div [children...]` | `Div0 [children...]` | |
| `text "Hello"` | `Doc.TextNode "Hello"` | |
| `textView aView` | `Doc.TextView aView` | Needs `open WebSharper.UI.Next.Client` |
| `attr.color "black"` | `Attr.Create Attributes.Color "black"` | |
| `attr.colorDyn aView` | `Attr.Dynamic Attributes.Color aView` | Needs `open WebSharper.UI.Next.Client` |
| `attr.colorDynPred aView aBoolView` | `Attr.DynamicPred Attributes.Color aBoolView aView` | Needs `open WebSharper.UI.Next.Client` |
| `attr.anim aView aFunc aTrans` | `Attr.Animated aTrans aView aFunc` | Needs `open WebSharper.UI.Next.Client` |

See "Tier-specific functionality" below for the rationale behind needing `open WebSharper.UI.Next.Client` for some of these.

Here is a small code sample:

```fsharp
/// New syntax                              │ /// Old syntax
open WebSharper.UI.Next                     │ open WebSharper.UI.Next
open WebSharper.UI.Next.Html                │ open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client              │
                                            │
let myDocument =                            │ let myDocument =
  let rvInput = Var.Create ""               │   let rvInput = Var.Create ""
  div [                                     │   Div0 [
    h1 [text "A small example"]             │     H10 [Doc.TextNode "A small example"]
    label [text "Type something here: "]    │     Label0 [Doc.TextNode "Type something here: "]
    Doc.Input [] rvInput                    │     Doc.Input [] rvInput
    pAttr [attr.``class`` "paragraph"] [    │     P [Attr.Create Attributes.Class "paragraph"] [
      text "You typed: "                    │       Doc.TextNode "You typed: "
      textView rvInput.View                 │       Doc.TextView rvInput.View
    ]                                       │     ]
  ]                                         │   ]
```

## Server-side markup

It is now possible to use the `Doc` type, and the above syntax, to create server-side markup, ie. markup that is generated on the server and output in the HTML document, as opposed to client-side markup, generated dynamically in JavaScript.

### Returning a server-side `Doc` as content

There are two ways to use a `Doc` as server-side content:

* Use `Content.Doc` to create a Sitelets `Content<_>` value:

    ```fsharp
    open WebSharper.Sitelets
    open WebSharper.UI.Next
    open WebSharper.UI.Next.Server
    
    [<Website>]
    let MyWebsite =
      Application.SinglePage <| fun context ->
        Content.Doc myDocument
    ```

* Convert it to `Html.Server` elements using `Doc.AsElements`:

    ```fsharp
    open WebSharper.Sitelets
    open WebSharper.UI.Next
    open WebSharper.UI.Next.Server
    
    [<Website>]
    let MyWebsite =
      Application.SinglePage <| fun context ->
        Content.Page(
          Body = Doc.AsElements myDocument
        )
    ```

### Including client-side markup

In order to include a client-generated `Doc` inside a server-side `Doc`, you can use the function `client`. This function is analogous to `ClientSide` from `Html.Server`: it takes a quotation of a top-level, `[<JavaScript>]`-annotated function and includes it in server-side markup.

You can also add event handlers on server-side `Doc`s using the methods in the `on` class. These methods take a quotation of a top-level, `[<JavaScript>]`-annotated function and return an `Attr` value that sets the handler as a static attribute on the element. This means that only one handler of a given type can be added to an element this way: you can't have two instances of eg. `on.click <@ ... @>` on the same server-side `Doc`.

```fsharp
open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html

[<JavaScript>]
module Client =
  open WebSharper.JavaScript
  open WebSharper.UI.Next.Client
  
  let Widget() =
    let rvInput = Var.Create ""
    Doc.Concat [
      Doc.Input [] rvInput
      p [text "You typed: "; textView rvInput.View]
    ]
    
  let Alert el ev =
    JS.Alert "Clicked!"

module Server =
  open WebSharper.Sitelets
  open WebSharper.UI.Next.Server

  [<Website>]
  let MyWebsite =
    Application.SinglePage <| fun context ->
      Content.Doc(
        div [
          h1 [text "Enter text below"]
          client <@ Client.Widget() @>
          buttonAttr [on.click <@ Client.Alert @>] [text "Click me!"]
        ]
      )
```

### Tier-specific functionality

Some `Doc` functionality is only available on the client, or only on the server. To use such functionality, you need to open `WebSharper.UI.Next.Client` or `WebSharper.UI.Next.Server`, respectively.

The following is only available on the client side, and will raise a runtime error if used from the server side:

* Reactive elements and attributes: `textView`, `Doc.EmbedView`, `attr.*Dyn`, `attr.*DynPred`, `attr.*Anim`, `Doc.Convert*`, reactive form elements (`Doc.Input`, etc).
* `Doc.Run`, `Doc.RunById`, `Doc.AsPagelet`
* `on.*` event handlers taking a function as argument. On the server you must use the version taking a quotation as argument.
* Element methods and properties described below in "New client-side functionality".

The following is only available on the server side, and will raise a compile-time error if used from the client side:

* `Content.Doc`, `Doc.AsElements`, `Attr.AsAttributes`.

## New client-side functionality and the `Elt` type

One challenge in entirely replacing `Html.Client` with `Doc` is that existing applications using `Html.Client` should be convertible without requiring a complete change in paradigm. This means that we need to add as much of the imperative capabilities of `Html.Client` to `Doc` as possible. However, a `Doc` value is not guaranteed to be composed of a single root element, so accessing the element to perform actions such as `SetAttribute` or `Append` cannot be guaranteed to succeed. To fix this, we made the following change to the API.

The `Doc` type is now an abstract class, and there is a new type `Elt` that inherits from it and represents docs that are guaranteed to be composed of a single root element. The following functions return an `Elt`:

* Element constructors, such as `div` and `divAttr`.
* `Doc.Element`, `Doc.SvgElement`, `Doc.Static`.
* Form input element constructors: `Doc.Input`, `Doc.Button`, etc.

Values of type `Elt`, in addition to being `Doc`s, also have the following properties and methods (non-exhaustive list):

* `Dom` returns the underlying `Dom.Element`.
* `Append(doc)` and `Prepend(doc)` add child `Doc`s to the beginning / end of the element. Reactive `Doc`s are properly handled.
* `Clear()` removes all children. Reactive children are properly disconnected from the `View` graph.
* `Text` gets or sets the text content. The setter properly disconnects reactive children from the `View` graph.
* `Value` gets or sets the value.
* Methods to get, set, remove, test the presence of attributes, classes, styles: `GetAttribute`, `SetProperty`, `HasClass`, etc.
* `On "eventName" function` adds an event callback.

We are still ironing out tricky parts required to implement `OnAfterRender`, which is quite ubiquitous in `Html.Client` code.

## Templating language change

In order to allow using `${string}` holes from the server side, the following change has been implemented:

* `${string}` holes now have type `string` instead of `View<string>`.
* `$!{string}` is the new syntax for holes of type `View<string>`.

## Conclusion

As you can see, a lot of enhancements are necessary to allow UI.Next markup to be usable as a replacement of Html.Client and Html.Server. We are confident that these changes will make it easy to convert existing applications to UI.Next for WebSharper 4.0.

Happy coding!