Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Math
Imports System.IO
Imports System.Drawing
Imports MySql.Data.MySqlClient
Imports System.Net
'Imports System.Threading

Partial Class ScheduledImports
    Inherits System.Web.UI.Page
    Public myconstring As String
    Private mUsers As ListItemCollection

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Page.MaintainScrollPositionOnPostBack = True
        LabelAlert.Text = " "
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'If Session("ntimerticks") = 0 Then
        '    Response.Redirect("RunScheduledImports.aspx")
        'End If

        Dim i As Integer = 0
        Dim rid, n As Integer
        Dim towhom, fl, ret, rundate As String
        ret = ""
        ''current month
        'Dim curmonth As String = String.Empty
        'If Session("curmonth") IsNot Nothing AndAlso Session("curmonth").ToString.Trim.StartsWith("20") AndAlso Session("curmonth").ToString.Trim.Length > 7 Then
        '    curmonth = Session("curmonth").ToString.Trim.Substring(0, 8)
        'Else
        '    curmonth = DateToString(Now()).ToString.Trim.Substring(0, 8)
        '    Session("curmonth") = curmonth
        'End If
        ''prev and next month
        'If Not Request("mnth") Is Nothing Then
        '    If Request("mnth").ToString.Trim = "prev" Then
        '        curmonth = curmonth & "01"
        '        curmonth = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Month, -1, CDate(curmonth)))).Trim.Substring(0, 8)
        '    ElseIf Request("mnth").ToString.Trim = "next" Then
        '        curmonth = curmonth & "01"
        '        curmonth = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Month, 1, CDate(curmonth)))).Trim.Substring(0, 8)
        '    End If
        'End If
        ''current month
        'Dim curmonth As String = String.Empty
        'If Session("curmonth") Is Nothing OrElse Session("curmonth").ToString.Trim = "" OrElse Not Session("curmonth").ToString.Trim.StartsWith("20") Then
        '    Session("curmonth") = DateToStringFormat(Now().ToString.Trim, "", "yyyy-MM-dd")
        'Else
        '    Session("curmonth") = DateToStringFormat(Session("curmonth"), "", "yyyy-MM-dd")
        'End If

        'If Session("curmonth") IsNot Nothing AndAlso Session("curmonth").ToString.Trim.StartsWith("20") AndAlso Session("curmonth").ToString.Trim.Length > 7 Then
        '    curmonth = Session("curmonth").ToString.Trim.Substring(0, 8)
        '    'Else
        '    '    curmonth = DateToString(Now()).ToString.Trim.Substring(0, 8)
        '    '    Session("curmonth") = curmonth
        'End If
        ''prev and next month
        'If Not Request("mnth") Is Nothing Then
        '    If Request("mnth").ToString.Trim = "prev" Then
        '        curmonth = curmonth & "01"
        '        curmonth = DateToStringFormat(CDate(DateAndTime.DateAdd(DateInterval.Month, -1, CDate(curmonth))), "", "yyyy-MM-dd").Trim.Substring(0, 8)
        '    ElseIf Request("mnth").ToString.Trim = "next" Then
        '        curmonth = curmonth & "01"
        '        curmonth = DateToStringFormat(CDate(DateAndTime.DateAdd(DateInterval.Month, 1, CDate(curmonth))), "", "yyyy-MM-dd").Trim.Substring(0, 8)
        '    End If
        '    Session("curmonth") = curmonth & "01"
        '    Response.Redirect("ScheduledImports.aspx")
        'End If

        'current month
        Dim curmonth As String = String.Empty
        If Session("curmonth") Is Nothing OrElse Session("curmonth").ToString.Trim = "" OrElse Not Session("curmonth").ToString.Trim.StartsWith("20") Then
            Session("curmonth") = DateToStringFormat(Now().ToString.Trim, "", "yyyy-MM-dd")
        Else
            Session("curmonth") = DateToStringFormat(Session("curmonth"), "", "yyyy-MM-dd")
        End If

        If Session("curmonth") IsNot Nothing AndAlso Session("curmonth").ToString.Trim.StartsWith("20") AndAlso Session("curmonth").ToString.Trim.Length > 7 Then
            curmonth = Session("curmonth").ToString.Trim.Substring(0, 8)
            Session("curmonth") = curmonth & "01"
            'Else
            '    curmonth = DateToString(Now()).ToString.Trim.Substring(0, 8)
            '    Session("curmonth") = curmonth
        End If
        'prev and next month
        If Not Request("mnth") Is Nothing Then
            If Request("mnth").ToString.Trim = "prev" Then
                curmonth = curmonth & "01"
                curmonth = DateToStringFormat(CDate(DateAndTime.DateAdd(DateInterval.Month, -1, CDate(curmonth))), "", "yyyy-MM-dd").Trim.Substring(0, 8)
            ElseIf Request("mnth").ToString.Trim = "next" Then
                curmonth = curmonth & "01"
                curmonth = DateToStringFormat(CDate(DateAndTime.DateAdd(DateInterval.Month, 1, CDate(curmonth))), "", "yyyy-MM-dd").Trim.Substring(0, 8)
            End If
            Session("curmonth") = curmonth & "01"
            Response.Redirect("ScheduledImports.aspx")
        End If

        Dim indf As String = String.Empty
        Dim recc As String = String.Empty
        Dim url As String = String.Empty
        Dim times As String = String.Empty
        Dim tblname As String = String.Empty
        Dim cleartable As String = String.Empty
        Dim delimtr As String = String.Empty
        'Session("curmonth") = curmonth
        'add new schedule
        If Not Request("calndr") Is Nothing AndAlso Request("calndr").ToString.Trim = "add" Then
            url = Request("url")
            tblname = Request("tbl")
            cleartable = Request("cleartbl")
            delimtr = Request("delmtr")
            rundate = Request("date")
            times = Request("times")
            towhom = Request("towhom").Replace(" ", "+")

            indf = GetScheduledImportsIdentifier()
            Session("indf") = indf
            'add future scheduled Imports
            If Not Request("ntimes") Is Nothing AndAlso Request("ntimes").ToString.Trim <> "" Then
                n = CInt(Request("ntimes"))
                i = 0
                recc = "run #1 from " & n.ToString & ", " & Request("repeat").ToString
            Else
                n = 0
            End If
            rundate = DateToStringFormat(rundate, "", "yyyy-MM-dd HH:mm:00")
            rundate = rundate.Replace("00:00:00", times)
            'add first scheduled Imports
            ret = AddScheduledImports(url, tblname, cleartable, delimtr, Session("logon"), rundate, towhom, indf, recc)
            If n > 0 AndAlso Not Request("repeat") Is Nothing AndAlso Request("repeat").ToString.Trim <> "" Then
                For i = 1 To n - 1   'first one already scheduled
                    recc = "run #" & (i + 1).ToString & " from " & n.ToString & ", " & Request("repeat").ToString
                    If Request("repeat").ToString.Trim = "daily" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 1, CDate(rundate))))
                        ret = AddScheduledImports(url, tblname, cleartable, delimtr, Session("logon"), rundate, towhom, indf, recc)
                    ElseIf Request("repeat").ToString.Trim = "weekly" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 7, CDate(rundate))))
                        ret = AddScheduledImports(url, tblname, cleartable, delimtr, Session("logon"), rundate, towhom, indf, recc)
                    ElseIf Request("repeat").ToString.Trim = "monthly" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Month, 1, CDate(rundate))))
                        ret = AddScheduledImports(url, tblname, cleartable, delimtr, Session("logon"), rundate, towhom, indf, recc)
                    ElseIf Request("repeat").ToString.Trim = "yearly" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Year, 1, CDate(rundate))))
                        ret = AddScheduledImports(url, tblname, cleartable, delimtr, Session("logon"), rundate, towhom, indf, recc)
                    End If
                Next
                LabelAlert.Text = " "
                Response.Redirect("ScheduledImports.aspx")
            Else
                LabelAlert.Text = ret
                Response.End()
            End If
        End If

        'search
        If FirstLetters.Text.Trim <> "" Then
            fl = FirstLetters.Text.Trim
        Else
            fl = ""
        End If
        Session("FirstLetters") = fl

        Dim fltr As String = String.Empty
        If fl.Trim <> "" Then
            fltr = " (URL LIKE '%" & fl & "%' OR Deadline LIKE '%" & fl & "%' OR [Status] LIKE '%" & fl & "%' OR TableName LIKE '%" & fl & "%' OR Prop3 LIKE '%" & fl & "%' OR Prop1 LIKE '%" & fl & "%' OR ToWhom LIKE '%" & fl & "%' OR Prop5 LIKE '%" & fl & "%' OR Prop6 LIKE '%" & fl & "%') "
        End If

        'RECORDS calculation start

        'find latest ID
        Dim AssignmentTable As DataTable = Nothing
        Dim sql As String = "SELECT ID FROM ourscheduledImports ORDER BY ID DESC"
        AssignmentTable = mRecords(sql).Table
        If AssignmentTable Is Nothing Then Return
        If AssignmentTable.Rows.Count > 0 Then
            Session("LastTicketNo") = AssignmentTable.Rows(0)("ID").ToString
        Else
            Session("LastTicketNo") = "0"
        End If
        Dim frsite As String = String.Empty
        Dim docs As String = String.Empty

        sql = "SELECT * FROM ourscheduledImports WHERE Deadline LIKE '" & curmonth & "%' AND UserId='" & Session("logon") & "' "
        'End If
        If fltr.Trim <> "" Then sql = sql & " AND (" & fltr & ") "
        sql = sql & " ORDER BY Deadline,ID"

        AssignmentTable = mRecords(sql, ret).Table


        Label3.Text = "Scheduled Imports for " & curmonth.Replace("-", " ").Trim.Replace(" ", "/")

        If AssignmentTable Is Nothing Then
            Exit Sub
        End If
        Dim clrtable As String = String.Empty
        'Draw recodrs on screen
        'Dim clrrow As String
        If AssignmentTable.Rows.Count > 0 Then
            MessageLabel.Text = AssignmentTable.Rows.Count.ToString
            For i = 0 To AssignmentTable.Rows.Count - 1
                'draw regular columns
                ret = AddRowIntoHTMLtableWithNcols(AssignmentTable.Rows(i), SchedDowns, 9)
                SchedDowns.Rows(i + 4).Cells(0).Align = "left"
                SchedDowns.Rows(i + 4).Cells(0).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(1).Align = "left"
                SchedDowns.Rows(i + 4).Cells(1).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(2).Align = "left"
                SchedDowns.Rows(i + 4).Cells(2).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(3).Align = "left"
                SchedDowns.Rows(i + 4).Cells(3).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(4).Align = "left"
                SchedDowns.Rows(i + 4).Cells(4).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(5).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(5).Align = "left"
                SchedDowns.Rows(i + 4).Cells(6).Align = "left"
                SchedDowns.Rows(i + 4).Cells(6).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(7).Align = "left"
                SchedDowns.Rows(i + 4).Cells(7).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(7).Style.Item("font-size") = "10px"

                Dim sttxt As String = AssignmentTable.Rows(i)("Status").ToString.Trim.ToLower
                Dim ftxt As String = "FldText='" & sttxt & "'"

                SchedDowns.Rows(i + 4).Cells(0).InnerText = AssignmentTable.Rows(i)("ID").ToString
                SchedDowns.Rows(i + 4).Cells(1).InnerHtml = FormatAsHTMLsimple(AssignmentTable.Rows(i)("URL").ToString & " | scheduled on " & AssignmentTable.Rows(i)("STARTDATE").ToString & " | by " & AssignmentTable.Rows(i)("UserId").ToString)

                clrtable = AssignmentTable.Rows(i)("TableName").ToString & " | (to empty table? - "
                If i = 0 AndAlso AssignmentTable.Rows(i)("Prop5").ToString = "True" Then
                    clrtable = clrtable & "yes"
                Else
                    clrtable = clrtable & "no"
                End If
                If AssignmentTable.Rows(i)("URL").ToString.ToUpper.EndsWith(".CSV") Then
                    clrtable = clrtable & ", delimeter """ & AssignmentTable.Rows(i)("Prop6").ToString.Trim & """"
                End If
                clrtable = clrtable & ")"
                SchedDowns.Rows(i + 4).Cells(2).InnerHtml = FormatAsHTMLsimple(clrtable)

                SchedDowns.Rows(i + 4).Cells(3).InnerHtml = FormatAsHTMLsimple(AssignmentTable.Rows(i)("DEADLINE").ToString & " | " & AssignmentTable.Rows(i)("Prop1").ToString)

                SchedDowns.Rows(i + 4).Cells(4).InnerText = AssignmentTable.Rows(i)("Status").ToString

                SchedDowns.Rows(i + 4).Cells(5).InnerHtml = ""
                SchedDowns.Rows(i + 4).Cells(5).InnerHtml = AssignmentTable.Rows(i)("ToWhom").ToString

                rid = AssignmentTable.Rows(i)("ID").ToString

                SchedDowns.Rows(i + 4).Cells(6).InnerText = ""
                Dim ctl As New LinkButton
                ctl.Text = "delete this one"
                ctl.ID = "DelOne ^" & CStr(rid)
                ctl.ToolTip = "Delete this occurrence of scheduled report " & CStr(rid)
                ctl.Style.Item("color") = "blue"
                ctl.Font.Size = 8
                ctl.Font.Italic = True
                AddHandler ctl.Click, AddressOf btnDelOne_Click
                SchedDowns.Rows(i + 4).Cells(6).Controls.Add(ctl)

                SchedDowns.Rows(i + 4).Cells(7).InnerText = ""
                Dim ctrl = New LinkButton
                ctrl.Text = "delete all occurrences"
                ctrl.ID = "DelAll ^" & CStr(rid)
                ctrl.ToolTip = "Delete all occurrences of scheduled download " & CStr(rid)
                ctrl.Style.Item("color") = "blue"
                ctrl.Font.Size = 8
                ctrl.Font.Italic = True
                AddHandler ctrl.Click, AddressOf btnDelAll_Click
                SchedDowns.Rows(i + 4).Cells(7).Controls.Add(ctrl)

                SchedDowns.Rows(i + 4).Cells(8).VAlign = "top"
                SchedDowns.Rows(i + 4).Cells(8).InnerText = AssignmentTable.Rows(i)("Prop3").ToString

                If Session("indf") <> "" AndAlso AssignmentTable.Rows(i)("Prop3").ToString = Session("indf") Then
                    SchedDowns.Rows(i + 4).Focus()
                    SchedDowns.Rows(i + 4).Cells(0).Focus()
                    SchedDowns.Rows(i + 4).Cells(0).BgColor = "yellow"

                End If
            Next
        End If
    End Sub

    Protected Sub btnDelOne_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Dim sql As String = "DELETE FROM ourscheduledImports WHERE ID=" & id & " AND UserId='" & Session("logon") & "'"
        Dim ret As String = String.Empty
        ret = ExequteSQLquery(sql)
        If ret <> "Query executed fine." Then
            ret = "ERROR!! " & ret
            MessageBox.Show(ret, "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        Else
            MessageBox.Show("Download deleted from the list of Scheduled Imports", "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
        End If
        Response.Redirect("ScheduledImports.aspx")
    End Sub
    Protected Sub btnDelAll_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM ourscheduledImports WHERE ID = " & id, err).ToTable
        Dim indf As String = dt.Rows(0)("Prop3").ToString
        Dim sql As String = "DELETE FROM ourscheduledImports WHERE Prop3='" & indf & "' AND UserId='" & Session("logon") & "'"
        Dim ret As String = String.Empty
        ret = ExequteSQLquery(sql)
        If ret <> "Query executed fine." Then
            ret = "ERROR!! " & ret
            MessageBox.Show(ret, "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        Else
            MessageBox.Show("Reccurence deleted from the list of Scheduled Imports", "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
        End If
        Response.Redirect("ScheduledImports.aspx")
    End Sub

    Private Function AddScheduledImports(ByVal url As String, ByVal tbl As String, ByVal clr As String, ByVal delim As String, ByVal logon As String, ByVal rundate As String, ByVal ToWhom As String, ByVal indf As String, ByVal runnumber As String) As String
        Dim ret As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            sqlq = "INSERT INTO ourscheduledImports (StartDate,[Status],UserId,URL,TableName,Deadline,ToWhom,Prop5,Prop6,Prop3,Prop1) VALUES ('" & DateToString(Now()) & "','scheduled',"
            sqlq = sqlq & "'" & logon & "',"
            sqlq = sqlq & "'" & url & "',"
            sqlq = sqlq & "'" & tbl & "',"
            sqlq = sqlq & "'" & rundate & "',"
            sqlq = sqlq & "'" & ToWhom.ToString & "',"
            sqlq = sqlq & "'" & clr & "',"
            sqlq = sqlq & "'" & delim & "',"
            sqlq = sqlq & "'" & indf & "',"
            sqlq = sqlq & "'" & runnumber & "')"

            ret = ExequteSQLquery(sqlq)
            If ret <> "Query executed fine." Then
                ret = "ERROR!! " & ret
            End If
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try

        Return ret
    End Function

    Public Function GetScheduledImportsIdentifier() As String
        Dim ret As String = String.Empty
        Dim m As Integer = 0
        'assign random identifier
        Dim r As Random = New Random
        m = r.Next(1000) 'Max range
        ret = "d" & Now().ToString.Replace(" ", "").Replace(":", "").Replace("/", "").Replace("M", "") & m.ToString
        Return ret
    End Function
    Private Function SendEmailScheduledImports(ByRef dts As DataTable, ByVal useremail As String) As String
        'NOT IN USE
        Dim ret As String = String.Empty
        Dim sqlu As String = String.Empty
        Dim i, j As Integer
        Dim emails() As String
        Dim url As String = String.Empty
        Dim ext As String = String.Empty
        Try
            For i = 0 To dts.Rows.Count - 1
                'check format of url
                url = dts.Rows(i)("URL").ToString.Trim
                If url.Trim.LastIndexOf(".") <= 0 OrElse Not url.ToLower.StartsWith("http") Then
                    WriteToAccessLog(Session("logon"), "Record deleted from ourscheduledImports. Format of URL does not supported: " & url & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                    'delete record
                    dts.Rows(i)("Status") = "wrong format of url"
                    sqlu = "UPDATE ourscheduledImports SET [Status]='wrong format of url' WHERE ID=" & dts.Rows(i)("ID")
                    ret = ExequteSQLquery(sqlu)

                ElseIf url.ToLower.StartsWith("http") Then
                    ext = url.Trim.Substring(url.Trim.LastIndexOf("."))
                    If ",.CSV,.XML,.JSON,.TXT,.XLS,.XLSX,.MDB,.ACCDB,".IndexOf(ext.ToUpper) < 0 Then
                        WriteToAccessLog(Session("logon"), "Record deleted from ourscheduledImports. Format of URL does not supported: " & url & " It should end with .CSV, .XML, .JSON, .TXT, .XLS, .XLSX, .MDB, .ACCDB ", 2)
                        'delete record
                        dts.Rows(i)("Status") = "wrong format of url"
                        sqlu = "UPDATE ourscheduledImports SET [Status]='wrong format of url' WHERE ID=" & dts.Rows(i)("ID")
                        ret = ExequteSQLquery(sqlu)

                    End If
                End If
                If dts.Rows(i)("Deadline").ToString.Trim.StartsWith(DateToString(Now()).Substring(0, 10)) AndAlso dts.Rows(i)("Status").ToString.Trim = "downloaded" Then
                    'send email with url and rundate
                    Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                    Dim cntus As String = webour & "ContactUs.aspx"
                    Dim emailbody As String = String.Empty
                    emailbody = "Scheduled download completed from the url: " & dts.Rows(i)("URL").ToString & " | See it in your  Imports folder | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                    emailbody = emailbody.Replace("|", Chr(10))
                    If Not dts.Rows(i)("ToWhom").ToString.Trim.Contains(useremail) Then
                        dts.Rows(i)("ToWhom") = useremail & "," & dts.Rows(i)("ToWhom")
                    End If
                    emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                    For j = 0 To emails.Length - 1
                        If emails(j).Trim <> "" Then
                            ret = SendHTMLEmail("", "Scheduled Download completed.", emailbody, emails(j).Trim, Session("SupportEmail"))
                            If Not ret.StartsWith("ERROR!! ") Then
                                'update ScheduledReports
                                sqlu = "UPDATE ourscheduledImports SET [Status]='email sent' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)
                            End If
                        End If
                    Next
                    WriteToAccessLog(Session("logon"), "Sent Email to " & dts.Rows(i)("ToWhom").ToString.Trim & " with body " & emailbody & ". Result: " & ret, 1)

                End If
            Next
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
End Class



