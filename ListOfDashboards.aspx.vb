Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
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
        Dim ddtv As DataView = mRecords("SELECT DISTINCT Dashboard FROM ourdashboards WHERE UserId='" & Session("logon") & "' ORDER BY Dashboard", ret)
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
End Class
