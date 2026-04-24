Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Imports System.IO
Imports System.Math
Imports System.Net.Mime.MediaTypeNames
Imports System.Web
Imports System.Web.UI.Page
Imports System.Windows.Forms.VisualStyles
Imports System.Xml
Imports Microsoft.VisualBasic
Imports Mysqlx.XDevAPI.Common
Public Module GenerateReports
    Public filename As String
    Public cmdReport As SqlCommand
    Public ds As System.Data.DataSet
    Public dt As DataTable
    Public da As SqlDataAdapter
    'Public doc As CrystalDecisions.CrystalReports.Engine.ReportDocument
    Public repid, comkey, crsid As String
    'Public que4() As Integer = {26, 28, 27, 533}
    Public strval As String
    Public rptfile, rptpath, pdfexportfile As String
    Public ParamValue(9) As String
    Public Parnames(), Parvalues(), Partypes() As String
    'Public Function GettingReportParameters(ByVal repid As String)
    '    Dim dt As New DataTable
    '    Dim err As String = ""
    '    Dim sqltext As String = "SELECT * FROM OURReportInfo WHERE ReportId='" & repid & "'"
    '    dt = mRecords(sqltext, err).Table
    '    'Dim cmdRepinfo As New SqlCommand
    '    'Dim RepConnStr As String = ConfigurationManager.ConnectionStrings.Item("MySqlConnection").ToString
    '    'Dim RepConn As New SqlConnection(RepConnStr)
    '    'cmdRepinfo.Connection = RepConn
    '    'cmdRepinfo.CommandText = sqltext
    '    'cmdRepinfo.CommandType = CommandType.Text
    '    'If RepConn.State = ConnectionState.Closed Then
    '    '    RepConn.Open()
    '    'End If
    '    'Dim Sqlda As New System.Data.SqlClient.SqlDataAdapter(cmdRepinfo)
    '    'Sqlda.Fill(dt)
    '    'Sqlda.Dispose()
    '    'RepConn.Close()
    '    If dt.Rows.Count = 1 Then
    '        If Not IsDBNull(dt.Rows(0).Item(5)) Then
    '            CreateReport_sp(dt)
    '        End If
    '    End If  'dt.Rows.Count = 1
    'End Function

    'Public Function CreateReport_sp(ByVal dt As DataTable) As String
    '    If dt.Rows(0).Item(2).ToString = "crystal" Then
    '        'CrystalReport_sp(dt)
    '    End If
    '    If dt.Rows(0).Item(2).ToString = "aspx" Then
    '        AspxReport_sp(dt)
    '    End If
    '    If dt.Rows(0).Item(2).ToString = "compcrys" Then
    '        Return dt.Rows(0).Item(8).ToString
    '    End If
    'End Function

    'Public Function CrystalReport_sp(ByVal dt As DataTable)
    '    cmdReport = New SqlCommand
    '    Myconn = New SqlConnection(MyconnStr)
    '    cmdReport.Connection = Myconn
    '    If Myconn.State = ConnectionState.Closed Then
    '        Myconn.Open()
    '    End If
    '    cmdReport.CommandType = Data.CommandType.StoredProcedure
    '    cmdReport.CommandText = dt.Rows(0).Item(5).ToString
    '    Dim ParamName As String
    '    k = CInt(dt.Rows(0).Item(6))
    '    For i = 1 To k
    '        n = CInt(Mid(dt.Rows(0).Item(7).ToString, i, 1))
    '        ParamName = "@" & dt.Rows(0).Item(9 + 2 * n).ToString
    '        If Trim(dt.Rows(0).Item(10 + 2 * n).ToString) = "nvarchar" Then
    '            cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParamName, System.Data.SqlDbType.NVarChar))
    '            cmdReport.Parameters.Item(i - 1).Value = ParamValue(n)
    '        End If
    '        If Trim(dt.Rows(0).Item(10 + 2 * n).ToString) = "datetime" Then
    '            cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParamName, System.Data.SqlDbType.DateTime))
    '            cmdReport.Parameters.Item(i - 1).Value = CInt(ParamValue(n))
    '        End If
    '        If Trim(dt.Rows(0).Item(10 + 2 * n).ToString) = "int" Then
    '            cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParamName, System.Data.SqlDbType.Int))
    '            cmdReport.Parameters.Item(i - 1).Value = CInt(ParamValue(n))
    '        End If
    '    Next
    '    cmdReport.ExecuteNonQuery()

    '    Dim ds As New System.Data.DataSet
    '    da = New SqlDataAdapter(cmdReport)
    '    da.Fill(ds)
    '    If ds.Tables(0).Rows.Count > 0 Then
    '        rptfile = "RPTFILES/" & dt.Rows(0).Item(8).ToString
    '        'rptpath = Server.MapPath(rptfile)
    '        doc = New CrystalDecisions.CrystalReports.Engine.ReportDocument
    '        doc.Load(rptpath)
    '        doc.SetDatabaseLogon("webappluser", "iaes2001")
    '        doc.SetDataSource(ds)
    '        strval = ""
    '        For i = 1 To k
    '            n = CInt(Mid(dt.Rows(0).Item(7).ToString, i, 1))
    '            ParamName = "@" & dt.Rows(0).Item(9 + 2 * n).ToString
    '            If Trim(dt.Rows(0).Item(10 + 2 * n).ToString) = "nvarchar" Then
    '                doc.SetParameterValue(ParamName, ParamValue(n))
    '            End If
    '            If Trim(dt.Rows(0).Item(10 + 2 * n).ToString) = "int" Then
    '                doc.SetParameterValue(ParamName, CInt(ParamValue(n)))
    '            End If
    '            strval = strval & "-" & ParamValue(n)
    '        Next
    '        pdfexportfile = "./TEMP/" & LCase(dt.Rows(0).Item(0).ToString) & strval & ".pdf"
    '        doc.ExportOptions.ExportFormatType = CrystalDecisions.[Shared].ExportFormatType.PortableDocFormat
    '        doc.ExportOptions.ExportDestinationType = CrystalDecisions.[Shared].ExportDestinationType.DiskFile
    '        doc.ExportOptions.DestinationOptions = New CrystalDecisions.[Shared].DiskFileDestinationOptions
    '        'doc.ExportOptions.DestinationOptions.DiskFileName = Server.MapPath(pdfexportfile)
    '        doc.Export()
    '        'Response.Redirect(pdfexportfile)
    '    Else
    '        'Response.Redirect("Nodata.aspx")
    '    End If
    '    da.Dispose()
    '    Myconn.Close()
    'End Function

    Public Function AspxReport_sp(ByVal dt As DataTable) As String
        Dim k, n As Integer
        k = CInt(dt.Rows(0).Item(6))
        ReDim Parnames(k - 1), Parvalues(k - 1), Partypes(k - 1)
        For i = 1 To k
            n = CInt(Mid(dt.Rows(0).Item(7).ToString, i, 1))
            Parnames(i - 1) = "@" & dt.Rows(0).Item(9 + 2 * n).ToString
            Partypes(i - 1) = dt.Rows(0).Item(10 + 2 * n).ToString
            Parvalues(i - 1) = ParamValue(n)
        Next
        'Session("sp") = dt.Rows(0).Item(5).ToString
        'Return dt.Rows(0).Item(5).ToString
        'Session("ParamQuantity") = k
        'Session("sesParamNames") = Parnames
        'Session("sesParamtypes") = Partypes
        'Session("sesParamValues") = Parvalues
        'Response.Redirect(dt.Rows(0).Item(8).ToString)

    End Function
    'Public Function ShowCrystalReportNow(ByVal repid As String, ByVal rptpath As String, ByVal dv As DataView, ByVal k As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal pdfpath As String) As String
    '    Dim r As String
    '    r = ""
    '    If pdfpath = "" Then
    '        pdfpath = "c:\Inetpub\wwwroot\OMS\InteractiveReporting\Temp\"
    '    End If

    '    Dim dt As DataTable
    '    Try
    '        dt = DataSetFromDataView(dv).Tables(0)
    '        doc = New CrystalDecisions.CrystalReports.Engine.ReportDocument
    '        doc.Load(rptpath)

    '        'doc.SetDatabaseLogon("webappluser", "iaes2001")
    '        If doc.HasSavedData = True Then doc.Dispose()
    '        doc.DataSourceConnections.Clear()
    '        Dim xsdpath As String = Replace(rptpath, "RPTFILES", "XSDFILES")
    '        xsdpath = Replace(xsdpath, ".rpt", ".xsd")
    '        'doc.ReportClientDocument.DatabaseController.AddDataSource(xsdpath)
    '        'doc.ReportClientDocument.DatabaseController.AddTable(dt)
    '        doc.SetDataSource(dt)
    '        Dim n As Integer = doc.DataSourceConnections.Count
    '        'If doc.HasSavedData Then doc.ReportClientDocument.Close()
    '        strval = ""
    '        For i = 0 To k - 1
    '            If ParamType(i) = "nvarchar" Then
    '                doc.SetParameterValue("@" & ParamName(i), ParamValue(i))
    '            End If
    '            If ParamType(i) = "datetime" Then
    '                doc.SetParameterValue("@" & ParamName(i), ParamValue(i))
    '            End If
    '            If ParamType(i) = "int" Then
    '                doc.SetParameterValue("@" & ParamName(i), CInt(ParamValue(i)))
    '            End If

    '            strval = strval & "-" & ParamValue(i)
    '        Next
    '        'doc.SetDataSource(ds)
    '        pdfexportfile = pdfpath & LCase(repid) & strval & ".pdf"
    '        r = pdfexportfile
    '        doc.ExportOptions.ExportFormatType = CrystalDecisions.[Shared].ExportFormatType.PortableDocFormat
    '        doc.ExportOptions.ExportDestinationType = CrystalDecisions.[Shared].ExportDestinationType.DiskFile
    '        doc.ExportOptions.DestinationOptions = New CrystalDecisions.[Shared].DiskFileDestinationOptions
    '        doc.ExportOptions.DestinationOptions.DiskFileName = pdfexportfile 'pdfpath & "\Temp\" & LCase(repid) & strval & ".pdf"     'Server.MapPath(pdfexportfile)
    '        doc.Export()

    '    Catch exc As Exception
    '        r = exc.Message
    '    End Try
    '    Return pdfexportfile
    'End Function
    'Public Function ShowCrystalReportLinkedToTable(ByVal repid As String, ByVal rptpath As String, ByVal k As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal pdfpath As String) As String
    '    Dim r As String
    '    r = ""
    '    If pdfpath = "" Then pdfpath = "c:\Inetpub\wwwroot\TCEreporting\Temp\"
    '    'Dim ds As DataSet
    '    Try
    '        'ds = DataSetFromDataView(dv)
    '        doc = New CrystalDecisions.CrystalReports.Engine.ReportDocument
    '        doc.Load(rptpath)
    '        doc.SetDatabaseLogon("webappluser", "iaes2001")
    '        'If doc.HasSavedData = True Then doc.Dispose()
    '        'doc.SetDataSource(ds)

    '        'If doc.HasSavedData Then doc.ReportClientDocument.Close()
    '        strval = ""
    '        For i = 0 To k - 1
    '            If ParamType(i) = "nvarchar" Then
    '                doc.SetParameterValue("@" & ParamName(i), ParamValue(i))
    '            End If
    '            If ParamType(i) = "datetime" Then
    '                doc.SetParameterValue("@" & ParamName(i), ParamValue(i))
    '            End If
    '            If ParamType(i) = "int" Then
    '                doc.SetParameterValue("@" & ParamName(i), CInt(ParamValue(i)))
    '            End If

    '            strval = strval & "-" & ParamValue(i)
    '        Next
    '        'doc.SetDataSource(ds)
    '        pdfexportfile = "./Temp/" & LCase(repid) & strval & ".pdf"
    '        r = pdfexportfile
    '        doc.ExportOptions.ExportFormatType = CrystalDecisions.[Shared].ExportFormatType.PortableDocFormat
    '        doc.ExportOptions.ExportDestinationType = CrystalDecisions.[Shared].ExportDestinationType.DiskFile
    '        doc.ExportOptions.DestinationOptions = New CrystalDecisions.[Shared].DiskFileDestinationOptions
    '        doc.ExportOptions.DestinationOptions.DiskFileName = pdfpath & "\Temp\" & LCase(repid) & strval & ".pdf"     'Server.MapPath(pdfexportfile)
    '        doc.Export()

    '    Catch exc As Exception
    '        r = exc.Message
    '    End Try
    '    Return r
    'End Function
    'Public Function ShowCrystalReportData(ByVal repid As String, ByVal rptpath As String, ByVal SP As String, ByVal k As Integer, ByVal ParamName As Array, ByVal ParamType As Array, ByVal ParamValue As Array, ByVal pdfpath As String) As String

    '    Dim r As String
    '    r = ""
    '    If pdfpath = "" Then pdfpath = "c:\Inetpub\wwwroot\TCEreporting\Temp\"

    '    Try
    '        Dim ds As New System.Data.DataSet
    '        ds.Clear()
    '        'ds = DataSetFromDataView(dv)
    '        cmdReport = New SqlCommand
    '        Myconn = New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString)
    '        cmdReport.Connection = Myconn
    '        If Myconn.State = ConnectionState.Closed Then
    '            Myconn.Open()
    '        End If
    '        cmdReport.CommandType = Data.CommandType.StoredProcedure
    '        cmdReport.CommandText = SP
    '        Dim ParameterName As String
    '        'k = CInt(dt.Rows(0).Item(6))
    '        For i = 0 To k - 1
    '            ParameterName = "@" & ParamName(i)
    '            If ParamType(i) = "nvarchar" Then
    '                cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParameterName, System.Data.SqlDbType.NVarChar))
    '                cmdReport.Parameters.Item(i).Value = ParamValue(i)
    '            End If
    '            If ParamType(i) = "datetime" Then
    '                cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParameterName, System.Data.SqlDbType.DateTime))
    '                cmdReport.Parameters.Item(i).Value = ParamValue(i)
    '            End If
    '            If ParamType(i) = "int" Then
    '                cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParameterName, System.Data.SqlDbType.Int))
    '                cmdReport.Parameters.Item(i).Value = CInt(ParamValue(i))
    '            End If
    '        Next
    '        cmdReport.ExecuteNonQuery()

    '        'Dim ds As New DataSet
    '        da = New SqlDataAdapter(cmdReport)
    '        da.Fill(ds)
    '        doc = New CrystalDecisions.CrystalReports.Engine.ReportDocument
    '        doc.Load(rptpath)
    '        doc.SetDatabaseLogon("webappluser", "iaes2001")
    '        If doc.HasSavedData = True Then doc.Dispose()
    '        doc.SetDataSource(ds)

    '        doc.RecordSelectionFormula = "{xp_test.Term Code Six} like [""" + ParamValue(0) + """]"

    '        strval = ""
    '        For i = 0 To k - 1
    '            If ParamType(i) = "nvarchar" Then
    '                doc.SetParameterValue("@" & ParamName(i), ParamValue(i))
    '            End If
    '            If ParamType(i) = "datetime" Then
    '                doc.SetParameterValue("@" & ParamName(i), ParamValue(i))
    '            End If
    '            If ParamType(i) = "int" Then
    '                doc.SetParameterValue("@" & ParamName(i), CInt(ParamValue(i)))
    '            End If
    '            strval = strval & "-" & ParamValue(i)
    '        Next

    '        'doc.SetDataSource(ds)
    '        pdfexportfile = "./Temp/" & LCase(repid) & strval & ".pdf"
    '        r = pdfexportfile
    '        doc.ExportOptions.ExportFormatType = CrystalDecisions.[Shared].ExportFormatType.PortableDocFormat
    '        doc.ExportOptions.ExportDestinationType = CrystalDecisions.[Shared].ExportDestinationType.DiskFile
    '        doc.ExportOptions.DestinationOptions = New CrystalDecisions.[Shared].DiskFileDestinationOptions
    '        doc.ExportOptions.DestinationOptions.DiskFileName = pdfpath & "\Temp\" & LCase(repid) & strval & ".pdf"     'Server.MapPath(pdfexportfile)
    '        doc.Export()
    '        ds.Clear()
    '        ds.Dispose()
    '    Catch exc As Exception
    '        r = exc.Message
    '    End Try
    '    Return r
    'End Function
    'Public Function CreateXSDsForCrystalReport(ByVal ReportName As String, ByVal cr As CrystalDecisions.CrystalReports.Engine.ReportDocument, ByVal xsdpath As String) As String
    '    Dim dtname As String
    '    Dim i, s As Integer
    '    Try
    '        For i = 0 To cr.Database.Tables.Count - 1
    '            dtname = cr.Database.Tables(i).Name
    '            CreateXSDForDatasourceTable(xsdpath, dtname, i, cr)
    '        Next
    '        If Not cr.IsSubreport Then
    '            'add subreport datasources
    '            For s = 0 To cr.Subreports.Count - 1
    '                For i = 0 To cr.Subreports(s).Database.Tables.Count - 1
    '                    dtname = cr.Subreports(s).Database.Tables(i).Name
    '                    CreateXSDForDatasourceTable(xsdpath, dtname, i, cr.Subreports(s))
    '                Next
    '            Next
    '        End If
    '        Return "XSD(s) created in the directory " & xsdpath
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        Return ex.Message
    '    End Try
    'End Function
    'Public Function CreateXSDForDatasourceTable(ByVal rptpath As String, ByVal txtDataSource As String, ByVal indDataSourse As Integer, ByVal cr As CrystalDecisions.CrystalReports.Engine.ReportDocument) As String
    '    If txtDataSource = String.Empty Or rptpath = String.Empty Then
    '        Return String.Empty
    '        Exit Function
    '    End If
    '    Dim ErrorLogText As String = String.Empty
    '    Dim j, tblFieldValueSize As Integer
    '    Dim tbl As CrystalDecisions.CrystalReports.Engine.Table
    '    Dim tblFieldValueType As CrystalDecisions.Shared.FieldValueType
    '    Dim table As New System.Data.DataTable
    '    table.TableName = txtDataSource
    '    Try
    '        'write xsd from report datasource
    '        tbl = cr.Database.Tables(indDataSourse)

    '        For j = 0 To tbl.Fields.Count - 1
    '            table.Columns.Add(tbl.Fields(j).Name)
    '            ' ConvertValueTypeToSystemType(tbl.Fields(j).ValueType))
    '            tblFieldValueType = tbl.Fields(j).ValueType
    '            tblFieldValueSize = tbl.Fields(j).NumberOfBytes
    '            If tblFieldValueType = FieldValueType.StringField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            ElseIf tblFieldValueType = FieldValueType.TimeField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.DataTime")
    '            ElseIf tblFieldValueType = FieldValueType.TransientMemoField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            ElseIf tblFieldValueType = FieldValueType.PersistentMemoField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            ElseIf tblFieldValueType = FieldValueType.BitmapField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Drawing.Bitmap")
    '            ElseIf tblFieldValueType = FieldValueType.BlobField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            ElseIf tblFieldValueType = FieldValueType.BooleanField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Boolean")
    '            ElseIf tblFieldValueType = FieldValueType.ChartField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Drawing.Graphics")
    '            ElseIf tblFieldValueType = FieldValueType.CurrencyField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Integer")
    '            ElseIf tblFieldValueType = FieldValueType.DateField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.DateTime")
    '            ElseIf tblFieldValueType = FieldValueType.DateTimeField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.DateTime")
    '            ElseIf tblFieldValueType = FieldValueType.IconField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Icon")
    '            ElseIf tblFieldValueType = FieldValueType.Int16sField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Int16")
    '            ElseIf tblFieldValueType = FieldValueType.Int16uField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.UInt16")
    '            ElseIf tblFieldValueType = FieldValueType.Int32sField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Int32")
    '            ElseIf tblFieldValueType = FieldValueType.Int32uField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.UInt32")
    '            ElseIf tblFieldValueType = FieldValueType.Int8sField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.SByte")
    '            ElseIf tblFieldValueType = FieldValueType.Int8uField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.UShort")
    '            ElseIf tblFieldValueType = FieldValueType.NumberField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Int32")
    '            ElseIf tblFieldValueType = FieldValueType.OleField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.OleDb.OleDbType")
    '            ElseIf tblFieldValueType = FieldValueType.PictureField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.Drawing.Graphics")
    '            ElseIf tblFieldValueType = FieldValueType.SameAsInputField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            ElseIf tblFieldValueType = FieldValueType.UnknownField Then
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            Else
    '                table.Columns(tbl.Fields(j).Name).DataType = Type.GetType("System.String")
    '                table.Columns(j).MaxLength = CInt((tblFieldValueSize - 2) / 2)
    '            End If
    '        Next
    '        'flash to xsd
    '        table.WriteXmlSchema(rptpath & txtDataSource & ".xsd")

    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        ErrorLogText = ErrorLogText & Chr(13) & Chr(10) & ex.Message
    '        Return String.Empty
    '    End Try
    '    Return rptpath & txtDataSource & ".xsd"
    'End Function
    Public Function GetEmbeddedImageForRdl(imagePath As String) As String
        ' Load the image from a file.
        Dim img As System.Drawing.Image = System.Drawing.Image.FromFile(imagePath)

        ' Save the image data to a memory stream.
        Using ms As New MemoryStream()
            img.Save(ms, img.RawFormat)
            ' Convert the byte array to a Base64 string.
            Dim imageBytes As Byte() = ms.ToArray()
            Return Convert.ToBase64String(imageBytes)
        End Using
    End Function

    Public Function GetImageMimeTypeForRdl(filePath As String) As String
        If File.Exists(filePath) Then
            Dim fileName As String = Path.GetFileName(filePath)
            Dim mimeType As String = MimeMapping.GetMimeMapping(fileName)
            Return mimeType
        Else
            Return "File not found."
        End If
    End Function

    Public Function ConvertIconToGif(iconFilePath As String, outputGifPath As String) As Boolean
        Try
            If File.Exists(iconFilePath) Then
                Dim memStream = New MemoryStream()
                Dim strmfileStream As FileStream = File.OpenRead(iconFilePath)
                strmfileStream.CopyTo(memStream)
                memStream.Position = 0

                Using icon As New Icon(memStream)
                    ' Convert the icon to a Bitmap
                    Dim bitmap As Bitmap = icon.ToBitmap()

                    ' Save the Bitmap as a GIF
                    bitmap.Save(outputGifPath, System.Drawing.Imaging.ImageFormat.Gif)
                    Return True
                End Using

            End If
            ' Load the icon file

        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function CreateXSDForDataTable(dt As DataTable, repid As String, xsdpath As String) As String
        If xsdpath = String.Empty Or repid = String.Empty Or dt Is Nothing Then
            Return "ERROR!! Nothing to create..."
            Exit Function
        End If
        Dim ErrorLogText As String = String.Empty
        dt = CorrectDatasetColumns(dt)
        dt.TableName = repid
        Try
            'flash to xsd file
            dt.WriteXmlSchema(xsdpath & repid & ".xsd")

            'flash to string
            Dim xsdstr As String = String.Empty
            Dim xsdstrm As New System.IO.StringWriter()
            dt.WriteXmlSchema(xsdstrm)
            xsdstr = xsdstrm.ToString
            If xsdstr <> "" Then
                Dim msql As String = String.Empty
                If HasRecords("SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & repid & "' AND Type='XSD'") Then
                    msql = "UPDATE OURFiles SET FileText='" & xsdstr & "' WHERE ReportId='" & repid & "' AND Type='XSD'"
                Else
                    msql = "INSERT INTO OURFiles (ReportId,Type,FileText) VALUES ( '" & repid & "','XSD','" & xsdstr & "')"
                End If
                ErrorLogText = ExequteSQLquery(msql)
            End If

        Catch ex As Exception
            ErrorLogText = ErrorLogText & Chr(13) & Chr(10) & ex.Message
            Return "ERROR!!" & ErrorLogText
        End Try
        Return xsdpath & repid & ".xsd"
    End Function
    'Public Function CreateCrystalReportForXSD(ByVal repid As String, ByVal rptpath As String) As String
    '    Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
    '    Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
    '    Dim rep As String = rptpath & repid & ".rpt"
    '    Dim dt As DataTable = ReturnDataTblFromXML(xsdfile)
    '    Dim crTable As CrystalDecisions.ReportAppServer.DataDefModel.Table
    '    Dim cr As New CrystalDecisions.CrystalReports.Engine.ReportDocument
    '    cr.Load(rptpath & "EmptyTemplate.rpt")
    '    Dim n As Integer
    '    n = cr.DataSourceConnections.Count
    '    cr.SaveAs(rep, False)
    '    'cr.Close()
    '    'cr = Nothing
    '    'cr.ReportClientDocument.ReportAppServer.
    '    'Dim cld As New CrystalDecisions.ReportAppServer.ClientDoc.ReportClientDocument
    '    'cld.ReportAppServer = System.Environment.MachineName
    '    'cld.Open(rep)
    '    cr.DataSourceConnections.Clear()
    '    n = cr.DataSourceConnections.Count
    '    'cr.SaveAs(rep, False)
    '    'cr.ReportClientDocument.DatabaseController.AddDataSource(xsdfile)
    '    'cr.SaveAs(rep, False)
    '    'cr.SetDataSource(dt)
    '    'n = cr.DataSourceConnections.Count
    '    'cr.SaveAs(rep, False)


    '    Try

    '        'cld.DataSourceConnections(0).SetConnection(xsdfile)
    '        'Dim path As Object = rptpath & repid & ".rpt"
    '        'cld.Open(Path, 0)
    '        'cld.Save()
    '        Dim myconstring As String
    '        myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString

    '        'Dim ci As New CrystalDecisions.Shared.ConnectionInfo
    '        'ci = GetConnectionInfoFromConnectionString(myconstring & ";")
    '        Dim ci As New CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo
    '        'ci = GetRASConnectionInfoFromConnectionString(myconstring & ";")

    '        'cr.ReportClientDocument.DataSourceConnections(0).SetConnection(xsdfile)
    '        Dim logon As NameValuePairs2 = New NameValuePairs2
    '        logon.Set("QE_ServerDescription", xsdfile)
    '        logon.Set("Local Schema File", xsdfile)
    '        logon.Set("Local XML File", xsdfile)
    '        'cr.ReportClientDocument.DatabaseController.AddDataSource(xsdfile)
    '        'cr.ReportClientDocument.DataSourceConnections(0).SetLogonProperties(logon)

    '        crTable = New CrystalDecisions.ReportAppServer.DataDefModel.Table
    '        'crTable = CrystalDecisions.ReportAppServer.DataSetConversion.DataSetConverter.Convert(dt)
    '        crTable.ConnectionInfo = ci
    '        crTable.Name = repid
    '        crTable.Alias = repid

    '        ' add fields to the table
    '        For i = 0 To dt.Columns.Count - 1
    '            Dim Field As CrystalDecisions.ReportAppServer.DataDefModel.Field = Nothing
    '            Field.Name = dt.Columns(i).Caption
    '            'dt.Columns(i).DataType
    '            If dt.Columns(i).DataType.ToString = "System.String" Then
    '                Field.Type = CrFieldValueTypeEnum.crFieldValueTypeStringField
    '            ElseIf dt.Columns(i).DataType.ToString = "System.DateTime" Then
    '                Field.Type = CrFieldValueTypeEnum.crFieldValueTypeDateTimeField
    '            Else
    '                Field.Type = CrFieldValueTypeEnum.crFieldValueTypeNumberField
    '            End If
    '            crTable.DataFields.Add(Field)
    '        Next

    '        cr.ReportClientDocument.Database.Tables.Add(crTable)
    '        'cr.ReportClientDocument.DatabaseController.DataSetConverter.AddDataSource(cr.ReportClientDocument, dt)
    '        cr.ReportClientDocument.DatabaseController.AddDataSource(crTable)
    '        'cr.ReportClientDocument.DatabaseController.SetTableLocation(dt, dt)
    '        '' add a table
    '        'cr.ReportClientDocument.DatabaseController.AddTable(crTable, False)

    '        ' pass dataset
    '        cr.ReportClientDocument.DatabaseController.SetDataSource(dt, repid, repid)





    '        cr.SaveAs(rep, False)
    '        Return rptpath & repid & ".rpt"
    '    Catch ex As Exception
    '        Return "ERROR!! " & rptpath & repid & ".rpt" & " - " & ex.Message
    '    End Try
    'End Function
    'Public Function GetConnectionInfoFromConnectionString(ByVal dataCacheConnectionString As String) As CrystalDecisions.Shared.ConnectionInfo
    '    Dim ci As New CrystalDecisions.Shared.ConnectionInfo
    '    Try
    '        ci.Type = CrystalDecisions.Shared.ConnectionInfoType.Unknown
    '        ci.DatabaseName = GetDatabaseFromConnectionString(dataCacheConnectionString)
    '        ci.ServerName = GetServerNameFromConnectionString(dataCacheConnectionString)
    '        ci.Password = GetPasswordFromConnectionString(dataCacheConnectionString)
    '        ci.UserID = GetUserIDFromConnectionString(dataCacheConnectionString)
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        Return Nothing
    '    End Try
    '    Return ci
    'End Function
    'Public Function GetRASConnectionInfoFromConnectionString(ByVal dataCacheConnectionString As String) As CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo
    '    Dim ci As New CrystalDecisions.ReportAppServer.DataDefModel.ConnectionInfo
    '    Try
    '        'ci.Type = CrystalDecisions.Shared.ConnectionInfoType.Unknown
    '        ci.DatabaseName = GetDatabaseFromConnectionString(dataCacheConnectionString)
    '        ci.ServerName = GetServerNameFromConnectionString(dataCacheConnectionString)
    '        ci.Password = GetPasswordFromConnectionString(dataCacheConnectionString)
    '        ci.UserID = GetUserIDFromConnectionString(dataCacheConnectionString)
    '    Catch ex As Exception
    '        'MsgBox(ex.Message)
    '        Return Nothing
    '    End Try
    '    Return ci
    'End Function
    Public Function GetServerNameFromConnectionString(ByVal cs As String) As String
        'extract namespace from the connection string cs
        cs = UCase(cs)
        Try
            If cs = "" OrElse cs.IndexOf("SERVER=") < 0 Then
                Return ""
                Exit Function
            End If
            Dim nmspace As String = String.Empty
            'extract server name from the connection string
            nmspace = cs.Substring(cs.IndexOf("SERVER="))
            If nmspace = "" OrElse nmspace.IndexOf(";") < 0 Then
                Return ""
                Exit Function
            End If
            nmspace = nmspace.Substring(6, nmspace.IndexOf(";") - 9)
            Return nmspace
        Catch ex As Exception
            MsgBox(ex.Message)
            Return String.Empty
        End Try
    End Function
    Public Function GetDatabaseFromConnectionString(ByVal cs As String) As String
        'NOT IN USE
        'extract namespace from the connection string cs
        cs = UCase(cs).Replace(" ", "")
        Try
            If cs = "" OrElse cs.IndexOf("DATABASE=") < 0 Then
                Return ""
                Exit Function
            End If
            cs = cs & ";"
            Dim nmspace As String = String.Empty
            'extract namespace from the connection string
            nmspace = cs.Substring(cs.IndexOf("DATABASE="))
            If nmspace = "" OrElse nmspace.IndexOf(";") < 0 Then
                Return ""
                Exit Function
            End If
            nmspace = nmspace.Substring(9, nmspace.IndexOf(";") - 9)
            Return nmspace
        Catch ex As Exception
            MsgBox(ex.Message)
            Return String.Empty
        End Try
    End Function
    Public Function GetPasswordFromConnectionString(ByVal cs As String) As String
        'extract password from the connection string cs
        Try
            cs = cs.Replace(" ", "")
            If cs = "" OrElse cs.ToUpper.IndexOf("Password=".ToUpper) < 0 Then
                Return ""
                Exit Function
            End If
            cs = cs & ";"
            Dim nmspace As String = String.Empty
            'extract PASSWORD from the connection string
            nmspace = cs.Substring(cs.ToUpper.IndexOf("Password=".ToUpper))
            If nmspace = "" Then
                Return ""
                Exit Function
            End If
            If nmspace.IndexOf(";") < 0 Then
                nmspace = nmspace.Substring(8).Replace("=", "").Replace(";", "")
            Else
                nmspace = nmspace.Substring(8, nmspace.IndexOf(";") - 8).Replace("=", "").Replace(";", "")
            End If

            Return nmspace
        Catch ex As Exception
            MsgBox(ex.Message)
            Return String.Empty
        End Try
    End Function
    Public Function GetUserIDFromConnectionString(ByVal cs As String) As String
        'extract user id from the connection string cs
        cs = UCase(cs).Replace(" ", "") & ";"
        Try
            If cs = "" OrElse cs.IndexOf("USERID=") < 0 Then
                Return ""
                Exit Function
            End If
            Dim nmspace As String = String.Empty
            'extract user from the connection string
            nmspace = cs.Substring(cs.IndexOf("USERID="))
            If nmspace = "" Then
                Return ""
                Exit Function
            End If
            nmspace = nmspace.Substring(7, nmspace.IndexOf(";") - 7).Replace("=", "")

            Return nmspace
        Catch ex As Exception
            MsgBox(ex.Message)
            Return String.Empty
        End Try

    End Function
    Public Function GetNamespaceFromConnectionString(ByVal cs As String) As String
        'NOT IN USE
        'extract namespace from the connection string cs
        cs = UCase(cs)
        Try
            If cs = "" OrElse cs.IndexOf("NAMESPACE =") < 0 Then
                Return ""
                Exit Function
            End If
            Dim nmspace As String = String.Empty
            'extract namespace from the connection string
            nmspace = cs.Substring(cs.IndexOf("NAMESPACE ="))
            If nmspace = "" OrElse nmspace.IndexOf(";") < 0 Then
                Return ""
                Exit Function
            End If
            nmspace = nmspace.Substring(12, nmspace.IndexOf(";") - 12)
            Return nmspace
        Catch ex As Exception
            MsgBox(ex.Message)
            Return String.Empty
        End Try
    End Function
    'Public Function MakeConnectionInfo(ByVal cs As String, ByVal newodbc As String) As CrystalDecisions.Shared.ConnectionInfo
    '    Dim ci As New CrystalDecisions.Shared.ConnectionInfo
    '    Try
    '        cs = cs & ";"
    '        ci.ServerName = newodbc
    '        ci.DatabaseName = GetDatabaseFromConnectionString(cs)
    '        ci.Password = GetPasswordFromConnectionString(cs)
    '        ci.UserID = GetUserIDFromConnectionString(cs)

    '    Catch ex As Exception
    '        ' MsgBox(ex.Message)
    '        Return Nothing
    '    End Try
    '    Return ci
    'End Function

    Public Function ReportCreatedByDesigner(ByVal repid As String) As Boolean
        Dim sSql As String = "SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL' AND Prop2='designer'"
        Return HasRecords(sSql)
        'Dim sSql As String = "SELECT * FROM OURREPORTINFO WHERE ReportID='" & repid & "' AND Param2type ='designer'"
        'If HasRecords(sSql) Then
        '    Return True
        'Else
        '    Return False
        'End If
    End Function

    Public Function CreateTabularReportForXSDByDesigner(ByVal repid As String, ByVal repttl As String, ByVal rdlpath As String, ByVal mSQL As String, ByVal datatype As String, ByVal orien As String, Optional ByVal PageFtr As String = "", Optional ByVal myconstr As String = "") As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        Dim rep As String = String.Empty
        Dim n, i, j, k, m, l As Integer
        Dim showall As Boolean = True     'show all fields 
        Dim fldname As String = String.Empty
        Dim fldcaption As String = String.Empty
        Dim fldexpr As String = String.Empty
        Dim flditemtype As String = String.Empty
        Dim tablixRows As XmlElement '= AddElement(tablixBody, "TablixRows", Nothing)
        Dim tablixCell As XmlElement
        Dim cellContents As XmlElement
        Dim textbox As XmlElement
        Dim paragraphs As XmlElement
        Dim paragraph As XmlElement
        Dim textRuns As XmlElement
        Dim textRun As XmlElement
        Dim style As XmlElement
        Dim border As XmlElement
        Dim image As XmlElement
        Dim embeddedImages As XmlElement
        Dim embeddedImage As XmlElement
        Dim hasImages As Boolean = False
        Dim name As String = String.Empty
        Dim colspan As XmlElement
        Dim fldval As String = String.Empty
        Dim fldfriendname As String = String.Empty
        Dim rectangl As XmlElement
        Dim Xtextbox As Decimal
        Dim Ytextbox As Decimal
        Dim Htextbox As Decimal
        Dim Wtextbox As Decimal
        Dim Ximage As Decimal
        Dim Yimage As Decimal
        Dim Himage As Decimal
        Dim Wimage As Decimal
        Dim imagePath As String = String.Empty
        Dim imageFileValue As String = String.Empty
        Dim fontnam As String = "Arial"
        Dim fontsiz As String = "12"
        Dim fontstyl As String = "Default"
        Dim fontweight As String = "Default"
        Dim underlin As String = "Default"
        Dim strikeout As String = "Default"
        Dim textdecor As String = "Default"
        Dim forecolr As String = "black"
        Dim backcolr As String = "white"
        Dim bordercolr As String = "black"
        Dim borderstyle As String = "None"
        Dim borderwidth As String = "1"
        Dim bordWidth As String = ""
        Dim repitems As XmlElement = Nothing
        Dim textalign As String = "Left"
        Dim captionalign As String = "Left"
        Dim HasCombinedFields As Boolean = False
        Dim BaseAppPath As String = String.Empty
        Dim FullImagePath As String = String.Empty
        Dim base64Image As String = String.Empty
        Dim imageMimeType As String = String.Empty

        Try
            Dim dt As DataTable = ReturnDataTblFromOURFiles(repid)
            If dt Is Nothing Then
                dt = ReturnDataTblFromXML(xsdfile)
            End If
            If dt Is Nothing OrElse dt.Columns Is Nothing OrElse dt.Columns.Count = 0 Then
                Return "ERROR!! " & repid & " - nothing to create ..."
            End If
            dt = CorrectDatasetColumns(dt)
            Dim dtrf As DataTable = GetReportFields(repid, True)

            Dim htFields As Hashtable = GetXSDFieldHashtable(repid)
            Dim htSQLTblFlds As Hashtable = SwitchKeyVal(htFields)
            Dim dr As DataRow = Nothing
            Dim val As String = String.Empty

            If dtrf.Rows.Count > 0 Then
                n = dtrf.Rows.Count
                showall = False
                For i = 0 To dtrf.Rows.Count - 1
                    dr = dtrf.Rows(i)
                    val = dr("Val")
                    If val.ToUpper.EndsWith("_COMBINED") Then
                        HasCombinedFields = True
                        Exit For
                    End If
                Next
            Else
                n = dt.Columns.Count
                showall = True
            End If

            'get report view record and report items records
            Dim dtrview As DataView = mRecords("SELECT * FROM ourreportview WHERE ReportId='" & repid & "'")
            Dim dtritems As DataView = mRecords("SELECT * FROM ourreportitems WHERE ReportId='" & repid & "' Order By ItemOrder")


            Dim dtbl As DataTable = dtritems.ToTable
            ' Create an XML document
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            Dim attr As XmlAttribute = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            Dim myconstring As String
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Else
                myconstring = myconstr
            End If

            AddElement(connectionProperties, "ConnectString", "")
            AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", repid)
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "3000")
            'Fields element
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                'If showall Then
                fldname = dt.Columns(i).Caption
                'Else
                '    fldname = dtrf.Rows(i)("Val").ToString
                'End If
                attr.Value = fldname
                AddElement(field, "DataField", fldname)
            Next
            'Add Combined Fields if defined
            If HasCombinedFields Then
                For i = 0 To dtrf.Rows.Count - 1
                    dr = dtrf.Rows(i)
                    val = dr("Val")
                    If val.ToUpper.EndsWith("_COMBINED") Then
                        field = AddElement(fields, "Field", Nothing)
                        attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = val
                        AddElement(field, "DataField", val)
                    End If
                Next
            End If
            'end of DataSources

            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            'If orien = "landscape" Then
            '    AddElement(reportSection, "Width", "11in")
            'Else
            '    AddElement(reportSection, "Width", "8.5in")
            'End If

            'Page
            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)

            Dim pagewidth As String
            Dim colwidth As Decimal = 2.0
            Dim bodywidth As Decimal
            Dim bodyheight As Decimal
            If orien = "landscape" Then
                bodywidth = 11 - dtrview.Table.Rows(0)("MarginLeft") - dtrview.Table.Rows(0)("MarginRight")
                bodyheight = 8.5 - dtrview.Table.Rows(0)("MarginTop") - dtrview.Table.Rows(0)("MarginBottom")
            Else
                bodywidth = 8.5 - dtrview.Table.Rows(0)("MarginLeft") - dtrview.Table.Rows(0)("MarginRight")
                bodyheight = 11 - dtrview.Table.Rows(0)("MarginTop") - dtrview.Table.Rows(0)("MarginBottom")
            End If
            AddElement(reportSection, "Width", bodywidth & "in")

            If orien = "landscape" Then
                AddElement(page, "PageWidth", "11in")
                AddElement(page, "PageHeight", "8.5in")
                pagewidth = "11in"
            Else
                AddElement(page, "PageWidth", "8.5in")
                AddElement(page, "PageHeight", "11in")
                pagewidth = "8.5in"
            End If
            'HEADER   +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim headerHeight As Decimal = dtrview.Table.Rows(0)("HeaderHeight")

            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", headerHeight & "in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Header'"
            If dtritems.ToTable.Rows.Count > 0 Then
                repitems = AddElement(pageheader, "ReportItems", Nothing)
            End If

            dtbl = dtritems.ToTable
            For i = 0 To dtbl.Rows.Count - 1
                'Field in header   ----------------------------------------------------------------------------------------
                dr = dtbl.Rows(i)
                fldname = dr("ItemID")
                fldcaption = dr("Caption")
                flditemtype = dr("ReportItemType")
                If flditemtype <> "Image" Then
                    textalign = dr("TextAlign")
                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname         '!!!!!!
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    Xtextbox = Round(dr("X") / 96, 2)
                    Ytextbox = Round(dr("Y") / 96, 2)
                    Wtextbox = Round(dr("Width") / 96, 2)
                    Htextbox = Round(dr("Height") / 96, 2)
                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")
                    ''TODO size of fields
                    'If flditemtype = "Label" Then
                    '    AddElement(textbox, "Width", Wtextbox.ToString & "in")
                    'Else
                    '    'TODO colwidth for now
                    '    AddElement(textbox, "Width", colwidth.ToString & "in")
                    'End If

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    'style
                    If flditemtype = "Label" Then
                        fontnam = dr("CaptionFontName").ToString
                        fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("CaptionFontStyle").ToString
                        forecolr = dr("CaptionForeColor").ToString
                        backcolr = dr("CaptionBackColor").ToString
                        bordercolr = dr("CaptionBorderColor").ToString
                        borderstyle = dr("CaptionBorderStyle").ToString
                        borderwidth = dr("CaptionBorderWidth").ToString
                        underlin = dr("CaptionUnderline").ToString
                        strikeout = dr("CaptionStrikeout").ToString
                    Else
                        fontnam = dr("FontName").ToString
                        fontsiz = Round(CInt(dr("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("FontStyle").ToString
                        forecolr = dr("ForeColor").ToString
                        backcolr = dr("BackColor").ToString
                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString
                        underlin = dr("Underline").ToString
                        strikeout = dr("Strikeout").ToString
                    End If
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    'fldval
                    fldval = fldcaption
                    If flditemtype = "ReportTitle" Then
                        'repttl
                        fldval = repttl

                    ElseIf flditemtype = "ReportComments" Then
                        'PageFtr
                        fldval = PageFtr

                    ElseIf flditemtype = "SqlQuery" Then
                        'mSQL
                        fldval = mSQL

                    ElseIf flditemtype = "Label" Then
                        fldval = dr("Caption").ToString

                    ElseIf flditemtype = "PageNumber" Then
                        fldval = "=Globals!PageNumber"

                    ElseIf flditemtype = "PageNofM" Then
                        'Dim npv As String = "Globals!PageNumber" & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        fldval = "=Globals!PageNumber" & "  & "" of "" & " & "Globals!TotalPages"
                    ElseIf flditemtype = "PrintDateTime" Then
                        fldval = "=Globals!ExecutionTime"
                    ElseIf flditemtype = "PrintDate" Then
                        'fldval = "=Format(Today(),"" M/d/yyyy"")"
                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.ShortDate)"
                    ElseIf flditemtype = "PrintTime" Then
                        'fldval = "=Format(Now(),""HH:mm"")"
                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.LongTime)"
                    End If

                    'fldval = fldval & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    AddElement(textRun, "Value", fldval)                             '!!!!!!!!

                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")

                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                ElseIf flditemtype = "Image" Then

                    If Not IsDBNull(dr("ImagePath")) AndAlso dr("ImagePath").ToString <> "" Then

                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString

                        imagePath = dr("ImagePath").ToString
                        imageFileValue = Piece(Piece(imagePath, "ImageFiles/", 2), ".", 1)
                        imageFileValue = Regex.Replace(imageFileValue, "[^a-zA-Z0-9_]", "")  'CLS-compliant
                        BaseAppPath = System.AppDomain.CurrentDomain.BaseDirectory()
                        FullImagePath = (BaseAppPath & imagePath).Replace("/", "\")
                        base64Image = GetEmbeddedImageForRdl(FullImagePath)
                        imageMimeType = GetImageMimeTypeForRdl(FullImagePath)

                        If Not imageMimeType.Contains("icon") Then
                            'Dim FullGifPath As String = FullImagePath.Replace(".ico", ".gif")
                            'If ConvertIconToGif(FullImagePath, FullGifPath) Then
                            '    imageMimeType = GetImageMimeTypeForRdl(FullGifPath)
                            'End If
                            image = AddElement(repitems, "Image", Nothing)
                            attr = image.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = fldname         '!!!!!!
                            AddElement(image, "Source", "Embedded")
                            AddElement(image, "Value", imageFileValue)
                            AddElement(image, "Sizing", "FitProportional")

                            Ximage = Round(dr("X") / 96, 2)
                            If Ximage < 0 Then Ximage = Ximage * -1
                            Yimage = Round(dr("Y") / 96, 2)
                            If Yimage < 0 Then Yimage = Yimage * -1

                            Wimage = Round(dr("Width") / 96, 2)
                            Himage = Round(dr("Height") / 96, 2)
                            AddElement(image, "Top", Yimage.ToString & "in")
                            AddElement(image, "Left", Ximage.ToString & "in")
                            AddElement(image, "Height", Himage.ToString & "in")
                            AddElement(image, "Width", Wimage.ToString & "in")
                            AddElement(image, "ZIndex", 1)

                            style = AddElement(image, "Style", Nothing)
                            border = AddElement(style, "Border", Nothing)

                            If borderstyle <> "None" Then
                                If borderwidth = "1" Then
                                    bordWidth = "0.25pt"
                                Else
                                    bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                End If
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Top border
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Bottom border
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Left border
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Right border
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                            Else
                                AddElement(border, "Style", borderstyle)
                            End If
                            If Not hasImages Then
                                hasImages = True
                                embeddedImages = AddElement(report, "EmbeddedImages", Nothing)
                            End If
                            embeddedImage = AddElement(embeddedImages, "EmbeddedImage", Nothing)
                            attr = embeddedImage.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = imageFileValue
                            AddElement(embeddedImage, "MIMEType", imageMimeType)
                            AddElement(embeddedImage, "ImageData", base64Image)
                        End If
                    End If
                End If

            Next
            ' Header Style +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim headerBackColor As String = dtrview.Table.Rows(0)("HeaderBackColor")
            Dim headerBorderColor As String = dtrview.Table.Rows(0)("HeaderBorderColor")
            Dim headerBorderStyle As String = dtrview.Table.Rows(0)("HeaderBorderStyle")
            Dim headerBorderWidth As String = dtrview.Table.Rows(0)("HeaderBorderWidth")
            Dim headerBordWidth As String

            If headerBorderWidth = "1" Then
                headerBordWidth = "0.25pt"
            Else
                headerBordWidth = (CInt(headerBorderWidth) / 1.3) & "pt"
            End If

            Dim headerStyle As XmlElement = AddElement(pageheader, "Style", Nothing)

            Dim headerBorder As XmlElement = AddElement(headerStyle, "Border", Nothing)
            AddElement(headerBorder, "Color", headerBorderColor)
            AddElement(headerBorder, "Style", headerBorderStyle)
            AddElement(headerBorder, "Width", headerBordWidth)

            'Dim headerBottomBorder = AddElement(headerStyle, "BottomBorder", Nothing)
            'AddElement(headerBottomBorder, "Color", headerBorderColor)
            'AddElement(headerBottomBorder, "Style", headerBorderStyle)

            AddElement(headerStyle, "BackgroundColor", headerBackColor)
            'FOOTER   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            Dim footerHeight As Decimal = dtrview.Table.Rows(0)("FooterHeight")

            AddElement(pagefooter, "Height", footerHeight & "in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Footer'"
            If dtritems.ToTable.Rows.Count > 0 Then
                repitems = AddElement(pagefooter, "ReportItems", Nothing)
            End If

            dtbl = dtritems.ToTable
            For i = 0 To dtbl.Rows.Count - 1
                'Field in footer   ---------------------------------------------------------------------------------------
                dr = dtbl.Rows(i)
                fldname = dr("ItemID")
                fldcaption = dr("Caption")
                flditemtype = dr("ReportItemType")
                If flditemtype <> "Image" Then
                    textalign = dr("TextAlign")
                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname         '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    Xtextbox = Round(dr("X") / 96, 2)
                    Ytextbox = Round(dr("Y") / 96, 2)
                    Wtextbox = Round(dr("Width") / 96, 2)
                    Htextbox = Round(dr("Height") / 96, 2)
                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")

                    'TODO size of fields
                    'If flditemtype = "Label" Then
                    '    AddElement(textbox, "Width", Wtextbox.ToString & "in")
                    'Else
                    '    'TODO colwidth for now
                    '    AddElement(textbox, "Width", colwidth.ToString & "in")
                    'End If
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    'style
                    If flditemtype = "Label" Then
                        fontnam = dr("CaptionFontName").ToString
                        fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("CaptionFontStyle").ToString
                        forecolr = dr("CaptionForeColor").ToString
                        backcolr = dr("CaptionBackColor").ToString
                        bordercolr = dr("CaptionBorderColor").ToString
                        borderstyle = dr("CaptionBorderStyle").ToString
                        borderwidth = dr("CaptionBorderWidth").ToString
                        underlin = dr("CaptionUnderline").ToString
                        strikeout = dr("CaptionStrikeout").ToString
                    Else
                        fontnam = dr("FontName").ToString
                        fontsiz = Round(CInt(dr("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("FontStyle").ToString
                        forecolr = dr("ForeColor").ToString
                        backcolr = dr("BackColor").ToString
                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString
                        underlin = dr("Underline").ToString
                        strikeout = dr("Strikeout").ToString
                    End If
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    fldval = fldcaption
                    If flditemtype = "ReportTitle" Then
                        'repttl
                        fldval = repttl

                    ElseIf flditemtype = "ReportComments" Then
                        'PageFtr
                        fldval = PageFtr

                    ElseIf flditemtype = "SqlQuery" Then
                        'mSQL
                        fldval = mSQL

                    ElseIf flditemtype = "Label" Then
                        fldval = dr("Caption").ToString

                    ElseIf flditemtype = "PageNumber" Then
                        fldval = "=Globals!PageNumber"

                    ElseIf flditemtype = "PageNofM" Then
                        fldval = "=Globals!PageNumber" & "  & "" of "" & " & "Globals!TotalPages"

                    ElseIf flditemtype = "PrintDateTime" Then
                        fldval = "=Globals!ExecutionTime"
                    ElseIf flditemtype = "PrintDate" Then
                        'fldval = "=Format(Today(),"" M/d/yyyy"")"
                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.ShortDate)"
                    ElseIf flditemtype = "PrintTime" Then
                        'fldval = "=Format(Now(),""HH:mm"")"
                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.LongTime)"
                    End If


                    AddElement(textRun, "Value", fldval)                             '!!!!!!!!

                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")

                    'border
                    border = AddElement(style, "Border", Nothing)

                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                ElseIf flditemtype = "Image" Then

                    If Not IsDBNull(dr("ImagePath")) AndAlso dr("ImagePath").ToString <> "" Then

                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString

                        imagePath = dr("ImagePath").ToString
                        imageFileValue = Piece(Piece(imagePath, "ImageFiles/", 2), ".", 1)
                        imageFileValue = Regex.Replace(imageFileValue, "[^a-zA-Z0-9_]", "")  'CLS-compliant
                        BaseAppPath = System.AppDomain.CurrentDomain.BaseDirectory()
                        FullImagePath = (BaseAppPath & imagePath).Replace("/", "\")
                        base64Image = GetEmbeddedImageForRdl(FullImagePath)
                        imageMimeType = GetImageMimeTypeForRdl(FullImagePath)

                        If Not imageMimeType.Contains("icon") Then
                            'Dim FullGifPath As String = FullImagePath.Replace(".ico", ".gif")
                            'If ConvertIconToGif(FullImagePath, FullGifPath) Then
                            '    imageMimeType = GetImageMimeTypeForRdl(FullGifPath)
                            'End If
                            image = AddElement(repitems, "Image", Nothing)
                            attr = image.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = fldname         '!!!!!!
                            AddElement(image, "Source", "Embedded")
                            AddElement(image, "Value", imageFileValue)
                            AddElement(image, "Sizing", "FitProportional")

                            Ximage = Round(dr("X") / 96, 2)
                            If Ximage < 0 Then Ximage = Ximage * -1
                            Yimage = Round(dr("Y") / 96, 2)
                            If Yimage < 0 Then Yimage = Yimage * -1


                            Wimage = Round(dr("Width") / 96, 2)
                            Himage = Round(dr("Height") / 96, 2)
                            AddElement(image, "Top", Yimage.ToString & "in")
                            AddElement(image, "Left", Ximage.ToString & "in")
                            AddElement(image, "Height", Himage.ToString & "in")
                            AddElement(image, "Width", Wimage.ToString & "in")
                            AddElement(image, "ZIndex", 1)

                            style = AddElement(image, "Style", Nothing)
                            border = AddElement(style, "Border", Nothing)

                            If borderstyle <> "None" Then
                                If borderwidth = "1" Then
                                    bordWidth = "0.25pt"
                                Else
                                    bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                End If
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Top border
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Bottom border
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Left border
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Right border
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                            Else
                                AddElement(border, "Style", borderstyle)
                            End If
                            If Not hasImages Then
                                hasImages = True
                                embeddedImages = AddElement(report, "EmbeddedImages", Nothing)
                            End If
                            embeddedImage = AddElement(embeddedImages, "EmbeddedImage", Nothing)
                            attr = embeddedImage.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = imageFileValue
                            AddElement(embeddedImage, "MIMEType", imageMimeType)
                            AddElement(embeddedImage, "ImageData", base64Image)
                        End If
                    End If
                End If
            Next
            ' Footer Style +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim footerBackColor As String = dtrview.Table.Rows(0)("FooterBackColor")
            Dim footerBorderColor As String = dtrview.Table.Rows(0)("FooterBorderColor")
            Dim footerBorderWidth As String = dtrview.Table.Rows(0)("FooterBorderWidth")
            Dim footerBorderStyle As String = dtrview.Table.Rows(0)("FooterBorderStyle")

            Dim footerBordWidth As String

            If footerBorderWidth = "1" Then
                footerBordWidth = "0.25pt"
            Else
                footerBordWidth = (CInt(footerBorderWidth) / 1.3) & "pt"
            End If

            Dim footerStyle As XmlElement = AddElement(pagefooter, "Style", Nothing)

            Dim footerBorder As XmlElement = AddElement(footerStyle, "Border", Nothing)
            AddElement(footerBorder, "Color", footerBorderColor)
            AddElement(footerBorder, "Style", footerBorderStyle)
            AddElement(footerBorder, "Width", footerBordWidth)

            'Dim footerTopBorder = AddElement(footerStyle, "TopBorder", Nothing)
            'AddElement(footerTopBorder, "Color", footerBorderColor)
            'AddElement(footerTopBorder, "Style", footerBorderStyle)

            AddElement(footerStyle, "BackgroundColor", footerBackColor)

            ' Margins
            AddElement(page, "LeftMargin", dtrview.Table.Rows(0)("MarginLeft").ToString & "in")
            AddElement(page, "RightMargin", dtrview.Table.Rows(0)("MarginRight").ToString & "in")
            AddElement(page, "TopMargin", dtrview.Table.Rows(0)("MarginTop").ToString & "in")
            AddElement(page, "BottomMargin", dtrview.Table.Rows(0)("MarginBottom").ToString & "in")

            'BODY   +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", bodyheight & "in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            Dim pagebreak As XmlElement

            ' Tablix element
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix1"
            AddElement(tablix, "DataSetName", repid)
            'AddElement(tablix, "Top", dtrview.Table.Rows(0)("MarginTop").ToString & "in")
            'AddElement(tablix, "Top", "0.1in")
            'AddElement(tablix, "Left", dtrview.Table.Rows(0)("MarginLeft").ToString & "in")
            AddElement(tablix, "Height", "0.5in")
            AddElement(tablix, "RepeatColumnHeaders", "true")
            AddElement(tablix, "FixedColumnHeaders", "true")
            AddElement(tablix, "RepeatRowHeaders", "true")
            AddElement(tablix, "FixedRowHeaders", "true")

            Dim tablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            Dim ltablex As Integer = 0

            'TablixColumns element
            Dim tablixColumns As XmlElement = AddElement(tablixBody, "TablixColumns", Nothing)
            Dim tablixColumn As XmlElement
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Details'"

            'get tabular column widths
            dtbl = dtritems.ToTable
            Dim TotalWidth As Decimal = 0.00
            Dim WidthVal As Decimal = 0.00
            Dim sWidthVal As String = String.Empty

            For i = 0 To dtbl.Rows.Count - 1
                dr = dtbl.Rows(i)
                sWidthVal = IIf(IsDBNull(dr("TabularColumnWidth")), "1.5in", dr("TabularColumnWidth").ToString)
                If sWidthVal = String.Empty Then sWidthVal = "1.5in"
                tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
                AddElement(tablixColumn, "Width", sWidthVal)

                WidthVal = Decimal.Parse(Piece(sWidthVal, "in", 1))
                TotalWidth += WidthVal
            Next
            AddElement(tablix, "Width", TotalWidth.ToString & "in")

            'For i = 0 To n - 1
            '    tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
            '    AddElement(tablixColumn, "Width", "1.5in")  'Unit.Pixel(dt.Columns(i).Caption.Length).ToString
            'Next

            'AddElement(tablix, "Width", (n * 1.5).ToString & "in") 'Unit.Pixel(ltablex).ToString

            Dim dtGrpRptItemByOrder As DataTable

            'first tablixRow
            tablixRows = AddElement(tablixBody, "TablixRows", Nothing)
            Dim tablixRow As XmlElement = AddElement(tablixRows, "TablixRow", Nothing)

            If Not HasReportGroups(repid) Then
                AddElement(tablixRow, "Height", "0.25in")
            ElseIf HasGroupReportItems(repid) Then
                dtGrpRptItemByOrder = GetGroupReportItemsByItemOrder(repid)
                dr = dtGrpRptItemByOrder.Rows(0)
                Htextbox = Round(dr("Height") / 96, 2)
                AddElement(tablixRow, "Height", Htextbox.ToString & "in")
            End If
            Dim tablixCells As XmlElement = AddElement(tablixRow, "TablixCells", Nothing)


            '****************************************** Group Name Rows ********************************************
            Dim dtgrp As DataTable = GetReportGroups(repid)
            Dim dtGrpItem As DataTable
            Dim drGrpItem As DataRow
            Dim bHasGroupReportItem As Boolean = False

            Dim hdrgr As String = String.Empty

            For j = 0 To dtgrp.Rows.Count - 1
                dr = dtgrp.Rows(j)

                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dr("CalcField").ToString)
                If dr("GroupField") <> "Overall" Then
                    If j = 0 OrElse dr("GroupField") <> dtgrp.Rows(j - 1)("GroupField") Then
                        drGrpItem = Nothing
                        bHasGroupReportItem = False

                        dtGrpItem = GetGroupReportItem(repid, dr("GroupField"))
                        If dtGrpItem.Rows.Count > 0 Then
                            drGrpItem = dtGrpItem.Rows(0)
                            bHasGroupReportItem = True
                        End If

                        If bHasGroupReportItem Then
                            Htextbox = Round(drGrpItem("Height") / 96, 2)
                        Else
                            Htextbox = 0.25
                        End If

                        'tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        'AddElement(tablixRow, "Height", Htextbox.ToString & "in")

                        'tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                        grpfrname = GetFriendlyReportGroupName(repid, dr("GroupField").ToString, dr("CalcField").ToString)
                        hdrgr = "="" "
                        For k = 0 To j
                            hdrgr = hdrgr & "   "
                        Next
                        hdrgr = hdrgr & grpfrname & "  "" & Fields!" & dr("GroupField") & ".Value "
                        'add tablexRow for the group friendly name
                        ' TablixCell element (column with group name)
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "GroupHdr" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        'If bHasGroupReportItem Then
                        '    AddElement(textbox, "Height", Htextbox.ToString & "in")
                        'End If
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Left")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", hdrgr)                             '!!!!!!!! 
                        style = AddElement(textRun, "Style", Nothing)

                        If bHasGroupReportItem Then
                            fontnam = drGrpItem("FontName").ToString
                            fontsiz = Round(CInt(drGrpItem("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                            fontstyl = drGrpItem("FontStyle").ToString
                            forecolr = drGrpItem("ForeColor").ToString
                            backcolr = drGrpItem("BackColor").ToString
                            bordercolr = drGrpItem("BorderColor").ToString
                            borderstyle = drGrpItem("BorderStyle").ToString
                            borderwidth = drGrpItem("BorderWidth").ToString
                            underlin = drGrpItem("Underline").ToString
                            strikeout = drGrpItem("Strikeout").ToString

                            If fontstyl.ToLower.Contains("bold") Then
                                fontweight = "Bold"
                            Else
                                fontweight = "Default"
                            End If
                            fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                            If fontstyl = "" Then fontstyl = "Default"
                            If underlin = "True" Then
                                textdecor = "Underline"
                            Else
                                textdecor = "Default"
                            End If

                            'TextRun style
                            AddElement(style, "FontStyle", fontstyl)
                            AddElement(style, "FontFamily", fontnam)
                            AddElement(style, "Color", forecolr)
                            AddElement(style, "FontWeight", fontweight)
                            AddElement(style, "FontSize", fontsiz)
                            AddElement(style, "TextDecoration", textdecor)

                            'AddElement(style, "PaddingLeft", "2pt")
                            'AddElement(style, "PaddingRight", "2pt")
                            'AddElement(style, "PaddingTop", "2pt")
                            'AddElement(style, "PaddingBottom", "2pt")

                            style = AddElement(textbox, "Style", Nothing)
                            'Textbox style
                            AddElement(style, "BackgroundColor", backcolr)
                            AddElement(style, "VerticalAlign", "Middle")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")

                        Else
                            AddElement(style, "FontStyle", "Italic")
                            AddElement(style, "Color", "White")
                            AddElement(style, "FontWeight", "Bold")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")

                            style = AddElement(textbox, "Style", Nothing)
                            'Textbox style
                            AddElement(style, "BackgroundColor", "DimGray")
                            AddElement(style, "VerticalAlign", "Middle")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")

                        End If

                        'AddElement(style, "Color", "White")
                        'AddElement(style, "TextDecoration", "Underline")
                        'AddElement(style, "TextAlign", "Left")
                        'AddElement(style, "FontWeight", "Bold")
                        'AddElement(style, "PaddingLeft", "2pt")
                        'AddElement(style, "PaddingRight", "2pt")
                        'AddElement(style, "PaddingTop", "2pt")
                        'AddElement(style, "PaddingBottom", "2pt")
                        'style = AddElement(textbox, "Style", Nothing)
                        'AddElement(style, "BackgroundColor", "DimGray")

                        'border
                        border = AddElement(style, "Border", Nothing)
                        If borderstyle <> "None" Then
                            If borderwidth = "1" Then
                                bordWidth = "0.25pt"
                            Else
                                bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                            End If
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Top border
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Bottom border
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Left border
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Right border
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                        Else
                            AddElement(border, "Style", borderstyle)
                        End If

                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "TopBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "BottomBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "LeftBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "RightBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")

                        'add columns and colspan
                        colspan = AddElement(cellContents, "ColSpan", n)
                        For k = 0 To n - 2
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        Next

                        'add tablix row for next with group and last one for columns names
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        If j < (dtgrp.Rows.Count - 1) AndAlso dtgrp.Rows(j + 1)("GroupField") <> dr("GroupField") Then
                            dr = dtgrp.Rows(j + 1)
                            dtGrpItem = GetGroupReportItem(repid, dr("GroupField"))

                            Htextbox = Round(dtGrpItem.Rows(0)("Height") / 96, 2)
                            AddElement(tablixRow, "Height", Htextbox.ToString & "in")
                        Else
                            AddElement(tablixRow, "Height", "0.25in")
                        End If
                        'AddElement(tablixRow, "Height", "0.25in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        End If
                    End If
            Next
            '******************************************* Details *************************************************
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Details'"

            'columns captions and values
            Dim tblfld As String = ""
            dtbl = dtritems.ToTable

            ' Column Captions
            For i = 0 To dtbl.Rows.Count - 1
                dr = dtbl.Rows(i)
                flditemtype = dr("ReportItemType")
                If flditemtype = "DataField" Then
                    tblfld = dr("SQLTable") & "." & dr("SQLField")
                    fldname = htSQLTblFlds(tblfld)
                    fldcaption = dr("Caption")

                    captionalign = dr("CaptionTextAlign")
                    If fldcaption = fldname Then
                        fldfriendname = GetFriendlyReportFieldName(repid, fldname)
                    Else
                        fldfriendname = fldcaption
                    End If

                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname & "h"     '!!!!
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", captionalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    'Style
                    fontnam = dr("CaptionFontName").ToString
                    fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                    fontstyl = dr("CaptionFontStyle").ToString
                    forecolr = dr("CaptionForeColor").ToString
                    backcolr = dr("CaptionBackColor").ToString
                    bordercolr = dr("CaptionBorderColor").ToString
                    borderstyle = dr("CaptionBorderStyle").ToString
                    borderwidth = dr("CaptionBorderWidth").ToString
                    underlin = dr("CaptionUnderline").ToString
                    strikeout = dr("CaptionStrikeout").ToString
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    AddElement(textRun, "Value", fldfriendname)  '!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Center")

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")

                    'border
                    border = AddElement(style, "Border", Nothing)

                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                End If
            Next

            'TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.25in")
            tablixCells = AddElement(tablixRow, "TablixCells", Nothing)


            ' fields
            For i = 0 To dtbl.Rows.Count - 1
                dr = dtbl.Rows(i)
                flditemtype = dr("ReportItemType")
                If flditemtype = "DataField" Then
                    tblfld = dr("SQLTable") & "." & dr("SQLField")
                    fldname = htSQLTblFlds(tblfld)

                    textalign = dr("TextAlign")
                    dtrf.DefaultView.RowFilter = ""
                    dtrf.DefaultView.RowFilter = "Val='" & fldname & "'"
                    If dtrf.DefaultView.ToTable.Rows.Count > 0 Then
                        fldexpr = dtrf.DefaultView.ToTable.Rows(0)("Prop2").ToString
                    End If
                    ' TablixCell element (first cell)
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname

                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    'AddElement(style, "TextAlign", "Center")
                    If textalign <> "Auto" Then AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    'add expressions
                    If fldexpr = "" Then
                        'if numeric field
                        If ColumnTypeIsNumeric(dt.Columns(fldname)) Then
                            AddElement(textRun, "Value", "=Round(Fields!" & fldname & ".Value, 2)")  '!!!!!!
                        Else
                            'not numeric 
                            AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                        End If
                    Else
                        AddElement(textRun, "Value", "=" & fldexpr)  '!!!!!!
                    End If

                    style = AddElement(textRun, "Style", Nothing)
                    'style
                    fontnam = dr("FontName").ToString
                    fontsiz = Round(CInt(dr("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                    fontstyl = dr("FontStyle").ToString
                    forecolr = dr("ForeColor").ToString
                    backcolr = dr("BackColor").ToString
                    underlin = dr("Underline").ToString
                    strikeout = dr("Strikeout").ToString
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If

                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    '    AddElement(border, "Color", "LightGrey")
                    '    AddElement(border, "Width", "0.25pt")
                    '    border = AddElement(style, "TopBorder", Nothing)
                    '    AddElement(border, "Style", "Solid")
                    '    AddElement(border, "Color", "LightGrey")
                    '    AddElement(border, "Width", "0.25pt")
                    '    border = AddElement(style, "BottomBorder", Nothing)
                    '    AddElement(border, "Style", "Solid")
                    '    AddElement(border, "Color", "LightGrey")
                    '    AddElement(border, "Width", "0.25pt")
                    '    border = AddElement(style, "LeftBorder", Nothing)
                    '    AddElement(border, "Style", "Solid")
                    '    AddElement(border, "Color", "LightGrey")
                    '    AddElement(border, "Width", "0.25pt")
                    '    border = AddElement(style, "RightBorder", Nothing)
                    '    AddElement(border, "Style", "Solid")
                    '    AddElement(border, "Color", "LightGrey")
                    '    AddElement(border, "Width", "0.25pt")
                End If
            Next
            ' ********************************** subtotal rows **************************
            Dim totgrp As String = String.Empty
            Dim totgr As String = String.Empty
            'add groups subtotals rows 
            Dim dtgt As DataTable = dtgrp 'GetReportGroups(repid)
            For i = 0 To dtgt.Rows.Count - 1
                j = dtgt.Rows.Count - 1 - i
                'group statistics title
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgt.Rows(j)("CalcField").ToString)
                If dtgt.Rows(j)("GroupField") = "Overall" Then
                    totgr = "Overall totals of " & fldfrname
                    'Else 'not overall
                    'For l = 1 To j
                    '    If dtgt.Rows(l)("GroupField") <> "Overall" AndAlso dtgt.Rows(j)("GroupField") <> dtgt.Rows(l)("GroupField") Then
                    '        grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(l)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    '        If l = 0 OrElse (l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField")) Then
                    '            totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(l)("GroupField") & ".Value "
                    '            If l < j Then totgr = totgr & " & "" "
                    '        End If
                    '    End If
                    'Next
                    'grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(j)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    'totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(j)("GroupField") & ".Value "

                Else 'not overall
                    totgr = "=""Subtotals of " & fldfrname & " for: "
                    For l = 0 To j
                        If dtgt.Rows(l)("GroupField") <> "Overall" AndAlso dtgt.Rows(j)("GroupField") <> dtgt.Rows(l)("GroupField") Then
                            grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(l)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                            'If l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField") Then
                            If l = 0 OrElse (l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField")) Then
                                totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(l)("GroupField") & ".Value "
                                If l < j Then totgr = totgr & " & "" "
                            End If
                        End If
                    Next
                    grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(j)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(j)("GroupField") & ".Value "
                End If
                'group title stats rows
                m = dtgt.Rows(j)("CntChk") + dtgt.Rows(j)("SumChk") + dtgt.Rows(j)("MaxChk") + dtgt.Rows(j)("MinChk") + dtgt.Rows(j)("AvgChk") + dtgt.Rows(j)("StDevChk") + dtgt.Rows(j)("CntDistChk") + dtgt.Rows(j)("FirstChk") + dtgt.Rows(j)("LastChk")
                If m > 0 Then 'stats requested
                    ' Tablix row for group totals name
                    tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                    AddElement(tablixRow, "Height", "0.25In")
                    tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    ' TablixCell element (group subtotal name)
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = dtgt.Rows(j)("GroupField") & "gr" & j.ToString      '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", "Left")

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", totgr)        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    'AddElement(style, "TextDecoration", "Underline")
                    'AddElement(style, "TextAlign", "Left")
                    AddElement(style, "FontWeight", "Bold")
                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If

                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'add columns and colspan
                    colspan = AddElement(cellContents, "ColSpan", n)
                    'add tablix row for each stat with columns and colspan
                    For k = 0 To n - 2
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    Next

                    ' Tablix rows for stats
                    If n < 6 Then 'less than 6 columns for stats - add  6 rows
                        For k = 0 To 8  'stats 5
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'add row
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.25In")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                                'text align
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Left")

                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"  '!!!!!!!!!!!!!!!!!!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "Last(" & fldval & ")"
                                End If
                                fldval = " & " & fldval
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Count:  """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Sum:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Max:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Min:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Avg:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""StDev:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""CntDist:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""First:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Last:   """ & fldval)
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                'AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)

                                If borderstyle <> "None" Then
                                    If borderwidth = "1" Then
                                        bordWidth = "0.25pt"
                                    Else
                                        bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                    End If
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Top border
                                    border = AddElement(style, "TopBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Bottom border
                                    border = AddElement(style, "BottomBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Left border
                                    border = AddElement(style, "LeftBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Right border
                                    border = AddElement(style, "RightBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                Else
                                    AddElement(border, "Style", borderstyle)
                                End If

                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "TopBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "BottomBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "LeftBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "RightBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            Else 'no stats for the column
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.25in")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                                ' TablixCell element (stat subtotal name) - column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "K" & k.ToString & "L" & l.ToString & "J" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                                'text align
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Left")

                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                'AddElement(textRun, "Value", " ")   '!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last: ")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                'AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")

                                'border
                                border = AddElement(style, "Border", Nothing)

                                If borderstyle <> "None" Then
                                    If borderwidth = "1" Then
                                        bordWidth = "0.25pt"
                                    Else
                                        bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                    End If
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Top border
                                    border = AddElement(style, "TopBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Bottom border
                                    border = AddElement(style, "BottomBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Left border
                                    border = AddElement(style, "LeftBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Right border
                                    border = AddElement(style, "RightBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                Else
                                    AddElement(border, "Style", borderstyle)
                                End If

                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "TopBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "BottomBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "LeftBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "RightBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            End If
                        Next
                    Else  'at least 8 columns for stats - add 2 rows
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.25in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        For k = 0 To 8  '8 stats names in one row
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then  'stat ordered
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) 
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "sth" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last:")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                'AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                'AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)

                                If borderstyle <> "None" Then
                                    If borderwidth = "1" Then
                                        bordWidth = "0.25pt"
                                    Else
                                        bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                    End If

                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Top border
                                    border = AddElement(style, "TopBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Bottom border
                                    border = AddElement(style, "BottomBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Left border
                                    border = AddElement(style, "LeftBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Right border
                                    border = AddElement(style, "RightBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                Else
                                    AddElement(border, "Style", borderstyle)
                                End If

                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "TopBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "BottomBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "LeftBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "RightBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "sthe" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            'AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            'AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)

                            If borderstyle <> "None" Then
                                If borderwidth = "1" Then
                                    bordWidth = "0.25pt"
                                Else
                                    bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                End If
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Top border
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Bottom border
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Left border
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Right border
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                            Else
                                AddElement(border, "Style", borderstyle)
                            End If

                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "TopBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "BottomBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "LeftBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "RightBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If

                        'second row with stats values
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.25in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        For k = 0 To 8  '8 stats 
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "=Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "=Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "=Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "=Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "=FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "=FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "=CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "=First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "=Last(" & fldval & ")"
                                End If
                                AddElement(textRun, "Value", fldval)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                'AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                'AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)

                                If borderstyle <> "None" Then
                                    If borderwidth = "1" Then
                                        bordWidth = "0.25pt"
                                    Else
                                        bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                    End If
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Top border
                                    border = AddElement(style, "TopBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Bottom border
                                    border = AddElement(style, "BottomBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Left border
                                    border = AddElement(style, "LeftBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                    'Right border
                                    border = AddElement(style, "RightBorder", Nothing)
                                    AddElement(border, "Style", borderstyle)
                                    AddElement(border, "Color", bordercolr)
                                    AddElement(border, "Width", bordWidth)
                                Else
                                    AddElement(border, "Style", borderstyle)
                                End If


                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "TopBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "BottomBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "LeftBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                                'border = AddElement(style, "RightBorder", Nothing)
                                'AddElement(border, "Style", "Solid")
                                'AddElement(border, "Color", "LightGrey")
                                'AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "stve" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            'AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            'AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)

                            If borderstyle <> "None" Then
                                If borderwidth = "1" Then
                                    bordWidth = "0.25pt"
                                Else
                                    bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                End If
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Top border
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Bottom border
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Left border
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Right border
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                            Else
                                AddElement(border, "Style", borderstyle)
                            End If

                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "TopBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "BottomBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "LeftBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'border = AddElement(style, "RightBorder", Nothing)
                            'AddElement(border, "Style", "Solid")
                            'AddElement(border, "Color", "LightGrey")
                            'AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If
                    End If
                End If
            Next
            ' *************************************** tablixColumnHierarchy **********
            Dim tablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim tablixMembers As XmlElement = AddElement(tablixColumnHierarchy, "TablixMembers", Nothing)
            For i = 0 To n - 1
                AddElement(tablixMembers, "TablixMember", Nothing)
            Next
            ' ************************************** tablixRowHierarchy **************
            Dim tablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim tablixMember As XmlElement
            Dim tablixMemberGrp As XmlElement
            Dim tablixMemberStat As XmlElement
            Dim group As XmlElement
            Dim groupexpressions As XmlElement
            Dim groupexpression As XmlElement
            Dim sortexpressions As XmlElement
            Dim sortexpression As XmlElement
            Dim value As XmlElement
            Dim grpnm As String = String.Empty
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", Nothing)
            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            'AddElement(tablixMember, "KeepWithGroup", "After")
            'AddElement(tablixMember, "RepeatOnNewPage", "true")
            AddElement(tablixMember, "KeepTogether", "true")

            'groups
            Dim ovrall As Integer = 0
            Dim dtgr As DataTable = dtgrp 'GetReportGroups(repid)
            If dtgr.Rows.Count = 0 Then
                '----------------------------------------------------- no groups  -----------------------------------------------------------------------------------------
                AddElement(tablixMember, "KeepWithGroup", "After")
                AddElement(tablixMember, "RepeatOnNewPage", "true")

                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMember, "DataElementName", "Detail_Collection")
                AddElement(tablixMember, "DataElementOutput", "Output")
                'AddElement(tablixMember, "KeepTogether", "true")
                group = AddElement(tablixMember, "Group", Nothing)
                attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = "Table1_Details_Group"
                AddElement(group, "DataElementName", "Detail")
                Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
                AddElement(tablixMembersNested, "TablixMember", Nothing)

            ElseIf dtgr.Rows(0)("GroupField").ToString.Trim = "Overall" AndAlso dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString.Trim = "Overall" Then
                '--------------------------------------------------  only overall group --------------------------------------------------------------------------------------

                'loop for groups Overall
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If

                    Else
                        Exit For
                    End If
                Next
                'last deepest group
                tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                'tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                'AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                'AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                tablixMember = DetailTablixMember(doc, tablixMember)



                '----------------------------------    overall and one or more not overall groups --------------------------------------------------------------------------------------

            Else
                grpnm = ""
                For i = 0 To dtgr.Rows.Count - 1
                    dr = dtgr.Rows(i)
                    If dr("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dr("CntChk") = 1 OrElse dr("SumChk") = 1 OrElse dr("MaxChk") = 1 OrElse dr("MinChk") = 1 OrElse dr("AvgChk") = 1 OrElse dr("StDevChk") = 1 OrElse dr("CntDistChk") = 1 OrElse dr("FirstChk") = 1 OrElse dr("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If
                    Else
                        Exit For
                    End If
                Next
                For i = 0 To dtgr.Rows.Count - 1  'loop for groups not Overall
                    dr = dtgr.Rows(i)
                    If dr("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dr("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dr("GroupField").ToString & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If

                        If (i = 0) OrElse (i > 0 AndAlso dr("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then
                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dr("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dr("GroupField").ToString & ".Value")
                            If i = ovrall OrElse dr("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                                pagebreak = AddElement(group, "PageBreak", Nothing)
                                AddElement(pagebreak, "BreakLocation", "End")
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            'add tablexMember for group name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            'add one more in the end... 
                            If dr("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            End If
                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                        End If

                        If i = dtgr.Rows.Count - 1 Then
                            tablixMember = DetailTablixMember(doc, tablixMember)
                        End If
                        'stats
                        If dr("CntChk") = 1 OrElse dr("SumChk") = 1 OrElse dr("MaxChk") = 1 OrElse dr("MinChk") = 1 OrElse dr("AvgChk") = 1 OrElse dr("StDevChk") = 1 OrElse dr("CntDistChk") = 1 OrElse dr("FirstChk") = 1 OrElse dr("LastChk") = 1 Then

                            'row for subtotal name
                            tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            'rows with stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If
                    End If
                Next

            End If

            'Save XML document in OURFiles
            ''flash to string
            Dim rdlstr As String = String.Empty
            Dim rdlfile As String = String.Empty
            Dim userupl As String = String.Empty
            Dim bydesigner As String = String.Empty
            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            rdlstr = sw.ToString
            Dim er As String = String.Empty
            rep = rdlpath & repid & ".rdl"
            Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                'read rdl from OURFiles
                rdlfile = dv.Table.Rows(0)("Path").ToString  'string with file path
                userupl = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
                bydesigner = dv.Table.Rows(0)("Prop2").ToString   'by designer
            End If
            If userupl.Trim = "" Then
                er = SaveRDLstringInOURFiles(repid, rdlstr)  'just generated
            Else
                Try
                    userupl = userupl.Replace("uploaded by", "").Trim
                    rdlfile = rdlfile.Replace("|", "\")  'in MySql file path does not saved properly !!!!
                    'copy user uploaded earlier rdl
                    Dim userrdl As String = rdlfile.Replace(".rdl", "UpldByUser" & userupl & ".rdl")
                    If File.Exists(userrdl) Then
                        File.Delete(userrdl)
                    End If
                    File.Copy(rdlfile, userrdl)  'save user rdl with new name
                    er = SaveRDLstringInOURFiles(repid, rdlstr, " ", " ")  'just generated but previously was uploaded by user
                Catch ex As Exception
                    er = "ERROR!! " & ex.Message
                End Try
            End If
            Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString

            If er = "Query executed fine." AndAlso myprovider <> "Oracle.ManagedDataAccess.Client" Then
                'delete file from RDLFILES folder if it is not user created rdl!!
                If File.Exists(rep) Then
                    File.Delete(rep)
                    rep = "Report format generated."
                End If
            Else
                doc.Save(rep)
            End If

            'If er = "Query executed fine." Then
            '    'delete file from RDLFILES folder if it is not user created rdl!!
            '    If File.Exists(rep) Then
            '        File.Delete(rep)
            '        rep = "Report format generated."
            '    End If
            'Else
            '    doc.Save(rep)
            'End If
            'TODO Remove it. Save RDL XML document to file temporary for now
            'doc.Save(rep)
            Return rep

        Catch ex As Exception
            Dim msg As String = "ERROR!! " & repid & " - " & ex.Message & vbCrLf
            msg &= ex.StackTrace
            Return msg
        End Try
        Return rep
    End Function
    Public Function CreateFreeFormReportForXSDByDesigner(repid As String, ByVal repttl As String, ByVal rdlpath As String, ByVal mSQL As String, ByVal datatype As String, ByVal orien As String, Optional ByVal PageFtr As String = "", Optional ByVal myconstr As String = "") As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        Dim rep As String = String.Empty
        Dim n, i, j, k, m, l As Integer
        Dim showall As Boolean = True     'show all fields 
        Dim fldname As String = String.Empty
        Dim fldcaption As String = String.Empty
        Dim fldexpr As String = String.Empty
        Dim flditemtype As String = String.Empty
        Dim tablixRows As XmlElement '= AddElement(tablixBody, "TablixRows", Nothing)
        Dim tablixCell As XmlElement
        Dim cellContents As XmlElement
        Dim textbox As XmlElement
        Dim paragraphs As XmlElement
        Dim paragraph As XmlElement
        Dim textRuns As XmlElement
        Dim textRun As XmlElement
        Dim style As XmlElement
        Dim border As XmlElement
        Dim image As XmlElement
        Dim embeddedImages As XmlElement
        Dim embeddedImage As XmlElement
        Dim hasImages As Boolean = False
        Dim name As String = String.Empty
        Dim colspan As XmlElement
        Dim fldval As String = String.Empty
        Dim fldfriendname As String = String.Empty
        Dim rectangl As XmlElement
        Dim Xtextbox As Decimal
        Dim Ytextbox As Decimal
        Dim Htextbox As Decimal
        Dim Wtextbox As Decimal
        Dim Ximage As Decimal
        Dim Yimage As Decimal
        Dim Himage As Decimal
        Dim Wimage As Decimal
        Dim imagePath As String = String.Empty
        Dim imageFileValue As String = String.Empty
        Dim fontnam As String = "Arial"
        Dim fontsiz As String = "12"
        Dim fontstyl As String = "Default"
        Dim fontweight As String = "Default"
        Dim underlin As String = "Default"
        Dim strikeout As String = "Default"
        Dim textdecor As String = "Default"
        Dim forecolr As String = "black"
        Dim backcolr As String = "white"
        Dim bordercolr As String = "black"
        Dim borderstyle As String = "None"
        Dim borderwidth As String = "1"
        Dim bordWidth As String = ""
        Dim textalign As String = "Left"
        Dim captionalign As String = "Left"
        Dim repitems As XmlElement = Nothing
        Dim HasCombinedFields As Boolean = False
        Dim BaseAppPath As String = String.Empty
        Dim FullImagePath As String = String.Empty
        Dim base64Image As String = String.Empty
        Dim imageMimeType As String = String.Empty

        'Dim txtbox As XmlElement

        Try
            Dim dt As DataTable = ReturnDataTblFromOURFiles(repid)
            If dt Is Nothing Then
                dt = ReturnDataTblFromXML(xsdfile)
            End If
            If dt Is Nothing OrElse dt.Columns Is Nothing OrElse dt.Columns.Count = 0 Then
                Return "ERROR!! " & rep & " - nothing to create ..."
            End If
            dt = CorrectDatasetColumns(dt)
            Dim dtrf As DataTable = GetReportFields(repid, True)

            Dim htFields As Hashtable = GetXSDFieldHashtable(repid)
            Dim htSQLTblFlds As Hashtable = SwitchKeyVal(htFields)

            Dim dr As DataRow
            Dim val As String

            If dtrf.Rows.Count > 0 Then
                n = dtrf.Rows.Count
                showall = False
                For i = 0 To dtrf.Rows.Count - 1
                    dr = dtrf.Rows(i)
                    val = dr("Val")
                    If val.ToUpper.EndsWith("_COMBINED") Then
                        HasCombinedFields = True
                        Exit For
                    End If
                Next
            Else
                n = dt.Columns.Count
                showall = True
            End If

            'get report view record and report items records
            Dim dtrview As DataView = mRecords("SELECT * FROM ourreportview WHERE ReportId='" & repid & "'")
            Dim dtritems As DataView = mRecords("SELECT * FROM ourreportitems WHERE ReportId='" & repid & "'")


            Dim dtbl As DataTable = dtritems.ToTable

            ' Create an XML document
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            Dim attr As XmlAttribute = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            Dim myconstring As String
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Else
                myconstring = myconstr
            End If

            AddElement(connectionProperties, "ConnectString", "")
            AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", repid)
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "3000")
            'Fields element
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                'If showall Then
                fldname = dt.Columns(i).Caption
                'Else
                '    fldname = dtrf.Rows(i)("Val").ToString
                'End If
                attr.Value = fldname
                AddElement(field, "DataField", fldname)
            Next
            'Add Combined Fields if defined
            If HasCombinedFields Then
                For i = 0 To dtrf.Rows.Count - 1
                    dr = dtrf.Rows(i)
                    val = dr("Val")
                    If val.ToUpper.EndsWith("_COMBINED") Then
                        field = AddElement(fields, "Field", Nothing)
                        attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = val
                        AddElement(field, "DataField", val)
                    End If
                Next
            End If
            'end of DataSources

            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            'If orien = "landscape" Then
            '    AddElement(reportSection, "Width", "11in")
            'Else
            '    AddElement(reportSection, "Width", "8.5in")
            'End If

            'Page
            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)
            Dim pagewidth As String

            If orien = "landscape" Then
                AddElement(page, "PageWidth", "11in")
                AddElement(page, "PageHeight", "8.5in")
                pagewidth = "11in"
            Else
                AddElement(page, "PageWidth", "8.5in")
                AddElement(page, "PageHeight", "11in")
                pagewidth = "8.5in"
            End If

            Dim colwidth As Decimal = 6.5
            Dim bodywidth As Decimal
            Dim bodyheight As Decimal
            If orien = "landscape" Then
                bodywidth = 11 - dtrview.Table.Rows(0)("MarginLeft") - dtrview.Table.Rows(0)("MarginRight")
                bodyheight = 8.5 - dtrview.Table.Rows(0)("MarginTop") - dtrview.Table.Rows(0)("MarginBottom") - 1.5 ' - (headerheight+footerheight)
            Else
                bodywidth = 8.5 - dtrview.Table.Rows(0)("MarginLeft") - dtrview.Table.Rows(0)("MarginRight")
                bodyheight = 11 - dtrview.Table.Rows(0)("MarginTop") - dtrview.Table.Rows(0)("MarginBottom") - 1.5 ' - (headerheight+footerheight)
            End If

            AddElement(reportSection, "Width", bodywidth & "in")

            Dim rowheight As Decimal = bodyheight

            'HEADER   +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim headerHeight As Decimal = dtrview.Table.Rows(0)("HeaderHeight")

            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)

            'AddElement(pageheader, "Height", "0.5in")
            AddElement(pageheader, "Height", headerHeight & "in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Header'"
            If dtritems.ToTable.Rows.Count > 0 Then
                repitems = AddElement(pageheader, "ReportItems", Nothing)
            End If

            dtbl = dtritems.ToTable
            For i = 0 To dtbl.Rows.Count - 1
                'Field in header   ----------------------------------------------------------------------------------------
                dr = dtbl.Rows(i)
                fldname = dr("ItemID")
                fldcaption = dr("Caption")
                flditemtype = dr("ReportItemType")
                If flditemtype <> "Image" Then
                    textalign = dr("TextAlign")
                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname         '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    Xtextbox = Round(dr("X") / 96, 2)
                    Ytextbox = Round(dr("Y") / 96, 2)
                    Wtextbox = Round(dr("Width") / 96, 2)
                    Htextbox = Round(dr("Height") / 96, 2)
                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")

                    ''TODO size of fields
                    'If flditemtype = "ReportTitle" OrElse flditemtype = "ReportComments" OrElse flditemtype = "SqlQuery" Then
                    '    'TODO colwidth for now
                    '    AddElement(textbox, "Width", colwidth.ToString & "in")
                    'Else
                    '    AddElement(textbox, "Width", Wtextbox.ToString & "in")
                    'End If

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    'style
                    If flditemtype = "Label" Then
                        fontnam = dr("CaptionFontName").ToString
                        fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("CaptionFontStyle").ToString
                        forecolr = dr("CaptionForeColor").ToString
                        backcolr = dr("CaptionBackColor").ToString
                        bordercolr = dr("CaptionBorderColor").ToString
                        borderstyle = dr("CaptionBorderStyle").ToString
                        borderwidth = dr("CaptionBorderWidth").ToString
                        underlin = dr("CaptionUnderline").ToString
                        strikeout = dr("CaptionStrikeout").ToString
                    Else
                        fontnam = dr("FontName").ToString
                        fontsiz = Round(CInt(dr("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("FontStyle").ToString
                        forecolr = dr("ForeColor").ToString
                        backcolr = dr("BackColor").ToString
                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString
                        underlin = dr("Underline").ToString
                        strikeout = dr("Strikeout").ToString
                    End If
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    'fldval
                    If flditemtype = "ReportTitle" Then
                        'repttl
                        fldval = repttl

                    ElseIf flditemtype = "ReportComments" Then
                        'PageFtr
                        fldval = PageFtr

                    ElseIf flditemtype = "SqlQuery" Then
                        'mSQL
                        fldval = mSQL

                    ElseIf flditemtype = "Label" Then
                        fldval = dr("Caption").ToString

                    ElseIf flditemtype = "PageNumber" Then
                        fldval = "=Globals!PageNumber"

                    ElseIf flditemtype = "PageNofM" Then
                        'Dim npv As String = "Globals!PageNumber" & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        fldval = "=Globals!PageNumber" & "  & "" of "" & " & "Globals!TotalPages"

                    ElseIf flditemtype = "PrintDateTime" Then
                        fldval = "=Globals!ExecutionTime"

                    ElseIf flditemtype = "PrintDate" Then

                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.ShortDate)"

                    ElseIf flditemtype = "PrintTime" Then

                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.LongTime)"

                    End If
                    AddElement(textRun, "Value", fldval)                             '!!!!!!!!

                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")

                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                ElseIf flditemtype = "Image" Then

                    If Not IsDBNull(dr("ImagePath")) AndAlso dr("ImagePath").ToString <> "" Then

                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString

                        imagePath = dr("ImagePath").ToString
                        imageFileValue = Piece(Piece(imagePath, "ImageFiles/", 2), ".", 1)
                        imageFileValue = Regex.Replace(imageFileValue, "[^a-zA-Z0-9_]", "")  'CLS-compliant
                        BaseAppPath = System.AppDomain.CurrentDomain.BaseDirectory()
                        FullImagePath = (BaseAppPath & imagePath).Replace("/", "\")
                        base64Image = GetEmbeddedImageForRdl(FullImagePath)
                        imageMimeType = GetImageMimeTypeForRdl(FullImagePath)

                        If Not imageMimeType.Contains("icon") Then
                            'Dim FullGifPath As String = FullImagePath.Replace(".ico", ".gif")
                            'If ConvertIconToGif(FullImagePath, FullGifPath) Then
                            '    imageMimeType = GetImageMimeTypeForRdl(FullGifPath)
                            'End If
                            image = AddElement(repitems, "Image", Nothing)
                            attr = image.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = fldname         '!!!!!!
                            AddElement(image, "Source", "Embedded")
                            AddElement(image, "Value", imageFileValue)
                            AddElement(image, "Sizing", "FitProportional")

                            Ximage = Round(dr("X") / 96, 2)
                            If Ximage < 0 Then Ximage = Ximage * -1
                            Yimage = Round(dr("Y") / 96, 2)
                            If Yimage < 0 Then Yimage = Yimage * -1

                            Wimage = Round(dr("Width") / 96, 2)
                            Himage = Round(dr("Height") / 96, 2)
                            AddElement(image, "Top", Yimage.ToString & "in")
                            AddElement(image, "Left", Ximage.ToString & "in")
                            AddElement(image, "Height", Himage.ToString & "in")
                            AddElement(image, "Width", Wimage.ToString & "in")
                            AddElement(image, "ZIndex", 1)

                            style = AddElement(image, "Style", Nothing)
                            border = AddElement(style, "Border", Nothing)

                            If borderstyle <> "None" Then
                                If borderwidth = "1" Then
                                    bordWidth = "0.25pt"
                                Else
                                    bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                End If
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Top border
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Bottom border
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Left border
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Right border
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                            Else
                                AddElement(border, "Style", borderstyle)
                            End If
                            If Not hasImages Then
                                hasImages = True
                                embeddedImages = AddElement(report, "EmbeddedImages", Nothing)
                            End If
                            embeddedImage = AddElement(embeddedImages, "EmbeddedImage", Nothing)
                            attr = embeddedImage.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = imageFileValue
                            AddElement(embeddedImage, "MIMEType", imageMimeType)
                            AddElement(embeddedImage, "ImageData", base64Image)
                        End If
                    End If
                End If
            Next

            ' Header style + ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim headerBackColor As String = dtrview.Table.Rows(0)("HeaderBackColor")
            Dim headerBorderColor As String = dtrview.Table.Rows(0)("HeaderBorderColor")
            Dim headerBorderStyle As String = dtrview.Table.Rows(0)("HeaderBorderStyle")
            Dim headerBorderWidth As String = dtrview.Table.Rows(0)("HeaderBorderWidth")
            Dim headerBordWidth As String

            If headerBorderWidth = "1" Then
                headerBordWidth = "0.25pt"
            Else
                headerBordWidth = (CInt(headerBorderWidth) / 1.3) & "pt"
            End If

            Dim headerStyle As XmlElement = AddElement(pageheader, "Style", Nothing)

            Dim headerBorder As XmlElement = AddElement(headerStyle, "Border", Nothing)
            AddElement(headerBorder, "Color", headerBorderColor)
            AddElement(headerBorder, "Style", headerBorderStyle)
            AddElement(headerBorder, "Width", headerBordWidth)

            AddElement(headerStyle, "BackgroundColor", headerBackColor)


            'style = AddElement(pageheader, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Style", "None")
            'border = AddElement(style, "BottomBorder", Nothing)
            'AddElement(border, "Style", "Solid")

            'Dim npv As String = "Globals!PageNumber" & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            'FOOTER   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            Dim footerHeight As Decimal = dtrview.Table.Rows(0)("FooterHeight")

            AddElement(pagefooter, "Height", footerHeight & "in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Footer'"
            If dtritems.ToTable.Rows.Count > 0 Then
                repitems = AddElement(pagefooter, "ReportItems", Nothing)
            End If
            dtbl = dtritems.ToTable
            For i = 0 To dtbl.Rows.Count - 1
                'Field in footer   ---------------------------------------------------------------------------------------
                dr = dtbl.Rows(i)
                fldname = dr("ItemID")
                fldcaption = dr("Caption")
                flditemtype = dr("ReportItemType")
                If flditemtype <> "Image" Then
                    textalign = dr("TextAlign")
                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname         '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    Xtextbox = Round(dr("X") / 96, 2)
                    Ytextbox = Round(dr("Y") / 96, 2)
                    Wtextbox = Round(dr("Width") / 96, 2)
                    Htextbox = Round(dr("Height") / 96, 2)
                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")
                    ''TODO
                    'If flditemtype = "ReportTitle" OrElse flditemtype = "ReportComments" OrElse flditemtype = "SqlQuery" Then
                    '    'colwidth for now
                    '    AddElement(textbox, "Width", colwidth.ToString & "in")
                    'Else
                    '    AddElement(textbox, "Width", Wtextbox.ToString & "pt")
                    'End If

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    'style
                    If flditemtype = "Label" Then
                        fontnam = dr("CaptionFontName").ToString
                        fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("CaptionFontStyle").ToString
                        forecolr = dr("CaptionForeColor").ToString
                        backcolr = dr("CaptionBackColor").ToString
                        bordercolr = dr("CaptionBorderColor").ToString
                        borderstyle = dr("CaptionBorderStyle").ToString
                        borderwidth = dr("CaptionBorderWidth").ToString
                        underlin = dr("CaptionUnderline").ToString
                        strikeout = dr("CaptionStrikeout").ToString
                    Else
                        fontnam = dr("FontName").ToString
                        fontsiz = Round(CInt(dr("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                        fontstyl = dr("FontStyle").ToString
                        forecolr = dr("ForeColor").ToString
                        backcolr = dr("BackColor").ToString
                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString
                        underlin = dr("Underline").ToString
                        strikeout = dr("Strikeout").ToString
                    End If
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    If flditemtype = "ReportTitle" Then
                        'repttl
                        fldval = repttl

                    ElseIf flditemtype = "ReportComments" Then
                        'PageFtr
                        fldval = PageFtr

                    ElseIf flditemtype = "SqlQuery" Then
                        'mSQL
                        fldval = mSQL

                    ElseIf flditemtype = "Label" Then
                        fldval = dr("Caption").ToString

                    ElseIf flditemtype = "PageNumber" Then
                        fldval = "=Globals!PageNumber"

                    ElseIf flditemtype = "PageNofM" Then
                        fldval = "=Globals!PageNumber" & "  & "" of "" & " & "Globals!TotalPages"

                    ElseIf flditemtype = "PrintDateTime" Then
                        fldval = "=Globals!ExecutionTime"

                    ElseIf flditemtype = "PrintDate" Then

                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.ShortDate)"

                    ElseIf flditemtype = "PrintTime" Then

                        fldval = "=FormatDatetime(Globals!ExecutionTime,DateFormat.LongTime)"
                    End If

                    AddElement(textRun, "Value", fldval)                             '!!!!!!!!

                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                ElseIf flditemtype = "Image" Then
                    If Not IsDBNull(dr("ImagePath")) AndAlso dr("ImagePath").ToString <> "" Then

                        bordercolr = dr("BorderColor").ToString
                        borderstyle = dr("BorderStyle").ToString
                        borderwidth = dr("BorderWidth").ToString

                        imagePath = dr("ImagePath").ToString
                        imageFileValue = Piece(Piece(imagePath, "ImageFiles/", 2), ".", 1)
                        imageFileValue = Regex.Replace(imageFileValue, "[^a-zA-Z0-9_]", "")  'CLS-compliant

                        BaseAppPath = System.AppDomain.CurrentDomain.BaseDirectory()
                        FullImagePath = (BaseAppPath & imagePath).Replace("/", "\")
                        base64Image = GetEmbeddedImageForRdl(FullImagePath)
                        imageMimeType = GetImageMimeTypeForRdl(FullImagePath)

                        If Not imageMimeType.Contains("icon") Then
                            'Dim FullGifPath As String = FullImagePath.Replace(".ico", ".gif")
                            'If ConvertIconToGif(FullImagePath, FullGifPath) Then
                            '    imageMimeType = GetImageMimeTypeForRdl(FullGifPath)
                            '    base64Image = GetEmbeddedImageForRdl(FullGifPath)
                            'End If
                            image = AddElement(repitems, "Image", Nothing)
                            attr = image.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = fldname         '!!!!!!
                            AddElement(image, "Source", "Embedded")
                            AddElement(image, "Value", imageFileValue)
                            AddElement(image, "Sizing", "FitProportional")

                            Ximage = Round(dr("X") / 96, 2)
                            If Ximage < 0 Then Ximage = Ximage * -1
                            Yimage = Round(dr("Y") / 96, 2)
                            If Yimage < 0 Then Yimage = Yimage * -1

                            Wimage = Round(dr("Width") / 96, 2)
                            Himage = Round(dr("Height") / 96, 2)
                            AddElement(image, "Top", Yimage.ToString & "in")
                            AddElement(image, "Left", Ximage.ToString & "in")
                            AddElement(image, "Height", Himage.ToString & "in")
                            AddElement(image, "Width", Wimage.ToString & "in")
                            AddElement(image, "ZIndex", 1)

                            style = AddElement(image, "Style", Nothing)
                            border = AddElement(style, "Border", Nothing)

                            If borderstyle <> "None" Then
                                If borderwidth = "1" Then
                                    bordWidth = "0.25pt"
                                Else
                                    bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                                End If
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Top border
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Bottom border
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Left border
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                                'Right border
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", borderstyle)
                                AddElement(border, "Color", bordercolr)
                                AddElement(border, "Width", bordWidth)
                            Else
                                AddElement(border, "Style", borderstyle)
                            End If
                            If Not hasImages Then
                                hasImages = True
                                embeddedImages = AddElement(report, "EmbeddedImages", Nothing)
                            End If
                            embeddedImage = AddElement(embeddedImages, "EmbeddedImage", Nothing)
                            attr = embeddedImage.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = imageFileValue
                            AddElement(embeddedImage, "MIMEType", imageMimeType)
                            AddElement(embeddedImage, "ImageData", base64Image)
                        End If
                    End If
                End If
            Next

            ' Footer Style +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            Dim footerBackColor As String = dtrview.Table.Rows(0)("FooterBackColor")
            Dim footerBorderColor As String = dtrview.Table.Rows(0)("FooterBorderColor")
            Dim footerBorderWidth As String = dtrview.Table.Rows(0)("FooterBorderWidth")
            Dim footerBorderStyle As String = dtrview.Table.Rows(0)("FooterBorderStyle")

            Dim footerBordWidth As String

            If footerBorderWidth = "1" Then
                footerBordWidth = "0.25pt"
            Else
                footerBordWidth = (CInt(footerBorderWidth) / 1.3) & "pt"
            End If

            Dim footerStyle As XmlElement = AddElement(pagefooter, "Style", Nothing)

            Dim footerBorder As XmlElement = AddElement(footerStyle, "Border", Nothing)
            AddElement(footerBorder, "Color", footerBorderColor)
            AddElement(footerBorder, "Style", footerBorderStyle)
            AddElement(footerBorder, "Width", footerBordWidth)

            AddElement(footerStyle, "BackgroundColor", footerBackColor)

            'BODY   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++====

            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", bodyheight & "in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            Dim pagebreak As XmlElement

            ' Tablix element
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix1"
            AddElement(tablix, "DataSetName", repid)
            'AddElement(tablix, "Top", dtrview.Table.Rows(0)("MarginTop").ToString & "in")
            AddElement(tablix, "Top", "0.1in")
            AddElement(tablix, "Left", dtrview.Table.Rows(0)("MarginLeft").ToString & "in")
            'AddElement(tablix, "Height", "0.5in")
            AddElement(tablix, "RepeatColumnHeaders", "false")
            AddElement(tablix, "FixedColumnHeaders", "false")
            AddElement(tablix, "RepeatRowHeaders", "false")
            AddElement(tablix, "FixedRowHeaders", "false")
            'pagebreak = AddElement(tablix, "PageBreak", Nothing)
            'AddElement(pagebreak, "BreakLocation", "Between")

            Dim tablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            Dim ltablex As Integer = 0
            'TablixColumns element
            Dim tablixColumns As XmlElement = AddElement(tablixBody, "TablixColumns", Nothing)
            Dim tablixColumn As XmlElement
            Dim tablixRow As XmlElement
            Dim tablixCells As XmlElement
            'Dim rowheight As Decimal = bodyheight
            tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
            AddElement(tablixColumn, "Width", colwidth.ToString & "in")  'Unit.Pixel(dt.Columns(i).Caption.Length).ToString

            'Next

            'first tablixRow
            AddElement(tablix, "Width", colwidth.ToString & "in") 'Unit.Pixel(ltablex).ToString
            tablixRows = AddElement(tablixBody, "TablixRows", Nothing)
            'Dim tablixRow As XmlElement = AddElement(tablixRows, "TablixRow", Nothing)
            'AddElement(tablixRow, "Height", rowheight.ToString & "in")
            'Dim tablixCells As XmlElement = AddElement(tablixRow, "TablixCells", Nothing)
            'tablixCell = AddElement(tablixCells, "TablixCell", Nothing)

            'GROUPS
            'add tablixRows for the group names  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim dtgrp As DataTable = GetReportGroups(repid)
            Dim dtGrpItem As DataTable
            Dim drGrpItem As DataRow
            Dim bHasGroupReportItem As Boolean = False
            Dim hdrgr As String = String.Empty
            'Dim ovall As Integer = 0
            'If dtgrp.Rows(0)("GroupField").ToString = "Overall" Then ovall = 1
            For j = 0 To dtgrp.Rows.Count - 1
                dr = dtgrp.Rows(j)

                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgrp.Rows(j)("CalcField").ToString)

                If dr("GroupField") <> "Overall" Then

                    'no duplicates
                    If j = 0 OrElse dr("GroupField") <> dtgrp.Rows(j - 1)("GroupField") Then
                        drGrpItem = Nothing
                        bHasGroupReportItem = False

                        dtGrpItem = GetGroupReportItem(repid, dr("GroupField"))
                        If dtGrpItem.Rows.Count > 0 Then
                            drGrpItem = dtGrpItem.Rows(0)
                            bHasGroupReportItem = True
                        End If
                        If bHasGroupReportItem Then
                            Htextbox = Round(drGrpItem("Height") / 96, 2)
                        Else
                            Htextbox = 0.1
                        End If

                        grpfrname = GetFriendlyReportGroupName(repid, dtgrp.Rows(j)("GroupField").ToString, dtgrp.Rows(j)("CalcField").ToString)
                        hdrgr = "="" "
                        For k = 0 To j
                            hdrgr = hdrgr & "   "
                        Next
                        hdrgr = hdrgr & grpfrname & "  "" & Fields!" & dtgrp.Rows(j)("GroupField") & ".Value "
                        'add tablexRow for the group friendly name
                        ' TablixCell element (column with group name)
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", Htextbox.ToString & "in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "GroupHdr" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Left")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", hdrgr)                             '!!!!!!!! 
                        style = AddElement(textRun, "Style", Nothing)
                        If bHasGroupReportItem Then
                            fontnam = drGrpItem("FontName").ToString
                            fontsiz = Round(CInt(drGrpItem("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                            fontstyl = drGrpItem("FontStyle").ToString
                            forecolr = drGrpItem("ForeColor").ToString
                            backcolr = drGrpItem("BackColor").ToString
                            bordercolr = drGrpItem("BorderColor").ToString
                            borderstyle = drGrpItem("BorderStyle").ToString
                            borderwidth = drGrpItem("BorderWidth").ToString
                            underlin = drGrpItem("Underline").ToString
                            strikeout = drGrpItem("Strikeout").ToString

                            If fontstyl.ToLower.Contains("bold") Then
                                fontweight = "Bold"
                            Else
                                fontweight = "Default"
                            End If
                            fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                            If fontstyl = "" Then fontstyl = "Default"
                            If underlin = "True" Then
                                textdecor = "Underline"
                            Else
                                textdecor = "Default"
                            End If

                            'TextRun style
                            AddElement(style, "FontStyle", fontstyl)
                            AddElement(style, "FontFamily", fontnam)
                            AddElement(style, "Color", forecolr)
                            AddElement(style, "FontWeight", fontweight)
                            AddElement(style, "FontSize", fontsiz)
                            AddElement(style, "TextDecoration", textdecor)

                            style = AddElement(textbox, "Style", Nothing)
                            'Textbox style
                            AddElement(style, "BackgroundColor", backcolr)
                            AddElement(style, "VerticalAlign", "Middle")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                        Else
                            AddElement(style, "FontStyle", "Italic")
                            AddElement(style, "Color", "White")
                            'AddElement(style, "TextDecoration", "Underline")
                            'AddElement(style, "TextAlign", "Left")
                            AddElement(style, "FontWeight", "Bold")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "BackgroundColor", "DimGray")
                        End If

                        'border
                        border = AddElement(style, "Border", Nothing)
                        If borderstyle <> "None" Then
                            If borderwidth = "1" Then
                                bordWidth = "0.25pt"
                            Else
                                bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                            End If
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Top border
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Bottom border
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Left border
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                            'Right border
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", borderstyle)
                            AddElement(border, "Color", bordercolr)
                            AddElement(border, "Width", bordWidth)
                        Else
                            AddElement(border, "Style", borderstyle)
                        End If


                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "TopBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "BottomBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "LeftBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'border = AddElement(style, "RightBorder", Nothing)
                        'AddElement(border, "Style", "Solid")
                        'AddElement(border, "Color", "LightGrey")
                        'AddElement(border, "Width", "0.25pt")
                        'add columns and colspan
                        'colspan = AddElement(cellContents, "ColSpan", n)
                        'For k = 0 To n - 2
                        '    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        'Next

                        'add tablix row for next with group and last one for columns names
                        'tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        'AddElement(tablixRow, "Height", "0.1in")
                        'rowheight = rowheight - 0.1
                        'tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    End If
                End If
            Next

            'DETAILS  +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            'FILTER
            dtritems.RowFilter = ""
            dtritems.RowFilter = "Section='Details'"

            'TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", rowheight.ToString & "in")
            tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
            cellContents = AddElement(tablixCell, "CellContents", Nothing)
            rectangl = AddElement(cellContents, "Rectangle", Nothing)
            attr = rectangl.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Rectangle1"
            If dtritems.ToTable.Rows.Count > 0 Then
                repitems = AddElement(rectangl, "ReportItems", Nothing)
            End If

            'columns captions and values

            Dim tblfld As String = ""
            dtbl = dtritems.ToTable

            For i = 0 To dtbl.Rows.Count - 1
                dr = dtbl.Rows(i)
                flditemtype = dr("ReportItemType")
                If flditemtype = "DataField" Then
                    captionalign = dr("CaptionTextAlign")
                    textalign = dr("TextAlign")
                    tblfld = dr("SQLTable") & "." & dr("SQLField")
                    fldname = htSQLTblFlds(tblfld)
                    fldcaption = dr("Caption")
                    If fldcaption = fldname Then
                        fldfriendname = GetFriendlyReportFieldName(repid, fldname)
                    Else
                        fldfriendname = fldcaption
                    End If

                    dtrf.DefaultView.RowFilter = ""
                    dtrf.DefaultView.RowFilter = "Val='" & fldname & "'"
                    If dtrf.DefaultView.ToTable.Rows.Count > 0 Then
                        fldexpr = dtrf.DefaultView.ToTable.Rows(0)("Prop2").ToString
                    End If
                    'Caption    ----------------------------------------------------------------------------------------
                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname & "cap"        '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    Xtextbox = Round(dr("CaptionX") / 96, 2)
                    Ytextbox = Round(dr("CaptionY") / 96, 2)

                    If Decimal.TryParse(dr("CaptionWidth"), Wtextbox) Then
                        'Wtextbox = Round(Wtextbox / 1.3, 2)
                        Wtextbox = Round(Wtextbox / 96, 2)
                    Else
                        'Wtextbox = 72 '72pts = 1 inch
                        Wtextbox = 1.0
                    End If
                    If Decimal.TryParse(dr("CaptionHeight"), Htextbox) Then
                        'Htextbox = Round(Htextbox / 1.3, 2)
                        Htextbox = Round(Htextbox / 96, 2)
                    Else
                        'Htextbox = Round(26 / 1.3, 2) '26px/1.3 = 20 points
                        Htextbox = Round(26 / 96, 2)
                    End If


                    'AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    'AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    'AddElement(textbox, "Height", Htextbox.ToString & "pt")
                    'AddElement(textbox, "Width", Wtextbox.ToString & "pt")

                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", captionalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    'Style
                    fontnam = dr("CaptionFontName").ToString
                    fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                    fontstyl = dr("CaptionFontStyle").ToString
                    forecolr = dr("CaptionForeColor").ToString
                    backcolr = dr("CaptionBackColor").ToString
                    bordercolr = dr("CaptionBorderColor").ToString
                    borderstyle = dr("CaptionBorderStyle").ToString
                    borderwidth = dr("CaptionBorderWidth").ToString
                    underlin = dr("CaptionUnderline").ToString
                    strikeout = dr("CaptionStrikeout").ToString
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    AddElement(textRun, "Value", fldfriendname)  '!!!!!!

                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")

                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")

                    'Field    ----------------------------------------------------------------------------------------

                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname         '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")

                    Xtextbox = Round(dr("DataX") / 96, 2)
                    Ytextbox = Round(dr("DataY") / 96, 2)
                    If Decimal.TryParse(dr("DataWidth"), Wtextbox) Then
                        Wtextbox = Round(Wtextbox / 96, 2)
                    Else
                        Wtextbox = 1.0
                    End If
                    If Decimal.TryParse(dr("DataHeight"), Htextbox) Then
                        Htextbox = Round(Htextbox / 96, 2)
                    Else
                        Htextbox = Round(26 / 96, 2)
                    End If

                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")


                    'If Decimal.TryParse(dr("DataWidth"), Wtextbox) Then
                    '    Wtextbox = Round(Wtextbox / 1.3, 2)
                    'Else
                    '    Wtextbox = 72 '72pts = 1 inch
                    'End If
                    'If Decimal.TryParse(dr("DataHeight"), Htextbox) Then
                    '    Htextbox = Round(Htextbox / 1.3, 2)
                    'Else
                    '    Htextbox = Round(26 / 1.3, 2) '20 pts
                    'End If

                    'AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    'AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    'AddElement(textbox, "Height", Htextbox.ToString & "pt")
                    'AddElement(textbox, "Width", Wtextbox.ToString & "pt")

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    'add expressions
                    If fldexpr = "" Then
                        'AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                        'if numeric field
                        If ColumnTypeIsNumeric(dt.Columns(fldname)) Then
                            AddElement(textRun, "Value", "=Round(Fields!" & fldname & ".Value, 2)")  '!!!!!!
                        Else
                            'not numeric 
                            AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                        End If
                    Else
                        AddElement(textRun, "Value", "=" & fldexpr)  '!!!!!!
                    End If
                    style = AddElement(textRun, "Style", Nothing)
                    'style
                    fontnam = dr("FontName").ToString
                    fontsiz = Round(CInt(dr("FontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                    fontstyl = dr("FontStyle").ToString
                    forecolr = dr("ForeColor").ToString
                    backcolr = dr("BackColor").ToString
                    underlin = dr("Underline").ToString
                    strikeout = dr("Strikeout").ToString
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If
                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If
                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontStyle", fontstyl)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")

                    'border
                    border = AddElement(style, "Border", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If

                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                ElseIf flditemtype = "Label" Then
                    fldval = dr("Caption").ToString
                    fldname = dr("ItemID")
                    textalign = dr("TextAlign")
                    textbox = AddElement(repitems, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = fldname         '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")


                    Xtextbox = Round(dr("X") / 96, 2)
                    Ytextbox = Round(dr("Y") / 96, 2)
                    Wtextbox = Round(dr("Width") / 96, 2)
                    Htextbox = Round(dr("Height") / 96, 2)
                    AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    AddElement(textbox, "Height", Htextbox.ToString & "in")
                    AddElement(textbox, "Width", Wtextbox.ToString & "in")


                    'Xtextbox = Round(dr("X") / 96, 2)
                    'Ytextbox = Round(dr("Y") / 96, 2)
                    'Wtextbox = Round(dr("Width") / 1.3, 2)
                    'Htextbox = Round(dr("Height") / 1.3, 2)
                    'AddElement(textbox, "Top", Ytextbox.ToString & "in")
                    'AddElement(textbox, "Left", Xtextbox.ToString & "in")
                    'AddElement(textbox, "Height", Htextbox.ToString & "pt")
                    'AddElement(textbox, "Width", Wtextbox.ToString & "pt")

                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)

                    'text align
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", textalign)

                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)

                    AddElement(textRun, "Value", fldval)                             '!!!!!!!!

                    style = AddElement(textRun, "Style", Nothing)
                    'Style
                    fontnam = dr("CaptionFontName").ToString
                    fontsiz = Round(CInt(dr("CaptionFontSize").ToString.Replace("px", "")) / 1.3).ToString & "pt"
                    fontstyl = dr("CaptionFontStyle").ToString
                    forecolr = dr("CaptionForeColor").ToString
                    backcolr = dr("CaptionBackColor").ToString
                    bordercolr = dr("CaptionBorderColor").ToString
                    borderstyle = dr("CaptionBorderStyle").ToString
                    borderwidth = dr("CaptionBorderWidth").ToString
                    underlin = dr("CaptionUnderline").ToString
                    strikeout = dr("CaptionStrikeout").ToString
                    If fontstyl.ToLower.Contains("bold") Then
                        fontweight = "Bold"
                    Else
                        fontweight = "Default"
                    End If
                    fontstyl = fontstyl.Replace("Bold", "").Trim.Replace("regular", "Default").Replace("Regular", "Default")
                    If fontstyl = "" Then fontstyl = "Default"
                    If underlin = "True" Then
                        textdecor = "Underline"
                    Else
                        textdecor = "Default"
                    End If

                    If strikeout = "True" Then
                        textdecor = "LineThrough"
                    End If

                    AddElement(style, "FontFamily", fontnam)
                    AddElement(style, "FontWeight", fontweight)
                    AddElement(style, "FontSize", fontsiz)
                    AddElement(style, "Color", forecolr)
                    AddElement(style, "TextDecoration", textdecor)
                    'AddElement(style, "TextAlign", "Left")   'For now... Until we have this field in ourreportitems  & "pt"

                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "BackgroundColor", backcolr)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "TopBorder", Nothing)
                    If borderstyle <> "None" Then
                        If borderwidth = "1" Then
                            bordWidth = "0.25pt"
                        Else
                            bordWidth = (CInt(borderwidth) / 1.3) & "pt"
                        End If
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Top border
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Bottom border
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Left border
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                        'Right border
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", borderstyle)
                        AddElement(border, "Color", bordercolr)
                        AddElement(border, "Width", bordWidth)
                    Else
                        AddElement(border, "Style", borderstyle)
                    End If
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "BottomBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "LeftBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                    'border = AddElement(style, "RightBorder", Nothing)
                    'AddElement(border, "Style", "Solid")
                    'AddElement(border, "Color", "LightGrey")
                    'AddElement(border, "Width", "0.25pt")
                End If
            Next
            'KeepTogether and RepeatWith for rectangle
            AddElement(rectangl, "KeepTogether", "true")
            AddElement(rectangl, "RepeatWith", "Tablix1")

            'End of TablixBody  -----------------------------------------------------------------------------------------------

            'TablixColumnHierarchy element
            Dim tablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim tablixMembers As XmlElement = AddElement(tablixColumnHierarchy, "TablixMembers", Nothing)
            'For i = 0 To n - 1
            AddElement(tablixMembers, "TablixMember", Nothing)
            'Next

            'TablixRowHierarchy element
            Dim tablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim tablixMember As XmlElement
            Dim tablixMemberGrp As XmlElement
            'Dim tablixMemberStat As XmlElement
            Dim group As XmlElement
            Dim groupexpressions As XmlElement
            Dim groupexpression As XmlElement
            'Dim pagebreak As XmlElement
            Dim sortexpressions As XmlElement
            Dim sortexpression As XmlElement
            Dim value As XmlElement
            Dim grpnm As String = String.Empty

            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", Nothing)
            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            AddElement(tablixMember, "KeepTogether", "true")

            'groups
            Dim ovrall As Integer = 0
            Dim dtgr As DataTable = GetDistinctReportGroups(repid)
            If dtgr.Rows.Count > 0 Then
                grpnm = ""

                For i = 0 To dtgr.Rows.Count - 1  'loop for groups not Overall
                    If dtgr.Rows(i)("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dtgr.Rows(i)("GroupField").ToString & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If

                        'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then

                            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                                pagebreak = AddElement(group, "PageBreak", Nothing)
                                AddElement(pagebreak, "BreakLocation", "End")
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")

                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                        End If

                        'if last deepest group
                        If i = dtgr.Rows.Count - 1 OrElse dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then

                            tablixMember = DetailTablixMember(doc, tablixMember, 0, 1)
                            Exit For
                        End If

                    End If
                Next

            Else  'no groups
                'detail
                'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMember, "DataElementName", "Detail_Collection")
                AddElement(tablixMember, "DataElementOutput", "Output")
                'AddElement(tablixMember, "KeepTogether", "true")
                group = AddElement(tablixMember, "Group", Nothing)
                attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = "Table1_Details_Group"
                AddElement(group, "DataElementName", "Detail")
                pagebreak = AddElement(group, "PageBreak", Nothing)
                AddElement(pagebreak, "BreakLocation", "Between")
                Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
                AddElement(tablixMembersNested, "TablixMember", Nothing)
            End If

            'Save XML document in OURFiles
            ''flash to string
            Dim rdlstr As String = String.Empty
            Dim rdlfile As String = String.Empty
            Dim userupl As String = String.Empty
            Dim bydesigner As String = String.Empty
            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            rdlstr = sw.ToString
            Dim er As String = String.Empty
            rep = rdlpath & repid & ".rdl"
            Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                'read rdl from OURFiles
                rdlfile = dv.Table.Rows(0)("Path").ToString  'string with file path
                userupl = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
                bydesigner = dv.Table.Rows(0)("Prop2").ToString   'by designer
            End If
            If userupl.Trim = "" Then
                er = SaveRDLstringInOURFiles(repid, rdlstr)  'just generated
            Else
                Try
                    userupl = userupl.Replace("uploaded by", "").Trim
                    rdlfile = rdlfile.Replace("|", "\")  'in MySql file path does not saved properly !!!!
                    'copy user uploaded earlier rdl
                    Dim userrdl As String = rdlfile.Replace(".rdl", "UpldByUser" & userupl & ".rdl")
                    If File.Exists(userrdl) Then
                        File.Delete(userrdl)
                    End If
                    File.Copy(rdlfile, userrdl)  'save user rdl with new name
                    er = SaveRDLstringInOURFiles(repid, rdlstr, " ", " ")  'just generated but previously was uploaded by user
                Catch ex As Exception
                    er = "ERROR!! " & ex.Message
                End Try
            End If
            Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString

            If er = "Query executed fine." AndAlso myprovider <> "Oracle.ManagedDataAccess.Client" Then
                'delete file from RDLFILES folder if it is not user created rdl!!
                If File.Exists(rep) Then
                    File.Delete(rep)
                    rep = "Report format generated."
                End If
            Else
                doc.Save(rep)
            End If

            'If er = "Query executed fine." Then
            '    'delete file from RDLFILES folder if it is not user created rdl!!
            '    If File.Exists(rep) Then
            '        File.Delete(rep)
            '        rep = "Report format generated."
            '    End If
            'Else
            '    doc.Save(rep)
            'End If
            'TODO Remove it. Save RDL XML document to file temporary for now
            'doc.Save(rep)
            Return rep
        Catch ex As Exception
            Return "ERROR!! " & repid & " - " & ex.Message
        End Try
        Return rep
    End Function
    Public Function CreateRDLReportForXSD(ByVal repid As String, ByVal repttl As String, ByVal rdlpath As String, ByVal mSQL As String, ByVal datatype As String, ByVal orien As String, Optional ByVal PageFtr As String = "", Optional ByRef rdlstrgen As String = "", Optional ByVal myconstr As String = "") As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        Dim rep As String = String.Empty
        Dim n, i, j, k, m, l As Integer
        Dim showall As Boolean = True     'show all fields 
        Dim fldname As String = String.Empty
        Dim fldexpr As String = String.Empty
        Dim HasCombinedFields As Boolean = False

        Try
            Dim dt As DataTable = ReturnDataTblFromOURFiles(repid)
            If dt Is Nothing Then
                dt = ReturnDataTblFromXML(xsdfile)
            End If
            If dt Is Nothing OrElse dt.Columns Is Nothing OrElse dt.Columns.Count = 0 Then
                Return "ERROR!! " & repid & " - nothing to create ..."
            End If
            dt = CorrectDatasetColumns(dt)
            Dim dtrf As DataTable = GetReportFields(repid, True)
            Dim dr As DataRow = Nothing
            Dim val As String = String.Empty

            If dtrf.Rows.Count > 0 Then

                n = dtrf.Rows.Count
                showall = False
                For i = 0 To dtrf.Rows.Count - 1
                    dr = dtrf.Rows(i)
                    val = dr("Val")
                    If val.ToUpper.EndsWith("_COMBINED") Then
                        HasCombinedFields = True
                        Exit For
                    End If
                Next
            Else
                n = dt.Columns.Count
                showall = True
            End If
            ' Create an XML document
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            Dim attr As XmlAttribute = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            Dim myconstring As String
            If myconstr.Trim = "" Then
                myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            Else
                myconstring = myconstr
            End If
            'TODO UserConnStr ?
            'myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'AddElement(connectionProperties, "ConnectString", myconstring)
            AddElement(connectionProperties, "ConnectString", "")
            AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", repid)
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "300")
            'Fields element
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            'For i = 0 To n - 1
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                'If showall Then
                fldname = dt.Columns(i).Caption
                'Else
                '    fldname = dtrf.Rows(i)("Val").ToString
                'End If
                attr.Value = fldname
                AddElement(field, "DataField", fldname)
            Next
            'Add Combined Fields if defined
            If HasCombinedFields Then
                For i = 0 To dtrf.Rows.Count - 1
                    dr = dtrf.Rows(i)
                    val = dr("Val")
                    If val.ToUpper.EndsWith("_COMBINED") Then
                        field = AddElement(fields, "Field", Nothing)
                        attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = val
                        AddElement(field, "DataField", val)
                    End If
                Next
            End If
            'end of DataSources
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If

            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)

            If n > 5 Then   '!!!! TODO depend of columns widths
                'landscape

                If orien = "landscape" Then
                    AddElement(page, "PageWidth", "11in")
                    AddElement(page, "PageHeight", "8.5in")
                Else
                    AddElement(page, "PageWidth", "8.5in")
                    AddElement(page, "PageHeight", "11in")
                End If

            End If

            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.45in")
            AddElement(pageheader, "PrintOnFirstPage", "false")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            ''execution time
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "ExecutionTime"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "0.1in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.2in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "8in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            Dim npv As String = "Globals!PageNumber" & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", "=" & npv)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "1.5in")
            AddElement(txtbox, "Height", "0.8in")
            AddElement(txtbox, "Width", "6.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ''page number
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "PageNumber"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "7.5in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "3in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "28pt")

            AddElement(reptitle, "Height", "1in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ' Tablix element
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix1"
            AddElement(tablix, "DataSetName", repid)
            AddElement(tablix, "Top", "1.1in")
            AddElement(tablix, "Left", "0.3in")
            AddElement(tablix, "Height", "0.5in")
            AddElement(tablix, "RepeatColumnHeaders", "true")
            AddElement(tablix, "FixedColumnHeaders", "true")
            AddElement(tablix, "RepeatRowHeaders", "true")
            AddElement(tablix, "FixedRowHeaders", "true")

            Dim tablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            Dim ltablex As Integer = 0
            'TablixColumns element
            Dim tablixColumns As XmlElement = AddElement(tablixBody, "TablixColumns", Nothing)
            Dim tablixColumn As XmlElement
            For i = 0 To n - 1
                tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
                AddElement(tablixColumn, "Width", "1.5in")  'Unit.Pixel(dt.Columns(i).Caption.Length).ToString

            Next
            AddElement(tablix, "Width", (n * 1.5).ToString & "in") 'Unit.Pixel(ltablex).ToString
            'TablixRows element
            Dim tablixRows As XmlElement = AddElement(tablixBody, "TablixRows", Nothing)
            Dim tablixCell As XmlElement
            Dim cellContents As XmlElement
            Dim textbox As XmlElement
            Dim paragraphs As XmlElement
            Dim paragraph As XmlElement
            Dim textRuns As XmlElement
            Dim textRun As XmlElement
            Dim style As XmlElement
            Dim border As XmlElement
            Dim name As String = String.Empty
            Dim colspan As XmlElement
            Dim fldval As String = String.Empty
            Dim fldfriendname As String = String.Empty

            'first tablixRow
            Dim tablixRow As XmlElement = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.25in")
            Dim tablixCells As XmlElement = AddElement(tablixRow, "TablixCells", Nothing)

            'add tablixRows for the group names  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim dtgrp As DataTable = GetReportGroups(repid)
            Dim hdrgr As String = String.Empty
            'Dim ovall As Integer = 0
            'If dtgrp.Rows(0)("GroupField").ToString = "Overall" Then ovall = 1
            For j = 0 To dtgrp.Rows.Count - 1
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgrp.Rows(j)("CalcField").ToString)
                If dtgrp.Rows(j)("GroupField") <> "Overall" Then
                    'no duplicates
                    If j = 0 OrElse dtgrp.Rows(j)("GroupField") <> dtgrp.Rows(j - 1)("GroupField") Then
                        grpfrname = GetFriendlyReportGroupName(repid, dtgrp.Rows(j)("GroupField").ToString, dtgrp.Rows(j)("CalcField").ToString)
                        hdrgr = "="" "
                        For k = 0 To j
                            hdrgr = hdrgr & "   "
                        Next
                        hdrgr = hdrgr & grpfrname & "  "" & Fields!" & dtgrp.Rows(j)("GroupField") & ".Value "
                        'add tablexRow for the group friendly name
                        ' TablixCell element (column with group name)
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "GroupHdr" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Left")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", hdrgr)                             '!!!!!!!! 
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "FontStyle", "Italic")
                        AddElement(style, "Color", "White")
                        'AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Left")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        AddElement(style, "BackgroundColor", "DimGray")
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        'add columns and colspan
                        colspan = AddElement(cellContents, "ColSpan", n)
                        For k = 0 To n - 2
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        Next

                        'add tablix row for next with group and last one for columns names
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.25in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    End If
                End If
            Next


            'columns names
            For i = 0 To n - 1
                If showall Then
                    fldname = dt.Columns(i).Caption
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                End If
                'fldname = dtrf.Rows(i)("Val").ToString

                fldfriendname = GetFriendlyReportFieldName(repid, fldname)

                ' TablixCell element (column names)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                AddElement(textbox, "CanGrow", "true")
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname & "h"     '!!!!
                AddElement(textbox, "KeepTogether", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                style = AddElement(paragraph, "Style", Nothing)
                AddElement(style, "TextAlign", "Center")
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                AddElement(textRun, "Value", fldfriendname)  '!!!!!!!!
                style = AddElement(textRun, "Style", Nothing)
                AddElement(style, "TextDecoration", "Underline")
                AddElement(style, "TextAlign", "Center")
                AddElement(style, "FontWeight", "Bold")
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                style = AddElement(textbox, "Style", Nothing)
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
            Next

            'TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.25in")
            tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

            'columns values
            For i = 0 To n - 1
                If showall Then
                    fldname = dt.Columns(i).Caption
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                    fldexpr = dtrf.Rows(i)("Prop2").ToString
                End If

                'fldname = dtrf.Rows(i)("Val").ToString
                'fldexpr = dtrf.Rows(i)("Prop2").ToString

                ' TablixCell element (first cell)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname         '!!!!!!
                'AddElement(textbox, "HideDuplicates", repid)
                AddElement(textbox, "KeepTogether", "true")
                AddElement(textbox, "CanGrow", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                'add expressions
                If fldexpr = "" Then
                    'AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                    'if numeric field
                    If ColumnTypeIsNumeric(dt.Columns(fldname)) Then
                        AddElement(textRun, "Value", "=Round(Fields!" & fldname & ".Value,2)")  '!!!!!!
                    Else
                        'not numeric 
                        AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                    End If
                Else
                    AddElement(textRun, "Value", "=" & fldexpr)  '!!!!!!
                End If

                style = AddElement(textRun, "Style", Nothing)
                style = AddElement(textbox, "Style", Nothing)
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")

            Next

            Dim totgrp As String = String.Empty
            Dim totgr As String = String.Empty
            'add groups subtotals rows 
            Dim dtgt As DataTable = dtgrp 'GetReportGroups(repid)
            For i = 0 To dtgt.Rows.Count - 1
                j = dtgt.Rows.Count - 1 - i
                'group statistics title
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgt.Rows(j)("CalcField").ToString)
                If dtgt.Rows(j)("GroupField") = "Overall" Then
                    totgr = "Overall totals of " & fldfrname 'dtgt.Rows(j)("CalcField").ToString
                Else 'not overall
                    totgr = "=""Subtotals of " & fldfrname & " for: "
                    For l = 0 To j
                        If dtgt.Rows(l)("GroupField") <> "Overall" AndAlso dtgt.Rows(j)("GroupField") <> dtgt.Rows(l)("GroupField") Then
                            grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(l)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                            'If l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField") Then
                            If l = 0 OrElse (l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField")) Then
                                totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(l)("GroupField") & ".Value "
                                If l < j Then totgr = totgr & " & "" "
                            End If
                        End If
                    Next
                    grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(j)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(j)("GroupField") & ".Value "
                End If
                'group title stats rows
                m = dtgt.Rows(j)("CntChk") + dtgt.Rows(j)("SumChk") + dtgt.Rows(j)("MaxChk") + dtgt.Rows(j)("MinChk") + dtgt.Rows(j)("AvgChk") + dtgt.Rows(j)("StDevChk") + dtgt.Rows(j)("CntDistChk") + dtgt.Rows(j)("FirstChk") + dtgt.Rows(j)("LastChk")
                If m > 0 Then  'm stats ordered
                    ' Tablix row for group totals name
                    tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                    AddElement(tablixRow, "Height", "0.25In")
                    tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    ' TablixCell element (group subtotal name)
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = dtgt.Rows(j)("GroupField") & "gr" & j.ToString      '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", totgr)        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    'AddElement(style, "TextDecoration", "Underline")
                    AddElement(style, "TextAlign", "Left")
                    AddElement(style, "FontWeight", "Bold")
                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "TopBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "BottomBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "LeftBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "RightBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    'add columns and colspan
                    colspan = AddElement(cellContents, "ColSpan", n)
                    'add tablix row for each stat with columns and colspan
                    For k = 0 To n - 2
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    Next

                    ' Tablix rows for stats
                    If n < 6 Then  'less than 6 columns for stats - add  6 rows
                        For k = 0 To 8  'stats 5
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'add row
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.25In")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"  '!!!!!!!!!!!!!!!!!!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "Last(" & fldval & ")"
                                End If
                                fldval = " & " & fldval
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Count:  """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Sum:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Max:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Min:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Avg:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""StDev:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""CntDist:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""First:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Last:   """ & fldval)
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            Else 'no stats for the column
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.25in")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                                ' TablixCell element (stat subtotal name) - column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "K" & k.ToString & "L" & l.ToString & "J" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                'AddElement(textRun, "Value", " ")   '!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last: ")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            End If
                        Next
                    Else  'at least 8 columns for stats - add 2 rows
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.25in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        For k = 0 To 8  '8 stats names in one row
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then  'stat ordered
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) 
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "sth" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last:")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "sthe" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If

                        'second row with stats values
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.25in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        For k = 0 To 8  '8 stats 
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "=Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "=Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "=Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "=Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "=FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "=FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "=CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "=First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "=Last(" & fldval & ")"
                                End If
                                AddElement(textRun, "Value", fldval)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "stve" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If
                    End If
                End If
            Next

            '******************************
            'End of TablixBody

            'TablixColumnHierarchy element
            Dim tablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim tablixMembers As XmlElement = AddElement(tablixColumnHierarchy, "TablixMembers", Nothing)
            For i = 0 To n - 1
                AddElement(tablixMembers, "TablixMember", Nothing)
            Next

            'TablixRowHierarchy element
            Dim tablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim tablixMember As XmlElement
            Dim tablixMemberGrp As XmlElement
            Dim tablixMemberStat As XmlElement
            Dim group As XmlElement
            Dim groupexpressions As XmlElement
            Dim groupexpression As XmlElement
            Dim pagebreak As XmlElement
            Dim sortexpressions As XmlElement
            Dim sortexpression As XmlElement
            Dim value As XmlElement
            Dim grpnm As String = String.Empty
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", Nothing)
            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            ' AddElement(tablixMember, "KeepWithGroup", "After")
            'AddElement(tablixMember, "RepeatOnNewPage", "true")
            AddElement(tablixMember, "KeepTogether", "true")
            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            'groups
            Dim ovrall As Integer = 0
            Dim dtgr As DataTable = dtgrp  ' GetReportGroups(repid)

            If dtgr.Rows.Count = 0 Then
                '-----------------------------------------  no groups  '--------------------------------------------------------------------------------------
                AddElement(tablixMember, "KeepWithGroup", "After")
                AddElement(tablixMember, "RepeatOnNewPage", "true")
                'detail
                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMember, "DataElementName", "Detail_Collection")
                AddElement(tablixMember, "DataElementOutput", "Output")
                AddElement(tablixMember, "KeepTogether", "true")
                group = AddElement(tablixMember, "Group", Nothing)
                attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = "Table1_Details_Group"
                AddElement(group, "DataElementName", "Detail")
                Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
                AddElement(tablixMembersNested, "TablixMember", Nothing)


            ElseIf dtgr.Rows(0)("GroupField").ToString.Trim = "Overall" AndAlso dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString.Trim = "Overall" Then
                '-----------------------------------------           only overall group --------------------------------------------------------------------------------------

                'loop for groups Overall
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If

                    Else
                        Exit For
                    End If
                Next
                'last deepest group
                tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                'tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                'AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                'AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                tablixMember = DetailTablixMember(doc, tablixMember)



                '----------------------------------    overall and one or more not overall groups --------------------------------------------------------------------------------------

            Else

                'loop for groups Overall
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If
                    Else
                        Exit For
                    End If
                Next

                'loop for groups not Overall
                grpnm = ""
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dtgr.Rows(i)("GroupField").ToString & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If

                        'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then

                            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                                pagebreak = AddElement(group, "PageBreak", Nothing)
                                AddElement(pagebreak, "BreakLocation", "End")
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            'add tablexMember for group name
                            'For j = 0 To i - ovrall
                            'If j = 0 OrElse dtgr.Rows(j)("GroupField").ToString <> dtgr.Rows(j - 1)("GroupField").ToString Then
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            'End If
                            'Next

                            'add one more in the end... 
                            If dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            End If

                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                        End If

                        'if last deepest group
                        If i = dtgr.Rows.Count - 1 Then
                            tablixMember = DetailTablixMember(doc, tablixMember)
                        End If

                        'stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then

                            'row for subtotal name
                            tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            'rows with stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If



                    End If
                Next


            End If

            'Save RDL document in OURFiles
            ''flash to string
            Dim rdlstr As String = String.Empty
            Dim rdlfile As String = String.Empty
            Dim userupl As String = String.Empty
            Dim bydesigner As String = String.Empty
            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            rdlstr = sw.ToString
            rdlstrgen = rdlstr
            Dim er As String = String.Empty
            rep = rdlpath & repid & ".rdl"
            Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                'read rdl from OURFiles
                rdlfile = dv.Table.Rows(0)("Path").ToString  'string with file path
                userupl = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
                bydesigner = dv.Table.Rows(0)("Prop2").ToString   'by designer
            End If
            If userupl.Trim = "" AndAlso bydesigner.Trim = "" Then
                er = SaveRDLstringInOURFiles(repid, rdlstr)  'just generated

            Else
                If userupl.Trim <> "" AndAlso bydesigner.Trim = "" Then
                    Try
                        userupl = userupl.Replace("uploaded by", "").Trim
                        rdlfile = rdlfile.Replace("|", "\")  'in MySql file path does not saved properly !!!!
                        'copy user uploaded earlier rdl
                        Dim userrdl As String = rdlfile.Replace(".rdl", "UpldByUser" & userupl & ".rdl")
                        If File.Exists(userrdl) Then
                            File.Delete(userrdl)
                        End If
                        File.Copy(rdlfile, userrdl)  'save user rdl with new name
                        er = SaveRDLstringInOURFiles(repid, rdlstr, " ", " ")  'just generated but previously was uploaded by user

                    Catch ex As Exception
                        er = "ERROR!! " & ex.Message
                    End Try
                ElseIf userupl.Trim = "" AndAlso bydesigner.Trim = "designer" Then
                    er = "generic report"
                End If

            End If
            Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString

            If er = "Query executed fine." AndAlso myprovider <> "Oracle.ManagedDataAccess.Client" Then
                'delete file from RDLFILES folder if it is not user created rdl!!
                If File.Exists(rep) Then
                    File.Delete(rep)
                    rep = "Report format generated."
                End If
            Else
                doc.Save(rep)
            End If
            'TODO Remove it. Save RDL XML document to file temporary for testing 
            'doc.Save(rep)
            Return rep
        Catch ex As Exception
            Return "ERROR!! " & repid & " - " & ex.Message
        End Try
        Return rep
    End Function
    Public Function SaveRDLstringInOURFiles(ByVal repid As String, ByVal rdlstr As String, Optional ByVal user As String = "", Optional ByVal pathfile As String = "") As String
        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString
        Dim qsql As String = "SELECT ReportId,Type,Prop1 FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'"

        Try
            'If myprovider <> "Oracle.ManagedDataAccess.Client" Then
            '    If rdlstr <> "" Then
            '        If HasRecords(qsql) Then
            '            'Dim qsql As String = "SELECT ReportId,Type,Prop1 FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'"
            '            qsql = "UPDATE OURFiles SET FileText='" & rdlstr & "',UserFile='" & rdlstr & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
            '        Else
            '            qsql = "INSERT INTO OURFiles (ReportId,Type,FileText,UserFile) VALUES ( '" & repid & "','RDL','" & rdlstr & "','" & rdlstr & "')"
            '        End If
            '        er = ExequteSQLquery(qsql)
            '        If er <> "Query executed fine." Then
            '            Return "ERROR!! " & er
            '        End If
            '        If user = "" Then
            '            Return er
            '        End If
            '        qsql = "UPDATE OURFiles SET Comments='" & user & "',Path='" & pathfile.Replace("\", "|") & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
            '        er = ExequteSQLquery(qsql)
            '    End If
            'Else
            '    If Not HasRecords(qsql) Then
            '        Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            '        Dim rdlpath As String = applpath & "RDLFILES\"
            '        Dim repfile As String = rdlpath & repid & ".rdl"
            '        repfile = repfile.Replace("\", "|")
            '        qsql = "INSERT INTO OURFiles (ReportId,Type,Path) VALUES ( '" & repid & "','RDL','" & repfile & "')"
            '        er = ExequteSQLquery(qsql)
            '        If er <> "Query executed fine." Then
            '            Return "ERROR!! " & er
            '        End If
            '    Else
            '        Return "Query executed fine."
            '    End If
            'End If
            ''If rdlstr <> "" Then
            ''    If HasRecords(qsql) Then
            ''    'Dim qsql As String = "SELECT ReportId,Type,Prop1 FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'"
            ''        qsql = "UPDATE OURFiles SET FileText='" & rdlstr & "',UserFile='" & rdlstr & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
            ''    Else
            ''        qsql = "INSERT INTO OURFiles (ReportId,Type,FileText,UserFile) VALUES ( '" & repid & "','RDL','" & rdlstr & "','" & rdlstr & "')"
            ''    End If
            ''    er = ExequteSQLquery(qsql)
            ''    If er <> "Query executed fine." Then
            ''        Return "ERROR!! " & er
            ''    End If
            ''    If user = "" Then
            ''        Return er
            ''    End If
            ''    qsql = "UPDATE OURFiles SET Comments='" & user & "',Path='" & pathfile.Replace("\", "|") & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
            ''    er = ExequteSQLquery(qsql)
            ''End If
            'If myprovider <> "Oracle.ManagedDataAccess.Client" Then
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim rdlpath As String = applpath & "RDLFILES\"
            Dim repfile As String = rdlpath & repid & ".rdl"
            repfile = repfile.Replace("\", "|")
            If rdlstr <> "" Then
                If HasRecords(qsql) Then
                    'Dim qsql As String = "SELECT ReportId,Type,Prop1 FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'"
                    qsql = "UPDATE OURFiles SET FileText='" & rdlstr & "',UserFile='" & rdlstr & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
                Else
                    qsql = "INSERT INTO OURFiles (ReportId,Type,FileText,UserFile) VALUES ( '" & repid & "','RDL','" & rdlstr & "','" & rdlstr & "')"
                End If
                er = ExequteSQLquery(qsql)
                If er <> "Query executed fine." Then
                    ret = "RDL is not saved in OURFiles " & er
                    qsql = "INSERT INTO OURFiles (ReportId,Type,Path) VALUES ( '" & repid & "','RDL','" & repfile & "')"
                    er = ExequteSQLquery(qsql)
                    If er <> "Query executed fine." Then
                        ret = "ERROR!! " & ret & ",  " & er & "  "
                    End If
                End If
                If user = "" Then
                    Return ret
                End If
                qsql = "UPDATE OURFiles SET Comments='" & user & "',Path='" & pathfile.Replace("\", "|") & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
                er = ExequteSQLquery(qsql)
                Return ret & er
            End If
        Catch ex As Exception
            er = "ERROR!! " & ex.Message

            'If Not HasRecords(qsql) Then

            '    qsql = "INSERT INTO OURFiles (ReportId,Type,Path) VALUES ( '" & repid & "','RDL','" & repfile & "')"
            '    er = ExequteSQLquery(qsql)
            '    If er <> "Query executed fine." Then
            '        Return "ERROR!! " & er
            '    End If
            'Else
            '    Return "Query executed fine."
            'End If
        End Try
        Return er
    End Function
    Public Function SaveRDLstreamInOURFiles(ByVal repid As String, ByVal rdlstream As Stream) As String
        Dim er As String = String.Empty
        Dim rdlstr As String = String.Empty
        Try
            Dim tx As New StreamReader(rdlstream)
            rdlstr = tx.ReadToEnd()
            If rdlstr <> "" Then
                Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'"
                If HasRecords(qsql) Then
                    qsql = "UPDATE OURFiles SET FileText='" & rdlstr & "' WHERE ReportId='" & repid & "' AND Type='RDL'"
                Else
                    qsql = "INSERT INTO OURFiles (ReportId,Type,FileText) VALUES ( '" & repid & "','RDL','" & rdlstr & "')"
                End If
                er = ExequteSQLquery(qsql)
            End If
        Catch ex As Exception
            er = ex.Message
        End Try
        Return er
    End Function
    Public Function SaveRPTstringInOURFiles(ByVal repid As String, ByVal rdlstr As String, Optional ByVal user As String = "", Optional ByVal pathfile As String = "") As String
        Dim er As String = String.Empty
        If user = "" OrElse rdlstr = "" Then
            Return er
        End If
        Try

            Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RPT'"
            If Not HasRecords(qsql) Then
                qsql = "INSERT INTO OURFiles (ReportId,Type,Path,Comments) VALUES ( '" & repid & "','RPT','" & pathfile & "','" & user & "')"
                er = ExequteSQLquery(qsql)
            End If
            qsql = "UPDATE OURFiles SET Type='RPT',Comments='" & user & "',Path='" & pathfile.Replace("\", "|") & "' WHERE ReportId='" & repid & "' AND Type='RPT'"
            er = ExequteSQLquery(qsql)
            If er = "Query executed fine." Then
                Return "Uploaded by user"
            End If

            'cannot save rpt in MySql longtext field Why?
            'qsql = "UPDATE OURFiles SET FileText='" & rdlstr & "' WHERE ReportId='" & repid & "' AND Type='RPT'"
            'er = ExequteSQLquery(qsql)
            'If er <> "Query executed fine." Then
            '    Return "ERROR!! " & er
            'End If

        Catch ex As Exception
            er = "ERROR!! " & ex.Message
        End Try
        Return er
    End Function
    Public Function SaveRPTstreamInOURFiles(ByVal repid As String, ByVal rdlstream As Stream) As String
        Dim er As String = String.Empty
        Dim rdlstr As String = String.Empty
        Try
            Dim tx As New StreamReader(rdlstream)
            rdlstr = tx.ReadToEnd()
            If rdlstr <> "" Then
                Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RPT'"
                If Not HasRecords(qsql) Then
                    qsql = "INSERT INTO OURFiles (ReportId,Type) VALUES ( '" & repid & "','RPT')"
                    er = ExequteSQLquery(qsql)
                End If
                qsql = "UPDATE OURFiles SET FileText='" & rdlstr & "' WHERE ReportId='" & repid & "' AND Type='RPT'"
                er = ExequteSQLquery(qsql)
            End If
        Catch ex As Exception
            er = ex.Message
        End Try
        Return er
    End Function
    'Public Function GroupTablixMemberReq(ByVal doc As XmlDocument, ByVal tablixMembers As XmlElement, ByVal tablixMember As XmlElement) As XmlDocument
    '    'group name
    '    If (i = 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> "Overall") OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
    '        grpnm = grpnm & "grp" '& dtgr.Rows(i)("CalcField").ToString
    '    Else
    '        grpnm = dtgr.Rows(i)("GroupField").ToString & "grp" '& dtgr.Rows(i)("CalcField").ToString
    '    End If

    '    'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

    '    If (i = 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> "Overall") OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then




    '        Group = AddElement(tablixMember, "Group", Nothing)
    '        attr = Group.Attributes.Append(doc.CreateAttribute("Name"))
    '        attr.Value = grpnm  'dtgr.Rows(i)("GroupField").ToString & "grp"
    '        groupexpressions = AddElement(Group, "GroupExpressions", Nothing)
    '        groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
    '        sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
    '        sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
    '        Value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
    '        If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
    '            pagebreak = AddElement(Group, "PageBreak", Nothing)
    '            AddElement(pagebreak, "BreakLocation", "End")
    '        End If

    '        tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

    '        'add tablexMember for group name
    '        For j = 0 To i - ovrall
    '            If j = 0 OrElse dtgr.Rows(j)("GroupField").ToString <> dtgr.Rows(j - 1)("GroupField").ToString Then
    '                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
    '                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
    '            End If
    '        Next
    '    End If
    '    Return doc
    'End Function

    Public Function DetailTablixMember(ByVal doc As XmlDocument, ByVal tablixMember As XmlElement, Optional ByVal ngr As Integer = 0, Optional ByVal pgbreak As Integer = 0) As XmlElement
        'detail
        Dim Group As XmlElement
        Dim attr As XmlAttribute
        Dim pagebreak As XmlElement
        'Dim visibility As XmlElement
        'If ngr = 1 Then
        '    visibility = AddElement(tablixMember, "Visibility", Nothing)
        '    AddElement(visibility, "Hidden", "true")
        '    'AddElement(visibility, "ToggleItem", "Toggle" & (ngr - 1).ToString)
        'End If
        AddElement(tablixMember, "DataElementName", "Detail_Collection")
        AddElement(tablixMember, "DataElementOutput", "Output")
        AddElement(tablixMember, "KeepTogether", "true")
        Group = AddElement(tablixMember, "Group", Nothing)
        attr = Group.Attributes.Append(doc.CreateAttribute("Name"))
        attr.Value = "Table1_Details_Group"
        If pgbreak = 1 Then
            pagebreak = AddElement(Group, "PageBreak", Nothing)
            AddElement(pagebreak, "BreakLocation", "End")
        End If
        AddElement(Group, "DataElementName", "Detail")
        Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
        AddElement(tablixMembersNested, "TablixMember", Nothing)
        Return tablixMember
    End Function
    Public Function AddElement(ByVal parent As XmlElement, ByVal name As String, ByVal value As String) As XmlElement
        Dim newelement As XmlElement = parent.OwnerDocument.CreateElement(name, "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition")
        parent.AppendChild(newelement)
        If value IsNot Nothing Then
            newelement.InnerText = value
        End If
        Return newelement
    End Function
    Public Function UpdateSQLinReportInfo(ByVal sql As String, ByVal repid As String) As String
        'NOT IN USE
        sql = cleanSQL(sql)  'will be done by Mike
        If sql <> "SQL not secure" Then
            Dim sqlupd As String
            sqlupd = "UPDATE OURReportInfo SET SQLquerytext='" & sql & "' WHERE (ReportID='" & repid & "')"
            Dim r As String = ExequteSQLquery(sqlupd)
            If r.Trim <> "Query executed fine." Then
                Return ""
            End If
        End If
        Return sql
    End Function
    Public Function ProcessLists(ByVal repid As String, ByVal dt As DataTable, Optional ByRef er As String = "") As DataTable
        Try
            Dim drf As DataTable = Nothing
            Dim dlf As DataTable = GetReportListFields(repid)
            If dlf.Rows.Count = 0 Then
                Return dt
            End If
            Dim i, j, n, m, k As Integer
            Dim lf, rf, cmb, nlf As String
            Dim comb(dt.Rows.Count) As Integer
            Dim c As Boolean

            'Process List for new computed column
            For n = 0 To dlf.Rows.Count - 1                           'loop on list fields
                lf = dlf.Rows(n)("ListFld")                           'field to combine
                nlf = lf
                If dlf.Rows(n)("Prop3").ToString = "1" Then           'computed column
                    nlf = dlf.Rows(n)("Prop1").ToString
                    'add combined column (nlf) into dt if it is not there
                    If nlf <> lf Then
                        c = False
                        For k = 0 To dt.Columns.Count - 1
                            If dt.Columns(k).Caption.Trim.ToUpper = nlf.Trim.ToUpper Then
                                c = True
                            End If
                        Next
                        If c = False Then
                            Dim col As New DataColumn
                            col.DataType = System.Type.GetType("System.String")
                            col.ColumnName = nlf
                            col.Caption = nlf
                            dt.Columns.Add(col)
                        End If
                    End If

                End If
                drf = GetReportRecFldsByListFld(repid, lf)            'get field data for combining
                Dim htRecFld As New Hashtable
                Dim htCombinedFld As New Hashtable
                Dim htCombinedVal As New Hashtable
                Dim RecFldVal As String = ""
                Dim ListFldVal As String = ""
                Dim dr As DataRow
                Dim dtListFld As New DataTable

                ' set up hashtable containing column values for column set
                For i = 0 To drf.Rows.Count - 1
                    rf = drf.Rows(i)("RecFld")
                    htRecFld.Clear()

                    For j = 0 To dt.Rows.Count - 1
                        dr = dt.Rows(j)
                        RecFldVal = dr(rf).ToString
                        If htRecFld(RecFldVal) Is Nothing OrElse htRecFld(RecFldVal).ToString <> "1" Then
                            htRecFld.Add(RecFldVal, "1")
                        End If
                    Next
                    Dim filt As String = ""
                    Dim sCombined As String = ""

                    'combine list field values for each column in column set
                    If htRecFld.Count > 0 Then
                        htCombinedVal.Clear()
                        For Each de As DictionaryEntry In htRecFld
                            filt = rf & " = '" & de.Key.ToString & "'"
                            dtListFld.Clear()
                            dtListFld = dt.Select(filt, lf & " ASC").CopyToDataTable
                            sCombined = ""
                            htCombinedFld.Clear()

                            If dtListFld.Rows.Count > 0 Then
                                For j = 0 To dtListFld.Rows.Count - 1
                                    dr = dtListFld.Rows(j)
                                    ListFldVal = dr(lf).ToString
                                    If htCombinedFld(ListFldVal) Is Nothing Then
                                        htCombinedFld.Add(ListFldVal, 1)
                                        sCombined = IIf(sCombined = "", ListFldVal, sCombined & "|" & ListFldVal)
                                    End If
                                Next
                                If htCombinedFld.Count > 0 Then
                                    htCombinedVal.Add(de.Key.ToString, sCombined)
                                End If
                            End If
                        Next
                        'put combined fields in datatable
                        For j = 0 To dt.Rows.Count - 1
                            dr = dt.Rows(j)
                            RecFldVal = dr(rf).ToString
                            If RecFldVal <> "" AndAlso htCombinedVal(RecFldVal) IsNot Nothing Then
                                dr(nlf) = htCombinedVal(RecFldVal).ToString
                            End If
                        Next
                    End If

                Next
                'For i = 0 To dt.Rows.Count - 1                        'loop on report records
                '    For j = 0 To dt.Rows.Count - 1
                '        'checking if record should be combined
                '        c = True
                '        If i <> j Then
                '            For m = 0 To drf.Rows.Count - 1                   'loop on recfields record
                '                rf = drf.Rows(m)("RecFld")
                '                If Not (dt.Rows(i)(rf).ToString = dt.Rows(j)(rf).ToString) Then
                '                    c = False
                '                    comb(j) = 0 'not to combine
                '                End If
                '            Next
                '            'record's field to combine
                '            If c = True Then
                '                comb(j) = 1   'combine the field lf values on the i and j rows
                '                If dt.Rows(i)(nlf).ToString.IndexOf(dt.Rows(j)(lf).ToString) < 0 Then
                '                    cmb = dt.Rows(i)(nlf).ToString & " | " & dt.Rows(j)(lf).ToString '& " | "
                '                    dt.Rows(i)(nlf) = cmb
                '                    dt.Rows(j)(nlf) = cmb
                '                End If
                '            End If
                '        End If
                '    Next


                '    'At that point dt.Rows(i)(lf) combined all what needed
                '    cmb = dt.Rows(i)(nlf).ToString
                '    For j = 0 To dt.Rows.Count - 1
                '        If comb(j) = 1 Then
                '            dt.Rows(j)(nlf) = cmb
                '        End If
                '    Next
                'Next
            Next
            'skip duplicates
            'dt = dt.DefaultView.ToTable(True)
            dt = dt.DefaultView.ToTable
        Catch ex As Exception
            er = ex.Message
        End Try
        Return dt
    End Function
    Public Function SkipDuplicates(ByVal dt As DataTable, ByRef Optional err As String = "") As DataTable
        Dim newdt As DataTable = dt
        Try
            newdt = dt.DefaultView.ToTable(True)
        Catch ex As Exception
            err = ex.Message
            Return dt
        End Try
        Return newdt
    End Function
    Public Function UpdateXSDandRDL(ByVal repid As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "", Optional ByRef repfile As String = "") As String
        Dim ret As String = String.Empty
        Dim ErrorLog As String = String.Empty
        Dim bCreatedByDesigner As Boolean = (repfile = "designer")
        Try
            'Create/update XSD 

            ret = CreateXSD(repid, userconstr, userconprv)
            If Not ret.Contains("ERROR!!") Then
                'Create/update RDL report
                Dim dri As DataTable = GetReportInfo(repid)
                If dri Is Nothing OrElse dri.Rows.Count = 0 Then
                    Return "Report is not found"
                    Exit Function
                End If
                Dim sqlquerytext As String = dri.Rows(0)("SQLquerytext")
                'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                Dim ReportTemplate As String = "Tabular"

                If Not IsDBNull(dri.Rows(0)("Param0type")) AndAlso dri.Rows(0)("Param0type") <> "" Then
                    ReportTemplate = dri.Rows(0)("Param0type").ToString
                End If

                'Dim xsdpath As String = applpath & "XSDFILES\"
                ''create xsd
                'Dim dtx As DataTable = mRecords(sqlquerytext).Table
                'ret = ret & " </br> " & CreateXSDForDataTable(dtx, repid, xsdpath)
                Dim rdlpath As String = applpath & "RDLFILES\"
                repfile = rdlpath & repid & ".rdl"
                Dim repfilecopy As String = rdlpath & repid & "Copy.rdl"
                Dim pageftr As String = dri.Rows(0)("Comments").ToString
                Dim orien As String = dri.Rows(0)("Param9type").ToString
                'if rdl file exist make the copy
                If File.Exists(repfile) Then
                    If File.Exists(repfilecopy) Then
                        File.Delete(repfilecopy)
                    End If
                    ret = ret & " </br> " & "File " & repid & ".rdl" & " has been copied to the file " & repid & "Copy.rdl"
                    File.Copy(repfile, repfilecopy)
                End If
                Dim repttl = dri.Rows(0)("ReportTtl").ToString
                If bCreatedByDesigner OrElse ReportCreatedByDesigner(repid) Then
                    If ReportTemplate.ToUpper = "FREEFORM" Then
                        ErrorLog = CreateFreeFormReportForXSDByDesigner(repid, repttl, rdlpath, sqlquerytext, "sql", orien, pageftr)
                    Else
                        ErrorLog = CreateTabularReportForXSDByDesigner(repid, repttl, rdlpath, sqlquerytext, "sql", orien, pageftr)
                    End If
                Else
                    ErrorLog = CreateRDLReportForXSD(repid, repttl, rdlpath, sqlquerytext, "sql", orien, pageftr)
                End If

                If ErrorLog.StartsWith("ERROR!!") Then
                    ret = "ERROR!! " & ret & " </br> " & ErrorLog
                ElseIf ErrorLog = "" Then
                    ret = ret & " </br> " & "File " & repid & ".rdl" & " has been saved."
                Else
                    ret = ret & " </br> " & "File " & repid & ".rdl" & " has been updated"
                End If
            End If

        Catch ex As Exception
            Return ret = ret & " </br> " & ex.Message
            Exit Function
        End Try
        Return ret
    End Function
    Public Function CopyXSDandRDL(ByVal rep As String, ByVal repnew As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim i As Integer
        Try
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim xsdpath As String = applpath & "XSDFILES\"
            Dim xsdfile As String = xsdpath & rep & ".xsd"
            Dim xsdfilenew As String = xsdpath & repnew & ".xsd"
            If File.Exists(xsdfile) Then
                If File.Exists(xsdfilenew) Then
                    File.Delete(xsdfilenew)
                End If
                ret = ret & " " & "File " & repid & ".xsd" & " has been copied to the file " & repid & ".xsd"
                File.Copy(xsdfile, xsdfilenew)
            End If
            Dim rdlpath As String = applpath & "RDLFILES\"
            Dim repfile As String = rdlpath & rep & ".rdl"
            Dim repfilenew As String = rdlpath & repnew & ".rdl"
            If File.Exists(repfile) Then
                If File.Exists(repfilenew) Then
                    File.Delete(repfilenew)
                End If
                ret = ret & " " & "File " & repid & ".rdl" & " has been copied to the file " & repid & ".rdl"
                File.Copy(repfile, repfilenew)
            End If
            'OURFiles
            Dim sqlq As String = "SELECT * FROM OURFiles WHERE (ReportId='" & rep & "')"
            Dim dt As DataTable = mRecords(sqlq).Table  'from OUR db
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportId") = repnew
                Dim rdlstr As String = dt.Rows(i)("FileText").ToString
                rdlstr = rdlstr.Replace(rep, repnew)
                'remove connection string
                Dim s1 As Integer = rdlstr.IndexOf("<ConnectString>")
                If s1 > 0 Then
                    Dim s2 As Integer = rdlstr.IndexOf("</ConnectString>")
                    rdlstr = rdlstr.Substring(0, s1) & "<ConnectString>" & rdlstr.Substring(s2)
                End If
                dt.Rows(i)("FileText") = rdlstr
            Next
            Dim sResult As String = AddDataTableIntoDBTable(dt, "OURFiles")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                Return ret = ret & " " & sResult
            End If
        Catch ex As Exception
            Return ret = ret & " </br> " & ex.Message
            Exit Function
        End Try
        Return ret
    End Function
    Public Function CreateXSD(ByVal repid As String, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim n As Integer
        Dim i As Integer
        Try
            Dim dri As DataTable = GetReportInfo(repid)
            If dri Is Nothing OrElse dri.Rows.Count = 0 Then
                Return "Report is not found"
                Exit Function
            End If
            Dim sqlquerytext As String = dri.Rows(0)("SQLquerytext")
            Dim sqltype As String = dri.Rows(0)("ReportAttributes")
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim xsdpath As String = applpath & "XSDFILES\"
            Dim dv2, dv3 As DataView
            Dim ParamNames() As String
            Dim ParamTypes() As String
            Dim ParamValues() As String
            If sqltype = "sql" OrElse sqlquerytext.ToUpper.StartsWith("SELECT ") Then
                Dim err As String = ""
                'add TOP 1 to speed up 
                If userconprv = "MySql.Data.MySqlClient" Then
                    sqlquerytext = sqlquerytext & " LIMIT 1;"
                ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                    sqlquerytext = sqlquerytext & " LIMIT 1;"
                ElseIf userconprv = "Sqlite" Then  'Sqlite
                    sqlquerytext = sqlquerytext & " LIMIT 1;"
                ElseIf userconprv <> "Oracle.ManagedDataAccess.Client" Then
                    sqlquerytext = "SELECT TOP 1 " & sqlquerytext.Substring(7)
                End If

                Dim indx As Integer = sqlquerytext.ToUpper.IndexOf("WHERE ")
                If indx <> -1 Then
                    sqlquerytext = sqlquerytext.Substring(0, indx)
                End If
                err = ""
                sqlquerytext = ConvertSQLSyntaxFromOURdbToUserDB(sqlquerytext, userconprv, err)
                err = ""
                dv3 = mRecords(sqlquerytext, err, userconstr, userconprv)
                If dv3 Is Nothing OrElse dv3.Table Is Nothing OrElse err <> "" Then  'OrElse dv3.Count = 0 
                    sqlquerytext = MakeSQLQueryFromDB(repid, userconstr, userconprv)
                    'add TOP 1 to speed up 
                    If userconprv = "MySql.Data.MySqlClient" Then
                        sqlquerytext = sqlquerytext & " LIMIT 1;"
                    ElseIf userconprv = "Npgsql" Then  'PostgreSQL  Npgsql
                        sqlquerytext = sqlquerytext & " LIMIT 1;"
                    ElseIf userconprv = "Sqlite" Then  'Sqlite
                        sqlquerytext = sqlquerytext & " LIMIT 1;"
                    ElseIf userconprv <> "Oracle.ManagedDataAccess.Client" Then
                        sqlquerytext = "SELECT TOP 1 " & sqlquerytext.Substring(7)
                    End If
                    err = ""
                    sqlquerytext = ConvertSQLSyntaxFromOURdbToUserDB(sqlquerytext, userconprv, err)
                    err = ""
                    dv3 = mRecords(sqlquerytext, err, userconstr, userconprv)

                    Dim userdb As String = userconstr
                    If userconprv <> "Oracle.ManagedDataAccess.Client" Then
                        If userdb.ToUpper.IndexOf("USER ID") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("USER ID")).Trim
                    Else
                        If userdb.ToUpper.IndexOf("PASSWORD") > 0 Then userdb = userdb.Substring(0, userdb.ToUpper.IndexOf("PASSWORD")).Trim
                    End If
                    If dv3 Is Nothing OrElse dv3.Table Is Nothing OrElse err <> "" Then
                        ret = ExequteSQLquery("UPDATE OurReportInfo SET Param1type='1' WHERE ReportID='" & repid & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'")
                        Return "ERROR!! " & err
                    Else
                        ret = ExequteSQLquery("UPDATE OurReportInfo SET Param1type='0' WHERE ReportID='" & repid & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'")
                    End If

                    'If dv3 Is Nothing OrElse dv3.Table Is Nothing Then  'OrElse dv3.Count = 0
                    '    Return "ERROR!! " & err
                    'End If
                End If
                'dv3 = CorrectColumnFromDateToString(dv3.Table, err).DefaultView
                'ConvertMySqlTable
                Dim dt3 As New DataTable
                dt3 = dv3.Table
                dv3 = ConvertMySqlTable(dt3, err).DefaultView
                Dim dts As New DataTable
                dts = dv3.Table
                dts = MakeDTColumnsNamesCLScompliant(dts, userconprv, ret)
                dv3 = dts.DefaultView

                'If userconprv = "MySql.Data.MySqlClient" Then
                '    Dim col As DataColumn
                '    For j = 0 To dv3.Table.Columns.Count - 1
                '        col = New DataColumn
                '        col.ColumnName = dv3.Table.Columns(j).Caption
                '        'col.ColumnName = dt3.Columns(j).Caption.Replace("_", " ").Trim.Replace(" ", "_").Trim
                '        If dv3.Table.Columns(j).DataType.FullName = "MySql.Data.Types.MySqlDateTime" Then
                '            col.DataType = System.Type.GetType("System.String")
                '        Else
                '            col.DataType = System.Type.GetType(dt3.Columns(j).DataType.FullName)
                '        End If
                '        dtb.Columns.Add(col)
                '    Next
                'End If

                If err <> "" Then
                    ret = "ERROR!! " & err
                End If
            Else 'If sqltype = "sp" Then
                'Parameters (drop-downs, data for params to sp) in OUR database
                dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
                n = dv2.Table.Rows.Count   'how many parameters (drop-downs)
                ReDim ParamNames(n)
                ReDim ParamTypes(n)
                ReDim ParamValues(n)
                For i = 0 To dv2.Table.Rows.Count - 1
                    ParamNames(i) = dv2.Table.Rows(i)("DropDownName")
                    ParamTypes(i) = dv2.Table.Rows(i)("DropDownFieldType")
                    If (ParamTypes(i) = "nvarchar" Or ParamTypes(i) = "datetime") Then
                        ParamValues(i) = "0"
                    Else
                        ParamValues(i) = 0
                    End If
                Next
                If userconprv.StartsWith("InterSystems.Data.") AndAlso sqlquerytext.Contains("||") Then
                    Dim cls As String = Piece(sqlquerytext, "||", 1)
                    Dim sp As String = Piece(sqlquerytext, "||", 2)
                    cls = Piece(cls, ".", 1, Pieces(cls, ".") - 1).Replace(".", "_")
                    sqlquerytext = cls & "." & sp
                End If
                dv3 = mRecordsFromSP(sqlquerytext, n, ParamNames, ParamTypes, ParamValues, userconstr, userconprv)  'Data for report from SP
                Dim dts As New DataTable
                dts = dv3.Table
                dts = MakeDTColumnsNamesCLScompliant(dts, userconprv, ret)
                dv3 = dts.DefaultView
            End If
            If dv3 IsNot Nothing Then
                Dim ErrorLog As String = CreateXSDForDataTable(dv3.Table, repid, xsdpath)
                If ErrorLog.StartsWith("ERROR!!") Then
                    ret = ret & " </br> " & ErrorLog
                Else
                    ret = ret & " </br> " & "XSD file " & repid & ".xsd" & " has been updated"
                End If
            End If
        Catch ex As Exception
            ret = "ERROR!!" & ret = ret & " </br> " & ex.Message
        End Try
        Return ret
    End Function
    Public Function CopyReport(ByVal rep As String, ByVal repttl As String, ByVal repnew As String, ByVal repnewttl As String, ByVal logon As String, ByRef bError As Boolean, Optional ByVal userconstr As String = "", Optional ByVal userconprv As String = "") As String
        Dim ret As String = String.Empty
        Dim sResult As String = String.Empty
        Dim bCreatedByDesigner As Boolean = False
        Dim sSql As String = String.Empty

        Try
            'OURReportInfo
            Dim reptype As String = String.Empty
            Dim dri As DataTable = GetReportInfo(rep)
            If dri.Rows.Count = 0 Then
                ret = "Cannot copy. No report " & rep & " found."
                bError = True
                Return ret
            Else
                'check if original report was created with designer
                If FileInOURFiles(rep) Then
                    sSql = "SELECT Prop2 From OURFiles WHERE ReportId ='" & rep & "' AND Type = 'RDL'"
                    Dim dv As DataView = mRecords(sSql)
                    If dv.Count <> 0 AndAlso dv.Table IsNot Nothing AndAlso dv.Table.Rows.Count > 0 Then
                        Dim dtbl As DataTable = dv.Table
                        If Not IsDBNull(dtbl.Rows(0)("Prop2")) AndAlso dtbl.Rows(0)("Prop2").ToString = "designer" Then
                            bCreatedByDesigner = True
                        End If
                    End If
                End If

                'OURReportInfo
                dri.Rows(0)("ReportId") = repnew
                dri.Rows(0)("ReportTtl") = repnewttl
                dri.Rows(0)("ReportName") = repttl   'title of the original report
                reptype = dri.Rows(0)("ReportType")
                'add row into OURReportInfo
                dri.Rows(0)("Param7type") = "user"
                ret = AddDataTableIntoDBTable(dri, "OURReportInfo")
                If ret <> "Query executed fine." Then
                    bError = True
                    Return ret
                End If
            End If
            Dim mysql As String = String.Empty
            Dim dt As DataTable

            'OURPermissions
            mysql = "SELECT * FROM OURPermissions WHERE (PARAM1='" & rep & "' AND APPLICATION='InteractiveReporting' AND NetId='" & logon & "')"
            dt = mRecords(mysql).Table  'from OUR db
            Dim err As String = ""
            dt = ConvertMySqlTable(dt, err)
            If err <> "" Then
                bError = True
                Return err
            End If
            Dim fldDate As Date
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("PARAM1") = repnew
                dt.Rows(i)("PARAM2") = repnewttl
                If IsDBNull(dt.Rows(i)("OpenFrom")) OrElse dt.Rows(i)("OpenFrom").ToString = "0000-00-00 00:00:00" Then
                    dt.Rows(i)("OpenFrom") = DateToString(Now)
                Else
                    fldDate = CDate(dt.Rows(i)("OpenFrom"))
                    dt.Rows(i)("OpenFrom") = DateToString(fldDate)
                End If
                If IsDBNull(dt.Rows(i)("OpenTo")) OrElse dt.Rows(i)("OpenTo").ToString() = "0000-00-00 00:00:00" Then
                    dt.Rows(i)("OpenTo") = DateToString(CDate(DateAndTime.DateAdd(DateInterval.Day, 3, Now())))
                Else
                    fldDate = CDate(dt.Rows(i)("OpenTo"))
                    dt.Rows(i)("OpenTo") = DateToString(fldDate)
                End If
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURPermissions")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportGroups
            mysql = "SELECT * FROM OURReportGroups WHERE (ReportId='" & rep & "') ORDER BY Indx"
            dt = mRecords(mysql).Table  'from OUR db
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportId") = repnew
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURReportGroups")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportView
            dt = GetReportView(rep)
            'converts date fields to string type
            dt = ConvertMySqlTable(dt, err)
            If err <> "" Then
                bError = True
                Return err
            End If
            If dt.Rows.Count = 1 Then
                dt.Rows(0)("ReportID") = repnew
                dt.Rows(0)("ReportTitle") = repnewttl
                dt.Rows(0)("DateCreated") = DateToString(Now)
                dt.Rows(0)("LastUpdate") = DateToString(Now)
            End If
            sResult = AddDataTableIntoDBTable(dt, "OURReportView")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportItems
            Dim dr As DataRow

            dt = GetReportItems(rep)
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportID") = repnew
                'copy images if defined
                If dt.Rows(i)("ReportItemType") = "Image" Then
                    dr = dt.Rows(i)
                    CopyReportImage(rep, repnew, dr)
                End If
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURReportItems")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportFormat
            mysql = "SELECT * FROM OURReportFormat WHERE (ReportId='" & rep & "') ORDER BY Indx"
            dt = mRecords(mysql).Table  'from OUR db
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportId") = repnew
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURReportFormat")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportLists
            mysql = "SELECT * FROM OURReportLists WHERE (ReportId='" & rep & "') ORDER BY Indx"
            dt = mRecords(mysql).Table  'from OUR db
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportId") = repnew
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURReportLists")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportSQLquery
            mysql = "SELECT * FROM OURReportSQLquery WHERE (ReportId='" & rep & "') ORDER BY Indx"
            dt = mRecords(mysql).Table  'from OUR db
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportId") = repnew
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURReportSQLquery")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            'OURReportShow
            mysql = "SELECT * FROM OURReportShow WHERE (ReportId='" & rep & "') ORDER BY Indx"
            dt = mRecords(mysql).Table  'from OUR db
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("ReportId") = repnew
            Next
            sResult = AddDataTableIntoDBTable(dt, "OURReportShow")
            If sResult <> "" AndAlso sResult <> "Query executed fine." Then
                bError = True
                Return sResult
            End If

            ''OURReportChildren
            'mysql = "SELECT * FROM OURReportChildren WHERE (ReportId='" & rep & "')"
            'dt = mRecords(mysql).Table  'from OUR db
            'For i = 0 To dt.Rows.Count - 1
            '    dt.Rows(i)("ReportId") = repnew
            'Next
            'sResult = AddDataTableIntoDBTable(dt, "OURReportChildren")
            'If sResult <> "" AndAlso sResult <> "Query executed fine." Then
            '    bError = True
            '    Return sResult
            'End If

            'for super user copy xsd and rdl files
            Dim userrol As String = UserRole(logon, err)
            If userrol = "super" Then
                CopyXSDandRDL(rep, repnew, userconstr, userconprv)
            Else
                If bCreatedByDesigner Then
                    Dim sRepFile As String = "designer"
                    ret = UpdateXSDandRDL(repnew, userconstr, userconprv, sRepFile)
                Else
                ret = UpdateXSDandRDL(repnew, userconstr, userconprv)
            End If

            End If

            Return ret.Replace("Query executed fine.", "")
        Catch ex As Exception
            bError = True
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function CreateGraphRDLForDTValue(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal fn As String, ByVal primX As String, ByVal secX As String, ByVal valueY As String, ByVal graphtype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "") As String
        Dim ret As String = String.Empty
        Dim graf As String = String.Empty
        Dim indx As String = String.Empty
        Dim i As Integer
        Try
            dt = CorrectDatasetColumns(dt)
            Dim grpfld As String = primX
            Dim grpfld2 As String = secX
            Dim calcfld As String = valueY
            Dim hdrfld As String = fn & " of " & calcfld & " by " & grpfld
            If grpfld <> grpfld2 Then
                hdrfld = hdrfld & ", " & grpfld2
            End If
            'calculate dynamic width and height
            Dim wdth As Integer = 0
            Dim fldnames(0) As String
            fldnames(0) = grpfld
            Dim gr1distcnt As Integer = dt.DefaultView.ToTable(1, fldnames).Rows.Count
            wdth = gr1distcnt / 10
            'If grpfld = grpfld2 Then
            '    wdth = gr1distcnt / 10
            'Else
            '    fldnames(0) = grpfld2
            '    Dim gr2distcnt As Integer = dt.DefaultView.ToTable(1, fldnames).Rows.Count
            '    wdth = gr1distcnt * gr2distcnt / 100
            'End If
            If wdth < 10 Then wdth = 10

            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            'If graphtype = "pie" Then
            '    xmlData = "<Report MustUnderstand='df' xmlns='http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition' xmlns:rd='http://schemas.microsoft.com/SQLServer/reporting/reportdesigner' xmlns:df='http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily'><df:DefaultFontFamily>Segoe UI</df:DefaultFontFamily>"
            'End If
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            Dim attr As XmlAttribute '= report.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = repid
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            attr = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "DataSource1"
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            AddElement(connectionProperties, "ConnectString", "")
            'AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext")
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", "DataSource1")
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "30000")
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = dt.Columns(i).Caption
                AddElement(field, "DataField", dt.Columns(i).Caption)
            Next
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If
            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)
            If orien = "landscape" Then
                AddElement(page, "PageWidth", "11in")
                AddElement(page, "PageHeight", "8.5in")
            Else
                AddElement(page, "PageWidth", "8.5in")
                AddElement(page, "PageHeight", "11in")
            End If
            'Page header
            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.2in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.2in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "1in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "1.5in")
            AddElement(txtbox, "Height", "0.8in")
            AddElement(txtbox, "Width", "8.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'doc report body
            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "8in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "16pt")
            AddElement(reptitle, "Height", "0.5in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'subtitle
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", params)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "10pt")
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'Chart
            Dim repgraph As XmlElement = AddElement(reportItems, "Chart", Nothing)
            attr = repgraph.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Chart1"
            'ChartCategoryHierarchy
            Dim chartcathierarchy As XmlElement = AddElement(repgraph, "ChartCategoryHierarchy", Nothing)
            Dim chartmembrs As XmlElement = AddElement(chartcathierarchy, "ChartMembers", Nothing)
            Dim chartmembr As XmlElement = AddElement(chartmembrs, "ChartMember", Nothing)
            Dim grp As XmlElement = AddElement(chartmembr, "Group", Nothing)
            attr = grp.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Chart1_CategoryGroup"
            'GroupExpressions
            Dim grpexprs As XmlElement = AddElement(grp, "GroupExpressions", Nothing)
            Dim grpexpr As XmlElement = AddElement(grpexprs, "GroupExpression", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            Dim sortexprs As XmlElement = AddElement(chartmembr, "SortExpressions", Nothing)
            Dim sortexpr As XmlElement = AddElement(sortexprs, "SortExpression", Nothing)
            AddElement(sortexpr, "Value", "=Fields!" & grpfld & ".Value")                                        '!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim lbl As XmlElement = AddElement(chartmembr, "Label", "=Fields!" & grpfld & ".Value")              '!!!!!!!!!!!!!!!!!!!!!!!!!!
            'ChartSeriesHierarchy
            Dim chartserhierarchy As XmlElement = AddElement(repgraph, "ChartSeriesHierarchy", Nothing)
            Dim chartsermembrs As XmlElement = AddElement(chartserhierarchy, "ChartMembers", Nothing)
            Dim chartsermembr As XmlElement = AddElement(chartsermembrs, "ChartMember", Nothing)
            Dim grpser As XmlElement = AddElement(chartsermembr, "Group", Nothing)
            attr = grpser.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Chart1_SeriesGroup"
            'GroupExpressions
            Dim grpserexprs As XmlElement = AddElement(grpser, "GroupExpressions", Nothing)
            'Dim grpserexpr As XmlElement = AddElement(grpserexprs, "GroupExpression", "=Fields!" & calcfld & ".Value")
            Dim grpserexpr As XmlElement = AddElement(grpserexprs, "GroupExpression", "=Fields!" & grpfld2 & ".Value")  '!!!!!!!!!!!!!!!!!!!!
            Dim sortserexprs As XmlElement = AddElement(chartsermembr, "SortExpressions", Nothing)
            Dim sortserexpr As XmlElement = AddElement(sortserexprs, "SortExpression", Nothing)
            AddElement(sortserexpr, "Value", "=Fields!" & grpfld2 & ".Value")                                         '!!!!!
            'Dim lblser As XmlElement = AddElement(chartsermembr, "Label", "=Fields!" & calcfld & ".Value")
            Dim lblser As XmlElement = AddElement(chartsermembr, "Label", "=Fields!" & grpfld2 & ".Value")            '!!!!!
            'ChartData
            Dim chartdata As XmlElement = AddElement(repgraph, "ChartData", Nothing)
            'ChartSeriesCollection
            Dim chartsercols As XmlElement = AddElement(chartdata, "ChartSeriesCollection", Nothing)
            Dim chartsers As XmlElement = AddElement(chartsercols, "ChartSeries", Nothing)
            attr = chartsers.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = calcfld
            If graphtype = "pie" Then
                AddElement(chartsers, "Type", "Shape")
            ElseIf graphtype = "line" Then
                AddElement(chartsers, "Type", "Line")
                AddElement(chartsers, "Subtype", "Smooth")
            End If
            'ChartDataPoints
            Dim chartdatapnts As XmlElement = AddElement(chartsers, "ChartDataPoints", Nothing)
            Dim chartdatapnt As XmlElement = AddElement(chartdatapnts, "ChartDataPoint", Nothing)
            'ChartDataPointValues
            Dim chartdatapntvals As XmlElement = AddElement(chartdatapnt, "ChartDataPointValues", Nothing)
            'Dim yval As String = fn & "(" & "Fields!" & calcfld & ".Value)"                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'AddElement(chartdatapntvals, "Y", "=" & yval)
            Dim yval As String = "Round(Fields!" & calcfld & ".Value,2)"                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(chartdatapntvals, "Y", "=" & yval)

            'ChartDataLabel
            Dim chartdatalbl As XmlElement = AddElement(chartdatapnt, "ChartDataLabel", Nothing)
            AddElement(chartdatalbl, "Style", Nothing)
            'AddElement(chartdatalbl, "UseValueAsLabel", "true")
            'AddElement(chartdatalbl, "Visible", "true")

            Dim ttp As String = yval & " & "" in group "" & " & "Fields!" & grpfld & ".Value" & " & ""-"" & " & "Fields!" & grpfld2 & ".Value"             '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11

            'Dim ttp As String = yval & " & "" for item "" & " & "Fields!" & grpfld & ".Value" '& " & ""-"" & " & "Fields!" & grpfld2 & ".Value"             '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11

            Dim ToolTip As XmlElement = AddElement(chartdatapnt, "ToolTip", "=" & ttp)

            Dim chartmarker As XmlElement = AddElement(chartdatapnt, "ChartMarker", Nothing)
            AddElement(chartmarker, "Style", Nothing)
            'DataElementOutput
            AddElement(chartdatapnt, "DataElementOutput", "Output")
            AddElement(chartsers, "Style", Nothing)
            'ChartEmptyPoints
            Dim chartemptypnts As XmlElement = AddElement(chartsers, "ChartEmptyPoints", Nothing)
            AddElement(chartemptypnts, "Style", Nothing)
            Dim chartemptylbl As XmlElement = AddElement(chartemptypnts, "ChartDataLabel", Nothing)
            AddElement(chartemptylbl, "Style", Nothing)
            Dim chartemptymarker As XmlElement = AddElement(chartemptypnts, "ChartMarker", Nothing)
            AddElement(chartemptymarker, "Style", Nothing)
            'ValueAxisName
            AddElement(chartsers, "ValueAxisName", "Primary")
            AddElement(chartsers, "CategoryAxisName", "Primary")
            'ChartSmartLabels
            Dim chartsmartlbl As XmlElement = AddElement(chartsers, "ChartSmartLabel", Nothing)
            AddElement(chartsmartlbl, "CalloutLineColor", "Black")
            AddElement(chartsmartlbl, "MinMovingDistance", "0pt")

            'ChartAreas
            Dim chartareas As XmlElement = AddElement(repgraph, "ChartAreas", Nothing)
            'ChartArea
            Dim chartarea As XmlElement = AddElement(chartareas, "ChartArea", Nothing)
            attr = chartarea.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Default"

            ''position does not work
            'Dim ChartElementPosition As XmlElement = AddElement(chartarea, "ChartElementPosition", Nothing)
            'AddElement(ChartElementPosition, "Width", "100")
            'Dim ChartInnerPlotPosition As XmlElement = AddElement(chartarea, "ChartInnerPlotPosition", Nothing)
            'AddElement(ChartInnerPlotPosition, "Width", "100")


            'ChartCategoryAxes
            Dim chartcataxes As XmlElement = AddElement(chartarea, "ChartCategoryAxes", Nothing)
            'ChartAxis primary
            Dim chartaxis As XmlElement = AddElement(chartcataxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Primary"

            Dim style As XmlElement = AddElement(chartaxis, "Style", Nothing)
            'border
            Dim border As XmlElement = AddElement(style, "Border", Nothing)
            'AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "Silver")
            'AddElement(border, "Width", "0.25pt")
            AddElement(style, "FontFamily", "Arial")
            'AddElement(style, "FontStyle", "Italic")
            AddElement(style, "Color", "#333333")
            'AddElement(style, "TextDecoration", "Underline")
            'AddElement(style, "TextAlign", "Left")
            'AddElement(style, "FontWeight", "Bold")
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            Dim ChartAxisTtl As XmlElement = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            Dim fldfrname As String = GetFriendlyReportFieldName(repid, grpfld)
            AddElement(ChartAxisTtl, "Caption", fldfrname)                             '!!!!!!!!!!!!!!!!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontFamily", "Arial")
            AddElement(style, "Color", "#333333")
            AddElement(style, "FontSize", "7pt")
            If graphtype <> "pie" Then
                AddElement(chartaxis, "Margin", "False")
                AddElement(chartaxis, "Interval", "=1")
            End If
            Dim ChartMajorGridLines As XmlElement = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            AddElement(ChartMajorGridLines, "Enabled", "False")
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            Dim ChartMinorGridLines As XmlElement = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            Dim ChartMajorTickMarks As XmlElement = AddElement(chartaxis, "ChartMajorTickMarks", Nothing)
            style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Silver")
            Dim ChartMinorTickMarks As XmlElement = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            AddElement(chartaxis, "Angle", "80")
            AddElement(chartaxis, "LabelsAutoFitDisabled", "true")
            Dim ChartAxisScaleBreak As XmlElement = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'ChartAxis secondary
            chartaxis = AddElement(chartcataxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Secondary"
            style = AddElement(chartaxis, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", hdrfld)                                            '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            AddElement(ChartMajorGridLines, "Enabled", "False")
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            'ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            AddElement(chartaxis, "Location", "Opposite")
            ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'ChartValueAxes
            Dim ChartValueAxes As XmlElement = AddElement(chartarea, "ChartValueAxes", Nothing)
            'ChartAxis primary
            chartaxis = AddElement(ChartValueAxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Primary"
            style = AddElement(chartaxis, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Silver")
            AddElement(style, "FontFamily", "Arial")
            AddElement(style, "Color", "#333333")
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", Nothing)                    '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            If graphtype <> "pie" Then
                AddElement(chartaxis, "Interval", "=IIf(Max(CountRows(""Chart1_CategoryGroup"")) <= 5,1, Nothing)")
            End If
            ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "White")
            ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorTickMarks", Nothing)
            style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Silver")
            ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            If graphtype <> "pie" Then
                'AddElement(chartaxis, "Angle", "80")
                AddElement(chartaxis, "LabelsAutoFitDisabled", "true")
            End If
            ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'ChartAxis secondary
            chartaxis = AddElement(ChartValueAxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Secondary"
            style = AddElement(chartaxis, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", hdrfld)                                        '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            AddElement(ChartMajorGridLines, "Enabled", "False")
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            'ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            AddElement(chartaxis, "Location", "Opposite")
            ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            style = AddElement(chartarea, "Style", Nothing)
            'AddElement(style, "BackgroundColor", "#e9ecee")
            AddElement(style, "BackgroundColor", "WhiteSmoke")
            AddElement(style, "BackgroundGradientType", "None")

            Dim ChartLegends As XmlElement = AddElement(repgraph, "ChartLegends", Nothing)
            Dim ChartLegend As XmlElement = AddElement(ChartLegends, "ChartLegend", Nothing)
            attr = ChartLegend.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Default"
            style = AddElement(ChartLegend, "Style", Nothing)
            AddElement(style, "FontSize", "8pt")
            AddElement(style, "Color", "#333333")
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(ChartLegend, "Position", "BottomLeft")
            'AddElement(ChartLegend, "Layout", "Row")
            AddElement(ChartLegend, "Layout", "WideTable")
            AddElement(ChartLegend, "DockOutsideChartArea", "true")

            'Dim ChartElementPosition As XmlElement = AddElement(ChartLegend, "ChartElementPosition", Nothing)
            'AddElement(ChartElementPosition, "Top", "80")
            'AddElement(ChartElementPosition, "Left", "3")
            'AddElement(ChartElementPosition, "Height", "20")
            'AddElement(ChartElementPosition, "Width", "100")
            ''Dim ChartInnerPlotPosition As XmlElement = AddElement(ChartLegend, "ChartInnerPlotPosition", Nothing)
            ''AddElement(ChartInnerPlotPosition, "Width", "100")
            AddElement(ChartLegend, "HeaderSeparatorColor", "Black")
            AddElement(ChartLegend, "ColumnSeparatorColor", "Black")
            AddElement(ChartLegend, "MaxAutoSize", "40")
            AddElement(ChartLegend, "TextWrapThreshold", "40")
            ChartAxisTtl = AddElement(ChartLegend, "ChartLegendTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", Nothing)                    '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "8pt")
            AddElement(style, "TextAlign", "Center")
            AddElement(style, "FontWeight", "Bold")

            Dim ChartTitles As XmlElement = AddElement(repgraph, "ChartTitles", Nothing)
            Dim ChartTitle As XmlElement = AddElement(ChartTitles, "ChartTitle", Nothing)
            attr = ChartTitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid            '!!!!!!!!!!!!!!!!!!
            AddElement(ChartTitle, "Caption", hdrfld)
            style = AddElement(ChartTitle, "Style", Nothing)
            AddElement(style, "FontSize", "11pt")
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "FontFamily", "Trebuchet MS")
            AddElement(style, "Color", "#222222")
            AddElement(style, "VerticalAlign", "Top")
            AddElement(style, "TextAlign", "General")
            AddElement(style, "FontWeight", "Normal")

            'Custom Palette
            AddElement(repgraph, "Palette", "Custom")
            Dim ChartCustomPaletteColors As XmlElement = AddElement(repgraph, "ChartCustomPaletteColors", Nothing)
            'Dim ChartCustomPaletteColor As XmlElement = AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", Nothing)
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#8C6ED2")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#9EEAFF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#BA00FF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFF19D")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FF9DCB")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#ADB26E")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#EEEEEE")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#0DCAFF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#5D0CE8")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#75FF9F")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#5D5D5D")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#3A0497")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#F6FF73")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#A375F3")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#112AE8")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#869EFF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#4661F6")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#EE3A63")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#C7F38A")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFFAD3")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#D78BFD")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FF8E33")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFE100")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#2CCC14")

            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "= Code.GetColorI(0)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(1)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(2)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(3)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(4)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(5)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(6)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(7)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(8)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(9)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(10)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(11)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(12)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(13)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(14)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(15)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(16)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(17)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(18)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(19)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(20)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(21)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(22)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(23)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(24)")

            'Dim dynamicwidth As XmlElement = AddElement(repgraph, "DynamicWidth", Nothing)
            AddElement(repgraph, "DynamicWidth", wdth.ToString & "In")

            Dim ChartBorderSkin As XmlElement = AddElement(repgraph, "ChartBorderSkin", Nothing)
            style = AddElement(ChartBorderSkin, "Style", Nothing)
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "Color", "White")
            AddElement(style, "BackgroundColor", "#e9ecee")

            Dim ChartNoDataMessage As XmlElement = AddElement(repgraph, "ChartNoDataMessage", Nothing)
            attr = ChartNoDataMessage.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "NoDataMessage"
            AddElement(ChartNoDataMessage, "Caption", "No Data Available")
            style = AddElement(ChartNoDataMessage, "Style", Nothing)
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "VerticalAlign", "Top")
            AddElement(style, "TextAlign", "General")

            ' AddElement(repgraph, "DataSetName", repid)
            AddElement(repgraph, "Top", "0.7In")
            AddElement(repgraph, "Left", "0.0375In")
            AddElement(repgraph, "Height", "6In")
            AddElement(repgraph, "Width", "10.6In")
            style = AddElement(repgraph, "Style", Nothing)
            AddElement(style, "VerticalAlign", "Top")
            AddElement(style, "PaddingLeft", "2pt")
            AddElement(style, "PaddingRight", "2pt")
            AddElement(style, "PaddingTop", "2pt")
            AddElement(style, "PaddingBottom", "20pt")
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "BackgroundColor", "White")
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Style", "Solid")

            'Dim Code As XmlElement = AddElement(report, "Code", Nothing)

            '           <Code>
            '  'Private colorPalette As String() ={"#AC9494", "#37392C","#939C41"}
            '          'Private colorPalette As String() ={"Red", "Green","Blue"}
            '          Private colorPalette As String() = {"#9EEAFF", "#BA00FF", "#FFF19D", "#FF9DCB", "#8C6ED2", "#ADB26E", "#EEEEEE", "#0DCAFF", "#5D0CE8", "#75FF9F", "#5D5D5D", "#3A0497", "#F6FF73", "#A375F3", "#112AE8", "#869EFF", "#4661F6", "#EE3A63", "#C7F38A", "#FFFAD3", "#C2EBFA", "#D78BFD", "#FF8E33", "#FFE100", "#2CCC14"}
            '          Private count As Integer = 0
            '          Private mapping As New System.Collections.Hashtable()
            '  Public Function GetColor(ByVal groupingValue As String) As String
            '      If mapping.ContainsKey(groupingValue) Then
            '          Return mapping(groupingValue)
            '      End If
            '      Dim c As String = colorPalette(count Mod colorPalette.Length)
            '      count = count + 1
            '      mapping.Add(groupingValue, c)
            '      Return c
            '  End Function

            '  Public Function GetColorI(ByVal i As Integer) As String
            '      Dim ColorCode As String = colorPalette(i Mod colorPalette.Length)
            '      Return ColorCode
            '  End Function
            '</Code>



            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            graf = sw.ToString
            Return graf
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function CreateMatrixRDLForDT(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal fn As String, ByVal primX As String, ByVal secX As String, ByVal valueY As String, ByVal graphtype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "") As String
        'NOT IN USE
        Dim ret As String = String.Empty
        Dim graf As String = String.Empty
        Dim indx As String = String.Empty
        Try
            dt = CorrectDatasetColumns(dt)
            Dim grpfld As String = primX
            Dim grpfld2 As String = secX
            Dim calcfld As String = valueY

            Dim grpfldfr As String = GetFriendlySQLFieldName(repid, grpfld)
            Dim grpfld2fr As String = GetFriendlySQLFieldName(repid, grpfld2)
            Dim calcfldfr As String = GetFriendlySQLFieldName(repid, calcfld)

            Dim hdrfld As String = fn & " Of " & calcfldfr
            Dim hdrfld2 As String = hdrfld
            hdrfld2 = hdrfld2 & " In group by " & grpfldfr
            If grpfld <> grpfld2 Then
                hdrfld2 = hdrfld2 & ", " & grpfld2fr
            End If
            Dim grps As String = grpfldfr & "\" & grpfld2fr
            Dim n, m, i, j As Integer
            'n - count of diffrent primary categories - rows
            'm - count of diffrent secondary categories - columns
            'n = ???????????????????????????
            'm= ??????????????????????????
            Dim grps2(0) As String
            grps2(0) = grpfld2
            Dim distgrps As DataTable = dt.DefaultView.ToTable(True, grps2)
            m = distgrps.Rows.Count

            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            Dim attr As XmlAttribute '= report.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = repid
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            attr = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "DataSource1"
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            AddElement(connectionProperties, "ConnectString", "")
            'AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext")
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", "DataSource1")
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "300")
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = dt.Columns(i).Caption
                AddElement(field, "DataField", dt.Columns(i).Caption)
            Next
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If
            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)
            If orien = "landscape" Then
                AddElement(page, "PageWidth", "11in")
                AddElement(page, "PageHeight", "8.5in")
            Else
                AddElement(page, "PageWidth", "8.5in")
                AddElement(page, "PageHeight", "11in")
            End If
            'Page header
            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.2in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.3in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "1in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "1.5in")
            AddElement(txtbox, "Height", "0.8in")
            AddElement(txtbox, "Width", "6.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'doc report body
            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "3in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "Color", "Gray")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "16pt")
            AddElement(reptitle, "Height", "0.5in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "12pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'subtitle
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", params)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "12pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'subtitle2
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", hdrfld2)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "Color", "Gray")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "12pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'Matrix
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix3"
            Dim TablixCorner As XmlElement = AddElement(tablix, "TablixCorner", Nothing)
            Dim TablixCornerRows As XmlElement = AddElement(TablixCorner, "TablixCornerRows", Nothing)

            Dim TablixCornerRow As XmlElement = AddElement(TablixCornerRows, "TablixCornerRow", Nothing)
            Dim TablixCornerCell As XmlElement = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            Dim CellContents As XmlElement = AddElement(TablixCornerCell, "CellContents", Nothing)
            AddElement(CellContents, "ColSpan", 3)              '!!!!!!!!!!!!!!!!!!!  m ???
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox31"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "rd:DefaultName", "Textbox22")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            Dim border As XmlElement = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "#333333")
            AddElement(ttlstyle, "BackgroundColor", "#333333")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)

            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "Overall " & hdrfld & ": ")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Color", "White")

            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
                '!!!!!!!!!!!!!!!!!!!!!
            Else
                AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")
            End If
            'AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Color", "White")

            AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            AddElement(TablixCornerRow, "TablixCornerCell", Nothing)

            '2d row
            TablixCornerRow = AddElement(TablixCornerRows, "TablixCornerRow", Nothing)
            'TablixCornerCell
            TablixCornerCell = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            CellContents = AddElement(TablixCornerCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox39"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)

            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(ttlstyle, "Color", "White")

            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=CountRows() &amp; "" records""")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "FontWeight", "Normal")
            'AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            'AddElement(ttlstyle, "FontSize", "9pt")
            ''AddElement(ttlstyle, "Color", "White")

            'TablixCornerCell
            TablixCornerCell = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            CellContents = AddElement(TablixCornerCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox16"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", grps)             '!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(ttlstyle, "Color", "White")

            'TablixCornerCell
            TablixCornerCell = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            CellContents = AddElement(TablixCornerCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox17"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "By " & grpfldfr & ":")             '!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")

            'tablixbody
            Dim TablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            'AddElement(TablixBody, "KeepTogether", "true")
            Dim TablixColumns As XmlElement = AddElement(TablixBody, "TablixColumns", Nothing)
            Dim TablixColumn As XmlElement = AddElement(TablixColumns, "TablixColumn", Nothing)
            AddElement(TablixColumn, "Width", "1.1in")
            Dim TablixRows As XmlElement = AddElement(TablixBody, "TablixRows", Nothing)
            Dim TablixRow As XmlElement = AddElement(TablixRows, "TablixRow", Nothing)
            AddElement(TablixRow, "Height", "0.25in")
            Dim TablixCells As XmlElement = AddElement(TablixRow, "TablixCells", Nothing)
            Dim TablixCell As XmlElement = AddElement(TablixCells, "TablixCell", Nothing)
            CellContents = AddElement(TablixCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox18"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "White")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(" & fn & "(" & "Fields!" & calcfld & ".Value),2)")             '!!!!!!!!!!!!!!!!!!!!!
            Else
                AddElement(ttltextRun, "Value", "=" & fn & "(" & "Fields!" & calcfld & ".Value)")             '!!!!!!!!!!!!!!!!!!!!!
            End If

            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")

            'tablix total row
            TablixRow = AddElement(TablixRows, "TablixRow", Nothing)
            AddElement(TablixRow, "Height", "0.25in")
            TablixCells = AddElement(TablixRow, "TablixCells", Nothing)
            TablixCell = AddElement(TablixCells, "TablixCell", Nothing)
            CellContents = AddElement(TablixCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox19"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")             '!!!!!!!!!!!!!!!!!!!!!
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            Else
                AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")
            End If
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")

            'TablixColumnHierarchy
            Dim TablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim TablixMembers As XmlElement = AddElement(TablixColumnHierarchy, "TablixMembers", Nothing)
            Dim TablixMember As XmlElement = AddElement(TablixMembers, "TablixMember", Nothing)
            Dim grptbx As XmlElement = AddElement(TablixMember, "Group", Nothing)
            attr = grptbx.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = grpfld2 'TODO friendly name of secondary group  !!!!!!!!!!!!!!!!!!!!!!!!
            'GroupExpressions
            Dim GroupExpressions As XmlElement = AddElement(grptbx, "GroupExpressions", Nothing)
            Dim GroupExpression As XmlElement = AddElement(GroupExpressions, "GroupExpression", "=Fields!" & grpfld2 & ".Value")  '!!!!!!!!!!!!!!!!!!!
            'SortExpressions
            Dim SortExpressions As XmlElement = AddElement(TablixMember, "SortExpressions", Nothing)
            Dim SortExpression As XmlElement = AddElement(SortExpressions, "SortExpression", Nothing)
            AddElement(SortExpression, "Value", "=Fields!" & grpfld2 & ".Value")
            AddElement(TablixMember, "KeepTogether", "true")
            'TablixHeader
            Dim TablixHeader As XmlElement = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox20"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "#333333")
            AddElement(ttlstyle, "BackgroundColor", "#333333")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            AddElement(ttlstyle, "FontSize", "9pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "White")

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            AddElement(ttlstyle, "FontSize", "9pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "White")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox21"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Fields!" & grpfld2 & ".Value")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            'AddElement(ttlstyle, "Color", "White")

            'TablixRowHierarchy
            Dim TablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim TablixMembers1 As XmlElement = AddElement(TablixRowHierarchy, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers1, "TablixMember", Nothing)
            grptbx = AddElement(TablixMember, "Group", Nothing)
            attr = grptbx.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = grpfld 'TODO ? friendly name of primary group  !!!!!!!!!!!!!!!!!!!!!!!!
            'GroupExpressions
            GroupExpressions = AddElement(grptbx, "GroupExpressions", Nothing)
            GroupExpression = AddElement(GroupExpressions, "GroupExpression", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            'SortExpressions
            SortExpressions = AddElement(TablixMember, "SortExpressions", Nothing)
            SortExpression = AddElement(SortExpressions, "SortExpression", Nothing)
            AddElement(SortExpression, "Value", "=Fields!" & grpfld & ".Value")                                        '!!!!!!!!!!!!!!!!!!!!!!!!!!
            'TablixHeader
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox22"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")  'TODO Colors  !!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            AddElement(ttlstyle, "FontSize", "9pt")
            AddElement(ttlstyle, "FontWeight", "Bold")
            'AddElement(ttlstyle, "Color", "White")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.8in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox23"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Label", grpfld)  ' TODO ? change for friendly !!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "#222222")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox24"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            'AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")  '!!!!!!!!!!!!!!!!!!!
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            Else
                AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")
            End If
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "#222222")

            'TablixMember in TablixMembers1
            TablixMember = AddElement(TablixMembers1, "TablixMember", Nothing)
            AddElement(TablixMember, "KeepWithGroup", "Before")
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "1.2in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox14"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Right")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Label", grpfld2)  ' TODO ? change for friendly !!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", Nothing)  ' ? !!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Bold")
            'AddElement(ttlstyle, "Color", "#222222")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox25"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "By " & grpfld2fr & ":")  '!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Bold")
            'AddElement(ttlstyle, "Color", "#222222")

            AddElement(tablix, "DataSetName", repid)
            AddElement(tablix, "KeepTogether", "true")
            AddElement(tablix, "Top", "1in")
            AddElement(tablix, "Left", "0.3in")
            AddElement(tablix, "Height", "1in")
            AddElement(tablix, "Width", "3in")
            ttlstyle = AddElement(tablix, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "None")





            ''Chart
            'Dim repgraph As XmlElement = AddElement(reportItems, "Chart", Nothing)
            'attr = repgraph.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Chart1"
            ''ChartCategoryHierarchy
            'Dim chartcathierarchy As XmlElement = AddElement(repgraph, "ChartCategoryHierarchy", Nothing)
            'Dim chartmembrs As XmlElement = AddElement(chartcathierarchy, "ChartMembers", Nothing)
            'Dim chartmembr As XmlElement = AddElement(chartmembrs, "ChartMember", Nothing)
            'Dim grp As XmlElement = AddElement(chartmembr, "Group", Nothing)
            'attr = grp.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Chart1_CategoryGroup"
            ''GroupExpressions
            'Dim grpexprs As XmlElement = AddElement(grp, "GroupExpressions", Nothing)
            'Dim grpexpr As XmlElement = AddElement(grpexprs, "GroupExpression", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            'Dim sortexprs As XmlElement = AddElement(chartmembr, "SortExpressions", Nothing)
            'Dim sortexpr As XmlElement = AddElement(sortexprs, "SortExpression", Nothing)
            'AddElement(sortexpr, "Value", "=Fields!" & grpfld & ".Value")                                        '!!!!!!!!!!!!!!!!!!!!!!!!!!
            'Dim lbl As XmlElement = AddElement(chartmembr, "Label", "=Fields!" & grpfld & ".Value")              '!!!!!!!!!!!!!!!!!!!!!!!!!!
            ''ChartSeriesHierarchy
            'Dim chartserhierarchy As XmlElement = AddElement(repgraph, "ChartSeriesHierarchy", Nothing)
            'Dim chartsermembrs As XmlElement = AddElement(chartserhierarchy, "ChartMembers", Nothing)
            'Dim chartsermembr As XmlElement = AddElement(chartsermembrs, "ChartMember", Nothing)
            'Dim grpser As XmlElement = AddElement(chartsermembr, "Group", Nothing)
            'attr = grpser.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Chart1_SeriesGroup"
            ''GroupExpressions
            'Dim grpserexprs As XmlElement = AddElement(grpser, "GroupExpressions", Nothing)
            ''Dim grpserexpr As XmlElement = AddElement(grpserexprs, "GroupExpression", "=Fields!" & calcfld & ".Value")
            'Dim grpserexpr As XmlElement = AddElement(grpserexprs, "GroupExpression", "=Fields!" & grpfld2 & ".Value")  '!!!!!!!!!!!!!!!!!!!!
            'Dim sortserexprs As XmlElement = AddElement(chartsermembr, "SortExpressions", Nothing)
            'Dim sortserexpr As XmlElement = AddElement(sortserexprs, "SortExpression", Nothing)
            'AddElement(sortserexpr, "Value", "=Fields!" & grpfld2 & ".Value")                                         '!!!!!
            ''Dim lblser As XmlElement = AddElement(chartsermembr, "Label", "=Fields!" & calcfld & ".Value")
            'Dim lblser As XmlElement = AddElement(chartsermembr, "Label", "=Fields!" & grpfld2 & ".Value")            '!!!!!
            ''ChartData
            'Dim chartdata As XmlElement = AddElement(repgraph, "ChartData", Nothing)
            ''ChartSeriesCollection
            'Dim chartsercols As XmlElement = AddElement(chartdata, "ChartSeriesCollection", Nothing)
            'Dim chartsers As XmlElement = AddElement(chartsercols, "ChartSeries", Nothing)
            'attr = chartsers.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = calcfld
            'If graphtype = "pie" Then
            '    AddElement(chartsers, "Type", "Shape")
            'End If
            ''ChartDataPoints
            'Dim chartdatapnts As XmlElement = AddElement(chartsers, "ChartDataPoints", Nothing)
            'Dim chartdatapnt As XmlElement = AddElement(chartdatapnts, "ChartDataPoint", Nothing)
            ''ChartDataPointValues
            'Dim chartdatapntvals As XmlElement = AddElement(chartdatapnt, "ChartDataPointValues", Nothing)
            'Dim yval As String = fn & "(" & "Fields!" & calcfld & ".Value)"                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'AddElement(chartdatapntvals, "Y", "=" & yval)
            ''ChartDataLabel
            'Dim chartdatalbl As XmlElement = AddElement(chartdatapnt, "ChartDataLabel", Nothing)
            'AddElement(chartdatalbl, "Style", Nothing)
            'Dim chartmarker As XmlElement = AddElement(chartdatapnt, "ChartMarker", Nothing)
            'AddElement(chartmarker, "Style", Nothing)
            ''DataElementOutput
            'AddElement(chartdatapnt, "DataElementOutput", "Output")
            'AddElement(chartsers, "Style", Nothing)
            ''ChartEmptyPoints
            'Dim chartemptypnts As XmlElement = AddElement(chartsers, "ChartEmptyPoints", Nothing)
            'AddElement(chartemptypnts, "Style", Nothing)
            'Dim chartemptylbl As XmlElement = AddElement(chartemptypnts, "ChartDataLabel", Nothing)
            'AddElement(chartemptylbl, "Style", Nothing)
            'Dim chartemptymarker As XmlElement = AddElement(chartemptypnts, "ChartMarker", Nothing)
            'AddElement(chartemptymarker, "Style", Nothing)
            ''ValueAxisName
            'AddElement(chartsers, "ValueAxisName", "Primary")
            'AddElement(chartsers, "CategoryAxisName", "Primary")
            ''ChartEmptyPoints
            'Dim chartsmartlbl As XmlElement = AddElement(chartsers, "ChartSmartLabel", Nothing)
            'AddElement(chartsmartlbl, "CalloutLineColor", "Black")
            'AddElement(chartsmartlbl, "MinMovingDistance", "0pt")

            ''ChartAreas
            'Dim chartareas As XmlElement = AddElement(repgraph, "ChartAreas", Nothing)
            ''ChartArea
            'Dim chartarea As XmlElement = AddElement(chartareas, "ChartArea", Nothing)
            'attr = chartarea.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Default"
            ''ChartCategoryAxes
            'Dim chartcataxes As XmlElement = AddElement(chartarea, "ChartCategoryAxes", Nothing)
            ''ChartAxis primary
            'Dim chartaxis As XmlElement = AddElement(chartcataxes, "ChartAxis", Nothing)
            'attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Primary"
            'Dim style As XmlElement = AddElement(chartaxis, "Style", Nothing)
            ''border
            'border = AddElement(style, "Border", Nothing)
            ''AddElement(border, "Style", "Solid")
            'AddElement(border, "Color", "Silver")
            ''AddElement(border, "Width", "0.25pt")
            'AddElement(style, "FontFamily", "Verdana")
            ''AddElement(style, "FontStyle", "Italic")
            'AddElement(style, "Color", "#333333")
            ''AddElement(style, "TextDecoration", "Underline")
            ''AddElement(style, "TextAlign", "Left")
            ''AddElement(style, "FontWeight", "Bold")
            'AddElement(style, "FontSize", "8pt")
            ''ChartAxisTitle
            'Dim ChartAxisTtl As XmlElement = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            'Dim fldfrname As String = GetFriendlyReportFieldName(repid, grpfld)
            'AddElement(ChartAxisTtl, "Caption", fldfrname)                             '!!!!!!!!!!!!!!!!!!!!!
            'style = AddElement(ChartAxisTtl, "Style", Nothing)
            'AddElement(style, "FontFamily", "Trebuchet MS")
            'AddElement(style, "Color", "#333333")
            ''AddElement(style, "FontSize", "9pt")
            'If graphtype <> "pie" Then
            '    AddElement(chartaxis, "Interval", "=1")
            'End If
            'Dim ChartMajorGridLines As XmlElement = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'AddElement(ChartMajorGridLines, "Enabled", "False")
            'style = AddElement(ChartMajorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'Dim ChartMinorGridLines As XmlElement = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            'style = AddElement(ChartMinorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'AddElement(border, "Style", "Dotted")
            'Dim ChartMajorTickMarks As XmlElement = AddElement(chartaxis, "ChartMajorTickMarks", Nothing)
            'style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            'Dim ChartMinorTickMarks As XmlElement = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            'AddElement(ChartMinorTickMarks, "Length", "0.5")
            'AddElement(chartaxis, "CrossAt", "NaN")
            'AddElement(chartaxis, "Minimum", "NaN")
            'AddElement(chartaxis, "Maximum", "NaN")
            'AddElement(chartaxis, "Angle", "80")
            'AddElement(chartaxis, "LabelsAutoFitDisabled", "true")
            'Dim ChartAxisScaleBreak As XmlElement = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            'AddElement(ChartAxisScaleBreak, "Style", Nothing)

            ''ChartAxis secondary
            'chartaxis = AddElement(chartcataxes, "ChartAxis", Nothing)
            'attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Secondary"
            'style = AddElement(chartaxis, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            ''ChartAxisTitle
            'ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            'AddElement(ChartAxisTtl, "Caption", hdrfld)                                            '!!!!!!
            'style = AddElement(ChartAxisTtl, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            'ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'AddElement(ChartMajorGridLines, "Enabled", "False")
            'style = AddElement(ChartMajorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            'style = AddElement(ChartMinorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'AddElement(border, "Style", "Dotted")
            ''ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            ''style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            ''border = AddElement(style, "Border", Nothing)
            ''AddElement(border, "Color", "Silver")
            'ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            'AddElement(ChartMinorTickMarks, "Length", "0.5")
            'AddElement(chartaxis, "CrossAt", "NaN")
            'AddElement(chartaxis, "Minimum", "NaN")
            'AddElement(chartaxis, "Maximum", "NaN")
            'AddElement(chartaxis, "Location", "Opposite")
            'ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            'AddElement(ChartAxisScaleBreak, "Style", Nothing)

            ''ChartValueAxes
            'Dim ChartValueAxes As XmlElement = AddElement(chartarea, "ChartValueAxes", Nothing)
            ''ChartAxis primary
            'chartaxis = AddElement(ChartValueAxes, "ChartAxis", Nothing)
            'attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Primary"
            'style = AddElement(chartaxis, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            'AddElement(style, "FontFamily", "Verdana")
            'AddElement(style, "Color", "#333333")
            'AddElement(style, "FontSize", "8pt")
            ''ChartAxisTitle
            'ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            'AddElement(ChartAxisTtl, "Caption", Nothing)                    '!!!!!!
            'style = AddElement(ChartAxisTtl, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            'If graphtype <> "pie" Then
            '    AddElement(chartaxis, "Interval", "=IIf(Max(CountRows(""Chart1_CategoryGroup"")) <= 5,1, Nothing)")
            'End If
            'ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'style = AddElement(ChartMajorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "White")
            'ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            'style = AddElement(ChartMinorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'AddElement(border, "Style", "Dotted")
            'ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorTickMarks", Nothing)
            'style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            'ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            'AddElement(ChartMinorTickMarks, "Length", "0.5")
            'AddElement(chartaxis, "CrossAt", "NaN")
            'AddElement(chartaxis, "Minimum", "NaN")
            'AddElement(chartaxis, "Maximum", "NaN")
            'If graphtype <> "pie" Then
            '    AddElement(chartaxis, "LabelsAutoFitDisabled", "true")
            'End If
            'ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            'AddElement(ChartAxisScaleBreak, "Style", Nothing)

            ''ChartAxis secondary
            'chartaxis = AddElement(ChartValueAxes, "ChartAxis", Nothing)
            'attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Secondary"
            'style = AddElement(chartaxis, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            ''ChartAxisTitle
            'ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            'AddElement(ChartAxisTtl, "Caption", hdrfld)                                        '!!!!!!
            'style = AddElement(ChartAxisTtl, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            'ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'AddElement(ChartMajorGridLines, "Enabled", "False")
            'style = AddElement(ChartMajorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            'style = AddElement(ChartMinorGridLines, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Gainsboro")
            'AddElement(border, "Style", "Dotted")
            ''ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            ''style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            ''border = AddElement(style, "Border", Nothing)
            ''AddElement(border, "Color", "Silver")
            'ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            'AddElement(ChartMinorTickMarks, "Length", "0.5")
            'AddElement(chartaxis, "CrossAt", "NaN")
            'AddElement(chartaxis, "Minimum", "NaN")
            'AddElement(chartaxis, "Maximum", "NaN")
            'AddElement(chartaxis, "Location", "Opposite")
            'ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            'AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'style = AddElement(chartarea, "Style", Nothing)
            'AddElement(style, "BackgroundColor", "#e9ecee")
            'AddElement(style, "BackgroundGradientType", "None")

            'Dim ChartLegends As XmlElement = AddElement(repgraph, "ChartLegends", Nothing)
            'Dim ChartLegend As XmlElement = AddElement(ChartLegends, "ChartLegend", Nothing)
            'attr = ChartLegend.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "Default"
            'style = AddElement(ChartLegend, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            'AddElement(style, "Color", "#333333")
            'AddElement(style, "BackgroundGradientType", "None")
            'AddElement(ChartLegend, "Position", "BottomLeft")
            'AddElement(ChartLegend, "DockOutsideChartArea", "true")
            'AddElement(ChartLegend, "HeaderSeparatorColor", "Black")
            'AddElement(ChartLegend, "ColumnSeparatorColor", "Black")
            'ChartAxisTtl = AddElement(ChartLegend, "ChartLegendTitle", Nothing)
            'AddElement(ChartAxisTtl, "Caption", Nothing)                    '!!!!!!
            'style = AddElement(ChartAxisTtl, "Style", Nothing)
            'AddElement(style, "FontSize", "8pt")
            'AddElement(style, "TextAlign", "Center")
            'AddElement(style, "FontWeight", "Bold")

            'Dim ChartTitles As XmlElement = AddElement(repgraph, "ChartTitles", Nothing)
            'Dim ChartTitle As XmlElement = AddElement(ChartTitles, "ChartTitle", Nothing)
            'attr = ChartTitle.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = repid            '!!!!!!!!!!!!!!!!!!
            'AddElement(ChartTitle, "Caption", hdrfld)
            'style = AddElement(ChartTitle, "Style", Nothing)
            'AddElement(style, "FontSize", "11pt")
            'AddElement(style, "BackgroundGradientType", "None")
            'AddElement(style, "FontFamily", "Trebuchet MS")
            'AddElement(style, "Color", "#222222")
            'AddElement(style, "VerticalAlign", "Top")
            'AddElement(style, "TextAlign", "General")
            'AddElement(style, "FontWeight", "Normal")

            'Dim ChartBorderSkin As XmlElement = AddElement(repgraph, "ChartBorderSkin", Nothing)
            'Style = AddElement(ChartBorderSkin, "Style", Nothing)
            'AddElement(Style, "BackgroundGradientType", "None")
            'AddElement(Style, "Color", "White")
            'AddElement(Style, "BackgroundColor", "Gray")

            'Dim ChartNoDataMessage As XmlElement = AddElement(repgraph, "ChartNoDataMessage", Nothing)
            'attr = ChartNoDataMessage.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "NoDataMessage"
            'AddElement(ChartNoDataMessage, "Caption", "No Data Available")
            'Style = AddElement(ChartNoDataMessage, "Style", Nothing)
            'AddElement(Style, "BackgroundGradientType", "None")
            'AddElement(Style, "VerticalAlign", "Top")
            'AddElement(Style, "TextAlign", "General")

            '' AddElement(repgraph, "DataSetName", repid)
            'AddElement(repgraph, "Top", "1.12166in")
            'AddElement(repgraph, "Left", "0.0375in")
            'AddElement(repgraph, "Height", "5.79376in")
            'AddElement(repgraph, "Width", "10.6in")
            'Style = AddElement(repgraph, "Style", Nothing)
            'AddElement(Style, "BackgroundGradientType", "None")
            'AddElement(Style, "BackgroundColor", "White")
            'border = AddElement(Style, "Border", Nothing)
            'AddElement(border, "Color", "LightGrey")
            'AddElement(border, "Style", "Solid")

            ''Custom Palette
            'AddElement(repgraph, "Palette", "Custom")
            'Dim ChartCustomPaletteColors As XmlElement = AddElement(repgraph, "ChartCustomPaletteColors", Nothing)
            ''Dim ChartCustomPaletteColor As XmlElement = AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", Nothing)
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#8C6ED2")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#9EEAFF")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#BA00FF")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFF19D")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FF9DCB")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#ADB26E")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#EEEEEE")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#0DCAFF")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#5D0CE8")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#75FF9F")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#5D5D5D")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#3A0497")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#F6FF73")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#A375F3")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#112AE8")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#869EFF")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#4661F6")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#EE3A63")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#C7F38A")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFFAD3")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#D78BFD")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FF8E33")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFE100")
            'AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#2CCC14")

            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(0)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(1)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(2)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(3)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(4)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(5)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(6)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(7)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(8)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(9)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(10)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(11)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(12)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(13)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(14)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(15)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(16)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(17)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(18)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(19)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(20)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(21)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(22)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(23)")
            ''AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(24)")



            ''Dim Code As XmlElement = AddElement(report, "Code", Nothing)

            ''           <Code>
            ''  'Private colorPalette As String() ={"#AC9494", "#37392C","#939C41"}
            ''          'Private colorPalette As String() ={"Red", "Green","Blue"}
            ''          Private colorPalette As String() = {"#9EEAFF", "#BA00FF", "#FFF19D", "#FF9DCB", "#8C6ED2", "#ADB26E", "#EEEEEE", "#0DCAFF", "#5D0CE8", "#75FF9F", "#5D5D5D", "#3A0497", "#F6FF73", "#A375F3", "#112AE8", "#869EFF", "#4661F6", "#EE3A63", "#C7F38A", "#FFFAD3", "#C2EBFA", "#D78BFD", "#FF8E33", "#FFE100", "#2CCC14"}
            ''          Private count As Integer = 0
            ''          Private mapping As New System.Collections.Hashtable()
            ''  Public Function GetColor(ByVal groupingValue As String) As String
            ''      If mapping.ContainsKey(groupingValue) Then
            ''          Return mapping(groupingValue)
            ''      End If
            ''      Dim c As String = colorPalette(count Mod colorPalette.Length)
            ''      count = count + 1
            ''      mapping.Add(groupingValue, c)
            ''      Return c
            ''  End Function

            ''  Public Function GetColorI(ByVal i As Integer) As String
            ''      Dim ColorCode As String = colorPalette(i Mod colorPalette.Length)
            ''      Return ColorCode
            ''  End Function
            ''</Code>



            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            graf = sw.ToString

            ''----------------------------------------------------------
            ''For testing only, comment this block for production
            'Dim graphpath As String = System.AppDomain.CurrentDomain.BaseDirectory() & "RDLFILES\testingmatrix.rdl"
            ''save to file
            'doc.Save(graphpath)

            'Dim Bytes() As Byte = Encoding.UTF8.GetBytes(graf)
            'Using Stream As New FileStream(graphpath, FileMode.Create)
            '    Stream.Write(Bytes, 0, Bytes.Length)
            '    Stream.Close()
            '    Stream.Dispose()
            'End Using
            '----------------------------------------------------------



            Return graf
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function CalculateDataTableForMatrixReportAI(ByVal repid As String, ByVal dt As DataTable, ByVal catX1 As String, ByVal catX2 As String, ByVal calcY As String, ByVal fn As String, Optional ByRef er As String = "") As DataTable
        Dim mdt As New DataTable
        Dim dtemp As DataTable
        Try
            Dim dcs As DataTable = ComputeStats(dt, fn, calcY, catX1, catX2, "", er)
            Dim flds(2) As String
            flds(0) = catX1
            flds(1) = catX2
            flds(2) = "ARR"
            'convert dcs into mdt table
            Dim i, j As Integer
            Dim col As DataColumn
            Dim row As DataRow
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = catX1 '& "/" & catX2
            mdt.Columns.Add(col)

            Dim mdtrows As DataTable = dt.DefaultView.ToTable(True, catX1)
            mdtrows.DefaultView.Sort = catX1 & " ASC"
            mdtrows = mdtrows.DefaultView.ToTable(True, catX1)
            Dim mdtcols As DataTable = dt.DefaultView.ToTable(True, catX2)
            mdtcols.DefaultView.Sort = catX2 & " ASC"
            mdtcols = mdtcols.DefaultView.ToTable(True, catX2)

            For j = 0 To mdtcols.Rows.Count - 1
                If mdtcols.Rows(j)(catX2).ToString.Trim <> "" Then
                    'add column to mdt
                    col = New DataColumn
                    If fn.ToUpper = "COUNT" OrElse fn.ToUpper.Trim = "DISTCOUNT" OrElse fn.ToUpper.Trim = "COUNTDISTINCT" Then
                        col.DataType = System.Type.GetType("System.Int32")
                    Else
                        col.DataType = System.Type.GetType("System.Double")
                    End If

                    col.ColumnName = mdtcols.Rows(j)(catX2).ToString.Trim
                    col.DefaultValue = 0
                    mdt.Columns.Add(col)
                End If
            Next
            For i = 0 To mdtrows.Rows.Count - 1
                row = mdt.NewRow
                row(0) = mdtrows(i)(catX1)
                For j = 0 To mdtcols.Rows.Count - 1
                    'calc values for cells in dt with rows where field catX1=mdtrows(i)(catX1) and field catX2=mdtcols(j)(catX2)
                    row(j + 1) = 0
                    dcs.DefaultView.RowFilter = ""
                    dcs.DefaultView.RowFilter = catX1 & "='" & mdtrows(i)(catX1) & "' AND " & catX2 & "='" & mdtcols(j)(catX2) & "'"
                    dtemp = dcs.DefaultView.ToTable(True, flds)
                    If dtemp.Rows.Count = 1 Then
                        row(j + 1) = dtemp(0)("ARR")
                    End If
                    dcs.DefaultView.RowFilter = ""
                Next
                mdt.Rows.Add(row)
            Next

        Catch ex As Exception
            er = ex.Message
        End Try
        Return mdt
    End Function
    Public Function CreateMatrixRDLWithLinks(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal fn As String, ByVal primX As String, ByVal secX As String, ByVal valueY As String, ByVal graphtype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "") As String
        Dim ret As String = String.Empty
        Dim graf As String = String.Empty
        Dim indx As String = String.Empty
        Dim url As String = String.Empty
        Try
            Dim grpfld As String = primX
            Dim grpfld2 As String = secX
            Dim calcfld As String = valueY

            Dim grpfldfr As String = GetFriendlySQLFieldName(repid, grpfld)
            Dim grpfld2fr As String = GetFriendlySQLFieldName(repid, grpfld2)
            Dim calcfldfr As String = GetFriendlySQLFieldName(repid, calcfld)

            Dim hdrfld As String = fn & " of " & calcfldfr
            Dim hdrfld2 As String = hdrfld
            hdrfld2 = hdrfld2 & " in group by " & grpfldfr
            If grpfld <> grpfld2 Then
                hdrfld2 = hdrfld2 & ", " & grpfld2fr
            End If
            Dim grps As String = grpfldfr & "\" & grpfld2fr
            Dim n, m, i, j As Integer
            'n - count of diffrent primary categories - rows
            'm - count of diffrent secondary categories - columns
            'n = ???????????????????????????
            'm= ??????????????????????????
            Dim grps2(0) As String
            grps2(0) = grpfld2
            Dim distgrps As DataTable = dt.DefaultView.ToTable(True, grps2)
            m = distgrps.Rows.Count

            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            Dim attr As XmlAttribute '= report.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = repid
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")
            'AddElement(report, "EnableHyperlinks", "true")
            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            attr = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "DataSource1"
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            AddElement(connectionProperties, "ConnectString", "")
            'AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext")
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", "DataSource1")
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "300")
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = dt.Columns(i).Caption
                AddElement(field, "DataField", dt.Columns(i).Caption)
            Next
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            Dim actioninfo As XmlElement
            Dim actions As XmlElement
            Dim action As XmlElement

            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If
            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)
            If orien = "landscape" Then
                AddElement(page, "PageWidth", "11in")
                AddElement(page, "PageHeight", "8.5in")
            Else
                AddElement(page, "PageWidth", "8.5in")
                AddElement(page, "PageHeight", "11in")
            End If
            'Page header
            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.2in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.3in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "1in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "1.5in")
            AddElement(txtbox, "Height", "0.8in")
            AddElement(txtbox, "Width", "6.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'doc report body
            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "3in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "Color", "Gray")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "16pt")
            AddElement(reptitle, "Height", "0.5in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "12pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'subtitle
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", params)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "12pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'subtitle2
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", hdrfld2)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "Color", "Gray")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "12pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'Matrix
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix3"
            Dim TablixCorner As XmlElement = AddElement(tablix, "TablixCorner", Nothing)
            Dim TablixCornerRows As XmlElement = AddElement(TablixCorner, "TablixCornerRows", Nothing)

            Dim TablixCornerRow As XmlElement = AddElement(TablixCornerRows, "TablixCornerRow", Nothing)
            Dim TablixCornerCell As XmlElement = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            Dim CellContents As XmlElement = AddElement(TablixCornerCell, "CellContents", Nothing)
            AddElement(CellContents, "ColSpan", 3)              '!!!!!!!!!!!!!!!!!!!  m ???
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox31"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")

            'hyperlink
            actioninfo = AddElement(txtbox, "ActionInfo", "true")
            actions = AddElement(actioninfo, "Actions", "true")
            action = AddElement(actions, "Action", "true")
            'url = "javascript:void(window.open('ReportViews.aspx?srd=11&det=yes&cat1=" + grpfld + "&cat2=" + grpfld2 + "&val1=all&val2=all'))"

            url = "javascript:window.location.href='ReportViews.aspx?srd=11&det=yes&cat1=" + grpfld + "&cat2=" + grpfld2 + "&val1=all&val2=all'"

            AddElement(action, "Hyperlink", url)


            ttlstyle = AddElement(txtbox, "Style", Nothing)
            Dim border As XmlElement = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "DimGray")
            AddElement(ttlstyle, "BackgroundColor", "DimGray")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            AddElement(ttlstyle, "Color", "Blue")

            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)

            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "Overall " & hdrfld & ": ")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Color", "White")

            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            ElseIf fn = "Value" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & "Fields!" & calcfld & ".Value),2)")
            Else
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            End If
            'AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Color", "Blue")


            AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            AddElement(TablixCornerRow, "TablixCornerCell", Nothing)

            '2d row
            TablixCornerRow = AddElement(TablixCornerRows, "TablixCornerRow", Nothing)
            'TablixCornerCell
            TablixCornerCell = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            CellContents = AddElement(TablixCornerCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox39"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)

            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")


            'TablixCornerCell
            TablixCornerCell = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            CellContents = AddElement(TablixCornerCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox16"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", grps)             '!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(ttlstyle, "Color", "White")

            'TablixCornerCell
            TablixCornerCell = AddElement(TablixCornerRow, "TablixCornerCell", Nothing)
            CellContents = AddElement(TablixCornerCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox17"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Value", Nothing)
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "By " & grpfldfr & ":")             '!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontWeight", "Bold")
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")

            'tablixbody
            Dim TablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            'AddElement(TablixBody, "KeepTogether", "true")
            Dim TablixColumns As XmlElement = AddElement(TablixBody, "TablixColumns", Nothing)
            Dim TablixColumn As XmlElement = AddElement(TablixColumns, "TablixColumn", Nothing)
            AddElement(TablixColumn, "Width", "1.1in")
            Dim TablixRows As XmlElement = AddElement(TablixBody, "TablixRows", Nothing)
            Dim TablixRow As XmlElement = AddElement(TablixRows, "TablixRow", Nothing)
            AddElement(TablixRow, "Height", "0.25in")
            Dim TablixCells As XmlElement = AddElement(TablixRow, "TablixCells", Nothing)
            Dim TablixCell As XmlElement = AddElement(TablixCells, "TablixCell", Nothing)
            CellContents = AddElement(TablixCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox18"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")

            'hyperlink
            actioninfo = AddElement(txtbox, "ActionInfo", "true")
            actions = AddElement(actioninfo, "Actions", "true")
            action = AddElement(actions, "Action", "true")
            'url = "=""" & "javascript:void(window.open('ReportViews.aspx?srd=11&det=yes&cat1=" & grpfld & "&cat2=" & grpfld2 & "&val1=" & """ & Fields!" & grpfld & ".Value  & ""&val2=""" & " & Fields!" & grpfld2 & ".Value  & "" '))"""

            url = "=""" & "javascript:window.location.href='ReportViews.aspx?srd=11&det=yes&cat1=" & grpfld & "&cat2=" & grpfld2 & "&val1=" & """ & Fields!" & grpfld & ".Value  & ""&val2=""" & " & Fields!" & grpfld2 & ".Value  & ""'"""

            AddElement(action, "Hyperlink", url)


            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "White")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(" & fn & "(" & "Fields!" & calcfld & ".Value),2)")             '!!!!!!!!!!!!!!!!!!!!!
            ElseIf fn = "Value" Then
                AddElement(ttltextRun, "Value", "=Round(Fields!" & calcfld & ".Value,2)")                         '!!!!!!!!!!!!!!!!!!!!!
            Else
                AddElement(ttltextRun, "Value", "=Round(" & fn & "(" & "Fields!" & calcfld & ".Value),2)")             '!!!!!!!!!!!!!!!!!!!!!
            End If

            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "Color", "Blue")

            'tablix total row
            TablixRow = AddElement(TablixRows, "TablixRow", Nothing)
            AddElement(TablixRow, "Height", "0.25in")
            TablixCells = AddElement(TablixRow, "TablixCells", Nothing)
            TablixCell = AddElement(TablixCells, "TablixCell", Nothing)
            CellContents = AddElement(TablixCell, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox19"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")

            'hyperlink
            actioninfo = AddElement(txtbox, "ActionInfo", "true")
            actions = AddElement(actioninfo, "Actions", "true")
            action = AddElement(actions, "Action", "true")
            url = "=""" & "javascript:window.location.href='ReportViews.aspx?srd=11&det=yes&cat1=" & grpfld & "&val1=all&cat2=" & grpfld2 & "&val2=" & """ & Fields!" & grpfld2 & ".Value  & ""'"""

            AddElement(action, "Hyperlink", url)


            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")             '!!!!!!!!!!!!!!!!!!!!!
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            ElseIf fn = "Value" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & "Fields!" & calcfld & ".Value),2)")
            Else
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            End If
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "Blue")

            'TablixColumnHierarchy
            Dim TablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim TablixMembers As XmlElement = AddElement(TablixColumnHierarchy, "TablixMembers", Nothing)
            Dim TablixMember As XmlElement = AddElement(TablixMembers, "TablixMember", Nothing)
            Dim grptbx As XmlElement = AddElement(TablixMember, "Group", Nothing)
            attr = grptbx.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = grpfld2 'TODO friendly name of secondary group  !!!!!!!!!!!!!!!!!!!!!!!!
            'GroupExpressions
            Dim GroupExpressions As XmlElement = AddElement(grptbx, "GroupExpressions", Nothing)
            Dim GroupExpression As XmlElement = AddElement(GroupExpressions, "GroupExpression", "=Fields!" & grpfld2 & ".Value")  '!!!!!!!!!!!!!!!!!!!
            'SortExpressions
            Dim SortExpressions As XmlElement = AddElement(TablixMember, "SortExpressions", Nothing)
            Dim SortExpression As XmlElement = AddElement(SortExpressions, "SortExpression", Nothing)
            AddElement(SortExpression, "Value", "=Fields!" & grpfld2 & ".Value")
            AddElement(TablixMember, "KeepTogether", "true")
            'TablixHeader
            Dim TablixHeader As XmlElement = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox20"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "DimGray")
            AddElement(ttlstyle, "BackgroundColor", "DimGray")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            AddElement(ttlstyle, "FontSize", "9pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "White")

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            AddElement(ttlstyle, "FontSize", "9pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "White")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox21"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#d5dae1")
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Fields!" & grpfld2 & ".Value")  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            'AddElement(ttlstyle, "Color", "White")

            'TablixRowHierarchy
            Dim TablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim TablixMembers1 As XmlElement = AddElement(TablixRowHierarchy, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers1, "TablixMember", Nothing)
            grptbx = AddElement(TablixMember, "Group", Nothing)
            attr = grptbx.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = grpfld 'TODO ? friendly name of primary group  !!!!!!!!!!!!!!!!!!!!!!!!
            'GroupExpressions
            GroupExpressions = AddElement(grptbx, "GroupExpressions", Nothing)
            GroupExpression = AddElement(GroupExpressions, "GroupExpression", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            'SortExpressions
            SortExpressions = AddElement(TablixMember, "SortExpressions", Nothing)
            SortExpression = AddElement(SortExpressions, "SortExpression", Nothing)
            AddElement(SortExpression, "Value", "=Fields!" & grpfld & ".Value")                                        '!!!!!!!!!!!!!!!!!!!!!!!!!!
            'TablixHeader
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox22"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")  'TODO Colors  !!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)

            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", Nothing)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Trebuchet MS")
            AddElement(ttlstyle, "FontSize", "9pt")
            AddElement(ttlstyle, "FontWeight", "Bold")
            'AddElement(ttlstyle, "Color", "White")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.8in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox23"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Label", grpfld)  ' TODO ? change for friendly !!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "#222222")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox24"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")

            'hyperlink
            actioninfo = AddElement(txtbox, "ActionInfo", "true")
            actions = AddElement(actioninfo, "Actions", "true")
            action = AddElement(actions, "Action", "true")
            url = "=""" & "javascript:window.location.href='ReportViews.aspx?srd=11&det=yes&cat2=" & grpfld2 & "&val2=all&cat1=" & grpfld & "&val1=" & """ & Fields!" & grpfld & ".Value  & ""'"""


            AddElement(action, "Hyperlink", url)

            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Sum(" & fn & "(" & "Fields!" & calcfld & ".Value))")  '!!!!!!!!!!!!!!!!!!!
            If fn = "Avg" OrElse fn = "StDev" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            ElseIf fn = "Value" Then
                AddElement(ttltextRun, "Value", "=Round(Sum(" & "Fields!" & calcfld & ".Value),2)")
            Else
                AddElement(ttltextRun, "Value", "=Round(Sum(" & fn & "(" & "Fields!" & calcfld & ".Value)),2)")
            End If
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Normal")
            AddElement(ttlstyle, "Color", "Blue")


            'TablixMember in TablixMembers1
            TablixMember = AddElement(TablixMembers1, "TablixMember", Nothing)
            AddElement(TablixMember, "KeepWithGroup", "Before")
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "1.2in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox14"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Right")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Label", grpfld2)  ' TODO ? change for friendly !!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", Nothing)  ' ? !!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Bold")
            'AddElement(ttlstyle, "Color", "#222222")

            TablixMembers = AddElement(TablixMember, "TablixMembers", Nothing)
            TablixMember = AddElement(TablixMembers, "TablixMember", Nothing)
            TablixHeader = AddElement(TablixMember, "TablixHeader", Nothing)
            AddElement(TablixHeader, "Size", "0.4in")
            CellContents = AddElement(TablixHeader, "CellContents", Nothing)
            'textbox
            txtbox = AddElement(CellContents, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "TextBox25"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            ttlstyle = AddElement(txtbox, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(ttlstyle, "BackgroundColor", "#e9ecee")
            AddElement(ttlstyle, "PaddingLeft", "3pt")
            AddElement(ttlstyle, "PaddingRight", "3pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "By " & grpfld2fr & ":")  '!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "FontFamily", "Verdana")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(ttlstyle, "FontWeight", "Bold")
            'AddElement(ttlstyle, "Color", "#222222")

            AddElement(tablix, "DataSetName", repid)
            AddElement(tablix, "KeepTogether", "true")
            AddElement(tablix, "Top", "1in")
            AddElement(tablix, "Left", "0.3in")
            AddElement(tablix, "Height", "1in")
            AddElement(tablix, "Width", "3in")
            ttlstyle = AddElement(tablix, "Style", Nothing)
            border = AddElement(ttlstyle, "Border", Nothing)
            AddElement(border, "Style", "None")


            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            graf = sw.ToString

            ''----------------------------------------------------------
            ''For testing only, comment this block for production
            'Dim graphpath As String = System.AppDomain.CurrentDomain.BaseDirectory() & "RDLFILES\testingmatrix.rdl"
            ''save to file
            'doc.Save(graphpath)

            'Dim Bytes() As Byte = Encoding.UTF8.GetBytes(graf)
            'Using Stream As New FileStream(graphpath, FileMode.Create)
            '    Stream.Write(Bytes, 0, Bytes.Length)
            '    Stream.Close()
            '    Stream.Dispose()
            'End Using
            '----------------------------------------------------------



            Return graf
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function


    Public Function GenerateDynamicReportOriginal(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal cat1 As String, ByVal cat2 As String, ByVal calcfld As String, ByVal reptype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "") As String
        'NOT IN USE
        Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        Dim rep As String = String.Empty
        Dim ret As String = String.Empty
        Dim n, j, k, m, l As Integer
        Dim showall As Boolean = True     'show all fields 
        Dim fldname As String = String.Empty
        Dim fldexpr As String = String.Empty
        repttl = repttl & " - Details"
        Try
            If dt Is Nothing Then dt = ReturnDataTblFromOURFiles(repid)
            If dt Is Nothing Then
                dt = ReturnDataTblFromXML(xsdfile)
            End If
            If dt Is Nothing OrElse dt.Columns Is Nothing OrElse dt.Columns.Count = 0 Then
                Return "ERROR!! " & rep & " - nothing to create ..."
            End If
            dt = CorrectDatasetColumns(dt)
            Dim dtrf As DataTable = GetReportFields(repid, True)

            n = dt.Columns.Count
            showall = True

            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext")
            If PageFtr = "" Then
                PageFtr = GetReportInfo(repid).Rows(0)("comments")
            End If
            ' Create an XML document
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            Dim attr As XmlAttribute = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            'Dim myconstring As String
            'If myconstr.Trim = "" Then
            '    myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'Else
            '    myconstring = myconstr
            'End If
            'TODO UserConnStr ?
            'myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'AddElement(connectionProperties, "ConnectString", myconstring)
            AddElement(connectionProperties, "ConnectString", "")
            AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", repid)
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "300")
            'Fields element
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            Dim i As Integer
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                'If showall Then
                fldname = dt.Columns(i).Caption
                'Else
                'fldname = dtrf.Rows(i)("Val").ToString
                'End If
                attr.Value = fldname
                AddElement(field, "DataField", fldname)
            Next
            'end of DataSources
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If

            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)

            If n > 5 Then   '!!!! TODO depend of columns widths
                'landscape

                If orien = "landscape" Then
                    AddElement(page, "PageWidth", "11in")
                    AddElement(page, "PageHeight", "8.5in")
                Else
                    AddElement(page, "PageWidth", "8.5in")
                    AddElement(page, "PageHeight", "11in")
                End If

            End If

            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.1in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            ''execution time
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "ExecutionTime"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "0.1in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.2in")
            AddElement(txtbox, "Height", "0.1in")
            AddElement(txtbox, "Width", "1in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.5in")
            AddElement(txtbox, "Height", "0.15in")
            AddElement(txtbox, "Width", "8.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.15in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ''page number
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "PageNumber"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "7.5in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "1in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "14pt")

            AddElement(reptitle, "Height", "0.15in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ' Tablix element
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix1"
            AddElement(tablix, "DataSetName", repid)
            AddElement(tablix, "Top", "0.01in")
            AddElement(tablix, "Left", "0.3in")
            AddElement(tablix, "Height", "0.15in")
            AddElement(tablix, "RepeatColumnHeaders", "true")
            AddElement(tablix, "FixedColumnHeaders", "true")
            AddElement(tablix, "RepeatRowHeaders", "true")
            AddElement(tablix, "FixedRowHeaders", "true")

            Dim tablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            Dim ltablex As Integer = 0
            'TablixColumns element
            Dim tablixColumns As XmlElement = AddElement(tablixBody, "TablixColumns", Nothing)
            Dim tablixColumn As XmlElement
            For i = 0 To n - 1
                tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
                AddElement(tablixColumn, "Width", "1.5in")  'Unit.Pixel(dt.Columns(i).Caption.Length).ToString

            Next
            AddElement(tablix, "Width", (n * 1.5).ToString & "in") 'Unit.Pixel(ltablex).ToString
            'TablixRows element
            Dim tablixRows As XmlElement = AddElement(tablixBody, "TablixRows", Nothing)
            Dim tablixCell As XmlElement
            Dim cellContents As XmlElement
            Dim textbox As XmlElement
            Dim paragraphs As XmlElement
            Dim paragraph As XmlElement
            Dim textRuns As XmlElement
            Dim textRun As XmlElement
            Dim style As XmlElement
            Dim border As XmlElement
            Dim name As String = String.Empty
            Dim colspan As XmlElement
            Dim fldval As String = String.Empty
            Dim fldfriendname As String = String.Empty

            'first tablixRow
            Dim tablixRow As XmlElement = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.15in")
            Dim tablixCells As XmlElement = AddElement(tablixRow, "TablixCells", Nothing)

            'add tablixRows for the group names  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim dtgrp As New DataTable
            dtgrp = GetReportGroups("ddd") 'empty group table
            'Dim col As DataColumn
            'col = New DataColumn
            'col.DataType = System.Type.GetType("System.String")
            'col.ColumnName = "GroupField"
            'dtgrp.Columns.Add(col)
            'col.DataType = System.Type.GetType("System.String")
            'col.ColumnName = "CalcField"
            'dtgrp.Columns.Add(col)
            Dim Row3 As DataRow = dtgrp.NewRow()
            Row3("ReportId") = repid
            Row3("GroupField") = "Overall"
            Row3("CalcField") = calcfld
            Row3("PageBrk") = 1
            Row3("CntChk") = 1
            Row3("CntDistChk") = 1
            Row3("GrpOrder") = 1
            If dt.Columns(calcfld).DataType.Name = "String" OrElse dt.Columns(calcfld).DataType.Name = "DateTime" OrElse calcfld.StartsWith("ID") Then
                Row3("SumChk") = 0
                Row3("MaxChk") = 0
                Row3("MinChk") = 0
                Row3("AvgChk") = 0
                Row3("StDevChk") = 0
            Else
                Row3("SumChk") = 1
                Row3("MaxChk") = 1
                Row3("MinChk") = 1
                Row3("AvgChk") = 1
                Row3("StDevChk") = 1
            End If
            Row3("FirstChk") = 0
            Row3("LastChk") = 0
            Row3("Indx") = 0
            dtgrp.Rows.Add(Row3)

            Dim Row1 As DataRow = dtgrp.NewRow()
            Row1("ReportId") = repid
            Row1("GroupField") = cat1
            Row1("CalcField") = calcfld
            Row1("PageBrk") = 1
            Row1("CntChk") = 1
            Row1("CntDistChk") = 1
            Row1("GrpOrder") = 2
            If dt.Columns(calcfld).DataType.Name = "String" OrElse dt.Columns(calcfld).DataType.Name = "DateTime" OrElse calcfld.StartsWith("ID") Then
                Row1("SumChk") = 0
                Row1("MaxChk") = 0
                Row1("MinChk") = 0
                Row1("AvgChk") = 0
                Row1("StDevChk") = 0
            Else
                Row1("SumChk") = 1
                Row1("MaxChk") = 1
                Row1("MinChk") = 1
                Row1("AvgChk") = 1
                Row1("StDevChk") = 1
            End If
            Row1("FirstChk") = 0
            Row1("LastChk") = 0
            Row1("Indx") = 0
            dtgrp.Rows.Add(Row1)

            Dim Row2 As DataRow = dtgrp.NewRow()
            Row2("ReportId") = repid
            Row2("GroupField") = cat2
            Row2("CalcField") = calcfld
            Row2("PageBrk") = 1
            Row2("CntChk") = 1
            Row2("CntDistChk") = 1
            Row2("GrpOrder") = 3
            If dt.Columns(calcfld).DataType.Name = "String" OrElse dt.Columns(calcfld).DataType.Name = "DateTime" OrElse calcfld.StartsWith("ID") Then
                Row2("SumChk") = 0
                Row2("MaxChk") = 0
                Row2("MinChk") = 0
                Row2("AvgChk") = 0
                Row2("StDevChk") = 0
            Else
                Row2("SumChk") = 1
                Row2("MaxChk") = 1
                Row2("MinChk") = 1
                Row2("AvgChk") = 1
                Row2("StDevChk") = 1
            End If

            Row2("FirstChk") = 0
            Row2("LastChk") = 0
            Row2("Indx") = 0
            dtgrp.Rows.Add(Row2)

            Dim hdrgr As String = String.Empty
            'Dim ovall As Integer = 0
            'If dtgrp.Rows(0)("GroupField").ToString = "Overall" Then ovall = 1
            For j = 0 To dtgrp.Rows.Count - 1
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgrp.Rows(j)("CalcField").ToString)
                If dtgrp.Rows(j)("GroupField") <> "Overall" Then
                    'no duplicates
                    If j = 0 OrElse dtgrp.Rows(j)("GroupField") <> dtgrp.Rows(j - 1)("GroupField") Then
                        grpfrname = GetFriendlyReportGroupName(repid, dtgrp.Rows(j)("GroupField").ToString, dtgrp.Rows(j)("CalcField").ToString)
                        hdrgr = "="" "
                        For k = 0 To j
                            hdrgr = hdrgr & "   "
                        Next
                        hdrgr = hdrgr & grpfrname & "  "" & Fields!" & dtgrp.Rows(j)("GroupField") & ".Value "
                        'add tablexRow for the group friendly name
                        ' TablixCell element (column with group name)
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "GroupHdr" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Left")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", hdrgr)                             '!!!!!!!! 
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "FontStyle", "Italic")
                        AddElement(style, "Color", "White")
                        'AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Left")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        AddElement(style, "BackgroundColor", "DimGray")
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        'add columns and colspan
                        colspan = AddElement(cellContents, "ColSpan", n)
                        For k = 0 To n - 2
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        Next

                        'add tablix row for next with group and last one for columns names
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    End If
                End If
            Next


            'columns names
            For i = 0 To n - 1
                If showall Then
                    fldname = dt.Columns(i).Caption
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                End If
                fldfriendname = GetFriendlyReportFieldName(repid, fldname)

                ' TablixCell element (column names)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                AddElement(textbox, "CanGrow", "true")
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname & "h"     '!!!!
                AddElement(textbox, "KeepTogether", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                style = AddElement(paragraph, "Style", Nothing)
                AddElement(style, "TextAlign", "Center")
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                AddElement(textRun, "Value", fldfriendname)  '!!!!!!!!
                style = AddElement(textRun, "Style", Nothing)
                AddElement(style, "TextDecoration", "Underline")
                AddElement(style, "TextAlign", "Center")
                AddElement(style, "FontWeight", "Bold")
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                style = AddElement(textbox, "Style", Nothing)
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
            Next

            'TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.15In")
            tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

            'columns values
            For i = 0 To n - 1
                'If showall Then
                '    fldname = dt.Columns(i).Caption
                'Else
                '    fldname = dtrf.Rows(i)("Val").ToString
                '    fldexpr = dtrf.Rows(i)("Prop2").ToString
                'End If

                fldname = dtrf.Rows(i)("Val").ToString
                fldexpr = dtrf.Rows(i)("Prop2").ToString

                ' TablixCell element (first cell)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname         '!!!!!!
                'AddElement(textbox, "HideDuplicates", repid)
                AddElement(textbox, "KeepTogether", "true")
                AddElement(textbox, "CanGrow", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                'add expressions
                If fldexpr = "" Then
                    AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                Else
                    AddElement(textRun, "Value", "=" & fldexpr)  '!!!!!!
                End If

                style = AddElement(textRun, "Style", Nothing)
                style = AddElement(textbox, "Style", Nothing)
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")

            Next

            Dim totgrp As String = String.Empty
            Dim totgr As String = String.Empty
            'add groups subtotals rows 
            Dim dtgt As DataTable = dtgrp  'GetReportGroups(repid)
            For i = 0 To dtgt.Rows.Count - 1
                j = dtgt.Rows.Count - 1 - i
                'group statistics title
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgt.Rows(j)("CalcField").ToString)
                If dtgt.Rows(j)("GroupField") = "Overall" Then
                    totgr = "Overall totals Of " & fldfrname 'dtgt.Rows(j)("CalcField").ToString
                Else 'not overall
                    totgr = "=""Subtotals Of " & fldfrname & " For: "
                    For l = 0 To j
                        If dtgt.Rows(l)("GroupField") <> "Overall" AndAlso dtgt.Rows(j)("GroupField") <> dtgt.Rows(l)("GroupField") Then
                            grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(l)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                            totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(l)("GroupField") & ".Value "
                            If l < j Then totgr = totgr & " & "" "
                        End If
                    Next
                    grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(j)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(j)("GroupField") & ".Value "

                End If
                'group title stats rows
                m = dtgt.Rows(j)("CntChk") + dtgt.Rows(j)("SumChk") + dtgt.Rows(j)("MaxChk") + dtgt.Rows(j)("MinChk") + dtgt.Rows(j)("AvgChk") + dtgt.Rows(j)("StDevChk") + dtgt.Rows(j)("CntDistChk") + dtgt.Rows(j)("FirstChk") + dtgt.Rows(j)("LastChk")
                If m > 0 Then  'm stats ordered
                    ' Tablix row for group totals name
                    tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                    AddElement(tablixRow, "Height", "0.15In")
                    tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    ' TablixCell element (group subtotal name)
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = dtgt.Rows(j)("GroupField") & "gr" & j.ToString      '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", totgr)        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    'AddElement(style, "TextDecoration", "Underline")
                    AddElement(style, "TextAlign", "Left")
                    AddElement(style, "FontWeight", "Bold")
                    style = AddElement(textbox, "Style", Nothing)
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "TopBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "BottomBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "LeftBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "RightBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    'add columns and colspan
                    colspan = AddElement(cellContents, "ColSpan", n)
                    'add tablix row for each stat with columns and colspan
                    For k = 0 To n - 2
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    Next

                    ' Tablix rows for stats
                    If n < 6 Then  'less than 6 columns for stats - add  6 rows
                        For k = 0 To 8  'stats 5
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'add row
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.15In")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"  '!!!!!!!!!!!!!!!!!!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "Max(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "Min(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "Last(" & fldval & ")"
                                End If
                                fldval = " & " & fldval
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Count:  """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Sum:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Max:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Min:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Avg:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""StDev:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""CntDist:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""First:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Last:   """ & fldval)
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            Else 'no stats for the column
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.15in")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                                ' TablixCell element (stat subtotal name) - column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "K" & k.ToString & "L" & l.ToString & "J" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                'AddElement(textRun, "Value", " ")   '!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last: ")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            End If
                        Next
                    Else  'at least 8 columns for stats - add 2 rows
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        For k = 0 To 8  '8 stats names in one row
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then  'stat ordered
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) 
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "sth" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last:")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "sthe" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If

                        'second row with stats values
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                        For k = 0 To 8  '8 stats 
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"
                                If dtgt.Columns(k + 3).Caption = "CntChk" Then
                                    fldval = "=Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption = "SumChk" Then
                                    fldval = "=Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption = "MaxChk" Then
                                    fldval = "=Max(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption = "MinChk" Then
                                    fldval = "=Min(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption = "AvgChk" Then
                                    fldval = "=FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption = "StDevChk" Then
                                    fldval = "=FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption = "CntDistChk" Then
                                    fldval = "=CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption = "FirstChk" Then
                                    fldval = "=First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption = "LastChk" Then
                                    fldval = "=Last(" & fldval & ")"
                                End If
                                AddElement(textRun, "Value", fldval)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "stve" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If
                    End If
                End If
            Next

            'End of TablixBody

            'TablixColumnHierarchy element
            Dim tablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim tablixMembers As XmlElement = AddElement(tablixColumnHierarchy, "TablixMembers", Nothing)
            For i = 0 To n - 1
                AddElement(tablixMembers, "TablixMember", Nothing)
            Next

            'TablixRowHierarchy element
            Dim tablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim tablixMember As XmlElement
            Dim tablixMemberGrp As XmlElement
            Dim tablixMemberStat As XmlElement
            Dim group As XmlElement
            Dim groupexpressions As XmlElement
            Dim groupexpression As XmlElement
            Dim pagebreak As XmlElement
            Dim sortexpressions As XmlElement
            Dim sortexpression As XmlElement
            Dim value As XmlElement
            Dim grpnm As String = String.Empty
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", Nothing)
            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            ' AddElement(tablixMember, "KeepWithGroup", "After")
            'AddElement(tablixMember, "RepeatOnNewPage", "true")
            AddElement(tablixMember, "KeepTogether", "true")
            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            'groups
            Dim ovrall As Integer = 0
            Dim dtgr As DataTable = dtgrp 'GetReportGroups(repid)
            If dtgr.Rows.Count > 0 Then
                grpnm = ""
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If
                    Else
                        Exit For
                    End If
                Next

                For i = 0 To dtgr.Rows.Count - 1  'loop for groups not Overall
                    If dtgr.Rows(i)("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dtgr.Rows(i)("GroupField").ToString & "grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If

                        'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then

                            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                                pagebreak = AddElement(group, "PageBreak", Nothing)
                                AddElement(pagebreak, "BreakLocation", "End")
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            'add tablexMember for group name
                            'For j = 0 To i - ovrall
                            'If j = 0 OrElse dtgr.Rows(j)("GroupField").ToString <> dtgr.Rows(j - 1)("GroupField").ToString Then
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            'End If
                            'Next

                            'add one more in the end... 
                            If dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            End If

                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                        End If

                        'if last deepest group
                        If i = dtgr.Rows.Count - 1 Then
                            tablixMember = DetailTablixMember(doc, tablixMember)
                        End If

                        'stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then

                            'row for subtotal name
                            tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            'rows with stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If



                    End If
                Next

            Else  'no groups
                'detail
                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMember, "DataElementName", "Detail_Collection")
                AddElement(tablixMember, "DataElementOutput", "Output")
                AddElement(tablixMember, "KeepTogether", "true")
                group = AddElement(tablixMember, "Group", Nothing)
                attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = "Table1_Details_Group"
                AddElement(group, "DataElementName", "Detail")
                Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
                AddElement(tablixMembersNested, "TablixMember", Nothing)
            End If

            'Save XML document in OURFiles
            ''flash to string
            Dim rdlstr As String = String.Empty
            Dim rdlfile As String = String.Empty
            Dim userupl As String = String.Empty
            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            rdlstr = sw.ToString

            ''TEMPORARY
            'Dim er As String = String.Empty
            ''Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            'rep = applpath & "Temp\" & repid & ".rdl"
            'doc.Save(rep)
            ''Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            ''If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
            ''    'read rdl from OURFiles
            ''    rdlfile = dv.Table.Rows(0)("Path").ToString  'string with file path
            ''    userupl = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
            ''End If
            ''If userupl.Trim = "" Then
            ''    er = SaveRDLstringInOURFiles(repid, rdlstr)  'just generated
            ''Else
            ''    Try
            ''        userupl = userupl.Replace("uploaded by", "").Trim
            ''        rdlfile = rdlfile.Replace("|", "\")  'in MySql file path does not saved properly !!!!
            ''        'copy user uploaded earlier rdl
            ''        Dim userrdl As String = rdlfile.Replace(".rdl", "UpldByUser" & userupl & ".rdl")
            ''        If File.Exists(userrdl) Then
            ''            File.Delete(userrdl)
            ''        End If
            ''        File.Copy(rdlfile, userrdl)  'save user rdl with new name
            ''        er = SaveRDLstringInOURFiles(repid, rdlstr, " ", " ")  'just generated but previously was uploaded by user
            ''    Catch ex As Exception
            ''        er = "ERROR!! " & ex.Message
            ''    End Try
            ''End If
            ''If er = "Query executed fine." Then
            ''    'delete file from RDLFILES folder if it is not user created rdl!!
            ''    If File.Exists(rep) Then
            ''        File.Delete(rep)
            ''        rep = "Report format generated."
            ''    End If
            ''Else
            ''    doc.Save(rep)
            ''End If
            ''TODO Remove it. Save RDL XML document to file temporary for now
            ''doc.Save(rep)
            Return rdlstr
        Catch ex As Exception
            ret = "ERROR!! " & rep & " - " & ex.Message
        End Try
        Return ret
    End Function
    Public Function GenerateDynamicReportForGroups(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal reptype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "", Optional ByVal expand1 As Boolean = True, Optional ByVal expand2 As Boolean = False, Optional ByVal showall As Boolean = True, Optional ByVal GroupTable As DataTable = Nothing, Optional ByVal showdetails As Boolean = True) As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        Dim ret As String = String.Empty
        Dim n, i, j, k, m, l As Integer
        'Dim showall As Boolean = True     'show all fields 
        Dim fldname As String = String.Empty
        Dim fldexpr As String = String.Empty
        repttl = repttl & " - Groups Statistics"
        If params.Trim <> "" Then repttl = repttl & " for:  " & params
        Try
            If dt Is Nothing Then dt = ReturnDataTblFromOURFiles(repid)
            If dt Is Nothing Then
                dt = ReturnDataTblFromXML(xsdfile)
            End If
            If dt Is Nothing OrElse dt.Columns Is Nothing OrElse dt.Columns.Count = 0 Then
                Return "ERROR!! " & repid & " - nothing to create ..."
            End If
            dt = CorrectDatasetColumns(dt)
            Dim dtrf As DataTable = GetReportFields(repid, True)

            If showall Then
                n = dt.Columns.Count
            Else
                n = dtrf.Rows.Count
            End If

            'showall = True as default

            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext").ToString
            If PageFtr = "" Then
                PageFtr = GetReportInfo(repid).Rows(0)("comments").ToString
            End If
            ' Create an XML document
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            Dim attr As XmlAttribute = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            'Dim myconstring As String
            'If myconstr.Trim = "" Then
            '    myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'Else
            '    myconstring = myconstr
            'End If
            'TODO UserConnStr ?
            'myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'AddElement(connectionProperties, "ConnectString", myconstring)
            AddElement(connectionProperties, "ConnectString", "")
            AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", repid)
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "300")
            'Fields element
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                'If showall Then
                fldname = dt.Columns(i).Caption
                'Else
                '    fldname = dtrf.Rows(i)("Val").ToString
                'End If
                attr.Value = fldname
                AddElement(field, "DataField", fldname)
            Next
            'end of DataSources
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If

            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)

            If n > 5 Then   '!!!! TODO depend of columns widths
                'landscape

                If orien = "landscape" Then
                    AddElement(page, "PageWidth", "11in")
                    AddElement(page, "PageHeight", "8.5in")
                Else
                    AddElement(page, "PageWidth", "8.5in")
                    AddElement(page, "PageHeight", "11in")
                End If

            End If

            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.1in")
            AddElement(pageheader, "PrintOnFirstPage", "false")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            ''execution time
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "ExecutionTime"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "0.1in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.2in")
            AddElement(txtbox, "Height", "0.1in")
            AddElement(txtbox, "Width", "8in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            Dim npv As String = "Globals!PageNumber" & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", "=" & npv)
            'AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.5in")
            AddElement(txtbox, "Height", "0.15in")
            AddElement(txtbox, "Width", "8.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.15in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ''page number
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "PageNumber"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "7.5in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "1in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "12pt")

            AddElement(reptitle, "Height", "0.15in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ' Tablix element
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix1"
            AddElement(tablix, "DataSetName", repid)
            AddElement(tablix, "Top", "0.01in")
            AddElement(tablix, "Left", "0.3in")
            AddElement(tablix, "Height", "0.15in")
            AddElement(tablix, "RepeatColumnHeaders", "true")
            AddElement(tablix, "FixedColumnHeaders", "true")
            AddElement(tablix, "RepeatRowHeaders", "true")
            AddElement(tablix, "FixedRowHeaders", "true")

            Dim tablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            Dim ltablex As Integer = 0
            'TablixColumns element
            Dim tablixColumns As XmlElement = AddElement(tablixBody, "TablixColumns", Nothing)
            Dim tablixColumn As XmlElement
            'toggle column
            tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
            AddElement(tablixColumn, "Width", "0.5in")
            For i = 0 To n - 1
                tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
                AddElement(tablixColumn, "Width", "1.5in")  'Unit.Pixel(dt.Columns(i).Caption.Length).ToString
            Next
            AddElement(tablix, "Width", (n * 1.5 + 0.5).ToString & "in") 'Unit.Pixel(ltablex).ToString
            'TablixRows element
            Dim tablixRows As XmlElement = AddElement(tablixBody, "TablixRows", Nothing)
            Dim tablixCell As XmlElement
            Dim cellContents As XmlElement
            Dim textbox As XmlElement
            Dim paragraphs As XmlElement
            Dim paragraph As XmlElement
            Dim textRuns As XmlElement
            Dim textRun As XmlElement
            Dim style As XmlElement
            Dim border As XmlElement
            Dim name As String = String.Empty
            Dim colspan As XmlElement
            Dim fldval As String = String.Empty
            Dim fldfriendname As String = String.Empty

            'first tablixRow
            Dim tablixRow As XmlElement = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.15in")
            Dim tablixCells As XmlElement = AddElement(tablixRow, "TablixCells", Nothing)

            'add tablixRows for the group names  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim dtgrp As New DataTable
            If GroupTable Is Nothing OrElse GroupTable.Rows.Count = 0 Then
                dtgrp = GetReportGroups(repid) 'group table
            Else
                dtgrp = GroupTable
            End If


            'only for this report: add overall row if no any overalls
            If dtgrp.Rows(0)("GroupField") <> "Overall" Then
                Dim calcfield = dtgrp.Rows(0)("CalcField")
                Dim row As DataRow = dtgrp.NewRow
                row("ReportId") = repid
                row("GroupField") = "Overall"
                row("CalcField") = calcfield
                row("CntChk") = dtgrp.Rows(0)("CntChk")
                row("SumChk") = dtgrp.Rows(0)("SumChk")
                row("MaxChk") = dtgrp.Rows(0)("MaxChk")
                row("MinChk") = dtgrp.Rows(0)("MinChk")
                row("AvgChk") = dtgrp.Rows(0)("AvgChk")
                row("StDevChk") = dtgrp.Rows(0)("StDevChk")
                row("CntDistChk") = dtgrp.Rows(0)("CntDistChk")
                row("FirstChk") = dtgrp.Rows(0)("FirstChk")
                row("LastChk") = dtgrp.Rows(0)("LastChk")
                row("PageBrk") = 0
                row("Indx") = 0
                row("GrpOrder") = 0
                dtgrp.Rows.Add(row)
                For i = 0 To dtgrp.Rows.Count - 1
                    dtgrp.Rows(i)("GrpOrder") = dtgrp.Rows(i)("GrpOrder") + 1
                Next
                dtgrp.DefaultView.Sort = "GrpOrder ASC"
                Dim dtgrpsorted As DataTable = dtgrp.DefaultView.ToTable
                dtgrp = dtgrpsorted
            End If

            'make a field with group number and number in group
            Dim col = New DataColumn
            col.DataType = System.Type.GetType("System.Int32")
            col.ColumnName = "GroupNum"
            dtgrp.Columns.Add(col)
            col = New DataColumn
            col.DataType = System.Type.GetType("System.Int32")
            col.ColumnName = "NumInGroup"
            dtgrp.Columns.Add(col)

            Dim lastgrpnum As Integer = 0
            Dim num As Integer = 0 'group number
            If dtgrp.Rows(0)("GroupField") = "Overall" Then
                num = 0
            Else
                num = 1
            End If
            dtgrp.Rows(0)("GroupNum") = num
            dtgrp.Rows(0)("NumInGroup") = 1
            For i = 1 To dtgrp.Rows.Count - 1
                If dtgrp.Rows(i)("GroupField") = dtgrp.Rows(i - 1)("GroupField") Then
                    dtgrp.Rows(i)("GroupNum") = dtgrp.Rows(i - 1)("GroupNum")
                    dtgrp.Rows(i)("NumInGroup") = dtgrp.Rows(i - 1)("NumInGroup") + 1
                Else
                    num = num + 1
                    dtgrp.Rows(i)("GroupNum") = num
                    dtgrp.Rows(i)("NumInGroup") = 1
                End If
            Next
            lastgrpnum = num


            Dim hdrgr As String = String.Empty
            'Dim ovall As Integer = 0
            'If dtgrp.Rows(0)("GroupField").ToString = "Overall" Then ovall = 1
            For j = 0 To dtgrp.Rows.Count - 1
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgrp.Rows(j)("CalcField").ToString)
                If dtgrp.Rows(j)("GroupField") <> "Overall" Then
                    'no duplicates
                    If j = 0 OrElse dtgrp.Rows(j)("GroupField") <> dtgrp.Rows(j - 1)("GroupField") Then
                        grpfrname = GetFriendlyReportGroupName(repid, dtgrp.Rows(j)("GroupField").ToString, dtgrp.Rows(j)("CalcField").ToString)
                        hdrgr = "="" "
                        For k = 0 To j
                            hdrgr = hdrgr & "   "
                        Next
                        hdrgr = hdrgr & grpfrname & "  "" & Fields!" & dtgrp.Rows(j)("GroupField") & ".Value "
                        'add tablexRow for the group friendly name

                        'toggle cell
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "Toggle" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Center")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", "")  '!!!!!!!!
                        style = AddElement(textRun, "Style", Nothing)
                        'AddElement(style, "TextDecoration", "")
                        AddElement(style, "TextAlign", "Center")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        If j = dtgrp.Rows.Count - 1 Then
                            AddElement(style, "BackgroundColor", "#CCCCCC")
                        ElseIf j = 0 Then
                            AddElement(style, "BackgroundColor", "DimGray")
                        Else
                            l = j
                            If l > 0 AndAlso dtgrp.Rows(l)("GroupField") = dtgrp.Rows(l - 1)("GroupField") Then
                                l = j - 1
                            End If
                            AddElement(style, "BackgroundColor", "#" & (l * 10).ToString.Substring(0, 2) & (l * 20).ToString.Substring(0, 2) & (l * 10).ToString.Substring(0, 2))
                        End If
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")

                        ' TablixCell element (column with group name)
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "GroupHdr" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Left")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", hdrgr)                             '!!!!!!!! 
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "FontStyle", "Italic")
                        AddElement(style, "Color", "White")
                        'AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Left")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)

                        If j = dtgrp.Rows.Count - 1 Then
                            AddElement(style, "BackgroundColor", "#CCCCCC")
                        ElseIf j = 0 Then
                            AddElement(style, "BackgroundColor", "DimGray")
                        Else
                            l = j
                            If l > 0 AndAlso dtgrp.Rows(l)("GroupField") = dtgrp.Rows(l - 1)("GroupField") Then
                                l = j - 1
                            End If
                            AddElement(style, "BackgroundColor", "#" & (l * 10).ToString.Substring(0, 2) & (l * 20).ToString.Substring(0, 2) & (l * 10).ToString.Substring(0, 2))
                        End If
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        'add columns and colspan
                        colspan = AddElement(cellContents, "ColSpan", n)
                        For k = 0 To n - 2
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        Next

                        'add tablix row for next with group and last one for columns names
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    End If
                End If
            Next

            'empty cell
            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
            cellContents = AddElement(tablixCell, "CellContents", Nothing)
            textbox = AddElement(cellContents, "Textbox", Nothing)
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "txtboxempty1"
            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
            style = AddElement(paragraph, "Style", Nothing)
            AddElement(style, "TextAlign", "Center")
            textRuns = AddElement(paragraph, "TextRuns", Nothing)
            textRun = AddElement(textRuns, "TextRun", Nothing)
            AddElement(textRun, "Value", "")  '!!!!!!!!
            style = AddElement(textRun, "Style", Nothing)
            AddElement(style, "TextDecoration", "Underline")
            AddElement(style, "TextAlign", "Center")
            AddElement(style, "FontWeight", "Bold")
            AddElement(style, "PaddingLeft", "2pt")
            AddElement(style, "PaddingRight", "2pt")
            AddElement(style, "PaddingTop", "2pt")
            AddElement(style, "PaddingBottom", "2pt")
            style = AddElement(textbox, "Style", Nothing)
            'border
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "TopBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "BottomBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "LeftBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "RightBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")

            'columns names
            For i = 0 To n - 1
                If showall Then
                    fldname = dt.Columns(i).Caption
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                End If
                fldfriendname = GetFriendlyReportFieldName(repid, fldname)

                ' TablixCell element (column names)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                AddElement(textbox, "CanGrow", "true")
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname & "h"     '!!!!
                AddElement(textbox, "KeepTogether", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                style = AddElement(paragraph, "Style", Nothing)
                AddElement(style, "TextAlign", "Center")
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                AddElement(textRun, "Value", fldfriendname)  '!!!!!!!!
                style = AddElement(textRun, "Style", Nothing)
                AddElement(style, "TextDecoration", "Underline")
                AddElement(style, "TextAlign", "Center")
                AddElement(style, "FontWeight", "Bold")
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                style = AddElement(textbox, "Style", Nothing)
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
            Next

            'TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.15in")
            tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

            'empty cell
            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
            cellContents = AddElement(tablixCell, "CellContents", Nothing)
            textbox = AddElement(cellContents, "Textbox", Nothing)
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "txtboxempty2"
            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
            style = AddElement(paragraph, "Style", Nothing)
            AddElement(style, "TextAlign", "Center")
            textRuns = AddElement(paragraph, "TextRuns", Nothing)
            textRun = AddElement(textRuns, "TextRun", Nothing)
            AddElement(textRun, "Value", "")  '!!!!!!!!
            style = AddElement(textRun, "Style", Nothing)
            AddElement(style, "TextDecoration", "Underline")
            AddElement(style, "TextAlign", "Center")
            AddElement(style, "FontWeight", "Bold")
            AddElement(style, "PaddingLeft", "2pt")
            AddElement(style, "PaddingRight", "2pt")
            AddElement(style, "PaddingTop", "2pt")
            AddElement(style, "PaddingBottom", "2pt")
            style = AddElement(textbox, "Style", Nothing)
            'border
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "TopBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "BottomBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "LeftBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "RightBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")

            'columns values
            For i = 0 To n - 1
                If showall Then
                    fldname = dt.Columns(i).Caption
                    'find field expression
                    dtrf.DefaultView.RowFilter = "Val='" & fldname & "'"
                    If Not dtrf.DefaultView.ToTable Is Nothing Then
                        Dim dtfex As DataTable = dtrf.DefaultView.ToTable
                        If dtfex.Rows.Count > 0 Then fldexpr = dtfex.Rows(0)("Prop2").ToString
                        dtrf.DefaultView.RowFilter = ""
                    End If
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                    fldexpr = dtrf.Rows(i)("Prop2").ToString
                End If

                'fldname = dtrf.Rows(i)("Val").ToString
                'fldexpr = dtrf.Rows(i)("Prop2").ToString

                ' TablixCell element (first cell)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname         '!!!!!!
                'AddElement(textbox, "HideDuplicates", repid)
                AddElement(textbox, "KeepTogether", "true")
                AddElement(textbox, "CanGrow", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                'add expressions
                If fldexpr = "" Then
                    'if numeric field
                    If ColumnTypeIsNumeric(dt.Columns(fldname)) Then
                        AddElement(textRun, "Value", "=Round(Fields!" & fldname & ".Value,2)")  '!!!!!!
                    Else
                        'not numeric 
                        AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                    End If
                Else
                    AddElement(textRun, "Value", "=" & fldexpr)  '!!!!!!
                End If

                style = AddElement(textRun, "Style", Nothing)
                style = AddElement(textbox, "Style", Nothing)
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")

            Next

            Dim totgrp As String = String.Empty
            Dim totgr As String = String.Empty
            'add groups subtotals rows 
            Dim dtgt As DataTable = dtgrp  'GetReportGroups(repid)
            For i = 0 To dtgt.Rows.Count - 1
                j = dtgt.Rows.Count - 1 - i
                'group statistics title
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgt.Rows(j)("CalcField").ToString)
                If dtgt.Rows(j)("GroupField") = "Overall" Then
                    totgr = "Overall totals of " & fldfrname 'dtgt.Rows(j)("CalcField").ToString

                Else 'not overall
                    totgr = "=""Subtotals for group {"
                    For l = 0 To j
                        If dtgt.Rows(l)("GroupField") <> "Overall" AndAlso dtgt.Rows(j)("GroupField") <> dtgt.Rows(l)("GroupField") Then
                            grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(l)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                            'If l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField") Then
                            If l = 0 OrElse (l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField")) Then
                                totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(l)("GroupField") & ".Value "
                                If l < j Then totgr = totgr & " & "" "
                            End If
                        End If
                    Next
                    grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(j)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(j)("GroupField") & ".Value "

                    totgr = totgr & " & ""  } of  " & fldfrname & ": "" "

                End If
                'group title stats rows
                m = dtgt.Rows(j)("CntChk") + dtgt.Rows(j)("SumChk") + dtgt.Rows(j)("MaxChk") + dtgt.Rows(j)("MinChk") + dtgt.Rows(j)("AvgChk") + dtgt.Rows(j)("StDevChk") + dtgt.Rows(j)("CntDistChk") + dtgt.Rows(j)("FirstChk") + dtgt.Rows(j)("LastChk")
                If m > 0 Then  'm stats ordered
                    ' Tablix row for group totals name
                    tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                    AddElement(tablixRow, "Height", "0.15In")
                    tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                    'empty cell
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = "txtbox3" & "e" & j.ToString
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", "Center")
                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", "")  '!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "TextDecoration", "Underline")
                    AddElement(style, "Color", "White")
                    AddElement(style, "TextAlign", "Center")
                    AddElement(style, "FontWeight", "Bold")
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    style = AddElement(textbox, "Style", Nothing)

                    If j = dtgt.Rows.Count - 1 Then
                        AddElement(style, "BackgroundColor", "#CCCCCC")
                    ElseIf j = 0 OrElse dtgt.Rows(j)("GroupField") = "Overall" Then
                        AddElement(style, "BackgroundColor", "DimGray")
                    Else
                        l = j
                        If l > 0 AndAlso dtgt.Rows(l)("GroupField") = dtgt.Rows(l - 1)("GroupField") Then
                            l = j - 1
                            If l = 0 Then
                                AddElement(style, "BackgroundColor", "DimGray")
                            Else
                                AddElement(style, "BackgroundColor", "#" & (l * 10).ToString.Substring(0, 2) & (l * 20).ToString.Substring(0, 2) & (l * 10).ToString.Substring(0, 2))
                            End If
                        ElseIf l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField") Then
                            AddElement(style, "BackgroundColor", "#" & (l * 10).ToString.Substring(0, 2) & (l * 20).ToString.Substring(0, 2) & (l * 10).ToString.Substring(0, 2))
                        End If
                    End If

                    'border
                    border = AddElement(style, "Border", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "TopBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "BottomBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "LeftBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "RightBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")

                    ' TablixCell element (group subtotal name)
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = dtgt.Rows(j)("GroupField") & "gr" & j.ToString      '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", totgr)        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "Color", "White")
                    'AddElement(style, "TextDecoration", "Underline")
                    AddElement(style, "TextAlign", "Left")
                    AddElement(style, "FontWeight", "Bold")
                    style = AddElement(textbox, "Style", Nothing)
                    If j = dtgt.Rows.Count - 1 Then
                        AddElement(style, "BackgroundColor", "#CCCCCC")
                    ElseIf j = 0 OrElse dtgt.Rows(j)("GroupField") = "Overall" Then
                        AddElement(style, "BackgroundColor", "DimGray")
                    Else
                        l = j
                        If l > 0 AndAlso dtgt.Rows(l)("GroupField") = dtgt.Rows(l - 1)("GroupField") Then
                            l = j - 1
                            If l = 0 Then
                                AddElement(style, "BackgroundColor", "DimGray")
                            Else
                                AddElement(style, "BackgroundColor", "#" & (l * 10).ToString.Substring(0, 2) & (l * 20).ToString.Substring(0, 2) & (l * 10).ToString.Substring(0, 2))
                            End If
                        ElseIf l > 0 AndAlso dtgt.Rows(l)("GroupField") <> dtgt.Rows(l - 1)("GroupField") Then
                            AddElement(style, "BackgroundColor", "#" & (l * 10).ToString.Substring(0, 2) & (l * 20).ToString.Substring(0, 2) & (l * 10).ToString.Substring(0, 2))
                        End If
                    End If
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "TopBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "BottomBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "LeftBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "RightBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")

                    'add columns and colspan
                    colspan = AddElement(cellContents, "ColSpan", n)
                    'add tablix row for each stat with columns and colspan
                    For k = 0 To n - 2
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    Next

                    ' Tablix rows for stats
                    If n < 6 Then  'less than 6 columns for stats - add  6 rows
                        For k = 0 To 8  'stats 5
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'add row
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.15In")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                'empty cell
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = "txtbox4e" & k.ToString & "empty" & j.ToString
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                AddElement(textRun, "Value", "")  '!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "FontWeight", "Bold")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                style = AddElement(textbox, "Style", Nothing)
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")

                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"  '!!!!!!!!!!!!!!!!!!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "Last(" & fldval & ")"
                                End If
                                fldval = " & " & fldval
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Count:  """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Sum:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Max:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Min:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Avg:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""StDev:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""CntDist:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""First:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Last:   """ & fldval)
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            Else 'no stats for the column
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.15in")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                'empty cell
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = "txtbox5e" & "e" & k.ToString & "e" & l.ToString & "e" & j.ToString
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                AddElement(textRun, "Value", "")  '!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "FontWeight", "Bold")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                style = AddElement(textbox, "Style", Nothing)
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")


                                ' TablixCell element (stat subtotal name) - column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "K" & k.ToString & "L" & l.ToString & "J" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                'AddElement(textRun, "Value", " ")   '!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last: ")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            End If
                        Next
                    Else  'at least 8 columns for stats - add 2 rows
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                        'empty cell
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "txtbox6e" & "e" & j.ToString
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Center")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", "")  '!!!!!!!!
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Center")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")

                        For k = 0 To 8  '8 stats names in one row
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then  'stat ordered
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) 
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "sth" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last:")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "sthe" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If

                        'second row with stats values
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                        'empty cell
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "txtbox7e" & "e" & j.ToString
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Center")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", "")  '!!!!!!!!
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Center")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")

                        For k = 0 To 8  '8 stats 
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "=Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "=Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "=Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "=Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "=FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "=FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "=CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "=First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "=Last(" & fldval & ")"
                                End If
                                AddElement(textRun, "Value", fldval)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "stve" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If
                    End If
                End If
            Next

            'End of TablixBody

            'TablixColumnHierarchy element
            Dim tablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim tablixMembers As XmlElement = AddElement(tablixColumnHierarchy, "TablixMembers", Nothing)
            For i = 0 To n
                AddElement(tablixMembers, "TablixMember", Nothing)
            Next

            'TablixRowHierarchy element
            Dim tablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim tablixMember As XmlElement
            Dim tablixMemberGrp As XmlElement
            Dim tablixMemberStat As XmlElement
            Dim group As XmlElement
            Dim groupexpressions As XmlElement
            Dim groupexpression As XmlElement
            Dim pagebreak As XmlElement
            Dim sortexpressions As XmlElement
            Dim sortexpression As XmlElement
            Dim visibility As XmlElement
            Dim value As XmlElement
            Dim grpnm As String = String.Empty
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", Nothing)
            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            ' AddElement(tablixMember, "KeepWithGroup", "After")
            'AddElement(tablixMember, "RepeatOnNewPage", "true")
            AddElement(tablixMember, "KeepTogether", "true")
            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            'groups
            Dim ovrall As Integer = 0
            Dim dtgr As DataTable = dtgrp  ' GetReportGroups(repid)

            '-----------------------------------------  no groups  OK! '--------------------------------------------------------------------------------------
            If dtgr.Rows.Count = 0 Then
                'no groups OK
                AddElement(tablixMember, "KeepWithGroup", "After")
                AddElement(tablixMember, "RepeatOnNewPage", "true")
                'detail
                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMember, "DataElementName", "Detail_Collection")
                AddElement(tablixMember, "DataElementOutput", "Output")
                AddElement(tablixMember, "KeepTogether", "true")
                group = AddElement(tablixMember, "Group", Nothing)
                attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = "Table1_Details_Group"
                AddElement(group, "DataElementName", "Detail")
                Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
                AddElement(tablixMembersNested, "TablixMember", Nothing)


                '-----------------------------------------  not overall one or two different groups  OK! '--------------------------------------------------------------------------------------
            ElseIf dtgr.Rows(0)("GroupField").ToString <> "Overall" AndAlso (dtgr.Rows.Count = 1 OrElse (dtgr.Rows.Count = 2 AndAlso (dtgr.Rows(1)("GroupField").ToString <> dtgr.Rows(0)("GroupField").ToString))) Then
                'OK!  one group not overall , second if exists not first one (Show Report ok, Groups Stats is ok)
                'from Dynamic report
                grpnm = ""
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If
                    Else
                        Exit For
                    End If
                Next

                For i = 0 To dtgr.Rows.Count - 1  'loop for groups not Overall
                    If dtgr.Rows(i)("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "Grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dtgr.Rows(i)("GroupField").ToString & "Grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If

                        'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then

                            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            'If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                            '    pagebreak = AddElement(group, "PageBreak", Nothing)
                            '    AddElement(pagebreak, "BreakLocation", "End")
                            'End If
                            If i > 1 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                If expand1 + expand2 Then
                                    AddElement(visibility, "Hidden", "false")
                                Else
                                    AddElement(visibility, "Hidden", "true")
                                End If
                                AddElement(visibility, "ToggleItem", "Toggle" & (i - 1).ToString)
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            'add tablexMember for group name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            If i > 1 Then
                                visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                If expand1 + expand2 Then
                                    AddElement(visibility, "Hidden", "false")
                                Else
                                    AddElement(visibility, "Hidden", "true")
                                End If
                                AddElement(visibility, "ToggleItem", "Toggle" & (i - 1).ToString)
                            End If

                            'add one more in the end for column headers... 
                            If dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                                visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                AddElement(visibility, "Hidden", "true")
                                AddElement(visibility, "ToggleItem", "Toggle" & (dtgr.Rows.Count - 1).ToString)
                            Else
                                If i > 1 Then
                                    visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                    AddElement(visibility, "Hidden", "true")
                                    AddElement(visibility, "ToggleItem", "Toggle" & (i - 1).ToString)
                                End If
                            End If

                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                        End If

                        'if last deepest group
                        If i = dtgr.Rows.Count - 1 Then

                            If expand1 AndAlso expand2 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                AddElement(visibility, "Hidden", "false")             '!!!! trying to show column headers
                                AddElement(visibility, "ToggleItem", "Toggle" & (dtgr.Rows.Count - 1).ToString)
                                tablixMember = DetailTablixMember(doc, tablixMember, 1)
                            ElseIf Not expand2 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                AddElement(visibility, "Hidden", "true")
                                AddElement(visibility, "ToggleItem", "Toggle" & (dtgr.Rows.Count - 1).ToString)
                                tablixMember = DetailTablixMember(doc, tablixMember, 0)
                            End If

                        End If

                        'stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then

                            'row for subtotal name
                            tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            'rows with stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If



                    End If
                Next



                '-----------------------------------------  only group overall  '--------------------------------------------------------------------------------------

            ElseIf dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString = "Overall" AndAlso dtgr.Rows(0)("GroupField").ToString = "Overall" Then
                'only overall group (Show is OK, Groups Stats crashed)

                For i = 0 To dtgr.Rows.Count - 1  'loop for groups Overall
                    ovrall = ovrall + 1
                    'overall stats
                    If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                        'Overall name
                        tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                        AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                        'rows with overall stats

                        If n < 6 Then
                            For l = 0 To 8
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            Next
                        Else
                            For l = 0 To 1
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            Next
                        End If

                    End If

                Next

                'last deepest group
                tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                visibility = AddElement(tablixMember, "Visibility", Nothing)
                AddElement(visibility, "Hidden", "true")
                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")

                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember
                tablixMember = DetailTablixMember(doc, tablixMember)
                visibility = AddElement(tablixMember, "Visibility", Nothing)
                AddElement(visibility, "Hidden", "true")


                '    '-----------------------------------------  first group overall, second and other groups not overall OK! '--------------------------------------------------------------------------------------

            ElseIf lastgrpnum > 2 OrElse (lastgrpnum = 2 AndAlso dtgr.Rows(0)("GroupField").ToString = "Overall") Then 'more than 2 different groups 

                For i = 0 To dtgr.Rows.Count - 1  'loop for groups Overall
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats

                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If

                        End If
                    Else
                        Exit For
                    End If
                Next

                Dim grpnum As Integer = 0
                lastgrpnum = 0
                grpnm = ""
                For i = 0 To dtgr.Rows.Count - 1  'loop for groups not Overall
                    If dtgr.Rows(i)("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "Grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dtgr.Rows(i)("GroupField").ToString & "Grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If


                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then

                            'Find grpnum of previous group
                            For j = 0 To dtgr.Rows.Count - 1
                                If i > 0 Then
                                    If dtgr.Rows(i - 1)("GroupField").ToString = "Overall" Then
                                        Exit For
                                    End If
                                    If dtgr.Rows(j)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString Then
                                        grpnum = j
                                        Exit For
                                    End If
                                End If
                            Next

                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            'If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                            '    pagebreak = AddElement(group, "PageBreak", Nothing)
                            '    AddElement(pagebreak, "BreakLocation", "End")
                            'End If
                            If grpnum > 0 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                'If showdetails Then
                                '    AddElement(visibility, "Hidden", "false")
                                'Else
                                '    AddElement(visibility, "Hidden", "true")
                                'End If
                                AddElement(visibility, "Hidden", "true")
                                AddElement(visibility, "ToggleItem", "Toggle" & grpnum.ToString)
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            'add tablexMember for group name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            If grpnum > 0 Then

                                visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                'If showdetails Then
                                '    AddElement(visibility, "Hidden", "false")
                                'Else
                                '    AddElement(visibility, "Hidden", "true")
                                'End If
                                AddElement(visibility, "Hidden", "true")
                                AddElement(visibility, "ToggleItem", "Toggle" & grpnum.ToString)
                            End If

                            'add one more in the end for column headers... ... last group
                            If dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then
                                'If i = dtgr.Rows.Count - 1 Then
                                If lastgrpnum = 0 Then
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                                    AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                                    If grpnum > 0 Then
                                        visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                        'If showdetails Then
                                        '    AddElement(visibility, "Hidden", "false")
                                        'Else
                                        '    AddElement(visibility, "Hidden", "true")
                                        'End If
                                        AddElement(visibility, "Hidden", "true")
                                        AddElement(visibility, "ToggleItem", "Toggle" & i.ToString)
                                        lastgrpnum = i
                                    End If
                                End If
                            End If

                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                        End If

                        'if last deepest group
                        If lastgrpnum > 0 Then
                            visibility = AddElement(tablixMember, "Visibility", Nothing)
                            AddElement(visibility, "Hidden", "true")
                            AddElement(visibility, "ToggleItem", "Toggle" & lastgrpnum.ToString)  '(dtgr.Rows.Count - 1)
                            tablixMember = DetailTablixMember(doc, tablixMember)
                            lastgrpnum = -1
                        End If

                        'stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then

                            'row for subtotal name
                            tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            'rows with stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If

                    End If
                Next


            Else
                '-----------------------------------------  not overall one group, few calc fields  '--------------------------------------------------------------------------------------

                'ElseIf (dtgr.Rows.Count = 2 AndAlso dtgr.Rows(0)("GroupField").ToString <> "Overall" AndAlso dtgr.Rows(1)("GroupField").ToString = dtgr.Rows(0)("GroupField").ToString) OrElse (dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString = dtgr.Rows(0)("GroupField").ToString) Then
                '    'both groups not overall  (Show Report and Dynamic Report ok, Groups Stats has 1 line?)  
                '    'OrElse dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString <> dtgr.Rows(0)("GroupField").ToString)

                '    '-----------------------------------------  first group overall, second groups not overall for few calc fields  '--------------------------------------------------------------------------------------

                'ElseIf dtgr.Rows.Count > 2 AndAlso dtgr.Rows(0)("GroupField").ToString = "Overall" AndAlso dtgr.Rows(1)("GroupField").ToString <> "Overall" AndAlso dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString = dtgr.Rows(1)("GroupField").ToString Then
                '    'first group overall, other groups not overall (Show Report ok, Groups Stats has 1 line?)

                '================================================================================================================

                'loop for groups Overall
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If
                    Else
                        Exit For
                    End If
                Next

                Dim grpnum As Integer = 0
                grpnm = ""
                'loop for groups not Overall
                For i = ovrall To dtgr.Rows.Count - 1

                    'group name
                    If (i = ovrall) OrElse (i > ovrall AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                        grpnm = grpnm & "Grp" & dtgr.Rows(i)("GroupNum").ToString
                    Else
                        grpnm = dtgr.Rows(i)("GroupField").ToString & "Grp" & dtgr.Rows(i)("GroupNum").ToString
                    End If

                    'previous group number
                    If (i = ovrall) Then
                        grpnum = 0
                    Else
                        grpnum = dtgr.Rows(i)("GroupNum") - 1
                    End If

                    If dtgr.Rows(i)("NumInGroup") = 1 Then 'first row in the group

                        group = AddElement(tablixMember, "Group", Nothing)
                        attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                        groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                        groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                        sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                        sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                        value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                        'If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                        '    pagebreak = AddElement(group, "PageBreak", Nothing)
                        '    AddElement(pagebreak, "BreakLocation", "End")
                        'End If
                        If grpnum > 0 Then
                            visibility = AddElement(tablixMember, "Visibility", Nothing)
                            If showdetails Then
                                AddElement(visibility, "Hidden", "false")
                            Else
                                AddElement(visibility, "Hidden", "true")
                            End If
                            'AddElement(visibility, "Hidden", "true")
                            AddElement(visibility, "ToggleItem", "Toggle" & grpnum.ToString)
                        End If

                        tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                        'add tablexMember for group name
                        tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                        AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                        AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                        If grpnum > 0 Then
                            visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                            If showdetails Then
                                AddElement(visibility, "Hidden", "false")
                            Else
                                AddElement(visibility, "Hidden", "true")
                            End If
                            'AddElement(visibility, "Hidden", "true")
                            AddElement(visibility, "ToggleItem", "Toggle" & grpnum.ToString)
                        End If

                        'add one more in the end for column headers... ... last group
                        If dtgr.Rows(i)("GroupNum").ToString = lastgrpnum Then
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")

                            visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                            If showdetails Then
                                AddElement(visibility, "Hidden", "false")
                            Else
                                AddElement(visibility, "Hidden", "true")
                            End If
                            'AddElement(visibility, "Hidden", "true")
                            If grpnum > 0 Then
                                AddElement(visibility, "ToggleItem", "Toggle" & grpnum.ToString)
                            End If


                        End If

                        tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                    End If

                    'if last deepest group - add 1 time only
                    If lastgrpnum > 0 AndAlso dtgr.Rows(i)("GroupNum").ToString = lastgrpnum Then
                        visibility = AddElement(tablixMember, "Visibility", Nothing)
                        If showdetails Then
                            AddElement(visibility, "Hidden", "false")
                        Else
                            AddElement(visibility, "Hidden", "true")
                        End If
                        'AddElement(visibility, "Hidden", "true")
                        If grpnum > 0 Then
                            AddElement(visibility, "ToggleItem", "Toggle" & grpnum.ToString)
                        End If
                        tablixMember = DetailTablixMember(doc, tablixMember)
                        lastgrpnum = 0
                    End If

                    'stats
                    If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then

                        'row for subtotal name
                        tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                        AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                        'rows with stats
                        If n < 6 Then
                            For l = 0 To 8
                                tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            Next
                        Else
                            For l = 0 To 1
                                tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            Next
                        End If
                    End If

                Next

            End If

            '==========================================================================================================================
            'Save XML document in OURFiles
            ''flash to string
            Dim rdlstr As String = String.Empty
            Dim rdlfile As String = String.Empty
            Dim userupl As String = String.Empty
            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            rdlstr = sw.ToString

            ''TEMPORARY
            'Dim er As String = String.Empty
            ''Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            'rep = applpath & "Temp\" & repid & ".rdl"
            'doc.Save(rep)
            ''Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            ''If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
            ''    'read rdl from OURFiles
            ''    rdlfile = dv.Table.Rows(0)("Path").ToString  'string with file path
            ''    userupl = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
            ''End If
            ''If userupl.Trim = "" Then
            ''    er = SaveRDLstringInOURFiles(repid, rdlstr)  'just generated
            ''Else
            ''    Try
            ''        userupl = userupl.Replace("uploaded by", "").Trim
            ''        rdlfile = rdlfile.Replace("|", "\")  'in MySql file path does not saved properly !!!!
            ''        'copy user uploaded earlier rdl
            ''        Dim userrdl As String = rdlfile.Replace(".rdl", "UpldByUser" & userupl & ".rdl")
            ''        If File.Exists(userrdl) Then
            ''            File.Delete(userrdl)
            ''        End If
            ''        File.Copy(rdlfile, userrdl)  'save user rdl with new name
            ''        er = SaveRDLstringInOURFiles(repid, rdlstr, " ", " ")  'just generated but previously was uploaded by user
            ''    Catch ex As Exception
            ''        er = "ERROR!! " & ex.Message
            ''    End Try
            ''End If
            ''If er = "Query executed fine." Then
            ''    'delete file from RDLFILES folder if it is not user created rdl!!
            ''    If File.Exists(rep) Then
            ''        File.Delete(rep)
            ''        rep = "Report format generated."
            ''    End If
            ''Else
            ''    doc.Save(rep)
            ''End If
            ''TODO Remove it. Save RDL XML document to file temporary for now
            ''doc.Save(rep)
            Return rdlstr
        Catch ex As Exception
            ret = "ERROR!! " & repid & " - " & ex.Message
        End Try
        Return ret
    End Function
    Public Function GenerateDynamicReport(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal cat1 As String, ByVal cat2 As String, ByVal calcfld As String, ByVal reptype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "", Optional ByVal expand1 As Boolean = True, Optional ByVal expand2 As Boolean = False, Optional ByVal showall As Boolean = True) As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        Dim ret As String = String.Empty
        Dim n, i, j, k, m, l As Integer
        'Dim showall As Boolean = True     'show all fields as default
        Dim fldname As String = String.Empty
        Dim fldexpr As String = String.Empty
        repttl = repttl & " - DrillDown"
        If params.Trim <> "" Then repttl = repttl & " for:  " & params
        Try
            If dt Is Nothing Then dt = ReturnDataTblFromOURFiles(repid)
            If dt Is Nothing Then
                dt = ReturnDataTblFromXML(xsdfile)
            End If
            If dt Is Nothing OrElse dt.Columns Is Nothing OrElse dt.Columns.Count = 0 Then
                Return "ERROR!! " & repid & " - nothing to create ..."
            End If
            If params.Trim <> "" Then
                dt.DefaultView.RowFilter = params
            End If
            Dim dtf As DataTable = dt.DefaultView.ToTable
            dtf = CorrectDatasetColumns(dtf)
            Dim dtrf As DataTable = GetReportFields(repid, True)
            If showall Then
                n = dtf.Columns.Count
            Else
                n = dtrf.Rows.Count
            End If

            'showall = True as default

            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext").ToString
            If PageFtr = "" Then
                PageFtr = GetReportInfo(repid).Rows(0)("comments").ToString
            End If
            ' Create an XML document
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            Dim attr As XmlAttribute = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            'Dim myconstring As String
            'If myconstr.Trim = "" Then
            '    myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'Else
            '    myconstring = myconstr
            'End If
            'TODO UserConnStr ?
            'myconstring = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ToString
            'AddElement(connectionProperties, "ConnectString", myconstring)
            AddElement(connectionProperties, "ConnectString", "")
            AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", repid)
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "300")
            'Fields element
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To n - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                If showall Then
                    fldname = dtf.Columns(i).Caption
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                End If
                attr.Value = fldname
                AddElement(field, "DataField", fldname)
            Next
            'end of DataSources
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If

            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)

            If n > 5 Then   '!!!! TODO depend of columns widths
                'landscape

                If orien = "landscape" Then
                    AddElement(page, "PageWidth", "11in")
                    AddElement(page, "PageHeight", "8.5in")
                Else
                    AddElement(page, "PageWidth", "8.5in")
                    AddElement(page, "PageHeight", "11in")
                End If

            End If

            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.1in")
            AddElement(pageheader, "PrintOnFirstPage", "false")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            ''execution time
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "ExecutionTime"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "0.1in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.2in")
            AddElement(txtbox, "Height", "0.1in")
            AddElement(txtbox, "Width", "8in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            Dim npv As String = "Globals!PageNumber" & " & ""                                             "" & """ & repttl & """"   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(ttltextRun, "Value", "=" & npv)
            'AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.5in")
            AddElement(txtbox, "Height", "0.15in")
            AddElement(txtbox, "Width", "8.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.15in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            ''page number
            'txtbox = AddElement(repitems, "Textbox", Nothing)
            'attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = "PageNumber"
            'AddElement(txtbox, "CanGrow", "true")
            'AddElement(txtbox, "KeepTogether", "true")
            'AddElement(txtbox, "Top", "0.1in")
            'AddElement(txtbox, "Left", "7.5in")
            'AddElement(txtbox, "Height", "0.25in")
            'AddElement(txtbox, "Width", "2.125in")
            'ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            'ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            'ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            'ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            ''TextAlign
            'AddElement(ttlstyle, "TextAlign", "Center")
            'ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            'AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            'ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            'AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            'AddElement(ttlstyle, "FontSize", "8pt")
            'AddElement(txtbox, "Style", Nothing)
            'AddElement(ttlstyle, "Border", Nothing)
            'AddElement(ttlstyle, "PaddingLeft", "2pt")
            'AddElement(ttlstyle, "PaddingRight", "2pt")
            'AddElement(ttlstyle, "PaddingTop", "2pt")
            'AddElement(ttlstyle, "PaddingBottom", "2pt")

            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "1in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "12pt")

            AddElement(reptitle, "Height", "0.15in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            ' Tablix element
            Dim tablix As XmlElement = AddElement(reportItems, "Tablix", Nothing)
            attr = tablix.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Tablix1"
            AddElement(tablix, "DataSetName", repid)
            AddElement(tablix, "Top", "0.01in")
            AddElement(tablix, "Left", "0.3in")
            AddElement(tablix, "Height", "0.15in")
            AddElement(tablix, "RepeatColumnHeaders", "true")
            AddElement(tablix, "FixedColumnHeaders", "true")
            AddElement(tablix, "RepeatRowHeaders", "true")
            AddElement(tablix, "FixedRowHeaders", "true")

            Dim tablixBody As XmlElement = AddElement(tablix, "TablixBody", Nothing)
            Dim ltablex As Integer = 0
            'TablixColumns element
            Dim tablixColumns As XmlElement = AddElement(tablixBody, "TablixColumns", Nothing)
            Dim tablixColumn As XmlElement
            'toggle column
            tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
            AddElement(tablixColumn, "Width", "0.5in")
            For i = 0 To n - 1
                tablixColumn = AddElement(tablixColumns, "TablixColumn", Nothing)
                AddElement(tablixColumn, "Width", "1.5in")  'Unit.Pixel(dt.Columns(i).Caption.Length).ToString
            Next
            AddElement(tablix, "Width", (n * 1.5 + 0.5).ToString & "in") 'Unit.Pixel(ltablex).ToString
            'TablixRows element
            Dim tablixRows As XmlElement = AddElement(tablixBody, "TablixRows", Nothing)
            Dim tablixCell As XmlElement
            Dim cellContents As XmlElement
            Dim textbox As XmlElement
            Dim paragraphs As XmlElement
            Dim paragraph As XmlElement
            Dim textRuns As XmlElement
            Dim textRun As XmlElement
            Dim style As XmlElement
            Dim border As XmlElement
            Dim name As String = String.Empty
            Dim colspan As XmlElement
            Dim fldval As String = String.Empty
            Dim fldfriendname As String = String.Empty

            'first tablixRow
            Dim tablixRow As XmlElement = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.15in")
            Dim tablixCells As XmlElement = AddElement(tablixRow, "TablixCells", Nothing)

            'add tablixRows for the group names  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim dtgrp As New DataTable
            dtgrp = GetReportGroups("ddd") 'empty group table
            'Dim col As DataColumn
            'col = New DataColumn
            'col.DataType = System.Type.GetType("System.String")
            'col.ColumnName = "GroupField"
            'dtgrp.Columns.Add(col)
            'col.DataType = System.Type.GetType("System.String")
            'col.ColumnName = "CalcField"
            'dtgrp.Columns.Add(col)
            Dim Row3 As DataRow = dtgrp.NewRow()
            Row3("ReportId") = repid
            Row3("GroupField") = "Overall"
            Row3("CalcField") = calcfld
            Row3("PageBrk") = 0
            Row3("CntChk") = 1
            Row3("CntDistChk") = 1
            Row3("GrpOrder") = 1
            If dt.Columns(calcfld).DataType.Name = "String" OrElse dt.Columns(calcfld).DataType.Name = "DateTime" OrElse calcfld.StartsWith("ID") Then
                Row3("SumChk") = 0
                Row3("MaxChk") = 0
                Row3("MinChk") = 0
                Row3("AvgChk") = 0
                Row3("StDevChk") = 0
            Else
                Row3("SumChk") = 1
                Row3("MaxChk") = 1
                Row3("MinChk") = 1
                Row3("AvgChk") = 1
                Row3("StDevChk") = 1
            End If
            Row3("FirstChk") = 0
            Row3("LastChk") = 0
            Row3("Indx") = 0
            dtgrp.Rows.Add(Row3)

            Dim Row1 As DataRow = dtgrp.NewRow()
            Row1("ReportId") = repid
            Row1("GroupField") = cat1
            Row1("CalcField") = calcfld
            Row1("PageBrk") = 0
            Row1("CntChk") = 1
            Row1("CntDistChk") = 1
            Row1("GrpOrder") = 2
            If dt.Columns(calcfld).DataType.Name = "String" OrElse dt.Columns(calcfld).DataType.Name = "DateTime" OrElse calcfld.StartsWith("ID") Then
                Row1("SumChk") = 0
                Row1("MaxChk") = 0
                Row1("MinChk") = 0
                Row1("AvgChk") = 0
                Row1("StDevChk") = 0
            Else
                Row1("SumChk") = 1
                Row1("MaxChk") = 1
                Row1("MinChk") = 1
                Row1("AvgChk") = 1
                Row1("StDevChk") = 1
            End If
            Row1("FirstChk") = 0
            Row1("LastChk") = 0
            Row1("Indx") = 1
            dtgrp.Rows.Add(Row1)

            Dim Row2 As DataRow = dtgrp.NewRow()
            Row2("ReportId") = repid
            Row2("GroupField") = cat2
            Row2("CalcField") = calcfld
            Row2("PageBrk") = 0
            Row2("CntChk") = 1
            Row2("CntDistChk") = 1
            Row2("GrpOrder") = 3
            If dt.Columns(calcfld).DataType.Name = "String" OrElse dt.Columns(calcfld).DataType.Name = "DateTime" OrElse calcfld.StartsWith("ID") Then
                Row2("SumChk") = 0
                Row2("MaxChk") = 0
                Row2("MinChk") = 0
                Row2("AvgChk") = 0
                Row2("StDevChk") = 0
            Else
                Row2("SumChk") = 1
                Row2("MaxChk") = 1
                Row2("MinChk") = 1
                Row2("AvgChk") = 1
                Row2("StDevChk") = 1
            End If

            Row2("FirstChk") = 0
            Row2("LastChk") = 0
            Row2("Indx") = 2
            dtgrp.Rows.Add(Row2)

            Dim hdrgr As String = String.Empty
            'Dim ovall As Integer = 0
            'If dtgrp.Rows(0)("GroupField").ToString = "Overall" Then ovall = 1
            For j = 0 To dtgrp.Rows.Count - 1
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgrp.Rows(j)("CalcField").ToString)
                If dtgrp.Rows(j)("GroupField") <> "Overall" Then
                    'no duplicates
                    If j = 0 OrElse dtgrp.Rows(j)("GroupField") <> dtgrp.Rows(j - 1)("GroupField") Then
                        grpfrname = GetFriendlyReportGroupName(repid, dtgrp.Rows(j)("GroupField").ToString, dtgrp.Rows(j)("CalcField").ToString)
                        hdrgr = "="" "
                        For k = 0 To j
                            hdrgr = hdrgr & "   "
                        Next
                        hdrgr = hdrgr & grpfrname & "  "" & Fields!" & dtgrp.Rows(j)("GroupField") & ".Value "
                        'add tablexRow for the group friendly name

                        'toggle cell
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "Toggle" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Center")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", "")  '!!!!!!!!
                        style = AddElement(textRun, "Style", Nothing)
                        'AddElement(style, "TextDecoration", "")
                        AddElement(style, "TextAlign", "Center")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        If j = dtgrp.Rows.Count - 1 Then
                            AddElement(style, "BackgroundColor", "#CCCCCC")
                        ElseIf j = 0 Then
                            AddElement(style, "BackgroundColor", "DimGray")
                        Else
                            AddElement(style, "BackgroundColor", "#" & (j * 10).ToString.Substring(0, 2) & (j * 20).ToString.Substring(0, 2) & (j * 10).ToString.Substring(0, 2))
                        End If
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")

                        ' TablixCell element (column with group name)
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        AddElement(textbox, "CanGrow", "true")
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "GroupHdr" & j.ToString                            '!!!!!!!!
                        AddElement(textbox, "KeepTogether", "true")
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Left")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", hdrgr)                             '!!!!!!!! 
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "FontStyle", "Italic")
                        AddElement(style, "Color", "White")
                        'AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Left")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        If j = dtgrp.Rows.Count - 1 Then
                            AddElement(style, "BackgroundColor", "#CCCCCC")
                        Else
                            'AddElement(style, "BackgroundColor", "DimGray")
                            AddElement(style, "BackgroundColor", "#" & (j * 10).ToString.Substring(0, 2) & (j * 20).ToString.Substring(0, 2) & (j * 10).ToString.Substring(0, 2))
                        End If
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        'add columns and colspan
                        colspan = AddElement(cellContents, "ColSpan", n)
                        For k = 0 To n - 2
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        Next

                        'add tablix row for next with group and last one for columns names
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)
                    End If
                End If
            Next

            'empty cell
            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
            cellContents = AddElement(tablixCell, "CellContents", Nothing)
            textbox = AddElement(cellContents, "Textbox", Nothing)
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "txtboxempty1"
            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
            style = AddElement(paragraph, "Style", Nothing)
            AddElement(style, "TextAlign", "Center")
            textRuns = AddElement(paragraph, "TextRuns", Nothing)
            textRun = AddElement(textRuns, "TextRun", Nothing)
            AddElement(textRun, "Value", "")  '!!!!!!!!
            style = AddElement(textRun, "Style", Nothing)
            AddElement(style, "TextDecoration", "Underline")
            AddElement(style, "TextAlign", "Center")
            AddElement(style, "FontWeight", "Bold")
            AddElement(style, "PaddingLeft", "2pt")
            AddElement(style, "PaddingRight", "2pt")
            AddElement(style, "PaddingTop", "2pt")
            AddElement(style, "PaddingBottom", "2pt")
            style = AddElement(textbox, "Style", Nothing)
            'border
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "TopBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "BottomBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "LeftBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "RightBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")

            'columns names
            For i = 0 To n - 1
                If showall Then
                    fldname = dtf.Columns(i).Caption
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                End If
                fldfriendname = GetFriendlyReportFieldName(repid, fldname)

                ' TablixCell element (column names)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                AddElement(textbox, "CanGrow", "true")
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname & "h"     '!!!!
                AddElement(textbox, "KeepTogether", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                style = AddElement(paragraph, "Style", Nothing)
                AddElement(style, "TextAlign", "Center")
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                AddElement(textRun, "Value", fldfriendname)  '!!!!!!!!
                style = AddElement(textRun, "Style", Nothing)
                AddElement(style, "TextDecoration", "Underline")
                AddElement(style, "TextAlign", "Center")
                AddElement(style, "FontWeight", "Bold")
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                style = AddElement(textbox, "Style", Nothing)
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
            Next

            'TablixRow element (details row)
            tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
            AddElement(tablixRow, "Height", "0.15in")
            tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

            'empty cell
            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
            cellContents = AddElement(tablixCell, "CellContents", Nothing)
            textbox = AddElement(cellContents, "Textbox", Nothing)
            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "txtboxempty2"
            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
            style = AddElement(paragraph, "Style", Nothing)
            AddElement(style, "TextAlign", "Center")
            textRuns = AddElement(paragraph, "TextRuns", Nothing)
            textRun = AddElement(textRuns, "TextRun", Nothing)
            AddElement(textRun, "Value", "")  '!!!!!!!!
            style = AddElement(textRun, "Style", Nothing)
            AddElement(style, "TextDecoration", "Underline")
            AddElement(style, "TextAlign", "Center")
            AddElement(style, "FontWeight", "Bold")
            AddElement(style, "PaddingLeft", "2pt")
            AddElement(style, "PaddingRight", "2pt")
            AddElement(style, "PaddingTop", "2pt")
            AddElement(style, "PaddingBottom", "2pt")
            style = AddElement(textbox, "Style", Nothing)
            'border
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "TopBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "BottomBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "LeftBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")
            border = AddElement(style, "RightBorder", Nothing)
            AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Width", "0.25pt")

            'columns values
            For i = 0 To n - 1
                fldexpr = ""
                If showall Then
                    fldname = dtf.Columns(i).Caption
                    'find field expression
                    dtrf.DefaultView.RowFilter = "Val='" & fldname & "'"
                    If Not dtrf.DefaultView.ToTable Is Nothing Then
                        Dim dtfex As DataTable = dtrf.DefaultView.ToTable
                        If dtfex.Rows.Count > 0 Then fldexpr = dtfex.Rows(0)("Prop2").ToString
                        dtrf.DefaultView.RowFilter = ""
                    End If
                Else
                    fldname = dtrf.Rows(i)("Val").ToString
                    fldexpr = dtrf.Rows(i)("Prop2").ToString
                End If

                'fldname = dtrf.Rows(i)("Val").ToString
                'fldexpr = dtrf.Rows(i)("Prop2").ToString


                ' TablixCell element (first cell)
                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                textbox = AddElement(cellContents, "Textbox", Nothing)
                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = fldname         '!!!!!!
                'AddElement(textbox, "HideDuplicates", repid)
                AddElement(textbox, "KeepTogether", "true")
                AddElement(textbox, "CanGrow", "true")
                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                textRun = AddElement(textRuns, "TextRun", Nothing)
                'add expressions
                If fldexpr = "" Then
                    'if numeric field
                    If ColumnTypeIsNumeric(dt.Columns(fldname)) Then
                        AddElement(textRun, "Value", "=Round(Fields!" & fldname & ".Value,2)")  '!!!!!!
                    Else
                        'not numeric 
                        AddElement(textRun, "Value", "=Fields!" & fldname & ".Value")  '!!!!!!
                    End If
                Else
                    AddElement(textRun, "Value", "=" & fldexpr)  '!!!!!!
                End If

                style = AddElement(textRun, "Style", Nothing)
                style = AddElement(textbox, "Style", Nothing)
                AddElement(style, "PaddingLeft", "2pt")
                AddElement(style, "PaddingRight", "2pt")
                AddElement(style, "PaddingTop", "2pt")
                AddElement(style, "PaddingBottom", "2pt")
                'border
                border = AddElement(style, "Border", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "TopBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "BottomBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "LeftBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")
                border = AddElement(style, "RightBorder", Nothing)
                AddElement(border, "Style", "Solid")
                AddElement(border, "Color", "LightGrey")
                AddElement(border, "Width", "0.25pt")

            Next

            Dim totgrp As String = String.Empty
            Dim totgr As String = String.Empty
            'add groups subtotals rows 
            Dim dtgt As DataTable = dtgrp  'GetReportGroups(repid)
            For i = 0 To dtgt.Rows.Count - 1
                j = dtgt.Rows.Count - 1 - i
                'group statistics title
                Dim grpfrname As String = ""
                Dim fldfrname As String = GetFriendlyReportFieldName(repid, dtgt.Rows(j)("CalcField").ToString)
                If dtgt.Rows(j)("GroupField") = "Overall" Then
                    totgr = "Overall totals Of " & fldfrname 'dtgt.Rows(j)("CalcField").ToString
                Else 'not overall
                    totgr = "=""Subtotals Of " & fldfrname & " For: "
                    For l = 0 To j
                        If dtgt.Rows(l)("GroupField") <> "Overall" AndAlso dtgt.Rows(j)("GroupField") <> dtgt.Rows(l)("GroupField") Then
                            grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(l)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                            totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(l)("GroupField") & ".Value "
                            If l < j Then totgr = totgr & " & "" "
                        End If
                    Next
                    grpfrname = GetFriendlyReportGroupName(repid, dtgt.Rows(j)("GroupField").ToString, dtgt.Rows(j)("CalcField").ToString)
                    totgr = totgr & "  " & grpfrname & "  "" & Fields!" & dtgt.Rows(j)("GroupField") & ".Value "

                End If
                'group title stats rows
                m = dtgt.Rows(j)("CntChk") + dtgt.Rows(j)("SumChk") + dtgt.Rows(j)("MaxChk") + dtgt.Rows(j)("MinChk") + dtgt.Rows(j)("AvgChk") + dtgt.Rows(j)("StDevChk") + dtgt.Rows(j)("CntDistChk") + dtgt.Rows(j)("FirstChk") + dtgt.Rows(j)("LastChk")
                If m > 0 Then  'm stats ordered
                    ' Tablix row for group totals name
                    tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                    AddElement(tablixRow, "Height", "0.15In")
                    tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                    'empty cell
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = "txtbox3" & "e" & j.ToString
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    style = AddElement(paragraph, "Style", Nothing)
                    AddElement(style, "TextAlign", "Center")
                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", "")  '!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    AddElement(style, "TextDecoration", "Underline")
                    AddElement(style, "TextAlign", "Center")
                    AddElement(style, "FontWeight", "Bold")
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    style = AddElement(textbox, "Style", Nothing)
                    If j = 0 OrElse dtgt.Rows(j)("GroupField") = "Overall" Then
                        AddElement(style, "BackgroundColor", "DimGray")
                    End If
                    'border
                    border = AddElement(style, "Border", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "TopBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "BottomBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "LeftBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "RightBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")

                    ' TablixCell element (group subtotal name)
                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    cellContents = AddElement(tablixCell, "CellContents", Nothing)
                    textbox = AddElement(cellContents, "Textbox", Nothing)
                    attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                    attr.Value = dtgt.Rows(j)("GroupField") & "gr" & j.ToString      '!!!!!!
                    'AddElement(textbox, "HideDuplicates", repid)
                    AddElement(textbox, "KeepTogether", "true")
                    AddElement(textbox, "CanGrow", "true")
                    paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                    paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                    textRuns = AddElement(paragraph, "TextRuns", Nothing)
                    textRun = AddElement(textRuns, "TextRun", Nothing)
                    AddElement(textRun, "Value", totgr)        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    style = AddElement(textRun, "Style", Nothing)
                    If dtgt.Rows(j)("GroupField") = "Overall" Then
                        AddElement(style, "Color", "White")
                    End If
                    'AddElement(style, "TextDecoration", "Underline")
                    AddElement(style, "TextAlign", "Left")
                    AddElement(style, "FontWeight", "Bold")
                    style = AddElement(textbox, "Style", Nothing)
                    If dtgt.Rows(j)("GroupField") = "Overall" Then
                        AddElement(style, "BackgroundColor", "DimGray")
                    End If
                    AddElement(style, "PaddingLeft", "2pt")
                    AddElement(style, "PaddingRight", "2pt")
                    AddElement(style, "PaddingTop", "2pt")
                    AddElement(style, "PaddingBottom", "2pt")
                    'border
                    border = AddElement(style, "Border", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "TopBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "BottomBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "LeftBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    border = AddElement(style, "RightBorder", Nothing)
                    AddElement(border, "Style", "Solid")
                    AddElement(border, "Color", "LightGrey")
                    AddElement(border, "Width", "0.25pt")
                    'add columns and colspan
                    colspan = AddElement(cellContents, "ColSpan", n)
                    'add tablix row for each stat with columns and colspan
                    For k = 0 To n - 2
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                    Next

                    ' Tablix rows for stats
                    If n < 6 Then  'less than 6 columns for stats - add  6 rows
                        For k = 0 To 8  'stats 5
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'add row
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.15In")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                'empty cell
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = "txtbox4e" & k.ToString & "empty" & j.ToString
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                AddElement(textRun, "Value", "")  '!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "FontWeight", "Bold")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                style = AddElement(textbox, "Style", Nothing)
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")

                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"  '!!!!!!!!!!!!!!!!!!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "Last(" & fldval & ")"
                                End If
                                fldval = " & " & fldval
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Count:  """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Sum:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Max:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Min:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Avg:    """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""StDev:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""CntDist:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""First:   """ & fldval)
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "=""Last:   """ & fldval)
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            Else 'no stats for the column
                                tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                                AddElement(tablixRow, "Height", "0.15in")
                                tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                                'empty cell
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = "txtbox5e" & "e" & k.ToString & "e" & l.ToString & "e" & j.ToString
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                AddElement(textRun, "Value", "")  '!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "FontWeight", "Bold")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                style = AddElement(textbox, "Style", Nothing)
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")


                                ' TablixCell element (stat subtotal name) - column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "K" & k.ToString & "L" & l.ToString & "J" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                'AddElement(textRun, "Value", " ")   '!!!!!
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First: ")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last: ")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Left")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                'add columns and colspan
                                colspan = AddElement(cellContents, "ColSpan", n)
                                'add columns and colspan
                                For l = 0 To n - 2
                                    tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                Next
                            End If
                        Next
                    Else  'at least 8 columns for stats - add 2 rows
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                        'empty cell
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "txtbox6e" & "e" & j.ToString
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Center")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", "")  '!!!!!!!!
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Center")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")

                        For k = 0 To 8  '8 stats names in one row
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then  'stat ordered
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) 
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "sth" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    AddElement(textRun, "Value", "Count:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    AddElement(textRun, "Value", "Sum:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    AddElement(textRun, "Value", "Max:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    AddElement(textRun, "Value", "Min:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    AddElement(textRun, "Value", "Avg:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    AddElement(textRun, "Value", "StDev:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    AddElement(textRun, "Value", "CntDist:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    AddElement(textRun, "Value", "First:")
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    AddElement(textRun, "Value", "Last:")
                                End If
                                style = AddElement(textRun, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "sthe" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If

                        'second row with stats values
                        tablixRow = AddElement(tablixRows, "TablixRow", Nothing)
                        AddElement(tablixRow, "Height", "0.15in")
                        tablixCells = AddElement(tablixRow, "TablixCells", Nothing)

                        'empty cell
                        tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                        cellContents = AddElement(tablixCell, "CellContents", Nothing)
                        textbox = AddElement(cellContents, "Textbox", Nothing)
                        attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                        attr.Value = "txtbox7e" & "e" & j.ToString
                        paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                        paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                        style = AddElement(paragraph, "Style", Nothing)
                        AddElement(style, "TextAlign", "Center")
                        textRuns = AddElement(paragraph, "TextRuns", Nothing)
                        textRun = AddElement(textRuns, "TextRun", Nothing)
                        AddElement(textRun, "Value", "")  '!!!!!!!!
                        style = AddElement(textRun, "Style", Nothing)
                        AddElement(style, "TextDecoration", "Underline")
                        AddElement(style, "TextAlign", "Center")
                        AddElement(style, "FontWeight", "Bold")
                        AddElement(style, "PaddingLeft", "2pt")
                        AddElement(style, "PaddingRight", "2pt")
                        AddElement(style, "PaddingTop", "2pt")
                        AddElement(style, "PaddingBottom", "2pt")
                        style = AddElement(textbox, "Style", Nothing)
                        'border
                        border = AddElement(style, "Border", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "TopBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "BottomBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "LeftBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")
                        border = AddElement(style, "RightBorder", Nothing)
                        AddElement(border, "Style", "Solid")
                        AddElement(border, "Color", "LightGrey")
                        AddElement(border, "Width", "0.25pt")

                        For k = 0 To 8  '8 stats 
                            If dtgt.Rows(j)(dtgt.Columns(k + 3).Caption) = 1 Then
                                'put name of stat in column 
                                ' TablixCell element (stat subtotal name) - first column
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                                cellContents = AddElement(tablixCell, "CellContents", Nothing)
                                textbox = AddElement(cellContents, "Textbox", Nothing)
                                attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                                attr.Value = dtgt.Columns(k + 3).Caption & "stk" & k.ToString & "j" & j.ToString    '!!!!!!
                                'AddElement(textbox, "HideDuplicates", repid)
                                AddElement(textbox, "KeepTogether", "true")
                                AddElement(textbox, "CanGrow", "true")
                                paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                                paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                                style = AddElement(paragraph, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                textRuns = AddElement(paragraph, "TextRuns", Nothing)
                                textRun = AddElement(textRuns, "TextRun", Nothing)
                                fldval = "Fields!" & dtgt.Rows(j)("CalcField") & ".Value"
                                If dtgt.Columns(k + 3).Caption.ToUpper = "CntChk".ToUpper Then
                                    fldval = "=Count(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "SumChk".ToUpper Then
                                    fldval = "=Sum(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MaxChk".ToUpper Then
                                    fldval = "=Round(Max(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "MinChk".ToUpper Then
                                    fldval = "=Round(Min(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "AvgChk".ToUpper Then
                                    fldval = "=FormatNumber(Avg(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "StDevChk".ToUpper Then
                                    fldval = "=FormatNumber(StDev(" & fldval & "),2)"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "CntDistChk".ToUpper Then
                                    fldval = "=CountDistinct(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "FirstChk".ToUpper Then
                                    fldval = "=First(" & fldval & ")"
                                ElseIf dtgt.Columns(k + 3).Caption.ToUpper = "LastChk".ToUpper Then
                                    fldval = "=Last(" & fldval & ")"
                                End If
                                AddElement(textRun, "Value", fldval)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                style = AddElement(textRun, "Style", Nothing)
                                'AddElement(style, "TextDecoration", "Underline")
                                AddElement(style, "TextAlign", "Center")
                                'AddElement(style, "FontWeight", "Bold")
                                style = AddElement(textbox, "Style", Nothing)
                                AddElement(style, "TextAlign", "Center")
                                AddElement(style, "PaddingLeft", "2pt")
                                AddElement(style, "PaddingRight", "2pt")
                                AddElement(style, "PaddingTop", "2pt")
                                AddElement(style, "PaddingBottom", "2pt")
                                'border
                                border = AddElement(style, "Border", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "TopBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "BottomBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "LeftBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                                border = AddElement(style, "RightBorder", Nothing)
                                AddElement(border, "Style", "Solid")
                                AddElement(border, "Color", "LightGrey")
                                AddElement(border, "Width", "0.25pt")
                            End If
                        Next
                        'add columns and colspan
                        If m < n Then
                            'add column
                            'put name of stat in column 
                            ' TablixCell element (stat subtotal name) 
                            tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            cellContents = AddElement(tablixCell, "CellContents", Nothing)
                            textbox = AddElement(cellContents, "Textbox", Nothing)
                            attr = textbox.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = dtgt.Columns(k + 3).Caption & "stve" & k.ToString & "j" & j.ToString    '!!!!!!
                            'AddElement(textbox, "HideDuplicates", repid)
                            AddElement(textbox, "KeepTogether", "true")
                            AddElement(textbox, "CanGrow", "true")
                            paragraphs = AddElement(textbox, "Paragraphs", Nothing)
                            paragraph = AddElement(paragraphs, "Paragraph", Nothing)
                            style = AddElement(paragraph, "Style", Nothing)
                            AddElement(style, "TextAlign", "Center")
                            textRuns = AddElement(paragraph, "TextRuns", Nothing)
                            textRun = AddElement(textRuns, "TextRun", Nothing)
                            AddElement(textRun, "Value", " ")
                            style = AddElement(textRun, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            'AddElement(style, "FontWeight", "Bold")
                            style = AddElement(textbox, "Style", Nothing)
                            AddElement(style, "TextDecoration", "Underline")
                            AddElement(style, "TextAlign", "Center")
                            AddElement(style, "PaddingLeft", "2pt")
                            AddElement(style, "PaddingRight", "2pt")
                            AddElement(style, "PaddingTop", "2pt")
                            AddElement(style, "PaddingBottom", "2pt")
                            'border
                            border = AddElement(style, "Border", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "TopBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "BottomBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "LeftBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            border = AddElement(style, "RightBorder", Nothing)
                            AddElement(border, "Style", "Solid")
                            AddElement(border, "Color", "LightGrey")
                            AddElement(border, "Width", "0.25pt")
                            'add columns and colspan
                            colspan = AddElement(cellContents, "ColSpan", n - m)
                            For l = 0 To n - m - 2
                                tablixCell = AddElement(tablixCells, "TablixCell", Nothing)
                            Next
                        End If
                    End If
                End If
            Next

            'End of TablixBody

            'TablixColumnHierarchy element
            Dim tablixColumnHierarchy As XmlElement = AddElement(tablix, "TablixColumnHierarchy", Nothing)
            Dim tablixMembers As XmlElement = AddElement(tablixColumnHierarchy, "TablixMembers", Nothing)
            For i = 0 To n
                AddElement(tablixMembers, "TablixMember", Nothing)
            Next

            'TablixRowHierarchy element
            Dim tablixRowHierarchy As XmlElement = AddElement(tablix, "TablixRowHierarchy", Nothing)
            Dim tablixMember As XmlElement
            Dim tablixMemberGrp As XmlElement
            Dim tablixMemberStat As XmlElement
            Dim group As XmlElement
            Dim groupexpressions As XmlElement
            Dim groupexpression As XmlElement
            Dim pagebreak As XmlElement
            Dim sortexpressions As XmlElement
            Dim sortexpression As XmlElement
            Dim visibility As XmlElement
            Dim value As XmlElement
            Dim grpnm As String = String.Empty
            tablixMembers = AddElement(tablixRowHierarchy, "TablixMembers", Nothing)
            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            ' AddElement(tablixMember, "KeepWithGroup", "After")
            'AddElement(tablixMember, "RepeatOnNewPage", "true")
            AddElement(tablixMember, "KeepTogether", "true")
            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
            'groups
            Dim ovrall As Integer = 0
            Dim dtgr As DataTable = dtgrp 'GetReportGroups(repid)
            If dtgr.Rows.Count > 0 Then
                grpnm = ""
                For i = 0 To dtgr.Rows.Count - 1
                    If dtgr.Rows(i)("GroupField").ToString = "Overall" Then
                        ovrall = ovrall + 1
                        'overall stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then
                            'Overall name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                            'rows with overall stats
                            'If ovrall = 1 Then
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberGrp, "KeepWithGroup", "Before")
                                Next
                            End If
                            'End If
                        End If
                    Else
                        Exit For
                    End If
                Next

                For i = 0 To dtgr.Rows.Count - 1  'loop for groups not Overall
                    If dtgr.Rows(i)("GroupField").ToString <> "Overall" Then
                        'group name
                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(i - 1)("GroupField").ToString) Then
                            grpnm = grpnm & "Grp" '& dtgr.Rows(i)("CalcField").ToString
                        Else
                            grpnm = dtgr.Rows(i)("GroupField").ToString & "Grp" '& dtgr.Rows(i)("CalcField").ToString
                        End If

                        'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)      '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        If (i = 0) OrElse (i > 0 AndAlso dtgr.Rows(i)("GroupField").ToString <> dtgr.Rows(i - 1)("GroupField").ToString) Then

                            'tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                            group = AddElement(tablixMember, "Group", Nothing)
                            attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                            attr.Value = grpnm & i.ToString  'dtgr.Rows(i)("GroupField").ToString & "grp"
                            groupexpressions = AddElement(group, "GroupExpressions", Nothing)
                            groupexpression = AddElement(groupexpressions, "GroupExpression", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            sortexpressions = AddElement(tablixMember, "SortExpressions", Nothing)
                            sortexpression = AddElement(sortexpressions, "SortExpression", Nothing)
                            value = AddElement(sortexpression, "Value", "=Fields!" & dtgr.Rows(i)("GroupField").ToString & ".Value")
                            'If i = ovrall OrElse dtgr.Rows(i)("PageBrk") = 1 Then 'page break on next group after overall or if ordered in RDL format page
                            '    pagebreak = AddElement(group, "PageBreak", Nothing)
                            '    AddElement(pagebreak, "BreakLocation", "End")
                            'End If
                            If i > 1 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                If expand1 + expand2 Then
                                    AddElement(visibility, "Hidden", "false")
                                Else
                                    AddElement(visibility, "Hidden", "true")
                                End If
                                AddElement(visibility, "ToggleItem", "Toggle" & (i - 1).ToString)
                            End If

                            tablixMembers = AddElement(tablixMember, "TablixMembers", Nothing)  '!!! new tablixMembers after sortexpression

                            'add tablexMember for group name
                            tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                            AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                            If i > 1 Then
                                visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                If expand1 + expand2 Then
                                    AddElement(visibility, "Hidden", "false")
                                Else
                                    AddElement(visibility, "Hidden", "true")
                                End If
                                AddElement(visibility, "ToggleItem", "Toggle" & (i - 1).ToString)
                            End If

                            'add one more in the end for column headers... 
                            If dtgr.Rows(i)("GroupField").ToString = dtgr.Rows(dtgr.Rows.Count - 1)("GroupField").ToString Then
                                tablixMemberGrp = AddElement(tablixMembers, "TablixMember", Nothing)
                                AddElement(tablixMemberGrp, "KeepWithGroup", "After")
                                AddElement(tablixMemberGrp, "RepeatOnNewPage", "true")
                                visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                AddElement(visibility, "Hidden", "true")
                                AddElement(visibility, "ToggleItem", "Toggle" & (dtgr.Rows.Count - 1).ToString)
                            Else
                                If i > 1 Then
                                    visibility = AddElement(tablixMemberGrp, "Visibility", Nothing)
                                    AddElement(visibility, "Hidden", "true")
                                    AddElement(visibility, "ToggleItem", "Toggle" & (i - 1).ToString)
                                End If
                            End If

                            tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)        '!!!!! new tablixMember

                        End If

                        'if last deepest group
                        If i = dtgr.Rows.Count - 1 Then

                            If expand1 AndAlso expand2 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                AddElement(visibility, "Hidden", "false")             '!!!! trying to show column headers
                                AddElement(visibility, "ToggleItem", "Toggle" & (dtgr.Rows.Count - 1).ToString)
                                tablixMember = DetailTablixMember(doc, tablixMember, 1)
                            ElseIf Not expand2 Then
                                visibility = AddElement(tablixMember, "Visibility", Nothing)
                                AddElement(visibility, "Hidden", "true")
                                AddElement(visibility, "ToggleItem", "Toggle" & (dtgr.Rows.Count - 1).ToString)
                                tablixMember = DetailTablixMember(doc, tablixMember, 0)
                            End If

                        End If

                        'stats
                        If dtgr.Rows(i)("CntChk") = 1 OrElse dtgr.Rows(i)("SumChk") = 1 OrElse dtgr.Rows(i)("MaxChk") = 1 OrElse dtgr.Rows(i)("MinChk") = 1 OrElse dtgr.Rows(i)("AvgChk") = 1 OrElse dtgr.Rows(i)("StDevChk") = 1 OrElse dtgr.Rows(i)("CntDistChk") = 1 OrElse dtgr.Rows(i)("FirstChk") = 1 OrElse dtgr.Rows(i)("LastChk") = 1 Then

                            'row for subtotal name
                            tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                            AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                            'rows with stats
                            If n < 6 Then
                                For l = 0 To 8
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            Else
                                For l = 0 To 1
                                    tablixMemberStat = AddElement(tablixMembers, "TablixMember", Nothing)
                                    AddElement(tablixMemberStat, "KeepWithGroup", "Before")
                                Next
                            End If
                        End If



                    End If
                Next

            Else  'no groups
                'detail
                tablixMember = AddElement(tablixMembers, "TablixMember", Nothing)
                AddElement(tablixMember, "DataElementName", "Detail_Collection")
                AddElement(tablixMember, "DataElementOutput", "Output")
                AddElement(tablixMember, "KeepTogether", "true")
                group = AddElement(tablixMember, "Group", Nothing)
                attr = group.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = "Table1_Details_Group"
                AddElement(group, "DataElementName", "Detail")
                Dim tablixMembersNested As XmlElement = AddElement(tablixMember, "TablixMembers", Nothing)
                AddElement(tablixMembersNested, "TablixMember", Nothing)
            End If

            'Save XML document in OURFiles
            ''flash to string
            Dim rdlstr As String = String.Empty
            Dim rdlfile As String = String.Empty
            Dim userupl As String = String.Empty
            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            rdlstr = sw.ToString

            ''TEMPORARY
            'Dim er As String = String.Empty
            ''Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            'rep = applpath & "Temp\" & repid & ".rdl"
            'doc.Save(rep)
            ''Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            ''If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
            ''    'read rdl from OURFiles
            ''    rdlfile = dv.Table.Rows(0)("Path").ToString  'string with file path
            ''    userupl = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
            ''End If
            ''If userupl.Trim = "" Then
            ''    er = SaveRDLstringInOURFiles(repid, rdlstr)  'just generated
            ''Else
            ''    Try
            ''        userupl = userupl.Replace("uploaded by", "").Trim
            ''        rdlfile = rdlfile.Replace("|", "\")  'in MySql file path does not saved properly !!!!
            ''        'copy user uploaded earlier rdl
            ''        Dim userrdl As String = rdlfile.Replace(".rdl", "UpldByUser" & userupl & ".rdl")
            ''        If File.Exists(userrdl) Then
            ''            File.Delete(userrdl)
            ''        End If
            ''        File.Copy(rdlfile, userrdl)  'save user rdl with new name
            ''        er = SaveRDLstringInOURFiles(repid, rdlstr, " ", " ")  'just generated but previously was uploaded by user
            ''    Catch ex As Exception
            ''        er = "ERROR!! " & ex.Message
            ''    End Try
            ''End If
            ''If er = "Query executed fine." Then
            ''    'delete file from RDLFILES folder if it is not user created rdl!!
            ''    If File.Exists(rep) Then
            ''        File.Delete(rep)
            ''        rep = "Report format generated."
            ''    End If
            ''Else
            ''    doc.Save(rep)
            ''End If
            ''TODO Remove it. Save RDL XML document to file temporary for now
            ''doc.Save(rep)
            Return rdlstr
        Catch ex As Exception
            ret = "ERROR!! " & repid & " - " & ex.Message
        End Try
        Return ret
    End Function
    Public Function CreateGraphRDLForDT(ByVal repid As String, ByVal repttl As String, ByVal params As String, ByVal dt As DataTable, ByVal fn As String, ByVal primX As String, ByVal secX As String, ByVal valueY As String, ByVal graphtype As String, Optional ByVal orien As String = "portrait", Optional ByVal PageFtr As String = "", Optional ByVal graphstyle As String = "") As String
        Dim ret As String = String.Empty
        Dim graf As String = String.Empty
        Dim indx As String = String.Empty
        Dim i As Integer
        Try
            dt = CorrectDatasetColumns(dt)
            Dim grpfld As String = primX
            Dim grpfld2 As String = secX
            Dim calcfld As String = valueY
            Dim hdrfld As String = fn & " of " & calcfld & " in group by " & grpfld
            If grpfld <> grpfld2 Then
                hdrfld = hdrfld & ", " & grpfld2
            End If
            'calculate dynamic width and height
            Dim wdth As Integer = 0
            Dim fldnames(0) As String
            fldnames(0) = grpfld
            Dim gr1distcnt As Integer = dt.DefaultView.ToTable(1, fldnames).Rows.Count
            wdth = gr1distcnt / 10
            'If grpfld = grpfld2 Then
            '    wdth = gr1distcnt / 10
            'Else
            '    fldnames(0) = grpfld2
            '    Dim gr2distcnt As Integer = dt.DefaultView.ToTable(1, fldnames).Rows.Count
            '    wdth = gr1distcnt * gr2distcnt / 100
            'End If
            If wdth < 10 Then wdth = 10

            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<Report xmlns='http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition'></Report>"
            'If graphtype = "pie" Then
            '    xmlData = "<Report MustUnderstand='df' xmlns='http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition' xmlns:rd='http://schemas.microsoft.com/SQLServer/reporting/reportdesigner' xmlns:df='http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition/defaultfontfamily'><df:DefaultFontFamily>Segoe UI</df:DefaultFontFamily>"
            'End If
            doc.Load(New StringReader(xmlData))

            ' Report element
            Dim report As XmlElement = DirectCast(doc.FirstChild, XmlElement)
            Dim attr As XmlAttribute '= report.Attributes.Append(doc.CreateAttribute("Name"))
            'attr.Value = repid
            AddElement(report, "AutoRefresh", "0")
            AddElement(report, "ConsumeContainerWhitespace", "true")

            'DataSources element
            Dim dataSources As XmlElement = AddElement(report, "DataSources", Nothing)
            'DataSource element
            Dim dataSource As XmlElement = AddElement(dataSources, "DataSource", Nothing)
            attr = dataSource.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "DataSource1"
            Dim connectionProperties As XmlElement = AddElement(dataSource, "ConnectionProperties", Nothing)
            AddElement(connectionProperties, "DataProvider", "SQL")
            AddElement(connectionProperties, "ConnectString", "")
            'AddElement(connectionProperties, "IntegratedSecurity", "true")
            'DataSets element
            Dim dataSets As XmlElement = AddElement(report, "DataSets", Nothing)
            Dim dataSet As XmlElement = AddElement(dataSets, "DataSet", Nothing)
            attr = dataSet.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid
            'Query element
            Dim mSQL As String = GetReportInfo(repid).Rows(0)("SQLquerytext")
            Dim query As XmlElement = AddElement(dataSet, "Query", Nothing)
            AddElement(query, "DataSourceName", "DataSource1")
            AddElement(query, "CommandText", mSQL)
            AddElement(query, "Timeout", "30000")
            Dim fields As XmlElement = AddElement(dataSet, "Fields", Nothing)
            Dim field As XmlElement
            For i = 0 To dt.Columns.Count - 1
                field = AddElement(fields, "Field", Nothing)
                attr = field.Attributes.Append(doc.CreateAttribute("Name"))
                attr.Value = dt.Columns(i).Caption
                AddElement(field, "DataField", dt.Columns(i).Caption)
            Next
            Dim ttlparagraphs As XmlElement
            Dim ttlparagraph As XmlElement
            Dim ttltextRuns As XmlElement
            Dim ttltextRun As XmlElement
            Dim ttlstyle As XmlElement
            Dim txtbox As XmlElement
            Dim repitems As XmlElement
            'ReportSections element
            Dim reportSections As XmlElement = AddElement(report, "ReportSections", Nothing)
            Dim reportSection As XmlElement = AddElement(reportSections, "ReportSection", Nothing)
            If orien = "landscape" Then
                AddElement(reportSection, "Width", "11in")
            Else
                AddElement(reportSection, "Width", "8.5in")
            End If
            Dim page As XmlElement = AddElement(reportSection, "Page", Nothing)
            If orien = "landscape" Then
                AddElement(page, "PageWidth", "11in")
                AddElement(page, "PageHeight", "8.5in")
            Else
                AddElement(page, "PageWidth", "8.5in")
                AddElement(page, "PageHeight", "11in")
            End If
            'Page header
            Dim pageheader As XmlElement = AddElement(page, "PageHeader", Nothing)
            AddElement(pageheader, "Height", "0.2in")
            AddElement(pageheader, "PrintOnFirstPage", "true")
            AddElement(pageheader, "PrintOnLastPage", "true")
            repitems = AddElement(pageheader, "ReportItems", Nothing)
            'page number
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageNumber"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0.2in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "1in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!PageNumber")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'page footer
            Dim pagefooter As XmlElement = AddElement(page, "PageFooter", Nothing)
            AddElement(pagefooter, "Height", "1in")
            AddElement(pagefooter, "PrintOnFirstPage", "true")
            AddElement(pagefooter, "PrintOnLastPage", "true")
            repitems = AddElement(pagefooter, "ReportItems", Nothing)
            'page footer text
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "PageFtr"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "1.5in")
            AddElement(txtbox, "Height", "0.8in")
            AddElement(txtbox, "Width", "8.0in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", PageFtr)
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")
            'execution time
            txtbox = AddElement(repitems, "Textbox", Nothing)
            attr = txtbox.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ExecutionTime"
            AddElement(txtbox, "CanGrow", "true")
            AddElement(txtbox, "KeepTogether", "true")
            AddElement(txtbox, "Top", "0.1in")
            AddElement(txtbox, "Left", "0in")
            AddElement(txtbox, "Height", "0.25in")
            AddElement(txtbox, "Width", "2.125in")
            ttlparagraphs = AddElement(txtbox, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Center")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", "=Globals!ExecutionTime")
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "8pt")
            AddElement(txtbox, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'doc report body
            Dim body As XmlElement = AddElement(reportSection, "Body", Nothing)
            AddElement(body, "Height", "8in")
            Dim reportItems As XmlElement = AddElement(body, "ReportItems", Nothing)
            'Report title
            Dim reptitle As XmlElement = AddElement(reportItems, "Textbox", Nothing)
            attr = reptitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "ReportTitle"
            AddElement(reptitle, "CanGrow", "true")
            AddElement(reptitle, "KeepTogether", "true")
            ttlparagraphs = AddElement(reptitle, "Paragraphs", Nothing)
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", repttl)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "16pt")
            AddElement(reptitle, "Height", "0.5in")
            AddElement(reptitle, "Width", "8in")
            AddElement(reptitle, "Style", Nothing)
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'subtitle
            ttlparagraph = AddElement(ttlparagraphs, "Paragraph", Nothing)
            ttltextRuns = AddElement(ttlparagraph, "TextRuns", Nothing)
            ttlstyle = AddElement(ttlparagraph, "Style", Nothing)
            'TextAlign
            AddElement(ttlstyle, "TextAlign", "Left")
            ttltextRun = AddElement(ttltextRuns, "TextRun", Nothing)
            AddElement(ttltextRun, "Value", params)                             '!!!!!!!!!!!!!!!!!!!!!!!!
            ttlstyle = AddElement(ttltextRun, "Style", Nothing)
            'AddElement(ttlstyle, "TextDecoration", "Underline")
            AddElement(ttlstyle, "FontFamily", "Segoe UI Light")
            AddElement(ttlstyle, "FontSize", "10pt")
            AddElement(ttlstyle, "Border", Nothing)
            AddElement(ttlstyle, "PaddingLeft", "2pt")
            AddElement(ttlstyle, "PaddingRight", "2pt")
            AddElement(ttlstyle, "PaddingTop", "2pt")
            AddElement(ttlstyle, "PaddingBottom", "2pt")

            'Chart
            Dim repgraph As XmlElement = AddElement(reportItems, "Chart", Nothing)
            attr = repgraph.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Chart1"
            'ChartCategoryHierarchy
            Dim chartcathierarchy As XmlElement = AddElement(repgraph, "ChartCategoryHierarchy", Nothing)
            Dim chartmembrs As XmlElement = AddElement(chartcathierarchy, "ChartMembers", Nothing)
            Dim chartmembr As XmlElement = AddElement(chartmembrs, "ChartMember", Nothing)
            Dim grp As XmlElement = AddElement(chartmembr, "Group", Nothing)
            attr = grp.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Chart1_CategoryGroup"
            'GroupExpressions
            Dim grpexprs As XmlElement = AddElement(grp, "GroupExpressions", Nothing)
            Dim grpexpr As XmlElement = AddElement(grpexprs, "GroupExpression", "=Fields!" & grpfld & ".Value")  '!!!!!!!!!!!!!!!!!!!
            Dim sortexprs As XmlElement = AddElement(chartmembr, "SortExpressions", Nothing)
            Dim sortexpr As XmlElement = AddElement(sortexprs, "SortExpression", Nothing)
            AddElement(sortexpr, "Value", "=Fields!" & grpfld & ".Value")                                        '!!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim lbl As XmlElement = AddElement(chartmembr, "Label", "=Fields!" & grpfld & ".Value")              '!!!!!!!!!!!!!!!!!!!!!!!!!!
            'ChartSeriesHierarchy
            Dim chartserhierarchy As XmlElement = AddElement(repgraph, "ChartSeriesHierarchy", Nothing)
            Dim chartsermembrs As XmlElement = AddElement(chartserhierarchy, "ChartMembers", Nothing)
            Dim chartsermembr As XmlElement = AddElement(chartsermembrs, "ChartMember", Nothing)
            Dim grpser As XmlElement = AddElement(chartsermembr, "Group", Nothing)
            attr = grpser.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Chart1_SeriesGroup"
            'GroupExpressions
            Dim grpserexprs As XmlElement = AddElement(grpser, "GroupExpressions", Nothing)
            'Dim grpserexpr As XmlElement = AddElement(grpserexprs, "GroupExpression", "=Fields!" & calcfld & ".Value")
            Dim grpserexpr As XmlElement = AddElement(grpserexprs, "GroupExpression", "=Fields!" & grpfld2 & ".Value")  '!!!!!!!!!!!!!!!!!!!!
            Dim sortserexprs As XmlElement = AddElement(chartsermembr, "SortExpressions", Nothing)
            Dim sortserexpr As XmlElement = AddElement(sortserexprs, "SortExpression", Nothing)
            AddElement(sortserexpr, "Value", "=Fields!" & grpfld2 & ".Value")                                         '!!!!!
            'Dim lblser As XmlElement = AddElement(chartsermembr, "Label", "=Fields!" & calcfld & ".Value")
            Dim lblser As XmlElement = AddElement(chartsermembr, "Label", "=Fields!" & grpfld2 & ".Value")            '!!!!!
            'ChartData
            Dim chartdata As XmlElement = AddElement(repgraph, "ChartData", Nothing)
            'ChartSeriesCollection
            Dim chartsercols As XmlElement = AddElement(chartdata, "ChartSeriesCollection", Nothing)
            Dim chartsers As XmlElement = AddElement(chartsercols, "ChartSeries", Nothing)
            attr = chartsers.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = calcfld
            If graphtype = "pie" Then
                AddElement(chartsers, "Type", "Shape")
            ElseIf graphtype = "line" Then
                AddElement(chartsers, "Type", "Line")
                AddElement(chartsers, "Subtype", "Smooth")
            End If
            'ChartDataPoints
            Dim chartdatapnts As XmlElement = AddElement(chartsers, "ChartDataPoints", Nothing)
            Dim chartdatapnt As XmlElement = AddElement(chartdatapnts, "ChartDataPoint", Nothing)
            'ChartDataPointValues
            Dim chartdatapntvals As XmlElement = AddElement(chartdatapnt, "ChartDataPointValues", Nothing)
            Dim yval As String = fn & "(" & "Fields!" & calcfld & ".Value)"                '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            AddElement(chartdatapntvals, "Y", "=" & yval)
            'ChartDataLabel
            Dim chartdatalbl As XmlElement = AddElement(chartdatapnt, "ChartDataLabel", Nothing)
            AddElement(chartdatalbl, "Style", Nothing)
            'AddElement(chartdatalbl, "UseValueAsLabel", "true")
            'AddElement(chartdatalbl, "Visible", "true")

            Dim ttp As String = yval & " & "" in group "" & " & "Fields!" & grpfld & ".Value" & " & ""-"" & " & "Fields!" & grpfld2 & ".Value"             '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
            Dim ToolTip As XmlElement = AddElement(chartdatapnt, "ToolTip", "=" & ttp)

            Dim chartmarker As XmlElement = AddElement(chartdatapnt, "ChartMarker", Nothing)
            AddElement(chartmarker, "Style", Nothing)
            'DataElementOutput
            AddElement(chartdatapnt, "DataElementOutput", "Output")
            AddElement(chartsers, "Style", Nothing)
            'ChartEmptyPoints
            Dim chartemptypnts As XmlElement = AddElement(chartsers, "ChartEmptyPoints", Nothing)
            AddElement(chartemptypnts, "Style", Nothing)
            Dim chartemptylbl As XmlElement = AddElement(chartemptypnts, "ChartDataLabel", Nothing)
            AddElement(chartemptylbl, "Style", Nothing)
            Dim chartemptymarker As XmlElement = AddElement(chartemptypnts, "ChartMarker", Nothing)
            AddElement(chartemptymarker, "Style", Nothing)
            'ValueAxisName
            AddElement(chartsers, "ValueAxisName", "Primary")
            AddElement(chartsers, "CategoryAxisName", "Primary")
            'ChartSmartLabels
            Dim chartsmartlbl As XmlElement = AddElement(chartsers, "ChartSmartLabel", Nothing)
            AddElement(chartsmartlbl, "CalloutLineColor", "Black")
            AddElement(chartsmartlbl, "MinMovingDistance", "0pt")

            'ChartAreas
            Dim chartareas As XmlElement = AddElement(repgraph, "ChartAreas", Nothing)
            'ChartArea
            Dim chartarea As XmlElement = AddElement(chartareas, "ChartArea", Nothing)
            attr = chartarea.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Default"

            ''position does not work
            'Dim ChartElementPosition As XmlElement = AddElement(chartarea, "ChartElementPosition", Nothing)
            'AddElement(ChartElementPosition, "Width", "100")
            'Dim ChartInnerPlotPosition As XmlElement = AddElement(chartarea, "ChartInnerPlotPosition", Nothing)
            'AddElement(ChartInnerPlotPosition, "Width", "100")


            'ChartCategoryAxes
            Dim chartcataxes As XmlElement = AddElement(chartarea, "ChartCategoryAxes", Nothing)
            'ChartAxis primary
            Dim chartaxis As XmlElement = AddElement(chartcataxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Primary"

            Dim style As XmlElement = AddElement(chartaxis, "Style", Nothing)
            'border
            Dim border As XmlElement = AddElement(style, "Border", Nothing)
            'AddElement(border, "Style", "Solid")
            AddElement(border, "Color", "Silver")
            'AddElement(border, "Width", "0.25pt")
            AddElement(style, "FontFamily", "Arial")
            'AddElement(style, "FontStyle", "Italic")
            AddElement(style, "Color", "#333333")
            'AddElement(style, "TextDecoration", "Underline")
            'AddElement(style, "TextAlign", "Left")
            'AddElement(style, "FontWeight", "Bold")
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            Dim ChartAxisTtl As XmlElement = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            Dim fldfrname As String = GetFriendlyReportFieldName(repid, grpfld)
            AddElement(ChartAxisTtl, "Caption", fldfrname)                             '!!!!!!!!!!!!!!!!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontFamily", "Arial")
            AddElement(style, "Color", "#333333")
            AddElement(style, "FontSize", "7pt")
            If graphtype <> "pie" Then
                AddElement(chartaxis, "Margin", "False")
                AddElement(chartaxis, "Interval", "=1")
            End If
            Dim ChartMajorGridLines As XmlElement = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            AddElement(ChartMajorGridLines, "Enabled", "False")
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            Dim ChartMinorGridLines As XmlElement = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            Dim ChartMajorTickMarks As XmlElement = AddElement(chartaxis, "ChartMajorTickMarks", Nothing)
            style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Silver")
            Dim ChartMinorTickMarks As XmlElement = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            AddElement(chartaxis, "Angle", "80")
            AddElement(chartaxis, "LabelsAutoFitDisabled", "true")
            Dim ChartAxisScaleBreak As XmlElement = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'ChartAxis secondary
            chartaxis = AddElement(chartcataxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Secondary"
            style = AddElement(chartaxis, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", hdrfld)                                            '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            AddElement(ChartMajorGridLines, "Enabled", "False")
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            'ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            AddElement(chartaxis, "Location", "Opposite")
            ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'ChartValueAxes
            Dim ChartValueAxes As XmlElement = AddElement(chartarea, "ChartValueAxes", Nothing)
            'ChartAxis primary
            chartaxis = AddElement(ChartValueAxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Primary"
            style = AddElement(chartaxis, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Silver")
            AddElement(style, "FontFamily", "Arial")
            AddElement(style, "Color", "#333333")
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", Nothing)                    '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            If graphtype <> "pie" Then
                AddElement(chartaxis, "Interval", "=IIf(Max(CountRows(""Chart1_CategoryGroup"")) <= 5,1, Nothing)")
            End If
            ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "White")
            ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorTickMarks", Nothing)
            style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Silver")
            ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            If graphtype <> "pie" Then
                'AddElement(chartaxis, "Angle", "80")
                AddElement(chartaxis, "LabelsAutoFitDisabled", "true")
            End If
            ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            'ChartAxis secondary
            chartaxis = AddElement(ChartValueAxes, "ChartAxis", Nothing)
            attr = chartaxis.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Secondary"
            style = AddElement(chartaxis, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            'ChartAxisTitle
            ChartAxisTtl = AddElement(chartaxis, "ChartAxisTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", hdrfld)                                        '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "7pt")
            ChartMajorGridLines = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            AddElement(ChartMajorGridLines, "Enabled", "False")
            style = AddElement(ChartMajorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            ChartMinorGridLines = AddElement(chartaxis, "ChartMinorGridLines", Nothing)
            style = AddElement(ChartMinorGridLines, "Style", Nothing)
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "Gainsboro")
            AddElement(border, "Style", "Dotted")
            'ChartMajorTickMarks = AddElement(chartaxis, "ChartMajorGridLines", Nothing)
            'style = AddElement(ChartMajorTickMarks, "Style", Nothing)
            'border = AddElement(style, "Border", Nothing)
            'AddElement(border, "Color", "Silver")
            ChartMinorTickMarks = AddElement(chartaxis, "ChartMinorTickMarks", Nothing)
            AddElement(ChartMinorTickMarks, "Length", "0.5")
            AddElement(chartaxis, "CrossAt", "NaN")
            AddElement(chartaxis, "Minimum", "NaN")
            AddElement(chartaxis, "Maximum", "NaN")
            AddElement(chartaxis, "Location", "Opposite")
            ChartAxisScaleBreak = AddElement(chartaxis, "ChartAxisScaleBreak", Nothing)
            AddElement(ChartAxisScaleBreak, "Style", Nothing)

            style = AddElement(chartarea, "Style", Nothing)
            'AddElement(style, "BackgroundColor", "#e9ecee")
            AddElement(style, "BackgroundColor", "WhiteSmoke")
            AddElement(style, "BackgroundGradientType", "None")

            Dim ChartLegends As XmlElement = AddElement(repgraph, "ChartLegends", Nothing)
            Dim ChartLegend As XmlElement = AddElement(ChartLegends, "ChartLegend", Nothing)
            attr = ChartLegend.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "Default"
            style = AddElement(ChartLegend, "Style", Nothing)
            AddElement(style, "FontSize", "8pt")
            AddElement(style, "Color", "#333333")
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(ChartLegend, "Position", "BottomLeft")
            'AddElement(ChartLegend, "Layout", "Row")
            AddElement(ChartLegend, "Layout", "WideTable")
            AddElement(ChartLegend, "DockOutsideChartArea", "true")

            'Dim ChartElementPosition As XmlElement = AddElement(ChartLegend, "ChartElementPosition", Nothing)
            'AddElement(ChartElementPosition, "Top", "80")
            'AddElement(ChartElementPosition, "Left", "3")
            'AddElement(ChartElementPosition, "Height", "20")
            'AddElement(ChartElementPosition, "Width", "100")
            ''Dim ChartInnerPlotPosition As XmlElement = AddElement(ChartLegend, "ChartInnerPlotPosition", Nothing)
            ''AddElement(ChartInnerPlotPosition, "Width", "100")
            AddElement(ChartLegend, "HeaderSeparatorColor", "Black")
            AddElement(ChartLegend, "ColumnSeparatorColor", "Black")
            AddElement(ChartLegend, "MaxAutoSize", "40")
            AddElement(ChartLegend, "TextWrapThreshold", "40")
            ChartAxisTtl = AddElement(ChartLegend, "ChartLegendTitle", Nothing)
            AddElement(ChartAxisTtl, "Caption", Nothing)                    '!!!!!!
            style = AddElement(ChartAxisTtl, "Style", Nothing)
            AddElement(style, "FontSize", "8pt")
            AddElement(style, "TextAlign", "Center")
            AddElement(style, "FontWeight", "Bold")

            Dim ChartTitles As XmlElement = AddElement(repgraph, "ChartTitles", Nothing)
            Dim ChartTitle As XmlElement = AddElement(ChartTitles, "ChartTitle", Nothing)
            attr = ChartTitle.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = repid            '!!!!!!!!!!!!!!!!!!
            AddElement(ChartTitle, "Caption", hdrfld)
            style = AddElement(ChartTitle, "Style", Nothing)
            AddElement(style, "FontSize", "11pt")
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "FontFamily", "Trebuchet MS")
            AddElement(style, "Color", "#222222")
            AddElement(style, "VerticalAlign", "Top")
            AddElement(style, "TextAlign", "General")
            AddElement(style, "FontWeight", "Normal")

            'Custom Palette
            AddElement(repgraph, "Palette", "Custom")
            Dim ChartCustomPaletteColors As XmlElement = AddElement(repgraph, "ChartCustomPaletteColors", Nothing)
            'Dim ChartCustomPaletteColor As XmlElement = AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", Nothing)
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#8C6ED2")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#9EEAFF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#BA00FF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFF19D")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FF9DCB")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#ADB26E")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#EEEEEE")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#0DCAFF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#5D0CE8")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#75FF9F")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#5D5D5D")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#3A0497")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#F6FF73")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#A375F3")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#112AE8")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#869EFF")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#4661F6")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#EE3A63")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#C7F38A")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFFAD3")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#D78BFD")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FF8E33")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#FFE100")
            AddElement(ChartCustomPaletteColors, "ChartCustomPaletteColor", "#2CCC14")

            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "= Code.GetColorI(0)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(1)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(2)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(3)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(4)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(5)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(6)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(7)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(8)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(9)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(10)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(11)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(12)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(13)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(14)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(15)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(16)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(17)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(18)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(19)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(20)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(21)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(22)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(23)")
            'AddElement(ChartCustomPaletteColor, "ChartCustomPaletteColor", "=Code.GetColorI(24)")

            'Dim dynamicwidth As XmlElement = AddElement(repgraph, "DynamicWidth", Nothing)
            AddElement(repgraph, "DynamicWidth", wdth.ToString & "In")

            Dim ChartBorderSkin As XmlElement = AddElement(repgraph, "ChartBorderSkin", Nothing)
            style = AddElement(ChartBorderSkin, "Style", Nothing)
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "Color", "White")
            AddElement(style, "BackgroundColor", "#e9ecee")

            Dim ChartNoDataMessage As XmlElement = AddElement(repgraph, "ChartNoDataMessage", Nothing)
            attr = ChartNoDataMessage.Attributes.Append(doc.CreateAttribute("Name"))
            attr.Value = "NoDataMessage"
            AddElement(ChartNoDataMessage, "Caption", "No Data Available")
            style = AddElement(ChartNoDataMessage, "Style", Nothing)
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "VerticalAlign", "Top")
            AddElement(style, "TextAlign", "General")

            ' AddElement(repgraph, "DataSetName", repid)
            AddElement(repgraph, "Top", "0.7In")
            AddElement(repgraph, "Left", "0.0375In")
            AddElement(repgraph, "Height", "6In")
            AddElement(repgraph, "Width", "10.6In")
            style = AddElement(repgraph, "Style", Nothing)
            AddElement(style, "VerticalAlign", "Top")
            AddElement(style, "PaddingLeft", "2pt")
            AddElement(style, "PaddingRight", "2pt")
            AddElement(style, "PaddingTop", "2pt")
            AddElement(style, "PaddingBottom", "20pt")
            AddElement(style, "BackgroundGradientType", "None")
            AddElement(style, "BackgroundColor", "White")
            border = AddElement(style, "Border", Nothing)
            AddElement(border, "Color", "LightGrey")
            AddElement(border, "Style", "Solid")

            'Dim Code As XmlElement = AddElement(report, "Code", Nothing)

            '           <Code>
            '  'Private colorPalette As String() ={"#AC9494", "#37392C","#939C41"}
            '          'Private colorPalette As String() ={"Red", "Green","Blue"}
            '          Private colorPalette As String() = {"#9EEAFF", "#BA00FF", "#FFF19D", "#FF9DCB", "#8C6ED2", "#ADB26E", "#EEEEEE", "#0DCAFF", "#5D0CE8", "#75FF9F", "#5D5D5D", "#3A0497", "#F6FF73", "#A375F3", "#112AE8", "#869EFF", "#4661F6", "#EE3A63", "#C7F38A", "#FFFAD3", "#C2EBFA", "#D78BFD", "#FF8E33", "#FFE100", "#2CCC14"}
            '          Private count As Integer = 0
            '          Private mapping As New System.Collections.Hashtable()
            '  Public Function GetColor(ByVal groupingValue As String) As String
            '      If mapping.ContainsKey(groupingValue) Then
            '          Return mapping(groupingValue)
            '      End If
            '      Dim c As String = colorPalette(count Mod colorPalette.Length)
            '      count = count + 1
            '      mapping.Add(groupingValue, c)
            '      Return c
            '  End Function

            '  Public Function GetColorI(ByVal i As Integer) As String
            '      Dim ColorCode As String = colorPalette(i Mod colorPalette.Length)
            '      Return ColorCode
            '  End Function
            '</Code>



            Dim sw As New StringWriter
            Dim tx As New XmlTextWriter(sw)
            doc.WriteTo(tx)
            graf = sw.ToString
            Return graf
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
End Module

