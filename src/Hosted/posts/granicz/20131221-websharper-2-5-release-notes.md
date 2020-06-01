---
title: "WebSharper 2.5 Release Notes"
categories: "f#,websharper"
abstract: "We are happy to announce the general availability of WebSharper 2.5, and the corresponding new WebSharper site that has gone through a major facelift. I plan to blog about the major individual features in detail as separate blog entries, until then, here is a short list of the main changes."
identity: "3627,76908"
---
We are happy to announce the general availability of [WebSharper 2.5](http://websharper.com/downloads), and the corresponding new [WebSharper site](http://websharper.com) that has gone through a major facelift. I plan to blog about the major individual features in detail as separate blog entries, until then, here is a short list of the main changes.

### Switched to NuGet

You can now grab WebSharper or WebSharper extensions directly from NuGet, on demand when your project needs them. This gives a simple distribution and upgrade model for the compiler and extensions, however, we recommend that you install WebSharper itself through the Visual Studio [.VSIX extension](http://websharper.com/websharper.vsix) to benefit from the various WebSharper templates.

If you have any installed legacy (2.4 or older) WebSharper binaries and you no longer wish to use them in your legacy projects, this would be a good time to uninstall those.

### Upgraded to F# 3.0

For WebSharper 2.5, we decided to move to F# 3.0 to better benefit from the new language features. F# 3.1 support is coming. By default, you will also need .NET 4.5 installed, as this is the new target framework for WebSharper 2.5. Those who wish to use WebSharper 2.5 on .NET 4.0 can use the new branch created for that. See #185 for details.

### Support for VS 2012 Express and VS 2013

WebSharper 2.5 now works with the full Visual Studio 2012 suite, including the free Express For Web edition. It also works with Visual Studio 2013.

### Support for Web.API and OWIN

WebSharper 2.5 now supports running WebSharper sitelets via Web.API, allowing to host sitelets standalone in a variety of web containers via OWIN or integrate them with latest ASP.NET MVC and Web.API projects (via the separate [websharper.webapi project](https://bitbucket.org/IntelliFactory/websharper.webapi)).

### Support for pure-F# web projects without template indirection

WebSharper 2.4 sitelets required a C# host web application. WebSharper 2.5 removes the need for these host projects and supports pure F# web projects with sitelets embedded.

### More open sourced components

We have open sourced [WebSharper Mobile](https://bitbucket.org/IntelliFactory/websharper.mobile), the component that supplies packaging and Visual Studio templates for Android and Windows Phone; and several key extensions ([Bing Maps](https://bitbucket.org/IntelliFactory/websharper.bing.maps), [Google Maps](https://bitbucket.org/IntelliFactory/websharper.google.maps), [Google Visualization](https://bitbucket.org/IntelliFactory/websharper.google.visualization), [WebGL](https://bitbucket.org/IntelliFactory/websharper/raw/tip/IntelliFactory.WebSharper.Html5/Definition.fs), [Google O3D](https://bitbucket.org/IntelliFactory/websharper.o3d), [GlMatrix](https://bitbucket.org/IntelliFactory/websharper.glmatrix), [jQuery UI](https://bitbucket.org/IntelliFactory/websharper.jquery.ui), [jQuery Mobile](https://bitbucket.org/IntelliFactory/websharper.jquerymobile)).

### New extensions

We have released two new extensions ([D3](https://bitbucket.org/IntelliFactory/websharper.d3), and HighCharts), and have several upcoming ones in the pipeline for HTML5 games, mobile application development, and charting and visualization.

### Introducing Piglets

Pluggable Interactive GUIlets - [Piglets](http://websharper.com/docs/piglets) - are an improved formalism similar to [formlets](http://websharper.com/docs/formlets), that provides for better UI and logic separation. We have published the first results of this new formalism in our upcoming IFL 2013 paper.

### Experimental: Bundling support

WebSharper can now merge all offline dependencies into a single bundle file, making it easier to package Single Page Applications and offline sitelets, and manage optional and on-demand loading of resources. To use bundling you can supply an additional command line argument (`bundle`) to `websharper.exe`, or simply use the new Visual Studio project template (Website Bundle) that we added to the VSIX installer. This is an experimental feature, see #128 for details.

### TypeScript support for generated JS code descriptors

WebSharper now outputs `.d.ts` files for each generated JavaScript file, making it easier to interface with those files from TypeScript. This is an experimental feature, see ticket #144 for details.

### TypeScript type provider for TypeScript 0.9.x

We have work underway to augment WebSharper 2.5 with a new TypeScript type provider that can read TypeScript declarations and create WebSharper extensions out of them on the fly. This component is released separately and its availability will be announced shortly.

### WebGL

We moved the WebGL bindings to the main WebSharper repository, so you can now develop WebGL-based mobile and web applications without an extension. See ticket #159 for details.

### A ton of smaller enhancements and fixes

Bug fixes and extended proxy support for .NET methods: #70, #71, #72, #73, #74, #75, #77, #78, #79, #80, #83, #86, #87, #90, #91, #92, #93, #96, #101, #107, #109, #110, #111, #112, #114, #117, #118, #123, #125, #126, #133, #134, #136, #139, #145, #148, #152, #153, #154, #156, #158, #160, #161, #162, #163, #164, #165, #166, #169, #170, #172, #176, #177, #178, #180, #182, #183, #184, #186, #187. This includes support for for/while syntax in async workflows, array and string slicing (#136), better `DateTime` and `TimeSpan` support (#134), improved compilation strategies for sequence operations (#145) and mutable variables (#148)

Happy coding!
