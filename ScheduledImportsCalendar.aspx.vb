Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports MySql.Data.MySqlClient
Partial Class ScheduledImportsCalendar
    Inherits System.Web.UI.Page
    Dim tblname As String
    Private events As ListItemCollection = New ListItemCollection
    Private mUsers As ListItemCollection
    Private Sub ScheduledImportsCalendar_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Session("calndr") = ""
        'Calendar1.SelectedDate = Now.Date
        ' Calendar1.SelectedDayStyle.BackColor = Color.LightGray
        Calendar1.SelectedDayStyle.BackColor = Color.LightGoldenrodYellow
        Calendar1.SelectedDayStyle.BorderColor = Color.Red
        Calendar1.SelectionMode = CalendarSelectionMode.Day
        GetCalendarMonthData(Now.Year, Now.Month, "")
        If Not IsPostBack Then

            'DropDownTables
            Dim er As String = String.Empty
            Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
            If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                LabelAlert.Text = "No tables in db available for the user yet..."
                LabelAlert.Visible = True
                Exit Sub
            End If
            If er <> "" Then
                LabelAlert.Text = er
                LabelAlert.Visible = True
                Exit Sub
            End If
            DropDownTables.Items.Add(" ")
            For i = 0 To ddtv.Table.Rows.Count - 1
                Dim li As New ListItem
                'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
                li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                Try
                    If Not ddtv.Table.Rows(i)("TABLENAME") Is Nothing AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                        li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                    End If
                Catch ex As Exception

                End Try

                li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                DropDownTables.Items.Add(li)
            Next

            Session("tblname") = " "
        End If
    End Sub

    Private Sub ScheduledImportsCalendar_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Calendar1.SelectedDate = Now.Date
        'Calendar1.SelectedDayStyle.BackColor = Color.LightGray
        Calendar1.SelectedDayStyle.BackColor = Color.LightGoldenrodYellow
        Calendar1.SelectedDayStyle.BorderColor = Color.Red
        Calendar1.SelectionMode = CalendarSelectionMode.Day
        Session("towhom") = Session("email")

        Dim exst As Boolean = False
        Dim er As String = String.Empty

        If Not IsPostBack Then

            'name of table
            tblname = cleanText(txtTableName.Text.Trim)
            If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
                If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
                If tblname.IndexOf(".") < 0 Then
                    tblname = "userdata." & tblname
                End If
            End If
            Dim tblfriendlyname As String = cleanText(txtTableName.Text)
            If tblname.Trim <> "" Then
                'check if this table name exist in user tables dropdown
                For i = 0 To DropDownTables.Items.Count - 1
                    If DropDownTables.Items(i).Value.Trim.ToUpper = tblname.ToUpper Then
                        DropDownTables.SelectedItem.Value = tblname
                        txtTableName.Text = ""
                        Exit For
                    End If
                Next
            End If
            'list of tables
            If DropDownTables.Items.Count > 1 AndAlso DropDownTables.SelectedItem.Value.Trim <> "" Then
                'existing table from dropdown
                tblname = DropDownTables.SelectedItem.Value
                exst = True
            Else 'tblname is not in dropdown of user tables, table name from textbox
                If tblname.Trim = "" Then
                    'textbox with table name is empty 
                    tblname = Session("logon") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
                    tblname = tblname.ToLower.Replace(" ", "")
                    txtTableName.Text = tblname
                    exst = False
                End If
                'if exist it belong to some other user
                If TableExists(tblname, Session("UserConnString"), Session("UserConnProvider"), er) Then
                    'not clear data from existing table - create new table correcting name because it exist, but was not selected from the list of existing tables
                    tblname = txtTableName.Text.Trim & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "")
                    tblname = tblname.ToLower.Replace(" ", "")
                    txtTableName.Text = tblname
                    exst = False
                Else 'new table
                    tblname = tblname.ToLower.Replace(" ", "")
                    exst = False
                End If

            End If
            Session("tblname") = tblname
            'TODO make proper list of users
            Dim rsc As DataTable = Nothing  'available users
            Dim sqlr As String = String.Empty
            If Session("FromSite") = "yes" Then
                'show dropdowns
                If Session("admin") = "user" Then
                    'find the site support contact
                    sqlr = "SELECT * FROM OURPermits WHERE (Access='SUPPORT' OR Access='SiteAdmin')  AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%') AND (Application='InteractiveReporting')"
                ElseIf Session("admin") = "admin" Then
                    'find the site support or tester contact
                    sqlr = "SELECT * FROM OURPermits WHERE (Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%') AND (Application='InteractiveReporting')"
                ElseIf Session("admin") = "super" Then
                    'all DEV and Site admin, support, or tester
                    sqlr = "SELECT * FROM OURPermits WHERE Application='InteractiveReporting' AND (Access='FULL' OR Access='DEV' OR ((Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%') )) "
                End If
            Else
                'Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim
                'Label1.Text = "Database " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim

                If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
                    If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                    If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
                Else
                    If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
                End If


                'show dropdowns
                If Session("admin") = "user" Then
                    'find the site support contact
                    sqlr = "SELECT * FROM OURPermits WHERE (Access='SUPPORT' OR Access='SiteAdmin') AND ConnStr LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%' AND Application='InteractiveReporting'"
                ElseIf Session("admin") = "admin" Then
                    'find the site support or tester contact
                    sqlr = "SELECT * FROM OURPermits WHERE (Access='FULL' OR Access='DEV' OR Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND ConnStr LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%' AND Application='InteractiveReporting'"
                    'sqlr = "SELECT * FROM OURPermits WHERE ConnStr LIKE '%" & Session("UserDB") & "%' AND Application='InteractiveReporting'"

                ElseIf Session("admin") = "super" Then
                    'all DEV and Site admin, support, or tester
                    sqlr = "SELECT * FROM OURPermits WHERE Application='InteractiveReporting' AND (Access='FULL' OR Access='DEV' OR Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin')"
                End If
            End If
            rsc = mRecords(sqlr).Table
            mUsers = New ListItemCollection
            Dim li As ListItem
            'fill out the dropdown list of who and whom
            li = New ListItem
            li.Text = Session("logon")
            li.Value = Session("Email")
            If li.Value <> "" Then
                mUsers.Add(li)
            End If
            If Not rsc Is Nothing AndAlso rsc.Rows.Count > 0 Then
                For i = 0 To rsc.Rows.Count - 1
                    li = New ListItem
                    li.Text = Trim(rsc.Rows(i)("NetID").ToString)
                    li.Value = Trim(rsc.Rows(i)("Email").ToString)
                    If li.Value <> "" Then
                        mUsers.Add(li)
                    End If
                Next
            End If
            DropDownColumns.Items.Clear()
            For i = 0 To mUsers.Count - 1   'draw drop-down start
                DropDownColumns.Items.Add(mUsers(i))
            Next

            If runtime.Text = "HH:mm:00" Then
                ''runtime.Text = DateTime.Now.Hour & ":" & DateTime.Now.Minute & ":00"
                'runtime.Text = DateToString(DateTime.Now, "", True)
                'If runtime.Text.Trim.LastIndexOf(" ") > 0 Then
                '    runtime.Text = runtime.Text.Substring(runtime.Text.LastIndexOf(" ")).Trim
                'End If
                runtime.Text = DateTime.Now.TimeOfDay.ToString
                If runtime.Text.Trim.LastIndexOf(".") > 0 Then
                    runtime.Text = runtime.Text.Substring(0, runtime.Text.LastIndexOf(".")).Trim
                End If
            End If
            If ntimes.Text = 0 Then
                ntimes.Text = 1
            End If

        End If
    End Sub

    Private Sub Calendar1_VisibleMonthChanged(sender As Object, e As MonthChangedEventArgs) Handles Calendar1.VisibleMonthChanged
        GetCalendarMonthData(e.NewDate.Year, e.NewDate.Month, "")
    End Sub

    Private Sub Calendar1_DayRender(sender As Object, e As DayRenderEventArgs) Handles Calendar1.DayRender
        Dim i As Integer = 0
        If events Is Nothing OrElse events.Count = 0 Then
            Exit Sub
        End If
        e.Cell.ToolTip = "Click on the day number to schedule the report"
        Dim tn As String = String.Empty
        For i = 0 To events.Count - 1
            If DateToString(e.Day.Date.ToShortDateString) = events(i).Value Then
                'If DateToString(e.Day.Date.ToShortDateString) = events(i).Value & " 00:00:00" Then
                'e.Cell.ToolTip = events(i).Text
                Dim literal1 As Literal = New Literal
                literal1.Text = "<br/>"
                e.Cell.Controls.Add(literal1)
                'Dim label1 As Label = New Label
                'label1.Text = events(i).Text
                'label1.Font.Size = FontSize.Medium
                'label1.Font.Bold = True
                'label1.ForeColor = Color.Red
                'e.Cell.Controls.Add(label1)
                Dim hyperlink1 As HyperLink = New HyperLink
                hyperlink1.Text = events(i).Text
                If hyperlink1.Text.Length > 40 Then hyperlink1.Text = hyperlink1.Text.Substring(0, 40)
                hyperlink1.Font.Size = FontSize.Medium
                hyperlink1.Font.Bold = True
                hyperlink1.ForeColor = Color.Blue
                hyperlink1.BackColor = Color.Bisque
                tn = events(i).Text.Substring(events(i).Text.IndexOf("#") + 1, events(i).Text.IndexOf(":") - events(i).Text.IndexOf("#") - 1)
                If Not Session("tn") Is Nothing AndAlso Session("tn").ToString = tn Then
                    hyperlink1.BackColor = Color.Yellow
                End If
                hyperlink1.NavigateUrl = "ScheduledImports.aspx?calndr=yes&tn=" & tn
                hyperlink1.ToolTip = events(i).Text
                e.Cell.Controls.Add(hyperlink1)
            End If
        Next

    End Sub

    Private Sub Calendar1_SelectionChanged(sender As Object, e As EventArgs) Handles Calendar1.SelectionChanged
        If ntimes.Text = "0" Then
            'MessageBox.Show("Enter how many times to download and if needed to whom to send the notification", "Scheduled Imports", "Scheduled Imports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            LabelAlert.Text = "ERROR!! Enter how many times to download and if needed to whom to send the notification. "
            ntimes.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            Exit Sub
        End If
        Dim repeatCount1 As Integer
        If Not Integer.TryParse(ntimes.Text, repeatCount1) OrElse CInt(ntimes.Text) <= 0 Then
            'MessageBox.Show("Enter how many times to download. It should be integer more than 0.", "Input Error", "Scheduled Imports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            LabelAlert.Text = "ERROR!! Enter how many times to import. It should be integer more than 0."
            ntimes.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            Exit Sub
        End If

        ' Validate "Download the file from website"
        If Not Uri.IsWellFormedUriString(txtURI.Text, UriKind.Absolute) Then
            'MessageBox.Show("The URL format is not valid. Please enter a valid URL.", "Input Error", "Scheduled Imports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            LabelAlert.Text = "ERROR!! The URL format is not valid. Please enter a valid URL."
            txtURI.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            Exit Sub
        End If

        ' Validate "Time"
        Dim timePattern As String = "^([0-1]?[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$"
        If Not Regex.IsMatch(runtime.Text, timePattern) Then
            'MessageBox.Show("Time format is not valid. Please enter a valid time in military HH:mm:ss format.", "Input Error", "Scheduled Imports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            LabelAlert.Text = "ERROR!! Time format is not valid. Please enter a valid time in military HH:mm:ss format."
            runtime.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            Exit Sub
        End If
        If Calendar1.SelectedDate < CDate(DateTime.Now.Date) Then
            LabelAlert.Text = "ERROR!! Select the current date  " & DateTime.Now.Date.ToString & "  or later "
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            Exit Sub
        End If
        'check format of url
        Dim ext As String = String.Empty
        If txtURI.Text.Trim.LastIndexOf(".") < 0 Then
            WriteToAccessLog(Session("logon"), "Format of URL does not supported: " & txtURI.Text & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
            LabelAlert.Text = "ERROR!! Format of URL is not supported: " & txtURI.Text & ". It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB "
            Calendar1.SelectedDate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Month, -1, CDate(Session("curmonth") & "01"))))
            Exit Sub
        ElseIf txtURI.Text.Trim.LastIndexOf(".") > 0 Then
            ext = txtURI.Text.Trim.Substring(txtURI.Text.Trim.LastIndexOf("."))
            If ",.CSV,.XML,.JSON,.TXT,.XLS,.XLSX,.MDB,.ACCDB,".IndexOf(ext.ToUpper) < 0 Then
                WriteToAccessLog(Session("logon"), "Format of URL does not supported: " & txtURI.Text & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                LabelAlert.Text = "ERROR!! Format of URL is not supported: " & txtURI.Text & ". It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB "
                'Calendar1.SelectedDate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Month, -1, CDate(Session("curmonth") & "01"))))
                Exit Sub
            Else
                Dim emails As String = String.Empty
                Dim i As Integer
                For i = 0 To DropDownColumns.Items.Count - 1
                    If DropDownColumns.Items(i).Selected Then
                        emails = emails & DropDownColumns.Items(i).Value
                        If i < DropDownColumns.Items.Count - 1 Then
                            emails = emails & ","
                        End If
                    End If
                Next
                Dim url As String = "ScheduledImports.aspx?calndr=add&url=" & txtURI.Text & "&tbl=" & Session("tblname").ToString.Trim & "&cleartbl=" & chkboxClearTable.Checked.ToString & "&delmtr=" & TextboxDelimiter.Text & "&repeat=" & DropDownRecurse.SelectedItem.Text & "&times=" & runtime.Text & "&ntimes=" & ntimes.Text & "&tn=0&date=" & DateToString(Calendar1.SelectedDate.ToShortDateString) & "&towhom=" & emails
                Response.Redirect(url)
            End If
        End If
    End Sub

    Private Function GetCalendarMonthData(ByVal cyear As String, ByVal cmonth As String, Optional ByRef er As String = "") As ListItemCollection
        events.Clear()
        Dim i As Integer = 0
        If cmonth.Length = 1 Then
            cmonth = "0" & cmonth
        End If
        Try
            Dim mnthdata As DataTable
            mnthdata = mRecords("SELECT * FROM ourscheduledimports WHERE  UserId='" & Session("logon") & "' AND Deadline LIKE '" & cyear & "-" & cmonth & "%'").Table
            If mnthdata Is Nothing OrElse mnthdata.Rows.Count = 0 Then
                Return events
            End If
            For i = 0 To mnthdata.Rows.Count - 1
                If mnthdata.Rows(i)("Deadline").ToString.Trim.StartsWith(cyear & "-" & cmonth) Then
                    Dim li As New ListItem
                    li.Value = mnthdata.Rows(i)("Deadline").ToString.Trim
                    li.Text = "ID #" & mnthdata.Rows(i)("ID").ToString & ": " & mnthdata.Rows(i)("Name").ToString.Trim & " (" & mnthdata.Rows(i)("DownloadId").ToString & ")"
                    'If li.Text.Length > 40 Then li.Text = li.Text.Substring(0, 40)
                    events.Add(li)
                End If
            Next
        Catch ex As Exception
            er = ex.Message
        End Try
        Return events
    End Function
    Private Sub txtTableName_TextChanged(sender As Object, e As EventArgs) Handles txtTableName.TextChanged
        LabelAlert.Text = ""
        tblname = cleanText(txtTableName.Text.Trim)
        txtTableName.Text = tblname
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "userdata." & tblname
            End If
        End If
        txtTableName.Text = tblname
        txtSearch.Text = ""

        'DropDownTables restore all
        Dim er As String = String.Empty
        Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
        If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
            LabelAlert.Text = "No tables in db available for the user yet..."
            LabelAlert.Visible = True
            Exit Sub
        End If
        If er <> "" Then
            LabelAlert.Text = er
            LabelAlert.Visible = True
            Exit Sub
        End If
        DropDownTables.Items.Clear()
        DropDownTables.Items.Add(" ")
        For i = 0 To ddtv.Table.Rows.Count - 1
            Dim li As New ListItem
            'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
            li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            Try
                If Not ddtv.Table.Rows(i)("TABLENAME") Is Nothing AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                    li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                End If
            Catch ex As Exception

            End Try

            li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
            DropDownTables.Items.Add(li)
        Next

        If txtTableName.Text.Trim <> "" Then
            Try
                DropDownTables.Text = txtTableName.Text
                txtTableName.Text = ""
            Catch ex As Exception
                DropDownTables.SelectedIndex = 0
                DropDownTables.SelectedItem.Value = " "
            End Try

        End If

        Session("dv3") = Nothing
        FindReportsWithTable(tblname)
    End Sub
    Private Sub FindReportsWithTable(ByVal tblname As String)
        tblname = cleanText(tblname)
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "userdata." & tblname
                txtTableName.Text = tblname
            End If
        End If
        If tblname = "" AndAlso DropDownTables.Items.Count > 1 AndAlso DropDownTables.SelectedItem.Value.Trim <> "" Then
            tblname = DropDownTables.SelectedItem.Value.Trim
        ElseIf tblname = "" AndAlso DropDownTables.SelectedItem.Value.Trim = "" Then
            tblname = cleanText(txtTableName.Text.Trim)
        End If

        Session("tblname") = tblname
        If tblname = "" Then
            Session("REPORTID") = ""
            Session("REPTITLE") = ""
            Exit Sub
        End If
        Try
            DropDownTables.SelectedItem.Value = tblname
        Catch ex As Exception

        End Try

        Dim SQLq As String = String.Empty ' 
        Dim dv6 As DataView = Nothing
        Dim dr As DataTable
        Dim repttl As String = String.Empty ' Session("REPTITLE")
        Dim rep As String = String.Empty
        Dim userdb As String = Session("UserDB").trim
        Dim fnd As String = String.Empty
        Dim i As Integer = 0
        Dim er As String = String.Empty
        Dim SQLtext As String = String.Empty ' TextBoxSQL.Text.Trim

        SQLq = "SELECT DISTINCT OURReportInfo.ReportId, OURReportInfo.SQLquerytext, OURReportSQLquery.Tbl1, OURReportInfo.ReportDB FROM OURReportSQLquery INNER JOIN OURPermissions  ON (OURReportSQLquery.ReportId = OURPermissions.Param1) INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId = OURReportInfo.ReportId) WHERE Tbl1='" & tblname & "' AND NetId='" & Session("logon") & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"

        dv6 = mRecords(SQLq, er)
        If dv6 IsNot Nothing AndAlso dv6.Table IsNot Nothing AndAlso dv6.Table.Rows.Count > 0 Then
            LabelReports.Visible = True
            LabelReports.Text = "Reports with the table " & tblname & ": "
            tdreports.Visible = True
            list.Rows(0).Cells(0).InnerHtml = "  Data "
            list.Rows(0).Cells(0).Align = "left"
            list.Rows(0).Cells(1).InnerHtml = "  Analytics  "
            list.Rows(0).Cells(1).Align = "center"
            list.Rows(0).Cells(2).InnerHtml = "  Report  "
            list.Rows(0).Cells(2).Align = "center"

            For i = 1 To dv6.Table.Rows.Count
                rep = dv6.Table.Rows(i - 1)("ReportId").ToString
                dr = GetReportInfo(rep)
                repttl = dr.Rows(0)("ReportTtl").ToString
                If fnd = "" AndAlso dv6.Table.Rows(i - 1)("SQLquerytext").ToString.ToUpper = "SELECT * FROM " & tblname.ToUpper Then
                    Session("REPORTID") = dv6.Table.Rows(i - 1)("ReportId").ToString
                    Session("REPTITLE") = repttl
                    fnd = "found"
                End If

                AddRowIntoHTMLtable(dv6.Table.Rows(i - 1), List)
                For j = 0 To list.Rows(i).Cells.Count - 1
                    list.Rows(i).Cells(j).InnerText = ""
                    list.Rows(i).Cells(j).InnerHtml = ""
                Next
                If i Mod 2 = 0 Then
                    list.Rows(i).BgColor = "#EFFBFB"
                Else
                    list.Rows(i).BgColor = "white"
                End If

                'Dim ctl As New LinkButton
                'ctl.Text = repttl
                'ctl.ID = rep & "*" & repttl
                'ctl.ToolTip = "Show " & rep
                'ctl.CssClass = "NodeStyle"
                'ctl.Font.Size = 10
                'AddHandler ctl.Click, AddressOf btnShow_Click
                'list.Rows(i + 1).Cells(0).InnerText = ""
                'list.Rows(i + 1).Cells(0).Controls.Add(ctl)

                list.Rows(i).Cells(0).InnerText = repttl
                list.Rows(i).Cells(0).InnerHtml = "<a href='ShowReport.aspx?Report=" & rep & "' data-toggle=""tooltip"" title=""" & dv6.Table.Rows(i - 1)("SQLquerytext").ToString & """ Target=""_blank"">data</a>"
                list.Rows(i).Cells(0).Align = "left"

                list.Rows(i).Cells(1).InnerText = "    analytics   "
                list.Rows(i).Cells(1).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?srd=11&Report=" & rep & "' data-toggle=""tooltip"" title=""DataAI"" Target=""_blank"">analytics</a>&nbsp;&nbsp;"
                list.Rows(i).Cells(1).Align = "center"

                list.Rows(i).Cells(2).InnerText = rep
                list.Rows(i).Cells(2).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?srd=3&Report=" & rep & "' data-toggle=""tooltip"" title=""" & rep & """ Target=""_blank"">" & repttl & "</a>&nbsp;&nbsp;"
                list.Rows(i).Cells(2).Align = "center"

                If i Mod 2 = 0 Then
                    list.Rows(i).BgColor = "#EFFBFB"
                Else
                    list.Rows(i).BgColor = "white"
                End If
            Next
        Else 'no reports
            txtTableName.Text = tblname
            DropDownTables.SelectedIndex = 0
            DropDownTables.SelectedItem.Value = " "
            DropDownTables.Text = " "
            LabelReports.Visible = False
            LabelReports.Text = " "
            trheaders.Visible = False

        End If

    End Sub

    Private Sub DropDownTables_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTables.SelectedIndexChanged
        txtTableName.Text = ""
        tblname = DropDownTables.SelectedItem.Value
        Session("tablename") = tblname
        FindReportsWithTable(DropDownTables.SelectedItem.Value)
    End Sub
End Class
