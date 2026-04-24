<%@ WebHandler Language="VB" Class="HandlerKML" %>

Imports System
Imports System.Web

Public Class HandlerKML : Implements IHttpHandler

    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        context.Response.ContentType = "application/vnd.google-earth.kml+xml"
        'context.Response.Write("Hello World")
        context.Response.AddHeader("Content-Disposition", "attachment; filename=" & kmlPath)
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class