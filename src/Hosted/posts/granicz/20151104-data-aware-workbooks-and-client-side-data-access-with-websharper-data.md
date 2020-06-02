---
title: "Data-aware workbooks and client-side data access with WebSharper.Data"
categories: "data,fsharp.data,fsharp,websharper"
abstract: "F# has always excelled at accessing heterogeneous data sources in server-side code through its unique type provider feature: a metaprogramming technique that enables generating (or \"providing\") domain-specific code to be consumed during compilation, such as generating typed schemas for relational databases, CSV and other data files, or bindings for web services and integration with other languages such as R. Type providers are given an optional set of arguments in your code using custom F# syntax, yielding a type space in return."
identity: "4623,80603"
---
F# has always excelled at accessing heterogeneous data sources in server-side code through its unique [type provider](https://msdn.microsoft.com/en-us/library/hh156509.aspx) feature: a metaprogramming technique that enables generating (or "providing") domain-specific code to be consumed during compilation, such as generating typed schemas for relational databases, CSV and other data files, or bindings for web services and integration with other languages such as R. Type providers are given an optional set of arguments in your code using custom F# syntax, yielding a type space in return.

For instance, [FSharp.Data](http://fsharp.github.io/FSharp.Data) provides access to various forms of structured data (XML, CSV, etc.) and the WorldBank database via its web API. You can now access some of this data, from client-side code in your WebSharper applications with [WebSharper.Data](http://github.com/IntelliFactory/websharper.data).

### WebSharper.Data

WebSharper.Data provides various proxies for FSharp.Data, currently supporting the `JsonProvider` and the `WorldBankProvider` runtimes in client-side use with WebSharper.

Below is a snippet that uses WebSharper.Data and WebSharper.Charting to provide a workbook with data obtained from the WorldBank database.

[![](http://i.imgur.com/ljGHAeCl.png)](http://try.websharper.com/snippet/adam.granicz/00003p)

A wide array of other data-aware scenarios are possible, below is another simple snippet that works out of the GitHub issue database and displays a list of open tickets for the [WebSharper repository](http://github.com/IntelliFactory/websharper).

[![](http://i.imgur.com/6xqISAOl.png)](http://try.websharper.com/snippet/qwe2/00003t)

Happy coding!
