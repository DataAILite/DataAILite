Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Text
Imports System.Web

Partial Class DataAdmin
    Inherits System.Web.UI.Page

    Private Const PreviewRows As Integer = 5
    Private Const PreviewColumns As Integer = 7
    Private Const DefaultDashboardPageSize As Integer = 12
    Private Const MinDashboardPageSize As Integer = 4
    Private Const MaxDashboardPageSize As Integer = 36
    Private dashboardPageNumber As Integer = 1
    Private dashboardPageCount As Integer = 1
    Private dashboardTotalTiles As Integer = 0

    Private Sub DataAdmin_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then
            Session("REPORTID") = Request("Report").ToString().Trim()
        End If

        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then
            LabelPageTtl.Text = Session("PAGETTL").ToString()
        End If

        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then
            lblHeader.Text = Session("REPTITLE").ToString() & " - Data Analytics Dashboard"
        ElseIf Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Data Analytics Dashboard"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Analytics%20Dashboard"
    End Sub

    Private Sub DataAdmin_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        BindDataPreviews()
        SetDashboardPagingControls()
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then
            Response.Redirect(node.Value)
        End If
    End Sub

    Private Sub BindDataPreviews()
        Dim dv As DataView = CurrentDataView()

        litPreviewAnalytics.Text = BuildAnalyticsPreviewHtml(dv)
        litPreviewStatistics.Text = BuildStatisticsPreviewHtml(dv)
        litPreviewGroups.Text = BuildGroupsPreviewHtml(dv)
        litPreviewChartRecommendations.Text = BuildChartRecommendationsPreviewHtml(dv)
        litPreviewCorrelation.Text = BuildCorrelationPreviewHtml(dv)
        litPreviewMatrix.Text = BuildMatrixPreviewHtml(dv)
        litPreviewPivot.Text = BuildPivotPreviewHtml(dv)
        litPreviewVariance.Text = BuildVariancePreviewHtml(dv)
        litPreviewComparison.Text = BuildComparisonPreviewHtml(dv)
        litPreviewProfiling.Text = BuildProfilingPreviewHtml(dv)
        litPreviewQuality.Text = BuildQualityPreviewHtml(dv)
        litPreviewRanking.Text = BuildRankingPreviewHtml(dv)
        litPreviewRegression.Text = BuildRegressionPreviewHtml(dv)
        litPreviewTimeBased.Text = BuildTimeBasedPreviewHtml(dv)
        litPreviewTimeSeries.Text = BuildTimeSeriesPreviewHtml(dv)
        litPreviewOutliers.Text = BuildOutlierPreviewHtml(dv)
        litPreviewMapReadiness.Text = BuildMapReadinessPreviewHtml(dv)
        litPreviewDataAI.Text = BuildDataAIPreviewHtml(dv)
    End Sub

    Private Sub SetDashboardPagingControls()
        Dim tiles As List(Of Control) = DashboardTiles()
        Dim pageSize As Integer = RequestedDashboardPageSize()
        dashboardTotalTiles = tiles.Count
        dashboardPageCount = CInt(Math.Ceiling(dashboardTotalTiles / CDbl(pageSize)))
        If dashboardPageCount < 1 Then dashboardPageCount = 1
        dashboardPageNumber = RequestedDashboardPage()
        If dashboardPageNumber > dashboardPageCount Then dashboardPageNumber = dashboardPageCount
        If dashboardPageNumber < 1 Then dashboardPageNumber = 1

        Dim startIndex As Integer = (dashboardPageNumber - 1) * pageSize
        Dim endIndex As Integer = Math.Min(startIndex + pageSize, dashboardTotalTiles)
        For i As Integer = 0 To tiles.Count - 1
            tiles(i).Visible = (i >= startIndex AndAlso i < endIndex)
        Next

        HiddenDashboardPageSize.Value = pageSize.ToString()
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

    Private Function DashboardTiles() As List(Of Control)
        Dim tiles As New List(Of Control)()
        tiles.Add(tileAnalytics)
        tiles.Add(tileStatistics)
        tiles.Add(tileGroups)
        tiles.Add(tileChartRecommendations)
        tiles.Add(tileDataAI)
        tiles.Add(tileRegression)
        tiles.Add(tilePivot)
        tiles.Add(tileVariance)
        tiles.Add(tileCorrelation)
        tiles.Add(tileComparison)
        tiles.Add(tileQuality)
        tiles.Add(tileRanking)
        tiles.Add(tileProfiling)
        tiles.Add(tileTimeBased)
        tiles.Add(tileTimeSeries)
        tiles.Add(tileOutliers)
        tiles.Add(tileMapReadiness)
        tiles.Add(tileMatrix)
        Return tiles
    End Function

    Private Function RequestedDashboardPageSize() As Integer
        Dim pageSize As Integer = DefaultDashboardPageSize
        If Request("ps") IsNot Nothing AndAlso Integer.TryParse(Request("ps").ToString(), pageSize) Then
            Return BoundedDashboardPageSize(pageSize)
        End If

        If Request.Form(HiddenDashboardPageSize.UniqueID) IsNot Nothing AndAlso Integer.TryParse(Request.Form(HiddenDashboardPageSize.UniqueID), pageSize) Then
            Return BoundedDashboardPageSize(pageSize)
        End If

        Return DefaultDashboardPageSize
    End Function

    Private Function BoundedDashboardPageSize(pageSize As Integer) As Integer
        If pageSize < MinDashboardPageSize Then Return MinDashboardPageSize
        If pageSize > MaxDashboardPageSize Then Return MaxDashboardPageSize
        Return pageSize
    End Function

    Private Function RequestedDashboardPage() As Integer
        Dim requestedPage As Integer = 1
        If Request("page") IsNot Nothing AndAlso Integer.TryParse(Request("page").ToString(), requestedPage) Then
            Return Math.Max(1, requestedPage)
        End If
        Return 1
    End Function

    Private Function DashboardPageUrl(pageNumber As Integer) As String
        Dim pageSize As Integer = RequestedDashboardPageSize()
        Return "DataAdmin.aspx?page=" & pageNumber.ToString() & "&ps=" & pageSize.ToString()
    End Function

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

    Private Function BuildAnalyticsPreviewHtml(dv As DataView) As String
        Dim analyticsTable As DataTable = AnalyticsGroupsTable()
        If analyticsTable IsNot Nothing AndAlso analyticsTable.Rows.Count > 0 Then
            Return RenderDataTablePreview(analyticsTable)
        End If

        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")
        Return BuildGroupsPreviewHtml(dv)
    End Function

    Private Function BuildStatisticsPreviewHtml(dv As DataView) As String
        Dim statsTable As DataTable = OverallStatisticsTable(dv)
        If statsTable IsNot Nothing AndAlso statsTable.Rows.Count > 0 Then
            Return RenderDataTablePreview(statsTable)
        End If

        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")
        Return BuildProfilingPreviewHtml(dv)
    End Function

    Private Function BuildGroupsPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim groupCol As DataColumn = FirstTextColumn(dv.Table)
        If groupCol Is Nothing Then Return BuildStatisticsPreviewHtml(dv)

        Dim counts As Dictionary(Of String, Integer) = GroupCounts(dv, groupCol)
        Dim rows As New List(Of String())()
        Dim added As Integer = 0
        For Each kvp As KeyValuePair(Of String, Integer) In counts
            rows.Add(New String() {kvp.Key, kvp.Value.ToString()})
            added += 1
            If added >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {groupCol.ColumnName, "Records"}, rows)
    End Function

    Private Function BuildCorrelationPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim numericCols As List(Of DataColumn) = NumericColumns(dv.Table)
        If numericCols.Count < 2 Then Return BuildStatisticsPreviewHtml(dv)

        Dim rows As New List(Of String())()
        For i As Integer = 0 To Math.Min(PreviewRows, numericCols.Count - 1) - 1
            rows.Add(New String() {numericCols(i).ColumnName, numericCols(i + 1).ColumnName, FormatPreviewNumber(Correlation(dv, numericCols(i), numericCols(i + 1)))})
        Next
        Return RenderPreviewTable(New String() {"Field 1", "Field 2", "Corr"}, rows)
    End Function

    Private Function BuildChartRecommendationsPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim textCols As List(Of DataColumn) = TextColumns(dv.Table)
        Dim numericCols As List(Of DataColumn) = NumericColumns(dv.Table)
        Dim dateCol As DataColumn = FirstDateColumn(dv.Table)
        Dim rows As New List(Of String())()

        If dateCol IsNot Nothing AndAlso numericCols.Count > 0 Then
            rows.Add(New String() {"Line Chart", dateCol.ColumnName, numericCols(0).ColumnName, "High"})
        End If
        If textCols.Count > 0 AndAlso numericCols.Count > 0 Then
            rows.Add(New String() {"Bar Chart", textCols(0).ColumnName, numericCols(0).ColumnName, "High"})
            rows.Add(New String() {"Column Chart", textCols(0).ColumnName, numericCols(0).ColumnName, "High"})
        End If
        If textCols.Count > 1 AndAlso numericCols.Count > 0 Then
            rows.Add(New String() {"Pivot / Combo", textCols(0).ColumnName & ", " & textCols(1).ColumnName, numericCols(0).ColumnName, "Medium"})
        End If
        If numericCols.Count > 1 Then
            rows.Add(New String() {"Scatter Chart", numericCols(0).ColumnName, numericCols(1).ColumnName, "High"})
        End If

        If rows.Count = 0 Then Return BuildStatisticsPreviewHtml(dv)
        While rows.Count > PreviewRows
            rows.RemoveAt(rows.Count - 1)
        End While
        Return RenderPreviewTable(New String() {"Recommended", "Category/Date", "Value", "Priority"}, rows)
    End Function

    Private Function BuildMatrixPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim textCols As List(Of DataColumn) = TextColumns(dv.Table)
        If textCols.Count < 2 Then Return BuildGroupsPreviewHtml(dv)

        Return BuildCrossTabPreviewHtml(dv, textCols(0), textCols(1), Nothing)
    End Function

    Private Function BuildPivotPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim textCols As List(Of DataColumn) = TextColumns(dv.Table)
        If textCols.Count < 2 Then Return BuildGroupsPreviewHtml(dv)

        Return BuildCrossTabPreviewHtml(dv, textCols(0), textCols(1), FirstNumericColumn(dv.Table))
    End Function

    Private Function BuildVariancePreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim textCols As List(Of DataColumn) = TextColumns(dv.Table)
        If textCols.Count = 0 Then Return BuildRawPreviewHtml(dv)

        Dim rowCol As DataColumn = textCols(0)
        Dim compareCol As DataColumn = Nothing
        If textCols.Count > 1 Then compareCol = textCols(1)

        Dim valueCol As DataColumn = FirstNumericColumn(dv.Table)

        If compareCol Is Nothing Then
            Dim totalsByGroup As Dictionary(Of String, Double) = GroupTotals(dv, rowCol, valueCol)
            Dim records As New List(Of CountRecord)()
            For Each kvp As KeyValuePair(Of String, Double) In totalsByGroup
                records.Add(New CountRecord(kvp.Key, kvp.Value))
            Next
            records.Sort(AddressOf CompareCountRecordsDescending)
            If records.Count < 2 Then Return BuildGroupsPreviewHtml(dv)

            Dim baseValue As Double = records(0).Value
            Dim compareValue As Double = records(1).Value
            Dim variance As Double = compareValue - baseValue
            Dim percentChange As Double = 0
            If baseValue <> 0 Then percentChange = variance / baseValue * 100

            Dim singleRows As New List(Of String())()
            singleRows.Add(New String() {records(0).Text, FormatPreviewNumber(baseValue), FormatPreviewNumber(compareValue), FormatPreviewNumber(variance), FormatPreviewNumber(percentChange) & "%"})
            Return RenderPreviewTable(New String() {rowCol.ColumnName, "Base", "Compare", "Variance", "% Change"}, singleRows)
        End If

        Dim compareValues As List(Of String) = TopValues(dv, compareCol, 2)
        If compareValues.Count < 2 Then Return BuildGroupsPreviewHtml(dv)

        Dim baseName As String = compareValues(0)
        Dim compareName As String = compareValues(1)
        If SameText(baseName, compareName) Then Return BuildGroupsPreviewHtml(dv)

        Dim rowValues As List(Of String) = TopValues(dv, rowCol, PreviewRows)
        Dim rows As New List(Of String())()
        For i As Integer = 0 To rowValues.Count - 1
            Dim baseValue As Double = FilteredTotal(dv, rowCol, rowValues(i), compareCol, baseName, valueCol)
            Dim compareValue As Double = FilteredTotal(dv, rowCol, rowValues(i), compareCol, compareName, valueCol)
            Dim variance As Double = compareValue - baseValue
            Dim percentChange As Double = 0
            If baseValue <> 0 Then percentChange = variance / baseValue * 100
            Dim records As Integer = FilteredCount(dv, rowCol, rowValues(i), compareCol, baseName) + FilteredCount(dv, rowCol, rowValues(i), compareCol, compareName)

            rows.Add(New String() {rowValues(i), FormatPreviewNumber(baseValue), FormatPreviewNumber(compareValue), FormatPreviewNumber(variance), FormatPreviewNumber(percentChange) & "%", records.ToString()})
        Next

        Return RenderPreviewTable(New String() {rowCol.ColumnName, baseName, compareName, "Variance", "% Change", "Records"}, rows)
    End Function

    Private Function BuildComparisonPreviewHtml(dv As DataView) As String
        Return BuildVariancePreviewHtml(dv)
    End Function

    Private Function BuildTimeBasedPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim dateCol As DataColumn = FirstDateColumn(dv.Table)
        If dateCol Is Nothing Then Return BuildStatisticsPreviewHtml(dv)

        Dim valueCol As DataColumn = FirstNumericColumn(dv.Table)
        Dim periodTotals As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        Dim periodCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To dv.Count - 1
            Dim dateValue As DateTime
            If Not DateTime.TryParse(FieldText(dv(i)(dateCol.ColumnName)), dateValue) Then Continue For

            Dim periodName As String = dateValue.ToString("yyyy-MM", CultureInfo.InvariantCulture)
            If Not periodTotals.ContainsKey(periodName) Then
                periodTotals(periodName) = 0
                periodCounts(periodName) = 0
            End If
            periodCounts(periodName) += 1
            If valueCol IsNot Nothing Then periodTotals(periodName) += NumericValue(dv(i)(valueCol.ColumnName))
        Next

        Dim periods As New List(Of String)(periodCounts.Keys)
        periods.Sort()

        Dim rows As New List(Of String())()
        For i As Integer = 0 To Math.Min(PreviewRows, periods.Count) - 1
            Dim totalValue As String = periodCounts(periods(i)).ToString()
            If valueCol IsNot Nothing Then totalValue = FormatPreviewNumber(periodTotals(periods(i)))
            rows.Add(New String() {periods(i), periodCounts(periods(i)).ToString(), totalValue})
        Next

        If rows.Count = 0 Then Return BuildStatisticsPreviewHtml(dv)
        Dim valueHeader As String = "Records"
        If valueCol IsNot Nothing Then valueHeader = valueCol.ColumnName
        Return RenderPreviewTable(New String() {"Period", "Records", valueHeader}, rows)
    End Function

    Private Function BuildTimeSeriesPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim dateCol As DataColumn = FirstDateColumn(dv.Table)
        Dim valueCol As DataColumn = FirstNumericColumn(dv.Table)
        If dateCol Is Nothing OrElse valueCol Is Nothing Then Return BuildTimeBasedPreviewHtml(dv)

        Dim periodTotals As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To dv.Count - 1
            Dim dateValue As DateTime
            If Not DateTime.TryParse(FieldText(dv(i)(dateCol.ColumnName)), dateValue) Then Continue For

            Dim periodName As String = dateValue.ToString("yyyy-MM", CultureInfo.InvariantCulture)
            If Not periodTotals.ContainsKey(periodName) Then periodTotals(periodName) = 0
            periodTotals(periodName) += NumericValue(dv(i)(valueCol.ColumnName))
        Next

        Dim periods As New List(Of String)(periodTotals.Keys)
        periods.Sort()

        Dim rows As New List(Of String())()
        For i As Integer = 0 To Math.Min(PreviewRows, periods.Count) - 1
            Dim rollingTotal As Double = 0
            Dim rollingCount As Integer = 0
            For j As Integer = Math.Max(0, i - 2) To i
                rollingTotal += periodTotals(periods(j))
                rollingCount += 1
            Next
            Dim movingAverage As Double = 0
            If rollingCount > 0 Then movingAverage = rollingTotal / rollingCount
            rows.Add(New String() {periods(i), FormatPreviewNumber(periodTotals(periods(i))), FormatPreviewNumber(rollingTotal), FormatPreviewNumber(movingAverage)})
        Next

        If rows.Count = 0 Then Return BuildStatisticsPreviewHtml(dv)
        Return RenderPreviewTable(New String() {"Period", "Total", "Rolling Total", "Moving Avg"}, rows)
    End Function

    Private Function BuildOutlierPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim valueCol As DataColumn = FirstNumericColumn(dv.Table)
        If valueCol Is Nothing Then Return BuildStatisticsPreviewHtml(dv)

        Dim groupCol As DataColumn = FirstTextColumn(dv.Table)
        Dim values As New List(Of Double)()
        For i As Integer = 0 To dv.Count - 1
            values.Add(NumericValue(dv(i)(valueCol.ColumnName)))
        Next

        If values.Count < 2 Then Return BuildStatisticsPreviewHtml(dv)

        Dim avg As Double = 0
        For i As Integer = 0 To values.Count - 1
            avg += values(i)
        Next
        avg = avg / values.Count

        Dim variance As Double = 0
        For i As Integer = 0 To values.Count - 1
            variance += Math.Pow(values(i) - avg, 2)
        Next
        Dim stdDev As Double = Math.Sqrt(variance / values.Count)
        If stdDev = 0 Then Return BuildRankingPreviewHtml(dv)

        Dim rows As New List(Of String())()
        For i As Integer = 0 To dv.Count - 1
            Dim value As Double = NumericValue(dv(i)(valueCol.ColumnName))
            Dim zScore As Double = Math.Abs((value - avg) / stdDev)
            If zScore >= 1.5 Then
                Dim itemText As String = "Row " & (i + 1).ToString()
                If groupCol IsNot Nothing Then itemText = FieldText(dv(i)(groupCol.ColumnName))
                rows.Add(New String() {itemText, FormatPreviewNumber(value), FormatPreviewNumber(zScore)})
                If rows.Count >= PreviewRows Then Exit For
            End If
        Next

        If rows.Count = 0 Then Return BuildRankingPreviewHtml(dv)
        Return RenderPreviewTable(New String() {"Item", valueCol.ColumnName, "Std Dev"}, rows)
    End Function

    Private Function BuildProfilingPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim rows As New List(Of String())()
        For colIndex As Integer = 0 To Math.Min(PreviewRows, dv.Table.Columns.Count) - 1
            Dim col As DataColumn = dv.Table.Columns(colIndex)
            rows.Add(New String() {col.ColumnName, FriendlyType(col), DistinctCount(dv, col).ToString()})
        Next
        Return RenderPreviewTable(New String() {"Field", "Type", "Distinct"}, rows)
    End Function

    Private Function BuildQualityPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim rows As New List(Of String())()
        For colIndex As Integer = 0 To Math.Min(PreviewRows - 1, dv.Table.Columns.Count) - 1
            Dim col As DataColumn = dv.Table.Columns(colIndex)
            rows.Add(New String() {"Missing", col.ColumnName, BlankCount(dv, col).ToString()})
        Next
        rows.Add(New String() {"Duplicate", "Record", DuplicateCount(dv).ToString()})
        Return RenderPreviewTable(New String() {"Check", "Field", "Records"}, rows)
    End Function

    Private Function BuildRankingPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim groupCol As DataColumn = FirstTextColumn(dv.Table)
        Dim valueCol As DataColumn = FirstNumericColumn(dv.Table)
        If groupCol Is Nothing Then Return BuildStatisticsPreviewHtml(dv)

        Dim totals As Dictionary(Of String, Double) = GroupTotals(dv, groupCol, valueCol)
        Dim records As New List(Of CountRecord)()
        For Each kvp As KeyValuePair(Of String, Double) In totals
            records.Add(New CountRecord(kvp.Key, kvp.Value))
        Next
        records.Sort(AddressOf CompareCountRecordsDescending)

        Dim rows As New List(Of String())()
        For i As Integer = 0 To Math.Min(PreviewRows, records.Count) - 1
            rows.Add(New String() {(i + 1).ToString(), records(i).Text, FormatPreviewNumber(records(i).Value)})
        Next
        Dim valueHeader As String = "Records"
        If valueCol IsNot Nothing Then valueHeader = "Value"
        Return RenderPreviewTable(New String() {"Rank", "Item", valueHeader}, rows)
    End Function

    Private Function BuildRegressionPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim numericCols As List(Of DataColumn) = NumericColumns(dv.Table)
        If numericCols.Count < 2 Then Return BuildStatisticsPreviewHtml(dv)

        Dim xCol As DataColumn = numericCols(0)
        Dim yCol As DataColumn = numericCols(1)
        Dim groupCol As DataColumn = FirstTextColumn(dv.Table)

        If groupCol Is Nothing Then
            Dim allBucket As RegressionPreviewBucket = RegressionBucketForRows(dv, Nothing, "", xCol, yCol)
            Dim allRows As New List(Of String())()
            AddRegressionPreviewRow(allRows, "All", xCol, yCol, allBucket)
            Return RenderPreviewTable(New String() {"Group", "X Field", "Y Field", "Records", "Slope", "Intercept", "R Squared", "Predicted Y"}, allRows)
        End If

        Dim groupValues As List(Of String) = TopValues(dv, groupCol, PreviewRows)
        Dim rows As New List(Of String())()
        For i As Integer = 0 To groupValues.Count - 1
            Dim bucket As RegressionPreviewBucket = RegressionBucketForRows(dv, groupCol, groupValues(i), xCol, yCol)
            AddRegressionPreviewRow(rows, groupValues(i), xCol, yCol, bucket)
        Next

        If rows.Count = 0 Then Return BuildStatisticsPreviewHtml(dv)
        Return RenderPreviewTable(New String() {"Group", "X Field", "Y Field", "Records", "Slope", "Intercept", "R Squared", "Predicted Y"}, rows)
    End Function

    Private Function RegressionBucketForRows(dv As DataView, groupCol As DataColumn, groupValue As String, xCol As DataColumn, yCol As DataColumn) As RegressionPreviewBucket
        Dim bucket As New RegressionPreviewBucket()
        For i As Integer = 0 To dv.Count - 1
            If groupCol IsNot Nothing AndAlso Not SameText(FieldText(dv(i)(groupCol.ColumnName)), groupValue) Then Continue For

            Dim xValue As Double = NumericValue(dv(i)(xCol.ColumnName))
            Dim yValue As Double = NumericValue(dv(i)(yCol.ColumnName))
            bucket.AddPair(xValue, yValue)
        Next
        Return bucket
    End Function

    Private Sub AddRegressionPreviewRow(rows As List(Of String()), groupText As String, xCol As DataColumn, yCol As DataColumn, bucket As RegressionPreviewBucket)
        If bucket.Count < 2 Then Exit Sub

        Dim slopeValue As Double = bucket.Slope()
        Dim interceptValue As Double = bucket.Intercept()
        Dim corrValue As Double = bucket.Correlation()
        Dim predictedValue As Double = bucket.PredictedY(bucket.AverageX())
        rows.Add(New String() {groupText, xCol.ColumnName, yCol.ColumnName, bucket.Count.ToString(), FormatPreviewNumber(slopeValue), FormatPreviewNumber(interceptValue), FormatPreviewNumber(corrValue * corrValue), FormatPreviewNumber(predictedValue)})
    End Sub

    Private Class RegressionPreviewBucket
        Public Count As Integer
        Public SumX As Double
        Public SumY As Double
        Public SumXX As Double
        Public SumYY As Double
        Public SumXY As Double

        Public Sub AddPair(xValue As Double, yValue As Double)
            Count += 1
            SumX += xValue
            SumY += yValue
            SumXX += xValue * xValue
            SumYY += yValue * yValue
            SumXY += xValue * yValue
        End Sub

        Public Function AverageX() As Double
            If Count = 0 Then Return 0
            Return SumX / Count
        End Function

        Public Function AverageY() As Double
            If Count = 0 Then Return 0
            Return SumY / Count
        End Function

        Public Function Slope() As Double
            Dim denominator As Double = Count * SumXX - SumX * SumX
            If Count < 2 OrElse denominator = 0 Then Return 0
            Return (Count * SumXY - SumX * SumY) / denominator
        End Function

        Public Function Intercept() As Double
            If Count = 0 Then Return 0
            Return AverageY() - Slope() * AverageX()
        End Function

        Public Function Correlation() As Double
            Dim denominator As Double = Math.Sqrt((Count * SumXX - SumX * SumX) * (Count * SumYY - SumY * SumY))
            If Count < 2 OrElse denominator = 0 Then Return 0
            Return (Count * SumXY - SumX * SumY) / denominator
        End Function

        Public Function PredictedY(xValue As Double) As Double
            Return Intercept() + Slope() * xValue
        End Function
    End Class

    Private Function BuildMapPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim locationCols As New List(Of DataColumn)()
        For i As Integer = 0 To dv.Table.Columns.Count - 1
            Dim name As String = dv.Table.Columns(i).ColumnName.ToLowerInvariant()
            If name.Contains("city") OrElse name.Contains("state") OrElse name.Contains("country") OrElse name.Contains("zip") OrElse name.Contains("lat") OrElse name.Contains("lon") OrElse name.Contains("address") Then
                locationCols.Add(dv.Table.Columns(i))
            End If
            If locationCols.Count >= PreviewColumns Then Exit For
        Next

        If locationCols.Count = 0 Then Return BuildRawPreviewHtml(dv)
        Return BuildColumnSamplePreviewHtml(dv, locationCols)
    End Function

    Private Function BuildMapReadinessPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim latCol As DataColumn = FindCoordinateColumn(dv.Table, True)
        Dim lonCol As DataColumn = FindCoordinateColumn(dv.Table, False)
        If latCol Is Nothing OrElse lonCol Is Nothing Then Return BuildMapPreviewHtml(dv)

        Dim totalRecords As Integer = dv.Count
        Dim missingCoordinates As Integer = 0
        Dim invalidRanges As Integer = 0
        Dim coordinateKeys As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To dv.Count - 1
            Dim latText As String = FieldText(dv(i)(latCol.ColumnName)).Trim()
            Dim lonText As String = FieldText(dv(i)(lonCol.ColumnName)).Trim()
            If latText = "" OrElse lonText = "" Then
                missingCoordinates += 1
                Continue For
            End If

            Dim latValue As Double
            Dim lonValue As Double
            If Not Double.TryParse(latText, NumberStyles.Any, CultureInfo.InvariantCulture, latValue) Then
                invalidRanges += 1
                Continue For
            End If
            If Not Double.TryParse(lonText, NumberStyles.Any, CultureInfo.InvariantCulture, lonValue) Then
                invalidRanges += 1
                Continue For
            End If
            If latValue < -90 OrElse latValue > 90 OrElse lonValue < -180 OrElse lonValue > 180 Then
                invalidRanges += 1
                Continue For
            End If

            Dim key As String = FormatPreviewNumber(latValue) & "|" & FormatPreviewNumber(lonValue)
            If Not coordinateKeys.ContainsKey(key) Then coordinateKeys.Add(key, 0)
            coordinateKeys(key) += 1
        Next

        Dim duplicateLocations As Integer = 0
        For Each pair As KeyValuePair(Of String, Integer) In coordinateKeys
            If pair.Value > 1 Then duplicateLocations += pair.Value
        Next

        Dim rows As New List(Of String())()
        rows.Add(New String() {"Total records", totalRecords.ToString(), "Rows checked"})
        rows.Add(New String() {"Missing coordinates", missingCoordinates.ToString(), "Latitude/longitude blanks"})
        rows.Add(New String() {"Invalid ranges", invalidRanges.ToString(), "Outside map limits"})
        rows.Add(New String() {"Duplicate locations", duplicateLocations.ToString(), "Repeated coordinates"})
        rows.Add(New String() {"KML-ready records", Math.Max(0, totalRecords - missingCoordinates - invalidRanges).ToString(), "Valid coordinates"})

        Return RenderPreviewTable(New String() {"Check", "Count", "Notes"}, rows)
    End Function

    Private Function BuildDataAIPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim cols As New List(Of DataColumn)()
        For colIndex As Integer = 0 To Math.Min(5, dv.Table.Columns.Count) - 1
            cols.Add(dv.Table.Columns(colIndex))
        Next

        Return BuildColumnSamplePreviewHtml(dv, cols, 5)
    End Function

    Private Function FindCoordinateColumn(table As DataTable, latitude As Boolean) As DataColumn
        If table Is Nothing Then Return Nothing

        For i As Integer = 0 To table.Columns.Count - 1
            Dim name As String = table.Columns(i).ColumnName.ToLowerInvariant()
            If latitude Then
                If name = "lat" OrElse name = "latitude" OrElse name.Contains("latitude") Then Return table.Columns(i)
            Else
                If name = "lon" OrElse name = "lng" OrElse name = "long" OrElse name = "longitude" OrElse name.Contains("longitude") Then Return table.Columns(i)
            End If
        Next

        For i As Integer = 0 To table.Columns.Count - 1
            Dim name As String = table.Columns(i).ColumnName.ToLowerInvariant()
            If latitude AndAlso name.Contains("lat") Then Return table.Columns(i)
            If Not latitude AndAlso (name.Contains("lon") OrElse name.Contains("lng")) Then Return table.Columns(i)
        Next

        Return Nothing
    End Function

    Private Function AnalyticsGroupsTable() As DataTable
        Dim dt As DataTable = TryCast(Session("dataGroups"), DataTable)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return AnalyticsDashboardTable(dt)

        If Session("REPORTID") Is Nothing OrElse Session("REPORTID").ToString().Trim() = "" Then Return Nothing

        Dim ret As String = String.Empty
        Try
            Dim dvGroups As DataView = mRecords("SELECT DISTINCT Tbl1Fld1,Tbl2Fld2 FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID").ToString().Replace("'", "''") & "' AND Doing='GROUP BY' ", ret)
            If dvGroups IsNot Nothing AndAlso dvGroups.Table IsNot Nothing AndAlso dvGroups.Table.Rows.Count > 0 Then
                Session("dataGroups") = dvGroups.Table
                Return AnalyticsDashboardTable(dvGroups.Table)
            End If
        Catch ex As Exception
            Return Nothing
        End Try

        Return Nothing
    End Function

    Private Function AnalyticsDashboardTable(source As DataTable) As DataTable
        If source Is Nothing OrElse source.Rows.Count = 0 Then Return source

        Dim dt As New DataTable()
        dt.Columns.Add("Category/Group 1", GetType(String))
        dt.Columns.Add("Category/Group 2", GetType(String))
        dt.Columns.Add("Matrix/Pivot", GetType(String))
        dt.Columns.Add("Bar Chart", GetType(String))
        dt.Columns.Add("...", GetType(String))
        dt.Columns.Add("Dashboard", GetType(String))

        For i As Integer = 0 To source.Rows.Count - 1
            Dim row As DataRow = dt.NewRow()
            row("Category/Group 1") = SourceFieldText(source.Rows(i), "Tbl1Fld1", 0)
            row("Category/Group 2") = SourceFieldText(source.Rows(i), "Tbl2Fld2", 1)
            row("Matrix/Pivot") = "matrix"
            row("Bar Chart") = "bar"
            row("...") = "..."
            row("Dashboard") = "stats dashboard"
            dt.Rows.Add(row)
        Next

        Return dt
    End Function

    Private Function SourceFieldText(row As DataRow, columnName As String, fallbackIndex As Integer) As String
        If row.Table.Columns.Contains(columnName) Then Return FieldText(row(columnName))
        If fallbackIndex >= 0 AndAlso fallbackIndex < row.Table.Columns.Count Then Return FieldText(row(fallbackIndex))
        Return ""
    End Function

    Private Function OverallStatisticsTable(dv As DataView) As DataTable
        Dim dt As DataTable = TryCast(Session("GridView2DataSource"), DataTable)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return dt

        dt = TryCast(Session("dbstats"), DataTable)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return dt

        If Not HasPreviewData(dv) Then Return Nothing

        Dim err As String = String.Empty
        Try
            Dim statsTable As DataTable = CreateStatsTable()
            statsTable = CalcStats(dv.ToTable(), statsTable, err)
            If statsTable IsNot Nothing AndAlso statsTable.Rows.Count > 0 Then
                Session("dbstats") = statsTable
                Session("GridView2DataSource") = statsTable
                Return statsTable
            End If
        Catch ex As Exception
            Return Nothing
        End Try

        Return Nothing
    End Function

    Private Function CurrentDataView() As DataView
        Dim dv As DataView = TryCast(Session("dv3"), DataView)
        If dv IsNot Nothing AndAlso dv.Table IsNot Nothing Then Return dv

        If Session("REPORTID") Is Nothing OrElse Session("REPORTID").ToString().Trim() = "" Then Return Nothing

        Dim ret As String = String.Empty
        Try
            dv = RetrieveReportData(Session("REPORTID").ToString(), "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
        Catch ex As Exception
            Return Nothing
        End Try

        If dv IsNot Nothing Then Session("dv3") = dv
        Return dv
    End Function

    Private Function FirstDateColumn(table As DataTable) As DataColumn
        If table Is Nothing Then Return Nothing

        For i As Integer = 0 To table.Columns.Count - 1
            If table.Columns(i).DataType.Equals(GetType(DateTime)) Then Return table.Columns(i)
        Next

        For i As Integer = 0 To table.Columns.Count - 1
            Dim name As String = table.Columns(i).ColumnName.ToLowerInvariant()
            If name.Contains("date") OrElse name.Contains("time") OrElse name.Contains("day") Then Return table.Columns(i)
        Next

        Return Nothing
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Function BuildRawPreviewHtml(dv As DataView) As String
        If Not HasPreviewData(dv) Then Return EmptyPreview("No report data available.")

        Dim cols As New List(Of DataColumn)()
        For i As Integer = 0 To Math.Min(PreviewColumns, dv.Table.Columns.Count) - 1
            cols.Add(dv.Table.Columns(i))
        Next
        Return BuildColumnSamplePreviewHtml(dv, cols)
    End Function

    Private Function BuildColumnSamplePreviewHtml(dv As DataView, cols As List(Of DataColumn)) As String
        Return BuildColumnSamplePreviewHtml(dv, cols, PreviewRows)
    End Function

    Private Function BuildColumnSamplePreviewHtml(dv As DataView, cols As List(Of DataColumn), rowLimit As Integer) As String
        Dim headers As New List(Of String)()
        For i As Integer = 0 To cols.Count - 1
            headers.Add(cols(i).ColumnName)
        Next

        Dim rows As New List(Of String())()
        For rowIndex As Integer = 0 To Math.Min(rowLimit, dv.Count) - 1
            Dim rowValues(cols.Count - 1) As String
            For colIndex As Integer = 0 To cols.Count - 1
                rowValues(colIndex) = FieldText(dv(rowIndex)(cols(colIndex).ColumnName))
            Next
            rows.Add(rowValues)
        Next

        Return RenderPreviewTable(headers.ToArray(), rows)
    End Function

    Private Function BuildCrossTabPreviewHtml(dv As DataView, rowCol As DataColumn, colCol As DataColumn, valueCol As DataColumn) As String
        Dim rowValues As List(Of String) = TopValues(dv, rowCol, PreviewRows)
        Dim colValues As List(Of String) = TopValues(dv, colCol, PreviewColumns - 1)
        If rowValues.Count = 0 OrElse colValues.Count = 0 Then Return BuildGroupsPreviewHtml(dv)

        Dim headers As New List(Of String)()
        headers.Add(rowCol.ColumnName)
        For i As Integer = 0 To colValues.Count - 1
            headers.Add(colValues(i))
        Next

        Dim rows As New List(Of String())()
        For r As Integer = 0 To rowValues.Count - 1
            Dim rowData(colValues.Count) As String
            rowData(0) = rowValues(r)
            For c As Integer = 0 To colValues.Count - 1
                rowData(c + 1) = FormatPreviewNumber(CrossTabValue(dv, rowCol, rowValues(r), colCol, colValues(c), valueCol))
            Next
            rows.Add(rowData)
        Next

        Return RenderPreviewTable(headers.ToArray(), rows)
    End Function

    Private Function RenderPreviewTable(headers As String(), rows As List(Of String())) As String
        If rows Is Nothing OrElse rows.Count = 0 Then Return EmptyPreview("No preview rows available.")

        Dim sb As New StringBuilder()
        sb.Append("<table class=""previewTable""><tr>")
        For i As Integer = 0 To headers.Length - 1
            sb.Append("<th>")
            sb.Append(HttpUtility.HtmlEncode(headers(i)))
            sb.Append("</th>")
        Next
        sb.Append("</tr>")

        For rowIndex As Integer = 0 To rows.Count - 1
            sb.Append("<tr>")
            For colIndex As Integer = 0 To headers.Length - 1
                sb.Append("<td>")
                If colIndex < rows(rowIndex).Length Then sb.Append(HttpUtility.HtmlEncode(rows(rowIndex)(colIndex)))
                sb.Append("</td>")
            Next
            sb.Append("</tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Function RenderDataTablePreview(dt As DataTable) As String
        If dt Is Nothing OrElse dt.Columns.Count = 0 OrElse dt.Rows.Count = 0 Then Return EmptyPreview("No preview rows available.")

        Dim colCount As Integer = Math.Min(PreviewColumns, dt.Columns.Count)
        Dim headers(colCount - 1) As String
        For colIndex As Integer = 0 To colCount - 1
            headers(colIndex) = dt.Columns(colIndex).ColumnName
        Next

        Dim rows As New List(Of String())()
        For rowIndex As Integer = 0 To Math.Min(PreviewRows, dt.Rows.Count) - 1
            Dim rowValues(colCount - 1) As String
            For colIndex As Integer = 0 To colCount - 1
                rowValues(colIndex) = FieldText(dt.Rows(rowIndex)(colIndex))
            Next
            rows.Add(rowValues)
        Next

        Return RenderPreviewTable(headers, rows)
    End Function

    Private Function HasPreviewData(dv As DataView) As Boolean
        Return dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Columns.Count > 0 AndAlso dv.Count > 0
    End Function

    Private Function EmptyPreview(message As String) As String
        Return "<span class=""previewEmpty"">" & HttpUtility.HtmlEncode(message) & "</span>"
    End Function

    Private Function FirstTextColumn(dt As DataTable) As DataColumn
        Dim cols As List(Of DataColumn) = TextColumns(dt)
        If cols.Count = 0 Then Return Nothing
        Return cols(0)
    End Function

    Private Function TextColumns(dt As DataTable) As List(Of DataColumn)
        Dim cols As New List(Of DataColumn)()
        If dt Is Nothing Then Return cols

        For i As Integer = 0 To dt.Columns.Count - 1
            If Not ColumnTypeIsNumeric(dt.Columns(i)) AndAlso dt.Columns(i).DataType IsNot GetType(DateTime) Then cols.Add(dt.Columns(i))
        Next
        Return cols
    End Function

    Private Function FirstNumericColumn(dt As DataTable) As DataColumn
        Dim cols As List(Of DataColumn) = NumericColumns(dt)
        If cols.Count = 0 Then Return Nothing
        Return cols(0)
    End Function

    Private Function NumericColumns(dt As DataTable) As List(Of DataColumn)
        Dim cols As New List(Of DataColumn)()
        If dt Is Nothing Then Return cols

        For i As Integer = 0 To dt.Columns.Count - 1
            If ColumnTypeIsNumeric(dt.Columns(i)) Then cols.Add(dt.Columns(i))
        Next
        Return cols
    End Function

    Private Function ColumnTypeIsNumeric(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(SByte) OrElse col.DataType Is GetType(Int16) OrElse col.DataType Is GetType(Int32) OrElse col.DataType Is GetType(Int64) OrElse col.DataType Is GetType(UInt16) OrElse col.DataType Is GetType(UInt32) OrElse col.DataType Is GetType(UInt64) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function GroupCounts(dv As DataView, groupCol As DataColumn) As Dictionary(Of String, Integer)
        Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To dv.Count - 1
            Dim key As String = FieldText(dv(i)(groupCol.ColumnName))
            If key.Trim() = "" Then key = "(Blank)"
            If Not counts.ContainsKey(key) Then counts.Add(key, 0)
            counts(key) += 1
        Next
        Return counts
    End Function

    Private Function GroupTotals(dv As DataView, groupCol As DataColumn, valueCol As DataColumn) As Dictionary(Of String, Double)
        Dim totals As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To dv.Count - 1
            Dim key As String = FieldText(dv(i)(groupCol.ColumnName))
            If key.Trim() = "" Then key = "(Blank)"
            If Not totals.ContainsKey(key) Then totals.Add(key, 0)

            If valueCol Is Nothing Then
                totals(key) += 1
            Else
                totals(key) += NumericValue(dv(i)(valueCol.ColumnName))
            End If
        Next
        Return totals
    End Function

    Private Function TopValues(dv As DataView, col As DataColumn, maxCount As Integer) As List(Of String)
        Dim counts As Dictionary(Of String, Integer) = GroupCounts(dv, col)
        Dim records As New List(Of CountRecord)()
        For Each kvp As KeyValuePair(Of String, Integer) In counts
            records.Add(New CountRecord(kvp.Key, kvp.Value))
        Next
        records.Sort(AddressOf CompareCountRecordsDescending)

        Dim values As New List(Of String)()
        For i As Integer = 0 To Math.Min(maxCount, records.Count) - 1
            values.Add(records(i).Text)
        Next
        Return values
    End Function

    Private Function CrossTabValue(dv As DataView, rowCol As DataColumn, rowValue As String, colCol As DataColumn, colValue As String, valueCol As DataColumn) As Double
        Dim total As Double = 0
        For i As Integer = 0 To dv.Count - 1
            If SameText(FieldText(dv(i)(rowCol.ColumnName)), rowValue) AndAlso SameText(FieldText(dv(i)(colCol.ColumnName)), colValue) Then
                If valueCol Is Nothing Then
                    total += 1
                Else
                    total += NumericValue(dv(i)(valueCol.ColumnName))
                End If
            End If
        Next
        Return total
    End Function

    Private Function FilteredTotal(dv As DataView, rowCol As DataColumn, rowValue As String, compareCol As DataColumn, compareValue As String, valueCol As DataColumn) As Double
        Dim total As Double = 0
        For i As Integer = 0 To dv.Count - 1
            If SameText(FieldText(dv(i)(rowCol.ColumnName)), rowValue) AndAlso SameText(FieldText(dv(i)(compareCol.ColumnName)), compareValue) Then
                If valueCol Is Nothing Then
                    total += 1
                Else
                    total += NumericValue(dv(i)(valueCol.ColumnName))
                End If
            End If
        Next
        Return total
    End Function

    Private Function FilteredCount(dv As DataView, rowCol As DataColumn, rowValue As String, compareCol As DataColumn, compareValue As String) As Integer
        Dim total As Integer = 0
        For i As Integer = 0 To dv.Count - 1
            If SameText(FieldText(dv(i)(rowCol.ColumnName)), rowValue) AndAlso SameText(FieldText(dv(i)(compareCol.ColumnName)), compareValue) Then
                total += 1
            End If
        Next
        Return total
    End Function

    Private Function SameText(leftValue As String, rightValue As String) As Boolean
        If leftValue.Trim() = "" Then leftValue = "(Blank)"
        If rightValue.Trim() = "" Then rightValue = "(Blank)"
        Return String.Compare(leftValue, rightValue, True, CultureInfo.InvariantCulture) = 0
    End Function

    Private Function NumericValue(valueObject As Object) As Double
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return 0
        Dim value As Double
        If Double.TryParse(valueObject.ToString(), value) Then Return value
        Return 0
    End Function

    Private Function BlankCount(dv As DataView, col As DataColumn) As Integer
        Dim count As Integer = 0
        For i As Integer = 0 To dv.Count - 1
            If FieldText(dv(i)(col.ColumnName)).Trim() = "" Then count += 1
        Next
        Return count
    End Function

    Private Function DistinctCount(dv As DataView, col As DataColumn) As Integer
        Dim values As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To dv.Count - 1
            Dim key As String = FieldText(dv(i)(col.ColumnName))
            If Not values.ContainsKey(key) Then values.Add(key, True)
        Next
        Return values.Count
    End Function

    Private Function DuplicateCount(dv As DataView) As Integer
        Dim seen As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim duplicateRows As Integer = 0

        For rowIndex As Integer = 0 To dv.Count - 1
            Dim key As String = RowKey(dv(rowIndex).Row)
            If seen.ContainsKey(key) Then
                seen(key) += 1
                duplicateRows += 1
            Else
                seen.Add(key, 1)
            End If
        Next
        Return duplicateRows
    End Function

    Private Function RowKey(row As DataRow) As String
        Dim sb As New StringBuilder()
        For i As Integer = 0 To row.Table.Columns.Count - 1
            If i > 0 Then sb.Append("|")
            sb.Append(FieldText(row(i)))
        Next
        Return sb.ToString()
    End Function

    Private Function Correlation(dv As DataView, col1 As DataColumn, col2 As DataColumn) As Double
        Dim n As Integer = 0
        Dim sumX As Double = 0
        Dim sumY As Double = 0
        Dim sumXY As Double = 0
        Dim sumX2 As Double = 0
        Dim sumY2 As Double = 0

        For i As Integer = 0 To dv.Count - 1
            Dim x As Double = NumericValue(dv(i)(col1.ColumnName))
            Dim y As Double = NumericValue(dv(i)(col2.ColumnName))
            n += 1
            sumX += x
            sumY += y
            sumXY += x * y
            sumX2 += x * x
            sumY2 += y * y
        Next

        Dim denominator As Double = Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY))
        If denominator = 0 Then Return 0
        Return (n * sumXY - sumX * sumY) / denominator
    End Function

    Private Function FriendlyType(col As DataColumn) As String
        If ColumnTypeIsNumeric(col) Then Return "Number"
        If col.DataType Is GetType(DateTime) Then Return "Date"
        Return "Text"
    End Function

    Private Function FormatPreviewNumber(value As Double) As String
        Return value.ToString("0.##", CultureInfo.InvariantCulture)
    End Function

    Private Class CountRecord
        Public Text As String
        Public Value As Double

        Public Sub New(recordText As String, recordValue As Double)
            Text = recordText
            Value = recordValue
        End Sub
    End Class

    Private Function CompareCountRecordsDescending(x As CountRecord, y As CountRecord) As Integer
        Dim ret As Integer = y.Value.CompareTo(x.Value)
        If ret = 0 Then ret = String.Compare(x.Text, y.Text, StringComparison.OrdinalIgnoreCase)
        Return ret
    End Function
End Class
