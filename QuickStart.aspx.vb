Imports System.Data
Partial Class QuickStart
    Inherits System.Web.UI.Page
    Private Sub QuickStart_Init(sender As Object, e As EventArgs) Handles Me.Init
        LblInvalid.Text = "Your data (CSV, XML, JSON, XLS, XLSX, Access table) can be uploaded into our database. Reports and analytics will be available only to you and to users you share them with."
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
    End Sub
    Protected Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        LblInvalid.Text = ""
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Session("logon") = cleanText(txtEmail.Text.Trim)

        If Session("logon") <> txtEmail.Text.Trim Then
            ret = "Illegal character found. Please retype Email."
            LblInvalid.Text = ret
            Exit Sub
        ElseIf Session("logon") = "" Then
            ret = "Email cannot be empty."
            LblInvalid.Text = ret
            Exit Sub
        End If
        Session("CSV") = "yes"
        Session("Unit") = "CSV"
        Session("UserConnProvider") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ProviderName.ToString
        Session("UserConnString") = System.Configuration.ConfigurationManager.ConnectionStrings.Item("CSVconnection").ToString
        Session("admin") = "admin"
        Session("Application") = "InteractiveReporting"
        Session("AdvancedUser") = True

        Dim userconnstrnopass As String = Session("UserConnString")
        If userconnstrnopass.ToUpper.IndexOf("PASSWORD") > 0 Then
            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("PASSWORD"))
        ElseIf userconnstrnopass.ToUpper.IndexOf("PWD") > 0 Then
            userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.ToUpper.IndexOf("PWD")).Trim
        End If

        Dim hasrec As Boolean = HasRecords("SELECT * FROM OURPermits WHERE (Application='InteractiveReporting' AND NetId='" & Session("logon") & "')")
        If hasrec Then
            ret = "The email is already used. Please log in or select a different one."
            LblInvalid.Text = ret
            Exit Sub
        End If

        If Not DatabaseConnected(Session("UserConnString"), Session("UserConnProvider")) Then
            ret = "Connection to our database can not been open... Please contact us."
            LblInvalid.Text = ret
            Exit Sub
            WriteToAccessLog(txtEmail.Text.Trim, "Connection to CSV database cannot be open: " & Session("UserConnString"), 1)
        End If

        Dim ndays As Integer = 2000
        If Not ConfigurationManager.AppSettings("DaysFree") Is Nothing AndAlso IsNumeric(ConfigurationManager.AppSettings("DaysFree").ToString) Then
            ndays = CInt(ConfigurationManager.AppSettings("DaysFree").ToString)
        End If
        Session("UserEndDate") = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, ndays, Now())))
        ret = RegisterUser("user", "friendly", Session("logon"), Session("logon"), Session("logon"), "CSV", "InteractiveReporting", "admin", "", "", "CSV", Session("logon"), "quick registration", userconnstrnopass.Trim, Session("UserConnProvider").Trim, DateToString(CDate(Now())), Session("UserEndDate"))

        'repid = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        ''add "\","/",":","*","?","""","<",">","|","'"
        'repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")
        'Session("REPORTID") = repid
        'Session("REPTITLE") = repid

        ''create new row in ReportInfo table
        'hasrec = HasRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")

        'Dim SQLq As String = String.Empty
        'If Not hasrec Then
        '    SQLq = "INSERT INTO OURReportInfo (ReportID, ReportName,ReportTtl,ReportType,ReportAttributes,Param9type,ReportDB) VALUES ('" & repid & "','" & repid & "','" & Session("REPTITLE") & "','rdl','sql','portrait','" & userconnstrnopass & "')"
        '    ret = ExequteSQLquery(SQLq)
        'Else
        '    LblInvalid.Text = "Report Name is not unique, please repeat..."
        '    Exit Sub
        'End If

        'If ret <> "Query executed fine." Then
        '    LblInvalid.Text = "ERROR!! Report has not been created: " & ret
        '    Exit Sub
        'End If
        Session("email") = cleanText(txtEmail.Text.Trim)
        ''no users yet: add creator as admin for this report
        'If Session("REPORTID") <> "" And Session("logon") <> "" Then
        '    SQLq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & Session("REPORTID") & "','" & Session("REPTITLE") & "','" & Session("email") & "','admin','" & DateToString(Now) & "','" & Session("UserEndDate") & "','" & Session("logon") & "')"
        '    ret = ExequteSQLquery(SQLq)
        '    If ret <> "Query executed fine." Then
        '        LblInvalid.Text = "ERROR!! Report has not been created: " & ret
        '        Exit Sub
        '    End If
        '    WriteToAccessLog(Session("logon"), "Report  " & Session("REPORTID") & " has been created:" & ret, 0)
        'End If

        Dim url As String = HttpContext.Current.Request.Url.AbsoluteUri
        Session("URL") = url.Substring(0, url.LastIndexOf("/")) & "/"
        Session("UnitWEB") = ""

        'Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=new")
        Response.Redirect("DataImport.aspx")
    End Sub
    Protected Sub ButtonVideo_Click(sender As Object, e As EventArgs) Handles ButtonVideo.Click
        Response.Redirect("Videos/QuickStart.mp4")
    End Sub
End Class
