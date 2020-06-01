---
title: "WebSharper-3.0.36-alpha released"
categories: "javascript,f#,websharper"
abstract: "This is our release candidate API-wise, last batch of breaking changes."
identity: "4241,77739"
---
This release brings the last batch of breaking changes we have planned before 3.0 stable. 
The three bigger items we have left are updating/fixing TypeScript output, reviewing code mapping and adding more documentation.

## Namespace changes

We have shortened namespace names by changing every occurence of `IntelliFactory.WebSharper` to `WebSharper`. This affects base package and all extensions. Also `IntelliFactory.JavaScript` is renamed to `WebSharper.Core.JavaScript`. DLLs are similarly renamed with the exception of `IntelliFactory.WebSharper.dll` which is now `WebSharper.Main.dll`.

## WebSharper.Formlets in a separate package

The Formlets library is now contained in the NuGet package `WebSharper.Formlets`.
Its dependency, `IntelliFactory.Reactive` is in another new package `IntelliFactory.Reactive`.

## JSON Sitelets

You can now create REST APIs easily by using F# types. See [previous announcement](websharper.com/blog-entry/4231).

## User Session in Sitelets and Rpc functions

Up until now, the module `WebSharper.Sitelets.UserSession` could be used to manage user login.
Unfortunately, functions in this module have two major drawbacks:

* They use ASP.NET-specific functionality, and therefore cannot be used with other hosts such as self-hosted OWIN.
* They assume that they run on the same thread that is responding to the request. This assumption is unreasonable,
    especially when using functions such as `Content.PageContentAsync`.

For these reasons, the following changes are made:

* The module `WebSharper.Sitelets.UserSession` is marked as obsolete.
* The type `Sitelets.Context<'T>` now contains a field `UserSession`, which can be safely used in asynchronous sitelets.
    Note that unlike the old `UserSession`, its methods are asynchronous.
* The new function `WebSharper.Web.Remoting.GetContext()` can be used in Rpc functions to retrieve a thread-safe user session manager.
    It *must* be called from the Rpc function's thread, but the resulting object can be used asynchronously. For example:

```fsharp
[<Rpc>]
let DoSomethingIfLoggedIn() =
    let ctx = Web.Remoting.GetContext()
    async {
        let! loggedIn = ctx.UserSession.GetLoggedInUser()
        match loggedIn with
        | None -> return "Error: not logged in"
        | Some username -> return! DoSomething username
    }
```

* This functionality is implemented both in the built-in ASP.NET module and in WebSharper.Owin.

## JavaScript interop

* Previous `.ToEcma()` methods for converting a .NET type to its EcmaScript binding equivalent has been obsoleted. Renamed to `.JS` property.
* Previous `.ToDotNet()` methods are now a `.Self` property on `WebShaper.JavaScript.` types.
* `JavaScript.Function` is now a base class for all `JavaScript.FuncWith...` types (giving strong typing to JavaScript functions).
* Methods on `JavaScript.Function` has been renamed to `ApplyUnsafe`, `CallUnsafe`, `BindUnsafe` to avoid overloading with the strongly-typed methods on descendant classes.

## WebSharper Interface Generator

* The generic helper for more than 4 type parameter is now `GenericN n - fun typeParamList -> ...`
* `Generic %` can be used to add the same type parameters to a list of members.
* `Generic *` can be used to add the same type parameters to a to a `ClassMembers` value (created by `Instance [...]` or `Static [...]`).
* These generic helpers all add type parameters in addition to previously defined ones on the members returned by the inner function,
instead of overwriting the generics list.
* Type parameters are automatically renamed on library generation to avoid name clashes.

## Other improvements

* A compile warning is given when a tupled or curried lambda or variable with the same types are coerced to `obj`.
    This can help avoiding some mistakes, for example writing

        New [ "compare" => fun (a, b) -> a - b ] 

    and passing it to JavaScript code expecting a function that takes two arguments.
    However, this is translated to a JavaScript function taking a single 2-element array argument.
    The former is the much more common scenario, so on this warning, change it to

        New [ "compare" => FuncWithArgs(fun (a, b) -> a - b) ] 

    to pass the right function.
    Otherwise, you can use the `Function.As` static method to wrap the lambda and and get rid of the warning.
* `[<OptionalField>]` attribute now can be used on class or record types.
This makes all option valued fields of the type represented as present/missing JavaScript properties in the translation.

## Fixes

* [#340](https://github.com/intellifactory/websharper/issues/340): Generic methods of a generic type used wrong type parameters for generating the member. This could have affected some of our extensions.