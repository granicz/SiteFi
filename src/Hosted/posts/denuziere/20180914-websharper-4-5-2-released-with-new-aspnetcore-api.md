---
title: "WebSharper 4.5.2 released with new AspNetCore API"
categories: "aspnetcore,ui,csharp,fsharp,javascript,websharper"
abstract: "This includes a more idiomatic API for WebSharper.AspNetCore."
identity: "5626,85883"
---
This is a minor release for WebSharper. The main highlight of this release is the improved API for WebSharper.AspNetCore.

Install templates for .NET Core SDK: `dotnet new -i WebSharper.Templates::4.5.2.275`

Download templates for Visual Studio 2017: http://websharper.com/installers/WebSharper.4.5.2.275.vsix

# WebSharper

## Features

* [#1020](https://github.com/dotnet-websharper/core/issues/1020): `Sitelets.HttpModule` now throws an error if no Sitelet definition is found.
* [#1022](https://github.com/dotnet-websharper/core/issues/1022): Update to FSharp.Compiler.Service 25.0.1

## Fixes

* [#1023](https://github.com/dotnet-websharper/core/issues/1023): Fix HTML escaping in `HtmlTextWriter`. This affects functions such as `text` and `attr.*` in WebSharper.UI.

# WebSharper.AspNetCore

WebSharper.AspNetCore now has a more idiomatic API and makes it easier to add your code to the dependency injection graph.

Here is how a typical current application should be ported to the new API:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSitelet(Site.Main) // <-- The sitelet should now be registered here
        		.AddAuthentication("WebSharper")
                .AddCookie("WebSharper", options => { });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

        // Not necessary anymore; WebSharper uses the host config's "websharper" section by default
        /*
        var config =
            new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .Build();
        */

        app.UseAuthentication()
            .UseStaticFiles()
            // .UseWebSharper(env, Site.Main, config.GetSection("websharper"))
            .UseWebSharper() // <-- This is now sufficient
            .Run(context => {
                context.Response.StatusCode = 404;
                return context.Response.WriteAsync("Page not found");
            });
    }
}
```

The new API is as follows:

* `IApplicationBuilder.UseWebSharper(Action<WebSharperBuilder> builder = null)`

    Adds WebSharper middleware to the application. Options can be passed using the `builder` action. For example:
    
    ```csharp
    app.UseWebSharper(builder =>
            builder.UseSitelets(false)
                   .Config(config.GetSection("mywebsharper")))
    ```

* `IServiceCollection.AddSitelet<T>(Sitelet<T> sitelet)`

    Defines the sitelet that will be served by WebSharper if `WebSharperBuilder`'s `UseSitelets` is `true` (which is the default).
    
    Note that if `UseSitelets` is `true` but no `AddSitelet` is called, WebSharper will look in the application's assemblies for a static value marked with the `[Website]` attribute.

* `IServiceCollection.AddSitelet<T: ISiteletService>()`

    Defines the sitelet that will be served by WebSharper if `WebSharperBuilder`'s `UseSitelets` is `true` (which is the default).
    
    `ISiteletService` is a singleton dependency injection service. Here is an example showing how to implement such a service:
    
    ```csharp
    public class MySiteletService : ISiteletService
    {
        ILogger<MySiteletService> logger;
        Sitelet<object> sitelet;

        public MySite(ILogger<MySiteletService> logger)
        {
            this.logger = logger;
            this.sitelet =
                new SiteletBuilder()
                    .With("/", ctx =>
                    {
                        logger.LogInformation("Serving home page");
                    	return Context.Text("Hello, world!");
                    })
                    .Install();
        }

        public Sitelet<object> Sitelet = sitelet;
    }
    ```
    
    In F#, you should use the abstract class `SiteletService<'T>`, which implements `ISiteletService` and is parameterized by the endpoint type:
    
    ```fsharp
    type MyEndPoint =
        | [<EndPoint "/">] Home

    type MySiteletService(logger: ILogger<MySiteletService>) =
        inherit SiteletService<MyEndPoint>()

        let sitelet = Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | Home ->
                logger.LogInformation("Serving home page")
                Content.Text "Hello, world!"
        )
        
        override this.Sitelet = sitelet;
    ```
    
* `IServiceCollection.AddWebSharperRemoting<THandler>()`

    Registers a WebSharper remoting handler. This is equivalent to calling the following:
    
    ```csharp
    WebSharper.Core.Remoting.AddHandler(typeof(THandler), new THandler());
    ```
    
    with the added benefit that `THandler` can depend on dependency injection services. [Learn more about remoting handlers.](https://developers.websharper.com/docs/v4.x/cs/remoting)

The old API is still available but marked as obsolete.

# WebSharper.UI

## Features

* [#192](https://github.com/dotnet-websharper/ui/issues/192) Add the ability to instantiate a template from a string.

    You can now pass a string to the constructor of a template to override the contents of the template. For example:
    
    ```fsharp
    type MyTemplate = Template<"""<p>Hello, ${Text}!</p>"""
    
    // Returns <p>Hello, world!</p>
    let usual =
        MyTemplate()
            .Text("world")
            .Doc()

    // Returns <p>Howdy, world!</p>
    let overridden =
        MyTemplate("""<p>Howdy, ${Text}!</p>""")
            .Text("world")
            .Doc()
    ```
    
    This feature is currently only available in F# on the server side.

Happy coding!