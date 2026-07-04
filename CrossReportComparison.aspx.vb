Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class CrossReportComparison
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub CrossReportComparison_Init(sender As Object, e As EventArgs) Handles Me.Init
        MenuExpansionHelper.Attach(Me)
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Cross-Report Comparison"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Cross-Report%20Comparison"
    End Sub

    Private Sub CrossReportComparison_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadBaseData()
            FillCompareReports()
            FillFieldLists()
            RestoreSelections()
            BuildAndBindAnalysis()
        ElseIf Session("CrossReportComparisonTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("CrossReportComparisonTable"), DataTable))
        End If
    End Sub

    Private Function LoadBaseData() As DataTable
        LabelError.Text = ""
        Dim dvExisting As DataView = TryCast(Session("dv3"), DataView)
        If dvExisting IsNot Nothing AndAlso dvExisting.Table IsNot Nothing AndAlso dvExisting.Table.Rows.Count > 0 Then
            Session("CrossReportBaseSource") = dvExisting.Table
            Return dvExisting.Table
        End If
        Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
        If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
            Session("CrossReportBaseSource") = existingTable
            Return existingTable
        End If
        Dim repid As String = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString())
        If repid.Trim() = "" Then
            LabelError.Text = "Report is not selected."
            Return Nothing
        End If
        Return LoadReportTable(repid, "CrossReportBaseSource")
    End Function

    Private Function LoadReportTable(reportId As String, sessionKey As String) As DataTable
        Dim ret As String = ""
        Try
            Dim dv As DataView = RetrieveReportData(reportId, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If ret.Trim() <> "" Then LabelError.Text = ret
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Session(sessionKey) = dv.Table
                Return dv.Table
            End If
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        End Try
        Return Nothing
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("CrossReportBaseSource") Is Nothing Then Return LoadBaseData()
        Return CType(Session("CrossReportBaseSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        SaveSelections()
        GridViewCrossReport.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        If DropDownCompareReport.Items.Count > 0 Then DropDownCompareReport.SelectedIndex = 0
        txtSearch.Text = ""
        DropDownAggregation.SelectedIndex = 0
        FillFieldLists()
        SaveSelections()
        GridViewCrossReport.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkCrossReportAI_Click(sender As Object, e As EventArgs) Handles lnkCrossReportAI.Click
        Dim dt As DataTable = TryCast(Session("CrossReportComparisonTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("CrossReportComparisonTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No cross-report comparison rows to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Cross-Report Comparison", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this cross-report comparison. Explain the largest differences, missing keys, percent changes, and what should be checked next.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        DropDownKeyField.Items.Clear()
        DropDownValueField.Items.Clear()
        DropDownKeyField.Items.Add(New ListItem("(all records)", ""))
        DropDownValueField.Items.Add(New ListItem("(records)", ""))
        If source Is Nothing Then Exit Sub
        For Each col As DataColumn In source.Columns
            DropDownKeyField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If ColumnTypeIsNumeric(col) Then DropDownValueField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
        Next
    End Sub

    Private Sub FillCompareReports()
        DropDownCompareReport.Items.Clear()
        DropDownCompareReport.Items.Add(New ListItem("(select report)", ""))

        Dim reports As DataTable = TryCast(Session("ListOfReportsForComparison"), DataTable)
        If reports Is Nothing OrElse Not reports.Columns.Contains("ReportID") OrElse Not reports.Columns.Contains("ReportTitle") Then
            LabelError.Text = "Open List of Reports first to load the accessible report list."
            Exit Sub
        End If

        Dim currentReportId As String = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString().Trim())
        Dim sortedReports As DataView = reports.DefaultView
        sortedReports.Sort = "ReportTitle ASC, ReportID ASC"
        For Each reportRow As DataRowView In sortedReports
            Dim reportId As String = FieldText(reportRow("ReportID")).Trim()
            Dim reportTitle As String = FieldText(reportRow("ReportTitle")).Trim()
            If reportId = "" OrElse reportId.Equals(currentReportId, StringComparison.OrdinalIgnoreCase) Then Continue For
            If reportTitle = "" Then reportTitle = reportId
            If DropDownCompareReport.Items.FindByValue(reportId) Is Nothing Then
                DropDownCompareReport.Items.Add(New ListItem(reportTitle, reportId))
            End If
        Next
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim baseTable As DataTable = GetSourceTable()
        Dim output As DataTable = OutputTable()
        If baseTable Is Nothing Then
            BindAnalysisGrid(output)
            Return
        End If
        If DropDownCompareReport.SelectedValue.Trim() = "" Then
            AddInfoRow(output, "Setup", "Select a Compare Report and click Build.", baseTable.Rows.Count)
            BindAnalysisGrid(output)
            Return
        End If
        Dim compareTable As DataTable = LoadReportTable(DropDownCompareReport.SelectedValue.Trim(), "CrossReportCompareSource")
        If compareTable Is Nothing Then
            AddInfoRow(output, "Compare report not loaded", "Return to List of Reports and confirm access to the selected report.", baseTable.Rows.Count)
            BindAnalysisGrid(output)
            Return
        End If
        Dim keyField As String = DropDownKeyField.SelectedValue
        Dim valueField As String = DropDownValueField.SelectedValue
        If keyField <> "" AndAlso Not compareTable.Columns.Contains(keyField) Then
            AddInfoRow(output, "Key field missing", "The compare report does not contain key field " & keyField & ".", baseTable.Rows.Count)
            BindAnalysisGrid(output)
            Return
        End If
        If valueField <> "" AndAlso Not compareTable.Columns.Contains(valueField) Then
            AddInfoRow(output, "Value field missing", "The compare report does not contain value field " & valueField & ".", baseTable.Rows.Count)
            BindAnalysisGrid(output)
            Return
        End If
        Dim baseSummary As Dictionary(Of String, SummaryValue) = Summarize(baseTable, keyField, valueField, txtSearch.Text)
        Dim compareSummary As Dictionary(Of String, SummaryValue) = Summarize(compareTable, keyField, valueField, "")
        Dim keys As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each k As String In baseSummary.Keys
            keys(k) = True
        Next
        For Each k As String In compareSummary.Keys
            keys(k) = True
        Next
        For Each k As String In keys.Keys
            Dim b As SummaryValue = If(baseSummary.ContainsKey(k), baseSummary(k), New SummaryValue())
            Dim c As SummaryValue = If(compareSummary.ContainsKey(k), compareSummary(k), New SummaryValue())
            Dim baseValue As Double = FinalValue(b)
            Dim compareValue As Double = FinalValue(c)
            Dim diff As Double = compareValue - baseValue
            Dim pct As String = ""
            If baseValue <> 0 Then pct = Math.Round(diff * 100.0 / Math.Abs(baseValue), 2).ToString() & "%"
            Dim r As DataRow = output.NewRow()
            r("Key") = k
            r("Base Records") = b.Count
            r("Compare Records") = c.Count
            r("Base Value") = Math.Round(baseValue, 4)
            r("Compare Value") = Math.Round(compareValue, 4)
            r("Difference") = Math.Round(diff, 4)
            r("Difference %") = pct
            r("Status") = If(b.Count = 0, "Only in compare", If(c.Count = 0, "Only in base", If(Math.Abs(diff) > 0, "Changed", "Same")))
            r("BaseFilterId") = RegisterAnalysisFilter(If(keyField = "", "1 = 1", FilterEquals(keyField, k)))
            output.Rows.Add(r)
        Next
        output.DefaultView.Sort = "Status ASC, Key ASC"
        BindAnalysisGrid(output.DefaultView.ToTable())
    End Sub

    Private Function OutputTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Base Records", GetType(Integer))
        dt.Columns.Add("Compare Records", GetType(Integer))
        dt.Columns.Add("Base Value", GetType(Double))
        dt.Columns.Add("Compare Value", GetType(Double))
        dt.Columns.Add("Difference", GetType(Double))
        dt.Columns.Add("Difference %", GetType(String))
        dt.Columns.Add("Status", GetType(String))
        dt.Columns.Add("BaseFilterId", GetType(String))
        Return dt
    End Function

    Private Sub AddInfoRow(output As DataTable, statusText As String, message As String, records As Integer)
        Dim r As DataRow = output.NewRow()
        r("Key") = message
        r("Base Records") = records
        r("Compare Records") = 0
        r("Base Value") = 0
        r("Compare Value") = 0
        r("Difference") = 0
        r("Difference %") = ""
        r("Status") = statusText
        r("BaseFilterId") = RegisterAnalysisFilter("1 = 1")
        output.Rows.Add(r)
    End Sub

    Private Class SummaryValue
        Public Count As Integer
        Public Sum As Double
        Public Min As Double
        Public Max As Double
        Public Sub Add(value As Double)
            If Count = 0 Then Min = value : Max = value
            If value < Min Then Min = value
            If value > Max Then Max = value
            Count += 1
            Sum += value
        End Sub
    End Class

    Private Function Summarize(source As DataTable, keyField As String, valueField As String, searchText As String) As Dictionary(Of String, SummaryValue)
        Dim result As New Dictionary(Of String, SummaryValue)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If searchText.Trim() <> "" AndAlso Not ContainsSearch(row, searchText) Then Continue For
            Dim key As String = If(keyField = "", "All Records", FieldText(row(keyField)).Trim())
            If key = "" Then key = "(blank)"
            If Not result.ContainsKey(key) Then result(key) = New SummaryValue()
            Dim value As Double = If(valueField = "", 1, NumericValue(row(valueField)))
            result(key).Add(value)
        Next
        Return result
    End Function

    Private Function FinalValue(summary As SummaryValue) As Double
        Select Case DropDownAggregation.SelectedValue
            Case "Sum"
                Return summary.Sum
            Case "Average"
                If summary.Count = 0 Then Return 0 Else Return summary.Sum / summary.Count
            Case "Min"
                Return summary.Min
            Case "Max"
                Return summary.Max
            Case Else
                Return summary.Count
        End Select
    End Function

    Private Sub GridViewCrossReport_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewCrossReport.RowDataBound
        Dim dt As DataTable = TryCast(Session("CrossReportComparisonTable"), DataTable)
        If dt Is Nothing OrElse e.Row.Cells.Count = 0 Then Exit Sub
        HideColumn(dt, e.Row, "BaseFilterId")
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Base Records", "BaseFilterId", "crossreportfilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("CrossReportComparisonTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        GridViewCrossReport.AllowPaging = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        GridViewCrossReport.PageSize = AnalysisGridPageSize
        If Not GridViewCrossReport.AllowPaging Then GridViewCrossReport.PageIndex = 0
        GridViewCrossReport.DataSource = dt
        GridViewCrossReport.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Cross-Report Comparison (" & If(dt Is Nothing, 0, dt.Rows.Count).ToString() & " rows)"
        AnalysisExportSnapshot.Save(Me, "CrossReportComparison", "Cross-Report Comparison", LabelInfo, GridViewCrossReport, dt)
    End Sub

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("CrossReportComparisonTable"), DataTable)
        If dt Is Nothing Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("CrossReportComparisonTable"), DataTable)
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "CrossReportComparison_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Cross-Report Comparison", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Cross-Report Comparison", ""))
        End If
        Response.End()
    End Sub

    Private Function RegisterAnalysisFilter(filterExpression As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("CrossReportComparisonFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("CrossReportComparisonFilters") = filters
        Return filterId
    End Function

    Private Sub SaveSelections()
        Dim key As String = "CrossReportComparison_" & ReportKey()
        Session(key & "_Compare") = DropDownCompareReport.SelectedValue
        Session(key & "_Key") = DropDownKeyField.SelectedValue
        Session(key & "_Value") = DropDownValueField.SelectedValue
        Session(key & "_Aggregation") = DropDownAggregation.SelectedValue
        Session(key & "_Search") = txtSearch.Text
    End Sub

    Private Sub RestoreSelections()
        Dim key As String = "CrossReportComparison_" & ReportKey()
        If Session(key & "_Compare") IsNot Nothing AndAlso DropDownCompareReport.Items.FindByValue(Session(key & "_Compare").ToString()) IsNot Nothing Then DropDownCompareReport.SelectedValue = Session(key & "_Compare").ToString()
        If Session(key & "_Key") IsNot Nothing AndAlso DropDownKeyField.Items.FindByValue(Session(key & "_Key").ToString()) IsNot Nothing Then DropDownKeyField.SelectedValue = Session(key & "_Key").ToString()
        If Session(key & "_Value") IsNot Nothing AndAlso DropDownValueField.Items.FindByValue(Session(key & "_Value").ToString()) IsNot Nothing Then DropDownValueField.SelectedValue = Session(key & "_Value").ToString()
        If Session(key & "_Aggregation") IsNot Nothing AndAlso DropDownAggregation.Items.FindByValue(Session(key & "_Aggregation").ToString()) IsNot Nothing Then DropDownAggregation.SelectedValue = Session(key & "_Aggregation").ToString()
        If Session(key & "_Search") IsNot Nothing Then txtSearch.Text = Session(key & "_Search").ToString()
    End Sub

    Private Function ReportKey() As String
        If Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then Return Session("REPORTID").ToString().Trim()
        Return "CurrentData"
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Compare Report lists report titles available from the List of Reports page and uses the selected report ID internally.", "Key Field aligns rows from both reports by a shared category, customer, product, period, location, or ID.", "Value Field selects the numeric measure to summarize; choose records when only row counts should be compared.", "Aggregation controls Count, Sum, Average, Min, or Max before differences are calculated.", "Search filters the base report before comparison.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "The page loads the active report and the compare report into memory.", "Both reports are summarized by the selected key field using the same aggregation.", "Keys from both reports are merged so missing base or missing compare keys are visible.", "Difference and Difference % are calculated from compare value minus base value.", "Base Records links open current-report rows behind the base side of the comparison.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Key shows the matching category or All Records when no key is selected.", "Base Records and Compare Records show row counts from each report.", "Base Value and Compare Value show the summarized measure.", "Difference and Difference % show movement between reports.", "Status identifies Changed, Same, Only in base, or Only in compare rows.")
        ReadinessFooterGuidance.SetFooter(Me, "Cross-Report Comparison", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
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
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewCrossReport.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewCrossReport.PageIndex < (GridViewCrossReport.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewCrossReport.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewCrossReport.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewCrossReport.PageIndex > 0 Then GridViewCrossReport.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewCrossReport.PageIndex < (GridViewCrossReport.PageCount - 1) Then GridViewCrossReport.PageIndex += 1
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
            GridViewCrossReport.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Function GridTableForAI(dt As DataTable) As DataTable
        If dt Is Nothing Then Return Nothing
        Dim aiTable As DataTable = dt.Copy()
        If aiTable.Columns.Contains("BaseFilterId") Then aiTable.Columns.Remove("BaseFilterId")
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

    Private Function NumericValue(valueObject As Object) As Double
        Dim number As Double = 0
        Double.TryParse(FieldText(valueObject), NumberStyles.Any, CultureInfo.InvariantCulture, number)
        If number = 0 Then Double.TryParse(FieldText(valueObject), number)
        Return number
    End Function

    Private Function ColumnTypeIsNumeric(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(Short) OrElse col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(Long) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
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
End Class
