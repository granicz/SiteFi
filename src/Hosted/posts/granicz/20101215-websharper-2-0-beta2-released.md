---
title: "WebSharper 2.0 Beta2 released"
categories: "f#,websharper"
abstract: "We just released a new WebSharper 2.0 Beta (2.0.63) and a matching WebSharper Manager (2.0.10) - please go to the WebSharper Download Page to grab the new binaries."
identity: "1004,74585"
---
We just released a new WebSharper 2.0 Beta (2.0.63) and a matching WebSharper Manager (2.0.10) - please go to the [WebSharper Download Page](http://www.websharper.com/Downloads.aspx) to grab the new binaries.

This beta release delivers a good number of improvements, most notably to sitelets and various speed and installation issues. We have also revised formlet styling and, among others, the new formlet library comes with a cleaner, more attractive look:

<img src="/assets/personForm.png">

Here is the change log:

* Newly installed WebSharper fails with Invalid license issue date (#252)
* License Invalid after obtaining a license on Win-7 (#246)
* Compilation fails on rebuild (#244)
* Module translation can shadow global namespace (#233)
* Page requests take ~10 seconds before they are processed (#210)
* WebSharper output does not work in IE7 (#216)
* HTTP headers configurable for pages (#240)
* Add web control for embedding HTML elements (#239)
* xhtml2fs is missing some irregular tag/attribute name conversions (#238)
* xhtml2fs fails on templates that have empty <body>'s (#237)
* Self-reference during compilation causes .DEP reading errors (#235)
* Missing CSS method on jQuery (#226)
* .dep files not portable across .NET versions (#223)
* Async.Start fails in IE (#222)
* Cannot select dates in a calendar without toggling it first (#221)
* Event.RemoveHandler error in IE (#220)
* Lost AfterRender (#219)
* google.maps.MapTypeId.ROADMAP undefined (#209)
* Formlet Layout breaks Render contract (#208)

Your feedback is very important to us. Please go to the [WebSharper Community](http://www.websharper.com/Extensions.aspx) page to submit any further issues you may find. Here you can find various forums to help you get started with WebSharper.