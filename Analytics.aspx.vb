Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Partial Class Analytics
    Inherits System.Web.UI.Page
    Public dv3 As DataView
    Private ddtv As DataView
    Private Sub Analytics_Init(sender As Object, e As EventArgs) Handles Me.Init
        lblHeader.Text = Session("REPTITLE") & " - Analytics"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Analytics"
        repid = Session("REPORTID")
        If Session("dataGroups") Is Nothing Then
            Session("dataGroups") = ""
        End If
        DropDownList2.Items.Clear()
        DropDownList7.Items.Clear()

    End Sub
    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLnk As LinkButton = CType(sender, LinkButton)
        Dim id As String = btnLnk.ID
        Dim tag As String = Piece(id, "^", 1)
        Dim link As String = Piece(id, "^", 2)

        Response.Redirect(link)
    End Sub
    Private Sub Analytics_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim ret As String = String.Empty
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

        Session("ParamNames") = Nothing
        Session("ParamValues") = Nothing
        Session("ParamTypes") = Nothing
        Session("ParamFields") = Nothing
        Session("ParamsCount") = -1
        Session("srchfld") = Nothing
        Session("srchoper") = Nothing
        Session("srchval") = Nothing
        Session("srchstm") = ""
        Session("WhereText") = ""
        Session("WhereStm") = ""
        Session("addwhere") = ""
        Session("filter") = ""
        Session("dv3") = Nothing
        'If Session("dv3") Is Nothing Then
        dv3 = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
        Session("dv3") = dv3
        'Label1.Text = "No data. Run report first"
        'Exit Sub
        'End If

        If dv3.Count = 0 Then
            Label1.Text = "No data. Import data first."
            Exit Sub
        End If

        'dropdowns
        If Not IsPostBack Then

            If Session("Aggregate") Is Nothing OrElse Session("Aggregate").ToString.Trim = "" Then
                DropDownList4.Items.Clear()
                DropDownList4.Items.Add("Count")
                DropDownList4.Items.Add("CountDistinct")
                Session("Aggregate") = "Count"
                DropDownList4.SelectedValue = Session("Aggregate")
                DropDownList4_SelectedIndexChanged(sender, e)
            Else
                Try
                    DropDownList4.SelectedValue = Session("Aggregate")
                    DropDownList4_SelectedIndexChanged(sender, e)
                Catch ex As Exception

                End Try
            End If

            If Session("Aggregate2") Is Nothing OrElse Session("Aggregate2").ToString.Trim = "" Then
                DropDownList6.Items.Clear()
                DropDownList6.Items.Add("Count")
                DropDownList6.Items.Add("CountDistinct")
                Session("Aggregate2") = "Count"
                DropDownList6.SelectedValue = Session("Aggregate2")
                DropDownList6_SelectedIndexChanged(sender, e)
            Else
                Try
                    DropDownList6.SelectedValue = Session("Aggregate2")
                    DropDownList6_SelectedIndexChanged(sender, e)
                Catch ex As Exception

                End Try
            End If

            DropDownList3.Items.Clear()

            Dim dt As DataTable = dv3.Table

            Dim fldname As String = String.Empty
            Dim frdname As String = String.Empty
            Dim fldtype As String = String.Empty
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
            Else
                Session("AxisY") = DropDownList3.SelectedValue
            End If
            DropDownList5.Items.Clear()
            DropDownList5.Items.Add(" ")
            If Not IsPostBack Then
                DropDownList5.SelectedValue = " "
            End If
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
            If Not Session("AxisY2") Is Nothing Then
                DropDownList5.SelectedValue = Session("AxisY2").ToString
                DropDownList5_SelectedIndexChanged(sender, e)
            End If
        End If
        If DropDownList3.Text.Trim = "" OrElse DropDownList5.Text.Trim = "" Then
            Label7.Text = " "
        Else 'If DropDownList3.Text.Trim <>"" AndAlso DropDownList5.Text.Trim <> "" Then
            'correlation between fields in dropdowns
            Dim sqlc As String = "SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='CORRELATE' AND Param1 LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%' "
            sqlc = sqlc & " AND Tbl1Fld1='" & DropDownList3.Text.Trim & "' AND Tbl2Fld2='" & DropDownList5.Text.Trim & "'"
            Dim dtcor As DataView = mRecords(sqlc)
            'Tbl1Fld1,Tbl2Fld2
            'dtcor.RowFilter = "Tbl1Fld1=" & DropDownList3.Text.Trim & " AND Tbl2Fld2=" & DropDownList5.Text.Trim
            If Not dtcor Is Nothing AndAlso dtcor.Count > 0 AndAlso dtcor.Table.Rows.Count > 0 Then
                Label7.Text = "Correlation:  " & dtcor.Table.Rows(0)("Param2").ToString
            End If

        End If

        Dim srch As String = cleanText(txtSearch.Text)
        Dim sqlwh As String = String.Empty
        If srch <> "" Then
            sqlwh = " AND ((Tbl1Fld1 LIKE '%" & srch & "%') OR (Tbl2Fld2 LIKE '%" & srch & "%')) "
        End If
        'If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
        '    sqls = "SELECT DISTINCT Tbl1Fld1,Tbl2Fld2 FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' "
        '    'sqls = "SELECT Tbl1,Tbl1Fld1 FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' "
        'Else
        sqls = "SELECT DISTINCT Tbl1Fld1,Tbl2Fld2 FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' "
        'End If


        sqls = sqls & sqlwh & "  ORDER BY Tbl1Fld1,Tbl2Fld2"

        ddtv = mRecords(sqls, ret)

        If ret.Trim <> "" Then
            Label1.Text = ret
            Exit Sub
        End If

        If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
            lblRecordsCount.Text = "0 records"
            If Not IsPostBack Then
                ret = AnalyticsOriginal(Session("REPORTID"), False)
                Dim er As String = String.Empty
                ddtv = mRecords(sqls, er)
                If ddtv Is Nothing OrElse ddtv.Count = 0 OrElse ddtv.Table.Rows.Count = 0 Then
                    If Not IsPostBack Then
                        If Session("OurConnProvider").ToString.Trim = "Sqlite" Then
                            Response.Redirect("ListOfReports.aspx")
                        Else
                            Response.Redirect("ReportViews.aspx")
                        End If
                    End If
                    Label1.Text = "There are no automatic analytics for this report. Click links above to see data, reports, charts, and statistics."
                    Session("dataGroups") = ""
                End If
            End If
        End If
        Session("dataGroups") = ddtv.Table
        lblRecordsCount.Text = ddtv.Table.Rows.Count.ToString & " records"
        Dim cat1, cat2, urlc, reps As String

        reps = ""
        For i = 0 To ddtv.Table.Rows.Count - 1
            AddRowIntoHTMLtable(ddtv.Table.Rows(i), list)
            cat1 = ddtv.Table.Rows(i)("Tbl1Fld1").ToString
            cat2 = ddtv.Table.Rows(i)("Tbl2Fld2").ToString

            If i Mod 2 = 0 Then
                list.Rows(i + 1).BgColor = "#EFFBFB"
            Else
                list.Rows(i + 1).BgColor = "white"
            End If
            list.Rows(i + 1).Cells(0).InnerHtml = cat1
            list.Rows(i + 1).Cells(1).InnerHtml = cat2

            urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=matrix&srd=11"
            If cat1 = cat2 Then
                list.Rows(i + 1).Cells(2).InnerText = " "
                'list.Rows(i + 1).Cells(2).InnerHtml = " "
            Else
                ctlLnk = New LinkButton
                ctlLnk.Text = "matrix"
                ctlLnk.ID = "matrix^" & urlc
                ctlLnk.ToolTip = "Show Matrix Graph for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                list.Rows(i + 1).Cells(2).InnerText = String.Empty
                list.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)
            End If

            urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=bar&srd=11"
            ctlLnk = New LinkButton
            ctlLnk.Text = "bar"
            ctlLnk.ID = "bar^" & urlc
            ctlLnk.ToolTip = "Show Bar Graph for the field1 and aggrigate function selected above"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(3).InnerText = String.Empty
            list.Rows(i + 1).Cells(3).Controls.Add(ctlLnk)

            urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=pie&srd=11"
            ctlLnk = New LinkButton
            ctlLnk.Text = "pie"
            ctlLnk.ID = "pie^" & urlc
            ctlLnk.ToolTip = "Show Pie Graph for the field1 and aggrigate function selected above"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(4).InnerText = String.Empty
            list.Rows(i + 1).Cells(4).Controls.Add(ctlLnk)

            If cat1 = cat2 Then
                list.Rows(i + 1).Cells(5).InnerText = " "
                'list.Rows(i + 1).Cells(5).InnerHtml = " "
            Else
                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=line&srd=11"
                ctlLnk = New LinkButton
                ctlLnk.Text = "line"
                ctlLnk.ID = "line^" & urlc
                ctlLnk.ToolTip = "Show Line Graph for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                list.Rows(i + 1).Cells(5).InnerText = String.Empty
                list.Rows(i + 1).Cells(5).Controls.Add(ctlLnk)
            End If

            'If cat1 = cat2 Then
            '    list.Rows(i + 1).Cells(6).InnerText = " "
            '    'list.Rows(i + 1).Cells(6).InnerHtml = " "
            'Else
            urlc = "ReportViews.aspx?det=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString
                ctlLnk = New LinkButton
                ctlLnk.Text = "detail report"
                ctlLnk.ID = "detail^" & urlc
                ctlLnk.ToolTip = "Show Detail Report by categories and overall totals and statistics for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                list.Rows(i + 1).Cells(6).InnerText = String.Empty
                list.Rows(i + 1).Cells(6).Controls.Add(ctlLnk)
            'End If

            'Response.Redirect("ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & primX & "&x2=" & secX & "&y1=" & valueY & "&fn=" & fn)
            urlc = "ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&frm=Analytics"
            ctlLnk = New LinkButton
            ctlLnk.Text = "stats dashboard"
            ctlLnk.ID = "dashboard^" & urlc
            ctlLnk.ToolTip = "Dashboard Statistics for the field1 and aggrigate function selected above"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(7).InnerText = String.Empty
            list.Rows(i + 1).Cells(7).Controls.Add(ctlLnk)



            'google charts

            If DropDownList5.Text.Trim = "" Then
                urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&frm=Analytics&domulti=no"
            Else
                '& "&domulti=yes
                urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&y2=" & DropDownList5.Text & "&fn2=" & DropDownList6.Text & "&frm=Analytics&domulti=yes"

            End If
            ctlLnk = New LinkButton
            ctlLnk.Text = "charts"
            ctlLnk.ID = "charts^" & urlc
            ctlLnk.ToolTip = "Google charts for the fields and aggregation functions selected above"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(8).InnerText = String.Empty
            list.Rows(i + 1).Cells(8).Controls.Add(ctlLnk)

            'advanced analytics
            urlc = "AdvancedAnalytics.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&itt=" & DropDownList5.Text & "&fnitt=" & DropDownList6.Text
            ctlLnk = New LinkButton
            ctlLnk.Text = "advanced"
            ctlLnk.ID = "advanced^" & urlc
            ctlLnk.ToolTip = "Advanced Analitics for the fields and aggregation functions selected above"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            list.Rows(i + 1).Cells(9).InnerText = String.Empty
            list.Rows(i + 1).Cells(9).Controls.Add(ctlLnk)
        Next

        'Category/Group dropdowns
        If Not IsPostBack Then
            DropDownList2.Items.Clear()
            DropDownList7.Items.Clear()
            Dim dd As DataTable = ddtv.ToTable(True, "Tbl1Fld1")
            For i = 0 To dd.Rows.Count - 1
                DropDownList2.Items.Add(dd.Rows(i)("Tbl1Fld1"))
                If Session("cat1") IsNot Nothing AndAlso Session("cat1").ToString.Trim <> "" Then
                    Try
                        DropDownList2.SelectedValue = Session("cat1").ToString

                    Catch ex As Exception

                    End Try
                End If
            Next
            dd = ddtv.ToTable(True, "Tbl2Fld2")
            For i = 0 To dd.Rows.Count - 1
                DropDownList7.Items.Add(dd.Rows(i)("Tbl2Fld2"))
                If Session("cat2") IsNot Nothing AndAlso Session("cat2").ToString.Trim <> "" Then
                    Try
                        DropDownList7.SelectedValue = Session("cat2").ToString

                    Catch ex As Exception

                    End Try
                End If
            Next

        End If
        DropDownList2_SelectedIndexChanged(sender, e)
        DropDownList7_SelectedIndexChanged(sender, e)
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
        'recalculate analytics
        Dim ret As String = AnalyticsOriginal(Session("REPORTID"), True)
    End Sub

    Protected Function AnalyticsOriginal(ByVal rep As String, ByVal redo As Boolean) As String
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim i As Integer
        Dim msql As String = String.Empty
        If Not Session("REPORTID") Is Nothing Then
            rep = Session("REPORTID")
        End If
        Dim sqlwh As String = String.Empty
        Dim sqls As String = "SELECT Tbl1,Tbl1Fld1,Tbl2,Tbl2Fld2,Indx FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' "
        sqls = sqls & "  ORDER BY Tbl1Fld1,Tbl2Fld2"
        Dim ddtv As DataView = mRecords(sqls)
        If ret.Trim <> "" Then
            Label1.Text = ret
            Return ret
        End If

        'Label1.Text = "There is analytics for this report."
        If redo Then
            'custom groups not deleted in production!
            ret = ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' AND comments='initial'")
            'ret = ExequteSQLquery("DELETE FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY'")
            'repair custom groups (fixing field names)
            Dim cgrv As DataView = mRecords("SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' AND comments='custom'")
            If cgrv IsNot Nothing AndAlso cgrv.Table.Rows.Count > 0 Then
                For i = 0 To cgrv.Table.Rows.Count - 1
                    If cgrv.Table.Rows(i)("Tbl1Fld1").ToString.LastIndexOf(".") > 0 Then
                        msql = "UPDATE OURReportSQLquery SET Tbl1Fld1='" & cgrv.Table.Rows(i)("Tbl1Fld1").ToString.Substring(cgrv.Table.Rows(i)("Tbl1Fld1").ToString.LastIndexOf(".")).Replace(".", "") & "' WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' AND comments='custom' AND Indx=" & cgrv.Table.Rows(i)("Indx")
                        ret = ExequteSQLquery(msql)
                    End If
                    If cgrv.Table.Rows(i)("Tbl2Fld2").ToString.LastIndexOf(".") > 0 Then
                        msql = "UPDATE OURReportSQLquery SET Tbl2Fld2='" & cgrv.Table.Rows(i)("Tbl2Fld2").ToString.Substring(cgrv.Table.Rows(i)("Tbl2Fld2").ToString.LastIndexOf(".")).Replace(".", "") & "' WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' AND comments='custom' AND Indx=" & cgrv.Table.Rows(i)("Indx")
                        ret = ExequteSQLquery(msql)
                    End If
                Next
            End If


        Else
            If Not ddtv Is Nothing AndAlso ddtv.Count > 0 AndAlso ddtv.Table.Rows.Count > 0 Then
                Return ret
            End If
        End If

        'Session("Analytics") = 1
        'dv3 = Session("dv3")
        dv3 = Nothing
        'If dv3 Is Nothing OrElse dv3.Count = 0 Then
        'Return ret
        dv3 = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
        'End If
        If Not Session("SortedView") Is Nothing Then
            dv3 = Session("SortedView")
        End If
        Session("dv3") = dv3
        Dim dtt As New DataTable
        dtt = dv3.ToTable

        Dim j As Integer
        Dim k As Integer = 0
        Dim dtv As DataView = Nothing
        Dim tbl As String = String.Empty
        Dim dtb As DataTable = CreateStatsAnalyticsTable()
        Try
            'existing custom Group By from OURReportSQLquery for this report
            msql = "SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' AND comments='custom'"
            dtv = mRecords(msql, ret)
            If dtv IsNot Nothing AndAlso dtv.Table IsNot Nothing AndAlso dtv.Table.Rows.Count > 0 Then
                For i = 0 To dtv.Table.Rows.Count - 1
                    Dim Row As DataRow = dtb.NewRow()
                    Row("Field") = dtv.Table.Rows(i)("Tbl1Fld1")
                    Row("comment") = "custom"
                    k = k + 1
                    dtb.Rows.Add(Row)
                    Row = dtb.NewRow()
                    Row("Field") = dtv.Table.Rows(i)("Tbl2Fld2")
                    Row("comment") = "custom"
                    k = k + 1
                    dtb.Rows.Add(Row)
                Next
            End If


            'existing groups fields
            Dim dtgrp As New DataTable
            dtgrp = GetReportGroups(Session("REPORTID")) 'groups table
            For i = 0 To dv3.Table.Columns.Count - 1
                If dv3.Table.Columns(i).DataType.Name = "String" OrElse dv3.Table.Columns(i).DataType.Name = "DateTime" Then
                    If dv3.Table.Columns(i).Caption <> "ID" AndAlso dv3.Table.Columns(i).Caption <> "Indx" Then
                        Dim Row As DataRow = dtb.NewRow()
                        Row("Field") = dv3.Table.Columns(i).Caption
                        Row("comment") = ""
                        For j = 0 To dtgrp.Rows.Count - 1
                            If Row("Field") = dtgrp.Rows(j)("GroupField") Then
                                'not to be removed in the next step
                                Row("comment") = "repgroup"
                            End If
                        Next
                        k = k + 1
                        dtb.Rows.Add(Row)
                    End If
                End If
            Next

            If k = 0 Then
                Return "No potential categories have been identified."
            End If

            Dim fldnames(3) As String
            fldnames(0) = "Field"
            fldnames(1) = "Count"
            fldnames(2) = "Count Distinct"
            fldnames(3) = "comment"
            dtb = dtb.DefaultView.ToTable(1, fldnames)

            'calc count and distinct count
            dtb = CalcStatsAnalytics(rep, dtt, dtb, er)
            dtb = dtb.DefaultView.ToTable(1, fldnames)
            Session("dtb") = dtb

            'calc analytics
            Dim dta As DataTable = CreateAnalyticsTable()
            Dim bgri As Boolean = False
            Dim bgrj As Boolean = False
            If dtb.Rows.Count = 1 Then
                Dim Row As DataRow = dta.NewRow()
                Row("Category1") = dtb.Rows(0)("Field")
                Row("Category2") = dtb.Rows(0)("Field")
                Row("Count1") = CInt(dtb.Rows(0)("Count"))
                Row("Count2") = CInt(dtb.Rows(0)("Count"))
                Row("CountDistinct1") = CInt(dtb.Rows(0)("Count Distinct"))
                Row("CountDistinct2") = CInt(dtb.Rows(0)("Count Distinct"))
                dta.Rows.Add(Row)
            Else
                For i = 0 To dtb.Rows.Count - 1
                    For j = 0 To dtb.Rows.Count - 1
                        'potential groups?
                        bgri = False
                        If CInt(dtb.Rows(i)("Count Distinct")) < 0.75 * CInt(dtb.Rows(i)("Count")) Then
                            bgri = True
                        ElseIf dtb.Rows(i)("comment") = "repgroup" Then
                            bgri = True
                        ElseIf dtb.Rows(i)("comment") = "custom" Then
                            bgri = True
                        ElseIf dtb.Rows(i)("Field").ToString.Trim.ToUpper.StartsWith("COUNTRY") Then
                            bgri = True
                        ElseIf dtb.Rows(i)("Field").ToString.Trim.ToUpper.StartsWith("STATE") Then
                            bgri = True
                        ElseIf dtb.Rows(i)("Field").ToString.Trim.ToUpper.StartsWith("YEAR") Then
                            bgri = True
                        ElseIf dtb.Rows(i)("Field").ToString.Trim.ToUpper.StartsWith("MONTH") Then
                            bgri = True
                        End If
                        bgrj = False
                        If CInt(dtb.Rows(j)("Count Distinct")) < 0.75 * CInt(dtb.Rows(j)("Count")) Then
                            bgrj = True
                        ElseIf dtb.Rows(j)("comment") = "repgroup" Then
                            bgrj = True
                        ElseIf dtb.Rows(j)("comment") = "custom" Then
                            bgrj = True
                        ElseIf dtb.Rows(j)("Field").ToString.Trim.ToUpper.StartsWith("COUNTRY") Then
                            bgrj = True
                        ElseIf dtb.Rows(j)("Field").ToString.Trim.ToUpper.StartsWith("STATE") Then
                            bgrj = True
                        ElseIf dtb.Rows(j)("Field").ToString.Trim.ToUpper.StartsWith("YEAR") Then
                            bgrj = True
                        ElseIf dtb.Rows(j)("Field").ToString.Trim.ToUpper.StartsWith("MONTH") Then
                            bgrj = True
                        End If

                        If bgri AndAlso bgrj Then
                            'add potential group to dta
                            Dim Row As DataRow = dta.NewRow()
                            Row("Category1") = dtb.Rows(i)("Field")
                            Row("Category2") = dtb.Rows(j)("Field")
                            Row("Count1") = CInt(dtb.Rows(i)("Count"))
                            Row("Count2") = CInt(dtb.Rows(j)("Count"))
                            Row("CountDistinct1") = CInt(dtb.Rows(i)("Count Distinct"))
                            Row("CountDistinct2") = CInt(dtb.Rows(j)("Count Distinct"))
                            dta.Rows.Add(Row)
                        Else
                        End If
                    Next
                Next
            End If

            If dta.Rows.Count = 0 Then
                Return "No potential categories have been identified."
            End If

            'update analytics
            Dim sqlst As String = String.Empty
            For i = 0 To dta.Rows.Count - 1
                If Not HasRecords("SELECT * FROM OURReportSQLquery WHERE ReportId='" & Session("REPORTID") & "' AND Doing='GROUP BY' AND Tbl1Fld1='" & dta.Rows(i)("Category1") & "'  AND Tbl2Fld2='" & dta.Rows(i)("Category2") & "' ") Then
                    'add record to OURReportSQLquery
                    Dim tbl1 As String = FindTableToTheField(repid, dta.Rows(i)("Category1"), Session("UserConnString"), Session("UserConnProvider"), er)
                    Dim tbl2 As String = FindTableToTheField(repid, dta.Rows(i)("Category2"), Session("UserConnString"), Session("UserConnProvider"), er)
                    'sqlst = "INSERT INTO OURReportSQLquery SET ReportId='" & Session("REPORTID") & "',Doing='GROUP BY',comments='initial',Tbl1Fld1='" & dta.Rows(i)("Category1") & "',Tbl2Fld2='" & dta.Rows(i)("Category2") & "',Tbl1='" & tbl1 & "',Tbl2='" & tbl2 & "'"
                    sqlst = "INSERT INTO OURReportSQLquery "
                    sqlst &= "(ReportId,Doing,comments,Tbl1Fld1,Tbl2Fld2,Tbl1,Tbl2) "
                    sqlst &= "VALUES ('" & Session("REPORTID") & "','"
                    sqlst &= "GROUP BY','"
                    sqlst &= "initial','"
                    sqlst &= dta.Rows(i)("Category1") & "','"
                    sqlst &= dta.Rows(i)("Category2") & "','"
                    sqlst &= tbl1 & "','"
                    sqlst &= tbl2 & "')"
                    ret = ExequteSQLquery(sqlst)
                End If
            Next
            Response.Redirect("Analytics.aspx")
            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click

    End Sub

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

        If DropDownList3.Text.Trim <> "" AndAlso DropDownList5.Text.Trim <> "" Then
            Session("AxisYM") = DropDownList3.Text.Trim & "," & DropDownList5.Text.Trim
            Session("charttype") = "LineChart"
            If DropDownList4.Text.Trim <> "" AndAlso DropDownList6.Text.Trim <> "" Then
                Session("Aggregate") = DropDownList4.Text.Trim '& "," & DropDownList6.Text.Trim
            End If
            Session("AggregateM") = DropDownList4.Text.Trim
        ElseIf DropDownList5.Text.Trim = "" Then
            Session("AxisYM") = DropDownList3.Text.Trim
            Session("AxisY") = DropDownList3.Text.Trim
            Session("charttype") = "PieChart"
            Session("Aggregate") = DropDownList4.Text.Trim
        End If

    End Sub

    Private Sub DropDownList4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList4.SelectedIndexChanged
        If DropDownList4.Text.Trim <> "" Then  'AndAlso DropDownList6.Text.Trim <> "" 
            Session("Aggregate") = DropDownList4.Text.Trim '& "," & DropDownList6.Text.Trim
            Session("AggregateM") = DropDownList4.SelectedItem.Text
        End If
    End Sub

    Private Sub DropDownList5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList5.SelectedIndexChanged
        'depended of type
        DropDownList6.Items.Clear()
        If DropDownList5.SelectedValue = " " Then
            DropDownList6.Items.Add(" ")
        End If
        DropDownList6.Items.Add("Count")
        DropDownList6.Items.Add("CountDistinct")

        If ColumnTypeIsNumeric(dv3.Table.Columns(DropDownList5.SelectedValue)) Then
            DropDownList6.Items.Add("Sum")
            DropDownList6.Items.Add("Max")
            DropDownList6.Items.Add("Min")
            DropDownList6.Items.Add("Avg")
            DropDownList6.Items.Add("StDev")
            DropDownList6.Items.Add("Value")
        End If

        Session("AxisY2") = DropDownList5.SelectedValue.Trim
        If Not Session("Aggregate2") Is Nothing AndAlso Session("Aggregate2").ToString.Trim <> "" Then
            Try
                DropDownList6.SelectedValue = Session("Aggregate2")
            Catch ex As Exception

            End Try
        End If
        Session("nv2") = DropDownList6.Items.Count

        If DropDownList3.Text.Trim <> "" AndAlso DropDownList5.Text.Trim <> "" Then
            Session("AxisYM") = DropDownList3.Text.Trim & "," & DropDownList5.Text.Trim
            Session("charttype") = "LineChart"
            If DropDownList4.Text.Trim <> "" AndAlso DropDownList6.Text.Trim <> "" Then
                Session("Aggregate") = DropDownList4.Text.Trim '& "," & DropDownList6.Text.Trim
            End If
            Session("AggregateM") = DropDownList4.Text.Trim
        ElseIf DropDownList5.Text.Trim = "" Then
            Session("AxisYM") = DropDownList3.Text.Trim
            Session("AxisY") = DropDownList3.Text.Trim
            Session("charttype") = "PieChart"
            Session("Aggregate") = DropDownList4.Text.Trim
        End If

    End Sub
    Private Sub DropDownList6_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList6.SelectedIndexChanged

        'If DropDownList4.Text.Trim <> "" AndAlso DropDownList6.Text.Trim <> "" Then
        '    Session("Aggregate") = DropDownList4.Text.Trim
        '    Session("AggregateM") = DropDownList4.Text.Trim
        'End If
        Try
            If DropDownList6.Text.Trim <> "" Then
                Session("Aggregate2") = DropDownList6.Text
            End If

        Catch ex As Exception

        End Try
    End Sub
    Private Sub lnkGridAI_Click(sender As Object, e As EventArgs) Handles lnkGridAI.Click
        Session("dataTable") = dv3.Table
        Session("DataToChatAI") = "Data: " & ExportToCSVtext(Session("dataTable"), Chr(9))
        If Session("dataGroups") IsNot Nothing Then
            Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups " & ExportGroupsToCSVtext(Session("dataGroups"), Chr(9))
        End If
        Response.Redirect("~/ChatAI.aspx?pg=expl&srd=0&qu=yes")
    End Sub

    Private Sub LinkButtonAddCategory_Click(sender As Object, e As EventArgs) Handles LinkButtonAddCategory.Click
        Dim rt As String = String.Empty
        Dim er As String = String.Empty
        If DropDownList1.SelectedValue.Trim <> "" Then
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList1.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
        Dim ret As String = AnalyticsOriginal(Session("REPORTID"), True)
    End Sub

    Private Sub DropDownList2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList2.SelectedIndexChanged

        For i = 0 To ddtv.Table.Rows.Count - 1
            If list.Rows(i + 1).Cells(0).InnerHtml = DropDownList2.SelectedItem.Value AndAlso list.Rows(i + 1).Cells(1).InnerHtml = DropDownList7.SelectedItem.Value Then
                list.Rows(i + 1).BgColor = "lightgreen"
                list.Rows(i + 1).Cells(0).Focus()
                Session("cat1") = DropDownList2.SelectedItem.Value

                listshort.Rows(0).Cells(0).InnerText = "Reports:"

                Dim ctlLnk As LinkButton
                Dim cat1, cat2, urlc, reps As String
                cat1 = DropDownList2.SelectedItem.Value
                cat2 = DropDownList7.SelectedItem.Value
                reps = ""

                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=matrix&srd=11"
                If cat1 = cat2 Then
                    listshort.Rows(0).Cells(1).InnerText = " "
                Else
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "matrix"
                    ctlLnk.ID = "smatrix^" & urlc
                    ctlLnk.ToolTip = "Show Matrix Graph for the field1 and aggrigate function selected above"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    listshort.Rows(0).Cells(1).InnerText = String.Empty
                    listshort.Rows(0).Cells(1).Controls.Add(ctlLnk)
                End If

                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=bar&srd=11"
                ctlLnk = New LinkButton
                ctlLnk.Text = "bar"
                ctlLnk.ID = "sbar^" & urlc
                ctlLnk.ToolTip = "Show Bar Graph for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(2).InnerText = String.Empty
                listshort.Rows(0).Cells(2).Controls.Add(ctlLnk)

                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=pie&srd=11"
                ctlLnk = New LinkButton
                ctlLnk.Text = "pie"
                ctlLnk.ID = "spie^" & urlc
                ctlLnk.ToolTip = "Show Pie Graph for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(3).InnerText = String.Empty
                listshort.Rows(0).Cells(3).Controls.Add(ctlLnk)

                If cat1 = cat2 Then
                    listshort.Rows(0).Cells(4).InnerText = " "
                Else
                    urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=line&srd=11"
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "line"
                    ctlLnk.ID = "sline^" & urlc
                    ctlLnk.ToolTip = "Show Line Graph for the field1 and aggrigate function selected above"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    listshort.Rows(0).Cells(4).InnerText = String.Empty
                    listshort.Rows(0).Cells(4).Controls.Add(ctlLnk)
                End If

                urlc = "ReportViews.aspx?det=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString
                ctlLnk = New LinkButton
                ctlLnk.Text = "detail report"
                ctlLnk.ID = "sdetail^" & urlc
                ctlLnk.ToolTip = "Show Detail Report by categories and overall totals and statistics for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(5).InnerText = String.Empty
                listshort.Rows(0).Cells(5).Controls.Add(ctlLnk)
                'End If

                urlc = "ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&frm=Analytics"
                ctlLnk = New LinkButton
                ctlLnk.Text = "stats dashboard"
                ctlLnk.ID = "sdashboard^" & urlc
                ctlLnk.ToolTip = "Dashboard Statistics for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(6).InnerText = String.Empty
                listshort.Rows(0).Cells(6).Controls.Add(ctlLnk)

                'google charts
                If DropDownList5.Text.Trim = "" Then
                    urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&frm=Analytics&domulti=no"

                Else

                    urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&y2=" & DropDownList5.Text & "&fn2=" & DropDownList6.Text & "&frm=Analytics&domulti=yes"

                End If
                ctlLnk = New LinkButton
                ctlLnk.Text = "charts"
                ctlLnk.ID = "scharts^" & urlc
                ctlLnk.ToolTip = "Google charts for the fields and aggregation functions selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(7).InnerText = String.Empty
                listshort.Rows(0).Cells(7).Controls.Add(ctlLnk)

            End If
        Next
    End Sub

    Private Sub DropDownList7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList7.SelectedIndexChanged

        For i = 0 To ddtv.Table.Rows.Count - 1
            If list.Rows(i + 1).Cells(0).InnerHtml = DropDownList2.SelectedItem.Value AndAlso list.Rows(i + 1).Cells(1).InnerHtml = DropDownList7.SelectedItem.Value Then
                list.Rows(i + 1).BgColor = "lightgreen"
                list.Rows(i + 1).Cells(0).Focus()
                Session("cat2") = DropDownList7.SelectedItem.Value

                listshort.Rows(0).Cells(0).InnerText = "Reports:"

                Dim ctlLnk As LinkButton
                Dim cat1, cat2, urlc, reps As String
                cat1 = DropDownList2.SelectedItem.Value
                cat2 = DropDownList7.SelectedItem.Value
                reps = ""

                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=matrix&srd=11"
                If cat1 = cat2 Then
                    listshort.Rows(0).Cells(1).InnerText = " "
                Else
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "matrix"
                    ctlLnk.ID = "smatrix^" & urlc
                    ctlLnk.ToolTip = "Show Matrix Graph for the field1 and aggrigate function selected above"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    listshort.Rows(0).Cells(1).InnerText = String.Empty
                    listshort.Rows(0).Cells(1).Controls.Add(ctlLnk)
                End If

                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=bar&srd=11"
                ctlLnk = New LinkButton
                ctlLnk.Text = "bar"
                ctlLnk.ID = "sbar^" & urlc
                ctlLnk.ToolTip = "Show Bar Graph for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(2).InnerText = String.Empty
                listshort.Rows(0).Cells(2).Controls.Add(ctlLnk)

                urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=pie&srd=11"
                ctlLnk = New LinkButton
                ctlLnk.Text = "pie"
                ctlLnk.ID = "spie^" & urlc
                ctlLnk.ToolTip = "Show Pie Graph for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(3).InnerText = String.Empty
                listshort.Rows(0).Cells(3).Controls.Add(ctlLnk)

                If cat1 = cat2 Then
                    listshort.Rows(0).Cells(4).InnerText = " "
                Else
                    urlc = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=line&srd=11"
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "line"
                    ctlLnk.ID = "sline^" & urlc
                    ctlLnk.ToolTip = "Show Line Graph for the field1 and aggrigate function selected above"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    listshort.Rows(0).Cells(4).InnerText = String.Empty
                    listshort.Rows(0).Cells(4).Controls.Add(ctlLnk)
                End If

                urlc = "ReportViews.aspx?det=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString
                ctlLnk = New LinkButton
                ctlLnk.Text = "detail report"
                ctlLnk.ID = "sdetail^" & urlc
                ctlLnk.ToolTip = "Show Detail Report by categories and overall totals and statistics for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(5).InnerText = String.Empty
                listshort.Rows(0).Cells(5).Controls.Add(ctlLnk)
                'End If

                urlc = "ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&frm=Analytics"
                ctlLnk = New LinkButton
                ctlLnk.Text = "stats dashboard"
                ctlLnk.ID = "sdashboard^" & urlc
                ctlLnk.ToolTip = "Dashboard Statistics for the field1 and aggrigate function selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(6).InnerText = String.Empty
                listshort.Rows(0).Cells(6).Controls.Add(ctlLnk)

                'google charts
                If DropDownList5.Text.Trim = "" Then
                    urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&frm=Analytics&domulti=no"

                Else

                    urlc = "ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & cat1.ToString & "&x2=" & cat2.ToString & "&y1=" & DropDownList3.Text & "&fn=" & DropDownList4.Text & "&y2=" & DropDownList5.Text & "&fn2=" & DropDownList6.Text & "&frm=Analytics&domulti=yes"

                End If
                ctlLnk = New LinkButton
                ctlLnk.Text = "charts"
                ctlLnk.ID = "scharts^" & urlc
                ctlLnk.ToolTip = "Google charts for the fields and aggregation functions selected above"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                listshort.Rows(0).Cells(7).InnerText = String.Empty
                listshort.Rows(0).Cells(7).Controls.Add(ctlLnk)

            End If
        Next
    End Sub
End Class


