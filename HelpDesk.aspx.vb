Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Partial Class HelpDesk
    Inherits System.Web.UI.Page
    Public myconstring As String
    Private mUsers As ListItemCollection

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim pw As String = Request("pass")
        If pw = "help" Then 'AndAlso Not Request("ln") Is Nothing AndAlso Request("ln").Trim <> "" Then
            'assign rights and open HelpDesk
            WriteToAccessLog(Session("logon"), "Help requested from the site " & pw, 10)
            Session("tn") = 0
            Session("Application") = "InteractiveReporting"
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
            Response.Redirect("HelpDesk.aspx")
        End If
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim er As String = ""
        Dim du As DataView = mRecords("SELECT * FROM ourunits WHERE [Unit]='" & Session("Unit") & "'", er)
        If du Is Nothing OrElse du.Count = 0 OrElse du.Table.Rows.Count = 0 Then
            Session("unitindx") = 3 '3 for OUReports, 8 for RunReport
            Session("unitname") = "OUR"
        Else
            Session("unitindx") = du.Table.Rows(0)("Indx")
            Session("unitname") = du.Table.Rows(0)("Unit")
        End If
        Dim re As String = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))

        Dim drvalue, drname As String
        Dim i As Integer
        Dim sqlr As String = String.Empty
        Dim rsc As DataTable = Nothing
        'Label3.Text = ConfigurationManager.AppSettings("pagettl").ToString '"OUR place: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim

        If Session("FromSite") = "yes" Then
            'show dropdowns
            If Session("admin") = "user" Then
                Label1.Visible = False
                'find the site support contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='SUPPORT' OR Access='SiteAdmin')  AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%') AND (Application='InteractiveReporting')"
            ElseIf Session("admin") = "admin" Then
                'find the site support or tester contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%') AND (Application='InteractiveReporting')"
            ElseIf Session("admin") = "super" Then
                'all DEV and Site admin, support, or tester
                sqlr = "SELECT * FROM OURPermits WHERE Application='InteractiveReporting' AND (Access='DEV' OR ((Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND (Unit='" & Session("Unit") & "') AND  (ConnStr LIKE '%" & Session("userdbname").Trim.Replace(" ", "%") & "%') )) "
            End If
        Else
            Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim
            Label1.Text = "Database:  " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            'show dropdowns
            If Session("admin") = "user" Then
                Label1.Visible = False
                'find the site support contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='SUPPORT' OR Access='SiteAdmin') AND ConnStr LIKE '" & Session("UserDB") & "%' AND Application='InteractiveReporting'"
            ElseIf Session("admin") = "admin" Then
                'find the site support or tester contact
                sqlr = "SELECT * FROM OURPermits WHERE (Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin') AND ConnStr LIKE '" & Session("UserDB") & "%' AND Application='InteractiveReporting'"
            ElseIf Session("admin") = "super" Then
                'all DEV and Site admin, support, or tester
                sqlr = "SELECT * FROM OURPermits WHERE Application='InteractiveReporting' AND (Access='DEV' OR Access='TEST' OR Access='SUPPORT' OR Access='SiteAdmin')"
            End If
        End If
        rsc = mRecords(sqlr).Table

        If Session("FromSite") = "yes" Then
            If rsc Is Nothing OrElse rsc.Rows.Count < 1 Then
                Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
                Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
                Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim
                Label1.Text = "Database: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            Else
                Session("UserConnString") = rsc.Rows(i)("ConnStr").ToString
                Session("UserConnProvider") = rsc.Rows(i)("ConnPrv").ToString
                Session("UserDB") = Session("UserConnString").ToString.Substring(0, Session("UserConnString").ToString.IndexOf("User ID")).Trim
                Label1.Text = "Database: " & Session("UserDB").Substring(Session("UserDB").LastIndexOf("=")).Replace("=", "").Replace(";", "").Trim
            End If
            HyperLink3.Visible = False
            HyperLink3.Enabled = False
        End If

        Session("urlback") = "HelpDesk.aspx"
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

        Page.MaintainScrollPositionOnPostBack = True
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        TextDate.Text = Now()
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Session("admin") <> "user" And Session("admin") <> "admin" And Session("admin") <> "super" Then
            Response.Redirect("Default.aspx")
            Response.End()
        End If

        Dim i, rid, tn, tni As Integer
        Dim m, s, comments, towhom, who, version, SQLq, mainText, newfileurl, fl, addtoRowi As String
        Dim bShowNotDone As Boolean = False
        If Request("shownotdone") IsNot Nothing AndAlso Request("shownotdone").ToString = "1" Then
            bShowNotDone = True
            ckNotDoneOnly.Checked = True
            'chkHowTo.Checked = False
        Else
            ckNotDoneOnly.Checked = False
        End If
        'Dim bShowHowTo As Boolean = False
        'If Request("showhowto") IsNot Nothing AndAlso Request("showhowto").ToString = "1" Then
        '    bShowHowTo = True
        '    chkHowTo.Checked = True
        'Else
        '    chkHowTo.Checked = False
        'End If
        comments = ""
        towhom = ""
        tn = Session("tn")

        ''search
        'fl = ""
        'If Request("FirstLetters") IsNot Nothing Then
        '    fl = Request("FirstLetters").ToString.Trim
        '    Session("FirstLetters") = fl
        'End If
        'If fl = "" AndAlso Session("FirstLetters") IsNot Nothing AndAlso Session("FirstLetters").ToString.Trim <> "" Then
        '    fl = Session("FirstLetters").ToString
        'Else
        '    Session("FirstLetters") = fl
        'End If
        'FirstLetters.Text = fl
        'Session("FirstLetters") = fl

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

        If (Session("admin") = "user" Or Session("admin") = "admin") And fl = "" Then
            fltr = " Name='" & Session("logon") & "' "
            'FirstLetters.Text = Session("logon")
        ElseIf (Session("admin") = "user" Or Session("admin") = "admin") And fl <> "" Then
            fltr = fltr & " AND Name='" & Session("logon") & "' "
        End If

        'RECORDS calculation start

        Dim AssignmentTable As DataTable = Nothing
        Dim sql As String = "SELECT ID FROM OURHelpDesk ORDER BY ID DESC"
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
        Dim er As String = String.Empty
        Dim dclr As DataView = mRecords("Select * FROM ourtasklistsetting WHERE Unit=" & Session("unitindx").ToString & " AND [User]='" & Session("logon") & "'")
        If dclr Is Nothing OrElse dclr.Table Is Nothing OrElse dclr.Table.Rows.Count = 0 Then
            er = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))
            dclr = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "'", er)
            If dclr Is Nothing OrElse dclr.Table Is Nothing OrElse dclr.Table.Rows.Count = 0 Then
                dclr = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND Prop3='default'", er)
            End If
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
            docs = " ([Status]='documentation') OR "
            frsite = " [Status] Not In ('how to','knowledge') AND "
        End If

        'If chkHowTo.Checked Then
        '    If fltr.Trim <> "" Then fltr = fltr & " AND "
        '    sql = "SELECT * FROM OURHelpDesk WHERE " & docs & " (" & fltr & " [Status] In ('how to','knowledge','documentation')) ORDER BY ID DESC"
        'Else
        '    If ckNotDoneOnly.Checked Then
        '        ' chkHowTo.Checked = False
        '        If fltr.Trim <> "" Then fltr = fltr & " AND "
        '        sql = "SELECT * FROM OURHelpDesk WHERE (" & fltr & " [Status] Not In ('done','deleted','dismissed','how to','knowledge','documentation')) ORDER BY ID DESC"
        '    Else
        '        If fltr.Trim <> "" OrElse frsite <> "" Then fltr = " WHERE  " & frsite & " (" & fltr
        '        sql = "SELECT * FROM OURHelpDesk " & fltr
        '        If fltr.Trim <> "" Then sql = sql & ") "
        '        sql = sql & " ORDER BY ID DESC"
        '    End If
        'End If

        sql = "SELECT "
        Dim ourdbpr As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ProviderName.ToString.Trim
        If ourdbpr <> "Oracle.ManagedDataAccess.Client" AndAlso ourdbpr <> "Npgsql" Then
            sql = sql & " TOP 1500 "
        End If

        If chkHowTo.Checked Then
            If fltr.Trim <> "" Then fltr = fltr & " AND "
            sql = sql & " * FROM OURHelpDesk WHERE " & docs & " (" & fltr & " [Status] In ('how to','knowledge','documentation')) ORDER BY ID DESC"
        Else
            If ckNotDoneOnly.Checked Then
                ' chkHowTo.Checked = False
                If fltr.Trim <> "" Then fltr = fltr & " AND "
                sql = sql & " * FROM OURHelpDesk WHERE (" & fltr & " [Status] Not In ('done','deleted','dismissed','how to','knowledge','documentation')) ORDER BY ID DESC"
            Else
                If fltr.Trim <> "" OrElse frsite <> "" Then fltr = " WHERE  " & frsite & " (" & fltr
                sql = sql & " * FROM OURHelpDesk " & fltr
                If fltr.Trim <> "" Then sql = sql & ") "
                sql = sql & " ORDER BY ID DESC"
            End If
        End If

        AssignmentTable = mRecords(sql).Table

        If AssignmentTable Is Nothing Then
            Exit Sub
        End If
        'Draw recodrs on screen
        'Dim clrrow As String
        If AssignmentTable.Rows.Count > 0 Then
            MessageLabel.Text = AssignmentTable.Rows.Count.ToString
            For i = 0 To AssignmentTable.Rows.Count - 1
                'draw regular columns
                AddRowIntoHTMLtable(AssignmentTable.Rows(i), HelpDesk)
                HelpDesk.Rows(i + 4).BgColor = "lightyellow"
                comments = MakeLinks(AssignmentTable.Rows(i)("COMMENTS").ToString) 'HelpDesk.Rows(i + 4).Cells(5).InnerText
                towhom = AssignmentTable.Rows(i)("ToWhom").ToString
                who = AssignmentTable.Rows(i)("NAME").ToString
                version = AssignmentTable.Rows(i)("Version").ToString
                HelpDesk.Rows(i + 4).Cells(5).InnerHtml = FormatAsHTML(comments.Replace(vbLf, vbCrLf))
                mainText = MakeLinks(AssignmentTable.Rows(i)("TICKET").ToString)
                HelpDesk.Rows(i + 4).Cells(3).InnerHtml = FormatAsHTMLsimple(mainText.Replace(vbLf, "<br/>"))
                HelpDesk.Rows(i + 4).Cells(3).Align = "left"
                HelpDesk.Rows(i + 4).Cells(5).Align = "left"
                HelpDesk.Rows(i + 4).Cells(0).Align = "left"
                HelpDesk.Rows(i + 4).Cells(0).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(1).Align = "left"
                HelpDesk.Rows(i + 4).Cells(1).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(2).Align = "left"
                HelpDesk.Rows(i + 4).Cells(2).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(3).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(4).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(5).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(6).Align = "left"
                HelpDesk.Rows(i + 4).Cells(6).VAlign = "top"
                HelpDesk.Rows(i + 4).Cells(6).Style.Item("font-size") = "10px"
                'dclr = Session("dcolor")
                Dim sttxt As String = AssignmentTable.Rows(i)("STATUS").ToString.Trim.ToLower
                Dim ftxt As String = "FldText='" & sttxt & "'"
                dclr.RowFilter = ftxt
                If Not dclr.ToTable Is Nothing AndAlso dclr.ToTable.Rows.Count > 0 AndAlso dclr.ToTable.Rows(0)("FldColor").ToString.Trim <> "" Then
                    HelpDesk.Rows(i + 4).BgColor = dclr.ToTable.Rows(0)("FldColor")
                End If
                'clrrow = dclr.ToTable.Rows(0)("FldColor")
                'If clrrow.Trim <> "" Then
                '    HelpDesk.Rows(i + 4).BgColor = clrrow
                'End If

                ftxt = "FldText='" & version & "'"
                dclr.RowFilter = ftxt
                If Not dclr.ToTable Is Nothing AndAlso dclr.ToTable.Rows.Count > 0 AndAlso dclr.ToTable.Rows(0)("FldColor").ToString.Trim <> "" Then
                    HelpDesk.Rows(i + 4).Cells(1).BgColor = dclr.ToTable.Rows(0)("FldColor")
                End If



                HelpDesk.Rows(i + 4).Cells(0).InnerText = AssignmentTable.Rows(i)("ID").ToString
                HelpDesk.Rows(i + 4).Cells(1).InnerText = version
                HelpDesk.Rows(i + 4).Cells(2).InnerHtml = FormatAsHTMLsimple(AssignmentTable.Rows(i)("NAME").ToString & " | " & AssignmentTable.Rows(i)("START").ToString)
                HelpDesk.Rows(i + 4).Cells(4).InnerText = AssignmentTable.Rows(i)("STATUS").ToString
                rid = AssignmentTable.Rows(i)("ID").ToString
                If rid = tn Then
                    HelpDesk.Rows(i + 4).Focus()
                    HelpDesk.Rows(i + 4).Cells(6).Focus()
                    HelpDesk.Rows(i + 4).Cells(0).BgColor = "yellow"  '="LightCoral" 
                    tni = i + 1
                End If

                'draw buttons in last column and correct some fields when "edit" clicked
                If Request("EditComments" & CInt(rid)) <> "" Then
                    HelpDesk.Rows(i + 4).BorderColor = "red"
                    comments = AssignmentTable.Rows(i)("COMMENTS").ToString
                    'comments = HelpDesk.Rows(i + 4).Cells(5).InnerText
                    HelpDesk.Rows(i + 4).Cells(5).InnerHtml = FormatAsHTMLsimple(comments) & "<br><textarea name=AddComments" & CStr(rid) & " rows=8 cols=40 >  </textarea><br/>Attach:<br /><input name='File" & CStr(rid) & "' id='File" & CStr(rid) & "' type='file'  style='width: 328px'/>"
                    HelpDesk.Rows(i + 4).Cells(5).InnerHtml = HelpDesk.Rows(i + 4).Cells(5).InnerHtml & "<br /><input type='checkbox' value='1' name='noemails" & CStr(rid) & "'> Delete me from the email list for this ticket."
                    Session("modifyRecord") = "yes"
                    'edit other fields for admin
                    If Session("admin") = "admin" Or Session("admin") = "super" Then
                        HelpDesk.Rows(i + 4).Cells(1).InnerHtml = "<textarea name=AddStart" & CStr(rid) & " rows=1 cols=10 >" & HelpDesk.Rows(i + 4).Cells(1).InnerText & "</textarea>"
                        HelpDesk.Rows(i + 4).Cells(2).InnerHtml = "<textarea name=AddFor" & CStr(rid) & " rows=2 cols=10 >" & HelpDesk.Rows(i + 4).Cells(2).InnerText & "</textarea>"
                        HelpDesk.Rows(i + 4).Cells(3).InnerHtml = "<textarea name=AddAssign" & CStr(rid) & " rows=8 cols=40 >" & AssignmentTable.Rows(i)("Ticket") & "</textarea>"
                        HelpDesk.Rows(i + 4).Cells(4).InnerHtml = "<textarea name=AddDeadline" & CStr(rid) & " rows=2 cols=20 >" & HelpDesk.Rows(i + 4).Cells(4).InnerText & "</textarea>"
                        HelpDesk.Rows(i + 4).Cells(6).InnerHtml = "<textarea name=AddTo" & CStr(rid) & " rows=2 cols=10 >" & AssignmentTable.Rows(i)("ToWhom") & "</textarea>" & "<br/><input type='submit' value='submit' name='SaveComments" & CStr(rid) & "'> " '<input type='hidden' name='s' value='" & CStr(i) & "'><input type='hidden' name='m' value=''>"  'submit button
                    Else
                        HelpDesk.Rows(i + 4).Cells(6).InnerHtml = towhom & "<br/><br/><input type='submit' value='submit' name='SaveComments" & CStr(rid) & "'> " '> <input type='hidden' name='s' value='" & CStr(i) & "'><input type='hidden' name='m' value=''>"  'submit button
                    End If
                Else
                    HelpDesk.Rows(i + 4).Cells(6).InnerHtml = towhom & "<br/><br/>"
                    Dim ctl As New LinkButton
                    ctl.Text = "edit"
                    ctl.ID = "Edit ^" & CStr(rid)
                    ctl.ToolTip = "Edit Ticket " & CStr(rid)
                    ctl.Style.Item("color") = "blue"
                    ctl.Font.Size = 10
                    ctl.Font.Italic = True
                    AddHandler ctl.Click, AddressOf btnEdit_Click
                    HelpDesk.Rows(i + 4).Cells(6).Controls.Add(ctl)
                    HelpDesk.Rows(i + 4).BorderColor = "black"
                End If

                'correct some fields when "submit" clicked
                If Request("SaveComments" & CInt(rid)) <> "" Then 'save is clicked 
                    s = rid
                    Session("tn") = rid
                    HelpDesk.Rows(i + 4).BorderColor = "black"

                    'delete from email list 
                    addtoRowi = cleanText(Request("AddTo" & CInt(s))) & ","
                    If Request("noemails" & CStr(s)) = 1 Then
                        addtoRowi = Trim(Replace(addtoRowi, Session("logon"), ""))
                        addtoRowi = Replace(addtoRowi, ",,", ",")
                        addtoRowi = Replace(addtoRowi, ", ,", ",")

                    End If

                    addtoRowi = cleanTextFromRepeatedCommas(addtoRowi) & ","
                    addtoRowi = addtoRowi.Replace(",,", ",")
                    towhom = addtoRowi
                    'save comments!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    'n = CInt(HelpDesk.Rows(i + 4).Cells(0).InnerText)
                    'comments = AssignmentTable.Rows(i)("COMMENTS").ToString
                    Dim ret As String = String.Empty
                    If cleanText(Request("AddComments" & CInt(s))) <> "" Or Request.Files(1).FileName <> "" Or Request("noemails" & CStr(s)) = 1 Then
                        comments = Session("logon") & " (" & CDate(Now) & " ): " & cleanText(Request("AddComments" & CInt(s))) '
                        If Request.Files(1).FileName <> "" Then
                            newfileurl = NewAddress(Request.Files(1))
                            comments = comments & " | File attached: " & newfileurl & " "
                        End If
                        If Request("noemails" & CStr(s)) = 1 Then
                            comments = comments & " | Delete me from the email list for this ticket."
                        End If
                        comments = comments & " | " & AssignmentTable.Rows(i)("COMMENTS").ToString
                        'update
                        SQLq = "UPDATE OURHelpDesk SET COMMENTS='" & cleanText(comments) & "' WHERE ID=" & rid.ToString 'HelpDesk.Rows(i + 4).Cells(0).InnerText
                        ExequteSQLquery(SQLq)
                        'HelpDesk.Rows(i + 4).Cells(5).InnerHtml = FormatAsHTML(comments)
                    End If
                    HelpDesk.Rows(i + 4).Cells(5).InnerHtml = FormatAsHTML(comments)

                    'update other fields when "submit" clicked...............................

                    If Session("admin") = "admin" Or Session("admin") = "super" Then
                        SQLq = "UPDATE OURHelpDesk SET START='" & cleanText(Request("AddStart" & CInt(s))) & "'  WHERE ID=" & rid.ToString ' & HelpDesk.Rows(i + 4).Cells(0).InnerText
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURHelpDesk SET NAME='" & cleanText(Request("AddFor" & CInt(s))) & "'  WHERE ID=" & rid.ToString ' & HelpDesk.Rows(i + 4).Cells(0).InnerText
                        ret = ExequteSQLquery(SQLq)
                        towhom = addtoRowi 'cleanText(Request("AddTo" & CInt(s))) & ","
                        SQLq = "UPDATE OURHelpDesk SET ToWhom='" & towhom & "'  WHERE ID=" & rid.ToString 'HelpDesk.Rows(i + 4).Cells(0).InnerText
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURHelpDesk SET TICKET='" & cleanText(Request("AddAssign" & CInt(s))) & "'  WHERE ID=" & rid.ToString ' & HelpDesk.Rows(i + 4).Cells(0).InnerText
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURHelpDesk SET [Status]='" & cleanText(Request("AddDeadline" & CInt(s))) & "'  WHERE ID=" & rid.ToString ' & HelpDesk.Rows(i + 4).Cells(0).InnerText
                        ret = ExequteSQLquery(SQLq)
                        HelpDesk.Rows(i + 4).Cells(1).InnerHtml = FormatAsHTMLsimple(Request("AddStart" & CInt(s)))
                        HelpDesk.Rows(i + 4).Cells(2).InnerHtml = FormatAsHTMLsimple(Request("AddFor" & CInt(s)))
                        HelpDesk.Rows(i + 4).Cells(3).InnerHtml = FormatAsHTMLsimple(Request("AddAssign" & CInt(s)))
                        HelpDesk.Rows(i + 4).Cells(4).InnerHtml = FormatAsHTMLsimple(Request("AddDeadline" & CInt(s)))
                        HelpDesk.Rows(i + 4).Cells(6).InnerHtml = FormatAsHTMLsimple(addtoRowi) & "<br/><br/><input type='submit' value='Edit' name='EditComments" & CStr(s) & "'>"
                    Else
                        HelpDesk.Rows(i + 4).Cells(6).InnerHtml = towhom & "<br/><br/><input type='submit' value='Edit' name='EditComments" & CStr(s) & "'>"

                    End If
                    Session("modifyRecord") = "no"
                    'send emails
                    'EmailTable = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='InteractiveReporting')").Table
                    'If EmailTable.Rows.Count > 0 Then
                    '    For j = 0 To EmailTable.Rows.Count - 1
                    '        SendHTMLEmail("", "Status: " & Request("AddDeadline" & CInt(s)) & ". Ticket #" & rid.ToString & " :    " & Left(mainText, 60) & "...  for " & towhom, "New comment from " & Session("logon") & " at http://OUReports.com to HelpDesk Ticket #" & rid.ToString & " :    " & cleanText(comments) & "  . Ticket text: " & cleanText(Request("AddAssign" & CInt(s))) & "... for " & towhom & ".  Status:  " & Request("AddDeadline" & CInt(s)) & ".  This is auto message from Help Desk OUReports.com site. Please do not reply on this email.", EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                    '    Next
                    'End If
                    s = ""
                    m = ""
                    Response.Redirect("HelpDesk.aspx")
                End If
            Next
        End If
        Session("maketicket") = ""
        Session("attachrep") = ""
        If Not IsPostBack AndAlso Request("maketicket") = "yes" Then
            Session("maketicket") = "yes"
            btnAddTicket_Click(sender, e)
        ElseIf Not IsPostBack AndAlso Request("attachrep") = "yes" Then
            Session("attachrep") = "yes"
            Session("tn") = Request("tn").ToString
            btnEditClick()
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
            data.Description = dt.Rows(0)("Ticket").ToString
            data.Status = dt.Rows(0)("Status").ToString
            data.Deadline = dt.Rows(0)("Deadline").ToString
            data.To = dt.Rows(0)("ToWhom").ToString
            data.Comments = dt.Rows(0)("comments").ToString
            'If Not IsPostBack AndAlso Request("attachrep") = "yes" Then
            '    data.Comments = data.Comments & " | File attached: SAVEDFILES/" & Session("myfile") & " ."
            'End If
            dlgTicket.Show("Edit Ticket (User = " & Session("logon") & ")", data, Controls_TicketDlg.Mode.Edit, "Update Ticket")
        End If
    End Sub
    Protected Sub btnEditClick()
        Dim data As New Controls_TicketDlg.TicketData
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords("SELECT * FROM OURHelpDesk WHERE ID = " & Session("tn"), err).ToTable
        If err = String.Empty AndAlso dt.Rows.Count > 0 Then
            dlgTicket.UserItems = mUsers
            data.From = dt.Rows(0)("Name").ToString
            data.DateTime = dt.Rows(0)("Start").ToString
            data.ID = dt.Rows(0)("ID").ToString
            data.Version = dt.Rows(0)("Version").ToString
            data.Description = dt.Rows(0)("Ticket").ToString
            data.Status = dt.Rows(0)("Status").ToString
            data.To = dt.Rows(0)("ToWhom").ToString
            data.Comments = dt.Rows(0)("comments").ToString
            'If Not IsPostBack AndAlso Request("attachrep") = "yes" Then
            '    data.Comments = data.Comments & " | File attached: SAVEDFILES/" & Session("myfile") & " ."
            'End If
            If Not IsPostBack AndAlso Request("attachrep") = "yes" Then
                If Session("myfile").ToString.Trim <> "" Then
                    ' TicketData.Comments = "Report file attached: <a href=""/SAVEDFILES/" & Session("myfile") & """ >" & Session("myfile") & "</a> | "
                    data.Description = data.Description & " | Report " & Session("REPTITLE") & "(" & Session("REPORTID") & ") attached automatically"
                    data.Comments = data.Comments & " | File attached: SAVEDFILES/" & Session("myfile") & " ."
                Else
                    data.Description = data.Description & " | Report " & Session("REPTITLE") & "(" & Session("REPORTID") & ") is empty."
                End If
            End If
            dlgTicket.Show("Edit Ticket (User = " & Session("logon") & ")", data, Controls_TicketDlg.Mode.Edit, "Update Ticket")
        End If
    End Sub
    Protected Sub ButtonAddAssignment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddAssignment.Click
        Dim tdata = TextDate.Text
        Dim j As Integer
        Dim SQLq As String
        Dim ret As String
        If Trim(LabelWho.Text) = "" Then LabelWho.Text = DropDownListWho.SelectedValue
        If Trim(LabelWho.Text) = "" Then LabelWho.Text = Session("logon")
        If Trim(LabelWhom.Text) = "" Then LabelWhom.Text = DropDownListWhom.SelectedValue
        LabelWhom.Text = cleanTextFromRepeatedCommas(LabelWhom.Text) & ","
        SQLq = "INSERT INTO OURHelpDesk (Start,Name,Ticket,[Status],COMMENTS, ToWhom,Version,Prop1) VALUES('" & TextDate.Text & "','" & LabelWho.Text & "','" & cleanText(TextTopics.Text) & "','" & cleanText(TextDecisions.Text) & "','" & cleanText(TextComments.Text) & "','" & LabelWhom.Text & "','" & Session("Unit").ToString & "')"
        ret = ExequteSQLquery(SQLq)
        ret = ret.Replace("Query executed fine.", "").Trim
        If ret = "" Then
            'send emails
            Dim EmailTable As DataTable
            'send emails
            EmailTable = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='InteractiveReporting')").Table
            'If EmailTable.Rows.Count > 0 Then
            '    For j = 0 To EmailTable.Rows.Count - 1
            '        SendHTMLEmail("", "Status: " & cleanText(TextDecisions.Text) & ". New Ticket: " & Left(cleanText(TextTopics.Text), 90) & ". ", "New ticket in Help Desk on http://OUReports.com  from " & DropDownListWho.Text & ", Status: " & cleanText(TextDecisions.Text) & ":   " & cleanText(TextTopics.Text) & ". Comments:  " & cleanText(TextComments.Text) & " - This is auto message from Help Desk OUReports.com site.  Please do not reply on this email.", EmailTable.Rows(j)("Email"), Session("SupportEmail"))
            '    Next
            'End If
        End If
        Response.Redirect("HelpDesk.aspx")
    End Sub

    Protected Sub ButtonAddWho_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddWho.Click
        If Request(DropDownListWho.UniqueID) = "all" Then
            'add everybody
            Dim everybody As DataTable
            Dim i As Integer
            everybody = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='InteractiveReporting')").Table
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
            everybody = mRecords("SELECT * FROM OURPERMITS WHERE (Access='DEV') AND (Application='InteractiveReporting')").Table
            For i = 0 To everybody.Rows.Count - 1
                LabelWhom.Text = LabelWhom.Text & ", " & Trim(everybody.Rows(i)("NETID"))
            Next
        Else
            LabelWhom.Text = LabelWhom.Text & ", " & Trim(Request(DropDownListWhom.UniqueID))
        End If
    End Sub
    Function NewAddress(ByVal FileNew As HttpPostedFile) As String
        'file upload
        If Session("admin") <> "user" And Session("admin") <> "admin" And Session("admin") <> "super" Then
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
                dir = Request.PhysicalApplicationPath & "SAVEDFILES\"  'files directory 
                FileNew.SaveAs(dir & filenamefix)
                'MessageLabel.Text = FileNew.FileName & " uploaded" & _
                '  "<br>content type: " & contentType & _
                '  "<br>content length: " & contentLength.ToString()
                Session("filename") = filename
                NewAddress = Session("applpath") & "SAVEDFILES/" & filenamefix
            Catch exc As Exception
                'MessageLabel.Text = "Failed uploading file"
            End Try
        Else
            'MessageLabel.Text = "Select your local file first..."
        End If

    End Function

    Protected Sub ButtonAttach_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAttach.Click
        If Session("admin") <> "user" And Session("admin") <> "admin" And Session("admin") <> "super" Then
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
                dir = Request.PhysicalApplicationPath & "Temp\"  'files directory 
                postedFile.SaveAs(dir & filenamefix)
                'MessageLabel.Text = postedFile.Filename & " uploaded" & _
                '  "<br>content type: " & contentType & _
                '  "<br>content length: " & contentLength.ToString()
                Session("filename") = filename
                TextComments.Text = TextComments.Text & " | File attached: " & "Temp/" & filenamefix & " ."
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
        If Not IsPostBack AndAlso Request("maketicket") = "yes" Then
            If Session("myfile").ToString.Trim <> "" Then
                ' TicketData.Comments = "Report file attached: <a href=""/SAVEDFILES/" & Session("myfile") & """ >" & Session("myfile") & "</a> | "
                TicketData.Description = "Report " & Session("REPTITLE") & "(" & Session("REPORTID") & ") attached automatically"
                TicketData.Comments = " | File attached: SAVEDFILES/" & Session("myfile") & " ."
            Else
                TicketData.Description = "Report " & Session("REPTITLE") & "(" & Session("REPORTID") & ") is empty."
            End If
        End If
        dlgTicket.Show("Add Ticket (User = " & Session("logon") & ")", TicketData, Controls_TicketDlg.Mode.Add, "Add Ticket")
    End Sub
    Private Sub SaveTicket(data As Controls_TicketDlg.TicketData, IsEdit As Boolean)
        Dim SQLq As String = "INSERT INTO OURHelpDesk "
        Dim tick As String = "New ticket # " & data.ID & ": "
        Dim tick2 As String = "New ticket # " & data.ID
        If IsEdit Then
            SQLq = "UPDATE OURHelpDesk "
            tick = "Ticket #" & data.ID & " edited: "
            tick2 = "Ticket #" & data.ID
            SQLq &= "SET Start = '" & data.DateTime & "',"
            SQLq &= "Name = '" & data.From & "',"
            SQLq &= "Version = '" & data.Version & "',"
            SQLq &= "Ticket = '" & cleanTextLight(data.Description) & "',"
            SQLq &= "[Status] = '" & cleanText(data.Status) & "',"
            SQLq &= "Deadline = '" & data.Deadline & "',"
            SQLq &= "Prop1 = '" & Session("Unit") & "',"
            If data.Comments <> String.Empty Then
                SQLq &= "COMMENTS = '" & cleanTextLight(data.Comments) & "',"
            End If
            SQLq &= "ToWhom = '" & data.To & "'"
            SQLq &= " WHERE ID = " & data.ID
        Else
            Dim sFields As String = "Start,Name,Version,Ticket,[Status],Deadline,Prop1"
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
            Dim sSubject As String = "Status: " & data.Status & ". " & tick & Left(sDesc, 90)
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
            Dim applweb As String = ConfigurationManager.AppSettings("webour").ToString
            For i As Integer = 1 To Pieces(data.To, ",")
                sTo = Piece(data.To, ",", i)
                sSQl = sStart & sTo & "')  AND (Application='InteractiveReporting') "
                dt = mRecords(sSQl, err).Table
                If err = String.Empty AndAlso dt.Rows.Count > 0 Then
                    Dim EmailAddress As String = dt.Rows(0)("Email").ToString
                    If EmailAddress <> String.Empty Then
                        sBody = tick & vbCrLf
                        sBody &= "Initiated by: " & data.From & " " & vbCrLf & " "
                        sBody &= "Status: " & data.Status & " " & vbCrLf & " "
                        sBody &= "Version: " & data.Version & " " & vbCrLf & " "
                        sBody &= "Description: " & cleanText(data.Description) & " " & vbCrLf & " "
                        sBody &= "Comments:" & " " & vbCrLf & " "
                        sBody &= sComments & " " & vbCrLf & " " & "--------------------------" & vbCrLf
                        sBody &= tick2 & " is in Help Desk on " & applweb & "default.aspx?tn=" & data.ID & " " & vbCrLf & " " & vbCrLf
                        sBody &= "This is auto message from Help Desk on " & applweb & " site.  Please Do Not reply to this email."
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
            'only for OUReports !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
            Dim EmailTable As DataTable
            EmailTable = mRecords("SELECT * FROM OURPERMITS WHERE (RoleApp = 'super') AND (Application='InteractiveReporting')").Table
            If EmailTable.Rows.Count > 0 Then
                For j = 0 To EmailTable.Rows.Count - 1
                    'SendHTMLEmail("", "Status: " & cleanText(data.Status) & ". " & tick & Left(data.Description, 90) & ". ", tick2 & " is in Help Desk on http://OUReports.com  from " & data.From & ", Status: " & cleanText(data.Status) & ":   " & cleanText(data.Description) & ". Comments:  " & cleanText(data.Comments) & " - This is auto message from Help Desk OUReports.com site.  Please do not reply on this email.", EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                    sSubject = "User = " & Session("Logon").ToString & ", User db = " & Session("dbname").ToString & ", " & sSubject
                    remail = SendHTMLEmail(data.Attachment, sSubject, sBody, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                Next
            End If
        ElseIf ret <> String.Empty Then
            MessageBox.Show(ret, "Save Ticket", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If
        If Request("shownotdone") IsNot Nothing AndAlso Request("shownotdone").ToString = "1" Then
            Response.Redirect("~/HelpDesk.aspx?shownotdone=1")
        Else
            Response.Redirect("HelpDesk.aspx")
        End If
    End Sub
    Private Sub dlgTicket_TicketDialogResulted(sender As Object, e As Controls_TicketDlg.TicketDlgEventArgs) Handles dlgTicket.TicketDialogResulted
        If e.EntryMode = Controls_TicketDlg.Mode.Add Then
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
            Response.Redirect("~/HelpDesk.aspx?shownotdone=0")
        Else
            Response.Redirect("~/HelpDesk.aspx?shownotdone=1")
        End If
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        'Dim srch As String = FirstLetters.Text.Trim
        'Session("FirstLetters") = srch
        'Response.Redirect("~/HelpDesk.aspx?FirstLetters=" & srch)
    End Sub

End Class



