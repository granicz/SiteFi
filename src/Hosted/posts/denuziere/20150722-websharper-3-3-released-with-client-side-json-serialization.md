---
title: "WebSharper 3.3 released with client-side JSON serialization"
categories: "json,fsharp,javascript,web,websharper"
abstract: "We just released WebSharper 3.3 with a new capability: Sitelets-compatible JSON serialization and deserialization on the client."
identity: "4419,79787"
---
We are happy to announce the availability of WebSharper 3.3, which you can download [here](http://websharper.com/downloads). The main highlight of this release is the addition of JSON serialization functions for client-side code.

The format used for this serialization is identical to [the format used by inferred Sitelets](http://websharper.com/docs/json). This means that you can now easily craft your request data on the client, perform an AJAX call to your Sitelets API, and parse the reply, all of this type-safely!

For example, for a website defined as follows:

```fsharp
open WebSharper
open WebSharper.Sitelets

type EndPoints =
    | [<EndPoint "POST /article"; Json "article">]
      PostArticle of article: Article
and Article = { title: string; body: string }

[<Website>]
let app = Sitelet.Infer <| function
    | PostArticle article ->
        let articleId = ApplicationLogic.SaveArticle article
        Content.JsonContent (fun _ -> articleId)
```

You can invoke the REST endpoint `POST /article` from the client-side like this:

```fsharp
[<JavaScript>]
module Client =
    open WebSharper.JavaScript
    open WebSharper.JQuery

    /// General function to send an AJAX request with a body.
    let Ajax (method: string) (url: string) (serializedData: string) : Async<string> =
        Async.FromContinuations <| fun (ok, ko, _) ->
            JQuery.Ajax(
                JQuery.AjaxSettings(
                    Url = url,
                    Type = As<JQuery.RequestType> method,
                    ContentType = "application/json",
                    DataType = JQuery.DataType.Text,
                    Data = serializedData,
                    Success = (fun (result, _, _) -> ok (result :?> string)),
                    Error = (fun (jqXHR, _, _) -> ko (System.Exception(jqXHR.ResponseText)))))
            |> ignore

    /// Use Json.Serialize and Deserialize to send and receive data to and from the server.
    let PostBlogArticle (article: Article) : Async<int> =
        async { let! response = Ajax "POST" "/article" (Json.Serialize article)
                return Json.Deserialize<int> response }
```

This new API is located in the module `WebSharper.Json`. Its only limitation compared with Sitelets is that the `[<DateTimeFormat>]` attribute is currently ignored, as JavaScript doesn't have built-in datetime formatting capabilities. We might consider using an external library such as [moment.js](http://momentjs.com/) for this purpose in the future.

#### Future plans

We have lots of exciting things to come in WebSharper. Here is what you can expect from upcoming releases:

* F# 4.0 support, including JavaScript proxies for the [new collection library functions](https://github.com/fsharp/FSharpLangDesign/blob/master/FSharp-4.0/ListSeqArrayAdditions.md).
* A new, cleaner HTML combinator syntax. This syntax will supersede the current Html.Server, Html.Client and UI.Next.HTML with a unique type. This means that the same HTML code will be usable both from the server and client side.

Happy coding!