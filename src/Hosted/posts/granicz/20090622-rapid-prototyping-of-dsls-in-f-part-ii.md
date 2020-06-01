---
title: "Rapid prototyping of DSLs in F# - Part II"
categories: "dsl,f#,websharper"
abstract: "The second part of \"Rapid prototyping of DSLs in F#\" presents the parser for Simply, a small programming language with functions and expressions."
identity: "1023,74604"
---
This is the second article in the "Rapid prototyping of DSLs in F#" series that summarizes the talks I gave at CEFP 2009 and goes beyond. Here is the short build-up of the series:


 * Part I - [Parsing with active patterns](//intellifactory.com/user/granicz/20090523-parsing-with-active-patterns) - parsing a small expression language
 * Part II - (current) Parser for Simply, a small programming language with functions and expressions
 * Part III - (coming soon) Semantic analysis for Simply programs
 * Part IV - (coming soon) Building a shell around Simply and instantiating it for a Logo-like language



![](/assets/Simply-Logo.png)

It has been almost three years since my ["A Logo interpreter under 400 lines"](http://www.intellifactory.com/articles/LogoInterpreter.aspx) article, a long time in any article's life. I am glad to see it being used in interesting ways by so many people (for example, [Kean Walmsley](http://through-the-interface.typepad.com/through_the_interface/) from Autodesk did a [3D implementation](http://through-the-interface.typepad.com/through_the_interface/2008/07/a-simple-3d-log.html) as an AutoCAD plugin), and I still get a few comments on it now and then.

The Logo-like language I implemented in the above article used a context-sensitive parser that accepted function calls only if the corresponding function definition was encountered beforehand - mostly to save some time and parse programs into a representation that was more readily executable without doing a separate phase of semantic checking.

For the CEFP 2009 example I wanted to get back to a more conventional parsing implementation and keep syntax and semantics separate. The goal of the example was to implement/establish a small programming language that had means to define and call functions, a simple control flow construct (say, repeat blocks), and support for at least the ordinary arithmetic operators with the usual precedences in its expression language.

Here is a bit of sample code from Simply:

```fsharp
var x=1
var x=2
var x=3

fun foo(y) {
    fun bar(foo) {
        var x=x+1
        foo+x
    }
    bar(y*2)
}

repeat 1000 as x {
    foo(x)
}
```

The parser I developed in the [first article](//intellifactory.com/user/granicz/20090523-parsing-with-active-patterns) of this series provides the perfect foundation to build upon. If you have played with that parser and want to make it work with this and the upcoming articles, you should wrap it inside a Language module, something like this:

```fsharp
namespace IntelliFactory.Simply

module Language =
    open System
    open System.Text.RegularExpressions

    let private matchToken pattern s =
        ...
```

The expression language defined there only covered arithmetic on floating-point numbers and variables. For the purposes of this series, Simply will support the same - so the only values in Simply are floats, and as a result the language will not include conditionals (and thus no useful recursive functions either).

But don't worry - adding multiple types and conditionals to Simply is relatively straightforward, and I will address this as an appendix to the series.

Recall that I used the Ast module to contain the type that the parser used for encoding expressions. Here I am going to change that representation slightly and tag a couple additional types for encoding commands and programs:

```fsharp
module Ast =
    type var = string

    type Expr =
        | Number   of float
        | BinOp    of (float -> float -> float) * Expr * Expr
        | Var      of var
        | FunApply of var * Expr list
        
        static member Sum (e1, e2)   = BinOp (( + ), e1, e2)
        static member Diff (e1, e2)  = BinOp (( - ), e1, e2)
        static member Prod (e1, e2)  = BinOp (( * ), e1, e2)
        static member Ratio (e1, e2) = BinOp (( / ), e1, e2)

    type Command =
        | Repeat   of Expr * var * Command
        | FunDef   of var * var list * Command
        | Sequence of Expr list
        | VarDef   of var * Expr
        | Yield    of Expr

    type Prog = Program of Command list
```

I can now add the parser rules for commands and entire programs. First, let's add the rules for the tokens Simply uses:

```fsharp
    let (|LBRACE|_|) s = "{"      |> MatchSymbol s
    let (|RBRACE|_|) s = "}"      |> MatchSymbol s
    let (|EQ|_|)     s = "="      |> MatchSymbol s

    let (|VAR|_|)    s = "var"    |> MatchSymbol s
    let (|FUN|_|)    s = "fun"    |> MatchSymbol s
    let (|REPEAT|_|) s = "repeat" |> MatchSymbol s
    let (|AS|_|)     s = "as"     |> MatchSymbol s
```

Commands differ from expressions in that they don't have a return value, and Simply programs are basically a list of commands. A command can be a variable or function definition, or a repeat block. Commands can also be sequenced (this is how you define function bodies), and they can invoke any expression (Yield here is not exactly a return command - it is simply a tag to stand for executing arbitrary expressions.)

```fsharp
    let rec (|Command|_|) = function
        | LBRACE (Star (|Command|_|) [] (commands, RBRACE rest)) ->
            (Ast.Command.Sequence commands, rest) |> Some
        | REPEAT (Expression (i, AS (ID (v, Command (body, rest))))) ->
            (Ast.Command.Repeat (i, v, body), rest) |> Some
        | FUN (ID (f, LPAREN (Star (|ID|_|) [] (pars, RPAREN (Command (body, rest)))))) ->
            (Ast.Command.FunDef (f, pars, body), rest) |> Some
        | VAR (ID (v, EQ (Expression (expr, rest)))) ->
            (Ast.Command.VarDef (v, expr), rest) |> Some
        | Expression (e, rest) ->
            (Ast.Command.Yield e, rest) |> Some
        | _ ->
            None

    let (|Prog|_|) = function
        | Star (|Command|_|) [] (commands, rest) ->
            (Ast.Prog.Program commands, rest) |> Some
        | _ ->
            None
```

With these handful of rules, I can now parse Simply programs. Here is a short F# Interactive session (slightly modified for better readability) for parsing the example Simply program from earlier:

```fsharp
open Language
    
"
var x=1
var x=2
var x=3

fun foo(y) {
    fun bar(foo) {
        var x=x+1
        foo+x
    }
    bar(y*2)
}

repeat 1000 as x {
    foo(x)
}"
|> (|Prog|_|)
|> printf "Result=%A\n";;

val it : unit = ()
> 
Result=Some
  (Program
     [VarDef ("x",Number 1.0);
      VarDef ("x",Number 2.0);
      VarDef ("x",Number 3.0);
      FunDef
        ("foo",["y"],
         Sequence
           [FunDef
              ("bar",["foo"],
               Sequence
                 [VarDef ("x",BinOp (<fun:Sum@44>,Var "x",Number 1.0));
                  Yield (BinOp (<fun:Sum@44>,Var "foo",Var "x"))]);
            Yield (FunApply ("bar",[BinOp (<fun:Prod@46>,Var "y",Number 2.0)]))]);
      Repeat (Number 1000.0,"x",Sequence [Yield (FunApply ("foo",[Var "x"]))])],
   "")
>
```

This completes the parser for Simply. In the next couple articles I will look at semantic analysis and evaluating of Simply programs, loading up the evaluation environment with useful built-in functions, and erecting a simple development shell around Simply to instantiate for different target domains (Logo in particular). So stay tuned!
