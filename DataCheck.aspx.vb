Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text
Imports System.Web

Partial Class DataCheck
    Inherits System.Web.UI.Page

    Private Const PreviewRows As Integer = 5

    Private Sub DataCheck_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        EnsureReportTitle()
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()

        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then
            lblHeader.Text = Session("REPTITLE").ToString() & " - Data Quality Dashboard"
            Page.Title = Session("REPTITLE").ToString() & " - Data Quality Dashboard"
        ElseIf Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Data Quality Dashboard"
            Page.Title = Session("REPORTID").ToString() & " - Data Quality Dashboard"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Data%20Quality"
    End Sub

    Private Sub DataCheck_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        BindQualityPreviews()
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub BindQualityPreviews()
        Dim source As DataTable = CurrentSourceTable()
        litQualitySuitability.Text = RenderQualitySuitabilityTable(source)

        litPreviewDataReadiness.Text = BuildReadinessPreviewHtml(source)
        litPreviewDataQuality.Text = BuildQualityPreviewHtml(source)
        litPreviewProfiling.Text = BuildProfilingPreviewHtml(source)
        litPreviewDataDictionary.Text = BuildDictionaryPreviewHtml(source)
        litPreviewDataDrift.Text = BuildDriftPreviewHtml(source)
        litPreviewAnomalyScoring.Text = BuildAnomalyPreviewHtml(source)
        litPreviewOutliers.Text = BuildOutlierPreviewHtml(source)
        litPreviewRuleBasedAlerts.Text = BuildRuleAlertsPreviewHtml(source)
        litPreviewMapReadiness.Text = BuildMapReadinessPreviewHtml(source)
    End Sub

    Private Sub EnsureReportTitle()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then Exit Sub
        If Session("REPORTID") Is Nothing OrElse Session("REPORTID").ToString().Trim() = "" Then Exit Sub

        Try
            Dim reportId As String = Session("REPORTID").ToString().Trim().Replace("'", "''")
            Dim dv As DataView = mRecords("SELECT ReportTtl, ReportName FROM OURReportInfo WHERE ReportID='" & reportId & "'")
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then Exit Sub
            Dim title As String = ""
            If dv.Table.Columns.Contains("ReportTtl") Then title = dv.Table.Rows(0)("ReportTtl").ToString().Trim()
            If title = "" AndAlso dv.Table.Columns.Contains("ReportName") Then title = dv.Table.Rows(0)("ReportName").ToString().Trim()
            If title <> "" Then Session("REPTITLE") = title
        Catch ex As Exception
        End Try
    End Sub

    Private Function CurrentSourceTable() As DataTable
        Dim dvSession As DataView = TryCast(Session("dv3"), DataView)
        If dvSession IsNot Nothing AndAlso dvSession.Table IsNot Nothing AndAlso dvSession.Table.Rows.Count > 0 Then Return dvSession.Table

        Dim ret As String = String.Empty
        If Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then
            Try
                Dim dv As DataView = RetrieveReportData(Session("REPORTID").ToString().Trim(), "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
                If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then Return dv.Table
            Catch ex As Exception
            End Try
        End If

        Return Nothing
    End Function

    Private Function RenderQualitySuitabilityTable(source As DataTable) As String
        Dim items As List(Of QualityItem) = BuildQualityItems(source)
        Dim sb As New StringBuilder()
        sb.Append("<table class=""qualityTable""><tr><th>Quality Page</th><th>Status</th><th>Useful For</th><th>Suggested Fields</th><th>What To Check Next</th></tr>")
        For Each item As QualityItem In items
            Dim statusClass As String = If(item.Status = "Good", "statusGood", If(item.Status = "Partial", "statusPartial", "statusMissing"))
            sb.Append("<tr>")
            sb.Append("<td><a href=""").Append(HttpUtility.HtmlAttributeEncode(item.PageUrl)).Append(""">").Append(HttpUtility.HtmlEncode(item.Name)).Append("</a></td>")
            sb.Append("<td class=""").Append(statusClass).Append(""">").Append(HttpUtility.HtmlEncode(item.Status)).Append("</td>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.UsefulFor)).Append("</td>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.SuggestedFields)).Append("</td>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.NextStep)).Append("</td>")
            sb.Append("</tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Function BuildQualityItems(source As DataTable) As List(Of QualityItem)
        Dim items As New List(Of QualityItem)()
        Dim anyData As Boolean = HasData(source)
        Dim textCols As List(Of DataColumn) = TextColumns(source)
        Dim numericCols As List(Of DataColumn) = NumericColumns(source)
        Dim dateCol As DataColumn = FirstDateColumn(source)
        Dim latCol As DataColumn = CoordinateColumn(source, True)
        Dim lonCol As DataColumn = CoordinateColumn(source, False)

        If Not anyData Then
            items.Add(New QualityItem("Data Readiness Scanner", "DataReadinessScanner.aspx", "Not enough data", "Choose which analytics can run.", "Report data loaded in memory or selected report data.", "Open a report or import data first."))
            items.Add(New QualityItem("Data Quality", "DataQuality.aspx", "Not enough data", "Find missing, duplicate, invalid, and suspicious records.", "Any report fields.", "Open a report or import data first."))
            items.Add(New QualityItem("Data Profiling", "Profiling.aspx", "Not enough data", "Profile every field.", "Any report fields.", "Open a report or import data first."))
            Return items
        End If

        items.Add(New QualityItem("Data Readiness Scanner", "DataReadinessScanner.aspx", "Good", "Ranks useful analytics and fields by readiness score.", FieldSummary(textCols, numericCols, dateCol), "Use it first, then open the highest-scored checks."))
        items.Add(New QualityItem("Data Quality", "DataQuality.aspx", "Good", "Checks missing values, duplicate rows, invalid dates, ranges, categories, and text values.", "All fields; date fields; numeric fields; category fields.", "Review affected record links before deeper analytics."))
        items.Add(New QualityItem("Data Profiling", "Profiling.aspx", "Good", "Summarizes count, blanks, distinct values, min/max, average, and stdev.", "Every field in the report or imported data.", "Use profiling to understand field reliability."))
        items.Add(New QualityItem("Data Dictionary", "DataDictionary.aspx", "Good", "Documents field type, meaning, blanks, distinct count, and usage suggestions.", "Every field; especially unclear names and business keys.", "Use before sharing dashboards with other users."))
        items.Add(New QualityItem("Data Drift Analysis", "DataDrift.aspx", If(textCols.Count >= 2 OrElse dateCol IsNot Nothing, "Good", "Partial"), "Compares distribution changes across periods or groups.", If(dateCol Is Nothing, "Two category/group fields.", "Date field: " & dateCol.ColumnName & "; optional category fields."), "Open when values or categories may have changed over time."))
        items.Add(New QualityItem("Anomaly Scoring", "AnomalyScoring.aspx", If(textCols.Count > 0 AndAlso numericCols.Count > 0, "Good", "Partial"), "Scores unusual combinations, group values, movements, and suspicious patterns.", FieldPairText(textCols, numericCols), "Use after profiling and quality checks to find hidden issues."))
        items.Add(New QualityItem("Outlier Flagging", "OutlierFlagging.aspx", If(numericCols.Count > 0, "Good", "Partial"), "Flags unusual numeric values by standard deviation, percent difference, or rules.", NumericSummary(numericCols), "Inspect high-impact outliers and related records."))
        items.Add(New QualityItem("Rule-Based Alerts", "RuleBasedAlerts.aspx", "Good", "Turns thresholds into alerts: missing values, variance, correlations, outliers, map readiness, churn risk.", "Fields used in rules; numeric/date/category/map fields.", "Define business thresholds and review alert record links."))
        items.Add(New QualityItem("Map Readiness", "MapReadines.aspx", If(latCol IsNot Nothing AndAlso lonCol IsNot Nothing, "Good", "Partial"), "Checks coordinate quality, missing coordinates, duplicates, invalid ranges, and KML-ready records.", CoordinateSummary(latCol, lonCol), "Open before Map Report when location fields exist."))
        Return items
    End Function

    Private Function BuildReadinessPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim rows As New List(Of String())()
        rows.Add(New String() {"Data Quality", "High", "All fields"})
        rows.Add(New String() {"Profiling", "High", "All fields"})
        rows.Add(New String() {"Data Dictionary", "High", "Field names/types"})
        rows.Add(New String() {"Outlier Flagging", If(NumericColumns(source).Count > 0, "High", "Partial"), NumericSummary(NumericColumns(source))})
        rows.Add(New String() {"Map Readiness", If(CoordinateColumn(source, True) IsNot Nothing AndAlso CoordinateColumn(source, False) IsNot Nothing, "High", "Partial"), CoordinateSummary(CoordinateColumn(source, True), CoordinateColumn(source, False))})
        Return RenderPreviewTable(New String() {"Analysis", "Readiness", "Suggested Fields"}, rows)
    End Function

    Private Function BuildQualityPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim rows As New List(Of String())()
        For Each col As DataColumn In FirstColumns(source, 4)
            rows.Add(New String() {"Missing", col.ColumnName, BlankCount(source, col).ToString()})
        Next
        rows.Add(New String() {"Duplicate", "Record", DuplicateCount(source).ToString()})
        Return RenderPreviewTable(New String() {"Check", "Field", "Records"}, rows)
    End Function

    Private Function BuildProfilingPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim rows As New List(Of String())()
        For Each col As DataColumn In FirstColumns(source, PreviewRows)
            rows.Add(New String() {col.ColumnName, FieldTypeName(source, col), BlankCount(source, col).ToString(), DistinctCount(source, col).ToString()})
        Next
        Return RenderPreviewTable(New String() {"Field", "Type", "Blanks", "Distinct"}, rows)
    End Function

    Private Function BuildDictionaryPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim rows As New List(Of String())()
        For Each col As DataColumn In FirstColumns(source, PreviewRows)
            rows.Add(New String() {col.ColumnName, FieldTypeName(source, col), SuggestedUse(source, col)})
        Next
        Return RenderPreviewTable(New String() {"Field", "Detected Type", "Suggested Use"}, rows)
    End Function

    Private Function BuildDriftPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim textCols As List(Of DataColumn) = TextColumns(source)
        If textCols.Count = 0 Then Return BuildProfilingPreviewHtml(source)
        Dim rows As New List(Of String())()
        Dim col As DataColumn = textCols(0)
        Dim counts As Dictionary(Of String, Integer) = ValueCounts(source, col)
        Dim added As Integer = 0
        For Each kvp As KeyValuePair(Of String, Integer) In counts
            rows.Add(New String() {col.ColumnName, kvp.Key, kvp.Value.ToString(), "Compare groups/periods"})
            added += 1
            If added >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Field", "Value", "Records", "Drift Check"}, rows)
    End Function

    Private Function BuildAnomalyPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim groupCol As DataColumn = FirstTextColumn(source)
        Dim valueCol As DataColumn = FirstNumericColumn(source)
        If groupCol Is Nothing OrElse valueCol Is Nothing Then Return BuildQualityPreviewHtml(source)

        Dim rows As New List(Of String())()
        Dim totals As Dictionary(Of String, Double) = GroupTotals(source, groupCol, valueCol)
        Dim counts As Dictionary(Of String, Integer) = ValueCounts(source, groupCol)
        For Each kvp As KeyValuePair(Of String, Double) In totals
            Dim cnt As Integer = If(counts.ContainsKey(kvp.Key), counts(kvp.Key), 0)
            rows.Add(New String() {kvp.Key, FormatNumber(kvp.Value), cnt.ToString(), "Review unusual group value"})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {groupCol.ColumnName, valueCol.ColumnName, "Records", "Signal"}, rows)
    End Function

    Private Function BuildOutlierPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim valueCol As DataColumn = FirstNumericColumn(source)
        If valueCol Is Nothing Then Return BuildProfilingPreviewHtml(source)
        Dim values As List(Of Double) = NumericValues(source, valueCol)
        If values.Count = 0 Then Return BuildProfilingPreviewHtml(source)
        values.Sort()
        values.Reverse()
        Dim rows As New List(Of String())()
        For i As Integer = 0 To Math.Min(values.Count, PreviewRows) - 1
            rows.Add(New String() {valueCol.ColumnName, FormatNumber(values(i)), "High value"})
        Next
        Return RenderPreviewTable(New String() {"Field", "Value", "Outlier Check"}, rows)
    End Function

    Private Function BuildRuleAlertsPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim rows As New List(Of String())()
        For Each col As DataColumn In FirstColumns(source, 3)
            Dim blanks As Integer = BlankCount(source, col)
            rows.Add(New String() {"Missing values > 10%", col.ColumnName, blanks.ToString()})
        Next
        Dim numCol As DataColumn = FirstNumericColumn(source)
        If numCol IsNot Nothing Then rows.Add(New String() {"Outliers above threshold", numCol.ColumnName, "Ready"})
        rows.Add(New String() {"Map readiness failed", CoordinateSummary(CoordinateColumn(source, True), CoordinateColumn(source, False)), "Review"})
        Return RenderPreviewTable(New String() {"Alert", "Field(s)", "Actual"}, rows)
    End Function

    Private Function BuildMapReadinessPreviewHtml(source As DataTable) As String
        If Not HasData(source) Then Return EmptyPreview("No report data available.")
        Dim latCol As DataColumn = CoordinateColumn(source, True)
        Dim lonCol As DataColumn = CoordinateColumn(source, False)
        Dim rows As New List(Of String())()
        rows.Add(New String() {"Latitude Field", If(latCol Is Nothing, "Not found", latCol.ColumnName), If(latCol Is Nothing, "Map not ready", "Ready")})
        rows.Add(New String() {"Longitude Field", If(lonCol Is Nothing, "Not found", lonCol.ColumnName), If(lonCol Is Nothing, "Map not ready", "Ready")})
        If latCol IsNot Nothing Then rows.Add(New String() {"Missing Latitude", BlankCount(source, latCol).ToString(), "Review"})
        If lonCol IsNot Nothing Then rows.Add(New String() {"Missing Longitude", BlankCount(source, lonCol).ToString(), "Review"})
        Return RenderPreviewTable(New String() {"Check", "Count/Field", "Notes"}, rows)
    End Function

    Private Function RenderPreviewTable(headers As String(), rows As List(Of String())) As String
        If rows Is Nothing OrElse rows.Count = 0 Then Return EmptyPreview("No preview rows available.")
        Dim sb As New StringBuilder()
        sb.Append("<table class=""previewTable""><tr>")
        For Each header As String In headers
            sb.Append("<th>").Append(HttpUtility.HtmlEncode(header)).Append("</th>")
        Next
        sb.Append("</tr>")
        For Each row As String() In rows
            sb.Append("<tr>")
            For i As Integer = 0 To headers.Length - 1
                Dim value As String = ""
                If row IsNot Nothing AndAlso i < row.Length AndAlso row(i) IsNot Nothing Then value = row(i)
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(value)).Append("</td>")
            Next
            sb.Append("</tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Function EmptyPreview(message As String) As String
        Return "<span class=""previewEmpty"">" & HttpUtility.HtmlEncode(message) & "</span>"
    End Function

    Private Function HasData(source As DataTable) As Boolean
        Return source IsNot Nothing AndAlso source.Rows.Count > 0 AndAlso source.Columns.Count > 0
    End Function

    Private Function FirstColumns(source As DataTable, count As Integer) As List(Of DataColumn)
        Dim cols As New List(Of DataColumn)()
        If source Is Nothing Then Return cols
        For Each col As DataColumn In source.Columns
            cols.Add(col)
            If cols.Count >= count Then Exit For
        Next
        Return cols
    End Function

    Private Function TextColumns(source As DataTable) As List(Of DataColumn)
        Dim cols As New List(Of DataColumn)()
        If source Is Nothing Then Return cols
        For Each col As DataColumn In source.Columns
            If Not IsNumericColumn(source, col) AndAlso Not IsDateColumn(source, col) Then cols.Add(col)
        Next
        Return cols
    End Function

    Private Function NumericColumns(source As DataTable) As List(Of DataColumn)
        Dim cols As New List(Of DataColumn)()
        If source Is Nothing Then Return cols
        For Each col As DataColumn In source.Columns
            If IsNumericColumn(source, col) Then cols.Add(col)
        Next
        Return cols
    End Function

    Private Function FirstTextColumn(source As DataTable) As DataColumn
        Dim cols As List(Of DataColumn) = TextColumns(source)
        If cols.Count = 0 Then Return Nothing
        Return cols(0)
    End Function

    Private Function FirstNumericColumn(source As DataTable) As DataColumn
        Dim cols As List(Of DataColumn) = NumericColumns(source)
        If cols.Count = 0 Then Return Nothing
        Return cols(0)
    End Function

    Private Function FirstDateColumn(source As DataTable) As DataColumn
        If source Is Nothing Then Return Nothing
        For Each col As DataColumn In source.Columns
            If IsDateColumn(source, col) Then Return col
        Next
        Return Nothing
    End Function

    Private Function IsNumericColumn(source As DataTable, col As DataColumn) As Boolean
        If col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal) Then Return True
        Dim checked As Integer = 0
        Dim numeric As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = row(col).ToString().Trim()
            If text = "" Then Continue For
            checked += 1
            Dim d As Double
            If Double.TryParse(text, d) Then numeric += 1
            If checked >= 25 Then Exit For
        Next
        Return checked > 0 AndAlso numeric = checked
    End Function

    Private Function IsDateColumn(source As DataTable, col As DataColumn) As Boolean
        If col.DataType Is GetType(DateTime) Then Return True
        Dim name As String = col.ColumnName.ToLower()
        If Not (name.Contains("date") OrElse name.Contains("time") OrElse name.Contains("period")) Then Return False
        Dim checked As Integer = 0
        Dim dates As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = row(col).ToString().Trim()
            If text = "" Then Continue For
            checked += 1
            Dim d As DateTime
            If DateTime.TryParse(text, d) Then dates += 1
            If checked >= 25 Then Exit For
        Next
        Return checked > 0 AndAlso dates = checked
    End Function

    Private Function FieldTypeName(source As DataTable, col As DataColumn) As String
        If IsNumericColumn(source, col) Then Return "Numeric"
        If IsDateColumn(source, col) Then Return "Date"
        Return "Text/Category"
    End Function

    Private Function SuggestedUse(source As DataTable, col As DataColumn) As String
        If IsNumericColumn(source, col) Then Return "Value, threshold, outlier, score"
        If IsDateColumn(source, col) Then Return "Period, drift, trend, freshness"
        Return "Category, group, dictionary, duplicate review"
    End Function

    Private Function BlankCount(source As DataTable, col As DataColumn) As Integer
        Dim count As Integer = 0
        For Each row As DataRow In source.Rows
            If row.IsNull(col) OrElse row(col).ToString().Trim() = "" Then count += 1
        Next
        Return count
    End Function

    Private Function DistinctCount(source As DataTable, col As DataColumn) As Integer
        Dim values As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim key As String = row(col).ToString().Trim()
            If Not values.ContainsKey(key) Then values.Add(key, True)
        Next
        Return values.Count
    End Function

    Private Function DuplicateCount(source As DataTable) As Integer
        Dim seen As New Dictionary(Of String, Integer)()
        Dim duplicates As Integer = 0
        For Each row As DataRow In source.Rows
            Dim parts As New List(Of String)()
            For Each col As DataColumn In source.Columns
                parts.Add(row(col).ToString())
            Next
            Dim key As String = String.Join("|", parts.ToArray())
            If seen.ContainsKey(key) Then
                seen(key) += 1
                duplicates += 1
            Else
                seen.Add(key, 1)
            End If
        Next
        Return duplicates
    End Function

    Private Function ValueCounts(source As DataTable, col As DataColumn) As Dictionary(Of String, Integer)
        Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim key As String = row(col).ToString().Trim()
            If key = "" Then key = "(blank)"
            If counts.ContainsKey(key) Then counts(key) += 1 Else counts.Add(key, 1)
        Next
        Return counts
    End Function

    Private Function GroupTotals(source As DataTable, groupCol As DataColumn, valueCol As DataColumn) As Dictionary(Of String, Double)
        Dim totals As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim key As String = row(groupCol).ToString().Trim()
            If key = "" Then key = "(blank)"
            Dim value As Double
            Double.TryParse(row(valueCol).ToString(), value)
            If totals.ContainsKey(key) Then totals(key) += value Else totals.Add(key, value)
        Next
        Return totals
    End Function

    Private Function NumericValues(source As DataTable, col As DataColumn) As List(Of Double)
        Dim values As New List(Of Double)()
        For Each row As DataRow In source.Rows
            Dim value As Double
            If Double.TryParse(row(col).ToString(), value) Then values.Add(value)
        Next
        Return values
    End Function

    Private Function CoordinateColumn(source As DataTable, latitude As Boolean) As DataColumn
        If source Is Nothing Then Return Nothing
        For Each col As DataColumn In source.Columns
            Dim name As String = col.ColumnName.ToLower().Replace("_", "").Replace(" ", "")
            If name = "id" OrElse name.EndsWith("id") OrElse name = "indx" OrElse name = "ind" OrElse name = "inx" Then Continue For
            If latitude AndAlso (name.Contains("latitude") OrElse name = "lat") Then Return col
            If Not latitude AndAlso (name.Contains("longitude") OrElse name.Contains("long") OrElse name = "lon" OrElse name = "lng") Then Return col
        Next
        Return Nothing
    End Function

    Private Function FieldSummary(textCols As List(Of DataColumn), numericCols As List(Of DataColumn), dateCol As DataColumn) As String
        Dim parts As New List(Of String)()
        If textCols.Count > 0 Then parts.Add("Category: " & textCols(0).ColumnName)
        If numericCols.Count > 0 Then parts.Add("Numeric: " & numericCols(0).ColumnName)
        If dateCol IsNot Nothing Then parts.Add("Date: " & dateCol.ColumnName)
        If parts.Count = 0 Then Return "All fields"
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Function FieldPairText(textCols As List(Of DataColumn), numericCols As List(Of DataColumn)) As String
        Dim groupName As String = If(textCols.Count > 0, textCols(0).ColumnName, "category/group field")
        Dim valueName As String = If(numericCols.Count > 0, numericCols(0).ColumnName, "numeric value field")
        Return "Group: " & groupName & "; Value: " & valueName
    End Function

    Private Function NumericSummary(numericCols As List(Of DataColumn)) As String
        If numericCols Is Nothing OrElse numericCols.Count = 0 Then Return "Numeric value fields are recommended."
        Dim names As New List(Of String)()
        For Each col As DataColumn In numericCols
            names.Add(col.ColumnName)
            If names.Count >= 3 Then Exit For
        Next
        Return String.Join(", ", names.ToArray())
    End Function

    Private Function CoordinateSummary(latCol As DataColumn, lonCol As DataColumn) As String
        Dim latText As String = If(latCol Is Nothing, "latitude field not found", "latitude: " & latCol.ColumnName)
        Dim lonText As String = If(lonCol Is Nothing, "longitude field not found", "longitude: " & lonCol.ColumnName)
        Return latText & "; " & lonText
    End Function

    Private Function FormatNumber(value As Double) As String
        Return value.ToString("0.####")
    End Function

    Private Class QualityItem
        Public Property Name As String
        Public Property PageUrl As String
        Public Property Status As String
        Public Property UsefulFor As String
        Public Property SuggestedFields As String
        Public Property NextStep As String

        Public Sub New(name As String, pageUrl As String, status As String, usefulFor As String, suggestedFields As String, nextStep As String)
            Me.Name = name
            Me.PageUrl = pageUrl
            Me.Status = status
            Me.UsefulFor = usefulFor
            Me.SuggestedFields = suggestedFields
            Me.NextStep = nextStep
        End Sub
    End Class
End Class
