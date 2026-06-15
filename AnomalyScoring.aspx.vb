Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class AnomalyScoring
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub AnomalyScoring_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Anomaly Scoring"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Anomaly%20Scoring"
    End Sub

    Private Sub AnomalyScoring_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindAnalysis()
        ElseIf Session("AnomalyScoringTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("AnomalyScoringTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = ""
        Dim repid As String = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString())
        If repid.Trim() = "" Then
            Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
            If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
                Session("AnomalyScoringSource") = existingTable
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
                Session("AnomalyScoringSource") = Nothing
                Return Nothing
            End If
            Session("AnomalyScoringSource") = dv.Table
            Return dv.Table
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            Return Nothing
        End Try
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("AnomalyScoringSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("AnomalyScoringSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing Then Exit Sub
        DropDownGroupField.Items.Clear()
        DropDownCategoryField.Items.Clear()
        DropDownValueField.Items.Clear()
        DropDownDateField.Items.Clear()

        DropDownCategoryField.Items.Add(New ListItem("(None)", ""))
        DropDownDateField.Items.Add(New ListItem("(None)", ""))
        For Each col As DataColumn In source.Columns
            DropDownGroupField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If Not LooksLikeTechnicalId(col.ColumnName) Then DropDownCategoryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If ColumnTypeIsNumeric(col) Then DropDownValueField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If LooksLikeDate(source, col) Then DropDownDateField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
        Next

        SelectFirstNonId(DropDownGroupField)
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewAnomaly.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        txtScoreThreshold.Text = "2"
        GridViewAnomaly.PageIndex = 0
        FillFieldLists()
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkAnomalyAI_Click(sender As Object, e As EventArgs) Handles lnkAnomalyAI.Click
        Dim dt As DataTable = TryCast(Session("AnomalyScoringTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("AnomalyScoringTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No anomaly scoring results to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Anomaly Scoring", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this anomaly scoring grid. Explain unusual combinations, unusual values inside groups, unusual period movement, suspicious category/value patterns, severity scores, and what should be reviewed first.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As DataTable = OutputTable()
        Session("AnomalyScoringFilters") = New Dictionary(Of String, String)()
        If source Is Nothing OrElse source.Rows.Count = 0 OrElse DropDownGroupField.SelectedValue = "" Then
            BindAnalysisGrid(output)
            Return
        End If

        Dim threshold As Double = 2
        Double.TryParse(txtScoreThreshold.Text.Trim(), threshold)
        If threshold < 0 Then threshold = 0

        AddCombinationAnomalies(source, output, threshold)
        AddValueAnomalies(source, output, threshold)
        AddPeriodMovementAnomalies(source, output, threshold)
        AddCategoryValuePatternAnomalies(source, output, threshold)

        If output.Rows.Count > 0 Then
            output.DefaultView.Sort = "Score DESC"
            output = output.DefaultView.ToTable()
        End If
        BindAnalysisGrid(output)
    End Sub

    Private Function OutputTable() As DataTable
        Dim output As New DataTable()
        output.Columns.Add("Anomaly Type", GetType(String))
        output.Columns.Add("Group", GetType(String))
        output.Columns.Add("Category Combination", GetType(String))
        output.Columns.Add("Period", GetType(String))
        output.Columns.Add("Value", GetType(String))
        output.Columns.Add("Expected / Average", GetType(String))
        output.Columns.Add("Difference", GetType(String))
        output.Columns.Add("Score", GetType(Double))
        output.Columns.Add("Note", GetType(String))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("FilterId", GetType(String))
        Return output
    End Function

    Private Sub AddCombinationAnomalies(source As DataTable, output As DataTable, threshold As Double)
        If DropDownCategoryField.SelectedValue = "" Then Exit Sub
        Dim groupCol As String = DropDownGroupField.SelectedValue
        Dim categoryCol As String = DropDownCategoryField.SelectedValue
        Dim groupCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim categoryCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim comboCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim total As Integer = 0

        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim groupText As String = DisplayValue(row(groupCol))
            Dim categoryText As String = DisplayValue(row(categoryCol))
            Dim comboKey As String = groupText & "||" & categoryText
            AddCount(groupCounts, groupText)
            AddCount(categoryCounts, categoryText)
            AddCount(comboCounts, comboKey)
            total += 1
        Next

        If total = 0 Then Exit Sub
        For Each pair As KeyValuePair(Of String, Integer) In comboCounts
            Dim parts() As String = pair.Key.Split(New String() {"||"}, StringSplitOptions.None)
            If parts.Length < 2 Then Continue For
            Dim expected As Double = groupCounts(parts(0)) * categoryCounts(parts(1)) / CDbl(total)
            If expected <= 0 Then Continue For
            Dim diff As Double = pair.Value - expected
            Dim score As Double = Math.Abs(diff) / Math.Sqrt(expected)
            If score >= threshold Then
                AddOutputRow(output, "Unusual Combination", parts(0), parts(0) & " | " & parts(1), "", pair.Value.ToString(), FormatNumber(expected, 2), FormatNumber(diff, 2), score, "Actual combination count is unusual compared with expected group/category frequency.", pair.Value, RegisterAnalysisFilter(FilterAnd(FilterEquals(groupCol, parts(0)), FilterEquals(categoryCol, parts(1)))))
            End If
        Next
    End Sub

    Private Sub AddValueAnomalies(source As DataTable, output As DataTable, threshold As Double)
        If DropDownValueField.SelectedValue = "" Then Exit Sub
        Dim groupCol As String = DropDownGroupField.SelectedValue
        Dim valueCol As String = DropDownValueField.SelectedValue
        Dim stats As New Dictionary(Of String, RunningStats)(StringComparer.OrdinalIgnoreCase)

        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim groupText As String = DisplayValue(row(groupCol))
            If Not stats.ContainsKey(groupText) Then stats(groupText) = New RunningStats()
            stats(groupText).Add(NumericValue(row(valueCol)))
        Next

        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim groupText As String = DisplayValue(row(groupCol))
            If Not stats.ContainsKey(groupText) OrElse stats(groupText).StdDev() = 0 OrElse stats(groupText).Count < 3 Then Continue For
            Dim value As Double = NumericValue(row(valueCol))
            Dim avg As Double = stats(groupText).Average()
            Dim diff As Double = value - avg
            Dim score As Double = Math.Abs(diff) / stats(groupText).StdDev()
            If score >= threshold Then
                AddOutputRow(output, "Unusual Value In Group", groupText, valueCol, "", FormatNumber(value, 2), FormatNumber(avg, 2), FormatNumber(diff, 2), score, "Value is unusual compared with other records in the same group.", 1, RegisterAnalysisFilter(SingleRowFilter(row)))
            End If
        Next
    End Sub

    Private Sub AddPeriodMovementAnomalies(source As DataTable, output As DataTable, threshold As Double)
        If DropDownDateField.SelectedValue = "" OrElse DropDownValueField.SelectedValue = "" Then Exit Sub
        Dim groupCol As String = DropDownGroupField.SelectedValue
        Dim valueCol As String = DropDownValueField.SelectedValue
        Dim dateCol As String = DropDownDateField.SelectedValue
        Dim buckets As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)
        Dim records As New Dictionary(Of String, Dictionary(Of String, Integer))(StringComparer.OrdinalIgnoreCase)

        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim groupText As String = DisplayValue(row(groupCol))
            Dim period As String = PeriodText(row(dateCol), DropDownDateAggregation.SelectedValue)
            If period = "" Then Continue For
            If Not buckets.ContainsKey(groupText) Then buckets(groupText) = New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            If Not records.ContainsKey(groupText) Then records(groupText) = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            If Not buckets(groupText).ContainsKey(period) Then buckets(groupText)(period) = 0
            If Not records(groupText).ContainsKey(period) Then records(groupText)(period) = 0
            buckets(groupText)(period) += NumericValue(row(valueCol))
            records(groupText)(period) += 1
        Next

        For Each groupPair As KeyValuePair(Of String, Dictionary(Of String, Double)) In buckets
            Dim periods As New List(Of String)(groupPair.Value.Keys)
            periods.Sort()
            For i As Integer = 1 To periods.Count - 1
                Dim previousValue As Double = groupPair.Value(periods(i - 1))
                Dim currentValue As Double = groupPair.Value(periods(i))
                If previousValue = 0 Then Continue For
                Dim pct As Double = (currentValue - previousValue) / Math.Abs(previousValue) * 100
                Dim score As Double = Math.Abs(pct) / 25.0
                If score >= threshold Then
                    AddOutputRow(output, "Unusual Period Movement", groupPair.Key, valueCol, periods(i), FormatNumber(currentValue, 2), FormatNumber(previousValue, 2), FormatNumber(pct, 2) & "%", score, "Current period changed unusually compared with the previous period.", records(groupPair.Key)(periods(i)), RegisterAnalysisFilter(FilterEquals(groupCol, groupPair.Key)))
                End If
            Next
        Next
    End Sub

    Private Sub AddCategoryValuePatternAnomalies(source As DataTable, output As DataTable, threshold As Double)
        If DropDownCategoryField.SelectedValue = "" OrElse DropDownValueField.SelectedValue = "" Then Exit Sub
        Dim categoryCol As String = DropDownCategoryField.SelectedValue
        Dim valueCol As String = DropDownValueField.SelectedValue
        Dim overall As New RunningStats()
        Dim stats As New Dictionary(Of String, RunningStats)(StringComparer.OrdinalIgnoreCase)

        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim categoryText As String = DisplayValue(row(categoryCol))
            Dim value As Double = NumericValue(row(valueCol))
            overall.Add(value)
            If Not stats.ContainsKey(categoryText) Then stats(categoryText) = New RunningStats()
            stats(categoryText).Add(value)
        Next
        If overall.Count < 3 OrElse overall.StdDev() = 0 Then Exit Sub

        For Each pair As KeyValuePair(Of String, RunningStats) In stats
            If pair.Value.Count < 2 Then Continue For
            Dim avg As Double = pair.Value.Average()
            Dim diff As Double = avg - overall.Average()
            Dim score As Double = Math.Abs(diff) / overall.StdDev()
            If score >= threshold Then
                AddOutputRow(output, "Suspicious Category/Value Pattern", pair.Key, categoryCol, "", FormatNumber(avg, 2), FormatNumber(overall.Average(), 2), FormatNumber(diff, 2), score, "Category average value is unusual compared with the whole dataset.", pair.Value.Count, RegisterAnalysisFilter(FilterEquals(categoryCol, pair.Key)))
            End If
        Next
    End Sub

    Private Sub AddOutputRow(output As DataTable, anomalyType As String, groupText As String, comboText As String, periodTextValue As String, valueText As String, expectedText As String, differenceText As String, score As Double, noteText As String, records As Integer, filterId As String)
        Dim r As DataRow = output.NewRow()
        r("Anomaly Type") = anomalyType
        r("Group") = groupText
        r("Category Combination") = comboText
        r("Period") = periodTextValue
        r("Value") = valueText
        r("Expected / Average") = expectedText
        r("Difference") = differenceText
        r("Score") = Math.Round(score, 2)
        r("Note") = noteText
        r("Records") = records
        r("FilterId") = filterId
        output.Rows.Add(r)
    End Sub

    Private Sub GridViewAnomaly_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewAnomaly.RowDataBound
        Dim dt As DataTable = TryCast(Session("AnomalyScoringTable"), DataTable)
        If dt Is Nothing OrElse e.Row.Cells.Count = 0 Then Exit Sub
        HideColumn(dt, e.Row, "FilterId")
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "anomalyfilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("AnomalyScoringTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewAnomaly.AllowPaging = False
            GridViewAnomaly.PageIndex = 0
            GridViewAnomaly.DataSource = Nothing
            GridViewAnomaly.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If
        GridViewAnomaly.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewAnomaly.PageSize = AnalysisGridPageSize
        If Not GridViewAnomaly.AllowPaging Then GridViewAnomaly.PageIndex = 0
        GridViewAnomaly.DataSource = dt
        GridViewAnomaly.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Anomaly Scoring (" & dt.Rows.Count.ToString() & " rows)"
        AnalysisExportSnapshot.Save(Me, "AnomalyScoring", "Anomaly Scoring", LabelInfo, GridViewAnomaly, dt)
    End Sub

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("AnomalyScoringTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("AnomalyScoringTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No anomaly scoring results to export."
            Exit Sub
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "AnomalyScoring_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Anomaly Scoring", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Anomaly Scoring", ""))
        End If
        Response.End()
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAnomaly.PageIndex > 0 Then GridViewAnomaly.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAnomaly.PageIndex < (GridViewAnomaly.PageCount - 1) Then GridViewAnomaly.PageIndex += 1
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
            GridViewAnomaly.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewAnomaly.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewAnomaly.PageIndex < (GridViewAnomaly.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewAnomaly.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewAnomaly.PageCount.ToString()
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
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Group field selects the main entity, category, product, customer, department, or location being scored.", "Category field adds a second dimension for unusual combination and category/value pattern scoring.", "Value field selects the numeric measure used for unusual values inside groups and suspicious value patterns.", "Date field and Date aggregation add period movement scoring by day, week, month, quarter, or year.", "Score threshold controls how strong an anomaly must be before it appears in the grid.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "Anomaly scoring combines several transparent rule-based checks instead of one narrow outlier rule.", "Unusual combinations compare actual group/category counts with expected counts calculated from group and category frequencies.", "Unusual values compare each numeric record with the average and standard deviation inside its group.", "Unusual period movement compares each period total with the previous period for the same group.", "Suspicious category/value patterns compare category averages with the overall dataset average.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Anomaly Type explains which scoring rule created the row.", "Group, Category Combination, and Period show the business context of the anomaly.", "Value, Expected / Average, and Difference show what was observed and what it was compared against.", "Score ranks the strength of the anomaly; higher scores should be reviewed first.", "Records links open the corresponding rows in Data Explorer where a reliable filter can be built.")
        ReadinessFooterGuidance.SetFooter(Me, "Anomaly Scoring", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
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

    Private Sub AddCount(dict As Dictionary(Of String, Integer), key As String)
        If Not dict.ContainsKey(key) Then dict(key) = 0
        dict(key) += 1
    End Sub

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

    Private Function RegisterAnalysisFilter(filterExpression As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("AnomalyScoringFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("AnomalyScoringFilters") = filters
        Return filterId
    End Function

    Private Function SingleRowFilter(row As DataRow) As String
        Dim filters As New List(Of String)()
        For Each col As DataColumn In row.Table.Columns
            filters.Add(ValueFilter(col, row(col)))
        Next
        Return String.Join(" AND ", filters.ToArray())
    End Function

    Private Function FilterEquals(columnName As String, valueText As String) As String
        Return "[" & columnName.Replace("]", "]]" ) & "] = '" & valueText.Replace("'", "''") & "'"
    End Function

    Private Function FilterAnd(leftFilter As String, rightFilter As String) As String
        If leftFilter.Trim() = "" Then Return rightFilter
        If rightFilter.Trim() = "" Then Return leftFilter
        Return "(" & leftFilter & ") AND (" & rightFilter & ")"
    End Function

    Private Function ValueFilter(col As DataColumn, valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return FieldRef(col) & " IS NULL"
        Dim valueText As String = FieldText(valueObject)
        If valueText.Trim() = "" Then Return "(" & FieldRef(col) & " IS NULL OR " & FieldRef(col) & " = '')"
        Dim numericValue As Double
        If ColumnTypeIsNumeric(col) AndAlso Double.TryParse(valueText, numericValue) Then Return FieldRef(col) & " = " & numericValue.ToString(CultureInfo.InvariantCulture)
        If TypeOf valueObject Is DateTime Then Return FieldRef(col) & " = #" & CType(valueObject, DateTime).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "#"
        Return FieldRef(col) & " = '" & valueText.Replace("'", "''") & "'"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "]]" ) & "]"
    End Function

    Private Function ContainsSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        Dim needle As String = searchText.ToLowerInvariant()
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).ToLowerInvariant().Contains(needle) Then Return True
        Next
        Return False
    End Function

    Private Function DisplayValue(valueObject As Object) As String
        Dim valueText As String = FieldText(valueObject).Trim()
        If valueText = "" Then Return "(blank)"
        Return valueText
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

    Private Function ColumnTypeIsNumeric(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function LooksLikeDate(dt As DataTable, col As DataColumn) As Boolean
        If col.DataType Is GetType(DateTime) Then Return True
        Dim checkedValues As Integer = 0
        Dim parsed As Integer = 0
        For i As Integer = 0 To Math.Min(20, dt.Rows.Count) - 1
            Dim valueText As String = FieldText(dt.Rows(i)(col)).Trim()
            If valueText = "" Then Continue For
            checkedValues += 1
            Dim dateValue As DateTime
            If DateTime.TryParse(valueText, dateValue) Then parsed += 1
        Next
        Return checkedValues > 0 AndAlso checkedValues = parsed
    End Function

    Private Function PeriodText(valueObject As Object, periodName As String) As String
        Dim d As DateTime
        If Not DateTime.TryParse(FieldText(valueObject), d) Then Return ""
        Select Case periodName
            Case "Day"
                Return d.ToString("yyyy-MM-dd")
            Case "Week"
                Return d.Year.ToString() & " W" & CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString("00")
            Case "Quarter"
                Return d.Year.ToString() & " Q" & (((d.Month - 1) \ 3) + 1).ToString()
            Case "Year"
                Return d.Year.ToString()
            Case Else
                Return d.ToString("yyyy-MM")
        End Select
    End Function

    Private Function LooksLikeTechnicalId(columnName As String) As Boolean
        Dim n As String = columnName.Trim().ToLowerInvariant()
        Return n = "id" OrElse n = "idx" OrElse n = "indx" OrElse n = "ind" OrElse n.EndsWith("id")
    End Function

    Private Sub SelectFirstNonId(list As DropDownList)
        For i As Integer = 0 To list.Items.Count - 1
            If Not LooksLikeTechnicalId(list.Items(i).Value) Then
                list.SelectedIndex = i
                Exit Sub
            End If
        Next
    End Sub
End Class
