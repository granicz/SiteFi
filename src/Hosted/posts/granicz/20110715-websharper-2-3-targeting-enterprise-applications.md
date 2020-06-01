---
title: "WebSharper 2.3 - targeting enterprise applications"
categories: "enterprise,f#,websharper"
abstract: "We have been pretty busy these past few months working on a number of enhancements to WebSharper for its 2011 Q2 milestone - and I am happy to announce that the new 2.3 release is now out for public consumption. [more...]"
identity: "987,74568"
---
We have been pretty busy these past few months working on a number of enhancements to WebSharper for its 2011 Q2 milestone - and I am happy to announce that the new 2.3 release is now out for public consumption.

Anton [gave a short overview](http://websharper.com/blog/2011/7/641) about some of the new features in this release, in particular the new deployment layout that packages generated artifacts into the target assembly, greatly simplifying the process of building and deploying WebSharper 2.3 applications.

### Generated artifacts

The WebSharper 2.2 compiler generated a pair of new files for every assembly it processed:

 * A `.dep` file with the dependency information for the assembly, and
 * A `.js` file with the generated JavaScript code for the client-side components.

This had several issues and small annoyances. On one hand, moving three files in sync often grew above the capabilities of MSBuild, leaving stale dependency or generated JavaScript files in place. This meant having to do a manual clean when things "got confused" - a rare, but annoying occurrence that could really steal a few minutes from your daily work. Furthermore, the "single-file-for-the-JavaScript-output" approach enabled no support for debug and release versions for the generated JavaScript code. Here in past versions we made the conscious choice to output verbose, developer-friendly JavaScript, but this was often too heavy and thus slower in production deployments.

With the 2.3 enhancements, WebSharper now produces both debug (verbose, with fewer optimizations applied) and release mode (more optimized and compact) JavaScript output, and packs both of these and the dependency information onto the top of the target assembly. As usual, this involves calling the WebSharper compiler after the F# compiler in sequence, producing an assembly that is "WebSharper-aware" and ready to be dropped in a standard web project with WebSharper enabled.

The standard WebSharper Visual Studio templates are modified to "process" the WebSharper-aware assemblies as part of preparing the web project to be run, dumping out the debug/release versions of the contained JavaScript code in a scripts folder. These are then served based on the "debug" setting in your Web.Config file by the WebSharper runtime.

### Enhanced `Async` support

There is quite a buzz around `Async`'s lately with their inclusion into C# - and a lot of people don't realize that `Async`'s have been a standard part of F# for years now. The previous versions of WebSharper had partial support for asynchronous computations, leaving you without a couple important API members such as `Async.Parallel` despite fully supporting the computation expression syntax. This was due to the inherent single-threaded nature of JavaScript execution engines in most modern browsers, which was a conceptual barrier to any asynchronous implementation.

The current 2.3 release further improves on WebSharper's Async support by implementing a round-robin scheduler with a granularity of a single bind operation (that is, a let! binding inside an Async computation expression) and giving a client-side implementation to Async.Parallel using this scheduler. Overall, this makes it possible to emulate executing multiple asynchronous computations "in parallel", such as initiating multiple web service calls or manipulating the DOM from multiple "agents" executing on your page independently.

We will surely be blogging about this new feature, in the meantime check out [Joel's article on implementing an asynchronous slot machine](http://websharper.com/blog/2011/7/643) using simple and powerful F# Async expressions.

### Sitelet enhancements

Sitelets have been an integral part of the WebSharper Professional product line, a clear differentiator to the WebSharper Standard line. If you have been to any of our talks or listened to our webcasts, you probably are aware that we recommend developing your web applications on a sitelet foundation, e.g. using sitelets to model your application functionality.

The 2.3 release bring a couple smaller enhancements to sitelets:

 * You can now use the tilde (`~`) character in your sitelet template markup files (`*.template.xml`) to refer to the application root directory. In particular, any src and href attribute will be mapped accordingly. This now works correctly even if you are deploying to a virtual directory under IIS.
 * Full authentication support using the ASP.NET authentication pipeline. Previous implementations relied on cookies only, this now is extended to all authentication protocols you may have been used to in your traditional ASP.NET applications.
 * Placeholders in the `<Head>` section of a markup template - it is now possible to place arbitrary placeholders inside the head section in tag positions. These placeholders have a slightly different type than ordinary placeholders.

### Bug fixes and further enhancements

[Anton's announcement](http://websharper.com/blog/2011/7/641) also lists over 30 further bug fixes and enhancements to the installer, the formlet library, updating to the latest jQuery, sitelets, and Visual Studio integration. We certainly recommend updating to the 2.3 release even if you are working on larger enterprise application projects, the new features and enhancements in this release are very much worth the effort.

### Extensions

As usual, the new 2.3 compiler is accompanied by an update to every available WebSharper extension, and the new 2.3 series of each are now available on the [WebSharper Extensions page](http://websharper.com/extensions). This new batch also includes the long-awaited [Twitter extension](http://websharper.com/extension/358-twitter) and the upcoming extensions for [TinyMCE](http://tinymce.moxiecode.com/), a mature and powerful, open-source and platform-independent WYSIWYG HTML editor.

### A tiny glimpse of WebSharper 2.4

In our efforts to bring you the most advanced functional web programming framework, we continue to implement various code optimizations to bring down the generated code size (now in Release mode). The current 2.3 release has no new optimizations, it simply removes whitespace to save you about 20-25% on the generated code size. We plan to implement some key optimizations in the upcoming 2.4 release, giving a massive (50% or more) drop in the generated code size.

The other major enhancement planned in 2.4 is making the jQuery dependency optional. The current releases assume jQuery as the default DOM functionality provider, and although it is technically feasible to implement additional providers, they don't come out-of-the-box with WebSharper. The plan is to support an alternate DOM provider implementation, targeting a more lightweight foundation suitable for running efficiently especially on mobile devices.

If you encounter any issues, please use the [WebSharper Bugzilla](https://bugs.intellifactory.com/websharper) so your issue can be tracked properly. Thanks, and enjoy the current 2.3 release!
