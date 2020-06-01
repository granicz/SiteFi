---
title: "Upcoming in WebSharper 3.0: serving REST APIs, easy as pie!"
categories: "fsharp,websharper"
abstract: "WebSharper 3.0 is coming with an awesome new feature: automatically parse JSON requests into typed F# values, and reply with JSON just as easily!"
identity: "4231,77707"
---
WebSharper Sitelets are a wonderful way to build websites quickly and safely.
They can automatically manage your URLs in a type-safe way, and generate HTML markup
using simple F# combinators. In WebSharper 3.0, we are extending Sitelets with the capability
to create REST APIs with unrivaled simplicity.

### Requests

Just like you can currently type these lines in F#:

```fsharp
type Action =
	| [<CompiledName "listing">] Listing of pagenum: int
    | [<CompiledName "article">] Article of id: int * slug: string
```

to tell WebSharper that your site will be served on these URLs:

```
/listing/1
/article/135/upcoming-in-websharper-30
```

You can now also specify all the information necessary to serve a web API on a given HTTP method and
with the given JSON body, simply by using a couple attributes:

```fsharp
type Action =
	| [<Method "GET"; CompiledName "article">]
    	GetArticle of id: int
    | [<Method "POST"; CompiledName "article"; Json "data">]
    	PostArticle of data: ArticleData

and ArticleData =
	{
    	author: string
        title: string
        tags: Set<string>
        summary: option<string>
        body: string
    }
```

The following requests are now accepted:

```fsharp
GET /article/135
```
####
```fsharp
POST /article

{
  "author": "loic.denuziere",
  "title": "Upcoming in WebSharper 3.0: serving REST APIs, easy as pie!",
  "tags": ["websharper", "fsharp"],
  "summary": "WebSharper 3.0 is coming with an (...)",
  "body": "WebSharper Sitelets are a wonderful way to (...)"
}
```

The JSON serialization provides all the niceties possible to be friendly with F# types:

* F# records are represented as JSON objects;
* `list<'T>`, `'T[]` and `Set<'T>` are representad as JSON arrays;
* `Map<string, 'T>` is represented as a flat JSON object;
* F# fields of type `option<'T>` are represented as a JSON field that is present if Some or absent if None;
* F# unions are represented as JSON objects using the union field names,
	and a separate named field to indicate the union case (the name of this field is specified in an attribute).
    
### Responses

Of course, a REST API is not just parsing requests, but also writing responses. For this too,
WebSharper 3.0 has you covered. A new function `Content.JsonContent` allows you to serve any F# value
as JSON with zero hassle:

```fsharp
let mySite = Sitelet.Infer <| function
	| GetArticle id ->
    	Content.JsonContent <| fun ctx ->
        	{ author = "loic.denuziere"; (* ... *) }
    | PostArticle articleData ->
    	Content.JsonContent <| fun ctx ->
        	SaveArticle articleData
```

Want to see a full example? How about a full CRUD API serving an in-memory database of people information,
with all interactions perfectly type-safe,
[in less than fifty lines](https://github.com/intellifactory/websharper/blob/master/tests/WebSharper.Sitelets.Tests/Api.fs)?

Look for this new WebSharper 3.0 pre-release on NuGet early next week, or build it right now [from source](https://github.com/intellifactory/websharper)!