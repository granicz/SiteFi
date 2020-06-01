---
title: "WebSharper 0.4.62 is available - with dependent formlets-flowlets-third-party extensions-and more!"
categories: "f#,websharper"
abstract: "We are happy to announce the availability of WebSharper 0.4.62 - the latest version of the WebSharper Platform that aims to equip professional F# developers with the right platform to rapidly develop rich, scalable, client-based web applications.  This is the first release candidate of WebSharper Platform 2010 - our commercial offering for client-based reactive web development with F#. In this release there are a number of new features, including: ..."
identity: "1021,74602"
---
We are happy to announce the availability of WebSharper 0.4.62 - the latest version of the WebSharper Platform that aims to equip professional F# developers with the right platform to rapidly develop rich, scalable, client-based web applications. This is the first release candidate of WebSharper Platform 2010 - our commercial offering for client-based reactive web development with F#. In this release there are a number of new features, including:


 * **Dependent formlets** - an embedded DSL for describing formlets whose parts depend on other parts of the same or other formlets.
 * **Flowlets** - an embedded DSL for describing sequences of web forms (such as steps in a registration page) as a strongly typed, first-class value in F#.
 * **Extensions for the Google Maps API** - you can build rich, interactive maps, street views, route planners, and other navigational functionality using the Google Maps API through WebSharper bindings - all with F# code and without a single line of JavaScript.
 * **Extensions for the Google Visualization API** - you can create stunning interactive visualizations (maps, charts, graphs, gauges, etc.) using the Google Visualization API through WebSharper - again, all with F# code.
 * **First-class resources** - pagelets can define their dependencies and the resources they need inside the F# type system, making it type-safe and robust, and freeing you from having to track various artifacts (style sheets, images, includes, etc.).



Go and grab your copy at our [download](http://www.intellifactory.com/products/wsp/Download.aspx) page, or check out the [demos](http://www.intellifactory.com/products/wsp/Tutorial.aspx) we have online (and don't forget to use the drop-down on the right to see all demos). More demos, tutorials, and some long-awaited screencasts will be coming shortly.
One last bit: you may get exceptions in Visual Studio debug mode (which is misleading, as most of your WebSharper code will be executing in the client browser) from WebSharper code that performs HTML construction - you can safely ignore these as the WebSharper ASP.NET integration layer will take care of them. We are working on removing this annoyance in the next release candidate.