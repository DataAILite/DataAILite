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
        Dim updateRet As String = UpdateOURDashboards()
        If updateRet.Trim <> "" AndAlso Not updateRet.Trim().Equals("No Updates", StringComparison.OrdinalIgnoreCase) AndAlso updateRet.ToLower().Contains("error") Then
            Label1.Text = updateRet
            Exit Sub
        End If
        ' Check for ReportID from request only. A plain ListOfDashboards link shows all user dashboards.
        Dim reportId As String = ""
        If Request("ReportID") IsNot Nothing AndAlso Request("ReportID").ToString().Trim() <> "" Then
            reportId = Request("ReportID").ToString().Trim()
        ElseIf Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then
            reportId = Request("Report").ToString().Trim()
        End If

        Dim sqlQuery As String
        If reportId <> "" Then
            sqlQuery = "SELECT Dashboard, MAX(Comments) AS Comments FROM ourdashboards WHERE UserId='" & SqlText(Session("logon")) & "' AND ReportID='" & SqlText(reportId) & "' GROUP BY Dashboard ORDER BY Dashboard"
            lblHeader.Text = "Dashboards for report " & ReportTitleText(reportId)
        Else
            sqlQuery = "SELECT Dashboard, MAX(Comments) AS Comments FROM ourdashboards WHERE UserId='" & SqlText(Session("logon")) & "' GROUP BY Dashboard ORDER BY Dashboard"
            lblHeader.Text = "User Dashboards across all reports"
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
            urlc = DashboardOpenUrl(dashboardname, reportId)
            If Page.FindControl("ctlDashboard_" & i.ToString()) Is Nothing Then
                AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
            ctlLnk = New LinkButton
            ctlLnk.Text = dashboardname
            ctlLnk.ID = "ctlDashboard_" & i.ToString()
            ctlLnk.CommandArgument = urlc
            ctlLnk.ToolTip = "Show '" & dashboardname & "' dashboard"
            'ctlLnk.OnClientClick = "showSpinner();return true;"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click

            list.Rows(i + 1).Cells(0).InnerText = String.Empty
            list.Rows(i + 1).Cells(0).Controls.Add(ctlLnk)
            AddDeleteDashboardCell(list.Rows(i + 1), dashboardname, i)
            AddDescriptionDashboardCell(list.Rows(i + 1), dashboardname, ddtv.Table.Rows(i)("Comments").ToString(), i)
            'list.Rows(i + 1).Cells(0).InnerHtml = "<a href='Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname & "'>" & dashboardname & "</a>"
            End If
        Next
    End Sub
    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLnk As LinkButton = CType(sender, LinkButton)
        Dim link As String = btnLnk.CommandArgument
        If link.Trim() = "" Then link = btnLnk.ID

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

    Private Sub AddDescriptionDashboardCell(row As HtmlTableRow, dashboardName As String, description As String, rowIndex As Integer)
        Dim descriptionCell As HtmlTableCell = Nothing
        If row.Cells.Count > 2 Then
            descriptionCell = row.Cells(2)
            descriptionCell.InnerText = String.Empty
            descriptionCell.Controls.Clear()
        Else
            descriptionCell = New HtmlTableCell()
            row.Cells.Add(descriptionCell)
        End If
        descriptionCell.Align = "center"

        Dim txtDescription As New TextBox()
        txtDescription.ID = "txtDashboardDescription_" & rowIndex.ToString()
        txtDescription.Text = TrimDescription(description)
        txtDescription.ToolTip = dashboardName
        txtDescription.MaxLength = 2000
        txtDescription.Width = Unit.Pixel(340)
        descriptionCell.Controls.Add(txtDescription)
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

    Private Sub ButtonUpdate_Click(sender As Object, e As EventArgs) Handles ButtonUpdate.Click
        Dim ret As String = String.Empty
        Dim updatedCount As Integer = 0

        For i As Integer = 1 To list.Rows.Count - 1
            If list.Rows(i).Cells.Count < 3 Then Continue For

            Dim txtDescription As TextBox = Nothing
            For Each ctl As Control In list.Rows(i).Cells(2).Controls
                txtDescription = TryCast(ctl, TextBox)
                If txtDescription IsNot Nothing Then Exit For
            Next

            If txtDescription Is Nothing Then Continue For

            Dim dashboardName As String = txtDescription.ToolTip
            Dim description As String = txtDescription.Text
            If Request.Form(txtDescription.UniqueID) IsNot Nothing Then
                description = Request.Form(txtDescription.UniqueID)
            End If
            description = TrimDescription(description)

            ret = ExequteSQLquery("UPDATE ourdashboards SET Comments='" & SqlText(description) & "' WHERE UserID='" & SqlText(Session("logon")) & "' AND Dashboard='" & SqlText(dashboardName) & "'")
            If Not IsSqlSuccess(ret) Then
                MessageBox.Show("Description for dashboard '" & dashboardName & "' was not updated. " & ret, "Dashboards", "DashboardDescriptionUpdateError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            End If
            updatedCount += 1
        Next

        Label1.Text = updatedCount.ToString() & " dashboard descriptions were updated."
    End Sub

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "DashboardsDeleted" AndAlso e.Result = Controls_Msgbox.MessageResult.OK Then
            Response.Redirect(CurrentListUrl())
        End If
    End Sub

    Private Function CurrentListUrl() As String
        Dim reportId As String = RequestedReportId()
        If reportId.Trim() = "" Then Return "ListOfDashboards.aspx"
        Return "ListOfDashboards.aspx?Report=" & Server.UrlEncode(reportId)
    End Function

    Private Function RequestedReportId() As String
        If Request("ReportID") IsNot Nothing AndAlso Request("ReportID").ToString().Trim() <> "" Then Return Request("ReportID").ToString().Trim()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Return Request("Report").ToString().Trim()
        Return String.Empty
    End Function

    Private Function ReportTitleText(reportId As String) As String
        If reportId Is Nothing OrElse reportId.Trim() = "" Then Return String.Empty
        Dim reportInfo As DataTable = GetReportInfo(reportId.Trim())
        If reportInfo IsNot Nothing AndAlso reportInfo.Rows.Count > 0 AndAlso reportInfo.Columns.Contains("ReportTtl") Then
            Dim title As String = FieldText(reportInfo.Rows(0)("ReportTtl")).Trim()
            If title <> "" Then Return title
        End If
        Return reportId.Trim()
    End Function

    Private Function FieldText(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return String.Empty
        Return value.ToString()
    End Function

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

    Private Function TrimDescription(value As String) As String
        If value Is Nothing Then Return String.Empty
        value = value.Trim()
        If value.Length > 2000 Then value = value.Substring(0, 2000)
        Return value
    End Function

    Private Function DashboardOpenUrl(dashboardName As String, reportId As String) As String
        Dim ret As String = String.Empty
        Dim safeUser As String = SqlText(Session("logon"))
        Dim safeDashboard As String = SqlText(dashboardName)
        Dim reportFilter As String = ""
        If reportId.Trim() <> "" Then reportFilter = " AND ReportID='" & SqlText(reportId) & "'"
        Dim analyticsCount As Integer = DashboardRowCount("SELECT COUNT(*) AS Cnt FROM ourdashboards WHERE UserID='" & safeUser & "' AND Dashboard='" & safeDashboard & "' AND UPPER(ChartType)='ANALYTICS'" & reportFilter, ret)
        Dim chartCount As Integer = DashboardRowCount("SELECT COUNT(*) AS Cnt FROM ourdashboards WHERE UserID='" & safeUser & "' AND Dashboard='" & safeDashboard & "' AND (ChartType IS NULL OR UPPER(ChartType)<>'ANALYTICS')" & reportFilter, ret)
        Dim reportParameter As String = ""
        If reportId.Trim() <> "" Then reportParameter = "&Report=" & Server.UrlEncode(reportId)

        If analyticsCount > 0 AndAlso chartCount > 0 Then
            Return "MixDashboard.aspx?dashboard=" & Server.UrlEncode(dashboardName) & reportParameter
        End If
        If analyticsCount > 0 Then
            Return "CustomDashboard.aspx?dashboard=" & Server.UrlEncode(dashboardName) & reportParameter
        End If
        Return "Dashboard.aspx?user=" & Server.UrlEncode(Session("logon").ToString()) & "&dashboard=" & Server.UrlEncode(dashboardName) & reportParameter
    End Function

    Private Function DashboardRowCount(sql As String, ByRef ret As String) As Integer
        Dim dv As DataView = mRecords(sql, ret)
        If ret.Trim() <> "" OrElse dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then Return 0
        Dim countValue As Integer = 0
        Integer.TryParse(dv.Table.Rows(0)(0).ToString(), countValue)
        Return countValue
    End Function
End Class
