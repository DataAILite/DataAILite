
Imports System.Drawing
Imports System.Web
Imports System.Web.UI.WebControls
Partial Class DataAIHelp
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim hilt As String = Request.QueryString("hilt")

        'If Not String.IsNullOrEmpty(hilt) Then
        '    ' 1) Server-side: highlight WebControls that expose .Text
        '    HighlightServerControls(Me.Form, hilt)

        '    ' 2) Client-side: inject JS to highlight plain <a> tags (and optionally any element)
        '    'InjectClientHighlighter(hilt)
        'End If
    End Sub

    'Private Sub HighlightServerControls(parent As Control, hilt As String)
    '    For Each ctrl As Control In parent.Controls
    '        ' Reflectively check for a Text property (Label, Button, TextBox, LinkButton, etc.)
    '        Dim textProp = ctrl.GetType().GetProperty("Text")
    '        If textProp IsNot Nothing Then
    '            Dim textValue As String = TryCast(textProp.GetValue(ctrl, Nothing), String)
    '            If Not String.IsNullOrEmpty(textValue) AndAlso
    '               textValue.IndexOf(hilt, StringComparison.OrdinalIgnoreCase) >= 0 Then
    '                Dim webCtrl As WebControl = TryCast(ctrl, WebControl)
    '                If webCtrl IsNot Nothing Then
    '                    webCtrl.CssClass = (webCtrl.CssClass & " hilt-yellow").Trim()
    '                End If
    '            End If
    '        End If

    '        ' Recurse into children
    '        If ctrl.HasControls() Then
    '            HighlightServerControls(ctrl, hilt)
    '        End If
    '    Next
    'End Sub

    '    Private Sub InjectClientHighlighter(hilt As String)
    '        ' Prevent JS injection; encode the string as a JS literal
    '        Dim safeHilt As String = HttpUtility.JavaScriptStringEncode(hilt)

    '        Dim js As String =
    '$"(function() {{
    '    var h = ""{safeHilt}"";
    '    if (!h) return;
    '    var hilts = function(text) {{
    '        return text && text.toLowerCase().indexOf(h.toLowerCase()) >= 0;
    '    }};
    '    // Highlight plain <a> tags
    '    document.querySelectorAll('a').forEach(function(a) {{
    '        if (hilts(a.textContent)) {{
    '            a.classList.add('hilt-yellow');
    '        }}
    '    }});
    '    // OPTIONAL: highlight ANY element containing the text (uncomment if desired)
    '    // document.querySelectorAll('body *:not(script):not(style)').forEach(function(el) {{
    '    //     if (hilts(el.childElementCount === 0 ? el.textContent : '')) {{
    '    //         el.classList.add('hilt-yellow');
    '    //     }}
    '    // }});
    '}})();"

    '        ' Use ScriptManager if you have UpdatePanels; otherwise ClientScript is fine.
    '        If ScriptManager.GetCurrent(Page) IsNot Nothing Then
    '            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hiltJS", js, True)
    '        Else
    '            ClientScript.RegisterStartupScript(Me.GetType(), "hiltJS", js, True)
    '        End If
    '    End Sub
End Class
