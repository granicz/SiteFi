---
title: "WebSharper 4.2 released"
categories: "aspnetcore,csharp,fsharp,websharper"
abstract: "It is finally here! Today we are releasing WebSharper 4.2."
identity: "5507,84664"
---
# WebSharper 4.2 released

It is finally here! Today we are releasing WebSharper 4.2.

The main highlight of this release is the support for .NET Standard 2.0 and ASP.NET Core.

New features also make getting started with WebSharper easier than ever:
* With `dotnet` templates, creating a new C# or F# WebSharper project is now done in a single command line.	
    ```
    dotnet new websharper-spa -lang f#
    ```
* WebSharper configuration for a project is now stored in a simple JSON file.

![Visual Studio Code screenshot](https://i.imgur.com/fGZ61Ge.png "A WebSharper app in Visual Studio Code")

## Installing

You can install or update the templates to create WebSharper projects via the following means:

* in Visual Studio 2017, from [the downloads page](http://websharper.com/downloads).

* for the `dotnet` command line, by typing the following command (see below for more details:

    ```
    dotnet new -i WebSharper.Templates
    ```

You can also add WebSharper to an existing project. Simply add the NuGet package `WebSharper.FSharp` or `WebSharper.CSharp` (depending on your language of choice) and add a [wsconfig.json file](#wsconfigjson).

Now, let's see the changes that are coming in this release.

## .NET Standard and .NET Core

WebSharper's built-in libraries, as well as the entire library of standard extensions, are now available for .NET Standard 2.0. This means that you can now run WebSharper applications on .NET Core 2.0 on Windows, OS X and Linux.

There are a small number of differences between the .NET Framework 4.6.1 and .NET Standard 2.0 versions of the WebSharper libraries, related to the fact that `System.Web` is exclusive to .NET Framework:

* `WebSharper.Web.RpcModule` and `WebSharper.Sitelets.HttpModule` are only available for .NET Framework.
* `WebSharper.Web.Control` inherits from `System.Web.UI.Control` in .NET Framework, and from `System.Object` in .NET Standard.
* `WebSharper.Core.Resources.HtmlTextWriter` inherits from `System.Web.UI.HtmlTextWriter` in .NET Framework, and from `System.IO.TextWriter` in .NET Standard.
* `WebSharper.Sitelets.IHostedWebsite`, which builds a sitelet from a `System.Web.HttpApplication`, is only available in .NET Framework.

## ASP.NET Core support

WebSharper Client-Server applications can now be hosted on ASP.NET Core thanks to WebSharper.AspNetCore. It is a simple interface that is very easy to connect:

```fsharp
// Your WebSharper website
module Site =
    let Main = // ...

// Serve the WebSharper website
type Startup() =
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        app.UseStaticFiles()
            .UseWebSharper(env, Site.Main)
```

WebSharper uses runtime configuration for things such as overriding the URL of dependent resources. WebSharper.AspNetCore provides this configuration from the standard runtime configuration system for ASP.NET Core, `Microsoft.Extensions.Configuration`:

```fsharp
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        // Create the configuration object...
        let config =
            ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build()

        app.UseAuthentication()
            .UseStaticFiles()
            // ... and pass it to WebSharper.
            .UseWebSharper(env, Site.Main, config.GetSection("websharper"))
```

You can then pass WebSharper configuration in `appSettings.json`:

```javascript
{
    "websharper": {
        "WebSharper.JQuery.Resources.JQuery": "https://code.jquery.com/jquery-3.2.1.min.js"
    }
}
```

WebSharper authentication also uses the standard methods:

```fsharp
    // Configure user authentication...
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddAuthentication("WebSharper")
            .AddCookie("WebSharper", fun options -> ())
        |> ignore

    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =
        // ... and use it in the application.
        app.UseAuthentication()
            .UseStaticFiles()
            .UseWebSharper(env, Site.Main)
```

## `dotnet` project templates

To easily create new .NET Standard and ASP.NET Core WebSharper projects, we created templates for the `dotnet new` command. You can install these templates with the following command:

```
dotnet new -i WebSharper.Templates
```

Once this is done, you can create a WebSharper project with one of the following commands (adding `-lang f#` or `-lang c#` to choose the language, with `c#` being the default):

* `dotnet new websharper-web`
    This creates a Client-Server ASP.NET Core web project.
* `dotnet new websharper-spa`
    This creates a Single-Page Application project. It is hosted in an ASP.NET Core project, so that you can quickly run it as well as serve RPCs to your SPA.
* `dotnet new websharper-lib`
    This creates a simple WebSharper-compiled library.
* `dotnet new websharper-html`
    This creates a static generated website.

## `wsconfig.json`

WebSharper compilation can now be configured using a JSON file called `wsconfig.json` located next to your project file. Its content overrides the corresponding properties set in your `.*proj` file. Here is an example:

```javascript
{
 "project": "bundle",
 "dce": false,
 "outputDir": "scripts/",
 "warnOnly": true
}
```

[Read the full documentation about wsconfig.json](https://developers.websharper.com/docs/v4.x/fs/project-variables).

The F# tooling extension for Visual Studio Code, Ionide, provides code completion for wsconfig.json.

## Other changes

* Updated FSharp.Compiler.Service to version 20.0.1, which makes compilation faster.

* Support [C# 7.2 language features](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-7-2#reference-semantics-with-value-types): reference semantics with value types, non-trailing named arguments.

* A new compilation mode, `bundleOnly`, allows faster compilation of SPA projects by skipping some parts of the C# or F# compilation that are unnecessary if you don't reference this project from other .NET projects. Use it by setting `"project": "bundleOnly"` in wsconfig.json, or `<WebSharperProject>BundleOnly</WebSharperProject>` in your .\*proj file if you are not using wsconfig.json.
    
* WebSharper.UI enhancements:
    * `View.WithInit init view` creates a View that immediately returns a given initial value `init` if the input `view` is still awaiting.

        ```fsharp
        module Server =
            [<Rpc>]
            let Greet (x: string) =
                async { return "Hello, " + x }

        [<JavaScript>]
        module Client =
            let userInput = Var.Create "world"

            let vDoubled =
                userInput.View
                |> View.MapAsync Server.Greet
                |> View.WithInit "Deciding how to greet you..."
        ```

        Similarly, `View.WithInitOption view` immediately returns `None` if the input `view` is still awaiting, then returns `Some` once `view` has a value.

    * `Attr.DynamicClass`'s API was inconsistent with other `Attr.*` functions: it was a generic function taking a `View<'T>` and a mapping function `'T -> bool`, rather than directly a `View<bool>`. It is now deprecated, and the new `Attr.DynamicClassPred` has the more standard behavior.

        Additionally, the new `Attr.ClassPred` is equivalent to `DynamicClassPred` but takes its argument as a `bool` rather than a `View<bool>` and is reactive when using `.V`. It is now the recommended way to create a class that is reactively set or unset.
        
        ```fsharp
        let currentPage = Var.Create "home"

        // Obsolete version:
        let hideIfHome() =
            Attr.DynamicClass "hidden" currentPage.View (fun x -> x = "home")
            
        // New recommended version:
        let hideIfHome() =
            Attr.ClassPred "hidden" (currentPage.V = "home")
        ```

Happy coding!