Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Web
Imports System.Web.UI.WebControls

Partial Class AddAnalyticsDashboard
    Inherits System.Web.UI.Page

    Private Const ChartTypeAnalytics As String = "analytics"

    Private Sub AddAnalyticsDashboard_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub

    Private Sub AddAnalyticsDashboard_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If PendingUrl() = "" Then
            LabelMessage.Text = "No analytics dashboard item is waiting to be added."
            ButtonAdd.Enabled = False
            Exit Sub
        End If

        LabelTileTitle.Text = "Tile: " & Server.HtmlEncode(PendingTitle())
        LabelTileUrl.Text = "URL: " & Server.HtmlEncode(PendingUrl())

        If Not IsPostBack Then BindDashboards("")
    End Sub

    Private Sub ButtonFind_Click(sender As Object, e As EventArgs) Handles ButtonFind.Click
        BindDashboards(TextBoxDashboardName.Text.Trim())
    End Sub

    Private Sub ButtonCancel_Click(sender As Object, e As EventArgs) Handles ButtonCancel.Click
        Response.Redirect(ReturnUrl())
    End Sub

    Private Sub ButtonAdd_Click(sender As Object, e As EventArgs) Handles ButtonAdd.Click
        Dim dashboardNames As New List(Of String)()

        For Each item As ListItem In CheckBoxListDashboards.Items
            If item.Selected AndAlso item.Value.Trim() <> "" Then dashboardNames.Add(item.Value.Trim())
        Next

        Dim typedName As String = TextBoxDashboardName.Text.Trim()
        If typedName <> "" AndAlso Not DashboardNameExists(dashboardNames, typedName) Then dashboardNames.Add(typedName)

        If dashboardNames.Count = 0 Then
            LabelMessage.Text = "Select a dashboard or type a new dashboard name."
            Exit Sub
        End If

        For Each dashboardName As String In dashboardNames
            SaveDashboardRow(dashboardName)
        Next

        Response.Redirect(DashboardListUrl())
    End Sub

    Private Sub BindDashboards(filterText As String)
        CheckBoxListDashboards.Items.Clear()
        Dim ret As String = String.Empty
        Dim sql As String = "SELECT DISTINCT Dashboard FROM ourdashboards WHERE UserID='" & SqlText(Session("logon")) & "'"
        If filterText.Trim() <> "" Then sql &= " AND Dashboard LIKE '%" & SqlText(filterText.Trim()) & "%'"
        sql &= " ORDER BY Dashboard"

        Dim dv As DataView = mRecords(sql, ret)
        If ret.Trim() <> "" Then
            LabelMessage.Text = ret
            Exit Sub
        End If
        If dv Is Nothing OrElse dv.Table Is Nothing Then Exit Sub

        For Each row As DataRow In dv.Table.Rows
            Dim dashboardName As String = FieldText(row("Dashboard")).Trim()
            If dashboardName <> "" Then CheckBoxListDashboards.Items.Add(New ListItem(dashboardName, dashboardName))
        Next
    End Sub

    Private Sub SaveDashboardRow(dashboardName As String)
        Dim safeUser As String = SqlText(Session("logon"))
        Dim safeDashboard As String = SqlText(dashboardName)
        Dim safeReport As String = SqlText(PendingReportId())
        Dim safeTitle As String = SqlText(TruncateText(PendingTitle(), 240))
        Dim safeUrl As String = SqlText(EncodeArr(PendingUrl()))

        Dim sql As String = "SELECT * FROM ourdashboards WHERE UserID='" & safeUser & "' AND Dashboard='" & safeDashboard & "' AND ChartType='" & ChartTypeAnalytics & "' AND ARR='" & safeUrl & "'"
        If Not HasRecords(sql) Then
            Dim fields As String = "UserID,Dashboard,ReportID,ChartType,GraphTitle,ARR"
            Dim values As String = "'" & safeUser & "','" & safeDashboard & "','" & safeReport & "','" & ChartTypeAnalytics & "','" & safeTitle & "','" & safeUrl & "'"
            ExequteSQLquery("INSERT INTO ourdashboards (" & fields & ") VALUES (" & values & ")")
        Else
            ExequteSQLquery("UPDATE ourdashboards SET GraphTitle='" & safeTitle & "', ReportID='" & safeReport & "', ChartType='" & ChartTypeAnalytics & "' WHERE UserID='" & safeUser & "' AND Dashboard='" & safeDashboard & "' AND ChartType='" & ChartTypeAnalytics & "' AND ARR='" & safeUrl & "'")
        End If

        GetDashboardIdentifier(dashboardName, Session("logon").ToString())
    End Sub

    Private Function PendingUrl() As String
        If Session("PendingAnalyticsDashboardUrl") Is Nothing Then Return ""
        Return Session("PendingAnalyticsDashboardUrl").ToString().Trim()
    End Function

    Private Function PendingTitle() As String
        If Session("PendingAnalyticsDashboardTitle") Is Nothing OrElse Session("PendingAnalyticsDashboardTitle").ToString().Trim() = "" Then Return "Analytics Tile"
        Return Session("PendingAnalyticsDashboardTitle").ToString().Trim()
    End Function

    Private Function PendingReportId() As String
        If Session("PendingAnalyticsDashboardReportID") Is Nothing Then Return ""
        Return Session("PendingAnalyticsDashboardReportID").ToString().Trim()
    End Function

    Private Function ReturnUrl() As String
        Dim url As String = PendingUrl()
        If url.Trim() = "" Then Return "ListOfDashboards.aspx"
        Return url
    End Function

    Private Function DashboardListUrl() As String
        Dim reportId As String = PendingReportId()
        If reportId.Trim() = "" Then Return "ListOfDashboards.aspx"
        Return "ListOfDashboards.aspx?Report=" & Server.UrlEncode(reportId)
    End Function

    Private Function EncodeArr(value As String) As String
        If value Is Nothing Then Return ""
        Return value.Replace("'", "^^").Replace("[", "**").Replace("]", "##")
    End Function

    Private Function SqlText(value As Object) As String
        If value Is Nothing Then Return ""
        Return value.ToString().Replace("'", "''")
    End Function

    Private Function FieldText(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return ""
        Return value.ToString()
    End Function

    Private Function TruncateText(value As String, maxLength As Integer) As String
        If value Is Nothing Then Return ""
        If value.Length <= maxLength Then Return value
        Return value.Substring(0, maxLength)
    End Function

    Private Function DashboardNameExists(dashboardNames As List(Of String), dashboardName As String) As Boolean
        For Each existingName As String In dashboardNames
            If existingName.Equals(dashboardName, StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function
End Class
