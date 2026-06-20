Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class SavedAnalysisTemplates
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50
    Private Const TemplateSessionKey As String = "SavedAnalysisTemplatesList"

    Private Sub SavedAnalysisTemplates_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Saved Analysis Templates"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Saved%20Analysis%20Templates"
    End Sub

    Private Sub SavedAnalysisTemplates_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            BuildAndBindAnalysis()
        ElseIf Session("SavedAnalysisTemplatesTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("SavedAnalysisTemplatesTable"), DataTable))
        End If
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewTemplates.PageIndex = 0
        SaveTemplateFromControls()
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtTemplateName.Text = ""
        txtPrimaryField.Text = ""
        txtSecondaryField.Text = ""
        txtRowField.Text = ""
        txtColumnField.Text = ""
        txtValueField.Text = ""
        txtSecondValueField.Text = ""
        txtDateField.Text = ""
        txtKeyField.Text = ""
        txtStageField.Text = ""
        txtBaseValue.Text = ""
        txtCompareValue.Text = ""
        txtCompareReport.Text = ""
        txtFieldSet.Text = ""
        txtFilters.Text = ""
        txtThresholds.Text = ""
        txtTopN.Text = ""
        txtWindowPeriods.Text = ""
        txtAssumptionPercent.Text = ""
        txtNotes.Text = ""
        txtSearch.Text = ""
        DropDownAggregation.SelectedIndex = 0
        DropDownDateAggregation.SelectedIndex = 0
        DropDownTemplateMode.SelectedIndex = 0
        DropDownAnalysisPage.SelectedIndex = 0
        GridViewTemplates.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkTemplatesAI_Click(sender As Object, e As EventArgs) Handles lnkTemplatesAI.Click
        Dim dt As DataTable = TryCast(Session("SavedAnalysisTemplatesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("SavedAnalysisTemplatesTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No saved templates to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Saved Analysis Templates", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret these saved analysis templates. Explain which templates should be used first, whether fields and thresholds are clear, and what follow-up analytics are implied.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Function TemplateTable() As DataTable
        Dim dt As DataTable = TryCast(Session(TemplateSessionKey), DataTable)
        If dt Is Nothing Then
            dt = New DataTable()
            dt.Columns.Add("Template Name", GetType(String))
            dt.Columns.Add("Analysis Page", GetType(String))
            dt.Columns.Add("Field Set", GetType(String))
            dt.Columns.Add("Primary Field", GetType(String))
            dt.Columns.Add("Secondary Field", GetType(String))
            dt.Columns.Add("Row Field", GetType(String))
            dt.Columns.Add("Column Field", GetType(String))
            dt.Columns.Add("Value Field", GetType(String))
            dt.Columns.Add("Second Value Field", GetType(String))
            dt.Columns.Add("Date Field", GetType(String))
            dt.Columns.Add("Date Aggregation", GetType(String))
            dt.Columns.Add("Key Field", GetType(String))
            dt.Columns.Add("Stage Field", GetType(String))
            dt.Columns.Add("Base Value", GetType(String))
            dt.Columns.Add("Compare Value", GetType(String))
            dt.Columns.Add("Compare Report ID", GetType(String))
            dt.Columns.Add("Filters", GetType(String))
            dt.Columns.Add("Thresholds", GetType(String))
            dt.Columns.Add("Aggregation", GetType(String))
            dt.Columns.Add("Model / Type", GetType(String))
            dt.Columns.Add("Top N / Number", GetType(String))
            dt.Columns.Add("Number of Time Periods", GetType(String))
            dt.Columns.Add("Assumption %", GetType(String))
            dt.Columns.Add("Notes", GetType(String))
            dt.Columns.Add("Created", GetType(String))
            dt.Columns.Add("Records", GetType(Integer))
            dt.Columns.Add("What Next", GetType(String))
            dt.Columns.Add("FilterId", GetType(String))
            Session(TemplateSessionKey) = dt
        End If
        EnsureTemplateColumns(dt)
        Return dt
    End Function

    Private Sub EnsureTemplateColumns(dt As DataTable)
        Dim columns As String() = New String() {"Primary Field", "Secondary Field", "Row Field", "Column Field", "Value Field", "Second Value Field", "Date Field", "Date Aggregation", "Key Field", "Stage Field", "Base Value", "Compare Value", "Compare Report ID", "Model / Type", "Top N / Number", "Number of Time Periods", "Assumption %"}
        For Each columnName As String In columns
            If Not dt.Columns.Contains(columnName) Then dt.Columns.Add(columnName, GetType(String))
        Next
    End Sub

    Private Sub SaveTemplateFromControls()
        If txtTemplateName.Text.Trim() = "" Then Return
        Dim dt As DataTable = TemplateTable()
        Dim r As DataRow = dt.NewRow()
        r("Template Name") = txtTemplateName.Text.Trim()
        r("Analysis Page") = DropDownAnalysisPage.SelectedValue
        r("Field Set") = TemplateFieldSetText()
        r("Primary Field") = txtPrimaryField.Text.Trim()
        r("Secondary Field") = txtSecondaryField.Text.Trim()
        r("Row Field") = txtRowField.Text.Trim()
        r("Column Field") = txtColumnField.Text.Trim()
        r("Value Field") = txtValueField.Text.Trim()
        r("Second Value Field") = txtSecondValueField.Text.Trim()
        r("Date Field") = txtDateField.Text.Trim()
        r("Date Aggregation") = DropDownDateAggregation.SelectedValue
        r("Key Field") = txtKeyField.Text.Trim()
        r("Stage Field") = txtStageField.Text.Trim()
        r("Base Value") = txtBaseValue.Text.Trim()
        r("Compare Value") = txtCompareValue.Text.Trim()
        r("Compare Report ID") = txtCompareReport.Text.Trim()
        r("Filters") = txtFilters.Text.Trim()
        r("Thresholds") = txtThresholds.Text.Trim()
        r("Aggregation") = DropDownAggregation.SelectedValue
        r("Model / Type") = DropDownTemplateMode.SelectedValue
        r("Top N / Number") = txtTopN.Text.Trim()
        r("Number of Time Periods") = txtWindowPeriods.Text.Trim()
        r("Assumption %") = txtAssumptionPercent.Text.Trim()
        r("Notes") = txtNotes.Text.Trim()
        r("Created") = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        r("Records") = SourceRecordCount()
        r("What Next") = NextPageFor(DropDownAnalysisPage.SelectedValue)
        r("FilterId") = RegisterAnalysisFilter("1 = 1")
        dt.Rows.Add(r)
    End Sub

    Private Function TemplateFieldSetText() As String
        If txtFieldSet.Text.Trim() <> "" Then Return txtFieldSet.Text.Trim()
        Dim fields As New List(Of String)()
        AddFieldText(fields, txtPrimaryField.Text)
        AddFieldText(fields, txtSecondaryField.Text)
        AddFieldText(fields, txtRowField.Text)
        AddFieldText(fields, txtColumnField.Text)
        AddFieldText(fields, txtValueField.Text)
        AddFieldText(fields, txtSecondValueField.Text)
        AddFieldText(fields, txtDateField.Text)
        AddFieldText(fields, txtKeyField.Text)
        AddFieldText(fields, txtStageField.Text)
        Return String.Join(", ", fields.ToArray())
    End Function

    Private Sub AddFieldText(fields As List(Of String), fieldText As String)
        If fieldText Is Nothing Then Exit Sub
        For Each fieldName As String In ParseTemplateFields(fieldText)
            If fieldName <> "" AndAlso Not fields.Contains(fieldName) Then fields.Add(fieldName)
        Next
    End Sub

    Private Function NextPageFor(pageName As String) As String
        Select Case pageName
            Case "Detail Analytics"
                Return "Analytics.aspx"
            Case "Data Readiness Scanner"
                Return "DataReadinessScanner.aspx"
            Case "Data Overall Statistics"
                Return "ShowReport.aspx?srd=8"
            Case "Groups Statistics"
                Return "ReportViews.aspx?grpstats=yes"
            Case "Fields Correlation"
                Return "ShowReport.aspx?srd=12"
            Case "Correlation Threshold"
                Return "CorrelationThreshold.aspx"
            Case "Chart Recommendations"
                Return "ChartRecommendationHelpers.aspx"
            Case "Map Readiness"
                Return "MapReadines.aspx"
            Case "Matrix Balancing"
                Return "ShowReport.aspx?srd=13"
            Case "Pivot / Cross Tab"
                Return "Pivot.aspx"
            Case "Variance Analysis"
                Return "Variance.aspx"
            Case "Comparison Reports"
                Return "ComparisonReports.aspx"
            Case "Data Profiling"
                Return "Profiling.aspx"
            Case "Data Dictionary"
                Return "DataDictionary.aspx"
            Case "Rule-Based Alerts"
                Return "RuleBasedAlerts.aspx"
            Case "Anomaly Scoring"
                Return "AnomalyScoring.aspx"
            Case "Data Quality"
                Return "DataQuality.aspx"
            Case "Ranking Analysis"
                Return "Ranking.aspx"
            Case "Regression Analysis"
                Return "Regression.aspx"
            Case "Time Based Summaries"
                Return "TimeBasedSummaries.aspx"
            Case "Time Series"
                Return "TimeSeries.aspx"
            Case "Outlier Flagging"
                Return "OutlierFlagging.aspx"
            Case "Audit Summaries"
                Return "AuditSummaries.aspx"
            Case "Cohort Analysis"
                Return "Cohort.aspx"
            Case "Funnel Analysis"
                Return "Funnel.aspx"
            Case "ABC Pareto Analysis"
                Return "ABCPareto.aspx"
            Case "Data Drift Analysis"
                Return "DataDrift.aspx"
            Case "KPI Builder"
                Return "KPIBuilder.aspx"
            Case "Saved Analysis Templates"
                Return "SavedAnalysisTemplates.aspx"
            Case "Automated Analysis Narratives"
                Return "AutomatedAnalysisNarratives.aspx"
            Case "Cross-Report Comparison"
                Return "CrossReportComparison.aspx"
            Case "Market Dashboard"
                Return "MarketAdmin.aspx"
            Case "Market Demand"
                Return "MarketDemand.aspx"
            Case "Market Pricing"
                Return "MarketPricing.aspx"
            Case "Market Elasticity"
                Return "MarketElasticity.aspx"
            Case "Market Basket"
                Return "MarketBasket.aspx"
            Case "Market Segments"
                Return "MarketSegments.aspx"
            Case "Market Churn"
                Return "MarketChurn.aspx"
            Case "Market Risk"
                Return "MarketRisk.aspx"
            Case "Market Inventory"
                Return "MarketInventory.aspx"
            Case "Market Profit"
                Return "MarketProfit.aspx"
            Case "Market Scenario"
                Return "MarketScenario.aspx"
            Case Else
                Return "Analytics.aspx"
        End Select
    End Function

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim dt As DataTable = TemplateTable().Copy()
        If txtSearch.Text.Trim() <> "" Then
            Dim filtered As DataTable = dt.Clone()
            For Each row As DataRow In dt.Rows
                If RowContains(row, txtSearch.Text.Trim()) Then filtered.ImportRow(row)
            Next
            dt = filtered
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Sub GridViewTemplates_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewTemplates.RowDataBound
        Dim dt As DataTable = TryCast(Session("SavedAnalysisTemplatesTable"), DataTable)
        If dt Is Nothing OrElse e.Row.Cells.Count = 0 Then Exit Sub
        HideColumn(dt, e.Row, "FilterId")
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "templatefilter")
        AddWhatNextLink(e.Row, dt)
    End Sub

    Private Sub GridViewTemplates_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewTemplates.RowCommand
        If e.CommandName <> "OpenTemplate" Then Exit Sub
        Dim dt As DataTable = TryCast(Session("SavedAnalysisTemplatesTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim rowIndex As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), rowIndex) Then Exit Sub
        If rowIndex < 0 OrElse rowIndex >= dt.Rows.Count Then Exit Sub

        Dim row As DataRow = dt.Rows(rowIndex)
        ApplyTemplateToSession(row)
        Response.Redirect(BuildTemplateOpenUrl(row))
    End Sub

    Private Sub AddWhatNextLink(row As GridViewRow, dt As DataTable)
        If Not dt.Columns.Contains("What Next") Then Exit Sub
        Dim idx As Integer = dt.Columns.IndexOf("What Next")
        If idx < 0 OrElse idx >= row.Cells.Count Then Exit Sub
        Dim dataIndex As Integer = row.DataItemIndex
        If dataIndex < 0 OrElse dataIndex >= dt.Rows.Count Then Exit Sub
        If Convert.ToString(dt.Rows(dataIndex)("What Next")).Trim() = "" Then Exit Sub
        Dim link As New LinkButton()
        link.Text = "open"
        link.CssClass = "NodeStyle"
        link.CommandName = "OpenTemplate"
        link.CommandArgument = dataIndex.ToString()
        link.ToolTip = "Open this saved template with its fields, filters, thresholds, aggregation, and notes."
        row.Cells(idx).Controls.Clear()
        row.Cells(idx).Controls.Add(link)
    End Sub

    Private Sub ApplyTemplateToSession(row As DataRow)
        Dim fields As TemplateFieldSelection = GetTemplateFieldSelection(TemplateValue(row, "Field Set"))
        Dim aggregation As String = NormalizeAggregation(TemplateValue(row, "Aggregation"))
        Dim primaryField As String = FirstAvailableValue(TemplateValue(row, "Primary Field"), fields.Category1)
        Dim secondaryField As String = FirstAvailableValue(TemplateValue(row, "Secondary Field"), TemplateValue(row, "Column Field"), fields.Category2)
        Dim rowField As String = FirstAvailableValue(TemplateValue(row, "Row Field"), primaryField)
        Dim columnField As String = FirstAvailableValue(TemplateValue(row, "Column Field"), secondaryField)
        Dim valueField As String = FirstAvailableValue(TemplateValue(row, "Value Field"), fields.Value1)
        Dim secondValueField As String = FirstAvailableValue(TemplateValue(row, "Second Value Field"), fields.Value2)

        Session("SavedTemplateName") = TemplateValue(row, "Template Name")
        Session("SavedTemplateAnalysisPage") = TemplateValue(row, "Analysis Page")
        Session("SavedTemplateFieldSet") = TemplateValue(row, "Field Set")
        Session("SavedTemplatePrimaryField") = primaryField
        Session("SavedTemplateSecondaryField") = secondaryField
        Session("SavedTemplateRowField") = rowField
        Session("SavedTemplateColumnField") = columnField
        Session("SavedTemplateValueField") = valueField
        Session("SavedTemplateSecondValueField") = secondValueField
        Session("SavedTemplateDateField") = TemplateValue(row, "Date Field")
        Session("SavedTemplateDateAggregation") = TemplateValue(row, "Date Aggregation")
        Session("SavedTemplateKeyField") = TemplateValue(row, "Key Field")
        Session("SavedTemplateStageField") = TemplateValue(row, "Stage Field")
        Session("SavedTemplateBaseValue") = TemplateValue(row, "Base Value")
        Session("SavedTemplateCompareValue") = TemplateValue(row, "Compare Value")
        Session("SavedTemplateCompareReport") = TemplateValue(row, "Compare Report ID")
        Session("SavedTemplateFilters") = TemplateValue(row, "Filters")
        Session("SavedTemplateThresholds") = TemplateValue(row, "Thresholds")
        Session("SavedTemplateAggregation") = aggregation
        Session("SavedTemplateModelType") = TemplateValue(row, "Model / Type")
        Session("SavedTemplateTopN") = TemplateValue(row, "Top N / Number")
        Session("SavedTemplateWindowPeriods") = TemplateValue(row, "Number of Time Periods")
        Session("SavedTemplateAssumptionPercent") = TemplateValue(row, "Assumption %")
        Session("SavedTemplateNotes") = TemplateValue(row, "Notes")

        If primaryField <> "" Then Session("cat1") = primaryField
        If secondaryField <> "" Then Session("cat2") = secondaryField
        If valueField <> "" Then
            Session("AxisY") = valueField
            Session("AxisYM") = valueField
        End If
        If secondValueField <> "" Then
            Session("AxisY2") = secondValueField
            Session("AxisYM") = valueField & "," & secondValueField
        End If
        If aggregation <> "" Then
            Session("Aggregate") = aggregation
            Session("AggregateM") = aggregation
            Session("Aggregate2") = aggregation
        End If

        Dim reportId As String = If(Session("REPORTID") Is Nothing, "CurrentReport", Session("REPORTID").ToString().Trim())
        If reportId = "" Then reportId = "CurrentReport"
        If valueField <> "" Then Session("Regression_" & reportId & "_XField") = valueField
        If secondValueField <> "" Then
            Session("Regression_" & reportId & "_YField") = secondValueField
        ElseIf primaryField <> "" Then
            Session("Regression_" & reportId & "_YField") = primaryField
        End If
        If primaryField <> "" Then Session("Regression_" & reportId & "_GroupField") = primaryField
        If TemplateValue(row, "Model / Type") <> "" Then
            Session("Regression_" & reportId & "_EquationType") = TemplateValue(row, "Model / Type")
        ElseIf aggregation <> "" Then
            Session("Regression_" & reportId & "_EquationType") = "BestFit"
        End If
    End Sub

    Private Function BuildTemplateOpenUrl(row As DataRow) As String
        Dim url As String = TemplateValue(row, "What Next")
        If url = "" Then url = NextPageFor(TemplateValue(row, "Analysis Page"))

        Dim fields As TemplateFieldSelection = GetTemplateFieldSelection(TemplateValue(row, "Field Set"))
        Dim primaryField As String = FirstAvailableValue(TemplateValue(row, "Primary Field"), fields.Category1)
        Dim secondaryField As String = FirstAvailableValue(TemplateValue(row, "Secondary Field"), TemplateValue(row, "Column Field"), fields.Category2)
        Dim rowField As String = FirstAvailableValue(TemplateValue(row, "Row Field"), primaryField)
        Dim columnField As String = FirstAvailableValue(TemplateValue(row, "Column Field"), secondaryField)
        Dim valueField As String = FirstAvailableValue(TemplateValue(row, "Value Field"), fields.Value1)
        Dim secondValueField As String = FirstAvailableValue(TemplateValue(row, "Second Value Field"), fields.Value2)
        Dim parameters As New List(Of String)()
        parameters.Add("from=SavedAnalysisTemplates")
        AddQueryParameter(parameters, "template", TemplateValue(row, "Template Name"))
        AddQueryParameter(parameters, "fieldset", TemplateValue(row, "Field Set"))
        AddQueryParameter(parameters, "primary", primaryField)
        AddQueryParameter(parameters, "secondary", secondaryField)
        AddQueryParameter(parameters, "rowfield", rowField)
        AddQueryParameter(parameters, "columnfield", columnField)
        AddQueryParameter(parameters, "datefield", TemplateValue(row, "Date Field"))
        AddQueryParameter(parameters, "dateagg", TemplateValue(row, "Date Aggregation"))
        AddQueryParameter(parameters, "keyfield", TemplateValue(row, "Key Field"))
        AddQueryParameter(parameters, "stagefield", TemplateValue(row, "Stage Field"))
        AddQueryParameter(parameters, "basevalue", TemplateValue(row, "Base Value"))
        AddQueryParameter(parameters, "comparevalue", TemplateValue(row, "Compare Value"))
        AddQueryParameter(parameters, "comparereport", TemplateValue(row, "Compare Report ID"))
        AddQueryParameter(parameters, "filters", TemplateValue(row, "Filters"))
        AddQueryParameter(parameters, "thresholds", TemplateValue(row, "Thresholds"))
        AddQueryParameter(parameters, "mode", TemplateValue(row, "Model / Type"))
        AddQueryParameter(parameters, "topn", TemplateValue(row, "Top N / Number"))
        AddQueryParameter(parameters, "periods", TemplateValue(row, "Number of Time Periods"))
        AddQueryParameter(parameters, "assumption", TemplateValue(row, "Assumption %"))
        AddQueryParameter(parameters, "notes", TemplateValue(row, "Notes"))
        AddQueryParameter(parameters, "cat1", primaryField)
        AddQueryParameter(parameters, "cat2", secondaryField)
        AddQueryParameter(parameters, "x1", primaryField)
        AddQueryParameter(parameters, "x2", secondaryField)
        AddQueryParameter(parameters, "y1", valueField)
        AddQueryParameter(parameters, "y2", secondValueField)
        AddQueryParameter(parameters, "fn", NormalizeAggregation(TemplateValue(row, "Aggregation")))

        If parameters.Count = 0 Then Return url
        Return url & If(url.Contains("?"), "&", "?") & String.Join("&", parameters.ToArray())
    End Function

    Private Function FirstAvailableValue(ParamArray values() As String) As String
        For Each valueText As String In values
            If valueText IsNot Nothing AndAlso valueText.Trim() <> "" Then Return valueText.Trim()
        Next
        Return ""
    End Function

    Private Sub AddQueryParameter(parameters As List(Of String), name As String, value As String)
        If value Is Nothing OrElse value.Trim() = "" Then Exit Sub
        parameters.Add(name & "=" & Server.UrlEncode(value.Trim()))
    End Sub

    Private Function TemplateValue(row As DataRow, columnName As String) As String
        If row Is Nothing OrElse Not row.Table.Columns.Contains(columnName) OrElse row(columnName) Is Nothing OrElse IsDBNull(row(columnName)) Then Return ""
        Return row(columnName).ToString().Trim()
    End Function

    Private Function GetTemplateFieldSelection(fieldSet As String) As TemplateFieldSelection
        Dim result As New TemplateFieldSelection()
        Dim fields As List(Of String) = ParseTemplateFields(fieldSet)
        Dim numericFields As New List(Of String)()
        Dim categoryFields As New List(Of String)()

        For Each fieldName As String In fields
            If IsNumericSourceField(fieldName) Then
                numericFields.Add(fieldName)
            Else
                categoryFields.Add(fieldName)
            End If
        Next

        If categoryFields.Count > 0 Then result.Category1 = categoryFields(0)
        If categoryFields.Count > 1 Then result.Category2 = categoryFields(1)
        If numericFields.Count > 0 Then result.Value1 = numericFields(0)
        If numericFields.Count > 1 Then result.Value2 = numericFields(1)

        If result.Category1 = "" AndAlso fields.Count > 0 Then result.Category1 = fields(0)
        If result.Category2 = "" AndAlso fields.Count > 1 AndAlso fields(1) <> result.Category1 Then result.Category2 = fields(1)
        If result.Value1 = "" AndAlso fields.Count > 0 Then result.Value1 = fields(fields.Count - 1)
        Return result
    End Function

    Private Function ParseTemplateFields(fieldSet As String) As List(Of String)
        Dim fields As New List(Of String)()
        If fieldSet Is Nothing Then Return fields
        Dim parts As String() = fieldSet.Split(New Char() {","c, ";"c, "|"c, ControlChars.Cr, ControlChars.Lf, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
        For Each part As String In parts
            Dim fieldName As String = part.Trim().Trim(""""c, "'"c)
            If fieldName <> "" AndAlso Not fields.Contains(fieldName) Then fields.Add(fieldName)
        Next
        Return fields
    End Function

    Private Function IsNumericSourceField(fieldName As String) As Boolean
        Dim source As DataTable = GetSourceTableForTemplate()
        If source Is Nothing OrElse fieldName = "" OrElse Not source.Columns.Contains(fieldName) Then Return False
        Dim col As DataColumn = source.Columns(fieldName)
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(Int16) OrElse col.DataType Is GetType(Int32) OrElse col.DataType Is GetType(Int64) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function GetSourceTableForTemplate() As DataTable
        Dim dv As DataView = TryCast(Session("dv3"), DataView)
        If dv IsNot Nothing Then Return dv.Table
        Return TryCast(Session("dataTable"), DataTable)
    End Function

    Private Function NormalizeAggregation(aggregation As String) As String
        Select Case aggregation.Trim().ToLowerInvariant()
            Case "average"
                Return "Avg"
            Case "minimum"
                Return "Min"
            Case "maximum"
                Return "Max"
            Case "standard deviation"
                Return "StDev"
            Case Else
                Return aggregation.Trim()
        End Select
    End Function

    Private Class TemplateFieldSelection
        Public Category1 As String = ""
        Public Category2 As String = ""
        Public Value1 As String = ""
        Public Value2 As String = ""
    End Class

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("SavedAnalysisTemplatesTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        GridViewTemplates.AllowPaging = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        GridViewTemplates.PageSize = AnalysisGridPageSize
        If Not GridViewTemplates.AllowPaging Then GridViewTemplates.PageIndex = 0
        GridViewTemplates.DataSource = dt
        GridViewTemplates.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Saved Analysis Templates (" & If(dt Is Nothing, 0, dt.Rows.Count).ToString() & " templates)"
        AnalysisExportSnapshot.Save(Me, "SavedAnalysisTemplates", "Saved Analysis Templates", LabelInfo, GridViewTemplates, dt)
    End Sub

    Private Function CurrentGrid() As GridView
        Return GridViewTemplates
    End Function

    Private Function SourceRecordCount() As Integer
        Dim dv As DataView = TryCast(Session("dv3"), DataView)
        If dv IsNot Nothing Then Return dv.Count
        Dim source As DataTable = TryCast(Session("dataTable"), DataTable)
        If source IsNot Nothing Then Return source.Rows.Count
        Return 0
    End Function

    Private Function RowContains(row As DataRow, text As String) As Boolean
        Dim needle As String = text.ToLowerInvariant()
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).ToLowerInvariant().Contains(needle) Then Return True
        Next
        Return False
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("SavedAnalysisTemplatesTable"), DataTable)
        If dt Is Nothing Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("SavedAnalysisTemplatesTable"), DataTable)
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "SavedAnalysisTemplates_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Saved Analysis Templates", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Saved Analysis Templates", ""))
        End If
        Response.End()
    End Sub

    Private Function RegisterAnalysisFilter(filterExpression As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("SavedAnalysisTemplateFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("SavedAnalysisTemplateFilters") = filters
        Return filterId
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Template Name identifies the reusable analysis setup.", "Analysis Page records where the template should be used.", "Field Set, Filters, Thresholds, Aggregation, and Notes document the selected controls and business assumptions.", "Search filters saved templates by any visible text.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "The page stores session-level analytical templates rather than report data.", "Each Build click with a Template Name creates one reusable template row.", "Templates preserve control choices so repeated analysis can be documented and reopened consistently.", "The template list can be exported or included in Export Packages as an Excel snapshot.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Template Name and Analysis Page show the saved analytical action.", "Field Set, Filters, Thresholds, Aggregation, and Notes explain how the analysis should be reproduced.", "Records links open the current report records used when the template was saved.", "What Next opens the corresponding analytics page.")
        ReadinessFooterGuidance.SetFooter(Me, "Saved Analysis Templates", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, Nothing)
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

    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso CurrentGrid().PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso CurrentGrid().PageIndex < (CurrentGrid().PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (CurrentGrid().PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & CurrentGrid().PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If CurrentGrid().PageIndex > 0 Then CurrentGrid().PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If CurrentGrid().PageIndex < (CurrentGrid().PageCount - 1) Then CurrentGrid().PageIndex += 1
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
            CurrentGrid().PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
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

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToString("yyyy-MM-dd")
        Return valueObject.ToString()
    End Function
End Class
