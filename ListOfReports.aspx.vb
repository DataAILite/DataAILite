Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Partial Class ListOfReports
    Inherits System.Web.UI.Page
    Private Sub ListOfReports_Init(sender As Object, e As EventArgs) Handles Me.Init
        Page.MaintainScrollPositionOnPostBack = True
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If

        'uploads folder
        If applpath Is Nothing Then applpath = ""
        If applpath = "" AndAlso ConfigurationManager.AppSettings("fileupload") IsNot Nothing AndAlso ConfigurationManager.AppSettings("fileupload").ToString.Trim <> "" Then
            applpath = ConfigurationManager.AppSettings("fileupload").ToString.Trim
        End If
        If applpath = "" Then
            applpath = System.AppDomain.CurrentDomain.BaseDirectory() & "\Temp\"
        End If
        applpath = applpath.Replace("/", "\") & "\"
        applpath = applpath.Replace("\\", "\")

        Dim crystalok As String = String.Empty
        crystalok = ConfigurationManager.AppSettings("crystal").ToString
        Session("Crystal") = crystalok
        Session("pdfpath") = ""
        'Session("UserDB") = Session("UserConnString").ToString
        'If Session("UserConnString").ToString.ToUpper.IndexOf("USER ID") >= 0 Then
        '    Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.ToUpper.IndexOf("USER ID")).Trim
        'End If
        chkInitialReports.Checked = False
        Session("SPparameters") = Nothing
        Session("Attributes") = ""
        Session("SQLquerytext") = ""
        Session("SPname") = ""
        Session("SPtext") = ""
        Session("mySQL") = Nothing
        Session("dv3") = Nothing
        Session("dtb") = Nothing
        Session("dbstats") = Nothing
        Session("RepTablesCount") = 0
        Session("RepTable") = Nothing
        Session("ParamNames") = Nothing
        Session("ParamValues") = Nothing
        Session("ParamTypes") = Nothing
        Session("ParamFields") = Nothing
        Session("srchfld") = Nothing
        Session("srchoper") = Nothing
        Session("srchval") = Nothing
        Session("noedit") = Nothing
        Session("srchstm") = ""
        Session("WhereText") = ""
        Session("WhereStm") = ""
        Session("addwhere") = ""
        Session("filter") = ""

        Session("cat1") = Nothing
        Session("cat2") = Nothing
        Session("AxisY") = Nothing
        Session("Aggregate") = Nothing
        Session("txtMapName") = ""
        Session("ColorField") = Nothing
        Session("color") = Nothing
        Session("openmap") = ""
        Session("StatDash") = ""
        Session("UserDash") = ""

        Session("origin") = ""
        Session("TableFriendlyName") = ""
        Session("arr") = ""
        Session("nrec") = ""
        Session("ttl") = ""
        Session("x1") = ""
        Session("x2") = ""
        Session("y1") = ""
        Session("srt") = ""
        Session("newarr") = "yes"
        Session("StatDash") = "no"
        Session("ParamsCount") = -1
        Session("MapChart") = ""
        Session("MatrixChart") = ""
        Session("AllUnSelected") = ""
        Session("AllSelected") = ""
        Session("cat1") = ""
        Session("cat2") = ""
        Session("AxisY") = ""
        Session("Aggregate") = ""
        Session("AxisYM") = ""
        Session("AxisXM") = ""
        Session("fnM") = ""
        Session("AggregateM") = ""
        Session("MFld") = " "
        Session("SELECTEDValuesM") = " "
        Session("SELECTEDFlds") = Nothing
        Session("sqltoexport") = ""
        Session("DataToChatAI") = ""
        Session("OriginalDataTable") = Nothing
        Session("dataTable") = Nothing
        Session("QuestionToAI") = ""
        Session("GridView1DataSource") = Nothing
        Session("GridView2DataSource") = Nothing
        Session("nv") = 0
        Session("dataGroups") = Nothing
        Session("DataToChatAIGroups") = Nothing
        'Session("pagewidth") = ""
        'Session("pageheight") = ""
        Session("RunSched") = "no"
        Session("SchedReport") = ""
        Session("transid") = "" 'ConfigurationManager.AppSettings("transid").ToString
        Session("curmonth") = ""
        Session("matrix") = ""
        If DataModule.userdbcase Is Nothing Then
            DataModule.userdbcase = "lower"
        End If
        If Not IsPostBack Then
            If Session("map") = "yes" AndAlso IsNumeric(Session("srd")) Then
                Response.Redirect("ShowReport.aspx?srd=" & Session("srd").ToString)
                Session("pagewidth") = ""
                Session("pageheight") = ""
            ElseIf Session("dash") = "yes" AndAlso IsNumeric(Session("srd")) AndAlso CInt(Session("srd")) = 30 Then
                Response.Redirect("Dashboard.aspx?dash=yes&Prop6=" & Session("logon") & "&dashboard=" & Session("dashboard").ToString)
                Session("pagewidth") = ""
                Session("pageheight") = ""
            ElseIf Session("sched") = "yes" AndAlso IsNumeric(Session("srd")) Then
                Response.Redirect("ShowReport.aspx?srd=" & Session("srd").ToString)
            ElseIf Session("shared") = "yes" AndAlso IsNumeric(Session("srd")) Then
                Response.Redirect("ShowReport.aspx?srd=" & Session("srd").ToString)
            Else
                Session("pagewidth") = ""
                Session("pageheight") = ""
                Session("srd") = 0
                Session("map") = ""
                Session("REPORTID") = String.Empty
            End If
        End If
        Session("Comments") = ""
        DropDownListConnStr.Items.Clear()
        Dim i As Integer
        If Session("admin") = "super" Then
            '"User ID=dbuserid; Password=dbpassword;"
            If Session("DBUserID") Is Nothing OrElse Session("DBUserID").ToString.Trim = "" Then
                Session("DBUserID") = ""
            End If
            If Session("DBUserPass") Is Nothing OrElse Session("DBUserPass").ToString.Trim = "" Then
                Session("DBUserPass") = ""
            End If
            If txtDBUserID.Text.Trim = "" Then
                txtDBUserID.BorderColor = Color.Red
            ElseIf txtDBUserPass.Text.Trim = "" Then
                txtDBUserPass.BorderColor = Color.Red
            Else
                '"User ID=dbuserid; Password=dbpassword;"
                txtDBUserID.BorderColor = Color.Black
                txtDBUserID.BorderWidth = 1
                txtDBUserPass.BorderColor = Color.Black
                txtDBUserPass.BorderWidth = 1
            End If
            DropDownListConnStr.Visible = True
            DropDownListConnStr.Enabled = True
            LabelDBUserPass.Visible = True
            LabelDBUserID.Visible = True
            txtDBUserPass.Visible = True
            txtDBUserPass.Enabled = True
            txtDBUserID.Visible = True
            txtDBUserID.Enabled = True
            btnShowList.Visible = True
            btnShowList.Enabled = True
            lnkHideBadReports.Visible = True
            lnkHideBadReports.Enabled = True
            lnkUnHideBadReports.Visible = True
            lnkUnHideBadReports.Enabled = True
            Dim userprov As String = Session("UserConnProvider")
            Dim sql As String = ""
            If userprov <> "Oracle.ManagedDataAccess.Client" Then
                sql = "SELECT DISTINCT ReportDB FROM OURReportInfo WHERE ReportDB>'a' ORDER BY ReportDB"
            Else
                sql = "SELECT DISTINCT ReportDB FROM OURReportInfo ORDER BY ReportDB"
            End If
            Dim dvc As DataView = mRecords(sql)
            If Not dvc Is Nothing AndAlso Not dvc.Table Is Nothing AndAlso dvc.Table.Rows.Count > 0 Then
                Dim dr As DataRow
                Dim db As String = GetDataBase(Session("UserConnString").ToString, userprov)
                Dim htOracleDB As New Hashtable

                For i = 0 To dvc.Table.Rows.Count - 1
                    dr = dvc.Table.Rows(i)
                    If userprov <> "Oracle.ManagedDataAccess.Client" Then
                        DropDownListConnStr.Items.Add(dr("ReportDB").ToString.Trim)
                    ElseIf htOracleDB(db) = "" Then
                        htOracleDB.Add(db, "1")
                        DropDownListConnStr.Items.Add(db & ";")
                    End If


                    If i = 0 Then
                        DropDownListConnStr.Items(0).Selected = True
                        DropDownListConnStr.SelectedValue = dvc.Table.Rows(0)("ReportDB").ToString.Trim
                    End If
                Next
                If Session("ReportDBforSuper") IsNot Nothing AndAlso Session("ReportDBforSuper").ToString.Trim <> "" Then
                    DropDownListConnStr.SelectedValue = Session("ReportDBforSuper")
                    DropDownListConnStr.SelectedItem.Text = Session("ReportDBforSuper")
                Else
                    DropDownListConnStr.SelectedValue = DropDownListConnStr.SelectedValue.ToString.Trim
                    DropDownListConnStr.SelectedItem.Text = DropDownListConnStr.SelectedValue.ToString.Trim
                    Session("ReportDBforSuper") = DropDownListConnStr.SelectedValue.ToString.Trim
                End If
            End If
            Dim er As String = String.Empty
            Session("ReportDBProvforSuper") = GetProviderNameByConnString(Session("ReportDBforSuper").ToString.Trim, er)
            If Not Session("UserConnString").ToString.Trim.StartsWith(Session("ReportDBforSuper").ToString.Trim) Then
                Session("UserConnString") = Session("ReportDBforSuper")
            End If
            Session("UserConnProvider") = Session("ReportDBProvforSuper")
            txtDBUserID.Text = Session("DBUserID")
            txtDBUserPass.Text = Session("DBUserPass")
        Else
            DropDownListConnStr.Visible = False
            DropDownListConnStr.Enabled = False
            LabelDBUserPass.Visible = False
            LabelDBUserID.Visible = False
            txtDBUserPass.Visible = False
            txtDBUserPass.Enabled = False
            txtDBUserID.Visible = False
            txtDBUserID.Enabled = False
            btnShowList.Visible = False
            btnShowList.Enabled = False
            lnkHideBadReports.Visible = False
            lnkHideBadReports.Enabled = False
            lnkUnHideBadReports.Visible = False
            lnkUnHideBadReports.Enabled = False
        End If
        'Session("UserDB") = Session("UserConnString").ToString
        'If Session("UserConnString").ToString.ToUpper.IndexOf("USER ID") >= 0 Then
        '    Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.ToUpper.IndexOf("USER ID")).Trim
        'End If

        If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
            If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
            If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        Else
            If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
        End If

        If Session("OURConnString").ToString.Trim = "Sqlite" AndAlso Session("UserConnString").ToString.Trim <> "Sqlite" Then
            'btnListOfTables.Enabled = True
            'btnListOfTables.Visible = True
            btnListOfClasses.Enabled = True
            btnListOfClasses.Visible = True
            'btnListOfJoins.Enabled = True
            'btnListOfJoins.Visible = True
            lnkRedoInitialReports.Visible = True
            chkInitialReports.Checked = True
            chkInitialReports.Visible = True
            chkInitialReports.Enabled = True
            Session("ShowInitialReports") = "yes"
        ElseIf Session("CSV") = "yes" Then
            'btnListOfTables.Enabled = True
            'btnListOfTables.Visible = True
            btnListOfClasses.Enabled = True
            btnListOfClasses.Visible = True
            'btnListOfJoins.Enabled = True
            'btnListOfJoins.Visible = True
            lnkRedoInitialReports.Visible = False
            chkInitialReports.Checked = False
            chkInitialReports.Visible = False
            chkInitialReports.Enabled = False
            Session("ShowInitialReports") = "no"
        ElseIf Session("CSV") <> "yes" AndAlso (Session("admin") = "super" OrElse Session("admin") = "admin") Then
            'btnListOfTables.Enabled = True
            'btnListOfTables.Visible = True
            btnListOfClasses.Enabled = True
            btnListOfClasses.Visible = True
            'btnListOfJoins.Enabled = True
            'btnListOfJoins.Visible = True
        ElseIf (Session("CSV") <> "yes" AndAlso Session("admin") = "user") OrElse Session("admin") = "expired" Then
            'btnListOfTables.Enabled = False
            'btnListOfTables.Visible = False
            'btnListOfJoins.Enabled = False
            'btnListOfJoins.Visible = False
            btnListOfClasses.Enabled = False
            btnListOfClasses.Visible = False
        End If

        If Not Session("ShowInitialReports") Is Nothing Then
            If Session("ShowInitialReports") = "yes" Then
                chkInitialReports.Checked = True
            Else
                chkInitialReports.Checked = False
            End If
        Else
            chkInitialReports.Checked = False
        End If
        Session("mapkey") = ConfigurationManager.AppSettings("mapkey").ToString
        GenerateMap.mapkey = Session("mapkey")

        If Session("OurConnProvider").ToString.Trim = "Sqlite" Then
            HyperLinkScheduledReports.Visible = False
            HyperLinkScheduledReports.Enabled = False
            HyperLinkScheduledDownloads.Visible = False
            HyperLinkScheduledDownloads.Enabled = False
            HyperLinkScheduledImports.Visible = False
            HyperLinkScheduledImports.Enabled = False
            LinkButtonHelpDesk.Enabled = False
            LinkButtonHelpDesk.Visible = False
            HyperLinkTestHelp.Enabled = False
            HyperLinkTestHelp.Visible = False
        End If


    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '' code SAMPLE
        'Dim redirect As String = "<script>window.open('~/RunScheduledItems.aspx');</script>"
        'Response.Write(redirect)
        're = ScheduledImportsAndSendEmail(Session("logon"), Session("SupportEmail"))

        Dim re As String = SendEmailAdminScheduledReports()

        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("admin") = "super" OrElse Session("admin") = "admin" Then
            chkAdvanced.Visible = True
        Else
            chkAdvanced.Visible = False
        End If
        If (Not IsPostBack) Then
            If (Not Session("AdvancedUser") Is Nothing AndAlso Session("AdvancedUser") = True) Then
                chkAdvanced.Checked = True
                btnDataImport.Visible = True
                btnDataImport.Enabled = True
            Else
                chkAdvanced.Checked = False
                btnDataImport.Visible = False
                btnDataImport.Enabled = False
            End If
        End If

        If Not IsPostBack Then
            Session("NewFileCreated") = False
            Session("FileJustDeleted") = False
            Session("TabN") = 0
            Session("TabNQ") = 0
            Session("TabNF") = 0
            Session("SortedView") = Nothing
            Session("SortColumn") = ""
            Session("ret") = ""
            Session("UserDB") = ""
            Session("delimiter") = ","
            Session("UserEndDate") = ""
            trDB.Visible = False
            trMessage.Visible = False
        End If
        Dim webinstall As String = ConfigurationManager.AppSettings("webinstall").ToString
        Dim dbinstall As String = ConfigurationManager.AppSettings("dbinstall").ToString
        If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
            trPay1.Visible = True
            HyperLinkPay1.Visible = True
        Else
            trPay1.Visible = True
            HyperLinkPay1.Visible = False
        End If
        Session("webinstall") = webinstall
        Session("dbinstall") = dbinstall
        trPay2.Visible = False
        Dim userdb As String = String.Empty
        Dim userprov As String = String.Empty

        If Session("UserConnString") <> "" Then
            userdb = Session("UserConnString")
            userprov = Session("UserConnProvider")
            If userprov <> "Oracle.ManagedDataAccess.Client" Then
                If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
                If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
            Else
                If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim  ' Piece(userdb, ";", 1, 2) & ";"
            End If

            Session("UserDB") = userdb
            Session("UnitDB") = userdb
            Session("dbname") = GetDataBase(Session("UserConnString"), Session("UserConnProvider"))
            If Session("dbname") = "" Then
                Session("dbname") = userdb.Substring(userdb.LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            End If

            trDB.Visible = True
            HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=List_of_Reports"

            If Session("UserConnProvider") = "InterSystems.Data.IRISClient" Then
                'Session("dbname") = GetNamespaceFromConnectionString(Session("UserConnString"))
                LabelDB.Text = "User IRIS Database: " & Session("dbname")
                'HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=55"
            ElseIf Session("UserConnProvider") = "InterSystems.Data.CacheClient" Then
                'Session("dbname") = GetNamespaceFromConnectionString(Session("UserConnString"))
                LabelDB.Text = "User Cache Database: " & Session("dbname")
                'HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=55"
            ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
                'Session("dbname") = GetDatabaseFromConnectionString(Session("UserConnString")).ToLower
                LabelDB.Text = "User MySql Database: " & Session("dbname")
                If Session("logon") = "demo" Then
                    LabelDB.Text = "DEMO Database Cinema"
                ElseIf Session("logon") = "csvdemo" Then
                    LabelDB.Text = "Analytics, Matrix balancing, Maps, KML generator DEMO"
                End If
                'HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=7"
            ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
                ' !!!!!! fix  Session("dbname") for Oracle
                'Session("dbname") = Piece(Session("dbname"), "/", 2).Replace(";", "")
                LabelDB.Text = "User Oracle Database: " & Session("UserDB") 'Session("dbname")
                'HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=79"
            ElseIf Session("UserConnProvider") = "System.Data.Odbc" Then
                LabelDB.Text = "User ODBC DSN: " & Session("dbname")
                'HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=79"
            ElseIf Session("UserConnProvider") = "System.Data.OleDb" Then
                LabelDB.Text = "User OleDb data source: " & Session("dbname")
            ElseIf Session("UserConnProvider") = "Npgsql" Then
                LabelDB.Text = "User PostgreSQL data source: " & Session("dbname")
            Else 'SQL Server
                'Session("dbname") = GetDatabaseFromConnectionString(Session("UserConnString"))
                LabelDB.Text = "User Sql Server Database: " & Session("dbname")
                'HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=79"
            End If
        ElseIf Session("UserConnString") = "" AndAlso Session("admin") = "super" Then 'using our database for super user
            trDB.Visible = True
            userdb = Session("OURConnString")
            userprov = Session("OURConnProvider")

            If userprov <> "Oracle.ManagedDataAccess.Client" Then
                If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
                If userdb.ToUpper.IndexOf("UID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("UID")).Trim
            Else
                If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
            End If

            Session("UserDB") = ""
            Session("UnitDB") = userdb
            If Session("OURConnProvider") = "InterSystems.Data.IRISClient" Then
                LabelDB.Text = "User IRIS Database: " & userdb
            ElseIf Session("OURConnProvider") = "InterSystems.Data.CacheClient" Then
                LabelDB.Text = "User Cache Database: " & userdb
            ElseIf Session("OURConnProvider") = "MySql.Data.MySqlClient" Then
                LabelDB.Text = "User MySql Database: " & userdb
            ElseIf Session("OURConnProvider") = "Oracle.ManagedDataAccess.Client" Then
                LabelDB.Text = "User Oracle Database: " & userdb
            ElseIf Session("OURConnProvider") = "System.Data.Odbc" Then
                LabelDB.Text = "User ODBC DSN: " & Session("dbname")
            ElseIf Session("OURConnProvider") = "System.Data.OleDb" Then
                LabelDB.Text = "User OleDb data source: " & Session("dbname")
            ElseIf Session("OURConnProvider") = "Npgsql" Then
                LabelDB.Text = "User PostgreSQL data source: " & Session("dbname")
            Else 'SQL Server
                LabelDB.Text = "User Sql Server Database: " & userdb
            End If
            Session("UserConnString") = Session("OURConnString")
            Session("UserConnProvider") = Session("OURConnProvider")
        End If
        HyperLinkPay1.PostBackUrl = "Pay.aspx?stat=addpay&lgn=" & Session("logon") & "&msg=---"
        HyperLinkPay2.PostBackUrl = "Pay.aspx?stat=addpay&lgn=" & Session("logon") & "&msg=---"
        LabelMessage.Text = " "
        'check expiration 
        Dim ret As String = String.Empty
        Dim enddate As String = String.Empty
        Dim startdate As String = String.Empty

        If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
            If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
            If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        Else
            If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
        End If
        Dim userconnstrnopass As String = Session("UserDB").ToString.Trim
        If userconnstrnopass.IndexOf("Password") > 0 Then userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.IndexOf("Password")).Trim()
        'If userconnstrnopass.ToUpper.IndexOf("USER ID") > 0 Then
        '    userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("USER ID")).Trim()
        'End If

        Dim mrec As DataView
        If Session("admin") = "super" Then
            mrec = mRecords("SELECT * FROM [OURPERMITS] WHERE ([NetId]='" & Session("logon") & "') AND ([Application]='InteractiveReporting')")
        Else
            mrec = mRecords("SELECT * FROM [OURPERMITS] WHERE ([ConnStr] LIKE '%" & userconnstrnopass.Trim.Replace(" ", "%") & "%') AND ([NetId]='" & Session("logon") & "') AND ([Application]='InteractiveReporting')")
        End If
        Dim email As String = String.Empty
        If mrec Is Nothing OrElse mrec.Table Is Nothing OrElse mrec.Table.Rows.Count = 0 Then
            LabelMessage.Text = "Wrong Logon information."
            LabelMessage.Visible = True
            trMessage.Visible = True
            WriteToAccessLog(Session("logon"), "Wrong Logon information.", 0)
            Response.Redirect("Default.aspx?msg=Wrong Logon or password")
        Else
            Session("logonindx") = mrec.Table.Rows(0)("Indx").ToString
            Session("Unit") = mrec.Table.Rows(0)("Unit").ToString
            Session("Permit") = mrec.Table.Rows(0)("PERMIT").ToString  'Friendly Names editting
            Session("Access") = mrec.Table.Rows(0)("ACCESS").ToString  'Site admin, Support, Tester
            email = mrec.Table.Rows(0)("Email").ToString

            If Session("Access") = "SITEADMIN" OrElse Session("admin") = "super" OrElse (Session("UnitWEB").ToString.Contains(Session("URL").ToString) AndAlso Session("admin") = "super") Then
                HyperLink4.Enabled = True
                HyperLink4.Visible = True
                'HyperLinkTaskList
                HyperLinkTaskList.Enabled = True
                HyperLinkTaskList.Visible = True
            Else   'not the unit web site
                HyperLink4.Enabled = False
                HyperLink4.Visible = False
                HyperLinkTaskList.Enabled = False
                HyperLinkTaskList.Visible = False
            End If

            If IsDBNull(mrec.Table.Rows(0)("StartDate")) OrElse mrec.Table.Rows(0)("StartDate").ToString = "0000-00-00 00:00:00" Then
                'update StartDate with Now()
                startdate = DateToString(Now)
                ret = ExequteSQLquery("UPDATE OURPERMITS SET StartDate='" & startdate & "' WHERE [Application]='InteractiveReporting' AND NetID='" & Session("logon") & "'")
            Else
                startdate = DateToString(mrec.Table.Rows(0)("StartDate").ToString)
            End If
            If IsDBNull(mrec.Table.Rows(0)("EndDate")) OrElse mrec.Table.Rows(0)("EndDate").ToString = "0000-00-00 00:00:00" Then
                'update EndDate with Now()+3
                Dim n As Integer
                If Session("admin") = "super" Then
                    n = 3000
                Else
                    n = 2000
                End If
                enddate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, n, Now())))
                ret = ExequteSQLquery("UPDATE OURPERMITS SET EndDate='" & enddate & "' WHERE [Application]='InteractiveReporting' AND NetID='" & Session("logon") & "'")

            Else
                enddate = DateToString(mrec.Table.Rows(0)("EndDate").ToString)
            End If
            startdate = DateToString(startdate)
            enddate = DateToString(enddate)
            Session("UserStartDate") = startdate
            Session("UserEndDate") = enddate
            Dim ts As TimeSpan = CDate(enddate) - Now
            Dim days As Integer = CInt(ts.TotalDays)
            If days > 0 AndAlso days < 7 Then
                LabelMessage.Text = "ATTENTION!! The service expires on " & enddate & ". <br/>Please pay to avoid any interuptions, loosing of your report definitions, and continue to use the Online User Reporting system."
                LabelMessage.Visible = True
                trMessage.Visible = True
                'send email
                Dim adminemail As String = ConfigurationManager.AppSettings("supportemail").ToString
                ret = SendHTMLEmail("", "ATTENTION!! The service OUReports.com expires on " & enddate & ".", LabelMessage.Text, email, adminemail)
            Else
                If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
                    HyperLinkPay1.Visible = False
                    HyperLinkPay2.Visible = False
                End If
            End If
        End If
        If Not IsDBNull(mrec.Table.Rows(0)("Email")) Then
            Session("email") = mrec.Table.Rows(0)("Email").ToString
        Else
            Session("email") = ""
            'TODO alert that no email and suggest to update it
            LabelMessage.Text = LabelMessage.Text & " <br/> " & " No email address found. Please update your email address."
            LabelMessage.Visible = True
            trMessage.Visible = True
            WriteToAccessLog(Session("logon"), "No email address found.", 0)
        End If

        'TESTING ONLY
        'lnkRedoInitialReports_Click(sender, e)

        'check if the service expired than give only user permissions
        Dim err As String = String.Empty
        If ServiceExpired(DateToString(enddate), err) AndAlso Session("admin") <> "super" Then
            Session("admin") = "expired"
            Dim msg As String = "Service expired On " & enddate & ". Access granted To read reports"
            WriteToAccessLog(Session("logon"), msg, 0)
            LabelMessage.Text = msg
            LabelMessage.Visible = True
            trMessage.Visible = True
            'btnListOfTables.Enabled = False
            'btnListOfTables.Visible = False
            'btnListOfJoins.Enabled = False
            'btnListOfJoins.Visible = False
            btnListOfClasses.Enabled = False
            btnListOfClasses.Visible = False
            btnFriendlyNames.Enabled = False
            btnFriendlyNames.Visible = False
            HyperLinkScheduledReports.Enabled = False
            HyperLinkScheduledReports.Visible = False
        End If
        Dim dv As DataView
        Dim rep As String = ""
        Dim repttl As String = ""
        Dim repdb As String = ""
        Dim openfrom As String = String.Empty
        Dim opento As String = String.Empty
        Dim repnew As String = Request("newrep")
        Dim j As Integer

        list.Rows(0).Cells(0).InnerHtml = ""
        list.Rows(0).Cells(1).Align = "center"
        list.Rows(0).Cells(2).InnerHtml = ""
        list.Rows(0).Cells(3).InnerHtml = ""
        list.Rows(0).Cells(4).InnerHtml = ""
        list.Rows(0).Cells(5).InnerHtml = ""
        list.Rows(0).Cells(6).InnerHtml = ""
        list.Rows(0).Cells(7).InnerHtml = ""
        dv = Nothing
        Dim sqlst As String = String.Empty
        Dim srch As String = cleanText(txtSearch.Text)
        Dim sqlwh As String = String.Empty
        Dim usrdb As String = Session("UserDB").ToString
        Dim er As String = String.Empty
        'get list of reports depending on permissions
        If Session("admin") = "super" Then
            LabelDB.Text = "User Database: "
            DropDownListConnStr.Visible = True
            DropDownListConnStr.Enabled = True
            If DropDownListConnStr.SelectedValue <> "" Then
                If Session("ShowList") <> "yes" Then
                    usrdb = DropDownListConnStr.SelectedValue
                Else
                    Session("ShowList") = ""
                End If


                sqlst = "Select DISTINCT ReportDB,Param7type,ReportTtl,ReportID FROM  OURReportInfo WHERE ReportDB LIKE '%" & usrdb.Trim.Replace(" ", "%") & "%' "
                If srch <> "" Then
                    sqlwh = " AND ((ReportDB LIKE '%" & srch & "%') OR (ReportTtl LIKE '%" & srch & "%') OR (ReportID LIKE '%" & srch & "%')) "
                    If Session("ShowInitialReports") = "no" Then
                        sqlwh = sqlwh & " AND ReportID NOT LIKE '" & Session("dbname").ToString.Trim & "%' "
                    End If
                End If
            Else
                sqlst = "Select DISTINCT ReportDB,Param7type,ReportTtl,ReportID FROM  OURReportInfo "
                If srch <> "" Then
                    sqlwh = " WHERE ((ReportDB LIKE '%" & srch & "%') OR (ReportTtl LIKE '%" & srch & "%') OR (ReportID LIKE '%" & srch & "%')) "
                    If Session("ShowInitialReports") = "no" Then
                        sqlwh = sqlwh & " AND ReportID NOT LIKE '" & Session("dbname").ToString.Trim & "%' "
                    End If
                Else
                    If Session("ShowInitialReports") = "no" Then
                        sqlwh = sqlwh & " WHERE ReportID NOT LIKE '" & Session("dbname").ToString.Trim & "%' "
                    End If
                End If
            End If
            sqlst = sqlst & sqlwh & " ORDER BY ReportDB,Param7type,ReportTtl,ReportID"
            dv = mRecords(sqlst, er)
        Else
            DropDownListConnStr.Visible = False
            DropDownListConnStr.Enabled = False
            Dim sqlt As String = String.Empty
            If usrdb <> String.Empty Then
                'Param1type = 1 if report corrupt
                sqlt = "SELECT DISTINCT PARAM1,ReportTtl,ReportDB,AccessLevel,OpenFrom,OpenTo,OURPERMISSIONS.Indx,Param7type,Param1type FROM OURPERMISSIONS INNER JOIN OURReportInfo on (OURPERMISSIONS.Param1=OURReportInfo.ReportID) WHERE NETID='" & Session("logon") & "' AND APPLICATION='" & Session("Application") & "' AND (Param1type Is NULL OR Param1type NOT LIKE '1')  AND ReportDB LIKE '%" & usrdb.Trim.Replace(" ", "%") & "%' "
                If srch <> "" Then
                    sqlwh = " AND ((ReportDB LIKE '%" & srch & "%') OR (ReportTtl LIKE '%" & srch & "%') OR (PARAM1 LIKE '%" & srch & "%')) "
                End If
                If Session("ShowInitialReports") = "no" Or Session("logon") = "csvdemo" Then
                    sqlwh = sqlwh & " AND ReportID NOT LIKE '" & Session("dbname").ToString.Trim & "%' "
                End If
                sqlt = sqlt & sqlwh & " ORDER BY Param7type,ReportTtl"
                dv = mRecords(sqlt, er)  'from OUR database
            End If
        End If
        Dim openforedit As Boolean = False
        Dim openforcopy As String = String.Empty
        Dim reccount As Integer = 0
        Dim reports As String = "|"
        Dim htReport As New Hashtable

        If dv IsNot Nothing AndAlso dv.Count > 0 AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
            Session("dvListOfReports") = dv
            'If Session("admin") = "super" Then
            '    UnHideCorruptedReports()
            'Else
            '    HideCorruptedReports()
            '    dv.RowFilter = "Param1type Is NULL OR Param1type NOT LIKE '1'"
            '    Session("dvListOfReports") = dv.ToTable
            'End If

            'lblReportsCount.Text = dv.Table.Rows.Count.ToString & " reports"
            For i = 0 To dv.Table.Rows.Count - 1
                openforedit = False
                openforcopy = ""
                openfrom = ""
                opento = ""
                AddRowIntoHTMLtable(dv.Table.Rows(i), list)
                For j = 0 To list.Rows(i + 1).Cells.Count - 1
                    list.Rows(i + 1).Cells(j).InnerText = ""
                    list.Rows(i + 1).Cells(j).InnerHtml = ""
                Next
                If i Mod 2 = 0 Then
                    list.Rows(i + 1).BgColor = "#EFFBFB"
                Else
                    list.Rows(i + 1).BgColor = "white"
                End If
                If Session("admin") = "super" Then
                    rep = dv.Table.Rows(i)("ReportId")
                    If reports.Contains("|" & rep & "|") Then
                        Continue For
                    End If
                    repttl = dv.Table.Rows(i)("ReportTtl").ToString.Replace(":", "-")
                    repdb = dv.Table.Rows(i)("ReportDB").ToString
                    openforedit = True
                    If Not IsDBNull(dv.Table.Rows(i)("Param7type")) Then  'lock or not
                        openforcopy = dv.Table.Rows(i)("Param7type").ToString
                    End If
                Else
                    rep = dv.Table.Rows(i)("Param1")
                    If reports.Contains("|" & rep & "|") Then
                        Continue For
                    End If
                    repttl = dv.Table.Rows(i)("ReportTtl").ToString.Replace(":", "-")
                    If Not IsDBNull(dv.Table.Rows(i)("Param7type")) Then
                        openforcopy = dv.Table.Rows(i)("Param7type").ToString
                    End If
                    'If dates are between "from" to "to"
                    openfrom = dv.Table.Rows(i)("OpenFrom").ToString
                    opento = dv.Table.Rows(i)("OpenTo").ToString
                    If dv.Table.Rows(i)("OpenFrom").ToString = "" Then
                        openfrom = Session("UserStartDate")
                    End If
                    If dv.Table.Rows(i)("OpenTo").ToString = "" Then
                        opento = Session("UserEndDate")
                    End If
                    Dim dtFrom As DateTime = CDate(openfrom)
                    Dim dtTo As DateTime = CDate(opento)

                    openfrom = DateToString(openfrom)
                    opento = DateToString(opento)
                    If Session("OURConnProvider") = "Oracle.ManagedDataAccess.Client" Then
                        If Now() <= dtTo AndAlso Now() >= dtFrom Then openforedit = True
                    Else
                        If DateToString(Now()).ToString <= opento AndAlso DateToString(Now()).ToString >= openfrom Then
                            openforedit = True
                        End If
                    End If
                    'If DateToString(Now()).ToString <= opento AndAlso DateToString(Now()).ToString >= openfrom Then
                    '    openforedit = True
                    'End If
                End If
                Dim upRep As String = rep.ToUpper
                If htReport(upRep) Is Nothing Then
                    htReport.Add(upRep, "1")

                    reccount = reccount + 1
                    reports = reports & rep & "|"

                    'list.Rows(0).Cells(0).InnerText = "DataAI"
                    'list.Rows(0).Cells(0).Align = "center"
                    'list.Rows(i + 1).Cells(0).InnerText = "AI"
                    'list.Rows(i + 1).Cells(0).InnerHtml = "<a href='ShowReport.aspx?srd=15&Report=" & rep & "' title='Chat with AI onclick='redirect(event); return false;'>AI</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href='ShowReport.aspx?srd=16&Report=" & rep & "' title='Chat with DataAI - Data Analytical Intelligence and AI' onclick='redirect(event);return false;'>DataAI</a>"  'Target='_blank'
                    'list.Rows(i + 1).Cells(0).Align = "center"

                    Dim ctl As New LinkButton

                    ctl.Text = "AI "
                    ctl.ID = rep
                    ctl.ToolTip = "Chat with AI"
                    ctl.CssClass = "NodeStyle"
                    ctl.Font.Size = 10
                    AddHandler ctl.Click, AddressOf btnAI_Click
                    list.Rows(i + 1).Cells(0).InnerText = ""
                    list.Rows(i + 1).Cells(0).Controls.Add(ctl)
                    list.Rows(i + 1).Cells(0).Align = "center"


                    list.Rows(0).Cells(0).Align = "center"

                    ctl = New LinkButton
                    ctl.Text = "DataAI"
                    ctl.ID = "DataAI*" & rep
                    ctl.ToolTip = "Chat with DataAI - Data Analytical Intelligence and AI"
                    ctl.CssClass = "NodeStyle"
                    ctl.Font.Size = 10
                    AddHandler ctl.Click, AddressOf btnDataAI_Click
                    list.Rows(i + 1).Cells(1).InnerText = ""
                    list.Rows(i + 1).Cells(1).Controls.Add(ctl)
                    list.Rows(i + 1).Cells(1).Align = "center"
                    list.Rows(0).Cells(1).Align = "center"

                    ctl = New LinkButton
                    ctl.Text = repttl
                    ctl.ID = rep & "*" & repttl
                    ctl.ToolTip = "Show report with ID = " & rep
                    ctl.CssClass = "NodeStyle"
                    ctl.Font.Size = 10
                    AddHandler ctl.Click, AddressOf btnShow_Click
                    list.Rows(i + 1).Cells(2).InnerText = ""
                    list.Rows(i + 1).Cells(2).Controls.Add(ctl)
                    list.Rows(i + 1).Cells(2).Align = "left"
                    list.Rows(0).Cells(2).Align = "left"

                    ctl = New LinkButton
                    ctl.Text = "analytics"
                    ctl.ID = rep & "&" & repttl
                    ctl.ToolTip = "Analytics for " & repttl
                    ctl.CssClass = "NodeStyle"
                    ctl.Font.Size = 10
                    AddHandler ctl.Click, AddressOf btnAnalytics_Click
                    list.Rows(0).Cells(9).InnerText = "Analytics"
                    list.Rows(i + 1).Cells(9).InnerText = ""
                    list.Rows(i + 1).Cells(9).Controls.Add(ctl)


                    ctl = New LinkButton
                    ctl.Text = "data"
                    ctl.ID = rep & ";" & repttl
                    ctl.ToolTip = "Data of " & repttl
                    ctl.CssClass = "NodeStyle"
                    ctl.Font.Size = 10
                    AddHandler ctl.Click, AddressOf btnData_Click
                    list.Rows(0).Cells(10).InnerText = "Data"
                    list.Rows(i + 1).Cells(10).InnerText = ""
                    list.Rows(i + 1).Cells(10).Controls.Add(ctl)


                    ctl = New LinkButton
                    ctl.Text = "charts"
                    ctl.ID = rep & "+" & repttl
                    ctl.ToolTip = "Charts for " & repttl
                    ctl.CssClass = "NodeStyle"
                    ctl.Font.Size = 10
                    AddHandler ctl.Click, AddressOf btnCharts_Click
                    list.Rows(0).Cells(11).InnerText = "Charts"
                    list.Rows(i + 1).Cells(11).InnerText = ""
                    list.Rows(i + 1).Cells(11).Controls.Add(ctl)


                    If Session("admin") = "super" OrElse (Session("admin") <> "expired" AndAlso dv.Table.Rows(i)("AccessLevel") = "admin") Then
                        'admin for this report to edit, not expired
                        list.Rows(0).Cells(0).InnerHtml = "Chat with AI"
                        list.Rows(0).Cells(1).InnerHtml = "DataAI"
                        list.Rows(0).Cells(2).InnerHtml = " Show Report "
                        If openforedit = True Then
                            ctl = New LinkButton
                            list.Rows(0).Cells(3).InnerHtml = " Edit"
                            list.Rows(0).Cells(4).InnerHtml = " Copy"
                            list.Rows(0).Cells(5).InnerHtml = " Delete"
                            'list.Rows(i + 1).Cells(3).InnerHtml = "<a href='ReportEdit.aspx?repedit=yes&Report=" & rep & "'>edit</a> &nbsp;&nbsp;&nbsp;&nbsp; <a href='Delete.aspx?repedit=del&Report=" & rep & "'>del</a>"
                            If openforcopy <> "standard" Then
                                ctl.Text = "edit"
                                ctl.ID = rep & "`" & repttl
                                ctl.ToolTip = "Edit " & rep
                                ctl.CssClass = "NodeStyle"
                                ctl.Font.Size = 10

                                AddHandler ctl.Click, AddressOf btnEdit_Click
                                list.Rows(i + 1).Cells(3).InnerText = ""
                                list.Rows(i + 1).Cells(3).Controls.Add(ctl)
                                'list.Rows(i + 1).Cells(3).InnerHtml = "<a href='ReportEdit.aspx?repedit=yes&Report=" & rep & "'>edit</a>"
                            Else
                                list.Rows(i + 1).Cells(3).InnerHtml = " locked "
                            End If

                            ctl = New LinkButton
                            list.Rows(0).Cells(8).InnerHtml = " Maps"
                            If openforcopy <> "standard" Then
                                ctl.Text = "map"
                                ctl.ID = rep & "#" & repttl
                                ctl.ToolTip = "Map " & rep
                                ctl.CssClass = "NodeStyle"
                                ctl.Font.Size = 10

                                AddHandler ctl.Click, AddressOf btnMap_Click
                                list.Rows(i + 1).Cells(8).InnerText = ""
                                list.Rows(i + 1).Cells(8).Controls.Add(ctl)
                            Else
                                list.Rows(i + 1).Cells(8).InnerHtml = " "
                            End If

                            ctl = New LinkButton
                            ctl.Text = "copy"
                            ctl.ID = rep & "~" & repttl
                            ctl.ToolTip = "Copy " & rep
                            ctl.CssClass = "NodeStyle"
                            ctl.Font.Size = 10
                            AddHandler ctl.Click, AddressOf btnCopy_Click
                            list.Rows(i + 1).Cells(4).InnerText = ""
                            list.Rows(i + 1).Cells(4).Controls.Add(ctl)
                            If openforcopy <> "standard" Then
                                ctl = New LinkButton
                                ctl.Text = "delete"
                                ctl.ID = rep & "@" & repttl
                                ctl.ToolTip = "Delete " & rep
                                ctl.CssClass = "NodeStyle"
                                ctl.Font.Size = 10
                                AddHandler ctl.Click, AddressOf btnDelete_Click
                                list.Rows(i + 1).Cells(5).InnerText = ""
                                list.Rows(i + 1).Cells(5).Controls.Add(ctl)
                            Else
                                list.Rows(i + 1).Cells(5).InnerHtml = " "
                            End If
                            'list.Rows(i + 1).Cells(4).InnerHtml = "<a href='ReportCopy.aspx?repcopy=copy&Report=" & rep & "&NewReport=" & newrepttl & "'>copy</a>"
                            'list.Rows(i + 1).Cells(5).InnerHtml = " "
                        Else
                            list.Rows(i + 1).Cells(3).InnerHtml = "<a> </a> "
                            list.Rows(i + 1).Cells(4).InnerHtml = "<a> </a> "
                            list.Rows(i + 1).Cells(5).InnerHtml = " "
                            list.Rows(i + 1).Cells(6).InnerHtml = " "
                        End If
                    Else
                        list.Rows(i + 1).Cells(1).InnerHtml = "<a> </a> "
                        list.Rows(i + 1).Cells(2).InnerHtml = "<a> </a> "
                        list.Rows(i + 1).Cells(3).InnerHtml = "<a> </a> "
                        list.Rows(i + 1).Cells(4).InnerHtml = "<a> </a> "
                        list.Rows(i + 1).Cells(5).InnerHtml = " "
                        list.Rows(i + 1).Cells(6).InnerHtml = " "
                    End If
                    If Session("admin") = "super" Then
                        'list.Rows(i + 1).Cells(2).InnerHtml = "<a href='ReportEdit.aspx?repedit=yes&Report=" & rep & "'>edit</a>"
                        '&nbsp;&nbsp;&nbsp; <a href='Delete.aspx?repedit=del&Report=" & rep & "'>del</a>"
                        list.Rows(0).Cells(6).InnerHtml = " Database"
                        list.Rows(i + 1).Cells(6).InnerHtml = repdb
                        'lock/unlock report
                        list.Rows(0).Cells(7).InnerHtml = " Lock/Unlock"
                        ctl = New LinkButton
                        ctl.ID = rep & "^" & repttl
                        If openforcopy <> "standard" Then
                            ctl.Text = "lock"
                            ctl.ToolTip = "Make Report not editable: " & rep
                            ctl.CssClass = "NodeStyle"
                            ctl.Font.Size = 10
                            AddHandler ctl.Click, AddressOf LockReport_click
                        Else
                            ctl.Text = "unlock"
                            ctl.ToolTip = "Make Report editable: " & rep
                            AddHandler ctl.Click, AddressOf UnlockReport_click
                            ctl.CssClass = "NodeStyle"
                            ctl.Font.Size = 10
                        End If
                        list.Rows(i + 1).Cells(7).InnerText = ""
                        list.Rows(i + 1).Cells(7).Controls.Add(ctl)

                    Else
                        list.Rows(0).Cells(6).InnerHtml = " Expiration"
                        list.Rows(i + 1).Cells(6).InnerHtml = opento
                        list.Rows(i + 1).Cells(7).InnerText = ""
                    End If
                    rep = ""
                End If
            Next
            lblReportsCount.Text = reccount.ToString & " reports"

        Else  'no user reports
            'calc initial reports for Sqlite
            If Session("OURConnProvider") = "Sqlite" AndAlso Session("UserConnProvider") <> "Sqlite" Then
                Session("dvListOfReports") = Nothing
                Dim errr As String = String.Empty
                Dim rtn As String = MakeInitialReports(Session("logon"), "", Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), errr, True)
                Response.Redirect("ListOfReports.aspx")
            End If

            'If generic reports created
            LabelMessage.Text = LabelMessage.Text & "<br/> Click checkbox below to show generic reports created for you <br/> Click the link ""Tables/Classes"" above to see the data and analytics for your tables if you have them <br/>  Click the link ""New Report"" below to start report for existing tables or to upload data into new tables:"
            trMessage.Visible = True
            'else
            'MessageBox.Show("No user reports available. Click checkbox on the top of the table to see generic reports. Do you want to see available tables/classes and analytics for them?", "Show List of Tables/Classes ", "ListOfTables ", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.OK)
            'Response.Redirect("DataImport.aspx")
            'end if
        End If

        If Session("admin") = "admin" OrElse Session("admin") = "super" Then  'full admin to create new reports
            If Session("Permit") = "friendly" Then
                btnFriendlyNames.Visible = True
                btnFriendlyNames.Enabled = True
                'HyperLink3.Visible = True
                'HyperLink3.Enabled = True
            Else
                btnFriendlyNames.Visible = False
                btnFriendlyNames.Enabled = False
                'HyperLink3.Visible = False
                'HyperLink3.Enabled = False
            End If
        Else  'user - no admin, no super
            LabelCreateReport.Visible = False
            lnkRedoInitialReports.Visible = False
            'LabelCopyReport.Visible = False
            TextBoxNewReportTtl.Visible = False
            ButtonCreateReport.Visible = False
            trPay1.Visible = False
            If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
                trPay2.Visible = True
            End If
        End If
        'To test the site helpdesk:
        Dim url As String = HttpContext.Current.Request.Url.AbsoluteUri
        If url.Contains("localhost") AndAlso Session("OurConnProvider").Trim <> "Sqlite" Then
            HyperLinkTestHelp.Visible = True
            HyperLinkTestHelp.Enabled = True
            HyperLinkTestHelp.NavigateUrl = "~/HelpDesk.aspx?pass=help&rol=user&db=" & Session("dbname") & "&unit=" & Session("Unit") & "&ln=" & Session("logon")
        Else
            HyperLinkTestHelp.Visible = False
            HyperLinkTestHelp.Enabled = False
        End If
        If Not Request("repprbl") Is Nothing AndAlso Request("repprbl") = "yes" Then
            LinkButtonHelpDesk_Click("", EventArgs.Empty)
        End If

    End Sub
    Protected Sub LockReport_click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToLock As String = Piece(ctl.ID, "^", 1)
        Dim RptTtlToLock As String = Piece(ctl.ID, "^", 2)
        MessageBox.Show("Are you sure you want to lock the report?", "Lock Report " & RptTtlToLock & " (" & RptToLock & ")", "LockReport " & RptToLock, Controls_Msgbox.Buttons.YesNo, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.No)
    End Sub
    Protected Sub UnlockReport_click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToLock As String = Piece(ctl.ID, "^", 1)
        Dim RptTtlToLock As String = Piece(ctl.ID, "^", 2)
        MessageBox.Show("Are you sure you want to unlock the report?", "Unlock Report " & RptTtlToLock & " (" & RptToLock & ")", "UnlockReport " & RptToLock, Controls_Msgbox.Buttons.YesNo, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.No)
    End Sub
    Protected Sub btnDelete_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToDelete As String = Piece(ctl.ID, "@", 1)
        Dim RptTtlToDelete As String = Piece(ctl.ID, "@", 2)
        MessageBox.Show("Are you sure you want to delete the report?", "Delete Report " & RptTtlToDelete & " (" & RptToDelete & ")", "DeleteReport " & RptToDelete, Controls_Msgbox.Buttons.YesNo, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.No)
    End Sub

    Protected Sub btnCopy_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToCopy As String = Piece(ctl.ID, "~", 1)
        Dim RptTtlToCopy As String = Piece(ctl.ID, "~", 2)
        Dim NewReportID As String = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        Dim MainCaption As String = "Copy Report -- " & RptTtlToCopy & " (" & RptToCopy & ") -- To:"
        Dim dlgdata As New Controls_TextboxDlg.TextboxDlgData()

        dlgdata.TextboxData1 = New Controls_TextboxDlg.TextboxData("New Report Id:", NewReportID, True, False)
        dlgdata.TextboxData2 = New Controls_TextboxDlg.TextboxData("New Report Title:", "", True, True)
        dlgdata.Tag = ctl.ID
        dlgTextbox.MainTitleFontSize = 14
        dlgTextbox.Show("Copy Report", MainCaption, "Copy", dlgdata, " Copy Report ")
    End Sub
    Protected Sub btnEdit_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToEdit As String = Piece(ctl.ID, "`", 1)
        Dim RptTtlToEdit As String = Piece(ctl.ID, "`", 2)
        Response.Redirect("ReportEdit.aspx?repedit=yes&Report=" & RptToEdit)
    End Sub
    Protected Sub btnMap_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToEdit As String = Piece(ctl.ID, "#", 1)
        Dim RptTtlToEdit As String = Piece(ctl.ID, "#", 2)
        Response.Redirect("ReportEdit.aspx?repedit=yes&openmap=yes&Report=" & RptToEdit)
    End Sub
    Protected Sub btnAnalytics_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim Rpt As String = Piece(ctl.ID, "&", 1)
        Dim RptTtl As String = Piece(ctl.ID, "&", 2)
        Response.Redirect("ShowReport.aspx?srd=11&REPORT=" & Rpt)
    End Sub
    Protected Sub btnData_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim Rpt As String = Piece(ctl.ID, ";", 1)
        Dim RptTtl As String = Piece(ctl.ID, ";", 2)
        Response.Redirect("ShowReport.aspx?srd=0&REPORT=" & Rpt)
    End Sub
    Protected Sub btnCharts_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim Rpt As String = Piece(ctl.ID, "+", 1)
        Dim RptTtl As String = Piece(ctl.ID, "+", 2)
        Response.Redirect("ChartGoogleOne.aspx?Report=" & Rpt & "&x1=&x2=&y1=&fn=Count" & "&frm=ListOfReports")
    End Sub
    Protected Sub btnShow_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim RptToShow As String = Piece(ctl.ID, "*", 1)
        Dim RptTtlToShow As String = Piece(ctl.ID, "*", 2)
        Session("REPORTID") = RptToShow
        'Response.Redirect("ReportViews.aspx?see=yes")
        Response.Redirect("ReportViews.aspx?see=yes&srd=3")
    End Sub
    Protected Sub btnAI_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim Rpt As String = ctl.ID
        'Dim RptTtlToShow As String = Piece(ctl.ID, "*", 2)
        Session("REPORTID") = Rpt
        Response.Redirect("ShowReport.aspx?srd=15&Report=" & Rpt, False)
    End Sub
    Protected Sub btnDataAI_Click(sender As Object, e As System.EventArgs)
        Dim ctl As LinkButton = CType(sender, LinkButton)
        Dim Rpt As String = Piece(ctl.ID, "*", 2)
        'Dim RptTtlToShow As String = Piece(ctl.ID, "*", 2)
        Session("REPORTID") = Rpt
        Response.Redirect("ShowReport.aspx?srd=16&Report=" & Rpt, False)
    End Sub
    Protected Sub ButtonCreateReport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonCreateReport.Click
        Dim SQLq As String
        Dim dv5 As DataView
        Dim j As Integer
        Dim repttl As String = String.Empty
        Dim userdb As String = Session("UserDB").trim

        repttl = TextBoxNewReportTtl.Text
        If repttl = "" Then
            LabelMessage.Text = "Report Name should not be empty."
            trMessage.Visible = True
            TextBoxNewReportTtl.Focus()
            Return
        Else
            repid = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
            'repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "")
            repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")

            Session("REPORTID") = repid
            Session("REPTITLE") = repttl
            'If new report then create new row in ReportInfo table
            'If Session("REPEDIT") = "new" Then
            dv5 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")

            If dv5.Count = 0 OrElse dv5.Table Is Nothing OrElse dv5.Table.Rows.Count = 0 Then
                SQLq = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,Param9type,ReportDB) VALUES ('" & repid & "','" & repid & "','" & repttl & "','rdl','sql','portrait','" & userdb & "')"
                ExequteSQLquery(SQLq)
                'ReportEdit.LabelAlert.Text = "Report is created. If report has parameters and/or users please assign them now."
                Session("REPORTID") = repid
            Else
                LabelMessage.Text = "Report Name should be unique, please choose another one..."
                trMessage.Visible = True
                Exit Sub
            End If

            'no users yet: add creator as admin for this report

            If Session("REPORTID") <> "" And Session("logon") <> "" Then
                SQLq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & Session("REPORTID") & "','" & Session("REPTITLE") & "','" & Session("email") & "','admin','" & DateToString(Now) & "','" & Session("UserEndDate") & "','" & Session("logon") & "')"
                Dim ret As String = ExequteSQLquery(SQLq)
                WriteToAccessLog(Session("logon"), "Report " & Session("REPTITLE") & " (" & Session("REPORTID") & ") has been created.", 0)
                'Dim re As String = SendHTMLEmail("", "Report " & Session("REPTITLE") & " has been created", "Report " & Session("REPTITLE") & " (" & Session("REPORTID") & ") has been created by user " & Session("logon"), Session("email"), Session("SupportEmail"))
                'WriteToAccessLog(Session("logon"), re, 0)
            End If

        End If
        If Session("admin") = "super" Then
            Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=new")
        ElseIf Session("admin") = "admin" Then
            Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=new")
        End If

    End Sub
    'Protected Sub TextBoxNewReportID_TextChanged(sender As Object, e As EventArgs) Handles TextBoxNewReportID.TextChanged
    '    For i = 1 To list.Rows.Count
    '        Dim txt As String = list.Rows(i).Cells(3).InnerHtml
    '        list.Rows(i).Cells(3).InnerHtml = txt.Replace("'>copy</a>", "&newrep=" & TextBoxNewReportID.Text & "'>copy</a>")
    '    Next
    'End Sub

    'Private Sub TextBoxNewReportID_PreRender(sender As Object, e As EventArgs) Handles TextBoxNewReportID.PreRender
    '    For i = 1 To list.Rows.Count - 1
    '        Dim txt As String = list.Rows(i).Cells(3).InnerHtml
    '        list.Rows(i).Cells(3).InnerHtml = txt.Replace("'>copy</a>", "&newrep=" & TextBoxNewReportID.Text & "'>copy</a>")
    '    Next
    'End Sub

    'Private Sub ListOfReports_Unload(sender As Object, e As EventArgs) Handles Me.Unload
    '    For i = 1 To list.Rows.Count - 1
    '        Dim txt As String = list.Rows(i).Cells(3).InnerHtml
    '        list.Rows(i).Cells(3).InnerHtml = txt.Replace("'>copy</a>", "&newrep=" & TextBoxNewReportID.Text & "'>copy</a>")
    '    Next
    'End Sub
    Protected Sub TextBoxNewReportTtl_TextChanged(sender As Object, e As EventArgs) Handles TextBoxNewReportTtl.TextChanged
        Session("NewReportTtl") = TextBoxNewReportTtl.Text
    End Sub

    Private Sub TextBoxNewReportTtl_PreRender(sender As Object, e As EventArgs) Handles TextBoxNewReportTtl.PreRender
        Session("NewReportTtl") = TextBoxNewReportTtl.Text
    End Sub
    Private Sub btnCreate_Click(sender As Object, e As EventArgs) Handles btnCreate.Click
        Dim dlgdata As New Controls_TextboxDlg.TextboxDlgData()
        dlgdata.TextboxData1 = New Controls_TextboxDlg.TextboxData("Report Title:", "", True, True)
        Dim msg As String = "To create a new report enter the Report Title and click the ""Create Report"" button."
        dlgTextbox.Show("New Report", msg, "New", dlgdata, "Create Report")

        'trNewRepHelp.Visible = True
        'trReportTitle.Visible = True
        'trCreateButton.Visible = True
        'trPay1.Visible = False
        'If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then
        'trPay2.Visible = True
        'End if
        'TextBoxNewReportTtl.Focus()

    End Sub
    Private Sub CreateReport()
        Dim SQLq As String
        Dim dv5 As DataView
        Dim j As Integer
        Dim repttl As String = Session("REPTITLE")
        Dim userdb As String = Session("UserDB").trim
        Dim msg As String = String.Empty

        repid = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        'repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "")
        repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")

        Session("REPORTID") = repid
        dv5 = mRecords("SELECT * FROM [OURReportInfo] WHERE ([ReportID]='" & repid & "')")
        If dv5.Count = 0 OrElse
           dv5.Table Is Nothing OrElse
           dv5.Table.Rows.Count = 0 Then

            SQLq = "INSERT INTO [OURReportInfo] "
            SQLq &= "([ReportId],"
            SQLq &= "[ReportName],"
            SQLq &= "[ReportTtl],"
            SQLq &= "[ReportType],"
            SQLq &= "[ReportAttributes],"
            SQLq &= "[Param7type],"
            SQLq &= "[Param9type],"
            SQLq &= "[ReportDB])"
            SQLq &= "VALUES "
            SQLq &= "('" & repid & "',"
            SQLq &= "'" & repid & "',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'rdl',"
            SQLq &= "'sql',"
            SQLq &= "'user',"
            SQLq &= "'portrait',"
            SQLq &= "'" & userdb & "')"
            msg = ExequteSQLquery(SQLq)
            Session("REPORTID") = repid
        Else
            MessageBox.Show("Report ID should be unique, please enter another one...", "Create Report Error", "CreateError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Exit Sub
        End If
        If Session("REPORTID") <> "" And Session("logon") <> "" Then
            Dim ret As String = String.Empty
            SQLq = "INSERT INTO [OURReportView] ("
            SQLq &= "[ReportID],"
            SQLq &= "[ReportTemplate],"
            SQLq &= "[ReportTitle],"
            SQLq &= "[CreatedBy],"
            SQLq &= "[DateCreated],"
            SQLq &= "[UpdatedBy],"
            SQLq &= "[LastUpdate])"
            SQLq &= "VALUES ("
            SQLq &= "'" & repid & "',"
            SQLq &= "'Tabular',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'" & Session("logon") & "',"
            SQLq &= "'" & DateToString(Now) & "',"
            SQLq &= "'" & Session("logon") & "',"
            SQLq &= "'" & DateToString(Now) & "')"

            ret = ExequteSQLquery(SQLq)

            If ret <> "Query executed fine." Then
                DeleteReport(repid)
                MessageBox.Show(ret, "Create Report Error", "ReportViewError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Exit Sub
            End If

            SQLq = "INSERT INTO [OURPermissions] "
            SQLq &= "([NetId],"
            SQLq &= "[Application],"
            SQLq &= "[Param1],"
            SQLq &= "[Param2],"
            SQLq &= "[Param3],"
            SQLq &= "[AccessLevel],"
            SQLq &= "[OpenFrom],"
            SQLq &= "[OpenTo],"
            SQLq &= "[Comments])"
            SQLq &= "VALUES"
            SQLq &= "('" & Session("logon") & "',"
            SQLq &= "'InteractiveReporting',"
            SQLq &= "'" & repid & "',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'" & Session("email") & "',"
            SQLq &= "'admin',"
            SQLq &= "'" & DateToString(Now) & "',"
            SQLq &= "'" & Session("UserEndDate") & "',"
            SQLq &= "'" & Session("logon") & "')"

            ret = ExequteSQLquery(SQLq)

            If ret <> "Query executed fine." Then
                MessageBox.Show(ret, "Create Report Error", "PermissionsError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Exit Sub
            Else
                msg = "Report " & repttl & " (" & repid & ") has been created."
                WriteToAccessLog(Session("logon"), msg, 0)
                If Session("CSV") = "yes" AndAlso Session("admin") = "admin" Then
                    Session("AdvancedUser") = True
                    '    Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=new")
                    'ElseIf Session("admin") = "super" OrElse Session("admin") = "admin" Then
                    '    Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=new")
                End If
                Response.Redirect("SQLquery.aspx?tnq=0")
                'MessageBox.Show(msg, "Reports Created", "ReportCreated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.OK)
            End If
        End If
    End Sub
    'Private Sub DeleteReport(ReportID As String) 'moved to mFunctions
    '    If ReportID <> "" Then
    '        ExequteSQLquery("DELETE FROM OURReportShow WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURReportInfo WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURReportGroups WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURReportFormat WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURReportLists WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURReportChildren WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM ourdashboards WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM ourscheduledreports WHERE ReportID='" & ReportID & "'")
    '        ExequteSQLquery("DELETE FROM OURPermissions WHERE PARAM1='" & ReportID & "' AND APPLICATION='InteractiveReporting'")
    '        Response.Redirect("ListOfReports.aspx")
    '    End If
    'End Sub
    Private Sub LockReport(ReportID As String)
        If ReportID <> "" Then
            ExequteSQLquery("UPDATE OURReportInfo SET Param7type='standard' WHERE ReportID='" & ReportID & "'")
            Response.Redirect("ListOfReports.aspx")
        End If
    End Sub
    Private Sub UnlockReport(ReportID As String)
        If ReportID <> "" Then
            ExequteSQLquery("UPDATE OURReportInfo SET Param7type='user' WHERE ReportID='" & ReportID & "'")
            Response.Redirect("ListOfReports.aspx")
        End If
    End Sub
    Private Sub dlgTextbox_TextboxDlgResulted(sender As Object, e As Controls_TextboxDlg.TextboxDlgEventArgs) Handles dlgTextbox.TextboxDlgResulted
        If e.Result = Controls_TextboxDlg.TextboxDlgResult.OK Then
            Dim dlgdata As Controls_TextboxDlg.TextboxDlgData = e.DialogData
            Dim ret, tag, repnew, repnewttl, rep, repttl, sSql As String
            Dim bErr As Boolean = False
            Dim bCreatedByDesigner As Boolean = False

            Select Case e.Tag
                Case "New"
                    Session("REPTITLE") = e.DialogData.TextboxData1.Text
                    CreateReport()
                Case "Copy"
                    'Session("REPORTID") = e.DialogData.Tag
                    tag = e.DialogData.Tag
                    rep = Piece(tag, "~", 1)
                    repttl = Piece(tag, "~", 2)
                    repnew = e.DialogData.TextboxData1.Text
                    repnewttl = e.DialogData.TextboxData2.Text
                    Dim dri As DataTable = GetReportInfo(repnew)
                    If dri.Rows.Count > 0 Then
                        MessageBox.Show("Report ID " & repnew & " already exists. Report ID must be unique", "Copy Error", "CopyError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                        Exit Sub
                    End If
                    If FileInOURFiles(rep) Then
                        sSql = "SELECT Prop2 From OURFiles WHERE ReportId ='" & rep & "' AND Type = 'RDL'"
                        Dim dv As DataView = mRecords(sSql)
                        If dv.Count <> 0 AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                            Dim dt As DataTable = dv.Table
                            If Not IsDBNull(dt.Rows(0)("Prop2")) AndAlso dt.Rows(0)("Prop2").ToString = "designer" Then
                                bCreatedByDesigner = True
                            End If
                        End If

                    End If
                    ret = CreateCleanReportColumnsFieldsItems(rep, Session(" Thendbname"), Session("UserConnString"), Session("UserConnProvider"))
                    ret = CopyReport(rep, repttl, repnew, repnewttl, Session("logon"), bErr, Session("UserConnString"), Session("UserConnProvider"))
                    If bCreatedByDesigner Then
                        If FileInOURFiles(repnew) Then
                            sSql = "UPDATE OURFiles SET Prop2 = 'designer' WHERE ReportId='" & repnew & "' AND Type = 'RDL'"
                            ExequteSQLquery(sSql)
                        End If
                    End If
                    ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repnew, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                    If bErr Then
                        MessageBox.Show(ret, "Report Copy Error", "CopyError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                    Else
                        Session("REPORTID") = repnew
                        Session("REPTITLE") = repnewttl
                        MessageBox.OtherButtonText1 = "Return To List"
                        MessageBox.OtherButtonText2 = "Edit Report"
                        ret = repttl & " copied To " & repnewttl & " ."
                        MessageBox.Show(ret, "Report Copied", "CopySuccess-" & repnew, Controls_Msgbox.Buttons.OtherOther, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.Other2)

                    End If
            End Select
        End If
    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Select Case e.Tag
            Case "CreateError"
                Session("REPORTID") = ""
                Session("REPTITLE") = ""
                btnCreate_Click(Me, New EventArgs())
            'Case "ReportCreated"
                'If Session("CSV") = "yes" AndAlso Session("admin") = "admin" Then
                '    Session("AdvancedUser") = True
                '    Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=old")
                'ElseIf Session("admin") = "super" OrElse Session("admin") = "admin" Then
                '    Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=New")
                'End If
            Case Else
                If e.Tag.StartsWith("CopySuccess") Then
                    If e.Result = Controls_Msgbox.MessageResult.Other1 Then
                        Session("REPORTID") = ""
                        Session("REPTITLE") = ""
                        Response.Redirect("ListOfReports.aspx")
                    Else
                        If Session("REPORTID") = "" Then Session("REPORTID") = Piece(e.Tag, "-", 2)
                    If Session("admin") = "super" OrElse Session("admin") = "admin" Then
                        Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=yes")
                    End If
                End If
                End If
        End Select
        If e.Tag.StartsWith("DeleteReport ") AndAlso e.Result = Controls_Msgbox.MessageResult.Yes Then
            Dim repid As String = Piece(e.Tag, "DeleteReport ", 2)
            If repid <> String.Empty Then
                DeleteReport(repid)
                Response.Redirect("ListOfReports.aspx")
            End If
        End If
        If e.Tag.StartsWith("LockReport ") AndAlso e.Result = Controls_Msgbox.MessageResult.Yes Then
            Dim repid As String = Piece(e.Tag, "LockReport ", 2)
            If repid <> String.Empty Then
                LockReport(repid)
            End If
        End If
        If e.Tag.StartsWith("UnlockReport ") AndAlso e.Result = Controls_Msgbox.MessageResult.Yes Then
            Dim repid As String = Piece(e.Tag, "UnlockReport ", 2)
            If repid <> String.Empty Then
                UnlockReport(repid)
            End If
        End If
        If e.Tag.StartsWith("ListOfTables ") AndAlso e.Result = Controls_Msgbox.MessageResult.Yes Then
            Response.Redirect("ClassExplorer.aspx")
        End If
    End Sub

    Private Sub btnCancelCreate_Click(sender As Object, e As EventArgs) Handles btnCancelCreate.Click
        trNewRepHelp.Visible = False
        trReportTitle.Visible = False
        trCreateButton.Visible = False
        If Session("webinstall") = "OURweb" AndAlso Session("dbinstall") = "OURdb" Then
            trPay1.Visible = True
        End If
        trPay2.Visible = False
    End Sub
    'Protected Sub ButtonHelp_Click(sender As Object, e As EventArgs) Handles ButtonHelp.Click
    '    'OpenWordWithBookmak("DevSiteSupportHelp.doc", "Developer Site Support Help", "ClassData")
    '    'Response.Redirect("OnlineUserReporting.pdf#nameddest=Online User Reporting#Demo (MySQL database)#List Of Reports")
    '    'Response.Redirect("OnlineUserReporting.pdf#nameddest=Demo (MySQL database)")
    '    Response.Redirect("OnlineUserReporting.pdf#page=5")
    'End Sub

    Private Sub DropDownListConnStr_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListConnStr.SelectedIndexChanged
        Dim er As String = String.Empty
        Session("ReportDBforSuper") = DropDownListConnStr.Text
        Session("ReportDBProvforSuper") = GetProviderNameByConnString(Session("ReportDBforSuper").ToString.Trim, er)
        If txtDBUserID.Text.Trim = "" Then
            txtDBUserID.BorderColor = Color.Red
        ElseIf txtDBUserPass.Text.Trim = "" Then
            txtDBUserPass.BorderColor = Color.Red
        Else
            '"User ID=dbuserid; Password=dbpassword;"
            txtDBUserID.BorderColor = Color.Black
            txtDBUserID.BorderWidth = 1
            txtDBUserPass.BorderColor = Color.Black
            txtDBUserPass.BorderWidth = 1
            Session("UserConnString") = Session("ReportDBforSuper") & "User ID=" & txtDBUserID.Text.Trim & "; Password=" & txtDBUserPass.Text.Trim & ";"
        End If
        Session("DBUserID") = txtDBUserID.Text
        Session("DBUserPass") = txtDBUserPass.Text
    End Sub

    Private Sub LinkButtonHelpDesk_Click(sender As Object, e As EventArgs) Handles LinkButtonHelpDesk.Click
        'Dim applweb As String = ConfigurationManager.AppSettings("webour").ToString
        ''Dim lnkaddr As String = Session("WEBHELPDESK") & "HelpDesk.aspx?pass=help&rol=user&db=" & Session("dbname") & "&unit=" & Session("Unit") & "&ln=" & Session("logon")
        'Dim lnkaddr As String = applweb & "HelpDesk.aspx?pass=help&rol=user&db=" & Session("dbname") & "&unit=" & Session("Unit") & "&ln=" & Session("logon")
        Dim lnkaddr As String = "HelpDesk.aspx?pass=help&rol=user&db=" & Session("dbname") & "&unit=" & Session("Unit") & "&ln=" & Session("logon")
        Response.Redirect(lnkaddr)

    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Response.Redirect(url)
    End Sub

    Private Sub chkInitialReports_CheckedChanged(sender As Object, e As EventArgs) Handles chkInitialReports.CheckedChanged
        If chkInitialReports.Checked Then
            'show all reports with ReportId starting with dbname & "_"
            Session("ShowInitialReports") = "yes"
            Response.Redirect("ListOfReports.aspx")
        Else
            'hide all reports with ReportId starting with dbname & "_"
            Session("ShowInitialReports") = "no"
            Response.Redirect("ListOfReports.aspx")
        End If

    End Sub

    Private Sub lnkRedoInitialReports_Click(sender As Object, e As EventArgs) Handles lnkRedoInitialReports.Click
        Dim er As String = String.Empty
        Dim ret As String = String.Empty

        'Dim redo As Boolean = True
        ''overkill
        'If redo Then
        '    Dim msql As String = String.Empty
        '    'delete Join Reports
        '    msql = "DELETE FROM OURReportInfo WHERE ReportId Like '" & Session("dbname") & "_%'"
        '    ret = ExequteSQLquery(msql)
        '    'delete Join Permissions
        '    msql = "DELETE FROM OURPermissions WHERE Param1 LIKE '" & Session("dbname") & "_%'"
        '    ret = ExequteSQLquery(msql)
        '    'delete sql query fields
        '    msql = "DELETE FROM OURReportSQLquery WHERE ReportId LIKE '" & Session("dbname") & "_%'"
        '    ret = ExequteSQLquery(msql)
        'End If

        If Session("admin") = "super" OrElse Session("Access") = "SITEADMIN" Then
            'Dim rtn As String = MakeInitialReports(Session("logon"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er, False, True)
            Dim rtn As String = MakeInitialReports(Session("logon"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er, True, True)

        Else
            Dim rtn As String = MakeInitialReports(Session("logon"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er, True)
        End If
        Response.Redirect("ListOfReports.aspx")
    End Sub
    'Private Sub btnListOfTables_Click(sender As Object, e As EventArgs) Handles btnListOfTables.Click
    '    Response.Redirect("ListOfTables.aspx")
    'End Sub

    Private Sub btnListOfClasses_Click(sender As Object, e As EventArgs) Handles btnListOfClasses.Click
        If Session("admin") = "super" OrElse Session("admin") = "admin" Then
            Response.Redirect("ClassExplorer.aspx")
        End If
    End Sub

    Private Sub btnFriendlyNames_Click(sender As Object, e As EventArgs) Handles btnFriendlyNames.Click
        Response.Redirect("FriendlyNames.aspx")
    End Sub

    'Private Sub btnListOfJoins_Click(sender As Object, e As EventArgs) Handles btnListOfJoins.Click
    '    Response.Redirect("ListOfJoins.aspx")
    'End Sub

    Private Sub btnShowList_Click(sender As Object, e As EventArgs) Handles btnShowList.Click
        Dim er As String = String.Empty
        If txtDBUserID.Text.Trim = "" Then
            txtDBUserID.BorderColor = Color.Red
            txtDBUserID.BorderWidth = 3
        ElseIf txtDBUserPass.Text.Trim = "" Then
            txtDBUserPass.BorderColor = Color.Red
            txtDBUserPass.BorderWidth = 3
        Else
            txtDBUserID.BorderColor = Color.Black
            txtDBUserID.BorderWidth = 1
            txtDBUserPass.BorderColor = Color.Black
            txtDBUserPass.BorderWidth = 1
            Session("UserConnString") = Session("ReportDBforSuper") & "User ID=" & txtDBUserID.Text.Trim & "; Password=" & txtDBUserPass.Text.Trim & ";"
            Session("UserConnProvider") = GetProviderNameByConnString(Session("ReportDBforSuper").ToString.Trim, er)
            Session("ShowList") = "yes"
            Session("DBUserID") = txtDBUserID.Text
            Session("DBUserPass") = txtDBUserPass.Text
            Response.Redirect("ListOfReports.aspx")
        End If
        'Session("DBUserID") = txtDBUserID.Text
        'Session("DBUserPass") = txtDBUserPass.Text
    End Sub

    Private Sub btnDataImport_Click(sender As Object, e As EventArgs) Handles btnDataImport.Click
        Response.Redirect("DataImport.aspx")
    End Sub

    Private Sub chkAdvanced_PreRender(sender As Object, e As EventArgs) Handles chkAdvanced.PreRender
        If chkAdvanced.Checked Then
            Session("AdvancedUser") = True
            btnDataImport.Visible = True
            btnDataImport.Enabled = True
        Else
            Session("AdvancedUser") = False
            btnDataImport.Visible = False
            btnDataImport.Enabled = False
        End If
    End Sub

    Private Sub lnkHideBadReports_Click(sender As Object, e As EventArgs) Handles lnkHideBadReports.Click
        Dim ret As String = HideCorruptedReports()
        Response.Redirect("ListOfReports.aspx")
    End Sub
    Private Sub lnkUnHideBadReports_Click(sender As Object, e As EventArgs) Handles lnkUnHideBadReports.Click
        Dim ret As String = UnHideCorruptedReports()
        Response.Redirect("ListOfReports.aspx")
    End Sub
    Public Function HideCorruptedReports() As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim dvd As DataView
        If Session("dvListOfReports") Is Nothing Then
            Return ""
        End If
        Try
            Dim dvl As DataView = Session("dvListOfReports")
            If dvl IsNot Nothing AndAlso dvl.Table.Rows.Count > 0 Then
                For i = 0 To dvl.Table.Rows.Count - 1
                    er = ""
                    dvd = RetrieveReportData(dvl.Table.Rows(i)("ReportID"), "", False, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), er)
                    If er <> "" Then
                        ret = ExequteSQLquery("UPDATE OurReportInfo SET Param1type='1' WHERE ReportID='" & dvl.Table.Rows(i)("ReportID") & "' AND ReportDB LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%'")
                    End If
                Next
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function UnHideCorruptedReports() As String
        Dim ret As String = String.Empty
        If Session("dvListOfReports") Is Nothing Then
            Return ""
        End If
        Try
            Dim dvl As DataView = Session("dvListOfReports")
            If dvl IsNot Nothing AndAlso dvl.Table.Rows.Count > 0 Then
                For i = 0 To dvl.Table.Rows.Count - 1

                    ret = ExequteSQLquery("UPDATE OurReportInfo SET Param1type='0' WHERE ReportID='" & dvl.Table.Rows(i)("ReportID") & "' AND ReportDB LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%'")

                Next
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
End Class
