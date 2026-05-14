Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class TimeBasedSummaries
    Inherits System.Web.UI.Page

    Private Class SummaryBucket
        Public Count As Integer
        Public Sum As Double
        Public Min As Double
        Public Max As Double
        Public HasNumeric As Boolean

        Public Sub AddValue(valueObject As Object)
            Count += 1
            Dim numericValue As Double
            Dim valueText As String = ""
            If valueObject IsNot Nothing AndAlso Not IsDBNull(valueObject) Then valueText = valueObject.ToString()
            If Double.TryParse(valueText, numericValue) Then
                Sum += numericValue
                If Not HasNumeric Then
                    Min = numericValue
                    Max = numericValue
                    HasNumeric = True
                Else
                    If numericValue < Min Then Min = numericValue
                    If numericValue > Max Then Max = numericValue
                End If
            End If
        End Sub

        Public Function Result(aggregateName As String) As Double
            Select Case aggregateName.Trim().ToUpperInvariant()
                Case "COUNT"
                    Return Count
                Case "AVG"
                    If Count = 0 OrElse Not HasNumeric Then Return 0
                    Return Sum / Count
                Case "MIN"
                    If Not HasNumeric Then Return 0
                    Return Min
                Case "MAX"
                    If Not HasNumeric Then Return 0
                    Return Max
                Case Else
                    If Not HasNumeric Then Return 0
                    Return Sum
            End Select
        End Function
    End Class

    Private Sub TimeBasedSummaries_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Time Based Summaries"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Time%20Based%20Summaries"
    End Sub

    Private Sub TimeBasedSummaries_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LabelError.Text = ""
            LoadReportData()
            FillFieldLists()
            BuildAndBindSummary()
        ElseIf Session("TimeBasedSummariesTable") IsNot Nothing Then
            BindSummary(CType(Session("TimeBasedSummariesTable"), DataTable))
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
            Session("TimeBasedSummariesSource") = Nothing
            Return Nothing
        End If

        Session("TimeBasedSummariesSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("TimeBasedSummariesSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("TimeBasedSummariesSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub
        DropDownDateField.Items.Clear()
        DropDownValueField.Items.Clear()

        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            If IsDateField(dt, fld) Then DropDownDateField.Items.Add(New ListItem(fld, fld))
            DropDownValueField.Items.Add(New ListItem(fld, fld))
        Next

        For i As Integer = 0 To dt.Columns.Count - 1
            If ColumnTypeIsNumeric(dt.Columns(i)) Then
                DropDownValueField.SelectedValue = dt.Columns(i).ColumnName
                Exit For
            End If
        Next
    End Sub

    Private Function IsDateField(dt As DataTable, columnName As String) As Boolean
        If dt.Columns(columnName).DataType Is GetType(DateTime) Then Return True
        Dim checkedValues As Integer = 0
        Dim parsed As Integer = 0
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim txt As String = FieldText(dt.Rows(i)(columnName)).Trim()
            If txt = "" Then Continue For
            checkedValues += 1
            Dim dateValue As DateTime
            If DateTime.TryParse(txt, dateValue) Then parsed += 1
            If checkedValues >= 20 Then Exit For
        Next
        Return checkedValues > 0 AndAlso parsed = checkedValues
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub GridViewSummary_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewSummary.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow OrElse e.Row.Cells.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(Session("TimeBasedSummariesTable"), DataTable)
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
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&tbsfilter=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding records in Data Explorer."
        e.Row.Cells(recordsIndex).Controls.Clear()
        e.Row.Cells(recordsIndex).Controls.Add(link)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindSummary()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindSummary()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportSummary("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportSummary("xls")
    End Sub

    Private Sub lnkTimeBasedSummariesAI_Click(sender As Object, e As EventArgs) Handles lnkTimeBasedSummariesAI.Click
        Dim dt As DataTable = TryCast(Session("TimeBasedSummariesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindSummary()
            dt = TryCast(Session("TimeBasedSummariesTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No time based summary results to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Time Based Summaries", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this time based summary analysis. Explain period totals, important increases or decreases, unusual periods, and any records or date ranges that should be reviewed.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindSummary()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub
        If DropDownDateField.Items.Count = 0 Then
            LabelError.Text = "No date fields were found in the current report data."
            Exit Sub
        End If

        Session("TimeBasedSummaryFilters") = New Dictionary(Of String, String)()

        Dim buckets As New SortedDictionary(Of DateTime, SummaryBucket)()
        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For
            Dim dateValue As DateTime
            If Not TryGetDate(dr(DropDownDateField.SelectedValue), dateValue) Then Continue For
            Dim keyDate As DateTime = PeriodStart(dateValue, DropDownPeriod.SelectedValue)
            If Not buckets.ContainsKey(keyDate) Then buckets.Add(keyDate, New SummaryBucket())
            buckets(keyDate).AddValue(dr(DropDownValueField.SelectedValue))
        Next

        Dim output As New DataTable()
        output.Columns.Add("Period", GetType(String))
        output.Columns.Add("Period Start", GetType(String))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add(DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue, GetType(String))
        output.Columns.Add("FilterId", GetType(String))

        For Each kvp As KeyValuePair(Of DateTime, SummaryBucket) In buckets
            Dim row As DataRow = output.NewRow()
            row("Period") = PeriodLabel(kvp.Key, DropDownPeriod.SelectedValue)
            row("Period Start") = kvp.Key.ToShortDateString()
            row("Records") = kvp.Value.Count
            row(3) = FormatNumber(kvp.Value.Result(DropDownAggregate.SelectedValue), 2)
            row("FilterId") = RegisterPeriodFilter(source.Columns(DropDownDateField.SelectedValue), kvp.Key, DropDownPeriod.SelectedValue)
            output.Rows.Add(row)
        Next

        Session("TimeBasedSummariesTable") = output
        BindSummary(output)
    End Sub

    Private Function PeriodStart(dateValue As DateTime, periodName As String) As DateTime
        Select Case periodName.Trim().ToUpperInvariant()
            Case "WEEK"
                Return dateValue.Date.AddDays(-(CInt(dateValue.DayOfWeek)))
            Case "MONTH"
                Return New DateTime(dateValue.Year, dateValue.Month, 1)
            Case "QUARTER"
                Dim quarterMonth As Integer = ((dateValue.Month - 1) \ 3) * 3 + 1
                Return New DateTime(dateValue.Year, quarterMonth, 1)
            Case "YEAR"
                Return New DateTime(dateValue.Year, 1, 1)
            Case Else
                Return dateValue.Date
        End Select
    End Function

    Private Function PeriodLabel(dateValue As DateTime, periodName As String) As String
        Select Case periodName.Trim().ToUpperInvariant()
            Case "WEEK"
                Return dateValue.Year.ToString() & " Week " & CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateValue, CalendarWeekRule.FirstDay, DayOfWeek.Sunday).ToString()
            Case "MONTH"
                Return dateValue.ToString("yyyy-MM")
            Case "QUARTER"
                Return dateValue.Year.ToString() & " Q" & (((dateValue.Month - 1) \ 3) + 1).ToString()
            Case "YEAR"
                Return dateValue.Year.ToString()
            Case Else
                Return dateValue.ToShortDateString()
        End Select
    End Function

    Private Function RegisterPeriodFilter(dateColumn As DataColumn, periodDate As DateTime, periodName As String) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("TimeBasedSummaryFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("TimeBasedSummaryFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = PeriodFilter(dateColumn, periodDate, periodName)
        Return filterId
    End Function

    Private Function PeriodFilter(dateColumn As DataColumn, periodDate As DateTime, periodName As String) As String
        Dim nextDate As DateTime = periodDate.AddDays(1)
        Select Case periodName.Trim().ToUpperInvariant()
            Case "WEEK"
                nextDate = periodDate.AddDays(7)
            Case "MONTH"
                nextDate = periodDate.AddMonths(1)
            Case "QUARTER"
                nextDate = periodDate.AddMonths(3)
            Case "YEAR"
                nextDate = periodDate.AddYears(1)
        End Select

        Return FieldRef(dateColumn) & " >= #" & periodDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "# AND " & FieldRef(dateColumn) & " < #" & nextDate.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "#"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Sub BindSummary(dt As DataTable)
        BindAnalysisGrid(dt)
        If GridViewSummary.HeaderRow IsNot Nothing AndAlso dt IsNot Nothing AndAlso dt.Columns.Contains("FilterId") Then
            Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
            If filterIndex >= 0 AndAlso filterIndex < GridViewSummary.HeaderRow.Cells.Count Then GridViewSummary.HeaderRow.Cells(filterIndex).Visible = False
        End If
        If dt Is Nothing Then LabelInfo.Text = "" Else LabelInfo.Text = "Time based summaries (" & dt.Rows.Count.ToString() & " rows)"
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

    Private Function RowMatchesSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        For i As Integer = 0 To row.Table.Columns.Count - 1
            If FieldText(row(i)).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 Then Return True
        Next
        Return False
    End Function

    Private Shared Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Sub ExportSummary(formatName As String)
        Dim dt As DataTable = TryCast(Session("TimeBasedSummariesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindSummary()
            dt = TryCast(Session("TimeBasedSummariesTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No summary data to export."
            Exit Sub
        End If
        Dim fileName As String = "TimeBasedSummaries_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Time Based Summaries", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Time Based Summaries", ""))
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
            GridViewSummary.AllowPaging = False
            GridViewSummary.PageIndex = 0
            GridViewSummary.DataSource = Nothing
            GridViewSummary.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewSummary.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewSummary.PageSize = AnalysisGridPageSize
        If Not GridViewSummary.AllowPaging Then
            GridViewSummary.PageIndex = 0
        ElseIf GridViewSummary.PageIndex < 0 OrElse GridViewSummary.PageIndex >= GridViewSummary.PageCount Then
            GridViewSummary.PageIndex = 0
        End If

        GridViewSummary.DataSource = dt
        GridViewSummary.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewSummary.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewSummary.HeaderRow.Cells.Count Then
            GridViewSummary.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewSummary.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewSummary.PageIndex < (GridViewSummary.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewSummary.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewSummary.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewSummary.PageIndex > 0 Then GridViewSummary.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewSummary.PageIndex < (GridViewSummary.PageCount - 1) Then GridViewSummary.PageIndex += 1
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
            GridViewSummary.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Period-based summary analysis for reports with date fields. Inputs are the selected date field, aggregation period, value field, aggregate option, and search text."
        LabelAlgorithmExplanation.Text = "Algorithm: Dates are normalized into day, week, month, quarter, or year buckets. Values inside each bucket are aggregated using the selected calculation, and each period receives a filter back to the contributing records."
        LabelOutputExplanation.Text = "Output: The grid shows time period, record count, selected aggregate value, and record links. The links open the records in the selected period so totals can be traced back to the report data."
    End Sub
End Class
