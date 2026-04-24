Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Partial Class FriendlyNames
    Inherits System.Web.UI.Page
    Private Sub FriendlyNames_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim i, j As Integer
        ' Dim cln As String
        Dim err As String = String.Empty
        repid = Session("REPORTID")
        Dim ddt, ddc As DataTable

        'dropdown of tables 
        DropDownTables.Items.Clear()
        Dim ddtv As DataView
        ddtv = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV"))
        If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
            LabelAlert1.Text = "No tables nor fields selected into report yet..."
            LabelAlert1.Visible = True
            Exit Sub
        End If
        ddt = ddtv.Table
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownTables.Items.Add(ddt.Rows(i)("TABLE_NAME"))
        Next
        If DropDownTables.Items.Count > 0 Then
            DropDownTables.Items(0).Selected = True
            DropDownTables.SelectedValue = ddt.Rows(0)("TABLE_NAME")
            DropDownTables.SelectedIndex = 0
        End If

        'dropdown of fields
        DropDownFields.Items.Clear()
        err = ""
        Dim ddcv As DataView
        ddcv = GetListOfTableColumns(DropDownTables.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider"), err)
        If ddcv Is Nothing OrElse ddcv.Table Is Nothing OrElse err <> "" Then
            LabelAlert1.Text = err
            LabelAlert1.Visible = True
            Exit Sub
        End If
        ddc = ddcv.Table
        If Not ddc Is Nothing AndAlso ddc.Rows.Count > 0 Then
            For j = 0 To ddc.Rows.Count - 1   'draw drop-down start
                DropDownFields.Items.Add(ddc.Rows(j)("COLUMN_NAME"))
            Next
        End If
        TextBoxFieldFriendly.Text = GlobalFriendlyName(DropDownTables.SelectedItem.Text, DropDownFields.SelectedItem.Text, Session("UnitDB"), err)
        DropDownFields_SelectedIndexChanged(sender, e)
    End Sub
    Protected Sub DropDownTables_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTables.SelectedIndexChanged
        'dropdown of fields
        DropDownFields.Items(0).Selected = False
        DropDownFields.Items.Clear()
        Dim err As String = String.Empty
        Dim ddcv As DataView
        ddcv = GetListOfTableColumns(DropDownTables.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider"), err)
        If ddcv Is Nothing OrElse ddcv.Table Is Nothing OrElse err <> "" Then
            LabelAlert1.Text = err
            LabelAlert1.Visible = True
            Exit Sub
        End If
        Dim ddc As DataTable = ddcv.Table
        If Not ddc Is Nothing AndAlso ddc.Rows.Count > 0 Then
            For j = 0 To ddc.Rows.Count - 1   'draw drop-down start
                DropDownFields.Items.Add(ddc.Rows(j)("COLUMN_NAME"))
            Next
            DropDownFields.Items(0).Selected = True
        End If
        TextBoxFieldFriendly.Text = GlobalFriendlyName(DropDownTables.SelectedItem.Text, DropDownFields.SelectedItem.Text, Session("UnitDB"), err)
        DropDownFields_SelectedIndexChanged(sender, e)
    End Sub
    Protected Sub DropDownFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFields.SelectedIndexChanged
        Dim err As String = String.Empty
        TextBoxFieldFriendly.Text = GlobalFriendlyName(DropDownTables.SelectedItem.Text, DropDownFields.SelectedItem.Text, Session("UnitDB"), err)
        Dim dvf As DataView = GetAllUserTablesWithField(DropDownFields.SelectedItem.Text, Session("logon"), Session("CSV"), Session("UserConnString"), Session("UserConnProvider"), err)
        If dvf Is Nothing OrElse dvf.Table Is Nothing OrElse err <> "" Then
            LabelAlert1.Text = err
            LabelAlert1.Visible = True
            Exit Sub
        End If
        If Session("UserConnProvider") = "MySql.Data.MySqlClient" OrElse ((Session("UserConnProvider") = "" And Session("OURConnProvider") = "MySql.Data.MySqlClient")) Then
            dvf = ConvertMySqlTable(dvf.Table, err).DefaultView
        ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
            dvf = ConvertOracleTable(dvf.Table, err).DefaultView
        End If
        LabelAlert1.Visible = True
        GridView1.DataSource = dvf
        GridView1.DataBind()
    End Sub
    Protected Sub ButtonAssignFriendlyName_Click(sender As Object, e As EventArgs) Handles ButtonAssignFriendlyName.Click
        If TextBoxFieldFriendly.Text.Trim = "" Then
            LabelAlert1.Text = "Friendly Name is empty..."
            LabelAlert1.Visible = True
            Exit Sub
        End If
        Dim err As String = String.Empty
        Dim i, j As Integer
        Dim reps() As String
        Dim rep As String
        Dim msql As String = "SELECT * FROM OURFriendlyNames WHERE TableName='" & DropDownTables.SelectedItem.Text & "' AND FieldName='" & DropDownFields.SelectedItem.Text & "' AND Unit='" & Session("Unit") & "' AND UnitDB='" & Session("UnitDB") & "'"
        Dim dv As DataView = mRecords(msql, err)
        If err <> "" Then
            LabelAlert1.Text = err
            LabelAlert1.Visible = True
            Exit Sub
        End If
        'insert or update a friendly name for selected field in selected table
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            'insert friendly name
            msql = "INSERT INTO OURFriendlyNames (Unit,UnitDB,TableName,FieldName,Friendly) VALUES ('" & Session("Unit") & "','" & Session("UnitDB") & "','" & DropDownTables.SelectedItem.Text & "','" & DropDownFields.SelectedItem.Text & "','" & TextBoxFieldFriendly.Text.Trim & "')"
            err = ExequteSQLquery(msql)
            If err <> "Query executed fine." Then
                LabelAlert1.Text = err
                LabelAlert1.Visible = True
                Exit Sub
            End If
        Else
            'update friendly name
            msql = "UPDATE OURFriendlyNames SET Friendly='" & TextBoxFieldFriendly.Text.Trim & "' WHERE Unit='" & Session("Unit") & "' AND UnitDB='" & Session("UnitDB") & "' AND TableName='" & DropDownTables.SelectedItem.Text & "' AND FieldName='" & DropDownFields.SelectedItem.Text & "'"
            err = ExequteSQLquery(msql)
        End If
        err = ""
        dv = GetAllUserTablesWithField(DropDownFields.SelectedItem.Text, Session("logon"), Session("CSV"), Session("UserConnString"), Session("UserConnProvider"), err)
        If err <> "" Then
            LabelAlert1.Text = err
            LabelAlert1.Visible = True
            Exit Sub
        End If
        Dim dt As DataTable = dv.Table
        For i = 0 To dt.Rows.Count - 1
            'table fields from db
            If CheckBoxAllTableFields.Checked Then
                If dt.Rows(i)("GlobalFriendlyName").ToString.Trim = "" Then
                    'insert
                    msql = "INSERT INTO OURFriendlyNames (Unit,UnitDB,TableName,FieldName,Friendly) VALUES ('" & Session("Unit") & "','" & Session("UnitDB") & "','" & dt.Rows(i)("TABLE_NAME").ToString.Trim & "','" & dt.Rows(i)("COLUMN_NAME").ToString.Trim & "','" & TextBoxFieldFriendly.Text.Trim & "')"
                    err = ExequteSQLquery(msql)
                Else
                    'update
                    msql = "UPDATE OURFriendlyNames SET Friendly='" & TextBoxFieldFriendly.Text.Trim & "' WHERE Unit='" & Session("Unit") & "' AND UnitDB='" & Session("UnitDB") & "' AND TableName='" & dt.Rows(i)("TABLE_NAME").ToString.Trim & "' AND FieldName='" & dt.Rows(i)("COLUMN_NAME").ToString.Trim & "'"
                    err = ExequteSQLquery(msql)
                End If
            End If
            'check if reports have this field
            If dt.Rows(i)("Reports").ToString.Trim.IndexOf("<br>") > 0 Then
                reps = dt.Rows(i)("Reports").ToString.Trim.Split("<br>")
                For j = 0 To reps.Length - 1
                    If reps(j).Trim = "" Then
                        Continue For
                    End If
                    'If j = 0 Then
                    '    rep = reps(j)
                    'Else
                    rep = reps(j).Substring(reps(j).IndexOf(".") + 1).Trim
                    'End If

                    'data fields from OURReportSQLquery
                    If CheckBoxAllDataFields.Checked Then
                        If dt.Rows(i)("ReportDataFieldFriendlyName").ToString.Trim = "" Then
                            'assign
                        Else
                            'update
                            WriteToAccessLog(Session("logon"), "DataField Friendly name updated from -" & dt.Rows(i)("ReportDataFieldFriendlyName").ToString & " to " & TextBoxFieldFriendly.Text, 5)
                        End If
                        msql = "UPDATE OURReportSQLquery SET Friendly='" & TextBoxFieldFriendly.Text.Trim & "' WHERE ReportId='" & rep & "' AND Doing='SELECT' AND Tbl1='" & dt.Rows(i)("TABLE_NAME").ToString.Trim & "' AND Tbl1Fld1='" & dt.Rows(i)("COLUMN_NAME").ToString.Trim & "'"
                        err = ExequteSQLquery(msql)
                    End If
                    'report columns from OURReportFormat
                    If CheckBoxAllReportColumns.Checked Then
                        If dt.Rows(i)("ReportColumnFriendlyName").ToString.Trim = "" Then
                            'assign
                        Else
                            'update
                            WriteToAccessLog(Session("logon"), "ReportColumn Friendly name updated from -" & dt.Rows(i)("ReportColumnFriendlyName").ToString & " to " & TextBoxFieldFriendly.Text, 5)
                        End If
                        msql = "UPDATE OURReportFormat SET Prop1='" & TextBoxFieldFriendly.Text.Trim & "' WHERE ReportId='" & rep & "' AND Prop='FIELDS' AND Val='" & dt.Rows(i)("COLUMN_NAME").ToString.Trim & "'"
                        err = ExequteSQLquery(msql)
                    End If
                    'group names from OURReportGroups
                    If CheckBoxAllGroupFields.Checked Then
                        If dt.Rows(i)("ReportGroupFriendlyName").ToString.Trim = "" Then
                            'assign
                        Else
                            'update
                            WriteToAccessLog(Session("logon"), "ReportGroup Friendly name updated from -" & dt.Rows(i)("ReportGroupFriendlyName").ToString & " to " & TextBoxFieldFriendly.Text, 5)
                        End If
                        msql = "UPDATE OURReportGroups SET comments='" & TextBoxFieldFriendly.Text.Trim & "' WHERE ReportId='" & rep & "' AND GroupField='" & dt.Rows(i)("COLUMN_NAME").ToString.Trim & "'"
                        err = ExequteSQLquery(msql)
                    End If
                    'parameters labels OURReportShow
                    If CheckBoxAllParameters.Checked Then
                        If dt.Rows(i)("ReportParameterFriendlyName").ToString.Trim = "" Then
                            'assign
                        Else
                            'update
                            WriteToAccessLog(Session("logon"), "ReportParameter Friendly name updated from -" & dt.Rows(i)("ReportParameterFriendlyName").ToString & " to " & TextBoxFieldFriendly.Text, 5)
                        End If
                        msql = "UPDATE OURReportShow SET DropDownLabel='" & TextBoxFieldFriendly.Text.Trim & "' WHERE ReportId='" & rep & "' AND DropDownID='" & dt.Rows(i)("COLUMN_NAME").ToString.Trim & "'"
                        err = ExequteSQLquery(msql)
                    End If
                Next
            End If
        Next
        'Response.Redirect("FriendlyNames.aspx")
        err = ""
        Dim dvf As DataView = GetAllUserTablesWithField(DropDownFields.SelectedItem.Text, Session("logon"), Session("CSV"), Session("UserConnString"), Session("UserConnProvider"), err)
        If dvf Is Nothing OrElse dvf.Table Is Nothing OrElse err <> "" Then
            LabelAlert1.Text = err
            LabelAlert1.Visible = True
            Exit Sub
        End If
        If Session("UserConnProvider") = "MySql.Data.MySqlClient" OrElse ((Session("UserConnProvider") = "" And Session("OURConnProvider") = "MySql.Data.MySqlClient")) Then
            dvf = ConvertMySqlTable(dvf.Table, err).DefaultView
        ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
            dvf = ConvertOracleTable(dvf.Table, err).DefaultView
        End If
        LabelAlert1.Visible = True
        GridView1.DataSource = dvf
        GridView1.DataBind()
    End Sub

End Class
