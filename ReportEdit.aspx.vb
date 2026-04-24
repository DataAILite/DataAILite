Imports System.Configuration
Imports System.Web.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.IO.Compression
Imports System.Net
Imports System.Math
Imports AjaxControlToolkit

Partial Class ReportEdit
    Inherits System.Web.UI.Page
    Dim dvp As DataView
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Not Page.IsPostBack AndAlso Not Request("tne") Is Nothing AndAlso (Session("TabN") Is Nothing OrElse Session("TabN") <> Request("tne")) Then
            Session("TabN") = CInt(Request("tne"))
            lblView.Text = MenuMain.Items(Session("TabN")).Text
        End If

        If Session("OurConnProvider").ToString.Trim = "Sqlite" Then
            'make tab Share invisible
            'Tab5.Visible = False
            MenuMain.Items(4).Enabled = False
            MenuMain.Items(4).Text = ""

        Else
            'Tab5.Visible = True
            MenuMain.Items(4).Enabled = True
            MenuMain.Items(4).Text = "Users"
        End If

        If Session("TableFriendlyName") Is Nothing Then
            Session("TableFriendlyName") = ""
        End If
        ButtonUploadRDL.OnClientClick = "showSpinner();"
        ButtonUploadFile.OnClientClick = "showSpinner();"

        Session("addwhere") = Nothing
        Dim ErrorLog = String.Empty
        Try
            Dim ctlID As String = Me.ClientID & "_"
            btnBrowse.OnClientClick = "clickFileUpload();return false;"
            FileRDL.Attributes.Add("onchange", "getAttachedFile();")
            Dim repedit As String = String.Empty
            Dim dv1 As DataView
            LabelAlert.Text = " "
            If Session("REPORTID") Is Nothing OrElse Session("REPORTID").ToString = "" Then
                repid = Request("REPORT")
                repedit = Request("repedit")
                Session("REPORTID") = repid
                If Request("openmap") IsNot Nothing AndAlso Request("openmap").ToString.Trim = "yes" Then
                    Session("openmap") = "yes"
                Else
                    Session("openmap") = ""
                End If
            ElseIf Session("REPORTID") IsNot Nothing AndAlso Session("REPORTID").ToString <> "" Then
                repid = Session("REPORTID")
                repedit = "yes"
            End If

            If Request("repedit") = "new" AndAlso Session("CSV") = "yes" Then
                trEditRep3.Visible = False
                trEditRep1.Visible = False
                trEditRep5.Visible = False
                ButtonUploadRDL.Visible = False
                ButtonUploadRDL.Enabled = False
                ButtonDownloadFile.Visible = False
                ButtonDownloadFile.Enabled = False
                ButtonUpdateDashboards.Visible = False
                ButtonUpdateDashboards.Enabled = False

            ElseIf Request("repedit") = "new" AndAlso Session("CSV") <> "yes" Then
                trEditRep3.Visible = True
                trEditRep1.Visible = True
                trEditRep5.Visible = True
                ButtonUploadRDL.Visible = True
                ButtonUploadRDL.Enabled = True
                ButtonDownloadFile.Visible = False
                ButtonDownloadFile.Enabled = False
                ButtonUpdateDashboards.Visible = True
                ButtonUpdateDashboards.Enabled = True

                Session("TabNQ") = 0
                repedit = "yes"
                Session("REPEDIT") = "yes"
                Response.Redirect("~/SQLquery.aspx")
            End If

            'del rep file
            If Request("DELREP") = "yes" Then
                'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
                Dim rpath As String = String.Empty
                Dim rfile As String = String.Empty
                If Request("REPTYPE") = "rpt" Then
                    rpath = applpath & "RPTFILES\"
                    rfile = rpath & repid & ".rpt"
                    'Session("ReportType") = "crystal"
                ElseIf Request("REPTYPE") = "rdl" Then
                    rpath = applpath & "RDLFILES\"
                    rfile = rpath & repid & ".rdl"
                    'Session("ReportType") = "rdl"
                End If
                Try
                    If File.Exists(rfile) Then
                        File.Delete(rfile)
                        TextBoxReportFile.Text = String.Empty
                        Session("FileJustDeleted") = True
                        ErrorLog = " "
                    Else
                        ErrorLog = "  "
                    End If
                Catch ex As Exception
                    ErrorLog = ex.Message
                End Try
                repedit = "yes"
                Response.Redirect("~/ReportEdit.aspx?repedit=yes&DELREP=no&REPORT=" & repid)
            End If
            'edit
            Session("REPEDIT") = repedit

            'draw report information
            If repedit = "yes" Then
                LabelEditOrNew.Text = "Edit"

                'TODO repdb !!!
                'Report Info (title, data for report)

                dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & repid & "')")

                If dv1 IsNot Nothing AndAlso dv1.Table.Rows.Count > 0 Then
                    Session("ParamsRelated") = dv1.Table.Rows(0)("Param0id").ToString
                    If Session("ParamsRelated") = "yes" Then
                        chkRelatedParameters.Checked = True
                    Else
                        chkRelatedParameters.Checked = False
                    End If
                    Session("noedit") = dv1.Table.Rows(0)("Param7type").ToString
                    If Session("noedit") = "standard" Then
                        'hide all links to report editing pages leaving only reporting
                        Dim treenodecr1 As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=2")
                        If Not treenodecr1 Is Nothing Then
                            treenodecr1.Value = "ReportViews.aspx"
                            treenodecr1.ToolTip = "disabled for locked reports"
                        End If
                        Dim treenodecr11 As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=3")
                        If Not treenodecr11 Is Nothing Then
                            treenodecr11.Value = "ReportViews.aspx"
                            treenodecr11.ToolTip = "disabled for locked reports"
                        End If
                        Dim treenodecr12 As WebControls.TreeNode = TreeView1.FindNode("ReportEdit.aspx?tne=4")
                        If Not treenodecr12 Is Nothing Then
                            treenodecr12.Value = "ReportViews.aspx"
                            treenodecr12.ToolTip = "disabled for locked reports"
                        End If
                        Dim treenodecr2 As WebControls.TreeNode = TreeView1.FindNode("SQLquery.aspx?tnq=0")
                        If Not treenodecr2 Is Nothing Then
                            treenodecr2.CollapseAll()
                            treenodecr2.Expanded = False
                            treenodecr2.ChildNodes.Clear()
                            treenodecr2.Value = "ReportViews.aspx"
                            treenodecr2.ToolTip = "disabled for locked reports"
                        End If
                        Dim treenodecr3 As WebControls.TreeNode = TreeView1.FindNode("RDLformat.aspx?tnf=0")
                        If Not treenodecr3 Is Nothing Then
                            treenodecr3.CollapseAll()
                            treenodecr3.Expanded = False
                            treenodecr3.ChildNodes.Clear()
                            treenodecr3.Value = "ReportViews.aspx"
                            treenodecr3.ToolTip = "disabled for locked reports"
                        End If

                    End If
                    TextBoxReportID.Text = repid
                    TextBoxReportID.Visible = False
                    LabelReportID.Text = repid
                    TextBoxPageFtr.Text = dv1.Table.Rows(0)("Comments").ToString
                    Session("PageFtr") = TextBoxPageFtr.Text
                    TextBoxReportTitle.Text = dv1.Table.Rows(0)("ReportTtl").ToString.Replace(":", "-")
                    If TextBoxReportTitle.Text.Trim = "" Then
                        TextBoxReportTitle.Text = dv1.Table.Rows(0)("ReportName").ToString  'for old reports
                    End If
                    Session("REPTITLE") = dv1.Table.Rows(0)("ReportTtl").ToString
                    ' LabelAlert.Text = "Report:   " & Session("REPTITLE") & " ,      ReportId:  " & Session("REPORTID")
                    LabelAlert1.Text = Session("REPTITLE")
                    LabelRepID.Text = Session("REPORTID")
                    DropDownReportType.Text = dv1.Table.Rows(0)("ReportType").ToString
                    Session("ReportType") = dv1.Table.Rows(0)("ReportType").ToString
                    If dv1.Table.Rows(0)("ReportType").ToString = "" Then
                        dv1.Table.Rows(0)("ReportType") = "rdl"
                    End If
                    DropDownReportType.SelectedValue = dv1.Table.Rows(0)("ReportType").ToString
                    If dv1.Table.Rows(0)("Param9Type").ToString = "" Then
                        dv1.Table.Rows(0)("Param9Type") = "portrait"
                    End If
                    DropDownOrientation.SelectedValue = dv1.Table.Rows(0)("Param9Type").ToString
                    Session("Orientation") = DropDownOrientation.SelectedValue
                    DropDownReportAttributes.Text = dv1.Table.Rows(0)("ReportAttributes").ToString
                    If dv1.Table.Rows(0)("SQLquerytext").ToString = "" OrElse dv1.Table.Rows(0)("SQLquerytext").ToString.ToUpper.StartsWith("SELECT ") OrElse Session("UserConnProvider") = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
                        DropDownReportAttributes.Text = "sql"
                        Session("SQLquerytext") = dv1.Table.Rows(0)("SQLquerytext").ToString
                        DropDownReportAttributes.SelectedValue = "sql"
                    Else
                        DropDownReportAttributes.Text = "sp"
                    End If
                    If Session("UserConnProvider") <> "System.Data.Odbc" AndAlso Session("UserConnProvider") <> "System.Data.OleDb" Then DropDownReportAttributes.SelectedValue = dv1.Table.Rows(0)("ReportAttributes").ToString
                    Session("Attributes") = dv1.Table.Rows(0)("ReportAttributes").ToString
                    If Session("UserConnProvider").ToString.Trim = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
                        trEditRep2.Visible = False
                    End If
                    If Session("Attributes") = "sp" AndAlso Not IsPostBack Then
                        'MenuMain.Items(0).Enabled = False
                        ''remove left menu node for Report Data Query
                        'Dim treenodedata As WebControls.TreeNode = TreeView1.FindNode("repdata")
                        'If treenodedata IsNot Nothing Then _
                        'TreeView1.Nodes.Remove(treenodedata)
                        chkRelatedParameters.Checked = False
                        chkRelatedParameters.Visible = False
                        chkRelatedParameters.Enabled = False
                        DropDownSPs.Visible = True
                        DropDownSPs.Enabled = True
                        LabelSP.Visible = True
                        Dim ddt As DataTable
                        Dim dv As DataView = Nothing
                        dv = GetListOfStoredProcedures(Session("UserConnString"), Session("UserConnProvider"))
                        If dv IsNot Nothing Then
                            ddt = dv.Table
                            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data") Then
                                Dim id As String = ""
                                Dim cls As String = ""
                                Dim sp As String = ""
                                Dim sql As String = ""
                                Dim li As ListItem = Nothing
                                Dim dtRoutine As DataTable = Nothing
                                Dim err As String = ""
                                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                                    id = ddt.Rows(i)("ROUTINE_NAME")
                                    cls = Piece(id, "||", 1)
                                    sp = Piece(id, "||", 2)
                                    sql = "SELECT ROUTINE_SCHEMA,ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE CLASSNAME = '" & cls & "' AND METHOD_OR_QUERY_NAME = '" & sp & "'"
                                    dtRoutine = mRecords(sql, err, Session("UserConnString"), Session("UserConnProvider")).Table
                                    If dtRoutine IsNot Nothing AndAlso dtRoutine.Rows.Count > 0 Then
                                        sp = dtRoutine.Rows(0)("ROUTINE_SCHEMA").ToString & "." & dtRoutine.Rows(0)("ROUTINE_NAME").ToString
                                        li = New ListItem(sp, id)
                                        DropDownSPs.Items.Add(li)
                                    End If
                                Next
                                sp = dv1.Table.Rows(0)("SQLquerytext").ToString
                                If sp.Contains("||") Then
                                    li = DropDownSPs.Items.FindByValue(sp)
                                Else
                                    li = DropDownSPs.Items.FindByText(sp)
                                End If
                                TextBoxNewSPname.Text = li.Text
                                DropDownSPs.Text = li.Text
                                DropDownSPs.SelectedValue = li.Value
                                TextBoxSQLorSPtext.Text = GetStoredProcedureText(li.Value, Session("UserConnString"), Session("UserConnProvider"))
                                TextBoxSQLorSPtext.Enabled = False
                                TextBoxNewSPname.Enabled = True
                                Session("SPname") = li.Value
                                Session("SPtext") = li.Value
                                Session("SPparameters") = GetListOfStoredProcedureParameters(li.Value, Session("UserConnString"), Session("UserConnProvider"))
                            ElseIf Session("UserConnProvider").ToString.Trim = "Oracle.ManagedDataAccess.Client" Then
                                Dim SPName As String = String.Empty
                                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                                    SPName = ddt.Rows(i)("OBJECT_NAME")
                                    'stored procedure must have a cursor
                                    If SPHasCursor(SPName, Session("UserConnString").ToString, Session("UserConnProvider").ToString) Then
                                        DropDownSPs.Items.Add(SPName)
                                    End If
                                Next
                                If DropDownSPs.Items.Count > 0 Then
                                    TextBoxNewSPname.Text = dv1.Table.Rows(0)("SQLquerytext").ToString
                                    DropDownSPs.Text = TextBoxNewSPname.Text
                                    DropDownSPs.SelectedValue = TextBoxNewSPname.Text
                                    TextBoxSQLorSPtext.Text = GetStoredProcedureText(TextBoxNewSPname.Text, Session("UserConnString"), Session("UserConnProvider"))
                                    TextBoxSQLorSPtext.Enabled = False
                                    TextBoxNewSPname.Enabled = True
                                    Session("SPname") = TextBoxNewSPname.Text
                                    Session("SPtext") = TextBoxNewSPname.Text
                                    Session("SPparameters") = GetListOfStoredProcedureParameters(TextBoxNewSPname.Text, Session("UserConnString"), Session("UserConnProvider"))
                                End If
                            ElseIf Session("UserConnProvider").ToString.Trim = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
                                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                                    If Not ddt.Rows(i)("PROCEDURE_NAME").ToString.StartsWith("%") Then
                                        DropDownSPs.Items.Add(Piece(ddt.Rows(i)("PROCEDURE_NAME"), ";", 1))
                                    End If
                                Next

                                'TODO maybe not needed: ElseIf Session("UserConnProvider").ToString.Trim = "Npgsql" Then  'PostgreSQL  Npgsql

                            Else
                                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                                    DropDownSPs.Items.Add(ddt.Rows(i)("ROUTINE_NAME"))
                                Next
                                TextBoxNewSPname.Text = dv1.Table.Rows(0)("SQLquerytext").ToString
                                DropDownSPs.Text = TextBoxNewSPname.Text
                                DropDownSPs.SelectedValue = TextBoxNewSPname.Text
                                TextBoxSQLorSPtext.Text = GetStoredProcedureText(TextBoxNewSPname.Text, Session("UserConnString"), Session("UserConnProvider"))
                                TextBoxSQLorSPtext.Enabled = False
                                TextBoxNewSPname.Enabled = True
                                Session("SPname") = TextBoxNewSPname.Text
                                Session("SPtext") = TextBoxNewSPname.Text
                                Session("SPparameters") = GetListOfStoredProcedureParameters(TextBoxNewSPname.Text, Session("UserConnString"), Session("UserConnProvider"))
                            End If
                        End If
                    ElseIf Session("Attributes") = "sql" Then
                        MenuMain.Items(0).Enabled = True
                        'Dim sQuery As String = dv1.Table.Rows(0)("SQLquerytext").ToString
                        'Dim nWhere As Integer = sQuery.ToUpper.IndexOf("WHERE")
                        'If nWhere > -1 Then
                        '    Dim WhereString As String = sQuery.Substring(nWhere, sQuery.Length - nWhere)
                        '    WhereString = WhereString.Replace("""", "'")
                        '    sQuery = sQuery.Substring(0, nWhere - 1) & WhereString
                        'End If
                        'TextBoxSQLorSPtext.Text = sQuery
                        DropDownSPs.Visible = False
                        DropDownSPs.Enabled = False
                        LabelSP.Visible = False
                        TextBoxSQLorSPtext.Enabled = True
                    End If

                    If ErrorLog = String.Empty And Session("FileJustDeleted") = False Then
                        TextBoxReportFile.Text = dv1.Table.Rows(0)("ReportFile").ToString
                    ElseIf Session("FileJustDeleted") = True Then
                        TextBoxReportFile.Text = String.Empty
                    Else
                        LabelRPT.ForeColor = Drawing.Color.DeepPink
                        LabelRPT.Text = ErrorLog
                    End If

                    main.Rows(1).Cells(0).InnerHtml = "Report ID:" & "<input type='hidden' name='REPORT' value='" & repid & "'><input type='hidden' name='REPEDIT' value='yes'>"
                Else
                    MessageBox.Show("Report error...", "Create Report Error", "CreateError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                    Response.Redirect("ListOfReports.aspx")
                End If
            Else 'repedit = "new"
                LabelEditOrNew.Text = "New"
                TextBoxReportID.Visible = True
                LabelReportID.Visible = False
                HyperLink5.NavigateUrl = "~/ReportEdit.aspx?DELREP=no&REPORT=" & repid
            End If
            If Not IsPostBack Then
                If Session("CSV") = "yes" Then
                    ButtonDownloadFile.Visible = False
                    ButtonDownloadFile.Enabled = False
                    'ButtonUploadRDL.Text = "Upload CSV/XML file"
                    'ButtonUploadRDL.ToolTip = "Upload selected CSV or XML file"
                    LabelTableName.Visible = True
                    txtTableName.Visible = True
                    txtTableName.Enabled = True
                    LabelDelimiter.Visible = True
                    TextboxDelimiter.Visible = True
                    TextboxDelimiter.Enabled = True
                    If TextboxDelimiter.Text.Trim = "" Then
                        TextboxDelimiter.Text = " "
                    End If
                    trEditRep2.Visible = False
                    If TextBoxSQLorSPtext.Text.Trim = "" OrElse TextBoxSQLorSPtext.Text.Trim = "SELECT" OrElse TextBoxSQLorSPtext.Text.ToUpper.Trim = "SELECT * FROM" Then
                        trEditRep3.Visible = False
                    Else
                        trEditRep3.Visible = True
                    End If
                    trEditRep4.Visible = False
                    DropDownTables.Enabled = True
                    DropDownTables.Visible = True
                    LabelTables.Enabled = True
                    LabelTables.Visible = True
                    'DropDownTables
                    Dim er As String = String.Empty
                    Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
                    If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                        LabelAlert1.Text = "No tables nor fields selected into report yet..."
                        LabelAlert1.Visible = True
                        Exit Sub
                    End If
                    If er <> "" Then
                        LabelAlert1.Text = er
                        LabelAlert1.Visible = True
                        Exit Sub
                    End If
                    DropDownTables.Items.Add(" ")
                    For i = 0 To ddtv.Table.Rows.Count - 1
                        Dim li As New ListItem
                        'li.Text = ddtv.Table.Rows(i)("TABLENAME").ToString & "(" & ddtv.Table.Rows(i)("TABLE_NAME").ToString & ")"
                        li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                        If IsColumnFromDataTable(ddtv.Table, "TABLENAME") AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddtv.Table.Rows(i)("TABLENAME").ToString.Trim <> ddtv.Table.Rows(i)("TABLE_NAME").ToString.Trim Then
                            li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString & "(" & ddtv.Table.Rows(i)("TABLENAME").ToString & ")"
                        End If
                        li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                        DropDownTables.Items.Add(li)
                    Next
                Else
                    'LabelDelimiter.Visible = False
                    'TextboxDelimiter.Visible = False
                    'TextboxDelimiter.Enabled = False
                    'LabelTableName.Visible = False
                    DropDownTables.Enabled = False
                    DropDownTables.Visible = False
                    LabelTables.Enabled = False
                    LabelTables.Visible = False
                    'txtTableName.Visible = False
                    'txtTableName.Enabled = False
                    DropDownTables.Enabled = True
                    DropDownTables.Visible = True
                    LabelTables.Enabled = True
                    LabelTables.Visible = True
                    'DropDownTables
                    Dim er As String = String.Empty
                    Dim ddtv As DataView = GetListOfUserTables(False, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
                    If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                        LabelAlert1.Text = "No tables found..."
                        LabelAlert1.Visible = True
                        Exit Sub
                    End If
                    If er <> "" Then
                        LabelAlert1.Text = er
                        LabelAlert1.Visible = True
                        Exit Sub
                    End If
                    DropDownTables.Items.Add(" ")
                    For i = 0 To ddtv.Table.Rows.Count - 1
                        Dim li As New ListItem
                        li.Text = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                        li.Value = ddtv.Table.Rows(i)("TABLE_NAME").ToString
                        DropDownTables.Items.Add(li)
                    Next
                End If
                If Request("msg") IsNot Nothing AndAlso Request("msg").ToString.Trim <> "" Then
                    LabelAlert.ForeColor = Color.Red
                    LabelAlert.Text = Request("msg").ToString & ":"
                    LabelAlert.Visible = True
                    HyperLinkImportData.Visible = True
                    HyperLinkImportData.Enabled = True
                    HyperLinkImportData.NavigateUrl = "DataImport.aspx?rep=" & Session("REPORTID")
                End If
            End If

            ButtonSubmit.OnClientClick = "buttonSubmitClick(); return false;"
        Catch ex As Exception
            ErrorLog = ex.Message
        End Try
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        hfSizeLimit.Value = getMaxRequestLength().ToString

        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target = "FileSizeExceeded" Then
                MessageBox.Show(data, "File Size Exceeded", target, Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
                Return
            ElseIf target = "SubmitClick" Then
                ButtonSubmit_Click(Me, EventArgs.Empty)
            End If
        End If
        Dim i As Integer
        If (Not IsPostBack) Then
            If (Not Session("AdvancedUser") Is Nothing AndAlso Session("AdvancedUser") = True) Then
                chkAdvanced.Checked = True
            Else
                chkAdvanced.Checked = False
            End If
        End If
        Dim qsql As String = "SELECT [ReportId],[Type] FROM [OURFiles] WHERE [ReportId]='" & Session("REPORTID") & "' AND [Type]='RPT'"
        If Session("Crystal") <> "ok" OrElse Not HasRecords(qsql) Then
            Dim treenodecr As WebControls.TreeNode = TreeView1.FindNode("ShowReport.aspx?srd=3")
            If treenodecr IsNot Nothing AndAlso treenodecr.ChildNodes.Count = 6 Then
                treenodecr.ChildNodes(5).NavigateUrl = ""
                treenodecr.ChildNodes(5).Text = "See report data"
                treenodecr.ChildNodes(5).Value = Nothing
                treenodecr.ChildNodes.Remove(treenodecr.ChildNodes(5))
            End If
        End If

        If chkAdvanced.Checked Then
            Session("AdvancedUser") = True
            trEditRep0.Visible = True
            If Session("CSV") = "yes" Then
                trEditRep2.Visible = False
                If TextBoxSQLorSPtext.Text.Trim = "" OrElse TextBoxSQLorSPtext.Text.Trim = "SELECT" OrElse TextBoxSQLorSPtext.Text.ToUpper.Trim = "SELECT * FROM" Then
                    trEditRep3.Visible = False
                Else
                    trEditRep3.Visible = True
                End If
            ElseIf Session("UserConnProvider").ToString.Trim = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
                trEditRep2.Visible = False
                trEditRep3.Visible = True
            Else
                trEditRep2.Visible = True
                trEditRep3.Visible = True
            End If
            trEditRep4.Visible = False
        Else
            Session("AdvancedUser") = False
            trEditRep0.Visible = False
            trEditRep2.Visible = False
            trEditRep3.Visible = False
            trEditRep4.Visible = False
        End If

        Dim ret As String = String.Empty
        Dim dv1 As DataView = mRecords("SELECT * FROM [OURReportInfo] WHERE ([ReportId]='" & repid & "')", ret)
        If dv1 Is Nothing OrElse dv1.Count = 0 OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then
            MessageBox.Show("Report error...", "Create Report Error", "CreateError", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Response.Redirect("ListOfReports.aspx")
        End If
        If Session("Attributes") = "sp" Then
            'disable Report Data tab
            MenuMain.Items(0).Enabled = False
            'remove left menu node for Report Data Query
            Dim treenodedata As WebControls.TreeNode = TreeView1.FindNode("SQLquery.aspx?tnq=0")
            If treenodedata IsNot Nothing Then _
                        TreeView1.Nodes.Remove(treenodedata)
            DropDownSPs.Visible = True
            DropDownSPs.Enabled = True
            LabelSP.Visible = True
            'Dim ddt As DataTable
            'ddt = GetListOfStoredProcedures(Session("UserConnString"), Session("UserConnProvider")).Table
            'For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            '    DropDownSPs.Items.Add(ddt.Rows(i)("ROUTINE_NAME"))
            'Next

            HyperLink3.NavigateUrl = "~/ReportEdit.aspx?REPORT=" & repid & "&REPEDIT=yes"

            chkUseSQLText.Checked = False
            chkUseSQLText.Visible = False
        Else  'If Session("Attributes") = "sql" Then
            MenuMain.Items(0).Enabled = True

            chkUseSQLText.Visible = True
            If Not IsPostBack Then
                Dim bUseSQLText As Boolean = (Not IsDBNull(dv1.Table.Rows(0)("Param8type")) AndAlso dv1.Table.Rows(0)("Param8type").ToString.ToUpper = "USESQLTEXT")
                chkUseSQLText.Checked = bUseSQLText
            End If

            Dim sQuery As String = String.Empty
            If chkUseSQLText.Checked = True Then
                sQuery = dv1.Table.Rows(0)("SQLquerytext").ToString
            Else
                sQuery = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                If sQuery = String.Empty Then sQuery = dv1.Table.Rows(0)("SQLquerytext").ToString
            End If

            Dim nWhere As Integer = sQuery.ToUpper.IndexOf("WHERE")
            If nWhere > -1 Then
                Dim WhereString As String = sQuery.Substring(nWhere, sQuery.Length - nWhere)
                WhereString = WhereString.Replace("""", "'")
                sQuery = sQuery.Substring(0, nWhere - 1) & " " & WhereString
            End If
            If TextBoxSQLorSPtext.Text.Trim = "" Then
                TextBoxSQLorSPtext.Text = sQuery
            End If
            DropDownSPs.Visible = False
            DropDownSPs.Enabled = False
            LabelSP.Visible = False
            TextBoxSQLorSPtext.Enabled = True
        End If
        If Not IsPostBack AndAlso Not IsDBNull(dv1.Table.Rows(0)("Param4id")) AndAlso dv1.Table.Rows(0)("Param4id").ToString.Trim <> "https://" AndAlso txtURI.Text = "https://" Then
            txtURI.Text = dv1.Table.Rows(0)("Param4id").ToString.Trim
        End If

        Dim reptype, repattributes As String
        reptype = Request(DropDownReportType.Text)
        If reptype <> "" Then
            DropDownReportType.Text = reptype
            DropDownReportType.SelectedValue = reptype
        End If
        repattributes = DropDownReportAttributes.Text 'data source type (sql or sp)
        If repattributes Is Nothing OrElse repattributes = "" Then
            repattributes = "sql"
        End If
        If repattributes <> "" Then
            DropDownReportAttributes.Text = repattributes
            DropDownReportAttributes.SelectedValue = repattributes
            If repattributes = "sp" Then                 'sp
                DropDownSPs.Visible = True
                DropDownSPs.Enabled = True
                LabelSP.Visible = True
                TextBoxSQLorSPtext.Enabled = False
                TextBoxSQLorSPtext.Wrap = False
                If Not (IsPostBack) Then
                    'MultiView1.ActiveViewIndex = 0
                    'MenuMain.Items(0).Selected = True
                    MenuMain.Items(0).Enabled = False
                    'If Session("REPEDIT") = "yes" Then
                    '    Session("TabN") = 2
                    'Else
                    '    Session("TabN") = 1
                    'End If
                End If
                'If Session("TabN") IsNot Nothing AndAlso Session("TabN") < MenuMain.Items.Count Then
                '    MultiView1.ActiveViewIndex = Session("TabN")
                '    MenuMain.Items(Session("TabN")).Selected = True
                'End If
            ElseIf repattributes = "sql" Then              'sql
                DropDownSPs.Visible = False
                DropDownSPs.Enabled = False
                LabelSP.Visible = False
                TextBoxSQLorSPtext.Enabled = True
                TextBoxSQLorSPtext.Wrap = True
                If Not (IsPostBack) Then
                    'MultiView1.ActiveViewIndex = 0
                    'MenuMain.Items(0).Selected = True
                    MenuMain.Items(0).Enabled = True
                    'If Session("REPEDIT") = "yes" Then
                    '    Session("TabN") = 2
                    'Else
                    '    Session("TabN") = 0
                    'End If
                End If

            End If
        End If

        'initial tab for new reports = 0 (sql query), for edited ones = 2 (report info)
        If Not (IsPostBack) AndAlso Session("TabN") IsNot Nothing AndAlso Session("TabN") = 0 Then
            If Session("REPEDIT") = "yes" Then
                If Session("openmap") = "yes" Then
                    Response.Redirect("MapReport.aspx")
                ElseIf DropDownTables.Items.Count = 1 Then  'no tables in user db, first user report 
                    Session("TabN") = 2
                Else
                    Session("TabN") = 2
                End If
            Else
                If DropDownTables.Items.Count = 1 Then  'no tables in user db, first user report 
                    Session("TabN") = 2
                Else
                    Session("TabN") = 0
                End If
            End If
        End If
        'for sp the sql query is not available
        If MenuMain.Items(0).Enabled = False AndAlso Session("TabN") IsNot Nothing AndAlso Session("TabN") = 0 Then
            Session("TabN") = 2
        End If
        If Session("TabN") IsNot Nothing AndAlso Session("TabN") < MenuMain.Items.Count Then
            MultiView1.ActiveViewIndex = Session("TabN")
            MenuMain.Items(Session("TabN")).Selected = True
            'lblView.Text = MenuMain.Items(Session("TabN")).Text
        End If

        'Dim dvp As DataView
        Dim m, s, d, SQLq, ddid As String
        Dim l, n As Integer

        'draw parameters       
        m = ""
        s = ""
        d = ""
        m = Request("m")  'edit parameter
        s = Request("s")  'show parameter
        d = Request("d")  'delete parameter

        dvp = mRecords("SELECT DropDownID, DropDownLabel, DropDownName, DropDownFieldType, DropDownSQL, comments, PrmOrder, Indx FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY PrmOrder", ret)
        'dvp = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY PrmOrder")
        n = 0
        If Not dvp Is Nothing AndAlso Not dvp.Table Is Nothing Then
            n = dvp.Table.Rows.Count   'how many parameters (drop-downs)
        End If

        'LabelNofParams.Text = n.ToString
        Dim ctl As LinkButton = Nothing

        lblParamNo.Text = n.ToString
        Dim sql As String = String.Empty

        If n > 0 Then
            For i = 0 To n - 1
                ddid = dvp.Table.Rows(i)("DropDownID")
                sql = dvp.Table.Rows(i)("DropDownSQL").ToString
                'add row into html table TableParams
                AddRowIntoHTMLtable(dvp.Table.Rows(i), tblParameters)
                'TableParams.Rows(i + 4).Cells(6).InnerHtml = "<a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&m=" & CStr(i) & "'>edit</a>  or  <a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&d=" & CStr(i) & "'>del</a>"
                'TableParams.Rows(i + 4).BorderColor = "black"
                tblParameters.Rows(i + 1).Cells(4).InnerText = sql
                If Session("AdvancedUser") = True Then
                    tblParameters.Rows(0).Cells(3).Visible = True
                    tblParameters.Rows(0).Cells(4).Visible = True
                    tblParameters.Rows(i + 1).Cells(3).Visible = True
                    tblParameters.Rows(i + 1).Cells(4).Visible = True
                Else
                    tblParameters.Rows(0).Cells(3).Visible = False
                    tblParameters.Rows(i + 1).Cells(3).Visible = False
                    tblParameters.Rows(0).Cells(4).Visible = False
                    tblParameters.Rows(i + 1).Cells(4).Visible = False
                End If

                If m = "" And d = "" Then 'draw parameter
                    ctl = New LinkButton
                    ctl.Text = "edit"
                    ctl.ID = "edit_" & ddid
                    ctl.ToolTip = "Edit parameter"
                    AddHandler ctl.Click, AddressOf btnEdit_Click
                    tblParameters.Rows(i + 1).Cells(6).InnerHtml = ""
                    tblParameters.Rows(i + 1).Cells(6).Controls.Add(ctl)
                    ctl = New LinkButton
                    ctl.Text = "delete"
                    ctl.ID = "delete_" & ddid
                    ctl.ToolTip = "Delete parameter"
                    ctl.PostBackUrl = "ReportEdit.aspx?Report=" & repid & "&repedit=yes&d=" & CStr(i)
                    tblParameters.Rows(i + 1).Cells(7).InnerHtml = ""
                    tblParameters.Rows(i + 1).Cells(7).Controls.Add(ctl)
                    tblParameters.Rows(i + 1).BorderColor = "black"

                    tblParameters.Rows(i + 1).Cells(8).InnerHtml = ""
                    If i > 0 Then
                        ctl = New LinkButton
                        ctl.Text = "up"
                        ctl.ID = "up_" & i.ToString
                        ctl.ToolTip = "Move parameter up"
                        AddHandler ctl.Click, AddressOf btnUP_Click
                        tblParameters.Rows(i + 1).Cells(8).Controls.Add(ctl)
                    End If
                    tblParameters.Rows(i + 1).Cells(9).InnerHtml = ""
                    If i < n - 1 Then
                        ctl = New LinkButton
                        ctl.Text = "down"
                        ctl.ID = "down_" & i.ToString
                        ctl.ToolTip = "Move parameter down"
                        AddHandler ctl.Click, AddressOf btnDOWN_Click
                        tblParameters.Rows(i + 1).Cells(9).Controls.Add(ctl)
                    End If
                Else 'm or d is not empty

                    If d >= "0" Then 'delete parameter
                        If CInt(d) = i Then
                            SQLq = "DELETE FROM OURReportShow WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                            ExequteSQLquery(SQLq)
                            d = ""
                            Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                        End If

                    End If
                    If m >= "0" Then 'edit parameter
                        'If CInt(m) = i Then
                        '    TableParams.Rows(i + 4).Cells(6).InnerHtml = "<input type='submit' value='Submit Parameter' name='SubmitParam" & CStr(i) & "'> <input type='hidden' name='s' value='" & CStr(i) & "'><input type='hidden' name='m' value=''><input type='hidden' name='d' value=''>"
                        '    TableParams.Rows(i + 4).BorderColor = "red"
                        '    TableParams.Rows(i + 4).Cells(0).InnerHtml = "<textarea name=EditID" & CStr(i) & " rows=1 cols=10 >" & TableParams.Rows(i + 4).Cells(0).InnerText & "</textarea>"
                        '    TableParams.Rows(i + 4).Cells(1).InnerHtml = "<textarea name=EditLabel" & CStr(i) & " rows=1 cols=20 >" & TableParams.Rows(i + 4).Cells(1).InnerText & "</textarea>"
                        '    TableParams.Rows(i + 4).Cells(2).InnerHtml = "<textarea name=EditField" & CStr(i) & " rows=2 cols=10 >" & TableParams.Rows(i + 4).Cells(2).InnerText & "</textarea>"
                        '    TableParams.Rows(i + 4).Cells(3).InnerHtml = "<select name=EditType" & CStr(i) & " ><option>" & TableParams.Rows(i + 4).Cells(3).InnerText & "</option><option>nvarchar</option><option>int</option><option>datetime</option>"
                        '    TableParams.Rows(i + 4).Cells(4).InnerHtml = "<textarea name=EditSQL" & CStr(i) & " rows=2 cols=40 >" & TableParams.Rows(i + 4).Cells(4).InnerText & "</textarea>"
                        '    TableParams.Rows(i + 4).Cells(5).InnerHtml = "<textarea name=EditComm" & CStr(i) & " rows=2 cols=20 >" & TableParams.Rows(i + 4).Cells(5).InnerText & "</textarea>"
                        'End If
                        'm = ""
                    End If
                End If
                '-------------------------------------------------------

                'correct some fields when "submit" clicked
                If s >= "0" Then
                    'tblParameters.Rows(i + 1).Cells(6).InnerHtml = "<a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&m=" & CStr(i) & "'>edit</a>"
                    'tblParameters.Rows(i + 1).Cells(7).InnerHtml = "<a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&d=" & CStr(i) & "'>delete</a>"
                    tblParameters.Rows(i + 1).BorderColor = "black"
                    If CInt(s) = i Then

                        'update other fields when "submit" clicked...............................

                        'SQLq = "UPDATE OURReportShow SET DropDownLabel='" & Request("EditLabel" & CInt(s)) & "' WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                        'ExequteSQLquery(SQLq)
                        'SQLq = "UPDATE OURReportShow SET DropDownFieldName='" & Request("EditField" & CInt(s)) & "',DropDownFieldValue='" & Request("EditField" & CInt(s)) & "' WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                        'ExequteSQLquery(SQLq)
                        'SQLq = "UPDATE OURReportShow SET DropDownFieldType='" & Request("EditType" & CInt(s)) & "' WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                        'ExequteSQLquery(SQLq)
                        'SQLq = "UPDATE OURReportShow SET DropDownSQL='" & Request("EditSQL" & CInt(s)) & "' WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                        'ExequteSQLquery(SQLq)
                        'SQLq = "UPDATE OURReportShow SET COMMENTS='" & Request("EditComm" & CInt(s)) & "' WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                        'ExequteSQLquery(SQLq)
                        'SQLq = "UPDATE OURReportShow SET DropDownID='" & Request("EditID" & CInt(s)) & "', DropDownName='" & Request("EditID" & CInt(s)) & "' WHERE (ReportID='" & repid & "') AND (DropDownID='" & ddid & "')"
                        'ExequteSQLquery(SQLq)
                        'Dim logs As String = "Report " & repid & " has been changed. Parameter #" & ddid & ": " & Request("EditID" & CInt(s)) & ", Field Name: " & Request("EditField" & CInt(s)) & ", Field Type: " & Request("EditType" & CInt(s)) & ", SQL: " & Request("EditSQL" & CInt(s)) & ", Comments: " & Request("EditComm" & CInt(s))
                        'WriteToAccessLog(Session("logon"), logs, 2)
                        ''send emails
                        'Dim EmailTable As DataTable
                        'EmailTable = mRecords("SELECT  * FROM OURPermissions WHERE APPLICATION='InteractiveReporting' AND Param2='email' AND (Param1='" & repid & "') AND AccessLevel='admin'").Table
                        'If EmailTable.Rows.Count > 0 And s <> "" Then
                        '    For j = 0 To EmailTable.Rows.Count - 1
                        '        SendHTMLEmail("", "Report " & repid & " has been changed", "Parameter #" & ddid & ": " & Request("EditID" & CInt(s)) & ", Field Name: " & Request("EditField" & CInt(s)) & ", Field Type: " & Request("EditType" & CInt(s)) & ", SQL: " & Request("EditSQL" & CInt(s)) & ", Comments: " & Request("EditComm" & CInt(s)), EmailTable.Rows(j)("Param3"), Session("SupportEmail"))
                        '    Next
                        'End If
                        s = ""
                        m = ""
                        d = ""
                        Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                    End If
                End If
            Next

        End If

        If DropDownReportType.Text = "crystal" Then
            ButtonRDL.Visible = False
            ButtonRDL.Enabled = False
            ButtonUploadRDL.Visible = False
            ButtonUploadRDL.Enabled = False
            LabelRdlFile.Visible = False
            LabelRdlFile.Enabled = False
            'FileRDL.Visible = False
        ElseIf DropDownReportType.Text = "rdl" Then
            ButtonCrystal.Visible = False
            ButtonCrystal.Enabled = False
            ButtonUploadRPT.Visible = False
            ButtonUploadRPT.Enabled = False
            LabelRptFile.Visible = False
            LabelRptFile.Enabled = False
            FileRPT.Visible = False
            'HyperLinkGroups.Visible = True
            'HyperLinkGroups.Enabled = True
        ElseIf DropDownReportType.Text = "aspx" Then
            ButtonRDL.Visible = False
            ButtonRDL.Enabled = False
            ButtonCrystal.Visible = False
            ButtonCrystal.Enabled = False
            ButtonUploadRDL.Visible = False
            ButtonUploadRDL.Enabled = False
            LabelRdlFile.Visible = False
            LabelRdlFile.Enabled = False
            ButtonUploadRPT.Visible = False
            ButtonUploadRPT.Enabled = False
            LabelRptFile.Visible = False
            LabelRptFile.Enabled = False
            FileRPT.Visible = False
            'FileRDL.Visible = False
            TextBoxReportFile.Visible = False
            TextBoxReportFile.Enabled = False
            HyperLink5.Enabled = False
            HyperLink5.Visible = False
            LabelRdlRpt.Visible = False
            LabelRdlRpt.Enabled = False
            ButtonDownloadFile.Visible = False
            ButtonDownloadFile.Enabled = False
        End If
        '=================================================================================================================
        'draw users
        Dim mu, du, su, uid As String
        Dim dvu As DataView
        mu = ""
        su = ""
        du = ""
        mu = Request("mu")
        su = Request("su")
        du = Request("du")
        l = 0
        dvu = mRecords("SELECT NetId,AccessLevel,Param3,OpenFrom,OpenTo,Comments,Indx FROM OURPermissions WHERE (APPLICATION='InteractiveReporting' AND  Param1='" & repid & "') ORDER BY NetId", ret)
        If Not dvu Is Nothing AndAlso dvu.Count > 0 AndAlso Not dvu.Table Is Nothing Then
            l = dvu.Table.Rows.Count   'how many users
        End If

        LabelNofUsers.Text = l.ToString
        If l > 0 Then
            For i = 0 To l - 1
                uid = dvu.Table.Rows(i)("Indx")
                'add row into html table TableParams
                AddRowIntoHTMLtable(dvu.Table.Rows(i), TableUsers)
                TableUsers.Rows(i + 4).Cells(6).InnerHtml = "<a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&mu=" & CStr(i) & "'>edit</a>  or  <a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&du=" & CStr(i) & "'>del</a>"
                TableUsers.Rows(i + 4).BorderColor = "black"
                If mu = "" And du = "" Then 'draw parameter
                    TableUsers.Rows(i + 4).Cells(6).InnerHtml = "<a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&mu=" & CStr(i) & "'>edit</a>  or  <a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&du=" & CStr(i) & "'>del</a>"
                    TableUsers.Rows(i + 4).BorderColor = "black"
                Else 'mu or du is not empty

                    If du >= "0" Then 'delete user
                        If CInt(du) = i Then
                            SQLq = "DELETE FROM OURPermissions WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                            ExequteSQLquery(SQLq)
                            Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                        End If

                    End If
                    If mu >= "0" Then 'edit user
                        If CInt(mu) = i Then
                            TableUsers.Rows(i + 4).Cells(6).InnerHtml = "<input type='submit' value='Submit User' name='SubmitUser" & CStr(i) & "'> <input type='hidden' name='su' value='" & CStr(i) & "'><input type='hidden' name='mu' value=''><input type='hidden' name='du' value=''>"
                            TableUsers.Rows(i + 4).BorderColor = "red"
                            TableUsers.Rows(i + 4).Cells(0).InnerHtml = "<textarea name=EditNetID" & CStr(i) & " rows=1 cols=10 >" & TableUsers.Rows(i + 4).Cells(0).InnerText & "</textarea>"
                            TableUsers.Rows(i + 4).Cells(1).InnerHtml = "<select name=EditLevel" & CStr(i) & " ><option>" & TableUsers.Rows(i + 4).Cells(1).InnerText & "</option><option>user</option><option>admin</option>"

                            TableUsers.Rows(i + 4).Cells(2).InnerHtml = "<textarea name=EditEmail" & CStr(i) & " rows=1 cols=20 >" & TableUsers.Rows(i + 4).Cells(2).InnerText & "</textarea>"
                            TableUsers.Rows(i + 4).Cells(3).InnerHtml = "<textarea name=EditFrom" & CStr(i) & " rows=1 cols=10 >" & TableUsers.Rows(i + 4).Cells(3).InnerText & "</textarea>"

                            TableUsers.Rows(i + 4).Cells(4).InnerHtml = "<textarea name=EditTo" & CStr(i) & " rows=1 cols=10 >" & TableUsers.Rows(i + 4).Cells(4).InnerText & "</textarea>"
                            TableUsers.Rows(i + 4).Cells(5).InnerHtml = "<textarea name=EditComments" & CStr(i) & " rows=2 cols=20 >" & TableUsers.Rows(i + 4).Cells(5).InnerText & "</textarea>"
                        End If

                    End If
                End If
                '-------------------------------------------------------

                'correct some fields when "submit" clicked
                If su >= "0" Then
                    TableUsers.Rows(i + 4).Cells(6).InnerHtml = "<a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&mu=" & CStr(i) & "'>edit</a>  or  <a href='ReportEdit.aspx?Report=" & repid & "&repedit=yes&du=" & CStr(i) & "'>del</a>"
                    TableUsers.Rows(i + 4).BorderColor = "black"
                    If CInt(su) = i Then
                        'update other fields when "submit" clicked...............................
                        Dim startdate As String = Request("EditFrom" & CInt(su)).ToString
                        Dim enddate As String = Request("EditTo" & CInt(su)).ToString
                        If startdate = "" Then
                            startdate = DateToString(Now)
                        End If
                        If enddate = "" OrElse DateToString(enddate) > Session("UserEndDate") Then
                            enddate = Session("UserEndDate")
                        End If
                        startdate = DateToString(startdate)
                        enddate = DateToString(enddate)
                        SQLq = "UPDATE OURPermissions SET AccessLevel='" & Request("EditLevel" & CInt(su)) & "' WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURPermissions SET Param3='" & Request("EditEmail" & CInt(su)) & "' WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURPermissions SET OpenFrom='" & startdate & "' WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURPermissions SET OpenTo='" & enddate & "' WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURPermissions SET COMMENTS='" & Request("EditComments" & CInt(su)) & "' WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                        ret = ExequteSQLquery(SQLq)
                        SQLq = "UPDATE OURPermissions SET NetID='" & Request("EditNetID" & CInt(su)) & "' WHERE (Param1='" & repid & "') AND (Indx=" & uid & ")"
                        ret = ExequteSQLquery(SQLq)
                        Dim logs As String = "Report " & repid & " has been changed. User #" & uid & ": " & Request("EditNetID" & CInt(su)) & ", Level: " & Request("EditLevel" & CInt(su)) & ", Email: " & Request("EditEmail" & CInt(su)) & ", From " & Request("EditFrom" & CInt(su)) & "= to: " & Request("EditTo" & CInt(su)) & ", Comments: " & Request("EditComments" & CInt(su))
                        WriteToAccessLog(Session("logon"), logs, 2)
                        'TODO add report tables into User Tables for admin user
                        Dim res As String = String.Empty
                        If Request("EditLevel" & CInt(su)) = "admin" Then
                            'add report tables into OURUsertables
                            Dim tbl As String = String.Empty
                            Dim dvt As DataTable
                            'dv = GetReportTablesFromOURUserTables(Session("REPORTID"), Session("Unit"), Session("logon"), Session("UserDB"), res)
                            dvt = GetReportTables(Session("REPORTID"))

                            If dvt Is Nothing Then
                                Label1.Text = "There are no tables found for this report."
                                Exit Sub
                            End If
                            'make loop to add tables in OURUserTables of Userdb
                            For j = 0 To dvt.Rows.Count - 1
                                tbl = FixReservedWords(dvt.Rows(j)("Table_Name").ToString.Trim, Session("UserConnProvider"))
                                ret = InsertTableIntoOURUserTables(tbl, tbl, Session("Unit"), TextBoxNetId.Text, Session("UserDB"), "", Session("REPORTID"))
                                WriteToAccessLog(Session("logon"), "The table " & tbl & " has been added into OURUserTables with result: " & res, 111)
                            Next
                        End If


                        ''send emails
                        'Dim re As String = String.Empty
                        'Dim EmailTable As DataTable
                        'EmailTable = mRecords("SELECT  * FROM OURPermissions WHERE APPLICATION='InteractiveReporting' AND (Param1='" & repid & "') AND AccessLevel='admin'").Table
                        'If EmailTable.Rows.Count > 0 And su >= "0" Then
                        '    For j = 0 To EmailTable.Rows.Count - 1
                        '        re = re & " " & SendHTMLEmail("", "Report " & repid & " has been changed", "User #" & uid & ": " & Request("EditNetID" & CInt(su)) & ", Level: " & Request("EditLevel" & CInt(su)) & ", Email: " & Request("EditEmail" & CInt(su)) & ", From " & Request("EditFrom" & CInt(su)) & "= to: " & Request("EditTo" & CInt(su)) & ", Comments: " & Request("EditComments" & CInt(su)), EmailTable.Rows(j)("Param3"), Session("SupportEmail"))
                        '    Next
                        'End If
                        su = ""
                        mu = ""
                        du = ""
                        Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                    End If
                End If
            Next
        End If
        Select Case Session("TabN")
            Case "2"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Report_Info"
            Case "3"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Parameters"
            Case "4"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Users"
        End Select
        If Session("CSV") = "yes" Then
            ButtonDownloadFile.Visible = False
            ButtonDownloadFile.Enabled = False
            'ButtonUploadRDL.Text = "Upload CSV/XML file"
            'ButtonUploadRDL.ToolTip = "Upload selected CSV or XML file"
            LabelTableName.Visible = True
            txtTableName.Visible = True
            txtTableName.Enabled = True
            LabelDelimiter.Visible = True
            TextboxDelimiter.Visible = True
            TextboxDelimiter.Enabled = True
            If TextboxDelimiter.Text.Trim = "" Then
                TextboxDelimiter.Text = " "
            End If
            trEditRep2.Visible = False
            If TextBoxSQLorSPtext.Text.Trim = "" OrElse TextBoxSQLorSPtext.Text.Trim = "SELECT" OrElse TextBoxSQLorSPtext.Text.ToUpper.Trim = "SELECT * FROM" Then
                trEditRep3.Visible = False
            Else
                trEditRep3.Visible = True
            End If
            trEditRep4.Visible = False
        ElseIf Session("admin") = "super" Then
            Session("AdvancedUser") = True
            trEditRep0.Visible = True
            trEditRep2.Visible = True
            trEditRep3.Visible = True
            trEditRep4.Visible = False
        End If
        If Session("UserConnProvider").ToString.Trim = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
            trEditRep2.Visible = False
        End If

        If lblFileChosen.Text.ToUpper.EndsWith(".JSON") OrElse lblFileChosen.Text.ToUpper.EndsWith(".TXT") OrElse lblFileChosen.Text.ToUpper.EndsWith(".XML") OrElse lblFileChosen.Text.ToUpper.EndsWith(".RDL") Then
            txtTableName.Enabled = False
            DropDownTables.Enabled = False
            DropDownTables.Text = " "
        ElseIf txtURI.Text.ToUpper.EndsWith(".JSON") OrElse txtURI.Text.ToUpper.EndsWith(".XML") OrElse txtURI.Text.ToUpper.EndsWith(".TXT") OrElse txtURI.Text.ToUpper.EndsWith(".RDL") Then
            txtTableName.Enabled = False
            DropDownTables.Enabled = False
            DropDownTables.Text = " "
        Else 'lblFileChosen.Text = " or from web site: " Then
            txtTableName.Enabled = True
            DropDownTables.Enabled = True
        End If
        If TextBoxSQLorSPtext.Text = "" OrElse TextBoxSQLorSPtext.Text.Trim = "SELECT  * FROM" Then
            txtTableName.Enabled = True
        End If
        'If (Not IsPostBack) AndAlso TextBoxSQLorSPtext.Text <> "" AndAlso TextBoxSQLorSPtext.Text.Trim <> "SELECT  * FROM" Then
        If TextBoxSQLorSPtext.Text <> "" AndAlso TextBoxSQLorSPtext.Text.Trim <> "SELECT  * FROM" Then
            'get tblname from ourusertables for this report
            Dim k As Integer = 0
            Dim tb As String = GetTableFriendlyName("", Session("Unit"), Session("logon"), Session("UserDB"), repid)
            k = tb.IndexOf("_")
            If k > 0 Then
                tb = tb.Substring(0, k)
            End If
            txtTableName.Text = tb

        End If
        If Session("TableFriendlyName").ToString.Trim = "" AndAlso txtTableName.Text.Trim <> "" Then
            Session("TableFriendlyName") = txtTableName.Text.Trim
        End If
        If Session("TableFriendlyName").ToString.Trim <> "" AndAlso txtTableName.Text.Trim = "" Then
            txtTableName.Text = Session("TableFriendlyName").ToString.Trim
        End If
    End Sub

    Private Sub SetReportFieldData(repid As String)
        Dim bfld As Boolean = False
        Dim insSQL As String = String.Empty
        Dim delSQL As String = String.Empty
        Dim ddt As DataTable = Nothing
        Dim dtrf As DataTable = Nothing
        Dim frname As String = String.Empty
        Dim ret As String = String.Empty

        ddt = GetListOfReportFields(repid)  'List of Report fields from xsd
        If ddt Is Nothing Then
            Exit Sub
        End If
        'add report fields from  OURReportFormat
        dtrf = GetReportFields(repid)
        If dtrf Is Nothing OrElse dtrf.Rows.Count = 0 Then  'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
            'add all fields from ddt
            For i As Integer = 0 To ddt.Columns.Count - 1
                If ddt.Columns(i).Caption <> "Indx" Then
                    frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                    If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & Session("REPORTID") & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
                    ExequteSQLquery(insSQL)
                End If
            Next
        Else
            ''delete from OURReportFormat fields that are not in dtrf
            'TODO WHY IS IT?
            For i As Integer = 0 To dtrf.Rows.Count - 1
                bfld = False
                For j = 0 To ddt.Columns.Count - 1
                    'check if this is computed column or from data - ddt (xsd)
                    If dtrf.Rows(i)("Prop3").ToString = "1" OrElse ddt.Columns(j).Caption = dtrf.Rows(i)("Val").ToString Then
                        bfld = True
                    End If
                Next
                If bfld = False Then
                    'delete from OURReportFormat the field not found in the data fields
                    delSQL = "DELETE FROM OURReportFormat WHERE ReportID='" & Session("REPORTID") & "' AND Prop='FIELDS' AND Indx=" & dtrf.Rows(i)("Indx").ToString
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportgroups
                    delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & Session("REPORTID") & "' AND GroupField='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportgroups
                    delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & Session("REPORTID") & "' AND CalcField='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportlists
                    delSQL = "DELETE FROM OURReportLists WHERE ReportID='" & Session("REPORTID") & "' AND ListFld='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    'delete from ourreportlists
                    delSQL = "DELETE FROM OURReportLists WHERE ReportID='" & Session("REPORTID") & "' AND RecFld='" & dtrf.Rows(i)("Val").ToString & "' "
                    ret = ExequteSQLquery(delSQL)
                    delSQL = "DELETE FROM OURReportItems WHERE ReportID = '" & Session("REPORTID") & "' AND SQLField = '" & dtrf.Rows(i)("Val").ToString & "'"
                    ret = ExequteSQLquery(delSQL)
                    'TODO update Report or show the message to update report
                End If
            Next
        End If



    End Sub
    Protected Sub ButtonSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonSubmit.Click
        Dim SQLq As String = String.Empty
        Dim dv5 As DataView
        Dim i As Integer
        Dim j As Integer
        Dim er As String = String.Empty
        Dim retr As String = String.Empty
        Dim SQLtext As String = String.Empty
        Dim bUseSQLText As Boolean = chkUseSQLText.Checked
        Dim sUseSQLText As String = ""

        LabelAlert.Text = ""
        If Session("REPORTID") = "" Then repid = Request(TextBoxReportID.UniqueID)
        If repid = "" Or Request(TextBoxReportTitle.UniqueID) = "" Then
            LabelAlert.Text = "ID, Name, Title, etc... should not be empty"
        Else
            Session("REPORTID") = repid
            If DropDownReportAttributes.Text = "sql" Then
                SQLtext = TextBoxSQLorSPtext.Text.Trim
                If SQLtext = String.Empty OrElse Piece(SQLtext.ToUpper, "FROM", 2) = String.Empty Then
                    MessageBox.Show("Report fields have not been chosen.", "Fields Not Chosen", "FieldsNotChosen", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                    Return
                End If
            End If
            'If Not HasSQLData(repid) Then
            '    MessageBox.Show("Report fields have not been chosen.", "Fields Not Chosen", "FieldsNotChosen", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
            '    Return
            'End If

            If chkRemoveReportFormating.Checked Then
                Dim delSQL As String = String.Empty
                'Delete From OURReportFormat
                delSQL = "DELETE FROM OURReportFormat WHERE ReportID='" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)
                'delete from ourreportgroups
                delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)
                'delete from ourreportlists
                delSQL = "DELETE FROM OURReportLists WHERE ReportID='" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)
                delSQL = "DELETE FROM OURReportShow WHERE ReportId = '" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)
                delSQL = "DELETE FROM OURReportItems WHERE ReportID='" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)
                delSQL = "UPDATE OURFiles SET Prop2 = NULL WHERE ReportId='" & Session("REPORTID") & "' AND Type = 'RDL'"
                retr = ExequteSQLquery(delSQL)
                delSQL = "UPDATE OURReportInfo SET Param0type = 'Tabular' WHERE ReportId='" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)
                delSQL = "UPDATE OURReportView SET ReportTemplate = 'Tabular' WHERE ReportId='" & Session("REPORTID") & "'"
                retr = ExequteSQLquery(delSQL)

            End If

            Dim ReportDate As String = DateToString(Now)
            Dim usr As String = Session("logon")

            'Update OURReportView
            If ReportViewExists(repid) Then
                SQLq = "UPDATE OURReportView "
                SQLq &= "SET ReportTitle='" & TextBoxReportTitle.Text & "',"
                SQLq &= "Orientation ='" & DropDownOrientation.SelectedValue.ToString & "',"
                SQLq &= "UpdatedBy = '" & usr & "',"
                SQLq &= "LastUpdate = '" & ReportDate & "' "
                SQLq &= "WHERE(ReportID ='" & repid & "')"
                retr = ExequteSQLquery(SQLq)
            Else
                SQLq = "INSERT INTO OURReportView ("
                SQLq &= "ReportID,"
                SQLq &= "ReportTitle,"
                SQLq &= "Orientation,"
                SQLq &= "CreatedBy,"
                SQLq &= "DateCreated,"
                SQLq &= "UpdatedBy,"
                SQLq &= "LastUpdate)"
                SQLq &= "VALUES ("
                SQLq &= "'" & repid & "',"
                SQLq &= "'" & TextBoxReportTitle.Text & "',"
                SQLq &= "'" & DropDownOrientation.SelectedValue.ToString & "',"
                SQLq &= "'" & usr & "',"
                SQLq &= "'" & ReportDate & "',"
                SQLq &= "'" & usr & "',"
                SQLq &= "'" & ReportDate & "')"
                retr = ExequteSQLquery(SQLq)
            End If


            Dim bQueryDiff As Boolean = False
            'If edit report then update ReportInfo table
            If Session("REPEDIT") = "yes" Then
                'landscape or portrait
                SQLq = "UPDATE OURReportInfo SET Param9type='" & DropDownOrientation.SelectedValue.ToString & "' WHERE (ReportID='" & repid & "')"
                retr = ExequteSQLquery(SQLq)
                SQLq = "UPDATE OURReportInfo SET ReportFile='" & Request(TextBoxReportFile.UniqueID) & "' WHERE (ReportID='" & repid & "')"
                retr = ExequteSQLquery(SQLq)

                'update OURReportInfo fields
                If DropDownReportAttributes.Text = "sp" Then
                    If DropDownSPs.Items.Count > 0 AndAlso DropDownSPs.SelectedItem.Text <> "" Then
                        TextBoxNewSPname.Text = DropDownSPs.SelectedItem.Text
                    End If
                    SQLq = "UPDATE OURReportInfo SET ReportName='" & repid & "', ReportTtl='" & TextBoxReportTitle.Text & "', Comments='" & TextBoxPageFtr.Text & "', ReportType='" & DropDownReportType.Text & "', ReportAttributes='" & DropDownReportAttributes.Text & "', SQLquerytext='" & TextBoxNewSPname.Text & "' WHERE (ReportID='" & repid & "')"
                    retr = ExequteSQLquery(SQLq)

                ElseIf DropDownReportAttributes.Text = "sql" Then
                    Dim tblfld As String
                    Dim delfld(0) As String
                    Dim ndf As Boolean = False
                    Dim nd As Boolean = False

                    'reset data from Query Text
                    SQLtext = TextBoxSQLorSPtext.Text.Trim
                    retr = SaveSQLquery(repid, SQLtext)
                    'list of new selected fields
                    Dim selfields As String = GetListOfSelectedFieldsFromSQLquery(repid)
                    'add table name:
                    selfields = FixSelectedFields(repid, selfields, Session("UserConnString"), Session("UserConnProvider"))
                    selfields = selfields.Replace("`", "").Replace("[", "").Replace("]", "").Replace("""", "").Replace(" ", "")
                    selfields = "," & selfields & ","
                    'original list of fields before updating
                    Dim dvl As DataView = GetListOfFieldsFromOURReportSQLquery(repid)
                    If dvl Is Nothing OrElse dvl.Table Is Nothing OrElse dvl.Table.Rows.Count = 0 Then
                        'no fields selected -> no deleted fields
                        'ndf=False
                    Else
                        'deleted fields
                        For i = 0 To dvl.Table.Rows.Count - 1
                            tblfld = "," & dvl.Table.Rows(i)("Tbl1") & "." & dvl.Table.Rows(i)("Tbl1Fld1") & ","
                            If selfields.ToUpper.IndexOf(tblfld.ToUpper) < 0 Then
                                'not in selected fields any more
                                DeleteReportField(repid, dvl.Table.Rows(i)("Tbl1"), dvl.Table.Rows(i)("Tbl1Fld1"))
                            End If
                        Next
                    End If

                    '---------------------------------------------------------------------------------------------------

                    sUseSQLText = IIf(bUseSQLText, "UseSQLText", "")
                    SQLq = "UPDATE OURReportInfo SET ReportName='" & repid & "', ReportTtl='" & TextBoxReportTitle.Text & "', Comments='" & TextBoxPageFtr.Text & "', ReportType='" & DropDownReportType.Text & "', ReportAttributes='" & DropDownReportAttributes.Text & "', Param8type='" & sUseSQLText & "' WHERE (ReportID='" & repid & "')"
                    retr = ExequteSQLquery(SQLq)

                    'update SQLquerytext field in OURReportInfo 

                    If Session("SQLquerytext") <> SQLtext.Replace("'", """") Then
                        bQueryDiff = True
                        'delete select fields from OURsqlquery
                        SQLq = "DELETE FROM OURReportSQLquery WHERE ReportId = '" & repid & "' AND (DOING = 'SELECT' OR DOING = 'WHERE' OR DOING = 'JOIN' OR DOING = 'ORDER BY')"
                        retr = ExequteSQLquery(SQLq)

                        Dim m As Integer = 0
                        Dim k As Integer = 0
                        Dim tabl, fild, frname As String
                        Dim sctSQL As String = String.Empty
                        Dim rt As String = String.Empty
                        Dim dt As DataTable
                        Dim ern As String = String.Empty
                        Dim sqlfield As String() = Nothing
                        If selfields <> "" Then
                            'Set SQLFields from sql statement
                            'selfields = FixSelectedFields(repid, selfields, Session("UserConnString"), Session("UserConnProvider"))
                            If Not selfields.ToUpper.Contains("ERROR") Then
                                sqlfield = selfields.Split(",")
                                For m = 0 To sqlfield.Length - 1
                                    If sqlfield(m).Trim <> "" Then
                                        tabl = sqlfield(m).Trim.Substring(0, sqlfield(m).Trim.LastIndexOf(".")).Trim
                                        fild = sqlfield(m).Trim.Substring(sqlfield(m).Trim.LastIndexOf(".") + 1).Trim
                                        frname = GlobalFriendlyName(tabl, fild, Session("UnitDB"), er) 'GetFriendlyFieldName(repid, fild)

                                        If Not SQLFieldExists(repid, sqlfield(m).Trim) Then
                                            sctSQL = "INSERT INTO OURReportSQLquery (ReportID, Doing, Tbl1, Tbl1Fld1,Friendly) VALUES('" & repid & "','SELECT','" & tabl & "','" & fild & "','" & frname & "')"
                                            rt = ExequteSQLquery(sctSQL)

                                            'TODO AddReportItem if does not exist  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1

                                        End If

                                    End If
                                Next
                            End If
                            'Set Joins from sql statement
                            dt = GetListOfJoinsFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)
                            'Set Sorts from sql statement
                            ern = String.Empty
                            dt = GetListOfOrderByFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)
                            'Set Conditions from sql statement
                            ern = String.Empty
                            dt = GetListOfConditionsFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)
                            'ern = String.Empty
                            'dt = GetListOfOrderByFromSQLquery(repid, Session("UserConnString"), Session("UserConnProvider"), ern)

                        End If
                    End If
                End If

                If HasReportColumns(repid) AndAlso Not ReportItemsExist(repid) Then
                    retr = CreateReportItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                    If retr.StartsWith("ERROR!!") Then
                        LabelAlert.Text = "Report info is not updated. " & retr
                        LabelAlert.Visible = True
                        Exit Sub
                    End If
                End If

                LabelAlert.Text = "Report info is updated. " 'If report has parameters and/or users please check them now."
                Dim logs As String = "Report " & Session("REPTITLE") & " (" & Session("REPORTID") & ") information has been updated."
                WriteToAccessLog(Session("logon"), logs, 2)

            End If
            'If new report then create new row in ReportInfo table
            If Session("REPEDIT") = "new" Then
                dv5 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & Request(TextBoxReportID.UniqueID.ToString) & "')")
                If dv5.Table.Rows.Count = 0 Then
                    If DropDownReportAttributes.Text = "sp" Then
                        TextBoxNewSPname.Text = DropDownSPs.SelectedItem.Text

                        SQLq = "INSERT  INTO OURReportInfo (ReportID, ReportName, ReportTtl, Comments, ReportType, ReportAttributes, SQLquerytext) VALUES ('" & Request(TextBoxReportID.UniqueID.ToString) & "','" & Request(TextBoxReportID.UniqueID.ToString) & "','" & Request(TextBoxReportTitle.UniqueID.ToString) & "','" & Request(TextBoxPageFtr.UniqueID) & "','" & Request(DropDownReportType.UniqueID.ToString) & "','" & Request(DropDownReportAttributes.UniqueID.ToString) & "','" & TextBoxNewSPname.Text & "')"

                    ElseIf DropDownReportAttributes.Text = "sql" Then
                            sUseSQLText = IIf(bUseSQLText, "UseSQLText", "")

                        SQLq = "INSERT  INTO OURReportInfo (ReportID, ReportName, ReportTtl, Comments, ReportType, ReportAttributes, SQLquerytext,Param8type) VALUES ('" & TextBoxReportID.Text & "','" & TextBoxReportID.Text & "','" & TextBoxReportTitle.Text & "','" & TextBoxPageFtr.Text & "','" & DropDownReportType.Text & "','" & DropDownReportAttributes.Text & "','" & TextBoxSQLorSPtext.Text & "','" & sUseSQLText & "')"

                    End If
                    retr = ExequteSQLquery(SQLq)
                    LabelAlert.Text = "Report is created. " 'If report has parameters and/or users please assign them now."
                    Session("REPORTID") = Request(TextBoxReportID.UniqueID.ToString)
                    Session("REPTITLE") = Request(TextBoxReportTitle.UniqueID.ToString)
                Else
                    LabelAlert.Text = "ID should be unique, please choose other one"
                End If

                'no users: add creator as admin for this report
                TextBoxNetId.Text = Session("logon")
                TextBoxFrom.Text = DateToString(Now)
                TextBoxTo.Text = DateToString(Session("UserEndDate"))
                If Session("REPORTID") <> "" And Session("logon") <> "" Then
                    SQLq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & Session("REPORTID") & "','" & Session("REPTITLE") & "','" & Session("email") & "','admin','" & TextBoxFrom.Text & "','" & TextBoxTo.Text & "','" & TextBoxComm.Text & "')"
                    retr = ExequteSQLquery(SQLq)

                    'Dim re As String = SendHTMLEmail("", "Report " & Session("REPTITLE") & " has been created", "Report " & Session("REPTITLE") & " (" & Session("REPORTID") & ") has been created by user " & Session("logon"), Session("email"), Session("SupportEmail"))
                    WriteToAccessLog(Session("logon"), "Report " & Session("REPTITLE") & " (" & Session("REPORTID") & ") has been created.", 0)

                End If
            End If

            ''DO NOT DELETE: add tables into ourusertables
            'If Session("UserConnProvider") = "System.Data.Odbc" Then
            '    Dim dts As DataTable = GetListOfTablesFromSQLquery(TextBoxSQLorSPtext.Text)
            '    If dts IsNot Nothing Then
            '        For i = 0 To dts.Rows.Count - 1
            '            er = InsertTableIntoOURUserTables(dts.Rows(i)("Tbl1").ToString, dts.Rows(i)("Tbl1").ToString, Session("Unit"), Session("logon"), Session("UserDB"))
            '        Next
            '    End If
            'End If
            'update parameters
            LabelAlert.Visible = True
            SQLtext = TextBoxSQLorSPtext.Text
            retr = UpdateParameters(repid, SQLtext)
            LabelAlert.Text = LabelAlert.Text & " , " & retr
            'update xsd and rdl
            Dim repfile As String = String.Empty
            Session("dv3") = Nothing
            Dim ret As String = UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"), repfile)
            ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            TextBoxReportFile.Text = repfile
            If bQueryDiff Then
                SetReportFieldData(repid)
            End If
            'If HasReportColumns(repid) AndAlso Not ReportItemsExist(repid) Then
            '    'CreateReportItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            '    retr = CreateReportItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            '    If retr.StartsWith("ERROR!!") Then
            '        LabelAlert.Text = "Report info is not updated. " & retr
            '        LabelAlert.Visible = True
            '        Exit Sub
            '    End If
            'End If
            WriteToAccessLog(Session("logon"), ret & "  " & LabelAlert.Text, 5)
            If ret.StartsWith("ERROR!!") Then
                LabelAlert.ForeColor = Color.Gray
                LabelAlert.Text = LabelAlert.Text & "  " & ret
                LabelAlert.Text = "Report format has been updated with errors. "
            Else
                ' ret = LabelAlert.Text & "Report format has been updated. "
                LabelAlert.Text = "Report format has been updated. "
            End If
            'Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=yes&msg=" & LabelAlert.Text)
            'Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=yes")
        End If
    End Sub

    Protected Sub ButtonAddUser_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddUser.Click
        'add report user
        If TextBoxNetId.Text.Trim = "" Then
            LabelAlert.Text = "User logon cannot be empty."
            LabelAlert.Visible = True
            Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=yes&msg=" & LabelAlert.Text)
            Exit Sub
        Else
            LabelAlert.Text = ""
        End If
        Dim SQLq, EmailOrNot As String
        Dim ret As String = String.Empty
        If DropDownListAccessLevel.Text = "admin" Then
            EmailOrNot = "email"
        Else
            EmailOrNot = ""
        End If
        If Session("REPORTID") <> "" Then 'add user
            If TextBoxFrom.Text = "" Then
                TextBoxFrom.Text = DateToString(Now.Date)
            End If
            If TextBoxTo.Text = "" OrElse DateToString(TextBoxTo.Text) > Session("UserEndDate") Then
                TextBoxTo.Text = Session("UserEndDate")
            End If
            TextBoxFrom.Text = DateToString(TextBoxFrom.Text)
            TextBoxTo.Text = DateToString(TextBoxTo.Text)
            SQLq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & TextBoxNetId.Text & "','InteractiveReporting','" & Session("REPORTID") & "','" & Session("REPTITLE") & "','" & TextBoxEmail.Text & "','" & DropDownListAccessLevel.Text & "','" & TextBoxFrom.Text & "','" & TextBoxTo.Text & "','" & TextBoxComm.Text & "')"
            ret = ExequteSQLquery(SQLq)

            Dim re As String = String.Empty
            'find if new report user is registered
            Dim userconnstr As String = Session("UserConnString")
            'remove password
            If userconnstr.IndexOf("Password") > 0 Then userconnstr = userconnstr.Substring(0, userconnstr.IndexOf("Password")).Trim
            Dim dt As DataTable = mRecords("SELECT * FROM OURPermits WHERE NetId='" & TextBoxNetId.Text & "' And  ConnStr ='" & userconnstr & "' AND Application='InteractiveReporting'").Table  'find the user
            If dt.Rows.Count = 0 Then
                'save Connection info and else
                SQLq = "INSERT INTO OURPermits (Permit,NetID,localpass,Application,Access,RoleApp,ConnStr,ConnPrv,Email,Comments,StartDate,EndDate) VALUES('user','" & TextBoxNetId.Text & "','" & TextBoxEmail.Text & "','InteractiveReporting','user','user','" & userconnstr & "','" & Session("UserConnProvider") & "','" & TextBoxEmail.Text & "','Added by " & Session("logon") & "','" & TextBoxFrom.Text & "','" & TextBoxTo.Text & "')"
                re = ExequteSQLquery(SQLq)
                'send email to added user
                re = re & " " & SendHTMLEmail("", "User for report -- " & Session("REPTITLE") & " -- has been added", "User has been created by admin " & Session("logon") & ". User logon: " & TextBoxNetId.Text & ", initial password is your email. You should change your password when first time you logged in. ", TextBoxEmail.Text, Session("SupportEmail"))
            End If

            If DropDownListAccessLevel.Text = "admin" Then
                'add report tables into OURUsertables
                Dim tbl As String = String.Empty
                Dim dvt As DataTable
                'dv = GetReportTablesFromOURUserTables(Session("REPORTID"), Session("Unit"), Session("logon"), Session("UserDB"), ret)
                dvt = GetReportTables(Session("REPORTID"))
                If dvt Is Nothing Then
                    Label1.Text = "There are no tables found for this report."
                    Exit Sub
                End If
                'make loop to add tables in OURUserTables of Userdb
                For i = 0 To dvt.Rows.Count - 1
                    tbl = FixReservedWords(dvt.Rows(i)("Table_Name").ToString.Trim, Session("UserConnProvider"))
                    ret = InsertTableIntoOURUserTables(tbl, tbl, Session("Unit"), TextBoxNetId.Text, Session("UserDB"), "", Session("REPORTID"))
                    WriteToAccessLog(Session("logon"), "The table " & tbl & " has been added into OURUserTables with result: " & ret, 111)
                Next
            End If

            'send emails
            Dim EmailTable As DataTable
            Dim j As Integer
            EmailTable = mRecords("SELECT  * FROM OURPermissions WHERE APPLICATION='InteractiveReporting' AND Param2='email' AND (Param1='" & Session("REPORTID") & "') AND AccessLevel='admin'").Table
            If EmailTable.Rows.Count > 0 Then
                For j = 0 To EmailTable.Rows.Count - 1
                    re = re & " " & SendHTMLEmail("", "User for report " & Session("REPORTID") & " has been added", "User " & TextBoxNetId.Text & " has been added to report users by NetId " & Session("logon") & ", " & ret, EmailTable.Rows(j)("Param3"), Session("SupportEmail"))
                Next
            End If
            WriteToAccessLog(Session("logon"), re, 0)

        End If
        Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=yes")
    End Sub

    Protected Sub ButtonAddParameter_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ButtonAddParameter.Click

        Dim j As Integer
        Dim SQLq As String
        If Session("REPORTID") <> "" Then
            SQLq = "INSERT INTO OURReportShow (ReportID, DropDownID, DropDownLabel, DropDownName, DropDownFieldName, DropDownFieldType, DropDownFieldValue, DropDownSQL, COMMENTS) VALUES('" & Session("REPORTID") & "','" & TextBoxID.Text & "','" & TextBoxLabel.Text & "','" & TextBoxID.Text & "','" & TextBoxField.Text & "','" & DropDownListType.Text & "','" & TextBoxID.Text & "','" & TextBoxSQL.Text & "','" & TextComments.Text & "')"
            ExequteSQLquery(SQLq)
            WriteToAccessLog(Session("logon"), "Parameter for report " & Session("REPORTID") & " has been created", 0)
            ''send emails
            'Dim EmailTable As DataTable
            'EmailTable = mRecords("SELECT  * FROM OURPermissions WHERE APPLICATION='InteractiveReporting' AND (Param1='" & Session("REPORTID") & "') AND AccessLevel='admin'").Table
            'If EmailTable.Rows.Count > 0 Then
            '    For j = 0 To EmailTable.Rows.Count - 1
            '        SendHTMLEmail("", "Parameter for report " & Session("REPORTID") & " has been created", "Parameter " & TextBoxLabel.Text & " has been created by NetId " & Session("logon"), EmailTable.Rows(j)("Param3"), Session("SupportEmail"))
            '    Next
            'End If
        Else

        End If
        Response.Redirect("ReportEdit.aspx?Report=" & Session("REPORTID") & "&repedit=yes")
    End Sub

    Private Sub ButtonXSD_Click(sender As Object, e As EventArgs) Handles ButtonXSD.Click
        'NOT IN USE
        Dim ret As String = CreateXSD(repid, Session("UserConnString"), Session("UserConnProvider"))

        'Dim dv2, dv3 As DataView
        'Dim dt As DataTable
        'Dim ParamNames() As String
        'Dim ParamTypes() As String
        'Dim ParamValues() As String
        'Dim reporttype As String = DropDownReportAttributes.Text
        'Dim mSql As String = String.Empty
        'If reporttype = "sql" Then
        '    mSql = TextBoxSQLorSPtext.Text  'sql statement
        '    dv3 = mRecords(mSql)
        'ElseIf reporttype = "sp" Then
        '    mSql = TextBoxNewSPname.Text    'stored procedure name
        '    'Parameters (drop-downs, data for params to sp)
        '    dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
        '    n = dv2.Table.Rows.Count   'how many parameters (drop-downs)
        '    ReDim ParamNames(n)
        '    ReDim ParamTypes(n)
        '    ReDim ParamValues(n)
        '    For i = 0 To dv2.Table.Rows.Count - 1
        '        ParamNames(i) = dv2.Table.Rows(i)("DropDownName")
        '        ParamTypes(i) = dv2.Table.Rows(i)("DropDownFieldType")
        '        If (ParamTypes(i) = "nvarchar" Or ParamTypes(i) = "datetime") Then
        '            ParamValues(i) = "0"
        '        Else
        '            ParamValues(i) = 0
        '        End If
        '    Next
        '    dv3 = mRecordsFromSP(mSql, n, ParamNames, ParamTypes, ParamValues)  'Data for report from SP
        'End If
        'Session("mySQL") = mSql

        ''Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        'Dim xsdpath As String = applpath & "XSDFILES\"
        'If dv3 IsNot Nothing Then
        '    dt = dv3.Table
        '    Dim ErrorLog As String = CreateXSDForDataTable(dt, repid, xsdpath)
        '    LabelXSD.Text = ErrorLog
        '    LabelXSD.Visible = True
        '    If ErrorLog.StartsWith("ERROR!!") Then
        '        LabelXSD.ForeColor = Drawing.Color.DeepPink
        '    End If
        'Else
        '    LabelXSD.ForeColor = Drawing.Color.DeepPink
        '    LabelXSD.Text = "Running stored procedure crashed. Check sp parameters."
        'End If

    End Sub

    Private Sub ButtonCrystal_Click(sender As Object, e As EventArgs) Handles ButtonCrystal.Click
        'Dim reporttype As String = DropDownReportAttributes.Text
        'Dim mSql As String = String.Empty
        'If reporttype = "sql" Then
        '    mSql = TextBoxSQLorSPtext.Text  'sql statement
        'ElseIf reporttype = "sp" Then
        '    mSql = TextBoxNewSPname.Text
        'End If
        'repid = Session("REPORTID")
        ''Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        'Dim rptpath As String = applpath & "RPTFILES\"
        'Dim repfile As String = rptpath & repid & ".rpt"
        'Dim ErrorLog = String.Empty
        ''if rdl file exist do not overwrite
        'If File.Exists(repfile) Then
        '    ErrorLog = "ERROR!! File " & repid & ".rpt" & " already exists. Delete it before creating a new one."
        '    TextBoxReportFile.Text = repid & ".rpt"
        '    HyperLink5.NavigateUrl = "~/ReportEdit.aspx?DELREP=yes&REPTYPE=rpt&REPORT=" & repid
        'Else
        '    ErrorLog = CreateCrystalReportForXSD(repid, rptpath)
        'End If
        'If ErrorLog.StartsWith("ERROR!!") Then
        '    LabelRPT.ForeColor = Drawing.Color.DeepPink
        '    Session("NewFileCreated") = False
        'ElseIf TextBoxReportFile.Text.Trim = String.Empty Then
        '    TextBoxReportFile.Text = repid & ".rpt"
        '    Session("NewFileCreated") = True
        '    Session("FileJustDeleted") = False
        'End If
        'LabelRPT.Text = ErrorLog
        'LabelRPT.Visible = True
    End Sub
    Protected Sub ButtonRDL_Click(sender As Object, e As EventArgs) Handles ButtonRDL.Click
        'NOT IN USE!
        'make xsd
        Dim ret As String = CreateXSD(repid, Session("UserConnString"), Session("UserConnProvider"))

        Dim reporttype As String = DropDownReportAttributes.Text
        Dim mSql As String = String.Empty
        If reporttype = "sql" Then
            mSql = TextBoxSQLorSPtext.Text  'sql statement
        ElseIf reporttype = "sp" Then
            mSql = TextBoxNewSPname.Text
        End If
        repid = Session("REPORTID")
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim rdlpath As String = applpath & "RDLFILES\"
        Dim repfile As String = rdlpath & repid & ".rdl"
        Dim repfilecopy As String = rdlpath & repid & "Copy.rdl"
        Dim ErrorLog = String.Empty
        Dim pageftr As String = Session("PageFtr")
        Dim orien As String = DropDownOrientation.SelectedValue.ToString
        Session("Orientation") = orien

        'if rdl file exist do not overwrite
        If File.Exists(repfile) Then
            ErrorLog = "File " & repid & ".rdl" & " has been copied to the file " & repid & "Copy.rdl"
            TextBoxReportFile.Text = repid & ".rdl"
            If File.Exists(repfilecopy) Then
                File.Delete(repfilecopy)
            End If
            File.Copy(repfile, repfilecopy)
            'HyperLink5.NavigateUrl = "~/ReportEdit.aspx?DELREP=yes&REPTYPE=rdl&REPORT=" & repid
        End If
        Dim repttl = Session("REPTITLE")
        Dim ReportTemplate As String = "Tabular"
        Dim dri As DataTable = GetReportInfo(repid)

        If dri IsNot Nothing AndAlso dri.Rows.Count > 0 Then
            If Not IsDBNull(dri.Rows(0)("Param0type")) AndAlso dri.Rows(0)("Param0type") <> "" Then
                ReportTemplate = dri.Rows(0)("Param0type").ToString
            End If
        End If
        If ReportCreatedByDesigner(repid) Then
            If ReportTemplate.ToUpper = "FREEFORM" Then
                ErrorLog = CreateFreeFormReportForXSDByDesigner(repid, repttl, rdlpath, mSql, "sql", orien, pageftr)
            Else
                ErrorLog = CreateTabularReportForXSDByDesigner(repid, repttl, rdlpath, mSql, "sql", orien, pageftr)
            End If
        Else
            ErrorLog = CreateRDLReportForXSD(repid, repttl, rdlpath, mSql, reporttype, orien, pageftr)  'Button NOT IN USE!
        End If

        If ErrorLog.StartsWith("ERROR!!") Then
            LabelRDL.ForeColor = Drawing.Color.DeepPink
            Session("NewFileCreated") = False
        ElseIf TextBoxReportFile.Text.Trim = String.Empty Then
            TextBoxReportFile.Text = repid & ".rdl"
            Session("NewFileCreated") = True
            Session("FileJustDeleted") = False
        End If
        LabelRDL.Text = ErrorLog
        LabelRDL.Visible = True
    End Sub
    Private Sub LockReport(ReportID As String)
        If ReportID <> "" Then
            ExequteSQLquery("UPDATE OURReportInfo SET Param7type='standard' WHERE ReportID='" & ReportID & "'")
            Response.Redirect("ListOfReports.aspx")
        End If
    End Sub
    Protected Sub ButtonUploadRDL_Click(sender As Object, e As EventArgs) Handles ButtonUploadRDL.Click
        Dim ErrorLog = String.Empty
        If FileRDL.HasFile Then
            'If Not FileRDL.PostedFile.FileName.ToUpper.EndsWith(".RDL") AndAlso Not FileRDL.PostedFile.FileName.ToUpper.EndsWith(".RPT") Then
            If Not FileRDL.PostedFile.FileName.ToUpper.EndsWith(".RDL") Then
                ErrorLog = "RDL file is not selected."
                MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Session("NewFileCreated") = False
                btnBrowse.Focus()
                Exit Sub
            End If
            repid = Session("REPORTID")
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            If FileRDL.PostedFile.FileName.ToUpper.EndsWith(".RDL") Then   'AndAlso Session("CSV") <> "yes"
                Dim rdlpath As String = applpath & "RDLFILES\"
                Dim fpath As String = applpath & "Temp\"
                Dim repfile As String = rdlpath & repid & ".rdl"
                Dim repfilecopy As String = rdlpath & repid & "Copy.rdl"
                Dim rep As String = String.Empty
                'if rdl file exist make a copy
                If File.Exists(repfile) Then
                    ErrorLog = "File " & repid & ".rdl" & " has been copied to the file " & repid & "Copy.rdl"
                    TextBoxReportFile.Text = repid & ".rdl"
                    If File.Exists(repfilecopy) Then
                        File.Delete(repfilecopy)
                    End If
                    File.Copy(repfile, repfilecopy)
                End If
                Dim filepath As String = fpath & FileRDL.PostedFile.FileName
                FileRDL.PostedFile.SaveAs(filepath)
                'check if file is ok
                Dim ret As String = cleanFile(filepath)
                If ret = filepath Then
                    'copy temp file and rename properly to repfile
                    If File.Exists(repfile) Then File.Delete(repfile)
                    File.Copy(filepath, repfile)
                    'delete temp file
                    File.Delete(filepath)
                    'insert rdl into OURFiles 
                    Dim sr As StreamReader = New StreamReader(repfile)
                    Dim rdlstr As String = sr.ReadToEnd()
                    Dim er As String = SaveRDLstringInOURFiles(repid, rdlstr, "uploaded by " & Session("logon"), repfile)  'user just uploaded
                    '!! NOT delete the file from the RDLFILES folder, it is the user one just uploaded
                    ErrorLog = er
                    If ErrorLog <> "Query executed fine." Then
                        MessageBox.Show(ErrorLog, "Import RDL", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Session("NewFileCreated") = False
                        Return
                    End If
                    If TextBoxReportFile.Text.Trim = String.Empty Then
                        TextBoxReportFile.Text = repid & ".rdl"
                        Session("NewFileCreated") = True
                        Session("FileJustDeleted") = False
                    End If
                    MessageBox.Show("Report format file uploaded.", "Import RDL", "upload", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                Else
                    MessageBox.Show(ret, "Import RDL", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Session("NewFileCreated") = False
                    Return
                End If
            ElseIf FileRDL.PostedFile.FileName.ToUpper.EndsWith(".RPT") Then

                TextBoxReportFile.Text = FileRDL.PostedFile.FileName

                Dim rdlpath As String = applpath & "RPTFILES\"
                Dim repfile As String = rdlpath & repid & ".rpt"
                Dim repfilecopy As String = rdlpath & repid & "Copy.rpt"
                'if rpt file exist make a copy
                If File.Exists(repfile) Then
                    ErrorLog = "File " & repid & ".rpt" & " has been copied to the file " & repid & "Copy.rpt"
                    TextBoxReportFile.Text = repid & ".rpt"
                    If File.Exists(repfilecopy) Then
                        File.Delete(repfilecopy)
                    End If
                    File.Copy(repfile, repfilecopy)
                End If

                'insert rpt into OURFiles 
                Dim sr As StreamReader = New StreamReader(FileRDL.FileContent)
                Dim rdlstr As String = sr.ReadToEnd()
                'TODO clean file content
                Dim er As String = SaveRPTstringInOURFiles(repid, rdlstr, Session("logon"), repfile)  'user just uploaded
                If er <> "Uploaded by user" Then
                    WriteToAccessLog(Session("logon"), "Crystal Report file upload failed - " & repfile & " - ERROR: " & er, 1)
                    MessageBox.Show(ErrorLog, "Import RPT", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Session("NewFileCreated") = False
                    Return
                End If
                'save in repfile - THIS IS NEEDED BECAUSE RPT DOES NOT SAVED IN OURFiles(!)
                Dim Bytes() As Byte = FileRDL.FileBytes
                Using Stream As New FileStream(repfile, FileMode.Create)
                    Stream.Write(Bytes, 0, Bytes.Length)
                    Stream.Close()
                    Stream.Dispose()
                End Using
                WriteToAccessLog(Session("logon"), "Crystal Report file uploaded - " & repfile, 1)
                MessageBox.Show("Crystal Report file uploaded.", "Import RPT", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)

            End If
        Else
            ErrorLog = "File Is Not selected."
            MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Session("NewFileCreated") = False
            btnBrowse.Focus()
            Return
        End If
    End Sub
    Protected Sub ButtonUploadRPT_Click(sender As Object, e As EventArgs) Handles ButtonUploadRPT.Click
        Dim ErrorLog = String.Empty
        Dim fpath As String = Trim(FileRPT.Value.ToString)
        If fpath = String.Empty OrElse File.Exists(fpath) = False Then
            ErrorLog = "ERROR!! File Is Not selected."
        ElseIf Not (fpath.EndsWith(".rpt") Or fpath.EndsWith(".RPT")) Then
            ErrorLog = "ERROR!! File Is Not In right format."
        Else
            repid = Session("REPORTID")
            'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
            Dim rptpath As String = applpath & "RPTFILES\"
            Dim repfile As String = rptpath & repid & ".rpt"
            Dim rep As String = String.Empty
            'if rdl file exist do not overwrite
            If File.Exists(repfile) Then
                ErrorLog = "ERROR!! File " & repid & ".rpt" & " already exists. Delete it before creating a New one."
                TextBoxReportFile.Text = repid & ".rpt"
                HyperLink5.NavigateUrl = "~/ReportEdit.aspx?DELREP=yes&REPTYPE=rpt&REPORT=" & repid
            Else
                'check if file is ok
                If cleanFile(fpath) = fpath Then
                    'copy file and rename properly
                    File.Copy(fpath, repfile)
                    ErrorLog = repfile
                End If
                If ErrorLog.StartsWith("ERROR!!") Then
                    LabelRPT.ForeColor = Drawing.Color.DeepPink
                    Session("NewFileCreated") = False
                ElseIf TextBoxReportFile.Text.Trim = String.Empty Then
                    TextBoxReportFile.Text = repid & ".rpt"
                    Session("NewFileCreated") = True
                    Session("FileJustDeleted") = False
                End If
            End If
        End If
        If ErrorLog.StartsWith("ERROR!!") Then
            LabelRPT.ForeColor = Drawing.Color.DeepPink
        End If
        LabelRPT.Text = ErrorLog
        LabelRPT.Visible = True
    End Sub

    Protected Sub MenuMain_MenuItemClick(ByVal sender As Object, ByVal e As MenuEventArgs) Handles MenuMain.MenuItemClick
        Dim i As Integer
        i = Int32.Parse(e.Item.Value)

        If i = 0 Then
            Session("TabN") = 0
            Session("TabNQ") = 0
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 10 Then
            Session("TabN") = 0
            Session("TabNQ") = 0
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 11 Then
            Session("TabN") = 0
            Session("TabNQ") = 1
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 12 Then
            Session("TabN") = 0
            Session("TabNQ") = 2
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 13 Then
            Session("TabN") = 0
            Session("TabNQ") = 3
            Response.Redirect("~/SQLquery.aspx")
        ElseIf i = 1 Then
            Session("TabN") = 1
            Session("TabNF") = 0
            Response.Redirect("~/RDLformat.aspx")
        ElseIf i = 20 Then
            Session("TabN") = 1
            Session("TabNF") = 0
            Response.Redirect("~/RDLformat.aspx")
        ElseIf i = 21 Then
            Session("TabN") = 1
            Session("TabNF") = 1
            Response.Redirect("~/RDLformat.aspx")
        ElseIf i = 22 Then
            Session("TabN") = 1
            Session("TabNF") = 2
            Response.Redirect("~/RDLformat.aspx")
        ElseIf i = 23 Then
            Session("TabN") = 1
            Session("TabNF") = 3
            Response.Redirect("~/ReportDesigner.aspx")
        ElseIf i = 24 Then
            Session("TabN") = 1
            Session("TabNF") = 4
            Response.Redirect("~/MapReport.aspx")
        Else
            MultiView1.ActiveViewIndex = i
            MenuMain.Items(i).Selected = True
            Session("TabN") = MultiView1.ActiveViewIndex
            lblView.Text = MenuMain.Items(Session("TabN")).Text
            Select Case Session("TabN")
                Case "2"
                    HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Report_Info"
                Case "3"
                    HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Parameters"
                Case "4"
                    HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Users"
            End Select
        End If
        LabelAlert.Text = ""
    End Sub
    Protected Sub DropDownSPs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownSPs.SelectedIndexChanged
        Session("SPname") = DropDownSPs.SelectedValue
        Session("SPtext") = DropDownSPs.SelectedItem.Text
        Dim sptext As String = String.Empty
        Dim ret As String = String.Empty
        Try
            If DropDownReportAttributes.Text = "sp" Then
                'check if parameters added in the list of report parameters
                Session("SPparameters") = GetListOfStoredProcedureParameters(Session("SPname").ToString, Session("UserConnString"), Session("UserConnProvider"))
                sptext = GetStoredProcedureText(Session("SPname").ToString, Session("UserConnString"), Session("UserConnProvider"))
                TextBoxSQLorSPtext.Text = sptext
                'define parameters
                'delete existing parameters
                Dim sqltext As String = String.Empty
                sqltext = "DELETE FROM OURReportShow WHERE ReportId='" & Session("REPORTID") & "'"
                ret = ExequteSQLquery(sqltext)
                'insert new parameters
                Dim i As Integer = 0
                If ret = "Query executed fine." Then
                    Dim params As ListItemCollection = New ListItemCollection
                    If Not Session("SPparameters") Is Nothing Then
                        params = Session("SPparameters")
                    End If
                    Dim mSQL As String = String.Empty
                    For i = 0 To params.Count - 1
                        If params(i).Text.Trim <> "" Then
                            mSQL = "SELECT DISTINCT [your field] AS " & params(i).Text & " FROM [your table] ORDER BY " & params(i).Text
                            If Not HasRecords("SELECT * FROM OURReportShow WHERE ReportId='" & Session("REPORTID") & "' AND DropDownID='" & params(i).Text & "'") Then
                                sqltext = "INSERT INTO OURReportShow (ReportID,DropDownID,DropDownLabel,DropDownName,DropDownFieldName,DropDownFieldType,DropDownFieldValue,DropDownSQL,COMMENTS,PrmOrder) VALUES('" & Session("REPORTID") & "','" & params(i).Text & "','" & params(i).Text & "','" & params(i).Text & "','" & params(i).Text & "','" & params(i).Value & "','" & params(i).Text & "','" & mSQL & "','manual'," & i.ToString & ")"
                                ret = ExequteSQLquery(sqltext)
                            End If
                        End If
                    Next
                End If
                Dim delSQL As String = String.Empty
                'delete from OURReportFormat
                delSQL = "DELETE FROM OURReportFormat WHERE ReportID='" & Session("REPORTID") & "'"
                ret = ExequteSQLquery(delSQL)
                'delete from ourreportgroups
                delSQL = "DELETE FROM OURReportGroups WHERE ReportID='" & Session("REPORTID") & "'"
                ret = ExequteSQLquery(delSQL)
                'delete from ourreportlists
                delSQL = "DELETE FROM OURReportLists WHERE ReportID='" & Session("REPORTID") & "'"
                ret = ExequteSQLquery(delSQL)

                ButtonSubmit_Click(sender, e)
                Session("AdvancedUser") = True
                If i > 1 Then
                    LabelAlert.ForeColor = Color.Red
                    LabelAlert.Text = "Go to Parameters page to complete parameters definition (sql statements, labels). "
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
    End Sub
    Protected Sub ButtonDownloadFile_Click(sender As Object, e As EventArgs) Handles ButtonDownloadFile.Click
        Dim ErrorLog = String.Empty
        repid = Session("REPORTID")
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim repfile As String = String.Empty
        Dim downrepzip As String = "y" & repid & "Dnld.zip"
        Dim dirpath As String = applpath & "TEMP\" & repid & "\"
        Directory.CreateDirectory(dirpath)
        repfile = CreateZipFile()
        If repfile = dirpath & downrepzip Then
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & downrepzip)
                Response.TransmitFile(dirpath & downrepzip)
            Catch ex As Exception
                ErrorLog = "ERROR!! " & ex.Message
                LabelAlert.Text = ErrorLog
                LabelAlert.Visible = True
                LabelAlert.Enabled = True
            End Try
            Response.End()
        Else
            LabelAlert.Text = repfile
            LabelAlert.ForeColor = Color.Red
            LabelAlert.Visible = True
            LabelAlert.Enabled = True
        End If

    End Sub
    Private Function CreateZipFile() As String
        Dim i As Integer
        Dim ret As String = String.Empty
        Dim ErrorLog = String.Empty
        repid = Session("REPORTID")
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim reppath As String = String.Empty
        Dim repfile As String = String.Empty
        Dim extfile As String = String.Empty
        Dim downrepfile As String = String.Empty
        Dim downrepzip As String = "y" & repid & "Dnld.zip" 'name of zip file should stay last in directory sorted by alphabetical order (!!)
        Dim rdlstr As String = String.Empty
        Dim rdlfile As String = String.Empty
        Dim dirpath As String = applpath & "TEMP\" & repid & "\"
        Directory.CreateDirectory(dirpath)
        If File.Exists(dirpath & downrepzip) Then
            File.Delete(dirpath & downrepzip)
        End If
        repfile = repid & "Dnld"
        'create the zip file 
        Dim dv As DataView = mRecords("Select * FROM OURFiles WHERE ReportId='" & repid & "'")
        If Not dv Is Nothing AndAlso Not dv.Table Is Nothing AndAlso dv.Table.Rows.Count > 0 Then
            For i = 0 To dv.Table.Rows.Count - 1
                If dv.Table.Rows(i)("Type").ToString.ToUpper = "RPT" Then
                    extfile = ".rpt"
                ElseIf dv.Table.Rows(i)("Type").ToString.ToUpper = "RDL" Then
                    extfile = ".rdl"
                ElseIf dv.Table.Rows(i)("Type").ToString.ToUpper = "XSD" Then
                    extfile = ".xsd"
                End If
                'read rdl,xsd,rpt from OURFiles
                downrepfile = repfile & extfile
                rdlstr = dv.Table.Rows(i)("FileText").ToString  'string with file content
                If rdlstr <> "" Then
                    If extfile = ".rdl" Then
                        'remove connection string
                        Dim s1 As Integer = rdlstr.IndexOf("<ConnectString>")
                        Dim s2 As Integer = rdlstr.IndexOf("</ConnectString>")
                        rdlstr = rdlstr.Substring(0, s1) & "<ConnectString>" & rdlstr.Substring(s2)
                    End If
                    'write the string rdlstr into the file downrepfile
                    ErrorLog = WriteXMLStringToFile(dirpath & downrepfile, rdlstr)
                    If ErrorLog = dirpath & downrepfile Then
                    Else
                        LabelAlert.Text = ErrorLog
                        LabelAlert.Visible = True
                        LabelAlert.Enabled = True
                    End If
                Else
                    ErrorLog = "ERROR!! No file exist."
                    'LabelAlert.ForeColor = Drawing.Color.DeepPink
                    LabelAlert.Text = ErrorLog
                    LabelAlert.Visible = True
                    LabelAlert.Enabled = True
                    Return ret
                    Exit Function
                End If
                'read user rdl from OURFiles
                rdlstr = dv.Table.Rows(i)("UserFile").ToString  'string with file content
                If rdlstr <> "" Then
                    downrepfile = repid & "User" & extfile
                    If extfile = ".rdl" Then
                        'remove connection string
                        Dim s1 As Integer = rdlstr.IndexOf("<ConnectString>")
                        Dim s2 As Integer = rdlstr.IndexOf("</ConnectString>")
                        rdlstr = rdlstr.Substring(0, s1) & "<ConnectString>" & rdlstr.Substring(s2)
                    End If
                    'write the string rdlstr into the file downrepfile
                    ErrorLog = WriteXMLStringToFile(dirpath & downrepfile, rdlstr)
                    If ErrorLog = dirpath & downrepfile Then
                    Else
                        LabelAlert.Text = ErrorLog
                        LabelAlert.Visible = True
                        LabelAlert.Enabled = True
                    End If
                End If
            Next
            Try
                ZipFile.CreateFromDirectory(dirpath, dirpath & downrepzip)
            Catch ex As Exception
            End Try
            ret = dirpath & downrepzip
        End If
        Return ret
    End Function

    Protected Sub DropDownReportType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownReportType.SelectedIndexChanged
        Session("ReportType") = DropDownReportType.Text
    End Sub
    Protected Sub TextBoxReportTitle_TextChanged(sender As Object, e As EventArgs) Handles TextBoxReportTitle.TextChanged
        TextBoxReportTitle.Text = cleanText(TextBoxReportTitle.Text).Replace(":", "-")
        TextBoxReportTitle.TextMode = TextBoxMode.SingleLine
    End Sub
    Private Sub TextBoxReportTitle_Unload(sender As Object, e As EventArgs) Handles TextBoxReportTitle.Unload
        TextBoxReportTitle.Text = cleanText(TextBoxReportTitle.Text).Replace(":", "-")
    End Sub

    Private Sub TextBoxReportTitle_PreRender(sender As Object, e As EventArgs) Handles TextBoxReportTitle.PreRender
        TextBoxReportTitle.Text = cleanText(TextBoxReportTitle.Text).Replace(":", "-")
    End Sub
    Protected Sub TextBoxPageFtr_PreRender(sender As Object, e As EventArgs) Handles TextBoxPageFtr.PreRender
        TextBoxPageFtr.Text = cleanText(TextBoxPageFtr.Text)
    End Sub
    'Protected Sub ButtonHelp_Click(sender As Object, e As EventArgs) Handles ButtonHelp.Click
    '    'OpenWordWithBookmak("DevSiteSupportHelp.doc", "Developer Site Support Help", "ClassData")
    '    Response.Redirect("OnlineUserReporting.pdf")
    'End Sub
    Protected Sub TextBoxPageFtr_TextChanged(sender As Object, e As EventArgs) Handles TextBoxPageFtr.TextChanged
        TextBoxPageFtr.Text = cleanText(TextBoxPageFtr.Text)
    End Sub
    Protected Sub TextBoxID_TextChanged(sender As Object, e As EventArgs) Handles TextBoxID.TextChanged
        TextBoxID.Text = cleanText(TextBoxID.Text)
    End Sub
    Protected Sub TextBoxLabel_TextChanged(sender As Object, e As EventArgs) Handles TextBoxLabel.TextChanged
        TextBoxLabel.Text = cleanText(TextBoxLabel.Text)
    End Sub
    Protected Sub TextBoxField_TextChanged(sender As Object, e As EventArgs) Handles TextBoxField.TextChanged
        TextBoxField.Text = cleanText(TextBoxField.Text)
    End Sub
    Protected Sub TextComments_TextChanged(sender As Object, e As EventArgs) Handles TextComments.TextChanged
        TextComments.Text = cleanText(TextComments.Text)
    End Sub
    Protected Sub TextBoxNetId_TextChanged(sender As Object, e As EventArgs) Handles TextBoxNetId.TextChanged
        TextBoxNetId.Text = cleanText(TextBoxNetId.Text)
    End Sub
    Protected Sub TextBoxComm_TextChanged(sender As Object, e As EventArgs) Handles TextBoxComm.TextChanged
        TextBoxComm.Text = cleanText(TextBoxComm.Text)
    End Sub
    Private Sub txtURI_PreRender(sender As Object, e As EventArgs) Handles txtURI.PreRender
        txtURI.Text = cleanText(txtURI.Text)
        If txtURI.Text.ToUpper.EndsWith(".JSON") OrElse txtURI.Text.ToUpper.EndsWith(".XML") OrElse txtURI.Text.ToUpper.EndsWith(".TXT") OrElse txtURI.Text.ToUpper.EndsWith(".RDL") Then
            txtTableName.Enabled = False
            DropDownTables.Enabled = False
        Else
            txtTableName.Enabled = True
            DropDownTables.Enabled = True
        End If
    End Sub
    Private Sub txtURI_TextChanged(sender As Object, e As EventArgs) Handles txtURI.TextChanged
        txtURI.Text = cleanText(txtURI.Text)
        If txtURI.Text.ToUpper.EndsWith(".JSON") OrElse txtURI.Text.ToUpper.EndsWith(".XML") OrElse txtURI.Text.ToUpper.EndsWith(".TXT") OrElse txtURI.Text.ToUpper.EndsWith(".RDL") Then
            txtTableName.Enabled = False
            DropDownTables.Enabled = False
        Else
            txtTableName.Enabled = True
            DropDownTables.Enabled = True
        End If
    End Sub
    Protected Sub DropDownReportAttributes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownReportAttributes.SelectedIndexChanged
        If DropDownReportAttributes.SelectedValue = "sp" Then
            Session("Attributes") = "sp"
            DropDownSPs.Visible = True
            LabelSP.Visible = True
            Dim i As Integer
            Dim dv As DataView = Nothing
            dv = GetListOfStoredProcedures(Session("UserConnString"), Session("UserConnProvider"))
            If dv Is Nothing Then
                Exit Sub
            End If
            Dim ddt As DataTable = dv.Table
            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data") Then
                Dim id As String = ""
                Dim cls As String = ""
                Dim sp As String = ""
                Dim sql As String = ""
                Dim li As ListItem = Nothing
                Dim dtRoutine As DataTable = Nothing
                Dim err As String = ""

                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    id = ddt.Rows(i)("ROUTINE_NAME")
                    cls = Piece(id, "||", 1)
                    sp = Piece(id, "||", 2)
                    sql = "SELECT ROUTINE_SCHEMA,ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES WHERE CLASSNAME = '" & cls & "' AND METHOD_OR_QUERY_NAME = '" & sp & "'"
                    dtRoutine = mRecords(sql, err, Session("UserConnString"), Session("UserConnProvider")).Table
                    If dtRoutine IsNot Nothing AndAlso dtRoutine.Rows.Count > 0 Then
                        sp = dtRoutine.Rows(0)("ROUTINE_SCHEMA").ToString & "." & dtRoutine.Rows(0)("ROUTINE_NAME").ToString
                        li = New ListItem(sp, id)
                        DropDownSPs.Items.Add(li)
                    End If
                Next
            ElseIf Session("UserConnProvider").ToString.Trim = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    DropDownSPs.Items.Add(Piece(ddt.Rows(i)("PROCEDURE_NAME"), ";", 1))
                Next
            ElseIf Session("UserConnProvider").ToString.Trim = "Oracle.ManagedDataAccess.Client" Then
                Dim SPName As String = String.Empty
                Dim n As Integer = 0
                TextBoxNewSPname.Text = String.Empty
                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    SPName = ddt.Rows(i)("OBJECT_NAME")
                    'stored procedure must have a cursor
                    If SPHasCursor(SPName, Session("UserConnString").ToString, Session("UserConnProvider").ToString) Then
                        DropDownSPs.Items.Add(SPName)
                        If n = 0 Then
                            TextBoxNewSPname.Text = SPName
                            Session("SPname") = SPName
                        End If
                        n += 1
                    End If
                Next
                If TextBoxNewSPname.Text <> String.Empty Then

                    DropDownSPs_SelectedIndexChanged(sender, e)
                End If

                'TODO maybe not needed: ElseIf Session("UserConnProvider").ToString.Trim = "Npgsql" Then  'PostgreSQL  Npgsql

            Else
                For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                    DropDownSPs.Items.Add(ddt.Rows(i)("ROUTINE_NAME"))
                Next
            End If
            TextBoxSQLorSPtext.Wrap = False
        Else
            Session("Attributes") = "sql"
            DropDownSPs.Visible = False
            LabelSP.Visible = False
            TextBoxSQLorSPtext.Wrap = True
            TextBoxSQLorSPtext.Text = ""
        End If

    End Sub
    Private Sub btnDefine_Click(sender As Object, e As EventArgs) Handles btnDefine.Click
        If Session("AdvancedUser") = True And Session("Attributes") = "sql" Then
            MessageBox.OtherButtonText1 = "Select Parameters"
            MessageBox.OtherButtonText2 = "Enter Definition Manually"
            Dim msg As String = "&nbsp;Choose ""Select Parameters"" for SQL based reports only or choose ""Enter Definition Manually"" for stored procedure or SQL based reports.<br/><br/>"
            MessageBox.Show(msg, "Definition Method", "DefinitionMethod", Controls_Msgbox.Buttons.OtherOtherCancel, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.Other1)
        ElseIf Session("AdvancedUser") = False And Session("Attributes") = "sql" Then
            Dim ItemList As ListItemCollection = GetReportFieldItems()
            If ItemList.Item(0).Text = "Error" Then Return
            If ItemList.Count > 0 Then
                ItemList.RemoveAt(0)
                dlgChooseParams.Items = ItemList
                dlgChooseParams.Show("Parameters", "SELECT REPORT PARAMETERS/FILTERS", "Submit Parameters")
            End If
        ElseIf Session("Attributes") = "sp" Then
            'open manual parameter dialog
            If Not Session("SPparameters") Is Nothing Then
                Dim ItemList As ListItemCollection = Session("SPparameters")
                If ItemList.Count > 0 Then
                    dlgEnterParams.FieldItems = ItemList
                    ItemList = New ListItemCollection
                    ItemList.Add(" ")
                    ItemList.Add("nvarchar")
                    ItemList.Add("int")
                    ItemList.Add("datetime")
                    dlgEnterParams.TypeItems = ItemList
                    dlgEnterParams.ReportSQL = GetExpandedReportSQL()
                    dlgEnterParams.Show("Add Parameter", New Controls_ParameterDlg.ParameterData(), Controls_ParameterDlg.Mode.Add, "Add Parameter")
                End If
            End If
        End If
    End Sub
    Private Function IsSelected(rep As String, fld As String) As Boolean
        Dim sql As String = "Select * From ourreportshow Where DropDownID = '" & fld & "' AND ReportId = '" & rep & "'"
        Dim err As String = String.Empty
        Dim dt As DataTable = mRecords(sql, err).Table
        If err = String.Empty AndAlso dt.Rows.Count = 1 Then
            Return True
        ElseIf err = String.Empty AndAlso dt.Rows.Count = 0 Then
            'check if old style parameter
            Dim n As Integer = Pieces(fld, ".")
            If n > 1 Then
                Dim f As String = Piece(fld, ".", n)
                sql = "Select * From ourreportshow Where DropDownID = '" & f & "' AND ReportId = '" & rep & "'"
                dt = mRecords(sql, err).Table
                If err = String.Empty AndAlso dt.Rows.Count = 1 Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Else
            If err <> String.Empty Then MessageBox.Show(err, "IsSelected", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
            Return False
        End If
    End Function
    Private Function GetExpandedFields() As Hashtable
        Dim htFields As New Hashtable
        Dim ExpandedSql As String = GetExpandedReportSQL()
        Dim SelectList As String = String.Empty
        Dim n As Integer = 0
        Dim i As Integer
        If ExpandedSql <> String.Empty Then
            SelectList = Piece(Piece(ExpandedSql, "SELECT ", 2), " FROM ", 1).Trim   'fixing by additing spaces
            Dim DistinctIndex As Integer = SelectList.ToUpper.IndexOf("DISTINCT ")
            If DistinctIndex > -1 Then SelectList = SelectList.Substring(DistinctIndex + 9).Trim
            Dim sFields As String() = Split(SelectList, ",")
            Dim sFld As String = String.Empty
            For i = 0 To sFields.Count - 1
                n = Pieces(sFields(i), ".")
                sFld = Piece(sFields(i).Trim, ".", n)
                If sFld.Contains(" AS ") Then sFld = Piece(sFld, " AS ", 2)
                htFields.Add(sFld.Replace("[", "").Replace("]", "").Replace("`", "").Replace("""", ""), sFields(i).Trim.Replace("[", "").Replace("]", "").Replace("`", "").Replace("""", ""))
            Next
        End If
        Return htFields
    End Function
    Private Function GetReportFieldItems() As ListItemCollection
        repid = Session("REPORTID")
        Dim dvm As DataView
        Dim par, ret As String
        Dim i As Integer
        Dim ItemList As New ListItemCollection
        'all parameters
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
        dvm = ReturnDataViewFromXML(xsdfile)

        If dvm Is Nothing OrElse dvm.Table Is Nothing Then
            'If dvm Is Nothing OrElse dvm.Count = 0 Then
            Try
                Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                'update XSD
                Dim xsdpath As String = applpath & "XSDFILES\"
                Dim err As String = String.Empty
                Dim dt As DataTable = mRecords(sqlquerytext, err, Session("UserConnString"), Session("UserConnProvider")).Table
                If err <> String.Empty Then
                    MessageBox.Show(err, "Get Report Field Items", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    ItemList.Add(New ListItem("Error"))
                    Return ItemList
                End If
                ret = CreateXSDForDataTable(dt, repid, xsdpath)
                dvm = ReturnDataViewFromXML(xsdfile)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Get Report Field Items", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                ItemList.Add(New ListItem("Error"))
                Return ItemList
            End Try
        End If
        Dim li As ListItem
        ItemList.Add(New ListItem(" "))

        Dim htFields As Hashtable = GetExpandedFields()
        Dim typ As String = String.Empty
        For i = 0 To dvm.Table.Columns.Count - 1
            'par = FixReservedWords(dvm.Table.Columns(i).Caption)
            par = dvm.Table.Columns(i).Caption
            If htFields(par) IsNot Nothing AndAlso
               htFields(par).ToString <> String.Empty Then
                par = htFields(par).ToString
                typ = dvm.Table.Columns(i).DataType.FullName
                li = New ListItem(par, typ)
                li.Selected = IsSelected(repid, li.Text)
                ItemList.Add(li)
            End If
        Next
        Return ItemList
    End Function
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Select Case e.Tag
            Case "DefinitionMethod"
                Dim ItemList As ListItemCollection = GetReportFieldItems()
                If ItemList.Item(0).Text = "Error" Then Return

                Select Case e.Result
                    Case Controls_Msgbox.MessageResult.Other1
                        If ItemList.Count > 0 Then
                            ItemList.RemoveAt(0)
                            dlgChooseParams.Items = ItemList
                            dlgChooseParams.Show("Parameters", "SELECT REPORT PARAMETERS/FILTERS", "Submit Parameters")
                        End If
                    Case Controls_Msgbox.MessageResult.Other2
                        If ItemList.Count > 0 Then
                            dlgEnterParams.FieldItems = ItemList
                            ItemList = New ListItemCollection
                            ItemList.Add(" ")
                            ItemList.Add("nvarchar")
                            ItemList.Add("int")
                            ItemList.Add("datetime")
                            dlgEnterParams.TypeItems = ItemList
                            dlgEnterParams.ReportSQL = GetExpandedReportSQL()
                            dlgEnterParams.IsCacheDB = (Session("UserConnProvider").ToString.ToUpper.StartsWith("INTERSYSTEMS.DATA"))
                            dlgEnterParams.IsOracleDB = (Session("UserConnProvider").ToString.ToUpper = "ORACLE.MANAGEDDATAACESS.CLIENT")
                            dlgEnterParams.Show("Add Parameter", New Controls_ParameterDlg.ParameterData(), Controls_ParameterDlg.Mode.Add, "Add Parameter")
                        End If
                End Select
            Case "ParamsAdded"
                'Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
            Case "upload"
                LockReport(Session("ReportID"))
            Case "ClearTable"
                Select Case e.Result
                    Case Controls_Msgbox.MessageResult.Cancel
                        chkboxClearTable.Checked = False
                End Select
            Case "FileSizeExceeded"
                If Session("admin") <> "super" Then
                    FileRDL = Nothing
                    Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                End If

            Case "Import File"
                Session("NewFileCreated") = False
                btnBrowse.Focus()
            Case "FieldsNotChosen"
                Response.Redirect("SQLquery.aspx?tnq=0")
            Case "ColumnsNotDefined"
        End Select
    End Sub
    Private Function EditParameterData(ParamID As String, Data As Controls_ParameterDlg.ParameterData) As Boolean
        Dim sqlq As String = Data.ParameterSQL
        Dim tblfield As String = Data.Field
        Dim field As String = String.Empty
        Dim label As String = Data.Label
        Dim param As String = Data.Parameter
        Dim paramtype As String = Data.ParameterType
        Dim paramcomment As String = Data.ParameterComments
        Dim n As Integer = Pieces(tblfield, ".")

        field = Piece(tblfield, ".", n)

        'check if old DropDownID was the field only - this will happen if old style parameter is edited.
        Dim dt As DataTable = mRecords("Select DropDownID FROM OURReportShow WHERE DropDownID='" & field & "' AND ReportId='" & repid & "'").Table
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then ParamID = field

        sqlq = sqlq.Replace("'", """")
        Dim updSQL As String = "UPDATE OURReportShow "
        'updSQL &= "SET ReportID='" & Session("REPORTID") & "',"
        updSQL &= "SET DropDownID='" & tblfield & "',"
        updSQL &= "DropDownLabel='" & label & "',"
        updSQL &= "DropDownName='" & param & "',"
        updSQL &= "DropDownFieldName='" & field & "',"
        updSQL &= "DropDownFieldType='" & paramtype & "',"
        updSQL &= "DropDownFieldValue='" & field & "',"
        updSQL &= "DropDownSQL='" & sqlq & "',"
        updSQL &= "COMMENTS='" & paramcomment & "' "
        updSQL &= "Where DropDownID='" & ParamID & "'" & " And ReportID = '" & Session("REPORTID") & "'"
        Dim ret As String = ExequteSQLquery(updSQL)
        If ret <> "Query executed fine." Then
            MessageBox.Show(ret, "Edit Parameter - " & ParamID, "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return False
        End If
        Dim logs As String = "Parameter " & param & " for report " & Session("REPORTID") & " has been edited."
        WriteToAccessLog(Session("logon"), logs, 2)
        Return True
    End Function
    Private Function AddParameterData(Data As Controls_ParameterDlg.ParameterData) As Boolean
        Dim sqlq As String = Data.ParameterSQL
        Dim tblfield As String = Data.Field
        Dim field As String = String.Empty
        Dim label As String = Data.Label
        Dim param As String = Data.Parameter
        Dim paramtype As String = Data.ParameterType
        Dim paramcomment As String = Data.ParameterComments
        Dim n As Integer = Pieces(tblfield, ".")

        field = Piece(tblfield, ".", n)

        'If sqlq = String.Empty Then
        '    sqlq = GetExpandedReportSQL()
        '    If sqlq <> String.Empty Then
        '        sqlq = "SELECT DISTINCT sub." & field & " FROM (" & sqlq & ") sub"
        '    End If
        'End If
        sqlq = sqlq.Replace("'", """")
        'Dim SQLqFrom As String = sqlq.Substring(sqlq.ToUpper.IndexOf(" FROM"))
        'Dim SQLqSelect As String = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" FROM"))
        'If IsCacheDatabase() Then
        '    'Remove TOP n, because Cache working strange if included sql has TOP n , it return subset of distinct values
        '    Dim k As Integer = SQLqSelect.ToUpper.IndexOf(" TOP ")
        '    If k > 0 Then
        '        SQLqSelect = SQLqSelect.Substring(k + 5).Trim
        '        SQLqSelect = SQLqSelect.Substring(SQLqSelect.IndexOf(" "))
        '        SQLqSelect = " SELECT " & SQLqSelect
        '    End If
        'End If
        'SQLqSelect = FixDoubleFieldNames(repid, SQLqSelect)
        'sqlq = SQLqSelect & " " & SQLqFrom
        'Dim mSQL As String = "SELECT DISTINCT sub." & field & " FROM (" & sqlq & ") sub"
        Dim insSQL As String
        '= "INSERT INTO OURReportShow "
        'insSQL &= "SET ReportID='" & Session("REPORTID") & "',"
        'insSQL &= "DropDownID='" & field & "',"
        'insSQL &= "DropDownLabel='" & label & "',"
        'insSQL &= "DropDownName='" & field & "',"
        'insSQL &= "DropDownFieldName='" & param & "',"
        'insSQL &= "DropDownFieldType='" & paramtype & "',"
        'insSQL &= "DropDownFieldValue='" & field & "',"
        'insSQL &= "DropDownSQL='" & sqlq & "',"
        'insSQL &= "COMMENTS='" & paramcomment & "'"

        insSQL = "INSERT INTO OURReportShow (ReportID,DropDownID,DropDownLabel,DropDownName,DropDownFieldName,DropDownFieldType,DropDownFieldValue,DropDownSQL,COMMENTS) "
        insSQL &= " VALUES('" & Session("REPORTID") & "','" & tblfield & "','" & label & "','" & param & "','" & field & "','" & paramtype & "','" & field & "','" & sqlq & "','" & paramcomment & "')"

        Dim ret As String = ExequteSQLquery(insSQL)

        Dim logs As String = "Parameter " & param & " for report " & Session("REPORTID") & " has been created."
        If ret <> "Query executed fine." Then
            logs = ret
            MessageBox.Show(ret, "Add Parameter Data", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return False
        End If
        WriteToAccessLog(Session("logon"), logs, 2)
        Return True
    End Function
    Private Sub AddParameter(sqlq As String, param As String, table As String, field As String, typ As String, ByRef sError As String)
        Dim mSQL, lbl, nw, insSQL As String
        Dim tblfld As String = table & "." & field
        'add parameter par into ReportShow table
        'DISTINCT in Cache return upper case and need a correction in where statement!!
        mSQL = sqlq.Replace("'", """")
        'mSQL = "SELECT DISTINCT sub." & param & " FROM (" & sqlq.Replace("'", """") & ") sub"
        'label for dropdown
        nw = ""
        lbl = GetFriendlyFieldName(repid, param, nw)
        If lbl = "" OrElse lbl = "none" Then
            lbl = param
        End If

        If typ = "System.String" Then
            typ = "nvarchar"
        ElseIf typ.ToUpper.Contains("DATETIME") Or typ.ToUpper.Contains("DATE") Or typ.ToUpper.Contains("TIME") Then
            typ = "datetime"
        Else
            typ = "int"
        End If
        insSQL = "INSERT INTO OURReportShow (ReportID,DropDownID,DropDownLabel,DropDownName,DropDownFieldName,DropDownFieldType,DropDownFieldValue,DropDownSQL,COMMENTS) "
        insSQL &= " VALUES('" & Session("REPORTID") & "','" & tblfld & "','" & lbl & "','" & param & "','" & field & "','" & typ & "','" & field & "','" & mSQL & "','checked')"
        Dim ret As String = String.Empty

        ret = ExequteSQLquery(insSQL)

        If ret <> "Query executed fine." Then
            sError = "&nbsp;&nbsp;Error occurred - " & param & ": " & ret
            WriteToAccessLog(Session("logon"), sError, 1)
            Exit Sub
        End If
        Dim logs As String = "Parameter " & param & " For report " & Session("REPORTID") & " has been created."
        WriteToAccessLog(Session("logon"), logs, 2)
        ''send emails
        'Dim EmailTable As DataTable
        'EmailTable = mRecords("SELECT  * FROM OURPermissions WHERE APPLICATION='InteractiveReporting' AND Param2='email' AND (Param1='" & Session("REPORTID") & "') AND AccessLevel='admin'").Table
        'If EmailTable.Rows.Count > 0 Then
        '    For j = 0 To EmailTable.Rows.Count - 1
        '        SendHTMLEmail("", "Parameter for report " & Session("REPORTID") & " has been created", "Parameter " & par & " has been created by NetId " & Session("logon"), EmailTable.Rows(j)("Param3"), Session("SupportEmail"))
        '    Next
        'End If
        ' continue

    End Sub
    Private Sub dlgChooseParams_DialogResulted(sender As Object, e As Controls_CheckboxDialog.DlgBoxEventArgs) Handles dlgChooseParams.DialogResulted
        If e.Result = Controls_CheckboxDialog.DialogResult.OK Then
            Dim SelectedItems As ListItemCollection = e.SelectedItems
            Dim htParams As New Hashtable
            Dim htTableField As New Hashtable
            Dim dvp As DataView = Nothing
            Dim dvr As DataView = Nothing
            Dim SQLq As String = String.Empty
            Dim par As String = String.Empty
            Dim err As String = String.Empty
            Dim sError As String = String.Empty
            Dim i As Integer

            repid = Session("REPORTID")
            'SQLq = GetExpandedReportSQL()

            'get results
            Dim li As ListItem
            Dim sItem As String = String.Empty
            Dim sTable As String = String.Empty
            Dim sField As String = String.Empty
            Dim n As Integer = 0
            For i = 0 To SelectedItems.Count - 1
                li = SelectedItems.Item(i)
                sItem = li.Text
                If sItem.Contains(" AS ") Then
                    sItem = Piece(sItem, " AS ", 2)
                ElseIf sItem.Contains(".") Then
                    n = Pieces(sItem, ".")
                    sItem = Piece(sItem, ".", n)
                End If
                htParams.Add(sItem, li.Value)
                htTableField.Add(sItem, li.Text)
            Next
            dvp = mRecords("SELECT DropDownID, DropDownLabel, DropDownName, DropDownFieldType, DropDownSQL, comments FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
            n = dvp.Table.Rows.Count
            For i = 0 To n - 1
                par = dvp.Table.Rows(i)("DropDownName")
                If htParams(par) IsNot Nothing AndAlso htParams(par) <> "" Then
                    htParams.Remove(par)
                    htTableField.Remove(par)
                End If
            Next
            If htParams.Count > 0 Then

                For Each key As DictionaryEntry In htParams
                    par = key.Key
                    sItem = htTableField(par).ToString
                    If sItem.Contains(".") Then
                        n = Pieces(sItem, ".")
                        sTable = Piece(sItem, ".", 1, n - 1)
                        sField = Piece(sItem, ".", n)
                        sField = sField.Replace("`", "")
                        If sField.Contains(" AS ") Then
                            sField = Piece(sField, " AS ", 1)
                        End If
                        SQLq = "SELECT DISTINCT " & FixReservedWords(sField, Session("UserConnProvider")) & " FROM " & CorrectTableNameWithDots(FixReservedWords(sTable, Session("UserConnProvider")), Session("UserConnProvider")) & " ORDER BY " & FixReservedWords(sField, Session("UserConnProvider"))

                    Else
                        Continue For
                    End If
                    AddParameter(SQLq, par, sTable, sField, key.Value, err)
                    If err <> String.Empty Then
                        If sError = String.Empty Then
                            sError = err
                        Else
                            sError &= "<br/>" & err
                        End If
                        err = String.Empty
                    End If
                Next
                If sError <> String.Empty Then
                    MessageBox.Show(sError, "Add Parameter Error", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                    Exit Sub
                Else
                    Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                End If
            End If
        End If
    End Sub
    Protected Sub btnUP_Click(sender As Object, e As EventArgs)
        repid = Session("REPORTID")
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim n As Integer = Pieces(sqlindx, ".")
        Dim ddid As String = Piece(sqlindx, ".", n)
        Dim ddid2 As String = Piece(sqlindx, "up_", 2)
        Dim dt As DataTable = UpDtRow(dvp.Table, "PrmOrder", ddid2)
        UpdateParamsOrderInDB(repid, dt)
        Response.Redirect("ReportEdit.aspx")
    End Sub
    Protected Sub btnDOWN_Click(sender As Object, e As EventArgs)
        repid = Session("REPORTID")
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim n As Integer = Pieces(sqlindx, ".")
        Dim ddid As String = Piece(sqlindx, ".", n)
        Dim ddid2 As String = Piece(sqlindx, "down_", 2)
        Dim dt As DataTable = DownDtRow(dvp.Table, "PrmOrder", ddid2)
        UpdateParamsOrderInDB(repid, dt)
        Response.Redirect("ReportEdit.aspx")
    End Sub
    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        repid = Session("REPORTID")
        Dim ItemList As ListItemCollection
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim n As Integer = Pieces(sqlindx, ".")
        Dim ddid As String = Piece(sqlindx, ".", n)
        Dim ddid2 As String = Piece(sqlindx, "edit_", 2)
        Dim ParamData As New Controls_ParameterDlg.ParameterData
        Dim dtbl As DataTable = mRecords("SELECT DropDownID, DropDownLabel, DropDownName, DropDownFieldType, DropDownSQL, comments FROM OURReportShow WHERE (ReportID='" & repid & "' AND DropDownID='" & ddid2 & "') ORDER BY Indx").Table

        If Not sqlindx.Contains(".") Then ddid = ddid2

        If Not ddid2.Contains(".") Then
            Dim tbl As String = FindTableToTheField(repid, ddid, Session("UserConnString"), Session("UserConnProvider"))
            If tbl <> String.Empty Then ddid2 = tbl & "." & ddid
        End If

        If Session("SPparameters") Is Nothing Then
            Dim htFields As Hashtable = GetExpandedFields()
            If htFields(ddid) IsNot Nothing Then ParamData.Field = htFields(ddid)
            ItemList = GetReportFieldItems()
        Else  'sp
            ItemList = Session("SPparameters")
        End If
        If dtbl.Rows.Count = 1 Then
            ParamData.Field = ddid2 'dtbl.Rows(0)("DropDownID")
            ParamData.Label = dtbl.Rows(0)("DropDownLabel").ToString
            ParamData.Parameter = dtbl.Rows(0)("DropDownName").ToString
            ParamData.ParameterType = dtbl.Rows(0)("DropDownFieldType").ToString
            ParamData.ParameterSQL = dtbl.Rows(0)("DropDownSQL").ToString.Replace("""", "'")
            ParamData.ParameterComments = dtbl.Rows(0)("comments").ToString
        End If
        If ItemList.Item(0).Text = "Error" Then Return
        If ItemList.Count > 0 Then
            dlgEnterParams.FieldItems = ItemList
            ItemList = New ListItemCollection
            ItemList.Add(" ")
            ItemList.Add("nvarchar")
            ItemList.Add("int")
            ItemList.Add("datetime")
            dlgEnterParams.TypeItems = ItemList
            dlgEnterParams.ReportSQL = GetExpandedReportSQL()
            dlgEnterParams.IsCacheDB = IsCacheDatabase()
            dlgEnterParams.Show("Edit Parameter - " & ddid, ParamData, Controls_ParameterDlg.Mode.Edit, "Update Parameter")
        End If
        'To do: finish edit
    End Sub

    Private Function GetExpandedReportSQL() As String
        Dim ret As String = String.Empty
        repid = Session("REPORTID")

        Dim dvr As DataView = mRecords("Select * FROM OURReportInfo WHERE (ReportID='" & repid & "')")
        If dvr.Table.Rows.Count = 1 Then
            If dvr.Table.Rows(0)("ReportAttributes").ToString = "sql" Then
                ret = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider")) 'dvr.Table.Rows(0)("SQLquerytext").ToString
                If ret.ToUpper.IndexOf(" FROM ") < 0 Then
                    Return ret
                End If
                Dim SQLqFrom As String = ret.Substring(ret.ToUpper.IndexOf(" FROM "))
                If SQLqFrom.ToUpper.IndexOf(" ORDER BY ") > 0 Then SQLqFrom = SQLqFrom.Substring(0, SQLqFrom.ToUpper.IndexOf(" ORDER BY "))
                Dim SQLqSelect As String = ret.Substring(0, ret.ToUpper.IndexOf(" FROM"))
                'Dim SQLqSelect As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                Dim SQLqWhere As String = String.Empty
                Dim idxWhere As Integer = SQLqFrom.ToUpper.IndexOf(" WHERE ")
                If idxWhere > -1 Then
                    Dim sFrom As String = SQLqFrom.Substring(0, idxWhere)
                    SQLqWhere = SQLqFrom.Substring(idxWhere).Replace("""", "'")
                    SQLqFrom = sFrom & SQLqWhere
                End If
                If IsCacheDatabase(Session("UserConnProvider")) Then
                    'Remove TOP n, because Cache working strange if included sql has TOP n , it return subset of distinct values
                    Dim k As Integer = SQLqSelect.ToUpper.IndexOf(" TOP ")
                    If k > 0 Then
                        SQLqSelect = SQLqSelect.Substring(k + 5).Trim
                        SQLqSelect = SQLqSelect.Substring(SQLqSelect.IndexOf(" "))
                        SQLqSelect = " SELECT " & SQLqSelect
                    End If
                End If
                SQLqSelect = FixDoubleFieldNames(repid, SQLqSelect, Session("UserConnProvider"))
                ret = SQLqSelect & " " & SQLqFrom
            ElseIf dvr.Table.Rows(0)("ReportAttributes").ToString = "sp" Then
                'TODO for sp

            End If
        End If

        Return ret
    End Function
    Private Function ParameterExists(param As String) As Boolean
        repid = Session("REPORTID")
        Dim ret As Boolean = False
        Dim dtbl As DataTable = mRecords("SELECT DropDownID FROM OURReportShow WHERE (ReportID='" & repid & "' AND DropDownID='" & param & "')").Table
        If dtbl.Rows.Count > 0 Then
            Return True
        Else
            'Check for old style parameter
            Dim n As Integer = Pieces(param, ".")
            If n > 1 Then
                Dim f As String = Piece(param, ".", n)
                dtbl = mRecords("SELECT DropDownID FROM OURReportShow WHERE (ReportID='" & repid & "' AND DropDownID='" & f & "')").Table
                If dtbl.Rows.Count > 0 Then Return True
            End If
        End If
        Return ret
    End Function
    Private Sub dlgEnterParams_ParamDialogResulted(sender As Object, e As Controls_ParameterDlg.ParamDlgBoxEventArgs) Handles dlgEnterParams.ParamDialogResulted
        Select Case e.Result
            Case Controls_ParameterDlg.ParamDialogResult.OK
                Dim pdata As Controls_ParameterDlg.ParameterData = e.ParameterItem
                Dim OldData As Controls_ParameterDlg.ParameterData = e.OldParameterItem
                repid = Session("REPORTID")

                If e.EntryMode = Controls_ParameterDlg.Mode.Add Then
                    If Not ParameterExists(pdata.Field) Then
                        If AddParameterData(pdata) Then
                            Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                        End If
                    Else
                        MessageBox.Show("Parameter """ & pdata.Field & " already exists.""", "Add New Parameter", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                    End If
                ElseIf e.EntryMode = Controls_ParameterDlg.Mode.Edit Then
                    If OldData.Field.Contains(" AS ") Then
                        OldData.Field = Piece(OldData.Field, " AS ", 1)
                        'ElseIf OldData.Field.Contains(".") Then
                        '    OldData.Field = Piece(OldData.Field, ".", Pieces(OldData.Field, "."))
                    End If
                    If pdata.Field.Contains(" AS ") Then
                        pdata.Field = Piece(pdata.Field, " AS ", 1)
                        'ElseIf pdata.Field.Contains(".") Then
                        '    pdata.Field = Piece(pdata.Field, ".", Pieces(pdata.Field, "."))
                    End If
                    If pdata.Field <> OldData.Field Then
                        If ParameterExists(pdata.Field) Then
                            MessageBox.Show("Parameter for field """ & pdata.Field & """ has already been defined.", "Edit Parameter - " & OldData.Field, "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                            Return
                        End If
                    End If
                    If EditParameterData(OldData.Field, pdata) Then
                        Response.Redirect("ReportEdit.aspx?Report=" & repid & "&repedit=yes")
                    End If
                End If
        End Select
    End Sub

    Private Sub chkAdvanced_PreRender(sender As Object, e As EventArgs) Handles chkAdvanced.PreRender
        If chkAdvanced.Checked Then
            Session("AdvancedUser") = True
            trEditRep0.Visible = True
            If Request("repedit") = "new" AndAlso Session("CSV") = "yes" Then
                trEditRep3.Visible = False
                trEditRep1.Visible = False
                trEditRep5.Visible = False
            Else
                trEditRep3.Visible = True
                trEditRep1.Visible = True
                trEditRep5.Visible = True
            End If
            If Session("CSV") = "yes" Then
                trEditRep2.Visible = False
                If TextBoxSQLorSPtext.Text.Trim = "" OrElse TextBoxSQLorSPtext.Text.Trim = "SELECT" OrElse TextBoxSQLorSPtext.Text.ToUpper.Trim = "SELECT * FROM" Then
                    trEditRep3.Visible = False
                End If
            ElseIf Session("UserConnProvider").ToString.Trim = "System.Data.Odbc" OrElse Session("UserConnProvider") = "System.Data.OleDb" Then
                trEditRep2.Visible = False
            Else
                trEditRep2.Visible = True
            End If
            trEditRep4.Visible = False
        Else
            Session("AdvancedUser") = False
            trEditRep0.Visible = False
            trEditRep2.Visible = False
            trEditRep3.Visible = False
            trEditRep4.Visible = False
        End If

    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        'Dim url As String = "~/" & node.Value
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

    'Private Sub FileRDL_Unload(sender As Object, e As EventArgs) Handles FileRDL.Unload
    '    If Not FileRDL.PostedFile Is Nothing Then
    '        TextBoxReportFile.Text = FileRDL.PostedFile.FileName
    '    End If

    'End Sub

    'Private Sub mnuDefine_MenuItemClick(sender As Object, e As MenuEventArgs) Handles mnuDefine.MenuItemClick
    '    If e.Item.Value = "01" Then  'Select Parameters
    '        repid = Session("REPORTID")
    '        Dim dvm As DataView
    '        Dim par, ret As String
    '        Dim i As Integer
    '        Dim ItemList As New ListItemCollection
    '        'all parameters
    '        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
    '        Dim xsdfile As String = applpath & "XSDFILES\" & repid & ".xsd"
    '        dvm = ReturnDataViewFromXML(xsdfile)
    '        If dvm Is Nothing OrElse dvm.Count = 0 Then
    '            Try
    '                Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
    '                'update XSD
    '                Dim xsdpath As String = applpath & "XSDFILES\"
    '                Dim err As String = String.Empty
    '                Dim dt As DataTable = mRecords(sqlquerytext, err, Session("UserConnString"), Session("UserConnProvider")).Table
    '                ret = CreateXSDForDataTable(dt, repid, xsdpath)
    '                dvm = ReturnDataViewFromXML(xsdfile)
    '            Catch ex As Exception
    '                MessageBox.Show(ex.Message, "Make SQL Query Error", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
    '                Exit Sub
    '            End Try
    '        End If
    '        Dim li As ListItem
    '        Dim typ As String = String.Empty
    '        For i = 0 To dvm.Table.Columns.Count - 1
    '            par = dvm.Table.Columns(i).Caption
    '            typ = dvm.Table.Columns(i).DataType.FullName
    '            li = New ListItem(par, typ)
    '            ItemList.Add(li)
    '        Next
    '        If ItemList.Count > 0 Then
    '            dlgChooseParams.Items = ItemList
    '            dlgChooseParams.Show("Parameters", "SELECT REPORT PARAMETERS/FILTERS", "Submit Parameters")
    '        End If
    '    ElseIf e.Item.Value = "02" Then 'Manually Enter a parameter

    '    End If
    'End Sub
    Protected Function CorrectParametersOrder(ByVal dt As DataTable, ByVal orderfld As String) As DataTable
        Dim dtt As DataTable = dt
        Dim i As Integer
        'reorder
        For i = 0 To dtt.Rows.Count - 1
            dtt.Rows(i)(orderfld) = i + 1
        Next
        Return dtt
    End Function
    Public Function UpDtRow(ByVal dt As DataTable, ByVal orderfld As String, ByVal uprowid As String) As DataTable
        Dim exm As String = String.Empty
        Dim ord1 As Integer = 0
        Dim ord2 As Integer = 0
        CorrectParametersOrder(dt, orderfld)
        Try
            ord1 = dt.Rows(uprowid - 1)(orderfld)
            ord2 = dt.Rows(uprowid)(orderfld)
            dt.Rows(uprowid - 1)(orderfld) = ord2
            dt.Rows(uprowid)(orderfld) = ord1
        Catch ex As Exception
            exm = ex.Message
        End Try
        Return dt
    End Function
    Public Function DownDtRow(ByVal dt As DataTable, ByVal orderfld As String, ByVal uprowid As String) As DataTable
        Dim exm As String = String.Empty
        Dim ord1 As Integer = 0
        Dim ord2 As Integer = 0
        CorrectParametersOrder(dt, orderfld)
        Try
            ord1 = dt.Rows(uprowid + 1)(orderfld)
            ord2 = dt.Rows(uprowid)(orderfld)
            dt.Rows(uprowid + 1)(orderfld) = ord2
            dt.Rows(uprowid)(orderfld) = ord1
        Catch ex As Exception
            exm = ex.Message
        End Try
        Return dt
    End Function
    Public Function UpdateParamsOrderInDB(ByVal repid As String, ByVal dt As DataTable) As String
        Try
            Dim i As Integer
            Dim sqly As String = String.Empty
            Dim ret As String = String.Empty
            For i = 0 To dt.Rows.Count - 1
                sqly = "UPDATE OURReportShow SET PrmOrder=" & dt.Rows(i)("PrmOrder").ToString & " WHERE ReportId='" & repid & "' AND Indx=" & dt.Rows(i)("Indx").ToString
                ret = ExequteSQLquery(sqly)
                If ret <> "Query executed fine." Then
                    Return ret
                End If
            Next
        Catch ex As Exception
            Return ex.Message
        End Try
        Return ""
    End Function

    Private Sub chkRelatedParameters_CheckedChanged(sender As Object, e As EventArgs) Handles chkRelatedParameters.CheckedChanged
        If chkRelatedParameters.Checked = True Then
            Session("ParamsRelated") = "yes"
        Else
            Session("ParamsRelated") = ""
        End If
        'save to OURreportInfo
        Dim SQLq As String = "UPDATE OURReportInfo SET Param0id='" & Session("ParamsRelated").ToString & "' WHERE (ReportID='" & Session("REPORTID") & "')"
        Dim ret As String = ExequteSQLquery(SQLq)
    End Sub

    Protected Sub ButtonUploadFile_Click(sender As Object, e As EventArgs) Handles ButtonUploadFile.Click
        Dim ret As String = String.Empty
        Dim ErrorLog = String.Empty
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        repid = Session("REPORTID")
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim xstring As String = String.Empty
        Dim exst As Boolean = False
        Dim er As String = String.Empty
        Dim fileExt As String = String.Empty
        Dim d As String = TextboxDelimiter.Text.Trim.Substring(0, 1)
        If d = "" Then d = ","
        Dim ntables As Integer = 0
        Dim dv As DataView
        Dim rtbl As String = String.Empty
        '----------------------------------------------------------------------------
        'name of table
        Dim tblname As String = cleanText(txtTableName.Text.Trim)
        Dim tblfriendlyname As String = cleanText(txtTableName.Text)
        If DropDownTables.Items.Count > 1 AndAlso DropDownTables.SelectedItem.Value.Trim <> "" Then
            tblname = DropDownTables.SelectedItem.Value
            exst = True
        Else
            If tblname <> "" Then
                If TableExists(txtTableName.Text.Trim, Session("UserConnString"), Session("UserConnProvider"), er) AndAlso Not chkboxClearTable.Checked Then
                    tblname = txtTableName.Text.Trim & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "")
                    tblname = tblname.ToLower.Replace(" ", "")
                ElseIf TableExists(txtTableName.Text.Trim, Session("UserConnString"), Session("UserConnProvider"), er) AndAlso chkboxClearTable.Checked Then
                    exst = True
                Else
                    tblname = tblname.ToLower.Replace(" ", "")
                End If
            Else
                tblname = Session("logon") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "")
                tblname = tblname.ToLower.Replace(" ", "")
            End If
        End If
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "UserData." & tblname
            End If
        End If

        ret = InsertTableIntoOURUserTables(tblname, tblfriendlyname, Session("Unit"), Session("logon"), Session("UserDB"), "", repid)
        '----------------------------------------------------------------------------

        'Dim fileuploadDir As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim csvpath As String = applpath & "\" & tblname '& ".csv"

        If txtURI.Text.Trim <> "" AndAlso txtURI.Text.Trim <> "https://" Then   'web file
            txtURI.Text = cleanText(txtURI.Text)
            fileExt = txtURI.Text.Substring(txtURI.Text.LastIndexOf("."))
            csvpath = csvpath & fileExt
            Try
                Dim wrequest As WebRequest = WebRequest.Create(txtURI.Text.Trim)
                Dim wresponse As WebResponse = wrequest.GetResponse()
                If (wresponse.ContentLength / 1024) > hfSizeLimit.Value AndAlso Session("admin") <> "super" Then
                    Session("NewFileCreated") = False
                    btnBrowse.Focus()
                    ErrorLog = "The size of the requested file is " & Round(wresponse.ContentLength / (1024 * 1024), 2).ToString & "MB. It exceeds the " & CInt(hfSizeLimit.Value / 1024).ToString & "MB limit for files on the OUReports server. If the web site is on your company's server, contact your system administrator to increase the allowable size."
                    MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                    Exit Sub
                End If
                Dim xstream As Stream = wresponse.GetResponseStream()
                Dim tx As New StreamReader(xstream)
                'TODO check if cleaning does not crash json or xml input
                'clean text of the file
                xstring = cleanTextOfFile(tx.ReadToEnd())
            Catch ex As Exception
                ErrorLog = ex.Message  'add our message of big file
                MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Session("NewFileCreated") = False
                btnBrowse.Focus()
                Exit Sub
            End Try
            If xstring = "" Then
                Session("NewFileCreated") = False
                btnBrowse.Focus()
                ErrorLog = "File is empty."
                MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If

            File.WriteAllText(csvpath, xstring)

            'save uri in ourreportinfo
            ErrorLog = ExequteSQLquery("UPDATE OURReportInfo SET Param4id='" & txtURI.Text.Trim & "' WHERE (ReportID='" & repid & "')")
            TextBoxPageFtr.Text = TextBoxPageFtr.Text & " Last imported from the " & txtURI.Text & " on " & Now.ToString

        ElseIf FileRDL.HasFile AndAlso FileRDL.PostedFile IsNot Nothing AndAlso FileRDL.PostedFile.FileName.Trim <> "" Then  'local file, not uri
            Dim filename As String = FileRDL.PostedFile.FileName.Trim
            fileExt = filename.Substring(filename.LastIndexOf(".")).Trim
            csvpath = csvpath & fileExt
            'read file FileRDL.PostedFile into csvpath
            Try
                FileRDL.PostedFile.SaveAs(csvpath)
            Catch ex As Exception
                btnBrowse.Focus()
                MessageBox.Show(ex.Message, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End Try
            TextBoxPageFtr.Text = TextBoxPageFtr.Text & " Last imported from the file " & FileRDL.PostedFile.FileName & " on " & Now
        ElseIf (txtURI.Text.Trim = "" OrElse txtURI.Text.Trim = "https://") AndAlso Not FileRDL.HasFile Then
            ErrorLog = "File is not selected."
            Session("NewFileCreated") = False
            btnBrowse.Focus()
            MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Exit Sub
        End If

        'check if file is ok if not Access db
        If Not (fileExt.ToUpper.EndsWith(".MDB") OrElse fileExt.ToUpper.EndsWith(".ACCDB")) Then
            ret = cleanFile(csvpath)
            If ret <> csvpath Then
                Try
                    'delete csvpath
                    File.Delete(csvpath)
                Catch ex As Exception
                    ret = ex.Message
                End Try
                ErrorLog = "File is dangerous and not imported. " & ret
                MessageBox.Show(ErrorLog, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
        End If

        Dim onemany As String = "one"
        If fileExt.ToUpper = ".XML" OrElse fileExt.ToUpper = ".JSON" OrElse fileExt.ToUpper = ".RDL" OrElse fileExt.ToUpper = ".TXT" Then
            onemany = "many"
        End If
        'clear tables
        If exst AndAlso chkboxClearTable.Checked Then
            Dim res As String = ClearTables(tblname, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), repid, er, onemany)
            If res.StartsWith("ERROR!!") OrElse res.StartsWith("Table copied, but not deleted: ") Then
                MessageBox.Show(tblname & " - " & res, "Import File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
        End If
        TextBoxSQLorSPtext.Text = "SELECT * FROM " & tblname

        '============================================================ start =======================================================================

        If fileExt.Trim.ToUpper.EndsWith(".CSV") Then  'OrElse FileRDL.PostedFile.FileName.ToUpper.EndsWith(".PRN")
            'Import CSV into Table
            ret = ImportCSVintoDbTable(Session("logon"), tblname, csvpath, d, Session("UserConnString"), Session("UserConnProvider"), exst)
            If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) AndAlso exst AndAlso chkboxClearTable.Checked Then
                'restore data in the table from DELETED
                ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
            ElseIf ret = "Query executed fine." Then
                If exst Then
                    MessageBox.Show("Import CSV File result: " & tblname & " table updated from " & csvpath & ".", "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                Else
                    MessageBox.Show("Import CSV File result: " & tblname & " table created from " & csvpath & ".", "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
                End If
            Else
            MessageBox.Show("Import CSV File result: " & ret, "Import CSV File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
            End If

            '--------------------------------------------------------------------------------------------------------------
        ElseIf fileExt.ToUpper.EndsWith(".XLS") OrElse fileExt.ToUpper.EndsWith(".XLSX") Then   'OrElse txtURI.Text.Trim.ToUpper.EndsWith(".PRN")
            'import Excel file into Table
            ret = ImportExcelIntoDbTable(tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), exst)
            If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                ret = ret & " Attention!! Save your file as Microsoft Excel 97-2003 Worksheet and try to import it again."
                If exst AndAlso chkboxClearTable.Checked Then
                    'restore data in the table from DELETED
                    ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                End If
            End If
            MessageBox.Show(ret, "Import Excel File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            '--------------------------------------------------------------------------------------------------------------
        ElseIf fileExt.ToUpper.EndsWith(".ACCDB") OrElse fileExt.ToUpper.EndsWith(".MDB") Then
            'import Access table into Table
            ret = ImportExcelIntoDbTable(tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), exst)
            If (ret <> "Query executed fine." OrElse ret.StartsWith("ERROR!!")) Then
                ret = ret & " Attention!! You can export Access table into the Microsoft Excel 97-2003 Worksheet and try to import it again."
                If exst AndAlso chkboxClearTable.Checked Then
                    'restore data in the table from DELETED
                    ret = ret & ", " & RestoreDataFromDELETED(tblname, Session("UserConnString"), Session("UserConnProvider"))
                End If
            End If
            MessageBox.Show(ret, "Import table from Access db", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            '--------------------------------------------------------------------------------------------------------------
        ElseIf fileExt.ToUpper.EndsWith(".XML") OrElse fileExt.ToUpper.EndsWith(".RDL") Then
            'Import XML into Table
            'ret = ImportXMLintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), ntables)
            ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), True, chkboxUniqueFields.Checked)
            If ret.StartsWith("ERROR!!") Then
                'delete csvpath
                Try
                    File.Delete(csvpath)
                Catch ex As Exception
                    ret = ret & "ERROR!! " & ex.Message
                    LabelAlert.Text = ret
                    LabelAlert.Visible = True
                End Try
                MessageBox.Show(ret, "Import XML File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            TextBoxSQLorSPtext.Text = "SELECT * FROM " & tblname & "root"
            '--------------------------------------------------------------------------------------------------------------
        ElseIf fileExt.ToUpper.EndsWith(".TXT") OrElse fileExt.ToUpper.EndsWith(".JSON") Then
            'Import JSON into tables
            'ret = ImportJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), ntables, chkboxUniqueFields.Checked)
            ret = ImportXMLorJSONintoDatabase(Session("REPORTID"), tblname, csvpath, Session("UserConnString"), Session("UserConnProvider"), Session("Unit"), Session("logon"), Session("UserDB"), False, chkboxUniqueFields.Checked)
            If ret.StartsWith("ERROR!!") Then
                'delete csvpath
                Try
                    File.Delete(csvpath)
                Catch ex As Exception
                    ret = ret & "ERROR!! " & ex.Message
                    LabelAlert.Text = ret
                    LabelAlert.Visible = True
                End Try
                MessageBox.Show(ret, "Import JSON File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Exit Sub
            End If
            TextBoxSQLorSPtext.Text = "SELECT * FROM " & tblname & "root"
        End If

        '============================================================ end =======================================================================

        If onemany = "many" Then
            'fileExt.ToUpper.EndsWith(".TXT") OrElse fileExt.ToUpper.EndsWith(".JSON") OrElse fileExt.ToUpper.EndsWith(".XML") OrElse fileExt.ToUpper.EndsWith(".RDL")

            'make loop to register the tables tblname(i) in OURUserTables of OURdb
            dv = GetReportTablesFromOURUserTables(repid, Session("Unit"), Session("logon"), Session("UserDB"), ret)
            If dv Is Nothing OrElse dv.Table Is Nothing Then
                Label1.Text = "There are no tables found for this report."
                Exit Sub

            End If
            ntables = dv.Table.Rows.Count
            'make loop to make initial reports for the tables in OURUserTables of OURdb
            For i = 0 To ntables - 1
                j = j + 1
                rtbl = FixReservedWords(dv.Table.Rows(i)("TableName").ToString.Trim, Session("UserConnProvider"))
                ' Make new report for the table !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                Dim repsqlquery As String = "SELECT * FROM " & rtbl
                'Dim er As String = String.Empty
                Dim rep As String = Session("REPORTID")
                rep = Session("REPORTID") & i.ToString
                ret = MakeNewUserReport(Session("logon"), rep, rtbl, Session("UserDB"), repsqlquery, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                WriteToAccessLog(Session("logon"), "User Report " & rep & " has been created with result: " & ret, 111)
            Next
        End If

        ButtonSubmit_Click("", EventArgs.Empty)
        '------------------------------------------------------------------------------------
        'Put data in OURReportFormat
        Dim ddt As DataTable = GetListOfReportFields(Session("REPORTID"))  'List of Report fields from xsd
        If ddt Is Nothing OrElse ddt.Columns Is Nothing OrElse ddt.Columns.Count = 0 Then
            Label1.Text = "There are no data found for this report or error while importing."
            Exit Sub
        End If

        Dim dtrf As DataTable = GetReportFields(Session("REPORTID"))
        Dim frname As String = String.Empty
        Dim insSQL As String = String.Empty
        If dtrf Is Nothing OrElse dtrf.Rows.Count = 0 Then  'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
            'add all fields from ddt
            For i = 0 To ddt.Columns.Count - 1
                If ddt.Columns(i).Caption <> "Indx" Then
                    frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                    If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & Session("REPORTID") & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
                    ExequteSQLquery(insSQL)
                End If
            Next
        End If
        Dim HasCol As Boolean = HasReportColumns(repid)
        Dim HasRepItems As Boolean = ReportItemsExist(repid)
        Dim HasSQLQueryData As Boolean = HasSQLData(repid)
        'Put data in OURReportSqlQuery
        If Not HasSQLQueryData Then
            Dim ItemList As ListItemCollection = GetReportFieldItems()
            Dim li As ListItem
            Dim fld As String
            Dim tbl As String
            For i = 0 To ItemList.Count - 1
                li = ItemList.Item(i)
                If li.Text <> String.Empty AndAlso li.Text <> " " Then
                    tbl = li.Text.Substring(0, li.Text.LastIndexOf("."))
                    fld = li.Text.Substring(li.Text.LastIndexOf(".")).Replace(".", "")
                    AddField(repid, tbl, fld, "")
                End If
            Next
        End If

        'Put data in OURReportItems
        If HasCol AndAlso Not HasRepItems Then
            'CreateReportItems(Session("REPORTID"), Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            Dim retr As String = CreateReportItems(Session("REPORTID"), Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            If retr.StartsWith("ERROR!!") Then
                LabelAlert.Text = "Report info is not updated. " & retr
                LabelAlert.Visible = True
                Exit Sub
            End If
        End If
        '------------------------------------------------------------------------------------
        n = 0
        ret = UpdateDashboardsWithReport(Session("REPORTID"), n, Session("UserConnString"), Session("UserConnProvider"), er)

        'delete csvpath
        Try
            File.Delete(csvpath)
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
            LabelAlert.Text = ret
            LabelAlert.Visible = True
        End Try

        If Session("CSV") = "yes" Then
            Response.Redirect("ShowReport.aspx?srd=3")
        Else
            Response.Redirect("SQLquery.aspx?tnq=0")
        End If

    End Sub
    'Private Function InsertTableIntoOURUserTables(ByVal tblname As String, ByVal tblfriendlyname As String) As String
    '    Dim ret As String = String.Empty
    '    Dim msql As String = String.Empty
    '    If Not HasRecords("SELECT * FROM OURUserTables WHERE Unit='" & Session("Unit") & "' AND UserID='" & Session("logon") & "' AND TableName='" & tblname & "' AND UserDB='" & Session("UserDB") & "'") Then
    '        'register the table tblname in OURUserTables of OURdb
    '        msql = "INSERT INTO OURUserTables (Unit,UserID,TableName,Prop1,UserDB) VALUES ('" & Session("Unit") & "','" & Session("logon") & "','" & tblname & "','" & tblfriendlyname & "','" & Session("UserDB") & "')"
    '        ret = ExequteSQLquery(msql)
    '        WriteToAccessLog(tblname, "Load table into OURUserTables: " & msql & " - with result: " & ret, 111)
    '    End If
    '    Return ret
    'End Function

    Private Sub chkboxClearTable_CheckedChanged(sender As Object, e As EventArgs) Handles chkboxClearTable.CheckedChanged
        'clear table or not
        If chkboxClearTable.Checked Then
            MessageBox.Show("Table will be copied into temporary table with DELETED added to the name (only for CSV, Excel, and ACCESS upload)", "Confirm deleting of all records from the table", "ClearTable", Controls_Msgbox.Buttons.OKCancel, Controls_Msgbox.MessageIcon.Question)

        End If
    End Sub

    Private Sub ButtonUpdateDashboards_Click(sender As Object, e As EventArgs) Handles ButtonUpdateDashboards.Click
        Dim n As Integer = 0
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        ret = UpdateDashboardsWithReport(Session("REPORTID"), n, Session("UserConnString"), Session("UserConnProvider"), er)

        MessageBox.Show("Dashboards updated with result: " & ret, "Dashboards", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
    End Sub

End Class



