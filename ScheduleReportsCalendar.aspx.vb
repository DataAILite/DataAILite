Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Partial Class ScheduleReportsCalendar
    Inherits System.Web.UI.Page
    Private events As ListItemCollection = New ListItemCollection
    Private mUsers As ListItemCollection
    Private Sub ScheduleReportsCalendar_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("calndr") = ""
        'Calendar1.SelectedDate = Now.Date
        ' Calendar1.SelectedDayStyle.BackColor = Color.LightGray
        Calendar1.SelectedDayStyle.BackColor = Color.LightGoldenrodYellow
        Calendar1.SelectedDayStyle.BorderColor = Color.Red
        Calendar1.SelectionMode = CalendarSelectionMode.Day
        GetCalendarMonthData(Now.Year, Now.Month, "")
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub

    Private Sub ScheduleReportsCalendar_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Calendar1.SelectedDate = Now.Date
        'Calendar1.SelectedDayStyle.BackColor = Color.LightGray
        Calendar1.SelectedDayStyle.BackColor = Color.LightGoldenrodYellow
        Calendar1.SelectedDayStyle.BorderColor = Color.Red
        Calendar1.SelectionMode = CalendarSelectionMode.Day
        If Not Request("tn") Is Nothing AndAlso Request("tn").Trim <> "" Then
            Session("tn") = cleanText(Request("tn")).ToString
        End If
        Session("towhom") = Session("email")

        If Not IsPostBack Then
            'Dim drvalue, drname As String
            Dim i As Integer
            Dim sqlr As String = String.Empty

            'TODO make proper list of users
            Dim rsc As DataTable = Nothing  'available users
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
                    sqlr = "SELECT * FROM OURPermits WHERE (Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND ConnStr LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%' AND Application='InteractiveReporting'"
                    'sqlr = "SELECT * FROM OURPermits WHERE ConnStr LIKE '%" & Session("UserDB") & "%' AND Application='InteractiveReporting'"

                ElseIf Session("admin") = "super" Then
                    'all DEV and Site admin, support, or tester
                    sqlr = "SELECT * FROM OURPermits WHERE Application='InteractiveReporting' AND (Access='FULL' OR Access='DEV' OR Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin')"
                End If
            End If
            rsc = mRecords(sqlr).Table

            '+report users
            Dim rsu As DataTable = Nothing  'report users
            sqlr = "SELECT NetId,AccessLevel,Param3 AS Email,OpenFrom,OpenTo,Comments,Indx FROM OURPermissions WHERE (APPLICATION='InteractiveReporting' AND  Param1='" & Session("REPORTID") & "') ORDER BY NetId"
            rsu = mRecords(sqlr).Table

            If Session("FromSite") = "yes" Then
                If rsc Is Nothing OrElse rsc.Rows.Count < 1 Then
                    Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                    Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                    'Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim
                    'Label1.Text = "User database: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim

                    If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
                        If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                        If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
                    Else
                        If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
                    End If


                Else
                    Session("UserConnString") = rsc.Rows(i)("ConnStr").ToString
                    Session("UserConnProvider") = rsc.Rows(i)("ConnPrv").ToString
                    'Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim
                    'Label1.Text = "User database: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim

                    If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
                        If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                        If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
                    Else
                        If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
                    End If

                End If
                HyperLink3.Visible = False
                HyperLink3.Enabled = False
            End If

            'DropDownListWho.Text = Session("logon")
            mUsers = New ListItemCollection
            Dim li As ListItem
            li = New ListItem
            li.Text = Session("logon")
            li.Value = Session("Email")
            If li.Value <> "" Then
                mUsers.Add(li)
            End If
            'fill out the dropdown list of who and whom
            If Not rsu Is Nothing AndAlso rsu.Rows.Count > 0 Then
                For i = 0 To rsu.Rows.Count - 1
                    li = New ListItem
                    li.Text = Trim(rsu.Rows(i)("NetID").ToString)
                    li.Value = Trim(rsu.Rows(i)("Email").ToString)
                    If li.Value <> "" Then
                        mUsers.Add(li)
                    End If
                Next
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
                'runtime.Text = DateTime.Now.Hour & ":" & DateTime.Now.Minute & ":00"
                'runtime.Text = DateToString(DateTime.Now, "", True)
                'If runtime.Text.Trim.LastIndexOf(" ") > 0 Then
                '    runtime.Text = runtime.Text.Substring(runtime.Text.LastIndexOf(" ")).Trim
                'End If
                'runtime.Text = DateToStringFormat(DateTime.Now, Session("OURConnProvider"), "yyyy-MM-dd HH:mm:00")
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
                Dim literal1 As Literal = New Literal()
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
                hyperlink1.NavigateUrl = "ScheduledReports.aspx?calndr=yes&tn=" & tn
                hyperlink1.ToolTip = events(i).Text
                e.Cell.Controls.Add(hyperlink1)
            End If
        Next

    End Sub

    Private Sub Calendar1_SelectionChanged(sender As Object, e As EventArgs) Handles Calendar1.SelectionChanged
        If ntimes.Text = "0" Then
            MessageBox.Show("Enter how many times and to whom to send the report", "Schedule Reports", "Schedule Reports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            ntimes.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -1, DateTime.Now.Date))
            Exit Sub
        End If
        ' Validate "Time"
        Dim timePattern As String = "^([0-1]?[0-9]|2[0-3]):([0-5][0-9]):([0-5][0-9])$"
        If Not Regex.IsMatch(runtime.Text, timePattern) Then
            'MessageBox.Show("Time format is not valid. Please enter a valid time in military HH:mm:ss format.", "Input Error", "Scheduled Imports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            'LabelAlert.Text = "ERROR!! Time format is not valid. Please enter a valid time in military HH:mm:ss format."
            runtime.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            MessageBox.Show("ERROR!! Time format is not valid. Please enter a valid time in military HH:mm:ss format.", "Schedule Reports", "Schedule Reports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Exit Sub
        End If
        If Calendar1.SelectedDate < CDate(DateTime.Now.Date) Then
            'LabelAlert.Text = "ERROR!! Select the current date  " & DateTime.Now.Date.ToString & "  or later "
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -2, DateTime.Now.Date))
            MessageBox.Show("ERROR!! Select the current date  " & DateTime.Now.Date.ToString & "  or later ", "Schedule Reports", "Schedule Reports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Exit Sub
        End If
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
        If emails.Trim = "" Then
            MessageBox.Show("Enter how many times and to whom to send the report", "Schedule Reports", "Schedule Reports", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            DropDownColumns.BorderColor = Color.Red
            Calendar1.SelectedDate = CDate(DateAndTime.DateAdd(DateInterval.Month, -1, DateTime.Now.Date))
            Exit Sub
        End If
        Dim url As String = "ScheduledReports.aspx?calndr=add&rep=" & Session("REPORTID") & "&repeat=" & DropDownRecurse.SelectedItem.Text & "&times=" & runtime.Text & "&ntimes=" & ntimes.Text & "&tn=0&date=" & DateToString(Calendar1.SelectedDate.ToShortDateString) & "&towhom=" & emails
        Response.Redirect(url)
    End Sub

    Private Function GetCalendarMonthData(ByVal cyear As String, ByVal cmonth As String, Optional ByRef er As String = "") As ListItemCollection
        events.Clear()
        Dim i As Integer = 0
        If cmonth.Length = 1 Then
            cmonth = "0" & cmonth
        End If
        Try
            Dim mnthdata As DataTable
            mnthdata = mRecords("SELECT * FROM ourscheduledreports WHERE  UserId='" & Session("logon") & "' AND Deadline LIKE '" & cyear & "-" & cmonth & "%'").Table
            If mnthdata Is Nothing OrElse mnthdata.Rows.Count = 0 Then
                Return events
            End If
            For i = 0 To mnthdata.Rows.Count - 1
                If mnthdata.Rows(i)("Deadline").ToString.Trim.StartsWith(cyear & "-" & cmonth) Then
                    Dim li As New ListItem
                    li.Value = mnthdata.Rows(i)("Deadline").ToString.Trim
                    li.Text = "ID #" & mnthdata.Rows(i)("ID").ToString & ": " & mnthdata.Rows(i)("Name").ToString.Trim & " (" & mnthdata.Rows(i)("ReportId").ToString & ")"
                    'If li.Text.Length > 40 Then li.Text = li.Text.Substring(0, 40)
                    events.Add(li)
                End If
            Next
        Catch ex As Exception
            er = ex.Message
        End Try
        Return events
    End Function

End Class
