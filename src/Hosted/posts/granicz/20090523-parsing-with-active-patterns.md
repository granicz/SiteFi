---
title: "Parsing with active patterns"
categories: "activepatterns,parsing,f#"
abstract: "Active patterns provide the perfect mechanism to implement recursive-descent parsers - allowing you to quickly prototype even complex grammars using a statically typed approach and without the need to resort to lexer and parser generators."
identity: "1024,74605"
---
I was giving an F# warm-up a couple days ago at [CEFP 2009](http://www.inf.elte.hu/english/conf/tfp_cefp_2009/Lapok/index.aspx), and one of the examples I was giving was on active patterns and their application for Parsing Expression Grammars (PEGs). In short, PEGs are a way to specify grammars using functions that either succeed or fail, and combining them to form a recursive-descent parser.

The cool thing about active patterns is that they provide the perfect mechanism to implement PEGs - and at the same time allowing you to quickly prototype even complex grammars using a statically typed approach and without the need to resort to lexer and parser generators.

If you are interested in parsing with active patterns, there are some great articles [here</a> (be sure to check out DewHawk's other articles on this topic) and <a href="http://langexplr.blogspot.com/2008/11/using-mgrammar-parser-from-f.html">here](http://devhawk.net/2008/01/29/Practical+F+Parsing+Recursion+And+Predicate+Functions.aspx) (Luis has some other great F# posts also).

```fsharp
    open System
    open System.Text.RegularExpressions

    let matchToken pattern s =
        Regex.Match(s, pattern |> sprintf "\A(%s)((?s).*)", RegexOptions.Multiline)
        |> fun mtch ->
            if mtch.Success then
                (mtch.Groups.[1].Value, mtch.Groups.[2].Value) |> Some
            else
                None
```

This defines a `matchToken` function that takes a regex pattern and an input string and tries to apply the regex on that input. Note that you are wrapping the regex pattern into two groups - one for the input pattern (%s) and another for the remaining input string ((?s).*). (\A specifies that you are matching from the beginning of the input string, and (?s) instructs the matcher to match irrespective of new line characters.)

You can now express whitespace and one-line comments (just to be different we use # for these) easily:

```fsharp
    let (|WS|_|) s =
        matchToken "[ |\t|\n|\r\n]+" s
    
    let (|COMMENT|_|) s =
        matchToken "#.*[\n|\r\n]" s
```

Using these you can define whitespace as:

```fsharp
    let (|WHITESPACE|_|) s =
        match s with
        | WS rest ->
            rest |> Some
        | COMMENT rest ->
            rest |> Some
        | _ ->
            None
```

This says that a WHITESPACE is either whitespace (space, tab, LF, or CR+LF) or a comment. Note that you defined whitespace (WS) with a regex that matches any number of whitespace characters, but even with that WHITESPACE won't match things like " # one-line comment \n " (whitespace and comments following each other), simply because WHITESPACE will either match a WS *or* a COMMENT. (Remember that this is why active patterns are great to develop PEGs - there is no ambiguity in parsing: once a match is made, all other choices are ignored.)

What you need is the * operator from ordinary BNF syntax: this matches zero or more occurences of something.

```fsharp
    let rec (|Star|_|) f acc s =
        match f s with
        | Some (res, rest) ->
            (|Star|_|) f (res :: acc) rest
        | None ->
            (acc, s) |> Some
```

The way you accomplish this is trying to parse a new symbol (via f), and if you succeed you recurse on the remaining input string with the matched input accumulated. If matching fails you simply return what you accumulated in preceeding calls. (Note that this function never fails - if you need "one or more" matches you need to modify the None clause to return None if the first match was unsuccessful.) Normally, you would want to call (|Star|_|) with an empty list at first:

```fsharp
    let (|WhiteSpace|_|) s = (|Star|_|) (|WHITESPACE|_|) [] s
```

This just says that WhiteSpace matches zero or more WHITESPACEs and will return a string list of these whitespace blocks and the remaining input string.
Armed with WhiteSpace you can implement your basic "tokenizer." For most grammars whitespace is ignored (a notable exception here is F# where indentation does matter), and in general it is much more tedious to treat whitespace as separate tokens.

```fsharp
    let rec MatchTokenNoWS wspattern s pattern =
        match (|WhiteSpace|_|) s with
        | Some (_, rest) ->
            rest |> matchToken pattern
        | None ->
            s |> matchToken pattern
```

This function simply removes any leading whitespace before matching a regex pattern. You can now write a function that parses a token for a given regex as:

```fsharp
    let MatchToken s f pattern =
        pattern |> MatchTokenNoWS s |> Option.bind f
```

Here the parameter f carries the function that is executed on the match - which, has type (string * string), where the first value in the tuple is the string matched, the second is the remaining input string.

For tokens where you don't need the matched string (operators, keywords, etc.) you can write a function that returns only the rest of the input:

```fsharp
    let MatchSymbol s pattern =
        pattern |> MatchToken s (fun (_, rest) -> rest |> Some)
```

Now you have all the primitives to start building your parsers. Consider a simple language of arithmetic expressions:

 * Numbers
 * Arithmetic (addition, subtraction, multiplication, division)
 * Functions (sin, cos, etc.)

A straightforward translation of this small language into F# types would be:

```fsharp
module Ast =
    type var = string

    type Expr =
    | Number   of float
    | Sum      of Expr * Expr
    | Diff     of Expr * Expr
    | Prod     of Expr * Expr
    | Ratio    of Expr * Expr
    | Var      of var
    | FunApply of var * Expr list
```

The parser is strikingly straightforward as well:

```fsharp
    let (|NUMBER|_|) s =
        "[0-9]+\.?[0-9]*" |> MatchToken s
            (fun (n, rest) -> (n |> Double.Parse, rest) |> Some)

    let (|ID|_|) s =
        "[a-zA-Z]+" |> MatchToken s (fun res -> res |> Some)

    let (|PLUS|_|)   s = "\+" |> MatchSymbol s
    let (|MINUS|_|)  s = "-"  |> MatchSymbol s
    let (|MUL|_|)    s = "\*" |> MatchSymbol s
    let (|DIV|_|)    s = "/"  |> MatchSymbol s
    let (|LPAREN|_|) s = "\(" |> MatchSymbol s
    let (|RPAREN|_|) s = "\)" |> MatchSymbol s
    
    let rec (|Factor|_|) = function
    | NUMBER (n, rest) ->
        (Ast.Expr.Number n, rest) |> Some
    | ID (f, LPAREN (Star (|Expression|_|) [] (args, RPAREN rest))) ->
        (Ast.Expr.FunApply (f, args), rest) |> Some
    | ID (v, rest) ->
        (Ast.Expr.Var v, rest) |> Some
    | _ ->
        None
        
    and (|Term|_|) = function
    | Factor (e1, MUL (Term (e2, rest))) ->
        (Ast.Expr.Prod (e1, e2), rest) |> Some
    | Factor (e1, DIV (Term (e2, rest))) ->
        (Ast.Expr.Ratio (e1, e2), rest) |> Some
    | Factor (e, rest) ->
        (e, rest) |> Some
    | _ ->
        None

    and (|Sum|_|) = function
    | Term (e1, PLUS (Sum (e2, rest))) ->
        (Ast.Expr.Sum (e1, e2), rest) |> Some
    | Term (e1, MINUS (Sum (e2, rest))) ->
        (Ast.Expr.Diff (e1, e2), rest) |> Some
    | Term (e, rest) ->
        (e, rest) |> Some
    | _ ->
        None

    and (|Expression|_|) = (|Sum|_|)
```

One last bit that you will need for practical parsing is checking for EOFs - afterall full programs are only valid if the entire input string was consumed.

```fsharp
    let (|Eof|_|) s =
        if s |> String.IsNullOrEmpty then
            () |> Some
        else
            match s with
            | WhiteSpace (_, rest) when rest |> String.IsNullOrEmpty ->
                () |> Some
            | _ ->
                None
```

You can quickly test your code in F# Interactive:

```fsharp
> match "sin(1 + 2 * 3)" with
    | Expression (e, Eof) ->
        e |> printf "Match – AST: %A\n“
    | _ ->
        printf "No match\n”;;

> Match – AST: FunApply ("sin",[Sum (Integer 1,Prod (Integer 2,Integer 3))])
```

You can also call active pattern recognizers directly and this can be tremenedously useful when debugging your parser rules:

```fsharp
> (|Expression|_|) "cos(1+2*3-4)";;
val it : (Ast.Expr * string) option =
  Some
    (FunApply
       ("cos",
        [Sum (Number 1.0,Diff (Prod (Number 2.0,Number 3.0),Number 4.0))]), "")
```