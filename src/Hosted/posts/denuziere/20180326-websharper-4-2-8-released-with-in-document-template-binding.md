---
title: "WebSharper 4.2.8 released with in-document template binding"
categories: "ui,templating,csharp,fsharp,websharper"
abstract: "The main highlight of this release is in-document template binding, a client-side feature which treats the whole document as a template to be filled."
identity: "5515,84856"
---
We are happy to announce the release of [WebSharper 4.2.8](https://nuget.org/packages/websharper/4.2.8.255) and [WebSharper.UI 4.2.3](https://nuget.org/packages/websharper.ui/4.2.3.111).

Install project templates for this release:

* with the .NET Core SDK: `dotnet new -i WebSharper.Templates::4.2.8.227`
* [in Visual Studio 2017](http://websharper.com/installers/WebSharper.4.2.8.227.vsix)

# In-document template binding

The main highlight of this release is a WebSharper.UI feature called **in-document template binding**.

Declaring a template with the type provider parameter `clientLoad = ClientLoad.FromDocument` loads templates directly from the DOM. This makes it convenient to work on the HTML file without having to recompile your F# code. However, before this release, this feature was limited: it only applied to sub-templates, ie. templates declared within the document using `ws-template` or `ws-children-template` attributes. Such an instantiation removes the corresponding element from the DOM and creates a client-side `Doc` that can be inserted using a function such as `Doc.RunById`.

With in-document template binding, it is now possible to bind holes declared directly in the document. This is done by using the `.Bind()` method instead of `.Doc()`, `.Elt()` or `.Create()`. This new method returns `unit` and directly binds the declared holes to the DOM, without the need to detach and re-attach a sub-template.

Here is an example, which sets the content of the `<div>` when the user clicks the button:

```xml
<html>
  <head><!-- head contents... --></head>
  <body>
    <input ws-var="Username" />
    <button ws-onclick="Click">Click me!</button>
    <div>${ConfirmedUsername}</div>
  </body>
</html>
```

```fsharp
type MyTemplate = Template<"index.html", clientLoad = ClientLoad.FromDocument>

[<SPAEntryPoint>]
let Main() =
    let confirmed = Var.Create ""
    MyTemplate()
        .ConfirmedUsername(confirmed.View)
        .Click(fun e -> confirmed := !e.Vars.Username)
        .Bind()
```

For a more comprehensive example, you can also check [Adam's new getting started tutorial](https://github.com/websharper-samples/LoginWithBulma) implementing a Bulma-styled login form with `Bind()`.

The above example uses `Bind()` in an SPA project, which is the most typical use case. But it is also possible to use it in a client-server application, where the server side uses the same template, potentially binding some holes itself. For this, you need to add the parameter `keepUnfilled = true` to your server-side `.Doc()` instantiation. Otherwise, all unbound holes would be removed by the server-side rendering engine, and there would be nothing left to bind on the client.

# Full changes

## Features

* [ui#161](https://github.com/dotnet-websharper/ui/issues/161) Add `.Bind() : unit` template terminating method to directly bind holes to the current document. This is available on the client-side only if the template is declared with `ClientLoad.FromDocument`.

* ui#161 Add optional argument `.Doc(keepUnfilled: bool)` for the server side to leave unfilled holes in the document, so that they can be used on the client side by `.Bind()`.

* [ui#160](https://github.com/dotnet-websharper/ui/issues/160) Add extension methods to bring C# in line with F#:
    
    ```csharp
    // Extensions on ListModel<K, T>:
    View<IEnumerable<V>> Map(Func<T, V> f);
    View<IEnumerable<V>> Map(Func<K, View<T>, V> f);
    View<IEnumerable<V>> MapLens(Func<K, Var<T>, V> f);
    Doc Doc(Func<T, Doc> f);
    Doc Doc(Func<K, View<T>, Doc> f);
    Doc DocLens(Func<K, Var<T>, Doc> f);
    ListModel<K, V> Wrap(Func<V, T> extract, Func<T, V> wrap, Func<V, T, V> update);

    // Extension on Var<FSharpList<T>>:
    Doc DocLens(Func<T, K> key, Func<K, Var<T>, Doc> f);
    ````

## Fixes

* [#931](https://github.com/dotnet-websharper/core/issues/931) Add downwards assembly redirects. This fixes an assembly load failure when a non-WebSharper-compiled referenced assembly uses FSharp.Core > 4.4.1.0.

# Happy coding!