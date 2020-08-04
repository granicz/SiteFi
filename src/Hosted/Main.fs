namespace Website

open System
open System.Xml.Linq
open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "GET /trainings">] Trainings
    | [<EndPoint "GET /blogs">] Blogs of lang:string
    // User-less blog articles
    | [<EndPoint "GET /article">] Article of slug:string
    // UserArticle: if slug is empty, we go to the user's home page
    | [<EndPoint "GET /user">] UserArticle of user:string * slug:string
    // Old URL format for blog articles
    | [<EndPoint "GET /blog">] Redirect1 of id1:int * slug:string
    | [<EndPoint "GET /category">] Category of string * lang:string
    | [<EndPoint "GET /feed.atom">] AtomFeed
    | [<EndPoint "GET /feed.rss">] RSSFeed
    | [<EndPoint "GET /refresh">] Refresh

// Utilities to make XML construction somewhat sane
[<AutoOpen>]
module Xml =
    let TEXT (s: string) = XText(s)
    let (=>) (a1: string) (a2: string) = XAttribute(XName.Get a1, a2)
    let N = XName.Get
    let X (tag: XName) (attrs: XAttribute list) (content: obj list) =
        XElement(tag, List.map box attrs @ List.map box content)

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
// These need to match the endpoint type of the main sitelet.
module Urls =
    let CATEGORY (cat: string) lang =
        if String.IsNullOrEmpty lang then
            sprintf "/category/%s" cat
        else
            sprintf "/category/%s/%s" cat lang
    let POST_URL (user: string, slug: string) =
        if String.IsNullOrEmpty user then
            sprintf "/blog/%s.html" slug
        else
            sprintf "/user/%s/%s" user slug
    let OLD_TO_POST_URL (user: string, datestring: string, oldslug: string) =
        POST_URL (user, sprintf "%s-%s" datestring oldslug)
    let USER_URL user =
        if String.IsNullOrEmpty user then
            sprintf "/user"
        else
            sprintf "/user/%s" user
    let LANG (lang: string) = sprintf "/%s" lang

module Helpers =
    open System.IO
    open System.Text.RegularExpressions

    let NULL_TO_EMPTY (s: string) = match s with null -> "" | t -> t

    let FORMATTED_DATE (dt: DateTime) = dt.ToString("MMM dd, yyyy")
    let ATOM_DATE (dt: DateTime) = dt.ToString("yyyy-MM-dd'T'HH:mm:ssZ")
    let RSS_DATE (dt: DateTime) = dt.ToString("ddd, dd MMM yyyy HH:mm:ss UTC")

    // Return (fullpath, filename-without-extension, (year, month, day), slug, extension)
    let (|ArticleFile|_|) (fullpath: string) =
        let I s = Int32.Parse s
        let filename = Path.GetFileName(fullpath)
        let filenameWithoutExt = Path.GetFileNameWithoutExtension(fullpath)
        let r = new Regex("^(([0-9]+)-([0-9]+)-([0-9]+))-(.+)\.(md)")
        let r2 = new Regex("^(([1-2][0-9][0-9][0-9])([0-1][0-9])([0-3][0-9]))-(.+)\.(md)")
        if r.IsMatch(filename) then
            let a = r.Match(filename)
            let V (i: int) = a.Groups.[i].Value
            Some (fullpath, filenameWithoutExt, V 1, (I (V 2), I (V 3), I (V 4)), V 5, V 6)
        elif r2.IsMatch(filename) then
            let a = r2.Match(filename)
            let V (i: int) = a.Groups.[i].Value
            Some (fullpath, filenameWithoutExt, V 1, (I (V 2), I (V 3), I (V 4)), V 5, V 6)
        else
            None

    let (|NUMBER|_|) (s: string) =
        let out = ref 0
        if Int32.TryParse(s, out) then Some !out else None

[<JavaScript>]
module ClientSideCode =

    module Swiper =
        open System.Web
        open WebSharper.UI.Html
        open WebSharper.JQuery
        open WebSharper.JavaScript

        [<assembly:WebResource("/assets/swiper.js", "text/javascript")>]
        [<assembly:WebResource("/assets/swiper.css", "style/css")>]
        do ()

        type private CssResource() =
            inherit Resources.BaseResource("/assets/swiper.css")

        type private JsResource() =
            inherit Resources.BaseResource("/assets/swiper.js")

        [<Require(typeof<CssResource>)>]
        [<Require(typeof<JsResource>)>]
        type Swiper private () =

            [<Inline "$this.params.slidesPerView = $slides">]
            member x.SetSlidesPerView (slides : obj) = ()

            [<Inline "$this.init()">]
            member x.Init () = ()

        [<Inline "new Swiper($selector, {scrollbar: $scrollbar, scrollbarHide: $scrollbarHide, slidesPerView: \"auto\", ceteredSlides: true, grabCursor: true, freeMode: $freeMode, preloadImages: false, lazyLoading: true, watchSlidesVisibility: true})">]
        let SwiperWithScrollLazy (selector : string) (scrollbar : string) (scrollbarHide: bool) (freeMode : bool) = X<Swiper>

        [<Require(typeof<CssResource>)>]
        [<Require(typeof<JsResource>)>]
        let InitImageSwiper () =
            JQuery.Of(fun () -> 
                SwiperWithScrollLazy 
                    ".image-swiper-container" ".image-swiper-scrollbar" false true
                |> ignore).Ignore

        let Init () =
            InitImageSwiper ()
            text ""

    module TalksAndPresentations =
        open WebSharper.JavaScript
        open WebSharper.UI.Html
        open WebSharper.Google.Maps
        open WebSharper.JQuery

        [<Inline "($options).styles=$styleJson">]
        let FixMapStyles (options: MapOptions) (styleJson: string) = failwith "N/A"

        [<Inline "eval($styleJson)">]
        let WireMapStyles (styleJson: string) : MapTypeStyle[] = failwith "N/A"

        type City =
            {
                City: string
                Country: string
                Latitude: float
                Longitude: float
            }

        let Ajax<'T> (url: string) : Async<'T> =
            Async.FromContinuations <| fun (ok, ko, _) ->
                let s = JQuery.AjaxSettings()
                JQuery.Ajax(
                    JQuery.AjaxSettings(
                        Url = url,
                        Type = RequestType.GET,
                        ContentType = Union<bool, string>.Union2Of2 "application/json",
                        DataType = JQuery.DataType.Text,
                        Success = (fun result _ _ -> ok (result :?> 'T)),
                        Error = (fun jqXHR _ _ -> ko (System.Exception(jqXHR.ResponseText)))): AjaxSettings)
                |> ignore

        let GetCities () =
            async {
                let! raw = Ajax<string> @"\assets\cities.txt"
                let lines = raw.Split([| "\r\n"; "\n" |], StringSplitOptions.RemoveEmptyEntries)
                let res =
                    lines
                    |> Seq.map (fun line ->
                        let parts = line.Split([| ',' |])
                        try
                            {
                                City = parts.[0]
                                Country = parts.[1]
                                Latitude = float parts.[2]
                                Longitude = float parts.[3]
                            }
                        with
                        | exn ->
                            Console.Log exn.Message
                            Console.Log exn.StackTrace
                            raise exn
                    )
                    |> Seq.toList
                return res
            }
    
        [<JavaScript>]
        let GMap (styleJson: string) =
            div [
                attr.``class`` "inner-map"
                on.afterRender (fun el ->
                    let point = new LatLng(50.0, -20.0)
                    let options = 
                        MapOptions(
         //                  MapTypeId = MapTypeId.TERRAIN,
                            Center = point,
                            Zoom = 3,
                            Styles = WireMapStyles styleJson,
                            Scrollwheel = false,
                            DisableDefaultUI = true
                        )
        //            let options = FixMapStyles options styleJson
                    let map = new Map(el, options)

                    async {
                        let! data = GetCities()
                        data
                        |> Seq.iter (fun m ->
                            let point = new LatLng(m.Latitude, m.Longitude)
                            let icon = Icon(Url = "/img/map-marker.png", Anchor = Point(8.0, 8.0))
                            new Marker(MarkerOptions(point, Map = map, Title = sprintf "%s, %s" m.City m.Country, Icon = icon)) |> ignore   
                        )
                    }
                    |> Async.Start
                )
            ] []

module Site =
    open System.IO
    open WebSharper.UI.Html

    type MainTemplate = Templating.Template<"../hosted/index.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type RedirectTemplate = Templating.Template<"../hosted/redirect.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type [<CLIMutable>] RawConfig =
        {
            serverUrl: string
            shortTitle: string
            title: string
            description: string
            masterUserDisplayName: string
            masterLanguage: string
            languages: string
            users: string
        }

    type Config =
        {
            ServerUrl: string
            ShortTitle: string
            Title: string
            Description: string
            MasterUserDisplayName: string
            MasterLanguage: string
            Languages: Map<string, string>
            Users: Map<string, string>
        }

    type [<CLIMutable>] RawArticle =
        {
            title: string
            subtitle: string
            ``abstract``: string
            url: string
            content: string
            date: string
            categories: string
            language: string
            identity: string
        }

    type Article =
        {
            Title: string
            Subtitle: string
            Abstract: string
            Url: string
            Content: string
            DateString: string
            SlugWithoutDate: string
            Date: DateTime
            Categories: string list
            Language: string
            Identity: int * int
        }

    // The article store, mapping (user*slug) pairs to articles.
    type Articles = Map<string*string, Article>

    // Mapping Id1 -> (username, datestring)
    type Identities1 = Map<int, string*string>

    /// Zero out if article has the master language
    let URL_LANG (config: Config) lang =
        if config.MasterLanguage = lang then "" else lang

    let ReadConfig() =
        let KEY_VALUE_LIST whatFor ss =
            (Helpers.NULL_TO_EMPTY ss).Split([| "," |], StringSplitOptions.None)
            |> Array.choose (fun s ->
                if String.IsNullOrEmpty s then
                    None
                else
                    let parts = s.Split([| "->" |], StringSplitOptions.None)
                    if Array.length parts <> 2 then
                        eprintfn "warning: Incorrect key-value format for substring [%s] in [%s] for [%s], ignoring." s ss whatFor
                        None
                    else
                        Some (parts.[0], parts.[1])
            )
            |> Set.ofArray
            |> Set.toList
            |> Map.ofList
        let config = Path.Combine (__SOURCE_DIRECTORY__, @"../hosted/config.yml")
        if File.Exists config then
            let config = Yaml.OfYaml<RawConfig> (File.ReadAllText config)
            let languages = KEY_VALUE_LIST "languages" config.languages
            let users = KEY_VALUE_LIST "users" config.users
            {
                ServerUrl = Helpers.NULL_TO_EMPTY config.serverUrl
                ShortTitle = Helpers.NULL_TO_EMPTY config.shortTitle
                Title = Helpers.NULL_TO_EMPTY config.title
                Description = Helpers.NULL_TO_EMPTY config.description
                MasterUserDisplayName = Helpers.NULL_TO_EMPTY config.masterUserDisplayName
                MasterLanguage = Helpers.NULL_TO_EMPTY config.masterLanguage
                Languages = languages
                Users = users
            }
        else
            {
                ServerUrl = "http://localhost:5000"
                ShortTitle = "My Blog"
                Title = "My F# Blog"
                Description = "TODO: write the description of this blog"
                MasterUserDisplayName = "My Name"
                MasterLanguage = "en"
                Languages = Map.ofList ["en", "English"]
                Users = Map.empty
            }

    let ReadArticles() : Articles =
        let root = Path.Combine (__SOURCE_DIRECTORY__, @"../hosted/posts")
        let ReadFolder user store =
            let folder = Path.Combine (root, user)
            if Directory.Exists folder then
                Directory.EnumerateFiles(folder, "*.md", SearchOption.TopDirectoryOnly)
                |> Seq.toList
                |> List.choose (Helpers.(|ArticleFile|_|))
                |> List.fold (fun map (fullpath, fname, datestring, (year, month, day), slug, extension) ->
                    eprintfn "Found file: %s" fname
                    let header, content =
                        File.ReadAllText fullpath
                        |> Yaml.SplitIntoHeaderAndContent
                    let article = Yaml.OfYaml<RawArticle> header
                    let title = Helpers.NULL_TO_EMPTY article.title
                    let subtitle = Helpers.NULL_TO_EMPTY article.subtitle
                    let ``abstract`` = Helpers.NULL_TO_EMPTY article.``abstract``
                    let url = Urls.POST_URL (user, fname)
                    eprintfn "DEBUG-URL: %s" url
                    // If the content is given in the header, use that instead.
                    let content =
                        if article.content <> null then
                            Markdown.Convert article.content
                        else
                            Markdown.Convert content
                    let date = DateTime(year, month, day)
                    let categories =
                        Helpers.NULL_TO_EMPTY article.categories
                    // Clean up article tags/categories:
                    let categories =
                        if not <| String.IsNullOrEmpty categories then
                            categories.Split [| ',' |]
                            // Note: categories are case-sensitive.
                            // Trim each and convert the "#" character - so "c/f#" becomes "c/fsharp" 
                            |> Array.map (fun cat -> cat.Trim().Replace("#", "sharp"))
                            |> Array.filter (not << String.IsNullOrEmpty)
                            |> Set.ofArray
                            |> Set.toList
                        else
                            []
                    let language = Helpers.NULL_TO_EMPTY article.language
                    let identity =
                        let raw = Helpers.NULL_TO_EMPTY article.identity
                        let entries = raw.Split([| ',' |])
                        match entries with
                        | [| Helpers.NUMBER id1; Helpers.NUMBER id2 |] ->
                            id1, id2
                        | _ ->
                            failwithf "Invalid identity found (%A)" entries
                    eprintfn "DEBUG-ADD: (%s, %s)\n-------------------" user fname
                    Map.add (user, fname)
                        {
                            Title = title
                            Subtitle = subtitle
                            Abstract = ``abstract``
                            Url = url
                            Content = content
                            DateString = datestring
                            SlugWithoutDate = slug
                            Date = date
                            Categories = categories
                            Language = language
                            Identity = identity
                        } map
                ) store
            else
                eprintfn "warning: the posts folder (%s) does not exist." folder
                store
        
        Directory.EnumerateDirectories(root)
        // Read user articles
        |> Seq.fold (fun store folder ->
            ReadFolder (Path.GetFileName(folder)) store) Map.empty
        // Read main articles
        |> ReadFolder ""

    // Here we map the Id1 -> (user, datestring).
    let ComputeIdentities1 (articles: Articles) : Identities1 =
        articles
        |> Map.fold (fun map (user, _) article ->
            Map.add (fst article.Identity) (user, article.DateString) map
        ) Map.empty

    let Menu articles =
        let latest =
            articles
            |> Map.toList
            |> List.sortByDescending (fun (_, article: Article) -> article.Date.Ticks)
            |> List.truncate 5
            |> Map.ofList
        [
            "Home", "/", Map.empty
            "Bolero", "https://fsbolero.io", Map.empty
            "WebSharper", "https://websharper.com", Map.empty
            "CloudSharper", "https://cloudsharper.com", Map.empty
            "Trainings", "/trainings", Map.empty
            "Jobs", "/jobs", Map.empty
            "Blogs", "/blogs", Map.empty
            "Contact", "/contact", Map.empty
        ]

    let private head() =
        __SOURCE_DIRECTORY__ + "/../Hosted/js/Client.head.html"
        |> File.ReadAllText
        |> Doc.Verbatim
    let private mapStyles() =
        __SOURCE_DIRECTORY__ + "/../Hosted/assets/home-map-styles.json"
        |> File.ReadAllText

    let Page (config: Config) (pageTitle: option<string>) hasBanner transparentHeader articles (body: Doc) =
        let head = head()
        MainTemplate()
#if !DEBUG
            .ReleaseMin(".min")
#endif
            .IsTransparentHeader(if transparentHeader then "transparent-navbar" else "")
            .NavbarOverlay(if hasBanner then "overlay-bar" else "")
            .Head(head)
            .ShortTitle(config.ShortTitle)
            .Title(
                match pageTitle with
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

    let ArticleBasePage langopt (config: Config) (pageTitle: option<string>) hasBanner transparentHeader articles (body: Doc) =
        // Compute the language keys used in all articles
        let languages =
            articles
            |> Map.toList
            |> List.map (fun (_, art) -> art.Language)
            |> List.distinct
            // Filter out the master language
            |> List.filter (fun lang ->
                URL_LANG config lang |> (String.IsNullOrEmpty >> not)
            )
        // Add back the default language IFF there is at least one other language
        let languages =
            // Turn a language key to a (key, displayname) pair.
            // Empty input corresponds to the master language.
            let LANG lang =
                let langkey =
                    if String.IsNullOrEmpty lang then config.MasterLanguage else lang
                if config.Languages.ContainsKey langkey then
                    lang, config.Languages.[langkey]
                else
                    lang, langkey
            if languages.Length > 0 then
                (LANG "") :: List.map LANG languages
            else
                []
        let head = head()
        MainTemplate()
#if !DEBUG
            .ReleaseMin(".min")
#endif
            .IsTransparentHeader(if transparentHeader then "transparent-navbar" else "")
            .NavbarOverlay(if hasBanner then "overlay-bar" else "")
            .Head(head)
            .ShortTitle(config.ShortTitle)
            .Title(
                match pageTitle with
                | None -> ""
                | Some t -> t + " | "
            )
            .LanguageSelectorPlaceholder(
                if languages.IsEmpty then
                    Doc.Empty
                else
                    MainTemplate.LanguageSelector()
                        .Languages(
                            languages
                            |> List.map (fun (url_lang, lang) ->
                                if langopt = url_lang then
                                    MainTemplate.LanguageItemActive()
                                        .Title(lang)
                                        .Url(Urls.LANG url_lang)
                                        .Doc()
                                else
                                    MainTemplate.LanguageItem()
                                        .Title(lang)
                                        .Url(Urls.LANG url_lang)
                                        .Doc()
                            )
                        )
                        .Doc()
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

    let BlogSidebar config articles (article: Article) =
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
                                    .Url(Urls.CATEGORY category (URL_LANG config article.Language))
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

    let ArticlePage (config: Config) articles (article: Article) =
        // Zero out if article has the master language
        let langopt = URL_LANG config article.Language
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
            .Sidebar(BlogSidebar config articles article)
            .Doc()
        |> ArticleBasePage langopt config (Some article.Title) false false articles

    // The silly ref's are needed because offline sitelets are
    // initialized in their own special way, without having access
    // to top-level values.
    let articles : Articles ref = ref Map.empty
    let identities1 : Identities1 ref = ref Map.empty
    let config : Config ref = ref <| ReadConfig()

    let Main (config: Config ref) (identities1: Identities1 ref) (articles: Articles ref) =
        let ARTICLES (articles: Articles) =
            let articles =
                articles
                |> Map.toList
                |> List.sortByDescending (fun (_, a: Article) -> a.Date.Ticks)
            [ for ((user, _), article) in articles ->
                let displayName =
                    if String.IsNullOrEmpty user then
                        config.Value.MasterUserDisplayName
                    elif Map.containsKey user config.Value.Users then
                        config.Value.Users.[user]
                    else user
                MainTemplate.ArticleCard()
                    .Author(
                        a [attr.href <| Urls.USER_URL user] [text displayName]
                    )
                    .Title(article.Title)
                    .Abstract(article.Abstract)
                    .Url(article.Url)
                    .Date(Helpers.FORMATTED_DATE article.Date)
                    .ArticleCategories(
                        if article.Categories.IsEmpty then
                            Doc.Empty
                        else
                            article.Categories
                            |> List.map (fun category ->
                                MainTemplate.ArticleCategory()
                                    .Title(category)
                                    .Url(Urls.CATEGORY category (URL_LANG config.Value article.Language))
                                    .Doc()
                            )
                            |> Doc.Concat
                    )
                    .Doc()
            ]                        
        let ARTICLE (user, p: string) =
            let page =
                if p.ToLower().EndsWith(".html") then
                    p.Substring(0, p.Length-5)
                else
                    p
            let key = user, page
            if articles.Value.ContainsKey key then
                ArticlePage config.Value articles.Value articles.Value.[key]
            else
                Map.toList articles.Value
                |> List.map fst
                |> sprintf "Trying to find page \"%s\" (with key=\"%s\"), but it's not in %A" p page
                |> Content.Text
        let TRAININGS () =
            let mapStyles = mapStyles()
            MainTemplate.TrainingBody()
                .Map(client <@ ClientSideCode.TalksAndPresentations.GMap(mapStyles) @>)
                .ImageSliderInit(client <@ ClientSideCode.Swiper.Init() @>)
                .Doc()
            |> Page config.Value None false true Map.empty
        let HOME langopt (banner: Doc) f =
            MainTemplate.HomeBody()
                .Banner(banner)
                .ArticleList(
                    Map.filter f articles.Value
                    |> ARTICLES
                )
                .Doc()
            |> ArticleBasePage langopt config.Value None false true articles.Value
        let REDIRECT_TO (url: string) =
            RedirectTemplate()
                .Url(url)
                .Doc()
            |> Content.Page
        Application.MultiPage (fun (ctx: Context<_>) -> function
            | Trainings ->
                TRAININGS ()
            | Blogs langopt ->
                HOME langopt
                    <| MainTemplate.HomeBanner()
                        .Title(config.Value.Title)
                        .Subtitle(config.Value.Description)
                        .Doc()
                    <| fun _ article ->
                        langopt = URL_LANG config.Value article.Language
            | Article p ->
                ARTICLE ("", p)
            | UserArticle (user, "") ->
                HOME ""
                    <| MainTemplate.HomeBanner().Doc()
                    <| fun (u, _) _ -> user = u
            | UserArticle (user, p) ->
                ARTICLE (user, p)
            | Redirect1 (id1, oldslug) ->
                let user, datestring =
                    if identities1.Value.ContainsKey id1 then
                        identities1.Value.[id1]
                    else
                        failwithf "Unable to find user for id1=%d, with map=%A" id1 identities1.Value
                REDIRECT_TO (Urls.OLD_TO_POST_URL (user, datestring, oldslug))
            | Category (cat, langopt) ->
                HOME langopt
                    <| MainTemplate.CategoryBanner()
                        .Category(cat)
                        .Doc()
                    <| fun _ article ->
                        langopt = URL_LANG config.Value article.Language
                        &&
                        List.contains cat article.Categories
            // For a simple but useful reference on Atom vs RSS content, refer to:
            // https://www.intertwingly.net/wiki/pie/Rss20AndAtom10Compared
            | AtomFeed ->
                Content.Custom (
                    Status = Http.Status.Ok,
                    Headers = [Http.Header.Custom "content-type" "application/atom+xml"],
                    WriteBody = fun stream ->
                        let ns = XNamespace.Get "http://www.w3.org/2005/Atom"
                        let articles =
                            articles.Value |> Map.toList |> List.sortByDescending (fun (_, article: Article) -> article.Date.Ticks)
                        let doc =
                            X (ns + "feed") [] [
                                X (ns + "title") [] [TEXT config.Value.Title]
                                X (ns + "subtitle") [] [TEXT config.Value.Description]
                                X (ns + "link") ["href" => config.Value.ServerUrl] []
                                X (ns + "updated") [] [Helpers.ATOM_DATE DateTime.UtcNow]
                                for ((user, slug), article) in articles do
                                    X (ns + "entry") [] [
                                        X (ns + "title") [] [TEXT article.Title]
                                        X (ns + "link") ["href" => config.Value.ServerUrl + Urls.POST_URL (user, slug)] []
                                        X (ns + "id") [] [TEXT (user+slug)]
                                        for category in article.Categories do
                                            X (ns + "category") [] [TEXT category]
                                        X (ns + "summary") [] [TEXT article.Abstract]
                                        X (ns + "updated") [] [TEXT <| Helpers.ATOM_DATE article.Date]
                                    ]
                            ]
                        doc.Save(stream)
                )
            | RSSFeed ->
                Content.Custom (
                    Status = Http.Status.Ok,
                    Headers = [Http.Header.Custom "content-type" "application/rss+xml"],
                    WriteBody = fun stream ->
                        let articles =
                            articles.Value |> Map.toList |> List.sortByDescending (fun (_, article: Article) -> article.Date.Ticks)
                        let doc =
                            X (N "rss") ["version" => "2.0"] [
                                X (N "channel") [] [
                                    X (N "title") [] [TEXT config.Value.Title]
                                    X (N "description") [] [TEXT config.Value.Description]
                                    X (N "link") [] [TEXT config.Value.ServerUrl]
                                    X (N "lastBuildDate") [] [Helpers.RSS_DATE DateTime.UtcNow]
                                    for ((user, slug), article) in articles do
                                        X (N "item") [] [
                                            X (N "title") [] [TEXT article.Title]
                                            X (N "link") [] [TEXT <| config.Value.ServerUrl + Urls.POST_URL (user, slug)]
                                            X (N "guid") ["isPermaLink" => "false"] [TEXT (user+slug)]
                                            for category in article.Categories do
                                                X (N "category") [] [TEXT category]
                                            X (N "description") [] [TEXT article.Abstract]
                                            X (N "pubDate") [] [TEXT <| Helpers.RSS_DATE article.Date]
                                        ]
                                ]
                            ]
                        doc.Save(stream)
                )
            | Refresh ->
                // Reload the article cache and the master configs
                articles := ReadArticles()
                identities1 := ComputeIdentities1 articles.Value
                config := ReadConfig()
                Content.Text "Articles/configs reloaded."
        )

open System.IO

[<Sealed>]
type Website() =
    let articles = ref <| Site.ReadArticles()
    let identities1 = ref <| Site.ComputeIdentities1 articles.Value
    let config = ref <| Site.ReadConfig()

    interface IWebsite<EndPoint> with
        member this.Sitelet = Site.Main config identities1 articles
        member this.Actions =
            let articles = Map.toList articles.Value
            let categories =
                articles
                |> List.map snd
                |> List.collect (fun article -> article.Categories)
                |> Set.ofList
                |> Set.toList
            let languages =
                articles
                |> List.map snd
                |> List.map (fun article -> Site.URL_LANG config.Value article.Language)
                |> Set.ofList
                |> Set.toList
            let users =
                articles
                |> List.map (fst >> fst)
                |> Set.ofList
                |> Set.toList
            eprintfn "DEBUG-users: %A" users
            [
                // Generate the learning page
                Trainings
                // Generate the blog home page(s), one per language
                for language in languages do
                    Blogs language
                // Generate redirection pages for the old article pages
                for (_, article) in articles do
                    Redirect1 (fst article.Identity, article.SlugWithoutDate)
                // Generate articles
                for ((user, slug), _) in articles do
                    if String.IsNullOrEmpty user then
                        Article slug
                    else
                        UserArticle (user, slug)
                // Generate user pages
                for user in users do
                    UserArticle (user, "")
                // Generate tag/category pages
                for category in categories do
                    for language in languages do
                        if
                            List.exists (fun (_, (art: Site.Article)) ->
                                language = Site.URL_LANG config.Value art.Language
                                &&
                                List.contains category art.Categories 
                            ) articles
                        then
                            Category (category, language)
                // Generate the RSS/Atom feeds
                RSSFeed
                AtomFeed
            ]

[<assembly: Website(typeof<Website>)>]
do ()
