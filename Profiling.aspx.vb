Imports System
Imports System.Collections.Generic
Imports System.Data

Partial Class Profiling
    Inherits System.Web.UI.Page

    Private Class FieldProfile
        Public FieldName As String = ""
        Public SourceType As String = ""
        Public TotalRows As Integer
        Public NonBlankCount As Integer
        Public BlankCount As Integer
        Public NumericCount As Integer
        Public DateCount As Integer
        Public Sum As Double
        Public SumSquares As Double
        Public NumericMin As Double
        Public NumericMax As Double
        Public HasNumeric As Boolean
        Public DateMin As DateTime
        Public DateMax As DateTime
        Public HasDate As Boolean
        Public DistinctValues As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        Public Sub AddValue(valueObject As Object)
            TotalRows += 1

            Dim valueText As String = FieldText(valueObject)
            If valueText.Trim() = "" Then
                BlankCount += 1
                Return
            End If

            NonBlankCount += 1
            If Not DistinctValues.ContainsKey(valueText) Then
                DistinctValues.Add(valueText, True)
            End If

            Dim numericValue As Double
            If Double.TryParse(valueText, numericValue) Then
                NumericCount += 1
                Sum += numericValue
                SumSquares += numericValue * numericValue
                If Not HasNumeric Then
                    NumericMin = numericValue
                    NumericMax = numericValue
                    HasNumeric = True
                Else
                    If numericValue < NumericMin Then NumericMin = numericValue
                    If numericValue > NumericMax Then NumericMax = numericValue
                End If
            End If

            Dim dateValue As DateTime
            If TypeOf valueObject Is DateTime Then
                dateValue = CType(valueObject, DateTime)
                AddDate(dateValue)
            ElseIf DateTime.TryParse(valueText, dateValue) Then
                AddDate(dateValue)
            End If
        End Sub

        Private Sub AddDate(dateValue As DateTime)
            DateCount += 1
            If Not HasDate Then
                DateMin = dateValue
                DateMax = dateValue
                HasDate = True
            Else
                If dateValue < DateMin Then DateMin = dateValue
                If dateValue > DateMax Then DateMax = dateValue
            End If
        End Sub

        Public Function AverageText() As String
            If NumericCount = 0 Then Return ""
            Return FormatNumber(Sum / NumericCount, 2)
        End Function

        Public Function StandardDeviationText() As String
            If NumericCount <= 1 Then Return ""
            Dim variance As Double = (SumSquares - ((Sum * Sum) / NumericCount)) / (NumericCount - 1)
            If variance < 0 Then variance = 0
            Return FormatNumber(Math.Sqrt(variance), 2)
        End Function

        Public Function MinimumText() As String
            If HasNumeric AndAlso NumericCount = NonBlankCount Then Return FormatNumber(NumericMin, 2)
            If HasDate AndAlso DateCount = NonBlankCount Then Return DateMin.ToShortDateString()
            Return ""
        End Function

        Public Function MaximumText() As String
            If HasNumeric AndAlso NumericCount = NonBlankCount Then Return FormatNumber(NumericMax, 2)
            If HasDate AndAlso DateCount = NonBlankCount Then Return DateMax.ToShortDateString()
            Return ""
        End Function

        Public Function DetectedType() As String
            If NonBlankCount = 0 Then Return SourceType
            If NumericCount = NonBlankCount Then Return "Numeric"
            If DateCount = NonBlankCount Then Return "Date/Time"
            Return SourceType
        End Function

        Public Shared Function FieldText(valueObject As Object) As String
            If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
            If TypeOf valueObject Is DateTime Then
                Return CType(valueObject, DateTime).ToShortDateString()
            End If
            Return valueObject.ToString()
        End Function
    End Class

    Private Sub Profiling_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            lblHeader.Text = Session("REPTITLE").ToString() & " - Data Profiling"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Data Profiling"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Data%20Profiling"
    End Sub

    Private Sub Profiling_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            BuildAndBindProfile()
        ElseIf Session("ProfilingTable") IsNot Nothing Then
            BindProfile(CType(Session("ProfilingTable"), DataTable))
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
                Session("ProfilingSource") = existingTable
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
            Session("ProfilingSource") = Nothing
            Return Nothing
        End If

        Session("ProfilingSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("ProfilingSource") Is Nothing Then
            Return LoadReportData()
        End If
        Return CType(Session("ProfilingSource"), DataTable)
    End Function

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then
            Response.Redirect(node.Value)
        End If
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindProfile()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindProfile()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportProfile("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportProfile("xls")
    End Sub

    Private Sub lnkProfilingAI_Click(sender As Object, e As EventArgs) Handles lnkProfilingAI.Click
        Dim dt As DataTable = TryCast(Session("ProfilingTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindProfile()
            dt = TryCast(Session("ProfilingTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No profiling data to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), ProfileTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this automatic data profile. Summarize data quality issues, blank fields, high-cardinality fields, numeric ranges, averages, standard deviations, and fields that may need review.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindProfile()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub

        Dim profile As DataTable = CreateProfileTable(source, txtSearch.Text.Trim())
        Session("ProfilingTable") = profile
        BindProfile(profile)
    End Sub

    Private Sub BindProfile(dt As DataTable)
        BindAnalysisGrid(dt)

        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = ProfileTitle() & " (" & dt.Rows.Count.ToString() & " fields)"
        End If
    End Sub

    Private Function CreateProfileTable(source As DataTable, searchText As String) As DataTable
        Dim profiles As New List(Of FieldProfile)()

        For i As Integer = 0 To source.Columns.Count - 1
            Dim col As DataColumn = source.Columns(i)
            Dim profile As New FieldProfile()
            profile.FieldName = col.ColumnName
            profile.SourceType = col.DataType.Name
            profiles.Add(profile)
        Next

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, searchText) Then Continue For

            For j As Integer = 0 To source.Columns.Count - 1
                profiles(j).AddValue(dr(j))
            Next
        Next

        Dim output As New DataTable()
        output.Columns.Add("Field", GetType(String))
        output.Columns.Add("Data Type", GetType(String))
        output.Columns.Add("Count", GetType(String))
        output.Columns.Add("Blanks", GetType(String))
        output.Columns.Add("Distinct Values", GetType(String))
        output.Columns.Add("Min", GetType(String))
        output.Columns.Add("Max", GetType(String))
        output.Columns.Add("Average", GetType(String))
        output.Columns.Add("Standard Deviation", GetType(String))

        For Each profile As FieldProfile In profiles
            Dim outRow As DataRow = output.NewRow()
            outRow("Field") = profile.FieldName
            outRow("Data Type") = profile.DetectedType()
            outRow("Count") = profile.NonBlankCount.ToString()
            outRow("Blanks") = profile.BlankCount.ToString()
            outRow("Distinct Values") = profile.DistinctValues.Count.ToString()
            outRow("Min") = profile.MinimumText()
            outRow("Max") = profile.MaximumText()
            outRow("Average") = profile.AverageText()
            outRow("Standard Deviation") = profile.StandardDeviationText()
            output.Rows.Add(outRow)
        Next

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
        Return FieldProfile.FieldText(valueObject)
    End Function

    Private Function ProfileTitle() As String
        If txtSearch.Text.Trim() = "" Then
            Return "Automatic data profile"
        End If
        Return "Automatic data profile filtered by '" & txtSearch.Text.Trim() & "'"
    End Function

    Private Sub ExportProfile(formatName As String)
        Dim dt As DataTable = TryCast(Session("ProfilingTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindProfile()
            dt = TryCast(Session("ProfilingTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No profiling data to export."
            Exit Sub
        End If

        Dim fileName As String = "DataProfile_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", ProfileTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), ProfileTitle(), ""))
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
            GridViewProfile.AllowPaging = False
            GridViewProfile.PageIndex = 0
            GridViewProfile.DataSource = Nothing
            GridViewProfile.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewProfile.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewProfile.PageSize = AnalysisGridPageSize
        If Not GridViewProfile.AllowPaging Then
            GridViewProfile.PageIndex = 0
        ElseIf GridViewProfile.PageIndex < 0 OrElse GridViewProfile.PageIndex >= GridViewProfile.PageCount Then
            GridViewProfile.PageIndex = 0
        End If

        GridViewProfile.DataSource = dt
        GridViewProfile.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewProfile.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewProfile.HeaderRow.Cells.Count Then
            GridViewProfile.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewProfile.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewProfile.PageIndex < (GridViewProfile.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewProfile.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewProfile.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewProfile.PageIndex > 0 Then GridViewProfile.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewProfile.PageIndex < (GridViewProfile.PageCount - 1) Then GridViewProfile.PageIndex += 1
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
            GridViewProfile.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Automatic field profiling for the current report or imported dataset. Inputs are all available columns and the records that match the current search text."
        LabelAlgorithmExplanation.Text = "Algorithm: The page scans every field, detects usable data type patterns, counts total and blank values, counts distinct values, and calculates numeric or date statistics where the column values support those calculations."
        LabelOutputExplanation.Text = "Output: The grid lists one row per field with detected type, count, blanks, distinct values, minimum, maximum, average, standard deviation, and notes. Numeric statistics are populated only for fields that can be interpreted as numbers."
    End Sub
End Class
