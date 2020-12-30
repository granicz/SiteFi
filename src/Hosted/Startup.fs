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
open Website

type Startup() =

    member this.ConfigureServices(services: IServiceCollection) =
        Site.__config := Site.ReadConfig()
        let _info, _articles = Site.ReadArticles Site.__config.Value
        Site.__info := _info
        Site.__articles := _articles
        Site.__identities1 := Site.ComputeIdentities1 Site.__articles.Value

        //services.Configure<KestrelServerOptions>(fun (options:KestrelServerOptions) ->
        //    options.AllowSynchronousIO <- true
        //)
        //|> ignore

        //services.Configure<IISServerOptions>(fun (options:IISServerOptions) -> 
        //    options.AllowSynchronousIO <- true
        //)
        //|> ignore

        services.AddSitelet(Site.Main Site.__config Site.__identities1 Site.__info Site.__articles)
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
