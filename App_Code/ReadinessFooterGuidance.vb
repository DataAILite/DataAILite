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
        fieldsLabel.Text = "Suggested Fields: " & suggestedFields
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
                fieldsLabel.Text = "Suggested Fields: " & Convert.ToString(row("Suggested Fields"))
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
                suggestedFields = FieldGuidance("Fields to document", textFields, numericFields, dateFields)
            Case "Data Profiling"
                whyUseful = "Detect type, blanks, distinct values, min, max, average, and standard deviation."
                suggestedFields = FieldGuidance("Profile all fields; numeric fields get min/max/average/stdev and text fields get blanks/distinct/examples", textFields, numericFields, dateFields)
            Case "Data Quality"
                whyUseful = "Missing values: " & CountMissingValues(source).ToString() & "; duplicate records: " & CountDuplicateRows(source).ToString() & "."
                suggestedFields = FieldGuidance("Check date fields for invalid dates", dateFields) & "; " & FieldGuidance("check numeric fields for out-of-range values", numericFields) & "; " & FieldGuidance("check category/text fields for blanks, inconsistent categories, and suspicious text", textFields)
            Case "Ranking Analysis"
                whyUseful = "Category fields and numeric values can be ranked by top, bottom, or average."
                suggestedFields = FieldGuidance("Group dropdown", categoryFields) & "; " & FieldGuidance("Value field dropdown", numericFields) & "; Rank Type can use Top, Bottom, or Average"
            Case "Pivot / Cross Tab"
                whyUseful = "Two category fields can form row and column axes for a cross-tab summary."
                suggestedFields = FieldGuidance("Row and Column field dropdowns", categoryFields) & "; " & FieldGuidance("Value field dropdown", Prefer(numericFields, textFields)) & "; choose Count, Sum, Average, Minimum, Maximum, or Standard Deviation aggregation where applicable"
            Case "ABC Pareto Analysis"
                whyUseful = "Find the few categories, products, or customers that explain most of the value."
                suggestedFields = FieldGuidance("Category field should be product/customer/category", Prefer(productFields, categoryFields)) & "; " & FieldGuidance("Value field should be sales, revenue, amount, quantity, profit, or another numeric measure", numericFields)
            Case "KPI Builder"
                whyUseful = "Numeric measures can become KPIs, totals, averages, rates, and thresholds."
                suggestedFields = FieldGuidance("KPI value fields", numericFields) & "; " & FieldGuidance("optional group/category fields", categoryFields) & "; " & FieldGuidance("optional date field for period KPIs", dateFields)
            Case "Regression Analysis"
                whyUseful = "Two or more numeric fields can be tested for prediction and fitted equations."
                suggestedFields = FieldGuidance("X Field should be the possible driver", numericFields) & "; " & FieldGuidance("Y Field should be the value to explain or predict", numericFields) & "; select equation type and Predict Y when X is for forecasting"
            Case "Correlation Threshold"
                whyUseful = "Filter correlation pairs by minimum strength and focus on the strongest relationships."
                suggestedFields = FieldGuidance("Numeric fields for correlation threshold filtering", numericFields) & "; raise threshold to show only stronger relationships"
            Case "Variance Analysis"
                whyUseful = "Compare values across groups, periods, or categories."
                suggestedFields = FieldGuidance("Row/group field dropdowns", categoryFields) & "; " & FieldGuidance("value field and aggregation dropdowns", numericFields) & "; compare base and comparison categories or periods"
            Case "Comparison Reports"
                whyUseful = "Compare two periods, groups, locations, queries, or imported files."
                suggestedFields = FieldGuidance("Comparison dropdown can use periods, groups, locations, two queries, or two imported files", categoryFields, dateFields) & "; " & FieldGuidance("value fields for differences", numericFields)
            Case "Time Based Summaries"
                whyUseful = "Date and numeric fields support summaries by day, week, month, quarter, and year."
                suggestedFields = FieldGuidance("Date Field dropdown", dateFields) & "; " & FieldGuidance("Value Field dropdown", numericFields) & "; Date Aggregation can be Day, Week, Month, Quarter, or Year"
            Case "Time Series"
                whyUseful = "Date and value fields support moving averages and rolling totals."
                suggestedFields = FieldGuidance("Date Field dropdown", dateFields) & "; " & FieldGuidance("Value Field dropdown", numericFields) & "; Number of time periods controls moving average or rolling total window"
            Case "Data Drift Analysis"
                whyUseful = "Repeated periods can reveal distribution changes across time."
                suggestedFields = FieldGuidance("Date/period field", dateFields) & "; " & FieldGuidance("numeric fields for value drift", numericFields) & "; " & FieldGuidance("category fields for distribution drift", categoryFields)
            Case "Cohort Analysis"
                whyUseful = "Customer or user IDs with dates can be grouped into cohorts."
                suggestedFields = FieldGuidance("Cohort date field", dateFields) & "; " & FieldGuidance("customer/user/entity ID field", Prefer(customerFields, idFields)) & "; optional value field can measure cohort value"
            Case "Funnel Analysis"
                whyUseful = "Stage/status fields with user/order IDs can show conversion through steps."
                suggestedFields = FieldGuidance("Stage/status field", statusFields) & "; " & FieldGuidance("customer/order/entity ID field", customerFields, orderFields, idFields) & "; optional date field can order events"
            Case "Outlier Flagging"
                whyUseful = "Numeric values can be checked for unusual deviations or business-rule exceptions."
                suggestedFields = FieldGuidance("Row/category field", categoryFields) & "; " & FieldGuidance("value field for standard deviation or percent-difference checks", numericFields) & "; threshold controls sensitivity"
            Case "Audit Summaries"
                whyUseful = "Document which fields, filters, thresholds, and aggregation options produced each analytical result."
                suggestedFields = FieldGuidance("Use the fields selected in other analytics pages as audit inputs", categoryFields, numericFields, dateFields)
            Case "Market Demand"
                whyUseful = "Demand models need product/category, period, and value or quantity fields."
                suggestedFields = FieldGuidance("Primary field should be product/category/market segment", Prefer(productFields, categoryFields)) & "; " & FieldGuidance("Date field supports period-based demand", dateFields) & "; " & FieldGuidance("Value field should be units, volume, sales, or revenue", numericFields)
            Case "Market Pricing"
                whyUseful = "Pricing analysis needs price-like fields and quantity or revenue response fields."
                suggestedFields = FieldGuidance("Price field or price-band source", priceFields) & "; " & FieldGuidance("quantity/revenue response field", quantityFields, revenueFields) & "; optional Primary Field groups pricing by product/category/customer"
            Case "Market Elasticity"
                whyUseful = "Elasticity needs price variation and quantity or demand response."
                suggestedFields = FieldGuidance("Price field", priceFields) & "; " & FieldGuidance("quantity/demand field", quantityFields) & "; " & FieldGuidance("product/category field for separate elasticity curves", productFields, categoryFields)
            Case "Market Basket"
                whyUseful = "Basket analysis needs order/customer identifiers and product or item fields."
                suggestedFields = FieldGuidance("Order/customer transaction field", orderFields, customerFields) & "; " & FieldGuidance("item/product field", productFields) & "; optional Value Field weights basket value"
            Case "Market Segments"
                whyUseful = "Segmentation groups customers, products, or categories by behavior and value."
                suggestedFields = FieldGuidance("Primary/customer/category field", customerFields, categoryFields) & "; " & FieldGuidance("value/behavior fields", numericFields, statusFields)
            Case "Market Churn"
                whyUseful = "Churn needs customer/user fields plus dates or status outcomes."
                suggestedFields = FieldGuidance("Customer/user field", customerFields) & "; " & FieldGuidance("date field for activity recency", dateFields) & "; " & FieldGuidance("status/outcome field for churn flags", statusFields)
            Case "Market Risk"
                whyUseful = "Risk scoring uses outcome/status fields or multiple numeric risk signals."
                suggestedFields = FieldGuidance("status/outcome risk field", statusFields) & "; " & FieldGuidance("numeric risk indicators", numericFields) & "; optional group field separates risk by segment"
            Case "Market Inventory"
                whyUseful = "Inventory movement needs product/category plus quantity, movement, or period fields."
                suggestedFields = FieldGuidance("product/category field", productFields, categoryFields) & "; " & FieldGuidance("quantity/current inventory/movement field", quantityFields, numericFields) & "; " & FieldGuidance("date field supports movement by period", dateFields)
            Case "Market Profit"
                whyUseful = "Profit models need revenue, price, cost, margin, or other numeric drivers."
                suggestedFields = FieldGuidance("revenue/price/profit field", revenueFields, priceFields) & "; " & FieldGuidance("cost or numeric driver fields", numericFields) & "; optional category field finds profit drivers"
            Case "Market Scenario"
                whyUseful = "Scenario models use numeric assumptions to test possible business changes."
                suggestedFields = FieldGuidance("numeric assumption fields", numericFields) & "; " & FieldGuidance("category fields restrict or group the scenario", categoryFields) & "; assumption percent changes the scenario result"
        End Select
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
End Module
