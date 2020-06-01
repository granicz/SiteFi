---
title: "Bolero 0.12 released"
categories: "bolero,f#,websharper"
abstract: "Updated for .NET Core 3.1 and Blazor 3.2, introducing lazy components, improved data binders, and more."
identity: "5900,87848"
---
We are happy to announce the release of [Bolero](https://fsbolero.io) version 0.12. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using [Blazor](https://blazor.net/).

This release requires the .NET Core SDK version 3.1.102 or newer, which you can download [here](https://dotnet.microsoft.com/download/dotnet-core/3.1) for Windows, OSX or Linux.

Install the latest project template with:

```
dotnet new -i Bolero.Templates::0.12.3
```

If you have an existing Bolero project, you can check [the upgrade guide](https://fsbolero.io/docs/Upgrade) to learn how to update your project for Bolero 0.12.

## Blazor 3.2

Bolero 0.12 brings the dependency up to date to Blazor 3.2-preview2. This release brought a number of breaking changes, notably to package names where some occurrences of the name `Blazor` became `Components.WebAssembly`, so make sure to follow [the upgrade guide](https://fsbolero.io/docs/Upgrade) for your existing projects.

Blazor 3.2-preview2 also fixes [the mono linker issue](https://github.com/mono/linker/issues/927) that prevented compiling any project referencing FSharp.Core with 3.2-preview1.

## Improvements

Since the previous blog was about Bolero 0.9, let's talk about some improvements added since then.

### Improved binders

The `bind` module provides functions for two-way binding between the Elmish model and user inputs in the view. In Bolero 0.11, we improved their implementation by using the same underlying methods as Blazor uses when generating C# from Razor pages. This brings feature parity in terms of supported types by adding `int64`, `float32`, `decimal`, `DateTime` and `DateTimeOffset`.

Additionally, the API of this module has changed. It now contains submodules `input` and `change`, each binding values to the corresponding event. These submodules contains functions for each supported type: `string`, `int`, and so on.

For example, the following:

```fsharp
concat [
    input [bind.input model.name (fun n -> dispatch (SetName n))]
    input [bind.changeInt model.age (fun a -> dispatch (SetAge a))]
]
```

becomes:

```fsharp
concat [
    input [bind.input.string model.name (fun n -> dispatch (SetName n))]
    input [bind.change.int model.age (fun a -> dispatch (SetAge a))]
]
```

And finally, the module `bind.withCulture` provides the same submodules and functions, but taking an extra `CultureInfo` argument to specify how values should be parsed and displayed in terms of date format, decimal point, etc.

### Lazy component functions

Sometimes, we know that a whole chunk of a page will only change when a given part of the model changes.

```fsharp
// A helper component for text inputs with a label
module Input =
    type Model = { label: string; value: string }

    // In this function, we know that only value should ever change, not label.
    let view model dispatch =
        label [] [
            text model.label
            input [bind.change.string model.value dispatch]
        ]

// Our application
type Model = { firstName: string; lastName: string }

type Message = SetFirstName of string | SetLastName of string

let view model dispatch =
    concat [
        Input.view { label = "First name"; value = model.firstName } (dispatch << SetFirstName)
        Input.view { label = "Last name"; value = model.lastName } (dispatch << SetLastName)
    ]
```

This knowledge is useful, because we can tell Blazor's renderer not to try and rerender it unless necessary. This can be done by creating a component that takes the changing model as a parameter:

```fsharp
module Input =
    type Model = { label: string; value: string }

    type Component() =
        inherit ElmishComponent<Model, string>()
        
        // Only rerender if the value has changed.
        override _.ShouldRender(oldModel, newModel) =
            oldModel.value <> newModel.value
            
        override _.View model dispatch =
            label [] [
                text model.label
                input [bind.change.string model.value dispatch]
            ]
            
    let view model dispatch =
        ecomp<Component,_,_> model dispatch
```

This is a bit tedious and verbose, though. So in Bolero 0.11, we took inspiration from Fable's `lazyView` family of functions to add a very similar `lazyComp` family of functions. The above becomes:

```fsharp
module Input =
    type Model = { label: string; value: string }
    
    // Don't rerender if the value is the same.
    // Ignore the label, we know it won't change.
    let equal oldModel newModel =
        oldModel.value = newModel.value
    
    let strictView model dispatch =
        label [] [
            text model.label
            input [bind.change.string model.value dispatch]
        ]
    
    let view model dispatch =
        lazyComp2With equal strictView model dispatch
        
    // lazyComp2With means:
    //  * lazyComp: it's a lazy component.
    //  * 2: it passes 2 arguments to the actual view function (the model and the dispatch).
    //  * With: it uses a custom equality function to check whether to rerender.
```

Thanks @brikken for your work on this!

### Asynchronous event handlers

We have also added support for asynchronous event handlers. They are located in submodules `on.async` and `on.task` and use callbacks that return `Async<unit>` and `Task`, respectively.

```fsharp
let myButton (js: IJSRuntime) =
    button [
        on.task.click (fun event ->
            js.InvokeVoidAsync("console.log", event.ClientX).AsTask()
        )
    ] [text "Click me!"]
```

We recommend not abusing these by putting too much logic directly in event handlers though; it is generally better practice to simply `dispatch` a message in the event handler, and do the actual processing in the Elmish `update` function.

### And more...

The full list of changes is available [on the GitHub releases page.](https://github.com/fsbolero/Bolero/releases)

Happy coding!