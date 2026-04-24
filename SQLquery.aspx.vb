Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports Newtonsoft.Json
Partial Class SQLquery
    Inherits System.Web.UI.Page
    Dim repid As String
    Private mWhereTable As String
    Private mWhereField As String
    Private mWhereFieldType As String
    Private mWhereOperator As String = "="

    Private mReportTables As ListItemCollection = Nothing
    Private msTableData As String = String.Empty
    Private mReportFields As ListItemCollection = Nothing
    Private Sub SetLists(ReportID As String)
        Dim ddt As DataTable = GetReportTablesFromSQLqueryText(ReportID)
        Dim i As Integer = 0
        Dim ii As Integer = 0
        Dim li As ListItem = Nothing
        Dim tbl As String = String.Empty
        Dim fld As String = String.Empty
        Dim fldtype As String = String.Empty

        Dim sReportTables As String = String.Empty

        mReportFields = New ListItemCollection
        mReportTables = New ListItemCollection

        If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
            For i = 0 To ddt.Rows.Count - 1
                tbl = ddt.Rows(i)("Tbl1").ToString
                tbl = tbl.Replace("`", "").Replace("[", "").Replace("]", "")
                If i = 0 Then
                    sReportTables = tbl
                Else
                    sReportTables &= "," & tbl
                End If
                mReportTables.Add(tbl)
            Next
            'ElseIf Session("CSV") = "yes" AndAlso mReportTables.Count = 0 Then
            '    Response.Redirect("ReportEdit.aspx")
        End If
        hdnReportTables.Value = sReportTables

        mReportFields.Add(New ListItem(" ", " "))
        For i = 0 To mReportTables.Count - 1
            'tbl = mReportTables.Item(i).Text
            tbl = mReportTables.Item(i).Value
            ddt = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
            If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
                For ii = 0 To ddt.Rows.Count - 1
                    fld = ddt.Rows(ii)("COLUMN_NAME").ToString
                    fldtype = GetFieldDataType(tbl, fld, Session("UserConnString"), Session("UserConnProvider"))
                    fld = tbl & "." & fld
                    li = New ListItem(fld, fldtype)
                    mReportFields.Add(li)
                Next
            End If
        Next
    End Sub
    Private Sub SQLquery_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Not IsPostBack AndAlso Not Request("tnq") Is Nothing AndAlso (Session("TabNQ") Is Nothing OrElse Session("TabNQ") <> Request("tnq")) Then
            Session("TabNQ") = Request("tnq")
            lblView.Text = Menu1.Items(Session("TabNQ")).Text
        End If

        Dim i As Integer
        Dim cln As String
        repid = Session("REPORTID")

        SetLists(repid)
        If Not IsPostBack Then
            msTableData = String.Empty
            Dim sData As String
            Dim ddt, ddc As DataTable
            Dim err As String = String.Empty
            Dim ddtv As DataView = Nothing
            'list of tables on "select" tab include all available tables
            DropDownTables.Items.Clear()
            For i = 0 To mReportTables.Count - 1
                DropDownTables.Items.Add(mReportTables(i))
                sData = mReportTables(i).Text & "~" & mReportTables(i).Value
                If i = 0 Then
                    msTableData = sData
                Else
                    msTableData = msTableData & "," & sData
                End If
            Next

            If Session("CSV") = "yes" Then
                CheckBoxSysTables.Checked = False
                CheckBoxSysTables.Visible = False
                CheckBoxSysTables.Enabled = False
            ElseIf Session("admin") = "super" OrElse Session("Access") = "SITEADMIN" Then
                CheckBoxSysTables.Checked = True
            End If
            ddtv = GetListOfUserTables(CheckBoxSysTables.Checked, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV"))
            If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                LabelAlert1.Text = "No tables nor fields selected into report yet..."
                LabelAlert1.Visible = True
                Exit Sub
            End If
            If err <> "" Then
                LabelAlert1.Text = err
                LabelAlert1.Visible = True
                Exit Sub
            End If

            ddt = ddtv.Table
            If Session("CSV") = "yes" AndAlso mReportTables.Count = 0 AndAlso ddt.Rows.Count = 0 Then
                Response.Redirect("ReportEdit.aspx")
            End If

            For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                'DropDownTables.Items.Add(ddt.Rows(i)("TABLE_NAME").ToString)
                ''If Session("CSV") = "yes" Then DropDownTables.Items(i).Text = ddt.Rows(i)("TABLE_NAME").ToString & " " & ddt.Rows(i)("TABLENAME").ToString
                Dim li As New ListItem
                If Session("CSV") = "yes" Then
                    'li.Text = ddt.Rows(i)("TABLENAME").ToString & "(" & ddt.Rows(i)("TABLE_NAME").ToString & ")"
                    li.Text = ddt.Rows(i)("TABLE_NAME").ToString
                    If IsColumnFromDataTable(ddt, "TABLENAME") AndAlso ddt.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddt.Rows(i)("TABLENAME").ToString.Trim <> ddt.Rows(i)("TABLE_NAME").ToString.Trim Then
                        li.Text = ddt.Rows(i)("TABLE_NAME").ToString & "(" & ddt.Rows(i)("TABLENAME").ToString & ")"
                    End If
                Else
                    li.Text = ddt.Rows(i)("TABLE_NAME").ToString.ToLower
                End If
                li.Value = ddt.Rows(i)("TABLE_NAME").ToString.ToLower
                sData = li.Text & "~" & li.Value

                If Not TableInReportQuery(li.Value) Then
                If DropDownTables.Items.FindByText(li.Text) Is Nothing Then
                    DropDownTables.Items.Add(li)
                        If msTableData.Length = 0 Then
                            msTableData = sData
                        Else
                            msTableData = msTableData & "," & sData
                        End If
                    End If
                End If

            Next
                hdnAllTables.Value = msTableData

            If DropDownTables.Items.Count > 0 Then
                DropDownTables.Items(0).Selected = True
                'DropDownTables.SelectedValue = ddt.Rows(0)("TABLE_NAME")
                'DropDownTables.SelectedIndex = 0
            Else

                Response.Redirect("ReportEdit.aspx?msg=No tables found. Import new data.")
            End If
            'list of tables on "join" and "where" tabs include tables selected to the report only
            Dim ddtj1 As DataTable
            Dim ddtj2 As DataTable
            'ddt = GetReportTables(repid)
            ddt = GetReportTablesFromSQLqueryText(repid)
            If CheckBoxAllTables1.Checked Then
                ddtj1 = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV")).Table
            Else
                ddtj1 = ddt
            End If
            If CheckBoxAllTables2.Checked Then
                ddtj2 = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV")).Table
            Else
                ddtj2 = ddt
            End If
            If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
                Dim seltbl As String = ddt.Rows(0)("Tbl1")
                For i = 0 To DropDownTables.Items.Count - 1
                    If DropDownTables.Items(i).Value = seltbl Then
                        DropDownTables.SelectedValue = seltbl
                        DropDownTables.SelectedIndex = i
                    End If
                Next
                For i = 0 To ddtj1.Rows.Count - 1   'draw drop-down start
                    DropDownTableJ1.Items.Add(ddtj1.Rows(i)("Tbl1"))
                Next
                For i = 0 To ddtj2.Rows.Count - 1   'draw drop-down start
                    DropDownTableJ2.Items.Add(ddtj2.Rows(i)("Tbl1"))
                Next
                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    DropDownTableW1.Items.Add(ddt.Rows(i)("Tbl1"))
                    DropDownTableS1.Items.Add(ddt.Rows(i)("Tbl1"))
                Next
                If DropDownTableJ1.Items.Count > 0 Then
                    DropDownTableJ1.Items(0).Selected = True
                End If
                If DropDownTableJ2.Items.Count > 0 Then
                    DropDownTableJ2.Items(0).Selected = True
                End If
                If DropDownTableW1.Items.Count > 0 Then
                    DropDownTableW1.Items(0).Selected = True
                End If
                Dim tbl As String = String.Empty

                'fields
                DropDownFieldJ1.Items.Clear()
                DropDownFieldJ2.Items.Clear()
                DropDownFieldW1.Items.Clear()
                DropDownFieldW2.Items.Clear()
                DropDownFieldW3.Items.Clear()

                DropDownFieldW4.Items.Clear()
                DropDownFieldW5.Items.Clear()

                DropDownFieldW1.Items.Add(" ")
                DropDownFieldW2.Items.Add(" ")
                DropDownFieldW3.Items.Add(" ")
                DropDownFieldW4.Items.Add(" ")
                DropDownFieldW5.Items.Add(" ")

                'sorting
                DropDownFieldS1.Items.Clear()
                tbl = DropDownTableS1.SelectedItem.Value.Replace("`", "")
                tbl = tbl.Replace("[", "").Replace("]", "").Trim
                ddt = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
                For ii As Integer = 0 To ddt.Rows.Count - 1
                    If IsDBNull(ddt.Rows(ii)("COLUMN_NAME")) Then
                        Continue For
                    End If
                    If ddt.Rows(ii)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldS1.Items.Add(ddt.Rows(ii)("COLUMN_NAME").ToString)
                Next

                For i = 0 To DropDownTableW1.Items.Count - 1
                    'tbl = DropDownTableW1.Items(i).Text.Replace("`", "")
                    tbl = DropDownTableW1.Items(i).Value.Replace("`", "")
                    tbl = tbl.Replace("[", "").Replace("]", "").Trim
                    ddt = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
                    For ii As Integer = 0 To ddt.Rows.Count - 1
                        If IsDBNull(ddt.Rows(ii)("COLUMN_NAME")) Then
                            Continue For
                        End If
                        If ddt.Rows(ii)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldW1.Items.Add(tbl & "." & ddt.Rows(ii)("COLUMN_NAME"))
                        If ddt.Rows(ii)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldW2.Items.Add(tbl & "." & ddt.Rows(ii)("COLUMN_NAME"))
                        If ddt.Rows(ii)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldW3.Items.Add(tbl & "." & ddt.Rows(ii)("COLUMN_NAME"))
                        If ddt.Rows(ii)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldW4.Items.Add(tbl & "." & ddt.Rows(ii)("COLUMN_NAME"))
                        If ddt.Rows(ii)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldW5.Items.Add(tbl & "." & ddt.Rows(ii)("COLUMN_NAME"))
                        'DropDownFieldS1.Items.Add(ddt.Rows(ii)("COLUMN_NAME"))
                    Next
                Next
                ddt = GetListOfTableColumns(DropDownTableJ1.SelectedItem.Value, Session("UserConnString"), Session("UserConnProvider")).Table
                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    If IsDBNull(ddt.Rows(i)("COLUMN_NAME")) Then
                        Continue For
                    End If
                    If ddt.Rows(i)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldJ1.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
                Next
                ddt = GetListOfTableColumns(DropDownTableJ2.SelectedItem.Value, Session("UserConnString"), Session("UserConnProvider")).Table
                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    If IsDBNull(ddt.Rows(i)("COLUMN_NAME")) Then
                        Continue For
                    End If
                    If ddt.Rows(i)("COLUMN_NAME").ToString.Trim <> "" Then DropDownFieldJ2.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
                Next
            End If

            'End If

            'fields
            DropDownColumns.Items.Clear()
            err = ""
            Dim ddcv As DataView
            ddcv = GetListOfTableColumns(DropDownTables.SelectedItem.Value, Session("UserConnString"), Session("UserConnProvider"), err)
            If ddcv Is Nothing OrElse ddcv.Table Is Nothing OrElse err <> "" Then
                LabelAlert1.Text = err
                LabelAlert1.Visible = True
                Exit Sub
            End If
            ddc = ddcv.Table
            Dim selfields As String = String.Empty
            selfields = GetListOfSelectedFieldsFromSQLquery(repid)  ' get list of fields from sql query in commas
            If selfields <> "" Then
                selfields = selfields.Replace("`", "")
                selfields = selfields.Replace("[", "").Replace("]", "")
                selfields = selfields.Replace("""", "")
            End If
            Session("SELECTEDFields") = selfields
            Dim j As Integer
            If Not ddc Is Nothing AndAlso ddc.Rows.Count > 0 Then
                'Dim bAllSelected As Boolean = True
                For j = 0 To ddc.Rows.Count - 1   'draw drop-down start
                    If IsDBNull(ddc.Rows(j)("COLUMN_NAME")) OrElse ddc.Rows(j)("COLUMN_NAME").ToString.Trim = "" Then
                        Continue For
                    End If
                    DropDownColumns.Items.Add(ddc.Rows(j)("COLUMN_NAME"))
                    cln = "," & DropDownTables.SelectedItem.Value & "." & ddc.Rows(j)("COLUMN_NAME").ToString & ","
                    Dim idx As Integer = selfields.IndexOf(cln)
                    If DropDownColumns.Items(j).Selected OrElse (selfields.Length > 2 AndAlso selfields.IndexOf(cln) >= 0) OrElse selfields = ",*," OrElse selfields.Contains("," & DropDownTables.SelectedItem.Value & ".*,") Then
                        DropDownColumns.Items(j).Selected = True
                    Else
                        DropDownColumns.Items(j).Selected = False
                        'bAllSelected = False
                    End If
                Next
                If DropDownColumns.AllSelected Then
                    DropDownColumns.Text = "ALL"
                    btnSelectAll.Enabled = False
                    btnSelectAll.CssClass = "DataButtonDisabled"
                    btnUnselectAll.Enabled = True
                    btnUnselectAll.CssClass = "DataButtonEnabled"

                ElseIf DropDownColumns.NoneSelected Then
                    DropDownColumns.Text = "Please select..."
                    btnSelectAll.Enabled = True
                    btnSelectAll.CssClass = "DataButtonEnabled"
                    btnUnselectAll.Enabled = False
                    btnUnselectAll.CssClass = "DataButtonDisabled"
                Else
                    Dim items As ListItemCollection = DropDownColumns.SelectedItems
                    Dim sSelected As String = String.Empty

                    For j = 0 To items.Count - 1
                        sSelected = IIf(sSelected = "", items.Item(j).Text, sSelected & "," & items.Item(j).Text)
                    Next
                    DropDownColumns.Text = sSelected
                    DropDownColumns.SelectedItemsString = sSelected
                    btnSelectAll.Enabled = True
                    btnSelectAll.CssClass = "DataButtonEnabled"
                    btnUnselectAll.Enabled = True
                    btnUnselectAll.CssClass = "DataButtonEnabled"

                End If

            End If
            If DropDownFieldJ1.Items.Count > 0 Then
                DropDownFieldJ1.Items(0).Selected = True
            Else
                DropDownFieldJ1.Text = ""
            End If
            If DropDownFieldJ2.Items.Count > 0 Then
                DropDownFieldJ2.Items(0).Selected = True
            Else
                DropDownFieldJ2.Text = ""
            End If
            If DropDownFieldW1.Items.Count > 0 Then
                DropDownFieldW1.Items(0).Selected = True
            Else
                DropDownFieldW1.Text = ""
            End If
            If DropDownFieldW2.Items.Count > 0 Then
                DropDownFieldW2.Items(0).Selected = True
            Else
                DropDownFieldW2.Text = ""
            End If
            If DropDownFieldW3.Items.Count > 0 Then
                DropDownFieldW3.Items(0).Selected = True
            Else
                DropDownFieldW3.Text = ""
            End If
            If DropDownFieldW4.Items.Count > 0 Then

                DropDownFieldW4.Items(0).Selected = True
            Else
                DropDownFieldW4.Text = ""
            End If

            If DropDownFieldW5.Items.Count > 0 Then

                DropDownFieldW5.Items(0).Selected = True
            Else
                DropDownFieldW5.Text = ""
            End If
            If DropDownFieldS1.Items.Count > 0 Then
                DropDownFieldS1.Items(0).Selected = True
            Else
                DropDownFieldS1.Text = ""
            End If

        End If

        Dim dr As DataTable = GetReportInfo(repid)
        If dr IsNot Nothing AndAlso dr.Rows.Count = 1 AndAlso dr.Rows(0)("Param6type").ToString.ToUpper = "TRUE" Then
            CheckBoxDistinct.Checked = True
        Else
            CheckBoxDistinct.Checked = False
        End If
        LabelAlert.Text = Session("REPTITLE")
        LabelReportID.Text = Session("REPORTID")

        If (Session("admin") = "super" AndAlso Session("Provider") = "Cache") OrElse (Session("AdvancedUser") = True AndAlso Session("admin") = "super" AndAlso Session("CSV") <> "yes") Then
            CheckBoxSysTables.Enabled = True
            CheckBoxSysTables.Visible = True
        Else
            CheckBoxSysTables.Enabled = False
            CheckBoxSysTables.Visible = False
        End If

        DropDownColumns.CheckBoxList.Attributes.Add("onchange", "OnChangeChecklistDropDown(event);")
        btnSelectAll.OnClientClick = "selectAll('DropDownColumns'); return false;"
        btnUnselectAll.OnClientClick = "unSelectAll('DropDownColumns'); return false;"
        ButtonSelectFields.OnClientClick = "OnSelectFieldsClick(event); return false;"
        imgSearch.OnClientClick = "OnSearchButtonClick(event); return false;"
        imgSearch.Attributes.Add("onfocus", "onImageButtonFocus(event);")
        imgSearch.Attributes.Add("onblur", "onImageButtonBlur(event);")
        imgSearch.Attributes.Add("onmouseover", "hoverState(""In"");")
        imgSearch.Attributes.Add("onmouseout", "hoverState(""Out"");")
        imgSearch.Attributes.Add("onmousedown", "pressState(""Down"");")
        imgSearch.Attributes.Add("onmouseup", "pressState(""Up"");")
        'txtSearch.Attributes.Add("onchange", "OnSearchTextChanged(event);")
        txtSearch.Attributes.Add("onkeydown", " OnSearchTextKeyDown(event);")
        DropDownTables.Attributes.Add("onchange", "OnDropDownTablesChange(event);")
    End Sub
    Private Function GetRelativeDate(DateFunc As String) As String
        Dim ret As String = String.Empty
        If DateFunc <> String.Empty Then
            Dim Days As Integer = 0
            Dim sS As String = String.Empty

            If DateFunc.ToUpper.StartsWith("DATEADD(") Then
                Days = CInt(Piece(DateFunc, ",", 2))
            ElseIf DateFunc.ToUpper.StartsWith("DATE_ADD(") Then
                sS = Piece(DateFunc, ",", 2)
                sS = Piece(Piece(sS, " INTERVAL ", 2), " DAY)", 1)
                Days = CInt(sS)
            ElseIf DateFunc.ToUpper.StartsWith("DATE_SUB(") Then
                sS = Piece(DateFunc, ",", 2)
                sS = Piece(Piece(sS, " INTERVAL ", 2), " DAY)", 1)
                Days = -CInt(sS)
            End If
            ret = Format(Now, "M/d/yyyy")
            If Days <> 0 Then
                ret = Format(Now.AddDays(Days), "M/d/yyyy")
            End If
        End If
        Return ret
    End Function
    Private Function GetConditionData(dtw As DataTable) As Controls_ConditionDlg.ConditionData
        Dim cdata As New Controls_ConditionDlg.ConditionData

        If dtw IsNot Nothing AndAlso dtw.Rows.Count = 1 Then
            Dim oper As String = String.Empty
            Dim tbl1 As String = String.Empty
            Dim fld1 As String = String.Empty
            Dim tbl2 As String = String.Empty
            Dim fld2 As String = String.Empty
            Dim tbl3 As String = String.Empty
            Dim fld3 As String = String.Empty
            Dim val As String = String.Empty
            Dim typ As String = String.Empty

            oper = dtw.Rows(0)("Oper").ToString
            tbl1 = dtw.Rows(0)("Tbl1").ToString
            fld1 = dtw.Rows(0)("Tbl1Fld1").ToString
            tbl2 = dtw.Rows(0)("Tbl2").ToString
            fld2 = dtw.Rows(0)("Tbl2Fld2").ToString
            tbl3 = dtw.Rows(0)("Tbl3").ToString
            fld3 = dtw.Rows(0)("Tbl3Fld3").ToString
            val = dtw.Rows(0)("Comments").ToString
            typ = dtw.Rows(0)("Type").ToString.Trim

            Dim field1 As String = tbl1 & "." & fld1
            Dim field2 As String = tbl2 & "." & fld2
            Dim field3 As String = tbl3 & "." & fld3

            Dim fieldtype As String = GetFieldDataType(tbl1, fld1, Session("UserConnString"), Session("UserConnProvider"))
            Dim IsDate As Boolean = ((fieldtype.ToUpper <> "TIME") AndAlso (fieldtype.ToUpper.Contains("DATE") OrElse fieldtype.ToUpper.Contains("TIME")))

            Dim idx As Integer = oper.ToUpper.IndexOf("NOT ")

            If idx > -1 Then
                cdata.NotOperator = True
                oper = oper.Substring(idx + 4)
            End If
            cdata.IsDate = IsDate
            cdata.Field = field1
            cdata.ConditionOperator = oper

            If typ = "Static" Then
                cdata.FieldChosen1 = False
                cdata.TextValue1 = val
            ElseIf typ = "Field" Then
                If IsDate Then
                    cdata.DateFieldChosen1 = True
                    cdata.DateFieldValue1 = field2
                Else
                    cdata.FieldChosen1 = True
                    cdata.FieldValue1 = field2
                End If
            ElseIf typ = "DateTime" Then
                cdata.DateFieldChosen1 = False
                cdata.DateRelative1 = False
                cdata.DateValue1 = val
            ElseIf typ = "RelDate" Then
                cdata.DateFieldChosen1 = False
                cdata.DateRelative1 = True
                cdata.DateValue1 = GetRelativeDate(val)
            ElseIf typ = "BtwFields" OrElse typ = "BtwFD1FD2" Then
                If IsDate Then
                    cdata.DateFieldChosen1 = True
                    cdata.DateFieldChosen2 = True
                    cdata.DateFieldValue1 = field2
                    cdata.DateFieldValue2 = field3
                Else
                    cdata.FieldChosen1 = True
                    cdata.FieldChosen2 = True
                    cdata.FieldValue1 = field2
                    cdata.FieldValue2 = field3
                End If
            ElseIf typ = "BtwDates" OrElse typ = "BtwDT1DT2" Then
                cdata.DateFieldChosen1 = False
                cdata.DateFieldChosen2 = False
                cdata.DateRelative1 = False
                cdata.DateRelative2 = False
                cdata.DateValue1 = fld2
                cdata.DateValue2 = fld3
            ElseIf typ = "BtwDateFld" OrElse typ = "BtwDT1FD2" Then
                cdata.DateFieldChosen1 = False
                cdata.DateFieldChosen2 = True
                cdata.DateRelative1 = False
                cdata.DateRelative2 = False
                cdata.DateValue1 = fld2
                cdata.DateFieldValue2 = field3
            ElseIf typ = "BtwFldDate" OrElse typ = "BtwFD1DT2" Then
                cdata.DateFieldChosen1 = True
                cdata.DateFieldChosen2 = False
                cdata.DateRelative1 = False
                cdata.DateRelative2 = False
                cdata.DateFieldValue1 = field2
                cdata.DateValue2 = fld3
            ElseIf typ = "BtwRD1RD2" Then
                cdata.DateFieldChosen1 = False
                cdata.DateFieldChosen2 = False
                cdata.DateRelative1 = True
                cdata.DateRelative2 = True
                cdata.DateValue1 = GetRelativeDate(fld2)
                cdata.DateValue2 = GetRelativeDate(fld3)
            ElseIf typ = "BwDate1RD2" OrElse typ = "BtwDT1RD2" Then
                cdata.DateFieldChosen1 = False
                cdata.DateFieldChosen2 = False
                cdata.DateRelative1 = False
                cdata.DateRelative2 = True
                cdata.DateValue1 = fld2
                cdata.DateValue2 = GetRelativeDate(fld3)
            ElseIf typ = "BwRD1Date2" OrElse typ = "BtwRD1DT2" Then
                cdata.DateFieldChosen1 = False
                cdata.DateFieldChosen2 = False
                cdata.DateRelative1 = True
                cdata.DateRelative2 = False
                cdata.DateValue1 = GetRelativeDate(fld2)
                cdata.DateValue2 = fld3
            ElseIf typ = "BtwFldRD2" OrElse typ = "BtwFD1RD2" Then
                cdata.DateFieldChosen1 = True
                cdata.DateFieldChosen2 = False
                cdata.DateRelative1 = False
                cdata.DateRelative2 = True
                cdata.DateFieldValue1 = field2
                cdata.DateValue2 = GetRelativeDate(fld3)
            ElseIf typ = "BtwRD1Fld" OrElse typ = "BtwRD1FD2" Then
                cdata.DateFieldChosen1 = False
                cdata.DateFieldChosen2 = True
                cdata.DateRelative1 = True
                cdata.DateRelative2 = False
                cdata.DateValue1 = GetRelativeDate(fld2)
                cdata.DateFieldValue2 = field3
            ElseIf typ = "BtwValues" OrElse typ = "BtwST1ST2" Then
                cdata.FieldChosen1 = False
                cdata.FieldChosen2 = False
                cdata.TextValue1 = fld2
                cdata.TextValue2 = fld3
            ElseIf typ = "BtwValFld" OrElse typ = "BtwST1FD2" Then
                cdata.FieldChosen1 = False
                cdata.FieldChosen2 = True
                cdata.TextValue1 = fld2
                cdata.FieldValue2 = field3
            ElseIf typ = "BtwFldVal" OrElse typ = "BtwFD1ST2" Then
                cdata.FieldChosen1 = True
                cdata.FieldChosen2 = False
                cdata.FieldValue1 = field2
                cdata.TextValue2 = fld3
            End If
        End If
        Return cdata
    End Function
    Private Sub LoadConditionEntry(dtw As DataTable)
        If dtw IsNot Nothing AndAlso dtw.Rows.Count = 1 Then
            Dim oper As String = String.Empty
            Dim tbl1 As String = String.Empty
            Dim fld1 As String = String.Empty
            Dim tbl2 As String = String.Empty
            Dim fld2 As String = String.Empty
            Dim tbl3 As String = String.Empty
            Dim fld3 As String = String.Empty
            Dim val As String = String.Empty
            Dim typ As String = String.Empty

            oper = dtw.Rows(0)("Oper").ToString
            tbl1 = dtw.Rows(0)("Tbl1").ToString
            fld1 = dtw.Rows(0)("Tbl1Fld1").ToString
            tbl2 = dtw.Rows(0)("Tbl2").ToString
            fld2 = dtw.Rows(0)("Tbl2Fld2").ToString
            tbl3 = dtw.Rows(0)("Tbl3").ToString
            fld3 = dtw.Rows(0)("Tbl3Fld3").ToString
            val = dtw.Rows(0)("Comments").ToString
            typ = dtw.Rows(0)("Type").ToString.Trim

            Dim field1 As String = tbl1 & "." & fld1
            Dim field2 As String = tbl2 & "." & fld2
            Dim field3 As String = tbl3 & "." & fld3

            Dim fieldtype As String = GetFieldDataType(tbl1, fld1, Session("UserConnString"), Session("UserConnProvider"))
            Dim IsDate As Boolean = (fieldtype.ToUpper.Contains("DATE") OrElse fieldtype.ToUpper.Contains("TIME"))

            DropDownFieldW1.Text = field1
            DropDownOperator.Text = oper

            If typ = "Static" Then
                ShowTextFields(False)
                TextBoxStatic.Text = val
            ElseIf typ = "Field" Then
                If IsDate Then
                    ShowDateFields(False)
                    DropDownFieldW4.Text = field2
                    divField3.Visible = True
                    divCalendar1.Visible = False
                    btnCal1.Text = "Date"
                    btnCal1.ToolTip = "Enter date value"
                Else
                    ShowTextFields(False)
                    DropDownFieldW2.Text = field2
                    divField1.Visible = True
                    divText1.Visible = False
                    btnTxt1.Text = "Text"
                    btnTxt1.ToolTip = "Enter text value"
                End If
            ElseIf typ = "BtwFields" Then
                If IsDate Then
                    ShowDateFields(True)

                    DropDownFieldW4.Text = field2
                    divField3.Visible = True
                    divCalendar1.Visible = False
                    btnCal1.Text = "Date"
                    btnCal1.ToolTip = "Enter date value"

                    DropDownFieldW5.Text = field3
                    divField4.Visible = True
                    divcalendar2.Visible = False
                    btnCal2.Text = "Date"
                    btnCal2.ToolTip = "Enter date value"
                Else
                    ShowTextFields(True)

                    DropDownFieldW2.Text = field2
                    divField1.Visible = True
                    divText1.Visible = False
                    btnTxt1.Text = "Text"
                    btnTxt1.ToolTip = "Enter text value"

                    DropDownFieldW3.Text = field3
                    divField2.Visible = True
                    divText2.Visible = False
                    btnTxt2.Text = "Text"
                    btnTxt2.ToolTip = "Enter text value"
                End If
            ElseIf typ = "DateTime" Then
                ShowDateFields(False)
                ddDate1.SelectedDate = CDate(val)
            ElseIf typ = "BtwDates" Then
                ShowDateFields(True)
                ddDate1.SelectedDate = CDate(fld2)
                ddDate2.SelectedDate = CDate(fld3)
            ElseIf typ = "BtwDateFld" Then
                ShowDateFields(True)
                ddDate1.SelectedDate = CDate(fld2)
                divcalendar2.Visible = False
                divField4.Visible = True
                DropDownFieldW5.Text = field3
                btnCal2.Text = "Date"
                btnCal2.ToolTip = "Enter date value"
            ElseIf typ = "BtwFldDate" Then
                ShowDateFields(True)
                divCalendar1.Visible = False
                divField3.Visible = True
                DropDownFieldW4.Text = field2
                btnCal1.Text = "Date"
                btnCal1.ToolTip = "Enter date value"
                ddDate2.SelectedDate = CDate(fld3)
            ElseIf typ = "BtwValues" Then
                ShowTextFields(True)
                TextBoxStatic.Text = fld2
                TextBoxStatic2.Text = fld3
            ElseIf typ = "BtwValFld" Then
                ShowTextFields(True)
                TextBoxStatic.Text = fld2
                divText2.Visible = False
                divField2.Visible = True
                DropDownFieldW3.Text = field3
                btnTxt2.Text = "Text"
                btnTxt2.ToolTip = "Enter text value"
            ElseIf typ = "BtwFldVal" Then
                ShowTextFields(True)
                divText1.Visible = False
                divField1.Visible = True
                DropDownFieldW2.Text = field2
                btnTxt1.Text = "Text"
                btnTxt1.ToolTip = "Enter text value"
                TextBoxStatic2.Text = fld3
            End If
        End If
    End Sub
    Private Sub ClearConditionEntry()
        TextBoxStatic.Text = ""
        TextBoxStatic2.Text = ""
        DropDownFieldW1.SelectedIndex = 0
        DropDownFieldW2.SelectedIndex = 0
        DropDownFieldW3.SelectedIndex = 0
        DropDownFieldW4.SelectedIndex = 0
        DropDownFieldW5.SelectedIndex = 0
        DropDownOperator.SelectedIndex = 0
        ddDate1.SelectedDate = Nothing
        ddDate2.SelectedDate = Nothing
        ShowTextFields(False)
    End Sub

    Private Sub SQLquery_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & Session("REPORTID") & "' AND Type='RPT'"
        If Session("Crystal") <> "ok" OrElse Not HasRecords(qsql) Then
            Dim treenodecr As WebControls.TreeNode = TreeView1.FindNode("ShowReport.aspx?srd=3")
            If treenodecr IsNot Nothing AndAlso treenodecr.ChildNodes.Count = 6 Then
                treenodecr.ChildNodes(5).NavigateUrl = ""
                treenodecr.ChildNodes(5).Text = "See report data"
                treenodecr.ChildNodes(5).Value = Nothing
                treenodecr.ChildNodes.Remove(treenodecr.ChildNodes(5))
            End If

        End If
        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing AndAlso target <> String.Empty AndAlso data <> String.Empty Then
            If target = "GetColumns" Then
                'Table name can have dots !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim tabl As String = data.Substring(0, data.LastIndexOf("."))    'Piece(data, ".", 1)
                Dim indx As Integer = CInt(data.Substring(data.LastIndexOf(".") + 1))                'CInt(Piece(data, ".", 2))
                DropDownTables.ClearSelection()
                DropDownTables.Items(indx).Selected = True
                DropDownTables_SelectedIndexChanged(DropDownTables, Nothing)
            ElseIf target = "LoadTables" Then
                LoadTables(data)
            ElseIf target = "SelectFields" Then
                ButtonSelectFields_Click(ButtonSelectFields, Nothing)
            ElseIf target = "FieldDelete" Then
                Dim arData As String() = data.Split(".")
                Dim tb As String = arData(0)
                Dim col As String = arData(1)
                DeleteField(tb, col)
                DropDownTables_SelectedIndexChanged(DropDownTables, Nothing)
            End If
        End If

        If Not IsPostBack Then
            txtSearch.Value = String.Empty
            hdnSearchText.Value = String.Empty
        End If

        'Threading.Thread.Sleep(1000)
        If Session("AdvancedUser") = True Then
            LabelSQL.Visible = True
            LabelSQL1.Visible = True
            LabelSQL2.Visible = True
            LabelSQLsort.Visible = True
        Else
            LabelSQL.Visible = False
            LabelSQL1.Visible = False
            LabelSQL2.Visible = False
            LabelSQLsort.Visible = False
        End If
        Dim er As String = String.Empty
        'DropDownTables_SelectedIndexChanged(sender, e)
        Label1.Text = "Selected column " & DropDownColumns.Text & " from the table " & DropDownTables.Text
        Dim retr As String = String.Empty
        repid = Session("REPORTID")
        If Not (IsPostBack) Then
            MultiView1.ActiveViewIndex = 0
            Menu1.Items(0).Selected = True
        End If
        If Session("TabNQ") IsNot Nothing AndAlso Session("TabNQ") < Menu1.Items.Count Then
            MultiView1.ActiveViewIndex = Session("TabNQ")
            Menu1.Items(Session("TabNQ")).Selected = True
            lblView.Text = Menu1.Items(Session("TabNQ")).Text
        End If
        'delete sql field
        If Request("delsqlf") IsNot Nothing AndAlso Request("delsqlf").ToString = "yes" Then
            Dim sqlfld As String = Request("sqlfld").ToString
            Dim sqltbl As String = Request("sqltbl").ToString
            DeleteSQLField(repid, sqltbl, sqlfld)
            Session("ParamNames") = Nothing
            Session("ParamValues") = Nothing
            Session("ParamTypes") = Nothing
            Session("ParamFields") = Nothing
            Session("ParamsCount") = -1
            Session("srchfld") = Nothing
            Session("srchoper") = Nothing
            Session("srchval") = Nothing
            'update the SQL query
            Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
            If sqlquerytext.Trim = "" Then
                sqlquerytext = LabelSQL.Text
            End If
            Dim ret As String = SaveSQLquery(repid, sqlquerytext)
            'update xsd and rdl
            If ret = "Query executed fine." Then
                Session("dv3") = Nothing
                ret = ret & UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
                ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            Else
                LabelAlert1.Text = ret
                WriteToAccessLog(Session("logon"), ret, 3)
            End If
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'delete sql join
        If Request("delsqlj") IsNot Nothing AndAlso Request("delsqlj").ToString = "yes" Then
            Dim sqlindx As String = Request("indx").ToString
            DeleteSQLJoin(repid, sqlindx)
            'update the SQL query
            Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
            If sqlquerytext.Trim = "" Then
                sqlquerytext = LabelSQL.Text
            End If
            Dim ret As String = SaveSQLquery(repid, sqlquerytext)
            'update xsd and rdl
            If ret = "Query executed fine." Then
                Session("dv3") = Nothing
                ret = ret & UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
                ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            Else
                LabelAlert1.Text = ret
                WriteToAccessLog(Session("logon"), ret, 3)
            End If
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'add sql join
        If Request("addjoin") IsNot Nothing AndAlso Request("addjoin").ToString = "yes" Then
            Dim tbl1 As String = Request("tbl1").ToString
            Dim tbl2 As String = Request("tbl2").ToString
            Dim tbl1fld1 As String = Request("tbl1fld1").ToString
            Dim tbl2fld2 As String = Request("tbl2fld2").ToString
            Dim jointype As String = Request("jointype").ToString

            If GetFieldDataType(tbl1, tbl1fld1, Session("UserConnString"), Session("UserConnProvider")).Trim = GetFieldDataType(tbl2, tbl2fld2, Session("UserConnString"), Session("UserConnProvider")).Trim Then
                retr = AddJoin(repid, tbl1, tbl2, tbl1fld1, tbl2fld2, jointype, Session("UserDB"), "custom")
                Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
            End If
        End If

        'up sql join
        If Request("upjoin") IsNot Nothing AndAlso Request("upjoin").ToString = "yes" Then
            Dim sqlindx As String = Request("indx").ToString
            dt = GetReportJoins(repid) 'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
            dt = CorrectJoinOrder(dt)
            UpJoin(dt, sqlindx)
            SaveJoins(repid, dt)
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'down sql join
        If Request("downjoin") IsNot Nothing AndAlso Request("downjoin").ToString = "yes" Then
            Dim sqlindx As String = Request("indx").ToString
            dt = GetReportJoins(repid) 'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
            CorrectJoinOrder(dt)
            DownJoin(dt, sqlindx)
            SaveJoins(repid, dt)
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'reverse tables,fields sql join
        If Request("rvrsjoin") IsNot Nothing AndAlso Request("rvrsjoin").ToString = "yes" Then
            Dim sqlindx As String = Request("indx").ToString
            dt = GetReportJoins(repid) 'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
            CorrectJoinOrder(dt)
            ReverseJoin(dt, sqlindx)
            SaveJoins(repid, dt)
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'delete sql sort
        If Request("delsqls") IsNot Nothing AndAlso Request("delsqls").ToString = "yes" Then
            Dim sqlindx As String = Request("indx").ToString
            DeleteSQLSort(repid, sqlindx)
            'Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
            'update the SQL query
            Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
            If sqlquerytext.Trim = "" Then
                sqlquerytext = LabelSQL.Text
            End If
            Dim ret As String = SaveSQLquery(repid, sqlquerytext)
            'update xsd and rdl
            If ret = "Query executed fine." Then
                Session("dv3") = Nothing
                ret = ret & UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
                ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            Else
                LabelAlert1.Text = ret
                WriteToAccessLog(Session("logon"), ret, 3)
            End If
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'up sort
        If Request("upsqls") IsNot Nothing AndAlso Request("upsqls").ToString = "yes" Then
            Dim indx As String = Request("indx").ToString
            dt = GetSQLSorts(repid) 'sorted by Oper
            CorrectSortOrder(dt)
            UpSortField(dt, indx)
            SaveSorts(repid, dt)
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'down sort
        If Request("downsqls") IsNot Nothing AndAlso Request("downsqls").ToString = "yes" Then
            Dim indx As String = Request("indx").ToString
            dt = GetSQLSorts(repid) 'sorted by Oper
            CorrectSortOrder(dt)
            DownSortField(dt, indx)
            SaveSorts(repid, dt)
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'delete sql where
        If Request("delsqlw") IsNot Nothing AndAlso Request("delsqlw").ToString = "yes" Then
            Dim sqlindx As String = Request("indx").ToString
            DeleteSQLWhere(repid, sqlindx)
            'update the SQL query
            Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
            If sqlquerytext.Trim = "" Then
                sqlquerytext = LabelSQL.Text
            End If
            Dim ret As String = SaveSQLquery(repid, sqlquerytext)
            Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
        End If
        'draw selected tables
        Dim rt As String = String.Empty
        Dim lnk As String = ""
        Dim ctlLnk As LinkButton = Nothing

        Dim dtf As DataTable = GetSQLFields(repid)
        If dtf Is Nothing OrElse dtf.Rows.Count = 0 Then
            'Make SQLFields from sql statement
            Dim selfields As String = Session("SELECTEDFields")  'fields from the report sql statement
            If selfields <> "" Then
                Dim m As Integer = 0
                Dim k As Integer = 0
                Dim tabl, fild, frname As String
                Dim sctSQL As String = String.Empty
                selfields = FixSelectedFields(Session("REPORTID"), selfields, Session("UserConnString"), Session("UserConnProvider"))
                If Not selfields.ToUpper.Contains("ERROR") Then
                    Dim sqlfield As String() = selfields.Split(",")
                    For m = 0 To sqlfield.Length - 1
                        If sqlfield(m).Trim <> "" Then
                            rt = CreateNewRowInHtmlTable(SQLfields)
                            If rt.ToUpper.Contains("ERROR") Then Continue For
                            tabl = sqlfield(m).Trim.Substring(0, sqlfield(m).Trim.LastIndexOf(".")).Trim
                            fild = sqlfield(m).Trim.Substring(sqlfield(m).Trim.LastIndexOf(".") + 1).Trim
                            frname = GlobalFriendlyName(tabl, fild, Session("UnitDB"), er) 'GetFriendlyFieldName(repid, fild)
                            SQLfields.Rows(k + 1).Cells(0).InnerHtml = tabl
                            SQLfields.Rows(k + 1).Cells(1).InnerHtml = fild
                            'SQLfields.Rows(k + 1).Cells(2).InnerHtml = "<a href ='SQLquery.aspx?Report=" & repid & "&sqltbl=" & tabl & "&sqlfld=" & fild & "&delsqlf=yes'>del</a>"

                            'lnk = "SQLquery.aspx?Report=" & repid & "&sqltbl=" & tabl & "&sqlfld=" & fild & "&delsqlf=yes"

                            ctlLnk = New LinkButton
                            ctlLnk.Text = "delete"
                            ctlLnk.ID = "FieldDelete" & "^" & tabl & "." & fild
                            ctlLnk.OnClientClick = "OnFieldDeleteClick(event); return false;"
                            ctlLnk.ToolTip = "Delete Report Field"
                            'AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                            SQLfields.Rows(k + 1).Cells(2).InnerText = String.Empty
                            SQLfields.Rows(k + 1).Cells(2).Controls.Add(ctlLnk)

                            SQLfields.Rows(k + 1).Cells(3).InnerHtml = frname
                            sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='SELECT' AND Tbl1='" & tabl & "' AND Tbl1Fld1='" & fild & "')"
                            If Not HasRecords(sctSQL) Then
                                sctSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1,Friendly) VALUES('" & Session("REPORTID") & "','SELECT','" & tabl & "','" & fild & "','" & frname & "')"
                                rt = ExequteSQLquery(sctSQL)
                            End If
                            k = k + 1
                        End If
                    Next
                End If
            End If
        ElseIf Not dtf Is Nothing AndAlso dtf.Rows.Count > 0 Then
            Dim t As String = String.Empty
            Dim f As String = String.Empty

            For i = 0 To dtf.Rows.Count - 1
                rt = AddRowIntoHTMLtable(dtf.Rows(i), SQLfields)
                If rt.ToUpper.Contains("ERROR") Then Continue For
                t = dtf.Rows(i)("Tbl1").ToString
                t = t.Replace("`", "").Replace("[", "").Replace("]", "")
                f = dtf.Rows(i)("Tbl1Fld1").ToString
                f = f.Replace("`", "").Replace("[", "").Replace("]", "")
                SQLfields.Rows(i + 1).Cells(0).InnerHtml = t
                SQLfields.Rows(i + 1).Cells(1).InnerHtml = f
                'SQLfields.Rows(i + 1).Cells(2).InnerHtml = "<a href ='SQLquery.aspx?Report=" & repid & "&sqltbl=" & dtf.Rows(i)("Tbl1").ToString & "&sqlfld=" & dtf.Rows(i)("Tbl1Fld1").ToString & "&delsqlf=yes'>del</a>"

                'lnk = "SQLquery.aspx?Report=" & repid & "&sqltbl=" & t & "&sqlfld=" & f & "&delsqlf=yes"

                ctlLnk = New LinkButton
                ctlLnk.Text = "delete"
                ctlLnk.ID = "FieldDelete" & "^" & t & "." & f
                ctlLnk.ToolTip = "Delete Report Field"
                ctlLnk.OnClientClick = "OnFieldDeleteClick(event); return false;"
                'AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                SQLfields.Rows(i + 1).Cells(2).InnerText = String.Empty
                SQLfields.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)

                SQLfields.Rows(i + 1).Cells(3).InnerHtml = dtf.Rows(i)("Friendly").ToString
            Next
        End If

        Dim typ As String = ""
        Dim tbl As String = ""
        Dim tabl2 As String = ""
        Dim tabl3 As String = ""
        Dim fld As String = ""
        Dim fld2 As String = ""
        Dim fld3 As String = ""
        Dim oper As String = ""
        Dim field2 As String = ""
        Dim field3 As String = ""
        Dim val As String = ""
        Dim RecOrder As String = ""
        Dim Logical As String = ""
        Dim Group As String = ""



        'report joins
        'Dim dtj As DataTable = GetSQLJoins(repid)   'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
        Dim dtj As DataTable = GetReportJoins(repid)   'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
        If dtj Is Nothing OrElse dtj.Rows.Count = 0 Then
            Dim ern As String = String.Empty
            Dim dtjs As DataTable = GetListOfJoinsFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)
            'dtj = GetSQLJoins(repid)  'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
            dtj = GetReportJoins(repid)
        End If

        If Not dtj Is Nothing AndAlso dtj.Rows.Count > 0 Then
            For i = 0 To dtj.Rows.Count - 1
                rt = AddRowIntoHTMLtable(dtj.Rows(i), SQLJoins)
                If rt.ToUpper.Contains("ERROR") Then Continue For
                tbl = dtj.Rows(i)("Tbl1").ToString
                fld = dtj.Rows(i)("Tbl1Fld1").ToString
                typ = dtj.Rows(i)("Comments").ToString
                tabl2 = dtj.Rows(i)("Tbl2").ToString
                fld2 = dtj.Rows(i)("Tbl2Fld2").ToString
                Dim indx As String = dtj.Rows(i)("Indx").ToString

                Dim ix As Integer = 1111 + i
                'Dim idx As String = CInt(dtj.Rows(i)("indx").ToString)

                'lnk = "SQLquery.aspx?Report=" & repid & "&indx=" & dtj.Rows(i)("indx").ToString & "&delsqlj=yes"

                SQLJoins.Rows(i + 1).Cells(0).InnerHtml = tbl
                SQLJoins.Rows(i + 1).Cells(1).InnerHtml = fld
                SQLJoins.Rows(i + 1).Cells(2).InnerHtml = typ
                SQLJoins.Rows(i + 1).Cells(3).InnerHtml = tabl2
                SQLJoins.Rows(i + 1).Cells(4).InnerHtml = fld2

                If i = 0 Then
                    SQLJoins.Rows(i + 1).Cells(5).InnerText = String.Empty
                Else
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "up"
                    ctlLnk.ID = "ReportJoinUp^" & indx
                    'ctlLnk.PostBackUrl = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&upjoin=yes"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    SQLJoins.Rows(i + 1).Cells(5).InnerText = String.Empty
                    SQLJoins.Rows(i + 1).Cells(5).Controls.Add(ctlLnk)
                End If
                If i = dtj.Rows.Count - 1 Then
                    SQLJoins.Rows(i + 1).Cells(6).InnerText = String.Empty
                Else
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "down"
                    ctlLnk.ID = "ReportJoinDown" & "^" & indx
                    ctlLnk.ToolTip = "Move Report Join Down"
                    'ctlLnk.PostBackUrl = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&downjoin=yes"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    SQLJoins.Rows(i + 1).Cells(6).InnerText = String.Empty
                    SQLJoins.Rows(i + 1).Cells(6).Controls.Add(ctlLnk)
                End If
                ctlLnk = New LinkButton
                ctlLnk.Text = "reverse"
                ctlLnk.ID = "ReportJoinReverse" & "^" & indx
                ctlLnk.ToolTip = "Reverse Report Join"
                'ctlLnk.PostBackUrl = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&rvrsjoin=yes"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                SQLJoins.Rows(i + 1).Cells(7).InnerText = String.Empty
                SQLJoins.Rows(i + 1).Cells(7).Controls.Add(ctlLnk)

                ctlLnk = New LinkButton
                ctlLnk.Text = "delete"
                ctlLnk.ID = "ReportJoinDelete" & "^" & indx
                ctlLnk.ToolTip = "Delete Report Join"
                'ctlLnk.PostBackUrl = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&delsqlj=yes"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                SQLJoins.Rows(i + 1).Cells(8).InnerText = String.Empty
                SQLJoins.Rows(i + 1).Cells(8).Controls.Add(ctlLnk)

                'If i = 0 Then
                '    SQLJoins.Rows(i + 1).Cells(5).InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"
                'Else
                '    SQLJoins.Rows(i + 1).Cells(5).InnerHtml = "<a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dtj.Rows(i)("indx").ToString & "&upjoin=yes'>up</a>"
                'End If
                'If i = dtj.Rows.Count - 1 Then
                '    SQLJoins.Rows(i + 1).Cells(5).InnerHtml = SQLJoins.Rows(i + 1).Cells(5).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                'Else
                '    SQLJoins.Rows(i + 1).Cells(5).InnerHtml = SQLJoins.Rows(i + 1).Cells(5).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp; <a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dtj.Rows(i)("indx").ToString & "&downjoin=yes'>down</a>"
                'End If
                'SQLJoins.Rows(i + 1).Cells(5).InnerHtml = SQLJoins.Rows(i + 1).Cells(5).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp; <a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dtj.Rows(i)("indx").ToString & "&delsqlj=yes'>del</a>"
                'SQLJoins.Rows(i + 1).Cells(5).InnerHtml = SQLJoins.Rows(i + 1).Cells(5).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp; <a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dtj.Rows(i)("indx").ToString & "&rvrsjoin=yes'>reverse</a>"

            Next
        End If

        'all possible existing joins 
        Dim dtc As DataTable
        If DropDownTableJ1.Items.Count > 0 AndAlso DropDownTableJ2.Items.Count > 0 Then
            dtc = ListAllSQLJoins(repid, Session("dbname") & "_joins", Session("UserDB"), DropDownTableJ1.SelectedItem.Text, DropDownTableJ2.SelectedItem.Text, Session("UserConnProvider"))
        Else
            dtc = ListAllSQLJoins(repid, Session("dbname") & "_joins", Session("UserDB"), "", "", Session("UserConnProvider"))
        End If
        If Not dtc Is Nothing AndAlso dtc.Rows.Count > 0 Then
            Dim j As Integer = 0
            Dim ret As String
            Dim id As String
            For i = 0 To dtc.Rows.Count - 1
                If dtc.Rows(i)("DEL") = 0 Then
                    'j = j + 1
                    'ret = AddRowIntoHTMLtable(dtc.Rows(i), ListOfJoins)
                    'If ret.ToUpper.Contains("ERROR") Then Continue For
                    'If dtc.Rows(i)("Param2").ToString.Trim = "" Then
                    '    dtc.Rows(i)("Param2") = "custom"
                    'End If
                    'ListOfJoins.Rows(j).Cells(0).InnerHtml = dtc.Rows(i)("Param2").ToString & ": " & dtc.Rows(i)("Tbl1").ToString & " JOIN " & dtc.Rows(i)("Tbl2").ToString & " ON (" & dtc.Rows(i)("Tbl1").ToString & "." & dtc.Rows(i)("Tbl1Fld1").ToString & " = " & dtc.Rows(i)("Tbl2").ToString & "." & dtc.Rows(i)("Tbl2Fld2").ToString & ")  "
                    lnk = "SQLquery.aspx?Report=" & repid & "&tbl1=" & dtc.Rows(i)("Tbl1").ToString & "&tbl2=" & dtc.Rows(i)("Tbl2").ToString & "&tbl1fld1=" & dtc.Rows(i)("Tbl1Fld1").ToString & "&tbl2fld2=" & dtc.Rows(i)("Tbl2Fld2").ToString & "&jointype=" & DropDownJoinType.SelectedValue.ToString.Replace(" ", "") & "&addjoin=yes"
                    id = "AddJoin" & "^" & lnk
                    If Page.FindControl(id) Is Nothing Then
                        j = j + 1
                        ret = AddRowIntoHTMLtable(dtc.Rows(i), ListOfJoins)
                        If ret.ToUpper.Contains("ERROR") Then Continue For
                        If dtc.Rows(i)("Param2").ToString.Trim = "" Then
                            dtc.Rows(i)("Param2") = "custom"
                        End If
                        ListOfJoins.Rows(j).Cells(0).InnerHtml = dtc.Rows(i)("Param2").ToString & ": " & dtc.Rows(i)("Tbl1").ToString & " JOIN " & dtc.Rows(i)("Tbl2").ToString & " ON (" & dtc.Rows(i)("Tbl1").ToString & "." & dtc.Rows(i)("Tbl1Fld1").ToString & " = " & dtc.Rows(i)("Tbl2").ToString & "." & dtc.Rows(i)("Tbl2Fld2").ToString & ")  "
                        ctlLnk = New LinkButton
                        ctlLnk.Text = "Add Join"
                        'ctlLnk.ID = "AddJoin" & "^" & lnk & "^" & j.ToString
                        ctlLnk.ID = id
                        ctlLnk.ToolTip = lnk
                        AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                        ListOfJoins.Rows(j).Cells(1).InnerText = String.Empty
                        ListOfJoins.Rows(j).Cells(1).Controls.Add(ctlLnk)
                        'ListOfJoins.Rows(j).Cells(1).InnerHtml = "&nbsp;&nbsp;&nbsp;<a href ='SQLquery.aspx?Report=" & repid & "&tbl1=" & dtc.Rows(i)("Tbl1").ToString & "&tbl2=" & dtc.Rows(i)("Tbl2").ToString & "&tbl1fld1=" & dtc.Rows(i)("Tbl1Fld1").ToString & "&tbl2fld2=" & dtc.Rows(i)("Tbl2Fld2").ToString & "&jointype=" & DropDownJoinType.SelectedValue.ToString.Replace(" ", "") & "&addjoin=yes'>add join</a>"
                        'If Not IsDBNull(dtc.Rows(i)("Comments") AndAlso (dtc.Rows(i)("Comments").ToString = "manually" OrElse dtc.Rows(i)("Comments").ToString = "custom")) Then ListOfJoins.Rows(j).Cells(0).BgColor = "#C0C0C1"
                        If dtc.Rows(i)("Param2").ToString = "manually" OrElse dtc.Rows(i)("Param2").ToString = "custom" Then ListOfJoins.Rows(j).Cells(0).BgColor = "#C0C0C1"
                        ListOfJoins.Rows(j).Cells(1).BgColor = "#C0C0C0"
                    End If
                End If
                'ctlLnk = New LinkButton
                'ctlLnk.Text = "Add Join"
                ''ctlLnk.ID = "AddJoin" & "^" & lnk & "^" & j.ToString
                'ctlLnk.ID = id
                'ctlLnk.ToolTip = lnk
                'AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                'ListOfJoins.Rows(j).Cells(1).InnerText = String.Empty
                'ListOfJoins.Rows(j).Cells(1).Controls.Add(ctlLnk)
                ''ListOfJoins.Rows(j).Cells(1).InnerHtml = "&nbsp;&nbsp;&nbsp;<a href ='SQLquery.aspx?Report=" & repid & "&tbl1=" & dtc.Rows(i)("Tbl1").ToString & "&tbl2=" & dtc.Rows(i)("Tbl2").ToString & "&tbl1fld1=" & dtc.Rows(i)("Tbl1Fld1").ToString & "&tbl2fld2=" & dtc.Rows(i)("Tbl2Fld2").ToString & "&jointype=" & DropDownJoinType.SelectedValue.ToString.Replace(" ", "") & "&addjoin=yes'>add join</a>"
                ''If Not IsDBNull(dtc.Rows(i)("Comments") AndAlso (dtc.Rows(i)("Comments").ToString = "manually" OrElse dtc.Rows(i)("Comments").ToString = "custom")) Then ListOfJoins.Rows(j).Cells(0).BgColor = "#C0C0C1"
                'If dtc.Rows(i)("Param2").ToString = "manually" OrElse dtc.Rows(i)("Param2").ToString = "custom" Then ListOfJoins.Rows(j).Cells(0).BgColor = "#C0C0C1"
                'ListOfJoins.Rows(j).Cells(1).BgColor = "#C0C0C0"
                'End If
            Next
        End If

        'sorting
        Dim dts As DataTable = GetSQLSorts(repid)
        If dts Is Nothing OrElse dts.Rows.Count = 0 Then
            Dim ern As String = String.Empty
            dts = GetListOfOrderByFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)
            dts = GetSQLSorts(repid)
        End If
        If Not dts Is Nothing AndAlso dts.Rows.Count > 0 Then
            For i = 0 To dts.Rows.Count - 1
                Dim indx As String = dts.Rows(i)("Indx").ToString

                rt = AddRowIntoHTMLtable(dts.Rows(i), SQLSort)
                If rt.ToUpper.Contains("ERROR") Then Continue For
                SQLSort.Rows(i + 1).Cells(0).InnerHtml = dts.Rows(i)("Tbl1").ToString
                SQLSort.Rows(i + 1).Cells(1).InnerHtml = dts.Rows(i)("Tbl1Fld1").ToString
                If i = 0 Then
                    SQLSort.Rows(i + 1).Cells(2).InnerText = String.Empty
                Else
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "up"
                    ctlLnk.ID = "SortUp" & "^" & indx
                    ctlLnk.ToolTip = "Move Sort Field Up"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    SQLSort.Rows(i + 1).Cells(2).InnerText = String.Empty
                    SQLSort.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)
                End If

                If i = dts.Rows.Count - 1 Then
                    SQLSort.Rows(i + 1).Cells(3).InnerText = String.Empty
                Else
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "down"
                    ctlLnk.ID = "SortDown" & "^" & indx
                    ctlLnk.ToolTip = "Move Sort Field Down"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    SQLSort.Rows(i + 1).Cells(3).InnerText = String.Empty
                    SQLSort.Rows(i + 1).Cells(3).Controls.Add(ctlLnk)
                End If

                ctlLnk = New LinkButton
                ctlLnk.Text = "delete"
                ctlLnk.ID = "SortDelete" & "^" & indx
                ctlLnk.ToolTip = "Delete Sort Field"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                SQLSort.Rows(i + 1).Cells(4).InnerText = String.Empty
                SQLSort.Rows(i + 1).Cells(4).Controls.Add(ctlLnk)

                SQLSort.Rows(i + 1).Cells(5).InnerHtml = GlobalFriendlyName(dts.Rows(i)("Tbl1").ToString, dts.Rows(i)("Tbl1Fld1").ToString, Session("UnitDB"), er)

                'If i = 0 Then
                '    SQLSort.Rows(i + 1).Cells(2).InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"
                'Else
                '    SQLSort.Rows(i + 1).Cells(2).InnerHtml = "<a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dts.Rows(i)("indx").ToString & "&upsqls=yes'>up</a>"
                'End If
                'If i = dts.Rows.Count - 1 Then
                '    SQLSort.Rows(i + 1).Cells(2).InnerHtml = SQLSort.Rows(i + 1).Cells(2).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                'Else
                '    SQLSort.Rows(i + 1).Cells(2).InnerHtml = SQLSort.Rows(i + 1).Cells(2).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp; <a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dts.Rows(i)("indx").ToString & "&downsqls=yes'>down</a>"
                'End If
                'SQLSort.Rows(i + 1).Cells(2).InnerHtml = SQLSort.Rows(i + 1).Cells(2).InnerHtml & "&nbsp;&nbsp;&nbsp;&nbsp; <a href ='SQLquery.aspx?Report=" & repid & "&indx=" & dts.Rows(i)("indx").ToString & "&delsqls=yes'>del</a>"
            Next
        End If

        'conditions
        Dim dtw As DataTable = GetSQLConditions(repid)
        If dtw Is Nothing OrElse dtw.Rows.Count = 0 Then
            Dim ern As String = String.Empty
            Dim dt As DataTable = GetListOfConditionsFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)
            dtw = GetSQLConditions(repid)
        End If

        Session("ConditionCount") = 0
        Session("ConditionGroupCount") = GetConditionGroupCount(repid)
        If Not dtw Is Nothing AndAlso dtw.Rows.Count > 0 Then
            Session("ConditionCount") = dtw.Rows.Count

            For i = 0 To dtw.Rows.Count - 1
                tbl = dtw.Rows(i)("Tbl1").ToString
                fld = dtw.Rows(i)("Tbl1Fld1").ToString
                oper = dtw.Rows(i)("Oper").ToString
                typ = dtw.Rows(i)("Type").ToString
                tabl2 = dtw.Rows(i)("Tbl2").ToString
                tabl3 = dtw.Rows(i)("Tbl3").ToString
                fld2 = dtw.Rows(i)("Tbl2Fld2").ToString
                fld3 = dtw.Rows(i)("Tbl3Fld3").ToString
                val = dtw.Rows(i)("Comments").ToString.Trim
                'RecOrder = dtw.Rows(i)("RecOrder").ToString
                RecOrder = (i + 1).ToString
                Logical = dtw.Rows(i)("Logical").ToString
                If Logical = String.Empty Then Logical = "And"
                Group = dtw.Rows(i)("Group").ToString

                field2 = tabl2 & "." & fld2
                field3 = tabl3 & "." & fld3

                AddRowIntoHTMLtable(dtw.Rows(i), SQLConditions)

                SQLConditions.Rows(i + 1).Cells(0).InnerHtml = tbl 'table
                SQLConditions.Rows(i + 1).Cells(1).InnerHtml = fld  'field
                SQLConditions.Rows(i + 1).Cells(2).InnerHtml = oper 'operator'
                If oper.ToUpper.Contains("BETWEEN") Then
                    If typ = "BtwDates" OrElse
                       typ = "BtwDT1DT2" OrElse
                       typ = "BtwValues" OrElse
                       typ = "BtwST1ST2" OrElse
                       typ = "BwRD1Date2" OrElse
                       typ = "BtwRD1DT2" OrElse
                       typ = "BwDate1RD2" OrElse
                       typ = "BtwDT1RD2" OrElse
                       typ = "BtwRD1RD2" Then
                        SQLConditions.Rows(i + 1).Cells(3).InnerHtml = fld2
                        SQLConditions.Rows(i + 1).Cells(4).InnerHtml = fld3
                    ElseIf typ = "BtwFields" OrElse typ = "BtwFD1FD2" Then
                        SQLConditions.Rows(i + 1).Cells(3).InnerHtml = field2
                        SQLConditions.Rows(i + 1).Cells(4).InnerHtml = field3
                    ElseIf typ = "BtwFldDate" OrElse typ = "BtwFD1DT2" OrElse typ = "BtwFldVal" OrElse typ = "BtwFD1ST2" OrElse typ = "BtwFldRD2" OrElse typ = "BtwFD1RD2" Then
                        SQLConditions.Rows(i + 1).Cells(3).InnerHtml = field2
                        SQLConditions.Rows(i + 1).Cells(4).InnerHtml = fld3
                    ElseIf typ = "BtwDateFld" OrElse typ = "BtwDT1FD2" OrElse typ = "BtwValFld" OrElse typ = "BtwST1FD2" OrElse typ = "BtwRD1Fld" OrElse typ = "BtwRD1FD2" Then
                        SQLConditions.Rows(i + 1).Cells(3).InnerHtml = fld2
                        SQLConditions.Rows(i + 1).Cells(4).InnerHtml = field3
                    End If
                Else
                    SQLConditions.Rows(i + 1).Cells(3).InnerHtml = val
                    SQLConditions.Rows(i + 1).Cells(4).InnerHtml = " "
                End If

                SQLConditions.Rows(i + 1).Cells(5).InnerHtml = Logical
                SQLConditions.Rows(i + 1).Cells(6).InnerHtml = Group
                SQLConditions.Rows(i + 1).Cells(7).InnerHtml = RecOrder
                'Edit link
                Dim ctl As New LinkButton
                ctl.Text = "edit"
                ctl.ID = dtw.Rows(i)("indx").ToString
                ctl.ToolTip = "Edit condition"
                AddHandler ctl.Click, AddressOf btnEdit_Click
                SQLConditions.Rows(i + 1).Cells(8).InnerText = ""
                SQLConditions.Rows(i + 1).Cells(8).Controls.Add(ctl)

                ctl = New LinkButton
                ctl.Text = "delete"
                ctl.ID = repid & "^" & dtw.Rows(i)("indx").ToString
                ctl.ToolTip = "Delete condition"
                AddHandler ctl.Click, AddressOf btnDelete_Click
                SQLConditions.Rows(i + 1).Cells(9).InnerText = ""
                SQLConditions.Rows(i + 1).Cells(9).Controls.Add(ctl)
            Next
            Session("ConditionTreeView") = String.Empty
            If Session("ConditionCount") < 2 Then
                btnCustomizeLogic.Enabled = False
                btnCustomizeLogic.ForeColor = Drawing.Color.Gray
            Else
                Dim tv As TreeView = GetConditionTree(repid, dtw, Session("UserConnString"), Session("UserConnProvider"))
                Session("ConditionTreeView") = JsonConvert.SerializeObject(tv)
            End If
        Else
            btnCustomizeLogic.Enabled = False
            btnCustomizeLogic.ForeColor = Drawing.Color.Gray
        End If
        LabelSQL.Text = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
        If LabelSQL.Text = "" Then
            Dim dr As DataTable = GetReportInfo(repid)
            If dr IsNot Nothing AndAlso dr.Rows.Count = 1 Then
                LabelSQL.Text = dr.Rows(0)("SQLquerytext").ToString
            End If
        End If
        LabelSQL1.Text = LabelSQL.Text
        LabelSQL2.Text = LabelSQL.Text
        LabelSQLsort.Text = LabelSQL.Text
        Select Case Session("TabNQ")
            Case "0"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Report_Data_Definition"
            Case "1"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Join_Tables"
            Case "2"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Filters"
            Case "3"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Sorting"
        End Select
    End Sub
    Protected Sub DropDownTables_SelectedIndexChanged(sender As Object, e As EventArgs)  'Handles DropDownTables.SelectedIndexChanged
        Dim i As Integer
        Dim cln As String = String.Empty
        DropDownColumns.Visible = True
        lblColumnsAlert.Text = " "
        lblColumnsAlert.Visible = False
        lblTableAlert.Text = " "
        DropDownColumns.Items.Clear()
        Dim ddt As DataTable = GetListOfTableColumns(DropDownTables.SelectedItem.Value, Session("UserConnString"), Session("UserConnProvider")).Table
        Dim selfields As String = String.Empty
        selfields = Session("SELECTEDFields").ToString.Trim 'GetListOfFieldsFromSQLquery(repid)  'get list of fields from sql query in commas
        If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
            For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                DropDownColumns.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
                cln = "," & DropDownTables.SelectedItem.Value & "." & ddt.Rows(i)("COLUMN_NAME").ToString & ","
                If DropDownColumns.Items(i).Selected OrElse (selfields.Length > 2 AndAlso selfields.ToUpper.IndexOf(cln.ToUpper) >= 0) OrElse selfields = ",*," OrElse selfields.Contains("," & DropDownTables.SelectedItem.Value & ".*,") Then
                    DropDownColumns.Items(i).Selected = True
                Else
                    DropDownColumns.Items(i).Selected = False
                End If
            Next
            If DropDownColumns.AllSelected Then
                DropDownColumns.Text = "ALL"
                btnSelectAll.Enabled = False
                btnSelectAll.CssClass = "DataButtonDisabled"
                btnUnselectAll.Enabled = True
                btnUnselectAll.CssClass = "DataButtonEnabled"

            ElseIf DropDownColumns.NoneSelected Then
                DropDownColumns.Text = "Please select..."
                btnSelectAll.Enabled = True
                btnSelectAll.CssClass = "DataButtonEnabled"
                btnUnselectAll.Enabled = False
                btnUnselectAll.CssClass = "DataButtonDisabled"
            Else
                Dim items As ListItemCollection = DropDownColumns.SelectedItems
                Dim sSelected As String = String.Empty

                For j = 0 To items.Count - 1
                    sSelected = IIf(sSelected = "", items.Item(j).Text, sSelected & "," & items.Item(j).Text)
                Next
                DropDownColumns.Text = sSelected
                DropDownColumns.SelectedItemsString = sSelected
                btnSelectAll.Enabled = True
                btnSelectAll.CssClass = "DataButtonEnabled"
                btnUnselectAll.Enabled = True
                btnUnselectAll.CssClass = "DataButtonEnabled"

            End If
        Else
            lblTableAlert.Text = "No fields are defined for table: " & """" & DropDownTables.SelectedItem.Text & """" & " !"
            DropDownColumns.Visible = False
            lblColumnsAlert.Visible = True
            lblColumnsAlert.Text = "No Fields Defined"
            btnSelectAll.Enabled = False
            btnSelectAll.CssClass = "DataButtonDisabled"
            btnUnselectAll.Enabled = False
            btnUnselectAll.CssClass = "DataButtonDisabled"
        End If
    End Sub
    Protected Sub DropDownTableJ1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTableJ1.SelectedIndexChanged
        Dim i As Integer
        DropDownFieldJ1.Items.Clear()
        Dim ddcol As DataTable = GetListOfTableColumns(DropDownTableJ1.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
        If Not ddcol Is Nothing AndAlso ddcol.Rows.Count > 0 Then
            For i = 0 To ddcol.Rows.Count - 1   'draw drop-down start
                DropDownFieldJ1.Items.Add(ddcol.Rows(i)("COLUMN_NAME"))
            Next
            DropDownFieldJ1.Items(0).Selected = True
            DropDownFieldJ1.Text = DropDownFieldJ1.Items(0).Text
        End If
    End Sub
    Protected Sub DropDownTableJ2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTableJ2.SelectedIndexChanged
        Dim i As Integer
        DropDownFieldJ2.Items.Clear()
        Dim ddcol As DataTable = GetListOfTableColumns(DropDownTableJ2.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
        If Not ddcol Is Nothing AndAlso ddcol.Rows.Count > 0 Then
            For i = 0 To ddcol.Rows.Count - 1   'draw drop-down start
                DropDownFieldJ2.Items.Add(ddcol.Rows(i)("COLUMN_NAME"))
            Next
            DropDownFieldJ2.Items(0).Selected = True
            DropDownFieldJ2.Text = DropDownFieldJ2.Items(0).Text
        End If
    End Sub
    Protected Sub DropDownTableW1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTableW1.SelectedIndexChanged
        Dim i As Integer
        DropDownFieldW1.Items.Clear()
        Dim ddt As DataTable = GetListOfTableColumns(DropDownTableW1.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownFieldW1.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
        Next
        DropDownFieldW1.Items(0).Selected = True
        DropDownFieldW1.Text = DropDownFieldW1.Items(0).Text
        'Label1.Text = "Selected column " & DropDownColumns.SelectedItem.Text & " from the table " & DropDownTables.SelectedItem.Text
    End Sub
    'Protected Sub DropDownTableW2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTableW2.SelectedIndexChanged
    '    Dim i As Integer
    '    DropDownFieldW2.Items.Clear()
    '    Dim ddt As DataTable = GetListOfTableColumns(DropDownTableW2.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
    '    For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
    '        DropDownFieldW2.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
    '    Next
    '    DropDownFieldW2.Items(0).Selected = True
    '    DropDownFieldW2.Text = DropDownFieldW2.Items(0).Text
    '    'Label1.Text = "Selected column " & DropDownColumns.SelectedItem.Text & " from the table " & DropDownTables.SelectedItem.Text
    'End Sub

    Protected Sub Menu1_MenuItemClick(ByVal sender As Object, ByVal e As MenuEventArgs) Handles Menu1.MenuItemClick
        MultiView1.ActiveViewIndex = Int32.Parse(e.Item.Value)
        Dim i As Integer
        'Make the selected menu item reflect the correct imageurl
        For i = 0 To Menu1.Items.Count - 1
            If i = e.Item.Value Then
                Menu1.Items(i).Selected = True
                Session("TabNQ") = MultiView1.ActiveViewIndex
                lblView.Text = Menu1.Items(Session("TabNQ")).Text
                Select Case Session("TabNQ")
                    Case "0"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Report_Data_Definition"
                    Case "1"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Join_Tables"
                    Case "2"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Filters"
                    Case "3"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Sorting"
                End Select
            Else
                Menu1.Items(i).Selected = False
            End If
        Next
        LabelAlert1.Text = ""
    End Sub
    Function FieldsSelected() As Boolean
        For i As Integer = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(i).Selected Then
                Return True
            End If
        Next
        Return False
    End Function
    Protected Sub ButtonSelectFields_Click(sender As Object, e As EventArgs) Handles ButtonSelectFields.Click
        Dim sctSQL As String = String.Empty
        Dim fldName As String = String.Empty
        Dim frname As String = String.Empty
        Dim i As Integer = 0
        Dim hasrec As Boolean
        Dim ret As String = String.Empty
        'insert/delete fields selected/unselected in dropdowncolumns
        Dim insSQL As String = String.Empty
        Dim tbl As String = DropDownTables.Text
        Dim fld As String

        Dim bFieldsSelected As Boolean = FieldsSelected()

        If Not bFieldsSelected Then
            Dim dt As DataTable = GetSQLTableFields(Session("REPORTID"), tbl)
            Dim dr As DataRow
            If dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    dr = dt.Rows(i)
                    DeleteSQLField(repid, tbl, dr("tbl1fld1").ToString)
                Next

            Else
                MessageBox.Show("No " & tbl & " fields are selected.", "No Fields Selected", "NoFieldsSelected", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                Return
            End If

        End If

        For i = 0 To DropDownColumns.Items.Count - 1
            fld = DropDownColumns.Items(i).Text
            'sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='SELECT' AND Tbl1='" & tbl & "' AND Tbl1Fld1='" & fld & "')"
            hasrec = SQLFieldExists(Session("REPORTID"), tbl & "." & fld) 'HasRecords(sctSQL)
            If DropDownColumns.Items(i).Selected Then
                'sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='SELECT' AND Tbl1='" & DropDownTables.Text & "' AND Tbl1Fld1='" & DropDownColumns.Items(i).Text & "')"
                If Not hasrec Then  'insert record into OURReportSQLquery
                    frname = GlobalFriendlyName(DropDownTables.Text, DropDownColumns.Items(i).Text, Session("UnitDB")) 'GetFriendlyFieldName(repid, DropDownColumns.Items(i).Text)
                    insSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Friendly) VALUES('" & Session("REPORTID") & "','SELECT','" & tbl & "','" & fld & "','" & frname & "')"
                    ret = ExequteSQLquery(insSQL)
                    'sctSQL = "SELECT * FROM OURReportItems WHERE "
                End If
                'If Not ReportItemExists(Session("REPORTID"), tbl, fld) Then

                'End If
            Else
                If hasrec Then
                    DeleteSQLField(repid, DropDownTables.Text, DropDownColumns.Items(i).Text)
                End If
            End If
        Next
        'update the SQL query
        Dim sqlquerytext As String = String.Empty
        'If bFieldsSelected Then
            sqlquerytext = MakeSQLQueryFromDB(Session("REPORTID"), Session("UserConnString"), Session("UserConnProvider"))
        'End If
        LabelSQL.Text = "SELECT * FROM"
        ret = SaveSQLquery(Session("REPORTID"), sqlquerytext)
        If ret <> "Query executed fine." Then
            LabelAlert1.Text = ret
            WriteToAccessLog(Session("logon"), ret, 3)
        End If
        'Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Sub ButtonAddJoin_Click(sender As Object, e As EventArgs) Handles ButtonAddJoin.Click
        'insert custom join
        Dim ret As String = String.Empty
        Dim jointype As String = DropDownJoinType.Text
        If GetFieldDataType(DropDownTableJ1.Text, DropDownFieldJ1.Text, Session("UserConnString"), Session("UserConnProvider")).Trim = GetFieldDataType(DropDownTableJ2.Text, DropDownFieldJ2.Text, Session("UserConnString"), Session("UserConnProvider")).Trim Then
            ret = AddJoin(Session("REPORTID"), DropDownTableJ1.Text, DropDownTableJ2.Text, DropDownFieldJ1.Text, DropDownFieldJ2.Text, jointype, Session("UserDB"), "custom")
        End If

        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Sub ButtonAddCondition_Click(sender As Object, e As EventArgs) Handles ButtonAddCondition.Click
        Dim value As String = DropDownFieldW1.Text.Trim
        Dim value2 As String = String.Empty
        Dim ValueType As String = String.Empty
        Dim insSQL As String = String.Empty
        Dim valTblFld1 As String = String.Empty
        Dim valTbl1 As String = String.Empty
        Dim valFld1 As String = String.Empty
        Dim valTblFld2 As String = String.Empty
        Dim valTbl2 As String = String.Empty
        Dim valFld2 As String = String.Empty
        Dim valTyp1 As String = String.Empty
        Dim valTyp2 As String = String.Empty
        Dim bEditing As Boolean = False
        Dim indx As String = ""


        Dim sFields As String = ""
        Dim sValues As String = ""


        If value = "" Then
            DropDownOperator.SelectedIndex = 0
            Return
        End If
        If Session("Indx") IsNot Nothing AndAlso Session("Indx") <> "" Then
            bEditing = True
            indx = Session("indx").ToString
        End If
        mWhereTable = Piece(value, ".", 1)
        mWhereField = Piece(value, ".", 2)
        mWhereFieldType = GetFieldDataType(mWhereTable, mWhereField, Session("UserConnString"), Session("UserConnProvider"))
        mWhereOperator = DropDownOperator.Text

        If mWhereOperator.ToUpper <> "BETWEEN" Then
            If mWhereFieldType.ToUpper.Contains("DATE") OrElse
               mWhereFieldType.ToUpper.Contains("TIME") Then
                If btnCal1.Text = "Fields" Then
                    ValueType = "DateTime"
                    value = ddDate1.Text
                    If value = String.Empty Then
                        ddDate1.Focus()
                        Return
                    End If
                Else
                    ValueType = "Field"
                    valTblFld1 = DropDownFieldW4.Text.Trim
                    If valTblFld1 = String.Empty Then
                        DropDownFieldW4.Focus()
                        Return
                    End If
                    valTbl1 = Piece(valTblFld1, ".", 1)
                    valFld1 = Piece(valTblFld1, ".", 2)
                    value = valTblFld1
                End If
            Else
                If btnTxt1.Text = "Fields" Then
                    ValueType = "Static"
                    value = TextBoxStatic.Text
                    If value = String.Empty Then
                        TextBoxStatic.Focus()
                        Return
                    End If
                Else
                    ValueType = "Field"
                    valTblFld1 = DropDownFieldW2.Text.Trim
                    If valTblFld1 = String.Empty Then
                        DropDownFieldW2.Focus()
                        Return
                    End If
                    valTbl1 = Piece(valTblFld1, ".", 1)
                    valFld1 = Piece(valTblFld1, ".", 2)
                    value = valTblFld1
                End If
            End If
            If ValueType <> "Field" Then
                If bEditing Then
                    insSQL = "UPDATE OURReportSQLquery "
                    insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                    insSQL &= "Doing = 'WHERE',"
                    insSQL &= "Type = '" & ValueType & "',"
                    insSQL &= "Tbl1 = '" & mWhereTable & "',"
                    insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                    insSQL &= "Tbl2 = NULL,"
                    insSQL &= "Tbl2Fld2 = NULL,"
                    insSQL &= "Tbl3 = NULL,"
                    insSQL &= "Tbl3Fld3 = NULL,"
                    insSQL &= "Oper = '" & mWhereOperator & "',"
                    insSQL &= "Comments = '" & value & "' "
                    insSQL &= "Where Indx = " & indx & " "
                Else
                    sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Comments"
                    sValues = "'" & Session("REPORTID") & "',"
                    sValues &= "'WHERE',"
                    sValues &= "'" & ValueType & "',"
                    sValues &= "'" & mWhereTable & "',"
                    sValues &= "'" & mWhereField & "',"
                    sValues &= "'" & mWhereOperator & "',"
                    sValues &= "'" & value & "'"

                    insSQL = "INSERT INTO OURReportSQLquery (" & sFields & ") VALUES (" & sValues & ")"
                    'insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                    'insSQL &= "Doing = 'WHERE',"
                    'insSQL &= "Type = '" & ValueType & "',"
                    'insSQL &= "Tbl1 = '" & mWhereTable & "',"
                    'insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                    'insSQL &= "Oper = '" & mWhereOperator & "',"
                    'insSQL &= "Comments = '" & value & "'"
                End If

            Else
                If bEditing Then
                    insSQL = "UPDATE OURReportSQLquery "
                    insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                    insSQL &= "Doing = 'WHERE',"
                    insSQL &= "Type = '" & ValueType & "',"
                    insSQL &= "Tbl1 = '" & mWhereTable & "',"
                    insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                    insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    insSQL &= "Tbl3 = NULL,"
                    insSQL &= "Tbl3Fld3 = NULL,"
                    insSQL &= "Oper = '" & mWhereOperator & "',"
                    insSQL &= "Comments = '" & value & "' "
                    insSQL &= "Where Indx = " & indx & " "
                Else
                    sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Comments"
                    sValues = "'" & Session("REPORTID") & "',"
                    sValues &= "'WHERE',"
                    sValues &= "'" & ValueType & "',"
                    sValues &= "'" & mWhereTable & "',"
                    sValues &= "'" & mWhereField & "',"
                    sValues &= "'" & mWhereOperator & "',"
                    sValues &= "'" & valTbl1 & "',"
                    sValues &= "'" & valFld1 & "',"
                    sValues &= "'" & value & "'"

                    insSQL = "INSERT INTO OURReportSQLquery (" & sFields & ") VALUES (" & sValues & ")"
                    'insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                    'insSQL &= "Doing = 'WHERE',"
                    'insSQL &= "Type = '" & ValueType & "',"
                    'insSQL &= "Tbl1 = '" & mWhereTable & "',"
                    'insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                    'insSQL &= "Oper = '" & mWhereOperator & "',"
                    'insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    'insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    'insSQL &= "Comments = '" & value & "'"
                End If

            End If
        Else
            If mWhereFieldType.ToUpper.Contains("DATE") OrElse
               mWhereFieldType.ToUpper.Contains("TIME") Then
                If btnCal1.Text = "Fields" Then
                    valTyp1 = "DateTime"
                    value = ddDate1.Text
                    If value = String.Empty Then
                        ddDate1.Focus()
                        Return
                    End If
                Else
                    valTyp1 = "Field"
                    valTblFld1 = DropDownFieldW4.Text.Trim
                    If valTblFld1 = String.Empty Then
                        DropDownFieldW4.Focus()
                        Return
                    End If
                    valTbl1 = Piece(valTblFld1, ".", 1)
                    valFld1 = Piece(valTblFld1, ".", 2)
                    value = valTblFld1
                End If
                If btnCal2.Text = "Fields" Then
                    valTyp2 = "DateTime"
                    value2 = ddDate2.Text
                    If value2 = String.Empty Then
                        ddDate2.Focus()
                        Return
                    End If
                Else
                    valTyp2 = "Field"
                    valTblFld2 = DropDownFieldW5.Text.Trim
                    If valTblFld2 = String.Empty Then
                        DropDownFieldW5.Focus()
                        Return
                    End If
                    valTbl2 = Piece(valTblFld2, ".", 1)
                    valFld2 = Piece(valTblFld2, ".", 2)
                    value2 = valTblFld2
                End If
                If valTyp1 = "DateTime" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BtwDates"
                ElseIf valTyp1 = "DateTime" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwDateFld"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BtwFldDate"
                Else
                    ValueType = "BtwFields"
                End If
                If bEditing Then
                    insSQL = "UPDATE OURReportSQLquery "
                    insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                    insSQL &= "Doing = 'WHERE',"
                    insSQL &= "Type = '" & ValueType & "',"
                    insSQL &= "Tbl1 = '" & mWhereTable & "',"
                    insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                    insSQL &= "Oper = '" & mWhereOperator & "',"
                    If ValueType = "BtwDates" Then
                        insSQL &= "Tbl2 = 'Date1',"
                        insSQL &= "Tbl2Fld2 = '" & value & "',"
                        insSQL &= "Tbl3 = 'Date2',"
                        insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    ElseIf ValueType = "BtwDateFld" Then
                        insSQL &= "Tbl2 = 'Date1',"
                        insSQL &= "Tbl2Fld2 = '" & value & "',"
                        insSQL &= "Tbl3 = '" & valTbl2 & "',"
                        insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    ElseIf ValueType = "BtwFldDate" Then
                        insSQL &= "Tbl2 = '" & valTbl1 & "',"
                        insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                        insSQL &= "Tbl3 = 'Date2',"
                        insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    ElseIf ValueType = "BtwFields" Then
                        insSQL &= "Tbl2 = '" & valTbl1 & "',"
                        insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                        insSQL &= "Tbl3 = '" & valTbl2 & "',"
                        insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    End If
                Else
                    sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments"
                    sValues = "'" & Session("REPORTID") & "',"
                    sValues &= "'WHERE',"
                    sValues &= "'" & ValueType & "',"
                    sValues &= "'" & mWhereTable & "',"
                    sValues &= "'" & mWhereField & "',"
                    sValues &= "'" & mWhereOperator & "',"
                    If ValueType = "BtwDates" Then
                        sValues &= "'Date1',"
                        sValues &= "'" & value & "',"
                        sValues &= "'Date2',"
                        sValues &= "'" & value2 & "',"
                        sValues &= "NULL"
                    ElseIf ValueType = "BtwDateFld" Then
                        sValues &= "'Date1',"
                        sValues &= "'" & value & "',"
                        sValues &= "'" & valTbl2 & "',"
                        sValues &= "'" & valFld2 & "',"
                        sValues &= "NULL"
                    ElseIf ValueType = "BtwFldDate" Then
                        sValues &= "'" & valTbl1 & "',"
                        sValues &= "'" & valFld1 & "',"
                        sValues &= "'Date2',"
                        sValues &= "'" & value2 & "',"
                        sValues &= "NULL"
                    ElseIf ValueType = "BtwFields" Then
                        sValues &= "'" & valTbl1 & "',"
                        sValues &= "'" & valFld1 & "',"
                        sValues &= "'" & valTbl2 & "',"
                        sValues &= "'" & valFld2 & "',"
                        sValues &= "NULL"
                    End If
                    insSQL = "INSERT INTO OURReportSQLquery (" & sFields & ") VALUES (" & sValues & ")"
                End If
            Else
                If btnTxt1.Text = "Fields" Then
                    valTyp1 = "Static"
                    value = TextBoxStatic.Text
                    If value = String.Empty Then
                        TextBoxStatic.Focus()
                        Return
                    End If
                Else
                    valTyp1 = "Field"
                    valTblFld1 = DropDownFieldW2.Text.Trim
                    If valTblFld1 = String.Empty Then
                        DropDownFieldW2.Focus()
                        Return
                    End If
                    valTbl1 = Piece(valTblFld1, ".", 1)
                    valFld1 = Piece(valTblFld1, ".", 2)
                    value = valTblFld1
                End If
                If btnTxt2.Text = "Fields" Then
                    valTyp2 = "Static"
                    value2 = TextBoxStatic2.Text
                    If value2 = String.Empty Then
                        TextBoxStatic2.Focus()
                        Return
                    End If
                Else
                    valTyp2 = "Field"
                    valTblFld2 = DropDownFieldW3.Text.Trim
                    If valTblFld2 = String.Empty Then
                        DropDownFieldW3.Focus()
                        Return
                    End If
                    valTbl2 = Piece(valTblFld2, ".", 1)
                    valFld2 = Piece(valTblFld2, ".", 2)
                    value2 = valTblFld2
                End If
                If valTyp1 = "Static" AndAlso valTyp2 = "Static" Then
                    ValueType = "BtwValues"
                ElseIf valTyp1 = "Static" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwValFld"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "Static" Then
                    ValueType = "BtwFldVal"
                Else
                    ValueType = "BtwFields"
                End If

                If bEditing Then
                    insSQL = "UPDATE OURReportSQLquery "
                    insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                    insSQL &= "Doing = 'WHERE',"
                    insSQL &= "Type = '" & ValueType & "',"
                    insSQL &= "Tbl1 = '" & mWhereTable & "',"
                    insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                    insSQL &= "Oper = '" & mWhereOperator & "',"
                    If ValueType = "BtwFields" Then
                        insSQL &= "Tbl2 = '" & valTbl1 & "',"
                        insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                        insSQL &= "Tbl3 = '" & valTbl2 & "',"
                        insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    ElseIf ValueType = "BtwValues" Then
                        insSQL &= "Tbl2 = 'Value1',"
                        insSQL &= "Tbl2Fld2 = '" & value & "',"
                        insSQL &= "Tbl3 = 'Value2',"
                        insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    ElseIf ValueType = "BtwValFld" Then
                        insSQL &= "Tbl2 = 'Value1',"
                        insSQL &= "Tbl2Fld2 = '" & value & "',"
                        insSQL &= "Tbl3 = '" & valTbl2 & "',"
                        insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    ElseIf ValueType = "BtwFldVal" Then
                        insSQL &= "Tbl2 = '" & valTbl1 & "',"
                        insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                        insSQL &= "Tbl3 = 'Value2',"
                        insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                        insSQL &= "Comments = NULL "
                        insSQL &= "Where Indx = " & indx & " "
                    End If
                Else
                    sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments"
                    sValues = "'" & Session("REPORTID") & "',"
                    sValues &= "'WHERE',"
                    sValues &= "'" & ValueType & "',"
                    sValues &= "'" & mWhereTable & "',"
                    sValues &= "'" & mWhereField & "',"
                    sValues &= "'" & mWhereOperator & "',"
                    If ValueType = "BtwFields" Then
                        sValues &= "'" & valTbl1 & "',"
                        sValues &= "'" & valFld1 & "',"
                        sValues &= "'" & valTbl2 & "',"
                        sValues &= "'" & valFld2 & "',"
                        sValues &= "NULL"
                    ElseIf ValueType = "BtwValues" Then
                        sValues &= "'Value1',"
                        sValues &= "'" & value & "',"
                        sValues &= "'Value2',"
                        sValues &= "'" & value2 & "',"
                        sValues &= "NULL"
                    ElseIf ValueType = "BtwValFld" Then
                        sValues &= "'Value1',"
                        sValues &= "'" & value & "',"
                        sValues &= "'" & valTbl2 & "',"
                        sValues &= "'" & valFld2 & "',"
                        sValues &= "NULL"
                    ElseIf ValueType = "BtwFldVal" Then
                        sValues &= "'" & valTbl1 & "',"
                        sValues &= "'" & valFld1 & "',"
                        sValues &= "'Value2',"
                        sValues &= "'" & value2 & "',"
                        sValues &= "NULL"
                    End If
                    insSQL = "INSERT INTO OURReportSQLquery (" & sFields & ") VALUES (" & sValues & ")"
                End If

            End If

        End If
        ExequteSQLquery(insSQL)
        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    'Protected Sub ButtonAddCondition1_Click(sender As Object, e As EventArgs) Handles ButtonAddCondition1.Click
    '    'insert field condition
    '    Dim insSQL As String = String.Empty
    '    insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Comments) VALUES('" & Session("REPORTID") & "','WHERE','Field','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','" & DropDownOperator.Text & "','" & DropDownTableW2.Text & "','" & DropDownFieldW2.Text & "','" & TextBoxStatic.Text & "')"
    '    ExequteSQLquery(insSQL)
    '    Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    'End Sub
    'Protected Sub ButtonAddCondition2_Click(sender As Object, e As EventArgs) Handles ButtonAddCondition2.Click
    '    'between two fields
    '    Dim insSQL As String = String.Empty
    '    insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwFields','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','" & DropDownTableW2.Text & "','" & DropDownFieldW2.Text & "','" & DropDownTableW3.Text & "','" & DropDownFieldW3.Text & "','')"
    '    ExequteSQLquery(insSQL)
    '    Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    'End Sub
    'Protected Sub ButtonAddCondition3_Click(sender As Object, e As EventArgs) Handles ButtonAddCondition3.Click
    '    'between two dates 
    '    Dim insSQL As String = String.Empty
    '    'make Date2 and Date3 relative
    '    Dim datefunc1 As String = String.Empty
    '    Dim datefunc2 As String = String.Empty
    '    Dim datetype As String = String.Empty

    '    'make Date3 relative
    '    If CheckBoxDate3Relative.Checked Then
    '        Dim intdate2 As String = MakeDateRelative(Now().ToShortDateString, ddDate2.SelectedDate.ToShortDateString)
    '        If Session("UserConnProvider") = "InterSystems.Data.CacheClient" Then
    '            datefunc2 = "DATEADD(dd," & intdate2 & ",CURRENT_DATE)"
    '        ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
    '            If CInt(intdate2) < 0 Then
    '                datefunc2 = "DATE_SUB(CURDATE(), INTERVAL " & (-CInt(intdate2)).ToString & " DAY)"
    '            ElseIf CInt(intdate2) > 0 Then
    '                datefunc2 = "DATE_ADD(CURDATE(), INTERVAL " & intdate2 & " DAY)"
    '            Else 'intdate1=0
    '                datefunc2 = "CURDATE()"
    '            End If
    '        Else 'SQL Server
    '            datefunc2 = "DateAdd(dd," & intdate2 & ",GETDATE())"
    '        End If
    '        datetype = datetype & "DATE2"
    '    End If
    '    If datetype = "DATE1" Then  'Date1 is relative
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BwRD1Date2','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','DATE1:','" & datefunc1 & "','DATE2:','" & ddDate2.SelectedDate.ToShortDateString & "','')"
    '    ElseIf datetype = "DATE2" Then 'Date2 is relative
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BwDate1RD2','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','DATE1:','" & ddDate1.SelectedDate.ToShortDateString & "','DATE2:','" & datefunc2 & "','')"
    '    ElseIf datetype = "DATE1DATE2" Then 'Date1,2 is relative
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwRD1RD2','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','DATE1:','" & datefunc1 & "','DATE2:','" & datefunc2 & "','')"
    '    Else  'no relative dates
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwDates','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','DATE1:','" & ddDate1.SelectedDate.ToShortDateString & "','DATE2:','" & ddDate2.SelectedDate.ToShortDateString & "','')"
    '    End If
    '    ExequteSQLquery(insSQL)
    '    Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    'End Sub
    'Protected Sub ButtonAddCondition5_Click(sender As Object, e As EventArgs) Handles ButtonAddCondition5.Click
    '    'between date2 and field 
    '    Dim insSQL As String = String.Empty
    '    Dim datefunc As String = String.Empty
    '    'make Date2 relative
    '    If CheckBoxDate2Relative.Checked Then
    '        Dim intdate1 As String = MakeDateRelative(Now().ToShortDateString, ddDate1.SelectedDate.ToShortDateString)
    '        If Session("UserConnProvider") = "InterSystems.Data.CacheClient" Then
    '            datefunc = "DATEADD(dd," & intdate1 & ",CURRENT_DATE)"
    '        ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
    '            If CInt(intdate1) > 0 Then
    '                datefunc = "DATE_SUB(" & intdate1 & ",CURDATE())"
    '            ElseIf CInt(intdate1) < 0 Then
    '                datefunc = "DATE_ADD(" & intdate1 & ",CURDATE())"
    '            Else 'intdate1=0
    '                datefunc = "CURDATE()"
    '            End If
    '        Else 'SQL Server
    '            datefunc = "DateAdd(dd," & intdate1 & ",GETDATE())"
    '        End If
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwRD1Fld','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','DATE1:','" & datefunc & "','" & DropDownTableW3.Text & "','" & DropDownFieldW3.Text & "','')"
    '    Else
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwDateFld','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','DATE1:','" & ddDate1.SelectedDate.ToShortDateString & "','" & DropDownTableW3.Text & "','" & DropDownFieldW3.Text & "','')"
    '    End If
    '    ExequteSQLquery(insSQL)
    '    Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    'End Sub
    'Protected Sub ButtonAddCondition4_Click(sender As Object, e As EventArgs) Handles ButtonAddCondition4.Click
    '    'between field and date3
    '    Dim insSQL As String = String.Empty
    '    'make Date3 relative
    '    Dim datefunc As String = String.Empty
    '    If CheckBoxDate3Relative.Checked Then
    '        Dim intdate2 As String = MakeDateRelative(Now().ToShortDateString, ddDate2.SelectedDate.ToShortDateString)
    '        If Session("UserConnProvider") = "InterSystems.Data.CacheClient" Then
    '            datefunc = "DATEADD(dd," & intdate2 & ",CURRENT_DATE)"
    '        ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
    '            If CInt(intdate2) > 0 Then
    '                datefunc = "DATE_SUB(" & intdate2 & ",CURDATE())"
    '            ElseIf CInt(intdate2) < 0 Then
    '                datefunc = "DATE_ADD(" & intdate2 & ",CURDATE())"
    '            Else 'intdate1=0
    '                datefunc = "CURDATE()"
    '            End If
    '        Else 'SQL Server
    '            datefunc = "DATEADD(dd," & intdate2 & ",GETDATE())"
    '        End If
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwFldRD2','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','" & DropDownTableW2.Text & "','" & DropDownFieldW2.Text & "','DATE2:','" & datefunc & "','')"
    '    Else
    '        insSQL = "INSERT INTO OURReportSQLquery (ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments) VALUES('" & Session("REPORTID") & "','WHERE','BtwFldDate','" & DropDownTableW1.Text & "','" & DropDownFieldW1.Text & "','BETWEEN','" & DropDownTableW2.Text & "','" & DropDownFieldW2.Text & "','DATE2:','" & ddDate2.SelectedDate.ToShortDateString & "','')"
    '    End If
    '    ExequteSQLquery(insSQL)
    '        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    'End Sub
    Protected Sub ButtonSubmitSELECT_Click(sender As Object, e As EventArgs) Handles ButtonSubmitSELECT.Click, ButtonSubmitJoins.Click, ButtonSubmitWHERE.Click, ButtonSubmitSorting.Click
        'save SQL query in OURReportInfo 
        LabelAlert1.Text = ""
        Dim ret As String = String.Empty
        Dim ErrorLog As String = String.Empty

        'update DISTINCT in ourreportinfo
        Dim dist As String = CheckBoxDistinct.Checked.ToString
        If dist.ToUpper = "TRUE" Then
            ExequteSQLquery("UPDATE OURReportInfo SET Param6type='" & dist & "' WHERE (ReportID='" & Session("REPORTID") & "')")
        Else
            ExequteSQLquery("UPDATE OURReportInfo SET Param6type='' WHERE (ReportID='" & Session("REPORTID") & "')")
        End If

        'add report joins to report dbname_joins
        Dim er As String = String.Empty
        ret = UpdateJoins(repid, Session("dbname").Replace("#", "") & "_joins", Session("UserDB"), Session("UserConnString"), Session("UserConnProvider"), er)

        'update sqlquerytest in ourreportinfo
        Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
        If sqlquerytext.Trim = "" Then
            sqlquerytext = LabelSQL.Text
        Else
            LabelSQL.Text = sqlquerytext
        End If
        If sqlquerytext.Replace("SELECT", "").Replace("*", "").Replace("FROM", "").Trim = "" Then
            LabelAlert1.Text = "Select fields to the query first!! </br> "
            Exit Sub
        End If
        ret = SaveSQLquery(repid, sqlquerytext)
        If ret = "Query executed fine." Then
            Session("dv3") = Nothing
            ErrorLog = UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
            WriteToAccessLog(Session("logon"), ret & "  " & ErrorLog, 0)
            If ErrorLog.StartsWith("ERROR!!") Then
                'LabelAlert1.ForeColor = Drawing.Color.DeepPink
                Session("NewFileCreated") = False
                LabelAlert1.Text = ret & " </br> " & ErrorLog & " </br> "
            Else 'If TextBoxReportFile.Text.Trim = String.Empty Then
                Session("NewFileCreated") = True
                Session("FileJustDeleted") = False
                ret = CreateCleanReportColumnsFieldsItems(Session("REPORTID"), Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))

                ''set OURReportFormat and OURReportItems
                'Dim dtrf As DataTable
                'Dim ddt As DataTable = GetListOfReportFields(repid)  'List of Report fields from xsd
                'If ddt IsNot Nothing Then
                '    dtrf = GetReportFields(repid)
                '    If dtrf Is Nothing OrElse dtrf.Rows.Count = 0 Then  'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
                '        Dim frname As String
                '        Dim insSQL As String
                '        'add all fields from ddt
                '        For i As Integer = 0 To ddt.Columns.Count - 1
                '            If ddt.Columns(i).Caption <> "Indx" Then
                '                frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                '                If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                '                insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & Session("REPORTID") & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
                '                ExequteSQLquery(insSQL)
                '            End If
                '        Next
                '    End If
                '    If HasReportColumns(repid) AndAlso Not ReportItemsExist(repid) Then
                '        Dim retr As String = CreateReportItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                '        If retr.StartsWith("ERROR!!") Then
                '            LabelAlert.Text = "Report info is not updated. " & retr
                '            LabelAlert.Visible = True
                '            Exit Sub
                '        End If
                '    End If
                'End If
                LabelAlert1.Text = "Report Format has been updated. </br> "
            End If
            'update parameters with new sql query - sqlquerytext
            If Session("Attributes") = "sql" Then
                ret = UpdateParameters(repid, sqlquerytext)
                LabelAlert1.Text = LabelAlert1.Text & ret
            End If
        Else
            LabelAlert1.Text = ret
        End If
    End Sub
    Protected Sub CheckBoxSysTables_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSysTables.CheckedChanged
        Dim ddt As DataTable
        'list of tables on "select" tab include all tables
        DropDownTables.Items.Clear()
        ddt = GetReportTablesFromSQLqueryText(Session("ReportID"))
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownTables.Items.Add(ddt.Rows(i)("Tbl1"))
        Next
        Dim err As String = String.Empty
        ddt = GetListOfUserTables(CheckBoxSysTables.Checked, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV")).Table
        If err <> "" Then
            LabelAlert1.Text = err
            Exit Sub
        End If
        Dim tbl As String = String.Empty
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            tbl = ddt.Rows(i)("TABLE_NAME").ToString
            If DropDownTables.Items.FindByText(tbl) Is Nothing Then
                DropDownTables.Items.Add(tbl)
            End If
            'DropDownTables.Items.Add(ddt.Rows(i)("TABLE_NAME"))
        Next
        If DropDownTables.Items.Count > 0 Then DropDownTables.Items(0).Selected = True
        'fields
        DropDownColumns.Items.Clear()
        ddt = GetListOfTableColumns(DropDownTables.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownColumns.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
            DropDownColumns.Items(i).Selected = False
        Next
    End Sub
    'Protected Sub CheckBoxAllFields_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllFields.CheckedChanged
    '    If CheckBoxAllFields.Checked Then
    '        Dim ddt As DataTable
    '        DropDownColumns.Items.Clear()
    '        ddt = GetListOfTableColumns(DropDownTables.SelectedItem.Text).Table
    '        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
    '            DropDownColumns.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
    '            DropDownColumns.Items(i).Selected = False
    '        Next
    '    End If
    'End Sub
    Protected Sub MenuMain_MenuItemClick(ByVal sender As Object, ByVal e As MenuEventArgs) Handles MenuMain.MenuItemClick
        Dim i As Integer
        i = Int32.Parse(e.Item.Value)
        If i = 10 Then
            Session("TabN") = 0
            Session("TabNQ") = 0
            Session("TabNF") = 0
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 11 Then
            Session("TabN") = 0
            Session("TabNF") = 0
            Session("TabNQ") = 1
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 12 Then
            Session("TabN") = 0
            Session("TabNF") = 0
            Session("TabNQ") = 2
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 13 Then
            Session("TabN") = 0
            Session("TabNQ") = 3
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 20 Then
            Session("TabN") = 1
            Session("TabNF") = 0
            Session("TabNQ") = 0
            Response.Redirect("~/RDLformat.aspx")
        ElseIf i = 21 Then
            Session("TabN") = 1
            Session("TabNF") = 1
            Session("TabNQ") = 0
            Response.Redirect("~/RDLformat.aspx")
        ElseIf i = 22 Then
            Session("TabN") = 1
            Session("TabNF") = 2
            Response.Redirect("~/RDLformat.aspx")
        Else
            Session("TabN") = i
            Session("TabNF") = 0
            Session("TabNQ") = 0
            Response.Redirect("~/ReportEdit.aspx")
        End If
        LabelAlert1.Text = ""
    End Sub
    Protected Sub btnSelectAllClicked(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        If Not DropDownColumns.AllSelected Then
            DropDownColumns.SetAll(True)
            DropDownColumns.Text = "ALL"
            btnSelectAll.Enabled = False
            btnSelectAll.CssClass = "DataButtonDisabled"
            btnUnselectAll.Enabled = True
            btnUnselectAll.CssClass = "DataButtonEnabled"
        End If
    End Sub
    Protected Sub btnUnselectAllClicked(sender As Object, e As EventArgs) Handles btnUnselectAll.Click
        If Not DropDownColumns.NoneSelected Then
            DropDownColumns.SetAll(False)
            DropDownColumns.Text = "Please select..."
            btnSelectAll.Enabled = True
            btnSelectAll.CssClass = "DataButtonEnabled"
            btnUnselectAll.Enabled = False
            btnUnselectAll.CssClass = "DataButtonDisabled"
        End If

    End Sub
    Protected Sub btnNewCondition_Click(sender As Object, e As EventArgs) Handles btnNewCondition.Click
        'ClearConditionEntry()
        'lblConditionEntry.Text = "New Condition"
        'ButtonAddCondition.Text = "Add Condition"
        'trConditionEntry.Visible = True
        'Session("Index") = ""
        'DropDownFieldW1.Focus()
        Session("Index") = ""
        dlgCondition.FieldItems = mReportFields
        dlgCondition.Show("New Condition", New Controls_ConditionDlg.ConditionData, Controls_ConditionDlg.Mode.Add, "Add Condition")
    End Sub
    Private Function GetRelativeDateFunction(DateVal As DateTime) As String
        Dim DateFunc As String = String.Empty
        Dim Days As String = MakeDateRelative(Now, DateVal)

        If IsNumeric(Days) Then
            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data.") Then
                DateFunc = "DATEADD(dd," & Days & ",CURRENT_DATE)"
            ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
                If CInt(Days) > 0 Then
                    DateFunc = "DATE_ADD(CURDATE(), INTERVAL " & Days & " DAY)"
                ElseIf CInt(Days) < 0 Then
                    DateFunc = "DATE_SUB(CURDATE(), INTERVAL " & -(CInt(Days)).ToString & " DAY)"
                Else 'Days=0
                    DateFunc = "CURDATE()"
                End If
            ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
                If CInt(Days) > 0 Then
                    DateFunc = "(CURRENT_DATE + " & Days.ToString & ")"
                ElseIf CInt(Days) < 0 Then
                    DateFunc = "(CURRENT_DATE - " & -(CInt(Days)).ToString & ")"
                Else 'Days=0
                    DateFunc = "CURRENT_DATE"
                End If

                'TODO if needed for PostgreSQL: ElseIf Session("UserConnProvider") = "Npgsql" Then  'PostgreSQL  Npgsql

            Else 'SQL Server
                DateFunc = "DATEADD(day," & Days & ",GETDATE())"
            End If
        End If

        Return DateFunc
    End Function
    Private Sub EditCondition(CData As Controls_ConditionDlg.ConditionData)
        Dim sqlindx As String = Session("Indx")
        Dim value As String = String.Empty
        Dim value2 As String = String.Empty
        Dim ValueType As String = String.Empty
        Dim insSQL As String = String.Empty
        Dim valTblFld1 As String = String.Empty
        Dim valTbl1 As String = String.Empty
        Dim valFld1 As String = String.Empty
        Dim valTblFld2 As String = String.Empty
        Dim valTbl2 As String = String.Empty
        Dim valFld2 As String = String.Empty
        Dim valTyp1 As String = String.Empty
        Dim valTyp2 As String = String.Empty

        Dim pces As Integer = Pieces(CData.Field, ".")
        mWhereTable = Piece(CData.Field, ".", 1, pces - 1)
        mWhereField = Piece(CData.Field, ".", pces)
        mWhereOperator = GetNotOperator(CData.ConditionOperator, CData.NotOperator)

        If CData.ConditionOperator <> "BETWEEN" Then
            If CData.IsDate Then
                If Not CData.DateFieldChosen1 Then
                    If CData.DateRelative1 Then
                        ValueType = "RelDate"
                        value = GetRelativeDateFunction(CDate(CData.DateValue1))
                    Else
                        ValueType = "DateTime"
                        value = CData.DateValue1
                    End If
                Else
                    ValueType = "Field"
                    valTblFld1 = CData.DateFieldValue1
                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
            Else
                If Not CData.FieldChosen1 Then
                    ValueType = "Static"
                    value = CData.TextValue1
                Else
                    ValueType = "Field"
                    valTblFld1 = CData.FieldValue1

                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
            End If
            If ValueType <> "Field" Then
                insSQL = "UPDATE OURReportSQLquery  "
                insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                insSQL &= "Doing = 'WHERE',"
                insSQL &= "Type = '" & ValueType & "',"
                insSQL &= "Tbl1 = '" & mWhereTable & "',"
                insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                insSQL &= "Oper = '" & mWhereOperator & "',"
                insSQL &= "Tbl2 = NULL,"
                insSQL &= "Tbl2Fld2 = NULL,"
                insSQL &= "Tbl3 = NULL,"
                insSQL &= "Tbl3Fld3 = NULL,"
                insSQL &= "Comments = '" & value & "' "
                insSQL &= "Where Indx = " & sqlindx
            Else
                insSQL = "UPDATE OURReportSQLquery "
                insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                insSQL &= "Doing = 'WHERE',"
                insSQL &= "Type = '" & ValueType & "',"
                insSQL &= "Tbl1 = '" & mWhereTable & "',"
                insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                insSQL &= "Oper = '" & mWhereOperator & "',"
                insSQL &= "Tbl2 = '" & valTbl1 & "',"
                insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                insSQL &= "Tbl3 = NULL,"
                insSQL &= "Tbl3Fld3 = NULL,"
                insSQL &= "Comments = '" & value & "' "
                insSQL &= "Where Indx = " & sqlindx
            End If
        Else
            If CData.IsDate Then
                If Not CData.DateFieldChosen1 Then
                    If CData.DateRelative1 Then
                        valTyp1 = "RelativeDate"
                        value = GetRelativeDateFunction(CDate(CData.DateValue1))
                    Else
                        valTyp1 = "DateTime"
                        value = CData.DateValue1
                    End If
                Else
                    valTyp1 = "Field"
                    valTblFld1 = CData.DateFieldValue1
                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
                If Not CData.DateFieldChosen2 Then
                    If CData.DateRelative2 Then
                        valTyp2 = "RelativeDate"
                        value2 = GetRelativeDateFunction(CDate(CData.DateValue2))
                    Else
                        valTyp2 = "DateTime"
                        value2 = CData.DateValue2
                    End If

                Else
                    valTyp2 = "Field"
                    valTblFld2 = CData.DateFieldValue2

                    pces = Pieces(valTblFld2, ".")

                    valTbl2 = Piece(valTblFld2, ".", 1, pces - 1)
                    valFld2 = Piece(valTblFld2, ".", pces)
                    value2 = valTblFld2
                End If
                If valTyp1 = "DateTime" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BtwDates"
                ElseIf valTyp1 = "DateTime" AndAlso valTyp2 = "RelativeDate" Then
                    ValueType = "BwDate1RD2"
                ElseIf valTyp1 = "RelativeDate" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BwRD1Date2"
                ElseIf valTyp1 = "RelativeDate" AndAlso valTyp2 = "RelativeDate" Then
                    ValueType = "BtwRD1RD2"
                ElseIf valTyp1 = "RelativeDate" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwRD1Fld"
                ElseIf valTyp1 = "DateTime" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwDateFld"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BtwFldDate"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "RelativeDate" Then
                    ValueType = "BtwFldRD2"
                Else
                    ValueType = "BtwFields"
                End If
                insSQL = "UPDATE OURReportSQLquery "
                insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                insSQL &= "Doing = 'WHERE',"
                insSQL &= "Type = '" & ValueType & "',"
                insSQL &= "Tbl1 = '" & mWhereTable & "',"
                insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                insSQL &= "Oper = '" & mWhereOperator & "',"

                If ValueType = "BtwDates" OrElse
                   ValueType = "BwRD1Date2" OrElse
                   ValueType = "BwDate1RD2" OrElse
                   ValueType = "BtwRD1RD2" Then
                    insSQL &= "Tbl2 = 'Date1',"
                    insSQL &= "Tbl2Fld2 = '" & value & "',"
                    insSQL &= "Tbl3 = 'Date2',"
                    insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwDateFld" OrElse ValueType = "BtwRD1Fld" Then
                    insSQL &= "Tbl2 = 'Date1',"
                    insSQL &= "Tbl2Fld2 = '" & value & "',"
                    insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwFldDate" OrElse ValueType = "BtwFldRD2" Then
                    insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    insSQL &= "Tbl3 = 'Date2',"
                    insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwFields" Then
                    insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    insSQL &= "Comments = NULL"
                End If
                insSQL &= " Where Indx = " & sqlindx
            Else
                If Not CData.FieldChosen1 Then
                    valTyp1 = "Static"
                    value = CData.TextValue1
                Else
                    valTyp1 = "Field"
                    valTblFld1 = CData.FieldValue1

                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
                If Not CData.FieldChosen2 Then
                    valTyp2 = "Static"
                    value2 = CData.TextValue2
                Else
                    valTyp2 = "Field"
                    valTblFld2 = CData.FieldValue2

                    pces = Pieces(valTblFld2, ".")

                    valTbl2 = Piece(valTblFld2, ".", 1, pces - 1)
                    valFld2 = Piece(valTblFld2, ".", pces)
                    value2 = valTblFld2
                End If
                If valTyp1 = "Static" AndAlso valTyp2 = "Static" Then
                    ValueType = "BtwValues"
                ElseIf valTyp1 = "Static" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwValFld"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "Static" Then
                    ValueType = "BtwFldVal"
                Else
                    ValueType = "BtwFields"
                End If
                insSQL = "UPDATE OURReportSQLquery "
                insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                insSQL &= "Doing = 'WHERE',"
                insSQL &= "Type = '" & ValueType & "',"
                insSQL &= "Tbl1 = '" & mWhereTable & "',"
                insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                insSQL &= "Oper = '" & mWhereOperator & "',"

                If ValueType = "BtwFields" Then
                    insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwValues" Then
                    insSQL &= "Tbl2 = 'Value1',"
                    insSQL &= "Tbl2Fld2 = '" & value & "',"
                    insSQL &= "Tbl3 = 'Value2',"
                    insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwValFld" Then
                    insSQL &= "Tbl2 = 'Value1',"
                    insSQL &= "Tbl2Fld2 = '" & value & "',"
                    insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwFldVal" Then
                    insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    insSQL &= "Tbl3 = 'Value2',"
                    insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    insSQL &= "Comments = NULL"
                End If
                insSQL &= " Where Indx = " & sqlindx
            End If
        End If
        ExequteSQLquery(insSQL)
        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Private Function GetNotOperator(oper As String, NotOper As Boolean) As String
        Dim ret As String = oper
        If NotOper Then
            Select Case oper
                Case "="
                    ret = "<>"
                Case "<>"
                    ret = "="
                Case "<"
                    ret = ">="
                Case ">"
                    ret = "<="
                Case "<="
                    ret = ">"
                Case ">="
                    ret = "<"
                Case Else
                    ret = "Not " & oper
            End Select
        End If
        Return ret
    End Function
    Private Sub AddCondition(CData As Controls_ConditionDlg.ConditionData)
        Dim value As String = String.Empty
        Dim value2 As String = String.Empty
        Dim ValueType As String = String.Empty
        Dim insSQL As String = String.Empty
        Dim valTblFld1 As String = String.Empty
        Dim valTbl1 As String = String.Empty
        Dim valFld1 As String = String.Empty
        Dim valTblFld2 As String = String.Empty
        Dim valTbl2 As String = String.Empty
        Dim valFld2 As String = String.Empty
        Dim valTyp1 As String = String.Empty
        Dim valTyp2 As String = String.Empty

        Dim pces As Integer = Pieces(CData.Field, ".")
        Dim logical As String = String.Empty

        Dim sFields As String = String.Empty
        Dim sValues As String = String.Empty

        'get ungrouped logical value
        Dim dt As DataTable = GetUngroupedConditions(Session("REPORTID"))
        If dt IsNot Nothing Then
            If dt.Rows.Count > 0 Then
                logical = dt.Rows(0)("Logical").ToString
            Else
                dt = GetSQLConditionGroups(Session("REPORTID"))
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then _
                    logical = dt.Rows(0)("Logical").ToString
            End If
        End If
        If logical = String.Empty Then logical = "And"

        mWhereTable = Piece(CData.Field, ".", 1, pces - 1)
        mWhereField = Piece(CData.Field, ".", pces)
        mWhereOperator = GetNotOperator(CData.ConditionOperator, CData.NotOperator)

        If CData.ConditionOperator.ToUpper <> "BETWEEN" Then

            If CData.IsDate Then
                If Not CData.DateFieldChosen1 Then
                    If CData.DateRelative1 Then
                        ValueType = "RelDate"
                        value = GetRelativeDateFunction(CDate(CData.DateValue1))
                    Else
                        ValueType = "DateTime"
                        value = CData.DateValue1
                    End If
                Else
                    ValueType = "Field"
                    valTblFld1 = CData.DateFieldValue1
                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
            Else
                If Not CData.FieldChosen1 Then
                    ValueType = "Static"
                    value = CData.TextValue1
                Else
                    ValueType = "Field"
                    valTblFld1 = CData.FieldValue1

                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
            End If
            If ValueType <> "Field" Then
                sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Comments," & FixReservedWords("Group") & ",Logical"
                sValues = "'" & Session("REPORTID") & "',"
                sValues &= "'WHERE',"
                sValues &= "'" & ValueType & "',"
                sValues &= "'" & mWhereTable & "',"
                sValues &= "'" & mWhereField & "',"
                sValues &= "'" & mWhereOperator & "',"
                sValues &= "'" & value & "',"
                sValues &= "NULL,"
                sValues &= "'" & logical & "'"

                'insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                'insSQL &= "Doing = 'WHERE',"
                'insSQL &= "Type = '" & ValueType & "',"
                'insSQL &= "Tbl1 = '" & mWhereTable & "',"
                'insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                'insSQL &= "Oper = '" & mWhereOperator & "',"
                'insSQL &= "Comments = '" & value & "',"
                'insSQL &= FixReservedWords("Group") & " = NULL,"
                'insSQL &= "Logical = '" & logical & "'"
            Else
                sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper,Tbl2,Tbl2Fld2,Comments," & FixReservedWords("Group") & ",Logical"
                sValues = "'" & Session("REPORTID") & "',"
                sValues &= "'WHERE',"
                sValues &= "'" & ValueType & "',"
                sValues &= "'" & mWhereTable & "',"
                sValues &= "'" & mWhereField & "',"
                sValues &= "'" & mWhereOperator & "',"
                sValues &= "'" & valTbl1 & "',"
                sValues &= "'" & valFld1 & "',"
                sValues &= "'" & value & "',"
                sValues &= "NULL,"
                sValues &= "'" & logical & "'"

                'insSQL = "INSERT INTO OURReportSQLquery "
                'insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                'insSQL &= "Doing = 'WHERE',"
                'insSQL &= "Type = '" & ValueType & "',"
                'insSQL &= "Tbl1 = '" & mWhereTable & "',"
                'insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                'insSQL &= "Oper = '" & mWhereOperator & "',"
                'insSQL &= "Tbl2 = '" & valTbl1 & "',"
                'insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                'insSQL &= "Comments = '" & value & "',"
                'insSQL &= FixReservedWords("Group") & " = NULL,"
                'insSQL &= "Logical = '" & logical & "'"
            End If
            insSQL = "INSERT INTO OURReportSQLquery  (" & sFields & ") VALUES (" & sValues & ")"
        Else
            If CData.IsDate Then
                If Not CData.DateFieldChosen1 Then
                    If CData.DateRelative1 Then
                        valTyp1 = "RelativeDate"
                        value = GetRelativeDateFunction(CDate(CData.DateValue1))
                    Else
                        valTyp1 = "DateTime"
                        value = CData.DateValue1
                    End If
                Else
                    valTyp1 = "Field"
                    valTblFld1 = CData.DateFieldValue1
                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
                If Not CData.DateFieldChosen2 Then
                    If CData.DateRelative2 Then
                        valTyp2 = "RelativeDate"
                        value2 = GetRelativeDateFunction(CDate(CData.DateValue2))
                    Else
                        valTyp2 = "DateTime"
                        value2 = CData.DateValue2
                    End If

                Else
                    valTyp2 = "Field"
                    valTblFld2 = CData.DateFieldValue2

                    pces = Pieces(valTblFld2, ".")

                    valTbl2 = Piece(valTblFld2, ".", 1, pces - 1)
                    valFld2 = Piece(valTblFld2, ".", pces)
                    value2 = valTblFld2
                End If
                If valTyp1 = "DateTime" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BtwDates"
                ElseIf valTyp1 = "DateTime" AndAlso valTyp2 = "RelativeDate" Then
                    ValueType = "BwDate1RD2"
                ElseIf valTyp1 = "RelativeDate" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BwRD1Date2"
                ElseIf valTyp1 = "RelativeDate" AndAlso valTyp2 = "RelativeDate" Then
                    ValueType = "BtwRD1RD2"
                ElseIf valTyp1 = "RelativeDate" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwRD1Fld"
                ElseIf valTyp1 = "DateTime" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwDateFld"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "DateTime" Then
                    ValueType = "BtwFldDate"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "RelativeDate" Then
                    ValueType = "BtwFldRD2"
                Else
                    ValueType = "BtwFields"
                End If
                sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper," & FixReservedWords("Group") & ",Logical,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments"

                sValues = "'" & Session("REPORTID") & "',"
                sValues &= "'WHERE',"
                sValues &= "'" & ValueType & "',"
                sValues &= "'" & mWhereTable & "',"
                sValues &= "'" & mWhereField & "',"
                sValues &= "'" & mWhereOperator & "',"
                sValues &= "NULL,"
                sValues &= "'" & logical & "',"

                'insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                'insSQL &= "Doing = 'WHERE',"
                'insSQL &= "Type = '" & ValueType & "',"
                'insSQL &= "Tbl1 = '" & mWhereTable & "',"
                'insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                'insSQL &= "Oper = '" & mWhereOperator & "',"
                'insSQL &= FixReservedWords("Group") & " = NULL,"
                'insSQL &= "Logical = '" & logical & "',"

                If ValueType = "BtwDates" OrElse
                   ValueType = "BwRD1Date2" OrElse
                   ValueType = "BwDate1RD2" OrElse
                   ValueType = "BtwRD1RD2" Then
                    sValues &= "'Date1',"
                    sValues &= "'" & value & "',"
                    sValues &= "'Date2',"
                    sValues &= "'" & value2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = 'Date1',"
                    'insSQL &= "Tbl2Fld2 = '" & value & "',"
                    'insSQL &= "Tbl3 = 'Date2',"
                    'insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    'insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwDateFld" OrElse ValueType = "BtwRD1Fld" Then
                    sValues &= "'Date1',"
                    sValues &= "'" & value & "',"
                    sValues &= "'" & valTbl2 & "',"
                    sValues &= "'" & valFld2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = 'Date1',"
                    'insSQL &= "Tbl2Fld2 = '" & value & "',"
                    'insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    'insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    'insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwFldDate" OrElse ValueType = "BtwFldRD2" Then
                    sValues &= "'" & valTbl1 & "',"
                    sValues &= "'" & valFld1 & "',"
                    sValues &= "'Date2',"
                    sValues &= "'" & value2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    'insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    'insSQL &= "Tbl3 = 'Date2',"
                    'insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    'insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwFields" Then
                    sValues &= "'" & valTbl1 & "',"
                    sValues &= "'" & valFld1 & "',"
                    sValues &= "'" & valTbl2 & "',"
                    sValues &= "'" & valFld2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    'insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    'insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    'insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    'insSQL &= "Comments = NULL"
                End If
                insSQL = "INSERT INTO OURReportSQLquery  (" & sFields & ") VALUES (" & sValues & ")"
            Else
                If Not CData.FieldChosen1 Then
                    valTyp1 = "Static"
                    value = CData.TextValue1
                Else
                    valTyp1 = "Field"
                    valTblFld1 = CData.FieldValue1

                    pces = Pieces(valTblFld1, ".")

                    valTbl1 = Piece(valTblFld1, ".", 1, pces - 1)
                    valFld1 = Piece(valTblFld1, ".", pces)
                    value = valTblFld1
                End If
                If Not CData.FieldChosen2 Then
                    valTyp2 = "Static"
                    value2 = CData.TextValue2
                Else
                    valTyp2 = "Field"
                    valTblFld2 = CData.FieldValue2

                    pces = Pieces(valTblFld2, ".")

                    valTbl2 = Piece(valTblFld2, ".", 1, pces - 1)
                    valFld2 = Piece(valTblFld2, ".", pces)
                    value2 = valTblFld2
                End If
                If valTyp1 = "Static" AndAlso valTyp2 = "Static" Then
                    ValueType = "BtwValues"
                ElseIf valTyp1 = "Static" AndAlso valTyp2 = "Field" Then
                    ValueType = "BtwValFld"
                ElseIf valTyp1 = "Field" AndAlso valTyp2 = "Static" Then
                    ValueType = "BtwFldVal"
                Else
                    ValueType = "BtwFields"
                End If
                sFields = "ReportID,Doing,Type,Tbl1,Tbl1Fld1,Oper," & FixReservedWords("Group") & ",Logical,Tbl2,Tbl2Fld2,Tbl3,Tbl3Fld3,Comments"

                sValues = "'" & Session("REPORTID") & "',"
                sValues &= "'WHERE',"
                sValues &= "'" & ValueType & "',"
                sValues &= "'" & mWhereTable & "',"
                sValues &= "'" & mWhereField & "',"
                sValues &= "'" & mWhereOperator & "',"
                sValues &= "NULL,"
                sValues &= "'" & logical & "',"

                'insSQL = "INSERT INTO OURReportSQLquery "
                'insSQL &= "SET ReportID = '" & Session("REPORTID") & "',"
                'insSQL &= "Doing = 'WHERE',"
                'insSQL &= "Type = '" & ValueType & "',"
                'insSQL &= "Tbl1 = '" & mWhereTable & "',"
                'insSQL &= "Tbl1Fld1 = '" & mWhereField & "',"
                'insSQL &= "Oper = '" & mWhereOperator & "',"
                'insSQL &= FixReservedWords("Group") & " = NULL,"
                'insSQL &= "Logical = '" & logical & "',"

                If ValueType = "BtwFields" Then
                    sValues &= "'" & valTbl1 & "',"
                    sValues &= "'" & valFld1 & "',"
                    sValues &= "'" & valTbl2 & "',"
                    sValues &= "'" & valFld2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    'insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    'insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    'insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    'insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwValues" Then
                    sValues &= "'Value1',"
                    sValues &= "'" & value & "',"
                    sValues &= "'Value2',"
                    sValues &= "'" & value2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = 'Value1',"
                    'insSQL &= "Tbl2Fld2 = '" & value & "',"
                    'insSQL &= "Tbl3 = 'Value2',"
                    'insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    'insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwValFld" Then
                    sValues &= "'Value1',"
                    sValues &= "'" & value & "',"
                    sValues &= "'" & valTbl2 & "',"
                    sValues &= "'" & valFld2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = 'Value1',"
                    'insSQL &= "Tbl2Fld2 = '" & value & "',"
                    'insSQL &= "Tbl3 = '" & valTbl2 & "',"
                    'insSQL &= "Tbl3Fld3 = '" & valFld2 & "',"
                    'insSQL &= "Comments = NULL"
                ElseIf ValueType = "BtwFldVal" Then
                    sValues &= "'" & valTbl1 & "',"
                    sValues &= "'" & valFld1 & "',"
                    sValues &= "'Value2',"
                    sValues &= "'" & value2 & "',"
                    sValues &= "NULL"

                    'insSQL &= "Tbl2 = '" & valTbl1 & "',"
                    'insSQL &= "Tbl2Fld2 = '" & valFld1 & "',"
                    'insSQL &= "Tbl3 = 'Value2',"
                    'insSQL &= "Tbl3Fld3 = '" & value2 & "',"
                    'insSQL &= "Comments = NULL"
                End If
                insSQL = "INSERT INTO OURReportSQLquery  (" & sFields & ") VALUES (" & sValues & ")"
            End If
        End If
        Dim ret As String = ExequteSQLquery(insSQL)
        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Private Sub dlgCondition_CondDialogResulted(sender As Object, e As Controls_ConditionDlg.CondDlgBoxEventArgs) Handles dlgCondition.CondDialogResulted
        Select Case e.Result
            Case Controls_ConditionDlg.ConditionDialogResult.OK
                Dim CondData As Controls_ConditionDlg.ConditionData = e.ConditionItem
                repid = Session("REPORTID")
                If e.EntryMode = Controls_ConditionDlg.Mode.Add Then
                    AddCondition(CondData)
                ElseIf e.EntryMode = Controls_ConditionDlg.Mode.Edit Then
                    EditCondition(CondData)
                End If
        End Select
    End Sub
    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim rpt As String = Piece(sqlindx, "^", 1)
        Session("Indx") = Piece(sqlindx, "^", 2)
        Dim url As String = "SQLquery.aspx?Report=" & rpt & "&indx=" & Session("Indx") & "&delsqlw=yes"
        Response.Redirect(url)
    End Sub
    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Session("Indx") = sqlindx
        Dim udt As DataTable = GetSQLWhereCondition(repid, sqlindx)
        dlgCondition.FieldItems = mReportFields
        Dim CData As Controls_ConditionDlg.ConditionData = GetConditionData(udt)
        dlgCondition.Show("Update Condition", CData, Controls_ConditionDlg.Mode.Edit, "Update Condition")
        'ClearConditionEntry()
        'trConditionEntry.Visible = True
        'lblConditionEntry.Text = "Edit Condition"
        'ButtonAddCondition.Text = "Update Condition"
        'LoadConditionEntry(udt)
        'DropDownFieldW1.Focus()
    End Sub
    Sub GetSelectedFields()
        Dim selfields As String = String.Empty
        selfields = GetListOfSelectedFieldsFromSQLquery(repid)  ' get list of fields from sql query in commas - repid is a global variable
        If selfields <> "" Then
            selfields = selfields.Replace("`", "")
            selfields = selfields.Replace("[", "").Replace("]", "")
            selfields = selfields.Replace("""", "")
        End If
        Session("SELECTEDFields") = selfields
    End Sub
    Sub DeleteField(sqltbl As String, sqlfld As String)

        DeleteSQLField(repid, sqltbl, sqlfld)
        Session("ParamNames") = Nothing
        Session("ParamValues") = Nothing
        Session("ParamTypes") = Nothing
        Session("ParamFields") = Nothing
        Session("ParamsCount") = -1
        Session("srchfld") = Nothing
        Session("srchoper") = Nothing
        Session("srchval") = Nothing
        'update the SQL query
        Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
        If sqlquerytext.Trim = "" Then
            sqlquerytext = LabelSQL.Text
        End If
        Dim ret As String = SaveSQLquery(repid, sqlquerytext)
        'update xsd and rdl
        If ret = "Query executed fine." Then
            Session("dv3") = Nothing
            ret = ret & UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
            ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        Else
            LabelAlert1.Text = ret
            WriteToAccessLog(Session("logon"), ret, 3)
        End If
        SetLists(repid)
        GetSelectedFields()
    End Sub

    Sub DrawSelectedFields()
        Dim rt As String = String.Empty
        Dim lnk As String = ""
        Dim ctlLnk As LinkButton = Nothing
        Dim er As String = String.Empty
        Dim dtf As DataTable = GetSQLFields(repid)
        If dtf Is Nothing OrElse dtf.Rows.Count = 0 Then
            'Make SQLFields from sql statement
            Dim selfields As String = Session("SELECTEDFields")  'fields from the report sql statement
            If selfields <> "" Then
                Dim m As Integer = 0
                Dim k As Integer = 0
                Dim tabl, fild, frname As String
                Dim sctSQL As String = String.Empty
                selfields = FixSelectedFields(Session("REPORTID"), selfields, Session("UserConnString"), Session("UserConnProvider"))
                If Not selfields.ToUpper.Contains("ERROR") Then
                    Dim sqlfield As String() = selfields.Split(",")
                    SQLfields.Rows.Clear()

                    For m = 0 To sqlfield.Length - 1
                        If sqlfield(m).Trim <> "" Then
                            rt = CreateNewRowInHtmlTable(SQLfields)
                            If rt.ToUpper.Contains("ERROR") Then Continue For
                            tabl = sqlfield(m).Trim.Substring(0, sqlfield(m).Trim.LastIndexOf(".")).Trim
                            fild = sqlfield(m).Trim.Substring(sqlfield(m).Trim.LastIndexOf(".") + 1).Trim
                            frname = GlobalFriendlyName(tabl, fild, Session("UnitDB"), er) 'GetFriendlyFieldName(repid, fild)
                            SQLfields.Rows(k + 1).Cells(0).InnerHtml = tabl
                            SQLfields.Rows(k + 1).Cells(1).InnerHtml = fild

                            ctlLnk = New LinkButton
                            ctlLnk.Text = "delete"
                            ctlLnk.ID = "FieldDelete" & "^" & tabl & "." & fild
                            ctlLnk.ToolTip = "Delete Report Field"
                            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                            SQLfields.Rows(k + 1).Cells(2).InnerText = String.Empty
                            SQLfields.Rows(k + 1).Cells(2).Controls.Add(ctlLnk)

                            SQLfields.Rows(k + 1).Cells(3).InnerHtml = frname
                            sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='SELECT' AND Tbl1='" & tabl & "' AND Tbl1Fld1='" & fild & "')"
                            If Not HasRecords(sctSQL) Then
                                sctSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1,Friendly) VALUES('" & Session("REPORTID") & "','SELECT','" & tabl & "','" & fild & "','" & frname & "')"
                                rt = ExequteSQLquery(sctSQL)
                            End If
                            k = k + 1
                        End If
                    Next
                End If
            End If
        ElseIf Not dtf Is Nothing AndAlso dtf.Rows.Count > 0 Then
            Dim t As String = String.Empty
            Dim f As String = String.Empty


            'SQLfields.Rows.Clear()

            For i = 0 To dtf.Rows.Count - 1
                rt = AddRowIntoHTMLtable(dtf.Rows(i), SQLfields)
                If rt.ToUpper.Contains("ERROR") Then Continue For
                t = dtf.Rows(i)("Tbl1").ToString
                t = t.Replace("`", "").Replace("[", "").Replace("]", "")
                f = dtf.Rows(i)("Tbl1Fld1").ToString
                f = f.Replace("`", "").Replace("[", "").Replace("]", "")
                SQLfields.Rows(i + 1).Cells(0).InnerHtml = t
                SQLfields.Rows(i + 1).Cells(1).InnerHtml = f

                ctlLnk = New LinkButton
                ctlLnk.Text = "delete"
                ctlLnk.ID = "FieldDelete" & "^" & t & "." & f
                ctlLnk.ToolTip = "Delete Report Field"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                SQLfields.Rows(i + 1).Cells(2).InnerText = String.Empty
                SQLfields.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)

                SQLfields.Rows(i + 1).Cells(3).InnerHtml = dtf.Rows(i)("Friendly").ToString
            Next
        End If
    End Sub

    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLink As LinkButton = CType(sender, LinkButton)
        Dim id As String = btnLink.ID
        Dim tag As String = Piece(id, "^", 1)
        Dim indx As String = Piece(id, "^", 2)
        Dim link As String = String.Empty

        Select Case tag
            Case "ReportJoinUp"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&upjoin=yes"
            Case "ReportJoinDown"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&downjoin=yes"
            Case "ReportJoinReverse"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&rvrsjoin=yes"
            Case "ReportJoinDelete"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&delsqlj=yes"
            Case "AddJoin"
                link = Piece(id, "^", 2)
            Case "SortUp"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&upsqls=yes"
            Case "SortDown"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&downsqls=yes"
            Case "SortDelete"
                link = "SQLquery.aspx?Report=" & repid & "&indx=" & indx & "&delsqls=yes"

        End Select
        If link <> String.Empty Then
            Response.Redirect(link)
        End If

    End Sub
    Private Sub ShowTextFields(ShowBoth As Boolean)
        divTextBlock.Visible = True
        divCalendarBlock.Visible = False
        If ShowBoth Then
            divText1.Visible = True
            divField1.Visible = False
            divTxtBtn1.Visible = True
            btnTxt1.Text = "Fields"
            divAnd1.Visible = True
            divText2.Visible = True
            divField2.Visible = False
            divTxtBtn2.Visible = True
            btnTxt2.Text = "Fields"
        Else
            divText1.Visible = True
            divField1.Visible = False
            divTxtBtn1.Visible = True
            btnTxt1.Text = "Fields"
            divAnd1.Visible = False
            divText2.Visible = False
            divField2.Visible = False
            divTxtBtn2.Visible = False
            btnTxt2.Text = "Fields"
        End If
    End Sub

    Private Sub ShowDateFields(ShowBoth As Boolean)
        divTextBlock.Visible = False
        divCalendarBlock.Visible = True
        If ShowBoth Then
            divCalendar1.Visible = True
            divField3.Visible = False
            divCalBtn1.Visible = True
            divAnd2.Visible = True
            divcalendar2.Visible = True
            divField4.Visible = False
            divCalBtn2.Visible = True
        Else
            divCalendar1.Visible = True
            divField3.Visible = False
            divCalBtn1.Visible = True
            divAnd2.Visible = False
            divcalendar2.Visible = False
            divField4.Visible = False
            divCalBtn2.Visible = False
        End If
    End Sub
    Private Sub DropDownFieldW1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFieldW1.SelectedIndexChanged
        If DropDownFieldW1.SelectedIndex > 0 Then
            Dim value As String = DropDownFieldW1.Text
            mWhereTable = Piece(value, ".", 1)
            mWhereField = Piece(value, ".", 2)
            mWhereFieldType = GetFieldDataType(mWhereTable, mWhereField, Session("UserConnString"), Session("UserConnProvider"))
            mWhereOperator = DropDownOperator.Text

            If mWhereFieldType.ToUpper.Contains("DATE") OrElse mWhereFieldType.ToUpper.Contains("TIME") Then
                If mWhereOperator.ToUpper <> "BETWEEN" Then
                    ShowDateFields(False)
                Else
                    ShowDateFields(True)
                End If
            Else
                If mWhereOperator.ToUpper <> "BETWEEN" Then
                    ShowTextFields(False)
                Else
                    ShowTextFields(True)
                End If
            End If
            DropDownOperator.Focus()
        End If
    End Sub

    Private Sub DropDownOperator_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownOperator.SelectedIndexChanged
        Dim value As String = DropDownFieldW1.Text.Trim

        If value = "" Then
            DropDownOperator.SelectedIndex = 0
            Return
        End If

        mWhereTable = Piece(value, ".", 1)
        mWhereField = Piece(value, ".", 2)
        mWhereFieldType = GetFieldDataType(mWhereTable, mWhereField, Session("UserConnString"), Session("UserConnProvider"))
        mWhereOperator = DropDownOperator.Text

        If mWhereOperator.ToUpper <> "BETWEEN" Then
            If mWhereFieldType.ToUpper.Contains("DATE") OrElse mWhereFieldType.ToUpper.Contains("TIME") Then
                If lblConditionEntry.Text = "Edit Condition" Then
                    If btnCal1.Text = "Date" Then
                        DropDownFieldW4.Focus()
                    Else
                        ddDate1.Focus()
                    End If
                Else
                    ShowDateFields(False)
                    ddDate1.Focus()
                End If
            Else
                If lblConditionEntry.Text = "Edit Condition" Then
                    If btnTxt1.Text = "Text" Then
                        DropDownFieldW2.Focus()
                    Else
                        TextBoxStatic.Focus()
                    End If
                Else
                    ShowTextFields(False)
                    TextBoxStatic.Focus()
                End If
            End If
        Else
            If mWhereFieldType.ToUpper.Contains("DATE") OrElse mWhereFieldType.ToUpper.Contains("TIME") Then
                If lblConditionEntry.Text = "Edit Condition" Then
                    If btnCal1.Text = "Date" Then
                        DropDownFieldW4.Focus()
                    Else
                        ddDate1.Focus()
                    End If
                Else
                    ShowDateFields(True)
                    ddDate1.Focus()
                End If
            Else
                If lblConditionEntry.Text = "Edit Condition" Then
                    If btnTxt1.Text = "Text" Then
                        DropDownFieldW2.Focus()
                    Else
                        TextBoxStatic.Focus()
                    End If
                Else
                    ShowTextFields(True)
                    TextBoxStatic.Focus()
                End If
            End If

        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        ClearConditionEntry()
        ButtonAddCondition.Text = "Add Condition"
        lblConditionEntry.Text = "New Condition"
        trConditionEntry.Visible = False
        Session("Indx") = ""
    End Sub

    Private Sub ddDate1_DateSelectionChanged(sender As Object, e As EventArgs) Handles ddDate1.DateSelectionChanged
        If divcalendar2.Visible Then
            ddDate2.Focus()
        ElseIf divField4.Visible Then
            DropDownFieldW5.Focus()
        Else
            ButtonAddCondition.Focus()
        End If
    End Sub

    Private Sub ddDate2_DateSelectionChanged(sender As Object, e As EventArgs) Handles ddDate2.DateSelectionChanged
        ButtonAddCondition.Focus()
    End Sub

    Private Sub btnTxt1_Click(sender As Object, e As EventArgs) Handles btnTxt1.Click
        If DropDownFieldW1.SelectedIndex > 0 Then
            If btnTxt1.Text = "Fields" Then
                divText1.Visible = False
                divField1.Visible = True
                DropDownFieldW2.SelectedIndex = 0
                btnTxt1.Text = "Text"
                btnTxt1.ToolTip = "Enter text value"
                DropDownFieldW2.Focus()
            Else
                divText1.Visible = True
                divField1.Visible = False
                DropDownFieldW2.SelectedIndex = 0
                btnTxt1.Text = "Fields"
                btnTxt1.ToolTip = "Choose field "
                TextBoxStatic.Focus()
            End If
        End If

    End Sub
    Private Sub btnTxt2_Click(sender As Object, e As EventArgs) Handles btnTxt2.Click
        If DropDownFieldW1.SelectedIndex > 0 Then
            If btnTxt2.Text = "Fields" Then
                divText2.Visible = False
                divField2.Visible = True
                DropDownFieldW3.SelectedIndex = 0
                btnTxt2.Text = "Text"
                btnTxt2.ToolTip = "Enter text value"
                DropDownFieldW3.Focus()
            Else
                divText2.Visible = True
                divField2.Visible = False
                DropDownFieldW3.SelectedIndex = 0
                btnTxt2.Text = "Fields"
                btnTxt2.ToolTip = "Choose field "
                TextBoxStatic2.Focus()
            End If
        End If

    End Sub
    Private Sub btnCal1_Click(sender As Object, e As EventArgs) Handles btnCal1.Click
        If DropDownFieldW1.SelectedIndex > 0 Then
            If btnCal1.Text = "Fields" Then
                divCalendar1.Visible = False
                divField3.Visible = True
                DropDownFieldW4.SelectedIndex = 0
                btnCal1.Text = "Date"
                btnCal1.ToolTip = "Enter date value"
                DropDownFieldW4.Focus()
            Else
                divCalendar1.Visible = True
                divField3.Visible = False
                DropDownFieldW4.SelectedIndex = 0
                btnCal1.Text = "Fields"
                btnCal1.ToolTip = "Choose field "
                ddDate1.Focus()
            End If
        End If
    End Sub

    Private Sub btnCal2_Click(sender As Object, e As EventArgs) Handles btnCal2.Click
        If DropDownFieldW1.SelectedIndex > 0 Then
            If btnCal2.Text = "Fields" Then
                divcalendar2.Visible = False
                divField4.Visible = True
                DropDownFieldW5.SelectedIndex = 0
                btnCal2.Text = "Date"
                btnCal2.ToolTip = "Enter date value"
                DropDownFieldW5.Focus()
            Else
                divcalendar2.Visible = True
                divField4.Visible = False
                DropDownFieldW5.SelectedIndex = 0
                btnCal2.Text = "Fields"
                btnCal2.ToolTip = "Choose field "
                ddDate2.Focus()
            End If
        End If
    End Sub
    Private Sub TextBoxStatic_TextChanged(sender As Object, e As EventArgs) Handles TextBoxStatic.TextChanged
        If divText2.Visible Then
            TextBoxStatic2.Focus()
        ElseIf divField2.Visible Then
            DropDownFieldW3.Focus()
        Else
            ButtonAddCondition.Focus()
        End If
    End Sub

    Private Sub TextBoxStatic2_TextChanged(sender As Object, e As EventArgs) Handles TextBoxStatic2.TextChanged
        ButtonAddCondition.Focus()
    End Sub

    Private Sub DropDownFieldW2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFieldW2.SelectedIndexChanged
        If DropDownFieldW2.SelectedIndex > 0 Then
            If divText2.Visible Then
                TextBoxStatic2.Focus()
            ElseIf divField2.Visible Then
                DropDownFieldW3.Focus()
            Else
                ButtonAddCondition.Focus()
            End If
        End If
    End Sub

    Private Sub DropDownFieldW3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFieldW3.SelectedIndexChanged
        If DropDownFieldW3.SelectedIndex > 0 Then
            ButtonAddCondition.Focus()
        End If
    End Sub

    Private Sub DropDownFieldW4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFieldW4.SelectedIndexChanged
        If DropDownFieldW4.SelectedIndex > 0 Then
            If divcalendar2.Visible Then
                ddDate2.Focus()
            ElseIf divField4.Visible Then
                DropDownFieldW5.Focus()
            Else
                ButtonAddCondition.Focus()
            End If
        End If
    End Sub

    Private Sub DropDownFieldW5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFieldW5.SelectedIndexChanged
        If DropDownFieldW5.SelectedIndex > 0 Then
            ButtonAddCondition.Focus()
        End If
    End Sub

    Protected Sub ButtonAddSort_Click(sender As Object, e As EventArgs) Handles ButtonAddSort.Click
        'insert
        Dim ret As String = String.Empty
        Dim sorttype As String = DropDownSortType.Text
        Dim insSQL As String = String.Empty
        Dim sctSQL As String = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='ORDER BY' AND Tbl1='" & DropDownTableS1.Text & "' AND Tbl1Fld1='" & DropDownFieldS1.Text & "')"
        If IsCacheDatabase() Then
            sctSQL = "SELECT * FROM OURReportSQLquery WHERE (ReportID='" & Session("REPORTID") & "' AND Doing='ORDER BY' AND UCASE(Tbl1)='" & UCase(DropDownTableS1.Text) & "' AND UCASE(Tbl1Fld1)='" & UCase(DropDownFieldS1.Text) & "' AND Comments='" & sorttype & "' ) "
        End If
        If Not HasRecords(sctSQL) Then
            'Oper will contain the order
            insSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1, Type, Oper) VALUES('" & Session("REPORTID") & "','ORDER BY','" & DropDownTableS1.Text & "','" & DropDownFieldS1.Text & "','" & DropDownSortType.Text & "',0)"
            ret = ExequteSQLquery(insSQL)
        End If
        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Public Function UpJoin(ByVal dt As DataTable, ByVal indx As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim fldord As Integer = 0
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("Indx") = indx Then
                fldord = dt.Rows(i - 1)("RecOrder")
                dt.Rows(i - 1)("RecOrder") = dt.Rows(i)("RecOrder")
                dt.Rows(i)("RecOrder") = fldord
            End If
        Next
        Return exm
    End Function
    Public Function DownJoin(ByVal dt As DataTable, ByVal indx As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim fldord As Integer = 0
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("Indx") = indx Then
                fldord = dt.Rows(i + 1)("RecOrder")
                dt.Rows(i + 1)("RecOrder") = dt.Rows(i)("RecOrder")
                dt.Rows(i)("RecOrder") = fldord
            End If
        Next
        Return exm
    End Function
    Public Function ReverseJoin(ByVal dt As DataTable, ByVal indx As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim tb, fl As String
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("Indx") = indx Then
                tb = dt.Rows(i)("Tbl1").ToString
                fl = dt.Rows(i)("Tbl1Fld1").ToString
                dt.Rows(i)("Tbl1") = dt.Rows(i)("Tbl2").ToString
                dt.Rows(i)("Tbl1Fld1") = dt.Rows(i)("Tbl2Fld2").ToString
                dt.Rows(i)("Tbl2") = tb
                dt.Rows(i)("Tbl2Fld2") = fl
            End If
        Next
        Return exm
    End Function

    Public Function UpSortField(ByVal dt As DataTable, ByVal indx As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim fldord As Integer = 0
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("Indx") = indx Then
                fldord = dt.Rows(i - 1)("Oper")
                dt.Rows(i - 1)("Oper") = dt.Rows(i)("Oper")
                dt.Rows(i)("Oper") = fldord
            End If
        Next
        Return exm
    End Function
    Public Function DownSortField(ByVal dt As DataTable, ByVal indx As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim fldord As Integer = 0
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("Indx") = indx Then
                fldord = dt.Rows(i + 1)("Oper")
                dt.Rows(i + 1)("Oper") = dt.Rows(i)("Oper")
                dt.Rows(i)("Oper") = fldord
            End If
        Next
        Return exm
    End Function
    Protected Function CorrectSortOrder(ByVal dt As DataTable) As DataTable
        Dim dtt As DataTable = dt
        'reorder
        For i = 0 To dtt.Rows.Count - 1
            dtt.Rows(i)("Oper") = i + 1
        Next
        Return dtt
    End Function
    Protected Sub CheckBoxAllTables1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllTables1.CheckedChanged
        Dim ddt As DataTable
        'list of tables on "join" tab include all user tables
        DropDownTableJ1.Items.Clear()
        Dim err As String = String.Empty
        If CheckBoxAllTables1.Checked Then
            ddt = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV")).Table
        Else
            ddt = GetReportTables(repid)
        End If
        If err <> "" Then
            LabelAlert1.Text = err
            Exit Sub
        End If
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownTableJ1.Items.Add(ddt.Rows(i)("TABLE_NAME"))
        Next
        If DropDownTableJ1.Items.Count > 0 Then DropDownTableJ1.Items(0).Selected = True
        'fields
        DropDownFieldJ1.Items.Clear()
        ddt = GetListOfTableColumns(DropDownTableJ1.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownFieldJ1.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
            DropDownFieldJ1.Items(i).Selected = False
        Next
    End Sub
    Protected Sub CheckBoxAllTables2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxAllTables2.CheckedChanged
        Dim ddt As DataTable
        'list of tables on "join" tab include all user tables
        DropDownTableJ2.Items.Clear()
        Dim err As String = String.Empty
        If CheckBoxAllTables2.Checked Then
            ddt = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV")).Table
        Else
            ddt = GetReportTables(repid)
        End If
        If err <> "" Then
            LabelAlert1.Text = err
            Exit Sub
        End If
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownTableJ2.Items.Add(ddt.Rows(i)("TABLE_NAME"))
        Next
        If DropDownTableJ2.Items.Count > 0 Then DropDownTableJ2.Items(0).Selected = True
        'fields
        DropDownFieldJ2.Items.Clear()
        ddt = GetListOfTableColumns(DropDownTableJ2.SelectedItem.Text, Session("UserConnString"), Session("UserConnProvider")).Table
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            DropDownFieldJ2.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
            DropDownFieldJ2.Items(i).Selected = False
        Next
    End Sub

    Private Sub DropDownJoinType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownJoinType.SelectedIndexChanged
        dt = GetReportJoins(repid) 'sorted by RecOrder,Tbl1,Tbl2,Tbl1Fld1,Tbl2Fld2
        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i)("comments") = DropDownJoinType.Text
            Next
        End If
        SaveJoins(repid, dt)
        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Private Sub btnCustomizeLogic_Click(sender As Object, e As EventArgs) Handles btnCustomizeLogic.Click
        dlgCustomizeLogic.ConditionCount = CInt(Session("ConditionCount"))
        dlgCustomizeLogic.GroupCount = CInt(Session("ConditionGroupCount"))
        dlgCustomizeLogic.Show("Customize Logic", Session("ConditionTreeView"), "Customize")
    End Sub
    Public Sub SaveLogicData(tvLogical As TreeView, GroupsToDelete As DeletedGroups)
        Dim tn, tnChild As TreeNode
        Dim cond As ReportCondition
        Dim sql, ret As String

        Dim sFields As String = String.Empty
        Dim sValues As String = String.Empty

        For i As Integer = 0 To tvLogical.Nodes.Count - 1
            tn = tvLogical.Nodes(i)
            cond = tn.ConditionData
            If cond IsNot Nothing Then
                If cond.GroupName <> String.Empty Then
                    If cond.ConditionId = "new" Then
                        sFields = "ReportID,Doing," & FixReservedWords("Group") & ",Logical,RecOrder"
                        sValues = "'" & Session("REPORTID") & "',"
                        sValues &= "'GROUP',"
                        sValues &= "'" & cond.GroupName & "',"
                        sValues &= "'" & cond.LogicalOperator & "',"
                        sValues &= cond.RecordOrder
                        sql = "INSERT INTO OURReportSQLquery  (" & sFields & ") VALUES (" & sValues & ")"

                        'sql = "INSERT INTO OURReportSQLquery  "
                        'sql &= "SET ReportID = '" & Session("REPORTID") & "',"
                        'sql &= "Doing = 'GROUP',"
                        'sql &= FixReservedWords("Group") & " = '" & cond.GroupName & "',"
                        'sql &= "Logical = '" & cond.LogicalOperator & "',"
                        'sql &= "RecOrder = " & cond.RecordOrder
                    Else
                        sql = "UPDATE OURReportSQLquery  "
                        sql &= "SET " & FixReservedWords("Group") & " = '" & cond.GroupName & "',"
                        sql &= "Logical = '" & cond.LogicalOperator & "',"
                        sql &= "RecOrder = " & cond.RecordOrder
                        sql &= " Where Indx = " & cond.ConditionId
                    End If
                    ExequteSQLquery(sql)
                    For j As Integer = 0 To tn.ChildNodes.Count - 1
                        tnChild = tn.ChildNodes(j)
                        cond = tnChild.ConditionData
                        If cond IsNot Nothing Then
                            sql = "UPDATE OURReportSQLquery  "
                            sql &= "SET Logical = '" & cond.LogicalOperator & "',"
                            If cond.ContainedBy <> String.Empty Then
                                sql &= FixReservedWords("Group") & " = '" & cond.ContainedBy & "',"
                            Else
                                sql &= FixReservedWords("Group") & " = NULL,"
                            End If
                            sql &= "RecOrder = " & cond.RecordOrder
                            sql &= " Where Indx = " & cond.ConditionId
                            ExequteSQLquery(sql)
                        End If
                    Next
                Else
                    sql = "UPDATE OURReportSQLquery  "
                    sql &= "SET Logical = '" & cond.LogicalOperator & "',"
                    If cond.ContainedBy <> String.Empty Then
                        sql &= FixReservedWords("Group") & " = '" & cond.ContainedBy & "',"
                    Else
                        sql &= FixReservedWords("Group") & " = NULL,"
                    End If
                    sql &= "RecOrder = " & cond.RecordOrder
                    sql &= " Where Indx = " & cond.ConditionId
                    ret = ExequteSQLquery(sql)
                End If
            End If
        Next
        If GroupsToDelete IsNot Nothing Then
            For i As Integer = 0 To GroupsToDelete.Groups.Count - 1
                cond = GroupsToDelete.Groups(i)
                If cond.ConditionId IsNot Nothing AndAlso cond.ConditionId <> "new" Then
                    sql = "DELETE FROM OURReportSQLquery Where Indx = " & cond.ConditionId
                    ExequteSQLquery(sql)
                End If
            Next
        End If
    End Sub
    Private Sub dlgCustomizeLogic_CustomLogicDialogResulted(sender As Object, e As Controls_CustomizeLogicDlg.CustomLogicDlgEventArgs) Handles dlgCustomizeLogic.CustomLogicDialogResulted
        Dim json As String = e.JsonData
        Dim DelGroups As DeletedGroups = Nothing
        Dim tv As TreeView = JsonConvert.DeserializeObject(Of TreeView)(json)
        If e.DeletedData <> String.Empty Then
            DelGroups = JsonConvert.DeserializeObject(Of DeletedGroups)(e.DeletedData)
        End If
        SaveLogicData(tv, DelGroups)
        Response.Redirect("SQLquery.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Sub DropDownTableS1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTableS1.SelectedIndexChanged
        'sorting
        Dim tbl As String = String.Empty
        DropDownFieldS1.Items.Clear()
        tbl = DropDownTableS1.SelectedItem.Text.Replace("`", "")
        tbl = tbl.Replace("[", "").Replace("]", "").Trim
        Dim ddt As DataTable = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
        For ii As Integer = 0 To ddt.Rows.Count - 1
            DropDownFieldS1.Items.Add(ddt.Rows(ii)("COLUMN_NAME"))
        Next
    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = "~/" & node.Value
        Dim urlBack As String = HttpContext.Current.Request.Url.ToString
        Dim nodeText As String = node.Text

        If nodeText.Contains("Advanced Report Designer") Then
            Session("urlback") = urlBack
        Else
            Session("urlback") = Nothing
        End If

        Response.Redirect(url)
    End Sub
    Private Sub DropDownColumns_ChecklistChanged(sender As Object, e As Controls_uc1.ChecklistChangedArgs) Handles DropDownColumns.ChecklistChanged
        If e.AllChecked Then
            btnSelectAll.Enabled = False
            btnSelectAll.CssClass = "DataButtonDisabled"
            btnUnselectAll.Enabled = True
            btnUnselectAll.CssClass = "DataButtonEnabled"
        ElseIf e.NoneChecked Then
            btnSelectAll.Enabled = True
            btnSelectAll.CssClass = "DataButtonEnabled"
            btnUnselectAll.Enabled = False
            btnUnselectAll.CssClass = "DataButtonDisabled"
        Else
            btnSelectAll.Enabled = True
            btnSelectAll.CssClass = "DataButtonEnabled"
            btnUnselectAll.Enabled = True
            btnUnselectAll.CssClass = "DataButtonEnabled"
        End If
    End Sub
    Private Function TableInReportQuery(tbl As String) As Boolean
        Dim n As Integer = mReportTables.Count
        For i As Integer = 0 To n - 1
            If mReportTables.Item(i).Value.ToUpper = tbl.ToUpper Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Sub LoadTables(sFoundTables As String)
        Dim sSearch As String = txtSearch.Value
        Dim arTables As String() = hdnAllTables.Value.Split(",")
        Dim n As Integer = arTables.Length
        Dim ar As String()
        Dim li As ListItem
        Dim nSelect As Integer = 0

        lblTableAlert.Text = " "
        If sFoundTables <> String.Empty Then
            If sFoundTables = "Not Found" Then
                lblTableAlert.Text = "There are no matching tables found for " & """" & sSearch & """" & "!"
            ElseIf sFoundTables = "All" Then
                DropDownTables.Items.Clear()
                For i As Integer = 0 To n - 1
                    ar = arTables(i).Split("~")
                    li = New ListItem(ar(0), ar(1))
                    DropDownTables.Items.Add(li)
                    If TableInReportQuery(li.Value) Then
                        nSelect = i
                    End If
                Next
                DropDownTables.ClearSelection()
                DropDownTables.Items(nSelect).Selected = True
                DropDownTables_SelectedIndexChanged(DropDownTables, Nothing)
            Else
                Dim arFound As String() = sFoundTables.Split(",")
                Dim nFound As Integer = arFound.Count

                DropDownTables.Items.Clear()
                For i As Integer = 0 To nFound - 1
                    ar = arFound(i).Split("~")
                    li = New ListItem(ar(0), ar(1))
                    DropDownTables.Items.Add(li)
                    If TableInReportQuery(li.Value) Then
                        nSelect = i
                    End If
                Next
                DropDownTables.ClearSelection()
                DropDownTables.Items(nSelect).Selected = True
                DropDownTables_SelectedIndexChanged(DropDownTables, Nothing)
            End If
        End If
    End Sub
    Private Sub imgSearch_Click(sender As Object, e As ImageClickEventArgs) Handles imgSearch.Click
        Dim sSearch As String = txtSearch.Value
        Dim sAllTables As String = hdnAllTables.Value
        Dim nTables As Integer = DropDownTables.Items.Count

        If sAllTables <> String.Empty Then
            Dim arTables As String() = sAllTables.Split(",")
            Dim n As Integer = arTables.Length
            Dim ar As String()
            Dim li As ListItem
            Dim nMatches As Integer = 0
            Dim nSelect As Integer = 0

            If sSearch <> String.Empty Then
                If sAllTables.ToUpper.Contains(sSearch.ToUpper) Then
                    DropDownTables.Items.Clear()
                    For i As Integer = 0 To n - 1
                        If arTables(i).ToUpper.Contains(sSearch.ToUpper) Then
                            ar = arTables(i).Split("~")
                            li = New ListItem(ar(0), ar(1))
                            DropDownTables.Items.Add(li)
                            If TableInReportQuery(li.Value) Then
                                nSelect = nMatches
                            End If
                            nMatches = nMatches + 1
                        End If
                    Next
                    If nMatches > 0 Then
                        lblTableAlert.Text = " "
                        DropDownTables.Items(nSelect).Selected = True
                        DropDownTables_SelectedIndexChanged(DropDownTables, Nothing)
                    End If
                Else
                    lblTableAlert.Text = "There are no matching tables for " & """" & sSearch & """" & "!"
                End If
            ElseIf n <> nTables Then
                ' restore all tables
                DropDownTables.Items.Clear()
                For i As Integer = 0 To n - 1
                    ar = arTables(i).Split("~")
                    li = New ListItem(ar(0), ar(1))
                    DropDownTables.Items.Add(li)
                    If TableInReportQuery(li.Value) Then
                        nSelect = i
                    End If
                Next
                DropDownTables.Items(nSelect).Selected = True
                DropDownTables_SelectedIndexChanged(DropDownTables, Nothing)
            End If
        End If
    End Sub
End Class
