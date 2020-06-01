---
title: "WebSharper 2010 Standard is available"
categories: "f#,websharper"
abstract: "This is an exciting time for the F# world. Monday, April 12, witnessed the launch of VisualStudio 2010 and the release of the official F#, 2.0. Great job, F# team!On our part, today we are pleased to announce the availability of WebSharper Platform 1.0."
identity: "1016,74597"
---
These are exciting times for the F# world! Monday, April 12, witnessed the launch of Visual Studio 2010 and the [release of the official F# 2.0](http://blogs.msdn.com/dsyme/archive/2010/04/12/f-2-0-released-as-part-of-visual-studio-2010.aspx). Great job, F# team!

On our part, today we are pleased to announce the availability of WebSharper Platform 2010, version 1.0.

WebSharper is a platform for developing web applications in F# which includes:

 * A compiler from F# assemblies to JavaScript. WebSharper applications implement all client-side and server-side logic in F#, and run the client-side logic in JavaScript.
 * Support for nearly the entire F# language, many core F# libraries, and some .NET standard libraries in the JavaScript environment.
 * A custom RPC protocol for seamlessly communicating between the client and the server.
 * ASP.NET integration, with a way to package and deploy mixed client/server components, and manage their resource dependencies.
 * Visual Studio 2008 and 2010 integration with a WebSharper project template, build enhancements, and error location reporting.

The main highlights of this release are below.

### Licensing

WebSharper 2010 Standard is available free of charge for both commercial and non-commercial purposes. We offer various support packages for individuals to large workgroups, and even entire enterprises, and premium extensions on our [Purchase page](http://www.intellifactory.com/products/wsp/Buy.aspx). We also have a full range of [training courses](http://www.intellifactory.com/Trainings.aspx), consultation and expert services available. For more details, please send an email to `sales@intellifactory.com`, or use our [contact form](http://www.intellifactory.com/ContactUs.aspx).

### The compiler

 * The error reporting has been much improved since the beta release. The compiler reports errors and warnings in bulk. Most messages carry source locations, allowing to click on them in Visual Studio.
 * The compilation of assemblies has been separated to avoid unnecessary recompilation, improving compilation time.
 * There are numerous improvements in the output code, to make it more compact, readable and efficient.

### The standard library support

 * Support for `Set<'T>` and `Map<'T1,'T2>` with a backend based on AVL trees.
 * Support for `System.DateTime` and `System.TimeSpan`. Support for passing `System.DateTime` objects over RPC calls.

### The language support

 * Thanks to the improvements in the recent F# compiler, constructors are no longer special-cased. Just like ordinary methods, they can now be marked with JavaScriptAttribute and implemented in F#.
 * More native JavaScript functionality is exposed for F# consumption, including such classes as `JConsole`, `JDate`, `JObject` (for JavaScript `Console`, `Date` and `Object` respectively).
 * Dom and jQuery bindings are likewise exposed for F#.

### ASP.NET integration

 * WebSharper now integrates via an `IHttpModule`, detecting RPC call requests by a special header. This solution is robust to deploying WebSharper applications in arbitrary virtual folders.

### The installer

 * Installation of templates for both Visual Studio 2008 and 2010, on 64 and 32-bit platforms.
 * Support for in-place upgrades and hotfix releases - the new versions of WebSharper 1.0 will be distributed as installers that will not require you to make any changes to your WebSharper projects in order to benefit from the latest versions' improvements.

We would love to hear from you! You can access a ton of information about WebSharper, or its growing collection of extensions on the [WebSharper home page](http://www.intellifactory.com/products/wsp/Home.aspx).
