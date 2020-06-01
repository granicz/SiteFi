---
title: "WebSharper 2.4 Q2 available"
categories: "release,f#,websharper"
abstract: "A few days ago, I blogged about WebSharper 2.4 Q2 Beta, announcing a handful of important changes to WebSharper.  Today, we are happy to announce the general availability of WebSharper 2.4 Q2, available from the WebSharper downloads page."
identity: "2525,75345"
---
A few days ago, [I blogged about WebSharper 2.4 Q2 Beta](//intellifactory.com/user/granicz/20120416-websharper-2-4-q2-beta-out), announcing a handful of important changes to WebSharper.  Today, we are happy to announce the general availability of **WebSharper 2.4 Q2**, available from the [WebSharper downloads page](http://websharper.com/downloads).

### Better HTML templates
All sitelets projects are now equipped with a new HTML template system. The major improvement over the previous system is the ability to edit the HTML and see changes take effect without recompiling the WebSharper project. Templates also make use of HTML5 `data-*` attributes in a way that makes it possible to develop, test and validate template files as HTML5 documents, for example:

```xml
<!DOCTYPE html>
<html>
  <head>
    <title>${title}</title>
  </head>
  <body>
    <div id="main" data-hole="main">
      Sample content here that will be replaced during expansion.
    </div>
  </body>
</html>
```

The former template system is now deprecated.

### Revamped mobile application support

We have reviewed and improved our experimental support for packaging WebSharper code into native Android and Windows Phone applications. One big change is that Android and Windows Phone project templates are now separate. You can still reuse code across these applications by putting it in a WebSharper library and referencing from both projects. Separating the project templates has allowed to specialize them. For example, you can now use Eclipse/ADT to customize and debug the generated Android application. On the Windows Phone platform, you can likewise now attach the Visual Studio debugger or customize the XAML. Please refer to the latest documentation for full details.

In addition, we are now releasing experimental support for writing Bluetooth clients and servers on the Android platform. Note that the experimental status means the API is unstable.

### `extra.files`

As a convenience for build automation, we now include a special file called `extra.files` with every mobile and HTML project. This file allows to easily copy extra files from the project to be deployed under the generated HTML application root. See [#31](https://bitbucket.org/IntelliFactory/websharper/issue/31/extra-files-in-mobile-applications) for details.

### Bug fixes

The bug fixes in this release primarily concern the fixes necessary to run WebSharper on the latest Microsoft platform tools, including Windows 8, .NET 4.5, F# 3.0 and VisualStudio 2011, in various combinations.

### Summary

[Issues resolved in 2.4.62](https://bitbucket.org/IntelliFactory/websharper/issues?status=resolved&version=2.4.38)
