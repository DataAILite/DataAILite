Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Math
Partial Class Correlation
    Inherits System.Web.UI.Page
    Private Sub Correlation_Init(sender As Object, e As EventArgs) Handles Me.Init
        lblHeader.Text = Session("REPTITLE") & " - Correlations"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Fields_Correlation"
        repid = Session("REPORTID")
        Session("nv") = 8
        Session("writetext") = ""
    End Sub
    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLnk As LinkButton = CType(sender, LinkButton)
        Dim id As String = btnLnk.ID
        Dim tag As String = Piece(id, "^", 1)
        Dim link As String = Piece(id, "^", 2)

        Response.Redirect(link)
    End Sub
    Private Sub Correlation_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim ret As String = String.Empty
        If Request("export") = "CorData" Then
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("Cordata"))
                Response.TransmitFile(Session("Cordata"))
            Catch ex As Exception
                LabelMessage.Text = "ERROR!! " & ex.Message
                LabelMessage.Visible = True
                LabelMessage.Enabled = True
            End Try
            Response.End()
            'Response.Redirect("Correlation.aspx?export=no")
        End If
        Dim sqls As String = String.Empty
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim ctlLnk As LinkButton = Nothing
        Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & Session("REPORTID") & "' AND Type='RPT'"
        If Session("Crystal") <> "ok" OrElse Not HasRecords(qsql) Then
            Dim treenodecr As WebControls.TreeNode
            treenodecr = TreeView1.FindNode("ShowReport.aspx?srd=3")
            If treenodecr IsNot Nothing AndAlso treenodecr.ChildNodes.Count = 5 Then
                treenodecr.ChildNodes(4).NavigateUrl = ""
                treenodecr.ChildNodes(4).Text = "See report data"
                treenodecr.ChildNodes(4).Value = Nothing
                treenodecr.ChildNodes.Remove(treenodecr.ChildNodes(4))
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
        Dim srch As String = cleanText(txtSearch.Text)
        Dim sqlwh As String = String.Empty
        If srch <> "" Then
            sqlwh = " AND ((Tbl1Fld1 LIKE '%" & srch & "%') OR (Tbl2Fld2 LIKE '%" & srch & "%')) "
        End If
        Dim cor As String = String.Empty
        If chkCorrelate.Checked = False Then
            cor = " AND Param2>'0.55' "
        End If
        sqls = "SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='CORRELATE' AND Param1 LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%' "
        sqls = sqls & cor & sqlwh & "  ORDER BY Tbl1Fld1,Tbl2Fld2"

        Dim ddtv As DataView = mRecords(sqls)
        'If ret.Trim <> "" Then
        '    Label1.Text = ret
        '    Exit Sub
        'End If
        Session("ddtv") = ddtv
        If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
            lnkExport.Visible = False
            lnkExport.Enabled = False
            lblRecordsCount.Text = "0 records"
            Label1.Text = "There are no correlated fields found yet. You can click the link above to recalculate correlations."
            ret = Correlation(Session("REPORTID"), True)
            If IsNumeric(ret) AndAlso CInt(ret) > 0 Then
                Response.Redirect("Correlation.aspx")
            End If
        Else
            lnkExport.Visible = True
            lnkExport.Enabled = True
        End If
        lblRecordsCount.Text = ddtv.Table.Rows.Count.ToString & " records"
        Dim cat1, cat2, urlc, reps, kk As String
        reps = ""
        For i = 0 To ddtv.Table.Rows.Count - 1
            AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
            If i Mod 2 = 0 Then
                list.Rows(i + 1).BgColor = "#EFFBFB"
            Else
                list.Rows(i + 1).BgColor = "white"
            End If
            cat1 = ddtv.Table.Rows(i)("Tbl1Fld1").ToString
            cat2 = ddtv.Table.Rows(i)("Tbl2Fld2").ToString
            kk = ddtv.Table.Rows(i)("Param2").ToString
            list.Rows(i + 1).Cells(0).InnerHtml = cat1
            list.Rows(i + 1).Cells(1).InnerHtml = cat2
            list.Rows(i + 1).Cells(2).InnerHtml = kk
            If kk > 0.55 Then
                list.Rows(i + 1).Cells(2).Style.Item("font-weight") = "bold"
                list.Rows(i + 1).Cells(2).Style.Item("color") = "Red"
            Else
                list.Rows(i + 1).Cells(2).Style.Item("color") = list.Rows(i + 1).Cells(1).Style.Item("color")
                list.Rows(i + 1).Cells(2).Style.Item("font-weight") = "normal"
            End If
            'urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=matrix"
            'If cat1 = cat2 Then
            '    list.Rows(i + 1).Cells(3).InnerText = " "
            'Else
            '    ctlLnk = New LinkButton
            '    ctlLnk.Text = "matrix"
            '    ctlLnk.ID = "matrix^" & urlc
            '    ctlLnk.ToolTip = "Show Matrix Graph"
            '    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            '    list.Rows(i + 1).Cells(3).InnerText = String.Empty
            '    list.Rows(i + 1).Cells(3).Controls.Add(ctlLnk)
            'End If

            urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat1.ToString & "&y1=" & cat2.ToString & "&fn=Avg&grtype=bar&srd=11"
            ctlLnk = New LinkButton
            ctlLnk.Text = "bar"
            ctlLnk.ID = "bar^" & urlc
            ctlLnk.ToolTip = "Show RDL Bar Graph"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(3).InnerText = String.Empty
            list.Rows(i + 1).Cells(3).Controls.Add(ctlLnk)
            If kk > 0.6 Then
                list.Rows(i + 1).Cells(2).Style.Item("font-weight") = "bold"
            End If

            urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat1.ToString & "&y1=" & cat2.ToString & "&fn=Avg"
            ctlLnk = New LinkButton
            ctlLnk.Text = "charts"
            ctlLnk.ID = "charts^" & urlc
            ctlLnk.ToolTip = "Charts for the fields values"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(4).InnerText = String.Empty
            list.Rows(i + 1).Cells(4).Controls.Add(ctlLnk)

            urlc = "ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat1.ToString & "&y1=" & cat2.ToString & "&fn=Avg"
            ctlLnk = New LinkButton
            ctlLnk.Text = "stats dashboard"
            ctlLnk.ID = "dashboard^" & urlc
            ctlLnk.ToolTip = "Dashboard Statistics for the fields values"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(5).InnerText = String.Empty
            list.Rows(i + 1).Cells(5).Controls.Add(ctlLnk)

            'If kk > 0.6 Then
            '    list.Rows(i + 1).Cells(2).Style.Item("font-weight") = "bold"
            'End If

            'urlc = "ReportViews.aspx?det=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString
            'ctlLnk = New LinkButton
            'ctlLnk.Text = "detail data"
            'ctlLnk.ID = "detail^" & urlc
            'ctlLnk.ToolTip = "Show Detail Data"
            'AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            'list.Rows(i + 1).Cells(7).InnerText = String.Empty
            'list.Rows(i + 1).Cells(7).Controls.Add(ctlLnk)

        Next
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

    Private Sub LinkButtonRefresh_Click(sender As Object, e As EventArgs) Handles LinkButtonRefresh.Click
        'recalculate correlations
        Dim ret As String = String.Empty
        Dim sqls As String = "DELETE FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='CORRELATE' AND Param1 LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%'"
        ret = ExequteSQLquery(sqls)
        ret = Correlation(Session("REPORTID"), True)
        Response.Redirect("Correlation.aspx")
    End Sub

    Protected Function Correlation(ByVal rep As String, ByVal redo As Boolean) As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        If Not Session("REPORTID") Is Nothing Then
            rep = Session("REPORTID")
        End If

        Dim sqlwh As String = String.Empty
        Dim sqls As String = "SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='CORRELATE' AND Param1 LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%'"
        sqls = sqls & "  ORDER BY Tbl1Fld1,Tbl2Fld2"
        Dim ddtv As DataView = mRecords(sqls)
        If ret.Trim <> "" Then
            Label1.Text = ret
            Return ret
        End If
        If Not ddtv Is Nothing AndAlso ddtv.Count > 0 AndAlso ddtv.Table.Rows.Count > 0 Then
            'Label1.Text = "There is analytics for this report."
            If redo Then
                ret = ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='CORRELATE' AND comments='initial'")
            Else
                Return ret
            End If
        End If
        Session("Correlation") = 1
        Dim dv3 As DataView = Session("dv3")
        If dv3 Is Nothing OrElse dv3.Count = 0 Then
            Return ret
        End If
        If Not Session("SortedView") Is Nothing Then
            dv3 = Session("SortedView")
            Dim dtf As New DataTable
            dtf = dv3.Table
            dtf = MakeDTColumnsNamesCLScompliant(dtf, Session("UserConnProvider"), ret)
            dv3 = dtf.DefaultView
        End If

        Dim dtt As New DataTable
        dtt = dv3.ToTable

        Dim i As Integer
        Dim j As Integer
        Dim k As Integer = 0
        Dim l As Integer = 0
        Dim n As Integer = dv3.Table.Rows.Count
        Dim m As Integer = 0
        Dim t1 As Integer = 0
        Dim t2 As Integer = 0
        Dim dtv As DataView = Nothing
        Dim tbl As String = String.Empty
        Dim fld As String = String.Empty


        Dim dbname = GetDataBase(Session("UserConnString"), Session("UserConnProvider"))
        'Dim repdb As String = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("User ID")).Trim
        Dim repdb As String = Session("UserConnString")
        'If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
        'If Session("UserConnString").IndexOf("UID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        If Session("UserConnProvider").ToString <> "Oracle.ManagedDataAccess.Client" Then
            If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
            If Session("UserConnString").IndexOf("UID") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        Else
            If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then repdb = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
        End If
        'create table
        Dim dtb As DataTable = CreateFieldStatsCorrelationTable()
        Try
            For i = 0 To dv3.Table.Columns.Count - 1
                If ColumnTypeIsNumeric(dv3.Table.Columns(i)) Then     'dv3.Table.Columns(i).DataType.Name = "String" OrElse dv3.Table.Columns(i).DataType.Name = "DateTime" Then
                    If dv3.Table.Columns(i).Caption <> "ID" AndAlso dv3.Table.Columns(i).Caption <> "Indx" Then
                        tbl = FindTableToTheField(repid, dv3.Table.Columns(i).Caption, Session("UserConnString"), Session("UserConnProvider"), er)
                        fld = dv3.Table.Columns(i).Caption
                        If Not IsRelation(repdb, dbname, tbl, fld) Then
                            Dim Row As DataRow = dtb.NewRow()
                            Row("Field") = fld
                            dtb.Rows.Add(Row)
                            l = l + 1
                        End If
                    End If
                End If
            Next
            If l = 0 Then
                Return "No numeric fields found."
            End If
            Dim fldnames(5) As String
            fldnames(0) = "Field"
            fldnames(1) = "Count"
            fldnames(2) = "Min"
            fldnames(3) = "Avg"
            fldnames(4) = "Sum"
            fldnames(5) = "StDev"
            dtb = dtb.DefaultView.ToTable(1, fldnames)

            'calc count, distinct count, and Avg
            dtb = CalcCorrelations(dv3.Table, dtb, er)

            dtb = dtb.DefaultView.ToTable(1, fldnames)
            Session("dtb") = dtb

            'TODO correct field values if Min<=0 and recalculate fields stats

            Dim fld1 As String = String.Empty
            Dim fld2 As String = String.Empty

            'calc correlations
            Dim dta As DataTable = Create2FieldsCorrelationTable()
            For i = 0 To dtb.Rows.Count - 1
                fld1 = dtb.Rows(i)("Field").ToString
                If fld1 = "ID" OrElse fld1 = "Indx" OrElse (fld1.StartsWith("ID") And IsNumeric(fld1.Replace("ID", ""))) Then
                    Continue For
                End If
                If Not ColumnTypeIsNumeric(dv3.Table.Columns(fld1)) Then
                    Continue For
                End If

                For j = 0 To dtb.Rows.Count - 1
                    fld2 = dtb.Rows(j)("Field").ToString
                    If fld2 = "ID" OrElse fld2 = "Indx" OrElse (fld2.StartsWith("ID") And IsNumeric(fld2.Replace("ID", ""))) Then
                        Continue For
                    End If
                    If Not ColumnTypeIsNumeric(dv3.Table.Columns(fld2)) Then
                        Continue For
                    End If

                    'at this point both columns are numeric
                    Dim correlated As Boolean = False
                    Dim kk As Double = 0
                    If j <> i Then 'If j >= i AndAlso dtb.Rows(i)("Field") <> dtb.Rows(j)("Field") Then
                        ' calc Sum(field1*field2), etc...
                        If dtb.Rows(i)("StDev") = 0 OrElse dtb.Rows(j)("StDev") = 0 Then
                            kk = 0
                        Else


                            m = 0
                            For k = 0 To dv3.Table.Rows.Count - 1
                                'm = m + ((dv3.Table.Rows(k)(dtb.Rows(i)("Field"))) * (dv3.Table.Rows(k)(dtb.Rows(j)("Field"))))
                                't1 = t1 + ((dv3.Table.Rows(k)(dtb.Rows(i)("Field"))) * (dv3.Table.Rows(k)(dtb.Rows(i)("Field"))))
                                't2 = t2 + ((dv3.Table.Rows(k)(dtb.Rows(j)("Field"))) * (dv3.Table.Rows(k)(dtb.Rows(j)("Field"))))
                                m = m + (((dv3.Table.Rows(k)(dtb.Rows(i)("Field")) - dtb.Rows(i)("Avg")) / dtb.Rows(i)("StDev")) * ((dv3.Table.Rows(k)(dtb.Rows(j)("Field")) - dtb.Rows(j)("Avg")) / dtb.Rows(j)("StDev")))
                            Next
                            'calc kk
                            'kk = n * m - dtb.Rows(i)("Sum") * dtb.Rows(j)("Sum")
                            'kk = kk / Sqrt((n * t1 - dtb.Rows(i)("Sum") * dtb.Rows(i)("Sum")) * (n * t2 - dtb.Rows(j)("Sum") * dtb.Rows(j)("Sum")))

                            'kk = m - n * dtb.Rows(i)("Avg") * dtb.Rows(j)("Avg")
                            'kk = kk / Sqrt((t1 - n * dtb.Rows(i)("Avg") * dtb.Rows(i)("Avg")) * (t2 - n * dtb.Rows(j)("Avg") * dtb.Rows(j)("Avg")))

                            kk = m / (dv3.Table.Rows.Count - 1) '/ dtb.Rows(i)("StDev") / dtb.Rows(j)("StDev")
                            kk = Abs(kk)

                            kk = FormatNumber(kk, 2)
                        End If
                        '    'potential correlation
                        Dim Row As DataRow = dta.NewRow()
                        Row("Field1") = dtb.Rows(i)("Field")
                        Row("Field2") = dtb.Rows(j)("Field")
                        Row("Count1") = CInt(dtb.Rows(i)("Count"))
                        Row("Count2") = CInt(dtb.Rows(j)("Count"))
                        Row("Avg1") = CInt(dtb.Rows(j)("Avg"))
                        Row("Avg2") = CInt(dtb.Rows(j)("Avg"))
                        'Row("Min1") = CInt(dtb.Rows(i)("Min1"))
                        'Row("Min2") = CInt(dtb.Rows(j)("Min2"))
                        'Row("Sum1") = CInt(dtb.Rows(i)("Sum"))
                        'Row("Sum2") = CInt(dtb.Rows(j)("Sum"))
                        Row("CorrelationCoefficient") = kk
                        If redo = True AndAlso chkCorrelate.Checked = False AndAlso kk > 0.55 Then
                            dta.Rows.Add(Row)
                        ElseIf redo = True AndAlso chkCorrelate.Checked Then
                            dta.Rows.Add(Row)
                        End If
                    End If
                Next
            Next
            'update correlations
            Dim sqlst As String = String.Empty
            For i = 0 To dta.Rows.Count - 1
                fld1 = dta.Rows(i)("Field1").ToString
                If fld1 = "ID" OrElse fld1 = "Indx" OrElse (fld1.StartsWith("ID") And IsNumeric(fld1.Replace("ID", ""))) Then
                    Continue For
                End If
                If dta.Rows(i)("CorrelationCoefficient") = 0 Then
                    Continue For
                End If
                fld2 = dta.Rows(i)("Field2").ToString
                If fld2.ToUpper = "ID" OrElse fld2.ToUpper = "INDX" OrElse (fld2.StartsWith("ID") And IsNumeric(fld2.Replace("ID", ""))) Then
                    Continue For
                End If
                If Not HasRecords("SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='CORRELATE' AND Tbl1Fld1='" & dta.Rows(i)("Field1") & "'  AND Tbl2Fld2='" & dta.Rows(i)("Field2") & "' AND Param1 LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%' ") Then
                    'add record to OURReportSQLquery
                    Dim tbl1 As String = FindTableToTheField(repid, dta.Rows(i)("Field1"), Session("UserConnString"), Session("UserConnProvider"), er)
                    Dim tbl2 As String = FindTableToTheField(repid, dta.Rows(i)("Field2"), Session("UserConnString"), Session("UserConnProvider"), er)
                    Dim sFieldList As String = "ReportId,Doing,comments,Tbl1Fld1,Tbl2Fld2,Tbl1,Tbl2,Param1,Param2"
                    Dim sValues As String = "'" & Session("REPORTID") & "',"
                    sValues &= "'CORRELATE',"
                    sValues &= "'initial',"
                    sValues &= "'" & dta.Rows(i)("Field1") & "',"
                    sValues &= "'" & dta.Rows(i)("Field2") & "',"
                    sValues &= "'" & tbl1 & "',"
                    sValues &= "'" & tbl2 & "',"
                    sValues &= "'" & Session("UserDB") & "',"
                    sValues &= "'" & dta.Rows(i)("CorrelationCoefficient").ToString & "'"

                    sqlst = "INSERT INTO OURReportSQLquery (" & sFieldList & ") VALUES (" & sValues & ")"

                    ret = ExequteSQLquery(sqlst)
                End If
            Next
            Return dta.Rows.Count.ToString
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Private Sub Correlation_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        Dim ret As String = String.Empty
    End Sub

    Private Sub lnkExport_Click(sender As Object, e As EventArgs) Handles lnkExport.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        'export to csv the datatable ddtv
        Dim ddv As DataView = Session("ddtv")
        If ddv IsNot Nothing AndAlso ddv.Table IsNot Nothing Then
            Dim res, appldirExcelFiles, myfile As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datestr, timestr As String
            Dim cols(2) As String
            cols(0) = "Tbl1Fld1"
            cols(1) = "Tbl2Fld2"
            cols(2) = "Param2"
            datestr = DateString()
            timestr = TimeString()
            myfile = "Correlation" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
            appldirExcelFiles = applpath & "Temp\"

            'header
            Dim hdr As String = "Correlation data for report:   " & Session("REPTITLE")
            hdr = hdr + System.Environment.NewLine
            Dim delmtr As String = Session("delimiter")
            Dim dt As New DataTable
            dt = ddv.ToTable(True, cols)
            dt.Columns("Tbl1Fld1").ColumnName = "Field1"
            dt.Columns("Tbl2Fld2").ColumnName = "Field2"
            dt.Columns("Param2").ColumnName = "Correlation Coefficient"
            res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)

            Session("Cordata") = res
            'Dim ret As String = String.Empty
            'Try
            '    Response.ContentType = "application/octet-stream"
            '    Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile)
            '    Response.TransmitFile(appldirExcelFiles & myfile)
            'Catch ex As Exception
            '    ret = "ERROR!!  " & ex.Message
            'End Try
            'Response.End()
            Response.Redirect("Correlation.aspx?export=CorData")


        End If
    End Sub

    Private Sub chkCorrelate_CheckedChanged(sender As Object, e As EventArgs) Handles chkCorrelate.CheckedChanged

    End Sub
    Private Sub lnkDataAI_Click(sender As Object, e As EventArgs) Handles lnkDataAI.Click
        'export to csv the datatable ddtv
        Dim ddv As DataView = Session("ddtv")
        If ddv Is Nothing OrElse ddv.Table Is Nothing Then
            Label1.Text = "No data"
            Exit Sub
        End If

        Dim cols(2) As String
        cols(0) = "Tbl1Fld1"
        cols(1) = "Tbl2Fld2"
        cols(2) = "Param2"

        Dim delmtr As String = Session("delimiter")
        Dim dt As New DataTable
        dt = ddv.ToTable(True, cols)
        dt.Columns("Tbl1Fld1").ColumnName = "Field1"
        dt.Columns("Tbl2Fld2").ColumnName = "Field2"
        dt.Columns("Param2").ColumnName = "CorCoef"

        Dim writetext As String = "Correlation data for report:   " & Session("REPTITLE")
        writetext = writetext & Chr(10)
        writetext = writetext & "Field1" & ","
        writetext = writetext & "Field2" & ","
        writetext = writetext & "Correlation Coefficient"
        writetext = writetext & Chr(10)
        For i = 0 To dt.Rows.Count - 1
            writetext = writetext & dt.Rows(i)("Field1") & ","
            writetext = writetext & dt.Rows(i)("Field2") & ","
            writetext = writetext & dt.Rows(i)("CorCoef")
            writetext = writetext & Chr(10)
        Next

        Session("writetext") = writetext
        Response.Redirect("DataAI.aspx?pg=cor")
    End Sub
End Class


