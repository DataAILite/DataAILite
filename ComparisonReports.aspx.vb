Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.FileIO

Partial Class ComparisonReports
    Inherits System.Web.UI.Page

    Private Const AllRowsValue As String = "(All)"
    Private Const ComparisonSourceColumn As String = "Comparison Source"

    Private Class ComparisonBucket
        Public Count As Integer
        Public Sum As Double
        Public Minimum As Double
        Public Maximum As Double
        Public HasNumeric As Boolean
        Public DistinctValues As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        Public Rows As New List(Of DataRow)()

        Public Sub AddValue(valueObject As Object)
            Count += 1

            Dim valueText As String = String.Empty
            If valueObject IsNot Nothing AndAlso Not IsDBNull(valueObject) Then valueText = valueObject.ToString()
            If Not DistinctValues.ContainsKey(valueText) Then DistinctValues.Add(valueText, True)

            Dim numericValue As Double
            If Double.TryParse(valueText, numericValue) Then
                Sum += numericValue
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

        Public Sub AddRecord(row As DataRow, valueObject As Object)
            Rows.Add(row)
            AddValue(valueObject)
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

    Private Sub GridViewComparison_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewComparison.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow OrElse e.Row.Cells.Count = 0 Then Exit Sub

        Dim dt As DataTable = TryCast(Session("ComparisonReportsTable"), DataTable)
        If dt Is Nothing Then Exit Sub

        Dim baseRecordsIndex As Integer = dt.Columns.IndexOf("Base Records")
        Dim compareRecordsIndex As Integer = dt.Columns.IndexOf("Compare Records")
        Dim baseFilterIndex As Integer = dt.Columns.IndexOf("BaseFilterId")
        Dim compareFilterIndex As Integer = dt.Columns.IndexOf("CompareFilterId")

        If baseFilterIndex >= 0 AndAlso baseFilterIndex < e.Row.Cells.Count Then e.Row.Cells(baseFilterIndex).Visible = False
        If compareFilterIndex >= 0 AndAlso compareFilterIndex < e.Row.Cells.Count Then e.Row.Cells(compareFilterIndex).Visible = False

        AddComparisonRecordLink(e.Row, baseRecordsIndex, baseFilterIndex)
        AddComparisonRecordLink(e.Row, compareRecordsIndex, compareFilterIndex)
    End Sub

    Private Sub AddComparisonRecordLink(row As GridViewRow, recordsIndex As Integer, filterIndex As Integer)
        If recordsIndex < 0 OrElse filterIndex < 0 OrElse recordsIndex >= row.Cells.Count OrElse filterIndex >= row.Cells.Count Then Exit Sub

        Dim recordsText As String = row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
        Dim filterId As String = row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
        If recordsText = "" OrElse recordsText = "0" OrElse filterId = "" Then Exit Sub

        Dim link As New HyperLink()
        link.Text = recordsText
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&comparisonfilter=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding comparison records in Data Explorer."
        row.Cells(recordsIndex).Controls.Clear()
        row.Cells(recordsIndex).Controls.Add(link)
    End Sub

    Private Sub ComparisonReports_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            lblHeader.Text = Session("REPTITLE").ToString() & " - Comparison Reports"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Comparison Reports"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Comparison%20Reports"
    End Sub

    Private Sub ComparisonReports_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            FillFieldLists()
            UpdateModePanels()
            BuildAndBindComparison()
        ElseIf Session("ComparisonReportsTable") IsNot Nothing Then
            UpdateModePanels()
            BindComparison(CType(Session("ComparisonReportsTable"), DataTable))
        Else
            UpdateModePanels()
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
            Session("ComparisonReportsSource") = Nothing
            Return Nothing
        End If

        Session("ComparisonReportsSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("ComparisonReportsSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("ComparisonReportsSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub

        FillSelectableFields(dt, True)
        FillCompareValues()
    End Sub

    Private Sub FillSelectableFields(dt As DataTable, includeCompareField As Boolean)
        Dim previousRowField As String = ""
        Dim previousCompareField As String = ""
        Dim previousValueField As String = ""

        If DropDownRowField.Items.Count > 0 Then previousRowField = DropDownRowField.SelectedValue
        If DropDownCompareField.Items.Count > 0 Then previousCompareField = DropDownCompareField.SelectedValue
        If DropDownValueField.Items.Count > 0 Then previousValueField = DropDownValueField.SelectedValue

        DropDownRowField.Items.Clear()
        DropDownValueField.Items.Clear()
        If includeCompareField Then DropDownCompareField.Items.Clear()

        DropDownRowField.Items.Add(New ListItem(AllRowsValue, ""))
        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            DropDownRowField.Items.Add(New ListItem(fld, fld))
            If includeCompareField Then DropDownCompareField.Items.Add(New ListItem(fld, fld))
            DropDownValueField.Items.Add(New ListItem(fld, fld))
        Next

        SelectDropdownValue(DropDownRowField, previousRowField)
        If includeCompareField Then
            If Not SelectDropdownValue(DropDownCompareField, previousCompareField) AndAlso dt.Columns.Count > 1 Then DropDownCompareField.SelectedIndex = 1
        End If
        If Not SelectDropdownValue(DropDownValueField, previousValueField) Then SetDefaultValueField(dt)
        FillAggregates(dt)
    End Sub

    Private Function SelectDropdownValue(list As System.Web.UI.WebControls.DropDownList, valueText As String) As Boolean
        If valueText.Trim() = "" Then Return False
        For i As Integer = 0 To list.Items.Count - 1
            If list.Items(i).Value = valueText Then
                list.SelectedIndex = i
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub UpdateModePanels()
        Dim isTwoQueries As Boolean = DropDownComparisonType.SelectedValue = "Queries"
        Dim isTwoFiles As Boolean = DropDownComparisonType.SelectedValue = "ImportedFiles"
        trStandardCompare.Visible = Not isTwoQueries AndAlso Not isTwoFiles
        trQueryCompare.Visible = isTwoQueries
        trQueryCompare2.Visible = isTwoQueries
        trFileCompare.Visible = isTwoFiles
    End Sub

    Private Function IsExternalComparisonMode() As Boolean
        Return DropDownComparisonType.SelectedValue = "Queries" OrElse DropDownComparisonType.SelectedValue = "ImportedFiles"
    End Function

    Private Function GetActiveFieldSource() As DataTable
        If IsExternalComparisonMode() AndAlso Session("ComparisonReportsExternalSource") IsNot Nothing Then
            Return CType(Session("ComparisonReportsExternalSource"), DataTable)
        End If
        Return GetSourceTable()
    End Function

    Private Function IsNumericField(dt As DataTable, columnName As String) As Boolean
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then Return False
        If ColumnTypeIsNumeric(dt.Columns(columnName)) Then Return True

        For i As Integer = 0 To dt.Rows.Count - 1
            Dim valueText As String = FieldText(dt.Rows(i)(columnName)).Trim()
            If valueText <> "" Then
                Dim numericValue As Double
                If Double.TryParse(valueText, numericValue) Then Return True
            End If
        Next
        Return False
    End Function

    Private Sub SetDefaultValueField(dt As DataTable)
        For i As Integer = 0 To dt.Columns.Count - 1
            If IsNumericField(dt, dt.Columns(i).ColumnName) Then
                DropDownValueField.SelectedValue = dt.Columns(i).ColumnName
                Exit Sub
            End If
        Next
    End Sub

    Private Sub FillAggregates()
        FillAggregates(GetActiveFieldSource())
    End Sub

    Private Sub FillAggregates(dt As DataTable)
        Dim selectedAggregate As String = String.Empty
        If DropDownAggregate.Items.Count > 0 Then selectedAggregate = DropDownAggregate.SelectedValue

        DropDownAggregate.Items.Clear()
        DropDownAggregate.Items.Add("Count")
        DropDownAggregate.Items.Add("CountDistinct")

        Dim isNumericValue As Boolean = False
        If dt IsNot Nothing AndAlso DropDownValueField.SelectedValue.Trim() <> "" AndAlso dt.Columns.Contains(DropDownValueField.SelectedValue) Then
            isNumericValue = IsNumericField(dt, DropDownValueField.SelectedValue)
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
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub DropDownComparisonType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownComparisonType.SelectedIndexChanged
        UpdateModePanels()
        Session("ComparisonReportsExternalSource") = Nothing
        LabelError.Text = ""
        LabelInfo.Text = ""
    End Sub

    Private Sub DropDownCompareField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownCompareField.SelectedIndexChanged
        FillCompareValues()
        BuildAndBindComparison()
    End Sub

    Private Sub DropDownValueField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownValueField.SelectedIndexChanged
        FillAggregates()
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindComparison()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        txtBaseQuery.Text = ""
        txtCompareQuery.Text = ""
        Session("ComparisonReportsExternalSource") = Nothing
        BuildAndBindComparison()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportComparison("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportComparison("xls")
    End Sub

    Private Sub lnkComparisonAI_Click(sender As Object, e As EventArgs) Handles lnkComparisonAI.Click
        Dim dt As DataTable = TryCast(Session("ComparisonReportsTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindComparison()
            dt = TryCast(Session("ComparisonReportsTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No comparison data to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), ComparisonTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this comparison report. Summarize the differences between the base and compare values, largest variances, percentage changes, and any records or groups that should be reviewed.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Function GetComparisonSourceTable() As DataTable
        If DropDownComparisonType.SelectedValue = "Queries" Then Return BuildQueryComparisonSource()
        If DropDownComparisonType.SelectedValue = "ImportedFiles" Then Return BuildFileComparisonSource()
        Return GetSourceTable()
    End Function

    Private Function BuildQueryComparisonSource() As DataTable
        If txtBaseQuery.Text.Trim() = "" OrElse txtCompareQuery.Text.Trim() = "" Then
            LabelError.Text = "Enter both SQL queries for Two Queries comparison."
            Return Nothing
        End If

        Dim errText As String = ""
        Dim baseView As DataView = Nothing
        Dim compareView As DataView = Nothing

        Try
            baseView = mRecords(txtBaseQuery.Text.Trim(), errText, Session("UserConnString"), Session("UserConnProvider"))
            If errText.Trim() <> "" Then
                LabelError.Text = "Base query error: " & errText
                Return Nothing
            End If

            errText = ""
            compareView = mRecords(txtCompareQuery.Text.Trim(), errText, Session("UserConnString"), Session("UserConnProvider"))
            If errText.Trim() <> "" Then
                LabelError.Text = "Compare query error: " & errText
                Return Nothing
            End If
        Catch ex As Exception
            LabelError.Text = "Query comparison error: " & ex.Message
            Return Nothing
        End Try

        If baseView Is Nothing OrElse baseView.Table Is Nothing OrElse compareView Is Nothing OrElse compareView.Table Is Nothing Then
            LabelError.Text = "Both SQL queries must return data."
            Return Nothing
        End If

        Dim combined As DataTable = CombineComparisonTables(baseView.Table, compareView.Table)
        Session("ComparisonReportsExternalSource") = combined
        FillSelectableFields(combined, False)
        Return combined
    End Function

    Private Function BuildFileComparisonSource() As DataTable
        If Not FileUploadBase.HasFile OrElse Not FileUploadCompare.HasFile Then
            If Session("ComparisonReportsExternalSource") IsNot Nothing Then
                Dim savedTable As DataTable = CType(Session("ComparisonReportsExternalSource"), DataTable)
                FillSelectableFields(savedTable, False)
                Return savedTable
            End If

            LabelError.Text = "Select both files for Two Imported Files comparison."
            Return Nothing
        End If

        Try
            Dim baseTable As DataTable = ParseDelimitedFile(FileUploadBase)
            Dim compareTable As DataTable = ParseDelimitedFile(FileUploadCompare)
            Dim combined As DataTable = CombineComparisonTables(baseTable, compareTable)
            Session("ComparisonReportsExternalSource") = combined
            FillSelectableFields(combined, False)
            Return combined
        Catch ex As Exception
            LabelError.Text = "File comparison error: " & ex.Message
            Return Nothing
        End Try
    End Function

    Private Function ParseDelimitedFile(upload As System.Web.UI.WebControls.FileUpload) As DataTable
        Dim extensionText As String = Path.GetExtension(upload.FileName).ToLower()
        If extensionText <> ".csv" AndAlso extensionText <> ".txt" AndAlso extensionText <> ".tsv" Then
            Throw New ApplicationException("Use CSV, TSV, or TXT delimited files for direct comparison.")
        End If

        Dim dt As New DataTable()
        Using parser As New TextFieldParser(upload.PostedFile.InputStream, Encoding.Default)
            parser.TextFieldType = FieldType.Delimited
            parser.HasFieldsEnclosedInQuotes = True
            If extensionText = ".tsv" Then
                parser.SetDelimiters(vbTab)
            Else
                parser.SetDelimiters(",")
            End If

            If parser.EndOfData Then Return dt
            Dim headers() As String = parser.ReadFields()
            For i As Integer = 0 To headers.Length - 1
                Dim columnName As String = headers(i).Trim()
                If columnName = "" Then columnName = "Column" & (i + 1).ToString()
                dt.Columns.Add(UniqueTableColumnName(dt, columnName), GetType(String))
            Next

            While Not parser.EndOfData
                Dim fields() As String = parser.ReadFields()
                Dim row As DataRow = dt.NewRow()
                For i As Integer = 0 To dt.Columns.Count - 1
                    If fields IsNot Nothing AndAlso i < fields.Length Then row(i) = fields(i)
                Next
                dt.Rows.Add(row)
            End While
        End Using
        Return dt
    End Function

    Private Function UniqueTableColumnName(dt As DataTable, requestedName As String) As String
        Dim name As String = requestedName
        If name.Trim() = "" Then name = "Column"
        Dim baseName As String = name
        Dim counter As Integer = 1
        While dt.Columns.Contains(name)
            counter += 1
            name = baseName & "_" & counter.ToString()
        End While
        Return name
    End Function

    Private Function CombineComparisonTables(baseTable As DataTable, compareTable As DataTable) As DataTable
        Dim combined As New DataTable()
        combined.Columns.Add(ComparisonSourceColumn, GetType(String))
        AddSourceColumns(combined, baseTable)
        AddSourceColumns(combined, compareTable)
        CopySourceRows(combined, baseTable, "Base")
        CopySourceRows(combined, compareTable, "Compare")
        Return combined
    End Function

    Private Sub AddSourceColumns(target As DataTable, source As DataTable)
        For i As Integer = 0 To source.Columns.Count - 1
            Dim columnName As String = source.Columns(i).ColumnName
            If columnName = ComparisonSourceColumn Then columnName = columnName & "_Data"

            If Not target.Columns.Contains(columnName) Then
                target.Columns.Add(columnName, GetType(String))
            End If
        Next
    End Sub

    Private Sub CopySourceRows(target As DataTable, source As DataTable, sourceName As String)
        For i As Integer = 0 To source.Rows.Count - 1
            Dim newRow As DataRow = target.NewRow()
            newRow(ComparisonSourceColumn) = sourceName
            For j As Integer = 0 To source.Columns.Count - 1
                Dim columnName As String = source.Columns(j).ColumnName
                If columnName = ComparisonSourceColumn Then columnName = columnName & "_Data"
                If target.Columns.Contains(columnName) Then newRow(columnName) = FieldText(source.Rows(i)(j))
            Next
            target.Rows.Add(newRow)
        Next
    End Sub

    Private Sub BuildAndBindComparison()
        LabelError.Text = ""
        UpdateModePanels()
        Dim source As DataTable = GetComparisonSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        Dim compareFieldName As String = DropDownCompareField.SelectedValue
        Dim baseCompareValue As String = DropDownBaseValue.SelectedValue
        Dim targetCompareValue As String = DropDownCompareValue.SelectedValue

        If IsExternalComparisonMode() Then
            compareFieldName = ComparisonSourceColumn
            baseCompareValue = "Base"
            targetCompareValue = "Compare"
        End If

        If compareFieldName.Trim() = "" OrElse DropDownValueField.SelectedValue.Trim() = "" Then
            LabelError.Text = "Compare field and value field must be selected."
            Exit Sub
        End If

        If baseCompareValue.Trim() = "" OrElse targetCompareValue.Trim() = "" Then
            LabelError.Text = "Base and compare values must be selected."
            Exit Sub
        End If

        If baseCompareValue = targetCompareValue Then
            LabelError.Text = "Base and compare values should be different."
            Exit Sub
        End If

        Dim output As DataTable = CreateComparisonTable(source, compareFieldName, baseCompareValue, targetCompareValue)
        Session("ComparisonReportsTable") = output
        BindComparison(output)
    End Sub

    Private Sub BindComparison(dt As DataTable)
        BindAnalysisGrid(dt)

        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = ComparisonTitle() & " (" & dt.Rows.Count.ToString() & " rows)"
        End If
    End Sub

    Private Function CreateComparisonTable(source As DataTable, compareFieldName As String, baseCompareValue As String, targetCompareValue As String) As DataTable
        Session("ComparisonReportsFilters") = New Dictionary(Of String, String)()

        Dim baseBuckets As New Dictionary(Of String, ComparisonBucket)(StringComparer.OrdinalIgnoreCase)
        Dim compareBuckets As New Dictionary(Of String, ComparisonBucket)(StringComparer.OrdinalIgnoreCase)
        Dim rowGroups As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For

            Dim rowText As String = AllRowsValue
            If DropDownRowField.SelectedValue.Trim() <> "" Then rowText = FieldText(dr(DropDownRowField.SelectedValue))

            Dim compareText As String = FieldText(dr(compareFieldName))
            If compareText <> baseCompareValue AndAlso compareText <> targetCompareValue Then Continue For

            If Not rowGroups.ContainsKey(rowText) Then rowGroups.Add(rowText, True)

            If compareText = baseCompareValue Then
                If Not baseBuckets.ContainsKey(rowText) Then baseBuckets.Add(rowText, New ComparisonBucket())
                baseBuckets(rowText).AddRecord(dr, dr(DropDownValueField.SelectedValue))
            Else
                If Not compareBuckets.ContainsKey(rowText) Then compareBuckets.Add(rowText, New ComparisonBucket())
                compareBuckets(rowText).AddRecord(dr, dr(DropDownValueField.SelectedValue))
            End If
        Next

        Dim output As New DataTable()
        output.Columns.Add("Comparison Type", GetType(String))
        Dim rowColumnName As String = "Rows"
        If DropDownRowField.SelectedValue.Trim() <> "" Then rowColumnName = DropDownRowField.SelectedValue
        output.Columns.Add(UniqueOutputColumnName(output, rowColumnName), GetType(String))
        output.Columns.Add("Base (" & baseCompareValue & ")", GetType(String))
        output.Columns.Add("Compare (" & targetCompareValue & ")", GetType(String))
        output.Columns.Add("Variance", GetType(String))
        output.Columns.Add("Percent Change", GetType(String))
        output.Columns.Add("Base Records", GetType(Integer))
        output.Columns.Add("Compare Records", GetType(Integer))
        output.Columns.Add("BaseFilterId", GetType(String))
        output.Columns.Add("CompareFilterId", GetType(String))

        For Each rowText As String In rowGroups.Keys
            Dim baseValue As Double = 0
            Dim compareValue As Double = 0
            Dim baseCount As Integer = 0
            Dim compareCount As Integer = 0
            Dim baseFilterId As String = ""
            Dim compareFilterId As String = ""

            If baseBuckets.ContainsKey(rowText) Then
                baseValue = baseBuckets(rowText).Result(DropDownAggregate.SelectedValue)
                baseCount = baseBuckets(rowText).Count
                baseFilterId = RegisterComparisonFilter(baseBuckets(rowText).Rows)
            End If
            If compareBuckets.ContainsKey(rowText) Then
                compareValue = compareBuckets(rowText).Result(DropDownAggregate.SelectedValue)
                compareCount = compareBuckets(rowText).Count
                compareFilterId = RegisterComparisonFilter(compareBuckets(rowText).Rows)
            End If

            Dim varianceValue As Double = compareValue - baseValue
            Dim percentText As String = ""
            If baseValue <> 0 Then percentText = FormatNumber((varianceValue / baseValue) * 100, 2) & "%"

            Dim outRow As DataRow = output.NewRow()
            outRow(0) = DropDownComparisonType.SelectedItem.Text
            outRow(1) = rowText
            outRow(2) = FormatNumber(baseValue, 2)
            outRow(3) = FormatNumber(compareValue, 2)
            outRow(4) = FormatNumber(varianceValue, 2)
            outRow(5) = percentText
            outRow(6) = baseCount
            outRow(7) = compareCount
            outRow("BaseFilterId") = baseFilterId
            outRow("CompareFilterId") = compareFilterId
            output.Rows.Add(outRow)
        Next

        Return output
    End Function

    Private Function RegisterComparisonFilter(rows As List(Of DataRow)) As String
        If IsExternalComparisonMode() Then Return ""

        Dim rowFilter As String = RowsFilter(rows)
        If rowFilter.Trim() = "" Then Return ""

        Dim filters As Dictionary(Of String, String) = TryCast(Session("ComparisonReportsFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("ComparisonReportsFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = rowFilter
        Return filterId
    End Function

    Private Function RowsFilter(rows As List(Of DataRow)) As String
        If rows Is Nothing OrElse rows.Count = 0 Then Return ""

        Dim filters As New List(Of String)()
        Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In rows
            Dim rowFilter As String = SingleRowFilter(row)
            If rowFilter.Trim() <> "" AndAlso Not seen.ContainsKey(rowFilter) Then
                filters.Add("(" & rowFilter & ")")
                seen(rowFilter) = True
            End If
        Next

        Return String.Join(" OR ", filters.ToArray())
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
        If ColumnTypeIsNumeric(col) AndAlso Double.TryParse(valueText, numericValue) Then Return FieldRef(col) & " = " & numericValue.ToString(Globalization.CultureInfo.InvariantCulture)

        If TypeOf valueObject Is DateTime Then Return FieldRef(col) & " = #" & CType(valueObject, DateTime).ToString("MM/dd/yyyy HH:mm:ss", Globalization.CultureInfo.InvariantCulture) & "#"

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

    Private Function ComparisonTitle() As String
        If IsExternalComparisonMode() Then
            Return DropDownComparisonType.SelectedItem.Text & ": " & DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue & " comparing Base to Compare"
        End If
        Return DropDownComparisonType.SelectedItem.Text & ": " & DropDownAggregate.SelectedValue & " of " & DropDownValueField.SelectedValue & " comparing " & DropDownBaseValue.SelectedValue & " to " & DropDownCompareValue.SelectedValue
    End Function

    Private Sub ExportComparison(formatName As String)
        Dim dt As DataTable = TryCast(Session("ComparisonReportsTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindComparison()
            dt = TryCast(Session("ComparisonReportsTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No comparison data to export."
            Exit Sub
        End If

        Dim fileName As String = "ComparisonReports_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", ComparisonTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), ComparisonTitle(), ""))
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
            GridViewComparison.AllowPaging = False
            GridViewComparison.PageIndex = 0
            GridViewComparison.DataSource = Nothing
            GridViewComparison.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewComparison.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewComparison.PageSize = AnalysisGridPageSize
        Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(dt.Rows.Count / CDbl(AnalysisGridPageSize))))
        If Not GridViewComparison.AllowPaging Then
            GridViewComparison.PageIndex = 0
        ElseIf GridViewComparison.PageIndex < 0 OrElse GridViewComparison.PageIndex >= pageCount Then
            GridViewComparison.PageIndex = 0
        End If

        GridViewComparison.DataSource = dt
        GridViewComparison.DataBind()
        HideAnalysisInternalColumns(dt)
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse GridViewComparison.HeaderRow Is Nothing Then Return
        HideAnalysisColumn(dt, "FilterId")
        HideAnalysisColumn(dt, "BaseFilterId")
        HideAnalysisColumn(dt, "CompareFilterId")
    End Sub

    Private Sub HideAnalysisColumn(ByVal dt As DataTable, ByVal columnName As String)
        If Not dt.Columns.Contains(columnName) Then Return
        Dim columnIndex As Integer = dt.Columns.IndexOf(columnName)
        If columnIndex >= 0 AndAlso columnIndex < GridViewComparison.HeaderRow.Cells.Count Then
            GridViewComparison.HeaderRow.Cells(columnIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewComparison.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewComparison.PageIndex < (GridViewComparison.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewComparison.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewComparison.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewComparison.PageIndex > 0 Then GridViewComparison.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewComparison.PageIndex < (GridViewComparison.PageCount - 1) Then GridViewComparison.PageIndex += 1
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
            GridViewComparison.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Two-source comparison analysis. Inputs can be two periods, two groups, two locations, two SQL queries, or two imported files, plus the row field, value field, aggregate option, base value, compare value, and search text."
        LabelAlgorithmExplanation.Text = "Algorithm: The page builds base and compare datasets, groups both sides by the selected row dimension, aggregates the selected value field, matches groups by the row value, and calculates Compare minus Base plus percent change from Base where Base is not zero."
        LabelOutputExplanation.Text = "Output: The grid shows comparison type, row dimension, Base value, Compare value, Variance, Percent Change, Base Records, and Compare Records. Base Records and Compare Records link to the exact rows used for each side of the comparison."
    End Sub
End Class
