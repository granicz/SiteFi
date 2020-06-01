---
title: "CloudSharper Local alpha 0.9.7"
categories: "cloudsharper,f#,websharper"
abstract: "We are happy to announce the latest release of CloudSharper, version 0.9.7, bringing a new type checking and code assistance engine for F# projects. This last week we have been working hard on moving CloudSharper to FCS (FSharp.Compiler.Service), and with this current release much of the migration work is completed. As a result, the type checker in F# projects is considerably faster, and code assistance features are available in more locations that in previous CloudSharper releases."
identity: "3755,77079"
---
We are happy to announce the latest release of CloudSharper, version 0.9.7, bringing a new type checking and code assistance engine for F# projects. This last week we have been working hard on moving CloudSharper to FCS ([FSharp.Compiler.Service](https://github.com/fsharp/FSharp.Compiler.Service)), and with this current release much of the migration work is completed. As a result, the type checker in F# projects is considerably faster, and code assistance features are available in more locations that in previous CloudSharper releases.

Moving to FCS replaces our heavily modified and customized fork of the earlier type checking engine, and given the amount of work going into FCS nowadays it is the only sensible choice going forwards.

Happy coding!