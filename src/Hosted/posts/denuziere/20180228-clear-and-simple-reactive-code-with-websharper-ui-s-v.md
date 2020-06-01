---
title: "Clear and simple reactive code with WebSharper.UI's V"
categories: "ui,fsharp,reactive,websharper"
abstract: "Reactive code can be just as clear and readable as static code with this new WebSharper.UI feature!"
identity: "5508,84675"
---
Writing reactive code in WebSharper.UI can result in the use of a lot of functions and combinators, and code that is more verbose than a static counterpart would be. However, a recent new feature can help reduce most of this overhead: the `V` property. Let's get started with it!

> Note: examples in this article are given in F#; but all the concepts are applicable in C#, except the final part about V and lenses, which is only compatible with F# record types.

## The working example

Here is a simple static form asking the user for their name. It has two input fields and a paragraph showing the value. Obviously this last part is not very useful yet since this is only a static form; it just always shows whatever was the initial value.

```fsharp
type FullName =
    {
        First: string
        Last: string
    }

let promptName (name: FullName) =
    form [] [
        input [attr.value name.First] []
        input [attr.value name.Last] []
        p [] [text (sprintf "Hello, %s %s!" name.First name.Last)]
    ]
```

[See it in action](https://try.websharper.com/snippet/loic.denuziere/0000In)

Over the course of this article, we'll transform it into a dynamic form where the paragraph reacts to the user's inputs. Spoiler alert: here's what our final code will look like.

```fsharp
let promptName (name: Var<FullName>) =
    form [] [
        Doc.Input [] (Lens name.V.First)
        Doc.Input [] (Lens name.V.Last)
        p [] [text (sprintf "Hello, %s %s!" name.V.First name.V.Last)]
    ]
```

[See it in action](https://try.websharper.com/snippet/loic.denuziere/0000Ir)

I would argue that the above is exactly as readable as the original static code, while providing extra reactive functionality. Now let's see how the sausage is made!

## A first, unmodular attempt

Before we try to make our little function reactive, here's a refresher on how reactive code works in WebSharper. Fundamentally, it is based on the combination of two types:

* `Var<'T>` is a mutable cell containing a value of type `'T`. It's basically equivalent to a `ref<'T>`, except that it can be reactively observed by a...
* `View<'T>`, which is a read-only reactive `'T`.

A number of functions integrate these into the page. For example:

* `Doc.Input` is similar to the regular `input` function that creates an `<input>` element, but it takes a `Var<string>` and binds itself to it. When the user types, the `Var` is updated, and when the `Var` changes due to other code, the input box is updated.
* `textView` shows text content, similar to `text`, except it's reactive: the text content changes whenever the `View<string>` changes.

Armed with these, let's write our first attempt at a reactive form.

```fsharp
    let promptName (first: Var<string>) (last: Var<string>) =
        form [] [
            Doc.Input [] first
            Doc.Input [] last
            p [] [
                textView (
                    View.Map2
                        (fun f l -> sprintf "Hello, %s %s!" f l)
                        first.View last.View
                )
            ]
        ]
```

[See it in action](https://try.websharper.com/snippet/loic.denuziere/0000Ik)

`View.Map2` creates a new `View` by applying a function to the values of two input `View`s.

This works well enough, but there are several improvements we could make.

* First, we completely dropped our `FullName` record, and instead we are taking the two fields as separate `Var`s. This is pretty bad for the maintainability of our code. `FullName` is probably a type from our application's model: we pass `FullName` values around, maybe even between the server and the client. We don't want to have to convert it into its components and then put them back together whenever we want the user to interact with it.
* Second, this is a bit verbose, isn't it? We have to call another function and write an anonymous function just to display two strings. That feels overkill.

## Deriving `Var`s with Lenses

Let's attack the first problem. Ideally, we'd like our function to receive a `Var<FullName>` and be able to deal with it. We want each of our input boxes to transparently update the corresponding field of this `Var`. This is what **lenses** are for.

Lenses are a way to "zoom in" (hence the name) on a part of a `Var`, such as a field of a record. You provide a getter (a function that retrieves the field value from the full value) and a setter (a function that takes an old full value and a new field value, and returns the new full value) and it gives you back a new `Var` for your field.

```fsharp
let promptName (name: Var<FullName>) =
    let first = Var.Lens name (fun n -> n.First) (fun n f -> { n with First = f })
    // ...
```

The important aspect of this is that the reactivity works as you would expect: if you set the value of the full `Var`, the field `Var`'s own `View` is updated, and vice-versa.

Armed with this new tool, we can write a better version of `promptName` that uses `FullName`.

```fsharp
let promptName (name: Var<FullName>) =
    let first = Var.Lens name (fun n -> n.First) (fun n f -> { n with First = f })
    let last = Var.Lens name (fun n -> n.Last) (fun n l -> { n with Last = l })
    form [] [
        Doc.Input [] first
        Doc.Input [] last
        p [] [
            textView (
                View.Map2
                    (fun f l -> sprintf "Hello, %s %s!" f l)
                    first.View last.View
            )
        ]
    ]
```

[See it in action](https://try.websharper.com/snippet/loic.denuziere/0000Il)

This works nicely! But... we've made the verbosity problem worse, haven't we? ☹️

## Introducing `V`

We'd like to be able to compose together `View`s as naturally as we would compose simple values. Instead of this:

```fsharp
View.Map2
    (fun f l -> sprintf "Hello, %s %s!" f l)
    first.View last.View
```

We want to write this:

```fsharp
sprintf "Hello, %s %s!" first.View last.View
```

Of course, that isn't quite possible directly &mdash; the types don't even match! `first.View` has type `View<string>`, but `sprintf` wants a `string`.

This is where WebSharper introduces a bit of "magic". The type `View<'T>` has a field called `V`, of type `'T`. Now, `'T` is not a reactive type, so this field should only be able to return one value: the current value of our `View` at the moment of calling `.V`. But when you use `.V` in an argument to certain special functions, WebSharper automatically transforms this argument into the appropriate calls to `View.Map`, `View.Map2`, or whatever else is needed to build a reactive value from this expression.

`text` is one such special function. When its argument contains uses of `.V`, it is compiled into a call to `textView` and the argument is transformed to be properly reactive. So this:

```fsharp
// Assuming you have a value myView: View<string>

text ("myView's current value is " + myView.V)
```

is exactly equivalent to this:

```fsharp
textView (myView |> View.Map (fun x -> "myView's current value is " + x))
```

That's much more readable, don't you think? It allows us to simplify our display function tremendously:

```fsharp
let promptName (name: Var<FullName>) =
    let first = Var.Lens name (fun n -> n.First) (fun n f -> { n with First = f })
    let last = Var.Lens name (fun n -> n.Last) (fun n l -> { n with Last = l })
    form [] [
        Doc.Input [] first
        Doc.Input [] last
        p [] [
            text (sprintf "Hello, %s %s!" first.View.V last.View.V)
        ]
    ]
```

In fact, we can simplify it even further, as `Var` also has a field `.V`, which is equivalent to `.View.V`.

```fsharp
let promptName (name: Var<FullName>) =
    let first = Var.Lens name (fun n -> n.First) (fun n f -> { n with First = f })
    let last = Var.Lens name (fun n -> n.Last) (fun n l -> { n with Last = l })
    form [] [
        Doc.Input [] first
        Doc.Input [] last
        p [] [
            text (sprintf "Hello, %s %s!" first.V last.V)
        ]
    ]
```

[See it in action](https://try.websharper.com/snippet/loic.denuziere/0000Ip)

> Side note: earlier I said that `.V` "should only be able to return one value: the current value of our `View` at the moment of calling `.V`". That's the only thing it _could_ do, if it *did* anything; but in fact, it causes a compile error. That's because `.V` is really intended to simplify reactive code; so I made sure that people wouldn't end up with non-reactive code by mistake. Plus, there are some cases (in particular when using `View.MapAsync`) where it is simply not possible to extract the current value of a `View`.

## Using `.V` with lenses

Now let's look into simplifying our lenses. They really feel like boilerplate: getting and setting a field in a record is a very uniform pattern. Can WebSharper's "magic" help us with it? As it turns out, yes it can.

As said before, `Var` also has a `.V` field. In addition to serving as a shortcut for `.View.V`, it can also be used to write lenses very concisely, in conjunction with the `Lens` function. So this:

```fsharp
Lens name.V.First
```

is exactly equivalent to this:

```fsharp
Var.Lens name (fun n -> n.First) (fun n f -> { n with First = f })
```

Note that, at least for the moment, `Lens` is only able to deal with record fields.

This allows us to simplify our code further:

```fsharp
let promptName (name: Var<FullName>) =
    let first = Lens name.V.First
    let last = Lens name.V.Last
    form [] [
        Doc.Input [] first
        Doc.Input [] last
        p [] [
            text (sprintf "Hello, %s %s!" first.V last.V)
        ]
    ]
```

Now, this final step is more subjective: I think that in the above code, the definitions of `first` and `last` have become short enough that it's a bit redundant to give them names at all. I would shorten it to the following:

```fsharp
let promptName (name: Var<FullName>) =
    form [] [
        Doc.Input [] (Lens name.V.First)
        Doc.Input [] (Lens name.V.Last)
        p [] [text (sprintf "Hello, %s %s!" name.V.First name.V.Last)]
    ]
```

[See it in action](https://try.websharper.com/snippet/loic.denuziere/0000Ir)

And here we are! Our reactive code is now clear and maintainable.
