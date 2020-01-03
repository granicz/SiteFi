namespace Hosted

open System
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
//open Microsoft.AspNetCore.Configuration
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open WebSharper.AspNetCore

type Startup() =

    member this.ConfigureServices(services: IServiceCollection) =
        let sitelet = new Website.Website() :> WebSharper.Sitelets.IWebsite<_>
        //services.Configure<KestrelServerOptions>(fun (options:KestrelServerOptions) ->
        //    options.AllowSynchronousIO <- true
        //)
        //|> ignore

        //services.Configure<IISServerOptions>(fun (options:IISServerOptions) -> 
        //    options.AllowSynchronousIO <- true
        //)
        //|> ignore

        services.AddSitelet(sitelet.Sitelet)
            .AddAuthentication("WebSharper")
            .AddCookie("WebSharper", fun options -> ())
        |> ignore

    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        if env.IsDevelopment() then app.UseDeveloperExceptionPage() |> ignore

        app.UseAuthentication()
            .UseStaticFiles()
            .UseWebSharper()
            .Run(fun context ->
                context.Response.StatusCode <- 404
                context.Response.WriteAsync("Page not found"))

module Program =
    let BuildWebHost args =
        WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseWebRoot(".")
            .Build()

    [<EntryPoint>]
    let main args =
        BuildWebHost(args).Run()
        0
