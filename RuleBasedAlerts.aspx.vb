Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class RuleBasedAlerts
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50
    Private Const PageSessionPrefix As String = "RuleBasedAlerts_"

    Private Sub RuleBasedAlerts_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Rule-Based Alerts"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Rule%20Based%20Alerts"
    End Sub

    Private Sub RuleBasedAlerts_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            RestoreSelections()
            BuildAndBindAnalysis()
        ElseIf Session("RuleBasedAlertsTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("RuleBasedAlertsTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = ""
        Dim repid As String = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString())
        If repid.Trim() = "" Then
            Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
            If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
                Session("RuleBasedAlertsSource") = existingTable
                Return existingTable
            End If
            LabelError.Text = "Report is not selected."
            Return Nothing
        End If

        Try
            Dim dv As DataView = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If ret.Trim() <> "" Then LabelError.Text = ret
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
                LabelError.Text = "No data. Run or import report data first."
                Session("RuleBasedAlertsSource") = Nothing
                Return Nothing
            End If
            Session("RuleBasedAlertsSource") = dv.Table
            Return dv.Table
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            Return Nothing
        End Try
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("RuleBasedAlertsSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("RuleBasedAlertsSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        SaveSelections()
        GridViewAlerts.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        txtMissingPercent.Text = "10"
        txtVariancePercent.Text = "20"
        txtCorrelationThreshold.Text = "0.8"
        txtOutlierThreshold.Text = "2"
        txtChurnScore.Text = "50"
        chkMapReadiness.Checked = True
        SaveSelections()
        GridViewAlerts.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkAlertsAI_Click(sender As Object, e As EventArgs) Handles lnkAlertsAI.Click
        Dim dt As DataTable = TryCast(Session("RuleBasedAlertsTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("RuleBasedAlertsTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No rule-based alerts to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Rule-Based Alerts", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this rule-based alerts grid. Explain which rules fired, which fields should be reviewed first, which alerts are most severe, and what follow-up analytics should be opened.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As DataTable = OutputTable()
        Session("RuleBasedAlertFilters") = New Dictionary(Of String, String)()

        If source Is Nothing OrElse source.Rows.Count = 0 Then
            BindAnalysisGrid(output)
            Return
        End If

        Dim filteredRows As List(Of DataRow) = RowsAfterSearch(source)
        If filteredRows.Count = 0 Then
            BindAnalysisGrid(output)
            Return
        End If

        AddMissingValueAlerts(source, filteredRows, output, ParseDouble(txtMissingPercent.Text, 10))
        AddVarianceAlerts(source, filteredRows, output, ParseDouble(txtVariancePercent.Text, 20))
        AddCorrelationAlerts(source, filteredRows, output, ParseDouble(txtCorrelationThreshold.Text, 0.8))
        AddOutlierAlerts(source, filteredRows, output, ParseDouble(txtOutlierThreshold.Text, 2))
        If chkMapReadiness.Checked Then AddMapReadinessAlerts(source, filteredRows, output)
        AddChurnScoreAlerts(source, filteredRows, output, ParseDouble(txtChurnScore.Text, 50))

        If output.Rows.Count > 0 Then
            output.DefaultView.Sort = "[Alert Type] ASC, Severity DESC"
            output = output.DefaultView.ToTable()
        End If
        BindAnalysisGrid(output)
    End Sub

    Private Function OutputTable() As DataTable
        Dim output As New DataTable()
        output.Columns.Add("Alert Type", GetType(String))
        output.Columns.Add("Field / Fields", GetType(String))
        output.Columns.Add("Rule", GetType(String))
        output.Columns.Add("Actual Value", GetType(String))
        output.Columns.Add("Status", GetType(String))
        output.Columns.Add("Severity", GetType(Double))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("What To Check Next", GetType(String))
        output.Columns.Add("Details", GetType(String))
        output.Columns.Add("FilterId", GetType(String))
        Return output
    End Function

    Private Sub AddMissingValueAlerts(source As DataTable, rows As List(Of DataRow), output As DataTable, thresholdPercent As Double)
        For Each col As DataColumn In source.Columns
            Dim blanks As Integer = 0
            For Each row As DataRow In rows
                If FieldText(row(col)).Trim() = "" Then blanks += 1
            Next
            Dim pct As Double = blanks * 100.0 / rows.Count
            If pct > thresholdPercent Then
                AddAlert(output, "Missing Values", col.ColumnName, "Missing values > " & FormatNumber(thresholdPercent, 2) & "%", FormatNumber(pct, 2) & "%", "Alert", pct, blanks, "Data Quality; Data Profiling", "Blank or null values exceed the selected missing-value threshold.", RegisterAlertFilter(MissingFilter(col)))
            End If
        Next
    End Sub

    Private Sub AddVarianceAlerts(source As DataTable, rows As List(Of DataRow), output As DataTable, thresholdPercent As Double)
        For Each col As DataColumn In source.Columns
            If Not ColumnTypeIsNumeric(col) Then Continue For
            Dim stats As RunningStats = StatsForColumn(rows, col)
            If stats.Count < 2 Then Continue For
            Dim avgAbs As Double = Math.Abs(stats.Average())
            If avgAbs = 0 Then Continue For
            Dim cv As Double = stats.StdDev() / avgAbs * 100
            If cv > thresholdPercent Then
                AddAlert(output, "Variance", col.ColumnName, "Variance > " & FormatNumber(thresholdPercent, 2) & "%", FormatNumber(cv, 2) & "%", "Alert", cv, stats.Count, "Variance Analysis; Outlier Flagging", "Coefficient of variation is high compared with the selected threshold.", RegisterAlertFilter(AllRowsFilter()))
            End If
        Next
    End Sub

    Private Sub AddCorrelationAlerts(source As DataTable, rows As List(Of DataRow), output As DataTable, threshold As Double)
        Dim numericCols As List(Of DataColumn) = NumericColumns(source)
        For i As Integer = 0 To numericCols.Count - 1
            For j As Integer = i + 1 To numericCols.Count - 1
                Dim corr As Double? = Correlation(rows, numericCols(i), numericCols(j))
                If Not corr.HasValue Then Continue For
                If Math.Abs(corr.Value) > threshold Then
                    AddAlert(output, "Correlation", numericCols(i).ColumnName & " / " & numericCols(j).ColumnName, "Correlation > " & FormatNumber(threshold, 2), FormatNumber(corr.Value, 4), "Alert", Math.Abs(corr.Value) * 100, rows.Count, "Correlation Threshold; Regression Analysis", "Two numeric fields move together strongly. Review whether this relationship is expected or useful for modeling.", RegisterAlertFilter(AllRowsFilter()))
                End If
            Next
        Next
    End Sub

    Private Sub AddOutlierAlerts(source As DataTable, rows As List(Of DataRow), output As DataTable, threshold As Double)
        For Each col As DataColumn In source.Columns
            If Not ColumnTypeIsNumeric(col) Then Continue For
            Dim stats As RunningStats = StatsForColumn(rows, col)
            If stats.Count < 3 OrElse stats.StdDev() = 0 Then Continue For
            Dim countOutliers As Integer = 0
            For Each row As DataRow In rows
                Dim value As Double = NumericValue(row(col))
                If Math.Abs(value - stats.Average()) / stats.StdDev() > threshold Then countOutliers += 1
            Next
            If countOutliers > 0 Then
                AddAlert(output, "Outliers", col.ColumnName, "Outliers above " & FormatNumber(threshold, 2) & " stdev", countOutliers.ToString(), "Alert", countOutliers, countOutliers, "Outlier Flagging; Anomaly Scoring", "Records were found beyond the selected standard-deviation threshold.", RegisterAlertFilter(OutlierFilter(col, stats.Average(), stats.StdDev(), threshold)))
            End If
        Next
    End Sub

    Private Sub AddMapReadinessAlerts(source As DataTable, rows As List(Of DataRow), output As DataTable)
        Dim latCol As DataColumn = FindCoordinateColumn(source, True)
        Dim lonCol As DataColumn = FindCoordinateColumn(source, False)
        If latCol Is Nothing OrElse lonCol Is Nothing Then
            AddAlert(output, "Map Readiness", "Latitude / Longitude", "Map readiness failed", "Coordinate fields not found", "Alert", 100, rows.Count, "Map Readiness; Map Report", "No reliable latitude and longitude field pair was found.", RegisterAlertFilter(AllRowsFilter()))
            Exit Sub
        End If

        Dim invalidCount As Integer = 0
        For Each row As DataRow In rows
            Dim lat As Double
            Dim lon As Double
            If Not Double.TryParse(FieldText(row(latCol)), lat) OrElse Not Double.TryParse(FieldText(row(lonCol)), lon) OrElse lat < -90 OrElse lat > 90 OrElse lon < -180 OrElse lon > 180 Then invalidCount += 1
        Next
        If invalidCount > 0 Then
            Dim filterId As String = RegisterAlertFilter(AllRowsFilter())
            If ColumnTypeIsNumeric(latCol) AndAlso ColumnTypeIsNumeric(lonCol) Then filterId = RegisterAlertFilter(InvalidCoordinateFilter(latCol, lonCol))
            AddAlert(output, "Map Readiness", latCol.ColumnName & " / " & lonCol.ColumnName, "Map readiness failed", invalidCount.ToString() & " invalid or missing", "Alert", invalidCount, invalidCount, "Map Readiness; Map Report", "Coordinate fields were found, but some records are missing values or have invalid coordinate ranges.", filterId)
        End If
    End Sub

    Private Sub AddChurnScoreAlerts(source As DataTable, rows As List(Of DataRow), output As DataTable, threshold As Double)
        For Each col As DataColumn In source.Columns
            If Not ColumnTypeIsNumeric(col) OrElse Not LooksLikeChurnScore(col.ColumnName) Then Continue For
            Dim belowCount As Integer = 0
            For Each row As DataRow In rows
                If NumericValue(row(col)) < threshold Then belowCount += 1
            Next
            If belowCount > 0 Then
                AddAlert(output, "Churn Score", col.ColumnName, "Churn score below " & FormatNumber(threshold, 2), belowCount.ToString(), "Alert", belowCount, belowCount, "Market Churn; Market Risk", "Score-like churn, retention, or risk field has records below the selected threshold.", RegisterAlertFilter(FieldRef(col) & " < " & threshold.ToString(CultureInfo.InvariantCulture)))
            End If
        Next
    End Sub

    Private Sub AddAlert(output As DataTable, alertType As String, fieldsText As String, ruleText As String, actualValue As String, statusText As String, severity As Double, records As Integer, nextText As String, details As String, filterId As String)
        Dim r As DataRow = output.NewRow()
        r("Alert Type") = alertType
        r("Field / Fields") = fieldsText
        r("Rule") = ruleText
        r("Actual Value") = actualValue
        r("Status") = statusText
        r("Severity") = Math.Round(severity, 2)
        r("Records") = records
        r("What To Check Next") = nextText
        r("Details") = details
        r("FilterId") = filterId
        output.Rows.Add(r)
    End Sub

    Private Sub GridViewAlerts_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewAlerts.RowDataBound
        Dim dt As DataTable = TryCast(Session("RuleBasedAlertsTable"), DataTable)
        If dt Is Nothing OrElse e.Row.Cells.Count = 0 Then Exit Sub
        HideColumn(dt, e.Row, "FilterId")
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "alertfilter")
        AddWhatNextLinks(e.Row, dt)
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("RuleBasedAlertsTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewAlerts.AllowPaging = False
            GridViewAlerts.PageIndex = 0
            GridViewAlerts.DataSource = Nothing
            GridViewAlerts.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If
        GridViewAlerts.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewAlerts.PageSize = AnalysisGridPageSize
        If Not GridViewAlerts.AllowPaging Then GridViewAlerts.PageIndex = 0
        GridViewAlerts.DataSource = dt
        GridViewAlerts.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Rule-Based Alerts (" & dt.Rows.Count.ToString() & " alerts)"
        AnalysisExportSnapshot.Save(Me, "RuleBasedAlerts", "Rule-Based Alerts", LabelInfo, GridViewAlerts, dt)
    End Sub

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("RuleBasedAlertsTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("RuleBasedAlertsTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No rule-based alerts to export."
            Exit Sub
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "RuleBasedAlerts_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Rule-Based Alerts", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Rule-Based Alerts", ""))
        End If
        Response.End()
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAlerts.PageIndex > 0 Then GridViewAlerts.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAlerts.PageIndex < (GridViewAlerts.PageCount - 1) Then GridViewAlerts.PageIndex += 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub TextBoxPageNumber_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        Dim requestedPage As Integer
        If Integer.TryParse(TextBoxPageNumber.Text, requestedPage) Then
            If requestedPage < 1 Then requestedPage = 1
            Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(dt.Rows.Count / CDbl(AnalysisGridPageSize))))
            If requestedPage > pageCount Then requestedPage = pageCount
            GridViewAlerts.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewAlerts.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewAlerts.PageIndex < (GridViewAlerts.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewAlerts.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewAlerts.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Function GridTableForAI(dt As DataTable) As DataTable
        If dt Is Nothing Then Return Nothing
        Dim aiTable As DataTable = dt.Copy()
        If aiTable.Columns.Contains("FilterId") Then aiTable.Columns.Remove("FilterId")
        Return aiTable
    End Function

    Private Function BuildAnalysisQuestion(baseQuestion As String) As String
        SetAnalysisExplanationLabels()
        Dim parts As New List(Of String)()
        parts.Add(baseQuestion)
        If LabelAnalysisSubtitle IsNot Nothing AndAlso LabelAnalysisSubtitle.Text.Trim() <> "" Then parts.Add("Input: " & LabelAnalysisSubtitle.Text.Trim())
        Return String.Join(vbCrLf & vbCrLf, parts.ToArray())
    End Function

    Private Function ExplanationBlock(title As String, ParamArray bullets() As String) As String
        Dim html As String = "<div class=""explanationBlock""><div class=""explanationTitle""><strong>" & Server.HtmlEncode(title) & "</strong></div><ul>"
        For Each bullet As String In bullets
            If bullet IsNot Nothing AndAlso bullet.Trim() <> "" Then html &= "<li>" & Server.HtmlEncode(bullet) & "</li>"
        Next
        html &= "</ul></div>"
        Return html
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Missing values % sets the maximum allowed blank/null rate by field.", "Variance % uses numeric coefficient of variation, calculated as standard deviation divided by absolute average.", "Correlation checks every numeric field pair and alerts when absolute correlation is above the selected level.", "Outlier stdev checks numeric fields for records outside the selected standard-deviation distance.", "Map readiness failed checks whether latitude/longitude fields exist and whether coordinates are in valid ranges.", "Churn score below checks score-like numeric fields whose names contain churn, retention, risk, probability, prob, score, or level.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "The page uses transparent user-defined rules rather than a hidden statistical model.", "Each rule creates one alert row only when its selected threshold is exceeded.", "Missing values and outlier alerts can link back to affected records when a reliable filter can be built.", "Correlation alerts compare pairs of numeric fields and show the strongest relationships first by severity.", "Map readiness alerts identify missing coordinate pairs or invalid latitude/longitude ranges.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Alert Type identifies which rule produced the alert.", "Field / Fields shows the affected field or pair of fields.", "Rule and Actual Value show the user-defined threshold and the measured result.", "Severity sorts alerts so the strongest or largest issues appear first.", "Records links open affected rows in Data Explorer where a row filter is available.", "What To Check Next recommends the next analytical page to open for deeper review.")
        ReadinessFooterGuidance.SetFooter(Me, "Rule-Based Alerts", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
    End Sub

    Private Class RunningStats
        Public Count As Integer
        Public Sum As Double
        Public SumSquares As Double
        Public Sub Add(value As Double)
            Count += 1
            Sum += value
            SumSquares += value * value
        End Sub
        Public Function Average() As Double
            If Count = 0 Then Return 0
            Return Sum / Count
        End Function
        Public Function StdDev() As Double
            If Count = 0 Then Return 0
            Dim avg As Double = Average()
            Dim variance As Double = (SumSquares / Count) - (avg * avg)
            If variance < 0 Then variance = 0
            Return Math.Sqrt(variance)
        End Function
    End Class

    Private Function RowsAfterSearch(source As DataTable) As List(Of DataRow)
        Dim rows As New List(Of DataRow)()
        For Each row As DataRow In source.Rows
            If ContainsSearch(row, txtSearch.Text) Then rows.Add(row)
        Next
        Return rows
    End Function

    Private Function StatsForColumn(rows As List(Of DataRow), col As DataColumn) As RunningStats
        Dim stats As New RunningStats()
        For Each row As DataRow In rows
            stats.Add(NumericValue(row(col)))
        Next
        Return stats
    End Function

    Private Function NumericColumns(source As DataTable) As List(Of DataColumn)
        Dim cols As New List(Of DataColumn)()
        For Each col As DataColumn In source.Columns
            If ColumnTypeIsNumeric(col) Then cols.Add(col)
        Next
        Return cols
    End Function

    Private Function Correlation(rows As List(Of DataRow), xCol As DataColumn, yCol As DataColumn) As Double?
        Dim n As Integer = 0
        Dim sumX As Double = 0
        Dim sumY As Double = 0
        Dim sumXX As Double = 0
        Dim sumYY As Double = 0
        Dim sumXY As Double = 0
        For Each row As DataRow In rows
            Dim x As Double = NumericValue(row(xCol))
            Dim y As Double = NumericValue(row(yCol))
            n += 1
            sumX += x
            sumY += y
            sumXX += x * x
            sumYY += y * y
            sumXY += x * y
        Next
        If n < 3 Then Return Nothing
        Dim numerator As Double = n * sumXY - sumX * sumY
        Dim denominator As Double = Math.Sqrt((n * sumXX - sumX * sumX) * (n * sumYY - sumY * sumY))
        If denominator = 0 Then Return Nothing
        Return numerator / denominator
    End Function

    Private Sub HideColumn(dt As DataTable, row As GridViewRow, columnName As String)
        If dt.Columns.Contains(columnName) Then
            Dim idx As Integer = dt.Columns.IndexOf(columnName)
            If idx >= 0 AndAlso idx < row.Cells.Count Then row.Cells(idx).Visible = False
        End If
    End Sub

    Private Sub AddRecordLink(row As GridViewRow, dt As DataTable, recordsColumn As String, filterColumn As String, paramName As String)
        If Not dt.Columns.Contains(recordsColumn) OrElse Not dt.Columns.Contains(filterColumn) Then Exit Sub
        Dim recordsIndex As Integer = dt.Columns.IndexOf(recordsColumn)
        Dim filterIndex As Integer = dt.Columns.IndexOf(filterColumn)
        If recordsIndex < 0 OrElse filterIndex < 0 OrElse recordsIndex >= row.Cells.Count OrElse filterIndex >= row.Cells.Count Then Exit Sub
        Dim recordsText As String = row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
        Dim filterId As String = row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
        If filterId.Trim() = "" Then Exit Sub
        Dim link As New HyperLink()
        link.Text = recordsText
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&" & paramName & "=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding records in Data Explorer."
        row.Cells(recordsIndex).Controls.Clear()
        row.Cells(recordsIndex).Controls.Add(link)
    End Sub

    Private Sub AddWhatNextLinks(row As GridViewRow, dt As DataTable)
        If Not dt.Columns.Contains("Alert Type") OrElse Not dt.Columns.Contains("Field / Fields") OrElse Not dt.Columns.Contains("What To Check Next") Then Exit Sub
        Dim alertIndex As Integer = dt.Columns.IndexOf("Alert Type")
        Dim fieldsIndex As Integer = dt.Columns.IndexOf("Field / Fields")
        Dim nextIndex As Integer = dt.Columns.IndexOf("What To Check Next")
        If alertIndex < 0 OrElse fieldsIndex < 0 OrElse nextIndex < 0 OrElse alertIndex >= row.Cells.Count OrElse fieldsIndex >= row.Cells.Count OrElse nextIndex >= row.Cells.Count Then Exit Sub

        Dim alertType As String = CellText(row.Cells(alertIndex))
        Dim fieldsText As String = CellText(row.Cells(fieldsIndex))
        Dim links As List(Of KeyValuePair(Of String, String)) = NextLinksForAlert(alertType, fieldsText)
        If links.Count = 0 Then Exit Sub

        row.Cells(nextIndex).Controls.Clear()
        For i As Integer = 0 To links.Count - 1
            If i > 0 Then row.Cells(nextIndex).Controls.Add(New LiteralControl(", "))
            Dim link As New HyperLink()
            link.Text = links(i).Key
            link.NavigateUrl = links(i).Value
            link.CssClass = "NodeStyle"
            link.ToolTip = "Open " & links(i).Key & " for " & fieldsText & "."
            row.Cells(nextIndex).Controls.Add(link)
        Next
    End Sub

    Private Function NextLinksForAlert(alertType As String, fieldsText As String) As List(Of KeyValuePair(Of String, String))
        Dim links As New List(Of KeyValuePair(Of String, String))()
        Select Case alertType
            Case "Missing Values"
                AddNextLink(links, "Data Quality", "DataQuality.aspx", fieldsText)
                AddNextLink(links, "Data Profiling", "Profiling.aspx", fieldsText)
            Case "Variance"
                AddNextLink(links, "Variance Analysis", "Variance.aspx", fieldsText)
                AddNextLink(links, "Outlier Flagging", "OutlierFlagging.aspx", fieldsText)
            Case "Correlation"
                AddNextLink(links, "Correlation Threshold", "CorrelationThreshold.aspx", fieldsText)
                AddNextLink(links, "Regression Analysis", "Regression.aspx", fieldsText)
            Case "Outliers"
                AddNextLink(links, "Outlier Flagging", "OutlierFlagging.aspx", fieldsText)
                AddNextLink(links, "Anomaly Scoring", "AnomalyScoring.aspx", fieldsText)
            Case "Map Readiness"
                AddNextLink(links, "Map Readiness", "MapReadines.aspx", fieldsText)
                AddNextLink(links, "Map Report", "MapReport.aspx", fieldsText)
            Case "Churn Score"
                AddNextLink(links, "Market Churn", "MarketChurn.aspx", fieldsText)
                AddNextLink(links, "Market Risk", "MarketRisk.aspx", fieldsText)
        End Select
        Return links
    End Function

    Private Sub AddNextLink(links As List(Of KeyValuePair(Of String, String)), caption As String, pageUrl As String, fieldsText As String)
        Dim separator As String = If(pageUrl.Contains("?"), "&", "?")
        Dim url As String = pageUrl & separator & "from=RuleBasedAlerts&alertfield=" & Server.UrlEncode(fieldsText)
        links.Add(New KeyValuePair(Of String, String)(caption, url))
    End Sub

    Private Function CellText(cell As TableCell) As String
        Return Server.HtmlDecode(cell.Text.Replace("&nbsp;", "")).Trim()
    End Function

    Private Function RegisterAlertFilter(filterExpression As String) As String
        If filterExpression.Trim() = "" Then Return ""
        Dim filters As Dictionary(Of String, String) = TryCast(Session("RuleBasedAlertFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("RuleBasedAlertFilters") = filters
        Return filterId
    End Function

    Private Function AllRowsFilter() As String
        Return "1 = 1"
    End Function

    Private Function MissingFilter(col As DataColumn) As String
        If ColumnTypeIsNumeric(col) OrElse col.DataType Is GetType(DateTime) Then Return FieldRef(col) & " IS NULL"
        Return "(" & FieldRef(col) & " IS NULL OR " & FieldRef(col) & " = '')"
    End Function

    Private Function OutlierFilter(col As DataColumn, avg As Double, stdev As Double, threshold As Double) As String
        Dim lower As Double = avg - threshold * stdev
        Dim upper As Double = avg + threshold * stdev
        Return "(" & FieldRef(col) & " < " & lower.ToString(CultureInfo.InvariantCulture) & " OR " & FieldRef(col) & " > " & upper.ToString(CultureInfo.InvariantCulture) & ")"
    End Function

    Private Function InvalidCoordinateFilter(latCol As DataColumn, lonCol As DataColumn) As String
        Return "(" & FieldRef(latCol) & " IS NULL OR " & FieldRef(lonCol) & " IS NULL OR " & FieldRef(latCol) & " < -90 OR " & FieldRef(latCol) & " > 90 OR " & FieldRef(lonCol) & " < -180 OR " & FieldRef(lonCol) & " > 180)"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "]]") & "]"
    End Function

    Private Function ContainsSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        Dim needle As String = searchText.ToLowerInvariant()
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).ToLowerInvariant().Contains(needle) Then Return True
        Next
        Return False
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToString("yyyy-MM-dd")
        Return valueObject.ToString()
    End Function

    Private Function NumericValue(valueObject As Object) As Double
        Dim number As Double = 0
        Double.TryParse(FieldText(valueObject), NumberStyles.Any, CultureInfo.InvariantCulture, number)
        If number = 0 Then Double.TryParse(FieldText(valueObject), number)
        Return number
    End Function

    Private Function ParseDouble(valueText As String, defaultValue As Double) As Double
        Dim value As Double
        If Double.TryParse(valueText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, value) Then Return value
        If Double.TryParse(valueText.Trim(), value) Then Return value
        Return defaultValue
    End Function

    Private Function ColumnTypeIsNumeric(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function FindCoordinateColumn(source As DataTable, latitude As Boolean) As DataColumn
        For Each col As DataColumn In source.Columns
            Dim n As String = col.ColumnName.ToLowerInvariant()
            If latitude AndAlso (n = "lat" OrElse n.Contains("latitude")) Then Return col
            If Not latitude AndAlso (n = "lon" OrElse n = "lng" OrElse n.Contains("longitude")) Then Return col
        Next
        Return Nothing
    End Function

    Private Function LooksLikeChurnScore(columnName As String) As Boolean
        Dim n As String = columnName.ToLowerInvariant()
        Return n.Contains("churn") OrElse n.Contains("retention") OrElse n.Contains("risk") OrElse n.Contains("probability") OrElse n.Contains("prob") OrElse n.Contains("score") OrElse n.Contains("level")
    End Function

    Private Sub SaveSelections()
        Dim key As String = PageSessionPrefix & ReportKey()
        Session(key & "_Missing") = txtMissingPercent.Text
        Session(key & "_Variance") = txtVariancePercent.Text
        Session(key & "_Correlation") = txtCorrelationThreshold.Text
        Session(key & "_Outlier") = txtOutlierThreshold.Text
        Session(key & "_Churn") = txtChurnScore.Text
        Session(key & "_Map") = chkMapReadiness.Checked
        Session(key & "_Search") = txtSearch.Text
    End Sub

    Private Sub RestoreSelections()
        Dim key As String = PageSessionPrefix & ReportKey()
        If Session(key & "_Missing") IsNot Nothing Then txtMissingPercent.Text = Session(key & "_Missing").ToString()
        If Session(key & "_Variance") IsNot Nothing Then txtVariancePercent.Text = Session(key & "_Variance").ToString()
        If Session(key & "_Correlation") IsNot Nothing Then txtCorrelationThreshold.Text = Session(key & "_Correlation").ToString()
        If Session(key & "_Outlier") IsNot Nothing Then txtOutlierThreshold.Text = Session(key & "_Outlier").ToString()
        If Session(key & "_Churn") IsNot Nothing Then txtChurnScore.Text = Session(key & "_Churn").ToString()
        If Session(key & "_Search") IsNot Nothing Then txtSearch.Text = Session(key & "_Search").ToString()
        If Session(key & "_Map") IsNot Nothing Then
            Dim mapChecked As Boolean = chkMapReadiness.Checked
            If Boolean.TryParse(Session(key & "_Map").ToString(), mapChecked) Then chkMapReadiness.Checked = mapChecked
        End If
    End Sub

    Private Function ReportKey() As String
        If Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then Return Session("REPORTID").ToString().Trim()
        Return "CurrentData"
    End Function
End Class
