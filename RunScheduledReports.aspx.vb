
Imports System.Data
Imports System.Threading
Imports System.Windows.Forms

Partial Class RunScheduledReports
    Inherits System.Web.UI.Page
    Private Sub RunScheduledReports_Init(sender As Object, e As EventArgs) Handles Me.Init
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
        If Session("nruntimes") Is Nothing Then
            Session("nruntimes") = 0
            Session("nreports") = 0
        End If
        Session.Timeout = Session.Timeout + 5
        Dim ret As String = String.Empty

        If Request("todown") Is Nothing OrElse Request("todown").ToString.Trim <> "yes" Then
            Dim i As Integer = 0
            Dim re As String = ""
            SendEmailUserScheduledReports(Session("logon"), Session("SupportEmail"), re)
            If re.Trim <> "" Then
                Dim repids() As String = re.Split(",")

                For i = 0 To repids.Length - 1
                    If repids(i).Trim <> "" Then
                        Dim er As String = String.Empty
                        Try
                            Dim sqls As String = String.Empty
                            If Session("admin") = "super" Then
                                sqls = "SELECT * FROM [OURScheduledReports] WHERE ID='" & Piece(repids(i).Trim, ";", 1) & "' "
                            Else
                                sqls = "SELECT * FROM [OURScheduledReports] WHERE ID='" & Piece(repids(i).Trim, ";", 1) & "' AND UserId='" & Session("logon") & "' "
                            End If
                            Dim dts As DataView = mRecords(sqls, er)
                            If dts Is Nothing OrElse dts.Count = 0 Then
                                Exit Sub
                            End If
                            Try
                                If dts.Table.Rows(0)("Prop4").ToString.Trim <> "" Then
                                    Session("pagewidth") = Piece(dts.Table.Rows(0)("Prop4").ToString, "~", 1)
                                    Session("pageheight") = Piece(dts.Table.Rows(0)("Prop4").ToString, "~", 2)
                                End If
                                If dts.Table.Rows(0)("Prop5").ToString.Trim <> "" Then
                                    er = "&" & dts.Table.Rows(0)("Prop5").ToString.Trim
                                End If
                            Catch ex As Exception

                            End Try
                            Session("filter") = dts.Table.Rows(0)("Filters").Replace("""", "'")
                            Session("WhereText") = dts.Table.Rows(0)("WhereText").Replace("""", "'")
                        Catch ex As Exception

                        End Try
                        Session("ReportID") = Piece(repids(i).Trim, ";", 2)
                        Session("nreports") = Session("nreports") + 1
                        'run and download report in TEMP folder
                        Dim url As String = "ReportViews.aspx?srd=6&runschedreps=yes&schedid=" & Piece(repids(i).Trim, ";", 1) & er
                        Response.Redirect(url)
                    End If

                Next
            End If
        End If
    End Sub
    Private Sub RunScheduledReports_Load(sender As Object, e As EventArgs) Handles Me.Load

        LabelNruns.Text = "Number of downloads to local computer: " & Session("nreports").ToString & ",  Number of running the page: " & Session("ntimerticks").ToString & ", timeout in " & Session.Timeout.ToString & " minutes"
        LabelNruns.Focus()


    End Sub

    Private Sub RunTimer_Tick(sender As Object, e As EventArgs) Handles RunTimer.Tick
        Session("ntimerticks") = Session("ntimerticks") + 1
        Session.Timeout = Session.Timeout + 5
        Thread.Sleep(500000)
        Response.Redirect("RunScheduledReports.aspx")
    End Sub

End Class
