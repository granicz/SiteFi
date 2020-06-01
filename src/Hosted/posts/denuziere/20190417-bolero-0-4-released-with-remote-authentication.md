---
title: "Bolero 0.4 released with remote authentication"
categories: "blazor,bolero,webassembly,fsharp,websharper"
abstract: "Authenticate users and authorize remote functions with standard ASP.NET Core auth."
identity: "5761,86650"
---
We are happy to announce the release of [Bolero](https://fsbolero.io) version 0.4. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using Blazor.

Install the latest project template with:

```
dotnet new -i Bolero.Templates
```

## Remote authentication

Version 0.4 brings user authentication to remote functions. Authentication is done through standard ASP.NET Core APIs, with a few F#-friendly methods on `HttpContext` for convenience.

```fsharp
/// A simple service that performs signin / signout.
type MyRemoteService =
    {
        signIn : string -> Async<unit>
        signOut : unit -> Async<unit>
    }

let myRemoteService =
    {
        signIn = Remote.withContext <| fun http username -> async {
            return! http.AsyncSignIn(username)
        }
        signOut = Remote.withContext <| fun http () -> async {
            return! http.AsyncSignOut()
        }
    }
```

Learn more about authentication in [the documentation](https://fsbolero.io/docs/Remoting#authentication-and-authorization).

## Remote authorization

You can also mark a remote function as requiring authentication with `Remote.authorize`, and provide additional authorization policies with `Remote.authorizeWith`.

```fsharp
type MyPrivateService =
    {
        getSecretData : unit -> Async<string>
    }
    
let myPrivateService =
    {
        getSecretData = Remote.authorize <| fun http () -> async {
            return "Secret user data!"
        }
    }
```

On the client side, authorization failure can be easily detected by the Elmish update function.

```fsharp
type Message =
    | GetSecretData
    | GotSecretData of RemoteResponse<string>
    | Error of exn
    
let update service message model =
    match message with
    | GetSecretData ->
        // Call the remote service
        model, Cmd.ofRemote service.getSecretData () GotSecretData Error
    | GotSecretData (Success data) ->
        // The call was successful
        { model with secretData = data }, Cmd.none
    | GotSecretData Unauthorized ->
        // The user wasn't logged in
        { model with showLoginPopup = true }, Cmd.none
    | Error exn ->
        // Another error happened (eg. the server was unavailable)
        { model with latestError = Some exn }, Cmd.none
```

## Full changelog

### Breaking changes

* NuGet package reorganization: the package `Bolero.HotReload.Server` is renamed to `Bolero.Server` and now contains all server-side facilities. It is necessary to reference it in the server-side project to use remoting.

* [#22](https://github.com/fsbolero/bolero/issues/22) Move the server-side and client-side `AddRemoting` extension methods to namespaces `Bolero.Remoting.Server` and `Bolero.Remoting.Client`, respectively.

### Features

* [#24](https://github.com/fsbolero/bolero/issues/24) Add remoting authentication.

    * Add F#-friendly extension methods on `HttpContext` for authentication:

        ```fsharp
        member AsyncSignIn : name: string
                           * ?persistsFor: TimeSpan
                           * ?claims: seq<Claim>
                           * ?properties: AuthenticationProperties
                           * ?authenticationType: string
                          -> Async<unit>

        member AsyncSignOut : ?properties: AuthenticationProperties
                           -> Async<unit>

        member TryUsername : unit -> option<string>

        member TryIdentity : unit -> option<Identity>
        ```

    * Add module `Bolero.Remoting.Server.Remote` with remote function wrappers to get access to the `HttpContext` and mark a function as requiring authorization:

        ```fsharp
        module Remote =
            val withContext : (HttpContext -> 'req -> Async<'resp>)
                           -> ('req -> Async<'resp>)

            val authorize : (HttpContext -> 'req -> Async<'resp>)
                         -> ('req -> Async<'resp>)

            val authorizeWith : seq<IAuthorizeData>
                             -> (HttpContext -> 'req -> Async<'resp>)
                             -> ('req -> Async<'resp>)
        ```

* [#28](https://github.com/fsbolero/bolero/issues/28) Templating: Holes that fill an HTML attribute's value (eg: `some-attr="${SomeHole}"`) now have type `obj` rather than `string`. This is useful in particular to fill attributes such as `disabled` with a boolean.

### Bug fixes

* [#29](https://github.com/fsbolero/bolero/issues/29) Templating: fix exception when using the bound value of a number input as a text hole.

Happy coding!