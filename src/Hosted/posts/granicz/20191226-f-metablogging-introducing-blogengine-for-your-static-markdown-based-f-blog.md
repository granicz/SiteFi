---
title: "F# metablogging: introducing BlogEngine for your static markdown-based F# blog"
categories: "blogging,f#,websharper"
abstract: "In this article, I want to show you another way of keeping your SSG in F#: using WebSharper. Armed with the full power of a .NET web framework and its templating features (see the HTML templates section), this has numerous advantages, most importantly, among others, that you can embed dynamic features written in F# or C# to give you a truly impressive and dynamic blog, despite being statically generated."
identity: "5862,87610"
---
(This post is part of [F# Advent 2019](https://sergeytihon.com/2019/11/05/f-advent-calendar-in-english-2019/) - a huge thanks to Sergey Tihon for organizing this initiative for many years now! Happy Holidays!)

Back in 2015, I was super happy to [announce FsBlogger](https://twitter.com/granicz/status/602928811320152064), an F#+markdown blogging platform.

[![](https://i.imgur.com/VVYjhlZl.png)](https://i.imgur.com/VVYjhlZ.png)

To this day, this platform runs our [IntelliFactory](http://intellifactory.com/blogs/1), [WebSharper](https://forums.websharper.com/blogs), and [CloudSharper](http://cloudsharper.com/blog) blogs, and is directly tied into [FPish](https://fpish.net) and the F# RSS feed on the [fsharp.org](https://fsharp.org) website. However, while a fully managed blogging platform such as FsBlogger.com or Blogger.com frees you from having to deal with storing, running, and managing your blog articles, it can also leave you feeling out of control and dependent on these platform's longevity. One complaint in particular is that these managed options don't have transparent change tracking and self-hosting is not available. To address this, and to a fuller extent managing full static web sites as well, a whole host of static site generators (SSGs) popped up on the horizon. [Jekyll](https://jekyllrb.com/), [Next.js](https://nextjs.org/), [Hugo](https://gohugo.io/) are prime examples.

### With and beyond SSGs in F#

Instead of looking at various SSGs, I am going to pick Jekyll as an example. A quick googling for "Jekyll and F#" reveals a staggering number of attempts to arrive at an elegant, static blog generator for F# developers. This stems from the fact that while Jekyll is pretty straightforward to use and easy to learn, we F# developers ~~like to do things our way~~ have extra needs. Here are some (totally random) examples of the ingenuity produced as a result.

 * One early approach, ca. 2013, was Ashley Towns's [tilde](https://github.com/aktowns/tilde) library, that used Razor and F# Formatting. Some of this made it into MattDrivenDev et. al's [FsBlog](https://github.com/fsprojects/FsBlog).

 * I also thoroughly enjoyed Mathias Brandewinder's [Get Stuff Done (GDS) article on porting his old blog to Jekyll](http://brandewinder.com/2016/08/28/gsd-with-fsharp/). Here, Mathias walks through some plain F# functions that he used to convert his past HTML articles to markdown files that can feed directly into Jekyll.

 * Colin Bull, another long-time F# veteran describes [his move to Jekyll](http://www.colinbull.net/2014/11/04/Blogging-with-FSharp/) and how he used F# Formatting to enable using `.fsx` blog files as well. A definite read for those who need support for blogging with F# literate programming.

 * The [fsharp.org](http://fsharp.org) website is also based on the Jekyll layout but uses an [.fsx script](https://github.com/fsharp/fsfoundation/commit/755d45d9c233374dbb44ba1fdf87214ef8b34520) that does away with the Jekyll runtime and instead uses a single F# script to process source `.md` files into Liquid templates to generate the static `.html` output. As Tomas called it in his above commit, this primitive approach does make certain cuts, for instance it only handles simple `{{ ... }}` placeholders in the Liquid source templates, although using DotLiquid or something similar would be an easy enhancement.

 * Krzysztof Cieslak's [Fornax](https://github.com/LambdaFactory/Fornax) does away with Jekyll all together, and instead gives you an F# DSL to define your templates and lets you instantiate them from F# values as models. It also uses markdown files as input.

 * Jeremie Chassaing [implements a full blog engine](https://thinkbeforecoding.com/post/2018/12/07/full-fsharp-blog) by using F# Formatting to render markdown to HTML and Fable HTML combinators to output the resulting content into simple layout pages. In subsequent articles ([Part 2](https://thinkbeforecoding.com/post/2018/12/09/full-fsharp-blog-2) and [Part 3](https://thinkbeforecoding.com/post/2019/12/13/full-fsharp-blog-3)) he sets up Azure Functions to push and retrieve blog articles to Azure blob storage and shows how to set up free SSL certs to secure the resulting blog. 

### Combining SSGs and F#

It's hard to argue that you should switch your entire static site generation to F# (such as to Fornax or Fable in some of the above approaches) - especially if your tool doesn't support templating. A reasonable choice would be to keep full compatibility with a given SSG such as Jekyll in terms of input and templating, but remove the need to install a full Ruby/etc. development environment just to run these tools that generate the output markup. So Tomas nailed it with fsharp.org: if we already have an F# development environment installed, we should use that.

In this article, I want to show you another way of keeping your SSG in F#: using [WebSharper](https://websharper.com). Armed with the full power of a .NET web framework and [its templating features](http://developers.websharper.com/docs/v4.x/fs/ui) (see the HTML templates section), this has numerous advantages, most importantly, among others, that you can embed dynamic features written in F# or C# to give you a truly impressive and dynamic blog, despite being statically generated.

So, how we do start?

### Getting your templates under control

A long while back I started working on a WebSharper+Jekyll implementation that enabled dropping in a full Jekyll theme on top of a set of markdown article files to generate a static blog. However, I quickly got annoyed with the mess these themes came with: templates composed from smaller layout files that collectively were nearly impossible to author/enhance, configuration allowed HTML fragments without checking consistency or well-formedness, etc. So although there are a lot of Jekyll themes out there ready to be used, considering the very real possibility that most users will end up wanting customizing these, this was a no go for me at the end. Instead, I decided to use WebSharper's HTML templating type provider and plain HTML files decorated with a few basic placeholders, and filling these site templates with content in an all-F# solution.

### Your blog as a WebSharper (offline) sitelet

A [sitelet](http://developers.websharper.com/docs/v4.x/fs/sitelets) is a WebSharper server-side abstraction that describes how incoming requests are routed to the content we return. A typical sitelet will use a discriminated union (DU) endpoint type and provide a mapping from those endpoints to responses with a simple pattern match (most often using `Application.MultiPage` as a helper).

So you can model your blog's endpoints as something like:

```fsharp
open WebSharper
open WebSharper.Sitelets

type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /blog">] Article of slug:string

...
module Site =
    let ArticlePage articles article =
        ...

    let Main articles =
        Application.MultiPage (fun (ctx: Context<_>) -> function
            | Home ->
                Content.Text "This will be the home page"
            | Article slug ->
                ArticlePage articles articles.[slug]
        )
```
Using DUs to represent the pages in our web application proves to be a handy tool when it comes to static site generation, and WebSharper provides a basic `IWebsite` interface for marking web apps for this sort of processing. But more on this later.

Just as simple as the above code looks, we have a home page and a set of blog pages identified by an article "slug." Rendering the actual blog page content is handled by `Site.ArticlePage`, a function that will compute a `Async<Content<'T>>`, where `Content` is the built-in type representing content generated by a sitelet.

Now, `Site.ArticlePage` will be different depending on your own template, this is a function every blog author has to write initially. Structurally, it will be looking something like:

```fsharp
open WebSharper.UI

type BlogTemplate = Templating.Template<"index.html">

let ArticlePage articles (article: Article) =
    BlogTemplate.Article()
        .Title(article.Title)
        .Subtitle(article.Subtitle)
        .Content(Doc.Verbatim article.ContentHtml)
        .Doc()
```

This code assumes you have an `index.html` file with an inner template called `Article` in it, which in turn contains multiple placeholders (`Title`, `Subtitle`, `Content`, etc.)

It also uses a type (`Article`) to store data about a given blog article. And because it's often the case that a given article needs references to other articles, we just pass the entire collection (as an `articles: Map<string, Article>` argument) as well.

### Offline vs online/regular sitelets

Now, back to `IWebsite` I mentioned earlier. A sitelet is most typically served in a live web app by exposing it as a module-bound value with the `[<Website>]` attribute, something like:

```fsharp
module App =
    let articles = Site.Articles () // compute all articles

    [<Website>]
    let App = Site.Main articles
```
Here, you need to use the WebSharper [client-server project template](http://developers.websharper.com/docs/v4.x/fs/templates) (available for `dotnet` CLI use and as a Visual Studio extension installer from the WebSharper [download page](https://websharper.com/downloads)) to get the correct build configuration to compile and run your sitelet-based app, and you should not need anything else to get a working app.

However, for your static site you don't want to run a server, all you want are the HTML files for each of your pages/articles. To set this up, you need to add a few extra lines to your app:

```fsharp
[<Sealed>]
type Website() =
    let articles = Site.Articles ()

    interface IWebsite<EndPoint> with
        member this.Sitelet = Site.Main articles
        member this.Actions = [
            Home
            for (slug, _) in Map.toList articles do
                Article slug
        ]

[<assembly: Website(typeof<Website>)>]
do ()
```
Note the use of the `[<Website>]` attribute once again, this time on the entire assembly, with a type argument. This type argument needs to implement `IWebsite<EndPoint>`, which requires two members: the main sitelet and a list of actions (aka. endpoints) for which you want to generate static content. 

(Now, you may have used other F# web tech before and if so, you likely remember constructing paths via various route combinators or paths that resembled `printf` format strings. Hopefully, here you can appreciate using DUs instead to represent web pages to process.)

This use of the `[<Website>]`  attribute marks what we call an **offline sitelet** - a sitelet statically generated on the server into an HTML application. The only additional change you need for this to work is to ensure that the `project` type in your `wsconfig.json` is set to `"html"`, naturally, or simply use the WebSharper HTML application project template to start from.

### BlogEngine:  Getting your new shiny F# blog and adding your articles

[![](https://i.imgur.com/RVJs5IXl.png)](https://i.imgur.com/RVJs5IX.png)

If you prefer to jump into a working setup, you can just check out the [BlogEngine repo](https://github.com/granicz/BlogEngine) I created to jumpstart the process, and follow the instructions in `README.md` to build your new F# blog:

 1) Run `install.ps` to install the npm packages needed, this is only [Bulma](http://bulma.io) currently.
 2) Run `dotnet build` to compile and generate your dummy blog (remember, you don't have any articles yet, but don't worry I copied a few of mine to help you get started.)
 3) Run `start.cmd` to power up a local webserver and serve your blog articles.
 
All in all, it only takes these three commands to get a blog up and running, and all the code behind it is under 300 lines of F#.

At this point, you can **start placing your markdown files** under `src\Website\posts` using the `YYYY-MM-DD-TitleWithNoSpaces.md` file format. This will ensure that your articles are displayed in the expected chronological order. Since BlogEngine is geared towards building developer/F# blogs, you can write F# code inside "triple-backtick fsharp" code blocks, these will be turned into properly syntax-highlighted code blocks at the end.

You may also notice that BlogEngine has a list of all blog articles on the home page. This uses a custom inner template (`HomeBody`) and an `ArticleList` placeholder in it, and is implemented in the main sitelet function as:

```fsharp
    let Main articles =
        Application.MultiPage (fun (ctx: Context<_>) -> function
            | Home ->
                MainTemplate.HomeBody()
                    .ArticleList(
                        Doc.Concat [
                            for (_, article) in Map.toList articles ->
                                MainTemplate.ArticleCard()
                                    .Author("My name")
                                    .Title(article.title)
                                    .Url(article.url)
                                    .Date(article.date)
                                    .Doc()
                        ]                        
                    )
                    .Doc()
                |> Page None false articles
            | Article slug -> ...
```

#### Getting the HTML content of markdown source files

Recall how we instantiated the `.Content` placeholder of an article by simply passing the HTML representation verbatim. Here is the actual code from the BlogEngine repo:

```fsharp
    let ArticlePage articles (article: Article) =
        MainTemplate.Article()
            .Title(article.title)
            .Subtitle(Doc.Verbatim article.subtitle)
            .Sidebar(BlogSidebar articles)
            .Content(PLAIN article.content)
            .Doc()
        |> Page (Some article.title) false articles
```

Here, `PLAIN` is just a helper for creating verbatim HTML output, the essence lies with `article.content` - which at this point is an HTML string parsed and generated for each article's markdown source file. I will spare you the details and instead refer you to the ~150 LOC (about 2/3 of the entire code base) that deals with that conversion. Here is a rough summary of what happens underneath:

1) I created the `Article` type to model the data I want to know about each article. It looks like this:

	```fsharp
    type [<CLIMutable>] Article =
        {
            title: string
            subtitle: string
            url: string
            content: string
            date: string
        }
	```

2) A couple important fields into this record (`title` and `subtitle`) will come from a YAML header of each markdown input file, which we strip and parse as YAML using [YamlDotNet](https://github.com/aaubry/YamlDotNet), a .NET library/NuGet that simplifies YAML handling. Note the `[<CLIMutable>]` attribute on `Article` above, this is to ensure that YamlDotNet can create an `Article` instance even when certain fields are missing.

3) Some of these fields: `url`, `content`, and `date` will be computed instead of passed in the YAML header. Part of this conversion is taking the rest of the markdown file (everything but the YAML header) and converting it to HTML using [Markdig](https://github.com/lunet-io/markdig), a .NET library/NuGet markdown processor. `date` will come from the input file name.

4) When we define the main offline sitelet for our blog, we run the above process for the markdown files in the `src\Website\posts` folder, and feed the result as a `Map<string, Article>` collection to the rest of our sitelet pipeline.

5) We then list/enumerate our articles in our offline sitelet definition (in the `Actions` member) using this article collection. Generating HTML files automatically completes the process.

Note, that each article HTML page is generated at compilation time. This means that if you change or add a new article, you need to recompile the `src\Website` project.

### Modifying the existing blog template or adding your own

Recall I made a statement about Jekyll and its templates being a PITA to edit/customize, because you end up writing unchecked HTML code in configuration files or in nested templates, both of which are impossible to preview until you regenerate your site. If you study the `index.html` we use in the BlogEngine repo, you will see that we did away with Jekyll-style configuration files and incorporated all templates (nested or not) into this one master file. Clearly, if you open this file in a browser, you won't find it easy to "edit and see the results," either. But you can do a few things to alleviate the situation:

 0) Recompile the `src\Website` project on every template or SCSS change to regenerate everything properly. You can also set up a file watcher for these files to trigger recompilation automatically. This sucks, but works, and you are in no worse situation than with Jekyll. Keep reading for more options.
 1) You can start a webserver relative to `src\Website` (just create a new script from `start.cmd` in the root folder to point here) - this way various  file references light up properly, but those WebSharper-specific templates still get in the way.

 2) You can apply `display:none` on those templates using a few lines of CSS. You can then edit your HTML and see your changes immediately. This however will make it impossible to work on (inner) templates.

 3) You can keep everything as is, but switch your `src/Website` project to `"site"`in `wsconfig.json` - essentially, making it a hosted client-server app. Follow the guidance in the "Offline vs online/regular sitelets" section above to mark your sitelet appropriately for this to work. Once you set this up, I also recommend setting up automatic template reloading when the template file changes by modifying the TP call as follows:

	```fsharp
    type MainTemplate = Templating.Template<"index.html", serverLoad=Templating.ServerLoad.WhenChanged>
	```
	With these changes, you can fire up your blog, make changes to `index.html` (including changes to your inner templates) and see those changes immediately when you hit Ctrl+F5 on a rendered page (clearly, you need to run the webserver to serve these files.) This should give you a pretty comfortable workflow to author new templates or enhance the built-in one.

### Adding client-side functionality to your blog pages

As I pointed out earlier, one of the most significant advantages of using the approach I outlined above is being able to write client-side functionality in F# (or C#) instead of JavaScript/TypeScript.

In fact, the reference BlogEngine repo already comes with two such client-side enhancements built-in: syntax-highlighting F# code in your blog articles and setting up the drawer menu interaction when using mobile devices. These are placed in the adjacent `src\Client` project, which is a WebSharper "bundle" project (project type `"bundle"` in `wsconfig.json`), ie. a project whose JS+CSS output is bundled into a single JS+CSS file. This is so that we can include these easily in our master template file and benefit immediately.

For instance, here is the code for setting up the syntax highlighter (which uses [Highlight.js](https://highlightjs.org/) as a WebSharper extension - a "proxy" that lets us talk to this JS library in F#/C#):

```fsharp
module Highlight =
    open WebSharper.HighlightJS

    [<Require(typeof<Resources.Languages.Fsharp>)>]
    [<Require(typeof<Resources.Styles.Vs>)>]
    let Run() =
        JS.Document.QuerySelectorAll("code[class^=language-]").ForEach(
            (fun (node, _, _, _) -> Hljs.HighlightBlock(node)),
            JS.Undefined
        )
```

This showcases WebSharper's important value proposition for resource handling: when `Highlight.Run()` is called in any dynamic functionality embedded into a sitelet page (thus in our blog article pages as well) it will automatically bring Highlight.js's  F# language mode and VS styles with it (and the inner call to `Hljs.HighlightBlock` will bring the main Highlight.js references/resources as well, in the right/expected order.)

For this to work, the bundle project from `src\Client` needs to have a "entrypoint", a function that will call and light up all desired functionality when the bundle script is loaded into an HTML page. WebSharper conveniently provides the `[<SPAEntryPoint>]` attribute, and we can tag our global entrypoint function with it as follows:

```fsharp
[<SPAEntryPoint>]
let Main() =
    Bulma.HookDrawer()
    Highlight.Run()

[<assembly:JavaScript>]
do ()
```

In a similar vein, you could also create your own custom client-side functionality for *selected* blog articles, such as visualizations/charts/etc for a given article. This, however, doesn't yet blend into the simplified "process all input files in the same way" we presented. One way to deal with this would be via a custom YAML header field and differentiate based on its value when rendering each article page. I will leave this suprisingly straightforward solution to another blog article.

### Where to go next?

Apart from the custom client-side content above (which is more of a blog-specific path, as opposed to a general wireframe solution I intend to give with BlogEngine), I plan to add support for multi-user articles and RSS feeds, and follow up with another article about setting up automatic deployment to GitHub Pages. We will most definitely use these new features in our own various company-wide blogs, and migrate away from the current self-hosted solutions.

Anyhow - lots to explore, I hope you found this article useful and will give BlogEngine a try. As always, feel free to get in touch or file tickets or PRs, I'd love to get more people involved. I also want to thank my [IntelliFactory](https://intellifactory.com) colleagues for their input/ideas and work that led to BlogEngine, most notably [Loic Denuziere](https://github.com/Tarmil), who has now adapted some of the earlier code for his own blog and for the [Bolero website](http://fsbolero.io). 

Happy coding and Happy Holidays!
