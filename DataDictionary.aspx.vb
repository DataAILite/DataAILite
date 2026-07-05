Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class DataDictionary
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub DataDictionary_Init(sender As Object, e As EventArgs) Handles Me.Init
        MenuExpansionHelper.Attach(Me)
        AnalyticsDashboardTileHelper.Attach(Me)
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Data Dictionary"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Data%20Dictionary"
    End Sub

    Private Sub DataDictionary_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            ApplyUrlParameters()
            BuildAndBindAnalysis()
        ElseIf Session("DataDictionaryTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("DataDictionaryTable"), DataTable))
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
                Session("DataDictionaryTableSource") = existingTable
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
            Session("DataDictionaryTableSource") = Nothing
            Return Nothing
        End If
        Session("DataDictionaryTableSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("DataDictionaryTableSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("DataDictionaryTableSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewDataDictionary.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        GridViewDataDictionary.PageIndex = 0
        FillFieldLists()
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkDataDictionaryAI_Click(sender As Object, e As EventArgs) Handles lnkDataDictionaryAI.Click
        Dim dt As DataTable = TryCast(Session("DataDictionaryTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("DataDictionaryTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data dictionary results to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Data Dictionary", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this data dictionary grid. Explain the strongest findings, unusual records, and business meaning of the results.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub FillFieldLists()
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As New DataTable()
        output.Columns.Add("Field", GetType(String))
        output.Columns.Add("Detected Type", GetType(String))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("Blanks", GetType(Integer))
        output.Columns.Add("Distinct Values", GetType(Integer))
        output.Columns.Add("Min", GetType(String))
        output.Columns.Add("Max", GetType(String))
        output.Columns.Add("Average", GetType(String))
        output.Columns.Add("Std Dev", GetType(String))
        output.Columns.Add("Examples", GetType(String))
        output.Columns.Add("Recommended Use", GetType(String))
        output.Columns.Add("FilterId", GetType(String))
        If source Is Nothing Then BindAnalysisGrid(output) : Return
        Dim examplesWanted As Integer = 3
        Integer.TryParse(DropDownExamples.SelectedValue, examplesWanted)
        For Each col As DataColumn In source.Columns
            Dim isNum As Boolean = ColumnTypeIsNumeric(col)
            Dim isDate As Boolean = LooksLikeDate(source, col)
            If DropDownFieldGroup.SelectedValue = "Numeric Fields" AndAlso Not isNum Then Continue For
            If DropDownFieldGroup.SelectedValue = "Date Fields" AndAlso Not isDate Then Continue For
            If DropDownFieldGroup.SelectedValue = "Text Fields" AndAlso (isNum OrElse isDate) Then Continue For
            If txtSearch.Text.Trim() <> "" AndAlso Not col.ColumnName.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) Then Continue For
            Dim blanks As Integer = 0
            Dim distinct As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            Dim examples As New List(Of String)()
            Dim sum As Double = 0
            Dim sumSq As Double = 0
            Dim n As Integer = 0
            Dim minText As String = ""
            Dim maxText As String = ""
            Dim minNum As Double = 0
            Dim maxNum As Double = 0
            For Each row As DataRow In source.Rows
                Dim textValue As String = FieldText(row(col)).Trim()
                If textValue = "" Then blanks += 1 Else distinct(textValue) = True
                If textValue <> "" AndAlso examples.Count < examplesWanted AndAlso Not examples.Contains(textValue) Then examples.Add(textValue)
                If isNum AndAlso textValue <> "" Then
                    Dim x As Double = NumericValue(row(col))
                    If n = 0 Then
                        minNum = x
                        maxNum = x
                    Else
                        If x < minNum Then minNum = x
                        If x > maxNum Then maxNum = x
                    End If
                    sum += x
                    sumSq += x * x
                    n += 1
                ElseIf textValue <> "" Then
                    If minText = "" OrElse String.Compare(textValue, minText, True) < 0 Then minText = textValue
                    If maxText = "" OrElse String.Compare(textValue, maxText, True) > 0 Then maxText = textValue
                End If
            Next
            Dim r As DataRow = output.NewRow()
            r("Field") = col.ColumnName
            r("Detected Type") = If(isNum, "Numeric", If(isDate, "Date", "Text"))
            r("Records") = source.Rows.Count
            r("Blanks") = blanks
            r("Distinct Values") = distinct.Count
            If isNum AndAlso n > 0 Then
                Dim avg As Double = sum / n
                Dim variance As Double = Math.Max(0, (sumSq / n) - (avg * avg))
                r("Min") = Math.Round(minNum, 4).ToString()
                r("Max") = Math.Round(maxNum, 4).ToString()
                r("Average") = Math.Round(avg, 4).ToString()
                r("Std Dev") = Math.Round(Math.Sqrt(variance), 4).ToString()
            Else
                r("Min") = minText
                r("Max") = maxText
                r("Average") = ""
                r("Std Dev") = ""
            End If
            r("Examples") = String.Join(", ", examples.ToArray())
            r("Recommended Use") = If(isNum, "Measure, KPI, ranking, ABC/Pareto, chart value, outlier check", If(isDate, "Period comparison, cohort, time summaries, time series", "Category, group, filter, funnel stage, segment"))
            r("FilterId") = RegisterAnalysisFilter("1 = 1")
            output.Rows.Add(r)
        Next
        BindAnalysisGrid(output)
    End Sub

    Private Sub GridViewDataDictionary_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewDataDictionary.RowDataBound
        If e.Row.Cells.Count = 0 Then Exit Sub
        Dim dt As DataTable = TryCast(Session("DataDictionaryTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        HideFilterColumnsInRow(dt, e.Row)
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "dictionaryfilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("DataDictionaryTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewDataDictionary.AllowPaging = False
            GridViewDataDictionary.PageIndex = 0
            GridViewDataDictionary.DataSource = Nothing
            GridViewDataDictionary.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If
        GridViewDataDictionary.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewDataDictionary.PageSize = AnalysisGridPageSize
        If Not GridViewDataDictionary.AllowPaging Then GridViewDataDictionary.PageIndex = 0
        GridViewDataDictionary.DataSource = dt
        GridViewDataDictionary.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Data Dictionary (" & dt.Rows.Count.ToString() & " rows)"
        AnalysisExportSnapshot.Save(Me, "DataDictionary", "Data Dictionary", LabelInfo, GridViewDataDictionary, dt)
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
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewDataDictionary.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewDataDictionary.PageIndex < (GridViewDataDictionary.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewDataDictionary.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewDataDictionary.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewDataDictionary.PageIndex > 0 Then GridViewDataDictionary.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewDataDictionary.PageIndex < (GridViewDataDictionary.PageCount - 1) Then GridViewDataDictionary.PageIndex += 1
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
            GridViewDataDictionary.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("DataDictionaryTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("DataDictionaryTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data dictionary results to export."
            Exit Sub
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "DataDictionary_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Data Dictionary", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Data Dictionary", ""))
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
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Field Group filters the dictionary to all fields, numeric fields, date fields, text fields, or other supported groups.", "Number of Examples controls how many sample values are shown.", "Search filters by field name or related field text.", "The page uses all available current report columns.", "Use this page before deeper analysis when field meaning or data behavior is unclear.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "Data dictionary model documents field meaning and practical analytical use.", "The page scans values to detect type, blanks, distinct values, examples, and numeric summaries.", "It identifies likely measures, categories, IDs, dates, and text fields.", "Recommended use is inferred from field behavior and naming patterns.", "The dictionary supports choosing better fields on other analytics pages.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Field and Detected Type identify each column.", "Records, Blanks, and Distinct Values describe completeness and uniqueness.", "Min, Max, Average, and Std Dev appear where applicable.", "Examples show representative values from the data.", "Recommended Use explains how the field can be used in analytics, charts, filters, grouping, or quality review.")
        ReadinessFooterGuidance.SetFooter(Me, "Data Dictionary", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
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
        Dim filters As Dictionary(Of String, String) = TryCast(Session("DataDictionaryFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("DataDictionaryFilters") = filters
        Return filterId
    End Function

    Private Sub ApplyUrlParameters()
        UrlInputHelper.ApplyDropDown(Me, "DropDownFieldGroup", "fieldgroup")
        UrlInputHelper.ApplyDropDown(Me, "DropDownExamples", "examples")
        UrlInputHelper.ApplyTextBox(Me, "txtSearch", "search")
    End Sub

End Class
