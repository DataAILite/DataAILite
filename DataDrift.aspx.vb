Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class DataDrift
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub DataDrift_Init(sender As Object, e As EventArgs) Handles Me.Init
        MenuExpansionHelper.Attach(Me)
        AnalyticsDashboardTileHelper.Attach(Me)
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Data Drift Analysis"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Data%20Drift%20Analysis"
    End Sub

    Private Sub DataDrift_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            ApplyUrlParameters()
            BuildAndBindAnalysis()
        ElseIf Session("DataDriftTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("DataDriftTable"), DataTable))
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
                Session("DataDriftTableSource") = existingTable
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
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            LabelError.Text = "No data. Run or import report data first."
            Session("DataDriftTableSource") = Nothing
            Return Nothing
        End If
        Session("DataDriftTableSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("DataDriftTableSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("DataDriftTableSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewDataDrift.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        GridViewDataDrift.PageIndex = 0
        FillFieldLists()
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkDataDriftAI_Click(sender As Object, e As EventArgs) Handles lnkDataDriftAI.Click
        Dim dt As DataTable = TryCast(Session("DataDriftTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("DataDriftTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data drift analysis results to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Data Drift Analysis", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this data drift analysis grid. Explain the strongest findings, unusual records, and business meaning of the results.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing Then Exit Sub
        DropDownPrimaryField.Items.Clear()
        DropDownSecondaryField.Items.Clear()



        For Each col As DataColumn In source.Columns
            DropDownPrimaryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            DropDownSecondaryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))



        Next



        FillSegmentValues()
    End Sub

    Private Sub DropDownSecondaryField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownSecondaryField.SelectedIndexChanged
        FillSegmentValues()
        BuildAndBindAnalysis()
    End Sub

    Private Sub FillSegmentValues()
        DropDownBaseValue.Items.Clear()
        DropDownCompareValue.Items.Clear()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse DropDownSecondaryField.Items.Count = 0 OrElse DropDownSecondaryField.SelectedValue = "" Then Exit Sub
        Dim values As New List(Of String)()
        Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            Dim v As String = FieldText(row(DropDownSecondaryField.SelectedValue)).Trim()
            If v = "" Then v = "(blank)"
            If Not seen.ContainsKey(v) Then
                seen(v) = True
                values.Add(v)
            End If
            If values.Count >= 200 Then Exit For
        Next
        For Each v As String In values
            DropDownBaseValue.Items.Add(New ListItem(v, v))
            DropDownCompareValue.Items.Add(New ListItem(v, v))
        Next
        If DropDownCompareValue.Items.Count > 1 Then DropDownCompareValue.SelectedIndex = 1
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As New DataTable()
        output.Columns.Add("Field Value", GetType(String))
        output.Columns.Add("Base Records", GetType(Integer))
        output.Columns.Add("Compare Records", GetType(Integer))
        output.Columns.Add("Base Share %", GetType(Double))
        output.Columns.Add("Compare Share %", GetType(Double))
        output.Columns.Add("Drift Points", GetType(Double))
        output.Columns.Add("BaseFilterId", GetType(String))
        output.Columns.Add("CompareFilterId", GetType(String))
        If source Is Nothing OrElse DropDownPrimaryField.SelectedValue = "" OrElse DropDownSecondaryField.SelectedValue = "" OrElse DropDownBaseValue.SelectedValue = "" OrElse DropDownCompareValue.SelectedValue = "" Then BindAnalysisGrid(output) : Return
        Dim compareCol As String = DropDownPrimaryField.SelectedValue
        Dim segCol As String = DropDownSecondaryField.SelectedValue
        Dim baseVal As String = DropDownBaseValue.SelectedValue
        Dim compVal As String = DropDownCompareValue.SelectedValue
        Dim baseCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim compCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim baseTotal As Integer = 0
        Dim compTotal As Integer = 0
        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim seg As String = FieldText(row(segCol)).Trim()
            If seg = "" Then seg = "(blank)"
            Dim fieldVal As String = FieldText(row(compareCol)).Trim()
            If fieldVal = "" Then fieldVal = "(blank)"
            If seg.Equals(baseVal, StringComparison.OrdinalIgnoreCase) Then
                If Not baseCounts.ContainsKey(fieldVal) Then baseCounts(fieldVal) = 0
                baseCounts(fieldVal) += 1
                baseTotal += 1
            ElseIf seg.Equals(compVal, StringComparison.OrdinalIgnoreCase) Then
                If Not compCounts.ContainsKey(fieldVal) Then compCounts(fieldVal) = 0
                compCounts(fieldVal) += 1
                compTotal += 1
            End If
        Next
        Dim keys3 As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each k As String In baseCounts.Keys : keys3(k) = True : Next
        For Each k As String In compCounts.Keys : keys3(k) = True : Next
        For Each k As String In keys3.Keys
            Dim br As Integer = If(baseCounts.ContainsKey(k), baseCounts(k), 0)
            Dim cr As Integer = If(compCounts.ContainsKey(k), compCounts(k), 0)
            Dim bs As Double = If(baseTotal = 0, 0, br * 100.0 / baseTotal)
            Dim cs As Double = If(compTotal = 0, 0, cr * 100.0 / compTotal)
            Dim r As DataRow = output.NewRow()
            r("Field Value") = k
            r("Base Records") = br
            r("Compare Records") = cr
            r("Base Share %") = Math.Round(bs, 2)
            r("Compare Share %") = Math.Round(cs, 2)
            r("Drift Points") = Math.Round(cs - bs, 2)
            r("BaseFilterId") = RegisterAnalysisFilter(FilterAnd(FilterEquals(segCol, baseVal), FilterEquals(compareCol, k)))
            r("CompareFilterId") = RegisterAnalysisFilter(FilterAnd(FilterEquals(segCol, compVal), FilterEquals(compareCol, k)))
            output.Rows.Add(r)
        Next
        output.DefaultView.Sort = "Drift Points DESC"
        BindAnalysisGrid(output.DefaultView.ToTable())
    End Sub

    Private Sub GridViewDataDrift_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewDataDrift.RowDataBound
        If e.Row.Cells.Count = 0 Then Exit Sub
        Dim dt As DataTable = TryCast(Session("DataDriftTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        HideFilterColumnsInRow(dt, e.Row)
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Base Records", "BaseFilterId", "driftfilter")
        AddRecordLink(e.Row, dt, "Compare Records", "CompareFilterId", "driftfilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("DataDriftTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewDataDrift.AllowPaging = False
            GridViewDataDrift.PageIndex = 0
            GridViewDataDrift.DataSource = Nothing
            GridViewDataDrift.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If
        GridViewDataDrift.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewDataDrift.PageSize = AnalysisGridPageSize
        If Not GridViewDataDrift.AllowPaging Then GridViewDataDrift.PageIndex = 0
        GridViewDataDrift.DataSource = dt
        GridViewDataDrift.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Data Drift Analysis (" & dt.Rows.Count.ToString() & " rows)"
        AnalysisExportSnapshot.Save(Me, "DataDriftAnalysis", "Data Drift Analysis", LabelInfo, GridViewDataDrift, dt)
    End Sub

    Private Sub HideFilterColumnsInRow(dt As DataTable, row As GridViewRow)
        HideColumn(dt, row, "FilterId")
        HideColumn(dt, row, "BaseFilterId")
        HideColumn(dt, row, "CompareFilterId")
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
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewDataDrift.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewDataDrift.PageIndex < (GridViewDataDrift.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewDataDrift.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewDataDrift.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewDataDrift.PageIndex > 0 Then GridViewDataDrift.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewDataDrift.PageIndex < (GridViewDataDrift.PageCount - 1) Then GridViewDataDrift.PageIndex += 1
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
            GridViewDataDrift.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("DataDriftTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("DataDriftTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data drift analysis results to export."
            Exit Sub
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "DataDrift_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Data Drift Analysis", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Data Drift Analysis", ""))
        End If
        Response.End()
    End Sub

    Private Function GridTableForAI(dt As DataTable) As DataTable
        If dt Is Nothing Then Return Nothing
        Dim aiTable As DataTable = dt.Copy()
        Dim hiddenColumns() As String = {"FilterId", "BaseFilterId", "CompareFilterId"}
        For Each columnName As String In hiddenColumns
            If aiTable.Columns.Contains(columnName) Then aiTable.Columns.Remove(columnName)
        Next
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
            If bullet IsNot Nothing AndAlso bullet.Trim() <> "" Then
                html &= "<li>" & Server.HtmlEncode(bullet) & "</li>"
            End If
        Next
        html &= "</ul></div>"
        Return html
    End Function
    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Compare Field selects the value whose distribution is being compared.", "Segment Field selects the field containing base and compare groups or periods.", "Base Value and Compare Value select the two segments to compare.", "Search filters records before drift is calculated.", "Use dates, periods, regions, channels, statuses, or source systems as segment fields.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "Data drift model compares distributions between two selected segments.", "The page counts each Compare Field value in the base segment and compare segment.", "Counts are converted to share percentages inside each segment.", "Drift points are calculated as Compare Share minus Base Share.", "Large positive or negative drift means the distribution changed materially.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Field Value is the compared category or value.", "Base Records and Compare Records link to the rows on each side.", "Base Share and Compare Share show relative distribution inside each segment.", "Drift Points shows the percentage-point change.", "Use this page to detect data mix changes, channel shifts, product mix changes, or source drift.")
        ReadinessFooterGuidance.SetFooter(Me, "Data Drift Analysis", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
    End Sub

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
            Case "Year"
                Return d.Year.ToString()
            Case "Quarter"
                Return d.Year.ToString() & " Q" & (((d.Month - 1) \ 3) + 1).ToString()
            Case Else
                Return d.ToString("yyyy-MM")
        End Select
    End Function

    Private Function ContainsSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        Dim needle As String = searchText.ToLowerInvariant()
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).ToLowerInvariant().Contains(needle) Then Return True
        Next
        Return False
    End Function

    Private Function FilterEquals(columnName As String, valueText As String) As String
        Return "[" & columnName.Replace("]", "]]" ) & "] = '" & valueText.Replace("'", "''") & "'"
    End Function

    Private Function FilterAnd(leftFilter As String, rightFilter As String) As String
        If leftFilter.Trim() = "" Then Return rightFilter
        If rightFilter.Trim() = "" Then Return leftFilter
        Return "(" & leftFilter & ") AND (" & rightFilter & ")"
    End Function

    Private Function RegisterAnalysisFilter(filterExpression As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("DataDriftFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("DataDriftFilters") = filters
        Return filterId
    End Function

    Private Sub ApplyUrlParameters()
        UrlInputHelper.ApplyDropDown(Me, "DropDownPrimaryField", "cat1")
        UrlInputHelper.ApplyDropDown(Me, "DropDownSecondaryField", "cat2")
        FillSegmentValues()
        UrlInputHelper.ApplyDropDown(Me, "DropDownBaseValue", "base")
        UrlInputHelper.ApplyDropDown(Me, "DropDownCompareValue", "compare")
        UrlInputHelper.ApplyTextBox(Me, "txtSearch", "search")
    End Sub

End Class
