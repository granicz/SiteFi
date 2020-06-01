---
title: "WebSharper.3.0.26-alpha released"
categories: "javascript,f#,websharper"
abstract: "New release of WebSharper 3.0-alpha contains strong typing for all kinds of JavaScript functions and a major update to the Interface Generator API."
identity: "4210,77685"
---
This release of WebSharper 3.0 alpha ships several major and breaking changes: introducing strongly-typed JavaScript function interoperability, various WebSharper Interface Generator (WIG) API enhancements and revisions, bringing the JQuery binding up to date (to 1.11.2) and more type-safe, and lots of other small fixes and improvements.

If you update WebSharper, be sure to update all your other WebSharper-based packages and rebuild all your projects with the new version because of metadata incompatibility.

## Strongly-typed JavaScript function interoperability

### Summary of the visible changes
 
This change standardizes the way functions passed to and from JavaScript libraries (eg. callbacks) are represented. Those used to be inconsistent depending on whether the function was expected to use the `this` value: a delegate was used if it was, and an F# function with tupled arguments if it wasn't. Now the type of JavaScript callbacks is as follows:

    fun this (arg1, arg2, ..., argn) -> result

If the function is not expected to use the `this` value, then the first argument is omitted, and the function is identical to previous versions of WebSharper. Conversely, if `this` is used and the JavaScript function takes no argument, then the second argument is omitted.

If the JavaScript function is variadic (ie. it takes *n* fixed arguments and a variable number of extra arguments), then the corresponding F# function takes a tuple of *n + 1* arguments, where the last argument is an array of the variable arguments.

In some rare cases (eg. when using arrays of functions), the function needs to be wrapped in one of the types described below in "New function types".
 
### In-depth explanation 

The `FSharpFunc<'TArg, 'TRes>` type (which is used for lambdas by the F# compiler) always take one parameter (which can be a tuple). In WebSharper translation, these become JavaScript functions taking a single fixed-length array (although curried functions only used in local scope are optimized). However, it is often required that we pass functions defined in F# to a JavaScript library (for example event handlers, callbacks, functional-style libraries).

Previous versions of WebSharper used a wrapper function called `Runtime.Tupled` for this. This created a function that passed along its arguments to the inner function if it was called with 0 or 1 arguments, otherwise passing the `arguments` object. This meant that `Runtime.Tupled`-wrapped functions had a double calling convention: with a single array or with multiple arguments.

Although this solution was a performance hit (tuple-argument lambdas got wrapped even if they were never passed to outside JavaScript code), it covered most cases. But if the JavaScript library would sometimes call the function with one argument and sometimes more, `Runtime.Tupled` would cause errors. Also there was no strongly typed way to handle functions returned by outside JavaScript libraries, or function-typed properties on objects.

This release introduces a one-to-one relation between new F# types describing functions and how they are to be called/treated in JavaScript. WIG is also expanded with support for interop transformations, including automatically converting between function calling conventions. F#-defined lambdas used only in passing to outside JavaScript code are now optimized in the output to be straight functions with multiple arguments, not a wrapped function. This means then a lambda like `fun (a, b) -> a + b` would be translated to `function(tupledArg) { return tupledArg[0] + tupledArg[1]; }` but if you have this lambda passed to a JavaScript library expecting a function with two arguments, it will become `function(a, b) { return a + b; }`.

## New function types

In the `IntelliFactory.WebSharper.JavaScript` namespace you can find several new types corresponding to JavaScript functions. These have constructors for creating them from F# functions, and methods to call them. As described in the WIG section, in most cases these types are not needed, because WIG is creating inlines which convert to and from F# functions. However if you need to pass an array of JavaScript functions, it will use one of these types as automatically mapping an array would be confusing (arrays are mutable, we need to preserve identity).

* For JavaScript functions with 0 or 1 parameters, that do not use the `this` value, simply use F# functions (`'a -> 'b`).
* `FuncWithArgs<'TArgs, 'TResult>`: Represents a JavaScript function with more than one argument. The `'TArgs` type parameter must be a tuple.
* `FuncWithThis<'TThis, 'TFunc>`: Represents a JavaScript function that cares about the `this` value. `'TFunc` can be a straight F# function or other interop type.
* `FuncWithRest<..., 'TRest, 'TResult>`: Represents a JavaScript function that takes some fixed arguments, and then rest arguments. The number of fixed arguments can be between 0 and 6.
* `FuncWithArgsRest<'TArgs, 'TRest, 'TResult>`: This is for covering the probably rare case of more than 6 fixed arguments. `'TArgs` must be a tuple type of the fixed arguments. Its `.Call` method also takes the fixed arguments as a tuple.
* Delegate types are no longer supported for creating JavaScript functions using the `this` argument. Only explicitly proxied delegate types are translated. (WebSharper currently only defines proxy for `System.Action` which is used by `CancellationToken.Register`.) 

## Interface Generator API changes

* We felt that using `|+> [...]` and `|+> Protocol [...]` to add static and instance members accordingly is a bit non-intuitive, and also an easy source of errors, because forgetting Protocol would give no warning. Adding a list of members to a type definition is now recommended using `|+> Static [...]` and `|+> Instance [...]`. Previous syntax will give deprecated warning.
Also: constructors can be in the Instance list and they will still get handled correctly.
* Adding members to an interface is still done with `|+> [...]`.
* There have been functions called `Static` and `Instance` operating on a single method definition. These would have no use now and have been removed.
* You can use the `TSelf` value as a placeholder in member declarations and will be resolved to the declaring type.
* The `WithMacro` helper adds a WebSharper macro to a method/constructor. Currently the macro type itself must be defined in another assembly than the WIG generator assembly.
* `ObjectConstructor` creates a constructor where the inline is a simple JavaScript object expression. This functionality was previously exposed only by the `Pattern.Config` helper.
* Generic type definitions also use `Generic –` now instead of `Generic /`, and produce a single `CodeModel.TypeDefinition` value. This can be directly inserted into `CodeModel.Namespace` member lists. When you want to add specify type arguments, use the item syntax `MyTypeDef.[T1, T2, ...]` as you can with other types. The number of applied type parameters is not checked by the type system currently, but will give a detailed error when compiling with WIG. This includes wrongly parameterizing external types which would previously generate invalid IL.
* Use `Generic + ["T1"; "T2"] - ...` to specify the names of the type parameters.
* `Generic –` now takes a function with arguments of type `CodeModel.TypeParameter`. In some cases, explicitly declaring the argument type can be required for type inference. Also, sometimes using `p.Type` to convert a `CodeModel.TypeParameter` to a `Type.Type` is needed.
* There is a new overload of `Generic –` which takes an `int * (list<CodeModel.TypeParameter> -> #CodeModel.Entity)`, where the first value is the number of type parameters. This allows creating generic type declarations or members with more than 4 type parameters.
* You can now set type constraints on parameters using `p.Constraints <- [...]` inside the lambda passed to `Generic -`. Previous `WithConstraints` helper is removed as we want to have all helper functions named `With...` to be non-destructive.
* You can add a custom defined inline transformation with the `WithInterop` helper. This takes a record with an `In` and an `Out` field, both `string -> string`. For example:

        let Index = 
            T<int> |> WithInterop { 
                In  = fun s -> s + " - 1"
                Out = fun s -> s + " + 1"
            }

    You can then use this `Type.Type` value instead of `T<int>` in your member declarations where you want to handle an index as 1-based in your code, but pass it to and get it from a 0-based value in a JavaScript library. On method parameters and property setters the `In` function will be used on the parameter or property value in the automatic inline. On method return values and property getters the `Out` function will be used on the whole inline of the method or property getter.
* You can use the `WithNoInterop` helper to (non-destructively) clean any automatic and custom inline transformations from a `Type.Type` value.
* To define a custom inline for a method that still makes use of the default or custom inline transformations on parameters and return value, use the `WithInteropInline` helper. It takes a function typed `(string -> string) -> string`, use the provided function on a parameter name or index to get its transformed inline. For example, defining an `on` event setter with function argument detupling:

        "onChange" => (T<int> * T<obj> ^-> T<unit>)?callback ^-> T<unit>
        |> WithInteropInline (fun tr -> "$this.on('change', " + tr "callback" + ")"

* Similar helpers exists for property getters and setters: `WithInteropGetterInline` and `WithInteropSetterInline`. For getters, provided function is only usable for transforming `"index"` in the case of an indexed property, and for setters transforming `"value"` or `"index"`.
* You can now use the `!?` operator on types too, not just parameters.  Existing method definitions work without change as before, producing method overloads of different arity. On properties the option type is converted to and from an existing or missing field on a JavaScript object. On method returns, undefined is converted to `None`, all other values (including `null`) to `Some ...`.
* Union types (for example `T<int> + T<string>`) was used previously for creating method overloads, but defaulted to `T<obj>` when used on a property or method return type. Now these get automatically transformed to `Choice`  types when the cases can be distinguished in JavaScript using the `typeof`, `Array.isArray` and the `arr.length` functions. In F# this means either at most one array case or possibly multiple tuple cases with all different length, at most one number type (including `DateTime` and `TimeSpan`, which are proxied as a Number), `string`, `bool`, and at most one other object type. If there are cases which can't be separated, the type will still default to `obj`.

## Other improvements

* The `IntelliFactory.WebSharper.JavaScript.Object` type is now generic. `Object<obj>` is equivalent to previous `Object` type. Now, if relevant, you can specify the types of all fields, for example `Object<string>` for a JavaScript string map.
* You can use the `[<OptionalField>]` attribute on a class or record field with an option type to represent it with an existing/nonexisting field in the JavaScript translation. (Planned: `[<OptionalField>]` on a class or record will apply to all fields with an option type.)
* The `[<Name "">]` attribute now works on class fields.
* JQuery binding updated with better typing for AJAX operations.

## Fixes

* WIG couldn't create tuple types with length more than 7.
* [Bug #323](https://github.com/intellifactory/websharper/issues/323) Derived classes defining a field with the same name as a base class don't overlap in JavaScript translation.
* [Bug #330](https://github.com/intellifactory/websharper/issues/330) `withCredentials` should not be used in synchronous requests. Thanks to ca-ta for the report and pull request.

## Known issues

* Currently the TypeScript definition output can be off from the generated JavaScript as it was not reviewed yet with the latest changes.
* Source mapping at the current state is not as usable as we would like. Many-to-many mapping is not handled well by either IE11 or Chrome.
