Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Xml

Partial Class Trends
    Inherits System.Web.UI.Page

    Private Sub Trends_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Trends"
    End Sub

    Private Sub Trends_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            If Request("Equation") IsNot Nothing AndAlso Request("Equation").ToString().Trim() <> "" Then
                txtEquation.Text = Request("Equation").ToString()
            Else
                txtEquation.Text = "Y = 0 + 1 * X"
            End If

            If Request("XValue") IsNot Nothing AndAlso Request("XValue").ToString().Trim() <> "" Then
                txtXValue.Text = Request("XValue").ToString()
            Else
                txtXValue.Text = "1"
            End If

            lblSubtitle.Text = TrendSubtitle()
        End If
    End Sub

    Private Function TrendSubtitle() As String
        Dim parts As New List(Of String)()
        AddSubtitlePart(parts, "Group", Request("Group"))
        AddSubtitlePart(parts, "X Field", Request("XField"))
        AddSubtitlePart(parts, "Y Field", Request("YField"))
        Return String.Join(" | ", parts.ToArray())
    End Function

    Private Sub AddSubtitlePart(parts As List(Of String), labelText As String, valueText As String)
        If valueText Is Nothing OrElse valueText.Trim() = "" Then Exit Sub
        parts.Add(labelText & ": " & Server.HtmlEncode(valueText.Trim()))
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")

        Dim tempPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory(), "Temp")
        Directory.CreateDirectory(tempPath)
        Dim fileName As String = "TrendsAndPredictions_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".xlsx"
        Dim filePath As String = Path.Combine(tempPath, fileName)

        Try
            Dim equationText As String = HiddenTrendEquation.Value
            Dim xValue As String = HiddenTrendXValue.Value
            Dim imageBytes() As Byte = TrendImageBytes()
            CreateTrendWorkbook(filePath, equationText, xValue, imageBytes)

            Response.Clear()
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
            Response.TransmitFile(filePath)
            Response.Flush()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Finally
            Try
                If File.Exists(filePath) Then File.Delete(filePath)
            Catch ex As Exception
            End Try
        End Try
    End Sub

    Private Function TrendImageBytes() As Byte()
        Dim imageText As String = HiddenTrendImage.Value
        If imageText Is Nothing Then Return New Byte() {}
        Dim marker As String = "base64,"
        Dim markerIndex As Integer = imageText.IndexOf(marker)
        If markerIndex >= 0 Then imageText = imageText.Substring(markerIndex + marker.Length)
        If imageText.Trim() = "" Then Return New Byte() {}
        Return Convert.FromBase64String(imageText)
    End Function

    Private Sub CreateTrendWorkbook(filePath As String, equationText As String, xValue As String, imageBytes() As Byte)
        If File.Exists(filePath) Then File.Delete(filePath)
        Using archive As ZipArchive = ZipFile.Open(filePath, ZipArchiveMode.Create)
            AddZipText(archive, "[Content_Types].xml", ContentTypesXml())
            AddZipText(archive, "_rels/.rels", RootRelsXml())
            AddZipText(archive, "docProps/app.xml", AppXml())
            AddZipText(archive, "docProps/core.xml", CoreXml())
            AddZipText(archive, "xl/workbook.xml", WorkbookXml())
            AddZipText(archive, "xl/_rels/workbook.xml.rels", WorkbookRelsXml())
            AddZipText(archive, "xl/styles.xml", StylesXml())
            AddZipText(archive, "xl/worksheets/sheet1.xml", WorksheetXml(equationText, xValue, imageBytes.Length > 0))
            If imageBytes.Length > 0 Then
                AddZipText(archive, "xl/worksheets/_rels/sheet1.xml.rels", SheetRelsXml())
                AddZipText(archive, "xl/drawings/drawing1.xml", DrawingXml())
                AddZipText(archive, "xl/drawings/_rels/drawing1.xml.rels", DrawingRelsXml())
                AddZipBytes(archive, "xl/media/trendChart.png", imageBytes)
            End If
        End Using
    End Sub

    Private Sub AddZipText(archive As ZipArchive, entryName As String, text As String)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(entryName)
        Using writer As New StreamWriter(entry.Open(), New UTF8Encoding(False))
            writer.Write(text)
        End Using
    End Sub

    Private Sub AddZipBytes(archive As ZipArchive, entryName As String, bytes() As Byte)
        Dim entry As ZipArchiveEntry = archive.CreateEntry(entryName)
        Using stream As Stream = entry.Open()
            stream.Write(bytes, 0, bytes.Length)
        End Using
    End Sub

    Private Function ContentTypesXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">" &
            "<Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>" &
            "<Default Extension=""xml"" ContentType=""application/xml""/>" &
            "<Default Extension=""png"" ContentType=""image/png""/>" &
            "<Override PartName=""/xl/workbook.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml""/>" &
            "<Override PartName=""/xl/worksheets/sheet1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml""/>" &
            "<Override PartName=""/xl/styles.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml""/>" &
            "<Override PartName=""/xl/drawings/drawing1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.drawing+xml""/>" &
            "<Override PartName=""/docProps/core.xml"" ContentType=""application/vnd.openxmlformats-package.core-properties+xml""/>" &
            "<Override PartName=""/docProps/app.xml"" ContentType=""application/vnd.openxmlformats-officedocument.extended-properties+xml""/>" &
            "</Types>"
    End Function

    Private Function RootRelsXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
            "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""xl/workbook.xml""/>" &
            "<Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"" Target=""docProps/core.xml""/>" &
            "<Relationship Id=""rId3"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties"" Target=""docProps/app.xml""/>" &
            "</Relationships>"
    End Function

    Private Function WorkbookXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<workbook xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"">" &
            "<sheets><sheet name=""Trends"" sheetId=""1"" r:id=""rId1""/></sheets></workbook>"
    End Function

    Private Function WorkbookRelsXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
            "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"" Target=""worksheets/sheet1.xml""/>" &
            "<Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"" Target=""styles.xml""/>" &
            "</Relationships>"
    End Function

    Private Function WorksheetXml(equationText As String, xValue As String, includeDrawing As Boolean) As String
        Dim drawingText As String = If(includeDrawing, "<drawing r:id=""rId1""/>", "")
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"">" &
            "<cols><col min=""1"" max=""1"" width=""18""/><col min=""2"" max=""2"" width=""95""/></cols>" &
            "<sheetData>" &
            "<row r=""1""><c r=""A1"" t=""inlineStr"" s=""1""><is><t>Trends and Predictions</t></is></c></row>" &
            "<row r=""3""><c r=""A3"" t=""inlineStr""><is><t>Equation</t></is></c><c r=""B3"" t=""inlineStr""><is><t>" & XmlEscape(equationText) & "</t></is></c></row>" &
            "<row r=""4""><c r=""A4"" t=""inlineStr""><is><t>X Value</t></is></c><c r=""B4"" t=""inlineStr""><is><t>" & XmlEscape(xValue) & "</t></is></c></row>" &
            "</sheetData>" & drawingText & "</worksheet>"
    End Function

    Private Function SheetRelsXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
            "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing"" Target=""../drawings/drawing1.xml""/>" &
            "</Relationships>"
    End Function

    Private Function DrawingXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<xdr:wsDr xmlns:xdr=""http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"" xmlns:a=""http://schemas.openxmlformats.org/drawingml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"">" &
            "<xdr:twoCellAnchor editAs=""oneCell""><xdr:from><xdr:col>0</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>6</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:from>" &
            "<xdr:to><xdr:col>12</xdr:col><xdr:colOff>0</xdr:colOff><xdr:row>38</xdr:row><xdr:rowOff>0</xdr:rowOff></xdr:to>" &
            "<xdr:pic><xdr:nvPicPr><xdr:cNvPr id=""2"" name=""Trend Chart"" descr=""Trend Chart""/><xdr:cNvPicPr><a:picLocks noChangeAspect=""1""/></xdr:cNvPicPr></xdr:nvPicPr><xdr:blipFill><a:blip r:embed=""rId1""/><a:stretch><a:fillRect/></a:stretch></xdr:blipFill><xdr:spPr><a:xfrm><a:off x=""0"" y=""0""/><a:ext cx=""9525000"" cy=""5715000""/></a:xfrm><a:prstGeom prst=""rect""><a:avLst/></a:prstGeom></xdr:spPr></xdr:pic><xdr:clientData/></xdr:twoCellAnchor></xdr:wsDr>"
    End Function

    Private Function DrawingRelsXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">" &
            "<Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/image"" Target=""../media/trendChart.png""/>" &
            "</Relationships>"
    End Function

    Private Function StylesXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" &
            "<styleSheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"">" &
            "<fonts count=""2""><font><sz val=""11""/><color theme=""1""/><name val=""Calibri""/><family val=""2""/></font><font><b/><sz val=""14""/><color theme=""1""/><name val=""Calibri""/><family val=""2""/></font></fonts>" &
            "<fills count=""2""><fill><patternFill patternType=""none""/></fill><fill><patternFill patternType=""gray125""/></fill></fills>" &
            "<borders count=""1""><border><left/><right/><top/><bottom/><diagonal/></border></borders>" &
            "<cellStyleXfs count=""1""><xf numFmtId=""0"" fontId=""0"" fillId=""0"" borderId=""0""/></cellStyleXfs>" &
            "<cellXfs count=""2""><xf numFmtId=""0"" fontId=""0"" fillId=""0"" borderId=""0"" xfId=""0""/><xf numFmtId=""0"" fontId=""1"" fillId=""0"" borderId=""0"" xfId=""0"" applyFont=""1""/></cellXfs>" &
            "<cellStyles count=""1""><cellStyle name=""Normal"" xfId=""0"" builtinId=""0""/></cellStyles>" &
            "<dxfs count=""0""/><tableStyles count=""0"" defaultTableStyle=""TableStyleMedium2"" defaultPivotStyle=""PivotStyleLight16""/>" &
            "</styleSheet>"
    End Function

    Private Function AppXml() As String
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><Properties xmlns=""http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"" xmlns:vt=""http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes""><Application>DataAI</Application></Properties>"
    End Function

    Private Function CoreXml() As String
        Dim stamp As String = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
        Return "<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><cp:coreProperties xmlns:cp=""http://schemas.openxmlformats.org/package/2006/metadata/core-properties"" xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:dcterms=""http://purl.org/dc/terms/"" xmlns:dcmitype=""http://purl.org/dc/dcmitype/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><dc:title>Trends and Predictions</dc:title><dc:creator>DataAI</dc:creator><dcterms:created xsi:type=""dcterms:W3CDTF"">" & stamp & "</dcterms:created><dcterms:modified xsi:type=""dcterms:W3CDTF"">" & stamp & "</dcterms:modified></cp:coreProperties>"
    End Function

    Private Function XmlEscape(valueText As String) As String
        If valueText Is Nothing Then Return ""
        Return valueText.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;").Replace("'", "&apos;")
    End Function
End Class
