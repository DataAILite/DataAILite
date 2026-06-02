Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class Funnel
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub Funnel_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Funnel Analysis"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Funnel%20Analysis"
    End Sub

    Private Sub Funnel_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindAnalysis()
        ElseIf Session("FunnelTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("FunnelTable"), DataTable))
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
                Session("FunnelTableSource") = existingTable
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
            Session("FunnelTableSource") = Nothing
            Return Nothing
        End If
        Session("FunnelTableSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("FunnelTableSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("FunnelTableSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewFunnel.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        GridViewFunnel.PageIndex = 0
        FillFieldLists()
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkFunnelAI_Click(sender As Object, e As EventArgs) Handles lnkFunnelAI.Click
        Dim dt As DataTable = TryCast(Session("FunnelTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("FunnelTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No funnel analysis results to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Funnel Analysis", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this funnel analysis grid. Explain the strongest findings, unusual records, and business meaning of the results.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing Then Exit Sub
        DropDownPrimaryField.Items.Clear()


        DropDownValueField.Items.Clear()

        For Each col As DataColumn In source.Columns
            DropDownPrimaryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))


            If ColumnTypeIsNumeric(col) Then DropDownValueField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))

        Next
        If DropDownValueField.Items.Count = 0 Then DropDownValueField.Items.Add(New ListItem("(records)", ""))



    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As New DataTable()
        output.Columns.Add("Step", GetType(Integer))
        output.Columns.Add("Stage", GetType(String))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("Value", GetType(Double))
        output.Columns.Add("Drop Off", GetType(Integer))
        output.Columns.Add("Conversion %", GetType(Double))
        output.Columns.Add("FilterId", GetType(String))
        If source Is Nothing OrElse DropDownPrimaryField.SelectedValue = "" Then BindAnalysisGrid(output) : Return
        Dim stageCol As String = DropDownPrimaryField.SelectedValue
        Dim valueCol As String = DropDownValueField.SelectedValue
        Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim values As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim stage As String = FieldText(row(stageCol)).Trim()
            If stage = "" Then stage = "(blank)"
            If Not counts.ContainsKey(stage) Then counts(stage) = 0 : values(stage) = 0
            counts(stage) += 1
            If valueCol <> "" Then values(stage) += NumericValue(row(valueCol))
        Next
        Dim stages As New List(Of String)()
        If txtStageOrder.Text.Trim() <> "" Then
            For Each part As String In txtStageOrder.Text.Split(","c)
                If part.Trim() <> "" AndAlso counts.ContainsKey(part.Trim()) Then stages.Add(part.Trim())
            Next
        End If
        For Each k As String In counts.Keys
            If Not stages.Contains(k) Then stages.Add(k)
        Next
        Dim firstCount As Integer = If(stages.Count = 0, 0, counts(stages(0)))
        Dim prevCount As Integer = firstCount
        Dim stepNo As Integer = 1
        For Each stage As String In stages
            Dim r As DataRow = output.NewRow()
            r("Step") = stepNo
            r("Stage") = stage
            r("Records") = counts(stage)
            r("Value") = Math.Round(values(stage), 4)
            r("Drop Off") = If(stepNo = 1, 0, prevCount - counts(stage))
            r("Conversion %") = If(firstCount = 0, 0, Math.Round(counts(stage) * 100.0 / firstCount, 2))
            r("FilterId") = RegisterAnalysisFilter(FilterEquals(stageCol, stage))
            output.Rows.Add(r)
            prevCount = counts(stage)
            stepNo += 1
        Next
        BindAnalysisGrid(output)
    End Sub

    Private Sub GridViewFunnel_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewFunnel.RowDataBound
        If e.Row.Cells.Count = 0 Then Exit Sub
        Dim dt As DataTable = TryCast(Session("FunnelTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        HideFilterColumnsInRow(dt, e.Row)
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "funnelfilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("FunnelTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewFunnel.AllowPaging = False
            GridViewFunnel.PageIndex = 0
            GridViewFunnel.DataSource = Nothing
            GridViewFunnel.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If
        GridViewFunnel.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewFunnel.PageSize = AnalysisGridPageSize
        If Not GridViewFunnel.AllowPaging Then GridViewFunnel.PageIndex = 0
        GridViewFunnel.DataSource = dt
        GridViewFunnel.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Funnel Analysis (" & dt.Rows.Count.ToString() & " rows)"
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
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewFunnel.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewFunnel.PageIndex < (GridViewFunnel.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewFunnel.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewFunnel.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewFunnel.PageIndex > 0 Then GridViewFunnel.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewFunnel.PageIndex < (GridViewFunnel.PageCount - 1) Then GridViewFunnel.PageIndex += 1
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
            GridViewFunnel.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("FunnelTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("FunnelTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No funnel analysis results to export."
            Exit Sub
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "Funnel_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Funnel Analysis", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Funnel Analysis", ""))
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
        If LabelModelExplanation IsNot Nothing AndAlso LabelModelExplanation.Text.Trim() <> "" Then parts.Add(LabelModelExplanation.Text.Trim())
        If LabelAlgorithmExplanation IsNot Nothing AndAlso LabelAlgorithmExplanation.Text.Trim() <> "" Then parts.Add(LabelAlgorithmExplanation.Text.Trim())
        If LabelOutputExplanation IsNot Nothing AndAlso LabelOutputExplanation.Text.Trim() <> "" Then parts.Add(LabelOutputExplanation.Text.Trim())
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
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Stage field selects the process step, status, outcome, or funnel stage.", "Optional Value field summarizes revenue, amount, quantity, or other value by stage.", "Stage Order lets the user define the intended process sequence.", "Search filters source records before funnel counts are calculated.", "Use fields such as lead status, order status, conversion step, application stage, or workflow state.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "Funnel model measures movement through ordered stages.", "The page counts records and value for each stage.", "If Stage Order is supplied, that order controls the funnel sequence.", "Drop-off is calculated against the previous stage.", "Conversion percent is calculated against the first stage.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Step shows the stage order used for the funnel.", "Stage is the selected status or process value.", "Records and Value summarize activity at that stage.", "Drop-off shows how many records were lost from the prior stage.", "Conversion % shows how much of the first stage remains at each step; record links open the stage records.")
        ReadinessFooterGuidance.SetFooter(Me, "Funnel Analysis", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
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
        Dim filters As Dictionary(Of String, String) = TryCast(Session("FunnelFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("FunnelFilters") = filters
        Return filterId
    End Function

End Class
