Imports System.Drawing
Partial Class index
    Inherits System.Web.UI.Page

    Private Sub index_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("WEBOUR") = ConfigurationManager.AppSettings("weboureports").ToString
        Session("WEBHELPDESK") = ConfigurationManager.AppSettings("webhelpdesk").ToString
        Session("SupportEmail") = ConfigurationManager.AppSettings("supportemail").ToString
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        Session("org") = ""
        Session("UnitIndx") = ""
        Head1.Title = Session("PAGETTL")
        If Not Request("ag") Is Nothing AndAlso Request("ag").ToString.Trim <> "" Then
            Session("agent") = Request("ag")
            WriteToAccessLog(Session("agent").ToString, "Link with agent #" & Session("agent").ToString & "clicked", 22)
        Else
            Session("agent") = ""
        End If
        If Not Request("pass") Is Nothing AndAlso Not Request("logon") Is Nothing Then
            Dim pw As String = Request("pass").ToString
            Dim lg As String = Request("logon").ToString
            If lg = "demo" AndAlso pw = "demo" Then
                Response.Redirect(Session("WEBOUR").ToString & "Default.aspx?logon=demo&pass=demo")
            End If
        End If
        Response.Redirect("index1.aspx")
    End Sub

    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs) Handles btnSendEmail.Click
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
        Dim sBody As String = cleanText(txtBody.Text)
        If sBody.Trim <> txtBody.Text.Trim Then
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

        Label1.Text = " Confirmation: " & SendHTMLEmail("", sSubject, sBody, sEmailAddress, Session("SupportEmail"))
        WriteToAccessLog("email", "Got email from " & sEmailAddress & " to " & Session("SupportEmail") & ", subject: " & sSubject & ", body: " & sBody & Label1.Text, 100)
        txtName.Text = ""
        txtEmail.Text = ""
        txtSubject.Text = ""
        txtBody.Text = ""
    End Sub
    'Protected Sub btnRegistration_Click(sender As Object, e As EventArgs) Handles btnRegistration.Click
    '    Response.Redirect(Session("WEBOUR").ToString & "Default.aspx")
    'End Sub
    Protected Sub btnDemo_Click(sender As Object, e As EventArgs) Handles btnDemo.Click
        Response.Redirect(Session("WEBOUR").ToString & "Default.aspx?logon=demo&pass=demo")
    End Sub
    Private Sub lnkPDF_Click(sender As Object, e As EventArgs) Handles lnkPDF.Click
        Response.Redirect(Session("WEBOUR") & "DataAIHelp.aspx")
    End Sub
    Private Sub btnSignUp_Click(sender As Object, e As EventArgs) Handles btnSignUp.Click
        Response.Redirect(Session("WEBOUR").ToString & "Default.aspx")
    End Sub

    Private Sub btnRegisterVendor_Click(sender As Object, e As EventArgs) Handles btnRegisterVendor.Click
        Response.Redirect(Session("WEBHELPDESK").ToString & "UnitRegistration.aspx?org=vendor")
    End Sub

    Private Sub btnRegisterCompany_Click(sender As Object, e As EventArgs) Handles btnRegisterCompany.Click
        Response.Redirect(Session("WEBHELPDESK").ToString & "UnitRegistration.aspx?org=company")
    End Sub

    Private Sub btnRegisterAgent_Click(sender As Object, e As EventArgs) Handles btnRegisterAgent.Click
        Response.Redirect(Session("WEBHELPDESK").ToString & "OUReportsAgents.aspx")
    End Sub

    Private Sub btnRegisteredAgentLogin_Click(sender As Object, e As EventArgs) Handles btnRegisteredAgentLogin.Click
        Response.Redirect(Session("WEBHELPDESK").ToString & "OUReportsAgents.aspx?login=yes")
    End Sub
    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Response.Redirect("index1.aspx")
    End Sub
End Class
