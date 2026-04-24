Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.Windows.Forms.VisualStyles
Partial Class ScheduledReports
    Inherits System.Web.UI.Page
    Public myconstring As String
    Private mUsers As ListItemCollection

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim du As DataView = mRecords("SELECT * FROM ourunits WHERE Unit='" & Session("Unit") & "'")
        If du Is Nothing OrElse du.Count = 0 OrElse du.Table.Rows.Count = 0 Then
            Session("unitindx") = 8
            Session("unitname") = "OUR"
        Else
            Session("unitindx") = du.Table.Rows(0)("Indx")
            Session("unitname") = du.Table.Rows(0)("Unit")
        End If
        Dim re As String = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))

        MessageLabel.Text = ""
        Session("filename") = ""

        Page.MaintainScrollPositionOnPostBack = True
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'TextDate.Text = Now()
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Session("admin") <> "user" And Session("admin") <> "admin" And Session("admin") <> "super" Then
            Response.Redirect("Default.aspx")
            Response.End()
        End If

        Dim i, rid, tn, n As Integer
        Dim towhom, fl, ret, rundate As String

        If Request("export") = "SchedReport" Then
            Session("curmonth") = ""
            Dim filepath As String = Session("SchedReport")
            Dim myfile As String = filepath
            If filepath.LastIndexOf("\") > 0 Then
                myfile = filepath.Substring(filepath.LastIndexOf("\") + 1)
            End If
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile)
                Response.TransmitFile(filepath)
            Catch ex As Exception
                If Not ex.Message.StartsWith("Thread ") Then
                    ret = "ERROR!!  " & ex.Message
                End If
            End Try
            Response.End()
            Exit Sub
        End If

        'current month
        Dim curmonth As String = String.Empty
        If Session("curmonth") Is Nothing OrElse Session("curmonth").ToString.Trim = "" OrElse Not Session("curmonth").ToString.Trim.StartsWith("20") Then
            Session("curmonth") = DateToStringFormat(Now().ToString.Trim, "", "yyyy-MM-dd")
        Else
            Session("curmonth") = DateToStringFormat(Session("curmonth"), "", "yyyy-MM-dd")
        End If

        If Session("curmonth") IsNot Nothing AndAlso Session("curmonth").ToString.Trim.StartsWith("20") AndAlso Session("curmonth").ToString.Trim.Length > 7 Then
            curmonth = Session("curmonth").ToString.Trim.Substring(0, 8)
            'Else
            '    curmonth = DateToStringFormat(Now().ToString.Trim, "", "yyyy-MM-dd").Substring(0, 8)
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
            Response.Redirect("ScheduledReports.aspx")
        End If


        Dim indf As String = String.Empty
        Dim recc As String = String.Empty
        Dim times As String = String.Empty

        'add new schedule
        If Not Request("calndr") Is Nothing AndAlso Request("calndr").ToString.Trim = "add" Then
            'add first scheduled reports
            rundate = Request("date")
            times = Request("times")
            towhom = Request("towhom").Replace(" ", "+")
            indf = GetScheduledReportIdentifier(Session("REPORTID"))
            Session("indf") = indf
            'add future scheduled reports
            If Not Request("ntimes") Is Nothing AndAlso Request("ntimes").ToString.Trim <> "" Then
                n = CInt(Request("ntimes"))
                i = 0
                recc = "run #1 from " & n.ToString & ", " & Request("repeat").ToString
            Else
                n = 0
            End If
            rundate = DateToStringFormat(rundate, "", "yyyy-MM-dd HH:mm:00")
            rundate = rundate.Replace("00:00:00", times)

            Dim added As String = String.Empty
            If Session("See") = "Generic" Then
                added = "&gen=yes&reptype=generic"
            ElseIf Session("See") = "GroupsStats" Then
                added = "&grpstats=yes&reptype=groupsstats"
            ElseIf Session("See") = "Details" Then
                added = "&det=yes&cat1=" & Session("cat1").ToString.Trim & "&cat2=" & Session("cat2").ToString.Trim & "&reptype=detail"
            ElseIf Session("Matrix") = "yes" Then
                added = "&graph=yes&srd=11&cat1=" & Session("cat1").ToString.Trim & "&cat2=" & Session("cat2").ToString.Trim & "&grtype=matrix&y1=" & Session("AxisY").ToString.Trim & "&fn=" & Session("Aggregate").ToString.Trim
            ElseIf Session("Graph") = "yes" Then
                added = "&graph=yes&srd=11&cat1=" & Session("cat1").ToString.Trim & "&cat2=" & Session("cat2").ToString.Trim & "&grtype=" & Session("GraphType").ToString.Trim & "&y1=" & Session("AxisY").ToString.Trim & "&fn=" & Session("Aggregate").ToString.Trim
            End If
            'urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=matrix"  Session("AxisY")  Session("Aggregate")
            'urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=bar" or pie or line  Session("AxisY")  Session("Aggregate")
            'urlc = "ReportViews.aspx?det=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&reptype=detail"
            'urlc = "ReportViews.aspx?gen=yes&reptype=generic"
            'urlc = "ReportViews.aspx?grpstats=yes&reptype=groupstats"


            ret = AddScheduledReport(Session("REPORTID"), Session("logon"), Session("REPTITLE"), Session("WhereText"), Session("srchstm"), rundate, towhom, indf, recc, added)
            If n > 0 AndAlso Not Request("repeat") Is Nothing AndAlso Request("repeat").ToString.Trim <> "" Then
                For i = 1 To n - 1   'first one already scheduled
                    recc = "run #" & (i + 1).ToString & " from " & n.ToString & ", " & Request("repeat").ToString
                    If Request("repeat").ToString.Trim = "daily" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 1, CDate(rundate))))
                        ret = AddScheduledReport(Session("REPORTID"), Session("logon"), Session("REPTITLE"), Session("WhereText"), Session("srchstm"), rundate, towhom, indf, recc, added)
                    ElseIf Request("repeat").ToString.Trim = "weekly" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 7, CDate(rundate))))
                        ret = AddScheduledReport(Session("REPORTID"), Session("logon"), Session("REPTITLE"), Session("WhereText"), Session("srchstm"), rundate, towhom, indf, recc, added)
                    ElseIf Request("repeat").ToString.Trim = "monthly" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Month, 1, CDate(rundate))))
                        ret = AddScheduledReport(Session("REPORTID"), Session("logon"), Session("REPTITLE"), Session("WhereText"), Session("srchstm"), rundate, towhom, indf, recc, added)
                    ElseIf Request("repeat").ToString.Trim = "yearly" Then
                        rundate = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Year, 1, CDate(rundate))))
                        ret = AddScheduledReport(Session("REPORTID"), Session("logon"), Session("REPTITLE"), Session("WhereText"), Session("srchstm"), rundate, towhom, indf, recc, added)
                    End If
                Next
            End If
            Response.Redirect("ScheduledReports.aspx")
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
            fltr = " (Name LIKE '%" & fl & "%' OR Deadline LIKE '%" & fl & "%' OR [Status] LIKE '%" & fl & "%' OR WhereText LIKE '%" & fl & "%' OR Filters LIKE '%" & fl & "%' OR Prop3 LIKE '%" & fl & "%' OR Prop1 LIKE '%" & fl & "%' OR ToWhom LIKE '%" & fl & "%' OR ReportId LIKE '%" & fl & "%') "
        End If

        'If (Session("admin") = "user" Or Session("admin") = "admin") And fl = "" Then
        '    fltr = " Name='" & Session("logon") & "' "
        '    'FirstLetters.Text = Session("logon")
        'ElseIf (Session("admin") = "user" Or Session("admin") = "admin") And fl <> "" Then
        '    fltr = fltr & " AND Name='" & Session("logon") & "' "
        'End If

        'RECORDS calculation start

        'find latest ID
        Dim AssignmentTable As DataTable = Nothing
        Dim sql As String = "SELECT ID FROM ourscheduledreports ORDER BY ID DESC"
        AssignmentTable = mRecords(sql).Table
        If AssignmentTable Is Nothing Then Return
        If AssignmentTable.Rows.Count > 0 Then
            Session("LastTicketNo") = AssignmentTable.Rows(0)("ID").ToString
        Else
            Session("LastTicketNo") = "0"
        End If
        Dim frsite As String = String.Empty
        Dim docs As String = String.Empty
        'colors
        Dim dclr As DataView = mRecords("Select * FROM ourtasklistsetting WHERE Unit=" & Session("unitindx").ToString & " AND [User]='" & Session("logon") & "'")
        If dclr Is Nothing OrElse dclr.Table Is Nothing OrElse dclr.Table.Rows.Count = 0 Then
            dclr = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND Prop3='default'")
        End If
        Session("dcolor") = dclr
        Dim h1txt As String = String.Empty
        Dim f1txt As String = "Prop1='header1'"
        dclr.RowFilter = f1txt
        h1txt = dclr.ToTable.Rows(0)("FldText")
        Dim clr1 As String = dclr.ToTable.Rows(0)("FldColor")
        'Dim clr1 As String = GetTaskListSetting(Session("Unit"), Session("logon"), "header1", h1txt)
        'If h1txt.Trim <> "" Then
        '    Label3.Text = h1txt
        'End If
        If clr1.Trim <> "" Then
            SchedReps.Rows(1).Style("background-color") = clr1
            For i = 0 To SchedReps.Rows(1).Cells.Count - 1
                SchedReps.Rows(1).Cells(i).Style("background-color") = clr1
            Next
        End If

        'dclr = Session("dcolor")
        Dim h2txt As String = String.Empty
        'Dim clr2 As String = GetTaskListSetting(Session("Unit"), Session("logon"), "header2", h2txt)
        Dim f2txt As String = "Prop1='header2'"
        dclr.RowFilter = f2txt
        h2txt = dclr.ToTable.Rows(0)("FldText")
        Dim clr2 As String = dclr.ToTable.Rows(0)("FldColor")
        If h2txt.Trim <> "" Then
            'Label4.Text = h2txt
        End If
        If clr2.Trim <> "" Then
            SchedReps.Rows(2).Style("background-color") = clr2
            For i = 0 To SchedReps.Rows(2).Cells.Count - 1
                SchedReps.Rows(2).Cells(i).Style("background-color") = clr2
            Next
        End If

        If Session("admin") = "super" Then
            sql = "SELECT * FROM ourscheduledreports INNER JOIN OURReportInfo ON (ourscheduledreports.ReportId=OURReportInfo.ReportId) WHERE Deadline LIKE '" & curmonth & "%' AND ReportDB LIKE '%" & Session("ReportDBforSuper").ToString.Trim.Replace(" ", "%") & "%' "
        Else
            sql = "SELECT * FROM ourscheduledreports WHERE Deadline LIKE '" & curmonth & "%' AND UserId='" & Session("logon") & "' "
        End If
        If fltr.Trim <> "" Then sql = sql & " AND (" & fltr & ") "
        sql = sql & " ORDER BY Deadline,ID"

        AssignmentTable = mRecords(sql).Table

        If Not IsPostBack Then
            ret = SendEmailScheduledReports(AssignmentTable, Session("email"))
            If ret <> "" Then
                'refresh AssignmentTable
                If Session("admin") = "super" Then
                    sql = "SELECT * FROM ourscheduledreports INNER JOIN OURReportInfo ON (ourscheduledreports.ReportId=OURReportInfo.ReportId) WHERE Deadline LIKE '" & curmonth & "%' AND ReportDB LIKE '%" & Session("ReportDBforSuper").ToString.Trim.Replace(" ", "%") & "%' "
                Else
                    sql = "SELECT * FROM ourscheduledreports WHERE Deadline LIKE '" & curmonth & "%' AND UserId='" & Session("logon") & "' "
                End If
                If fltr.Trim <> "" Then sql = sql & " AND (" & fltr & ") "
                sql = sql & " ORDER BY Deadline,ID"
                AssignmentTable = mRecords(sql).Table
            End If
        End If

        Label3.Text = "Scheduled Reports for " & curmonth.Replace("-", " ").Trim.Replace(" ", "/")

        If AssignmentTable Is Nothing Then
            Exit Sub
        End If


        Dim whtext As String = String.Empty
        'Draw recodrs on screen
        'Dim clrrow As String
        If AssignmentTable.Rows.Count > 0 Then
            MessageLabel.Text = AssignmentTable.Rows.Count.ToString
            For i = 0 To AssignmentTable.Rows.Count - 1
                'draw regular columns
                ret = AddRowIntoHTMLtableWithNcols(AssignmentTable.Rows(i), SchedReps, 11)
                SchedReps.Rows(i + 4).Cells(0).Align = "left"
                SchedReps.Rows(i + 4).Cells(0).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(1).Align = "left"
                SchedReps.Rows(i + 4).Cells(1).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(2).Align = "left"
                SchedReps.Rows(i + 4).Cells(2).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(3).Align = "left"
                SchedReps.Rows(i + 4).Cells(3).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(4).Align = "left"
                SchedReps.Rows(i + 4).Cells(4).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(5).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(5).Align = "left"
                SchedReps.Rows(i + 4).Cells(6).Align = "left"
                SchedReps.Rows(i + 4).Cells(6).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(6).Style.Item("font-size") = "10px"
                SchedReps.Rows(i + 4).Cells(7).Align = "left"
                SchedReps.Rows(i + 4).Cells(7).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(7).Style.Item("font-size") = "10px"
                SchedReps.Rows(i + 4).Cells(8).Align = "left"
                SchedReps.Rows(i + 4).Cells(8).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(8).Style.Item("font-size") = "10px"
                SchedReps.Rows(i + 4).Cells(9).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(9).Style.Item("font-size") = "10px"
                SchedReps.Rows(i + 4).Cells(10).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(10).Style.Item("font-size") = "10px"
                'dclr = Session("dcolor")
                Dim sttxt As String = AssignmentTable.Rows(i)("Status").ToString.Trim.ToLower
                Dim ftxt As String = "FldText='" & sttxt & "'"
                dclr.RowFilter = ftxt
                If Not dclr.ToTable Is Nothing AndAlso dclr.ToTable.Rows.Count > 0 AndAlso dclr.ToTable.Rows(0)("FldColor").ToString.Trim <> "" Then
                    SchedReps.Rows(i + 4).BgColor = dclr.ToTable.Rows(0)("FldColor")
                End If
                'clrrow = dclr.ToTable.Rows(0)("FldColor")
                'If clrrow.Trim <> "" Then
                '    HelpDesk.Rows(i + 4).BgColor = clrrow
                'End If

                'ftxt = "FldText='" & version & "'"
                dclr.RowFilter = ftxt
                If Not dclr.ToTable Is Nothing AndAlso dclr.ToTable.Rows.Count > 0 AndAlso dclr.ToTable.Rows(0)("FldColor").ToString.Trim <> "" Then
                    SchedReps.Rows(i + 4).Cells(4).BgColor = dclr.ToTable.Rows(0)("FldColor")
                End If

                SchedReps.Rows(i + 4).Cells(0).InnerText = AssignmentTable.Rows(i)("ID").ToString
                SchedReps.Rows(i + 4).Cells(1).InnerHtml = FormatAsHTMLsimple(AssignmentTable.Rows(i)("ReportId").ToString & " | scheduled on " & AssignmentTable.Rows(i)("START").ToString & " | by " & AssignmentTable.Rows(i)("UserId").ToString)
                SchedReps.Rows(i + 4).Cells(2).InnerText = AssignmentTable.Rows(i)("DEADLINE").ToString
                SchedReps.Rows(i + 4).Cells(3).InnerHtml = FormatAsHTMLsimple(AssignmentTable.Rows(i)("NAME").ToString & " | " & AssignmentTable.Rows(i)("Prop1").ToString)
                SchedReps.Rows(i + 4).Cells(4).InnerText = AssignmentTable.Rows(i)("Status").ToString
                whtext = " Parameters: " & AssignmentTable.Rows(i)("WhereText").ToString
                If AssignmentTable.Rows(i)("Filters").ToString.Trim <> "" Then
                    whtext = whtext & " | Filter: " & AssignmentTable.Rows(i)("Filters").ToString
                End If
                SchedReps.Rows(i + 4).Cells(5).InnerHtml = FormatAsHTMLsimple(whtext)
                SchedReps.Rows(i + 4).Cells(6).InnerText = ""
                SchedReps.Rows(i + 4).Cells(6).InnerHtml = ""
                SchedReps.Rows(i + 4).Cells(6).InnerHtml = AssignmentTable.Rows(i)("ToWhom").ToString

                rid = AssignmentTable.Rows(i)("ID").ToString

                SchedReps.Rows(i + 4).Cells(7).InnerText = ""
                Dim ctl As New LinkButton
                ctl.Text = "delete this one"
                ctl.ID = "DelOne ^" & CStr(rid)
                ctl.ToolTip = "Delete this occurrence of scheduled report " & CStr(rid)
                ctl.Style.Item("color") = "blue"
                ctl.Font.Size = 8
                ctl.Font.Italic = True
                AddHandler ctl.Click, AddressOf btnDelOne_Click
                SchedReps.Rows(i + 4).Cells(7).Controls.Add(ctl)

                SchedReps.Rows(i + 4).Cells(8).InnerText = ""
                Dim ctrl = New LinkButton
                ctrl.Text = "delete all occurrences"
                ctrl.ID = "DelAll ^" & CStr(rid)
                ctrl.ToolTip = "Delete all occurrences of scheduled report " & CStr(rid)
                ctrl.Style.Item("color") = "blue"
                ctrl.Font.Size = 8
                ctrl.Font.Italic = True
                AddHandler ctrl.Click, AddressOf btnDelAll_Click
                SchedReps.Rows(i + 4).Cells(8).Controls.Add(ctrl)

                SchedReps.Rows(i + 4).Cells(9).VAlign = "top"
                SchedReps.Rows(i + 4).Cells(9).InnerText = AssignmentTable.Rows(i)("Prop3").ToString

                If Session("indf") <> "" AndAlso AssignmentTable.Rows(i)("Prop3").ToString = Session("indf") Then
                    SchedReps.Rows(i + 4).Focus()
                    SchedReps.Rows(i + 4).Cells(0).Focus()
                    SchedReps.Rows(i + 4).Cells(0).BgColor = "yellow"
                End If

                SchedReps.Rows(i + 4).Cells(10).InnerText = ""
                If curmonth = DateToStringFormat(Now().ToString.Trim, "", "yyyy-MM-dd").Substring(0, 8) AndAlso AssignmentTable.Rows(i)("Prop2") IsNot Nothing AndAlso AssignmentTable.Rows(i)("Prop2").ToString.Trim <> "" Then
                    ctl = New LinkButton
                    ctl.Text = "download"
                    ctl.ID = "Down ^" & CStr(rid)
                    ctl.ToolTip = "Download this occurrence of scheduled report " & CStr(rid) & " from the saved file"
                    ctl.Style.Item("color") = "blue"
                    ctl.Font.Size = 8
                    ctl.Font.Italic = True
                    AddHandler ctl.Click, AddressOf btnDown_Click
                    SchedReps.Rows(i + 4).Cells(10).Controls.Add(ctl)
                End If
            Next
        End If
    End Sub

    Protected Sub btnDelOne_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Dim sql As String = "DELETE FROM ourscheduledreports WHERE ID=" & id & " AND UserId='" & Session("logon") & "'"
        Dim ret As String = String.Empty
        ret = ExequteSQLquery(sql)
        If ret <> "Query executed fine." Then
            ret = "ERROR!! " & ret
            MessageBox.Show(ret, "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        Else
            MessageBox.Show("Report deleted from the list of Scheduled Reports", "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
        End If
        Response.Redirect("ScheduledReports.aspx")
    End Sub
    Protected Sub btnDelAll_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM ourscheduledreports WHERE ID = " & id, err).ToTable
        Dim indf As String = dt.Rows(0)("Prop3").ToString
        Dim sql As String = "DELETE FROM ourscheduledreports WHERE Prop3='" & indf & "' AND UserId='" & Session("logon") & "'"
        Dim ret As String = String.Empty
        ret = ExequteSQLquery(sql)
        If ret <> "Query executed fine." Then
            ret = "ERROR!! " & ret
            MessageBox.Show(ret, "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        Else
            MessageBox.Show("Reccurence deleted from the list of Scheduled Reports", "Delete Schedule", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
        End If
        Response.Redirect("ScheduledReports.aspx")
    End Sub
    Protected Sub btnDown_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Dim ret As String = String.Empty
        Dim filepath As String = String.Empty
        Dim myfile As String = String.Empty
        Dim sql As String = "SELECT * FROM ourscheduledreports WHERE ID=" & id '& " AND UserId='" & Session("logon") & "'"

        Dim dtr As DataView = mRecords(sql, ret)
        If ret.Trim <> "" Then
            'ret = "ERROR!! " & ret
            MessageBox.Show(ret, "Download Scheduled Report", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        Else

            If dtr IsNot Nothing AndAlso dtr.Table.Rows.Count = 1 Then
                filepath = dtr.Table.Rows(0)("Prop2")
                filepath = filepath.Replace("*", "\")
                myfile = filepath.Substring(filepath.LastIndexOf("\") + 1)
                Session("SchedReport") = filepath
            End If

            If filepath.Trim <> "" Then
                Session("curmonth") = ""
                Response.Redirect("ScheduledReports.aspx?export=SchedReport")
                'Try
                '    Response.ContentType = "application/octet-stream"
                '    Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile)
                '    Response.TransmitFile(filepath)
                'Catch ex As Exception
                '    If Not ex.Message.StartsWith("Thread ") Then
                '        ret = "ERROR!!  " & ex.Message
                '    End If
                'End Try
                'Response.End()
                ''Exit Sub
            End If
        End If
        'Response.Redirect("ScheduledReports.aspx")
    End Sub

    'Private Sub ScheduledReports_PreLoad(sender As Object, e As EventArgs) Handles Me.PreLoad
    '    Dim ret As String = String.Empty
    '    If Request("export") = "SchedReport" Then
    '        Try
    '            Response.ContentType = "application/octet-stream"
    '            Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("SchedReport"))
    '            Response.TransmitFile(Session("SchedReport"))
    '        Catch ex As Exception
    '            If Not ex.Message.StartsWith("Thread ") Then
    '                ret = "ERROR!!  " & ex.Message
    '            End If
    '        End Try
    '        'Response.End()
    '        Exit Sub
    '    End If
    'End Sub

    Private Function AddScheduledReport(ByVal rep As String, ByVal logon As String, ByVal repttl As String, ByVal WhereText As String, ByVal srchstm As String, ByVal rundate As String, ByVal ToWhom As String, ByVal indf As String, ByVal runnumber As String, Optional ByVal added As String = "") As String
        Dim ret As String = String.Empty
        Dim sqlq As String = String.Empty
        Try
            sqlq = "INSERT INTO ourscheduledreports ([Start],[Status],UserId,ReportId,Name,WhereText,Filters,Deadline,ToWhom,Prop3,Prop4,Prop5,Prop1) VALUES ('" & DateToString(Now()) & "','scheduled',"
            sqlq = sqlq & "'" & logon & "',"
            sqlq = sqlq & "'" & rep & "',"
            sqlq = sqlq & "'" & repttl & "',"
            sqlq = sqlq & "'" & WhereText.Replace("'", """").Replace("[", "").Replace("]", "") & "',"
            sqlq = sqlq & "'" & srchstm.Replace("'", """").Replace("[", "").Replace("]", "") & "',"
            sqlq = sqlq & "'" & rundate & "',"
            sqlq = sqlq & "'" & ToWhom & "',"
            sqlq = sqlq & "'" & indf & "',"
            sqlq = sqlq & "'" & Session("pagewidth").ToString & "~" & Session("pageheight") & "',"
            sqlq = sqlq & "'" & added & "',"
            sqlq = sqlq & "'" & runnumber & "')"

            ret = ExequteSQLquery(sqlq)
            If ret <> "Query executed fine." Then
                ret = "ERROR!! " & ret
            End If

            'sqlq = "UPDATE ourscheduledreports SET Prop4='" & Session("pagewidth").ToString & "~" & Session("pageheight").ToString & "' WHERE .............."  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'ret = ExequteSQLquery(sqlq)
            'If ret <> "Query executed fine." Then
            '    ret = "ERROR!! " & ret
            'End If

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try

        Return ret
    End Function
    Private Function SendEmailScheduledReports(ByVal dts As DataTable, ByVal useremail As String) As String
        Dim ret As String = String.Empty
        Dim sqlu As String = String.Empty
        Dim i, j As Integer
        Dim emails() As String
        Try
            For i = 0 To dts.Rows.Count - 1
                'dts.Rows(i)("Deadline").ToString.Trim.StartsWith(DateToString(Now()).Substring(0, 10)) 
                If Not (dts.Rows(i)("Status").ToString.Trim = "processed") AndAlso Not (dts.Rows(i)("Status").ToString.Trim = "email sent") AndAlso dts.Rows(i)("Deadline").ToString.Trim < DateToString(Now()) AndAlso ((dts.Rows(i)("Deadline").ToString.Trim.StartsWith(DateToString(Now()).Substring(0, 10)) OrElse (dts.Rows(i)("Deadline").ToString.Trim.StartsWith(DateToString(Now.AddDays(-1)).Substring(0, 10))))) Then
                    'send email with link default.aspx?srd=6&sched=yes&rep=" & repid & "&reid=" & indf & "&rundate=" & rundate
                    Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
                    Dim cntus As String = webour & "ContactUs.aspx"
                    Dim lnk As String = webour & "default.aspx?srd=6&sched=yes&rep=" & dts.Rows(i)("ReportId").ToString.Trim & "&reid=" & dts.Rows(i)("Prop3").ToString.Trim & "&rundate=" & dts.Rows(i)("Deadline").ToString.Trim.Substring(0, 10)
                    Dim emailbody As String = String.Empty
                    emailbody = "Click to download up to date report: " & lnk & " . | Access to the report is available only today. | Please be patient, it might take longer to complete big reports. |  | Do not answer to this email. | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
                    emailbody = emailbody.Replace("|", Chr(10))
                    If Not dts.Rows(i)("ToWhom").ToString.Trim.Contains(useremail) Then
                        dts.Rows(i)("ToWhom") = useremail & "," & dts.Rows(i)("ToWhom")
                    End If
                    emails = dts.Rows(i)("ToWhom").ToString.Trim.Split(",")
                    For j = 0 To emails.Length - 1
                        If emails(j).Trim <> "" Then
                            ret = SendHTMLEmail("", "Up To Date Report is shared with you today only.", emailbody, emails(j).Trim, Session("SupportEmail"))
                            If Not ret.StartsWith("ERROR!! ") Then
                                'update ScheduledReports
                                sqlu = "UPDATE ourscheduledreports SET [Status]='email sent' WHERE ID=" & dts.Rows(i)("ID")
                                ret = ExequteSQLquery(sqlu)
                            End If
                        End If
                    Next
                    WriteToAccessLog(Session("logon"), "Sent Email to " & dts.Rows(i)("ToWhom").ToString.Trim & " with body " & emailbody & ". Result: " & ret, 1)
                    'MessageBox.Show(ret, "Report share", "ReportShared", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

                End If
            Next
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
End Class



