Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Partial Class SiteAdmin
    Inherits System.Web.UI.Page
    Private Sub SiteAdmin_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
            txtsiteBanner.Text = Session("PAGETTL")
        End If
        txtDescript.Text = ConfigurationManager.AppSettings("SiteFor").ToString
        Dim i, j As Integer
        Dim er As String = String.Empty
        Dim mSql As String = String.Empty
        If Not IsPostBack Then
            'groups dropdown
            If Session("admin") = "super" Then
                mSql = "SELECT * FROM ourunits WHERE (UserConnStr LIKE '%" & Session("UnitDB").Trim.Replace(" ", "%") & "%') "
            ElseIf Session("admin") = "admin" Then
                mSql = "SELECT * FROM ourunits WHERE (Unit='" & Session("Unit") & "') AND (UserConnStr LIKE '" & Session("UnitDB").Trim.Replace(" ", "%") & "%') "
            End If
            Dim dv As DataView = mRecords(mSql, er)
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Session("Groups") = dv.Table.Rows(0)("Prop3").ToString.Trim
            End If
            If Session("Groups") Is Nothing OrElse Session("Groups") = "" Then
                Session("Groups") = "All,"
            End If
            Dim grps() As String = Session("Groups").Split(",")
            ddGroups.Items.Clear()
            For i = 0 To grps.Length - 1
                If grps(i).Trim <> "" Then ddGroups.Items.Add(grps(i))
            Next
            ddGroups.Items.Add("All")
            If Session("Group") Is Nothing OrElse Session("Group") = "" Then
                Session("Group") = "All"
                'ElseIf Not Session("Group") Is Nothing AndAlso Session("Group").ToString.Trim <> "" Then 'AndAlso Session("Group").ToString.Trim <> "All"
            End If
            ddGroups.SelectedItem.Text = Session("Group").ToString.Trim

            'tables dropdown
            Dim dvp As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
            If Not dvp Is Nothing AndAlso Not dvp.Table Is Nothing AndAlso dvp.Table.Rows.Count > 0 Then
                For i = 0 To dvp.Table.Rows.Count - 1
                    DropDownGroupTables.Items.Add(dvp.Table.Rows(i)("TABLE_NAME"))
                Next
            End If
            'selected tables
            'For i = 0 To DropDownGroupTables.Items.Count - 1
            '    If Session("Group").ToString.ToUpper = "ALL" Then
            '        DropDownGroupTables.Items(i).Selected = True
            '    Else
            '        dvp = GetListOfGroupTables(Session("Unit"), Session("Group"))
            '        For j = 0 To dvp.Table.Rows.Count - 1
            '            If DropDownGroupTables.Items(i).Text.Trim.ToUpper = dvp.Table.Rows(j)("TABLE_NAME").ToString.Trim.ToUpper Then
            '                DropDownGroupTables.Items(i).Selected = True
            '            End If
            '        Next
            '    End If
            'Next
        End If
    End Sub
    Private Sub SiteAdmin_Load(sender As Object, e As EventArgs) Handles Me.Load
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Request("msg") IsNot Nothing And Request("msg") <> String.Empty Then
            Dim msg = Request("msg")
            MessageBox.Show(msg, "Message", "OnLoadMsg", Controls_Msgbox.Buttons.OK)
            Exit Sub
        End If

        Dim i, j As Integer
        Dim er As String = String.Empty
        Dim mSql As String = String.Empty
        'If Not IsPostBack Then
        ''groups dropdown
        'mSql = "SELECT * FROM ourunits WHERE (Unit='" & Session("Unit") & "') AND (UserConnStr LIKE '" & Session("UnitDB") & "%') "
        'Dim dv As DataView = mRecords(mSql, er)
        'Session("Groups") = dv.Table.Rows(0)("Prop3").ToString.Trim
        'If Session("Groups") Is Nothing OrElse Session("Groups") = "" Then
        '    Session("Groups") = "All,"
        'End If
        'Dim grps() As String = Session("Groups").Split(",")
        'ddGroups.Items.Clear()
        'For i = 0 To grps.Length - 1
        '    If grps(i).Trim <> "" Then ddGroups.Items.Add(grps(i))
        'Next
        'If Session("Group") Is Nothing OrElse Session("Group") = "" Then
        '    Session("Group") = "All"
        'End If

        'selected group in dropdown
        If Not Session("Group") Is Nothing AndAlso Session("Group").ToString.Trim <> "" Then 'AndAlso Session("Group").ToString.Trim <> "All"
            ddGroups.SelectedItem.Text = Session("Group").ToString.Trim
        End If

        ''tables dropdown
        'Dim dvp As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
        'If Not dvp Is Nothing AndAlso Not dvp.Table Is Nothing AndAlso dvp.Table.Rows.Count > 0 Then
        '    For i = 0 To dvp.Table.Rows.Count - 1
        '        DropDownGroupTables.Items.Add(dvp.Table.Rows(i)("TABLE_NAME"))
        '    Next
        'End If

        'selected tables in dropdown
        For i = 0 To DropDownGroupTables.Items.Count - 1
            If Session("Group").ToString.ToUpper = "ALL" Then
                DropDownGroupTables.Items(i).Selected = True
            Else
                Dim dvp As DataView = GetListOfGroupTables(Session("Unit"), Session("Group"))
                For j = 0 To dvp.Table.Rows.Count - 1
                    If DropDownGroupTables.Items(i).Text.Trim.ToUpper = dvp.Table.Rows(j)("TABLE_NAME").ToString.Trim.ToUpper Then
                        DropDownGroupTables.Items(i).Selected = True
                    End If
                Next
            End If
        Next
        'End If

        Dim userconnstrnopass As String = Session("UserConnString")
        If userconnstrnopass.IndexOf("Password") > 0 Then userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.IndexOf("Password")).Trim()

        Dim srch As String = SearchText.Text.Trim

        'group users
        If Session("admin") = "super" Then
            'AndAlso (Session("Access") Is Nothing OrElse Session("Access").ToString <> "SITEADMIN") Then 'super
            mSql = "SELECT NetId as Logon,Name,Unit,Email,ACCESS as Roles,PERMIT as ""Friendly Names"",RoleApp as Rights,ConnStr,ConnPrv,Comments,Group3 as ""Groups"",EndDate,Indx FROM OURPermits "
            'If Not Request("unitdb") Is Nothing AndAlso Request("unitdb") = "yes" Then
            '    mSql = mSql & " WHERE ConnStr LIKE '%" & Session("UnitDB") & "%' AND (Unit='" & Session("Unit") & "') "
            'Else
            '    'mSql = mSql & " WHERE (RoleApp='super') "
            'End If
        ElseIf Session("admin") = "admin" AndAlso Session("Access") IsNot Nothing AndAlso Session("Access").ToString = "SITEADMIN" Then
            mSql = "SELECT NetId as Logon,Name,Unit,Email,ACCESS as Roles,PERMIT as ""Friendly Names"",RoleApp as Rights,ConnStr,ConnPrv,Comments,Group3 as ""Groups"",EndDate,Indx FROM OURPermits WHERE (RoleApp<>'super') AND (Unit='" & Session("Unit") & "') AND (ConnStr LIKE '" & userconnstrnopass & "%') AND (Application='InteractiveReporting') "
            Session("UnitDB") = userconnstrnopass
        Else
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If ddGroups.SelectedItem.Text.ToUpper <> "ALL" Then
            If mSql.ToUpper.IndexOf(" WHERE ") > 0 Then
                mSql = mSql & " AND "
            Else
                mSql = mSql & " WHERE "
            End If
            mSql = mSql & " (Group3 LIKE '" & ddGroups.SelectedItem.Text & ",%' OR Group3 LIKE '%," & ddGroups.SelectedItem.Text & ",%' OR Group3 LIKE '%," & ddGroups.SelectedItem.Text & "' OR Group3 ='" & ddGroups.SelectedItem.Text & "') "

        End If
        If srch <> "" Then
            If Session("admin") = "super" Then
                mSql = mSql & "  WHERE "
            Else
                mSql = mSql & "  AND "
            End If
            mSql = mSql & " ((NetId LIKE '%" & srch & "%') OR (Name LIKE '%" & srch & "%') OR (Unit LIKE '%" & srch & "%') OR (Email LIKE '%" & srch & "%') OR (Group3 LIKE '%" & srch & "%') OR (ConnStr LIKE '%" & srch & "%') OR (Comments LIKE '%" & srch & "%')) "
        End If
        mSql = mSql & " ORDER BY ConnStr,Unit,NetId"
        Dim dvu As DataView = mRecords(mSql, er)  'Data for report by SQL statement from the OURdb database

        dvu = ConvertMySqlTable(dvu.Table, er).DefaultView
        GridView1.DataSource = dvu
        GridView1.Visible = True
        GridView1.DataBind()

        For i = 0 To GridView1.Rows.Count - 1
            If DateToString(dvu.Table.Rows(i).Item("EndDate").ToString) <= DateToString(Now).ToString Then
                GridView1.Rows(i).BackColor = Color.Gray '.FromArgb(152, 203, 203)
            End If
        Next
    End Sub
    Private Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataBind()
        For i = 0 To GridView1.Rows.Count - 1
            If GridView1.Rows(i).Cells(12).Text <= DateToString(Now).ToString Then
                GridView1.Rows(i).BackColor = Color.Gray '.FromArgb(152, 203, 203)
            End If
        Next
    End Sub

    Private Sub btnRegistration_Click(sender As Object, e As EventArgs) Handles btnRegistration.Click
        Response.Redirect("UserDefinition.aspx")
    End Sub
    Protected Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        'Dim srch As String = SearchText.Text.Trim
        'If srch = "" Then
        '    Exit Sub
        'End If

    End Sub

    Private Sub btAddGroup_Click(sender As Object, e As EventArgs) Handles btAddGroup.Click
        If txtGroup.Text.Trim = "" Then
            Exit Sub
        End If
        If Not Session("Groups").ToString.Contains(txtGroup.Text.Trim & ",") Then
            Session("Groups") = Session("Groups").ToString.Trim & txtGroup.Text.Trim & ","
            Dim mSql As String = "UPDATE ourunits SET Prop3 = '" & Session("Groups").ToString.Trim & "' WHERE (Unit ='" & Session("Unit") & "') AND (UserConnStr LIKE '" & Session("UnitDB") & "%') "
            Dim ret As String = ExequteSQLquery(mSql)
        End If
        Response.Redirect("SiteAdmin.aspx")
    End Sub

    Private Sub ddGroups_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddGroups.SelectedIndexChanged
        Session("Group") = ddGroups.Text
        For i = 0 To DropDownGroupTables.Items.Count - 1
            If Session("Group").ToString.ToUpper = "ALL" Then
                DropDownGroupTables.Items(i).Selected = True
            Else
                Dim dvp As DataView = GetListOfGroupTables(Session("Unit"), Session("Group"))
                For j = 0 To dvp.Table.Rows.Count - 1
                    If DropDownGroupTables.Items(i).Text.Trim.ToUpper = dvp.Table.Rows(j)("TABLE_NAME").ToString.Trim.ToUpper Then
                        DropDownGroupTables.Items(i).Selected = True
                    End If
                Next
            End If
        Next
        Response.Redirect("SiteAdmin.aspx")
    End Sub

    Private Sub btTables_Click(sender As Object, e As EventArgs) Handles btTables.Click
        Dim ret As String = String.Empty
        Dim i As Integer
        For i = 0 To DropDownGroupTables.Items.Count - 1
            If DropDownGroupTables.Items(i).Selected Then
                ret = InsertTableIntoOURUserTables(DropDownGroupTables.Items(i).Text, DropDownGroupTables.Items(i).Text, Session("Unit"), Session("logon"), Session("UserDB"), ddGroups.SelectedItem.Text)
            End If
        Next
        Response.Redirect("SiteAdmin.aspx")
    End Sub

    Private Sub ButtonSubmit_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click
        'update banner
        Dim webconfigpath As String = Request.PhysicalApplicationPath & "\web.config"
        Dim sr() As String = File.ReadAllLines(webconfigpath)
        For i = 0 To sr.Length - 1

            If sr(i).Contains("<add key=""SiteFor"" value=""") Then
                sr(i) = "   <add key=""SiteFor"" value=""" & txtDescript.Text.Trim & """/>"
            End If
            If sr(i).Contains("<add key=""pagettl"" value=""") Then
                sr(i) = "   <add key=""pagettl"" value=""" & txtSiteBanner.Text.Trim & """/>"
            End If
        Next
        'write in file
        File.WriteAllLines(webconfigpath, sr)
        Response.Redirect("Default.aspx")
    End Sub

    Protected Sub btnCopyReports_Click(sender As Object, e As EventArgs) Handles btnCopyReports.Click
        Dim ret As String = String.Empty
        If ddOURConnPrv1.Text.Trim = "" Then
            ddOURConnPrv1.BorderColor = Color.Red
            ddOURConnPrv1.Focus()
        ElseIf txtOURdb1.Text.Trim = "" Then
            txtOURdb1.BorderColor = Color.Red
            txtOURdb1.Focus()
        ElseIf txtOURdb1.Text.Trim <> "" AndAlso txtOURdb1.Text.IndexOf("Password") < 0 Then
            txtOURdb1.Text = txtOURdb1.Text & " Password="
            txtOURdb1.BorderColor = Color.Red
            txtOURdb1.Focus()
        ElseIf txtUserdb1.Text.Trim = "" Then
            txtUserdb1.BorderColor = Color.Red
            txtUserdb1.Focus()

        ElseIf ddOURConnPrv2.Text.Trim = "" Then
            ddOURConnPrv2.BorderColor = Color.Red
            ddOURConnPrv2.Focus()
        ElseIf txtOURdb2.Text.Trim = "" Then
            txtOURdb2.BorderColor = Color.Red
            txtOURdb2.Focus()
        ElseIf txtOURdb2.Text.Trim <> "" AndAlso txtOURdb2.Text.IndexOf("Password") < 0 Then
            txtOURdb2.Text = txtOURdb2.Text & " Password="
            txtOURdb2.BorderColor = Color.Red
            txtOURdb2.Focus()
        ElseIf txtUserdb2.Text.Trim = "" Then
            txtUserdb2.BorderColor = Color.Red
            txtUserdb2.Focus()
        Else
            Dim connstr1, connprv1, connstr2, connprv2, userconn1, userconn2 As String
            connstr1 = txtOURdb1.Text
            connprv1 = ddOURConnPrv1.SelectedValue
            connstr2 = txtOURdb2.Text
            connprv2 = ddOURConnPrv2.SelectedValue
            userconn1 = txtUserdb1.Text
            userconn2 = txtUserdb2.Text
            Dim er As String = String.Empty
            Dim tbl As String = "OURReportInfo"
            'Dim tbls As ListItemCollection = ddOURTables.SelectedItems
            'For i = 0 To tbls.Count - 1
            '    tbl = tbls.Item(i).Text
            ret = ret & " | Copy reports from ourdb1 to ourdb2: " & CopyReports(tbl, ddUnit1.SelectedValue, userconn1, connstr1, connprv1, ddUnit2.SelectedValue, userconn2, connstr2, connprv2, er)
            'Next
        End If
        Label2.Text = ret.Replace("Query executed fine.", "").Replace("|", "")
        Label2.Visible = True

    End Sub

    Private Sub btnCopyDashboards_Click(sender As Object, e As EventArgs) Handles btnCopyDashboards.Click
        Dim ret As String = String.Empty
        If ddOURConnPrv1.Text.Trim = "" Then
            ddOURConnPrv1.BorderColor = Color.Red
            ddOURConnPrv1.Focus()
        ElseIf txtOURdb1.Text.Trim = "" Then
            txtOURdb1.BorderColor = Color.Red
            txtOURdb1.Focus()
        ElseIf txtOURdb1.Text.Trim <> "" AndAlso txtOURdb1.Text.IndexOf("Password") < 0 Then
            txtOURdb1.Text = txtOURdb1.Text & " Password="
            txtOURdb1.BorderColor = Color.Red
            txtOURdb1.Focus()
        ElseIf txtUserdb1.Text.Trim = "" Then
            txtUserdb1.BorderColor = Color.Red
            txtUserdb1.Focus()
        ElseIf ddOURConnPrv2.Text.Trim = "" Then
            ddOURConnPrv2.BorderColor = Color.Red
            ddOURConnPrv2.Focus()
        ElseIf txtOURdb2.Text.Trim = "" Then
            txtOURdb2.BorderColor = Color.Red
            txtOURdb2.Focus()
        ElseIf txtOURdb2.Text.Trim <> "" AndAlso txtOURdb2.Text.IndexOf("Password") < 0 Then
            txtOURdb2.Text = txtOURdb2.Text & " Password="
            txtOURdb2.BorderColor = Color.Red
            txtOURdb2.Focus()
        ElseIf txtUserdb2.Text.Trim = "" Then
            txtUserdb2.BorderColor = Color.Red
            txtUserdb2.Focus()
        Else
            Dim connstr1, connprv1, connstr2, connprv2, userconn1, userconn2 As String
            connstr1 = txtOURdb1.Text
            connprv1 = ddOURConnPrv1.SelectedValue
            connstr2 = txtOURdb2.Text
            connprv2 = ddOURConnPrv2.SelectedValue
            userconn1 = txtUserdb1.Text
            userconn2 = txtUserdb2.Text
            Dim er As String = String.Empty
            Dim tbl As String = "OURDashboards"
            ret = ret & " | Copy records from table " & tbl & " : " & UpdateOURTable(tbl, ddUnit1.SelectedValue, userconn1, connstr1, connprv1, ddUnit2.SelectedValue, userconn2, connstr2, connprv2, er)
        End If
        Label2.Text = ret
        Label2.Visible = True
    End Sub
    Protected Function UpdateOURTable(ByVal tbl As String, ByVal unit1 As String, ByVal userconnstr1 As String, ByVal connstr1 As String, ByVal connprv1 As String, ByVal unit2 As String, ByVal userconnstr2 As String, ByVal connstr2 As String, ByVal connprv2 As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim i As Integer = 0
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim rt As Boolean = False
        Dim conndb1 As String = connstr1.Substring(0, connstr1.IndexOf("User ID"))  'user db
        Try
            If connprv1 = "MySql.Data.MySqlClient" Then
                'Dim db As String = GetDataBase(connstr1)
                'sqlt = "SELECT * FROM " & db & "." & tbl.ToLower
                sqlt = "SELECT * FROM " & tbl.ToLower
                'TODO !!!!!! for Oracle.ManagedDataAccess.Client
            Else
                sqlt = "SELECT * FROM " & tbl   '.ToUpper  ???
            End If
            If tbl = "OURFriendlyNames" Then
                sqlt = sqlt & " WHERE " & " UnitDB LIKE '%" & conndb1.Trim.Replace(" ", "%") & "%' AND Unit='" & unit1 & "'"
            ElseIf tbl = "OURPermits" Then
                sqlt = sqlt & " WHERE " & " ConnStr LIKE '%" & conndb1.Trim.Replace(" ", "%") & "%' AND Unit='" & unit1 & "'"
                'csv or group table permissions
            ElseIf tbl = "OURUserTables" Then     'not copy
                sqlt = sqlt & " WHERE " & " UserDB LIKE '%" & conndb1.Trim.Replace(" ", "%") & "%' AND Unit='" & unit1 & "'"
            Else
                Return ret
                Exit Function
            End If
            Dim dt As DataTable
            If connprv1 = "MySql.Data.MySqlClient" Then
                dt = ConvertMySqlTable(mRecords(sqlt, er, connstr1, connprv1).Table)
            Else
                dt = mRecords(sqlt, er, connstr1, connprv1).Table
            End If
            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    ret = RecordExistOrInsertInTargetTable(tbl, dt.Rows(i), unit2, userconnstr2, connstr2, connprv2)
                    'If HasRecords(sqlt, connstr2, connprv2) Then
                    '    'update existing records and add new ones
                    '    If RecordExistInTargetTable(tbl, dt.Rows(i), unit2, userconnstr2, connstr2, connprv2) Then
                    '        'update connection string in the table field to new one
                    '        'rt = RecordUpdateInTargetTable(tbl, dt.Rows(i), unit2, connstr2, connprv2)
                    '        'If rt = True Then
                    '        n = n + 1
                    '        'End If
                    '    Else
                    '        'insert from dt to tbl in connstr2, connprv2
                    '        rt = RecordInsertInTargetTable(tbl, dt.Rows(i), unit2, userconnstr2, connstr2, connprv2)
                    '        If rt = True Then
                    '            m = m + 1
                    '        End If
                    '    End If
                    'Else
                    '    'insert from dt to tbl in connstr2, connprv2
                    '    rt = RecordInsertInTargetTable(tbl, dt.Rows(i), unit2, userconnstr2, connstr2, connprv2)
                    '    If rt = True Then
                    '        m = m + 1
                    '    End If
                    'End If
                Next
            End If
            'ret = n.ToString & " records in table " & tbl & " existed, " & m & " records into table " & tbl & " inserted."
        Catch ex As Exception
            ret = ret & ex.Message
        End Try
        Return ret
    End Function
    Protected Function CopyReports(ByVal tbl As String, ByVal unit1 As String, ByVal userconnstr1 As String, ByVal connstr1 As String, ByVal connprv1 As String, ByVal unit2 As String, ByVal userconnstr2 As String, ByVal connstr2 As String, ByVal connprv2 As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim rt As Boolean = False
        Dim sqlstmt As String = String.Empty
        Dim conndb1 As String = userconnstr1.Substring(0, userconnstr1.IndexOf("User ID"))  'user db1
        Dim conndb2 As String = userconnstr2.Substring(0, userconnstr2.IndexOf("User ID"))  'user db2
        Dim dt As DataTable
        Try
            If tbl = "OURReportInfo" Then
                If connprv2 = "MySql.Data.MySqlClient" Then
                    Dim db As String = GetDataBase(connstr2)
                    'sqlt = "SELECT * FROM " & db.ToLower & "." & tbl.ToLower
                    sqlt = "SELECT * FROM " & tbl.ToLower & " WHERE ReportDB LIKE '%" & conndb1.Trim.Replace(" ", "%") & "%'"
                    dt = ConvertMySqlTable(mRecords(sqlt, er, connstr1, connprv1).Table)
                Else
                    sqlt = "SELECT * FROM " & tbl.ToUpper & " WHERE ReportDB LIKE '%" & conndb1.Trim.Replace(" ", "%") & "%'"
                    dt = mRecords(sqlt, er, connstr1, connprv1).Table
                End If
                'tbl = "OURReportInfo"
                m = 0
                Dim rep As String = String.Empty
                Dim repext As String = unit1 & Now.ToShortDateString.Replace(" ", "").Replace(":", "_").Replace("/", "_").Replace("-", "_")
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    For i = 0 To dt.Rows.Count - 1
                        rep = dt.Rows(i)("ReportID").ToString.Trim
                        tbl = "OURReportInfo"
                        dt.Rows(i)("ReportID") = dt.Rows(i)("ReportID") & repext
                        dt.Rows(i)("Reportdb") = conndb2
                        'InsertRowIntoTable
                        m = m + 1
                        ret = ret & "| " & InsertRowIntoTable(tbl, dt.Rows(i), dt, connstr1, connprv1, connstr2, connprv2)

                        'copy reports
                        tbl = "OURPermissions"
                        Try
                            m = 0
                            Dim dtp As DataTable
                            'CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)
                            If connprv2 = "MySql.Data.MySqlClient" Then
                                'Dim db As String = GetDataBase(connstr2)
                                'sqlt = "SELECT * FROM `" & db & "`.`" & tbl.ToLower & "` "
                                sqlt = "SELECT * FROM `" & tbl.ToLower & "` " & " WHERE Param1='" & rep & "'"
                                dtp = ConvertMySqlTable(mRecords(sqlt, er, connstr1, connprv1).Table)
                            Else
                                sqlt = "SELECT * FROM " & tbl.ToUpper & " WHERE Param1='" & rep & "'"
                                dtp = mRecords(sqlt, er, connstr1, connprv1).Table
                            End If
                            If Not dtp Is Nothing AndAlso dtp.Rows.Count > 0 Then
                                For j = 0 To dtp.Rows.Count - 1
                                    dtp.Rows(j)("Param1") = dtp.Rows(j)("Param1") & repext
                                    dtp.Rows(j)("OpenFrom") = DateToString(Now())
                                    dtp.Rows(j)("OpenTo") = DateToString(Session("Unit2EndDate"))
                                    'insert from dtp to tbl in connstr2, connprv2
                                    m = m + 1
                                    ret = ret & "| " & InsertRowIntoTable(tbl, dtp.Rows(j), dtp, connstr1, connprv1, connstr2, connprv2)
                                Next
                            End If
                        Catch ex As Exception
                            ret = ret & " ERROR!! " & ex.Message
                        End Try
                        ret = ret & ";  " & m.ToString & " records into table " & tbl & " inserted."

                        tbl = "OURReportSQLquery"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                        tbl = "OURReportGroups"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                        tbl = "OURReportLists"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                        tbl = "OURReportFormat"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                        tbl = "OURReportShow"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                        tbl = "OURFiles"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                        tbl = "OURReportItems"
                        ret = ret & CopyReport(rep, repext, tbl, unit1, userconnstr1, connstr1, connprv1, unit2, userconnstr2, connstr2, connprv2, er)

                    Next
                End If
                ret = ret & ";  " & m.ToString & " records into table " & tbl & " inserted."
            End If
        Catch ex As Exception
            ret = ret & ex.Message
        End Try
        Return ret
    End Function
    Protected Function CopyReport(ByVal rep As String, ByVal repext As String, ByVal tbl As String, ByVal unit1 As String, ByVal userconnstr1 As String, ByVal connstr1 As String, ByVal connprv1 As String, ByVal unit2 As String, ByVal userconnstr2 As String, ByVal connstr2 As String, ByVal connprv2 As String, Optional ByRef er As String = "") As String
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim i As Integer = 0
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim rt As Boolean = False
        ' Dim conndb2 As String = userconnstr2.Substring(0, userconnstr2.IndexOf("User ID"))  'user db2
        Try
            Dim dtc As DataTable = Nothing
            'copy report records from supplemental tables
            If connprv1 = "MySql.Data.MySqlClient" Then
                'Dim db As String = GetDataBase(connstr2)
                'sqlt = "SELECT * FROM `" & db & "`.`" & tbl.ToLower & "` "
                sqlt = "SELECT * FROM `" & tbl.ToLower & "` "
            Else
                sqlt = "SELECT * FROM " & tbl.ToUpper
            End If

            sqlt = sqlt & " WHERE ReportID='" & rep & "'"
            'tbl = "OURReportSQLquery"  
            'tbl = "OURReportGroups"    
            'tbl = "OURReportLists"     
            'tbl = "OURReportFormat"    
            'tbl = "OURReportShow"    
            'tbl = "OURFiles" 
            'tbl = "OURReportItems"
            m = 0
            dtc = mRecords(sqlt, er, connstr1, connprv1).Table
            If Not dtc Is Nothing AndAlso dtc.Rows.Count > 0 Then
                For i = 0 To dtc.Rows.Count - 1
                    dtc.Rows(i)("ReportID") = dtc.Rows(i)("ReportID") & repext
                    'insert from dt to tbl in connstr2, connprv2
                    m = m + 1
                    ret = ret & "| " & InsertRowIntoTable(tbl, dtc.Rows(i), dtc, connstr1, connprv1, connstr2, connprv2)
                Next
            End If
            ret = ret & ";  " & m.ToString & " records into table " & tbl & " inserted."
        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Protected Function RecordExistOrInsertInTargetTable(ByVal tbl As String, ByVal dr As DataRow, ByVal unit As String, ByVal userconnstr As String, ByVal connstr As String, ByVal connprv As String) As String
        'dr - datarow from old table, tbl - target table
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim rt As Boolean = False
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim conndb As String = connstr.Substring(0, connstr.IndexOf("User ID")).Trim
        Dim userdb As String = userconnstr.Substring(0, userconnstr.IndexOf("User ID"))  'user db
        Try
            If connprv = "MySql.Data.MySqlClient" Then
                Dim db As String = GetDataBase(connstr)
                'sqlt = "SELECT * FROM " & db & "." & tbl.ToLower
                sqlt = "SELECT * FROM " & tbl.ToLower
                'TODO !!!!!! for Oracle.ManagedDataAccess.Client
            Else
                sqlt = "SELECT * FROM " & tbl.ToUpper
            End If
            If tbl = "OURFriendlyNames" Then
                sqlt = sqlt & " WHERE " & " UnitDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND Unit='" & unit & "' AND  TableName='" & dr("TableName") & "' AND  FieldName='" & dr("FieldName") & "'"
            ElseIf tbl = "OURPermits" Then
                sqlt = sqlt & " WHERE " & " ConnStr LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND Unit='" & unit & "' AND  NetId='" & dr("NetId") & "'"
                'csv or group table permissions
            ElseIf tbl = "OURUserTables" Then     'not copy
                sqlt = sqlt & " WHERE " & " UserDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%' AND Unit='" & unit & "' AND TableName='" & dr("TableName") & "'"
            Else
                Return ret
                Exit Function
            End If
            If HasRecords(sqlt, connstr, connprv) Then
                rt = True
                n = n + 1
                ret = n.ToString & " records in table " & tbl & " existed, " & m & " records into table " & tbl & " inserted." & ret
            Else
                rt = False
                'insert record in target table
                dr("Unit") = unit
                If tbl = "OURFriendlyNames" Then
                    dr("UnitDB") = userconnstr
                ElseIf tbl = "OURPermits" Then
                    dr("ConnStr") = userconnstr
                    'csv or group table permissions
                ElseIf tbl = "OURUserTables" Then     'not copy
                    dr("UserDB") = userconnstr
                Else
                    Return ret
                    Exit Function
                End If
                m = m + 1
                ret = AddRowIntoTable(dr, tbl, connstr, connprv)
                ret = n.ToString & " records in table " & tbl & " existed, " & m & " records into table " & tbl & " inserted." & ret
            End If
            ret = n.ToString & " records in table " & tbl & " existed, " & m & " records into table " & tbl & " inserted."
        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "OnLoadMsg" Then
            Response.Redirect("SiteAdmin.aspx?unitdb=yes")
        End If
    End Sub
End Class
