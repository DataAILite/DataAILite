Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Text
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
        If Not IsPostBack Then BuildAndBindPackage()
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        ExportPackage()
    End Sub

    Private Sub BuildAndBindPackage()
        Dim dt As New DataTable()
        dt.Columns.Add("Package Item", GetType(String))
        dt.Columns.Add("Included", GetType(String))
        dt.Columns.Add("Description", GetType(String))
        AddPackageRow(dt, "Report", CheckReport.Checked, "Current report links and report output references.")
        AddPackageRow(dt, "Report Definition", CheckDefinitions.Checked, "Report id, title, current report definition references, and RDL file.")
        AddPackageRow(dt, DropDownDataFormat.SelectedItem.Text, True, SelectedDataDescription())
        AddPackageRow(dt, "AI analysis", CheckAIAnalysis.Checked, "DataAI and ChatAI analysis entry points for the current report.")
        AddPackageRow(dt, "Charts", CheckCharts.Checked, "Report charts and analytics chart views.")
        AddPackageRow(dt, "Notes", CheckNotes.Checked, "User-entered package notes.")
        Session("ExportPackageTable") = dt
        GridViewPackage.DataSource = dt
        GridViewPackage.DataBind()
        LabelInfo.Text = "Export package manifest (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

    Private Sub AddPackageRow(dt As DataTable, itemName As String, included As Boolean, description As String)
        Dim row As DataRow = dt.NewRow()
        row("Package Item") = itemName
        If included Then row("Included") = "Yes" Else row("Included") = "No"
        row("Description") = description
        dt.Rows.Add(row)
    End Sub

    Private Function SelectedDataDescription() As String
        If DropDownDataFormat.SelectedValue = "Excel" Then
            Return "Current report data exported as Excel-compatible tab-delimited content."
        End If
        Return "Current report data exported as comma-delimited text."
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

    Private Sub ExportPackage()
        BuildAndBindPackage()
        Dim packageFolder As String = ""
        Dim zipPath As String = ""

        Try
            If applpath Is Nothing OrElse applpath.Trim() = "" Then applpath = System.AppDomain.CurrentDomain.BaseDirectory()
            Dim tempFolder As String = Path.Combine(applpath, "Temp")
            Directory.CreateDirectory(tempFolder)

            Dim packageName As String = "ExportPackage_" & SafeFilePart(FieldText(Session("REPORTID"))) & "_" & DateTime.Now.ToString("yyyyMMddHHmmss")
            packageFolder = Path.Combine(tempFolder, packageName)
            zipPath = Path.Combine(tempFolder, packageName & ".zip")
            Directory.CreateDirectory(packageFolder)

            WritePackageFiles(packageFolder)

            If File.Exists(zipPath) Then File.Delete(zipPath)
            ZipFile.CreateFromDirectory(packageFolder, zipPath)

            Response.Clear()
            Response.ContentType = "application/octet-stream"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & Path.GetFileName(zipPath))
            Response.TransmitFile(zipPath)
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
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

    Private Sub WritePackageFiles(packageFolder As String)
        Dim manifest As DataTable = CType(Session("ExportPackageTable"), DataTable)
        File.WriteAllText(Path.Combine(packageFolder, "PackageManifest.txt"), PackageHeader() & Environment.NewLine & ExportToCSVtext(manifest, Chr(9), "", ""), Encoding.UTF8)

        Dim reportData As DataTable = Nothing
        If DropDownDataFormat.SelectedValue <> "" OrElse CheckReport.Checked OrElse CheckAIAnalysis.Checked Then
            reportData = CurrentReportData()
        End If

        If DropDownDataFormat.SelectedValue = "CSV" AndAlso reportData IsNot Nothing Then
            File.WriteAllText(Path.Combine(packageFolder, "ReportData.csv"), ExportToCSVtext(reportData, ",", "", ""), Encoding.UTF8)
        End If

        If DropDownDataFormat.SelectedValue = "Excel" AndAlso reportData IsNot Nothing Then
            Dim header As String = "Data for Report: " & FieldText(Session("REPTITLE")) & Environment.NewLine & "Records returned: " & reportData.Rows.Count.ToString()
            Dim packagePath As String = packageFolder
            If Not packagePath.EndsWith("\") Then packagePath &= "\"
            DataModule.ExportToExcel(reportData, packagePath, "ReportData.xls", header, FieldText(Session("PageFtr")))
        End If

        If CheckDefinitions.Checked Then
            File.WriteAllText(Path.Combine(packageFolder, "ReportDefinitions.txt"), ReportDefinitionsText(), Encoding.UTF8)
            WriteRdlDefinitionFile(packageFolder)
        End If

        If CheckReport.Checked Then
            WriteReportPdfFile(packageFolder, reportData)
        End If

        If CheckAIAnalysis.Checked Then
            WriteAIAnalysisFile(packageFolder, reportData)
        End If

        If CheckCharts.Checked Then
            WriteChartsPackage(packageFolder, reportData)
        End If

        If CheckNotes.Checked Then
            File.WriteAllText(Path.Combine(packageFolder, "AnalysisNotes.txt"), txtNotes.Text, Encoding.UTF8)
        End If
    End Sub

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
