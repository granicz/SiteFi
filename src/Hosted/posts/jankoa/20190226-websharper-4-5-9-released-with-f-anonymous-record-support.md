---
title: "WebSharper 4.5.9 released with F# anonymous record support"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.5.9 has client-side support for F# anonymous records and build fixes."
identity: "5730,86510"
---
WebSharper 4.5.9 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](https://websharper.com/downloads).

It contains client-side support for F# anonymous records, and a minor new UI templating feature.

Documentation: [WebSharper 4.x for C#](https://developers.websharper.com/docs/v4.x/cs) and [WebSharper 4.x for F#](https://developers.websharper.com/docs/v4.x/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/core/releases/tag/4.5.9.330).

See associated WebSharper.UI release notes [here](https://github.com/dotnet-websharper/ui/releases/tag/4.5.8.161).

# Fixes
* Assembly version mismatch in package WebSharper.FSharp, causing build fauilures.

# Enhancements
* Added client-side support for anonymous F# records.
* UI Templating: add a new attribute `ws-preserve.` When an element is marked with this attribute, it and its children are not parsed for holes and `ws-*` attributes.

## Anonymous records

F# anonymous records are now fully usable in client-side code. They are translated to plain JS objects:

```fsharp
let anon1 = {| A = 1 |} // translates to { A: 1 }
```

This gives a nice new alternative of constructing plain JS objects instead of `New [ "A" => 1 ]` or `JS.Inline("{A: $0}", 1)` or using a record/class type annotated with `Prototype(false)`.

`option<_>` and `voption<_>` valued fields on anonymous records are automatically transformed to and from existing/missing JS properties:

```fsharp
let anon3 = {| A = Some 1; B = None |} // translates to { A: 1 }
```

This helps storing the values in minimal form, while keeping the full type safety of F# code.

See it live [here](http://try.websharper.com/snippet/JankoA/0000NK):

[![Imgur](https://i.imgur.com/G0cDYLu.jpg)](http://try.websharper.com/snippet/JankoA/0000NK)

## Opt out of templating transformations

By default a `${Name}` in a WebSharper.UI template file is treated as a hole to be filled. Including this verbatim is now made easier with new attribute `ws-preserve` that excludes children of current node from being interpreted for template holes and `ws-` attributes and nodes.

For example:

```html
<div>
    ${ThisIsAHole}
    ${ThisToo}
    <div ws-preserve>${ThisIsNotAHole}</div>
</div>
```

filled with:

```fsharp
MyTemplate().ThisIsAHole("foo").Doc()
```

gives:

```html
<div>
    foo
    
    <div>${ThisIsNotAHole}</div>
</div>
```

**Happy coding!**
