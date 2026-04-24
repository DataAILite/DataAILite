Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing

Partial Class UnitRegistration
    Inherits System.Web.UI.Page
    Private indx As String
    Private lblSqlServer As String
    Private lblCache As String
    Private lblOracle As String
    Private Sub UnitRegistration_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        LabelDBpassword.Visible = False
        txtDBpass.Visible = False
        txtDBpass.Enabled = False
        Session("WEBOUR") = ConfigurationManager.AppSettings("weboureports").ToString
        Session("SupportEmail") = ConfigurationManager.AppSettings("supportemail").ToString
        HyperLink1.NavigateUrl = Session("WEBOUR") & "index.aspx"
        'lblSqlServer = "Server=yourserver; Database=yourdatabase; User ID=youruserid; Password=yourpassword"
        'lblCache = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser; Password = yourpassword"
        'lblOracle = "data source=yourserver:yourport/yourdatabase;user id=youruser;password=yourpassword"
        lblSqlServer = "Server=yourserver; Database=yourdatabase; User ID=youruserid;"
        lblCache = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser;"
        lblOracle = "data source=yourserver:yourport/yourdatabase;user id=youruser;"
        If Not Request("org") Is Nothing Then
            Session("org") = Request("org").ToString
        End If
        If Session("org") Is Nothing Then
            Session("org") = ""
        End If
        'it will be saved in Prop1 field of OURUnits table
        'If Request("org") Is Nothing AndAlso Session("org") = "" Then
        '    Response.Redirect("Default.aspx")
        '    Exit Sub
        'End If
        btnUpdate.Enabled = False
        btnUpdate.Visible = False
        btnSave.Enabled = False
        btnSave.Visible = False
        Date1.Enabled = False
        Date2.Enabled = False
        Dim i As Integer
        Dim err As String = String.Empty
        Dim selprov As String = String.Empty
        Dim seluprov As String = String.Empty
        If Not Request("indx") Is Nothing Then
            indx = Request("indx").ToString
        ElseIf Not Session("UnitIndx") Is Nothing Then
            indx = Session("UnitIndx").ToString.Trim
        Else
            indx = ""
            btnUpdate.Enabled = False
            btnUpdate.Visible = False
            chkTermsAndCond.Checked = False
        End If
        Session("UnitIndx") = indx
        Label9.Visible = False
        'txtLogon.Text = Session("logon").ToString.Trim
        If indx = "" Then
            'new unit
            Label10.Text = "new unit"
            txtUserConnStr.Text = lblSqlServer
            ddUserConnPrv.SelectedValue = "System.Data.SqlClient"
            btnUpdate.Enabled = False
            btnUpdate.Visible = False

            HyperLinkSeeProposal.Enabled = False
            HyperLinkSeeProposal.Visible = False
            HyperLinkUnitOURWeb.Enabled = False
            HyperLinkUnitOURWeb.Visible = False

            Date1.Text = DateToString(CDate(Now()))
            Date2.Text = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 30, Now())))
            txtComments.Text = "On " & Date1.Text & " "
        Else
            'btnUpdate.Enabled = False
            'btnUpdate.Visible = False

            ''txtLogon.Enabled = False
            'Session("UnitIndx") = indx
            'Label10.Text = "Unit index #" & indx
            'Dim mSql As String = "SELECT * FROM OURUnits WHERE Indx='" & indx & "'"
            'Dim dt As DataTable = mRecords(mSql, err).Table  'Data for report by SQL statement from the OURdb database
            'If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            '    Response.Redirect("Default.aspx?msg=User is not found.")
            '    Exit Sub
            'Else
            '    txtUnitWeb.Text = dt.Rows(0)("UnitWeb").ToString.Trim
            '    txtUnit.Text = dt.Rows(0)("Unit").ToString.Trim
            '    txtOURdb.Text = dt.Rows(0)("OURConnStr").ToString.Trim
            '    txtUserConnStr.Text = dt.Rows(0)("UserConnStr").ToString.Trim
            '    txtComments.Text = dt.Rows(0)("Comments").ToString.Trim & " | On " & DateToString(CDate(Now())) & " edited by " & txtLogon.Text
            '    'txtLogon.Text =
            '    For i = 0 To ddModels.Items.Count - 1
            '        If ddModels.Items(i).Value = dt.Rows(0)("DistrMode").ToString.Trim.Replace(" ", "") Then
            '            ddModels.Items(i).Selected = True
            '        End If
            '    Next
            '    For i = 0 To ddOURConnPrv.Items.Count - 1
            '        If ddOURConnPrv.Items(i).Value = dt.Rows(0)("OURConnPrv").ToString.Trim Then
            '            ddOURConnPrv.Items(i).Selected = True
            '            selprov = ddOURConnPrv.Items(i).Text.ToUpper
            '        End If
            '    Next
            '    For i = 0 To ddUserConnPrv.Items.Count - 1
            '        If ddUserConnPrv.Items(i).Value = dt.Rows(0)("UserConnPrv").ToString.Trim Then
            '            ddUserConnPrv.Items(i).Selected = True
            '            seluprov = ddUserConnPrv.Items(i).Text.ToUpper
            '        End If
            '    Next
            '    Date1.Text = dt.Rows(0)("StartDate").ToString.Trim
            '    Date2.Text = dt.Rows(0)("EndDate").ToString.Trim

            '    Session("CurrentEndDate") = dt.Rows(0)("EndDate").ToString.Trim
            '    Session("Unit") = dt.Rows(0)("Unit").ToString.Trim
            '    Session("UnitDB") = dt.Rows(0)("UserConnStr").ToString.Trim
            'End If
        End If

        For i = 0 To ddOURConnPrv.Items.Count - 1
            If ddOURConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SqlServerProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "SQL"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "CACHE"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "IRIS"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "MYSQL"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "Npgsql"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "ORACLE"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "CSV"
                Session("CSV") = "yes"
            End If
        Next

        For i = 0 To ddUserConnPrv.Items.Count - 1
            If ddUserConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SqlServerProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "SQL"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "CACHE"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "IRIS"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "MYSQL"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "Npgsql"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "ORACLE"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "CSV"
                Session("CSV") = "yes"
            End If
        Next
        If Not Request("res") Is Nothing AndAlso Request("res").ToString.Trim <> "" Then
            Label9.Text = cleanText(Request("res").ToString)
        End If

    End Sub

    Private Sub UnitRegistration_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack AndAlso txtPassword.Text.Trim = "" Then
            txtPassword.BorderColor = Color.Red
        End If
        If IsPostBack AndAlso txtRepeat.Text.Trim = "" Then
            txtRepeat.BorderColor = Color.Red
        End If
        'Label29
        Dim nm As String = ConfigurationManager.AppSettings("unit").ToString.Trim
        If nm <> "OUR" Then
            Label29.Visible = False
        End If
        If Session("org") = "company" OrElse Session("org") = "vendor" OrElse Session("org").ToString.Trim = "" Then
            If chkOURdb.Checked Then
                ddModels.SelectedItem.Text = "Unit OUReports on OUR server, Unit OURdb on Unit data server"
                txtOURdb.Text = ""
                ddOURConnPrv_SelectedIndexChanged(sender, e)
            Else
                txtOURdb.Text = ConfigurationManager.AppSettings("unitOURdbConnStr").ToString 'at this point with generic name Database=OURdataUnit 
                ddOURConnPrv.SelectedValue = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                ddModels.SelectedItem.Text = "Unit OUReports and Unit OURdb on OUR server"
            End If

            txtUnitWeb.Text = ConfigurationManager.AppSettings("unitOUReportsWeb").ToString.Replace("OURUnitWeb", "") & "[yoursubfolder] - will be assigned after registration"
            'txtUnitWeb.Text = "https://OUReports.net/[yoursubfolder] - will be assigned after registration"
            txtUnitWeb.Enabled = False
            trOURdb.Visible = False
            trOURdbPass.Visible = False
            trOURprv.Visible = False
            trModels.Visible = False
            tr9.Visible = False
            tr10.Visible = False
            tr12.Visible = False
        ElseIf Session("org") = "vendor" Then
            ddModels.SelectedValue = "Unit OUReports on UNIT server"
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        'SAMPLE CLOSING WINDOWS FROM CODE BEHIND: - DO NOT DELETE!!!
        'Response.Write("<script language='javascript'> { window.close(); }</script>")
        Response.Redirect("index1.aspx")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'check textboxes
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Try
            If cleanText(txtLogon.Text.Trim) <> txtLogon.Text.Trim Then
                ret = "Illegal character found in Logon text."
                Exit Try
            End If
            If cleanText(txtPassword.Text.Trim) <> txtPassword.Text.Trim Then
                ret = "Illegal character found in Password text."
                Exit Try
            End If
            If cleanText(txtRepeat.Text.Trim) <> txtRepeat.Text.Trim Then
                ret = "Illegal character found in Password Repeat text."
                Exit Try
            End If
            If cleanText(txtName.Text.Trim) <> txtName.Text.Trim Then
                ret = "Illegal character found in Name text."
                Exit Try
            End If
            If cleanText(txtPhone.Text.Trim) <> txtPhone.Text.Trim Then
                ret = "Illegal character found in Phone text."
                Exit Try
            End If
            If cleanText(txtEmail.Text.Trim) <> txtEmail.Text.Trim Then
                ret = "Illegal character found in Email text."
                Exit Try
            End If
            If cleanText(txtUnitWeb.Text.Trim) <> txtUnitWeb.Text.Trim Then
                ret = "Illegal character found in Unit Web text."
                Exit Try
            End If
            If cleanText(txtUnit.Text.Trim) <> txtUnit.Text.Trim Then
                ret = "Illegal character found in Unit text."
                Exit Try
            End If
            If cleanText(txtUnitBossName.Text.Trim) <> txtUnitBossName.Text.Trim Then
                ret = "Illegal character found in Unit Official Title and Name text."
                Exit Try
            End If
            If cleanText(txtUnitPhone.Text.Trim) <> txtUnitPhone.Text.Trim Then
                ret = "Illegal character found in Unit Phone text."
                Exit Try
            End If
            If cleanText(txtUnitEmail.Text.Trim) <> txtUnitEmail.Text.Trim Then
                ret = "Illegal character found in Unit Email text."
                Exit Try
            End If
            If cleanText(txtOURdb.Text.Trim) <> txtOURdb.Text.Trim Then
                ret = "Illegal character found in OUR db connection string."
                Exit Try
            End If
            If cleanText(txtOURdbPass.Text.Trim) <> txtOURdb.Text.Trim Then
                ret = "Illegal character found in OUR db password connection string."
                Exit Try
            End If
            If cleanText(txtUserConnStr.Text.Trim) <> txtUserConnStr.Text.Trim Then
                ret = "Illegal character found in User db connection string."
                Exit Try
            End If
            If cleanText(txtComments.Text.Trim) <> txtComments.Text.Trim Then
                ret = "Illegal character found in Comments."
                Exit Try
            End If
            If cleanText(txtAgentName.Text.Trim) <> txtAgentName.Text.Trim Then
                ret = "Illegal character found in Agent Name text."
                Exit Try
            End If
            If cleanText(txtAgentPhone.Text.Trim) <> txtAgentPhone.Text.Trim Then
                ret = "Illegal character found in Agent Phone text."
                Exit Try
            End If
            If cleanText(txtAgentEmail.Text.Trim) <> txtAgentEmail.Text.Trim Then
                ret = "Illegal character found in Agent Email text."
                Exit Try
            End If

            If txtPassword.Text <> txtRepeat.Text Then
                ret = "Password and Repeat Password do not match."
                Exit Try
            End If
            If cleanText(txtDBpass.Text.Trim) <> txtDBpass.Text.Trim Then
                ret = "Illegal character found in Database Password text."
                Exit Try
            End If
            'If txtLogon.Text.Trim = "" OrElse txtPassword.Text.Trim = "" OrElse txtDBpass.Text.Trim = "" OrElse txtName.Text.Trim = "" OrElse txtPhone.Text.Trim = "" OrElse txtEmail.Text.Trim = "" Then
            If txtLogon.Text.Trim = "" OrElse txtPassword.Text.Trim = "" OrElse txtName.Text.Trim = "" OrElse txtPhone.Text.Trim = "" OrElse txtEmail.Text.Trim = "" Then
                ret = "Fill out all form. Fields should not be empty."
                Exit Try
            End If
            'TODO fill out agent fields from url?
            'If chkOURAgent.Checked Then
            '    If txtName.Text.Trim = "" OrElse txtPhone.Text.Trim = "" OrElse txtEmail.Text.Trim = "" Then
            '        ret = "Fill out all form. Agent fields should not be empty."
            '        Exit Try
            '    End If
            'Else
            '    txtName.Text = ""
            '    txtPhone.Text = ""
            '    txtEmail.Text = ""
            'End If
            'TODO add more checking for fields as needed
            If txtUserConnStr.Text = lblSqlServer OrElse txtUserConnStr.Text = lblCache OrElse txtUserConnStr.Text = lblOracle Then
                txtUserConnStr.BorderColor = Color.Red
                ret = "Fill out proper connection string (server, database/namespace, user id, etc...)."
                Exit Try
            End If

            If Not chkOURdb.Checked Then
                'not checked
                If ddOURConnPrv.SelectedValue <> "MySql.Data.MySqlClient" AndAlso ddOURConnPrv.SelectedValue <> "Npgsql" Then
                    ret = "Unit cannot be created if the original oureports database is not MySql nor PostgreSQL."
                    Exit Try
                End If
            ElseIf chkOURdb.Checked Then
                txtOURdb.Text = ""
                ddOURConnPrv_SelectedIndexChanged(sender, e)
                If txtOURdbPass.Text.Trim = "" Then
                    txtOURdbPass.BorderColor = Color.Red
                    ret = "Fill out Unit Report Definition Database password."
                    Exit Try
                End If
            End If


            'Dim dbpass As String = String.Empty
            'Dim userconnstr As String
            'If Not Request("DBpass") Is Nothing AndAlso Request("DBpass").ToString.Trim <> "" Then
            '    dbpass = Request("DBpass").ToString.Trim
            'End If
            'If userconnstr.IndexOf(";Password=") < 0 AndAlso userconnstr.IndexOf(";PWD=") < 0 AndAlso dbpass <> "" Then
            '    userconnstr = userconnstr & "; " & ";Password=" & dbpass
            'End If

            'TODO check connections to databases and make warning
            If chkOURdb.Checked Then
                If Not DatabaseConnected(txtOURdb.Text.Trim & "; Password=" & txtOURdbPass.Text.Trim, ddOURConnPrv.SelectedValue, er) Then
                    ret = "Connection to Unit Reports Definition database can not be open... Wrong connection string: " & txtOURdb.Text & " or password"
                    Label9.Text = ret
                    ' WriteToAccessLog(txtLogon.Text, "Connection to Unit Reports Definition database cannot be open:" & txtOURdb.Text, 1)
                    Exit Try
                End If
            End If

            'remove what not needed from conn strings
            Dim ourdb1 As String = txtOURdb.Text.Trim
            Dim userdb1 As String = txtUserConnStr.Text.Trim
            Dim ourdb As String = txtOURdb.Text.Trim
            Dim userdb As String = txtUserConnStr.Text.Trim
            If ourdb.ToUpper.IndexOf("PASSWORD") > 0 Then ourdb1 = ourdb.Substring(0, ourdb.ToUpper.IndexOf("PASSWORD")).Trim
            If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb1 = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim

            'If ourdb1.ToUpper.IndexOf("USER ID") > 0 Then ourdb1 = ourdb1.Substring(0, ourdb1.ToUpper.IndexOf("USER ID")).Trim
            'If userdb1.ToUpper.IndexOf("USER ID") > 0 Then userdb1 = userdb1.Substring(0, userdb1.ToUpper.IndexOf("USER ID")).Trim

            If ddUserConnPrv.SelectedItem.Text.ToUpper <> "ORACLE" Then
                If userdb1.ToUpper.IndexOf("USER ID") > 0 Then userdb1 = userdb1.Substring(0, userdb1.ToUpper.IndexOf("USER ID")).Trim
                If userdb1.ToUpper.IndexOf("UID") > 0 Then userdb1 = userdb1.Substring(0, userdb1.ToUpper.IndexOf("UID")).Trim
            Else
                If userdb1.ToUpper.IndexOf("PASSWORD") > 0 Then userdb1 = userdb1.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
            End If

            If ddOURConnPrv.SelectedItem.Text.ToUpper <> "ORACLE" Then
                If ourdb1.ToUpper.IndexOf("USER ID") > 0 Then ourdb1 = ourdb1.Substring(0, ourdb1.ToUpper.IndexOf("USER ID")).Trim
                If ourdb1.ToUpper.IndexOf("UID") > 0 Then ourdb1 = ourdb1.Substring(0, ourdb1.ToUpper.IndexOf("UID")).Trim
            Else
                If ourdb1.ToUpper.IndexOf("PASSWORD") > 0 Then ourdb1 = ourdb1.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
            End If

            'insert or update OURUnits
            Dim sqlt As String = String.Empty
            Dim n As Integer = 0
            Dim agentindx As String = Session("agent")
            Dim sqlag As String = String.Empty
            Dim sFields As String = String.Empty
            Dim sValues As String = String.Empty
            Dim dbstr As String = String.Empty
            Dim tm As String = String.Empty
            Dim hasrecord As Boolean
            If indx = "" Then

                'check if unit is in ourunits table in oureports original ourdb !!!!!! Only one company can have particular userdb.
                sqlt = "SELECT * FROM OURUnits WHERE UserConnStr LIKE '%" & userdb1.Replace(" ", "%") & "%'"
                hasrecord = HasRecords(sqlt)
                    'if false no that database in OURUnits
                    If hasrecord = True Then
                        Label9.Text = "Request to add Unit cannot be completed. Unit with the same unit data database " & userdb & " is already registered."
                        Label9.Visible = True
                        Label9.ForeColor = Color.Red
                        Exit Sub
                    Else
                        Label9.Text = ""
                        Label9.Visible = False
                        Label9.ForeColor = Color.Gray
                    End If

                If chkOURdb.Checked Then 'OURdb on unit server
                    'check if unit is in ourunits table in oureports original ourdb !!!!!! Only one company can have particular unit ourdb.
                    sqlt = "SELECT * FROM OURUnits WHERE OURConnStr LIKE '" & ourdb1 & "%'"
                    hasrecord = HasRecords(sqlt)
                    'if false no that database in OURUnits
                    If hasrecord = True Then
                        Label9.Text = "Request to add Unit cannot be completed. Unit with the same unit report definitions database " & ourdb1 & " is already registered."
                        Label9.Visible = True
                        Label9.ForeColor = Color.Red
                        Exit Sub
                    Else
                        Label9.Text = ""
                        Label9.Visible = False
                        Label9.ForeColor = Color.Gray
                    End If
                End If

                txtComments.Text = txtComments.Text & " added by " & txtLogon.Text & ", agreed to Terms and Conditions."

                'insert into ourreports ourdb database the unit info
                sFields = "Unit,DistrMode,UnitWeb,Comments,StartDate,EndDate,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Official,Address,Phone,Email,Prop1"
                sValues = "'" & txtUnit.Text.Trim & "',"
                sValues &= "'" & ddModels.SelectedValue & "',"
                sValues &= "'" & txtUnitWeb.Text & "',"
                sValues &= "'" & txtComments.Text & "',"
                sValues &= "'" & Date1.Text & "',"
                sValues &= "'" & Date2.Text & "',"
                sValues &= "'" & ourdb1 & "',"
                sValues &= "'" & ddOURConnPrv.SelectedValue & "',"
                sValues &= "'" & userdb1 & "',"
                sValues &= "'" & ddUserConnPrv.SelectedValue & "',"
                sValues &= "'" & txtUnitBossName.Text.Trim & "',"
                sValues &= "'" & txtUnitAddress.Text & "',"
                sValues &= "'" & txtUnitPhone.Text & "',"
                sValues &= "'" & txtUnitEmail.Text & "',"
                sValues &= "'" & Session("org") & "'"

                sqlt = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"

                er = ExequteSQLquery(sqlt)  'unit registered in original site

                'correct db name and web address for new unit
                Dim unitname As String = txtUnit.Text.Trim.Replace(" ", "")
                Dim unitindx As String = String.Empty
                'Dim tm As String = String.Empty
                Dim unitourdbname As String = String.Empty
                Dim unitcsvdbname As String = String.Empty
                Dim nm As String = ConfigurationManager.AppSettings("unit").ToString 'original unit
                Dim ourdbconn As String = String.Empty
                If er = "Query executed fine." Then  'unit inserted in ourdb in original site
                    ret = "Unit has been registered in our database."
                    WriteToAccessLog(txtLogon.Text, "Unit has been registered:" & unitname & ", ConnStr:" & txtUserConnStr.Text, 1)
                    'MessageBox.Show("Unit has been registered", "New Unit Registration", "NewUnitRegistration", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                    'TODO add activity into OURActivity table (?)

                    Dim dtunit As DataTable = mRecords("SELECT * FROM OURUnits WHERE OURConnStr LIKE '" & ourdb1 & "%' AND UserConnStr LIKE '" & userdb1 & "%'").Table
                    If Not dtunit Is Nothing AndAlso dtunit.Rows.Count > 0 AndAlso dtunit.Rows(0)("Indx").ToString <> "" Then
                        unitindx = dtunit.Rows(0)("Indx").ToString
                        Session("UnitIndx") = unitindx
                        If chkOURdb.Checked Then 'OURdb on unit server
                            unitourdbname = GetDataBase(ourdb1)
                            ourdbconn = txtOURdb.Text.Trim & "; Password=" & txtOURdbPass.Text.Trim
                        Else 'not checked - OURdb on our data server
                            'create databases on our server
                            unitourdbname = "OURdataUnit" & unitindx & unitname & nm
                            If DatabaseExist(unitourdbname) Then
                                tm = DateToString(Now).Replace("-", "").Replace(":", "").Replace(" ", "")
                                unitourdbname = unitourdbname & tm
                            End If
                            sqlt = "CREATE DATABASE `" & unitourdbname & "`"
                            ret = ExequteSQLquery(sqlt)
                            unitcsvdbname = unitourdbname
                            'unitcsvdbname = "CSVdataUnit" & unitindx & unitname & nm
                            'If DatabaseExist(unitcsvdbname) Then
                            '    tm = DateToString(Now).Replace("-", "").Replace(":", "").Replace(" ", "")
                            '    unitcsvdbname = unitcsvdbname & tm
                            'End If
                            'sqlt = "CREATE DATABASE `" & unitcsvdbname & "`"
                            'ret = ExequteSQLquery(sqlt)

                            'update txtOURdb with unit ourdb name !!! at this point txtOURdb has new unit ourdb name inside conn string
                            txtOURdb.Text = ConfigurationManager.AppSettings("unitOURdbConnStr").ToString.Replace("OURdataUnit", unitourdbname)
                            ourdbconn = txtOURdb.Text.Trim
                        End If


                        txtUnitWeb.Text = ConfigurationManager.AppSettings("unitOUReportsWeb").ToString.Replace("OURUnitWeb", "Unit" & unitindx & unitname & nm)

                        'update ourunits in our original db with at this point unit ourdb and unitweb
                        'sqlt = "UPDATE OURUnits SET UnitWeb='" & txtUnitWeb.Text & "' , OURConnStr='" & txtOURdb.Text.Trim & "' WHERE Indx='" & Session("UnitIndx") & "'"
                        'ourdb1
                        sqlt = "UPDATE OURUnits SET UnitWeb='" & txtUnitWeb.Text & "' , OURConnStr='" & ourdb1.Trim & "' WHERE Indx='" & Session("UnitIndx") & "'"
                        ret = ret & " - Unit " & unitname & " updated " & sqlt & "  with result: " & ExequteSQLquery(sqlt)

                        'If agentindx <> "" Then
                        '    'sqlag = "INSERT INTO OURActivity SET Unit=" & unitindx & ", Agent=" & agentindx & ",Activity='Unit registration', AgentLevel='" & ddAgentHelps.SelectedItem.Text & "'"
                        '    'er = ExequteSQLquery(sqlag)
                        '    WriteToAccessLog(txtLogon.Text, "Unit" & unitindx & " registration, agent=" & agentindx, 1)
                        '    'TODO pay to agent for unit registration !!!!!! PayPal to email?
                        'End If

                        '!!!!!!!!!!!!!!!  at this point ourdbconn is unit ourdb connection string !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        ret = UpdateOURdbToCurrentVersion(ourdbconn, ddOURConnPrv.SelectedValue, False)
                        'update ourpermits table in unit ourdb with unit name
                        sqlt = "UPDATE OURPermits SET Unit='" & unitname & "'"
                        ret = ret & ", " & ExequteSQLquery(sqlt, ourdbconn, ddOURConnPrv.SelectedValue)
                        WriteToAccessLog(txtLogon.Text, "OURdataUnit has been installed and database:" & unitourdbname & " updated to current version with result" & ret, 1)

                        'insert or update siteadmin, who filled out the form, into OURPermits in Unit OURdb
                        Dim hasrec As Boolean = HasRecords("SELECT * FROM OURPERMITS WHERE (NetId='" & txtLogon.Text & "') ", ourdbconn, ddOURConnPrv.SelectedValue)
                        If hasrec Then
                            ret = "The logon is not available. Please select a different one."
                            WriteToAccessLog(txtLogon.Text, "Unit" & unitindx & ". The logon is not available. Please select a different one. " & txtUserConnStr.Text, 1)
                            Exit Try
                        End If
                        ret = RegisterUser("SITEADMIN", "friendly", txtLogon.Text, txtPassword.Text, txtName.Text, txtUnit.Text, "InteractiveReporting", "admin", "", "", "", txtEmail.Text, "site registration", txtUserConnStr.Text.Trim, ddUserConnPrv.SelectedValue, DateToString(Date1.Text), DateToString(Date2.Text), txtPhone.Text, ourdbconn, ddOURConnPrv.SelectedValue)

                    End If


                    'add record in ourunits in unit ourdb
                    Try
                        'make Unit record in unit ourdb as ourdb, ddOURConnPrv.SelectedValue
                        sFields = "Unit,DistrMode,UnitWeb,Comments,StartDate,EndDate,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Official,Address,Phone,Email,Prop1"
                        sValues = "'" & txtUnit.Text.Trim & "',"
                        sValues &= "'" & ddModels.SelectedValue & "',"
                        sValues &= "'" & txtUnitWeb.Text & "',"
                        sValues &= "'" & txtComments.Text & "',"
                        sValues &= "'" & Date1.Text & "',"
                        sValues &= "'" & Date2.Text & "',"
                        sValues &= "'" & ourdb & "',"
                        sValues &= "'" & ddOURConnPrv.SelectedValue & "',"
                        sValues &= "'" & txtUserConnStr.Text.Trim & "',"
                        sValues &= "'" & ddUserConnPrv.SelectedValue & "',"
                        sValues &= "'" & txtUnitBossName.Text.Trim & "',"
                        sValues &= "'" & txtUnitAddress.Text & "',"
                        sValues &= "'" & txtUnitPhone.Text & "',"
                        sValues &= "'" & txtUnitEmail.Text & "',"
                        sValues &= "'" & Session("org") & "'"
                        sqlt = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"
                        ret = ExequteSQLquery(sqlt, ourdbconn, ddOURConnPrv.SelectedValue)
                    Catch ex As Exception
                        ret = ex.Message
                    End Try

                    Dim EmailTable As DataTable
                    Dim j As Integer
                    EmailTable = mRecords("SELECT  * FROM OURPERMITS WHERE APPLICATION='InteractiveReporting' AND RoleApp='super'").Table
                    If Not ret.Contains("ERROR!!") Then
                        'send emails     
                        If EmailTable.Rows.Count > 0 Then
                            For j = 0 To EmailTable.Rows.Count - 1
                                ret = SendHTMLEmail("", "Unit" & unitindx & unitname & nm & " has been registered", "Unit web site " & txtUnitWeb.Text & " should be created ASAP! Folder Unit" & unitindx & unitname & nm & " should be created in the wwwroot. If not - copy Unit0OUR folder from wwwroot to wwwroot, rename it to Unit" & unitindx & unitname & nm & ". In IIS right click on Default Web Site and click Add Application. Fill out the form as this: Alias: Unit" & unitindx & unitname & nm & ", Physical path: browse and find the wwwroot/Unit" & unitindx & unitname & nm & " folder. Click OK. After that check that web.config had been updated: web.config should have the connection string to OURdataUnit" & unitindx & unitname & nm & " database as " & txtOURdb.Text, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                            Next
                        End If
                        ret = "Your OUReports web site will be available in 24-48 hours."
                    Else
                        'create urgent ticket
                        'send emails
                        If EmailTable.Rows.Count > 0 Then
                            For j = 0 To EmailTable.Rows.Count - 1
                                ret = SendHTMLEmail("", "Update the database " & unitourdbname & " to CurrentVersion erroring with: " & ret & ". Unit" & unitindx & unitname & nm & " has been registered in  ourunits table in original ourdb", "Unit web site " & txtUnitWeb.Text & " should be created ASAP! Database " & unitourdbname & " should be updated to current version.  Unit" & unitindx & unitname & nm & " should be created in the wwwroot. If not - copy Unit0OUR folder from wwwroot To wwwroot, rename it to Unit" & unitindx & unitname & nm & ". In IIS right click on Default Web Site And click Add Application. Fill out the form as this: Alias: Unit" & unitindx & unitname & nm & ", Physical path: browse and find the wwwroot/Unit" & unitindx & unitname & nm & " folder. Click OK. After that check that web.config had been updated: web.config should have the connection string to OURdataUnit" & unitindx & unitname & nm & " as " & txtOURdb.Text, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                            Next
                        End If
                        ret = "Your OUReports web site will be available in 24-48 hours."
                        Exit Try
                    End If

                    tr11.Visible = False
                    trText1.Visible = False
                    txtUserConnStr.Enabled = False
                    ddUserConnPrv.Enabled = False
                    btnSave.Visible = False
                    btnSave.Enabled = False
                    txtComments.Enabled = False
                    HyperLinkSeeProposal.Enabled = True
                    HyperLinkSeeProposal.Visible = True
                    HyperLinkUnitOURWeb.Enabled = True
                    HyperLinkUnitOURWeb.Visible = True
                    'Response.Redirect(Session("WEBHELPDESK").ToString & "ShowBusinessProposal.aspx?unit=" & unitindx)
                    HyperLinkSeeProposal.NavigateUrl = "Partners.pdf"
                    Dim uniturl As String = txtUnitWeb.Text.Trim
                    If Not uniturl.StartsWith("http") Then
                        'TODO https://
                        uniturl = "https://" & uniturl
                    End If
                    HyperLinkUnitOURWeb.NavigateUrl = uniturl
                Else
                    Dim rt As String = "Request to add Unit crashed. Unit #" & txtUnit.Text & " was not registered."
                    MessageBox.Show(rt, "No Unit", "NoUnit", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                End If

                Session("email") = txtEmail.Text
                Session("UserEndDate") = DateToString(Date2.Text)

                'If DatabaseConnected(ourdbconn, ddOURConnPrv.SelectedValue, er) AndAlso DatabaseConnected(txtUserConnStr.Text.Trim & ";" & txtDBpass.Text.Trim, ddUserConnPrv.SelectedValue, er) Then
                '    'TODO message about long time for data analysis
                '    'Make Initial Reports
                '    Session("UserConnString") = txtUserConnStr.Text.Trim & "; Password=" & txtDBpass.Text.Trim
                '    Session("UserConnProvider") = ddUserConnPrv.SelectedValue
                '    'TODO redo initial unit reports !!!!!!!!!!!!!!!!!!!!!!
                '    'Dim rtn As String = MakeInitialReports(Session("logon"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                'End If

                'Unit db created, now create folder in wwwroot
                Dim unitweb As String = "Unit" & unitindx & unitname & nm

                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                'Dim surl As String = "UnitWebOnServer.aspx?UnitWebIndex=" & unitindx & "&UnitWebKey=111&UnitName=" & unitname & "&UnitCsvdb=" & unitcsvdbname & "&UnitOurdb=" & unitourdbname & "&UnitWeb=" & unitweb
                'Response.Redirect(surl)
                'Response.End()
                If chkOURdb.Checked Then
                    WriteToAccessLog("Unit registration", "dbstr: " & ourdb1, 1)
                    dbstr = ourdb1
                Else
                    dbstr = ConfigurationManager.AppSettings("unitOURdbConnStr").ToString.Replace("OURdataUnit", unitourdbname)
                    WriteToAccessLog("Unit registration", "dbstr: " & dbstr, 1)
                End If


                If IsNumeric(unitindx) Then
                    'On Server the actual folder
                    Dim sourceFolder As String = Request.PhysicalApplicationPath
                    n = sourceFolder.LastIndexOf("\")
                    If n = sourceFolder.Length - 1 Then
                        sourceFolder = sourceFolder.Substring(0, n)
                        n = sourceFolder.LastIndexOf("\")
                    End If
                    sourceFolder = sourceFolder.Substring(0, n) & "\Unit0" & nm
                    'unitweb="Unit" & nm & unitindx & unitname
                    Dim outputFolder As String = sourceFolder.Replace("Unit0" & nm, unitweb)
                    If Directory.Exists(outputFolder) Then
                        'Label1.Text = "Site Unit" & unitindx & nm & " already exists. Please choose another unit abbreviation."
                        'Exit Sub
                        ' Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                        tm = DateToString(Now).Replace("-", "").Replace(":", "").Replace(" ", "")
                        outputFolder = outputFolder & tm
                    End If
                    'unitweb = ConfigurationManager.AppSettings("unitOUReportsWeb").ToString.Replace("OURUnitWeb", "Unit" & nm & unitindx & unitname & tm)
                    Dim unitweburl As String = ConfigurationManager.AppSettings("unitOUReportsWeb").ToString.Replace("OURUnitWeb", unitweb & tm)
                    WriteToAccessLog("Unit registration", "unitweb: " & unitweb, 1)

                    'update unitweb in original ourdb
                    sqlt = "UPDATE OURUnits SET UnitWeb='" & unitweburl & "'  WHERE Indx='" & unitindx & "'"
                    ret = ExequteSQLquery(sqlt)

                    ret = DirectoryCopy(sourceFolder, outputFolder, True)

                    If ret = "" Then 'created
                        'update web.config in outputFolder:  web.config should have the connection string to OURdataUnit" & unitindx & " OUR database
                        Dim webconfigpath As String = outputFolder & "\web.config"
                        Dim sr() As String = File.ReadAllLines(webconfigpath)
                        Dim userdbstr As String = String.Empty
                        Dim userdbprv As String = String.Empty
                        Dim unitdbstr As String = String.Empty
                        Dim unitdbprv As String = String.Empty
                        'Dim dv As DataView = mRecords("SELECT * FROM ourunits", ret, dbstr, System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString)
                        Dim dv As DataView = mRecords("SELECT * FROM ourunits", ret, ourdbconn, ddOURConnPrv.SelectedValue)
                        If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                            userdbstr = dv.Table.Rows(0)("UserConnStr")
                            userdbprv = dv.Table.Rows(0)("UserConnPrv")
                            unitdbstr = dv.Table.Rows(0)("OURConnStr")
                            unitdbprv = dv.Table.Rows(0)("OURConnPrv")
                        Else
                            WriteToAccessLog("Unit registration failed", "dbstr: " & dbstr & " ret: " & ret, 3)
                        End If
                        'update unitweb in unit db
                        sqlt = "UPDATE OURUnits SET UnitWeb='" & unitweburl & "'  WHERE Unit='" & unitname & "'"
                        ret = ExequteSQLquery(sqlt, ourdbconn, ddOURConnPrv.SelectedValue)

                        'make user connection strings in web.config from the very first record in the ourunit table from dbstr database
                        For i = 0 To sr.Length - 1

                            If sr(i).Contains("UserSqlConnection") Then
                                'UserSqlConnection
                                sr(i) = "      <add name=""UserSqlConnection"" connectionString=""" & userdbstr & """ providerName=""" & userdbprv & """ />"
                                WriteToAccessLog("Unit registration", "sr(" & i.ToString & "):" & sr(i), 2)
                            ElseIf sr(i).Contains("CSVconnection") Then
                                'CSVconnection
                                sr(i) = "      <add name=""CSVconnection"" connectionString=""" & userdbstr & """ providerName=""" & userdbprv & """ />"
                                WriteToAccessLog("Unit registration", "sr(" & i.ToString & "):" & sr(i), 2)
                            ElseIf sr(i).Contains("MySqlConnection") Then
                                'MySqlConnection
                                If chkOURdb.Checked Then 'OURdb on unit server  ' txtOURdb.Text.Trim, ddOURConnPrv.SelectedValue
                                    sr(i) = "      <add name=""MySqlConnection"" connectionString=""" & ourdbconn & """ providerName=""" & userdbprv & """ />"
                                    WriteToAccessLog("Unit registration", "sr(" & i.ToString & "):" & sr(i), 2)
                                Else
                                    sr(i) = sr(i).Replace("OURdataUnit;", unitourdbname & ";")
                                End If
                            Else
                                sr(i) = sr(i).Replace("OURdataUnit;", unitourdbname & ";")
                                'sr(i) = sr(i).Replace("CSVdataUnit;", unitcsvdbname & ";")
                            End If

                            If sr(i).Contains("<add key=""unit"" value=") Then
                                sr(i) = "   <add key=""unit"" value=""" & unitname & """/>"
                            End If
                            sr(i) = sr(i).Replace("/OURUnitWeb/", "/" & unitweb & tm & "/")

                            If sr(i).Contains("<add key=""SiteFor"" value=""") Then
                                sr(i) = "   <add key=""SiteFor"" value=""OUReports for " & unitname & """/>"
                            End If
                            If sr(i).Contains("<add key=""pagettl"" value=""") Then
                                sr(i) = "   <add key=""pagettl"" value=""" & "Online User Reporting and Analytics for " & unitname & """/>"
                            End If

                        Next
                        'write in file
                        File.WriteAllLines(webconfigpath, sr)

                        Label1.Text = "Unit Web will be available at " & unitweb & " in 24 hours. Please contact us if it does not happen."

                        Dim EmailTable As DataTable
                        Dim j As Integer
                        EmailTable = mRecords("SELECT  * FROM OURPERMITS WHERE APPLICATION='InteractiveReporting' AND RoleApp='super'").Table
                        If Not ret.Contains("ERROR!!") Then
                            'send emails 
                            Dim emailbody As String = "Unit web site " & unitweb & tm & " should be created ASAP! Folder " & outputFolder & " should already be created. "
                            emailbody = emailbody & "If Not - copy the folder " & sourceFolder & " from wwwroot to wwwroot, rename it to " & outputFolder & " and correct web.config with proper values of unitourdbname, unitcsvdbname, unitweburl, etc.. "
                            emailbody = emailbody & "In IIS right click on Default Web Site And right click on " & outputFolder & ". "
                            emailbody = emailbody & "From right click menu select ""Convert to application"" Or click Add Application. The form should be as this: "
                            'emailbody = emailbody & "Alias: Unit" & unitname & unitindx & nm & tm & ", Physical path: browse and find the folder " & outputFolder & ". Click OK. "
                            emailbody = emailbody & "Alias: " & unitweb & ", Physical path: browse and find the folder " & outputFolder & ". Click OK. "
                            emailbody = emailbody & "After that check that web.config had been updated: web.config should have the unit's OURdata database as " & unitourdbname & " the unit's CSVdata database as " & unitcsvdbname

                            If EmailTable.Rows.Count > 0 Then
                                For j = 0 To EmailTable.Rows.Count - 1
                                    ret = SendHTMLEmail("", "Unit #" & unitindx & " has been registered", emailbody, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                                Next
                            End If
                            ret = "Your OUReports web site will be available in 24-48 hours."
                        Else
                            ''create urgent ticket
                            ''send emails
                            'If EmailTable.Rows.Count > 0 Then
                            '    For j = 0 To EmailTable.Rows.Count - 1
                            '        ret = SendHTMLEmail("", "Update OURdb To CurrentVersion return: " & ret & ". Unit #" & unitindx & " has been registered", "Unit web site " & txtUnitWeb.Text & " should be created ASAP!  Unit" & unitindx & "OUR should be created in the wwwroot. If not - copy Unit0OUR folder from wwwroot To wwwroot, rename it to Unit" & unitindx & "OUR. In IIS right click on Default Web Site And click Add Application. Fill out the form as this: Alias: Unit" & unitindx & "OUR, Physical path: browse and find the wwwroot/Unit" & unitindx & "OUR folder. Click OK. After that check that web.config had been updated: web.config should have the connection string to OURdataUnit" & unitindx & " OUR database as " & txtOURdb.Text, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                            '    Next
                            'End If
                            'ret = "Your OUReports web site will be available in 24-48 hours."

                        End If
                    Else
                        Label1.Text = ret


                    End If
                Else
                    Label1.Text = "Index is not numeric. Unit Web has not been created."
                    'Response.Redirect("UnitWebOnServer.aspx?lbl=" & Label1.Text)
                End If
                'Else
                '    Label1.Text = "Unit Web has been created. Unit Web will be available at " & unitweb & " in 24 hours. Please contact us if it does not happen."
                '    'Response.End()
                'End If
                ret = Label1.Text



            Else 'existing Unit !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                'TODO update all fields and redirect to UserDefinition.aspx
                Dim ourdbconn As String = String.Empty
                If chkOURdb.Checked Then 'OURdb on unit server
                    ourdbconn = txtOURdb.Text.Trim & "; Password=" & txtOURdbPass.Text.Trim
                Else
                    ourdbconn = txtOURdb.Text.Trim
                End If
                'insert or update siteadmin, who fill out the form, into OURPermits in Unit OURdb
                Dim hasrec As Boolean = HasRecords("SELECT * FROM OURPERMITS WHERE (NetId='" & txtLogon.Text & "') ", ourdbconn, ddOURConnPrv.SelectedValue)
                If hasrec Then
                    ret = "The logon is not available. Please select a different one."
                    'WriteToAccessLog(txtLogon.Text, "Unit" & unitindx & ". The logon is not available. Please select a different one. " & txtUserConnStr.Text, 1)
                    Exit Try
                End If
                ret = RegisterUser("SITEADMIN", "friendly", txtLogon.Text, txtPassword.Text, txtName.Text, txtUnit.Text, "InteractiveReporting", "admin", "", "", "", txtEmail.Text, "site registration", txtUserConnStr.Text.Trim, ddUserConnPrv.SelectedValue, DateToString(Date1.Text), DateToString(Date2.Text), txtPhone.Text, ourdbconn, ddOURConnPrv.SelectedValue)
            End If

        Catch ex As Exception
            ret = ret & ex.Message
            Label9.Text = "Request to add Unit cannot be completed. " & ret
            Label9.Visible = True
            Label9.ForeColor = Color.Red
        End Try
        WriteToAccessLog(txtLogon.Text, ret, 11)
        MessageBox.Show(ret, "Unit Registration", "UnitRegistration", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
    End Sub

    'Private Sub btnDeleteUser_Click(sender As Object, e As EventArgs) Handles btnDeleteUser.Click
    '    Dim ret As String = "Request to disable Unit. Unit #" & Session("UnitIndx") & " will be permanently disabled along with Unit users and reports. Please confirm."
    '    MessageBox.Show(ret, "Disable Unit", "DisableUnit", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

    'End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "DisableUnit" Then
            Dim duedate As String = DateToString(Now)
            Dim sqlt As String = String.Empty
            Dim ret As String = String.Empty
            'disable active at this moment users and report permissions
            sqlt = "UPDATE OURPermits SET EndDate='" & duedate & "' WHERE Unit='" & Session("Unit") & "' AND ConnStr='" & Session("UnitDB") & "' AND (EndDate > '" & duedate & "')"
            ret = "Users in OURPermits updated with result: " & ExequteSQLquery(sqlt)

            sqlt = "UPDATE OURPermissions SET OpenTo='" & duedate & "' FROM OURPermissions INNER JOIN OURReportInfo ON (OURPermissions.Param1=OURReportInfo.ReportId) INNER JOIN OURPermits ON (OURPermissions.NetId=OURPermits.NetId) WHERE OURPermits.Unit='" & Session("Unit") & "' AND OURReportInfo.ReportDB LIKE '%" & Session("UnitDB").ToString.Trim.Replace(" ", "%") & "%' AND (OURPermissions.OpenTo > '" & duedate & "')"
            ret = ret & " -Reports in OURPermissions updated with result: " & ExequteSQLquery(sqlt)

            'disable Unit
            sqlt = "UPDATE OURUnits SET EndDate='" & duedate & "'  WHERE Indx='" & Session("UnitIndx") & "'"
            ret = ret & " -Unit updated with result: " & ExequteSQLquery(sqlt)

            WriteToAccessLog(txtLogon.Text, "Unit #" & Session("UnitIndx") & " disabled with result: " & ret, 11)
            Response.Redirect("UnitsAdmin.aspx")
        ElseIf e.Tag = "UpdateUnit" Then
            'update Unit OURdb to current version
            Dim ret As String = String.Empty
            ret = UpdateOURdbToCurrentVersion(txtOURdb.Text.Trim, ddOURConnPrv.SelectedValue, False)
            Label9.Text = ret
            'update Unit Web and web.config
            'TODO
            WriteToAccessLog(txtLogon.Text, "Unit #" & Session("UnitIndx") & " updated to current version with result: " & ret, 11)
            Response.Redirect("UnitDefinition.aspx?indx=" & Session("UnitIndx") & "&res=" & cleanText(ret.Replace("<br/>", " | ")))
        ElseIf e.Tag = "UninstallUnitOURdb" Then
            'Uninstall Unit's OURdb
            Dim ret As String = String.Empty
            ret = UninstallOURTablesClasses(txtOURdb.Text.Trim, ddOURConnPrv.SelectedValue)
            Label9.Text = ret
            'delete Unit Web
            'TODO
            WriteToAccessLog(txtLogon.Text, "Unit #" & Session("UnitIndx") & " OURdb tables uninstalled with result: " & ret, 11)
            Response.Redirect("UnitDefinition.aspx?indx=" & Session("UnitIndx") & "&res=" & cleanText(ret.Replace("<br/>", " | ")))
        ElseIf e.Tag = "DoubleUnit" Then
            txtUnit.Focus()
            'ElseIf e.Tag = "UnitRegistration" Then
            '    If chkTermsAndCond.Checked Then
            '        btnSave.Enabled = True
            '        btnSave.Visible = True
            '    Else
            '        btnSave.Enabled = False
            '        btnSave.Visible = False
            '    End If
            '    If chkOURdb.Checked Then
            '        trOURprv.Visible = True
            '        trOURdb.Visible = True
            '        trOURdbPass.Visible = True
            '        'txtOURdb.Text = "Server=yourserver; Database=yourdatabase; User ID=youruserid; Password=yourpassword"
            '    Else
            '        trOURprv.Visible = False
            '        trOURdb.Visible = False
            '        trOURdbPass.Visible = False
            '    End If
        Else
            Response.Redirect("index1.aspx")
        End If
    End Sub
    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If txtOURdb.Text.IndexOf("Password") < 0 Then
            txtOURdb.Text = txtOURdb.Text & " Password="
            txtOURdb.BorderColor = Color.Red
            txtOURdb.Focus()
        Else
            Dim ret As String = "Request to update the Unit. Unit #" & Session("UnitIndx") & " will be updated to the current version of OUReports. Please confirm."
            MessageBox.Show(ret, "Update Unit", "UpdateUnit", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If

    End Sub

    Private Sub ddUserConnPrv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddUserConnPrv.SelectedIndexChanged
        If ddUserConnPrv.SelectedItem.Text.StartsWith("Intersystems") Then
            txtUserConnStr.Text = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser;"
        ElseIf ddUserConnPrv.SelectedItem.Text = "Oracle" Then
            txtUserConnStr.Text = "data source=yourserver:yourport/yourdatabase;user id=youruser;"
        ElseIf ddUserConnPrv.SelectedItem.Text.StartsWith("Postgre") Then
            txtUserConnStr.Text = "Server = yourserver; Port = yourport; Database=yourdatabase; User ID=youruser;"
        Else
            txtUserConnStr.Text = "Server=yourserver; Database=yourdatabase; User ID=youruser;"
        End If
        If chkTermsAndCond.Checked Then
            btnSave.Enabled = True
            btnSave.Visible = True
        Else
            btnSave.Enabled = False
            btnSave.Visible = False
        End If
        If chkOURdb.Checked Then
            trOURprv.Visible = True
            trOURdb.Visible = True
            trOURdbPass.Visible = True
            'txtOURdb.Text = "Server=yourserver; Database=yourdatabase; User ID=youruserid; Password=yourpassword"
        Else
            trOURprv.Visible = False
            trOURdb.Visible = False
            trOURdbPass.Visible = False
        End If
    End Sub
    Private Sub ddOURConnPrv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddOURConnPrv.SelectedIndexChanged
        If ddOURConnPrv.SelectedItem.Text.StartsWith("Intersystems") Then
            txtOURdb.Text = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser;"
        ElseIf ddOURConnPrv.SelectedItem.Text = "Oracle" Then
            txtOURdb.Text = "data source=yourserver:yourport/yourdatabase;user id=youruser;"
        ElseIf ddOURConnPrv.SelectedItem.Text.StartsWith("Postgre") Then
            txtOURdb.Text = "Server = yourserver; Port = yourport; Database=yourdatabase; User ID=youruser;"
        Else
            txtOURdb.Text = "Server=yourserver; Database=yourdatabase; User ID=youruser;"
        End If
        If chkTermsAndCond.Checked Then
            btnSave.Enabled = True
            btnSave.Visible = True
        Else
            btnSave.Enabled = False
            btnSave.Visible = False
        End If
        If chkOURdb.Checked Then
            trOURprv.Visible = True
            trOURdb.Visible = True
            trOURdbPass.Visible = True
            'txtOURdb.Text = "Server=yourserver; Database=yourdatabase; User ID=youruserid; "
        Else
            trOURprv.Visible = False
            trOURdb.Visible = False
            trOURdbPass.Visible = False
        End If
    End Sub
    Private Sub chkTermsAndCond_CheckedChanged(sender As Object, e As EventArgs) Handles chkTermsAndCond.CheckedChanged
        If chkTermsAndCond.Checked Then
            btnSave.Enabled = True
            btnSave.Visible = True
        Else
            btnSave.Enabled = False
            btnSave.Visible = False
        End If
        If chkOURdb.Checked Then
            trOURprv.Visible = True
            trOURdb.Visible = True
            trOURdbPass.Visible = True
            'txtOURdb.Text = "Server=yourserver; Database=yourdatabase; User ID=youruserid;"
        Else
            trOURprv.Visible = False
            trOURdb.Visible = False
            trOURdbPass.Visible = False
        End If
    End Sub

    Private Sub chkOURdb_CheckedChanged(sender As Object, e As EventArgs) Handles chkOURdb.CheckedChanged
        If chkOURdb.Checked Then
            trOURprv.Visible = True
            trOURdb.Visible = True
            trOURdbPass.Visible = True
            'txtOURdb.Text = "Server=yourserver; Database=yourdatabase; User ID=youruserid;"
        Else
            trOURprv.Visible = False
            trOURdb.Visible = False
            trOURdbPass.Visible = False
        End If
        If chkTermsAndCond.Checked Then
            btnSave.Enabled = True
            btnSave.Visible = True
        Else
            btnSave.Enabled = False
            btnSave.Visible = False
        End If
    End Sub



    'Private Sub txtUnit_TextChanged(sender As Object, e As EventArgs) Handles txtUnit.TextChanged
    '    txtLogon.Enabled = True
    '    txtLogon.Text = ""
    '    Session("UnitIndx") = Nothing
    '    Response.Redirect("UnitRegistration.aspx")
    'End Sub
End Class

