---
title: "Self-hosted WebSharper application template available"
categories: "websharper"
abstract: "A new Visual Studio template has been added to the latest Visual Studio installer called \"Self-Hosted Client-Server Application,\" and you can use this template to build OWIN-based self-hosted applications that can be deployed via an .exe file and without requiring an installed web container (IIS, etc.) on the serving machine."
identity: "4098,77529"
---
A new Visual Studio template has been added to the latest [Visual Studio installer](http://websharper.com/WebSharper.vsix) called "Self-Hosted Client-Server Application," and you can use this template to build OWIN-based self-hosted applications that can be deployed via an `.exe` file and without requiring an installed web container (IIS, etc.) on the serving machine.

The following code shows embedding a given sitelet (`Site.MainSitelet`) into an OWIN container and starting it:

```fsharp
module SelfHostedServer =

    open global.Owin
    open Microsoft.Owin.Hosting
    open Microsoft.Owin.StaticFiles
    open Microsoft.Owin.FileSystems
    open IntelliFactory.WebSharper.Owin

    [<EntryPoint>]
    let Main = function
        | [| rootDirectory; url |] ->
            use server = WebApp.Start(url, fun appB ->
                appB.UseStaticFiles(
                        StaticFileOptions(
                            FileSystem = PhysicalFileSystem(rootDirectory)))
                    .UseSitelet(rootDirectory, Site.MainSitelet)
                |> ignore)
            stdout.WriteLine("Serving {0}", url)
            stdin.ReadLine() |> ignore
            0
        | _ ->
            eprintfn "Usage: OWIN1 ROOT_DIRECTORY URL"
            1
```

The OWIN machinery to make this work has been released as a new `WebSharper.Owin` NuGet package as well. The new Visual Studio template contains this boilerplate code, takes care of fetching the dependent OWIN packages, and calls the generated OWIN container `.exe` with the right arguments to host your sitelets easily.

Work to add this new WebSharper project template to the MonoDevelop/Xamarin Studio integration is underway.

Let us know what you think and happy coding!
