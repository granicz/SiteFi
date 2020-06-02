---
title: "WebSharper - a year in review"
categories: "livedata,jointjs,rappid,forms,review,react,bootstrap,c#,f#,websharper"
abstract: "Just over a year ago, last year in December we released WebSharper 3 on Apache, putting it into the hands of every F# web developer. One thing we learned from WebSharper 2 is that more frequent releases are better and this year kept the whole team busy with constant innovation and new releases. Below is a list I cherry-picked from the WebSharper blog.. [more]"
identity: "4665,81029"
---
## WebSharper – a year in review

Just over a year ago, last year in December [we released WebSharper 3 on Apache]( http://websharper.com/blog-entry/4124/websharper-3-alpha-now-under-apache-2), putting it into the hands of every F# web developer. One thing we learned from WebSharper 2 is that more frequent releases are better and this year kept the whole team busy with constant innovation and new releases. Below is a list I cherry-picked from [the WebSharper blog](http://websharper.com/blog):
 
 * WebSharper 3.0 with support for [source maps](http://websharper.com/blog-entry/4146/websharper-3-0-3-alpha-released), [embedded sitelets]( https://github.com/intellifactory/websharper/issues/307), a [more standardized API](http://websharper.com/blog-entry/4155/websharper-3-0-8-alpha-published) and [namespace changes](http://websharper.com/blog-entry/4241/websharper-3-0-36-alpha-released), better JavaScript interoperability [#1](http://websharper.com/blog-entry/4210/websharper-3-0-26-alpha-released), [#2](http://websharper.com/blog-entry/4241/websharper-3-0-36-alpha-released), [#3](http://websharper.com/blog-entry/4247/websharper-3-0-rc-released), and [#4](http://websharper.com/blog-entry/4323/websharper-3-0-released), [streamlined routing syntax](http://websharper.com/blog-entry/4231/upcoming-in-websharper-3-0-serving-rest-apis-easy-as-pie), [MonoDevelop and Xamarin Studio templates]( http://websharper.com/blog-entry/4242/websharper-3-0-alpha-for-xamarin-studio-monodevelop-is-now-available), [self-hosted applications](http://websharper.com/blog-entry/4356/websharper-3-0-59-released)

 * [WebSharper 3.1](http://websharper.com/blog-entry/4372/websharper-3-1-published) with support for ASP.NET MVC, lightweight syntax to embed client-side code into sitelets, more routing enhancements (wildcard paths, multiple endpoints with a shared URL prefix, posting form data)

 * [WebSharper 3.2](http://websharper.com/blog-entry/4401/websharper-3-2-with-support-for-scriptable-applications-better-resource-management-and-additional-streamlined-syntax) with streamlined syntax for sitelets, dot syntax for event handlers, server-side templating enhancements, more [router syntax simplifications]( http://websharper.com/blog-entry/4412/websharper-3-2-10-released), and [WebSharper.Warp-related enhancements]( http://websharper.com/blog-entry/4418/websharper-3-2-22-released)

 * [WebSharper 3.3](http://websharper.com/blog-entry/4419/websharper-3-3-released-with-client-side-json-serialization) with client-side JSON serialization, F# 4.0 and its new collection functions, and cross-tier server-client-reusable HTML.

 * [WebSharper 3.4](http://websharper.com/blog-entry/4422/websharper-3-4-released) with a revamped Sitelets API, [streamlined HMTL syntax](http://websharper.com/blog-entry/4423/websharper-ui-next-3-4-the-new-html-syntax) (lowercase HTML combinators, more natural attribute syntax, cross-tier includes, client-side extensions, basic reactive templating), cross-site RPC for mobile web applications, [new templates](http://websharper.com/blog-entry/4425/new-websharper-templates). You also get [ASP.NET-hosted OWIN applications, data binding] (http://websharper.com/blog-entry/4547/websharper-3-4-14-released) and a ton of [other enhancements](http://websharper.com/blog-entry/4555/websharper-3-4-19-released) in UI.Next applications, as well as [support for `OnAfterRender`](http://websharper.com/blog-entry/4550/websharper-ui-next-3-4-19-with-onafterrender).

 * WebSharper 3.5 with [pluggable HTML support](http://websharper.com/blog-entry/4584/announcing-websharper-3-5-with-pluggable-html-support), and various fixes [#1](http://websharper.com/blog-entry/4601/websharper-3-5-9-released), [#2](http://websharper.com/blog-entry/4618/websharper-3-5-13-released), [#3](http://websharper.com/blog-entry/4619/websharper-3-5-14-released), and splitting off the internal [testing framework](http://websharper.com/blog-entry/4625/websharper-3-5-16-released) into a separate library

 * WebSharper 3.6 with [CDN support](http://websharper.com/blog-entry/4630/websharper-3-6-released-with-cdn-support), making WebSharper 3.6+ applications blazing fast, and a built-in [cookies library and server-side resources](http://websharper.com/blog-entry/4636/websharper-3-6-6-released).

We have also:

 * [Beefed up the WebSharper website](http://websharper.com/blog-entry/4336/websharper-site-enhancements)

 * Showed how to do [auto-refreshing Azure-deployments on GitHub commits]( http://websharper.com/blog-entry/4367/websharper-from-zero-to-an-azure-deployed-web-application) and provided a new [template](http://websharper.com/blog-entry/4368/deploying-websharper-apps-to-azure-via-github) for these projects

 * [Introduced WebSharper.Warp](http://websharper.com/blog-entry/4409/introducing-websharper-warp): a frictionless WebSharper library for building scripted and standalone OWIN-based, full-stack F# web applications.

 * [Introduced Try WebSharper](http://websharper.com/blog-entry/4424/introducing-try-websharper), an online mini-IDE for developing and [sharing](http://websharper.com/blog-entry/4426/share-and-embed-try-websharper-snippets) WebSharper snippets with on-the-fly type checking, code completion and compiling to JavaScript. You can also [version these snippets, import them from gists](http://websharper.com/blog-entry/4537/try-websharper-snippet-versioning-gist-import-and-other-enhancements-now-available) and add [update notes](http://websharper.com/blog-entry/4540/try-websharper-update-notes-for-snippets) to each version. And best of all, you can use almost [all WebSharper extensions](http://websharper.com/blog-entry/4546/live-f-coding-and-snippets-with-dependencies-in-try-websharper) and you get [on-hover type info]( http://websharper.com/blog-entry/4552/try-websharper-on-hover-type-info) and [extension info](http://websharper.com/blog-entry/4559/try-websharper-version-info-about-extensions-and-some-embedding-improvements) as well.

 * [Introduced WebSharper.Suave](http://websharper.com/blog-entry/4556/announcing-websharper-suave), a middleware for running WebSharper sitelets on Suave, and a new [client-server template](http://websharper.com/blog-entry/4584/announcing-websharper-3-5-with-pluggable-html-support) to help you get started easily.

 * [Rolled out WebSharper.Data and WebSharper.Charting](http://websharper.com/blog-entry/4623/data-aware-workbooks-and-client-side-data-access-with-websharper-data), making it easy to work with heterogeous data sources from client-side code and adding advanced charts and visualizations to your projects.
 
 * Exceeded 1000 topics on the [WebSharper Forums](http://websharper.com/questions).
 
### Conferences and academia

This year we concentrated on getting work out and attended fewer conferences than in previous years.  In 2014, the WebSharper team gave 16 talks on WebSharper in six countries, this year we gave nine talks in six countries.

The team submitted three research papers to academic conferences in 2015, continuing our tradition to publish research results in peer-reviewed conferences.

Next to conferences and research papers, our team has been active in teaching and trainings.  Among others, we run functional reactive web development courses with F# and WebSharper at the Eotvos Lorand University (ELTE) and the University of Dunaujvaros, and have signed a similar agreement with the University of Szeged.  We are also involved in creating an online semester long course for international students.

### Coming up

Our WebSharper team has been cooking up some awesome things that are not yet available or not yet documented. In particular, with more blog entries coming up on each:

 * **WebSharper 4** - we are finalizing the last bits of moving WebSharper to the F# Compiler Services (FCS) and merging the F#+WebSharper compilation steps into a single sweep, giving a significant boost to compilation speed, better F# language coverage, and fixing a number of corner cases in earlier releases. The first alphas are out in early January under a new WebSharper codename “Zafir”.
 
 * **WebSharper 4 for C#** - we are finally able to bring WebSharper to C# developers, covering the most common C# scenarios (asyncs, LINQ, etc.), most of the language, and calls to any one of the existing extensions in the WebSharper ecosystem. A lot more on this in upcoming blogs.
 
 * **WebSharper.React** - bringing [React](https://facebook.github.io/react) to WebSharper applications. Here is a live example:
 
   <div style="width:100%;min-height:300px;position:relative"><iframe style="position:absolute;border:none;width:100%;height:100%" src="http://try.websharper.com/embed/sandorr/00005G"></iframe></div>

 * **WebSharper.LiveData** - automatic syncronization of reactive data models between a server and its participating clients.  You can read a draft of [our first, upcoming tutorial](https://github.com/Tarmil/websharper.docs/blob/master/tutorials/LiveData.md).

 * **WebSharper.Forms** and **WebSharper.Forms.Bootstrap** - reactive web forms (aka. reactive Piglets) with custom rendering. Here is a live example that uses Bootstrap-based rendering for a login form:
 
   <div style="width:100%;min-height:400px;position:relative"><iframe style="position:absolute;border:none;width:100%;height:100%" src="http://try.websharper.com/embed/adam.granicz/00004x"></iframe></div>
 
 * New extensions, in particular **WebSharper.JointJs** and **WebSharper.Rappid** - bringing awesome diagramming solutions to WebSharper:
 
   <div style="width:100%;min-height:400px;position:relative"><iframe style="position:absolute;border:none;width:100%;height:100%" src="http://try.websharper.com/embed/qwe2/00005D"></iframe></div>
 
 * Updating [CloudSharper](http://cloudsharper.com) with the latest WebSharper - this has been left on the backburner for several releases, now it's time to sync the two again.  Once this work is finished, CloudSharper will be your one-stop shop for online web and mobile development with C# and F#; quick data access, analytics and visualization, and a host of other interactive capabilities.
 
This article gave you a quick overview of WebSharper in 2015, and is by no means complete.  One thing is certain: WebSharper remains the primary choice for F# web developers, giving unparalleled productivity and access to the largest F# web ecosystem, and works well with a number of other efforts (ASP.NET, MVC, OWIN, Suave, Hopac, etc.) in the web and development space.

Happy coding and Happy Holidays!
