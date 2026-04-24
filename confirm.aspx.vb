Imports System.Data
Imports System.Data.OleDb
Partial Class confirm
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Session("logon") Is Nothing AndAlso Session("logon") = "demo" Then
            Response.Redirect("Default.aspx?msg=DemoUserNotChanged")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Not IsPostBack Then
            Dim bLogOn As Boolean = False
            Dim userconnstr As String = String.Empty
            Dim userconnprv As String = String.Empty
            If Not System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection") Is Nothing Then
                'sql connection to user database from webconfig 
                userconnstr = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ToString
                userconnprv = System.Configuration.ConfigurationManager.ConnectionStrings.Item("userSQLconnection").ProviderName.ToString
            Else
                userconnstr = cleanText(Request("connstr"))
                userconnprv = cleanText(Request("connprv"))
            End If
            Session("UserConnString") = userconnstr
            Session("UserConnProvider") = userconnprv
            If userconnstr.Trim <> "" Then
                dropdownDatabases.SelectedValue = userconnprv
                dropdownDatabases.Visible = False
                dropdownDatabases.Enabled = False
                txtConnStr.Text = userconnstr
                txtConnStr.Visible = False
                'ConnStr.Enabled = False
                lblConnection.Text = ""
                lblPassWord.Text = ""
                Regist.Rows(6).Visible = False
                Regist.Rows(7).Visible = False
            Else
                dropdownDatabases.Visible = True
                dropdownDatabases.Enabled = True
                txtConnStr.Visible = True
                'txtConnStr.Enabled = True
            End If

            If Not Session("logon") Is Nothing AndAlso cleanText(Session("logon")) <> String.Empty Then
                txtLogin.Text = Session("logon")
                txtLogin.ReadOnly = True
                txtLogin.Enabled = False
                bLogOn = True
            Else
                Exit Sub
            End If
            If Session("ChangePassword") = True Then
                WriteToAccessLog(Session("logon"), "Request to change password.", 0)
            Else
                Response.Redirect("default.aspx")
            End If
            If Not Request("unit") Is Nothing AndAlso cleanText(Request("unit")) <> String.Empty Then
                txtUnit.Text = cleanText(Request("unit"))
            End If
            If Not Request("email") Is Nothing AndAlso cleanText(Request("email")) <> String.Empty Then
                txtEmail.Text = cleanText(Request("email"))
            End If
            If cleanText(dropdownDatabases.SelectedValue) <> String.Empty AndAlso dropdownDatabases.Visible = True Then
                'TODO check if this is needed?
                Select Case dropdownDatabases.SelectedValue
                    Case "InterSystems.Data.CacheClient"
                        dropdownDatabases.SelectedIndex = 1
                        lblConnection.Text = "Server = yourserver; Port = 1972; Namespace = yournamespace; User ID = youruser; Password = yourpassword"
                    Case "MySql.Data.MySqlClient"
                        dropdownDatabases.SelectedIndex = 2
                End Select
            End If
            If cleanText(txtConnStr.Text) <> String.Empty Then
                txtConnStr.Text = cleanText(txtConnStr.Text)
            End If
            If bLogOn Then
                txtCurrent.Focus()
            Else
                txtLogin.Focus()
            End If

            txtConnStr.Enabled = False
            txtUnit.Enabled = False
            txtUnit.Visible = False
            dropdownDatabases.Enabled = False
            'lblConnection.Text = ""
            Dim webinstall As String = ConfigurationManager.AppSettings("webinstall").ToString
            Dim dbinstall As String = ConfigurationManager.AppSettings("dbinstall").ToString
            If webinstall = "OURweb" AndAlso dbinstall = "OURdb" Then

            Else

            End If
        End If

    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim ret As String = String.Empty

        'check text boxes
        If cleanText(txtCurrent.Text.Trim) <> txtCurrent.Text.Trim Then
            ret = "Illegal character found in Password."
            MessageBox.Show(ret, "Change Password", "LogonError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        ElseIf cleanText(txtCurrent.Text.Trim) = "" Then
            ret = "Password cannot be empty."
            MessageBox.Show(ret, "Change Password", "LogonError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        End If
        If cleanText(txtNew.Text.Trim) <> txtNew.Text.Trim Then
            ret = "Illegal character found in Password."
            MessageBox.Show(ret, "Change Password", "PasswordError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        ElseIf cleanText(txtNew.Text.Trim) = "" Then
            ret = "Password cannot be empty."
            MessageBox.Show(ret, "Change Password", "PasswordError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        End If
        If cleanText(txtRepeat.Text.Trim) <> txtRepeat.Text.Trim Then
            ret = "Illegal character found in Password."
            MessageBox.Show(ret, "Change Password", "PasswordError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        ElseIf cleanText(txtRepeat.Text.Trim) = "" Then
            ret = "Password cannot be empty."
            MessageBox.Show(ret, "Change Password", "PasswordError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        End If
        If cleanText(txtEmail.Text.Trim) <> txtEmail.Text.Trim Then
            ret = "Illegal character found in Email."
            MessageBox.Show(ret, "Change Password", "EmailError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        ElseIf cleanText(txtEmail.Text.Trim) = "" Then
            ret = "Email cannot be empty."
            MessageBox.Show(ret, "Change Password", "EmailError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        End If
        If cleanText(txtUnit.Text.Trim) <> txtUnit.Text.Trim Then
            ret = "Illegal character found in Unit."
            MessageBox.Show(ret, "Change Password", "CompanyError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        ElseIf cleanText(txtUnit.Text.Trim) = "" Then
            ret = "Organization cannot be empty."
            MessageBox.Show(ret, "Change Password", "CompanyError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        End If
        If cleanText(txtConnStr.Text.Trim) <> txtConnStr.Text.Trim Then
            ret = "Illegal character found in Connection String."
            MessageBox.Show(ret, "Change Password", "ConnectionString", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        ElseIf cleanText(txtConnStr.Text.Trim) = "" Then
            ret = "Connection String cannot be empty."
            MessageBox.Show(ret, "Change Password", "ConnectionString", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Return
        End If

        trMsg.Visible = False
        'If txtLogin.Text = String.Empty Then
        '    'LblMsg.Text = "Logon was not entered..."
        '    ret = "Logon was not entered..."
        '    MessageBox.Show(ret, "Change Password", "Logon", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        '    'trMsg.Visible = True
        '    ret = WriteToAccessLog(Session("logon"), "Request to change password. " & ret, 0)
        '    'txtLogin.Focus()
        '    Return
        'End If
        'If txtCurrent.Text = String.Empty Then
        '    'LblMsg.Text = "Current password was not entered..."
        '    ret = "Current password was not entered..."
        '    MessageBox.Show(ret, "Change Password", "Password", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

        '    'trMsg.Visible = True
        '    ret = WriteToAccessLog(Session("logon"), "Request to change password. " & ret, 0)
        '    'txtCurrent.Focus()
        '    Return
        'End If
        'If txtNew.Text = String.Empty Then
        '    LblMsg.Text = "New password was not entered..."
        '    trMsg.Visible = True
        '    ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '    txtNew.Focus()
        '    Return
        'End If
        'If txtRepeat.Text = String.Empty Then
        '    LblMsg.Text = "Repeat password was not entered..."
        '    trMsg.Visible = True
        '    ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '    txtRepeat.Focus()
        '    Return
        'End If
        If txtNew.Text <> txtRepeat.Text Then
            'LblMsg.Text = "New password and repeat password are not the same..."
            ret = "New password and repeat password are not the same..."
            MessageBox.Show(ret, "Change Password", "RepeatNotEqual", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            'trMsg.Visible = True
            'txtNew.Text = ""
            'txtRepeat.Text = ""
            ret = WriteToAccessLog(Session("logon"), "Request to change password. " & ret, 0)
            'txtNew.Focus()
            Return
        End If
        If txtCurrent.Text = txtNew.Text Then
            'LblMsg.Text = "New password and current password are the same..."
            ret = "New password and current password are the same..."
            MessageBox.Show(ret, "Change Password", "NewCurrentEqual", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            'txtNew.Text = ""

            'trMsg.Visible = True
            'txtNew.Text = ""
            'txtRepeat.Text = ""
            ret = WriteToAccessLog(Session("logon"), "Request to change password. " & ret, 0)
            'txtNew.Focus()
            Return
        End If
        If txtConnStr.Text = "" Then
            ret = "Connection string is empty..."
            MessageBox.Show(ret, "Change Password", "EmptyConnStr", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            ret = WriteToAccessLog(Session("logon"), "Request to change password. " & ret, 0)
            Return
        Else
            Session("UserConnString") = txtConnStr.Text
        End If
        Dim userconnstrnopass As String = Session("UserConnString")
        If userconnstrnopass.IndexOf("Password") > 0 Then
            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.IndexOf("Password")).Trim
        End If
        Dim orgEmail As String = String.Empty
        Dim orgUnit As String = String.Empty
        Dim orgConnStr As String = String.Empty
        Dim orgConnPrv As String = String.Empty
        Dim err As String = String.Empty
        Dim sql As String = String.Empty

        'If Session("admin") = "super" Then
        sql = "Select * From OURPermits Where NetId='" & txtLogin.Text & "' And LocalPass='" & txtCurrent.Text & "'"
        'Else
        '    sql = "Select * From OURPermits Where ConnStr LIKE '" & userconnstrnopass & "%' And NetId='" & txtLogin.Text & "' And LocalPass='" & txtCurrent.Text & "'"
        'End If

        Dim dv As DataView = mRecords(sql, err)  'from OUR db
        If err <> "" OrElse dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            If err = "" Then
                ret = "Current log in information is incorrect. User not found..."
            Else
                ret = "Error occurred during log in: " & err
            End If
            'trMsg.Visible = True
            MessageBox.Show(ret, "Change Password", "LogonError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
            'txtCurrent.Text = ""
            'txtCurrent.Focus()
            Return
        End If
        ret = ""
        orgEmail = dv.Table.Rows(0)("Email")
        If Not IsDBNull(dv.Table.Rows(0)("Unit")) Then _
          orgUnit = dv.Table.Rows(0)("Unit")
        If Not IsDBNull(dv.Table.Rows(0)("ConnStr")) Then _
            orgConnStr = dv.Table.Rows(0)("ConnStr")
        If Not IsDBNull(dv.Table.Rows(0)("ConnPrv")) Then _
        orgConnPrv = dv.Table.Rows(0)("ConnPrv")
        ''Unit should not be changed!
        'If txtUnit.Text.Trim <> "" Then
        '    sql = "UPDATE OURPERMITS SET Unit='" & txtUnit.Text & "' WHERE ConnStr='" & orgConnStr & "' AND NetID='" & txtLogin.Text & "' AND localpass='" & txtCurrent.Text & "' AND Application='InteractiveReporting'"
        '    ret = ExequteSQLquery(sql)
        '    If ret = "Query executed fine." Then
        '        WriteToAccessLog(Session("logon"), "Unit changed from " & orgUnit & " to " & txtUnit.Text, 0)
        '        ret = ""
        '    Else
        '        err = "Error: " & ret
        '        MessageBox.Show(err, "Company Entry Error", "CompanyError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        '        WriteToAccessLog(Session("logon"), "Problem to change Unit from " & orgUnit & " to " & txtUnit.Text & " - " & ret, 0)
        '        Return
        '    End If
        'End If
        If txtEmail.Text.Trim <> "" Then
            sql = "UPDATE OURPERMITS SET Email='" & txtEmail.Text & "' WHERE ConnStr='" & orgConnStr & "' AND NetID='" & txtLogin.Text & "' AND localpass='" & txtCurrent.Text & "' AND Application='InteractiveReporting'"
            ret = ExequteSQLquery(sql)
            If ret = "Query executed fine." Then
                WriteToAccessLog(Session("logon"), "Email changed from " & orgEmail & " to " & txtEmail.Text, 0)
                ret = ""
            Else
                err = "Error: " & ret
                MessageBox.Show(err, "Email Error", "EmailError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                WriteToAccessLog(Session("logon"), "Problem to change Email from " & orgEmail & " to " & txtEmail.Text & " - " & ret, 0)
                Return
            End If
        End If
        If txtConnStr.Text.Trim <> "" Then
            If txtConnStr.Text.IndexOf("Password") <> -1 Then
                txtConnStr.Text = txtConnStr.Text.Substring(0, txtConnStr.Text.IndexOf("Password")).Trim
            End If
            'ConnStr cannot be updated
            'sql = "UPDATE OURPERMITS SET ConnStr='" & txtConnStr.Text & "', ConnPrv='" & dropdownDatabases.SelectedValue.ToString & "' WHERE ConnStr='" & orgConnStr & "' AND NetID='" & txtLogin.Text & "' AND localpass='" & txtCurrent.Text & "' AND Application='InteractiveReporting'"
            sql = "UPDATE OURPERMITS SET ConnPrv='" & dropdownDatabases.SelectedValue.ToString & "' WHERE ConnStr='" & orgConnStr & "' AND NetID='" & txtLogin.Text & "' AND localpass='" & txtCurrent.Text & "' AND Application='InteractiveReporting'"
            ret = ExequteSQLquery(sql)
            If ret = "Query executed fine." Then
                WriteToAccessLog(Session("logon"), "ConnStr and ConnPrv changed from " & orgConnStr & ", " & orgConnPrv & " to " & txtConnStr.Text & ", " & dropdownDatabases.SelectedValue.ToString, 0)
                ret = ""
            Else
                err = "Error: " & ret
                MessageBox.Show(err, "Connection String", "ConnectionString", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                WriteToAccessLog(Session("logon"), "Problem to change ConnStr and ConnPrv from " & orgConnStr & ", " & orgConnPrv & " to " & txtConnStr.Text & ", " & dropdownDatabases.SelectedValue.ToString & " - " & ret, 0)
                Return
            End If
        End If
        If txtNew.Text <> String.Empty Then  'update password
            sql = "UPDATE OURPERMITS SET localpass='" & txtNew.Text & "' WHERE ConnStr='" & orgConnStr & "' AND NetID='" & txtLogin.Text & "' AND localpass='" & txtCurrent.Text & "' AND Application='InteractiveReporting'"
            ret = ExequteSQLquery(sql)
            If ret = "Query executed fine." Then
                Session("ChangePassword") = False
                Session("PasswordChanged") = "changed"
                ret = WriteToAccessLog(Session("logon"), "Password changed.", 0)
                'TODO send email to user about password changed
                Response.Redirect("default.aspx")
            End If
        End If
        '    If cleanText(txtOldPass.Text) <> txtOldPass.Text Then
        '        LblMsg.Text = "Wrong Old Password information. Illegal character found..."
        '        ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '    End If
        '    If cleanText(txtNewPass.Text) <> txtNewPass.Text Then
        '        LblMsg.Text = "Wrong New Password information. Illegal character found..."
        '        ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '    End If
        '    If cleanText(txtNewPass.Text) <> txtNewPass.Text Then
        '        LblMsg.Text = "Wrong Repeat Password information. Illegal character found..."
        '        ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '    End If
        '    If cleanText(txtLogon.Text) <> String.Empty AndAlso cleanText(txtOldPass.Text) <> String.Empty AndAlso cleanText(txtNewPass.Text) <> String.Empty AndAlso cleanText(txtRepPass.Text) <> String.Empty AndAlso cleanText(txtNewPass.Text) = cleanText(txtRepPass.Text) Then
        '        ret = ExequteSQLquery("UPDATE OURPERMITS SET localpass='" & cleanText(txtNewPass.Text) & "' WHERE NetID='" & Session("logon") & "' AND localpass='" & cleanText(txtOldPass.Text) & "' AND Application='InteractiveReporting'")
        '        If ret = "Query executed fine." Then
        '            Session("ChangePassword") = False
        '            ret = WriteToAccessLog(Session("logon"), "Password changed.", 0)
        '            'TODO send email to user about password changed
        '            Response.Redirect("default.aspx?msg=PasswordChanged")
        '        Else
        '            'Response.Write("<h1>Wrong information. Please go back and fill out the form again.</h1>")
        '            LblMsg.Text = "Wrong information. User has not been found."
        '            ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '            'Response.End()
        '        End If
        '    Else
        '        LblMsg.Text = "Wrong or missing information. Try again..."
        '        ret = WriteToAccessLog(Session("logon"), "Request to change password. " & LblMsg.Text, 0)
        '        'Response.Write("<h1>Wrong information. Please go back and fill out the form again.</h1>")
        '        'Response.End()
        '    End If
    End Sub
    'Sub SetFoc(id As String)
    '    Dim sb As New System.Text.StringBuilder("")
    '    Dim cs As ClientScriptManager = Me.ClientScript
    '    With sb
    '        .Append("<script language='JavaScript'>")
    '        .Append("function SetFocus()")
    '        .Append("{")
    '        .Append("var txt = document.getElementById('" & id & "');")
    '        .Append("txt.focus();")
    '        .Append("}")
    '        .Append("window.onload = SetFocus;")
    '        .Append("</script>")
    '        cs.RegisterStartupScript(Me.GetType, "SetFocus", sb.ToString())
    '    End With

    'End Sub

    Protected Sub dropdownDatabases_SelectedIndexChanged(sender As Object, e As EventArgs) Handles dropdownDatabases.SelectedIndexChanged
        txtConnStr.Text = String.Empty
        If dropdownDatabases.SelectedItem.Text.StartsWith("Intersystems") Then
            lblConnection.Text = "Server = yourserver; Port = yourport; Namespace = yournamespace; User ID = youruser; Password = yourpassword"
        ElseIf dropdownDatabases.SelectedItem.Text = "Oracle" Then
            lblConnection.Text = "data source=yourserver:yourport/yourdatabase;user id=youruser;password=yourpassword"
        Else
            lblConnection.Text = "Server=yourserver; Database=yourdatabase; User ID=youruser; Password=yourpassword"
        End If
    End Sub
    Sub SetText(id As String, text As String)
        ScriptManager.RegisterStartupScript(Me, Me.Page.GetType(), "settext" & id, "javascript:SetText('" & id & "','" & text & "')", True)
    End Sub
    Private Sub ClearNewPasswords()
        ScriptManager.RegisterStartupScript(Me, Me.Page.GetType(), "Clear", "javascript:ClearNewPswd()", True)
    End Sub
    Private Sub ClearText(CtlId As String)
        ScriptManager.RegisterStartupScript(Me, Me.Page.GetType(), "Clear" & CtlId, "javascript:ClearTextbox('" & CtlId & "')", True)
    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Select Case e.Tag
            Case "RepeatNotEqual", "NewCurrentEqual", "PasswordError"
                ClearNewPasswords()
                txtNew.Focus()
            Case "LogonError"
                ClearText("txtCurrent")
                txtCurrent.Focus()
            Case "CompanyError"
                ClearText("txtUnit")
                txtUnit.Focus()
            Case "EmailError"
                ClearText("txtEmail")
                txtEmail.Focus()
            Case "ConnectionString"
                ClearText("txtConnStr")
                txtConnStr.Focus()
        End Select
    End Sub

End Class
