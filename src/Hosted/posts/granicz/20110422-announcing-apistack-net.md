---
title: "Announcing ApiStack.net"
categories: "apistack.net,f#,websharper"
abstract: "Have you ever wanted to give your F# library users a nice and clean API documentation but didn't have the right tools to create it? Or you have tried all kinds of API documentation tools and none of them worked for your F# assemblies? Well, now you have to look no further - we have the perfect tool for you: ApiStack.net."
identity: "990,74571"
---
Have you ever wanted to give your F# library users a nice and clean API documentation but didn't have the right tools to create it? Or you have tried all kinds of API documentation tools and none of them worked for your F# assemblies? Well, now you have to look no further - we have the perfect tool for you: [ApiStack.net](http://apistack.net/).

### Introducing ApiStack.net (a.k.a. FsApis.net)

The concept of ApiStack.net is very simple: you upload the assemblies (one or more) of your project and you get a nice API browser prepared and hosted on ApiStack.net. You can give the Permalink to your users or redirect to it from your website, your users won't even see that it's hosted elsewhere.

<img src="/assets/ApiStack.png">

### Why ApiStack.net?

For one thing, it works well on F# assemblies, in fact it's been designed so - unlike most other "mainstream" documentation generation tools that didn't catch up with F# yet. So it can render your tuple and record types correctly, it will display your custom F# operators properly, and it will adjust over the F#-specific details so you don't have to worry about any of that internals stuff.

### Why is ApiStack.net so special?

[ApiStack.net](http://apistack.net/) is a [WebSharper](http://websharper.com/) application modeled entirely as a [sitelet](http://websharper.com/docs/WorkingWithSitelets.aspx). It uses no ASPX markup and all of its code (~55 KB total, a dozen 4-5 KB files) is in F#. This small code base contains a few non-trivial pieces, including handling file uploads via [formlets](http://websharper.com/docs/WorkingWithFormlets.aspx), full login story with user registration via confirmation emails and authentication, and some database logic for filtering by tags and submitters. In general, WebSharper sitelet pages are very fast, often much faster than equivalent ASP.NET content.

### How do you use it?

Using [ApiStack.net](http://apistack.net/) is as easy as it gets. First sign up by giving your email address and a couple other things. Then wait to receive your confirmation email, click on the activation link in it, and you will be taken to your home page. Here, you can go to Upload - and send in your project assemblies. Below is an example for the F# Core library.

<img src="/assets/ApiStack-form.png">

Once uploaded, you will get the Permalink to your documentation and it will appear in the listing on the home page as well. Clicking on it will render your documentation in its full glory:

<img src="/assets/ApiStack-FSharpCore.png">

While still in beta, [ApiStack.net](http://apistack.net/) can already be very useful. For instance, we are hosting a lot of [WebSharper](http://websharper.com/) documentation on the site already. Even though some of the more advanced features are still in development, we hope you will find the site useful for your projects - and we definitely want to hear your opinion so don't hesitate to get in touch.
