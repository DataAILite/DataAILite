Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Math
Imports System.Drawing
Imports System.Web.UI.WebControls
Imports Microsoft.Reporting.WebForms
Partial Class ShowReport
    Inherits System.Web.UI.Page
    Public MyconnStr As String = ConfigurationManager.ConnectionStrings.Item("MySQLConnection").ToString
    Public Myconn As SqlConnection
    Public cmdReport As SqlCommand
    Public dt As DataTable
    Public dr As DataRow
    Public dv1, dv2, dv3, dv4 As DataView
    Public da As SqlDataAdapter
    Public sp, repid, repname, reptitle, reporttable, WhereText, mSql, reppath, pdfpath, temppath As String
    Public Q, i, j, nrec, ncol, ndd, n As Integer
    Public ParamNames() As String
    Public ParamTypes() As String
    Public ParamValues() As String
    Public ParamFields() As String
    Public dirsort As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        Session("Stats") = 0
        Session("graph") = ""
        Session("matrix") = ""
        trStatLabel.Visible = True
        trReportStats.Visible = True
        GridView2.Enabled = True
        GridView2.Visible = True

        TextBoxDelimeter.Text = Session("delimiter")
        If WhereText Is Nothing Then WhereText = ""
        If Session("WhereText") Is Nothing Then Session("WhereText") = ""
        ButtonSearch.OnClientClick() = "searchReport(event); return false;"

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        ' if link from Data Import list, clear Session("dv3")
        If Request("didata") = "yes" Then
            Session("dv3") = Nothing
            Session("ParamNames") = Nothing
            Session("ParamValues") = Nothing
            Session("ParamTypes") = Nothing
            Session("ParamFields") = Nothing
            Session("ParamsCount") = 0
        End If
        If Request("srd") = "15" Then Session("dv3") = Nothing
        HyperLinkDataAI.NavigateUrl = "DataAI.aspx?pg=expl&srd=" & Request("srd")
        If Not IsPostBack Then
            Session("GridView1DataSource") = Nothing
            Session("GridView2DataSource") = Nothing
            Session("QuestionToAI") = ""
            Session("SELECTEDFlds") = ""
            Session("dataTable") = Nothing
            Session("DataToChatAI") = ""
            Session("OriginalDataTable") = Nothing
            Session("dataGroups") = Nothing
        End If
        Dim target As String = Page.Request("__EVENTTARGET")
        Dim data As String = Page.Request("__EVENTARGUMENT")
        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target = "DoSearch" Then
                Dim parts As String() = data.Split("~")
                If parts.Length = 3 AndAlso parts(0) <> String.Empty AndAlso parts(1) <> String.Empty AndAlso parts(2) <> String.Empty Then
                    Session("srchfld") = parts(0)
                    Session("srchoper") = parts(1)
                    Session("srchval") = parts(2)
                    If Session("srd") = "8" Then
                        Response.Redirect("ShowReport.aspx?srd=8")
                    Else
                        Response.Redirect("ShowReport.aspx?srd=0")
                    End If

                End If
            End If
        End If
        If Request("export") = "GridData" Then
            If Request("srd") = "8" Then
                lnkExportGrid2_Click("", EventArgs.Empty)
            Else
                lnkExportGrid1_Click("", EventArgs.Empty)
            End If

            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("FileGridViewdata"))
                Response.TransmitFile(Session("FileGridViewdata"))
            Catch ex As Exception
                LabelError.Text = "ERROR!! " & ex.Message
                LabelError.Visible = True
                LabelError.Enabled = True
            End Try
            'Try
            '    Dim httpApp = New HttpApplication
            '    'Response.Flush()
            '    Page.Visible = False
            '    'Response.SuppressContent = True
            '    httpApp.CompleteRequest()
            '    'Response.End()
            'Catch ex As Exception

            'End Try
            Response.End()
        Else
            Page.Visible = True
        End If

        Try
            Dim ret As String = String.Empty
            Dim ddid, ddname, ddsql, dddsql, ddrSql, ddfield, ddvalue, ddlabel, ddtype, ddrequest As String
            Dim drvalue, options As String
            LabelError.Text = ""
            trStatLabel.Visible = False
            trReportStats.Visible = False
            trParameters.Visible = True
            repid = Request("REPORT")
            If repid = "" AndAlso Session("REPORTID") <> "" Then
                repid = Session("REPORTID")
            End If
            If repid <> "" Then Session("REPORTID") = repid
            LabelReportID.Text = Session("REPORTID")
            dirsort = ""
            reppath = ""
            LabelCrystalLink.Text = " "
            LabelExportExcel.Text = " "
            LabelExport.Text = " "
            HyperLinkExport.Text = " "
            HyperLinkToCSVFile.Text = " "
            trParameters.Visible = True
            Session("ParamsCount") = -1
            If Not Session("pdfpath") Is Nothing AndAlso Session("pdfpath").ToString <> "" Then
                ButtonDownloadFile.Visible = True
                ButtonDownloadFile.Enabled = True
            Else
                ButtonDownloadFile.Visible = False
                ButtonDownloadFile.Enabled = False
            End If

            If repid = "" Then
                Exit Sub
            End If
            Dim bReportExpired As Boolean = ReportExpired(repid, Session("logon"))
            'Report Info (title, data for report)
            dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
            If dv1.Table.Rows(0)("ReportType").ToString = "" Then
                dv1.Table.Rows(0)("ReportType") = "rdl"
            End If
            Session("noedit") = dv1.Table.Rows(0)("Param7type").ToString
            If Session("noedit") = "standard" OrElse bReportExpired Then
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
            If Session("Attributes") = "sp" Then
                Dim node As WebControls.TreeNode = TreeView1.FindNode("SQLquery.aspx?tnq=0")
                Dim idx As Integer
                If node IsNot Nothing Then
                    idx = TreeView1.Nodes.IndexOf(node)
                    If idx > -1 Then
                        'For i = 0 To 2
                        TreeView1.Nodes.RemoveAt(idx)
                        'Next
                    End If
                End If
                Session("ParamsRelated") = ""
            End If
            If dv1.Table.Rows.Count = 1 Then
                reptitle = dv1.Table.Rows(0)("ReportTtl").ToString
                Session("ParamsRelated") = dv1.Table.Rows(0)("Param0id").ToString
                Session("REPTITLE") = reptitle
                Session("REPORTID") = repid
                LabelReportTitle.Text = "Data for report:     " & reptitle
                lblStatistics.Text = "Statistics for report: " & reptitle
                LabelPageFtr.Text = dv1.Table.Rows(0)("Comments").ToString
                Session("PageFtr") = LabelPageFtr.Text
                ' main.Rows(4).Cells(0).InnerHtml = "&nbsp;"
            Else
                'Response.Redirect("Nodata.aspx")
            End If
            If dv1.Table.Rows(0)("ReportType") = "crystal" Then
                ShowRDL.Visible = False
                ShowRDL.Enabled = False
            ElseIf dv1.Table.Rows(0)("ReportType") = "rdl" Then
                ShowCrystal.Visible = False
                ShowCrystal.Enabled = False
            ElseIf dv1.Table.Rows(0)("ReportType") = "aspx" Then
                ShowRDL.Visible = False
                ShowRDL.Enabled = False
                ShowCrystal.Visible = False
                ShowCrystal.Enabled = False
            End If

            Dim qsql As String = "SELECT ReportId,Type FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RPT'"
            If Session("Crystal") <> "ok" OrElse Not HasRecords(qsql) Then
                Dim treenodecr As WebControls.TreeNode = TreeView1.FindNode("ShowReport.aspx?srd=3")
                If treenodecr IsNot Nothing AndAlso treenodecr.ChildNodes.Count = 6 Then
                    treenodecr.ChildNodes(5).NavigateUrl = ""
                    treenodecr.ChildNodes(5).Text = "See report data"
                    treenodecr.ChildNodes(5).Value = Nothing
                    treenodecr.ChildNodes.Remove(treenodecr.ChildNodes(5))
                End If

            End If

            'Report Show (drop-downs, data for drop-downs)
            'dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
            Dim er As String = String.Empty
            dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY PrmOrder", er)

            n = dv2.Table.Rows.Count   'how many parameters (drop-downs)
            Session("ParamsCount") = n
            ReDim Preserve ParamNames(n - 1)
            ReDim Preserve ParamTypes(n - 1)
            ReDim Preserve ParamValues(n - 1)
            ReDim Preserve ParamFields(n - 1)

            If n = 0 Then
                trParameters.Visible = False
            Else
                trParameters.Visible = True
            End If

            Dim j As Integer = 0
            i = 0
            'create parameters and draw drop-downs
            For i = 0 To dv2.Table.Rows.Count - 1   'draw drop-down start
                options = ""
                ddid = dv2.Table.Rows(i)("DropDownID")
                ddlabel = dv2.Table.Rows(i)("DropDownLabel")
                ddname = dv2.Table.Rows(i)("DropDownName")
                ddfield = dv2.Table.Rows(i)("DropDownFieldName")
                ddtype = dv2.Table.Rows(i)("DropDownFieldType")
                ddvalue = dv2.Table.Rows(i)("DropDownFieldValue")
                ddsql = dv2.Table.Rows(i)("DropDownSQL").ToString '.Replace("""", "'")
                dddsql = ddsql
                ddrSql = dv1.Table.Rows(0)("SQLquerytext")

                'check if drop-down is selected
                'ddrequest = Request("Select" & ddid)  'old
                ddrequest = Request(ddid)

                If Not IsPostBack AndAlso ddrequest = "" AndAlso Not Session("ParamValues") Is Nothing AndAlso Session("ParamsCount") > i AndAlso Not Session("ParamValues")(i) Is Nothing AndAlso Session("ParamValues")(i).ToString.Trim <> "" AndAlso Session("ParamValues")(i).ToString.ToUpper <> "ALL" Then
                    ddrequest = Session("ParamValues")(i)
                End If

                ParamNames(i) = ddname
                ParamTypes(i) = ddtype
                ParamFields(i) = ddfield
                If ddrequest <> "" Then
                    ParamValues(i) = ddrequest.ToString.Replace("'", "")
                Else
                    ParamValues(i) = "ALL"
                End If

                'retrieve selected drop-down data
                If ddrequest <> "" And ddrequest <> "All" Then 'And UCase(dv1.Table.Rows(0)("ReportAttributes").ToString) = "SQL" Then

                    'TODO find out what is it about: -------------------
                    If ddrequest.ToUpper = "TRUE" Then
                        ddrequest = "1"
                    ElseIf ddrequest.ToUpper = "FALSE" Then
                        ddrequest = "0"
                    End If
                    '---------------------------------------------------

                    If Trim(WhereText) <> "" Then WhereText = WhereText & " AND "
                    'correct ddfield adding table name
                    'mSql = dv1.Table.Rows(0)("SQLquerytext")
                    mSql = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                    If ddid.Contains(".") Then
                        ddfield = ddid
                    Else
                        ddfield = AddTableNameToField(repid, ddvalue, mSql, er, Session("UserConnProvider"))
                    End If
                    ddfield = FixReservedWords(ddfield, Session("UserConnProvider"))
                    If IsCacheDatabase(Session("UserConnProvider")) Then
                        WhereText = WhereText & "UCASE(" & ddfield & ")="
                    Else
                        WhereText = WhereText & ddfield & "="
                    End If

                    ddrequest = ddrequest.ToString.Replace("'", "")
                    If (ddtype = "datetime") OrElse IsDateTime(ddrequest) Then
                        Dim bTimeStamp As Boolean = IsTimeStamp(Piece(ddfield, ".", 1), Piece(ddfield, ".", 2), Session("UserConnString"), Session("UserConnProvider"))
                        ddrequest = "'" & DateToString(ddrequest, Session("UserConnProvider"), bTimeStamp) & "'"
                    ElseIf (ddtype = "nvarchar") Then
                        ddrequest = "'" & ddrequest & "'"
                    End If

                    If IsCacheDatabase(Session("UserConnProvider")) Then
                        If ddtype = "nvarchar" OrElse ddtype = "datetime" Then
                            WhereText = WhereText & "UCASE(" & ddrequest & ")"
                        Else
                            WhereText = WhereText & ddrequest
                        End If
                    Else
                        WhereText = WhereText & ddrequest
                    End If
                    ddrequest = ddrequest.ToString.Replace("'", "")
                ElseIf ddrequest <> "" And ddrequest = "All" Then

                End If

                'dropdown sql if parameters related
                If Session("ParamsRelated") = "yes" AndAlso i > 0 AndAlso ParamValues(0).ToUpper <> "ALL" Then
                    If ddsql.Trim.ToUpper.StartsWith("SELECT ") Then  'sql
                        'dfld = ParamFields(i).ToString
                        ddsql = "SELECT DISTINCT " & ddfield & ddrSql.Substring(ddrSql.ToUpper.IndexOf(" FROM "))
                        If ddsql.ToUpper.IndexOf(" ORDER BY ") > 0 Then ddsql = ddsql.Substring(0, ddsql.ToUpper.IndexOf(" ORDER BY "))
                        If ddsql.ToUpper.IndexOf(" GROUP BY ") > 0 Then ddsql = ddsql.Substring(0, ddsql.ToUpper.IndexOf(" GROUP BY "))
                        If ddsql.ToUpper.IndexOf(" HAVING ") > 0 Then ddsql = ddsql.Substring(0, ddsql.ToUpper.IndexOf(" HAVING "))
                        If WhereText.Trim <> "" Then
                            If ddsql.ToUpper.Contains(" WHERE ") Then
                                ddsql = ddsql & " AND " & WhereText
                            Else
                                ddsql = ddsql & " WHERE " & WhereText
                            End If
                        End If
                        ddrSql = ddsql & " ORDER BY " & ddfield
                    End If
                End If

                'dropdown label control
                Dim ctllbl As New Label
                ctllbl.Text = ddlabel
                main.Rows(4).Cells(0).Controls.Add(ctllbl)
                'dropdown control
                Dim ctldd As New DropDownList
                ctldd.Items.Clear()
                Dim l0 As New ListItem("All")
                ctldd.Items.Add(l0)
                ctldd.ID = ddid
                If Session("ParamsRelated") = "yes" Then ctldd.AutoPostBack = True

                'draw parameters dropdown
                Dim err As String = ""
                Dim fldnames(0) As String
                fldnames(0) = ddvalue
                'get options dropdown (name,value) - run sql in user database !!! Correct ddsql for user database provider:
                If Session("ParamsRelated") = "yes" AndAlso i > 0 AndAlso ParamValues(0).ToUpper <> "ALL" Then
                    dddsql = ConvertSQLSyntaxFromOURdbToUserDB(ddrSql, Session("UserConnProvider"), err)
                Else
                    dddsql = ConvertSQLSyntaxFromOURdbToUserDB(ddsql, Session("UserConnProvider"), err)
                End If
                Dim dv As DataView = mRecords(dddsql, err, Session("UserConnString"), Session("UserConnProvider"))
                If Not dv Is Nothing AndAlso dv.Count > 0 Then
                    dt = dv.ToTable(1, fldnames)
                    ctldd.Items.Clear()
                    ctldd.Items.Add(l0)
                    If ddid.Contains(".") Then
                        ddfield = ddid
                    Else
                        ddfield = AddTableNameToField(repid, ddvalue, mSql, er, Session("UserConnProvider"))
                    End If
                    ddfield = FixReservedWords(ddfield, Session("UserConnProvider"))
                    Dim bTimeStamp As Boolean = IsTimeStamp(Piece(ddfield, ".", 1), Piece(ddfield, ".", 2), Session("UserConnString"), Session("UserConnProvider"))
                    j = 0
                    For j = 0 To dt.Rows.Count - 1
                        drvalue = Trim(dt.Rows(j)(ddvalue).ToString)
                        If IsDateTime(drvalue) Then
                            drvalue = DateToString(drvalue, Session("UserConnProvider"), bTimeStamp)
                        End If
                        If drvalue <> "" Then
                            'option 
                            Dim li As New ListItem(drvalue)
                            If drvalue = ddrequest Then
                                li.Selected = True
                                ctldd.Text = ddrequest
                            End If
                            ctldd.Items.Add(li)
                        End If
                    Next
                Else
                    If err <> "" Then
                        LabelError.Text = "ERROR:  " & err
                        LabelError.Visible = True
                    End If
                End If
                main.Rows(4).Cells(0).Controls.Add(ctldd)

            Next   ' go to next draw dop-down 

            Session("ParamNames") = ParamNames
            Session("ParamValues") = ParamValues
            Session("ParamTypes") = ParamTypes
            Session("ParamFields") = ParamFields
            Session("ParamsCount") = ParamNames.Length
            j = 0
            'draw submit button
            If n > 0 Then
                Dim ctlsbm As New Button
                ctlsbm.Text = "Apply"
                ctlsbm.ID = "SubmitParams"
                ctlsbm.ToolTip = "Apply Parameters"
                ctlsbm.UseSubmitBehavior = True
                main.Rows(4).Cells(0).Controls.Add(ctlsbm)
            End If

            '----------------------------------------------------------------------------------------------
            'scheduled report with WhereText
            If Not IsPostBack AndAlso Session("sched") = "yes" AndAlso Session("srd") = "6" AndAlso Session("WhereText").ToString.Trim <> "" Then
                WhereText = WhereText & Session("WhereText").ToString
            End If
            '----------------------------------------------------------------------------------------------
            'add additional where statement from cat1 and cat2
            'If Not Session("addwhere") Is Nothing AndAlso Session("addwhere").ToString.Trim <> "" Then
            '    If Trim(WhereText) <> "" Then WhereText = WhereText & " AND "
            '    WhereText = WhereText & Session("addwhere").ToString

            '    LabelAddWhere.Text = Session("addwhere").ToString
            'End If
            If Session("srd") = 11 Then
                If Not Session("addwhere") Is Nothing AndAlso Session("addwhere").ToString.Trim <> "" Then
                    'If Trim(WhereText) <> "" Then WhereText = WhereText & " AND "
                    'WhereText = WhereText & Session("addwhere").ToString
                    WhereText = Session("addwhere").ToString
                    LabelAddWhere.Text = Session("addwhere").ToString
                End If
                Session("dv3") = Nothing
            End If
            '----------------------------------------------------------------------------------------------
            dv3 = Session("dv3")
            'retrieve data for report
            Dim sqltoexport As String = ""
            If Session("dv3") Is Nothing OrElse Session("dv3").Count = 0 OrElse Session("dv3").Table.Rows.Count = 0 Then
                dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "", sqltoexport)
                Session("sqltoexport") = sqltoexport
            ElseIf IsPostBack AndAlso Session("WhereText").ToString.Trim <> WhereText.Trim Then
                dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "", sqltoexport)
                Session("sqltoexport") = sqltoexport
            Else
                dv3 = Session("dv3")
            End If

            If dv3.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
            '----------------------------------------------------------------------------------------------
            Session("WhereText") = WhereText

            'make search statement to filter dv3
            If Not IsPostBack AndAlso Not dv3 Is Nothing AndAlso Not dv3.Table Is Nothing Then
                'dropdowns
                DropDownColumns.Items.Clear()
                Dim i As Integer
                Dim dt As DataTable = dv3.Table
                Dim fldname As String = String.Empty
                Dim frdname As String = String.Empty
                Dim fldtype As String = String.Empty
                DropDownColumns.Items.Add("")
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
                    If Not Session("srchfld") Is Nothing Then
                        'search from session
                        If DropDownColumns.Items(i).Text = Session("srchfld").ToString.Trim Then
                            DropDownColumns.Items(i).Selected = True
                            For j = 0 To DropDownOperator.Items.Count - 1
                                If DropDownOperator.Items(j).Text = Session("srchoper").ToString.Trim Then
                                    DropDownOperator.Items(j).Selected = True
                                End If
                            Next
                            TextBoxSearch.Text = Session("srchval").ToString
                        End If
                    End If
                Next
            End If
            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'Filter dv3
            Dim srch As String = SearchStatement()
            If srch.Trim <> "" Then
                dv3.RowFilter = srch
                Dim dtt As New DataTable
                dtt = dv3.ToTable
                Session("DataToChatAI") = ExportToCSVtext(dtt, Chr(9))
                If dtt.Rows Is Nothing Then
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                    Exit Sub
                End If
                If dtt.Rows.Count > 0 Then
                    LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
                Else
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                End If
                If Session("srch") <> srch Then
                    Session("srch") = srch
                    Session("SortedView") = Nothing
                    Session("SortColumn") = ""
                End If
                If Not Session("sqltoexport").ToUpper.Contains(" WHERE " & srch.ToUpper) < 0 Then
                    If Session("sqltoexport").ToUpper.indexof(" WHERE ") > 1 Then
                        Session("sqltoexport") = Session("sqltoexport").Replace(" WHERE ", " WHERE " & srch & " AND ")
                    ElseIf Session("sqltoexport").ToUpper.indexof(" ORDER BY ") > 1 Then
                        Session("sqltoexport") = Session("sqltoexport").Replace(" ORDER BY ", " WHERE " & srch & "  ORDER BY  ")
                    Else
                        Session("sqltoexport") = Session("sqltoexport") & " WHERE " & srch
                    End If
                End If
            ElseIf srch.Trim = "" AndAlso Session("srch") <> "" Then
                Session("SortedView") = Nothing
                Session("srch") = ""
                Session("SortColumn") = ""
                dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "", sqltoexport)
                Session("DataToChatAI") = ExportToCSVtext(dv3.Table, Chr(9))
                Session("sqltoexport") = sqltoexport
                If dv3.Count > 0 Then
                    LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString
                Else
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                End If
            ElseIf srch.Trim = "" Then
                Session("DataToChatAI") = ExportToCSVtext(dv3.Table, Chr(9))
            End If

            Session("dv3") = dv3

            If LabelAddWhere.Text.Trim = "" Then LabelAddWhere.Text = "<=>"
            LabelAddWhere.ToolTip = "_" & Session("WhereText") & "_" & Session("WhereStm") & "_" & Session("addwhere") & "_" & Session("filter") & "_" & Session("srchstm") & "_"

            'Session("RepTablesCount") =
            If Session("RepTablesCount") = 0 AndAlso GetReportTables(repid).Rows.Count = 1 Then
                Session("RepTable") = GetReportTables(repid).Rows(0)("Tbl1")
                Session("RepTablesCount") = 1
            End If

            If Session("admin") = "super" OrElse Session("admin") = "admin" Then
                LabelSQL.Text = mSql
                LabelSQL.Visible = True
            Else
                LabelSQL.Visible = False
            End If
            If Session("AdvancedUser") = True Then
                LabelSQL.Visible = True
            Else
                LabelSQL.Visible = False
            End If
            ' show report in grid
            BindReportFields()
            If dv3.Table IsNot Nothing AndAlso dv3.Table.Rows.Count = 0 Then
                LabelError.Text = "No data"
                GridView1.DataSource = dv3
                GridView1.Visible = True
                GridView1.DataBind()
            End If

            Session("GridView1DataSource") = GridView1.DataSource.Table

            If Not Request("srd") Is Nothing Then
                Dim srd As String = Request("srd").ToString
                Session("srd") = srd
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Explore_Report_Data"
                If srd = 0 Then
                    Session("filter") = ""
                ElseIf srd = "1" Then   'Excel
                    ButtonExportToExcel_Click("", EventArgs.Empty)
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        LabelExportExcel.Text = "ERROR!! " & ex.Message
                        LabelExportExcel.Visible = True
                        LabelExportExcel.Enabled = True
                    End Try
                    Response.End()
                ElseIf srd = "2" OrElse srd = "10" Then  'CSV
                    If srd = 2 OrElse TextBoxDelimeter.Text = "" Then
                        TextBoxDelimeter.Text = ","
                        Session("delimiter") = ","
                    End If
                    ButtonExportIntoCSV_Click("", EventArgs.Empty)
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        LabelExportExcel.Text = "ERROR!! " & ex.Message
                        LabelExportExcel.Visible = True
                        LabelExportExcel.Enabled = True
                    End Try
                    Response.End()
                ElseIf srd = "14" Then  'XML
                    dv3 = Session("dv3")
                    If dv3 Is Nothing Then 'OrElse dv3.Count = 0
                        LabelError.Text = "No data"
                        Exit Sub
                    End If
                    'If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
                    '    Response.Redirect("~/Default.aspx?msg=SessionExpired")
                    'End If
                    Dim i As Integer
                    Dim appldirExcelFiles, myfile As String
                    'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                    Dim datestr, timestr As String
                    datestr = DateString()
                    timestr = TimeString()
                    myfile = Session("REPORTID") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".xml"
                    appldirExcelFiles = applpath & "Temp\"
                    Session("appldirExcelFiles") = appldirExcelFiles
                    Session("myfile") = myfile
                    Dim adr As String = ExportToXML(dv3.Table, Session("appldirExcelFiles"), Session("myfile"), repid)
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        LabelExportExcel.Text = "ERROR!! " & ex.Message
                        LabelExportExcel.Visible = True
                        LabelExportExcel.Enabled = True
                    End Try
                    Response.End()
                ElseIf srd = "3" Then
                    'Show Report
                    ShowRDL_Click("", EventArgs.Empty)
                ElseIf srd = "4" Then
                    'Export Report to Excel
                    ShowRDL_Click("", EventArgs.Empty)
                ElseIf srd = "5" Then
                    'Export Report to Word
                    ShowRDL_Click("", EventArgs.Empty)
                ElseIf srd = "6" Then
                    'Export Report to PDF
                    ShowRDL_Click("", EventArgs.Empty)
                ElseIf srd = "7" Then
                    'crystal
                    If Session("Crystal") = "ok" Then
                        ' show crystal report
                        ShowCrystalReport()
                    End If
                ElseIf srd = "11" Then 'See Analytics
                    Response.Redirect("Analytics.aspx")
                ElseIf srd = "12" Then 'See Correlations
                    Response.Redirect("Correlation.aspx")
                ElseIf srd = "13" Then 'See AdvancedAnalytics
                    Response.Redirect("AdvancedAnalytics.aspx")
                ElseIf srd = "15" Then 'See Chat with AI
                    Response.Redirect("ChatAI.aspx")
                ElseIf srd = "16" Then 'See DataAI
                    Response.Redirect("DataAI.aspx?pg=expl&srd=0")
                ElseIf srd = "17" Then 'See chart DataAI
                    Response.Redirect("ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=&x2=&y1=&fn=")
                ElseIf srd = "8" Then 'See Statistics on Site
                    ButtonShowStats_Click("", EventArgs.Empty)
                    HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Overall_Statistics"
                ElseIf srd = "9" Then 'Export Statistics to Excel
                    'Session("Stats") = 1
                    dv3 = Session("dv3")
                    If dv3 Is Nothing Then 'OrElse dv3.Count = 0
                        LabelError.Text = "No data"
                        Exit Sub
                    End If
                    Dim err As String = String.Empty
                    Dim dtb As DataTable = CreateStatsTable()
                    If Not Session("SortedView") Is Nothing Then
                        dv3 = Session("SortedView")
                    End If

                    'apply search
                    Dim srch1 As String = SearchStatement()
                    Dim dtt As New DataTable
                    If srch1.Trim <> "" Then
                        dv3.RowFilter = srch1
                    Else
                        dv3.RowFilter = ""
                    End If
                    dtt = dv3.ToTable

                    Dim i As Integer
                    Try
                        For i = 0 To dv3.Table.Columns.Count - 1
                            Dim Row As DataRow = dtb.NewRow()
                            Row("Field") = dv3.Table.Columns(i).Caption
                            Row("Friendly Name") = GetFriendlyFieldName(repid, dv3.Table.Columns(i).Caption)
                            dtb.Rows.Add(Row)
                        Next

                        dtb = CalcStats(dtt, dtb, err)

                    Catch ex As Exception

                    End Try
                    Session("dbstats") = dtb
                    ExportStatsToExcel_Click("", EventArgs.Empty)
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        LabelExportExcel.Text = "ERROR!! " & ex.Message
                        LabelExportExcel.Visible = True
                        LabelExportExcel.Enabled = True
                    End Try
                    Response.End()
                End If
            End If
        Catch ex As Exception
            Dim re As String = ex.Message
            'Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End Try
    End Sub
    Private Sub BindReportFields()
        Dim ReportDataView As DataView = CType(Session("dv3"), DataView)
        If Session("SortedView") IsNot Nothing Then
            ReportDataView = Session("SortedView")
        End If
        'If ColumnTypeIsNumeric(dv3.Table.Columns(i)) Then
        '    GridView1.Columns(i).ControlStyle = Type. 'ItemStyle =  "{0:P}" '.DefaultCellStyle.Format = "N0"
        'End If
        GridView1.DataSource = ReportDataView
        GridView1.DataBind()
    End Sub
    Private Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        If Session("SortedView") Is Nothing Then
            BindReportFields()
        Else
            GridView1.DataSource = Session("SortedView")
            GridView1.DataBind()
        End If
    End Sub

    Protected Sub GridView1_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView1.Sorting
        Dim ReportDataView As DataView = CType(Session("dv3"), DataView)
        If ReportDataView.Count < 2 Then Return
        If String.IsNullOrEmpty(Session("SortColumn")) OrElse
           Session("SortColumn") <> e.SortExpression Then
            Session("SortColumn") = e.SortExpression
            Session("SortDirection") = "ASC"
        ElseIf Session("SortDirection") = "ASC" Then
            Session("SortDirection") = "DESC"
        Else
            Session("SortDirection") = "ASC"
        End If
        Dim SortedView As New DataView(ReportDataView.Table)
        SortedView.RowFilter = ReportDataView.RowFilter
        SortedView.Sort = Session("SortColumn") & " " & Session("SortDirection")
        Session("SortedView") = SortedView
        GridView1.DataSource = SortedView
        GridView1.DataBind()

        'Session("dvsort") = SortedView
        If Request("srd") = 0 Then
            GridView2.Enabled = False
            GridView2.Visible = False
        ElseIf Request("srd") = 8 Then
            GridView2.Enabled = True
            GridView2.Visible = True
            ButtonShowStats_Click("", EventArgs.Empty)
        Else
            GridView2.Enabled = False
            GridView2.Visible = False
        End If


    End Sub

    Protected Sub ButtonExportToExcel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ExportToExcel.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim i As Integer
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = Session("REPORTID") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".xls"
        appldirExcelFiles = applpath & "Temp\"
        Session("appldirExcelFiles") = appldirExcelFiles
        Session("myfile") = myfile
        Dim dv3 As DataView
        dv3 = Session("dv3")

        'apply search
        Dim srch As String = SearchStatement()
        Dim dtt As New DataTable
        If srch.Trim <> "" Then
            dv3.RowFilter = srch
            dtt = dv3.ToTable
            If dtt.Rows Is Nothing Then
                LabelRowCount.Text = "Records returned: " & 0.ToString
                Exit Sub
            End If
            If dtt.Rows.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
        Else
            dv3.RowFilter = ""
            dtt = dv3.ToTable
        End If

        Dim hdr As String = "Data for Report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        'parameters
        If Session("ParamsCount") Is Nothing Then
            Session("ParamsCount") = 0
        Else
            n = Session("ParamsCount")
            For i = 0 To Session("ParamsCount") - 1
                Dim parvalue As String = ParamNames(i) & ":   " & ParamValues(i).ToString
                hdr = hdr + parvalue
                hdr = hdr + System.Environment.NewLine
            Next
        End If
        hdr = hdr + LabelRowCount.Text
        hdr = hdr + System.Environment.NewLine

        res = DataModule.ExportToExcel(dtt, appldirExcelFiles, myfile, hdr, Session("PageFtr"))

        If Left(res, 5) <> "Error" Then
            LabelExportExcel.Text = " "
            'HyperLinkToExcelFile.Dispose()
            'HyperLinkToExcelFile.NavigateUrl = "./Temp/" & myfile
            'HyperLinkToExcelFile.Text = "here"
            LabelExport.Text = "To open Excel report click "
            HyperLinkExport.Dispose()
            HyperLinkExport.NavigateUrl = "./Temp/" & myfile
            HyperLinkExport.Text = "here"

        Else
            LabelExportExcel.Text = res
            'HyperLinkToExcelFile.Text = " "
            LabelExport.Text = res
            HyperLinkExport.Text = " "
        End If
        'Response.ContentType = "application/octet-stream"
        'Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
        'Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
        'Response.End()
    End Sub
    Protected Sub ButtonExportIntoCSV_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonExportIntoCSV.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim i As Integer
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = Session("REPORTID") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Session("appldirExcelFiles") = appldirExcelFiles
        Session("myfile") = myfile
        Dim dv3 As DataView
        dv3 = Session("dv3")

        'apply search
        Dim srch As String = SearchStatement()
        Dim dtt As New DataTable
        If srch.Trim <> "" Then
            dv3.RowFilter = srch
            dtt = dv3.ToTable
            If dtt.Rows Is Nothing Then
                LabelRowCount.Text = "Records returned: " & 0.ToString
                Exit Sub
            End If
            If dtt.Rows.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
        Else
            dv3.RowFilter = ""
            dtt = dv3.ToTable
        End If

        'header
        Dim hdr As String = "Data for Report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        'parameters
        If Session("ParamsCount") Is Nothing Then
            Session("ParamsCount") = 0
        Else
            n = Session("ParamsCount")
            For i = 0 To Session("ParamsCount") - 1
                Dim parvalue As String = ParamNames(i) & ":   " & ParamValues(i).ToString
                hdr = hdr + parvalue
                hdr = hdr + System.Environment.NewLine
            Next
        End If
        hdr = hdr + LabelRowCount.Text
        hdr = hdr + System.Environment.NewLine

        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dtt, appldirExcelFiles, myfile, hdr, Session("PageFtr"), delmtr)
        If Left(res, 5) <> "Error" Then
            LabelExportExcel.Text = "To open CSV file click "
            HyperLinkToCSVFile.Dispose()
            HyperLinkToCSVFile.NavigateUrl = "./Temp/" & myfile
            HyperLinkToCSVFile.Text = "here"
            LabelExport.Text = " "
            'HyperLinkExport.Dispose()
            'HyperLinkExport.NavigateUrl = "./Temp/" & myfile
            'HyperLinkExport.Text = "here"
        Else
            LabelExportExcel.Text = res
            HyperLinkToCSVFile.Text = " "
            LabelExport.Text = res
            'HyperLinkExport.Text = " "
        End If
        'Response.ContentType = "application/octet-stream"
        'Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
        'Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
    End Sub
    Protected Function ShowCrystalReport() As String
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim err As String = String.Empty
        Dim ret As String = String.Empty
        Dim i As Integer
        Try
            'TODO in future, when create crystal on fly
            'Dim dv3 As DataView = Session("dv3")
            'Dim dt As DataTable = dv3.Table
            'Dim repid As String = Session("REPORTID")
            'dt = ProcessLists(repid, dt, err)

            'Show Crystal report
            'parameters to Crystal Header
            Dim hdr As String = String.Empty
            If Not Session("ParamsCount") Is Nothing AndAlso Session("ParamsCount") > 0 Then
                For i = 0 To Session("ParamsCount") - 1
                    Dim parvalue As String = Session("ParamNames")(i) & ":   " & Session("ParamValues")(i).ToString
                    hdr = hdr + parvalue
                    hdr = hdr + ", "
                Next
            End If
            Session("ReportHeader") = hdr
            Response.Redirect("ViewCrystal.aspx?see=yes")
            Return ret
        Catch ex As Exception
            Return "ERROR!! " & ex.Message
        End Try
    End Function
    Protected Sub ShowRDL_Click(sender As Object, e As EventArgs) Handles ShowRDL.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        'Response.Redirect("ReportViews.aspx")
        Dim err As String = String.Empty
        Dim ret As String = String.Empty
        Try
            'Show RDL report
            ButtonDownloadFile.Visible = False
            ButtonDownloadFile.Enabled = False
            If Not Session("srd") Is Nothing Then
                Response.Redirect("ReportViews.aspx?see=yes&srd=" & Session("srd").ToString)
            Else
                Response.Redirect("ReportViews.aspx?see=yes")
            End If

            Exit Sub
            ''Get the physical path to the file.
            'Dim FilePath As String = MapPath(pdfpath)
            ''Write the file directly to the HTTP output stream.
            'Response.WriteFile(FilePath)
            ''Response.End()  'uncomment for testing the report viewer
        Catch ex As Exception
            If ex.Message <> "Thread was being aborted." Then
                LabelError.Text = ex.Message
                If Not ex.InnerException Is Nothing Then
                    LabelError.Text = ex.InnerException.Message
                    If Not ex.InnerException.InnerException Is Nothing Then
                        LabelError.Text = ex.InnerException.InnerException.Message
                    End If
                End If
                LabelError.Text = "Error during exporting report to PDF...  " & LabelError.Text & " , " & err & " Oppening in the Report Viewer"
                LabelError.Visible = True
                ButtonDownloadFile.Visible = False
                ButtonDownloadFile.Enabled = False
                Response.Redirect("ReportViews.aspx?see=yes&er=" & LabelError.Text)
            End If
        End Try
    End Sub
    Protected Sub ButtonDownloadFile_Click(sender As Object, e As EventArgs) Handles ButtonDownloadFile.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim reppath As String = Session("pdfpath").ToString
        Dim repfile As String = reppath.Substring(reppath.LastIndexOf("\"))
        Response.ContentType = "application/octet-stream"
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & repfile)
        Response.TransmitFile(reppath)  '(Server.MapPath("~/logfile.txt"))
        Response.End()
    End Sub

    Protected Sub ButtonShowStats_Click(sender As Object, e As EventArgs) Handles ButtonShowStats.Click
        Session("Stats") = 1
        dv3 = Session("dv3")
        If dv3 Is Nothing OrElse dv3.Count = 0 Then
            LabelError.Text = "No data"
            Exit Sub
        End If
        trStatLabel.Visible = True
        trReportStats.Visible = True
        GridView2.Enabled = True
        GridView2.Visible = True
        Dim err As String = String.Empty
        Dim dtb As DataTable = CreateStatsTable()
        If Not Session("SortedView") Is Nothing Then
            dv3 = Session("SortedView")
        End If
        Dim i As Integer
        Dim nw As String = String.Empty
        Dim er As String = String.Empty

        'apply search
        Dim srch1 As String = SearchStatement()
        Dim dtt As New DataTable
        If srch1.Trim <> "" Then
            dv3.RowFilter = srch1
        Else
            dv3.RowFilter = ""
        End If
        dtt = dv3.ToTable

        Try
            For i = 0 To dv3.Table.Columns.Count - 1
                Dim Row As DataRow = dtb.NewRow()
                Row("Field") = dv3.Table.Columns(i).Caption
                Row("Friendly Name") = GetFriendlyFieldName(repid, dv3.Table.Columns(i).Caption)
                dtb.Rows.Add(Row)
            Next
            dtb = CalcStats(dtt, dtb, err)
            Session("dbstats") = dtb
            GridView2.DataSource = dtb.DefaultView
            GridView2.DataBind()
            Session("GridView2DataSource") = GridView2.DataSource.Table
            BindReportFields()
            Session("DataToChatAI") = ExportToCSVtext(GridView2.DataSource.Table, Chr(9))
        Catch ex As Exception
            err = "ERROR!! " & err & "  " & ex.Message
            Exit Sub
        End Try
    End Sub
    Protected Sub ExportStatsToExcel_Click(sender As Object, e As EventArgs) Handles ExportStatsToExcel.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = Session("REPORTID") & "Stats" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".xls"
        appldirExcelFiles = applpath & "Temp\"
        Session("appldirExcelFiles") = appldirExcelFiles
        Session("myfile") = myfile
        Dim hdr As String = "Statistics for Report:     " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        'parameters
        If Session("ParamsCount") Is Nothing Then
            Session("ParamsCount") = 0
        Else
            n = Session("ParamsCount")
            For i = 0 To Session("ParamsCount") - 1
                Dim parvalue As String = ParamNames(i) & ":   " & ParamValues(i).ToString
                hdr = hdr + parvalue
                hdr = hdr + System.Environment.NewLine
            Next
        End If
        Dim dtb As DataTable
        dtb = Session("dbstats")
        res = DataModule.ExportToExcel(dtb, appldirExcelFiles, myfile, hdr, Session("PageFtr"))
        If Left(res, 5) <> "Error" Then
            LabelExportExcel.Text = "To open Excel report stats click "
            HyperLinkToCSVFile.Dispose()
            HyperLinkToCSVFile.NavigateUrl = "./Temp/" & myfile
            HyperLinkToCSVFile.Text = "here"
            LabelExport.Text = "To open Excel report stats click "
            HyperLinkExport.Dispose()
            HyperLinkExport.NavigateUrl = "./Temp/" & myfile
            HyperLinkExport.Text = "here"
        Else
            LabelExportExcel.Text = res
            HyperLinkToCSVFile.Text = " "
            LabelExport.Text = res
            HyperLinkExport.Text = " "
        End If
        'Response.ContentType = "application/octet-stream"
        'Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
        'Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
    End Sub
    Private Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.Header Then
            For Each cell As TableCell In e.Row.Cells
                If cell.HasControls Then
                    Dim btn As LinkButton = CType(cell.Controls(0), LinkButton)
                    If btn IsNot Nothing Then
                        If Session("SortColumn") = btn.CommandArgument Then
                            Dim image As New WebControls.Image
                            If Session("SortDirection") = "ASC" Then
                                image.ImageUrl = "~\Controls\Images\arrow-down.bmp"
                            ElseIf Session("SortDirection") = "DESC" Then
                                image.ImageUrl = "~\Controls\Images\arrow-up.bmp"
                            End If
                            cell.Controls.Add(image)
                        End If
                    End If
                End If
            Next
        ElseIf e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow AndAlso ((Session("Stats") = 1 AndAlso Not Session("dbstats") Is Nothing) Or (Session("RepTablesCount") = 1 AndAlso Not Session("RepTable") Is Nothing)) Then
            Dim i As Integer
            Dim fld As String = String.Empty
            Dim dvs3 As DataView = Session("dv3")
            For i = 0 To e.Row.Cells.Count - 1
                fld = dvs3.Table.Columns(i).Caption
                If Session("Stats") = 1 AndAlso Not Session("dbstats") Is Nothing Then
                    'color of background
                    Dim val As Double
                    Dim maxval As Double
                    Dim dts2 As DataTable = Session("dbstats")
                    If IsDBNull(dts2.Rows(i)("Max")) OrElse dts2.Rows(i)("Max") = 0 Then
                        Continue For
                    End If
                    If dts2.Rows(i)("Field") = fld Then
                        maxval = CType(dts2.Rows(i)("Max"), Double)
                        If IsNumeric(e.Row.Cells(i).Text) Then
                            val = CType(e.Row.Cells(i).Text, Double)
                            If maxval > 0 AndAlso val >= maxval * 0.9 Then
                                e.Row.Cells(i).BackColor = Color.FromArgb(152, 203, 203)
                            ElseIf maxval > 0 AndAlso val >= maxval * 0.7 AndAlso val < maxval * 0.9 Then
                                e.Row.Cells(i).BackColor = Color.FromArgb(160, 213, 213)
                            ElseIf maxval > 0 AndAlso val >= maxval * 0.5 AndAlso val < maxval * 0.7 Then
                                e.Row.Cells(i).BackColor = Color.FromArgb(172, 223, 223)
                            End If

                        End If
                    End If
                End If
                If ColumnTypeIsNumeric(dv3.Table.Columns(i)) Then
                    'TODO convert exponential notation to number
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
                'If Session("RepTablesCount") = 1 AndAlso Not Session("RepTable") Is Nothing Then
                '    If e.Row.Cells(i).Text.Contains("~") Then
                '        'if column is a child
                '        Dim j As Integer
                '        Dim dvj As DataView
                '        Dim sqls As String
                '        Dim alnk As String = String.Empty
                '        Dim childtbl As String = String.Empty
                '        Dim parentid As String = String.Empty
                '        Dim cellval As String = e.Row.Cells(i).Text
                '        childtbl = cellval.Substring(0, cellval.IndexOf("~"))
                '        parentid = cellval.Substring(cellval.IndexOf("~"))

                '        If IsCacheDatabase() Then
                '            sqls = "Select * FROM OURReportSQLquery WHERE (ReportId Like '" & Session("REPORTID") & "%' AND DOING = 'JOIN' AND ((UCASE(Tbl1)='" & Session("RepTable").ToString.ToUpper & "') AND (UCASE(Tbl2)='" & childtbl.ToUpper & "'))) "
                '        Else
                '            sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId LIKE '" & Session("REPORTID") & "%' AND DOING = 'JOIN' AND ((Tbl1='" & Session("RepTable").ToString & "') AND (Tbl2='" & childtbl & "'))) "
                '        End If


                '        'If IsCacheDatabase() Then
                '        '    sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & Session("REPORTID") & "' AND DOING = 'JOIN' AND ((UCASE(Tbl1)='" & Session("RepTable").ToString.ToUpper & "') AND (UCASE(Tbl2)='" & childtbl.ToUpper & "'))) "
                '        'Else
                '        '    sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & Session("REPORTID") & "' AND DOING = 'JOIN' AND ((Tbl1='" & Session("RepTable").ToString & "') AND (Tbl2='" & childtbl & "'))) "
                '        'End If


                '        'If IsCacheDatabase() Then
                '        '    sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & Session("REPORTID") & "' AND DOING = 'JOIN' AND ((UCASE(Tbl1)='" & Session("RepTable").ToString.ToUpper & "' AND UCASE(Tbl1Fld1)='" & UCase(fld) & "') OR (UCASE(Tbl2)='" & Session("RepTable").ToString.ToUpper & "' AND UCASE(Tbl2Fld2)='" & UCase(fld) & "'))) "
                '        'Else
                '        '    sqls = "SELECT * FROM OURReportSQLquery WHERE (ReportId = '" & Session("REPORTID") & "' AND DOING = 'JOIN' AND ((Tbl1='" & Session("RepTable").ToString & "' AND Tbl1Fld1='" & fld & "') OR (Tbl2='" & Session("RepTable").ToString & "' AND Tbl2Fld2='" & fld & "'))) "
                '        'End If
                '        dvj = mRecords(sqls)
                '        If Not dvj Is Nothing AndAlso dvj.Count > 0 AndAlso dvj.Table.Rows.Count > 0 Then
                '            'If dvj.Table.Rows(j)("Param2") <> "parent to child" Then
                '            '    For j = 0 To dvj.Table.Rows.Count - 1
                '            '        alnk = alnk & dvj.Table.Rows(j)("Indx") & "_"
                '            '    Next
                '            '    If alnk.EndsWith("_") Then
                '            '        alnk = alnk.Substring(0, alnk.Length - 1)
                '            '    End If
                '            'End If

                '            'alnk = cellval

                '            Dim myLink As HyperLink = New HyperLink
                '            myLink.Text = childtbl
                '            myLink.NavigateUrl = "ShowReport.aspx?cld=" & childtbl & "&parntid=" & parentid
                '            myLink.Target = "_blank"
                '            e.Row.Cells(i).Controls.Add(myLink)
                '        End If
                '    End If
                'End If
            Next
        End If
    End Sub

    Private Sub ShowReport_Unload(sender As Object, e As EventArgs) Handles Me.Unload
        If Not IsPostBack AndAlso Not Session("appldirExcelFiles") Is Nothing AndAlso Not Session("myfile") Is Nothing Then
            'File.Delete(Session("appldirExcelFiles") & Session("myfile"))
        End If
    End Sub

    'Private Sub ButtonShowGraph_Click(sender As Object, e As EventArgs) Handles ButtonShowGraph.Click
    '    'show selected graph
    '    If Not Session("srd") Is Nothing AndAlso Session("srd") > 0 Then
    '        Response.Redirect("ReportViews.aspx?graph=yes&srd=" & Session("srd").ToString)
    '    Else
    '        Response.Redirect("ReportViews.aspx?graph=yes&grph=" & Session("graph"))
    '    End If

    '    Exit Sub
    'End Sub

    'Private Sub DropDownGroupGraphs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownGroupGraphs.SelectedIndexChanged
    '    Session("graph") = DropDownGroupGraphs.SelectedItem.Text & ",indx=" & DropDownGroupGraphs.SelectedItem.Value.ToString
    'End Sub
    'Protected Sub DropDownColumns_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownColumns.SelectedIndexChanged  ', CheckBoxAsNumber.CheckedChanged
    'depended of type
    '    DropDownOperator.Items.Clear()
    '    DropDownOperator.Items.Add("")
    '    DropDownOperator.Items.Add("=")
    '    DropDownOperator.Items.Add("<>")
    '    DropDownOperator.Items.Add("<")
    '    DropDownOperator.Items.Add("<=")
    '    DropDownOperator.Items.Add(">")
    '    DropDownOperator.Items.Add(">=")
    '    DropDownOperator.Items.Add("IN")
    '    DropDownOperator.Items.Add("Not IN")

    '    If Not TblSQLqueryFieldIsNumeric(Session("REPORTID"), DropDownColumns.SelectedValue, Session("UserConnString"), Session("UserConnProvider")) Then 'AndAlso CheckBoxAsNumber.Checked = False Then
    '        DropDownOperator.Items.Add("Contains")
    '        DropDownOperator.Items.Add("Not Contains")
    '        DropDownOperator.Items.Add("StartsWith")
    '        DropDownOperator.Items.Add("Not StartsWith")
    '        DropDownOperator.Items.Add("EndsWith")
    '        DropDownOperator.Items.Add("Not EndsWith")
    '    End If
    'End Sub

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
    Private Function SearchStatement() As String
        Dim srch As String = String.Empty
        If DropDownColumns.SelectedValue.ToString.Trim <> "" AndAlso DropDownOperator.SelectedValue.ToString.Trim <> "" AndAlso TextBoxSearch.Text <> "" Then
            Dim myprovider = Session("UserConnProvider")
            Dim srchfld As String = "[" & DropDownColumns.SelectedValue & "] "
            Dim oper As String = DropDownOperator.SelectedValue
            Dim val As String = TextBoxSearch.Text
            Dim HasNot As Boolean = False
            Session("srchfld") = DropDownColumns.SelectedValue 'srchfld
            Session("srchoper") = oper
            Session("srchval") = val
            HasNot = oper.ToUpper.Contains("NOT")
            If Not TblSQLqueryFieldIsNumeric(repid, srchfld, Session("UserConnString"), Session("UserConnProvider")) Then
                If oper.ToUpper.Contains("STARTSWITH") Then
                    If HasNot Then
                        oper = "Not Like"
                    Else
                        oper = "Like"
                    End If
                    srch = srchfld & oper & " '" & val & "%'"
                ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                    If HasNot Then
                        oper = "Not Like"
                    Else
                        oper = "Like"
                    End If
                    srch = srchfld & oper & " '%" & val & "'"
                ElseIf oper.ToUpper.Contains("CONTAINS") Then
                    If HasNot Then
                        oper = "Not Like"
                    Else
                        oper = "Like"
                    End If
                    srch = srchfld & oper & " '%" & val & "%'"
                ElseIf oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                    srch = srchfld & oper & " ('" & val.Replace("(", "").Replace(")", "").Replace(",", "','") & "')"
                End If
                If srch.Trim = "" Then
                    srch = srchfld & oper & " '" & val.Replace("'", "") & "'"
                End If
            End If
            If TblSQLqueryFieldIsNumeric(repid, srchfld, Session("UserConnString"), Session("UserConnProvider")) Then
                If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                    srch = srchfld & oper & " (" & val.Replace("(", "").Replace(")", "").Replace("'", "") & ")"
                End If
                If srch.Trim = "" Then
                    srch = srchfld & oper & " " & val.Replace("'", "")
                End If
            End If
        Else
            Session("srchfld") = Nothing
            Session("srchoper") = Nothing
            Session("srchval") = Nothing
            DropDownColumns.SelectedIndex = 0
            DropDownOperator.SelectedIndex = 0
            TextBoxSearch.Text = String.Empty
        End If
        Session("Search") = srch
        Return srch
    End Function
    Private Sub TextBoxDelimeter_TextChanged(sender As Object, e As EventArgs) Handles TextBoxDelimeter.TextChanged
        Dim delim As String = TextBoxDelimeter.Text
        If delim = String.Empty Then delim = ","
        Session("delimiter") = delim
    End Sub
    'Private Function RetrieveDataForReportOld() As DataView
    '    'NOT IN USE
    '    'retrieve data for report
    '    Dim r As String = String.Empty
    '    'add request drop-downs and constract filter
    '    If UCase(dv1.Table.Rows(0)("ReportAttributes").ToString) = "SQL" Then  'SQL statement
    '        mSql = dv1.Table.Rows(0)("SQLquerytext").ToString
    '        'find WHERE, ORDER BY, and GROUP BY: add to WHERE or place WHERE in front of GROUP, and place ORDER BY in the end of SQL
    '        Dim wherenum, ordernum, groupnum As Integer
    '        wherenum = 0
    '        ordernum = 0
    '        groupnum = 0
    '        wherenum = InStr(UCase(mSql), " WHERE ")
    '        ordernum = InStr(UCase(mSql), " ORDER BY ")
    '        groupnum = InStr(UCase(mSql), " GROUP BY ")

    '        If wherenum > 0 Then        'mSql has " WHERE "
    '            Dim sqlWhere As String = mSql.Substring(wherenum + 6, Len(mSql) - wherenum - 6)
    '            sqlWhere = sqlWhere.Replace("""", "'")
    '            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data.") Then
    '                Dim parts = sqlWhere.Split(" "c)
    '                Dim sWhere As String = String.Empty
    '                For p As Integer = 0 To parts.Length - 1
    '                    If parts(p).Contains(".") AndAlso Not parts(p).Contains("'%'") Then
    '                        parts(p) = parts(p).Replace("'", """")
    '                    End If
    '                    If sWhere = String.Empty Then
    '                        sWhere = parts(p)
    '                    Else
    '                        sWhere &= " " & parts(p)
    '                    End If
    '                Next
    '                sqlWhere = sWhere
    '            End If
    '            If Trim(WhereText) <> "" Then
    '                mSql = mSql.Substring(0, wherenum + 6) & WhereText & " AND " & sqlWhere
    '            Else
    '                mSql = mSql.Substring(0, wherenum + 6) & sqlWhere
    '            End If

    '        Else    'mSql does not have " WHERE "
    '            If groupnum > 0 Then       'mSql has " GROUP BY " and mSql does not have " WHERE "
    '                If Trim(WhereText) <> "" Then mSql = mSql.Substring(0, groupnum - 1) & " WHERE " & WhereText & " " & mSql.Substring(groupnum, Len(mSql) - groupnum)
    '            Else                       'mSql does not have " GROUP BY " and mSql does not have " WHERE "
    '                If ordernum > 0 Then      'mSql has " ORDER BY " and mSql does not have " WHERE "
    '                    If Trim(WhereText) <> "" Then mSql = mSql.Substring(0, ordernum - 1) & " WHERE " & WhereText & " " & mSql.Substring(ordernum, Len(mSql) - ordernum)
    '                Else                      'mSql does not have " ORDER BY " and mSql does not have " WHERE "
    '                    'if no GROUP and no ORDER
    '                    If Trim(WhereText) <> "" Then mSql = mSql & " WHERE " & WhereText
    '                    'ORDER BY in the end always
    '                    If Trim(dirsort) <> "" Then mSql = mSql & " ORDER BY " & dirsort   'where dirsort is coming from?
    '                End If
    '            End If
    '        End If
    '        Dim err As String = String.Empty
    '        'run sql in user database !!! Correct mSql for user database provider:
    '        mSql = ConvertSQLSyntaxFromOURdbToUserDB(mSql, Session("UserConnProvider"), err)
    '        err = ""
    '        'Data for report by SQL statement from the user database
    '        dv3 = mRecords(mSql, err, Session("UserConnString"), Session("UserConnProvider"))
    '        r = err
    '        Session("WhereText") = WhereText
    '    Else 'sp                                                  'Stored procedure
    '        mSql = Trim(dv1.Table.Rows(0)("SQLquerytext"))
    '        '!!!!!!!!!!!!!!!!!!!!!  Should be this one:
    '        If Session("UserConnProvider").StartsWith("InterSystems.Data.") And mSql.Contains("||") Then
    '            Dim cls As String = Piece(mSql, "||", 1)
    '            Dim sp As String = Piece(mSql, "||", 2)
    '            cls = Piece(cls, ".", 1, Pieces(cls, ".") - 1).Replace(".", "_")
    '            mSql = cls & "." & sp
    '        End If
    '        Dim Nparameters As Integer = -1
    '        If Session("NparamNames") IsNot Nothing Then Nparameters = Session("NparamNames")
    '        If Nparameters = -1 Then
    '            r = ExtractParameters(mSql, WhereText, Nparameters, Session("ParamNames"), Session("ParamTypes"), Session("ParamValues"), Session("UserConnString"), Session("UserConnProvider"))
    '        End If
    '        dv3 = mRecordsFromSP(mSql, Session("ParamsCount"), Session("ParamNames"), Session("ParamTypes"), Session("ParamValues"), Session("UserConnString"), Session("UserConnProvider"))  'Data for report from SP
    '    End If
    '    If dv3 Is Nothing OrElse dv3.Count = 0 Then
    '        LabelRowCount.Text = "Records returned: " & 0.ToString
    '        If r <> "" Then
    '            MessageBox.Show("ERROR!! " & r, "Data for report ", "NoData", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
    '        End If
    '        Return dv3
    '    End If

    '    Dim er As String = String.Empty
    '    Session("mySQL") = mSql
    '    If Session("UserConnProvider") = "MySql.Data.MySqlClient" OrElse (Session("UserConnProvider") = "" And Session("OURConnProvider") = "MySql.Data.MySqlClient") Then
    '        dv3 = ConvertMySqlTable(dv3.Table, er).DefaultView
    '    ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
    '        dv3 = ConvertOracleTable(dv3.Table, er).DefaultView
    '        dv3 = CorrectDataset(dv3.Table, er).DefaultView
    '    Else
    '        dv3 = CorrectDataset(dv3.Table, er).DefaultView  'from single quote
    '    End If
    '    If CheckBoxHideDuplicates.Checked AndAlso Not dv3 Is Nothing AndAlso dv3.Count > 0 Then
    '        dv3 = dv3.ToTable(True).DefaultView
    '    End If
    '    If dv3.Count > 0 Then
    '        LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString
    '    Else
    '        LabelRowCount.Text = "Records returned: " & 0.ToString
    '        Return dv3
    '    End If
    '    Session("dv3") = dv3
    '    '----------------------------------------------------------------------------------------------
    '    Return dv3
    'End Function
    Private Sub GridView2_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView2.RowDataBound
        If e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 0 To e.Row.Cells.Count - 1
                If IsNumeric(e.Row.Cells(i).Text) Then
                    e.Row.Cells(i).Text = ExponentToNumber(e.Row.Cells(i).Text.ToUpper)
                End If
            Next
        End If
    End Sub
    Private Sub CheckBoxHideDuplicates_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxHideDuplicates.CheckedChanged
        'hide or unhide duplicates
        Dim ret As String = String.Empty
        Dim sqltoexport As String = ""
        dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "", sqltoexport)
        Session("sqltoexport") = sqltoexport
        'make search statement to filter dv3
        Dim srch As String = SearchStatement()
        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        If Not Session("filter") Is Nothing AndAlso Session("filter").ToString.Trim <> "" Then
            If srch.Trim = "" Then
                srch = Session("filter").ToString.Trim
            Else
                srch = srch.Trim & " AND " & Session("filter").ToString.Trim
                LabelAddWhere.Text = srch
            End If
            'LabelAddWhere.Text = Session("filter")
        End If
        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        'Filter dv3
        If srch.Trim <> "" Then
            Session("filter") = srch
            dv3.RowFilter = srch
            Dim dtt As New DataTable
            dtt = dv3.ToTable
            If dtt.Rows Is Nothing Then
                LabelRowCount.Text = "Records returned: " & 0.ToString
                'Exit Sub
            End If
            If dtt.Rows.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
            If Session("sqltoexport").ToUpper.indexof(" WHERE ") > 1 Then
                Session("sqltoexport") = Session("sqltoexport").Replace(" WHERE ", " WHERE " & srch & " AND ")
            ElseIf Session("sqltoexport").ToUpper.indexof(" ORDER BY ") > 1 Then
                Session("sqltoexport") = Session("sqltoexport").Replace(" ORDER BY ", " WHERE " & srch & "  ORDER BY  ")
            Else
                Session("sqltoexport") = Session("sqltoexport") & " WHERE " & srch
            End If

        Else
            If dv3 Is Nothing OrElse dv3.Count = 0 Then
                LabelRowCount.Text = "Records returned: " & 0.ToString
                'Exit Sub
            Else
                LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString
                dv3.RowFilter = ""
            End If
        End If
        Session("dv3") = dv3
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
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
        If Session("srd") = "8" Then
            Response.Redirect("ShowReport.aspx?srd=8")
        Else
            Response.Redirect("ShowReport.aspx?srd=0")
        End If

    End Sub
    Private Sub lnkExportGrid1_Click(sender As Object, e As EventArgs) 'Handles lnkExportGrid1.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "ExploreData" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView1DataSource")
        'header
        Dim hdr As String = "Explore data of report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
    End Sub

    Private Sub lnkExportGrid2_Click(sender As Object, e As EventArgs) 'Handles lnkExportGrid2.Click

        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "Statistics" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView2DataSource")
        'header
        Dim hdr As String = "Statistics for report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
    End Sub

    Private Sub btnExportToTable_Click(sender As Object, e As EventArgs) Handles btnExportToTable.Click
        Dim er As String = String.Empty
        Dim tblname As String = cleanText(TextBoxExportTableName.Text.Trim)
        Dim tblfriendlyname As String = cleanText(TextBoxExportTableName.Text)
        tblname = tblname.ToLower.Replace(" ", "")
        If TableExists(tblname, Session("UserConnString"), Session("UserConnProvider"), er) Then
            'create new table correcting name because it exist
            tblname = tblname & "_" & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "")
            tblname = tblname.ToLower.Replace(" ", "")
        End If
        TextBoxExportTableName.Text = tblname
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "UserData." & tblname
            End If
        End If
        Dim reportid As String = String.Empty
        er = CreateReport(tblname, Session("UserDB").trim, reportid)

        If er.StartsWith("ERROR!!") OrElse reportid.Trim = "" Then
            MessageBox.Show(er, "Create Report Error", "CreateError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Exit Sub
        End If

        'create new table
        Try
            Dim dri As DataTable = GetReportInfo(repid)
            If UCase(dri.Rows(0)("ReportAttributes").ToString) = "SQL" Then  'SQL statement
                If Session("sqltoexport") IsNot Nothing AndAlso Session("sqltoexport").trim <> "" Then
                    Dim sSql As String = Session("sqltoexport").ToString.Trim
                    If Session("UserConnProvider").StartsWith("MySql") Then
                        'sSql = sSql.Replace(" FROM ", " INTO " & Session("dbname") & "." & tblname & " FROM ")
                        sSql = "CREATE TABLE " & tblname & " " & sSql & ";"
                    ElseIf Session("UserConnProvider").StartsWith("Sql") Then
                        sSql = sSql.Replace(" FROM ", " INTO " & tblname & " FROM ")
                    ElseIf Session("UserConnProvider").StartsWith("Oracle") Then
                        sSql = "INSERT INTO " & tblname & " AS " & sSql
                    ElseIf Session("UserConnProvider").StartsWith("Npgsql") Then
                        sSql = sSql.Replace(" FROM ", " INTO " & tblname & " FROM ")
                    ElseIf Session("UserConnProvider").StartsWith("InterSystems") Then
                        'MessageBox.Show("Export Data Not Supported yet", "Export Data Not Supported yet", "ExportData", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                        'Exit Sub
                        If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
                        If tblname.IndexOf(".") < 0 Then
                            tblname = "UserData." & tblname
                        End If

                        'use dv3 to create table tblname
                        Dim collen As Integer = 0
                        Dim selflds As String = String.Empty
                        Dim namflds As String = String.Empty
                        Dim fldtype(dv3.Table.Columns.Count) As String
                        Dim selfield(dv3.Table.Columns.Count) As String
                        Dim namfield(dv3.Table.Columns.Count) As String
                        For i = 0 To dv3.Table.Columns.Count - 1
                            selfield(i) = dv3.Table.Columns(i).Caption
                            fldtype(i) = dv3.Table.Columns(i).DataType.Name
                            namfield(i) = dv3.Table.Columns(i).Caption
                            If dv3.Table.Columns(i).DataType.Name = "String" Then
                                For j = 0 To dv3.Table.Rows.Count - 1
                                    collen = Max(collen, dv3.Table.Rows(j)(i).ToString.Length)
                                Next
                                collen = collen + 10
                                If selflds.Trim = "" Then
                                    selflds = selfield(i) & " " & fldtype(i).Replace("String", "nvarchar") & "(" & collen.ToString & ")" & " DEFAULT NULL"
                                    namflds = namfield(i)
                                Else
                                    selflds = selflds & "," & selfield(i) & " " & fldtype(i).Replace("String", "nvarchar") & "(" & collen.ToString & ")" & " DEFAULT NULL"
                                    namflds = namflds & "," & namfield(i)
                                End If

                            ElseIf dv3.Table.Columns(i).DataType.Name = "DateTime" OrElse dv3.Table.Columns(i).DataType.Name = "Decimal" Then
                                If selflds.Trim = "" Then
                                    selflds = selfield(i) & " " & fldtype(i) & " DEFAULT NULL"
                                    namflds = namfield(i)
                                Else
                                    selflds = selflds & "," & selfield(i) & " " & fldtype(i) & " DEFAULT NULL"
                                    namflds = namflds & "," & namfield(i)
                                End If

                            Else
                                If selflds.Trim = "" Then
                                    selflds = selfield(i) & " INT DEFAULT NULL"
                                    namflds = namfield(i)
                                Else
                                    selflds = selflds & "," & selfield(i) & " INT DEFAULT NULL"
                                    namflds = namflds & "," & namfield(i)
                                End If
                            End If

                        Next

                        'TODO create tblname with fields selflds
                        Dim csql As String
                        csql = "CREATE TABLE " & tblname & " (" & selflds & ") "


                        er = ExequteSQLquery(csql, Session("UserConnString"), Session("UserConnProvider"))

                        sSql = sSql.Replace("*", namflds)
                        sSql = "INSERT INTO " & tblname & " (" & namflds & ") " & sSql
                    Else
                        sSql = sSql.Replace(" FROM ", " INTO " & tblname & " FROM ")
                    End If
                    er = ExequteSQLquery(sSql, Session("UserConnString"), Session("UserConnProvider"))
                    If er <> "Query executed fine." Then
                        MessageBox.Show(er, "Export Data Error", "ExportDataError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                        Exit Sub
                    End If
                End If
                Else 'SP
                Dim dt As DataTable = dv3.ToTable
                'TODO
                'create table

                'loop to import rows from dt

            End If
            Session("REPORTID") = reportid

            'save report
            'update xsd and rdl
            Dim repfile As String = String.Empty
            Session("dv3") = Nothing
            er = UpdateXSDandRDL(reportid, Session("UserConnString"), Session("UserConnProvider"), repfile)
            er = er & ", " & CreateCleanReportColumnsFieldsItems(reportid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            'If HasReportColumns(reportid) AndAlso Not ReportItemsExist(repid) Then
            '    Dim retr As String = CreateReportItems(reportid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            '    If retr.StartsWith("ERROR!!") Then
            '        LabelError.Text = "Report info is not updated. " & retr
            '        LabelError.Visible = True
            '        Exit Sub
            '    End If
            'End If
            Session("sqltoexport") = ""
            Response.Redirect("ShowReport.aspx?srd=0")
        Catch ex As Exception
            'btnOpenReport.Enabled = True
            'btnOpenReport.Visible = True
        End Try

    End Sub
    Private Function CreateReport(ByVal tblname As String, ByVal userdb As String, Optional ByRef reportid As String = "") As String
        Dim SQLq As String
        Dim dv5 As DataView
        Dim repttl As String = "Data exported into " & tblname & " on " & Now.ToString.Replace(":", "-").Replace("/", "-")
        Dim msg As String = String.Empty
        Dim SQLtext As String = "SELECT * FROM " & tblname
        Dim ret As String = String.Empty

        repid = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        repid = repid.Replace(" ", "").Replace("@", "").Replace(".", "").Replace("+", "").Replace("-", "").Replace("\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "").Replace("""", "").Replace("'", "").Replace(",", "")

        dv5 = mRecords("SELECT * FROM [OURReportInfo] WHERE ([ReportID]='" & repid & "')")
        If dv5.Count = 0 OrElse
           dv5.Table Is Nothing OrElse
           dv5.Table.Rows.Count = 0 Then

            SQLq = "INSERT INTO [OURReportInfo] "
            SQLq &= "([ReportId],"
            SQLq &= "[ReportName],"
            SQLq &= "[ReportTtl],"
            SQLq &= "[ReportType],"
            SQLq &= "[ReportAttributes],"
            SQLq &= "[Param7type],"
            SQLq &= "[Param9type],"
            SQLq &= "[ReportDB])"
            SQLq &= "VALUES "
            SQLq &= "('" & repid & "',"
            SQLq &= "'" & repid & "',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'rdl',"
            SQLq &= "'sql',"
            SQLq &= "'user',"
            SQLq &= "'portrait',"
            SQLq &= "'" & userdb & "')"
            msg = ExequteSQLquery(SQLq)
            ret = msg & ", " & SaveSQLquery(repid, SQLtext)
            SQLq = "UPDATE OURReportInfo SET Comments='" & repttl & " by query: " & Session("sqltoexport").ToString.Trim.Replace("[", "").Replace("]", "").Replace("'", "") & "'  WHERE (ReportID='" & repid & "')"
            ret = ExequteSQLquery(SQLq)
        Else
            ret = "ERROR!! Report ID should be unique, import denied..."
            Return ret
        End If
        If repid <> "" And Session("logon") <> "" Then
            SQLq = "INSERT INTO [OURReportView] ("
            SQLq &= "[ReportID],"
            SQLq &= "[ReportTemplate],"
            SQLq &= "[ReportTitle],"
            SQLq &= "[CreatedBy],"
            SQLq &= "[DateCreated],"
            SQLq &= "[UpdatedBy],"
            SQLq &= "[LastUpdate])"
            SQLq &= "VALUES ("
            SQLq &= "'" & repid & "',"
            SQLq &= "'Tabular',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'" & Session("logon") & "',"
            SQLq &= "'" & DateToString(Now) & "',"
            SQLq &= "'" & Session("logon") & "',"
            SQLq &= "'" & DateToString(Now) & "')"

            ret = ExequteSQLquery(SQLq)

            If ret <> "Query executed fine." Then
                DeleteReport(repid)
                Return ret = "ERROR!! " & ret
            End If

            SQLq = "INSERT INTO [OURPermissions] "
            SQLq &= "([NetId],"
            SQLq &= "[Application],"
            SQLq &= "[Param1],"
            SQLq &= "[Param2],"
            SQLq &= "[Param3],"
            SQLq &= "[AccessLevel],"
            SQLq &= "[OpenFrom],"
            SQLq &= "[OpenTo],"
            SQLq &= "[Comments])"
            SQLq &= "VALUES"
            SQLq &= "('" & Session("logon") & "',"
            SQLq &= "'InteractiveReporting',"
            SQLq &= "'" & repid & "',"
            SQLq &= "'" & repttl & "',"
            SQLq &= "'" & Session("email") & "',"
            SQLq &= "'admin',"
            SQLq &= "'" & DateToString(Now) & "',"
            SQLq &= "'" & Session("UserEndDate") & "',"
            SQLq &= "'" & Session("logon") & "')"

            ret = ExequteSQLquery(SQLq)

            If ret <> "Query executed fine." Then
                Return ret = "ERROR!! " & ret
            End If
        End If
        reportid = repid

        Return ret
    End Function

    Private Sub btnOpenReport_Click(sender As Object, e As EventArgs) Handles btnOpenReport.Click
        'Response.Redirect("ShowReport.aspx?srd=0")
    End Sub

    Private Sub lnkDataAI_Click(sender As Object, e As EventArgs) Handles lnkDataAI.Click
        Response.Redirect("DataAI.aspx?pg=expl&srd=" & Request("srd"), True)

    End Sub

End Class


