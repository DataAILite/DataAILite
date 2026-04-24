Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
'Imports InterSystems.Data.CacheClient
Imports Microsoft.Web.Administration
Partial Class _Default
    Inherits System.Web.UI.Page
    Public myConnection As SqlConnection
    Public myConnect As SqlConnection
    Public myConn As SqlConnection
    Public myConnt As SqlConnection
    Public empID As String
    Private Sub _Default_Init(sender As Object, e As EventArgs) Handles Me.Init
        ' DataAI - AI-driven data analysis system
        ' Copyright (C) 2026 Yanbor LLC
        '
        ' This program is free software: you can redistribute it and/or modify
        ' it under the terms of the GNU General Public License as published by
        ' the Free Software Foundation, either version 3 of the License, or
        ' (at your option) any later version.
        '
        ' This program is distributed in the hope that it will be useful,
        ' but WITHOUT ANY WARRANTY; without even the implied warranty of
        ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
        ' See the GNU General Public License for more details.
        '
        ' You should have received a copy of the GNU General Public License
        ' along with this program. If not, see <https://www.gnu.org/licenses/>.
        Session("dvListOfReports") = Nothing
        Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        If Session("OURConnProvider") = "Sqlite" Then
            If Session("logon") IsNot Nothing AndAlso Session("logon").ToString.Trim <> "" Then
                'clean up Temp, RDLFILES, XSDFILES, KLMS, ImageFiles folders from files starts with Session("logon")
                Dim ret As String = CleanFolders(Session("logon"))
            End If
            Session("logon") = ""
            Session("Email") = ""
            Session("txtURI") = ""
            Session("OpenAIkey") = ""
            Session("OpenAIOrganization") = ""
            Session("OpenAIurl") = ""
            Session("OpenAImodel") = ""
            Session("maxTokens") = ""


            Response.Redirect("DataAIaddons.aspx")
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'check if ourdb exists
        'ourdb
        Dim ourdb As String = String.Empty
        Dim ourdbstr As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString
        Dim ourdbpr As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString
        Dim er As String = String.Empty
        If Not DatabaseConnected(ourdbstr, ourdbpr, er) Then
            'create new empty ourdb
            Dim ourdbcase As String = ConfigurationManager.AppSettings("ourdbcase").ToString.Trim
            er = CreateNewOURdbOnNewServer(ourdbstr, ourdbpr, ourdbcase)
            If er.Contains("ERROR!!") Then
                MessageBox.Show("Creation of the operational database with connection string from the web.config crashed! Create the empty operational database with connection string from the web.config manually and try running this page again.", "The operational database is not created - Error", "NoDatabaseError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Exit Sub
            End If
            'Else
        End If
        'updated our database to current version
        Try
            er = UpdateOURdbToCurrentVersion(ourdbstr, ourdbpr)
        Catch ex As Exception
            er = "ERROR!! " & ex.Message
        End Try


        'create new folders if needed
        Dim applpath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim folderPath As String = applpath & "\KMLS"
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If
        folderPath = applpath & "\RDLFILES"
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If
        folderPath = applpath & "\Temp"
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If
        folderPath = applpath & "\XSDFILES"
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If
        folderPath = applpath & "\ImageFiles"
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If
        folderPath = applpath & "\Controls\Images\ReportDesignerImages"
        If Not Directory.Exists(folderPath) Then
            Directory.CreateDirectory(folderPath)
        End If
        If Not File.Exists(folderPath & "\reportdesignerimage.ico") Then
            Try
                'download the standard reportdesignerimage.ico file from the OUReports to this directory folderPath
                Dim client As New WebClient()
                client.DownloadFile(New Uri("https://oureports.net/OUReports/Controls/Images/ReportDesignerImages/reportdesignerimage.ico"), folderPath & "\reportdesignerimage.ico")
            Catch ex As Exception
                er = ex.Message
            End Try
        End If
        If Request("payourdbst") IsNot Nothing AndAlso Request("payourdbst").ToString.ToUpper.Trim = "COMPLETED" Then
            If DatabaseConnected(ourdbstr, ourdbpr, er) Then
                ''create db tables
                'Try
                '    er = UpdateOURdbToCurrentVersion(ourdbstr, ourdbpr)
                'Catch ex As Exception
                '    er = "ERROR!! " & ex.Message
                'End Try
                If Not er.Contains("ERROR!!") Then
                    'success!
                    Response.Redirect("Default.aspx")
                Else
                    MessageBox.Show("Creation of the operational database with connection string from the web.config completed with errors! Do not close the page! Contact your database administrator, check permissions and try running this page again.", "The operational database is not created - Error", "NoDatabaseError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                    Exit Sub
                End If
            Else
                MessageBox.Show("Creation of the operational database with connection string from the web.config crashed! Do not close the page! Create the empty operational database with connection string from the web.config manually and refresh this page!", "The operational database is not created - Error", "NoDatabaseError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Exit Sub
            End If
        Else
            If Not TableExists("ourpermits", ourdbstr, ourdbpr, er) Then
                MessageBox.Show("Creation of the operational database with connection string from the web.config completed with errors! Do not close the page! Contact your database administrator, check permissions and try running this page again.", "The operational database is not created - Error", "NoDatabaseError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Exit Sub
            End If
        End If
        'DeleteTestUsers()
        Dim re As String = SendEmailAdminScheduledReports()
        'if from Default.aspx?StopIt=yes
        If Request("StopIt") IsNot Nothing AndAlso Request("StopIt").ToString.Trim = "yes" Then
            Exit Sub
            'clouse app window
        End If
        Session("SortedView") = Nothing
        Session("SortColumn") = ""
        Session("admin") = ""
        Session("logon") = ""
        Session("Application") = ""
        Session("OURConnString") = ""
        Session("OURConnProvider") = ""
        Session("UserConnString") = ""
        Session("UserConnProvider") = ""
        Session("DBUserID") = ""
        Session("DBUserPass") = ""
        Session("ReportDBforSuper") = ""
        Session("ReportDBProvforSuper") = ""
        Session("ChangePassword") = False
        Session("tn") = 0
        Session("UserDB") = ""
        Session("admin") = ""
        Session("Unit") = ""
        Session("FromSite") = ""
        Session("CSV") = ""
        Session("AdvancedUser") = False
        Session("ShowInitialReports") = "no"
        Session("syschk") = False
        Session("spam") = ""
        Session("ip") = ""
        Session("Groups") = ""
        Session("urlback") = ""
        Session("email") = ""
        Session("ShowList") = ""
        Dim url As String = HttpContext.Current.Request.Url.AbsoluteUri
        Session("URL") = url.Substring(0, url.LastIndexOf("/")) & "/"
        Session("UnitWEB") = ""
        Dim SiteFor As String = ConfigurationManager.AppSettings("SiteFor").ToString
        Dim unit As String = ConfigurationManager.AppSettings("unit").ToString
        Session("Unit") = unit
        Session("UnitEndDate") = ConfigurationManager.AppSettings("unitenddate").ToString
        LabelVersion.Text = "Version " & ConfigurationManager.AppSettings("version").ToString '& " - " & SiteFor
        Session("Version") = LabelVersion.Text.Trim
        LblInvalid.Text = SiteFor
        Session("WEBOUR") = ConfigurationManager.AppSettings("webour").ToString
        Session("WEBOURPAID") = ConfigurationManager.AppSettings("webour").ToString & "Paid.aspx"
        Session("WEBHELPDESK") = ConfigurationManager.AppSettings("webhelpdesk").ToString
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        Session("SupportEmail") = ConfigurationManager.AppSettings("supportemail").ToString
        Session("webinstall") = ConfigurationManager.AppSettings("webinstall").ToString
        Session("dbinstall") = ConfigurationManager.AppSettings("dbinstall").ToString
        Session("nruntimes") = 0
        Session("ntimerticks") = 0

        If Not ConfigurationManager.AppSettings("UnitAuthenticate") Is Nothing Then
            Session("UnitAuthenticate") = ConfigurationManager.AppSettings("UnitAuthenticate").ToString
        Else
            Session("UnitAuthenticate") = "NO"
        End If
        Head1.Title = Session("PAGETTL")
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        Dim ret As String = String.Empty

        Dim pw As String = String.Empty
        ' check if ip block request
        If Request("spam") IsNot Nothing AndAlso Request("spam") = "yes" AndAlso Request("ip") IsNot Nothing AndAlso Request("ip") <> String.Empty Then

            If IsFilterDefined(Request("ip")) Then
                MessageBox.Show("IP Filter is already defined!", "Block Emails From " & Request("ip"), "spam", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
                Exit Sub
            Else
                Session("spam") = Request("spam")
                Session("ip") = Request("ip")
                pw = Now.ToShortDateString
                Session("pass") = pw
                Session("logon") = "super"
            End If

        End If

        'check if this is scheduled or shared report
        Session("sched") = ""
        Session("shared") = ""
        Session("map") = ""   '????
        '"default.aspx?srd=6&sched=yes&rep=" & repid & "&reid=" & indf & "&rundate=" & rundate
        If Not Request("rundate") Is Nothing AndAlso Not Request("sched") Is Nothing AndAlso Request("sched").ToString.Trim = "yes" AndAlso Not Request("srd") Is Nothing AndAlso Not Request("rep") Is Nothing AndAlso Not Request("reid") Is Nothing AndAlso Not Request("rundate") Is Nothing AndAlso IsNumeric(Request("srd")) Then
            'check if current date is rundate
            Dim rundate As String = Request("rundate").ToString.Trim
            'If DateToString(Now()).Substring(0, 10) <> rundate.Substring(0, 10) AndAlso DateToString(Now.AddDays(-1)).Substring(0, 10) <> rundate.Substring(0, 10) Then
            If (DateToString(Now()).Substring(0, 7) <> rundate.Substring(0, 7)) And (DateToString(Now.AddDays(-1)).Substring(0, 10) <> rundate.Substring(0, 10)) Then
                'MessageBox.Show("Report expired and is not available any more for downloading from email. ", "Scheduled Reports", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                LblInvalid.Text = "Report expired and is not available any more for downloading from email. Reports are available for current month only. "
                LblInvalid.Font.Bold = True
                Exit Sub
            End If

            Session("srd") = CInt(Request("srd"))
            Session("sched") = "yes"
            Session("REPORTID") = Request("rep").ToString.Trim
            Session("admin") = "user"
            Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Session("Application") = "InteractiveReporting"
            Session("AdvancedUser") = False
            Session("lgn") = ""
            Session("lgn") = GetReportIdentifier(Session("REPORTID"))
            Session("logon") = Session("lgn")

            'Dim er As String = String.Empty
            Dim sqls As String = "SELECT * FROM  [OURScheduledReports] WHERE [Deadline] LIKE '" & rundate & "%' AND [Prop3]='" & Request("reid").ToString.Trim & "' AND [ReportId]='" & Request("rep").ToString.Trim & "'"
            Dim dts As DataView = mRecords(sqls, er)
            If dts Is Nothing OrElse dts.Count = 0 Then
                'MessageBox.Show("Report expired and is not available any more for downloading from email. ", "Scheduled Reports", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                LblInvalid.Text = "Report is not found or is not available for downloading from email. "
                LblInvalid.Font.Bold = True
                Exit Sub
            End If
            Try
                If dts.Table.Rows(0)("Prop4").ToString.Trim <> "" Then
                    Session("pagewidth") = Piece(dts.Table.Rows(0)("Prop4").ToString, "~", 1)
                    Session("pageheight") = Piece(dts.Table.Rows(0)("Prop4").ToString, "~", 2)
                End If
            Catch ex As Exception

            End Try
            Session("filter") = dts.Table.Rows(0)("Filters").Replace("""", "'")
            Session("WhereText") = dts.Table.Rows(0)("WhereText").Replace("""", "'")
            'extract parameters from WhereText will be done in RetrieveReportData

            Try
                WriteToAccessLog("sched", "Login successful for report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 0)
                Dim ri As DataTable = GetReportInfoWithParam(Session("lgn"), Session("REPORTID"))
                If ri.Rows.Count = 1 AndAlso ri.Rows(0)("Param1id").trim <> "" Then
                    If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection") Is Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'user db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("userdbcase").ToString.Trim
                    ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'our csv db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("csvdbcase").ToString.Trim
                    Else
                        'demo
                        If ri.Rows(0)("ReportDB").ToString.Contains("DEMOdata") Then
                            'Session("UserConnString") = ri.Rows(0)("ReportDB").ToString.Trim & " User ID=DEMOdata; Password=Demo!@#4"
                            Session("UserConnString") = Session("OURConnString").Replace("OURdata", "DEMOdata")
                            Session("UserConnProvider") = Session("OURConnProvider")
                            DataModule.userdbcase = ConfigurationManager.AppSettings("ourdbcase").ToString.Trim
                        Else
                            Exit Try
                        End If
                    End If
                    'check connection to user database
                    'Dim er As String = String.Empty
                    If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider"), er) Then
                        WriteToAccessLog("sched", "Connection to database open  for report " & Session("REPORTID") & " with lgn=" & Session("lgn") & " on " & Now().ToString, 1)
                        Response.Redirect("ListOfReports.aspx")
                    Else
                        WriteToAccessLog("sched", "Connection to database cannot be open  for report " & Session("REPORTID") & " with lgn=" & Session("lgn") & " on " & Now().ToString, 1)
                        Exit Try
                    End If
                End If
            Catch ex As Exception
                ret = ex.Message
                WriteToAccessLog(Session("lgn"), "Error!! scheduled report" & Session("REPORTID") & ": " & ret, 1)
            End Try
        End If

        'check if this is shared or from google map
        '"default.aspx?srd=3&map=yes&rep=" & repid & "&lgn=" & lgn
        If Not Request("map") Is Nothing AndAlso Not Request("srd") Is Nothing AndAlso Not Request("rep") Is Nothing AndAlso Not Request("lgn") Is Nothing AndAlso Request("map").ToString.Trim = "yes" AndAlso IsNumeric(Request("srd")) Then
            Session("srd") = CInt(Request("srd"))
            Session("map") = "yes"
            Session("REPORTID") = Request("rep").ToString.Trim
            Session("lgn") = ""
            Session("lgn") = Request("lgn").ToString.Trim
            Session("lgn") = GetReportIdentifier(Session("REPORTID"))
            Session("logon") = Session("lgn")
            Session("admin") = "user"
            Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Session("Application") = "InteractiveReporting"
            Session("AdvancedUser") = False
            If Not Request("flt") Is Nothing AndAlso Request("flt").ToString.Trim <> "" Then
                Session("filter") = Request("flt").Replace("~", "=").Replace("^", " AND ").Replace("*", "'")
            End If
            Try
                WriteToAccessLog("map", "Login successful for report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 0)
                Dim ri As DataTable = GetReportInfoWithParam(Session("lgn"), Session("REPORTID"))
                If ri.Rows.Count = 1 AndAlso ri.Rows(0)("Param1id").trim <> "" Then
                    If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection") Is Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'user db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("userdbcase").ToString.Trim
                    ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'our csv db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("csvdbcase").ToString.Trim
                    Else
                        'demo
                        If ri.Rows(0)("ReportDB").ToString.Contains("DEMOdata") Then
                            'Session("UserConnString") = ri.Rows(0)("ReportDB").ToString.Trim & " User ID=DEMOdata; Password=Demo!@#4"
                            Session("UserConnString") = Session("OURConnString").Replace("OURdata", "DEMOdata")
                            Session("UserConnProvider") = Session("OURConnProvider")
                            DataModule.userdbcase = ConfigurationManager.AppSettings("ourdbcase").ToString.Trim
                        Else
                            Exit Try
                        End If
                    End If
                    'check connection to user database
                    'Dim er As String = String.Empty
                    If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider"), er) Then
                        WriteToAccessLog("map", "Connection to database open  for report " & Session("REPORTID") & " with lgn=" & Session("lgn") & " on " & Now().ToString, 1)
                        Response.Redirect("ListOfReports.aspx")
                    Else
                        WriteToAccessLog("map", "Connection to database cannot be open  for report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 1)
                        Exit Try
                    End If
                End If
            Catch ex As Exception
                ret = ex.Message
                WriteToAccessLog("map", "Error!! " & ret & " - connecting to the report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 1)
            End Try
        End If
        If Not Request("shared") Is Nothing AndAlso Not Request("srd") Is Nothing AndAlso Not Request("rep") Is Nothing AndAlso Not Request("lgn") Is Nothing AndAlso Request("shared").ToString.Trim = "yes" AndAlso IsNumeric(Request("srd")) Then
            Session("srd") = CInt(Request("srd"))
            'Session("map") = "yes"
            Session("REPORTID") = Request("rep").ToString.Trim
            Session("lgn") = ""
            Session("lgn") = Request("lgn").ToString.Trim
            'Session("lgn") = GetReportIdentifier(Session("REPORTID"))
            Session("logon") = Session("lgn")
            Session("admin") = "user"
            Session("shared") = "yes"
            Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Session("Application") = "InteractiveReporting"
            Session("AdvancedUser") = False
            If Not Request("flt") Is Nothing AndAlso Request("flt").ToString.Trim <> "" Then
                Session("filter") = Request("flt").Replace("~", "=").Replace("^", " AND ").Replace("*", "'")
            End If
            Try
                WriteToAccessLog("shared", "Login successful for report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 0)
                Dim ri As DataTable = GetReportInfoWithParam(Session("lgn"), Session("REPORTID"))
                If ri.Rows.Count = 1 AndAlso ri.Rows(0)("Param1id").trim <> "" Then
                    If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection") Is Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'user db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("userdbcase").ToString.Trim
                    ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'our csv db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("csvdbcase").ToString.Trim
                    Else
                        'demo
                        If ri.Rows(0)("ReportDB").ToString.Contains("DEMOdata") Then
                            'Session("UserConnString") = ri.Rows(0)("ReportDB").ToString.Trim & " User ID=DEMOdata; Password=Demo!@#4"
                            Session("UserConnString") = Session("OURConnString").Replace("OURdata", "DEMOdata")
                            Session("UserConnProvider") = Session("OURConnProvider")
                            DataModule.userdbcase = ConfigurationManager.AppSettings("ourdbcase").ToString.Trim
                        Else
                            Exit Try
                        End If
                    End If
                    'check connection to user database
                    'Dim er As String = String.Empty
                    If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider"), er) Then
                        WriteToAccessLog("map", "Connection to database open  for report " & Session("REPORTID") & " with lgn=" & Session("lgn") & " on " & Now().ToString, 1)
                        Response.Redirect("ListOfReports.aspx")
                    Else
                        WriteToAccessLog("map", "Connection to database cannot be open  for report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 1)
                        Exit Try
                    End If
                End If
            Catch ex As Exception
                ret = ex.Message
                WriteToAccessLog("map", "Error!! " & ret & " - connecting to the report " & Session("REPORTID") & " with lgn=" & Session("lgn"), 1)
            End Try
        End If
        Session("dash") = "no"
        Session("dashboard") = ""
        If Not Request("dash") Is Nothing AndAlso Not Request("srd") Is Nothing AndAlso Not Request("lgn") Is Nothing AndAlso Request("dash").ToString.Trim = "yes" AndAlso IsNumeric(Request("srd")) AndAlso CInt(Request("srd")) = 30 Then
            Session("srd") = CInt(Request("srd"))
            Session("dash") = "yes"
            Session("lgn") = ""
            Session("lgn") = Request("lgn").ToString.Trim
            Session("logon") = Session("lgn")
            Session("admin") = "user"
            Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Session("Application") = "InteractiveReporting"
            Session("AdvancedUser") = False
            Try
                Dim di As DataView = mRecords("SELECT * FROM [OURDashboards] WHERE [Prop6]='" & Session("lgn") & "'")
                If di Is Nothing OrElse di.Table.Rows.Count = 0 Then
                    MessageBox.Show("Dashboard is not found", "Page Load ", "Logon", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                    Exit Sub
                End If
                WriteToAccessLog("dash", "Login successful for dashboard with lgn=" & Session("lgn"), 0)
                repid = di.Table.Rows(0)("ReportID").ToString
                Session("dashboard") = di.Table.Rows(0)("Dashboard").ToString
                Dim ri As DataTable = GetReportInfo(repid)
                If ri.Rows.Count = 1 Then
                    If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection") Is Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'user db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("UserSqlConnection").ProviderName.ToString
                    ElseIf System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection") IsNot Nothing AndAlso System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString.Trim.StartsWith(ri.Rows(0)("ReportDB").ToString.Trim) Then
                        'our csv db
                        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
                        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
                    Else
                        'demo
                        If ri.Rows(0)("ReportDB").ToString.Contains("DEMOdata") Then
                            'Session("UserConnString") = ri.Rows(0)("ReportDB").ToString.Trim & " User ID=DEMOdata; Password=Demo!@#4"
                            Session("UserConnString") = Session("OURConnString").Replace("OURdata", "DEMOdata")
                            Session("UserConnProvider") = Session("OURConnProvider")

                        End If
                    End If
                    'check connection to user database
                    'Dim er As String = String.Empty
                    If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider"), er) Then
                        Response.Redirect("Dashboard.aspx?dash=yes&Prop6=" & Session("logon") & "&dashboard=" & Session("dashboard"))
                    Else
                        WriteToAccessLog("dash", "Connection to database cannot be open  for dashboard with lgn=" & Session("lgn"), 1)
                        Exit Try
                    End If
                End If
            Catch ex As Exception
                ret = ex.Message
                WriteToAccessLog("dash", "Error!! Connection to database cannot be open  for dashboard with lgn=" & Session("lgn"), 1)
            End Try
        End If

        If Session("sched") Is Nothing OrElse Session("sched").ToString.Trim <> "yes" Then
            Session("srd") = 0
        End If
        Session("map") = ""


        If Not Request("msg") Is Nothing AndAlso Request("msg") = "DemoUserNotChanged" Then
            ret = "Demo user should Not be changed!"
        End If

        If Not Request("msg") Is Nothing AndAlso Request("msg") = "SessionExpired" Then
            ret = "Session Expired ..."
        End If

        If Not Request("msg") Is Nothing AndAlso Request("msg") = "WrongLogonPassword" Then
            ret = "Wrong Logon Or Password ..."
        End If
        If Not Request("msg") Is Nothing AndAlso Request("msg") = "ipblocked" Then
            ret = "Email messages from " & Request("ipaddr") & " are now blocked."
        End If

        If Not Request("msg") Is Nothing AndAlso Request("msg") = "ipblockerr" Then
            ret = "An error was generated when trying to block " & Request("ipaddr") & ":" & Request("err")
        End If


        If Not Session("PasswordChanged") Is Nothing AndAlso Session("PasswordChanged") <> "" Then
            ret = "Password has been changed ..."
            Session("PasswordChanged") = ""
        End If
        If Not Request("pass") Is Nothing Then
            'assign rights and redirect to HelpDesk
            pw = Request("pass")
            If Request("pass") = "help" AndAlso Not Request("ln") Is Nothing AndAlso Request("ln").Trim <> "" Then
                btLogin_Click(sender, e)
            End If
        End If
        'If Session("map") = "yes" Then
        '    'Session("logon") = "csvuser"
        '    'Session("pass") = "test"
        '    Session("logon") = Request("lgn")
        '    Session("pass") = Request("psw")
        '    pw = Session("pass")
        '    btLogin_Click(sender, e)
        'End If
        If Not Request("logon") Is Nothing AndAlso Request("logon").Trim <> "" Then
            Session("logon") = cleanText(Request("logon"))
            Logon.Text = Session("logon")
            Pass.Focus()
        End If
        If Not Request("tn") Is Nothing AndAlso Request("tn").Trim <> "" Then
            Session("tn") = CInt(cleanText(Request("tn"))).ToString
        End If

        If pw <> "" AndAlso Session("spam") = String.Empty Then
            SetPassword("Pass", pw)
        End If
        If Not IsPostBack Then
            Logon.Focus()
        End If
        If ret <> String.Empty Then _
            MessageBox.Show(ret, "Page Load ", "Logon", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

        If Session("logon") = "demo" AndAlso pw = "demo" Then
            btLogin_Click(sender, e)
        End If
        If Session("logon") = "csvdemo" OrElse Session("spam") = "yes" Then
            btLogin_Click(sender, e)
        End If
    End Sub

    Private Function ToAccessLog(ByVal logon As String, ByVal actn As String, ByVal cnt As Integer, constr As String, conprov As String) As String
        Dim ret As String = String.Empty
        actn = cleanText(actn)
        Dim ipaddr As String = GetIPAddress()
        If ipaddr.Trim <> "" AndAlso ipaddr <> "::1" Then logon = logon & " at " & ipaddr
        Try
            Dim EventDate As String = DateToString(DateTime.Now, "", False)
            If IsCacheDatabase() Then
                ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,Logon,Action,""Count"") VALUES('" & EventDate & "','" & logon & "','" & actn & "'," & cnt.ToString & ")", constr, conprov)
            Else
                ret = ExequteSQLquery("INSERT INTO OURAccessLog (EventDate,Logon,Action,[Count]) VALUES('" & EventDate & "','" & logon & "','" & actn & "'," & cnt.ToString & ")", constr, conprov)
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Private Sub DeleteUser(indx As String)
        Dim constring As String = " Server=92.204.144.76 ; Database=OURdata; User ID=root; Password=iy1001IY!;"
        Dim conprv As String = "MySql.Data.MySqlClient"
        Dim sql As String = "SELECT * FROM OURPermits WHERE Indx ='" & indx & "'"
        Dim Err As String = ""
        Dim dt As DataTable = mRecords(sql, Err, constring, conprv).ToTable
        Dim sUserId As String
        Dim sRole As String
        Dim sStartReport As String
        Dim sReportID As String
        Dim ret As String

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            sUserId = dt.Rows(0)("NetId").ToString.Trim
            sRole = dt.Rows(0)("RoleApp").ToString.Trim
            sStartReport = sUserId & indx & "_"
            sStartReport = sStartReport.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")
            If sRole.ToUpper <> "SUPER" Then
                sql = "SELECT * FROM OURPermissions WHERE NetId = '" & sUserId & "' AND param1 like '" & sStartReport & "%'"
                dt = mRecords(sql, Err, constring, conprv).Table
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For i As Integer = 0 To dt.Rows.Count - 1
                        sReportID = dt.Rows(i)("param1")
                        DeleteReport(sReportID, constring, conprv)
                        'ret = ExequteSQLquery("DELETE FROM OURFiles WHERE ReportID='" & sReportID & "'")
                    Next
                    'ret = ExequteSQLquery("DELETE FROM OURUserTables WHERE UserID ='" & sUserId & "'", constring, conprv)
                    ret = ExequteSQLquery("DELETE FROM OURPermissions WHERE NetId = '" & sUserId & "' AND param1 like '" & sStartReport & "%'", constring, conprv)
                End If
                ret = ExequteSQLquery("DELETE FROM OURPermits WHERE Indx='" & indx & "'", constring, conprv)

                'Delete accesslog entries for this user
                sql = "DELETE FROM OURAccessLog WHERE Logon LIKE '" & sUserId & "%'"
                ret = ExequteSQLquery(sql, constring, conprv)

                'sql = "SELECT Logon FROM OURAccessLog WHERE Logon LIKE '" & sUserId & "%'"
                'dt = mRecords(sql, Err, constring, conprv).Table
                'Dim sLogon As String = ""
                'If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                '    sLogon = dt.Rows(0)("Logon")
                'End If
            End If
        End If
    End Sub
    Private Sub DeleteTestUsers()
        Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
        Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString

        Dim sCurDate As String = Format(Today, "yyyy-MM-dd") & " 00:00:00"
        Dim sql As String = "SELECT * FROM OURPermits WHERE NetId LIKE 'test%'  AND NetId Like '%@test.com' AND StartDate < '" & sCurDate & "'"
        Dim er As String = ""
        Dim dt As DataTable = mRecords(sql, er, Session("OURConnString"), Session("OURConnProvider")).ToTable
        Dim Indx As String = ""
        Dim dr As DataRow = Nothing
        Dim msg As String = ""
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                Indx = dr("Indx").ToString
                DeleteUser(Indx)
            Next
            msg = dt.Rows.Count.ToString & " test users, along with any reports or access logs they created, have been deleted."
            ToAccessLog("DeleteTestUsers", msg, 0, Session("OURConnString"), Session("OURConnProvider"))
        Else
            ToAccessLog("DeleteTestUsers", "No test users were found", 0, Session("OURConnString"), Session("OURConnProvider"))
        End If
    End Sub
    Private Sub btLogin_Click(sender As Object, e As EventArgs) Handles btLogin.Click
        Dim logon As String
        Dim password As String
        Dim ret As String = String.Empty
        Session("admin") = "user"
        Dim issuper As String = ""
        LblInvalid0.Visible = False
        Dim erro As String = String.Empty
        Try
            'checking texts
            If Session("map") = "yes" OrElse Session("spam") = "yes" Then
                logon = Session("logon")
                password = Session("pass")
            Else
                If Not Request("logon") Is Nothing AndAlso Request("logon").Trim <> "" Then
                    logon = cleanText(Request("logon"))
                Else
                    logon = ""
                End If
                If Not Request("pass") Is Nothing AndAlso Request("pass").Trim <> "" Then
                    password = cleanText(Request("pass"))
                Else
                    password = ""
                End If
                password = cleanText(Request("pass"))

                'TODO check texts
                If logon = "" Or password = "" Then
                    ret = "Logon And password should Not be empty."
                    'LblInvalid.Text = ret
                    WriteToAccessLog(Session("logon"), ret, 0)
                    Exit Try
                ElseIf logon <> Request("logon").ToString Then
                    ret = "Illegal character found In logon."
                    'LblInvalid.Text = ret
                    WriteToAccessLog(Session("logon"), ret, 0)
                    Exit Try
                ElseIf password <> Request("Pass").ToString Then
                    ret = "Illegal character found In password."
                    'LblInvalid.Text = ret
                    WriteToAccessLog(Session("logon"), ret, 0)
                    Exit Try
                End If
                'Else
            End If
            ret = FixDatetimeInOURPermits()
            If ret <> "" Then
                WriteToAccessLog(logon, "DateTime zeros fixed in OURPermits.", 1)
                ret = ""
            End If
            'pass checks
            Dim auto As String = "Public"  'autorization 
            Dim auth As Boolean = False   'authentication
            Dim pass As String = ConfigurationManager.AppSettings("superpass").ToString
            If pass = "super" Then
                pass = Now.ToShortDateString
            End If

            Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            'Not needed, will be done in DataModule:
            'If Session("OURConnProvider") = "MySql.Data.MySqlClient" Then
            '    Session("OURConnString") = (Session("OURConnString") & ";Convert Zero Datetime=True; Allow Zero Datetime=True").Replace(";;", ";")
            'End If
            If logon = "super" AndAlso password = pass Then
                Session("admin") = "super"   'FOR the first INSTALL ONLY !!!!!!!!!!!!!!!!!!!!!!!!!
                Session("logon") = "super"   'FOR the first INSTALL ONLY !!!!!!!!!!!!!!!!!!!!!!!!!
                Session("Application") = "InteractiveReporting"
                WriteToAccessLog(Session("logon"), "Login successful", 0)
                'check and save the user connection
                Dim userconnstr As String = String.Empty
                Dim userconnprv As String = String.Empty
                If Not Request("ConnStr") Is Nothing AndAlso cleanText(Request("ConnStr")) <> Request("ConnStr").ToString.Trim Then
                    ret = "Illegal character found. Please retype Connection String."
                    Exit Try
                ElseIf Not Request("ConnStr") Is Nothing AndAlso cleanText(Request("ConnStr")) = Request("ConnStr").ToString.Trim AndAlso cleanText(Request("ConnStr")).Length > 0 Then
                    Session("UserConnString") = cleanText(Request("ConnStr"))
                    userconnstr = Session("UserConnString")
                    If userconnstr.ToUpper.IndexOf("PASSWORD") < 0 AndAlso userconnstr.ToUpper.IndexOf("PWD") < 0 Then
                        ret = "Password has Not been found. Please retype Connection String."
                        Exit Try
                    End If
                    Session("UserConnProvider") = dropdownDatabases.SelectedValue
                Else 'only for super user
                    'sql connection to user database from web.config
                    If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection") Is Nothing Then
                        'sql connection to user database from webconfig 
                        userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ToString
                        userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ProviderName.ToString
                        DataModule.userdbcase = ConfigurationManager.AppSettings("userdbcase").ToString.Trim
                    Else
                        userconnstr = ""
                        userconnprv = ""
                        DataModule.userdbcase = ConfigurationManager.AppSettings("ourdbcase").ToString.Trim
                    End If
                    Session("UserConnString") = userconnstr
                    Session("UserConnProvider") = userconnprv
                    'Not needed, will be done in DataModule:
                    'If Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
                    '    Session("UserConnString") = (Session("OURConnString") & ";Convert Zero Datetime=True; Allow Zero Datetime=True").Replace(";;", ";")
                    'End If
                End If
                ret = FixDatetimeInOURPermissions()
                If ret <> "" Then
                    WriteToAccessLog(logon, "DateTime zeros fixed in OURPermissions.", 1)
                End If
                ret = FixDatetimeInOURUnits()
                If ret <> "" Then
                    WriteToAccessLog(logon, "DateTime zeros fixed in OURPermissions.", 1)
                End If
                Response.Redirect("InstallIt.aspx")

                'authentication  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Else  'not super user
                auth = False
                If Not Session("UnitAuthenticate") Is Nothing AndAlso Session("UnitAuthenticate") = "OK" Then
                    auth = UnitLogin.UnitAuthenticate(logon, password, erro)
                Else
                    auth = Login.OURAuthenticate(logon, password, "OURPermits", issuper, erro)
                End If
            End If
            ret = erro
            If (auth) Then   'authenticated

                'check if this is a CSV user and autorize it
                Dim dvprm As DataView
                Dim mSQL As String = "SELECT * FROM [OURPermits] WHERE ([Application]='InteractiveReporting' AND [Unit]='CSV' AND [Group3]='CSV' AND [NetId]='" & logon & "' AND [localpass]='" & password & "')"
                'Dim mSQL As String = "SELECT * FROM OURPermits WHERE (Application='InteractiveReporting' AND Group3='CSV' AND NetId='" & logon & "' AND localpass='" & password & "')"
                dvprm = DataModule.mRecords(mSQL, erro)
                ret = erro
                'CSV
                If Not dvprm Is Nothing AndAlso dvprm.Count > 0 AndAlso dvprm.Table.Rows.Count = 1 Then
                    Session("CSV") = "yes"
                    Session("Unit") = "CSV"
                    Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
                    ConnStr.Text = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
                    Session("UserConnString") = ConnStr.Text
                    If dvprm.Table.Rows(0)("Email").ToString.Trim = password Then
                        WriteToAccessLog(Session("logon"), "First time login successful", 0)
                        Session("ChangePassword") = True
                        Dim url As String = "confirm.aspx?unit=" & Session("Unit") & "&connstr=" & Session("UserConnString") & "&connprv=" & Session("UserConnProvider") & "&email=" & dvprm.Table.Rows(0)("Email").ToString.Trim
                        Response.Redirect(url)
                    End If
                    Session("admin") = dvprm.Table.Rows(0)("RoleApp")
                    Session("logon") = logon
                    Session("Application") = "InteractiveReporting"
                    Session("AdvancedUser") = True
                    'Response.Redirect("ListOfReports.aspx")
                    If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider")) Then
                        DataModule.userdbcase = ConfigurationManager.AppSettings("csvdbcase").ToString
                        Response.Redirect("ListOfReports.aspx")
                    Else
                        ret = "Connection to user database can not been open..."
                        Exit Try
                        WriteToAccessLog(logon, "Connection to database cannot be open.", 1)
                    End If
                End If

                'NO CSV
                'check and save the user connection
                Dim userconnstr As String = String.Empty
                Dim userconnprv As String = String.Empty
                If Not Request("ConnStr") Is Nothing AndAlso cleanText(Request("ConnStr")) <> Request("ConnStr").ToString.Trim Then
                    ret = "Illegal character found. Please retype Connection String."
                    Exit Try
                End If
                If Not Request("ConnStr") Is Nothing AndAlso cleanText(Request("ConnStr")) = Request("ConnStr").ToString.Trim AndAlso cleanText(Request("ConnStr")).Length > 0 Then
                    userconnstr = Request("ConnStr").ToString.Trim & "Password=" & cleanText(Request("txtDBpass"))
                    Dim userconnstrnouser As String = String.Empty
                    If userconnstr.ToUpper.IndexOf("PASSWORD") > 0 Then
                        userconnstrnouser = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD"))
                    End If
                    If userconnstr.ToUpper.IndexOf("PWD") > 0 Then
                        userconnstrnouser = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PWD"))
                    End If
                    'If userconnstrnouser.ToUpper.IndexOf("USER ID") > 0 Then
                    '    userconnstrnouser = userconnstrnouser.Substring(0, userconnstrnouser.ToUpper.IndexOf("USER ID"))
                    'End If
                    'If userconnstrnouser.ToUpper.IndexOf("UID") > 0 Then
                    '    userconnstrnouser = userconnstrnouser.Substring(0, userconnstrnouser.ToUpper.IndexOf("UID"))
                    'End If
                    If userconnstrnouser.ToUpper.IndexOf("USER ID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                        userconnstrnouser = userconnstrnouser.Substring(0, userconnstrnouser.ToUpper.IndexOf("USER ID")).Trim
                    End If
                    If userconnstrnouser.IndexOf("UID") > 0 AndAlso userconnprv <> "Oracle.ManagedDataAccess.Client" Then
                        userconnstrnouser = userconnstrnouser.Substring(0, userconnstrnouser.IndexOf("UID")).Trim
                    End If

                    mSQL = "SELECT * FROM OURPermits WHERE (Application='InteractiveReporting' AND NetId='" & logon & "' AND localpass='" & password & "' AND ConnStr LIKE '%" & userconnstrnouser.Trim.Replace(" ", "%") & "%')"
                    dvprm = DataModule.mRecords(mSQL, erro)
                    If Not dvprm Is Nothing AndAlso dvprm.Count > 0 AndAlso dvprm.Table.Rows.Count = 1 Then
                        userconnprv = dvprm.Table.Rows(0)("ConnPrv").ToString.Trim()
                    End If
                    Session("UserConnString") = userconnstr
                    Session("UserConnProvider") = userconnprv

                    If userconnstr.ToUpper.IndexOf("SERVER") < 0 AndAlso userconnstr.ToUpper.IndexOf("DATA SOURCE") < 0 AndAlso userconnstr.ToUpper.IndexOf("DSN") < 0 Then
                        ret = "Server/Data Source has not been found In the connection String. Please correct Connection String."
                        Exit Try
                    End If
                    If userconnstr.ToUpper.IndexOf("USER ID") < 0 AndAlso userconnstr.ToUpper.IndexOf("UID") < 0 Then
                        ret = "User ID has Not been found In the connection String. Please correct Connection String."
                        Exit Try
                    End If
                    If userconnstr.ToUpper.IndexOf("PASSWORD") < 0 AndAlso userconnstr.ToUpper.IndexOf("PWD") < 0 Then
                        ret = "Password has Not been found In the connection String. Please correct Connection String."
                        Exit Try
                    End If

                    If CheckBox1.Checked Then
                        'save Connection info 
                        userconnstr = Session("UserConnString")
                        userconnprv = Session("UserConnProvider")
                        'remove password
                        If userconnstr.ToUpper.IndexOf("PASSWORD") > 0 Then
                            userconnstr = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PASSWORD"))
                        End If
                        If userconnstr.ToUpper.IndexOf("PWD") > 0 Then
                            userconnstr = userconnstr.Substring(0, userconnstr.ToUpper.IndexOf("PWD"))
                        End If
                        'update OURPERMITS
                        ret = ExequteSQLquery("UPDATE OURPERMITS Set ConnStr='" & userconnstr & "',ConnPrv='" & userconnprv & "' WHERE NetID='" & Session("logon") & "' AND Application='InteractiveReporting'")
                        If ret = "Query executed fine." Then
                            WriteToAccessLog(Session("logon"), "ConnStr updated: " & userconnstr, 1)
                            ret = ""
                        Else
                            Exit Try
                        End If
                    End If
                ElseIf cleanText(Request("ConnStr")) = "" Then
                    If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection") Is Nothing Then
                        'connection to user database from web.config
                        userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ToString
                        userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ProviderName.ToString
                        Session("UserConnString") = userconnstr
                        Session("UserConnProvider") = userconnprv
                    Else 'connection to user database from OURPermits
                        Dim dv As DataView = mRecords("SELECT * FROM OURPERMITS WHERE NetID='" & Session("logon") & "' AND Application='InteractiveReporting'")
                        If Not IsDBNull(dv.Table.Rows(0)("ConnStr")) Then
                            userconnstr = cleanText(dv.Table.Rows(0)("ConnStr"))
                        End If
                        If Not IsDBNull(dv.Table.Rows(0)("ConnPrv")) Then
                            userconnprv = cleanText(dv.Table.Rows(0)("ConnPrv"))
                        End If
                        Session("UserConnString") = userconnstr
                        Session("UserConnProvider") = userconnprv
                        If userconnstr = "" OrElse userconnprv = "" Then
                            If issuper = "super" Then
                                Session("UserConnString") = Session("OURConnString")
                                Session("UserConnProvider") = Session("OURConnProvider")
                            Else
                                ret = "Connection string to user database has not been found. Please enter proper Connection String and select Database type."
                            End If
                            'Exit Try
                        ElseIf Session("logon") = "demo" Then
                            'userconnstr = userconnstr & " Password=Demo!@#4"
                            'Session("UserConnString") = userconnstr
                            'Session("UserConnProvider") = userconnprv
                            Session("UserConnString") = Session("OURConnString").Replace("OURdata", "DEMOdata")
                            Session("UserConnProvider") = Session("OURConnProvider")
                        ElseIf userconnprv <> "System.Data.OleDb" Then
                            LblInvalid.Text = "Add password to your database into Connection String."
                            Dim userconnstrnopass As String = userconnstr
                            If userconnstrnopass.IndexOf("Password") > 0 Then
                                userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.IndexOf("Password")).Trim
                            End If
                            ConnStr.Text = userconnstrnopass '& ";" ' & " Password="
                            dropdownDatabases.SelectedValue = userconnprv
                            trConnection.Visible = True
                            trDBPassword.Visible = True
                            trProvider.Visible = True
                            btUserConnection.Visible = False
                            If userconnprv = "Npgsql" AndAlso ConfigurationManager.AppSettings("userdbcase").ToString.Trim = "" Then
                                chkUserDBcase.Visible = True
                                chkUserDBcase.Enabled = True
                            End If
                            Exit Try
                        End If
                    End If
                End If



                'autorization !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                'check if first login
                Dim userEmail As String = String.Empty
                If Session("UnitAuthenticate") <> "OK" Then
                    auto = Login.OURAuthorize("OURPermits", logon, password, "InteractiveReporting", "", userconnstr, userconnprv, userEmail, issuper)
                Else
                    'sample of custom autorization if needed
                    auto = UnitLogin.UnitAuthorize(True, Session("Unit"), "OURPermits", "RoleApp", "Email", "NetId", "Localpass", logon, password, userEmail)
                End If
                If auto = "admin" Or auto = "user" Or auto = "public" Then
                    Session("admin") = auto
                    Session("logon") = logon
                    If auto = "public" Then Session("logon") = "public"
                    If userconnprv = "System.Data.Odbc" Then
                        userconnstr = userconnstr.Replace("UID", "User ID")
                    End If
                    If userconnprv = "System.Data.OleDb" Then
                        'userconnstr = userconnstr.Replace("UID", "User ID")
                    End If
                    Session("Application") = "InteractiveReporting"
                    If Session("logon") <> "demo" Then
                        Session("UserConnString") = userconnstr
                        Session("UserConnProvider") = userconnprv
                    End If
                    'Not needed, will be done in DataModule:
                    'If Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
                    '    Session("UserConnString") = (Session("OURConnString") & ";Convert Zero Datetime=True; Allow Zero Datetime=True").Replace(";;", ";")
                    'End If
                    If userEmail = password Then
                        WriteToAccessLog(Session("logon"), "First time login successful", 0)
                        Session("ChangePassword") = True
                        'Dim url As String = "confirm.aspx?unit=" & Session("Unit") & "&connstr=" & Session("OURConnString") & "&connprv=" & Session("OURConnProvider") & "&email=" & userEmail
                        Dim url As String = "confirm.aspx?unit=" & Session("Unit") & "&connstr=" & Session("UserConnString") & "&connprv=" & Session("UserConnProvider") & "&email=" & userEmail
                        Response.Redirect(url)
                    Else
                        WriteToAccessLog(Session("logon"), "Login successful", 0)
                        ret = FixDatetimeInOURPermissions()
                        If ret <> "" Then
                            WriteToAccessLog(logon, "DateTime zeros fixed in OURPermissions.", 1)
                        End If
                    End If

                    If ConfigurationManager.AppSettings("userdbcase").ToString.Trim <> "" Then
                        DataModule.userdbcase = ConfigurationManager.AppSettings("userdbcase").ToString.Trim
                    ElseIf chkUserDBcase.Checked Then
                        DataModule.userdbcase = "doublequoted"
                    Else
                        DataModule.userdbcase = "lower"
                    End If
                    'check connection to user database
                    Dim er As String = String.Empty
                    If DatabaseConnected(Session("UserConnString"), Session("UserConnProvider"), er) Then
                        Response.Redirect("ListOfReports.aspx")
                    Else
                        ret = "Connection to user database can not be open..."
                        Exit Try
                        WriteToAccessLog(logon, "Connection to database cannot be open.", 1)
                    End If

                ElseIf auto = "super" OrElse (logon = "super" AndAlso password = pass) Then
                    Session("admin") = "super"   'FOR INSTALL ONLY !!!!!!!!!!!!!!!!!!!!!!!!!
                    'Session("logon") = "super"   'FOR INSTALL ONLY !!!!!!!!!!!!!!!!!!!!!!!!!
                    Session("Application") = "InteractiveReporting"
                    WriteToAccessLog(Session("logon"), "Login successful", 0)
                    ret = FixDatetimeInOURPermissions()
                    If ret <> "" Then
                        WriteToAccessLog(logon, "DateTime zeros fixed in OURPermissions.", 1)
                    End If
                    If userEmail = password Then
                        WriteToAccessLog(Session("logon"), "First time login successful", 0)
                        Session("ChangePassword") = True
                        Dim url As String = "confirm.aspx?unit=" & Session("Unit") & "&connstr=" & Session("OURConnString") & "&connprv=" & Session("OURConnProvider") & "&email=" & userEmail
                        Response.Redirect(url)
                    Else
                        Response.Redirect("InstallIt.aspx")
                    End If
                Else
                    ret = "You do not have permissions to use this system... "
                    'LblInvalid.Text = ret
                    WriteToAccessLog(Session("logon"), ret, 0)
                End If
            Else
                ret = "User was not found. Wrong logon or password... " & ret
                'LblInvalid.Text = ret
                WriteToAccessLog(Session("logon"), ret & erro, 0)
            End If
            'End If
        Catch ex As Exception
            ret = ret & "  " & ex.Message
        End Try
        If ret <> String.Empty Then
            MessageBox.Show(ret, "Logon ", "Logon", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            'LblInvalid.Text = ret
            WriteToAccessLog(Session("logon"), ret, 2)
        End If
    End Sub
    Protected Sub btRegister_Click(sender As Object, e As EventArgs) Handles btRegister.Click
        Response.Redirect("Registration.aspx")
    End Sub
    Protected Sub btForgot_Click(sender As Object, e As EventArgs) Handles btForgot.Click
        Dim ret As String = String.Empty
        Dim useremail As String = String.Empty
        Dim passwd As String = String.Empty
        If Session("logon") = "" Then
            ret = "Request to Forgotten Password. Logon should not be empty."
            'LblInvalid.Text = ret
            MessageBox.Show(ret, "Forgotten Password", "ForgottenPassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            ret = WriteToAccessLog(Session("logon"), ret, 0)
            'Logon.Focus()
            Exit Sub
        Else
            Dim listofpermits As DataView
            listofpermits = mRecords("SELECT * FROM OURPERMITS WHERE (NetId='" & Session("logon") & "')  AND (Application='InteractiveReporting')")
            If listofpermits Is Nothing OrElse listofpermits.Table Is Nothing OrElse listofpermits.Table.Rows.Count <> 1 Then
                ret = "Request to Forgotten Password. Wrong Logon."
                MessageBox.Show(ret, "Forgotten Password", "ForgottenPassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                ret = WriteToAccessLog(Session("logon"), ret, 0)
                Exit Sub
            End If
            If Not IsDBNull(listofpermits.Table.Rows(0)("Email")) Then
                useremail = listofpermits.Table.Rows(0)("Email").ToString
                passwd = listofpermits.Table.Rows(0)("localpass").ToString
            Else
                ret = "Request to Forgotten Password. Email cannot be empty."
                'LblInvalid.Text = ret
                MessageBox.Show(ret, "Forgotten Password", "ForgottenPassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                ret = WriteToAccessLog(Session("logon"), ret, 0)
                Exit Sub
            End If
            'TODO make temp. password
            ret = SendHTMLEmail("", "Password Reminder", "This email has been sent by Online User Reporting in response to your request to remind you the password. Use temporary password  " & passwd & ". You must change it in your first login. Thanks for your interest in  Online User Reporting.", useremail, Session("SupportEmail"))
            'LblInvalid.Text = "Request to forgotten password. " & ret
            MessageBox.Show(ret, "Forgotten Password", "ForgottenPassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

            'If ret = False Then
            '    ret = "We could not find you in the list of Online User Reporting users with this user name and email address provided during registration."
            '    LblInvalid.Text = ret
            '    ret = WriteToAccessLog(Session("logon"), ret, 0)
            'Else
            '    ret = "Reminder has been sent to the email address provided during registration."
            '    LblInvalid.Text = ret
            '    ret = WriteToAccessLog(Session("logon"), ret, 0)
            'End If
        End If
    End Sub
    Protected Sub btChange_Click(sender As Object, e As EventArgs) Handles btChange.Click
        Dim ret As String = ""
        If Session("webinstall") = "OURweb" AndAlso Session("dbinstall") = "OURdb" Then
            If Request("logon") = "" AndAlso ConfigurationManager.AppSettings("unit").ToString = "OUR" Then
                Response.Redirect("Registration.aspx")
            End If
        End If

        If Request("logon") = "" Then
            ret = "Request to change password. Logon should not be empty."
            MessageBox.Show(ret, "Change Password", "ChangePassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

            'LblInvalid.Text = ret
            ret = WriteToAccessLog(Session("logon"), ret, 0)
            'Logon.Focus()
            Return
        End If
        Dim sql As String = "Select * From OURPermits Where NetId='" & Request("logon") & "'"
        Dim err As String = ""
        Dim dv As DataView = mRecords(sql, err)  'from OUR db
        If err <> "" OrElse dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            If err = "" Then
                'LblInvalid.Text = "Request to change password. User not found..."
                ret = "Request to change password. User not found..."
                MessageBox.Show(ret, "Change Password", "ChangePassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

            Else
                '.Text = "Request to change password. Error occurred: " & err
                ret = "Request to change password. Error occurred: " & err
                MessageBox.Show(ret, "Change Password", "ChangePassword", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            End If
            ret = WriteToAccessLog(Session("logon"), "Request to change password. " & ret, 0)
            Logon.Text = ""
            Logon.Focus()
            Return
        End If
        Session("ChangePassword") = True
        Dim tbl As DataTable = dv.Table
        Dim pswd As String = tbl.Rows(0)("localpass").ToString
        Dim unit As String = tbl.Rows(0)("Unit").ToString
        Dim connstr As String = tbl.Rows(0)("Connstr").ToString
        Dim connprv As String = tbl.Rows(0)("ConnPrv").ToString
        Dim email As String = tbl.Rows(0)("Email").ToString
        Dim url As String = "confirm.aspx?unit=" & unit & "&connstr=" & connstr & "&connprv=" & connprv & "&email=" & email

        Response.Redirect(url)

    End Sub
    Protected Sub dropdownDatabases_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dropdownDatabases.SelectedIndexChanged
        'If dropdownDatabases.Text.StartsWith("Intersystems") Then
        '    dropdownDatabases.ToolTip = "Server = yorserver; Port = yourportas1972; Namespace = yournamespace; User ID = youruser; Password = yourpassword"
        'ElseIf dropdownDatabases.Text = "Oracle" Then
        '    dropdownDatabases.ToolTip = "data source=yourserver:yourport/yourdatabase;user id=youruser;password=yourpassword"
        'ElseIf dropdownDatabases.Text = "ODBC" Then
        '    dropdownDatabases.ToolTip = "DSN=yourDSNname;UID=youruser;Password=yourpassword"
        'ElseIf dropdownDatabases.Text = "OleDb" Then
        '    dropdownDatabases.ToolTip = "Data Source=yourfilepath;"  'Persist Security Info=True;" & ConfigurationManager.AppSettings("ACEOLEDBversion").ToString 
        'Else
        '    dropdownDatabases.ToolTip = "Server=yorserver; Database=yourdatabase; User ID=youruser; Password=yourpassword"
        'End If
        txtDBpass.Enabled = True
        txtDBpass.Visible = True
        If dropdownDatabases.Text.StartsWith("Intersystems") Then
            dropdownDatabases.ToolTip = "Server = yorserver; Port = yourportas1972; Namespace = yournamespace; User ID = youruser"
        ElseIf dropdownDatabases.Text = "Oracle" Then
            dropdownDatabases.ToolTip = "data source=yourserver:yourport/yourdatabase;user id=youruser"
        ElseIf dropdownDatabases.Text = "ODBC" Then
            dropdownDatabases.ToolTip = "DSN=yourDSNname;UID=youruser"
        ElseIf dropdownDatabases.Text = "OleDb" Then
            dropdownDatabases.ToolTip = "Data Source=yourfilepath;" & ConfigurationManager.AppSettings("ACEOLEDBversion").ToString & "Persist Security Info=True;"
            txtDBpass.Enabled = False
            txtDBpass.Visible = False
        Else
            dropdownDatabases.ToolTip = "Server=yorserver; Database=yourdatabase; User ID=youruser"
        End If
        ConnStr.ToolTip = dropdownDatabases.ToolTip
    End Sub

    Private Sub btUserConnection_Click(sender As Object, e As EventArgs) Handles btUserConnection.Click
        trProvider.Visible = True
        trConnection.Visible = True
        trDBPassword.Visible = True
        trSaveConnection.Visible = True
        btUserConnection.Visible = False
        Logon.Focus()
    End Sub
    Sub SetPassword(id As String, psword As String)
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Me.ClientScript
        With sb
            .Append("<script language='JavaScript'>")
            .Append("function SetPassword()")
            .Append("{")
            .Append("var txt = document.getElementById('" & id & "');")
            .Append("txt.value='" & psword & "';")
            .Append("var MyControl = document.getElementById('txtDBpass');")
            .Append("if (MyControl != null)")
            .Append("{")
            .Append("MyControl.focus();")
            .Append("if (MyControl.createTextRange)")
            .Append("{")
            .Append("var range = MyControl.createTextRange();")
            .Append("range.moveStart('character',MyControl.value.length);")
            .Append("range.collapse(false);")
            .Append("range.select();")
            .Append("}")
            .Append("else")
            .Append("{")
            .Append("MyControl.selectionStart=MyControl.selectionEnd=MyControl.value.length;")
            .Append("}")

            .Append("}")
            .Append("}")
            .Append("window.onload = SetPassword;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "SetPassword", sb.ToString())
    End Sub
    Private Sub ClearText(CtlId As String)
        ScriptManager.RegisterStartupScript(Me, Me.Page.GetType(), CtlId, "javascript:ClearTextbox('" & CtlId & "')", True)
    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "ForgottenPassword" OrElse e.Tag = "Logon" OrElse e.Tag = "ChangePassword" OrElse e.Tag = "spam" Then
            'ClearText("Logon")
            'ClearText("ConnStr")
            'trConnection.Visible = False
            'trProvider.Visible = False
            'btUserConnection.Visible = True
            'Logon.Focus()
            Response.Redirect("Default.aspx")
        End If
    End Sub
    Private Sub lnkDemo_Click(sender As Object, e As EventArgs) Handles lnkDemo.Click
        'Threading.Thread.Sleep(1000)
        Response.Redirect(Session("WEBOUR").ToString & "Default.aspx?logon=demo&pass=demo")
    End Sub
    Protected Sub lnkPDF_Click(sender As Object, e As EventArgs) Handles lnkPDF.Click
        Response.Redirect(Session("WEBOUR").ToString & "OnlineUserReporting.pdf#page=4")
    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Response.Redirect(url)
    End Sub

End Class

