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

module Newsletter =

    let SignUpAction () =
        JQuery.JQuery.Of("#signUp").Click(fun _ ev ->
            JQuery.JQuery.Of(".newsletter-form .success-box").RemoveClass("visible").Ignore
            JQuery.JQuery.Of(".newsletter-form .error-box").RemoveClass("visible").Ignore
            JQuery.JQuery.Of("#signUp").AddClass("loading").Attr("disabled", "disabled").Ignore
            let email : string = JQuery.JQuery.Of("#nemail").Val() :?> string
            if email.Trim() <> "" then
                let fd = FormData()
                fd.Append("email", email)
                fd.Append("type", "Blogs")
                let ajaxSettings =
                    JQuery.AjaxSettings(
                        Url = "https://api.intellifactory.com/api/newsletter",
                        Data = fd,
                        ProcessData = false,
                        ContentType = Union1Of2(false),
                        Type = JQuery.RequestType.POST,
                        Success = (fun _ _ _ ->
                            JQuery.JQuery.Of(".newsletter-form .success-box").AddClass("visible").Ignore
                            JQuery.JQuery.Of("#signUp").RemoveClass("loading").RemoveAttr("disabled").Ignore
                        ),
                        Error = (fun _ _ _ ->
                            JQuery.JQuery.Of(".newsletter-form .error-box").AddClass("visible").Ignore
                            JQuery.JQuery.Of("#signUp").RemoveClass("loading").RemoveAttr("disabled").Ignore
                        )
                    )
                JQuery.JQuery.Ajax(ajaxSettings) |> ignore
                ev.PreventDefault()
            else
                JQuery.JQuery.Of("#signUp").RemoveClass("loading").RemoveAttr("disabled").Ignore
        ).Ignore

[<SPAEntryPoint>]
let Main() =
    Bulma.HookDrawer()
    Highlight.Run()
    Newsletter.SignUpAction()


[<assembly:JavaScript>]
do ()
