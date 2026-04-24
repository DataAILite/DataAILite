
Imports System.Threading

Partial Class RunScheduledItems
    Inherits System.Web.UI.Page
    Private Sub RunScheduledItems_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Session("ntimerticks") = 0 Then
            Session.Timeout = 600
        End If
        Session("ntimerticks") = Session("ntimerticks") + 1
        Session.Timeout = Session.Timeout + 5
        'Dim cook As New HttpCookie("ASP.NET_SessionId")
        'cook.Value = Session.SessionID
        'cook.Expires = Date.Now.AddYears(10)
        'Response.Cookies.Add(cook)
        'HyperLink1.Focus()
        Dim i As Integer = 0
        Dim re As String = ScheduledDownloadsAndSendEmail(Session("logon"), Session("SupportEmail"))
        If re.Trim <> "" Then
            Dim retfiles() As String = re.Split(",")
            Dim retfilename As String = String.Empty
            For i = 0 To retfiles.Length - 1
                retfilename = ""
                If retfiles(i).LastIndexOf("\") > 1 Then
                    retfilename = retfiles(i).Substring(retfiles(i).LastIndexOf("\") + 1)
                End If
                If retfilename.Trim <> "" Then
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & retfilename)
                        Response.TransmitFile(retfiles(i))
                        Session("nruntimes") = Session("nruntimes") + 1
                        Try
                            Dim sqlu As String = "UPDATE ourscheduleddownloads SET [Status]='downloaded on local' WHERE Prop2='" & retfilename & "'"
                            Dim ret As String = ExequteSQLquery(sqlu)
                        Catch ex As Exception

                        End Try

                    Catch ex As Exception
                        re = "ERROR!! downloading " & retfilename & ":  " & ex.Message
                    End Try
                    Response.End()
                End If
            Next
        End If
    End Sub
    Private Sub RunScheduledItems_Load(sender As Object, e As EventArgs) Handles Me.Load
        LabelNruns.Text = "Number of downloads to local computer: " & Session("nruntimes").ToString & ",  Number of running the page: " & Session("ntimerticks").ToString & ", timeout in " & Session.Timeout.ToString & " minutes"
        LabelNruns.Focus()
    End Sub

    Private Sub RunTimer_Tick(sender As Object, e As EventArgs) Handles RunTimer.Tick
        Session("ntimerticks") = Session("ntimerticks") + 1
        Session.Timeout = Session.Timeout + 5
        Thread.Sleep(5000)
        Response.Redirect("RunScheduledItems.aspx")
    End Sub
End Class
