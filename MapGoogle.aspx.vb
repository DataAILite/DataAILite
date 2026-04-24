Partial Class MapGoogle
    Inherits System.Web.UI.Page
    Public kmlfile As String
    Private Sub MapGoogle_Init(sender As Object, e As EventArgs) Handles Me.Init
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        kmlfile = Session("WEBOUR") & "KMLS/" & Session("kmlfile").ToString.Replace(Session("appldirKMLFiles"), "")
    End Sub
    Private Sub MapGoogle_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Request("showlinks") IsNot Nothing Then _
                Session("showlinks") = Request("showlinks")
            If Request("showcircles") IsNot Nothing Then _
                Session("showcircles") = Request("showcircles")
            If Request("showpins") IsNot Nothing Then _
                Session("showpins") = Request("showpins")
            If Request("initalt") IsNot Nothing Then _
                Session("initalt") = Request("initalt")
            If Request("linewidth") IsNot Nothing Then _
                Session("linewidth") = Request("linewidth")
        End If
    End Sub
    Protected Sub LinkButtonBack_Click(sender As Object, e As EventArgs) Handles LinkButtonBack.Click
        Dim showlinks As String = "false"
        Dim showcircles As String = "false"
        Dim showpins As String = "false"
        Dim initalt As String = "4000"
        Dim linewidth As String = "4"

        If Session("showlinks") IsNot Nothing Then showlinks = Session("showlinks")
        If Session("showcircles") IsNot Nothing Then showcircles = Session("showcircles")
        If Session("showpins") IsNot Nothing Then showpins = Session("showpins")
        If Session("initalt") IsNot Nothing Then initalt = Session("initalt")
        If Session("linewidth") IsNot Nothing Then linewidth = Session("linewidth")
        'Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&mapname=" & Session("txtMapName")) ' & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value)
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&showlinks=" & showlinks & "&showcircles=" & showcircles & "&showpins=" & showpins & "&initalt=" & initalt & "&linewidth=" & linewidth)
    End Sub
End Class
