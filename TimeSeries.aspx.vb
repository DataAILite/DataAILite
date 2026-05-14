Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class TimeSeries
    Inherits System.Web.UI.Page

    Private Sub TimeSeries_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Time Series"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Time%20Series"
    End Sub

    Private Sub TimeSeries_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindSeries()
        ElseIf Session("TimeSeriesTable") IsNot Nothing Then
            BindSeries(CType(Session("TimeSeriesTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = ""
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()
        If repid.Trim() = "" Then
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
            Session("TimeSeriesSource") = Nothing
            Return Nothing
        End If
        Session("TimeSeriesSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("TimeSeriesSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("TimeSeriesSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub
        DropDownDateField.Items.Clear()
        DropDownValueField.Items.Clear()
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            If IsDateField(dt, fld) Then DropDownDateField.Items.Add(New ListItem(fld, fld))
            If ColumnTypeIsNumeric(dt.Columns(i)) Then DropDownValueField.Items.Add(New ListItem(fld, fld))
        Next
    End Sub

    Private Function IsDateField(dt As DataTable, columnName As String) As Boolean
        If dt.Columns(columnName).DataType Is GetType(DateTime) Then Return True
        For i As Integer = 0 To Math.Min(20, dt.Rows.Count) - 1
            Dim txt As String = FieldText(dt.Rows(i)(columnName)).Trim()
            If txt = "" Then Continue For
            Dim dateValue As DateTime
            Return DateTime.TryParse(txt, dateValue)
        Next
        Return False
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub GridViewTimeSeries_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewTimeSeries.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow OrElse e.Row.Cells.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(Session("TimeSeriesTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") Then Exit Sub

        Dim recordsIndex As Integer = dt.Columns.IndexOf("Records")
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < e.Row.Cells.Count Then e.Row.Cells(filterIndex).Visible = False
        If recordsIndex < 0 OrElse filterIndex < 0 OrElse recordsIndex >= e.Row.Cells.Count OrElse filterIndex >= e.Row.Cells.Count Then Exit Sub

        Dim recordsText As String = e.Row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
        Dim filterId As String = e.Row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
        If filterId.Trim() = "" Then Exit Sub

        Dim link As New HyperLink()
        link.Text = recordsText
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&tsfilter=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding records in Data Explorer."
        e.Row.Cells(recordsIndex).Controls.Clear()
        e.Row.Cells(recordsIndex).Controls.Add(link)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindSeries()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindSeries()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportSeries("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportSeries("xls")
    End Sub

    Private Sub lnkTimeSeriesAI_Click(sender As Object, e As EventArgs) Handles lnkTimeSeriesAI.Click
        Dim dt As DataTable = TryCast(Session("TimeSeriesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindSeries()
            dt = TryCast(Session("TimeSeriesTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No time series results to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Time Series", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this time series analysis. Explain moving averages, rolling totals, trend direction, unusual periods, and any date ranges that should be reviewed.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindSeries()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub
        If DropDownDateField.Items.Count = 0 OrElse DropDownValueField.Items.Count = 0 Then
            LabelError.Text = "Date and numeric value fields are required."
            Exit Sub
        End If
        Dim windowSize As Integer = 3
        If Not Integer.TryParse(txtWindow.Text.Trim(), windowSize) OrElse windowSize < 1 Then windowSize = 3

        Session("TimeSeriesFilters") = New Dictionary(Of String, String)()

        Dim totals As New SortedDictionary(Of DateTime, Double)()
        Dim counts As New Dictionary(Of DateTime, Integer)()
        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For
            Dim dateValue As DateTime
            If Not TryGetDate(dr(DropDownDateField.SelectedValue), dateValue) Then Continue For
            Dim keyDate As DateTime = SeriesDate(dateValue)
            If Not totals.ContainsKey(keyDate) Then
                totals.Add(keyDate, 0)
                counts.Add(keyDate, 0)
            End If
            totals(keyDate) += NumericValue(dr(DropDownValueField.SelectedValue))
            counts(keyDate) += 1
        Next

        Dim output As New DataTable()
        output.Columns.Add("Date", GetType(String))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("Value", GetType(String))
        output.Columns.Add("Moving Average", GetType(String))
        output.Columns.Add("Rolling Total", GetType(String))
        output.Columns.Add("FilterId", GetType(String))

        Dim values As New List(Of Double)()
        For Each kvp As KeyValuePair(Of DateTime, Double) In totals
            values.Add(kvp.Value)
            Dim startIndex As Integer = Math.Max(0, values.Count - windowSize)
            Dim rollingTotal As Double = 0
            For i As Integer = startIndex To values.Count - 1
                rollingTotal += values(i)
            Next
            Dim movingAverage As Double = rollingTotal / (values.Count - startIndex)
            Dim row As DataRow = output.NewRow()
            row("Date") = kvp.Key.ToShortDateString()
            row("Records") = counts(kvp.Key)
            row("Value") = FormatNumber(kvp.Value, 2)
            row("Moving Average") = FormatNumber(movingAverage, 2)
            row("Rolling Total") = FormatNumber(rollingTotal, 2)
            row("FilterId") = RegisterSeriesFilter(source.Columns(DropDownDateField.SelectedValue), kvp.Key)
            output.Rows.Add(row)
        Next

        Session("TimeSeriesTable") = output
        BindSeries(output)
    End Sub

    Private Function SeriesDate(dateValue As DateTime) As DateTime
        Select Case DropDownDateRollup.SelectedValue
            Case "Month"
                Return New DateTime(dateValue.Year, dateValue.Month, 1)
            Case "Quarter"
                Dim quarterMonth As Integer = ((dateValue.Month - 1) \ 3) * 3 + 1
                Return New DateTime(dateValue.Year, quarterMonth, 1)
            Case "Year"
                Return New DateTime(dateValue.Year, 1, 1)
            Case Else
                Return dateValue.Date
        End Select
    End Function

    Private Function RegisterSeriesFilter(dateColumn As DataColumn, seriesDate As DateTime) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("TimeSeriesFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("TimeSeriesFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = SeriesFilter(dateColumn, seriesDate)
        Return filterId
    End Function

    Private Function SeriesFilter(dateColumn As DataColumn, seriesDate As DateTime) As String
        Dim nextDate As DateTime = seriesDate.AddDays(1)
        If DropDownDateRollup.SelectedValue = "Month" Then nextDate = seriesDate.AddMonths(1)
        If DropDownDateRollup.SelectedValue = "Quarter" Then nextDate = seriesDate.AddMonths(3)
        If DropDownDateRollup.SelectedValue = "Year" Then nextDate = seriesDate.AddYears(1)
        Return FieldRef(dateColumn) & " >= #" & seriesDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "# AND " & FieldRef(dateColumn) & " < #" & nextDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "#"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Sub BindSeries(dt As DataTable)
        BindAnalysisGrid(dt)
        If GridViewTimeSeries.HeaderRow IsNot Nothing AndAlso dt IsNot Nothing AndAlso dt.Columns.Contains("FilterId") Then
            Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
            If filterIndex >= 0 AndAlso filterIndex < GridViewTimeSeries.HeaderRow.Cells.Count Then GridViewTimeSeries.HeaderRow.Cells(filterIndex).Visible = False
        End If
        If dt Is Nothing Then LabelInfo.Text = "" Else LabelInfo.Text = "Time series (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

    Private Function TryGetDate(valueObject As Object, ByRef dateValue As DateTime) As Boolean
        dateValue = DateTime.MinValue
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return False
        If TypeOf valueObject Is DateTime Then
            dateValue = CType(valueObject, DateTime)
            Return True
        End If
        Return DateTime.TryParse(valueObject.ToString(), dateValue)
    End Function

    Private Function NumericValue(valueObject As Object) As Double
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return 0
        Dim value As Double
        If Double.TryParse(valueObject.ToString(), value) Then Return value
        Return 0
    End Function

    Private Function RowMatchesSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        For i As Integer = 0 To row.Table.Columns.Count - 1
            If FieldText(row(i)).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 Then Return True
        Next
        Return False
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Sub ExportSeries(formatName As String)
        Dim dt As DataTable = TryCast(Session("TimeSeriesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindSeries()
            dt = TryCast(Session("TimeSeriesTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No time series data to export."
            Exit Sub
        End If
        Dim fileName As String = "TimeSeries_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Time Series", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Time Series", ""))
        End If
        Response.End()
    End Sub
    Private Const AnalysisGridPageSize As Integer = 50

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewTimeSeries.AllowPaging = False
            GridViewTimeSeries.PageIndex = 0
            GridViewTimeSeries.DataSource = Nothing
            GridViewTimeSeries.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewTimeSeries.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewTimeSeries.PageSize = AnalysisGridPageSize
        If Not GridViewTimeSeries.AllowPaging Then
            GridViewTimeSeries.PageIndex = 0
        ElseIf GridViewTimeSeries.PageIndex < 0 OrElse GridViewTimeSeries.PageIndex >= GridViewTimeSeries.PageCount Then
            GridViewTimeSeries.PageIndex = 0
        End If

        GridViewTimeSeries.DataSource = dt
        GridViewTimeSeries.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewTimeSeries.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewTimeSeries.HeaderRow.Cells.Count Then
            GridViewTimeSeries.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewTimeSeries.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewTimeSeries.PageIndex < (GridViewTimeSeries.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewTimeSeries.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewTimeSeries.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewTimeSeries.PageIndex > 0 Then GridViewTimeSeries.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewTimeSeries.PageIndex < (GridViewTimeSeries.PageCount - 1) Then GridViewTimeSeries.PageIndex += 1
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
            GridViewTimeSeries.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
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
    Private Function BuildAnalysisQuestion(baseQuestion As String) As String
        SetAnalysisExplanationLabels()
        Dim parts As New List(Of String)()
        parts.Add(baseQuestion)
        If LabelAnalysisSubtitle IsNot Nothing AndAlso LabelAnalysisSubtitle.Text.Trim() <> "" Then parts.Add("Input: " & LabelAnalysisSubtitle.Text.Trim())
        If LabelModelExplanation IsNot Nothing AndAlso LabelModelExplanation.Text.Trim() <> "" Then parts.Add(LabelModelExplanation.Text.Trim())
        If LabelAlgorithmExplanation IsNot Nothing AndAlso LabelAlgorithmExplanation.Text.Trim() <> "" Then parts.Add(LabelAlgorithmExplanation.Text.Trim())
        If LabelOutputExplanation IsNot Nothing AndAlso LabelOutputExplanation.Text.Trim() <> "" Then parts.Add(LabelOutputExplanation.Text.Trim())
        Return String.Join(vbCrLf & vbCrLf, parts.ToArray())
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = "Model: Time-series rolling analysis. Inputs are date field, value field, date aggregation period, number of time periods, calculation type, and optional search text."
        LabelAlgorithmExplanation.Text = "Algorithm: Records are grouped into ordered time periods, period totals are calculated, and the selected rolling window is applied to produce moving averages or rolling totals across consecutive periods."
        LabelOutputExplanation.Text = "Output: The grid shows each period, records, period total, moving average or rolling total, and record links. The Number of time periods controls how many prior periods are included in each rolling calculation."
    End Sub
End Class
