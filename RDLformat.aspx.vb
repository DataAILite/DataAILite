Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
'Imports Microsoft.Reporting.WebForms
Imports System.Drawing
Imports Newtonsoft.Json
Imports MyWebControls.DragList

Partial Class RDLformat
    Inherits System.Web.UI.Page
    Dim repid As String
    Dim dt As DataTable
    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLnk As LinkButton = CType(sender, LinkButton)
        Dim id As String = btnLnk.ID
        Dim tag As String = Piece(id, "^", 1)
        Dim link As String = Piece(id, "^", 2)

        Response.Redirect(link)
    End Sub
    Private Sub RDLformat_Init(sender As Object, e As EventArgs) Handles Me.Init
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Not IsPostBack AndAlso Not Request("tnf") Is Nothing AndAlso (Session("TabNF") Is Nothing OrElse Session("TabNF") <> Request("tnf")) Then
            Session("TabNF") = Request("tnf")
            lblView.Text = Menu1.Items(Session("TabNF")).Text
        End If
        'Dim repid = Session("REPORTID")
        LabelAlert.Text = Session("REPTITLE")
        LabelReprtID.Text = Session("REPORTID")
        ' js events
        'divColumnMenu.Attributes.Add("onclick", "showMenu(event);")
        divColumnMenu.Attributes.Add("onclick", "showColumnOrderDlg();")
        divColumnOrderDlgX.Attributes.Add("onclick", "closeColumnOrderDialog();")
        lstColumns.Attributes.Add("onchange", "onListChanged('lstColumns');")
        chkOldColumn.Attributes.Add("onchange", "chkOldColumnChanged(event);")
        btnColumnOrderDlgSave.OnClientClick = "saveColumnOrder(); return false;"
        btnColumnOrderDlgCancel.OnClientClick = "closeColumnOrderDialog();"
        btnOrderUp.OnClientClick = "moveUp();return false;"
        btnOrderDown.OnClientClick = "moveDown();return false;"
    End Sub
    Private Sub RDLformat_Load(sender As Object, e As EventArgs) Handles Me.Load
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim i As Integer
        Dim j As Integer
        Dim ddt, dtl, dtrf As DataTable
        Dim dv As DataView = Nothing
        Dim ctlLnk As LinkButton = Nothing

        Dim target As String = Page.Request("__EVENTTARGET")
        Dim data As String = Page.Request("__EVENTARGUMENT")
        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target = "SaveColumnOrder" Then
                Dim foSaved As FieldOrder = JsonConvert.DeserializeObject(Of FieldOrder)(data)
                If foSaved IsNot Nothing Then
                    Dim oi As OrderItem = Nothing
                    Dim sSql As String = Nothing

                    For i = 0 To foSaved.OrderItems.Length - 1
                        oi = foSaved.OrderItems(i)
                        sSql = "UPDATE OURReportFormat SET [Order]=" & oi.ItemOrder & " WHERE ReportId = '" & Session("REPORTID") & "' AND Val = '" & oi.Field & "' AND Prop = 'FIELDS'"
                        ExequteSQLquery(sSql)
                    Next
                    CopyFormattedFieldOrder(Session("REPORTID"))
                    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
                End If
            End If
            If target = "ShowReportDesigner" Then
                Response.Redirect("ReportDesigner.aspx?Report=" & Session("REPORTID"))
            End If
        End If
        'Threading.Thread.Sleep(1000)
        LabelSelected.Text = "Selected field " & DropDownCalcFields.Text & " for the group " & DropDownGroupFields.Text
        repid = Session("REPORTID")
        Page.Title = "Format for Report " & repid

        If Not (IsPostBack) Then
            MultiView1.ActiveViewIndex = 0
            Menu1.Items(0).Selected = True
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
        If Session("TabNF") IsNot Nothing AndAlso Session("TabNF") < Menu1.Items.Count Then
            MultiView1.ActiveViewIndex = Session("TabNF")
            Menu1.Items(Session("TabNF")).Selected = True
            lblView.Text = Menu1.Items(Session("TabNF")).Text
        End If
        'from init
        If Session("Attributes") = "sp" Then
            'disable Report Data tab
            MenuMain.Items(0).Enabled = False
            'remove left menu node for report data definition
            Dim treenodedata As WebControls.TreeNode = TreeView1.FindNode("SQLquery.aspx?tnq=0")
            If treenodedata IsNot Nothing Then TreeView1.Nodes.Remove(treenodedata)
        Else
            MenuMain.Items(0).Enabled = True
        End If

        Dim frname As String
        Dim ret As String = String.Empty


        'draw existing groups
        dt = GetReportGroups(repid)
        'delete list
        If Request("dellist") IsNot Nothing AndAlso Request("dellist").ToString = "yes" Then
            Dim listfld As String = Request("listfld").ToString
            Dim recfld As String = Request("recfld").ToString
            ret = DeleteReportList(repid, listfld, recfld)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        'delete group
        If Request("delgroup") IsNot Nothing AndAlso Request("delgroup").ToString = "yes" Then
            Dim grp As String = Request("grp").ToString
            Dim fld As String = Request("fld").ToString
            ret = DeleteReportGroup(repid, grp, fld)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        'up group
        If Request("up") IsNot Nothing AndAlso Request("up").ToString = "yes" Then
            Dim grp As String = Request("grp").ToString
            Dim fld As String = Request("fld").ToString
            SaveGroupsAndTotals(repid, dt)
            dt = GetReportGroups(repid) 'sorted by GrpOrder
            dt = CorrectGroupOrder(dt)
            UpReportGroup(dt, grp, fld)
            SaveGroupsAndTotals(repid, dt)
            ''correct order in group with multi calc fields
            'CorrectMultiGroupOrder(dt)
            'SaveGroupsAndTotals(repid, dt)
            'dt = GetReportGroups(repid) 'sorted by GrpOrder
            'CorrectGroupOrder(dt)
            'SaveGroupsAndTotals(repid, dt)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        'down group
        If Request("down") IsNot Nothing AndAlso Request("down").ToString = "yes" Then
            Dim grp As String = Request("grp").ToString
            Dim fld As String = Request("fld").ToString
            SaveGroupsAndTotals(repid, dt)
            dt = GetReportGroups(repid) 'sorted by GrpOrder
            CorrectGroupOrder(dt)
            DownReportGroup(dt, grp, fld)
            SaveGroupsAndTotals(repid, dt)
            ''correct order in group with multi calc fields
            'CorrectMultiGroupOrder(dt)
            'SaveGroupsAndTotals(repid, dt)
            'dt = GetReportGroups(repid) 'sorted by GrpOrder
            'CorrectGroupOrder(dt)
            'SaveGroupsAndTotals(repid, dt)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        'delete report column
        If Request("delrepfld") IsNot Nothing AndAlso Request("delrepfld").ToString = "yes" Then
            Dim repfld As String = Request("repfld").ToString
            DeleteReportColumn(repid, repfld)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        'up report field
        If Request("uprepfld") IsNot Nothing AndAlso Request("uprepfld").ToString = "yes" Then
            Dim repfld As String = Request("repfld").ToString
            'Dim dtrf As DataTable '= GetReportFields(repid) 'sorted by Order
            dtrf = UpReportField(repid, repfld)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        'down report field
        If Request("downrepfld") IsNot Nothing AndAlso Request("downrepfld").ToString = "yes" Then
            Dim repfld As String = Request("repfld").ToString
            'Dim dtrf As DataTable '= GetReportFields(repid) 'sorted by Order
            dtrf = DownReportField(repid, repfld)
            Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        End If
        '***************************************************************************************



        Try
            If Not IsPostBack Then
                DropDownGroupFields.Items.Clear()
                DropDownCalcFields.Items.Clear()
                DropDownListFields.Items.Clear()
                DropDownListRecFields.Items.Clear()
                DropDownRepFields.Items.Clear()

                Dim bfld As Boolean = False
                Dim insSQL As String = String.Empty
                Dim delSQL As String = String.Empty

                ddt = GetListOfReportFields(repid)  'List of Report fields from xsd
                If ddt Is Nothing Then
                    Exit Sub
                End If

                'add report fields from  OURReportFormat
                dtrf = GetReportFields(repid)
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
                Else
                    ''delete from OURReportFormat fields that are not in dtt
                    'WHY IS IT? in case the field was deleted from data fields or from sql statement
                    For i = 0 To dtrf.Rows.Count - 1
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

                Dim li As ListItem

                'draw dropdown start
                For i = 0 To ddt.Columns.Count - 1
                    'If ddt.Columns(i).DataType.Name = "String" OrElse ddt.Columns(i).DataType.Name = "DateTime" Then
                    '    DropDownRepFields.Items.Add(ddt.Columns(i).Caption)
                    '    If IsFieldInReportFormat(repid, ddt.Columns(i).Caption) Then
                    '        DropDownGroupFields.Items.Add(ddt.Columns(i).Caption)
                    '        DropDownListFields.Items.Add(ddt.Columns(i).Caption)
                    '        DropDownListRecFields.Items.Add(ddt.Columns(i).Caption)
                    '        DropDownCalcFields.Items.Add(ddt.Columns(i).Caption)
                    '    End If
                    'Else
                    '    DropDownRepFields.Items.Add(ddt.Columns(i).Caption)
                    '    If IsFieldInReportFormat(repid, ddt.Columns(i).Caption) Then
                    '        DropDownCalcFields.Items.Add(ddt.Columns(i).Caption)
                    '        DropDownListRecFields.Items.Add(ddt.Columns(i).Caption)
                    '    End If
                    'End If
                    'If ddt.Columns(i).DataType.Name = "String" OrElse ddt.Columns(i).DataType.Name = "DateTime" Then
                    DropDownGroupFields.Items.Add(ddt.Columns(i).Caption)
                    'DropDownListFields.Items.Add(ddt.Columns(i).Caption)
                    DropDownListRecFields.Items.Add(ddt.Columns(i).Caption)
                    DropDownCalcFields.Items.Add(ddt.Columns(i).Caption)
                    DropDownRepFields.Items.Add(ddt.Columns(i).Caption)
                    If IsFieldInReportFormat(repid, ddt.Columns(i).Caption) Then
                        bfld = False
                        For j = 0 To dtrf.Rows.Count - 1
                            If ddt.Columns(i).Caption = dtrf.Rows(j)("Val").ToString Then
                                bfld = True
                                Exit For
                            End If
                        Next
                        If bfld Then
                            Dim frN As String = dtrf.Rows(j)("Prop1").ToString   'friendly name
                            If frN = String.Empty Then frN = dtrf.Rows(j)("Val").ToString
                            li = New ListItem(dtrf.Rows(j)("Val").ToString, frN)
                            DropDownListFields.Items.Add(li)
                        End If

                        'DropDownGroupFields.Items.Add(ddt.Columns(i).Caption)
                        ''DropDownListFields.Items.Add(ddt.Columns(i).Caption)
                        'DropDownListRecFields.Items.Add(ddt.Columns(i).Caption)
                        'DropDownCalcFields.Items.Add(ddt.Columns(i).Caption)
                    End If

                    'Else
                    '    DropDownRepFields.Items.Add(ddt.Columns(i).Caption)
                    '    If IsFieldInReportFormat(repid, ddt.Columns(i).Caption) Then
                    '        DropDownCalcFields.Items.Add(ddt.Columns(i).Caption)
                    '        DropDownListRecFields.Items.Add(ddt.Columns(i).Caption)
                    '    End If
                    'End If
                Next

                For i = 0 To dtrf.Rows.Count - 1
                    bfld = False
                    For j = 0 To ddt.Columns.Count - 1
                        If ddt.Columns(j).Caption = dtrf.Rows(i)("Val").ToString Then
                            bfld = True
                        End If
                    Next
                    If Not bfld Then
                        DropDownRepFields.Items.Add(dtrf.Rows(i)("Val"))
                    End If

                    '    DropDownRepFields.Items.Add(dtrf.Rows(i)("Val"))
                    '    If IsFieldInReportFormat(repid, dtrf.Rows(i)("Val")) Then
                    '        DropDownGroupFields.Items.Add(dtrf.Rows(i)("Val"))
                    '        DropDownListFields.Items.Add(dtrf.Rows(i)("Val"))
                    '        DropDownListRecFields.Items.Add(dtrf.Rows(i)("Val"))
                    '        DropDownCalcFields.Items.Add(dtrf.Rows(i)("Val"))
                    '    End If
                Next

                If HasReportColumns(repid) AndAlso Not ReportItemsExist(repid) Then
                    Dim retr As String = CreateReportItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                    If retr.StartsWith("ERROR!!") Then
                        LabelAlert.Text = "Report info is not updated. " & retr
                        LabelAlert.Visible = True
                        Exit Sub
                    End If
                End If

                DropDownGroupFields.Items.Add("Overall")
                DropDownGroupFields.Items(0).Selected = True
                If DropDownCalcFields.Items.Count > 0 Then DropDownCalcFields.Items(0).Selected = True
                If DropDownListFields.Items.Count > 0 Then DropDownListFields.Items(0).Selected = True
                If DropDownListRecFields.Items.Count > 0 Then DropDownListRecFields.Items(0).Selected = True

                'get friendly group name
                TextCommentsGroup.Text = GetFriendlyReportGroupName(repid, DropDownGroupFields.Text, DropDownCalcFields.Text)
                If TextCommentsGroup.Text = DropDownGroupFields.Text Then
                    TextCommentsGroup.Text = GetSuggestedFriendlyGroupName(repid, DropDownGroupFields.Text)
                    TextCommentsGroup.BackColor = Color.LightSalmon
                Else
                    TextCommentsGroup.BackColor = Color.White
                End If

                'functions
                If DropDownFunctionsType.Text = "Text" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("Format")
                    DropDownFunctions.Items.Add("FormatCurrency")
                    DropDownFunctions.Items.Add("FormatDateTime")
                    DropDownFunctions.Items.Add("FormatNumber")
                    DropDownFunctions.Items.Add("FormatPercent")
                    DropDownFunctions.Items.Add("LCase")
                    DropDownFunctions.Items.Add("Left")
                    DropDownFunctions.Items.Add("Len")
                    DropDownFunctions.Items.Add("LTrim")
                    DropDownFunctions.Items.Add("Mid")
                    DropDownFunctions.Items.Add("Replace")
                    DropDownFunctions.Items.Add("Right")
                    DropDownFunctions.Items.Add("RTrim")
                    DropDownFunctions.Items.Add("Trim")
                    DropDownFunctions.Items.Add("UCase")
                ElseIf DropDownFunctionsType.Text = "Math" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("Abs")
                    DropDownFunctions.Items.Add("Acos")
                    DropDownFunctions.Items.Add("Asin")
                    DropDownFunctions.Items.Add("Atan")
                    DropDownFunctions.Items.Add("Cos")
                    DropDownFunctions.Items.Add("Exp")
                    DropDownFunctions.Items.Add("Int")
                    DropDownFunctions.Items.Add("Log")
                    DropDownFunctions.Items.Add("Log10")
                    DropDownFunctions.Items.Add("Round")
                    DropDownFunctions.Items.Add("Sign")
                    DropDownFunctions.Items.Add("Sin")
                    DropDownFunctions.Items.Add("Tan")
                ElseIf DropDownFunctionsType.Text = "DateTime" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("DateAdd")
                    DropDownFunctions.Items.Add("Day")
                    DropDownFunctions.Items.Add("FormatDateTime")
                    DropDownFunctions.Items.Add("Hour")
                    DropDownFunctions.Items.Add("Minute")
                    DropDownFunctions.Items.Add("MonthName")
                    DropDownFunctions.Items.Add("Now")
                    DropDownFunctions.Items.Add("Second")
                    DropDownFunctions.Items.Add("TimeValue")
                    DropDownFunctions.Items.Add("Today")
                    DropDownFunctions.Items.Add("Weekday")
                    DropDownFunctions.Items.Add("WeekdayName")
                    DropDownFunctions.Items.Add("Year")
                ElseIf DropDownFunctionsType.Text = "Aggregate" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("Avg")
                    DropDownFunctions.Items.Add("Count")
                    DropDownFunctions.Items.Add("CountDistinct")
                    DropDownFunctions.Items.Add("CountRows")
                    DropDownFunctions.Items.Add("First")
                    DropDownFunctions.Items.Add("Last")
                    DropDownFunctions.Items.Add("Max")
                    DropDownFunctions.Items.Add("Min")
                    DropDownFunctions.Items.Add("StDev")
                ElseIf DropDownFunctionsType.Text = "Financial" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("DDB")
                    DropDownFunctions.Items.Add("FV")
                    DropDownFunctions.Items.Add("IPmt")
                    DropDownFunctions.Items.Add("NPer")
                    DropDownFunctions.Items.Add("Pmt")
                    DropDownFunctions.Items.Add("PPmt")
                    DropDownFunctions.Items.Add("PV")
                    DropDownFunctions.Items.Add("Rate")
                    DropDownFunctions.Items.Add("SLN")
                    DropDownFunctions.Items.Add("SYD")
                ElseIf DropDownFunctionsType.Text = "Conversion" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("Fix")
                    DropDownFunctions.Items.Add("Hex")
                    DropDownFunctions.Items.Add("Int")
                    DropDownFunctions.Items.Add("Oct")
                    DropDownFunctions.Items.Add("Str")
                    DropDownFunctions.Items.Add("Val")
                ElseIf DropDownFunctionsType.Text = "Statistics" Then
                    DropDownFunctions.Items.Clear()
                    DropDownFunctions.Items.Add(" ")
                    DropDownFunctions.Items.Add("Avg")
                    DropDownFunctions.Items.Add("Max")
                    DropDownFunctions.Items.Add("Min")
                    DropDownFunctions.Items.Add("Sum")
                    DropDownFunctions.Items.Add("StDev")
                    DropDownFunctions.Items.Add("Count")
                    DropDownFunctions.Items.Add("CountDistinct")
                End If

                TextBoxExpression.Text = ""
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try



        ''draw existing groups
        'dt = GetReportGroups(repid)
        ''delete list
        'If Request("dellist") IsNot Nothing AndAlso Request("dellist").ToString = "yes" Then
        '    Dim listfld As String = Request("listfld").ToString
        '    Dim recfld As String = Request("recfld").ToString
        '    DeleteReportList(repid, listfld, recfld)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''delete group
        'If Request("delgroup") IsNot Nothing AndAlso Request("delgroup").ToString = "yes" Then
        '    Dim grp As String = Request("grp").ToString
        '    Dim fld As String = Request("fld").ToString
        '    DeleteReportGroup(repid, grp, fld)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''up group
        'If Request("up") IsNot Nothing AndAlso Request("up").ToString = "yes" Then
        '    Dim grp As String = Request("grp").ToString
        '    Dim fld As String = Request("fld").ToString
        '    SaveGroupsAndTotals(repid, dt)
        '    dt = GetReportGroups(repid) 'sorted by GrpOrder
        '    dt = CorrectGroupOrder(dt)
        '    UpReportGroup(dt, grp, fld)
        '    SaveGroupsAndTotals(repid, dt)
        '    ''correct order in group with multi calc fields
        '    'CorrectMultiGroupOrder(dt)
        '    'SaveGroupsAndTotals(repid, dt)
        '    'dt = GetReportGroups(repid) 'sorted by GrpOrder
        '    'CorrectGroupOrder(dt)
        '    'SaveGroupsAndTotals(repid, dt)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''down group
        'If Request("down") IsNot Nothing AndAlso Request("down").ToString = "yes" Then
        '    Dim grp As String = Request("grp").ToString
        '    Dim fld As String = Request("fld").ToString
        '    SaveGroupsAndTotals(repid, dt)
        '    dt = GetReportGroups(repid) 'sorted by GrpOrder
        '    CorrectGroupOrder(dt)
        '    DownReportGroup(dt, grp, fld)
        '    SaveGroupsAndTotals(repid, dt)
        '    ''correct order in group with multi calc fields
        '    'CorrectMultiGroupOrder(dt)
        '    'SaveGroupsAndTotals(repid, dt)
        '    'dt = GetReportGroups(repid) 'sorted by GrpOrder
        '    'CorrectGroupOrder(dt)
        '    'SaveGroupsAndTotals(repid, dt)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''delete report column
        'If Request("delrepfld") IsNot Nothing AndAlso Request("delrepfld").ToString = "yes" Then
        '    Dim repfld As String = Request("repfld").ToString
        '    DeleteReportColumn(repid, repfld)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''up report field
        'If Request("uprepfld") IsNot Nothing AndAlso Request("uprepfld").ToString = "yes" Then
        '    Dim repfld As String = Request("repfld").ToString
        '    'Dim dtrf As DataTable '= GetReportFields(repid) 'sorted by Order
        '    dtrf = UpReportField(repid, repfld)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''down report field
        'If Request("downrepfld") IsNot Nothing AndAlso Request("downrepfld").ToString = "yes" Then
        '    Dim repfld As String = Request("repfld").ToString
        '    'Dim dtrf As DataTable '= GetReportFields(repid) 'sorted by Order
        '    dtrf = DownReportField(repid, repfld)
        '    Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        'End If
        ''***************************************************************************************

        'List of Report Fields
        ddt = GetListOfReportFields(repid)
        If ddt Is Nothing Then
            Exit Sub
        End If
        Dim id As String = String.Empty

        'draw existing groups
        dt = GetReportGroups(repid)
        For i = 0 To dt.Rows.Count - 1
            AddRowIntoHTMLtable(dt.Rows(i), RDLgroups)
            RDLgroups.Rows(i + 1).Cells(0).InnerHtml = dt.Rows(i)("GroupField").ToString
            RDLgroups.Rows(i + 1).Cells(1).InnerHtml = dt.Rows(i)("CalcField").ToString
            RDLgroups.Rows(i + 1).Cells(2).InnerHtml = dt.Rows(i)("Comments").ToString 'friendly name

            If dt.Rows(i)("CntChk") = 1 Then
                RDLgroups.Rows(i + 1).Cells(3).InnerHtml = "<input id='Cnt" & i.ToString & "' type='checkbox' name='Cnt" & i.ToString & "' checked/>  "
            Else
                RDLgroups.Rows(i + 1).Cells(3).InnerHtml = "<input id='Cnt" & i.ToString & "' type='checkbox' name='Cnt" & i.ToString & "'/>  "
            End If

            If RepFieldIsNumeric(ddt, dt.Rows(i)("CalcField").ToString) Then

                If dt.Rows(i)("SumChk") = 1 Then
                    RDLgroups.Rows(i + 1).Cells(4).InnerHtml = "<input id='Sum" & i.ToString & "' type='checkbox' name='Sum" & i.ToString & "' checked/>  "
                Else
                    RDLgroups.Rows(i + 1).Cells(4).InnerHtml = "<input id='Sum" & i.ToString & "' type='checkbox' name='Sum" & i.ToString & "'/>  "
                End If
                If dt.Rows(i)("MaxChk") = 1 Then
                    RDLgroups.Rows(i + 1).Cells(5).InnerHtml = "<input id='Max" & i.ToString & "' type='checkbox' name='Max" & i.ToString & "' checked/>  "
                Else
                    RDLgroups.Rows(i + 1).Cells(5).InnerHtml = "<input id='Max" & i.ToString & "' type='checkbox' name='Max" & i.ToString & "'/>  "
                End If
                If dt.Rows(i)("MinChk") = 1 Then
                    RDLgroups.Rows(i + 1).Cells(6).InnerHtml = "<input id='Min" & i.ToString & "' type='checkbox' name='Min" & i.ToString & "' checked/>  "
                Else
                    RDLgroups.Rows(i + 1).Cells(6).InnerHtml = "<input id='Min" & i.ToString & "' type='checkbox' name='Min" & i.ToString & "'/>  "
                End If
                If dt.Rows(i)("AvgChk") = 1 Then
                    RDLgroups.Rows(i + 1).Cells(7).InnerHtml = "<input id='Avg" & i.ToString & "' type='checkbox' name='Avg" & i.ToString & "' checked/>  "
                Else
                    RDLgroups.Rows(i + 1).Cells(7).InnerHtml = "<input id='Avg" & i.ToString & "' type='checkbox' name='Avg" & i.ToString & "'/>  "
                End If
                If dt.Rows(i)("StDevChk") = 1 Then
                    RDLgroups.Rows(i + 1).Cells(8).InnerHtml = "<input id='StDev" & i.ToString & "' type='checkbox' name='StDev" & i.ToString & "' checked/>  "
                Else
                    RDLgroups.Rows(i + 1).Cells(8).InnerHtml = "<input id='StDev" & i.ToString & "' type='checkbox' name='StDev" & i.ToString & "'/>  "
                End If

            Else
                RDLgroups.Rows(i + 1).Cells(4).InnerHtml = "  "
                RDLgroups.Rows(i + 1).Cells(5).InnerHtml = "  "
                RDLgroups.Rows(i + 1).Cells(6).InnerHtml = "  "
                RDLgroups.Rows(i + 1).Cells(7).InnerHtml = "  "
                RDLgroups.Rows(i + 1).Cells(8).InnerHtml = "  "
            End If
            If dt.Rows(i)("CntDistChk") = 1 Then
                RDLgroups.Rows(i + 1).Cells(9).InnerHtml = "<input id='CntDist" & i.ToString & "' type='checkbox' name='CntDist" & i.ToString & "' checked/>  "
            Else
                RDLgroups.Rows(i + 1).Cells(9).InnerHtml = "<input id='CntDist" & i.ToString & "' type='checkbox' name='CntDist" & i.ToString & "'/>  "
            End If
            If dt.Rows(i)("FirstChk") = 1 Then
                RDLgroups.Rows(i + 1).Cells(10).InnerHtml = "<input id='Frst" & i.ToString & "' type='checkbox' name='Frst" & i.ToString & "' checked/>  "
            Else
                RDLgroups.Rows(i + 1).Cells(10).InnerHtml = "<input id='Frst" & i.ToString & "' type='checkbox' name='Frst" & i.ToString & "'/>  "
            End If
            If dt.Rows(i)("LastChk") = 1 Then
                RDLgroups.Rows(i + 1).Cells(11).InnerHtml = "<input id='Lst" & i.ToString & "' type='checkbox' name='Lst" & i.ToString & "' checked/>  "
            Else
                RDLgroups.Rows(i + 1).Cells(11).InnerHtml = "<input id='Lst" & i.ToString & "' type='checkbox' name='Lst" & i.ToString & "'/>  "
            End If
            If dt.Rows(i)("PageBrk") = 1 Then
                RDLgroups.Rows(i + 1).Cells(12).InnerHtml = "<input id='Brk" & i.ToString & "' type='checkbox' name='Brk" & i.ToString & "' checked/>  "
            Else
                RDLgroups.Rows(i + 1).Cells(12).InnerHtml = "<input id='Brk" & i.ToString & "' type='checkbox' name='Brk" & i.ToString & "'/>  "
            End If
            RDLgroups.Rows(i + 1).Cells(13).InnerHtml = dt.Rows(i)("GrpOrder").ToString

            If i = 0 Then
                RDLgroups.Rows(i + 1).Cells(14).InnerText = String.Empty
            Else
                id = "MoveGroupUp^RDLformat.aspx?Report=" & repid & "&grp=" & dt.Rows(i)("GroupField").ToString & "&fld=" & dt.Rows(i)("CalcField").ToString & "&up=yes"
                ctlLnk = New LinkButton
                ctlLnk.Text = "up"
                ctlLnk.ID = id
                ctlLnk.ToolTip = "Move Group Up"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                RDLgroups.Rows(i + 1).Cells(14).InnerText = String.Empty
                RDLgroups.Rows(i + 1).Cells(14).Controls.Add(ctlLnk)
            End If
            If i = dt.Rows.Count - 1 Then
                RDLgroups.Rows(i + 1).Cells(15).InnerText = String.Empty
            Else
                id = "MoveGroupUp^RDLformat.aspx?Report=" & repid & "&grp=" & dt.Rows(i)("GroupField").ToString & "&fld=" & dt.Rows(i)("CalcField").ToString & "&down=yes"
                ctlLnk = New LinkButton
                ctlLnk.Text = "down"
                ctlLnk.ID = id
                ctlLnk.ToolTip = "Move Group Down"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                RDLgroups.Rows(i + 1).Cells(15).InnerText = String.Empty
                RDLgroups.Rows(i + 1).Cells(15).Controls.Add(ctlLnk)
            End If

            id = "DeleteGroup^RDLformat.aspx?Report=" & repid & "&grp=" & dt.Rows(i)("GroupField").ToString & "&fld=" & dt.Rows(i)("CalcField").ToString & "&delgroup=yes"
            ctlLnk = New LinkButton
            ctlLnk.Text = "delete"
            ctlLnk.ID = id
            ctlLnk.ToolTip = "Delete Group"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            RDLgroups.Rows(i + 1).Cells(16).InnerText = String.Empty
            RDLgroups.Rows(i + 1).Cells(16).Controls.Add(ctlLnk)
        Next

        'draw existing groups for combined fields
        dtl = GetReportLists(repid)
        For i = 0 To dtl.Rows.Count - 1
            AddRowIntoHTMLtable(dtl.Rows(i), RDLlists)
            If dtl.Rows(i)("Prop1").ToString.Trim = "" Then
                RDLlists.Rows(i + 1).Cells(0).InnerHtml = dtl.Rows(i)("RecFld").ToString
                id = "DeleteCombinedField^RDLformat.aspx?Report=" & repid & "&listfld=" & dtl.Rows(i)("ListFld").ToString & "&recfld=" & dtl.Rows(i)("RecFld").ToString & "&dellist=yes"

            Else
                RDLlists.Rows(i + 1).Cells(0).InnerHtml = dtl.Rows(i)("Prop1").ToString
                id = "DeleteCombinedField^RDLformat.aspx?Report=" & repid & "&listfld=" & dtl.Rows(i)("ListFld").ToString & "&recfld=" & dtl.Rows(i)("Prop1").ToString & "&dellist=yes"

            End If

            RDLlists.Rows(i + 1).Cells(1).InnerHtml = dtl.Rows(i)("RecFld").ToString
            RDLlists.Rows(i + 1).Cells(2).InnerHtml = dtl.Rows(i)("ListFld").ToString
            RDLlists.Rows(i + 1).Cells(3).InnerHtml = dtl.Rows(i)("Prop2").ToString

            ctlLnk = New LinkButton
            ctlLnk.Text = "delete"
            ctlLnk.ID = id
            ctlLnk.ToolTip = "Delete Combined Field"
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            RDLlists.Rows(i + 1).Cells(4).InnerText = String.Empty
            RDLlists.Rows(i + 1).Cells(4).Controls.Add(ctlLnk)

        Next

        'add report fields from  OURReportFormat
        dtrf = GetReportFields(repid)

        If dtrf.Rows.Count = 0 Then  'no records of Report Fields in OURReportFormat, insert them from ddt aka xsd fields...
            'add all fields from ddt
            For i = 0 To ddt.Columns.Count - 1
                If ddt.Columns(i).Caption <> "Indx" Then
                    frname = GetFriendlySQLFieldName(repid, ddt.Columns(i).Caption)
                    If frname.Trim = ddt.Columns(i).Caption.Trim Then frname = ""
                    Dim insSQL As String = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order], Prop1) VALUES('" & Session("REPORTID") & "','FIELDS','" & ddt.Columns(i).Caption & "'," & (i + 1).ToString & ",'" & frname & "')"
                    ExequteSQLquery(insSQL)
                End If
            Next
        End If
        dtrf = GetReportFields(repid)
        Dim fldOrder As FieldOrder = SetFieldOrder(repid)
        Dim di As DragItem
        Dim itm As OrderItem
        Dim FieldOrderItems As New DragListItems
        Dim idx As Integer = 0

        ' add items to lstColumns
        For i = 0 To fldOrder.OrderItems.Length - 1
            di = New DragItem
            itm = fldOrder.OrderItems(i)
            di.Text = itm.Field
            di.ID = itm.Field
            di.Tag = itm.ItemOrder
            ReDim Preserve FieldOrderItems.Items(idx)
            FieldOrderItems.Items(idx) = di
            idx += 1
        Next
        lstColumns.DragItems = FieldOrderItems
        hdnFieldOrder.Value = JsonConvert.SerializeObject(fldOrder)
        'dtrf = CorrectFieldOrder(dtrf, "Order")
        For i = 0 To dtrf.Rows.Count - 1
            AddRowIntoHTMLtable(dtrf.Rows(i), RDLrepfields)
            RDLrepfields.Rows(i + 1).Cells(0).InnerHtml = dtrf.Rows(i)("Val").ToString

            ctlLnk = New LinkButton
            ctlLnk.Text = "delete"
            ctlLnk.ID = "DeleteColumn^RDLformat.aspx?Report=" & repid & "&repfld=" & dtrf.Rows(i)("Val").ToString & "&delrepfld=yes"
            ctlLnk.ToolTip = "Delete " & dtrf.Rows(i)("Val")
            AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
            RDLrepfields.Rows(i + 1).Cells(1).InnerText = String.Empty
            RDLrepfields.Rows(i + 1).Cells(1).Controls.Add(ctlLnk)
            If i = 0 Then
                RDLrepfields.Rows(i + 1).Cells(2).InnerText = String.Empty
            Else
                ctlLnk = New LinkButton
                ctlLnk.Text = "up"
                ctlLnk.ID = "MoveColumnUp^RDLformat.aspx?Report=" & repid & "&repfld=" & dtrf.Rows(i)("Val").ToString & "&uprepfld=yes"
                ctlLnk.ToolTip = "Move " & dtrf.Rows(i)("Val") & " up"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                RDLrepfields.Rows(i + 1).Cells(2).InnerText = String.Empty
                RDLrepfields.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)
            End If
            If i = dtrf.Rows.Count - 1 Then
                RDLrepfields.Rows(i + 1).Cells(3).InnerText = String.Empty
            Else
                ctlLnk = New LinkButton
                ctlLnk.Text = "down"
                ctlLnk.ID = "MoveColumnDown^RDLformat.aspx?Report=" & repid & "&repfld=" & dtrf.Rows(i)("Val").ToString & "&downrepfld=yes"
                ctlLnk.ToolTip = "Move " & dtrf.Rows(i)("Val") & " Down"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                RDLrepfields.Rows(i + 1).Cells(3).InnerText = String.Empty
                RDLrepfields.Rows(i + 1).Cells(3).Controls.Add(ctlLnk)
            End If
            RDLrepfields.Rows(i + 1).Cells(4).InnerHtml = dtrf.Rows(i)("Order").ToString
            RDLrepfields.Rows(i + 1).Cells(5).InnerHtml = dtrf.Rows(i)("Prop2").ToString
            RDLrepfields.Rows(i + 1).Cells(6).InnerHtml = dtrf.Rows(i)("Prop1").ToString
        Next
        frname = ""
        If TextBoxFieldFriendly.Text = "" Then
            'find friendly name
            frname = GetFriendlyReportFieldName(repid, DropDownRepFields.Text)

            If frname <> String.Empty AndAlso frname = DropDownRepFields.Text Then   'no friendly name yet
                frname = GetFriendlySQLFieldName(repid, DropDownRepFields.Text)
                'GetSuggestedFriendlyFieldName(DropDownRepFields.Text, repid)
                If frname <> "" Then TextBoxFieldFriendly.BackColor = Color.Coral
            Else
                TextBoxFieldFriendly.BackColor = Color.White
            End If
            If frname <> "" AndAlso frname <> DropDownRepFields.Text Then
                TextBoxFieldFriendly.Text = frname
            End If
        End If
        If chkOldColumn.Checked Then
            TextBoxFriendlyNameField.Text = GetFriendlyReportFieldName(repid, DropDownListFields.Text)
            TextBoxFriendlyNameField.Enabled = False
        Else
            TextBoxFriendlyNameField.Enabled = True
        End If

        LabelAlert1.Text = Session("ret")
        Select Case Session("TabNF")
            Case "0"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Columns_Expressions"
            Case "1"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Groups_and_Totals"
            Case "2"
                HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Combine_Column_Values"
        End Select

    End Sub
    Protected Sub DropDownGroupFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownGroupFields.SelectedIndexChanged
        LabelSelected.Text = "Selected field " & DropDownCalcFields.Text & " for the group " & DropDownGroupFields.SelectedItem.Text
    End Sub

    Protected Sub DropDownCalcFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownCalcFields.SelectedIndexChanged
        LabelSelected.Text = "Selected field " & DropDownCalcFields.Text & " for the group " & DropDownGroupFields.Text
        'get friendly group name
        TextCommentsGroup.Text = GetFriendlyReportGroupName(repid, DropDownGroupFields.Text, DropDownCalcFields.Text)
        If TextCommentsGroup.Text = DropDownGroupFields.Text Then
            TextCommentsGroup.Text = GetSuggestedFriendlyGroupName(repid, DropDownGroupFields.Text)
            TextCommentsGroup.BackColor = Color.LightSalmon
        Else
            TextCommentsGroup.BackColor = Color.White
        End If
    End Sub
    Protected Sub ButtonSubmit_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click
        'submit groups
        SaveGroupsAndTotalsInDT(dt)
        SaveGroupsAndTotals(repid, dt)
        dt = GetReportGroups(repid) 'sorted by GrpOrder
        CorrectGroupOrder(dt)
        MoveOverallOnTop(dt)
        SaveGroupsAndTotals(repid, dt)
        dt = GetReportGroups(repid) 'sorted by GrpOrder
        CorrectGroupOrder(dt)
        SaveGroupsAndTotals(repid, dt)
        'correct order in group with multi calc fields
        CorrectMultiGroupOrder(dt)
        SaveGroupsAndTotals(repid, dt)
        dt = GetReportGroups(repid) 'sorted by GrpOrder
        CorrectGroupOrder(dt)
        SaveGroupsAndTotals(repid, dt)
        'Set final group reportitem item orders
        UpdateGroupReportItemOrders(repid)
        Dim ret As String = UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
        ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        If ret.StartsWith("ERROR!!") Then
            WriteToAccessLog(Session("logon"), ret, 3)
        Else
            WriteToAccessLog(Session("logon"), ret, 3)
            ret = "Report Format has been updated. "
        End If
        LabelAlert1.Text = ret
        Session("ret") = ret
        Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Sub ButtonAddGroup_Click(sender As Object, e As EventArgs) Handles ButtonAddGroup.Click
        Dim sctSQL As String = "SELECT * FROM OURReportGroups WHERE (ReportID='" & Session("REPORTID") & "' AND GroupField='" & DropDownGroupFields.Text & "' AND CalcField='" & DropDownCalcFields.Text & "' )"
        If Not HasRecords(sctSQL) Then
            'insert
            Dim insSQL As String = "INSERT INTO OURReportGroups (ReportID,GroupField,CalcField,COMMENTS,GrpOrder) VALUES('" & Session("REPORTID") & "','" & DropDownGroupFields.Text & "','" & DropDownCalcFields.Text & "','" & TextCommentsGroup.Text & "',0)"
            ExequteSQLquery(insSQL)
            If DropDownGroupFields.Text <> "Overall" Then
                CreateGroupReportItem(Session("REPORTID"), DropDownGroupFields.Text, "0")
            End If
        Else
            'update
            Dim updSQL As String = "UPDATE OURReportGroups SET COMMENTS='" & TextCommentsGroup.Text & "' WHERE (ReportID='" & Session("REPORTID") & "' AND GroupField='" & DropDownGroupFields.Text & "' AND CalcField='" & DropDownCalcFields.Text & "' )"
            ExequteSQLquery(updSQL)
        End If
        Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Function SaveGroupsAndTotalsInDT(ByVal dt As DataTable) As String
        Dim err As String = String.Empty
        Dim i As Integer
        For i = 0 To dt.Rows.Count - 1
            '"Cnt" & i.ToString
            If Request("Cnt" & i.ToString) IsNot Nothing AndAlso Request("Cnt" & i.ToString).ToString = "on" Then
                dt.Rows(i)("CntChk") = 1
            Else
                dt.Rows(i)("CntChk") = 0
            End If
            If Request("Sum" & i.ToString) IsNot Nothing AndAlso Request("Sum" & i.ToString).ToString = "on" Then
                dt.Rows(i)("SumChk") = 1
            Else
                dt.Rows(i)("SumChk") = 0
            End If
            If Request("Max" & i.ToString) IsNot Nothing AndAlso Request("Max" & i.ToString).ToString = "on" Then
                dt.Rows(i)("MaxChk") = 1
            Else
                dt.Rows(i)("MaxChk") = 0
            End If
            If Request("Min" & i.ToString) IsNot Nothing AndAlso Request("Min" & i.ToString).ToString = "on" Then
                dt.Rows(i)("MinChk") = 1
            Else
                dt.Rows(i)("MinChk") = 0
            End If
            If Request("Avg" & i.ToString) IsNot Nothing AndAlso Request("Avg" & i.ToString).ToString = "on" Then
                dt.Rows(i)("AvgChk") = 1
            Else
                dt.Rows(i)("AvgChk") = 0
            End If
            If Request("StDev" & i.ToString) IsNot Nothing AndAlso Request("StDev" & i.ToString).ToString = "on" Then
                dt.Rows(i)("StDevChk") = 1
            Else
                dt.Rows(i)("StDevChk") = 0
            End If

            If Request("CntDist" & i.ToString) IsNot Nothing AndAlso Request("CntDist" & i.ToString).ToString = "on" Then
                dt.Rows(i)("CntDistChk") = 1
            Else
                dt.Rows(i)("CntDistChk") = 0
            End If
            If Request("Frst" & i.ToString) IsNot Nothing AndAlso Request("Frst" & i.ToString).ToString = "on" Then
                dt.Rows(i)("FirstChk") = 1
            Else
                dt.Rows(i)("FirstChk") = 0
            End If
            If Request("Lst" & i.ToString) IsNot Nothing AndAlso Request("Lst" & i.ToString).ToString = "on" Then
                dt.Rows(i)("LastChk") = 1
            Else
                dt.Rows(i)("LastChk") = 0
            End If

            If Request("Brk" & i.ToString) IsNot Nothing AndAlso Request("Brk" & i.ToString).ToString = "on" Then
                dt.Rows(i)("PageBrk") = 1
            Else
                dt.Rows(i)("PageBrk") = 0
            End If
            If dt.Rows(i)("CalcField").ToString.Trim = "" Then
                dt.Rows(i)("CalcField") = dt.Rows(i)("GroupField")
                dt.Rows(i)("SumChk") = 0
                dt.Rows(i)("MaxChk") = 0
                dt.Rows(i)("MinChk") = 0
                dt.Rows(i)("AvgChk") = 0
                dt.Rows(i)("StDevChk") = 0
            End If
        Next
        Return err
    End Function
    Protected Function CorrectGroupOrder(ByVal dt As DataTable) As DataTable
        Dim dtt As DataTable = dt
        Dim i As Integer
        'reorder
        For i = 0 To dtt.Rows.Count - 1
            dtt.Rows(i)("GrpOrder") = i + 1
        Next
        Return dtt
    End Function
    Protected Function CorrectMultiGroupOrder(ByVal dt As DataTable) As DataTable
        Dim dtt As DataTable = dt
        Dim i, j, n As Integer
        Dim grpf As String
        'group together
        For i = 0 To dtt.Rows.Count - 1
            grpf = dtt.Rows(i)("GroupField")
            n = dtt.Rows(i)("GrpOrder")
            For j = i To dtt.Rows.Count - 1
                If dtt.Rows(j)("GroupField") = grpf Then
                    dtt.Rows(j)("GrpOrder") = n
                End If
            Next
        Next
        Return dtt
    End Function
    Protected Function MoveOverallOnTop(ByVal dt As DataTable) As DataTable
        Dim dtt As DataTable = dt
        Dim i As Integer
        For i = 0 To dtt.Rows.Count - 1
            If dtt.Rows(i)("GroupField") = "Overall" Then dtt.Rows(i)("GrpOrder") = 0
        Next
        Return dtt
    End Function
    Public Function UpReportGroup(ByRef dt As DataTable, ByVal grp As String, ByVal fld As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim grpord As Integer = 0
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("GroupField") = grp AndAlso dt.Rows(i)("CalcField") = fld Then
                grpord = dt.Rows(i - 1)("GrpOrder")
                dt.Rows(i - 1)("GrpOrder") = dt.Rows(i)("GrpOrder")
                dt.Rows(i)("GrpOrder") = grpord
            End If
        Next
        Return exm
    End Function
    Public Function DownReportGroup(ByRef dt As DataTable, ByVal grp As String, ByVal fld As String) As String
        Dim exm As String = String.Empty
        Dim i As Integer
        Dim grpord As Integer = 0
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)("GroupField") = grp AndAlso dt.Rows(i)("CalcField") = fld Then
                grpord = dt.Rows(i + 1)("GrpOrder")
                dt.Rows(i + 1)("GrpOrder") = dt.Rows(i)("GrpOrder")
                dt.Rows(i)("GrpOrder") = grpord
            End If
        Next
        Return exm
    End Function
    'Protected Function CorrectFieldOrder(ByVal dtb As DataTable, ByVal orderfld As String) As DataTable
    '    For i = 0 To dtb.Rows.Count - 1
    '        dtb.Rows(i)(orderfld) = i + 1
    '    Next
    '    Return dtb
    'End Function
    'Public Function UpReportField(ByVal dtrf As DataTable, ByVal rfld As String) As DataTable
    '    Dim i As Integer
    '    Dim fldord As Integer = 0
    '    For i = 0 To dtrf.Rows.Count - 1
    '        If dtrf.Rows(i)("VAL") = rfld Then
    '            fldord = dtrf.Rows(i - 1)("Order")
    '            dtrf.Rows(i - 1)("Order") = dtrf.Rows(i)("Order")
    '            dtrf.Rows(i)("Order") = fldord
    '        End If
    '    Next
    '    Return dtrf
    'End Function
    'Public Function DownReportField(ByVal dtrf As DataTable, ByVal rfld As String) As DataTable
    '    Dim i As Integer
    '    Dim fldord As Integer = 0
    '    For i = 0 To dtrf.Rows.Count - 1
    '        If dtrf.Rows(i)("VAL") = rfld Then
    '            fldord = dtrf.Rows(i + 1)("Order")
    '            dtrf.Rows(i + 1)("Order") = dtrf.Rows(i)("Order")
    '            dtrf.Rows(i)("Order") = fldord
    '        End If
    '    Next
    '    Return dtrf
    'End Function
    Protected Sub Menu1_MenuItemClick(ByVal sender As Object, ByVal e As MenuEventArgs) Handles Menu1.MenuItemClick
        MultiView1.ActiveViewIndex = Int32.Parse(e.Item.Value)
        Session("TabNF") = MultiView1.ActiveViewIndex
        Dim i As Integer
        'Make the selected menu item reflect the correct imageurl
        For i = 0 To Menu1.Items.Count - 1
            If i = e.Item.Value Then
                Menu1.Items(i).Selected = True
                Session("TabNF") = MultiView1.ActiveViewIndex
                lblView.Text = Menu1.Items(Session("TabNF")).Text
                Select Case Session("TabNF")
                    Case "0"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Columns_Expressions"
                    Case "1"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Groups_and_Totals"
                    Case "2"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Combine_Column_Values"
                    Case "3"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Advanced_Report_Designer"
                        Response.Redirect("~/ReportDesigner.aspx")
                    Case "4"
                        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Map_Definition"
                        Response.Redirect("~/MapReport.aspx")
                End Select
            Else
                Menu1.Items(i).Selected = False
            End If
        Next
        Session("ret") = ""
        LabelAlert1.Text = ""
        Response.Redirect("RDLformat.aspx?tnf=" & Session("TabNF").ToString)
    End Sub
    Protected Sub ButtonAddList_Click(sender As Object, e As EventArgs) Handles ButtonAddList.Click
        Dim ret As String = String.Empty
        Dim insSQL As String = String.Empty

        Dim frname As String = TextBoxFriendlyNameField.Text.Trim

        If chkOldColumn.Checked Then
            'replace column with combined value
            insSQL = "INSERT INTO OURReportLists (ReportID,ListFld,RecFld,COMMENTS,Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','" & DropDownListFields.Text & "','" & DropDownListRecFields.Text & "','" & TextCommentsList.Text & "','" & DropDownListFields.Text & "','" & frname & "',0)"
            ret = ExequteSQLquery(insSQL)
            'Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
        Else
            Dim newcolname As String = DropDownListFields.Text & "_Combined"

            If TextBoxFriendlyNameField.Text.Trim = "" Then
                frname = newcolname
                TextBoxFriendlyNameField.Text = frname
            Else
                frname = TextBoxFriendlyNameField.Text
            End If

            ' if this is an update to an existing definition, delete it first
            If ReportFieldExists(Session("REPORTID"), newcolname) Then
                DeleteReportList(Session("REPORTID"), DropDownListFields.Text, newcolname)
            End If

            'new field in ourreportformat, computed column
            insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','FIELDS','" & newcolname & "',0,'" & frname & "','',1)"
            ret = ExequteSQLquery(insSQL)

            'new record in ourreportlist
            insSQL = "INSERT INTO OURReportLists (ReportID,ListFld,RecFld,COMMENTS,Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','" & DropDownListFields.Text & "','" & DropDownListRecFields.Text & "','" & TextCommentsList.Text & "','" & newcolname & "','" & frname & "',1)"
            ret = ExequteSQLquery(insSQL)

            'insert into OurReportItems
            'TODO add report item for computed field, use String type for field type !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            ret = AddReportItem(Session("REPORTID"), newcolname, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        End If

        Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Sub ButtonAddField_Click(sender As Object, e As EventArgs) Handles ButtonAddField.Click
        Dim ret As String = String.Empty
        Dim rep As String = String.Empty
        Dim frname As String = String.Empty
        'TODO insert into report fields OURReportSQLquery

        Try
            Dim sctSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='FIELDS' AND VAL='" & DropDownRepFields.Text & "' )"
            If Not HasRecords(sctSQL) Then           'column is not in report
                If TextBoxFieldFriendly.Text.Trim = "" Then
                    frname = GetFriendlyFieldName(repid, DropDownRepFields.Text)
                    TextBoxFieldFriendly.Text = frname
                Else
                    frname = TextBoxFieldFriendly.Text
                End If
                'insert
                Dim insSQL As String = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2) VALUES('" & Session("REPORTID") & "','FIELDS','" & DropDownRepFields.Text & "',0,'" & frname & "','" & TextBoxExpression.Text & "')"
                ret = ExequteSQLquery(insSQL)

                'insert into OurReportItems
                ret = AddReportItem(Session("REPORTID"), DropDownRepFields.Text, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
            Else                                    'column is in report
                If chkReplaceColValue.Checked Then  'use old column
                    If TextBoxFieldFriendly.Text.Trim = "" Then
                        frname = GetFriendlyFieldName(repid, DropDownRepFields.Text)
                        TextBoxFieldFriendly.Text = frname
                    Else
                        frname = TextBoxFieldFriendly.Text
                    End If
                    'update
                    Dim updSQL As String = "UPDATE OURReportFormat SET Prop1='" & frname & "',Prop2='" & TextBoxExpression.Text & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='FIELDS' AND VAL='" & DropDownRepFields.Text & "' )"
                    ret = ExequteSQLquery(updSQL)
                Else                                'add new computed column !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
                    'insert new field with name = column name & function name
                    Dim newcolname As String = ""
                    If DropDownFunctions.Text.Trim <> "" Then
                        newcolname = DropDownRepFields.Text & "_" & DropDownFunctions.Text
                    ElseIf DropDownFunctions.Text.Trim = "" AndAlso TextBoxFieldFriendly.Text.Trim <> "" Then
                        frname = TextBoxFieldFriendly.Text
                        newcolname = frname.Replace(" ", "_")
                    Else
                        newcolname = "Column_1"
                    End If
                    If TextBoxFieldFriendly.Text.Trim = "" Then
                        frname = newcolname
                        TextBoxFieldFriendly.Text = frname
                    Else
                        frname = TextBoxFieldFriendly.Text
                    End If
                    Dim insSQL As String = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','FIELDS','" & newcolname & "',0,'" & frname & "','" & TextBoxExpression.Text & "',1)"
                    ret = ExequteSQLquery(insSQL)

                    'insert into OurReportItems
                    'TODO add report item for computed field, use function type for field type and original field(s)  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    ret = AddReportItem(Session("REPORTID"), newcolname, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                End If
            End If

        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID") & "&ret=" & ret)
    End Sub

    Protected Sub DropDownRepFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownRepFields.SelectedIndexChanged
        repid = Session("REPORTID")
        TextBoxFieldFriendly.Text = ""
        'find friendly name
        'Dim frname As String = GetFriendlyReportFieldName(repid, DropDownRepFields.Text)
        'If frname = DropDownRepFields.Text Then   'no friendly name yet
        '    frname = GetSuggestedFriendlyFieldName(DropDownRepFields.Text, repid)
        '    If frname <> "" Then TextBoxFieldFriendly.BackColor = Color.Coral
        'Else
        '    TextBoxFieldFriendly.BackColor = Color.White
        'End If
        'If frname <> "" AndAlso frname <> DropDownRepFields.Text Then
        '    TextBoxFieldFriendly.Text = frname
        'End If
        'If TextBoxFieldFriendly.Text = "" Then
        'find friendly name
        Dim frname As String = GetFriendlyReportFieldName(repid, DropDownRepFields.Text)
        If frname = DropDownRepFields.Text Then   'no friendly name yet
            frname = GetFriendlySQLFieldName(repid, DropDownRepFields.Text)
            If frname <> "" AndAlso frname <> DropDownRepFields.Text Then
                TextBoxFieldFriendly.BackColor = Color.Coral
            End If
        Else
            TextBoxFieldFriendly.BackColor = Color.White
        End If
        If frname <> "" AndAlso frname <> DropDownRepFields.Text Then
            TextBoxFieldFriendly.Text = frname
        End If
        'End If
        Dim expression = GetFieldExpression(repid, DropDownRepFields.Text)
        TextBoxExpression.Text = expression
    End Sub
    Protected Sub ButtonSubmit3_Click(sender As Object, e As EventArgs) Handles ButtonSubmit3.Click, ButtonSubmit2.Click
        'submit columns and lists fixing order of columns
        Dim i As Integer
        repid = Session("REPORTID")
        Dim sqls As String = String.Empty
        Dim dtb As DataTable = GetReportFields(repid) 'sorted by Order
        dtb = CorrectFieldOrder(repid, dtb, "Order", "OURReportFormat", "Val", "Prop", "FIELDS")
        For i = 0 To dtb.Rows.Count - 1
            sqls = "UPDATE OURReportFormat SET [Order]=" & dtb.Rows(i)("Order") & " WHERE ReportId = '" & repid & "' AND [Val]= '" & dtb.Rows(i)("Val") & "' AND Prop = 'FIELDS'"
            ExequteSQLquery(sqls)
        Next
        Dim ret As String = UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
        CopyFormattedFieldOrder(repid)
        ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
        If ret.StartsWith("ERROR!!") Then
            WriteToAccessLog(Session("logon"), ret, 3)
            ret = "Report Format has been updated with errors. "
        Else
            WriteToAccessLog(Session("logon"), ret, 3)
            ret = "Report Format has been updated. "
        End If
        LabelAlert1.Text = ret
        Session("ret") = ret
        Response.Redirect("RDLformat.aspx?Report=" & Session("REPORTID"))
    End Sub
    Protected Sub DropDownFunctionsType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFunctionsType.SelectedIndexChanged
        TextBoxExpression.Text = ""
        If DropDownFunctionsType.Text = "Text" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("Format")
            DropDownFunctions.Items.Add("FormatCurrency")
            DropDownFunctions.Items.Add("FormatDateTime")
            DropDownFunctions.Items.Add("FormatNumber")
            DropDownFunctions.Items.Add("FormatPercent")
            DropDownFunctions.Items.Add("LCase")
            DropDownFunctions.Items.Add("Left")
            DropDownFunctions.Items.Add("Len")
            DropDownFunctions.Items.Add("LTrim")
            DropDownFunctions.Items.Add("Mid")
            DropDownFunctions.Items.Add("Replace")
            DropDownFunctions.Items.Add("Right")
            DropDownFunctions.Items.Add("RTrim")
            DropDownFunctions.Items.Add("Trim")
            DropDownFunctions.Items.Add("UCase")
        ElseIf DropDownFunctionsType.Text = "Math" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("Abs")
            DropDownFunctions.Items.Add("Acos")
            DropDownFunctions.Items.Add("Asin")
            DropDownFunctions.Items.Add("Atan")
            DropDownFunctions.Items.Add("Cos")
            DropDownFunctions.Items.Add("Exp")
            DropDownFunctions.Items.Add("Int")
            DropDownFunctions.Items.Add("Log")
            DropDownFunctions.Items.Add("Log10")
            DropDownFunctions.Items.Add("Round")
            DropDownFunctions.Items.Add("Sign")
            DropDownFunctions.Items.Add("Sin")
            DropDownFunctions.Items.Add("Tan")
        ElseIf DropDownFunctionsType.Text = "DateTime" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("DateAdd")
            DropDownFunctions.Items.Add("Day")
            DropDownFunctions.Items.Add("FormatDateTime")
            DropDownFunctions.Items.Add("Hour")
            DropDownFunctions.Items.Add("Minute")
            DropDownFunctions.Items.Add("MonthName")
            'DropDownFunctions.Items.Add("Now")
            DropDownFunctions.Items.Add("Second")
            DropDownFunctions.Items.Add("TimeValue")
            'DropDownFunctions.Items.Add("Today")
            DropDownFunctions.Items.Add("Weekday")
            DropDownFunctions.Items.Add("WeekdayName")
            DropDownFunctions.Items.Add("Year")
        ElseIf DropDownFunctionsType.Text = "Aggregate" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("Avg")
            DropDownFunctions.Items.Add("Count")
            DropDownFunctions.Items.Add("CountDistinct")
            DropDownFunctions.Items.Add("CountRows")
            DropDownFunctions.Items.Add("First")
            DropDownFunctions.Items.Add("Last")
            DropDownFunctions.Items.Add("Max")
            DropDownFunctions.Items.Add("Min")
            DropDownFunctions.Items.Add("Sum")
            DropDownFunctions.Items.Add("StDev")
        ElseIf DropDownFunctionsType.Text = "Financial" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("DDB")
            DropDownFunctions.Items.Add("FV")
            DropDownFunctions.Items.Add("IPmt")
            DropDownFunctions.Items.Add("NPer")
            DropDownFunctions.Items.Add("Pmt")
            DropDownFunctions.Items.Add("PPmt")
            DropDownFunctions.Items.Add("PV")
            DropDownFunctions.Items.Add("Rate")
            DropDownFunctions.Items.Add("SLN")
            DropDownFunctions.Items.Add("SYD")
        ElseIf DropDownFunctionsType.Text = "Conversion" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("Fix")
            DropDownFunctions.Items.Add("Hex")
            DropDownFunctions.Items.Add("Int")
            DropDownFunctions.Items.Add("Oct")
            DropDownFunctions.Items.Add("Str")
            DropDownFunctions.Items.Add("Val")
        ElseIf DropDownFunctionsType.Text = "Statistics" Then
            DropDownFunctions.Items.Clear()
            DropDownFunctions.Items.Add(" ")
            DropDownFunctions.Items.Add("Avg")
            DropDownFunctions.Items.Add("Max")
            DropDownFunctions.Items.Add("Min")
            DropDownFunctions.Items.Add("Sum")
            DropDownFunctions.Items.Add("StDev")
            DropDownFunctions.Items.Add("Count")
            DropDownFunctions.Items.Add("CountDistinct")
            DropDownFunctions.Items.Add("CountRows")

        End If
    End Sub
    Protected Sub DropDownFunctions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownFunctions.SelectedIndexChanged
        TextBoxExpression.Text = ""
        TextBoxExpression.Text = DropDownFunctions.Text & "(Fields!" & DropDownRepFields.Text & ".Value)"
        TextBoxExpression.ToolTip = ExpressionToolTip(DropDownFunctions.Text)
    End Sub
    Protected Sub TextCommentsGroup_TextChanged(sender As Object, e As EventArgs) Handles TextCommentsGroup.TextChanged
        TextCommentsGroup.Text = cleanText(TextCommentsGroup.Text)
    End Sub
    Protected Sub TextCommentsList_TextChanged(sender As Object, e As EventArgs) Handles TextCommentsList.TextChanged
        TextCommentsList.Text = cleanText(TextCommentsList.Text)
    End Sub
    Protected Sub TextBoxFieldFriendly_TextChanged(sender As Object, e As EventArgs) Handles TextBoxFieldFriendly.TextChanged
        TextBoxFieldFriendly.Text = cleanText(TextBoxFieldFriendly.Text)
    End Sub
    Protected Sub TextBoxExpression_TextChanged(sender As Object, e As EventArgs) Handles TextBoxExpression.TextChanged
        TextBoxExpression.Text = cleanText(TextBoxExpression.Text)
    End Sub
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
        ElseIf i = 3 Then
            Session("TabN") = 1
            Session("TabNF") = 3
            Response.Redirect("~/ReportDesigner.aspx")
        ElseIf i = 4 Then
            Session("TabN") = 1
            Session("TabNF") = 4
            Response.Redirect("~/MapReport.aspx")
        Else
            Session("TabN") = i
            Session("TabNF") = 0
            Session("TabNQ") = 0
            Response.Redirect("~/ReportEdit.aspx")
        End If
        Session("ret") = ""
        LabelAlert1.Text = ""
    End Sub
    Private Function RepFieldIsNumeric(ByVal dt As DataTable, ByVal colname As String) As Boolean
        Dim num As Boolean = Nothing
        If dt Is Nothing Then
            Return num
            Exit Function
        End If
        Dim i As Integer
        For i = 0 To dt.Columns.Count - 1
            If (dt.Columns(i).Caption = colname) Then
                If dt.Columns(i).DataType.Name = "String" OrElse dt.Columns(i).DataType.Name = "DateTime" Then
                    num = False
                Else
                    num = True
                End If
                Return num
                Exit Function
            End If
        Next
        Return num
    End Function
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

    Private Sub chkOldColumn_CheckedChanged(sender As Object, e As EventArgs) Handles chkOldColumn.CheckedChanged
        If chkOldColumn.Checked Then
            TextBoxFriendlyNameField.Enabled = True
        Else
            TextBoxFriendlyNameField.Enabled = False
        End If
    End Sub
    'Protected Sub ButtonHelp_Click(sender As Object, e As EventArgs) Handles ButtonHelp.Click
    '    'OpenWordWithBookmak("DevSiteSupportHelp.doc", "Developer Site Support Help", "ClassData")
    '    Response.Redirect("OnlineUserReporting.pdf")
    'End Sub
End Class



