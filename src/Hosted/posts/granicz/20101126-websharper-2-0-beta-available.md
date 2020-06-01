---
title: "WebSharper 2.0 Beta available"
categories: "f#,websharper"
abstract: "We are making excellent progress on releasing WebSharper 2.0 in the coming weeks, and have just put out the first beta version of WebSharper 2.0, ready for some public testing and community feedback. Here are the links:"
identity: "1006,74587"
---
We are making excellent progress on releasing WebSharper 2.0 in the coming weeks, and have just put out the first beta version of WebSharper 2.0, ready for some public testing and community feedback. Here are the links (BE AWARE that these may become stale as new beta versions are released, so ALWAYS check the main WebSharper page for the latest download links):

 * WebSharper 2.0.49 - [WebSharper 2.0.49.msi](http://www.intellifactory.com/AssetDownload.aspx?AssetName=WebSharper-2.0.49.msi)
 * WebSharper Manager - [WebSharperManager-2.0.0.msi](http://www.intellifactory.com/AssetDownload.aspx?AssetName=WebSharperManager-2.0.0.msi)

Keep in mind that these are BETA releases, and may and probably do contain bugs and have various issues. We are aware of a number of issues and are working on resolving those for the next Beta releases, but none of these should have any significant impact on your ability to test the current release.

### Introducing WebSharper Manager

To work with WebSharper 2.0 and any of the upcoming premium extensions, a valid product license is required for each component on every development machine. We have trial and developer licenses, and soon will be announcing community and academic licenses also. These may be obtained and managed with a new utility program called the WebSharper Manager (WSM) - see the link for a separate installer above. At this time, WSM serves as a license manager - but we are adding functionality to equip it with support features such as filing tickets, viewing resolutions, updating components, scheduling updates, etc. We recommend that WSM is installed on every machine where you intend to develop with WebSharper (at this point there is no other way to activate installations) - it will prove invaluable in keeping the members of your development team informed about new WebSharper components and any support items and their resolutions. WSM itself needs no license to run.

Since the main activation site is not yet operational, we have installed a temporary activation site with "dummy data" for early insiders and folks testing. The address of this temporary activation site is baked into the Beta release of WSM. Once the regular Beta releases are available and the official activation site opened, there will be a matching WSM release and all activation data will be transferred to the live activation site.

<img src="/assets/WSM-beta.png">

Upon start, WSM will retrieve specific WebSharper product information from the configured activation site and display all known products and their activation status. Green items don't need a license, orange ones do, blue ones show those that are licensed already. You can obtain a license to any component (including WebSharper Professional) by requesting a trial license (30, 60, or 90 days, depending on the component) with a single click, or by entering a valid S/N you obtained one way or another.

<img src="/assets/WSM-beta-activate.png">

### What's in WebSharper 2.0 Beta?

We will be blogging extensively about the upcoming features in the next couple weeks, until then here is a short list of some of the main items:

 * Sitelets - the latest WebSharper feature. You can use sitelets to compose entire sites as first-class site values, even with embedded/composed client-side functionality, and use site-wide combinators to add behavior.
 * HTML providers - compose HTML reusing the capabilities of external frameworks (or default to jQuery)
 * Generalized formlets - plug in your own reactive framework such as RxJs or Flapjax (or default to our built-in one)
 * Unified flowlets and dependent formlets - to model complex UI interactions and transitions
 * Super-fine granularity for resource tracking - JavaScript resources are loaded automatically whenever they are needed, and only then (as opposed to dependencies coming from type/assembly level)
 * More F# and .NET coverage - seamlessly translate active patterns, units of measure, inheritance/interfaces, etc. to JavaScript
 * WebSharper Manager to help you explore and try out new WebSharper extensions/tools/libraries
 * WebSharper Sample Web Application (ASP.NET) - Visual Studio solution template
 * WebSharper Sample Web Application (ASP.NET MVC) - Visual Studio solution template
 * WebSharper Sample Web Application (Sitelets) - Visual Studio solution template

### What about extensions?

WebSharper 2.0 will have a TON of new extensions available shortly after the WebSharper 2.0 release, coming in small waves to cover a wide range of third-party technologies, from advanced visualization to massive UI control sets.

WebSharper 2.0 itself comes bundled with a number of "core" extensions, so you can enjoy them immediately:

 * Web foundation (`IntelliFactory.Html`, `IntelliFactory.JavaScript`, `IntelliFactory.Json`, `IntelliFactory.Formlet.Base`, `IntelliFactory.Reactive`, etc.)
 * `IntelliFactory.WebSharper.Dom`
 * `IntelliFactory.WebSharper.EcmaScript`
 * `IntelliFactory.WebSharper.Formlet`
 * `IntelliFactory.WebSharper.Html`
 * `IntelliFactory.WebSharper.JQuery`

### Where to send feedback?

You can send us your feedback, bug reports, etc. at `websharper-reports@intellifactory.com`, or use the [Bug Submission page](http://www.intellifactory.com/products/wsp/Support.aspx) on this website. We look forward to hearing from you!
