Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Math
Imports System.Text
Imports System.Web.UI.WebControls

Partial Class OpenTrends
    Inherits System.Web.UI.Page

    Private Const TrendGridPageSize As Integer = 30

    Private Class TrendPoint
        Public X As Double
        Public Y As Double
    End Class

    Private Class TrendBucket
        Public Points As New List(Of TrendPoint)()
        Public SumX As Double
        Public SumY As Double
        Public SumXX As Double
        Public SumYY As Double
        Public SumXY As Double
        Public MinX As Double
        Public MaxX As Double

        Public ReadOnly Property Count As Integer
            Get
                Return Points.Count
            End Get
        End Property

        Public Sub AddPoint(xValue As Double, yValue As Double)
            Points.Add(New TrendPoint() With {.X = xValue, .Y = yValue})
            SumX += xValue
            SumY += yValue
            SumXX += xValue * xValue
            SumYY += yValue * yValue
            SumXY += xValue * yValue
            If Points.Count = 1 Then
                MinX = xValue
                MaxX = xValue
            Else
                If xValue < MinX Then MinX = xValue
                If xValue > MaxX Then MaxX = xValue
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
    End Class

    Private Class TrendFit
        Public ModelName As String
        Public Equation As String
        Public Coefficients As String
        Public RSquared As Double
        Public Slope As Nullable(Of Double)
        Public Intercept As Nullable(Of Double)
        Public IsValid As Boolean
        Public Predictor As Func(Of Double, Double)
    End Class

    Private Sub OpenTrends_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then
            LabelPageTtl.Text = Session("PAGETTL").ToString()
        End If

        Dim reportTitle As String = SessionText("REPTITLE")
        If reportTitle <> "" Then
            lblHeader.Text = reportTitle & " - Open Trends from Chart"
        ElseIf SessionText("REPORTID") <> "" Then
            lblHeader.Text = SessionText("REPORTID") & " - Open Trends from Chart"
        End If
        SetChartHeaderLabels()
    End Sub

    Private Sub OpenTrends_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        If Not IsPostBack Then
            BuildAndBindOpenTrends()
        ElseIf Session("OpenTrendsTable") IsNot Nothing Then
            BindTrendGrid(CType(Session("OpenTrendsTable"), DataTable))
        End If
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindOpenTrends()
    End Sub

    Private Sub ButtonBack_Click(sender As Object, e As EventArgs) Handles ButtonBack.Click
        If SessionText("REPORTID") <> "" Then
            Response.Redirect("~/ChartGoogleOne.aspx?Report=" & Server.UrlEncode(SessionText("REPORTID")) & "&domulti=yes")
        Else
            Response.Redirect("~/ChartGoogleOne.aspx")
        End If
    End Sub

    Private Sub lnkOpenTrendsAI_Click(sender As Object, e As EventArgs) Handles lnkOpenTrendsAI.Click
        Dim dt As DataTable = TryCast(Session("OpenTrendsTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindOpenTrends()
            dt = TryCast(Session("OpenTrendsTable"), DataTable)
        End If

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            LabelError.Text = "No trend data to send to AI."
            Exit Sub
        End If

        Dim aiTable As DataTable = GridTableForAI(dt)
        Session("dataTable") = aiTable
        Session("OriginalDataTable") = Session("dataTable")
        Session("DataToChatAI") = ExportToCSVtext(aiTable, Chr(9), OpenTrendsTitle(), "")
        Session("QuestionToAI") = BuildAnalysisQuestion("Interpret these trend equations created from the current chart data. Explain which series are increasing or decreasing, the strength of the relationship, and which trends should be reviewed.")
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub BuildAndBindOpenTrends()
        LabelError.Text = ""
        LabelInfo.Text = ""
        SetChartHeaderLabels()
        SetAnalysisExplanationLabels()

        Dim chartType As String = SessionText("ChartType")
        If chartType = "" Then chartType = "Chart"

        Dim trendEligibility As OpenTrendsEligibility = OpenTrendsSupport.Evaluate(chartType, SessionText("arr"))
        If Not trendEligibility.CanProduce Then
            LabelError.Text = trendEligibility.Reason
            BindTrendGrid(CreateOpenTrendsTable())
            Exit Sub
        End If

        Dim output As DataTable = BuildTrendTable(trendEligibility.Rows, chartType)
        If output.Rows.Count = 0 AndAlso LabelError.Text.Trim() = "" Then
            LabelError.Text = "No numeric chart series were found for trend analysis."
        End If

        Session("OpenTrendsTable") = output
        BindTrendGrid(output)
    End Sub

    Private Function BuildTrendTable(rows As List(Of List(Of String)), chartType As String) As DataTable
        Dim output As DataTable = CreateOpenTrendsTable()
        Dim header As List(Of String) = rows(0)
        If header.Count < 2 Then Return output

        Dim seriesColumns As New List(Of Integer)()
        For i As Integer = 1 To header.Count - 1
            If Not OpenTrendsSupport.IsRoleColumn(header(i)) Then seriesColumns.Add(i)
        Next

        If seriesColumns.Count = 0 Then Return output

        Dim xField As String = HeaderText(header(0), "Chart X")
        Dim xMode As String = ""
        Dim xValues As List(Of Double) = ResolveXValues(rows, xMode)
        Dim defaultX As Double = If(xValues.Count > 0, xValues(xValues.Count - 1), 0)
        If Not IsPostBack AndAlso txtPredictX.Text.Trim() = "" AndAlso xValues.Count > 0 Then
            txtPredictX.Text = FormatForEquation(defaultX)
        End If

        Dim predictionX As Double = defaultX
        If txtPredictX.Text.Trim() <> "" AndAlso Not OpenTrendsSupport.TryGetDouble(txtPredictX.Text.Trim(), predictionX) Then
            LabelError.Text = "Predict Y when X is must be numeric."
            Return output
        End If

        For Each seriesIndex As Integer In seriesColumns
            Dim bucket As New TrendBucket()
            For r As Integer = 1 To rows.Count - 1
                If r - 1 >= xValues.Count Then Continue For
                If seriesIndex >= rows(r).Count Then Continue For
                Dim yValue As Double
                If OpenTrendsSupport.TryGetDouble(rows(r)(seriesIndex), yValue) Then
                    bucket.AddPoint(xValues(r - 1), yValue)
                End If
            Next

            If bucket.Count < 2 Then Continue For

            Dim fit As TrendFit = BuildFit(bucket)
            Dim correlationValue As Double = bucket.Correlation()
            Dim yField As String = HeaderText(header(seriesIndex), "Series " & seriesIndex.ToString())
            Dim predictedText As String = ""
            If fit IsNot Nothing AndAlso fit.IsValid AndAlso fit.Predictor IsNot Nothing Then
                Dim predictedValue As Double = fit.Predictor(predictionX)
                If Not Double.IsNaN(predictedValue) AndAlso Not Double.IsInfinity(predictedValue) Then
                    predictedText = FormatNumber(predictedValue, 4)
                End If
            End If

            Dim outRow As DataRow = output.NewRow()
            outRow("X Field") = If(xMode = "", xField, xField & " (" & xMode & ")")
            outRow("Y Field / Series") = yField
            outRow("Aggregation") = SessionText("AggregateM")
            outRow("Records") = bucket.Count
            outRow("Equation Type") = If(fit Is Nothing, "", fit.ModelName)
            outRow("Equation") = If(fit Is Nothing, "", fit.Equation)
            outRow("Slope") = If(fit IsNot Nothing AndAlso fit.Slope.HasValue, FormatNumber(fit.Slope.Value, 4), "")
            outRow("Intercept") = If(fit IsNot Nothing AndAlso fit.Intercept.HasValue, FormatNumber(fit.Intercept.Value, 4), "")
            outRow("Coefficients") = If(fit Is Nothing, "", fit.Coefficients)
            outRow("Correlation") = FormatNumber(correlationValue, 4)
            outRow("R Squared") = If(fit IsNot Nothing AndAlso fit.IsValid, FormatNumber(fit.RSquared, 4), "")
            outRow("Average X") = FormatNumber(bucket.AverageX(), 4)
            outRow("Average Y") = FormatNumber(bucket.AverageY(), 4)
            outRow("Min X") = FormatNumber(bucket.MinX, 4)
            outRow("Max X") = FormatNumber(bucket.MaxX, 4)
            outRow("Predicted Y") = predictedText
            outRow("Trends and Predictions") = If(fit IsNot Nothing AndAlso fit.IsValid, TrendsUrl(fit.Equation, predictionX, SessionText("ttl"), If(xMode = "", xField, xMode), yField), "")
            output.Rows.Add(outRow)
        Next

        LabelInfo.Text = "Trend rows are calculated from the current chart data in memory. Best Fit compares supported equation types by R squared. For category labels, X is chart row order."
        Return output
    End Function

    Private Function CreateOpenTrendsTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("X Field", GetType(String))
        dt.Columns.Add("Y Field / Series", GetType(String))
        dt.Columns.Add("Aggregation", GetType(String))
        dt.Columns.Add("Records", GetType(Integer))
        dt.Columns.Add("Equation Type", GetType(String))
        dt.Columns.Add("Equation", GetType(String))
        dt.Columns.Add("Slope", GetType(String))
        dt.Columns.Add("Intercept", GetType(String))
        dt.Columns.Add("Coefficients", GetType(String))
        dt.Columns.Add("Correlation", GetType(String))
        dt.Columns.Add("R Squared", GetType(String))
        dt.Columns.Add("Average X", GetType(String))
        dt.Columns.Add("Average Y", GetType(String))
        dt.Columns.Add("Min X", GetType(String))
        dt.Columns.Add("Max X", GetType(String))
        dt.Columns.Add("Predicted Y", GetType(String))
        dt.Columns.Add("Trends and Predictions", GetType(String))
        Return dt
    End Function

    Private Function ResolveXValues(rows As List(Of List(Of String)), ByRef xMode As String) As List(Of Double)
        Dim values As New List(Of Double)()
        Dim allNumeric As Boolean = True
        Dim allDates As Boolean = True

        For r As Integer = 1 To rows.Count - 1
            Dim text As String = If(rows(r).Count > 0, rows(r)(0), "")
            Dim numericValue As Double
            If Not TryGetDouble(text, numericValue) Then allNumeric = False
            Dim dateValue As DateTime
            If Not DateTime.TryParse(text, dateValue) Then allDates = False
        Next

        For r As Integer = 1 To rows.Count - 1
            Dim text As String = If(rows(r).Count > 0, rows(r)(0), "")
            If allNumeric Then
                Dim numericValue As Double
                Double.TryParse(CleanValue(text), NumberStyles.Any, CultureInfo.InvariantCulture, numericValue)
                values.Add(numericValue)
            ElseIf allDates Then
                Dim dateValue As DateTime
                DateTime.TryParse(text, dateValue)
                values.Add(dateValue.ToOADate())
            Else
                values.Add(r)
            End If
        Next

        If allNumeric Then
            xMode = "numeric X"
        ElseIf allDates Then
            xMode = "date serial X"
        Else
            xMode = "chart row order"
        End If
        Return values
    End Function

    Private Function ParseGoogleArray(arrText As String) As List(Of List(Of String))
        Dim rows As New List(Of List(Of String))()
        Dim currentRow As List(Of String) = Nothing
        Dim value As New StringBuilder()
        Dim inQuote As Boolean = False
        Dim quoteChar As Char = ChrW(0)
        Dim braceDepth As Integer = 0

        For i As Integer = 0 To arrText.Length - 1
            Dim ch As Char = arrText(i)
            If inQuote Then
                If ch = quoteChar Then
                    inQuote = False
                Else
                    value.Append(ch)
                End If
            Else
                If ch = "'"c OrElse ch = """"c Then
                    inQuote = True
                    quoteChar = ch
                ElseIf ch = "["c Then
                    currentRow = New List(Of String)()
                    value.Length = 0
                    braceDepth = 0
                ElseIf ch = "{"c Then
                    braceDepth += 1
                    value.Append(ch)
                ElseIf ch = "}"c Then
                    If braceDepth > 0 Then braceDepth -= 1
                    value.Append(ch)
                ElseIf ch = ","c AndAlso currentRow IsNot Nothing AndAlso braceDepth = 0 Then
                    currentRow.Add(CleanValue(value.ToString()))
                    value.Length = 0
                ElseIf ch = "]"c AndAlso currentRow IsNot Nothing Then
                    currentRow.Add(CleanValue(value.ToString()))
                    rows.Add(currentRow)
                    currentRow = Nothing
                    value.Length = 0
                Else
                    value.Append(ch)
                End If
            End If
        Next

        Return rows
    End Function

    Private Function CleanValue(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Dim text As String = valueText.Trim()
        If text.StartsWith("'") AndAlso text.EndsWith("'") AndAlso text.Length >= 2 Then text = text.Substring(1, text.Length - 2)
        If text.StartsWith("""") AndAlso text.EndsWith("""") AndAlso text.Length >= 2 Then text = text.Substring(1, text.Length - 2)
        Return text.Trim()
    End Function

    Private Function IsRoleColumn(headerText As String) As Boolean
        If headerText Is Nothing Then Return False
        Dim text As String = headerText.ToLowerInvariant()
        Return text.Contains("role") OrElse text.Contains("style") OrElse text.Contains("tooltip")
    End Function

    Private Function HeaderText(headerValue As String, fallbackText As String) As String
        If headerValue Is Nothing OrElse headerValue.Trim() = "" OrElse IsRoleColumn(headerValue) Then Return fallbackText
        Return headerValue.Trim()
    End Function

    Private Function TrendAllowed(chartType As String) As Boolean
        Dim allowed As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
            "LineChart",
            "AreaChart",
            "SteppedAreaChart",
            "ScatterChart",
            "ColumnChart",
            "Column",
            "BarChart",
            "ComboChart"
        }
        Return allowed.Contains(chartType)
    End Function

    Private Function TryGetDouble(valueObject As Object, ByRef numericValue As Double) As Boolean
        numericValue = 0
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return False
        Dim valueText As String = CleanValue(valueObject.ToString())
        If valueText = "" Then Return False
        Return Double.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, numericValue) OrElse Double.TryParse(valueText, numericValue)
    End Function

    Private Function BuildFit(bucket As TrendBucket) As TrendFit
        Dim selectedModel As String = DropDownEquationType.SelectedValue
        If selectedModel = "" Then selectedModel = "BestFit"

        Dim fits As New List(Of TrendFit)()
        If selectedModel = "BestFit" OrElse selectedModel = "Linear" Then fits.Add(LinearFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Quadratic" Then fits.Add(PolynomialFit(bucket, 2, "Quadratic"))
        If selectedModel = "BestFit" OrElse selectedModel = "Cubic" Then fits.Add(PolynomialFit(bucket, 3, "Cubic"))
        If selectedModel = "BestFit" OrElse selectedModel = "Exponential" Then fits.Add(ExponentialFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Logarithmic" Then fits.Add(LogarithmicFit(bucket))
        If selectedModel = "BestFit" OrElse selectedModel = "Power" Then fits.Add(PowerFit(bucket))

        Dim best As TrendFit = Nothing
        For Each fit As TrendFit In fits
            If fit Is Nothing OrElse Not fit.IsValid Then Continue For
            If best Is Nothing OrElse fit.RSquared > best.RSquared Then best = fit
        Next

        If best IsNot Nothing Then Return best
        If selectedModel <> "BestFit" Then Return InvalidFit(selectedModel)
        Return LinearFit(bucket)
    End Function

    Private Function InvalidFit(selectedModel As String) As TrendFit
        Dim fit As New TrendFit()
        fit.ModelName = ModelDisplayName(selectedModel)
        fit.Equation = fit.ModelName & " could not be calculated for the chart data."
        fit.IsValid = False
        fit.RSquared = 0
        Return fit
    End Function

    Private Function ModelDisplayName(selectedModel As String) As String
        Select Case selectedModel
            Case "BestFit"
                Return "Best Fit"
            Case "Linear", "Quadratic", "Cubic", "Exponential", "Logarithmic", "Power"
                Return selectedModel
            Case Else
                Return selectedModel
        End Select
    End Function

    Private Function LinearFit(bucket As TrendBucket) As TrendFit
        Dim slopeValue As Double = bucket.Slope()
        Dim interceptValue As Double = bucket.Intercept()
        Dim fit As New TrendFit()
        fit.ModelName = "Linear"
        fit.Slope = slopeValue
        fit.Intercept = interceptValue
        fit.Equation = PolynomialEquation(New Double() {interceptValue, slopeValue})
        fit.Coefficients = "Intercept=" & FormatForEquation(interceptValue) & "; Slope=" & FormatForEquation(slopeValue)
        fit.Predictor = Function(x As Double) interceptValue + slopeValue * x
        fit.RSquared = CalculateRSquared(bucket, fit.Predictor)
        fit.IsValid = bucket.Count >= 2
        Return fit
    End Function

    Private Function PolynomialFit(bucket As TrendBucket, degree As Integer, modelName As String) As TrendFit
        If bucket.Count < degree + 1 Then Return Nothing

        Dim matrix(degree, degree) As Double
        Dim vector(degree) As Double

        For r As Integer = 0 To degree
            For c As Integer = 0 To degree
                Dim sumValue As Double = 0
                For i As Integer = 0 To bucket.Count - 1
                    sumValue += Pow(bucket.Points(i).X, r + c)
                Next
                matrix(r, c) = sumValue
            Next

            Dim ySum As Double = 0
            For i As Integer = 0 To bucket.Count - 1
                ySum += bucket.Points(i).Y * Pow(bucket.Points(i).X, r)
            Next
            vector(r) = ySum
        Next

        Dim coefficients() As Double = SolveLinearSystem(matrix, vector)
        If coefficients Is Nothing Then Return Nothing

        Dim fit As New TrendFit()
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

    Private Function ExponentialFit(bucket As TrendBucket) As TrendFit
        Dim xs As New List(Of Double)()
        Dim ys As New List(Of Double)()
        For Each point As TrendPoint In bucket.Points
            If point.Y > 0 Then
                xs.Add(point.X)
                ys.Add(Log(point.Y))
            End If
        Next
        If xs.Count < 2 Then Return Nothing

        Dim parts As Double() = LinearCoefficients(xs, ys)
        If parts Is Nothing Then Return Nothing
        Dim a As Double = Exp(parts(0))
        Dim b As Double = parts(1)

        Dim fit As New TrendFit()
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

    Private Function LogarithmicFit(bucket As TrendBucket) As TrendFit
        Dim xs As New List(Of Double)()
        Dim ys As New List(Of Double)()
        For Each point As TrendPoint In bucket.Points
            If point.X > 0 Then
                xs.Add(Log(point.X))
                ys.Add(point.Y)
            End If
        Next
        If xs.Count < 2 Then Return Nothing

        Dim parts As Double() = LinearCoefficients(xs, ys)
        If parts Is Nothing Then Return Nothing
        Dim a As Double = parts(0)
        Dim b As Double = parts(1)

        Dim fit As New TrendFit()
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

    Private Function PowerFit(bucket As TrendBucket) As TrendFit
        Dim xs As New List(Of Double)()
        Dim ys As New List(Of Double)()
        For Each point As TrendPoint In bucket.Points
            If point.X > 0 AndAlso point.Y > 0 Then
                xs.Add(Log(point.X))
                ys.Add(Log(point.Y))
            End If
        Next
        If xs.Count < 2 Then Return Nothing

        Dim parts As Double() = LinearCoefficients(xs, ys)
        If parts Is Nothing Then Return Nothing
        Dim a As Double = Exp(parts(0))
        Dim b As Double = parts(1)

        Dim fit As New TrendFit()
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

    Private Function CalculateRSquared(bucket As TrendBucket, predictor As Func(Of Double, Double)) As Double
        If bucket.Count < 2 OrElse predictor Is Nothing Then Return 0
        Dim meanY As Double = bucket.AverageY()
        Dim ssTotal As Double = 0
        Dim ssResidual As Double = 0
        Dim usedRecords As Integer = 0
        For Each point As TrendPoint In bucket.Points
            Dim predicted As Double = predictor(point.X)
            If Double.IsNaN(predicted) OrElse Double.IsInfinity(predicted) Then Continue For
            ssTotal += Pow(point.Y - meanY, 2)
            ssResidual += Pow(point.Y - predicted, 2)
            usedRecords += 1
        Next
        If usedRecords < 2 OrElse ssTotal = 0 Then Return 0
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
        Return value.ToString("0.########", CultureInfo.InvariantCulture)
    End Function

    Private Function TrendsUrl(equationText As String, xValue As Double, groupText As String, xField As String, yField As String) As String
        Return "Trends.aspx?Equation=" & Server.UrlEncode(equationText) &
            "&XValue=" & Server.UrlEncode(xValue.ToString(CultureInfo.InvariantCulture)) &
            "&Group=" & Server.UrlEncode(groupText) &
            "&XField=" & Server.UrlEncode(xField) &
            "&YField=" & Server.UrlEncode(yField)
    End Function

    Private Sub BindTrendGrid(dt As DataTable)
        Session("OpenTrendsTable") = dt
        GridViewOpenTrends.AllowPaging = (dt IsNot Nothing AndAlso dt.Rows.Count > TrendGridPageSize)
        GridViewOpenTrends.PageSize = TrendGridPageSize
        If Not GridViewOpenTrends.AllowPaging Then GridViewOpenTrends.PageIndex = 0
        GridViewOpenTrends.DataSource = dt
        GridViewOpenTrends.DataBind()
        UpdatePager(dt)
        SetAnalysisExplanationLabels()
    End Sub

    Private Sub GridViewOpenTrends_RowDataBound(sender As Object, e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewOpenTrends.RowDataBound
        Dim dt As DataTable = TryCast(Session("OpenTrendsTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("Trends and Predictions") Then Exit Sub
        Dim linkIndex As Integer = dt.Columns.IndexOf("Trends and Predictions")
        If linkIndex < 0 OrElse linkIndex >= e.Row.Cells.Count Then Exit Sub
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim url As String = e.Row.Cells(linkIndex).Text.Replace("&nbsp;", "").Trim()
        If url <> "" Then
            Dim link As New System.Web.UI.WebControls.HyperLink()
            link.Text = "open trends"
            link.NavigateUrl = Server.HtmlDecode(url)
            link.CssClass = "NodeStyle"
            link.ToolTip = "Open this chart series in Trends and Predictions."
            e.Row.Cells(linkIndex).Controls.Clear()
            e.Row.Cells(linkIndex).Controls.Add(link)
        End If
    End Sub

    Protected Sub LinkButtonPrevious_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session("OpenTrendsTable"), DataTable)
        If dt Is Nothing Then Return
        If GridViewOpenTrends.PageIndex > 0 Then GridViewOpenTrends.PageIndex -= 1
        BindTrendGrid(dt)
    End Sub

    Protected Sub LinkButtonNext_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session("OpenTrendsTable"), DataTable)
        If dt Is Nothing Then Return
        If GridViewOpenTrends.PageIndex < (GridViewOpenTrends.PageCount - 1) Then GridViewOpenTrends.PageIndex += 1
        BindTrendGrid(dt)
    End Sub

    Protected Sub TextBoxPageNumber_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim dt As DataTable = TryCast(Session("OpenTrendsTable"), DataTable)
        If dt Is Nothing Then Return
        Dim requestedPage As Integer
        If Integer.TryParse(TextBoxPageNumber.Text, requestedPage) Then
            If requestedPage < 1 Then requestedPage = 1
            Dim pageCount As Integer = Math.Max(1, CInt(Math.Ceiling(dt.Rows.Count / CDbl(TrendGridPageSize))))
            If requestedPage > pageCount Then requestedPage = pageCount
            GridViewOpenTrends.PageIndex = requestedPage - 1
        End If
        BindTrendGrid(dt)
    End Sub

    Private Sub UpdatePager(dt As DataTable)
        Dim hasPages As Boolean = (dt IsNot Nothing AndAlso dt.Rows.Count > TrendGridPageSize)
        LinkButtonPrevious.Visible = hasPages AndAlso GridViewOpenTrends.PageIndex > 0
        LinkButtonNext.Visible = hasPages AndAlso GridViewOpenTrends.PageIndex < (GridViewOpenTrends.PageCount - 1)
        LabelPageNumberCaption.Visible = hasPages
        TextBoxPageNumber.Visible = hasPages
        LabelPageCount.Visible = hasPages
        If hasPages Then
            TextBoxPageNumber.Text = (GridViewOpenTrends.PageIndex + 1).ToString()
            LabelPageCount.Text = " of " & GridViewOpenTrends.PageCount.ToString()
        Else
            TextBoxPageNumber.Text = ""
            LabelPageCount.Text = ""
        End If
    End Sub

    Private Function GridTableForAI(dt As DataTable) As DataTable
        If dt Is Nothing Then Return Nothing
        Return dt.Copy()
    End Function

    Private Function BuildAnalysisQuestion(baseQuestion As String) As String
        SetAnalysisExplanationLabels()
        Dim parts As New List(Of String)()
        parts.Add(baseQuestion)
        If LabelAnalysisSubtitle IsNot Nothing AndAlso LabelAnalysisSubtitle.Text.Trim() <> "" Then parts.Add("Input: " & LabelAnalysisSubtitle.Text.Trim())
        Return String.Join(vbCrLf & vbCrLf, parts.ToArray())
    End Function

    Private Sub SetAnalysisExplanationLabels()
        LabelModelExplanation.Text = "Model: Chart trend analysis uses the currently generated Google Chart data and calculates a trend equation for each numeric Y series. Best Fit compares Linear, Quadratic, Cubic, Exponential, Logarithmic, and Power equations and keeps the strongest supported model by R squared."
        LabelAlgorithmExplanation.Text = "Algorithm: The page reads Session(""arr""), treats the first chart column as X, ignores role/style columns, converts numeric or date X labels when possible, uses chart row order for category labels, tests the selected equation family, and calculates prediction at the requested X value."
        LabelOutputExplanation.Text = "Output: The chart title and chart type are shown above the grid. The grid shows X field, Y series, aggregation, records, selected equation type, equation, slope, intercept, coefficients, correlation, R squared, prediction at the selected X value, and an open trends link for each series."
    End Sub

    Private Function OpenTrendsTitle() As String
        Dim title As String = SessionText("ttl")
        If title = "" Then title = "Open Trends from Chart"
        Return title
    End Function

    Private Sub SetChartHeaderLabels()
        Dim titleText As String = SessionText("ttl")
        Dim chartType As String = SessionText("ChartType")
        LabelChartTitle.Text = If(titleText.Trim() <> "", "Chart Title: " & titleText, "Chart Title: current chart")
        LabelChartType.Text = If(chartType.Trim() <> "", "Chart Type: " & chartType, "Chart Type: chart")
    End Sub

    Private Function SessionText(key As String) As String
        If Session Is Nothing OrElse Session(key) Is Nothing Then Return ""
        Return Session(key).ToString()
    End Function
End Class
