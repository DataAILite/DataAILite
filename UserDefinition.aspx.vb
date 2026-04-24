Imports System.Data
Imports System.Data.SqlClient
Partial Class UserDefinition
    Inherits System.Web.UI.Page
    Private createsuper As String
    Private Sub UserDefinition_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        'Dim userid As String = Request("uid")
        Dim i, j As Integer
        Dim err As String = String.Empty
        Dim mSql As String = String.Empty
        Dim selprov As String = String.Empty
        Dim indx As String = ""
        If (Session("Unit") = "CSV" OrElse Session("Unit") = "OUR") AndAlso Not (Session("admin") = "super" OrElse Session("Access") = "SITEADMIN") Then
            btnSave.Visible = False
            btnSave.Enabled = False
            btnDeleteUser.Visible = False
            btnDeleteUser.Enabled = False
            btnDelete.Visible = False
            btnDelete.Enabled = False
        End If
        If Not Request("crsuper") Is Nothing Then
            createsuper = Request("crsuper").ToString.Trim
            btnSave.Visible = True
            btnSave.Enabled = True
            btnDeleteUser.Visible = True
            btnDeleteUser.Enabled = True
            btnDelete.Visible = True
            btnDelete.Enabled = True

        End If
        If Not Request("indx") Is Nothing Then
            indx = Request("indx").ToString
        Else
            'new user
            indx = ""
            btnDeleteUser.Enabled = False
            btnDeleteUser.Visible = False
            btnDelete.Visible = False
            btnDelete.Enabled = False

            txtLogon.Enabled = True
        End If
        Session("UserIndx") = indx
        If Session("admin") = "super" AndAlso createsuper = "yes" Then
            mSql = "SELECT * FROM OURUnits WHERE (Unit='" & Session("Unit") & "')"
        Else
            mSql = "SELECT * FROM OURUnits WHERE (Unit='" & Session("Unit") & "') AND (UserConnStr LIKE '%" & Session("UnitDB").Trim.Replace(" ", "%") & "%') "
        End If
        Dim grps() As String
        Dim dtu As DataTable = mRecords(mSql, err).Table
        Dim ourconnstr As String = String.Empty
        Dim ourconnprv As String = String.Empty
        Dim userconnprv As String = String.Empty
        Dim strtdate As String = String.Empty
        Dim enddate As String = String.Empty
        If Not dtu Is Nothing AndAlso dtu.Rows.Count > 0 Then
            ourconnstr = dtu.Rows(0)("OURConnStr").ToString.Trim
            ourconnprv = dtu.Rows(0)("OURConnPrv").ToString.Trim
            txtConnStr.Text = Session("UnitDB") 'dtu.Rows(0)("UserConnStr").ToString.Trim
            userconnprv = dtu.Rows(0)("UserConnPrv").ToString.Trim
            'strtdate = DateToString(Now.ToString.Trim)
            enddate = DateToString(dtu.Rows(0)("EndDate")).Trim
            Session("UnitEndDate") = enddate
            If dtu IsNot Nothing AndAlso dtu.Rows.Count > 0 Then
                Session("Groups") = dtu.Rows(0)("Prop3").ToString.Trim
            End If
            If Session("Groups") = "" Then
                Session("Groups") = "All,"
            End If
            grps = Session("Groups").Split(",")
            DropDownGroups.Items.Clear()
            For i = 0 To grps.Length - 1
                If grps(i).Trim <> "" Then DropDownGroups.Items.Add(grps(i))
            Next
        Else
            If Session("admin") = "super" AndAlso createsuper = "yes" Then
                enddate = "2100-12-31 23:59:00"
            End If
        End If
        strtdate = DateToString(Now.ToString.Trim)
        If indx = "" Then
            'new user
            txtLogon.Enabled = True
            txtLogon.ReadOnly = False
            txtUnit.Text = Session("Unit")
            'For i = 0 To ddConnPrv.Items.Count - 1
            '    If ddConnPrv.Items(i).Value = userconnprv.Trim Then
            '        ddConnPrv.Items(i).Selected = True
            '    End If
            'Next
            txtComments.Text = "added by " & Session("logon").ToString
            txtStartDate.Text = strtdate
            txtEndDate.Text = enddate
        Else
            'existing user
            txtLogon.Enabled = False
            txtLogon.ReadOnly = True
            If Session("admin") = "super" Then
                mSql = "SELECT * FROM OURPermits WHERE Indx='" & indx & "'"
            Else
                mSql = "SELECT * FROM OURPermits WHERE RoleApp<>'super' AND Indx='" & indx & "'"
            End If

            Dim dt As DataTable = mRecords(mSql, err).Table  'Data for report by SQL statement from the OURdb database
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Response.Redirect("Default.aspx?msg=User is not found.")
                Exit Sub
            Else
                txtLogon.Text = dt.Rows(0)("NetId").ToString.Trim
                txtName.Text = dt.Rows(0)("Name").ToString.Trim
                txtUnit.Text = dt.Rows(0)("Unit").ToString.Trim
                txtEmail.Text = dt.Rows(0)("Email").ToString.Trim
                txtConnStr.Text = dt.Rows(0)("ConnStr").ToString.Trim
                'txtConnStr.Text = Session("UnitDB") 'dt.Rows(0)("ConnStr").ToString.Trim
                txtComments.Text = dt.Rows(0)("Comments").ToString.Trim
                txtStartDate.Text = dt.Rows(0)("StartDate").ToString.Trim
                txtEndDate.Text = dt.Rows(0)("EndDate").ToString.Trim
                If dt.Rows(0)("PERMIT").ToString.ToUpper = "friendly".ToUpper Then
                    chkFriendlyNames.Checked = True
                Else
                    chkFriendlyNames.Checked = False
                End If
                For i = 0 To ddRoles.Items.Count - 1
                    If ddRoles.Items(i).Value = dt.Rows(0)("ACCESS").ToString Then
                        ddRoles.Items(i).Selected = True
                    End If
                Next
                For i = 0 To ddRights.Items.Count - 1
                    If ddRights.Items(i).Value = dt.Rows(0)("RoleApp").ToString Then
                        ddRights.Items(i).Selected = True
                    End If
                Next
                For i = 0 To ddConnPrv.Items.Count - 1
                    If ddConnPrv.Items(i).Value = dt.Rows(0)("ConnPrv").ToString.Trim Then
                        ddConnPrv.Items(i).Selected = True
                        selprov = dt.Rows(0)("ConnPrv").ToString.Trim
                    End If
                Next
                If dt.Rows(0)("Group3").ToString.Trim <> "" Then
                    grps = dt.Rows(0)("Group3").ToString.Trim.Split(",")
                    For i = 0 To grps.Length - 1
                        For j = 0 To DropDownGroups.Items.Count - 1
                            If DropDownGroups.Items(j).Value = grps(i) Then
                                DropDownGroups.Items(j).Selected = True
                            End If
                        Next
                    Next
                End If
            End If
        End If

        If txtEndDate.Text.Trim = "" Then
            txtEndDate.Text = Session("UnitEndDate")
        End If

        'Provider dropdown
        For i = 0 To ddConnPrv.Items.Count - 1
            If ddConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SqlServerProv").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "SQL"

            End If

            If ddConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "CACHE"

            End If

            If ddConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "IRIS"

            End If

            If ddConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "MYSQL"

            End If

            If ddConnPrv.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "Npgsql"

            End If

            If ddConnPrv.Items(i).Text.ToUpper = "USE OUR DATABASE" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "USE OUR DATABASE" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "CSV"
                Session("CSV") = "yes"
            End If

            If ddConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "Oracle"
                Session("Oracle") = "yes"
            End If

            If ddConnPrv.Items(i).Text.ToUpper = "ODBC" AndAlso ConfigurationManager.AppSettings("ODBC").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "ODBC" AndAlso ConfigurationManager.AppSettings("ODBC").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "ODBC"
                Session("ODBC") = "yes"
            End If

            If ddConnPrv.Items(i).Text.ToUpper = "OLEDB" AndAlso ConfigurationManager.AppSettings("OleDb").ToString <> "OK" Then
                ddConnPrv.Items(i).Enabled = False
                ddConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "OleDb" AndAlso ConfigurationManager.AppSettings("OleDb").ToString = "OK" Then
                ddConnPrv.Text = ddConnPrv.Items(i).Text
                ddConnPrv.Items(i).Selected = True
                selprov = "OleDb"
                Session("OleDb") = "yes"
                'btnBrowse.Visible = True
                'btnBrowse.Enabled = True
            End If
        Next
        ''Dim i As Integer = 0
        'Dim selprov As String = String.Empty
        'For i = 0 To ddConnPrv.Items.Count - 1
        '    If ddConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
        '        ddConnPrv.Items(i).Enabled = False
        '        ddConnPrv.Items(i).Selected = False
        '        'ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SqlServerProv").ToString = "OK" Then
        '        '    ddConnPrv.Text = ddConnPrv.Items(i).Text
        '        '    ddConnPrv.Items(i).Selected = True
        '        '    selprov = "SQL"
        '    End If
        '    If ddConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
        '        ddConnPrv.Items(i).Enabled = False
        '        ddConnPrv.Items(i).Selected = False
        '        'ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
        '        '    ddConnPrv.Text = ddConnPrv.Items(i).Text
        '        '    ddConnPrv.Items(i).Selected = True
        '        '    selprov = "CACHE"
        '    End If
        '    If ddConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString <> "OK" Then
        '        ddConnPrv.Items(i).Enabled = False
        '        ddConnPrv.Items(i).Selected = False

        '    End If
        '    If ddConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
        '        ddConnPrv.Items(i).Enabled = False
        '        ddConnPrv.Items(i).Selected = False
        '        'ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
        '        '    ddConnPrv.Text = ddConnPrv.Items(i).Text
        '        '    ddConnPrv.Items(i).Selected = True
        '        '    selprov = "MYSQL"
        '    End If
        '    If ddConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
        '        ddConnPrv.Items(i).Enabled = False
        '        ddConnPrv.Items(i).Selected = False
        '        'ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
        '        '    ddConnPrv.Text = ddConnPrv.Items(i).Text
        '        '    ddConnPrv.Items(i).Selected = True
        '        '    selprov = "ORACLE"
        '    End If
        '    If ddConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
        '        ddConnPrv.Items(i).Enabled = False
        '        ddConnPrv.Items(i).Selected = False
        '    ElseIf selprov = "" AndAlso ddConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
        '        'ddConnPrv.Text = ddConnPrv.Items(i).Text
        '        'ddConnPrv.Items(i).Selected = True
        '        'selprov = "CSV"
        '        Session("CSV") = "yes"
        '    End If
        'Next
    End Sub

    Private Sub UserDefinition_Load(sender As Object, e As EventArgs) Handles Me.Load
        If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso (Session("CSV") = "yes" OrElse ddConnPrv.Text.ToUpper = "CSVFILES") Then
            txtConnStr.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
            txtConnStr.Enabled = False
            txtConnStr.Visible = False
            If txtConnStr.Text.IndexOf("Password") > 0 Then
                txtConnStr.Text = txtConnStr.Text.Substring(0, txtConnStr.Text.IndexOf("Password")).Trim
            End If
            ddConnPrv.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
            ddConnPrv.Enabled = False
            ddConnPrv.Visible = False
            tr5.Visible = False
            tr6.Visible = False
            ddRights.SelectedValue = "admin"
            chkFriendlyNames.Checked = True
            tr4.Visible = False
            tr2.Visible = False
            txtUnit.Text = "CSV"
            trText3.Visible = False

            Label11.Text = "Your data (CSV, XML, JSON, XLS, XLSX, Access table) can be uploaded into database on our server. Reports and analytics will be available only to you and to users you share them with."
        End If
        If Session("admin") = "super" Then
            txtUnit.Enabled = True
            txtUnit.ReadOnly = False
            txtConnStr.Enabled = True
            txtConnStr.ReadOnly = False
            ddConnPrv.Enabled = True
            txtStartDate.Enabled = True
            txtStartDate.ReadOnly = False
            txtEndDate.Enabled = True
            txtEndDate.ReadOnly = False
            If createsuper = "yes" Then
                tr2.Visible = False
                'tr3.Visible = False
                tr4.Visible = False
                tr5.Visible = False
                tr6.Visible = False
            End If
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        'SAMPLE CLOSING WINDOWS FROM CODE BEHIND: - DO NOT DELETE!!!
        'Response.Write("<script language='javascript'> { window.close(); }</script>")
        Response.Redirect("SiteAdmin.aspx?unitdb=yes")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'check textboxes
        Dim ret As String = String.Empty
        Dim fr As String = String.Empty
        Dim acss As String = String.Empty
        Dim rol As String = String.Empty
        Dim prv As String = String.Empty
        Dim sFields As String = String.Empty
        Dim sValues As String = String.Empty

        Try
            If cleanText(txtName.Text.Trim) <> txtName.Text.Trim Then
                ret = "Illegal character found in Name."
                Exit Try
            End If
            If cleanText(txtUnit.Text.Trim) <> txtUnit.Text.Trim Then
                ret = "Illegal character found in Unit."
                Exit Try
            End If
            If cleanText(txtEmail.Text.Trim) <> txtEmail.Text.Trim Then
                ret = "Illegal character found in Email."
                Exit Try
            End If
            If cleanText(txtConnStr.Text.Trim) <> txtConnStr.Text.Trim Then
                ret = "Illegal character found in Connection String."
                Exit Try
            End If
            If cleanText(txtComments.Text.Trim) <> txtComments.Text.Trim Then
                ret = "Illegal character found in Comments."
                Exit Try
            End If
            'update OURPermits
            If chkFriendlyNames.Checked Then
                fr = "friendly"
            End If
            acss = ddRoles.SelectedValue
            rol = ddRights.SelectedValue
            prv = ddConnPrv.SelectedValue
            Dim grs As String = String.Empty
            Dim i As Integer
            For i = 0 To DropDownGroups.Items.Count - 1
                If DropDownGroups.Items(i).Selected Then
                    grs = grs & DropDownGroups.Items(i).Value
                    If i < DropDownGroups.Items.Count - 1 Then
                        grs = grs & ","
                    End If
                End If
            Next
            If grs.StartsWith("All,") Then
                grs = "All"
            End If
            If txtUnit.Text = "CSV" Then
                grs = "CSV"
            End If
            If txtConnStr.Text.IndexOf("Password") > 0 Then
                txtConnStr.Text = txtConnStr.Text.Substring(0, txtConnStr.Text.IndexOf("Password")).Trim
            End If
            If txtEndDate.Text.Trim = "" Then
                txtEndDate.Text = Session("UnitEndDate")
            End If
            If Session("UnitEndDate") IsNot Nothing AndAlso DateToString(CDate(txtEndDate.Text)) > DateToString(CDate(Session("UnitEndDate"))) Then
                txtEndDate.Text = DateToString(CDate(Session("UnitEndDate")))
            End If
            If Session("UserIndx") = "" Then
                'Dim hasrecord As Boolean = HasRecords("SELECT * FROM OURPermits WHERE NetId='" & txtLogon.Text & "' AND Unit='" & txtUnit.Text & "' AND ConnStr LIKE '" & txtConnStr.Text.Trim & "%'")
                Dim hasrecord As Boolean = HasRecords("SELECT * FROM OURPermits WHERE NetId='" & txtLogon.Text & "'")
                If hasrecord = False Then
                    If Session("admin") = "super" AndAlso createsuper = "yes" Then
                        sFields = "Unit,Email,Name,Comments,NetId,localpass," & FixReservedWords("Access") & ",PERMIT,RoleApp,Group3,StartDate,EndDate,Application"
                        sValues = "'" & txtUnit.Text & "',"
                        sValues &= "'" & txtEmail.Text & "',"
                        sValues &= "'" & txtName.Text & "',"
                        sValues &= "'" & txtComments.Text & "',"
                        sValues &= "'" & txtLogon.Text & "',"
                        sValues &= "'" & txtEmail.Text & "',"
                        sValues &= "'DEV',"
                        sValues &= "'" & fr & "',"
                        sValues &= "'super',"
                        sValues &= "'" & grs & "',"
                        sValues &= "'" & txtStartDate.Text & "',"
                        sValues &= "'" & txtEndDate.Text & "',"
                        sValues &= "'InteractiveReporting'"

                        Dim sqlt As String = "INSERT INTO OURPermits (" & sFields & ") VALUES (" & sValues & ")"

                        'Dim sqlt As String = "INSERT INTO OURPermits Set Unit='" & txtUnit.Text & "', Email='" & txtEmail.Text & "', Name='" & txtName.Text & "' , Comments='" & txtComments.Text & "',  "
                        'sqlt = sqlt & " ConnStr='',  ConnPrv='',  NetId='" & txtLogon.Text & "',  localpass='" & txtEmail.Text & "', "
                        'sqlt = sqlt & " ACCESS='DEV', PERMIT='" & fr & "', RoleApp='super', StartDate='" & txtStartDate.Text & "', EndDate='" & txtEndDate.Text & "', Application='InteractiveReporting'"
                        'ret = "Super User created " & ExequteSQLquery(sqlt)
                        ret = RegisterUser("DEV", fr, txtLogon.Text.Trim, txtEmail.Text.Trim, txtName.Text, txtUnit.Text.Trim, "InteractiveReporting", "super", "", "", grs, txtEmail.Text, "user definition-" & txtComments.Text, "", "", DateToString(txtStartDate.Text), DateToString(txtEndDate.Text))

                        WriteToAccessLog(Session("logon"), "Super User created with result: " & ret, 11)
                    Else
                        sFields = "Unit,Email,Name,Comments,ConnStr,ConnPrv,NetId,localpass," & FixReservedWords("Access") & ",PERMIT,RoleApp,Group3,StartDate,EndDate,Application"
                        'If txtUnit.Text.Trim = "CSV" Then
                        '    sFields &= ",Group3"
                        'End If
                        sValues = "'" & txtUnit.Text & "',"
                        sValues &= "'" & txtEmail.Text & "',"
                        sValues &= "'" & txtName.Text & "',"
                        sValues &= "'" & txtComments.Text & "',"
                        sValues &= "'" & txtConnStr.Text & "',"
                        sValues &= "'" & prv & "',"
                        sValues &= "'" & txtLogon.Text & "',"
                        sValues &= "'" & txtEmail.Text & "',"
                        sValues &= "'" & acss & "',"
                        sValues &= "'" & fr & "',"
                        sValues &= "'" & rol & "',"
                        sValues &= "'" & grs & "',"
                        sValues &= "'" & txtStartDate.Text & "',"
                        sValues &= "'" & txtEndDate.Text & "',"
                        sValues &= "'InteractiveReporting'"
                        'If txtUnit.Text.Trim = "CSV" Then
                        '    sValues &= ",'CSV'"
                        'End If
                        Dim sqlt As String = "INSERT INTO OURPermits (" & sFields & ") VALUES (" & sValues & ")"

                        'Dim sqlt As String = "INSERT INTO OURPermits SET Unit='" & txtUnit.Text & "', Email='" & txtEmail.Text & "', Name='" & txtName.Text & "' , Comments='" & txtComments.Text & "',  "
                        'sqlt = sqlt & " ConnStr='" & txtConnStr.Text & "',  ConnPrv='" & prv & "',  NetId='" & txtLogon.Text & "',  localpass='" & txtEmail.Text & "', "
                        'sqlt = sqlt & " ACCESS='" & acss & "', PERMIT='" & fr & "', RoleApp='" & rol & "', StartDate='" & txtStartDate.Text & "', EndDate='" & txtEndDate.Text & "', Application='InteractiveReporting'"
                        'If txtUnit.Text.Trim = "CSV" Then
                        '    sqlt = sqlt & ", Group3='CSV'"
                        'End If
                        'ret = ExequteSQLquery(sqlt)
                        ret = RegisterUser(acss, fr, txtLogon.Text.Trim, txtEmail.Text.Trim, txtName.Text, txtUnit.Text.Trim, "InteractiveReporting", rol, "", "", grs, txtEmail.Text, "user definition-" & txtComments.Text, txtConnStr.Text.Trim, prv, DateToString(txtStartDate.Text), DateToString(txtEndDate.Text))

                        WriteToAccessLog(Session("logon"), "User created with result: " & ret, 11)
                    End If


                Else
                    Dim rt As String = "Request to add User crashed. User #" & txtLogon.Text & " is already there."
                    MessageBox.Show(rt, "Double User", "DoubleUser", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                    Exit Sub
                End If

            Else
                Dim sqlt As String = "UPDATE OURPermits SET Unit='" & txtUnit.Text & "', Email='" & txtEmail.Text & "', Name='" & txtName.Text & "' , Comments='" & "user updated, " & txtComments.Text & "',  "
                If Session("admin") = "super" Then
                    sqlt = sqlt & " ConnStr='" & txtConnStr.Text & "',  ConnPrv='" & prv & "',  NetId='" & txtLogon.Text & "', " ' localpass='" & txtEmail.Text & "', "
                End If
                sqlt = sqlt & " ACCESS='" & acss & "', PERMIT='" & fr & "', RoleApp='" & rol & "', Group3='" & grs & "', StartDate='" & DateToString(CDate(txtStartDate.Text)) & "', EndDate='" & DateToString(CDate(txtEndDate.Text)) & "'  WHERE Indx='" & Session("UserIndx") & "'"
                ret = ExequteSQLquery(sqlt)
                WriteToAccessLog(Session("logon"), "User updated with result: " & ret, 11)
            End If
        Catch ex As Exception
            ret = ret & ex.Message
        End Try
        Response.Redirect("SiteAdmin.aspx?unitdb=yes")
    End Sub

    Private Sub btnDeleteUser_Click(sender As Object, e As EventArgs) Handles btnDeleteUser.Click
        'Dim ret As String = "Request to delete User. User #" & Session("UserIndx") & " will be permanently deleted. Please confirm."
        Dim ret As String = "Request to disable User #" & Session("UserIndx") & ". Please confirm."
        MessageBox.Show(ret, "Disable User", "DisableUser", Controls_Msgbox.Buttons.OKCancel, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.Cancel)

    End Sub
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim sqlt As String = "SELECT * FROM OURPermits WHERE Indx='" & Session("UserIndx") & "'"
        Dim Err As String = String.Empty
        Dim dt As DataTable = mRecords(sqlt, Err).Table
        Dim sUserId As String
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            sUserId = dt.Rows(0)("NetId").ToString.Trim
            Dim ret As String = "User " & sUserId & ", in addition to his reports and access logs, will  be permanently deleted." & vbCrLf & "Please confirm."

            MessageBox.Show(ret, "Delete User", "DeleteUser", Controls_Msgbox.Buttons.OKCancel, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.Cancel)
        End If
    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "DisableUser" Then
            Select Case e.Result
                Case Controls_Msgbox.MessageResult.OK
                    Dim duedate As String = DateToString(Now)
                    Dim sqlt As String = "UPDATE OURPermits SET EndDate='" & duedate & "'   WHERE Indx='" & Session("UserIndx") & "'"
                    Dim ret As String = ExequteSQLquery(sqlt)
                    WriteToAccessLog(Session("logon"), "User #" & Session("UserIndx") & " disabled with result: " & ret, 11)
                    'Response.Write("<script language='javascript'> { window.close(); }</script>")
                    Response.Redirect("SiteAdmin.aspx?unitdb=yes")
                Case Controls_Msgbox.MessageResult.Cancel
                    txtUnit.Focus()
            End Select
        ElseIf e.Tag = "DeleteUser" Then
            Select Case e.Result
                Case Controls_Msgbox.MessageResult.OK
                    Dim sqlt As String = "SELECT * FROM OURPermits WHERE Indx='" & Session("UserIndx") & "'"
                    Dim Err As String = String.Empty
                    Dim ret As String
                    Dim dt As DataTable = mRecords(sqlt, Err).Table
                    Dim sUserId As String
                    Dim sRole As String
                    Dim sStartReport As String
                    Dim sReportID As String

                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        sUserId = dt.Rows(0)("NetId").ToString.Trim
                        sRole = dt.Rows(0)("RoleApp").ToString.Trim
                        sStartReport = sUserId & Session("UserIndx") & "_"
                        sStartReport = sStartReport.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")
                        If sRole.ToUpper <> "SUPER" Then
                            sqlt = "SELECT * FROM OURPermissions WHERE NetId = '" & sUserId & "' AND param1 like '" & sStartReport & "%'"
                            dt = mRecords(sqlt, Err).Table
                            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                                For i As Integer = 0 To dt.Rows.Count - 1
                                    sReportID = dt.Rows(i)("param1")
                                    DeleteReport(sReportID)
                                    'ret = ExequteSQLquery("DELETE FROM OURFiles WHERE ReportID='" & sReportID & "'")
                                Next
                                'ret = ExequteSQLquery("DELETE FROM OURUserTables WHERE UserID ='" & sUserId & "'")
                                ret = ExequteSQLquery("DELETE FROM OURPermissions WHERE NetId = '" & sUserId & "' AND param1 like '" & sStartReport & "%'")
                            End If
                            ret = ExequteSQLquery("DELETE FROM OURPermits WHERE Indx='" & Session("UserIndx") & "'")

                            'Delete accesslog entries for this user
                            sqlt = "DELETE FROM OURAccessLog WHERE Logon LIKE '" & sUserId & "%" & "'"
                            ret = ExequteSQLquery(sqlt)
                            Dim url As String = "SiteAdmin.aspx?msg=" & "User " & sUserId & " and any reports and access logs created by the user have been deleted."
                            Response.Redirect(url)
                        End If
                    End If
                    Response.Redirect("SiteAdmin.aspx?unitdb=yes")
                Case Controls_Msgbox.MessageResult.Cancel
                    txtUnit.Focus()
            End Select
        ElseIf e.Tag = "DoubleUser" Then
            txtLogon.Focus()
        End If
    End Sub
End Class
