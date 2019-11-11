namespace Website

open System
open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "GET /">] Home
    | [<EndPoint "GET /blog">] BlogPage of string

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
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
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

module Helpers =
    open System.IO
    open System.Text.RegularExpressions

    let NULL_TO_EMPTY (s: string) = match s with null -> "" | t -> t

    // Return (fullpath, filename, (year, month, day), slug, extension)
    let (|ArticleFile|_|) (fullpath: string) =
        let s = Path.GetFileName(fullpath)
        let r = new Regex("([0-9]+)-([0-9]+)-([0-9]+)-(.+)\.(md|html)")
        if r.IsMatch(s) then
            let a = r.Match(s)
            let V (i: int) = a.Groups.[i].Value
            let I = Int32.Parse
            Some (fullpath, V 0, (I (V 1), I (V 2), I (V 3)), V 4, V 5)
        else
            None

module Site =
    open System.IO
    open WebSharper.UI.Html

    type MainTemplate = Templating.Template<"index.html">

    type [<CLIMutable>] BlogPage =
        {
            title: string
            subtitle: string
            url: string
            content: string
            date: string
        }

    let BlogPages () : Map<string, BlogPage> =
        let folder = Path.Combine (__SOURCE_DIRECTORY__, "posts")
        if Directory.Exists folder then
            Directory.EnumerateFiles(folder, "*.md", SearchOption.AllDirectories)
            |> Seq.toList
            |> List.choose (Helpers.(|ArticleFile|_|))
            |> List.fold (fun map (fullpath, fname, (year, month, day), slug, extension) ->
                eprintfn "Found file: %s" fname
                let header, content =
                    File.ReadAllText fullpath
                    |> Yaml.SplitIntoHeaderAndContent
                let blog = Yaml.OfYaml<BlogPage> header
                let title = Helpers.NULL_TO_EMPTY blog.title
                let url = "/blog/" + fname + ".html"
                let subtitle = Helpers.NULL_TO_EMPTY blog.subtitle
                let content = Markdown.Convert content
                let date = String.Format("{0:D4}{1:D2}{2:D2}", year, month, day)
                Map.add fname
                    {
                        title = title
                        subtitle = subtitle
                        url = url
                        content = content
                        date = date
                    } map
            ) Map.empty
        else
            eprintfn "warning: the posts folder (%s) does not exist." folder
            Map.empty

    let Menu blogs = [
        "Home", "/", Map.empty
        "Blog", "/blog", blogs
        "Try F#", "https://tryfsharp.fsbolero.io", Map.empty
    ]

    let private head =
        __SOURCE_DIRECTORY__ + "/js/Client.head.html"
        |> File.ReadAllText
        |> Doc.Verbatim

    let Page (title: option<string>) hasBanner blogs (body: Doc) =
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
            .TopMenu(Menu blogs |> List.map (function
                | text, url, map when Map.isEmpty map ->
                    MainTemplate.TopMenuItem()
                        .Text(text)
                        .Url(url)
                        .Doc()
                | text, url, children ->
                    let items =
                        children
                        |> Map.toList
                        |> List.sortByDescending (fun (key, item) -> item.date)
                        |> List.map (fun (key, item) ->
                            MainTemplate.TopMenuDropdownItem()
                                .Text(item.title)
                                .Url(item.url)
                                .Doc())
                    MainTemplate.TopMenuItemWithDropdown()
                        .Text(text)
                        .Url(url)
                        .DropdownItems(items)
                        .Doc()
            ))
            .DrawerMenu(Menu blogs |> List.map (fun (text, url, children) ->
                MainTemplate.DrawerMenuItem()
                    .Text(text)
                    .Url(url)
                    .Children(
                        match url with
                        | "/blog" ->
                            ul []
                                (children
                                |> Map.toList
                                |> List.sortByDescending (fun (_, item) -> item.date)
                                |> List.map (fun (_, item) ->
                                    MainTemplate.DrawerMenuItem()
                                        .Text(item.title)
                                        .Url(item.url)
                                        .Doc()
                                ))
                        | _ -> Doc.Empty
                    )
                    .Doc()
            ))
            .Body(body)
            .Doc()
        |> Content.Page

    let BlogSidebar (blogs: Map<string, BlogPage>) (blog: BlogPage) =
        blogs
        |> Map.toList
        |> List.sortByDescending (fun (_, blog) -> blog.date)
        |> List.map (fun (_, item) ->
            let tpl =
                MainTemplate.DocsSidebarItem()
                    .Title(item.title)
                    .Url(item.url)
                    .SubItemsAttr(attr.``class`` "is-hidden")
            tpl.Doc()
        )
        |> Doc.Concat
    
    let PlainHtml html =
        div [Attr.Create "ws-preserve" ""] [Doc.Verbatim html]

    let BlogPage blogs (blog: BlogPage) =
        MainTemplate.DocsBody()
            .Title(blog.title)
            .Subtitle(Doc.Verbatim blog.subtitle)
            .Sidebar(BlogSidebar blogs blog)
            .Content(PlainHtml blog.content)
            .Doc()
        |> Page (Some blog.title) false blogs

    let Main blogs =
        Application.MultiPage (fun (ctx: Context<_>) -> function
            | Home ->
                Content.Text "Hello"
            | BlogPage p ->
                BlogPage blogs blogs.[p]
        )

[<Sealed>]
type Website() =
    let blogs = Site.BlogPages ()

    interface IWebsite<EndPoint> with
        member this.Sitelet = Site.Main blogs
        member this.Actions = [
            yield Home
            for (fname, blog) in Map.toList blogs do
                yield BlogPage fname
        ]

[<assembly: Website(typeof<Website>)>]
do ()
