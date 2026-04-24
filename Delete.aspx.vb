
Partial Class Delete
    Inherits System.Web.UI.Page
    Private Sub Delete_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("NewFileCreated") = False
        Session("REPORTID") = Request("REPORT")
        Session("REPEDIT") = Request("repedit")
        LabelSure.Text = LabelSure.Text & " <br/>Report - " & Session("REPORTID")
    End Sub


    Protected Sub ButtonYes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonYes.Click
        'delete report from Permissions, ReportInfo and ReportShow
        If Session("REPEDIT") = "del" Then

            ExequteSQLquery("DELETE FROM OURReportShow WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURReportInfo WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURReportGroups WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURReportFormat WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURReportLists WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURReportChildren WHERE ReportID='" & Session("REPORTID") & "'")
            ExequteSQLquery("DELETE FROM OURPermissions WHERE PARAM1='" & Session("REPORTID") & "' AND APPLICATION='InteractiveReporting'")
            'TODO
            'DELETE FROM ALL OUR TABLES  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Response.Redirect("ListOfReports.aspx")
        End If

    End Sub

    Protected Sub ButtonNo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonNo.Click
        'return to ListOfReports
        Response.Redirect("ListOfReports.aspx")
    End Sub
End Class
