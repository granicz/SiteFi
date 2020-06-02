---
title: "Announcing Try F# on WebAssembly"
categories: "wasm,fcs,bolero,webassembly,tryfs,f#,websharper"
abstract: "Just a short week ago, we announced the first release of Bolero, enabling full-stack F# web development on WebAssembly. Since then we have been working on a small side project to see what it would take to implement a fully client-side implementation of the F# compiler: basically, to embed FSharp.Compiler.Services (FCS) in a small Bolero application."
identity: "5696,86246"
---
Just a short week ago, we [announced](https://forums.websharper.com/blog/86199) the first release of [Bolero](https://github.com/fsbolero/Bolero), enabling full-stack F# web development on WebAssembly. Since then we have been working on a small side project to see what it would take to implement **a fully client-side implementation of the F# compiler**: basically, to embed FSharp.Compiler.Services (FCS) in a small Bolero application.

The main motivations of this project are pretty straightforward and incredibly useful - it will enable:

 * **Running the F# compiler in the browser**, without a backend to provide code services
 * **Incremental type checking**, i.e. getting compiler warnings and errors as you type
 * **Fast compilation/execution**, enabled by the caching of the compilation context on the client
 * Basic **type provider support**, so you can quickly demo all the F# goodness to your peers
 * Switching among multiple "built-in" snippets to facilitate easier experimentation

### Current status

You can see the current status below and you can also [try it live](https://fsbolero.github.io/TryFSharpOnWasm/) (Firefox recommended for enhanced performance): 

[![](https://i.imgur.com/6jzHiQSl.png)](https://i.imgur.com/6jzHiQS.png)

One of the snippets that are available demonstrates the use of the JSON type provider from FSharp.Data:

[![](https://i.imgur.com/jqPFM5Rl.png)](https://i.imgur.com/jqPFM5R.png)

### Limitations and things to improve on

There is a **significant download size** to download initialize the application. This involves downloading all the assembly references to get FCS running (~28 MBs, which should cache in the browser for future page loads/refreshes), loading them into the runtime and initializing the code service (around 8-10 seconds, depending on your machine).

Currently, the code service is invoked in the main thread, causing **noticable freezes in the UI** while it's executing. This can be significant on larger code snippets. Given the nature of on-the-fly type checking, going around the single-threaded limitation with web workers, a possible path to alleviate the UI blocking issue, is not exactly ideal. Instead, this limitation will solve itself when proper threading support is added to the underlying runtime (see [one of the related issues](https://github.com/aspnet/AspNetCore/issues/5475) filed under Blazor#139/AspNetCore#5475).

Furthermore, in this first version, only basic type providers are supported. This includes the JSON and the XML type providers from FSharp.Data for the time being.

### Get involved

Check out [the source code for the project](https://github.com/fsbolero/TryFSharpOnWasm) and get involved, your suggestions/contributions/PRs are welcome!

Happy coding!
