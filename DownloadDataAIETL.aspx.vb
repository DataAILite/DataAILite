Imports System
Imports System.Configuration
Imports System.Globalization
Imports System.IO
Imports System.Web

Partial Class DownloadDataAIETL
    Inherits System.Web.UI.Page

    Private Shared ReadOnly AllowedExtensions As String() = {
        ".zip", ".nupkg", ".yxi", ".sha256", ".json", ".md", ".txt"
    }

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim channel As String = If(Request.QueryString("channel"), String.Empty).Trim().ToLowerInvariant()
        Dim requestedFile As String = If(Request.QueryString("file"), String.Empty).Trim()
        Dim channelFolder As String

        Select Case channel
            Case "evaluation"
                channelFolder = "DataAIETLevaluation"
            Case "evaluation-customer-ready"
                channelFolder = "DataAIETLevalCustomerReady"
            Case "production"
                channelFolder = "DataAIETLproduction"
            Case Else
                SendError(400, "The download channel must be evaluation, evaluation-customer-ready, or production.")
                Return
        End Select

        If requestedFile.Length = 0 OrElse
           Not String.Equals(Path.GetFileName(requestedFile), requestedFile, StringComparison.Ordinal) Then
            SendError(400, "The requested file name is invalid.")
            Return
        End If

        Dim extension As String = Path.GetExtension(requestedFile).ToLowerInvariant()
        If Array.IndexOf(AllowedExtensions, extension) < 0 Then
            SendError(400, "This file type is not available through the download endpoint.")
            Return
        End If

        Dim configuredUploadRoot As String = ConfigurationManager.AppSettings("fileupload")
        If String.IsNullOrWhiteSpace(configuredUploadRoot) Then
            SendError(500, "The upload directory is not configured.")
            Return
        End If

        Dim channelRoot As String = Path.GetFullPath(
            Path.Combine(configuredUploadRoot, "SAVEDFILES", channelFolder))
        Dim resolvedFile As String = Path.GetFullPath(Path.Combine(channelRoot, requestedFile))
        Dim requiredPrefix As String = channelRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) &
            Path.DirectorySeparatorChar

        If Not resolvedFile.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase) OrElse
           Not File.Exists(resolvedFile) Then
            SendError(404, "The requested DataAI ETL file was not found.")
            Return
        End If

        Dim fileInfo As New FileInfo(resolvedFile)
        Response.Clear()
        Response.BufferOutput = False
        Response.ContentType = ContentTypeFor(extension)
        Response.Cache.SetCacheability(HttpCacheability.Private)
        Response.Cache.SetNoStore()
        Response.AppendHeader("X-Content-Type-Options", "nosniff")
        Response.AppendHeader("Content-Length", fileInfo.Length.ToString(CultureInfo.InvariantCulture))
        Response.AppendHeader("Content-Disposition", "attachment; filename=""" &
            requestedFile.Replace("""", String.Empty) & """")

        If Not String.Equals(Request.HttpMethod, "HEAD", StringComparison.OrdinalIgnoreCase) Then
            Response.TransmitFile(resolvedFile)
        End If

        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Shared Function ContentTypeFor(extension As String) As String
        Select Case extension
            Case ".zip"
                Return "application/zip"
            Case ".json"
                Return "application/json"
            Case ".sha256"
                Return "text/plain; charset=utf-8"
            Case ".txt"
                Return "text/plain; charset=utf-8"
            Case ".md"
                Return "text/markdown; charset=utf-8"
            Case Else
                Return "application/octet-stream"
        End Select
    End Function

    Private Sub SendError(statusCode As Integer, message As String)
        Response.Clear()
        Response.StatusCode = statusCode
        Response.TrySkipIisCustomErrors = True
        Response.ContentType = "text/plain; charset=utf-8"
        Response.Write(message)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
End Class
