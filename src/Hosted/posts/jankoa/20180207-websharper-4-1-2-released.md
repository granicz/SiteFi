---
title: "WebSharper 4.1.2 released"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.1.2 has enhancements for C# routing and bug fixes"
identity: "5495,84392"
---
WebSharper 4.1.2 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](http://websharper.com/downloads).

It contains enhancements to the `EndPoint` attribute and C# routing as well as fixes for JSON serialization and remoting of F# `Map` and `Set` values.

Documentation: [WebSharper 4.1 for C#](http://developers-test.websharper.io/docs/v4.1/cs) and [WebSharper 4.1 for F#](http://developers-test.websharper.io/docs/v4.1/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/websharper/releases/tag/4.1.2.178).

# New features
* WebSharper for C# now supports syntax introduced in C# 7.2: `in` arguments and `ref readonly` returns, non-trailing named arguments, numeric literals with leading underscores, `private protected`.

# Fixes/improvements
* WebSharper build errors and warnings in F# projects now show up correctly in Errors window and as squiggly underlines.
* Fixed deserializing an empty `Set` value sent to a `Remote` function.
* `Map` and `Set` values with any key type can now be serialized into JSON with `Json.Serialize`. Maps are stored as a flat list of key/value pairs. `Map<string>` still serializes to a single JSON object.
* C# inferred routers now respect HTTP methods same as F#: declared either by the `Method` attribute or like `EndPoint("POST /post")`
* Multiple `EndPoint` attributes can be used on types and members. Equivalently, the `EndPoint` attribute can be passed multiple string arguments. In the case of multiple attributes, they can be ordered by setting the `order` parameter of the `EndPoint` attibute contructor.
* C# subclasses used in inferred routers have the option to declare the full route to parse/write, not inheriting from base class by having `inheritRoute: false` on an `EndPoint` attribute,
* `EndPoint "/"` can be used on a union case with fields (for example another union, allowing breaking up a big endpoint definition into multiple union types)
* Decoding/encoding string values by `Router.Infer` (and `Router.rString`) are now using the same logic as WebSharper 4.0: any non-alphanumeric character replaced to `~xx` or `~uxxxx`. This ensures that string values are recovered properly and are passing standard URL correctness checks.

# Example for C# routing

A C# class hierarchy can define an inferred router:

```csharp
    [JavaScript, EndPoint("/", "/home")]
    public class Home {
        [EndPoint("/article/{Id}")]
        public class Article : Home
        {
            public int Id;
            [EndPoint("/page/{Page}")]
            [EndPoint("/article-page/{Id}/{Page}", order: 1, inheritRoute: false)]
            public class Page : Article
            {
                public int Page;
            }
        }

        public static Router<Home> Router = InferRouter.Router.Infer<Home>();
    }
```

Then `Home.Router` will be able to parse these routes:

```
    /
    /home
        => new Home()
    /article/1
    /home/article/1
        => new Home.Article { Id = 1 }
    /article/1/page/2
    /home/article/1/page/2
    /article-page/1/2 (because inheritRoute is false, it is re-defining whole route)
    	=> new Home.Article.Page { Id = 1, Page = 2 }
```

The first of these paths is the canonical form that will be used for writing links.

See the details of how to use a router to serve a site and create links [here](https://developers.websharper.com/docs/v4.1/cs/sitelets) under "Using the router".