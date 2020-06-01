---
title: "Create JavaScript libraries from your C# or F# code"
categories: "c#,f#,websharper"
abstract: "Learn how to use WebSharper's JavaScriptExport feature to create standalone .js files from your .NET projects"
identity: "5601,85353"
---
There might be many reasons why a .NET-to-JavaScript transpiler is useful, some of the most common use cases:

* Reuse existing code within a web application
* Use the same data models and part of the code for the server and client side of the same web project
* Write complicated algorithms or business logic with strong typing and advanced features of C# and/or F#, which is easier to implement and test in .NET

WebSharper is such a transpiler, but also more, a set of tools for creating full websites or standalone single page web application.
But sometimes something simpler and more to the point tool is needed, to just create a single self-contained `.js` file that is exposing the translated code well and can be used as a module in a  website or JavaScript-based mobile application using any other technologies and frameworks.
WebSharper 4.2.13 introduces a simple way to do just this that previously required some hacks.

This article is using C# terminology and syntax for attributes, but all the same features apply to F# as well.

## Translating a library project

Suppose that you have a library project that contains functionality that you would like to access in JavaScript.
First, add `WebSharper.CSharp` nuget package (or `WebSharper.FSharp` for an F# project).
This also installs the `WebSharper` package, containing the core APIs and proxies (JavaScript implementations of .NET framework built-in classes).

### Translate whole code

WebSharper does not auto-translate anything in a project, only marked code parts.
This is so that mixed client/server projects can be written within a single project.
But if you do like to translate it all, use

```csharp
using WebSharper;

[assembly: JavaScript]
```

If the project only uses .NET standard library classes and methods that WebSharper knows a translation for, you get no errors,
otherwise at this point the WebSharper code analyzer alerts you at the method and constructor calls that are not supported for client-side use
(You have to build the project to see WebSharper-related warnings/errors for F#).
To handle these errors, [look below](#resolve).

Even if you have no errors, when you build the project, no output is produced other than the normal `.dll`.
This is because the default mode of WebSharper is to prepare libraries to be used in WebSharper-based sites.
When you look inside the `.dll` with a tool like `ILSpy`, you can find resources added to the assembly, for example a `WebSharper.js` which contains the translated code for the current library (but not any dependency it might have).
Extracting this is still not enough, as this code can rely on JavaScript files contained in references (like WebSharper core libraries itself).

To create a self-contained `.js`, we need to configure the WebSharper compiler a bit.

### The `wsconfig.json` file

The easiest way for configuration is to add a file named `wsconfig.json` to your project root folder.
Initialize it like this:

```
{
  "$schema": "https://websharper.com/wsconfig.schema.json",
  "project": "bundle",
  "outputDir": "Content",
  "javascriptExport": true
}
```

Now if you build your library, you will see a `Content` folder populated within your project folder.
Our main interest is in the `MyLibrary.js` file, which contains the full JavaScript output (there is also a minified version of it).
The "bundle" project type uses dead code elimination to produce this, recursively only including those functions that are called from the starting code points.
These starting points are:

* You can have a static argumentless method marked with the `[SPAEntryPoint]` attribute. This will be included in bundle output (along with all code dependencies), and will run on script load.
* Any other code marked directly or indirectly with `[JavaScriptExport]` (which is a stronger attribute than `[JavaScript]`, marking it for translation but also inclusion in bundle output) and their code dependencies.

The `"javascriptExport": true` setting in `wsconfig.json` is actually equivalent to having `[assembly: JavaScriptExport]`, so the previously added assembly-level attribute can be removed with no change.

Note, that you also have a `MyLibrary.head.html` and `MyLibrary.css` files.
The former contains links generated to all external scripts that the bundle output `.js` file needs.
The `.css` contains all style resources embedded in WebSharper-enabled libraries that are required by the bundled code.
(This should be empty if you only use WebSharper core libraries.)

The `MyLibrary.head.js` is just a variant of the `head.html` to use in active development only, it uses `document.write` to add the required scripts.
This is a deprecated feature in modern browsers, but allows linking this script and not worry about if required scripts are changing during development.
It is highly recommended to take a look at `head.html` and making sure those dependencies are included in your `html` file when building for deployment.

### Output `.js` file only

If you use the `"project": "bundleOnly"` project type, the output `.dll` will not get the WebSharper-specific resources added, so it will not be able to be used as a reference to another WebSharper-enabled project.
(In the case of F#, even emitting a `.ddl` is skipped.)
The only goal of WebSharper compiler here becomes to produce the bundle output files.

If you do not set `"outputDir"` but set one or both of `"jsOutput"` and `"minJsOutput"` (value must be a relative or absolute path to the output file), only the `.js` and/or `.min.js` files will be produced.

### Translate select methods and types

If you only want to mark specific methods and/or types to be included in `.js` output (serving as starting points for dead code elimination), you can apply the `JavaScriptExport` attribute and/or `wsconfig.json` setting in various ways:

* Add a `[JavaScriptExport]` attribute to the type or member itself or on a whole assembly. The latter is equivalent to having `"javascriptExport": true`.
* Add a `[JavaScriptExport(typeof(MyType))]` attribute on either assembly-level or on a class that contains the targeted type as a nested class. This marks the targeted type, same as if it would have a `[JavaScriptExport]` directly. This can also be used to target types in another project. Note that the other project need to be WebSharper-enabled too, and have that type in marked with `[JavaScript]` (using one of the same ways as for `JavaScriptExport`). So when the first assembly is compiled, WebSharper can prepare the translation of that type, and when targeting it with `JavaScriptExport`, that can be added to the bundle output readily.
* Add a `[JavaScriptExport("MyNameSpace.MyType")]` attribute, which works the same as using `typeof`. It also has a configuration equivalent in `"javascriptExport": [ "MyNameSpace.MyType" ]`. You can add multiple types to this array.
* Add a `[JavaScriptExport("AssemblyName")]` attribute or equivalently use `"javascriptExport": [ "AssemblyName" ]`. This marks the full WebSharper-translated part of targeted assembly to be included in bundle output. So that library must have the WebSharper libraries and tools referenced and have the parts intended for client-side use marked for translation with `JavaScript`/`JavaScriptExport`.

<a name="resolve"></a>
## Resolve `not found in JavaScript translation compilation` errors

There are limitations to WebSharper's ability to translate method and constructor calls, only those that have proxies in the core libraries will be able to be used in JavaScript-annotated code.
WebSharper uses type-erasure for the output JavaScript code, using dynamic code generation based on type information at compile time.
This means that using `System.Type` and reflection are not supported.
So sometimes you have to modify your code to run in a suitable way in the browser while also keeping .NET functionality intact.

### Branching via `IsClient`

The easiest solution is to introducing some branching around code parts that cannot be translated directly to JavaScript.
Use the static property `WebSharper.Pervasives.IsClient` in a branching (`if` or `? :`) to have the `false` branch not considered for translation, while
it will be run in .NET proper.
(You can write only `IsClient` in F# if you have `open WebSharper`)

```csharp
    if (WebSharper.Pervasives.IsClient) {
        // this will be translated and run in the browser
    } else {
        // this will only run in .NET
    }
```

### Adding proxies

WebSharper core libraries itself are defining types that are `internal` but marked with the `Proxy` attribute to denote that they are implementing another class for client-side use.
You can do the same to add support for translating types that are found in non-WebSharper enabled assemblies (either framework assemblies or third-party libraries).

For a minimal implementation of `StringBuilder`, you can write:

```csharp
[Proxy(typeof(StringBuilder))]
internal class StringBuilderProxy
{
    private List<string> b = new List<string>();

    public StringBuilderProxy Append(string s)
    {
        b.Add(s);
        return this;
    }

    public override string ToString()
    {
        var s = String.Concat(b);
        b.Clear();
        b.Add(s);
        return s;
    }
}
```

Or the same in F#:

```fsharp
[<Proxy(typeof<System.Text.StringBuilder>)>]
type internal StringBuilderProxy () =
    let b = ResizeArray()

    member this.Append(s: string) =
        b.Add(s)
        this

    override this.ToString() =
        let s = String.concat "" b 
        b.Clear()
        b.Add(s)
        s
```

If the type signature of a method matches the a method on the targeted class (taking the proxy type itself to be equivalent to the target), then it will serve as the JavaScript implementation.

If you feel that some type or method should have a standard proxy, feel free to ask about it on the [WebSharper forums](https://forums.websharper.com/) or the submit a pull request to the open-source [repository on GitHub](https://github.com/dotnet-websharper/core).

Have a good time running .NET code in your websites!