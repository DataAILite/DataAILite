Imports System
Imports System.Collections.Generic
Imports System.Data

Partial Class CorrelationThreshold
    Inherits System.Web.UI.Page

    Private Sub CorrelationThreshold_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Correlation Threshold"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Correlation%20Threshold"
    End Sub

    Private Sub CorrelationThreshold_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            BuildAndBindCorrelation()
        ElseIf Session("CorrelationThresholdTable") IsNot Nothing Then
            BindCorrelation(CType(Session("CorrelationThresholdTable"), DataTable))
        End If
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindCorrelation()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        BuildAndBindCorrelation()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportCorrelation("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportCorrelation("xls")
    End Sub

    Private Sub lnkCorrelationAI_Click(sender As Object, e As EventArgs) Handles lnkCorrelationAI.Click
        Dim dt As DataTable = TryCast(Session("CorrelationThresholdTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindCorrelation()
            dt = TryCast(Session("CorrelationThresholdTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No correlation threshold results to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), "Correlation Threshold", "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this correlation threshold analysis. Explain the strongest positive and negative relationships, which field pairs should be reviewed, and any patterns that may support reporting, dashboards, or further analysis.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindCorrelation()
        LabelError.Text = ""
        Dim threshold As Double = 0.55
        Double.TryParse(txtThreshold.Text.Trim(), threshold)

        Dim output As New DataTable()
        output.Columns.Add("Field 1", GetType(String))
        output.Columns.Add("Field 2", GetType(String))
        output.Columns.Add("Correlation", GetType(String))
        output.Columns.Add("Strength", GetType(String))
        output.Columns.Add("View", GetType(String))

        Dim source As DataTable = CorrelationSource()
        If source IsNot Nothing AndAlso source.Rows.Count > 0 Then
            For i As Integer = 0 To source.Rows.Count - 1
                AddStoredCorrelationIfMatched(output, source.Rows(i), threshold)
            Next
        Else
            BuildLiveCorrelations(output, threshold)
        End If

        Session("CorrelationThresholdTable") = output
        BindCorrelation(output)
    End Sub

    Private Function CorrelationSource() As DataTable
        If Session("REPORTID") Is Nothing Then Return Nothing
        Try
            Dim sql As String = "SELECT Tbl1Fld1,Tbl2Fld2,Param2 FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID").ToString().Replace("'", "''") & "' AND Doing='CORRELATE' ORDER BY Tbl1Fld1,Tbl2Fld2"
            Dim dv As DataView = mRecords(sql)
            If dv IsNot Nothing Then Return dv.Table
        Catch ex As Exception
        End Try
        Return Nothing
    End Function

    Private Sub AddStoredCorrelationIfMatched(output As DataTable, row As DataRow, threshold As Double)
        Dim field1 As String = FieldText(row("Tbl1Fld1"))
        Dim field2 As String = FieldText(row("Tbl2Fld2"))
        Dim corr As Double = NumericValue(row("Param2"))
        If Not CorrelationMatches(field1, field2, corr, threshold) Then Exit Sub
        AddCorrelationRow(output, field1, field2, corr)
    End Sub

    Private Sub BuildLiveCorrelations(output As DataTable, threshold As Double)
        Dim dt As DataTable = CurrentReportData()
        If dt Is Nothing Then Exit Sub
        Dim numericCols As New List(Of DataColumn)()
        For i As Integer = 0 To dt.Columns.Count - 1
            If ColumnTypeIsNumeric(dt.Columns(i)) Then numericCols.Add(dt.Columns(i))
        Next
        For i As Integer = 0 To numericCols.Count - 1
            For j As Integer = i + 1 To numericCols.Count - 1
                Dim corr As Double = Correlation(dt, numericCols(i), numericCols(j))
                If CorrelationMatches(numericCols(i).ColumnName, numericCols(j).ColumnName, corr, threshold) Then AddCorrelationRow(output, numericCols(i).ColumnName, numericCols(j).ColumnName, corr)
            Next
        Next
    End Sub

    Private Function CurrentReportData() As DataTable
        If Session("REPORTID") Is Nothing Then Return Nothing
        Dim ret As String = ""
        Try
            Dim dv As DataView = RetrieveReportData(Session("REPORTID").ToString(), "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If dv IsNot Nothing Then Return dv.Table
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        End Try
        Return Nothing
    End Function

    Private Function CorrelationMatches(field1 As String, field2 As String, corr As Double, threshold As Double) As Boolean
        If txtSearch.Text.Trim() <> "" AndAlso field1.IndexOf(txtSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase) < 0 AndAlso field2.IndexOf(txtSearch.Text.Trim(), StringComparison.OrdinalIgnoreCase) < 0 Then Return False
        If Math.Abs(corr) < threshold Then Return False
        If DropDownView.SelectedValue = "Positive" AndAlso corr < 0 Then Return False
        If DropDownView.SelectedValue = "Negative" AndAlso corr > 0 Then Return False
        Return True
    End Function

    Private Sub AddCorrelationRow(output As DataTable, field1 As String, field2 As String, corr As Double)
        Dim row As DataRow = output.NewRow()
        row("Field 1") = field1
        row("Field 2") = field2
        row("Correlation") = FormatNumber(corr, 4)
        If Math.Abs(corr) >= 0.8 Then row("Strength") = "Strong" Else row("Strength") = "Moderate"
        If corr >= 0 Then row("View") = "Positive" Else row("View") = "Negative"
        output.Rows.Add(row)
    End Sub

    Private Function Correlation(dt As DataTable, col1 As DataColumn, col2 As DataColumn) As Double
        Dim n As Integer = 0
        Dim sx As Double = 0
        Dim sy As Double = 0
        Dim sxy As Double = 0
        Dim sx2 As Double = 0
        Dim sy2 As Double = 0
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim x As Double = NumericValue(dt.Rows(i)(col1.ColumnName))
            Dim y As Double = NumericValue(dt.Rows(i)(col2.ColumnName))
            n += 1
            sx += x
            sy += y
            sxy += x * y
            sx2 += x * x
            sy2 += y * y
        Next
        Dim denominator As Double = Math.Sqrt((n * sx2 - sx * sx) * (n * sy2 - sy * sy))
        If denominator = 0 Then Return 0
        Return (n * sxy - sx * sy) / denominator
    End Function

    Private Sub BindCorrelation(dt As DataTable)
        BindAnalysisGrid(dt)
        If dt Is Nothing Then LabelInfo.Text = "" Else LabelInfo.Text = "Correlation threshold results (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

    Private Function NumericValue(valueObject As Object) As Double
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return 0
        Dim value As Double
        If Double.TryParse(valueObject.ToString(), value) Then Return value
        Return 0
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        Return valueObject.ToString()
    End Function

    Private Sub ExportCorrelation(formatName As String)
        Dim dt As DataTable = TryCast(Session("CorrelationThresholdTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindCorrelation()
            dt = TryCast(Session("CorrelationThresholdTable"), DataTable)
        End If
        If dt Is Nothing Then Exit Sub
        Dim fileName As String = "CorrelationThreshold_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Correlation Threshold", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Correlation Threshold", ""))
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
            GridViewCorrelation.AllowPaging = False
            GridViewCorrelation.PageIndex = 0
            GridViewCorrelation.DataSource = Nothing
            GridViewCorrelation.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewCorrelation.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewCorrelation.PageSize = AnalysisGridPageSize
        If Not GridViewCorrelation.AllowPaging Then
            GridViewCorrelation.PageIndex = 0
        ElseIf GridViewCorrelation.PageIndex < 0 OrElse GridViewCorrelation.PageIndex >= GridViewCorrelation.PageCount Then
            GridViewCorrelation.PageIndex = 0
        End If

        GridViewCorrelation.DataSource = dt
        GridViewCorrelation.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewCorrelation.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewCorrelation.HeaderRow.Cells.Count Then
            GridViewCorrelation.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewCorrelation.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewCorrelation.PageIndex < (GridViewCorrelation.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewCorrelation.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewCorrelation.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewCorrelation.PageIndex > 0 Then GridViewCorrelation.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewCorrelation.PageIndex < (GridViewCorrelation.PageCount - 1) Then GridViewCorrelation.PageIndex += 1
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
            GridViewCorrelation.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Correlation threshold analysis for numeric field relationships. Inputs are eligible numeric fields, threshold direction, threshold value, and search text."
        LabelAlgorithmExplanation.Text = "Algorithm: The page calculates pairwise correlation values between numeric fields, compares each pair to the selected threshold, and classifies the relationship direction and strength."
        LabelOutputExplanation.Text = "Output: The grid shows field pair, correlation value, threshold match, direction, and interpretation. Values close to 1 indicate strong positive movement together; values close to -1 indicate strong opposite movement."
    End Sub
End Class
