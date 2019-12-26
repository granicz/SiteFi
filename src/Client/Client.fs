module Client

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation

module Highlight =
    open WebSharper.HighlightJS

    [<Require(typeof<Resources.Languages.Fsharp>)>]
    [<Require(typeof<Resources.Styles.Vs>)>]
    let Run() =
        JS.Document.QuerySelectorAll("code[class^=language-]").ForEach(
            (fun (node, _, _, _) -> Hljs.HighlightBlock(node)),
            JS.Undefined
        )

module Bulma =

    let DrawerShown = Var.Create false

    [<JavaScriptExport>]
    let ToggleDrawer() = DrawerShown.Update not

    let HookDrawer() =
        DrawerShown.View |> View.Sink (fun shown ->
            JS.Document.QuerySelectorAll(".drawer-backdrop, .lhs-drawer").ForEach(
                (fun (node, _, _, _) ->
                    let node = node :?> Dom.Element
                    "shown"
                    |> if shown then node.ClassList.Add else node.ClassList.Remove
                ),
                JS.Undefined
            )
        )

[<SPAEntryPoint>]
let Main() =
    Bulma.HookDrawer()
    Highlight.Run()

[<assembly:JavaScript>]
do ()
