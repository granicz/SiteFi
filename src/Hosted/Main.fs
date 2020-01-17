namespace Website

open System
open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /blog">] Article of slug:string
    | [<EndPoint "GET /category">] Category of string
    | [<EndPoint "GET /refresh">] Refresh

module Markdown =
    open Markdig

    let pipeline =
        MarkdownPipelineBuilder()
            .UsePipeTables()
            .UseGridTables()
            .UseListExtras()
            .UseEmphasisExtras()
            .UseGenericAttributes()
            .UseAutoLinks()
            .UseTaskLists()
            .UseMediaLinks()
            .UseCustomContainers()
            .UseMathematics()
            .UseEmojiAndSmiley()
            .UseYamlFrontMatter()
            .UseAdvancedExtensions()
            .Build()

    let Convert content = Markdown.ToHtml(content, pipeline)

module Yaml =
    open System.Text.RegularExpressions
    open YamlDotNet.Serialization

    let SplitIntoHeaderAndContent (source: string) =
        let delimRE = Regex("^---\\w*\r?$", RegexOptions.Compiled ||| RegexOptions.Multiline)
        let searchFrom = if source.StartsWith("---") then 3 else 0
        let m = delimRE.Match(source, searchFrom)
        if m.Success then
            source.[searchFrom..m.Index-1], source.[m.Index + m.Length..]
        else
            "", source

    let OfYaml<'T> (yaml: string) =
        let deserializer = (new DeserializerBuilder()).Build()
        if String.IsNullOrWhiteSpace yaml then
            deserializer.Deserialize<'T>("{}")
        else
            let yaml = deserializer.Deserialize<'T>(yaml)
            eprintfn "DEBUG/YAML=%A" yaml
            yaml

// Helpers around blog URLs.
// These need to match the endpoints type of the main sitelet.
module Urls =
    let CATEGORY (cat: string) = sprintf "/category/%s" cat
    let POST_URL (slug: string) = "/blog/" + slug + ".html"

module Helpers =
    open System.IO
    open System.Text.RegularExpressions

    let NULL_TO_EMPTY (s: string) = match s with null -> "" | t -> t

    // Return (fullpath, filename-without-extension, (year, month, day), slug, extension)
    let (|ArticleFile|_|) (fullpath: string) =
        let filename = Path.GetFileName(fullpath)
        let filenameWithoutExt = Path.GetFileNameWithoutExtension(fullpath)
        let r = new Regex("([0-9]+)-([0-9]+)-([0-9]+)-(.+)\.(md)")
        if r.IsMatch(filename) then
            let a = r.Match(filename)
            let V (i: int) = a.Groups.[i].Value
            let I = Int32.Parse
            Some (fullpath, filenameWithoutExt, (I (V 1), I (V 2), I (V 3)), V 4, V 5)
        else
            None

module Site =
    open System.IO
    open WebSharper.UI.Html

    type MainTemplate = Templating.Template<"..\\hosted\\index.html", serverLoad=Templating.ServerLoad.WhenChanged>

    type [<CLIMutable>] RawArticle =
        {
            title: string
            subtitle: string
            ``abstract``: string
            url: string
            content: string
            date: string
            categories: string
        }

    type Article =
        {
            Title: string
            Subtitle: string
            Abstract: string
            Url: string
            Content: string
            Date: string
            Categories: string list
        }

    let Articles() : Map<string, Article> =
        let folder = Path.Combine (__SOURCE_DIRECTORY__, @"..\hosted\posts")
        if Directory.Exists folder then
            Directory.EnumerateFiles(folder, "*.md", SearchOption.AllDirectories)
            |> Seq.toList
            |> List.choose (Helpers.(|ArticleFile|_|))
            |> List.fold (fun map (fullpath, fname, (year, month, day), slug, extension) ->
                eprintfn "Found file: %s" fname
                let header, content =
                    File.ReadAllText fullpath
                    |> Yaml.SplitIntoHeaderAndContent
                let article = Yaml.OfYaml<RawArticle> header
                let title = Helpers.NULL_TO_EMPTY article.title
                let subtitle = Helpers.NULL_TO_EMPTY article.subtitle
                let ``abstract`` = Helpers.NULL_TO_EMPTY article.``abstract``
                let url = Urls.POST_URL fname
                let content = Markdown.Convert content
                let date = String.Format("{0:D4}{1:D2}{2:D2}", year, month, day)
                let categories =
                    Helpers.NULL_TO_EMPTY article.categories
                let categories =
                    if not <| String.IsNullOrEmpty categories then
                        categories.Split [| ',' |]
                        // Note: categories are case-sensitive.
                        |> Array.map (fun cat -> cat.Trim())
                        |> Array.filter (not << String.IsNullOrEmpty)
                        |> Set.ofArray
                        |> Set.toList
                    else
                        []
                Map.add fname
                    {
                        Title = title
                        Subtitle = subtitle
                        Abstract = ``abstract``
                        Url = url
                        Content = content
                        Date = date
                        Categories = categories
                    } map
            ) Map.empty
        else
            eprintfn "warning: the posts folder (%s) does not exist." folder
            Map.empty

    let Menu (articles: Map<string, Article>) =
        let latest =
            articles
            |> Map.toSeq
            |> Seq.truncate 5
            |> Map.ofSeq
        [
            "Home", "/", Map.empty
            "Latest", "#", latest
        ]

    let private head() =
        __SOURCE_DIRECTORY__ + "/../Hosted/js/Client.head.html"
        |> File.ReadAllText
        |> Doc.Verbatim

    let Page (title: option<string>) hasBanner articles (body: Doc) =
        let head = head()
        MainTemplate()
#if !DEBUG
            .ReleaseMin(".min")
#endif
            .NavbarOverlay(if hasBanner then "overlay-bar" else "")
            .Head(head)
            .Title(
                match title with
                | None -> ""
                | Some t -> t + " | "
            )
            .TopMenu(Menu articles |> List.map (function
                | text, url, map when Map.isEmpty map ->
                    MainTemplate.TopMenuItem()
                        .Text(text)
                        .Url(url)
                        .Doc()
                | text, _, children ->
                    let items =
                        children
                        |> Map.toList
                        |> List.sortByDescending (fun (key, item) -> item.Date)
                        |> List.map (fun (key, item) ->
                            MainTemplate.TopMenuDropdownItem()
                                .Text(item.Title)
                                .Url(item.Url)
                                .Doc())
                    MainTemplate.TopMenuItemWithDropdown()
                        .Text(text)
                        .DropdownItems(items)
                        .Doc()
            ))
            .DrawerMenu(Menu articles |> List.map (fun (text, url, children) ->
                MainTemplate.DrawerMenuItem()
                    .Text(text)
                    .Url(url)
                    .Children(
                        match url with
                        | "/blog" ->
                            ul []
                                (children
                                |> Map.toList
                                |> List.sortByDescending (fun (_, item) -> item.Date)
                                |> List.map (fun (_, item) ->
                                    MainTemplate.DrawerMenuItem()
                                        .Text(item.Title)
                                        .Url(item.Url)
                                        .Doc()
                                ))
                        | _ -> Doc.Empty
                    )
                    .Doc()
            ))
            .Body(body)
            .Doc()
        |> Content.Page

    let BlogSidebar (articles: Map<string, Article>) (article: Article) =
        MainTemplate.Sidebar()
            .Categories(
                // Render the categories widget iff there are categories
                if article.Categories.IsEmpty then
                    Doc.Empty
                else
                    MainTemplate.Categories()
                        .Categories(
                            article.Categories
                            |> List.map (fun category ->
                                MainTemplate.Category()
                                    .Name(category)
                                    .Url(Urls.CATEGORY category)
                                    .Doc()
                            )
                        )
                        .Doc()
            )
            // There is always at least one blog post, so we render this
            // section no matter what.
            .ArticleItems(
                articles
                |> Map.toList
                |> List.sortByDescending (fun (_, item) -> item.Date)
                |> List.map (fun (_, item) ->
                    MainTemplate.ArticleItem()
                        .Title(item.Title)
                        .Url(item.Url)
                        .ExtraCSS(if article.Url = item.Url then "is-active" else "")
                        .Doc()
                )
            )
            .Doc()

    let PLAIN html =
        div [Attr.Create "ws-preserve" ""] [Doc.Verbatim html]

    let ArticlePage articles (article: Article) =
        MainTemplate.ArticlePage()
            // Main content panel
            .Article(
                MainTemplate.Article()
                    .Title(article.Title)
                    .Subtitle(Doc.Verbatim article.Subtitle)
                    .Content(PLAIN article.Content)
                    .Doc()
            )
            // Sidebar
            .Sidebar(BlogSidebar articles article)
            .Doc()
        |> Page (Some article.Title) false articles

    let articles : Map<string, Article> ref = ref Map.empty

    let Main articles =
        let ARTICLES_BY f articles =
            Map.filter f articles
        let ARTICLES (articles: Map<_, Article>) =
            [ for (_, article) in Map.toList articles ->
                MainTemplate.ArticleCard()
                    .Author("My name")
                    .Title(article.Title)
                    .Abstract(article.Abstract)
                    .Url(article.Url)
                    .Date(article.Date)
                    .ArticleCategories(
                        if article.Categories.IsEmpty then
                            Doc.Empty
                        else
                            article.Categories
                            |> List.map (fun category ->
                                MainTemplate.ArticleCategory()
                                    .Title(category)
                                    .Url(Urls.CATEGORY category)
                                    .Doc()
                            )
                            |> Doc.Concat
                    )
                    .Doc()
            ]                        
        Application.MultiPage (fun (ctx: Context<_>) -> function
            | Home ->
                MainTemplate.HomeBody()
                    .Banner(
                        MainTemplate.HomeBanner().Doc()
                    )
                    .ArticleList(
                        ARTICLES !articles
                    )
                    .Doc()
                |> Page None false !articles
            | Article p ->
                let page =
                    if p.ToLower().EndsWith(".html") then
                        p.Substring(0, p.Length-5)
                    else
                        p
                if (!articles).ContainsKey page then
                    ArticlePage !articles (!articles).[page]
                else
                    Map.toList !articles
                    |> List.map fst
                    |> sprintf "Trying to find page \"%s\" (with key=\"%s\"), but it's not in %A" p page
                    |> Content.Text
            | Category cat ->
                MainTemplate.HomeBody()
                    .Banner(
                        MainTemplate.CategoryBanner()
                            .Category(cat)
                            .Doc()
                    )
                    .ArticleList(
                        ARTICLES_BY (fun _ article ->
                            List.contains cat article.Categories
                        ) !articles
                        |> ARTICLES
                    )
                    .Doc()
                |> Page None false !articles
            | Refresh ->
                // Reload the article cache
                articles := Articles()
                Content.Text "Articles reloaded."
        )

[<Sealed>]
type Website() =
    let articles = ref <| Site.Articles()

    interface IWebsite<EndPoint> with
        member this.Sitelet = Site.Main articles
        member this.Actions = [
            Home
            for (slug, _) in Map.toList !articles do
                Article slug
            for category in
                !articles
                |> Map.toList
                |> List.map snd
                |> List.collect (fun article -> article.Categories)
                |> Set.ofList
                |> Set.toList
                do
                    Category category
        ]

[<assembly: Website(typeof<Website>)>]
do ()
