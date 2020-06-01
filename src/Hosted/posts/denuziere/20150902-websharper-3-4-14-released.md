---
title: "WebSharper 3.4.14 released"
categories: "ui.next,owin,release,fsharp,javascript,web,websharper"
abstract: "This release introduces features such as lenses to UI.Next and prepares the terrain for UI.Next versions of Formlets and Piglets."
identity: "4547,80162"
---
We are happy to announce the availability of WebSharper 3.4.14, which you can download [here](http://websharper.com/downloads). Here are the main highlights of this release.

## ASP.NET-hosted OWIN

Previously, a [bug](https://github.com/intellifactory/websharper.owin/issues/1) prevented the WebSharper.Owin middleware from running on top of ASP.NET using Microsoft.Owin.Host.SystemWeb. This has now been solved, and the following is now possible:

```fsharp
open global.Owin
open Microsoft.Owin
open WebSharper
open WebSharper.Owin

module Site =
    let Main = Application.SinglePage(fun ctx -> Content.Text "Hello world!")

type Startup() =
    member this.Configuration(app: IAppBuilder) =
        app.UseSitelet(System.Web.HttpRuntime.AppDomainAppPath, Site.Main)
        |> ignore

[<assembly:OwinStartup(typeof<Startup>)>]
do ()
```

For this you need to install the following NuGet packages in your web application: `WebSharper`, `WebSharper.Owin`, `Microsoft.Owin.Host.SystemWeb` and `Mono.Cecil`.

On the topic of Owin, the [Suave](http://suave.io) team recently pre-released support for running Owin middleware; we will soon publish a blog entry about running WebSharper applications within Suave.

## WebSharper UI.Next

A number of new features have also been added to UI.Next.

### Data binding: abstract references and lenses

Until now, UI.Next interactive elements such as `Doc.Input` were only able to interact directly with a `Var` of primitive type. This was quite limiting in terms of how you could structure your data at the root of the dataflow graph. For example, if you wanted to have a `ListModel` with a record type for items, and display it with input fields to edit the individual record fields, then those record fields would need to have type `Var<'T>` themselves.

The new abstract reference type `IRef<'T>` relieves this restriction. `IRef<'T>` is an interface type that represents data that can be directly read or written, or observed by a `View<'T>`. Conceptually, that's exactly what `Var<'T>` is, and of course `Var<'T>` implements this interface; but not only. There are now methods on `Var<'T>` and `ListModel<'K, 'T>` that return `IRef<'U>`s where `'U` represents a part of the original `'T`. This concept is called _lensing_. For example:

```fsharp
type Person = { FirstName: string; LastName: string; Id: Key }

let me : Var<Person> =
  Var.Create { FirstName = "Loïc"; LastName = "Denuzière"; Id = Key.Fresh() }
let myFirstName : IRef<string> =
  me.Lens (fun p -> p.FirstName) (fun p n -> { p with FirstName = n })
let myFirstNameInput =
  Doc.Input [] myFirstName
```

In the above code, `myFirstName` is an `IRef<string>` that _lenses_ into the `FirstName` field of `me`. Retrieving `myFirstName`'s value retrieves the `FirstName` field of the current value of `me`, and setting `myFirstName` performs a record update on the current value of `me`. It is therefore possible to create an input field that is bound to the `FirstName` field of `me`. Note how the type `Person` is able to be entirely immutable.

Even more useful is lensing into `ListModel`s. See for example the following editable list of people:

```fsharp
let people = ListModel.Create (fun p -> p.Id) []
let firstNameOf : Key -> IRef<string> =
  people.LensInto (fun p -> p.FirstName) (fun p n -> { p with FirstName = n })
let lastNameOf : Key -> IRef<string> =
  people.LensInto (fun p -> p.LastName) (fun p n -> { p with LastName = n })
let displayPeople =
  ul [
    people.View |> Doc.ConvertSeqBy people.Key (fun pid person ->
      li [
        Doc.Input [] (firstNameOf pid)
        Doc.Input [] (lastNameOf pid)
      ]
    )
  ]
```

Creating a lensed reference is currently somewhat awkward, since you have to provide both the get and update functions. We are planning on providing a macro that will be able to infer the update function based on the getter function, making lenses more convenient to use.

### Submitters

A simple way to allow the dataflow to react to isolated events is to have a view whose value is taken from an input view, but only gets updated when events occur. This was already possible using `View.SnapshotOn`, but somewhat more involved than it needed to be. The new type `Submitter<'T>` simplifies this task:

```fsharp
let rvInput = Var.Create "the input data"
let submit = Submitter.Create rvInput.View "no data submitted yet"
div [
  Doc.Input [] rvInput
  Doc.Button "Submit" [] submit.Trigger
  p [
    text "You entered: "
    textView submit.View
  ]
]
```

### Other UI.Next features

* **Add some instance equivalents to static members:** the functions `View.Map`, `View.Bind` and `ListModel.View` are now available as instance members on the corresponding types, allowing slightly shorter code for those who prefer this style.

* **ListModel.Key**: both as a static and instance member, retrieves the function that is used by a `ListModel` to get the key of an item. Typically needed by `View.Convert*By` or `Doc.Convert*By` (see the lens example above).

* **Doc.BindView**: it is rare, when inserting a dynamic doc using `Doc.EmbedView`, to have a `View<Doc>` handy; instead, you generally have a `View<'T>` that needs to be mapped to a `View<Doc>`. The new function `Doc.BindView : ('T -> Doc) -> View<'T> -> Doc` combines these two steps in a single function call.

* **Event handlers that can use a `View`'s current value:** it is fairly common to need the current value of a reactive `View` inside the callback of an event handler. It is now very easy to do so using the function `Attr.HandlerView`, or methods such as `on.clickView`.

* **Templating: allow an element to be both a hole and a subtemplate.** This is useful for example when the hole is to be filled with a list of items, and the subtemplate describes how these items will be displayed.

* **`doc.On*` methods for standard events** have been added, akin to the `on.*` attributes. They also exist in server-side friendly version, taking a quotation as argument.

* **`on.*` attributes** have been converted to camelCase; for example, `on.animationend` is now `on.animationEnd`.

## Bug fixes

* [#464](https://github.com/intellifactory/websharper/issues/464): Fixed the client-side JSON encoding of `option<_>` union case arguments.

* [#463](https://github.com/intellifactory/websharper/issues/463): Track dependencies from the body of `[<Macro>]`-annotated functions.

* Fixed the type of `JQuery.Position` fields from `int` to `float`.

* [UI.Next#28](https://github.com/intellifactory/websharper.ui.next/issues/28): correctly map from string the value of `Doc.IntInput` and `Doc.FloatInput`.

## Future plans

As mentioned in previous blog entries, our intention is to merge UI.Next into the main WebSharper distribution and to obsolete `Html.Client` and `Html.Server`. We are planning to have this done for version 3.5.

At the same time, we are starting to implement UI.Next-based [Formlets](https://github.com/intellifactory/websharper.ui.next.formlets) and [Piglets](https://github.com/intellifactory/websharper.ui.next.piglets). Piglets are pretty much usable already, while Formlets are still in a more early phase. In both cases, we are seeing huge gains compared with the IntelliFactory.Reactive-based counterparts in terms of code simplicity and safety. Look forward to being able to use Piglets and Formlets within UI.Next!

Happy coding!