---
title: "Bolero 0.9 released"
categories: "bolero,f#,websharper"
abstract: "Release for .NET Core 3.0 RTM, introducing page models!"
identity: "5824,87347"
---
We are happy to announce the release of [Bolero](https://fsbolero.io/) version 0.9. Bolero is a library that enables writing full-stack applications in F#, whose client side runs in WebAssembly using [Blazor](https://blazor.net/).

This release requires the .NET Core SDK version 3.0.100, which you can [download here for Windows, OSX or Linux](https://dotnet.microsoft.com/download/dotnet-core/3.0).

Install the latest project template with:

```
dotnet new -i Bolero.Templates
```

If you have an existing Bolero project, you can check [the upgrade guide](https://fsbolero.io/docs/Upgrade) to learn how to update your project for Bolero 0.9.

## Page models

The new feature introduced in this release is **page models**.

A common need for rich applications is to have, in addition to the application's global model, a model that is specific to each page. For example, the login page has the username and password that are being entered as model. A page dedicated to displaying and manipulating a list of items has that list of items as model.

In Bolero, pages are represented as F# unions, with one case per page. With page models, the page's model can now be included as an argument to this union.

```fsharp
type LoginModel = { username: string; password: string }

type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/login">] Login of PageModel<LoginModel>
```

This type is known by the routing system so that URLs are parsed and generated appropriately; you simply have to define what the default model is when first switching to a page:

```fsharp
let defaultModel = function
    | Home -> ()
    | Login model -> Router.definePageModel model { username = ""; password = "" }
```

Learn more about page models in [the documentation](https://fsbolero.io/docs/Routing#page-models).

## .NET Core 3.0

Bolero 0.9 coincides with the first release of .NET Core 3.0 RTM.

However, Blazor is still in preview and slated for release in May next year. Therefore, Bolero 0.9 still bears the tag "preview9".

## SourceLink

To enable debugging Bolero in server-side applications, we have added .pdb files to the NuGet packages, pointing to GitHub using SourceLink.

Happy coding!