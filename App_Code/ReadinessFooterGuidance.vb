Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Module ReadinessFooterGuidance
    Public Sub SetFooter(page As Page, analysisName As String, whyLabel As Label, fieldsLabel As Label, Optional sourceTable As DataTable = Nothing)
        If whyLabel Is Nothing OrElse fieldsLabel Is Nothing Then Return

        whyLabel.Text = ""
        fieldsLabel.Text = ""
        whyLabel.Visible = False
        fieldsLabel.Visible = False

        If page Is Nothing OrElse page.Session Is Nothing Then Return
        If TrySetStoredGuidance(page, analysisName, whyLabel, fieldsLabel) Then Return

        Dim source As DataTable = sourceTable
        If source Is Nothing OrElse source.Rows.Count = 0 Then source = CurrentSource(page)
        If source Is Nothing OrElse source.Columns.Count = 0 Then Return

        Dim whyUseful As String = ""
        Dim suggestedFields As String = ""
        BuildGuidance(source, analysisName, whyUseful, suggestedFields)
        If whyUseful.Trim() = "" AndAlso suggestedFields.Trim() = "" Then Return

        whyLabel.Text = "Why Useful: " & whyUseful
        fieldsLabel.Text = SuggestedFieldsBlock(suggestedFields)
        whyLabel.Visible = True
        fieldsLabel.Visible = True
    End Sub

    Private Function TrySetStoredGuidance(page As Page, analysisName As String, whyLabel As Label, fieldsLabel As Label) As Boolean
        If page.Session("REPORTID") Is Nothing Then Return False

        Dim scannerReportID As String = Convert.ToString(page.Session("DataReadinessScannerReportID")).Trim()
        Dim currentReportID As String = Convert.ToString(page.Session("REPORTID")).Trim()
        If scannerReportID = "" OrElse Not String.Equals(scannerReportID, currentReportID, StringComparison.OrdinalIgnoreCase) Then Return False

        Dim recommendations As DataTable = TryCast(page.Session("DataReadinessScannerAllTable"), DataTable)
        If recommendations Is Nothing OrElse
            Not recommendations.Columns.Contains("Analysis") OrElse
            Not recommendations.Columns.Contains("Why Useful") OrElse
            Not recommendations.Columns.Contains("Suggested Fields") Then Return False

        For Each row As DataRow In recommendations.Rows
            If String.Equals(Convert.ToString(row("Analysis")).Trim(), analysisName.Trim(), StringComparison.OrdinalIgnoreCase) Then
                whyLabel.Text = "Why Useful: " & Convert.ToString(row("Why Useful"))
                fieldsLabel.Text = SuggestedFieldsBlock(Convert.ToString(row("Suggested Fields")))
                whyLabel.Visible = True
                fieldsLabel.Visible = True
                Return True
            End If
        Next
        Return False
    End Function

    Private Function CurrentSource(page As Page) As DataTable
        Dim view As DataView = TryCast(page.Session("dv3"), DataView)
        If view IsNot Nothing AndAlso view.Table IsNot Nothing AndAlso view.Table.Rows.Count > 0 Then Return view.Table

        Dim source As DataTable = TryCast(page.Session("DataReadinessScannerSource"), DataTable)
        If source IsNot Nothing AndAlso source.Rows.Count > 0 Then Return source

        source = TryCast(page.Session("dataTable"), DataTable)
        If source IsNot Nothing AndAlso source.Rows.Count > 0 Then Return source
        Return Nothing
    End Function

    Private Sub BuildGuidance(source As DataTable, analysisName As String, ByRef whyUseful As String, ByRef suggestedFields As String)
        Dim numericFields As List(Of String) = DetectNumericFields(source)
        Dim dateFields As List(Of String) = DetectDateFields(source)
        Dim textFields As List(Of String) = DetectTextFields(source, numericFields, dateFields)
        Dim categoryFields As List(Of String) = DetectCategoryFields(source, textFields)
        Dim idFields As List(Of String) = DetectIdFields(source)
        Dim customerFields As List(Of String) = DetectNamedFields(source, New String() {"customer", "client", "user", "member", "account"})
        Dim orderFields As List(Of String) = DetectNamedFields(source, New String() {"order", "invoice", "transaction", "receipt", "basket"})
        Dim productFields As List(Of String) = DetectNamedFields(source, New String() {"product", "item", "sku", "part", "service"})
        Dim priceFields As List(Of String) = DetectNamedFields(source, New String() {"price", "rate", "fee", "cost"})
        Dim quantityFields As List(Of String) = DetectNamedFields(source, New String() {"quantity", "qty", "units", "volume", "count"})
        Dim revenueFields As List(Of String) = DetectNamedFields(source, New String() {"sales", "revenue", "amount", "total", "profit", "margin"})
        Dim statusFields As List(Of String) = DetectNamedFields(source, New String() {"status", "stage", "step", "result", "outcome", "flag", "churn", "risk"})

        Select Case analysisName
            Case "Data Dictionary"
                whyUseful = "Field-level documentation is useful for any unfamiliar dataset."
                suggestedFields = FieldGuidance("Field Group / Search: use all fields, or narrow to fields whose meaning, type, examples, blanks, or distinct values need documentation", textFields, numericFields, dateFields)
            Case "Saved Analysis Templates"
                whyUseful = "Reusable templates help repeat useful analysis setups without reselecting the same fields, filters, thresholds, and aggregation options."
                suggestedFields = FieldGuidance("Analysis Page: choose the page recommended by Data Readiness Scanner or used in the current workflow", textFields, numericFields, dateFields) & "; Field Set should list the category, date, value, and filter fields that make the template repeatable; Thresholds should record limits used by alerts, anomaly scoring, drift, regression, or market models"
            Case "Automated Analysis Narratives"
                whyUseful = "Narratives summarize what the current data is showing in readable finding/evidence/action rows."
                suggestedFields = FieldGuidance("Primary Field: select category, group, date, status, product, customer, location, or another field to emphasize in the narrative", Prefer(Prefer(categoryFields, dateFields), textFields)) & "; " & FieldGuidance("Value Field: select a numeric measure for narrative evidence where useful", numericFields) & "; Narrative Focus controls whether the output emphasizes summary, quality, field behavior, trends, or exceptions"
            Case "Cross-Report Comparison"
                whyUseful = "Compare the active report with another report by matching common keys and measuring value or count differences."
                suggestedFields = FieldGuidance("Key Field: use a field that exists in both reports, such as product, customer, region, category, location, status, or period", categoryFields, dateFields) & "; " & FieldGuidance("Value Field: use a numeric measure for Sum, Average, Min, or Max comparisons; leave as records for count-only comparison", numericFields) & "; Compare Report ID identifies the second report loaded into memory for comparison"
            Case "Data Profiling"
                whyUseful = "Detect type, blanks, distinct values, min, max, average, and standard deviation."
                suggestedFields = FieldGuidance("Automatic profiling: all fields are scanned; numeric fields receive min/max/average/stdev, date fields receive date ranges, and text/category fields receive blanks, distinct counts, and examples", textFields, numericFields, dateFields)
            Case "Data Quality"
                whyUseful = "Missing values: " & CountMissingValues(source).ToString() & "; duplicate records: " & CountDuplicateRows(source).ToString() & "."
                suggestedFields = FieldGuidance("Date checks: review date-like fields for invalid dates, impossible dates, and missing date values", dateFields) & "; " & FieldGuidance("Numeric checks: review numeric measure fields for out-of-range values, suspicious extremes, and standard-deviation exceptions", numericFields) & "; " & FieldGuidance("Category/Text checks: review text fields for blanks, inconsistent spelling/casing, duplicate-looking categories, and suspicious text values", textFields)
            Case "Ranking Analysis"
                whyUseful = "Category fields and numeric values can be ranked by top, bottom, or average."
                suggestedFields = FieldGuidance("Group Field dropdown: optionally split rankings by category, customer, product, department, location, period, or another dimension", categoryFields) & "; " & FieldGuidance("Value Field dropdown: select the numeric measure to rank, such as sales, revenue, quantity, amount, cost, profit, score, or duration", numericFields) & "; Rank Type dropdown: choose Top, Bottom, or Average, and use Top Count to control how many rows are returned"
            Case "Pivot / Cross Tab"
                whyUseful = "Two category fields can form row and column axes for a cross-tab summary."
                suggestedFields = FieldGuidance("Row Field and Column Field dropdowns: select two category/group fields that form the pivot rows and pivot columns", categoryFields) & "; " & FieldGuidance("Value Field dropdown: select the measure to Count, CountDistinct, Sum, Average, Minimum, Maximum, or Standard Deviation", Prefer(numericFields, textFields)) & "; Aggregation dropdown: choose Count, CountDistinct, Sum, Average, Minimum, Maximum, or Standard Deviation where applicable"
            Case "ABC Pareto Analysis"
                whyUseful = "Find the few categories, products, or customers that explain most of the value."
                suggestedFields = FieldGuidance("Category Field dropdown: select product, customer, category, item, department, region, channel, or another dimension to classify by contribution", Prefer(productFields, categoryFields)) & "; " & FieldGuidance("Value Field dropdown: select sales, revenue, amount, quantity, profit, cost, or another numeric contribution measure", numericFields)
            Case "KPI Builder"
                whyUseful = "Numeric measures can become KPIs, totals, averages, rates, and thresholds."
                suggestedFields = FieldGuidance("Numerator / KPI value fields: select the main numeric measure used to build the KPI", numericFields) & "; " & FieldGuidance("Dimension Field dropdown: optionally group KPI results by category, product, customer, department, location, or period", categoryFields) & "; " & FieldGuidance("Date Field dropdown: optionally select a date field when KPI results should be reviewed by period", dateFields)
            Case "Regression Analysis"
                whyUseful = "Two or more numeric fields can be tested for prediction and fitted equations."
                suggestedFields = FieldGuidance("X Field dropdown: select the numeric driver or independent variable that may explain changes", numericFields) & "; " & FieldGuidance("Y Field dropdown: select the numeric result or dependent variable to explain or predict", numericFields) & "; Equation Type dropdown selects the fitted model, and Predict Y when X is supplies the X value for forecast calculation"
            Case "Correlation Threshold"
                whyUseful = "Filter correlation pairs by minimum strength and focus on the strongest relationships."
                suggestedFields = FieldGuidance("Numeric Fields: select or review numeric measure fields to create correlation pairs; avoid ID/index fields because they usually do not explain business relationships", numericFields) & "; Threshold textbox controls minimum absolute correlation, and View dropdown filters All, Positive, or Negative relationships"
            Case "Variance Analysis"
                whyUseful = "Compare values across groups, periods, or categories."
                suggestedFields = FieldGuidance("Group Field dropdown: select the category, period, location, department, customer, product, or other dimension that becomes each output row", categoryFields) & "; " & FieldGuidance("Value Field and Aggregation dropdowns: select the numeric measure and how records should be summarized before comparison", numericFields) & "; Analysis Type dropdown chooses Variance, Percent Change, or Contribution to Total; for Variance/Percent Change, Compare Field supplies Base Value and Compare Value choices; for Contribution to Total, Base/Compare are not used"
            Case "Comparison Reports"
                whyUseful = "Compare two periods, groups, locations, queries, or imported files."
                suggestedFields = FieldGuidance("Comparison Type dropdown: choose Periods, Groups, Locations, Two Queries, or Two Imported Files; then fill the matching base/compare query, file, period, group, or location controls", categoryFields, dateFields) & "; " & FieldGuidance("Value Field / Aggregation controls: select the numeric measure and summary calculation used to compare base and compare results", numericFields)
            Case "Time Based Summaries"
                whyUseful = "Date and numeric fields support summaries by day, week, month, quarter, and year."
                suggestedFields = FieldGuidance("Date Field dropdown: select the date column used to create day, week, month, quarter, or year periods", dateFields) & "; " & FieldGuidance("Value Field dropdown: select the numeric measure summarized in each period", numericFields) & "; Date Aggregation dropdown can be Day, Week, Month, Quarter, or Year"
            Case "Time Series"
                whyUseful = "Date and value fields support moving averages and rolling totals."
                suggestedFields = FieldGuidance("Date Field dropdown: select the chronological date column used to order periods", dateFields) & "; " & FieldGuidance("Value Field dropdown: select the numeric measure used for period value, moving average, and rolling total", numericFields) & "; Number of time periods textbox controls the rolling window used for moving average and rolling total"
            Case "Data Drift Analysis"
                whyUseful = "Repeated periods can reveal distribution changes across time."
                suggestedFields = FieldGuidance("Date or Segment Field dropdown: select the field that separates base and compare periods or segments for drift review", dateFields) & "; " & FieldGuidance("Numeric/Value fields: use numeric measures when drift should compare value levels or measure distributions", numericFields) & "; " & FieldGuidance("Compare Field dropdown: use category/status/product/channel fields when drift should compare distribution changes", categoryFields)
            Case "Cohort Analysis"
                whyUseful = "Customer or user IDs with dates can be grouped into cohorts."
                suggestedFields = FieldGuidance("Date Field dropdown: select the first-activity or event date used to assign each entity to a cohort period", dateFields) & "; " & FieldGuidance("Entity Field dropdown: select customer, account, user, member, order, device, product, or another identifier followed over time", Prefer(customerFields, idFields)) & "; Value Field is optional and summarizes cohort value when revenue, amount, quantity, or score fields exist"
            Case "Funnel Analysis"
                whyUseful = "Stage/status fields with user/order IDs can show conversion through steps."
                suggestedFields = FieldGuidance("Stage Field dropdown: select the status, stage, outcome, step, workflow, or lifecycle field used as funnel steps", statusFields) & "; " & FieldGuidance("Entity/Record Field dropdown: select customer, order, transaction, account, user, or another identifier counted through stages", customerFields, orderFields, idFields) & "; Date Field is optional and can support event order or period review when stage records include dates"
            Case "Outlier Flagging"
                whyUseful = "Numeric values can be checked for unusual deviations or business-rule exceptions."
                suggestedFields = FieldGuidance("Row Field dropdown: select the row label, category, entity, or record identifier shown for each flagged outlier", categoryFields) & "; " & FieldGuidance("Value Field dropdown: select the numeric measure tested by standard deviation, percent difference, or business rule limits", numericFields) & "; Method dropdown selects Standard Deviation, Percent Difference, or Business Rule, and threshold textboxes control sensitivity"
            Case "Audit Summaries"
                whyUseful = "Document which fields, filters, thresholds, and aggregation options produced each analytical result."
                suggestedFields = FieldGuidance("Audit textboxes: enter the report fields, filters, thresholds, aggregation options, result name, and notes used by the analysis being documented", categoryFields, numericFields, dateFields)
            Case "Market Demand"
                whyUseful = "Demand models need product/category, period, and value or quantity fields."
                suggestedFields = FieldGuidance("Primary Field(s): select product, category, customer, region, channel, location, or market segment used to group demand", Prefer(productFields, categoryFields)) & "; " & FieldGuidance("Date Field / Date Aggregation: select a date field and day/week/month/quarter/year when demand should be summarized by period", dateFields) & "; " & FieldGuidance("Value Field: select units, volume, quantity, sales, revenue, amount, or another demand measure", numericFields)
            Case "Market Pricing"
                whyUseful = "Pricing analysis needs price-like fields and quantity or revenue response fields."
                suggestedFields = FieldGuidance("Value Field: select price, rate, fee, unit price, or another numeric field used to build price bands", priceFields) & "; " & FieldGuidance("Secondary Field: select quantity, units, volume, orders, sales, revenue, or another response measure affected by price", quantityFields, revenueFields) & "; Primary Field is optional: choose (None) for price bands only or choose product/category/customer/location to group price bands by Dimension"
            Case "Market Elasticity"
                whyUseful = "Elasticity needs price variation and quantity or demand response."
                suggestedFields = FieldGuidance("Value Field: select price, rate, fee, unit price, or another numeric price driver", priceFields) & "; " & FieldGuidance("Secondary Field: select quantity, units, volume, demand, or orders used to measure price response", quantityFields) & "; " & FieldGuidance("Primary Field(s): select product, category, customer, region, channel, or segment to calculate separate elasticity rows", productFields, categoryFields)
            Case "Market Basket"
                whyUseful = "Basket analysis needs order/customer identifiers and product or item fields."
                suggestedFields = FieldGuidance("Secondary Field: select order, invoice, transaction, receipt, basket, customer, or session field defining items seen together", orderFields, customerFields) & "; " & FieldGuidance("Primary Field(s): select item, product, SKU, service, or category used to build basket pairs", productFields) & "; Value Field is optional and weights basket value for the matching item pairs"
            Case "Market Segments"
                whyUseful = "Segmentation groups customers, products, or categories by behavior and value."
                suggestedFields = FieldGuidance("Primary Field(s): select customer, product, category, region, channel, department, or combined fields defining each segment", customerFields, categoryFields) & "; " & FieldGuidance("Value Field: select revenue, sales, amount, quantity, score, status count, or another behavior/value measure", numericFields, statusFields)
            Case "Market Churn"
                whyUseful = "Churn needs customer/user fields plus dates or status outcomes."
                suggestedFields = FieldGuidance("Primary Field(s): select customer, account, user, member, client, or segment being scored for churn/retention", customerFields) & "; " & FieldGuidance("Date Field: select last activity, order date, transaction date, service date, or another recency field", dateFields) & "; " & FieldGuidance("Status/Outcome field: use churn, active/inactive, status, result, or outcome fields when available for interpretation", statusFields)
            Case "Market Risk"
                whyUseful = "Risk scoring uses outcome/status fields or multiple numeric risk signals."
                suggestedFields = FieldGuidance("Status/Outcome field: use risk, status, result, flag, default, claim, incident, or outcome fields when available", statusFields) & "; " & FieldGuidance("Value Field / numeric indicators: select exposure, amount, balance, score, loss, count, or other risk-weight fields", numericFields) & "; Primary Field(s) separate risk by customer, product, region, channel, or segment"
            Case "Market Inventory"
                whyUseful = "Inventory movement needs product/category plus quantity, movement, or period fields."
                suggestedFields = FieldGuidance("Primary Field(s): select item, product, SKU, category, location, warehouse, or combined inventory dimension", productFields, categoryFields) & "; " & FieldGuidance("Value Field / Current Inventory: select movement, demand, units, quantity, on-hand, stock, or inventory fields", quantityFields, numericFields) & "; " & FieldGuidance("Date Field / Date Aggregation: select movement date and day/week/month/quarter/year to review inventory by period", dateFields)
            Case "Market Profit"
                whyUseful = "Profit models need revenue, price, cost, margin, or other numeric drivers."
                suggestedFields = FieldGuidance("Value Field: select revenue, sales, amount, price, profit, margin, or another profitability value", revenueFields, priceFields) & "; " & FieldGuidance("Cost/Numeric fields: use direct cost, unit cost, quantity, discount, expense, or other cost-driver fields when present", numericFields) & "; Primary Field(s) group profit by product, customer, region, channel, department, or other driver"
            Case "Market Scenario"
                whyUseful = "Scenario models use numeric assumptions to test possible business changes."
                suggestedFields = FieldGuidance("Value Field: select the current numeric value, revenue, demand, cost, quantity, or score being stressed by the scenario", numericFields) & "; " & FieldGuidance("Primary Field(s): select category, product, customer, region, channel, department, or location to group scenario results", categoryFields) & "; Assumption % textbox creates downside and upside scenario values around the current value"
        End Select
    End Sub

    Private Function SuggestedFieldsBlock(suggestedFields As String) As String
        Dim html As String = "<div class=""suggestedFieldsBlock""><div class=""suggestedFieldsTitle""><strong>Suggested Fields:</strong></div><ul>"
        Dim items As String() = Convert.ToString(suggestedFields).Split(";"c)
        For Each item As String In items
            Dim itemText As String = item.Trim()
            If itemText <> "" Then html &= "<li>" & System.Web.HttpUtility.HtmlEncode(itemText) & "</li>"
        Next
        html &= "</ul></div>"
        Return html
    End Function

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
End Module
