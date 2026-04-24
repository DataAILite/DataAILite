Imports System.Data
Imports System.Math
Imports System.Drawing
Imports System.Diagnostics
Imports System.IO
Partial Class InstallIt
    Inherits System.Web.UI.Page
    Dim process As System.Diagnostics.Process
    Private Sub InstallIt_Init(sender As Object, e As EventArgs) Handles Me.Init
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("admin").ToString <> "super" Then
            Response.Redirect("~/Default.aspx")
        Else
            HyperLink1.Enabled = True
            HyperLink1.Visible = True
            HyperLink2.Enabled = True
            HyperLink2.Visible = True
        End If
        HyperLinkHelpDesk.Visible = False
        HyperLinkHelpDesk.Enabled = False
        Label11.Visible = False
        Dim url As String = HttpContext.Current.Request.Url.AbsoluteUri
        If url.Contains("localhost") Then
            process = New System.Diagnostics.Process()
            ddTickets.Visible = True
            ddTickets.Enabled = True
            txtIP.Visible = True
            txtIP.Enabled = True
            btnRemoteConnect.Visible = True
            btnRemoteConnect.Enabled = True
            btnRemoteDisconnect.Visible = True
            btnRemoteDisconnect.Enabled = True
            txtIP.Text = ConfigurationManager.AppSettings("OUReportsServer").ToString
        Else
            ddTickets.Visible = False
            ddTickets.Enabled = False
            txtIP.Visible = False
            txtIP.Enabled = False
            btnRemoteConnect.Visible = False
            btnRemoteConnect.Enabled = False
            btnRemoteDisconnect.Visible = False
            btnRemoteDisconnect.Enabled = False
        End If
        Dim i As Integer
        Dim er As String = String.Empty
        Dim mSql As String = String.Empty
        mSql = "SELECT DISTINCT Unit FROM OURUnits ORDER BY Unit"
        Dim dv As DataView = mRecords(mSql, er)  'Data for report by SQL statement from the OURdb database
        ddUnit1.Items.Add(" ")
        ddUnit2.Items.Add(" ")
        If er = "" AndAlso Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
            For i = 0 To dv.Table.Rows.Count - 1
                ddUnit1.Items.Add(dv.Table.Rows(i)("Unit"))
                ddUnit2.Items.Add(dv.Table.Rows(i)("Unit"))
            Next
        End If

        mSql = "SELECT * FROM OURHelpDesk ORDER BY ID DESC"
        dv = mRecords(mSql, er)  'Data for report by SQL statement from the OURdb database
        If er = "" AndAlso Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
            For i = 0 To dv.Table.Rows.Count - 1
                Dim itm As New ListItem
                itm.Value = dv.Table.Rows(i)("ID").ToString
                itm.Text = "#" & itm.Value & ": " & dv.Table.Rows(i)("Ticket").ToString
                If itm.Text.Length > 40 Then itm.Text = itm.Text.Substring(0, 40) & "..."
                ddTickets.Items.Add(itm)
            Next
        End If

        ddOURTables.Items.Add("OURUserTables")
        ddOURTables.Items.Add("OURPermits")
        ddOURTables.Items.Add("OURFriendlyNames")
        'ddOURTables.Items.Add("OURPermissions")
        'ddOURTables.Items.Add("OURReportInfo")
        'ddOURTables.Items.Add("OURReportSQLquery")
        'ddOURTables.Items.Add("OURReportGroups")
        'ddOURTables.Items.Add("OURReportLists")
        'ddOURTables.Items.Add("OURReportFormat")
        'ddOURTables.Items.Add("OURReportShow")
        'ddOURTables.Items.Add("OURFiles")
        'ddOURTables.Items.Add("OURUnits")
        'ddOURTables.Items.Add("OURHelpDesk")
        'ddOURTables.Items.Add("OURAccessLog")
        'ddOURTables.Items.Add("OURReportChildren")
        If Not IsPostBack Then
            TextBoxDBConn.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            DropDownListDBProv.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim err As String = String.Empty
        Dim mSql As String = String.Empty
        Dim srch As String = SearchText.Text.Trim
        mSql = "SELECT Unit,DistrMode,UnitWeb,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Comments,StartDate,EndDate,Indx FROM OURUnits "
        If srch <> "" Then
            mSql = mSql & "  WHERE "
            mSql = mSql & " ((Unit LIKE '%" & srch & "%') OR (DistrMode LIKE '%" & srch & "%') OR (UnitWeb LIKE '%" & srch & "%') OR (OURConnStr LIKE '%" & srch.Trim.Replace(" ", "%") & "%') OR (UserConnStr LIKE '%" & srch.Trim.Replace(" ", "%") & "%') OR (Comments LIKE '%" & srch & "%')) "
        End If
        mSql = mSql & " ORDER BY Unit, UserConnStr"
        Try
            Dim dvu As DataView = mRecords(mSql, err)  'Data for report by SQL statement from the OURdb database
            If err <> "" Then
                Label8.Text = err
                Exit Sub
            End If
            Label8.Text = dvu.Table.Rows.Count.ToString & " units"
            GridViewUnits.DataSource = dvu
            GridViewUnits.Visible = True
            GridViewUnits.DataBind()
        Catch ex As Exception
            Label8.Text = ex.Message
        End Try
        If DropDownListDBProv.SelectedValue = "Npgsql" Then
            chkUserDBcase.Visible = True
            chkUserDBcase.Enabled = True
        Else
            chkUserDBcase.Visible = False
            chkUserDBcase.Enabled = False
        End If
        If chkUserDBcase.Checked Then
            DataModule.userdbcase = "doublequoted"
        Else
            DataModule.userdbcase = "lower"
        End If
        'activity grid
        mSql = "SELECT Ticket,Activity,ActivityType,ConnOpen,ConnClosed,ConnectedBy,ConnectedServer,Comments,Indx FROM OURActivity "
        'If srch <> "" Then
        '    mSql = mSql & "  WHERE "
        '    mSql = mSql & " ((Unit LIKE '%" & srch & "%') OR (DistrMode LIKE '%" & srch & "%') OR (UnitWeb LIKE '%" & srch & "%') OR (OURConnStr LIKE '%" & srch & "%') OR (UserConnStr LIKE '%" & srch & "%') OR (Comments LIKE '%" & srch & "%')) "
        'End If
        mSql = mSql & " ORDER BY Indx DESC"
        Try
            Dim dva As DataView = mRecords(mSql, err)  'Data by SQL statement from the OURdb database
            If err <> "" Then
                Label11.Text = err
                Exit Sub
            End If
            Label11.Text = dva.Table.Rows.Count.ToString & " activities"
            GridViewConnections.DataSource = dva
            GridViewConnections.Visible = True
            GridViewConnections.DataBind()
        Catch ex As Exception
            Label11.Text = ex.Message
        End Try

        Dim url As String = HttpContext.Current.Request.Url.AbsoluteUri
        Session("URL") = url.Substring(0, url.LastIndexOf("/")) & "/"
        If url.ToUpper.StartsWith(Session("WEBHELPDESK").ToUpper) Then
            HyperLinkHelpDesk.Visible = True
            HyperLinkHelpDesk.Enabled = True
            btnUnits.Visible = True
            btnUnitRegistration.Visible = True
            GridViewUnits.Visible = True
            GridViewUnits.Enabled = True
            Label8.Enabled = True
            Label8.Visible = True
        Else
            HyperLinkHelpDesk.Visible = False
            HyperLinkHelpDesk.Enabled = False
            btnUnits.Visible = False
            btnUnitRegistration.Visible = False
            GridViewUnits.Visible = False
            GridViewUnits.Enabled = False
            Label8.Enabled = False
            Label8.Visible = False
        End If
        If Not IsPostBack Then
            TextBoxDBConn.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            DropDownListDBProv.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        End If
    End Sub
    Private Sub InstallIt_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim myconstring, myprovider As String
        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        myprovider = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Session("OURConnString") = myconstring
        Session("OURConnProvider") = myprovider
        If myprovider = "InterSystems.Data.IRISClient" Then
            'If myconstring = "" Then myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
            Label1.Text = "OUR IRIS Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("USER ID"))
        ElseIf myprovider = "InterSystems.Data.CacheClient" Then
            'If myconstring = "" Then myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
            Label1.Text = "OUR Cache Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("USER ID"))
        ElseIf myprovider = "MySql.Data.MySqlClient" Then
            Label1.Text = "OUR MySql Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("USER ID"))
        ElseIf myprovider = "Oracle.ManagedDataAccess.Client" Then
            Label1.Text = "OUR Oracle Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("PASSWORD"))
        ElseIf myprovider = "System.Data.Odbc" Then
            Label1.Text = "OUR Odbc Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("UID"))
        ElseIf myprovider = "Npgsql" Then
            Label1.Text = "OUR PostgreSQL Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("USER ID"))
        Else  'SQL Server
            Label1.Text = "OUR Sql Server Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("USER ID"))
        End If
        If Session("UserConnString") = "" Then
            Session("UserConnString") = Session("OURConnString")
            Session("UserConnProvider") = Session("OURConnProvider")
        End If
        If Session("UserConnString") <> "" Then
            myconstring = Session("UserConnString")
            If Session("UserConnProvider") = "InterSystems.Data.IRISClient" Then
                myconstring = myconstring.Substring(0, myconstring.IndexOf("User ID"))
                Label5.Text = "User IRIS Database: " & myconstring
            ElseIf Session("UserConnProvider") = "InterSystems.Data.CacheClient" Then
                'If myconstring = "" Then myconstring = InterSystems.Data.CacheClient.CacheConnection.ConnectDlg()
                myconstring = myconstring.Substring(0, myconstring.IndexOf("User ID"))
                Label5.Text = "User Cache Database: " & myconstring
            ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
                myconstring = myconstring.Substring(0, myconstring.IndexOf("User ID"))
                Label5.Text = "User MySql Database: " & myconstring
            ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
                myconstring = myconstring.Substring(0, myconstring.IndexOf("Password"))
                Label5.Text = "User Oracle Database: " & myconstring
            ElseIf Session("UserConnProvider") = "System.Data.Odbc" Then
                Label5.Text = "User Odbc Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("UID"))
            ElseIf Session("UserConnProvider") = "Npgsql" Then
                Label5.Text = "User PostgreSQL Database: " & myconstring.Substring(0, myconstring.ToUpper.IndexOf("USER ID"))
            Else 'SQL Server
                myconstring = myconstring.Substring(0, myconstring.IndexOf("User ID"))
                Label5.Text = "User Sql Server Database: " & myconstring
            End If
        End If
        Label2.Text = ""
        If Session("spam") IsNot Nothing AndAlso Session("spam") = "yes" Then Response.Redirect("~/ourspam.aspx")
        'If Session("logon").ToString = "super" Then
        '    divSpam.Style("display") = ""
        'Else
        '    divSpam.Style("display") = "none"
        'End If
    End Sub

    'Protected Sub ButtonInstall_Click(sender As Object, e As EventArgs) Handles ButtonInstall.Click
    '    Dim ret As String = InstallTablesClasses()
    '    ButtonInstall.Enabled = False
    '    ButtonInstall.Visible = False
    '    Label2.Text = ret 'cleanText(ret)
    'End Sub
    'Protected Sub ButtonUninstall_Click(sender As Object, e As EventArgs) Handles ButtonUninstall.Click
    '    Dim ret As String = UninstallOURTablesClasses()
    '    ButtonUninstall.Enabled = False
    '    ButtonUninstall.Visible = False
    '    Label2.Text = ret 'cleanText(ret)
    'End Sub

    Protected Sub ButtonPrepareSQL_Click(sender As Object, e As EventArgs) Handles ButtonPrepareSQL.Click
        Label4.Text = ""
        Try
            Dim sqltext = TextBoxSQL.Text.Trim
            Dim temp As String = String.Empty
            'UPDATE
            If sqltext.Trim.ToUpper.StartsWith("UPDATE ") Then
                Dim i As Integer = sqltext.ToUpper.IndexOf(" FROM ")
                If i > 0 Then
                    sqltext = "SELECT * " & sqltext.Substring(i)
                Else
                    i = sqltext.ToUpper.IndexOf(" SET ")
                    temp = sqltext.Substring(7, i - 7).Trim
                    If Session("OURConnProvider") = "MySql.Data.MySqlClient" Then
                        temp = temp.ToLower
                        'TODO !!!!!! for Oracle.ManagedDataAccess.Client
                    End If
                    i = sqltext.ToUpper.IndexOf(" WHERE ")
                    sqltext = "SELECT * FROM " & temp & sqltext.Substring(i)
                End If
                'DELETE
            ElseIf sqltext.Trim.ToUpper.StartsWith("DELETE ") Then
                Dim i As Integer = sqltext.ToUpper.IndexOf(" FROM ")
                sqltext = "SELECT * " & sqltext.Substring(i)
                'INSERT
            ElseIf sqltext.Trim.ToUpper.StartsWith("INSERT ") Then
                Dim i, j As Integer
                Dim flds(), vals() As String
                Dim tbl As String = String.Empty
                flds = sqltext.Substring(sqltext.IndexOf("("), sqltext.IndexOf(")") - sqltext.IndexOf("(")).Replace("(", "").Replace(")", "").Split(",")
                i = MAX(sqltext.ToUpper.IndexOf(" VALUES("), sqltext.ToUpper.IndexOf(" VALUES "))
                vals = sqltext.Substring(i + 8).Replace("(", "").Replace(")", "").Split(",")
                i = flds.Length
                j = vals.Length
                If i <> j Then
                    Label4.Text = "Wrong SQL statement. Number of fields should be the same as number of values..."
                    Label4.Visible = True
                    Exit Sub
                End If
                tbl = sqltext.Substring(sqltext.ToUpper.IndexOf(" INTO ") + 6, sqltext.IndexOf("(") - sqltext.ToUpper.IndexOf(" INTO ") - 6).Replace("(", "").Trim
                If tbl = "" Then
                    Label4.Text = "Wrong SQL statement. Table/Class is missing..."
                    Label4.Visible = True
                    Exit Sub
                End If
                sqltext = "SELECT * FROM " & tbl & " WHERE ("
                For i = 0 To j - 1
                    vals(i) = vals(i).Replace("'", "")
                    If TblFieldIsNumeric(tbl, flds(i)) Then
                        sqltext = sqltext & flds(i) & "=" & vals(i)
                    Else
                        sqltext = sqltext & flds(i) & "='" & vals(i) & "'"
                    End If
                    If i < j - 1 Then
                        sqltext = sqltext & " AND "
                    Else
                        sqltext = sqltext & ")"
                    End If
                Next
            End If
            Dim dv As DataView
            dv = mRecords(sqltext) 'ONLY for our or user db
            GridView1.DataSource = dv
            GridView1.Visible = True
            GridView1.DataBind()
        Catch ex As Exception
            Label4.Text = "ERROR!!   " & ex.Message
            Label4.Visible = True
        End Try
    End Sub
    Protected Sub ButtonRunSQL_Click(sender As Object, e As EventArgs) Handles ButtonRunSQL.Click
        Dim sqltext = TextBoxSQL.Text.Trim
        Dim ret As String = ExequteSQLquery(TextBoxSQL.Text.Trim) 'ONLY for ourdb
        Label4.Text = ret
        If sqltext.ToUpper.StartsWith("SELECT ") = False Then
            Dim temp As String = String.Empty
            'UPDATE
            If sqltext.Trim.ToUpper.StartsWith("UPDATE ") Then
                Dim i As Integer = sqltext.ToUpper.IndexOf(" FROM ")
                If i > 0 Then
                    sqltext = "SELECT * " & sqltext.Substring(i)
                Else
                    i = sqltext.ToUpper.IndexOf(" SET ")
                    temp = sqltext.Substring(7, i - 7).Trim
                    If Session("OURConnProvider") = "MySql.Data.MySqlClient" Then
                        temp = temp.ToLower
                        'TODO !!!!!! for Oracle.ManagedDataAccess.Client
                    End If
                    i = sqltext.ToUpper.IndexOf(" WHERE ")
                    sqltext = "SELECT * FROM " & temp & sqltext.Substring(i)
                End If
                'DELETE
            ElseIf sqltext.Trim.ToUpper.StartsWith("DELETE ") Then
                Dim i As Integer = sqltext.ToUpper.IndexOf(" FROM ")
                sqltext = "SELECT * " & sqltext.Substring(i)
            End If
        End If
        Dim dv As DataView
        dv = mRecords(sqltext) 'ONLY for our or user db
        GridView1.DataSource = dv
        GridView1.Visible = True
        GridView1.DataBind()
    End Sub
    'Protected Sub ButtonPermits_Click(sender As Object, e As EventArgs) Handles ButtonPermits.Click
    '    Dim ret As String = FixDatetimeInOURPermits()
    '    If ret <> "" Then
    '        'WriteToAccessLog(Session("logon"), "DateTime zeros fixed in OURPermits.", 1)
    '        Label2.Text = "DateTime zeros fixed in OURPermits."
    '    End If
    'End Sub
    'Protected Sub ButtonPermissions_Click(sender As Object, e As EventArgs) Handles ButtonPermissions.Click
    '    Dim ret As String = FixDatetimeInOURPermissions()
    '    If ret <> "" Then
    '        'WriteToAccessLog(Session("logon"), "DateTime zeros fixed in OURPermissions.", 1)
    '        Label2.Text = "DateTime zeros fixed in OURPermissions."
    '    End If
    'End Sub
    'Protected Sub ButtonAccessLog_Click(sender As Object, e As EventArgs) Handles ButtonAccessLog.Click
    '    Dim ret As String = String.Empty
    '    Dim err As String = String.Empty
    '    Dim mrec As DataView
    '    mrec = mRecords("SELECT * FROM OURAccessLog", err)
    '    If err <> "" Then
    '        'fix DateTime fields
    '        ret = ExequteSQLquery("UPDATE OURAccessLog SET EventDate=NULL WHERE EventDate=0")
    '        WriteToAccessLog(Session("logon"), "DateTime zeros fixed in OURAccessLog.", 1)
    '    End If
    'End Sub
    'Protected Sub ButtonAddDays_Click(sender As Object, e As EventArgs) Handles ButtonAddDays.Click
    '    Dim webinstall As String = ConfigurationManager.AppSettings("webinstall").ToString
    '    Dim dbinstall As String = ConfigurationManager.AppSettings("dbinstall").ToString
    '    If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
    '        Dim lgn As String = TextBoxUser.Text
    '        Dim amnt As String = TextBoxNumber.Text
    '        Dim ret As String = ProcessPayment(lgn, amnt)
    '        Label2.Text = ret
    '        Label2.Visible = True
    '        WriteToAccessLog(Session("logon"), lgn & " paid is added manually: +" & amnt.ToString & ret, 3)
    '    Else
    '        Label2.Text = "You can add paid days only for the OUR distribution model #4: OURweb-OURdb. Check web.config for the distribution model."
    '        Label2.Visible = True
    '    End If
    'End Sub

    Private Sub btnUnits_Click(sender As Object, e As EventArgs) Handles btnUnits.Click
        Response.Redirect("UnitsAdmin.aspx")
    End Sub

    Private Sub btnUnitRegistration_Click(sender As Object, e As EventArgs) Handles btnUnitRegistration.Click
        Response.Redirect("UnitDefinition.aspx")
    End Sub
    Private Sub GridViewUnits_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridViewUnits.PageIndexChanging
        GridViewUnits.PageIndex = e.NewPageIndex
        GridViewUnits.DataBind()
    End Sub
    'Protected Sub ButtonInstallVersion2_Click(sender As Object, e As EventArgs) Handles ButtonInstallVersion2.Click
    '    Dim ret As String
    '    ButtonInstallVersion2.Enabled = False
    '    ButtonInstallVersion2.Visible = False
    '    ret = InstallOURFriendlyNames()
    '    ret = ret & "<br>" & InstallOURFiles()
    '    Label2.Text = ret
    'End Sub
    'Protected Sub ButtonUninstallVersion2_Click(sender As Object, e As EventArgs) Handles ButtonUninstallVersion2.Click
    '    Dim ret As String
    '    ButtonUninstallVersion2.Enabled = False
    '    ButtonUninstallVersion2.Visible = False
    '    ret = UninstallOURFriendlyNames()
    '    ret = ret & "<br>" & UninstallOURFiles()
    '    Label2.Text = ret
    'End Sub
    'Protected Sub ButtonMakeReports_Click(sender As Object, e As EventArgs) Handles ButtonMakeReports.Click
    '    Dim er As String = String.Empty
    '    Dim userconnstr As String = DropDownListProvider.SelectedItem.Text
    '    Dim userconnpr As String = DropDownListConnStr.SelectedItem.Text
    '    Try
    '        If Not HasRecords("SELECT * FROM OURReportInfo WHERE ReportDB LIKE '%" & DropDownListConnStr.SelectedItem.Text.Trim.Replace(" ", "%") & "%'") Then
    '            Dim dv As DataView = GetListOfUserTables(False, userconnstr, userconnpr, er) 'password is needed!!!
    '        End If
    '        Label7.Text = "Creating new reports is finished successfully. "
    '    Catch ex As Exception
    '        Label7.Text = "ERROR!! creating new reports: " & ex.Message
    '    End Try
    'End Sub

    'Protected Sub DropDownListProvider_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListProvider.SelectedIndexChanged
    '    Dim dv As DataView = mRecords("SELECT DISTINCT ConnStr,ConnPrv FROM OURPermits WHERE ConnPrv='" & DropDownListProvider.Text & "'")
    '    DropDownListConnStr.Items.Clear()
    '    For i = 0 To dv.Table.Rows.Count - 1
    '        DropDownListConnStr.Items.Add(dv.Table.Rows(i)("ConnStr").ToString)
    '    Next
    '    Label6.Text = "User DB is " & DropDownListProvider.Text & " with Connection string: " & DropDownListConnStr.Text

    'End Sub
    'Protected Sub ButtonMakeOURReports_Click(sender As Object, e As EventArgs) Handles ButtonMakeOURReports.Click
    '    Dim ret As String = String.Empty
    '    Try
    '        Dim myconstring As String = Session("OURConnString")
    '        myconstring = myconstring.Substring(0, myconstring.IndexOf("User ID")).Trim
    '        Dim repid As String = Session("logon")
    '        If Not HasRecords("SELECT * FROM OURReportInfo WHERE ReportDB LIKE '%" & myconstring.Trim.Replace(" ", "%") & "%'") Then
    '            Dim dvp As DataView = GetListOfUserTables(False) 'password is needed!!!
    '            If Not dvp Is Nothing AndAlso Not dvp.Table Is Nothing AndAlso dvp.Table.Rows.Count > 0 Then
    '                'fill out provider dropdown
    '                For i = 0 To dvp.Table.Rows.Count - 1
    '                    'make report for each table
    '                    Dim tbl As String = dvp.Table.Rows(i)("TABLE_NAME")
    '                    Dim sqltxt As String = "SELECT * FROM " & tbl
    '                    Dim sqlq As String = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,SQLquerytext,Param7type,Param9type,ReportDB) VALUES ('" & repid & "_" & tbl & "','" & tbl & "','" & tbl & "','rdl','sql','" & sqltxt & "','standard','landscape','" & myconstring & "')"
    '                    Dim er As String = ExequteSQLquery(sqlq)
    '                    If er = "Query executed fine." Then
    '                        ret = ret & "Report for table " & tbl & " created. <br>"
    '                    Else
    '                        ret = ret & "Report for table " & tbl & " crashed. <br>"
    '                    End If
    '                    'add permissions

    '                Next
    '            End If
    '        End If
    '        Label8.Text = "Creating new reports is finished successfully. "
    '    Catch ex As Exception
    '        Label8.Text = "ERROR!! creating new reports: " & ex.Message
    '    End Try
    'End Sub
    Protected Sub CheckBoxSelectAllOURTables_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSelectAllOURTables.CheckedChanged
        Dim j As Integer
        Dim n As Integer = 0
        ddOURTables.Text = ""
        If CheckBoxSelectAllOURTables.Checked Then
            CheckBoxUnselectAllOURTables.Checked = False
            For j = 0 To ddOURTables.Items.Count - 1   'all fields in drop-down selected
                ddOURTables.Items(j).Selected = True
                n = n + 1
                If n = 1 Then
                    ddOURTables.Text = ddOURTables.Items(j).Text
                ElseIf n > 1 Then
                    ddOURTables.Text = ddOURTables.Text & "," & ddOURTables.Items(j).Text
                End If

            Next
        End If
    End Sub
    Protected Sub CheckBoxUnselectAllOURTables_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxUnselectAllOURTables.CheckedChanged
        Dim j As Integer
        If CheckBoxUnselectAllOURTables.Checked Then
            CheckBoxSelectAllOURTables.Checked = False
            For j = 0 To ddOURTables.Items.Count - 1   'all fields in drop-down selected
                ddOURTables.Items(j).Selected = False
            Next
            ddOURTables.Text = ""
        End If
    End Sub
    Protected Sub btnUpdateOURTables_Click(sender As Object, e As EventArgs) Handles btnUpdateOURTables.Click
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
            Dim i As Integer
            Dim connstr1, connprv1, connstr2, connprv2, userconn1, userconn2 As String
            connstr1 = txtOURdb1.Text
            connprv1 = ddOURConnPrv1.SelectedValue
            connstr2 = txtOURdb2.Text
            connprv2 = ddOURConnPrv2.SelectedValue
            userconn1 = txtUserdb1.Text
            userconn2 = txtUserdb2.Text
            Dim er As String = String.Empty
            Dim tbl As String = String.Empty
            Dim tbls As ListItemCollection = ddOURTables.SelectedItems
            For i = 0 To tbls.Count - 1
                tbl = tbls.Item(i).Text
                ret = ret & " | Copy records from table " & tbl & " : " & UpdateOURTable(tbl, ddUnit1.SelectedValue, userconn1, connstr1, connprv1, ddUnit2.SelectedValue, userconn2, connstr2, connprv2, er)
            Next
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
                    Dim db As String = GetDataBase(connstr2, connprv2)
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
    'Function InsertRowIntoTable(ByVal tbl As String, ByVal mRow As DataRow, ByRef dt As DataTable, ByVal connstr2 As String, ByVal connprv2 As String) As String
    '    Dim ret As String = String.Empty
    '    Dim sqlstmt As String = "INSERT INTO " & tbl & " SET "
    '    Try


    '        sqlstmt = "INSERT INTO " & tbl & " SET "
    '        For j = 0 To dt.Columns.Count - 1
    '            If dt.Columns(j).Caption <> "Indx" AndAlso dt.Columns(j).Caption <> "ID" Then
    '                sqlstmt = sqlstmt & dt.Columns(j).Caption & "="
    '                If TblFieldIsNumeric(tbl, dt.Columns(j).Caption, connstr2, connprv2) Then
    '                    sqlstmt = sqlstmt & mRow(j)
    '                Else
    '                    sqlstmt = sqlstmt & "'" & mRow(j) & "'"
    '                End If
    '                If j < dt.Columns.Count - 1 Then
    '                    sqlstmt = sqlstmt & ","
    '                End If
    '            End If
    '        Next
    '        If sqlstmt.EndsWith(",") Then
    '            sqlstmt = sqlstmt.Substring(0, sqlstmt.Length - 1)
    '        End If
    '        ret = ExequteSQLquery(sqlstmt, connstr2, connprv2)
    '    Catch ex As Exception
    '        ret = ex.Message
    '    End Try
    '    Return ret
    'End Function
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
    Protected Function RecordExistInTargetTable(ByVal tbl As String, ByVal dr As DataRow, ByVal unit As String, ByVal userconnstr As String, ByVal connstr As String, ByVal connprv As String) As Boolean
        'dr - datarow from old table, tbl - target table
        Dim rt As Boolean = False
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim conndb As String = connstr.Substring(0, connstr.IndexOf("User ID")).Trim
        Dim userdb As String = userconnstr.Substring(0, userconnstr.IndexOf("User ID"))  'user db
        Try
            If connprv = "MySql.Data.MySqlClient" Then
                Dim db As String = GetDataBase(connstr, connprv)
                'sqlt = "SELECT * FROM " & db & "." & tbl.ToLower
                sqlt = "SELECT * FROM " & tbl.ToLower

                'TODO if needed !!!!!! for Oracle.ManagedDataAccess.Client

            Else
                sqlt = "SELECT * FROM " & tbl.ToUpper
            End If
            If tbl = "OURFriendlyNames" Then
                sqlt = sqlt & " WHERE " & " UnitDB LIKE '%" & conndb.Trim.Replace(" ", "%") & "%' AND Unit='" & unit & "' AND  TableName='" & dr("TableName") & "' AND  FieldName='" & dr("FieldName") & "'"
            ElseIf tbl = "OURPermits" Then
                sqlt = sqlt & " WHERE " & " ConnStr LIKE '%" & conndb.Trim.Replace(" ", "%") & "%' AND Unit='" & unit & "' AND  NetId='" & dr("NetId") & "'"
                'csv or group table permissions
            ElseIf tbl = "OURUserTables" Then     'not copy
                sqlt = sqlt & " WHERE " & " UserDB LIKE '%" & conndb.Trim.Replace(" ", "%") & "%' AND Unit='" & unit & "' AND TableName='" & dr("TableName") & "'"
            Else
                Return ret
                Exit Function
            End If
            If HasRecords(sqlt, connstr, connprv) Then
                rt = True
            Else
                rt = False
            End If
        Catch ex As Exception
            rt = True
        End Try
        Return rt
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
                Dim db As String = GetDataBase(connstr, connprv)
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
    Protected Function RecordUpdateInTargetTable(ByVal tbl As String, ByVal dr As DataRow, ByVal unit As String, ByVal connstr As String, ByVal connprv As String) As Boolean
        'NOT FINISHED YET !!!
        'dr - datarow from old table, tbl - target table
        Dim rt As Boolean = False
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim conndb As String = connstr.Substring(0, connstr.IndexOf("User ID")).Trim
        Try
            If connprv = "MySql.Data.MySqlClient" Then
                Dim db As String = GetDataBase(connstr, connprv)
                sqlt = "UPDATE '" & db & "'." & tbl.ToLower
                'TODO !!!!!! for Oracle.ManagedDataAccess.Client
            Else
                sqlt = "UPDATE " & tbl.ToUpper
            End If
            If tbl = "OURFriendlyNames" Then
                sqlt = sqlt & " SET UnitDb='" & conndb & "' "
                sqlt = sqlt & " WHERE " & " Unit='" & unit & "' AND  UnitDB LIKE '%" & conndb.Trim.Replace(" ", "%") & "%' AND  TableName='" & dr("TableName") & "' AND  FieldName='" & dr("FieldName") & "'"
            ElseIf tbl = "OURPermits" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURPermissions" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportInfo" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportSQLquery" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportGroups" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportLists" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportFormat" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportShow" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURFiles" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURUnits" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURHelpDesk" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURAccessLog" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportChildren" Then
                sqlt = sqlt & " WHERE "
            End If

            ret = ExequteSQLquery(sqlt, connstr, connprv)
            If ret = "Query executed fine." Then
                ret = ""
                rt = True
            Else
                ret = " Query: " & sqlt & " crashed: " & ret
                rt = False
            End If

        Catch ex As Exception
            rt = False
        End Try
        Return rt
    End Function
    Protected Function RecordInsertInTargetTable(ByVal tbl As String, ByVal dr As DataRow, ByVal unit As String, ByVal userconnstr As String, ByVal connstr As String, ByVal connprv As String) As Boolean
        'NOT IN USE, not finished
        'dr - datarow from old table, tbl - target table
        Dim rt As Boolean = False
        Dim ret As String = String.Empty
        Dim sqlt As String = String.Empty
        Dim conndb As String = connstr.Substring(0, connstr.IndexOf("User ID")).Trim
        Try
            If connprv = "MySql.Data.MySqlClient" Then
                Dim db As String = GetDataBase(connstr, connprv)
                sqlt = "INSERT INTO `" & db & "`.`" & tbl.ToLower & "`"
                'TODO !!!!!! for Oracle.ManagedDataAccess.Client
            Else
                sqlt = "INSERT INTO " & tbl.ToUpper
            End If
            If tbl = "OURFriendlyNames" Then
                sqlt = sqlt & " (Unit,UnitDB,Tablename,FieldName,Friendly) VALUES"
                sqlt = sqlt & "('" & unit & "','" & conndb & "','" & dr("TableName") & "','" & dr("FieldName") & "','" & dr("Friendly") & "')"
            ElseIf tbl = "OURPermits" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURPermissions" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportInfo" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportSQLquery" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportGroups" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportLists" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportFormat" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportShow" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURFiles" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURUnits" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURHelpDesk" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURAccessLog" Then
                sqlt = sqlt & " WHERE "
            ElseIf tbl = "OURReportChildren" Then
                sqlt = sqlt & " WHERE "
            End If
            ret = ExequteSQLquery(sqlt, connstr, connprv)
            If ret = "Query executed fine." Then
                ret = ""
                rt = True
            Else
                ret = " Query: " & sqlt & " crashed: " & ret
                rt = False
            End If
        Catch ex As Exception
            rt = False
        End Try
        Return rt
    End Function

    Private Sub btnUpdateOURdb_Click(sender As Object, e As EventArgs) Handles btnUpdateOURdb.Click
        Dim ret As String = String.Empty
        Dim myconstring, myprovider As String
        Try
            myconstring = TextBoxDBConn.Text.Trim   'System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = DropDownListDBProv.Text 'System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            ret = UpdateOURdbToCurrentVersion(myconstring.Trim, myprovider)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Label19.Text = ret
    End Sub
    Protected Sub btnRemoteConnect_Click(sender As Object, e As EventArgs) Handles btnRemoteConnect.Click
        Dim ret As String = String.Empty
        Try
            Dim sqlt As String = "SELECT * FROM OURActivity WHERE ActivityType='remote' AND Activity='connected' AND ConnectedServer='" & txtIP.Text & "'"
            Dim dv As DataView = mRecords(sqlt)
            If Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
                Label11.Visible = True
                Label11.Text = "Remote Connection is already open by " & dv.Table.Rows(0)("ConnectedBy") & ":"
            Else
                Label11.Visible = False
                process.StartInfo.FileName = "mstsc"
                process.StartInfo.Arguments = "/v:" + txtIP.Text
                process.Start()
                Dim sFields As String = "ConnectedBy,ActivityType,Activity,ConnectedServer,Ticket,ConnOpen"
                Dim sValues As String = "'" & Session("logon") & "',"
                sValues &= "'remote',"
                sValues &= "'connected',"
                sValues &= "'" & txtIP.Text & "',"
                sValues &= "'" & ddTickets.Text & "',"
                sValues &= "'" & DateToString(DateTime.Now) & "'"
                'add record in OURActivity table
                Dim sqlag As String = "INSERT INTO OURActivity (" & sFields & ") VALUES (" & sValues & ")"
                'Dim sqlag As String = "INSERT INTO OURActivity SET ConnectedBy='" & Session("logon") & "', ActivityType='remote',Activity='connected', ConnectedServer='" & txtIP.Text & "', Ticket='" & ddTickets.Text & "', ConnOpen='" & DateToString(Now()) & "'"
                ret = ExequteSQLquery(sqlag)
                Response.Redirect("InstallIt.aspx")
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
    End Sub
    Protected Sub btnRemoteDisconnect_Click(sender As Object, e As EventArgs) Handles btnRemoteDisconnect.Click
        Dim ret As String = String.Empty
        Try
            Dim sqlag As String = "UPDATE OURActivity SET Activity='disconnected', ConnClosed='" & DateToString(Now()) & "'  WHERE ConnectedBy='" & Session("logon") & "' AND ActivityType='remote' AND Activity='connected' AND ConnectedServer='" & txtIP.Text & "'"
            ret = ExequteSQLquery(sqlag)
            Label11.Visible = False
            'process.StartInfo.FileName = "mstsc"
            'process.StartInfo.Arguments = "/v:" + txtIP.Text
            'process.Kill()
            'process.Close()
            Response.Redirect("InstallIt.aspx")
        Catch ex As Exception
            ret = ex.Message
        End Try
    End Sub

    Private Sub GridViewConnections_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridViewConnections.PageIndexChanging
        GridViewConnections.PageIndex = e.NewPageIndex
        GridViewConnections.DataBind()
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
    Protected Sub ddUnit1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddUnit1.SelectedIndexChanged
        Dim er As String = String.Empty
        Dim mSql As String = String.Empty
        mSql = "SELECT * FROM OURUnits WHERE Unit='" & ddUnit1.SelectedValue & "'"
        Dim dv As DataView = mRecords(mSql, er)  'Data for report by SQL statement from the OURdb database
        If er = "" AndAlso Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
            ddOURConnPrv1.SelectedValue = dv.Table.Rows(0)("OURConnPrv")
            ddUserConnPrv1.SelectedValue = dv.Table.Rows(0)("UserConnPrv")
            txtOURdb1.Text = dv.Table.Rows(0)("OURConnStr")
            txtUserdb1.Text = dv.Table.Rows(0)("UserConnStr")
        End If
    End Sub
    Protected Sub ddUnit2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddUnit2.SelectedIndexChanged
        Dim er As String = String.Empty
        Dim mSql As String = String.Empty
        mSql = "SELECT * FROM OURUnits WHERE Unit='" & ddUnit2.SelectedValue & "'"
        Dim dv As DataView = mRecords(mSql, er)  'Data for report by SQL statement from the OURdb database
        If er = "" AndAlso Not dv Is Nothing AndAlso dv.Count > 0 AndAlso dv.Table.Rows.Count > 0 Then
            ddOURConnPrv2.SelectedValue = dv.Table.Rows(0)("OURConnPrv")
            ddUserConnPrv2.SelectedValue = dv.Table.Rows(0)("UserConnPrv")
            txtOURdb2.Text = dv.Table.Rows(0)("OURConnStr")
            txtUserdb2.Text = dv.Table.Rows(0)("UserConnStr")
            Session("Unit2EndDate") = dv.Table.Rows(0)("EndDate")
        End If
    End Sub
    Function CreateForeinKeys(ByVal rep As String, ByVal userconnstr As String, ByVal userconnprv As String) As String
        Dim ret As String = String.Empty
        Dim sqlq As String = String.Empty
        'make dset from joins
        Dim ds As New DataView
        Dim njoins As Integer = 0
        sqlq = "SELECT * FROM OURReportSQLquery WHERE (ReportId LIKE '" & rep & "' AND Doing = 'JOIN') "
        ds = mRecords(sqlq, ret)
        If Not ds Is Nothing AndAlso ds.Count > 0 Then
            njoins = ds.Table.Rows.Count
            If njoins = 0 Then
                Return ret
            End If
            'Make forein keys from ds  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            For i = 0 To njoins - 1
                ret = CreateForeinKeyRelationshipForTables(rep, ds.Table.Rows(i)("Tbl1"), ds.Table.Rows(i)("Tbl1Fld1"), ds.Table.Rows(i)("Tbl2"), ds.Table.Rows(i)("Tbl2Fld2"), userconnstr, userconnprv)
            Next
        Else
            Return ret
        End If
        Return ret
    End Function

    Private Sub btnForeinKeys_Click(sender As Object, e As EventArgs) Handles btnForeinKeys.Click
        Dim ret As String = String.Empty
        ret = CreateForeinKeys(TextBoxRep.Text, TextBoxUserConnStr.Text, DropDownListUserDB3.SelectedValue)
    End Sub

    Private Sub ButtonClone_Click(sender As Object, e As EventArgs) Handles ButtonClone.Click
        Dim i As Integer
        Dim ret As String = String.Empty
        Label25.Text = ""
        Dim nm As String = cleanText(TextBoxSiteAbr.Text.Trim)
        If nm = "" Then
            Label25.Text = "Illegal character found. Please choose another abbreviation."
            Exit Sub
        End If

        'directory exist?
        Dim newFolder As String = Request.PhysicalApplicationPath
        Dim n As Integer = newFolder.LastIndexOf("\")
        If n = newFolder.Length - 1 Then
            newFolder = newFolder.Substring(0, n)
            n = newFolder.LastIndexOf("\")
        End If
        Dim sourceFolder As String = newFolder.Substring(0, n) & "\" & TextBoxSourceFolder.Text.Trim
        newFolder = newFolder.Substring(0, n) & "\OUReports" & nm
        If Directory.Exists(newFolder) Then
            Label25.Text = "Site OUReports" & nm & " already exists. Please choose another abbreviation."
            Exit Sub
        End If

        'abbreviation exist?
        Dim hasrecord As Boolean = HasRecords("SELECT FROM OURUnits WHERE Unit='" & nm & "'")  ' AND OURConnStr='" & txtOURdb.Text.Trim & "' ")
        If hasrecord = True Then
            Label25.Text = "Site OUReports" & nm & " already exists. Please choose another abbreviation."
            Exit Sub
        End If

        'ourdb
        Dim ourdb As String = String.Empty
        Dim ourdbstr As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString
        Dim ourdbpr As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString

        If ourdbpr <> "MySql.Data.MySqlClient" Then
            Label25.Text = "Clone cannot be created if the original oureports database is not MySql."
            Exit Sub
        End If

        Dim dbname As String = GetDataBase(ourdbstr, ourdbpr)

        i = ourdbstr.ToUpper.IndexOf(" DATABASE")
        Dim part1 As String = ourdbstr.Substring(0, i)
        Dim part2 As String = ourdbstr.Substring(i)
        part2 = part2.Substring(part2.IndexOf(";"))
        ourdb = part1 & " Database=" & dbname & nm
        If DatabaseExist(dbname & nm) Then
            Label25.Text = "Database " & dbname & nm & " already exists. Please choose another abbreviation."
            Exit Sub
        End If
        'abbreviation exist?
        hasrecord = HasRecords("SELECT FROM OURUnits WHERE OURConnStr LIKE '" & ourdb & "%' ")
        If hasrecord = True Then
            Label25.Text = "Database " & dbname & nm & " already exists. Please choose another abbreviation."
            Exit Sub
        End If
        ourdb = ourdb & " " & part2
        Dim ourdbstrnew As String = ourdb
        If ourdb.ToUpper.IndexOf("USER ID") > 0 Then ourdb = ourdb.Substring(0, ourdb.ToUpper.IndexOf("USER ID")).Trim
        If ourdb.ToUpper.IndexOf("PASSWORD") > 0 Then ourdb = ourdb.Substring(0, ourdb.ToUpper.IndexOf("PASSWORD")).Trim

        'Make dbs
        'Set Up dbname & nm
        ret = UpdateOURdbToCurrentVersion(ourdbstrnew, ourdbpr)
        WriteToAccessLog(Session("logon").ToString, "Database " & dbname & nm & " has been created:" & ourdbstrnew & " with result" & ret, 1)

        Dim er As String = String.Empty
        'csvdb
        Dim csvdbnew As String = String.Empty
        Dim csvpr As String = String.Empty
        If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing Then
            Dim csvdb As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
            csvpr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
            Dim csvdbname As String = GetDataBase(csvdb, csvpr)
            i = csvdb.ToUpper.IndexOf(" DATABASE")
            part1 = csvdb.Substring(0, i)
            part2 = csvdb.Substring(i)
            part2 = part2.Substring(part2.IndexOf(";"))
            csvdbnew = part1 & " Database=" & csvdbname & nm & " " & part2

            If DatabaseExist(csvdbname & nm, csvdb, csvpr, er) Then
                Label25.Text = "Database " & csvdb & nm & " already exists. Please choose another abbreviation."
                Exit Sub
            End If
            'abbreviation exist?
            csvdb = part1 & " Database=" & csvdbname & nm
            hasrecord = HasRecords("SELECT FROM OURUnits WHERE OURConnStr LIKE '" & csvdb & "%' ")
            If hasrecord = True Then
                Label25.Text = "Database " & csvdbname & " already exists. Please choose another abbreviation."
                Exit Sub
            End If

            ''Set Up csvdbname & nm
            'ret = UpdateOURdbToCurrentVersion(csvdbnew, csvpr)
            ret = ExequteSQLquery("CREATE DATABASE `" & csvdbnew & "`")
            WriteToAccessLog(Session("logon").ToString, "Database " & csvdbname & nm & " has been created:" & csvdbnew & " with result" & ret, 1)


        End If

        'Make folder
        ret = DirectoryCopy(sourceFolder, newFolder, True)

        'correct web.config
        Dim oldsitefor As String = ConfigurationManager.AppSettings("SiteFor").ToString.Trim
        If ret = "" Then 'created
            'update web.config in newFolder:  web.config should have the connection string to OURdataUnit" & unitindx & " OUR database
            Dim webconfigpath As String = newFolder & "\web.config"
            Dim sr() As String = File.ReadAllLines(webconfigpath)
            For i = 0 To sr.Length - 1
                If sr(i).Contains("MySqlConnection") Then
                    WriteToAccessLog("Clone ourdb registration", "ourdbstr:" & ourdbstrnew, 1)
                    sr(i) = "      <add name=""MySqlConnection"" connectionString=""" & ourdbstrnew & """ providerName=""" & ourdbpr & """ />"
                End If
                If sr(i).Contains("CSVconnection") Then
                    WriteToAccessLog("Clone csvdb registration", "csvdbstr:" & csvdbnew, 1)
                    sr(i) = "      <add name=""CSVconnection"" connectionString=""" & csvdbnew & """ providerName=""" & csvpr & """ />"
                End If

                If sr(i).Contains("<add key=""unit"" value=") Then
                    sr(i) = "   <add key=""unit"" value=""" & nm & """/>"
                End If
                sr(i) = sr(i).Replace("/" & TextBoxSourceFolder.Text.Trim & "/", "/" & "OUReports" & nm & "/")
                'sr(i) = sr(i).Replace(oldsitefor, TextBoxSiteFor.Text.Trim)
                If sr(i).Contains("<add key=""SiteFor"" value=""") Then
                    sr(i) = "   <add key=""SiteFor"" value=""" & TextBoxSiteFor.Text.Trim & """/>"
                End If
                If sr(i).Contains("<add key=""pagettl"" value=""") Then
                    sr(i) = "   <add key=""pagettl"" value=""" & "Online User Reporting - " & TextBoxSiteFor.Text.Trim & """/>"
                End If
            Next
            'write in file
            File.WriteAllLines(webconfigpath, sr)

            'insert record into ourunits
            Dim sFields As String = String.Empty
            Dim sValues As String = String.Empty
            Dim oldunit As String = ConfigurationManager.AppSettings("unit").ToString.Trim
            Dim oldweb As String = ConfigurationManager.AppSettings("weboureports").ToString '.Replace(TextBoxSourceFolder.Text.Trim, "OUReports" & nm)
            Dim cloneweb As String = oldweb
            n = oldweb.LastIndexOf("/")
            If n = oldweb.Length - 1 Then
                cloneweb = oldweb.Substring(0, n)
                n = cloneweb.LastIndexOf("/")
            End If
            cloneweb = cloneweb.Substring(0, n) & "/OUReports" & nm

            Dim commnts As String = "On " & Now.ToString & " clonned by " & Session("logon").ToString
            Dim date1 As String = DateToString(CDate(Now))
            Dim date2 As String = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 4000, Now())))
            Dim sqlt As String = String.Empty
            Dim oldemail As String = ConfigurationManager.AppSettings("supportemail").ToString.Trim
            If txtCloneEmail.Text.Trim = "" Then
                txtCloneEmail.Text = oldemail
            End If

            'insert unit into ourunits in target database
            sFields = "Unit,DistrMode,UnitWeb,Comments,StartDate,EndDate,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Email"
            sValues = "'" & nm & "',"
            sValues &= "'" & "clone of" & oldunit & " ',"
            sValues &= "'" & cloneweb & "',"
            sValues &= "'" & commnts & "',"
            sValues &= "'" & date1 & "',"
            sValues &= "'" & date2 & "',"
            sValues &= "'" & ourdbstrnew & "',"
            sValues &= "'" & ourdbpr & "',"
            sValues &= "'" & ourdbstrnew & "',"
            sValues &= "'" & ourdbpr & "',"
            sValues &= "'" & txtCloneEmail.Text & "'"

            sqlt = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"
            er = ExequteSQLquery(sqlt, ourdbstrnew, ourdbpr)  'unit registered in target ourunits

            'insert unit into ourunits in source database
            commnts = commnts & ", clone support email: " & txtCloneEmail.Text
            sFields = "Unit,DistrMode,UnitWeb,Comments,StartDate,EndDate,OURConnStr,OURConnPrv,Email"
            sValues = "'" & nm & "',"
            sValues &= "'" & "clone of " & oldunit & " ',"
            sValues &= "'" & cloneweb & "',"
            sValues &= "'" & commnts & "',"
            sValues &= "'" & date1 & "',"
            sValues &= "'" & date2 & "',"
            sValues &= "'" & ourdbstrnew & "',"
            sValues &= "'" & ourdbpr & "',"
            sValues &= "'" & oldemail & "'"

            sqlt = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"
            er = ExequteSQLquery(sqlt)  'unit registered in source ourunits

            If csvdbnew <> "" Then
                'insert CSV unit into ourunits in target database
                sFields = "Unit,DistrMode,UnitWeb,Comments,StartDate,EndDate,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Prop3,Email"
                sValues = "'CSV',"
                sValues &= "'" & "clone CSV of" & oldunit & " ',"
                sValues &= "'" & cloneweb & "',"
                sValues &= "'" & commnts & "',"
                sValues &= "'" & date1 & "',"
                sValues &= "'" & date2 & "',"
                sValues &= "'" & ourdbstrnew & "',"
                sValues &= "'" & ourdbpr & "',"
                sValues &= "'" & csvdbnew & "',"
                sValues &= "'" & csvpr & "',"
                sValues &= "'CSV',"
                sValues &= "'" & txtCloneEmail.Text & "'"

                sqlt = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"
                er = ExequteSQLquery(sqlt, ourdbstrnew, ourdbpr)  'unit registered in target ourunits
            End If
            Dim EmailTable As DataTable
            Dim j As Integer
            EmailTable = mRecords("SELECT  * FROM OURPERMITS WHERE APPLICATION='InteractiveReporting' AND RoleApp='super'").Table
            If Not ret.Contains("ERROR!!") Then
                'send emails     
                If EmailTable.Rows.Count > 0 Then
                    For j = 0 To EmailTable.Rows.Count - 1
                        ret = SendHTMLEmail("", nm & " - clone of " & oldunit & " has been created and registered", "Clone web site " & cloneweb & " should be created ASAP! The Folder " & newFolder & "  should be already created. If not - copy the folder " & sourceFolder & " from wwwroot to wwwroot, rename it to " & newFolder & ". In IIS right click on the folder and convert in to application. Or in IIS right click on Default Web Site and click Add Application. Fill out the form as this: Alias: OUReports" & nm & ", Physical path: browse and find the folder" & newFolder & ". Click OK. After that check that web.config had been updated: web.config should have the MySqlConnection string as " & ourdbstrnew & " and CSVconnection as " & csvdbnew, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                    Next
                End If
            Else
                'create urgent ticket

            End If

            Label25.Text = "Clone Web will be available at " & cloneweb & " in 24 hours. Please contact us if it does not happen."
        Else
            Label25.Text = ret
        End If

    End Sub

    Private Sub ButtonDeleteReports_Click(sender As Object, e As EventArgs) Handles ButtonDeleteReports.Click
        Dim ret As String = String.Empty
        Dim rep As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dbconn As String = TextBoxOurdbConnStr.Text
        Dim prconn As String = DropDownListOurdb.SelectedValue
        Dim userdbconn As String = TextBoxUserConnStr.Text
        Dim userprconn As String = DropDownListUserDB3.SelectedValue
        Dim userdb As String = userdbconn
        If userprconn <> "Oracle.ManagedDataAccess.Client" Then
            If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
            If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
        Else
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
        End If

        Dim ds As New DataView
        Dim n As Integer = 0
        sqlq = "SELECT * FROM OURReportInfo WHERE (ReportId LIKE '" & TextBoxRep.Text & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%') "
        ds = mRecords(sqlq, ret, dbconn, prconn)
        If Not ds Is Nothing AndAlso ds.Count > 0 Then
            n = ds.Table.Rows.Count
            'delete reports  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            For i = 0 To n - 1
                rep = ds.Table.Rows(i)("ReportId").ToString
                ret = ret & " | Report " & rep & " deleted: " & DeleteReport(rep, dbconn, prconn)
            Next
            ret = "Deleted " & n.ToString & " reports: " & ret
        End If
    End Sub

    Private Sub ButtonDeleteReportsTtl_Click(sender As Object, e As EventArgs) Handles ButtonDeleteReportsTtl.Click
        Dim ret As String = String.Empty
        Dim rep As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dbconn As String = TextBoxOurdbConnStr.Text
        Dim prconn As String = DropDownListOurdb.SelectedValue
        Dim userdbconn As String = TextBoxUserConnStr.Text
        Dim userprconn As String = DropDownListUserDB3.SelectedValue
        Dim userdb As String = userdbconn

        If userprconn <> "Oracle.ManagedDataAccess.Client" Then
            If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
            If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
        Else
            'userdb = Piece(userdb, ";", 1, 2) & ";"
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
        End If

        Dim ds As New DataView
        Dim n As Integer = 0
        sqlq = "SELECT * FROM OURReportInfo WHERE (ReportTtl LIKE '" & TextBoxRep.Text & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%') "
        ds = mRecords(sqlq, ret, dbconn, prconn)
        If Not ds Is Nothing AndAlso ds.Count > 0 Then
            n = ds.Table.Rows.Count
            'delete reports  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            For i = 0 To n - 1
                rep = ds.Table.Rows(i)("ReportId").ToString
                ret = ret & " | Report " & rep & " deleted: " & DeleteReport(rep, dbconn, prconn)
            Next
            ret = "Deleted " & n.ToString & " reports: " & ret
        End If
    End Sub

    Private Sub ButtonCleanOURDB_Click(sender As Object, e As EventArgs) Handles ButtonCleanOURDB.Click
        Dim ret As String = String.Empty
        Dim tb As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dbconn As String = TextBoxOurdbConnStr.Text
        Dim prconn As String = DropDownListOurdb.SelectedValue
        Dim userdbconn As String = TextBoxUserConnStr.Text
        Dim userprconn As String = DropDownListUserDB3.SelectedValue
        Dim userdb As String = userdbconn
        If userprconn <> "Oracle.ManagedDataAccess.Client" Then
            If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
            If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
        Else
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
        End If

        Dim ds As New DataView
        Dim n As Integer = 0
        Dim m As Integer = 0
        'delete not used ourusertables
        'sqlq = "SELECT TableName FROM ourusertables WHERE (UserDB LIKE '%" & userdb & "%') "
        'sqlq = "SELECT DISTINCT TableName,Tbl1 FROM ourusertables RIGHT JOIN OURReportSQLquery ON ourusertables.TableName=OURReportSQLquery.Tbl1 WHERE (ourusertables.UserDB Like '%" & userdb & "%'  AND OURReportSQLquery.Doing = 'SELECT') "
        sqlq = "SELECT TableName FROM ourusertables WHERE (UserDB Like '%" & userdb & "%')"
        ds = mRecords(sqlq, ret, dbconn, prconn)
        If Not ds Is Nothing AndAlso ds.Count > 0 Then
            n = ds.Table.Rows.Count
            'delete from ourusertables  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            For i = 0 To n - 1
                tb = ds.Table.Rows(i)("TableName").ToString
                sqlq = "SELECT * FROM OURReportSQLquery WHERE (Tbl1 = '" & tb & "' AND Doing = 'SELECT') "
                If Not HasRecords(sqlq, dbconn, prconn) Then
                    'If ds.Table.Rows(i)("Tbl1") Is Nothing OrElse ds.Table.Rows(i)("Tbl1").ToString = "" Then
                    sqlq = "DELETE FROM ourusertables WHERE (TableName='" & tb & "' AND UserDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%') "
                    ret = ret & " | Table " & tb & " deleted: " & ExequteSQLquery(sqlq, dbconn, prconn)  'commented for testing!!!!!!!!!!!!!!!!!!!!!!!
                    m = m + 1
                Else
                    ret = ret & "Table " & tb & " stay,"
                End If
            Next
            ret = "Deleted " & m.ToString & " rows from " & n.ToString & " rows in ourusertables... " & ret
        End If
        'delete reports without tables
        m = 0
        Dim rep As String = String.Empty
        sqlq = "SELECT OURReportInfo.ReportId,Tbl1 FROM OURReportInfo LEFT JOIN OURReportSQLquery ON OURReportInfo.ReportId=OURReportSQLquery.ReportId WHERE (OURReportInfo.ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'  AND OURReportSQLquery.Doing = 'SELECT') "
        ds = mRecords(sqlq, ret, dbconn, prconn)
        If Not ds Is Nothing AndAlso ds.Count > 0 Then
            n = ds.Table.Rows.Count
            'delete reports  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            For i = 0 To n - 1
                rep = ds.Table.Rows(i)("ReportId").ToString
                If ds.Table.Rows(i)("Tbl1") Is Nothing OrElse ds.Table.Rows(i)("Tbl1").ToString = "" Then
                    'delete reports with no tables
                    ' ret =ret & " | Report " & rep & " deleted: " &  DeleteReport(rep, dbconn, prconn)
                    m = m + 1
                Else
                    ret = ret & "Report " & rep & " stay,"
                End If
            Next
            ret = "Deleted " & m.ToString & " reports from " & n.ToString & " reports... " & ret
        End If

    End Sub

    Private Sub ButtonDeleteTables_Click(sender As Object, e As EventArgs) Handles ButtonDeleteTables.Click
        Dim ret As String = String.Empty
        Dim tb As String = String.Empty
        Dim sqlq As String = String.Empty
        Dim dbconn As String = TextBoxOurdbConnStr.Text
        Dim prconn As String = DropDownListOurdb.SelectedValue
        Dim userdbconn As String = TextBoxUserConnStr.Text
        Dim userprconn As String = DropDownListUserDB3.SelectedValue
        Dim db As String = String.Empty
        If userprconn = "MySql.Data.MySqlClient" Then
            db = GetDataBase(userdbconn, userprconn).ToLower
        End If
        Dim userdb As String = userdbconn
        'If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
        'If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
        If userprconn <> "Oracle.ManagedDataAccess.Client" Then
            If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
            If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
        Else
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
        End If

        Dim ds As New DataView
        Dim n As Integer = 0
        'ds = GetListOfUserTables(False, dbconn, prconn, ret, TextBoxLogon.Text.Trim)
        'ds.RowFilter = "TABLE_NAME LIKE '" & TextBoxTableName.Text.Trim & "'"
        sqlq = "SELECT TableName FROM ourusertables WHERE (UserID = '" & TextBoxLogon.Text.Trim & "' AND UserDB Like '%" & userdb & "%' AND TableName Like '" & TextBoxTableName.Text.Trim & "') "
        ds = mRecords(sqlq, ret, dbconn, prconn)
        If Not ds Is Nothing AndAlso ds.Count > 0 Then
            n = ds.Table.Rows.Count
            'delete tables  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            For i = 0 To n - 1
                tb = ds.Table.Rows(i)("TableName").ToString
                'delete table
                If userprconn = "MySql.Data.MySqlClient" Then
                    sqlq = "DROP TABLE  `" & db & "`.`" & tb & "`"
                Else
                    sqlq = "DROP TABLE " & tb
                End If
                ret = ExequteSQLquery(sqlq, userdbconn, userprconn)  'commented for testing!!!!!!!!!!!!!!!!!!!!!!!
            Next
        End If

    End Sub

    Private Sub DropDownListDBProv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListDBProv.SelectedIndexChanged

    End Sub

    Private Sub btnUpdateToVersion_Click(sender As Object, e As EventArgs) Handles btnUpdateToVersion.Click
        Dim ret As String = String.Empty
        Dim myconstring, myprovider As String
        Try
            myconstring = TextBoxDBConn.Text.Trim   'System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = DropDownListDBProv.Text 'System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            If TextBoxFromVersion.Text.Trim = "" OrElse TextBoxToVersion.Text.Trim = "" Then
                Label19.Text = "Versions should not be empty"
                Exit Sub
            End If
            ret = UpdateOURdbToVersion(TextBoxFromVersion.Text.Trim, TextBoxToVersion.Text.Trim, myconstring.Trim, myprovider)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Label19.Text = ret
    End Sub

    Private Sub btnCreateOURdbOnNewserver_Click(sender As Object, e As EventArgs) Handles btnCreateOURdbOnNewserver.Click
        Dim ret As String = String.Empty
        Dim myconstring, myprovider As String
        Try
            myconstring = TextBoxDBConn.Text.Trim   'System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            myprovider = DropDownListDBProv.Text 'System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            ret = CreateNewOURdbOnNewServer(myconstring.Trim, myprovider)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Label19.Text = ret
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


    'Private Sub ButtonCleanUserdb_Click(sender As Object, e As EventArgs) Handles ButtonCleanUserdb.Click
    '    'delete tables without reports



    'End Sub
End Class
