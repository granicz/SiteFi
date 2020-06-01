---
title: "Password-less authentication with WebSharper.OAuth"
categories: "authentication,oauth,websharper"
abstract: "This tutorial shows how to authenticate a WebSharper Client-Server Application with an OAuth2 service and query its API."
identity: "5492,84332"
---
It used to be that authenticating users in a properly secure way was a big hassle; but with protocols like OAuth and OpenID, things are now much simpler. We can delegate most of the trouble to services such as Facebook, Google, Twitter, Github, etc and simply interface with them.

Today I will show how to use WebSharper.OAuth to authenticate with an OAuth2 provider and and query this provider's API. I will use Github, but any OAuth2 provider will work similarly.

Let's start by creating a simple client-server website application. In Visual Studio, select "WebSharper 4 Client-Server Application". Then, add the `WebSharper.OAuth` NuGet package to the project. Once this is done, we are ready to code.

This application consists of two pages, Home and About. We will make the About page only accessible to authenticated users; unauthorized users will see a link to log in with Github instead.

The OAuth2 authorization works as follows:

- The user clicks a link from your site to the service provider (in our case, Github).
- The service provider's website prompts the user to accept or refuse to give your website access to their data.
- They are redirected back to a special page of your website, called the *redirect endpoint*, where you can examine their answer.
- If they accepted, this answer includes an *authentication token* which you can use to request the service provider's API for user data. In this example, we will simply request their Github username.

There are some additional requests and checks between your website and the service provider, but you don't need to worry about these; they are handled by WebSharper.OAuth.

First, we need to add the redirect endpoint to our endpoint type; let's set its URL to `/oauth-github`.

```
type EndPoint =
    | [<EndPoint "/">] Home
    | [<EndPoint "/about">] About
    | [<EndPoint "/oauth-github">] OAuthGitHub
```

In WebSharper.OAuth, the authorization process is handled by a value of type `OAuth2.Provider`. Let's create a module just below the `EndPoint` to contain it:

```
module Authentication =
    open System.IO
    open WebSharper.OAuth

    /// The OAuth2 authorization provider for GitHub.
    let Provider =
```

The provider is created using the static method `OAuth2.Provider.Setup`. It takes three mandatory arguments:

* `service` gives information about the service provider you want to connect to: the standard URLs to which requests are made throughout the process, and the credentials of your application with this service.  
    WebSharper.OAuth knows these standard URLs for a few common services, and Github is one of those, so you just need to provide your app's credentials: `OAuth2.ServiceSettings.Github("APP_ID", "APP_SECRET")`.  
    To obtain these credentials, you need to register your application with Github. At [this page](https://github.com/settings/developers), click "New OAuth App". You can enter whatever you want as Application name, Homepage URL and Application description. Authorization callback URL, however, must correspond to your redirect endpoint. For our local testing, this will be something like `http://localhost:56385/oauth-github` (check the actual port in your URL bar!), but once you deploy it to a live server, you will need to change the base URL accordingly.

* `redirectEndpointAction` is the sitelet endpoint value for the redirect endpoint: `EndPoint.OAuthGitHub`.

* `redirectEndpoint` creates your actual response when the user is redirected to this redirect endpoint. It is similar to the function passed to `Application.MultiPage`, except instead of receiving an endpoint value, you receive the user's response to the authorization prompt.

    * If they said yes, then this response is `OAuth2.Success token`. We can use this token to retrieve the username, log in the user, and redirect them to the protected page they were requesting.
    
    * If they said no, then we simply redirect them to the home page.

```
    /// The OAuth2 authorization provider for GitHub.
    let Provider =
        OAuth2.Provider.Setup(
            service = OAuth2.ServiceSettings.Github("APP_ID", "APP_SECRET"),
            redirectEndpointAction = EndPoint.OAuthGitHub,
            redirectEndpoint = (fun ctx response ->
                async {
                    match response with
                    | OAuth2.Success token ->
                        let! userData = getUserData token
                        // All good! The user is authenticated.
                        do! ctx.UserSession.LoginUser(userData.login)
                        return! Content.RedirectTemporary EndPoint.About
                    | _ ->
                        // This is "normal" failure: the user simply rejected the authorization.
                        do! ctx.UserSession.Logout()
                        return! Content.RedirectTemporary EndPoint.Home
                }
            )
        )
```

I won't go over all the optional arguments, but one of them is particularly useful: `defaultScope`. Many providers require that you provide a *scope* for authentication requests, indicating what API data the user will give your application access to. `defaultScope` defines the scope that will be used for all authentication requests, unless overridden in `GetAuthorizationRequestUrl` (see below).

Now, we haven't yet implemented `getUserData`. This function needs to request Github's API using the token and parse the response for the username.

Requesting the service's API is done using a standard `System.Net.HttpWebRequest`, to which we add the necessary authorization headers using `token.AuthorizeRequest`.

Github user information is retrieved from `https://api.github.com/user`. This endpoint returns [a JSON object containing a lot of fields](https://developer.github.com/v3/users/#get-the-authenticated-user). Right now we only care about one: `login`. We can create a record with just this field and use `WebSharper.Json` to parse the response.

```
    type GithubUserData = { login: string }

    /// Get basic user data from GitHub.
    let getUserData (token: OAuth2.AuthenticationToken) =
        async {
            let req =
                System.Net.HttpWebRequest.CreateHttp("https://api.github.com/user",
                    KeepAlive = false,
                    UserAgent = "WebSharper OAuthExample"
                )
            token.AuthorizeRequest(req)
            let! response = req.AsyncGetResponse()
            use reader = new StreamReader(response.GetResponseStream())
            let! jsonData = reader.ReadToEndAsync() |> Async.AwaitTask
            return WebSharper.Json.Deserialize<GithubUserData> jsonData
        }
```

All right, our provider is ready! Time to use it in the application proper.

First, we need to add the mini-sitelet comprising the redirect endpoint to our main sitelet. It is available as the `RedirectEndpointSitelet` property.

```
    [<Website>]
    let Main =
        Authentication.Provider.RedirectEndpointSitelet
        <|>
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.Home -> HomePage ctx
            | EndPoint.About -> AboutPage ctx
            | EndPoint.OAuthGitHub ->
                // This endpoint is already handled by RedirectEndpointSitelet above,
                // so we should never encounter it here.
                Content.ServerError
        )
```

Then, we need to handle the user authentication in `AboutPage`. We will first check the logged in user (with `ctx.UserSession.GetLoggedInUser()`). If there is one, then we show the normal About page. Otherwise, we provide a link to log in with Github. The full URL for this link can be created by the provider's `GetAuthorizationRequestUrl` method.

```
    let AboutPage (ctx: Context<EndPoint>) = async {
        let! loggedIn = ctx.UserSession.GetLoggedInUser()
        match loggedIn with
        | Some username ->
            return! Templating.Main ctx EndPoint.About "About" [
                h1 [] [text ("Welcome " + username)]
                p [] [text "This is a template WebSharper client-server application."]
            ]
        | None ->
            let loginUrl = Authentication.Provider.GetAuthorizationRequestUrl(ctx)
            return! Templating.Main ctx EndPoint.About "About" [
                h1 [] [text "Not logged in!"]
                p [] [
                    text "Sorry, you need to be logged in to access this page. "
                    a [attr.href loginUrl] [text "Click here"]
                    text " to log in."
                ]
            ]
    }
```

And voilà! We have added Github authentication support to our application. Now, when a user clicks About, they are prompted to log in using Github:

![Login prompt](https://imgur.com/bux26F1l.png)

and when they click the link and authorize your application with Github, they will be logged in and see that you have retrieved their username:

![Logged in About page](https://imgur.com/jl6I4Mtl.png)

That's all folks!