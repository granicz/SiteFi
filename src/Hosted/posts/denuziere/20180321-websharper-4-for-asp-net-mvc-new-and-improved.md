---
title: "WebSharper 4 for ASP.NET MVC: new and improved"
categories: "csharp,aspnetmvc,websharper"
abstract: "Today we are releasing WebSharper.AspNetMvc 4.2.0, with a simplified API."
identity: "5513,84826"
---
We are happy to announce the release of [WebSharper 4.2.7](https://www.nuget.org/packages/WebSharper/4.2.7.254), and with it, [WebSharper.AspNetMvc 4.2.0](https://www.nuget.org/packages/WebSharper.AspNetMvc/4.2.0.1). This layer for running WebSharper applications alongside ASP.NET MVC has been simplified compared to its previous 3.x incarnations.

## About WebSharper.AspNetMvc

WebSharper.AspNetMvc is a small package that facilitates the integration of WebSharper in an existing ASP.NET MVC application.

You can see [an example mixed application here](https://github.com/dotnet-websharper/aspnetmvc/tree/master/WebSharper.AspNetMvc.Tests). It demonstrates:

* [Including WebSharper client-side controls in Razor pages;](https://github.com/dotnet-websharper/aspnetmvc/blob/master/WebSharper.AspNetMvc.Tests/Views/Home/Index.cshtml)

* [Calling server-side remote functions from the client side;](https://github.com/dotnet-websharper/aspnetmvc/blob/master/WebSharper.AspNetMvc.Tests/Client/Client.cs)

* [Serving content using WebSharper.](https://github.com/dotnet-websharper/aspnetmvc/blob/master/WebSharper.AspNetMvc.Tests/Controllers/Sitelet.cs)

Learn more in [the documentation](http://developers.websharper.com/docs/v4.x/fs/aspnetmvc).

## Changes from v3.6

In WebSharper.AspNetMvc 3.x, in order to call Remote functions from an MVC page or to run a Sitelet whose URL supersedes an MVC controller, it was necessary to register `WebSharper.AspNet.Filter` with code such as the following:

```csharp
public static void RegisterGlobalFilters(GlobalFilterCollection filters)
{
    filters.Add(new WebSharper.AspNetMvc.Filter());
}
```

**The above code is not necessary anymore.** In fact, the entire `Filter` class is now marked obsolete. Registering WebSharper's HTTP modules in `Web.config` is now sufficient, just like in ASP.NET non-MVC applications:

```xml
<!-- for IIS 7.0 and newer -->
<system.webServer>
  <modules>
    <add name="WebSharper.RemotingModule" type="WebSharper.Web.RpcModule, WebSharper.Web" />
    <add name="WebSharper.Sitelets" type="WebSharper.Sitelets.HttpModule, WebSharper.Sitelets" />
  </modules>
</system.webServer>

<!-- for IIS 6.0 -->
<system.web>
  <httpModules>
    <add name="WebSharper.RemotingModule" type="WebSharper.Web.RpcModule, WebSharper.Web" />
    <add name="WebSharper.Sitelets" type="WebSharper.Sitelets.HttpModule, WebSharper.Sitelets" />
  </httpModules>
</system.web>
```

The properties of `Filter` have either lost their purpose or been moved:

* `SiteletsOverrideMvc` was used to decide what happens when both MVC and WebSharper are able to handle a given URL: if `true` (the default), then the WebSharper sitelet is invoked; otherwise, the MVC controller is invoked.

	This is now handled by the global static property `WebSharper.Sitelets.HttpModule.OverrideHandler`, introduced in WebSharper 4.2.7. If you need to customize this paramater, you should do so in the application's startup:

    ```csharp
    public class MyMvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WebSharper.Sitelets.HttpModule.OverrideHandler = false;
        }
    }
    ```

* `SiteletsModuleName` and `RemotingModuleName` are not needed anymore.

The handling of client-side controls in Razor pages using `WebSharper.AspNetMvc.ScriptManager` remains unchanged; [see the documentation](http://developers.websharper.com/docs/v4.x/fs/aspnetmvc) for more information about it.

## Happy coding!