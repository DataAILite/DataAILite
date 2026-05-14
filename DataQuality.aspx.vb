Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Text
Imports System.Web.UI.WebControls

Partial Class DataQuality
    Inherits System.Web.UI.Page

    Private Class NumericStats
        Public Count As Integer
        Public Sum As Double
        Public SumSquares As Double
        Public Minimum As Double
        Public Maximum As Double
        Public HasValue As Boolean

        Public Sub AddValue(value As Double)
            Count += 1
            Sum += value
            SumSquares += value * value
            If Not HasValue Then
                Minimum = value
                Maximum = value
                HasValue = True
            Else
                If value < Minimum Then Minimum = value
                If value > Maximum Then Maximum = value
            End If
        End Sub

        Public Function Average() As Double
            If Count = 0 Then Return 0
            Return Sum / Count
        End Function

        Public Function StandardDeviation() As Double
            If Count <= 1 Then Return 0
            Dim variance As Double = (SumSquares - ((Sum * Sum) / Count)) / (Count - 1)
            If variance < 0 Then variance = 0
            Return Math.Sqrt(variance)
        End Function
    End Class

    Private Sub DataQuality_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            lblHeader.Text = Session("REPTITLE").ToString() & " - Data Quality"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Data Quality"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Data%20Quality"
    End Sub

    Private Sub DataQuality_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            BuildAndBindQuality()
        ElseIf Session("DataQualityTable") IsNot Nothing Then
            BindQuality(CType(Session("DataQualityTable"), DataTable))
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
            Dim existingTable As DataTable = TryCast(Session("dataTable"), DataTable)
            If existingTable IsNot Nothing AndAlso existingTable.Rows.Count > 0 Then
                Session("DataQualitySource") = existingTable
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

        If ret.Trim() <> "" Then
            LabelError.Text = ret
        End If

        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            LabelError.Text = "No data. Run or import report data first."
            Session("DataQualitySource") = Nothing
            Return Nothing
        End If

        Session("DataQualitySource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("DataQualitySource") Is Nothing Then
            Return LoadReportData()
        End If
        Return CType(Session("DataQualitySource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then
            Response.Redirect(node.Value)
        End If
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindQuality()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        txtStdDevLimit.Text = "3"
        BuildAndBindQuality()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportQuality("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportQuality("xls")
    End Sub

    Private Sub lnkDataQualityAI_Click(sender As Object, e As EventArgs) Handles lnkDataQualityAI.Click
        Dim dt As DataTable = TryCast(Session("DataQualityTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindQuality()
            dt = TryCast(Session("DataQualityTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data quality checks to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt, "Records")
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), QualityTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret these data quality checks. Summarize missing values, duplicate records, invalid dates, out-of-range numbers, inconsistent categories, suspicious text values, and recommended cleanup priorities.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindQuality()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        Dim stdevLimit As Double = 3
        If Not Double.TryParse(txtStdDevLimit.Text.Trim(), stdevLimit) OrElse stdevLimit <= 0 Then
            stdevLimit = 3
            txtStdDevLimit.Text = "3"
        End If

        Dim checks As DataTable = CreateQualityTable(source, txtSearch.Text.Trim(), stdevLimit)
        Session("DataQualityTable") = checks
        BindQuality(checks)
    End Sub

    Private Sub BindQuality(dt As DataTable)
        BindAnalysisGrid(dt)

        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = QualityTitle() & " (" & dt.Rows.Count.ToString() & " findings)"
        End If
    End Sub

    Private Sub GridViewQuality_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewQuality.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow OrElse e.Row.Cells.Count = 0 Then Exit Sub

        Dim filterId As String = e.Row.Cells(0).Text.Replace("&nbsp;", "").Trim()
        Dim countIndex As Integer = GetQualityColumnIndex("Count")
        e.Row.Cells(0).Controls.Clear()

        If filterId <> "" AndAlso countIndex >= 0 AndAlso countIndex < e.Row.Cells.Count Then
            Dim countText As String = e.Row.Cells(countIndex).Text.Replace("&nbsp;", "").Trim()
            Dim link As New HyperLink()
            link.Text = countText
            link.CssClass = "NodeStyle"
            link.NavigateUrl = "~/ShowReport.aspx?srd=0&dqfilter=" & Server.UrlEncode(filterId)
            link.ToolTip = "Open corresponding records in ShowReport."
            e.Row.Cells(countIndex).Controls.Clear()
            e.Row.Cells(countIndex).Controls.Add(link)
        End If

        e.Row.Cells(0).Text = ""
    End Sub

    Private Function GetQualityColumnIndex(columnName As String) As Integer
        Dim dt As DataTable = TryCast(Session("DataQualityTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then Return -1
        Return dt.Columns.IndexOf(columnName)
    End Function

    Private Function CreateQualityTable(source As DataTable, searchText As String, stdevLimit As Double) As DataTable
        Session("DataQualityFilters") = New Dictionary(Of String, String)()

        Dim filteredRows As New List(Of DataRow)()
        For i As Integer = 0 To source.Rows.Count - 1
            If RowMatchesSearch(source.Rows(i), searchText) Then
                filteredRows.Add(source.Rows(i))
            End If
        Next

        Dim output As DataTable = CreateOutputTable()
        CheckMissingValues(source, filteredRows, output)
        CheckDuplicateRecords(source, filteredRows, output)
        CheckInvalidDates(source, filteredRows, output)
        CheckOutOfRangeNumbers(source, filteredRows, output, stdevLimit)
        CheckInconsistentCategories(source, filteredRows, output)
        CheckSuspiciousText(source, filteredRows, output)

        If output.Rows.Count = 0 Then
            AddFinding(output, "Summary", "", "No issues found", 0, "No quality issues were detected using the current checks.", "")
        End If

        Return output
    End Function

    Private Function CreateOutputTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Records", GetType(String))
        dt.Columns.Add("Check", GetType(String))
        dt.Columns.Add("Field", GetType(String))
        dt.Columns.Add("Issue", GetType(String))
        dt.Columns.Add("Count", GetType(String))
        dt.Columns.Add("Details", GetType(String))
        Return dt
    End Function

    Private Sub AddFinding(output As DataTable, checkName As String, fieldName As String, issue As String, count As Integer, details As String, rowFilter As String)
        Dim filterId As String = ""
        If rowFilter.Trim() <> "" Then
            Dim filters As Dictionary(Of String, String) = TryCast(Session("DataQualityFilters"), Dictionary(Of String, String))
            If filters Is Nothing Then
                filters = New Dictionary(Of String, String)()
                Session("DataQualityFilters") = filters
            End If
            filterId = Guid.NewGuid().ToString("N")
            filters(filterId) = rowFilter
        End If

        Dim dr As DataRow = output.NewRow()
        dr("Records") = filterId
        dr("Check") = checkName
        dr("Field") = fieldName
        dr("Issue") = issue
        dr("Count") = count.ToString()
        dr("Details") = details
        output.Rows.Add(dr)
    End Sub

    Private Sub CheckMissingValues(source As DataTable, rows As List(Of DataRow), output As DataTable)
        For Each col As DataColumn In source.Columns
            Dim blanks As Integer = 0
            For Each row As DataRow In rows
                If FieldText(row(col)).Trim() = "" Then blanks += 1
            Next
            If blanks > 0 Then
                AddFinding(output, "Missing Values", col.ColumnName, "Blank or null values", blanks, blanks.ToString() & " of " & rows.Count.ToString() & " records are blank.", BlankFilter(col))
            End If
        Next
    End Sub

    Private Sub CheckDuplicateRecords(source As DataTable, rows As List(Of DataRow), output As DataTable)
        Dim seen As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In rows
            Dim key As String = RowKey(row)
            If seen.ContainsKey(key) Then
                seen(key) += 1
            Else
                seen.Add(key, 1)
            End If
        Next

        Dim duplicateGroups As Integer = 0
        Dim duplicateRows As Integer = 0
        Dim duplicateKeys As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each kvp As KeyValuePair(Of String, Integer) In seen
            If kvp.Value > 1 Then
                duplicateGroups += 1
                duplicateRows += kvp.Value
                duplicateKeys(kvp.Key) = True
            End If
        Next

        If duplicateRows > 0 Then
            Dim issueRows As New List(Of DataRow)()
            For Each row As DataRow In rows
                If duplicateKeys.ContainsKey(RowKey(row)) Then issueRows.Add(row)
            Next
            AddFinding(output, "Duplicate Records", "All fields", "Duplicate full records", duplicateRows, duplicateGroups.ToString() & " duplicate record group(s) found.", RowsFilter(issueRows))
        End If
    End Sub

    Private Sub CheckInvalidDates(source As DataTable, rows As List(Of DataRow), output As DataTable)
        For Each col As DataColumn In source.Columns
            If ColumnTypeIsNumeric(col) Then Continue For

            Dim dateLike As Integer = 0
            Dim invalidDates As Integer = 0
            Dim issueRows As New List(Of DataRow)()
            For Each row As DataRow In rows
                Dim text As String = FieldText(row(col)).Trim()
                If text = "" Then Continue For

                If LooksLikeDate(text) Then
                    dateLike += 1
                    Dim parsed As DateTime
                    If Not DateTime.TryParse(text, parsed) Then
                        invalidDates += 1
                        issueRows.Add(row)
                    End If
                End If
            Next

            If invalidDates > 0 Then
                AddFinding(output, "Invalid Dates", col.ColumnName, "Date-like values that cannot be parsed", invalidDates, invalidDates.ToString() & " invalid date value(s) among " & dateLike.ToString() & " date-like values.", RowsFilter(issueRows))
            End If
        Next
    End Sub

    Private Sub CheckOutOfRangeNumbers(source As DataTable, rows As List(Of DataRow), output As DataTable, stdevLimit As Double)
        For Each col As DataColumn In source.Columns
            Dim stats As New NumericStats()
            For Each row As DataRow In rows
                Dim value As Double
                If Double.TryParse(FieldText(row(col)), value) Then
                    stats.AddValue(value)
                End If
            Next

            If stats.Count <= 1 Then Continue For

            Dim avg As Double = stats.Average()
            Dim sd As Double = stats.StandardDeviation()
            If sd = 0 Then Continue For

            Dim lowLimit As Double = avg - (stdevLimit * sd)
            Dim highLimit As Double = avg + (stdevLimit * sd)
            Dim outliers As Integer = 0
            Dim issueRows As New List(Of DataRow)()

            For Each row As DataRow In rows
                Dim value As Double
                If Double.TryParse(FieldText(row(col)), value) Then
                    If value < lowLimit OrElse value > highLimit Then
                        outliers += 1
                        issueRows.Add(row)
                    End If
                End If
            Next

            If outliers > 0 Then
                AddFinding(output, "Out-of-Range Numbers", col.ColumnName, "Values outside " & stdevLimit.ToString() & " standard deviations", outliers, "Average=" & FormatNumber(avg, 2) & "; StDev=" & FormatNumber(sd, 2) & "; Range=" & FormatNumber(lowLimit, 2) & " to " & FormatNumber(highLimit, 2) & ".", RowsFilter(issueRows))
            End If
        Next
    End Sub

    Private Sub CheckInconsistentCategories(source As DataTable, rows As List(Of DataRow), output As DataTable)
        For Each col As DataColumn In source.Columns
            If ColumnTypeIsNumeric(col) Then Continue For

            Dim variants As New Dictionary(Of String, Dictionary(Of String, Boolean))(StringComparer.OrdinalIgnoreCase)
            Dim distinctValues As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

            For Each row As DataRow In rows
                Dim valueText As String = FieldText(row(col))
                If valueText.Trim() = "" Then Continue For
                If Not distinctValues.ContainsKey(valueText) Then distinctValues.Add(valueText, True)

                Dim normalized As String = NormalizeCategory(valueText)
                If Not variants.ContainsKey(normalized) Then
                    variants.Add(normalized, New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase))
                End If
                If Not variants(normalized).ContainsKey(valueText) Then variants(normalized).Add(valueText, True)
            Next

            If distinctValues.Count > 100 Then Continue For

            Dim variantGroups As Integer = 0
            Dim examples As New List(Of String)()
            Dim variantKeys As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            For Each kvp As KeyValuePair(Of String, Dictionary(Of String, Boolean)) In variants
                If kvp.Value.Count > 1 Then
                    variantGroups += 1
                    variantKeys(kvp.Key) = True
                    If examples.Count < 3 Then examples.Add(String.Join(" / ", New List(Of String)(kvp.Value.Keys).ToArray()))
                End If
            Next

            If variantGroups > 0 Then
                Dim issueRows As New List(Of DataRow)()
                For Each row As DataRow In rows
                    Dim normalized As String = NormalizeCategory(FieldText(row(col)))
                    If variantKeys.ContainsKey(normalized) Then issueRows.Add(row)
                Next
                AddFinding(output, "Inconsistent Categories", col.ColumnName, "Case, spacing, or punctuation variants", variantGroups, "Examples: " & String.Join("; ", examples.ToArray()), RowsFilter(issueRows))
            End If
        Next
    End Sub

    Private Sub CheckSuspiciousText(source As DataTable, rows As List(Of DataRow), output As DataTable)
        For Each col As DataColumn In source.Columns
            If ColumnTypeIsNumeric(col) Then Continue For

            Dim leadingTrailing As Integer = 0
            Dim controlChars As Integer = 0
            Dim longText As Integer = 0
            Dim htmlText As Integer = 0
            Dim repeatedChars As Integer = 0
            Dim leadingTrailingRows As New List(Of DataRow)()
            Dim controlCharRows As New List(Of DataRow)()
            Dim longTextRows As New List(Of DataRow)()
            Dim htmlTextRows As New List(Of DataRow)()
            Dim repeatedCharRows As New List(Of DataRow)()

            For Each row As DataRow In rows
                Dim valueText As String = FieldText(row(col))
                If valueText.Trim() = "" Then Continue For

                If valueText <> valueText.Trim() Then
                    leadingTrailing += 1
                    leadingTrailingRows.Add(row)
                End If
                If HasControlCharacters(valueText) Then
                    controlChars += 1
                    controlCharRows.Add(row)
                End If
                If valueText.Length > 255 Then
                    longText += 1
                    longTextRows.Add(row)
                End If
                If ContainsHtmlOrScript(valueText) Then
                    htmlText += 1
                    htmlTextRows.Add(row)
                End If
                If HasRepeatedCharacters(valueText) Then
                    repeatedChars += 1
                    repeatedCharRows.Add(row)
                End If
            Next

            If leadingTrailing > 0 Then AddFinding(output, "Suspicious Text", col.ColumnName, "Leading or trailing spaces", leadingTrailing, "Values may need trimming.", RowsFilter(leadingTrailingRows))
            If controlChars > 0 Then AddFinding(output, "Suspicious Text", col.ColumnName, "Control characters", controlChars, "Values contain tab, line break, or non-printable characters.", RowsFilter(controlCharRows))
            If longText > 0 Then AddFinding(output, "Suspicious Text", col.ColumnName, "Very long text", longText, "Values longer than 255 characters.", RowsFilter(longTextRows))
            If htmlText > 0 Then AddFinding(output, "Suspicious Text", col.ColumnName, "HTML or script-like text", htmlText, "Values contain markup-like text.", RowsFilter(htmlTextRows))
            If repeatedChars > 0 Then AddFinding(output, "Suspicious Text", col.ColumnName, "Repeated character patterns", repeatedChars, "Values contain a character repeated 6 or more times.", RowsFilter(repeatedCharRows))
        Next
    End Sub

    Private Function RowMatchesSearch(row As DataRow, searchText As String) As Boolean
        If searchText.Trim() = "" Then Return True
        For i As Integer = 0 To row.Table.Columns.Count - 1
            If FieldText(row(i)).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0 Then
                Return True
            End If
        Next
        Return False
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

    Private Function BlankFilter(col As DataColumn) As String
        If ColumnTypeIsNumeric(col) OrElse col.DataType Is GetType(DateTime) Then
            Return FieldRef(col) & " IS NULL"
        End If
        Return "(" & FieldRef(col) & " IS NULL OR " & FieldRef(col) & " = '')"
    End Function

    Private Function ValueFilter(col As DataColumn, valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return FieldRef(col) & " IS NULL"

        Dim valueText As String = FieldText(valueObject)
        If valueText.Trim() = "" Then Return BlankFilter(col)

        Dim numericValue As Double
        If ColumnTypeIsNumeric(col) AndAlso Double.TryParse(valueText, numericValue) Then
            Return FieldRef(col) & " = " & numericValue.ToString(CultureInfo.InvariantCulture)
        End If

        If TypeOf valueObject Is DateTime Then
            Return FieldRef(col) & " = #" & CType(valueObject, DateTime).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture) & "#"
        End If

        Return FieldRef(col) & " = '" & EscapeFilterValue(valueText) & "'"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Function EscapeFilterValue(valueText As String) As String
        Return valueText.Replace("'", "''")
    End Function

    Private Function RowKey(row As DataRow) As String
        Dim parts As New List(Of String)()
        For i As Integer = 0 To row.Table.Columns.Count - 1
            parts.Add(FieldText(row(i)).Trim())
        Next
        Return String.Join(ChrW(30), parts.ToArray())
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Function LooksLikeDate(valueText As String) As Boolean
        If valueText.IndexOf("/"c) >= 0 OrElse valueText.IndexOf("-"c) >= 0 Then
            For i As Integer = 0 To valueText.Length - 1
                If Char.IsDigit(valueText.Chars(i)) Then Return True
            Next
        End If
        Return False
    End Function

    Private Function NormalizeCategory(valueText As String) As String
        Dim sb As New StringBuilder()
        Dim lastWasSpace As Boolean = False
        Dim text As String = valueText.Trim().ToUpper()

        For i As Integer = 0 To text.Length - 1
            Dim ch As Char = text.Chars(i)
            If Char.IsLetterOrDigit(ch) Then
                sb.Append(ch)
                lastWasSpace = False
            ElseIf Char.IsWhiteSpace(ch) AndAlso Not lastWasSpace Then
                sb.Append(" "c)
                lastWasSpace = True
            End If
        Next

        Return sb.ToString().Trim()
    End Function

    Private Function HasControlCharacters(valueText As String) As Boolean
        For i As Integer = 0 To valueText.Length - 1
            Dim ch As Char = valueText.Chars(i)
            If Char.IsControl(ch) Then Return True
        Next
        Return False
    End Function

    Private Function ContainsHtmlOrScript(valueText As String) As Boolean
        Dim text As String = valueText.ToLower()
        Return text.Contains("<script") OrElse text.Contains("</") OrElse text.Contains("<br") OrElse text.Contains("<div") OrElse text.Contains("<span") OrElse text.Contains("<html")
    End Function

    Private Function HasRepeatedCharacters(valueText As String) As Boolean
        Dim repeatCount As Integer = 1
        Dim lastChar As Char = ChrW(0)

        For i As Integer = 0 To valueText.Length - 1
            Dim ch As Char = valueText.Chars(i)
            If ch = lastChar Then
                repeatCount += 1
                If repeatCount >= 6 Then Return True
            Else
                repeatCount = 1
                lastChar = ch
            End If
        Next

        Return False
    End Function

    Private Function QualityTitle() As String
        If txtSearch.Text.Trim() = "" Then Return "Data quality checks"
        Return "Data quality checks filtered by '" & txtSearch.Text.Trim() & "'"
    End Function

    Private Sub ExportQuality(formatName As String)
        Dim dt As DataTable = TryCast(Session("DataQualityTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindQuality()
            dt = TryCast(Session("DataQualityTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No data quality checks to export."
            Exit Sub
        End If

        Dim fileName As String = "DataQuality_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", QualityTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), QualityTitle(), ""))
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
            GridViewQuality.AllowPaging = False
            GridViewQuality.PageIndex = 0
            GridViewQuality.DataSource = Nothing
            GridViewQuality.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewQuality.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewQuality.PageSize = AnalysisGridPageSize
        If Not GridViewQuality.AllowPaging Then
            GridViewQuality.PageIndex = 0
        ElseIf GridViewQuality.PageIndex < 0 OrElse GridViewQuality.PageIndex >= GridViewQuality.PageCount Then
            GridViewQuality.PageIndex = 0
        End If

        GridViewQuality.DataSource = dt
        GridViewQuality.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewQuality.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewQuality.HeaderRow.Cells.Count Then
            GridViewQuality.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewQuality.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewQuality.PageIndex < (GridViewQuality.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewQuality.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewQuality.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewQuality.PageIndex > 0 Then GridViewQuality.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewQuality.PageIndex < (GridViewQuality.PageCount - 1) Then GridViewQuality.PageIndex += 1
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
            GridViewQuality.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Data quality rule checks for the current report data. Inputs are all fields, current search text, and the configured numeric standard-deviation threshold."
        LabelAlgorithmExplanation.Text = "Algorithm: The page evaluates missing values, duplicate rows, invalid date-like values, out-of-range or suspicious numeric values, inconsistent category spelling/casing, and suspicious text patterns. Each issue is registered with a filter that can reopen the affected records."
        LabelOutputExplanation.Text = "Output: The grid shows the issue type, field, affected record count, and issue description. Record-count links open the exact rows in Data Explorer so the questionable values can be reviewed in context."
    End Sub
End Class
