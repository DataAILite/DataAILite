Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.sqlClient
Imports System.IO
Partial Class ShowCrystalReport
    Inherits System.Web.UI.Page
    Public MyconnStr As String = ConfigurationManager.ConnectionStrings.Item("MySQLConnection").ToString
    Public Myconn As SqlConnection
    Public cmdReport As SqlCommand
    Public dt As DataTable
    Public dr As DataRow
    Public dv1, dv2, dv3, dv4 As DataView
    Public da As SqlDataAdapter
    Public sp, repid, repname, reptitle, WhereText As String
    'Public Q, i, j, nrec, ncol, ndd As Integer
    Public ParamNames() As String
    Public ParamValues() As String
    Public ParamTypes() As String

    Private Sub ShowCrystalReport_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim n As Integer
        Dim i, k As Integer
        repid = Request("REPORT")
        Session("REPORTID") = repid

        If repid <> "" Then

            'dv1 - Report Info (title, data for report)
            dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")

            'DEFINE PARAMvalue() FROM ReportShow !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            If dv1.Table.Rows.Count = 1 Then
                reptitle = dv1.Table.Rows(0)("ReportTtl")
                Session("REPTITLE") = repid
                sp = dv1.Table.Rows(0)("SQLquerytext")
                cmdReport = New SqlCommand(MyconnStr)
                cmdReport.CommandType = Data.CommandType.StoredProcedure
                cmdReport.CommandText = sp
                Dim ParamName As String
                k = CInt(dv1.Table.Rows(0)("ParamQuantity"))
                For i = 0 To k - 1
                    ParamName = "@" & dv1.Table.Rows(0).Item(9 + 2 * i).ToString
                    If Trim(dv1.Table.Rows(0).Item(10 + 2 * i).ToString) = "nvarchar" Then
                        cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParamName, System.Data.SqlDbType.NVarChar))
                        cmdReport.Parameters.Item(i).Value = ParamValue(i)
                    End If
                    If Trim(dv1.Table.Rows(0).Item(10 + 2 * i).ToString) = "datetime" Then
                        cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParamName, System.Data.SqlDbType.NVarChar))
                        cmdReport.Parameters.Item(i).Value = ParamValue(i)
                    End If
                    If Trim(dv1.Table.Rows(0).Item(10 + 2 * n).ToString) = "int" Then
                        cmdReport.Parameters.Add(New System.Data.SqlClient.SqlParameter(ParamName, System.Data.SqlDbType.Int))
                        cmdReport.Parameters.Item(i).Value = CInt(ParamValue(i))
                    End If
                Next
            Else
                Response.Redirect("Nodata.aspx")
            End If
        End If
    End Sub


End Class
