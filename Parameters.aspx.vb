Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Partial Class Parameters
    Inherits System.Web.UI.Page
    Dim repid As String
    Private Sub Parameters_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        repid = Session("REPORTID")
        Dim dvm As DataView
        Dim par, ret As String
        Dim i As Integer
        'all parameters
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        dvm = ReturnDataViewFromXML(xsdfile)
        If dvm.Count = 0 Then
            Try
                Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                'update XSD
                Dim xsdpath As String = applpath & "XSDFILES\"
                Dim err As String = String.Empty
                Dim dt As DataTable = mRecords(sqlquerytext, err, Session("UserConnString"), Session("UserConnProvider")).Table
                ret = CreateXSDForDataTable(dt, repid, xsdpath)
                dvm = ReturnDataViewFromXML(xsdfile)
            Catch ex As Exception
                LabelError.Text = ex.Message
                Exit Sub
            End Try
        End If
        For i = 0 To dvm.Table.Columns.Count - 1
            par = dvm.Table.Columns(i).Caption
            CreateRowInHTMLtable(TableParams)
            'TO PASS VALUE TO REQUEST the name of control is required!
            TableParams.Rows(i + 1).Cells(0).InnerHtml = "<input id='" & par & "' type='checkbox' name='" & par & "'/>  "
            TableParams.Rows(i + 1).Cells(1).InnerHtml = "&nbsp;&nbsp;&nbsp;" & par
        Next
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        repid = Session("REPORTID")
        'Dim dvr, dvm, dvp As DataView
        'Dim SQLq, par As String
        'Dim i, n As Integer

        ''checked parameters
        'dvp = mRecords("SELECT DropDownID, DropDownLabel, DropDownFieldName, DropDownFieldType, DropDownSQL, comments FROM OURReportShow WHERE(ReportID ='" & repid & "') ORDER BY Indx")
        'n = dvp.Table.Rows.Count

        'dvr = mRecords("Select * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
        'If dvr.Table.Rows.Count = 1 Then
        '    If dvr.Table.Rows(0)("ReportAttributes").ToString = "sql" Then
        '        SQLq = dvr.Table.Rows(0)("SQLquerytext").ToString


        '    End If
        'End If



    End Sub


End Class
