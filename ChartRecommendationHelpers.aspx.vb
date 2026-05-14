Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Text.RegularExpressions
Imports System.Web.UI.WebControls

Partial Class ChartRecommendationHelpers
    Inherits System.Web.UI.Page

    Private Const MaxRecommendedRecords As Integer = 1000
    Private Const MaxRawRecommendationRecords As Integer = 3000
    Private Const MaxAutoCategoryFields As Integer = 40
    Private Const MaxAutoValueFields As Integer = 40
    Private Const MaxAutoDateFields As Integer = 8
    Private Const MaxCategoryCombinations As Integer = 160
    Private Const MaxValueCombinations As Integer = 160

    Private Sub ChartRecommendationHelpers_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Chart Recommendations"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Chart%20Recommendations"

        ' Handle Open Chart link click - set sessions and redirect to ChartGoogleOne
        If Request("openchart") IsNot Nothing AndAlso Request("openchart").ToString().Trim() <> "" Then
            Dim dt As DataTable = TryCast(Session("ChartRecommendationTable"), DataTable)
            If dt IsNot Nothing Then
                Dim rowIndex As Integer
                If Integer.TryParse(Request("openchart").ToString(), rowIndex) AndAlso rowIndex >= 0 AndAlso rowIndex < dt.Rows.Count Then
                    Dim row As DataRow = dt.Rows(rowIndex)
                    Dim axisX As String = row("Axis X").ToString()
                    Dim axisY As String = row("Axis Y").ToString()
                    Dim chartType As String = row("Chart Type").ToString()

                    Session("arr") = ""
                    Session("ttl") = ""
                    Session("writetext") = ""

                    If chartType.Trim() = "" Then
                        Response.Redirect(row("Open Chart").ToString())
                        Return
                    End If

                    PrepareChartSession(axisX, axisY, chartType)

                    Dim reportId As String = ""
                    If Session("REPORTID") IsNot Nothing Then reportId = Session("REPORTID").ToString()

                    Response.Redirect(ChartRedirectUrl(reportId, axisX, axisY, chartType))
                End If
            End If
        End If

        ButtonBuild.ToolTip = "Rebuild chart recommendations based on the selected category, value, and date fields."
        ButtonReset.ToolTip = "Clear the search filter and rebuild all chart recommendations."
        ButtonSearch.ToolTip = "Filter the recommendations grid by the search text."
        ButtonExportCSV.ToolTip = "Export the current recommendations list to a CSV file for download."
        ButtonExportExcel.ToolTip = "Export the current recommendations list to an Excel file for download."
        ButtonDashboard.ToolTip = "Create a new dashboard from selected charts. If none are checked, only Highest priority charts are included."
        txtSearch.ToolTip = "Type search text and press Enter to filter recommendations."
    End Sub

    Private Sub ChartRecommendationHelpers_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindRecommendations()
        ElseIf Session("ChartRecommendationTable") IsNot Nothing Then
            BindRecommendations(CType(Session("ChartRecommendationTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        Dim ret As String = ""
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()
        If repid.Trim() = "" Then Return Nothing
        Dim dv As DataView = Nothing
        Try
            dv = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            Return Nothing
        End Try
        If dv Is Nothing OrElse dv.Table Is Nothing Then Return Nothing
        Session("ChartRecommendationSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("ChartRecommendationSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("ChartRecommendationSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub
        DropDownCategoryField.Items.Clear()
        DropDownDateField.Items.Clear()
        DropDownValueField.Items.Clear()
        DropDownCategoryField.Items.Add(" ")
        DropDownDateField.Items.Add(New ListItem("(None)", ""))
        DropDownValueField.Items.Add(" ")
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            If IsNumericAnalysisField(dt, fld) Then
                If Not IsIndexFieldName(fld) Then DropDownValueField.Items.Add(fld)
            Else
                DropDownCategoryField.Items.Add(fld)
            End If
            If dt.Columns(i).DataType Is GetType(DateTime) OrElse LooksLikeDate(dt, fld) Then DropDownDateField.Items.Add(New ListItem(fld, fld))
        Next
        DropDownCategoryField.Text = "Please select..."
        DropDownValueField.Text = "Please select..."
        RestoreSelections()
    End Sub

    Private Function IsIndexFieldName(columnName As String) As Boolean
        Dim name As String = columnName.Trim().ToLower()
        Return name = "id" OrElse name = "ind" OrElse name = "indx" OrElse name = "recordid" OrElse
               name.EndsWith("id") OrElse name.EndsWith("_indx") OrElse name.EndsWith("_recordid")
    End Function

    Private Function IsNumericAnalysisField(dt As DataTable, columnName As String) As Boolean
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then Return False
        If ColumnTypeIsNumeric(dt.Columns(columnName)) Then Return True

        Dim checkedValues As Integer = 0
        For i As Integer = 0 To Math.Min(50, dt.Rows.Count) - 1
            Dim txt As String = FieldText(dt.Rows(i)(columnName)).Trim()
            If txt = "" Then Continue For
            Dim value As Double
            checkedValues += 1
            If Not Double.TryParse(txt, value) Then Return False
        Next
        Return checkedValues > 0
    End Function

    Private Function NumericSelectionIsValid(dt As DataTable, columnName As String) As Boolean
        Return columnName.Trim() <> "" AndAlso IsNumericAnalysisField(dt, columnName)
    End Function

    Private Function LooksLikeDate(dt As DataTable, columnName As String) As Boolean
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

    Private Sub GridViewRecommendations_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewRecommendations.RowDataBound
        Dim dt As DataTable = TryCast(Session("ChartRecommendationTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("Open Chart") Then Exit Sub

        HideHelperColumn(e.Row, dt, "Chart Type")
        HideHelperColumn(e.Row, dt, "Axis X")
        HideHelperColumn(e.Row, dt, "Axis Y")
        HideHelperColumn(e.Row, dt, "Score")

        If e.Row.RowType <> DataControlRowType.DataRow OrElse e.Row.Cells.Count = 0 Then Exit Sub

        Dim chartType As String = ""
        Dim dataRowIndex As Integer = RecommendationDataIndex(e.Row.RowIndex)
        If dt.Columns.Contains("Chart Type") AndAlso dataRowIndex >= 0 AndAlso dataRowIndex < dt.Rows.Count Then chartType = dt.Rows(dataRowIndex)("Chart Type").ToString().Trim()

        Dim dashboardCheckBox As CheckBox = TryCast(e.Row.FindControl("chkAddToDashboard"), CheckBox)
        If dashboardCheckBox IsNot Nothing AndAlso Not IsDashboardSafeChartType(chartType) Then
            dashboardCheckBox.Enabled = False
            If chartType = "" Then
                dashboardCheckBox.ToolTip = "This row opens the report and charts page, so it is not added to dashboard."
            Else
                dashboardCheckBox.ToolTip = "This chart type is available to open, but is not added to dashboard."
            End If
        End If

        Dim linkIndex As Integer = DataColumnCellIndex(dt, "Open Chart")
        If linkIndex < 0 OrElse linkIndex >= e.Row.Cells.Count Then Exit Sub

        Dim url As String = e.Row.Cells(linkIndex).Text.Replace("&nbsp;", "").Trim()
        If url = "" Then Exit Sub

        Dim link As New HyperLink()
        link.Text = "open chart"
        If chartType = "" Then link.Text = "open data"
        If chartType = "" Then
            link.NavigateUrl = url
        Else
            link.NavigateUrl = "ChartRecommendationHelpers.aspx?openchart=" & dataRowIndex.ToString()
        End If
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open recommended chart or data."
        e.Row.Cells(linkIndex).Controls.Clear()
        e.Row.Cells(linkIndex).Controls.Add(link)
    End Sub

    Private Sub GridViewRecommendations_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewRecommendations.RowCommand
        If e.CommandName <> "OpenChart" Then Exit Sub

        Dim dt As DataTable = TryCast(Session("ChartRecommendationTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim rowIndex As Integer
        If Not Integer.TryParse(e.CommandArgument.ToString(), rowIndex) OrElse rowIndex < 0 OrElse rowIndex >= dt.Rows.Count Then Exit Sub

        Dim row As DataRow = dt.Rows(rowIndex)
        Dim axisX As String = row("Axis X").ToString()
        Dim axisY As String = row("Axis Y").ToString()
        Dim chartType As String = row("Chart Type").ToString()

        Session("arr") = ""
        Session("ttl") = ""
        Session("writetext") = ""
        If chartType.Trim() = "" Then
            Response.Redirect(row("Open Chart").ToString())
            Exit Sub
        End If

        PrepareChartSession(axisX, axisY, chartType)

        Dim reportId As String = ""
        If Session("REPORTID") IsNot Nothing Then reportId = Session("REPORTID").ToString()

        Response.Redirect(ChartRedirectUrl(reportId, axisX, axisY, chartType))
    End Sub

    Private Sub PrepareChartSession(axisX As String, axisY As String, chartType As String)
        Session("AxisXM") = axisX
        Session("AxisYM") = axisY
        Session("AggregateM") = "Sum"
        Session("Aggregate") = "Sum"
        Session("ChartType") = chartType
        Session("cat1") = FirstSelectedField(axisX)
        Session("cat2") = SecondSelectedField(axisX)
        Session("AxisY") = FirstSelectedField(axisY)
        Session("y1") = FirstSelectedField(axisY)
        Session("y2") = axisY
        Session("nYselM") = SelectedFieldCount(axisY)
        Session("MFld") = " "
        Session("SELECTEDValuesM") = " "
        Session("fnM") = "Sum"
        Session("fn") = "Sum"
        Session("fn2") = "Sum"
        Session("nv") = 8
        Session("frm") = "Analytics"
        Session("MapChart") = ""
        Session("MatrixChart") = ""
        Session("newarr") = "yes"
        Session("srt") = ""
        Session("dv3") = Nothing
    End Sub

    Private Function ChartRedirectUrl(reportId As String, axisX As String, axisY As String, chartType As String) As String
        Dim url As String = "ChartGoogleOne.aspx?Report=" & Server.UrlEncode(reportId) &
            "&x1=" & Server.UrlEncode(FirstSelectedField(axisX)) &
            "&x2=" & Server.UrlEncode(SecondSelectedField(axisX)) &
            "&y1=" & Server.UrlEncode(FirstSelectedField(axisY)) &
            "&fn=Sum" &
            "&charttype=" & Server.UrlEncode(chartType) &
            "&frm=Analytics"

        Dim isMultiX As Boolean = axisX.IndexOf(","c) >= 0
        Dim isMultiY As Boolean = axisY.IndexOf(","c) >= 0
        If isMultiX Then url &= "&mx=" & Server.UrlEncode(axisX)
        If isMultiX OrElse isMultiY Then url &= "&y2=" & Server.UrlEncode(axisY) & "&fn2=Sum&domulti=yes"
        Return url
    End Function

    Private Sub HideHelperColumn(row As GridViewRow, dt As DataTable, columnName As String)
        If Not dt.Columns.Contains(columnName) Then Exit Sub
        Dim columnIndex As Integer = DataColumnCellIndex(dt, columnName)
        If columnIndex >= 0 AndAlso columnIndex < row.Cells.Count Then row.Cells(columnIndex).Visible = False
    End Sub

    Private Function DataColumnCellIndex(dt As DataTable, columnName As String) As Integer
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then Return -1
        Return GridViewRecommendations.Columns.Count + dt.Columns.IndexOf(columnName)
    End Function

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        Session("ChartRecommendationFiltered") = Nothing
        BuildAndBindRecommendations()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        Session("ChartRecommendationFiltered") = Nothing
        GridViewRecommendations.PageIndex = 0
        BuildAndBindRecommendations()
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        SearchGrid()
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        SearchGrid()
    End Sub

    Private Sub SearchGrid()
        Dim dt As DataTable = TryCast(Session("ChartRecommendationTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRecommendations()
            Exit Sub
        End If
        Dim searchText As String = txtSearch.Text.Trim()
        If searchText = "" Then
            Session("ChartRecommendationFiltered") = Nothing
            GridViewRecommendations.PageIndex = 0
            BindRecommendations(dt)
            Exit Sub
        End If
        Dim filtered As DataTable = dt.Copy()
        filtered = ApplySearchFilter(filtered, searchText)
        Session("ChartRecommendationFiltered") = filtered
        GridViewRecommendations.PageIndex = 0
        BindRecommendations(filtered)
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportRecommendations("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportRecommendations("xls")
    End Sub

    Private Sub ButtonDashboard_Click(sender As Object, e As EventArgs) Handles ButtonDashboard.Click
        BuildRecommendedDashboard()
    End Sub

    Private Sub BuildAndBindRecommendations()
        Dim sourceDt As DataTable = GetSourceTable()
        Dim dt As New DataTable()
        dt.Columns.Add("Recommended Chart", GetType(String))
        dt.Columns.Add("Priority", GetType(String))
        dt.Columns.Add("Fields", GetType(String))
        dt.Columns.Add("Reason", GetType(String))
        dt.Columns.Add("Open Chart", GetType(String))
        dt.Columns.Add("Chart Type", GetType(String))
        dt.Columns.Add("Axis X", GetType(String))
        dt.Columns.Add("Axis Y", GetType(String))
        dt.Columns.Add("Score", GetType(Integer))

        Dim selectedValueText As String = SelectedValueFields(sourceDt)
        Dim selectedCategoryText As String = SelectedCategoryFields()
        Dim selectedDateField As String = DropDownDateField.SelectedValue.Trim()
        SaveSelections(selectedCategoryText, selectedValueText)

        Dim valueList As List(Of String) = AllowedValueFields(sourceDt, selectedValueText)
        Dim categoryList As List(Of String) = AllowedCategoryFields(sourceDt, selectedCategoryText)
        Dim dateList As List(Of String) = AllowedDateFields(sourceDt, selectedDateField)
        If selectedValueText.Trim() = "" Then valueList = DistributedFieldSample(valueList, MaxAutoValueFields)
        If selectedCategoryText.Trim() = "" Then categoryList = DistributedFieldSample(categoryList, MaxAutoCategoryFields)
        If selectedDateField.Trim() = "" Then dateList = DistributedFieldSample(dateList, MaxAutoDateFields)
        Dim categoryCombinations As List(Of String) = CategoryFieldCombinations(categoryList, selectedCategoryText.Trim() <> "")
        Dim valueCombinations As List(Of String) = ValueFieldCombinations(valueList)

        For Each dateField As String In dateList
            For Each valueField As String In valueList
                AddRecommendation(dt, "Line Chart", "High", dateField & ", " & valueField, "Best for showing " & valueField & " over " & dateField & ".", "LineChart", dateField, valueField)
                AddRecommendation(dt, "Column Chart", "High", dateField & ", " & valueField, "Column comparison of " & valueField & " over " & dateField & ".", "ColumnChart", dateField, valueField)
            Next
            For Each valueFields As String In valueCombinations
                AddRecommendation(dt, "Area Chart", "High", dateField & ", " & valueFields, "Area comparison of multiple values over " & dateField & ".", "AreaChart", dateField, valueFields)
                AddRecommendation(dt, "Stepped Area Chart", "High", dateField & ", " & valueFields, "Stepped area comparison of multiple values over " & dateField & ".", "SteppedAreaChart", dateField, valueFields)
                AddRecommendation(dt, "Line Chart", "High", dateField & ", " & valueFields, "Line comparison of multiple numeric values over " & dateField & ".", "LineChart", dateField, valueFields)
                AddRecommendation(dt, "Column Chart", "Medium", dateField & ", " & valueFields, "Column comparison of multiple values over " & dateField & ".", "ColumnChart", dateField, valueFields)
            Next
        Next

        For Each categoryFields As String In categoryCombinations
            For Each valueField As String In valueList
                AddRecommendation(dt, "Line Chart", "High", categoryFields & ", " & valueField, "Line view of " & valueField & " by " & CategoryText(categoryFields) & ".", "LineChart", categoryFields, valueField)
                AddRecommendation(dt, "Column Chart", "High", categoryFields & ", " & valueField, "Vertical column comparison of " & valueField & " by " & CategoryText(categoryFields) & ".", "ColumnChart", categoryFields, valueField)
                AddRecommendation(dt, "Pie Chart", "High", categoryFields & ", " & valueField, "Useful for contribution-to-total of " & valueField & " by " & CategoryText(categoryFields) & ".", "PieChart", categoryFields, valueField)
                AddRecommendation(dt, "Histogram", "High", categoryFields & ", " & valueField, "Useful for seeing how " & valueField & " is distributed across " & CategoryText(categoryFields) & ".", "Histogram", categoryFields, valueField)
            Next
            For Each valueFields As String In valueCombinations
                AddRecommendation(dt, "Area Chart", "High", categoryFields & ", " & valueFields, "Area comparison of multiple values by " & CategoryText(categoryFields) & ".", "AreaChart", categoryFields, valueFields)
                AddRecommendation(dt, "Stepped Area Chart", "High", categoryFields & ", " & valueFields, "Stepped area comparison of multiple values by " & CategoryText(categoryFields) & ".", "SteppedAreaChart", categoryFields, valueFields)
                AddRecommendation(dt, "Line Chart", "Medium", categoryFields & ", " & valueFields, "Line comparison of multiple values by " & CategoryText(categoryFields) & ".", "LineChart", categoryFields, valueFields)
                AddRecommendation(dt, "Column Chart", "High", categoryFields & ", " & valueFields, "Column comparison of multiple values by " & CategoryText(categoryFields) & ".", "ColumnChart", categoryFields, valueFields)
            Next
        Next

        For Each valueFields As String In valueCombinations
            AddRecommendation(dt, "Scatter Chart", "High", valueFields, "Best for relationships between numeric fields.", "ScatterChart", valueFields, valueFields)
        Next

        For Each categoryFields As String In categoryCombinations
            If SelectedFieldCount(categoryFields) <> 2 Then Continue For
            For Each valueField As String In valueList
                AddRecommendation(dt, "Bubble Chart", "Medium", categoryFields & ", " & valueField, "Useful when " & CategoryText(categoryFields) & " describe each point.", "BubbleChart", categoryFields, valueField)
                AddRecommendation(dt, "Sankey Chart", "Medium", categoryFields & ", " & valueField, "Useful for showing flow across " & CategoryText(categoryFields) & ".", "Sankey", categoryFields, valueField)
            Next
        Next

        For Each categoryFields As String In categoryCombinations
            If SelectedFieldCount(categoryFields) <> 1 Then Continue For
            If valueList.Count >= 1 Then
                AddRecommendation(dt, "Bubble Chart", "Low", categoryFields & ", " & JoinFields(valueList), "Bubble view of values by " & CategoryText(categoryFields) & ".", "BubbleChart", categoryFields, If(valueList.Count > 1, valueList(0) & "," & valueList(1), valueList(0)))
                AddRecommendation(dt, "Sankey Chart", "Low", categoryFields & ", " & valueList(0), "Flow of " & valueList(0) & " across " & CategoryText(categoryFields) & ".", "Sankey", categoryFields, valueList(0))
            End If
        Next

        For Each valueField As String In valueList
            AddRecommendation(dt, "Gauge Chart", "Medium", valueField, "Gauge view of " & valueField & " showing current value against a range.", "Gauge", valueField, valueField)
        Next

        AddRecommendation(dt, "Report and Charts", "Fallback", "Current report fields", "Use when selected fields are not suitable for a compact chart.", "", "", "")

        dt = RemoveDuplicateRecommendations(dt)
        dt = AddHighestPriorityRows(dt)
        Dim totalPossible As Integer = dt.Rows.Count
        dt = ApplySearchFilter(dt, txtSearch.Text.Trim())
        dt = SortRecommendations(dt)
        dt = LimitRecommendationRows(dt, MaxRecommendedRecords)

        Session("ChartRecommendationTable") = dt
        Session("ChartRecommendationTotalPossible") = totalPossible
        BindRecommendations(dt, totalPossible)
    End Sub

    Private Function LimitRecommendationRows(dt As DataTable, maxRows As Integer) As DataTable
        If dt Is Nothing OrElse maxRows <= 0 OrElse dt.Rows.Count <= maxRows Then Return dt
        Dim limited As DataTable = dt.Clone()
        For i As Integer = 0 To maxRows - 1
            limited.ImportRow(dt.Rows(i))
        Next
        limited.AcceptChanges()
        Return limited
    End Function

    Private Function RemoveDuplicateRecommendations(dt As DataTable) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return dt

        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim toRemove As New List(Of DataRow)()

        For Each row As DataRow In dt.Rows
            Dim chartType As String = row("Chart Type").ToString().Trim()
            Dim axisX As String = row("Axis X").ToString().Trim()
            Dim axisY As String = row("Axis Y").ToString().Trim()

            If chartType = "" Then Continue For

            Dim key As String = chartType & "|" & axisX & "|" & axisY
            If seen.Contains(key) Then
                toRemove.Add(row)
            Else
                seen.Add(key)
            End If
        Next

        For Each row As DataRow In toRemove
            dt.Rows.Remove(row)
        Next
        dt.AcceptChanges()
        Return dt
    End Function

    Private Function AddHighestPriorityRows(dt As DataTable) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return dt

        Dim rowsByFields As New Dictionary(Of String, List(Of Integer))(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim ct As String = dt.Rows(i)("Chart Type").ToString().Trim()
            Dim fv As String = dt.Rows(i)("Fields").ToString().Trim()
            Dim axisY As String = dt.Rows(i)("Axis Y").ToString().Trim()
            If ct = "" Then Continue For
            If Not IsDashboardSafeChartType(ct) Then Continue For
            If fv = "" OrElse axisY = "" Then Continue For
            If ContainsIndexField(fv) OrElse ContainsIndexField(dt.Rows(i)("Axis X").ToString().Trim()) OrElse ContainsIndexField(axisY) Then Continue For
            If SelectedFieldCount(axisY) > 1 AndAlso Not SupportsMultipleYValues(ct) Then Continue For
            If Not rowsByFields.ContainsKey(fv) Then rowsByFields(fv) = New List(Of Integer)()
            rowsByFields(fv).Add(i)
        Next

        Dim promotedSet As HashSet(Of Integer) = AssignHighestRoundRobin(dt, rowsByFields)

        ' Apply Highest priority
        For Each idx As Integer In promotedSet
            Dim ct As String = dt.Rows(idx)("Chart Type").ToString().Trim()
            dt.Rows(idx)("Priority") = "Highest"
            dt.Rows(idx)("Score") = RecommendationScore("Highest", ct, dt.Rows(idx)("Axis X").ToString().Trim(), dt.Rows(idx)("Axis Y").ToString().Trim())
        Next

        dt.AcceptChanges()
        Return dt
    End Function

    Private Function AssignHighestRoundRobin(dt As DataTable, rowsByFields As Dictionary(Of String, List(Of Integer))) As HashSet(Of Integer)
        Dim promotedSet As New HashSet(Of Integer)()
        Dim assignedFields As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If rowsByFields Is Nothing OrElse rowsByFields.Count = 0 Then Return promotedSet

        Dim madeAssignment As Boolean = True
        While madeAssignment AndAlso assignedFields.Count < rowsByFields.Count
            madeAssignment = False
            For Each chartType As String In DashboardChartTypeOrder()
                If assignedFields.Count >= rowsByFields.Count Then Exit For

                Dim fieldKey As String = BestUnassignedFieldForChart(dt, rowsByFields, assignedFields, chartType)
                If fieldKey = "" Then Continue For

                Dim rowIndex As Integer = BestRowForChart(dt, rowsByFields(fieldKey), chartType)
                If rowIndex < 0 Then Continue For

                promotedSet.Add(rowIndex)
                assignedFields.Add(fieldKey)
                madeAssignment = True
            Next
        End While

        AssignRemainingHighest(dt, rowsByFields, assignedFields, promotedSet)
        Return promotedSet
    End Function

    Private Function EmptyChartTypeCounts(rowsByFields As Dictionary(Of String, List(Of Integer)), dt As DataTable) As Dictionary(Of String, Integer)
        Dim counts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each fieldKey As String In rowsByFields.Keys
            For Each chartType As String In ChartTypesForField(dt, rowsByFields(fieldKey))
                If Not counts.ContainsKey(chartType) Then counts(chartType) = 0
            Next
        Next
        Return counts
    End Function

    Private Function HighestChartTypeTargets(rowsByFields As Dictionary(Of String, List(Of Integer)), dt As DataTable, chartTypeCount As Integer) As Dictionary(Of String, Integer)
        Dim targets As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        If rowsByFields Is Nothing OrElse rowsByFields.Count = 0 OrElse chartTypeCount = 0 Then Return targets

        Dim supportCounts As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each fieldKey As String In rowsByFields.Keys
            For Each chartType As String In ChartTypesForField(dt, rowsByFields(fieldKey))
                If Not supportCounts.ContainsKey(chartType) Then supportCounts(chartType) = 0
                supportCounts(chartType) += 1
                If Not targets.ContainsKey(chartType) Then targets(chartType) = 0
            Next
        Next

        For i As Integer = 1 To rowsByFields.Count
            Dim selectedChartType As String = ""
            For Each chartType As String In OrderedChartTypes(supportCounts.Keys)
                If targets(chartType) >= supportCounts(chartType) Then Continue For
                If selectedChartType = "" Then
                    selectedChartType = chartType
                ElseIf targets(chartType) < targets(selectedChartType) Then
                    selectedChartType = chartType
                End If
            Next
            If selectedChartType = "" Then Exit For
            targets(selectedChartType) += 1
        Next

        Return targets
    End Function

    Private Sub AssignHighestByTargets(dt As DataTable, rowsByFields As Dictionary(Of String, List(Of Integer)), targets As Dictionary(Of String, Integer), counts As Dictionary(Of String, Integer), assignedFields As HashSet(Of String), promotedSet As HashSet(Of Integer))
        If rowsByFields Is Nothing OrElse targets Is Nothing Then Exit Sub

        Dim madeAssignment As Boolean = True
        While madeAssignment
            madeAssignment = False
            For Each chartType As String In OrderedChartTypes(targets.Keys)
                If Not counts.ContainsKey(chartType) Then counts(chartType) = 0
                If counts(chartType) >= targets(chartType) Then Continue For

                Dim fieldKey As String = BestUnassignedFieldForChart(dt, rowsByFields, assignedFields, chartType)
                If fieldKey = "" Then Continue For

                Dim rowIndex As Integer = BestRowForChart(dt, rowsByFields(fieldKey), chartType)
                If rowIndex < 0 Then Continue For

                promotedSet.Add(rowIndex)
                assignedFields.Add(fieldKey)
                counts(chartType) += 1
                madeAssignment = True
            Next
        End While
    End Sub

    Private Sub AssignRemainingHighest(dt As DataTable, rowsByFields As Dictionary(Of String, List(Of Integer)), assignedFields As HashSet(Of String), promotedSet As HashSet(Of Integer))
        Dim fieldKeys As New List(Of String)(rowsByFields.Keys)
        fieldKeys.Sort(StringComparer.OrdinalIgnoreCase)

        For Each fieldKey As String In fieldKeys
            If assignedFields.Contains(fieldKey) Then Continue For

            Dim bestIndex As Integer = -1
            For Each chartType As String In DashboardChartTypeOrder()
                Dim rowIndex As Integer = BestRowForChart(dt, rowsByFields(fieldKey), chartType)
                If rowIndex >= 0 Then
                    bestIndex = rowIndex
                    Exit For
                End If
            Next

            If bestIndex >= 0 Then
                promotedSet.Add(bestIndex)
                assignedFields.Add(fieldKey)
            End If
        Next
    End Sub

    Private Sub GridViewRecommendations_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridViewRecommendations.PageIndexChanging
        GridViewRecommendations.PageIndex = e.NewPageIndex
        Dim dt As DataTable = TryCast(Session("ChartRecommendationFiltered"), DataTable)
        If dt Is Nothing Then dt = TryCast(Session("ChartRecommendationTable"), DataTable)
        If dt Is Nothing Then
            BuildAndBindRecommendations()
        Else
            BindRecommendations(dt)
        End If
    End Sub

    Private Sub LinkButtonPrevious_Click(sender As Object, e As EventArgs) Handles LinkButtonPrevious.Click
        MoveRecommendationPage(GridViewRecommendations.PageIndex)
    End Sub

    Private Sub LinkButtonNext_Click(sender As Object, e As EventArgs) Handles LinkButtonNext.Click
        MoveRecommendationPage(GridViewRecommendations.PageIndex + 2)
    End Sub

    Private Sub TextBoxPageNumber_TextChanged(sender As Object, e As EventArgs) Handles TextBoxPageNumber.TextChanged
        Dim requestedPage As Integer = GridViewRecommendations.PageIndex + 1
        Dim postedPageText As String = TextBoxPageNumber.Text.Trim()
        If Request.Form(TextBoxPageNumber.UniqueID) IsNot Nothing Then postedPageText = Request.Form(TextBoxPageNumber.UniqueID).Trim()
        If Not Integer.TryParse(postedPageText, requestedPage) Then requestedPage = GridViewRecommendations.PageIndex + 1
        MoveRecommendationPage(requestedPage)
    End Sub

    Private Sub MoveRecommendationPage(pageNumber As Integer)
        Dim dt As DataTable = CurrentRecommendationGridTable()
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRecommendations()
            Return
        End If

        Dim pageCount As Integer = RecommendationPageCount(dt)
        If pageNumber < 1 Then pageNumber = 1
        If pageNumber > pageCount Then pageNumber = pageCount
        GridViewRecommendations.PageIndex = pageNumber - 1
        BindRecommendations(dt)
    End Sub

    Private Function RecommendationDataIndex(gridRowIndex As Integer) As Integer
        Return (GridViewRecommendations.PageIndex * GridViewRecommendations.PageSize) + gridRowIndex
    End Function

    Private Function BestUnassignedFieldForChart(dt As DataTable, rowsByFields As Dictionary(Of String, List(Of Integer)), assignedFields As HashSet(Of String), chartType As String) As String
        Dim bestField As String = ""
        Dim bestOptionCount As Integer = Integer.MaxValue
        Dim fieldKeys As New List(Of String)(rowsByFields.Keys)
        fieldKeys.Sort(StringComparer.OrdinalIgnoreCase)

        For Each fieldKey As String In fieldKeys
            If assignedFields.Contains(fieldKey) Then Continue For
            If BestRowForChart(dt, rowsByFields(fieldKey), chartType) < 0 Then Continue For

            Dim optionCount As Integer = ChartTypesForField(dt, rowsByFields(fieldKey)).Count
            If bestField = "" OrElse optionCount < bestOptionCount Then
                bestField = fieldKey
                bestOptionCount = optionCount
            End If
        Next

        Return bestField
    End Function

    Private Function BestRowForChart(dt As DataTable, indices As List(Of Integer), chartType As String) As Integer
        Dim bestIndex As Integer = -1
        For Each idx As Integer In indices
            If Not dt.Rows(idx)("Chart Type").ToString().Trim().Equals(chartType, StringComparison.OrdinalIgnoreCase) Then Continue For
            If bestIndex < 0 OrElse CInt(dt.Rows(idx)("Score")) > CInt(dt.Rows(bestIndex)("Score")) Then bestIndex = idx
        Next
        Return bestIndex
    End Function

    Private Function ChartTypesForField(dt As DataTable, indices As List(Of Integer)) As HashSet(Of String)
        Dim chartTypes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If indices Is Nothing Then Return chartTypes
        For Each idx As Integer In indices
            Dim chartType As String = dt.Rows(idx)("Chart Type").ToString().Trim()
            If chartType <> "" Then chartTypes.Add(chartType)
        Next
        Return chartTypes
    End Function

    Private Function OrderedChartTypes(chartTypes As ICollection(Of String)) As List(Of String)
        Dim ordered As New List(Of String)()
        Dim remaining As New HashSet(Of String)(chartTypes, StringComparer.OrdinalIgnoreCase)
        For Each chartType As String In DashboardChartTypeOrder()
            If remaining.Contains(chartType) Then
                ordered.Add(chartType)
                remaining.Remove(chartType)
            End If
        Next
        Dim extras As New List(Of String)(remaining)
        extras.Sort(StringComparer.OrdinalIgnoreCase)
        For Each chartType As String In extras
            ordered.Add(chartType)
        Next
        Return ordered
    End Function

    Private Function EligibleHighestChartTypes(dt As DataTable) As HashSet(Of String)
        Dim chartTypes As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        If dt Is Nothing Then Return chartTypes
        For Each row As DataRow In dt.Rows
            Dim ct As String = row("Chart Type").ToString().Trim()
            Dim fv As String = row("Fields").ToString().Trim()
            Dim axisY As String = row("Axis Y").ToString().Trim()
            If ct = "" OrElse fv = "" OrElse axisY = "" Then Continue For
            If Not IsDashboardSafeChartType(ct) Then Continue For
            If ContainsIndexField(fv) OrElse ContainsIndexField(row("Axis X").ToString().Trim()) OrElse ContainsIndexField(axisY) Then Continue For
            If SelectedFieldCount(axisY) > 1 AndAlso Not SupportsMultipleYValues(ct) Then Continue For
            chartTypes.Add(ct)
        Next
        Return chartTypes
    End Function

    Private Function BestBalancedRecommendationIndex(dt As DataTable, indices As List(Of Integer), chartTypeCounts As Dictionary(Of String, Integer), nextChartTypeIndex As Integer) As Integer
        If indices Is Nothing OrElse indices.Count = 0 Then Return -1

        indices.Sort(Function(a, b)
                         Dim aType As String = dt.Rows(a)("Chart Type").ToString().Trim()
                         Dim bType As String = dt.Rows(b)("Chart Type").ToString().Trim()
                         Dim aCount As Integer = If(chartTypeCounts.ContainsKey(aType), chartTypeCounts(aType), 0)
                         Dim bCount As Integer = If(chartTypeCounts.ContainsKey(bType), chartTypeCounts(bType), 0)
                         Dim cmp As Integer = aCount.CompareTo(bCount)
                         If cmp <> 0 Then Return cmp
                         cmp = ChartRotationDistance(aType, nextChartTypeIndex).CompareTo(ChartRotationDistance(bType, nextChartTypeIndex))
                         If cmp <> 0 Then Return cmp
                         Return String.Compare(aType, bType, StringComparison.OrdinalIgnoreCase)
                     End Function)

        Return indices(0)
    End Function

    Private Function DashboardChartTypeOrder() As List(Of String)
        Return New List(Of String)(New String() {"AreaChart", "SteppedAreaChart", "ColumnChart", "LineChart", "PieChart", "ScatterChart", "Histogram"})
    End Function

    Private Function ChartRotationDistance(chartType As String, nextChartTypeIndex As Integer) As Integer
        Dim chartTypes As List(Of String) = DashboardChartTypeOrder()
        Dim chartIndex As Integer = chartTypes.FindIndex(Function(item) item.Equals(chartType, StringComparison.OrdinalIgnoreCase))
        If chartIndex < 0 Then Return chartTypes.Count + 1
        If nextChartTypeIndex < 0 OrElse nextChartTypeIndex >= chartTypes.Count Then nextChartTypeIndex = 0
        If chartIndex >= nextChartTypeIndex Then Return chartIndex - nextChartTypeIndex
        Return chartTypes.Count - nextChartTypeIndex + chartIndex
    End Function

    Private Function NextDashboardChartTypeIndex(chartType As String) As Integer
        Dim chartTypes As List(Of String) = DashboardChartTypeOrder()
        Dim chartIndex As Integer = chartTypes.FindIndex(Function(item) item.Equals(chartType, StringComparison.OrdinalIgnoreCase))
        If chartIndex < 0 Then Return 0
        Return (chartIndex + 1) Mod chartTypes.Count
    End Function

    Private Function SupportsMultipleYValues(chartType As String) As Boolean
        Select Case chartType.Trim()
            Case "LineChart", "AreaChart", "SteppedAreaChart", "ColumnChart", "ScatterChart"
                Return True
        End Select
        Return False
    End Function

    Private Function SelectTopRecommendations(dt As DataTable, maxRows As Integer) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return dt

        Dim highestRows As New List(Of DataRow)()
        Dim otherRows As DataTable = dt.Clone()
        Dim fallbackRow As DataRow = Nothing
        For Each row As DataRow In dt.Rows
            If row("Chart Type").ToString().Trim() = "" Then
                fallbackRow = row
            ElseIf row("Priority").ToString().Trim().Equals("Highest", StringComparison.OrdinalIgnoreCase) Then
                highestRows.Add(row)
            Else
                otherRows.ImportRow(row)
            End If
        Next

        highestRows.Sort(Function(a, b)
                             Dim cmp As Integer = ChartRotationDistance(a("Chart Type").ToString().Trim(), 0).CompareTo(ChartRotationDistance(b("Chart Type").ToString().Trim(), 0))
                             If cmp <> 0 Then Return cmp
                             Return String.Compare(a("Fields").ToString().Trim(), b("Fields").ToString().Trim(), StringComparison.OrdinalIgnoreCase)
                         End Function)

        If highestRows.Count >= maxRows Then
            Dim highestOnly As DataTable = dt.Clone()
            For Each row As DataRow In highestRows
                highestOnly.ImportRow(row)
            Next
            If fallbackRow IsNot Nothing Then highestOnly.ImportRow(fallbackRow)
            Return highestOnly
        End If

        Dim multiTarget As Integer = CInt(Math.Round(maxRows * 0.8))
        Dim singleTarget As Integer = maxRows - multiTarget

        Dim multiPool As New List(Of DataRow)()
        Dim singlePool As New List(Of DataRow)()

        For Each row As DataRow In otherRows.Rows
            Dim chartType As String = row("Chart Type").ToString().Trim()
            Dim axisX As String = row("Axis X").ToString().Trim()
            Dim axisY As String = row("Axis Y").ToString().Trim()
            If axisY.Contains(",") Then
                multiPool.Add(row)
            Else
                singlePool.Add(row)
            End If
        Next

        multiPool.Sort(Function(a, b) CInt(b("Score")).CompareTo(CInt(a("Score"))))
        singlePool.Sort(Function(a, b) CInt(b("Score")).CompareTo(CInt(a("Score"))))

        Dim remainingSlots As Integer = maxRows - highestRows.Count
        Dim adjustedMultiTarget As Integer = Math.Min(multiTarget, remainingSlots)
        Dim multiSelected As List(Of DataRow) = SelectDiverseRows(multiPool, adjustedMultiTarget)
        Dim remainingAfterMulti As Integer = remainingSlots - multiSelected.Count
        Dim singleSelected As List(Of DataRow) = SelectDiverseRows(singlePool, remainingAfterMulti)

        Dim result As DataTable = dt.Clone()
        For Each row As DataRow In highestRows
            result.ImportRow(row)
        Next
        For Each row As DataRow In multiSelected
            result.ImportRow(row)
        Next
        For Each row As DataRow In singleSelected
            result.ImportRow(row)
        Next
        If fallbackRow IsNot Nothing Then result.ImportRow(fallbackRow)
        Return result
    End Function

    Private Function SelectDiverseRows(pool As List(Of DataRow), maxCount As Integer) As List(Of DataRow)
        If pool.Count = 0 OrElse maxCount = 0 Then Return New List(Of DataRow)()

        Dim selected As New List(Of DataRow)()
        Dim selectedSet As New HashSet(Of DataRow)()
        Dim seenCats As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim seenVals As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        Dim byType As New Dictionary(Of String, List(Of DataRow))(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In pool
            Dim ct As String = row("Chart Type").ToString().Trim()
            If Not byType.ContainsKey(ct) Then byType(ct) = New List(Of DataRow)()
            byType(ct).Add(row)
        Next

        ' Pass 1: one row per chart type, preferring unseen category and value sets
        For Each ct As String In byType.Keys
            If selected.Count >= maxCount Then Exit For
            Dim best As DataRow = Nothing
            For Each row As DataRow In byType(ct)
                Dim axX As String = row("Axis X").ToString().Trim()
                Dim axY As String = row("Axis Y").ToString().Trim()
                If Not seenCats.Contains(axX) AndAlso Not seenVals.Contains(axY) Then
                    best = row
                    Exit For
                End If
            Next
            If best Is Nothing Then
                For Each row As DataRow In byType(ct)
                    Dim axX As String = row("Axis X").ToString().Trim()
                    Dim axY As String = row("Axis Y").ToString().Trim()
                    If Not seenCats.Contains(axX) OrElse Not seenVals.Contains(axY) Then
                        best = row
                        Exit For
                    End If
                Next
            End If
            If best Is Nothing Then best = byType(ct)(0)
            selected.Add(best)
            selectedSet.Add(best)
            seenCats.Add(best("Axis X").ToString().Trim())
            seenVals.Add(best("Axis Y").ToString().Trim())
        Next

        ' Pass 2: rows with both unseen category set and unseen value set
        For Each row As DataRow In pool
            If selected.Count >= maxCount Then Exit For
            If selectedSet.Contains(row) Then Continue For
            Dim axX As String = row("Axis X").ToString().Trim()
            Dim axY As String = row("Axis Y").ToString().Trim()
            If Not seenCats.Contains(axX) AndAlso Not seenVals.Contains(axY) Then
                selected.Add(row)
                selectedSet.Add(row)
                seenCats.Add(axX)
                seenVals.Add(axY)
            End If
        Next

        ' Pass 3: rows with unseen category set or unseen value set
        For Each row As DataRow In pool
            If selected.Count >= maxCount Then Exit For
            If selectedSet.Contains(row) Then Continue For
            Dim axX As String = row("Axis X").ToString().Trim()
            Dim axY As String = row("Axis Y").ToString().Trim()
            If Not seenCats.Contains(axX) OrElse Not seenVals.Contains(axY) Then
                selected.Add(row)
                selectedSet.Add(row)
                seenCats.Add(axX)
                seenVals.Add(axY)
            End If
        Next

        ' Pass 4: fill remaining slots by score
        For Each row As DataRow In pool
            If selected.Count >= maxCount Then Exit For
            If Not selectedSet.Contains(row) Then
                selected.Add(row)
                selectedSet.Add(row)
            End If
        Next

        Return selected
    End Function

    Private Function SortRecommendations(dt As DataTable) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return dt
        Dim sorted As DataTable = dt.Clone()

        ' Build a list of rows with sortable priority
        Dim rowList As New List(Of DataRow)()
        For Each row As DataRow In dt.Rows
            If row("Axis Y").ToString().Trim() <> "" Then rowList.Add(row)
        Next

        ' Sort by priority rank, then Fields, then Recommended Chart
        rowList.Sort(Function(a, b)
                         Dim cmp As Integer = PriorityRank(a("Priority").ToString().Trim()).CompareTo(PriorityRank(b("Priority").ToString().Trim()))
                         If cmp <> 0 Then Return cmp
                         cmp = String.Compare(a("Fields").ToString().Trim(), b("Fields").ToString().Trim(), StringComparison.OrdinalIgnoreCase)
                         If cmp <> 0 Then Return cmp
                         Return String.Compare(a("Recommended Chart").ToString().Trim(), b("Recommended Chart").ToString().Trim(), StringComparison.OrdinalIgnoreCase)
                     End Function)

        For Each row As DataRow In rowList
            sorted.ImportRow(row)
        Next
        ' Append fallback rows (empty Axis Y)
        For Each row As DataRow In dt.Rows
            If row("Axis Y").ToString().Trim() = "" Then sorted.ImportRow(row)
        Next
        Return sorted
    End Function

    Private Function PriorityRank(priority As String) As Integer
        Select Case priority
            Case "Highest" : Return 0
            Case "High" : Return 1
            Case "Medium" : Return 2
            Case "Low" : Return 3
            Case Else : Return 4
        End Select
    End Function

    Private Function ApplySearchFilter(dt As DataTable, searchText As String) As DataTable
        If dt Is Nothing OrElse dt.Rows.Count = 0 OrElse searchText = "" Then Return dt

        Dim toRemove As New List(Of DataRow)()
        For Each row As DataRow In dt.Rows
            Dim matched As Boolean = False
            For Each col As DataColumn In dt.Columns
                If row(col).ToString().IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    matched = True
                    Exit For
                End If
            Next
            If Not matched Then toRemove.Add(row)
        Next

        For Each row As DataRow In toRemove
            dt.Rows.Remove(row)
        Next
        dt.AcceptChanges()
        Return dt
    End Function

    Private Function ReportSessionKey(name As String) As String
        Dim reportId As String = ""
        If Session("REPORTID") IsNot Nothing Then reportId = Session("REPORTID").ToString().Trim()
        Return "ChartRecommendation_" & name & "_" & reportId
    End Function

    Private Sub RestoreSelections()
        Dim categoryFields As String = SessionValue(ReportSessionKey("CategoryFields"))
        Dim valueFields As String = SessionValue(ReportSessionKey("ValueFields"))
        Dim dateField As String = SessionValue(ReportSessionKey("DateField"))

        If categoryFields.Trim() <> "" Then DropDownCategoryField.Text = categoryFields
        If valueFields.Trim() <> "" Then DropDownValueField.Text = valueFields
        If dateField.Trim() <> "" AndAlso DropDownDateField.Items.FindByValue(dateField) IsNot Nothing Then DropDownDateField.SelectedValue = dateField
    End Sub

    Private Sub SaveSelections(categoryFields As String, valueFields As String)
        Session(ReportSessionKey("CategoryFields")) = categoryFields
        Session(ReportSessionKey("ValueFields")) = valueFields
        Session(ReportSessionKey("DateField")) = DropDownDateField.SelectedValue.Trim()
    End Sub

    Private Function SessionValue(key As String) As String
        If Session(key) Is Nothing Then Return ""
        Return Session(key).ToString()
    End Function

    Private Function SelectedFieldCount(fields As String) As Integer
        If fields Is Nothing OrElse fields.Trim() = "" Then Return 0

        Dim count As Integer = 0
        For Each part As String In fields.Split(","c)
            If part.Trim() <> "" Then count += 1
        Next
        Return count
    End Function

    Private Function AllowedCategoryFields(dt As DataTable, selectedFields As String) As List(Of String)
        Dim fields As List(Of String) = SplitFieldList(selectedFields)
        If fields.Count > 0 Then Return fields

        Dim allFields As New List(Of String)()
        If dt Is Nothing Then Return allFields
        For Each col As DataColumn In dt.Columns
            If Not IsNumericAnalysisField(dt, col.ColumnName) Then allFields.Add(col.ColumnName)
        Next
        Return allFields
    End Function

    Private Function AllowedValueFields(dt As DataTable, selectedFields As String) As List(Of String)
        Dim fields As New List(Of String)()
        If dt Is Nothing Then Return fields

        For Each fieldName As String In SplitFieldList(selectedFields)
            If IsNumericAnalysisField(dt, fieldName) AndAlso Not IsIndexFieldName(fieldName) Then fields.Add(fieldName)
        Next
        If fields.Count > 0 Then Return fields

        For Each col As DataColumn In dt.Columns
            If IsNumericAnalysisField(dt, col.ColumnName) AndAlso Not IsIndexFieldName(col.ColumnName) Then fields.Add(col.ColumnName)
        Next
        Return fields
    End Function

    Private Function AllowedDateFields(dt As DataTable, selectedField As String) As List(Of String)
        Dim fields As New List(Of String)()
        If dt Is Nothing Then Return fields
        If selectedField.Trim() <> "" AndAlso dt.Columns.Contains(selectedField.Trim()) Then
            fields.Add(selectedField.Trim())
            Return fields
        End If

        For Each col As DataColumn In dt.Columns
            If col.DataType Is GetType(DateTime) OrElse LooksLikeDate(dt, col.ColumnName) Then fields.Add(col.ColumnName)
        Next
        Return fields
    End Function

    Private Function SplitFieldList(fields As String) As List(Of String)
        Dim result As New List(Of String)()
        If fields Is Nothing Then Return result

        Dim normalizedFields As String = NormalizeSelectedFieldsText(fields)
        For Each part As String In normalizedFields.Split(","c)
            Dim fieldName As String = CleanSelectedFieldName(part)
            If fieldName <> "" AndAlso fieldName <> " " AndAlso Not fieldName.Equals("Please select...", StringComparison.OrdinalIgnoreCase) AndAlso Not result.Contains(fieldName) Then result.Add(fieldName)
        Next
        Return result
    End Function

    Private Function NormalizeSelectedFieldsText(fields As String) As String
        If fields Is Nothing Then Return ""
        Return fields.Replace("&nbsp;", " ").Replace(vbCrLf, ",").Replace(vbCr, ",").Replace(vbLf, ",").Replace(";", ",").Replace("|", ",").Trim()
    End Function

    Private Function CleanSelectedFieldName(fieldName As String) As String
        If fieldName Is Nothing Then Return ""
        Return fieldName.Trim().Trim(","c).Trim()
    End Function

    Private Function JoinFields(fields As List(Of String)) As String
        If fields Is Nothing OrElse fields.Count = 0 Then Return ""
        Return String.Join(",", fields.ToArray())
    End Function

    Private Function LimitFieldList(fields As List(Of String), maxFields As Integer) As List(Of String)
        Dim limited As New List(Of String)()
        If fields Is Nothing OrElse maxFields <= 0 Then Return limited
        For Each fieldName As String In fields
            If limited.Count >= maxFields Then Exit For
            If fieldName IsNot Nothing AndAlso fieldName.Trim() <> "" AndAlso Not limited.Contains(fieldName) Then limited.Add(fieldName)
        Next
        Return limited
    End Function

    Private Function ValueFieldCombinations(fields As List(Of String)) As List(Of String)
        Dim combinations As New List(Of String)()
        If fields Is Nothing OrElse fields.Count < 2 Then Return combinations

        Dim maxCombinationSize As Integer = Math.Min(3, fields.Count)
        AddBalancedFieldCombinations(fields, 2, maxCombinationSize, MaxValueCombinations, combinations)
        Return combinations
    End Function

    Private Function CategoryFieldCombinations(fields As List(Of String), includeCompleteSet As Boolean) As List(Of String)
        Dim combinations As New List(Of String)()
        If fields Is Nothing OrElse fields.Count = 0 Then Return combinations

        Dim maxCombinationSize As Integer = Math.Min(2, fields.Count)
        AddBalancedFieldCombinations(fields, 1, maxCombinationSize, MaxCategoryCombinations, combinations)
        Return combinations
    End Function

    Private Function DistributedFieldSample(fields As List(Of String), maxFields As Integer) As List(Of String)
        If fields Is Nothing Then Return New List(Of String)()
        If maxFields <= 0 OrElse fields.Count <= maxFields Then Return New List(Of String)(fields)

        Dim sampled As New List(Of String)()
        Dim used As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To maxFields - 1
            Dim index As Integer = CInt(Math.Floor((i * fields.Count) / CDbl(maxFields)))
            If index >= fields.Count Then index = fields.Count - 1
            AddFieldIfNew(sampled, used, fields(index))
        Next

        Dim fillIndex As Integer = 0
        While sampled.Count < maxFields AndAlso fillIndex < fields.Count
            AddFieldIfNew(sampled, used, fields(fillIndex))
            fillIndex += 1
        End While
        Return sampled
    End Function

    Private Sub AddFieldIfNew(fields As List(Of String), used As HashSet(Of String), fieldName As String)
        If fieldName Is Nothing Then Exit Sub
        Dim cleanName As String = fieldName.Trim()
        If cleanName = "" OrElse used.Contains(cleanName) Then Exit Sub
        fields.Add(cleanName)
        used.Add(cleanName)
    End Sub

    Private Sub AddBalancedFieldCombinations(fields As List(Of String), minSize As Integer, maxSize As Integer, maxCombinations As Integer, combinations As List(Of String))
        If fields Is Nothing OrElse combinations Is Nothing OrElse fields.Count = 0 OrElse maxCombinations <= 0 Then Exit Sub
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For combinationSize As Integer = minSize To maxSize
            If combinationSize <= 1 Then
                For Each fieldName As String In fields
                    AddCombinationIfNew(New List(Of String)(New String() {fieldName}), combinations, seen, maxCombinations)
                    If combinations.Count >= maxCombinations Then Exit Sub
                Next
            Else
                For gap As Integer = 1 To fields.Count - 1
                    For startIndex As Integer = 0 To fields.Count - 1
                        Dim indexes As New List(Of Integer)()
                        For offset As Integer = 0 To combinationSize - 1
                            Dim index As Integer = (startIndex + (offset * gap)) Mod fields.Count
                            If indexes.Contains(index) Then Exit For
                            indexes.Add(index)
                        Next
                        If indexes.Count <> combinationSize Then Continue For
                        indexes.Sort()

                        Dim currentFields As New List(Of String)()
                        For Each index As Integer In indexes
                            currentFields.Add(fields(index))
                        Next
                        AddCombinationIfNew(currentFields, combinations, seen, maxCombinations)
                        If combinations.Count >= maxCombinations Then Exit Sub
                    Next
                Next
            End If
        Next
    End Sub

    Private Sub AddCombinationIfNew(fields As List(Of String), combinations As List(Of String), seen As HashSet(Of String), maxCombinations As Integer)
        If fields Is Nothing OrElse combinations Is Nothing OrElse seen Is Nothing OrElse combinations.Count >= maxCombinations Then Exit Sub
        Dim combination As String = JoinFields(fields)
        If combination.Trim() = "" OrElse seen.Contains(combination) Then Exit Sub
        combinations.Add(combination)
        seen.Add(combination)
    End Sub

    Private Sub AddFieldCombinations(fields As List(Of String), targetSize As Integer, startIndex As Integer, currentFields As List(Of String), combinations As List(Of String))
        If currentFields.Count = targetSize Then
            Dim combination As String = JoinFields(currentFields)
            If combination.Trim() <> "" AndAlso Not combinations.Contains(combination) Then combinations.Add(combination)
            Exit Sub
        End If

        For i As Integer = startIndex To fields.Count - 1
            currentFields.Add(fields(i))
            AddFieldCombinations(fields, targetSize, i + 1, currentFields, combinations)
            currentFields.RemoveAt(currentFields.Count - 1)
        Next
    End Sub

    Private Function CategoryText(categoryFields As String) As String
        Return categoryFields.Replace(",", " and ")
    End Function

    Private Sub AddRecommendation(dt As DataTable, chartName As String, priority As String, fields As String, reason As String, chartType As String, axisX As String, axisY As String)
        If dt Is Nothing OrElse dt.Rows.Count >= MaxRawRecommendationRecords Then Exit Sub

        ' Demote priority if any value field ends with "ID"
        Dim adjustedPriority As String = priority
        If HasIDField(axisY) Then adjustedPriority = "Low"

        Dim row As DataRow = dt.NewRow()
        row("Recommended Chart") = chartName
        row("Priority") = adjustedPriority
        row("Fields") = fields
        row("Reason") = reason
        row("Open Chart") = ChartUrl(chartType, axisX, axisY)
        row("Chart Type") = chartType
        row("Axis X") = axisX
        row("Axis Y") = axisY
        row("Score") = RecommendationScore(adjustedPriority, chartType, axisX, axisY)
        dt.Rows.Add(row)
    End Sub

    Private Function HasIDField(axisFields As String) As Boolean
        If axisFields Is Nothing OrElse axisFields.Trim() = "" Then Return False
        For Each fieldName As String In axisFields.Split(","c)
            If IsIndexFieldName(fieldName.Trim()) Then Return True
        Next
        Return False
    End Function

    Private Function ContainsIndexField(fields As String) As Boolean
        If fields Is Nothing OrElse fields.Trim() = "" Then Return False
        For Each fieldName As String In fields.Split(","c)
            If IsIndexFieldName(fieldName.Trim()) Then Return True
        Next
        Return False
    End Function

    Private Function RecommendationScore(priority As String, chartType As String, axisX As String, axisY As String) As Integer
        Dim score As Integer = 0

        ' Base score from priority
        Select Case priority
            Case "Highest" : score = 50
            Case "High" : score = 30
            Case "Medium" : score = 20
            Case "Low" : score = 10
            Case Else : score = 0
        End Select

        ' Bonus for most analytically useful chart types
        Select Case chartType
            Case "PieChart" : score += 15
            Case "LineChart" : score += 10
            Case "ScatterChart" : score += 9
            Case "BarChart" : score += 8
            Case "ComboChart" : score += 6
            Case "AreaChart" : score += 4
            Case "SteppedAreaChart" : score += 3
            Case "Histogram" : score += 4
            Case "BubbleChart" : score += 3
            Case "Sankey" : score += 2
            Case "Gauge" : score += 2
        End Select

        ' Bonus for multi-category multi-value charts (highest priority)
        Dim xFieldCount As Integer = SelectedFieldCount(axisX)
        Dim yFieldCount As Integer = SelectedFieldCount(axisY)
        If xFieldCount > 1 AndAlso yFieldCount > 1 Then
            score += 15
        ElseIf xFieldCount = 1 AndAlso yFieldCount = 1 Then
            score += 5
        ElseIf xFieldCount <= 2 AndAlso yFieldCount <= 2 Then
            score += 2
        End If

        Return score
    End Function

    Private Function ChartUrl(chartType As String, xAxisFields As String, yAxisFields As String) As String
        If chartType.Trim() = "" Then Return "ShowReport.aspx?srd=3"

        Dim reportId As String = ""
        If Session("REPORTID") IsNot Nothing Then reportId = Session("REPORTID").ToString()

        Return ChartRedirectUrl(reportId, xAxisFields, yAxisFields, chartType)
    End Function

    Private Function SelectedCategoryFields() As String
        Dim selectedText As String = DropDownCategoryField.SelectedItemsString
        If selectedText Is Nothing OrElse selectedText.Trim() = "" Then selectedText = DropDownCategoryField.Text
        If selectedText Is Nothing Then Return ""

        selectedText = NormalizeSelectedFieldsText(selectedText)
        If selectedText = "" OrElse selectedText.Equals("Please select...", StringComparison.OrdinalIgnoreCase) OrElse selectedText = " " Then Return ""

        Dim cleaned As New List(Of String)()
        For Each part As String In selectedText.Split(","c)
            Dim fieldName As String = CleanSelectedFieldName(part)
            If fieldName <> "" AndAlso fieldName <> " " AndAlso Not fieldName.Equals("Please select...", StringComparison.OrdinalIgnoreCase) AndAlso Not cleaned.Contains(fieldName) Then cleaned.Add(fieldName)
        Next
        Return String.Join(",", cleaned.ToArray())
    End Function

    Private Function SelectedValueFields(dt As DataTable) As String
        Dim selectedText As String = DropDownValueField.SelectedItemsString
        If selectedText Is Nothing OrElse selectedText.Trim() = "" Then selectedText = DropDownValueField.Text
        If selectedText Is Nothing Then Return ""

        selectedText = NormalizeSelectedFieldsText(selectedText)
        If selectedText = "" OrElse selectedText.Equals("Please select...", StringComparison.OrdinalIgnoreCase) OrElse selectedText = " " Then Return ""

        Dim cleaned As New List(Of String)()
        For Each part As String In selectedText.Split(","c)
            Dim fieldName As String = CleanSelectedFieldName(part)
            If fieldName <> "" AndAlso fieldName <> " " AndAlso Not fieldName.Equals("Please select...", StringComparison.OrdinalIgnoreCase) AndAlso IsNumericAnalysisField(dt, fieldName) AndAlso Not cleaned.Contains(fieldName) Then cleaned.Add(fieldName)
        Next
        Return String.Join(",", cleaned.ToArray())
    End Function

    Private Function FirstSelectedField(fields As String) As String
        Dim parts() As String = fields.Split(","c)
        If parts.Length = 0 Then Return ""
        Return parts(0).Trim()
    End Function

    Private Function SecondSelectedField(fields As String) As String
        Dim parts() As String = fields.Split(","c)
        If parts.Length > 1 Then Return parts(1).Trim()
        If parts.Length = 1 Then Return parts(0).Trim()
        Return ""
    End Function

    Private Sub BindRecommendations(dt As DataTable, Optional totalPossible As Integer = -1)
        If dt IsNot Nothing Then
            Dim pageCount As Integer = RecommendationPageCount(dt)
            If GridViewRecommendations.PageIndex >= pageCount Then GridViewRecommendations.PageIndex = pageCount - 1
            If GridViewRecommendations.PageIndex < 0 Then GridViewRecommendations.PageIndex = 0
        End If
        GridViewRecommendations.DataSource = dt
        GridViewRecommendations.DataBind()
        If dt Is Nothing Then
            LabelInfo.Text = ""
            LabelRecordCount.Text = "Number of records: 0"
        Else
            If totalPossible > 0 AndAlso dt.Rows.Count < totalPossible Then
                LabelInfo.Text = dt.Rows.Count.ToString() & " best chart recommendations from " & totalPossible.ToString() & " possible"
                If dt.Rows.Count = MaxRecommendedRecords Then LabelInfo.Text &= " (limited to 1000)"
            Else
                LabelInfo.Text = "Chart recommendations (" & dt.Rows.Count.ToString() & " rows)"
            End If
            LabelRecordCount.Text = "Number of records: " & dt.Rows.Count.ToString()
        End If
        UpdateRecommendationPager(dt)
    End Sub

    Private Function RecommendationPageCount(dt As DataTable) As Integer
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then Return 1
        Dim pageCount As Integer = CInt(Math.Ceiling(dt.Rows.Count / CDbl(GridViewRecommendations.PageSize)))
        If pageCount < 1 Then pageCount = 1
        Return pageCount
    End Function

    Private Sub UpdateRecommendationPager(dt As DataTable)
        Dim pageCount As Integer = RecommendationPageCount(dt)
        Dim pageNumber As Integer = GridViewRecommendations.PageIndex + 1
        If pageNumber < 1 Then pageNumber = 1
        If pageNumber > pageCount Then pageNumber = pageCount

        TextBoxPageNumber.Text = pageNumber.ToString()
        LabelPageCount.Text = " of " & pageCount.ToString()
        LinkButtonPrevious.Enabled = pageNumber > 1
        LinkButtonPrevious.Visible = pageCount > 1 AndAlso pageNumber > 1
        LinkButtonNext.Enabled = pageNumber < pageCount
        LinkButtonNext.Visible = pageCount > 1 AndAlso pageNumber < pageCount
        LabelPageNumberCaption.Visible = pageCount > 1
        TextBoxPageNumber.Visible = pageCount > 1
        LabelPageCount.Visible = pageCount > 1
    End Sub

    Private Sub BuildRecommendedDashboard()
        LabelError.Text = ""
        HyperLinkDashboard.Visible = False

        Dim dt As DataTable = CurrentRecommendationGridTable()
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRecommendations()
            dt = CurrentRecommendationGridTable()
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No chart recommendations are available."
            Exit Sub
        End If

        Dim reportId As String = ""
        If Session("REPORTID") IsNot Nothing Then reportId = Session("REPORTID").ToString().Trim()
        If reportId = "" Then
            LabelError.Text = "Report is not selected."
            Exit Sub
        End If

        Dim dashboardName As String = NewDashboardName()
        Dim added As Integer = 0
        Dim skipped As Integer = 0
        Dim selectedRows As Dictionary(Of Integer, Boolean) = SelectedDashboardRows()
        If selectedRows.Count = 0 Then
            ' No checkboxes selected - only include Highest priority charts from the current page.
            For Each rowIndex As Integer In CurrentPageDataIndexes(dt)
                Dim chartType As String = dt.Rows(rowIndex)("Chart Type").ToString().Trim()
                Dim priority As String = dt.Rows(rowIndex)("Priority").ToString().Trim()
                If IsDashboardSafeChartType(chartType) AndAlso priority = "Highest" Then selectedRows.Add(rowIndex, True)
            Next
        End If
        If selectedRows.Count = 0 Then
            LabelError.Text = "No dashboard-safe chart recommendations are available."
            Exit Sub
        End If

        For Each rowIndex As Integer In CurrentPageDataIndexes(dt)
            If Not selectedRows.ContainsKey(rowIndex) Then Continue For

            Dim row As DataRow = dt.Rows(rowIndex)

            Dim chartType As String = row("Chart Type").ToString().Trim()
            If chartType = "" Then
                skipped += 1
                Continue For
            End If
            If Not IsDashboardSafeChartType(chartType) Then
                skipped += 1
                Continue For
            End If

            Dim axisX As String = row("Axis X").ToString().Trim()
            Dim axisY As String = row("Axis Y").ToString().Trim()
            If axisX = "" OrElse axisY = "" Then
                skipped += 1
                Continue For
            End If

            Dim dashboardRowIndx As Integer = 0
            Dim ret As String = AddRecommendedChartToDashboard(dashboardName, reportId, chartType, axisX, axisY, row("Recommended Chart").ToString(), dashboardRowIndx)
            If ret.StartsWith("ERROR!!", StringComparison.OrdinalIgnoreCase) Then
                LabelError.Text = ret
                Exit Sub
            End If

            Dim updateError As String = ""
            Dim chartCount As Integer = 0
            UpdateDashboard(dashboardName, chartCount, Session("UserConnString"), Session("UserConnProvider"), updateError)

            If updateError.Trim() <> "" OrElse dashboardRowIndx <= 0 OrElse Not DashboardRowHasData(dashboardRowIndx) Then
                If dashboardRowIndx > 0 Then DeleteDashboardRow(dashboardRowIndx)
                skipped += 1
                Continue For
            End If

            added += 1
        Next

        If added = 0 Then
            LabelError.Text = "No chart recommendations could be added to a dashboard."
            Exit Sub
        End If

        Dim finalUpdateError As String = ""
        Dim finalChartCount As Integer = 0
        UpdateDashboard(dashboardName, finalChartCount, Session("UserConnString"), Session("UserConnProvider"), finalUpdateError)
        If finalUpdateError.Trim() <> "" Then LabelError.Text = finalUpdateError

        LabelInfo.Text = "Dashboard '" & dashboardName & "' created with " & added.ToString() & " recommended charts"
        Dim totalPossible As Integer = 0
        If Session("ChartRecommendationTotalPossible") IsNot Nothing Then totalPossible = CInt(Session("ChartRecommendationTotalPossible"))
        If totalPossible > added Then LabelInfo.Text &= " from " & totalPossible.ToString() & " possible"
        LabelInfo.Text &= "."
        If skipped > 0 Then LabelInfo.Text &= " " & skipped.ToString() & " recommendations were skipped because they had no dashboard chart data."
        HyperLinkDashboard.NavigateUrl = "~/Dashboard.aspx?user=" & Server.UrlEncode(FieldText(Session("logon"))) & "&dashboard=" & Server.UrlEncode(dashboardName)
        HyperLinkDashboard.Text = "Open dashboard"
        HyperLinkDashboard.Visible = True
    End Sub

    Private Function AddRecommendedChartToDashboard(dashboardName As String, reportId As String, chartType As String, axisX As String, axisY As String, recommendationName As String, ByRef dashboardRowIndx As Integer) As String
        Try
            dashboardRowIndx = 0
            Dim x1 As String = FirstSelectedField(axisX)
            Dim x2 As String = SecondSelectedField(axisX)
            Dim y1 As String = FirstSelectedField(axisY)
            Dim isMultiChart As Boolean = axisX.IndexOf(","c) >= 0 OrElse axisY.IndexOf(","c) >= 0
            Dim yAll As String = If(isMultiChart, axisY, "")
            Dim xAll As String = If(isMultiChart, axisX, "")
            Dim whereText As String = ""
            If Session("WhereStm") IsNot Nothing Then whereText = Session("WhereStm").ToString().Trim()

            Dim title As String = DashboardChartTitle(recommendationName, chartType, axisX, axisY)
            Dim fieldList As String = "UserID,Dashboard,ReportID,ChartType,MapName,x1,x2,y1,fn1,WhereStm,GraphTitle,MapYesNo,Prop3,y2"
            Dim values As String = SqlValue(FieldText(Session("logon"))) &
                "," & SqlValue(dashboardName) &
                "," & SqlValue(reportId) &
                "," & SqlValue(chartType) &
                "," & SqlValue("") &
                "," & SqlValue(x1) &
                "," & SqlValue(x2) &
                "," & SqlValue(y1) &
                "," & SqlValue("Sum") &
                "," & SqlValue(whereText.Replace("'", "^")) &
                "," & SqlValue(TruncateText(title, 200)) &
                "," & SqlValue("no") &
                "," & SqlValue(xAll) &
                "," & SqlValue(yAll)

            Dim sqlInsert As String = "INSERT INTO ourdashboards (" & fieldList & ") VALUES (" & values & ")"
            Dim ret As String = ExequteSQLquery(sqlInsert)
            If ret <> "Query executed fine." Then Return "ERROR!! " & ret
            dashboardRowIndx = LastDashboardRowIndx(dashboardName, FieldText(Session("logon")))
            Return ret
        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try
    End Function

    Private Function LastDashboardRowIndx(dashboardName As String, userId As String) As Integer
        Dim lastIndx As Integer = 0
        Try
            Dim dv As DataView = mRecords("SELECT * FROM ourdashboards WHERE UserID=" & SqlValue(userId) & " AND Dashboard=" & SqlValue(dashboardName))
            If dv Is Nothing OrElse dv.Table Is Nothing Then Return 0
            For Each row As DataRow In dv.Table.Rows
                If row.Table.Columns.Contains("Indx") AndAlso IsNumeric(row("Indx").ToString()) Then
                    lastIndx = Math.Max(lastIndx, CInt(row("Indx")))
                End If
            Next
        Catch
            Return 0
        End Try
        Return lastIndx
    End Function

    Private Function DashboardRowHasData(dashboardRowIndx As Integer) As Boolean
        Try
            Dim dv As DataView = mRecords("SELECT * FROM ourdashboards WHERE Indx=" & dashboardRowIndx.ToString())
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then Return False
            Dim row As DataRow = dv.Table.Rows(0)
            Dim prop1 As String = ""
            Dim arr As String = ""
            If row.Table.Columns.Contains("Prop1") Then prop1 = row("Prop1").ToString().Trim()
            If row.Table.Columns.Contains("ARR") Then arr = row("ARR").ToString().Trim()
            If prop1 <> "1" OrElse arr = "" Then Return False

            Dim normalizedArr As String = arr.Replace("**", "[").Replace("##", "]")
            Dim dataRows As Integer = 0
            For Each ch As Char In normalizedArr
                If ch = "["c Then dataRows += 1
            Next
            If dataRows < 2 Then Return False

            Dim firstRowEnd As Integer = normalizedArr.IndexOf("]"c)
            If firstRowEnd < 0 OrElse firstRowEnd >= normalizedArr.Length - 1 Then Return False
            Dim dataPart As String = normalizedArr.Substring(firstRowEnd + 1)
            Return Regex.IsMatch(dataPart, ",\s*-?(\d+(\.\d*)?|\.\d+)([eE][+-]?\d+)?\s*(,|\])")
        Catch
            Return False
        End Try
    End Function

    Private Function IsDashboardSafeChartType(chartType As String) As Boolean
        Select Case chartType.Trim()
            Case "LineChart", "AreaChart", "SteppedAreaChart", "ColumnChart", "BarChart", "PieChart", "Histogram", "ScatterChart", "ComboChart"
                Return True
        End Select
        Return False
    End Function

    Private Sub DeleteDashboardRow(dashboardRowIndx As Integer)
        If dashboardRowIndx <= 0 Then Exit Sub
        ExequteSQLquery("DELETE FROM ourdashboards WHERE Indx=" & dashboardRowIndx.ToString())
    End Sub

    Private Function CurrentRecommendationGridTable() As DataTable
        Dim dt As DataTable = TryCast(Session("ChartRecommendationFiltered"), DataTable)
        If dt Is Nothing Then dt = TryCast(Session("ChartRecommendationTable"), DataTable)
        Return dt
    End Function

    Private Function CurrentPageDataIndexes(dt As DataTable) As List(Of Integer)
        Dim indexes As New List(Of Integer)()
        If dt Is Nothing Then Return indexes
        Dim startIndex As Integer = GridViewRecommendations.PageIndex * GridViewRecommendations.PageSize
        Dim endIndex As Integer = Math.Min(startIndex + GridViewRecommendations.PageSize, dt.Rows.Count)
        For rowIndex As Integer = startIndex To endIndex - 1
            indexes.Add(rowIndex)
        Next
        Return indexes
    End Function

    Private Function SelectedDashboardRows() As Dictionary(Of Integer, Boolean)
        Dim selectedRows As New Dictionary(Of Integer, Boolean)()
        For Each gridRow As GridViewRow In GridViewRecommendations.Rows
            If gridRow.RowType <> DataControlRowType.DataRow Then Continue For
            Dim dashboardCheckBox As CheckBox = TryCast(gridRow.FindControl("chkAddToDashboard"), CheckBox)
            If dashboardCheckBox Is Nothing Then Continue For
            Dim dataRowIndex As Integer = RecommendationDataIndex(gridRow.RowIndex)
            If Request.Form(dashboardCheckBox.UniqueID) IsNot Nothing AndAlso Not selectedRows.ContainsKey(dataRowIndex) Then
                selectedRows.Add(dataRowIndex, True)
            End If
        Next
        Return selectedRows
    End Function

    Private Function NewDashboardName() As String
        Dim baseName As String = "Recommended Charts"
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then
            baseName &= " - " & Session("REPTITLE").ToString().Trim()
        End If
        baseName = TruncateText(baseName, 190)
        Return baseName & " - " & DateTime.Now.ToString("yyyyMMddHHmmss")
    End Function

    Private Function DashboardChartTitle(recommendationName As String, chartType As String, axisX As String, axisY As String) As String
        Dim title As String = recommendationName
        If title.Trim() = "" Then title = chartType
        Return title & ": Sum of [" & axisY & "] by [" & axisX & "]"
    End Function

    Private Function SqlValue(valueText As String) As String
        If valueText Is Nothing Then valueText = ""
        Return "'" & valueText.Replace("'", "''") & "'"
    End Function

    Private Function TruncateText(valueText As String, maxLength As Integer) As String
        If valueText Is Nothing Then Return ""
        If valueText.Length <= maxLength Then Return valueText
        If maxLength <= 3 Then Return valueText.Substring(0, maxLength)
        Return valueText.Substring(0, maxLength - 3) & "..."
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Sub ExportRecommendations(formatName As String)
        ' Use filtered data if search was applied, otherwise full data
        Dim dt As DataTable = TryCast(Session("ChartRecommendationFiltered"), DataTable)
        If dt Is Nothing Then dt = TryCast(Session("ChartRecommendationTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRecommendations()
            dt = TryCast(Session("ChartRecommendationTable"), DataTable)
        End If
        Dim fileName As String = "ChartRecommendations_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Chart Recommendations", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Chart Recommendations", ""))
        End If
        Response.End()
    End Sub
End Class
