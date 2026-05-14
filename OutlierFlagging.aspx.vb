Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class OutlierFlagging
    Inherits System.Web.UI.Page

    Private Sub OutlierFlagging_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Outlier Flagging"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Outlier%20Flagging"
    End Sub

    Private Sub OutlierFlagging_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindOutliers()
        ElseIf Session("OutlierFlaggingTable") IsNot Nothing Then
            BindOutliers(CType(Session("OutlierFlaggingTable"), DataTable))
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
            Session("OutlierFlaggingSource") = Nothing
            Return Nothing
        End If
        Session("OutlierFlaggingSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("OutlierFlaggingSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("OutlierFlaggingSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub
        DropDownValueField.Items.Clear()
        For i As Integer = 0 To dt.Columns.Count - 1
            If ColumnTypeIsNumeric(dt.Columns(i)) Then DropDownValueField.Items.Add(New ListItem(dt.Columns(i).ColumnName, dt.Columns(i).ColumnName))
        Next
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub GridViewOutliers_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewOutliers.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow OrElse e.Row.Cells.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(Session("OutlierFlaggingTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") Then Exit Sub

        Dim rowIndex As Integer = dt.Columns.IndexOf("Row")
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < e.Row.Cells.Count Then e.Row.Cells(filterIndex).Visible = False
        If rowIndex < 0 OrElse filterIndex < 0 OrElse rowIndex >= e.Row.Cells.Count OrElse filterIndex >= e.Row.Cells.Count Then Exit Sub

        Dim rowText As String = e.Row.Cells(rowIndex).Text.Replace("&nbsp;", "").Trim()
        Dim filterId As String = e.Row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
        If filterId.Trim() = "" Then Exit Sub

        Dim link As New HyperLink()
        link.Text = rowText
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&outlierfilter=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding record in Data Explorer."
        e.Row.Cells(rowIndex).Controls.Clear()
        e.Row.Cells(rowIndex).Controls.Add(link)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindOutliers()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindOutliers()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportOutliers("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportOutliers("xls")
    End Sub

    Private Sub lnkOutlierAI_Click(sender As Object, e As EventArgs) Handles lnkOutlierAI.Click
        Dim dt As DataTable = TryCast(Session("OutlierFlaggingTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindOutliers()
            dt = TryCast(Session("OutlierFlaggingTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No outlier results to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Outlier Flagging", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this outlier flagging analysis. Explain the outlier rules, flagged values, reasons, severity, and which records should be reviewed first.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindOutliers()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub
        If DropDownValueField.Items.Count = 0 Then
            LabelError.Text = "No numeric fields were found."
            Exit Sub
        End If

        Session("OutlierFlaggingFilters") = New Dictionary(Of String, String)()

        Dim values As New List(Of Double)()
        For i As Integer = 0 To source.Rows.Count - 1
            If Not RowMatchesSearch(source.Rows(i), txtSearch.Text.Trim()) Then Continue For
            values.Add(NumericValue(source.Rows(i)(DropDownValueField.SelectedValue)))
        Next
        If values.Count = 0 Then Exit Sub

        Dim average As Double = 0
        For i As Integer = 0 To values.Count - 1
            average += values(i)
        Next
        average = average / values.Count

        Dim variance As Double = 0
        For i As Integer = 0 To values.Count - 1
            variance += Math.Pow(values(i) - average, 2)
        Next
        Dim stdev As Double = Math.Sqrt(variance / values.Count)

        Dim stdThreshold As Double = 2
        Dim percentThreshold As Double = 25
        Dim minValue As Double
        Dim maxValue As Double
        Double.TryParse(txtStdDev.Text.Trim(), stdThreshold)
        Double.TryParse(txtPercent.Text.Trim(), percentThreshold)
        Dim hasMin As Boolean = Double.TryParse(txtMin.Text.Trim(), minValue)
        Dim hasMax As Boolean = Double.TryParse(txtMax.Text.Trim(), maxValue)

        Dim output As New DataTable()
        output.Columns.Add("Row", GetType(Integer))
        output.Columns.Add("Field", GetType(String))
        output.Columns.Add("Value", GetType(String))
        output.Columns.Add("Method", GetType(String))
        output.Columns.Add("Reason", GetType(String))
        output.Columns.Add("Average", GetType(String))
        output.Columns.Add("Std Dev", GetType(String))
        output.Columns.Add("FilterId", GetType(String))

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For
            Dim value As Double = NumericValue(dr(DropDownValueField.SelectedValue))
            Dim reason As String = OutlierReason(value, average, stdev, stdThreshold, percentThreshold, hasMin, minValue, hasMax, maxValue)
            If reason.Trim() = "" Then Continue For
            Dim row As DataRow = output.NewRow()
            row("Row") = i + 1
            row("Field") = DropDownValueField.SelectedValue
            row("Value") = FormatNumber(value, 2)
            row("Method") = DropDownMethod.SelectedItem.Text
            row("Reason") = reason
            row("Average") = FormatNumber(average, 2)
            row("Std Dev") = FormatNumber(stdev, 2)
            row("FilterId") = RegisterOutlierFilter(dr)
            output.Rows.Add(row)
        Next

        Session("OutlierFlaggingTable") = output
        BindOutliers(output)
    End Sub

    Private Function OutlierReason(value As Double, average As Double, stdev As Double, stdThreshold As Double, percentThreshold As Double, hasMin As Boolean, minValue As Double, hasMax As Boolean, maxValue As Double) As String
        If DropDownMethod.SelectedValue = "BusinessRule" Then
            If hasMin AndAlso value < minValue Then Return "Below business minimum " & minValue.ToString()
            If hasMax AndAlso value > maxValue Then Return "Above business maximum " & maxValue.ToString()
            Return ""
        End If
        If DropDownMethod.SelectedValue = "PercentDifference" Then
            If average = 0 Then Return ""
            Dim pct As Double = Math.Abs((value - average) / average * 100)
            If pct >= percentThreshold Then Return FormatNumber(pct, 2) & "% from average"
            Return ""
        End If
        If stdev = 0 Then Return ""
        Dim deviations As Double = Math.Abs(value - average) / stdev
        If deviations >= stdThreshold Then Return FormatNumber(deviations, 2) & " standard deviations from average"
        Return ""
    End Function

    Private Function RegisterOutlierFilter(row As DataRow) As String
        Dim filters As Dictionary(Of String, String) = TryCast(Session("OutlierFlaggingFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("OutlierFlaggingFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = SingleRowFilter(row)
        Return filterId
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
        If ColumnTypeIsNumeric(col) AndAlso Double.TryParse(valueText, numericValue) Then Return FieldRef(col) & " = " & numericValue.ToString(CultureInfo.InvariantCulture)

        If TypeOf valueObject Is DateTime Then Return FieldRef(col) & " = #" & CType(valueObject, DateTime).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "#"

        Return FieldRef(col) & " = '" & valueText.Replace("'", "''") & "'"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Sub BindOutliers(dt As DataTable)
        BindAnalysisGrid(dt)
        If GridViewOutliers.HeaderRow IsNot Nothing AndAlso dt IsNot Nothing AndAlso dt.Columns.Contains("FilterId") Then
            Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
            If filterIndex >= 0 AndAlso filterIndex < GridViewOutliers.HeaderRow.Cells.Count Then GridViewOutliers.HeaderRow.Cells(filterIndex).Visible = False
        End If
        If dt Is Nothing Then LabelInfo.Text = "" Else LabelInfo.Text = "Outlier flags (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

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

    Private Sub ExportOutliers(formatName As String)
        Dim dt As DataTable = TryCast(Session("OutlierFlaggingTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindOutliers()
            dt = TryCast(Session("OutlierFlaggingTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No outlier data to export."
            Exit Sub
        End If
        Dim fileName As String = "OutlierFlagging_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Outlier Flagging", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Outlier Flagging", ""))
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
            GridViewOutliers.AllowPaging = False
            GridViewOutliers.PageIndex = 0
            GridViewOutliers.DataSource = Nothing
            GridViewOutliers.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewOutliers.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewOutliers.PageSize = AnalysisGridPageSize
        If Not GridViewOutliers.AllowPaging Then
            GridViewOutliers.PageIndex = 0
        ElseIf GridViewOutliers.PageIndex < 0 OrElse GridViewOutliers.PageIndex >= GridViewOutliers.PageCount Then
            GridViewOutliers.PageIndex = 0
        End If

        GridViewOutliers.DataSource = dt
        GridViewOutliers.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewOutliers.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewOutliers.HeaderRow.Cells.Count Then
            GridViewOutliers.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewOutliers.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewOutliers.PageIndex < (GridViewOutliers.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewOutliers.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewOutliers.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewOutliers.PageIndex > 0 Then GridViewOutliers.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewOutliers.PageIndex < (GridViewOutliers.PageCount - 1) Then GridViewOutliers.PageIndex += 1
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
            GridViewOutliers.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Outlier detection for numeric values. Inputs are the selected row field, value field, rule type, threshold settings, and search text."
        LabelAlgorithmExplanation.Text = "Algorithm: The page calculates baseline statistics such as average and standard deviation or applies percent/business-rule thresholds, then flags records whose value is outside the selected rule limits."
        LabelOutputExplanation.Text = "Output: The grid shows row value, numeric value, rule, threshold, difference, average, standard deviation, and a reason explaining why the row was flagged. Row links open the flagged records in Data Explorer."
    End Sub
End Class
