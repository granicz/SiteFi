---
title: "WebSharper 4.1.1 released"
categories: "c#,javascript,f#,websharper"
abstract: "WebSharper 4.1.1 expands and fixes multiple client-server features"
identity: "5491,84324"
---
WebSharper 4.1.1 is now available on [NuGet](https://www.nuget.org/packages/websharper), and as a vsix installer on the [WebSharper website](http://websharper.com/downloads).

It contains enhancements and fixes to some client-server functionality introduced in WebSharper 4.1 (`on.*` server-side event handlers pre-compiling any form of quotation), special treatment of `IsClient` value for conditional compilation for server/client, and other bug fixes.

Documentation: [WebSharper 4.1 for C#](http://developers-test.websharper.io/docs/v4.1/cs) and [WebSharper 4.1 for F#](http://developers-test.websharper.io/docs/v4.1/fs).

The release notes are also found on [GitHub](https://github.com/dotnet-websharper/websharper/releases/tag/4.1.1.175).

# New features
* Server-side event handlers (`on.*`) and `ClientSide` can take any fixed F# quotation now and translates it to JavaScript.

```fsharp
div [] [ text "div created on the server" ]
client 
    <@ JavaScript.Console.Log "running client-side code"
       div [] [ text "div created on the client" ]
    @>
```

* The `IsClient` value (`WebSharper.Pervasives.IsClient` from C#) and their negations (`not IsClient` and `!Pervasives.IsClient`) can be used as a condition in branching logic in shared client-server code to make the JavaScript compiler skip the false branch entirely.

```csharp
    // using WebSharper;
    if (Pervasives.IsClient) { ClientSideMethod(); } else { ServerSideMethod(); }
    var res = Pervasives.IsClient ? ClientSideMethod() : ServerSideMethod();
```
```fsharp
    if IsClient then ClientSideMethod() else ServerSideMethod()
```

# New features in WebSharper.UI

* Added `Lens` function to lens into a `Var` using the `.V` syntax. Example usage:

```fsharp
Lens(myvar.V.MyField)
// is equivalent to:
myVar.LensAuto(fun x -> x.MyField)
```

    This returns creates a `Var<T>` where `T` is the type of `MyField` record field, which gets and sets its value by updating the underlying `myVar`.
    
* Add `Var<list<T>>.MapLens` and `.DocLens`, similar in functionality to their `ListModel` counterpart, although with linear-time complexity. Example:

```fsharp
type Person = { Id: int; Name: string; Age: int }

let people =
    Var.Create (fun p -> p.Id) [
        { Id = 0; Name = "Ann"; Age = 34 }
        { Id = 1; Name = "Brian"; Age = 28 }
        { Id = 2; Name = "Clara"; Age = 43 }
    ]

let editPeople =
    people.DocLens(fun k vp ->
        form [] [
            Doc.Input [] (Lens vp.V.Name)
            Doc.IntInputUnchecked [] (Lens vp.V.Age)
        ]
    )
```

   This ties the inputs to the values in `people.Value` (of type `list<Person>`), recreating the immutable list if any of the inputs are changed.

# Fixes and improvements
* An empty F# `Map` passed to an RPC function is deserialized correctly.
* The behavior of the `Stub` attribute on constructors and static methods are now consistent with the [documented translation logic](https://developers.websharper.com/docs/v3.x/fs/attributes) since WebSharper 3.0.
* Multiple server-side event handlers and `ClientSide` having captured arguments within the same method do not interfere with each other.
* The `client` helper in `WebSharper.UI` is now available again within client-side code. So a `div [] [ client <@ div [] [] @> ]` in server side-code will create the internal `div` on the client (the server returning a placeholder) while fully running on the it just creates the internal `div` basically as if `client <@ @>` wrapper was not there.
* Server-side event handlers and `ClientSide` do not create extra functions in translated JavaScript code if their expression is containing a single static method call. These functions in back-compatibility package `WebSharper.UI.Next` work the same as in WebSharper 4.0.
* Some quotation forms were missing when exploring for captured arguments inside server-side `on.*` event handlers, including tuple gets (used by F# implicitly when passing captured a tuple value to a client-side function taking a tuple, resulting in a runtime failure). This is now working correctly:

```fsharp
let t = 1, 2
div [] [
    button [ on.click (fun _ _ -> Client.TestTuple t) ] [ text "click me" ] 
]
```
