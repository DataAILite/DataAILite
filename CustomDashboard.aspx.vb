Imports System
Imports System.Data
Imports System.Text
Imports System.Web

Partial Class CustomDashboard
    Inherits System.Web.UI.Page

    Private dashboardName As String = String.Empty
    Private dashboardPageNumber As Integer = 1
    Private dashboardPageCount As Integer = 1
    Private dashboardTotalTiles As Integer = 0
    Private dashboardPageSize As Integer = 12
    Private currentReportId As String = String.Empty

    Private Sub CustomDashboard_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub

    Private Sub CustomDashboard_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        dashboardName = RequestedDashboardName()
        currentReportId = RequestedReportId()
        ApplyListOfReportsMenuWhenNoReport()
        If Not IsPostBack Then SetDefaultExportNotes()
        PrepareDashboardExportManifest("")
        If dashboardName = "" Then
            LabelMessage.Text = "No custom dashboard was selected."
            LiteralTiles.Text = ""
            LiteralDashboardExplanation.Text = ""
            SetDashboardPagingControls()
            Exit Sub
        End If

        lblHeader.Text = DashboardHeaderText()
        Page.Title = lblHeader.Text
        HandleDashboardTileActions()
        HyperLinkListOfDashboards.NavigateUrl = DashboardListUrl()

        Dim ret As String = String.Empty
        Dim sql As String = "SELECT Indx, Dashboard, GraphTitle, ARR, ReportID, Comments FROM ourdashboards WHERE UserID='" & SqlText(Session("logon")) & "' AND Dashboard='" & SqlText(dashboardName) & "' AND UPPER(ChartType)='ANALYTICS'" & ReportSqlFilter() & " ORDER BY Indx"
        Dim dv As DataView = mRecords(sql, ret)
        If ret.Trim() <> "" Then
            LabelMessage.Text = ret
            LiteralTiles.Text = ""
            LiteralDashboardExplanation.Text = ""
            SetDashboardPagingControls()
            Exit Sub
        End If

        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            LabelMessage.Text = "This custom analytics dashboard does not have saved tiles."
            LiteralTiles.Text = ""
            LiteralDashboardExplanation.Text = ""
            SetDashboardPagingControls()
            Exit Sub
        End If

        dashboardTotalTiles = dv.Table.Rows.Count
        dashboardPageSize = RequestedPageSize()
        dashboardPageNumber = RequestedDashboardPage()
        dashboardPageCount = CInt(Math.Ceiling(dashboardTotalTiles / CDbl(dashboardPageSize)))
        If dashboardPageCount < 1 Then dashboardPageCount = 1
        If dashboardPageNumber > dashboardPageCount Then dashboardPageNumber = dashboardPageCount
        If dashboardPageNumber < 1 Then dashboardPageNumber = 1

        LiteralDashboardExplanation.Text = BuildDashboardExplanationHtml(dv.Table)
        LiteralTiles.Text = BuildTilesHtml(dv.Table)
        SetDashboardPagingControls()
    End Sub

    Private Function DashboardHeaderText() As String
        Dim reportId As String = currentReportId.Trim()
        If reportId = "" AndAlso Session("REPORTID") IsNot Nothing Then reportId = Session("REPORTID").ToString().Trim()
        If reportId = "" Then Return "Dashboard across all reports - " & dashboardName
        Return "Dashboard for report " & DashboardReportTitleText(reportId) & " - " & dashboardName
    End Function

    Private Function RequestedDashboardName() As String
        If Request("dashboard") Is Nothing Then Return String.Empty
        Return Request("dashboard").ToString().Trim()
    End Function

    Private Function RequestedReportId() As String
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Return Request("Report").ToString().Trim()
        If Request("REPORT") IsNot Nothing AndAlso Request("REPORT").ToString().Trim() <> "" Then Return Request("REPORT").ToString().Trim()
        If Request("ReportID") IsNot Nothing AndAlso Request("ReportID").ToString().Trim() <> "" Then Return Request("ReportID").ToString().Trim()
        If Request("REPORTID") IsNot Nothing AndAlso Request("REPORTID").ToString().Trim() <> "" Then Return Request("REPORTID").ToString().Trim()
        If Request("repid") IsNot Nothing AndAlso Request("repid").ToString().Trim() <> "" Then Return Request("repid").ToString().Trim()
        Return String.Empty
    End Function

    Private Function RequestedDashboardPage() As Integer
        Dim requestedPage As Integer = 1
        If Request("page") IsNot Nothing AndAlso Integer.TryParse(Request("page").ToString(), requestedPage) Then
            Return Math.Max(1, requestedPage)
        End If
        Return 1
    End Function

    Private Function RequestedPageSize() As Integer
        Dim requestedSize As Integer = 12
        If Request("ps") IsNot Nothing AndAlso Integer.TryParse(Request("ps").ToString(), requestedSize) Then
            Return Math.Max(4, Math.Min(36, requestedSize))
        End If
        If HiddenDashboardPageSize.Value IsNot Nothing AndAlso Integer.TryParse(HiddenDashboardPageSize.Value, requestedSize) Then
            Return Math.Max(4, Math.Min(36, requestedSize))
        End If
        Return 12
    End Function

    Private Function BuildTilesHtml(table As DataTable) As String
        Dim sb As New StringBuilder()
        Dim startIndex As Integer = (dashboardPageNumber - 1) * dashboardPageSize
        Dim endIndex As Integer = Math.Min(startIndex + dashboardPageSize, table.Rows.Count)

        For rowIndex As Integer = startIndex To endIndex - 1
            Dim row As DataRow = table.Rows(rowIndex)
            Dim rawUrl As String = DecodeArr(FieldText(row("ARR")))
            Dim reportId As String = TileReportId(row, rawUrl)
            rawUrl = EnsureReportParameter(rawUrl, reportId)
            If rawUrl.Trim() = "" Then Continue For

            Dim title As String = FieldText(row("GraphTitle")).Trim()
            rawUrl = EnsureReportViewMode(rawUrl, title)
            If IsGenericTileTitle(title) Then title = ActionTitleFromUrl(rawUrl)
            If title = "" Then title = PageTitleFromUrl(rawUrl)
            title = TileTitleWithReport(title, reportId)
            Dim pageName As String = PageTitleFromUrl(rawUrl)
            Dim tileText As String = pageName
            If reportId <> "" Then tileText &= " - report " & reportId
            Dim previewHtml As String = BuildPreviewHtml(rawUrl, reportId)

            sb.Append("<div class=""analyticsTile"" title=""Open ")
            sb.Append(Server.HtmlEncode(title))
            sb.Append(""">")
            sb.Append("<span class=""tileCaption""><span class=""tileTitle"">")
            sb.Append(Server.HtmlEncode(title))
            sb.Append("</span><span class=""tileText"">")
            sb.Append(Server.HtmlEncode(tileText))
            sb.Append("</span></span>")
            sb.Append("<span class=""tileBody"">Open this saved analytics view with the selected fields, filters, and options.</span>")
            sb.Append("<span class=""previewBox"">")
            sb.Append(previewHtml)
            sb.Append("</span><span class=""openText""><a href=""")
            sb.Append(Server.HtmlEncode(ResolveTileUrl(rawUrl)))
            sb.Append(""" title=""Open ")
            sb.Append(Server.HtmlEncode(title))
            sb.Append(""">open</a>")
            sb.Append("&nbsp;&nbsp;&nbsp;&nbsp;<a href=""")
            sb.Append(Server.HtmlEncode(BuildCustomDashboardActionUrl("delindx", FieldText(row("Indx")).Trim())))
            sb.Append(""" title=""Delete this tile from the dashboard"" onclick=""return confirm('Delete this tile from the dashboard?');"">delete from dashboard</a></span></div>")
        Next

        Return sb.ToString()
    End Function

    Private Function BuildDashboardExplanationHtml(table As DataTable) As String
        If table Is Nothing OrElse table.Rows.Count = 0 Then Return String.Empty

        Dim sb As New StringBuilder()
        sb.Append("<div style=""font-family:Arial;font-size:12px;font-weight:bold;color:#333333;margin-bottom:4px;"">Dashboard rows: ")
        sb.Append(table.Rows.Count.ToString())
        sb.Append("</div>")
        sb.Append("<table class=""suitabilityTable""><tr><th>Dashboard Item</th><th>Report</th><th>Saved Page</th><th>What Opens</th></tr>")
        For Each row As DataRow In table.Rows
            Dim rawUrl As String = DecodeArr(FieldText(row("ARR")))
            Dim reportId As String = TileReportId(row, rawUrl)
            rawUrl = EnsureReportParameter(rawUrl, reportId)
            Dim title As String = FieldText(row("GraphTitle")).Trim()
            If IsGenericTileTitle(title) Then title = ActionTitleFromUrl(rawUrl)
            If title = "" Then title = PageTitleFromUrl(rawUrl)
            Dim reportTitle As String = DashboardReportTitleText(reportId)
            Dim pageName As String = PageTitleFromUrl(rawUrl)
            Dim summary As String = SettingSummary(rawUrl)
            If summary.Trim() = "" Then summary = "Saved analytics tile opened with the fields and options stored in this dashboard row."

            sb.Append("<tr><td>")
            sb.Append(Server.HtmlEncode(title))
            sb.Append("</td><td>")
            sb.Append(Server.HtmlEncode(If(reportTitle.Trim() = "", If(reportId.Trim() = "", "All reports", reportId), reportTitle)))
            sb.Append("</td><td>")
            sb.Append(Server.HtmlEncode(pageName))
            sb.Append("</td><td>")
            sb.Append(Server.HtmlEncode(summary))
            sb.Append("</td></tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Sub HandleDashboardTileActions()
        If Request("delindx") IsNot Nothing AndAlso Request("delindx").ToString().Trim() <> "" Then
            DeleteDashboardTile(Request("delindx").ToString().Trim())
            Response.Redirect(CurrentCustomDashboardUrl())
        End If
    End Sub

    Private Sub DeleteDashboardTile(indexValue As String)
        If SqlNumber(indexValue) = "0" AndAlso indexValue.Trim() <> "0" Then Exit Sub
        ExequteSQLquery("DELETE FROM ourdashboards WHERE UserID='" & SqlText(Session("logon")) & "' AND Dashboard='" & SqlText(dashboardName) & "' AND UPPER(ChartType)='ANALYTICS'" & ReportSqlFilter() & " AND Indx=" & SqlNumber(indexValue))
    End Sub

    Private Function CurrentCustomDashboardUrl() As String
        Dim url As String = "CustomDashboard.aspx?dashboard=" & Server.UrlEncode(dashboardName)
        If currentReportId.Trim() <> "" Then url &= "&Report=" & Server.UrlEncode(currentReportId)
        Return url
    End Function

    Private Function DashboardListUrl() As String
        If currentReportId.Trim() = "" Then Return ResolveUrl("~/ListOfDashboards.aspx")
        Return ResolveUrl("~/ListOfDashboards.aspx?Report=" & Server.UrlEncode(currentReportId))
    End Function

    Private Function ReportSqlFilter() As String
        If currentReportId.Trim() = "" Then Return String.Empty
        Return " AND ReportID='" & SqlText(currentReportId) & "'"
    End Function

    Private Function TileTitleWithReport(title As String, reportId As String) As String
        Dim reportTitle As String = DashboardReportTitleText(reportId)
        If reportTitle.Trim() = "" Then Return title
        If title.Trim() = "" Then Return reportTitle
        If title.IndexOf(reportTitle, StringComparison.OrdinalIgnoreCase) >= 0 Then Return title
        Return reportTitle & " - " & title
    End Function

    Private Function DashboardReportTitleText(reportId As String) As String
        If reportId Is Nothing OrElse reportId.Trim() = "" Then Return String.Empty
        Dim reportInfo As DataTable = GetReportInfo(reportId.Trim())
        If reportInfo IsNot Nothing AndAlso reportInfo.Rows.Count > 0 Then
            Dim title As String = FieldText(reportInfo.Rows(0)("ReportTtl")).Trim()
            If title <> "" Then Return title
        End If
        Return reportId.Trim()
    End Function

    Private Function TileReportId(row As DataRow, rawUrl As String) As String
        Dim reportId As String = FieldText(row("ReportID")).Trim()
        If reportId <> "" Then Return reportId
        Return ReportIdFromUrl(rawUrl)
    End Function

    Private Function ReportIdFromUrl(rawUrl As String) As String
        If rawUrl Is Nothing Then Return String.Empty
        Dim questionIndex As Integer = rawUrl.IndexOf("?"c)
        If questionIndex < 0 OrElse questionIndex >= rawUrl.Length - 1 Then Return String.Empty
        Dim values As System.Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(rawUrl.Substring(questionIndex + 1))
        Dim reportId As String = values("Report")
        If reportId Is Nothing OrElse reportId.Trim() = "" Then reportId = values("ReportID")
        If reportId Is Nothing Then Return String.Empty
        Return reportId.Trim()
    End Function

    Private Function BuildCustomDashboardActionUrl(actionName As String, indexValue As String) As String
        Return CurrentCustomDashboardUrl() & "&" & actionName & "=" & Server.UrlEncode(indexValue)
    End Function

    Private Function SqlNumber(value As String) As String
        Dim parsed As Integer = 0
        If Integer.TryParse(If(value, "").Trim(), parsed) Then Return parsed.ToString()
        Return "0"
    End Function

    Private Function BuildPreviewHtml(rawUrl As String, reportId As String) As String
        Dim settingsPreview As String = ReportViewsSettingsPreview(rawUrl)
        Dim source As DataTable = CurrentPreviewTable(reportId)
        If source IsNot Nothing AndAlso source.Rows.Count > 0 Then
            If settingsPreview.Trim() <> "" Then Return settingsPreview & RenderPreviewTable(source)
            Return RenderPreviewTable(source)
        End If

        If settingsPreview.Trim() <> "" Then Return settingsPreview

        Dim summary As String = SettingSummary(rawUrl)
        If summary.Trim() = "" Then summary = "Open this tile to view the saved dashboard item."
        Return "<span class=""previewEmpty"">" & Server.HtmlEncode(summary) & "</span>"
    End Function

    Private Function CurrentPreviewTable(reportId As String) As DataTable
        If Session Is Nothing Then Return Nothing
        Dim sessionReportId As String = ""
        If Session("REPORTID") IsNot Nothing Then sessionReportId = Session("REPORTID").ToString().Trim()

        If reportId.Trim() <> "" AndAlso sessionReportId.Equals(reportId.Trim(), StringComparison.OrdinalIgnoreCase) Then
            Dim candidates() As String = {"dv3", "OriginalDataTable", "GridView1DataSource", "dataTable"}
            For Each key As String In candidates
                If Session(key) Is Nothing Then Continue For
                If TypeOf Session(key) Is DataView Then
                    Dim dv As DataView = CType(Session(key), DataView)
                    If dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then Return dv.Table
                ElseIf TypeOf Session(key) Is DataTable Then
                    Dim dt As DataTable = CType(Session(key), DataTable)
                    If dt.Rows.Count > 0 Then Return dt
                End If
            Next
        End If

        If reportId.Trim() <> "" Then
            Dim ret As String = String.Empty
            Dim dvPreview As DataView = RetrieveReportData(reportId.Trim(), "", 1, 20, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If ret.Trim() = "" AndAlso dvPreview IsNot Nothing AndAlso dvPreview.Table IsNot Nothing AndAlso dvPreview.Table.Rows.Count > 0 Then Return dvPreview.Table
        End If
        Return Nothing
    End Function

    Private Function RenderPreviewTable(source As DataTable) As String
        Dim maxRows As Integer = Math.Min(5, source.Rows.Count)
        Dim maxCols As Integer = Math.Min(5, source.Columns.Count)
        If maxRows = 0 OrElse maxCols = 0 Then Return "<span class=""previewEmpty"">No preview rows available.</span>"

        Dim sb As New StringBuilder()
        sb.Append("<table class=""previewTable""><tr>")
        For colIndex As Integer = 0 To maxCols - 1
            sb.Append("<th>")
            sb.Append(Server.HtmlEncode(source.Columns(colIndex).ColumnName))
            sb.Append("</th>")
        Next
        sb.Append("</tr>")

        For rowIndex As Integer = 0 To maxRows - 1
            sb.Append("<tr>")
            For colIndex As Integer = 0 To maxCols - 1
                sb.Append("<td>")
                sb.Append(Server.HtmlEncode(PreviewValue(source.Rows(rowIndex)(colIndex))))
                sb.Append("</td>")
            Next
            sb.Append("</tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Function ReportViewsSettingsPreview(rawUrl As String) As String
        If Not IsReportViewsUrl(rawUrl) Then Return String.Empty

        Dim questionIndex As Integer = rawUrl.IndexOf("?"c)
        If questionIndex < 0 OrElse questionIndex >= rawUrl.Length - 1 Then Return String.Empty

        Dim values As System.Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(rawUrl.Substring(questionIndex + 1))
        Dim rows As New List(Of String())()
        Dim modeText As String = ActionTitleFromUrl(rawUrl)
        If modeText.Trim() <> "" Then rows.Add(New String() {"View", modeText})
        AddPreviewSetting(rows, "Row / Category 1", values("cat1"))
        AddPreviewSetting(rows, "Column / Category 2", values("cat2"))
        AddPreviewSetting(rows, "Value Field", values("y1"))
        AddPreviewSetting(rows, "Aggregation", values("fn"))
        AddPreviewSetting(rows, "Chart Type", values("grtype"))
        If rows.Count = 0 Then Return String.Empty

        Dim sb As New StringBuilder()
        sb.Append("<table class=""previewTable""><tr><th>Setting</th><th>Value</th></tr>")
        For Each row As String() In rows
            sb.Append("<tr><td>")
            sb.Append(Server.HtmlEncode(row(0)))
            sb.Append("</td><td>")
            sb.Append(Server.HtmlEncode(row(1)))
            sb.Append("</td></tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Sub AddPreviewSetting(rows As List(Of String()), label As String, value As String)
        If value Is Nothing OrElse value.Trim() = "" Then Exit Sub
        rows.Add(New String() {label, value.Trim()})
    End Sub

    Private Function PreviewValue(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return ""
        Dim text As String = value.ToString()
        If text.Length > 40 Then text = text.Substring(0, 40)
        Return text
    End Function

    Private Function ResolveTileUrl(rawUrl As String) As String
        rawUrl = rawUrl.Trim()
        If rawUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) OrElse rawUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then Return rawUrl
        If rawUrl.StartsWith("~/", StringComparison.OrdinalIgnoreCase) Then Return ResolveUrl(rawUrl)
        If rawUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase) Then Return rawUrl
        Return ResolveUrl("~/" & rawUrl)
    End Function

    Private Function EnsureReportParameter(rawUrl As String, reportId As String) As String
        If rawUrl Is Nothing Then Return String.Empty
        If reportId Is Nothing OrElse reportId.Trim() = "" Then Return rawUrl
        If rawUrl.IndexOf("Report=", StringComparison.OrdinalIgnoreCase) >= 0 OrElse rawUrl.IndexOf("ReportID=", StringComparison.OrdinalIgnoreCase) >= 0 Then Return rawUrl
        If rawUrl.Contains("?") Then Return rawUrl & "&Report=" & HttpUtility.UrlEncode(reportId.Trim())
        Return rawUrl & "?Report=" & HttpUtility.UrlEncode(reportId.Trim())
    End Function

    Private Function EnsureReportViewMode(rawUrl As String, title As String) As String
        If rawUrl Is Nothing OrElse Not IsReportViewsUrl(rawUrl) Then Return rawUrl
        If rawUrl.IndexOf("grtype=", StringComparison.OrdinalIgnoreCase) >= 0 OrElse rawUrl.IndexOf("det=yes", StringComparison.OrdinalIgnoreCase) >= 0 Then Return rawUrl

        Dim lowerTitle As String = If(title, "").Trim().ToLowerInvariant()
        If lowerTitle.Contains("matrix") OrElse lowerTitle.Contains("pivot") Then
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "graph", "yes")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "grtype", "matrix")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "srd", "11")
        ElseIf lowerTitle.Contains("bar report") Then
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "graph", "yes")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "grtype", "bar")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "srd", "11")
        ElseIf lowerTitle.Contains("pie report") Then
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "graph", "yes")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "grtype", "pie")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "srd", "11")
        ElseIf lowerTitle.Contains("line report") Then
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "graph", "yes")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "grtype", "line")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "srd", "11")
        ElseIf lowerTitle.Contains("drilldown") OrElse lowerTitle.Contains("drill down") OrElse lowerTitle.Contains("detail report") OrElse lowerTitle.Contains("details") Then
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "det", "yes")
            rawUrl = AddOrReplaceQueryParameter(rawUrl, "srd", "11")
        End If

        Return rawUrl
    End Function

    Private Function IsReportViewsUrl(rawUrl As String) As Boolean
        Dim checkUrl As String = rawUrl.Trim()
        If checkUrl.StartsWith("~/", StringComparison.OrdinalIgnoreCase) Then checkUrl = checkUrl.Substring(2)
        If checkUrl.StartsWith("./", StringComparison.OrdinalIgnoreCase) Then checkUrl = checkUrl.Substring(2)
        Return checkUrl.StartsWith("ReportViews.aspx", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function AddOrReplaceQueryParameter(rawUrl As String, key As String, value As String) As String
        Dim questionIndex As Integer = rawUrl.IndexOf("?"c)
        Dim baseUrl As String = rawUrl
        Dim query As String = ""
        If questionIndex >= 0 Then
            baseUrl = rawUrl.Substring(0, questionIndex)
            query = rawUrl.Substring(questionIndex + 1)
        End If

        Dim values As System.Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(query)
        values(key) = value

        Dim sb As New StringBuilder(baseUrl)
        Dim first As Boolean = True
        For Each itemKey As String In values.AllKeys
            If itemKey Is Nothing Then Continue For
            If first Then
                sb.Append("?")
                first = False
            Else
                sb.Append("&")
            End If
            sb.Append(HttpUtility.UrlEncode(itemKey))
            sb.Append("=")
            sb.Append(HttpUtility.UrlEncode(values(itemKey)))
        Next
        Return sb.ToString()
    End Function

    Private Function DecodeArr(value As String) As String
        If value Is Nothing Then Return String.Empty
        Return value.Replace("^^", "'").Replace("**", "[").Replace("##", "]")
    End Function

    Private Function SettingSummary(rawUrl As String) As String
        Dim questionIndex As Integer = rawUrl.IndexOf("?"c)
        If questionIndex < 0 OrElse questionIndex >= rawUrl.Length - 1 Then Return rawUrl

        Dim query As String = rawUrl.Substring(questionIndex + 1)
        Dim values As System.Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(query)
        Dim parts As New List(Of String)()
        For Each key As String In values.AllKeys
            If key Is Nothing Then Continue For
            Dim value As String = values(key)
            If value Is Nothing OrElse value.Trim() = "" Then Continue For
            If key.Equals("Report", StringComparison.OrdinalIgnoreCase) OrElse key.Equals("ReportID", StringComparison.OrdinalIgnoreCase) Then Continue For
            parts.Add(key & ": " & value)
            If parts.Count >= 8 Then Exit For
        Next

        If parts.Count = 0 Then Return rawUrl
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Function PageTitleFromUrl(rawUrl As String) As String
        Dim actionTitle As String = ActionTitleFromUrl(rawUrl)
        If actionTitle.Trim() <> "" Then Return actionTitle

        Dim url As String = rawUrl
        Dim questionIndex As Integer = url.IndexOf("?"c)
        If questionIndex >= 0 Then url = url.Substring(0, questionIndex)
        url = url.Replace("~/", "").Trim("/"c)
        If url = "" Then Return "Analytics"
        Dim fileName As String = url
        Dim slashIndex As Integer = Math.Max(fileName.LastIndexOf("/"c), fileName.LastIndexOf("\"c))
        If slashIndex >= 0 Then fileName = fileName.Substring(slashIndex + 1)
        If fileName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) Then fileName = fileName.Substring(0, fileName.Length - 5)
        Return FriendlyPageName(fileName)
    End Function

    Private Function ActionTitleFromUrl(rawUrl As String) As String
        If rawUrl Is Nothing Then Return ""
        Dim urlOnly As String = rawUrl
        Dim questionIndex As Integer = urlOnly.IndexOf("?"c)
        Dim query As String = ""
        If questionIndex >= 0 Then
            query = urlOnly.Substring(questionIndex + 1)
            urlOnly = urlOnly.Substring(0, questionIndex)
        End If

        Dim values As System.Collections.Specialized.NameValueCollection = HttpUtility.ParseQueryString(query)
        Dim fileName As String = urlOnly.Replace("~/", "").Trim("/"c)
        Dim slashIndex As Integer = Math.Max(fileName.LastIndexOf("/"c), fileName.LastIndexOf("\"c))
        If slashIndex >= 0 Then fileName = fileName.Substring(slashIndex + 1)

        If fileName.Equals("ShowReport.aspx", StringComparison.OrdinalIgnoreCase) Then Return ShowReportActionTitle(values("srd"))
        If fileName.Equals("ReportViews.aspx", StringComparison.OrdinalIgnoreCase) Then
            If FieldText(values("grpstats")).Equals("yes", StringComparison.OrdinalIgnoreCase) Then Return "See Groups Statistics"
            If FieldText(values("gen")).Equals("yes", StringComparison.OrdinalIgnoreCase) Then Return "Show Generic Report"
            If FieldText(values("det")).Equals("yes", StringComparison.OrdinalIgnoreCase) Then Return "DrillDown Groups"
            Select Case FieldText(values("grtype")).Trim().ToLowerInvariant()
                Case "matrix"
                    Return "Matrix / Pivot Report"
                Case "bar"
                    Return "Bar Report"
                Case "pie"
                    Return "Pie Report"
                Case "line"
                    Return "Line Report"
            End Select
            Return ShowReportActionTitle(values("srd"))
        End If

        Return ""
    End Function

    Private Function ShowReportActionTitle(srdValue As String) As String
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

    Private Function IsGenericTileTitle(title As String) As Boolean
        Dim text As String = If(title, "").Trim()
        Return text = "" OrElse text.Equals("Message", StringComparison.OrdinalIgnoreCase) OrElse text.Equals("Online User Reporting", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function FriendlyPageName(value As String) As String
        If value Is Nothing Then Return "Analytics"
        Dim text As String = value.Replace("_", " ").Replace("-", " ")
        Dim sb As New StringBuilder()
        For i As Integer = 0 To text.Length - 1
            Dim ch As Char = text(i)
            If i > 0 AndAlso Char.IsUpper(ch) AndAlso Not Char.IsWhiteSpace(text(i - 1)) Then sb.Append(" ")
            sb.Append(ch)
        Next
        Return sb.ToString().Trim()
    End Function

    Private Sub SetDashboardPagingControls()
        TextBoxPageNumber.Text = dashboardPageNumber.ToString()
        LabelPageCount.Text = " of " & dashboardPageCount.ToString() & " (" & dashboardTotalTiles.ToString() & " tiles)"
        LinkButtonPrevious.Enabled = dashboardPageNumber > 1
        LinkButtonPrevious.Visible = dashboardPageCount > 1 AndAlso dashboardPageNumber > 1
        LinkButtonNext.Enabled = dashboardPageNumber < dashboardPageCount
        LinkButtonNext.Visible = dashboardPageCount > 1 AndAlso dashboardPageNumber < dashboardPageCount
        LabelPageNumberCaption.Visible = dashboardPageCount > 1
        TextBoxPageNumber.Visible = dashboardPageCount > 1
        LabelPageCount.Visible = dashboardPageCount > 1
    End Sub

    Private Function DashboardPageUrl(pageNumber As Integer) As String
        Dim url As String = "CustomDashboard.aspx?dashboard=" & Server.UrlEncode(dashboardName) & "&page=" & pageNumber.ToString() & "&ps=" & dashboardPageSize.ToString()
        If currentReportId.Trim() <> "" Then url &= "&Report=" & Server.UrlEncode(currentReportId)
        Return url
    End Function

    Private Sub ButtonExportZip_Click(sender As Object, e As EventArgs) Handles ButtonExportZip.Click
        PrepareDashboardExportManifest("zip")
        Response.Redirect("ExportPackages.aspx?dashboardexport=1&action=zip")
    End Sub

    Private Sub ButtonExportPdf_Click(sender As Object, e As EventArgs) Handles ButtonExportPdf.Click
        PrepareDashboardExportManifest("pdf")
        Response.Redirect("ExportPackages.aspx?dashboardexport=1&action=pdf")
    End Sub

    Private Sub PrepareDashboardExportManifest(action As String)
        If dashboardName.Trim() = "" Then Exit Sub
        DashboardExportHelper.PrepareExportPackageSession(Me, dashboardName, currentReportId, "analytics", TextBoxExportNotes.Text, action)
    End Sub

    Private Sub SetDefaultExportNotes()
        If TextBoxExportNotes.Text.Trim() <> "" Then Exit Sub
        TextBoxExportNotes.Text = "DataAI custom analytics dashboard export created on " & DateTime.Now.ToString() & vbCrLf &
            "Dashboard: " & dashboardName & vbCrLf &
            "Report: " & If(currentReportId.Trim() = "", "all reports in this dashboard", currentReportId) & vbCrLf &
            "This export includes dashboard notes, file manifest, and available ReportViews PDF files for dashboard report-view tiles."
    End Sub

    Private Sub ApplyListOfReportsMenuWhenNoReport()
        If currentReportId.Trim() <> "" Then Exit Sub
        DashboardMenuHelper.ApplyListOfReportsMenu(TreeView1)
    End Sub

    Private Sub LinkButtonPrevious_Click(sender As Object, e As EventArgs) Handles LinkButtonPrevious.Click
        Response.Redirect(DashboardPageUrl(Math.Max(1, dashboardPageNumber - 1)))
    End Sub

    Private Sub LinkButtonNext_Click(sender As Object, e As EventArgs) Handles LinkButtonNext.Click
        Response.Redirect(DashboardPageUrl(Math.Min(dashboardPageCount, dashboardPageNumber + 1)))
    End Sub

    Private Sub TextBoxPageNumber_TextChanged(sender As Object, e As EventArgs) Handles TextBoxPageNumber.TextChanged
        Dim requestedPage As Integer = dashboardPageNumber
        Dim postedPageText As String = TextBoxPageNumber.Text.Trim()
        If Request.Form(TextBoxPageNumber.UniqueID) IsNot Nothing Then postedPageText = Request.Form(TextBoxPageNumber.UniqueID).Trim()
        If Not Integer.TryParse(postedPageText, requestedPage) Then requestedPage = dashboardPageNumber
        If requestedPage < 1 Then requestedPage = 1
        If requestedPage > dashboardPageCount Then requestedPage = dashboardPageCount
        Response.Redirect(DashboardPageUrl(requestedPage))
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        If node Is Nothing Then Exit Sub
        Dim url As String = node.Value
        If url.Trim() <> "" Then Response.Redirect(url)
    End Sub

    Private Function FieldText(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return String.Empty
        Return value.ToString()
    End Function

    Private Function SqlText(value As Object) As String
        If value Is Nothing Then Return String.Empty
        Return value.ToString().Replace("'", "''")
    End Function
End Class
