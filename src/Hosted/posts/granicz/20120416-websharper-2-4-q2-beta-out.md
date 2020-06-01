---
title: "WebSharper 2.4 Q2 Beta out"
categories: "release,f#,websharper"
abstract: "We have been working hard to bring you WebSharper 2.4 Q2 - and today we are happy to announce the availability of the the first beta (2.4.46)... [more]"
identity: "2509,75303"
---
We have been working hard to bring you WebSharper 2.4 Q2 - and today we are happy to announce the availability of the [the first beta (2.4.46)](http://intellifactorylabs.com/beta/WebSharperInstaller-2.4.46.exe).

We will be doing a detailed change log and announcement in the coming days, so stay tuned.  One of the most significant changes in this release is **dynamic templating** - a feature that enables authoring template-based web applications whose template(s) can be changed runtime without requiring re-compilation.

Dynamic templates are not only flexible but also come with a couple significant enhancements over the static templating available in earlier releases.  For instance, you can tune how and when your templates get served and instantiated, whether placeholders act as containers or direct content holes, and what type of content can go into them.

For instance, here is a dynamic template in F#:

```fsharp
    let TemplateLoadFrequency = Content.Template.PerRequest

    type Page =
        {
            Title : string
            Head : list<Content.HtmlElement>
            HeadStyles : list<Content.HtmlElement>
            Slider : list<Content.HtmlElement>
            Breadcrumbs : list<Content.HtmlElement>
            Main : list<Content.HtmlElement>
        }

    let MainTemplate =
        let path = HttpContext.Current.Server.MapPath("~/static/my-template.html")
        Content.Template<Page>(path, TemplateLoadFrequency)
            .With("title", fun x -> x.Title)
            .With("head", fun x -> x.Head)
            .With("headStyles", fun x -> x.HeadStyles)
            .With("slider", fun x -> x.Slider)
            .With("breadcrumbs", fun x -> x.Breadcrumbs)
            .With("main", fun x -> x.Main)
```

You can use this template to construct sitelet pages as follows:

```fsharp
    let Site =
        Sitelet.Content "/" Action.Home 
            <| (Content.WithTemplate MainTemplate <| fun ctx ->
                {
                    Page.Title = "My title"
                    Page.Head = []
                    Page.HeadStyles = []
                    Page.Slider = []
                    Page.Breadcrumbs = MyBreadcrumbsFor ctx action
                    Page.Main =
                        [
                            H1 [Text "This is where your main content goes"]
                        ]
                })
```

The beta release also ships support for .NET 4.5, and should work if you have Windows 8 and Visual Studio 2011 Beta.

If you find any bugs, please get in touch at [the WebSharper contact page](http://websharper.com/contact), post them in the public [BitBucket issue tracker](https://bitbucket.org/IntelliFactory/websharper/issues), or simply post them here.

Have fun!
