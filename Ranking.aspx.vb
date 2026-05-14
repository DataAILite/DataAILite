Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class Ranking
    Inherits System.Web.UI.Page

    Private Const NoGroupValue As String = "(All)"

    Private Class RankBucket
        Public Count As Integer
        Public Sum As Double
        Public SumSquares As Double
        Public Minimum As Double
        Public Maximum As Double
        Public HasNumeric As Boolean
        Public DistinctValues As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        Public Sub AddValue(valueObject As Object)
            Count += 1

            Dim valueText As String = String.Empty
            If valueObject IsNot Nothing AndAlso Not IsDBNull(valueObject) Then
                valueText = valueObject.ToString()
            End If

            If Not DistinctValues.ContainsKey(valueText) Then DistinctValues.Add(valueText, True)

            Dim numericValue As Double
            If Double.TryParse(valueText, numericValue) Then
                Sum += numericValue
                SumSquares += numericValue * numericValue
                If Not HasNumeric Then
                    Minimum = numericValue
                    Maximum = numericValue
                    HasNumeric = True
                Else
                    If numericValue < Minimum Then Minimum = numericValue
                    If numericValue > Maximum Then Maximum = numericValue
                End If
            End If
        End Sub

        Public Function Result(aggregateName As String) As Double
            Select Case aggregateName.Trim().ToUpper()
                Case "COUNT"
                    Return Count
                Case "COUNTDISTINCT"
                    Return DistinctValues.Count
                Case "AVG"
                    If Count = 0 OrElse Not HasNumeric Then Return 0
                    Return Sum / Count
                Case "MIN"
                    If Not HasNumeric Then Return 0
                    Return Minimum
                Case "MAX"
                    If Not HasNumeric Then Return 0
                    Return Maximum
                Case Else
                    If Not HasNumeric Then Return 0
                    Return Sum
            End Select
        End Function
    End Class

    Private Class RankRecord
        Public GroupText As String = ""
        Public RankText As String = ""
        Public Value As Double
        Public AverageDifference As Double
        Public Records As Integer
        Public FilterId As String = ""
    End Class

    Private Sub Ranking_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            lblHeader.Text = Session("REPTITLE").ToString() & " - Ranking Analysis"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Ranking Analysis"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Ranking%20Analysis"
    End Sub

    Private Sub Ranking_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            FillFieldLists()
            BuildAndBindRanking()
        ElseIf Session("RankingTable") IsNot Nothing Then
            BindRanking(CType(Session("RankingTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = String.Empty
        Dim repid As String = String.Empty

        If Not Session("REPORTID") Is Nothing Then repid = Session("REPORTID").ToString()

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
            Session("RankingSource") = Nothing
            Return Nothing
        End If

        Session("RankingSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("RankingSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("RankingSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub

        DropDownRankField.Items.Clear()
        DropDownGroupField.Items.Clear()
        DropDownValueField.Items.Clear()

        DropDownGroupField.Items.Add(New ListItem(NoGroupValue, ""))

        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            DropDownRankField.Items.Add(New ListItem(fld, fld))
            DropDownGroupField.Items.Add(New ListItem(fld, fld))
            DropDownValueField.Items.Add(New ListItem(fld, fld))
        Next

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
        If DropDownAggregate.Items.Count > 0 Then selectedAggregate = DropDownAggregate.SelectedValue

        DropDownAggregate.Items.Clear()
        DropDownAggregate.Items.Add("Count")
        DropDownAggregate.Items.Add("CountDistinct")

        Dim dt As DataTable = GetSourceTable()
        Dim isNumericValue As Boolean = False
        If dt IsNot Nothing AndAlso DropDownValueField.SelectedValue.Trim() <> "" AndAlso dt.Columns.Contains(DropDownValueField.SelectedValue) Then
            isNumericValue = ColumnTypeIsNumeric(dt.Columns(DropDownValueField.SelectedValue))
        End If

        If isNumericValue Then
            DropDownAggregate.Items.Insert(0, "Max")
            DropDownAggregate.Items.Insert(0, "Min")
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
        BuildAndBindRanking()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        txtTopN.Text = "10"
        BuildAndBindRanking()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportRanking("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportRanking("xls")
    End Sub

    Private Sub lnkRankingAI_Click(sender As Object, e As EventArgs) Handles lnkRankingAI.Click
        Dim dt As DataTable = TryCast(Session("RankingTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRanking()
            dt = TryCast(Session("RankingTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No ranking data to send to AI."
            Exit Sub
        End If

        Dim publicTable As DataTable = PublicRankingTable(dt)
        Session("dataTable") = publicTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(publicTable, Chr(9), RankingTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this ranking and top/bottom analysis. Summarize the strongest and weakest categories, important group differences, and any records that should be reviewed.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindRanking()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        If DropDownRankField.SelectedValue.Trim() = "" OrElse DropDownValueField.SelectedValue.Trim() = "" Then
            LabelError.Text = "Rank field and value field must be selected."
            Exit Sub
        End If

        Dim topN As Integer = 10
        If Not Integer.TryParse(txtTopN.Text.Trim(), topN) OrElse topN <= 0 Then
            topN = 10
            txtTopN.Text = "10"
        End If

        Dim ranking As DataTable = CreateRankingTable(source, topN)
        Session("RankingTable") = ranking
        BindRanking(ranking)
    End Sub

    Private Sub BindRanking(dt As DataTable)
        BindAnalysisGrid(dt)

        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = RankingTitle() & " (" & dt.Rows.Count.ToString() & " rows)"
        End If
    End Sub

    Private Sub GridViewRanking_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewRanking.RowDataBound
        Dim rankingTable As DataTable = TryCast(Session("RankingTable"), DataTable)
        If rankingTable Is Nothing Then Exit Sub

        Dim recordsIndex As Integer = rankingTable.Columns.IndexOf("Records")
        Dim filterIndex As Integer = rankingTable.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < e.Row.Cells.Count Then e.Row.Cells(filterIndex).Visible = False
        If recordsIndex < 0 OrElse filterIndex < 0 OrElse recordsIndex >= e.Row.Cells.Count OrElse filterIndex >= e.Row.Cells.Count Then Exit Sub

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim recordsText As String = e.Row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
            Dim filterId As String = e.Row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()

            If filterId <> "" Then
                Dim link As New HyperLink()
                link.Text = recordsText
                link.NavigateUrl = "~/ShowReport.aspx?srd=0&rankingfilter=" & Server.UrlEncode(filterId)
                link.CssClass = "NodeStyle"
                e.Row.Cells(recordsIndex).Controls.Clear()
                e.Row.Cells(recordsIndex).Controls.Add(link)
            End If
        End If
    End Sub

    Private Function CreateRankingTable(source As DataTable, topN As Integer) As DataTable
        Session("RankingFilters") = New Dictionary(Of String, String)()

        Dim buckets As New Dictionary(Of String, RankBucket)(StringComparer.OrdinalIgnoreCase)
        Dim groups As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Dim sep As String = ChrW(30)

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For

            Dim groupText As String = NoGroupValue
            If DropDownGroupField.SelectedValue.Trim() <> "" Then groupText = FieldText(dr(DropDownGroupField.SelectedValue))
            Dim rankText As String = FieldText(dr(DropDownRankField.SelectedValue))
            Dim key As String = groupText & sep & rankText

            If Not groups.ContainsKey(groupText) Then groups.Add(groupText, True)
            If Not buckets.ContainsKey(key) Then buckets.Add(key, New RankBucket())
            buckets(key).AddValue(dr(DropDownValueField.SelectedValue))
        Next

        Dim groupRecords As New Dictionary(Of String, List(Of RankRecord))(StringComparer.OrdinalIgnoreCase)
        For Each kvp As KeyValuePair(Of String, RankBucket) In buckets
            Dim parts As String() = kvp.Key.Split(New String() {sep}, StringSplitOptions.None)
            Dim rec As New RankRecord()
            rec.GroupText = parts(0)
            If parts.Length > 1 Then rec.RankText = parts(1)
            rec.Value = kvp.Value.Result(DropDownAggregate.SelectedValue)
            rec.Records = kvp.Value.Count

            If Not groupRecords.ContainsKey(rec.GroupText) Then groupRecords.Add(rec.GroupText, New List(Of RankRecord)())
            groupRecords(rec.GroupText).Add(rec)
        Next

        For Each groupText As String In groupRecords.Keys
            Dim records As List(Of RankRecord) = groupRecords(groupText)
            Dim totalValue As Double = 0
            For i As Integer = 0 To records.Count - 1
                totalValue += records(i).Value
            Next

            Dim groupAverage As Double = 0
            If records.Count > 0 Then groupAverage = totalValue / records.Count

            For i As Integer = 0 To records.Count - 1
                records(i).AverageDifference = Math.Abs(records(i).Value - groupAverage)
                records(i).FilterId = RegisterRankingFilter(source, records(i))
            Next
        Next

        Dim output As New DataTable()
        Dim valueColumnName As String = RankValueColumnName()
        Dim groupValueColumnName As String = RankGroupValueColumnName()
        Dim hasGroupField As Boolean = DropDownGroupField.SelectedValue.Trim() <> ""
        output.Columns.Add("Group", GetType(String))
        If hasGroupField Then output.Columns.Add(groupValueColumnName, GetType(String))
        output.Columns.Add("Rank", GetType(Integer))
        output.Columns.Add("Item", GetType(String))
        output.Columns.Add(valueColumnName, GetType(String))
        output.Columns.Add("Records", GetType(String))
        output.Columns.Add("FilterId", GetType(String))

        For Each groupText As String In groups.Keys
            If Not groupRecords.ContainsKey(groupText) Then Continue For

            Dim records As List(Of RankRecord) = groupRecords(groupText)
            Dim groupValue As Double = CalculateGroupValue(records)
            records.Sort(AddressOf CompareRankRecords)

            Dim rowLimit As Integer = Math.Min(topN, records.Count)
            For i As Integer = 0 To rowLimit - 1
                Dim outRow As DataRow = output.NewRow()
                If DropDownGroupField.SelectedValue.Trim() = "" Then
                    outRow("Group") = ""
                Else
                    outRow("Group") = groupText
                End If
                If hasGroupField Then outRow(groupValueColumnName) = FormatNumber(groupValue, 2)
                outRow("Rank") = i + 1
                outRow("Item") = records(i).RankText
                outRow(valueColumnName) = FormatNumber(records(i).Value, 2)
                outRow("Records") = records(i).Records.ToString()
                outRow("FilterId") = records(i).FilterId
                output.Rows.Add(outRow)
            Next
        Next

        Return output
    End Function

    Private Function CalculateGroupValue(records As List(Of RankRecord)) As Double
        If records Is Nothing OrElse records.Count = 0 Then Return 0

        Dim value As Double = records(0).Value
        Dim total As Double = 0

        For i As Integer = 0 To records.Count - 1
            total += records(i).Value
            Select Case DropDownRankType.SelectedValue
                Case "Bottom"
                    If records(i).Value < value Then value = records(i).Value
                Case "Average"
                    value = total
                Case Else
                    If records(i).Value > value Then value = records(i).Value
            End Select
        Next

        If DropDownRankType.SelectedValue = "Average" Then Return total / records.Count
        Return value
    End Function

    Private Function CompareRankRecords(x As RankRecord, y As RankRecord) As Integer
        Dim ret As Integer
        Select Case DropDownRankType.SelectedValue
            Case "Bottom"
                ret = x.Value.CompareTo(y.Value)
            Case "Average"
                ret = x.AverageDifference.CompareTo(y.AverageDifference)
            Case Else
                ret = y.Value.CompareTo(x.Value)
        End Select

        If ret = 0 Then ret = String.Compare(x.RankText, y.RankText, StringComparison.OrdinalIgnoreCase)
        Return ret
    End Function

    Private Function RegisterRankingFilter(source As DataTable, rec As RankRecord) As String
        If source Is Nothing OrElse Not source.Columns.Contains(DropDownRankField.SelectedValue) Then Return ""

        Dim rowFilter As String = ValueFilter(source.Columns(DropDownRankField.SelectedValue), rec.RankText)
        If DropDownGroupField.SelectedValue.Trim() <> "" AndAlso source.Columns.Contains(DropDownGroupField.SelectedValue) Then
            rowFilter = "(" & rowFilter & ") AND (" & ValueFilter(source.Columns(DropDownGroupField.SelectedValue), rec.GroupText) & ")"
        End If

        Dim filters As Dictionary(Of String, String) = TryCast(Session("RankingFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("RankingFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = rowFilter
        Return filterId
    End Function

    Private Function ValueFilter(col As DataColumn, valueText As String) As String
        If valueText Is Nothing Then valueText = ""
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

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Function EscapeFilterValue(valueText As String) As String
        Return valueText.Replace("'", "''")
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

    Private Function RankingTitle() As String
        Dim rankText As String = DropDownRankType.SelectedValue
        If DropDownRankType.SelectedValue = "Average" Then rankText = "Average-nearest"

        Dim title As String = rankText & " " & txtTopN.Text.Trim() & " ranking: " & DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue & " by " & DropDownRankField.SelectedValue
        If DropDownGroupField.SelectedValue.Trim() <> "" Then title &= " within " & DropDownGroupField.SelectedValue
        Return title
    End Function

    Private Function RankValueColumnName() As String
        Select Case DropDownRankType.SelectedValue
            Case "Bottom"
                Return "Bottom Value"
            Case "Average"
                Return "Average Value"
            Case Else
                Return "Top Value"
        End Select
    End Function

    Private Function RankGroupValueColumnName() As String
        Select Case DropDownRankType.SelectedValue
            Case "Bottom"
                Return "Group Bottom Value"
            Case "Average"
                Return "Group Average Value"
            Case Else
                Return "Group Top Value"
        End Select
    End Function

    Private Function PublicRankingTable(dt As DataTable) As DataTable
        If dt Is Nothing Then Return Nothing
        Dim publicTable As DataTable = dt.Copy()
        If publicTable.Columns.Contains("FilterId") Then publicTable.Columns.Remove("FilterId")
        Return publicTable
    End Function

    Private Sub ExportRanking(formatName As String)
        Dim dt As DataTable = TryCast(Session("RankingTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRanking()
            dt = TryCast(Session("RankingTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No ranking data to export."
            Exit Sub
        End If

        Dim publicTable As DataTable = PublicRankingTable(dt)
        Dim fileName As String = "Ranking_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", RankingTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), RankingTitle(), ""))
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
            GridViewRanking.AllowPaging = False
            GridViewRanking.PageIndex = 0
            GridViewRanking.DataSource = Nothing
            GridViewRanking.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewRanking.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewRanking.PageSize = AnalysisGridPageSize
        If Not GridViewRanking.AllowPaging Then
            GridViewRanking.PageIndex = 0
        ElseIf GridViewRanking.PageIndex < 0 OrElse GridViewRanking.PageIndex >= GridViewRanking.PageCount Then
            GridViewRanking.PageIndex = 0
        End If

        GridViewRanking.DataSource = dt
        GridViewRanking.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewRanking.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewRanking.HeaderRow.Cells.Count Then
            GridViewRanking.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewRanking.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewRanking.PageIndex < (GridViewRanking.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewRanking.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewRanking.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewRanking.PageIndex > 0 Then GridViewRanking.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewRanking.PageIndex < (GridViewRanking.PageCount - 1) Then GridViewRanking.PageIndex += 1
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
            GridViewRanking.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

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
        LabelModelExplanation.Text = "Model: Ranking and top/bottom/average analysis for categories or other dimensions. Inputs are the selected rank field, optional group field, value field, rank type, top count, and search text."
        LabelAlgorithmExplanation.Text = "Algorithm: Records are grouped by the rank field and optional group, the selected value field is aggregated, then rows are sorted for Top, Bottom, or Average ranking. Group value columns show the selected rank result inside each group when a group field is used."
        LabelOutputExplanation.Text = "Output: The grid shows the ranked dimension, optional group, rank type, calculated top/bottom/average value, group value when applicable, and record count. Record links open the rows used to calculate each ranked result."
    End Sub
End Class
