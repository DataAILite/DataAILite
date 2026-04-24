Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Partial Class ListOfJoins
    Inherits System.Web.UI.Page

    Private Sub ListOfJoins_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Session("mapkey") = ConfigurationManager.AppSettings("mapkey").ToString
        GenerateMap.mapkey = Session("mapkey")
    End Sub

    Private Sub ListOfJoins_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("CSV") <> "yes" AndAlso Session("admin") = "user" Then
            Response.Redirect("ListOfReports.aspx")
        End If
        Dim ret As String = String.Empty
        Dim userdb As String = Session("UserDB")

        If Not IsPostBack AndAlso Not Request("delindx") Is Nothing AndAlso Request("delindx").Trim <> "" Then
            'delete join
            Dim sqlindx As String = Request("delindx").ToString
            ret = DeleteDatadrivenOrCustomJoins(userdb, sqlindx)
            'Label1.Text = ret
        End If
        If Not IsPostBack AndAlso Not Request("undodelindx") Is Nothing AndAlso Request("undodelindx").Trim <> "" Then
            'delete join
            Dim sqlindx As String = Request("undodelindx").ToString
            ret = UndoDeleteDatadrivenOrCustomJoins(userdb, sqlindx)
            'make report for this join if needed
            ret = RedoReportForJoin(sqlindx, Session("logon"), Session("dbname"), Session("UserDB"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"))
            'Label1.Text = ret
        End If
        If Not IsPostBack AndAlso Not Session("origin") Is Nothing AndAlso Session("origin").ToString.Trim <> "" Then
            DropDownListJoinOrigin.SelectedValue = Session("origin").ToString
            DropDownListJoinOrigin.Text = Session("origin").ToString
            chkRedoJoins.Checked = False
        End If
        If DropDownListJoinOrigin.SelectedValue = "initial" OrElse DropDownListJoinOrigin.SelectedValue = "datadriven" Then
            btnCalcJoins.Visible = True
            btnCalcJoins.Enabled = True
        Else
            btnCalcJoins.Visible = False
            btnCalcJoins.Enabled = False
        End If
        ret = ""
        Dim sqls As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim fl As String = txtSearch.Text.Trim
        Dim fltr As String = String.Empty
        If fl.Trim <> "" Then
            fltr = " (ReportId LIKE '%" & fl & "%' OR Tbl1 LIKE '%" & fl & "%' OR Tbl2 LIKE '%" & fl & "%' OR Param2 LIKE '%" & fl & "%' OR Tbl1Fld1 LIKE '%" & fl & "%' OR Tbl2Fld2 LIKE '%" & fl & "%') "
        End If

        Dim origin = DropDownListJoinOrigin.Text
        Session("origin") = origin

        Dim er As String = ""
        Dim dv As DataView
        dv = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), ret, Session("logon"), Session("CSV"))
        If dv Is Nothing OrElse dv.Table Is Nothing Then
            Label1.Text = "There are no user tables found for this user."
            Exit Sub
        End If
        Dim ddtv As DataView = GetDBJoins(dv, Session("logon"), userdb, origin, fltr, ret)

        If ret.Trim <> "" Then
            Label1.Text = ret
            lblTablesCount.Text = 0
            Exit Sub
        End If

        If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
            lblTablesCount.Text = 0
            Label1.Text = "There are no new " & DropDownListJoinOrigin.Text & " joins for this db."
            Exit Sub
        Else
            lblTablesCount.Text = ddtv.Table.Rows.Count.ToString & " joins"
        End If
        Dim table1, table2, field1, field2, comm, par1, par2, par3, delurl, rep, indx As String

        For i = 0 To ddtv.Table.Rows.Count - 1
            Try
                rep = ddtv.Table.Rows(i)("ReportId").ToString
            Catch ex As Exception
                ret = " ERROR!! in reading DBJoins"
                Continue For
            End Try
            'If rep <> Session("dbname") & "_joins" OrElse origin = "not used" Then
            AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
                table1 = ddtv.Table.Rows(i)("Tbl1").ToString
                table2 = ddtv.Table.Rows(i)("Tbl2").ToString
                field1 = ddtv.Table.Rows(i)("Tbl1Fld1").ToString
                field2 = ddtv.Table.Rows(i)("Tbl2Fld2").ToString
                comm = ddtv.Table.Rows(i)("comments").ToString
                par1 = ddtv.Table.Rows(i)("Param1").ToString
                par2 = ddtv.Table.Rows(i)("Param2").ToString
                par3 = ddtv.Table.Rows(i)("Param3").ToString
                indx = ddtv.Table.Rows(i)("Indx").ToString

                list.Rows(i + 1).Cells(0).InnerHtml = "<a href='ClassExplorer.aspx?cld=" & table1.Replace("%", "%25") & "'>" & table1 & "</a>"
                list.Rows(i + 1).Cells(1).InnerHtml = field1
                list.Rows(i + 1).Cells(2).InnerHtml = "<a href='ClassExplorer.aspx?cld=" & table2.Replace("%", "%25") & "'>" & table2 & "</a>"
                list.Rows(i + 1).Cells(3).InnerHtml = field2
                list.Rows(i + 1).Cells(4).InnerHtml = par2

                list.Rows(i + 1).Cells(5).InnerHtml = " "
                list.Rows(i + 1).Cells(6).InnerHtml = " "
                list.Rows(i + 1).Cells(7).InnerHtml = " "
                list.Rows(i + 1).Cells(8).InnerHtml = " "

                If par2 = "datadriven" OrElse origin = "not used" Then 'dt Is Nothing OrElse dt.Rows.Count = 0 Then
                    If par3 = "deleted" Then
                        delurl = "ListOfJoins.aspx?undodelindx=" & indx
                        list.Rows(i + 1).Cells(5).InnerHtml = "<a href='" & delurl & "'>undo del</a>"
                        list.Rows(i + 1).BgColor = "LightGray"
                        list.Rows(i + 1).BorderColor = "Gray"

                        list.Rows(i + 1).Cells(6).InnerHtml = "&nbsp;" & rep & "&nbsp;"
                        list.Rows(i + 1).Cells(7).InnerHtml = "&nbsp;"
                        list.Rows(i + 1).Cells(8).InnerHtml = "&nbsp;"

                    Else
                        delurl = "ListOfJoins.aspx?delindx=" & indx
                        list.Rows(i + 1).Cells(5).InnerHtml = "<a href='" & delurl & "'>del</a>"
                        list.Rows(i + 1).BgColor = "White"
                        list.Rows(i + 1).BorderColor = "White"
                        If origin = "not used" Then
                            list.Rows(i + 1).Cells(6).InnerHtml = "&nbsp;" & rep & "&nbsp;"
                            list.Rows(i + 1).Cells(7).InnerHtml = "&nbsp;"
                            list.Rows(i + 1).Cells(8).InnerHtml = "&nbsp;"
                        Else
                            list.Rows(i + 1).Cells(6).InnerHtml = "&nbsp;&nbsp;<a href='" & "ShowReport.aspx?srd=3&REPORT=" & rep & "'>" & rep & "</a>&nbsp;&nbsp;"
                            list.Rows(i + 1).Cells(7).InnerHtml = "&nbsp;&nbsp;<a href='" & "ShowReport.aspx?srd=11&REPORT=" & rep & "'>analytics</a>&nbsp;&nbsp;"
                            list.Rows(i + 1).Cells(8).InnerHtml = "&nbsp;&nbsp;<a href='" & "ShowReport.aspx?srd=12&REPORT=" & rep & "'>correlations</a>&nbsp;&nbsp;"
                        End If

                    End If
                'Else
                '    list.Rows(i + 1).Cells(5).InnerHtml = " - "
                '    list.Rows(i + 1).Cells(6).InnerHtml = "&nbsp;&nbsp;<a href='" & "ShowReport.aspx?srd=3&REPORT=" & rep & "'>" & rep & "</a>&nbsp;&nbsp;"
                '    list.Rows(i + 1).Cells(7).InnerHtml = "&nbsp;&nbsp;<a href='" & "ShowReport.aspx?srd=11&REPORT=" & rep & "'>analytics</a>&nbsp;&nbsp;"
                '    list.Rows(i + 1).Cells(8).InnerHtml = "&nbsp;&nbsp;<a href='" & "ShowReport.aspx?srd=12&REPORT=" & rep & "'>correlations</a>&nbsp;&nbsp;"
                'End If
                If i Mod 2 = 0 Then
                    list.Rows(i + 1).BgColor = "#EFFBFB"
                Else
                    list.Rows(i + 1).BgColor = "white"
                End If
            End If
        Next

    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Response.Redirect(url)
    End Sub

    Private Sub btnCalcJoins_Click(sender As Object, e As EventArgs) Handles btnCalcJoins.Click
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim msql As String = String.Empty
        Dim i As Integer
        Dim dv As DataView

        Dim redo As Boolean = chkRedoJoins.Checked
        Dim origin As String = DropDownListJoinOrigin.Text
        If origin = "custom" Then
            Label1.Text = "Custom joins should not be recalculated."
            Exit Sub
        End If
        If origin = "deleted" Then
            Label1.Text = "Deleted joins should not be recalculated. You can undo delete for each Join. "
            Exit Sub
        End If
        er = ""
        If IsCSVuser(Session("UserDB"), "", er) = "yes" Then
            Session("CSV") = "yes"
        End If
        dv = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), ret, Session("logon"), Session("CSV"))
        If dv Is Nothing OrElse dv.Table Is Nothing Then
            Label1.Text = "There are no user tables found for this user."
            Exit Sub
        End If
        er = ""
        'already not custom and not deleted !!!!!!!!!!!!!!
        If redo Then
            'delete user joins
            ret = DeleteUserJoinsAndReports(dv, origin, Session("CSV"), Session("UserDB"), er)
        End If

        If origin = "datadriven" Then
            'check if initial reports exists for all user tables
            Dim tbl As String = String.Empty
            Dim sqltxt As String = String.Empty
            Dim repdb As String = Session("UserConnString")
            'If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
            'If Session("UserConnString").IndexOf("UID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
            If Session("UserConnProvider").ToString <> "Oracle.ManagedDataAccess.Client" Then
                If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                If Session("UserConnString").IndexOf("UID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
            Else
                If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
            End If

            Dim dbname As String = GetDataBase(Session("UserConnString"))
            Session("dbname") = dbname
            For i = 0 To dv.Table.Rows.Count - 1
                tbl = dv.Table.Rows(i)("TABLE_NAME")
                repid = dbname.Replace(" ", "") & "_INIT_" & i.ToString & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                repid = repid.Replace(" ", "").Replace("#", "")
                If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
                    sqltxt = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(tbl), Session("UserConnProvider"))
                Else
                    'TODO Is it needed to do CorrectTableNameWithDots(tbl) for SQL Server and MySql?
                    sqltxt = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(tbl), Session("UserConnProvider"))
                End If
                If Not ReportForTableExist(tbl, er, repdb) Then
                    ret = MakeNewStanardReport(Session("logon"), repid, tbl, repdb, sqltxt, dbname, Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                End If
            Next
            'create datadriven joins
            ret = MakeDatadrivenJoins(dv.Table, Session("UserConnString"), Session("UserConnProvider"), ret, redo)

            'Redo datadriven reports
            ret = RedoReports(dv.Table, "datadriven", Session("logon"), Session("dbname"), Session("UserDB"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er, redo)

        ElseIf origin = "initial" Then
            Dim dbname = Session("dbname")
            Dim repid As String = dbname & "_joins"
            'create initial joins
            ret = ret & " Joins: " & MakeInitialJoins(Session("logon"), dv.Table, repid, dbname, Session("UserConnString"), Session("UserConnProvider"), ret, redo)

            'Redo initial reports
            ret = RedoReports(dv.Table, "initial", Session("logon"), Session("dbname"), Session("UserDB"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er, redo)

        End If

        Response.Redirect("~/ListOfJoins.aspx")
    End Sub

    Private Sub DropDownListJoinOrigin_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListJoinOrigin.SelectedIndexChanged
        Session("origin") = DropDownListJoinOrigin.SelectedValue
        Response.Redirect("ListOfJoins.aspx")
    End Sub
End Class

