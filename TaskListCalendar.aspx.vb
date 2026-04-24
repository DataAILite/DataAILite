Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Partial Class TaskListCalendar
    Inherits System.Web.UI.Page
    Private events As ListItemCollection = New ListItemCollection
    Private Sub TaskListCalendar_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("calndr") = ""
        Calendar1.SelectedDate = Now.Date
        ' Calendar1.SelectedDayStyle.BackColor = Color.LightGray
        Calendar1.SelectedDayStyle.BackColor = Color.LightGoldenrodYellow
        Calendar1.SelectedDayStyle.BorderColor = Color.Red
        Calendar1.SelectionMode = CalendarSelectionMode.Day
        GetCalendarMonthData(Now.Year, Now.Month, "")
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Session("WEBOUR") = ConfigurationManager.AppSettings("weboureports").ToString
        HyperLink2.NavigateUrl = Session("WEBOUR")
    End Sub

    Private Sub TaskListCalendar_Load(sender As Object, e As EventArgs) Handles Me.Load
        Calendar1.SelectedDate = Now.Date
        'Calendar1.SelectedDayStyle.BackColor = Color.LightGray
        Calendar1.SelectedDayStyle.BackColor = Color.LightGoldenrodYellow
        Calendar1.SelectedDayStyle.BorderColor = Color.Red
        Calendar1.SelectionMode = CalendarSelectionMode.Day
        If Not Request("tn") Is Nothing AndAlso Request("tn").Trim <> "" Then
            Session("tn") = cleanText(Request("tn")).ToString
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
        e.Cell.ToolTip = "Click on the day number to add an event"
        Dim tn As String = String.Empty
        For i = 0 To events.Count - 1
            If DateToString(e.Day.Date.ToShortDateString) = events(i).Value.Trim OrElse DateToString(e.Day.Date.ToShortDateString) = events(i).Value & " 00:00:00" Then
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
                hyperlink1.NavigateUrl = "HelpDesk.aspx?calndr=yes&tn=" & tn
                hyperlink1.ToolTip = events(i).Text
                e.Cell.Controls.Add(hyperlink1)
            End If
        Next

    End Sub

    Private Sub Calendar1_SelectionChanged(sender As Object, e As EventArgs) Handles Calendar1.SelectionChanged
        Response.Redirect("HelpDesk.aspx?calndr=yes&tn=0&date=" & Calendar1.SelectedDate.ToShortDateString)
    End Sub

    Private Function GetCalendarMonthData(ByVal cyear As String, ByVal cmonth As String, Optional ByRef er As String = "") As ListItemCollection
        events.Clear()
        Dim i As Integer = 0
        If cmonth.Length = 1 Then
            cmonth = "0" & cmonth
        End If
        Try
            Dim mnthdata As DataTable = Session("Assignments")
            If mnthdata Is Nothing OrElse mnthdata.Rows.Count = 0 Then
                Return events
            End If
            For i = 0 To mnthdata.Rows.Count - 1
                If mnthdata.Rows(i)("Deadline").ToString.Trim.StartsWith(cyear & "-" & cmonth) Then
                    Dim li As New ListItem
                    li.Value = mnthdata.Rows(i)("Deadline").ToString.Trim
                    li.Text = "Deadline for Task #" & mnthdata.Rows(i)("ID").ToString & ": " & mnthdata.Rows(i)("Ticket").ToString.Trim
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
