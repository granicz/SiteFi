---
title: "Running the jQuery Mobile WebSharper sample"
categories: "jquerymobile,f#,websharper"
abstract: "The jQuery Mobile sample on the WebSharper site has been out for quite some time now, but it fails to mention an important detail on how to run it standalone. Indeed, copying it straight into a new WebSharper ASP.NET project produces an empty page (which is not empty, only jQuery Mobile fails to kick in to decorate it into a full application), which can be a discouraging start for anyone looking into using jQuery Mobile for building mobile applications with F# and WebSharper. [...]"
identity: "2982,76078"
---
The [jQuery Mobile sample](http://websharper.com/samples/JQueryMobile) on the [WebSharper](http://websharper.com) site has been out for quite some time now, but it fails to mention an important detail on how to run it standalone. Indeed, copying it straight into a new WebSharper ASP.NET project produces an empty page (which is not empty, only jQuery Mobile fails to kick in to decorate it into a full application), which can be a discouraging start for anyone looking into using jQuery Mobile for building mobile applications with F# and WebSharper.

What's not shown in that sample is that the sample runner sitelet adds a container `div` and a "dummy" jQuery Mobile page for the dynamic "paging" mechanism to work. So when you copy-and-paste the sample code into a new application, you need to add that same wrapper yourself. In case you are starting from a WebSharper ASP.NET template, you need to modify the ASPX file as follows:

```html
...
<body>
     <div>
            <div data-role="page" id="dummy"></div>
            <ws:MyControl ID="MyControl1" runat="server"/>
     </div>
...
```

Hope this helps.