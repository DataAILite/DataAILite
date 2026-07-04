Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class AutomatedAnalysisNarratives
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub AutomatedAnalysisNarratives_Init(sender As Object, e As EventArgs) Handles Me.Init
        MenuExpansionHelper.Attach(Me)
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Automated Analysis Narratives"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Automated%20Analysis%20Narratives"
    End Sub

    Private Sub AutomatedAnalysisNarratives_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            RestoreSelections()
            BuildAndBindAnalysis()
        ElseIf Session("AutomatedAnalysisNarrativesTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("AutomatedAnalysisNarrativesTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim dvExisting As DataView = TryCast(Session("dv3"), DataView)
        If dvExisting IsNot Nothing AndAlso dvExisting.Table IsNot Nothing AndAlso dvExisting.Table.Rows.Count > 0 Then
            Session("AutomatedNarrativesSource") = dvExisting.Table
            Return dvExisting.Table
        End If
        Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
        If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
            Session("AutomatedNarrativesSource") = existingTable
            Return existingTable
        End If
        Dim repid As String = If(Session("REPORTID") Is Nothing, "", Session("REPORTID").ToString())
        If repid.Trim() = "" Then
            LabelError.Text = "Report is not selected."
            Return Nothing
        End If
        Dim ret As String = ""
        Try
            Dim dv As DataView = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If ret.Trim() <> "" Then LabelError.Text = ret
            If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                Session("AutomatedNarrativesSource") = dv.Table
                Return dv.Table
            End If
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        End Try
        Return Nothing
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("AutomatedNarrativesSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("AutomatedNarrativesSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        SaveSelections()
        GridViewNarratives.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        DropDownFocus.SelectedIndex = 0
        DropDownDetail.SelectedIndex = 1
        FillFieldLists()
        SaveSelections()
        GridViewNarratives.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkNarrativesAI_Click(sender As Object, e As EventArgs) Handles lnkNarrativesAI.Click
        Dim dt As DataTable = TryCast(Session("AutomatedAnalysisNarrativesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("AutomatedAnalysisNarrativesTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No narrative rows to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Automated Analysis Narratives", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Review these automated analysis narratives. Make the findings clearer, identify priorities, and recommend the next analytics pages.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        DropDownPrimaryField.Items.Clear()
        DropDownValueField.Items.Clear()
        DropDownPrimaryField.Items.Add(New ListItem("(automatic)", ""))
        DropDownValueField.Items.Add(New ListItem("(records)", ""))
        If source Is Nothing Then Exit Sub
        For Each col As DataColumn In source.Columns
            DropDownPrimaryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If ColumnTypeIsNumeric(col) Then DropDownValueField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
        Next
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As DataTable = OutputTable()
        If source Is Nothing Then
            BindAnalysisGrid(output)
            Return
        End If
        Dim rows As List(Of DataRow) = FilteredRows(source)
        Dim filterId As String = RegisterAnalysisFilter("1 = 1")
        AddNarrative(output, "Dataset Summary", "All Fields", "The active dataset has " & rows.Count.ToString() & " filtered records and " & source.Columns.Count.ToString() & " fields.", "Search filter: " & If(txtSearch.Text.Trim() = "", "none", txtSearch.Text.Trim()), "Start with Data Readiness Scanner, then open the highest scored analysis pages.", rows.Count, filterId)
        AddQualityNarratives(source, rows, output, filterId)
        AddFieldNarratives(source, rows, output, filterId)
        AddSelectedNarrative(source, rows, output, filterId)
        output.DefaultView.Sort = "Narrative Section ASC, [Field(s)] ASC"
        BindAnalysisGrid(output.DefaultView.ToTable())
    End Sub

    Private Function OutputTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Narrative Section", GetType(String))
        dt.Columns.Add("Field(s)", GetType(String))
        dt.Columns.Add("Finding", GetType(String))
        dt.Columns.Add("Evidence", GetType(String))
        dt.Columns.Add("Recommended Action", GetType(String))
        dt.Columns.Add("Records", GetType(Integer))
        dt.Columns.Add("FilterId", GetType(String))
        Return dt
    End Function

    Private Sub AddQualityNarratives(source As DataTable, rows As List(Of DataRow), output As DataTable, filterId As String)
        For Each col As DataColumn In source.Columns
            Dim blanks As Integer = 0
            For Each row As DataRow In rows
                If FieldText(row(col)).Trim() = "" Then blanks += 1
            Next
            If blanks > 0 Then AddNarrative(output, "Data Quality", col.ColumnName, "Missing values were found and may affect reports or model inputs.", blanks.ToString() & " blanks out of " & Math.Max(1, rows.Count).ToString() & " records", "Open Data Quality or Rule-Based Alerts to inspect affected records.", blanks, RegisterAnalysisFilter("1 = 1"))
            If output.Rows.Count > 20 AndAlso DropDownDetail.SelectedValue <> "Detailed" Then Exit For
        Next
    End Sub

    Private Sub AddFieldNarratives(source As DataTable, rows As List(Of DataRow), output As DataTable, filterId As String)
        For Each col As DataColumn In source.Columns
            If ColumnTypeIsNumeric(col) Then
                Dim stats As RunningStats = StatsFor(rows, col)
                If stats.Count > 0 Then AddNarrative(output, "Field Behavior", col.ColumnName, "Numeric field ranges from " & Math.Round(stats.Min, 4).ToString() & " to " & Math.Round(stats.Max, 4).ToString() & ".", "Average " & Math.Round(stats.Average(), 4).ToString() & "; standard deviation " & Math.Round(stats.StdDev(), 4).ToString(), "Use Ranking, Regression, Outlier Flagging, or KPI Builder if this measure is business important.", stats.Count, filterId)
            ElseIf LooksLikeDate(source, col) Then
                AddNarrative(output, "Trends and Movement", col.ColumnName, "Date-like field can support time summaries, time series, cohorts, or drift review.", "Detected as date field from available values.", "Open Time Based Summaries, Time Series, or Data Drift Analysis.", rows.Count, filterId)
            Else
                Dim distinctCount As Integer = CountDistinctValues(rows, col)
                If distinctCount > 1 Then AddNarrative(output, "Field Behavior", col.ColumnName, "Category/text field has " & distinctCount.ToString() & " distinct values.", "Can be used for grouping, filtering, ranking, pivots, funnels, or market segments.", "Open Detail Analytics, Ranking, Pivot, Funnel, or Market Segments.", rows.Count, filterId)
            End If
            If output.Rows.Count > 25 AndAlso DropDownDetail.SelectedValue = "Short" Then Exit For
        Next
    End Sub

    Private Sub AddSelectedNarrative(source As DataTable, rows As List(Of DataRow), output As DataTable, filterId As String)
        If DropDownPrimaryField.SelectedValue.Trim() = "" Then Return
        Dim col As DataColumn = source.Columns(DropDownPrimaryField.SelectedValue)
        Dim distinctCount As Integer = CountDistinctValues(rows, col)
        AddNarrative(output, "Selected Focus", col.ColumnName, "Selected primary field is being emphasized in this narrative run.", distinctCount.ToString() & " distinct values found.", "Use this field as a category, segment, comparison, or drill-down field on related pages.", rows.Count, filterId)
    End Sub

    Private Sub AddNarrative(output As DataTable, section As String, fieldsText As String, finding As String, evidence As String, actionText As String, records As Integer, filterId As String)
        If DropDownFocus.SelectedValue <> "Executive Summary" AndAlso section <> DropDownFocus.SelectedValue AndAlso section <> "Selected Focus" Then Return
        Dim r As DataRow = output.NewRow()
        r("Narrative Section") = section
        r("Field(s)") = fieldsText
        r("Finding") = finding
        r("Evidence") = evidence
        r("Recommended Action") = actionText
        r("Records") = records
        r("FilterId") = filterId
        output.Rows.Add(r)
    End Sub

    Private Sub GridViewNarratives_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewNarratives.RowDataBound
        Dim dt As DataTable = TryCast(Session("AutomatedAnalysisNarrativesTable"), DataTable)
        If dt Is Nothing OrElse e.Row.Cells.Count = 0 Then Exit Sub
        HideColumn(dt, e.Row, "FilterId")
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "narrativefilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("AutomatedAnalysisNarrativesTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        GridViewNarratives.AllowPaging = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        GridViewNarratives.PageSize = AnalysisGridPageSize
        If Not GridViewNarratives.AllowPaging Then GridViewNarratives.PageIndex = 0
        GridViewNarratives.DataSource = dt
        GridViewNarratives.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Automated Analysis Narratives (" & If(dt Is Nothing, 0, dt.Rows.Count).ToString() & " findings)"
        AnalysisExportSnapshot.Save(Me, "AutomatedAnalysisNarratives", "Automated Analysis Narratives", LabelInfo, GridViewNarratives, dt)
    End Sub

    Private Function CurrentGrid() As GridView
        Return GridViewNarratives
    End Function

    Private Function FilteredRows(source As DataTable) As List(Of DataRow)
        Dim rows As New List(Of DataRow)()
        For Each row As DataRow In source.Rows
            If ContainsSearch(row, txtSearch.Text) Then rows.Add(row)
        Next
        Return rows
    End Function

    Private Function CountDistinctValues(rows As List(Of DataRow), col As DataColumn) As Integer
        Dim dict As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In rows
            Dim valueText As String = FieldText(row(col)).Trim()
            If valueText <> "" Then dict(valueText) = True
        Next
        Return dict.Count
    End Function

    Private Class RunningStats
        Public Count As Integer
        Public Sum As Double
        Public SumSquares As Double
        Public Min As Double
        Public Max As Double
        Public Sub Add(value As Double)
            If Count = 0 Then Min = value : Max = value
            If value < Min Then Min = value
            If value > Max Then Max = value
            Count += 1
            Sum += value
            SumSquares += value * value
        End Sub
        Public Function Average() As Double
            If Count = 0 Then Return 0
            Return Sum / Count
        End Function
        Public Function StdDev() As Double
            If Count = 0 Then Return 0
            Dim avg As Double = Average()
            Return Math.Sqrt(Math.Max(0, (SumSquares / Count) - (avg * avg)))
        End Function
    End Class

    Private Function StatsFor(rows As List(Of DataRow), col As DataColumn) As RunningStats
        Dim stats As New RunningStats()
        For Each row As DataRow In rows
            stats.Add(NumericValue(row(col)))
        Next
        Return stats
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("AutomatedAnalysisNarrativesTable"), DataTable)
        If dt Is Nothing Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("AutomatedAnalysisNarrativesTable"), DataTable)
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "AutomatedAnalysisNarratives_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Automated Analysis Narratives", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Automated Analysis Narratives", ""))
        End If
        Response.End()
    End Sub

    Private Function RegisterAnalysisFilter(filterExpression As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("AutomatedNarrativeFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("AutomatedNarrativeFilters") = filters
        Return filterId
    End Function

    Private Sub SaveSelections()
        Dim key As String = "AutomatedNarratives_" & ReportKey()
        Session(key & "_Focus") = DropDownFocus.SelectedValue
        Session(key & "_Detail") = DropDownDetail.SelectedValue
        Session(key & "_Primary") = DropDownPrimaryField.SelectedValue
        Session(key & "_Value") = DropDownValueField.SelectedValue
        Session(key & "_Search") = txtSearch.Text
    End Sub

    Private Sub RestoreSelections()
        Dim key As String = "AutomatedNarratives_" & ReportKey()
        If Session(key & "_Focus") IsNot Nothing AndAlso DropDownFocus.Items.FindByValue(Session(key & "_Focus").ToString()) IsNot Nothing Then DropDownFocus.SelectedValue = Session(key & "_Focus").ToString()
        If Session(key & "_Detail") IsNot Nothing AndAlso DropDownDetail.Items.FindByValue(Session(key & "_Detail").ToString()) IsNot Nothing Then DropDownDetail.SelectedValue = Session(key & "_Detail").ToString()
        If Session(key & "_Primary") IsNot Nothing AndAlso DropDownPrimaryField.Items.FindByValue(Session(key & "_Primary").ToString()) IsNot Nothing Then DropDownPrimaryField.SelectedValue = Session(key & "_Primary").ToString()
        If Session(key & "_Value") IsNot Nothing AndAlso DropDownValueField.Items.FindByValue(Session(key & "_Value").ToString()) IsNot Nothing Then DropDownValueField.SelectedValue = Session(key & "_Value").ToString()
        If Session(key & "_Search") IsNot Nothing Then txtSearch.Text = Session(key & "_Search").ToString()
    End Sub

    Private Function ReportKey() As String
        If Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString().Trim() <> "" Then Return Session("REPORTID").ToString().Trim()
        Return "CurrentData"
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Narrative Focus chooses the type of text findings to generate.", "Detail Level controls whether the grid shows compact or more complete evidence.", "Primary Field emphasizes one field in the narrative.", "Value Field supplies a numeric measure for evidence where available.", "Search filters source records before the narrative is built.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "The page uses deterministic narrative rules rather than machine learning.", "It scans row counts, missing values, numeric ranges, date-like fields, category distinct counts, and selected field behavior.", "Each narrative row combines a finding, evidence, and recommended next action.", "The output can be exported, sent to AI for expansion, or included in Export Packages.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Narrative Section groups findings by topic.", "Field(s) identifies the source field or fields behind the statement.", "Finding and Evidence explain what was observed.", "Recommended Action points to the best next analytical page.", "Records links open the data behind the narrative row when a filter is available.")
        ReadinessFooterGuidance.SetFooter(Me, "Automated Analysis Narratives", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
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
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewNarratives.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewNarratives.PageIndex < (GridViewNarratives.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewNarratives.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewNarratives.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewNarratives.PageIndex > 0 Then GridViewNarratives.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewNarratives.PageIndex < (GridViewNarratives.PageCount - 1) Then GridViewNarratives.PageIndex += 1
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
            GridViewNarratives.PageIndex = requestedPage - 1
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

    Private Function ContainsSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        Dim needle As String = searchText.ToLowerInvariant()
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).ToLowerInvariant().Contains(needle) Then Return True
        Next
        Return False
    End Function
End Class
