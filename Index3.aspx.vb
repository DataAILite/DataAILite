Partial Class Index3
    Inherits System.Web.UI.Page

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'Response.Redirect(Session("WEBHELPDESK").ToString & "UnitRegistration.aspx?org=company")
        Response.Redirect("UnitRegistration.aspx?org=company")
    End Sub
    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'Response.Redirect(Session("WEBOUR").ToString & "Registration.aspx")
        Response.Redirect("Registration.aspx")
    End Sub

    Private Sub Index3_Init(sender As Object, e As EventArgs) Handles Me.Init
        Session("WEBOUR") = ConfigurationManager.AppSettings("weboureports").ToString
        Session("WEBHELPDESK") = ConfigurationManager.AppSettings("webhelpdesk").ToString
        Session("CSV") = ""
        Session("UnitIndx") = Nothing
        Session("PAGETTL") = ConfigurationManager.AppSettings("pagettl").ToString
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
    End Sub
    'Protected Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
    '    Response.Redirect(Session("WEBHELPDESK").ToString & "OUReportsAgents.aspx")
    'End Sub
End Class
