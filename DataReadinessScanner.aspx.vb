Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class DataReadinessScanner
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub DataReadinessScanner_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Data Readiness Scanner"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Data%20Readiness"
    End Sub

    Private Sub DataReadinessScanner_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            BuildAndBindAnalysis()
        ElseIf Session("DataReadinessScannerTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("DataReadinessScannerTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim existingView As DataView = TryCast(Session("dv3"), DataView)
        If existingView IsNot Nothing AndAlso existingView.Table IsNot Nothing AndAlso existingView.Table.Rows.Count > 0 Then
            existingView.RowFilter = ""
            Session("DataReadinessScannerSource") = existingView.Table
            Return existingView.Table
        End If
        Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
        If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
            Session("DataReadinessScannerSource") = existingTable
            Return existingTable
        End If
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()
        If repid.Trim() = "" Then
            LabelError.Text = "Report is not selected."
            Return Nothing
        End If
        Dim ret As String = ""
        Try
            Dim dv As DataView = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If ret.Trim() <> "" Then LabelError.Text = ret
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Session("DataReadinessScannerSource") = dv.Table
                Session("dv3") = dv
                Return dv.Table
            End If
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        LabelError.Text = "No data. Run or import report data first."
        Return Nothing
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("DataReadinessScannerSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("DataReadinessScannerSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewDataReadinessScanner.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        GridViewDataReadinessScanner.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkDataReadinessAI_Click(sender As Object, e As EventArgs) Handles lnkDataReadinessAI.Click
        Dim dt As DataTable = TryCast(Session("DataReadinessScannerTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("DataReadinessScannerTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data readiness results to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = PublicGridTable(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Data Readiness Scanner", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this data readiness scan. Explain which analyses are most useful, which fields support them, and what business questions should be answered first.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = LoadReportData()
        Dim output As DataTable = CreateOutputTable()
        If source Is Nothing Then
            Session("DataReadinessScannerAllTable") = output
            Session("DataReadinessScannerReportID") = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString())
            BindAnalysisGrid(output)
            Return
        End If

        Dim numericFields As List(Of String) = DetectNumericFields(source)
        Dim dateFields As List(Of String) = DetectDateFields(source)
        Dim textFields As List(Of String) = DetectTextFields(source, numericFields, dateFields)
        Dim categoryFields As List(Of String) = DetectCategoryFields(source, textFields)
        Dim idFields As List(Of String) = DetectIdFields(source)
        Dim customerFields As List(Of String) = DetectNamedFields(source, New String() {"customer", "client", "user", "member", "account"})
        Dim orderFields As List(Of String) = DetectNamedFields(source, New String() {"order", "invoice", "transaction", "receipt", "basket"})
        Dim productFields As List(Of String) = DetectNamedFields(source, New String() {"product", "item", "sku", "part", "service"})
        Dim locationFields As List(Of String) = DetectNamedFields(source, New String() {"country", "state", "city", "zip", "postal", "region", "location", "address"})
        Dim latitudeFields As List(Of String) = DetectNamedFields(source, New String() {"latitude", "lat"})
        Dim longitudeFields As List(Of String) = DetectNamedFields(source, New String() {"longitude", "lon", "lng"})
        Dim priceFields As List(Of String) = DetectNamedFields(source, New String() {"price", "rate", "fee", "cost"})
        Dim quantityFields As List(Of String) = DetectNamedFields(source, New String() {"quantity", "qty", "units", "volume", "count"})
        Dim revenueFields As List(Of String) = DetectNamedFields(source, New String() {"sales", "revenue", "amount", "total", "profit", "margin"})
        Dim statusFields As List(Of String) = DetectNamedFields(source, New String() {"status", "stage", "step", "result", "outcome", "flag", "churn", "risk"})
        Dim missingCount As Integer = CountMissingValues(source)
        Dim duplicateRows As Integer = CountDuplicateRows(source)
        Dim rowCount As Integer = source.Rows.Count

        AddRecommendation(output, "Detail Analytics", ScoreAny(source), "Overall field combinations and report exploration.", FieldGuidance("Category/Group 1 and 2 dropdowns", categoryFields) & "; " & FieldGuidance("Argument Y / value dropdown", numericFields) & "; " & FieldGuidance("optional Field2 for correlation, charts, and advanced analytics", Prefer(numericFields, textFields)), "Analytics.aspx", rowCount)
        AddRecommendation(output, "Data Overall Statistics", ScoreAny(source), "Whole-dataset statistics summarize every field before choosing deeper analysis.", FieldGuidance("Use numeric fields for min/max/average/stdev", numericFields) & "; " & FieldGuidance("use text fields for count/distinct count", textFields), "ShowReport.aspx?srd=8", rowCount)
        AddRecommendation(output, "Groups Statistics", If(categoryFields.Count > 0 AndAlso (numericFields.Count > 0 OrElse textFields.Count > 0), 92, 45), "Group statistics summarize measures by category/group fields for groups defined in Report Format Definition page.", FieldGuidance("Group field dropdowns", categoryFields) & "; " & FieldGuidance("value/statistics fields", Prefer(numericFields, textFields)) & "; choose aggregation such as Count, CountDistinct, Sum, Avg, Min, Max, or StDev where available", "ReportViews.aspx?grpstats=yes", rowCount)
        AddRecommendation(output, "Fields Correlation", If(numericFields.Count >= 2, 90, 25), "Numeric fields can be compared to discover strong positive or negative relationships.", FieldGuidance("Select two or more numeric fields for correlation pairs", numericFields) & "; avoid IDs and index-like fields because they do not usually explain business relationships", "ShowReport.aspx?srd=12", rowCount)
        AddRecommendation(output, "Data Dictionary", ScoreAny(source), "Field-level documentation is useful for any unfamiliar dataset.", FieldGuidance("Fields to document", textFields, numericFields, dateFields), "DataDictionary.aspx", rowCount)
        AddRecommendation(output, "Data Profiling", ScoreAny(source), "Detect type, blanks, distinct values, min, max, average, and standard deviation.", FieldGuidance("Profile all fields; numeric fields get min/max/average/stdev and text fields get blanks/distinct/examples", textFields, numericFields, dateFields), "Profiling.aspx", rowCount)
        AddRecommendation(output, "Data Quality", If(missingCount > 0 OrElse duplicateRows > 0, 95, 70), "Missing values: " & missingCount.ToString() & "; duplicate records: " & duplicateRows.ToString() & ".", FieldGuidance("Check date fields for invalid dates", dateFields) & "; " & FieldGuidance("check numeric fields for out-of-range values", numericFields) & "; " & FieldGuidance("check category/text fields for blanks, inconsistent categories, and suspicious text", textFields), "DataQuality.aspx", rowCount)
        AddRecommendation(output, "Ranking Analysis", If(categoryFields.Count > 0 AndAlso numericFields.Count > 0, 92, 45), "Category fields and numeric values can be ranked by top, bottom, or average.", FieldGuidance("Group dropdown", categoryFields) & "; " & FieldGuidance("Value field dropdown", numericFields) & "; Rank Type can use Top, Bottom, or Average", "Ranking.aspx", rowCount)
        AddRecommendation(output, "Pivot / Cross Tab", If(categoryFields.Count >= 2 AndAlso (numericFields.Count > 0 OrElse textFields.Count > 0), 90, 35), "Two category fields can form row and column axes for a cross-tab summary.", FieldGuidance("Row and Column field dropdowns", categoryFields) & "; " & FieldGuidance("Value field dropdown", Prefer(numericFields, textFields)) & "; choose Count, Sum, Average, Minimum, Maximum, or Standard Deviation aggregation where applicable", "Pivot.aspx", rowCount)
        AddRecommendation(output, "ABC Pareto Analysis", If((categoryFields.Count > 0 OrElse productFields.Count > 0) AndAlso numericFields.Count > 0, 90, 35), "Find the few categories, products, or customers that explain most of the value.", FieldGuidance("Category field should be product/customer/category", Prefer(productFields, categoryFields)) & "; " & FieldGuidance("Value field should be sales, revenue, amount, quantity, profit, or another numeric measure", numericFields), "ABCPareto.aspx", rowCount)
        AddRecommendation(output, "KPI Builder", If(numericFields.Count > 0, 88, 30), "Numeric measures can become KPIs, totals, averages, rates, and thresholds.", FieldGuidance("KPI value fields", numericFields) & "; " & FieldGuidance("optional group/category fields", categoryFields) & "; " & FieldGuidance("optional date field for period KPIs", dateFields), "KPIBuilder.aspx", rowCount)
        AddRecommendation(output, "Regression Analysis", If(numericFields.Count >= 2, 86, 25), "Two or more numeric fields can be tested for prediction and fitted equations.", FieldGuidance("X Field should be the possible driver", numericFields) & "; " & FieldGuidance("Y Field should be the value to explain or predict", numericFields) & "; select equation type and Predict Y when X is for forecasting", "Regression.aspx", rowCount)
        AddRecommendation(output, "Correlation Threshold", If(numericFields.Count >= 2, 83, 20), "Filter correlation pairs by minimum strength and focus on the strongest relationships.", FieldGuidance("Numeric fields for correlation threshold filtering", numericFields) & "; raise threshold to show only stronger relationships", "CorrelationThreshold.aspx", rowCount)
        AddRecommendation(output, "Variance Analysis", If(categoryFields.Count > 0 AndAlso numericFields.Count > 0, 82, 35), "Compare values across groups, periods, or categories.", FieldGuidance("Row/group field dropdowns", categoryFields) & "; " & FieldGuidance("value field and aggregation dropdowns", numericFields) & "; compare base and comparison categories or periods", "Variance.aspx", rowCount)
        AddRecommendation(output, "Comparison Reports", If((categoryFields.Count > 0 OrElse dateFields.Count > 0) AndAlso numericFields.Count > 0, 82, 35), "Compare two periods, groups, locations, queries, or imported files.", FieldGuidance("Comparison dropdown can use periods, groups, locations, two queries, or two imported files", categoryFields, dateFields) & "; " & FieldGuidance("value fields for differences", numericFields), "ComparisonReports.aspx", rowCount)
        AddRecommendation(output, "Time Based Summaries", If(dateFields.Count > 0 AndAlso numericFields.Count > 0, 90, 20), "Date and numeric fields support summaries by day, week, month, quarter, and year.", FieldGuidance("Date Field dropdown", dateFields) & "; " & FieldGuidance("Value Field dropdown", numericFields) & "; Date Aggregation can be Day, Week, Month, Quarter, or Year", "TimeBasedSummaries.aspx", rowCount)
        AddRecommendation(output, "Time Series", If(dateFields.Count > 0 AndAlso numericFields.Count > 0, 88, 20), "Date and value fields support moving averages and rolling totals.", FieldGuidance("Date Field dropdown", dateFields) & "; " & FieldGuidance("Value Field dropdown", numericFields) & "; Number of time periods controls moving average or rolling total window", "TimeSeries.aspx", rowCount)
        AddRecommendation(output, "Data Drift Analysis", If(dateFields.Count > 0 AndAlso (numericFields.Count > 0 OrElse categoryFields.Count > 0), 86, 25), "Repeated periods can reveal distribution changes across time.", FieldGuidance("Date/period field", dateFields) & "; " & FieldGuidance("numeric fields for value drift", numericFields) & "; " & FieldGuidance("category fields for distribution drift", categoryFields), "DataDrift.aspx", rowCount)
        AddRecommendation(output, "Cohort Analysis", If(dateFields.Count > 0 AndAlso (customerFields.Count > 0 OrElse idFields.Count > 0), 84, 20), "Customer or user IDs with dates can be grouped into cohorts.", FieldGuidance("Cohort date field", dateFields) & "; " & FieldGuidance("customer/user/entity ID field", Prefer(customerFields, idFields)) & "; optional value field can measure cohort value", "Cohort.aspx", rowCount)
        AddRecommendation(output, "Funnel Analysis", If(statusFields.Count > 0 AndAlso (customerFields.Count > 0 OrElse orderFields.Count > 0 OrElse idFields.Count > 0), 84, 20), "Stage/status fields with user/order IDs can show conversion through steps.", FieldGuidance("Stage/status field", statusFields) & "; " & FieldGuidance("customer/order/entity ID field", customerFields, orderFields, idFields) & "; optional date field can order events", "Funnel.aspx", rowCount)
        AddRecommendation(output, "Outlier Flagging", If(numericFields.Count > 0, 82, 25), "Numeric values can be checked for unusual deviations or business-rule exceptions.", FieldGuidance("Row/category field", categoryFields) & "; " & FieldGuidance("value field for standard deviation or percent-difference checks", numericFields) & "; threshold controls sensitivity", "OutlierFlagging.aspx", rowCount)
        AddRecommendation(output, "Chart Recommendations", If(numericFields.Count > 0 OrElse categoryFields.Count > 0, 82, 35), "Field patterns can be converted into chart suggestions and dashboards.", FieldGuidance("Category field(s) become chart X/group labels", categoryFields) & "; " & FieldGuidance("Date field supports time charts", dateFields) & "; " & FieldGuidance("Value field(s) become Y measures", numericFields), "ChartRecommendationHelpers.aspx", rowCount)
        AddRecommendation(output, "Map Readiness", If((latitudeFields.Count > 0 AndAlso longitudeFields.Count > 0) OrElse locationFields.Count > 0, 88, 20), "Location or coordinate fields can be checked for map and KML readiness.", FieldGuidance("Latitude field", latitudeFields) & "; " & FieldGuidance("Longitude field", longitudeFields) & "; " & FieldGuidance("location fields can support geocoding or map labels", locationFields), "MapReadines.aspx", rowCount)
        AddRecommendation(output, "Matrix Balancing", If(categoryFields.Count >= 2 AndAlso numericFields.Count >= 1, 80, 30), "Two category dimensions and numeric measures can form matrices for balancing and comparison.", FieldGuidance("Matrix rows and columns by category/group fields", categoryFields) & "; " & FieldGuidance("matrix item/value field", numericFields) & "; optional Field2 supports scenario 2a iterations or target matrix comparisons", "ShowReport.aspx?srd=13", rowCount)
        AddRecommendation(output, "Audit Summaries", ScoreAny(source), "Document which fields, filters, thresholds, and aggregation options produced each analytical result.", FieldGuidance("Use the fields selected in other analytics pages as audit inputs", categoryFields, numericFields, dateFields), "AuditSummaries.aspx", rowCount)
        AddRecommendation(output, "Market Demand", If(dateFields.Count > 0 AndAlso (productFields.Count > 0 OrElse categoryFields.Count > 0) AndAlso numericFields.Count > 0, 82, 25), "Demand models need product/category, period, and value or quantity fields.", FieldGuidance("Primary field should be product/category/market segment", Prefer(productFields, categoryFields)) & "; " & FieldGuidance("Date field supports period-based demand", dateFields) & "; " & FieldGuidance("Value field should be units, volume, sales, or revenue", numericFields), "MarketDemand.aspx", rowCount)
        AddRecommendation(output, "Market Pricing", If(priceFields.Count > 0 AndAlso (quantityFields.Count > 0 OrElse revenueFields.Count > 0), 82, 20), "Pricing analysis needs price-like fields and quantity or revenue response fields.", FieldGuidance("Price field or price-band source", priceFields) & "; " & FieldGuidance("quantity/revenue response field", quantityFields, revenueFields) & "; optional Primary Field groups pricing by product/category/customer", "MarketPricing.aspx", rowCount)
        AddRecommendation(output, "Market Elasticity", If(priceFields.Count > 0 AndAlso quantityFields.Count > 0, 82, 20), "Elasticity needs price variation and quantity or demand response.", FieldGuidance("Price field", priceFields) & "; " & FieldGuidance("quantity/demand field", quantityFields) & "; " & FieldGuidance("product/category field for separate elasticity curves", productFields, categoryFields), "MarketElasticity.aspx", rowCount)
        AddRecommendation(output, "Market Basket", If((orderFields.Count > 0 OrElse customerFields.Count > 0) AndAlso productFields.Count > 0, 82, 15), "Basket analysis needs order/customer identifiers and product or item fields.", FieldGuidance("Order/customer transaction field", orderFields, customerFields) & "; " & FieldGuidance("item/product field", productFields) & "; optional Value Field weights basket value", "MarketBasket.aspx", rowCount)
        AddRecommendation(output, "Market Segments", If((customerFields.Count > 0 OrElse categoryFields.Count > 0) AndAlso (numericFields.Count > 0 OrElse statusFields.Count > 0), 78, 25), "Segmentation groups customers, products, or categories by behavior and value.", FieldGuidance("Primary/customer/category field", customerFields, categoryFields) & "; " & FieldGuidance("value/behavior fields", numericFields, statusFields), "MarketSegments.aspx", rowCount)
        AddRecommendation(output, "Market Churn", If(customerFields.Count > 0 AndAlso (dateFields.Count > 0 OrElse statusFields.Count > 0), 78, 20), "Churn needs customer/user fields plus dates or status outcomes.", FieldGuidance("Customer/user field", customerFields) & "; " & FieldGuidance("date field for activity recency", dateFields) & "; " & FieldGuidance("status/outcome field for churn flags", statusFields), "MarketChurn.aspx", rowCount)
        AddRecommendation(output, "Market Risk", If(statusFields.Count > 0 OrElse numericFields.Count >= 2, 76, 20), "Risk scoring uses outcome/status fields or multiple numeric risk signals.", FieldGuidance("status/outcome risk field", statusFields) & "; " & FieldGuidance("numeric risk indicators", numericFields) & "; optional group field separates risk by segment", "MarketRisk.aspx", rowCount)
        AddRecommendation(output, "Market Inventory", If((productFields.Count > 0 OrElse categoryFields.Count > 0) AndAlso (quantityFields.Count > 0 OrElse dateFields.Count > 0), 78, 20), "Inventory movement needs product/category plus quantity, movement, or period fields.", FieldGuidance("product/category field", productFields, categoryFields) & "; " & FieldGuidance("quantity/current inventory/movement field", quantityFields, numericFields) & "; " & FieldGuidance("date field supports movement by period", dateFields), "MarketInventory.aspx", rowCount)
        AddRecommendation(output, "Market Profit", If((revenueFields.Count > 0 OrElse priceFields.Count > 0) AndAlso numericFields.Count > 0, 78, 20), "Profit models need revenue, price, cost, margin, or other numeric drivers.", FieldGuidance("revenue/price/profit field", revenueFields, priceFields) & "; " & FieldGuidance("cost or numeric driver fields", numericFields) & "; optional category field finds profit drivers", "MarketProfit.aspx", rowCount)
        AddRecommendation(output, "Market Scenario", If(numericFields.Count > 0, 74, 20), "Scenario models use numeric assumptions to test possible business changes.", FieldGuidance("numeric assumption fields", numericFields) & "; " & FieldGuidance("category fields restrict or group the scenario", categoryFields) & "; assumption percent changes the scenario result", "MarketScenario.aspx", rowCount)

        output.DefaultView.Sort = "Score DESC, Analysis ASC"
        Session("DataReadinessScannerAllTable") = output.DefaultView.ToTable()
        Session("DataReadinessScannerReportID") = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString())
        If txtSearch.Text.Trim() <> "" Then output = FilterOutput(output, txtSearch.Text.Trim())
        output.DefaultView.Sort = "Score DESC, Analysis ASC"
        BindAnalysisGrid(output.DefaultView.ToTable())
    End Sub

    Private Function CreateOutputTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Analysis", GetType(String))
        dt.Columns.Add("Readiness", GetType(String))
        dt.Columns.Add("Score", GetType(Integer))
        dt.Columns.Add("Why Useful", GetType(String))
        dt.Columns.Add("Suggested Fields", GetType(String))
        dt.Columns.Add("Open", GetType(String))
        dt.Columns.Add("What Next", GetType(String))
        dt.Columns.Add("Records", GetType(Integer))
        dt.Columns.Add("FilterId", GetType(String))
        Return dt
    End Function

    Private Sub AddRecommendation(output As DataTable, analysis As String, score As Integer, whyUseful As String, suggestedFields As String, pageUrl As String, records As Integer)
        score = Math.Max(0, Math.Min(100, score))
        Dim row As DataRow = output.NewRow()
        row("Analysis") = analysis
        row("Readiness") = ReadinessName(score)
        row("Score") = score
        row("Why Useful") = whyUseful
        row("Suggested Fields") = If(suggestedFields.Trim() = "", "No strong matching fields detected", suggestedFields)
        row("Open") = pageUrl
        row("What Next") = WhatNextLinks(analysis)
        row("Records") = records
        row("FilterId") = RegisterAnalysisFilter("1 = 1")
        output.Rows.Add(row)
    End Sub

    Private Function WhatNextLinks(analysis As String) As String
        Select Case analysis
            Case "Detail Analytics"
                Return "DataAdmin.aspx|Analytics Dashboard;ChartRecommendationHelpers.aspx|Chart Recommendations"
            Case "Data Overall Statistics"
                Return "Profiling.aspx|Data Profiling;DataQuality.aspx|Data Quality"
            Case "Groups Statistics"
                Return "Ranking.aspx|Ranking Analysis;Pivot.aspx|Pivot / Cross Tab"
            Case "Fields Correlation"
                Return "CorrelationThreshold.aspx|Correlation Threshold;Regression.aspx|Regression Analysis"
            Case "Data Dictionary"
                Return "Profiling.aspx|Data Profiling;DataReadinessScanner.aspx|Data Readiness Scanner"
            Case "Data Profiling"
                Return "DataQuality.aspx|Data Quality;DataDictionary.aspx|Data Dictionary"
            Case "Data Quality"
                Return "Profiling.aspx|Data Profiling;DataDictionary.aspx|Data Dictionary"
            Case "Ranking Analysis"
                Return "ABCPareto.aspx|ABC Pareto Analysis;MarketSegments.aspx|Market Segments"
            Case "Pivot / Cross Tab"
                Return "Variance.aspx|Variance Analysis;Ranking.aspx|Ranking Analysis"
            Case "ABC Pareto Analysis"
                Return "Ranking.aspx|Ranking Analysis;MarketProfit.aspx|Market Profit"
            Case "KPI Builder"
                Return "Variance.aspx|Variance Analysis;TimeBasedSummaries.aspx|Time Based Summaries"
            Case "Regression Analysis"
                Return "Trends.aspx|Trends;CorrelationThreshold.aspx|Correlation Threshold"
            Case "Correlation Threshold"
                Return "Regression.aspx|Regression Analysis;ChartRecommendationHelpers.aspx|Chart Recommendations"
            Case "Variance Analysis"
                Return "ComparisonReports.aspx|Comparison Reports;TimeBasedSummaries.aspx|Time Based Summaries"
            Case "Comparison Reports"
                Return "Variance.aspx|Variance Analysis;DataDrift.aspx|Data Drift Analysis"
            Case "Time Based Summaries"
                Return "TimeSeries.aspx|Time Series;DataDrift.aspx|Data Drift Analysis"
            Case "Time Series"
                Return "TimeBasedSummaries.aspx|Time Based Summaries;OutlierFlagging.aspx|Outlier Flagging"
            Case "Data Drift Analysis"
                Return "ComparisonReports.aspx|Comparison Reports;TimeSeries.aspx|Time Series"
            Case "Cohort Analysis"
                Return "MarketChurn.aspx|Market Churn;TimeSeries.aspx|Time Series"
            Case "Funnel Analysis"
                Return "MarketChurn.aspx|Market Churn;MarketSegments.aspx|Market Segments"
            Case "Outlier Flagging"
                Return "DataQuality.aspx|Data Quality;Regression.aspx|Regression Analysis"
            Case "Chart Recommendations"
                Return "DataAdmin.aspx|Analytics Dashboard;MarketAdmin.aspx|Market Dashboard"
            Case "Map Readiness"
                Return "MapReport.aspx|Map Report;DataQuality.aspx|Data Quality"
            Case "Matrix Balancing"
                Return "Variance.aspx|Variance Analysis;ComparisonReports.aspx|Comparison Reports"
            Case "Audit Summaries"
                Return "Analytics.aspx|Detail Analytics;DataDictionary.aspx|Data Dictionary"
            Case "Market Demand"
                Return "MarketInventory.aspx|Market Inventory;MarketScenario.aspx|Market Scenario"
            Case "Market Pricing"
                Return "MarketElasticity.aspx|Market Elasticity;MarketProfit.aspx|Market Profit"
            Case "Market Elasticity"
                Return "MarketPricing.aspx|Market Pricing;MarketScenario.aspx|Market Scenario"
            Case "Market Basket"
                Return "MarketSegments.aspx|Market Segments;MarketProfit.aspx|Market Profit"
            Case "Market Segments"
                Return "MarketChurn.aspx|Market Churn;MarketProfit.aspx|Market Profit"
            Case "Market Churn"
                Return "MarketSegments.aspx|Market Segments;MarketRisk.aspx|Market Risk"
            Case "Market Risk"
                Return "MarketChurn.aspx|Market Churn;MarketScenario.aspx|Market Scenario"
            Case "Market Inventory"
                Return "MarketDemand.aspx|Market Demand;MarketScenario.aspx|Market Scenario"
            Case "Market Profit"
                Return "MarketPricing.aspx|Market Pricing;MarketScenario.aspx|Market Scenario"
            Case "Market Scenario"
                Return "MarketDemand.aspx|Market Demand;MarketProfit.aspx|Market Profit"
            Case Else
                Return "DataAdmin.aspx|Analytics Dashboard;MarketAdmin.aspx|Market Dashboard"
        End Select
    End Function

    Private Function ReadinessName(score As Integer) As String
        If score >= 80 Then Return "High"
        If score >= 50 Then Return "Possible"
        Return "Low"
    End Function

    Private Function ScoreAny(source As DataTable) As Integer
        If source Is Nothing OrElse source.Rows.Count = 0 OrElse source.Columns.Count = 0 Then Return 0
        Return 95
    End Function

    Private Function FilterOutput(source As DataTable, searchText As String) As DataTable
        Dim filtered As DataTable = source.Clone()
        Dim needle As String = searchText.ToLowerInvariant()
        For Each row As DataRow In source.Rows
            If FieldText(row("Analysis")).ToLowerInvariant().Contains(needle) OrElse FieldText(row("Why Useful")).ToLowerInvariant().Contains(needle) OrElse FieldText(row("Suggested Fields")).ToLowerInvariant().Contains(needle) Then
                filtered.ImportRow(row)
            End If
        Next
        Return filtered
    End Function

    Private Sub GridViewDataReadinessScanner_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewDataReadinessScanner.RowDataBound
        Dim dt As DataTable = TryCast(Session("DataReadinessScannerTable"), DataTable)
        If dt Is Nothing OrElse e.Row.Cells.Count = 0 Then Exit Sub
        HideColumn(dt, e.Row, "FilterId")
        WrapColumn(dt, e.Row, "Suggested Fields")
        WrapColumn(dt, e.Row, "What Next")
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddOpenLink(e.Row, dt)
        AddWhatNextLinks(e.Row, dt)
        AddRecordLink(e.Row, dt, "Records", "FilterId", "readinessfilter")
    End Sub

    Private Sub AddOpenLink(row As GridViewRow, dt As DataTable)
        If Not dt.Columns.Contains("Open") Then Exit Sub
        Dim idx As Integer = dt.Columns.IndexOf("Open")
        If idx < 0 OrElse idx >= row.Cells.Count Then Exit Sub
        Dim pageUrl As String = row.Cells(idx).Text.Replace("&nbsp;", "").Trim()
        If pageUrl = "" Then Exit Sub
        Dim link As New HyperLink()
        link.Text = "open"
        link.NavigateUrl = AddReportParameter(pageUrl)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open the recommended analysis page."
        row.Cells(idx).Controls.Clear()
        row.Cells(idx).Controls.Add(link)
    End Sub

    Private Function AddReportParameter(pageUrl As String) As String
        If Session("REPORTID") Is Nothing OrElse Session("REPORTID").ToString().Trim() = "" Then Return pageUrl
        If pageUrl.Contains("?") Then Return pageUrl & "&Report=" & Server.UrlEncode(Session("REPORTID").ToString())
        Return pageUrl & "?Report=" & Server.UrlEncode(Session("REPORTID").ToString())
    End Function

    Private Sub AddWhatNextLinks(row As GridViewRow, dt As DataTable)
        If Not dt.Columns.Contains("What Next") Then Exit Sub
        Dim idx As Integer = dt.Columns.IndexOf("What Next")
        If idx < 0 OrElse idx >= row.Cells.Count Then Exit Sub
        Dim suggestedPages As String = row.Cells(idx).Text.Replace("&nbsp;", "").Trim()
        If suggestedPages = "" Then Exit Sub
        row.Cells(idx).Controls.Clear()
        Dim firstLink As Boolean = True
        For Each recommendation As String In suggestedPages.Split(";"c)
            Dim parts() As String = recommendation.Split("|"c)
            If parts.Length <> 2 OrElse parts(0).Trim() = "" OrElse parts(1).Trim() = "" Then Continue For
            If Not firstLink Then row.Cells(idx).Controls.Add(New LiteralControl(" | "))
            Dim link As New HyperLink()
            link.Text = parts(1).Trim()
            link.NavigateUrl = AddReportParameter(parts(0).Trim())
            link.CssClass = "NodeStyle"
            link.ToolTip = "Open a highly recommended follow-up page."
            row.Cells(idx).Controls.Add(link)
            firstLink = False
        Next
    End Sub

    Private Sub HideColumn(dt As DataTable, row As GridViewRow, columnName As String)
        If dt.Columns.Contains(columnName) Then
            Dim idx As Integer = dt.Columns.IndexOf(columnName)
            If idx >= 0 AndAlso idx < row.Cells.Count Then row.Cells(idx).Visible = False
        End If
    End Sub

    Private Sub WrapColumn(dt As DataTable, row As GridViewRow, columnName As String)
        If Not dt.Columns.Contains(columnName) Then Exit Sub
        Dim idx As Integer = dt.Columns.IndexOf(columnName)
        If idx < 0 OrElse idx >= row.Cells.Count Then Exit Sub
        row.Cells(idx).Style("white-space") = "normal"
        row.Cells(idx).Style("word-break") = "normal"
        row.Cells(idx).Style("overflow-wrap") = "break-word"
        row.Cells(idx).Style("min-width") = "320px"
        row.Cells(idx).Style("max-width") = "560px"
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

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("DataReadinessScannerTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        GridViewDataReadinessScanner.AllowPaging = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        GridViewDataReadinessScanner.PageSize = AnalysisGridPageSize
        If Not GridViewDataReadinessScanner.AllowPaging Then GridViewDataReadinessScanner.PageIndex = 0
        GridViewDataReadinessScanner.DataSource = dt
        GridViewDataReadinessScanner.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Data Readiness Scanner (" & If(dt Is Nothing, 0, dt.Rows.Count).ToString() & " recommendations)"
        PanelDashboardRecommendation.Visible = dt IsNot Nothing AndAlso dt.Rows.Count > 0
        HyperLinkAnalyticsDashboardRecommendation.NavigateUrl = AddReportParameter("DataAdmin.aspx")
        HyperLinkMarketDashboardRecommendation.NavigateUrl = AddReportParameter("MarketAdmin.aspx")
        BindWorkflow(dt)
    End Sub

    Private Sub BindWorkflow(displayTable As DataTable)
        PlaceHolderWorkflow.Controls.Clear()
        PanelWorkflow.Visible = False
        Dim workflowTable As DataTable = TryCast(Session("DataReadinessScannerAllTable"), DataTable)
        If workflowTable Is Nothing Then workflowTable = displayTable
        If workflowTable Is Nothing OrElse Not workflowTable.Columns.Contains("Readiness") OrElse Not workflowTable.Columns.Contains("Analysis") OrElse Not workflowTable.Columns.Contains("Open") OrElse Not workflowTable.Columns.Contains("What Next") Then Exit Sub

        Dim highPages As New Dictionary(Of String, DataRow)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In workflowTable.Rows
            If String.Equals(FieldText(row("Readiness")).Trim(), "High", StringComparison.OrdinalIgnoreCase) Then
                Dim analysisName As String = FieldText(row("Analysis")).Trim()
                If analysisName <> "" AndAlso Not highPages.ContainsKey(analysisName) Then highPages.Add(analysisName, row)
            End If
        Next
        If highPages.Count = 0 Then Exit Sub

        Dim expanded As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim queued As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim pendingAlternatives As New List(Of String)()
        If highPages.ContainsKey("Detail Analytics") Then
            AddWorkflowChain(PlaceHolderWorkflow, "Detail Analytics", highPages, expanded, queued, pendingAlternatives)
        End If

        Do While expanded.Count < highPages.Count
            Dim nextStart As String = ""
            Dim isAlternativePath As Boolean = False
            Do While pendingAlternatives.Count > 0 AndAlso nextStart = ""
                Dim alternativeName As String = pendingAlternatives(0)
                pendingAlternatives.RemoveAt(0)
                If Not expanded.ContainsKey(alternativeName) Then
                    nextStart = alternativeName
                    isAlternativePath = True
                End If
            Loop
            If nextStart = "" Then
                For Each row As DataRow In workflowTable.Rows
                    Dim analysisName As String = FieldText(row("Analysis")).Trim()
                    If highPages.ContainsKey(analysisName) AndAlso Not expanded.ContainsKey(analysisName) Then
                        nextStart = analysisName
                        Exit For
                    End If
                Next
            End If
            If nextStart = "" Then Exit Do
            PlaceHolderWorkflow.Controls.Add(New LiteralControl(If(isAlternativePath, "; alternative path: ", "; additional path: ")))
            AddWorkflowChain(PlaceHolderWorkflow, nextStart, highPages, expanded, queued, pendingAlternatives)
        Loop
        PanelWorkflow.Visible = True
    End Sub

    Private Sub AddWorkflowChain(container As System.Web.UI.Control, startName As String, highPages As Dictionary(Of String, DataRow), expanded As Dictionary(Of String, Boolean), queued As Dictionary(Of String, Boolean), pendingAlternatives As List(Of String))
        Dim analysisName As String = startName
        Dim firstStep As Boolean = True
        Do While analysisName <> "" AndAlso highPages.ContainsKey(analysisName) AndAlso Not expanded.ContainsKey(analysisName)
            If Not firstStep Then container.Controls.Add(New LiteralControl(" -> "))
            AddWorkflowLink(container, analysisName, FieldText(highPages(analysisName)("Open")).Trim(), WorkflowToolTip(analysisName, highPages))
            expanded.Add(analysisName, True)

            Dim allNextRecommendations As List(Of String) = WorkflowNextRecommendations(highPages(analysisName))
            Dim nextPages As List(Of String) = HighNextPages(highPages(analysisName), highPages)
            Dim mainNext As String = BestWorkflowContinuation(nextPages, highPages, expanded)
            Dim secondaryRecommendations As New List(Of String)()
            For Each recommendation As String In allNextRecommendations
                Dim parts() As String = recommendation.Split("|"c)
                If parts.Length <> 2 OrElse String.Equals(parts(1).Trim(), mainNext, StringComparison.OrdinalIgnoreCase) Then Continue For
                secondaryRecommendations.Add(recommendation)
            Next
            If secondaryRecommendations.Count > 0 Then
                Dim caption As String = If(mainNext = "", " (connects to: ", " (alternative: ")
                container.Controls.Add(New LiteralControl(caption))
                For i As Integer = 0 To secondaryRecommendations.Count - 1
                    If i > 0 Then container.Controls.Add(New LiteralControl(" / "))
                    Dim parts() As String = secondaryRecommendations(i).Split("|"c)
                    Dim nextUrl As String = parts(0).Trim()
                    Dim nextName As String = parts(1).Trim()
                    AddWorkflowLink(container, nextName, nextUrl, WorkflowToolTip(nextName, highPages))
                    If highPages.ContainsKey(nextName) AndAlso Not expanded.ContainsKey(nextName) AndAlso Not queued.ContainsKey(nextName) Then
                        pendingAlternatives.Add(nextName)
                        queued.Add(nextName, True)
                    End If
                Next
                container.Controls.Add(New LiteralControl(")"))
            End If
            analysisName = mainNext
            firstStep = False
        Loop
    End Sub

    Private Function BestWorkflowContinuation(nextPages As List(Of String), highPages As Dictionary(Of String, DataRow), expanded As Dictionary(Of String, Boolean)) As String
        Dim bestName As String = ""
        Dim bestLength As Integer = 0
        For Each nextName As String In nextPages
            If expanded.ContainsKey(nextName) Then Continue For
            Dim length As Integer = WorkflowContinuationLength(nextName, highPages, expanded, New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase))
            If length > bestLength Then
                bestLength = length
                bestName = nextName
            End If
        Next
        Return bestName
    End Function

    Private Function WorkflowContinuationLength(analysisName As String, highPages As Dictionary(Of String, DataRow), expanded As Dictionary(Of String, Boolean), pathVisited As Dictionary(Of String, Boolean)) As Integer
        If Not highPages.ContainsKey(analysisName) OrElse expanded.ContainsKey(analysisName) OrElse pathVisited.ContainsKey(analysisName) Then Return 0
        Dim visitedWithCurrent As New Dictionary(Of String, Boolean)(pathVisited, StringComparer.OrdinalIgnoreCase)
        visitedWithCurrent.Add(analysisName, True)
        Dim bestLength As Integer = 0
        For Each nextName As String In HighNextPages(highPages(analysisName), highPages)
            bestLength = Math.Max(bestLength, WorkflowContinuationLength(nextName, highPages, expanded, visitedWithCurrent))
        Next
        Return 1 + bestLength
    End Function

    Private Function HighNextPages(row As DataRow, highPages As Dictionary(Of String, DataRow)) As List(Of String)
        Dim nextPages As New List(Of String)()
        For Each recommendation As String In WorkflowNextRecommendations(row)
            Dim parts() As String = recommendation.Split("|"c)
            If parts.Length <> 2 Then Continue For
            Dim nextName As String = parts(1).Trim()
            If highPages.ContainsKey(nextName) AndAlso Not nextPages.Contains(nextName) Then nextPages.Add(nextName)
        Next
        Return nextPages
    End Function

    Private Function WorkflowNextRecommendations(row As DataRow) As List(Of String)
        Dim recommendations As New List(Of String)()
        For Each recommendation As String In FieldText(row("What Next")).Trim().Split(";"c)
            Dim parts() As String = recommendation.Split("|"c)
            If parts.Length <> 2 OrElse parts(0).Trim() = "" OrElse parts(1).Trim() = "" Then Continue For
            Dim normalizedRecommendation As String = parts(0).Trim() & "|" & parts(1).Trim()
            If Not recommendations.Contains(normalizedRecommendation) Then recommendations.Add(normalizedRecommendation)
        Next
        Return recommendations
    End Function

    Private Function WorkflowToolTip(analysisName As String, highPages As Dictionary(Of String, DataRow)) As String
        If highPages.ContainsKey(analysisName) AndAlso highPages(analysisName).Table.Columns.Contains("Why Useful") Then
            Dim whyUseful As String = FieldText(highPages(analysisName)("Why Useful")).Trim()
            If whyUseful <> "" Then Return whyUseful
        End If
        Dim allRecommendations As DataTable = TryCast(Session("DataReadinessScannerAllTable"), DataTable)
        If allRecommendations IsNot Nothing AndAlso allRecommendations.Columns.Contains("Analysis") AndAlso allRecommendations.Columns.Contains("Why Useful") Then
            For Each row As DataRow In allRecommendations.Rows
                If String.Equals(FieldText(row("Analysis")).Trim(), analysisName.Trim(), StringComparison.OrdinalIgnoreCase) Then
                    Dim whyUseful As String = FieldText(row("Why Useful")).Trim()
                    If whyUseful <> "" Then Return whyUseful
                End If
            Next
        End If
        Return "Open " & analysisName & "."
    End Function

    Private Sub AddWorkflowLink(container As System.Web.UI.Control, caption As String, pageUrl As String, toolTipText As String)
        If caption.Trim() = "" OrElse pageUrl.Trim() = "" Then Exit Sub
        Dim link As New HyperLink()
        link.Text = caption
        link.NavigateUrl = AddReportParameter(pageUrl)
        link.CssClass = "NodeStyle"
        link.ToolTip = toolTipText
        container.Controls.Add(link)
    End Sub

    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewDataReadinessScanner.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewDataReadinessScanner.PageIndex < (GridViewDataReadinessScanner.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewDataReadinessScanner.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewDataReadinessScanner.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewDataReadinessScanner.PageIndex > 0 Then GridViewDataReadinessScanner.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewDataReadinessScanner.PageIndex < (GridViewDataReadinessScanner.PageCount - 1) Then GridViewDataReadinessScanner.PageIndex += 1
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
            GridViewDataReadinessScanner.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("DataReadinessScannerTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then BuildAndBindAnalysis() : dt = TryCast(Session("DataReadinessScannerTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then LabelError.Text = "No data readiness results to export." : Exit Sub
        Dim publicTable As DataTable = PublicGridTable(dt)
        Dim fileName As String = "DataReadinessScanner_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Data Readiness Scanner", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Data Readiness Scanner", ""))
        End If
        Response.End()
    End Sub

    Private Function PublicGridTable(dt As DataTable) As DataTable
        If dt Is Nothing Then Return Nothing
        Dim publicTable As DataTable = dt.Copy()
        If publicTable.Columns.Contains("FilterId") Then publicTable.Columns.Remove("FilterId")
        Return publicTable
    End Function

    Private Function BuildAnalysisQuestion(baseQuestion As String) As String
        SetAnalysisExplanationLabels()
        Dim parts As New List(Of String)()
        parts.Add(baseQuestion)
        parts.Add(LabelAnalysisSubtitle.Text.Trim())
        parts.Add(LabelModelExplanation.Text.Trim())
        parts.Add(LabelAlgorithmExplanation.Text.Trim())
        parts.Add(LabelOutputExplanation.Text.Trim())
        Return String.Join(vbCrLf & vbCrLf, parts.ToArray())
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelAnalysisSubtitle.Text = "Scan the current report or imported dataset and recommend the analytics, market models, charts, maps, and quality checks that are most useful for its fields. The grid is sorted by readiness score assigned by the algorithm."
        LabelModelExplanation.Text = "Model: The readiness scanner treats the dataset as an unknown table and classifies fields as numeric measures, dates, categories, IDs, products, customers, orders, locations, prices, quantities, revenue, and status/outcome fields."
        LabelAlgorithmExplanation.Text = "Algorithm: The page inspects column names, data types, blank counts, duplicate records, distinct values, and field combinations. Each analysis receives a readiness score based on the minimum fields normally needed for that analysis."
        LabelOutputExplanation.Text = "Output: Work Flow is one connected line beginning with Detail Analytics, continuing through the longest High-readiness route and showing every What Next connection from each High-readiness page inline, including destinations with lower readiness scores. Additional paths are appended only when needed to include every High recommendation. The grid shows each recommended analysis, readiness level, score, reason, suggested fields, an open link to the page, highly recommended What Next follow-up pages, and a records link back to Data Explorer."
    End Sub

    Private Function DetectNumericFields(dt As DataTable) As List(Of String)
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If ColumnTypeIsNumeric(col) AndAlso Not IsIndexLike(col.ColumnName) Then fields.Add(col.ColumnName)
        Next
        Return fields
    End Function

    Private Function DetectDateFields(dt As DataTable) As List(Of String)
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If LooksLikeDate(dt, col) AndAlso Not IsIndexLike(col.ColumnName) Then fields.Add(col.ColumnName)
        Next
        Return fields
    End Function

    Private Function DetectTextFields(dt As DataTable, numericFields As List(Of String), dateFields As List(Of String)) As List(Of String)
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If Not numericFields.Contains(col.ColumnName) AndAlso Not dateFields.Contains(col.ColumnName) AndAlso Not IsIndexLike(col.ColumnName) Then fields.Add(col.ColumnName)
        Next
        Return fields
    End Function

    Private Function DetectCategoryFields(dt As DataTable, textFields As List(Of String)) As List(Of String)
        Dim fields As New List(Of String)()
        For Each fieldName As String In textFields
            Dim distinctCount As Integer = CountDistinct(dt, fieldName)
            If distinctCount > 1 AndAlso distinctCount <= Math.Max(50, CInt(Math.Ceiling(dt.Rows.Count * 0.6))) Then fields.Add(fieldName)
        Next
        Return fields
    End Function

    Private Function DetectIdFields(dt As DataTable) As List(Of String)
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If IsIndexLike(col.ColumnName) Then Continue For
            Dim name As String = col.ColumnName.ToLowerInvariant()
            If name = "id" OrElse name.EndsWith("id") OrElse name.Contains(" id") OrElse name.Contains("_id") OrElse name.Contains("number") OrElse name.Contains("no") Then fields.Add(col.ColumnName)
        Next
        Return fields
    End Function

    Private Function DetectNamedFields(dt As DataTable, tokens() As String) As List(Of String)
        Dim fields As New List(Of String)()
        For Each col As DataColumn In dt.Columns
            If IsIndexLike(col.ColumnName) Then Continue For
            Dim name As String = col.ColumnName.ToLowerInvariant()
            For Each token As String In tokens
                If name.Contains(token.ToLowerInvariant()) Then
                    fields.Add(col.ColumnName)
                    Exit For
                End If
            Next
        Next
        Return fields
    End Function

    Private Function CountMissingValues(dt As DataTable) As Integer
        Dim missing As Integer = 0
        For Each row As DataRow In dt.Rows
            For Each col As DataColumn In dt.Columns
                If FieldText(row(col)).Trim() = "" Then missing += 1
            Next
        Next
        Return missing
    End Function

    Private Function CountDuplicateRows(dt As DataTable) As Integer
        Dim seen As New Dictionary(Of String, Boolean)()
        Dim duplicates As Integer = 0
        For Each row As DataRow In dt.Rows
            Dim values As New List(Of String)()
            For Each col As DataColumn In dt.Columns
                values.Add(FieldText(row(col)))
            Next
            Dim key As String = String.Join(Chr(30), values.ToArray())
            If seen.ContainsKey(key) Then duplicates += 1 Else seen(key) = True
        Next
        Return duplicates
    End Function

    Private Function CountDistinct(dt As DataTable, fieldName As String) As Integer
        Dim values As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In dt.Rows
            Dim textValue As String = FieldText(row(fieldName)).Trim()
            If textValue <> "" Then values(textValue) = True
        Next
        Return values.Count
    End Function

    Private Function Prefer(firstList As List(Of String), secondList As List(Of String)) As List(Of String)
        If firstList IsNot Nothing AndAlso firstList.Count > 0 Then Return firstList
        Return secondList
    End Function

    Private Function JoinFields(ParamArray fieldLists() As List(Of String)) As String
        Dim result As New List(Of String)()
        For Each fields As List(Of String) In fieldLists
            If fields Is Nothing Then Continue For
            For Each fieldName As String In fields
                If Not result.Contains(fieldName) Then result.Add(fieldName)
                If result.Count >= 8 Then Exit For
            Next
            If result.Count >= 8 Then Exit For
        Next
        Return String.Join(", ", result.ToArray())
    End Function

    Private Function FieldGuidance(labelText As String, ParamArray fieldLists() As List(Of String)) As String
        Dim fieldsText As String = JoinFields(fieldLists)
        If fieldsText.Trim() = "" Then Return labelText & ": no strong matching fields detected"
        Return labelText & ": " & fieldsText
    End Function

    Private Function IsIndexLike(fieldName As String) As Boolean
        Dim name As String = fieldName.Trim().ToLowerInvariant()
        Return name = "indx" OrElse name = "ind" OrElse name = "inx" OrElse name = "index" OrElse name.StartsWith("indx") OrElse name.StartsWith("index")
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToString("yyyy-MM-dd")
        Return valueObject.ToString()
    End Function

    Private Function ColumnTypeIsNumeric(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function LooksLikeDate(dt As DataTable, col As DataColumn) As Boolean
        If col.DataType Is GetType(DateTime) Then Return True
        Dim checkedValues As Integer = 0
        Dim parsed As Integer = 0
        For i As Integer = 0 To Math.Min(30, dt.Rows.Count) - 1
            Dim valueText As String = FieldText(dt.Rows(i)(col)).Trim()
            If valueText = "" Then Continue For
            checkedValues += 1
            Dim dateValue As DateTime
            If DateTime.TryParse(valueText, dateValue) Then parsed += 1
        Next
        Return checkedValues > 0 AndAlso parsed >= Math.Max(1, CInt(Math.Ceiling(checkedValues * 0.8)))
    End Function

    Private Function RegisterAnalysisFilter(filterExpression As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("DataReadinessScannerFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("DataReadinessScannerFilters") = filters
        Return filterId
    End Function

End Class
