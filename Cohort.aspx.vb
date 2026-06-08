Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Web.UI.WebControls

Partial Class Cohort
    Inherits System.Web.UI.Page

    Private Const AnalysisGridPageSize As Integer = 50

    Private Sub Cohort_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Cohort Analysis"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Cohort%20Analysis"
    End Sub

    Private Sub Cohort_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindAnalysis()
        ElseIf Session("CohortTable") IsNot Nothing Then
            BindAnalysisGrid(CType(Session("CohortTable"), DataTable))
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
                Session("CohortTableSource") = existingTable
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
            Session("CohortTableSource") = Nothing
            Return Nothing
        End If
        Session("CohortTableSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("CohortTableSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("CohortTableSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        GridViewCohort.PageIndex = 0
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        GridViewCohort.PageIndex = 0
        FillFieldLists()
        BuildAndBindAnalysis()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportAnalysis("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportAnalysis("xls")
    End Sub

    Private Sub lnkCohortAI_Click(sender As Object, e As EventArgs) Handles lnkCohortAI.Click
        Dim dt As DataTable = TryCast(Session("CohortTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("CohortTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No cohort analysis results to send to AI."
            Exit Sub
        End If
        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Cohort Analysis", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this cohort analysis grid. Explain the strongest findings, unusual records, and business meaning of the results.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing Then Exit Sub
        DropDownPrimaryField.Items.Clear()

        DropDownDateField.Items.Clear()
        DropDownValueField.Items.Clear()

        For Each col As DataColumn In source.Columns
            DropDownPrimaryField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))

            If LooksLikeDate(source, col) Then DropDownDateField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            If ColumnTypeIsNumeric(col) Then DropDownValueField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))

        Next
        If DropDownValueField.Items.Count = 0 Then DropDownValueField.Items.Add(New ListItem("(records)", ""))

        If DropDownDateField.Items.Count = 0 Then DropDownDateField.Items.Add(New ListItem("(no date field)", ""))

    End Sub

    Private Sub BuildAndBindAnalysis()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        Dim output As New DataTable()
        output.Columns.Add("Cohort Period", GetType(String))
        output.Columns.Add("Activity Period", GetType(String))
        output.Columns.Add("Period Number", GetType(Integer))
        output.Columns.Add("Entities", GetType(Integer))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("Value", GetType(Double))
        output.Columns.Add("Retention %", GetType(Double))
        output.Columns.Add("FilterId", GetType(String))
        If source Is Nothing OrElse DropDownPrimaryField.SelectedValue = "" OrElse DropDownDateField.SelectedValue = "" Then
            LabelError.Text = "Select entity and date fields."
            BindAnalysisGrid(output)
            Return
        End If
        Dim entityCol As String = DropDownPrimaryField.SelectedValue
        Dim dateCol As String = DropDownDateField.SelectedValue
        Dim valueCol As String = DropDownValueField.SelectedValue
        Dim periodName As String = DropDownPeriod.SelectedValue
        Dim firstPeriod As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim firstSort As New Dictionary(Of String, DateTime)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim entity As String = FieldText(row(entityCol)).Trim()
            Dim d As DateTime
            If entity = "" OrElse Not DateTime.TryParse(FieldText(row(dateCol)), d) Then Continue For
            If Not firstSort.ContainsKey(entity) OrElse d < firstSort(entity) Then
                firstSort(entity) = d
                firstPeriod(entity) = PeriodText(row(dateCol), periodName)
            End If
        Next
        Dim buckets As New Dictionary(Of String, Dictionary(Of String, Object))()
        Dim cohortEntities As New Dictionary(Of String, Dictionary(Of String, Boolean))()
        For Each row As DataRow In source.Rows
            If Not ContainsSearch(row, txtSearch.Text) Then Continue For
            Dim entity As String = FieldText(row(entityCol)).Trim()
            If entity = "" OrElse Not firstPeriod.ContainsKey(entity) Then Continue For
            Dim activityPeriod As String = PeriodText(row(dateCol), periodName)
            If activityPeriod = "" Then Continue For
            Dim cohortPeriod As String = firstPeriod(entity)
            Dim key As String = cohortPeriod & Chr(9) & activityPeriod
            If Not buckets.ContainsKey(key) Then
                Dim b As New Dictionary(Of String, Object)()
                b("Entities") = New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
                b("Records") = 0
                b("Value") = 0.0
                buckets(key) = b
            End If
            CType(buckets(key)("Entities"), Dictionary(Of String, Boolean))(entity) = True
            buckets(key)("Records") = CInt(buckets(key)("Records")) + 1
            If valueCol <> "" Then buckets(key)("Value") = CDbl(buckets(key)("Value")) + NumericValue(row(valueCol))
            If Not cohortEntities.ContainsKey(cohortPeriod) Then cohortEntities(cohortPeriod) = New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            cohortEntities(cohortPeriod)(entity) = True
        Next
        For Each key As String In buckets.Keys
            Dim parts() As String = key.Split(Chr(9))
            Dim b As Dictionary(Of String, Object) = buckets(key)
            Dim rowOut As DataRow = output.NewRow()
            Dim entCount As Integer = CType(b("Entities"), Dictionary(Of String, Boolean)).Count
            rowOut("Cohort Period") = parts(0)
            rowOut("Activity Period") = parts(1)
            rowOut("Period Number") = Math.Max(0, PeriodIndex(parts(1), periodName) - PeriodIndex(parts(0), periodName))
            rowOut("Entities") = entCount
            rowOut("Records") = CInt(b("Records"))
            rowOut("Value") = Math.Round(CDbl(b("Value")), 4)
            If cohortEntities.ContainsKey(parts(0)) AndAlso cohortEntities(parts(0)).Count > 0 Then rowOut("Retention %") = Math.Round(entCount * 100.0 / cohortEntities(parts(0)).Count, 2)
            rowOut("FilterId") = RegisterAnalysisFilter(PeriodFilter(dateCol, parts(1), periodName))
            output.Rows.Add(rowOut)
        Next
        output.DefaultView.Sort = "Cohort Period, Activity Period"
        BindAnalysisGrid(output.DefaultView.ToTable())
    End Sub

    Private Sub GridViewCohort_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewCohort.RowDataBound
        If e.Row.Cells.Count = 0 Then Exit Sub
        Dim dt As DataTable = TryCast(Session("CohortTable"), DataTable)
        If dt Is Nothing Then Exit Sub
        HideFilterColumnsInRow(dt, e.Row)
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        AddRecordLink(e.Row, dt, "Records", "FilterId", "cohortfilter")
    End Sub

    Private Sub BindAnalysisGrid(ByVal dt As DataTable)
        Session("CohortTable") = dt
        Session(AnalysisGridSessionKey()) = dt
        If dt Is Nothing Then
            GridViewCohort.AllowPaging = False
            GridViewCohort.PageIndex = 0
            GridViewCohort.DataSource = Nothing
            GridViewCohort.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If
        GridViewCohort.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewCohort.PageSize = AnalysisGridPageSize
        If Not GridViewCohort.AllowPaging Then GridViewCohort.PageIndex = 0
        GridViewCohort.DataSource = dt
        GridViewCohort.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
        LabelInfo.Text = "Cohort Analysis (" & dt.Rows.Count.ToString() & " rows)"
        AnalysisExportSnapshot.Save(Me, "CohortAnalysis", "Cohort Analysis", LabelInfo, GridViewCohort, dt)
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
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewCohort.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewCohort.PageIndex < (GridViewCohort.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewCohort.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewCohort.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewCohort.PageIndex > 0 Then GridViewCohort.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewCohort.PageIndex < (GridViewCohort.PageCount - 1) Then GridViewCohort.PageIndex += 1
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
            GridViewCohort.PageIndex = requestedPage - 1
        End If
        BindAnalysisGrid(dt)
    End Sub

    Private Function AnalysisGridSessionKey() As String
        Return "AnalysisGrid_" & Page.AppRelativeVirtualPath
    End Function

    Private Sub ExportAnalysis(formatName As String)
        Dim dt As DataTable = TryCast(Session("CohortTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindAnalysis()
            dt = TryCast(Session("CohortTable"), DataTable)
        End If
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No cohort analysis results to export."
            Exit Sub
        End If
        Dim publicTable As DataTable = GridTableForAI(dt)
        Dim fileName As String = "Cohort_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(publicTable, ",", "Cohort Analysis", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(publicTable, Chr(9), "Cohort Analysis", ""))
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
        LabelModelExplanation.Text = ExplanationBlock("Input and Fields Selection", "Entity field selects the customer, user, account, product, or other entity being followed.", "Date field identifies the activity date used to place records into periods.", "Value field is optional and summarizes value generated by the cohort.", "Period grain controls how dates are grouped, such as month or quarter.", "Search filters source records before cohorts are built.")
        LabelAlgorithmExplanation.Text = ExplanationBlock("Model and Algorithm", "Cohort model groups entities by their first observed activity period.", "The first period for each entity becomes the cohort period.", "Later activity is grouped by cohort period and activity period.", "The page counts active entities and records in each period.", "Retention percent compares active entities to the original cohort size.")
        LabelOutputExplanation.Text = ExplanationBlock("Output", "Cohort Period shows when the entity first appeared.", "Activity Period shows the later period being measured.", "Period Number shows how far the activity period is from the cohort start.", "Entities, Records, Value, and Retention % summarize cohort behavior.", "Records links open the rows behind each cohort period.")
        ReadinessFooterGuidance.SetFooter(Me, "Cohort Analysis", LabelReadinessWhyUseful, LabelReadinessSuggestedFields, GetSourceTable())
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

    Private Function PeriodIndex(periodValue As String, periodName As String) As Integer
        If periodValue.Trim() = "" Then Return 0
        Select Case periodName
            Case "Year"
                Dim yearValue As Integer = 0
                Integer.TryParse(periodValue, yearValue)
                Return yearValue
            Case "Quarter"
                Dim parts() As String = periodValue.Replace(" Q", "-").Split("-"c)
                If parts.Length = 2 Then
                    Dim yearValue As Integer = 0
                    Dim quarterValue As Integer = 0
                    Integer.TryParse(parts(0), yearValue)
                    Integer.TryParse(parts(1), quarterValue)
                    Return (yearValue * 4) + quarterValue
                End If
            Case Else
                Dim d As DateTime
                If DateTime.TryParse(periodValue & "-01", d) Then Return (d.Year * 12) + d.Month
        End Select
        Return 0
    End Function

    Private Function PeriodFilter(dateColumn As String, periodValue As String, periodName As String) As String
        Dim startDate As DateTime
        Dim endDate As DateTime
        Select Case periodName
            Case "Year"
                Dim yearValue As Integer = 0
                If Not Integer.TryParse(periodValue, yearValue) Then Return "1 = 1"
                startDate = New DateTime(yearValue, 1, 1)
                endDate = startDate.AddYears(1)
            Case "Quarter"
                Dim parts() As String = periodValue.Replace(" Q", "-").Split("-"c)
                If parts.Length <> 2 Then Return "1 = 1"
                Dim yearValue As Integer = 0
                Dim quarterValue As Integer = 0
                Integer.TryParse(parts(0), yearValue)
                Integer.TryParse(parts(1), quarterValue)
                If yearValue = 0 OrElse quarterValue < 1 OrElse quarterValue > 4 Then Return "1 = 1"
                startDate = New DateTime(yearValue, ((quarterValue - 1) * 3) + 1, 1)
                endDate = startDate.AddMonths(3)
            Case Else
                If Not DateTime.TryParse(periodValue & "-01", startDate) Then Return "1 = 1"
                endDate = startDate.AddMonths(1)
        End Select
        Dim columnText As String = "[" & dateColumn.Replace("]", "]]") & "]"
        Return columnText & " >= #" & startDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) & "# AND " & columnText & " < #" & endDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) & "#"
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
        Dim filters As Dictionary(Of String, String) = TryCast(Session("CohortFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then filters = New Dictionary(Of String, String)()
        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = filterExpression
        Session("CohortFilters") = filters
        Return filterId
    End Function

End Class
