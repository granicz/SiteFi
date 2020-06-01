---
title: "WebSharper 3.4.19 released"
categories: "ui.next,xs,release,fsharp,javascript,web,websharper"
abstract: "This new release of WebSharper adds the Xamarin Studio addin to the main release channel and introduces checked UI.Next number inputs, among many minor features and bug fixes."
identity: "4555,80344"
---
We are happy to announce the release of version 3.4.19 of the WebSharper stack, which you can download [here](http://websharper.com/downloads). This release is mainly dedicated to bug fixes and minor features.

### Xamarin Studio / MonoDevelop addin on the main channel

The WebSharper addin is now available on the main Xamarin Studio channel. This means that you can now drop the `githubusercontent.com`-based custom repository, and grab WebSharper directly from Xamarin instead! Simply check the "Web Development" category in the addin manager's gallery.

### UI.Next Checked number inputs

The main new feature (and breaking change) is in UI.Next. The functions `Doc.IntInput` and `Doc.FloatInput`, which associate a `Var<int>` or `Var<float>` to a `<input type="number">` element, took little care to check the correctness of the actual text entered by the user. This could result in awkward and even browser-inconsistent user experience, where mistakenly entering "1e" would return 1 on some browsers and 0 on others.

These combinators have therefore been modified to perform more input checking and bind the value to a `Var<CheckedInput<_>>`, with the following definition:

```fsharp
type CheckedInput<'T> =
    | Valid of value: 'T * inputText: string
    | Invalid of inputText: string
    | Blank of inputText: string
```

This way, typing "1e" consistently sets the `Var` to `Invalid "1e"`.

The old behavior can still be achieved using `Doc.IntInputUnchecked` and `Doc.FloatInputUnchecked`, but is not advised.

### Full change log

Here are the minor changes and bug fixes to WebSharper and components since the previous release.

#### WebSharper

* [#402](https://github.com/intellifactory/websharper/issues/402): Fix exception when compiling a WebSharper project that references a Roslyn-compiled assembly.

* [#467](https://github.com/intellifactory/websharper/issues/467): Add `JS.RequestAnimationFrame` and `JS.CancelAnimationFrame`, with a shim for IE9-.

* [#470](https://github.com/intellifactory/websharper/issues/470): Fix the `printf` family of functions for union cases with `[<Constant>]` attributes.

* [#471](https://github.com/intellifactory/websharper/issues/472): Fix exception on mono when looking for a `[<Website>]` if a dependent assembly is missing.

* [#473](https://github.com/intellifactory/websharper/issues/473): Add server-side JSON serialization of the `unit` type.

* Add missing proxy for `jQuery.parseHTML`.

#### WebSharper.UI.Next

* [#31](https://github.com/intellifactory/websharper.ui.next/issues/31): Add `Doc.Verbatim` to create a `Doc` from verbatim HTML. Be careful not to use it with user input data!

* [#33](https://github.com/intellifactory/websharper.ui.next/issues/33): camelCase on Expr-based events (eg. `on.keyUp` instead of `on.keyup`).

* [#34](https://github.com/intellifactory/websharper.ui.next/issues/34): make `IntInput` and `FloatInput` checked (see above).

* [#36](https://github.com/intellifactory/websharper.ui.next/issues/36): Add attr.\`\`data-\`\` combinator to `WebSharper.UI.Next.Html` to create `data-foo="value"`-type attributes.

* [#37](https://github.com/intellifactory/websharper.ui.next/issues/37): Add `Doc.SelectOptional`, which adds a first selectable option that sets the `Var` to `None`.

* [#38](https://github.com/intellifactory/websharper.ui.next/issues/38): Add `Doc.SelectDyn` and `Doc.SelectDynOptional`, for which the list of available options comes dynamically from a `View`.

* [#39](https://github.com/intellifactory/websharper.ui.next/issues/39): Add `View.MapCached`, which is identical to `View.Map` except that it doesn't call the mapping function again if the input value is equal to the previous input. Useful if the mapping function is computationally expensive. Note that the output is still marked obsolete, it is simply re-filled with the same cached value.

* [#40](https://github.com/intellifactory/websharper.ui.next/issues/40): Add `Doc.WebControl`, which allows including a `WebSharper.Web.Control` inside a server-side `Doc`.

* [#41](https://github.com/intellifactory/websharper.ui.next/issues/41): Add `.With()` method overloads for `WebSharper.Sitelets.Content.Template` that take `Doc`s as holes.

* [#42](https://github.com/intellifactory/websharper.ui.next/issues/42): Add instance member versions of `Doc.Run`, `Doc.RunById` and `Doc.AsPagelet`.

#### MonoDevelop.WebSharper

* Release on the main Xamarin channel (see above).

* [#3 on websharper.templates](https://github.com/intellifactory/websharper.templates/issues/3): duplicate `.fsproj` file when creating a project whose name isn't also a valid identifier.

Happy coding!