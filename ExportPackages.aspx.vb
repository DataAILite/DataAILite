Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports Microsoft.Reporting.WebForms

Partial Class ExportPackages
    Inherits System.Web.UI.Page

    Private Sub ExportPackages_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Export Packages"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Export%20Packages"
    End Sub

    Private Sub ExportPackages_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            SetDefaultNotes()
            BuildAndBindPackage()
        End If
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        UpdatePackageSelectionsFromGrid()
        ExportPackage()
    End Sub

    Private Sub ButtonExportPdf_Click(sender As Object, e As EventArgs) Handles ButtonExportPdf.Click
        UpdatePackageSelectionsFromGrid()
        ExportPackagePdf()
    End Sub

    Private Sub GridViewPackage_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewPackage.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub
        Dim lnk As LinkButton = TryCast(e.Row.FindControl("lnkOpenFile"), LinkButton)
        If lnk Is Nothing Then Exit Sub
        If lnk.Text.Trim() = "" Then
            lnk.Visible = False
            Exit Sub
        End If
        Dim sm As ScriptManager = ScriptManager.GetCurrent(Page)
        If sm IsNot Nothing Then sm.RegisterPostBackControl(lnk)
    End Sub

    Private Sub GridViewPackage_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewPackage.RowCommand
        If Not String.Equals(e.CommandName, "OpenPackageFile", StringComparison.OrdinalIgnoreCase) Then Exit Sub
        Dim rowIndex As Integer
        If Not Integer.TryParse(e.CommandArgument.ToString(), rowIndex) Then Exit Sub
        Dim manifest As DataTable = TryCast(Session("ExportPackageTable"), DataTable)
        If manifest Is Nothing Then
            BuildAndBindPackage()
            manifest = TryCast(Session("ExportPackageTable"), DataTable)
        End If
        If manifest Is Nothing OrElse rowIndex < 0 OrElse rowIndex >= manifest.Rows.Count Then Exit Sub
        OpenPackageFile(manifest.Rows(rowIndex))
    End Sub

    Private Sub SetDefaultNotes()
        If txtNotes.Text.Trim() <> "" Then Exit Sub

        Dim sb As New StringBuilder()
        sb.AppendLine("DataAI export package created on " & DateTime.Now.ToString())
        sb.AppendLine("Report: " & FieldText(Session("REPORTID")))
        sb.AppendLine("Title: " & FieldText(Session("REPTITLE")))
        sb.AppendLine("This package includes the checked rows from the Export Package grid.")
        sb.AppendLine("Report, report definition, CSV data, Excel data, AI analysis, chart snapshots, and notes can be included or excluded with the Included checkboxes.")
        sb.AppendLine("Analytics and Market Excel snapshots are created when Build is clicked on those pages and are listed here with the label that appeared above each result grid.")
        txtNotes.Text = sb.ToString().Trim()
    End Sub

    Private Sub BuildAndBindPackage()
        Dim dt As New DataTable()
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Included", GetType(Boolean))
        dt.Columns.Add("Package Item", GetType(String))
        dt.Columns.Add("Label Above Grid", GetType(String))
        dt.Columns.Add("File", GetType(String))
        dt.Columns.Add("Description", GetType(String))
        dt.Columns.Add("FullPath", GetType(String))
        dt.Columns.Add("ManifestOrder", GetType(Integer))

        AddPackageRow(dt, "Notes", "Notes", True, "", "AnalysisNotes.txt", "User-entered package notes.")
        AddPackageRow(dt, "Report", "Report", True, "", "Report.pdf", "Current report exported as PDF when report data and RDL definition are available.")
        AddPackageRow(dt, "ReportDefinition", "Report Definition", True, "", "ReportDefinitions.txt; RDL file", "Report id, title, current report definition text, and RDL file.")
        AddPackageRow(dt, "CSVData", "CSV Data", False, "", "ReportData.csv", "Current report data exported as comma-delimited text.")
        AddPackageRow(dt, "ExcelData", "Excel Data", True, "", "ReportData.xls", "Current report data exported as Excel-compatible tab-delimited content.")
        AddPackageRow(dt, "AIAnalysis", "AI analysis", True, "", "AIAnalysis.txt", "Real AI output for the current report when AI settings are available.")
        PrepareStandardPackageFiles(dt)
        AddSnapshotRows(dt)

        Session("ExportPackageTable") = dt
        GridViewPackage.DataSource = dt
        GridViewPackage.DataBind()
        LabelInfo.Text = "Export package manifest (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

    Private Sub AddPackageRow(dt As DataTable, itemKey As String, itemName As String, included As Boolean, labelText As String, fileText As String, description As String, Optional fullPath As String = "")
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

    Private Sub AddSnapshotRows(dt As DataTable)
        Dim snapshots As DataTable = AnalysisExportSnapshot.SnapshotTable(Session)
        If snapshots Is Nothing Then Exit Sub
        Dim addedPaths As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each snapshot As DataRow In snapshots.Rows
            Dim fullPath As String = snapshot("FullPath").ToString()
            If fullPath.Trim() <> "" Then
                If addedPaths.ContainsKey(fullPath) Then Continue For
                addedPaths.Add(fullPath, True)
            End If
            AddPackageRow(dt,
                snapshot("Key").ToString(),
                snapshot("Package Item").ToString(),
                True,
                snapshot("Label Above Grid").ToString(),
                snapshot("File").ToString(),
                snapshot("Description").ToString(),
                fullPath)
        Next
    End Sub

    Private Sub PrepareStandardPackageFiles(dt As DataTable)
        If dt Is Nothing Then Exit Sub

        Dim tempFolder As String = PreviewPackageFolder()
        Directory.CreateDirectory(tempFolder)
        Dim reportData As DataTable = Nothing

        Try
            SetPackageRowFullPath(dt, "Notes", WriteNotesPackageFile(tempFolder))

            If HasPackageRow(dt, "CSVData") OrElse HasPackageRow(dt, "ExcelData") OrElse HasPackageRow(dt, "Report") Then
                reportData = CurrentReportData()
            End If

            If reportData IsNot Nothing Then
                SetPackageRowFullPath(dt, "CSVData", WriteCsvDataPackageFile(tempFolder, reportData))
                SetPackageRowFullPath(dt, "ExcelData", WriteExcelDataPackageFile(tempFolder, reportData))
                SetPackageRowFullPath(dt, "Report", WriteReportPdfPackageFile(tempFolder, reportData))
            End If

            SetPackageRowFullPath(dt, "ReportDefinition", WriteReportDefinitionPackageFile(tempFolder))
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        End Try
    End Sub

    Private Function HasPackageRow(dt As DataTable, itemKey As String) As Boolean
        If dt Is Nothing Then Return False
        For Each row As DataRow In dt.Rows
            If row("Key").ToString().Equals(itemKey, StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function

    Private Sub SetPackageRowFullPath(dt As DataTable, itemKey As String, filePath As String)
        If dt Is Nothing OrElse filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Exit Sub
        For Each row As DataRow In dt.Rows
            If row("Key").ToString().Equals(itemKey, StringComparison.OrdinalIgnoreCase) Then
                row("FullPath") = filePath
                If itemKey.Equals("ReportDefinition", StringComparison.OrdinalIgnoreCase) Then row("File") = Path.GetFileName(filePath)
                Exit Sub
            End If
        Next
    End Sub

    Private Sub UpdatePackageSelectionsFromGrid()
        Dim manifest As DataTable = TryCast(Session("ExportPackageTable"), DataTable)
        If manifest Is Nothing Then
            BuildAndBindPackage()
            manifest = TryCast(Session("ExportPackageTable"), DataTable)
        End If
        If manifest Is Nothing Then Exit Sub

        For i As Integer = 0 To Math.Min(GridViewPackage.Rows.Count, manifest.Rows.Count) - 1
            Dim chk As CheckBox = TryCast(GridViewPackage.Rows(i).FindControl("chkIncluded"), CheckBox)
            If chk IsNot Nothing Then manifest.Rows(i)("Included") = chk.Checked
        Next
        Session("ExportPackageTable") = manifest
    End Sub

    Private Function IsIncluded(itemKey As String) As Boolean
        Dim manifest As DataTable = TryCast(Session("ExportPackageTable"), DataTable)
        If manifest Is Nothing Then Return False
        For Each row As DataRow In manifest.Rows
            If String.Equals(row("Key").ToString(), itemKey, StringComparison.OrdinalIgnoreCase) Then
                Return Convert.ToBoolean(row("Included"))
            End If
        Next
        Return False
    End Function

    Private Function CurrentReportData() As DataTable
        Dim ret As String = ""
        If Session("REPORTID") Is Nothing Then Return Nothing
        Try
            Dim dv As DataView = RetrieveReportData(Session("REPORTID").ToString(), "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
            If dv IsNot Nothing Then Return dv.Table
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        End Try
        Return Nothing
    End Function

    Private Function WriteNotesPackageFile(tempFolder As String) As String
        Directory.CreateDirectory(tempFolder)
        Dim filePath As String = Path.Combine(tempFolder, "AnalysisNotes.txt")
        File.WriteAllText(filePath, txtNotes.Text, Encoding.UTF8)
        Return filePath
    End Function

    Private Function WriteCsvDataPackageFile(tempFolder As String, reportData As DataTable) As String
        If reportData Is Nothing Then Return ""
        Directory.CreateDirectory(tempFolder)
        Dim filePath As String = Path.Combine(tempFolder, "ReportData.csv")
        File.WriteAllText(filePath, ExportToCSVtext(reportData, ",", "", ""), Encoding.UTF8)
        Return filePath
    End Function

    Private Function WriteExcelDataPackageFile(tempFolder As String, reportData As DataTable) As String
        If reportData Is Nothing Then Return ""
        Directory.CreateDirectory(tempFolder)
        Dim filePath As String = Path.Combine(tempFolder, "ReportData.xls")
        Dim header As String = "Data for Report: " & FieldText(Session("REPTITLE")) & Environment.NewLine & "Records returned: " & reportData.Rows.Count.ToString()
        DataModule.ExportToExcel(reportData, EnsureTrailingSlash(tempFolder), "ReportData.xls", header, FieldText(Session("PageFtr")))
        If File.Exists(filePath) Then Return filePath
        Return ""
    End Function

    Private Function WriteReportDefinitionPackageFile(tempFolder As String) As String
        Directory.CreateDirectory(tempFolder)
        Dim definitionFolder As String = Path.Combine(tempFolder, "ReportDefinition")
        Dim zipPath As String = Path.Combine(tempFolder, "ReportDefinition.zip")
        If Directory.Exists(definitionFolder) Then Directory.Delete(definitionFolder, True)
        If File.Exists(zipPath) Then File.Delete(zipPath)
        Directory.CreateDirectory(definitionFolder)
        File.WriteAllText(Path.Combine(definitionFolder, "ReportDefinitions.txt"), ReportDefinitionsText(), Encoding.UTF8)
        WriteRdlDefinitionFile(definitionFolder)
        ZipFile.CreateFromDirectory(definitionFolder, zipPath)
        Return zipPath
    End Function

    Private Function WriteReportPdfPackageFile(tempFolder As String, reportData As DataTable) As String
        If reportData Is Nothing Then Return ""
        Directory.CreateDirectory(tempFolder)
        WriteReportPdfFile(tempFolder, reportData)
        Dim filePath As String = Path.Combine(tempFolder, "Report.pdf")
        If File.Exists(filePath) Then Return filePath
        Return ""
    End Function

    Private Sub ExportPackage()
        Dim packageFolder As String = ""
        Dim zipPath As String = ""

        Try
            If applpath Is Nothing OrElse applpath.Trim() = "" Then applpath = System.AppDomain.CurrentDomain.BaseDirectory()
            Dim tempFolder As String = Path.Combine(applpath, "Temp")
            Directory.CreateDirectory(tempFolder)

            Dim packageName As String = "ExportPackage_" & SafeFilePart(FieldText(Session("REPORTID"))) & "_" & DateTime.Now.ToString("yyyyMMddHHmmssfff") & "_" & Guid.NewGuid().ToString("N").Substring(0, 8)
            packageFolder = Path.Combine(tempFolder, packageName)
            zipPath = Path.Combine(tempFolder, packageName & ".zip")
            Directory.CreateDirectory(packageFolder)

            WritePackageFiles(packageFolder)

            EnsurePackageHasFiles(packageFolder)
            If File.Exists(zipPath) Then File.Delete(zipPath)
            ZipFile.CreateFromDirectory(packageFolder, zipPath)
            ValidateZipFile(zipPath)
            Dim zipBytes() As Byte = File.ReadAllBytes(zipPath)

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()
            Response.BufferOutput = True
            Response.ContentType = "application/octet-stream"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & Path.GetFileName(zipPath))
            Response.AppendHeader("Content-Length", zipBytes.Length.ToString())
            Response.BinaryWrite(zipBytes)
            Response.Flush()
            Response.End()
        Catch ex As System.Threading.ThreadAbortException
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        Finally
            Try
                If packageFolder <> "" AndAlso Directory.Exists(packageFolder) Then Directory.Delete(packageFolder, True)
            Catch ex As Exception
            End Try
            Try
                If zipPath <> "" AndAlso File.Exists(zipPath) Then File.Delete(zipPath)
            Catch ex As Exception
            End Try
        End Try
    End Sub

    Private Sub ExportPackagePdf()
        Try
            Dim packageName As String = "ExportPackagePdf_" & SafeFilePart(FieldText(Session("REPORTID"))) & "_" & DateTime.Now.ToString("yyyyMMddHHmmssfff") & "_" & Guid.NewGuid().ToString("N").Substring(0, 8)
            Dim manifest As DataTable = CType(Session("ExportPackageTable"), DataTable)
            Dim checkedPdfFiles As List(Of String) = CheckedPdfFilesForInlinePages(manifest)
            Dim ghostscriptMissingForExternalPdfs As Boolean = checkedPdfFiles.Count > 0 AndAlso FindGhostscriptPath().Trim() = ""
            Dim pdfBytes() As Byte = BuildExportPackagePdf(manifest, ghostscriptMissingForExternalPdfs, checkedPdfFiles)
            If pdfBytes Is Nothing OrElse pdfBytes.Length = 0 Then Throw New Exception("Export package PDF was not created.")

            Dim pdfName As String = packageName & ".pdf"
            If ghostscriptMissingForExternalPdfs Then
                ExportCheckedPdfFilesZip(packageName, pdfName, pdfBytes, checkedPdfFiles)
                Exit Sub
            End If

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()
            Response.BufferOutput = True
            Response.ContentType = "application/pdf"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & pdfName)
            Response.AppendHeader("Content-Length", pdfBytes.Length.ToString())
            Response.BinaryWrite(pdfBytes)
            Response.Flush()
            Response.End()
        Catch ex As System.Threading.ThreadAbortException
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
        End Try
    End Sub

    Private Sub ExportCheckedPdfFilesZip(packageName As String, mainPdfName As String, mainPdfBytes As Byte(), checkedPdfFiles As List(Of String))
        Dim packageFolder As String = ""
        Dim zipPath As String = ""
        Try
            If mainPdfBytes Is Nothing OrElse mainPdfBytes.Length = 0 Then Throw New Exception("Main export PDF was not created for fallback ZIP export.")
            If checkedPdfFiles Is Nothing OrElse checkedPdfFiles.Count = 0 Then Throw New Exception("No PDF files are available for fallback ZIP export.")
            If applpath Is Nothing OrElse applpath.Trim() = "" Then applpath = System.AppDomain.CurrentDomain.BaseDirectory()
            Dim tempFolder As String = Path.Combine(applpath, "Temp")
            Directory.CreateDirectory(tempFolder)
            packageFolder = Path.Combine(tempFolder, packageName & "_PdfFiles")
            zipPath = Path.Combine(tempFolder, packageName & "_PdfFiles.zip")
            If Directory.Exists(packageFolder) Then Directory.Delete(packageFolder, True)
            If File.Exists(zipPath) Then File.Delete(zipPath)
            Directory.CreateDirectory(packageFolder)

            Dim usedNames As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            Dim mainTargetName As String = UniquePackageFileName(If(mainPdfName Is Nothing OrElse mainPdfName.Trim() = "", "ExportPackage.pdf", mainPdfName), usedNames)
            File.WriteAllBytes(Path.Combine(packageFolder, mainTargetName), mainPdfBytes)

            For Each filePath As String In checkedPdfFiles
                If filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Continue For
                Dim targetName As String = UniquePackageFileName(Path.GetFileName(filePath), usedNames)
                File.Copy(filePath, Path.Combine(packageFolder, targetName), True)
            Next

            EnsurePackageHasFiles(packageFolder)
            ZipFile.CreateFromDirectory(packageFolder, zipPath)
            ValidateZipFile(zipPath)
            Dim zipBytes() As Byte = File.ReadAllBytes(zipPath)

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()
            Response.BufferOutput = True
            Response.ContentType = "application/zip"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & Path.GetFileName(zipPath))
            Response.AppendHeader("Content-Length", zipBytes.Length.ToString())
            Response.BinaryWrite(zipBytes)
            Response.Flush()
            Response.End()
        Finally
            Try
                If packageFolder <> "" AndAlso Directory.Exists(packageFolder) Then Directory.Delete(packageFolder, True)
            Catch ex As Exception
            End Try
            Try
                If zipPath <> "" AndAlso File.Exists(zipPath) Then File.Delete(zipPath)
            Catch ex As Exception
            End Try
        End Try
    End Sub

    Private Sub EnsurePackageHasFiles(packageFolder As String)
        If Directory.GetFiles(packageFolder, "*", SearchOption.AllDirectories).Length > 0 Then Exit Sub
        File.WriteAllText(Path.Combine(packageFolder, "ExportPackageStatus.txt"), "No checked package items created files. Check included rows and report data availability.", Encoding.UTF8)
    End Sub

    Private Sub ValidateZipFile(zipPath As String)
        If Not File.Exists(zipPath) Then Throw New Exception("Export package zip was not created.")
        Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
            If archive.Entries.Count = 0 Then Throw New Exception("Export package zip is empty.")
        End Using
    End Sub

    Private Sub WritePackageFiles(packageFolder As String)
        Dim manifest As DataTable = CType(Session("ExportPackageTable"), DataTable)
        RefreshNotesFilePath(manifest)
        File.WriteAllText(Path.Combine(packageFolder, "PackageManifest.txt"), PackageHeader() & Environment.NewLine & ExportToCSVtext(ManifestForPackage(manifest), Chr(9), "", ""), Encoding.UTF8)

        WriteCheckedExistingFiles(packageFolder, manifest)
        WriteMissingCheckedStandardFiles(packageFolder, manifest)
    End Sub

    Private Sub RefreshNotesFilePath(manifest As DataTable)
        If manifest Is Nothing Then Exit Sub
        For Each row As DataRow In manifest.Rows
            If Not row("Key").ToString().Equals("Notes", StringComparison.OrdinalIgnoreCase) Then Continue For
            row("FullPath") = WriteNotesPackageFile(PreviewPackageFolder())
            Exit Sub
        Next
    End Sub

    Private Sub WriteCheckedExistingFiles(packageFolder As String, manifest As DataTable)
        If manifest Is Nothing Then Exit Sub
        Dim usedNames As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In manifest.Rows
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            Dim filePath As String = row("FullPath").ToString()
            If filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Continue For
            Dim targetFolder As String = If(IsStandardPackageRow(row), packageFolder, Path.Combine(packageFolder, "AnalysisSnapshots"))
            Directory.CreateDirectory(targetFolder)
            Dim targetName As String = UniquePackageFileName(Path.GetFileName(filePath), usedNames)
            File.Copy(filePath, Path.Combine(targetFolder, targetName), True)
        Next
    End Sub

    Private Function IsStandardPackageRow(row As DataRow) As Boolean
        Dim key As String = row("Key").ToString().ToLowerInvariant()
        Return key = "notes" OrElse key = "report" OrElse key = "reportdefinition" OrElse key = "csvdata" OrElse key = "exceldata" OrElse key = "aianalysis"
    End Function

    Private Function UniquePackageFileName(fileName As String, usedNames As Dictionary(Of String, Integer)) As String
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

    Private Sub WriteMissingCheckedStandardFiles(packageFolder As String, manifest As DataTable)
        If manifest Is Nothing Then Exit Sub
        Dim reportData As DataTable = Nothing

        For Each row As DataRow In manifest.Rows
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            If Not IsStandardPackageRow(row) Then Continue For
            Dim key As String = row("Key").ToString().ToLowerInvariant()
            Dim filePath As String = row("FullPath").ToString()
            If filePath.Trim() <> "" AndAlso File.Exists(filePath) Then Continue For

            If key = "aianalysis" Then
                If reportData Is Nothing Then reportData = CurrentReportData()
                WriteAIAnalysisFile(packageFolder, reportData)
            ElseIf key = "notes" Then
                File.Copy(WriteNotesPackageFile(PreviewPackageFolder()), Path.Combine(packageFolder, "AnalysisNotes.txt"), True)
            ElseIf key = "csvdata" Then
                If reportData Is Nothing Then reportData = CurrentReportData()
                Dim createdPath As String = WriteCsvDataPackageFile(PreviewPackageFolder(), reportData)
                If createdPath.Trim() <> "" AndAlso File.Exists(createdPath) Then File.Copy(createdPath, Path.Combine(packageFolder, "ReportData.csv"), True)
            ElseIf key = "exceldata" Then
                If reportData Is Nothing Then reportData = CurrentReportData()
                Dim createdPath As String = WriteExcelDataPackageFile(PreviewPackageFolder(), reportData)
                If createdPath.Trim() <> "" AndAlso File.Exists(createdPath) Then File.Copy(createdPath, Path.Combine(packageFolder, "ReportData.xls"), True)
            ElseIf key = "reportdefinition" Then
                Dim createdPath As String = WriteReportDefinitionPackageFile(PreviewPackageFolder())
                If createdPath.Trim() <> "" AndAlso File.Exists(createdPath) Then File.Copy(createdPath, Path.Combine(packageFolder, "ReportDefinition.zip"), True)
            ElseIf key = "report" Then
                If reportData Is Nothing Then reportData = CurrentReportData()
                Dim createdPath As String = WriteReportPdfPackageFile(PreviewPackageFolder(), reportData)
                If createdPath.Trim() <> "" AndAlso File.Exists(createdPath) Then File.Copy(createdPath, Path.Combine(packageFolder, "Report.pdf"), True)
            End If
        Next
    End Sub

    Private Function ManifestForPackage(manifest As DataTable) As DataTable
        Dim output As New DataTable()
        output.Columns.Add("Included", GetType(String))
        output.Columns.Add("Package Item", GetType(String))
        output.Columns.Add("Label Above Grid", GetType(String))
        output.Columns.Add("File", GetType(String))
        output.Columns.Add("Description", GetType(String))
        If manifest Is Nothing Then Return output

        For Each row As DataRow In manifest.Rows
            Dim outRow As DataRow = output.NewRow()
            outRow("Included") = If(Convert.ToBoolean(row("Included")), "Yes", "No")
            outRow("Package Item") = row("Package Item").ToString()
            outRow("Label Above Grid") = row("Label Above Grid").ToString()
            outRow("File") = row("File").ToString()
            outRow("Description") = row("Description").ToString()
            output.Rows.Add(outRow)
        Next
        Return output
    End Function

    Private Sub WriteSelectedSnapshotFiles(packageFolder As String, manifest As DataTable)
        If manifest Is Nothing Then Exit Sub
        Dim snapshotsFolder As String = Path.Combine(packageFolder, "AnalysisSnapshots")
        For Each row As DataRow In manifest.Rows
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            Dim filePath As String = row("FullPath").ToString()
            If filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Continue For
            Directory.CreateDirectory(snapshotsFolder)
            Dim targetName As String = Path.GetFileName(filePath)
            File.Copy(filePath, Path.Combine(snapshotsFolder, targetName), True)
        Next
    End Sub

    Private Sub OpenPackageFile(row As DataRow)
        If row Is Nothing Then Exit Sub
        Dim fullPath As String = row("FullPath").ToString()
        If fullPath.Trim() <> "" Then
            ServeExistingPackageFile(fullPath, row("File").ToString())
            Exit Sub
        End If

        Dim tempFolder As String = PreviewPackageFolder()
        Directory.CreateDirectory(tempFolder)
        Dim itemKey As String = row("Key").ToString()
        Dim filePath As String = ""

        Select Case itemKey.ToLowerInvariant()
            Case "notes"
                filePath = Path.Combine(tempFolder, "AnalysisNotes.txt")
                File.WriteAllText(filePath, txtNotes.Text, Encoding.UTF8)
            Case "csvdata"
                Dim reportData As DataTable = CurrentReportData()
                If reportData Is Nothing Then Throw New Exception("Report data is not available.")
                filePath = Path.Combine(tempFolder, "ReportData.csv")
                File.WriteAllText(filePath, ExportToCSVtext(reportData, ",", "", ""), Encoding.UTF8)
            Case "exceldata"
                Dim reportData As DataTable = CurrentReportData()
                If reportData Is Nothing Then Throw New Exception("Report data is not available.")
                Dim header As String = "Data for Report: " & FieldText(Session("REPTITLE")) & Environment.NewLine & "Records returned: " & reportData.Rows.Count.ToString()
                DataModule.ExportToExcel(reportData, EnsureTrailingSlash(tempFolder), "ReportData.xls", header, FieldText(Session("PageFtr")))
                filePath = Path.Combine(tempFolder, "ReportData.xls")
            Case "reportdefinition"
                Dim definitionFolder As String = Path.Combine(tempFolder, "ReportDefinition")
                Directory.CreateDirectory(definitionFolder)
                File.WriteAllText(Path.Combine(definitionFolder, "ReportDefinitions.txt"), ReportDefinitionsText(), Encoding.UTF8)
                WriteRdlDefinitionFile(definitionFolder)
                filePath = Path.Combine(tempFolder, "ReportDefinition.zip")
                If File.Exists(filePath) Then File.Delete(filePath)
                ZipFile.CreateFromDirectory(definitionFolder, filePath)
            Case "report"
                Dim reportData As DataTable = CurrentReportData()
                If reportData Is Nothing Then Throw New Exception("Report data is not available.")
                WriteReportPdfFile(tempFolder, reportData)
                filePath = Path.Combine(tempFolder, "Report.pdf")
            Case "aianalysis"
                Dim reportData As DataTable = CurrentReportData()
                WriteAIAnalysisFile(tempFolder, reportData)
                filePath = Path.Combine(tempFolder, "AIAnalysis.txt")
        End Select

        If filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Throw New Exception("The selected export file was not created.")
        ServeExistingPackageFile(filePath, Path.GetFileName(filePath))
    End Sub

    Private Function PreviewPackageFolder() As String
        If applpath Is Nothing OrElse applpath.Trim() = "" Then applpath = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim sessionPart As String = "nosession"
        If Session IsNot Nothing AndAlso Session.SessionID IsNot Nothing Then sessionPart = SafeFilePart(Session.SessionID)
        Return Path.Combine(applpath, "Temp", "ExportPackagePreview_" & sessionPart)
    End Function

    Private Function EnsureTrailingSlash(folderPath As String) As String
        If folderPath.EndsWith("\") Then Return folderPath
        Return folderPath & "\"
    End Function

    Private Sub ServeExistingPackageFile(filePath As String, displayName As String)
        If filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Throw New Exception("The selected export file was not found.")
        If Not IsAllowedPackageFilePath(filePath) Then Throw New Exception("The selected export file is outside the package Temp folder.")

        Dim fileName As String = Path.GetFileName(If(displayName Is Nothing OrElse displayName.Trim() = "", filePath, displayName))
        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()
        Response.BufferOutput = True
        Response.ContentType = ContentTypeForFile(fileName)
        Response.AppendHeader("Content-Disposition", "inline; filename=" & fileName)
        Response.AppendHeader("Content-Length", New FileInfo(filePath).Length.ToString())
        Response.TransmitFile(filePath)
        Response.Flush()
        Response.End()
    End Sub

    Private Function IsAllowedPackageFilePath(filePath As String) As Boolean
        Dim fullFilePath As String = Path.GetFullPath(filePath)
        Dim baseTempRoot As String = NormalizedFolderPath(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory(), "Temp"))
        If fullFilePath.StartsWith(baseTempRoot, StringComparison.OrdinalIgnoreCase) Then Return True

        If applpath IsNot Nothing AndAlso applpath.Trim() <> "" Then
            Dim appTempRoot As String = NormalizedFolderPath(Path.Combine(applpath, "Temp"))
            If fullFilePath.StartsWith(appTempRoot, StringComparison.OrdinalIgnoreCase) Then Return True
        End If

        Return False
    End Function

    Private Function NormalizedFolderPath(folderPath As String) As String
        Dim normalized As String = Path.GetFullPath(folderPath)
        If Not normalized.EndsWith(Path.DirectorySeparatorChar.ToString()) Then normalized &= Path.DirectorySeparatorChar
        Return normalized
    End Function

    Private Function ContentTypeForFile(fileName As String) As String
        Select Case Path.GetExtension(fileName).ToLowerInvariant()
            Case ".pdf"
                Return "application/pdf"
            Case ".png"
                Return "image/png"
            Case ".jpg", ".jpeg"
                Return "image/jpeg"
            Case ".gif"
                Return "image/gif"
            Case ".html", ".htm"
                Return "text/html"
            Case ".csv"
                Return "text/csv"
            Case ".xls"
                Return "application/vnd.ms-excel"
            Case ".xlsx"
                Return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Case ".zip"
                Return "application/zip"
            Case Else
                Return "text/plain"
        End Select
    End Function

    Private Function BuildExportPackagePdf(manifest As DataTable, Optional skipExternalPdfRendering As Boolean = False, Optional externalPdfFilesForZip As List(Of String) = Nothing) As Byte()
        Dim elements As New List(Of PdfElement)()
        elements.Add(PdfElement.Title("DataAI Export Package"))
        elements.Add(PdfElement.TextLine("Report: " & FieldText(Session("REPORTID"))))
        elements.Add(PdfElement.TextLine("Title: " & FieldText(Session("REPTITLE"))))
        elements.Add(PdfElement.TextLine("Created: " & DateTime.Now.ToString()))
        elements.Add(PdfElement.Space())
        elements.Add(PdfElement.TextLine("Selected existing analytical outputs are shown below in a meaningful review order. Raw data exports, report definitions, and RDL files are skipped in PDF export. Use zipped folder export when original files are needed."))
        elements.Add(PdfElement.Space())

        If manifest Is Nothing Then
            elements.Add(PdfElement.TextLine("No package manifest is available."))
            Return StyledPdf(elements)
        End If

        Dim includedItems As Integer = 0
        Dim orderedRows As List(Of DataRow) = OrderedManifestRows(manifest)
        For i As Integer = 0 To orderedRows.Count - 1
            Dim row As DataRow = orderedRows(i)
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            If Not IncludeRowInPdf(row) Then Continue For
            If skipExternalPdfRendering AndAlso RowHasPdfFile(row) AndAlso NextIncludedPdfRowIsAIOutput(orderedRows, i) Then
                AppendPdfZipReferenceElements(elements, row)
                includedItems += 1
                Continue For
            End If
            If AppendPackageItemToPdfElements(elements, row, skipExternalPdfRendering) Then includedItems += 1
        Next

        If skipExternalPdfRendering AndAlso externalPdfFilesForZip IsNot Nothing AndAlso externalPdfFilesForZip.Count > 0 Then
            elements.Add(PdfElement.Space())
            elements.Add(PdfElement.Section("PDF Files Included In ZIP"))
            elements.Add(PdfElement.TextLine("Ghostscript was not found. Checked PDF files are stored separately in the ZIP file and are not inserted into this PDF."))
            For Each filePath As String In externalPdfFilesForZip
                elements.Add(PdfElement.TextLine(Path.GetFileName(filePath)))
            Next
            includedItems += 1
        End If

        If includedItems = 0 Then
            elements.Add(PdfElement.Section("No checked analytical, chart, AI, or report snapshot files are available for PDF export."))
            elements.Add(PdfElement.TextLine("Use Build buttons on analytical pages first, or use zipped folder export when raw files are needed."))
        End If

        Return StyledPdfWithInlinePdfPages(elements, manifest, skipExternalPdfRendering)
    End Function

    Private Function OrderedManifestRows(manifest As DataTable) As List(Of DataRow)
        Dim rows As New List(Of DataRow)()
        If manifest Is Nothing Then Return rows
        For Each row As DataRow In manifest.Rows
            rows.Add(row)
        Next
        rows.Sort(Function(a, b)
                      Dim orderCompare As Integer = PackageOrder(a).CompareTo(PackageOrder(b))
                      If orderCompare <> 0 Then Return orderCompare
                      If PackageOrder(a) >= 1000 Then Return ManifestOrder(a).CompareTo(ManifestOrder(b))
                      Return String.Compare(a("Package Item").ToString(), b("Package Item").ToString(), StringComparison.OrdinalIgnoreCase)
                  End Function)
        Return rows
    End Function

    Private Function PackageOrder(row As DataRow) As Integer
        Dim key As String = row("Key").ToString().ToLowerInvariant()
        Dim item As String = row("Package Item").ToString().ToLowerInvariant()
        If key = "notes" Then Return 10
        If key = "report" Then Return 20
        If key = "reportdefinition" Then Return 30
        If key = "exceldata" Then Return 40
        If key = "csvdata" Then Return 50
        If IsSnapshotPackageRow(row) Then Return 1000
        If item.Contains("dataai") Then Return 20
        If item.Contains("ai") Then Return 30
        If item.Contains("chart") Then Return 40
        If item.Contains("market") Then Return 50
        If item.Contains("analytics") OrElse item.Contains("analysis") OrElse item.Contains("regression") OrElse item.Contains("profil") OrElse item.Contains("quality") OrElse item.Contains("ranking") OrElse item.Contains("variance") Then Return 60
        Return 200
    End Function

    Private Function IsSnapshotPackageRow(row As DataRow) As Boolean
        Dim key As String = row("Key").ToString().ToLowerInvariant()
        If key = "notes" OrElse key = "report" OrElse key = "reportdefinition" OrElse key = "exceldata" OrElse key = "csvdata" OrElse key = "aianalysis" Then Return False
        Return True
    End Function

    Private Function ManifestOrder(row As DataRow) As Integer
        If row.Table IsNot Nothing AndAlso row.Table.Columns.Contains("ManifestOrder") AndAlso IsNumeric(row("ManifestOrder").ToString()) Then
            Return CInt(row("ManifestOrder"))
        End If
        Return 0
    End Function

    Private Function IncludeRowInPdf(row As DataRow) As Boolean
        Dim key As String = row("Key").ToString().ToLowerInvariant()
        Dim fileName As String = row("File").ToString().ToLowerInvariant()
        Dim fullPath As String = row("FullPath").ToString()
        If IsChartDataFile(row) Then Return False
        If key = "csvdata" OrElse key = "exceldata" OrElse key = "reportdefinition" Then Return False
        If fileName.EndsWith(".rdl") OrElse fileName.EndsWith(".csv") Then Return False
        If fullPath.ToLowerInvariant().EndsWith(".rdl") OrElse fullPath.ToLowerInvariant().EndsWith(".csv") Then Return False
        If key = "notes" Then Return True
        If key = "aianalysis" Then Return True
        Return fullPath.Trim() <> "" AndAlso File.Exists(fullPath)
    End Function

    Private Function AppendPackageItemToPdfElements(elements As List(Of PdfElement), row As DataRow, Optional skipPdfRows As Boolean = False) As Boolean
        If skipPdfRows AndAlso RowHasPdfFile(row) Then Return False
        elements.Add(PdfElement.Space())
        elements.Add(PdfElement.Section(row("Package Item").ToString()))
        If row("Label Above Grid").ToString().Trim() <> "" Then elements.Add(PdfElement.TextLine("Details: " & row("Label Above Grid").ToString()))
        If row("Description").ToString().Trim() <> "" Then elements.Add(PdfElement.TextLine("Description: " & row("Description").ToString()))
        If row("File").ToString().Trim() <> "" Then elements.Add(PdfElement.TextLine("File: " & row("File").ToString()))

        If row("Key").ToString().Equals("Notes", StringComparison.OrdinalIgnoreCase) Then
            AppendNotesToPdfElements(elements)
            Return True
        End If

        Dim files As List(Of String) = PackageFilesForRow(row)
        If files.Count = 0 Then
            elements.Add(PdfElement.TextLine("No existing readable file is available for this PDF export."))
            Return True
        End If

        For Each filePath As String In files
            elements.Add(PdfElement.SubSection("File content: " & Path.GetFileName(filePath)))
            If IsPngFile(filePath) Then
                elements.Add(PdfElement.ImageElement(filePath))
            ElseIf IsPdfFile(filePath) Then
                ' Existing PDF files are rendered together at the bottom of the exported PDF.
            ElseIf IsPdfTextReadableFile(filePath) Then
                AppendReadableFileElements(elements, filePath)
            Else
                Dim info As New FileInfo(filePath)
                elements.Add(PdfElement.TextLine("Binary or formatted file included in ZIP export and represented here by reference."))
                elements.Add(PdfElement.TextLine("File type: " & Path.GetExtension(filePath)))
                elements.Add(PdfElement.TextLine("File size: " & info.Length.ToString() & " bytes"))
            End If
        Next
        Return True
    End Function

    Private Function IsChartDataFile(row As DataRow) As Boolean
        Dim item As String = row("Package Item").ToString().ToLowerInvariant()
        Dim fileName As String = row("File").ToString().ToLowerInvariant()
        Dim description As String = row("Description").ToString().ToLowerInvariant()
        If item.Contains("chart data file") OrElse item.Contains("chart dashboard data file") Then Return True
        If fileName.StartsWith("chartdata") OrElse fileName.StartsWith("chartdashboarddata") Then Return True
        If description.Contains("chart selections and chart-ready data") Then Return True
        If description.Contains("chartgoogle dashboard chart-ready data") Then Return True
        Return False
    End Function

    Private Function IsPngFile(filePath As String) As Boolean
        Return Path.GetExtension(filePath).Equals(".png", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function IsPdfFile(filePath As String) As Boolean
        Return Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function RowHasPdfFile(row As DataRow) As Boolean
        If row Is Nothing Then Return False
        Dim fileName As String = row("File").ToString()
        Dim fullPath As String = row("FullPath").ToString()
        If fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) Then Return True
        If fullPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) Then Return True
        Return False
    End Function

    Private Function NextIncludedPdfRowIsAIOutput(rows As List(Of DataRow), currentIndex As Integer) As Boolean
        If rows Is Nothing Then Return False
        For i As Integer = currentIndex + 1 To rows.Count - 1
            Dim row As DataRow = rows(i)
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            If Not IncludeRowInPdf(row) Then Continue For
            Return IsAIChatOutputRow(row)
        Next
        Return False
    End Function

    Private Function IsAIChatOutputRow(row As DataRow) As Boolean
        Dim key As String = row("Key").ToString().ToLowerInvariant()
        Dim item As String = row("Package Item").ToString().ToLowerInvariant()
        Dim fileName As String = row("File").ToString().ToLowerInvariant()
        If key.StartsWith("chataioutput") Then Return True
        If item.Contains("ai chat output") Then Return True
        If fileName.StartsWith("chataioutput") Then Return True
        Return False
    End Function

    Private Sub AppendPdfZipReferenceElements(elements As List(Of PdfElement), row As DataRow)
        elements.Add(PdfElement.Space())
        elements.Add(PdfElement.Section(row("Package Item").ToString()))
        If row("Label Above Grid").ToString().Trim() <> "" Then elements.Add(PdfElement.TextLine("Details: " & row("Label Above Grid").ToString()))
        If row("Description").ToString().Trim() <> "" Then elements.Add(PdfElement.TextLine("Description: " & row("Description").ToString()))
        For Each filePath As String In PackageFilesForRow(row)
            If IsPdfFile(filePath) Then
                elements.Add(PdfElement.TextLine("PDF file included in ZIP: " & Path.GetFileName(filePath)))
            End If
        Next
    End Sub

    Private Function AppendCheckedPdfPages(generatedPdfBytes As Byte(), manifest As DataTable) As Byte()
        Dim pdfFiles As List(Of String) = CheckedPdfFilesForInlinePages(manifest)
        If pdfFiles.Count = 0 Then Return generatedPdfBytes

        Dim tempFolder As String = PreviewPackageFolder()
        Directory.CreateDirectory(tempFolder)
        Dim generatedPath As String = Path.Combine(tempFolder, "ExportPackageGenerated_" & DateTime.Now.ToString("yyyyMMddHHmmssfff") & ".pdf")
        File.WriteAllBytes(generatedPath, generatedPdfBytes)

        Dim errors As New List(Of String)()
        Dim outputDocument As New PdfSharp.Pdf.PdfDocument()
        ImportPdfPagesIntoDocument(outputDocument, generatedPath, "Export package summary", errors)

        For Each filePath As String In pdfFiles
            ImportPdfPagesIntoDocument(outputDocument, filePath, Path.GetFileName(filePath), errors)
        Next

        If errors.Count > 0 Then AddPdfImportDiagnosticsPage(outputDocument, errors)
        If outputDocument.PageCount = 0 Then Return generatedPdfBytes

        Using ms As New MemoryStream()
            outputDocument.Save(ms, False)
            outputDocument.Close()
            Return ms.ToArray()
        End Using
    End Function

    Private Sub ImportPdfPagesIntoDocument(outputDocument As PdfSharp.Pdf.PdfDocument, filePath As String, displayName As String, errors As List(Of String))
        Dim inputDocument As PdfSharp.Pdf.PdfDocument = Nothing
        Try
            inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(filePath, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)
            For pageIndex As Integer = 0 To inputDocument.PageCount - 1
                outputDocument.AddPage(inputDocument.Pages(pageIndex))
            Next
        Catch ex As Exception
            errors.Add(displayName & ": " & ex.Message)
        Finally
            If inputDocument IsNot Nothing Then inputDocument.Close()
        End Try
    End Sub

    Private Sub AddPdfImportDiagnosticsPage(outputDocument As PdfSharp.Pdf.PdfDocument, errors As List(Of String))
        Dim page As PdfSharp.Pdf.PdfPage = outputDocument.AddPage()
        page.Size = PdfSharp.PageSize.Letter
        Dim gfx As PdfSharp.Drawing.XGraphics = PdfSharp.Drawing.XGraphics.FromPdfPage(page)
        Dim titleFont As New PdfSharp.Drawing.XFont("Arial", 14, PdfSharp.Drawing.XFontStyle.Bold)
        Dim textFont As New PdfSharp.Drawing.XFont("Arial", 9, PdfSharp.Drawing.XFontStyle.Regular)
        gfx.DrawString("PDF Import Diagnostics", titleFont, PdfSharp.Drawing.XBrushes.Black, New PdfSharp.Drawing.XRect(40, 40, page.Width.Point - 80, 24), PdfSharp.Drawing.XStringFormats.TopLeft)
        Dim y As Double = 76
        For Each errorText As String In errors
            For Each line As String In WrapPdfLine(errorText, 105)
                If y > page.Height.Point - 50 Then
                    page = outputDocument.AddPage()
                    page.Size = PdfSharp.PageSize.Letter
                    gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page)
                    y = 40
                End If
                gfx.DrawString(line, textFont, PdfSharp.Drawing.XBrushes.Black, New PdfSharp.Drawing.XRect(40, y, page.Width.Point - 80, 14), PdfSharp.Drawing.XStringFormats.TopLeft)
                y += 14
            Next
            y += 6
        Next
    End Sub

    Private Function CheckedPdfFilesForInlinePages(manifest As DataTable) As List(Of String)
        Dim files As New List(Of String)()
        If manifest Is Nothing Then Return files

        Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In OrderedManifestRows(manifest)
            If Not Convert.ToBoolean(row("Included")) Then Continue For
            If Not IncludeRowInPdf(row) Then Continue For
            Dim filePath As String = row("FullPath").ToString()
            If filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Continue For
            If Not IsPdfFile(filePath) Then Continue For
            If seen.ContainsKey(filePath) Then Continue For
            seen.Add(filePath, True)
            files.Add(filePath)
        Next
        Return files
    End Function

    Private Function StyledPdfWithInlinePdfPages(elements As List(Of PdfElement), manifest As DataTable, Optional skipExternalPdfRendering As Boolean = False) As Byte()
        Dim exportErrors As New List(Of String)()
        Try
            Return StyledPdfWithInlinePdfPagesCore(elements, manifest, exportErrors, skipExternalPdfRendering)
        Catch ex As Exception
            exportErrors.Add("Export PDF failed: " & ex.Message)
            Return StyledPdfWithoutImportedPages(elements, exportErrors)
        End Try
    End Function

    Private Function StyledPdfWithInlinePdfPagesCore(elements As List(Of PdfElement), manifest As DataTable, exportErrors As List(Of String), skipExternalPdfRendering As Boolean) As Byte()
        Dim document As New PdfSharp.Pdf.PdfDocument()
        Dim page As PdfSharp.Pdf.PdfPage = Nothing
        Dim gfx As PdfSharp.Drawing.XGraphics = Nothing
        Dim y As Double = 0
        Dim importErrors As New List(Of String)()

        Try
            StartPdfSharpPage(document, page, gfx, y)

            For Each element As PdfElement In elements
                If element Is Nothing Then Continue For
                Try
                    Select Case element.ElementType
                        Case "title"
                            EnsurePdfSharpSpace(document, page, gfx, y, 38)
                            DrawPdfSharpText(gfx, element.Text, 50, y, 18, True, page.Width.Point - 100)
                            y += 32
                        Case "section"
                            EnsurePdfSharpSpace(document, page, gfx, y, 34)
                            DrawPdfSharpText(gfx, element.Text, 50, y, 14, True, page.Width.Point - 100)
                            y += 22
                            gfx.DrawLine(PdfSharp.Drawing.XPens.Gray, 50, y, page.Width.Point - 50, y)
                            y += 10
                        Case "subsection"
                            EnsurePdfSharpSpace(document, page, gfx, y, 24)
                            DrawPdfSharpText(gfx, element.Text, 50, y, 11, True, page.Width.Point - 100)
                            y += 20
                        Case "space"
                            y += 8
                        Case "table"
                            DrawPdfSharpTable(document, page, gfx, y, element.Rows)
                        Case "image"
                            DrawPdfSharpImage(document, page, gfx, y, element.Text)
                        Case Else
                            DrawPdfSharpParagraph(document, page, gfx, y, element.Text, 9, False)
                    End Select
                Catch exElement As Exception
                    importErrors.Add("Summary element " & element.ElementType & " failed: " & exElement.Message)
                End Try
            Next

            If Not skipExternalPdfRendering Then
                Dim pdfFilesForBottom As List(Of String) = CheckedPdfFilesForInlinePages(manifest)
                If pdfFilesForBottom.Count > 0 Then DrawPdfSharpParagraph(document, page, gfx, y, "Inserted PDF Files", 14, True)
                For Each filePath As String In pdfFilesForBottom
                    DrawPdfSharpParagraph(document, page, gfx, y, "PDF pages from " & Path.GetFileName(filePath) & ":", 11, True)
                    Dim renderedPages As List(Of String) = RenderPdfPagesToImages(filePath, importErrors)
                    If renderedPages.Count = 0 Then
                        DrawPdfSharpParagraph(document, page, gfx, y, "PDF pages could not be rendered as images for this file. See PDF Import Diagnostics below.", 9, False)
                    Else
                        For Each renderedPage As String In renderedPages
                            DrawPdfSharpImage(document, page, gfx, y, renderedPage)
                        Next
                    End If
                Next
            End If

            If importErrors.Count > 0 Then AddPdfSharpDiagnostics(document, page, gfx, y, importErrors)
            If document.PageCount = 0 Then StartPdfSharpPage(document, page, gfx, y)

            Using ms As New MemoryStream()
                document.Save(ms, False)
                document.Close()
                Return ms.ToArray()
            End Using
        Catch ex As Exception
            importErrors.Add("Combined PDF save failed: " & ex.Message)
            Return StyledPdfWithoutImportedPages(elements, importErrors)
        End Try
    End Function

    Private Function StyledPdfWithoutImportedPages(elements As List(Of PdfElement), errors As List(Of String)) As Byte()
        Dim fallbackElements As New List(Of PdfElement)()
        If elements IsNot Nothing Then
            For Each element As PdfElement In elements
                If element Is Nothing Then Continue For
                If element.ElementType = "text" AndAlso element.Text IsNot Nothing AndAlso element.Text.StartsWith("PDF pages from ", StringComparison.OrdinalIgnoreCase) Then
                    fallbackElements.Add(PdfElement.TextLine(element.Text & " Import failed; see diagnostics below."))
                Else
                    fallbackElements.Add(element)
                End If
            Next
        End If

        fallbackElements.Add(PdfElement.Space())
        fallbackElements.Add(PdfElement.Section("PDF Import Diagnostics"))
        If errors Is Nothing OrElse errors.Count = 0 Then
            fallbackElements.Add(PdfElement.TextLine("PDF page import was not completed. No additional diagnostic message was available."))
        Else
            For Each errorText As String In errors
                fallbackElements.Add(PdfElement.TextLine(errorText))
            Next
        End If
        Return StyledPdf(fallbackElements)
    End Function

    Private Sub StartPdfSharpPage(document As PdfSharp.Pdf.PdfDocument, ByRef page As PdfSharp.Pdf.PdfPage, ByRef gfx As PdfSharp.Drawing.XGraphics, ByRef y As Double)
        page = document.AddPage()
        page.Size = PdfSharp.PageSize.Letter
        gfx = PdfSharp.Drawing.XGraphics.FromPdfPage(page)
        y = 40
    End Sub

    Private Sub EnsurePdfSharpSpace(document As PdfSharp.Pdf.PdfDocument, ByRef page As PdfSharp.Pdf.PdfPage, ByRef gfx As PdfSharp.Drawing.XGraphics, ByRef y As Double, requiredHeight As Double)
        If page Is Nothing OrElse gfx Is Nothing Then
            StartPdfSharpPage(document, page, gfx, y)
            Exit Sub
        End If
        If y + requiredHeight <= page.Height.Point - 40 Then Exit Sub
        StartPdfSharpPage(document, page, gfx, y)
    End Sub

    Private Sub DrawPdfSharpText(gfx As PdfSharp.Drawing.XGraphics, valueText As String, x As Double, y As Double, fontSize As Double, bold As Boolean, width As Double)
        Dim fontStyle As PdfSharp.Drawing.XFontStyle = If(bold, PdfSharp.Drawing.XFontStyle.Bold, PdfSharp.Drawing.XFontStyle.Regular)
        Dim font As New PdfSharp.Drawing.XFont("Arial", fontSize, fontStyle)
        gfx.DrawString(If(valueText, ""), font, PdfSharp.Drawing.XBrushes.Black, New PdfSharp.Drawing.XRect(x, y, width, fontSize + 6), PdfSharp.Drawing.XStringFormats.TopLeft)
    End Sub

    Private Sub DrawPdfSharpParagraph(document As PdfSharp.Pdf.PdfDocument, ByRef page As PdfSharp.Pdf.PdfPage, ByRef gfx As PdfSharp.Drawing.XGraphics, ByRef y As Double, valueText As String, fontSize As Integer, bold As Boolean)
        Dim maxChars As Integer = If(fontSize <= 8, 105, 92)
        For Each line As String In WrapPdfLine(If(valueText, ""), maxChars)
            EnsurePdfSharpSpace(document, page, gfx, y, fontSize + 8)
            DrawPdfSharpText(gfx, line, 50, y, fontSize, bold, page.Width.Point - 100)
            y += fontSize + 5
        Next
    End Sub

    Private Sub DrawPdfSharpTable(document As PdfSharp.Pdf.PdfDocument, ByRef page As PdfSharp.Pdf.PdfPage, ByRef gfx As PdfSharp.Drawing.XGraphics, ByRef y As Double, tableRows As List(Of List(Of String)))
        If tableRows Is Nothing OrElse tableRows.Count = 0 Then Exit Sub
        Dim columnCount As Integer = MaxPdfTableColumns(tableRows)
        If columnCount <= 0 Then Exit Sub
        If columnCount > 8 Then columnCount = 8

        Dim left As Double = 40
        Dim totalWidth As Double = page.Width.Point - 80
        Dim colWidth As Double = totalWidth / columnCount
        Dim rowIndex As Integer = 0
        Dim borderPen As New PdfSharp.Drawing.XPen(PdfSharp.Drawing.XColors.Gray, 0.5)
        Dim headerBrush As New PdfSharp.Drawing.XSolidBrush(PdfSharp.Drawing.XColor.FromArgb(230, 245, 230))

        For Each row As List(Of String) In tableRows
            rowIndex += 1
            Dim wrappedCells As New List(Of List(Of String))()
            Dim maxLines As Integer = 1
            For colIndex As Integer = 0 To columnCount - 1
                Dim cellText As String = ""
                If colIndex < row.Count Then cellText = row(colIndex)
                Dim cellLines As List(Of String) = WrapPdfLine(cellText, Math.Max(8, CInt(colWidth / 4.5)))
                If cellLines.Count > 4 Then
                    cellLines = cellLines.GetRange(0, 4)
                    cellLines(cellLines.Count - 1) &= "..."
                End If
                wrappedCells.Add(cellLines)
                If cellLines.Count > maxLines Then maxLines = cellLines.Count
            Next

            Dim rowHeight As Double = Math.Max(18, maxLines * 9 + 8)
            EnsurePdfSharpSpace(document, page, gfx, y, rowHeight + 8)
            Dim x As Double = left
            For colIndex As Integer = 0 To columnCount - 1
                Dim rect As New PdfSharp.Drawing.XRect(x, y, colWidth, rowHeight)
                If rowIndex = 1 Then gfx.DrawRectangle(headerBrush, rect)
                gfx.DrawRectangle(borderPen, rect)
                Dim textY As Double = y + 4
                For Each cellLine As String In wrappedCells(colIndex)
                    DrawPdfSharpText(gfx, cellLine, x + 3, textY, 7, rowIndex = 1, colWidth - 6)
                    textY += 9
                Next
                x += colWidth
            Next
            y += rowHeight
        Next
        y += 10
    End Sub

    Private Sub DrawPdfSharpImage(document As PdfSharp.Pdf.PdfDocument, ByRef page As PdfSharp.Pdf.PdfPage, ByRef gfx As PdfSharp.Drawing.XGraphics, ByRef y As Double, filePath As String)
        If filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then
            DrawPdfSharpParagraph(document, page, gfx, y, "Image file is not available.", 9, False)
            Exit Sub
        End If

        Try
            Dim image As PdfSharp.Drawing.XImage = PdfSharp.Drawing.XImage.FromFile(filePath)
            Dim maxWidth As Double = page.Width.Point - 100
            Dim maxHeight As Double = 420
            Dim drawWidth As Double = image.PixelWidth
            Dim drawHeight As Double = image.PixelHeight
            Dim ratio As Double = Math.Min(maxWidth / drawWidth, maxHeight / drawHeight)
            If ratio < 1 Then
                drawWidth *= ratio
                drawHeight *= ratio
            End If
            EnsurePdfSharpSpace(document, page, gfx, y, drawHeight + 20)
            Dim x As Double = 50 + ((maxWidth - drawWidth) / 2)
            gfx.DrawImage(image, x, y, drawWidth, drawHeight)
            image.Dispose()
            y += drawHeight + 18
        Catch ex As Exception
            DrawPdfSharpParagraph(document, page, gfx, y, "Could not embed PNG image: " & ex.Message, 9, False)
        End Try
    End Sub

    Private Sub AddPdfSharpDiagnostics(document As PdfSharp.Pdf.PdfDocument, ByRef page As PdfSharp.Pdf.PdfPage, ByRef gfx As PdfSharp.Drawing.XGraphics, ByRef y As Double, errors As List(Of String))
        DrawPdfSharpParagraph(document, page, gfx, y, "PDF Import Diagnostics", 12, True)
        If errors Is Nothing OrElse errors.Count = 0 Then
            DrawPdfSharpParagraph(document, page, gfx, y, "No diagnostics are available.", 9, False)
            Exit Sub
        End If
        For Each errorText As String In errors
            DrawPdfSharpParagraph(document, page, gfx, y, errorText, 9, False)
        Next
    End Sub

    Private Function RenderPdfPagesToImages(pdfPath As String, errors As List(Of String)) As List(Of String)
        Dim renderedFiles As New List(Of String)()
        If pdfPath Is Nothing OrElse pdfPath.Trim() = "" OrElse Not File.Exists(pdfPath) Then
            errors.Add("PDF render skipped because file was not found.")
            Return renderedFiles
        End If

        Dim rendererPath As String = FindGhostscriptPath()
        If rendererPath.Trim() = "" Then
            errors.Add(Path.GetFileName(pdfPath) & ": Ghostscript renderer was not found. Install Ghostscript or provide its path so PDF pages can be rendered as images.")
            Return renderedFiles
        End If

        Try
            Dim outputFolder As String = Path.Combine(PreviewPackageFolder(), "RenderedPdfPages_" & SafeFilePart(Path.GetFileNameWithoutExtension(pdfPath)) & "_" & DateTime.Now.ToString("yyyyMMddHHmmssfff"))
            Directory.CreateDirectory(outputFolder)
            Dim outputPattern As String = Path.Combine(outputFolder, "page_%03d.png")
            Dim args As String = "-dSAFER -dBATCH -dNOPAUSE -sDEVICE=png16m -r144 -dTextAlphaBits=4 -dGraphicsAlphaBits=4 -sOutputFile=" & QuoteProcessArgument(outputPattern) & " " & QuoteProcessArgument(pdfPath)

            Dim psi As New System.Diagnostics.ProcessStartInfo()
            psi.FileName = rendererPath
            psi.Arguments = args
            psi.UseShellExecute = False
            psi.CreateNoWindow = True
            psi.RedirectStandardError = True
            psi.RedirectStandardOutput = True

            Using proc As System.Diagnostics.Process = System.Diagnostics.Process.Start(psi)
                If proc Is Nothing Then
                    errors.Add(Path.GetFileName(pdfPath) & ": Ghostscript process could not be started.")
                    Return renderedFiles
                End If
                If Not proc.WaitForExit(120000) Then
                    Try
                        proc.Kill()
                    Catch exKill As Exception
                    End Try
                    errors.Add(Path.GetFileName(pdfPath) & ": Ghostscript rendering timed out.")
                    Return renderedFiles
                End If
                Dim stdout As String = proc.StandardOutput.ReadToEnd()
                Dim stderr As String = proc.StandardError.ReadToEnd()
                If proc.ExitCode <> 0 Then
                    errors.Add(Path.GetFileName(pdfPath) & ": Ghostscript failed with exit code " & proc.ExitCode.ToString() & ". " & (stderr & " " & stdout).Trim())
                    Return renderedFiles
                End If
            End Using

            Dim files As String() = Directory.GetFiles(outputFolder, "page_*.png")
            Array.Sort(files, StringComparer.OrdinalIgnoreCase)
            For Each filePath As String In files
                renderedFiles.Add(filePath)
            Next
            If renderedFiles.Count = 0 Then errors.Add(Path.GetFileName(pdfPath) & ": Ghostscript completed but no page images were created.")
        Catch ex As Exception
            errors.Add(Path.GetFileName(pdfPath) & ": PDF page rendering failed. " & ex.Message)
        End Try
        Return renderedFiles
    End Function

    Private Function FindGhostscriptPath() As String
        Dim configured As String = ""
        If ConfigurationManager.AppSettings("GhostscriptPath") IsNot Nothing Then configured = ConfigurationManager.AppSettings("GhostscriptPath").ToString()
        If configured.Trim() <> "" AndAlso File.Exists(configured) Then Return configured
        If Session IsNot Nothing AndAlso Session("GhostscriptPath") IsNot Nothing Then
            configured = Session("GhostscriptPath").ToString()
            If configured.Trim() <> "" AndAlso File.Exists(configured) Then Return configured
        End If

        Dim fromPath As String = FindExecutableInPath("gswin64c.exe")
        If fromPath.Trim() <> "" Then Return fromPath
        fromPath = FindExecutableInPath("gswin32c.exe")
        If fromPath.Trim() <> "" Then Return fromPath

        Dim candidates As New List(Of String)()
        candidates.AddRange(GhostscriptCandidatesInFolder("C:\Program Files\gs"))
        candidates.AddRange(GhostscriptCandidatesInFolder("C:\Program Files (x86)\gs"))
        For Each candidate As String In candidates
            If File.Exists(candidate) Then Return candidate
        Next
        Return ""
    End Function

    Private Function GhostscriptCandidatesInFolder(rootFolder As String) As List(Of String)
        Dim candidates As New List(Of String)()
        Try
            If Not Directory.Exists(rootFolder) Then Return candidates
            For Each versionFolder As String In Directory.GetDirectories(rootFolder)
                candidates.Add(Path.Combine(versionFolder, "bin", "gswin64c.exe"))
                candidates.Add(Path.Combine(versionFolder, "bin", "gswin32c.exe"))
            Next
        Catch ex As Exception
        End Try
        Return candidates
    End Function

    Private Function FindExecutableInPath(executableName As String) As String
        Try
            Dim pathText As String = Environment.GetEnvironmentVariable("PATH")
            If pathText Is Nothing Then Return ""
            For Each folder As String In pathText.Split(";"c)
                If folder.Trim() = "" Then Continue For
                Dim candidate As String = Path.Combine(folder.Trim(), executableName)
                If File.Exists(candidate) Then Return candidate
            Next
        Catch ex As Exception
        End Try
        Return ""
    End Function

    Private Function QuoteProcessArgument(valueText As String) As String
        If valueText Is Nothing Then Return Chr(34) & Chr(34)
        Return Chr(34) & valueText.Replace(Chr(34), "\" & Chr(34)) & Chr(34)
    End Function

    Private Sub AppendPackageItemToPdfLines(lines As List(Of String), row As DataRow)
        lines.Add("")
        lines.Add("============================================================")
        lines.Add(row("Package Item").ToString())
        lines.Add("Details: " & row("Label Above Grid").ToString())
        lines.Add("Description: " & row("Description").ToString())
        lines.Add("File: " & row("File").ToString())
        lines.Add("------------------------------------------------------------")

        If row("Key").ToString().Equals("Notes", StringComparison.OrdinalIgnoreCase) Then
            AppendNotesToPdfLines(lines)
            Exit Sub
        End If

        Dim files As List(Of String) = PackageFilesForRow(row)
        If files.Count = 0 Then
            lines.Add("No existing readable file is available for this PDF export.")
            Exit Sub
        End If

        For Each filePath As String In files
            lines.Add("")
            lines.Add("File content: " & Path.GetFileName(filePath))
            If IsPdfTextReadableFile(filePath) Then
                AppendReadableFile(lines, filePath)
            Else
                Dim info As New FileInfo(filePath)
                lines.Add("Binary or formatted file included in ZIP export and represented here by reference.")
                lines.Add("File type: " & Path.GetExtension(filePath))
                lines.Add("File size: " & info.Length.ToString() & " bytes")
            End If
        Next
    End Sub

    Private Function PackageFilesForRow(row As DataRow) As List(Of String)
        Dim files As New List(Of String)()
        If row("Key").ToString().Equals("AIAnalysis", StringComparison.OrdinalIgnoreCase) AndAlso row("FullPath").ToString().Trim() = "" Then
            Dim tempFolder As String = PreviewPackageFolder()
            Directory.CreateDirectory(tempFolder)
            WriteAIAnalysisFile(tempFolder, CurrentReportData())
            row("FullPath") = Path.Combine(tempFolder, "AIAnalysis.txt")
        End If
        AddIfExists(files, row("FullPath").ToString())

        Return files
    End Function

    Private Sub AppendNotesToPdfElements(elements As List(Of PdfElement))
        Dim notes As String = txtNotes.Text
        If notes Is Nothing OrElse notes.Trim() = "" Then
            elements.Add(PdfElement.TextLine("No notes were entered."))
            Exit Sub
        End If

        notes = notes.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        For Each line As String In notes.Split(ControlChars.Lf)
            elements.Add(PdfElement.TextLine(line))
        Next
    End Sub

    Private Sub AppendNotesToPdfLines(lines As List(Of String))
        Dim notes As String = txtNotes.Text
        If notes Is Nothing OrElse notes.Trim() = "" Then
            lines.Add("No notes were entered.")
            Exit Sub
        End If

        notes = notes.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
        For Each line As String In notes.Split(ControlChars.Lf)
            lines.Add(line)
        Next
    End Sub

    Private Sub AddIfExists(files As List(Of String), filePath As String)
        If filePath IsNot Nothing AndAlso filePath.Trim() <> "" AndAlso File.Exists(filePath) Then files.Add(filePath)
    End Sub

    Private Function IsPdfTextReadableFile(filePath As String) As Boolean
        Dim ext As String = Path.GetExtension(filePath).ToLowerInvariant()
        Return ext = ".txt" OrElse ext = ".html" OrElse ext = ".htm" OrElse ext = ".xls" OrElse ext = ".xml"
    End Function

    Private Sub AppendReadableFile(lines As List(Of String), filePath As String)
        Try
            Dim content As String = File.ReadAllText(filePath)
            If Path.GetExtension(filePath).ToLowerInvariant() = ".html" OrElse Path.GetExtension(filePath).ToLowerInvariant() = ".htm" OrElse LooksLikeHtml(content) Then
                Dim tableLines As List(Of String) = HtmlTablesToLines(content)
                If tableLines.Count > 0 Then
                    For Each tableLine As String In tableLines
                        lines.Add(tableLine)
                    Next
                    Exit Sub
                End If
                content = HtmlToPlainText(content)
            End If
            content = content.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
            Dim contentLines() As String = content.Split(ControlChars.Lf)
            Dim maxLines As Integer = 500
            For i As Integer = 0 To Math.Min(contentLines.Length, maxLines) - 1
                lines.Add(contentLines(i))
            Next
            If contentLines.Length > maxLines Then lines.Add("... content truncated in PDF preview; full file is available in ZIP export.")
        Catch ex As Exception
            lines.Add("Could not read file content for PDF: " & ex.Message)
        End Try
    End Sub

    Private Sub AppendReadableFileElements(elements As List(Of PdfElement), filePath As String)
        Try
            Dim content As String = File.ReadAllText(filePath)
            Dim extension As String = Path.GetExtension(filePath).ToLowerInvariant()
            If extension = ".html" OrElse extension = ".htm" Then
                content = HtmlToPlainText(content)
            ElseIf extension = ".xls" OrElse LooksLikeHtml(content) Then
                Dim tables As List(Of List(Of List(Of String))) = HtmlTablesToPdfTables(content)
                If tables.Count > 0 Then
                    Dim tableNumber As Integer = 0
                    For Each tableRows As List(Of List(Of String)) In tables
                        tableNumber += 1
                        elements.Add(PdfElement.SubSection("Table " & tableNumber.ToString()))
                        elements.Add(PdfElement.TableElement(tableRows))
                    Next
                    Exit Sub
                End If
                content = HtmlToPlainText(content)
            End If

            content = content.Replace(vbCrLf, vbLf).Replace(vbCr, vbLf)
            Dim contentLines() As String = content.Split(ControlChars.Lf)
            Dim maxLines As Integer = 700
            For i As Integer = 0 To Math.Min(contentLines.Length, maxLines) - 1
                elements.Add(PdfElement.TextLine(contentLines(i)))
            Next
            If contentLines.Length > maxLines Then elements.Add(PdfElement.TextLine("... content truncated in PDF preview; full file is available in ZIP export."))
        Catch ex As Exception
            elements.Add(PdfElement.TextLine("Could not read file content for PDF: " & ex.Message))
        End Try
    End Sub

    Private Function HtmlTablesToPdfTables(html As String) As List(Of List(Of List(Of String)))
        Dim result As New List(Of List(Of List(Of String)))()
        If html Is Nothing OrElse Not html.ToLowerInvariant().Contains("<table") Then Return result

        Dim tableMatches As MatchCollection = Regex.Matches(html, "(?is)<table[^>]*>(.*?)</table>")
        For Each tableMatch As Match In tableMatches
            Dim rowMatches As MatchCollection = Regex.Matches(tableMatch.Value, "(?is)<tr[^>]*>(.*?)</tr>")
            Dim tableRows As New List(Of List(Of String))()
            For Each rowMatch As Match In rowMatches
                Dim cells As New List(Of String)()
                Dim cellMatches As MatchCollection = Regex.Matches(rowMatch.Value, "(?is)<t[dh][^>]*>(.*?)</t[dh]>")
                For Each cellMatch As Match In cellMatches
                    Dim cellText As String = HtmlToPlainText(cellMatch.Groups(1).Value).Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ").Trim()
                    cellText = Regex.Replace(cellText, "\s+", " ")
                    cells.Add(cellText)
                Next
                If cells.Count > 0 Then tableRows.Add(cells)
                If tableRows.Count >= 80 Then Exit For
            Next
            If tableRows.Count > 0 Then result.Add(tableRows)
        Next
        Return result
    End Function

    Private Function HtmlTablesToLines(html As String) As List(Of String)
        Dim result As New List(Of String)()
        If html Is Nothing OrElse Not html.ToLowerInvariant().Contains("<table") Then Return result

        Dim tableMatches As MatchCollection = Regex.Matches(html, "(?is)<table[^>]*>(.*?)</table>")
        Dim tableNumber As Integer = 0
        For Each tableMatch As Match In tableMatches
            tableNumber += 1
            result.Add("")
            result.Add("Table " & tableNumber.ToString())
            result.Add("")

            Dim rowMatches As MatchCollection = Regex.Matches(tableMatch.Value, "(?is)<tr[^>]*>(.*?)</tr>")
            Dim tableRows As New List(Of List(Of String))()
            For Each rowMatch As Match In rowMatches
                Dim cells As New List(Of String)()
                Dim cellMatches As MatchCollection = Regex.Matches(rowMatch.Value, "(?is)<t[dh][^>]*>(.*?)</t[dh]>")
                For Each cellMatch As Match In cellMatches
                    Dim cellText As String = HtmlToPlainText(cellMatch.Groups(1).Value).Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ").Trim()
                    cells.Add(cellText)
                Next
                If cells.Count > 0 Then tableRows.Add(cells)
                If tableRows.Count >= 60 Then Exit For
            Next

            If tableRows.Count = 0 Then Continue For
            Dim columnCount As Integer = MaxPdfTableColumns(tableRows)
            Dim widths As New List(Of Integer)()
            For colIndex As Integer = 0 To columnCount - 1
                Dim maxWidth As Integer = 8
                For Each row As List(Of String) In tableRows
                    If colIndex < row.Count Then maxWidth = Math.Max(maxWidth, Math.Min(22, row(colIndex).Length))
                Next
                widths.Add(maxWidth)
            Next

            Dim rowNumber As Integer = 0
            For Each row As List(Of String) In tableRows
                rowNumber += 1
                result.Add(FormatPdfTableRow(row, widths))
                If rowNumber = 1 Then result.Add(New String("-"c, PdfTableWidth(widths)))
            Next
            If rowMatches.Count > tableRows.Count Then result.Add("... table truncated in PDF preview.")
        Next
        Return result
    End Function

    Private Function MaxPdfTableColumns(tableRows As List(Of List(Of String))) As Integer
        Dim columnCount As Integer = 0
        For Each row As List(Of String) In tableRows
            If row.Count > columnCount Then columnCount = row.Count
        Next
        Return columnCount
    End Function

    Private Function PdfTableWidth(widths As List(Of Integer)) As Integer
        Dim totalWidth As Integer = 0
        For Each width As Integer In widths
            totalWidth += width
        Next
        totalWidth += widths.Count * 3
        Return Math.Min(92, totalWidth)
    End Function

    Private Function FormatPdfTableRow(cells As List(Of String), widths As List(Of Integer)) As String
        Dim sb As New StringBuilder()
        For i As Integer = 0 To widths.Count - 1
            Dim valueText As String = ""
            If i < cells.Count Then valueText = cells(i)
            valueText = valueText.Replace("|", "/")
            If valueText.Length > widths(i) Then valueText = valueText.Substring(0, Math.Max(0, widths(i) - 1)) & "."
            sb.Append(valueText.PadRight(widths(i))).Append(" | ")
        Next
        Return sb.ToString().TrimEnd()
    End Function

    Private Function LooksLikeHtml(content As String) As Boolean
        If content Is Nothing Then Return False
        Dim sample As String = content.TrimStart().ToLowerInvariant()
        Return sample.StartsWith("<html") OrElse sample.Contains("<table") OrElse sample.Contains("<br")
    End Function

    Private Function HtmlToPlainText(html As String) As String
        Dim text As String = html
        text = Regex.Replace(text, "(?is)<script[^>]*>.*?</script>", "")
        text = Regex.Replace(text, "(?is)<style[^>]*>.*?</style>", "")
        text = Regex.Replace(text, "(?i)<br\s*/?>", Environment.NewLine)
        text = Regex.Replace(text, "(?i)</p>|</tr>|</div>|</h[1-6]>|</li>", Environment.NewLine)
        text = Regex.Replace(text, "(?i)<li[^>]*>", "- ")
        text = Regex.Replace(text, "(?i)</td>|</th>", "    ")
        text = Regex.Replace(text, "<[^>]+>", "")
        Return HttpUtility.HtmlDecode(text)
    End Function

    Private Class PdfElement
        Public Property ElementType As String
        Public Property Text As String
        Public Property Rows As List(Of List(Of String))

        Public Shared Function Title(valueText As String) As PdfElement
            Return New PdfElement With {.ElementType = "title", .Text = valueText}
        End Function

        Public Shared Function Section(valueText As String) As PdfElement
            Return New PdfElement With {.ElementType = "section", .Text = valueText}
        End Function

        Public Shared Function SubSection(valueText As String) As PdfElement
            Return New PdfElement With {.ElementType = "subsection", .Text = valueText}
        End Function

        Public Shared Function TextLine(valueText As String) As PdfElement
            Return New PdfElement With {.ElementType = "text", .Text = valueText}
        End Function

        Public Shared Function Space() As PdfElement
            Return New PdfElement With {.ElementType = "space", .Text = ""}
        End Function

        Public Shared Function TableElement(tableRows As List(Of List(Of String))) As PdfElement
            Return New PdfElement With {.ElementType = "table", .Rows = tableRows}
        End Function

        Public Shared Function ImageElement(filePath As String) As PdfElement
            Return New PdfElement With {.ElementType = "image", .Text = filePath}
        End Function

        Public Shared Function AttachmentElement(filePath As String) As PdfElement
            Return New PdfElement With {.ElementType = "attachment", .Text = filePath}
        End Function
    End Class

    Private Class PdfImageResource
        Public Property Name As String
        Public Property Bytes As Byte()
        Public Property Width As Integer
        Public Property Height As Integer
    End Class

    Private Class PdfAttachmentResource
        Public Property FileName As String
        Public Property Bytes As Byte()
    End Class

    Private Function StyledPdf(elements As List(Of PdfElement)) As Byte()
        Dim pageStreams As New List(Of String)()
        Dim images As New List(Of PdfImageResource)()
        Dim attachments As New List(Of PdfAttachmentResource)()
        Dim content As New StringBuilder()
        Dim y As Double = 760

        For Each element As PdfElement In elements
            If element Is Nothing Then Continue For
            Select Case element.ElementType
                Case "title"
                    EnsurePdfSpace(pageStreams, content, y, 36)
                    AppendPdfText(content, element.Text, 50, y, 18, True)
                    y -= 30
                Case "section"
                    EnsurePdfSpace(pageStreams, content, y, 34)
                    AppendPdfText(content, element.Text, 50, y, 14, True)
                    y -= 22
                    AppendPdfLine(content, 50, y + 4, 562, y + 4, "0.55 0.55 0.55")
                    y -= 8
                Case "subsection"
                    EnsurePdfSpace(pageStreams, content, y, 26)
                    AppendPdfText(content, element.Text, 50, y, 11, True)
                    y -= 18
                Case "space"
                    y -= 8
                Case "table"
                    AppendPdfTable(pageStreams, content, y, element.Rows)
                Case "image"
                    AppendPdfImage(pageStreams, content, y, images, element.Text)
                Case "attachment"
                    AddPdfAttachment(attachments, element.Text)
                Case Else
                    AppendPdfParagraph(pageStreams, content, y, element.Text, 9, False)
            End Select
        Next

        If content.Length > 0 Then pageStreams.Add(content.ToString())
        If pageStreams.Count = 0 Then pageStreams.Add("")
        Return PdfFromStreams(pageStreams, images, attachments)
    End Function

    Private Sub EnsurePdfSpace(pageStreams As List(Of String), ByRef content As StringBuilder, ByRef y As Double, requiredHeight As Double)
        If y - requiredHeight >= 45 Then Exit Sub
        pageStreams.Add(content.ToString())
        content = New StringBuilder()
        y = 760
    End Sub

    Private Sub AppendPdfParagraph(pageStreams As List(Of String), ByRef content As StringBuilder, ByRef y As Double, valueText As String, fontSize As Integer, bold As Boolean)
        Dim maxChars As Integer = If(fontSize <= 8, 105, 92)
        Dim wrapped As List(Of String) = WrapPdfLine(If(valueText, ""), maxChars)
        For Each line As String In wrapped
            EnsurePdfSpace(pageStreams, content, y, fontSize + 8)
            AppendPdfText(content, line, 50, y, fontSize, bold)
            y -= fontSize + 5
        Next
    End Sub

    Private Sub AppendPdfTable(pageStreams As List(Of String), ByRef content As StringBuilder, ByRef y As Double, tableRows As List(Of List(Of String)))
        If tableRows Is Nothing OrElse tableRows.Count = 0 Then Exit Sub

        Dim columnCount As Integer = MaxPdfTableColumns(tableRows)
        If columnCount <= 0 Then Exit Sub
        If columnCount > 8 Then columnCount = 8

        Dim left As Double = 40
        Dim totalWidth As Double = 532
        Dim colWidth As Double = totalWidth / columnCount
        Dim rowIndex As Integer = 0

        For Each row As List(Of String) In tableRows
            rowIndex += 1
            Dim wrappedCells As New List(Of List(Of String))()
            Dim maxLines As Integer = 1
            For colIndex As Integer = 0 To columnCount - 1
                Dim cellText As String = ""
                If colIndex < row.Count Then cellText = row(colIndex)
                Dim cellLines As List(Of String) = WrapPdfLine(cellText, Math.Max(8, CInt(colWidth / 4.4)))
                If cellLines.Count > 4 Then
                    cellLines = cellLines.GetRange(0, 4)
                    cellLines(cellLines.Count - 1) &= "..."
                End If
                wrappedCells.Add(cellLines)
                If cellLines.Count > maxLines Then maxLines = cellLines.Count
            Next

            Dim rowHeight As Double = Math.Max(18, maxLines * 9 + 8)
            EnsurePdfSpace(pageStreams, content, y, rowHeight + 8)

            Dim x As Double = left
            For colIndex As Integer = 0 To columnCount - 1
                If rowIndex = 1 Then AppendPdfFillRect(content, x, y - rowHeight, colWidth, rowHeight, "0.90 0.96 0.90")
                AppendPdfRect(content, x, y - rowHeight, colWidth, rowHeight, "0.55 0.55 0.55")
                Dim textY As Double = y - 11
                For Each cellLine As String In wrappedCells(colIndex)
                    AppendPdfText(content, cellLine, x + 3, textY, 7, rowIndex = 1)
                    textY -= 9
                Next
                x += colWidth
            Next
            y -= rowHeight
        Next
        y -= 10
    End Sub

    Private Sub AppendPdfImage(pageStreams As List(Of String), ByRef content As StringBuilder, ByRef y As Double, images As List(Of PdfImageResource), filePath As String)
        If filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then
            AppendPdfParagraph(pageStreams, content, y, "Image file is not available.", 9, False)
            Exit Sub
        End If

        Try
            Dim imageResource As PdfImageResource = LoadPdfImageResource(filePath, images.Count + 1)
            images.Add(imageResource)

            Dim maxWidth As Double = 500
            Dim maxHeight As Double = 420
            Dim drawWidth As Double = imageResource.Width
            Dim drawHeight As Double = imageResource.Height
            Dim ratio As Double = Math.Min(maxWidth / drawWidth, maxHeight / drawHeight)
            If ratio < 1 Then
                drawWidth *= ratio
                drawHeight *= ratio
            End If

            EnsurePdfSpace(pageStreams, content, y, drawHeight + 28)
            Dim x As Double = 50 + ((500 - drawWidth) / 2)
            Dim imageY As Double = y - drawHeight
            content.Append("q").Append(vbLf)
            content.Append(PdfNumber(drawWidth)).Append(" 0 0 ").Append(PdfNumber(drawHeight)).Append(" ").Append(PdfNumber(x)).Append(" ").Append(PdfNumber(imageY)).Append(" cm").Append(vbLf)
            content.Append("/").Append(imageResource.Name).Append(" Do").Append(vbLf)
            content.Append("Q").Append(vbLf)
            y = imageY - 18
        Catch ex As Exception
            AppendPdfParagraph(pageStreams, content, y, "Could not embed PNG image: " & ex.Message, 9, False)
        End Try
    End Sub

    Private Function LoadPdfImageResource(filePath As String, imageNumber As Integer) As PdfImageResource
        Using sourceImage As System.Drawing.Image = System.Drawing.Image.FromFile(filePath)
            Using bitmap As New System.Drawing.Bitmap(sourceImage.Width, sourceImage.Height)
                Using graphics As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(bitmap)
                    graphics.Clear(System.Drawing.Color.White)
                    graphics.DrawImage(sourceImage, 0, 0, sourceImage.Width, sourceImage.Height)
                End Using
                Using ms As New MemoryStream()
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
                    Return New PdfImageResource With {.Name = "Im" & imageNumber.ToString(), .Bytes = ms.ToArray(), .Width = bitmap.Width, .Height = bitmap.Height}
                End Using
            End Using
        End Using
    End Function

    Private Sub AddPdfAttachment(attachments As List(Of PdfAttachmentResource), filePath As String)
        If filePath Is Nothing OrElse filePath.Trim() = "" OrElse Not File.Exists(filePath) Then Exit Sub
        attachments.Add(New PdfAttachmentResource With {.FileName = Path.GetFileName(filePath), .Bytes = File.ReadAllBytes(filePath)})
    End Sub

    Private Sub AppendPdfText(content As StringBuilder, valueText As String, x As Double, y As Double, fontSize As Integer, bold As Boolean)
        content.Append("BT").Append(vbLf)
        content.Append(If(bold, "/F2 ", "/F1 ")).Append(fontSize.ToString()).Append(" Tf").Append(vbLf)
        content.Append(PdfNumber(x)).Append(" ").Append(PdfNumber(y)).Append(" Td").Append(vbLf)
        content.Append("(").Append(PdfEscape(valueText)).Append(") Tj").Append(vbLf)
        content.Append("ET").Append(vbLf)
    End Sub

    Private Sub AppendPdfLine(content As StringBuilder, x1 As Double, y1 As Double, x2 As Double, y2 As Double, color As String)
        content.Append(color).Append(" RG").Append(vbLf)
        content.Append(PdfNumber(x1)).Append(" ").Append(PdfNumber(y1)).Append(" m ").Append(PdfNumber(x2)).Append(" ").Append(PdfNumber(y2)).Append(" l S").Append(vbLf)
        content.Append("0 0 0 RG").Append(vbLf)
    End Sub

    Private Sub AppendPdfRect(content As StringBuilder, x As Double, y As Double, width As Double, height As Double, color As String)
        content.Append(color).Append(" RG").Append(vbLf)
        content.Append(PdfNumber(x)).Append(" ").Append(PdfNumber(y)).Append(" ").Append(PdfNumber(width)).Append(" ").Append(PdfNumber(height)).Append(" re S").Append(vbLf)
        content.Append("0 0 0 RG").Append(vbLf)
    End Sub

    Private Sub AppendPdfFillRect(content As StringBuilder, x As Double, y As Double, width As Double, height As Double, color As String)
        content.Append(color).Append(" rg").Append(vbLf)
        content.Append(PdfNumber(x)).Append(" ").Append(PdfNumber(y)).Append(" ").Append(PdfNumber(width)).Append(" ").Append(PdfNumber(height)).Append(" re f").Append(vbLf)
        content.Append("0 0 0 rg").Append(vbLf)
    End Sub

    Private Function PdfNumber(value As Double) As String
        Return value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
    End Function

    Private Function PdfFromStreams(pageStreams As List(Of String), images As List(Of PdfImageResource), attachments As List(Of PdfAttachmentResource)) As Byte()
        If images Is Nothing Then images = New List(Of PdfImageResource)()
        If attachments Is Nothing Then attachments = New List(Of PdfAttachmentResource)()

        Dim firstImageObject As Integer = 3 + pageStreams.Count * 2
        Dim firstAttachmentObject As Integer = firstImageObject + images.Count
        Dim namesBuilder As New StringBuilder()
        For i As Integer = 0 To attachments.Count - 1
            Dim fileSpecObjectNumber As Integer = firstAttachmentObject + (i * 2)
            namesBuilder.Append("(").Append(PdfEscape(attachments(i).FileName)).Append(") ").Append(fileSpecObjectNumber.ToString()).Append(" 0 R ")
        Next

        Dim objects As New List(Of Byte())()
        Dim catalog As String = "<< /Type /Catalog /Pages 2 0 R"
        If attachments.Count > 0 Then catalog &= " /Names << /EmbeddedFiles << /Names [" & namesBuilder.ToString().Trim() & "] >> >>"
        catalog &= " >>"
        objects.Add(PdfAsciiBytes(catalog))
        Dim kids As New StringBuilder()
        For i As Integer = 0 To pageStreams.Count - 1
            kids.Append((3 + i * 2).ToString()).Append(" 0 R ")
        Next
        objects.Add(PdfAsciiBytes("<< /Type /Pages /Kids [" & kids.ToString().Trim() & "] /Count " & pageStreams.Count.ToString() & " >>"))

        For i As Integer = 0 To pageStreams.Count - 1
            Dim pageObjectNumber As Integer = 3 + i * 2
            Dim contentObjectNumber As Integer = pageObjectNumber + 1
            Dim xObjects As New StringBuilder()
            If images.Count > 0 Then
                xObjects.Append(" /XObject << ")
                For imageIndex As Integer = 0 To images.Count - 1
                    xObjects.Append("/").Append(images(imageIndex).Name).Append(" ").Append((firstImageObject + imageIndex).ToString()).Append(" 0 R ")
                Next
                xObjects.Append(">>")
            End If
            objects.Add(PdfAsciiBytes("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> /F2 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >> >>" & xObjects.ToString() & " >> /Contents " & contentObjectNumber.ToString() & " 0 R >>"))
            Dim footer As String = "BT /F1 8 Tf 500 24 Td (" & PdfEscape("Page " & (i + 1).ToString() & " of " & pageStreams.Count.ToString()) & ") Tj ET" & vbLf
            Dim streamText As String = pageStreams(i) & footer
            Dim streamBytes() As Byte = Encoding.ASCII.GetBytes(streamText)
            objects.Add(PdfStreamBytes("<< /Length " & streamBytes.Length.ToString() & " >>", streamBytes))
        Next

        For Each image As PdfImageResource In images
            objects.Add(PdfStreamBytes("<< /Type /XObject /Subtype /Image /Width " & image.Width.ToString() & " /Height " & image.Height.ToString() & " /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length " & image.Bytes.Length.ToString() & " >>", image.Bytes))
        Next

        For Each attachment As PdfAttachmentResource In attachments
            Dim embeddedFileObjectNumber As Integer = objects.Count + 2
            objects.Add(PdfAsciiBytes("<< /Type /Filespec /F (" & PdfEscape(attachment.FileName) & ") /EF << /F " & embeddedFileObjectNumber.ToString() & " 0 R >> >>"))
            objects.Add(PdfStreamBytes("<< /Type /EmbeddedFile /Subtype /application#2Fpdf /Length " & attachment.Bytes.Length.ToString() & " >>", attachment.Bytes))
        Next

        Dim ms As New MemoryStream()
        WritePdfAscii(ms, "%PDF-1.4" & vbLf)
        Dim offsets As New List(Of Long)()
        offsets.Add(0)
        For i As Integer = 0 To objects.Count - 1
            offsets.Add(ms.Position)
            WritePdfAscii(ms, (i + 1).ToString() & " 0 obj" & vbLf)
            ms.Write(objects(i), 0, objects(i).Length)
            WritePdfAscii(ms, vbLf & "endobj" & vbLf)
        Next
        Dim xrefPosition As Long = ms.Position
        WritePdfAscii(ms, "xref" & vbLf)
        WritePdfAscii(ms, "0 " & (objects.Count + 1).ToString() & vbLf)
        WritePdfAscii(ms, "0000000000 65535 f " & vbLf)
        For i As Integer = 1 To offsets.Count - 1
            WritePdfAscii(ms, offsets(i).ToString("0000000000") & " 00000 n " & vbLf)
        Next
        WritePdfAscii(ms, "trailer" & vbLf)
        WritePdfAscii(ms, "<< /Size " & (objects.Count + 1).ToString() & " /Root 1 0 R >>" & vbLf)
        WritePdfAscii(ms, "startxref" & vbLf)
        WritePdfAscii(ms, xrefPosition.ToString() & vbLf)
        WritePdfAscii(ms, "%%EOF")
        Return ms.ToArray()
    End Function

    Private Function PdfAsciiBytes(valueText As String) As Byte()
        Return Encoding.ASCII.GetBytes(valueText)
    End Function

    Private Function PdfStreamBytes(dictionaryText As String, streamBytes As Byte()) As Byte()
        Dim prefix As Byte() = Encoding.ASCII.GetBytes(dictionaryText & vbLf & "stream" & vbLf)
        Dim suffix As Byte() = Encoding.ASCII.GetBytes(vbLf & "endstream")
        Dim output(prefix.Length + streamBytes.Length + suffix.Length - 1) As Byte
        System.Buffer.BlockCopy(prefix, 0, output, 0, prefix.Length)
        System.Buffer.BlockCopy(streamBytes, 0, output, prefix.Length, streamBytes.Length)
        System.Buffer.BlockCopy(suffix, 0, output, prefix.Length + streamBytes.Length, suffix.Length)
        Return output
    End Function

    Private Sub WritePdfAscii(ms As MemoryStream, valueText As String)
        Dim bytes As Byte() = Encoding.ASCII.GetBytes(valueText)
        ms.Write(bytes, 0, bytes.Length)
    End Sub

    Private Function SimplePdf(sourceLines As List(Of String)) As Byte()
        Dim pages As New List(Of List(Of String))()
        Dim current As New List(Of String)()
        For Each sourceLine As String In sourceLines
            Dim wrapped As List(Of String) = WrapPdfLine(sourceLine, 92)
            For Each line As String In wrapped
                If current.Count >= 48 Then
                    pages.Add(current)
                    current = New List(Of String)()
                End If
                current.Add(line)
            Next
        Next
        If current.Count > 0 Then pages.Add(current)
        If pages.Count = 0 Then pages.Add(New List(Of String)(New String() {"DataAI Export Package"}))

        Dim objects As New List(Of String)()
        objects.Add("<< /Type /Catalog /Pages 2 0 R >>")
        Dim kids As New StringBuilder()
        For i As Integer = 0 To pages.Count - 1
            kids.Append((3 + i * 2).ToString()).Append(" 0 R ")
        Next
        objects.Add("<< /Type /Pages /Kids [" & kids.ToString().Trim() & "] /Count " & pages.Count.ToString() & " >>")

        For i As Integer = 0 To pages.Count - 1
            Dim pageObjectNumber As Integer = 3 + i * 2
            Dim contentObjectNumber As Integer = pageObjectNumber + 1
            objects.Add("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> >> >> /Contents " & contentObjectNumber.ToString() & " 0 R >>")
            Dim streamText As String = PdfPageStream(pages(i), i + 1, pages.Count)
            Dim streamBytes() As Byte = Encoding.ASCII.GetBytes(streamText)
            objects.Add("<< /Length " & streamBytes.Length.ToString() & " >>" & vbLf & "stream" & vbLf & streamText & "endstream")
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
            writer.Write(objects(i) & vbLf)
            writer.Write("endobj" & vbLf)
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

    Private Function PdfPageStream(lines As List(Of String), pageNumber As Integer, pageCount As Integer) As String
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

    Private Function WrapPdfLine(valueText As String, width As Integer) As List(Of String)
        Dim result As New List(Of String)()
        If valueText Is Nothing Then
            result.Add("")
            Return result
        End If
        Dim text As String = valueText.Replace(vbTab, "    ")
        If text.Length = 0 Then
            result.Add("")
            Return result
        End If

        Dim words As String() = Regex.Split(text.Trim(), "\s+")
        Dim line As New StringBuilder()
        For Each word As String In words
            If word.Length = 0 Then Continue For

            If word.Length > width Then
                If line.Length > 0 Then
                    result.Add(line.ToString())
                    line.Length = 0
                End If
                Dim remaining As String = word
                While remaining.Length > width
                    result.Add(remaining.Substring(0, width))
                    remaining = remaining.Substring(width)
                End While
                If remaining.Length > 0 Then line.Append(remaining)
            ElseIf line.Length = 0 Then
                line.Append(word)
            ElseIf line.Length + 1 + word.Length <= width Then
                line.Append(" ").Append(word)
            Else
                result.Add(line.ToString())
                line.Length = 0
                line.Append(word)
            End If
        Next

        If line.Length > 0 Then result.Add(line.ToString())
        If result.Count = 0 Then result.Add("")
        Return result
    End Function

    Private Function PdfEscape(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Dim text As String = valueText
        text = Regex.Replace(text, "[^\u0009\u000A\u000D\u0020-\u007E]", "?")
        text = text.Replace("\", "\\").Replace("(", "\(").Replace(")", "\)")
        Return text
    End Function

    Private Function PackageHeader() As String
        Dim sb As New StringBuilder()
        sb.AppendLine("DataAI Export Package")
        sb.AppendLine("Report: " & FieldText(Session("REPORTID")))
        sb.AppendLine("Title: " & FieldText(Session("REPTITLE")))
        sb.AppendLine("Created: " & DateTime.Now.ToString())
        Return sb.ToString()
    End Function

    Private Function ReportDefinitionsText() As String
        Dim sb As New StringBuilder()
        sb.AppendLine(PackageHeader())
        sb.AppendLine("Report Definitions")
        If Session("REPORTID") Is Nothing Then Return sb.ToString()

        Dim reportId As String = Session("REPORTID").ToString().Replace("'", "''")
        Dim info As DataView = mRecords("SELECT * FROM OURReportInfo WHERE ReportID='" & reportId & "'")
        If info IsNot Nothing AndAlso info.Table IsNot Nothing AndAlso info.Table.Rows.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine("Report Definition Textboxes")
            AppendRowValues(sb, info.Table.Rows(0))
        End If

        sb.AppendLine()
        sb.AppendLine("SQL Query Textbox")
        If info IsNot Nothing AndAlso info.Table IsNot Nothing AndAlso info.Table.Rows.Count > 0 AndAlso info.Table.Columns.Contains("SQLquerytext") Then
            sb.AppendLine(info.Table.Rows(0)("SQLquerytext").ToString())
        End If

        Dim formatRows As DataView = mRecords("SELECT * FROM OURReportFormat WHERE ReportID='" & reportId & "' ORDER BY Prop, Indx")
        If formatRows IsNot Nothing AndAlso formatRows.Table IsNot Nothing AndAlso formatRows.Table.Rows.Count > 0 Then
            sb.AppendLine()
            sb.AppendLine("RDL Format Textboxes")
            For Each row As DataRow In formatRows.Table.Rows
                Dim rowText As String = FormatDefinitionRow(row)
                If rowText.Trim() <> "" Then
                    sb.AppendLine(rowText)
                    sb.AppendLine()
                End If
            Next
        End If
        Return sb.ToString()
    End Function

    Private Function FormatDefinitionRow(row As DataRow) As String
        Dim sb As New StringBuilder()
        AppendDefinitionValue(sb, "Field/Item", SafeRowValue(row, "Val"))
        AppendDefinitionValue(sb, "Friendly name", SafeRowValue(row, "Prop1"))
        AppendDefinitionValue(sb, "Expression", SafeRowValue(row, "Prop2"))
        AppendDefinitionValue(sb, "Comments", SafeRowValue(row, "Comments"))
        AppendDefinitionValue(sb, "Property", SafeRowValue(row, "Prop"))
        AppendDefinitionValue(sb, "Order", SafeRowValue(row, "Order"))
        Return sb.ToString()
    End Function

    Private Sub AppendDefinitionValue(sb As StringBuilder, labelText As String, valueText As String)
        If valueText Is Nothing OrElse valueText.Trim() = "" Then Exit Sub
        sb.AppendLine(labelText & ": " & valueText.Trim())
    End Sub

    Private Sub AppendRowValues(sb As StringBuilder, row As DataRow)
        For Each col As DataColumn In row.Table.Columns
            If col.ColumnName.StartsWith("Param", StringComparison.OrdinalIgnoreCase) Then Continue For
            Dim valueText As String = row(col).ToString().Trim()
            If valueText = "" Then Continue For
            sb.AppendLine(col.ColumnName & ": " & valueText)
        Next
    End Sub

    Private Function SafeRowValue(row As DataRow, columnName As String) As String
        If row.Table.Columns.Contains(columnName) Then Return row(columnName).ToString()
        Return ""
    End Function

    Private Sub WriteRdlDefinitionFile(packageFolder As String)
        If Session("REPORTID") Is Nothing Then Exit Sub

        Dim reportId As String = Session("REPORTID").ToString()
        Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & reportId.Replace("'", "''") & "' AND Type='RDL'")
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then Exit Sub

        Dim rdlText As String = dv.Table.Rows(0)("FileText").ToString()
        If dv.Table.Columns.Contains("UserFile") AndAlso dv.Table.Rows(0)("UserFile").ToString().Trim() <> "" Then
            rdlText = dv.Table.Rows(0)("UserFile").ToString()
        End If
        If rdlText.Trim() = "" Then Exit Sub

        rdlText = RemoveRdlConnectionString(rdlText)
        File.WriteAllText(Path.Combine(packageFolder, SafeFilePart(reportId) & ".rdl"), rdlText, Encoding.UTF8)
    End Sub

    Private Function RemoveRdlConnectionString(rdlText As String) As String
        Dim startIndex As Integer = rdlText.IndexOf("<ConnectString>")
        Dim endIndex As Integer = rdlText.IndexOf("</ConnectString>")
        If startIndex >= 0 AndAlso endIndex > startIndex Then
            Return rdlText.Substring(0, startIndex) & "<ConnectString>" & rdlText.Substring(endIndex)
        End If
        Return rdlText
    End Function

    Private Sub WriteReportPdfFile(packageFolder As String, reportData As DataTable)
        If reportData Is Nothing Then Exit Sub

        Dim rdlText As String = CurrentReportRdlText()
        If rdlText.Trim() = "" Then
            File.WriteAllText(Path.Combine(packageFolder, "ReportPdfError.txt"), "Report RDL was not found.", Encoding.UTF8)
            Exit Sub
        End If

        Dim report As New LocalReport()
        Using textReader As New StringReader(rdlText)
            report.LoadReportDefinition(textReader)
        End Using
        report.DataSources.Clear()
        report.DataSources.Add(New ReportDataSource(FieldText(Session("REPORTID")), reportData))

        Dim mimeType As String = ""
        Dim reportEncoding As String = ""
        Dim fileNameExtension As String = ""
        Dim streams As String() = Nothing
        Dim warnings As Warning() = Nothing
        Dim pageWidth As String = FieldText(Session("pagewidth"))
        Dim pageHeight As String = FieldText(Session("pageheight"))
        If pageWidth.Trim() = "" Then pageWidth = "11in"
        If pageHeight.Trim() = "" Then pageHeight = "11in"
        Dim deviceInfo As String = "<DeviceInfo><OutputFormat>PDF</OutputFormat><PageWidth>" & pageWidth & "</PageWidth><PageHeight>" & pageHeight & "</PageHeight><MarginTop>0in</MarginTop><MarginLeft>0in</MarginLeft><MarginRight>0in</MarginRight><MarginBottom>0in</MarginBottom></DeviceInfo>"

        Dim bytes() As Byte = report.Render("PDF", deviceInfo, mimeType, reportEncoding, fileNameExtension, streams, warnings)
        If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
            File.WriteAllBytes(Path.Combine(packageFolder, "Report.pdf"), bytes)
        End If
    End Sub

    Private Sub WriteAIAnalysisFile(packageFolder As String, reportData As DataTable)
        Dim question As String = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc... providing comparison between groups."
        Dim prompt As String = question
        If reportData IsNot Nothing Then
            Session("dataTable") = reportData
            Session("DataToChatAI") = ExportToCSVtext(reportData, Chr(9))
            prompt &= Environment.NewLine() & Session("DataToChatAI").ToString()
        End If
        Session("QuestionToAI") = question

        Dim aiOutput As String = GenerateAIAnalysisOutput(prompt)
        File.WriteAllText(Path.Combine(packageFolder, "AIAnalysis.txt"), aiOutput, Encoding.UTF8)
    End Sub

    Private Function GenerateAIAnalysisOutput(prompt As String) As String
        Dim apiKey As String = SettingText("openaikey", "OpenAIkey")
        Dim organization As String = SettingText("openaiorganization", "OpenAIOrganization")
        Dim apiUrl As String = SettingText("apiURL", "OpenAIurl")
        Dim model As String = SettingText("openaimodel", "OpenAImodel")
        Dim maxTokens As Integer = 128000

        If ConfigurationManager.AppSettings("openaimaxTokens") IsNot Nothing AndAlso IsNumeric(ConfigurationManager.AppSettings("openaimaxTokens").ToString()) Then
            maxTokens = CInt(ConfigurationManager.AppSettings("openaimaxTokens").ToString())
        ElseIf Session("maxTokens") IsNot Nothing AndAlso IsNumeric(Session("maxTokens").ToString()) Then
            maxTokens = CInt(Session("maxTokens").ToString())
        End If

        If apiKey.Trim() = "" Then Return "OpenAI user setting is not defined. AI analysis was not generated."
        If apiUrl.Trim() = "" Then Return "OpenAI API URL is not defined. AI analysis was not generated."
        If model.Trim() = "" Then Return "OpenAI model is not defined. AI analysis was not generated."

        If prompt.Length > maxTokens Then
            prompt = prompt.Substring(0, maxTokens)
            Dim lastBreak As Integer = prompt.LastIndexOf(Environment.NewLine())
            If lastBreak > 0 Then prompt = prompt.Substring(0, lastBreak)
        End If

        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim request As HttpWebRequest = CType(WebRequest.Create(apiUrl), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/json"
            request.Headers.Add("Authorization", "Bearer " & apiKey)
            If organization.Trim() <> "" Then request.Headers.Add("OpenAI-Organization", organization)

            Dim data As String = "{" & """model"":""" & model & """," & """messages"": [{""role"":""user"", ""content"": """ & JsonEscape(prompt) & """}]}"
            Using streamWriter As New StreamWriter(request.GetRequestStream())
                streamWriter.Write(data)
                streamWriter.Flush()
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim jsonText As String = streamReader.ReadToEnd()
                    Dim serializer As New System.Web.Script.Serialization.JavaScriptSerializer()
                    Dim json As Hashtable = serializer.Deserialize(Of Hashtable)(jsonText)
                    If json.ContainsKey("choices") Then
                        Dim choices As Object() = CType(json("choices"), Object())
                        If choices.Length > 0 Then
                            Dim firstChoice As Dictionary(Of String, Object) = CType(choices(0), Dictionary(Of String, Object))
                            If firstChoice.ContainsKey("message") Then
                                Dim message As Dictionary(Of String, Object) = CType(firstChoice("message"), Dictionary(Of String, Object))
                                If message.ContainsKey("content") Then Return CType(message("content"), String)
                            End If
                        End If
                    End If
                    Return "AI analysis response did not contain output text."
                End Using
            End Using
        Catch ex As Exception
            Return "ERROR!! AI analysis was not generated. " & ex.Message
        End Try
    End Function

    Private Function SettingText(appKey As String, sessionKey As String) As String
        Try
            If ConfigurationManager.AppSettings(appKey) IsNot Nothing AndAlso ConfigurationManager.AppSettings(appKey).ToString().Trim() <> "" Then
                Return ConfigurationManager.AppSettings(appKey).ToString().Trim()
            End If
        Catch ex As Exception
        End Try
        If Session(sessionKey) IsNot Nothing Then Return Session(sessionKey).ToString().Trim()
        Return ""
    End Function

    Private Function JsonEscape(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Return valueText.Replace("\", "\\").Replace("""", "\""").Replace(vbCrLf, "\n").Replace(vbCr, "\r").Replace(vbLf, "\n").Replace(vbTab, "\t")
    End Function

    Private Function CurrentReportRdlText() As String
        If Session("REPORTID") Is Nothing Then Return ""
        Dim reportId As String = Session("REPORTID").ToString()
        Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & reportId.Replace("'", "''") & "' AND Type='RDL'")
        If dv IsNot Nothing AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
            Dim rdlText As String = dv.Table.Rows(0)("FileText").ToString()
            If rdlText.Trim() <> "" Then Return rdlText
            If dv.Table.Columns.Contains("Path") AndAlso dv.Table.Rows(0)("Path").ToString().Trim() <> "" Then
                Dim rdlPath As String = dv.Table.Rows(0)("Path").ToString().Replace("|", "\")
                If File.Exists(rdlPath) Then Return File.ReadAllText(rdlPath)
            End If
        End If

        If applpath Is Nothing OrElse applpath.Trim() = "" Then applpath = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim filePath As String = Path.Combine(applpath, "RDLFILES\" & reportId & ".rdl")
        If File.Exists(filePath) Then Return File.ReadAllText(filePath)
        Return ""
    End Function

    Private Sub WriteChartsPackage(packageFolder As String, reportData As DataTable)
        Dim chartsFolder As String = Path.Combine(packageFolder, "Charts")
        Directory.CreateDirectory(chartsFolder)

        If reportData Is Nothing OrElse reportData.Rows.Count = 0 Then
            File.WriteAllText(Path.Combine(chartsFolder, "ChartsSummary.txt"), "No report data available for chart export.", Encoding.UTF8)
            Exit Sub
        End If

        Dim categoryColumn As String = FirstCategoryColumn(reportData)
        Dim numericColumn As String = FirstNumericColumn(reportData)
        Dim dateColumn As String = FirstDateColumn(reportData)

        Dim sb As New StringBuilder()
        sb.AppendLine(PackageHeader())
        sb.AppendLine("Charts")
        sb.AppendLine("Chart data are calculated from the current report data and exported as chart-ready CSV files.")
        sb.AppendLine("Category field: " & categoryColumn)
        sb.AppendLine("Value field: " & numericColumn)
        sb.AppendLine("Date field: " & dateColumn)

        If categoryColumn <> "" AndAlso numericColumn <> "" Then
            Dim grouped As DataTable = BuildGroupedChartData(reportData, categoryColumn, numericColumn)
            File.WriteAllText(Path.Combine(chartsFolder, "BarChart_GroupTotals.csv"), ExportToCSVtext(grouped, ",", "", ""), Encoding.UTF8)
            File.WriteAllText(Path.Combine(chartsFolder, "PieChart_GroupShares.csv"), ExportToCSVtext(grouped, ",", "", ""), Encoding.UTF8)
            File.WriteAllText(Path.Combine(chartsFolder, "BarChart_GroupTotals.svg"), BuildBarChartSvg(grouped, "Category", "Value", "Group Totals"), Encoding.UTF8)
            File.WriteAllText(Path.Combine(chartsFolder, "PieChart_GroupShares.svg"), BuildPieChartSvg(grouped, "Category", "Value", "Group Shares"), Encoding.UTF8)
            sb.AppendLine("BarChart_GroupTotals.csv: category totals for a bar chart.")
            sb.AppendLine("PieChart_GroupShares.csv: same grouped totals for a pie chart.")
            sb.AppendLine("BarChart_GroupTotals.svg: visible bar chart.")
            sb.AppendLine("PieChart_GroupShares.svg: visible pie chart.")
        Else
            sb.AppendLine("Bar and pie chart data were not created because a category field and numeric value field were not found.")
        End If

        If dateColumn <> "" AndAlso numericColumn <> "" Then
            Dim timeData As DataTable = BuildTimeChartData(reportData, dateColumn, numericColumn)
            File.WriteAllText(Path.Combine(chartsFolder, "LineChart_TimeTotals.csv"), ExportToCSVtext(timeData, ",", "", ""), Encoding.UTF8)
            File.WriteAllText(Path.Combine(chartsFolder, "LineChart_TimeTotals.svg"), BuildLineChartSvg(timeData, "Date", "Value", "Time Totals"), Encoding.UTF8)
            sb.AppendLine("LineChart_TimeTotals.csv: date totals for a line chart.")
            sb.AppendLine("LineChart_TimeTotals.svg: visible line chart.")
        Else
            sb.AppendLine("Line chart data were not created because a date field and numeric value field were not found.")
        End If

        File.WriteAllText(Path.Combine(chartsFolder, "Charts.html"), BuildChartsHtml(chartsFolder), Encoding.UTF8)
        File.WriteAllText(Path.Combine(chartsFolder, "ChartsSummary.txt"), sb.ToString(), Encoding.UTF8)
    End Sub

    Private Function BuildChartsHtml(chartsFolder As String) As String
        Dim sb As New StringBuilder()
        sb.AppendLine("<html><head><title>Charts</title></head><body style=""font-family:Arial"">")
        sb.AppendLine("<h2>Export Package Charts</h2>")
        For Each chartFile As String In Directory.GetFiles(chartsFolder, "*.svg")
            Dim fileName As String = Path.GetFileName(chartFile)
            sb.AppendLine("<h3>" & HtmlEncodeText(Path.GetFileNameWithoutExtension(chartFile)) & "</h3>")
            sb.AppendLine("<img src=""" & HtmlEncodeText(fileName) & """ style=""max-width:100%; border:1px solid #ddd;"" />")
        Next
        sb.AppendLine("</body></html>")
        Return sb.ToString()
    End Function

    Private Function BuildBarChartSvg(dt As DataTable, labelColumn As String, valueColumn As String, title As String) As String
        Dim width As Integer = 900
        Dim barHeight As Integer = 28
        Dim top As Integer = 55
        Dim left As Integer = 190
        Dim maxRows As Integer = Math.Min(dt.Rows.Count, 20)
        Dim height As Integer = top + Math.Max(1, maxRows) * (barHeight + 8) + 35
        Dim maxValue As Double = MaxColumnValue(dt, valueColumn)
        If maxValue <= 0 Then maxValue = 1

        Dim sb As New StringBuilder()
        sb.AppendLine("<svg xmlns=""http://www.w3.org/2000/svg"" width=""" & width & """ height=""" & height & """ viewBox=""0 0 " & width & " " & height & """>")
        sb.AppendLine("<rect width=""100%"" height=""100%"" fill=""white""/>")
        sb.AppendLine("<text x=""20"" y=""30"" font-family=""Arial"" font-size=""20"" font-weight=""bold"">" & XmlEncodeText(title) & "</text>")
        For i As Integer = 0 To maxRows - 1
            Dim row As DataRow = dt.Rows(i)
            Dim y As Integer = top + i * (barHeight + 8)
            Dim value As Double = NumericValue(row(valueColumn))
            Dim barWidth As Integer = CInt((width - left - 80) * value / maxValue)
            sb.AppendLine("<text x=""15"" y=""" & (y + 19).ToString() & """ font-family=""Arial"" font-size=""12"">" & XmlEncodeText(ShortText(row(labelColumn).ToString(), 26)) & "</text>")
            sb.AppendLine("<rect x=""" & left & """ y=""" & y & """ width=""" & barWidth & """ height=""" & barHeight & """ fill=""#4f81bd""/>")
            sb.AppendLine("<text x=""" & (left + barWidth + 6).ToString() & """ y=""" & (y + 19).ToString() & """ font-family=""Arial"" font-size=""12"">" & value.ToString("0.##") & "</text>")
        Next
        sb.AppendLine("</svg>")
        Return sb.ToString()
    End Function

    Private Function BuildPieChartSvg(dt As DataTable, labelColumn As String, valueColumn As String, title As String) As String
        Dim width As Integer = 900
        Dim height As Integer = 520
        Dim cx As Double = 260
        Dim cy As Double = 275
        Dim r As Double = 180
        Dim total As Double = MaxColumnSum(dt, valueColumn)
        If total <= 0 Then total = 1
        Dim colors() As String = {"#4f81bd", "#c0504d", "#9bbb59", "#8064a2", "#4bacc6", "#f79646", "#2f5597", "#7f6000", "#00b050", "#7030a0"}
        Dim angle As Double = -Math.PI / 2
        Dim maxRows As Integer = Math.Min(dt.Rows.Count, 10)

        Dim sb As New StringBuilder()
        sb.AppendLine("<svg xmlns=""http://www.w3.org/2000/svg"" width=""" & width & """ height=""" & height & """ viewBox=""0 0 " & width & " " & height & """>")
        sb.AppendLine("<rect width=""100%"" height=""100%"" fill=""white""/>")
        sb.AppendLine("<text x=""20"" y=""30"" font-family=""Arial"" font-size=""20"" font-weight=""bold"">" & XmlEncodeText(title) & "</text>")
        For i As Integer = 0 To maxRows - 1
            Dim value As Double = NumericValue(dt.Rows(i)(valueColumn))
            Dim slice As Double = value / total * 2 * Math.PI
            Dim endAngle As Double = angle + slice
            Dim largeArc As Integer = If(slice > Math.PI, 1, 0)
            Dim x1 As Double = cx + r * Math.Cos(angle)
            Dim y1 As Double = cy + r * Math.Sin(angle)
            Dim x2 As Double = cx + r * Math.Cos(endAngle)
            Dim y2 As Double = cy + r * Math.Sin(endAngle)
            sb.AppendLine("<path d=""M " & cx.ToString("0.##") & " " & cy.ToString("0.##") & " L " & x1.ToString("0.##") & " " & y1.ToString("0.##") & " A " & r.ToString("0.##") & " " & r.ToString("0.##") & " 0 " & largeArc & " 1 " & x2.ToString("0.##") & " " & y2.ToString("0.##") & " Z"" fill=""" & colors(i Mod colors.Length) & """ stroke=""white"" stroke-width=""1""/>")
            Dim ly As Integer = 90 + i * 30
            sb.AppendLine("<rect x=""520"" y=""" & (ly - 14).ToString() & """ width=""18"" height=""18"" fill=""" & colors(i Mod colors.Length) & """/>")
            sb.AppendLine("<text x=""548"" y=""" & ly & """ font-family=""Arial"" font-size=""13"">" & XmlEncodeText(ShortText(dt.Rows(i)(labelColumn).ToString(), 34)) & " (" & (value / total).ToString("0.0%") & ")</text>")
            angle = endAngle
        Next
        sb.AppendLine("</svg>")
        Return sb.ToString()
    End Function

    Private Function BuildLineChartSvg(dt As DataTable, labelColumn As String, valueColumn As String, title As String) As String
        Dim width As Integer = 900
        Dim height As Integer = 520
        Dim left As Integer = 80
        Dim top As Integer = 60
        Dim plotWidth As Integer = 760
        Dim plotHeight As Integer = 360
        Dim maxRows As Integer = Math.Min(dt.Rows.Count, 60)
        Dim maxValue As Double = MaxColumnValue(dt, valueColumn)
        If maxValue <= 0 Then maxValue = 1
        Dim points As New StringBuilder()

        For i As Integer = 0 To maxRows - 1
            Dim x As Double = left + If(maxRows <= 1, 0, (plotWidth * i / (maxRows - 1)))
            Dim y As Double = top + plotHeight - (plotHeight * NumericValue(dt.Rows(i)(valueColumn)) / maxValue)
            points.Append(x.ToString("0.##") & "," & y.ToString("0.##") & " ")
        Next

        Dim sb As New StringBuilder()
        sb.AppendLine("<svg xmlns=""http://www.w3.org/2000/svg"" width=""" & width & """ height=""" & height & """ viewBox=""0 0 " & width & " " & height & """>")
        sb.AppendLine("<rect width=""100%"" height=""100%"" fill=""white""/>")
        sb.AppendLine("<text x=""20"" y=""30"" font-family=""Arial"" font-size=""20"" font-weight=""bold"">" & XmlEncodeText(title) & "</text>")
        sb.AppendLine("<line x1=""" & left & """ y1=""" & top & """ x2=""" & left & """ y2=""" & (top + plotHeight) & """ stroke=""#888""/>")
        sb.AppendLine("<line x1=""" & left & """ y1=""" & (top + plotHeight) & """ x2=""" & (left + plotWidth) & """ y2=""" & (top + plotHeight) & """ stroke=""#888""/>")
        sb.AppendLine("<polyline fill=""none"" stroke=""#4f81bd"" stroke-width=""3"" points=""" & points.ToString().Trim() & """/>")
        If maxRows > 0 Then
            sb.AppendLine("<text x=""" & left & """ y=""" & (top + plotHeight + 22) & """ font-family=""Arial"" font-size=""12"">" & XmlEncodeText(ShortText(dt.Rows(0)(labelColumn).ToString(), 18)) & "</text>")
            sb.AppendLine("<text x=""" & (left + plotWidth - 120) & """ y=""" & (top + plotHeight + 22) & """ font-family=""Arial"" font-size=""12"">" & XmlEncodeText(ShortText(dt.Rows(maxRows - 1)(labelColumn).ToString(), 18)) & "</text>")
        End If
        sb.AppendLine("</svg>")
        Return sb.ToString()
    End Function

    Private Function BuildGroupedChartData(source As DataTable, categoryColumn As String, numericColumn As String) As DataTable
        Dim totals As New Dictionary(Of String, Double)()
        For Each row As DataRow In source.Rows
            Dim key As String = row(categoryColumn).ToString()
            If key.Trim() = "" Then key = "(blank)"
            If Not totals.ContainsKey(key) Then totals(key) = 0
            totals(key) += NumericValue(row(numericColumn))
        Next

        Dim dt As New DataTable()
        dt.Columns.Add("Category", GetType(String))
        dt.Columns.Add("Value", GetType(Double))
        dt.Columns.Add("PercentOfTotal", GetType(Double))
        Dim grandTotal As Double = 0
        For Each item In totals
            grandTotal += item.Value
        Next
        For Each item In totals
            Dim row As DataRow = dt.NewRow()
            row("Category") = item.Key
            row("Value") = item.Value
            If grandTotal <> 0 Then row("PercentOfTotal") = item.Value / grandTotal Else row("PercentOfTotal") = 0
            dt.Rows.Add(row)
        Next

        Dim dv As New DataView(dt)
        dv.Sort = "Value DESC"
        Return dv.ToTable()
    End Function

    Private Function BuildTimeChartData(source As DataTable, dateColumn As String, numericColumn As String) As DataTable
        Dim totals As New Dictionary(Of String, Double)()
        For Each row As DataRow In source.Rows
            Dim d As DateTime
            If DateTime.TryParse(row(dateColumn).ToString(), d) Then
                Dim key As String = d.ToString("yyyy-MM-dd")
                If Not totals.ContainsKey(key) Then totals(key) = 0
                totals(key) += NumericValue(row(numericColumn))
            End If
        Next

        Dim dt As New DataTable()
        dt.Columns.Add("Date", GetType(String))
        dt.Columns.Add("Value", GetType(Double))
        For Each item In totals
            Dim row As DataRow = dt.NewRow()
            row("Date") = item.Key
            row("Value") = item.Value
            dt.Rows.Add(row)
        Next

        Dim dv As New DataView(dt)
        dv.Sort = "Date ASC"
        Return dv.ToTable()
    End Function

    Private Function FirstCategoryColumn(dt As DataTable) As String
        For Each col As DataColumn In dt.Columns
            If Not IsNumericColumn(dt, col.ColumnName) AndAlso Not IsDateColumn(dt, col.ColumnName) Then Return col.ColumnName
        Next
        Return ""
    End Function

    Private Function FirstNumericColumn(dt As DataTable) As String
        For Each col As DataColumn In dt.Columns
            If IsNumericColumn(dt, col.ColumnName) Then Return col.ColumnName
        Next
        Return ""
    End Function

    Private Function FirstDateColumn(dt As DataTable) As String
        For Each col As DataColumn In dt.Columns
            If IsDateColumn(dt, col.ColumnName) Then Return col.ColumnName
        Next
        Return ""
    End Function

    Private Function IsNumericColumn(dt As DataTable, columnName As String) As Boolean
        Dim checkedRows As Integer = 0
        For Each row As DataRow In dt.Rows
            If row(columnName) IsNot Nothing AndAlso row(columnName).ToString().Trim() <> "" Then
                checkedRows += 1
                Dim value As Double
                If Not Double.TryParse(row(columnName).ToString(), value) Then Return False
                If checkedRows >= 20 Then Exit For
            End If
        Next
        Return checkedRows > 0
    End Function

    Private Function IsDateColumn(dt As DataTable, columnName As String) As Boolean
        Dim checkedRows As Integer = 0
        For Each row As DataRow In dt.Rows
            If row(columnName) IsNot Nothing AndAlso row(columnName).ToString().Trim() <> "" Then
                checkedRows += 1
                Dim value As DateTime
                If Not DateTime.TryParse(row(columnName).ToString(), value) Then Return False
                If checkedRows >= 20 Then Exit For
            End If
        Next
        Return checkedRows > 0
    End Function

    Private Function NumericValue(valueObject As Object) As Double
        Dim value As Double
        If valueObject IsNot Nothing AndAlso Double.TryParse(valueObject.ToString(), value) Then Return value
        Return 0
    End Function

    Private Function MaxColumnValue(dt As DataTable, columnName As String) As Double
        Dim maxValue As Double = 0
        For Each row As DataRow In dt.Rows
            maxValue = Math.Max(maxValue, NumericValue(row(columnName)))
        Next
        Return maxValue
    End Function

    Private Function MaxColumnSum(dt As DataTable, columnName As String) As Double
        Dim total As Double = 0
        For Each row As DataRow In dt.Rows
            total += NumericValue(row(columnName))
        Next
        Return total
    End Function

    Private Function ShortText(valueText As String, maxLength As Integer) As String
        If valueText Is Nothing Then Return ""
        If valueText.Length <= maxLength Then Return valueText
        If maxLength <= 3 Then Return valueText.Substring(0, maxLength)
        Return valueText.Substring(0, maxLength - 3) & "..."
    End Function

    Private Function XmlEncodeText(valueText As String) As String
        Return HttpUtility.HtmlEncode(FieldText(valueText))
    End Function

    Private Function HtmlEncodeText(valueText As String) As String
        Return HttpUtility.HtmlEncode(FieldText(valueText))
    End Function

    Private Function SafeFilePart(valueText As String) As String
        If valueText Is Nothing OrElse valueText.Trim() = "" Then Return "Report"
        Dim safeText As String = valueText.Trim()
        For Each invalidChar As Char In Path.GetInvalidFileNameChars()
            safeText = safeText.Replace(invalidChar, "_"c)
        Next
        Return safeText
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing Then Return ""
        Return valueObject.ToString()
    End Function
End Class
