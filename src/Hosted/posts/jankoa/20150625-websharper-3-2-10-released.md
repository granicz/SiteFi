---
title: "WebSharper 3.2.10 released"
categories: "javascript,f#,websharper"
abstract: "This is a bugfix release mainly focusing on issues exposed by WebSharper.Warp"
---
This release fixes some bugs when compiling from F# quotations (used for example by **WebSharper.Warp**).

### Full change log since 3.2.7:

* [#430](https://github.com/intellifactory/websharper/issues/430): Inferred sitelets: Allow specifying the method in `EndPoint` 

  For example this:
  ```fsharp
  type Action =
    | [<EndPoint "GET /home">] Home
  ```

  would be equivalent to:
  ```fsharp
  type Action =
    | [<Method "GET"; EndPoint "/home">] Home
  ```

* [#431](https://github.com/intellifactory/websharper/issues/431): Don't write meta and init script tags when there are no resources

* [#437](https://github.com/intellifactory/websharper/issues/437): Interfaces generated by WIG

	Previously using interfaces generated invalid IL, implementations were missing.
    Also interface methods had no `Inline` attributes for WebSharper translation.
    
    Now an interface is auto-implemented on the class.
    Inlines are copied from the interface declaration, no customization is possible currently.
    If there are multiple interfaces defining the same method, the inline found on the first interface definition will be used.
    
* [#435](https://github.com/intellifactory/websharper/issues/435): Generic type constructor in tranlating a quoatation
    
* [#424](https://github.com/intellifactory/websharper/issues/424): Nested types are handled incorrectly when translating with runtime reflection