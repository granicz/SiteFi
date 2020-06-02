---
title: "Part 2. Model-View-Update (MVU) style apps with WebSharper UI"
categories: "mvu,ui,elm,f#,websharper"
abstract: "In this tutorial, you will learn about using WebSharper UI to implement a simple Model-View-Update (MVU) application pattern, similar to the Elm architecture. In subsequent tutorials, you will learn about enhancing this pattern to a full-scale application development architecture (The WebSharper Architecture) that has superior performance and sufficient flexibility to implement any type of web application."
identity: "5561,84980"
---
In this tutorial, you will learn about using WebSharper UI to implement a simple Model-View-Update (MVU) application pattern, similar to the [Elm architecture](https://guide.elm-lang.org/architecture/). In subsequent tutorials, you will learn about **enhancing this pattern to a full-scale application development architecture (The WebSharper Architecture)** that has superior performance and sufficient flexibility to implement any type of web application. Be sure to read the [First Steps: Using HTML templates, accessing form values, and wiring events](//intellifactory.com/user/granicz/20180327-first-steps-using-html-templates-accessing-form-values-and-wiring-events) tutorial to get a solid grip on some of the WebSharper fundamentals, these will come handy while reading this tutorial.

### What you will learn and a quick recap

 1. **Using a Model-View-Update (MVU)-like approach** with WebSharper UI. As you will see, WebSharper UI can implement a number of different styles of reactive UI programming easily.

### You already learned in the [previous tutorial](//intellifactory.com/user/granicz/20180327-first-steps-using-html-templates-accessing-form-values-and-wiring-events) how to:

 2. **Create WebSharper SPA projects**

    In the parent folder of your choice, type
    ```
    dotnet new websharper-spa -lang f# -n YourApp
    ```
 3. **Edit the key files in your SPA project**

    For simple SPAs, these files will be:
    -   **`wwwroot\index.html`** - Your main SPA - this is the file you open to run your app
    -   **`Client.fs`** - The logic for your SPA - this is where your F# code will be

 4. **Use HTML templates**

    WebSharper UI provides an advanced templating engine with dynamic code generation both for C# and F#. Always consider using external HTML templates instead of inlined HTML combinators to speed up your developer workflow. Templates allow you to make changes to your presentation layer without having to compile your project.
    ```fsharp
    open WebSharper.UI.Templating
    
    type MySPA = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    MySPA()
        ...
        .Bind()
    ```
    You can use templates everywhere you need `Doc` values by calling `.Doc()`, or you can use `.Bind()` to apply your template instantiations to your master document (typically your main SPA page.)

 5. **Wire events**
    * In `wwwroot/index.html`, mark input controls with `ws-on[xxx]` for each event `xxx` you want to bind:
        ```html
        <button ws-onclick="OnSubmit">Click me</button>
        ```

 6. **Read and write the values of HTML input controls**
    * In `wwwroot/index.html`, mark input controls with `ws-var="xxx"`:
        ```html
        <input ws-var="Name" ws-onkeydown="OnEdit" type="text" />
        ```

    * To read form values, in `Client.fs` access as follows:
        ```fsharp
        open WebSharper.UI
        open WebSharper.UI.Client
        ...
        MySPA()
            .OnSubmit(fun e -> ... e.Vars.Name ...)
        ```
    * To write/update form values, in `Client.fs` access as follows:
        ```fsharp
        open WebSharper.UI.Notation
        ...
        MySPA()
            .OnSubmit(fun e ->
                e.Vars.Name := ""
            )
        ```

 7. **Use reactive variables**
    * Create a reactive variable:
      ```fsharp
      let v = Var.Create 0
      ```
    * Read the value of a reactive variable:
      ```fsharp
      v.Value
      ```
    * Set the value of a reactive variable:
      ```fsharp
      open WebSharper.UI.Notation
      ...
      v := !v + 1
      ```

### Prerequisites

To get the most out of this tutorial, make sure you have installed:

* .NET Core 2.0+ and ASP.NET Core
* the  [latest WebSharper templates](http://websharper.com/downloads). This is not strictly required if you clone the GitHub project directly.
* Visual Studio Code with  [Ionide](http://ionide.io/)  or Visual Studio 2017 

### Model-View-Update (MVU) - the basic Elm architecture

[Elm](https://guide.elm-lang.org) defines a simple pattern for building web applications, founded on the clean separation between:

 * **Model** - the state
 * **View** - the presentation (HTML) of the state
 * **Update** - describes how to update the state

The basic architecture (without effects) uses two key functions:

```fsharp
val update: 'Msg -> 'Model -> 'Model
val view: 'Model -> 'View
```
where `'Msg` values encode various messages that take the model forward. Both `update` and `view` are assumed to be pure, i.e. `update` computes new model values based only on the message, and `view` computes the HTML representation without changing the model (thus both are without side effects), and both produce the same value for the same input.

### Buttons - keeping a counter

Now you will implement the [buttons](https://guide.elm-lang.org/architecture/user_input/buttons.html) example from the Elm documentation by mimicing the above MVU pattern. You can [try the WebSharper UI version online](http://try.websharper.com/snippet/adam.granicz/0000Jr) to see how it works. Here we do:

#### a) Create a new SPA called Counter

```
dotnet new websharper-spa -lang f# -n Counter
```

#### b) `wwwroot\index.html`

As a best practice, always consider keeping all of your SPA markup (including templates and subtemplates, if any) in the master HTML file. So replace `wwwroot\index.html` with:

```html
<!DOCTYPE html>
<html lang="en">
<head>
     <title>Counter</title>
     <meta charset="utf-8" />
     <meta name="viewport" content="width=device-width, initial-scale=1.0" />
</head>
<body>
     <button ws-onclick="OnDecrement">-</button>
     <div ws-hole="Counter"></div>
     <button ws-onclick="OnIncrement">+</button>
     <script type="text/javascript" src="Content/Counter.min.js"></script>
</body>
</html>
```
Here, you added a `click` event handler to the increment/decrement buttons, and a `Counter` placeholder for displaying the current value. (The reference to `Content/Counter.head.js` - a file WebSharper generates for SPAs - was removed, since no JQuery is needed for this app and there are no other dependencies.)

#### c) `Client.fs`

The main application looks quite a lot like the Elm version:

```fsharp
namespace Counter

open WebSharper
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Templating

[<JavaScript>]
module Client =
    type MySPA = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    type Model = int

    type Message =
        | Increment
        | Decrement

    let update msg model  =
        match msg with
        | Message.Increment ->
            model+1
        | Message.Decrement ->
            model-1

    let init = 0

    let view =
        let vmodel = Var.Create init
        let handle msg =
            let model = update msg vmodel.Value
            vmodel := model
        MySPA()
            .OnIncrement(fun _ -> handle Message.Increment)
            .OnDecrement(fun _ -> handle Message.Decrement)
            .Counter(V(string vmodel.V))
            .Bind()
        fun model ->
            vmodel := model

    [<SPAEntryPoint>]
    let Main () =
        view init
```

#### The `view` function

You probably spotted that `view` looks slightly more complicated than just "take the master HTML file, bind the button event handlers, and reflect the counter as a text label," and you are right.

First, `view` encodes an **entire "runtime"**: it gives means to dispatch messages (`handle`), updates the model upon receiving those messages, and rebinds the changes onto the UI. Yet it manages to do all that in only four lines of code because the real complexity: binding the model to the UI is **done automatically by the UI layer** (via templating, here).

Second, this is possible because your presentation layer describes reactive content and is expressed in terms of `vmodel` and not on `model`, thus using **`Var<Model>`** values and not `Model` ones. (This distinction will become important in the upcoming tutorials as we work towards establishing a more comprehensive application pattern.)  For instance, it uses `V(string vmodel.V)` to take the current value of the model (`vmodel.V`, an integer), convert it to a string (`string vmodel.V`), and convert the result back to a view (`V(string vmodel.V)`) to bind it the `Counter` placeholder.

Third, `MySPA().xxx(...).Bind()` lights the reactive machinery on the document the first time it runs, as a "side-effect". Therefore, you fill in your placeholders and bind your events, and seal things off with `.Bind()` **in the closure of `view`**.  Another way to think about this is that `view` is not constructing the presentation layer, but instead it **sets up and binds the reactive pieces and the main logic** onto that presentation layer (which is supplied by the template.)

And last, the main job of `view` ends up simply **updating the reactive model** underneath the bound UI (by simply doing `vmodel := model`), and thus its return value is `unit`. Note, however, that you don't call `view` more than once (unlike in Elm) because the first call already sets everything up, but nothing keeps you from doing it. For instance, you might as well start with:

```fsharp
[<SPAEntryPoint>]
let Main () =
    view 0
    view 1
    view 2
    view 3
    view 4
    view 5
```


### Conclusion

In this tutorial, you saw how you can use WebSharper UI to build a Model-View-Update (MVU)-like pattern to develop simple web applications. Your model and message type, and your `update` function were exactly as you would expect, while your `view` function had a couple important differences that reflect the capabilities of the underlying WebSharper UI reactive layer.

One key thing to remember is that WebSharper UI applications **don't require "diffing" between two virtual DOM representations** as employed by popular libraries like React, but instead, changes propagate through the dataflow graph constructed from the reactive embeddings and always yield the minimal number of updates right where necessary. This also means that your WebSharper UI applications **don't depend on React or other reactive libraries**.

> Note: you can work with React and React components through direct bindings, if you prefer.

There is a whole lot more to see, so stay tuned for more.

### Source code and try the app

You can fork [this SPA project](https://github.com/websharper-samples/Counter) via GitHub. You can also [try out a slightly adapted version](http://try.websharper.com/snippet/adam.granicz/0000Jr) live on Try WebSharper.

Happy coding!
