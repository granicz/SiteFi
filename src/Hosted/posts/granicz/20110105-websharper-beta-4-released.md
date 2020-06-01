---
title: "WebSharper Beta 4 released"
categories: "f#,websharper"
abstract: "Happy new year folks, we just rolled out the WebSharper 2.0 Beta4 release - with an enhanced installer and better support for sitelet-based development with some significant performance improvements. The new installer also became smarter and for new users obtains a 90-day trial license automatically so you no longer have to manually activate."
identity: "1000,74581"
---
Happy new year folks, we just rolled out the [WebSharper 2.0 Beta 4](http://www.intellifactory.com/AssetDownload.aspx?AssetName=WebSharperInstaller-2.0.88.msi) release - with an enhanced installer and better support for sitelet-based development with some significant performance improvements. The new installer also became smarter and for new users obtains a 90-day trial license automatically so you no longer have to manually activate.

It also contains the first batch of the documentation as a Compiled HTML manual (CHM) and a matching PDF book, and all basic and HTML5 samples for Visual Studio 2010 so you can quickly experiment with those samples without having to manually copy them off the website.

All in all, you can get productive immediately after installation, choose between the six available Visual Studio project templates for WebSharper-based development (ASP.NET, MVC, and sitelets), and start developing stunning HTML5 and other client-based web applications. Remember to try sitelets - these allow you to develop markup-less web applications in a single language. Subsequent Beta's will supply even more tools around sitelets, so make sure you follow the beta announcements in the coming weeks.

You can find the combined release notes [here](http://www.websharper.com/ChangeLog.aspx), and the relevant change log below:

 * Avoid dynamically linking to all ~/bin assemblies on startup (#264)
 * DateTime values fail to pass over RPC (#262)
 * Add missing HTML4 attributes/tags to the server-side HTML repre... (#272)
 * Add missing HTML4 attributes/tags to the client-side HTML repres... (#282)
 * Fetch a trial license for ws-356 automatically during installation (#267)
 * Remove install.log when uninstalling WebSharper 2.0 (#268)
 * Add WSM to Program Menu (#269)
 * Add Uninstall menu item to Program Menu (#270)
 * Add chm documentation to the installer (#273)
 * Add PDF documentation to the installer (#281)
 * Add Basic and Html5 samples to installer (#260)
 * Update to 1.4.4 version of jQuery (#283)
 * Css and Attr return a JQuery object (#178)
 * Missing method - Css : string -> string (#226)
 * Fx attribute not found in jQuery (#284)
