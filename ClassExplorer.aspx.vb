Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Math
Imports System.Drawing
Imports System.Web.UI.WebControls
Imports Microsoft.Reporting.WebForms
Partial Class ClassExplorer
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
    Public dirsort As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        Session("addfilter") = ""
        Session("Stats") = 0
        Session("graph") = ""
        'Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
        'Dim repdb As String = Session("UserConnString")
        If Session("UserConnProvider").ToString <> "Oracle.ManagedDataAccess.Client" Then
            If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
            If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        Else
            If Session("UserConnString").ToUpper.IndexOf("PASSWORD") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("PASSWORD")).Trim
        End If
        'If Session("UserConnString").ToUpper.IndexOf("USER ID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").ToUpper.IndexOf("USER ID")).Trim
        'If Session("UserConnString").IndexOf("UID") > 0 Then Session("UserDB") = Session("UserConnString").Substring(0, Session("UserConnString").IndexOf("UID")).Trim
        Session("dbname") = GetDataBase(Session("UserConnString"), Session("UserConnProvider")).Replace("#", "")
        trStatLabel.Visible = False
        trReportStats.Visible = False
        GridView2.Enabled = False
        GridView2.Visible = False
        CheckBoxHideDuplicates.Checked = False

        TextBoxDelimeter.Text = Session("delimiter")
        If WhereText Is Nothing Then WhereText = ""
        If Session("WhereText") Is Nothing Then Session("WhereText") = ""

        If Not IsPostBack Then
            Session("sqltoexport") = ""
            Dim ddt As DataTable
            Dim err As String = String.Empty
            Dim ddtv As DataView = Nothing
            If Session("CSV") = "yes" Or Session("logon") = "demo" Then
                CheckBoxSysTables.Checked = False
                CheckBoxSysTables.Visible = False
                CheckBoxSysTables.Enabled = False
            ElseIf Session("admin") = "super" OrElse Session("Access") = "SITEADMIN" OrElse Session("syschk") = True Then
                CheckBoxSysTables.Checked = True
            Else
                CheckBoxSysTables.Checked = False
            End If
            Session("syschk") = CheckBoxSysTables.Checked
            ddtv = GetListOfUserTables(CheckBoxSysTables.Checked, Session("UserConnString"), Session("UserConnProvider"), err, Session("logon"), Session("CSV"))
            If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
                LabelError.Text = "No tables nor fields selected into report yet..."
                LabelError.Visible = True
                Exit Sub
            End If
            If err <> "" Then
                LabelError.Text = err
                LabelError.Visible = True
                Exit Sub
            End If
            ddt = ddtv.ToTable
            DropDownTables.Items.Add(" ")
            For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                Dim li As New ListItem
                If Session("CSV") = "yes" Then
                    'li.Text = ddt.Rows(i)("TABLENAME").ToString & "(" & ddt.Rows(i)("TABLE_NAME").ToString & ")"
                    li.Text = ddt.Rows(i)("TABLE_NAME").ToString
                    If IsColumnFromDataTable(ddt, "TABLENAME") AndAlso ddt.Rows(i)("TABLENAME").ToString.Trim <> "" AndAlso ddt.Rows(i)("TABLENAME").ToString.Trim <> ddt.Rows(i)("TABLE_NAME").ToString.Trim Then
                        li.Text = ddt.Rows(i)("TABLE_NAME").ToString '& "(" & ddt.Rows(i)("TABLENAME").ToString & ")"
                    End If
                Else
                    li.Text = ddt.Rows(i)("TABLE_NAME").ToString
                End If
                li.Value = ddt.Rows(i)("TABLE_NAME").ToString
                DropDownTables.Items.Add(li)
            Next
            If DropDownTables.Items.Count > 1 Then
                DropDownTables.Items(0).Selected = True
                DropDownTables.SelectedIndex = 0
                If Not IsPostBack AndAlso Session("ReportID").ToString.Trim = "" Then
                    DropDownTables.SelectedValue = " "
                ElseIf Not IsPostBack AndAlso Session("ReportID").ToString.Trim <> "" AndAlso Request("FromData") = "yes" Then
                    'first in the list of report tables
                    Dim drt As DataTable = GetReportTables(Session("ReportID"))
                    If drt IsNot Nothing AndAlso drt.Rows.Count > 0 Then
                        DropDownTables.SelectedValue = drt.Rows(0)("Tbl1")
                        Session("ReportID") = ""
                        Response.Redirect("ClassExplorer.aspx?cld=" & drt.Rows(0)("Tbl1"))
                    Else
                        DropDownTables.SelectedValue = " "
                    End If
                Else
                    DropDownTables.SelectedValue = ddt.Rows(0)("TABLE_NAME")
                End If
            End If
        End If
        HyperLinkExport.Text = " "
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
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
            'Response.Redirect("Correlation.aspx?export=no")
        End If
        If DropDownTables.Text.Trim = "" Then
            Session("noedit") = Nothing
            Session("REPORTID") = ""
            Session("dv3") = Nothing
            Session("DataToChatAI") = ""
            Session("sqltoexport") = ""
            LabelSelect.Visible = True
            LabelReports.Visible = False
            list.Visible = False
            GridView1.DataSource = Nothing
            'GridView1.DataBind()
            Session("dataTable") = Nothing
            GridView1.Visible = False
            LabelRowCount.Text = ""
            DropDownColumns.Enabled = False
            TextBoxSearch.Text = ""
            LinkButtonAnalytics.Visible = False
            LinkButtonAnalytics.Enabled = False
            HyperLinkChatAI.Enabled = False
            HyperLinkChatAI.Visible = False
            Session("REPORTID") = ""
            LabelReportID.Text = ""
            btnCorrectFields.Visible = False
            btnCorrectFields.Enabled = False
            lnkExportGrid1.Enabled = False
            lnkExportGrid1.Visible = False
        Else
            LabelSelect.Visible = False
            LabelReports.Visible = True
            list.Visible = True
            GridView1.Visible = True
            DropDownColumns.Enabled = True
            LinkButtonAnalytics.Visible = True
            LinkButtonAnalytics.Enabled = True
            HyperLinkChatAI.Enabled = True
            HyperLinkChatAI.Visible = True
            btnCorrectFields.Visible = True
            btnCorrectFields.Enabled = True
            ButtonSearch.Enabled = True
            lnkExportGrid1.Enabled = True
            lnkExportGrid1.Visible = True
        End If
        Try
            Dim er As String = String.Empty
            Dim ddid, ddname, ddsql, ddfield, ddvalue, ddlabel, ddtype, ddrequest, ret As String
            Dim drvalue, options As String
            LabelError.Text = ""
            trStatLabel.Visible = False
            trReportStats.Visible = False
            trParameters.Visible = True
            dirsort = ""
            reppath = ""
            LabelResult.Text = " "
            LabelExportExcel.Text = " "
            LabelExport.Text = " "

            HyperLinkToCSVFile.Text = " "
            trParameters.Visible = True

            If Not Session("pdfpath") Is Nothing AndAlso Session("pdfpath").ToString <> "" Then
                ButtonDownloadFile.Visible = True
                ButtonDownloadFile.Enabled = True
            Else
                ButtonDownloadFile.Visible = False
                ButtonDownloadFile.Enabled = False
            End If
            HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Class_Table_Explorer"
            If Not IsPostBack AndAlso Not Request("cld") Is Nothing AndAlso Request("cld").ToString.Trim <> "" AndAlso Request("pid") Is Nothing Then
                dv3 = Nothing
                Session("dv3") = Nothing
                DropDownTables.Text = Request("cld").ToString.Trim
                DropDownTables.SelectedItem.Text = Request("cld").ToString.Trim
                DropDownTables.SelectedItem.Value = Request("cld").ToString.Trim
                DropDownTables.SelectedValue = Request("cld").ToString.Trim
                DropDownColumns.Visible = True
                DropDownColumns.Enabled = True
                DropDownColumns.Items.Clear()
                Dim ddt As DataTable = GetListOfTableColumns(DropDownTables.Text, Session("UserConnString"), Session("UserConnProvider")).Table
                DropDownColumns.Items.Add(" ")
                If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
                    For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                        DropDownColumns.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
                        If Not Request("prf") Is Nothing AndAlso Request("prf").ToString.Trim <> "" AndAlso ddt.Rows(i)("COLUMN_NAME").ToString.Trim = Request("prf").ToString.Trim Then
                            DropDownColumns.SelectedValue = Request("prf").ToString.Trim
                        End If
                    Next
                End If

                DropDownTables_SelectedIndexChanged(sender, e)

            ElseIf Not IsPostBack AndAlso Not Request("cld") Is Nothing AndAlso Request("cld").ToString.Trim <> "" AndAlso Not Request("pid") Is Nothing AndAlso Request("pid").ToString.Trim <> "" Then
                dv3 = Nothing
                Session("dv3") = Nothing

                'TODO add one more request for parent class !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                TextBoxSearch.Text = Request("pid").ToString.Trim
                If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data") Then
                    If Not Request("rel") Is Nothing AndAlso Request("rel").ToString.Trim = "chd" Then
                        'TextBoxSearch.Text = TextBoxSearch.Text & "||"
                    Else
                        DropDownOperator.SelectedValue = "="
                    End If
                End If
                DropDownTables.Text = Request("cld").ToString.Trim
                DropDownTables.SelectedItem.Text = Request("cld").ToString.Trim
                DropDownTables.SelectedItem.Value = Request("cld").ToString.Trim
                DropDownTables.SelectedValue = Request("cld").ToString.Trim
                DropDownColumns.Items.Clear()
                Dim ddt As DataTable = GetListOfTableColumns(DropDownTables.Text, Session("UserConnString"), Session("UserConnProvider")).Table
                DropDownColumns.Items.Add(" ")
                If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
                    For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                        DropDownColumns.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
                        If Not Request("prf") Is Nothing AndAlso Request("prf").ToString.Trim <> "" AndAlso ddt.Rows(i)("COLUMN_NAME").ToString.Trim = Request("prf").ToString.Trim Then
                            DropDownColumns.SelectedValue = Request("prf").ToString.Trim
                        End If
                    Next
                End If
                If DropDownTables.SelectedItem.Text.Trim = "" Then
                    LinkButtonAnalytics.Visible = False
                    LinkButtonAnalytics.Enabled = False
                Else
                    LinkButtonAnalytics.Visible = True
                    LinkButtonAnalytics.Enabled = True
                End If
                If Not Request("prf") Is Nothing Then
                    'inverse field
                    If DropDownColumns.SelectedValue.Trim <> "" Then
                        DropDownColumns.Text = Request("prf").ToString.Trim
                        DropDownOperator.SelectedValue = "="
                    Else
                        DropDownColumns.SelectedValue = "ID"
                        DropDownColumns.Text = "ID"
                        If Not Request("rel") Is Nothing AndAlso Request("rel").ToString.Trim = "prn" Then
                            DropDownOperator.SelectedValue = "="
                        Else
                            DropDownOperator.SelectedValue = "StartsWith"
                        End If
                    End If
                Else
                    DropDownColumns.SelectedValue = "ID"
                    DropDownColumns.Text = "ID"
                    If Not Request("rel") Is Nothing AndAlso Request("rel").ToString.Trim = "prn" Then
                        DropDownOperator.SelectedValue = "="
                    Else
                        DropDownOperator.SelectedValue = "StartsWith"
                    End If
                End If
                Session("addfilter") = ""
                If Not Request("pcl") Is Nothing AndAlso Request("pcl").ToString.Trim <> "" AndAlso Not Request("pcf") Is Nothing AndAlso Request("pcf").ToString.Trim <> "" Then
                    Session("addfilter") = " " & Request("pcf").ToString.Trim & " LIKE '" & Request("pcl").ToString.Trim & "%' "
                End If
            End If

            If Request("tbl") IsNot Nothing AndAlso Request("tbl").ToString.Trim <> "" Then
                DropDownTables.SelectedValue = Request("tbl")
                DropDownTables.Text = Request("tbl")

            End If

            If DropDownTables.Text.Trim = "" Then
                Exit Sub
            End If


            'TODO check in SQL Server, MySql,Oracle
            Dim cln As String = FixReservedWords(CorrectTableNameWithDots(DropDownTables.Text, Session("UserConnProvider")), Session("UserConnProvider"))

            'Dim er As String = String.Empty
            'Check Report Info (title, data for report) for this class 
            dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & DropDownTables.Text & "') AND (ReportDB LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")

            'Session("sqltoexport") = "SELECT * FROM " & cln

            If dv1 Is Nothing OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then  'no reports for select from table
                If HasRecords("SELECT * FROM " & cln, Session("UserConnString"), Session("UserConnProvider")) Then
                    'TODO create new standard report - it was done in DataImport just after importing data into the table
                    repid = Session("dbname") & "_INIT_00" & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                    ret = MakeNewStanardReport(Session("logon"), repid, DropDownTables.Text, Session("UserDB"), "SELECT * FROM " & cln, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
                    If ret.Contains("ERROR!!") Then
                        WriteToAccessLog(Session("logon"), "Initial Report created with errors:" & DropDownTables.Text & ", error:" & ret, 6)
                        Session("REPORTID") = ""
                    Else
                        ret = CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                        dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & DropDownTables.Text & "') AND (ReportDB LIKE '%" & Session("UserDB").ToString.Trim.Replace(" ", "%") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
                    End If
                End If
                If dv1 Is Nothing OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then  'still no reports for select from table
                    Session("REPORTID") = ""
                    Session("sqltoexport") = ""
                    Exit Try
                End If
                'Else
                '    'add permissions for user if they are not there
                '    repid = dv1.Table.Rows(0)("ReportId").ToString
                '    Dim sqlq As String = "SELECT * FROM OURPermissions WHERE Param1='" & repid & "'"
                '    If Not HasRecords(sqlq) Then
                '        'new permission required
                '        sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & repid & "','" & DropDownTables.Text & "','" & Session("email") & "','admin','" & DateToString(Now) & "','" & Session("UserEndDate") & "','initial')"
                '        Dim retr As String = ExequteSQLquery(sqlq)
                '    End If
                '    ret = CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                '    'dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & DropDownTables.Text & "') AND (ReportDB LIKE '%" & Session("UserDB") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
            End If

            If Not (dv1 Is Nothing OrElse dv1.Table Is Nothing) AndAlso dv1.Table.Rows.Count = 1 Then
                If dv1.Table.Rows(0)("ReportType").ToString = "" Then
                    dv1.Table.Rows(0)("ReportType") = "rdl"
                End If
                repid = dv1.Table.Rows(0)("ReportId").ToString
                If repid <> "" Then Session("REPORTID") = repid
                LabelReportID.Text = Session("REPORTID")
                    Session("noedit") = dv1.Table.Rows(0)("Param7type").ToString

                    reptitle = dv1.Table.Rows(0)("ReportTtl").ToString
                Session("REPTITLE") = reptitle
                Session("REPORTID") = repid
                'LabelReportTitle.Text = "Data for report:     " & reptitle
                lblStatistics.Text = "Statistics for report: " & reptitle
                LabelPageFtr.Text = dv1.Table.Rows(0)("Comments").ToString
                Session("PageFtr") = LabelPageFtr.Text
                main.Rows(4).Cells(0).InnerHtml = "&nbsp;"
            Else
                'Response.Redirect("Nodata.aspx")
            End If

            If Session("REPORTID") <> "" Then

                'Report Show (drop-downs, data for drop-downs)
                'dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY Indx")
                dv2 = mRecords("SELECT * FROM OURReportShow WHERE (ReportID='" & repid & "') ORDER BY PrmOrder")
                n = dv2.Table.Rows.Count   'how many parameters (drop-downs)
                Session("ParamsCount") = n
                ReDim ParamNames(n)
                ReDim ParamTypes(n)
                ReDim ParamValues(n)

                If n > 0 Then
                    trParameters.Visible = True
                Else
                    trParameters.Visible = False
                End If

                'create parameters and draw drop-downs
                For i = 0 To dv2.Table.Rows.Count - 1   'draw drop-down start
                    options = ""
                    ddid = dv2.Table.Rows(i)("DropDownID")
                    ddlabel = dv2.Table.Rows(i)("DropDownLabel")
                    ddname = dv2.Table.Rows(i)("DropDownName")
                    ddfield = dv2.Table.Rows(i)("DropDownFieldName")
                    ddtype = dv2.Table.Rows(i)("DropDownFieldType")
                    ddvalue = dv2.Table.Rows(i)("DropDownFieldValue")
                    ddsql = dv2.Table.Rows(i)("DropDownSQL").ToString.Replace("""", "'")

                    'check if drop-down is selected
                    ddrequest = Request("Select" & ddid)

                    If ddrequest = "" AndAlso Not Session("ParamValues") Is Nothing AndAlso Session("ParamsCount") > i AndAlso Not Session("ParamValues")(i) Is Nothing AndAlso Session("ParamValues")(i).ToString.Trim <> "" AndAlso Session("ParamValues")(i).ToString.ToUpper <> "ALL" Then
                        ddrequest = Session("ParamValues")(i).ToString
                    End If

                    ParamNames(i) = ddname
                    ParamTypes(i) = ddtype
                    If ddrequest <> "" Then
                        ParamValues(i) = ddrequest
                    Else
                        ParamValues(i) = "ALL"
                    End If

                    'drop down
                    main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & ddlabel & ": &nbsp;"
                    'main.Rows(1).Cells(0).InnerHtml = ddlabel & ": &nbsp;"

                    'main.Rows(1).Cells(0).InnerHtml = main.Rows(1).Cells(0).InnerHtml & " &nbsp;<asp:DropDownList runat='server' ID='DropDownList" & ddid & "'></asp:DropDownList>"
                    main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & "<select name ='Select" & ddid & "' id='Select" & ddid & "' >"
                    If ddrequest <> "" Then
                        main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & "<option>" & ddrequest & "</option>"
                    End If
                    er = ""
                    'get dropdown (name,value) - run sql in user database !!! Correct ddsql for user database provider:
                    ddsql = ConvertSQLSyntaxFromOURdbToUserDB(ddsql, Session("UserConnProvider"), er)
                    Dim dv As DataView = mRecords(ddsql, er, Session("UserConnString"), Session("UserConnProvider"))
                    If Not dv Is Nothing AndAlso dv.Count > 0 Then
                        dt = dv.Table
                        ndd = dt.Rows.Count
                        If ddid.Contains(".") Then
                            ddfield = ddid
                        Else
                            ddfield = AddTableNameToField(repid, ddvalue, mSql, er)
                        End If
                        ddfield = FixReservedWords(ddfield, Session("UserConnProvider"))
                        Dim bTimeStamp As Boolean = IsTimeStamp(Piece(ddfield, ".", 1), Piece(ddfield, ".", 2), Session("UserConnString"), Session("UserConnProvider"))

                        For j = 0 To dt.Rows.Count - 1
                            drvalue = Trim(dt.Rows(j)(ddvalue).ToString)
                            If IsDateTime(drvalue) Then
                                drvalue = DateToString(drvalue, Session("UserConnProvider"), bTimeStamp)
                            End If
                            If drvalue <> "" Then
                                options = options & "<option>" & drvalue & "</option>"
                            End If
                        Next
                    Else
                        If er <> "" Then
                            LabelError.Text = "ERROR:  " & er
                            LabelError.Visible = True
                            LabelError1.Text = "ERROR:  " & er
                            LabelError1.Visible = True
                        End If
                    End If
                    If UCase(dv1.Table.Rows(0)("ReportAttributes").ToString) = "SQL" Then
                        main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & "<option>All</option>"
                        main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & options & "</select> &nbsp;"
                    Else
                        main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & options & "<option>All</option>"
                        main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & "</select> &nbsp;"
                    End If

                    'retrieve selected drop-down data
                    If ddrequest <> "" And ddrequest <> "All" And UCase(dv1.Table.Rows(0)("ReportAttributes").ToString) = "SQL" Then
                        If ddrequest.ToUpper = "TRUE" Then
                            ddrequest = "1"
                        ElseIf ddrequest.ToUpper = "FALSE" Then
                            ddrequest = "0"
                        End If
                        If Trim(WhereText) <> "" Then WhereText = WhereText & " AND "
                        'correct ddfield adding table name
                        'mSql = dv1.Table.Rows(0)("SQLquerytext")
                        mSql = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                        'ddfield = AddTableNameToField(repid, ddfield, mSql, er)
                        ddfield = AddTableNameToField(repid, ddvalue, mSql, er)
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
                        'If (ddtype = "nvarchar" Or ddtype = "datetime") Then WhereText = WhereText & "'"
                    End If
                    'Submit1.Visible = True
                Next   ' go to next draw dop-down 

                Session("ParamNames") = ParamNames
                Session("ParamValues") = ParamValues
                Session("ParamsCount") = ParamNames.Length

                'draw submit button
                If n > 0 Then
                    main.Rows(4).Cells(0).InnerHtml = main.Rows(4).Cells(0).InnerHtml & "<input type=submit name='Submit2' id='Submit2' value='Apply' AutoPostBack='true'>&nbsp;&nbsp;"
                End If

                If Session("admin") = "super" OrElse Session("admin") = "admin" OrElse Session("AdvancedUser") = True Then
                    LabelSQL.Text = mSql
                    LabelSQL.Visible = True
                Else
                    LabelSQL.Visible = False
                End If

            End If
            If Not IsPostBack AndAlso Not Request("cld") Is Nothing AndAlso Request("cld").ToString.Trim <> "" AndAlso Not Request("pid") Is Nothing AndAlso Request("pid").ToString.Trim <> "" Then
                ButtonSearch_Click(sender, e)
            End If
        Catch ex As Exception
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End Try
    End Sub
    Private Sub BindReportFields()
        Dim ReportDataView As DataView = CType(Session("dv3"), DataView)
        If Session("SortedView") IsNot Nothing Then
            ReportDataView = Session("SortedView")
        End If
        If ReportDataView Is Nothing Then
            Session("dataTable") = Nothing
            Session("GridView1DataSource") = Nothing
            'GridView1.Dispose()
            GridView1.Enabled = False
            GridView1.Visible = False
        Else
            GridView1.Enabled = True
            GridView1.Visible = True
            GridView1.DataSource = ReportDataView
            GridView1.DataBind()
            Session("dataTable") = ReportDataView.Table
            Session("GridView1DataSource") = Session("dataTable")
        End If

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

        GridView2.Enabled = False
        GridView2.Visible = False

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
        'myfile = Session("REPORTID") & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".xls"
        myfile = Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".xls"

        appldirExcelFiles = applpath & "Temp\"
        Session("appldirExcelFiles") = appldirExcelFiles
        Session("myfile") = appldirExcelFiles & myfile
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

        Dim hdr As String = "Data for  " & Session("REPTITLE")
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

        res = DataModule.ExportToExcel(dtt, appldirExcelFiles, myfile, hdr, Session("PageFtr"), TextBoxDelimeter.Text)

        If Left(res, 5) <> "Error" Then
            LabelExportExcel.Text = " "
            LabelExport.Text = "To open Excel file click "
            HyperLinkExport.Visible = True
            HyperLinkExport.NavigateUrl = "./Temp/" & myfile
            HyperLinkExport.Text = "here"

        Else
            LabelExportExcel.Text = res
            'HyperLinkToExcelFile.Text = " "
            LabelExport.Text = res
            HyperLinkExport.Text = " "
        End If
        Try
            Response.ContentType = "application/octet-stream"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile)
            Response.TransmitFile(Session("myfile"))
        Catch ex As Exception
            LabelExportExcel.Text = "ERROR!! " & ex.Message
            LabelExportExcel.Visible = True
            LabelExportExcel.Enabled = True
        End Try
        Response.End()
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
        'NOT IN USE
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
            'Return ret
        Catch ex As Exception
            'Return "ERROR!! " & ex.Message
        End Try
    End Function
    Protected Sub ShowRDL_Click(sender As Object, e As EventArgs) Handles ShowRDL.Click
        'NOT IN USE
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
        Dim i As Integer
        Dim nw As String = String.Empty
        Dim er As String = String.Empty
        Session("Stats") = 1
        dv3 = Session("dv3")
        If dv3 Is Nothing OrElse dv3.Count = 0 Then
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
            Dim dts As New DataTable
            dts = dv3.Table
            dts = MakeDTColumnsNamesCLScompliant(dts, Session("UserConnProvider"), er)
            dv3 = dts.DefaultView
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
            BindReportFields()
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
        ElseIf e.Row IsNot Nothing AndAlso e.Row.RowType = DataControlRowType.DataRow Then
            Dim i As Integer
            Dim fld As String = String.Empty
            Dim sql As String = String.Empty
            'Dim prf As String = String.Empty
            Dim dvs3 As DataView = Session("dv3")
            For i = 0 To e.Row.Cells.Count - 1
                If dvs3 Is Nothing Then
                    Continue For
                End If
                fld = dvs3.Table.Columns(i).Caption
                e.Row.Cells(i).ToolTip = "Row: " & e.Row.Cells(0).Text & ", Column #" & i.ToString & " - " & fld
                If e.Row.Cells(i).Text.Contains("^") Then
                    'if column is a parent
                    Dim cellvals() As String = e.Row.Cells(i).Text.Split("^")
                    If cellvals.Length > 3 AndAlso cellvals(3) = "prn" Then
                        Dim myLink As HyperLink = New HyperLink
                        myLink.Text = cellvals(0)
                        myLink.NavigateUrl = "ClassExplorer.aspx?cld=" & cellvals(0).Replace("%", "%25") & "&prf=" & cellvals(1) & "&pid=" & cellvals(2).ToString.Replace("%", "%25") & "&rel=prn"
                        'myLink.Target = "_blank"
                        e.Row.Cells(i).Controls.Add(myLink)
                        e.Row.Cells(i).ToolTip = e.Row.Cells(i).ToolTip & ", points to parent:  " & cellvals(0)
                    ElseIf i = 0 AndAlso cellvals.Length = 2 AndAlso cellvals(1) = "" Then  'from json or xml input
                        Dim myLink As HyperLink = New HyperLink
                        myLink.Text = cellvals(0)
                        myLink.NavigateUrl = "ClassExplorer.aspx?cld=" & cellvals(0).Replace("%", "%25") & "&prf=id&pid=" & e.Row.Cells(1).Text.Trim.Replace("%", "%25") & "&rel=prn"
                        'myLink.Target = "_blank"
                        e.Row.Cells(i).Controls.Add(myLink)
                        e.Row.Cells(i).ToolTip = e.Row.Cells(i).ToolTip & ", points to parent:  " & cellvals(0)
                    End If
                ElseIf e.Row.Cells(i).Text.Contains("~") Then
                    'if column is a child
                    Dim cellvals() As String = e.Row.Cells(i).Text.Split("~")
                    If cellvals.Length > 3 AndAlso cellvals(3) = "cld" Then
                        Dim myLink As HyperLink = New HyperLink
                        myLink.Text = cellvals(0)
                        myLink.NavigateUrl = "ClassExplorer.aspx?cld=" & cellvals(0).Replace("%", "%25") & "&prf=" & cellvals(1) & "&pid=" & cellvals(2).ToString.Replace("%", "%25") & "&rel=chd"
                        sql = "SELECT * FROM " & cellvals(0) & " WHERE " & cellvals(1) & "='" & cellvals(2) & "'"
                        If cellvals.Length = 6 Then
                            myLink.NavigateUrl = myLink.NavigateUrl & "&pcl=" & cellvals(4).Replace("%", "%25") & "&pcf=" & cellvals(5).Replace("%", "%25")
                            sql = sql & " AND " & cellvals(5) & " LIKE '" & cellvals(4) & "%'"
                        End If
                        'myLink.Target = "_blank"
                        e.Row.Cells(i).Controls.Add(myLink)
                        e.Row.Cells(i).ToolTip = e.Row.Cells(i).ToolTip & ", number of children: " & CountOfRecords(sql, Session("UserConnString"), Session("UserConnProvider"))
                    ElseIf i <> 0 AndAlso Not e.Row.Cells(3) Is Nothing AndAlso e.Row.Cells(3).Text.Trim <> "" AndAlso cellvals.Length = 2 AndAlso cellvals(1) = "" Then  'from json or xml input
                        Dim myLink As HyperLink = New HyperLink
                        myLink.Text = cellvals(0)
                        myLink.NavigateUrl = "ClassExplorer.aspx?cld=" & cellvals(0).Replace("%", "%25") & "&prf=parentid&pid=" & e.Row.Cells(3).Text.Replace("%", "%25") & "&rel=chd"
                        sql = "SELECT * FROM " & cellvals(0) & " WHERE parentid='" & e.Row.Cells(3).Text.Trim & "'"
                        'myLink.Target = "_blank"
                        e.Row.Cells(i).Controls.Add(myLink)
                        e.Row.Cells(i).ToolTip = e.Row.Cells(i).ToolTip & ", number of children: " & CountOfRecords(sql, Session("UserConnString"), Session("UserConnProvider"))

                    End If
                End If
            Next
        End If
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        'Dim url As String = "~/" & node.Value
        'Response.Redirect(url)
        Response.Redirect(node.Value)
    End Sub
    Private Function SearchStatement() As String
        Dim srch As String = String.Empty
        Try

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
                If Not TblFieldIsNumeric(DropDownTables.Text, srchfld, Session("UserConnString"), Session("UserConnProvider")) Then
                    If oper.ToUpper.Contains("STARTSWITH") Then
                        If HasNot Then
                            oper = "Not Like"
                        Else
                            oper = "Like"
                        End If
                        srch = srchfld & " " & oper & " '" & val & "%'"
                    ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                        If HasNot Then
                            oper = "Not Like"
                        Else
                            oper = "Like"
                        End If
                        srch = srchfld & " " & oper & " '%" & val & "'"
                    ElseIf oper.ToUpper.Contains("CONTAINS") Then
                        If HasNot Then
                            oper = "Not Like"
                        Else
                            oper = "Like"
                        End If
                        srch = srchfld & " " & oper & " '%" & val & "%'"
                    ElseIf oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                        srch = srchfld & " " & oper & " ('" & val.Replace("(", "").Replace(")", "").Replace(",", "','") & "')"
                    End If
                    If srch.Trim = "" Then
                        srch = srchfld & " " & oper & " '" & val.Replace("'", "") & "'"
                    End If
                End If
                If TblFieldIsNumeric(DropDownTables.Text, srchfld, Session("UserConnString"), Session("UserConnProvider")) Then
                    If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                        srch = srchfld & " " & oper & " (" & val.Replace("(", "").Replace(")", "").Replace("'", "") & ")"
                    End If
                    If srch.Trim = "" Then
                        srch = srchfld & " " & oper & " " & val.Replace("'", "")
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

        Catch ex As Exception
            srch = "ERROR!! " & ex.Message
            Return srch
        End Try
        Return srch
    End Function
    Private Sub TextBoxDelimeter_TextChanged(sender As Object, e As EventArgs) Handles TextBoxDelimeter.TextChanged
        Dim delim As String = TextBoxDelimeter.Text
        If delim = String.Empty Then delim = ","
        Session("delimiter") = delim
    End Sub
    Private Function RetrieveDataForReport() As DataView
        'retrieve data for report
        Dim r As String = String.Empty
        'add request drop-downs and constract filter
        'If UCase(dv1.Table.Rows(0)("ReportAttributes").ToString) = "SQL" Then  'SQL statement
        'mSql = dv1.Table.Rows(0)("SQLquerytext").ToString
        If DropDownTables.Text.Trim = "" Then
            Return Nothing
        End If

        mSql = "SELECT * FROM " & FixReservedWords(CorrectTableNameWithDots(DropDownTables.Text, Session("UserConnProvider")), Session("UserConnProvider"))

        If Not Session("addfilter") Is Nothing AndAlso Session("addfilter").ToString.Trim <> "" Then
            mSql = mSql & " WHERE " & Session("addfilter") & " "
        End If

        'find WHERE, ORDER BY, and GROUP BY: add to WHERE or place WHERE in front of GROUP, and place ORDER BY in the end of SQL
        Dim wherenum, ordernum, groupnum As Integer
        wherenum = 0
        ordernum = 0
        groupnum = 0
        wherenum = InStr(UCase(mSql), " WHERE ")
        ordernum = InStr(UCase(mSql), " ORDER BY ")
        groupnum = InStr(UCase(mSql), " GROUP BY ")

        If wherenum > 0 Then        'mSql has " WHERE "
            Dim sqlWhere As String = mSql.Substring(wherenum + 6, Len(mSql) - wherenum - 6)
            sqlWhere = sqlWhere.Replace("""", "'")
            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data.") Then
                Dim parts = sqlWhere.Split(" "c)
                Dim sWhere As String = String.Empty
                For p As Integer = 0 To parts.Length - 1
                    If parts(p).Contains(".") AndAlso Not parts(p).Contains("'%'") Then
                        parts(p) = parts(p).Replace("'", """")
                    End If
                    If sWhere = String.Empty Then
                        sWhere = parts(p)
                    Else
                        sWhere &= " " & parts(p)
                    End If
                Next
                sqlWhere = sWhere
            End If
            If Trim(WhereText) <> "" Then
                mSql = mSql.Substring(0, wherenum + 6) & WhereText & " AND " & sqlWhere
            Else
                mSql = mSql.Substring(0, wherenum + 6) & sqlWhere
            End If

        Else    'mSql does not have " WHERE "
            If groupnum > 0 Then       'mSql has " GROUP BY " and mSql does not have " WHERE "
                If Trim(WhereText) <> "" Then mSql = mSql.Substring(0, groupnum - 1) & " WHERE " & WhereText & " " & mSql.Substring(groupnum, Len(mSql) - groupnum)
            Else                       'mSql does not have " GROUP BY " and mSql does not have " WHERE "
                If ordernum > 0 Then      'mSql has " ORDER BY " and mSql does not have " WHERE "
                    If Trim(WhereText) <> "" Then mSql = mSql.Substring(0, ordernum - 1) & " WHERE " & WhereText & " " & mSql.Substring(ordernum, Len(mSql) - ordernum)
                Else                      'mSql does not have " ORDER BY " and mSql does not have " WHERE "
                    'if no GROUP and no ORDER
                    If Trim(WhereText) <> "" Then mSql = mSql & " WHERE " & WhereText
                    'ORDER BY in the end always
                    If Trim(dirsort) <> "" Then mSql = mSql & " ORDER BY " & dirsort
                End If
            End If
        End If

        Dim err As String = String.Empty
        'run sql in user database !!! Correct mSql for user database provider:
        mSql = ConvertSQLSyntaxFromOURdbToUserDB(mSql, Session("UserConnProvider"), err)
        err = ""
        'Data for report by SQL statement from the user database
        dv3 = mRecords(mSql, err, Session("UserConnString"), Session("UserConnProvider"))
        r = err
        If err <> "" Then
            dv3 = Nothing
        End If

        Session("WhereText") = WhereText
        'Else 'sp                                                  'Stored procedure
        '    mSql = Trim(dv1.Table.Rows(0)("SQLquerytext"))
        '    '!!!!!!!!!!!!!!!!!!!!!  Should be this one:
        '    dv3 = mRecordsFromSP(mSql, Session("ParamsCount"), Session("ParamNames"), Session("ParamTypes"), Session("ParamValues"), Session("UserConnString"), Session("UserConnProvider"))  'Data for report from SP
        'End If
        If dv3 Is Nothing OrElse dv3.Count = 0 Then
            LabelRowCount.Text = "Records returned: " & 0.ToString '& " from " & DropDownTables.Text
            If r <> "" Then
                MessageBox.Show("ERROR!! " & r, "Data for report ", "NoData", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            End If
            Return dv3
        End If
        If dv3.Count > 0 Then
            LabelRowCount.Text = "Records returned: " & dv3.Table.Rows.Count.ToString '& " from " & DropDownTables.Text
        Else
            LabelRowCount.Text = "Records returned: " & 0.ToString '& " from " & DropDownTables.Text
            Return dv3
        End If
        Dim er As String = String.Empty
        Session("mySQL") = mSql
        If Session("UserConnProvider") = "MySql.Data.MySqlClient" OrElse (Session("UserConnProvider") = "" And Session("OURConnProvider") = "MySql.Data.MySqlClient") Then
            dv3 = ConvertMySqlTable(dv3.Table, er).DefaultView
        ElseIf Session("UserConnProvider") = "Oracle.ManagedDataAccess.Client" Then
            dv3 = ConvertOracleTable(dv3.Table, er).DefaultView
            'dv3 = CorrectDataset(dv3.Table, er).DefaultView
        Else
            dv3 = CorrectDatasetColumns(dv3.Table, er).DefaultView  'from single quote
        End If
        If CheckBoxHideDuplicates.Checked AndAlso Not dv3 Is Nothing AndAlso dv3.Count > 0 Then
            dv3 = dv3.ToTable(True).DefaultView
        End If

        Dim dtf As New DataTable
        dtf = dv3.Table
        dtf = MakeDTColumnsNamesCLScompliant(dtf, Session("UserConnProvider"), r)
        dv3 = dtf.DefaultView
        '----------------------------------------------------------------------------------------------
        Session("dv3") = dv3
        Session("DataToChatAI") = ExportToCSVtext(dv3.Table, Chr(9))
        '----------------------------------------------------------------------------------------------

        If CheckBoxRelations.Checked Then
            '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'add cardinality or forein keys fields to dt, loop in property definition class records for this class.
            dv3 = AddRelationshipColumnsToDataTable(dv3, DropDownTables.Text, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"), CheckBoxSysTables.Checked)
        End If

        Return dv3
    End Function
    Protected Sub DropDownTables_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownTables.SelectedIndexChanged
        DropDownColumns.SelectedValue = " "
        DropDownOperator.SelectedValue = " "
        DropDownColumns.Text = " "
        DropDownOperator.Text = " "
        TextBoxSearch.Text = ""
        Dim i As Integer
        Dim tbl As String = DropDownTables.Text
        If tbl = "" Then
            LinkButtonAnalytics.Visible = False
            LinkButtonAnalytics.Enabled = False
        Else
            LinkButtonAnalytics.Visible = True
            LinkButtonAnalytics.Enabled = True
        End If
        If tbl.Trim = "" Then
            Exit Sub
        End If
        GridView1.Enabled = False

        DropDownColumns.Items.Clear()
        Dim ddt As DataTable = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
        DropDownColumns.Items.Add(" ")
        If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
            For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
                DropDownColumns.Items.Add(ddt.Rows(i)("COLUMN_NAME"))
            Next
        End If

        'retrieve data
        dv3 = RetrieveDataForReport()


        If dv3 Is Nothing Then   'OrElse dv3.Count = 0
            Session("dv3") = Nothing
            Session("DataToChatAI") = ""
            LabelRowCount.Text = "Records returned: " & 0.ToString '& " from " & DropDownTables.Text

            LabelError.Text = "No data"
            'MessageBox.Show("ERROR!! No data retrived.", "Data for report ", "NoData", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

            Exit Sub
        End If

        '----------------------------------------------------------------------------------------------
        Session("dv3") = dv3
        Session("DataToChatAI") = ExportToCSVtext(dv3.Table, Chr(9))
        '----------------------------------------------------------------------------------------------


        'apply search
        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        'Filter dv3
        Dim srch As String = SearchStatement()
        If srch.Trim <> "" Then
            dv3.RowFilter = srch
            Dim dtt As New DataTable
            dtt = dv3.ToTable

            If dtt.Rows Is Nothing Then
                Session("dv3") = Nothing
                Session("DataToChatAI") = ""
                LabelRowCount.Text = "Records returned: " & 0.ToString
                Exit Sub
            End If

            '----------------------------------------------------------------------------------------------
            Session("dv3") = dv3.ToTable
            Session("DataToChatAI") = ExportToCSVtext(dtt, Chr(9))
            '----------------------------------------------------------------------------------------------


            If dtt.Rows.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString '& " from " & DropDownTables.Text
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString '& " from " & DropDownTables.Text
            End If
            If Session("srch") <> srch Then
                Session("SortedView") = Nothing
                Session("SortColumn") = ""
            End If
            Session("srch") = srch
            'BindReportFields()
        ElseIf Session("srch") <> "" Then
            Session("SortedView") = Nothing
            Session("srch") = ""
            Session("SortColumn") = ""
        End If
        'Session("dv3") = dv3.ToTable
        If dv3 Is Nothing Then
            Session("dv3") = Nothing
            Session("DataToChatAI") = ""
            LabelRowCount.Text = "Records returned: " & 0.ToString '& " from " & DropDownTables.Text
            MessageBox.Show("ERROR!! No data retrieved", "Data for report ", "NoData", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

            Exit Sub
        End If
        ' show data in grid
        BindReportFields()
        FindReportsWithTable(tbl)
    End Sub

    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Dim srch As String = SearchStatement()
        Dim ret As String = String.Empty
        Try

            If srch.Trim <> "" Then
                'Filter dv3
                If dv3 Is Nothing Then
                    If Session("dv3") Is Nothing Then
                        dv3 = RetrieveDataForReport()
                    Else
                        dv3 = Session("dv3")

                    End If
                End If
                Try
                    dv3.RowFilter = srch
                Catch ex As Exception
                    ret = ex.Message
                End Try

                Dim dtt As New DataTable
                dtt = dv3.ToTable
                If dtt.Rows Is Nothing Then
                    LabelRowCount.Text = "Records returned: " & 0.ToString & " from " & DropDownTables.Text
                    BindReportFields()
                    Exit Sub
                End If
                If dtt.Rows.Count > 0 Then
                    LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString & " from " & DropDownTables.Text
                Else
                    LabelRowCount.Text = "Records returned: " & 0.ToString & " from " & DropDownTables.Text
                End If
                If Session("srch") <> srch Then
                    Session("SortedView") = Nothing
                    Session("SortColumn") = ""
                End If
                Session("srch") = srch
                BindReportFields()
                Session("sqltoexport") = "SELECT * FROM " & DropDownTables.Text & " WHERE " & srch
            ElseIf Session("srch") <> "" Then
                Session("SortedView") = Nothing
                Session("srch") = ""
                Session("SortColumn") = ""
            Else
                'recalculate dv3
                Session("sqltoexport") = "SELECT * FROM " & DropDownTables.Text
                dv3 = RetrieveDataForReport()
                BindReportFields()
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try

        If dv3 Is Nothing Then
            'recalculate dv3
            Session("sqltoexport") = "SELECT * FROM " & DropDownTables.Text
            dv3 = RetrieveDataForReport()
            BindReportFields()
        End If
        '----------------------------------------------------------------------------------------------
        Session("dv3") = dv3
        Session("DataToChatAI") = ExportToCSVtext(dv3.ToTable, Chr(9))
        '----------------------------------------------------------------------------------------------
        If dv3 Is Nothing Then

            MessageBox.Show("ERROR!! getting data", "Data for report ", "NoData", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)

        ElseIf dv3.Count = 0 Then
            LabelRowCount.Text = "Records returned: " & 0.ToString & " from " & DropDownTables.Text
            BindReportFields()
            Exit Sub
        End If
        FindReportsWithTable(DropDownTables.Text)
    End Sub

    Private Sub CheckBoxRelations_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxRelations.CheckedChanged
        'retrieve data
        dv3 = RetrieveDataForReport()
        Session("dv3") = dv3
        If dv3 Is Nothing OrElse dv3.Count = 0 Then
            LabelRowCount.Text = "Records returned: " & 0.ToString
            BindReportFields()
            Exit Sub
        End If
        'apply search
        '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        'Filter dv3
        Dim srch As String = SearchStatement()
        If srch.Trim <> "" Then
            dv3.RowFilter = srch
            Dim dtt As New DataTable
            dtt = dv3.ToTable
            If dtt.Rows Is Nothing Then
                LabelRowCount.Text = "Records returned: " & 0.ToString
                BindReportFields()
                Exit Sub
            End If
            If dtt.Rows.Count > 0 Then
                LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
            If Session("srch") <> srch Then
                Session("SortedView") = Nothing
                Session("SortColumn") = ""
            End If
            Session("srch") = srch
            'BindReportFields()
        ElseIf Session("srch") <> "" Then
            Session("SortedView") = Nothing
            Session("srch") = ""
            Session("SortColumn") = ""
        End If

        '----------------------------------------------------------------------------------------------
        Session("dv3") = dv3
        Session("DataToChatAI") = ExportToCSVtext(dv3.ToTable, Chr(9))
        '----------------------------------------------------------------------------------------------

        If dv3 Is Nothing OrElse dv3.Count = 0 Then
            LabelRowCount.Text = "Records returned: " & 0.ToString
            BindReportFields()
            Exit Sub
        End If
        ' show data in grid
        BindReportFields()
    End Sub

    Private Sub CheckBoxSysTables_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSysTables.CheckedChanged
        Dim er As String = String.Empty
        Dim ddt As DataTable
        Dim ddtv As DataView = Nothing
        Dim sel As Boolean = False
        Dim tbl As String = String.Empty
        tbl = DropDownTables.SelectedValue
        DropDownTables.Items.Clear()
        Session("syschk") = CheckBoxSysTables.Checked
        ddtv = GetListOfUserTables(CheckBoxSysTables.Checked, Session("UserConnString"), Session("UserConnProvider"), er, Session("logon"), Session("CSV"))
        If ddtv Is Nothing OrElse ddtv.Table Is Nothing Then
            LabelError.Text = "No tables nor fields selected into report yet..."
            LabelError.Visible = True
            Exit Sub
        End If
        If er <> "" Then
            LabelError.Text = er
            LabelError.Visible = True
            Exit Sub
        End If
        ddt = ddtv.Table
        DropDownTables.Items.Add(" ")
        For i = 0 To ddt.Rows.Count - 1   'draw drop-down start
            Dim li As New ListItem
            If Session("CSV") = "yes" Then
                li.Text = ddt.Rows(i)("TABLENAME").ToString & "(" & ddt.Rows(i)("TABLE_NAME").ToString & ")"
            Else
                li.Text = ddt.Rows(i)("TABLE_NAME").ToString
            End If
            li.Value = ddt.Rows(i)("TABLE_NAME").ToString
            DropDownTables.Items.Add(li)
            If li.Value = tbl Then
                DropDownTables.Items(i + 1).Selected = True
                DropDownTables.SelectedValue = ddt.Rows(i)("TABLE_NAME")
                DropDownTables.SelectedIndex = i + 1
                sel = True
            End If
        Next
        If sel = False Then
            DropDownTables.Items(0).Selected = True
            DropDownTables.SelectedValue = ddt.Rows(0)("TABLE_NAME")
            DropDownTables.SelectedIndex = 0
            dv3 = Nothing
            LabelRowCount.Text = "Records returned: " & 0.ToString
        End If
        BindReportFields()
    End Sub

    Private Sub btnCorrectFields_Click(sender As Object, e As EventArgs) Handles btnCorrectFields.Click
        Dim tbl As String = DropDownTables.Text
        Dim ret As String = CorrectFieldTypesInTable(tbl, Session("UserConnString"), Session("UserConnProvider"))
    End Sub

    Private Sub btnListOfTables_Click(sender As Object, e As EventArgs) Handles btnListOfTables.Click
        Response.Redirect("ListOfTables.aspx")
    End Sub

    Private Sub LinkButtonAnalytics_Click(sender As Object, e As EventArgs) Handles LinkButtonAnalytics.Click
        Dim tableid As String = DropDownTables.SelectedItem.Text.Trim
        Dim cln As String = FixReservedWords(CorrectTableNameWithDots(tableid, Session("UserConnProvider")), Session("UserConnProvider"))
        Dim dv1 As DataView
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        Dim repid As String = String.Empty
        'Check Report Info (title, data for report) for this class 
        dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & tableid & "') AND (ReportDB LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
        If dv1 Is Nothing OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then
            'TODO create new standard report
            repid = Session("dbname") & "_INIT_00" & tableid.Replace(" ", "") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
            ret = MakeNewStanardReport(Session("logon"), repid, tableid, Session("UserDB"), "SELECT * FROM " & cln, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
            If ret.Contains("ERROR!!") Then
                WriteToAccessLog(Session("logon"), "Initial Report created with errors:" & tableid & ", error:" & ret, 6)
                Session("REPORTID") = ""
            Else
                dv1 = mRecords("SELECT * FROM OURReportInfo WHERE (ReportTtl='" & tableid & "') AND (ReportDB LIKE '%" & Session("UserDB").Trim.Replace(" ", "%") & "%') AND (SQLqueryText='SELECT * FROM " & cln & "')")
            End If
        Else
            'add permissions for user if they are not there
            repid = dv1.Table.Rows(0)("ReportId").ToString
            Dim sqlq As String = "SELECT * FROM OURPermissions WHERE NetId='" & Session("logon") & "' AND Param1='" & repid & "'"
            If Not HasRecords(sqlq) Then
                'new permission required
                sqlq = "INSERT INTO OURPermissions (NetID,Application,Param1,Param2,Param3,AccessLevel,OpenFrom,OpenTo,COMMENTS) VALUES('" & Session("logon") & "','InteractiveReporting','" & repid & "','" & tableid & "','" & Session("email") & "','admin','" & DateToString(Now) & "','" & Session("UserEndDate") & "','initial')"
                Dim retr As String = ExequteSQLquery(sqlq)
            End If
        End If
        If dv1 Is Nothing OrElse dv1.Table Is Nothing OrElse dv1.Table.Rows.Count = 0 Then
            Session("REPORTID") = ""
        Else
            If dv1.Table.Rows(0)("ReportType").ToString = "" Then
                dv1.Table.Rows(0)("ReportType") = "rdl"
            End If
            repid = dv1.Table.Rows(0)("ReportId").ToString
            If repid <> "" Then Session("REPORTID") = repid
            Session("noedit") = dv1.Table.Rows(0)("Param7type").ToString
        End If
        If Not (dv1 Is Nothing OrElse dv1.Table Is Nothing) AndAlso dv1.Table.Rows.Count = 1 Then
            Session("REPTITLE") = dv1.Table.Rows(0)("ReportTtl").ToString
            Session("REPORTID") = repid
            'LabelReportTitle.Text = "Data for report:     " & reptitle
            'lblStatistics.Text = "Statistics for report: " & reptitle
            'LabelPageFtr.Text = dv1.Table.Rows(0)("Comments").ToString
            Session("PageFtr") = dv1.Table.Rows(0)("Comments").ToString
            'main.Rows(4).Cells(0).InnerHtml = "&nbsp;"
        Else
            'Response.Redirect("Nodata.aspx")
        End If
        Dim RptTtl As String = DropDownTables.SelectedItem.Text
        If repid.Trim <> "" Then
            Response.Redirect("ShowReport.aspx?srd=11&REPORT=" & repid)
        End If

    End Sub
    Private Sub btnListOfJoins_Click(sender As Object, e As EventArgs) Handles btnListOfJoins.Click
        Response.Redirect("ListOfJoins.aspx")
    End Sub

    Private Sub FindReportsWithTable(ByVal tblname As String)
        tblname = cleanText(tblname)
        If Session("UserConnProvider").StartsWith("InterSystems.Data.") Then
            If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
            If tblname.IndexOf(".") < 0 Then
                tblname = "userdata." & tblname
            End If
        End If
        If tblname = "" AndAlso DropDownTables.Items.Count > 1 AndAlso DropDownTables.SelectedItem.Value.Trim <> "" Then
            tblname = DropDownTables.SelectedItem.Value.Trim
            'ElseIf tblname = "" AndAlso DropDownTables.SelectedItem.Value.Trim = "" Then
            '    tblname = txtTableName.Text.Trim
        End If
        Session("tablenm") = tblname
        If tblname = "" Then
            Session("REPORTID") = ""
            Session("REPTITLE") = ""
            'TextBoxSQL.Text = ""
            'TextBoxReportTitle.Text = ""
            'TextboxPageFtr.Text = ""
            'TextboxRepID.Text = ""
            Exit Sub
        End If
        Try
            DropDownTables.SelectedItem.Value = tblname
        Catch ex As Exception

        End Try
        Dim SQLq As String = String.Empty ' 
        Dim dv6 As DataView = Nothing
        'Dim dr As DataTable
        Dim repttl As String = String.Empty ' Session("REPTITLE")
        Dim rep As String = String.Empty
        Dim userdb As String = Session("UserDB").trim
        Dim fnd As String = String.Empty
        Dim i As Integer = 0
        Dim er As String = String.Empty
        Dim SQLtext As String = String.Empty ' TextBoxSQL.Text.Trim

        SQLq = "SELECT DISTINCT OURReportInfo.ReportId, OURReportInfo.SQLquerytext, OURReportSQLquery.Tbl1, OURReportInfo.ReportDB,OURReportInfo.Comments,OURReportInfo.ReportTtl FROM OURReportSQLquery INNER JOIN OURPermissions  ON (OURReportSQLquery.ReportId = OURPermissions.Param1) INNER JOIN OURReportInfo ON (OURReportSQLquery.ReportId = OURReportInfo.ReportId) WHERE Tbl1='" & tblname.ToLower & "' AND NetId='" & Session("logon") & "' AND ReportDB LIKE '%" & userdb.Trim.Replace(" ", "%") & "%'"

        dv6 = mRecords(SQLq, er)
        If dv6 IsNot Nothing AndAlso dv6.Table IsNot Nothing AndAlso dv6.Table.Rows.Count > 0 Then
            LabelReports.Visible = True
            LabelReports.Text = "Reports with the table " & tblname & ": "
            trheaders.Visible = True
            List.Rows(1).Cells(0).InnerHtml = "  Data "
            List.Rows(1).Cells(0).Align = "left"
            List.Rows(1).Cells(1).InnerHtml = "  Analytics  "
            List.Rows(1).Cells(1).Align = "center"
            List.Rows(1).Cells(2).InnerHtml = "  Report  "
            List.Rows(1).Cells(2).Align = "center"

            For i = 1 To dv6.Table.Rows.Count
                rep = dv6.Table.Rows(i - 1)("ReportId").ToString
                'dr = GetReportInfo(rep)
                repttl = dv6.Table.Rows(i - 1)("ReportTtl").ToString
                If dv6.Table.Rows(i - 1)("SQLquerytext").ToString.ToUpper = "SELECT * FROM " & tblname.ToUpper Then  'fnd = "" AndAlso 
                    Session("REPORTID") = dv6.Table.Rows(i - 1)("ReportId").ToString
                    'TextBoxSQL.Text = dv6.Table.Rows(i - 1)("SQLquerytext").ToString
                    'TextBoxReportTitle.Text = repttl
                    Session("REPTITLE") = repttl
                    'TextboxPageFtr.Text = dv6.Table.Rows(i - 1)("comments").ToString
                    'TextboxRepID.Text = Session("REPORTID")
                    'fnd = "found"
                End If

                AddRowIntoHTMLtable(dv6.Table.Rows(i - 1), List)
                For j = 0 To List.Rows(i + 1).Cells.Count - 1
                    List.Rows(i + 1).Cells(j).InnerText = ""
                    List.Rows(i + 1).Cells(j).InnerHtml = ""
                Next
                If i Mod 2 = 0 Then
                    List.Rows(i + 1).BgColor = "#EFFBFB"
                Else
                    List.Rows(i + 1).BgColor = "white"
                End If

                'Dim ctl As New LinkButton
                'ctl.Text = repttl
                'ctl.ID = rep & "*" & repttl
                'ctl.ToolTip = "Show " & rep
                'ctl.CssClass = "NodeStyle"
                'ctl.Font.Size = 10
                'AddHandler ctl.Click, AddressOf btnShow_Click
                'list.Rows(i + 1).Cells(0).InnerText = ""
                'list.Rows(i + 1).Cells(0).Controls.Add(ctl)

                List.Rows(i + 1).Cells(0).InnerText = repttl
                List.Rows(i + 1).Cells(0).InnerHtml = "<a href='ShowReport.aspx?didata=yes&Report=" & rep & "' data-toggle=""tooltip"" title=""" & dv6.Table.Rows(i - 1)("SQLquerytext").ToString & """ Target=""_blank"">data</a>"
                List.Rows(i + 1).Cells(0).Align = "left"

                List.Rows(i + 1).Cells(1).InnerText = "    analytics   "
                List.Rows(i + 1).Cells(1).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?didata=yes&srd=11&Report=" & rep & "' data-toggle=""tooltip"" title=""DataAI"" Target=""_blank"">analytics</a>&nbsp;&nbsp;"
                List.Rows(i + 1).Cells(1).Align = "center"

                List.Rows(i + 1).Cells(2).InnerText = rep
                List.Rows(i + 1).Cells(2).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?didata=yes&srd=3&Report=" & rep & "' data-toggle=""tooltip"" title=""" & rep & """ Target=""_blank"">" & repttl & "</a>&nbsp;&nbsp;"
                List.Rows(i + 1).Cells(2).Align = "center"

                List.Rows(i + 1).Cells(3).InnerText = "  AI   "
                List.Rows(i + 1).Cells(3).InnerHtml = "&nbsp;&nbsp;<a href='ShowReport.aspx?didata=yes&srd=15&Report=" & rep & "' data-toggle=""tooltip"" title=""DataAI"" Target=""_blank"">AI</a>&nbsp;&nbsp;"
                List.Rows(i + 1).Cells(3).Align = "center"

                If i Mod 2 = 0 Then
                    List.Rows(i + 1).BgColor = "#EFFBFB"
                Else
                    List.Rows(i + 1).BgColor = "white"
                End If
            Next
        Else
            'txtTableName.Text = tblname
            DropDownTables.SelectedIndex = 0
            DropDownTables.SelectedItem.Value = " "
            'LabelReports.Visible = False
            'LabelReports.Text = " "
            'trheaders.Visible = False
            'If Not Session("CameFromReport") Then
            '    TextBoxSQL.Text = ""
            '    TextboxPageFtr.Text = ""
            '    TextBoxReportTitle.Text = ""
            '    Session("REPTITLE") = ""
            '    TextboxRepID.Text = ""
            'End If

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
        myfile = "ExploreData" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("dataTable") 'Session("GridView1DataSource")
        'header
        Dim hdr As String = "Explore data of report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
        Response.Redirect("ClassExplorer.aspx?export=GridData")


    End Sub
    Private Sub btnExportToTable_Click(sender As Object, e As EventArgs) Handles btnExportToTable.Click
        Dim er As String = String.Empty
        Dim tblname As String = cleanText(TextBoxExportTableName.Text.Trim)
        If tblname = "" Then
            LabelResult.Text = "Table name cannot be empty."
            Exit Sub
        End If
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

        er = InsertTableIntoOURUserTables(tblname, tblname, Session("Unit"), Session("logon"), Session("UserDB"), "", reportid)

        'create new table
        Try
            Dim dri As DataTable = GetReportInfo(reportid)
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

                        If tblname.StartsWith("User.") Then tblname = tblname.Replace("User.", "")
                        If tblname.IndexOf(".") < 0 Then
                            tblname = "userdata." & tblname
                        End If
                        dv3 = Session("dv3")
                        'use dv3 to create table tblname
                        Dim collen As Integer = 0
                        Dim selflds As String = String.Empty
                        Dim namflds As String = String.Empty
                        Dim fldtype(dv3.ToTable.Columns.Count) As String
                        Dim selfield(dv3.ToTable.Columns.Count) As String
                        Dim namfield(dv3.ToTable.Columns.Count) As String
                        For i = 0 To dv3.ToTable.Columns.Count - 1
                            selfield(i) = dv3.ToTable.Columns(i).Caption
                            fldtype(i) = dv3.ToTable.Columns(i).DataType.Name
                            namfield(i) = dv3.ToTable.Columns(i).Caption
                            If dv3.ToTable.Columns(i).DataType.Name = "String" Then
                                For j = 0 To dv3.ToTable.Rows.Count - 1
                                    collen = Max(collen, dv3.ToTable.Rows(j)(i).ToString.Length)
                                Next
                                collen = collen + 10
                                If selflds.Trim = "" Then
                                    selflds = selfield(i) & " " & fldtype(i).Replace("String", "nvarchar") & "(" & collen.ToString & ")" & " DEFAULT NULL"
                                    namflds = namfield(i)
                                Else
                                    selflds = selflds & "," & selfield(i) & " " & fldtype(i).Replace("String", "nvarchar") & "(" & collen.ToString & ")" & " DEFAULT NULL"
                                    namflds = namflds & "," & namfield(i)
                                End If

                            ElseIf dv3.ToTable.Columns(i).DataType.Name = "DateTime" OrElse dv3.ToTable.Columns(i).DataType.Name = "Decimal" Then
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

                    ElseIf Session("UserConnProvider").StartsWith("Sqlite") Then
                        sSql = sSql.Replace(" FROM ", " INTO " & tblname & " FROM ")

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
            If Not er.StartsWith("ERROR!!") Then
                er = CreateCleanReportColumnsFieldsItems(reportid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                'create new standard report
                repid = Session("dbname") & "_INIT_00" & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
                Dim re As String = MakeNewStanardReport(Session("logon"), repid, tblname, Session("UserDB"), "SELECT * FROM " & tblname, Session("dbname"), Session("email"), Session("UserEndDate"), Session("UserConnString"), Session("UserConnProvider"), er)
            End If

            DropDownTables.Items.Add(tblname)


            LabelResult.Text = "Table " & tblname & " and initial report for it completed with the result: " & er '& ". Refresh the page to see it in the list of tables/classes."
            MessageBox.Show("Table " & tblname & " and initial report for it have been completed with the result: " & er, "Table created ", "TableCreated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.PostOK)

            tblname = ""
            TextBoxExportTableName.Text = ""

            'Response.Redirect("ClassExplorer.aspx")  '?tbl=" & tblname)
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            LabelError.Visible = True
        End Try

    End Sub
    Private Function CreateReport(ByVal tblname As String, ByVal userdb As String, Optional ByRef reportid As String = "") As String
        Dim SQLq As String
        Dim dv5 As DataView
        Dim repttl As String = "Data exported into " & tblname & " on " & Now.ToString.Replace(": ", "-").Replace("/", "-")
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

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Select Case e.Tag
            Case "TableCreated"
                TextBoxExportTableName.Text = ""
                Session("ReportID") = ""

                Response.Redirect("ClassExplorer.aspx")
            Case "NoData"
                TextBoxExportTableName.Text = ""
                Session("ReportID") = ""

                Response.Redirect("ClassExplorer.aspx")
            Case "CreateError"
                TextBoxExportTableName.Text = ""
                Session("ReportID") = ""

                Response.Redirect("ClassExplorer.aspx")
            Case "ExportDataError"
                TextBoxExportTableName.Text = ""
                Session("ReportID") = ""

                Response.Redirect("ClassExplorer.aspx")
        End Select
    End Sub
End Class
