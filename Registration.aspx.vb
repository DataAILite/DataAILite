Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing

Partial Class Registration
    Inherits System.Web.UI.Page
    Private Sub Registration_Init(sender As Object, e As EventArgs) Handles Me.Init
        btRegister.Enabled = False
        btRegister.Visible = False
        btnBrowse.Visible = False
        btnBrowse.Enabled = False
        btnBrowse.OnClientClick = "clickFileOleDbUpload();return false;"
        FileOleDb.Attributes.Add("onchange", "getAttachedFileOleDb();")
        Dim i As Integer = 0
        Dim selprov As String = String.Empty
        For i = 0 To dropdownDatabases.Items.Count - 1
            If dropdownDatabases.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "SQL"
            End If

            If dropdownDatabases.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "CACHE"

            End If

            If dropdownDatabases.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "IRIS"

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "MYSQL"

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "Npgsql"

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "USE OUR DATABASE" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "USE OUR DATABASE" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "CSV"
                Session("CSV") = "yes"
            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "Oracle"
                Session("Oracle") = "yes"
            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "ODBC" AndAlso ConfigurationManager.AppSettings("ODBC").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "ODBC" AndAlso ConfigurationManager.AppSettings("ODBC").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "ODBC"
                Session("ODBC") = "yes"
            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "OLEDB" AndAlso ConfigurationManager.AppSettings("OleDb").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False
            ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "OleDb" AndAlso ConfigurationManager.AppSettings("OleDb").ToString = "OK" Then
                dropdownDatabases.Text = dropdownDatabases.Items(i).Text
                dropdownDatabases.Items(i).Selected = True
                selprov = "OleDb"
                Session("OleDb") = "yes"
                'btnBrowse.Visible = True
                'btnBrowse.Enabled = True
            End If
        Next
    End Sub
    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        Label2.Text = ""
        Dim unt As String = ConfigurationManager.AppSettings("unit").ToString
        Session("Unit") = unt
        Unit.Text = unt
        If unt = "OUR" Then  'it is shared individual user or even csv user
            'Unit.Enabled = False
            'Unit.Visible = False
            trunit.Visible = False
        End If
        If System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso (Session("CSV") = "yes" OrElse dropdownDatabases.Text.ToUpper = "USE OUR DATABASE") Then
            ConnStr.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
            ConnStr.Enabled = False
            ConnStr.Visible = False
            'trprov.Visible = False
            trlabels.Visible = False
            trconnstr.Visible = False
            lblConnection.Visible = False
            lblPassWord.Visible = False
            lblConnection.Text = ""
            lblPassWord.Text = ""
            LabelProv.Visible = False
            Label2.Text = "Your data (CSV, XML, JSON, XLS, XLSX, Access table) can be uploaded into database on our server. Reports and analytics will be available only to you and to users you share them with."
        End If
        Dim userconnstr As String = String.Empty
        Dim userconnprv As String = String.Empty
        If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection") Is Nothing Then
            'sql connection to user database from webconfig 
            userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ToString
            userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ProviderName.ToString
        Else
            userconnstr = ""
            userconnprv = ""
        End If
        Session("UserConnString") = userconnstr
        Session("UserConnProvider") = userconnprv
        If userconnstr.Trim <> "" Then
            dropdownDatabases.SelectedValue = userconnprv
            'dropdownDatabases.Visible = False
            dropdownDatabases.Enabled = False
            ConnStr.Text = userconnstr.Substring(0, userconnstr.IndexOf("Password")).Trim
            'ConnStr.Visible = False
            ConnStr.Enabled = False
            lblConnection.Text = ""
            lblPassWord.Text = ""
            'Registr.Rows(5).Visible = False
            'Registr.Rows(6).Visible = False
        ElseIf Session("CSV") <> "yes" Then
            dropdownDatabases.Visible = True
            dropdownDatabases.Enabled = True
            ConnStr.Visible = True
            ConnStr.Enabled = True
            If ConnStr.Text.Trim = "" Then
                ConnStr.Text = lblConnection.Text
            End If
        ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso Session("CSV") = "yes" Then
            Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
            Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
            dropdownDatabases.Visible = False
            dropdownDatabases.Enabled = False
            ConnStr.Visible = False
            ConnStr.Enabled = False
            lblConnection.Visible = False
            lblPassWord.Visible = False
            lblConnection.Text = ""
            lblPassWord.Text = ""
            LabelProv.Visible = False
        End If
        Dim webinstall As String = ConfigurationManager.AppSettings("webinstall").ToString
        Dim dbinstall As String = ConfigurationManager.AppSettings("dbinstall").ToString
        If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then

        Else
            dropdownDatabases.Visible = False
            dropdownDatabases.Enabled = False
            ConnStr.Visible = False
            ConnStr.Enabled = False
            trunit.Visible = False
            trlabels.Visible = False
            trconnstr.Visible = False
            trprov.Visible = False
            lblConnection.Visible = False
        End If
    End Sub
    Protected Sub btRegister_Click(sender As Object, e As EventArgs) Handles btRegister.Click
        Dim ret As String = String.Empty
        Try
            If cleanText(Request("logon")) <> Request("logon").ToString Then
                ret = "Illegal character found. Try another logon."
                Exit Try
            ElseIf cleanText(Request("logon")) = "" Then
                ret = "Logon cannot be empty."
                Exit Try
            End If
            If cleanText(Request("Name")) <> Request("Name").ToString Then
                ret = "Illegal character found. Try another logon."
                Exit Try
            End If
            If cleanText(Request("Pass")) <> Request("Pass").ToString Then
                ret = "Illegal character found. Try another password."
                Exit Try
            ElseIf cleanText(Request("Pass")) = "" Then
                ret = "Password cannot be empty."
                Exit Try
            End If
            If cleanText(Request("RepeatPass")) <> cleanText(Request("Pass")) Then
                ret = "Passwords don't match"
                Exit Try
            End If
            If cleanText(Request("RepeatPass")) <> Request("RepeatPass").ToString Then
                ret = "Illegal character found. Please retype passwords"
                Exit Try
            ElseIf cleanText(Request("RepeatPass")) = "" Then
                ret = "Repeat Password cannot be empty."
                Exit Try
            End If
            If cleanText(Unit.Text) <> Unit.Text.Trim Then
                ret = "Illegal character found. Please retype Organization."
                Exit Try
            ElseIf cleanText(Unit.Text) = "" Then
                ret = "Organization cannot be empty."
                Exit Try
            Else
                Session("Unit") = Unit.Text
            End If
            If cleanText(Request("Email")) <> Request("Email").ToString.Trim Then
                ret = "Illegal character found. Please retype Email."
                Exit Try
            ElseIf cleanText(Request("Email")) = "" Then
                ret = "Email cannot be empty."
                Exit Try
            End If

            'check and save the user connection
            Dim userconnstr As String = String.Empty
            Dim userconnprv As String = String.Empty
            If cleanText(ConnStr.Text) <> ConnStr.Text.Trim Then
                ret = "Illegal character found. Please retype Connection String removing double quotes if needed."
                Exit Try
            ElseIf cleanText(ConnStr.Text) = "" OrElse ConnStr.Text = lblConnection.Text Then
                ret = "Please enter valid Connection String:"
                ConnStr.BorderColor = Color.Red
                Exit Try
            ElseIf cleanText(ConnStr.Text) = ConnStr.Text.Trim AndAlso ConnStr.Text.Trim.Length > 0 Then
                ConnStr.BorderColor = Color.Black
                Session("UserConnString") = cleanText(ConnStr.Text)
                If Session("CSV") <> "yes" Then
                    Session("UserConnProvider") = dropdownDatabases.SelectedValue
                End If

                If CheckBox1.Checked Then  'it is always checked and invisible
                    'save Connection info
                    userconnstr = Session("UserConnString")
                    userconnprv = Session("UserConnProvider")
                End If
            End If

            'CORRECT the User Connection String to unified format
            Dim connparts As String()
            Dim ord As Integer = 0
            connparts = Split(userconnstr, ";")
            userconnstr = ""
            For i = 0 To connparts.Length - 1
                connparts(i) = connparts(i).Replace(" ", "").Trim
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("SERVER=") Then
                    connparts(i) = connparts(i).Replace("SERVER=", "Server=").Replace("server=", "Server=")
                    userconnstr = userconnstr & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("PORT=") Then
                    connparts(i) = connparts(i).Replace("PORT=", "Port=").Replace("port=", "Port=")
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("DATABASE=") Then
                    connparts(i) = connparts(i).Replace("DATABASE=", "Database=").Replace("database=", "Database=")
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("DATASOURCE=") Then
                    connparts(i) = connparts(i).Replace("DATASOURCE=", "data source=").Replace("Data Source=", "data source=").Replace("DataSource=", "data source=").Replace("datasource=", "data source=")
                    If userconnprv = "Oracle.ManagedDataAccess.Client" Then
                        connparts(i) = connparts(i).Replace("data source=", "Data Source=")
                        userconnstr = connparts(i)
                    ElseIf i = 0 OrElse userconnprv = "System.Data.OleDb" Then
                        userconnstr = connparts(i)
                    Else
                        userconnstr = userconnstr & "; " & connparts(i)
                        End If

                    End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("NAMESPACE=") Then
                    connparts(i) = connparts(i).Replace("NAMESPACE=", "Namespace=").Replace("namespace=", "Namespace=")
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("DSN=") Then
                    connparts(i) = Piece(connparts(i).ToUpper, "=", 1) & "=" & Piece(connparts(i), "=", 2)
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("UID=") Then
                    connparts(i) = Piece(connparts(i).ToUpper, "=", 1) & "=" & Piece(connparts(i), "=", 2)
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("USERID=") Then
                    connparts(i) = connparts(i).Replace("USERID=", "User ID=").Replace("userid=", "User ID=").Replace("UserID=", "User ID=").Replace("UserId=", "User ID=")
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("PASSWORD=") Then
                    connparts(i) = connparts(i).Replace("PASSWORD=", "Password=").Replace("password=", "Password=")
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            For i = 0 To connparts.Length - 1
                If connparts(i).ToUpper.StartsWith("PWD=") Then
                    connparts(i) = Piece(connparts(i).ToUpper, "=", 1) & "=" & Piece(connparts(i), "=", 2)
                    userconnstr = userconnstr & "; " & connparts(i)
                End If
            Next
            Dim dbpass As String = String.Empty
            If Not Request("DBpass") Is Nothing AndAlso Request("DBpass").ToString.Trim <> "" Then
                dbpass = Request("DBpass").ToString.Trim
            End If
            If userconnstr.IndexOf(";Password=") < 0 AndAlso userconnstr.IndexOf(";PWD=") < 0 AndAlso dbpass <> "" Then
                userconnstr = userconnstr & ";Password=" & dbpass
            End If

            Session("UserConnString") = userconnstr

            'Session("UserDB") = userconnstr.Substring(0, userconnstr.IndexOf("User ID"))
            Session("logon") = cleanText(Request("logon"))
            Dim name As String = cleanText(Request("Name"))
            If name = "" Then name = Session("logon")
            WriteToAccessLog(Session("logon"), "Request to register", 0)
            'check if logon is available
            'remove password from userconnstr
            Dim userconnstrnopass As String = userconnstr
            If userconnstr.ToUpper.Contains(";PASSWORD") Then
                userconnstrnopass = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD")).Trim
            ElseIf userconnstr.ToUpper.Contains(";PWD") Then
                userconnstrnopass = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PWD")).Trim
            End If
            Dim hasrec As Boolean
            Dim webinstall As String = ConfigurationManager.AppSettings("webinstall").ToString
            Dim dbinstall As String = ConfigurationManager.AppSettings("dbinstall").ToString
            If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
                hasrec = HasRecords("SELECT * FROM [OURPERMITS] WHERE ([NetId]='" & Session("logon") & "') AND ([Application]='InteractiveReporting')")
            Else  'logon should be unique by each database, but can be the same for multiple databases in the unit.
                hasrec = HasRecords("SELECT * FROM [OURPERMITS] WHERE ([NetId]='" & Session("logon") & "') AND ([ConnStr] LIKE '" & userconnstrnopass & "%') AND ([Application]='InteractiveReporting')")
            End If

            If hasrec Then
                ret = "The logon is not available. Please select a different one."
                Exit Try
            End If
            Dim ndays As Integer = 20000
            If Not ConfigurationManager.AppSettings("DaysFree") Is Nothing AndAlso IsNumeric(ConfigurationManager.AppSettings("DaysFree").ToString) Then
                ndays = CInt(ConfigurationManager.AppSettings("DaysFree").ToString)
            End If
            'insert the registration record into OURPERMITS
            If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
                If Session("CSV") = "yes" OrElse Session("Unit") = "CSV" Then
                    'userconnprv = "MySql.Data.MySqlClient"
                    'Session("UserConnProvider") = "MySql.Data.MySqlClient"

                    userconnprv = Session("UserConnProvider")

                    ret = RegisterUser("user", "friendly", Session("logon"), cleanText(Request("Pass")), name, "CSV", "InteractiveReporting", "admin", "", "", "CSV", cleanText(Request("Email")), "user registration", userconnstrnopass.Trim, userconnprv.Trim, DateToString(CDate(Now())), DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, ndays, Now()))))
                Else
                    ret = RegisterUser("user", "friendly", Session("logon"), cleanText(Request("Pass")), name, cleanText(Session("Unit")), "InteractiveReporting", "admin", "", "", "", cleanText(Request("Email")), "user registration", userconnstrnopass.Trim, userconnprv.Trim, DateToString(CDate(Now())), DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, ndays, Now()))))
                End If
            Else
                ret = RegisterUser("user", "user", Session("logon"), cleanText(Request("Pass")), name, cleanText(Session("Unit")), "InteractiveReporting", "user", "", "", "", cleanText(Request("Email")), "site registration", userconnstrnopass.Trim, userconnprv.Trim, DateToString(CDate(Now())), DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, ndays, Now()))))
            End If
            Dim rtn As String = String.Empty
            If userconnstr.IndexOf(";Password=") < 0 AndAlso userconnstr.IndexOf(";PWD=") < 0 AndAlso dbpass = "" Then
                ret = Session("logon") & " has been registered. Initial reports and analytics were not required."
            Else
                If ret.StartsWith("User has been registered.") Then
                    If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then  'OUR distribution model 4: OURweb-OURdb.
                        ret = Session("logon") & " has been registered. You have " & ndays.ToString & " days of complimentary trial. To avoid the service interuption please proseed to payment."

                        'TODO message about long time for data analysis
                        If Session("CSV") Is Nothing OrElse Session("CSV").ToString <> "yes" Then
                            'Make Initial Reports
                            Dim er As String = String.Empty
                            Session("email") = cleanText(Request("Email"))
                            Session("UserEndDate") = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, ndays, Now())))
                            'If Session("UserConnProvider").ToString.Trim = "System.Data.OleDb" Then
                            '    Session("UserConnString") = ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;" & Session("UserConnString")
                            'Else
                            '    If Not Session("UserConnString").ToString.ToUpper.Contains("PASSWORD") AndAlso Not Session("UserConnString").ToString.ToUpper.Contains("PWD") Then
                            '        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString
                            '    End If
                            'End If

                            If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider"), er) Then
                                rtn = MakeInitialReports(Session("logon"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er, True)
                            End If
                            If er.Trim <> "" Then
                                ret = er
                                Exit Try
                            End If
                        End If
                    End If
                End If
            End If
            LblInvalid.Text = ret
            ret = WriteToAccessLog(Session("logon"), "Registration: " & ret & " " & rtn, 2)
            'free for individual user:
            Response.Redirect("Default.aspx")
            'Response.Redirect("Pay.aspx?stat=justreg&msg=" & ret)
        Catch ex As Exception
            ret = ex.Message
        End Try
        LblInvalid.Text = ret
        ret = WriteToAccessLog(Session("logon"), ret, 2)
    End Sub
    Protected Sub dropdownDatabases_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dropdownDatabases.SelectedIndexChanged
        btnBrowse.Visible = False
        btnBrowse.Enabled = False
        DBpass.Enabled = True
        DBpass.Visible = True
        If dropdownDatabases.SelectedItem.Text.StartsWith("Intersystems") Then
            ConnStr.Enabled = True
            ConnStr.Visible = True
            trprov.Visible = True
            trconnstr.Visible = True
            'lblConnection.Text = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser; Password = yourpassword"
            lblConnection.Text = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser;"
        ElseIf dropdownDatabases.SelectedItem.Text.StartsWith("Postgre") Then
            ConnStr.Enabled = True
            ConnStr.Visible = True
            trprov.Visible = True
            trconnstr.Visible = True
            'lblConnection.Text = "Server = yourserver; Port = yourport; Database=yourdatabase; User ID=youruser; Password=yourpassword"
            lblConnection.Text = "Server = yourserver; Port = yourport; Database=yourdatabase; User ID=youruser;"
        ElseIf dropdownDatabases.SelectedItem.Text = "ODBC" Then
            ConnStr.Enabled = True
            ConnStr.Visible = True
            trprov.Visible = True
            trconnstr.Visible = True
            trDBpass.Visible = False
            DBpass.Enabled = False
            'lblConnection.Text = "DSN=yourDSNname;UID=youruser;Password=yourpassword"
            lblConnection.Text = "DSN=yourDSNname;UID=youruser;"
        ElseIf dropdownDatabases.SelectedItem.Text = "OleDb" Then
            ConnStr.Enabled = True
            ConnStr.Visible = True
            trprov.Visible = True
            trconnstr.Visible = True
            lblConnection.Text = "Data Source=yourfilepath;"
            DBpass.Enabled = False
            trDBpass.Visible = False
            LabelDBpassword.Visible = False
        ElseIf dropdownDatabases.SelectedItem.Text = "Oracle" Then
            ConnStr.Enabled = True
            ConnStr.Visible = True
            trprov.Visible = True
            trconnstr.Visible = True
            'lblConnection.Text = "data source=yourserver:yourport/yourdatabase;user id=youruser;password=yourpassword"
            lblConnection.Text = "data source=yourserver:yourport/yourdatabase;user id=youruser;"
        ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso dropdownDatabases.SelectedItem.Text.ToUpper = "USE OUR DATABASE" Then
            ConnStr.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
            ConnStr.Enabled = False
            ConnStr.Visible = False
            trprov.Visible = False
            trconnstr.Visible = False
            Session("CSV") = "yes"
            lblConnection.Visible = False
            lblPassWord.Visible = False
            lblConnection.Text = ""
            lblPassWord.Text = ""
            DBpass.Enabled = False
            trDBpass.Visible = False
            Label2.Text = "Your data (CSV, XML, JSON, XLS, XLSX, Access table) can be uploaded into database on our server. Reports and analytics will be available only to you and to users you share them with."
        Else
            ConnStr.Enabled = True
            ConnStr.Visible = True
            trprov.Visible = True
            trconnstr.Visible = True
            'lblConnection.Text = "Server=yourserver; Database=yourdatabase; User ID=youruser; Password=yourpassword"
            lblConnection.Text = "Server=yourserver; Database=yourdatabase; User ID=youruser;"
        End If
        ConnStr.Text = lblConnection.Text
    End Sub
    Private Sub chkTermsAndCond_CheckedChanged(sender As Object, e As EventArgs) Handles chkTermsAndCond.CheckedChanged
        If chkTermsAndCond.Checked Then
            btRegister.Enabled = True
            btRegister.Visible = True
        Else
            btRegister.Enabled = False
            btRegister.Visible = False
        End If
    End Sub


End Class
