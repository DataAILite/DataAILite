Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml
Imports System.Net
Imports System.Net.Http
Imports Microsoft.Reporting.WebForms
Partial Class ReportViews
    Inherits System.Web.UI.Page

    Public MyconnStr As String = ConfigurationManager.ConnectionStrings.Item("MySQLConnection").ToString
    Public Myconn As SqlConnection
    Public cmdReport As SqlCommand
    Public dt As DataTable
    Public dr As DataRow
    Public dv1, dv2, dv3, dv4 As DataView
    Public da As SqlDataAdapter
    Public sp, repid, repname, reptitle, reporttable, WhereText, mSql, ddrSql, reppath, pdfpath, temppath As String
    Public ParamNames() As String
    Public ParamTypes() As String
    Public ParamValues() As String
    Public ParamFields() As String
    Public dirsort As String
    Public expand1, expand2 As Boolean
    Public pagewidth As String
    Public pageheight As String

    Private Sub ReportViews_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("OurConnProvider").ToString.Trim = "Sqlite" Then
            HyperLinkSchedule.Visible = False
            HyperLinkSchedule.Enabled = False
            LabelShare.Visible = False
            txtShareEmail.Visible = False
            txtShareEmail.Enabled = False
            btnShare.Visible = False
            btnShare.Enabled = False
            LabelTicket.Visible = False
            LabelTicket.Enabled = False
            DropDownTickets.Visible = False
            DropDownTickets.Enabled = False
            ButtonTicket.Visible = False
            ButtonTicket.Enabled = False
        End If
        Session("arr") = ""
        Session("nrec") = ""
        Session("ttl") = ""
        Session("x1") = ""
        Session("x2") = ""
        Session("y1") = ""
        Session("srt") = ""
        Session("newarr") = "yes"
        Session("StatDash") = "no"
        Session("ParamsCount") = -1
        Session("MapChart") = ""
        Session("MatrixChart") = ""
        Session("AllUnSelected") = ""
        Session("AllSelected") = ""
        If Session("See") Is Nothing Then
            Session("See") = "yes"
        End If
        If Session("matrix") Is Nothing Then
            Session("matrix") = ""
        End If

        If Request("runschedreps") IsNot Nothing AndAlso Request("runschedreps").ToString.Trim = "yes" Then
            LabelRunSched.Text = "yes"
            Session("RunSched") = "yes"
            'Session("See") = "yes"
            If Request("srd") Is Nothing OrElse Request("srd").ToString.Trim = "" Then
                Session("srd") = 6
            Else
                Session("srd") = Request("srd").ToString.Trim
            End If
            If Request("det") IsNot Nothing AndAlso Request("det").ToString.Trim = "yes" Then
                Session("See") = "Detail"
            ElseIf Request("gen") IsNot Nothing AndAlso Request("gen").ToString.Trim = "yes" Then
                Session("See") = "Report"
            ElseIf Request("grpstats") IsNot Nothing AndAlso Request("grpstats").ToString.Trim = "yes" Then
                Session("See") = "GroupsStats"
            ElseIf Request("grtype") IsNot Nothing AndAlso Request("grtype").ToString.Trim = "matrix" Then
                Session("See") = "yes"
                Session("matrix") = "yes"
            ElseIf Request("grtype") IsNot Nothing AndAlso Request("grtype").ToString.Trim = "bar" Then
                Session("See") = "Graph"
                Session("GraphType") = "bar"
            ElseIf Request("grtype") IsNot Nothing AndAlso Request("grtype").ToString.Trim = "pie" Then
                Session("See") = "pie"
                Session("GraphType") = "pie"
            ElseIf Request("grtype") IsNot Nothing AndAlso Request("grtype").ToString.Trim = "line" Then
                Session("See") = "line"
                Session("GraphType") = "line"
            End If

            If Request("fn") IsNot Nothing AndAlso Request("fn").ToString.Trim <> "" Then
                Session("Aggregate") = Request("fn").ToString.Trim
            End If
            If Request("y1") IsNot Nothing AndAlso Request("y1").ToString.Trim <> "" Then
                Session("AxisY") = Request("y1").ToString.Trim
            End If

        Else
            LabelRunSched.Text = "no"

            If Request("srd") Is Nothing OrElse Request("srd").ToString.Trim = "" Then
                Session("srd") = 3
            Else
                Session("srd") = Request("srd").ToString.Trim
            End If
        End If
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=View_Report"
        'If Session("UserConnProvider") = "InterSystems.Data.IRISClient" Then
        '    HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=75"
        'ElseIf Session("UserConnProvider") = "InterSystems.Data.CacheClient" Then
        '    HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=75"
        'ElseIf Session("UserConnProvider") = "MySql.Data.MySqlClient" Then
        '    HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=43"
        'ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
        '    'TODO !!!!!!
        '    HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=100"
        'Else 'SQL Server
        '    HyperLinkHelp.NavigateUrl = "OnlineUserReporting.pdf#page=100"
        'End If
        If WhereText Is Nothing Then WhereText = ""
        If Session("WhereText") Is Nothing Then Session("WhereText") = ""
        If Session("WhereStm") Is Nothing Then Session("WhereStm") = ""
        If Not IsPostBack Then
            Session("SeeGraph") = "no"
            Session("SeeReport") = "no"
        End If
        LabelReportID.Text = Session("REPORTID")
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        Dim addwhere As String = String.Empty

        If Not IsPostBack AndAlso Not Request("cat1") Is Nothing AndAlso Not Request("cat2") Is Nothing Then
            DropDownList1.SelectedValue = Request("cat1").ToString
            DropDownList2.SelectedValue = Request("cat2").ToString
            Session("cat1") = Request("cat1").ToString
            Session("cat2") = Request("cat2").ToString

            If Request("fn") IsNot Nothing AndAlso Request("fn").ToString.Trim <> "" Then
                Session("Aggregate") = Request("fn").ToString.Trim
            End If
            If Request("y1") IsNot Nothing AndAlso Request("y1").ToString.Trim <> "" Then
                Session("AxisY") = Request("y1").ToString.Trim
            End If

            If Not Request("val1") Is Nothing AndAlso Request("val1").ToString.Trim <> "all" AndAlso Request("val1").ToString.Trim <> "" Then
                addwhere = Request("cat1").ToString & "='" & Request("val1").ToString.Trim & "' "
                expand1 = True
            End If
            If Not Request("val2") Is Nothing AndAlso Request("val2").ToString.Trim <> "all" AndAlso Request("val2").ToString.Trim <> "" Then
                If addwhere.Trim <> "" Then
                    addwhere = addwhere & " AND "
                    'expand2 = True
                    expand2 = False
                End If
                'addwhere = addwhere & Request("cat2").ToString & "='" & Request("val2").ToString.Trim & "' "
                addwhere = addwhere & Request("cat2").ToString & "='" & Request("val2").ToString & "' "
            End If
            'txtAddWhere.Text = addwhere
            Session("addwhere") = addwhere
            LabelAddWhere.Text = Session("addwhere").ToString
        ElseIf Not IsPostBack AndAlso Request("cat1") Is Nothing AndAlso Request("cat2") Is Nothing Then
            If Not Session("cat1") Is Nothing AndAlso Not Session("cat2") Is Nothing Then
                DropDownList1.SelectedValue = Session("cat1").ToString
                DropDownList2.SelectedValue = Session("cat2").ToString
            End If
        End If
        'TODO put groups in graph dropdown
        If Not IsPostBack AndAlso DropDownList1.SelectedValue.ToString.Trim = "" AndAlso DropDownList2.SelectedValue.ToString.Trim = "" Then
            Dim dtgrp As New DataTable
            dtgrp = GetReportGroups(LabelReportID.Text) 'group table
            Dim gr1 As String = String.Empty
            Dim gr2 As String = String.Empty
            Dim fl1 As String = String.Empty
            For i = 0 To dtgrp.Rows.Count - 1
                If dtgrp.Rows(i)("GroupField") <> "Overall" Then
                    If gr1 = "" Then
                        gr1 = dtgrp.Rows(i)("GroupField")
                        fl1 = dtgrp.Rows(i)("CalcField")
                        If (Session("cat1") Is Nothing OrElse Session("cat1").ToString.Trim = "") Then
                            Session("cat1") = gr1
                        End If
                        If (Session("AxisY") Is Nothing OrElse Session("AxisY").ToString.Trim = "") Then
                            Session("AxisY") = fl1
                        End If
                    Else
                        If gr2 = "" AndAlso dtgrp.Rows(i)("GroupField") <> gr1 Then
                            gr2 = dtgrp.Rows(i)("GroupField")
                            If (Session("cat2") Is Nothing OrElse Session("cat2").ToString.Trim = "") Then
                                Session("cat2") = gr2
                            End If
                            Exit For
                        End If

                    End If
                End If
            Next
            DropDownList1.SelectedValue = gr1
            DropDownList2.SelectedValue = gr2
            DropDownList3.SelectedValue = fl1
        End If
        'If addwhere <> "" Then
        '    maintable.Rows(1).Cells(0).InnerHtml = Session("addwhere").ToString
        'Else
        '    maintable.Rows(1).Cells(0).InnerHtml = ""
        'End If
        If Not IsPostBack Then
            Dim li As ListItem = New ListItem
            li.Text = "new"
            li.Value = 0
            DropDownTickets.Items.Add(li)
            'GetListOfUsertickets
            Dim fltr As String = "AND Name='" & Session("logon") & "' "
            If Session("admin") = "super" OrElse Session("Access") = "SITEADMIN" Then
                fltr = ""
            End If
            Dim sqlh As String = "SELECT * FROM OURHelpDesk WHERE ( [Status] Not In ('deleted','dismissed','how to','knowledge','documentation') " & fltr & ") ORDER BY ID DESC"
            Dim dvh As DataView = mRecords(sqlh)
            If Not dvh Is Nothing AndAlso Not dvh.Table Is Nothing AndAlso dvh.Table.Rows.Count > 0 Then
                For i = 0 To dvh.Table.Rows.Count - 1
                    li = New ListItem
                    li.Value = dvh.Table.Rows(i)("ID")
                    li.Text = dvh.Table.Rows(i)("ID") & " - " & dvh.Table.Rows(i)("Status").ToString & " - " & dvh.Table.Rows(i)("Ticket").ToString
                    DropDownTickets.Items.Add(li)
                Next
            End If

        End If
        If Session("admin") = "admin" OrElse Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LabelShare.Visible = True
            txtShareEmail.Visible = True
            txtShareEmail.Enabled = True
            btnShare.Visible = True
            btnShare.Enabled = True
            LabelTicket.Visible = True
            DropDownTickets.Visible = True
            DropDownTickets.Enabled = True
            ButtonTicket.Visible = True
            ButtonTicket.Enabled = True

        Else
            LabelShare.Visible = False
            txtShareEmail.Visible = False
            txtShareEmail.Enabled = False
            LabelTicket.Visible = False
            btnShare.Visible = False
            btnShare.Enabled = False
            DropDownTickets.Visible = False
            DropDownTickets.Enabled = False
            ButtonTicket.Visible = False
            ButtonTicket.Enabled = True
        End If

        lnkImage.Visible = True
        lnkImage.Enabled = True
        lnkExpandAll.Visible = False
        lnkExpandAll.Enabled = False

        lnkImage.OnClientClick = "getPageDimensions(); return false;"
        lnkPDF.OnClientClick = "getReportDimensions(""PDF""); return false;"
        lnkWord.OnClientClick = "getReportDimensions(""WORD""); return false;"
        ButtonLine.OnClientClick = "getReportDimensions(""LINE""); return false;"
        ButtonShowGraph.OnClientClick = "getReportDimensions(""BAR""); return false;"
        ButtonPie.OnClientClick = "getReportDimensions(""PIE""); return false;"
        ButtonMatrix.OnClientClick = "getReportDimensions(""MATRIX""); return false;"
        ButtonDynamicReport.OnClientClick = "getReportDimensions(""DRILLDOWN""); return false;"
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        lnkExpandAll.Visible = False
        lnkExpandAll.Enabled = False
        If Session("OurConnProvider").ToString.Trim = "Sqlite" Then
            HyperLinkSchedule.Visible = False
            HyperLinkSchedule.Enabled = False
            LabelShare.Visible = False
            txtShareEmail.Visible = False
            txtShareEmail.Enabled = False
            btnShare.Visible = False
            btnShare.Enabled = False
            LabelTicket.Visible = False
            LabelTicket.Enabled = False
            DropDownTickets.Visible = False
            DropDownTickets.Enabled = False
            ButtonTicket.Visible = False
            ButtonTicket.Enabled = False
        End If
        If Session("chrt") Is Nothing Then Session("chrt") = ""

        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")
        Dim app As String = String.Empty

        If target IsNot Nothing AndAlso data IsNot Nothing AndAlso target <> String.Empty AndAlso data <> String.Empty Then
            If target = "PageDimensions" Then
                pagewidth = Piece(data, "~", 1) & "in"
                pageheight = Piece(data, "~", 2) & "in"
                hdnReportDimensions.Value = pagewidth & "~" & pageheight
                lnkImage_Click(sender, EventArgs.Empty)
            ElseIf target = "ReportDimensions" Then
                pagewidth = Piece(data, "~", 1) & "in"
                pageheight = Piece(data, "~", 2) & "in"
                app = Piece(data, "~", 3)

                'hdnReportDimensions.Value = pagewidth & "~" & pageheight
                If Not IsPostBack OrElse Session("pagewidth") = "" Then
                    Session("pagewidth") = pagewidth
                    Session("pageheight") = pageheight
                End If
                If app <> String.Empty Then
                    Dim provider As String = Session("UserConnProvider").ToString
                    Select Case app.ToUpper
                        Case "WORD"
                            lnkWord_Click(sender, EventArgs.Empty)
                        Case "PDF"
                            lnkPDF_Click(sender, EventArgs.Empty)
                        Case "LINE"
                            'ButtonLine_Click(sender, EventArgs.Empty)
                            DoLine(provider)
                            Session("chrt") = "Line"
                        Case "BAR"
                            DoBar(provider)
                            'ButtonShowGraph_Click(sender, EventArgs.Empty)
                            Session("chrt") = "Bar"
                        Case "PIE"
                            DoPie(provider)
                            'ButtonPie_Click(sender, EventArgs.Empty)
                            Session("chrt") = "Pie"
                        Case "MATRIX"
                            DoMatrix(provider)
                            'ButtonMatrix_Click(sender, EventArgs.Empty)
                        Case "DRILLDOWN"
                            DoDrillDown(provider)
                            'ButtonDynamicReport_Click(sender, EventArgs.Empty)
                    End Select
                End If
            End If
        End If

        If Request("grtype") IsNot Nothing AndAlso Request("grtype").ToString = "matrix" AndAlso Session("See").ToString = "yes" Then
            lnkImage.Visible = False
            lnkImage.Enabled = False
        ElseIf Session("See") IsNot Nothing AndAlso Session("See").ToString = "Matrix" Then
            lnkImage.Visible = False
            lnkImage.Enabled = False
        ElseIf Session("See") IsNot Nothing AndAlso Session("See").ToString <> "Matrix" Then
            lnkImage.Visible = True
            lnkImage.Enabled = True
        Else
            lnkImage.Visible = True
            lnkImage.Enabled = True
        End If


        Dim i As Integer = 0
        HyperLinkRDL.Visible = False
        trParameters.Visible = True
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

        Dim ret As String = String.Empty
        repid = Session("REPORTID")
        If repid = "" Then
            Exit Sub
        End If
        Dim bReportExpired As Boolean = ReportExpired(repid, Session("logon"))
        Try
            'download rdl
            If Not Request("downrdl") Is Nothing AndAlso Request("downrdl").ToString = "yes" AndAlso Not Session("GraphRDL") Is Nothing Then

                If Not Session("DynamicRDL") Is Nothing AndAlso Session("DynamicRDL").ToString.Trim <> "" Then
                    'download RDL dynamic reports: drilldown, group statistics, generic
                    Dim graphstr As String = Session("DynamicRDL").ToString
                    Dim reptype As String = Session("DynamicType").ToString
                    Dim ErrorLog = String.Empty
                    repid = Session("REPORTID")
                    'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString & "RDLFILES\"
                    Dim repfile As String = repid & "_" & reptype & ".rdl"

                    'System.IO.File.WriteAllText (@"D:\path.txt", contents)
                    Try
                        ErrorLog = DownloadXML(graphstr, repfile)
                        If ErrorLog <> "" Then
                            LabelError.Text = ErrorLog
                            'Exit Sub
                        End If
                        Try
                            Response.ContentType = "application/octet-stream"
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" & repfile)
                            Response.TransmitFile(applpath & "RDLFILES\" & repfile)
                        Catch ex As Exception
                            ErrorLog = "ERROR!!  " & ex.Message
                        End Try
                        Response.End()
                    Catch ex As Exception
                        ErrorLog = "ERROR!! " & ex.Message
                    End Try
                    LabelError.Text = ErrorLog
                ElseIf Not Session("GraphRDL") Is Nothing AndAlso Session("GraphRDL").ToString.Trim <> "" Then
                    'download RDL reports: matrix, bar, line, pie 
                    Dim graphstr As String = Session("GraphRDL").ToString
                    Dim reptype As String = Session("GraphType").ToString
                    Dim ErrorLog = String.Empty
                    repid = Session("REPORTID")
                    'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString & "RDLFILES\"
                    Dim repfile As String = repid & "_" & reptype & ".rdl"

                    'System.IO.File.WriteAllText (@"D:\path.txt", contents)
                    Try
                        ErrorLog = DownloadXML(graphstr, repfile)
                        If ErrorLog <> "" Then
                            LabelError.Text = ErrorLog
                            'Exit Sub
                        End If
                        Try
                            Response.ContentType = "application/octet-stream"
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" & repfile)
                            Response.TransmitFile(applpath & "RDLFILES\" & repfile)
                        Catch ex As Exception
                            ErrorLog = "ERROR!!  " & ex.Message
                        End Try
                        Response.End()
                    Catch ex As Exception
                        ErrorLog = "ERROR!! " & ex.Message
                    End Try
                    LabelError.Text = ErrorLog
                Else
                    'download usual report files (RDL and XSD)
                    Dim ErrorLog = String.Empty
                    repid = Session("REPORTID")
                    'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                    Dim repfile As String = String.Empty
                    Dim downrepzip As String = "y" & repid & "Dnld.zip"
                    Dim dirpath As String = applpath & "TEMP\" & repid & "\"
                    Directory.CreateDirectory(dirpath)
                    repfile = CreateZipFile(repid)
                    If repfile = dirpath & downrepzip Then
                        Try
                            Response.ContentType = "application/octet-stream"
                            Response.AppendHeader("Content-Disposition", "attachment; filename=" & downrepzip)
                            Response.TransmitFile(dirpath & downrepzip)
                        Catch ex As Exception
                            ErrorLog = "ERROR!! " & ex.Message
                        End Try
                        Response.End()
                    End If
                    LabelError.Text = ErrorLog
                End If
            End If

            'Report Info (title, data for report)
            dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
            If dv1.Table.Rows(0)("ReportType").ToString = "" Then
                dv1.Table.Rows(0)("ReportType") = "rdl"
            End If

            Session("Attributes") = dv1.Table.Rows(0)("ReportAttributes").ToString
            Session("ParamsRelated") = dv1.Table.Rows(0)("Param0id").ToString
            ddrSql = dv1.Table.Rows(0)("SQLquerytext")
            LabelError.Text = ""
            Session("noedit") = dv1.Table.Rows(0)("Param7type").ToString
            If Session("noedit") = "standard" OrElse bReportExpired Then
                'hide all links to report editing pages leaving only reporting
                Dim treenodecr1 As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=2")
                If Not treenodecr1 Is Nothing Then
                    treenodecr1.Value = "ReportViews.aspx"
                    treenodecr1.ToolTip = "disabled for locked reports"
                    LabelError.Text = "Locked report cannot be edited."
                    LabelError.Visible = True
                End If
                Dim treenodecr2 As WebControls.TreeNode = TreeView1.FindNode("SQLquery.aspx?tnq=0")
                If Not treenodecr2 Is Nothing Then
                    treenodecr2.Value = "ReportViews.aspx"
                    treenodecr2.ToolTip = "disabled for locked reports"
                    LabelError.Text = "Locked report cannot be edited."
                    LabelError.Visible = True
                End If
                Dim treenodecr3 As WebControls.TreeNode = TreeView1.FindNode("RDLformat.aspx?tnf=0")
                If Not treenodecr3 Is Nothing Then
                    treenodecr3.Value = "ReportViews.aspx"
                    treenodecr3.ToolTip = "disabled for locked reports"
                    LabelError.Text = "Locked report cannot be edited."
                    LabelError.Visible = True
                End If
            End If
            i = 0
            Dim dt As DataTable '= dv3.Table
            Dim fldname As String = String.Empty
            Dim frdname As String = String.Empty
            Dim fldtype As String = String.Empty
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
                Session("REPTITLE") = reptitle
                Session("REPORTID") = repid
                LabelReportTitle.Text = reptitle
                Session("PageFtr") = dv1.Table.Rows(0)("Comments").ToString
            Else
                Exit Sub
                'Response.Redirect("Nodata.aspx")
            End If
            Dim n As Integer
            'parameters
            Dim ddrequest As String = String.Empty
            'maintable.Rows(1).Cells(0).InnerHtml = "Parameters: "  '" <asp:Button ID=""ButtonShowReport"" runat=""server"" Text=""Show Report"" />&nbsp;&nbsp;&nbsp;" '<asp:Label ID=""LabelParam"" runat=""server"" Font-Bold=""True"" Font-Names=""Arial"" Font-Size=""Small"" Text=""Parameters for data: "" ForeColor=""Gray""></asp:Label>"
            'Report Show (drop-downs, data for drop-downs)
            Dim ddid, ddname, ddsql, dddsql, ddfield, ddvalue, ddlabel, ddtype As String
            Dim drvalue, options As String
            'LabelError.Text = ""
            'dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
            dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY PrmOrder")
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

            Dim er As String = String.Empty
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
                ddsql = dv2.Table.Rows(i)("DropDownSQL").ToString  '.Replace("""", "'")
                dddsql = ddsql
                ddrSql = dv1.Table.Rows(0)("SQLquerytext")

                'check if drop-down is selected
                'ddrequest = Request("Select" & ddid)  'old
                ddrequest = Request(ddid)

                If Not IsPostBack AndAlso ddrequest = "" AndAlso Not Session("ParamValues") Is Nothing AndAlso Session("ParamsCount") > i AndAlso Session("ParamValues")(i).ToString.Trim <> "" AndAlso Session("ParamValues")(i).ToString.ToUpper <> "ALL" Then
                    ddrequest = Session("ParamValues")(i).ToString.Replace("'", "")
                End If

                ParamNames(i) = ddname
                ParamTypes(i) = ddtype
                ParamFields(i) = ddfield
                If ddrequest <> "" Then
                    ParamValues(i) = ddrequest.ToString.Replace("'", "")
                Else

                    If Session("Attributes") <> "sp" Then
                        ParamValues(i) = "ALL"
                    Else
                        ParamValues(i) = ""
                    End If
                End If


                'retrieve selected drop-down data
                If Not ddrequest Is Nothing AndAlso ddrequest <> "" AndAlso ddrequest <> "ALL" AndAlso ddrequest <> "All" Then 'And UCase(dv1.Table.Rows(0)("ReportAttributes").ToString) = "SQL" Then

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
                        WhereText = WhereText & "UCASE(" & ddrequest & ")"
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
                maintable.Rows(1).Cells(0).Controls.Add(ctllbl)
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
                maintable.Rows(1).Cells(0).Controls.Add(ctldd)

            Next   ' go to next drop-down draw 

            Session("ParamNames") = ParamNames
            Session("ParamValues") = ParamValues
            Session("ParamTypes") = ParamTypes
            Session("ParamFields") = ParamFields
            Session("ParamsCount") = ParamNames.Length
            j = 0
            'draw submit button
            If n > 0 Then
                '    maintable.Rows(1).Cells(0).InnerHtml = maintable.Rows(1).Cells(0).InnerHtml & "<input type=submit name='SubmitParams' id='SubmitParams' value='Apply' AutoPostBack='true'>&nbsp;&nbsp;"
                Dim ctlsbm As New Button
                ctlsbm.Text = "Apply"
                ctlsbm.ID = "SubmitParams"
                ctlsbm.ToolTip = "Apply Parameters"
                ctlsbm.UseSubmitBehavior = True
                maintable.Rows(1).Cells(0).Controls.Add(ctlsbm)
            End If

            '----------------------------------------------------------------------------------------------
            'scheduled report with WhereText
            If Not IsPostBack AndAlso Session("sched") = "yes" AndAlso Session("srd") = 6 AndAlso Session("WhereText").ToString.Trim <> "" Then
                WhereText = WhereText & Session("WhereText").ToString
            End If

            '----------------------------------------------------------------------------------------------
            'add additional where statement from cat1 and cat2
            If Session("srd") = 11 Then
                If Not Session("addwhere") Is Nothing AndAlso Session("addwhere").ToString.Trim <> "" Then
                    'If Trim(WhereText) <> "" Then WhereText = WhereText & " AND "
                    'WhereText = WhereText & Session("addwhere").ToString
                    WhereText = Session("addwhere").ToString
                    LabelAddWhere.Text = Session("addwhere").ToString
                End If
                Session("dv3") = Nothing
            End If

            dv3 = Session("dv3")
            '----------------------------------------------------------------------------------------------
            Dim doReport As Boolean = False

            'retrieve data for report
            If Session("dv3") Is Nothing OrElse Session("dv3").Count = 0 OrElse Session("dv3").Table.Rows.Count = 0 Then
                'dv3 = RetrieveDataForReport()
                dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret)
            ElseIf IsPostBack AndAlso Session("WhereText").Trim <> WhereText.Trim Then
                'dv3 = RetrieveDataForReport()
                doReport = True
                dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret)
            ElseIf IsPostBack AndAlso Session("WhereText").Trim = WhereText.Trim Then
                dv3 = Session("dv3")
            ElseIf Not IsPostBack Then

            End If
            'TODO recalculate Session("DataToChatAI") depending of report type: Request("grtype") = "matrix" or Request("grpstats") = "yes" or else... in corresponding functions
            If dv3 Is Nothing OrElse dv3.Table Is Nothing Then
                MessageBox.Show("ERROR!! No data", "Error!", "No data", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            End If
            If Request("grtype") IsNot Nothing AndAlso Not Request("grtype") = "matrix" AndAlso Session("See") <> "Matrix" AndAlso Session("matrix") <> "yes" Then
                Session("DataToChatAI") = ExportToCSVtext(dv3.Table, Chr(9))
                Session("OriginalDataTable") = dv3.Table
                Session("datatable") = dv3.Table
            End If
            Dim dtf As New DataTable
            dtf = dv3.Table
            dtf = MakeDTColumnsNamesCLScompliant(dtf, Session("UserConnProvider"), ret)

            dv3 = dtf.DefaultView
            '----------------------------------------------------------------------------------------------

            Session("WhereText") = WhereText

            If dv3.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
            Session("dv3") = dv3

            If dv3 Is Nothing Then 'OrElse dv3.Count = 0
                LabelError.Text = "No data"
                LabelRowCount.Text = "Records returned: " & 0.ToString
                'Response.Redirect("ReportEdit.aspx?msg=nodata")
                LinkButtonDownRDL.Visible = False
                LinkButtonDownRDL.Enabled = False
                viewer.Visible = False
                'Exit Sub
                'ElseIf IsPostBack AndAlso Session("WhereText").Trim <> WhereText.Trim Then
                '    viewer.Visible = False
                '    viewer.Enabled = False
            End If
            If Not IsPostBack AndAlso Not dv3 Is Nothing AndAlso Not dv3.Table Is Nothing Then
                'dropdowns
                DropDownColumns.Items.Clear()

                dt = dv3.Table
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


            'Graph dropdowns
            If Not IsPostBack Then
                DropDownList1.Items.Clear()
                DropDownList2.Items.Clear()
                DropDownList3.Items.Clear()
                DropDownList1.Items.Add(" ")
                DropDownList2.Items.Add(" ")
                DropDownList3.Items.Add(" ")

                If Session("Aggregate") Is Nothing OrElse Session("Aggregate").ToString.Trim = "" Then
                    DropDownList4.Items.Clear()
                    DropDownList4.Items.Add("Count")
                    DropDownList4.Items.Add("CountDistinct")
                    Session("Aggregate") = "Count"
                End If
                Session("nv") = DropDownList4.Items.Count
                dt = dv3.Table
                fldname = String.Empty
                frdname = String.Empty
                fldtype = String.Empty
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
                    If Session("srd") = 11 AndAlso Not Request("cat1") Is Nothing AndAlso li.Value = Request("cat1").ToString Then
                        DropDownList1.Items(i).Selected = True
                    End If
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
                    DropDownList2.Items.Add(li)
                    If Session("srd") = 11 AndAlso Not Request("cat1") Is Nothing AndAlso Not Request("cat2") Is Nothing AndAlso li.Value = Request("cat2").ToString Then
                        DropDownList2.Items(i).Selected = True
                    End If
                Next
            End If
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
            'for ChartGoogle
            If srch.Trim <> "" Then
                If Session("WhereText").ToString.Trim <> "" Then
                    Session("WhereStm") = srch.Trim & " AND " & Session("WhereText").ToString.Trim
                Else
                    Session("WhereStm") = srch.Trim
                End If
            Else
                'Session("WhereStm") = Session("WhereText").ToString.Trim
                'srch = Session("WhereStm")
            End If
            Session("srchstm") = srch
            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'Filter dv3
            'dv3.RowFilter = Session("WhereStm")
            dv3.RowFilter = srch
            Dim dtt As New DataTable
            dtt = dv3.ToTable
            If Not Request("grtype") = "matrix" AndAlso Session("See") <> "Matrix" AndAlso Session("matrix") <> "yes" Then
                Session("OriginalDataTable") = dtt
                Session("DataToChatAI") = ExportToCSVtext(dtt, Chr(9))
                Session("datatable") = dtt
            End If

            If srch.Trim <> "" Then
                If dtt.Rows Is Nothing Then
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                    'Exit Sub
                End If
                If dtt.Rows.Count > 0 Then
                    LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
                Else
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                End If
            Else
                If dv3 Is Nothing Then
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                    'Exit Sub
                Else
                    LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString  ' dv3.Table.Rows.Count.ToString
                End If
            End If

            If Not IsPostBack Then dv3.RowFilter = ""
            Session("dv3") = dv3

            If LabelAddWhere.Text.Trim = "" Then LabelAddWhere.Text = "<=>"
            LabelAddWhere.ToolTip = "_" & Session("WhereText") & "_" & Session("WhereStm") & "_" & Session("addwhere") & "_" & Session("filter") & "_" & Session("srchstm") & "_"

            Session("mySQL") = GetReportInfo(repid).Rows(0)("SQLquerytext").ToString

            If Session("admin") = "super" Then
                LabelSQL.Text = "Data query: " & Session("mySQL")
                LabelSQL.Visible = True
                LinkButtonDownRDL.Visible = True
                LinkButtonDownRDL.Enabled = True
            ElseIf IsPostBack AndAlso Session("AdvancedUser") = True Then
                LabelSQL.Text = "Data query: " & Session("mySQL")
                LabelSQL.Visible = True
                LinkButtonDownRDL.Visible = True
                LinkButtonDownRDL.Enabled = True
            Else
                LabelSQL.Visible = False
                LinkButtonDownRDL.Visible = False
                LinkButtonDownRDL.Enabled = False
            End If

            If Not IsPostBack AndAlso Not Session("cat1") Is Nothing AndAlso Not Session("cat2") Is Nothing AndAlso Session("cat1") <> "" AndAlso Session("cat2") <> "" AndAlso Session("cat1") <> "all" AndAlso Session("cat2") <> "all" Then
                DropDownList1.SelectedValue = Session("cat1").ToString
                DropDownList2.SelectedValue = Session("cat2").ToString
            End If
            'If Not IsPostBack AndAlso Not Session("cat1") Is Nothing AndAlso Not Session("AxisY") Is Nothing AndAlso Not Session("Aggregate") Is Nothing Then
            If Not IsPostBack AndAlso Not Session("AxisY") Is Nothing AndAlso Not Session("Aggregate") Is Nothing Then
                DropDownList3.SelectedValue = Session("AxisY").ToString
                DropDownList4.SelectedValue = Session("Aggregate").ToString
            End If

            If Not IsPostBack AndAlso Not Request("val1") Is Nothing AndAlso Not Request("val2") Is Nothing AndAlso Request("val1").ToString.Trim = "" AndAlso Request("val2").ToString.Trim = "" Then
                LabelError.Text = "No data."
                LabelError.Visible = True
                LinkButtonDownRDL.Visible = False
                LinkButtonDownRDL.Enabled = False
                Exit Sub
            End If
            Dim gr As String = String.Empty
            Session("See") = Request("see")
            If Session("See") Is Nothing Then
                Session("See") = "yes"
            End If

            'If Not IsPostBack AndAlso Session("See") = "yes" Then
            '    lblReportFunction.Text = "Show Report:"
            '    If ReportCreatedByDesigner(repid) Then
            '        lblDesignerCreated.Text = "Created by Designer"
            '    Else
            '        lblDesignerCreated.Text = "Not Created by Designer"
            '    End If
            '    lblDesignerCreated.Visible = True
            '    Session("DynamicType") = ""
            '    gr = SeeReport()
            '    Session("See") = "Report"
            '    Session("GraphType") = ""
            'Else
            If Not IsPostBack AndAlso Request("gen") = "yes" Then
                lblReportFunction.Text = "Generic Report:"
                lblDesignerCreated.Visible = False
                Session("DynamicType") = ""
                gr = SeeReport()
                Session("See") = "Report"
                Session("GraphType") = ""
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=View_Report"
            ElseIf Not IsPostBack AndAlso Request("det") = "yes" Then
                lblReportFunction.Text = "Detail Report:"
                lblDesignerCreated.Visible = False
                If DropDownList1.Text = DropDownList2.Text Then
                    'MessageBox.Show("Primary and Secondary Categories shoud be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    'Exit Sub
                    dv3 = Session("dv3")
                    Dim GroupTable As DataTable = Nothing
                    'make GroupTable
                    GroupTable = GetReportGroups("0")
                    Dim row As DataRow = GroupTable.NewRow
                    row("ReportId") = Session("REPORTID")
                    row("GroupField") = DropDownList1.Text
                    row("CalcField") = DropDownList3.Text
                    row("CntChk") = 1
                    row("CntDistChk") = 1
                    If ColumnTypeIsNumeric(dv3.Table.Columns(DropDownList3.Text)) Then
                        row("SumChk") = 1
                        row("MaxChk") = 1
                        row("MinChk") = 1
                        row("AvgChk") = 1
                        row("StDevChk") = 0
                    Else
                        row("SumChk") = 0
                        row("MaxChk") = 0
                        row("MinChk") = 0
                        row("AvgChk") = 0
                        row("StDevChk") = 0
                    End If
                    row("FirstChk") = 0
                    row("LastChk") = 0
                    row("PageBrk") = 0
                    row("Indx") = 1
                    row("GrpOrder") = 1
                    GroupTable.Rows.Add(row)
                    gr = SeeDynamicReportForGroups(GroupTable)
                Else
                    lnkExpandAll.Visible = False
                    lnkExpandAll.Enabled = False
                    gr = SeeDynamicReport(DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue)

                End If
                Session("See") = "Details"
                Session("GraphType") = ""
            ElseIf Not IsPostBack AndAlso Request("grpstats") = "yes" Then
                lblReportFunction.Text = "Group Statistics:"
                lblDesignerCreated.Visible = False
                gr = SeeDynamicReportForGroups()
                Session("See") = "GroupsStats"
                Session("GraphType") = ""
                LabelError.Text = gr
            ElseIf Not IsPostBack AndAlso Request("graph") = "yes" Then
                lblReportFunction.Text = "Report Graphs:"
                lblDesignerCreated.Visible = False

                Session("DynamicType") = ""
                'gr = SeeGraph("")
                Session("See") = "Graph"
                LinkButtonDownRDL.Visible = False
                LinkButtonDownRDL.Enabled = False
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Show_Report_Graphs"
                If Session("srd") = 11 OrElse Session("RunSched") = "yes" Then  'from analytics or from run scheduled reports
                    If Not IsPostBack AndAlso Request("grtype") = "bar" Then
                        gr = SeeGraph("bar")  'bar
                    ElseIf Not IsPostBack AndAlso Request("grtype") = "matrix" Then
                        gr = SeeMatrix()
                    ElseIf Not IsPostBack AndAlso Request("grtype") = "pie" Then
                        gr = SeeGraph("pie")
                    ElseIf Not IsPostBack AndAlso Request("grtype") = "line" Then
                        gr = SeeGraph("line")
                    End If
                End If
                If Session("srd") = 12 Then 'from correlation
                    DropDownList1.SelectedValue = Session("cat2").ToString
                    DropDownList3.SelectedValue = Session("cat1").ToString
                    DropDownList3_SelectedIndexChanged(sender, e)
                    DropDownList4.SelectedValue = "Avg"   'correlation  ???????????????????
                    If Not IsPostBack AndAlso Request("grtype") = "bar" Then
                        gr = SeeGraph("bar")  'bar

                    End If
                End If
            ElseIf Not IsPostBack AndAlso Session("See") = "yes" Then
                lblReportFunction.Text = "Show Report:"
                If ReportCreatedByDesigner(repid) Then
                    lblDesignerCreated.Text = "Created by Designer"
                Else
                    lblDesignerCreated.Text = "Not Created by Designer"
                End If
                lblDesignerCreated.Visible = True
                Session("DynamicType") = ""
                gr = SeeReport()
                Session("See") = "Report"
                Session("GraphType") = ""
            End If
            If Request("SubmitParams") = "Apply" OrElse Request("ButtonSearch") = "Search" OrElse doReport Then
                If IsPostBack AndAlso (Session("See") = "Report" Or Session("See") = "yes") Then
                    'Session("addwhere") = ""
                    'LabelAddWhere.Text = "<=>"
                    If LabelAddWhere.Text.Trim = "" Then LabelAddWhere.Text = "<=>"
                    gr = SeeReport()

                ElseIf IsPostBack AndAlso Session("See") = "Graph" Then
                    gr = SeeGraph("bar")  'bar

                ElseIf IsPostBack AndAlso Session("See") = "Matrix" Then
                    gr = SeeMatrix()

                ElseIf IsPostBack AndAlso Session("See") = "Pie" Then
                    gr = SeeGraph("pie")

                ElseIf IsPostBack AndAlso Session("See") = "Line" Then
                    gr = SeeGraph("line")
                ElseIf IsPostBack AndAlso Session("See") = "GroupsStats" Then
                    gr = SeeDynamicReportForGroups()
                ElseIf IsPostBack AndAlso Session("See") = "Details" Then

                    gr = SeeDynamicReport(DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue)
                ElseIf IsPostBack AndAlso Session("See") Is Nothing Then
                    gr = SeeDynamicReport(DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue)

                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            If Not ret.StartsWith("Thread ") Then
                MessageBox.Show("ERROR!! " & ret, "Error!", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                'Response.Redirect("ShowReport.aspx?srd=0")
            End If
        End Try
    End Sub

    Protected Function SeeReport() As String
        lnkWord.Visible = True
        lnkWord.Enabled = True
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LinkButtonDownRDL.Visible = True
            LinkButtonDownRDL.Enabled = True
        End If
        Session("GraphRDL") = ""
        Session("matrix") = ""
        Dim err, ret As String
        err = ""
        ret = ""
        'report is not saved, RDL and XSD are not created, make RDL and XSD 
        Dim dv5 As DataView = mRecords("SELECT FileText FROM OURFiles WHERE (ReportID='" & repid & "') AND (Type='XSD')")
        If dv5 Is Nothing OrElse dv5.Table Is Nothing OrElse dv5.Table.Rows.Count = 0 OrElse dv5.Table.Rows(0)("FileText").ToString.Trim = "" Then
            ret = ret & UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
            ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        End If

        'Dim viewer As New ReportViewer
        viewer.Visible = True
        viewer.Enabled = True
        'viewer.Width = 1200
        'viewer.Height = 800
        'viewer.ShowPageNavigationControls = True
        'viewer.ShowExportControls = True
        'viewer.WaitControlDisplayAfter = 300
        ScriptManager1.AsyncPostBackTimeout = "999999"
        Try
            'ToTable hides all duplicates !!!!!!!!!!!!!!!!!!!!!!!!!!
            Dim dt As DataTable = CType(Session("dv3"), DataView).ToTable
            dt = CorrectDatasetColumns(dt)
            Dim repid As String = Session("REPORTID")

            dt = ProcessLists(repid, dt, err)

            'Show RDL report
            Dim rdlfile = String.Empty
            Dim datestr, timestr As String
            datestr = DateString()
            timestr = TimeString()

            viewer.ZoomMode = Microsoft.Reporting.WebForms.ZoomMode.FullPage
            viewer.PageCountMode = Microsoft.Reporting.WebForms.PageCountMode.Actual
            viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local

            Dim er As String = String.Empty
            Dim userfile As String = String.Empty
            'Dim rdlstream As Stream
            Dim rdlstr As String = String.Empty
            Dim myprovider As String = System.Configuration.ConfigurationManager.ConnectionStrings.Item("mySQLconnection").ProviderName.ToString

            Dim dv As DataView = mRecords("SELECT * FROM OURFiles WHERE ReportId='" & repid & "' AND Type='RDL'")
            If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
                If Request("gen") = "yes" AndAlso ReportCreatedByDesigner(repid) Then
                    'Create generic RDL report without advanced designer formatting
                    Dim dri As DataTable = GetReportInfo(repid)
                    If dri Is Nothing OrElse dri.Rows.Count = 0 Then
                        Return "Report is not found"
                        Exit Function
                    End If
                    Dim sqlquerytext As String = dri.Rows(0)("SQLquerytext")
                    'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                    Dim rdlpath As String = applpath & "RDLFILES\"
                    'Dim repfile As String = rdlpath & repid & ".rdl"
                    'Dim repfilecopy As String = rdlpath & repid & "Copy.rdl"

                    Session("DynamicType") = "Generic"

                    Dim pageftr As String = dri.Rows(0)("Comments").ToString
                    Dim orien As String = dri.Rows(0)("Param9type").ToString
                    Try

                        Dim rdlstrgen As String = String.Empty
                        rdlfile = CreateRDLReportForXSD(Session("REPORTID"), Session("REPTITLE"), rdlpath, sqlquerytext, "sql", orien, pageftr, rdlstrgen)

                        Session("DynamicRDL") = rdlstrgen
                        Dim textread As New StringReader(rdlstrgen)
                        viewer.LocalReport.LoadReportDefinition(textread)

                        ''in this case the file in RDLFILES folder will be updated with generic report
                        ''viewer.LocalReport.ReportPath = rdlfile
                        'Dim sr As StreamReader = New StreamReader(rdlfile)
                        '    rdlstr = sr.ReadToEnd()
                        '    Dim textread As New StringReader(rdlstr)
                        '    Session("DynamicRDL") = textread
                        '    viewer.LocalReport.LoadReportDefinition(textread)
                    Catch ex As Exception
                        Response.Redirect("ShowReport.aspx?srd=0")
                    End Try
                Else
                    'read rdl from OURFiles
                    rdlstr = dv.Table.Rows(0)("FileText").ToString  'string with file content
                    userfile = dv.Table.Rows(0)("Prop1").ToString   'uploaded by
                    If rdlstr <> "" Then
                        'Dim byteArray As Byte() = Encoding.UTF8.GetBytes(rdlstr)
                        'rdlstream = New MemoryStream(byteArray)
                        'rdlstream.Position = 0
                        'viewer.LocalReport.LoadReportDefinition(rdlstream)
                        Dim textread As New StringReader(rdlstr)
                        viewer.LocalReport.LoadReportDefinition(textread)
                        er = ""
                    Else
                        If dv.Table.Rows(0)("Path").ToString <> "" Then
                            rdlfile = dv.Table.Rows(0)("Path").ToString
                            rdlfile = rdlfile.Replace("|", "\")
                            viewer.LocalReport.ReportPath = rdlfile
                            'If myprovider <> "Oracle.ManagedDataAccess.Client" Then
                            'insert rdl into OURFiles 
                            Dim sr As StreamReader = New StreamReader(rdlfile)
                            rdlstr = sr.ReadToEnd()
                            er = SaveRDLstringInOURFiles(repid, rdlstr)  'file path from the the OURFiles record
                            'End If
                        End If

                        'End If
                    End If
                End If
            Else
                'get rdl from RDLFILES directory
                If rdlfile = "" Then
                    'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                    Dim rdlpath As String = applpath & "RDLFILES\"
                    rdlfile = rdlpath & repid & ".rdl"
                    'insert rdl into OURFiles 
                    Dim sr As StreamReader = New StreamReader(rdlfile)
                    rdlstr = sr.ReadToEnd()
                    er = SaveRDLstringInOURFiles(repid, rdlstr) 'file from RDLFILES
                End If
                viewer.LocalReport.ReportPath = rdlfile
            End If

            'Dim localReport As LocalReport = viewer.LocalReport
            'localReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalSubreportProcessingEventHandler)
            viewer.LocalReport.DataSources.Clear()
            Dim rds As Microsoft.Reporting.WebForms.ReportDataSource = New Microsoft.Reporting.WebForms.ReportDataSource
            rds.Name = Session("REPORTID")
            rds.Value = dt
            viewer.LocalReport.DataSources.Add(rds)

            If userfile.Trim = "" AndAlso rdlfile.Trim <> "" AndAlso er = "Query executed fine." Then
                If myprovider <> "Oracle.ManagedDataAccess.Client" Then
                    'delete it from the RDLFILES folder
                    If File.Exists(rdlfile) Then
                        File.Delete(rdlfile)
                    End If
                End If
            End If
            If Not IsPostBack AndAlso Not Session("srd") Is Nothing AndAlso (Session("srd") = 4 Or Session("srd") = 5 Or Session("srd") = 6 Or Session("srd") = 18) Then

                'Render the report  with viewer dimentions
                pagewidth = Session("pagewidth")
                pageheight = Session("pageheight")
                If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
                If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
                Dim mimeType As String = ""
                Dim encoding As String = ""
                Dim fileNameExtension As String = ""
                Dim streams As String() = Nothing
                Dim warnings As Warning() = Nothing
                Dim deviceInf As String = "" ' "<DeviceInfo>" & "  <OutputFormat>PNG</OutputFormat>"
                deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
                deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
                deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
                deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
                deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
                deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
                deviceInf = deviceInf & "</DeviceInfo>"

                Dim appldirExcelFiles, myfile As String
                'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                Dim datest, timest As String
                datest = DateString()
                timest = TimeString()
                myfile = Session("REPORTID") & "Rep" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)
                appldirExcelFiles = applpath & "Temp\"
                Session("appldirExcelFiles") = appldirExcelFiles
                Session("myfile") = myfile
                Dim Bytes() As Byte = Nothing
                If Session("srd") = 4 Then
                    myfile = myfile & ".xls"
                    Bytes = viewer.LocalReport.Render("Excel")
                ElseIf Session("srd") = 5 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>WORD</OutputFormat>" & deviceInf
                    myfile = myfile & ".doc"
                    Bytes = viewer.LocalReport.Render("Word", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)
                ElseIf Session("srd") = 6 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>PDF</OutputFormat>" & deviceInf
                    myfile = myfile & ".pdf"
                    Bytes = viewer.LocalReport.Render("PDF", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

                End If
                If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                    Session("myfile") = myfile
                    Dim filepath As String = String.Empty
                    'scheduled report
                    If LabelRunSched.Text = "yes" AndAlso Request("schedid") IsNot Nothing AndAlso IsNumeric(Request("schedid").ToString.Trim) Then
                        Dim fileuploadDir As String = applpath 'ConfigurationManager.AppSettings("fileupload").ToString
                        filepath = fileuploadDir & myfile
                        'filepath = filepath.Replace("\\", "\")
                        Using Stream As New FileStream(filepath, FileMode.Create)
                            Stream.Write(Bytes, 0, Bytes.Length)
                            Stream.Close()
                            Stream.Dispose()
                        End Using
                        Try
                            Dim sqlu As String = "UPDATE ourscheduledreports SET [Status]='processed', Prop2='" & filepath.Replace("\", "*") & "' WHERE ID=" & Request("schedid").ToString.Trim
                            ret = ExequteSQLquery(sqlu)
                            Session("nruntimes") = Session("nruntimes") + 1
                        Catch ex As Exception
                            If Not ex.Message.StartsWith("Thread ") Then
                                ret = "ERROR!!  " & ex.Message
                            End If
                        End Try

                        Response.Redirect("RunScheduledReports.aspx")
                    Else
                        'not scheduled report
                        filepath = Session("appldirExcelFiles") & Session("myfile")
                        filepath = filepath.Replace("/", "\").Replace("\\", "\")
                    End If

                    Using Stream As New FileStream(filepath, FileMode.Create)
                        Stream.Write(Bytes, 0, Bytes.Length)
                        Stream.Close()
                        Stream.Dispose()
                    End Using

                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(filepath)
                    Catch ex As Exception
                        If Not ex.Message.StartsWith("Thread ") Then
                            ret = "ERROR!!  " & ex.Message
                        End If
                    End Try
                    Response.End()
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            If Not ret.StartsWith("Thread ") Then
                MessageBox.Show("ERROR!! " & ret, "Error!", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Else
                'If LabelRunSched.Text = "yes" Then
                '    'Response.Redirect("RunScheduledReports.aspx?todown=yes&filename=" & Session("myfile"))
                '    Response.Redirect("RunScheduledReports.aspx")
                'End If
            End If
        End Try
        Return ret
    End Function

    Private Function SeeGraph(ByVal graphtype As String) As String
        lnkWord.Visible = False
        lnkWord.Enabled = False
        If Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LinkButtonDownRDL.Visible = True
            LinkButtonDownRDL.Enabled = True
        End If
        Session("GraphRDL") = ""
        If graphtype = "" Then graphtype = "bar"
        Session("Graph") = "yes"
        Session("GraphType") = graphtype
        Session("matrix") = ""
        Dim ret As String = String.Empty

        If Session("dv3") Is Nothing Then
            Return ret
        End If
        ScriptManager1.AsyncPostBackTimeout = "999999"

        viewer.Visible = True
        viewer.Enabled = True
        viewer.ZoomMode = Microsoft.Reporting.WebForms.ZoomMode.FullPage
        viewer.PageCountMode = Microsoft.Reporting.WebForms.PageCountMode.Actual
        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local
        Dim n As Integer
        Dim i As Integer
        Dim dt As DataTable = CType(Session("dv3"), DataView).ToTable
        dt = CorrectDatasetColumns(dt)
        Dim graphstr As String = String.Empty
        Dim paramstr As String = String.Empty
        Try
            If Session("ParamsCount") Is Nothing Then
                Session("ParamsCount") = 0
            Else
                n = Session("ParamsCount")
                For i = 0 To Session("ParamsCount") - 1
                    Dim parvalue As String = Session("ParamNames")(i) & ":   " & Session("ParamValues")(i).ToString
                    paramstr = paramstr + parvalue
                    paramstr = paramstr + ", "
                Next
            End If
            'If Not Session("addwhere") Is Nothing AndAlso Session("addwhere").ToString.Trim <> "" Then
            '    paramstr = Session("addwhere").ToString
            'End If
            If Not Session("WhereStm") Is Nothing AndAlso Session("WhereStm").ToString.Trim <> "" Then
                paramstr = Session("WhereStm").ToString.Replace("[", "").Replace("]", "")
            End If
            If DropDownList4.SelectedValue = "Value" Then
                graphstr = CreateGraphRDLForDTValue(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, DropDownList4.SelectedValue, DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue, graphtype, "landscape", Session("PageFtr").ToString)

            Else
                graphstr = CreateGraphRDLForDT(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, DropDownList4.SelectedValue, DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue, graphtype, "landscape", Session("PageFtr").ToString)

            End If

            If graphstr <> "" AndAlso Not graphstr.StartsWith("ERROR!!") Then
                Session("GraphRDL") = graphstr
                ''----------------------------------------------------------
                ''For testing only, comment this block for production
                'Dim graphpath As String = System.AppDomain.CurrentDomain.BaseDirectory() & "RDLFILES\graph" & Session("REPORTID") & Now.ToString.Replace(" ", "").Replace(":", "") & graphtype & ".rdl"
                ''save to file
                'Dim Bytes() As Byte = Encoding.UTF8.GetBytes(graphstr)
                'Using Stream As New FileStream(graphpath, FileMode.Create)
                '    Stream.Write(Bytes, 0, Bytes.Length)
                '    Stream.Close()
                '    Stream.Dispose()
                'End Using
                ''----------------------------------------------------------

                ''Dim rdlstream As Stream = Nothing
                'Dim byteArray As Byte() = Encoding.UTF8.GetBytes(graphstr)
                'Dim rdlstream As MemoryStream = New MemoryStream(byteArray)
                'rdlstream.Position = 0
                'viewer.LocalReport.LoadReportDefinition(rdlstream)


                Dim textread As New StringReader(graphstr)
                viewer.LocalReport.LoadReportDefinition(textread)
                viewer.LocalReport.DataSources.Clear()
                Dim rds As Microsoft.Reporting.WebForms.ReportDataSource = New Microsoft.Reporting.WebForms.ReportDataSource
                rds.Name = Session("REPORTID")
                rds.Value = dt
                viewer.LocalReport.DataSources.Add(rds)
            End If
            Session("GraphRDL") = graphstr
            If Not Session("srd") Is Nothing AndAlso (Session("srd") = 4 Or Session("srd") = 5 Or Session("srd") = 6) Then

                'Render the report with viewer dimentions
                pagewidth = Session("pagewidth")
                pageheight = Session("pageheight")
                If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
                If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
                Dim mimeType As String = ""
                Dim encoding As String = ""
                Dim fileNameExtension As String = ""
                Dim streams As String() = Nothing
                Dim warnings As Warning() = Nothing
                Dim deviceInf As String = "" ' "<DeviceInfo>" & "  <OutputFormat>PNG</OutputFormat>"
                deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
                deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
                deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
                deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
                deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
                deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
                deviceInf = deviceInf & "</DeviceInfo>"

                Dim appldirExcelFiles, myfile As String
                'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                Dim datest, timest As String
                datest = DateString()
                timest = TimeString()
                myfile = Session("REPORTID") & "Graph" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)
                appldirExcelFiles = applpath & "Temp\"
                Session("appldirExcelFiles") = appldirExcelFiles
                Session("myfile") = myfile
                Dim Bytes() As Byte = Nothing
                If Session("srd") = 4 Then
                    myfile = myfile & ".xls"
                    Bytes = viewer.LocalReport.Render("Excel")
                ElseIf Session("srd") = 5 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>WORD</OutputFormat>" & deviceInf
                    myfile = myfile & ".doc"
                    Bytes = viewer.LocalReport.Render("Word", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)
                ElseIf Session("srd") = 6 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>PDF</OutputFormat>" & deviceInf
                    myfile = myfile & ".pdf"
                    Bytes = viewer.LocalReport.Render("PDF", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

                End If
                If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                    Session("myfile") = myfile
                    Dim filepath As String = String.Empty
                    'scheduled report
                    If LabelRunSched.Text = "yes" AndAlso Request("schedid") IsNot Nothing AndAlso IsNumeric(Request("schedid").ToString.Trim) Then
                        'Dim fileuploadDir As String = applpath 'ConfigurationManager.AppSettings("fileupload").ToString
                        filepath = applpath & myfile
                        'filepath = filepath.Replace("\\", "\")
                        Using Stream As New FileStream(filepath, FileMode.Create)
                            Stream.Write(Bytes, 0, Bytes.Length)
                            Stream.Close()
                            Stream.Dispose()
                        End Using
                        Try
                            Dim sqlu As String = "UPDATE ourscheduledreports SET [Status]='processed', Prop2='" & filepath.Replace("\", "*") & "' WHERE ID=" & Request("schedid").ToString.Trim
                            ret = ExequteSQLquery(sqlu)
                            Session("nruntimes") = Session("nruntimes") + 1
                        Catch ex As Exception
                            If Not ex.Message.StartsWith("Thread ") Then
                                ret = "ERROR!!  " & ex.Message
                            End If
                        End Try

                        Response.Redirect("RunScheduledReports.aspx")
                    Else
                        'not scheduled report
                        filepath = Session("appldirExcelFiles") & Session("myfile")
                        filepath = filepath.Replace("/", "\").Replace("\\", "\")
                    End If
                    Using Stream As New FileStream(Session("appldirExcelFiles") & Session("myfile"), FileMode.Create)
                        Stream.Write(Bytes, 0, Bytes.Length)
                        Stream.Close()
                        Stream.Dispose()
                    End Using
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        If Not ex.Message.StartsWith("Thread ") Then
                            ret = "ERROR!!  " & ex.Message
                        End If

                    End Try
                    Response.End()
                End If
            End If

        Catch ex As Exception
            ret = ex.Message
            If Not ret.StartsWith("Thread ") Then
                MessageBox.Show("ERROR!! " & ret, "Error!", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                'Response.Redirect("ShowReport.aspx?srd=0")
            End If
        End Try
        Return ret
    End Function
    Private Function SeeMatrix() As String
        lnkWord.Visible = True
        lnkWord.Enabled = True
        If Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LinkButtonDownRDL.Visible = True
            LinkButtonDownRDL.Enabled = True
        End If
        If DropDownList1.SelectedValue = DropDownList2.SelectedValue Then
            LabelError.Text = "Categories must be different for Matrix report!"
            LabelError.Visible = True
            Return "Categories must be different for Matrix report!"
        End If

        If DropDownList4.SelectedValue = "Value" Then
            LabelError.Text = "Matrix report is not available for function Value."
            LabelError.Visible = True
            Return "Matrix report is not available for function Value."
        End If
        Session("GraphRDL") = ""
        Session("GraphType") = "matrix"
        Dim ret As String = String.Empty
        If Session("dv3") Is Nothing Then
            Return ret
        End If
        If DropDownList4.SelectedValue = "Value" Then
            Return ret
        End If
        ScriptManager1.AsyncPostBackTimeout = "999999"
        viewer.Visible = True
        viewer.Enabled = True
        viewer.ZoomMode = Microsoft.Reporting.WebForms.ZoomMode.FullPage
        viewer.PageCountMode = Microsoft.Reporting.WebForms.PageCountMode.Actual
        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local
        Dim n As Integer
        Dim i As Integer
        Dim dt As DataTable = CType(Session("dv3"), DataView).ToTable
        dt = CorrectDatasetColumns(dt)
        Dim er As String = String.Empty
        Dim mtbl As DataTable = Nothing
        'calculate mtbl from scratch and convert into csv file for AI
        mtbl = CalculateDataTableForMatrixReportAI(Session("REPORTID"), dt, DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue, DropDownList4.SelectedValue)
        mtbl = MakeDTColumnsNamesCLScompliant(mtbl, Session("UserConnProvider"), er)
        Session("datatable") = mtbl
        Session("QuestionToAI") = "Explore data of report - " & Session("REPTITLE") & " - for " & DropDownList4.SelectedValue & " of " & DropDownList3.SelectedValue & " by " & DropDownList1.SelectedValue & ", " & DropDownList2.SelectedValue
        Session("DataToChatAI") = ExportToCSVtext(Session("datatable"), Chr(9))
        Session("OriginalDataTable") = Session("dataTable")
        Session("matrix") = "yes"
        Dim graphstr As String = String.Empty
        Dim paramstr As String = String.Empty
        Try
            If Session("ParamsCount") Is Nothing Then
                Session("ParamsCount") = 0
            Else
                n = Session("ParamsCount")
                For i = 0 To Session("ParamsCount") - 1
                    Dim parvalue As String = Session("ParamNames")(i) & ":   " & Session("ParamValues")(i).ToString
                    paramstr = paramstr + parvalue
                    paramstr = paramstr + ", "
                Next
            End If
            'If Not Session("addwhere") Is Nothing AndAlso Session("addwhere").ToString.Trim <> "" Then
            '    paramstr = Session("addwhere").ToString
            'End If
            If Not Session("WhereStm") Is Nothing AndAlso Session("WhereStm").ToString.Trim <> "" Then
                paramstr = Session("WhereStm").ToString.Replace("[", "").Replace("]", "")
            End If

            'with links
            graphstr = CreateMatrixRDLWithLinks(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, DropDownList4.SelectedValue, DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue, "", "landscape", Session("PageFtr"))
            'no links
            'graphstr = CreateMatrixRDLForDT(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, DropDownList4.SelectedValue, DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue, "", "landscape", Session("PageFtr"))
            Dim appldirExcelFiles, myfile As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datest, timest As String
            datest = DateString()
            timest = TimeString()
            myfile = Session("REPORTID") & "Graph" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)
            appldirExcelFiles = applpath & "Temp\"
            Session("appldirExcelFiles") = appldirExcelFiles
            Session("myfile") = myfile
            Dim Bytes() As Byte = Nothing

            If graphstr <> "" AndAlso Not graphstr.StartsWith("ERROR!!") Then
                Session("GraphRDL") = graphstr
                ''----------------------------------------------------------
                ''For testing only, comment this block for production
                'Dim graphpath As String = System.AppDomain.CurrentDomain.BaseDirectory() & "RDLFILES\testingmatrix.rdl"
                ''save to file
                'Dim Bytes() As Byte = Encoding.UTF8.GetBytes(graphstr)
                'Using Stream As New FileStream(graphpath, FileMode.Create)
                '    Stream.Write(Bytes, 0, Bytes.Length)
                '    Stream.Close()
                '    Stream.Dispose()
                'End Using
                ''----------------------------------------------------------

                'Dim byteArray As Byte() = Encoding.UTF8.GetBytes(graphstr)
                'Dim rdlstream As Stream
                'rdlstream = New MemoryStream(byteArray)
                'rdlstream.Position = 0

                Dim textread As New StringReader(graphstr)
                viewer.LocalReport.LoadReportDefinition(textread)
                viewer.LocalReport.EnableHyperlinks = True  'for matrix with links
                'viewer.LocalReport.
                viewer.LocalReport.DataSources.Clear()
                Dim rds As Microsoft.Reporting.WebForms.ReportDataSource = New Microsoft.Reporting.WebForms.ReportDataSource
                rds.Name = Session("REPORTID")
                rds.Value = dt
                viewer.LocalReport.DataSources.Add(rds)
            End If


            If Not Session("srd") Is Nothing AndAlso (Session("srd") = 4 Or Session("srd") = 5 Or Session("srd") = 6 Or Session("srd") = 18) Then

                'Render the report  with viewer dimentions
                pagewidth = Session("pagewidth")
                pageheight = Session("pageheight")
                If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
                If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
                Dim mimeType As String = ""
                Dim encoding As String = ""
                Dim fileNameExtension As String = ""
                Dim streams As String() = Nothing
                Dim warnings As Warning() = Nothing
                Dim deviceInf As String = "" ' "<DeviceInfo>" & "  <OutputFormat>PNG</OutputFormat>"
                deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
                deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
                deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
                deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
                deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
                deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
                deviceInf = deviceInf & "</DeviceInfo>"

                If Session("srd") = 4 Then
                    myfile = myfile & ".xls"
                    Bytes = viewer.LocalReport.Render("Excel")
                ElseIf Session("srd") = 5 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>WORD</OutputFormat>" & deviceInf
                    myfile = myfile & ".doc"
                    Bytes = viewer.LocalReport.Render("Word", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)
                ElseIf Session("srd") = 6 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>PDF</OutputFormat>" & deviceInf
                    myfile = myfile & ".pdf"
                    Bytes = viewer.LocalReport.Render("PDF", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

                End If
                If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                    Session("myfile") = myfile
                    Dim filepath As String = String.Empty
                    'scheduled report
                    If LabelRunSched.Text = "yes" AndAlso Request("schedid") IsNot Nothing AndAlso IsNumeric(Request("schedid").ToString.Trim) Then
                        'Dim fileuploadDir As String = ConfigurationManager.AppSettings("fileupload").ToString
                        filepath = applpath & myfile
                        'filepath = filepath.Replace("\\", "\")
                        Using Stream As New FileStream(filepath, FileMode.Create)
                            Stream.Write(Bytes, 0, Bytes.Length)
                            Stream.Close()
                            Stream.Dispose()
                        End Using
                        Try
                            Dim sqlu As String = "UPDATE ourscheduledreports SET [Status]='processed', Prop2='" & filepath.Replace("\", "*") & "' WHERE ID=" & Request("schedid").ToString.Trim
                            ret = ExequteSQLquery(sqlu)
                            Session("nruntimes") = Session("nruntimes") + 1
                        Catch ex As Exception
                            If Not ex.Message.StartsWith("Thread ") Then
                                ret = "ERROR!!  " & ex.Message
                            End If
                        End Try
                        Response.Redirect("RunScheduledReports.aspx")
                    Else
                        'not scheduled report
                        filepath = Session("appldirExcelFiles") & Session("myfile")
                        filepath = filepath.Replace("/", "\").Replace("\\", "\")
                    End If




                    Using Stream As New FileStream(Session("appldirExcelFiles") & Session("myfile"), FileMode.Create)
                        Stream.Write(Bytes, 0, Bytes.Length)
                        Stream.Close()
                        Stream.Dispose()
                    End Using
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        If Not ex.Message.StartsWith("Thread ") Then
                            ret = "ERROR!!  " & ex.Message
                        End If

                    End Try
                    Response.End()
                End If
            Else
                'myfile = myfile & ".xls"
                'Bytes = viewer.LocalReport.Render("Excel")
                'If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                '    'Session("myfile") = myfile
                '    Using Stream As New FileStream(Session("appldirExcelFiles") & myfile, FileMode.Create)
                '        Stream.Write(Bytes, 0, Bytes.Length)
                '        Stream.Close()
                '        Stream.Dispose()
                '    End Using
                'End If
                'Dim mtbl As DataTable = Nothing
                ''calculate mtbl from scratch and convert into csv file
                'mtbl = CalculateDataTableForMatrixReportAI(Session("REPORTID"), dt, DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue, DropDownList4.SelectedValue)
                'mtbl = MakeDTColumnsNamesCLScompliant(mtbl, Session("UserConnProvider"), ret)
                'Session("datatable") = mtbl
                'Session("QuestionToAI") = "Explore data of report - " & Session("REPTITLE") & " - for " & DropDownList4.SelectedValue & " of " & DropDownList3.SelectedValue & " by " & DropDownList1.SelectedValue & ", " & DropDownList2.SelectedValue
                'Session("DataToChatAI") = ExportToCSVtext(Session("datatable"), Chr(9))
                'Session("OriginalDataTable") = Session("dataTable")


                ''restore viewer
                'Dim textread As New StringReader(graphstr)
                'viewer.LocalReport.LoadReportDefinition(textread)
                'viewer.LocalReport.EnableHyperlinks = True  'for matrix with links
                ''viewer.LocalReport.
                'viewer.LocalReport.DataSources.Clear()
                'Dim rds As Microsoft.Reporting.WebForms.ReportDataSource = New Microsoft.Reporting.WebForms.ReportDataSource
                'rds.Name = Session("REPORTID")
                'rds.Value = dt
                'viewer.LocalReport.DataSources.Add(rds)

            End If

        Catch ex As Exception
            ret = ex.Message
            If Not ret.StartsWith("Thread ") Then
                MessageBox.Show("ERROR!! " & ret, "Error!", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                'Response.Redirect("ShowReport.aspx?srd=0")
            End If
        End Try
        Return ret
    End Function

    Private Sub ButtonShowGraph_Click(sender As Object, e As EventArgs) Handles ButtonShowGraph.Click
        Session("See") = "Graph"
        Dim rt As String = SeeGraph("bar")
        Dim er As String = String.Empty
        If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
    End Sub
    Private Sub ButtonPie_Click(sender As Object, e As EventArgs) Handles ButtonPie.Click
        'If DropDownList4.SelectedValue = "Value" Then
        '    MessageBox.Show("Pie graph does not support the selection Value", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        '    Exit Sub
        'End If
        Session("See") = "Pie"
        Dim rt As String = SeeGraph("pie")
        Dim er As String = String.Empty
        If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
    End Sub
    Private Sub ButtonLine_Click(sender As Object, e As EventArgs) Handles ButtonLine.Click
        'If DropDownList4.SelectedValue = "Value" Then
        '    MessageBox.Show("Line graph does not support the selection Value", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        '    Exit Sub
        'End If
        Session("See") = "Line"
        Dim rt As String = SeeGraph("line")
        Dim er As String = String.Empty
        If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
    End Sub
    Private Sub DoBar(Provider As String)
        If Provider = "Oracle.ManagedDataAccess.Client" Then
            Dim cat1text As String = DropDownList1.Text
            Dim cat2text As String = DropDownList2.Text
            Dim cat1 As String = DropDownList1.SelectedValue
            Dim cat2 As String = DropDownList2.SelectedValue
            Session("AxisY") = DropDownList3.SelectedValue
            Session("Aggregate") = DropDownList4.SelectedValue



            If cat1text = cat2text Then
                MessageBox.Show("Primary and Secondary Categories should be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            If cat1 <> String.Empty AndAlso cat2 <> String.Empty AndAlso Session("AxisY") <> String.Empty AndAlso Session("Aggregate") <> String.Empty Then
                Dim urlc As String = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=bar&srd=11"
                Session("srd") = 11
                Response.Redirect(urlc)
            End If
        Else
            ButtonShowGraph_Click(Me, EventArgs.Empty)
        End If

    End Sub
    Private Sub DoPie(Provider As String)
        If Provider = "Oracle.ManagedDataAccess.Client" Then
            Dim cat1text As String = DropDownList1.Text
            Dim cat2text As String = DropDownList2.Text
            Dim cat1 As String = DropDownList1.SelectedValue
            Dim cat2 As String = DropDownList2.SelectedValue
            Session("AxisY") = DropDownList3.SelectedValue
            Session("Aggregate") = DropDownList4.SelectedValue



            If cat1text = cat2text Then
                MessageBox.Show("Primary and Secondary Categories should be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            If cat1 <> String.Empty AndAlso cat2 <> String.Empty AndAlso Session("AxisY") <> String.Empty AndAlso Session("Aggregate") <> String.Empty Then
                Dim urlc As String = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=pie&srd=11"
                Session("srd") = 11
                Response.Redirect(urlc)
            End If
        Else
            ButtonPie_Click(Me, EventArgs.Empty)
        End If

    End Sub

    Private Sub DoLine(Provider As String)
        If Provider = "Oracle.ManagedDataAccess.Client" Then
            Dim cat1text As String = DropDownList1.Text
            Dim cat2text As String = DropDownList2.Text
            Dim cat1 As String = DropDownList1.SelectedValue
            Dim cat2 As String = DropDownList2.SelectedValue
            Session("AxisY") = DropDownList3.SelectedValue
            Session("Aggregate") = DropDownList4.SelectedValue



            If cat1text = cat2text Then
                MessageBox.Show("Primary and Secondary Categories should be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            If cat1 <> String.Empty AndAlso cat2 <> String.Empty AndAlso Session("AxisY") <> String.Empty AndAlso Session("Aggregate") <> String.Empty Then
                Dim urlc As String = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=line&srd=11"

                Session("srd") = 11
                Response.Redirect(urlc)
            End If
        Else
            ButtonLine_Click(Me, EventArgs.Empty)
        End If

    End Sub

    Private Sub DoMatrix(Provider As String)
        If Provider = "Oracle.ManagedDataAccess.Client" Then
            Dim cat1text As String = DropDownList1.Text
            Dim cat2text As String = DropDownList2.Text
            Dim cat1 As String = DropDownList1.SelectedValue
            Dim cat2 As String = DropDownList2.SelectedValue
            Session("AxisY") = DropDownList3.SelectedValue
            Session("Aggregate") = DropDownList4.SelectedValue



            If cat1text = cat2text Then
                MessageBox.Show("Primary and Secondary Categories should be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            If cat1 <> String.Empty AndAlso cat2 <> String.Empty AndAlso Session("AxisY") <> String.Empty AndAlso Session("Aggregate") <> String.Empty Then
                Dim urlc As String = "ReportViews.aspx?graph=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString & "&grtype=matrix&srd=11"
                Session("srd") = 11
                Response.Redirect(urlc)
            End If
        Else
            ButtonMatrix_Click(Me, EventArgs.Empty)
        End If
    End Sub
    Private Sub DoDrillDown(Provider As String)
        If Provider = "Oracle.ManagedDataAccess.Client" Then
            Dim cat1text As String = DropDownList1.Text
            Dim cat2text As String = DropDownList2.Text
            Dim cat1 As String = DropDownList1.SelectedValue
            Dim cat2 As String = DropDownList2.SelectedValue
            Session("AxisY") = DropDownList3.SelectedValue
            Session("Aggregate") = DropDownList4.SelectedValue



            If cat1text = cat2text Then
                MessageBox.Show("Primary and Secondary Categories should be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            If cat1 <> String.Empty AndAlso cat2 <> String.Empty AndAlso Session("AxisY") <> String.Empty AndAlso Session("Aggregate") <> String.Empty Then
                Dim urlc As String = "ReportViews.aspx?det=yes&cat1=" & cat1.ToString & "&cat2=" & cat2.ToString
                Session("srd") = 11
                Response.Redirect(urlc)
            End If
        Else
            ButtonDynamicReport_Click(Me, EventArgs.Empty)
        End If

    End Sub
    Private Sub ButtonMatrix_Click(sender As Object, e As EventArgs) Handles ButtonMatrix.Click
        lnkImage.Visible = False
        lnkImage.Enabled = False
        Dim rt As String = String.Empty
        Dim er As String = String.Empty
        If DropDownList1.Text = DropDownList2.Text Then
            MessageBox.Show("Primary And Secondary Categories should be different!", "Primary And Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        Else
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
        If DropDownList4.Text = "Value" Then
            MessageBox.Show("Matrix report is not available for function Value.", "Function", "Function", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If
        Session("See") = "Matrix"
        rt = SeeMatrix()
    End Sub

    Private Sub DropDownList3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList3.SelectedIndexChanged
        LabelSQL.Visible = False
        chkboxNumeric.Checked = False
        If Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LinkButtonDownRDL.Visible = True
            LinkButtonDownRDL.Enabled = True
        Else
            LinkButtonDownRDL.Visible = False
            LinkButtonDownRDL.Enabled = False
        End If
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
            chkboxNumeric.Checked = True
            'TODO make check if all values in column are numeric 
            'Else
            '    Dim er As String = String.Empty
            '    Dim tbl As String = FindTableToTheField(repid, DropDownList3.SelectedValue, Session("UserConnString"), Session("UserConnProvider"), er)
            '    If tbl <> "" AndAlso TblFieldIsDateTime(tbl, DropDownList3.SelectedValue, Session("UserConnString"), Session("UserConnProvider")) Then
            '        DropDownList4.Items.Add("Value")
            '    Else
            '        'check if field has only datetime or only numeric values
            '        Dim coltype As String = String.Empty
            '        Dim collen As String = String.Empty
            '        Dim coldate As String = String.Empty
            '        Dim dt As DataTable = ColumnValuesType(dv3.Table, DropDownList3.SelectedValue, coltype, collen, coldate)
            '        If coldate = "date" Then
            '            dv3 = dt.DefaultView
            '            DropDownList4.Items.Add("Value")
            '        ElseIf coldate <> "date" AndAlso coltype = "num" Then
            '            dv3 = dt.DefaultView
            '            DropDownList4.Items.Add("Value")
            '        End If
            '    End If
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

    Private Sub LinkButtonDownRDL_Click(sender As Object, e As EventArgs) Handles LinkButtonDownRDL.Click
        Response.Redirect("ReportViews.aspx?downrdl=yes")

    End Sub

    Private Function DownloadXML(ByVal str As String, ByVal repfile As String) As String
        Dim ErrorLog = String.Empty
        Dim rdlpath As String = applpath & "RDLFILES\"
        Dim doc As New XmlDocument
        Try
            doc.Load(New StringReader(str))
            If File.Exists(rdlpath & repfile) Then
                File.Delete(rdlpath & repfile)
            End If
            doc.Save(rdlpath & repfile)
            doc = Nothing
        Catch ex As Exception
            ErrorLog = "ERROR!!  " & ex.Message
        End Try
        'Try
        '    Response.ContentType = "application/octet-stream"
        '    Response.AppendHeader("Content-Disposition", "attachment; filename=" & repfile)
        '    Response.TransmitFile(applpath & repfile)
        'Catch ex As Exception
        '    ErrorLog = "ERROR!!  " & ex.Message
        'End Try
        'Response.End()
        Return ErrorLog
    End Function

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Dim gr As String = String.Empty
        If Session("See") = "Report" Then
            gr = SeeReport()
        ElseIf Session("See") = "Graph" Then
            gr = SeeGraph("bar")  'bar
        ElseIf Session("See") = "Matrix" Then
            gr = SeeMatrix()
        ElseIf Session("See") = "Pie" Then
            gr = SeeGraph("pie")
        ElseIf Session("See") = "Line" Then
            gr = SeeGraph("line")
        ElseIf Session("See") = "GroupsStat" Then
            gr = SeeDynamicReportForGroups()
        ElseIf Session("See") = "Details" Then
            gr = SeeDynamicReport(DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue)

        End If
    End Sub
    Private Function SearchStatement() As String
        Dim srch As String = String.Empty
        If DropDownColumns.SelectedValue.Trim <> "" AndAlso DropDownOperator.SelectedValue.Trim <> "" AndAlso TextBoxSearch.Text <> "" Then
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
                    srch = srchfld & " " & oper & " ('" & val.Replace("(", "").Replace(")", "").Replace(",", "','") & "')"
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
            'Else
            '    Session("srchfld") = Nothing
            '    Session("srchoper") = Nothing
            '    Session("srchval") = Nothing
            '    If DropDownColumns.Items.Count > 0 Then
            '        DropDownColumns.SelectedIndex = 0
            '    End If
            '    DropDownOperator.SelectedIndex = 0
            '    TextBoxSearch.Text = String.Empty
        End If
        Session("Search") = srch
        Return srch
    End Function
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
    '                    If Trim(dirsort) <> "" Then mSql = mSql & " ORDER BY " & dirsort  'where dirsort is coming from?
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
    '    Else 'sp                                                  'Stored procedure
    '        mSql = Trim(dv1.Table.Rows(0)("SQLquerytext"))
    '        '!!!!!!!!!!!!!!!!!!!!!  Should be this one:
    '        If Session("UserConnProvider").StartsWith("InterSystems.Data.") AndAlso mSql.Contains("||") Then
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
    '        'dv3 = CorrectDataset(dv3.Table, er).DefaultView
    '    Else
    '        dv3 = CorrectDatasetColumns(dv3.Table, er).DefaultView  'from single quote
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

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted

        Select Case e.Tag
            Case "NoData"
                Response.Redirect("ListOfReports.aspx")
            Case "Categories"

            Case "Error"
                Response.Redirect("ShowReport.aspx?srd=0")
        End Select
    End Sub

    Private Sub ButtonDynamicReport_Click(sender As Object, e As EventArgs) Handles ButtonDynamicReport.Click
        If DropDownList1.Text = DropDownList2.Text Then  'cat1=cat2
            'MessageBox.Show("Primary and Secondary Categories shoud be different!", "Primary and Secondary Categories", "Categories", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            'Exit Sub
            dv3 = Session("dv3")
            Dim GroupTable As DataTable = Nothing
            'make GroupTable
            GroupTable = GetReportGroups("0")
            Dim row As DataRow = GroupTable.NewRow
            row("ReportId") = Session("REPORTID")
            row("GroupField") = DropDownList1.Text
            row("CalcField") = DropDownList3.Text
            row("CntChk") = 1
            row("CntDistChk") = 1
            If ColumnTypeIsNumeric(dv3.Table.Columns(DropDownList3.Text)) Then
                row("SumChk") = 1
                row("MaxChk") = 1
                row("MinChk") = 1
                row("AvgChk") = 1
                row("StDevChk") = 0
            Else
                row("SumChk") = 0
                row("MaxChk") = 0
                row("MinChk") = 0
                row("AvgChk") = 0
                row("StDevChk") = 0
            End If
            row("FirstChk") = 0
            row("LastChk") = 0
            row("PageBrk") = 0
            row("Indx") = 1
            row("GrpOrder") = 1
            GroupTable.Rows.Add(row)
            Dim ret As String = SeeDynamicReportForGroups(GroupTable)
        Else
            lnkExpandAll.Visible = False
            lnkExpandAll.Enabled = False
            Dim rt As String = SeeDynamicReport(DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue)
            'Dim er As String = String.Empty
            'If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            '    rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
            'End If
        End If
        Session("See") = "Details"
    End Sub
    Private Function SeeDynamicReport(ByVal cat1 As String, ByVal cat2 As String, ByVal calc1 As String) As String
        If DropDownList1.SelectedValue = DropDownList2.SelectedValue Then
            Return "Categories must be different for DrillDown report!"
        End If
        Session("matrix") = ""
        lnkWord.Visible = True
        lnkWord.Enabled = True
        If Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LinkButtonDownRDL.Visible = True
            LinkButtonDownRDL.Enabled = True
        End If
        Session("GraphRDL") = ""
        Session("GraphType") = "Details"
        Session("DynamicType") = "DrillDown"
        Dim ret As String = String.Empty
        Dim n As Integer
        Dim i As Integer
        If Session("dv3") Is Nothing Then
            Return ret
        End If
        ScriptManager1.AsyncPostBackTimeout = "999999"
        viewer.Visible = True
        viewer.Enabled = True
        viewer.ZoomMode = Microsoft.Reporting.WebForms.ZoomMode.FullPage
        viewer.PageCountMode = Microsoft.Reporting.WebForms.PageCountMode.Actual
        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local
        'viewer.HyperlinkTarget = "_blank"

        Dim dt As DataTable = CType(Session("dv3"), DataView).ToTable
        dt = CorrectDatasetColumns(dt)
        Dim graphstr As String = String.Empty
        Dim paramstr As String = String.Empty
        Dim parvalue As String
        Try
            If Session("ParamsCount") Is Nothing Then
                Session("ParamsCount") = 0
            Else
                n = Session("ParamsCount")
                For i = 0 To Session("ParamsCount") - 1
                    parvalue = Session("ParamValues")(i).ToString
                    If parvalue.ToUpper <> "ALL" Then
                        paramstr = paramstr + Session("ParamNames")(i) & ":   " & Session("ParamValues")(i).ToString
                        paramstr = paramstr + ", "
                    End If
                Next
                If paramstr.EndsWith(",") Then
                    paramstr = paramstr.Substring(0, paramstr.LastIndexOf(","))
                End If
            End If

            Dim dtf As DataTable = dt
            If Not Session("WhereStm") Is Nothing AndAlso Session("WhereStm").ToString.Trim <> "" Then
                paramstr = Session("WhereStm").ToString.Replace("[", "").Replace("]", "")
                If paramstr.Trim <> "" Then
                    dt.DefaultView.RowFilter = paramstr
                End If
                dtf = dt.DefaultView.ToTable
            End If
            Dim er As String = String.Empty
            Dim rt As String = String.Empty
            'If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            '    rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
            'End If

            If Not Session("addwhere") Is Nothing AndAlso Session("addwhere").ToString.Trim <> "" Then
                paramstr = Session("addwhere").ToString
            End If
            'graphstr = GenerateDynamicReportOriginal(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, cat1, cat2, calc1, "", "portrait", Session("PageFtr"))
            expand1 = False
            graphstr = GenerateDynamicReport(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, cat1, cat2, calc1, "", "portrait", Session("PageFtr"), "details", expand1, expand2)
            If graphstr <> "" AndAlso Not graphstr.StartsWith("ERROR!!") Then
                Session("DynamicRDL") = graphstr
                Session("GraphRDL") = graphstr
                If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
                    rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
                End If
                ''----------------------------------------------------------
                ''For testing only, comment this block for production
                'Dim graphpath As String = System.AppDomain.CurrentDomain.BaseDirectory() & "RDLFILES\testingmatrix.rdl"
                ''save to file
                'Dim Bytes() As Byte = Encoding.UTF8.GetBytes(graphstr)
                'Using Stream As New FileStream(graphpath, FileMode.Create)
                '    Stream.Write(Bytes, 0, Bytes.Length)
                '    Stream.Close()
                '    Stream.Dispose()
                'End Using
                ''----------------------------------------------------------

                'Dim byteArray As Byte() = Encoding.UTF8.GetBytes(graphstr)
                'Dim rdlstream As Stream
                'rdlstream = New MemoryStream(byteArray)
                'rdlstream.Position = 0

                Dim textread As New StringReader(graphstr)
                viewer.LocalReport.EnableHyperlinks = True
                viewer.LocalReport.LoadReportDefinition(textread)
                viewer.LocalReport.DataSources.Clear()
                Dim rds As Microsoft.Reporting.WebForms.ReportDataSource = New Microsoft.Reporting.WebForms.ReportDataSource
                rds.Name = Session("REPORTID")
                rds.Value = dtf
                viewer.LocalReport.DataSources.Add(rds)
            End If
            Session("GraphRDL") = graphstr
            If Not IsPostBack AndAlso Not Session("srd") Is Nothing AndAlso (Session("srd") = 4 Or Session("srd") = 5 Or Session("srd") = 6 Or Session("srd") = 18) Then
                'Render the report  with viewer dimentions
                pagewidth = Session("pagewidth")
                pageheight = Session("pageheight")
                If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
                If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
                Dim mimeType As String = ""
                Dim encoding As String = ""
                Dim fileNameExtension As String = ""
                Dim streams As String() = Nothing
                Dim warnings As Warning() = Nothing
                Dim deviceInf As String = "" ' "<DeviceInfo>" & "  <OutputFormat>PNG</OutputFormat>"
                deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
                deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
                deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
                deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
                deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
                deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
                deviceInf = deviceInf & "</DeviceInfo>"

                Dim appldirExcelFiles, myfile As String
                'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                Dim datest, timest As String
                datest = DateString()
                timest = TimeString()
                myfile = Session("REPORTID") & "Details" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)
                appldirExcelFiles = applpath & "Temp\"
                Session("appldirExcelFiles") = appldirExcelFiles
                Session("myfile") = myfile
                Dim Bytes() As Byte = Nothing
                If Session("srd") = 4 Then
                    myfile = myfile & ".xls"
                    Bytes = viewer.LocalReport.Render("Excel")
                ElseIf Session("srd") = 5 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>WORD</OutputFormat>" & deviceInf
                    myfile = myfile & ".doc"
                    Bytes = viewer.LocalReport.Render("Word", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)
                ElseIf Session("srd") = 6 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>PDF</OutputFormat>" & deviceInf
                    myfile = myfile & ".pdf"
                    Bytes = viewer.LocalReport.Render("PDF", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

                End If
                If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                    Session("myfile") = myfile
                    Dim filepath As String = String.Empty
                    'scheduled report
                    If LabelRunSched.Text = "yes" AndAlso Request("schedid") IsNot Nothing AndAlso IsNumeric(Request("schedid").ToString.Trim) Then
                        'Dim fileuploadDir As String = ConfigurationManager.AppSettings("fileupload").ToString
                        filepath = applpath & myfile
                        'filepath = filepath.Replace("\\", "\")
                        Using Stream As New FileStream(filepath, FileMode.Create)
                            Stream.Write(Bytes, 0, Bytes.Length)
                            Stream.Close()
                            Stream.Dispose()
                        End Using
                        Try
                            Dim sqlu As String = "UPDATE ourscheduledreports SET [Status]='processed', Prop2='" & filepath.Replace("\", "*") & "' WHERE ID=" & Request("schedid").ToString.Trim
                            ret = ExequteSQLquery(sqlu)
                            Session("nruntimes") = Session("nruntimes") + 1
                        Catch ex As Exception
                            If Not ex.Message.StartsWith("Thread ") Then
                                ret = "ERROR!!  " & ex.Message
                            End If
                        End Try

                        Response.Redirect("RunScheduledReports.aspx")
                    Else
                        'not scheduled report
                        filepath = Session("appldirExcelFiles") & Session("myfile")
                        filepath = filepath.Replace("/", "\").Replace("\\", "\")
                    End If
                    Using Stream As New FileStream(Session("appldirExcelFiles") & Session("myfile"), FileMode.Create)
                        Stream.Write(Bytes, 0, Bytes.Length)
                        Stream.Close()
                        Stream.Dispose()
                    End Using
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        If Not ex.Message.StartsWith("Thread ") Then
                            ret = "ERROR!!  " & ex.Message
                        End If

                    End Try
                    Response.End()
                End If
            End If

        Catch ex As Exception
            ret = ex.Message
            If Not ret.StartsWith("Thread ") Then
                MessageBox.Show("ERROR!! " & ret, "Error!", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                'Response.Redirect("ShowReport.aspx?srd=0")
            End If
        End Try
        Return ret
    End Function

    Private Function SeeDynamicReportForGroups(Optional ByVal GroupTable As DataTable = Nothing, Optional showdetls As Boolean = False) As String
        'If DropDownList1.SelectedValue = DropDownList2.SelectedValue Then
        '    Return "Categories must be different for DrillDown reports!"
        'End If
        lnkWord.Visible = True
        lnkWord.Enabled = True
        If Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LinkButtonDownRDL.Visible = True
            LinkButtonDownRDL.Enabled = True
        End If
        Session("matrix") = ""
        Session("GraphRDL") = ""
        Session("GraphType") = "GroupsStats"
        Session("DynamicType") = "GroupsStats"
        Session("See") = "GroupsStats"
        Dim ret As String = String.Empty
        Dim dtgrp As New DataTable
        'Dim showdetls As Boolean = False
        If GroupTable Is Nothing OrElse GroupTable.Rows.Count = 0 Then
            dtgrp = GetReportGroups(repid) 'group table
            If dtgrp.Rows.Count = 1 OrElse (dtgrp.Rows.Count = 2 AndAlso dtgrp.Rows(0)("GroupField").ToString.Trim = "Overall") Then
                lnkExpandAll.Visible = True
                lnkExpandAll.Enabled = True
            Else
                lnkExpandAll.Visible = False
                lnkExpandAll.Enabled = False
            End If
        Else
            dtgrp = GroupTable
            showdetls = True
            lnkExpandAll.Visible = False
            lnkExpandAll.Enabled = False
        End If
        If dtgrp.Rows.Count < 1 Then
            ret = "No groups defined for report... "
            Return ret
        End If
        Dim n As Integer
        Dim i As Integer
        If Session("dv3") Is Nothing Then
            Return ret
        End If
        ScriptManager1.AsyncPostBackTimeout = "999999"
        viewer.Visible = True
        viewer.Enabled = True
        viewer.ZoomMode = Microsoft.Reporting.WebForms.ZoomMode.FullPage
        viewer.PageCountMode = Microsoft.Reporting.WebForms.PageCountMode.Actual
        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local
        'viewer.HyperlinkTarget = "_blank"

        Dim dt As DataTable = CType(Session("dv3"), DataView).ToTable
        dt = CorrectDatasetColumns(dt)
        Dim graphstr As String = String.Empty
        Dim paramstr As String = String.Empty
        Dim parvalue As String
        Try
            If Session("ParamsCount") Is Nothing Then
                Session("ParamsCount") = 0
            Else
                n = Session("ParamsCount")
                For i = 0 To Session("ParamsCount") - 1
                    parvalue = Session("ParamValues")(i).ToString
                    If parvalue.ToUpper <> "ALL" Then
                        paramstr = paramstr + Session("ParamNames")(i) & ":   " & Session("ParamValues")(i).ToString
                        paramstr = paramstr + ", "
                    End If
                Next
                If paramstr.EndsWith(",") Then
                    paramstr = paramstr.Substring(0, paramstr.LastIndexOf(","))
                End If
            End If
            If Not Session("WhereStm") Is Nothing AndAlso Session("WhereStm").ToString.Trim <> "" Then
                paramstr = Session("WhereStm").ToString.Replace("[", "").Replace("]", "")
            End If
            expand1 = False
            expand2 = False
            graphstr = GenerateDynamicReportForGroups(Session("REPORTID"), Session("REPTITLE"), paramstr, dt, "", "portrait", Session("PageFtr"), "groupsstat", expand1, expand2, True, GroupTable, showdetls)
            If graphstr <> "" AndAlso Not graphstr.StartsWith("ERROR!!") Then
                Session("DynamicRDL") = graphstr

                Session("GraphRDL") = graphstr
                Session("GraphType") = "GroupsStats"
                Session("DynamicType") = "GroupsStats"
                Dim textread As New StringReader(graphstr)
                viewer.LocalReport.EnableHyperlinks = True
                viewer.LocalReport.LoadReportDefinition(textread)
                viewer.LocalReport.DataSources.Clear()
                Dim rds As Microsoft.Reporting.WebForms.ReportDataSource = New Microsoft.Reporting.WebForms.ReportDataSource
                rds.Name = Session("REPORTID")
                rds.Value = dt
                viewer.LocalReport.DataSources.Add(rds)
            End If
            Session("GraphRDL") = graphstr
            If Not IsPostBack AndAlso Not Session("srd") Is Nothing AndAlso (Session("srd") = 4 Or Session("srd") = 5 Or Session("srd") = 6 Or Session("srd") = 18) Then

                'Render the report  with viewer dimentions
                pagewidth = Session("pagewidth")
                pageheight = Session("pageheight")
                If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
                If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
                Dim mimeType As String = ""
                Dim encoding As String = ""
                Dim fileNameExtension As String = ""
                Dim streams As String() = Nothing
                Dim warnings As Warning() = Nothing
                Dim deviceInf As String = "" '"<DeviceInfo>" & "  <OutputFormat>PNG</OutputFormat>"
                deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
                deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
                deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
                deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
                deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
                deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
                deviceInf = deviceInf & "</DeviceInfo>"

                Dim appldirExcelFiles, myfile As String
                'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                Dim datest, timest As String
                datest = DateString()
                timest = TimeString()
                myfile = Session("REPORTID") & "GroupsStats" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)
                appldirExcelFiles = applpath & "Temp\"
                Session("appldirExcelFiles") = appldirExcelFiles
                Session("myfile") = myfile
                Dim Bytes() As Byte = Nothing
                If Session("srd") = 4 Then
                    myfile = myfile & ".xls"
                    Bytes = viewer.LocalReport.Render("Excel")
                ElseIf Session("srd") = 5 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>WORD</OutputFormat>" & deviceInf
                    myfile = myfile & ".doc"
                    Bytes = viewer.LocalReport.Render("Word", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)
                ElseIf Session("srd") = 6 Then
                    deviceInf = "<DeviceInfo>" & "  <OutputFormat>PDF</OutputFormat>" & deviceInf
                    myfile = myfile & ".pdf"
                    Bytes = viewer.LocalReport.Render("PDF", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

                End If
                If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                    Session("myfile") = myfile
                    Dim filepath As String = String.Empty
                    'scheduled report
                    If LabelRunSched.Text = "yes" AndAlso Request("schedid") IsNot Nothing AndAlso IsNumeric(Request("schedid").ToString.Trim) Then
                        'Dim fileuploadDir As String = ConfigurationManager.AppSettings("fileupload").ToString
                        filepath = applpath & myfile
                        'filepath = filepath.Replace("\\", "\")
                        Using Stream As New FileStream(filepath, FileMode.Create)
                            Stream.Write(Bytes, 0, Bytes.Length)
                            Stream.Close()
                            Stream.Dispose()
                        End Using
                        Try
                            Dim sqlu As String = "UPDATE ourscheduledreports SET [Status]='processed', Prop2='" & filepath.Replace("\", "*") & "' WHERE ID=" & Request("schedid").ToString.Trim
                            ret = ExequteSQLquery(sqlu)
                            Session("nruntimes") = Session("nruntimes") + 1
                        Catch ex As Exception
                            If Not ex.Message.StartsWith("Thread ") Then
                                ret = "ERROR!!  " & ex.Message
                            End If
                        End Try

                        Response.Redirect("RunScheduledReports.aspx")
                    Else
                        'not scheduled report
                        filepath = Session("appldirExcelFiles") & Session("myfile")
                        filepath = filepath.Replace("/", "\").Replace("\\", "\")
                    End If
                    Using Stream As New FileStream(Session("appldirExcelFiles") & Session("myfile"), FileMode.Create)
                        Stream.Write(Bytes, 0, Bytes.Length)
                        Stream.Close()
                        Stream.Dispose()
                    End Using
                    Try
                        Response.ContentType = "application/octet-stream"
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                        Response.TransmitFile(Session("appldirExcelFiles") & Session("myfile"))
                    Catch ex As Exception
                        If Not ex.Message.StartsWith("Thread ") Then
                            ret = "ERROR!!  " & ex.Message
                        End If

                    End Try
                    Response.End()
                End If
            End If

        Catch ex As Exception
            ret = ex.Message
            If Not ret.StartsWith("Thread ") Then
                MessageBox.Show("ERROR!! " & ret, "Error!", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                'Response.Redirect("ShowReport.aspx?srd=0")
            End If
        End Try
        Return ret
    End Function
    Private Sub LinkButtonReverse_Click(sender As Object, e As EventArgs) Handles LinkButtonReverse.Click
        Dim selval As String = DropDownList2.SelectedValue
        DropDownList2.SelectedValue = DropDownList1.SelectedValue
        DropDownList1.SelectedValue = selval
        Dim gr As String
        If IsPostBack AndAlso (Session("See") = "Graph" OrElse Session("See") = "yes") AndAlso Session("GraphType") = "bar" Then
            gr = SeeGraph("bar")  'bar
        ElseIf IsPostBack AndAlso (Session("See") = "Matrix" OrElse Session("See") = "Graph" OrElse Session("See") = "yes") AndAlso Session("GraphType") = "matrix" Then
            gr = SeeMatrix()

        ElseIf IsPostBack AndAlso (Session("See") = "Pie" OrElse Session("See") = "yes") AndAlso Session("GraphType") = "pie" Then
            gr = SeeGraph("pie")

        ElseIf IsPostBack AndAlso (Session("See") = "Line" OrElse Session("See") = "yes") AndAlso Session("GraphType") = "line" Then
            gr = SeeGraph("line")
        ElseIf IsPostBack AndAlso (Session("See") = "Details" OrElse Session("See") = "yes") AndAlso Session("GraphType") = "Details" Then
            gr = SeeDynamicReport(DropDownList1.SelectedValue, DropDownList2.SelectedValue, DropDownList3.SelectedValue)
        ElseIf IsPostBack AndAlso (Session("See") = "GroupsStats" OrElse Session("See") = "yes") AndAlso Session("GraphType") = "GroupsStats" Then
            gr = SeeDynamicReportForGroups()
        End If
    End Sub

    Private Sub DropDownList1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList1.SelectedIndexChanged
        Session("cat1") = DropDownList1.SelectedValue
        Session("addwhere") = ""
        LabelAddWhere.Text = Session("addwhere").ToString
        If LabelAddWhere.Text.Trim = "" Then LabelAddWhere.Text = "<=>"
        If Session("WhereText") Is Nothing Then Session("WhereText") = " "
    End Sub
    Private Sub DropDownList2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList2.SelectedIndexChanged
        Session("cat2") = DropDownList2.SelectedValue
        Session("addwhere") = ""
        LabelAddWhere.Text = Session("addwhere").ToString
        If LabelAddWhere.Text.Trim = "" Then LabelAddWhere.Text = "<=>"
        If Session("WhereText") Is Nothing Then Session("WhereText") = " "
    End Sub

    Private Sub DropDownList4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList4.SelectedIndexChanged
        Session("Aggregate") = DropDownList4.SelectedValue
    End Sub

    Private Sub viewer_Load(sender As Object, e As EventArgs) Handles viewer.Load
        Try

        Catch ex As Exception
            If Not ex.Message.StartsWith("Thread ") Then
                Dim ret As String = ex.Message
            End If

        End Try
    End Sub

    Private Sub viewer_PreRender(sender As Object, e As EventArgs) Handles viewer.PreRender
        Try

        Catch ex As Exception
            If Not ex.Message.StartsWith("Thread ") Then
                Dim ret As String = ex.Message
            End If

        End Try
    End Sub

    Private Sub btnShare_Click(sender As Object, e As EventArgs) Handles btnShare.Click
        If cleanText(txtShareEmail.Text) <> txtShareEmail.Text.Trim Then
            txtShareEmail.Text = "Illegal character found. Please retype Email."
            Exit Sub
        End If
        Dim lgn As String = GetReportIdentifier(Session("REPORTID"))
        Dim webour As String = ConfigurationManager.AppSettings("weboureports").ToString.Trim
        Dim cntus As String = webour & "ContactUs.aspx"
        'Dim lnk As String = webour & "default.aspx?srd=3&map=yes&rep=" & repid & "&lgn=" & lgn
        Dim lnk As String = webour & "default.aspx?srd=6&shared=yes&rep=" & repid & "&lgn=" & lgn & "&keepit=yes"
        Dim ret As String = String.Empty
        Dim emailbody As String = String.Empty
        emailbody = "Please click at " & lnk & " to see up to date report shared with you.  | Access to the report is available only today.  Please be patient, it might take longer to complete big reports. | | Do not answer to this email. | | Feel free to contact us at " & cntus & " if you have any questions. | OUReports"
        emailbody = emailbody.Replace("|", Chr(10))
        ret = SendHTMLEmail("", "Up To Date Report is shared with you", emailbody, txtShareEmail.Text, Session("SupportEmail"))
        WriteToAccessLog(Session("logon"), "Sent Email to " & txtShareEmail.Text & " with body " & emailbody & ". Result: " & ret, 1)
        MessageBox.Show(ret, "Report share", "ReportShared", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        LabelShare.Text = "The link to the report: " & lnk & " . Everybody who has the link will be able to see it.<br/> Send report link to email address:"

    End Sub

    Private Sub CheckBoxHideDuplicates_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxHideDuplicates.CheckedChanged
        'hide or unhide duplicates
        Dim ret As String = String.Empty
        dv3 = RetrieveReportData(repid, WhereText, CheckBoxHideDuplicates.Checked, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret)
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
        Else
            If dv3 Is Nothing OrElse dv3.Count = 0 Then
                LabelRowCount.Text = "Records returned: " & 0.ToString
                'Exit Sub
            Else
                LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString
                dv3.RowFilter = ""
            End If
        End If
        Dim dtf As New DataTable
        dtf = dv3.Table
        dtf = MakeDTColumnsNamesCLScompliant(dtf, Session("UserConnProvider"), ret)
        dv3 = dtf.DefaultView
        Session("dv3") = dv3

        Dim gr As String = SeeReport()

    End Sub

    Private Sub ButtonCharts_Click(sender As Object, e As EventArgs) Handles ButtonCharts.Click
        Dim fn As String = DropDownList4.SelectedValue
        Dim primX As String = DropDownList1.SelectedValue
        Dim secX As String = DropDownList2.SelectedValue
        Dim valueY As String = DropDownList3.SelectedValue
        If primX = secX Then
            Session("AxisXM") = primX
        Else
            Session("AxisXM") = primX & "," & secX
        End If
        Session("AxisYM") = valueY
        Session("fnM") = fn
        Session("AggregateM") = fn
        Session("cat1") = primX
        Session("cat2") = secX
        Session("AxisY") = valueY
        Session("Aggregate") = fn
        Session("MFld") = " "
        Session("SELECTEDValuesM") = " "
        Session("MapChart") = ""
        Session("MatrixChart") = ""
        Dim er As String = String.Empty
        Dim rt As String = String.Empty
        If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
        Response.Redirect("ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & primX & "&x2=" & secX & "&y1=" & valueY & "&fn=" & fn)
    End Sub

    Private Sub ButtonDashbords_Click(sender As Object, e As EventArgs) Handles ButtonDashbords.Click
        Dim fn As String = DropDownList4.SelectedValue
        Dim primX As String = DropDownList1.SelectedValue
        Dim secX As String = DropDownList2.SelectedValue
        Dim valueY As String = DropDownList3.SelectedValue
        Dim er As String = String.Empty
        Dim rt As String = String.Empty
        If DropDownList1.SelectedValue <> DropDownList2.SelectedValue Then
            rt = AddGroupBy(Session("REPORTID"), DropDownList1.SelectedValue, DropDownList2.SelectedValue, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
        End If
        Response.Redirect("ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & primX & "&x2=" & secX & "&y1=" & valueY & "&fn=" & fn)
    End Sub
    Private Sub chkboxNumeric_CheckedChanged(sender As Object, e As EventArgs) Handles chkboxNumeric.CheckedChanged
        DropDownList4.Items.Clear()
        DropDownList4.Items.Add("Count")
        DropDownList4.Items.Add("CountDistinct")
        If chkboxNumeric.Checked Then
            DropDownList4.Items.Add("Sum")
            DropDownList4.Items.Add("Max")
            DropDownList4.Items.Add("Min")
            DropDownList4.Items.Add("Avg")
            DropDownList4.Items.Add("StDev")
            DropDownList4.Items.Add("Value")
            Dim er As String = String.Empty
            dv3 = CorrectColumnAsNumeric(dv3.Table, er, DropDownList3.Text, True).DefaultView
            Session("dv3") = dv3
        End If
        Session("nv") = DropDownList4.Items.Count
    End Sub
    Private Sub ButtonTicket_Click(sender As Object, e As EventArgs) Handles ButtonTicket.Click
        'make repot pdf file in temp dir with timestamp and send the link to Helpdesk
        Try
            Dim appldirSavedFiles, myfile, ret As String
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datest, timest As String
            datest = DateString()
            timest = TimeString()
            myfile = Session("REPORTID") & "Details" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)
            appldirSavedFiles = applpath & "SAVEDFILES\"
            Session("appldirSavedFiles") = appldirSavedFiles
            Session("myfile") = myfile & ".pdf"
            Dim Bytes() As Byte = Nothing
            Bytes = viewer.LocalReport.Render("PDF")
            If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
                Using Stream As New FileStream(Session("appldirSavedFiles") & Session("myfile"), FileMode.Create)
                    Stream.Write(Bytes, 0, Bytes.Length)
                    Stream.Close()
                    Stream.Dispose()
                End Using
                Try
                    Response.ContentType = "application/octet-stream"
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" & Session("myfile"))
                    Response.TransmitFile(Session("appldirSavedFiles") & Session("myfile"))
                Catch ex As Exception
                    ret = "ERROR!!  " & ex.Message
                    Session("appldirSavedFiles") = ""
                    Session("myfile") = ""
                End Try
            Else
                Session("appldirSavedFiles") = ""
                Session("myfile") = ""
            End If
        Catch ex As Exception
            Session("appldirSavedFiles") = ""
            Session("myfile") = ""
        End Try
        'Send to HelpDesk
        Dim tnum As Integer = DropDownTickets.SelectedValue
        If tnum = 0 Then
            Response.Redirect("HelpDesk.aspx?maketicket=yes&repid=" & Session("REPORTID").ToString & "&saved=" & Session("appldirSavedFiles") & Session("myfile"))
        Else
            Response.Redirect("HelpDesk.aspx?attachrep=yes&tn=" & tnum.ToString & "&repid=" & Session("REPORTID").ToString & "&saved=" & Session("appldirSavedFiles") & Session("myfile"))
        End If
    End Sub
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
        Response.Redirect("ReportViews.aspx?see=yes")
    End Sub
    Private Sub lnkChatAI_Click(sender As Object, e As EventArgs) Handles lnkChatAI.Click
        'Dim ret As String = String.Empty
        Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups "
        Session("grpstats") = Request("grpstats")
        If Request("grpstats") = "yes" Then
            'Session("OriginalDataTable") = dv3.Table  'already done
            Session("dataTable") = Session("OriginalDataTable")
            'Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))   'already done
            Dim dtgrp As New DataTable
            dtgrp = GetReportGroups(Session("REPORTID")) 'groups table

            Dim grpsnames As String = String.Empty
            If dtgrp IsNot Nothing AndAlso dtgrp.Rows.Count > 0 Then
                Dim i As Integer
                For i = 0 To dtgrp.Rows.Count - 1
                    If Not grpsnames.Contains(Chr(9) & dtgrp.Rows(i)("GroupField")) Then
                        grpsnames = grpsnames & Chr(9) & dtgrp.Rows(i)("GroupField")
                    End If
                Next
            End If
            Session("dataGroups") = dtgrp
            If Session("dataGroups") IsNot Nothing Then
                Session("QuestionToAI") = "Interpret the data and make meaningful analytical reports studying trends, correlations, etc...  providing comparison between groups " & grpsnames
                'Session("DataToChatAI") = " Data: " & Chr(9) & Session("DataToChatAI")  'already done
            End If
        End If
        Response.Redirect("~/ChatAI.aspx?qu=yes")
    End Sub

    Private Sub lnkImage_Click(sender As Object, e As EventArgs) Handles lnkImage.Click
        'Render the report as an image
        Dim myfile As String = String.Empty
        Dim ret As String = String.Empty
        Dim Bytes() As Byte = Nothing

        pagewidth = Piece(hdnReportDimensions.Value, "~", 1)
        pageheight = Piece(hdnReportDimensions.Value, "~", 2)



        If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
        If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
        Dim mimeType As String = ""
        Dim encoding As String = ""
        Dim fileNameExtension As String = ""
        Dim streams As String() = Nothing
        Dim warnings As Warning() = Nothing
        Dim deviceInf As String = "<DeviceInfo>" & "  <OutputFormat>PNG</OutputFormat>"
        deviceInf = deviceInf & "  <StartPage>" & viewer.CurrentPage.ToString & "</StartPage>"
        deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
        deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
        deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
        deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
        deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
        deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
        deviceInf = deviceInf & "</DeviceInfo>"


        Bytes = viewer.LocalReport.Render("Image", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

        If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
            'for testing save in file
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datest, timest As String
            datest = DateString()
            timest = TimeString()
            myfile = Session("REPORTID") & "Image" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)

            Using Stream As New FileStream(applpath & "Temp\" & myfile & ".png", FileMode.Create)
                Stream.Write(Bytes, 0, Bytes.Length)
                Stream.Close()
                Stream.Dispose()
            End Using
            Try
                Response.ContentType = "image/PNG"    '"application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile & ".png")
                Response.TransmitFile(applpath & "Temp\" & myfile & ".png")
                Response.End()
            Catch ex As Exception
                If Not ex.Message.StartsWith("Thread") Then ret = "ERROR!!  " & ex.Message
            End Try
        End If

    End Sub

    Private Sub lnkPDF_Click(sender As Object, e As EventArgs) Handles lnkPDF.Click
        'Render the report as an image
        Dim myfile As String = String.Empty
        Dim ret As String = String.Empty
        Dim Bytes() As Byte = Nothing

        pagewidth = Session("pagewidth")
        pageheight = Session("pageheight")
        Dim drep As DataTable = GetReportInfo(Session("REPORTID").ToString)
        If drep(0)("Param9type") = "landscape" Then
            If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "11in"
            If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "8.5in"
        Else
            If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then pagewidth = "8.5in"
            If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
        End If

        Dim mimeType As String = ""
        Dim encoding As String = ""
        Dim fileNameExtension As String = ""
        Dim streams As String() = Nothing
        Dim warnings As Warning() = Nothing
        Dim deviceInf As String = "<DeviceInfo>" & "  <OutputFormat>PDF</OutputFormat>"
        deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
        deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
        deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
        deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
        deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
        deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
        deviceInf = deviceInf & "</DeviceInfo>"


        Bytes = viewer.LocalReport.Render("PDF", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

        If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
            'for testing save in file
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datest, timest As String
            datest = DateString()
            timest = TimeString()
            myfile = Session("REPORTID") & "PDF" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)

            Dim tempPDF As String = applpath & "Temp\" & myfile & ".pdf"
            Using Stream As New FileStream(tempPDF, FileMode.Create)
                Stream.Write(Bytes, 0, Bytes.Length)
                Stream.Close()
                Stream.Dispose()
            End Using
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile & ".pdf")
                Response.TransmitFile(applpath & "Temp\" & myfile & ".pdf")
                Response.End()
            Catch ex As Exception
                'If Not ex.Message.StartsWith("Thread") Then ret = "ERROR!!  " & ex.Message
            End Try
        End If
    End Sub

    Private Sub lnkWord_Click(sender As Object, e As EventArgs) Handles lnkWord.Click
        'Render the report as an image
        Dim myfile As String = String.Empty
        Dim ret As String = String.Empty
        Dim Bytes() As Byte = Nothing

        pagewidth = Session("pagewidth")
        pageheight = Session("pageheight")
        Dim drep As DataTable = GetReportInfo(Session("REPORTID").ToString)
        If drep(0)("Param9type") = "landscape" Then
            If pagewidth Is Nothing OrElse pagewidth.Trim = "" OrElse CInt(pagewidth.Replace("in", "")) > 11 Then pagewidth = "11in"
            If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "8.5in"
        Else
            If pagewidth Is Nothing OrElse pagewidth.Trim = "" Then
                pagewidth = "8.5in"
            ElseIf CInt(pagewidth.Replace("in", "")) > 11 Then
                pagewidth = "11in"
            End If
            If pageheight Is Nothing OrElse pageheight.Trim = "" Then pageheight = "11in"
        End If

        Dim mimeType As String = ""
        Dim encoding As String = ""
        Dim fileNameExtension As String = ""
        Dim streams As String() = Nothing
        Dim warnings As Warning() = Nothing
        Dim deviceInf As String = "<DeviceInfo>" & "  <OutputFormat>WORD</OutputFormat>"
        deviceInf = deviceInf & "  <PageWidth>" & pagewidth & "</PageWidth>"       ' Set the page width
        deviceInf = deviceInf & "  <PageHeight>" & pageheight & "</PageHeight>"    ' Set the page height
        deviceInf = deviceInf & "  <MarginTop>0in</MarginTop>"
        deviceInf = deviceInf & "  <MarginLeft>0in</MarginLeft>"
        deviceInf = deviceInf & "  <MarginRight>0in</MarginRight>"
        deviceInf = deviceInf & "  <MarginBottom>0in</MarginBottom>"
        deviceInf = deviceInf & "</DeviceInfo>"


        Bytes = viewer.LocalReport.Render("Word", deviceInf, mimeType, encoding, fileNameExtension, streams, warnings)

        If Not Bytes Is Nothing AndAlso Bytes.Length > 0 Then
            'for testing save in file
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim datest, timest As String
            datest = DateString()
            timest = TimeString()
            myfile = Session("REPORTID") & "WORD" & Session("logon").ToString & "_" & Mid(datest, 7, 4) & Mid(datest, 1, 2) & Mid(datest, 4, 2) & Mid(timest, 1, 2) & Mid(timest, 4, 2) & Mid(timest, 7, 2)

            Using Stream As New FileStream(applpath & "Temp\" & myfile & ".doc", FileMode.Create)
                Stream.Write(Bytes, 0, Bytes.Length)
                Stream.Close()
                Stream.Dispose()
            End Using
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile & ".doc")
                Response.TransmitFile(applpath & "Temp\" & myfile & ".doc")
                Response.End()
            Catch ex As Exception
                'If Not ex.Message.StartsWith("Thread") Then ret = "ERROR!!  " & ex.Message
            End Try
        End If
    End Sub

    Private Sub lnkExpandAll_Click(sender As Object, e As EventArgs) Handles lnkExpandAll.Click
        lnkExpandAll.Visible = False
        lnkExpandAll.Enabled = False
        Dim ret As String = SeeDynamicReportForGroups(Nothing, True)
    End Sub


    'Public Sub ConvertHtmlToPng(htmlContent As String, ByVal imageOutputDirectory As String, ByVal imageFileName As String)
    '    ' Create a PDF document
    '    Dim pdfDocument As New PdfSharp.Pdf.PdfDocument()

    '    ' Add a page to the document
    '    Dim pdfPage As PdfSharp.Pdf.PdfPage = pdfDocument.AddPage()

    '    ' Create an XGraphics object for drawing on the page
    '    Using graphics As XGraphics = XGraphics.FromPdfPage(pdfPage)
    '        ' Render HTML to the graphics object
    '        'HtmlRenderer.PdfSharp.HtmlRenderer.Render(graphics, htmlContent)
    '        pdfDocument = PdfGenerator.GeneratePdf(htmlContent, PdfSharp.PageSize.A4)
    '        ' Define a temporary file path for the PDF
    '        Dim tempPdfPath As String = Path.Combine(imageOutputDirectory, imageFileName & ".pdf")
    '        pdfDocument.Save(tempPdfPath)

    '        '' Load the PDF page as an image
    '        'Using pdfImage As Image = RenderPdfPageToImage(tempPdfPath, 0)
    '        '    ' Save the image as a PNG file
    '        '    pdfImage.Save(outputFilePath, ImageFormat.Png)
    '        'End Using

    '        ' Convert the PDF to PNG
    '        Using pdfDocumentLoaded As PdfiumViewer.PdfDocument = PdfiumViewer.PdfDocument.Load(tempPdfPath)
    '            Using image As Image = pdfDocumentLoaded.Render(0, 300, 300, True)
    '                ' Save the image as a PNG file
    '                image.Save(outputFilePath, ImageFormat.Png)
    '            End Using
    '        End Using

    '        ' Delete the temporary PDF file
    '        File.Delete(tempPdfPath)
    '    End Using
    'End Sub
    'Public Function ConvertWordToImage(ByVal wordFilePath As String, ByVal imageOutputDirectory As String, ByVal imageFileName As String) As String
    '    Dim ret As String = String.Empty
    '    Dim outputFilePath As String = String.Empty
    '    Try
    '        ' Load the Word document
    '        Dim doc As New Document(wordFilePath)

    '        ' Ensure the output directory exists
    '        If Not Directory.Exists(imageOutputDirectory) Then
    '            Directory.CreateDirectory(imageOutputDirectory)
    '        End If

    '        ' Loop through each page in the Word document
    '        For pageIndex As Integer = 0 To doc.PageCount - 1
    '            ' Define the image options
    '            Dim imageOptions As New ImageSaveOptions(SaveFormat.Png)
    '            imageOptions.PageSet = New PageSet(pageIndex)

    '            ' Define the output file path
    '            outputFilePath = imageOutputDirectory & imageFileName

    '            ' Save the page as an image
    '            doc.Save(outputFilePath, imageOptions)
    '        Next
    '        Return outputFilePath
    '    Catch ex As Exception
    '        ret = "ERROR! " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Function ConvertToImageOpenXML(ByVal wordFilePath As String, ByVal imageOutputDirectory As String, ByVal imageFileName As String) As String
    '    Dim ret As String = String.Empty
    '    Dim outputFilePath As String = String.Empty
    '    Try
    '        ' Load the Word document
    '        Dim doc As New Document(wordFilePath)

    '        ' Ensure the output directory exists
    '        If Not Directory.Exists(imageOutputDirectory) Then
    '            Directory.CreateDirectory(imageOutputDirectory)
    '        End If

    '        ' Loop through each page in the Word document
    '        For pageIndex As Integer = 0 To doc.PageCount - 1
    '            ' Define the image options
    '            Dim imageOptions As New ImageSaveOptions(SaveFormat.Png)
    '            imageOptions.PageSet = New PageSet(pageIndex)

    '            ' Define the output file path
    '            outputFilePath = imageOutputDirectory & imageFileName

    '            ' Save the page as an image
    '            doc.Save(outputFilePath, imageOptions)
    '        Next
    '        Return outputFilePath
    '    Catch ex As Exception
    '        ret = "ERROR! " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    'Public Sub ConvertHtmlToPng(htmlContent As String, outputFilePath As String)
    '    Using img As Image = HtmlRender.RenderToImage(htmlContent, New Size(800, 600))
    '        img.Save(outputFilePath, ImageFormat.Png)
    '    End Using
    'End Sub
    'Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
    '    ' Create a StringWriter to capture the rendered HTML
    '    Using stringWriter As New StringWriter()
    '        Using htmlWriter As New HtmlTextWriter(stringWriter)
    '            ' Render the page content to the HtmlTextWriter
    '            MyBase.Render(htmlWriter)
    '            ' Get the HTML content as a string
    '            Dim htmlContent As String = stringWriter.ToString()
    '            ' Write the original HTML content back to the response
    '            writer.Write(htmlContent)
    '            If Not IsPostBack AndAlso Request("grpstats") = "yes" Then
    '                Session("DataToChatAIGroups") = htmlContent
    '            End If
    '        End Using
    '    End Using
    'End Sub
End Class


