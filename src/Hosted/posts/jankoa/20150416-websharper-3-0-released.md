---
title: "WebSharper 3.0 released"
categories: "javascript,f#,websharper"
abstract: "This new major release adds a cleaner metaprogramming API, date formatting for JSON API, bug fixes and proxy revisions, among many other changes."
identity: "4323,79261"
---
Finally!

## Changes since the last release candidate

* Added `DateTimeFormatAttribute` for specifying date formatting for
JSON REST APIs. See [documentation](http://websharper.com/docs/json#heading-6)

* Macro and code generation API have been simplified.
See [documentation](http://websharper.com/docs/translation#heading-3).

* `Content.Redirect` has been renamed to `Content.RedirectPermanent`.

* Improved source mapping.
Now source map files contain the original sources, no separate hosting is necessary.
See [documentation](http://websharper.com/docs/source-mapping).

* Some proxy revisions for performance.

* Improved TypeScript declaration ouput, but it is still considered
an experimental feature. See [documentation](http://websharper.com/docs/ts-output).

### Bug fixes
* [#374](https://github.com/intellifactory/websharper/issues/374):
`WebSharper.JavaScript.X` now throws a `ClientSideOnly` exception
when called from .NET code.

* [#375](https://github.com/intellifactory/websharper/issues/375):
`FuncWithArgs` constructor fails when called with a function that
does not take a tuple.
(If you want to create a JavaScript function that takes a variable
number of arguments, use the `FuncWithRest` type.)

* [#376](https://github.com/intellifactory/websharper/issues/376):
Exception "This stream does not support seek operations" in OWIN project.

* [#378](https://github.com/intellifactory/websharper/issues/378):
Added basic CSSOM features to Dom.Element.

* [#379](https://github.com/intellifactory/websharper/issues/379):
Union types containing records get translated to the wrong representation when returned from RPC.

* [#381](https://github.com/intellifactory/websharper/issues/381):
Html projects do not output source maps or TypeScript declaration.

Happy coding!