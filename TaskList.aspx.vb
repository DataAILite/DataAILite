Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Partial Class TaskList
    Inherits System.Web.UI.Page
    Public myconstring As String
    Private mUsers As ListItemCollection
    Private sql As String
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim pw As String = Request("pass")
        If pw = "help" Then 'AndAlso Not Request("ln") Is Nothing AndAlso Request("ln").Trim <> "" Then
            'assign rights and open HelpDesk
            WriteToAccessLog(Session("logon"), "Help requested from the site " & pw, 10)
            Session("tn") = 0
            Session("Application") = "Tasklist"
            Session("logon") = cleanText(Request("ln"))
            Session("userdbname") = cleanText(Request("db"))
            Session("admin") = "user" 'cleanText(Request("rol"))
            Session("Unit") = cleanText(Request("unit"))
            Session("FromSite") = "yes"
            Session("OURConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("OURConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
            WriteToAccessLog(Session("logon"), "Help requested from the site " & Session("Unit") & " , database " & Session("userdbname"), 10)
            Response.Redirect("TaskList.aspx")
        End If
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Session("WEBOUR") = ConfigurationManager.AppSettings("weboureports").ToString
        HyperLink2.NavigateUrl = Session("WEBOUR")
        Session("urlback") = "TaskList.aspx"
        If Session("Topic") Is Nothing OrElse Session("Topic").ToString = "" Then
            Session("Topic") = "All"
        End If
        Dim i As Integer = 0
        'colors and ddTopics
        If Not IsPostBack Then
            Dim du As DataView
            du = mRecords("SELECT * FROM ourunits WHERE Unit='" & Session("Unit") & "' ORDER BY Prop2")
            If du Is Nothing OrElse du.Count = 0 OrElse du.Table.Rows.Count = 0 Then
                Session("unitindx") = 8
                Session("unitname") = "OUR"
                ddTopics.Visible = False
                Dim ret As String = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))
            Else
                Session("unitindx") = du.Table.Rows(0)("Indx")
                Session("unitname") = du.Table.Rows(0)("Unit")
                ddTopics.Visible = True
                ddTopics.Items.Clear()

                If Session("Group2") Is Nothing OrElse Session("Group2").ToString.Trim = "" OrElse Session("Group2").ToString.Trim = "All" Then
                    ddTopics.Items.Add("All")
                    ddTopics.SelectedIndex = 0
                    ddTopics.Text = "All"
                    For i = 0 To du.Table.Rows.Count - 1
                        ddTopics.Items.Add(du.Table.Rows(i)("Prop2").ToString.Trim)
                        If Session("Topic").Trim = du.Table.Rows(i)("Prop2").ToString.Trim Then
                            ddTopics.SelectedIndex = i
                            ddTopics.Text = du.Table.Rows(i)("Prop2").ToString
                        End If
                    Next
                Else
                    Session("Topic") = Session("Group2").ToString.Trim
                        ddTopics.Items.Add(Session("Topic"))
                        ddTopics.SelectedIndex = 0
                        ddTopics.Text = Session("Topic")
                        ddTopics.Enabled = False
                    End If
                End If
                Dim re As String = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))
        End If

        Dim drvalue, drname As String
        Dim sqlr As String = String.Empty
        Dim rsc As DataTable = Nothing
        Label3.Text = ConfigurationManager.AppSettings("pagettl").ToString '"OUR place: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
        If Session("Access") = "TEAMADMIN" OrElse Session("Access") = "TOPICADMIN" Then
            HyperLink1.Visible = True
            HyperLink1.Enabled = True
        Else
            HyperLink1.Visible = False
            HyperLink1.Enabled = False
        End If
        'To Whom
        If Session("FromSite") = "yes" Then
            'show dropdowns
            If Session("admin") = "user" Then
                'find the site support contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='SUPPORT' OR Access='TEAMADMIN') AND Application='" & Session("Application").ToString.Trim & "' AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%')"
            ElseIf Session("admin") = "admin" Then
                'find the site support or tester contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='TEST' OR Access='SUPPORT' OR Access='TEAMADMIN') AND Application='" & Session("Application").ToString.Trim & "' AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%')"
            ElseIf Session("admin") = "super" Then
                sqlr = "SELECT * FROM OURPermits WHERE (Access='TEST' OR Access='SUPPORT' OR Access='TEAMADMIN') AND Application='" & Session("Application").ToString.Trim & "' AND (Unit='" & Session("Unit") & "'))"
            End If
        Else
            'Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim

            If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
                If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
            Else
                If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
            End If


            Label1.Text = "Database:  " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            'show dropdowns
            If Session("admin") = "user" Then
                'find the site support contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='SUPPORT' OR Access='TEAMADMIN') AND Application='" & Session("Application").ToString.Trim & "' AND (Unit='" & Session("Unit") & "')  AND ConnStr LIKE '" & Session("UserDB") & "%'"
                Label1.Visible = False
            ElseIf Session("admin") = "admin" Then
                'find users
                If Session("Topic") = "All" Then
                    sqlr = "SELECT * FROM OURPermits WHERE Application='" & Session("Application").ToString.Trim & "' AND (Unit='" & Session("Unit") & "')  AND ConnStr LIKE '" & Session("UserDB") & "%'"
                Else
                    sqlr = "SELECT * FROM OURPermits WHERE Application='" & Session("Application").ToString.Trim & "' AND (Unit='" & Session("Unit") & "') AND ((Group2='" & Session("Group2") & "') OR (Group2='All') OR (Group2='')) AND ConnStr LIKE '" & Session("UserDB") & "%'"
                End If
            ElseIf Session("admin") = "super" Then
                'find users
                If Session("Topic") = "All" Then
                    sqlr = "SELECT * FROM OURPermits WHERE Application='" & Session("Application").ToString.Trim & "' " 'AND (Unit='" & Session("Unit") & "')"
                Else
                    sqlr = "SELECT * FROM OURPermits WHERE Application='" & Session("Application").ToString.Trim & "' AND ((Group2='" & Session("Group2") & "') OR (Group2='All') OR (Group2='')) " 'AND (Unit='" & Session("Unit") & "') 
                End If
            End If
        End If

        rsc = mRecords(sqlr).Table  'ToWhom

        If Session("FromSite") = "yes" Then
            If rsc Is Nothing OrElse rsc.Rows.Count < 1 Then
                Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                'Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim

                If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
                    If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                    If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
                Else
                    If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
                End If


                Label1.Text = "Database: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            Else
                Session("UserConnString") = rsc.Rows(i)("ConnStr").ToString
                Session("UserConnProvider") = rsc.Rows(i)("ConnPrv").ToString
                'Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim

                If Session("UserConnProvider") <> "Oracle.ManagedDataAccess.Client" Then
                    If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
                    If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
                Else
                    If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
                End If


                Label1.Text = "Database: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            End If

        End If

        DropDownListWho.Text = Session("logon")
        mUsers = New ListItemCollection
        'fill out the dropdown list of who and whom
        If rsc Is Nothing OrElse rsc.Rows.Count < 1 Then
            drvalue = Session("logon")
            drname = Session("logon")
            If drvalue <> "" Then
                mUsers.Add(drvalue)
            End If
        Else
            For i = 0 To rsc.Rows.Count - 1
                drvalue = Trim(rsc.Rows(i)("NetID").ToString)
                drname = Trim(rsc.Rows(i)("NAME").ToString)
                If drvalue <> "" Then
                    mUsers.Add(drvalue)
                End If
            Next
        End If

        MessageLabel.Text = ""
        Session("filename") = ""
        Head1.Title = Session("PAGETTL")
        Page.MaintainScrollPositionOnPostBack = True
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        TextDate.Text = Now()
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Request("calendarfile") IsNot Nothing And Request("calendarfile") <> String.Empty Then
            Response.Redirect(Request("calendarfile"))
        End If
        If Request("assignmentfile") IsNot Nothing And Request("assignmentfile") <> String.Empty Then
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
            Catch ex As Exception

            End Try
            Response.Redirect(Request("assignmentfile"))
        End If

        If Session("admin") <> "user" And Session("admin") <> "admin" And Session("admin") <> "super" Then
            Response.Redirect("Default.aspx")
            Response.End()
        End If
        Dim i, rid, tn, tni As Integer
        'colors and ddTopics
        If Not IsPostBack Then
            Dim re As String = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))
        End If

        Dim comments, towhom, who, version, mainText, fl As String
        Dim bShowNotDone As Boolean = False
        If Request("shownotdone") IsNot Nothing AndAlso Request("shownotdone").ToString = "1" Then
            bShowNotDone = True
            ckNotDoneOnly.Checked = True
            'chkHowTo.Checked = False
        Else
            ckNotDoneOnly.Checked = False
        End If

        comments = ""
        towhom = ""
        If Not Request("tn") Is Nothing AndAlso Request("tn").Trim <> "" Then
            Session("tn") = CInt(cleanText(Request("tn"))).ToString
        End If
        tn = Session("tn")

        'search
        If FirstLetters.Text.Trim <> "" Then
            fl = FirstLetters.Text.Trim
        Else
            fl = ""
        End If
        Session("FirstLetters") = fl

        Dim fltr As String = String.Empty
        If fl.Trim <> "" Then
            fltr = " (Name LIKE '%" & fl & "%' OR [Status] LIKE '%" & fl & "%' OR Ticket LIKE '%" & fl & "%' OR comments LIKE '%" & fl & "%' OR ToWhom LIKE '%" & fl & "%' OR Version LIKE '%" & fl & "%') "
        End If

        If (Session("admin") = "super") And fl = "" Then
            fltr = ""
        ElseIf (Session("admin") = "admin") And fl = "" Then
            fltr = " Prop1='" & Session("Unit") & "' "
        ElseIf (Session("admin") = "super") And fl <> "" Then
            fltr = fltr & ""
        ElseIf (Session("admin") = "admin") And fl <> "" Then
            fltr = fltr & " AND Prop1='" & Session("Unit") & "' "
        ElseIf Session("admin") = "user" And Session("Access") <> "TEAMADMIN" And fl = "" Then
            fltr = " Name='" & Session("logon") & "' AND Prop1='" & Session("Unit") & "' "
        ElseIf Session("admin") = "user" And Session("Access") <> "TEAMADMIN" And fl <> "" Then
            fltr = fltr & " AND Name='" & Session("logon") & "' AND Prop1='" & Session("Unit") & "' "
        End If
        If Session("Topic") <> "All" Then
            fltr = fltr & " AND Prop2='" & Session("Topic") & "' "
        End If

        'RECORDS calculation start

        Dim AssignmentTable As DataTable = Nothing
        Dim sql1 As String = "SELECT ID FROM OURHelpDesk ORDER BY ID DESC"
        AssignmentTable = mRecords(sql1).Table
        If AssignmentTable Is Nothing Then Return
        If AssignmentTable.Rows.Count > 0 Then
            Session("LastTicketNo") = AssignmentTable.Rows(0)("ID").ToString
        Else
            Session("LastTicketNo") = "0"
        End If
        If Not IsPostBack AndAlso Not Request("calndr") Is Nothing AndAlso Request("calndr").ToString.Trim = "yes" Then
            'btnAddTicket_Click
            Session("calndr") = "yes"
            If tn = 0 Then
                Dim dat As String = DateToString(Request("date"))
                Dim TicketData As New Controls_TicketDlg.TicketData
                dlgTicket.UserItems = mUsers
                Dim LastTicketNo As Integer = 0
                LastTicketNo = CInt(Session("LastTicketNo"))
                TicketData.ID = (LastTicketNo + 1).ToString
                TicketData.From = Session("logon")
                TicketData.Deadline = dat.Substring(0, dat.IndexOf(" ")).Trim
                dlgTicket.Show("Add Ticket (User = " & Session("logon") & ")", TicketData, Controls_TicketDlg.Mode.Add, "Add Ticket")
            End If
        End If

        Dim frsite As String = String.Empty
        Dim flr As String = String.Empty

        'colors
        Dim dclr As DataView = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "'")
        Session("dcolor") = dclr
        Dim h1txt As String = String.Empty
        Dim f1txt As String = "Prop1='header1'"
        dclr.RowFilter = f1txt
        h1txt = dclr.ToTable.Rows(0)("FldText")
        Dim clr1 As String = dclr.ToTable.Rows(0)("FldColor")
        'Dim clr1 As String = GetTaskListSetting(Session("Unit"), Session("logon"), "header1", h1txt)
        If h1txt.Trim <> "" Then
            Label3.Text = h1txt
        End If
        If clr1.Trim <> "" Then
            HelpDesk.Rows(1).Style("background-color") = clr1
            For i = 0 To HelpDesk.Rows(1).Cells.Count - 1
                HelpDesk.Rows(1).Cells(i).Style("background-color") = clr1
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
            Label4.Text = h2txt
        End If
        If clr2.Trim <> "" Then
            HelpDesk.Rows(2).Style("background-color") = clr2
            For i = 0 To HelpDesk.Rows(2).Cells.Count - 1
                HelpDesk.Rows(2).Cells(i).Style("background-color") = clr2
            Next
        End If

        If Session("FromSite") = "yes" Then
            flr = " ([Status]='documentation') OR "
            frsite = " [Status] Not In ('how to','knowledge') AND "
        End If
        If chkHowTo.Checked Then
            If fltr.Trim <> "" Then fltr = fltr & " AND "
            sql = "SELECT * FROM OURHelpDesk WHERE " & flr & " (" & fltr & " [Status] In ('how to','knowledge','documentation')) ORDER BY ID DESC"
        Else
            If ckNotDoneOnly.Checked Then
                ' chkHowTo.Checked = False
                If fltr.Trim <> "" Then fltr = fltr & " AND "
                sql = "SELECT * FROM OURHelpDesk WHERE (" & fltr & " [Status] Not In ('done','deleted','dismissed','how to','knowledge','documentation')) ORDER BY ID DESC"
            Else
                If fltr.Trim <> "" OrElse frsite <> "" Then fltr = " WHERE  " & frsite & " (" & fltr
                sql = "SELECT * FROM OURHelpDesk " & fltr
                If fltr.Trim <> "" Then sql = sql & ") "
                sql = sql & " ORDER BY ID DESC"
            End If
        End If

        AssignmentTable = mRecords(sql).Table
        Session("Assignments") = AssignmentTable
        Dim enddate As String = String.Empty
        If AssignmentTable Is Nothing Then
            Exit Sub
        End If
        'Draw recodrs on screen
        'Dim clrrow As String
        If AssignmentTable.Rows.Count > 0 Then
            MessageLabel.Text = AssignmentTable.Rows.Count.ToString
            Dim sCSVFiles As String = GetAssignmentsCSV()
            If sCSVFiles <> String.Empty Then
                Dim shref As String = "TaskList.aspx?assignmentfile=" & sCSVFiles
                btnDown.Attributes.Add("href", shref)
            End If
            LogOff.NavigateUrl = "Default.aspx?logoff=" & Session("logon")
            For i = 0 To AssignmentTable.Rows.Count - 1
                'draw regular columns
                AddRowIntoHTMLtable(AssignmentTable.Rows(i), HelpDesk)
                HelpDesk.Rows(i + 4).BgColor = "lightyellow"
                comments = MakeLinks(AssignmentTable.Rows(i)("COMMENTS").ToString)
                towhom = AssignmentTable.Rows(i)("ToWhom").ToString
                who = AssignmentTable.Rows(i)("NAME").ToString
                version = AssignmentTable.Rows(i)("Version").ToString
                HelpDesk.Rows(i + 4).Cells(6).InnerHtml = FormatAsHTML(comments.Replace(vbLf, vbCrLf)) & "<br/>"
                mainText = MakeLinks(AssignmentTable.Rows(i)("TICKET").ToString)
                HelpDesk.Rows(i + 4).Cells(4).InnerHtml = FormatAsHTMLsimple(mainText.Replace(vbLf, "<br/>")) & "<br/>"
                HelpDesk.Rows(i + 4).Cells(4).Align = "left"
                HelpDesk.Rows(i + 4).Cells(4).Align = "left"
                HelpDesk.Rows(i + 4).Cells(5).Align = "left"
                HelpDesk.Rows(i + 4).Cells(0).Align = "left"
                HelpDesk.Rows(i + 4).Cells(0).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(1).Align = "left"
                HelpDesk.Rows(i + 4).Cells(1).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(2).Align = "left"
                HelpDesk.Rows(i + 4).Cells(2).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(3).Align = "left"
                HelpDesk.Rows(i + 4).Cells(3).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(4).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(5).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(5).Align = "left"
                HelpDesk.Rows(i + 4).Cells(6).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(6).Align = "left"
                HelpDesk.Rows(i + 4).Cells(7).Align = "left"
                HelpDesk.Rows(i + 4).Cells(7).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(7).Style.Item("font-size") = "10px"

                rid = AssignmentTable.Rows(i)("ID").ToString

                Dim sttxt As String = AssignmentTable.Rows(i)("Status").ToString.Trim.ToLower
                Dim ftxt As String = "FldText='" & sttxt & "'"
                dclr.RowFilter = ftxt
                If Not dclr.ToTable Is Nothing AndAlso dclr.ToTable.Rows.Count > 0 AndAlso dclr.ToTable.Rows(0)("FldColor").ToString.Trim <> "" Then
                    HelpDesk.Rows(i + 4).BgColor = dclr.ToTable.Rows(0)("FldColor")
                End If

                ftxt = "FldText='" & version & "'"
                dclr.RowFilter = ftxt
                If Not dclr.ToTable Is Nothing AndAlso dclr.ToTable.Rows.Count > 0 AndAlso dclr.ToTable.Rows(0)("FldColor").ToString.Trim <> "" Then
                    HelpDesk.Rows(i + 4).Cells(1).BgColor = dclr.ToTable.Rows(0)("FldColor")
                End If

                HelpDesk.Rows(i + 4).Cells(1).InnerText = version
                HelpDesk.Rows(i + 4).Cells(2).InnerHtml = FormatAsHTMLsimple(AssignmentTable.Rows(i)("NAME").ToString & " | " & AssignmentTable.Rows(i)("START").ToString)
                HelpDesk.Rows(i + 4).Cells(5).InnerText = AssignmentTable.Rows(i)("Status").ToString
                If AssignmentTable.Rows(i)("Deadline").ToString.Trim <> "" Then
                    enddate = ""
                    Try
                        enddate = DateToString(AssignmentTable.Rows(i)("Deadline").ToString)


                        Dim ts As TimeSpan = CDate(enddate) - Now
                        Dim days As Integer = CInt(ts.TotalDays)
                        If days >= 0 Then
                            If days < 4 Then
                                HelpDesk.Rows(i + 4).Cells(3).BgColor = "red"
                                'HelpDesk.Rows(i + 4).Cells(3).BorderColor = "red"
                            ElseIf days > 3 AndAlso days < 7 Then
                                HelpDesk.Rows(i + 4).Cells(3).BgColor = "#fb4d67"
                                'HelpDesk.Rows(i + 4).Cells(3).BorderColor = "#fb4d67"
                            ElseIf days > 6 AndAlso days < 10 Then
                                HelpDesk.Rows(i + 4).Cells(3).BgColor = "pink"
                                'HelpDesk.Rows(i + 4).Cells(3).BorderColor = "pink"
                            End If
                        End If

                        'calendar
                        If days >= 0 Then
                            HelpDesk.Rows(i + 4).Cells(3).InnerHtml = AssignmentTable.Rows(i)("Deadline").ToString & "<br/>" & days.ToString & " days left <br/>"
                            'Dim ctle As New LinkButton
                            Dim ctle As New HyperLink
                            ctle.Text = "add to calendar"
                            ctle.ID = "Calendar ^" & CStr(rid)
                            ctle.ToolTip = "Add to the Calendar the task #" & CStr(rid)
                            ctle.Style.Item("color") = "blue"
                            ctle.Font.Size = 8
                            ctle.Font.Italic = True
                            Dim sFile As String = GetCalendarFile(CStr(rid))
                            ctle.NavigateUrl = "TaskList.aspx?calendarfile=" & sFile
                            'AddHandler ctle.Click, AddressOf btnCalendar_Click
                            HelpDesk.Rows(i + 4).Cells(3).Controls.Add(ctle)
                        Else
                            HelpDesk.Rows(i + 4).Cells(3).InnerHtml = AssignmentTable.Rows(i)("Deadline").ToString & "<br/><br/>"
                        End If
                    Catch ex As Exception
                        HelpDesk.Rows(i + 4).Cells(3).InnerText = ""
                    End Try
                Else
                    HelpDesk.Rows(i + 4).Cells(3).InnerText = ""
                End If


                If rid = tn Then
                    HelpDesk.Rows(i + 4).Focus()
                    HelpDesk.Rows(i + 4).Cells(0).Focus()
                    HelpDesk.Rows(i + 4).Cells(0).BgColor = "yellow"  '="LightCoral" 
                    tni = i + 1
                    If Not IsPostBack AndAlso Not Request("calndr") Is Nothing AndAlso Request("calndr").ToString.Trim = "yes" Then
                        'btnEdit_Click
                        Session("calndr") = "yes"
                        Dim data As New Controls_TicketDlg.TicketData
                        Dim err As String = String.Empty
                        Dim dt As DataTable = mRecords("SELECT * FROM OURHelpDesk WHERE ID = " & rid, err).ToTable
                        If err = String.Empty AndAlso dt.Rows.Count > 0 Then
                            dlgTicket.UserItems = mUsers
                            data.From = dt.Rows(0)("Name").ToString
                            data.DateTime = dt.Rows(0)("Start").ToString
                            data.ID = dt.Rows(0)("ID").ToString
                            data.Version = dt.Rows(0)("Version").ToString
                            'data.Deadline = dt.Rows(0)("Deadline").ToString
                            Try
                                data.Deadline = DateToString(dt.Rows(0)("Deadline").ToString)
                            Catch ex As Exception
                                data.Deadline = ""
                            End Try
                            data.Description = dt.Rows(0)("Ticket").ToString
                            data.Status = dt.Rows(0)("Status").ToString
                            data.To = dt.Rows(0)("ToWhom").ToString
                            data.Comments = dt.Rows(0)("comments").ToString
                            dlgTicket.Show("Edit Ticket (User = " & Session("logon") & ")", data, Controls_TicketDlg.Mode.Edit, "Update Ticket")
                        End If

                    End If
                End If

                Dim ctl As New LinkButton
                ctl.Text = "edit"
                ctl.ID = "Edit ^" & CStr(rid)
                ctl.ToolTip = "Edit Ticket " & CStr(rid) & ". Topic: " & AssignmentTable.Rows(i)("Prop2").ToString
                ctl.Style.Item("color") = "blue"
                ctl.Font.Size = 8
                ctl.Font.Italic = True
                AddHandler ctl.Click, AddressOf btnEdit_Click
                HelpDesk.Rows(i + 4).Cells(0).InnerHtml = AssignmentTable.Rows(i)("ID").ToString & "<br/>"
                HelpDesk.Rows(i + 4).Cells(0).Controls.Add(ctl)

                HelpDesk.Rows(i + 4).BorderColor = "black"
                HelpDesk.Rows(i + 4).Cells(7).InnerHtml = towhom.Replace(",", ", ")
            Next
        End If
    End Sub
    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Session("tn") = id
        Dim data As New Controls_TicketDlg.TicketData
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM OURHelpDesk WHERE ID = " & id, err).ToTable
        If err = String.Empty AndAlso dt.Rows.Count > 0 Then
            dlgTicket.UserItems = mUsers
            data.From = dt.Rows(0)("Name").ToString
            data.DateTime = dt.Rows(0)("Start").ToString
            data.ID = dt.Rows(0)("ID").ToString
            data.Version = dt.Rows(0)("Version").ToString
            data.Deadline = dt.Rows(0)("Deadline").ToString
            data.Description = dt.Rows(0)("Ticket").ToString
            data.Status = dt.Rows(0)("Status").ToString
            data.To = dt.Rows(0)("ToWhom").ToString
            data.Comments = dt.Rows(0)("comments").ToString
            dlgTicket.Show("Edit Ticket (User = " & Session("logon") & ")", data, Controls_TicketDlg.Mode.Edit, "Update Ticket")
        End If
    End Sub
    Private Function GetAssignmentsCSV() As String
        If Not Session("Assignments") Is Nothing Then
            Dim dtt As DataTable = Session("Assignments")
            Dim res, appldirExcelFiles, myfile As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datestr, timestr As String
            datestr = DateString()
            timestr = TimeString()
            myfile = Session("Application") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
            appldirExcelFiles = applpath & "Temp\"
            Session("appldirExcelFiles") = appldirExcelFiles
            Session("myfile") = myfile
            'header
            Dim hdr As String = "Data for " & Session("logon") & " on " & Label3.Text & ", Topic - " & ddTopics.Text & " "
            hdr = hdr + System.Environment.NewLine
            Dim delmtr As String = ","
            res = ExportDataTableToExcel(dtt, appldirExcelFiles, myfile, hdr, "", delmtr)
            res = "~/Temp/" & Session("myfile")

            Return res
        Else
            Return String.Empty
        End If
    End Function
    Private Function GetCalendarFile(id As String) As String
        Dim err As String = String.Empty
        Dim res As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM OURHelpDesk WHERE ID = " & id, err).ToTable
        If err = String.Empty AndAlso dt.Rows.Count > 0 Then
            Dim appldirFiles, myfile As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datestr, timestr As String

            datestr = DateString()
            timestr = TimeString()
            myfile = Session("Application") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & "_" & id & ".ics"
            appldirFiles = applpath & "Temp\"
            Dim File As StreamWriter = New StreamWriter(appldirFiles & myfile)
            Try
                'calendar
                Dim cldr As String = "BEGIN:VCALENDAR"
                File.WriteLine(cldr)

                cldr = "VERSION:2.0"
                File.WriteLine(cldr)
                cldr = "PRODID:<TaskList TeamWorks>"
                File.WriteLine(cldr)
                cldr = "BEGIN:VEVENT"
                File.WriteLine(cldr)


                cldr = "DESCRIPTION: " & dt.Rows(0)("Ticket").ToString.Replace("|", " ").Replace(":", " ")
                File.WriteLine(cldr)
                cldr = "SUMMARY:Deadline for Task " & id
                File.WriteLine(cldr)
                If dt.Rows(0)("Start").ToString <> "" Then
                    cldr = "DTSTART:" & DateToString(dt.Rows(0)("Start").ToString).Replace(" ", "T").Replace("-", "").Replace(":", "")
                    File.WriteLine(cldr)
                End If
                If dt.Rows(0)("Deadline").ToString <> "" Then
                    cldr = "DTEND:" & DateToString(dt.Rows(0)("Deadline").ToString).Replace(" ", "T").Replace("-", "").Replace("00:00:00", "100000")
                    File.WriteLine(cldr)
                End If
                If dt.Rows(0)("Deadline").ToString <> "" Then
                    cldr = "DTSTAMP:" & DateToString(dt.Rows(0)("Deadline").ToString).Replace(" ", "T").Replace("-", "").Replace("00:00:00", "100000")
                    File.WriteLine(cldr)
                End If


                cldr = "END:VEVENT"
                File.WriteLine(cldr)
                cldr = "END:VCALENDAR"
                File.WriteLine(cldr)
                File.Flush()
                File.Close()
                File = Nothing
                res = appldirFiles & myfile
            Catch ex As Exception
                res = "Error: " & ex.Message
                File.Close()
                File = Nothing
                Return res
            End Try
            res = "~/Temp/" & myfile
        End If

        Return res
    End Function
    Protected Sub btnCalendar_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Session("tn") = id
        Dim data As New Controls_TicketDlg.TicketData
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM OURHelpDesk WHERE ID = " & id, err).ToTable
        If err = String.Empty AndAlso dt.Rows.Count > 0 Then
            Dim Res, appldirFiles, myfile As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datestr, timestr As String

            datestr = DateString()
            timestr = TimeString()
            myfile = Session("Application") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".ics"
            appldirFiles = applpath & "Temp\"
            Dim File As StreamWriter = New StreamWriter(appldirFiles & myfile)
            Try
                'calendar
                Dim cldr As String = "BEGIN:VCALENDAR"
                File.WriteLine(cldr)

                cldr = "VERSION:2.0"
                File.WriteLine(cldr)
                cldr = "PRODID:<TaskList TeamWorks>"
                File.WriteLine(cldr)
                cldr = "BEGIN:VEVENT"
                File.WriteLine(cldr)


                cldr = "DESCRIPTION: " & dt.Rows(0)("Ticket").ToString.Replace("|", " ").Replace(":", " ")
                File.WriteLine(cldr)
                cldr = "SUMMARY:Deadline for Task " & id
                File.WriteLine(cldr)
                If dt.Rows(0)("Start").ToString <> "" Then
                    cldr = "DTSTART:" & DateToString(dt.Rows(0)("Start").ToString).Replace(" ", "T").Replace("-", "").Replace(":", "")
                    File.WriteLine(cldr)
                End If
                If dt.Rows(0)("Deadline").ToString <> "" Then
                    cldr = "DTEND:" & DateToString(dt.Rows(0)("Deadline").ToString).Replace(" ", "T").Replace("-", "").Replace("00:00:00", "100000")
                    File.WriteLine(cldr)
                End If
                If dt.Rows(0)("Deadline").ToString <> "" Then
                    cldr = "DTSTAMP:" & DateToString(dt.Rows(0)("Deadline").ToString).Replace(" ", "T").Replace("-", "").Replace("00:00:00", "100000")
                    File.WriteLine(cldr)
                End If


                cldr = "END:VEVENT"
                File.WriteLine(cldr)
                cldr = "END:VCALENDAR"
                File.WriteLine(cldr)

                File.Flush()
                File.Close()
                File = Nothing
                Res = appldirFiles & myfile
            Catch ex As Exception
                Res = ex.Message
                File.Close()
                File = Nothing
            End Try

            Res = "~/Temp/" & myfile
            Response.Redirect(Res)
        End If
    End Sub
    Protected Sub btnCalendarCSV_Click(sender As Object, e As EventArgs)
        Dim id As String = Piece(CType(sender, LinkButton).ID, "^", 2)
        Session("tn") = id
        Dim data As New Controls_TicketDlg.TicketData
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM OURHelpDesk WHERE ID = " & id, err).ToTable
        If err = String.Empty AndAlso dt.Rows.Count > 0 Then
            Dim res, appldirFiles, myfile As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datestr, timestr As String
            datestr = DateString()
            timestr = TimeString()
            myfile = Session("Application") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
            appldirFiles = applpath & "Temp\"
            Dim File As StreamWriter = New StreamWriter(appldirFiles & myfile)
            Try
                'calendar
                Dim cldr As String = "Subject,Start Date,End Date,All Day Event,Description,Location,Private"
                File.WriteLine(cldr)

                Dim cldt As String = "Task #" & id & "," & dt.Rows(0)("Start").ToString & "," & dt.Rows(0)("Deadline").ToString & ",True," & dt.Rows(0)("Ticket").ToString & ",TaskList,True"
                File.WriteLine(cldt)

                File.Flush()
                File.Close()
                File = Nothing
                res = appldirFiles & myfile
            Catch ex As Exception
                res = ex.Message
                File.Close()
                File = Nothing
            End Try

            res = "~/Temp/" & myfile
            Response.Redirect(res)
        End If
    End Sub
    Protected Sub ButtonAddAssignment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddAssignment.Click
        Dim tdata = TextDate.Text
        'Dim j As Integer
        Dim SQLq As String
        Dim ret As String
        If Trim(LabelWho.Text) = "" Then LabelWho.Text = DropDownListWho.SelectedValue
        If Trim(LabelWho.Text) = "" Then LabelWho.Text = Session("logon")
        If Trim(LabelWhom.Text) = "" Then LabelWhom.Text = DropDownListWhom.SelectedValue
        LabelWhom.Text = cleanTextFromRepeatedCommas(LabelWhom.Text) & ","
        SQLq = "INSERT INTO OURHelpDesk (Start,Name,Ticket,[Status],COMMENTS, ToWhom,Version) VALUES('" & TextDate.Text & "','" & LabelWho.Text & "','" & cleanText(TextTopics.Text) & "','" & cleanText(TextDecisions.Text) & "','" & cleanText(TextComments.Text) & "','" & LabelWhom.Text & "')"
        ret = ExequteSQLquery(SQLq)
        ret = ret.Replace("Query executed fine.", "").Trim
        If ret = "" Then
            'send emails
            Dim EmailTable As DataTable
            'send emails
            EmailTable = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='" & Session("Application").ToString.Trim & "')").Table
            'If EmailTable.Rows.Count > 0 Then
            '    For j = 0 To EmailTable.Rows.Count - 1
            '        SendHTMLEmail("", "Status: " & cleanText(TextDecisions.Text) & ". New Ticket: " & Left(cleanText(TextTopics.Text), 90) & ". ", "New ticket in Help Desk on http://OUReports.com  from " & DropDownListWho.Text & ", Status: " & cleanText(TextDecisions.Text) & ":   " & cleanText(TextTopics.Text) & ". Comments:  " & cleanText(TextComments.Text) & " - This is auto message from Help Desk OUReports.com site.  Please do not reply on this email.", EmailTable.Rows(j)("Email"), Session("SupportEmail"))
            '    Next
            'End If
        End If
        Response.Redirect("TaskList.aspx")
    End Sub

    Protected Sub ButtonAddWho_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddWho.Click
        If Request(DropDownListWho.UniqueID) = "all" Then
            'add everybody
            Dim everybody As DataTable
            Dim i As Integer
            everybody = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='" & Session("Application").ToString.Trim & "')").Table
            For i = 0 To everybody.Rows.Count - 1
                LabelWho.Text = LabelWho.Text & ", " & Trim(everybody.Rows(i)("NETID"))
            Next
        Else
            LabelWho.Text = LabelWho.Text & ", " & Trim(Request(DropDownListWho.UniqueID))
        End If
        LabelWho.Text = LabelWho.Text & ", " & Request(DropDownListWho.UniqueID)
    End Sub

    Protected Sub ButtonAddWhom_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddWhom.Click
        'LabelWhom.Text = Request(DropDownListWhom.UniqueID) & ", " & LabelWhom.Text
        If Request(DropDownListWhom.UniqueID) = "ALL" Then
            'add everybody
            Dim everybody As DataTable
            Dim i As Integer
            everybody = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='" & Session("Application").ToString.Trim & "')").Table
            For i = 0 To everybody.Rows.Count - 1
                LabelWhom.Text = LabelWhom.Text & ", " & Trim(everybody.Rows(i)("NETID"))
            Next
        Else
            LabelWhom.Text = LabelWhom.Text & ", " & Trim(Request(DropDownListWhom.UniqueID))
        End If
    End Sub
    Function NewAddress(ByVal FileNew As HttpPostedFile) As String
        'file upload
        If Session("admin") <> "user" And Session("admin") <> "admin" Then
            Response.Redirect("~/Default.aspx?msg=No rights to upload a file")
            Response.End()
        End If
        If Not (FileNew Is Nothing) Then
            Try
                'Dim postedFile = FileO.PostedFile
                Dim filename As String = Path.GetFileName(FileNew.FileName)
                Dim filenamefix As String
                filename = Now.ToString & "_" & filename
                filenamefix = Replace(filename, " ", "_")
                filenamefix = Replace(filenamefix, ",", "_")
                filenamefix = Replace(filenamefix, "#", "_")
                filenamefix = Replace(filenamefix, ":", "-")
                filenamefix = Replace(filenamefix, "/", "-")
                Dim fixpoint As String
                fixpoint = Replace(Left(filenamefix, Len(filenamefix) - 5), ".", "_")
                filenamefix = fixpoint & Right(filenamefix, 5)

                Dim contentType As String = FileNew.ContentType
                Dim contentLength As Integer = FileNew.ContentLength
                Dim dir As String
                dir = Request.PhysicalApplicationPath & "Temp\"  'files directory 
                FileNew.SaveAs(dir & filenamefix)
                'MessageLabel.Text = FileNew.FileName & " uploaded" & _
                '  "<br>content type: " & contentType & _
                '  "<br>content length: " & contentLength.ToString()
                Session("filename") = filename
                NewAddress = Session("applpath") & "Temp/" & filenamefix
            Catch exc As Exception
                'MessageLabel.Text = "Failed uploading file"
            End Try
        Else
            'MessageLabel.Text = "Select your local file first..."
        End If
        Return ""
    End Function

    Protected Sub ButtonAttach_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAttach.Click
        If Session("admin") <> "user" And Session("admin") <> "admin" Then
            Response.Redirect("~/Default.aspx?msg=No rights to attach a file")
            Response.End()
        End If
        If Not (FileO.PostedFile Is Nothing) Then
            Try
                Dim postedFile = FileO.PostedFile
                Dim filename As String = Path.GetFileName(postedFile.FileName)
                Dim filenamefix As String
                filenamefix = Replace(filename, " ", "_")
                filenamefix = Replace(filenamefix, ",", "_")
                filenamefix = Replace(filenamefix, "#", "_")
                filenamefix = Replace(filenamefix, ":", "-")
                filenamefix = Replace(filenamefix, "/", "-")
                Dim fixpoint As String
                fixpoint = Replace(Left(filenamefix, Len(filenamefix) - 5), ".", "_")
                filenamefix = fixpoint & Right(filenamefix, 5)
                Dim contentType As String = postedFile.ContentType
                Dim contentLength As Integer = postedFile.ContentLength
                Dim dir As String
                dir = Request.PhysicalApplicationPath & "SAVEDFILES\"  'files directory 
                postedFile.SaveAs(dir & filenamefix)
                'MessageLabel.Text = postedFile.Filename & " uploaded" & _
                '  "<br>content type: " & contentType & _
                '  "<br>content length: " & contentLength.ToString()
                Session("filename") = filename
                TextComments.Text = TextComments.Text & " | File attached: " & "SAVEDFILES/" & filenamefix & " ."
            Catch exc As Exception
                'MessageLabel.Text = "Failed uploading file"
            End Try
        Else
            'MessageLabel.Text = "Select your local file first..."
        End If
    End Sub

    Private Sub btnAddTicket_Click(sender As Object, e As EventArgs) Handles btnAddTicket.Click
        Dim TicketData As New Controls_TicketDlg.TicketData
        dlgTicket.UserItems = mUsers
        Dim LastTicketNo As Integer = 0
        LastTicketNo = CInt(Session("LastTicketNo"))
        TicketData.ID = (LastTicketNo + 1).ToString
        TicketData.From = Session("logon")
        dlgTicket.Show("Add Ticket (User = " & Session("logon") & ")", TicketData, Controls_TicketDlg.Mode.Add, "Add Ticket")
    End Sub
    Private Sub SaveTicket(data As Controls_TicketDlg.TicketData, IsEdit As Boolean)
        Dim SQLq As String = "INSERT INTO OURHelpDesk "
        Dim tick As String = "New ticket # " & data.ID & ": "
        Dim tick2 As String = "New ticket # " & data.ID
        'If IsEdit Then
        '    SQLq = "UPDATE OURHelpDesk "
        '    tick = "Ticket #" & data.ID & " edited: "
        '    tick2 = "Ticket #" & data.ID
        'End If
        'SQLq &= "SET Start = '" & data.DateTime & "',"
        'SQLq &= "Name = '" & data.From & "',"
        'SQLq &= "Version = '" & data.Version & "',"
        'SQLq &= "Deadline = '" & data.Deadline & "',"
        'SQLq &= "Prop1 = '" & Session("Unit") & "',"
        'If Not IsEdit Then SQLq &= "Prop2 = '" & Session("Topic") & "',"
        'SQLq &= "Ticket = '" & cleanTextLight(data.Description) & "',"
        'SQLq &= "[Status] = '" & cleanText(data.Status) & "',"
        'If data.Comments <> String.Empty Then
        '    SQLq &= "COMMENTS = '" & cleanTextLight(data.Comments) & "',"
        'End If
        'SQLq &= "ToWhom = '" & data.To & "'"
        'If IsEdit Then SQLq &= " WHERE ID = " & data.ID


        If IsEdit Then
            SQLq = "UPDATE OURHelpDesk "
            tick = "Ticket #" & data.ID & " edited: "
            tick2 = "Ticket #" & data.ID
            SQLq &= "SET Start = '" & data.DateTime & "',"
            SQLq &= "Name = '" & data.From & "',"
            SQLq &= "Version = '" & data.Version & "',"
            SQLq &= "Deadline = '" & data.Deadline & "',"
            'If Not IsEdit Then SQLq &= "Prop2 = '" & Session("Topic") & "',"
            SQLq &= "Prop1 = '" & Session("Unit") & "',"
            SQLq &= "Ticket = '" & cleanTextLight(data.Description) & "',"
            SQLq &= "[Status] = '" & cleanText(data.Status) & "',"
            If data.Comments <> String.Empty Then
                SQLq &= "COMMENTS = '" & cleanTextLight(data.Comments) & "',"
            End If
            SQLq &= "ToWhom = '" & data.To & "'"
            SQLq &= " WHERE ID = " & data.ID
        Else
            Dim sFields As String = "Start,Name,Version,Ticket,[Status],Deadline,Prop1,Prop2"
            If data.Comments <> String.Empty Then
                sFields &= ",COMMENTS"
            End If
            sFields &= ",ToWhom"
            Dim sValues As String = "'" & data.DateTime & "',"
            sValues &= "'" & data.From & "',"
            sValues &= "'" & data.Version & "',"
            sValues &= "'" & cleanTextLight(data.Description) & "',"
            sValues &= "'" & cleanText(data.Status) & "',"
            sValues &= "'" & data.Deadline & "',"
            sValues &= "'" & Session("Unit") & "',"
            sValues &= "'" & Session("Topic") & "',"
            If data.Comments <> String.Empty Then
                sValues &= "'" & cleanTextLight(data.Comments) & "',"
            End If
            sValues &= "'" & data.To & "'"
            SQLq = "INSERT INTO OURHelpDesk (" & sFields & ") VALUES (" & sValues & ")"
        End If

        Dim ret As String = ExequteSQLquery(SQLq)
        ret = ret.Replace("Query executed fine.", "").Trim

        If ret = String.Empty AndAlso data.To <> String.Empty Then
            Session("tn") = data.ID
            Dim sTo As String = String.Empty
            Dim sStart As String = "SELECT Email FROM OURPERMITS WHERE (NetId = '"
            Dim sSQl As String = String.Empty
            Dim err As String = String.Empty
            Dim sDesc As String = cleanText(data.Description)
            Dim sSubject As String = "Status: " & cleanText(data.Status) & ". " & tick & Left(sDesc, 90)
            If sDesc.Length > 90 Then
                sSubject &= "..."
            End If
            Dim sBody As String = String.Empty
            Dim sComments As String = cleanTextLight(data.Comments)
            If sComments.Trim <> "" Then
                sComments = sComments.Replace("|", " " & vbCrLf & " ").Replace(vbLf, vbCrLf)
            End If
            Dim dt As DataTable = Nothing
            Dim remail As String = String.Empty
            For i As Integer = 1 To Pieces(data.To, ",")
                sTo = Piece(data.To, ",", i)
                sSQl = sStart & sTo & "')  AND (Application='" & Session("Application").ToString.Trim & "') "
                dt = mRecords(sSQl, err).Table
                If err = String.Empty AndAlso dt.Rows.Count > 0 Then
                    Dim EmailAddress As String = dt.Rows(0)("Email").ToString
                    If EmailAddress <> String.Empty Then
                        sBody = tick & vbCrLf
                        sBody &= "Initiated by: " & data.From & " " & vbCrLf & " "
                        sBody &= "Status: " & cleanText(data.Status) & " " & vbCrLf & " "
                        sBody &= "Version: " & cleanText(data.Version) & " " & vbCrLf & " "
                        sBody &= "Deadline: " & data.Deadline & " " & vbCrLf & " "
                        sBody &= "Description: " & cleanText(data.Description) & " " & vbCrLf & " "
                        sBody &= "Comments:" & " " & vbCrLf & " "
                        sBody &= cleanText(sComments) & " " & vbCrLf & " " & "--------------------------" & vbCrLf
                        sBody &= tick2 & " is in HelpDesk on https://OUReports.net/HelpDesk/default.aspx?tn=" & data.ID & " " & vbCrLf & " " & vbCrLf
                        sBody &= "This is auto message from HelpDesk on https://OUReports.net/HelpDesk/default.aspx.  Please Do Not reply to this email."
                        remail = SendHTMLEmail(data.Attachment, sSubject, sBody, EmailAddress, Session("SupportEmail"))
                        WriteToAccessLog(Session("logon"), "Email was sent to - " & sTo & " with result: " & remail, 3)
                    Else
                        WriteToAccessLog(Session("logon"), "Save Ticket - " & sTo & " has no email address defined.", 0)
                    End If
                ElseIf err <> String.Empty Then
                    MessageBox.Show(err, "Save Ticket - Get email address", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Exit Sub
                Else
                    WriteToAccessLog(Session("logon"), "Save Ticket - " & sTo & " is not defined.", 0)
                    'MessageBox.Show("No records returned.", "Save Ticket - Get email address", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    'Return
                End If
            Next

        ElseIf ret <> String.Empty Then
            MessageBox.Show(ret, "Save Ticket", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If
        If Session("calndr") = "yes" Then
            Session("Assignments") = mRecords(sql).Table
            Response.Redirect("TaskListCalendar.aspx?tn=" & data.ID.ToString)
        End If
        If Request("shownotdone") IsNot Nothing AndAlso Request("shownotdone").ToString = "1" Then
            Response.Redirect("TaskList.aspx?shownotdone=1")
        Else
            Response.Redirect("TaskList.aspx")
        End If
    End Sub
    Private Sub CancelTicket()
        If Session("calndr") = "yes" Then
            Session("Assignments") = mRecords(sql).Table
            Response.Redirect("TaskListCalendar.aspx")
        End If
    End Sub
    Private Sub dlgTicket_TicketDialogResulted(sender As Object, e As Controls_TicketDlg.TicketDlgEventArgs) Handles dlgTicket.TicketDialogResulted
        If e.Result = Controls_TicketDlg.TicketDialogResult.Cancel Then
            CancelTicket()
        ElseIf e.EntryMode = Controls_TicketDlg.Mode.Add Then
            SaveTicket(e.TicketItem, False)
        ElseIf e.EntryMode = Controls_TicketDlg.Mode.Edit Then
            SaveTicket(e.TicketItem, True)
        End If
    End Sub

    Private Sub chkHowTo_CheckedChanged(sender As Object, e As EventArgs) Handles chkHowTo.CheckedChanged
        If chkHowTo.Checked Then
            ckNotDoneOnly.Checked = False
        End If
    End Sub
    Private Sub ckNotDoneOnly_CheckedChanged(sender As Object, e As EventArgs) Handles ckNotDoneOnly.CheckedChanged
        If ckNotDoneOnly.Checked Then
            chkHowTo.Checked = False
            Response.Redirect("TaskList.aspx?shownotdone=0")
        Else
            Response.Redirect("TaskList.aspx?shownotdone=1")
        End If
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        'Dim srch As String = FirstLetters.Text.Trim
        'Session("FirstLetters") = srch
        'Response.Redirect("~/TaskList.aspx?FirstLetters=" & srch)
    End Sub

    Private Sub ddTopics_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddTopics.SelectedIndexChanged
        Session("Topic") = ddTopics.SelectedValue
        Session("TopicIndx") = ddTopics.SelectedIndex
        'Session("ddTopic") = ddTopics
        Response.Redirect("TaskList.aspx")
    End Sub
    Private Sub btTopic_Click(sender As Object, e As EventArgs) Handles btTopic.Click

        Response.Redirect("TeamAdmin.aspx")
    End Sub

    Private Sub HelpDesk_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        'TODO delete user ics and csv files

    End Sub
End Class



