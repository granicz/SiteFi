---
title: "WebSharper 4 beta-10 released"
categories: "c#,javascript,f#,websharper"
abstract: "Fixes and old and recent community requests are released"
identity: "5412,83173"
---
We are preparing for stable release, reviewing all current issues and documentation. Beta-10 solves some old (reference equality for classes) and recent (`ToString` for `Systen.Char`) issues reported. Look for VS2017 installer [here](http://websharper.com/downloads) (VS2015 support upcoming).

# Fixes
* [#734](https://github.com/intellifactory/websharper/issues/734) Classes now have reference equality by default (you can override `Equals` as in .NET). Arrays, tuples, F# records and unions still use structural equality
* [#736](https://github.com/intellifactory/websharper/issues/736) `System.Char` is now translated to a 1-length `String` in JavaScript. RPC handling and all methods dealing with `Char`s have been updated.
* [#742](https://github.com/intellifactory/websharper/issues/742) C# 7.1 features are working.
* [#743](https://github.com/intellifactory/websharper/issues/743) `printf ""` compiles properly.
* [#741](https://github.com/intellifactory/websharper/issues/741) `Guid.Empty` is usable client-side.
* [#547](https://github.com/intellifactory/websharper/issues/547) Numeric conversions in C# have the same semantics as F# helpers (parsing numbers, integral types truncate but no size checks).
* [#549](https://github.com/intellifactory/websharper/issues/549) You can use padding numbers in C# string interpolation syntax.
* [#739](https://github.com/intellifactory/websharper/issues/739) fix assembly resolution on Mono to read runtime Sitelet mertadata.
* [#746](https://github.com/intellifactory/websharper/issues/746) JavaScript named functions inside inlines are preserved properly
* [#747](https://github.com/intellifactory/websharper/issues/747) `Stub` attribute behaves in compatibile way with WS3: if all members of a class have the `Stub` attribute, it is equivalent to the class having it (WS does not redefine the type, it is assumed to exist in outside JS), but having any non-Stub instance members causes WS to define the class. This is fixing issues in WebSharper.JQuery.

# Enhancements
* [#545](https://github.com/intellifactory/websharper/issues/545) Nullable operators with correct semantics. In F#, you can use the `FSharp.Linq.Nullable` and `FSharp.Linq.NullableOperators` modules on the client-side. C# operations on Nullables now follow .NET semantics properly: a `null` value is propagated to the result.
* [#744](https://github.com/intellifactory/websharper/pull/744) Static members can be proxied across multiple projects.

# Breaking changes
* If you have relied on classes having automatic structural equality in the WebSharper translation, this is now incorrect. Implement an override for `Equals`.
* Create an empty JavaScript plain object with `new JSObject()` / `New []`. Previously this was equivalent to `new object()` / `obj()`, but now the latter translates to an instance of `WebSharper.Obj` which defines its own `Equals` and `GetHashCode` methods.
* Default hash value of classes are now always `-1`. This should not be breaking, but if you use a class as keys or rely on hashing in any other way, be sure to override `GetHashCode` on your class for performance.
* `System.Decimal` support has been removed from WebSharper main libraries. It is now a part of `WebSharper.MathJS` and now has correct precision.