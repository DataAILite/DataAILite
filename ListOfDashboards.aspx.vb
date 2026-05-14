Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.HtmlControls
Partial Class ListOfDashboards
    Inherits System.Web.UI.Page

    Private Sub ListOfDashboards_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub

    Private Sub ListOfDashboards_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim ret As String = String.Empty
        Dim sqls As String = String.Empty
        'If Not IsPostBack AndAlso Not Request("delindx") Is Nothing AndAlso Request("delindx").Trim <> "" Then
        '    ret = ExequteSQLquery("DROP TABLE " & Request("delindx").Trim, Session("UserConnString"), Session("UserConnProvider")) 'in OURcsv
        '    Label1.Text = ret
        '    sqls = "DELETE FROM OURUserTables WHERE TableName='" & Request("delindx").Trim & "' AND UserID='" & Session("logon") & "'"
        '    ret = ExequteSQLquery(sqls)  'in OURdb
        'End If
        'If Not IsPostBack AndAlso Not Request("corindx") Is Nothing AndAlso Request("corindx").Trim <> "" Then
        '    Dim tbl As String = Request("corindx").Trim
        '    ret = CorrectFieldTypesInTable(tbl, Session("UserConnString"), Session("UserConnProvider"))
        '    Label1.Text = tbl & " updated: " & ret
        'End If
        Dim i As Integer = 0
        Dim j As Integer = 0
        ret = ""

        ' Check for ReportID from request or session
        Dim reportId As String = ""
        If Request("ReportID") IsNot Nothing AndAlso Request("ReportID").ToString().Trim() <> "" Then
            reportId = Request("ReportID").ToString().Trim()
        ElseIf Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then
            reportId = Request("Report").ToString().Trim()
        ElseIf Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then
            reportId = Session("REPORTID").ToString().Trim()
        End If

        Dim sqlQuery As String
        If reportId <> "" Then
            sqlQuery = "SELECT DISTINCT Dashboard FROM ourdashboards WHERE UserId='" & SqlText(Session("logon")) & "' AND ReportID='" & SqlText(reportId) & "' ORDER BY Dashboard"
            lblHeader.Text = "Chart Dashboards for Report:"
        Else
            sqlQuery = "SELECT DISTINCT Dashboard FROM ourdashboards WHERE UserId='" & SqlText(Session("logon")) & "' ORDER BY Dashboard"
        End If

        Dim ddtv As DataView = mRecords(sqlQuery, ret)
        If ret.Trim <> "" Then
            Label1.Text = ret
            Exit Sub
        End If
        If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
            Label1.Text = "There are no dashboards for this user."
            Exit Sub
        Else
            lblTablesCount.Text = ddtv.Table.Rows.Count.ToString & " dashboards"
        End If
        Dim dashboardname As String = String.Empty

        Dim ctlLnk As LinkButton = Nothing
        Dim urlc As String = String.Empty

        For i = 0 To ddtv.Table.Rows.Count - 1
            dashboardname = ddtv.Table.Rows(i)("Dashboard").ToString
            urlc = "Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname
            If Page.FindControl(urlc) Is Nothing Then
                AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
            ctlLnk = New LinkButton
            ctlLnk.Text = dashboardname
            ctlLnk.ID = urlc
            ctlLnk.ToolTip = "Show '" & dashboardname & "' dashboard"
            'ctlLnk.OnClientClick = "showSpinner();return true;"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click

            list.Rows(i + 1).Cells(0).InnerText = String.Empty
            list.Rows(i + 1).Cells(0).Controls.Add(ctlLnk)
            AddDeleteDashboardCell(list.Rows(i + 1), dashboardname, i)
            'list.Rows(i + 1).Cells(0).InnerHtml = "<a href='Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname & "'>" & dashboardname & "</a>"
            End If
        Next
    End Sub
    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLnk As LinkButton = CType(sender, LinkButton)
        Dim link As String = btnLnk.ID

        Response.Redirect(link)
    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Response.Redirect(url)
    End Sub

    Private Sub AddDeleteDashboardCell(row As HtmlTableRow, dashboardName As String, rowIndex As Integer)
        Dim deleteCell As HtmlTableCell = Nothing
        If row.Cells.Count > 1 Then
            deleteCell = row.Cells(1)
            deleteCell.InnerText = String.Empty
            deleteCell.Controls.Clear()
        Else
            deleteCell = New HtmlTableCell()
            row.Cells.Add(deleteCell)
        End If
        deleteCell.Align = "center"

        Dim chkDelete As New CheckBox()
        chkDelete.ID = "chkDeleteDashboard_" & rowIndex.ToString()
        chkDelete.ToolTip = dashboardName
        deleteCell.Controls.Add(chkDelete)
    End Sub

    Private Sub ButtonDelete_Click(sender As Object, e As EventArgs) Handles ButtonDelete.Click
        Dim selectedDashboards As New List(Of String)()

        For i As Integer = 1 To list.Rows.Count - 1
            If list.Rows(i).Cells.Count < 2 Then Continue For

            Dim chkDelete As CheckBox = Nothing
            For Each ctl As Control In list.Rows(i).Cells(1).Controls
                chkDelete = TryCast(ctl, CheckBox)
                If chkDelete IsNot Nothing Then Exit For
            Next

            If chkDelete IsNot Nothing AndAlso Request.Form(chkDelete.UniqueID) IsNot Nothing Then
                selectedDashboards.Add(chkDelete.ToolTip)
            End If
        Next

        If selectedDashboards.Count = 0 Then
            Label1.Text = "No dashboards were selected for deletion."
            Exit Sub
        End If

        Dim ret As String = String.Empty
        For Each dashboardName As String In selectedDashboards
            ret = ExequteSQLquery("DELETE FROM ourdashboards WHERE UserID='" & SqlText(Session("logon")) & "' AND Dashboard='" & SqlText(dashboardName) & "'")
            If Not IsSqlSuccess(ret) Then
                MessageBox.Show("Dashboard '" & dashboardName & "' was not deleted. " & ret, "Dashboards", "DashboardDeleteError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            End If
        Next

        MessageBox.Show(DeletedDashboardsMessage(selectedDashboards), "Dashboards", "DashboardsDeleted", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.PostOK)
    End Sub

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "DashboardsDeleted" AndAlso e.Result = Controls_Msgbox.MessageResult.OK Then
            Response.Redirect("ListOfDashboards.aspx")
        End If
    End Sub

    Private Function SqlText(value As Object) As String
        If value Is Nothing Then Return String.Empty
        Return value.ToString().Replace("'", "''")
    End Function

    Private Function IsSqlSuccess(ret As String) As Boolean
        Return ret Is Nothing OrElse ret.Trim() = "" OrElse ret.Trim().Equals("Query executed fine.", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function DeletedDashboardsMessage(dashboardNames As List(Of String)) As String
        If dashboardNames Is Nothing OrElse dashboardNames.Count = 0 Then Return "Dashboard has been deleted."
        If dashboardNames.Count = 1 Then Return "Dashboard '" & dashboardNames(0) & "' has been deleted."
        Return "Dashboards have been deleted: " & String.Join(", ", dashboardNames.ToArray())
    End Function
End Class
