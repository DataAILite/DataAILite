Imports System.Data
Imports System.Data.SqlClient
Imports System.Drawing
Partial Class ShowBusinessProposal
    Inherits System.Web.UI.Page
    Dim indx As String
    Dim distrmod As String
    Private Sub ShowBusinessProposal_Init(sender As Object, e As EventArgs) Handles Me.Init
        'If Session Is Nothing Then
        '    Response.Redirect("~/Default.aspx?msg=SessionExpired")
        'End If
        If Not Request("unit") Is Nothing Then
            indx = Request("unit").ToString.Trim
        Else
            indx = ""
        End If
    End Sub

    Private Sub ShowBusinessProposal_Load(sender As Object, e As EventArgs) Handles Me.Load
        If indx <> "" Then
            Dim mSql As String = "SELECT * FROM OURUnits WHERE Indx='" & indx & "'"
            Dim er As String = String.Empty
            Dim dt As DataTable = mRecords(mSql, er).Table  'Data for report by SQL statement from the OURdb database
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Response.Redirect("Default.aspx?msg=Unit is not found.")
                Exit Sub
            Else
                Label1.Text = dt.Rows(0)("Unit").ToString.Trim
                distrmod = dt.Rows(0)("DistrMode").ToString.Trim.Replace(" ", "")
            End If
        End If

    End Sub
End Class
