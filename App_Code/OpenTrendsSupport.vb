Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text

Public Class OpenTrendsEligibility
    Public Property CanProduce As Boolean
    Public Property Reason As String
    Public Property Rows As List(Of List(Of String))
End Class

Public NotInheritable Class OpenTrendsSupport
    Private Sub New()
    End Sub

    Public Shared Function Evaluate(chartType As String, arrText As String) As OpenTrendsEligibility
        Dim result As New OpenTrendsEligibility()
        result.Rows = New List(Of List(Of String))()

        If Not TrendAllowed(chartType) Then
            Dim chartLabel As String = If(chartType, "").Trim()
            If chartLabel = "" Then chartLabel = "this chart"
            result.Reason = "Open Trends is not available for " & chartLabel & ". Use Line, Area, Stepped Area, Scatter, Column, Bar, or Combo charts."
            Return result
        End If

        If arrText Is Nothing OrElse arrText.Trim() = "" Then
            result.Reason = "No chart data was found. Open or refresh a chart first."
            Return result
        End If

        result.Rows = ParseGoogleArray(arrText)
        If result.Rows.Count < 2 Then
            result.Reason = "Chart data does not contain enough rows for trend analysis."
            Return result
        End If

        If Not HasNumericTrendSeries(result.Rows) Then
            result.Reason = "No numeric chart series were found for trend analysis."
            Return result
        End If

        result.CanProduce = True
        result.Reason = ""
        Return result
    End Function

    Public Shared Function TrendAllowed(chartType As String) As Boolean
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
        Return allowed.Contains(If(chartType, ""))
    End Function

    Public Shared Function ParseGoogleArray(arrText As String) As List(Of List(Of String))
        Dim rows As New List(Of List(Of String))()
        If arrText Is Nothing Then Return rows

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

    Public Shared Function CleanValue(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Dim text As String = valueText.Trim()
        If text.StartsWith("'") AndAlso text.EndsWith("'") AndAlso text.Length >= 2 Then text = text.Substring(1, text.Length - 2)
        If text.StartsWith("""") AndAlso text.EndsWith("""") AndAlso text.Length >= 2 Then text = text.Substring(1, text.Length - 2)
        Return text.Trim()
    End Function

    Public Shared Function IsRoleColumn(headerText As String) As Boolean
        If headerText Is Nothing Then Return False
        Dim text As String = headerText.ToLowerInvariant()
        Return text.Contains("role") OrElse text.Contains("style") OrElse text.Contains("tooltip")
    End Function

    Public Shared Function TryGetDouble(valueObject As Object, ByRef numericValue As Double) As Boolean
        numericValue = 0
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return False
        Dim valueText As String = CleanValue(valueObject.ToString())
        If valueText = "" Then Return False
        Return Double.TryParse(valueText, NumberStyles.Any, CultureInfo.InvariantCulture, numericValue) OrElse Double.TryParse(valueText, numericValue)
    End Function

    Private Shared Function HasNumericTrendSeries(rows As List(Of List(Of String))) As Boolean
        If rows Is Nothing OrElse rows.Count < 2 OrElse rows(0).Count < 2 Then Return False
        Dim header As List(Of String) = rows(0)

        For seriesIndex As Integer = 1 To header.Count - 1
            If IsRoleColumn(header(seriesIndex)) Then Continue For

            Dim numericCount As Integer = 0
            For r As Integer = 1 To rows.Count - 1
                If seriesIndex >= rows(r).Count Then Continue For
                Dim yValue As Double
                If TryGetDouble(rows(r)(seriesIndex), yValue) Then numericCount += 1
                If numericCount >= 2 Then Return True
            Next
        Next

        Return False
    End Function
End Class
