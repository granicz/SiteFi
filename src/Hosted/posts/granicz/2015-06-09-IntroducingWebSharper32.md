---
title: Introducing WebSharper 3.2
categories: websharper
abstract: We are thrilled to announce the availability of WebSharper 3.2, paving the road to further upcoming enhancements to streamline developing and deploying WebSharper apps, and also shipping several key changes summarized here.
---

We are thrilled to announce the availability of WebSharper 3.2, paving the road to further upcoming enhancements to streamline developing and deploying WebSharper apps, and also shipping several key changes summarized here.

### No need to annotate sitelet assemblies with `Website`

This is what pre-3.2 code looked like:

```fsharp
module Site =
    ...
    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
            Sitelet.Content "/About" About AboutPage
        ]

[<Sealed>]
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Site.Main
        member this.Actions = []

[<assembly: Website(typeof<Website>)>]
do ()
```

Now you can simply do:

```fsharp
module Site =
    ...
    [<Website>]
    let Main =
        Sitelet.Sum [
            Sitelet.Content "/" Home HomePage
            Sitelet.Content "/About" About AboutPage
        ]
```

Old code works as before, but we now look for the `Website` attribute on values as well if no assembly-level instance is found, yielding the shorter syntax above.

### Dot-syntax for chained event handlers

The following code:

```fsharp
Button [Text "some text"]
|>! OnClick (fun e args ->
    JS.Alert "Clicked"
)
```

can now be written as:

```fsharp
Button([Text "some text"])
    .OnClick(fun e args ->
        JS.Alert "Clicked"
    )
```
This syntax is more familiar to many developers, eliminates the need for a special operator (`|>!`), and makes code more discoverable by having API comments and code completion choices available when attaching the event handler.

### Server-side templating enhancements

Traditionally, in sitelet templates you had the following line in the `<HEAD>` section to stand for the placeholder for including generated page dependencies (e.g. all the CSS, JS, etc. files that are implicitly referenced in your page):

    <meta name="generator" content="websharper" data-replace="scripts" />

With 3.2, you can now refine how these dependencies are inserted if you provide additional placeholders:

1. `styles`: output the generated stylesheets only.  Usually, you will want to put this placeholder in the `<HEAD>` section.
2. `meta`: output the client-server integration (arguments to server-side controls, etc.) bits only.  You will want this in `<HEAD>`

When either of these are present, `scripts` only outputs the JavaScript dependencies, making it possible to place this placeholder anywhere else, away from the other bits, say, to the tail of the `<BODY>` element.

A typical new template might thus be:

```xml
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta data-replace="meta" />
    <meta data-replace="styles" />
    ...
</head>
<body>
  ...
  <script data-replace="scripts"></script>
</body>
</html>
```

## Change log

The full list of items completed since the last `3.1.6` release:

 * [#410](https://github.com/intellifactory/websharper/issues/410): add `FuncWithOnlyThis` type, compile error on invalid use of `FuncWithThis`
 * [#394](https://github.com/intellifactory/websharper/issues/394): `JQuery.Add(JQuery)` binding missing
 * [#411](https://github.com/intellifactory/websharper/issues/411): ref cells are broken
 * [#412](https://github.com/intellifactory/websharper/issues/412): Sequence range throws an exception when end < start
 * [#413](https://github.com/intellifactory/websharper/issues/413): Range Expressions Are Not Properly Compiled
 * [#415](https://github.com/intellifactory/websharper/issues/415): Missing proxy for `Seq.last`
 * [#417](https://github.com/intellifactory/websharper/issues/417): `ClientSide` doesn't incur a dependency on the declaring type of the invoked method
 * [#418](https://github.com/intellifactory/websharper/issues/418): Json: record in single union case field doesn't respect `[<Name>]` for its fields
 * [#421](https://github.com/intellifactory/websharper/issues/421): Sitelet templates: intermittent `IOException` on changed file
 * [#419](https://github.com/intellifactory/websharper/issues/419): Allow splitting generated styles and scripts via separate placeholders
 * [#422](https://github.com/intellifactory/websharper/issues/422): Allow defining a sitelet assembly by just putting `[<Website>]` on a `Sitelet<'T>` value
 * [#423](https://github.com/intellifactory/websharper/issues/423): `Html.Client`: Add chainable extension methods for event handlers
 * [#425](https://github.com/intellifactory/websharper/issues/425): `Seq.distinct`/`distinctBy` proxy only compares hashes
 * [#426](https://github.com/intellifactory/websharper/issues/426): Create a `WebSharper.Compiler` nuget package 
 * [#414](https://github.com/intellifactory/websharper/issues/414): `sprintf` Is Not Properly Compiled
 * [#429](https://github.com/intellifactory/websharper/issues/429): Add `WebSharperProject`=`Ignore` build task option
 * [#428](https://github.com/intellifactory/websharper/issues/428): `Sitelet.Infer`: add `EndPointAttribute` as synonym for `CompiledNameAttribute`
 * [#427](https://github.com/intellifactory/websharper/issues/427): `Sitelet.Infer`: allow prefixing union case's `CompiledName` with `/`

Just a note: CloudSharper out-of-the-box templates still use 3.1 - we will be migrating these to 3.2 in the following days, keep an eye on our blog for when this is finalized.

Happy coding!
