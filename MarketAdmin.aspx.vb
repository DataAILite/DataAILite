Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web
Imports Microsoft.VisualBasic.FileIO

Partial Class MarketAdmin
    Inherits System.Web.UI.Page

    Private Const PreviewRows As Integer = 5

    Private Sub MarketAdmin_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        EnsureReportTitle()
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()

        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then
            lblHeader.Text = Session("REPTITLE").ToString() & " - Market Dashboard"
            Page.Title = Session("REPTITLE").ToString() & " - Market Dashboard"
        ElseIf Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Market Dashboard"
            Page.Title = Session("REPORTID").ToString() & " - Market Dashboard"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Market%20Dashboard"
    End Sub

    Private Sub EnsureReportTitle()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then Exit Sub
        If Session("REPORTID") Is Nothing OrElse Session("REPORTID").ToString().Trim() = "" Then Exit Sub

        Dim reportTitle As String = ReportTitleFromId(Session("REPORTID").ToString().Trim())
        If reportTitle.Trim() <> "" Then Session("REPTITLE") = reportTitle
    End Sub

    Private Function ReportTitleFromId(reportId As String) As String
        If reportId.Trim() = "" Then Return ""
        Try
            Dim safeReportId As String = reportId.Replace("'", "''")
            Dim dv As DataView = mRecords("SELECT ReportTtl, ReportName FROM OURReportInfo WHERE ReportID='" & safeReportId & "'")
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then Return ""

            Dim row As DataRow = dv.Table.Rows(0)
            Dim title As String = ""
            If dv.Table.Columns.Contains("ReportTtl") Then title = row("ReportTtl").ToString().Trim()
            If title = "" AndAlso dv.Table.Columns.Contains("ReportName") Then title = row("ReportName").ToString().Trim()
            Return title
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub MarketAdmin_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        BindMarketPreviews()
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub BindMarketPreviews()
        Dim source As DataTable = MarketSourceTable()
        Dim suitability As List(Of MarketSuitability) = BuildMarketSuitability(source)
        ApplyMarketMenuSuitability(suitability)
        litMarketSuitability.Text = RenderSuitabilityTable(suitability)

        litPreviewDemand.Text = BuildDemandPreviewHtml(source)
        litPreviewPricing.Text = BuildPricingPreviewHtml(source)
        litPreviewElasticity.Text = BuildElasticityPreviewHtml(source)
        litPreviewBasket.Text = BuildBasketPreviewHtml(source)
        litPreviewSegments.Text = BuildSegmentsPreviewHtml(source)
        litPreviewChurn.Text = BuildChurnPreviewHtml(source)
        litPreviewRisk.Text = BuildRiskPreviewHtml(source)
        litPreviewInventory.Text = BuildInventoryPreviewHtml(source)
        litPreviewProfit.Text = BuildProfitPreviewHtml(source)
        litPreviewScenario.Text = BuildScenarioPreviewHtml(source)
    End Sub

    Private Function BuildMarketSuitability(source As DataTable) As List(Of MarketSuitability)
        Dim items As New List(Of MarketSuitability)()
        Dim anyData As Boolean = HasPreviewData(source)
        If Not anyData Then
            items.Add(NotEnough("Market Demand", "MarketDemand.aspx", "", "Grouping field, numeric value field", "No market data is available."))
            items.Add(NotEnough("Market Pricing", "MarketPricing.aspx", "", "Price field", "No market data is available."))
            items.Add(NotEnough("Market Elasticity", "MarketElasticity.aspx", "", "Grouping field, price field, quantity field", "No market data is available."))
            items.Add(NotEnough("Market Basket", "MarketBasket.aspx", "", "Item field, order/transaction field", "No market data is available."))
            items.Add(NotEnough("Market Segments", "MarketSegments.aspx", "", "Grouping field, numeric value field", "No market data is available."))
            items.Add(NotEnough("Market Churn", "MarketChurn.aspx", "", "Customer/segment field, date field, numeric value field", "No market data is available."))
            items.Add(NotEnough("Market Risk", "MarketRisk.aspx", "", "Grouping field, numeric exposure/value field", "No market data is available."))
            items.Add(NotEnough("Market Inventory", "MarketInventory.aspx", "", "Item/grouping field, movement value field", "No market data is available."))
            items.Add(NotEnough("Market Profit", "MarketProfit.aspx", "", "Grouping field, revenue/value field", "No market data is available."))
            items.Add(NotEnough("Market Scenario", "MarketScenario.aspx", "", "Grouping field, current value field", "No market data is available."))
            Return items
        End If

        Dim groupCol As DataColumn = PreferredTextColumn(source, New String() {"product", "category", "region", "customer", "segment", "location"})
        Dim productCol As DataColumn = PreferredTextColumn(source, New String() {"product", "item", "category"})
        Dim customerCol As DataColumn = PreferredTextColumn(source, New String() {"customer", "segment", "account", "client"})
        Dim orderCol As DataColumn = PreferredTextColumn(source, New String() {"order", "invoice", "transaction"})
        Dim valueCol As DataColumn = PreferredNumericColumn(source, New String() {"sales", "revenue", "amount", "total", "quantity", "value"})
        Dim priceCol As DataColumn = PreferredNumericColumn(source, New String() {"price", "unit price", "price per unit"})
        Dim qtyCol As DataColumn = PreferredNumericColumn(source, New String() {"quantity", "qty", "units", "volume"})
        Dim dateCol As DataColumn = PreferredDateColumn(source)
        Dim stockCol As DataColumn = PreferredNumericColumn(source, New String() {"inventory", "stock", "on hand", "available", "balance"})
        Dim costCol As DataColumn = PreferredNumericColumn(source, New String() {"total cost", "extended cost", "cost amount", "direct cost", "unit cost", "cost per unit", "expense"})

        items.Add(Assess("Market Demand", "MarketDemand.aspx", New DataColumn() {groupCol, valueCol}, New String() {"Grouping field", "Numeric demand/value field"}, "", "Use Demand to summarize volume or value by market group."))
        items.Add(Assess("Market Pricing", "MarketPricing.aspx", New DataColumn() {priceCol}, New String() {"Price field"}, OptionalFieldText(qtyCol, "Quantity field"), "Use Pricing when a price field is present; quantity makes sensitivity notes stronger.", "Quantity field"))
        items.Add(Assess("Market Elasticity", "MarketElasticity.aspx", New DataColumn() {groupCol, priceCol, qtyCol}, New String() {"Grouping field", "Price field", "Quantity field"}, "", "Use Elasticity to compare quantity response across price bands."))
        items.Add(AssessBasket(productCol, orderCol))
        items.Add(Assess("Market Segments", "MarketSegments.aspx", New DataColumn() {groupCol, valueCol}, New String() {"Grouping field", "Numeric segment value field"}, "", "Use Segments to compare value and average value by market group."))
        items.Add(Assess("Market Churn", "MarketChurn.aspx", New DataColumn() {customerCol, dateCol, valueCol}, New String() {"Customer/segment field", "Date field", "Numeric value field"}, "", "Use Churn when customer or segment activity dates are available."))
        items.Add(Assess("Market Risk", "MarketRisk.aspx", New DataColumn() {groupCol, valueCol}, New String() {"Grouping field", "Numeric exposure/value field"}, "", "Use Risk to score exposure by market group."))
        items.Add(Assess("Market Inventory", "MarketInventory.aspx", New DataColumn() {productCol, valueCol}, New String() {"Item/grouping field", "Movement value field"}, OptionalFieldText(stockCol, "Current inventory field"), "Use Inventory for movement and reorder review; stock/on-hand data makes reorder columns complete.", "Current inventory field"))
        items.Add(Assess("Market Profit", "MarketProfit.aspx", New DataColumn() {groupCol, valueCol}, New String() {"Grouping field", "Revenue/value field"}, OptionalFieldText(costCol, "Cost field"), "Use Profit for revenue and margin review; cost fields improve estimated cost.", "Cost field"))
        items.Add(Assess("Market Scenario", "MarketScenario.aspx", New DataColumn() {groupCol, valueCol}, New String() {"Grouping field", "Current value field"}, "", "Use Scenario to apply assumption changes to current values."))
        Return items
    End Function

    Private Function Assess(modelName As String, pageUrl As String, requiredFields() As DataColumn, requiredLabels() As String, optionalFound As String, recommendation As String, Optional optionalMissingLabel As String = "") As MarketSuitability
        Dim found As New List(Of String)()
        Dim missing As New List(Of String)()
        For i As Integer = 0 To requiredFields.Length - 1
            If requiredFields(i) Is Nothing Then
                missing.Add(requiredLabels(i))
            Else
                found.Add(requiredLabels(i) & ": " & requiredFields(i).ColumnName)
            End If
        Next
        If optionalFound.Trim() <> "" Then found.Add(optionalFound)

        If missing.Count > 0 Then Return New MarketSuitability(modelName, pageUrl, "Not enough data", False, String.Join("; ", found.ToArray()), String.Join("; ", missing.ToArray()), "Not recommended until missing fields are added to the report.")
        If optionalMissingLabel.Trim() <> "" AndAlso optionalFound.Trim() = "" Then
            Return New MarketSuitability(modelName, pageUrl, "Partial", True, String.Join("; ", found.ToArray()), optionalMissingLabel, recommendation)
        End If
        Return New MarketSuitability(modelName, pageUrl, "Good", True, String.Join("; ", found.ToArray()), "", recommendation)
    End Function

    Private Function AssessBasket(itemCol As DataColumn, orderCol As DataColumn) As MarketSuitability
        Dim found As New List(Of String)()
        Dim missing As New List(Of String)()
        If itemCol Is Nothing Then found.Add("") Else found.Add("Item field: " & itemCol.ColumnName)
        If orderCol Is Nothing OrElse (itemCol IsNot Nothing AndAlso orderCol.ColumnName = itemCol.ColumnName) Then
            missing.Add("Separate order/transaction field")
        Else
            found.Add("Order/transaction field: " & orderCol.ColumnName)
        End If
        If itemCol Is Nothing Then missing.Add("Item/product field")
        If missing.Count > 0 Then Return New MarketSuitability("Market Basket", "MarketBasket.aspx", "Not enough data", False, CleanJoin(found), String.Join("; ", missing.ToArray()), "Not recommended until item and transaction fields are both available.")
        Return New MarketSuitability("Market Basket", "MarketBasket.aspx", "Good", True, CleanJoin(found), "", "Use Basket to find item pairs that occur in the same transaction.")
    End Function

    Private Function OptionalFieldText(col As DataColumn, label As String) As String
        If col Is Nothing Then Return ""
        Return label & ": " & col.ColumnName
    End Function

    Private Function NotEnough(modelName As String, pageUrl As String, found As String, missing As String, recommendation As String) As MarketSuitability
        Return New MarketSuitability(modelName, pageUrl, "Not enough data", False, found, missing, recommendation)
    End Function

    Private Function CleanJoin(values As List(Of String)) As String
        Dim cleaned As New List(Of String)()
        For Each value As String In values
            If value.Trim() <> "" Then cleaned.Add(value)
        Next
        Return String.Join("; ", cleaned.ToArray())
    End Function

    Private Sub ApplyMarketMenuSuitability(items As List(Of MarketSuitability))
        Dim marketNode As System.Web.UI.WebControls.TreeNode = FindTreeNodeByValue(TreeView1.Nodes, "MarketAdmin.aspx")
        If marketNode Is Nothing Then Exit Sub

        Dim allowedPages As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each item As MarketSuitability In items
            If item.Available Then allowedPages(item.PageUrl) = True
        Next

        For i As Integer = marketNode.ChildNodes.Count - 1 To 0 Step -1
            Dim child As System.Web.UI.WebControls.TreeNode = marketNode.ChildNodes(i)
            If Not allowedPages.ContainsKey(child.Value) Then marketNode.ChildNodes.RemoveAt(i)
        Next

        marketNode.Expanded = marketNode.ChildNodes.Count > 0
    End Sub

    Private Function FindTreeNodeByValue(nodes As System.Web.UI.WebControls.TreeNodeCollection, value As String) As System.Web.UI.WebControls.TreeNode
        For Each node As System.Web.UI.WebControls.TreeNode In nodes
            If String.Equals(node.Value, value, StringComparison.OrdinalIgnoreCase) Then Return node
            Dim found As System.Web.UI.WebControls.TreeNode = FindTreeNodeByValue(node.ChildNodes, value)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function

    Private Function RenderSuitabilityTable(items As List(Of MarketSuitability)) As String
        Dim sb As New StringBuilder()
        sb.Append("<table class=""suitabilityTable""><tr><th>Market Page</th><th>Status</th><th>Found Fields</th><th>Missing Fields</th><th>Recommendation</th></tr>")
        For Each item As MarketSuitability In items
            Dim statusClass As String = If(item.Status = "Good", "statusGood", If(item.Status = "Partial", "statusPartial", "statusMissing"))
            sb.Append("<tr>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Name)).Append("</td>")
            sb.Append("<td class=""").Append(statusClass).Append(""">").Append(HttpUtility.HtmlEncode(item.Status)).Append("</td>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.FoundFields)).Append("</td>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.MissingFields)).Append("</td>")
            sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Recommendation)).Append("</td>")
            sb.Append("</tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Function MarketSourceTable() As DataTable
        Dim ret As String = String.Empty
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()

        If repid.Trim() <> "" Then
            Try
                Dim dv As DataView = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
                If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then Return dv.Table
            Catch ex As Exception
            End Try
        End If

        Dim samplePath As String = Server.MapPath("~/SampleData/MarketRetailSales.csv")
        If File.Exists(samplePath) Then Return LoadCsv(samplePath)
        Return Nothing
    End Function

    Private Function LoadCsv(path As String) As DataTable
        Dim dt As New DataTable()
        Using parser As New TextFieldParser(path)
            parser.TextFieldType = FieldType.Delimited
            parser.SetDelimiters(",")
            parser.HasFieldsEnclosedInQuotes = True
            If parser.EndOfData Then Return dt
            Dim headers() As String = parser.ReadFields()
            For Each header As String In headers
                Dim columnName As String = If(header, "").Trim()
                If columnName = "" Then columnName = "Column" & (dt.Columns.Count + 1).ToString()
                If dt.Columns.Contains(columnName) Then columnName &= "_" & dt.Columns.Count.ToString()
                dt.Columns.Add(columnName, GetType(String))
            Next
            While Not parser.EndOfData
                Dim fields() As String = parser.ReadFields()
                Dim row As DataRow = dt.NewRow()
                For i As Integer = 0 To Math.Min(fields.Length, dt.Columns.Count) - 1
                    row(i) = fields(i)
                Next
                dt.Rows.Add(row)
            End While
        End Using
        Return dt
    End Function

    Private Function BuildDemandPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim dimension As DataColumn = PreferredTextColumn(source)
        Dim valueCol As DataColumn = PreferredNumericColumn(source, New String() {"quantity", "sales", "revenue", "amount", "total"})
        If dimension Is Nothing OrElse valueCol Is Nothing Then Return BuildRawPreviewHtml(source)

        Dim buckets As Dictionary(Of String, Bucket) = GroupBuckets(source, dimension, valueCol)
        Dim total As Double = 0
        For Each b As Bucket In buckets.Values
            total += b.Sum
        Next

        Dim rows As New List(Of String())()
        For Each key As String In TopKeysBySum(buckets)
            Dim b As Bucket = buckets(key)
            rows.Add(New String() {key, FormatNumber(b.Sum, 2), b.Count.ToString(), PercentText(b.Sum, total)})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Dimension", "Demand", "Records", "Share %"}, rows)
    End Function

    Private Function BuildPricingPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim priceCol As DataColumn = PreferredNumericColumn(source, New String() {"price", "unit price"})
        Dim qtyCol As DataColumn = PreferredNumericColumn(source, New String() {"quantity", "qty", "units"})
        If priceCol Is Nothing Then Return BuildDemandPreviewHtml(source)

        Dim bands As New Dictionary(Of String, Bucket)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim price As Double = ValueOf(row(priceCol))
            Dim band As String = PriceBand(price)
            If Not bands.ContainsKey(band) Then bands.Add(band, New Bucket())
            bands(band).Count += 1
            bands(band).Sum += price
            If qtyCol IsNot Nothing Then bands(band).SecondSum += ValueOf(row(qtyCol))
        Next

        Dim rows As New List(Of String())()
        For Each key As String In SortedKeys(bands)
            Dim b As Bucket = bands(key)
            rows.Add(New String() {key, b.Count.ToString(), FormatNumber(b.AverageSecond(), 2), FormatNumber(b.Average(), 2)})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Price Band", "Records", "Avg Qty", "Avg Price"}, rows)
    End Function

    Private Function BuildElasticityPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim dimension As DataColumn = PreferredTextColumn(source, New String() {"product", "category", "region", "location"})
        Dim priceCol As DataColumn = PreferredNumericColumn(source, New String() {"price", "unit price"})
        Dim qtyCol As DataColumn = PreferredNumericColumn(source, New String() {"quantity", "qty", "units"})
        If dimension Is Nothing OrElse priceCol Is Nothing OrElse qtyCol Is Nothing Then Return BuildPricingPreviewHtml(source)

        Dim buckets As New Dictionary(Of String, Bucket)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim key As String = FieldText(row(dimension)) & " / " & PriceBand(ValueOf(row(priceCol)))
            If key.Trim() = "/" Then key = "(blank)"
            If Not buckets.ContainsKey(key) Then buckets.Add(key, New Bucket())
            buckets(key).Count += 1
            buckets(key).Sum += ValueOf(row(priceCol))
            buckets(key).SecondSum += ValueOf(row(qtyCol))
        Next

        Dim rows As New List(Of String())()
        For Each key As String In TopKeysBySum(buckets)
            Dim b As Bucket = buckets(key)
            rows.Add(New String() {key, FormatNumber(b.Average(), 2), FormatNumber(b.SecondSum, 2), b.Count.ToString()})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Group / Band", "Avg Price", "Quantity", "Records"}, rows)
    End Function

    Private Function BuildBasketPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim itemCol As DataColumn = PreferredTextColumn(source)
        Dim orderCol As DataColumn = PreferredTextColumn(source, New String() {"order", "invoice", "transaction"})
        If itemCol Is Nothing OrElse orderCol Is Nothing OrElse itemCol.ColumnName = orderCol.ColumnName Then Return BuildRawPreviewHtml(source)

        Dim orderItems As New Dictionary(Of String, Dictionary(Of String, Boolean))(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim orderId As String = FieldText(row(orderCol))
            Dim itemText As String = FieldText(row(itemCol))
            If orderId = "" OrElse itemText = "" Then Continue For
            If Not orderItems.ContainsKey(orderId) Then orderItems.Add(orderId, New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase))
            orderItems(orderId)(itemText) = True
        Next

        Dim pairs As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each items As Dictionary(Of String, Boolean) In orderItems.Values
            Dim names As New List(Of String)(items.Keys)
            names.Sort()
            For i As Integer = 0 To names.Count - 1
                For j As Integer = i + 1 To names.Count - 1
                    Dim key As String = names(i) & ChrW(30) & names(j)
                    If Not pairs.ContainsKey(key) Then pairs.Add(key, 0)
                    pairs(key) += 1
                Next
            Next
        Next

        Dim rows As New List(Of String())()
        For Each key As String In TopKeys(pairs)
            Dim parts() As String = key.Split(ChrW(30))
            rows.Add(New String() {parts(0), parts(1), pairs(key).ToString(), PercentText(pairs(key), Math.Max(1, orderItems.Count))})
            If rows.Count >= PreviewRows Then Exit For
        Next
        If rows.Count = 0 Then Return EmptyPreview("No basket pairs available.")
        Return RenderPreviewTable(New String() {"Item A", "Item B", "Orders", "Support %"}, rows)
    End Function

    Private Function BuildSegmentsPreviewHtml(source As DataTable) As String
        Return BuildGroupedPreviewHtml(source, "Segment", "Value", "Average")
    End Function

    Private Function BuildChurnPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim dimension As DataColumn = PreferredTextColumn(source, New String() {"customer", "segment", "region"})
        Dim valueCol As DataColumn = PreferredNumericColumn(source, New String() {"sales", "revenue", "amount", "quantity"})
        Dim dateCol As DataColumn = PreferredDateColumn(source)
        If dimension Is Nothing OrElse valueCol Is Nothing Then Return BuildGroupedPreviewHtml(source, "Customer", "Value", "Records")

        Dim rows As New List(Of String())()
        Dim buckets As Dictionary(Of String, Bucket) = GroupBuckets(source, dimension, valueCol, dateCol)
        For Each key As String In TopKeysByDate(buckets)
            Dim b As Bucket = buckets(key)
            rows.Add(New String() {key, b.Count.ToString(), If(b.HasDate, b.MaxDate.ToShortDateString(), ""), FormatNumber(b.Sum, 2)})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Customer / Segment", "Records", "Last Activity", "Value"}, rows)
    End Function

    Private Function BuildRiskPreviewHtml(source As DataTable) As String
        Return BuildGroupedPreviewHtml(source, "Dimension", "Value", "Risk Score")
    End Function

    Private Function BuildInventoryPreviewHtml(source As DataTable) As String
        Return BuildGroupedPreviewHtml(source, "Item", "Units", "Velocity")
    End Function

    Private Function BuildProfitPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim dimension As DataColumn = PreferredTextColumn(source)
        Dim valueCol As DataColumn = PreferredNumericColumn(source, New String() {"profit", "sales", "revenue", "amount", "total"})
        If dimension Is Nothing OrElse valueCol Is Nothing Then Return BuildRawPreviewHtml(source)

        Dim rows As New List(Of String())()
        For Each key As String In TopKeysBySum(GroupBuckets(source, dimension, valueCol))
            Dim buckets As Dictionary(Of String, Bucket) = GroupBuckets(source, dimension, valueCol)
            Dim revenue As Double = buckets(key).Sum
            Dim cost As Double = revenue * 0.65
            rows.Add(New String() {key, FormatNumber(revenue, 2), FormatNumber(cost, 2), FormatNumber(revenue - cost, 2)})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Driver", "Revenue", "Cost", "Profit"}, rows)
    End Function

    Private Function BuildScenarioPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim dimension As DataColumn = PreferredTextColumn(source)
        Dim valueCol As DataColumn = PreferredNumericColumn(source, New String() {"sales", "revenue", "amount", "quantity"})
        If dimension Is Nothing OrElse valueCol Is Nothing Then Return BuildRawPreviewHtml(source)

        Dim rows As New List(Of String())()
        Dim buckets As Dictionary(Of String, Bucket) = GroupBuckets(source, dimension, valueCol)
        For Each key As String In TopKeysBySum(buckets)
            Dim currentValue As Double = buckets(key).Sum
            Dim scenarioValue As Double = currentValue * 1.1
            rows.Add(New String() {key, FormatNumber(currentValue, 2), "10.00", FormatNumber(scenarioValue, 2)})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {"Dimension", "Current", "Change %", "Scenario"}, rows)
    End Function

    Private Function BuildGroupedPreviewHtml(source As DataTable, firstHeader As String, secondHeader As String, thirdHeader As String) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim dimension As DataColumn = PreferredTextColumn(source)
        Dim valueCol As DataColumn = PreferredNumericColumn(source, New String() {"sales", "revenue", "amount", "quantity", "profit"})
        If dimension Is Nothing OrElse valueCol Is Nothing Then Return BuildRawPreviewHtml(source)

        Dim rows As New List(Of String())()
        Dim buckets As Dictionary(Of String, Bucket) = GroupBuckets(source, dimension, valueCol)
        Dim maxValue As Double = 1
        For Each b As Bucket In buckets.Values
            If b.Sum > maxValue Then maxValue = b.Sum
        Next

        For Each key As String In TopKeysBySum(buckets)
            Dim b As Bucket = buckets(key)
            rows.Add(New String() {key, b.Count.ToString(), FormatNumber(b.Sum, 2), FormatNumber((b.Sum / maxValue) * 100, 1)})
            If rows.Count >= PreviewRows Then Exit For
        Next
        Return RenderPreviewTable(New String() {firstHeader, "Records", secondHeader, thirdHeader}, rows)
    End Function

    Private Function GroupBuckets(source As DataTable, dimension As DataColumn, valueCol As DataColumn, Optional dateCol As DataColumn = Nothing) As Dictionary(Of String, Bucket)
        Dim buckets As New Dictionary(Of String, Bucket)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim key As String = FieldText(row(dimension))
            If key = "" Then key = "(blank)"
            If Not buckets.ContainsKey(key) Then buckets.Add(key, New Bucket())
            buckets(key).Count += 1
            buckets(key).Sum += ValueOf(row(valueCol))
            If dateCol IsNot Nothing Then
                Dim dateValue As DateTime
                If DateTime.TryParse(FieldText(row(dateCol)), dateValue) Then
                    If Not buckets(key).HasDate OrElse dateValue > buckets(key).MaxDate Then buckets(key).MaxDate = dateValue
                    buckets(key).HasDate = True
                End If
            End If
        Next
        Return buckets
    End Function

    Private Function BuildRawPreviewHtml(source As DataTable) As String
        If Not HasPreviewData(source) Then Return EmptyPreview("No market data available.")
        Dim headers As New List(Of String)()
        For i As Integer = 0 To Math.Min(4, source.Columns.Count) - 1
            headers.Add(source.Columns(i).ColumnName)
        Next

        Dim rows As New List(Of String())()
        For i As Integer = 0 To Math.Min(PreviewRows, source.Rows.Count) - 1
            Dim values As New List(Of String)()
            For Each header As String In headers
                values.Add(FieldText(source.Rows(i)(header)))
            Next
            rows.Add(values.ToArray())
        Next
        Return RenderPreviewTable(headers.ToArray(), rows)
    End Function

    Private Function RenderPreviewTable(headers() As String, rows As List(Of String())) As String
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
                Dim valueText As String = If(i < row.Length, row(i), "")
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(valueText)).Append("</td>")
            Next
            sb.Append("</tr>")
        Next
        sb.Append("</table>")
        Return sb.ToString()
    End Function

    Private Function EmptyPreview(message As String) As String
        Return "<span class=""previewEmpty"">" & HttpUtility.HtmlEncode(message) & "</span>"
    End Function

    Private Function HasPreviewData(source As DataTable) As Boolean
        Return source IsNot Nothing AndAlso source.Rows.Count > 0 AndAlso source.Columns.Count > 0
    End Function

    Private Function PreferredTextColumn(source As DataTable, Optional hints() As String = Nothing) As DataColumn
        If source Is Nothing Then Return Nothing
        If hints IsNot Nothing Then
            For Each hint As String In hints
                For Each col As DataColumn In source.Columns
                    If col.ColumnName.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0 Then Return col
                Next
            Next
        End If
        For Each col As DataColumn In source.Columns
            If Not IsNumericColumn(source, col) AndAlso Not IsDateColumn(source, col) Then Return col
        Next
        If source.Columns.Count > 0 Then Return source.Columns(0)
        Return Nothing
    End Function

    Private Function PreferredNumericColumn(source As DataTable, hints() As String) As DataColumn
        If source Is Nothing Then Return Nothing
        For Each hint As String In hints
            For Each col As DataColumn In source.Columns
                If col.ColumnName.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0 AndAlso IsNumericColumn(source, col) Then Return col
            Next
        Next
        For Each col As DataColumn In source.Columns
            If IsNumericColumn(source, col) Then Return col
        Next
        Return Nothing
    End Function

    Private Function PreferredDateColumn(source As DataTable) As DataColumn
        If source Is Nothing Then Return Nothing
        For Each col As DataColumn In source.Columns
            If col.ColumnName.IndexOf("date", StringComparison.OrdinalIgnoreCase) >= 0 AndAlso IsDateColumn(source, col) Then Return col
        Next
        For Each col As DataColumn In source.Columns
            If IsDateColumn(source, col) Then Return col
        Next
        Return Nothing
    End Function

    Private Function IsNumericColumn(source As DataTable, col As DataColumn) As Boolean
        If col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(SByte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(UShort) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(UInteger) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(ULong) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal) Then Return True
        Dim checked As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = FieldText(row(col)).Replace("$", "").Replace(",", "").Trim()
            If text = "" Then Continue For
            Dim value As Double
            checked += 1
            If Not (Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, value) OrElse Double.TryParse(text, value)) Then Return False
            If checked >= 25 Then Exit For
        Next
        Return checked > 0
    End Function

    Private Function IsDateColumn(source As DataTable, col As DataColumn) As Boolean
        If col.DataType Is GetType(DateTime) Then Return True
        Dim checked As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = FieldText(row(col)).Trim()
            If text = "" Then Continue For
            Dim value As DateTime
            checked += 1
            If Not DateTime.TryParse(text, value) Then Return False
            If checked >= 25 Then Exit For
        Next
        Return checked > 0
    End Function

    Private Function TopKeysBySum(buckets As Dictionary(Of String, Bucket)) As List(Of String)
        Dim keys As New List(Of String)(buckets.Keys)
        keys.Sort(Function(a, b)
                      Dim ret As Integer = buckets(b).Sum.CompareTo(buckets(a).Sum)
                      If ret = 0 Then ret = String.Compare(a, b, StringComparison.OrdinalIgnoreCase)
                      Return ret
                  End Function)
        Return keys
    End Function

    Private Function TopKeysByDate(buckets As Dictionary(Of String, Bucket)) As List(Of String)
        Dim keys As New List(Of String)(buckets.Keys)
        keys.Sort(Function(a, b)
                      Dim ret As Integer = buckets(b).MaxDate.CompareTo(buckets(a).MaxDate)
                      If ret = 0 Then ret = String.Compare(a, b, StringComparison.OrdinalIgnoreCase)
                      Return ret
                  End Function)
        Return keys
    End Function

    Private Function TopKeys(values As Dictionary(Of String, Integer)) As List(Of String)
        Dim keys As New List(Of String)(values.Keys)
        keys.Sort(Function(a, b)
                      Dim ret As Integer = values(b).CompareTo(values(a))
                      If ret = 0 Then ret = String.Compare(a, b, StringComparison.OrdinalIgnoreCase)
                      Return ret
                  End Function)
        Return keys
    End Function

    Private Function SortedKeys(buckets As Dictionary(Of String, Bucket)) As List(Of String)
        Dim keys As New List(Of String)(buckets.Keys)
        keys.Sort()
        Return keys
    End Function

    Private Function PriceBand(price As Double) As String
        If price < 25 Then Return "Under 25"
        If price < 50 Then Return "25 - 49.99"
        If price < 100 Then Return "50 - 99.99"
        If price < 250 Then Return "100 - 249.99"
        Return "250 and over"
    End Function

    Private Function PercentText(value As Double, total As Double) As String
        If total = 0 Then Return "0.00"
        Return FormatNumber((value / total) * 100, 2)
    End Function

    Private Function ValueOf(valueObject As Object) As Double
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return 0
        Dim text As String = valueObject.ToString().Replace("$", "").Replace(",", "").Trim()
        Dim value As Double
        If Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, value) OrElse Double.TryParse(text, value) Then Return value
        Return 0
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Class Bucket
        Public Count As Integer
        Public Sum As Double
        Public SecondSum As Double
        Public HasDate As Boolean
        Public MaxDate As DateTime

        Public Function Average() As Double
            If Count = 0 Then Return 0
            Return Sum / Count
        End Function

        Public Function AverageSecond() As Double
            If Count = 0 Then Return 0
            Return SecondSum / Count
        End Function
    End Class

    Private Class MarketSuitability
        Public Name As String
        Public PageUrl As String
        Public Status As String
        Public Available As Boolean
        Public FoundFields As String
        Public MissingFields As String
        Public Recommendation As String

        Public Sub New(name As String, pageUrl As String, status As String, available As Boolean, foundFields As String, missingFields As String, recommendation As String)
            Me.Name = name
            Me.PageUrl = pageUrl
            Me.Status = status
            Me.Available = available
            Me.FoundFields = foundFields
            Me.MissingFields = missingFields
            Me.Recommendation = recommendation
        End Sub
    End Class
End Class
