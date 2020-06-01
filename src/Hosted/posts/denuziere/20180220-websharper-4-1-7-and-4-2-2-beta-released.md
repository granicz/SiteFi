---
title: "WebSharper 4.1.7 and 4.2.2-beta released"
categories: "csharp,fsharp,websharper"
abstract: "This is a bugfix release for both the stable and beta branches of WebSharper."
identity: "5504,84606"
---
This is a bugfix release for both the stable and beta branches of WebSharper. It is available as `dotnet` templates or as a Visual Studio installer; see [how to install WebSharper](http://developers.websharper.com/docs/v4.x/fs/install).

# Fixes

## For 4.1.7 and 4.2.2-beta

* [#911](https://github.com/dotnet-websharper/core/issues/911) More `System` namespace numeric types handled correctly by `Router.Infer` (both for server and client-side usage): `SByte`, `Byte`, `Int16`, `UInt16`, `UInt32`, `Int64`, `UInt64`, `Single`.
* [#906](https://github.com/dotnet-websharper/core/issues/906) Fix handling of F# type alias resolving to a type parameter (`type Alias<'T> = 'T`)

## For 4.2.2-beta

* [#909](https://github.com/dotnet-websharper/core/issues/909) Compiling with WebSharper does not require installing .NET 4.7.1 anymore; 4.6.1 is now sufficient.
