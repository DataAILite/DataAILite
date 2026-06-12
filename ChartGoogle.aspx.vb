Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.Services
Partial Class ChartGoogle
    Inherits System.Web.UI.Page
    Public arr As String
    Public ttl As String
    Public srt As String
    Public y1 As String
    Public x1 As String
    Public x2 As String
    Public arrCount As String
    Public ttlCount As String
    Public arrDistCount As String
    Public ttlDistCount As String
    Public arrValue As String
    Public ttlValue As String
    Public arrSum As String
    Public ttlSum As String
    Public arrAvg As String
    Public ttlAvg As String
    Public arrStDev As String
    Public ttlStDev As String
    Public arrMax As String
    Public ttlMax As String
    Public arrMin As String
    Public ttlMin As String
    Public charttype As String
    Public chartpckg As String
    Public nv As Integer

    Private Sub ChartGoogle_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        LinkButtonBack.OnClientClick = "showSpinner();return true;"
        lnkbtnAvg.OnClientClick = "showSpinner();return true;"
        lnkbtnCount.OnClientClick = "showSpinner();return true;"
        lnkbtnDistCount.OnClientClick = "showSpinner();return true;"
        lnkbtnMax.OnClientClick = "showSpinner();return true;"
        lnkbtnMin.OnClientClick = "showSpinner();return true;"
        lnkbtnStDev.OnClientClick = "showSpinner();return true;"
        lnkbtnSum.OnClientClick = "showSpinner();return true;"
        lnkbtnValue.OnClientClick = "showSpinner();return true;"
        DropDownChartType.Attributes.Add("onchange", "showSpinner();")
        nv = 8
        If Not Session("nv") Is Nothing And IsNumeric(Session("nv")) Then
            nv = Session("nv")
        End If
        Session("StatDash") = "yes"
        charttype = DropDownChartType.Text
        chartpckg = ChartPackageForType(charttype)
        Session("arr") = ""
        arr = "['Mushrooms', 2],"
        arr = arr & "['Onions', 2],"
        arr = arr & "['Olives', 2],"
        arr = arr & "['Zucchini', 0],"
        arr = arr & "['Pepperoni', 3]"
        ttl = "How Much Pizza Anthony Ate Last Night"
        If Not IsPostBack AndAlso Request("map") IsNot Nothing AndAlso Request("map").ToString.Trim = "yes" Then
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("Map")
            DropDownChartType.Items.Add("GeoChart")
        Else
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("PieChart")
            DropDownChartType.Items.Add("BarChart")
            DropDownChartType.Items.Add("LineChart")
            DropDownChartType.Items.Add("AreaChart")
            DropDownChartType.Items.Add("SteppedAreaChart")
            DropDownChartType.Items.Add("ScatterChart")
            DropDownChartType.Items.Add("ComboChart")
            'DropDownChartType.Items.Add("BubbleChart")
            DropDownChartType.Items.Add("ColumnChart")
            DropDownChartType.Items.Add("Histogram")
            'DropDownChartType.Items.Add("Gauge")
            'DropDownChartType.Items.Add("Sankey")
            'DropDownChartType.Items.Add("CandlestickChart")
            'DropDownChartType.Items.Add("Waterfall")
        End If
        If Request("charttype") IsNot Nothing AndAlso Request("charttype").ToString.Trim <> "" Then
            DropDownChartType.Text = Request("charttype").ToString.Trim
            Session("ChartType") = DropDownChartType.Text
        ElseIf Session("ChartType") IsNot Nothing AndAlso Session("ChartType").ToString.Trim <> "" Then
            DropDownChartType.Text = Session("ChartType")
        End If
        charttype = DropDownChartType.Text
        chartpckg = ChartPackageForType(charttype)
        If nv < 8 Then
            Value_chart_div.Visible = False
            Sum_chart_div.Visible = False
            Avg_chart_div.Visible = False
            StDev_chart_div.Visible = False
            Max_chart_div.Visible = False
            Min_chart_div.Visible = False
            lnkbtnValue.Visible = False
            lnkbtnValue.Enabled = False
            lnkbtnSum.Visible = False
            lnkbtnSum.Enabled = False
            lnkbtnAvg.Visible = False
            lnkbtnAvg.Enabled = False
            lnkbtnStDev.Visible = False
            lnkbtnStDev.Enabled = False
            lnkbtnMax.Visible = False
            lnkbtnMax.Enabled = False
            lnkbtnMin.Visible = False
            lnkbtnMin.Enabled = False
        End If
        If nv = 3 Then
            Value_chart_div.Visible = True
            lnkbtnValue.Visible = True
            lnkbtnValue.Enabled = True
        End If
        If Session("WhereStm") Is Nothing Then
            Session("WhereStm") = ""
        End If
    End Sub

    Private Sub ChartGoogle_Load(sender As Object, e As EventArgs) Handles Me.Load
        charttype = DropDownChartType.SelectedItem.Text
        chartpckg = ChartPackageForType(charttype)
        Session("ChartType") = charttype
        Dim repid As String = String.Empty
        If Request("Report") Is Nothing OrElse Request("Report").ToString.Trim = "" Then
            Exit Sub
        Else
            repid = Request("Report").ToString
        End If
        Session("arr") = ""
        If Not IsPostBack Then
            Session("arrCount") = ""
            Session("arrDistCount") = ""
            Session("arrSum") = ""
            Session("arrAvg") = ""
            Session("arrStDev") = ""
            Session("arrMax") = ""
            Session("arrMin") = ""
            Session("arrValue") = ""
        Else
            arrCount = Session("arrCount")
            arrDistCount = Session("arrDistCount")
            arrSum = Session("arrSum")
            arrAvg = Session("arrAvg")
            arrStDev = Session("arrStDev")
            arrMax = Session("arrMax")
            arrMin = Session("arrMin")
            arrValue = Session("arrValue")
        End If
        Dim ret As String = String.Empty
        Dim dt As DataTable
        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            Exit Sub
        End If
        Try
            Dim sqlq As String = dri.Rows(0)("SQLquerytext").ToString

            LabelWhere.Text = dri.Rows(0)("ReportTtl").ToString & " _____ " & Session("WhereStm").ToString.Trim

            x1 = String.Empty
            x2 = String.Empty
            y1 = String.Empty
            Dim fn As String = String.Empty
            If Request("x1") IsNot Nothing Then
                x1 = Request("x1").ToString
            End If
            If Request("x2") IsNot Nothing Then
                x2 = Request("x2").ToString
            End If
            If Request("y1") IsNot Nothing Then
                y1 = Request("y1").ToString
            End If
            If Request("fn") IsNot Nothing AndAlso Request("fn") <> "Value" Then
                fn = Request("fn").ToString
            End If
            Session("cat1") = x1
            Session("cat2") = x2
            Session("AxisY") = y1
            Session("Aggregate") = fn

            srt = x1 & ", " & x2
            If x1 = x2 Then
                srt = x1
            End If

            Dim er As String = String.Empty
            Dim rt As String = String.Empty
            If x1 <> x2 AndAlso Not IsPostBack Then
                rt = AddGroupBy(Session("REPORTID"), x1, x2, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
            End If

            Dim selflds As String = x1 & "," & x2 & "," & y1
            selflds = FixSelectedFields(repid, selflds, Session("UserConnString"), Session("UserConnProvider"))
            Dim xx1, xx2, yy1, ssrt As String
            xx1 = Piece(selflds, ",", 1)
            xx2 = Piece(selflds, ",", 2)
            yy1 = Piece(selflds, ",", 3)

            ssrt = xx1 & ", " & xx2
            If x1 = x2 Then
                ssrt = xx1
                lnkbtnReverse.Visible = False
                lnkbtnReverse.Enabled = False
            Else
                lnkbtnReverse.Visible = True
                lnkbtnReverse.Enabled = True
            End If

            Dim msql As String = String.Empty
            Dim i As Integer
            Dim grp As String = String.Empty

            arrValue = Session("arrValue")
            ttlValue = "Value of [" & y1 & "] in group by [" & srt & "]"

            arrCount = Session("arrCount")
            ttlCount = "Count of records in group by [" & srt & "]"

            arrDistCount = Session("arrDistCount")
            ttlDistCount = "Distinct Count of [" & y1 & "] in group by [" & srt & "]"

            arrSum = Session("arrSum")
            ttlSum = "Sum of [" & y1 & "] in group by [" & srt & "]"

            arrAvg = Session("arrAvg")
            ttlAvg = "Avg of [" & y1 & "] in group by [" & srt & "]"

            arrStDev = Session("arrStDev")
            ttlStDev = "StDev of [" & y1 & "] in group by [" & srt & "]"

            arrMax = Session("arrMax")
            ttlMax = "Max of [" & y1 & "] in group by [" & srt & "]"

            arrMin = Session("arrMin")
            ttlMin = "Min of [" & y1 & "] in group by [" & srt & "]"

            If Not IsPostBack Then

                Dim dv3 As DataView
                If Session("dv3") Is Nothing Then
                    'retrieve dv3
                    dv3 = RetrieveReportData(repid, Session("WhereStm").ToString, True, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret)
                    If dv3 Is Nothing OrElse dv3.Table.Rows.Count = 0 Then
                        LabelError.Text = "No data"
                        Exit Sub
                    End If
                    Session("dv3") = dv3
                Else
                    dv3 = Session("dv3")
                End If
                If dv3 Is Nothing Then
                    LabelError.Text = ret
                    Exit Sub
                End If

                'calc stats and make arrs
                'Dim dta As DataTable
                For Each fn In {"Count", "DistCount", "Sum", "AVG", "MAX", "MIN", "StDev"}
                    If nv <= 3 AndAlso Not fn.Contains("Count") Then
                        Exit For
                    End If
                    arr = ""
                    Dim dta As New DataTable
                    'dta = ComputeStats(dv3.Table, fn, y1, x1, x2, Session("WhereStm").ToString.Trim, ret, Session("UserConnString"), Session("UserConnProvider"))
                    dta = ComputeStats(dv3.Table, fn, y1, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))

                    If dta Is Nothing Then
                        LabelError.Text = LabelError.Text & " " & ret
                        Continue For
                    End If
                    For i = 0 To dta.Rows.Count - 1
                        If x1 = x2 Then
                            grp = dta.Rows(i)(x1).ToString
                        Else
                            grp = dta.Rows(i)(x1).ToString & "," & dta.Rows(i)(x2).ToString
                        End If
                        arr = arr & "['" & grp & "'," & dta.Rows(i)("ARR").ToString & "]"
                        If i < dta.Rows.Count - 1 Then arr = arr & ","
                    Next
                    If fn = "Count" Then
                        arrCount = arr
                    ElseIf fn = "DistCount" Then
                        arrDistCount = arr
                    ElseIf fn = "Sum" Then
                        arrSum = arr
                    ElseIf fn = "AVG" Then
                        arrAvg = arr
                    ElseIf fn = "MAX" Then
                        arrMax = arr
                    ElseIf fn = "MIN" Then
                        arrMin = arr
                    ElseIf fn = "StDev" Then
                        arrStDev = arr
                    End If
                Next

                dt = dv3.ToTable
                If nv > 2 AndAlso ColumnTypeIsNumeric(dt.Columns(y1)) Then
                    'calc Value and arrValue
                    For i = 0 To dt.Rows.Count - 1
                        If x1 = x2 Then
                            grp = dt.Rows(i)(x1).ToString
                        Else
                            grp = dt.Rows(i)(x1).ToString & "," & dt.Rows(i)(x2).ToString
                        End If
                        arrValue = arrValue & "['" & grp & "'," & dt.Rows(i)(y1).ToString & "]"
                        If i < dt.Rows.Count - 1 Then arrValue = arrValue & ","
                    Next
                End If

                If nv = 2 Then
                    arrSum = ""
                    arrAvg = ""
                    arrStDev = ""
                    arrMax = ""
                    arrMin = ""
                    arrValue = ""
                ElseIf nv = 3 Then
                    arrSum = ""
                    arrAvg = ""
                    arrStDev = ""
                    arrMax = ""
                    arrMin = ""
                End If
                Session("arrCount") = arrCount
                Session("arrDistCount") = arrDistCount
                Session("arrSum") = arrSum
                Session("arrAvg") = arrAvg
                Session("arrStDev") = arrStDev
                Session("arrMax") = arrMax
                Session("arrMin") = arrMin
                Session("arrValue") = arrValue

            End If
        Catch ex As Exception
            ret = ex.Message
            LabelError.Text = ret
            'Session("arrCount") = ""
            'Session("arrDistCount") = ""
            'Session("arrSum") = ""
            'Session("arrAvg") = ""
            'Session("arrStDev") = ""
            'Session("arrMax") = ""
            'Session("arrMin") = ""
            'Session("arrValue") = ""
        End Try

        RegisterChartGoogleExportSnapshot(repid)

    End Sub

    Private Function ChartPackageForType(chartTypeText As String) As String
        If chartTypeText = "MapChart" OrElse chartTypeText = "Map" Then
            Return "Map"
        ElseIf chartTypeText = "GeoChart" Then
            Return "geochart"
        ElseIf chartTypeText = "Gauge" Then
            Return "gauge"
        ElseIf chartTypeText = "Sankey" Then
            Return "sankey"
        End If
        Return "corechart"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LogDashboardChartCaptureStatusForExport(sectionName As String, statusText As String) As String
        If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then Return "no-session"
        Return DashboardUploadStatus(HttpContext.Current.Session, sectionName, statusText)
    End Function

    Private Sub RegisterChartGoogleExportSnapshot(repid As String)
        If Session Is Nothing Then Exit Sub
        If repid Is Nothing OrElse repid.Trim() = "" Then Exit Sub
        If Not HasAnyChartData() Then Exit Sub

        Try
            Dim labelText As String = ChartGoogleLabelText()
            Dim signature As String = ChartGoogleSnapshotSignature(labelText)
            Dim snapshots As DataTable = AnalysisExportSnapshot.SnapshotTable(Session)
            Dim dataSnapshotExists As Boolean = False
            For Each row As DataRow In snapshots.Rows
                If snapshots.Columns.Contains("Signature") AndAlso String.Equals(row("Signature").ToString(), signature, StringComparison.OrdinalIgnoreCase) Then
                    dataSnapshotExists = True
                    Exit For
                End If
            Next

            Dim folderPath As String = ChartGoogleSnapshotFolder()
            If folderPath.Trim() = "" Then Exit Sub
            Directory.CreateDirectory(folderPath)

            If Not dataSnapshotExists Then
                Dim stamp As String = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                Dim fileName As String = SafeFilePartLocal("ChartDashboardData_" & repid & "_" & stamp) & ".html"
                Dim filePath As String = Path.Combine(folderPath, fileName)
                File.WriteAllText(filePath, BuildChartGoogleSnapshotHtml(labelText), Encoding.UTF8)

                Dim snapshotRow As DataRow = snapshots.NewRow()
                snapshotRow("Key") = "ChartGoogle_" & stamp
                snapshotRow("Included") = True
                snapshotRow("Package Item") = "Chart Dashboard Data File - " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                snapshotRow("Label Above Grid") = labelText
                snapshotRow("File") = fileName
                snapshotRow("Description") = "Separate HTML export created from ChartGoogle dashboard chart-ready data and page selections."
                snapshotRow("FullPath") = filePath
                If snapshots.Columns.Contains("Signature") Then snapshotRow("Signature") = signature
                snapshots.Rows.Add(snapshotRow)
            End If
        Catch ex As Exception
            If Not ex.Message.StartsWith("Thread ") Then LabelError.Text = "ERROR!! " & ex.Message
        End Try
    End Sub

    Private Function HasAnyChartData() As Boolean
        Return FieldTextLocal(arrCount).Trim() <> "" OrElse
            FieldTextLocal(arrDistCount).Trim() <> "" OrElse
            FieldTextLocal(arrValue).Trim() <> "" OrElse
            FieldTextLocal(arrSum).Trim() <> "" OrElse
            FieldTextLocal(arrAvg).Trim() <> "" OrElse
            FieldTextLocal(arrStDev).Trim() <> "" OrElse
            FieldTextLocal(arrMax).Trim() <> "" OrElse
            FieldTextLocal(arrMin).Trim() <> ""
    End Function

    Private Function ChartGoogleLabelText() As String
        Dim sb As New StringBuilder()
        sb.Append("Chart Dashboard. ")
        sb.Append("Report: " & FieldTextLocal(Session("REPTITLE")) & ". ")
        sb.Append("Chart type: " & charttype & ". ")
        sb.Append("Category field(s): " & srt & ". ")
        sb.Append("Value field: " & y1 & ". ")
        If FieldTextLocal(Session("WhereStm")).Trim() <> "" Then sb.Append("Filter: " & FieldTextLocal(Session("WhereStm")).Trim() & ". ")
        sb.Append("The export package item includes chart-ready grouped data for the visible dashboard tiles.")
        Return sb.ToString()
    End Function

    Private Function BuildChartGoogleSnapshotHtml(labelText As String) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" /><title>Chart Dashboard</title>")
        sb.AppendLine("<style>pre{white-space:pre-wrap;word-wrap:break-word;overflow-wrap:anywhere;max-width:100%;} body{font-family:Arial;}</style>")
        sb.AppendLine("</head><body>")
        sb.AppendLine("<h2>Chart Dashboard</h2>")
        sb.AppendLine("<table border=""1"" cellspacing=""0"" cellpadding=""4"">")
        AppendInfoRow(sb, "Created", DateTime.Now.ToString())
        AppendInfoRow(sb, "Report", FieldTextLocal(Session("REPORTID")))
        AppendInfoRow(sb, "Report Title", FieldTextLocal(Session("REPTITLE")))
        AppendInfoRow(sb, "Chart Type", charttype)
        AppendInfoRow(sb, "Category Field(s)", srt)
        AppendInfoRow(sb, "Value Field", y1)
        AppendInfoRow(sb, "Label Above Grid", labelText)
        sb.AppendLine("</table>")
        sb.AppendLine("<p>This file contains the server-side chart-ready data used by ChartGoogle.aspx. The browser renders the visual charts with Google Charts from these grouped values.</p>")
        AppendChartSection(sb, "Count", ttlCount, arrCount)
        AppendChartSection(sb, "Distinct Count", ttlDistCount, arrDistCount)
        AppendChartSection(sb, "Value", ttlValue, arrValue)
        AppendChartSection(sb, "Sum", ttlSum, arrSum)
        AppendChartSection(sb, "Average", ttlAvg, arrAvg)
        AppendChartSection(sb, "Standard Deviation", ttlStDev, arrStDev)
        AppendChartSection(sb, "Maximum", ttlMax, arrMax)
        AppendChartSection(sb, "Minimum", ttlMin, arrMin)
        sb.AppendLine("</body></html>")
        Return sb.ToString()
    End Function

    Private Sub AppendChartSection(sb As StringBuilder, sectionName As String, titleText As String, arrayText As String)
        If FieldTextLocal(arrayText).Trim() = "" Then Exit Sub
        sb.AppendLine("<h3>" & HtmlEncodeText(sectionName) & "</h3>")
        sb.AppendLine("<p><b>" & HtmlEncodeText(titleText) & "</b></p>")
        sb.AppendLine("<pre>" & HtmlEncodeText(arrayText) & "</pre>")
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function SaveDashboardChartImageChunkForExport(sectionName As String, titleText As String, chartTypeText As String, imageHeader As String, chunkText As String, chunkIndex As Integer, totalChunks As Integer) As String
        If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then Return "no-session"
        Dim session = HttpContext.Current.Session
        If imageHeader Is Nothing OrElse Not imageHeader.StartsWith("data:image", StringComparison.OrdinalIgnoreCase) Then Return DashboardUploadStatus(session, sectionName, "invalid-image-header")
        If sectionName Is Nothing OrElse sectionName.Trim() = "" Then Return DashboardUploadStatus(session, sectionName, "blank-section")
        If chunkText Is Nothing Then chunkText = ""
        If totalChunks <= 0 OrElse chunkIndex < 0 OrElse chunkIndex >= totalChunks Then Return DashboardUploadStatus(session, sectionName, "invalid-chunk-index")

        Try
            Dim labelText As String = ChartGoogleLabelTextShared(session)
            Dim chartData As String = DashboardChartArrayForSection(session, sectionName)
            If chartData.Trim() = "" Then chartData = "Dashboard image only: " & sectionName.Trim() & ". " & FieldTextLocalShared(titleText)
            Dim uploadKey As String = ChartGoogleSnapshotSignatureShared("imageUpload", sectionName, titleText, labelText, chartData)
            Dim chunksKey As String = "ChartGoogleDashboardImageChunks_" & uploadKey
            Dim receivedKey As String = "ChartGoogleDashboardImageReceived_" & uploadKey

            Dim chunks As String() = TryCast(session(chunksKey), String())
            If chunks Is Nothing OrElse chunks.Length <> totalChunks Then
                chunks = New String(totalChunks - 1) {}
                session(chunksKey) = chunks
                session(receivedKey) = 0
            End If

            If chunks(chunkIndex) Is Nothing Then
                chunks(chunkIndex) = chunkText
                Dim received As Integer = 0
                If session(receivedKey) IsNot Nothing AndAlso IsNumeric(session(receivedKey).ToString()) Then received = CInt(session(receivedKey))
                received += 1
                session(receivedKey) = received
            End If

            Dim receivedCount As Integer = 0
            If session(receivedKey) IsNot Nothing AndAlso IsNumeric(session(receivedKey).ToString()) Then receivedCount = CInt(session(receivedKey))
            If receivedCount < totalChunks Then Return "chunk"

            Dim sb As New StringBuilder()
            For i As Integer = 0 To totalChunks - 1
                If chunks(i) Is Nothing Then Return "chunk"
                sb.Append(chunks(i))
            Next

            Dim result As String = SaveDashboardChartImageForExport(session, sectionName, titleText, chartTypeText, imageHeader & sb.ToString(), chartData, labelText)
            session.Remove(chunksKey)
            session.Remove(receivedKey)
            DashboardUploadStatus(session, sectionName, result)
            Return result
        Catch ex As Exception
            Return DashboardUploadStatus(session, sectionName, "exception: " & ex.Message)
        End Try
    End Function

    Private Shared Function SaveDashboardChartImageForExport(session As HttpSessionState, sectionName As String, titleText As String, chartTypeTextFromPage As String, imageData As String, chartData As String, labelText As String) As String
        Try
            Dim commaIndex As Integer = imageData.IndexOf(","c)
            If commaIndex < 0 OrElse commaIndex >= imageData.Length - 1 Then Return DashboardUploadStatus(session, sectionName, "invalid-image-data")

            Dim base64Text As String = imageData.Substring(commaIndex + 1)
            Dim imageBytes() As Byte = Convert.FromBase64String(base64Text)
            If imageBytes.Length = 0 Then Return DashboardUploadStatus(session, sectionName, "empty-image-bytes")

            Dim signature As String = ChartGoogleSnapshotSignatureShared("image", sectionName, titleText, labelText, chartData)
            Dim snapshots As DataTable = AnalysisExportSnapshot.SnapshotTable(session)
            For Each row As DataRow In snapshots.Rows
                If snapshots.Columns.Contains("Signature") AndAlso String.Equals(row("Signature").ToString(), signature, StringComparison.OrdinalIgnoreCase) Then Return DashboardUploadStatus(session, sectionName, "exists")
            Next

            Dim folderPath As String = ChartGoogleSnapshotFolderShared(session)
            If folderPath.Trim() = "" Then Return DashboardUploadStatus(session, sectionName, "blank-folder")
            Directory.CreateDirectory(folderPath)

            Dim chartTypeText As String = FieldTextLocalShared(chartTypeTextFromPage)
            If chartTypeText.Trim() = "" Then chartTypeText = FieldTextLocalShared(session("ChartType"))
            If chartTypeText.Trim() = "" Then chartTypeText = "Chart"
            Dim stamp As String = DateTime.Now.ToString("yyyyMMddHHmmssfff")
            Dim fileName As String = SafeFilePartLocalShared("ChartDashboardPng" & chartTypeText & "_" & FieldTextLocalShared(session("REPORTID")) & "_" & sectionName & "_" & stamp) & ".png"
            Dim filePath As String = Path.Combine(folderPath, fileName)
            File.WriteAllBytes(filePath, imageBytes)

            Dim snapshotRow As DataRow = snapshots.NewRow()
            snapshotRow("Key") = "ChartGoogleImage_" & SafeFilePartLocalShared(sectionName) & "_" & stamp
            snapshotRow("Included") = True
            snapshotRow("Package Item") = "Chart Dashboard PNG Picture - " & chartTypeText & " - " & sectionName & " - " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            snapshotRow("Label Above Grid") = labelText & " Picture section: " & sectionName & "."
            snapshotRow("File") = fileName
            snapshotRow("Description") = "Separate PNG picture captured from the ChartGoogle dashboard " & chartTypeText & " " & sectionName & " tile."
            snapshotRow("FullPath") = filePath
            If snapshots.Columns.Contains("Signature") Then snapshotRow("Signature") = signature
            snapshots.Rows.Add(snapshotRow)
            If chartData.StartsWith("Dashboard image only:", StringComparison.OrdinalIgnoreCase) Then Return DashboardUploadStatus(session, sectionName, "blank-data-but-saved")
            Return DashboardUploadStatus(session, sectionName, "saved")
        Catch ex As Exception
            Return DashboardUploadStatus(session, sectionName, "exception: " & ex.Message)
        End Try
    End Function

    Private Shared Function DashboardChartArrayForSection(session As HttpSessionState, sectionName As String) As String
        Select Case sectionName.Trim().ToLowerInvariant()
            Case "count"
                Return FieldTextLocalShared(session("arrCount"))
            Case "distinct count"
                Return FieldTextLocalShared(session("arrDistCount"))
            Case "value"
                Return FieldTextLocalShared(session("arrValue"))
            Case "sum"
                Return FieldTextLocalShared(session("arrSum"))
            Case "average"
                Return FieldTextLocalShared(session("arrAvg"))
            Case "standard deviation"
                Return FieldTextLocalShared(session("arrStDev"))
            Case "maximum"
                Return FieldTextLocalShared(session("arrMax"))
            Case "minimum"
                Return FieldTextLocalShared(session("arrMin"))
        End Select
        Return ""
    End Function

    Private Shared Function ChartGoogleLabelTextShared(session As HttpSessionState) As String
        Dim sb As New StringBuilder()
        sb.Append("Chart Dashboard. ")
        sb.Append("Report: " & FieldTextLocalShared(session("REPTITLE")) & ". ")
        sb.Append("Chart type: " & FieldTextLocalShared(session("ChartType")) & ". ")
        sb.Append("Category field(s): " & FieldTextLocalShared(session("cat1")))
        If FieldTextLocalShared(session("cat2")).Trim() <> "" AndAlso Not String.Equals(FieldTextLocalShared(session("cat1")), FieldTextLocalShared(session("cat2")), StringComparison.OrdinalIgnoreCase) Then
            sb.Append(", " & FieldTextLocalShared(session("cat2")))
        End If
        sb.Append(". ")
        sb.Append("Value field: " & FieldTextLocalShared(session("AxisY")) & ". ")
        If FieldTextLocalShared(session("WhereStm")).Trim() <> "" Then sb.Append("Filter: " & FieldTextLocalShared(session("WhereStm")).Trim() & ". ")
        sb.Append("The export package item includes chart-ready grouped data for the visible dashboard tiles.")
        Return sb.ToString()
    End Function

    Private Shared Function ChartGoogleSnapshotSignatureShared(itemType As String, sectionName As String, titleText As String, labelText As String, dataText As String) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("ChartGoogle")
        sb.AppendLine(itemType)
        sb.AppendLine(sectionName)
        sb.AppendLine(titleText)
        sb.AppendLine(labelText)
        sb.AppendLine(dataText)
        Using sha As SHA256 = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()))).Replace("-", "")
        End Using
    End Function

    Private Sub AppendInfoRow(sb As StringBuilder, labelText As String, valueText As String)
        sb.AppendLine("<tr><th align=""left"">" & HtmlEncodeText(labelText) & "</th><td>" & HtmlEncodeText(valueText) & "</td></tr>")
    End Sub

    Private Function ChartGoogleSnapshotSignature(labelText As String) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("ChartGoogle")
        sb.AppendLine(FieldTextLocal(Session("REPORTID")))
        sb.AppendLine(labelText)
        sb.AppendLine(arrCount)
        sb.AppendLine(arrDistCount)
        sb.AppendLine(arrValue)
        sb.AppendLine(arrSum)
        sb.AppendLine(arrAvg)
        sb.AppendLine(arrStDev)
        sb.AppendLine(arrMax)
        sb.AppendLine(arrMin)
        Using sha As SHA256 = SHA256.Create()
            Return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()))).Replace("-", "")
        End Using
    End Function

    Private Function ChartGoogleSnapshotFolder() As String
        Return ChartGoogleSnapshotFolderShared(Session)
    End Function

    Private Shared Function DashboardUploadStatus(session As HttpSessionState, sectionName As String, statusText As String) As String
        Try
            If session Is Nothing Then Return statusText
            Dim folderPath As String = ChartGoogleSnapshotFolderShared(session)
            If folderPath.Trim() <> "" Then
                Directory.CreateDirectory(folderPath)
                Dim logPath As String = Path.Combine(folderPath, "ChartGoogleDashboardUploadLog.txt")
                Dim lineText As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") &
                    " | " & FieldTextLocalShared(sectionName) &
                    " | " & FieldTextLocalShared(statusText) &
                    " | Report=" & FieldTextLocalShared(session("REPORTID")) &
                    " | ChartType=" & FieldTextLocalShared(session("ChartType")) &
                    Environment.NewLine
                File.AppendAllText(logPath, lineText, Encoding.UTF8)
            End If
        Catch
        End Try
        Return statusText
    End Function

    Private Shared Function ChartGoogleSnapshotFolderShared(session As HttpSessionState) As String
        Dim folderPath As String = ""
        If session("AnalysisExportSnapshotFolder") IsNot Nothing Then folderPath = session("AnalysisExportSnapshotFolder").ToString()
        If folderPath.Trim() <> "" Then Return folderPath

        Dim tempPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory(), "Temp")
        Dim sessionName As String = "AnalysisSnapshots_" & SafeFilePartLocalShared(session.SessionID)
        If session("logon") IsNot Nothing AndAlso session("logon").ToString().Trim() <> "" Then sessionName &= "_" & SafeFilePartLocalShared(session("logon").ToString())
        folderPath = Path.Combine(tempPath, sessionName)
        session("AnalysisExportSnapshotFolder") = folderPath
        Return folderPath
    End Function

    Private Function HtmlEncodeText(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Return Server.HtmlEncode(valueText)
    End Function

    Private Function FieldTextLocal(value As Object) As String
        Return FieldTextLocalShared(value)
    End Function

    Private Shared Function FieldTextLocalShared(value As Object) As String
        If value Is Nothing OrElse Convert.IsDBNull(value) Then Return ""
        Return value.ToString()
    End Function

    Private Function SafeFilePartLocal(valueText As String) As String
        Return SafeFilePartLocalShared(valueText)
    End Function

    Private Shared Function SafeFilePartLocalShared(valueText As String) As String
        If valueText Is Nothing Then valueText = ""
        Dim invalidChars() As Char = Path.GetInvalidFileNameChars()
        For Each ch As Char In invalidChars
            valueText = valueText.Replace(ch, "_"c)
        Next
        valueText = valueText.Replace(" ", "_")
        If valueText.Trim() = "" Then valueText = "ChartGoogle"
        Return valueText
    End Function

    Private Sub LinkButtonBack_Click(sender As Object, e As EventArgs) Handles LinkButtonBack.Click
        'from Analytics, from ListOfReports
        If Request("frm") = "Analytics" Then
            Response.Redirect("ShowReport.aspx?srd=11&REPORT=" & Session("REPORTID"))
        ElseIf Request("frm") = "ListOfReports" Then
            Response.Redirect("ListOfReports.aspx")

        Else
            Response.Redirect("ReportViews.aspx?Report=" & Session("REPORTID") & "&see=yes")
            'Response.Redirect("ReportViews.aspx?see=yes")
        End If

    End Sub

    Private Sub lnkExportDashboardData_Click(sender As Object, e As EventArgs) Handles lnkExportDashboardData.Click
        If Not HasAnyChartData() Then
            LabelError.Text = "No dashboard data are available to export."
            Exit Sub
        End If

        Dim labelText As String = ChartGoogleLabelText()
        Dim fileName As String = SafeFilePartLocal("ChartDashboard_" & FieldTextLocal(Session("REPORTID")) & "_" & DateTime.Now.ToString("yyyyMMddHHmmss")) & ".html"
        Dim bytes() As Byte = Encoding.UTF8.GetBytes(BuildChartGoogleSnapshotHtml(labelText))
        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()
        Response.ContentType = "text/html"
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.AppendHeader("Content-Length", bytes.Length.ToString())
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.End()
    End Sub

    Private Sub DropDownChartType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownChartType.SelectedIndexChanged
        charttype = DropDownChartType.Text
        Session("ChartType") = charttype
    End Sub

    Private Sub lnkbtnCount_Click(sender As Object, e As EventArgs) Handles lnkbtnCount.Click
        Session("arr") = arrCount
        Response.Redirect("ChartGoogleOne.aspx?fn=Count&Report=" & Session("REPORTID") & "&ttl=" & ttlCount & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnDistCount_Click(sender As Object, e As EventArgs) Handles lnkbtnDistCount.Click
        Session("arr") = arrDistCount
        Response.Redirect("ChartGoogleOne.aspx?fn=CountDistinct&Report=" & Session("REPORTID") & "&ttl=" & ttlDistCount & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnValue_Click(sender As Object, e As EventArgs) Handles lnkbtnValue.Click
        Session("arr") = arrValue
        Response.Redirect("ChartGoogleOne.aspx?fn=Value&Report=" & Session("REPORTID") & "&ttl=" & ttlValue & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnSum_Click(sender As Object, e As EventArgs) Handles lnkbtnSum.Click
        Session("arr") = arrSum
        Response.Redirect("ChartGoogleOne.aspx?fn=Sum&Report=" & Session("REPORTID") & "&ttl=" & ttlSum & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnAvg_Click(sender As Object, e As EventArgs) Handles lnkbtnAvg.Click
        Session("arr") = arrAvg
        Response.Redirect("ChartGoogleOne.aspx?fn=Avg&Report=" & Session("REPORTID") & "&ttl=" & ttlAvg & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnStDev_Click(sender As Object, e As EventArgs) Handles lnkbtnStDev.Click
        Session("arr") = arrStDev
        Response.Redirect("ChartGoogleOne.aspx?fn=StDev&Report=" & Session("REPORTID") & "&ttl=" & ttlStDev & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnMax_Click(sender As Object, e As EventArgs) Handles lnkbtnMax.Click
        Session("arr") = arrMax
        Response.Redirect("ChartGoogleOne.aspx?fn=Max&Report=" & Session("REPORTID") & "&ttl=" & ttlMax & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnMin_Click(sender As Object, e As EventArgs) Handles lnkbtnMin.Click
        Session("arr") = arrMin
        Response.Redirect("ChartGoogleOne.aspx?fn=Min&Report=" & Session("REPORTID") & "&ttl=" & ttlMin & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnReverse_Click(sender As Object, e As EventArgs) Handles lnkbtnReverse.Click
        Response.Redirect("ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat2") & "&x2=" & Session("cat1") & "&y1=" & Session("AxisY") & "&fn=" & Session("Aggregate") & "&charttype=" & charttype)
    End Sub
End Class
