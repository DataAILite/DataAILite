Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Partial Class AddParameters
    Inherits System.Web.UI.Page
    Dim repid As String
    Private Sub AddParameters_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
    End Sub
    Private Sub AddParameters_Load(sender As Object, e As EventArgs) Handles Me.Load
        repid = Session("REPORTID")
        Dim chk As Integer
        chk = 0

        Dim dvr, dvm, dvp As DataView
        Dim SQLq, mSQL, insSQL, par, typ, replacement As String
        Dim i, n As Integer
        SQLq = String.Empty
        replacement = String.Empty
        'list of report parameters
        dvp = mRecords("SELECT DropDownID, DropDownLabel, DropDownFieldName, DropDownFieldType, DropDownSQL, comments FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
        n = dvp.Table.Rows.Count

        'get SQL
        dvr = mRecords("Select * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
        If dvr.Table.Rows.Count = 1 Then
            If dvr.Table.Rows(0)("ReportAttributes").ToString = "sql" Then
                SQLq = dvr.Table.Rows(0)("SQLquerytext").ToString
            End If
        End If

        'loop in all parameters
        'Dim applpath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        dvm = ReturnDataViewFromXML(xsdfile)
        For i = 0 To dvm.Table.Columns.Count - 1
            par = dvm.Table.Columns(i).Caption
            If Request(par) = "on" Then
                'check if par is already in the list of report parameters
                chk = 1
                For j = 0 To dvp.Table.Rows.Count - 1
                    If dvp.Table.Rows(j)("DropDownFieldName") = par Then
                        chk = 0
                        Exit For
                    End If
                Next
                If chk = 1 Then 'new parameter
                    'fix SQLq renaming double fields in select statement
                    Dim SQLqSelect, SQLqFrom, lbl, nw As String
                    SQLqFrom = SQLq.Substring(SQLq.ToUpper.IndexOf(" FROM"))
                    SQLqSelect = SQLq.Substring(0, SQLq.ToUpper.IndexOf(" FROM"))
                    If IsCacheDatabase() Then
                        'Remove TOP n, because Cache working strange if included sql has TOP n , it return subset of distinct values
                        Dim k As Integer = SQLqSelect.ToUpper.IndexOf(" TOP ")
                        If k > 0 Then
                            SQLqSelect = SQLqSelect.Substring(k + 5).Trim
                            SQLqSelect = SQLqSelect.Substring(SQLqSelect.IndexOf(" "))
                            SQLqSelect = " SELECT " & SQLqSelect
                        End If
                    End If
                    SQLqSelect = FixDoubleFieldNames(repid, SQLqSelect, Session("UserConnProvider"))
                    SQLq = SQLqSelect & " " & SQLqFrom
                    'add parameter par into ReportShow table
                    'DISTINCT in Cache return upper case and need a correction in where statement!!
                    mSQL = "SELECT DISTINCT sub." & par & " FROM (" & SQLq & ") sub"
                    'label for dropdown
                    nw = ""
                    lbl = par
                    lbl = GetFriendlyFieldName(repid, par, nw)
                    If lbl = "" OrElse lbl = "none" Then
                        lbl = par
                    End If
                    typ = dvm.Table.Columns(i).DataType.FullName
                    If typ = "System.String" Then
                        typ = "nvarchar"
                    ElseIf typ = "System.DateTime" Then
                        typ = "datetime"
                    Else
                        typ = "int"
                    End If
                    insSQL = "INSERT INTO OURReportShow (ReportID,DropDownID,DropDownLabel,DropDownName,DropDownFieldName,DropDownFieldType,DropDownFieldValue,DropDownSQL,COMMENTS) VALUES('" & Session("REPORTID") & "','" & par & "','" & lbl & "','" & par & "','" & par & "','" & typ & "','" & par & "','" & mSQL & "','checked')"
                    Dim ret As String = ExequteSQLquery(insSQL)
                    Dim logs As String = "Parameter " & par & " for report " & Session("REPORTID") & " has been created."
                    WriteToAccessLog(Session("logon"), logs, 2)
                    ''send emails
                    'Dim EmailTable As DataTable
                    'EmailTable = mRecords("SELECT  * FROM OURPermissions WHERE APPLICATION='InteractiveReporting' AND Param2='email' AND (Param1='" & Session("REPORTID") & "') AND AccessLevel='admin'").Table
                    'If EmailTable.Rows.Count > 0 Then
                    '    For j = 0 To EmailTable.Rows.Count - 1
                    '        SendHTMLEmail("", "Parameter for report " & Session("REPORTID") & " has been created", "Parameter " & par & " has been created by NetId " & Session("logon"), EmailTable.Rows(j)("Param3"), Session("SupportEmail"))
                    '    Next
                    'End If
                End If
            End If

        Next

        Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")

    End Sub
End Class
