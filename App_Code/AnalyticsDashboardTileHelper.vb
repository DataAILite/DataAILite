Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Public NotInheritable Class AnalyticsDashboardTileHelper
    Private Const ChartTypeAnalytics As String = "analytics"

    Private Sub New()
    End Sub

    Public Shared Sub Attach(page As Page)
        If page Is Nothing Then Exit Sub
        If page.Items("AnalyticsDashboardTileHelperAttached") IsNot Nothing Then Exit Sub
        page.Items("AnalyticsDashboardTileHelperAttached") = True

        Dim helpLink As HyperLink = TryCast(FindControlRecursive(page, "HyperLinkHelp"), HyperLink)
        If helpLink Is Nothing OrElse helpLink.Parent Is Nothing Then Exit Sub

        Dim link As New LinkButton()
        link.ID = "lnkAddAnalyticsDashboard"
        link.ClientIDMode = ClientIDMode.Static
        link.Text = "add to dashboard"
        link.CssClass = "NodeStyle"
        link.Font.Names = New String() {"Arial"}
        link.ToolTip = "Add this analytics page with current selected controls to an Analytics Dashboard."
        link.OnClientClick = "showAnalyticsDashboardDialog();return false;"
        link.Visible = UserCanEditDashboards(page)

        Dim parent As Control = helpLink.Parent
        Dim index As Integer = parent.Controls.IndexOf(helpLink)
        If index >= 0 Then
            parent.Controls.AddAt(index, New LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;"))
            parent.Controls.AddAt(index, link)
        End If

        AddDialogControls(page)
        RegisterScript(page)
    End Sub

    Private Shared Function UserCanEditDashboards(page As Page) As Boolean
        If page.Session Is Nothing OrElse page.Session("admin") Is Nothing Then Return False
        Dim admin As String = page.Session("admin").ToString().Trim().ToLowerInvariant()
        Return admin = "admin" OrElse admin = "super"
    End Function

    Private Shared Sub AddDialogControls(page As Page)
        Dim form As HtmlForm = TryCast(FindControlRecursive(page, "form1"), HtmlForm)
        If form Is Nothing OrElse FindControlRecursive(form, "pnlAnalyticsDashboardPopup") IsNot Nothing Then Exit Sub

        Dim hiddenSelected As New HiddenField()
        hiddenSelected.ID = "hdnAnalyticsDashboardSelected"
        hiddenSelected.ClientIDMode = ClientIDMode.Static
        form.Controls.Add(hiddenSelected)

        Dim panel As New Panel()
        panel.ID = "pnlAnalyticsDashboardPopup"
        panel.ClientIDMode = ClientIDMode.Static
        panel.Style("display") = "none"
        panel.Style("position") = "absolute"
        panel.Style("top") = "0"
        panel.Style("left") = "0"
        panel.Style("width") = "100%"
        panel.Style("height") = "100%"
        panel.Style("background-color") = "rgba(158, 188, 250,0.5)"
        panel.Style("z-index") = "2147483600"

        Dim box As New Panel()
        box.ID = "pnlAnalyticsDashboardBox"
        box.ClientIDMode = ClientIDMode.Static
        box.Style("position") = "absolute"
        box.Style("top") = "25%"
        box.Style("left") = "25%"
        box.Style("width") = "50%"
        box.Style("height") = "320px"
        box.Style("background-color") = "#e6eefa"
        box.Style("border") = "1px solid #222222"
        box.Style("font-family") = "Arial"
        box.Style("font-size") = "small"

        box.Controls.Add(New LiteralControl("<div style=""font-size:small;text-align:center;background-color:gray;width:100%;height:22px;line-height:22px;color:white;"">Add To Dashboard <span onclick=""closeAnalyticsDashboardDialog();"" title=""close dialog"" style=""color:white;float:right;font-size:20px;font-weight:bold;padding-right:10px;cursor:pointer;"">&times;</span></div>"))
        box.Controls.Add(New LiteralControl("<div style=""clear:both;"">"))
        box.Controls.Add(New LiteralControl("<div style=""float:left;width:80%;height:400px;padding:0px;"">"))
        box.Controls.Add(New LiteralControl("<div style=""margin-left:5px;"">"))
        box.Controls.Add(New LiteralControl("<div style=""display:inline;width:100%;height:30px;padding-top:5px;padding-bottom:5px;"">Name: "))

        Dim search As New TextBox()
        search.ID = "txtAnalyticsDashboardSearch"
        search.ClientIDMode = ClientIDMode.Static
        search.Width = Unit.Pixel(200)
        search.ToolTip = "Type text to filter dashboards, or type a new analytics dashboard name."
        box.Controls.Add(search)
        box.Controls.Add(New LiteralControl("&nbsp;"))
        Dim find As New Button()
        find.ID = "btnAnalyticsDashboardFind"
        find.ClientIDMode = ClientIDMode.Static
        find.Text = "Find"
        find.CssClass = "dlgboxbutton"
        find.CausesValidation = False
        find.ToolTip = "Find dashboards matching the text in the name box."
        find.OnClientClick = "filterAnalyticsDashboardList();return false;"
        ApplyDialogButtonStyle(find)
        box.Controls.Add(find)
        box.Controls.Add(New LiteralControl("</div>"))
        box.Controls.Add(New LiteralControl("<div style=""background-color:darkgray;border:1px solid #808080;height:20px;line-height:20px;padding-left:8px;color:#FFFFFF;"">Dashboards</div>"))

        Dim listPanel As New Panel()
        listPanel.ID = "pnlAnalyticsDashboardList"
        listPanel.ClientIDMode = ClientIDMode.Static
        listPanel.Style("border-style") = "none solid solid solid"
        listPanel.Style("border-right-width") = "1px"
        listPanel.Style("border-bottom-width") = "1px"
        listPanel.Style("border-left-width") = "1px"
        listPanel.Style("border-right-color") = "#808080"
        listPanel.Style("border-bottom-color") = "#808080"
        listPanel.Style("border-left-color") = "#808080"
        listPanel.Style("height") = "225px"
        listPanel.Style("overflow-y") = "scroll"

        Dim list As New CheckBoxList()
        list.ID = "lstAnalyticsDashboards"
        list.ClientIDMode = ClientIDMode.Static
        list.Width = Unit.Percentage(100)
        list.BorderStyle = BorderStyle.None
        list.RepeatLayout = RepeatLayout.Table
        PopulateDashboards(page, list)
        If list.Items.Count = 0 Then search.Text = "Custom Analytics"
        listPanel.Controls.Add(list)
        box.Controls.Add(listPanel)

        box.Controls.Add(New LiteralControl("</div></div>"))
        box.Controls.Add(New LiteralControl("<div style=""float:left;width:19%;height:120px;text-align:center;padding:0px;"">"))
        Dim save As New Button()
        save.ID = "btnAnalyticsDashboardSave"
        save.ClientIDMode = ClientIDMode.Static
        save.Text = "Add"
        save.CssClass = "dlgboxbutton"
        save.CausesValidation = False
        save.OnClientClick = "return submitAnalyticsDashboardDialog();"
        ApplyDialogButtonStyle(save)
        AddHandler save.Click, AddressOf SaveButtonClick
        box.Controls.Add(save)

        Dim cancel As New Button()
        cancel.ID = "btnAnalyticsDashboardCancel"
        cancel.ClientIDMode = ClientIDMode.Static
        cancel.Text = "Cancel"
        cancel.CssClass = "dlgboxbutton"
        cancel.CausesValidation = False
        cancel.OnClientClick = "closeAnalyticsDashboardDialog();return false;"
        ApplyDialogButtonStyle(cancel)
        box.Controls.Add(New LiteralControl("<br />"))
        box.Controls.Add(cancel)
        box.Controls.Add(New LiteralControl("</div></div>"))

        panel.Controls.Add(box)
        form.Controls.Add(panel)
    End Sub

    Private Shared Sub ApplyDialogButtonStyle(button As Button)
        button.Width = Unit.Pixel(80)
        button.Height = Unit.Pixel(25)
        button.Style("font-size") = "12px"
        button.Style("border-radius") = "5px"
        button.Style("border-style") = "solid"
        button.Style("border-color") = "#4e4747"
        button.Style("color") = "black"
        button.Style("border-width") = "1px"
        button.Style("background-image") = "linear-gradient(to bottom, rgba(158, 188, 250,0),rgba(158, 188, 250,1))"
        button.Style("padding") = "3px"
        button.Style("margin") = "5px"
        button.Style("z-index") = "9999"
    End Sub

    Private Shared Sub PopulateDashboards(page As Page, list As CheckBoxList)
        list.Items.Clear()
        If page.Session Is Nothing OrElse page.Session("logon") Is Nothing Then Exit Sub
        Dim userId As String = SqlText(page.Session("logon").ToString())
        Dim sql As String = "SELECT DISTINCT Dashboard FROM ourdashboards WHERE UserID='" & userId & "'"
        sql &= " ORDER BY Dashboard"
        Try
            Dim dv As DataView = mRecords(sql)
            If dv Is Nothing OrElse dv.Table Is Nothing Then Exit Sub
            For Each row As DataRow In dv.Table.Rows
                Dim name As String = row("Dashboard").ToString()
                If name.Trim() <> "" Then list.Items.Add(New ListItem(name, name))
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Sub SaveButtonClick(sender As Object, e As EventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button Is Nothing OrElse button.Page Is Nothing Then Exit Sub
        Dim page As Page = button.Page
        Dim selected As HiddenField = TryCast(FindControlRecursive(page, "hdnAnalyticsDashboardSelected"), HiddenField)
        Dim dashboardText As String = If(selected Is Nothing, "", selected.Value.Trim())
        If dashboardText = "" Then Exit Sub

        Dim dashboardNames() As String = dashboardText.Split(","c)
        For Each rawName As String In dashboardNames
            Dim dashboardName As String = rawName.Trim()
            If dashboardName <> "" Then SaveAnalyticsDashboardItem(page, dashboardName)
        Next

        page.Response.Redirect(DashboardListUrl(page))
    End Sub

    Private Shared Function DashboardListUrl(page As Page) As String
        Dim reportId As String = CurrentReportId(page)
        If reportId.Trim() = "" Then Return "ListOfDashboards.aspx"
        Return "ListOfDashboards.aspx?Report=" & page.Server.UrlEncode(reportId)
    End Function

    Private Shared Sub SaveAnalyticsDashboardItem(page As Page, dashboardName As String)
        If page.Session Is Nothing OrElse page.Session("logon") Is Nothing Then Exit Sub

        Dim userId As String = page.Session("logon").ToString()
        Dim reportId As String = CurrentReportId(page)
        If userId.Trim() = "" OrElse reportId.Trim() = "" Then Exit Sub
        Dim url As String = BuildCurrentPageUrl(page)
        Dim title As String = CurrentPageTitle(page)
        Dim safeUser As String = SqlText(userId)
        Dim safeDashboard As String = SqlText(dashboardName)
        Dim safeReport As String = SqlText(reportId)
        Dim safeUrl As String = SqlText(EncodeArr(url))
        Dim safeTitle As String = SqlText(TruncateText(title, 240))

        Dim sql As String = "SELECT * FROM ourdashboards WHERE UserID='" & safeUser & "' AND Dashboard='" & safeDashboard & "' AND ChartType='" & ChartTypeAnalytics & "' AND ARR='" & safeUrl & "'"
        If Not HasRecords(sql) Then
            Dim fields As String = "UserID,Dashboard,ReportID,ChartType,GraphTitle,ARR"
            Dim values As String = "'" & safeUser & "','" & safeDashboard & "','" & safeReport & "','" & ChartTypeAnalytics & "','" & safeTitle & "','" & safeUrl & "'"
            ExequteSQLquery("INSERT INTO ourdashboards (" & fields & ") VALUES (" & values & ")")
        Else
            ExequteSQLquery("UPDATE ourdashboards SET GraphTitle='" & safeTitle & "', ReportID='" & safeReport & "', ChartType='" & ChartTypeAnalytics & "' WHERE UserID='" & safeUser & "' AND Dashboard='" & safeDashboard & "' AND ChartType='" & ChartTypeAnalytics & "' AND ARR='" & safeUrl & "'")
        End If

        GetDashboardIdentifier(dashboardName, userId)
    End Sub

    Private Shared Function BuildCurrentPageUrl(page As Page) As String
        Dim fileName As String = VirtualPathUtility.GetFileName(page.AppRelativeVirtualPath)
        If fileName.Trim() = "" Then fileName = VirtualPathUtility.GetFileName(page.Request.Path)

        Dim parameters As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim reportId As String = CurrentReportId(page)
        If reportId.Trim() <> "" Then parameters("Report") = reportId

        For Each key As String In page.Request.QueryString.AllKeys
            If key IsNot Nothing AndAlso Not parameters.ContainsKey(key) Then parameters(key) = page.Request.QueryString(key)
        Next
        If reportId.Trim() <> "" Then parameters("Report") = reportId

        AddMappedControlValue(page, parameters, "DropDownRowField", "cat1")
        AddMappedControlValue(page, parameters, "DropDownPrimaryField", "cat1")
        AddMappedControlValue(page, parameters, "DropDownDimension", "cat1")
        AddMappedControlValue(page, parameters, "ListBoxDimension", "cat1")
        AddMappedControlValue(page, parameters, "DropDownGroupField", "group")
        AddMappedControlValue(page, parameters, "DropDownCategoryField", "cat2")
        AddMappedControlValue(page, parameters, "DropDownColumnField", "cat2")
        AddMappedControlValue(page, parameters, "DropDownSecondaryField", "cat2")
        AddMappedControlValue(page, parameters, "DropDownCompareField", "cat2")
        AddMappedControlValue(page, parameters, "DropDownValueField", "y1")
        AddMappedControlValue(page, parameters, "DropDownSecondaryValueField", "y2")
        AddMappedControlValue(page, parameters, "DropDownAggregate", "fn")
        AddMappedControlValue(page, parameters, "DropDownAggregation", "fn")
        AddMappedControlValue(page, parameters, "DropDownDateField", "date")
        AddMappedControlValue(page, parameters, "DropDownDateAggregation", "dateagg")
        AddMappedControlValue(page, parameters, "DropDownDateRollup", "dateagg")
        AddMappedControlValue(page, parameters, "DropDownPeriod", "period")
        AddMappedControlValue(page, parameters, "DropDownAnalysisType", "analysis")
        AddMappedControlValue(page, parameters, "DropDownComparisonType", "comparison")
        AddMappedControlValue(page, parameters, "DropDownEquationType", "equation")
        AddMappedControlValue(page, parameters, "DropDownXField", "x1")
        AddMappedControlValue(page, parameters, "DropDownYField", "y1")
        AddMappedControlValue(page, parameters, "DropDownOperation", "operation")
        AddMappedControlValue(page, parameters, "DropDownMethod", "method")
        AddMappedControlValue(page, parameters, "DropDownFieldGroup", "fieldgroup")
        AddMappedControlValue(page, parameters, "DropDownExamples", "examples")
        AddMappedControlValue(page, parameters, "DropDownFocus", "focus")
        AddMappedControlValue(page, parameters, "DropDownDetail", "detail")
        AddMappedControlValue(page, parameters, "DropDownCompareReport", "compareReport")
        AddMappedControlValue(page, parameters, "DropDownKeyField", "key")
        AddMappedControlValue(page, parameters, "DropDownLatitude", "lat")
        AddMappedControlValue(page, parameters, "DropDownLongitude", "lon")
        AddMappedControlValue(page, parameters, "DropDownNameField", "name")
        AddMappedControlValue(page, parameters, "DropDownView", "view")
        AddMappedControlValue(page, parameters, "DropDownBaseValue", "base")
        AddMappedControlValue(page, parameters, "DropDownCompareValue", "compare")
        AddMappedControlValue(page, parameters, "DropDownInventoryField", "inventory")
        AddMappedControlValue(page, parameters, "txtSearch", "search")
        AddMappedControlValue(page, parameters, "txtPredictX", "predictx")
        AddMappedControlValue(page, parameters, "txtWindow", "window")
        AddMappedControlValue(page, parameters, "txtStageOrder", "stageorder")
        AddMappedControlValue(page, parameters, "txtScoreThreshold", "threshold")
        AddMappedControlValue(page, parameters, "txtStdDev", "stdev")
        AddMappedControlValue(page, parameters, "txtStdDevLimit", "stdev")
        AddMappedControlValue(page, parameters, "txtPercent", "percent")
        AddMappedControlValue(page, parameters, "txtMin", "min")
        AddMappedControlValue(page, parameters, "txtMax", "max")
        AddMappedControlValue(page, parameters, "txtMissingPercent", "missing")
        AddMappedControlValue(page, parameters, "txtVariancePercent", "variance")
        AddMappedControlValue(page, parameters, "txtCorrelationThreshold", "corr")
        AddMappedControlValue(page, parameters, "txtOutlierThreshold", "outlier")
        AddMappedControlValue(page, parameters, "txtChurnScore", "churn")
        AddMappedControlValue(page, parameters, "txtThreshold", "threshold")
        AddMappedControlValue(page, parameters, "txtAssumption", "assumption")
        AddMappedControlValue(page, parameters, "txtBaseQuery", "basequery")
        AddMappedControlValue(page, parameters, "txtCompareQuery", "comparequery")
        AddMappedControlValue(page, parameters, "chkMapReadiness", "mapfailed")

        Dim sb As New StringBuilder(fileName)
        Dim first As Boolean = True
        For Each item As KeyValuePair(Of String, String) In parameters
            If item.Value Is Nothing Then Continue For
            If first Then
                sb.Append("?")
                first = False
            Else
                sb.Append("&")
            End If
            sb.Append(HttpUtility.UrlEncode(item.Key))
            sb.Append("=")
            sb.Append(HttpUtility.UrlEncode(item.Value))
        Next
        Return sb.ToString()
    End Function

    Private Shared Sub AddMappedControlValue(page As Page, parameters As Dictionary(Of String, String), controlId As String, parameterName As String)
        Dim ctl As Control = FindControlRecursive(page, controlId)
        If ctl Is Nothing Then Exit Sub
        Dim value As String = ControlValue(ctl)
        If value Is Nothing Then Exit Sub
        value = value.Trim()
        If value = "" OrElse value = "Please select..." Then Exit Sub
        parameters(parameterName) = value
    End Sub

    Private Shared Function ControlValue(ctl As Control) As String
        If TypeOf ctl Is DropDownList Then Return CType(ctl, DropDownList).SelectedValue
        If TypeOf ctl Is TextBox Then Return CType(ctl, TextBox).Text
        If TypeOf ctl Is CheckBox Then Return If(CType(ctl, CheckBox).Checked, "true", "false")
        If TypeOf ctl Is ListBox Then
            Dim values As New List(Of String)()
            For Each item As ListItem In CType(ctl, ListBox).Items
                If item.Selected AndAlso item.Value.Trim() <> "" Then values.Add(item.Value)
            Next
            Return String.Join(",", values.ToArray())
        End If

        Dim prop = ctl.GetType().GetProperty("SelectedItemsString")
        If prop IsNot Nothing AndAlso prop.CanRead Then
            Dim result As Object = prop.GetValue(ctl, Nothing)
            If result IsNot Nothing Then Return result.ToString()
        End If
        Return Nothing
    End Function

    Private Shared Function CurrentPageTitle(page As Page) As String
        Dim actionTitle As String = CurrentPageActionTitle(page)
        If actionTitle.Trim() <> "" Then Return actionTitle

        Dim labelIds() As String = {"lblHeader", "LabelPageTtl"}
        For Each id As String In labelIds
            Dim lbl As Label = TryCast(FindControlRecursive(page, id), Label)
            If lbl IsNot Nothing AndAlso lbl.Text.Trim() <> "" Then Return StripHtml(lbl.Text.Trim())
        Next
        Return VirtualPathUtility.GetFileName(page.AppRelativeVirtualPath)
    End Function

    Private Shared Function CurrentPageActionTitle(page As Page) As String
        If page Is Nothing OrElse page.Request Is Nothing Then Return ""

        Dim fileName As String = VirtualPathUtility.GetFileName(page.AppRelativeVirtualPath)
        If fileName.Trim() = "" Then fileName = VirtualPathUtility.GetFileName(page.Request.Path)

        If fileName.Equals("ShowReport.aspx", StringComparison.OrdinalIgnoreCase) Then
            Return ShowReportActionTitle(page.Request("srd"))
        End If

        If fileName.Equals("ReportViews.aspx", StringComparison.OrdinalIgnoreCase) Then
            If RequestValue(page, "grpstats").Equals("yes", StringComparison.OrdinalIgnoreCase) Then Return "See Groups Statistics"
            If RequestValue(page, "gen").Equals("yes", StringComparison.OrdinalIgnoreCase) Then Return "Show Generic Report"
            If RequestValue(page, "det").Equals("yes", StringComparison.OrdinalIgnoreCase) Then Return "DrillDown Groups"

            Dim graphType As String = RequestValue(page, "grtype").Trim().ToLowerInvariant()
            Select Case graphType
                Case "matrix"
                    Return "Matrix / Pivot Report"
                Case "bar"
                    Return "Bar Report"
                Case "pie"
                    Return "Pie Report"
                Case "line"
                    Return "Line Report"
            End Select

            Return ShowReportActionTitle(RequestValue(page, "srd"))
        End If

        Return ""
    End Function

    Private Shared Function ShowReportActionTitle(srdValue As String) As String
        Select Case If(srdValue, "").Trim()
            Case "0"
                Return "Explore Report Data"
            Case "1"
                Return "Export Data to Excel"
            Case "2"
                Return "Export Data to CSV"
            Case "3"
                Return "Show Formatted Report"
            Case "4"
                Return "Export Report to Excel"
            Case "5"
                Return "Export Report to Word"
            Case "6"
                Return "Export Report to PDF"
            Case "8"
                Return "See Data Overall Statistics"
            Case "9"
                Return "Export Overall Statistics to Excel"
            Case "10"
                Return "Export Data to Delimited File"
            Case "12"
                Return "See Fields Correlation"
            Case "13"
                Return "Matrix Balancing"
            Case "14"
                Return "Export Data to XML"
            Case "17"
                Return "Show Report Charts"
        End Select
        Return ""
    End Function

    Private Shared Function RequestValue(page As Page, key As String) As String
        If page Is Nothing OrElse page.Request Is Nothing OrElse page.Request(key) Is Nothing Then Return ""
        Return page.Request(key).ToString()
    End Function

    Private Shared Function CurrentReportId(page As Page) As String
        If page Is Nothing Then Return ""

        Dim requestKeys() As String = {"Report", "REPORT", "ReportID", "REPORTID", "repid"}
        For Each key As String In requestKeys
            If page.Request(key) IsNot Nothing AndAlso page.Request(key).ToString().Trim() <> "" Then
                If page.Session IsNot Nothing Then page.Session("REPORTID") = page.Request(key).ToString().Trim()
                Return page.Request(key).ToString().Trim()
            End If
        Next

        If page.Session IsNot Nothing AndAlso page.Session("REPORTID") IsNot Nothing AndAlso page.Session("REPORTID").ToString().Trim() <> "" Then
            Return page.Session("REPORTID").ToString().Trim()
        End If

        If page.Session IsNot Nothing AndAlso page.Session("DataReadinessScannerReportID") IsNot Nothing AndAlso page.Session("DataReadinessScannerReportID").ToString().Trim() <> "" Then
            page.Session("REPORTID") = page.Session("DataReadinessScannerReportID").ToString().Trim()
            Return page.Session("DataReadinessScannerReportID").ToString().Trim()
        End If

        Dim labelIds() As String = {"LabelReportID", "lblReportID"}
        For Each id As String In labelIds
            Dim lbl As Label = TryCast(FindControlRecursive(page, id), Label)
            If lbl IsNot Nothing AndAlso lbl.Text.Trim() <> "" Then Return StripHtml(lbl.Text.Trim())
        Next

        Return ""
    End Function

    Private Shared Function StripHtml(value As String) As String
        Return value.Replace("<b>", "").Replace("</b>", "").Replace("&lt;b&gt;", "").Replace("&lt;/b&gt;", "").Trim()
    End Function

    Private Shared Function EncodeArr(value As String) As String
        If value Is Nothing Then Return ""
        Return value.Replace("'", "^^").Replace("[", "**").Replace("]", "##")
    End Function

    Private Shared Function SqlText(value As String) As String
        If value Is Nothing Then Return ""
        Return value.Replace("'", "''")
    End Function

    Private Shared Function TruncateText(value As String, maxLength As Integer) As String
        If value Is Nothing Then Return ""
        If value.Length <= maxLength Then Return value
        Return value.Substring(0, maxLength)
    End Function

    Private Shared Sub RegisterScript(page As Page)
        Dim script As String = String.Join(vbCrLf, New String() {
            "function showAnalyticsDashboardDialog() {",
            "    var popup = document.getElementById('pnlAnalyticsDashboardPopup');",
            "    var txt = document.getElementById('txtAnalyticsDashboardSearch');",
            "    var btn = document.getElementById('btnAnalyticsDashboardSave');",
            "    if (popup) { popup.style.display = ''; }",
            "    analyticsDashboardSelectionChanged();",
            "    if (txt) { txt.focus(); }",
            "}",
            "function closeAnalyticsDashboardDialog() {",
            "    var popup = document.getElementById('pnlAnalyticsDashboardPopup');",
            "    if (popup) { popup.style.display = 'none'; }",
            "}",
            "function analyticsDashboardSelectionChanged() {",
            "    var list = document.getElementById('lstAnalyticsDashboards');",
            "    var txt = document.getElementById('txtAnalyticsDashboardSearch');",
            "    var btn = document.getElementById('btnAnalyticsDashboardSave');",
            "    var hasChecked = false;",
            "    if (list) {",
            "        var checks = list.getElementsByTagName('input');",
            "        for (var i = 0; i < checks.length; i++) {",
            "            if (checks[i].checked) { hasChecked = true; break; }",
            "        }",
            "    }",
            "    if (btn) { btn.disabled = !(hasChecked || (txt && txt.value.replace(/^\s+|\s+$/g, '') != '')); }",
            "}",
            "function filterAnalyticsDashboardList() {",
            "    var txt = document.getElementById('txtAnalyticsDashboardSearch');",
            "    var list = document.getElementById('lstAnalyticsDashboards');",
            "    if (!txt || !list) { return; }",
            "    var q = txt.value.toLowerCase();",
            "    var rows = list.getElementsByTagName('tr');",
            "    for (var i = 0; i < rows.length; i++) {",
            "        rows[i].style.display = rows[i].innerText.toLowerCase().indexOf(q) >= 0 ? '' : 'none';",
            "    }",
            "    analyticsDashboardSelectionChanged();",
            "}",
            "function submitAnalyticsDashboardDialog() {",
            "    var list = document.getElementById('lstAnalyticsDashboards');",
            "    var txt = document.getElementById('txtAnalyticsDashboardSearch');",
            "    var hidden = document.getElementById('hdnAnalyticsDashboardSelected');",
            "    var values = [];",
            "    if (list) {",
            "        var checks = list.getElementsByTagName('input');",
            "        for (var i = 0; i < checks.length; i++) {",
            "            if (checks[i].checked) {",
            "                var label = document.querySelector('label[for=""' + checks[i].id + '""]');",
            "                values.push(label ? label.innerText : checks[i].value);",
            "            }",
            "        }",
            "    }",
            "    if (values.length == 0 && txt && txt.value.replace(/^\s+|\s+$/g, '') != '') { values.push(txt.value.replace(/^\s+|\s+$/g, '')); }",
            "    if (hidden) { hidden.value = values.join(','); }",
            "    if (values.length == 0) { return false; }",
            "    closeAnalyticsDashboardDialog();",
            "    return true;",
            "}",
            "function wireAnalyticsDashboardDialog() {",
            "    var txt = document.getElementById('txtAnalyticsDashboardSearch');",
            "    var list = document.getElementById('lstAnalyticsDashboards');",
            "    if (txt) {",
            "        txt.onkeyup = filterAnalyticsDashboardList;",
            "        txt.onkeydown = function(e) { if ((e || window.event).keyCode == 13) { filterAnalyticsDashboardList(); return false; } };",
            "    }",
            "    if (list) { list.onclick = analyticsDashboardSelectionChanged; }",
            "}",
            "if (window.Sys && Sys.Application) { Sys.Application.add_load(wireAnalyticsDashboardDialog); }",
            "else if (window.addEventListener) { window.addEventListener('load', wireAnalyticsDashboardDialog); }",
            "else { window.attachEvent('onload', wireAnalyticsDashboardDialog); }"
        })
        page.ClientScript.RegisterClientScriptBlock(page.GetType(), "AnalyticsDashboardTileHelperScript", script, True)
    End Sub

    Private Shared Function FindControlRecursive(root As Control, id As String) As Control
        If root Is Nothing Then Return Nothing
        If String.Equals(root.ID, id, StringComparison.OrdinalIgnoreCase) Then Return root
        For Each child As Control In root.Controls
            Dim found As Control = FindControlRecursive(child, id)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function
End Class
