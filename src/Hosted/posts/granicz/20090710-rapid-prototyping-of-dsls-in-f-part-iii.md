---
title: "Rapid prototyping of DSLs in F# - Part III"
categories: "dsl,f#,websharper"
abstract: "The third part of \"Rapid prototyping of DSLs in F#\" presents the semantic checker for Simply, a small programming language with functions and expressions."
identity: "1022,74603"
---
Consider the following short Simply program:

```fsharp
var x = 2
fun x(a b) { a + b + x  }
fun x(y)   { y + x(1 2) }

x(3)
```

Here "x" is defined multiple times: first it is assigned the value 2. Then it is redefined as a function taking two arguments, adding them together and incrementing the result with the previous value of "x". It is again redefined as a new function taking a single argument and incrementing it with the result of the previous definition called with arguments 1 and 2. The point to take away from here is that "x" has different meanings in each line, and it is non-trivial to evaluate such code unless we perform some "magic" and verify that each reference to "x" is a valid one and signal an error otherwise. This is called semantic checking.

Normally semantic checking includes type checking, this is the phase in an interpreter or compiler when values are checked against type expectations of consuming functions or signatures. For instance, when you write "1 @ [ 2 ]" in F#, the compiler verifies that the "@" function (whose signature is 'A -> 'A list -> 'A list) can be called with parameters of type int and int list. Since these fit the more general polymorphic type signature, the compiler happily accepts this expression. However, when you try to add a string and an integer together, the compiler will flag it as an error because it can not find an instance of the plus operator (a function) that adds values of disparate types.

Recall however that in Simply the only values allowed are floating-point numbers (I will add more types, conditionals and recursive functions in an appendix to the series), and thus type checking Simply programs is a no-op. However, the "core" of the magic I talked about above are still to be done, so let's just jump right into it.

Now that I have a full parser for Simply from [Part II](//intellifactory.com/user/granicz/20090622-rapid-prototyping-of-dsls-in-f-part-ii), I am going to do two things on an AST value in a single sweep: rename variables and functions to avoid name collisions, and verify that variable and function references (and the arity of function calls) are valid. If you are coming from a functional programming background you may wonder about that last bit, but since Simply values are floating-point only the language can not have partial function calls. This limitation can be removed once a more elaborate type system (and closures) is added. So here is the first bif of the Standardize module:

```fsharp
namespace IntelliFactory.Simply

module Standardize =
    open IntelliFactory.Simply.Ast
    
    type Arity =
        | NoArity
        | Arity of int

    module Env =
        let EnvAdd env (v: string, arity: Arity) =
            Map.tryFind v env
            |> function
                | None ->
                    0
                | Some (_, i) ->
                    i+1
            |> fun idx ->
                Map.add v (arity, idx) env, idx |> sprintf "%s%d" v

        let EnvRefresh env (v: string) =
            Map.tryFind v env
            |> function
                | None ->
                    v |> sprintf "The variable '%s' is not defined" |> failwith
                | Some (_, idx) ->
                    idx |> sprintf "%s%d" v

        let EnvArity env (v: string) =
            Map.tryFind v env
            |> function
                | None ->
                    v |> sprintf "The function '%s' is not defined" |> failwith
                | Some (arity, _) ->
                    arity
```

This contains an inner Env module that supplies various utility functions: EnvAdd to add a new binding with its name and arity (variables have Arity.NoArity), EnvRefresh to return the latest binding to a given concrete variable name (I simply number these bindings from 0 - see EnvAdd above), and EnvArity to return the arity of the latest binding of a given variable.

All three functions operate on environments. Internally, these are represented as a map from strings (concrete variable names) to arity*index pairs, where index is simply an integer suffix that gets appended to the original variable name to form a fresh binding (and thus the parser doesn't allow variable names to contain numeric characters). The rest of the standardization module is:

```fsharp
    let private thread f acc (lst: 'a list) =
        lst
        |> List.fold (fun (acc1, acc2) el ->
            let _acc1, _acc2 = f acc1 el
            _acc1, _acc2 :: acc2) (acc, [])
        |> fun (acc1, acc2) ->
            acc1, List.rev acc2
    let rec ConvertProgram env (Ast.Program commands) =
        commands
        |> thread CC env
        |> fun (env, commands) ->
            env, commands |> Ast.Program
    and private CC env command =
        match command with
        | Ast.Command.Repeat (expr, v, body) ->
            let _env, _v = Env.EnvAdd env (v, Arity.NoArity)
            env, Ast.Command.Repeat (CE env expr, _v, CC _env body |> snd)
        | Ast.Command.FunDef (f, pars, body) ->
            let _env, _f =
                Env.EnvAdd env (f, pars |> List.length |> Arity.Arity)
            pars
            |> List.map (fun par -> par, Arity.NoArity)
            |> thread Env.EnvAdd _env
            |> fun (envPars, parsP) ->
                _env, Ast.Command.FunDef (_f, parsP, CC envPars body |> snd)
        | Ast.Command.Sequence commands ->
            commands
            |> thread CC env
            |> fun (_, commands) ->
                env, Ast.Command.Sequence commands
        | Ast.Command.VarDef (v, expr) ->
            let _env, _v = Env.EnvAdd env (v, Arity.NoArity)
            _env, Ast.Command.VarDef (_v, CE env expr)
        | Ast.Yield e ->
            env, Yield (CE env e)
    and private CE env expr =
        match expr with
        | Ast.Expr.Number n ->
            Ast.Expr.Number n
        | Ast.Expr.Var v ->
            v
            |> Env.EnvArity env
            |> function
                | NoArity ->
                    v
                    |> Env.EnvRefresh env
                    |> Ast.Expr.Var
                | Arity _ ->
                    v
                    |> sprintf "'%s' is a function and needs arguments"
                    |> failwith
        | Ast.Expr.BinOp (f, e1, e2) ->
            Ast.Expr.BinOp (f, CE env e1, CE env e2)
        | Ast.Expr.FunApply (f, args) ->
            Env.EnvRefresh env f
            |> fun _f ->
                f
                |> Env.EnvArity env
                |> function
                    | Arity ar when ar = List.length args ->
                        Ast.Expr.FunApply (_f, List.map (CE env) args)
                    | _ ->
                        f
                        |> sprintf "Arity mismatch on calling '%s'"
                        |> failwith
```

Having the Ast, Language and Standardize modules loaded up in F# Interactive allows me to test the standardizer quickly:

```fsharp
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
|> fun s ->
    match s with
    | Language.Prog (prog, Language.Eof) ->
        prog
        |> Standardize.ConvertProgram Map.Empty
        |> fun (_, _prog) ->
            _prog |> printf "Result=%A\n"
    | _ ->
        printf "Syntax error\n";;
```

gives:

```fsharp
> 
Result=Program
  [VarDef ("x0",Number 1.0); VarDef ("x1",Number 2.0); VarDef ("x2",Number 3.0);
   FunDef
     ("foo0",["y0"],
      Sequence
        [FunDef
           ("bar0",["foo1"],
            Sequence
              [VarDef ("x3",BinOp (<fun:Sum@44>,Var "x2",Number 1.0));
               Yield (BinOp (<fun:Sum@44>,Var "foo1",Var "x3"))]);
         Yield (FunApply ("bar0",[BinOp (<fun:Prod@46>,Var "y0",Number 2.0)]))]);
   Repeat (Number 1000.0,"x3",Sequence [Yield (FunApply ("foo0",[Var "x3"]))])]
val it : unit = ()
>

```

Which if I had a pretty-printer would correspond to:

```fsharp
var x0=1
var x1=2
var x2=3

fun foo0(y0) {
    fun bar0(foo1) {
        var x3=x2+1
        foo1+x3
    }
    bar0(y0*2)
}

repeat 1000 as x3 {
    foo0(x3)
}
```

Now bindings and their usage sites are clearly coupled, arities are checked on function calls, and variable and function names are checked for existence. With this I have just enough to implement an evaluator that comes with some built-in functions, and plug it in into a small shell that enables me to write and test Logo-flavour Simply programs. So stay tuned for Part IV of this series!
