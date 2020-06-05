---
title: "A Logo interpreter under 400 lines"
categories: "tutorial,f#,parsing,logo"
abstract: "This first time I picked something that F# is particularly good at: implementing other programming languages. The target is Logo, your favorite turtle graphics language from grade school - mostly because it has something for everyone: it has some peculiarities from the language design angle that are worth looking at, but foremost the implementation once again shows you how easy it is to get things done with F#. So I sat down yesterday night and wrote a small Logo interpreter."
identity: "-1,56781"
---
This first time I picked something that F# is particularly good at: implementing other programming languages. The target is Logo, your favorite turtle graphics language from grade school - mostly because it has something for everyone: it has some peculiarities from the language design angle that are worth looking at, but foremost the implementation once again shows you how easy it is to get things done with F#. So I sat down yesterday night and wrote a small Logo interpreter.

Our application has two parts: the core language module (`logo.fs`) that defines all types, the lexer, the parser and the evaluator; and the UI client (`main.fs`) that constructs the main application form and adds all interaction within.

Here is what our final application will look like (after you ran some Logo code ;)):

![LogoInterpreter01](/assets/logo01.jpeg)

Logo is an interesting language because it is strikingly simple yet quite powerful and it is an excellent medium to teach kids how to program. There are a lot of implementations out there and here you will see mine without any desire to adhere to often used or even standard features - my objective is simply to be able to write short Logo programs that create nice drawings.

### The language

Logo programs are made up of words; these can be commands (that do not return a value) or functions (that return words), both operating on other words. Numbers are special words. Lists of expressions are enclosed in brackets and the value of a list is the value of the last item obtained by evaluating all of list elements first. Arguments to words having a meaning are passed "by-value" and using **prefix** notation. Our implementation supports the following "built-in" words:

#### Commands and functions

`Canvas` e1 e2, `Left` (`Lt`) e1, `Right` (`Rt`) e1, `Forward` (`Fd`) e1, `PenUp`, `PenDown`, `Repeat` e1 e2, `Sum` (`+`) e1 e2, `Minus` (`-`) e1 e2, `Times` (`*`) e1 e2, `Divide` (`/`) e1 e2, `Min` e1 e2, `Max` e1 e2, `Sin` e1, `Cos` e1, `Tan` e1, `Pi`, `RepCount`, `To`

#### Defining new words

You can define new words (functions or commands) using `To`:

```
TO add [:x :y] + :x :y
```

Single parameters can be given using a variable

```
TO double :x + :x :x
```

#### The language module

First, we start by opening a few namespaces and declaring some basic types.

```fsharp
module IntelliFactory.Languages.Logo

open System.IO
open Printf
open String
open System.Collections.Generic
open System.Windows.Forms
open System.Drawing
open System.Drawing.Imaging

type num = float

type source = {
   reader : IEnumerator<TOKEN>;
   pos : pos
}
and ppos = {
   mutable x' : int;
   mutable y' : int;
}
and pos = {
   x : int;
   y : int;
}
and token =
 | WORD of string * pos
 | VAR of string * pos
 | NUMBER of num * pos
 | LBRACK of pos
 | RBRACK of pos
 | HOLE of pos
with
   member self.IsRBrack = match self with
    | RBRACK _ -> true | _ -> false
end
```

Here, the `source` type will correspond to a stream of tokens and the current position. Each token can be a word (such as `repeat`), a variable (such as `:x`), a number, or one of the bracket characters, or a "hole" (the `?` symbol). At a very basic level, everything is a word, but we attach the usual semantics to numbers and the value-of semantics to variables. The bracket characters are used to construct lists or sequences that can in turn contain other expressions. The hole symbol is actually not used in this basic implementation.

Our lexer will construct a stream of tokens that will be iterated through by the parser function, and as we consume a sequence of meaningful tokens we output an "instruction" that can be interpreted later. This instruction type is defined next:

```fsharp
type instr =
 | Hole of pos
 | Number of num * pos
 | Var of string * pos
 | List of instr list * pos
 | Canvas of instr * instr * pos
 | Binop of (num -> num -> num) * instr * instr * pos
 | Unop of (num -> num) * instr * pos
 | Left of instr * pos
 | Right of instr * pos
 | Forward of instr * pos
 | PenUp of pos
 | PenDown of pos
 | Repeat of instr * instr * pos
 | To of string * string list * instr * pos
 | FunCall of string * instr list * pos
with
   member self.Pos = match self with
    | Var (_, pos) | List (_, pos)
    | Hole pos | Number (_, pos)
    | Binop (_, _, _, pos) | Unop (_, _, pos)
    | Canvas (_, _, pos) | Left (_, pos)
    | Right (_, pos) | Forward (_, pos)
    | PenUp pos | PenDown pos
    | Repeat (_, _, pos) | To (_, _, _, pos)
    | FunCall (_, _, pos) ->
         pos
end
```

This pretty much tells you what our Logo machine can interpret (holes are again here, but we won't work with them). We will review the concrete syntax for the instruction set shortly. Before we do that we need to define what our evaluator will work with:

```fsharp
type state = {
   funs : Map<STRING, instr * list string>;
   vars : Map<STRING, num>;
} with
   static member Default = {
      funs = Map.Empty ();
      vars = Map.Empty ();
   }
end

type gstate = {
   currX : float;
   currY : float;
   penDown : bool;
   angle : float;
   graphics : Graphics;
} with
   static member Default = {
      currX = 0.0;
      currY = 0.0;
      penDown = true;
      angle = 90.0;
      graphics = null;
   }
end
```

The `state` type encapsulates the runtime environment, and stores a map of functions and variables in scope. Logo uses dynamic scoping, so a given variable or function name will refer to the "latest" binding site, so the following is completely valid and sensible:

```
to add :x + :x :y
```

This defines a new function called `add` that takes one parameter called `x`, adds its value to (the value of) `y` and returns this sum. We don't care that `y` may be unbound as it is expected that by the time we call the add function we will have a binding for `y`. If not, we get a runtime error.

As we evaluate our Logo programs, we will be changing the "graphics" state, e.g. the current position on the canvas, whether our pen is placed on the canvas or not, what the turtle's orientation is and what canvas we are drawing on. These are all represented in the `gstate` type.

Now, onto our lexer. We won't use fslex or fsyacc for this project as lexing and parsing Logo programs is unconventional. While the lexer is fairly simple, the parser itself constructs Logo instructions depending on the tokens it sees and the semantics each has. So after parsing a function definition, a call to that function will have to be parsed taking into consideration the number of arguments; e.g. our parser has a state and decides what each encountered word means and how to parse the tokens that follow.

```fsharp
let isEof (stream: #StringReader) = stream.Peek() = -1

let rec neq_peek allowEof (stream: #StringReader) = function
 | [] ->
      if allowEof then stream.Peek() <> -1 else true
 | a :: b ->
      stream.Peek() <> Char.code a && neq_peek allowEof stream b

let rec eq_peek (stream: #StringReader) = function
 | [] ->
      stream.Peek() <> -1
 | [ a ] ->
      stream.Peek() = Char.code a && stream.Peek() <> -1
 | a :: b :: rest ->
      stream.Peek() = Char.code a || eq_peek stream (b :: rest)

let eatWS ((stream: #StringReader), pos) =
   while (eq_peek stream [' '; '\n'; '\t']) do
      let x, y = if stream.Peek()=10 then
         1, (!pos).y'+1 else (!pos).x'+1, (!pos).y' in
      stream.Read() |> ignore;
      (!pos).x' <- x; (!pos).y' <- y
   done

let readWord (((stream: #StringReader), pos) as source) =
   let res, seps = ref "", [' '; ']'; '['; '?'; '\n'; '\t'] in
   eatWS source;
   if not (neq_peek true stream []) then
      failwith "readWord";
   while (neq_peek true stream seps) do
      res := !res ^ (String.of_char (Char.chr (stream.Peek())));
      (!pos).x' <- (!pos).x' + 1;
      stream.Read() |> ignore
   done;
   !res
```

Our lexer works with a `StringReader` instance passed as part of the `source` argument to the two core functions, `eatWS` and `readWord`, where the first skips all whitespace (spaces, newlines and tabs), and the latter consuming a word - which consists of any character other than whitespace or the brackets. Both functions keep track of the source position, incrementing on each character encountered. Newlines can only occur in whitespace, and upon finding one we start a new source line.

The actual lexer function is below:

```fsharp
exception Eof

let GenerateTokenStream s =
   let pos_of_ppos ({ x'=x; y'=y }: ppos) = { x=x; y=y } in
   let reader = IEnumerable.generate
      (fun () -> new StringReader(s), ref { x'=1; y'=1 })
      (fun ((stream, pos) as source) ->
         let rec fetch stream =
            eatWS source;
            if eq_peek stream [':'] then begin
               let w = readWord source in
               VAR (w, pos_of_ppos !pos)
            end elif eq_peek stream ['['] then begin
               stream.Read() |> ignore;
               LBRACK (pos_of_ppos !pos)
            end elif eq_peek stream [']'] then begin
               stream.Read() |> ignore;
               RBRACK (pos_of_ppos !pos)
            end elif eq_peek stream ['?'] then begin
               stream.Read() |> ignore; HOLE (pos_of_ppos !pos)
            end elif isEof stream then
               raise Eof
            else begin
               let w = readWord source in
               try NUMBER (float_of_string w, (pos_of_ppos !pos)) with
                  _ -> WORD (w, pos_of_ppos !pos)
            end
         in
         try Some (fetch stream) with Eof -> None)
      (fun (stream, _) -> stream.Dispose())
   in
   reader.GetEnumerator()
```

The lexer function contructs a stream of tokens (an `IEnumberable<TOKEN>`), consisting of the various token shapes we discussed. Note how the last else clause determines whether a word is a number or not, returning the appropriate token. Numeric literals are just words with a special meaning.

Next is our parser. As we parse we will keep a state of variables and functions in the current scope, and to work with this state with introduce a few convenience functions and the exceptions that will be signaling the various errors eirther during parsing or running our programs.

```fsharp
exception VarNotFound of string * pos
exception FunNotFound of string * pos
exception NoCanvas of pos
exception InvalidFunName of pos
exception InvalidParam of pos
exception UnexpectedRBrack of pos
exception UnexpectedEof
exception NumberExpected of pos
exception UnexpectedHole of pos

let find_var state pos v = try state.vars.[v] with _ -> raise (VarNotFound (v, pos))
let find_fun state pos f = try state.funs.[f] with _ -> raise (FunNotFound (f, pos))
let add_var state v va = { state with vars=state.vars.Add (v, va) }
let add_fun state f va = { state with funs=state.funs.Add (f, va) }
```

Our parser then takes a state, a scoping predicate (discussed shortly), and the token enumerator obtained from the lexer:

```fsharp
let ParseTokenStream state stScoping (en: IEnumerator<TOKEN>) =
   en.Reset();
   let rec parse_until state check en f =
      let state', e = parse state check true en in
      let state', res = ref state', ref [ e ] in
      while (f state en) do
         let state'', res' = parse !state' check false en in
         res := res' :: !res; state' := state'';
      done;
      !state', List.rev !res
   and parse state check shouldMove (en: IEnumerator<TOKEN>) =
      let get1 state check en = snd (parse state check true en) in
      let get2 state check en =
         let _, e1 = parse state check true en in
         let _, e2 = parse state check true en in
         e1, e2
      in
      let funname_of = function
       | WORD (s, pos) -> s
       | VAR (_, pos) | NUMBER (_, pos) | LBRACK pos
       | RBRACK pos | HOLE pos ->
            raise (InvalidFunName pos)
      in
      let rec params_of = function
       | Var (s, _) -> [ s ]
       | List (l, _) ->
            List.fold_left (fun lst e -> lst @ params_of e) [] l
       | instr ->
            raise (InvalidParam instr.Pos)
      in
      let eq_string1 s s1 = String.lowercase s = String.lowercase s1 in
      let eq_string2 s s1 s2 =
         let s' = String.lowercase s in
         s' = String.lowercase s1 || s' = String.lowercase s2
      in
      if shouldMove && en.MoveNext() || not shouldMove then begin
      match en.Current with
       | WORD (s, pos) when eq_string1 s "canvas" ->
            let e1, e2 = get2 state check en in
            state, Canvas (e1, e2, pos)
       | WORD (s, pos) when eq_string2 s "lt" "left" ->
            let _, e = parse state check true en in
            state, Left (e, pos)
       | WORD (s, pos) when eq_string2 s "rt" "right" ->
            let _, e = parse state check true en in
            state, Right (e, pos)
       | WORD (s, pos) when eq_string2 s "fd" "forward" ->
            let _, e = parse state check true en in
            state, Forward (e, pos)
       | WORD (s, pos) when eq_string1 s "penup" ->
            state, PenUp pos
       | WORD (s, pos) when eq_string1 s "pendown" ->
            state, PenDown pos
       | WORD (s, pos) when eq_string1 s "repeat" ->
            let _, e1 = parse state check true en in
            let state' = add_var state ":repcount" 1.0 in
            let _, e2 = parse state' check true en in
            state, Repeat (e1, e2, pos)
       | WORD (s, pos) when eq_string2 s "+" "sum" ->
            let e1, e2 = get2 state check en in
            state, Binop ((+), e1, e2, pos)
       | WORD (s, pos) when eq_string2 s "-" "minus" ->
            let e1, e2 = get2 state check en in
            state, Binop ((-), e1, e2, pos)
       | WORD (s, pos) when eq_string2 s "*" "times" ->
            let e1, e2 = get2 state check en in
            state, Binop (( * ), e1, e2, pos)
       | WORD (s, pos) when eq_string2 s "/" "divide" ->
            let e1, e2 = get2 state check en in
            state, Binop ((/), e1, e2, pos)
       | WORD (s, pos) when eq_string1 s "min" ->
            let e1, e2 = get2 state check en in
            state, Binop (min, e1, e2, pos)
       | WORD (s, pos) when eq_string1 s "max" ->
            let e1, e2 = get2 state check en in
            state, Binop (max, e1, e2, pos)
       | WORD (s, pos) when eq_string1 s "sin" ->
            let e = get1 state check en in
            state, Unop (sin, e, pos)
       | WORD (s, pos) when eq_string1 s "cos" ->
            let e = get1 state check en in
            state, Unop (cos, e, pos)
       | WORD (s, pos) when eq_string1 s "tan" ->
            let e = get1 state check en in
            state, Unop (tan, e, pos)
       | WORD (s, pos) when eq_string1 s "pi" ->
            state, Number (System.Math.PI, pos)
       | WORD (s, pos) when eq_string1 s "repcount" ->
            state, Var (":repcount", pos)
       | WORD (s, pos) when String.lowercase s="to" ->
            let f = if en.MoveNext() then
                  funname_of en.Current
               else
                  raise UnexpectedEof
            in
            // Don't check formal parameters
            let _, pars = parse state false true en in
            let pars' = params_of pars in
            // Add formal parameters to the current state
            let state' = List.fold_left (fun state p ->
               add_var state p 0.0) state pars'
            in
            // Add dummy reference to self to allow recursive calls
            let state' = add_fun state' f (pars', Hole pos) in
            let _, e = parse state' check true en in
            let state'' = add_fun state f (pars', e) in
            state'', To (f, pars', e, pos)
       | WORD (s, pos) ->
            let pars, _ = find_fun state pos s in
            let args = List.fold_left (fun args _ ->
               let _, e = parse state check true en in
               e :: args) [] pars in
            state, FunCall (s, List.rev args, pos)
       | NUMBER (i, pos) ->
            state, Number (i, pos)
       | VAR (v, pos) ->
            if check then find_var state pos v |> ignore;
            state, Var (v, pos)
       | HOLE pos ->
            state, Hole pos
       | LBRACK pos ->
            let _, exps = parse_until state check en (fun state (en: IEnumerator<TOKEN>) ->
               en.MoveNext() && not en.Current.IsRBrack) in
            state, List (exps, pos)
       | RBRACK pos ->
            raise (UnexpectedRBrack pos)
      end else
         raise UnexpectedEof
   in
   let _, exps = parse_until state stScoping en (fun state en -> en.MoveNext()) in
   exps
```

Note how we construct a list of Logo instructions (last three lines) using the `parse_until` function defined locally. The `check` parameter forces a check on any variable encountered (except in formal parameters in `To`); this check is `false` by default to allow dynamic scoping, but turning it on causes lexical scoping, e.g. each variable has to be on the parameter list of one of the containing functions/commands (as this is the only means to introduce new variables in our implementation, e.g. we don't support the standard `make` Logo command).

And finally, our evaluator is a straightforward implementation of the semantics of each instruction, carefully updating the state and the graphics state:

```fsharp
let pen = new Pen(Color.White)

let draw_line (gra: Graphics) pos x1 y1 x2 y2 =
   let round (f: float) = int_of_float (System.Math.Round(f)) in
   if gra = null then raise (NoCanvas pos);
   gra.DrawLine (pen, round x1, round y1, round x2, round y2)

let rec eval (im: PictureBox) (state: state) gstate instr =
   let rec eval2 state gstate e1 e2 =
      let state', gstate', n1 = eval1 state gstate e1 in
      let state'', gstate'', n2 = eval1 state' gstate' e2 in
      state'', gstate'', n1, n2
   and eval1 state gstate (e: instr) =
      let state', gstate', e' = eval im state gstate e in
      let n = num_of_value e.Pos e' in
      state', gstate', n
   and num_of_value pos = function
    | Some i -> i | _ -> raise (NumberExpected pos)
   in
   let move state (d: float) =
      let pi = 4.0 *. atan 1.0 in
      let angle = state.angle /. 180.0 *. pi in
      state.currX + d *. cos angle,
      state.currY - d *. sin angle
   in
   let forward state gstate pos d =
      let x', y' = move gstate d in
      if gstate.penDown then begin
         draw_line gstate.graphics pos gstate.currX gstate.currY x' y';
         state, { gstate with currX=x'; currY=y' }, None
      end else
         state, { gstate with currX=x'; currY=y' }, None
   in match instr with
    | Canvas (e1, e2, _) ->
         let _, gstate', n1, n2 = eval2 state gstate e1 e2 in
         let i1, i2 = int_of_float n1, int_of_float n2 in
         let bmp = new Bitmap (i1, i2, PixelFormat.Format16bppRgb555) in
         let gra = Graphics.FromImage bmp in
         im.Image <- bmp;
         im.Width <- bmp.Width; im.Height <- bmp.Height;
         state, { gstate' with graphics=gra; angle=90.0; penDown=true;
            currX=float_of_int bmp.Width /. 2.0;
            currY=float_of_int bmp.Height /. 2.0 }, None
    | Binop (f, e1, e2, _) ->
         let _, gstate', n1, n2 = eval2 state gstate e1 e2 in
         state, gstate', Some (f n1 n2)
    | Unop (f, e, _) ->
         let _, gstate', n = eval1 state gstate e in
         state, gstate', Some (f n)
    | Left (e, _) ->
         let _, gstate', i = eval1 state gstate e in
         state, { gstate' with angle=gstate'.angle+i }, None
    | Right (e, _) ->
         let _, gstate', i = eval1 state gstate e in
         state, { gstate' with angle=gstate'.angle-i }, None
    | Forward (e, pos) ->
         let _, gstate', i = eval1 state gstate e in
         forward state gstate' pos i
    | PenUp _ ->
         state, { gstate with penDown=false }, None
    | PenDown _ ->
         state, { gstate with penDown=true }, None
    | Number (n, _) ->
         state, gstate, Some n
    | Var (s, pos) ->
         let n = find_var state pos s in
         state, gstate, Some n
    | FunCall (f, args, pos) ->
         let pars, body = find_fun state pos f in
         let state', gstate' = List.fold_left2 (fun (state, gstate) p arg ->
            let _, gstate', i = eval1 state gstate arg in
            add_var state p i, gstate') (state, gstate) pars args in
         let _, gstate'', res = eval im state' gstate' body in
         state, gstate'', res
    | Hole pos ->
         raise (UnexpectedHole pos)
    | List (lst, _) ->
         let _, gstate', res = List.fold_left (fun (state, gstate, res) e ->
            eval im state gstate e) (state, gstate, None) lst
         in
         state, gstate', res
    | Repeat (num, e, pos) ->
         let _, gstate', i = eval1 state gstate num in
         let state', gstate', res = ref state, ref gstate, ref None in
         for j = 1 to (int_of_float i) do
            state' := add_var !state' ":repcount" (float_of_int j);
            let st, gst, rs = eval im !state' !gstate' e in
            state' := st; gstate' := gst; res := rs;
         done;
         state, !gstate', !res
    | To (f, par, e, pos) ->
         let state' = add_fun state f (par, e) in
         state', gstate, None

let eval_list im (state: state) gstate =
   List.fold_left (fun (state, gstate, res) e ->
      eval im state gstate e) (state, gstate, None)
```

#### How does the evaluator work?

During evaluation we maintain the program state and the graphics state. Each argument to a defined word is evaluated (which could affect the graphics state, so this is preserved throughout) and passed to the definition (body) of that word. Built-in words are evaluated similarly. The program state changes at three locations: after evaluating a new word definition (using `To`, and we add a new function to the state), during evaluation of a function/command call (we add the formal parameters to the state with the value of each argument), and during evaluating the body of a `repeat` command (when we add a special variable called `:repcount` to the state; this can be read directly or through the `RepCount` built-in command).

#### The client module

The client code is straightforward and we don't show some of the event binding to preserve compactness:

```fsharp
open IntelliFactory.Languages.Logo

open System.Windows.Forms
open System.Drawing

let error {x=x; y=y} msg =
   MessageBox.Show (sprintf "Error at %d:%d\n%s" y x msg) |> ignore

let error_no_pos msg =
   MessageBox.Show (sprintf "Error: %s" msg) |> ignore

let create_child_form parent =
   let form = new Form() in
   let sbox = new ScrollableControl() in
   sbox.Dock <- DockStyle.Fill;
   sbox.AutoScroll <- true;
   let im = new PictureBox() in
   sbox.Controls.Add im;
   form.Controls.Add sbox;
   form.MdiParent <- parent;
   form.Visible <- true;
   form, im

let create_main_form () =
   let castToMenu (arr: #ToolStripItem array) =
      Array.map (fun s -> s :> ToolStripItem) arr
   in
   let form = new Form() in
   let panel = new Panel() in
   let memo = new RichTextBox() in
   let menu = new MenuStrip() in
   let sep1 = new ToolStripSeparator() in
   let sep2 = new ToolStripSeparator() in
   let sep3 = new ToolStripSeparator() in
   let fileM = new ToolStripMenuItem("&File") in
   let newMI = new ToolStripMenuItem("&New") in
   let openMI = new ToolStripMenuItem("&Open...") in
   let saveMI = new ToolStripMenuItem("&Save...") in
   let optionsMI = new ToolStripMenuItem("O&ptions") in
   let forceStaticScopingMI = new ToolStripMenuItem("Force lexical scoping") in
   forceStaticScopingMI.CheckOnClick <- true;
   let exitMI = new ToolStripMenuItem("E&xit") in
   let runM = new ToolStripMenuItem("&Run") in
   fileM.DropDownItems.AddRange (castToMenu [| newMI |]);
   fileM.DropDownItems.AddRange (castToMenu [| sep1 |]);
   fileM.DropDownItems.AddRange (castToMenu [| openMI; saveMI |]);
   fileM.DropDownItems.AddRange (castToMenu [| sep2 |]);
   fileM.DropDownItems.AddRange (castToMenu [| optionsMI |]);
   fileM.DropDownItems.AddRange (castToMenu [| sep3 |]);
   fileM.DropDownItems.AddRange (castToMenu [| exitMI |]);
   optionsMI.DropDownItems.AddRange (castToMenu [| forceStaticScopingMI |]);
   menu.Items.AddRange [| (fileM :> ToolStripItem); (runM :> ToolStripItem) |];
   exitMI.Click.Add (fun _ -> form.Close ());
   runM.Click.Add (fun _ ->
      try
         let source = GenerateTokenStream memo.Text in
         let e = ParseTokenStream state.Default forceStaticScopingMI.Checked source in
         let form1, im = create_child_form form in
         let _, _, res = eval_list im state.Default gstate.Default e in
         match res with
          | None -> ()
          | Some i ->
               MessageBox.Show (sprintf "Exit value=%f" i) |> ignore
      with
       | VarNotFound (v, pos) ->
            error pos (sprintf "Unbound variable '%s'" v)
       | FunNotFound (f, pos) ->
            error pos (sprintf "Unbound function '%s'" f)
       | NoCanvas pos ->
            error pos "No canvas available to draw on"
       | InvalidFunName pos ->
            error pos "Invalid function"
       | InvalidParam pos ->
            error pos "Invalid parameter"
       | UnexpectedRBrack pos ->
            error pos "Unexpected ]"
       | UnexpectedEof ->
            error_no_pos "Unexpected eof"
       | NumberExpected pos ->
            error pos "Number expected"
       | UnexpectedHole pos ->
            error pos "[internal] Uninstantiated hole");
   panel.Height <- 100;
   panel.Dock <- DockStyle.Bottom;
   memo.Dock <- DockStyle.Fill;
   memo.Font <- new Font("Courier New", 12.0f);
   panel.Controls.Add memo;
   form.Text <- "IntelliLOGO Interpreter";
   form.Height <- 480; form.Width <- 640;
   form.Controls.Add panel;
   form.Controls.Add menu;
   form.IsMdiContainer <- true;
   form.Visible <- true;
   form, memo

let _ =
   let form, memo = create_main_form () in
   Application.Run form
```

The essence of using the core Logo functionality is in the event delegate for the *Run* menu item. First we generate a token stream from the input string (the `Text` property of the edit box), parse it into our internal representation, and evaluate the resulting instruction list in the context of a child window where the results are displayed. If the evaluation yielded a value we display it after execution.

For instance, trying the following program will result in no graphics output (we get an empty child window) and the exit value of 300:

```
to foo :x + :x :y
to foobar :y [
   to bar :z foo 100
   bar 300
]
foobar 200
```

### Further improvements

The implementation just described lacks a number of standard Logo features. It makes no use of Logo's higher-order functions (`foreach`, etc.) using holes, there are no control flow commands, etc. These can be added with a small effort and we leave that for the reader to experiment with.
