Imports System.Drawing
Imports System.Data
Partial Class ContactUs
    Inherits System.Web.UI.Page

    Private Sub ContactUs_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("SupportEmail") = ConfigurationManager.AppSettings("supportemail").ToString
        Session("WEBOUR") = ConfigurationManager.AppSettings("webour").ToString
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
    End Sub
    Private Sub ClearEmail()
        txtName.Text = ""
        txtEmail.Text = ""
        txtSubject.Text = ""
        txtBody.Text = ""
        ddTopics.BorderColor = Color.Black
        ddTopics.BackColor = Color.White
        ddTopics.Text = ""
    End Sub
    Function CheckEmailFrequency(ipaddr As String) As Boolean
        Dim bRet As Boolean = True
        Dim sCheck As String = "email at " & ipaddr
        Dim today As String = Format(Date.Today, "yyyy-MM-dd")
        Dim TodayNormal As String = Format(Date.Today, "MM/dd/yyyy")

        If ipaddr <> String.Empty AndAlso ipaddr <> "::1" Then
            'Check if already marked as spam
            Dim dt As DataTable = mRecords("SELECT DISTINCT Logon FROM ouraccesslog WHERE [Action] LIKE 'Got email%'  AND Logon ='" & sCheck.Trim & "' AND Comments = 'spam' ORDER BY ID DESC").ToTable
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Dim sql As String = "SELECT Logon FROM ouraccesslog WHERE [Action] LIKE 'Got email%'  AND  Logon ='" & sCheck.Trim & "' AND EventDate LIKE '" & today & "%' ORDER BY ID DESC"
                'if not already spam and more than 4 emails have  been sent already, email should be blocked.
                dt = mRecords(sql).ToTable
                If dt IsNot Nothing AndAlso dt.Rows.Count >= 5 Then
                    WriteToAccessLog("Contact Page", "Contact email from " & ipaddr & " has already been sent " & dt.Rows.Count.ToString & " times on " & TodayNormal & ". It will be marked as spam.", 25)
                    bRet = False
                End If
            End If
        End If
        Return bRet
    End Function
    Sub BlockContactEmails(ipaddr As String)

        If ipaddr <> String.Empty AndAlso ipaddr <> "::1" Then
            Dim sCheck As String = "email at " & ipaddr
            Dim sql As String = "SELECT * FROM ouraccesslog WHERE [Action] LIKE 'Got email%'  AND  Logon ='" & sCheck.Trim & "'"
            Dim dt As DataTable = mRecords(sql).ToTable

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    Dim dr As DataRow = dt.Rows(i)
                    Dim id As String = dr("ID")
                    ExequteSQLquery("UPDATE ouraccesslog SET Comments='spam' WHERE ID=" & id)
                Next
            End If
            'If Pieces(ipaddr, ".") > 2 Then
            Dim filter As String = ipaddr
            Dim action As String = "equals:" & filter

            'Dim filter As String = Piece(ipaddr, ".", 1, 2) & "."
            'Dim action As String = "startswith:" & filter

            sql = "Select *  FROM ouraccesslog Where [Action] = '" & Action & "' AND Comments = 'ipfilter'"
                'if the filter doesn't already exist, create it.
                If Not HasRecords(sql) Then
                    WriteToAccessLogComment("filter", action, 15, "ipfilter")
                End If

            'End If
        End If

    End Sub
    Function IsBlocked(ipaddr As String) As Boolean
        Dim bRet As Boolean = False
        Dim sql As String = "SELECT [Action] FROM ouraccesslog WHERE Comments = 'ipfilter'"
        Dim action As String
        Dim condition As String
        Dim value As String

        If ipaddr <> String.Empty AndAlso ipaddr <> "::1" Then
            Dim dt As DataTable = mRecords(sql).ToTable

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    action = dt.Rows(i)("Action")
                    If action.Contains(":") Then
                        condition = Piece(action, ":", 1)
                        value = Piece(action, ":", 2)
                        If condition = "equals" Then
                            If ipaddr = value Then
                                Return True
                            End If
                        ElseIf condition = "startswith" Then
                            If ipaddr.StartsWith(value) Then
                                Return True
                            End If
                        End If
                    End If
                Next
            End If
        End If
        Return bRet
    End Function
    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs) Handles btnSendEmail.Click
        Dim ipaddr As String = GetIPAddress()
        'Dim ipaddr As String = "67.1.121.132"

        'Check for proper entries

        Dim sEmailAddress As String = cleanText(txtEmail.Text)
        If sEmailAddress = "" OrElse sEmailAddress.Trim <> txtEmail.Text.Trim Then
            txtEmail.Text = "Enter proper email address!"
            txtEmail.ForeColor = Color.Red
            txtEmail.Focus()
            Exit Sub
        End If
        Dim sName As String = cleanText(txtName.Text)
        If sName = "" OrElse sName.Trim <> txtName.Text.Trim Then
            txtName.Text = "Enter your name please..."
            txtName.ForeColor = Color.Red
            txtName.Focus()
            Exit Sub
        End If
        Dim sSubject As String = cleanText(txtSubject.Text)
        If sSubject.Trim <> txtSubject.Text.Trim Then
            txtSubject.Text = "Enter email subject please..."
            txtSubject.ForeColor = Color.Red
            txtSubject.Focus()
            Exit Sub
        End If
        Dim sBody As String = cleanText(txtBody.Text, True)
        If sBody.Trim = String.Empty OrElse sBody.Replace(vbCrLf, "").Trim = String.Empty Then
            txtBody.Text = "Enter email message please..."
            txtBody.ForeColor = Color.Red
            txtBody.Focus()
            Exit Sub
        End If
        If sSubject.Trim = "" AndAlso sBody.Trim = "" Then
            txtSubject.Text = "Enter email subject and/or email message..."
            txtSubject.ForeColor = Color.Red
            txtSubject.Focus()
            Exit Sub
        End If
        If ddTopics.Text.Trim = "" Then
            ddTopics.BorderColor = Color.Red
            ddTopics.BackColor = Color.OrangeRed
            ddTopics.Focus()
            Exit Sub
        End If
        'Send confirmation email
        Label1.Text = "We got your request and will be in contact shortly."
        'Label1.Text = " Confirmation " & SendHTMLEmail("", ddTopics.Text.Trim & ": " & sSubject, "Confirmation: " & sBody, sEmailAddress, Session("SupportEmail"))
        'WriteToAccessLog("confirmation email", "Sent confirmation email to " & sEmailAddress & " from " & Session("SupportEmail") & ", Email subject: " & ddTopics.Text.Trim & ": " & sSubject & ", Email body: " & sBody, 100)

        'check for spam
        'If ipaddr = "37.139.53.81" OrElse ipaddr.StartsWith("185.107.") OrElse ipaddr.StartsWith("77.247") Then
        If IsBlocked(ipaddr) Then
            ClearEmail()
            WriteToAccessLog("Contact Page", "Contact email from IP Address " & ipaddr & " has been blocked as spam.", 25)
            Exit Sub
        End If
        Dim bOK As Boolean = CheckEmailFrequency(ipaddr)
        'Dim bOK As Boolean = CheckEmailFrequency("67.1.121.132")

        If Not bOK Then
            BlockContactEmails(ipaddr)
            'BlockContactEmails("67.1.121.132")
            ClearEmail()
            Exit Sub
        End If
        Dim sEmailSubject As String = "Received Contact Page Email From " & sEmailAddress
        Dim sEmailBody As String = "Contact Page Email" & vbCrLf
        sEmailBody &= "--------------------------------------" & vbCrLf
        sEmailBody &= "From: " & sEmailAddress & vbCrLf
        sEmailBody &= "From IP Address: " & ipaddr & vbCrLf
        sEmailBody &= "Email Subject: " & ddTopics.Text.Trim & ": " & sSubject & vbCrLf
        sEmailBody &= "Email Message: " & sBody & vbCrLf
        sEmailBody &= "---------------------------------------" & vbCrLf & vbCrLf
        sEmailBody &= "Block further emails from " & ipaddr & " here: " & Session("WEBOUR").ToString.Trim & "default.aspx?spam=yes&ip=" & ipaddr & vbCrLf
        Dim dsp As DataView = mRecords("SELECT DISTINCT Logon FROM ouraccesslog WHERE [Action] LIKE 'Got email%'  AND Logon LIKE 'email at %' AND Comments='spam' ORDER BY ID DESC")
        Dim i As Integer
        If Not dsp Is Nothing AndAlso Not dsp.Table Is Nothing AndAlso dsp.Table.Rows.Count > 0 Then
            For i = 0 To dsp.Table.Rows.Count - 1
                If dsp.Table.Rows(i)("Logon").ToString.Replace("email at", "").Trim = ipaddr Then
                    ClearEmail()
                    Exit Sub
                End If
            Next
        End If

        'WriteToAccessLog("email", "Got email from " & sEmailAddress & " to " & Session("SupportEmail") & ", Email subject: " & ddTopics.Text.Trim & ": " & sSubject & ", Email body: " & sBody & Label1.Text, 100)
        WriteToAccessLog("email", "Got email from " & sEmailAddress & " to " & Session("SupportEmail") & ", Email subject: " & ddTopics.Text.Trim & ": " & sSubject & ", Email body: " & sBody, 100)

        'send email to super users if ip is not blocked
        Dim EmailTable As DataTable
        Dim ret As String = String.Empty
        Dim j As Integer
        EmailTable = mRecords("SELECT  * FROM OURPERMITS WHERE APPLICATION='InteractiveReporting' AND RoleApp='super'").Table
        If EmailTable.Rows.Count > 0 Then
            For j = 0 To EmailTable.Rows.Count - 1
                ret = SendHTMLEmail("", sEmailSubject, sEmailBody, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
                'ret = ret & " " & SendHTMLEmail("", "Got email from " & sEmailAddress, "Contact Us page sent email from " & sEmailAddress & ", IP - " & ipaddr & " to " & Session("SupportEmail") & ", Email Subject: " & ddTopics.Text.Trim & ": " & sSubject & ", Email Body: " & sBody & Label1.Text, EmailTable.Rows(j)("Email"), Session("SupportEmail"))
            Next
        End If
        ClearEmail()
    End Sub

    Private Sub chkme_CheckedChanged(sender As Object, e As EventArgs) Handles chkme.CheckedChanged
        If chkme.Checked Then
            btnSendEmail.Visible = True
        Else
            btnSendEmail.Visible = False
        End If
    End Sub

    Private Sub ContactUs_Load(sender As Object, e As EventArgs) Handles Me.Load
        If chkme.Checked Then
            btnSendEmail.Visible = True
        Else
            btnSendEmail.Visible = False
        End If
    End Sub
End Class
