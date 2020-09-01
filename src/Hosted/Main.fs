namespace Website

open System
open System.Xml.Linq
open System.Text.RegularExpressions
open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type BlogListingArgs =
    | [<EndPoint "">] Empty
    | [<EndPoint "">] Index of int
    | [<EndPoint "">] LanguageAndIndex of string * int

type EndPoint =
    | [<EndPoint "GET /trainings">] Trainings
    | [<EndPoint "GET /blogs">] Blogs of BlogListingArgs
    // User-less blog articles
    | [<EndPoint "GET /post">] Article of slug:string
    // UserArticle: if slug is empty, we go to the user's home page
    | [<EndPoint "GET /user">] UserArticle of user:string * slug:string
    // Old URL format for blog articles
    | [<EndPoint "GET /blog">] Redirect1 of id1:int * slug:string
    | [<EndPoint "GET /category">] Category of string * lang:string
    | [<EndPoint "GET /feed.atom">] AtomFeed
    | [<EndPoint "GET /feed.rss">] RSSFeed
    | [<EndPoint "GET /refresh">] Refresh
    | [<EndPoint "GET /contact">] Contact
    | [<EndPoint "GET /termsofuse">] TermsOfUse
    | [<EndPoint "GET /privacypolicy">] PrivacyPolicy
    | [<EndPoint "GET /cookiepolicy">] CookiePolicy
    | [<EndPoint "GET /404.html">] Error404

// Utilities to make XML construction somewhat sane
[<AutoOpen>]
module Xml =
    let TEXT (s: string) = XText(s)
    let (=>) (a1: string) (a2: string) = XAttribute(XName.Get a1, a2)
    let N = XName.Get
    let X (tag: XName) (attrs: XAttribute list) (content: obj list) =
        XElement(tag, List.map box attrs @ List.map box content)

module Cookies =
    open WebSharper.UI.Html

    /// Server-side function to include the cookie acceptance banner and Google Analytics script.
    let Banner (includeGoogleAnalytics: bool) =
        // Using combinators rather than HTML template because server-side templates don't work
        // across libraries right now :(
        Doc.Concat [
            div [attr.id "cookie-banner"] [
                p [] [text "This site uses cookies and other tracking technologies to assist with navigation \
                            and your ability to provide feedback, and analyse your use of our products and services."]
                a [attr.target "_blank"; attr.href "/cookie-policy"] [text "Read our Cookie Policy"]
                div [] [
                    button [Attr.Create "onclick" "wscookies.accept()"] [text "Accept cookies"]
                    button [Attr.Create "onclick" "wscookies.refuse()"] [text "Refuse cookies"]
                ]
            ]
            script [] [
                Doc.Verbatim """
    var wscookies = {
        accepted: document.cookie.replace(/(?:(?:^|.*;\s*)cookie_accept\s*\=\s*([^;]*).*$)|^.*$/, "$1"),
        accept: function () {
        document.cookie = 'cookie_accept=true;max-age=31536000';
        this.set_banner('none');
        this.after_accept();
        },
        after_accept: function () { },
        refuse: function () {
        document.cookie = 'cookie_accept=false;max-age=31536000';
        this.set_banner('none');
        },
        set_banner: function (style) {
        document.getElementById('cookie-banner').style.display = style;
        }
    };
    switch (wscookies.accepted) {
        case '':
        wscookies.set_banner('flex');
        break;
        case 'true':
        wscookies.accept();
        break;
        default:
        wscookies.refuse();
    }
"""
            ]
        ]

module Markdown =
    open Markdig

    let pipeline =
        MarkdownPipelineBuilder()
            .UseAutoIdentifiers()
            .UsePipeTables()
            .UseFootnotes()
            .UseGridTables()
            .UseListExtras()
            .UseEmphasisExtras()
            .UseAutoLinks()
            .UseTaskLists()
            .UseMediaLinks()
            .UseCustomContainers()
            .UseGenericAttributes()
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
            sprintf "/post/%s.html" slug
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

        [<JavaScript>]
        let GMapOffice (styleJson: string) =
            div [
                attr.``class`` "inner-map"
                on.afterRender (fun el ->
                    let point = new LatLng(47.48543, 19.071336)
                    let options = 
                        MapOptions(
         //                  MapTypeId = MapTypeId.TERRAIN,
                            Center = point,
                            Zoom = 15,
                            Styles = WireMapStyles styleJson,
                            Scrollwheel = false,
                            DisableDefaultUI = true,
                            ZoomControl = true
                        )
        //            let options = FixMapStyles options styleJson
                    let map = new Map(el, options)
                    let point = new LatLng(47.48543, 19.071336)
                    let icon = Icon(Url = "/img/map-marker.png", Anchor = Point(8.0, 8.0))
                    new Marker(MarkerOptions(point, Map = map, Title = "IntelliFactory", Icon = icon)) |> ignore
                )
            ] []

module Site =
    open System.IO
    open WebSharper.UI.Html

    type MainTemplate = Templating.Template<"../Hosted/index.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type RedirectTemplate = Templating.Template<"../Hosted/redirect.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type TrainingsTemplate = Templating.Template<"../Hosted/trainings.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type BlogListTemplate = Templating.Template<"../Hosted/bloglist.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type BlogPostTemplate = Templating.Template<"../Hosted/blogpost.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type ContactTemplate = Templating.Template<"../Hosted/contact.html", serverLoad=Templating.ServerLoad.WhenChanged>
    type LegalTemplate = Templating.Template<"../Hosted/legal.html", serverLoad=Templating.ServerLoad.WhenChanged>

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
            pageSize: int
            githubRepo: string
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
            PageSize: int
            GitHubRepo: string
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
        let config = Path.Combine (__SOURCE_DIRECTORY__, @"../Hosted/config.yml")
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
                PageSize = if config.pageSize > 0 then config.pageSize else 30
                GitHubRepo = Helpers.NULL_TO_EMPTY config.githubRepo
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
                PageSize = 30
                GitHubRepo = "https://github.com/IntelliFactory/blogs"
            }

    let ReadArticles() : Articles =
        let root = Path.Combine (__SOURCE_DIRECTORY__, @"../Hosted/posts")
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

    let private head() =
        __SOURCE_DIRECTORY__ + "/../Hosted/js/Client.head.html"
        |> File.ReadAllText
        |> Doc.Verbatim
    let private mapStyles() =
        __SOURCE_DIRECTORY__ + "/../Hosted/assets/home-map-styles.json"
        |> File.ReadAllText

    let private mapContactStyles() =
        __SOURCE_DIRECTORY__ + "/../Hosted/assets/contact-map-styles.json"
        |> File.ReadAllText

    let private menubar(config: Config) =
        MainTemplate.Menubar()
            .ShortTitle(config.ShortTitle)
            .Doc()

    let ArticleBasePage langopt (config: Config) (pageTitle: option<string>) hasBanner (transparentHeader: bool) articles (body: Doc) =
        let head = head()
        MainTemplate()
#if !DEBUG
            .ReleaseMin(".min")
#endif
            .IsTransparentHeader(if transparentHeader then "menu-transparent" else "")
            // TODO: .NavbarOverlay(if hasBanner then "overlay-bar" else "")
            .Head(head)
            .MenuBar(menubar config)
            .Title(
                match pageTitle with
                | None -> ""
                | Some t -> t + " | "
            )
            .Body(body)
            .Cookie(Cookies.Banner false)
            .Doc()
        |> Content.Page

    let BlogSidebar config articles (article: Article) =
        MainTemplate.Sidebar()
            .Categories(
                // Render the categories widget iff there are categories
                if article.Categories.IsEmpty then
                    Doc.Empty
                else
                        article.Categories
                        |> List.map (fun category ->
                            MainTemplate.Category()
                                .Name(category)
                                .Url(Urls.CATEGORY category (URL_LANG config article.Language))
                                .Doc()
                        )
                        |> Doc.Concat
            )
            // There is always at least one blog post, so we render this
            // section no matter what.
            .ArticleItems(
                MainTemplate.ArticleItems()
                    .ArticleItems(
                        articles
                        |> Map.toList
                        |> List.sortByDescending (fun (_, item) -> item.Date)
                        |> List.truncate 10
                        |> List.map (fun (_, item) ->
                            MainTemplate.ArticleItem()
                                .Title(item.Title)
                                .Url(item.Url)
                                .Date(item.Date.ToShortDateString())
                                .Doc()
                        )
                    )
                    .Doc()
            )
            .Doc()

    let PLAIN html =
        div [Attr.Create "ws-preserve" ""] [Doc.Verbatim html]

    let ArticlePage (config: Config) articles (article: Article) =
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
        // Zero out if article has the master language
        let langopt = URL_LANG config article.Language
        // MainTemplate.ArticlePage()
        MainTemplate.ArticlePage()
            // Main content panel
            .Article(
                PLAIN article.Content
                //MainTemplate.Article()
                //    .Title(article.Title)
                //    .Subtitle(Doc.Verbatim article.Subtitle)
                //    .Content(PLAIN article.Content)
                //    .Doc()
            )
            .SourceCodeUrl(sprintf "%s/tree/master%s.md" config.GitHubRepo article.Url)
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
            .Date(article.DateString)
            .Title(article.Title)
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
        let getContent (ctx: Context<_>) fileName =
            use r = new StreamReader(Path.Combine(@"../Hosted/", "legal", fileName))
            r.ReadToEnd()
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
                // Assign a category to each article
                // 0 - General
                // 1 - Bolero
                // 2 - CloudSharper
                // 3 - WebSharper
                // 4 - Blogging/SiteFi
                // 5 - Release announcement
                let categoryNo =
                    let cats = List.map (fun (c: string) -> c.ToLower()) article.Categories
                    let rn = new Regex("^(Bolero|WebSharper|CloudSharper|SiteFi)\s[0-9\.]+\srelease")
                    if rn.IsMatch(article.Title) then
                        5
                    elif List.contains "bolero" cats then
                        1
                    elif List.contains "cloudsharper" cats then
                        2
                    elif List.contains "websharper" cats then
                        3
                    elif List.contains "blogging" cats || List.contains "sitefi" cats then
                        4
                    else
                        0
                let thumbnail =
                    match categoryNo with
                    | 1 ->
                        BlogListTemplate.Category1().Doc()
                    | 2 ->
                        BlogListTemplate.Category2().Doc()
                    | 3 ->
                        BlogListTemplate.Category3().Doc()
                    | 4 ->
                        BlogListTemplate.Category4().Doc()
                    | _ ->
                        BlogListTemplate.Category0().Doc()
                BlogListTemplate.ArticleCard()
                    .Author(
                        a [attr.href <| Urls.USER_URL user] [text displayName]
                    )
                    .Title(article.Title)
                    .CategoryNo(string categoryNo)
                    .Thumbnail(thumbnail)
                    .Url(article.Url)
                    .Date(Helpers.FORMATTED_DATE article.Date)
                    .ArticleCategories(
                        if article.Categories.IsEmpty then
                            Doc.Empty
                        else
                            article.Categories
                            |> List.map (fun category ->
                                BlogListTemplate.ArticleCategory()
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
            let header =
                TrainingsTemplate.TrainingBody()
                    .Map(client <@ ClientSideCode.TalksAndPresentations.GMap(mapStyles) @>)
                    .ImageSliderInit(client <@ ClientSideCode.Swiper.Init() @>)
                    .Doc()
            TrainingsTemplate()
                .MenuBar(menubar config.Value)
                .HeaderContent(header)
                .Cookie(Cookies.Banner false)
                .Doc()
            |> Content.Page
        let TERMSOFUSE (ctx: Context<_>) =
            LegalTemplate()
                .MenuBar(menubar config.Value)
                .HeaderContent(Doc.Empty)
                .Content(Doc.Verbatim <| Markdown.Convert (getContent ctx "TermsOfUse.md"))
                .Cookie(Cookies.Banner false)
                .Doc()
            |> Content.Page
        let COOKIEPOLICY (ctx: Context<_>) =
            LegalTemplate()
                .MenuBar(menubar config.Value)
                .HeaderContent(Doc.Empty)
                .Content(Doc.Verbatim <| Markdown.Convert (getContent ctx "CookiePolicy.md"))
                .Cookie(Cookies.Banner false)
                .Doc()
            |> Content.Page
        let PRIVACYPOLICY (ctx: Context<_>) =
            LegalTemplate()
                .MenuBar(menubar config.Value)
                .HeaderContent(Doc.Empty)
                .Content(Doc.Verbatim <| Markdown.Convert (getContent ctx "PrivacyPolicy.md"))
                .Cookie(Cookies.Banner false)
                .Doc()
            |> Content.Page
        let CONTACT () =
            let mapContactStyles = mapContactStyles()
            ContactTemplate()
                .MenuBar(menubar config.Value)
                .Map(client <@ ClientSideCode.TalksAndPresentations.GMapOffice(mapContactStyles) @>)
                .Cookie(Cookies.Banner false)
                .Doc()
            |> Content.Page
        // pageNo is 1-based
        let BLOG_LISTING (banner: Doc) (pageNo: int) f =
            let as1 =
                // Filter articles
                Map.filter f articles.Value
                |> Map.toList
                // Sort articles chronologically
                |> List.sortBy (fun ((user, slug), art) -> -art.Date.Ticks)
                // Slice out the articles on the given "page"
                |> List.chunkBySize config.Value.PageSize
            if List.length as1 >= pageNo-1 then
                let articles =
                    if List.length as1 = 0 then
                        Map.empty
                    else
                        List.item (pageNo-1) as1
                        |> Map.ofList
                let isFirst = pageNo = 1
                let isLast = List.length as1 - pageNo < 1
                BlogListTemplate()
                    .Menubar(menubar config.Value)
                    .Banner(banner)
                    .ArticleList(ARTICLES articles)
                    .Pagination(
                        BlogListTemplate.Paginator()
                            .PreviousExtraCss(if isFirst then "disabled" else "")
                            .PreviousUrl(if isFirst then "#" else sprintf "/blogs/%d" (pageNo-1))
                            .NextExtraCss(if isLast then "disabled" else "")
                            .NextUrl(if isLast then "#" else sprintf "/blogs/%d" (pageNo+1))
                            .Doc()
                    )
                    .Cookie(Cookies.Banner false)
                    .Doc()
                |> Content.Page
            else
                Content.Text "Page out of bounds"
        let BLOG_LISTING_NO_PAGING (banner: Doc) f =
            BlogListTemplate()
                .Menubar(menubar config.Value)
                .Banner(banner)
                .ArticleList(Map.filter f articles.Value |> ARTICLES)
                .Pagination(Doc.Empty)
                .Cookie(Cookies.Banner false)
                .Doc()
            |> Content.Page
        let REDIRECT_TO (url: string) =
            RedirectTemplate()
                .Url(url)
                .Doc()
            |> Content.Page
        Application.MultiPage (fun (ctx: Context<_>) -> function
            | Trainings ->
                TRAININGS ()
            | TermsOfUse ->
                TERMSOFUSE ctx
            | CookiePolicy ->
                COOKIEPOLICY ctx
            | PrivacyPolicy ->
                PRIVACYPOLICY ctx
            // The main blogs page
            | Blogs (BlogListingArgs.Empty) ->
                REDIRECT_TO "/blogs/1"
            | Contact ->
                CONTACT ()
            | Blogs (BlogListingArgs.Index ind) ->
                let pageNo =
                    if ind < 1 then 1 else ind
                BLOG_LISTING
                    <| BlogListTemplate.BlogListBanner()
                        .Title(config.Value.Title)
                        .Subtitle(config.Value.Description)
                        .Doc()
                    <| pageNo
                    <| fun _ article -> true
            | Blogs (BlogListingArgs.LanguageAndIndex (lang, ind)) ->
                let langopt, pageNo =
                    lang, if ind < 1 then 1 else ind
                BLOG_LISTING
                    <| BlogListTemplate.BlogListBanner()
                        .Title(config.Value.Title)
                        .Subtitle(config.Value.Description)
                        .Doc()
                    <| pageNo
                    <| fun _ article ->
                        langopt = URL_LANG config.Value article.Language
            | Article p ->
                ARTICLE ("", p)
            // All articles by a given user
            | UserArticle (user, "") ->
                BLOG_LISTING_NO_PAGING
                    <| BlogListTemplate.BlogCategoryBanner()
                        .Category(user)
                        .Doc()
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
            // Blog articles in a given category
            | Category (cat, langopt) ->
                BLOG_LISTING_NO_PAGING
                    <| BlogListTemplate.BlogCategoryBanner()
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
            | Error404 ->
                Content.File("../Hosted/404.html", AllowOutsideRootFolder=true)
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
            let noPagesForLanguage language =
                let noArticlesInLanguage =
                    articles
                    |> List.map snd
                    |> List.filter (fun article -> language = Site.URL_LANG config.Value article.Language)
                    |> List.length
                noArticlesInLanguage / config.Value.PageSize + 1
            let users =
                articles
                |> List.map (fst >> fst)
                |> Set.ofList
                |> Set.toList
            eprintfn "DEBUG-users: %A" users
            [
                // Generate the learning page
                Trainings
                // Generate contact page
                Contact
                // Generate the main blog page (a redirect)
                Blogs (BlogListingArgs.Empty)
                // Generate the blog home page(s), one per language
                for language in languages do
                    for page in [1 .. noPagesForLanguage language] do
                        Blogs (BlogListingArgs.LanguageAndIndex (language, page))
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
                // Generate 404 page
                Error404
                // Generate legal pages
                CookiePolicy
                TermsOfUse
                PrivacyPolicy
            ]

[<assembly: Website(typeof<Website>)>]
do ()
