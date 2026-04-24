
Partial Class SendEmailsForScheduledReports
    Inherits System.Web.UI.Page

    Private Sub SendEmailsForScheduledReports_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim re As String = SendEmailAdminScheduledReports()
        Response.End()
        Me.Dispose()
    End Sub
End Class
