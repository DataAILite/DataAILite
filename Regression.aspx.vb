Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Math

Partial Class Regression
    Inherits System.Web.UI.Page

    Private Const AllGroupsValue As String = "(All)"

    Private Class RegressionBucket
        Public Count As Integer
        Public SumX As Double
        Public SumY As Double
        Public SumXX As Double
        Public SumYY As Double
        Public SumXY As Double
        Public MinX As Double
        Public MaxX As Double
        Public MinY As Double
        Public MaxY As Double
        Public XValues As New List(Of Double)()
        Public YValues As New List(Of Double)()
        Public Rows As New List(Of DataRow)()

        Public Sub AddPair(xValue As Double, yValue As Double, Optional sourceRow As DataRow = Nothing)
            Count += 1
            XValues.Add(xValue)
            YValues.Add(yValue)
            If sourceRow IsNot Nothing Then Rows.Add(sourceRow)
            SumX += xValue
            SumY += yValue
            SumXX += xValue * xValue
            SumYY += yValue * yValue
            SumXY += xValue * yValue

            If Count = 1 Then
                MinX = xValue
                MaxX = xValue
                MinY = yValue
                MaxY = yValue
            Else
                If xValue < MinX Then MinX = xValue
                If xValue > MaxX Then MaxX = xValue
                If yValue < MinY Then MinY = yValue
                If yValue > MaxY Then MaxY = yValue
            End If
        End Sub

        Public Function AverageX() As Double
            If Count = 0 Then Return 0
            Return SumX / Count
        End Function

        Public Function AverageY() As Double
            If Count = 0 Then Return 0
            Return SumY / Count
        End Function

        Public Function Slope() As Double
            Dim denominator As Double = Count * SumXX - SumX * SumX
            If Count < 2 OrElse denominator = 0 Then Return 0
            Return (Count * SumXY - SumX * SumY) / denominator
        End Function

        Public Function Intercept() As Double
            If Count = 0 Then Return 0
            Return AverageY() - Slope() * AverageX()
        End Function

        Public Function Correlation() As Double
            Dim denominator As Double = Sqrt((Count * SumXX - SumX * SumX) * (Count * SumYY - SumY * SumY))
            If Count < 2 OrElse denominator = 0 Then Return 0
            Return (Count * SumXY - SumX * SumY) / denominator
        End Function

        Public Function PredictedY(xValue As Double) As Double
            Return Intercept() + Slope() * xValue
        End Function
    End Class

    Private Class RegressionFit
        Public ModelName As String = ""
        Public Equation As String = ""
        Public Coefficients As String = ""
        Public RSquared As Double = 0
        Public Slope As Nullable(Of Double)
        Public Intercept As Nullable(Of Double)
        Public IsValid As Boolean = False
        Public Predictor As Func(Of Double, Double)
    End Class

    Private Sub Regression_Init(sender As Object, e As EventArgs) Handles Me.Init
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
            lblHeader.Text = Session("REPTITLE").ToString() & " - Regression Analysis"
        ElseIf Not Session("REPORTID") Is Nothing Then
            lblHeader.Text = Session("REPORTID").ToString() & " - Regression Analysis"
        End If

        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Regression%20Analysis"
    End Sub

    Private Sub Regression_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            LabelError.Text = ""
            LabelInfo.Text = ""
            LoadReportData()
            FillFieldLists()
            RestoreRegressionSelections()
            BuildAndBindRegression()
        ElseIf Session("RegressionTable") IsNot Nothing Then
            BindRegression(CType(Session("RegressionTable"), DataTable))
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
            Session("RegressionSource") = Nothing
            Return Nothing
        End If

        Session("RegressionSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("RegressionSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("RegressionSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim dt As DataTable = GetSourceTable()
        If dt Is Nothing Then Exit Sub

        DropDownXField.Items.Clear()
        DropDownYField.Items.Clear()
        DropDownGroupField.Items.Clear()
        DropDownGroupField.Items.Add(New ListItem(AllGroupsValue, ""))

        For i As Integer = 0 To dt.Columns.Count - 1
            Dim fld As String = dt.Columns(i).ColumnName
            If IsNumericField(dt, fld) Then
                DropDownXField.Items.Add(New ListItem(fld, fld))
                DropDownYField.Items.Add(New ListItem(fld, fld))
            ElseIf IsBinaryField(dt, fld) Then
                DropDownYField.Items.Add(New ListItem(fld, fld))
            End If
            DropDownGroupField.Items.Add(New ListItem(fld, fld))
        Next

        If DropDownYField.Items.Count > 1 Then DropDownYField.SelectedIndex = 1
        SetDefaultPredictionValue(dt)
    End Sub

    Private Function RegressionSessionKey(settingName As String) As String
        Dim repid As String = ""
        If Not Session("REPORTID") Is Nothing Then repid = Session("REPORTID").ToString().Trim()
        If repid = "" Then repid = "CurrentReport"
        Return "Regression_" & repid & "_" & settingName
    End Function

    Private Sub RestoreRegressionSelections()
        SelectDropDownSessionValue(DropDownXField, RegressionSessionKey("XField"))
        SelectDropDownSessionValue(DropDownYField, RegressionSessionKey("YField"))
        SelectDropDownSessionValue(DropDownGroupField, RegressionSessionKey("GroupField"))
        SelectDropDownSessionValue(DropDownEquationType, RegressionSessionKey("EquationType"))

        If Not Session(RegressionSessionKey("PredictX")) Is Nothing Then
            txtPredictX.Text = Session(RegressionSessionKey("PredictX")).ToString()
        End If

        If Not Session(RegressionSessionKey("Search")) Is Nothing Then
            txtSearch.Text = Session(RegressionSessionKey("Search")).ToString()
        End If
    End Sub

    Private Sub SaveRegressionSelections()
        Session(RegressionSessionKey("XField")) = DropDownXField.SelectedValue
        Session(RegressionSessionKey("YField")) = DropDownYField.SelectedValue
        Session(RegressionSessionKey("GroupField")) = DropDownGroupField.SelectedValue
        Session(RegressionSessionKey("EquationType")) = DropDownEquationType.SelectedValue
        Session(RegressionSessionKey("PredictX")) = txtPredictX.Text.Trim()
        Session(RegressionSessionKey("Search")) = txtSearch.Text.Trim()
    End Sub

    Private Sub ClearRegressionSelections()
        Session.Remove(RegressionSessionKey("XField"))
        Session.Remove(RegressionSessionKey("YField"))
        Session.Remove(RegressionSessionKey("GroupField"))
        Session.Remove(RegressionSessionKey("EquationType"))
        Session.Remove(RegressionSessionKey("PredictX"))
        Session.Remove(RegressionSessionKey("Search"))
    End Sub

    Private Sub SelectDropDownSessionValue(dropDown As DropDownList, sessionKey As String)
        If Session(sessionKey) Is Nothing Then Exit Sub

        Dim savedValue As String = Session(sessionKey).ToString()
        Dim item As ListItem = dropDown.Items.FindByValue(savedValue)
        If item IsNot Nothing Then
            dropDown.ClearSelection()
            item.Selected = True
        End If
    End Sub

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

    Private Function IsBinaryField(dt As DataTable, columnName As String) As Boolean
        If dt Is Nothing OrElse Not dt.Columns.Contains(columnName) Then Return False
        Dim values As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim valueText As String = FieldText(dt.Rows(i)(columnName)).Trim()
            If valueText = "" Then Continue For
            Dim normalized As String = NormalizeBinaryText(valueText)
            If normalized = "" Then Return False
            If Not values.ContainsKey(normalized) Then values.Add(normalized, True)
            If values.Count > 2 Then Return False
        Next
        Return values.Count > 0
    End Function

    Private Function NormalizeBinaryText(valueText As String) As String
        Dim text As String = valueText.Trim().ToLowerInvariant()
        If text = "1" OrElse text = "true" OrElse text = "yes" OrElse text = "y" OrElse text = "approved" OrElse text = "success" OrElse text = "positive" Then Return "1"
        If text = "0" OrElse text = "false" OrElse text = "no" OrElse text = "n" OrElse text = "denied" OrElse text = "fail" OrElse text = "failed" OrElse text = "negative" Then Return "0"
        Return ""
    End Function

    Private Sub SetDefaultPredictionValue(dt As DataTable)
        If DropDownXField.Items.Count = 0 Then Exit Sub

        Dim bucket As New RegressionBucket()
        For i As Integer = 0 To dt.Rows.Count - 1
            Dim xValue As Double
            If TryGetDouble(dt.Rows(i)(DropDownXField.SelectedValue), xValue) Then bucket.AddPair(xValue, 0)
        Next

        If bucket.Count > 0 Then txtPredictX.Text = FormatNumber(bucket.AverageX(), 2, TriState.True, TriState.False, TriState.False)
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub DropDownXField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownXField.SelectedIndexChanged
        SetDefaultPredictionValue(GetSourceTable())
        SaveRegressionSelections()
    End Sub

    Private Sub DropDownYField_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownYField.SelectedIndexChanged
        BuildAndBindRegression()
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindRegression()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        ClearRegressionSelections()
        If DropDownXField.Items.Count > 0 Then DropDownXField.SelectedIndex = 0
        If DropDownYField.Items.Count > 1 Then DropDownYField.SelectedIndex = 1
        If DropDownGroupField.Items.Count > 0 Then DropDownGroupField.SelectedIndex = 0
        If DropDownEquationType.Items.FindByValue("BestFit") IsNot Nothing Then DropDownEquationType.SelectedValue = "BestFit"
        txtSearch.Text = ""
        SetDefaultPredictionValue(GetSourceTable())
        BuildAndBindRegression()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportRegression("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportRegression("xls")
    End Sub

    Private Sub lnkRegressionAI_Click(sender As Object, e As EventArgs) Handles lnkRegressionAI.Click
        Dim dt As DataTable = TryCast(Session("RegressionTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRegression()
            dt = TryCast(Session("RegressionTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No regression data to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), RegressionTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret this regression analysis. Explain how the Y column changes when the X value changes, the strength of the relationship, the prediction value, and any groups that should be reviewed.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindRegression()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then Exit Sub
        SaveRegressionSelections()

        If DropDownXField.SelectedValue.Trim() = "" OrElse DropDownYField.SelectedValue.Trim() = "" Then
            LabelError.Text = "Select numeric X and Y fields."
            Exit Sub
        End If

        If DropDownXField.SelectedValue = DropDownYField.SelectedValue Then
            LabelError.Text = "X and Y fields should be different."
            Exit Sub
        End If

        Dim predictionX As Double = 0
        Dim hasPrediction As Boolean = False
        If txtPredictX.Text.Trim() <> "" Then
            hasPrediction = Double.TryParse(txtPredictX.Text.Trim(), predictionX)
            If Not hasPrediction Then
                LabelError.Text = "Prediction X value must be numeric."
                Exit Sub
            End If
        End If

        Dim output As DataTable = CreateRegressionTable(source, predictionX, hasPrediction)
        Session("RegressionTable") = output
        BindRegression(output)
    End Sub

    Private Sub BindRegression(dt As DataTable)
        BindAnalysisGrid(dt)

        If dt Is Nothing Then
            LabelInfo.Text = ""
        Else
            LabelInfo.Text = RegressionTitle() & " (" & dt.Rows.Count.ToString() & " rows)"
        End If
    End Sub

    Private Sub GridViewRegression_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewRegression.RowDataBound
        Dim dt As DataTable = TryCast(Session("RegressionTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("Trends and Predictions") Then Exit Sub

        Dim linkIndex As Integer = dt.Columns.IndexOf("Trends and Predictions")
        Dim recordsIndex As Integer = dt.Columns.IndexOf("Records")
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < e.Row.Cells.Count Then e.Row.Cells(filterIndex).Visible = False
        If linkIndex < 0 OrElse linkIndex >= e.Row.Cells.Count Then Exit Sub

        If e.Row.RowType <> System.Web.UI.WebControls.DataControlRowType.DataRow Then Exit Sub

        Dim url As String = e.Row.Cells(linkIndex).Text.Replace("&nbsp;", "").Trim()
        If url <> "" Then
            Dim link As New System.Web.UI.WebControls.HyperLink()
            link.Text = "open trends"
            link.NavigateUrl = Server.HtmlDecode(url)
            link.CssClass = "NodeStyle"
            link.ToolTip = "Open trends and predictions chart."
            e.Row.Cells(linkIndex).Controls.Clear()
            e.Row.Cells(linkIndex).Controls.Add(link)
        End If

        If recordsIndex >= 0 AndAlso filterIndex >= 0 AndAlso recordsIndex < e.Row.Cells.Count AndAlso filterIndex < e.Row.Cells.Count Then
            Dim recordsText As String = e.Row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
            Dim filterId As String = e.Row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
            If filterId <> "" Then
                Dim recordsLink As New System.Web.UI.WebControls.HyperLink()
                recordsLink.Text = recordsText
                recordsLink.NavigateUrl = "~/ShowReport.aspx?srd=0&regressionfilter=" & Server.UrlEncode(filterId)
                recordsLink.CssClass = "NodeStyle"
                recordsLink.ToolTip = "Open corresponding records in Data Explorer."
                e.Row.Cells(recordsIndex).Controls.Clear()
                e.Row.Cells(recordsIndex).Controls.Add(recordsLink)
            End If
        End If
    End Sub

    Private Function CreateRegressionTable(source As DataTable, predictionX As Double, hasPrediction As Boolean) As DataTable
        Session("RegressionFilters") = New Dictionary(Of String, String)()
        Dim buckets As New Dictionary(Of String, RegressionBucket)(StringComparer.OrdinalIgnoreCase)
        Dim groups As New SortedDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To source.Rows.Count - 1
            Dim dr As DataRow = source.Rows(i)
            If Not RowMatchesSearch(dr, txtSearch.Text.Trim()) Then Continue For

            Dim xValue As Double
            Dim yValue As Double
            If Not TryGetDouble(dr(DropDownXField.SelectedValue), xValue) Then Continue For
            If Not TryGetYValue(dr(DropDownYField.SelectedValue), yValue) Then Continue For

            Dim groupText As String = AllGroupsValue
            If DropDownGroupField.SelectedValue.Trim() <> "" Then groupText = FieldText(dr(DropDownGroupField.SelectedValue))

            If Not groups.ContainsKey(groupText) Then groups.Add(groupText, True)
            If Not buckets.ContainsKey(groupText) Then buckets.Add(groupText, New RegressionBucket())
            buckets(groupText).AddPair(xValue, yValue, dr)
        Next

        Dim output As New DataTable()
        output.Columns.Add("Group", GetType(String))
        output.Columns.Add("X Field", GetType(String))
        output.Columns.Add("Y Field", GetType(String))
        output.Columns.Add("Records", GetType(Integer))
        output.Columns.Add("Equation Type", GetType(String))
        output.Columns.Add("Equation", GetType(String))
        output.Columns.Add("Slope", GetType(String))
        output.Columns.Add("Intercept", GetType(String))
        output.Columns.Add("Coefficients", GetType(String))
        output.Columns.Add("Correlation", GetType(String))
        output.Columns.Add("R Squared", GetType(String))
        output.Columns.Add("Average X", GetType(String))
        output.Columns.Add("Average Y", GetType(String))
        output.Columns.Add("Min X", GetType(String))
        output.Columns.Add("Max X", GetType(String))
        output.Columns.Add("Predicted Y", GetType(String))
        output.Columns.Add("Trends and Predictions", GetType(String))
        output.Columns.Add("FilterId", GetType(String))

        For Each groupText As String In groups.Keys
            Dim bucket As RegressionBucket = buckets(groupText)
            AddRegressionRow(output, groupText, bucket, predictionX, hasPrediction)
        Next

        Return output
    End Function

    Private Sub AddRegressionRow(output As DataTable, groupText As String, bucket As RegressionBucket, predictionX As Double, hasPrediction As Boolean)
        Dim fit As RegressionFit = BuildFit(bucket)
        Dim correlationValue As Double = bucket.Correlation()
        Dim predictedText As String = ""
        If hasPrediction AndAlso fit IsNot Nothing AndAlso fit.IsValid AndAlso fit.Predictor IsNot Nothing Then
            Dim predictedValue As Double = fit.Predictor(predictionX)
            If Not Double.IsNaN(predictedValue) AndAlso Not Double.IsInfinity(predictedValue) Then predictedText = FormatNumber(predictedValue, 4)
        End If

        Dim outRow As DataRow = output.NewRow()
        outRow("Group") = groupText
        outRow("X Field") = DropDownXField.SelectedValue
        outRow("Y Field") = DropDownYField.SelectedValue
        outRow("Records") = bucket.Count
        outRow("Equation Type") = fit.ModelName
        outRow("Equation") = fit.Equation
        outRow("Slope") = If(fit.Slope.HasValue, FormatNumber(fit.Slope.Value, 4), "")
        outRow("Intercept") = If(fit.Intercept.HasValue, FormatNumber(fit.Intercept.Value, 4), "")
        outRow("Coefficients") = fit.Coefficients
        outRow("Correlation") = FormatNumber(correlationValue, 4)
        outRow("R Squared") = If(fit.IsValid, FormatNumber(fit.RSquared, 4), "")
        outRow("Average X") = FormatNumber(bucket.AverageX(), 4)
        outRow("Average Y") = FormatNumber(bucket.AverageY(), 4)
        outRow("Min X") = FormatNumber(bucket.MinX, 4)
        outRow("Max X") = FormatNumber(bucket.MaxX, 4)
        outRow("Predicted Y") = predictedText
        outRow("Trends and Predictions") = If(fit.IsValid, TrendsUrl(outRow("Equation").ToString(), predictionX, hasPrediction, groupText, DropDownXField.SelectedValue, DropDownYField.SelectedValue), "")
        outRow("FilterId") = RegisterRegressionFilter(bucket.Rows)
        output.Rows.Add(outRow)
    End Sub

    Private Function RegressionEquation(interceptValue As Double, slopeValue As Double) As String
        If IsZero(slopeValue) Then Return "Y = " & FormatForEquation(interceptValue)
        Dim signText As String = " + "
        If slopeValue < 0 Then signText = " - "
        Return "Y = " & FormatForEquation(interceptValue) & signText & FormatForEquation(Abs(slopeValue)) & " * X"
    End Function

    Private Function BuildFit(bucket As RegressionBucket) As RegressionFit
        Dim selectedModel As String = DropDownEquationType.SelectedValue
        Dim fits As New List(Of RegressionFit)()

        If selectedModel = "BestFit" OrElse selectedModel = "Linear" Then fits.Add(LinearFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Quadratic" Then fits.Add(PolynomialFit(bucket, 2, "Quadratic"))
        If selectedModel = "BestFit" OrElse selectedModel = "Cubic" Then fits.Add(PolynomialFit(bucket, 3, "Cubic"))
        If selectedModel = "BestFit" OrElse selectedModel = "Exponential" Then fits.Add(ExponentialFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Logarithmic" Then fits.Add(LogarithmicFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Power" Then fits.Add(PowerFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Logistic" Then fits.Add(LogisticFit(bucket))

        Dim best As RegressionFit = Nothing
        For Each fit As RegressionFit In fits
            If fit Is Nothing OrElse Not fit.IsValid Then Continue For
            If best Is Nothing OrElse fit.RSquared > best.RSquared Then best = fit
        Next

        If best IsNot Nothing Then Return best
        If selectedModel <> "BestFit" Then Return InvalidFit(selectedModel, bucket)
        Return LinearFit(bucket)
    End Function

    Private Function InvalidFit(selectedModel As String, bucket As RegressionBucket) As RegressionFit
        Dim fit As New RegressionFit()
        fit.ModelName = ModelDisplayName(selectedModel)
        fit.IsValid = False
        fit.RSquared = 0

        If selectedModel = "Logistic" Then
            If bucket.Count < 3 Then
                fit.Equation = "Logistic Probability requires at least 3 records."
            Else
                fit.Equation = "Logistic Probability requires Y values that are binary, such as 1/0, yes/no, true/false, or success/failed."
            End If
        Else
            fit.Equation = fit.ModelName & " could not be calculated for the selected data."
        End If

        Return fit
    End Function

    Private Function ModelDisplayName(selectedModel As String) As String
        Select Case selectedModel
            Case "BestFit"
                Return "Best Fit"
            Case "Linear"
                Return "Linear"
            Case "Quadratic"
                Return "Quadratic"
            Case "Cubic"
                Return "Cubic"
            Case "Exponential"
                Return "Exponential"
            Case "Logarithmic"
                Return "Logarithmic"
            Case "Power"
                Return "Power"
            Case "Logistic"
                Return "Logistic Probability"
            Case Else
                Return selectedModel
        End Select
    End Function

    Private Function LinearFit(bucket As RegressionBucket) As RegressionFit
        Dim slopeValue As Double = bucket.Slope()
        Dim interceptValue As Double = bucket.Intercept()
        Dim fit As New RegressionFit()
        fit.ModelName = "Linear"
        fit.Slope = slopeValue
        fit.Intercept = interceptValue
        fit.Equation = RegressionEquation(interceptValue, slopeValue)
        fit.Coefficients = "Intercept=" & FormatForEquation(interceptValue) & "; Slope=" & FormatForEquation(slopeValue)
        fit.Predictor = Function(x As Double) interceptValue + slopeValue * x
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = bucket.Count >= 2
        Return fit
    End Function

    Private Function PolynomialFit(bucket As RegressionBucket, degree As Integer, modelName As String) As RegressionFit
        If bucket.Count < degree + 1 Then Return Nothing

        Dim matrix(degree, degree) As Double
        Dim vector(degree) As Double

        For r As Integer = 0 To degree
            For c As Integer = 0 To degree
                Dim sumValue As Double = 0
                For i As Integer = 0 To bucket.Count - 1
                    sumValue += Pow(bucket.XValues(i), r + c)
                Next
                matrix(r, c) = sumValue
            Next

            Dim ySum As Double = 0
            For i As Integer = 0 To bucket.Count - 1
                ySum += bucket.YValues(i) * Pow(bucket.XValues(i), r)
            Next
            vector(r) = ySum
        Next

        Dim coefficients() As Double = SolveLinearSystem(matrix, vector)
        If coefficients Is Nothing Then Return Nothing

        Dim fit As New RegressionFit()
        fit.ModelName = modelName
        fit.Intercept = coefficients(0)
        fit.Slope = If(coefficients.Length > 1, coefficients(1), 0)
        fit.Equation = PolynomialEquation(coefficients)
        fit.Coefficients = CoefficientText(coefficients)
        fit.Predictor = Function(x As Double) EvaluatePolynomial(coefficients, x)
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = True
        Return fit
    End Function

    Private Function ExponentialFit(bucket As RegressionBucket) As RegressionFit
        Dim xs As New List(Of Double)()
        Dim ys As New List(Of Double)()
        For i As Integer = 0 To bucket.Count - 1
            If bucket.YValues(i) > 0 Then
                xs.Add(bucket.XValues(i))
                ys.Add(Log(bucket.YValues(i)))
            End If
        Next
        If xs.Count < 2 Then Return Nothing

        Dim parts As Double() = LinearCoefficients(xs, ys)
        If parts Is Nothing Then Return Nothing
        Dim a As Double = Exp(parts(0))
        Dim b As Double = parts(1)

        Dim fit As New RegressionFit()
        fit.ModelName = "Exponential"
        fit.Intercept = a
        fit.Slope = b
        If IsZero(b) Then
            fit.Equation = "Y = " & FormatForEquation(a)
        ElseIf IsZero(a) Then
            fit.Equation = "Y = 0"
        Else
            fit.Equation = "Y = " & FormatForEquation(a) & " * exp(" & FormatForEquation(b) & " * X)"
        End If
        fit.Coefficients = "A=" & FormatForEquation(a) & "; B=" & FormatForEquation(b)
        fit.Predictor = Function(x As Double) a * Exp(b * x)
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = True
        Return fit
    End Function

    Private Function LogarithmicFit(bucket As RegressionBucket) As RegressionFit
        Dim xs As New List(Of Double)()
        Dim ys As New List(Of Double)()
        For i As Integer = 0 To bucket.Count - 1
            If bucket.XValues(i) > 0 Then
                xs.Add(Log(bucket.XValues(i)))
                ys.Add(bucket.YValues(i))
            End If
        Next
        If xs.Count < 2 Then Return Nothing

        Dim parts As Double() = LinearCoefficients(xs, ys)
        If parts Is Nothing Then Return Nothing
        Dim a As Double = parts(0)
        Dim b As Double = parts(1)

        Dim fit As New RegressionFit()
        fit.ModelName = "Logarithmic"
        fit.Intercept = a
        fit.Slope = b
        If IsZero(b) Then fit.Equation = "Y = " & FormatForEquation(a) Else fit.Equation = "Y = " & FormatForEquation(a) & SignedTerm(b, " * log(X)")
        fit.Coefficients = "A=" & FormatForEquation(a) & "; B=" & FormatForEquation(b)
        fit.Predictor = Function(x As Double) If(x > 0, a + b * Log(x), Double.NaN)
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = True
        Return fit
    End Function

    Private Function PowerFit(bucket As RegressionBucket) As RegressionFit
        Dim xs As New List(Of Double)()
        Dim ys As New List(Of Double)()
        For i As Integer = 0 To bucket.Count - 1
            If bucket.XValues(i) > 0 AndAlso bucket.YValues(i) > 0 Then
                xs.Add(Log(bucket.XValues(i)))
                ys.Add(Log(bucket.YValues(i)))
            End If
        Next
        If xs.Count < 2 Then Return Nothing

        Dim parts As Double() = LinearCoefficients(xs, ys)
        If parts Is Nothing Then Return Nothing
        Dim a As Double = Exp(parts(0))
        Dim b As Double = parts(1)

        Dim fit As New RegressionFit()
        fit.ModelName = "Power"
        fit.Intercept = a
        fit.Slope = b
        If IsZero(b) Then
            fit.Equation = "Y = " & FormatForEquation(a)
        ElseIf IsZero(a) Then
            fit.Equation = "Y = 0"
        Else
            fit.Equation = "Y = " & FormatForEquation(a) & " * pow(X," & FormatForEquation(b) & ")"
        End If
        fit.Coefficients = "A=" & FormatForEquation(a) & "; B=" & FormatForEquation(b)
        fit.Predictor = Function(x As Double) If(x > 0, a * Pow(x, b), Double.NaN)
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = True
        Return fit
    End Function

    Private Function LogisticFit(bucket As RegressionBucket) As RegressionFit
        If bucket.Count < 3 OrElse Not BucketYIsBinary(bucket) Then Return Nothing

        Dim meanX As Double = bucket.AverageX()
        Dim stdX As Double = 0
        For i As Integer = 0 To bucket.Count - 1
            stdX += Pow(bucket.XValues(i) - meanX, 2)
        Next
        stdX = Sqrt(stdX / bucket.Count)
        If stdX = 0 Then Return Nothing

        Dim a As Double = 0
        Dim b As Double = 0
        Dim learningRate As Double = 0.08

        For iteration As Integer = 1 To 4000
            Dim gradA As Double = 0
            Dim gradB As Double = 0
            For i As Integer = 0 To bucket.Count - 1
                Dim xn As Double = (bucket.XValues(i) - meanX) / stdX
                Dim p As Double = LogisticValue(a + b * xn)
                Dim err As Double = bucket.YValues(i) - p
                gradA += err
                gradB += err * xn
            Next
            a += learningRate * gradA / bucket.Count
            b += learningRate * gradB / bucket.Count
        Next

        Dim rawB As Double = b / stdX
        Dim rawA As Double = a - (b * meanX / stdX)

        Dim fit As New RegressionFit()
        fit.ModelName = "Logistic Probability"
        fit.Intercept = rawA
        fit.Slope = rawB
        If IsZero(rawB) Then
            fit.Equation = "Y = 1 / (1 + exp(-(" & FormatForEquation(rawA) & ")))"
        Else
            fit.Equation = "Y = 1 / (1 + exp(-(" & FormatForEquation(rawA) & SignedTerm(rawB, " * X") & ")))"
        End If
        fit.Coefficients = "Intercept=" & FormatForEquation(rawA) & "; Slope=" & FormatForEquation(rawB)
        fit.Predictor = Function(x As Double) LogisticValue(rawA + rawB * x)
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = True
        Return fit
    End Function

    Private Function BucketYIsBinary(bucket As RegressionBucket) As Boolean
        Dim hasZero As Boolean = False
        Dim hasOne As Boolean = False
        For i As Integer = 0 To bucket.YValues.Count - 1
            If Abs(bucket.YValues(i)) < 0.0000001 Then
                hasZero = True
            ElseIf Abs(bucket.YValues(i) - 1) < 0.0000001 Then
                hasOne = True
            Else
                Return False
            End If
        Next
        Return hasZero AndAlso hasOne
    End Function

    Private Function LogisticValue(z As Double) As Double
        If z > 35 Then Return 1
        If z < -35 Then Return 0
        Return 1 / (1 + Exp(-z))
    End Function

    Private Function LinearCoefficients(xs As List(Of Double), ys As List(Of Double)) As Double()
        Dim n As Integer = xs.Count
        If n < 2 OrElse ys.Count <> n Then Return Nothing
        Dim sumX As Double = 0
        Dim sumY As Double = 0
        Dim sumXX As Double = 0
        Dim sumXY As Double = 0
        For i As Integer = 0 To n - 1
            sumX += xs(i)
            sumY += ys(i)
            sumXX += xs(i) * xs(i)
            sumXY += xs(i) * ys(i)
        Next
        Dim denominator As Double = n * sumXX - sumX * sumX
        If denominator = 0 Then Return Nothing
        Dim slopeValue As Double = (n * sumXY - sumX * sumY) / denominator
        Dim interceptValue As Double = (sumY - slopeValue * sumX) / n
        Return New Double() {interceptValue, slopeValue}
    End Function

    Private Function SolveLinearSystem(matrix As Double(,), vector As Double()) As Double()
        Dim n As Integer = vector.Length
        Dim a(n - 1, n) As Double
        For r As Integer = 0 To n - 1
            For c As Integer = 0 To n - 1
                a(r, c) = matrix(r, c)
            Next
            a(r, n) = vector(r)
        Next

        For pivot As Integer = 0 To n - 1
            Dim bestRow As Integer = pivot
            For r As Integer = pivot + 1 To n - 1
                If Abs(a(r, pivot)) > Abs(a(bestRow, pivot)) Then bestRow = r
            Next
            If Abs(a(bestRow, pivot)) < 0.0000000001 Then Return Nothing
            If bestRow <> pivot Then
                For c As Integer = pivot To n
                    Dim temp As Double = a(pivot, c)
                    a(pivot, c) = a(bestRow, c)
                    a(bestRow, c) = temp
                Next
            End If

            Dim divider As Double = a(pivot, pivot)
            For c As Integer = pivot To n
                a(pivot, c) /= divider
            Next

            For r As Integer = 0 To n - 1
                If r = pivot Then Continue For
                Dim factor As Double = a(r, pivot)
                For c As Integer = pivot To n
                    a(r, c) -= factor * a(pivot, c)
                Next
            Next
        Next

        Dim result(n - 1) As Double
        For r As Integer = 0 To n - 1
            result(r) = a(r, n)
        Next
        Return result
    End Function

    Private Function CalculateRSquared(bucket As RegressionBucket, predictor As Func(Of Double, Double)) As Double
        If bucket.Count < 2 OrElse predictor Is Nothing Then Return 0
        Dim meanY As Double = bucket.AverageY()
        Dim ssTotal As Double = 0
        Dim ssResidual As Double = 0
        For i As Integer = 0 To bucket.Count - 1
            Dim predicted As Double = predictor(bucket.XValues(i))
            If Double.IsNaN(predicted) OrElse Double.IsInfinity(predicted) Then Continue For
            ssTotal += Pow(bucket.YValues(i) - meanY, 2)
            ssResidual += Pow(bucket.YValues(i) - predicted, 2)
        Next
        If ssTotal = 0 Then Return 0
        Dim r2 As Double = 1 - (ssResidual / ssTotal)
        If r2 < 0 Then r2 = 0
        If r2 > 1 Then r2 = 1
        Return r2
    End Function

    Private Function PolynomialEquation(coefficients() As Double) As String
        Dim text As String = "Y = "
        Dim hasTerm As Boolean = False
        If coefficients.Length > 0 AndAlso Not IsZero(coefficients(0)) Then
            text &= FormatForEquation(coefficients(0))
            hasTerm = True
        End If
        For i As Integer = 1 To coefficients.Length - 1
            If IsZero(coefficients(i)) Then Continue For
            Dim variableText As String = " * X"
            If i > 1 Then variableText = " * X"
            If i = 2 Then variableText = " * X * X"
            If i = 3 Then variableText = " * X * X * X"
            If hasTerm Then
                text &= SignedTerm(coefficients(i), variableText)
            Else
                If coefficients(i) < 0 Then text &= "-" & FormatForEquation(Abs(coefficients(i))) & variableText Else text &= FormatForEquation(coefficients(i)) & variableText
                hasTerm = True
            End If
        Next
        If Not hasTerm Then text &= "0"
        Return text
    End Function

    Private Function EvaluatePolynomial(coefficients() As Double, x As Double) As Double
        Dim total As Double = 0
        For i As Integer = 0 To coefficients.Length - 1
            total += coefficients(i) * Pow(x, i)
        Next
        Return total
    End Function

    Private Function CoefficientText(coefficients() As Double) As String
        Dim parts As New List(Of String)()
        For i As Integer = 0 To coefficients.Length - 1
            parts.Add("A" & i.ToString() & "=" & FormatForEquation(coefficients(i)))
        Next
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Function SignedTerm(value As Double, suffix As String) As String
        If value < 0 Then Return " - " & FormatForEquation(Abs(value)) & suffix
        Return " + " & FormatForEquation(value) & suffix
    End Function

    Private Function IsZero(value As Double) As Boolean
        Return Abs(value) < 0.0000000001
    End Function

    Private Function FormatForEquation(value As Double) As String
        Return value.ToString("0.########", Globalization.CultureInfo.InvariantCulture)
    End Function

    Private Function TrendsUrl(equationText As String, predictionX As Double, hasPrediction As Boolean, groupText As String, xField As String, yField As String) As String
        Dim xValue As String = ""
        If hasPrediction Then xValue = predictionX.ToString(Globalization.CultureInfo.InvariantCulture)
        Return "Trends.aspx?Equation=" & Server.UrlEncode(equationText) &
            "&XValue=" & Server.UrlEncode(xValue) &
            "&Group=" & Server.UrlEncode(groupText) &
            "&XField=" & Server.UrlEncode(xField) &
            "&YField=" & Server.UrlEncode(yField)
    End Function

    Private Function RegisterRegressionFilter(rows As List(Of DataRow)) As String
        Dim rowFilter As String = RowsFilter(rows)
        If rowFilter.Trim() = "" Then Return ""

        Dim filters As Dictionary(Of String, String) = TryCast(Session("RegressionFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("RegressionFilters") = filters
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

    Private Function TryGetDouble(valueObject As Object, ByRef numericValue As Double) As Boolean
        numericValue = 0
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return False
        Dim valueText As String = valueObject.ToString().Trim()
        If valueText = "" Then Return False
        Return Double.TryParse(valueText, numericValue)
    End Function

    Private Function TryGetYValue(valueObject As Object, ByRef numericValue As Double) As Boolean
        If TryGetDouble(valueObject, numericValue) Then Return True
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return False
        Dim normalized As String = NormalizeBinaryText(valueObject.ToString())
        If normalized = "" Then Return False
        numericValue = If(normalized = "1", 1, 0)
        Return True
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

    Private Function RegressionTitle() As String
        Return "Regression: predict " & DropDownYField.SelectedValue & " from " & DropDownXField.SelectedValue
    End Function

    Private Sub ExportRegression(formatName As String)
        Dim dt As DataTable = TryCast(Session("RegressionTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindRegression()
            dt = TryCast(Session("RegressionTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No regression data to export."
            Exit Sub
        End If

        Dim fileName As String = "RegressionAnalysis_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        If formatName = "csv" Then
            Response.Clear()
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", RegressionTitle(), ""))
            Response.End()
        Else
            Response.Clear()
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), RegressionTitle(), ""))
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
            GridViewRegression.AllowPaging = False
            GridViewRegression.PageIndex = 0
            GridViewRegression.DataSource = Nothing
            GridViewRegression.DataBind()
            UpdateAnalysisPager(Nothing)
            SetAnalysisExplanationLabels()
            Return
        End If

        GridViewRegression.AllowPaging = (dt.Rows.Count > AnalysisGridPageSize)
        GridViewRegression.PageSize = AnalysisGridPageSize
        If Not GridViewRegression.AllowPaging Then
            GridViewRegression.PageIndex = 0
        ElseIf GridViewRegression.PageIndex < 0 OrElse GridViewRegression.PageIndex >= GridViewRegression.PageCount Then
            GridViewRegression.PageIndex = 0
        End If

        GridViewRegression.DataSource = dt
        GridViewRegression.DataBind()
        UpdateAnalysisPager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub HideAnalysisInternalColumns(ByVal dt As DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") OrElse GridViewRegression.HeaderRow Is Nothing Then Return
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < GridViewRegression.HeaderRow.Cells.Count Then
            GridViewRegression.HeaderRow.Cells(filterIndex).Visible = False
        End If
    End Sub
    Private Sub UpdateAnalysisPager(ByVal dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > AnalysisGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewRegression.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewRegression.PageIndex < (GridViewRegression.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewRegression.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewRegression.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewRegression.PageIndex > 0 Then GridViewRegression.PageIndex -= 1
        BindAnalysisGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session(AnalysisGridSessionKey()), DataTable)
        If dt Is Nothing Then Return
        If GridViewRegression.PageIndex < (GridViewRegression.PageCount - 1) Then GridViewRegression.PageIndex += 1
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
            GridViewRegression.PageIndex = requestedPage - 1
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
        LabelModelExplanation.Text = "Model: Regression and prediction analysis for numeric relationships. Inputs are selected X field, Y field, optional group field, equation type, and optional prediction X value."
        LabelAlgorithmExplanation.Text = "Algorithm: The page collects numeric X/Y pairs, optionally separates them by group, fits the selected model type, calculates coefficients and prediction values, and creates a trend link with the equation and selected fields."
        LabelOutputExplanation.Text = "Output: The grid shows group, X field, Y field, equation, coefficient details, predicted Y where requested, record count, and a Trends link. Record links open the rows used to fit each equation."
    End Sub
End Class
