Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Partial Class UnitDefinition
    Inherits System.Web.UI.Page
    Dim indx As String
    Private Sub UnitDefinition_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("admin").ToString <> "super" Then
            Response.Redirect("~/Default.aspx")
        End If
        Dim i As Integer
        Dim err As String = String.Empty
        Dim selprov As String = String.Empty
        Dim seluprov As String = String.Empty
        If Not Request("indx") Is Nothing Then
            indx = Request("indx").ToString
        Else
            indx = ""
            btnUpdate.Enabled = False
            btnUpdate.Visible = False
            btnSeeProposal.Enabled = False
            btnSeeProposal.Visible = False
            btnUninstall.Enabled = False
            btnUninstall.Visible = False
            btnDeleteUnit.Enabled = False
            btnDeleteUnit.Visible = False
        End If
        If indx = "" OrElse Request("install") Is Nothing Then
            btnInstall.Enabled = False
            btnInstall.Visible = False
        End If
        Session("UnitIndx") = indx
        txtLogon.Text = Session("logon").ToString.Trim
        If indx = "" Then
            'new user
            lblText1.Text = "Add by"
            Label10.Text = "new unit"
            Session("UnitWEB") = ""
        Else
            lblText1.Text = "Edit by"
            Session("UnitIndx") = indx
            Label10.Text = "Unit index #" & indx
            Dim mSql As String = "SELECT * FROM OURUnits WHERE Indx='" & indx & "'"
            Dim dt As DataTable = mRecords(mSql, err).Table  'Data for report by SQL statement from the OURdb database
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Response.Redirect("Default.aspx?msg=Unit is not found.")
                Exit Sub
            Else
                txtUnitWeb.Text = dt.Rows(0)("UnitWeb").ToString.Trim
                txtUnit.Text = dt.Rows(0)("Unit").ToString.Trim
                txtOURdb.Text = dt.Rows(0)("OURConnStr").ToString.Trim
                txtUserConnStr.Text = dt.Rows(0)("UserConnStr").ToString.Trim
                txtComments.Text = dt.Rows(0)("Comments").ToString.Trim

                For i = 0 To ddModels.Items.Count - 1
                    If ddModels.Items(i).Value = dt.Rows(0)("DistrMode").ToString.Trim.Replace(" ", "") Then
                        ddModels.Items(i).Selected = True
                    End If
                Next
                For i = 0 To ddOURConnPrv.Items.Count - 1
                    If ddOURConnPrv.Items(i).Value = dt.Rows(0)("OURConnPrv").ToString.Trim Then
                        ddOURConnPrv.Items(i).Selected = True
                        selprov = ddOURConnPrv.Items(i).Text.ToUpper
                    End If
                Next
                For i = 0 To ddUserConnPrv.Items.Count - 1
                    If ddUserConnPrv.Items(i).Value = dt.Rows(0)("UserConnPrv").ToString.Trim Then
                        ddUserConnPrv.Items(i).Selected = True
                        seluprov = ddUserConnPrv.Items(i).Text.ToUpper
                    End If
                Next
                Date1.Text = dt.Rows(0)("StartDate").ToString.Trim
                Date2.Text = dt.Rows(0)("EndDate").ToString.Trim

                Session("CurrentEndDate") = dt.Rows(0)("EndDate").ToString.Trim
                Session("Unit") = dt.Rows(0)("Unit").ToString.Trim
                Session("UnitWEB") = dt.Rows(0)("UnitWeb").ToString.Trim
                Session("UnitDB") = dt.Rows(0)("UserConnStr").ToString.Trim
                Session("UnitOURDB") = dt.Rows(0)("OURConnStr").ToString.Trim
            End If
        End If
        If Session("UnitWEB").ToString.Contains(Session("URL").ToString) OrElse Session("admin") = "super" Then
            HyperLinkUsers.Enabled = True
            HyperLinkUsers.Visible = True
        Else   'not the unit web site
            HyperLinkUsers.Enabled = False
            HyperLinkUsers.Visible = False
        End If

        For i = 0 To ddOURConnPrv.Items.Count - 1
            If ddOURConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SqlServerProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "SQL"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "CACHE"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "MYSQL"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "ORACLE"
            End If
            If ddOURConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
                ddOURConnPrv.Items(i).Enabled = False
                ddOURConnPrv.Items(i).Selected = False
            ElseIf selprov = "" AndAlso ddOURConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
                ddOURConnPrv.Text = ddOURConnPrv.Items(i).Text
                ddOURConnPrv.Items(i).Selected = True
                selprov = "CSV"
                Session("CSV") = "yes"
            End If
        Next

        For i = 0 To ddUserConnPrv.Items.Count - 1
            If ddUserConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SQLServerProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "SQL SERVER" AndAlso ConfigurationManager.AppSettings("SqlServerProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "SQL"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper.Contains("CACHE") AndAlso ConfigurationManager.AppSettings("CacheProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "CACHE"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "MYSQL" AndAlso ConfigurationManager.AppSettings("MySqlProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "MYSQL"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "ORACLE" AndAlso ConfigurationManager.AppSettings("Oracle").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "ORACLE"
            End If
            If ddUserConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString <> "OK" Then
                ddUserConnPrv.Items(i).Enabled = False
                ddUserConnPrv.Items(i).Selected = False
            ElseIf seluprov = "" AndAlso ddUserConnPrv.Items(i).Text.ToUpper = "CSV FILES" AndAlso ConfigurationManager.AppSettings("CSVProv").ToString = "OK" Then
                ddUserConnPrv.Text = ddUserConnPrv.Items(i).Text
                ddUserConnPrv.Items(i).Selected = True
                seluprov = "CSV"
                Session("CSV") = "yes"
            End If
        Next
        If Not Request("res") Is Nothing AndAlso Request("res").ToString.Trim <> "" Then
            Label9.Text = cleanText(Request("res").ToString)
        End If
    End Sub

    Private Sub UnitDefinition_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        'SAMPLE CLOSING WINDOWS FROM CODE BEHIND: - DO NOT DELETE!!!
        'Response.Write("<script language='javascript'> { window.close(); }</script>")
        Response.Redirect("UnitsAdmin.aspx")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'check textboxes
        Dim ret As String = String.Empty
        Try
            If cleanText(txtUnitWeb.Text.Trim) <> txtUnitWeb.Text.Trim Then
                ret = "Illegal character found in Unit Web text."
                Exit Try
            End If
            If cleanText(txtUnit.Text.Trim) <> txtUnit.Text.Trim Then
                ret = "Illegal character found in Unit text."
                Exit Try
            End If
            If cleanText(txtOURdb.Text.Trim) <> txtOURdb.Text.Trim Then
                ret = "Illegal character found in OUR db connection string."
                Exit Try
            End If
            If cleanText(txtUserConnStr.Text.Trim) <> txtUserConnStr.Text.Trim Then
                ret = "Illegal character found in User db connection string."
                Exit Try
            End If
            If cleanText(txtComments.Text.Trim) <> txtComments.Text.Trim Then
                ret = "Illegal character found in Comments."
                Exit Try
            End If
            'update OURUnits
            Dim sqlt As String = String.Empty
            Dim n As Integer = 0
            Dim sFields As String = String.Empty
            Dim sValues As String = String.Empty
            'restore user id in conn strings
            Dim ourdb1 As String = txtOURdb.Text.Trim
            If ourdb1.ToUpper.IndexOf("PASSWORD") > 0 Then ourdb1 = ourdb1.Substring(0, ourdb1.ToUpper.IndexOf("PASSWORD")).Trim
            Dim userdb1 As String = txtUserConnStr.Text.Trim
            If userdb1.ToUpper.IndexOf("PASSWORD") > 0 Then userdb1 = userdb1.Substring(0, userdb1.ToUpper.IndexOf("PASSWORD")).Trim
            If indx = "" Then
                'check if unit/db exist
                Dim hasrecord As Boolean = HasRecords("SELECT FROM OURUnits WHERE Unit='" & txtUnit.Text & "' AND OURConnStr='" & txtOURdb.Text.Trim & "' AND UserConnStr='" & txtUserConnStr.Text.Trim & "'")
                If hasrecord = False Then
                    txtComments.Text = txtComments.Text & " added by " & Session("logon")
                    sFields = "Unit,DistrMode,UnitWeb,Comments,StartDate,EndDate,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv"
                    sValues = "'" & txtUnit.Text & "',"
                    sValues &= "'" & ddModels.SelectedValue & "',"
                    sValues &= "'" & txtUnitWeb.Text & "',"
                    sValues &= "'" & txtComments.Text & "',"
                    sValues &= "'" & Date1.Text & "',"
                    sValues &= "'" & Date2.Text & "',"
                    sValues &= "'" & ourdb1 & "',"
                    sValues &= "'" & ddOURConnPrv.SelectedValue & "',"
                    sValues &= "'" & userdb1 & "',"
                    sValues &= "'" & ddUserConnPrv.SelectedValue & "'"

                    sqlt = "INSERT INTO OURUnits (" & sFields & ") VALUES (" & sValues & ")"

                    ret = "Unit inserted with result: " & ExequteSQLquery(sqlt)
                Else
                    Dim rt As String = "Request to add Unit crashed. Unit #" & txtUnit.Text & " is already there."
                    MessageBox.Show(rt, "Double Unit", "DoubleUnit", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                End If
            Else
                'update 
                Dim curdate As String = DateToString(Now)
                If txtComments.Text.IndexOf("edited by " & Session("logon")) < 0 Then
                    txtComments.Text = txtComments.Text & " - edited by " & Session("logon")
                End If
                If txtOURdb.Text.ToUpper.Replace(" ", "").IndexOf("PASSWORD=") > 0 Then
                    n = txtOURdb.Text.ToUpper.IndexOf("PASSWORD")
                    txtOURdb.Text = txtOURdb.Text.Substring(0, n)
                End If
                If txtUserConnStr.Text.ToUpper.Replace(" ", "").IndexOf("PASSWORD=") > 0 Then
                    n = txtUserConnStr.Text.ToUpper.IndexOf("PASSWORD")
                    txtUserConnStr.Text = txtUserConnStr.Text.Substring(0, n)
                End If
                If Session("Unit") = "CSV" OrElse Session("Unit") = "OUR" Then
                    ret = "For units OUR and CSV end date in OURPermits and OURPermissions shall not be changed. They are regulated by individual payment. "
                Else
                    'update active (or disabled at the same time as unit itself) users and report permissions
                    sqlt = "UPDATE OURPermits SET EndDate='" & DateToString(Date2.Text) & "' WHERE Unit='" & Session("Unit") & "' AND ConnStr='" & Session("UnitDB") & "' AND (EndDate > '" & curdate & "' OR EndDate = '" & Session("CurrentEndDate") & "')"
                    ret = "Users in OURPermits updated with result: " & ExequteSQLquery(sqlt)

                    sqlt = "UPDATE OURPermissions INNER JOIN OURReportInfo ON (OURPermissions.Param1=OURReportInfo.ReportId) INNER JOIN OURPermits ON (OURPermissions.NetId=OURPermits.NetId) "
                    sqlt = sqlt & " SET OURPermissions.OpenTo='" & DateToString(Date2.Text) & "' WHERE OURPermits.Unit='" & Session("Unit") & "' AND  OURReportInfo.ReportDB LIKE '%" & Session("UnitDB").ToString.Trim.Replace(" ", "%") & "%' AND (OURPermissions.OpenTo > '" & curdate & "' OR OURPermissions.OpenTo = '" & Session("CurrentEndDate") & "')"
                    ret = ret & " -Reports in OURPermissions updated with result: " & ExequteSQLquery(sqlt)
                End If
                'update unit
                sqlt = "UPDATE OURUnits SET Unit='" & txtUnit.Text & "', DistrMode='" & ddModels.SelectedValue & "', UnitWeb='" & txtUnitWeb.Text & "' , Comments='" & txtComments.Text & "' , StartDate='" & DateToString(Date1.Text) & "' , EndDate='" & DateToString(Date2.Text) & "',  "
                sqlt = sqlt & " OURConnStr='" & txtOURdb.Text.Trim & "', OURConnPrv='" & ddOURConnPrv.SelectedValue & "', UserConnStr='" & txtUserConnStr.Text.Trim & "', UserConnPrv='" & ddUserConnPrv.SelectedValue & "'  WHERE Indx='" & Session("UnitIndx") & "'"
                ret = ret & " -Unit updated with result: " & ExequteSQLquery(sqlt)
            End If

        Catch ex As Exception
            ret = ret & ex.Message
        End Try
        WriteToAccessLog(Session("logon"), ret, 11)
        Response.Redirect("UnitsAdmin.aspx")
    End Sub

    Private Sub btnDeleteUnit_Click(sender As Object, e As EventArgs) Handles btnDeleteUnit.Click
        Dim ret As String = "Request to disable Unit. Unit #" & Session("UnitIndx") & " will be permanently disabled along with Unit users and reports. Please confirm."
        MessageBox.Show(ret, "Disable Unit", "DisableUnit", Controls_Msgbox.Buttons.OKCancel, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.PostOK)

    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "DisableUnit" Then
            Select Case e.Result
                Case Controls_Msgbox.MessageResult.OK
                    Dim duedate As String = DateToString(Now)
                    Dim sqlt As String = String.Empty
                    Dim ret As String = String.Empty
                    'disable active at this moment users and report permissions
                    sqlt = "UPDATE OURPermits SET EndDate='" & duedate & "' WHERE Unit='" & Session("Unit") & "' AND ConnStr='" & Session("UnitDB") & "' AND (EndDate > '" & duedate & "')"
                    ret = "Users in OURPermits updated with result: " & ExequteSQLquery(sqlt)

                    sqlt = "UPDATE OURPermissions SET OpenTo='" & duedate & "' FROM OURPermissions INNER JOIN OURReportInfo ON (OURPermissions.Param1=OURReportInfo.ReportId) INNER JOIN OURPermits ON (OURPermissions.NetId=OURPermits.NetId) WHERE OURPermits.Unit='" & Session("Unit") & "' AND OURReportInfo.ReportDB LIKE '%" & Session("UnitDB").ToString.Trim.Replace(" ", "%") & "%' AND (OURPermissions.OpenTo > '" & duedate & "')"
                    ret = ret & " -Reports in OURPermissions updated with result: " & ExequteSQLquery(sqlt)

                    'disable Unit
                    sqlt = "UPDATE OURUnits SET EndDate='" & duedate & "'  WHERE Indx='" & Session("UnitIndx") & "'"
                    ret = ret & " -Unit updated with result: " & ExequteSQLquery(sqlt)

                    WriteToAccessLog(Session("logon"), "Unit #" & Session("UnitIndx") & " disabled with result: " & ret, 11)
                    Response.Redirect("UnitsAdmin.aspx")
                Case Controls_Msgbox.MessageResult.Cancel
                    txtUnit.Focus()
            End Select
        ElseIf e.Tag = "UpdateUnit" Then
            Select Case e.Result
                Case Controls_Msgbox.MessageResult.OK
                    'update Unit OURdb to current version
                    Dim ret As String = String.Empty
                    ret = UpdateOURdbToCurrentVersion(txtOURdb.Text.Trim, ddOURConnPrv.SelectedValue)
                    Label9.Text = ret
                    'update Unit Web and web.config
                    'TODO
                    WriteToAccessLog(Session("logon"), "Unit #" & Session("UnitIndx") & " updated to current version with result: " & ret, 11)
                    Response.Redirect("UnitDefinition.aspx?indx=" & Session("UnitIndx") & "&res=" & cleanText(ret.Replace("<br/>", " | ")))
                Case Controls_Msgbox.MessageResult.Cancel
                    txtUnit.Focus()
            End Select

        ElseIf e.Tag = "InstallUnit" Then
            'update Unit OURdb to current version
            Dim ret As String = String.Empty
            'TODO Install new Unit  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'ret = InstallNewUnit(Session("UnitIndx"),txtUnit.Text,txtOURdb.Text.Trim, ddOURConnPrv.SelectedValue)
            Label9.Text = ret
            'update Unit Web and web.config
            'TODO
            WriteToAccessLog(Session("logon"), "Unit #" & Session("UnitIndx") & " - " & txtUnit.Text & " installed with result: " & ret, 11)
            Response.Redirect("UnitDefinition.aspx?indx=" & Session("UnitIndx") & "&res=" & cleanText(ret.Replace("<br/>", " | ")))
        ElseIf e.Tag = "UninstallUnitOURdb" Then
            Select Case e.Result
                Case Controls_Msgbox.MessageResult.OK
                    'Uninstall Unit's OURdb
                    Dim ret As String = String.Empty
                    ret = UninstallOURTablesClasses(txtOURdb.Text.Trim, ddOURConnPrv.SelectedValue)
                    Label9.Text = ret
                    'delete Unit Web
                    'TODO
                    WriteToAccessLog(Session("logon"), "Unit #" & Session("UnitIndx") & " - " & txtUnit.Text & " OURdb tables uninstalled with result: " & ret, 11)
                    Response.Redirect("UnitDefinition.aspx?indx=" & Session("UnitIndx") & "&res=" & cleanText(ret.Replace("<br/>", " | ")))
                Case Controls_Msgbox.MessageResult.Cancel
                    txtUnit.Focus()
            End Select
        ElseIf e.Tag = "DoubleUnit" Then
            txtUnit.Focus()
        End If
    End Sub
    Protected Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If txtOURdb.Text.IndexOf("Password") < 0 Then
            txtOURdb.Text = txtOURdb.Text & " Password="
            txtOURdb.BorderColor = Color.Red
            txtOURdb.Focus()
        Else
            Dim ret As String = "Request to update the Unit. Unit #" & Session("UnitIndx") & " - " & txtUnit.Text & " will be updated to the current version of OUReports. Please confirm."
            MessageBox.Show(ret, "Update Unit", "UpdateUnit", Controls_Msgbox.Buttons.OKCancel, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
    End Sub
    Protected Sub btnUninstall_Click(sender As Object, e As EventArgs) Handles btnUninstall.Click
        If txtOURdb.Text.IndexOf("Password") < 0 Then
            txtOURdb.Text = txtOURdb.Text & " Password="
            txtOURdb.BorderColor = Color.Red
            txtOURdb.Focus()
        Else
            Dim ret As String = "Request to Unintstall all tables in the Unit's OURdb . Unit #" & Session("UnitIndx") & " - " & txtUnit.Text & " OURdb tables will be deleted. Please confirm."
            MessageBox.Show(ret, "Uninstall Unit OURdb", "UninstallUnitOURdb", Controls_Msgbox.Buttons.OKCancel, Controls_Msgbox.MessageIcon.Question, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
    End Sub
    Protected Sub btnInstall_Click(sender As Object, e As EventArgs) Handles btnInstall.Click
        If txtOURdb.Text.IndexOf("Password") < 0 Then
            txtOURdb.Text = txtOURdb.Text & " Password="
            txtOURdb.BorderColor = Color.Red
            txtOURdb.Focus()
        Else
            Dim ret As String = "Request to install new Unit. Unit #" & Session("UnitIndx") & " will be install. Please confirm."
            MessageBox.Show(ret, "Install Unit", "InstallUnit", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
    End Sub
    Protected Sub btnSeeProposal_Click(sender As Object, e As EventArgs) Handles btnSeeProposal.Click
        Response.Redirect("~/Partners.pdf")
    End Sub
End Class
