---
title: "Bolero 0.13 released"
categories: "bolero,webassembly,f#,websharper"
abstract: "Updated to Blazor 3.2.0 final release, with improvements to server-side hosting and component attributes."
identity: "5918,88011"
---
We are happy to announce the release of [Bolero](https://fsbolero.io) version 0.13. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using [Blazor](https://blazor.net/).

This release requires the .NET Core SDK version 3.1.300 or newer, which you can download [here](https://dotnet.microsoft.com/download/dotnet-core/3.1) for Windows, OSX or Linux.

Install the latest project template with:

```
dotnet new -i Bolero.Templates::0.13.6
```

If you have an existing Bolero project, you can check [the upgrade guide](https://fsbolero.io/docs/Upgrade) to learn how to update your project for Bolero 0.13.

## Blazor 3.2.0 final release

The Blazor team [have released their first stable version](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-now-available/), v3.2.0. Congratulations to them!

In Bolero 0.13, we have updated the dependencies to v3.2.0, and as a consequence, we have also removed the `preview` tag from Bolero's package version.

## Improved server-side hosting

Bolero applications can be hosted by an ASP.NET Core server, not only when running in server-side mode, but also in WebAssembly mode. This enables features such as server-side prerendering of the page contents. Until v0.12, some of the hosting configuration logic was implemented in the Bolero project template, in a file called `HostModel.fs` and in the server-side Razor page `_Host.cshtml`.

In v0.13, this logic has been streamlined into `Bolero.Server`. `HostModel.fs` is now gone, `_Host.cshtml` is simpler, and configuration is centralized in the ASP.NET Core application startup. [See the upgrade guide for how to adapt an existing project](https://fsbolero.io/docs/Upgrade).

```fsharp
open Microsoft.Extensions.DependencyInjection
open Bolero.Server.RazorHost

type Startup() =

    member _.ConfigureServices(services: IServiceCollection) =
        services.AddBoleroHost() |> ignore
        // Add other services, including MVC...
```

This method `AddBoleroHost()` has optional arguments that configure the hosting of Bolero applications:

* `server: bool` decides where the application runs: in WebAssembly if `server = false`, or on the server if `server = true`. The default is `false`.

* `prerendered: bool` decides whether the application's initial view should be prerendered in the served HTML (`true`), or should be blank and only rendered once the application is loaded (`false`).

* `devToggle: bool`: when this is `true` and the ASP.NET Core application runs in the `Development` [environment](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1), the `server` option can be temporarily overridden by passing `?server=true` or `?server=false` in the URL. This is particularly useful when developing a WebAssembly application, to temporarily switch to server mode for easier debugging.

## Component attributes

It is simple to use Blazor components from Bolero: call the `comp` function parameterized by the component type, then pass a list of attributes and a list of children, and voilà. Here is an example using the [MatBlazor](https://www.matblazor.com) library for Material UI style components:

```fsharp
open Bolero.Html
open MatBlazor

let myButton =
    comp<MatButton>
      [ "Icon" => "favorite"
        "Link" => "https://github.com/BlazorComponents/MatBlazor" ]
      [ text "Click me!" ]
```

However, some attribute types need specific handling. Bolero 0.13 adds functions for the following:

* `EventCallback<T>` can be used using one of the following functions:

    * `attr.callback : string -> (T -> unit) -> Attr` to create a synchronous handler;
    * `attr.async.callback : string -> (T -> Async<unit>) -> Attr` or
    * `attr.task.callback : string -> (T -> Task) -> Attr` to create an asynchronous handler.

    For example:

    ```fsharp
    let myButton (js: IJSRuntime) =
        comp<MatButton>
          [ attr.task.callback "OnClick" (fun _ -> js.InvokeVoidAsync("alert", "Clicked!").AsTask()) ]
          [ text "Click me!" ]
    ```

* `RenderFragment` and `RenderFragment<T>` can be used using the following functions:

    * `attr.fragment : string -> Node -> Attr` for `RenderFragment`;
    * `attr.fragmentWith : string -> (T -> Node) -> Attr` for `RenderFragment<T>`.
    
    For example:
    
    ```fsharp
    open MatBlazor
    
    type Car =
      { Name: string
        Price: decimal
        Horsepower: int }
        
    type Model =
      { Cars: Car[] }
    
    let myTable model dispatch =
        comp<MatTable>
          [ "Items" => model.Cars

            attr.fragment "MatTableHeader" (
              concat
                [ th [] [ text "Name" ]
                  th [] [ text "Price" ]
                  th [] [ text "Horsepower"]
                ]
            )

            attr.fragmentWith "MatTableRow" (fun (car: Car) ->
              concat
                [ td [] [ text car.Name ]
                  td [] [ textf "%.2f" car.Price ]
                  td [] [ textf "%i" car.Horsepower ]
                ]
            )
          ]
          []
    ```


Happy coding!