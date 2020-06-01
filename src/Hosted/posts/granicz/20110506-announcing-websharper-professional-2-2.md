---
title: "Announcing WebSharper Professional 2.2"
categories: "f#,websharper"
abstract: "Today we are very excited to announce the availability of the final release of WebSharper Professional (version 2.2.x), the premiere web development framework for F# developers, combining the strengths of functional programming with robust and composable web programming abstractions such as formlets and sitelets that unrival the capabilities of any other web framework.  This release follows many long months of hard work and sleepless nights and overall signifies a major new addition to the WebSharper family."
identity: "989,74570"
---
Today we are very excited to announce the availability of the final release of WebSharper Professional (version 2.2.x), the premiere web development framework for F# developers, combining the strengths of functional programming with robust and composable web programming abstractions such as formlets and sitelets that unrival the capabilities of any other web framework. This release follows many long months of hard work and sleepless nights and overall signifies a major new addition to the WebSharper family.

### Authentication support for Sitelets

WebSharper Professional delivers a new generation of WebSharper Sitelets functionality that now includes support for authentication, enabling you to compose sitelets with protected content that to be accessed requires users to be authenticated. Defining the authentication process is short and straightforward, and consist of checking the validity of user credentials, and specifying a filter for each "role" of the application. These filters then can be used to protect smaller sitelets that are then combined with other, unprotected counterparts, as shown in the following snippet:

```fsharp
/// The sitelet that corresponds to the entire site.
let EntireSite =

    // A simple sitelet for the home page, available at the root of the application.
    let home = 
        Sitelet.Content "/" Action.Home Pages.HomePage

    // An automatically inferred sitelet for the basic parts of the application.
    let basic =

        Sitelet.Infer <| fun action ->
             ...

    // A sitelet for the protected content that requires users to log in first.
    let authenticated =
        let filter : Sitelet.Filter<Action> =
            {
                VerifyUser = fun _ -> true
                LoginRedirect = Some >> Action.Login
            }

        Sitelet.Protect filter <|

            Sitelet.Content "/protected" Action.Protected Pages.ProtectedPage

    // Compose the above sitelets into a larger one.
    [
        home
        authenticated
        basic
    ]
    |> Sitelet.Sum
```

### Change log

You can find the full change log here.

 * Add the ability to identify product updates (#315)
 * CLR invalid program (#326)
 * Template generator fails on HEAD placeholders (#350)
 * Formlet values triggered multiple times (#368)
 * ASP.NET MVC Sample Application Demo Error (#373)
 * VS 2010 Add-in failed to load (#379)
 * Formlet buttons look weird in IE7 (#382)
 * WIG compilation not working (#390)
 * After successful installation VS2010 add-in fails to load (#393)
 * 2.1.80 installer bug (#398)
 * Templates not visible on Vista 64 (#400)
 * xhtml2fs - Comments after body tag yielded no code for the body (#401)
 * Visual Studio Templates should be installed for all users (#402)
 * WebSharper.exe fails unless F# for .NET 2.0 is installed (#403)
 * Binding template fails on Vista64 (#404)
 * The HTML Application template fails on Vista64 (#406)
 * Failed to uninstall (#407)
 * Sitelets Sample Template for VS9 fails to map '/' (#409)
 * Installer doesn't create menu shortcut for samples (#411)
 * Installer should contain the Scribble-based documentation (#412)
 * Embedding text under body tag in xml template fails (#413)
 * Installer for 2.2 puts it's content in WebSharper\2.1 folder (#415)
 * Implement authentication support for sitelets (#416)

### Installation and extensions

You can grab the latest binaries from [this link](http://websharper.com/latest/ws2). Be sure to uninstall any previous version of WebSharper 2.x.
If you have any earlier extensions installed for 2.1, you should remove them and get their new matching versions for 2.2 from the [WebSharper Extensions Gallery](http://websharper.com/extensions). If you have any trouble installing or using these new extension releases, let us know on the [Community page](http://websharper.com/Community.aspx). Happy coding!
