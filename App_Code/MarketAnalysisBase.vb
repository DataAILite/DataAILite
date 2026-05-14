Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.VisualBasic.FileIO

Public MustInherit Class MarketAnalysisBase
    Inherits Page

    Protected Overridable ReadOnly Property MarketTitle As String
        Get
            Return "Market Analysis"
        End Get
    End Property

    Protected Overridable ReadOnly Property MarketModel As String
        Get
            Return "Demand"
        End Get
    End Property

    Protected Sub Market_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        Dim pageTitle As Label = LabelControl("LabelPageTtl")
        If pageTitle IsNot Nothing AndAlso Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then
            pageTitle.Text = Session("PAGETTL").ToString()
        End If

        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        EnsureReportTitle()

        Dim header As Label = LabelControl("lblHeader")
        If header IsNot Nothing Then
            If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then
                header.Text = Session("REPTITLE").ToString() & " - " & MarketTitle
                Page.Title = Session("REPTITLE").ToString() & " - " & MarketTitle
            Else
                header.Text = MarketTitle
                Page.Title = MarketTitle
            End If
        End If

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

    Protected Sub Market_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            SetLabel("LabelError", "")
            SetLabel("LabelInfo", "")
            LoadMarketData(String.Equals(MarketModel, "Basket", StringComparison.OrdinalIgnoreCase))
            FillFieldLists()
            ApplyModelControlVisibility()
            If String.Equals(MarketModel, "Basket", StringComparison.OrdinalIgnoreCase) Then
                SetLabel("LabelInfo", "Select basket fields and click Build to create the market-basket analysis.")
            Else
                BuildAndBindMarket()
            End If
        ElseIf Session(MarketSessionKey("Table")) IsNot Nothing Then
            ApplyModelControlVisibility()
            BindMarket(CType(Session(MarketSessionKey("Table")), DataTable))
        End If
    End Sub

    Public Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs)
        Dim tree As System.Web.UI.WebControls.TreeView = TryCast(sender, System.Web.UI.WebControls.TreeView)
        If tree IsNot Nothing AndAlso tree.SelectedNode IsNot Nothing AndAlso tree.SelectedNode.Value IsNot Nothing AndAlso tree.SelectedNode.Value.Trim() <> "" Then
            Response.Redirect(tree.SelectedNode.Value)
        End If
    End Sub

    Public Sub ButtonBuild_Click(sender As Object, e As EventArgs)
        ResetMarketGridPage()
        BuildAndBindMarket()
    End Sub

    Public Sub ButtonReset_Click(sender As Object, e As EventArgs)
        Dim searchBox As TextBox = TextBoxControl("txtSearch")
        If searchBox IsNot Nothing Then searchBox.Text = ""
        Dim assumptionBox As TextBox = TextBoxControl("txtAssumption")
        If assumptionBox IsNot Nothing Then assumptionBox.Text = "10"
        ResetMarketGridPage()
        BuildAndBindMarket()
    End Sub

    Public Sub LinkButtonPrevious_Click(sender As Object, e As EventArgs)
        Dim grid As GridView = GridControl("GridViewMarket")
        If grid IsNot Nothing AndAlso grid.PageIndex > 0 Then grid.PageIndex -= 1
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If dt IsNot Nothing Then BindMarket(dt)
    End Sub

    Public Sub LinkButtonNext_Click(sender As Object, e As EventArgs)
        Dim grid As GridView = GridControl("GridViewMarket")
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If grid IsNot Nothing AndAlso dt IsNot Nothing Then
            Dim pageCount As Integer = MarketGridPageCount(dt)
            If grid.PageIndex < pageCount - 1 Then grid.PageIndex += 1
            BindMarket(dt)
        End If
    End Sub

    Public Sub TextBoxPageNumber_TextChanged(sender As Object, e As EventArgs)
        Dim grid As GridView = GridControl("GridViewMarket")
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If grid Is Nothing OrElse dt Is Nothing Then Exit Sub

        Dim requestedPage As Integer = grid.PageIndex + 1
        Dim pageBox As TextBox = TextBoxControl("TextBoxPageNumber")
        Dim postedPageText As String = If(pageBox Is Nothing, "", pageBox.Text.Trim())
        If pageBox IsNot Nothing AndAlso Request.Form(pageBox.UniqueID) IsNot Nothing Then postedPageText = Request.Form(pageBox.UniqueID).Trim()
        If Not Integer.TryParse(postedPageText, requestedPage) Then requestedPage = grid.PageIndex + 1
        If requestedPage < 1 Then requestedPage = 1
        Dim pageCount As Integer = MarketGridPageCount(dt)
        If requestedPage > pageCount Then requestedPage = pageCount
        grid.PageIndex = requestedPage - 1
        BindMarket(dt)
    End Sub

    Public Sub ButtonExportCSV_Click(sender As Object, e As EventArgs)
        ExportMarket("csv")
    End Sub

    Public Sub ButtonExportExcel_Click(sender As Object, e As EventArgs)
        ExportMarket("xls")
    End Sub

    Public Sub lnkMarketAI_Click(sender As Object, e As EventArgs)
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindMarket()
            dt = TryCast(Session(MarketSessionKey("Table")), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            SetLabel("LabelError", "No market model data to send to AI.")
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), MarketTitle, "")
        Session("QuestionToAI") = BuildMarketQuestion("Interpret this market/business model output. Summarize the most important segments, risks, opportunities, and recommended business actions.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
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
    Private Function BuildMarketQuestion(baseQuestion As String) As String
        SetMarketExplanationLabels()
        Dim parts As New List(Of String)()
        parts.Add(baseQuestion)
        Dim subtitleText As String = LabelText("LabelMarketSubtitle")
        If subtitleText.Trim() <> "" Then parts.Add("Input: " & subtitleText.Trim())
        AddQuestionPart(parts, "LabelModelExplanation")
        AddQuestionPart(parts, "LabelAlgorithmExplanation")
        AddQuestionPart(parts, "LabelOutputExplanation")
        Return String.Join(vbCrLf & vbCrLf, parts.ToArray())
    End Function

    Private Sub AddQuestionPart(parts As List(Of String), labelId As String)
        Dim valueText As String = LabelText(labelId)
        If valueText.Trim() <> "" Then parts.Add(valueText.Trim())
    End Sub

    Private Function LabelText(labelId As String) As String
        Dim lbl As Label = LabelControl(labelId)
        If lbl Is Nothing OrElse lbl.Text Is Nothing Then Return ""
        Return lbl.Text
    End Function

    Private Sub LoadMarketData(Optional schemaOnly As Boolean = False)
        Dim ret As String = String.Empty
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()

        Dim sessionTable As DataTable = SessionDv3Table()
        If sessionTable IsNot Nothing AndAlso sessionTable.Rows.Count > 0 Then
            Session(MarketSessionKey("Source")) = sessionTable
            Session(MarketSessionKey("SourceSchemaOnly")) = False
            SetLabel("LabelInfo", "Using current in-memory report data.")
            Exit Sub
        End If

        If schemaOnly AndAlso repid.Trim() <> "" Then
            Dim schemaTable As DataTable = ReportDefinitionSchemaTable(repid)
            If schemaTable IsNot Nothing AndAlso schemaTable.Columns.Count > 0 Then
                Session(MarketSessionKey("Source")) = schemaTable
                Session(MarketSessionKey("SourceSchemaOnly")) = True
                SetLabel("LabelInfo", "Using report field definitions. Click Build to load report records.")
                Exit Sub
            End If
        End If

        If repid.Trim() <> "" AndAlso Not schemaOnly Then
            Try
                Dim rowCount As Integer = If(schemaOnly, 1, -1)
                Dim dv As DataView = RetrieveReportData(repid, "", 1, rowCount, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
                If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                    Session(MarketSessionKey("Source")) = dv.Table
                    Session(MarketSessionKey("SourceSchemaOnly")) = schemaOnly
                    SetLabel("LabelInfo", "Using current report data.")
                    Exit Sub
                End If
            Catch ex As Exception
                SetLabel("LabelError", "Report data could not be loaded. Using sample market data. " & ex.Message)
            End Try
        End If

        Dim samplePath As String = Server.MapPath("~/SampleData/MarketRetailSales.csv")
        If File.Exists(samplePath) Then
            Session(MarketSessionKey("Source")) = LoadCsv(samplePath)
            Session(MarketSessionKey("SourceSchemaOnly")) = False
            SetLabel("LabelInfo", "Using sample market data from SampleData/MarketRetailSales.csv.")
        Else
            Session(MarketSessionKey("Source")) = Nothing
            Session(MarketSessionKey("SourceSchemaOnly")) = False
            SetLabel("LabelError", "No current report data or sample market data is available.")
        End If
    End Sub

    Private Function SessionDv3Table() As DataTable
        If Session Is Nothing OrElse Session("dv3") Is Nothing Then Return Nothing
        Dim dv As DataView = TryCast(Session("dv3"), DataView)
        If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then Return dv.Table

        Dim dt As DataTable = TryCast(Session("dv3"), DataTable)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then Return dt
        Return Nothing
    End Function

    Private Function ReportDefinitionSchemaTable(reportId As String) As DataTable
        Dim dt As New DataTable()
        If reportId.Trim() = "" Then Return dt
        Try
            Dim safeReportId As String = reportId.Replace("'", "''")
            Dim dv As DataView = mRecords("SELECT Val FROM OURReportFormat WHERE ReportID='" & safeReportId & "' AND Prop='FIELDS' ORDER BY [Order], Val")
            If dv Is Nothing OrElse dv.Table Is Nothing Then Return dt
            For Each row As DataRow In dv.Table.Rows
                Dim columnName As String = row("Val").ToString().Trim()
                If columnName = "" OrElse dt.Columns.Contains(columnName) Then Continue For
                dt.Columns.Add(columnName, GetType(String))
            Next
        Catch ex As Exception
            Return New DataTable()
        End Try
        Return dt
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

    Private Function SourceTable(Optional requireFullSource As Boolean = True) As DataTable
        If Session(MarketSessionKey("Source")) Is Nothing OrElse (requireFullSource AndAlso SourceIsSchemaOnly()) Then LoadMarketData(False)
        Return TryCast(Session(MarketSessionKey("Source")), DataTable)
    End Function

    Private Function SourceIsSchemaOnly() As Boolean
        If Session(MarketSessionKey("SourceSchemaOnly")) Is Nothing Then Return False
        Return Convert.ToBoolean(Session(MarketSessionKey("SourceSchemaOnly")))
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = SourceTable(False)
        If dt Is Nothing OrElse dt.Columns.Count = 0 Then Exit Sub

        Dim dimension As DropDownList = DropDownControl("DropDownDimension")
        Dim dimensionMulti As ListBox = ListBoxControl("ListBoxDimension")
        Dim valueField As DropDownList = DropDownControl("DropDownValueField")
        Dim dateField As DropDownList = DropDownControl("DropDownDateField")
        Dim secondary As DropDownList = DropDownControl("DropDownSecondaryField")
        Dim inventoryField As DropDownList = DropDownControl("DropDownInventoryField")
        If dimension Is Nothing OrElse dimensionMulti Is Nothing OrElse valueField Is Nothing OrElse dateField Is Nothing OrElse secondary Is Nothing Then Exit Sub

        dimension.Items.Clear()
        dimensionMulti.Items.Clear()
        valueField.Items.Clear()
        dateField.Items.Clear()
        secondary.Items.Clear()
        If String.Equals(MarketModel, "Pricing", StringComparison.OrdinalIgnoreCase) Then dimension.Items.Add(New ListItem("(None)", ""))
        dateField.Items.Add(New ListItem("(None)", ""))
        secondary.Items.Add(New ListItem("(None)", ""))
        If inventoryField IsNot Nothing Then
            inventoryField.Items.Clear()
            inventoryField.Items.Add(New ListItem("(None)", ""))
        End If

        For Each col As DataColumn In dt.Columns
            dimension.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            dimensionMulti.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            valueField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            dateField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            secondary.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If inventoryField IsNot Nothing Then inventoryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
        Next

        If Not String.Equals(MarketModel, "Pricing", StringComparison.OrdinalIgnoreCase) Then SelectFieldByHints(dimension, PreferredDimensionHints())
        SelectFieldsByHints(dimensionMulti, PreferredDimensionHints())
        SelectFieldByHints(valueField, PreferredValueHints())
        SelectFieldByHints(dateField, New String() {"date", "order date", "invoice date", "sale date"})
        SelectFieldByHints(secondary, PreferredSecondaryHints())
        If inventoryField IsNot Nothing Then SelectFieldByHints(inventoryField, New String() {"inventory", "stock", "on hand", "available", "balance"})
    End Sub

    Private Sub ApplyModelControlVisibility()
        Dim showMultiPrimary As Boolean = SupportsMultiPrimary()
        Dim showDateField As Boolean = String.Equals(MarketModel, "Churn", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Demand", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Inventory", StringComparison.OrdinalIgnoreCase)
        Dim showSecondaryField As Boolean = String.Equals(MarketModel, "Pricing", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Basket", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Elasticity", StringComparison.OrdinalIgnoreCase)
        Dim showAssumptionField As Boolean = ModelUsesAssumption()
        Dim primarySingle As Panel = PanelControl("pnlPrimarySingle")
        Dim primaryMulti As Panel = PanelControl("pnlPrimaryMulti")
        Dim dateLabel As Panel = PanelControl("pnlDateLabel")
        Dim dateControl As Panel = PanelControl("pnlDateControl")
        Dim dateAggregationLabel As Panel = PanelControl("pnlDateAggregationLabel")
        Dim dateAggregationControl As Panel = PanelControl("pnlDateAggregationControl")
        Dim dateAggregationField As DropDownList = DropDownControl("DropDownDateAggregation")
        Dim dateField As DropDownList = DropDownControl("DropDownDateField")
        Dim secondaryLabel As Panel = PanelControl("pnlSecondaryLabel")
        Dim secondaryControl As Panel = PanelControl("pnlSecondaryControl")
        Dim secondaryField As DropDownList = DropDownControl("DropDownSecondaryField")

        If primarySingle IsNot Nothing Then primarySingle.Visible = Not showMultiPrimary
        If primaryMulti IsNot Nothing Then primaryMulti.Visible = showMultiPrimary

        If dateLabel IsNot Nothing Then dateLabel.Visible = showDateField
        If dateControl IsNot Nothing Then dateControl.Visible = showDateField
        If dateAggregationLabel IsNot Nothing Then dateAggregationLabel.Visible = showDateField
        If dateAggregationControl IsNot Nothing Then dateAggregationControl.Visible = showDateField
        If Not showDateField AndAlso dateField IsNot Nothing AndAlso dateField.Items.Count > 0 Then
            dateField.ClearSelection()
            If dateField.Items.FindByValue("") IsNot Nothing Then dateField.Items.FindByValue("").Selected = True
        End If
        If Not showDateField AndAlso dateAggregationField IsNot Nothing AndAlso dateAggregationField.Items.Count > 0 Then
            dateAggregationField.ClearSelection()
            If dateAggregationField.Items.FindByValue("Month") IsNot Nothing Then dateAggregationField.Items.FindByValue("Month").Selected = True
        End If

        If secondaryLabel IsNot Nothing Then secondaryLabel.Visible = showSecondaryField
        If secondaryControl IsNot Nothing Then secondaryControl.Visible = showSecondaryField
        If Not showSecondaryField AndAlso secondaryField IsNot Nothing AndAlso secondaryField.Items.Count > 0 Then
            secondaryField.ClearSelection()
            If secondaryField.Items.FindByValue("") IsNot Nothing Then secondaryField.Items.FindByValue("").Selected = True
        End If

        RegisterAssumptionRowVisibility(showAssumptionField)
    End Sub

    Private Function SupportsMultiPrimary() As Boolean
        Return String.Equals(MarketModel, "Demand", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Segments", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Risk", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Inventory", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Profit", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Scenario", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Elasticity", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function ModelUsesAssumption() As Boolean
        Return String.Equals(MarketModel, "Demand", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Elasticity", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Inventory", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Profit", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(MarketModel, "Scenario", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Sub RegisterAssumptionRowVisibility(visible As Boolean)
        Dim assumptionBox As TextBox = TextBoxControl("txtAssumption")
        If assumptionBox Is Nothing Then Exit Sub
        Dim displayValue As String = If(visible, "", "none")
        Dim script As String = "var e=document.getElementById('" & assumptionBox.ClientID & "'); if(e){var r=e.closest('tr'); if(r){r.style.display='" & displayValue & "';}}"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "MarketAssumptionRow_" & MarketModel, script, True)
    End Sub

    Private Function PreferredDimensionHints() As String()
        Select Case MarketModel
            Case "Basket"
                Return New String() {"product", "item", "category", "description"}
            Case "Segments", "Churn", "Risk"
                Return New String() {"customer", "customer id", "segment", "gender", "age"}
            Case "Inventory", "Profit", "Pricing", "Demand", "Elasticity"
                Return New String() {"product", "category", "region", "location"}
            Case Else
                Return New String() {"product", "category", "region", "customer"}
        End Select
    End Function

    Private Function PreferredSecondaryHints() As String()
        Select Case MarketModel
            Case "Basket"
                Return New String() {"transaction", "order", "invoice"}
            Case "Pricing", "Elasticity"
                Return New String() {"quantity", "qty", "units"}
            Case "Churn", "Risk", "Segments"
                Return New String() {"customer", "gender", "age", "segment"}
            Case Else
                Return New String() {"region", "category", "customer", "product"}
        End Select
    End Function

    Private Function PreferredValueHints() As String()
        Select Case MarketModel
            Case "Pricing", "Elasticity"
                Return New String() {"price", "unit price", "price per unit"}
            Case "Profit"
                Return New String() {"profit", "total amount", "total", "sales", "revenue"}
            Case "Inventory", "Demand"
                Return New String() {"quantity", "qty", "units", "total amount", "sales"}
            Case "Risk", "Churn"
                Return New String() {"total amount", "sales", "quantity", "age"}
            Case Else
                Return New String() {"total amount", "total", "sales", "revenue", "quantity"}
        End Select
    End Function

    Private Sub SelectFieldByHints(dropDown As DropDownList, hints() As String)
        For Each hint As String In hints
            For Each item As ListItem In dropDown.Items
                If item.Text.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    dropDown.ClearSelection()
                    item.Selected = True
                    Exit Sub
                End If
            Next
        Next
    End Sub

    Private Sub SelectFieldsByHints(listBox As ListBox, hints() As String)
        If listBox Is Nothing Then Exit Sub
        For Each item As ListItem In listBox.Items
            item.Selected = False
        Next

        Dim selectedCount As Integer = 0
        For Each hint As String In hints
            For Each item As ListItem In listBox.Items
                If selectedCount >= 2 Then Exit Sub
                If Not item.Selected AndAlso item.Text.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    item.Selected = True
                    selectedCount += 1
                    Exit For
                End If
            Next
        Next

        If selectedCount = 0 AndAlso listBox.Items.Count > 0 Then listBox.Items(0).Selected = True
    End Sub

    Private Sub BuildAndBindMarket()
        SetLabel("LabelError", "")
        Dim source As DataTable = SourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub
        Session("MarketFilters") = New Dictionary(Of String, String)()

        Dim output As DataTable
        Select Case MarketModel
            Case "Pricing"
                output = BuildPricing(source)
            Case "Elasticity"
                output = BuildElasticity(source)
            Case "Basket"
                output = BuildBasket(source)
            Case "Segments"
                output = BuildSegments(source)
            Case "Churn"
                output = BuildChurn(source)
            Case "Risk"
                output = BuildRisk(source)
            Case "Inventory"
                output = BuildInventory(source)
            Case "Profit"
                output = BuildProfit(source)
            Case "Scenario"
                output = BuildScenario(source)
            Case Else
                output = BuildDemand(source)
        End Select

        Session(MarketSessionKey("Table")) = output
        BindMarket(output)
    End Sub

    Private Sub BindMarket(dt As DataTable)
        Dim grid As GridView = GridControl("GridViewMarket")
        If grid IsNot Nothing Then
            RemoveHandler grid.RowDataBound, AddressOf GridViewMarket_RowDataBound
            RemoveHandler grid.PageIndexChanging, AddressOf GridViewMarket_PageIndexChanging
            AddHandler grid.RowDataBound, AddressOf GridViewMarket_RowDataBound
            AddHandler grid.PageIndexChanging, AddressOf GridViewMarket_PageIndexChanging
            grid.AllowPaging = dt IsNot Nothing AndAlso dt.Rows.Count > MarketGridPageSize()
            grid.PageSize = MarketGridPageSize()
            If dt IsNot Nothing AndAlso grid.AllowPaging AndAlso grid.PageIndex * grid.PageSize >= dt.Rows.Count Then grid.PageIndex = 0
            If dt IsNot Nothing AndAlso Not grid.AllowPaging Then grid.PageIndex = 0
            grid.DataSource = dt
            grid.DataBind()
        End If
        If dt IsNot Nothing Then SetLabel("LabelInfo", MarketTitle & " (" & dt.Rows.Count.ToString() & " rows)")
        UpdateMarketPager(dt)
        SetMarketExplanationLabels()
    End Sub

    Private Function MarketGridPageSize() As Integer
        Return 50
    End Function

    Private Function MarketGridPageCount(dt As DataTable) As Integer
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return 1
        Return Math.Max(1, CInt(Math.Ceiling(dt.Rows.Count / CDbl(MarketGridPageSize()))))
    End Function

    Private Sub ResetMarketGridPage()
        Dim grid As GridView = GridControl("GridViewMarket")
        If grid IsNot Nothing Then grid.PageIndex = 0
    End Sub

    Private Sub UpdateMarketPager(dt As DataTable)
        Dim grid As GridView = GridControl("GridViewMarket")
        Dim previousLink As LinkButton = LinkButtonControl("LinkButtonPrevious")
        Dim nextLink As LinkButton = LinkButtonControl("LinkButtonNext")
        Dim pageCaption As Label = LabelControl("LabelPageNumberCaption")
        Dim pageBox As TextBox = TextBoxControl("TextBoxPageNumber")
        Dim pageCountLabel As Label = LabelControl("LabelPageCount")
        Dim pageCount As Integer = MarketGridPageCount(dt)
        Dim pageNumber As Integer = If(grid Is Nothing, 1, grid.PageIndex + 1)
        If pageNumber < 1 Then pageNumber = 1
        If pageNumber > pageCount Then pageNumber = pageCount
        Dim showPager As Boolean = pageCount > 1

        If pageBox IsNot Nothing Then
            pageBox.Text = pageNumber.ToString()
            pageBox.Visible = showPager
        End If
        If pageCountLabel IsNot Nothing Then
            pageCountLabel.Text = " of " & pageCount.ToString()
            pageCountLabel.Visible = showPager
        End If
        If pageCaption IsNot Nothing Then pageCaption.Visible = showPager
        If previousLink IsNot Nothing Then
            previousLink.Enabled = pageNumber > 1
            previousLink.Visible = showPager AndAlso pageNumber > 1
        End If
        If nextLink IsNot Nothing Then
            nextLink.Enabled = pageNumber < pageCount
            nextLink.Visible = showPager AndAlso pageNumber < pageCount
        End If
    End Sub

    Private Sub GridViewMarket_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Dim grid As GridView = TryCast(sender, GridView)
        If grid IsNot Nothing Then grid.PageIndex = e.NewPageIndex
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If dt IsNot Nothing Then BindMarket(dt)
    End Sub

    Private Sub SetMarketExplanationLabels()
        Dim explanations As String() = MarketExplanations()
        SetLabel("LabelModelExplanation", "Model: " & explanations(0))
        SetLabel("LabelAlgorithmExplanation", "Algorithm: " & explanations(1))
        SetLabel("LabelOutputExplanation", "Output: " & explanations(2))
    End Sub

    Private Function MarketExplanations() As String()
        Select Case MarketModel
            Case "Pricing"
                Return New String() {
                    "Price-band sensitivity model comparing volume and revenue behavior across price ranges, optionally by a selected market dimension.",
                    "Filtered records are grouped into price bands from the selected Value Field. If Primary field is (None), results are grouped by price band only. If a Primary field is selected, results are grouped by Dimension plus Price Band. The selected Secondary Field is treated as quantity or units, and the page calculates record count, average quantity, average revenue, and a sensitivity note based on relative volume.",
                    "Dimension, when shown, is the selected Primary field value used to split the pricing result. Price Band is the calculated price range for the selected Value Field. Records is the count of matching source rows and links to those records. Average Quantity is the average selected Secondary Field quantity or units in the band. Average Revenue is the average calculated revenue, price times quantity where quantity is available. Sensitivity Note flags whether that band has higher or lower unit volume compared with the built-in volume threshold."}
            Case "Elasticity"
                Return New String() {
                    "Pricing elasticity model measuring how quantity changes when price changes.",
                    "Filtered records are grouped by selected market dimension and price band. Average price, quantity sold, and revenue are calculated for each band; then price change and quantity change are compared between bands. Assumption % is used as a possible price change to project price, quantity, revenue, and revenue impact.",
                    "Dimension is the selected market group. Price Band is the calculated range for prices in that group. Average Price is the mean selected price field in the band. Quantity Sold is the total selected Secondary Field quantity. Revenue is average price times quantity. Price Change % compares the current band price with the previous price band. Quantity Change % compares the current band quantity with the previous band. Elasticity is quantity change divided by price change. Assumption Price Change % is the selected what-if percentage. Projected Price, Projected Quantity, and Projected Revenue show the modeled result after applying the assumption. Revenue Impact is projected revenue minus current revenue. Elasticity Note classifies the response as base band, inelastic, near unit elasticity, or elastic demand. Records links to the matching rows."}
            Case "Basket"
                Return New String() {
                    "Market-basket co-occurrence model for finding items that appear together in the same transaction.",
                    "Filtered records are grouped by the selected Secondary Field as order, invoice, or transaction. Unique Primary Field item values are collected per transaction, item pairs are counted, support % is calculated from orders together divided by total orders, and Weighted Basket Value is summed from the selected Value Field for matching pair records.",
                    "Item A and Item B are the two Primary Field values found together in the same Secondary Field transaction, order, or invoice. Records is the number of transactions containing the pair and links to the matching rows. Support % is the share of all checked transactions that contain the pair. Weighted Basket Value is the sum of the selected Value Field for the matching pair rows. Basket Note identifies the pair as a candidate for bundle, cross-sell, or co-occurrence review."}
            Case "Segments"
                Return New String() {
                    "Grouped segmentation model comparing market, customer, product, or location segments by value concentration and average value.",
                    "Filtered records are grouped by one or more selected Primary Fields. The Value Field is summed and averaged for each segment, then each segment is compared with the overall average to assign a segment note.",
                    "Segment is the combined selected Primary field value. Records is the count of source rows in that segment and links to those records. Value is the sum of the selected Value Field for the segment. Average Value is Value divided by Records. Segment Note compares the segment average with the overall average and labels the segment as above or below average."}
            Case "Churn"
                Return New String() {
                    "Recency-based churn and retention review model.",
                    "Filtered records are grouped by customer or segment. The page keeps each group’s latest activity date, sums the Value Field, compares latest activity with the latest date in the data, and calculates a retention score. When Date Field is selected, results can also be grouped by selected date aggregation.",
                    "Customer / Segment is the selected Primary field value being scored. Period appears when Date Field is selected and shows the selected day, week, month, quarter, or year bucket. Records is the number of matching rows and links to those records. Last Activity is the latest date found for the group or period. Value is the sum of the selected Value Field. Retention Score is a recency score where more recent activity scores higher. Churn Note flags recently active groups versus groups that should be reviewed for churn risk."}
            Case "Risk"
                Return New String() {
                    "Relative exposure risk model for ranking market groups by value or exposure.",
                    "Filtered records are grouped by one or more selected Primary Fields. The Value Field is summed as exposure, each group is compared with the maximum group exposure, and a risk score from 0 to 100 is assigned with a risk note.",
                    "Dimension is the combined selected Primary field value. Records is the number of rows behind the risk score and links to those records. Value is the sum of the selected Value Field, treated as exposure. Risk Score scales each group's exposure from 0 to 100 against the highest exposure group. Risk Note classifies the result as lower, medium, or high exposure."}
            Case "Inventory"
                Return New String() {
                    "Inventory movement and reorder review model.",
                    "Filtered records are grouped by one or more selected Primary Fields and optionally by date period. The Value Field is summed as movement, velocity is movement divided by records, Current Inventory is taken from the selected or detected inventory field, Assumption % is treated as safety stock, and reorder point, supply periods, and reorder need are calculated.",
                    "Item is the selected Primary field value. Period appears when Date Field is selected and shows the selected aggregation bucket. Units / Movement is the sum of the selected Value Field, treated as demand or movement. Records is the count of matching rows and links to those records. Velocity is Units / Movement divided by Records. Inventory Field shows the selected or detected current-stock field. Current Inventory is the summed stock value for the item. Supply Periods estimates how many velocity periods current inventory can cover. Safety Stock % is the selected Assumption %. Reorder Point is velocity plus safety stock. Reorder Needed shows Yes when current inventory is at or below reorder point. Inventory Note explains whether the item needs reorder review, is fast or slow moving, or is missing an inventory field."}
            Case "Profit"
                Return New String() {
                    "Profitability driver model using revenue, cost source, estimated cost, estimated profit, margin, and contribution.",
                    "Filtered records are grouped by one or more selected Primary Fields. The Value Field is summed as revenue. Cost is taken from direct cost fields when available, otherwise from unit cost times quantity, otherwise from Assumption % as a fallback cost rate.",
                    "Driver is the selected Primary field value used as the profitability driver. Revenue is the sum of the selected Value Field. Direct Cost is the cost found from total cost fields or unit cost times quantity where available. Cost Source explains whether cost came from a direct cost field, unit cost times quantity, or Assumption %. Cost Rate % is estimated cost divided by revenue. Estimated Cost is direct cost when available or revenue times the assumption cost rate. Estimated Profit is revenue minus estimated cost. Margin % is estimated profit divided by revenue. Profit Contribution % is the driver's share of total estimated profit. Profit Note flags strong, moderate, thin, or negative margin and whether cost was direct or estimated. Records links to the matching rows."}
            Case "Scenario"
                Return New String() {
                    "What-if scenario model for market assumptions.",
                    "Filtered records are grouped by one or more selected Primary Fields. The Value Field is summed as current value, and Assumption % creates downside, base, and upside scenario values with differences and scenario range.",
                    "Dimension is the selected Primary field value. Current Value is the sum of the selected Value Field before assumptions. Downside Value applies the Assumption % as a decrease. Base Value repeats the current value for comparison. Upside Value applies the Assumption % as an increase. Downside Difference and Upside Difference show the change from Current Value. Scenario Range is Upside Value minus Downside Value. Assumption % shows the what-if percentage used. Scenario Note labels the scenario spread as narrow, moderate, or wide. Records links to the matching rows."}
            Case Else
                Return New String() {
                    "Grouped demand model by category, product, customer, location, or combined selected dimensions.",
                    "Filtered records are grouped by one or more selected Primary Fields and optionally by date period. The Value Field is summed for demand, share of total demand is calculated, and Assumption % is used to calculate projected demand.",
                    "Dimension is the combined selected Primary field value. Period appears when Date Field is selected and shows the selected day, week, month, quarter, or year bucket. Demand Value is the sum of the selected Value Field. Records is the number of matching source rows and links to those records. Share % is the group's portion of total demand value. Projected Demand applies the selected Assumption % to Demand Value to show an adjusted demand estimate."}
        End Select
    End Function

    Private Sub GridViewMarket_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") Then Exit Sub

        Dim recordsIndex As Integer = dt.Columns.IndexOf("Records")
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < e.Row.Cells.Count Then e.Row.Cells(filterIndex).Visible = False
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        If recordsIndex < 0 OrElse filterIndex < 0 OrElse recordsIndex >= e.Row.Cells.Count OrElse filterIndex >= e.Row.Cells.Count Then Exit Sub

        Dim recordsText As String = e.Row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
        Dim filterId As String = e.Row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
        If filterId = "" OrElse recordsText = "" Then Exit Sub

        Dim link As New HyperLink()
        link.Text = recordsText
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&marketfilter=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding records in Data Explorer."
        e.Row.Cells(recordsIndex).Controls.Clear()
        e.Row.Cells(recordsIndex).Controls.Add(link)
    End Sub

    Private Function BuildDemand(source As DataTable) As DataTable
        Dim usePeriods As Boolean = SelectedDate().Trim() <> "" AndAlso source.Columns.Contains(SelectedDate())
        Dim dt As DataTable = If(usePeriods, ResultTable("Dimension", "Period", "Demand Value", "Records", "Share %", "Projected Demand", "FilterId"), ResultTable("Dimension", "Demand Value", "Records", "Share %", "Projected Demand", "FilterId"))
        Dim buckets As Dictionary(Of String, MarketBucket) = If(usePeriods, GroupBucketsByPeriod(source, SelectedDimension(), SelectedValue(), SelectedDate(), ""), GroupBuckets(source, SelectedDimension(), SelectedValue(), ""))
        Dim total As Double = BucketTotal(buckets)
        For Each key As String In SortedKeys(buckets)
            Dim b As MarketBucket = buckets(key)
            If usePeriods Then
                Dim parts() As String = SplitPeriodKey(key)
                AddRow(dt, parts(0), parts(1), FormatNumber(b.Sum, 2), b.Count.ToString(), PercentText(b.Sum, total), FormatNumber(b.Sum * (1 + AssumptionRate()), 2), RegisterMarketFilter(b.Rows))
            Else
                AddRow(dt, key, FormatNumber(b.Sum, 2), b.Count.ToString(), PercentText(b.Sum, total), FormatNumber(b.Sum * (1 + AssumptionRate()), 2), RegisterMarketFilter(b.Rows))
            End If
        Next
        Return dt
    End Function

    Private Function BuildPricing(source As DataTable) As DataTable
        Dim dimensionField As String = SelectedDimension()
        Dim includeDimension As Boolean = dimensionField.Trim() <> "" AndAlso source.Columns.Contains(dimensionField)
        Dim dt As DataTable = If(includeDimension, ResultTable("Dimension", "Price Band", "Records", "Average Quantity", "Average Revenue", "Sensitivity Note", "FilterId"), ResultTable("Price Band", "Records", "Average Quantity", "Average Revenue", "Sensitivity Note", "FilterId"))
        Dim priceField As String = SelectedValue()
        Dim qtyField As String = SelectedSecondary()
        Dim buckets As Dictionary(Of String, MarketBucket) = PriceBuckets(source, priceField, qtyField, dimensionField)
        For Each key As String In SortedPriceBucketKeys(buckets, includeDimension)
            Dim b As MarketBucket = buckets(key)
            If includeDimension Then
                Dim parts() As String = SplitPriceDimensionKey(key)
                AddRow(dt, parts(0), parts(1), b.Count.ToString(), FormatNumber(b.AverageSecondary(), 2), FormatNumber(b.Average(), 2), If(b.AverageSecondary() >= 2, "Higher unit volume", "Lower unit volume"), RegisterMarketFilter(b.Rows))
            Else
                AddRow(dt, key, b.Count.ToString(), FormatNumber(b.AverageSecondary(), 2), FormatNumber(b.Average(), 2), If(b.AverageSecondary() >= 2, "Higher unit volume", "Lower unit volume"), RegisterMarketFilter(b.Rows))
            End If
        Next
        Return dt
    End Function

    Private Function BuildElasticity(source As DataTable) As DataTable
        Dim dt As DataTable = ResultTable("Dimension", "Price Band", "Average Price", "Quantity Sold", "Revenue", "Price Change %", "Quantity Change %", "Elasticity", "Assumption Price Change %", "Projected Price", "Projected Quantity", "Projected Revenue", "Revenue Impact", "Elasticity Note", "Records", "FilterId")
        Dim priceField As String = SelectedValue()
        Dim quantityField As String = SelectedSecondary()
        If quantityField = "" OrElse Not source.Columns.Contains(quantityField) Then
            SetLabel("LabelError", "Select a quantity, units, or sales-volume field as Secondary field for elasticity analysis.")
            Return dt
        End If

        Dim groups As New Dictionary(Of String, Dictionary(Of String, ElasticityBucket))(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For
            Dim dimensionText As String = DimensionKey(row, SelectedDimension())
            If dimensionText = "" Then dimensionText = "(blank)"
            Dim price As Double = ValueOf(row(priceField))
            Dim quantity As Double = ValueOf(row(quantityField))
            Dim band As String = PriceBand(price)
            If Not groups.ContainsKey(dimensionText) Then groups.Add(dimensionText, New Dictionary(Of String, ElasticityBucket)(StringComparer.OrdinalIgnoreCase))
            If Not groups(dimensionText).ContainsKey(band) Then groups(dimensionText).Add(band, New ElasticityBucket())
            groups(dimensionText)(band).Add(price, quantity, row)
        Next

        For Each dimensionText As String In SortedKeys(groups)
            Dim bands As List(Of ElasticityBucket) = SortedElasticityBuckets(groups(dimensionText))
            Dim previousBucket As ElasticityBucket = Nothing
            For Each bucket As ElasticityBucket In bands
                Dim priceChange As Nullable(Of Double) = Nothing
                Dim quantityChange As Nullable(Of Double) = Nothing
                Dim elasticity As Nullable(Of Double) = Nothing
                If previousBucket IsNot Nothing Then
                    priceChange = PercentChangeNumber(bucket.AveragePrice(), previousBucket.AveragePrice())
                    quantityChange = PercentChangeNumber(bucket.QuantitySum, previousBucket.QuantitySum)
                    If priceChange.HasValue AndAlso Math.Abs(priceChange.Value) > 0.000001 AndAlso quantityChange.HasValue Then elasticity = quantityChange.Value / priceChange.Value
                End If

                Dim projectedPrice As Double = bucket.AveragePrice() * (1 + AssumptionRate())
                Dim projectedQuantity As Double = bucket.QuantitySum
                If elasticity.HasValue Then projectedQuantity = Math.Max(0, bucket.QuantitySum * (1 + (elasticity.Value * AssumptionRate())))
                Dim projectedRevenue As Double = projectedPrice * projectedQuantity
                Dim revenueImpact As Double = projectedRevenue - bucket.RevenueSum

                AddRow(dt, dimensionText, bucket.PriceBand, FormatNumber(bucket.AveragePrice(), 2), FormatNumber(bucket.QuantitySum, 2), FormatNumber(bucket.RevenueSum, 2), NullableNumberText(priceChange), NullableNumberText(quantityChange), NullableNumberText(elasticity), FormatNumber(AssumptionRate() * 100, 2), FormatNumber(projectedPrice, 2), FormatNumber(projectedQuantity, 2), FormatNumber(projectedRevenue, 2), FormatNumber(revenueImpact, 2), ElasticityNote(elasticity), bucket.Count.ToString(), RegisterMarketFilter(bucket.Rows))
                previousBucket = bucket
            Next
        Next

        Return dt
    End Function

    Private Function BuildBasket(source As DataTable) As DataTable
        Dim dt As DataTable = ResultTable("Item A", "Item B", "Records", "Support %", "Weighted Basket Value", "Basket Note", "FilterId")
        Dim itemField As String = SelectedDimension()
        Dim orderField As String = SelectedSecondary()
        Dim valueField As String = SelectedValue()
        If orderField = "" Then
            SetLabel("LabelError", "Select an order, transaction, or invoice field as Secondary field for basket analysis.")
            Return dt
        End If

        Dim orderItems As New Dictionary(Of String, Dictionary(Of String, Boolean))(StringComparer.OrdinalIgnoreCase)
        Dim orderItemRows As New Dictionary(Of String, Dictionary(Of String, List(Of DataRow)))(StringComparer.OrdinalIgnoreCase)
        Dim processedRows As Integer = 0
        Dim maxBasketRows As Integer = 5000
        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For
            processedRows += 1
            If processedRows > maxBasketRows Then Exit For
            Dim orderId As String = FieldText(row(orderField))
            Dim itemText As String = FieldText(row(itemField))
            If orderId = "" OrElse itemText = "" Then Continue For
            If Not orderItems.ContainsKey(orderId) Then orderItems.Add(orderId, New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase))
            If Not orderItems(orderId).ContainsKey(itemText) Then orderItems(orderId).Add(itemText, True)
            If Not orderItemRows.ContainsKey(orderId) Then orderItemRows.Add(orderId, New Dictionary(Of String, List(Of DataRow))(StringComparer.OrdinalIgnoreCase))
            If Not orderItemRows(orderId).ContainsKey(itemText) Then orderItemRows(orderId).Add(itemText, New List(Of DataRow)())
            orderItemRows(orderId)(itemText).Add(row)
        Next

        Dim pairs As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim pairRows As New Dictionary(Of String, List(Of DataRow))(StringComparer.OrdinalIgnoreCase)
        Dim maxItemsPerOrder As Integer = 20
        Dim maxBasketPairs As Integer = 500
        For Each orderKvp As KeyValuePair(Of String, Dictionary(Of String, Boolean)) In orderItems
            If pairs.Count >= maxBasketPairs Then Exit For
            Dim names As New List(Of String)(orderKvp.Value.Keys)
            names.Sort()
            If names.Count > maxItemsPerOrder Then names.RemoveRange(maxItemsPerOrder, names.Count - maxItemsPerOrder)
            For i As Integer = 0 To names.Count - 1
                For j As Integer = i + 1 To names.Count - 1
                    Dim pairKey As String = names(i) & ChrW(30) & names(j)
                    If pairs.Count >= maxBasketPairs AndAlso Not pairs.ContainsKey(pairKey) Then Continue For
                    If Not pairs.ContainsKey(pairKey) Then pairs.Add(pairKey, 0)
                    pairs(pairKey) += 1
                    If Not pairRows.ContainsKey(pairKey) Then pairRows.Add(pairKey, New List(Of DataRow)())
                    AddBasketPairRows(pairRows(pairKey), orderItemRows, orderKvp.Key, names(i))
                    AddBasketPairRows(pairRows(pairKey), orderItemRows, orderKvp.Key, names(j))
                Next
            Next
        Next

        For Each pairKey As String In SortedBasketPairKeys(pairs, pairRows, valueField)
            Dim parts() As String = pairKey.Split(ChrW(30))
            AddRow(dt, parts(0), parts(1), pairs(pairKey).ToString(), PercentText(pairs(pairKey), Math.Max(1, orderItems.Count)), FormatNumber(SumRowsIfFieldExists(pairRows(pairKey), valueField).GetValueOrDefault(0), 2), "Review for bundle or cross-sell", RegisterMarketFilter(pairRows(pairKey)))
        Next
        Return dt
    End Function

    Private Function SortedBasketPairKeys(pairs As Dictionary(Of String, Integer), pairRows As Dictionary(Of String, List(Of DataRow)), valueField As String) As List(Of String)
        Dim keys As New List(Of String)(pairs.Keys)
        Dim pairValues As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        For Each key As String In keys
            Dim rows As List(Of DataRow) = Nothing
            If pairRows IsNot Nothing AndAlso pairRows.TryGetValue(key, rows) Then
                pairValues(key) = SumRowsIfFieldExists(rows, valueField).GetValueOrDefault(0)
            Else
                pairValues(key) = 0
            End If
        Next

        keys.Sort(Function(leftKey As String, rightKey As String)
                      Dim recordCompare As Integer = pairs(rightKey).CompareTo(pairs(leftKey))
                      If recordCompare <> 0 Then Return recordCompare
                      Dim valueCompare As Integer = pairValues(rightKey).CompareTo(pairValues(leftKey))
                      If valueCompare <> 0 Then Return valueCompare
                      Return String.Compare(leftKey, rightKey, StringComparison.OrdinalIgnoreCase)
                  End Function)
        Return keys
    End Function

    Private Sub AddBasketPairRows(pairRowList As List(Of DataRow), orderItemRows As Dictionary(Of String, Dictionary(Of String, List(Of DataRow))), orderId As String, itemText As String)
        If pairRowList Is Nothing OrElse orderItemRows Is Nothing Then Exit Sub
        If Not orderItemRows.ContainsKey(orderId) Then Exit Sub
        If Not orderItemRows(orderId).ContainsKey(itemText) Then Exit Sub
        pairRowList.AddRange(orderItemRows(orderId)(itemText))
    End Sub

    Private Function BuildSegments(source As DataTable) As DataTable
        Dim dt As DataTable = ResultTable("Segment", "Records", "Value", "Average Value", "Segment Note", "FilterId")
        Dim buckets As Dictionary(Of String, MarketBucket) = GroupBuckets(source, SelectedDimension(), SelectedValue(), SelectedSecondary())
        For Each key As String In SortedKeys(buckets)
            Dim b As MarketBucket = buckets(key)
            AddRow(dt, key, b.Count.ToString(), FormatNumber(b.Sum, 2), FormatNumber(b.Average(), 2), If(b.Average() >= OverallAverage(buckets), "Above average segment", "Below average segment"), RegisterMarketFilter(b.Rows))
        Next
        Return dt
    End Function

    Private Function BuildChurn(source As DataTable) As DataTable
        Dim usePeriods As Boolean = SelectedDate().Trim() <> "" AndAlso source.Columns.Contains(SelectedDate())
        Dim dt As DataTable = If(usePeriods, ResultTable("Customer / Segment", "Period", "Records", "Last Activity", "Value", "Retention Score", "Churn Note", "FilterId"), ResultTable("Customer / Segment", "Records", "Last Activity", "Value", "Retention Score", "Churn Note", "FilterId"))
        Dim customerField As String = SelectedDimension()
        Dim valueField As String = SelectedValue()
        Dim dateField As String = SelectedDate()
        Dim buckets As Dictionary(Of String, MarketBucket) = If(usePeriods, GroupBucketsByPeriod(source, customerField, valueField, dateField, ""), GroupBuckets(source, customerField, valueField, ""))
        Dim maxDate As DateTime = MaxBucketDate(buckets)
        For Each key As String In SortedKeys(buckets)
            Dim b As MarketBucket = buckets(key)
            Dim daysOld As Integer = If(b.HasDate, CInt((maxDate - b.MaxDate).TotalDays), 0)
            Dim score As Double = Math.Max(0, 100 - daysOld)
            If usePeriods Then
                Dim parts() As String = SplitPeriodKey(key)
                AddRow(dt, parts(0), parts(1), b.Count.ToString(), If(b.HasDate, b.MaxDate.ToShortDateString(), ""), FormatNumber(b.Sum, 2), FormatNumber(score, 1), If(score < 50, "Review for churn risk", "Recently active"), RegisterMarketFilter(b.Rows))
            Else
                AddRow(dt, key, b.Count.ToString(), If(b.HasDate, b.MaxDate.ToShortDateString(), ""), FormatNumber(b.Sum, 2), FormatNumber(score, 1), If(score < 50, "Review for churn risk", "Recently active"), RegisterMarketFilter(b.Rows))
            End If
        Next
        Return dt
    End Function

    Private Function BuildRisk(source As DataTable) As DataTable
        Dim dt As DataTable = ResultTable("Dimension", "Records", "Value", "Risk Score", "Risk Note", "FilterId")
        Dim buckets As Dictionary(Of String, MarketBucket) = GroupBuckets(source, SelectedDimension(), SelectedValue(), "")
        Dim maxValue As Double = Math.Max(1, MaxBucketValue(buckets))
        For Each key As String In SortedKeys(buckets)
            Dim b As MarketBucket = buckets(key)
            Dim score As Double = (b.Sum / maxValue) * 100
            AddRow(dt, key, b.Count.ToString(), FormatNumber(b.Sum, 2), FormatNumber(score, 1), If(score >= 75, "High exposure", If(score >= 40, "Medium exposure", "Lower exposure")), RegisterMarketFilter(b.Rows))
        Next
        Return dt
    End Function

    Private Function BuildInventory(source As DataTable) As DataTable
        Dim usePeriods As Boolean = SelectedDate().Trim() <> "" AndAlso source.Columns.Contains(SelectedDate())
        Dim dt As DataTable = If(usePeriods, ResultTable("Item", "Period", "Units / Movement", "Records", "Velocity", "Inventory Field", "Current Inventory", "Supply Periods", "Safety Stock %", "Reorder Point", "Reorder Needed", "Inventory Note", "FilterId"), ResultTable("Item", "Units / Movement", "Records", "Velocity", "Inventory Field", "Current Inventory", "Supply Periods", "Safety Stock %", "Reorder Point", "Reorder Needed", "Inventory Note", "FilterId"))
        Dim buckets As Dictionary(Of String, MarketBucket) = If(usePeriods, GroupBucketsByPeriod(source, SelectedDimension(), SelectedValue(), SelectedDate(), ""), GroupBuckets(source, SelectedDimension(), SelectedValue(), ""))
        Dim average As Double = OverallAverage(buckets)
        Dim stockField As String = SelectedInventoryField()
        If stockField.Trim() = "" OrElse Not source.Columns.Contains(stockField) Then stockField = FindNumericField(source, New String() {"inventory", "stock", "on hand", "available", "balance"})
        Dim stockFieldText As String = If(stockField.Trim() = "", "Not found", stockField)
        For Each key As String In SortedKeys(buckets)
            Dim b As MarketBucket = buckets(key)
            Dim velocity As Double = b.Sum / Math.Max(1, b.Count)
            Dim currentInventory As Nullable(Of Double) = SumRowsIfFieldExists(b.Rows, stockField)
            Dim reorderPoint As Double = velocity * (1 + Math.Max(0, AssumptionRate()))
            Dim supplyPeriods As Nullable(Of Double) = Nothing
            Dim reorderNeeded As String = ""
            If currentInventory.HasValue Then
                If velocity > 0 Then supplyPeriods = currentInventory.Value / velocity
                reorderNeeded = If(currentInventory.Value <= reorderPoint, "Yes", "No")
            End If
            Dim note As String = If(Not currentInventory.HasValue, "Select inventory field", If(reorderNeeded = "Yes", "Reorder review", If(b.Sum >= average, "Fast movement", "Slow movement")))
            If usePeriods Then
                Dim parts() As String = SplitPeriodKey(key)
                AddRow(dt, parts(0), parts(1), FormatNumber(b.Sum, 2), b.Count.ToString(), FormatNumber(velocity, 2), stockFieldText, NullableNumberText(currentInventory), NullableNumberText(supplyPeriods), FormatNumber(AssumptionRate() * 100, 2), FormatNumber(reorderPoint, 2), reorderNeeded, note, RegisterMarketFilter(b.Rows))
            Else
                AddRow(dt, key, FormatNumber(b.Sum, 2), b.Count.ToString(), FormatNumber(velocity, 2), stockFieldText, NullableNumberText(currentInventory), NullableNumberText(supplyPeriods), FormatNumber(AssumptionRate() * 100, 2), FormatNumber(reorderPoint, 2), reorderNeeded, note, RegisterMarketFilter(b.Rows))
            End If
        Next
        Return dt
    End Function

    Private Function BuildProfit(source As DataTable) As DataTable
        Dim dt As DataTable = ResultTable("Driver", "Revenue", "Direct Cost", "Cost Source", "Cost Rate %", "Estimated Cost", "Estimated Profit", "Margin %", "Profit Contribution %", "Profit Note", "Records", "FilterId")
        Dim costRate As Double = Math.Min(0.95, Math.Max(0, AssumptionRate()))
        If costRate = 0 Then costRate = 0.65
        Dim buckets As Dictionary(Of String, MarketBucket) = GroupBuckets(source, SelectedDimension(), SelectedValue(), "")
        Dim unitCostField As String = FindNumericField(source, New String() {"unit cost", "cost per unit"})
        Dim quantityField As String = FindNumericField(source, New String() {"quantity", "qty", "units"})
        Dim costField As String = FindNumericField(source, New String() {"total cost", "extended cost", "cost amount", "direct cost", "expense"})
        Dim totalProfit As Double = 0
        Dim profits As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        For Each key As String In SortedKeys(buckets)
            Dim revenue As Double = buckets(key).Sum
            Dim directCost As Double = DirectCostForRows(buckets(key).Rows, costField, unitCostField, quantityField)
            Dim estimatedCost As Double = If(directCost > 0, directCost, revenue * costRate)
            Dim profit As Double = revenue - estimatedCost
            profits(key) = profit
            totalProfit += profit
        Next
        For Each key As String In SortedKeys(buckets)
            Dim revenue As Double = buckets(key).Sum
            Dim directCost As Double = DirectCostForRows(buckets(key).Rows, costField, unitCostField, quantityField)
            Dim estimatedCost As Double = If(directCost > 0, directCost, revenue * costRate)
            Dim profit As Double = profits(key)
            Dim sourceText As String = CostSourceForRows(buckets(key).Rows, costField, unitCostField, quantityField, directCost)
            Dim effectiveCostRate As Double = If(revenue = 0, 0, estimatedCost / revenue)
            AddRow(dt, key, FormatNumber(revenue, 2), FormatNumber(directCost, 2), sourceText, FormatNumber(effectiveCostRate * 100, 2), FormatNumber(estimatedCost, 2), FormatNumber(profit, 2), PercentText(profit, revenue), PercentText(profit, totalProfit), ProfitNote(profit, revenue, sourceText), buckets(key).Count.ToString(), RegisterMarketFilter(buckets(key).Rows))
        Next
        Return dt
    End Function

    Private Function BuildScenario(source As DataTable) As DataTable
        Dim dt As DataTable = ResultTable("Dimension", "Current Value", "Downside Value", "Base Value", "Upside Value", "Downside Difference", "Upside Difference", "Scenario Range", "Assumption %", "Scenario Note", "Records", "FilterId")
        Dim rate As Double = AssumptionRate()
        Dim buckets As Dictionary(Of String, MarketBucket) = GroupBuckets(source, SelectedDimension(), SelectedValue(), "")
        For Each key As String In SortedKeys(buckets)
            Dim currentValue As Double = buckets(key).Sum
            Dim downsideValue As Double = currentValue * (1 - Math.Abs(rate))
            Dim upsideValue As Double = currentValue * (1 + Math.Abs(rate))
            Dim downsideDifference As Double = downsideValue - currentValue
            Dim upsideDifference As Double = upsideValue - currentValue
            Dim scenarioRange As Double = upsideValue - downsideValue
            AddRow(dt, key, FormatNumber(currentValue, 2), FormatNumber(downsideValue, 2), FormatNumber(currentValue, 2), FormatNumber(upsideValue, 2), FormatNumber(downsideDifference, 2), FormatNumber(upsideDifference, 2), FormatNumber(scenarioRange, 2), FormatNumber(Math.Abs(rate) * 100, 2), ScenarioRangeNote(scenarioRange, currentValue), buckets(key).Count.ToString(), RegisterMarketFilter(buckets(key).Rows))
        Next
        Return dt
    End Function

    Private Class MarketBucket
        Public Count As Integer
        Public Sum As Double
        Public SecondarySum As Double
        Public HasDate As Boolean
        Public MaxDate As DateTime
        Public Rows As New List(Of DataRow)()
        Public Sub Add(value As Double, dateValue As Nullable(Of DateTime), secondaryValue As Double, Optional sourceRow As DataRow = Nothing)
            Count += 1
            Sum += value
            SecondarySum += secondaryValue
            If sourceRow IsNot Nothing Then Rows.Add(sourceRow)
            If dateValue.HasValue Then
                If Not HasDate OrElse dateValue.Value > MaxDate Then MaxDate = dateValue.Value
                HasDate = True
            End If
        End Sub
        Public Function Average() As Double
            If Count = 0 Then Return 0
            Return Sum / Count
        End Function
        Public Function AverageSecondary() As Double
            If Count = 0 Then Return 0
            Return SecondarySum / Count
        End Function
    End Class

    Private Class ElasticityBucket
        Public Count As Integer
        Public PriceBand As String
        Public PriceSum As Double
        Public QuantitySum As Double
        Public RevenueSum As Double
        Public Rows As New List(Of DataRow)()
        Public Sub Add(price As Double, quantity As Double, sourceRow As DataRow)
            Count += 1
            PriceSum += price
            QuantitySum += quantity
            RevenueSum += price * Math.Max(0, quantity)
            If sourceRow IsNot Nothing Then Rows.Add(sourceRow)
        End Sub
        Public Function AveragePrice() As Double
            If Count = 0 Then Return 0
            Return PriceSum / Count
        End Function
    End Class

    Private Function GroupBuckets(source As DataTable, dimensionField As String, valueField As String, secondaryField As String) As Dictionary(Of String, MarketBucket)
        Dim buckets As New Dictionary(Of String, MarketBucket)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For
            Dim key As String = DimensionKey(row, dimensionField)
            If key = "" Then key = "(blank)"
            If Not buckets.ContainsKey(key) Then buckets.Add(key, New MarketBucket())
            Dim secondaryValue As Double = 0
            If secondaryField <> "" AndAlso source.Columns.Contains(secondaryField) Then secondaryValue = ValueOf(row(secondaryField))
            buckets(key).Add(ValueOf(row(valueField)), DateOf(row, SelectedDate()), secondaryValue, row)
        Next
        Return buckets
    End Function

    Private Function GroupBucketsByPeriod(source As DataTable, dimensionField As String, valueField As String, dateField As String, secondaryField As String) As Dictionary(Of String, MarketBucket)
        Dim buckets As New Dictionary(Of String, MarketBucket)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For

            Dim dimensionText As String = DimensionKey(row, dimensionField)
            If dimensionText = "" Then dimensionText = "(blank)"

            Dim dateValue As Nullable(Of DateTime) = DateOf(row, dateField)
            Dim periodText As String = PeriodLabel(dateValue)
            Dim key As String = dimensionText & ChrW(30) & periodText
            If Not buckets.ContainsKey(key) Then buckets.Add(key, New MarketBucket())

            Dim secondaryValue As Double = 0
            If secondaryField <> "" AndAlso source.Columns.Contains(secondaryField) Then secondaryValue = ValueOf(row(secondaryField))
            buckets(key).Add(ValueOf(row(valueField)), dateValue, secondaryValue, row)
        Next
        Return buckets
    End Function

    Private Function PeriodLabel(dateValue As Nullable(Of DateTime)) As String
        If Not dateValue.HasValue Then Return "(no date)"
        Dim value As DateTime = dateValue.Value
        Select Case SelectedDateAggregation()
            Case "Day"
                Return value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            Case "Week"
                Dim weekNumber As Integer = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                Return value.Year.ToString(CultureInfo.InvariantCulture) & "-W" & weekNumber.ToString("00", CultureInfo.InvariantCulture)
            Case "Quarter"
                Dim quarter As Integer = CInt(Math.Ceiling(value.Month / 3.0))
                Return value.Year.ToString(CultureInfo.InvariantCulture) & "-Q" & quarter.ToString(CultureInfo.InvariantCulture)
            Case "Year"
                Return value.Year.ToString(CultureInfo.InvariantCulture)
            Case Else
                Return value.ToString("yyyy-MM", CultureInfo.InvariantCulture)
        End Select
    End Function

    Private Function SplitPeriodKey(key As String) As String()
        Dim parts() As String = key.Split(ChrW(30))
        If parts.Length < 2 Then Return New String() {key, ""}
        Return New String() {parts(0), parts(1)}
    End Function

    Private Function PriceBuckets(source As DataTable, priceField As String, quantityField As String, Optional dimensionField As String = "") As Dictionary(Of String, MarketBucket)
        Dim buckets As New Dictionary(Of String, MarketBucket)(StringComparer.OrdinalIgnoreCase)
        Dim includeDimension As Boolean = dimensionField.Trim() <> "" AndAlso source IsNot Nothing AndAlso source.Columns.Contains(dimensionField)
        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For
            Dim price As Double = ValueOf(row(priceField))
            Dim band As String = PriceBand(price)
            Dim key As String = band
            If includeDimension Then
                Dim dimensionText As String = FieldText(row(dimensionField))
                If dimensionText.Trim() = "" Then dimensionText = "(blank)"
                key = dimensionText & ChrW(30) & band
            End If
            If Not buckets.ContainsKey(key) Then buckets.Add(key, New MarketBucket())
            Dim qty As Double = 0
            If quantityField <> "" AndAlso source.Columns.Contains(quantityField) Then qty = ValueOf(row(quantityField))
            buckets(key).Add(price * Math.Max(1, qty), Nothing, qty, row)
        Next
        Return buckets
    End Function

    Private Function SplitPriceDimensionKey(key As String) As String()
        Dim parts() As String = key.Split(ChrW(30))
        If parts.Length < 2 Then Return New String() {"", key}
        Return New String() {parts(0), parts(1)}
    End Function

    Private Function SortedPriceBucketKeys(buckets As Dictionary(Of String, MarketBucket), includeDimension As Boolean) As List(Of String)
        Dim keys As New List(Of String)(buckets.Keys)
        If includeDimension Then
            keys.Sort(AddressOf ComparePriceDimensionKeys)
        Else
            keys.Sort(AddressOf ComparePriceBandKeys)
        End If
        Return keys
    End Function

    Private Function ComparePriceDimensionKeys(leftKey As String, rightKey As String) As Integer
        Dim leftParts() As String = SplitPriceDimensionKey(leftKey)
        Dim rightParts() As String = SplitPriceDimensionKey(rightKey)
        Dim dimensionCompare As Integer = String.Compare(leftParts(0), rightParts(0), StringComparison.OrdinalIgnoreCase)
        If dimensionCompare <> 0 Then Return dimensionCompare
        Return ComparePriceBandKeys(leftParts(1), rightParts(1))
    End Function

    Private Function ComparePriceBandKeys(leftBand As String, rightBand As String) As Integer
        Dim leftRank As Integer = PriceBandRank(leftBand)
        Dim rightRank As Integer = PriceBandRank(rightBand)
        If leftRank <> rightRank Then Return leftRank.CompareTo(rightRank)
        Return String.Compare(leftBand, rightBand, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function PriceBandRank(priceBandText As String) As Integer
        Select Case priceBandText
            Case "Under 25"
                Return 1
            Case "25 - 49.99"
                Return 2
            Case "50 - 99.99"
                Return 3
            Case "100 - 249.99"
                Return 4
            Case "250 and over"
                Return 5
            Case Else
                Return 99
        End Select
    End Function

    Private Function PriceBand(price As Double) As String
        If price < 25 Then Return "Under 25"
        If price < 50 Then Return "25 - 49.99"
        If price < 100 Then Return "50 - 99.99"
        If price < 250 Then Return "100 - 249.99"
        Return "250 and over"
    End Function

    Private Function SortedElasticityBuckets(dict As Dictionary(Of String, ElasticityBucket)) As List(Of ElasticityBucket)
        Dim buckets As New List(Of ElasticityBucket)()
        For Each kvp As KeyValuePair(Of String, ElasticityBucket) In dict
            kvp.Value.PriceBand = kvp.Key
            buckets.Add(kvp.Value)
        Next
        buckets.Sort(Function(left As ElasticityBucket, right As ElasticityBucket) left.AveragePrice().CompareTo(right.AveragePrice()))
        Return buckets
    End Function

    Private Function PercentChangeNumber(currentValue As Double, previousValue As Double) As Nullable(Of Double)
        If Math.Abs(previousValue) < 0.000001 Then Return Nothing
        Return ((currentValue - previousValue) / Math.Abs(previousValue)) * 100
    End Function

    Private Function NullableNumberText(value As Nullable(Of Double)) As String
        If Not value.HasValue Then Return ""
        Return FormatNumber(value.Value, 2)
    End Function

    Private Function ElasticityNote(elasticity As Nullable(Of Double)) As String
        If Not elasticity.HasValue Then Return "Base band"
        Dim absoluteValue As Double = Math.Abs(elasticity.Value)
        If absoluteValue >= 1.25 Then Return "Elastic demand"
        If absoluteValue >= 0.75 Then Return "Near unit elasticity"
        Return "Inelastic demand"
    End Function

    Private Function FindNumericField(source As DataTable, hints() As String) As String
        If source Is Nothing Then Return ""
        For Each hint As String In hints
            For Each col As DataColumn In source.Columns
                If col.ColumnName.IndexOf(hint, StringComparison.OrdinalIgnoreCase) >= 0 AndAlso ColumnLooksNumeric(source, col) Then Return col.ColumnName
            Next
        Next
        Return ""
    End Function

    Private Function ColumnLooksNumeric(source As DataTable, col As DataColumn) As Boolean
        If ColumnTypeIsNumeric(col) Then Return True
        Dim checkedRows As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = FieldText(row(col)).Replace("$", "").Replace(",", "").Trim()
            If text = "" Then Continue For
            checkedRows += 1
            Dim value As Double
            If Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, value) OrElse Double.TryParse(text, value) Then Return True
            If checkedRows >= 25 Then Exit For
        Next
        Return False
    End Function

    Private Function SumRowsIfFieldExists(rows As List(Of DataRow), fieldName As String) As Nullable(Of Double)
        If rows Is Nothing OrElse rows.Count = 0 OrElse fieldName.Trim() = "" OrElse Not rows(0).Table.Columns.Contains(fieldName) Then Return Nothing
        Dim total As Double = 0
        For Each row As DataRow In rows
            total += ValueOf(row(fieldName))
        Next
        Return total
    End Function

    Private Function DirectCostForRows(rows As List(Of DataRow), costField As String, unitCostField As String, quantityField As String) As Double
        If rows Is Nothing OrElse rows.Count = 0 Then Return 0
        Dim total As Double = 0
        If costField.Trim() <> "" AndAlso rows(0).Table.Columns.Contains(costField) Then
            For Each row As DataRow In rows
                total += ValueOf(row(costField))
            Next
            Return total
        End If
        If unitCostField.Trim() <> "" AndAlso quantityField.Trim() <> "" AndAlso rows(0).Table.Columns.Contains(unitCostField) AndAlso rows(0).Table.Columns.Contains(quantityField) Then
            For Each row As DataRow In rows
                total += ValueOf(row(unitCostField)) * ValueOf(row(quantityField))
            Next
        End If
        Return total
    End Function

    Private Function CostSourceForRows(rows As List(Of DataRow), costField As String, unitCostField As String, quantityField As String, directCost As Double) As String
        If rows Is Nothing OrElse rows.Count = 0 OrElse directCost <= 0 Then Return "Assumption %"
        If costField.Trim() <> "" AndAlso rows(0).Table.Columns.Contains(costField) Then Return costField
        If unitCostField.Trim() <> "" AndAlso quantityField.Trim() <> "" AndAlso rows(0).Table.Columns.Contains(unitCostField) AndAlso rows(0).Table.Columns.Contains(quantityField) Then Return unitCostField & " * " & quantityField
        Return "Assumption %"
    End Function

    Private Function ProfitNote(profit As Double, revenue As Double, costSource As String) As String
        If revenue = 0 Then Return "No revenue"
        Dim margin As Double = profit / revenue
        Dim prefix As String = If(costSource = "Assumption %", "Estimated cost", "Direct cost")
        If margin >= 0.25 Then Return prefix & "; strong margin"
        If margin >= 0.1 Then Return prefix & "; moderate margin"
        If margin >= 0 Then Return prefix & "; thin margin"
        Return prefix & "; loss review"
    End Function

    Private Function ScenarioRangeNote(scenarioRange As Double, currentValue As Double) As String
        If currentValue = 0 Then Return "No base value"
        Dim rangePercent As Double = Math.Abs(scenarioRange / currentValue)
        If rangePercent >= 0.4 Then Return "Wide scenario range"
        If rangePercent >= 0.15 Then Return "Moderate scenario range"
        Return "Narrow scenario range"
    End Function

    Private Function ScenarioNote(difference As Double) As String
        If difference > 0 Then Return "Increase"
        If difference < 0 Then Return "Decrease"
        Return "No change"
    End Function

    Private Function ResultTable(ParamArray columns() As String) As DataTable
        Dim dt As New DataTable()
        For Each columnName As String In columns
            dt.Columns.Add(columnName, GetType(String))
        Next
        Return dt
    End Function

    Private Sub AddRow(dt As DataTable, ParamArray values() As String)
        Dim row As DataRow = dt.NewRow()
        For i As Integer = 0 To Math.Min(values.Length, dt.Columns.Count) - 1
            row(i) = values(i)
        Next
        dt.Rows.Add(row)
    End Sub

    Private Function RowsForBasketPair(source As DataTable, orderField As String, itemField As String, itemA As String, itemB As String) As List(Of DataRow)
        Dim rows As New List(Of DataRow)()
        If source Is Nothing OrElse Not source.Columns.Contains(orderField) OrElse Not source.Columns.Contains(itemField) Then Return rows

        Dim matchingOrders As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim orderItems As New Dictionary(Of String, Dictionary(Of String, Boolean))(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For
            Dim orderId As String = FieldText(row(orderField))
            Dim itemText As String = FieldText(row(itemField))
            If orderId = "" OrElse itemText = "" Then Continue For
            If Not orderItems.ContainsKey(orderId) Then orderItems.Add(orderId, New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase))
            If Not orderItems(orderId).ContainsKey(itemText) Then orderItems(orderId).Add(itemText, True)
        Next

        For Each kvp As KeyValuePair(Of String, Dictionary(Of String, Boolean)) In orderItems
            If kvp.Value.ContainsKey(itemA) AndAlso kvp.Value.ContainsKey(itemB) Then matchingOrders(kvp.Key) = True
        Next

        For Each row As DataRow In source.Rows
            If Not RowMatchesSearch(row) Then Continue For
            Dim orderId As String = FieldText(row(orderField))
            Dim itemText As String = FieldText(row(itemField))
            If matchingOrders.ContainsKey(orderId) AndAlso (String.Equals(itemText, itemA, StringComparison.OrdinalIgnoreCase) OrElse String.Equals(itemText, itemB, StringComparison.OrdinalIgnoreCase)) Then rows.Add(row)
        Next

        Return rows
    End Function

    Private Function RegisterMarketFilter(rows As List(Of DataRow)) As String
        Dim rowFilter As String = RowsFilter(rows)
        If rowFilter.Trim() = "" Then Return ""

        Dim filters As Dictionary(Of String, String) = TryCast(Session("MarketFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("MarketFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = rowFilter
        Return filterId
    End Function

    Private Function RowsFilter(rows As List(Of DataRow)) As String
        If rows Is Nothing OrElse rows.Count = 0 Then Return ""

        Dim filters As New List(Of String)()
        Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In rows
            Dim rowFilter As String = SingleRowFilter(row)
            If rowFilter.Trim() <> "" AndAlso Not seen.ContainsKey(rowFilter) Then
                filters.Add("(" & rowFilter & ")")
                seen(rowFilter) = True
            End If
        Next

        Return String.Join(" OR ", filters.ToArray())
    End Function

    Private Function SingleRowFilter(row As DataRow) As String
        Dim filters As New List(Of String)()
        For Each col As DataColumn In row.Table.Columns
            filters.Add(ValueFilter(col, row(col)))
        Next
        Return String.Join(" AND ", filters.ToArray())
    End Function

    Private Function ValueFilter(col As DataColumn, valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return FieldRef(col) & " IS NULL"

        Dim valueText As String = FieldText(valueObject)
        If valueText.Trim() = "" Then
            If ColumnTypeIsNumeric(col) OrElse col.DataType Is GetType(DateTime) Then Return FieldRef(col) & " IS NULL"
            Return "(" & FieldRef(col) & " IS NULL OR " & FieldRef(col) & " = '')"
        End If

        Dim numericValue As Double
        If ColumnTypeIsNumeric(col) AndAlso (Double.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, numericValue) OrElse Double.TryParse(valueText, numericValue)) Then
            Return FieldRef(col) & " = " & numericValue.ToString(CultureInfo.InvariantCulture)
        End If

        Dim dateValue As DateTime
        If col.DataType Is GetType(DateTime) AndAlso DateTime.TryParse(valueText, dateValue) Then
            Dim startDate As DateTime = dateValue.Date
            Dim nextDate As DateTime = startDate.AddDays(1)
            Return "(" & FieldRef(col) & " >= #" & startDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "# AND " & FieldRef(col) & " < #" & nextDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "#)"
        End If

        Return FieldRef(col) & " = '" & EscapeFilterValue(valueText) & "'"
    End Function

    Private Function ColumnTypeIsNumeric(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(SByte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(UShort) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(UInteger) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(ULong) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Function EscapeFilterValue(valueText As String) As String
        Return valueText.Replace("'", "''")
    End Function

    Private Function RowMatchesSearch(row As DataRow) As Boolean
        Dim searchBox As TextBox = TextBoxControl("txtSearch")
        Dim searchText As String = If(searchBox Is Nothing, "", searchBox.Text.Trim())
        If searchText = "" Then Return True
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 Then Return True
        Next
        Return False
    End Function

    Private Function SelectedDimension() As String
        Return SelectedValue("DropDownDimension")
    End Function

    Private Function SelectedDimensionFields() As List(Of String)
        Dim fields As New List(Of String)()
        If SupportsMultiPrimary() Then
            Dim listBox As ListBox = ListBoxControl("ListBoxDimension")
            If listBox IsNot Nothing Then
                For Each item As ListItem In listBox.Items
                    If item.Selected AndAlso item.Value.Trim() <> "" Then fields.Add(item.Value)
                Next
            End If
        End If

        If fields.Count = 0 Then
            Dim selectedSingle As String = SelectedDimension()
            If selectedSingle.Trim() <> "" Then fields.Add(selectedSingle)
        End If

        Return fields
    End Function

    Private Function DimensionKey(row As DataRow, fallbackDimensionField As String) As String
        Dim fields As List(Of String) = SelectedDimensionFields()
        Dim values As New List(Of String)()

        For Each fieldName As String In fields
            If row.Table.Columns.Contains(fieldName) Then
                Dim valueText As String = FieldText(row(fieldName))
                If valueText.Trim() = "" Then valueText = "(blank)"
                values.Add(valueText)
            End If
        Next

        If values.Count = 0 AndAlso fallbackDimensionField <> "" AndAlso row.Table.Columns.Contains(fallbackDimensionField) Then
            Dim fallbackText As String = FieldText(row(fallbackDimensionField))
            If fallbackText.Trim() = "" Then fallbackText = "(blank)"
            values.Add(fallbackText)
        End If

        Return String.Join(" | ", values.ToArray())
    End Function

    Private Function SelectedValue() As String
        Return SelectedValue("DropDownValueField")
    End Function

    Private Function SelectedDate() As String
        Return SelectedValue("DropDownDateField")
    End Function

    Private Function SelectedDateAggregation() As String
        Dim selectedAggregation As String = SelectedValue("DropDownDateAggregation")
        If selectedAggregation.Trim() = "" Then Return "Month"
        Return selectedAggregation
    End Function

    Private Function SelectedSecondary() As String
        Return SelectedValue("DropDownSecondaryField")
    End Function

    Private Function SelectedInventoryField() As String
        Return SelectedValue("DropDownInventoryField")
    End Function

    Private Function SelectedValue(id As String) As String
        Dim ddl As DropDownList = DropDownControl(id)
        If ddl Is Nothing OrElse ddl.SelectedValue Is Nothing Then Return ""
        Return ddl.SelectedValue
    End Function

    Private Function AssumptionRate() As Double
        Dim txt As TextBox = TextBoxControl("txtAssumption")
        Dim value As Double = 10
        If txt IsNot Nothing Then Double.TryParse(txt.Text.Trim(), value)
        Return value / 100
    End Function

    Private Function ValueOf(valueObject As Object) As Double
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return 0
        Dim text As String = valueObject.ToString().Replace("$", "").Replace(",", "").Trim()
        Dim value As Double
        If Double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, value) OrElse Double.TryParse(text, value) Then Return value
        Return 0
    End Function

    Private Function DateOf(row As DataRow, dateField As String) As Nullable(Of DateTime)
        If dateField = "" OrElse Not row.Table.Columns.Contains(dateField) Then Return Nothing
        Dim value As DateTime
        If DateTime.TryParse(FieldText(row(dateField)), value) Then Return value
        Return Nothing
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Function BucketTotal(buckets As Dictionary(Of String, MarketBucket)) As Double
        Dim total As Double = 0
        For Each b As MarketBucket In buckets.Values
            total += b.Sum
        Next
        Return total
    End Function

    Private Function OverallAverage(buckets As Dictionary(Of String, MarketBucket)) As Double
        If buckets.Count = 0 Then Return 0
        Return BucketTotal(buckets) / buckets.Count
    End Function

    Private Function MaxBucketValue(buckets As Dictionary(Of String, MarketBucket)) As Double
        Dim maxValue As Double = 0
        For Each b As MarketBucket In buckets.Values
            If b.Sum > maxValue Then maxValue = b.Sum
        Next
        Return maxValue
    End Function

    Private Function MaxBucketDate(buckets As Dictionary(Of String, MarketBucket)) As DateTime
        Dim maxDate As DateTime = DateTime.MinValue
        For Each b As MarketBucket In buckets.Values
            If b.HasDate AndAlso b.MaxDate > maxDate Then maxDate = b.MaxDate
        Next
        If maxDate = DateTime.MinValue Then maxDate = DateTime.Today
        Return maxDate
    End Function

    Private Function PercentText(value As Double, total As Double) As String
        If total = 0 Then Return "0.00"
        Return FormatNumber((value / total) * 100, 2)
    End Function

    Private Function SortedKeys(Of T)(dict As Dictionary(Of String, T)) As List(Of String)
        Dim keys As New List(Of String)(dict.Keys)
        keys.Sort()
        Return keys
    End Function

    Private Sub ExportMarket(formatName As String)
        Dim dt As DataTable = TryCast(Session(MarketSessionKey("Table")), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindMarket()
            dt = TryCast(Session(MarketSessionKey("Table")), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Exit Sub

        Dim delimiter As String = If(formatName = "xls", Chr(9), ",")
        Dim extension As String = If(formatName = "xls", "xls", "csv")
        Response.Clear()
        Response.ContentType = If(formatName = "xls", "application/vnd.ms-excel", "text/csv")
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & MarketModel & "MarketModel." & extension)
        Response.Write(ExportToCSVtext(dt, delimiter, MarketTitle, ""))
        Response.Flush()
        HttpContext.Current.ApplicationInstance.CompleteRequest()
    End Sub

    Private Function MarketSessionKey(name As String) As String
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString().Trim()
        If repid = "" Then repid = "Sample"
        Return "Market_" & MarketModel & "_" & repid & "_" & name
    End Function

    Private Function DropDownControl(id As String) As DropDownList
        Return TryCast(FindRecursive(Me, id), DropDownList)
    End Function

    Private Function ListBoxControl(id As String) As ListBox
        Return TryCast(FindRecursive(Me, id), ListBox)
    End Function

    Private Function TextBoxControl(id As String) As TextBox
        Return TryCast(FindRecursive(Me, id), TextBox)
    End Function

    Private Function LabelControl(id As String) As Label
        Return TryCast(FindRecursive(Me, id), Label)
    End Function

    Private Function HyperLinkControl(id As String) As HyperLink
        Return TryCast(FindRecursive(Me, id), HyperLink)
    End Function

    Private Function LinkButtonControl(id As String) As LinkButton
        Return TryCast(FindRecursive(Me, id), LinkButton)
    End Function

    Private Function PanelControl(id As String) As Panel
        Return TryCast(FindRecursive(Me, id), Panel)
    End Function

    Private Function GridControl(id As String) As GridView
        Return TryCast(FindRecursive(Me, id), GridView)
    End Function

    Private Sub SetLabel(id As String, text As String)
        Dim lbl As Label = LabelControl(id)
        If lbl IsNot Nothing Then lbl.Text = text
    End Sub

    Private Function FindRecursive(root As Control, id As String) As Control
        If root Is Nothing Then Return Nothing
        If root.ID = id Then Return root
        For Each child As Control In root.Controls
            Dim found As Control = FindRecursive(child, id)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function
End Class
