Imports System
Imports System.Collections.Generic
Imports System.Data

Partial Class Variance
    Inherits System.Web.UI.Page

    Private Class AnalysisBucket
        Public Count As Integer
        Public Sum As Double
        Public HasNumeric As Boolean

        Public Sub AddValue(valueObject As Object)
            Count += 1

            Dim valueText As String = String.Empty
            If valueObject IsNot Nothing AndAlso Not IsDBNull(valueObject) Then
                valueText = valueObject.ToString()
            End If

            Dim numericValue As Double
            If Double.TryParse(valueText, numericValue) Then
                Sum += numericValue
                HasNumeric = True
            End If
        End Sub

        Public Function Result(aggregateName As String) As Double
            Select Case aggregateName.Trim().ToUpper()
                Case "COUNT"
                    Return Count
                Case "AVG"
                    If Count = 0 OrElse Not HasNumeric Then Return 0
                    Return Sum / Count
                Case Else
                    If Not HasNumeric Then Return 0
                    Return Sum
            End Select
        End Function
    End Class

    Private Sub Variance_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            lblHeader.Text = Session("REPTITLE").ToString() & " - Variance Analysis"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Variance Analysis"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Variance%20Analysis"
    End Sub

    Private Sub Variance_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            FillFieldLists()
            BuildAndBindAnalysis()
        ElseIf Session("VarianceTable") IsNot Nothing Then
            BindAnalysis(CType(Session("VarianceTable"), DataTable))
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
            Session("VarianceSource") = Nothing
            Return Nothing
        End If

        Session("VarianceSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("VarianceSource") Is Nothing Then
            Return LoadReportData()
        End If
        Return CType(Session("VarianceSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub

        DropDownGroupField.Items.Clear()
        DropDownCompareField.Items.Clear()
        DropDownValueField.Items.Clear()

        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            DropDownGroupField.Items.Add(New ListItem(fld, fld))
            DropDownCompareField.Items.Add(New ListItem(fld, fld))
            DropDownValueField.Items.Add(New ListItem(fld, fld))
        Next

        If dt.Columns.Count > 1 Then
            DropDownCompareField.SelectedIndex = 1
        End If

        SetDefaultValueField(dt)
        FillAggregates()
        FillCompareValues()
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

        Dim dt As DataTable = GetSourceTable()
        Dim isNumericValue As Boolean = False
        If dt IsNot Nothing AndAlso DropDownValueField.SelectedValue.Trim() <> "" AndAlso dt.Columns.Contains(DropDownValueField.SelectedValue) Then
            isNumericValue = ColumnTypeIsNumeric(dt.Columns(DropDownValueField.SelectedValue))
        End If

        If isNumericValue Then
            DropDownAggregate.Items.Insert(0, "Avg")
            DropDownAggregate.Items.Insert(0, "Sum")
        End If

        If selectedAggregate.Trim() <> "" Then
            Try
                DropDownAggregate.SelectedValue = selectedAggregate
            Catch ex As Exception
                DropDownAggregate.SelectedValue = DropDownAggregate.Items(0).Value
            End Try
        Else
            DropDownAggregate.SelectedValue = DropDownAggregate.Items(0).Value
        End If
    End Sub

    Private Sub FillCompareValues()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing OrElse DropDownCompareField.SelectedValue.Trim() = "" Then Exit Sub

        Dim previousBase As String = DropDownBaseValue.SelectedValue
        Dim previousCompare As String = DropDownCompareValue.SelectedValue
        Dim values As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim valueText As String = FieldText(dt.Rows(i)(DropDownCompareField.SelectedValue))
            If Not values.ContainsKey(valueText) Then values.Add(valueText, True)
        Next

        DropDownBaseValue.Items.Clear()
        DropDownCompareValue.Items.Clear()

        For Each valueText As String In values.Keys
            DropDownBaseValue.Items.Add(New ListItem(valueText, valueText))
            DropDownCompareValue.Items.Add(New ListItem(valueText, valueText))
        Next

        If DropDownBaseValue.Items.Count > 0 Then
            Try
                DropDownBaseValue.SelectedValue = previousBase
            Catch ex As Exception
                DropDownBaseValue.SelectedIndex = 0
            End Try
        End If

        If DropDownCompareValue.Items.Count > 1 Then
            Try
                DropDownCompareValue.SelectedValue = previousCompare
            Catch ex As Exception
                DropDownCompareValue.SelectedIndex = 1
            End Try
        ElseIf DropDownCompareValue.Items.Count = 1 Then
            DropDownCompareValue.SelectedIndex = 0
        End If
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then
            Response.Redirect(node.Value)
        End If
    End Sub

    Private Sub DropDownCompareField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownCompareField.SelectedIndexChanged
        FillCompareValues()
        BuildAndBindAnalysis()
    End Sub

    Private Sub DropDownValueField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownValueField.SelectedIndexChanged
        FillAggregates()
    End Sub

    Private Sub DropDownAnalysisType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownAnalysisType.SelectedIndexChanged
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkVarianceAI_Click(sender As Object, e As EventArgs) Handles lnkVarianceAI.Click
        Dim dt As DataTable = TryCast(Session("VarianceTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("VarianceTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No analysis data to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), AnalysisTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this percentage-change, variance, or contribution-to-total analysis. Summarize the largest changes, important drivers, and any unusual values.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        If DropDownGroupField.SelectedValue.Trim() = "" OrElse DropDownValueField.SelectedValue.Trim() = "" Then
            LabelError.Text = "Group and value fields must be selected."
            Exit Sub
        End If

        Dim output As DataTable
        If DropDownAnalysisType.SelectedValue = "Contribution" Then
            output = CreateContributionTable(source)
        Else
            If DropDownCompareField.SelectedValue.Trim() = "" OrElse DropDownBaseValue.SelectedValue.Trim() = "" OrElse DropDownCompareValue.SelectedValue.Trim() = "" Then
                LabelError.Text = "Compare field, base value, and compare value must be selected."
                Exit Sub
            End If

            If DropDownBaseValue.SelectedValue = DropDownCompareValue.SelectedValue Then
                LabelError.Text = "Base and compare values should be different."
                Exit Sub
            End If

            output = CreateVarianceTable(source)
        End If

        Session("VarianceTable") = output
        BindAnalysis(output)
    End Sub

    Private Sub BindAnalysis(dt As DataTable)
        BindAnalysisGrid(dt)

        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = AnalysisTitle() & " (" & dt.Rows.Count.ToString() & " rows)"
        End If
    End Sub

    Private Function CreateVarianceTable(source As DataTable) As DataTable
        Dim baseBuckets As New Dictionary(Of String, AnalysisBucket)(StringComparer.OrdinalIgnoreCase)
        Dim compareBuckets As New Dictionary(Of String, AnalysisBucket)(StringComparer.OrdinalIgnoreCase)
        Dim groups As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For

            Dim groupText As String = FieldText(dr(DropDownGroupField.SelectedValue))
            Dim compareText As String = FieldText(dr(DropDownCompareField.SelectedValue))

            If compareText <> DropDownBaseValue.SelectedValue AndAlso compareText <> DropDownCompareValue.SelectedValue Then
                Continue For
            End If

            If Not groups.ContainsKey(groupText) Then groups.Add(groupText, True)

            If compareText = DropDownBaseValue.SelectedValue Then
                If Not baseBuckets.ContainsKey(groupText) Then baseBuckets.Add(groupText, New AnalysisBucket())
                baseBuckets(groupText).AddValue(dr(DropDownValueField.SelectedValue))
            ElseIf compareText = DropDownCompareValue.SelectedValue Then
                If Not compareBuckets.ContainsKey(groupText) Then compareBuckets.Add(groupText, New AnalysisBucket())
                compareBuckets(groupText).AddValue(dr(DropDownValueField.SelectedValue))
            End If
        Next

        Dim output As New DataTable()
        output.Columns.Add(UniqueOutputColumnName(output, DropDownGroupField.SelectedValue), GetType(String))
        output.Columns.Add(UniqueOutputColumnName(output, "Base (" & DropDownBaseValue.SelectedValue & ")"), GetType(String))
        output.Columns.Add(UniqueOutputColumnName(output, "Compare (" & DropDownCompareValue.SelectedValue & ")"), GetType(String))
        output.Columns.Add(UniqueOutputColumnName(output, "Variance"), GetType(String))
        output.Columns.Add(UniqueOutputColumnName(output, "Percent Change"), GetType(String))

        For Each groupText As String In groups.Keys
            Dim baseValue As Double = 0
            Dim compareValue As Double = 0

            If baseBuckets.ContainsKey(groupText) Then baseValue = baseBuckets(groupText).Result(DropDownAggregate.SelectedValue)
            If compareBuckets.ContainsKey(groupText) Then compareValue = compareBuckets(groupText).Result(DropDownAggregate.SelectedValue)

            AddVarianceRow(output, groupText, baseValue, compareValue)
        Next

        Return output
    End Function

    Private Sub AddVarianceRow(output As DataTable, groupText As String, baseValue As Double, compareValue As Double)
        Dim varianceValue As Double = compareValue - baseValue
        Dim percentText As String = ""

        If baseValue <> 0 Then
            percentText = FormatNumber((varianceValue / baseValue) * 100, 2) & "%"
        End If

        Dim outRow As DataRow = output.NewRow()
        outRow(0) = groupText
        outRow(1) = FormatNumber(baseValue, 2)
        outRow(2) = FormatNumber(compareValue, 2)
        outRow(3) = FormatNumber(varianceValue, 2)
        outRow(4) = percentText
        output.Rows.Add(outRow)
    End Sub

    Private Function CreateContributionTable(source As DataTable) As DataTable
        Dim buckets As New Dictionary(Of String, AnalysisBucket)(StringComparer.OrdinalIgnoreCase)
        Dim groups As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim totalBucket As New AnalysisBucket()

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For

            Dim groupText As String = FieldText(dr(DropDownGroupField.SelectedValue))
            If Not groups.ContainsKey(groupText) Then groups.Add(groupText, True)
            If Not buckets.ContainsKey(groupText) Then buckets.Add(groupText, New AnalysisBucket())

            buckets(groupText).AddValue(dr(DropDownValueField.SelectedValue))
            totalBucket.AddValue(dr(DropDownValueField.SelectedValue))
        Next

        Dim totalValue As Double = totalBucket.Result(DropDownAggregate.SelectedValue)
        Dim output As New DataTable()
        output.Columns.Add(UniqueOutputColumnName(output, DropDownGroupField.SelectedValue), GetType(String))
        output.Columns.Add(UniqueOutputColumnName(output, "Value"), GetType(String))
        output.Columns.Add(UniqueOutputColumnName(output, "Contribution to Total"), GetType(String))

        For Each groupText As String In groups.Keys
            Dim value As Double = buckets(groupText).Result(DropDownAggregate.SelectedValue)
            Dim outRow As DataRow = output.NewRow()
            outRow(0) = groupText
            outRow(1) = FormatNumber(value, 2)
            If totalValue <> 0 Then
                outRow(2) = FormatNumber((value / totalValue) * 100, 2) & "%"
            Else
                outRow(2) = ""
            End If
            output.Rows.Add(outRow)
        Next

        If output.Rows.Count > 0 Then
            Dim totalRow As DataRow = output.NewRow()
            totalRow(0) = "Total"
            totalRow(1) = FormatNumber(totalValue, 2)
            totalRow(2) = "100.00%"
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

    Private Function UniqueOutputColumnName(output As DataTable, requestedName As String) As String
        Dim name As String = requestedName
        If name.Trim() = "" Then name = "Column"

        Dim baseName As String = name
        Dim counter As Integer = 1
        While output.Columns.Contains(name)
            counter += 1
            name = baseName & "_" & counter.ToString()
        End While

        Return name
    End Function

    Private Function AnalysisTitle() As String
        If DropDownAnalysisType.SelectedValue = "Contribution" Then
            Return "Contribution to total: " & DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue & " by " & DropDownGroupField.SelectedValue
        End If

        Return DropDownAnalysisType.SelectedItem.Text & ": " & DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue & " by " & DropDownGroupField.SelectedValue & " comparing " & DropDownBaseValue.SelectedValue & " to " & DropDownCompareValue.SelectedValue
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("VarianceTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("VarianceTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No analysis data to export."
            Exit Sub
        End If

        Dim fileName As String = "VarianceAnalysis_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", AnalysisTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), AnalysisTitle(), ""))
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
            GridViewAnalysis.AllowPaging = False
            GridViewAnalysis.PageIndex = 0
            GridViewAnalysis.DataSource = Nothing
            GridViewAnalysis.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewAnalysis.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewAnalysis.PageSize = AnalysisGridPageSize
        If Not GridViewAnalysis.AllowPaging Then
            GridViewAnalysis.PageIndex = 0
        ElseIf GridViewAnalysis.PageIndex < 0 OrElse GridViewAnalysis.PageIndex >= GridViewAnalysis.PageCount Then
            GridViewAnalysis.PageIndex = 0
        End If

        GridViewAnalysis.DataSource = dt
        GridViewAnalysis.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewAnalysis.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewAnalysis.HeaderRow.Cells.Count Then
            GridViewAnalysis.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewAnalysis.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewAnalysis.PageIndex < (GridViewAnalysis.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewAnalysis.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewAnalysis.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAnalysis.PageIndex > 0 Then GridViewAnalysis.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewAnalysis.PageIndex < (GridViewAnalysis.PageCount - 1) Then GridViewAnalysis.PageIndex += 1
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
            GridViewAnalysis.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Variance, percent-change, and contribution-to-total analysis. Inputs are the selected grouping fields, base value field, comparison value field, and any search restriction on the current report data."
        LabelAlgorithmExplanation.Text = "Algorithm: Records are grouped by the selected dimension, base and comparison values are aggregated separately, variance is calculated as Compare minus Base, percent change is calculated from the base value when possible, and contribution shows the row share of the total comparison amount."
        LabelOutputExplanation.Text = "Output: The grid shows each dimension/group, base value, comparison value, variance, percent change, contribution to total, and record counts. Positive variance means the compare value is higher than the base value; negative variance means it is lower."
    End Sub
End Class
