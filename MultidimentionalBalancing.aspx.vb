Imports System.Data
Imports System.Drawing
Imports System.Math
Partial Class MultidimentionalBalancing
    Inherits System.Web.UI.Page
    Public dv3 As DataView

    Private Sub MultidimentionalBalancing_Init(sender As Object, e As EventArgs) Handles Me.Init
        btnBalanceFlds2a.OnClientClick = "showSpinner();"
        btnBalanceMatrixColumns3b.OnClientClick = "showSpinner();"
        btnBalanceMatrixSumsRowsCols1b.OnClientClick = "showSpinner();"
        btnBalanceSumsRowsCols1a.OnClientClick = "showSpinner();"
        btnBalanceVals2b.OnClientClick = "showSpinner();"
        btnGetCoeffsByFields3a.OnClientClick = "showSpinner();"
        btnGetCoeffsByVals2c.OnClientClick = "showSpinner();"
        btnGetCoeffsMatrixColumns3c.OnClientClick = "showSpinner();"

        lblHeader.Text = Session("REPTITLE") & " - Multidimensional Balancing"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Matrix_Balancing"
        repid = Session("REPORTID")
        Session("MatrixChart") = ""

    End Sub
    Private Sub MultidimentionalBalancing_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        'DropDownList1 - field values as rows
        'DropDownList2 - field values columns
        'DropDownList3 - field1
        'DropDownList4 - field1 aggregation function
        'DropDownList5 - field2
        'DropDownList8 - field2 aggregation function
        'DropDownList6 - starting value of field2
        'DropDownList7 - target value of field2
        'DropDownColumns - multiple columns
        'DropDownColumnsDim - multiple columns for balancing
        'DropDownList10 - multiple columns aggregation function
        'TextBoxSumsByRows
        'TextBoxSumsByCols
        Dim ret As String = String.Empty
        'no partial balancing yet (sv=0)
        TextBoxUV.Text = "0,0"
        TextBoxUV.Visible = False
        TextBoxUV.Enabled = False
        Label16.Visible = False
        If Request("export") = "GridData" Then
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("FileGridViewdata"))
                Response.TransmitFile(Session("FileGridViewdata"))
            Catch ex As Exception
                LabelError.Text = "ERROR!! " & ex.Message
                LabelError.Visible = True
                LabelError.Enabled = True
            End Try
            Response.End()
        End If
        If Not IsPostBack Then
            DropDownListScenarios.SelectedIndex = 1
            DropDownListScenarios.SelectedIndex = 0
            Session("GridView1DataSource") = Nothing
            Session("GridView2DataSource") = Nothing
            Session("GridView3DataSource") = Nothing
            Session("GridView4DataSource") = Nothing
            Session("GridView5DataSource") = Nothing
            Session("GridView6DataSource") = Nothing
            Label2.Text = ""
            LabelResult.Text = ""
            LabelError.Text = ""
            LabelCompare.Text = ""
            mainGrids.Visible = False
            Label8.Visible = False
            TextBoxNumberSteps.Visible = False
            Label9.Visible = False
            TextBoxPrecision.Visible = False
            Label16.Visible = False
            TextBoxUV.Visible = False
            chkAdjustByStart.Visible = False
            tr1a.Visible = True
            tr1b.Visible = True
            tr2a.Visible = True
            tr2b.Visible = True
            tr2c.Visible = True
            tr3a.Visible = True
            tr3b.Visible = True
            tr3c.Visible = True
            tr1aBtn.Visible = False
            tr1bBtn.Visible = False
            tr2aBtn.Visible = False
            tr2bBtn.Visible = False
            tr2cBtn.Visible = False
            tr3aBtn.Visible = False
            tr3bBtn.Visible = False
            tr3cBtn.Visible = False
            trRowsCols.Visible = False
            trField1.Visible = False
            trSumsByRows.Visible = False
            trSumsByCols.Visible = False
            trField2.Visible = False
            'tr1MultiCols.Visible = False
            divMultiCols.Style("display") = "none"
            DropDownList1.Enabled = False
            DropDownList2.Enabled = False
            DropDownList3.Enabled = False
            DropDownList4.Enabled = False
            DropDownList5.Enabled = False
            DropDownList6.Enabled = False
            DropDownList7.Enabled = False
            DropDownList8.Enabled = False
            DropDownList10.Enabled = False
            TextBoxSumsByCols.Enabled = False
            TextBoxSumsByRows.Enabled = False
            CheckBoxSelectAllFields.Enabled = False
            CheckBoxUnselectAllFields.Enabled = False
            DropDownColumns.DropDownBackColor = Color.Gray
            DropDownColumns.BorderColor = Color.Gray
            DropDownColumns.TextBoxBackColor = Color.Gray
            DropDownColumns.ForeColor = Color.Gray
            HyperLinkChart.Text = ""
        Else
            mainGrids.Visible = True
            Label8.Visible = True
            TextBoxNumberSteps.Visible = True
            Label9.Visible = True
            TextBoxPrecision.Visible = True
            'Label16.Visible = True
            'TextBoxUV.Visible = True
            chkAdjustByStart.Visible = True
        End If

        Try
            GridView2.Visible = True
            GridView3.Visible = True
            GridView5.Visible = True
            GridView6.Visible = True
            Label13.Text = "Target Matrix"
            Label14.Text = "Balancing Matrix"
            Label19.Text = "=>"
            Label15.Text = "Balancing of Whole Matrix"
            Label20.Text = "Balancing coefficients for Whole Matrix"
            Dim u As Integer = 0
            Dim v As Integer = 0
            If TextBoxUV.Text.Trim <> "" Then
                u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
                v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
            End If
            If u > 0 AndAlso v > 0 Then
                trbal1.Visible = True
                trbal2.Visible = True
                trbal3.Visible = True
                trbal4.Visible = True
            Else
                trbal1.Visible = False
                trbal2.Visible = False
                trbal3.Visible = False
                trbal4.Visible = False
            End If

            Dim sqls As String = String.Empty
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim ctlLnk As LinkButton = Nothing
            Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & Session("REPORTID") & "' AND Type='RPT'"
            If Session("Crystal") <> "ok" OrElse Not HasRecords(qsql) Then
                Dim treenodecr As WebControls.TreeNode
                treenodecr = TreeView1.FindNode("ShowReport.aspx?srd=3")
                If treenodecr IsNot Nothing AndAlso treenodecr.ChildNodes.Count = 6 Then
                    treenodecr.ChildNodes(5).NavigateUrl = ""
                    treenodecr.ChildNodes(5).Text = "See report data"
                    treenodecr.ChildNodes(5).Value = Nothing
                    treenodecr.ChildNodes.Remove(treenodecr.ChildNodes(5))
                End If
            End If
            If Session("admin") <> "admin" AndAlso Session("admin") <> "super" Then
                Dim node As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=2")
                Dim idx As Integer
                If node IsNot Nothing Then
                    idx = TreeView1.Nodes.IndexOf(node)
                    If idx > -1 Then
                        For i = 0 To 2
                            TreeView1.Nodes.RemoveAt(idx)
                        Next
                    End If
                End If
            ElseIf Session("OurConnProvider").ToString.Trim = "Sqlite" Then
                Dim node As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=2")
                Dim idx As Integer
                If node IsNot Nothing Then
                    idx = TreeView1.Nodes.IndexOf(node)
                    If idx > -1 Then
                        For i = 0 To 2
                            TreeView1.Nodes.RemoveAt(idx)
                        Next
                    End If
                End If
            End If
            'Report Info (title, data for report)
            Dim dv1 As DataView = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
            If dv1.Table.Rows(0)("ReportType").ToString = "" Then
                dv1.Table.Rows(0)("ReportType") = "rdl"
            End If
            Session("noedit") = dv1.Table.Rows(0)("Param7type").ToString
            If Session("noedit") = "standard" Then
                'hide all links to report editing pages leaving only reporting
                Dim treenodecr1 As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=2")
                If Not treenodecr1 Is Nothing Then
                    treenodecr1.Value = "ReportViews.aspx"
                    treenodecr1.ToolTip = "disabled for locked reports"

                End If
                Dim treenodecr2 As WebControls.TreeNode = TreeView1.FindNode("SQLquery.aspx?tnq=0")
                If Not treenodecr2 Is Nothing Then
                    treenodecr2.Value = "ReportViews.aspx"
                    treenodecr2.ToolTip = "disabled for locked reports"

                End If
                Dim treenodecr3 As WebControls.TreeNode = TreeView1.FindNode("RDLformat.aspx?tnf=0")
                If Not treenodecr3 Is Nothing Then
                    treenodecr3.Value = "ReportViews.aspx"
                    treenodecr3.ToolTip = "disabled for locked reports"

                End If
            End If
            dv3 = Session("dv3")
            dv3.RowFilter = ""
            'dropdowns
            Dim cat1, cat2, fld, aggrf As String
            Dim dt As DataTable = dv3.Table

            If Not IsPostBack Then
                If Request("x1") IsNot Nothing Then
                    cat1 = Request("x1")
                    Session("Cat1") = cat1
                End If
                If Request("x2") IsNot Nothing Then
                    cat2 = Request("x2")
                    Session("Cat2") = cat2
                End If
                If Request("y1") IsNot Nothing Then
                    fld = Request("y1")
                    Session("AxisY") = fld
                End If
                If Request("fn") IsNot Nothing Then
                    aggrf = Request("fn")
                    Session("Aggregate") = aggrf
                End If
                If Request("itt") IsNot Nothing Then
                    Session("Itt") = Request("itt").ToString
                End If
                If Request("fnitt") IsNot Nothing Then
                    Session("AggregateF2") = Request("fnitt").ToString
                End If
                'lblGroups.Text = "Matrix by: " '& cat1.ToString & ", " & cat2.ToString

                If Session("Aggregate") Is Nothing Then
                    DropDownList4.Items.Clear()
                    DropDownList4.Items.Add("Count")
                    DropDownList4.Items.Add("CountDistinct")
                End If

                If Session("AggregateF2") Is Nothing Then
                    DropDownList8.Items.Clear()
                    DropDownList8.Items.Add("Count")
                    DropDownList8.Items.Add("CountDistinct")
                End If

                If Session("AggregateF3") Is Nothing Then
                    DropDownList10.Items.Clear()
                    DropDownList10.Items.Add("Count")
                    DropDownList10.Items.Add("CountDistinct")
                End If

                Dim fldname As String = String.Empty
                Dim frdname As String = String.Empty
                Dim fldtype As String = String.Empty
                DropDownList1.Items.Clear()
                DropDownList1.Items.Add(" ")
                For i = 0 To dt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = dt.Columns(i).Caption
                    fldtype = dt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownList1.Items.Add(li)
                Next
                If Not Session("Cat1") Is Nothing Then
                    DropDownList1.SelectedValue = Session("Cat1").ToString
                    'DropDownList1_SelectedIndexChanged(sender, e)
                End If
                DropDownList2.Items.Clear()
                DropDownList2.Items.Add(" ")
                For i = 0 To dt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = dt.Columns(i).Caption
                    fldtype = dt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownList2.Items.Add(li)
                Next
                If Not Session("Cat2") Is Nothing Then
                    DropDownList2.SelectedValue = Session("Cat2").ToString
                    'DropDownList2_SelectedIndexChanged(sender, e)
                End If
                DropDownList3.Items.Clear()
                DropDownList3.Items.Add(" ")
                For i = 0 To dt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = dt.Columns(i).Caption
                    fldtype = dt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownList3.Items.Add(li)
                Next
                If Not Session("AxisY") Is Nothing Then
                    DropDownList3.SelectedValue = Session("AxisY").ToString
                    DropDownList3_SelectedIndexChanged(sender, e)
                End If
                DropDownList6.Items.Clear()
                DropDownList7.Items.Clear()
                DropDownList5.Items.Clear()
                DropDownList5.Items.Add(" ")

                For i = 0 To dt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = dt.Columns(i).Caption
                    fldtype = dt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownList5.Items.Add(li)
                Next
                If Not Session("Itt") Is Nothing AndAlso Session("Itt").ToString.Trim <> "" Then
                    DropDownList5.SelectedValue = Session("Itt").ToString
                    DropDownList5_SelectedIndexChanged(sender, e)
                ElseIf Not IsPostBack Then
                    DropDownList5.SelectedValue = " "
                End If

                DropDownColumns.Items.Clear()
                For i = 0 To dt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = dt.Columns(i).Caption
                    fldtype = dt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownColumns.Items.Add(li)
                Next

                DropDownColumnsDim.Items.Clear()
                For i = 0 To dt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = dt.Columns(i).Caption
                    fldtype = dt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownColumnsDim.Items.Add(li)
                Next

            Else  'is postback


            End If

            'Session("WhereStm") = srch.Trim & " AND " & Session("WhereText").ToString.Trim
            LabelMessage.Text = Session("WhereStm").ToString.Trim
            If Not IsPostBack Then
                DropDownListScenarios.SelectedValue = " "
                If Request("sel") = "4a" Then
                    DropDownListScenarios.SelectedValue = "4a"
                ElseIf Request("sel") = "4b" Then
                    DropDownListScenarios.SelectedValue = "4b"
                ElseIf Request("sel") = "4c" Then
                    DropDownListScenarios.SelectedValue = "4c"
                End If
                DropDownListScenarios_SelectedIndexChanged(sender, e)
            End If
            If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
                chkAdjustByStart.Checked = True
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
    End Sub
    Protected Sub CheckBoxSelectAllFields_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSelectAllFields.CheckedChanged
        Dim j As Integer
        Session("SELECTEDFields") = ""
        If CheckBoxSelectAllFields.Checked Then
            CheckBoxUnselectAllFields.Checked = False
            For j = 0 To DropDownColumns.Items.Count - 1   'all fields in drop-down selected
                DropDownColumns.Items(j).Selected = True
                Session("SELECTEDFields") = Session("SELECTEDFields") & "," & DropDownColumns.Items(j).Text
            Next
        End If
        DropDownColumns.Text = Session("SELECTEDFields")
        'Dim ret As String = DropDownList10Calc()
    End Sub
    Protected Sub CheckBoxUnselectAllFields_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxUnselectAllFields.CheckedChanged
        Dim j As Integer
        Session("SELECTEDFields") = ""
        If CheckBoxUnselectAllFields.Checked Then
            CheckBoxSelectAllFields.Checked = False
            For j = 0 To DropDownColumns.Items.Count - 1   'draw drop-down start
                DropDownColumns.Items(j).Selected = False
            Next
        End If
        DropDownColumns.Text = Session("SELECTEDFields")
        'Dim ret As String = DropDownList10Calc()
    End Sub

    Private Function DropDownList10Calc() As String
        Dim ret As String = "Numeric"
        'depended of type
        DropDownList10.Items.Clear()
        DropDownList10.Items.Add("Count")
        DropDownList10.Items.Add("CountDistinct")
        Dim bNumeric As Boolean = True
        Dim i As Integer
        For i = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(i).Selected Then
                If Not ColumnTypeIsNumeric(dv3.Table.Columns(DropDownColumns.Items(i).Text)) Then
                    bNumeric = False
                    ret = "Text"
                    Exit For
                End If
            End If
        Next
        If bNumeric Then
            DropDownList10.Items.Add("Sum")
            DropDownList10.Items.Add("Max")
            DropDownList10.Items.Add("Min")
            DropDownList10.Items.Add("Avg")
            DropDownList10.Items.Add("StDev")
            DropDownList10.Items.Add("Value")
        End If
        Return ret
    End Function
    Private Sub DropDownList3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList3.SelectedIndexChanged
        'depended of type
        DropDownList4.Items.Clear()
        DropDownList4.Items.Add("Count")
        DropDownList4.Items.Add("CountDistinct")
        If ColumnTypeIsNumeric(dv3.Table.Columns(DropDownList3.SelectedValue)) Then
            DropDownList4.Items.Add("Sum")
            DropDownList4.Items.Add("Max")
            DropDownList4.Items.Add("Min")
            DropDownList4.Items.Add("Avg")
            DropDownList4.Items.Add("StDev")
            DropDownList4.Items.Add("Value")
        End If
        Session("AxisY") = DropDownList3.SelectedValue
        If Not Session("Aggregate") Is Nothing Then
            Try
                DropDownList4.SelectedValue = Session("Aggregate")
            Catch ex As Exception

            End Try
        End If
        Session("nv") = DropDownList4.Items.Count
    End Sub

    Private Sub DropDownList4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList4.SelectedIndexChanged
        Session("Aggregate") = DropDownList4.SelectedItem.Text
    End Sub

    Private Sub DropDownList5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList5.SelectedIndexChanged
        'fill out DropDownList8 depended of type
        DropDownList8.Items.Clear()
        DropDownList8.Items.Add("Count")
        DropDownList8.Items.Add("CountDistinct")
        If ColumnTypeIsNumeric(dv3.Table.Columns(DropDownList5.SelectedValue)) Then
            DropDownList8.Items.Add("Sum")
            DropDownList8.Items.Add("Max")
            DropDownList8.Items.Add("Min")
            DropDownList8.Items.Add("Avg")
            DropDownList8.Items.Add("StDev")
            DropDownList8.Items.Add("Value")
        End If
        Session("AxisYF2") = DropDownList3.SelectedValue
        If Not Session("AggregateF2") Is Nothing Then
            Try
                DropDownList8.SelectedValue = Session("AggregateF2")
            Catch ex As Exception

            End Try
        End If
        Session("nvf2") = DropDownList8.Items.Count

        'fill out DropDownList6 and DropDownList7 from dv3
        DropDownList6.Items.Clear()
        DropDownList7.Items.Clear()
        Dim fld As String = DropDownList5.SelectedValue
        Session("Itt") = fld
        dv3 = Session("dv3")
        dv3.Sort = fld & " ASC"
        Dim dt As DataTable = dv3.ToTable(True, fld)
        For i = 0 To dt.Rows.Count - 1
            'Dim li As ListItem = New ListItem
            'li.Text = frdname
            'li.Value = fldname
            If IsNumeric(dt.Rows(i)(0).ToString) Then
                DropDownList6.Items.Add(ExponentToNumber(dt.Rows(i)(0).ToString))
            Else
                DropDownList6.Items.Add(dt.Rows(i)(0).ToString)
            End If
        Next
        dv3.Sort = fld & " DESC"
        dt = dv3.ToTable(True, fld)
        For i = 0 To dt.Rows.Count - 1
            'Dim li As ListItem = New ListItem
            'li.Text = frdname
            'li.Value = fldname
            If IsNumeric(dt.Rows(i)(0).ToString) Then
                DropDownList7.Items.Add(ExponentToNumber(dt.Rows(i)(0).ToString))
            Else
                DropDownList7.Items.Add(dt.Rows(i)(0).ToString)
            End If

        Next
    End Sub

    Private Sub btnBalanceVals2b_Click(sender As Object, e As EventArgs) 'Handles btnBalanceVals2b.Click
        '(2b) Balancing matrix of field1 between starting and target values of the field2
        'DropDownList1 - field values as rows
        'DropDownList2 - field values columns
        'DropDownList3 - field1
        'DropDownList4 - field1 aggregation function
        'DropDownList5 - field2
        'DropDownList6 - starting value of field2
        'DropDownList7 - target value of field2

        lnkExportGrid2.Visible = True
        lnkExportGrid2.Enabled = True
        lnkExportGrid3.Visible = True
        lnkExportGrid3.Enabled = True
        lnkExportGrid5.Visible = True
        lnkExportGrid5.Enabled = True
        lnkExportGrid6.Visible = True
        lnkExportGrid6.Enabled = True
        mainGrids.Visible = True
        LabelError.Text = ""
        LabelResult.Text = ""
        If DropDownList1.Text.Trim = "" OrElse DropDownList2.Text.Trim = "" OrElse DropDownList3.Text.Trim = "" OrElse DropDownList5.Text.Trim = "" Then
            LabelError.Text = "Matrix fields must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString
        Dim x2 As String = DropDownList2.SelectedValue.ToString
        Dim fn As String = DropDownList4.SelectedValue.ToString
        Dim y1 As String = DropDownList3.SelectedValue.ToString
        Dim fld As String = DropDownList5.SelectedValue.ToString
        Dim sval As String = DropDownList6.SelectedValue.ToString
        Dim tval As String = DropDownList7.SelectedValue.ToString
        Dim fnf2 As String = DropDownList4.SelectedValue.ToString  'no sense to have different ones
        Label12.Text = "Starting Matrix of " & fn & " of " & y1 & " where " & fld & "='" & sval & "'"
        Label13.Text = "Target Matrix of " & fnf2 & " of " & y1 & " where " & fld & "='" & tval & "'"
        Label14.Text = "Balancing Matrix of " & fnf2 & " of " & y1
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        'starting matrix
        Dim dstart As DataTable
        dstart = ComputeStats(dv3.Table, fn, y1, x1, x2, fld & "='" & sval & "'", ret, Session("UserConnString"), Session("UserConnProvider"))
        dstart.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
        dstart = dstart.DefaultView.ToTable
        Dim dtarget As DataTable
        'dv3.RowFilter = fld & "='" & sval & "'"
        dtarget = ComputeStats(dv3.Table, fnf2, y1, x1, x2, fld & "='" & tval & "'", ret, Session("UserConnString"), Session("UserConnProvider"))
        dtarget.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
        dtarget = dtarget.DefaultView.ToTable
        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        ret = MakeRowsColumnsArraysFromDataTables(dstart, dtarget, x1, x2, "ARR", ax1vals, ax2vals)
        Dim a(,) As Double = MakeMatrixFromDataTable(dstart, x1, x2, "ARR", ax1vals, ax2vals)
        Dim b(,) As Double = MakeMatrixFromDataTable(dtarget, x1, x2, "ARR", ax1vals, ax2vals)

        Dim sma As Decimal = 0  'overall sum for target matrix
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                sma = sma + a(i, j)
            Next
        Next
        Dim smb As Decimal = 0  'overall sum for target matrix
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                smb = smb + b(i, j)
            Next
        Next

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
        'Dim xi(ax1vals.Length - 1) As Decimal
        'Dim xj(ax2vals.Length - 1) As Decimal
        Dim dtk As New DataTable

        'balancing
        ret = AdjustMatrixByOverallSum(b, sma)
        ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, x, dtk, u, v)

        'adjust to original target
        If chkAdjustByStart.Checked = False Then  'adjust by target sums
            ret = ret & AdjustMatrixByOverallSum(x, smb)
        End If
        ret = ret & AdjustMatrixByOverallSum(b, smb)
        Dim mx As Decimal = 0  'cell diff
        If u > 0 AndAlso v > 0 Then
            For i = 0 To u - 1
                For j = 0 To v - 1
                    mx = Max(mx, Abs(x(i, j) - b(i, j)))
                Next
            Next
            For i = u To ax1vals.Length - 1
                For j = v To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - b(i, j)))
                Next
            Next

            mx = Round(mx, 2)
            ret = ret & ", maximum difference of cells in selected parts of balancing and target matrixs = " & mx.ToString

            mx = 0
            For i = 0 To u - 1
                For j = 0 To v - 1
                    mx = Max(mx, Abs(x(i, j) - a(i, j)))
                Next
            Next
            For i = u To ax1vals.Length - 1
                For j = v To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - a(i, j)))
                Next
            Next

            mx = Round(mx, 2)
            ret = ret & ", maximum difference of cells in selected parts of balancing and starting matrixs = " & mx.ToString
        End If
        mx = 0
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                mx = Max(mx, Abs(x(i, j) - b(i, j)))
            Next
        Next
        mx = Round(mx, 2)
        ret = ret & ", maximum difference of cells in balancing and target matrixs = " & mx.ToString
        mx = 0
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                mx = Max(mx, Abs(x(i, j) - a(i, j)))
            Next
        Next
        mx = Round(mx, 2)
        ret = ret & ", maximum difference of cells in balancing and starting matrixs = " & mx.ToString
        LabelResult.Text = ret
        Dim dtstrt As DataTable = MakeDataTableFromMatrix(a, x1, x2, y1, ax1vals, ax2vals, er, fn)
        Dim dttrgt As DataTable = MakeDataTableFromMatrix(b, x1, x2, y1, ax1vals, ax2vals, er, fnf2)
        LabelResult.Text = ret

        If Not dtstrt Is Nothing Then
            GridView1.DataSource = dtstrt.DefaultView
            GridView1.DataBind()
            GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
            For i = 0 To ax1vals.Count - 1
                GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
            Next
            For j = 0 To ax2vals.Count - 1
                GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
            Next
        End If
        If Not dttrgt Is Nothing Then
            GridView2.DataSource = dttrgt.DefaultView
            GridView2.DataBind()
            GridView2.Rows(GridView2.Rows.Count - 1).Font.Bold = True
        End If

        Dim dtbal As DataTable = MakeDataTableFromMatrix(x, x1, x2, y1, ax1vals, ax2vals, er, fnf2)



        If Not dtbal Is Nothing Then
            GridView3.DataSource = dtbal.DefaultView
            GridView3.DataBind()
            GridView3.Rows(GridView3.Rows.Count - 1).Font.Bold = True
        End If

        'colors and tooltips
        Dim cl As Integer = 100
        Dim clr As Integer = 50
        Dim bg As String = String.Empty
        For i = 0 To GridView2.Rows.Count - 1
            For j = 0 To GridView2.Rows(0).Cells.Count - 1
                If u > 0 AndAlso v > 0 Then
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Ridge
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue

                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2

                    End If
                    If i > u - 1 AndAlso i < GridView2.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2
                    End If
                End If
                'target-ballanced
                If GridView3.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to balanced"
                            Else
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to balanced"
                            End If
                        Else
                            GridView3.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = "<1% difference compare to balanced"
                        End If
                    End If
                ElseIf GridView3.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView3.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = "no difference compare to balanced"
                End If
                'starting-target
                If GridView1.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        'cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"

                            Else
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso GridView1.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
                'starting-ballanced
                If GridView1.Rows(i).Cells(j).Text <> GridView3.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView3.Rows(i).Cells(j).Text) Then
                        'cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView3.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to balanced"
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"
                            Else
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to balanced"
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". <1% difference compare to balanced"
                            GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso GridView1.Rows(i).Cells(j).Text = GridView3.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". no difference compare to balanced"
                    GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
            Next
        Next

        'last row in coefficients
        If Not dtk Is Nothing Then
            GridView4.DataSource = dtk.DefaultView
            GridView4.DataBind()
            If dtk.Rows.Count > 0 Then
                GridView4.Rows(GridView4.Rows.Count - 1).Font.Bold = True
                For i = 0 To GridView4.Rows.Count - 1
                    GridView4.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                Next
                Dim z As Integer = 0
                For i = 0 To GridView1.Rows.Count - 2
                    GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                    z = i + 1
                Next
                z = z + 2
                For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                    GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
                Next
            End If
        End If

        Session("GridView1DataSource") = dtstrt
        Session("GridView2DataSource") = dttrgt
        Session("GridView3DataSource") = dtbal
        Session("GridView4DataSource") = dtk

        'GridView5,6
        If u > 0 AndAlso v > 0 Then
            Dim y(ax1vals.Length - 1, ax2vals.Length - 1) As Double
            Dim dwk As New DataTable
            'balancing
            ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, y, dwk, 0, 0)
            Dim dtwbal As DataTable = MakeDataTableFromMatrix(y, x1, x2, y1, ax1vals, ax2vals, er, fnf2)
            If Not dtwbal Is Nothing Then
                GridView5.DataSource = dtwbal.DefaultView
                GridView5.DataBind()
                GridView5.Rows(GridView3.Rows.Count - 1).Font.Bold = True
            End If

            'last row in coefficients
            If Not dwk Is Nothing Then
                GridView6.DataSource = dwk.DefaultView
                GridView6.DataBind()
                If dwk.Rows.Count > 0 Then
                    GridView6.Rows(GridView6.Rows.Count - 1).Font.Bold = True
                    For i = 0 To GridView6.Rows.Count - 1
                        GridView6.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                    Next
                End If
            End If

            'colors
            For i = 0 To GridView3.Rows.Count - 1
                For j = 0 To GridView3.Rows(0).Cells.Count - 1
                    'partially ballanced <=> ballanced
                    If GridView3.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to partially balanced"
                                Else
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to partially balanced"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = "<1% difference compare to partially balanced"
                            End If
                        End If
                    ElseIf GridView3.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = "no difference compare to partially balanced"
                    End If

                    'target <=> balanced
                    If GridView2.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView2.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView2.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView2.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView2.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to target"
                                Else
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to target"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". <1% difference compare to target"
                            End If
                        End If
                    ElseIf GridView2.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". no difference compare to target"
                    End If
                Next
            Next
            mx = 0
            For i = 0 To ax1vals.Length - 1
                For j = 0 To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - y(i, j)))
                Next
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = "Compare Partially Balanced Matrix with Balaced Whole Matrix: maximum difference of cells in partially balancinging and whole balancing matrixs = " & mx.ToString

            'compare partly balanced coefficients and whole balanced, maximum cell difference
            mx = 0
            For i = 1 To ax1vals.Length
                mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging rows coefficients and whole balancing rows coefficients  = " & mx.ToString

            mx = 0
            For i = ax1vals.Length + 2 To ax1vals.Length + ax2vals.Length + 1
                mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging columns coefficients and whole balancing columns coefficients  = " & mx.ToString

            For i = 1 To ax1vals.Length + ax2vals.Length + 1
                If dtk(dtk.Rows.Count - 1)(i) <> dwk(dwk.Rows.Count - 1)(i) Then
                    If IsNumeric(dtk(dtk.Rows.Count - 1)(i)) AndAlso IsNumeric(dwk(dwk.Rows.Count - 1)(i)) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(dtk(dtk.Rows.Count - 1)(i)) - CDbl(dwk(dwk.Rows.Count - 1)(i))) / Max(1, Max(CDbl(dtk(dtk.Rows.Count - 1)(i)), CDbl(dwk(dwk.Rows.Count - 1)(i)))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If dtk(dtk.Rows.Count - 1)(i) > dwk(dwk.Rows.Count - 1)(i) Then
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " -" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                            Else
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " +" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                            End If
                        Else
                            GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " <1% difference compare to partially balancing coefficients"
                        End If
                    End If
                ElseIf dtk(dtk.Rows.Count - 1)(i) = dwk(dwk.Rows.Count - 1)(i) Then
                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " no difference compare to partially balancing coefficients"
                End If

            Next
            Session("GridView5DataSource") = dtwbal
            Session("GridView6DataSource") = dwk
        End If

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Label2.Text = "Balancing for sum of rows and columns for " & fn.ToLower & " of values of the field1 '" & y1 & "' in the starting matrix and for " & fnf2.ToLower & " of the field2 = " & sval & " and the target matrix for " & fnf2.ToLower & " of field2 = " & tval & " : "

    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = node.Value
        Dim urlBack As String = HttpContext.Current.Request.Url.ToString
        Dim nodeText As String = node.Text

        If nodeText.Contains("Advanced Report Designer") Then
            Session("urlback") = urlBack
        Else
            Session("urlback") = Nothing
        End If
        Response.Redirect(url)
    End Sub
    Private Sub btnShowEntryTbl4c_Click(sender As Object, e As EventArgs) Handles btnShowEntryTbl4c.Click
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim m As Integer = 0
        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim fn As String = DropDownList4.SelectedValue.ToString
        Dim y1 As String = DropDownList3.SelectedValue.ToString  'field1
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        Dim dims As String = DropDownColumnsDim.Text
        Dim flds As String()
        flds = Split(dims, ",")
        Dim n As Integer = flds.Length
        Dim dstart As DataTable = Nothing
        Dim dtarget As DataTable = Nothing
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            dstart = MakeMultidimesionalTables(dv3.ToTable, fn, y1, "", dims, ret, Session("UserConnString"), Session("UserConnProvider"))
            If dstart Is Nothing OrElse dstart.Rows.Count = 0 Then
                LabelError.Visible = True
                LabelError.Text = "No data for selected parameters."
                Exit Sub
            End If
        End If
        Dim dtstrt As New DataTable
        Dim lensums(0) As Integer
        'dtstrt - original start sums without fixing zeros
        ret = MakeMultidimensionalSumsFromDataTable(dstart, dims, "Value", dtstrt, lensums)

        'make table
        'dtarget - show table
        Dim row As HtmlTableRow
        Dim cell As HtmlTableCell

        For i = 0 To n - 1
            If i > 0 Then
                cell = New HtmlTableCell
                EntryTable.Rows(0).Cells.Add(cell)
            End If
            EntryTable.Rows(0).Cells(2 * i).InnerText = flds(i)
            EntryTable.Rows(0).Cells(2 * i).BgColor = "#EFFBFB"
            cell = New HtmlTableCell
            EntryTable.Rows(0).Cells.Add(cell)
            EntryTable.Rows(0).Cells((2 * i) + 1).InnerText = "Sums by " & flds(i)
            EntryTable.Rows(0).Cells((2 * i) + 1).BgColor = "#EFFBFB"
        Next
        m = lensums(0)
        For k = 1 To n - 1
            m = Max(m, lensums(k))
        Next
        For j = 1 To m
            row = New HtmlTableRow
            For k = 1 To 2 * n
                cell = New HtmlTableCell
                row.Cells.Add(cell)
            Next
            EntryTable.Rows.Add(row)
        Next
        For i = 0 To n - 1
            For j = 1 To lensums(i)

                EntryTable.Rows(j).Cells(2 * i).InnerText = dtstrt.Rows(j - 1)(2 * i).ToString

                Dim ctl As New TextBox
                ctl.Text = ""
                ctl.ID = "txt" & "_" & i.ToString & "_" & (j - 1).ToString
                ctl.ToolTip = "Enter Sum"
                ctl.Font.Size = 10
                ctl.BorderColor = Color.DarkRed
                EntryTable.Rows(j).Cells((2 * i) + 1).InnerText = ""
                EntryTable.Rows(j).Cells((2 * i) + 1).Controls.Add(ctl)
                EntryTable.Rows(j).Cells((2 * i) + 1).Align = "right"
                EntryTable.Rows(j).Cells((2 * i) + 1).BorderColor = "red"
            Next
        Next


    End Sub
    Private Sub btnBalanceFlds2a_Click(sender As Object, e As EventArgs) Handles btnBalanceFlds2a.Click, btnBalanceVals2b.Click, btnManuallyEntered4c.Click
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4a:") Then
            '(4a) Balancing matrix of field1 for the field2 sums by selected fields
            'DropDownColumnsDim - multiple columns for balancing
            'DropDownList3 - field1
            'DropDownList4 - field1 aggregation function
            'DropDownList5 - field2
            'DropDownList8 - field2 aggregation function
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4b:") Then
            '(4b) Balancing matrix of field1 between starting and target values of the field2
            'DropDownColumnsDim - multiple columns for balancing
            'DropDownList3 - field1
            'DropDownList4 - field1 aggregation function
            'DropDownList5 - field2
            'DropDownList6 - starting value of field2
            'DropDownList7 - target value of field2
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            '(4c) Balancing matrix of field1 for the manually entered sums by selected fields
            'DropDownColumnsDim - multiple columns for balancing
            'DropDownList3 - field1
            'DropDownList4 - field1 aggregation function
            'table of sums with textboxes ctl.ID = "txt" & "_" & i.ToString & "_" & j.ToString
        End If

        LabelError.Text = ""
        LabelResult.Text = "Waiting..."
        Label2.Text = "Waiting..."
        mainGrids.Visible = True
        DropDownList1.Visible = False
        DropDownList1.Enabled = False
        DropDownList2.Visible = False
        DropDownList2.Enabled = False

        Dim i As Integer
        Dim j As Integer
        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim fn As String = DropDownList4.SelectedValue.ToString
        Dim y1 As String = DropDownList3.SelectedValue.ToString  'field1
        Dim y2 As String = String.Empty
        Dim fnf2 As String = String.Empty
        Dim fld As String = String.Empty
        Dim sval As String = String.Empty
        Dim tval As String = String.Empty
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4a:") Then
            y2 = DropDownList5.SelectedValue.ToString  'field2
            fnf2 = DropDownList8.SelectedValue.ToString
            Label12.Text = "Starting Sums of " & fn & " of " & y1 & " by selected fields"
            Label13.Text = "Target Sums of " & fnf2 & " of " & y2 & " by selected fields"
            Label14.Text = "Balancing Sums of " & fnf2 & " of " & y2 & " by selected fields"
            Label27.Text = "Starting Values of " & fn & " of " & y1 & " and final Balancing Coefficients, Precisions, and Values"
            Label12.Text = Label12.Text.Replace("Sums of Sum", "Sums")
            Label13.Text = Label13.Text.Replace("Sums of Sum", "Sums")
            Label14.Text = Label14.Text.Replace("Sums of Sum", "Sums")
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4b:") Then
            y2 = DropDownList5.SelectedValue.ToString  'field2
            fld = DropDownList5.SelectedValue.ToString
            sval = DropDownList6.SelectedValue.ToString
            tval = DropDownList7.SelectedValue.ToString
            fnf2 = DropDownList4.SelectedValue.ToString  'no sense to have different ones
            y2 = y1
            Label12.Text = "Starting Sums of " & fn & " of " & y1 & " where " & fld & "='" & sval & "'"
            Label13.Text = "Target Sums of " & fnf2 & " of " & y1 & " where " & fld & "='" & tval & "'"
            Label14.Text = "Balancing Sums of " & fnf2 & " of " & y1
            Label27.Text = "Starting Values of " & fn & " of " & y1 & " and final Balancing Coefficients, Precisions, and Values"
            Label12.Text = Label12.Text.Replace("Sums of Sum", "Sums")
            Label13.Text = Label13.Text.Replace("Sums of Sum", "Sums")
            Label14.Text = Label14.Text.Replace("Sums of Sum", "Sums")
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            Label12.Text = "Starting Sums of " & fn & " of " & y1 & " by selected fields"
            Label13.Text = "Target Sums by selected fields"
            Label14.Text = "Balancing Sums by selected fields"
            Label27.Text = "Starting Values of " & fn & " of " & y1 & " and final Balancing Coefficients, Precisions, and Values"
            Label12.Text = Label12.Text.Replace("Sums of Sum", "Sums")
            Label13.Text = Label13.Text.Replace("Sums of Sum", "Sums")
            Label14.Text = Label14.Text.Replace("Sums of Sum", "Sums")
        End If

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        Dim dims As String = DropDownColumnsDim.Text
        Dim dstart As DataTable = Nothing
        Dim dtarget As DataTable = Nothing
        Dim dstartSums As New DataTable
        Dim dtargetSums As New DataTable
        Dim dtstrt As New DataTable
        Dim dttrgt As New DataTable
        Dim dtk As New DataTable
        Dim dtbal As New DataTable
        Dim nsteps As Integer = 0
        Dim lensums(0) As Integer
        Dim fls() As String = Split(dims, ",")
        Dim n As Integer = fls.Length

        '4a --------------------------------------------------------------------------------------------------
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4a:") Then
            If DropDownList3.Text.Trim = "" OrElse DropDownList5.Text.Trim = "" Then
                LabelError.Text = "Matrix fields must be selected."
                Exit Sub
            End If
            dstart = MakeMultidimesionalTables(dv3.ToTable, fn, y1, "", dims, ret, Session("UserConnString"), Session("UserConnProvider"))
            dtarget = MakeMultidimesionalTables(dv3.ToTable, fnf2, y2, "", dims, ret, Session("UserConnString"), Session("UserConnProvider"))
            ret = MakeMultidimensionalSumsFromDataTable(dtarget, dims, "Value", dttrgt, lensums)
            If dstart Is Nothing OrElse dstart.Rows.Count = 0 OrElse dtarget Is Nothing OrElse dtarget.Rows.Count = 0 Then
                LabelError.Visible = True
                LabelError.Text = "No data for selected parameters."
                Exit Sub
            End If
            dv3.RowFilter = ""

            '4b --------------------------------------------------------------------------------------------------
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4b:") Then
            If DropDownList3.Text.Trim = "" OrElse DropDownList5.Text.Trim = "" Then
                LabelError.Text = "Matrix fields must be selected."
                Exit Sub
            End If
            dstart = MakeMultidimesionalTables(dv3.ToTable, fn, y1, fld & "='" & sval & "'", dims, ret, Session("UserConnString"), Session("UserConnProvider"))
            dtarget = MakeMultidimesionalTables(dv3.ToTable, fn, y1, fld & "='" & tval & "'", dims, ret, Session("UserConnString"), Session("UserConnProvider"))
            ret = MakeMultidimensionalSumsFromDataTable(dtarget, dims, "Value", dttrgt, lensums)
            If dstart Is Nothing OrElse dstart.Rows.Count = 0 OrElse dtarget Is Nothing OrElse dtarget.Rows.Count = 0 Then
                LabelError.Visible = True
                LabelError.Text = "No data for selected parameters."
                Exit Sub
            End If

            '4c --------------------------------------------------------------------------------------------------
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            If DropDownList3.Text.Trim = "" Then
                LabelError.Text = "Matrix field1 must be selected."
                Exit Sub
            End If
            dstart = MakeMultidimesionalTables(dv3.ToTable, fn, y1, "", dims, ret, Session("UserConnString"), Session("UserConnProvider"))
            If dstart Is Nothing OrElse dstart.Rows.Count = 0 Then
                LabelError.Visible = True
                LabelError.Text = "No data for selected parameters."
                Exit Sub
            End If

        End If

        'make them even
        Dim sma As Decimal = 0  'overall sum for target matrix
        Dim smb As Decimal = 0  'overall sum for target matrix
        '4a  and  4b  --------------------------------------------------------------------------------------------------
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4a:") OrElse DropDownListScenarios.SelectedItem.Text.StartsWith("4b:") Then
            ''dtargetSumsOrg - original start sums without fixing zeros
            'Dim dtargetSumsOrg As New DataTable
            'ret = MakeMultidimensionalSumsFromDataTable(dtarget, dims, "Value", dtargetSumsOrg, lensums, False)

            ret = MakeStartAndTargetEven(dstart, dtarget, dims)
            For i = 0 To dstart.Rows.Count - 1
                sma = sma + dstart.Rows(i)("Value")
            Next
            For i = 0 To dstart.Rows.Count - 1
                smb = smb + dtarget.Rows(i)("Value")
            Next
            ret = AdjustDataTableByOverallSum(dtarget, sma)
            ret = MakeMultidimensionalSumsFromDataTable(dtarget, dims, "Value", dtargetSums, lensums, True)
        End If

        'dtstrt - original start sums without fixing zeros
        ret = MakeMultidimensionalSumsFromDataTable(dstart, dims, "Value", dtstrt, lensums)

        ret = MakeMultidimensionalSumsFromDataTable(dstart, dims, "Value", dstartSums, lensums, True)

        '4c --------------------------------------------------------------------------------------------------
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            'dtargetSums = 'take from table
            Dim col As DataColumn
            For i = 0 To dstartSums.Columns.Count - 1
                col = New DataColumn
                col.DataType = dstartSums.Columns(i).DataType 'System.Type.GetType("System.Double")
                col.ColumnName = dstartSums.Columns(i).ColumnName
                dtargetSums.Columns.Add(col)
            Next
            Dim row As DataRow
            For j = 0 To dstartSums.Rows.Count - 1
                row = dtargetSums.NewRow
                For i = 0 To n - 1
                    row(2 * i) = dstartSums.Rows(j)(2 * i)
                    If Not Request("txt" & "_" & i.ToString & "_" & j.ToString) Is Nothing AndAlso IsNumeric(Request("txt" & "_" & i.ToString & "_" & j.ToString)) Then
                        row(2 * i + 1) = Request("txt" & "_" & i.ToString & "_" & j.ToString)
                    ElseIf i = 0 AndAlso j = 1 AndAlso Request("txt" & "_" & i.ToString & "_" & j.ToString) Is Nothing Then
                        MessageBox.Show("Enter all target sums! ", "ERROR!! No data", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                        Exit Sub
                    End If
                Next
                dtargetSums.Rows.Add(row)
            Next
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = "TargetValue"
            dstart.Columns.Add(col)
            'sum on Start
            For i = 0 To dstart.Rows.Count - 1
                sma = sma + dstart.Rows(i)("Value")
                dstart.Rows(i)("TargetValue") = 0
            Next
            'sum of first selected field on Target used for callibration
            For i = 0 To dtargetSums.Rows.Count - 1
                If dtargetSums.Rows(i)(1).ToString <> "" AndAlso IsNumeric(dtargetSums.Rows(i)(1)) Then
                    smb = smb + dtargetSums.Rows(i)(1)
                End If
            Next
            ret = AdjustSumsTableByOverallSum(dtargetSums, dims, sma, lensums)
        End If

        ' DO NOT REMOVE!!! for additional dimensions and partially balancing - not yet implemented, TextBoxUV.Text.Trim=""
        Dim u() As String = Split(TextBoxUV.Text.Trim, ",")
        Dim m As Integer = u.Length
        Dim v(m - 1) As Integer
        Dim sv As Integer = 0 'sums of v(i)
        For i = 0 To m - 1
            v(i) = CInt(u(i))
            If v(i) > lensums(i) Then
                LabelError.Text = "Partial numbers shoud be smaller than dimensions (" & lensums.ToString & ")."
                Exit Sub
                sv = sv + v(i)
            End If
        Next
        If sv > 0 Then 'partially balancing
            If m <> n Then
                LabelError.Text = "There should be the same partial numbers as fields selected (" & n.ToString & ")."
                Exit Sub
            End If
        End If

        'balancing
        ret = CanTargetDataTableBalancedFromStartingOne(dstart, dtarget, dstartSums, dtargetSums, dims, l, q, er, dtbal, dtk, nsteps, lensums, v)


        If chkAdjustByStart.Checked = False Then  'adjust back by target sums
            'adjust to original target
            ret = ret & AdjustSumsTableByOverallSum(dtargetSums, dims, smb, lensums)
            'dtbal
            ret = ret & AdjustSumsTableByOverallSum(dtbal, dims, smb, lensums)
            'dtk BalancedValue
            For j = 0 To dtk.Rows.Count - 1
                If dtk.Rows(j)("BalancedValue").ToString.Trim = "" Then
                    dtk.Rows(j)("BalancedValue") = 1
                End If
                dtk.Rows(j)("BalancedValue") = Round(dtk.Rows(j)("BalancedValue") * (smb / sma), 2)
            Next
        End If

        If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            dttrgt = dtargetSums
        End If

        'DO NOT REMOVE!!!  for additional dimensions
        Dim mx As Decimal = 0  'cell diff
        If sv > 0 Then
            For i = 0 To m - 1
                For j = 0 To v(i) - 1
                    mx = Max(mx, Abs(dtbal.Rows(i)("Value") - dtarget.Rows(i)("Value")))
                Next
            Next

            mx = Round(mx, 2)
            ret = ret & ", maximum difference of cells in selected parts of balancing and target matrixs = " & mx.ToString

            mx = 0
            For i = 0 To m - 1
                For j = 0 To v(i) - 1
                    mx = Max(mx, Abs(dtbal.Rows(i)("Value") - dstart.Rows(i)("Value")))
                Next
            Next

            mx = Round(mx, 2)
            ret = ret & ", maximum difference of cells in selected parts of balancing and starting matrixs = " & mx.ToString
        End If

        mx = 0
        mx = DifferenceBetweenSums(dtbal, dtargetSums, dims, er)
        mx = Round(mx, 2)
        If mx > 0 Then ret = ret & ", maximum difference of cells in balancing and target sums = " & mx.ToString

        mx = 0
        mx = DifferenceBetweenSums(dtbal, dstartSums, dims, er)
        mx = Round(mx, 2)
        If mx > 0 Then ret = ret & ", maximum difference of cells in balancing and starting sums = " & mx.ToString

        mx = 0
        mx = DifferenceBetweenSums(dstartSums, dtargetSums, dims, er)
        mx = Round(mx, 2)
        If mx > 0 Then ret = ret & ", maximum difference of cells in starting and target sums = " & mx.ToString
        mx = 0

        LabelResult.Text = ret

        If Not dtstrt Is Nothing Then
            'last summary row and sum column names
            Dim ssm As Double = 0
            Dim row As DataRow
            row = dtstrt.NewRow
            Try
                For i = 0 To n - 1 'loop through selected fields
                    ssm = 0
                    row(2 * i) = "Total:"
                    For j = 0 To lensums(i) - 1 'loop through table dtstrt column i  rows j  
                        ssm = ssm + dtstrt.Rows(j)(2 * i + 1)
                    Next
                    row(2 * i + 1) = Round(ssm, 2)
                    dtstrt.Columns(2 * i + 1).ColumnName = "Sums by " & dtstrt.Columns(2 * i).ColumnName
                Next
                dtstrt.Rows.Add(row)
            Catch ex As Exception
                er = ex.Message
            End Try
            GridView1.DataSource = dtstrt.DefaultView
            GridView1.DataBind()
            GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
        End If
        If Not dttrgt Is Nothing Then
            'last summary row
            Dim ssm As Double = 0
            Dim row As DataRow
            row = dttrgt.NewRow
            Try
                For i = 0 To n - 1 'loop through selected fields
                    ssm = 0
                    row(2 * i) = "Total:"
                    For j = 0 To lensums(i) - 1 'loop through table dtstrt column i  rows j  
                        ssm = ssm + dttrgt.Rows(j)(2 * i + 1)
                    Next
                    row(2 * i + 1) = Round(ssm, 2)
                    dttrgt.Columns(2 * i + 1).ColumnName = "Sums by " & dttrgt.Columns(2 * i).ColumnName
                Next
                dttrgt.Rows.Add(row)
            Catch ex As Exception
                er = ex.Message
            End Try
            GridView2.DataSource = dttrgt.DefaultView
            GridView2.DataBind()
            GridView2.Rows(GridView2.Rows.Count - 1).Font.Bold = True

            ''SAMPLE !!!  only one row editable !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'For i = 0 To dttrgt.Rows.Count - 2
            '    GridView2.EditIndex = i
            'Next

        End If
        If Not dtbal Is Nothing Then
            'last summary row
            Dim ssm As Double = 0
            Dim row As DataRow
            row = dtbal.NewRow
            Try
                For i = 0 To n - 1 'loop through selected fields
                    ssm = 0
                    row(2 * i) = "Total:"
                    For j = 0 To lensums(i) - 1 'loop through table dtstrt column i  rows j  
                        ssm = ssm + dtbal.Rows(j)(2 * i + 1)
                        dtbal.Rows(j)(2 * i + 1) = Round(dtbal.Rows(j)(2 * i + 1), 2)
                    Next
                    row(2 * i + 1) = Round(ssm, 2)
                    dtbal.Columns(2 * i + 1).ColumnName = "Sums by " & dtbal.Columns(2 * i).ColumnName
                Next
                dtbal.Rows.Add(row)
            Catch ex As Exception
                er = ex.Message
            End Try
            GridView3.DataSource = dtbal.DefaultView
            GridView3.DataBind()
            GridView3.Rows(GridView3.Rows.Count - 1).Font.Bold = True
        End If

        'colors and tooltips
        Dim cl As Integer = 100
        Dim clr As Integer = 50
        Dim bg As String = String.Empty
        mx = 0
        For i = 0 To GridView2.Rows.Count - 1
            For j = 0 To GridView2.Rows(0).Cells.Count - 1
                If sv > 0 Then
                    If i < v(0) AndAlso j > 1 AndAlso j < v(1) Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Ridge
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue

                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2

                    End If
                    If i > v(0) - 1 AndAlso i < GridView2.Rows.Count - 1 AndAlso j > v(1) + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2
                    End If
                End If

                'target-ballanced
                If GridView3.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                            mx = Max(cl, mx)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to balanced"
                            Else
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to balanced"
                            End If
                        Else
                            GridView3.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = "<1% difference compare to balanced"
                        End If
                    End If
                ElseIf GridView3.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView3.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = "no difference compare to balanced"
                End If
                'starting-target
                If GridView1.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"
                            Else
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso GridView1.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
                'starting-ballanced
                If GridView1.Rows(i).Cells(j).Text <> GridView3.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView3.Rows(i).Cells(j).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView3.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to balanced"
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"
                            Else
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to balanced"
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        ElseIf cl = 0 Then
                            GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". <1% difference compare to balanced"
                            GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso GridView1.Rows(i).Cells(j).Text = GridView3.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". no difference compare to balanced"
                    GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
            Next
        Next
        LabelResult.Text = LabelResult.Text & ", maxumum difference in percentage between balanced sums and target sums = " & Round(mx, 2).ToString & "%"

        If Not dtk Is Nothing Then
            'n = Split(dims, ",").Length
            dtk.Columns(n).Caption = "StartValue"
            dtk.Columns(n).ColumnName = "StartValue"
            Dim row As DataRow
            row = dtk.NewRow
            Try
                row(0) = "Total:"
                Dim sso As Double = 0
                Dim sst As Double = 0
                Dim ssb As Double = 0
                For j = 0 To dtk.Rows.Count - 1
                    If IsNumeric(dtk.Rows(j)(n)) Then sso = sso + dtk.Rows(j)(n)
                    If IsNumeric(dtk.Rows(j)(n + 1)) Then sst = sst + dtk.Rows(j)(n + 1)
                    If IsNumeric(dtk.Rows(j)(dtk.Columns.Count - 1)) Then ssb = ssb + dtk.Rows(j)(dtk.Columns.Count - 1)
                Next
                row(n) = Round(sso, 2)
                row(n + 1) = Round(sst, 2)
                row(dtk.Columns.Count - 1) = Round(ssb, 2)
                dtk.Rows.Add(row)
            Catch ex As Exception
                er = ex.Message
            End Try


            'final last step in coefficients
            'Dim k As Integer = dtk.Columns.Count - (n * 4) - 4
            Dim k As Integer = dtk.Columns.Count - 4
            If chkSeeSteps.Checked = False Then
                If chkSeeLastSteps.Checked = True Then
                    'If ret.StartsWith("Not ") Then
                    k = k - 4 * n
                    While k > n + 1
                        dtk.Columns.Remove(dtk.Columns(k).Caption)
                        k = k - 1
                    End While
                Else
                    While k > n + 1
                        dtk.Columns.Remove(dtk.Columns(k).Caption)
                        k = k - 1
                    End While
                End If

            End If

            Dim col As DataColumn = New DataColumn
            col.DataType = System.Type.GetType("System.Double")
            If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
                col.ColumnName = "DifferenceStartBalanced"
                dtk.Columns.Add(col)
                'change column TargetValue
                dtk.Columns("TargetValue").DataType = System.Type.GetType("System.String")
                For i = 0 To dtk.Rows.Count - 1
                    dtk.Rows(i)("DifferenceStartBalanced") = Round(dtk.Rows(i)("StartValue") - dtk.Rows(i)("BalancedValue"), 2)
                    dtk.Rows(i)("TargetValue") = "n/a"
                Next
            Else
                col.ColumnName = "DifferenceTargetBalanced"
                dtk.Columns.Add(col)
                For i = 0 To dtk.Rows.Count - 1
                    dtk.Rows(i)("DifferenceTargetBalanced") = Round(dtk.Rows(i)("TargetValue") - dtk.Rows(i)("BalancedValue"), 2)
                Next
            End If

            GridView4.DataSource = dtk.DefaultView
            GridView4.DataBind()
            If dtk.Rows.Count > 0 Then
                Dim z As Integer = dtk.Columns.Count - 1
                'GridView4.Rows(GridView4.Rows.Count - 1).Font.Bold = True
                For i = 0 To GridView4.Rows.Count - 1
                    If IsNumeric(GridView4.Rows(i).Cells(n).Text) AndAlso IsNumeric(GridView4.Rows(i).Cells(z).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView4.Rows(i).Cells(n).Text) - CDbl(GridView4.Rows(i).Cells(z).Text)) / Max(1, Max(CDbl(GridView4.Rows(i).Cells(n).Text), CDbl(GridView4.Rows(i).Cells(z).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView4.Rows(i).Cells(z).Text) > CDbl(GridView4.Rows(i).Cells(n).Text) Then
                                'GridView4.Rows(i).Cells(z).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView4.Rows(i).Cells(n).BackColor = Color.FromArgb(207 - clr, 207 - cl, 207 - clr)
                                GridView4.Rows(i).Cells(z).ToolTip = " +" & bg & cl.ToString & "% compare To original Value"
                                GridView4.Rows(i).Cells(n).ToolTip = " -" & bg & cl.ToString & "% compare To balancing value"
                            Else
                                GridView4.Rows(i).Cells(n).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                'GridView4.Rows(i).Cells(z).BackColor = Color.FromArgb(207 - clr, 207 - cl, 207 - clr)
                                GridView4.Rows(i).Cells(z).ToolTip = " -" & bg & cl.ToString & "% compare To original Value"
                                GridView4.Rows(i).Cells(n).ToolTip = " +" & bg & cl.ToString & "% compare To balancing value"
                            End If
                        ElseIf cl = 0 Then
                            GridView4.Rows(i).Cells(z).ToolTip = " <1% difference compare To original Value"
                            GridView4.Rows(i).Cells(n).ToolTip = " <1% difference compare To balancing value"
                        End If
                    End If
                    If IsNumeric(GridView4.Rows(i).Cells(n + 1).Text) AndAlso IsNumeric(GridView4.Rows(i).Cells(z).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView4.Rows(i).Cells(n + 1).Text) - CDbl(GridView4.Rows(i).Cells(z).Text)) / Max(1, Max(CDbl(GridView4.Rows(i).Cells(n + 1).Text), CDbl(GridView4.Rows(i).Cells(z).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView4.Rows(i).Cells(z).Text) > CDbl(GridView4.Rows(i).Cells(n + 1).Text) Then
                                GridView4.Rows(i).Cells(z).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView4.Rows(i).Cells(n + 1).BackColor = Color.FromArgb(207 - clr, 207 - cl, 207 - clr)
                                GridView4.Rows(i).Cells(z).ToolTip = GridView4.Rows(i).Cells(z).ToolTip & ", +" & bg & cl.ToString & "% compare To target Value"
                                GridView4.Rows(i).Cells(n + 1).ToolTip = " -" & bg & cl.ToString & "% compare To balancing value"
                            Else
                                GridView4.Rows(i).Cells(n + 1).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView4.Rows(i).Cells(z).BackColor = Color.FromArgb(207 - clr, 207 - cl, 207 - clr)
                                GridView4.Rows(i).Cells(z).ToolTip = GridView4.Rows(i).Cells(z).ToolTip & ", -" & bg & cl.ToString & "% compare To target Value"
                                GridView4.Rows(i).Cells(n + 1).ToolTip = " +" & bg & cl.ToString & "% compare To balancing value"
                            End If
                        ElseIf cl = 0 Then
                            GridView4.Rows(i).Cells(z).ToolTip = GridView4.Rows(i).Cells(z).ToolTip & ", <1% difference compare to target Value"
                            GridView4.Rows(i).Cells(n + 1).ToolTip = " <1% difference compare to balancing value"
                        End If
                    End If
                    If i = GridView4.Rows.Count - 1 Then
                        GridView4.Rows(i).Cells(0).Font.Bold = True
                        GridView4.Rows(i).Cells(n).Font.Bold = True
                        GridView4.Rows(i).Cells(n + 1).Font.Bold = True
                        GridView4.Rows(i).Cells(z).Font.Bold = True
                    End If
                Next
            End If
        End If
        Session(" GridView1DataSource") = dtstrt
        Session(" GridView2DataSource") = dttrgt
        Session(" GridView3DataSource") = dtbal
        Session(" GridView4DataSource") = dtk


        'GridView5,6
        If sv > 0 Then
            'Dim y(axvals.GetLength(0) - 1, axvals.GetLength(1) - 1) As Double
            Dim dwk As New DataTable
            'balancing

            'ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, y, dwk, 0, 0)
            'Dim dtwbal As DataTable = MakeDataTableFromMatrix(y, x1, x2, y2, ax1vals, ax2vals, er, fnf2)
            Dim dtwbal As New DataTable
            ret = CanTargetDataTableBalancedFromStartingOne(dstart, dtarget, dstartSums, dtargetSums, dims, l, q, er, dtwbal, dwk, nsteps, lensums, v)

            If Not dtwbal Is Nothing Then
                GridView5.DataSource = dtwbal.DefaultView
                GridView5.DataBind()
                GridView5.Rows(GridView3.Rows.Count - 1).Font.Bold = True
            End If

            'last row in coefficients
            If Not dwk Is Nothing Then
                GridView6.DataSource = dwk.DefaultView
                GridView6.DataBind()
                If dwk.Rows.Count > 0 Then
                    GridView6.Rows(GridView6.Rows.Count - 1).Font.Bold = True
                End If
            End If

            'colors
            For i = 0 To GridView3.Rows.Count - 1
                For j = 0 To GridView3.Rows(0).Cells.Count - 1
                    'partially ballanced <=> ballanced
                    If GridView3.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to partially balanced"
                                Else
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to partially balanced"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = "<1% difference compare to partially balanced"
                            End If
                        End If
                    ElseIf GridView3.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = " no difference compare to partially balanced"
                    End If

                    'target <=> balanced
                    If GridView2.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView2.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView2.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView2.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView2.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to target"
                                Else
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to target"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". <1% difference compare to target"
                            End If
                        End If
                    ElseIf GridView2.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & " . no difference compare to target"
                    End If
                Next
            Next

            'compare partly balanced and whole balanced, maximum cell difference
            mx = 0
            mx = DifferenceBetweenSums(dtbal, dtwbal, dims, er)
            mx = Round(mx, 2)
            LabelCompare.Text = " Compare Partially Balanced Matrix with Balaced Whole Matrix:maximum difference of cells in partially balancinging and whole balancing matrixs=" & mx.ToString


            'TODO for additional dimensions  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'compare partly balanced coefficients and whole balanced, maximum cell difference
            'mx = 0
            'For i = 1 To axvals.GetLength(0)
            '    mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            'Next
            'mx = Round(mx, 2)
            'LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging rows coefficients and whole balancing rows coefficients  = " & mx.ToString

            'mx = 0
                'For i = axvals.GetLength(0) + 2 To axvals.GetLength(0) + axvals.GetLength(1) + 1
                '    mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
                'Next
                'mx = Round(mx, 2)
                'LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging columns coefficients and whole balancing columns coefficients  = " & mx.ToString

                'For i = 1 To axvals.GetLength(0) + axvals.GetLength(1) + 1
                '    If dtk(dtk.Rows.Count - 1)(i) <> dwk(dwk.Rows.Count - 1)(i) Then
                '        If IsNumeric(dtk(dtk.Rows.Count - 1)(i)) AndAlso IsNumeric(dwk(dwk.Rows.Count - 1)(i)) Then
                '            Try
                '                cl = 100 * Round(Abs(CDbl(dtk(dtk.Rows.Count - 1)(i)) - CDbl(dwk(dwk.Rows.Count - 1)(i))) / Max(1, Max(CDbl(dtk(dtk.Rows.Count - 1)(i)), CDbl(dwk(dwk.Rows.Count - 1)(i)))), 2)
                '                cl = Min(101, cl)
                '            Catch ex As Exception
                '                cl = 101
                '            End Try
                '            clr = cl - 43
                '            If cl > 0 Then
                '                bg = ""
                '                If cl > 100 Then bg = " >"
                '                If dtk(dtk.Rows.Count - 1)(i) > dwk(dwk.Rows.Count - 1)(i) Then
                '                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " -" & bg & cl.ToString & "% compare to partially balancing coefficients"
                '                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                '                Else
                '                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " +" & bg & cl.ToString & "% compare to partially balancing coefficients"
                '                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                '                End If
                '            Else
                '                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " <1% difference compare to partially balancing coefficients"
                '            End If
                '        End If
                '    ElseIf dtk(dtk.Rows.Count - 1)(i) = dwk(dwk.Rows.Count - 1)(i) Then
                '        GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " no difference compare to partially balancing coefficients"
                '    End If
                '    Session("GridView5DataSource") = dtwbal
                '    Session("GridView6DataSource") = dwk
                'Next

            End If

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4a:") Then
            Label2.Text = "Balancing for sums of fields " & dims & " of the starting matrix for " & fn.ToLower & " values of the field1 '" & y1 & "' and the target matrix for " & fnf2.ToLower & " values of the field2 '" & y2 & "' : "

        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4b:") Then
            Label2.Text = "Balancing for sums of fields " & dims & " of the starting matrix for " & fn.ToLower & " values of the field1 '" & y1 & "' for " & fld & "='" & sval & "' and the target matrix for " & fld & "='" & tval & "' : "
        ElseIf DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            Label2.Text = "Balancing for sums of fields " & dims & " of the starting matrix for " & fn.ToLower & " values of the field1 '" & y1 & "' for manually entered sums for selected fields : "

        End If
    End Sub

    Private Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(0).Font.Bold = True  '.BackColor = Color.FromArgb(152, 203, 203)
            e.Row.Cells(1).Font.Bold = True
            'e.Row.Cells(1).BackColor = Color.FromArgb(160, 213, 213)

            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub
    Private Sub GridView2_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(0).Font.Bold = True  '.BackColor = Color.FromArgb(152, 203, 203)
            e.Row.Cells(1).Font.Bold = True
            'e.Row.Cells(1).BackColor = Color.FromArgb(160, 213, 213)
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub
    Private Sub GridView3_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView3.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(0).Font.Bold = True  '.BackColor = Color.FromArgb(152, 203, 203)
            e.Row.Cells(1).Font.Bold = True
            'e.Row.Cells(1).BackColor = Color.FromArgb(160, 213, 213)
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub
    Private Sub GridView4_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView4.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(0).Font.Bold = True  '.BackColor = Color.FromArgb(152, 203, 203)
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub

    Private Sub btnBalanceSumsRowsCols1a_Click(sender As Object, e As EventArgs) Handles btnBalanceSumsRowsCols1a.Click
        '(1a) Balancing matrix of field1 for given above sums by rows and by columns
        'DropDownList1 - field values as rows
        'DropDownList2 - field values columns
        'DropDownList3 - field1
        'DropDownList4 - field1 aggregation function
        'TextBoxSumsByRows
        'TextBoxSumsByCols
        mainGrids.Visible = True
        LabelError.Text = ""
        LabelResult.Text = ""
        If DropDownList1.Text.Trim = "" OrElse DropDownList2.Text.Trim = "" OrElse DropDownList3.Text.Trim = "" OrElse TextBoxSumsByRows.Text.Trim = "" OrElse TextBoxSumsByCols.Text.Trim = "" Then
            LabelError.Text = "Matrix field1 and sums by rows and columns must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString
        Dim x2 As String = DropDownList2.SelectedValue.ToString
        Dim fn As String = DropDownList4.SelectedValue.ToString
        Dim y1 As String = DropDownList3.SelectedValue.ToString
        Label12.Text = "Starting Matrix of " & fn & " of " & y1
        Label13.Text = "Target Matrix of proportional to requested sums by rows and columns of " & y1
        Label14.Text = "Balancing Matrix of " & fn & " of " & y1

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        Dim cst() As String = TextBoxSumsByRows.Text.Trim.Split(",")
        Dim dst() As String = TextBoxSumsByCols.Text.Trim.Split(",")
        Dim c(cst.Length - 1) As Double
        Dim d(dst.Length - 1) As Double
        Dim smb As Double = 0
        Dim dmb As Double = 0
        For i = 0 To cst.Length - 1
            c(i) = Convert.ToDecimal(cst(i))
            smb = smb + c(i)
        Next
        For j = 0 To dst.Length - 1
            d(j) = Convert.ToDecimal(dst(j))
            dmb = dmb + d(j)
        Next
        'correct total sums
        If smb <> dmb Then
            For j = 0 To dst.Length - 1
                d(j) = d(j) * smb / dmb
            Next
        End If

        'Starting matrix
        Dim dstart As DataTable
        dstart = ComputeStats(dv3.Table, fn, y1, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))
        dstart.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
        dstart = dstart.DefaultView.ToTable

        ret = MakeRowsColumnsArraysFromDataTables(dstart, dstart, x1, x2, "ARR", ax1vals, ax2vals)
        If ax1vals.Length <> c.Length Or ax2vals.Length <> d.Length Then
            LabelError.Text = "Arrays of sums by rows and columns shoud have the same dimensions as Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If
        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        Dim a(,) As Double = MakeMatrixFromDataTable(dstart, x1, x2, "ARR", ax1vals, ax2vals)
        Dim dtstrt As DataTable = MakeDataTableFromMatrix(a, x1, x2, y1, ax1vals, ax2vals, er, fn)

        Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
        Dim dtk As New DataTable

        'balancing
        Dim sma As Double = 0
        er = AdjustSumsByMatrix(a, c, d, sma)

        'a - starting array, c - sums for rows, d -sums for columns, x - balanced array, dtk - coefficients, l -number of steps, q - precision
        ret = CanBalanceMatrixToColRowSums(a, c, d, l, q, er, x, dtk, u, v)

        'adjust to original target sums or starting ones
        If Not chkAdjustByStart.Checked Then  'adjust by target sums

            ret = ret & AdjustMatrixByOverallSum(x, smb)

            'restore original sums
            For i = 0 To cst.Length - 1
                c(i) = c(i) * (smb / sma)
            Next
            For j = 0 To dst.Length - 1
                d(j) = d(j) * (smb / sma)
            Next

        Else 'chkAdjustByStart.Checked adjust by starting matrix sums

            ret = ret & AdjustMatrixByOverallSum(x, sma)

        End If

        Dim dtbal As DataTable = MakeDataTableFromMatrix(x, x1, x2, y1, ax1vals, ax2vals, er, fn)

        'Target matrix
        Dim dttrgt As DataTable
        dttrgt = MakeDataTableFromSumsOfRowsCols(a, c, d, x1, x2, y1, ax1vals, ax2vals, er, fn)

        LabelResult.Text = ret

        If Not dtstrt Is Nothing Then
            GridView1.DataSource = dtstrt.DefaultView
            GridView1.DataBind()
            GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
            For i = 0 To ax1vals.Count - 1
                GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
            Next
            For j = 0 To ax2vals.Count - 1
                GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
            Next
        End If
        If Not dttrgt Is Nothing Then
            GridView2.DataSource = dttrgt.DefaultView
            GridView2.DataBind()
            GridView2.Rows(GridView2.Rows.Count - 1).Font.Bold = True
        End If
        If Not dtbal Is Nothing Then
            GridView3.DataSource = dtbal.DefaultView
            GridView3.DataBind()
            GridView3.Rows(GridView3.Rows.Count - 1).Font.Bold = True
        End If

        'colors and tooltips
        Dim cl As Integer = 100
        Dim clr As Integer = 50
        Dim bg As String = String.Empty
        For i = 0 To GridView2.Rows.Count - 1
            For j = 0 To GridView2.Rows(0).Cells.Count - 1
                If u > 0 AndAlso v > 0 Then
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Ridge
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue

                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2

                    End If
                    If i > u - 1 AndAlso i < GridView2.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2
                    End If
                End If

                'target-ballanced
                If GridView2.Rows(i).Cells(j).Text.Trim <> "" AndAlso GridView2.Rows(i).Cells(j).Text.Trim <> "&nbsp;" AndAlso GridView3.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to balanced"
                            Else
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to balanced"
                            End If
                        Else
                            GridView3.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = "<1% difference compare to balanced"
                        End If
                    End If
                ElseIf GridView2.Rows(i).Cells(j).Text.Trim <> "" AndAlso GridView2.Rows(i).Cells(j).Text.Trim <> "&nbsp;" AndAlso GridView3.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView3.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = "no difference compare to balanced"
                End If
                'starting-target
                If (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"

                            Else
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
                'starting-ballanced
                If (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text <> GridView3.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView3.Rows(i).Cells(j).Text) Then
                        cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView3.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to balanced"
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"
                            Else
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to balanced"
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". <1% difference compare to balanced"
                            GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text = GridView3.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". no difference compare to balanced"
                    GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
            Next
        Next

        'last row in coefficients
        If Not dtk Is Nothing Then
            GridView4.DataSource = dtk.DefaultView
            GridView4.DataBind()
            If dtk.Rows.Count > 0 Then
                GridView4.Rows(GridView4.Rows.Count - 1).Font.Bold = True
                For i = 0 To GridView4.Rows.Count - 1
                    GridView4.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                Next
                Dim z As Integer = 0
                For i = 0 To GridView1.Rows.Count - 2
                    GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                    z = i + 1
                Next
                z = z + 2
                For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                    GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
                Next
            End If
        End If

        Session("GridView1DataSource") = dtstrt
        Session("GridView2DataSource") = dttrgt
        Session("GridView3DataSource") = dtbal
        Session("GridView4DataSource") = dtk

        'GridView5,6 for whole matrix balancing
        If u > 0 AndAlso v > 0 Then
            Dim y(ax1vals.Length - 1, ax2vals.Length - 1) As Double
            Dim dwk As New DataTable

            'balancing
            er = AdjustSumsByMatrix(a, c, d, sma)
            ret = CanBalanceMatrixToColRowSums(a, c, d, l, q, er, y, dwk, 0, 0)

            'adjust to original target
            If Not chkAdjustByStart.Checked Then  'adjust by target sums

                ret = ret & AdjustMatrixByOverallSum(y, smb)

                'restore original sums
                For i = 0 To cst.Length - 1
                    c(i) = c(i) * (smb / sma)
                Next
                For j = 0 To dst.Length - 1
                    d(j) = d(j) * (smb / sma)
                Next
            Else 'chkAdjustByStart.Checked adjust by starting matrix sums

                ret = ret & AdjustMatrixByOverallSum(y, sma)

            End If

            Dim dtwbal As DataTable = MakeDataTableFromMatrix(y, x1, x2, y1, ax1vals, ax2vals, er, fn)




            If Not dtwbal Is Nothing Then
                GridView5.DataSource = dtwbal.DefaultView
                GridView5.DataBind()
                GridView5.Rows(GridView3.Rows.Count - 1).Font.Bold = True
            End If

            'last row in coefficients
            If Not dwk Is Nothing Then
                GridView6.DataSource = dwk.DefaultView
                GridView6.DataBind()
                If dwk.Rows.Count > 0 Then
                    GridView6.Rows(GridView6.Rows.Count - 1).Font.Bold = True
                    For i = 0 To GridView6.Rows.Count - 1
                        GridView6.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                    Next
                End If
            End If

            'colors
            cl = 101
            For i = 0 To GridView3.Rows.Count - 1
                For j = 0 To GridView3.Rows(0).Cells.Count - 1
                    'partially ballanced <=> ballanced
                    If GridView3.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to partially balanced"
                                Else
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to partially balanced"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = "<1% difference compare to partially balanced"
                            End If
                        End If
                    ElseIf GridView3.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = "no difference compare to partially balanced"
                    End If

                    'target <=> balanced
                    If GridView2.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView2.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView2.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView2.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView2.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to target"
                                Else
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to target"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". <1% difference compare to target"
                            End If
                        End If
                    ElseIf GridView2.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". no difference compare to target"
                    End If
                Next
            Next
            Dim mx As Decimal = 0
            For i = 0 To ax1vals.Length - 1
                For j = 0 To ax2vals.Length - 1
                    Try
                        mx = Max(mx, Abs(x(i, j) - y(i, j)))
                    Catch ex As Exception

                    End Try

                Next
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = "Compare Partially Balanced Matrix with Balaced Whole Matrix: maximum difference of cells in partially balancinging and whole balancing matrixs = " & mx.ToString

            'compare partly balanced coefficients and whole balanced, maximum cell difference
            mx = 0
            For i = 1 To ax1vals.Length
                Try
                    mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
                Catch ex As Exception

                End Try

            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging rows coefficients and whole balancing rows coefficients  = " & mx.ToString

            mx = 0
            For i = ax1vals.Length + 2 To ax1vals.Length + ax2vals.Length + 1
                Try
                    mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
                Catch ex As Exception

                End Try

            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging columns coefficients and whole balancing columns coefficients  = " & mx.ToString

            For i = 1 To ax1vals.Length + ax2vals.Length + 1
                If dtk(dtk.Rows.Count - 1)(i) <> dwk(dwk.Rows.Count - 1)(i) Then
                    If IsNumeric(dtk(dtk.Rows.Count - 1)(i)) AndAlso IsNumeric(dwk(dwk.Rows.Count - 1)(i)) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(dtk(dtk.Rows.Count - 1)(i)) - CDbl(dwk(dwk.Rows.Count - 1)(i))) / Max(1, Max(CDbl(dtk(dtk.Rows.Count - 1)(i)), CDbl(dwk(dwk.Rows.Count - 1)(i)))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If dtk(dtk.Rows.Count - 1)(i) > dwk(dwk.Rows.Count - 1)(i) Then
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " -" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                            Else
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " +" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                            End If
                        Else
                            GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " <1% difference compare to partially balancing coefficients"
                        End If
                    End If
                ElseIf dtk(dtk.Rows.Count - 1)(i) = dwk(dwk.Rows.Count - 1)(i) Then
                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " no difference compare to partially balancing coefficients"
                End If

            Next
            Session("GridView5DataSource") = dtwbal
            Session("GridView6DataSource") = dwk
        End If

        dv3 = Session("dv3")
        dv3.RowFilter = ""

        Label2.Text = "Balancing for sum of rows and columns of the starting matrix for " & fn.ToLower & " values of field1 '" & y1 & "' : "

    End Sub

    Private Sub btnGetCoeffsByFields3a_Click(sender As Object, e As EventArgs) Handles btnGetCoeffsByFields3a.Click
        '(3a) Balancing coefficients for matrix of field1 values for multiple fields sums by rows and by columns
        'DropDownList1 - field values as rows
        'DropDownList2 - field values columns
        'DropDownList3 - field1
        'DropDownList4 - field1 aggregation function
        'DropDownColumns - multiple columns
        'DropDownList10 - multiple columns aggregation function

        mainGrids.Visible = True
        GridView2.Visible = False
        GridView3.Visible = False
        GridView5.Visible = False
        GridView6.Visible = False

        Label13.Text = ""
        Label14.Text = ""
        Label19.Text = ""
        Label15.Text = ""
        Label20.Text = ""
        LabelCompare.Text = ""
        LabelError.Text = ""
        LabelResult.Text = "Waiting..."
        Label2.Text = ""
        lnkExportGrid2.Visible = False
        lnkExportGrid2.Enabled = False
        lnkExportGrid3.Visible = False
        lnkExportGrid3.Enabled = False
        lnkExportGrid5.Visible = False
        lnkExportGrid5.Enabled = False
        lnkExportGrid6.Visible = False
        lnkExportGrid6.Enabled = False
        If DropDownList1.Text.Trim = "" OrElse DropDownList2.Text.Trim = "" OrElse DropDownList3.Text.Trim = "" Then
            LabelError.Text = "Matrix fields must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString  'group field for matrix rows
        Dim x2 As String = DropDownList2.SelectedValue.ToString  'group field for matrix columns
        Dim fn As String = DropDownList4.SelectedValue.ToString  'aggregation function for field1
        Dim y1 As String = DropDownList3.SelectedValue.ToString  'field1
        Dim y2 As String = DropDownColumns.Text  'multi columns
        Dim fnf2 As String = DropDownList10.SelectedValue.ToString 'aggregation function for fields2
        Dim n As Integer = 0
        Dim dtkr As New DataTable
        Dim c As Boolean = False

        Label12.Text = "Starting Matrix of " & fn & " of " & y1

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Dim fltr As String = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        dv3.RowFilter = fltr

        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        ret = MakeRowsColumnsArraysFromDataView(dv3.Table, x1, x2, "ARR", ax1vals, ax2vals)

        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        Dim dstart As DataTable
        dstart = ComputeStats(dv3.ToTable, fn, y1, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))
        dstart.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
        dstart = dstart.DefaultView.ToTable

        'loop for target matrixs
        For n = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(n).Selected Then
                y2 = DropDownColumns.Items(n).Text
                Dim dtarget As DataTable
                dtarget = ComputeStats(dv3.ToTable, fnf2, y2, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))
                dtarget.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
                dtarget = dtarget.DefaultView.ToTable

                Dim a(,) As Double = MakeMatrixFromDataTable(dstart, x1, x2, "ARR", ax1vals, ax2vals)
                Dim b(,) As Double = MakeMatrixFromDataTable(dtarget, x1, x2, "ARR", ax1vals, ax2vals)
                Dim dtstrt As DataTable = MakeDataTableFromMatrix(a, x1, x2, y1, ax1vals, ax2vals, er, fn)
                Dim dttrgt As DataTable = MakeDataTableFromMatrix(b, x1, x2, y2, ax1vals, ax2vals, er, fnf2)
                Dim sma As Decimal = 0  'overall sum for target matrix
                For i = 0 To ax1vals.Length - 1
                    For j = 0 To ax2vals.Length - 1
                        sma = sma + a(i, j)
                    Next
                Next
                Dim smb As Decimal = 0  'overall sum for target matrix
                For i = 0 To ax1vals.Length - 1
                    For j = 0 To ax2vals.Length - 1
                        smb = smb + b(i, j)
                    Next
                Next

                Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
                Dim dtk As New DataTable

                'balancing
                ret = AdjustMatrixByOverallSum(b, sma)
                ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, x, dtk, u, v)

                'adjust to original target
                ret = ret & AdjustMatrixByOverallSum(b, smb)
                If chkAdjustByStart.Checked = False Then  'adjust by target sums
                    ret = ret & AdjustMatrixByOverallSum(x, smb)
                End If

                Dim dtbal As DataTable = MakeDataTableFromMatrix(x, x1, x2, y2, ax1vals, ax2vals, er, fnf2)

                If Not c Then
                    If Not dtstrt Is Nothing Then
                        GridView1.DataSource = dtstrt.DefaultView
                        GridView1.DataBind()
                        GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
                        For i = 0 To ax1vals.Count - 1
                            GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
                        Next
                        For j = 0 To ax2vals.Count - 1
                            GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
                        Next
                    End If
                    c = True
                End If

                Session("GridView1DataSource") = dtstrt

                'last row in coefficients dtk add to dtkr
                If Not dtk Is Nothing Then
                    If dtkr Is Nothing OrElse dtkr.Rows Is Nothing OrElse dtkr.Rows.Count = 0 Then
                        dtkr = dtk
                        If dtkr.Rows.Count > 1 Then
                            For j = 0 To dtkr.Rows.Count - 2
                                dtkr.Rows(0).Delete()
                            Next
                        End If
                        Dim s As Integer = dtkr.Rows.Count
                        dtkr.Rows(s - 1)(0) = y2
                    Else
                        'dtkr.Rows.Add(dtkrow)
                        Dim dtkrow As DataRow = dtkr.Rows.Add ' dtk.Rows(dtk.Rows.Count - 1)
                        Dim s As Integer = dtkr.Rows.Count
                        dtkr.Rows(s - 1)(0) = y2
                        For j = 1 To dtk.Columns.Count - 1
                            dtkr.Rows(s - 1)(j) = dtk.Rows(dtk.Rows.Count - 1)(j)
                        Next
                    End If
                End If
            End If
        Next


        Session("GridView4DataSource") = dtkr

        'color partly balaced grid
        If u > 0 AndAlso v > 0 Then
            For i = 0 To GridView1.Rows.Count - 1
                For j = 0 To GridView1.Rows(0).Cells.Count - 1
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                    End If
                    If i > u - 1 AndAlso i < GridView1.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                    End If
                Next
            Next
        End If

        Dim cl As Double = 0
        'Dim clr As Integer = 50
        'Dim bg As String = String.Empty
        Dim z As Integer = 0
        'last rows in coefficients
        If Not dtkr Is Nothing Then
            GridView4.DataSource = dtkr.DefaultView
            GridView4.DataBind()
            GridView4.HeaderRow.Cells(0).ToolTip = "Balancing by fields:"
            For i = 0 To GridView1.Rows.Count - 2
                GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                z = i + 1
            Next
            z = z + 2
            For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
            Next
            For i = 0 To GridView4.Rows.Count - 1
                For j = 0 To GridView4.Rows(i).Cells.Count - 1
                    If GridView4.Rows(i).Cells(j).Text.Trim = "" Then
                        GridView4.Rows(i).Cells(j).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                    Else
                        If i > 0 AndAlso GridView4.Rows(i).Cells(j).Text > GridView4.Rows(i - 1).Cells(j).Text Then
                            If IsNumeric(GridView4.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView4.Rows(i - 1).Cells(j).Text) Then
                                cl = Round(Abs(CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)), 5)
                                'GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - 60, 255 - 100, 179 - 60)
                                'cl = CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)
                                GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(191, 255, 179)
                                GridView4.Rows(i).Cells(j).ToolTip = "+" & cl.ToString & " compare to previous row"

                            End If
                        ElseIf i > 0 AndAlso GridView4.Rows(i).Cells(j).Text < GridView4.Rows(i - 1).Cells(j).Text Then
                            If IsNumeric(GridView4.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView4.Rows(i - 1).Cells(j).Text) Then
                                cl = Round(Abs(CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)), 5)
                                'GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - 60, 207 - 60, 207 - 60)
                                'cl = CDbl(GridView4.Rows(i - 1).Cells(j).Text) - CDbl(GridView4.Rows(i).Cells(j).Text)
                                GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(207, 207, 207)
                                GridView4.Rows(i).Cells(j).ToolTip = "-" & cl.ToString & " compare to previous row"

                            End If
                        ElseIf i > 0 AndAlso GridView4.Rows(i).Cells(j).Text = GridView4.Rows(i - 1).Cells(j).Text Then
                            GridView4.Rows(i).Cells(j).ToolTip = "no difference compare to previous row"
                        End If
                    End If
                Next
            Next

        End If
        LabelResult.Text = "Done!"

        dv3 = Session("dv3")
        dv3.RowFilter = ""

        Session("ttl") = "Starting Matrix with Balancing Values"
        ret = ChartLink(dtkr, DropDownList3.SelectedItem.Text & " Starting value")
    End Sub

    Private Sub btnGetCoeffsByVals2c_Click(sender As Object, e As EventArgs) Handles btnGetCoeffsByVals2c.Click
        '(2c) Balancing coefficients for matrix of field1 values and all itterations between starting and target of the field2 values
        'DropDownList1 - field values as rows
        'DropDownList2 - field values columns
        'DropDownList3 - field1
        'DropDownList4 - field1 aggregation function
        'DropDownList5 - field2
        'DropDownList6 - starting value of field2
        'DropDownList7 - target value of field2

        mainGrids.Visible = True
        GridView2.Visible = False
        GridView3.Visible = False
        GridView5.Visible = False
        GridView6.Visible = False
        Label13.Text = ""
        Label14.Text = ""
        Label19.Text = ""
        Label15.Text = ""
        Label20.Text = ""
        LabelCompare.Text = ""
        LabelError.Text = ""
        LabelResult.Text = "Waiting..."
        Label2.Text = ""
        lnkExportGrid2.Visible = False
        lnkExportGrid2.Enabled = False
        lnkExportGrid3.Visible = False
        lnkExportGrid3.Enabled = False
        lnkExportGrid5.Visible = False
        lnkExportGrid5.Enabled = False
        lnkExportGrid6.Visible = False
        lnkExportGrid6.Enabled = False

        If DropDownList1.Text.Trim = "" OrElse DropDownList2.Text.Trim = "" OrElse DropDownList3.Text.Trim = "" Then
            LabelError.Text = "Matrix fields must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString  'group field for matrix rows
        Dim x2 As String = DropDownList2.SelectedValue.ToString  'group field for matrix columns
        Dim fn As String = DropDownList4.SelectedValue.ToString  'aggregation function for field1
        Dim y1 As String = DropDownList3.SelectedValue.ToString  'field1
        'Dim y2 As String = DropDownColumns.Text  'fields2
        'Dim fnf2 As String = DropDownList10.SelectedValue.ToString 'aggregation function for fields2
        Dim fld As String = DropDownList5.SelectedValue.ToString
        Dim sval As String = DropDownList6.SelectedValue.ToString
        Dim tval As String = DropDownList7.SelectedValue.ToString
        Dim fnf2 As String = DropDownList4.SelectedValue.ToString  'no sense to have different ones
        'Label12.Text = "Starting Matrix of " & fn & " of " & y1
        Label12.Text = "Starting Matrix of " & fn & " of " & y1 & " where " & fld & "='" & sval & "'"
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Dim fltr As String = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        If fltr <> "" Then
            fltr = fltr & " AND "
        End If
        ' if numeric
        If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
            fltr = fltr & " NOT " & fld & " <" & sval & " AND NOT " & fld & ">" & tval
        Else
            fltr = fltr & " NOT " & fld & " <'" & sval & "' AND NOT " & fld & ">'" & tval & "'"
        End If
        dv3.RowFilter = fltr
        dv3.Sort = x1 & " ASC," & x2 & " ASC"

        'TODO make matrixs the same dimensions !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        ret = MakeRowsColumnsArraysFromDataView(dv3.Table, x1, x2, "ARR", ax1vals, ax2vals)

        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        Dim dstart As DataTable
        'dstart = ComputeStats(dv3.ToTable, fn, y1, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))
        dstart = ComputeStats(dv3.Table, fn, y1, x1, x2, fld & "='" & sval & "'", ret, Session("UserConnString"), Session("UserConnProvider"))
        dstart.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
        dstart = dstart.DefaultView.ToTable

        Dim n As Integer = 0
        Dim dtkr As New DataTable
        Dim c As Boolean = False
        'loop for target matrixs defined by values of field2
        Dim val2arr(0) As String
        Dim val2 As String = String.Empty
        '!!!!!!!!!!!!!!!!!!  make array of fld values between sval and tval
        val2arr(0) = sval
        For i = 1 To DropDownList6.Items.Count - 1
            If DropDownList6.Items(i).Value > sval AndAlso DropDownList6.Items(i).Value <= tval Then
                n = val2arr.Length
                ReDim Preserve val2arr(n)
                val2arr(n) = DropDownList6.Items(i).Value
            End If
        Next
        n = 0
        For n = 0 To val2arr.Length - 1
            val2 = val2arr(n)
            If val2.Trim <> "" Then
                Dim dtarget As DataTable
                'dtarget = ComputeStats(dv3.ToTable, fnf2, y2, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))
                dtarget = ComputeStats(dv3.Table, fnf2, y1, x1, x2, fld & "='" & val2 & "'", ret, Session("UserConnString"), Session("UserConnProvider"))
                dtarget.DefaultView.Sort = x1 & " ASC," & x2 & " ASC"
                dtarget = dtarget.DefaultView.ToTable

                Dim a(,) As Double = MakeMatrixFromDataTable(dstart, x1, x2, "ARR", ax1vals, ax2vals)
                Dim b(,) As Double = MakeMatrixFromDataTable(dtarget, x1, x2, "ARR", ax1vals, ax2vals)

                Dim dtstrt As DataTable = MakeDataTableFromMatrix(a, x1, x2, y1, ax1vals, ax2vals, er, fn)
                Dim dttrgt As DataTable = MakeDataTableFromMatrix(b, x1, x2, y1, ax1vals, ax2vals, er, fnf2)

                Dim sma As Decimal = 0  'overall sum for target matrix
                For i = 0 To ax1vals.Length - 1
                    For j = 0 To ax2vals.Length - 1
                        sma = sma + a(i, j)
                    Next
                Next
                Dim smb As Decimal = 0  'overall sum for target matrix
                For i = 0 To ax1vals.Length - 1
                    For j = 0 To ax2vals.Length - 1
                        smb = smb + b(i, j)
                    Next
                Next

                Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
                Dim dtk As New DataTable

                'balancing
                ret = AdjustMatrixByOverallSum(b, sma)
                ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, x, dtk, u, v)

                'adjust to original target
                ret = ret & AdjustMatrixByOverallSum(b, smb)
                If chkAdjustByStart.Checked = False Then  'adjust by target sums
                    ret = ret & AdjustMatrixByOverallSum(x, smb)
                End If

                Dim dtbal As DataTable = MakeDataTableFromMatrix(x, x1, x2, y1, ax1vals, ax2vals, er, fnf2)

                If Not c Then
                    If Not dtstrt Is Nothing Then
                        GridView1.DataSource = dtstrt.DefaultView
                        GridView1.DataBind()
                        GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
                        For i = 0 To ax1vals.Count - 1
                            GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
                        Next
                        For j = 0 To ax2vals.Count - 1
                            GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
                        Next
                    End If
                    c = True
                End If

                Session("GridView1DataSource") = dtstrt

                'last row in coefficients dtk add to dtkr
                If Not dtk Is Nothing AndAlso dtk.Rows.Count > 0 Then
                    If dtkr Is Nothing OrElse dtkr.Rows Is Nothing OrElse dtkr.Rows.Count = 0 Then
                        dtkr = dtk
                        If dtkr.Rows.Count > 1 Then
                            For j = 0 To dtkr.Rows.Count - 2
                                dtkr.Rows(0).Delete()
                            Next
                        End If
                        Dim s As Integer = dtkr.Rows.Count
                        dtkr.Rows(s - 1)(0) = val2
                    Else
                        'dtkr.Rows.Add(dtkrow)
                        Dim dtkrow As DataRow = dtkr.Rows.Add ' dtk.Rows(dtk.Rows.Count - 1)
                        'update dtkr by dtk adding columns

                        Dim s As Integer = dtkr.Rows.Count
                        dtkr.Rows(s - 1)(0) = val2
                        For j = 1 To dtk.Columns.Count - 1
                            dtkr.Rows(s - 1)(j) = dtk.Rows(dtk.Rows.Count - 1)(j)
                        Next
                    End If
                End If
            End If
        Next

        'color partly balaced grid
        If u > 0 AndAlso v > 0 Then
            For i = 0 To GridView1.Rows.Count - 1
                For j = 0 To GridView1.Rows(0).Cells.Count - 1
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                    End If
                    If i > u - 1 AndAlso i < GridView1.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                    End If
                Next
            Next

        End If

        Dim cl As Double = 0
        'Dim clr As Integer = 50
        'Dim bg As String = String.Empty
        Dim z As Integer = 0
        'last rows in coefficients
        If Not dtkr Is Nothing AndAlso dtkr.Rows.Count > 0 Then
            GridView4.DataSource = dtkr.DefaultView
            GridView4.DataBind()
            GridView4.HeaderRow.Cells(0).ToolTip = "Balancing by fields:"
            For i = 0 To GridView1.Rows.Count - 2
                GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                z = i + 1
            Next
            z = z + 2
            For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
            Next
            For i = 0 To GridView4.Rows.Count - 1
                For j = 0 To GridView4.Rows(i).Cells.Count - 1
                    If GridView4.Rows(i).Cells(j).Text.Trim = "" Then
                        GridView4.Rows(i).Cells(j).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                    Else
                        If i > 0 AndAlso GridView4.Rows(i).Cells(j).Text > GridView4.Rows(i - 1).Cells(j).Text Then
                            If IsNumeric(GridView4.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView4.Rows(i - 1).Cells(j).Text) Then
                                cl = Round(Abs(CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)), 5)
                                'GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - 60, 255 - 100, 179 - 60)
                                'cl = CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)
                                GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(191, 255, 179)
                                GridView4.Rows(i).Cells(j).ToolTip = "+" & cl.ToString & " compare to previous row"

                            End If
                        ElseIf i > 0 AndAlso GridView4.Rows(i).Cells(j).Text < GridView4.Rows(i - 1).Cells(j).Text Then
                            If IsNumeric(GridView4.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView4.Rows(i - 1).Cells(j).Text) Then
                                cl = Round(Abs(CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)), 5)
                                'GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - 60, 207 - 60, 207 - 60)
                                'cl = CDbl(GridView4.Rows(i - 1).Cells(j).Text) - CDbl(GridView4.Rows(i).Cells(j).Text)
                                GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(207, 207, 207)
                                GridView4.Rows(i).Cells(j).ToolTip = "-" & cl.ToString & " compare to previous row"

                            End If
                        ElseIf i > 0 AndAlso GridView4.Rows(i).Cells(j).Text = GridView4.Rows(i - 1).Cells(j).Text Then
                            GridView4.Rows(i).Cells(j).ToolTip = "no difference compare to previous row"
                        End If
                    End If
                Next
            Next
            Session("GridView4DataSource") = dtkr
        End If


        LabelResult.Text = "Done!"

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Session("ttl") = "Starting Matrix with Balancing Values"
        ret = ChartLink(dtkr, DropDownList3.SelectedItem.Text & " Starting value")
    End Sub

    Private Sub DropDownColumns_ChecklistChanged(sender As Object, e As Controls_uc1.ChecklistChangedArgs) Handles DropDownColumns.ChecklistChanged
        'depended of type
        DropDownList10.Items.Clear()
        DropDownList10.Items.Add("Count")
        DropDownList10.Items.Add("CountDistinct")
        Dim bNumeric As Boolean = True
        For i = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(i).Selected Then
                Session("SELECTEDFields") = Session("SELECTEDFields") & "," & DropDownColumns.Items(i).Text
                If Not ColumnTypeIsNumeric(dv3.Table.Columns(DropDownColumns.Items(i).Text)) Then
                    bNumeric = False
                    Exit For
                End If
            End If
        Next
        If bNumeric Then
            DropDownList10.Items.Add("Sum")
            DropDownList10.Items.Add("Max")
            DropDownList10.Items.Add("Min")
            DropDownList10.Items.Add("Avg")
            DropDownList10.Items.Add("StDev")
            DropDownList10.Items.Add("Value")
        End If
        If Not DropDownColumns.AllSelected Then
            CheckBoxSelectAllFields.Checked = False
        End If
    End Sub

    Private Sub btnGetCoeffsMatrixColumns3c_Click(sender As Object, e As EventArgs) Handles btnGetCoeffsMatrixColumns3c.Click
        '(3c) Balancing matrix of rows defined by values in the field1 and selected multiple fields as columns for all itterations between starting and target of the field2 values
        'DropDownList1 - field values as rows
        'DropDownList5 - field2
        'DropDownList6 - starting value of field2
        'DropDownList7 - target value of field2
        'DropDownColumns - multiple columns

        mainGrids.Visible = True
        GridView2.Visible = False
        GridView3.Visible = False
        GridView5.Visible = False
        GridView6.Visible = False
        Label13.Text = ""
        Label14.Text = ""
        Label19.Text = ""
        Label15.Text = ""
        Label20.Text = ""
        LabelCompare.Text = ""
        LabelError.Text = ""
        LabelResult.Text = "Waiting..."
        Label2.Text = ""
        lnkExportGrid2.Visible = False
        lnkExportGrid2.Enabled = False
        lnkExportGrid3.Visible = False
        lnkExportGrid3.Enabled = False
        lnkExportGrid5.Visible = False
        lnkExportGrid5.Enabled = False
        lnkExportGrid6.Visible = False
        lnkExportGrid6.Enabled = False

        If DropDownList1.Text.Trim = "" OrElse DropDownList5.Text.Trim = "" Then
            LabelError.Text = "Matrix fields must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString  'group field for matrix rows
        'Dim x2 As String = DropDownList2.SelectedValue.ToString  'group field for matrix columns
        'Dim fn As String = DropDownList10.SelectedValue.ToString  'aggregation function for selected fields
        'Dim y1 As String = DropDownList3.SelectedValue.ToString  'field1
        Dim y2 As String = DropDownColumns.Text  'fields2
        'Dim fnf2 As String = DropDownList10.SelectedValue.ToString 'aggregation function for fields2
        Dim fld As String = DropDownList5.SelectedValue.ToString
        Dim sval As String = DropDownList6.SelectedValue.ToString
        Dim tval As String = DropDownList7.SelectedValue.ToString
        'Dim fnf2 As String = DropDownList4.SelectedValue.ToString  'no sense to have different ones
        Label12.Text = "Starting Matrix of " & x1 & " and " & y2 & " for " & fld & " = " & sval.ToString

        'ax1vals, ax2vals
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Dim fltr As String = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        If fltr <> "" Then
            fltr = fltr & " AND "
        End If
        ' if numeric
        If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
            fltr = fltr & " NOT " & fld & " <" & sval & " AND NOT " & fld & ">" & tval
        Else
            fltr = fltr & " NOT " & fld & " <'" & sval & "' AND NOT " & fld & ">'" & tval & "'"
        End If
        dv3.RowFilter = fltr

        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        Dim n As Integer
        Dim i As Integer = 0
        For n = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(n).Selected Then
                ReDim Preserve ax2vals(i)
                ax2vals(i) = DropDownColumns.Items(n).Text
                i = i + 1
            End If
        Next
        ret = MakeRowsColumnsArraysFromMultiColumns(dv3.Table, x1, "", fld, ax1vals, ax2vals)

        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        fltr = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        'dv3 with filter fld & "='" & sval & "'"
        If fltr <> "" Then
            fltr = fltr & " AND "
        End If
        ' if numeric
        If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
            fltr = fltr & fld & "=" & sval
        Else
            fltr = fltr & fld & "='" & sval & "'"
        End If
        dv3.RowFilter = fltr
        er = ""
        Dim a(,) As Double = MakeMatrixFromDataTableMultiColumns(dv3.ToTable, x1, ax1vals, ax2vals, er)

        If er.Trim <> "" Then
            LabelError.Text = "ERROR: " & er
            Exit Sub
        End If


        Dim dtstrt As DataTable = MakeDataTableFromMatrixMultiColumns(a, x1, ax1vals, ax2vals, q, er)
        Session("GridView1DataSource") = dtstrt

        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        fltr = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        n = 0
        Dim dtkr As New DataTable
        Dim c As Boolean = False
        'loop for target matrixs defined by values of field2
        Dim val2arr(0) As String
        Dim val2 As String = String.Empty
        '!!!!!!!!!!!!!!!!!!  make array of fld values between sval and tval
        val2arr(0) = sval
        For i = 1 To DropDownList6.Items.Count - 1
            If DropDownList6.Items(i).Value > sval AndAlso DropDownList6.Items(i).Value <= tval Then
                n = val2arr.Length
                ReDim Preserve val2arr(n)
                val2arr(n) = DropDownList6.Items(i).Value
            End If
        Next
        n = 0
        For n = 0 To val2arr.Length - 1
            val2 = val2arr(n)
            If val2.Trim <> "" AndAlso val2.Trim <> sval Then

                'TODO dv3 with filter fld & " ='" & val2 & "'"
                If fltr <> "" Then
                    fltr = fltr & " AND "
                End If
                ' if numeric
                If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
                    fltr = fltr & fld & "=" & val2
                Else
                    fltr = fltr & fld & "='" & val2 & "'"
                End If
                dv3.RowFilter = fltr

                er = ""
                Dim b(,) As Double = MakeMatrixFromDataTableMultiColumns(dv3.ToTable, x1, ax1vals, ax2vals, er)

                If er.Trim <> "" Then
                    LabelError.Text = "ERROR: " & er
                    Exit Sub
                End If

                'restore dv3
                dv3 = Session("dv3")
                dv3.RowFilter = ""
                fltr = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim


                Dim dttrgt As DataTable = MakeDataTableFromMatrixMultiColumns(b, x1, ax1vals, ax2vals, q, er)

                Dim sma As Decimal = 0  'overall sum for target matrix
                For i = 0 To ax1vals.Length - 1
                    For j = 0 To ax2vals.Length - 1
                        sma = sma + a(i, j)
                    Next
                Next
                Dim smb As Decimal = 0  'overall sum for target matrix
                For i = 0 To ax1vals.Length - 1
                    For j = 0 To ax2vals.Length - 1
                        smb = smb + b(i, j)
                    Next
                Next

                Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
                Dim dtk As New DataTable

                'balancing
                ret = AdjustMatrixByOverallSum(b, sma)
                ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, x, dtk, u, v)

                'adjust to original target
                ret = ret & AdjustMatrixByOverallSum(b, smb)
                If chkAdjustByStart.Checked = False Then  'adjust by target sums
                    ret = ret & AdjustMatrixByOverallSum(x, smb)
                End If

                Dim dtbal As DataTable = MakeDataTableFromMatrixMultiColumns(x, x1, ax1vals, ax2vals, q, er)

                If Not c Then
                    If Not dtstrt Is Nothing Then
                        GridView1.DataSource = dtstrt.DefaultView
                        GridView1.DataBind()
                        GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
                        For i = 0 To ax1vals.Count - 1
                            GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
                        Next
                        For j = 0 To ax2vals.Count - 1
                            GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
                        Next
                    End If
                    c = True
                End If

                'last row in coefficients dtk add to dtkr
                If Not dtk Is Nothing AndAlso dtk.Rows.Count > 0 Then
                    If dtkr Is Nothing OrElse dtkr.Rows Is Nothing OrElse dtkr.Rows.Count = 0 Then
                        dtkr = dtk
                        If dtkr.Rows.Count > 1 Then
                            For j = 0 To dtkr.Rows.Count - 2
                                dtkr.Rows(0).Delete()
                            Next
                        End If
                        Dim s As Integer = dtkr.Rows.Count
                        dtkr.Rows(s - 1)(0) = val2
                    Else
                        'dtkr.Rows.Add(dtkrow)
                        Dim dtkrow As DataRow = dtkr.Rows.Add ' dtk.Rows(dtk.Rows.Count - 1)
                        'update dtkr by dtk adding columns

                        Dim s As Integer = dtkr.Rows.Count
                        dtkr.Rows(s - 1)(0) = val2
                        For j = 1 To dtk.Columns.Count - 1
                            dtkr.Rows(s - 1)(j) = dtk.Rows(dtk.Rows.Count - 1)(j)
                        Next
                    End If
                End If
            End If
        Next

        'color partly balaced grid
        If u > 0 AndAlso v > 0 Then
            For i = 0 To GridView1.Rows.Count - 1
                For j = 0 To GridView1.Rows(0).Cells.Count - 1
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                    End If
                    If i > u - 1 AndAlso i < GridView1.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                    End If
                Next
            Next
        End If

        Try


            Dim cl As Double = 0
            'Dim clr As Integer = 50
            'Dim bg As String = String.Empty
            Dim z As Integer = 0
            'last rows in coefficients
            If Not dtkr Is Nothing Then
                GridView4.DataSource = dtkr.DefaultView
                GridView4.DataBind()
                GridView4.HeaderRow.Cells(0).ToolTip = "Balancing by fields:"
                For i = 0 To GridView1.Rows.Count - 2
                    GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                    z = i + 1
                Next
                z = z + 2
                For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                    GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
                Next
                For i = 0 To GridView4.Rows.Count - 1
                    For j = 0 To GridView4.Rows(i).Cells.Count - 1
                        If GridView4.Rows(i).Cells(j).Text.Trim = "" Then
                            GridView4.Rows(i).Cells(j).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                        Else
                            If i > 0 AndAlso GridView4.Rows(i).Cells(j).Text > GridView4.Rows(i - 1).Cells(j).Text Then
                                If IsNumeric(GridView4.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView4.Rows(i - 1).Cells(j).Text) Then
                                    cl = Round(Abs(CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)), 5)
                                    'GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - 60, 255 - 100, 179 - 60)
                                    'cl = CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)
                                    GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(191, 255, 179)
                                    GridView4.Rows(i).Cells(j).ToolTip = "+" & cl.ToString & " compare to previous row"

                                End If
                            ElseIf i > 0 AndAlso GridView4.Rows(i).Cells(j).Text < GridView4.Rows(i - 1).Cells(j).Text Then
                                If IsNumeric(GridView4.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView4.Rows(i - 1).Cells(j).Text) Then
                                    cl = Round(Abs(CDbl(GridView4.Rows(i).Cells(j).Text) - CDbl(GridView4.Rows(i - 1).Cells(j).Text)), 5)
                                    'GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - 60, 207 - 60, 207 - 60)
                                    'cl = CDbl(GridView4.Rows(i - 1).Cells(j).Text) - CDbl(GridView4.Rows(i).Cells(j).Text)
                                    GridView4.Rows(i).Cells(j).BackColor = Color.FromArgb(207, 207, 207)
                                    GridView4.Rows(i).Cells(j).ToolTip = "-" & cl.ToString & " compare to previous row"

                                End If
                            ElseIf i > 0 AndAlso GridView4.Rows(i).Cells(j).Text = GridView4.Rows(i - 1).Cells(j).Text Then
                                GridView4.Rows(i).Cells(j).ToolTip = "no difference compare to previous row"
                            End If
                        End If
                    Next
                Next
                Session("GridView4DataSource") = dtkr
            End If
            LabelResult.Text = "Done!"

            dv3 = Session("dv3")
            dv3.RowFilter = ""

            ' Session("ttl") = "Starting Matrix with Balancing Values"
            ret = ChartLink(dtkr, DropDownList6.SelectedItem.Text & " Starting value")

        Catch ex As Exception
            er = ex.Message
        End Try
    End Sub

    Private Sub btnBalanceMatrixColumns3b_Click(sender As Object, e As EventArgs) Handles btnBalanceMatrixColumns3b.Click
        '(3b) Balancing matrix of rows defined by values in the group field for rows and selected multiple fields as columns between starting and target values of the field2
        'DropDownList1 - field values as rows
        'DropDownList5 - field2
        'DropDownList6 - starting value of field2
        'DropDownList7 - target value of field2
        'DropDownColumns - multiple columns
        mainGrids.Visible = True
        GridView2.Visible = True
        GridView3.Visible = True
        GridView5.Visible = True
        GridView6.Visible = True
        lnkExportGrid2.Visible = True
        lnkExportGrid2.Enabled = True
        lnkExportGrid3.Visible = True
        lnkExportGrid3.Enabled = True
        lnkExportGrid5.Visible = True
        lnkExportGrid5.Enabled = True
        lnkExportGrid6.Visible = True
        lnkExportGrid6.Enabled = True

        Label12.Text = "Starting Matrix"
        Label13.Text = "Target Matrix"
        Label14.Text = "Balancing Matrix"

        Label19.Text = ""
        Label15.Text = ""
        Label20.Text = ""
        LabelCompare.Text = ""
        LabelError.Text = ""
        LabelResult.Text = "Waiting..."
        Label2.Text = ""

        If DropDownList1.Text.Trim = "" OrElse DropDownList5.Text.Trim = "" Then
            LabelError.Text = "Matrix fields must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If
        Dim dtkr As New DataTable
        Dim c As Boolean = False

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString  'group field for matrix rows
        Dim y2 As String = DropDownColumns.Text  'fields2
        Dim fld As String = DropDownList5.SelectedValue.ToString
        Dim sval As String = DropDownList6.SelectedValue.ToString
        Dim tval As String = DropDownList7.SelectedValue.ToString
        'Dim fnf2 As String = DropDownList4.SelectedValue.ToString  'no sense to have different ones
        'Label12.Text = "Starting Matrix of " & x1 & " and " & y2 & " for " & fld & " = " & sval.ToString
        Label12.Text = "Starting Matrix for " & fld & " = " & sval.ToString
        Label13.Text = "Target Matrix for " & fld & "='" & tval & "'"
        'ax1vals, ax2vals
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Dim fltr As String = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        If fltr <> "" Then
            fltr = fltr & " AND "
        End If
        ' if numeric
        If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
            fltr = fltr & " NOT " & fld & " <" & sval & " AND NOT " & fld & ">" & tval
        Else
            fltr = fltr & " NOT " & fld & " <'" & sval & "' AND NOT " & fld & ">'" & tval & "'"
        End If
        dv3.RowFilter = fltr

        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        Dim n As Integer
        Dim i As Integer = 0
        For n = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(n).Selected Then
                ReDim Preserve ax2vals(i)
                ax2vals(i) = DropDownColumns.Items(n).Text
                i = i + 1
            End If
        Next
        ret = MakeRowsColumnsArraysFromMultiColumns(dv3.Table, x1, "", fld, ax1vals, ax2vals)

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        fltr = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        'Starting Matrix
        'dv3 with filter fld & "='" & sval & "'"
        If fltr <> "" Then
            fltr = fltr & " AND "
        End If
        ' if numeric
        If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
            fltr = fltr & fld & "=" & sval
        Else
            fltr = fltr & fld & "='" & sval & "'"
        End If
        dv3.RowFilter = fltr

        Dim a(,) As Double = MakeMatrixFromDataTableMultiColumns(dv3.ToTable, x1, ax1vals, ax2vals, er)

        If er.Trim <> "" Then
            LabelError.Text = "ERROR: " & er
            Exit Sub
        End If

        Dim dtstrt As DataTable = MakeDataTableFromMatrixMultiColumns(a, x1, ax1vals, ax2vals, q, er)

        'Target matrix
        'TODO dv3 with filter fld & " ='" & tval & "'"
        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        'dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        fltr = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        If fltr <> "" Then
            fltr = fltr & " AND "
        End If
        ' if numeric
        If ColumnTypeIsNumeric(dv3.Table.Columns(fld)) Then
            fltr = fltr & fld & "=" & tval
        Else
            fltr = fltr & fld & "='" & tval & "'"
        End If
        dv3.RowFilter = fltr

        Dim b(,) As Double = MakeMatrixFromDataTableMultiColumns(dv3.ToTable, x1, ax1vals, ax2vals, er)

        If er.Trim <> "" Then
            LabelError.Text = "ERROR: " & er
            Exit Sub
        End If

        Dim dttrgt As DataTable = MakeDataTableFromMatrixMultiColumns(b, x1, ax1vals, ax2vals, q, er)

        'restore dv3
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        dv3.RowFilter = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim

        Dim sma As Decimal = 0  'overall sum for target matrix
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                sma = sma + a(i, j)
            Next
        Next
        Dim smb As Decimal = 0  'overall sum for target matrix
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                smb = smb + b(i, j)
            Next
        Next

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
        Dim dtk As New DataTable

        'balancing
        ret = AdjustMatrixByOverallSum(b, sma)
        ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, x, dtk, u, v)

        'adjust to original target
        If chkAdjustByStart.Checked = False Then  'adjust by target sums
            ret = ret & AdjustMatrixByOverallSum(x, smb)
        End If
        ret = ret & AdjustMatrixByOverallSum(b, smb)
        Dim mx As Decimal = 0  'cell diff
        If u > 0 AndAlso v > 0 Then
            For i = 0 To u - 1
                For j = 0 To v - 1
                    mx = Max(mx, Abs(x(i, j) - b(i, j)))
                Next
            Next
            For i = u To ax1vals.Length - 1
                For j = v To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - b(i, j)))
                Next
            Next

            mx = Round(mx, 2)
            ret = ret & ", maximum difference of cells in selected parts of balancing and target matrixs = " & mx.ToString

            mx = 0
            For i = 0 To u - 1
                For j = 0 To v - 1
                    mx = Max(mx, Abs(x(i, j) - a(i, j)))
                Next
            Next
            For i = u To ax1vals.Length - 1
                For j = v To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - a(i, j)))
                Next
            Next

            mx = Round(mx, 2)
            ret = ret & ", maximum difference of cells in selected parts of balancing and starting matrixs = " & mx.ToString
        End If
        mx = 0
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                mx = Max(mx, Abs(x(i, j) - b(i, j)))
            Next
        Next
        mx = Round(mx, 2)
        ret = ret & ", maximum difference of cells in balancing and target matrixs = " & mx.ToString
        mx = 0
        For i = 0 To ax1vals.Length - 1
            For j = 0 To ax2vals.Length - 1
                mx = Max(mx, Abs(x(i, j) - a(i, j)))
            Next
        Next
        mx = Round(mx, 2)
        ret = ret & ", maximum difference of cells in balancing and starting matrixs = " & mx.ToString
        LabelResult.Text = ret
        LabelResult.Text = ret

        If Not dtstrt Is Nothing Then
            GridView1.DataSource = dtstrt.DefaultView
            GridView1.DataBind()
            GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
            For i = 0 To ax1vals.Count - 1
                GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
            Next
            For j = 0 To ax2vals.Count - 1
                GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
            Next
        End If
        If Not dttrgt Is Nothing Then
            GridView2.DataSource = dttrgt.DefaultView
            GridView2.DataBind()
            GridView2.Rows(GridView2.Rows.Count - 1).Font.Bold = True
        End If

        Dim dtbal As DataTable = MakeDataTableFromMatrixMultiColumns(x, x1, ax1vals, ax2vals, q, er)



        If Not dtbal Is Nothing Then
            GridView3.DataSource = dtbal.DefaultView
            GridView3.DataBind()
            GridView3.Rows(GridView3.Rows.Count - 1).Font.Bold = True
        End If

        'colors and tooltips
        Dim cl As Integer = 100
        Dim clr As Integer = 50
        Dim bg As String = String.Empty
        For i = 0 To GridView2.Rows.Count - 1
            For j = 0 To GridView2.Rows(0).Cells.Count - 1
                If u > 0 AndAlso v > 0 Then
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Ridge
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue

                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2

                    End If
                    If i > u - 1 AndAlso i < GridView2.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2
                    End If
                End If
                'target-ballanced
                If GridView3.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to balanced"
                            Else
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to balanced"
                            End If
                        Else
                            GridView3.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = "<1% difference compare to balanced"
                        End If
                    End If
                ElseIf GridView3.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView3.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = "no difference compare to balanced"
                End If
                'starting-target
                If GridView1.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        'cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"

                            Else
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso GridView1.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
                'starting-ballanced
                If GridView1.Rows(i).Cells(j).Text <> GridView3.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView3.Rows(i).Cells(j).Text) Then
                        'cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                        Try
                            cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView3.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to balanced"
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"
                            Else
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to balanced"
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". <1% difference compare to balanced"
                            GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso GridView1.Rows(i).Cells(j).Text = GridView3.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". no difference compare to balanced"
                    GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
            Next
        Next

        'last row in coefficients
        If Not dtk Is Nothing Then
            GridView4.DataSource = dtk.DefaultView
            GridView4.DataBind()
            If dtk.Rows.Count > 0 Then
                GridView4.Rows(GridView4.Rows.Count - 1).Font.Bold = True
                For i = 0 To GridView4.Rows.Count - 1
                    GridView4.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                Next
                Dim z As Integer = 0
                For i = 0 To GridView1.Rows.Count - 2
                    GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                    z = i + 1
                Next
                z = z + 2
                For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                    GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
                Next
            End If
        End If
        Session("GridView1DataSource") = dtstrt
        Session("GridView2DataSource") = dttrgt
        Session("GridView3DataSource") = dtbal
        Session("GridView4DataSource") = dtk

        'GridView5,6
        If u > 0 AndAlso v > 0 Then
            Dim y(ax1vals.Length - 1, ax2vals.Length - 1) As Double
            Dim dwk As New DataTable

            'balancing
            ret = CanTargetMatrixBalancedFromStartingOne(a, b, l, q, er, y, dwk, 0, 0)

            Dim dtwbal As DataTable = MakeDataTableFromMatrixMultiColumns(y, x1, ax1vals, ax2vals, q, er)

            If Not dtwbal Is Nothing Then
                GridView5.DataSource = dtwbal.DefaultView
                GridView5.DataBind()
                GridView5.Rows(GridView3.Rows.Count - 1).Font.Bold = True
            End If

            'last row in coefficients
            If Not dwk Is Nothing Then
                GridView6.DataSource = dwk.DefaultView
                GridView6.DataBind()
                If dwk.Rows.Count > 0 Then
                    GridView6.Rows(GridView6.Rows.Count - 1).Font.Bold = True
                    For i = 0 To GridView6.Rows.Count - 1
                        GridView6.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                    Next
                End If
            End If

            'colors
            For i = 0 To GridView3.Rows.Count - 1
                For j = 0 To GridView3.Rows(0).Cells.Count - 1
                    'partially ballanced <=> ballanced
                    If GridView3.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to partially balanced"
                                Else
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to partially balanced"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = "<1% difference compare to partially balanced"
                            End If
                        End If
                    ElseIf GridView3.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = "no difference compare to partially balanced"
                    End If

                    'target <=> balanced
                    If GridView2.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView2.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView2.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView2.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView2.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to target"
                                Else
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to target"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". <1% difference compare to target"
                            End If
                        End If
                    ElseIf GridView2.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". no difference compare to target"
                    End If
                Next
            Next
            mx = 0
            For i = 0 To ax1vals.Length - 1
                For j = 0 To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - y(i, j)))
                Next
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = "Compare Partially Balanced Matrix with Balaced Whole Matrix: maximum difference of cells in partially balancinging and whole balancing matrixs = " & mx.ToString

            'compare partly balanced coefficients and whole balanced, maximum cell difference
            mx = 0
            For i = 1 To ax1vals.Length
                mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging rows coefficients and whole balancing rows coefficients  = " & mx.ToString

            mx = 0
            For i = ax1vals.Length + 2 To ax1vals.Length + ax2vals.Length + 1
                mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging columns coefficients and whole balancing columns coefficients  = " & mx.ToString

            For i = 1 To ax1vals.Length + ax2vals.Length + 1
                If dtk(dtk.Rows.Count - 1)(i) <> dwk(dwk.Rows.Count - 1)(i) Then
                    If IsNumeric(dtk(dtk.Rows.Count - 1)(i)) AndAlso IsNumeric(dwk(dwk.Rows.Count - 1)(i)) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(dtk(dtk.Rows.Count - 1)(i)) - CDbl(dwk(dwk.Rows.Count - 1)(i))) / Max(1, Max(CDbl(dtk(dtk.Rows.Count - 1)(i)), CDbl(dwk(dwk.Rows.Count - 1)(i)))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If dtk(dtk.Rows.Count - 1)(i) > dwk(dwk.Rows.Count - 1)(i) Then
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " -" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                            Else
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " +" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                            End If
                        Else
                            GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " <1% difference compare to partially balancing coefficients"
                        End If
                    End If
                ElseIf dtk(dtk.Rows.Count - 1)(i) = dwk(dwk.Rows.Count - 1)(i) Then
                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " no difference compare to partially balancing coefficients"
                End If

            Next
            Session("GridView5DataSource") = dtwbal
            Session("GridView6DataSource") = dwk
        End If

        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Label2.Text = "Balancing for sum of rows and columns of the starting matrix and sums of rows and columns of the target matrix: "

    End Sub
    Private Function ChartLink(ByRef dtkr As DataTable, Optional ByVal lbl As String = "") As String
        'Chart link
        Dim er As String = ""
        Try
            HyperLinkChart.NavigateUrl = "ChartGoogleOne.aspx?MatrixChart=yes"
            HyperLinkChart.Text = "Chart"
            Dim i, j As Integer
            Dim arr As String = ""
            Dim grp As String = ""
            Dim val As String = ""
            Dim clr As String = ""
            Dim srt As String = GridView4.HeaderRow.Cells(0).ToolTip '& "," & DropDownList1.SelectedItem.Text & "   " & DropDownList1.SelectedItem.Text & " Columns"
            'arr
            arr = ""
            Dim mcolors() As String = {"blue", "gold", "lightgreen", "red", "yellow", "darkred", "lightblue", "green", "#e5e4e2", "orange", "darkgreen", "pink", "darkblue", "lightyellow", "maroon", "salmon", "darkorange"}
            Dim k As Integer = 0
            Dim lastk As Integer = 0
            For i = 0 To dtkr.Rows.Count - 1
                'GridView4.Rows(i).Cells(0).ResolveUrl("ChartGoogleOne.aspx?MatrixChart=yes,MatrixRow=i")
                Dim urlControl As New HyperLink
                urlControl.NavigateUrl = "ChartGoogleOne.aspx?MatrixChart=yes&MatrixRow=" & i.ToString
                urlControl.Text = GridView4.Rows(i).Cells(0).Text
                urlControl.Target = "_blank"
                GridView4.Rows(i).Cells(0).ToolTip = GridView4.Rows(i).Cells(0).Text & "=>Chart"
                GridView4.Rows(i).Cells(0).Controls.Add(urlControl)
                For j = 0 To dtkr.Columns.Count - 2
                    grp = dtkr.Rows(i)(0).ToString & "," & GridView4.HeaderRow.Cells(j + 1).ToolTip & "-" & GridView4.HeaderRow.Cells(j + 1).Text
                    dtkr.Columns(j + 1).Caption = GridView4.HeaderRow.Cells(j + 1).ToolTip & "-" & GridView4.HeaderRow.Cells(j + 1).Text
                    val = dtkr.Rows(i)(j + 1).ToString.Trim
                    If val = "" Then val = "0"
                    arr = arr & "['" & grp & "'," & val & ","
                    'color
                    k = lastk Mod mcolors.Length
                    clr = mcolors(k)
                    If GridView4.HeaderRow.Cells(j + 1).Text = "-" Then
                        clr = "black"
                    End If
                    lastk = k + 1
                    arr = arr & "'" & clr & "'"
                    arr = arr & "]"
                    If i < dtkr.Rows.Count OrElse j < dtkr.Columns.Count - 1 Then
                        arr = arr & ","
                    End If
                Next
            Next
            For j = 0 To dtkr.Columns.Count - 2
                If GridView4.HeaderRow.Cells(j + 1).Text = "-" Then Continue For
                Dim urlControl As New HyperLink
                urlControl.NavigateUrl = "ChartGoogleOne.aspx?MatrixChart=yes&MatrixColumn=" & (j + 1).ToString
                urlControl.Text = GridView4.HeaderRow.Cells(j + 1).Text
                urlControl.Target = "_blank"
                GridView4.HeaderRow.Cells(j + 1).ToolTip = GridView4.HeaderRow.Cells(j + 1).ToolTip & "-" & GridView4.HeaderRow.Cells(j + 1).Text & "=>Chart"
                GridView4.HeaderRow.Cells(j + 1).Controls.Add(urlControl)
            Next
            arr = "['" & srt & "','Balancing coefficients',{ role: 'style' }]," & arr
            'Session("nrec") = dtkr.Rows.Count * (dtkr.Columns.Count - 1)
            'Session("arr") = arr
            Session("dtkr") = dtkr

            'links in GridView1
            For i = 0 To GridView1.Rows.Count - 2
                For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                    Dim urlControl As New HyperLink
                    urlControl.NavigateUrl = "ChartGoogleOne.aspx?MatrixChart=yes&MatrixItem=" & GridView1.Rows(i).Cells(j + 2).Text.Trim & "&MatrixRow=" & i.ToString & "&MatrixColumn=" & j.ToString & "&MatrixItemLabel=" & lbl
                    urlControl.Text = GridView1.Rows(i).Cells(j + 2).Text
                    urlControl.Target = "_blank"
                    GridView1.Rows(i).Cells(j + 2).ToolTip = GridView1.Rows(i).Cells(0).Text & " - " & GridView1.HeaderRow.Cells(j + 2).Text & ": " & GridView1.Rows(i).Cells(j + 2).Text & " =>Chart of balanced value of matrix item by Steps"
                    GridView1.Rows(i).Cells(j + 2).Controls.Add(urlControl)
                Next
            Next
            Session("ChartType") = "SteppedAreaChart"
            Return HyperLinkChart.NavigateUrl
        Catch ex As Exception
            er = ex.Message
            Return er
        End Try
    End Function

    Private Sub btnBalanceMatrixSumsRowsCols1b_Click(sender As Object, e As EventArgs) Handles btnBalanceMatrixSumsRowsCols1b.Click
        '(1b) Balancing matrix of rows defined by values in the group field for rows and selected multiple fields as columns for given above sums by rows and by columns
        'DropDownList1 - field values as rows
        'DropDownColumns - multiple columns
        'TextBoxSumsByRows
        'TextBoxSumsByCols

        GridView2.Visible = True
        GridView3.Visible = True
        GridView5.Visible = True
        GridView6.Visible = True
        mainGrids.Visible = True
        Label12.Text = "Starting Matrix"
        Label13.Text = "Target Matrix"
        Label14.Text = "Balancing Matrix"

        Label19.Text = ""
        Label15.Text = ""
        Label20.Text = ""
        LabelCompare.Text = ""
        LabelError.Text = ""
        LabelResult.Text = "Waiting..."
        Label2.Text = ""

        If DropDownList1.Text.Trim = "" OrElse DropDownColumns.Text.Trim = "" OrElse TextBoxSumsByRows.Text.Trim = "" OrElse TextBoxSumsByCols.Text.Trim = "" Then
            LabelError.Text = "Matrix fields must be selected."
            Exit Sub
        End If
        Dim u As Integer = 0
        Dim v As Integer = 0
        If TextBoxUV.Text.Trim <> "" Then
            u = CInt(Piece(TextBoxUV.Text.Trim, ",", 1))
            v = CInt(Piece(TextBoxUV.Text.Trim, ",", 2))
        End If
        Dim dtkr As New DataTable
        Dim c As Boolean = False

        Dim er As String = String.Empty
        Dim ret As String = String.Empty
        Dim l As Integer = CInt(TextBoxNumberSteps.Text.Trim) 'Number of steps
        Dim q As Double = Convert.ToDouble(TextBoxPrecision.Text.Trim) 'Precision
        Dim x1 As String = DropDownList1.SelectedValue.ToString  'group field for matrix rows
        Dim y2 As String = DropDownColumns.Text  'fields2
        ''Label12.Text = "Starting Matrix of " & x1 & " and " & y2 & " for " & fld & " = " & sval.ToString
        'Label12.Text = "Starting Matrix for " & fld & " = " & sval.ToString

        'ax1vals, ax2vals
        dv3 = Session("dv3")
        dv3.RowFilter = ""
        Dim fltr As String = Session("WhereStm").Replace("[", "").Replace("]", "").ToString.Trim
        dv3.RowFilter = fltr

        Dim ax1vals(0) As String
        Dim ax2vals(0) As String
        Dim n As Integer
        Dim i As Integer = 0
        For n = 0 To DropDownColumns.Items.Count - 1
            If DropDownColumns.Items(n).Selected Then
                ReDim Preserve ax2vals(i)
                ax2vals(i) = DropDownColumns.Items(n).Text
                i = i + 1
            End If
        Next

        ret = MakeRowsColumnsArraysFromMultiColumns(dv3.Table, x1, "", "", ax1vals, ax2vals)

        If u >= ax1vals.Length Or v >= ax2vals.Length Then
            LabelError.Text = "Partial number of rows and columns shoud be smaller than dimensions of Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        'Starting matrix
        Dim a(,) As Double = MakeMatrixFromDataTableMultiColumns(dv3.ToTable, x1, ax1vals, ax2vals, er)

        If er.Trim <> "" Then
            LabelError.Text = "ERROR: " & er
            Exit Sub
        End If

        Dim dtstrt As DataTable = MakeDataTableFromMatrixMultiColumns(a, x1, ax1vals, ax2vals, q, er)

        'Target matrix
        Dim cst() As String = TextBoxSumsByRows.Text.Trim.Split(",")
        Dim dst() As String = TextBoxSumsByCols.Text.Trim.Split(",")
        Dim cc(cst.Length - 1) As Double
        Dim dd(dst.Length - 1) As Double
        Dim smb As Double = 0
        Dim dmb As Double = 0
        For i = 0 To cst.Length - 1
            cc(i) = Convert.ToDecimal(cst(i))
            smb = smb + cc(i)
        Next
        For j = 0 To dst.Length - 1
            dd(j) = Convert.ToDecimal(dst(j))
            dmb = dmb + dd(j)
        Next
        'correct total sums
        If smb <> dmb Then
            For j = 0 To dst.Length - 1
                dd(j) = dd(j) * smb / dmb
            Next
        End If

        If ax1vals.Length <> cc.Length Or ax2vals.Length <> dd.Length Then
            LabelError.Text = "Arrays of sums by rows and columns shoud have the same dimensions as Matrix (" & ax1vals.Length.ToString & "," & ax2vals.Length.ToString & ")."
            Exit Sub
        End If

        Dim x(ax1vals.Length - 1, ax2vals.Length - 1) As Double
        Dim dtk As New DataTable

        'balancing
        Dim sma As Double = 0
        er = AdjustSumsByMatrix(a, cc, dd, sma)

        'a - starting array, cc - sums for rows, dd -sums for columns, x - balanced array, dtk - coefficients, l -number of steps, q - precision
        ret = CanBalanceMatrixToColRowSums(a, cc, dd, l, q, er, x, dtk, u, v)

        'adjust to original target sums or starting ones
        If Not chkAdjustByStart.Checked Then  'adjust by target sums

            ret = ret & AdjustMatrixByOverallSum(x, smb)

            'restore original sums
            For i = 0 To cst.Length - 1
                cc(i) = cc(i) * (smb / sma)
            Next
            For j = 0 To dst.Length - 1
                dd(j) = dd(j) * (smb / sma)
            Next

        Else 'chkAdjustByStart.Checked adjust by starting matrix sums

            ret = ret & AdjustMatrixByOverallSum(x, sma)

        End If

        'Balancing matrix
        Dim dtbal As DataTable = MakeDataTableFromMatrixMultiColumns(x, x1, ax1vals, ax2vals, q, er)

        'Target matrix
        Dim dttrgt As DataTable = MakeDataTableMatrixFromSumsOfRowsCols(cc, dd, x1, ax1vals, ax2vals, er)

        If Not dtstrt Is Nothing Then
            GridView1.DataSource = dtstrt.DefaultView
            GridView1.DataBind()
            GridView1.Rows(GridView1.Rows.Count - 1).Font.Bold = True
            For i = 0 To ax1vals.Count - 1
                GridView1.Rows(i).Cells(0).ToolTip = "Balancing Coefficient (weight): ki" & (i + 1).ToString
            Next
            For j = 0 To ax2vals.Count - 1
                GridView1.HeaderRow.Cells(j + 2).ToolTip = "Balancing Coefficient (weight): kj" & (j + 1).ToString
            Next
        End If
        If Not dttrgt Is Nothing Then
            GridView2.DataSource = dttrgt.DefaultView
            GridView2.DataBind()
            GridView2.Rows(GridView2.Rows.Count - 1).Font.Bold = True
        End If

        If Not dtbal Is Nothing Then
            GridView3.DataSource = dtbal.DefaultView
            GridView3.DataBind()
            GridView3.Rows(GridView3.Rows.Count - 1).Font.Bold = True
        End If

        'colors and tooltips
        Dim cl As Integer = 100
        Dim clr As Integer = 50
        Dim bg As String = String.Empty
        For i = 0 To GridView2.Rows.Count - 1
            For j = 0 To GridView2.Rows(0).Cells.Count - 1
                If u > 0 AndAlso v > 0 Then
                    If i < u AndAlso j > 1 AndAlso j < v + 2 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Ridge
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue

                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2

                    End If
                    If i > u - 1 AndAlso i < GridView2.Rows.Count - 1 AndAlso j > v + 1 Then
                        GridView1.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView2.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView3.Rows(i).Cells(j).BorderStyle = BorderStyle.Solid
                        GridView1.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView2.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView3.Rows(i).Cells(j).BorderColor = Color.Blue
                        GridView1.Rows(i).Cells(j).BorderWidth = 2
                        GridView2.Rows(i).Cells(j).BorderWidth = 2
                        GridView3.Rows(i).Cells(j).BorderWidth = 2
                    End If
                End If

                'target-ballanced
                If GridView2.Rows(i).Cells(j).Text.Trim <> "" AndAlso GridView2.Rows(i).Cells(j).Text.Trim <> "&nbsp;" AndAlso GridView3.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to balanced"
                            Else
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"

                                GridView2.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView2.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to balanced"
                            End If
                        Else
                            GridView3.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = "<1% difference compare to balanced"
                        End If
                    End If
                ElseIf GridView2.Rows(i).Cells(j).Text.Trim <> "" AndAlso GridView2.Rows(i).Cells(j).Text.Trim <> "&nbsp;" AndAlso GridView3.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView3.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = "no difference compare to balanced"
                End If
                'starting-target
                If (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text <> GridView2.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView2.Rows(i).Cells(j).Text) Then
                        cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView2.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView2.Rows(i).Cells(j).Text))), 2)
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView2.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"

                            Else
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView1.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to target"
                                GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = "<1% difference compare to target"
                            GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text = GridView2.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = "no difference compare to target"
                    GridView2.Rows(i).Cells(j).ToolTip = GridView2.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
                'starting-ballanced
                If (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text <> GridView3.Rows(i).Cells(j).Text Then
                    If IsNumeric(GridView1.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView3.Rows(i).Cells(j).Text) Then
                        cl = 100 * Round(Abs(CDbl(GridView1.Rows(i).Cells(j).Text) - CDbl(GridView3.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView1.Rows(i).Cells(j).Text), CDbl(GridView3.Rows(i).Cells(j).Text))), 2)
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If CDbl(GridView1.Rows(i).Cells(j).Text) > CDbl(GridView3.Rows(i).Cells(j).Text) Then
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to balanced"
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to starting"
                            Else
                                GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to balanced"
                                GridView1.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                GridView3.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to starting"
                            End If
                        Else
                            GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". <1% difference compare to balanced"
                            GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". <1% difference compare to starting"
                        End If
                    End If
                ElseIf j > 0 AndAlso (GridView2.Rows(i).Cells(j).Text.Trim = "" OrElse GridView2.Rows(i).Cells(j).Text.Trim = "&nbsp;") AndAlso GridView1.Rows(i).Cells(j).Text = GridView3.Rows(i).Cells(j).Text Then
                    GridView1.Rows(i).Cells(j).ToolTip = GridView1.Rows(i).Cells(j).ToolTip & ". no difference compare to balanced"
                    GridView3.Rows(i).Cells(j).ToolTip = GridView3.Rows(i).Cells(j).ToolTip & ". no difference compare to starting"
                End If
            Next
        Next

        'last row in coefficients
        If Not dtk Is Nothing Then
            GridView4.DataSource = dtk.DefaultView
            GridView4.DataBind()
            If dtk.Rows.Count > 0 Then
                GridView4.Rows(GridView4.Rows.Count - 1).Font.Bold = True
                For i = 0 To GridView4.Rows.Count - 1
                    GridView4.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                Next
                Dim z As Integer = 0
                For i = 0 To GridView1.Rows.Count - 2
                    GridView4.HeaderRow.Cells(i + 1).ToolTip = GridView1.Rows(i).Cells(0).Text
                    z = i + 1
                Next
                z = z + 2
                For j = 0 To GridView1.HeaderRow.Cells.Count - 3
                    GridView4.HeaderRow.Cells(z + j).ToolTip = GridView1.HeaderRow.Cells(j + 2).Text
                Next
            End If
        End If

        LabelResult.Text = ret

        Session("GridView1DataSource") = dtstrt
        Session("GridView2DataSource") = dttrgt
        Session("GridView3DataSource") = dtbal
        Session("GridView4DataSource") = dtk

        'GridView5,6 for whole matrix balancing
        If u > 0 AndAlso v > 0 Then
            Dim y(ax1vals.Length - 1, ax2vals.Length - 1) As Double
            Dim dwk As New DataTable

            'balancing
            er = AdjustSumsByMatrix(a, cc, dd, sma)
            ret = CanBalanceMatrixToColRowSums(a, cc, dd, l, q, er, y, dwk, 0, 0)

            'adjust to original target
            If Not chkAdjustByStart.Checked Then  'adjust by target sums

                ret = ret & AdjustMatrixByOverallSum(y, smb)

                'restore original sums
                For i = 0 To cst.Length - 1
                    cc(i) = cc(i) * (smb / sma)
                Next
                For j = 0 To dst.Length - 1
                    dd(j) = dd(j) * (smb / sma)
                Next
            Else 'chkAdjustByStart.Checked adjust by starting matrix sums

                ret = ret & AdjustMatrixByOverallSum(y, sma)

            End If

            Dim dtwbal As DataTable = MakeDataTableFromMatrixMultiColumns(x, x1, ax1vals, ax2vals, q, er)

            If Not dtwbal Is Nothing Then
                GridView5.DataSource = dtwbal.DefaultView
                GridView5.DataBind()
                GridView5.Rows(GridView3.Rows.Count - 1).Font.Bold = True
            End If

            'last row in coefficients
            If Not dwk Is Nothing Then
                GridView6.DataSource = dwk.DefaultView
                GridView6.DataBind()
                If dwk.Rows.Count > 0 Then
                    GridView6.Rows(GridView6.Rows.Count - 1).Font.Bold = True
                    For i = 0 To GridView6.Rows.Count - 1
                        GridView6.Rows(i).Cells(ax1vals.Length + 1).BackColor = Color.Cyan '("#84dd98") 'Color.FromArgb(160, 213, 213)
                    Next
                End If
            End If

            'colors
            For i = 0 To GridView3.Rows.Count - 1
                For j = 0 To GridView3.Rows(0).Cells.Count - 1
                    'partially ballanced <=> ballanced
                    If GridView3.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView3.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView3.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView3.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView3.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "-" & bg & cl.ToString & "% compare to partially balanced"
                                Else
                                    GridView5.Rows(i).Cells(j).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                                    GridView5.Rows(i).Cells(j).ToolTip = "+" & bg & cl.ToString & "% compare to partially balanced"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = "<1% difference compare to partially balanced"
                            End If
                        End If
                    ElseIf GridView3.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = "no difference compare to partially balanced"
                    End If

                    'target <=> balanced
                    If GridView2.Rows(i).Cells(j).Text <> GridView5.Rows(i).Cells(j).Text Then
                        If IsNumeric(GridView2.Rows(i).Cells(j).Text) AndAlso IsNumeric(GridView5.Rows(i).Cells(j).Text) Then
                            Try
                                cl = 100 * Round(Abs(CDbl(GridView2.Rows(i).Cells(j).Text) - CDbl(GridView5.Rows(i).Cells(j).Text)) / Max(1, Max(CDbl(GridView2.Rows(i).Cells(j).Text), CDbl(GridView5.Rows(i).Cells(j).Text))), 2)
                                cl = Min(101, cl)
                            Catch ex As Exception
                                cl = 101
                            End Try
                            clr = cl - 43
                            If cl > 0 Then
                                bg = ""
                                If cl > 100 Then bg = " >"
                                If CDbl(GridView2.Rows(i).Cells(j).Text) > CDbl(GridView5.Rows(i).Cells(j).Text) Then
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". -" & bg & cl.ToString & "% compare to target"
                                Else
                                    GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". +" & bg & cl.ToString & "% compare to target"
                                End If
                            Else
                                GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". <1% difference compare to target"
                            End If
                        End If
                    ElseIf GridView2.Rows(i).Cells(j).Text = GridView5.Rows(i).Cells(j).Text Then
                        GridView5.Rows(i).Cells(j).ToolTip = GridView5.Rows(i).Cells(j).ToolTip & ". no difference compare to target"
                    End If
                Next
            Next
            Dim mx As Decimal = 0
            For i = 0 To ax1vals.Length - 1
                For j = 0 To ax2vals.Length - 1
                    mx = Max(mx, Abs(x(i, j) - y(i, j)))
                Next
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = "Compare Partially Balanced Matrix with Balaced Whole Matrix: maximum difference of cells in partially balancinging and whole balancing matrixs = " & mx.ToString

            'compare partly balanced coefficients and whole balanced, maximum cell difference
            mx = 0
            For i = 1 To ax1vals.Length
                mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging rows coefficients and whole balancing rows coefficients  = " & mx.ToString

            mx = 0
            For i = ax1vals.Length + 2 To ax1vals.Length + ax2vals.Length + 1
                mx = Max(mx, Abs(dtk(dtk.Rows.Count - 1)(i) - dwk(dwk.Rows.Count - 1)(i)))
            Next
            mx = Round(mx, 2)
            LabelCompare.Text = LabelCompare.Text & ", maximum difference of partially balancinging columns coefficients and whole balancing columns coefficients  = " & mx.ToString

            For i = 1 To ax1vals.Length + ax2vals.Length + 1
                If dtk(dtk.Rows.Count - 1)(i) <> dwk(dwk.Rows.Count - 1)(i) Then
                    If IsNumeric(dtk(dtk.Rows.Count - 1)(i)) AndAlso IsNumeric(dwk(dwk.Rows.Count - 1)(i)) Then
                        Try
                            cl = 100 * Round(Abs(CDbl(dtk(dtk.Rows.Count - 1)(i)) - CDbl(dwk(dwk.Rows.Count - 1)(i))) / Max(1, Max(CDbl(dtk(dtk.Rows.Count - 1)(i)), CDbl(dwk(dwk.Rows.Count - 1)(i)))), 2)
                            cl = Min(101, cl)
                        Catch ex As Exception
                            cl = 101
                        End Try
                        clr = cl - 43
                        If cl > 0 Then
                            bg = ""
                            If cl > 100 Then bg = " >"
                            If dtk(dtk.Rows.Count - 1)(i) > dwk(dwk.Rows.Count - 1)(i) Then
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " -" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(207 - clr, 207 - clr, 207 - clr)
                            Else
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " +" & bg & cl.ToString & "% compare to partially balancing coefficients"
                                GridView6.Rows(dwk.Rows.Count - 1).Cells(i).BackColor = Color.FromArgb(191 - clr, 255 - cl, 179 - clr)
                            End If
                        Else
                            GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " <1% difference compare to partially balancing coefficients"
                        End If
                    End If
                ElseIf dtk(dtk.Rows.Count - 1)(i) = dwk(dwk.Rows.Count - 1)(i) Then
                    GridView6.Rows(dwk.Rows.Count - 1).Cells(i).ToolTip = " no difference compare to partially balancing coefficients"
                End If

            Next
            Session("GridView5DataSource") = dtwbal
            Session("GridView6DataSource") = dwk
        End If

        dv3 = Session("dv3")
        dv3.RowFilter = ""

        Label2.Text = "Balancing for sum of rows and columns of the starting matrix of rows and multiple selected columns: "


    End Sub

    Private Sub DropDownListScenarios_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListScenarios.SelectedIndexChanged
        'scenario selected
        Dim scen As String = ""
        If DropDownListScenarios.Text.Trim <> "" Then
            scen = DropDownListScenarios.Text.Substring(0, 2).ToString
            If scen = "1a" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=1a")
            ElseIf scen = "1b" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=1b")
            ElseIf scen = "2a" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=2a")
            ElseIf scen = "2b" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=2b")
            ElseIf scen = "3a" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=3a")
            ElseIf scen = "2c" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=2c")
            ElseIf scen = "3b" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=3b")
            ElseIf scen = "3c" Then
                Response.Redirect("AdvancedAnalytics.aspx?sel=3c")
            End If
            'Label22.Text = ""
            'Label16.Text = ""
            'Label22.Visible = True
            'Label16.Visible = False
        End If
        HyperLinkChart.NavigateUrl = ""
        HyperLinkChart.Text = ""
        Label2.Text = ""
        LabelResult.Text = ""
        LabelError.Text = ""
        LabelCompare.Text = ""
        mainGrids.Visible = False
        Label8.Visible = False
        TextBoxNumberSteps.Visible = False
        Label9.Visible = False
        TextBoxPrecision.Visible = False
        Label16.Visible = False
        TextBoxUV.Visible = False
        chkAdjustByStart.Visible = False
        GridView1.DataSource = Nothing
        GridView1.DataBind()
        GridView2.DataSource = Nothing
        GridView2.DataBind()
        GridView3.DataSource = Nothing
        GridView3.DataBind()
        GridView4.DataSource = Nothing
        GridView4.DataBind()
        GridView5.DataSource = Nothing
        GridView5.DataBind()
        GridView6.DataSource = Nothing
        GridView6.DataBind()
        Session("GridView1DataSource") = Nothing
        Session("GridView2DataSource") = Nothing
        Session("GridView3DataSource") = Nothing
        Session("GridView4DataSource") = Nothing
        Session("GridView5DataSource") = Nothing
        Session("GridView6DataSource") = Nothing
        lnkExportGrid2.Visible = True
        lnkExportGrid2.Enabled = True
        lnkExportGrid3.Visible = True
        lnkExportGrid3.Enabled = True
        lnkExportGrid5.Visible = True
        lnkExportGrid5.Enabled = True
        lnkExportGrid6.Visible = True
        lnkExportGrid6.Enabled = True
        DropDownList1.Visible = False
        DropDownList1.Enabled = False
        DropDownList2.Visible = False
        DropDownList2.Enabled = False
        lblGroups1.Visible = False
        lblGroups2.Visible = False
        lblGroups.Text = " Balancing by fields:"
        trEntryTbl.Visible = False

        If DropDownListScenarios.Text.Trim = "" Then
            devtrControls.Style("display") = "none"
            trEntryTbl.Visible = False
            TableHelp.Visible = True
            tr1a.Visible = True
            tr1b.Visible = True
            tr2a.Visible = True
            tr2b.Visible = True
            tr2c.Visible = True
            tr3a.Visible = True
            tr3b.Visible = True
            tr3c.Visible = True
            tr4a.Visible = True
            tr4b.Visible = True
            tr1aBtn.Visible = False
            tr1bBtn.Visible = False
            tr2aBtn.Visible = False
            tr2bBtn.Visible = False
            tr2cBtn.Visible = False
            tr3aBtn.Visible = False
            tr3bBtn.Visible = False
            tr3cBtn.Visible = False
            trRowsCols.Visible = False
            trField1.Visible = False
            trSumsByRows.Visible = False
            trSumsByCols.Visible = False
            trField2.Visible = False
            'tr1MultiCols.Visible = False
            mainGrids.Visible = False
            Label8.Visible = False
            TextBoxNumberSteps.Visible = False
            Label9.Visible = False
            TextBoxPrecision.Visible = False
            'Label16.Visible = False
            'TextBoxUV.Visible = False
            chkAdjustByStart.Visible = False
            Label31.Visible = False
            DropDownList1.Visible = False
            DropDownList1.Enabled = False
            DropDownList2.Visible = False
            DropDownList2.Enabled = False
            DropDownList3.Enabled = False
            DropDownList4.Enabled = False
            DropDownList5.Enabled = False
            DropDownList6.Enabled = False
            DropDownList7.Enabled = False
            DropDownList8.Enabled = False
            DropDownList10.Enabled = False
            TextBoxSumsByCols.Enabled = False
            TextBoxSumsByRows.Enabled = False

            divMultiCols.Style("display") = "none"
            'DropDownColumns.DropDownBackColor = Color.Gray
            'DropDownColumns.BorderColor = Color.Gray
            'DropDownColumns.TextBoxBackColor = Color.Gray
            'DropDownColumns.ForeColor = Color.Gray
            'CheckBoxSelectAllFields.Enabled = False
            'CheckBoxUnselectAllFields.Enabled = False
        Else
            devtrControls.Style("display") = ""
            tr1a.Visible = False
            tr1b.Visible = False
            tr2a.Visible = False
            tr2b.Visible = False
            tr2c.Visible = False
            tr3a.Visible = False
            tr3b.Visible = False
            tr3c.Visible = False
            TableHelp.Visible = False
            mainGrids.Visible = False
            Label8.Visible = True
            TextBoxNumberSteps.Visible = True
            Label9.Visible = True
            TextBoxPrecision.Visible = True
            'Label16.Visible = True
            'TextBoxUV.Visible = True
            chkAdjustByStart.Visible = True
            'Label31.Visible = True
            'Label31.Text = "Enter:"
            DropDownList1.Enabled = False
            DropDownList2.Enabled = False
            DropDownList1.Visible = False
            DropDownList2.Visible = False
            DropDownList3.Enabled = True
            DropDownList4.Enabled = True
            DropDownList5.Enabled = True
            DropDownList6.Enabled = True
            DropDownList7.Enabled = True
            DropDownList8.Enabled = True
            DropDownList10.Enabled = True
            TextBoxSumsByCols.Enabled = True
            TextBoxSumsByRows.Enabled = True
            CheckBoxSelectAllFields.Enabled = True
            CheckBoxUnselectAllFields.Enabled = True
            DropDownColumns.DropDownBackColor = Color.White
            DropDownColumns.BorderColor = Color.Black
            DropDownColumns.TextBoxBackColor = Color.White
            DropDownColumns.ForeColor = Color.Black
            scen = DropDownListScenarios.Text.Substring(0, 2).ToString
            If scen = "1a" Then
                '<asp:Label ID="Label28" runat="server" Text="1a: Starting Matrix of aggregated field1 values to balance by manually entered sums by rows and sums by columns " 
                'ToolTip = "Scenario 1a: Select both group fields (for rows and columns) and the field1 with aggregation function. Starting matrix of aggregated field1 balances by manually entered sums by rows and columns" ></asp:Label>
                tr1a.Visible = True
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = False
                tr1aBtn.Visible = True
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                trRowsCols.Visible = True
                lblGroups2.Visible = True
                DropDownList2.Visible = True
                trField1.Visible = True
                Label4.Visible = True
                DropDownList4.Visible = True
                trSumsByRows.Visible = True
                trSumsByCols.Visible = True
                trField2.Visible = False
                divMultiCols.Style("display") = "none"
            ElseIf scen = "1b" Then
                '<asp:Label ID="Label29" runat="server" Text="1b: Starting Matrix of rows by matrix group field for rows and columns from selected multiple fields to balance by manually entered sums by rows and sums by columns " 
                'ToolTip = "Scenario 1b: Select the group field for rows and multiple matrix columns. Starting matrix of rows by matrix group field for rows and columns from selected multiple fields balances by manually entered sums by rows and columns.  For Scenarios 1b, 3b, and 3c the selected columns are columns in the matrix." ></asp:Label>
                tr1a.Visible = False
                tr1b.Visible = True
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = True
                tr2aBtn.Visible = False
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                trRowsCols.Visible = True
                lblGroups2.Visible = False
                DropDownList2.Visible = False
                trField1.Visible = False
                trSumsByRows.Visible = True
                trSumsByCols.Visible = True
                trField2.Visible = False
                Label18.Visible = False
                DropDownList10.Visible = False
                divMultiCols.Style("display") = ""
            ElseIf scen = "2a" Or scen = "4a" Then
                '<asp:Label ID="Label22" runat="server" Text="2a: Starting Matrix of aggregated field1 to balance for sums of rows and columns of the Target Matrix of the aggregated field2: " 
                'ToolTip = "Scenario 2a. Select both group fields for rows and columns, field1 with aggregation function for items of Starting Matrix, field2 with aggregation function for items in Target Matrix. Starting Matrix of aggregated field1 to balance for sums of rows and columns of the Target Matrix of the aggregated field2" ></asp:Label>
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = False
                tr4a.Visible = True
                tr4b.Visible = False
                tr4c.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = True
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                tr4cBtn.Visible = False
                trRowsCols.Visible = True
                'lblGroups2.Visible = True
                'DropDownList2.Visible = True
                trField1.Visible = True
                Label4.Visible = True
                DropDownList4.Visible = True
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = True
                Label11.Visible = True
                DropDownList8.Visible = True
                Label1.Visible = False
                DropDownList6.Visible = False
                Label7.Visible = False
                DropDownList7.Visible = False
                divMultiCols.Style("display") = "none"
            ElseIf scen = "2b" Or scen = "4b" Then
                '<asp:Label ID="Label6" runat="server" Text="2b: The starting value of field2 to get starting matrix of field1 values: " 
                'ToolTip = "Starting field2 value as restriction to get the field1 values for starting matrix, values of field2 used to get each itteration Matrix in Scenarios 2b, 2c, 3b, 3c where values of the field2 used as restrictions on data to get itterations of Matrix. Not used in Scenario 3a, the multiple selected fields aggregation function used there instead." ></asp:Label>
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = False
                tr4a.Visible = False
                tr4b.Visible = True
                tr4c.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = True
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                tr4cBtn.Visible = False
                trRowsCols.Visible = True
                'lblGroups2.Visible = True
                'DropDownList2.Visible = True
                trField1.Visible = True
                Label4.Visible = True
                DropDownList4.Visible = True
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = True
                Label11.Visible = False
                DropDownList8.Visible = False
                Label1.Visible = True
                DropDownList6.Visible = True
                Label7.Visible = True
                DropDownList7.Visible = True
                divMultiCols.Style("display") = "none"
            ElseIf scen = "4c" Then
                '<asp:Label ID="Label22" runat="server" Text="2a: Starting Matrix of aggregated field1 to balance for sums of rows and columns of the Target Matrix of the aggregated field2: " 
                'ToolTip = "Scenario 4c. Select fields for balancing sums, field1 with aggregation function for items of Starting Matrix, entry sums for items in Target Matrix. Starting Matrix of aggregated field1 to balance for sums of selected fields of the Target Matrix" ></asp:Label>
                trEntryTbl.Visible = True
                trEntryTbl.BgColor = "#FFFFFF"
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = False
                tr4a.Visible = False
                tr4b.Visible = False
                tr4c.Visible = True
                trRowsCols.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                tr4cBtn.Visible = True
                trRowsCols.Visible = True
                'lblGroups2.Visible = True
                'DropDownList2.Visible = True
                trField1.Visible = True
                Label4.Visible = True
                DropDownList4.Visible = True
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = False
                Label11.Visible = False
                DropDownList8.Visible = False
                Label1.Visible = False
                DropDownList6.Visible = False
                Label7.Visible = False
                DropDownList7.Visible = False
                divMultiCols.Style("display") = "none"
            ElseIf scen = "2c" Then
                '<asp:Label ID="Label26" runat="server" Text=" 2c: Get balancing coefficients for Starting Matrix of field1 for all itterations between starting and target values of the field2" 
                'ToolTip = "Scenario 2c. Select both group fields for rows and columns, field1 with aggregation function for matrix items, and field2 with starting and target values. Get balancing coefficients for Starting Matrix of aggregated field1 for all itterations between starting and target values of the field2" ></asp:Label>
                Label13.Text = ""
                Label14.Text = ""
                Label19.Text = ""
                Label15.Text = ""
                Label20.Text = ""
                GridView2.Visible = False
                GridView3.Visible = False
                GridView5.Visible = False
                GridView6.Visible = False
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = True
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = True
                tr2cBtn.Visible = True
                tr3aBtn.Visible = False
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                trRowsCols.Visible = True
                'lblGroups2.Visible = True
                'DropDownList2.Visible = True
                trField1.Visible = True
                Label4.Visible = True
                DropDownList4.Visible = True
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = True
                Label11.Visible = False
                DropDownList8.Visible = False
                Label1.Visible = True
                DropDownList6.Visible = True
                Label7.Visible = True
                DropDownList7.Visible = True
                'tr1MultiCols.Visible = False
                divMultiCols.Style("display") = "none"
            ElseIf scen = "3a" Then
                '<asp:Label ID="Label23" runat="server" Text=" 3a: Get balancing coefficients for Starting Matrix of aggregated values of field1 and multiple Target Matrix of aggregated selected fields: " 
                'ToolTip = "Scenario 3a. Select both group fields, field1 with aggregation function, and multiple fields with aggregation function. Matrix balancing for itterations for multiple fields values. Field2 is not used." ></asp:Label>
                Label13.Text = ""
                Label14.Text = ""
                Label19.Text = ""
                Label15.Text = ""
                Label20.Text = ""
                GridView2.Visible = False
                GridView3.Visible = False
                GridView5.Visible = False
                GridView6.Visible = False
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = True
                tr3b.Visible = False
                tr3c.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = True
                tr3bBtn.Visible = False
                tr3cBtn.Visible = False
                trRowsCols.Visible = True
                'lblGroups2.Visible = True
                'DropDownList2.Visible = True
                trField1.Visible = True
                Label4.Visible = True
                DropDownList4.Visible = True
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = False
                'tr1MultiCols.Visible = True
                divMultiCols.Style("display") = ""
                Label18.Visible = True
                DropDownList10.Visible = True
            ElseIf scen = "3b" Then
                '<asp:Label ID="Label25" runat="server" Text=" 3b: Starting Matrix as rows by matrix group field for rows and selected multiple columns to balance itterations from starting to target values of the field2" 
                'ToolTip = "Scenario 3b. Select group field for rows, multiple fields for columns, and field2 with starting and target values. Matrix balancing for itterations by starting and target values of the field2. For Scenarios 1b, 3b, and 3c the selected multiple columns are columns in the matrix, and itterations are done using field2 values as restrictions for itterations." ></asp:Label>
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = True
                tr3c.Visible = False
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = True
                tr3cBtn.Visible = True
                trRowsCols.Visible = True
                'lblGroups2.Visible = False
                'DropDownList2.Visible = False
                trField1.Visible = False
                Label4.Visible = False
                DropDownList4.Visible = False
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = True
                Label11.Visible = False
                DropDownList8.Visible = False
                Label1.Visible = True
                DropDownList6.Visible = True
                Label7.Visible = True
                DropDownList7.Visible = True
                divMultiCols.Style("display") = ""
                Label18.Visible = False
                DropDownList10.Visible = False
            ElseIf scen = "3c" Then
                '<asp:Label ID="Label24" runat="server" Text=" 3c: Get balancing coefficients for Starting Matrix as rows by matrix group field for rows and columns from selected multiple fields, for all itterations between starting and target of the field2 values:" 
                'ToolTip = "Scenario 3c. Select group field for rows, multiple fields for columns, and field2 with starting and target values. Get balancing coefficients for Starting Matriix as rows by matrix group field for rows and the selected multiple columns, and all itterations between starting and target of the field2 values. For Scenarios 1b, 3b, and 3c the selected columns are columns in the matrix, and itterations are done using field2 values as restrictions for itterations. " ></asp:Label>
                Label13.Text = ""
                Label14.Text = ""
                Label19.Text = ""
                Label15.Text = ""
                Label20.Text = ""
                GridView2.Visible = False
                GridView3.Visible = False
                GridView5.Visible = False
                GridView6.Visible = False
                tr1a.Visible = False
                tr1b.Visible = False
                tr2a.Visible = False
                tr2b.Visible = False
                tr2c.Visible = False
                tr3a.Visible = False
                tr3b.Visible = False
                tr3c.Visible = True
                tr1aBtn.Visible = False
                tr1bBtn.Visible = False
                tr2aBtn.Visible = False
                tr2bBtn.Visible = False
                tr2cBtn.Visible = False
                tr3aBtn.Visible = False
                tr3bBtn.Visible = True
                tr3cBtn.Visible = True
                trRowsCols.Visible = True
                'lblGroups2.Visible = False
                'DropDownList2.Visible = False
                trField1.Visible = False
                Label4.Visible = False
                DropDownList4.Visible = False
                trSumsByRows.Visible = False
                trSumsByCols.Visible = False
                trField2.Visible = True
                Label11.Visible = False
                DropDownList8.Visible = False
                Label1.Visible = True
                DropDownList6.Visible = True
                Label7.Visible = True
                DropDownList7.Visible = True
                'tr1MultiCols.Visible = True
                divMultiCols.Style("display") = ""
                Label18.Visible = False
                DropDownList10.Visible = False
            End If
        End If
    End Sub

    Private Sub lnkExportGrid1_Click(sender As Object, e As EventArgs) Handles lnkExportGrid1.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "StartingMatrix" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView1DataSource")
        'header
        Dim hdr As String = "Starting Matrix data for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("AdvancedAnalytics.aspx?export=GridData")

    End Sub

    Private Sub lnkExportGrid2_Click(sender As Object, e As EventArgs) Handles lnkExportGrid2.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "TargetMatrix" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView2DataSource")
        'header
        Dim hdr As String = "Target Matrix data for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("AdvancedAnalytics.aspx?export=GridData")

    End Sub

    Private Sub lnkExportGrid3_Click(sender As Object, e As EventArgs) Handles lnkExportGrid3.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "BalancingMatrix" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView3DataSource")
        'header
        Dim hdr As String = "Balancing Matrix data for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("AdvancedAnalytics.aspx?export=GridData")

    End Sub

    Private Sub lnkExportGrid4_Click(sender As Object, e As EventArgs) Handles lnkExportGrid4.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "BalancingCoeffs" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView4DataSource")
        'header
        Dim hdr As String = "Balancing Coefficients for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("AdvancedAnalytics.aspx?export=GridData")

    End Sub

    Private Sub lnkExportGrid5_Click(sender As Object, e As EventArgs) Handles lnkExportGrid5.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "WholeBalancingMatrix" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView5DataSource")
        'header
        Dim hdr As String = "Whole Balancing Matrix data for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("AdvancedAnalytics.aspx?export=GridData")

    End Sub

    Private Sub lnkExportGrid6_Click(sender As Object, e As EventArgs) Handles lnkExportGrid6.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "BalancingCoeffsWhole" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView6DataSource")
        'header
        Dim hdr As String = "Balancing Coefficients of whole balancing for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("AdvancedAnalytics.aspx?export=GridData")

    End Sub

    Private Sub GridView5_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView5.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(0).Font.Bold = True  '.BackColor = Color.FromArgb(152, 203, 203)
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub

    Private Sub GridView6_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView6.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(0).Font.Bold = True  '.BackColor = Color.FromArgb(152, 203, 203)
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub

    Private Sub lnkGrid1AI_Click(sender As Object, e As EventArgs) Handles lnkGrid1AI.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("GridView1DataSource") Is Nothing Then
            Exit Sub
        End If
        'header
        Dim hdr As String = "Starting Matrix data for report:   " & Session("REPTITLE") & ", while " & Label2.Text
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups for rows and columns of the " & hdr & ": "
        Session("dataTable") = Session("GridView1DataSource")
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0")
    End Sub
    Private Sub lnkGrid2AI_Click(sender As Object, e As EventArgs) Handles lnkGrid2AI.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("GridView2DataSource") Is Nothing Then
            Exit Sub
        End If
        'header
        Dim hdr As String = "Target Matrix data for report:   " & Session("REPTITLE") & ", while " & Label2.Text
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups for rows and columns of the " & hdr & ": "
        Session("dataTable") = Session("GridView2DataSource")
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0")
    End Sub

    Private Sub lnkGrid3AI_Click(sender As Object, e As EventArgs) Handles lnkGrid3AI.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("GridView3DataSource") Is Nothing Then
            Exit Sub
        End If
        'header
        Dim hdr As String = "Balancing Matrix data for report:   " & Session("REPTITLE") & ", while " & Label2.Text
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups for rows and columns of the " & hdr & ": "
        Session("dataTable") = Session("GridView3DataSource")
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0")
    End Sub

    Private Sub lnkGrid4AI_Click(sender As Object, e As EventArgs) Handles lnkGrid4AI.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("GridView4DataSource") Is Nothing Then
            Exit Sub
        End If
        'header
        Dim hdr As String = " for the report:   " & Session("REPTITLE") & ", while " & Label2.Text
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison of values in column DifferenceStartBalanced showing the columns difference between StartValue(actual) and BalancedValue(balanced expectation) " & hdr & ": "
        Else
            Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison of values in column DifferenceTargetBalanced showing the columns difference between TargetValue(actual) and BalancedValue(balanced expectation) " & hdr & ": "
        End If
        Session("dataTable") = Session("GridView4DataSource")
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0qu=yes")
    End Sub

    Private Sub lnkGrid5AI_Click(sender As Object, e As EventArgs) Handles lnkGrid5AI.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("GridView5DataSource") Is Nothing Then
            Exit Sub
        End If
        'header
        Dim hdr As String = "Whole Balancing Matrix data for report:   " & Session("REPTITLE") & ", while " & Label2.Text
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups for rows and columns of the " & hdr & ": "
        Session("dataTable") = Session("GridView5DataSource")
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0")
    End Sub

    Private Sub lnkGrid6AI_Click(sender As Object, e As EventArgs) Handles lnkGrid6AI.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("GridView6DataSource") Is Nothing Then
            Exit Sub
        End If
        'header
        Dim hdr As String = "Balancing Coefficients of whole balancing for report:   " & Session("REPTITLE") & ", while " & Label2.Text
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups for rows and columns of the " & hdr & ": "
        Session("dataTable") = Session("GridView6DataSource")
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0")
    End Sub

    Private Sub lnkCompareTargetBalancingSums_Click(sender As Object, e As EventArgs) Handles lnkCompareTargetBalancingSums.Click
        If Session("GridView2DataSource") Is Nothing Then
            Exit Sub
        End If
        If Session("GridView3DataSource") Is Nothing Then
            Exit Sub
        End If

        'header

        Dim hdr As String = ""
        If DropDownListScenarios.SelectedItem.Text.StartsWith("4c:") Then
            hdr = "Differences between Target(requested sums) matrix and Balancing(balanced expectation sums) matrix, where value 0 means that there is no difference for the report - " & Session("REPTITLE") & ", while " & Label2.Text
        Else
            hdr = "Differences between Target(actual sums) matrix and Balancing(balanced expectation sums) matrix, where value 0 means that there is no difference for the report - " & Session("REPTITLE") & ", while " & Label2.Text
        End If
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups for rows and for columns, and for cells of the matrix of the " & hdr & " : "
        '" and  " & LabelResult.Text  - too much
        Dim tbl1 As DataTable = Session("GridView2DataSource")
        Dim tbl2 As DataTable = Session("GridView3DataSource")

        Session("dataTable") = DifferencesOfTwoMatrices(tbl1, tbl2)

        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))

        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub
End Class



