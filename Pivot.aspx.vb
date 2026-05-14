Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text

Partial Class Pivot
    Inherits System.Web.UI.Page

    Private Shared ReadOnly DistinctSeparator As String = ChrW(30)

    Private Class PivotBucket
        Public Count As Integer
        Public Sum As Double
        Public SumSquares As Double
        Public Minimum As Double
        Public Maximum As Double
        Public HasNumeric As Boolean
        Public LastValue As String = String.Empty
        Public DistinctValues As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        Public Sub AddValue(valueObject As Object)
            Count += 1

            Dim valueText As String = String.Empty
            If valueObject IsNot Nothing AndAlso Not IsDBNull(valueObject) Then
                valueText = valueObject.ToString()
            End If

            LastValue = valueText
            If Not DistinctValues.ContainsKey(valueText) Then
                DistinctValues.Add(valueText, True)
            End If

            Dim numericValue As Double
            If Double.TryParse(valueText, numericValue) Then
                If Not HasNumeric Then
                    Minimum = numericValue
                    Maximum = numericValue
                    HasNumeric = True
                Else
                    If numericValue < Minimum Then Minimum = numericValue
                    If numericValue > Maximum Then Maximum = numericValue
                End If
                Sum += numericValue
                SumSquares += numericValue * numericValue
            End If
        End Sub

        Public Function Result(aggregateName As String) As String
            Select Case aggregateName.Trim().ToUpper()
                Case "COUNT"
                    Return Count.ToString()
                Case "COUNTDISTINCT"
                    Return DistinctValues.Count.ToString()
                Case "SUM"
                    If Not HasNumeric Then Return "0"
                    Return FormatNumber(Sum, 2)
                Case "MAX"
                    If Not HasNumeric Then Return String.Empty
                    Return FormatNumber(Maximum, 2)
                Case "MIN"
                    If Not HasNumeric Then Return String.Empty
                    Return FormatNumber(Minimum, 2)
                Case "AVG"
                    If Count = 0 OrElse Not HasNumeric Then Return String.Empty
                    Return FormatNumber(Sum / Count, 2)
                Case "STDEV"
                    If Count <= 1 OrElse Not HasNumeric Then Return "0"
                    Dim variance As Double = (SumSquares - ((Sum * Sum) / Count)) / (Count - 1)
                    If variance < 0 Then variance = 0
                    Return FormatNumber(Math.Sqrt(variance), 2)
                Case "VALUE"
                    Return LastValue
                Case Else
                    Return Count.ToString()
            End Select
        End Function
    End Class

    Private Sub Pivot_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL").ToString()
        End If

        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString.Trim() <> "" Then
            Session("REPORTID") = Request("Report").ToString.Trim()
        End If

        If Not Session("REPTITLE") Is Nothing AndAlso Session("REPTITLE").ToString.Trim() <> "" Then
            lblHeader.Text = Session("REPTITLE").ToString() & " - Pivot / Cross Tab"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Pivot / Cross Tab"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Pivot%20Cross%20Tab"
    End Sub

    Private Sub Pivot_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            FillFieldLists()
            BuildAndBindPivot()
        ElseIf Session("PivotTable") IsNot Nothing Then
            BindPivot(CType(Session("PivotTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = String.Empty
        Dim repid As String = String.Empty

        If Not Session("REPORTID") Is Nothing Then
            repid = Session("REPORTID").ToString()
        End If

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

        If ret.Trim() <> "" Then
            LabelError.Text = ret
        End If

        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            LabelError.Text = "No data. Run or import report data first."
            Session("PivotSource") = Nothing
            Return Nothing
        End If

        Session("PivotSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("PivotSource") Is Nothing Then
            Return LoadReportData()
        End If
        Return CType(Session("PivotSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub

        DropDownRowField.Items.Clear()
        DropDownColumnField.Items.Clear()
        DropDownValueField.Items.Clear()

        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            DropDownRowField.Items.Add(New ListItem(fld, fld))
            DropDownColumnField.Items.Add(New ListItem(fld, fld))
            DropDownValueField.Items.Add(New ListItem(fld, fld))
        Next

        If dt.Columns.Count > 1 Then
            DropDownColumnField.SelectedIndex = 1
        End If

        SetDefaultValueField(dt)
        FillAggregates()
    End Sub

    Private Sub SetDefaultValueField(dt As DataTable)
        For i As Integer = 0 To dt.Columns.Count - 1
            If ColumnTypeIsNumeric(dt.Columns(i)) Then
                DropDownValueField.SelectedValue = dt.Columns(i).ColumnName
                Exit Sub
            End If
        Next
    End Sub

    Private Sub FillAggregates()
        Dim selectedAggregate As String = String.Empty
        If DropDownAggregate.Items.Count > 0 Then
            selectedAggregate = DropDownAggregate.SelectedValue
        End If
        DropDownAggregate.Items.Clear()
        DropDownAggregate.Items.Add("Count")
        DropDownAggregate.Items.Add("CountDistinct")

        Dim dt As DataTable = GetSourceTable()
        Dim isNumericValue As Boolean = False
        If dt IsNot Nothing AndAlso DropDownValueField.SelectedValue.Trim() <> "" AndAlso dt.Columns.Contains(DropDownValueField.SelectedValue) Then
            isNumericValue = ColumnTypeIsNumeric(dt.Columns(DropDownValueField.SelectedValue))
        End If

        If isNumericValue Then
            DropDownAggregate.Items.Add("Sum")
            DropDownAggregate.Items.Add("Max")
            DropDownAggregate.Items.Add("Min")
            DropDownAggregate.Items.Add("Avg")
            DropDownAggregate.Items.Add("StDev")
            DropDownAggregate.Items.Add("Value")
        End If

        If selectedAggregate.Trim() <> "" Then
            Try
                DropDownAggregate.SelectedValue = selectedAggregate
            Catch ex As Exception
                DropDownAggregate.SelectedValue = "Count"
            End Try
        ElseIf isNumericValue Then
            DropDownAggregate.SelectedValue = "Sum"
        Else
            DropDownAggregate.SelectedValue = "Count"
        End If
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then
            Response.Redirect(node.Value)
        End If
    End Sub

    Private Sub DropDownValueField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownValueField.SelectedIndexChanged
        FillAggregates()
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindPivot()
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        BuildAndBindPivot()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindPivot()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportPivot("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportPivot("xls")
    End Sub

    Private Sub lnkPivotAI_Click(sender As Object, e As EventArgs) Handles lnkPivotAI.Click
        Dim dt As DataTable = TryCast(Session("PivotTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindPivot()
            dt = TryCast(Session("PivotTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No pivot data to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), PivotTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret the pivot-style cross-tab report. Summarize important patterns, high and low values, and any meaningful differences.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindPivot()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        If DropDownRowField.SelectedValue.Trim() = "" OrElse DropDownColumnField.SelectedValue.Trim() = "" OrElse DropDownValueField.SelectedValue.Trim() = "" Then
            LabelError.Text = "Row, column, and value fields must be selected."
            Exit Sub
        End If

        If DropDownRowField.SelectedValue = DropDownColumnField.SelectedValue Then
            LabelError.Text = "Row field and column field should be different."
            Exit Sub
        End If

        Dim pivot As DataTable = CreatePivotTable(source, DropDownRowField.SelectedValue, DropDownColumnField.SelectedValue, DropDownValueField.SelectedValue, DropDownAggregate.SelectedValue, txtSearch.Text.Trim())
        Session("PivotTable") = pivot
        BindPivot(pivot)
    End Sub

    Private Sub BindPivot(dt As DataTable)
        BindAnalysisGrid(dt)
        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = PivotTitle() & " (" & dt.Rows.Count.ToString() & " rows)"
        End If
    End Sub

    Private Function CreatePivotTable(source As DataTable, rowField As String, columnField As String, valueField As String, aggregateName As String, searchText As String) As DataTable
        Dim buckets As New Dictionary(Of String, PivotBucket)()
        Dim rowValues As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim columnValues As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim rowTotals As New Dictionary(Of String, PivotBucket)(StringComparer.OrdinalIgnoreCase)
        Dim columnTotals As New Dictionary(Of String, PivotBucket)(StringComparer.OrdinalIgnoreCase)
        Dim grandTotal As New PivotBucket()

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, searchText) Then Continue For

            Dim rowText As String = FieldText(dr(rowField))
            Dim colText As String = FieldText(dr(columnField))
            Dim bucketKey As String = rowText & DistinctSeparator & colText

            If Not rowValues.ContainsKey(rowText) Then rowValues.Add(rowText, True)
            If Not columnValues.ContainsKey(colText) Then columnValues.Add(colText, True)
            If Not buckets.ContainsKey(bucketKey) Then buckets.Add(bucketKey, New PivotBucket())
            If Not rowTotals.ContainsKey(rowText) Then rowTotals.Add(rowText, New PivotBucket())
            If Not columnTotals.ContainsKey(colText) Then columnTotals.Add(colText, New PivotBucket())

            buckets(bucketKey).AddValue(dr(valueField))
            rowTotals(rowText).AddValue(dr(valueField))
            columnTotals(colText).AddValue(dr(valueField))
            grandTotal.AddValue(dr(valueField))
        Next

        Dim output As New DataTable()
        output.Columns.Add(rowField, GetType(String))
        Dim outputColumnNames As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        For Each colValue As String In columnValues.Keys
            Dim outputName As String = UniqueColumnName(SafeColumnName(colValue), outputColumnNames)
            outputColumnNames.Add(colValue, outputName)
            output.Columns.Add(outputName, GetType(String))
        Next
        output.Columns.Add("Total", GetType(String))

        For Each rowValue As String In rowValues.Keys
            Dim outRow As DataRow = output.NewRow()
            outRow(rowField) = rowValue
            For Each colValue As String In columnValues.Keys
                Dim bucketKey As String = rowValue & DistinctSeparator & colValue
                If buckets.ContainsKey(bucketKey) Then
                    outRow(outputColumnNames(colValue)) = buckets(bucketKey).Result(aggregateName)
                Else
                    outRow(outputColumnNames(colValue)) = ""
                End If
            Next
            If rowTotals.ContainsKey(rowValue) Then
                outRow("Total") = rowTotals(rowValue).Result(aggregateName)
            End If
            output.Rows.Add(outRow)
        Next

        If output.Rows.Count > 0 Then
            Dim totalRow As DataRow = output.NewRow()
            totalRow(rowField) = "Total"
            For Each colValue As String In columnValues.Keys
                If columnTotals.ContainsKey(colValue) Then
                    totalRow(outputColumnNames(colValue)) = columnTotals(colValue).Result(aggregateName)
                End If
            Next
            totalRow("Total") = grandTotal.Result(aggregateName)
            output.Rows.Add(totalRow)
        End If

        Return output
    End Function

    Private Function RowMatchesSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        For i As Integer = 0 To row.Table.Columns.Count - 1
            If FieldText(row(i)).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then
            Return CType(valueObject, DateTime).ToShortDateString()
        End If
        Return valueObject.ToString()
    End Function

    Private Function SafeColumnName(valueText As String) As String
        Dim ret As String = valueText
        If ret.Trim() = "" Then ret = "(blank)"
        ret = ret.Replace("[", "(").Replace("]", ")")
        If ret.Length > 80 Then ret = ret.Substring(0, 80)
        Return ret
    End Function

    Private Function UniqueColumnName(baseName As String, existing As Dictionary(Of String, String)) As String
        Dim name As String = baseName
        Dim n As Integer = 1
        If name.Trim() = "" Then name = "Column"
        While existing.ContainsValue(name)
            n += 1
            name = baseName & "_" & n.ToString()
        End While
        Return name
    End Function

    Private Function PivotTitle() As String
        Return "Pivot: " & DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue & " by " & DropDownRowField.SelectedValue & " and " & DropDownColumnField.SelectedValue
    End Function

    Private Sub ExportPivot(formatName As String)
        Dim dt As DataTable = TryCast(Session("PivotTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindPivot()
            dt = TryCast(Session("PivotTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No pivot data to export."
            Exit Sub
        End If

        Dim fileName As String = "Pivot_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", PivotTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), PivotTitle(), ""))
            Response.End()
        End If
    End Sub
    Private Const AnalysisGridPageSize As Integer = 50

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewPivot.AllowPaging = False
            GridViewPivot.PageIndex = 0
            GridViewPivot.DataSource = Nothing
            GridViewPivot.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewPivot.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewPivot.PageSize = AnalysisGridPageSize
        If Not GridViewPivot.AllowPaging Then
            GridViewPivot.PageIndex = 0
        ElseIf GridViewPivot.PageIndex < 0 OrElse GridViewPivot.PageIndex >= GridViewPivot.PageCount Then
            GridViewPivot.PageIndex = 0
        End If

        GridViewPivot.DataSource = dt
        GridViewPivot.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewPivot.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewPivot.HeaderRow.Cells.Count Then
            GridViewPivot.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewPivot.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewPivot.PageIndex < (GridViewPivot.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewPivot.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewPivot.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewPivot.PageIndex > 0 Then GridViewPivot.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewPivot.PageIndex < (GridViewPivot.PageCount - 1) Then GridViewPivot.PageIndex += 1
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
            GridViewPivot.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Pivot cross-tab summary. Inputs are the current report records after the page search text is applied, the selected Row field, Column field, Value field, and Aggregate option."
        LabelAlgorithmExplanation.Text = "Algorithm: Each record is assigned to a row group and column group, then the value field is accumulated by the selected aggregate such as Sum, Count, Average, Minimum, or Maximum. The grid is rebuilt from these grouped buckets so the row and column intersections show comparable totals."
        LabelOutputExplanation.Text = "Output: The first columns identify the row-field values, each generated pivot column represents a column-field value, and the cells show the calculated aggregate for that row/column intersection. Blank or zero cells mean no matching records or no usable numeric value for that intersection."
    End Sub
End Class
