Imports System.Web

Partial Class DataAIHelp
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim hilt As String = Request.QueryString("hilt")

        If Not String.IsNullOrWhiteSpace(hilt) Then
            InjectClientHighlighter(hilt)
        End If
    End Sub

    Private Sub InjectClientHighlighter(hilt As String)
        Dim safeHilt As String = HttpUtility.JavaScriptStringEncode(hilt.Trim())

        Dim js As String =
"(function() {" & vbCrLf &
"    var h = """ & safeHilt & """;" & vbCrLf &
"    if (!h) return;" & vbCrLf &
"    function norm(text) {" & vbCrLf &
"        return (text || '').toLowerCase()" & vbCrLf &
"            .replace(/&/g, ' and ')" & vbCrLf &
"            .replace(/[^a-z0-9]+/g, ' ')" & vbCrLf &
"            .replace(/\s+/g, ' ')" & vbCrLf &
"            .trim();" & vbCrLf &
"    }" & vbCrLf &
"    var target = norm(h);" & vbCrLf &
"    if (!target) return;" & vbCrLf &
"    var candidates = document.querySelectorAll('a, .cell-title, .sec-title, .nav-btn');" & vbCrLf &
"    var firstMatch = null;" & vbCrLf &
"    candidates.forEach(function(el) {" & vbCrLf &
"        var text = norm(el.textContent);" & vbCrLf &
"        if (text && (text.indexOf(target) >= 0 || target.indexOf(text) >= 0)) {" & vbCrLf &
"            el.classList.add('hilt-yellow');" & vbCrLf &
"            if (!firstMatch) firstMatch = el;" & vbCrLf &
"        }" & vbCrLf &
"    });" & vbCrLf &
"    if (firstMatch) {" & vbCrLf &
"        firstMatch.scrollIntoView({ block: 'center', behavior: 'smooth' });" & vbCrLf &
"    }" & vbCrLf &
"})();"

        ClientScript.RegisterStartupScript(Me.GetType(), "hiltJS", js, True)
    End Sub
End Class
