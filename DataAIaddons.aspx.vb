Imports System.Data
Imports System.IO
Imports System.Text
Imports System.Net
Imports System.Xml
Imports AjaxControlToolkit.HtmlEditor.ToolbarButtons
Imports Mysqlx.XDevAPI.Relational
Imports Newtonsoft.Json
Imports OracleInternal.Secure
Imports Org.BouncyCastle.Ocsp
Imports PdfSharp.Drawing
Imports Microsoft.Data.Sqlite
Imports System.Windows.Forms
Imports AjaxControlToolkit
Imports Mysqlx
Imports System.Drawing
Imports Newtonsoft.Json.Linq
Imports System.IO.Compression
Partial Class DataAIaddons
    Inherits System.Web.UI.Page

    Private Sub DataAIaddons_Init(sender As Object, e As EventArgs) Handles Me.Init
        'LblInvalid.Text = "Your data (CSV, XML, JSON, XLS, XLSX, Access table) can be uploaded into our database. Reports and analytics will be available only to you and to users you share them with."
        LblInvalid.Text = "Your data in JSON, XML, CSV, XLS format will be analyzed in memory and will not be kept after Log Off is clicked."
        'analyzed and if you wish they can be uploaded into our database for your future use if checkbox above are checked. Reports and analytics will be available only to you."

        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If

        If dropdownDatabases.Text = "System.Data.SqlClient" Then
            ConnStr.ToolTip = "Server=yourserver; Database=yourdatabase; User ID=youruser;"
            ConnStr.Text = "Server=yourserver; Database=yourdatabase; User ID=youruser;"
        End If

        Dim i As Integer

        If Request.QueryString("txtConn") IsNot Nothing AndAlso Request.QueryString("txtConn").ToString.Trim <> "" Then
            Session("txtConn") = Request.QueryString("txtConn").ToString.Trim
        Else
            Session("txtConn") = ""
        End If


        For i = 0 To dropdownDatabases.Items.Count - 1
            If dropdownDatabases.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            If dropdownDatabases.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            If dropdownDatabases.Items(i).Text.ToUpper.Contains("IRIS") AndAlso ConfigurationManager.AppSettings("IRISProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "POSTGRESQL" AndAlso ConfigurationManager.AppSettings("Npgsql").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            'If dropdownDatabases.Items(i).Text.ToUpper = "USE OUR DATABASE" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
            '    dropdownDatabases.Items(i).Enabled = False
            '    dropdownDatabases.Items(i).Selected = False
            'ElseIf selprov = "" AndAlso dropdownDatabases.Items(i).Text.ToUpper = "USE OUR DATABASE" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
            '    dropdownDatabases.Text = dropdownDatabases.Items(i).Text
            '    dropdownDatabases.Items(i).Selected = True
            '    selprov = "CSV"
            '    Session("CSV") = "yes"
            'End If

            If dropdownDatabases.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "ODBC" AndAlso ConfigurationManager.AppSettings("ODBC").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If

            If dropdownDatabases.Items(i).Text.ToUpper = "OLEDB" AndAlso ConfigurationManager.AppSettings("OleDb").ToString <> "OK" Then
                dropdownDatabases.Items(i).Enabled = False
                dropdownDatabases.Items(i).Selected = False

            End If
        Next

        Dim apiKey As String = String.Empty
        Dim OpenAIOrganization As String = String.Empty
        Dim apiUrl As String = String.Empty
        Dim model As String = String.Empty
        Dim maxTokens As Integer = 0
        Try
            apiKey = ConfigurationManager.AppSettings("openaikey").ToString.Trim
        Catch ex As Exception
            apiKey = ""
        End Try
        Try
            OpenAIOrganization = ConfigurationManager.AppSettings("openaiorganization").ToString.Trim
        Catch ex As Exception
            OpenAIOrganization = ""
        End Try
        Try
            apiUrl = ConfigurationManager.AppSettings("apiURL").ToString.Trim
        Catch ex As Exception
            apiUrl = "https://api.openai.com/v1/chat/completions"
        End Try
        Try
            model = ConfigurationManager.AppSettings("openaimodel").ToString.Trim
        Catch ex As Exception
            model = "gpt-4o-mini"
        End Try
        Try
            maxTokens = CInt(ConfigurationManager.AppSettings("openaimaxTokens").ToString.Trim)
        Catch ex As Exception
            maxTokens = 128000
        End Try

        If apiKey = "" AndAlso Request("txtOpenAIkey") IsNot Nothing AndAlso Request("txtOpenAIkey").ToString.Trim <> "" Then
            apiKey = Request("txtOpenAIkey").ToString.Trim
            Session("OpenAIkey") = apiKey
        End If
        If apiKey = "" AndAlso Session("OpenAIkey") IsNot Nothing AndAlso Session("OpenAIkey").ToString.Trim <> "" Then
            apiKey = Session("OpenAIkey").ToString.Trim
        End If

        If OpenAIOrganization = "" AndAlso Request("txtOpenAIorgcode") IsNot Nothing AndAlso Request("txtOpenAIorgcode").ToString.Trim <> "" Then
            OpenAIOrganization = Request("txtOpenAIorgcode").ToString.Trim
            Session("OpenAIOrganization") = Request("txtOpenAIorgcode")
        End If
        If OpenAIOrganization = "" AndAlso Session("OpenAIOrganization") IsNot Nothing AndAlso Session("OpenAIOrganization").ToString.Trim <> "" Then
            OpenAIOrganization = Session("OpenAIOrganization").ToString.Trim
        End If

        If Request.QueryString("txtOpenAIurl") IsNot Nothing AndAlso Request.QueryString("txtOpenAIurl").ToString.Trim <> "" Then
            apiUrl = Request.QueryString("txtOpenAIurl").ToString.Trim
        ElseIf Session("OpenAIurl") IsNot Nothing AndAlso Session("OpenAIurl").ToString.Trim <> "" Then
            apiUrl = Session("OpenAIurl").ToString.Trim
        End If
        Session("OpenAIurl") = apiUrl

        If Request("txtOpenAImodel") IsNot Nothing AndAlso Request("txtOpenAImodel").ToString.Trim <> "" Then
            model = Request("txtOpenAImodel").ToString.Trim
        ElseIf Session("OpenAImodel") IsNot Nothing AndAlso Session("OpenAImodel").ToString.Trim <> "" Then
            model = Session("OpenAImodel").ToString.Trim
        End If
        Session("OpenAImodel") = model

        If Request("txtOpenAImaxtokens") IsNot Nothing AndAlso Request("txtOpenAImaxtokens").ToString.Trim <> "" AndAlso IsNumeric(Request("txtOpenAImaxtokens").ToString.Trim) Then
            maxTokens = CInt(Request("txtOpenAImaxtokens").ToString.Trim)
        ElseIf Session("maxTokens") IsNot Nothing AndAlso Session("maxTokens").ToString.Trim <> "" AndAlso IsNumeric(Session("maxTokens").ToString.Trim) Then
            maxTokens = CInt(Session("maxTokens").ToString.Trim)
        End If
        Session("maxTokens") = maxTokens

        txtOpenAIkey.Text = apiKey
        txtOpenAIorgcode.Text = OpenAIOrganization
        txtOpenAIurl.Text = apiUrl
        txtOpenAImodel.Text = model
        txtOpenAImaxtokens.Text = maxTokens.ToString
        If Request("frm") IsNot Nothing AndAlso Request("frm").ToString.Trim = "DataAIsqlite" Then
            trOpenAI.Visible = False
            Label1.Visible = False
        ElseIf apiKey.Trim = "" Then
            trOpenAI.Visible = True
            Label1.Visible = True
        Else
            trOpenAI.Visible = False
            Label1.Visible = False
        End If

        Dim useremail As String = String.Empty
        If Request("txtEmail") IsNot Nothing AndAlso Request("txtEmail").ToString.Trim <> "" Then
            useremail = cleanText(Request("txtEmail").ToString.Trim)
            useremail = Regex.Replace(useremail, "[^a-zA-Z0-9]", "")
            Session("Email") = useremail
        End If
        If useremail = "" AndAlso Session("Email") IsNot Nothing AndAlso Session("Email").ToString.Trim <> "" Then
            useremail = Session("Email").ToString.Trim
        End If
        If useremail = "" Then
            useremail = "time" & Now.ToString.Replace("/", "").Replace(":", "").Replace(" ", "")
            Session("Email") = useremail
        End If
        txtEmail.Text = useremail
        If useremail.Trim = "" Then
            txtEmail.Visible = True
            Label2.Visible = True
        Else
            txtEmail.Visible = False
            Label2.Visible = False
        End If

        If Request.QueryString("txtURI") IsNot Nothing AndAlso Request.QueryString("txtURI").ToString.Trim <> "" Then
            Session("txtURI") = Request.QueryString("txtURI").ToString.Trim
        End If
        If Session("txtURI") IsNot Nothing AndAlso Session("txtURI").ToString.Trim <> "" Then    'web file AndAlso Session("txtURI").ToString.Trim <> "https://" AndAlso Session("txtURI").Text.Trim <> "http://"
            txtURI.Text = Session("txtURI").ToString.Trim
        End If

        'uploads folder
        If applpath Is Nothing Then applpath = ""
        If applpath = "\" Then applpath = ""
        If applpath = "" AndAlso ConfigurationManager.AppSettings("fileupload") IsNot Nothing AndAlso ConfigurationManager.AppSettings("fileupload").ToString.Trim <> "" Then
            applpath = ConfigurationManager.AppSettings("fileupload").ToString.Trim
        End If
        If applpath = "" AndAlso Request("txtUploads") IsNot Nothing AndAlso Request("txtUploads").ToString.Trim <> "" Then
            applpath = Request("txtUploads").ToString.Trim
        End If
        If applpath = "" Then
            applpath = System.AppDomain.CurrentDomain.BaseDirectory()
        End If

        txtUploads.Text = applpath
        If applpath.Trim = "" Then
            txtUploads.Visible = True
            Label3.Visible = True
        Else
            txtUploads.Visible = False
            Label3.Visible = False
        End If

        If Session("txtConn") IsNot Nothing AndAlso Session("txtConn").ToString.Trim <> "" AndAlso Not Session("txtConn").ToString.Contains("youruser") AndAlso Not Session("txtConn").ToString.Contains("yourfilepath") Then
            'assign fields
            Dim conndb As String = Session("txtConn").ToString.Trim
            DBpass.Text = GetPasswordFromConnectionString(conndb)
            Dim provname As String = GetProviderNameFromConnString(conndb)
            Dim b As Boolean = False
            For i = 0 To dropdownDatabases.Items.Count - 1
                If dropdownDatabases.Items(i).Value.ToUpper = provname.ToUpper Then
                    dropdownDatabases.Items(i).Selected = True
                    dropdownDatabases.Text = provname
                    Session("UserConnProvider") = provname
                    b = True
                    Exit For
                Else
                    dropdownDatabases.Items(i).Selected = False
                End If
            Next
            If Not b Then
                MessageBox.Show("Connection string provider is not supported: " & provname, "Connect to user database", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                LblInvalid.Text = "Connection to user database failed!"
                Exit Sub
            End If
            i = conndb.ToUpper.IndexOf("PROVIDERNAME")
            ConnStr.Text = conndb.Substring(0, i).Trim
            Session("UserConnString") = ConnStr.Text
            btStart_Click(sender, e)

        End If

        btStart.OnClientClick = "showSpinner();"

        btnBrowseAddOns.OnClientClick = "clickFileAddOnsUpload();return false;"
        FileAddOns.Attributes.Add("onchange", "getAttachedFileAddOns();")
        'FOR TESTING ONLY !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        If Session("addon") Is Nothing Then Session("addon") = "PowerBI"
    End Sub
    Private Sub DataAIaddons_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Request("txtEmail") IsNot Nothing AndAlso Request("txtEmail").ToString.Trim <> "" Then
            txtEmail.Text = cleanText(Request("txtEmail").ToString.Trim)
        End If


        hfSizeLimit.Value = getMaxRequestLength().ToString

        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target = "FileSizeExceeded" Then
                MessageBox.Show(data, "File Size Exceeded", target, Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
                Exit Sub
            End If
        End If

        If txtURI.Text.Trim <> "" AndAlso txtURI.Text.Trim <> "http://" AndAlso txtURI.Text.Trim <> "https://" AndAlso txtEmail.Text.Trim <> "" Then
            btStart_Click(sender, e)
        End If

    End Sub
    Protected Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click

        hfSizeLimit.Value = getMaxRequestLength().ToString

        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target = "FileSizeExceeded" Then
                MessageBox.Show(data, "File Size Exceeded", target, Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
                Exit Sub
            End If
        End If

        'If applpath = "" Then
        '    applpath = System.AppDomain.CurrentDomain.BaseDirectory()
        'End If

        LblInvalid.Text = ""
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim fileext As String = String.Empty
        Session("logon") = cleanText(txtEmail.Text.Trim)
        If Session("logon") <> txtEmail.Text.Trim Then
            ret = "Illegal character found. Please retype Email/Logon."
            LblInvalid.Text = ret
            Exit Sub
        ElseIf Session("logon") = "" Then
            ret = "Email/Logon cannot be empty."
            LblInvalid.Text = ret
            MessageBox.Show("Email/Logon cannot be empty.", "Email/Logon/User ID", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If
        Session("logon") = Session("logon").Replace("@", "").Replace(".com", "").Replace(".", "")

        Session("OurConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString
        Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString()
        Session("CSV") = "yes"
        Session("Unit") = "CSV"


        'Session("UserConnString")
        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
        If Request("addon") IsNot Nothing AndAlso Request("addon").ToString.Trim <> "" Then
            Session("addon") = Request("addon")
            Session("UserConnProvider") = "Sqlite"
            Session("UserConnString") = "Sqlite"
        End If
        'no youruser and nor yourfilepath - UserConnString assigned
        If DBpass.Text.Trim <> "" AndAlso Not ConnStr.Text.Contains("youruser") AndAlso Not ConnStr.Text.Contains("yourfilepath") Then
            Session("UserConnProvider") = dropdownDatabases.SelectedItem.Value
            Session("UserConnString") = ConnStr.Text.Trim
            If Not ConnStr.Text.ToUpper.Contains("PASSWORD") Then
                Session("UserConnString") = ConnStr.Text.Trim & "; Password=" & DBpass.Text.Trim
            End If
            Session("UserConnString") = Session("UserConnString").ToString.Replace(";;", ";").Replace("""", ";")
            'TODO maybe CSVdb should be ourdb and not userdb?
            Session("CSVConnProvider") = Session("UserConnProvider")
            Session("CSVConnString") = Session("UserConnString")
        End If



        If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" AndAlso Session("UserConnProvider") <> "Sqlite" Then
            If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
            If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        ElseIf Session("UserConnProvider") = "Sqlite" Then
            Session("UserDB") = "Sqlite"
        Else
            If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
        End If

        Session("dbname") = GetDataBase(Session("UserConnString"), Session("UserConnProvider")).Replace("#", "")
        Session("admin") = "admin"
        Session("Application") = "InteractiveReporting"
        Session("AdvancedUser") = True
        Session("email") = cleanText(txtEmail.Text.Trim)
        Session("REPORTID") = Session("logon").Replace("@", "").Replace(".", "")
        repid = Session("REPORTID")
        Session("REPTITLE") = repid
        'If repid.Length > 10 Then
        '    repid = repid.Substring(0, 9)
        'End If
        repid = repid & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
        Session("REPORTID") = repid
        Dim url As String = HttpContext.Current.Request.Url.AbsoluteUri
        Session("URL") = url.Substring(0, url.LastIndexOf("/")) & "/"
        Session("UnitWEB") = ""


        Dim userconnstrnopass As String = Session("UserConnString")
        If userconnstrnopass.ToUpper.IndexOf("PASSWORD") > 0 Then
            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("PASSWORD"))
        ElseIf userconnstrnopass.ToUpper.IndexOf("PWD") > 0 Then
            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("PWD")).Trim
        End If

        'In MEMORY !!!!!!!!!!!!!!!!!!!!!!!
        sqliteconn = New SqliteConnection("Data Source=:memory:")
        sqliteconn.Open()
        'Dim pragmaCmd As New SqliteCommand("PRAGMA busy_timeout = 30000000;", sqliteconn)
        'pragmaCmd.ExecuteNonQuery()

        'create ourdata database
        er = UpdateOURdbToCurrentVersion("Sqlite", "Sqlite", False)
        If er.Contains("ERROR!!") Then
            MessageBox.Show("Creation of the operational database with connection string from the web.config crashed! Create the empty operational database with connection string from the web.config manually and try running this page again.", "The operational database is not created - Error", "NoDatabaseError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Exit Sub
        End If

        'user registration
        Dim ndays As Integer = 2000
        If Not ConfigurationManager.AppSettings("DaysFree") Is Nothing AndAlso IsNumeric(ConfigurationManager.AppSettings("DaysFree").ToString) Then
            ndays = CInt(ConfigurationManager.AppSettings("DaysFree").ToString)
        End If
        Session("UserEndDate") = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, ndays, Now())))
        ret = RegisterUser("user", "friendly", Session("logon"), Session("logon"), Session("logon"), "CSV", "InteractiveReporting", "admin", "", "", "CSV", Session("logon"), "quick registration", userconnstrnopass.Trim, Session("UserConnProvider").Trim, DateToString(CDate(Now())), Session("UserEndDate"))

        If Session("UserConnString").ToString.Trim <> "Sqlite" Then

            Response.Redirect("ListOfReports.aspx?addon=yes&savedata=no")

        End If

        'GET DATA in format: XML or JSON or CSV or XLS --------------------------------------------------------------------------------------
        Dim filepath As String = String.Empty
        Dim jsontext As String = String.Empty
        Dim strm As Stream = Nothing
        Dim client = New WebClient
        If (txtURI.Text.Trim = "" OrElse txtURI.Text.Trim = "https://" OrElse txtURI.Text.Trim = "http://") AndAlso Not FileAddOns.HasFiles AndAlso Session("UserConnString") = "Sqlite" Then  'AndAlso Request.InputStream Is Nothing
            btnBrowseAddOns.Focus()
            MessageBox.Show("File is not selected.", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            LblInvalid.Text = "File is not selected!"
            Exit Sub
        End If
        If Request("FileType") IsNot Nothing AndAlso ",.CSV,.XML,.JSON,.XLS".IndexOf(Request("FileType").ToString.Trim.ToUpper) > 0 AndAlso Request.InputStream IsNot Nothing AndAlso Session("UserConnString") = "Sqlite" Then
            'input stream from PowerBI or Tableau if needed in future  
            fileext = Request("FileType")
            Dim reader As New System.IO.StreamReader(Request.InputStream)
            jsontext = reader.ReadToEnd()

        ElseIf txtURI.Text.Trim <> "" AndAlso txtURI.Text.Trim <> "https://" AndAlso txtURI.Text.Trim <> "http://" AndAlso Session("UserConnString") = "Sqlite" Then    'web file
            'web file
            If txtURI.Text.Trim.LastIndexOf(".") < 0 Then
                LblInvalid.Text = "ERROR!! The URL format is not valid. Please enter a valid URL."
                txtURI.BorderColor = Color.Red
                Exit Sub
            End If
            fileext = txtURI.Text.Trim.Substring(txtURI.Text.Trim.LastIndexOf(".")).Replace(".", "")
            If Not Uri.IsWellFormedUriString(txtURI.Text, UriKind.Absolute) OrElse ",.CSV,.XML,.JSON,.XLS".IndexOf(fileext.ToUpper) < 0 Then
                LblInvalid.Text = "ERROR!! The URL format is not valid. Please enter a valid URL with .CSV,.XML,.JSON,.XLS format."
                txtURI.BorderColor = Color.Red
                Exit Sub
            End If

            jsontext = client.DownloadString(txtURI.Text)

            If fileext.ToUpper = "CSV" Then
                'convert string jsontext into stream strm
                Dim byteArray As Byte() = Encoding.UTF8.GetBytes(jsontext)
                strm = New MemoryStream(byteArray)

            ElseIf fileext.ToUpper.StartsWith("XLS") Then
                'save jsontext into file filepath
                filepath = applpath & "Temp\" & Session("logon").Replace("@", "").Replace(".", "") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "").Replace(".", "") & "." & fileext
                File.WriteAllText(filepath, jsontext)

            Else 'JSON, XML
                Dim reader As New System.IO.StreamReader(Request.InputStream)
                jsontext = reader.ReadToEnd()
            End If

        ElseIf FileAddOns.HasFiles AndAlso Session("UserConnString") = "Sqlite" Then
            'local file
            If Not FileAddOns.HasFiles OrElse Not FileAddOns.HasFile OrElse FileAddOns.PostedFiles.Count = 0 Then 'local file
                btnBrowseAddOns.Focus()
                MessageBox.Show("No file selected", "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                LblInvalid.Text = "File is not selected!"
                Exit Sub
            End If

            fileext = FileAddOns.PostedFile.FileName.Trim.Substring(FileAddOns.PostedFile.FileName.Trim.LastIndexOf(".")).Replace(".", "")

            If fileext.ToUpper = "CSV" Then
                'get postedfile stream
                strm = FileAddOns.PostedFile.InputStream

            ElseIf fileext.ToUpper.StartsWith("XLS") Then
                'save PostedFile in file filepath or get path to posted file
                filepath = applpath & "Temp\" & Session("logon").Replace("@", "").Replace(".", "") & FileAddOns.PostedFile.FileName.Replace(".", "") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "").Replace(".", "") & "." & fileext
                FileAddOns.PostedFile.SaveAs(filepath)

            Else  'JSON, XML
                Dim reader As New System.IO.StreamReader(FileAddOns.PostedFile.InputStream)
                jsontext = reader.ReadToEnd()
            End If

        End If


        Dim dv3 As DataView
        Dim dts As DataTable

        'CSV
        If fileext.ToUpper = "CSV" Then
            Try
                ret = ImportCSVintoDbTableFromStream(Session("logon"), repid, strm, ",", Session("UserConnString"), Session("UserConnProvider"), False)

                'add table tbl into list for OURUserTables
                ret = InsertTableIntoOURUserTables(repid, repid, Session("Unit"), Session("logon"), Session("UserDB"), "", repid)

                'make columns names CLS-compliant
                dv3 = mRecords("SELECT * FROM " & repid, er)
                dts = MakeDTColumnsNamesCLScompliant(dv3.Table, Session("UserConnProvider"), er)
                dv3 = dts.DefaultView
                Session("dv3") = dv3

                ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), repid, Session("UserDB"), "SELECT * FROM " & repid, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)


                Response.Redirect("Analytics.aspx")
            Catch ex As Exception
                ret = "ERROR!! " & ex.Message
                MessageBox.Show("Import of CSV file crashed. " & ret, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End Try
        ElseIf fileext.ToUpper.StartsWith("XLS") Then
            Try
                ret = ImportExcelIntoDbTable(repid, filepath, Session("UserConnString"), Session("UserConnProvider"), False)

                'add table tbl into list for OURUserTables
                ret = InsertTableIntoOURUserTables(repid, repid, Session("Unit"), Session("logon"), Session("UserDB"), "", repid)

                'make columns names CLS-compliant
                dv3 = mRecords("SELECT * FROM " & repid, er)
                dts = MakeDTColumnsNamesCLScompliant(dv3.Table, Session("UserConnProvider"), er)
                dv3 = dts.DefaultView
                Session("dv3") = dv3

                ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), repid, Session("UserDB"), "SELECT * FROM " & repid, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)


                Response.Redirect("Analytics.aspx")
            Catch ex As Exception
                ret = "ERROR!! " & ex.Message
                MessageBox.Show("Import of Excel file crashed. " & ret, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End Try
        End If

        'If Session("addon") = "PowerBI" Then
        '    'Download File from uri
        '    'save File in strFile
        '    Dim strFile As String = "C:\Uploads\" & repid
        '    client.DownloadFile(txtURI.Text, strFile)
        '    jsontext = File.ReadAllText(strFile)
        'End If

        'If Session("addon") = "Tableau" Then
        '    'DownloadString from common area with REST API
        '    client.Headers.Add("X-Tableau-Auth", "YOUR_SESSION_TOKEN")
        '    jsontext = client.DownloadString("https://YOUR-SERVER/api/3.12/sites/site-id/views/view-id/data")
        'End If

        'JSON, XML
        dv3 = RetrieveDataFromJsonXML(repid, jsontext, True, "Sqlite", "Sqlite", er, fileext)

        If dv3 Is Nothing Then
            MessageBox.Show("Import of JSON or XML file crashed. " & ret, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If

        'make columns names CLS-compliant
        dts = MakeDTColumnsNamesCLScompliant(dv3.Table, Session("UserConnProvider"), er)
        dv3 = dts.DefaultView
        Session("dv3") = dv3

        Response.Redirect("Analytics.aspx?addon=yes&savedata=no")


    End Sub
    Protected Sub ButtonVideo_Click(sender As Object, e As EventArgs) Handles ButtonVideo.Click
        Response.Redirect("Videos/DataAIaddons.mp4")
    End Sub
    Public Function RetrieveDataFromJsonXML(ByVal repid As String, ByRef datastr As String, ByVal onetable As Boolean, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef er As String = "", Optional ByVal fileext As String = "") As DataView
        Dim dt As New DataTable
        Dim ret As String = String.Empty
        Dim n As Integer = 0
        Dim m As Integer = 0
        Dim doc As New XmlDocument
        Dim node As XmlNode = Nothing
        Dim Xdoc As XDocument
        Dim tblname As String = String.Empty
        Dim xmltext As String = String.Empty
        Dim i, j As Integer


        Dim cmd As New SqliteCommand("PRAGMA foreign_keys = ON;", sqliteconn)
        cmd.ExecuteNonQuery()

        If fileext.ToUpper = "XML" OrElse fileext.ToUpper = "XMLS" Then
            'xml
            doc.Load(datastr)
        Else
            'json
            Try
                Xdoc = JsonConvert.DeserializeXNode(datastr, "root", True)
                'Xdoc = JsonConvert.DeserializeXNode(datastr, "")
            Catch ex As Exception
                Dim parsedJson As JToken = JToken.Parse(datastr)
                Dim safeJson As JToken = SanitizeKeys(parsedJson)

                ' Convert sanitized JSON to XML
                Xdoc = JsonConvert.DeserializeXNode(safeJson.ToString(), "root", True)

            End Try
            doc.LoadXml(Xdoc.ToString)
        End If

        Try
            'create db 
            Dim sqlq As String = String.Empty
            Dim db As String = GetDataBase(userconstr, userconprv).ToLower  'user db

            'find first node with ChildNodes
            Dim firsttblname As String = String.Empty
            For i = 0 To doc.ChildNodes.Count - 1
                node = doc.ChildNodes(i)
                If node.HasChildNodes Then
                    If node.Name.Trim <> "" Then
                        firsttblname = node.Name
                    Else
                        firsttblname = tblname  'rep

                    End If
                    Exit For
                End If
            Next

            Dim task As Threading.Tasks.Task(Of String) = ProcessXMLJSONNodeIntoTable(repid, m, tblname, node, firsttblname, tblname, 0, userconstr, userconprv, Session("Unit").ToString, Session("logon").ToString, Session("UserDB").ToString, er, False)

            ret = "input done"

            'Register tables and make reports
            Dim tbs As String = String.Empty
            'make loop to register the tables tblname(i) in OURUserTables of OURdb using main report that have all joins of subtables
            Dim dv As DataView = GetReportTablesFromOURUserTables(repid, Session("Unit"), Session("logon"), Session("UserDB"), ret)
            If dv Is Nothing OrElse dv.Table Is Nothing Then
                Label1.Text = "There are no tables found for this report."
                Return Nothing
            End If
            Dim ntables As Integer = dv.Table.Rows.Count
            Dim repsqlquery As String = String.Empty
            Dim rtbl As String = String.Empty
            'make loop to make initial reports for the tables in OURUserTables of OURdb
            For i = 0 To ntables - 1
                j = j + 1
                rtbl = FixReservedWords(dv.Table.Rows(i)("TableName").ToString.Trim, Session("UserConnProvider"))
                'correct fields types
                ret = CorrectFieldsInTable(dv.Table.Rows(i)("TableName"), Session("UserConnString"), Session("UserConnProvider"), True)

                If ret.Contains("ERROR!!") AndAlso ret.ToUpper.Contains(("Table '" & dv.Table.Rows(i)("TableName").ToUpper & "' not found").ToString.ToUpper) Then
                    'ret = ExequteSQLquery("DELETE FROM OURUserTables WHERE  [Unit]='" & Session("Unit") & "' AND [UserID]='" & Session("logon") & "' AND [UserDB]='" & Session("UserDB") & "' AND [Prop2]='" & repid & "' And TableName='" & dv.Table.Rows(i)("TableName") & "'", Session("UserConnString"), Session("UserConnProvider"))
                    Continue For
                End If

                ' Make new reports for the table !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                repsqlquery = "SELECT * FROM " & rtbl
                repid = Session("REPORTID") & i.ToString
                ret = MakeNewUserReport(Session("logon"), repid, dv.Table.Rows(i)("TableName"), Session("UserDB"), repsqlquery, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
            Next

            'Make dv3 joining all tables: "
            Dim msql As String = "SELECT * FROM OURReportSQLquery WHERE (Doing='JOIN' AND ReportID='" & Session("REPORTID") & "' )  ORDER BY Tbl1, Tbl2, Tbl1Fld1, Tbl2Fld2"
            Dim dvj As DataView = mRecords(msql, er)  'all possible joins
            If dvj Is Nothing OrElse dvj.Table Is Nothing OrElse dvj.Table.Rows.Count = 0 Then
                msql = "SELECT * FROM " & dv.Table.Rows(0)("TableName")
                dv = mRecords(msql, er)
                Return dv
            End If
            ret = ret & " Make initial reports with Joins: " & MakeInitialReportsWithJoins(Session("logon"), Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
            dt = MakeTableWithAllJoins(Session("REPORTID"))
            tblname = Session("REPORTID")
            ret = ImportDataTableIntoDb(dt, tblname, "Sqlite", "Sqlite", er)
            repsqlquery = "SELECT * FROM " & tblname
            ret = MakeNewUserReport(Session("logon"), Session("REPORTID"), "All Joined Data in " & tblname, Session("UserDB"), repsqlquery, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
            sqlq = "UPDATE OURReportInfo SET SQLquerytext='" & repsqlquery & "', Param8type='USESQLTEXT' WHERE ReportId ='" & Session("REPORTID") & "'  And ReportDB Like '%" & Session("UserDB") & "%'"
            ret = ExequteSQLquery(sqlq)

            Return dt.DefaultView
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        doc = Nothing

        Return dt.DefaultView
    End Function

    Public Function ProcessXMLJSONNodeIntoDataTable(ByRef dt As DataTable, ByVal repid As String, ByVal m As Integer, ByVal tblname As String, ByVal node As XmlNode, ByVal tbl As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional er As String = "") As String
        'recursive
        Dim ret As String = String.Empty
        Dim str As String = String.Empty
        Dim fixedstr As String = String.Empty
        'Dim sqliteconn As New SqliteConnection("Data Source=:memory:")
        'sqliteconn.Open()
        Try

            'add attributes as fields in dt and insert their values 
            If node.Attributes IsNot Nothing Then
                For i = 0 To node.Attributes.Count - 1
                    str = node.Attributes(i).Name.Replace(":", "").Replace("#", "") & m.ToString
                    fixedstr = FixReservedWords(str, userconnprv)
                    If ColumnExistsInDataTable(dt, fixedstr) Then Continue For
                    Dim col As New DataColumn
                    col.DataType = System.Type.GetType("System.String")
                    col.ColumnName = fixedstr
                    dt.Columns.Add(col)
                Next
            End If
            'add childnodes names as fields 
            If node.ChildNodes IsNot Nothing Then
                For i = 0 To node.ChildNodes.Count - 1
                    str = node.ChildNodes(i).Name.Replace(":", "").Replace("#", "") & m.ToString
                    fixedstr = FixReservedWords(str, userconnprv)
                    If ColumnExistsInDataTable(dt, fixedstr) Then Continue For
                    Dim col As New DataColumn
                    col.DataType = System.Type.GetType("System.String")
                    col.ColumnName = fixedstr
                    dt.Columns.Add(col)
                    ret = ProcessXMLJSONNodeIntoDataTable(dt, repid, m + 1, tblname, node.ChildNodes(i), tblname, "", "", er)
                Next
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ProcessXMLJSONNodeValuesIntoDataTableRow(ByRef dt As DataTable, ByRef dr As DataRow, ByVal repid As String, ByVal m As Integer, ByVal tblname As String, ByVal node As XmlNode, ByVal tbl As String, ByVal userconnstr As String, ByVal userconnprv As String, Optional er As String = "") As String
        'recursive
        Dim ret As String = String.Empty
        Dim str As String = String.Empty
        Dim fixedstr As String = String.Empty
        Try

            'insert attributes values 
            If node.Attributes IsNot Nothing Then
                For i = 0 To node.Attributes.Count - 1
                    str = node.Attributes(i).Name.Replace(":", "").Replace("#", "") & m.ToString
                    fixedstr = FixReservedWords(str, userconnprv)
                    If Not ColumnExistsInDataRow(dr, fixedstr) Then Continue For
                    dr(fixedstr) = node.Attributes(i).Value.ToString
                Next
            End If
            'insert childnodes values
            If node.ChildNodes IsNot Nothing Then
                For i = 0 To node.ChildNodes.Count - 1
                    str = node.ChildNodes(i).Name.Replace(":", "").Replace("#", "") & m.ToString
                    fixedstr = FixReservedWords(str, userconnprv)
                    If Not ColumnExistsInDataRow(dr, fixedstr) Then Continue For
                    If i = 0 Then
                        If node.ChildNodes(i).Value IsNot Nothing Then dr(fixedstr) = node.ChildNodes(i).Value.ToString
                        ret = ProcessXMLJSONNodeValuesIntoDataTableRow(dt, dr, repid, m + 1, tblname, node.ChildNodes(i), tblname, "", "", er)
                    ElseIf i > 0 Then
                        dt.Rows.Add(dr)
                        Dim drt As DataRow
                        drt = dt.NewRow
                        drt = dr
                        If node.ChildNodes(i).Value IsNot Nothing Then dr(fixedstr) = node.ChildNodes(i).Value.ToString
                        ret = ProcessXMLJSONNodeValuesIntoDataTableRow(dt, drt, repid, m + 1, tblname, node.ChildNodes(i), tblname, "", "", er)
                    End If


                Next
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function ColumnExistsInDataTable(ByRef dt As DataTable, ByVal fixedstr As String) As Boolean
        Dim colexists As Boolean = False
        Dim i As Integer = 0
        For i = 0 To dt.Columns.Count - 1
            If dt.Columns(i).ColumnName.ToUpper.Trim = fixedstr.ToUpper.Trim Then
                colexists = True
                Exit For
            End If
        Next
        Return colexists
    End Function
    Public Function ColumnExistsInDataRow(ByVal dr As DataRow, ByVal fixedstr As String) As Boolean
        Dim colexists As Boolean = False
        Dim i As Integer = 0
        For i = 0 To dr.Table.Columns.Count - 1
            If dr.Table.Columns(i).ColumnName.ToUpper.Trim = fixedstr.ToUpper.Trim Then
                colexists = True
                Exit For
            End If
        Next
        Return colexists
    End Function

    Function SanitizeKeys(obj As JToken) As JToken
        ' Recursively rename all properties to make them XML-safe
        If TypeOf obj Is JObject Then
            Dim newObj As New JObject()
            For Each prop As JProperty In obj.Children(Of JProperty)()
                ' Replace invalid characters (e.g., space) with underscore
                Dim safeName = prop.Name.Replace(" ", "_").Replace("-", "_")
                If Char.IsDigit(safeName(0)) Then
                    safeName = "_" & safeName
                End If

                newObj(safeName) = SanitizeKeys(prop.Value)
            Next
            Return newObj
        ElseIf TypeOf obj Is JArray Then
            Dim newArray As New JArray()
            For Each item In obj.Children()
                newArray.Add(SanitizeKeys(item))
            Next
            Return newArray
        Else
            Return obj
        End If
    End Function

    Private Sub dropdownDatabases_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dropdownDatabases.SelectedIndexChanged
        DBpass.Enabled = True
        DBpass.Visible = True
        LabelDBpassword.Text = "password:"
        If dropdownDatabases.SelectedItem.Text.StartsWith("Intersystems") Then
            DBpass.Enabled = True
            LabelDBpassword.Visible = True
            ConnStr.ToolTip = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser;"
        ElseIf dropdownDatabases.SelectedItem.Text.StartsWith("Postgre") Then
            DBpass.Enabled = True
            LabelDBpassword.Visible = True
            ConnStr.ToolTip = "Server = yourserver; Port = yourport; Database=yourdatabase; User ID=youruser;"
        ElseIf dropdownDatabases.SelectedItem.Text = "ODBC" Then
            DBpass.Enabled = False
            DBpass.Visible = False
            LabelDBpassword.Visible = False
            LabelDBpassword.Text = ""
            ConnStr.ToolTip = "DSN=yourDSNname;UID=youruser;"
            'ElseIf dropdownDatabases.SelectedItem.Text = "OleDb" Then
            '    DBpass.Enabled = False
            '    DBpass.Visible = False
            '    LabelDBpassword.Visible = False
            '    LabelDBpassword.Text = ""
            '    ConnStr.ToolTip = "Data Source=yourfilepath;"
        ElseIf dropdownDatabases.SelectedItem.Text = "Oracle" Then
            DBpass.Enabled = True
            LabelDBpassword.Visible = True
            ConnStr.ToolTip = "data source=yourserver:yourport/yourdatabase;user id=youruser;"
        Else
            DBpass.Enabled = True
            LabelDBpassword.Visible = True
            ConnStr.ToolTip = "Server=yourserver; Database=yourdatabase; User ID=youruser;"
        End If
        ConnStr.Text = ConnStr.ToolTip
    End Sub
End Class
