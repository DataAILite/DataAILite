Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Partial Class ReportCopy
    Inherits System.Web.UI.Page
    Private Sub ReportCopy_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        TextBoxReportID.Text = (Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_")).Replace(" ", "")
        TextBoxReportTitle.Text = Session("NewReportTtl")
    End Sub
    Private Sub ReportCopy_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim rep As String = Request("Report")
        Session("REPORTID") = rep
        LabelAlert0.Text = "Copy Report -- " & rep & " -- To:"
    End Sub
    Protected Sub ButtonSubmit_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click
        Dim repnew As String = TextBoxReportID.Text                                 'Request("newrep")
        Dim repnewttl As String = TextBoxReportTitle.Text                           'Request("newrepttl")
        Dim rep As String = Session("REPORTID")
        Dim repttl As String = String.Empty

        Dim dri As DataTable = GetReportInfo(repnew)
        If dri.Rows.Count > 0 Then
            LabelAlert1.Text = "Report " & repnewttl & " with ReportId=" & repnew & " already exists. ReportId should be unique. Select another one."
            Exit Sub
        Else
            Dim dro As DataTable = GetReportInfo(rep)
            repttl = dro.Rows(0)("ReportTtl")
        End If

        Dim bErr As Boolean = False
        Dim ret As String = CreateCleanReportColumnsFieldsItems(rep, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        ret = CopyReport(rep, repttl, repnew, repnewttl, Session("logon"), bErr, Session("UserConnString"), Session("UserConnProvider"))
        WriteToAccessLog(Session("logon"), "Report " & rep & " has been copied into report " & repnew & " with result: " & ret, 3)
        Dim repfile As String = String.Empty
        ret = UpdateXSDandRDL(repnew, Session("UserConnString"), Session("UserConnProvider"), repfile)
        ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repnew, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        'LabelAlert1.Text = "Report " & rep & " has been copied into report " & repnew
        Session("REPORTID") = repnew
        LabelAlert1.Text = ret

    End Sub


End Class
