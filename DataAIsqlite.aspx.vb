
Imports System.Windows.Forms
Imports Org.BouncyCastle.Asn1.X509
Imports PdfSharp.Pdf.Content.Objects

Partial Class DataAIsqlite
    Inherits System.Web.UI.Page
    Private Sub DataAIsqlite_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("txtConn") = ""
        Session("txtURI") = ""
        'If connected from outside then following parameters are expected:
        'User credentials:
        '------------------
        'txtEmail - Logon or Email or user ID
        'txtURI - URL to the web address with user data (csv, json, xml file) to analyze

        'Optional OpenAI account credentials:
        '-----------------------------
        'txtOpenAIkey - OpenAI key
        'txtOpenAIorgcode - OpenAI organization code
        'txtOpenAIurl - OpenAI Base URL
        'txtOpenAImodel -  OpenAI model
        'txtOpenAImaxtokens - OpenAI maximum tokens limit (128,000 or 200,000 oe esle)


        'URL with QueryString should be as:
        'https://oureports.net/DataAIsqlite/txtEmail=...&txtURI=...
        'Optional add to QueryString to use AI in analytics:
        '&DataAIsqlite.aspx?txtOpenAIkey=...&OpenAIkey=...&txtOpenAIorgcode=...&OpenAIOrganization=...&txtOpenAIurl=...&txtOpenAImodel=...&txtOpenAImaxtokens=...


        If txtOpenAIkey.Text = "" AndAlso Request("txtOpenAIkey") IsNot Nothing AndAlso Request("txtOpenAIkey").ToString.Trim <> "" Then
            txtOpenAIkey.Text = Request("txtOpenAIkey").ToString.Trim
            Session("OpenAIkey") = txtOpenAIkey.Text
        End If
        If txtOpenAIkey.Text = "" AndAlso Session("OpenAIkey") IsNot Nothing AndAlso Session("OpenAIkey").ToString.Trim <> "" Then
            txtOpenAIkey.Text = Session("OpenAIkey").ToString.Trim
        End If

        If txtOpenAIorgcode.Text = "" AndAlso Request("txtOpenAIorgcode") IsNot Nothing AndAlso Request("txtOpenAIorgcode").ToString.Trim <> "" Then
            txtOpenAIorgcode.Text = Request("txtOpenAIorgcode").ToString.Trim
            Session("OpenAIOrganization") = Request("txtOpenAIorgcode")
        End If
        If txtOpenAIorgcode.Text = "" AndAlso Session("OpenAIOrganization") IsNot Nothing AndAlso Session("OpenAIOrganization").ToString.Trim <> "" Then
            txtOpenAIorgcode.Text = Session("OpenAIOrganization").ToString.Trim
        End If

        If Request.QueryString("txtOpenAIurl") IsNot Nothing AndAlso Request.QueryString("txtOpenAIurl").ToString.Trim <> "" Then
            txtOpenAIurl.Text = Request.QueryString("txtOpenAIurl").ToString.Trim
        ElseIf Session("OpenAIurl") IsNot Nothing AndAlso Session("OpenAIurl").ToString.Trim <> "" Then
            txtOpenAIurl.Text = Session("OpenAIurl").ToString.Trim
        End If
        If txtOpenAIurl.Text.Trim = "" Then 'default
            txtOpenAIurl.Text = "https://api.openai.com/v1/chat/completions"
        End If
        Session("OpenAIurl") = txtOpenAIurl.Text

        If Request("txtOpenAImodel") IsNot Nothing AndAlso Request("txtOpenAImodel").ToString.Trim <> "" Then
            txtOpenAImodel.Text = Request("txtOpenAImodel").ToString.Trim
        ElseIf Session("OpenAImodel") IsNot Nothing AndAlso Session("OpenAImodel").ToString.Trim <> "" Then
            txtOpenAImodel.Text = Session("OpenAImodel").ToString.Trim
        End If
        If txtOpenAImodel.Text.Trim = "" Then 'default
            txtOpenAImodel.Text = "gpt-4o-mini"
        End If
        Session("OpenAImodel") = txtOpenAImodel.Text

        If Request("txtOpenAImaxtokens") IsNot Nothing AndAlso Request("txtOpenAImaxtokens").ToString.Trim <> "" AndAlso IsNumeric(Request("txtOpenAImaxtokens").ToString.Trim) Then
            txtOpenAImaxtokens.Text = CInt(Request("txtOpenAImaxtokens").ToString.Trim)
        ElseIf Session("maxTokens") IsNot Nothing AndAlso Session("maxTokens").ToString.Trim <> "" AndAlso IsNumeric(Session("maxTokens").ToString.Trim) Then
            txtOpenAImaxtokens.Text = CInt(Session("maxTokens").ToString.Trim)
        End If
        If txtOpenAImaxtokens.Text.Trim = "" Then 'default
            txtOpenAImaxtokens.Text = "128000"
        End If
        Session("maxTokens") = txtOpenAImaxtokens.Text

        If Request("txtEmail") IsNot Nothing AndAlso Request("txtEmail").ToString.Trim <> "" Then
            txtEmail.Text = cleanText(Request("txtEmail").ToString.Trim)
            txtEmail.Text = Regex.Replace(txtEmail.Text, "[^a-zA-Z0-9]", "")
            Session("Email") = txtEmail.Text
        End If
        If txtEmail.Text = "" AndAlso Session("Email") IsNot Nothing AndAlso Session("Email").ToString.Trim <> "" Then
            txtEmail.Text = Session("Email").ToString.Trim
        End If
        If txtEmail.Text = "" Then
            txtEmail.Text = "time" & Now.ToString.Replace("/", "").Replace(":", "").Replace(" ", "")
            Session("Email") = txtEmail.Text
        End If

        If Request("frm") IsNot Nothing AndAlso Request("frm").ToString.Trim = "chatai" Then
            txtEmail.Enabled = False

        Else
            txtEmail.Enabled = True
            If Request.QueryString("txtURI") IsNot Nothing AndAlso Request.QueryString("txtURI").ToString.Trim <> "" Then
                Session("txtURI") = Request.QueryString("txtURI").ToString.Trim
                If Session("txtURI").ToString.Trim <> "" AndAlso Session("txtURI").ToString.Trim <> "https://" AndAlso Session("txtURI").ToString.Trim <> "http://" Then    'web file
                    'add payment
                    Response.Redirect("DataAIaddons.aspx?frm=DataAIsqlite")
                End If
            ElseIf Request.QueryString("txtConn") IsNot Nothing AndAlso Not Request.QueryString("txtConn").ToString.Contains("youruser") AndAlso Not Request.QueryString("txtConn").ToString.Contains("yourfilepath") Then
                Session("txtConn") = Request.QueryString("txtConn").ToString.Trim.Replace("""", "")
                'If Session("txtConn").ToString.Trim <> "" AndAlso Not Session("txtConn").ToString.Contains("") Then    'connection string
                'add payment
                Response.Redirect("DataAIaddons.aspx?txtConn=" & Session("txtConn").ToString.Trim)
                'End If
            End If

        End If



    End Sub

    Private Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        Session("OpenAIkey") = txtOpenAIkey.Text
        Session("OpenAIOrganization") = txtOpenAIorgcode.Text
        Session("OpenAIurl") = txtOpenAIurl.Text
        Session("OpenAImodel") = txtOpenAImodel.Text
        If IsNumeric(txtOpenAImaxtokens.Text) Then
            Session("maxTokens") = CInt(txtOpenAImaxtokens.Text)
        Else
            If txtOpenAIkey.Text.Trim <> "" Then
                MessageBox.Show("MaxTokens should be integer!", "MaxTokens", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
                Exit Sub
            End If
        End If
        Session("Email") = txtEmail.Text
        If Request("frm") IsNot Nothing AndAlso Request("frm").ToString.Trim = "chatai" Then
            Response.Redirect("ChatAI.aspx")
        Else
            Response.Redirect("DataAIaddons.aspx?frm=DataAIsqlite")
        End If
    End Sub



End Class
