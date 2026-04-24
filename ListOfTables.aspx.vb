Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Partial Class ListOfTables
    Inherits System.Web.UI.Page

    Private Sub ListOfTables_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        'Session("mapkey") = ConfigurationManager.AppSettings("mapkey").ToString
        'GenerateMap.mapkey = Session("mapkey")
    End Sub

    Private Sub ListOfTables_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("CSV") <> "yes" AndAlso Session("admin") = "user" Then
            Response.Redirect("ListOfReports.aspx")
        End If
        'If Session("CSV") <> "yes" AndAlso (Session("admin") = "admin" Or Session("admin") = "super") Then
        '    list.Rows(0).Cells(2).InnerHtml = " "
        '    list.Rows(0).Cells(4).InnerHtml = " "
        'End If
        Dim ret As String = String.Empty
        If Session("admin") = "super" OrElse Session("admin") = "admin" Then
            lnkRedoListOfUserTables.Visible = True
            lnkRedoListOfUserTables.Enabled = True
        Else
            lnkRedoListOfUserTables.Visible = False
            lnkRedoListOfUserTables.Enabled = False
        End If
        Dim sqls As String = String.Empty
        If Not IsPostBack AndAlso Not Request("delindx") Is Nothing AndAlso Request("delindx").Trim <> "" Then
            'delete table from OURUserTables in OUR db
            sqls = "DELETE FROM OURUserTables WHERE TableName='" & Request("delindx").Trim & "' AND UserID='" & Session("logon") & "'"
            ret = ExequteSQLquery(sqls)
            If Session("admin") = "super" OrElse Session("admin") = "admin" Then
                'drop table from User database
                ret = ExequteSQLquery("DROP TABLE " & Request("delindx").Trim, Session("UserConnString"), Session("UserConnProvider")) 'in user db
                Label1.Text = ret
            End If
        End If
        If Not IsPostBack AndAlso (Session("admin") = "super" OrElse Session("admin") = "admin") AndAlso Not Request("corindx") Is Nothing AndAlso Request("corindx").Trim <> "" Then
            Dim tbl As String = Request("corindx").Trim
            'TODO correct fields in table
            ret = CorrectFieldsInTable(tbl, Session("UserConnString"), Session("UserConnProvider"), True)
            'TODO redo report

            Label1.Text = tbl & " updated: " & ret
        Else
            Label1.Text = "You do not have rights to change the data. Contact your database admin."
        End If
        'If Not IsPostBack AndAlso Not Request("anindx") Is Nothing AndAlso Request("anindx").Trim <> "" Then
        '    Dim tbl As String = Request("anindx").Trim
        '    ret = AnalyticsForTable(tbl, Session("UserConnString"), Session("UserConnProvider"))
        '    Label1.Text = tbl & " analytics: " & ret
        'End If
        'If Not IsPostBack AndAlso Not Request("dtindx") Is Nothing AndAlso Request("dtindx").Trim <> "" Then
        '    Dim tbl As String = Request("dtindx").Trim
        '    ret = DataInTable(tbl, Session("UserConnString"), Session("UserConnProvider"))
        '    Label1.Text = tbl & " data: " & ret
        'End If
        Dim i As Integer = 0
        Dim j As Integer = 0
        ret = ""
        Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), ret, Session("logon"), Session("CSV"))
        If ret.Trim <> "" Then
            Label1.Text = ret
            Exit Sub
        End If
        If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
            Label1.Text = "There are no tables for this user."
            Exit Sub
        Else
            lblTablesCount.Text = ddtv.Table.Rows.Count.ToString & " tables/classes"
        End If
        Dim tableid, tablename, delurl, reps, corurl As String

        Dim htTableName As New Hashtable

        For i = 0 To ddtv.Table.Rows.Count - 1
            'AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
            tableid = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            Try
                tablename = ddtv.Table.Rows(i)("TableName").ToString
            Catch ex As Exception
                tablename = tableid
            End Try
            If tablename.Trim = "" Then
                tablename = tableid
            End If
            Dim id As String = tableid.ToUpper
            If htTableName(id) Is Nothing Then
                htTableName.Add(id, "1")
                j += 1
                AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
                If i Mod 2 = 0 Then
                    list.Rows(j).BgColor = "#EFFBFB"
                Else
                    list.Rows(j).BgColor = "white"
                End If
                list.Rows(j).Cells(0).InnerHtml = "<a href='ClassExplorer.aspx?cld=" & tableid.Replace("%", "%25") & "'>" & tableid & "</a>"
                list.Rows(j).Cells(1).InnerHtml = tableid
                corurl = "ListOfTables.aspx?corindx=" & tableid
                list.Rows(j).Cells(4).InnerHtml = "<a href='" & corurl & "'>correct</a>"
                Dim cln As String = FixReservedWords(CorrectTableNameWithDots(tableid), Session("UserConnProvider"))
                Dim dv1 As DataView
                Dim er As String = String.Empty
                ''Check Report Info (title, data for report) for this class 
                'dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & tableid & "') AND (ReportDB LIKE '%" & Session("UserDB") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
                'If dv1 Is Nothing OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then
                '    ''create new standard report  'it will be done in Redo                    repid = Session("dbname") & "_INIT_" & (j - 1).ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                '    'ret = MakeNewStanardReport(Session("logon"), repid, tableid, Session("UserDB"), "SELECT * FROM " & cln, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                '    'If ret.Contains("ERROR!!") Then
                '    '    WriteToAccessLog(Session("logon"), "Initial Report created with errors:" & tableid & ", error:" & ret, 6)
                '    '    Session("REPORTID") = ""
                '    '    'Else
                '    '    '    dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & tableid & "') AND (ReportDB LIKE '%" & Session("UserDB") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
                '    'End If
                '    'Else  'it will be done in Redo
                '    '    'add permissions for user if they are not there
                '    '    repid = dv1.Table.Rows(0)("ReportId").ToString
                '    '    sqls = "SELECT * FROM OURPermissions WHERE NetId='" & Session("logon") & "' AND Param1='" & repid & "'"
                '    '    If Not HasRecords(sqls) Then
                '    '        'new permission required
                '    '        sqls = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & repid & "','" & tableid & "','" & Session("email") & "','admin','" & DateToString(Now) & "','" & Session("UserEndDate") & "','initial')"
                '    '        Dim retr As String = ExequteSQLquery(sqls)
                '    '    End If

                'End If

                'find all reports with the table
                sqls = "SELECT DISTINCT OURReportInfo.ReportId, OURReportInfo.SQLquerytext, OURReportSQLquery.Tbl1, OURReportInfo.ReportDB FROM OURReportSQLquery INNER JOIN OURPermissions  ON (OURReportSQLquery.ReportId = OURPermissions.Param1) INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId = OURReportInfo.ReportId) WHERE Tbl1='" & tableid & "' AND NetId='" & Session("logon") & "' AND ReportDB LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%'"
                dv1 = mRecords(sqls, ret)
                Dim dt As DataTable = dv1.Table

                If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                    Session("REPORTID") = ""
                    delurl = "ListOfTables.aspx?delindx=" & tableid
                    list.Rows(j).Cells(2).InnerHtml = "<a href='" & delurl & "'>del</a>"
                    list.Rows(j).Cells(3).InnerHtml = " "
                    list.Rows(j).Cells(5).InnerHtml = " "
                    list.Rows(j).Cells(6).InnerHtml = " "
                    Continue For
                Else
                    Session("REPORTID") = dt.Rows(0)("ReportId")
                End If
                reps = ""
                repid = ""
                For jj As Integer = 0 To dt.Rows.Count - 1
                    reps = reps & dt.Rows(jj)("ReportId").ToString & ", "
                    If repid = "" AndAlso (dt.Rows(jj)("SQLquerytext").ToString.ToUpper.Trim = "SELECT * FROM " & cln.ToUpper) Then
                        repid = dt.Rows(jj)("ReportId").ToString
                    End If
                Next
                list.Rows(j).Cells(2).InnerHtml = " - "
                list.Rows(j).Cells(3).InnerHtml = reps

                list.Rows(j).Cells(5).InnerHtml = "<a href='ClassExplorer.aspx?cld=" & tableid.Replace("%", "%25") & "'>data</a>"


                Dim LinkID As String = repid & "&" & cln 'Session("REPTITLE")
                'If Page.FindControl(LinkID) Is Nothing Then
                Dim ctl As New LinkButton
                ctl.Text = "analytics"
                ctl.ID = LinkID
                ctl.ToolTip = "Analytics for " & tableid 'Session("REPTITLE")
                ctl.CssClass = "NodeStyle"
                ctl.Font.Size = 10
                AddHandler ctl.Click, AddressOf btnAnalytics_Click
                'list.Rows(0).Cells(6).InnerText = "analytics"
                list.Rows(j).Cells(6).InnerText = ""
                list.Rows(j).Cells(6).Controls.Add(ctl)

                'If Session("CSV") <> "yes" AndAlso (Session("admin") = "admin" Or Session("admin") = "super") Then
                '    list.Rows(j).Cells(2).InnerHtml = " "
                '    list.Rows(j).Cells(4).InnerHtml = " "
                'End If
            End If
        Next
        Session("REPORTID") = ""
    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Response.Redirect(url)
    End Sub
    Public Function DataInTable(ByVal tblname As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByVal exst As Boolean = False) As String
        Dim ret As String = String.Empty
        'open ClassExplorer page for tblname

        Return ret
    End Function
    'Public Function AnalyticsForTable(ByVal tblname As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional ByVal exst As Boolean = False) As String
    '    Dim ret As String = String.Empty
    '    'open Analitics page for report "SELECT * FROM " & tblname

    '    Return ret
    'End Function
    Protected Sub btnAnalytics_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim Rpt As String = Piece(ctl.ID, "&", 1)
        Dim RptTtl As String = Piece(ctl.ID, "&", 2)
        Response.Redirect("ShowReport.aspx?srd=11&REPORT=" & Rpt)
    End Sub
    Private Sub btnListOfJoins_Click(sender As Object, e As EventArgs) Handles btnListOfJoins.Click
        Response.Redirect("ListOfJoins.aspx")
    End Sub

    Private Sub btnListOfClasses_Click(sender As Object, e As EventArgs) Handles btnListOfClasses.Click
        Response.Redirect("ClassExplorer.aspx")
    End Sub

    Private Sub lnkRedoListOfUserTables_Click(sender As Object, e As EventArgs) Handles lnkRedoListOfUserTables.Click
        Dim ret As String = String.Empty
        Dim tbl As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim i As Integer
        Dim dv As DataView

        Try
            'list of user tables and reports
            sqlq = "SELECT DISTINCT Tbl1,ReportID FROM OURReportSQLquery INNER JOIN OURPermissions ON OURReportSQLquery.ReportId=OURPermissions.Param1 WHERE OURPermissions.NetId='" & Session("logon") & "'"
            dv = mRecords(sqlq, ret)
            If dv Is Nothing OrElse dv.Count = 0 OrElse dv.Table.Rows.Count = 0 Then
                Label1.Text = "There are no reports and tables for this user."
                Exit Sub
            End If
            'make loop to add tables with proper names in OURUserTables of Userdb
            For i = 0 To dv.Table.Rows.Count - 1
                tbl = FixReservedWords(dv.Table.Rows(i)("Tbl1").ToString.Trim, Session("UserConnProvider"))
                If tbl.Trim <> "" Then
                    ret = InsertTableIntoOURUserTables(tbl, tbl, Session("Unit"), Session("logon"), Session("UserDB"), "", dv.Table.Rows(i)("Tbl1").ToString.Trim)
                    WriteToAccessLog(Session("logon"), "The table " & tbl & " has been added into OURUserTables with result: " & ret, 111)

                    'TODO ticket # 1291 correct fields in table !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    'ret = CorrectFieldsInTable(tbl, Session("UserConnString"), Session("UserConnProvider"), True)
                    'If ret.StartsWith("ERROR!!") Then
                    '    WriteToAccessLog(Session("logon"), "The table " & tbl & " filelds have been corrected with result: " & ret, 111)
                    'Else
                    'WriteToAccessLog(Session("logon"), "The table " & tbl & " filelds typed have been corrected with result: " & ret, 111)
                    'End If
                    'Make reports and correct fields, columns, items
                End If
            Next

            '===============================================================================
            'Make missing initial reports
            Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), ret, Session("logon"), Session("CSV"))
            If ret.Trim <> "" Then
                Label1.Text = ret
                Exit Sub
            End If
            If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
                Label1.Text = "There are no tables for this user."
                Exit Sub
            Else
                lblTablesCount.Text = ddtv.Table.Rows.Count.ToString & " tables/classes"
            End If
            Dim tableid, tablename As String

            Dim htTableName As New Hashtable

            For i = 0 To ddtv.Table.Rows.Count - 1
                tableid = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                Try
                    tablename = ddtv.Table.Rows(i)("TableName").ToString
                Catch ex As Exception
                    tablename = tableid
                End Try
                If tablename.Trim = "" Then
                    tablename = tableid
                End If
                Dim cln As String = FixReservedWords(CorrectTableNameWithDots(tableid), Session("UserConnProvider"))
                Dim dv1 As DataView
                Dim er As String = String.Empty
                'TODO Check Report Info (title, data for report) for this class 
                dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & tableid & "') AND (ReportDB LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
                If dv1 Is Nothing OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then
                    'create new standard report
                    repid = Session("dbname") & "_INIT_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                    ret = MakeNewStanardReport(Session("logon"), repid, tableid, Session("UserDB"), "SELECT * FROM " & cln, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    If ret.Contains("ERROR!!") Then
                        WriteToAccessLog(Session("logon"), "Initial Report created with errors:" & tableid & ", error:" & ret, 6)
                        Session("REPORTID") = ""
                    End If
                Else
                    'add permissions for user if they are not there
                    repid = dv1.Table.Rows(0)("ReportId").ToString
                    sqlq = "SELECT * FROM OURPermissions WHERE NetId='" & Session("logon") & "' AND Param1='" & repid & "'"
                    If Not HasRecords(sqlq) Then
                        'new permission required
                        sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & repid & "','" & tableid & "','" & Session("email") & "','admin','" & DateToString(Now) & "','" & Session("UserEndDate") & "','initial')"
                        Dim retr As String = ExequteSQLquery(sqlq)
                    End If
                End If
            Next
            '=====================================================================================

        Catch ex As Exception
            ret = ex.Message
        End Try
        Response.Redirect("ListOfTables.aspx")

    End Sub
End Class
