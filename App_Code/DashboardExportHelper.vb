Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.UI

Public NotInheritable Class DashboardExportHelper
    Private Sub New()
    End Sub

    Public Shared Sub PrepareExportPackageSession(page As Page, dashboardName As String, reportId As String, dashboardKind As String, notes As String, Optional action As String = "")
        If page Is Nothing OrElse page.Session Is Nothing Then Exit Sub
        Dim runId As String = CurrentDashboardExportRunId(page)
        page.Session("DashboardExportRunId") = runId
        SavePostedDashboardImages(page)
        Dim folder As String = BuildDashboardExportFolder(page)
        Dim manifest As DataTable = BuildDashboardManifest(page, folder, dashboardName, reportId, dashboardKind, notes)
        AddDashboardSpecificationManifest(page, manifest, folder, dashboardName, reportId, dashboardKind)
        page.Session("ExportPackageTable") = manifest
        page.Session("DashboardExportPackageNotes") = DefaultNotes(page, dashboardName, reportId, notes)
        page.Session("DashboardExportPackageAction") = action
        page.Session("DashboardExportPackagePrepared") = "yes"
    End Sub

    Private Shared Function CurrentDashboardExportRunId(page As Page) As String
        If page IsNot Nothing AndAlso page.Request IsNot Nothing Then
            Dim posted As String = page.Request.Form("DashboardExportRunId")
            If posted IsNot Nothing AndAlso posted.Trim() <> "" Then Return posted.Trim()
        End If
        Return DateTime.Now.ToString("yyyyMMddHHmmssfff") & "_" & Guid.NewGuid().ToString("N").Substring(0, 8)
    End Function

    Private Class DashboardImagePayload
        Public Property title As String
        Public Property chartType As String
        Public Property reportId As String
        Public Property section As String
        Public Property image As String
    End Class

    Private Shared Sub SavePostedDashboardImages(page As Page)
        If page Is Nothing OrElse page.Session Is Nothing OrElse page.Request Is Nothing Then Exit Sub
        Dim payloadText As String = page.Request.Form("DashboardExportImages")
        If payloadText Is Nothing OrElse payloadText.Trim() = "" Then Exit Sub

        Try
            Dim serializer As New JavaScriptSerializer()
            serializer.MaxJsonLength = Integer.MaxValue
            Dim images As DashboardImagePayload() = serializer.Deserialize(Of DashboardImagePayload())(payloadText)
            If images Is Nothing Then Exit Sub

            Dim snapshots As DataTable = AnalysisExportSnapshot.SnapshotTable(page.Session)
            Dim folderPath As String = DashboardSnapshotFolder(page.Session)
            If folderPath.Trim() = "" Then Exit Sub
            Directory.CreateDirectory(folderPath)

            For Each item As DashboardImagePayload In images
                If item Is Nothing OrElse item.image Is Nothing OrElse item.image.Trim() = "" Then Continue For
                Dim commaIndex As Integer = item.image.IndexOf(","c)
                If commaIndex < 0 OrElse commaIndex >= item.image.Length - 1 Then Continue For

                Dim base64Text As String = item.image.Substring(commaIndex + 1)
                Dim imageBytes() As Byte = Convert.FromBase64String(base64Text)
                If imageBytes Is Nothing OrElse imageBytes.Length = 0 Then Continue For

                Dim chartTypeText As String = If(item.chartType, "").Trim()
                If chartTypeText = "" Then chartTypeText = "Chart"
                Dim reportText As String = If(item.reportId, "").Trim()
                If reportText = "" AndAlso page.Session("REPORTID") IsNot Nothing Then reportText = page.Session("REPORTID").ToString()
                Dim sectionText As String = If(item.section, "").Trim()
                If sectionText = "" Then sectionText = If(item.title, "").Trim()
                If sectionText = "" Then sectionText = chartTypeText

                Dim stamp As String = DateTime.Now.ToString("yyyyMMddHHmmssfff")
                Dim fileName As String = SafeFilePart("ChartDashboardPng" & chartTypeText & "_" & reportText & "_" & sectionText & "_" & stamp) & ".png"
                Dim filePath As String = Path.Combine(folderPath, fileName)
                File.WriteAllBytes(filePath, imageBytes)

                Dim row As DataRow = snapshots.NewRow()
                Dim runId As String = ""
                If page.Session("DashboardExportRunId") IsNot Nothing Then runId = page.Session("DashboardExportRunId").ToString()
                row("Key") = "DashboardImage_" & If(runId.Trim() = "", "", SafeFilePart(runId) & "_") & SafeFilePart(sectionText) & "_" & stamp
                row("Included") = True
                row("Package Item") = "Chart Dashboard PNG Picture - " & chartTypeText & " - " & sectionText & " - " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                row("Label Above Grid") = "Dashboard picture. Report: " & reportText & ". Chart type: " & chartTypeText & ". Title/section: " & sectionText & If(runId.Trim() = "", ".", ". Dashboard export run id: " & runId & ".")
                row("File") = fileName
                row("Description") = "PNG picture captured directly from the visible dashboard chart during dashboard export." & If(runId.Trim() = "", "", " DashboardExportRunId=" & runId & ".")
                row("FullPath") = filePath
                If snapshots.Columns.Contains("Signature") Then row("Signature") = "DashboardImage|" & runId & "|" & filePath
                snapshots.Rows.Add(row)
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Function DashboardSnapshotFolder(session As System.Web.SessionState.HttpSessionState) As String
        If session Is Nothing Then Return ""
        If session("AnalysisExportSnapshotFolder") IsNot Nothing AndAlso session("AnalysisExportSnapshotFolder").ToString().Trim() <> "" Then Return session("AnalysisExportSnapshotFolder").ToString()
        Dim tempPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory(), "Temp")
        Dim folderName As String = "AnalysisSnapshots_" & SafeFilePart(session.SessionID)
        If session("logon") IsNot Nothing AndAlso session("logon").ToString().Trim() <> "" Then folderName &= "_" & SafeFilePart(session("logon").ToString())
        Dim folderPath As String = Path.Combine(tempPath, folderName)
        session("AnalysisExportSnapshotFolder") = folderPath
        Return folderPath
    End Function

    Public Shared Sub ExportZip(page As Page, dashboardName As String, reportId As String, dashboardKind As String, notes As String)
        Dim folder As String = BuildDashboardExportFolder(page)
        Dim manifest As DataTable = BuildDashboardManifest(page, folder, dashboardName, reportId, dashboardKind, notes)
        WritePackageFiles(folder, manifest)
        Dim zipPath As String = Path.Combine(Path.GetDirectoryName(folder), SafeFilePart("DashboardExport_" & dashboardName & "_" & DateTime.Now.ToString("yyyyMMddHHmmss")) & ".zip")
        If File.Exists(zipPath) Then File.Delete(zipPath)
        ZipFile.CreateFromDirectory(folder, zipPath)
        DownloadFile(page, zipPath, "application/zip")
    End Sub

    Public Shared Sub ExportPdf(page As Page, dashboardName As String, reportId As String, dashboardKind As String, notes As String)
        Dim folder As String = BuildDashboardExportFolder(page)
        Dim manifest As DataTable = BuildDashboardManifest(page, folder, dashboardName, reportId, dashboardKind, notes)
        WritePackageFiles(folder, manifest)
        Dim pdfPath As String = Path.Combine(folder, SafeFilePart("DashboardExport_" & dashboardName) & ".pdf")
        File.WriteAllBytes(pdfPath, SimplePdf(BuildDashboardPdfLines(page, dashboardName, reportId, dashboardKind, notes, manifest)))

        Dim separateFiles As List(Of String) = SeparatePdfPackageFiles(manifest)
        If separateFiles.Count > 0 Then
            Dim zipPath As String = Path.Combine(Path.GetDirectoryName(folder), SafeFilePart("DashboardExportPdf_" & dashboardName & "_" & DateTime.Now.ToString("yyyyMMddHHmmss")) & ".zip")
            Dim pdfFolder As String = Path.Combine(folder, "PdfDocuments")
            Directory.CreateDirectory(pdfFolder)
            File.Copy(pdfPath, Path.Combine(pdfFolder, Path.GetFileName(pdfPath)), True)
            CopyFilesToFolder(separateFiles, pdfFolder)
            If File.Exists(zipPath) Then File.Delete(zipPath)
            ZipFile.CreateFromDirectory(pdfFolder, zipPath)
            DownloadFile(page, zipPath, "application/zip")
            Return
        End If

        DownloadFile(page, pdfPath, "application/pdf")
    End Sub

    Private Shared Function BuildDashboardExportFolder(page As Page) As String
        Dim tempRoot As String = page.Server.MapPath("~/Temp")
        Directory.CreateDirectory(tempRoot)
        Dim sessionPart As String = "session"
        If page.Session IsNot Nothing AndAlso page.Session.SessionID IsNot Nothing Then sessionPart = page.Session.SessionID
        Dim folder As String = Path.Combine(tempRoot, "DashboardExport_" & SafeFilePart(sessionPart) & "_" & DateTime.Now.ToString("yyyyMMddHHmmssfff"))
        Directory.CreateDirectory(folder)
        Return folder
    End Function

    Private Shared Function BuildDashboardManifest(page As Page, folder As String, dashboardName As String, reportId As String, dashboardKind As String, notes As String) As DataTable
        Directory.CreateDirectory(folder)
        Dim manifest As DataTable = EmptyManifestTable()
        Dim rows As DataTable = DashboardRows(page, dashboardName, reportId, dashboardKind)

        Dim notesPath As String = Path.Combine(folder, "DashboardNotes.txt")
        File.WriteAllText(notesPath, DefaultNotes(page, dashboardName, reportId, notes), Encoding.UTF8)
        AddPackageRow(manifest, "Notes", "Dashboard Notes", True, "User notes and export context.", "DashboardNotes.txt", "Notes entered for this dashboard export.", notesPath)

        AddVisualSnapshotRows(page, manifest, dashboardName, reportId, dashboardKind, rows)
        If manifest.Rows.Count = 1 Then AddNoVisualRowsMessage(manifest, folder, dashboardName, reportId)
        Return manifest
    End Function

    Private Shared Sub AddDashboardSpecificationManifest(page As Page, manifest As DataTable, folder As String, dashboardName As String, reportId As String, dashboardKind As String)
        If manifest Is Nothing Then Exit Sub
        Dim rows As DataTable = DashboardRows(page, dashboardName, reportId, dashboardKind)
        Dim manifestPath As String = Path.Combine(folder, "DashboardFileManifest.txt")
        Dim sb As New StringBuilder()
        sb.AppendLine("Dashboard Export Manifest")
        sb.AppendLine("Created: " & DateTime.Now.ToString())
        sb.AppendLine("Dashboard: " & dashboardName)
        sb.AppendLine("Report: " & If(reportId.Trim() = "", "all reports in this dashboard", reportId))
        sb.AppendLine("Type: " & dashboardKind)
        sb.AppendLine()
        sb.AppendLine("Included files")
        For Each row As DataRow In manifest.Rows
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            sb.AppendLine("- " & FieldText(row, "Package Item"))
            sb.AppendLine("  File: " & FieldText(row, "File"))
            sb.AppendLine("  Details: " & FieldText(row, "Label Above Grid"))
            sb.AppendLine("  Description: " & FieldText(row, "Description"))
        Next
        sb.AppendLine()
        sb.AppendLine("Dashboard tile specifications")
        If rows Is Nothing OrElse rows.Rows.Count = 0 Then
            sb.AppendLine("- No dashboard rows were found.")
        Else
            Dim index As Integer = 1
            For Each tile As DataRow In rows.Rows
                sb.AppendLine(index.ToString() & ". " & TileTitle(tile))
                sb.AppendLine("  Report: " & FieldText(tile, "ReportID"))
                sb.AppendLine("  Chart type: " & FieldText(tile, "ChartType"))
                sb.AppendLine("  Fields/options: " & TileSummary(tile))
                index += 1
            Next
        End If
        File.WriteAllText(manifestPath, sb.ToString(), Encoding.UTF8)
        AddPackageRow(manifest, "DashboardFileManifest", "Dashboard File Manifest", True, "List of files and dashboard tile specifications. No chart data are included.", Path.GetFileName(manifestPath), "Text manifest listing exported files and dashboard tile settings without raw data.", manifestPath)
    End Sub

    Private Shared Function EmptyManifestTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Included", GetType(Boolean))
        dt.Columns.Add("Package Item", GetType(String))
        dt.Columns.Add("Label Above Grid", GetType(String))
        dt.Columns.Add("File", GetType(String))
        dt.Columns.Add("Description", GetType(String))
        dt.Columns.Add("FullPath", GetType(String))
        dt.Columns.Add("ManifestOrder", GetType(Integer))
        Return dt
    End Function

    Private Shared Sub AddPackageRow(dt As DataTable, itemKey As String, itemName As String, included As Boolean, labelText As String, fileText As String, description As String, Optional fullPath As String = "")
        If dt Is Nothing Then Exit Sub
        Dim row As DataRow = dt.NewRow()
        row("Key") = itemKey
        row("Included") = included
        row("Package Item") = itemName
        row("Label Above Grid") = labelText
        row("File") = fileText
        row("Description") = description
        row("FullPath") = fullPath
        row("ManifestOrder") = dt.Rows.Count
        dt.Rows.Add(row)
    End Sub

    Private Shared Sub AddVisualSnapshotRows(page As Page, manifest As DataTable, dashboardName As String, reportId As String, dashboardKind As String, dashboardRows As DataTable)
        If page Is Nothing OrElse page.Session Is Nothing OrElse manifest Is Nothing Then Exit Sub
        Dim snapshots As DataTable = AnalysisExportSnapshot.SnapshotTable(page.Session)
        If snapshots Is Nothing Then Exit Sub

        Dim addedPaths As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each existing As DataRow In manifest.Rows
            Dim existingPath As String = FieldText(existing, "FullPath")
            If existingPath.Trim() <> "" AndAlso Not addedPaths.ContainsKey(existingPath) Then addedPaths.Add(existingPath, True)
        Next

        For Each snapshot As DataRow In snapshots.Rows
            Dim fullPath As String = FieldText(snapshot, "FullPath")
            If fullPath.Trim() = "" OrElse Not File.Exists(fullPath) Then Continue For
            If addedPaths.ContainsKey(fullPath) Then Continue For
            If Not IsVisualSnapshot(snapshot) Then Continue For
            If Not SnapshotMatchesCurrentRun(page, snapshot) Then Continue For
            If Not VisualSnapshotBelongsToDashboard(page, snapshot, dashboardName, reportId, dashboardKind, dashboardRows) Then Continue For

            AddPackageRow(manifest,
                FieldText(snapshot, "Key"),
                FieldText(snapshot, "Package Item"),
                True,
                FieldText(snapshot, "Label Above Grid"),
                FieldText(snapshot, "File"),
                FieldText(snapshot, "Description"),
                fullPath)
            addedPaths.Add(fullPath, True)
        Next
    End Sub

    Private Shared Function IsVisualSnapshot(snapshot As DataRow) As Boolean
        Dim fullPath As String = FieldText(snapshot, "FullPath")
        Dim fileName As String = FieldText(snapshot, "File")
        Dim item As String = FieldText(snapshot, "Package Item")
        Dim extension As String = Path.GetExtension(fullPath).ToLowerInvariant()

        If extension = ".png" AndAlso (fileName.StartsWith("ChartDashboardPng", StringComparison.OrdinalIgnoreCase) OrElse item.IndexOf("PNG Picture", StringComparison.OrdinalIgnoreCase) >= 0) Then Return True
        If extension = ".pdf" AndAlso (fileName.StartsWith("ReportViews_", StringComparison.OrdinalIgnoreCase) OrElse item.IndexOf("PDF Report", StringComparison.OrdinalIgnoreCase) >= 0) Then Return True
        Return False
    End Function

    Private Shared Function SnapshotMatchesCurrentRun(page As Page, snapshot As DataRow) As Boolean
        If page Is Nothing OrElse page.Session Is Nothing OrElse page.Session("DashboardExportRunId") Is Nothing Then Return False
        Dim runId As String = page.Session("DashboardExportRunId").ToString().Trim()
        If runId = "" Then Return False
        Dim snapshotText As String = (FieldText(snapshot, "Key") & " " & FieldText(snapshot, "Label Above Grid") & " " & FieldText(snapshot, "Description") & " " & FieldText(snapshot, "Signature")).ToLowerInvariant()
        Return snapshotText.Contains(runId.ToLowerInvariant())
    End Function

    Private Shared Function VisualSnapshotBelongsToDashboard(page As Page, snapshot As DataRow, dashboardName As String, reportId As String, dashboardKind As String, dashboardRows As DataTable) As Boolean
        Dim extension As String = Path.GetExtension(FieldText(snapshot, "FullPath")).ToLowerInvariant()
        Dim isChartPicture As Boolean = extension = ".png"
        Dim isReportPdf As Boolean = extension = ".pdf"
        Dim isAnalyticsTilePicture As Boolean = isChartPicture AndAlso SnapshotText(snapshot).IndexOf("analytics tile", StringComparison.OrdinalIgnoreCase) >= 0

        Select Case dashboardKind.Trim().ToLowerInvariant()
            Case "analytics"
                If Not (isReportPdf OrElse isAnalyticsTilePicture) Then Return False
            Case "chart"
                If Not isChartPicture Then Return False
        End Select

        If reportId.Trim() <> "" AndAlso Not SnapshotMatchesReport(snapshot, reportId) Then Return False
        If reportId.Trim() = "" AndAlso dashboardRows IsNot Nothing AndAlso dashboardRows.Rows.Count > 0 Then
            Dim matchedAnyReport As Boolean = False
            For Each row As DataRow In dashboardRows.Rows
                Dim rowReport As String = FieldText(row, "ReportID")
                If rowReport.Trim() <> "" AndAlso SnapshotMatchesReport(snapshot, rowReport) Then
                    matchedAnyReport = True
                    Exit For
                End If
            Next
            If Not matchedAnyReport Then Return False
        End If

        If isChartPicture Then Return True
        If isReportPdf Then Return ReportPdfMatchesDashboardRows(snapshot, dashboardRows)
        Return False
    End Function

    Private Shared Function SnapshotMatchesReport(snapshot As DataRow, reportId As String) As Boolean
        If reportId.Trim() = "" Then Return True
        Return SnapshotText(snapshot).Contains(reportId.ToLowerInvariant())
    End Function

    Private Shared Function SnapshotText(snapshot As DataRow) As String
        Return (FieldText(snapshot, "Key") & " " & FieldText(snapshot, "Package Item") & " " & FieldText(snapshot, "Label Above Grid") & " " & FieldText(snapshot, "File") & " " & FieldText(snapshot, "FullPath") & " " & FieldText(snapshot, "Description")).ToLowerInvariant()
    End Function

    Private Shared Function ReportPdfMatchesDashboardRows(snapshot As DataRow, dashboardRows As DataTable) As Boolean
        If dashboardRows Is Nothing OrElse dashboardRows.Rows.Count = 0 Then Return True
        Dim snapshotText As String = (FieldText(snapshot, "Key") & " " & FieldText(snapshot, "Package Item") & " " & FieldText(snapshot, "Label Above Grid") & " " & FieldText(snapshot, "File")).ToLowerInvariant()

        For Each row As DataRow In dashboardRows.Rows
            If Not IsAnalyticsRow(row) Then Continue For
            Dim rowText As String = (TileTitle(row) & " " & DecodeArr(FieldText(row, "ARR"))).ToLowerInvariant()
            If rowText.Contains("reportviews.aspx") OrElse rowText.Contains("showreport.aspx") Then
                If snapshotText.Contains("export report to pdf") Then Return True
                If rowText.Contains("matrix") AndAlso snapshotText.Contains("matrix") Then Return True
                If rowText.Contains("drill") AndAlso snapshotText.Contains("drilldown") Then Return True
                If rowText.Contains("bar") AndAlso snapshotText.Contains("bar") Then Return True
                If rowText.Contains("pie") AndAlso snapshotText.Contains("pie") Then Return True
                If rowText.Contains("line") AndAlso snapshotText.Contains("line") Then Return True
                If rowText.Contains("group") AndAlso snapshotText.Contains("group statistics") Then Return True
            End If
        Next
        Return False
    End Function

    Private Shared Sub AddNoVisualRowsMessage(manifest As DataTable, folder As String, dashboardName As String, reportId As String)
        Dim statusPath As String = Path.Combine(folder, "DashboardVisualExportStatus.txt")
        Dim sb As New StringBuilder()
        sb.AppendLine("No saved dashboard pictures or ReportViews PDF files were found for this dashboard export.")
        sb.AppendLine("Dashboard: " & dashboardName)
        sb.AppendLine("Report: " & If(reportId.Trim() = "", "all reports in this dashboard", reportId))
        sb.AppendLine("For chart tiles, open/render the dashboard so PNG pictures are captured before export.")
        sb.AppendLine("For RDL/report-view tiles, open the corresponding ReportViews page or use Export report to PDF so the PDF snapshot is available.")
        File.WriteAllText(statusPath, sb.ToString(), Encoding.UTF8)
        AddPackageRow(manifest, "DashboardVisualStatus", "Dashboard Visual Export Status", True, "No saved visual files were available.", Path.GetFileName(statusPath), "Status message explaining why no PNG/PDF visual files were included.", statusPath)
    End Sub

    Private Shared Function SnapshotBelongsToDashboard(page As Page, snapshot As DataRow, dashboardName As String, reportId As String, dashboardRows As DataTable) As Boolean
        If reportId.Trim() = "" Then Return True
        If page IsNot Nothing AndAlso page.Session IsNot Nothing AndAlso page.Session("REPORTID") IsNot Nothing Then
            If String.Equals(page.Session("REPORTID").ToString(), reportId, StringComparison.OrdinalIgnoreCase) Then Return True
        End If

        Dim snapshotText As String = (FieldText(snapshot, "Key") & " " & FieldText(snapshot, "Package Item") & " " & FieldText(snapshot, "Label Above Grid") & " " & FieldText(snapshot, "File") & " " & FieldText(snapshot, "FullPath")).ToLowerInvariant()
        If snapshotText.Contains(reportId.ToLowerInvariant()) Then Return True
        If dashboardName.Trim() <> "" AndAlso snapshotText.Contains(dashboardName.ToLowerInvariant()) Then Return True

        If dashboardRows IsNot Nothing Then
            For Each row As DataRow In dashboardRows.Rows
                Dim title As String = TileTitle(row).ToLowerInvariant()
                Dim chartType As String = FieldText(row, "ChartType").ToLowerInvariant()
                If title.Trim() <> "" AndAlso snapshotText.Contains(title) Then Return True
                If chartType.Trim() <> "" AndAlso snapshotText.Contains(chartType) Then Return True
            Next
        End If
        Return False
    End Function

    Private Shared Sub WritePackageFiles(packageFolder As String, manifest As DataTable)
        Directory.CreateDirectory(packageFolder)
        File.WriteAllText(Path.Combine(packageFolder, "PackageManifest.txt"), ManifestText(manifest), Encoding.UTF8)
        If manifest Is Nothing Then Exit Sub

        Dim usedNames As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In manifest.Rows
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            Dim fullPath As String = FieldText(row, "FullPath")
            If fullPath.Trim() = "" OrElse Not File.Exists(fullPath) Then Continue For
            Dim targetFolder As String = If(IsDashboardGeneratedRow(row), packageFolder, Path.Combine(packageFolder, "AnalysisSnapshots"))
            Directory.CreateDirectory(targetFolder)
            Dim targetName As String = UniquePackageFileName(Path.GetFileName(fullPath), usedNames)
            Dim targetPath As String = Path.Combine(targetFolder, targetName)
            If String.Equals(Path.GetFullPath(fullPath), Path.GetFullPath(targetPath), StringComparison.OrdinalIgnoreCase) Then Continue For
            File.Copy(fullPath, targetPath, True)
        Next
    End Sub

    Private Shared Function IsDashboardGeneratedRow(row As DataRow) As Boolean
        Dim key As String = FieldText(row, "Key").ToLowerInvariant()
        Return key = "notes" OrElse key = "dashboardmanifest" OrElse key = "dashboardfilemanifest" OrElse key.StartsWith("dashboardtile_") OrElse key.StartsWith("dashboardchartdata_") OrElse key.StartsWith("dashboardanalyticstile_")
    End Function

    Private Shared Function ManifestText(manifest As DataTable) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("Included" & vbTab & "Package Item" & vbTab & "Details" & vbTab & "File" & vbTab & "Description")
        If manifest Is Nothing Then Return sb.ToString()
        For Each row As DataRow In manifest.Rows
            sb.AppendLine(If(Convert.ToBoolean(row("Included")), "Yes", "No") & vbTab & FieldText(row, "Package Item") & vbTab & FieldText(row, "Label Above Grid") & vbTab & FieldText(row, "File") & vbTab & FieldText(row, "Description"))
        Next
        Return sb.ToString()
    End Function

    Private Shared Function DashboardRows(page As Page, dashboardName As String, reportId As String, dashboardKind As String) As DataTable
        If page.Session Is Nothing OrElse page.Session("logon") Is Nothing OrElse dashboardName.Trim() = "" Then Return Nothing

        Dim ownerField As String = "UserID"
        If page.Request("dash") IsNot Nothing AndAlso page.Request("dash").ToString().Trim().Equals("yes", StringComparison.OrdinalIgnoreCase) Then ownerField = "Prop6"
        Dim sql As String = "SELECT * FROM ourdashboards WHERE " & ownerField & "='" & SqlText(page.Session("logon")) & "' AND Dashboard='" & SqlText(dashboardName) & "'"
        If reportId.Trim() <> "" Then sql &= " AND ReportID='" & SqlText(reportId) & "'"

        Select Case dashboardKind.Trim().ToLowerInvariant()
            Case "analytics"
                sql &= " AND UPPER(ChartType)='ANALYTICS'"
            Case "chart"
                sql &= " AND UPPER(ChartType)<>'ANALYTICS'"
        End Select

        sql &= " ORDER BY Indx"
        Dim ret As String = String.Empty
        Dim dv As DataView = mRecords(sql, ret)
        If ret.Trim() <> "" OrElse dv Is Nothing OrElse dv.Table Is Nothing Then Return Nothing
        Return dv.Table
    End Function

    Private Shared Function ManifestHtml(page As Page, dashboardName As String, reportId As String, dashboardKind As String, rows As DataTable) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<html><head><style>body{font-family:Arial;font-size:12px;} table{border-collapse:collapse;width:100%;} th{background:#663300;color:white;} th,td{border:1px solid #ccc;padding:4px;vertical-align:top;word-break:break-word;}</style></head><body>")
        sb.AppendLine("<h2>Dashboard Export</h2>")
        sb.AppendLine("<p><b>Dashboard:</b> " & Html(dashboardName) & "<br/><b>Report:</b> " & Html(If(reportId.Trim() = "", "all dashboard reports", reportId)) & "<br/><b>Type:</b> " & Html(dashboardKind) & "<br/><b>Created:</b> " & Html(DateTime.Now.ToString()) & "</p>")
        sb.AppendLine("<table><tr><th>#</th><th>Item</th><th>Report</th><th>Type</th><th>Fields / URL</th></tr>")
        If rows IsNot Nothing Then
            Dim index As Integer = 1
            For Each row As DataRow In rows.Rows
                sb.AppendLine("<tr><td>" & index.ToString() & "</td><td>" & Html(TileTitle(row)) & "</td><td>" & Html(FieldText(row, "ReportID")) & "</td><td>" & Html(FieldText(row, "ChartType")) & "</td><td>" & Html(TileSummary(row)) & "</td></tr>")
                index += 1
            Next
        End If
        sb.AppendLine("</table></body></html>")
        Return sb.ToString()
    End Function

    Private Shared Function BuildDashboardPdfLines(page As Page, dashboardName As String, reportId As String, dashboardKind As String, notes As String, manifest As DataTable) As List(Of String)
        Dim lines As New List(Of String)()
        lines.Add("Dashboard Export")
        lines.Add("Dashboard: " & dashboardName)
        lines.Add("Report: " & If(reportId.Trim() = "", "all dashboard reports", reportId))
        lines.Add("Type: " & dashboardKind)
        lines.Add("Created: " & DateTime.Now.ToString())
        lines.Add("")
        lines.Add("Notes:")
        For Each line As String In DefaultNotes(page, dashboardName, reportId, notes).Split(New String() {vbCrLf, vbLf}, StringSplitOptions.None)
            lines.Add(line)
        Next
        lines.Add("")
        lines.Add("Included Package Items:")

        If manifest IsNot Nothing AndAlso manifest.Rows.Count > 0 Then
            Dim manifestIndex As Integer = 1
            For Each row As DataRow In manifest.Rows
                If Not Convert.ToBoolean(row("Included")) Then Continue For
                lines.Add(manifestIndex.ToString() & ". " & FieldText(row, "Package Item"))
                If FieldText(row, "Label Above Grid").Trim() <> "" Then lines.Add("   Details: " & FieldText(row, "Label Above Grid"))
                If FieldText(row, "File").Trim() <> "" Then lines.Add("   File: " & FieldText(row, "File"))
                If FieldText(row, "Description").Trim() <> "" Then lines.Add("   Description: " & FieldText(row, "Description"))
                AppendFilePreview(lines, FieldText(row, "FullPath"))
                manifestIndex += 1
            Next
            Return lines
        End If

        lines.Add("Dashboard Items:")

        Dim rows As DataTable = DashboardRows(page, dashboardName, reportId, dashboardKind)
        If rows Is Nothing OrElse rows.Rows.Count = 0 Then
            lines.Add("No dashboard rows were found.")
            Return lines
        End If

        Dim index As Integer = 1
        For Each row As DataRow In rows.Rows
            lines.Add(index.ToString() & ". " & TileTitle(row))
            lines.Add("   Report: " & FieldText(row, "ReportID") & "   Type: " & FieldText(row, "ChartType"))
            lines.Add("   " & TileSummary(row))
            index += 1
        Next
        Return lines
    End Function

    Private Shared Sub AppendFilePreview(lines As List(Of String), filePath As String)
        If lines Is Nothing OrElse filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Exit Sub
        Dim extension As String = Path.GetExtension(filePath).ToLowerInvariant()
        If extension = ".pdf" OrElse extension = ".png" OrElse extension = ".jpg" OrElse extension = ".jpeg" OrElse extension = ".gif" Then
            lines.Add("   This file is included separately in the downloaded ZIP when needed.")
            Exit Sub
        End If

        Try
            Dim content As String = File.ReadAllText(filePath)
            If extension = ".html" OrElse extension = ".htm" OrElse extension = ".xls" Then content = PlainTextFromHtml(content)
            Dim count As Integer = 0
            For Each sourceLine As String In content.Split(New String() {vbCrLf, vbLf}, StringSplitOptions.None)
                Dim cleanLine As String = sourceLine.Trim()
                If cleanLine = "" Then Continue For
                lines.Add("   " & cleanLine)
                count += 1
                If count >= 35 Then
                    lines.Add("   ...")
                    Exit For
                End If
            Next
        Catch ex As Exception
            lines.Add("   Preview could not be read: " & ex.Message)
        End Try
    End Sub

    Private Shared Function PlainTextFromHtml(htmlText As String) As String
        If htmlText Is Nothing Then Return ""
        Dim text As String = htmlText
        text = Regex.Replace(text, "(?i)<br\s*/?>", Environment.NewLine)
        text = Regex.Replace(text, "(?i)</tr>", Environment.NewLine)
        text = Regex.Replace(text, "(?i)</p>", Environment.NewLine)
        text = Regex.Replace(text, "(?i)</h[1-6]>", Environment.NewLine)
        text = Regex.Replace(text, "<[^>]+>", " ")
        text = HttpUtility.HtmlDecode(text)
        text = Regex.Replace(text, "[ \t]+", " ")
        text = Regex.Replace(text, "(\r?\n\s*){3,}", Environment.NewLine & Environment.NewLine)
        Return text.Trim()
    End Function

    Private Shared Function SeparatePdfPackageFiles(manifest As DataTable) As List(Of String)
        Dim files As New List(Of String)()
        If manifest Is Nothing Then Return files
        For Each row As DataRow In manifest.Rows
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            Dim fullPath As String = FieldText(row, "FullPath")
            If fullPath.Trim() = "" OrElse Not File.Exists(fullPath) Then Continue For
            Dim extension As String = Path.GetExtension(fullPath).ToLowerInvariant()
            If extension = ".pdf" OrElse extension = ".png" OrElse extension = ".jpg" OrElse extension = ".jpeg" OrElse extension = ".gif" Then files.Add(fullPath)
        Next
        Return files
    End Function

    Private Shared Sub CopyFilesToFolder(files As List(Of String), targetFolder As String)
        If files Is Nothing Then Exit Sub
        Directory.CreateDirectory(targetFolder)
        Dim usedNames As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each filePath As String In files
            If filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Continue For
            Dim targetName As String = UniquePackageFileName(Path.GetFileName(filePath), usedNames)
            File.Copy(filePath, Path.Combine(targetFolder, targetName), True)
        Next
    End Sub

    Private Shared Function UniquePackageFileName(fileName As String, usedNames As Dictionary(Of String, Integer)) As String
        If fileName Is Nothing OrElse fileName.Trim() = "" Then fileName = "PackageFile.txt"
        Dim baseName As String = Path.GetFileNameWithoutExtension(fileName)
        Dim extension As String = Path.GetExtension(fileName)
        If Not usedNames.ContainsKey(fileName) Then
            usedNames(fileName) = 1
            Return fileName
        End If
        usedNames(fileName) += 1
        Return baseName & "_" & usedNames(fileName).ToString() & extension
    End Function

    Private Shared Function DefaultNotes(page As Page, dashboardName As String, reportId As String, notes As String) As String
        If notes IsNot Nothing AndAlso notes.Trim() <> "" Then Return notes.Trim()
        Dim sb As New StringBuilder()
        sb.AppendLine("DataAI dashboard export created on " & DateTime.Now.ToString())
        sb.AppendLine("Dashboard: " & dashboardName)
        sb.AppendLine("Report: " & If(reportId.Trim() = "", "all reports in this dashboard", reportId))
        sb.AppendLine("This export contains notes and dashboard tile input rows prepared for Export Packages.")
        Return sb.ToString().Trim()
    End Function

    Private Shared Function TileDetails(row As DataRow) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("Dashboard Item")
        sb.AppendLine("Title: " & TileTitle(row))
        sb.AppendLine("Report: " & FieldText(row, "ReportID"))
        sb.AppendLine("Chart Type: " & FieldText(row, "ChartType"))
        sb.AppendLine("Dashboard URL / ARR: " & DecodeArr(FieldText(row, "ARR")))
        sb.AppendLine("Summary: " & TileSummary(row))
        sb.AppendLine("Comments: " & FieldText(row, "Comments"))
        Return sb.ToString()
    End Function

    Private Shared Function TileSummary(row As DataRow) As String
        If IsAnalyticsRow(row) Then Return DecodeArr(FieldText(row, "ARR"))
        Dim parts As New List(Of String)()
        AddPart(parts, "x1", FieldText(row, "x1"))
        AddPart(parts, "x2", FieldText(row, "x2"))
        AddPart(parts, "y1", FieldText(row, "y1"))
        AddPart(parts, "aggregation", FieldText(row, "fn1"))
        AddPart(parts, "where", FieldText(row, "WhereStm").Replace("^", "'"))
        AddPart(parts, "map", FieldText(row, "MapName"))
        Return String.Join("; ", parts.ToArray())
    End Function

    Private Shared Sub AddPart(parts As List(Of String), label As String, value As String)
        If value IsNot Nothing AndAlso value.Trim() <> "" Then parts.Add(label & ": " & value.Trim())
    End Sub

    Private Shared Function ChartDataHtml(row As DataRow, arrText As String) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<html><head><style>body{font-family:Arial;font-size:12px;} pre{white-space:pre-wrap;word-break:break-word;border:1px solid #ccc;padding:8px;}</style></head><body>")
        sb.AppendLine("<h2>" & Html(TileTitle(row)) & "</h2>")
        sb.AppendLine("<p>" & Html(TileSummary(row)) & "</p>")
        sb.AppendLine("<pre>" & Html(arrText) & "</pre>")
        sb.AppendLine("</body></html>")
        Return sb.ToString()
    End Function

    Private Shared Function TileTitle(row As DataRow) As String
        Dim title As String = FieldText(row, "GraphTitle").Trim()
        If title <> "" AndAlso Not title.Equals("Message", StringComparison.OrdinalIgnoreCase) Then Return title
        If IsAnalyticsRow(row) Then Return "Analytics Tile"
        Dim chartType As String = FieldText(row, "ChartType").Trim()
        If chartType = "" Then chartType = "Chart"
        Return chartType & " Dashboard Item"
    End Function

    Private Shared Function IsAnalyticsRow(row As DataRow) As Boolean
        Return FieldText(row, "ChartType").Trim().Equals("analytics", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function FieldText(row As DataRow, columnName As String) As String
        If row Is Nothing OrElse row.Table Is Nothing OrElse Not row.Table.Columns.Contains(columnName) Then Return ""
        If row(columnName) Is Nothing OrElse Convert.IsDBNull(row(columnName)) Then Return ""
        Return row(columnName).ToString()
    End Function

    Private Shared Function DecodeArr(value As String) As String
        If value Is Nothing Then Return ""
        Return value.Replace("^^", "'").Replace("**", "[").Replace("##", "]")
    End Function

    Private Shared Function SqlText(value As Object) As String
        If value Is Nothing Then Return ""
        Return value.ToString().Replace("'", "''")
    End Function

    Private Shared Function SafeFilePart(value As String) As String
        Dim text As String = If(value, "").Trim()
        If text = "" Then text = "Dashboard"
        For Each ch As Char In Path.GetInvalidFileNameChars()
            text = text.Replace(ch, "_"c)
        Next
        text = Regex.Replace(text, "\s+", "_")
        If text.Length > 80 Then text = text.Substring(0, 80)
        Return text
    End Function

    Private Shared Function Html(value As String) As String
        Return HttpUtility.HtmlEncode(If(value, ""))
    End Function

    Private Shared Sub DownloadFile(page As Page, filePath As String, contentType As String)
        page.Response.Clear()
        page.Response.ContentType = contentType
        page.Response.AddHeader("Content-Disposition", "attachment; filename=" & Path.GetFileName(filePath))
        page.Response.TransmitFile(filePath)
        page.Response.Flush()
        page.Response.End()
    End Sub

    Private Shared Function SimplePdf(sourceLines As List(Of String)) As Byte()
        Dim pages As New List(Of List(Of String))()
        Dim current As New List(Of String)()
        For Each sourceLine As String In sourceLines
            For Each line As String In WrapPdfLine(sourceLine, 92)
                If current.Count >= 48 Then
                    pages.Add(current)
                    current = New List(Of String)()
                End If
                current.Add(line)
            Next
        Next
        If current.Count > 0 Then pages.Add(current)
        If pages.Count = 0 Then pages.Add(New List(Of String)(New String() {"Dashboard Export"}))

        Dim objects As New List(Of String)()
        objects.Add("<< /Type /Catalog /Pages 2 0 R >>")
        Dim kids As New StringBuilder()
        For i As Integer = 0 To pages.Count - 1
            kids.Append((3 + i * 2).ToString()).Append(" 0 R ")
        Next
        objects.Add("<< /Type /Pages /Kids [" & kids.ToString().Trim() & "] /Count " & pages.Count.ToString() & " >>")
        For i As Integer = 0 To pages.Count - 1
            Dim pageObj As Integer = 3 + i * 2
            Dim contentObj As Integer = pageObj + 1
            objects.Add("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> /F2 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >> >> >> /Contents " & contentObj.ToString() & " 0 R >>")
            Dim streamText As String = PdfPageStream(pages(i), i + 1, pages.Count)
            objects.Add("<< /Length " & Encoding.ASCII.GetByteCount(streamText).ToString() & " >>" & vbLf & "stream" & vbLf & streamText & "endstream")
        Next

        Dim ms As New MemoryStream()
        Dim writer As New StreamWriter(ms, Encoding.ASCII)
        writer.Write("%PDF-1.4" & vbLf)
        Dim offsets As New List(Of Long)()
        offsets.Add(0)
        For i As Integer = 0 To objects.Count - 1
            writer.Flush()
            offsets.Add(ms.Position)
            writer.Write((i + 1).ToString() & " 0 obj" & vbLf)
            writer.Write(objects(i))
            writer.Write(vbLf & "endobj" & vbLf)
        Next
        writer.Flush()
        Dim xrefPosition As Long = ms.Position
        writer.Write("xref" & vbLf)
        writer.Write("0 " & (objects.Count + 1).ToString() & vbLf)
        writer.Write("0000000000 65535 f " & vbLf)
        For i As Integer = 1 To offsets.Count - 1
            writer.Write(offsets(i).ToString("0000000000") & " 00000 n " & vbLf)
        Next
        writer.Write("trailer" & vbLf)
        writer.Write("<< /Size " & (objects.Count + 1).ToString() & " /Root 1 0 R >>" & vbLf)
        writer.Write("startxref" & vbLf)
        writer.Write(xrefPosition.ToString() & vbLf)
        writer.Write("%%EOF")
        writer.Flush()
        Return ms.ToArray()
    End Function

    Private Shared Function PdfPageStream(lines As List(Of String), pageNumber As Integer, pageCount As Integer) As String
        Dim sb As New StringBuilder()
        sb.Append("BT").Append(vbLf)
        sb.Append("/F1 9 Tf").Append(vbLf)
        sb.Append("50 760 Td").Append(vbLf)
        For Each line As String In lines
            sb.Append("(").Append(PdfEscape(line)).Append(") Tj").Append(vbLf)
            sb.Append("0 -14 Td").Append(vbLf)
        Next
        sb.Append("0 -14 Td").Append(vbLf)
        sb.Append("(").Append(PdfEscape("Page " & pageNumber.ToString() & " of " & pageCount.ToString())).Append(") Tj").Append(vbLf)
        sb.Append("ET").Append(vbLf)
        Return sb.ToString()
    End Function

    Private Shared Function WrapPdfLine(valueText As String, width As Integer) As List(Of String)
        Dim result As New List(Of String)()
        If valueText Is Nothing Then
            result.Add("")
            Return result
        End If
        Dim remaining As String = valueText
        Do While remaining.Length > width
            Dim splitAt As Integer = remaining.LastIndexOf(" "c, width)
            If splitAt <= 0 Then splitAt = width
            result.Add(remaining.Substring(0, splitAt).TrimEnd())
            remaining = remaining.Substring(splitAt).TrimStart()
        Loop
        result.Add(remaining)
        Return result
    End Function

    Private Shared Function PdfEscape(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Dim text As String = Regex.Replace(valueText, "[^\u0009\u000A\u000D\u0020-\u007E]", "?")
        Return text.Replace("\", "\\").Replace("(", "\(").Replace(")", "\)")
    End Function
End Class
