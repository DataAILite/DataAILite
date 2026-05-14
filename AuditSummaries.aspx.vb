Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Web.UI.WebControls

Partial Class AuditSummaries
    Inherits System.Web.UI.Page

    Private Sub AuditSummaries_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Audit Summaries"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Audit%20Summaries"
    End Sub

    Private Sub AuditSummaries_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillAnalysisTypes()
            FillDefaultAuditInputs()
            BuildAndBindAudit()
        ElseIf Session("AuditSummariesTable") IsNot Nothing Then
            BindAudit(CType(Session("AuditSummariesTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = ""
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()
        If repid.Trim() = "" Then
            Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
            If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
                Session("AuditSummariesSource") = existingTable
                Return existingTable
            End If

            LabelError.Text = "Report is not selected."
            Return Nothing
        End If

        Dim dv As DataView = Nothing
        Try
            dv = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            Return Nothing
        End Try

        If ret.Trim() <> "" Then LabelError.Text = ret
        If dv Is Nothing OrElse dv.Table Is Nothing Then
            LabelError.Text = "No data. Run or import report data first."
            Session("AuditSummariesSource") = Nothing
            Return Nothing
        End If

        Session("AuditSummariesSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("AuditSummariesSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("AuditSummariesSource"), DataTable)
    End Function

    Private Sub FillAnalysisTypes()
        DropDownAnalysisType.Items.Clear()
        DropDownAnalysisType.Items.Add(New ListItem("Analytics", "Analytics.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Pivot / Cross Tab", "Pivot.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Variance Analysis", "Variance.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Comparison Reports", "ComparisonReports.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Data Profiling", "Profiling.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Data Quality", "DataQuality.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Ranking Analysis", "Ranking.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Regression Analysis", "Regression.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Time Based Summaries", "TimeBasedSummaries.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Time Series", "TimeSeries.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Outlier Flagging", "OutlierFlagging.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Chart Recommendations", "ChartRecommendationHelpers.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Correlation Threshold", "CorrelationThreshold.aspx"))
        DropDownAnalysisType.Items.Add(New ListItem("Matrix Balancing", "ShowReport.aspx?srd=13"))
        DropDownAnalysisType.Items.Add(New ListItem("Custom / Other", "Custom"))
    End Sub

    Private Sub FillDefaultAuditInputs()
        Dim source As DataTable = GetSourceTable()
        txtResultName.Text = If(Session("REPTITLE") Is Nothing, "Analytical result", Session("REPTITLE").ToString() & " analytical result")
        txtReportFields.Text = FieldList(source)
        txtFilter.Text = CurrentFilterText()
        txtThresholds.Text = CurrentThresholdText()
        txtAggregations.Text = CurrentAggregationText()
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindAudit()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        FillDefaultAuditInputs()
        BuildAndBindAudit()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAudit("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAudit("xls")
    End Sub

    Private Sub lnkAuditAI_Click(sender As Object, e As EventArgs) Handles lnkAuditAI.Click
        Dim dt As DataTable = TryCast(Session("AuditSummariesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAudit()
            dt = TryCast(Session("AuditSummariesTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No audit summary results to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Audit Summaries", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this audit summary. Explain which report fields, filters, thresholds, aggregation options, notes, and current analytical result settings produced the analysis.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindAudit()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As DataTable = CreateAuditTable()

        AddAuditRow(output, "Report", "Report ID", ReportIdText(), "Report used as the source of the analytical result.")
        AddAuditRow(output, "Report", "Report Title", ReportTitleText(), "Display title for the selected report.")
        AddAuditRow(output, "Report", "Source Records", If(source Is Nothing, "0", source.Rows.Count.ToString()), "Record count retrieved from the current report.")
        AddAuditRow(output, "Report", "Source Fields", FieldList(source), "Fields available in the current report dataset.")
        AddAuditRow(output, "Report", "Numeric Fields", NumericFieldList(source), "Numeric fields that can be used in measures, thresholds, regression, ranking, charts, or outlier checks.")
        AddAuditRow(output, "Report", "Date Fields", DateFieldList(source), "Date fields that can drive time-based summaries, time series, comparison periods, and charts.")

        AddAuditRow(output, "Analysis", "Analysis Page/Type", DropDownAnalysisType.SelectedItem.Text, "Analysis page or analytical method that produced, or will produce, the result.")
        AddAuditRow(output, "Analysis", "Result Name", txtResultName.Text.Trim(), "Name used to identify this analytical result.")
        AddAuditRow(output, "Analysis", "Report Fields Used", txtReportFields.Text.Trim(), "Fields selected or considered for the analytical result.")
        AddAuditRow(output, "Analysis", "Filters/Search", EmptyAsNone(txtFilter.Text.Trim()), "Filters, search text, where clauses, period selections, group selections, or source-record restrictions.")
        AddAuditRow(output, "Analysis", "Thresholds", EmptyAsNone(txtThresholds.Text.Trim()), "Thresholds used by the analysis, such as standard deviation, percentage, correlation, ranking count, or business min/max.")
        AddAuditRow(output, "Analysis", "Aggregation Options", EmptyAsNone(txtAggregations.Text.Trim()), "Aggregation options such as Sum, Average, Count, CountDistinct, Min, Max, standard deviation, moving window, or date period.")
        AddAuditRow(output, "Analysis", "Notes", EmptyAsNone(txtNotes.Text.Trim()), "User notes describing assumptions, purpose, or review comments.")

        AddSessionResultRows(output)
        AddAuditRow(output, "Audit", "Generated By", FieldText(Session("logon")), "Signed-in user who generated the audit summary.")
        AddAuditRow(output, "Audit", "Generated At", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Server timestamp for this audit summary.")

        Session("AuditSummariesTable") = output
        BindAudit(output)
    End Sub

    Private Function CreateAuditTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Section", GetType(String))
        dt.Columns.Add("Audit Item", GetType(String))
        dt.Columns.Add("Value", GetType(String))
        dt.Columns.Add("Purpose", GetType(String))
        Return dt
    End Function

    Private Sub AddAuditRow(dt As DataTable, sectionName As String, itemName As String, itemValue As String, purpose As String)
        Dim row As DataRow = dt.NewRow()
        row("Section") = sectionName
        row("Audit Item") = itemName
        row("Value") = itemValue
        row("Purpose") = purpose
        dt.Rows.Add(row)
    End Sub

    Private Sub AddSessionResultRows(dt As DataTable)
        AddSessionTableAudit(dt, "Analytics", "AnalyticsTable", "Current grouped analytics result saved in Session.")
        AddSessionTableAudit(dt, "Pivot / Cross Tab", "PivotTable", "Current pivot result saved in Session.")
        AddSessionTableAudit(dt, "Variance Analysis", "VarianceTable", "Current variance result saved in Session.")
        AddSessionTableAudit(dt, "Comparison Reports", "ComparisonReportsTable", "Current comparison result saved in Session.")
        AddSessionTableAudit(dt, "Data Profiling", "ProfilingTable", "Current profiling result saved in Session.")
        AddSessionTableAudit(dt, "Data Quality", "DataQualityTable", "Current data quality result saved in Session.")
        AddSessionTableAudit(dt, "Ranking Analysis", "RankingTable", "Current ranking result saved in Session.")
        AddSessionTableAudit(dt, "Regression Analysis", "RegressionTable", "Current regression result saved in Session.")
        AddSessionTableAudit(dt, "Time Based Summaries", "TimeBasedSummariesTable", "Current time-based summary result saved in Session.")
        AddSessionTableAudit(dt, "Time Series", "TimeSeriesTable", "Current time-series result saved in Session.")
        AddSessionTableAudit(dt, "Outlier Flagging", "OutlierFlaggingTable", "Current outlier result saved in Session.")
        AddSessionTableAudit(dt, "Chart Recommendations", "ChartRecommendationTable", "Current chart recommendation result saved in Session.")
        AddSessionTableAudit(dt, "Correlation Threshold", "CorrelationThresholdTable", "Current thresholded correlation result saved in Session.")
    End Sub

    Private Sub AddSessionTableAudit(dt As DataTable, labelText As String, sessionKey As String, purpose As String)
        Dim resultTable As DataTable = TryCast(Session(sessionKey), DataTable)
        If resultTable Is Nothing Then Exit Sub
        AddAuditRow(dt, "Current Result", labelText, resultTable.Rows.Count.ToString() & " rows, " & resultTable.Columns.Count.ToString() & " columns", purpose)
    End Sub

    Private Sub BindAudit(dt As DataTable)
        BindAnalysisGrid(dt)
        If dt Is Nothing Then LabelInfo.Text = "" Else LabelInfo.Text = "Audit summary (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

    Private Function FieldList(dt As DataTable) As String
        If dt Is Nothing Then Return ""
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            fields.Add(col.ColumnName)
        Next
        Return String.Join(", ", fields.ToArray())
    End Function

    Private Function NumericFieldList(dt As DataTable) As String
        If dt Is Nothing Then Return ""
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If ColumnTypeIsNumeric(col) Then fields.Add(col.ColumnName)
        Next
        Return EmptyAsNone(String.Join(", ", fields.ToArray()))
    End Function

    Private Function DateFieldList(dt As DataTable) As String
        If dt Is Nothing Then Return ""
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If col.DataType Is GetType(DateTime) OrElse LooksLikeDate(dt, col) Then fields.Add(col.ColumnName)
        Next
        Return EmptyAsNone(String.Join(", ", fields.ToArray()))
    End Function

    Private Function LooksLikeDate(dt As DataTable, col As DataColumn) As Boolean
        For i As Integer = 0 To Math.Min(20, dt.Rows.Count) - 1
            Dim textValue As String = FieldText(dt.Rows(i)(col)).Trim()
            If textValue = "" Then Continue For
            Dim dateValue As DateTime
            Return DateTime.TryParse(textValue, dateValue)
        Next
        Return False
    End Function

    Private Function CurrentFilterText() As String
        Dim parts As New List(Of String)()
        AddSessionText(parts, "WhereStm", "Where")
        AddSessionText(parts, "FilterStm", "Filter")
        AddSessionText(parts, "SearchText", "Search")
        AddSessionText(parts, "srt", "Sort")
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Function CurrentThresholdText() As String
        Dim parts As New List(Of String)()
        AddSessionText(parts, "CorrelationThreshold", "Correlation threshold")
        AddSessionText(parts, "OutlierStdDev", "Std dev")
        AddSessionText(parts, "OutlierPercent", "Percent")
        AddSessionText(parts, "BusinessMin", "Business min")
        AddSessionText(parts, "BusinessMax", "Business max")
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Function CurrentAggregationText() As String
        Dim parts As New List(Of String)()
        AddSessionText(parts, "Aggregate", "Aggregate")
        AddSessionText(parts, "AggregateM", "Multi aggregate")
        AddSessionText(parts, "AxisXM", "X axis")
        AddSessionText(parts, "AxisYM", "Y axis")
        AddSessionText(parts, "ChartType", "Chart type")
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Sub AddSessionText(parts As List(Of String), key As String, labelText As String)
        If Session(key) Is Nothing Then Exit Sub
        Dim valueText As String = Session(key).ToString().Trim()
        If valueText <> "" Then parts.Add(labelText & ": " & valueText)
    End Sub

    Private Function ReportIdText() As String
        If Session("REPORTID") Is Nothing Then Return ""
        Return Session("REPORTID").ToString()
    End Function

    Private Function ReportTitleText() As String
        If Session("REPTITLE") Is Nothing Then Return ""
        Return Session("REPTITLE").ToString()
    End Function

    Private Function EmptyAsNone(valueText As String) As String
        If valueText Is Nothing OrElse valueText.Trim() = "" Then Return "(none)"
        Return valueText
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Sub ExportAudit(formatName As String)
        Dim dt As DataTable = TryCast(Session("AuditSummariesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAudit()
            dt = TryCast(Session("AuditSummariesTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No audit summary to export."
            Exit Sub
        End If

        Dim fileName As String = "AuditSummaries_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Audit Summaries", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Audit Summaries", ""))
        End If
        Response.End()
    End Sub
    Private Const AnalysisGridPageSize As Integer = 50

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewAudit.AllowPaging = False
            GridViewAudit.PageIndex = 0
            GridViewAudit.DataSource = Nothing
            GridViewAudit.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewAudit.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewAudit.PageSize = AnalysisGridPageSize
        If Not GridViewAudit.AllowPaging Then
            GridViewAudit.PageIndex = 0
        ElseIf GridViewAudit.PageIndex < 0 OrElse GridViewAudit.PageIndex >= GridViewAudit.PageCount Then
            GridViewAudit.PageIndex = 0
        End If

        GridViewAudit.DataSource = dt
        GridViewAudit.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewAudit.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewAudit.HeaderRow.Cells.Count Then
            GridViewAudit.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewAudit.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewAudit.PageIndex < (GridViewAudit.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewAudit.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewAudit.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAudit.PageIndex > 0 Then GridViewAudit.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAudit.PageIndex < (GridViewAudit.PageCount - 1) Then GridViewAudit.PageIndex += 1
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
            GridViewAudit.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub


    Private Function GridTableForAI(dt As DataTable, ParamArray extraHiddenColumns() As String) As DataTable
        If dt Is Nothing Then Return Nothing
        Dim aiTable As DataTable = dt.Copy()
        Dim hiddenColumns() As String = {"FilterId", "BaseFilterId", "CompareFilterId"}
        For Each columnName As String In hiddenColumns
            If aiTable.Columns.Contains(columnName) Then aiTable.Columns.Remove(columnName)
        Next
        If extraHiddenColumns IsNot Nothing Then
            For Each columnName As String In extraHiddenColumns
                If columnName IsNot Nothing AndAlso aiTable.Columns.Contains(columnName) Then aiTable.Columns.Remove(columnName)
            Next
        End If
        Return aiTable
    End Function
    Private Function BuildAnalysisQuestion(baseQuestion As String) As String
        SetAnalysisExplanationLabels()
        Dim parts As New List(Of String)()
        parts.Add(baseQuestion)
        If LabelAnalysisSubtitle IsNot Nothing AndAlso LabelAnalysisSubtitle.Text.Trim() <> "" Then parts.Add("Input: " & LabelAnalysisSubtitle.Text.Trim())
        If LabelModelExplanation IsNot Nothing AndAlso LabelModelExplanation.Text.Trim() <> "" Then parts.Add(LabelModelExplanation.Text.Trim())
        If LabelAlgorithmExplanation IsNot Nothing AndAlso LabelAlgorithmExplanation.Text.Trim() <> "" Then parts.Add(LabelAlgorithmExplanation.Text.Trim())
        If LabelOutputExplanation IsNot Nothing AndAlso LabelOutputExplanation.Text.Trim() <> "" Then parts.Add(LabelOutputExplanation.Text.Trim())
        Return String.Join(vbCrLf & vbCrLf, parts.ToArray())
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = "Model: Audit summary for analytical traceability. Inputs are the selected report fields, filters, thresholds, aggregation choices, search text, and current report context."
        LabelAlgorithmExplanation.Text = "Algorithm: The page records the analytical settings that produced the result, including field selections, filter text, threshold values, aggregation options, and result counts so the analysis can be reviewed later."
        LabelOutputExplanation.Text = "Output: The grid shows analysis type, selected fields, filters, thresholds, aggregation options, records/results affected, and audit notes. It explains which choices produced each analytical result."
    End Sub
End Class
