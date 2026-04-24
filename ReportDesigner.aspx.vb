Imports MyWebControls
Imports MyWebControls.DragList
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports System.Drawing.Text
Imports System.Data

Imports System.IO

Partial Class ReportDesigner
    Inherits System.Web.UI.Page

    Private Sub ReportDesigner_Init(sender As Object, e As EventArgs) Handles Me.Init

        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            lblPageTitle.Text = Session("PAGETTL")
        End If

        'lstSections.Attributes.Add("onchange", "OnListChanged('lstSections');")
        divSectionList.Attributes.Add("onclick", "OnListClicked();")
        divGroupList.Attributes.Add("onclick", "OnGroupListClicked();")
        divDrop.Attributes.Add("ondragover", "allowDrop(event);")
        divDrop.Attributes.Add("ondrop", "onDrop(event);")
        divHeader.Attributes.Add("ondrop", "dropOnHeaderFooter(event);")
        divHeader.Attributes.Add("ondragover", "allowDrop(event);")
        divFooter.Attributes.Add("ondrop", "dropOnHeaderFooter(event);")
        divFooter.Attributes.Add("ondragover", "allowDrop(event);")
        btnClose.Attributes.Add("onfocus", "loadReportView();")
        btnShow.Attributes.Add("onfocus", "showReport(event);")
        btnReturn.Attributes.Add("onfocus", "returnToDesigner(event);")
        btnExit.Attributes.Add("onfocus", "closeDesigner(event);")
        btnSelectImageFile.Attributes.Add("onfocus", "addControlOutline(event);")
        btnUploadImageFile.Attributes.Add("onfocus", "addControlOutline(event);")
        btnSelectImageFile.Attributes.Add("onblur", "removeControlOutline(event);")
        btnUploadImageFile.Attributes.Add("onblur", "removeControlOutline(event);")
        FileRDL.Attributes.Add("onchange", "loadSelectedFile(event);")

        'divX.Attributes.Add("onclick", "closePopup('divMsgBoxBackground');")
        divX.Attributes.Add("onclick", "closeMessageBoxX(event);")
        divFontDlgX.Attributes.Add("onclick", "closePopup('divFontDlgBackground');")
        divHeaderFooterDlgX.Attributes.Add("onclick", "closePopup('divHeaderFooterDlgBackground');")
        divTabularWidthX.Attributes.Add("onclick", "closePopup('TabularWidthBackground');")
        divImageFieldDlgHeadingX.Attributes.Add("onclick", "closePopup('divImageFieldSettingsBackground');")
        btnImageFieldDlgBoxCancel.OnClientClick = "closePopup('divImageFieldSettingsBackground'); return false;"
        btnImageFieldDlgBoxOK.OnClientClick = "saveImageFieldData('divImageFieldSettingsBackground'); return false;"
        btnOK.OnClientClick = "closeMessageBox(event); return false;"
        btnDlgBoxCancel.OnClientClick = "closePopup('divFontDlgBackground'); return false;"
        btnDlgBoxOK.OnClientClick = " saveFontData('divFontDlgBackground'); return false;"
        btnHeaderFooterCancel.OnClientClick = "cancelHeaderFooterDlg('divHeaderFooterDlgBackground'); return false;"
        btnHeaderFooterOK.OnClientClick = "saveHeaderFooterData('divHeaderFooterDlgBackground'); return false;"
        btnSubmit.OnClientClick() = "saveAndShow(); return false;"
        btnCancelColWidths.OnClientClick = "closePopup('TabularWidthBackground'); return false;"
        btnSaveColWidths.OnClientClick = "applyColSizerChanges(); return false;"
        'btnHeaderSettings.OnClientClick = "showHeaderFooterDlg(event); return false;"
        'btnFooterSettings.OnClientClick = "showHeaderFooterDlg(event); return false; "
        btnHelp.OnClientClick = "showHelp(); return false;"
        btnDesignerMenu.OnClientClick = "showOptions(event); return false;"
        'btnUploadImageFile.OnClientClick = " uploadSelectedFile(event); return false;"
        btnSelectImageFile.OnClientClick = "clickFileUpload();return false;"


    End Sub
    Private Sub SetLists(ReportID As String)
        Dim dt As DataTable = GetReportTablesFromSQLqueryText(ReportID, Session("UserConnString"), Session("UserConnProvider"))
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim ReportTableItems = New DragListItems
            Dim ReportFieldItems = New DragListItems
            Dim ReportGroupItems = New DragListItems
            Dim ReportSpecialFieldItems = New DragListItems
            Dim ReportSectionItems = New DragListItems
            Dim di As DragItem
            Dim dr As DataRow
            Dim tbl As String = String.Empty
            Dim fld As String = String.Empty
            Dim fldtype As String = String.Empty

            '****************************** Special Field Items ***********************
            'di = New DragItem()
            'di.Text = "Confidentiality Statement"
            'di.ID = "Confidentiality"
            'di.Tag = "0"
            'ReDim Preserve ReportSpecialFieldItems.Items(0)
            'ReportSpecialFieldItems.Items(0) = di

            di = New DragItem()
            di.Text = "Page Number"
            di.ID = "PageNumber"
            di.Tag = "0"
            ReDim Preserve ReportSpecialFieldItems.Items(0)
            ReportSpecialFieldItems.Items(0) = di

            di = New DragItem()
            di.Text = "Page N of M"
            di.ID = "PageNofM"
            di.Tag = "1"
            ReDim Preserve ReportSpecialFieldItems.Items(1)
            ReportSpecialFieldItems.Items(1) = di

            di = New DragItem()
            di.Text = "Label"
            di.ID = "Label"
            di.Tag = "2"
            ReDim Preserve ReportSpecialFieldItems.Items(2)
            ReportSpecialFieldItems.Items(2) = di

            di = New DragItem()
            di.Text = "Report Title"
            di.ID = "ReportTitle"
            di.Tag = "3"
            ReDim Preserve ReportSpecialFieldItems.Items(3)
            ReportSpecialFieldItems.Items(3) = di

            'di = New DragItem()
            'di.Text = "Report User"
            'di.ID = "ReportUser"
            'di.Tag = "5"
            'ReDim Preserve ReportSpecialFieldItems.Items(5)
            'ReportSpecialFieldItems.Items(5) = di

            di = New DragItem()
            di.Text = "Report SQL Query"
            di.ID = "SqlQuery"
            di.Tag = "4"
            ReDim Preserve ReportSpecialFieldItems.Items(4)
            ReportSpecialFieldItems.Items(4) = di

            di = New DragItem()
            di.Text = "Report Comments"
            di.ID = "ReportComments"
            di.Tag = "5"
            ReDim Preserve ReportSpecialFieldItems.Items(5)
            ReportSpecialFieldItems.Items(5) = di

            di = New DragItem()
            di.Text = "Print Date"
            di.ID = "PrintDate"
            di.Tag = "6"
            ReDim Preserve ReportSpecialFieldItems.Items(6)
            ReportSpecialFieldItems.Items(6) = di

            di = New DragItem()
            di.Text = "Print Time"
            di.ID = "PrintTime"
            di.Tag = "7"
            ReDim Preserve ReportSpecialFieldItems.Items(7)
            ReportSpecialFieldItems.Items(7) = di

            di = New DragItem()
            di.Text = "Print DateTime"
            di.ID = "PrintDateTime"
            di.Tag = "8"
            ReDim Preserve ReportSpecialFieldItems.Items(8)
            ReportSpecialFieldItems.Items(8) = di
            di = New DragItem()
            di.Text = "Image"
            di.ID = "Image"
            di.Tag = "9"
            ReDim Preserve ReportSpecialFieldItems.Items(9)
            ReportSpecialFieldItems.Items(9) = di

            lstSpecialFields.DragItems = ReportSpecialFieldItems

            '****************************** Section items *****************************
            di = New DragItem()
            di.Text = "Details"
            di.ID = "Section_Details"
            di.Tag = "Details"
            ReDim Preserve ReportSectionItems.Items(0)
            ReportSectionItems.Items(0) = di

            di = New DragItem()
            di.Text = "Report Header"
            di.ID = "Section_ReportHeader"
            di.Tag = "ReportHeader"
            ReDim Preserve ReportSectionItems.Items(1)
            ReportSectionItems.Items(1) = di

            di = New DragItem()
            di.Text = "Report Footer"
            di.ID = "Section_ReportFooter"
            di.Tag = "ReportFooter"
            ReDim Preserve ReportSectionItems.Items(2)
            ReportSectionItems.Items(2) = di

            If DataModule.HasReportGroups(ReportID) Then
                di = New DragItem()
                di.Text = "Report Groups"
                di.ID = "Section_ReportGroups"
                di.Tag = "ReportGroups"
                ReDim Preserve ReportSectionItems.Items(3)
                ReportSectionItems.Items(3) = di
            End If

            lstSections.DragItems = ReportSectionItems
            lstSections.SelectedIndex = 0
            hdnSection.Value = "Details"

            '**************************** Table Items ****************************

            ReDim ReportTableItems.Items(-1)
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                tbl = dr("Tbl1").ToString
                di = New DragItem
                di.Text = tbl
                di.ID = "Table_" & i.ToString
                di.Tag = i.ToString
                ReDim Preserve ReportTableItems.Items(i)
                ReportTableItems.Items(i) = di
            Next

            '************************** Field Items ******************************
            Dim idx As Integer = 0
            Dim ReportTemplate As String = GetReportTemplate(ReportID)

            ReDim ReportFieldItems.Items(-1)

            If ReportTemplate.ToUpper = "FREEFORM" Then
                di = New DragItem()
                di.Text = "** Label **"
                di.ID = "Label"
                di.Tag = "0"

                ReDim Preserve ReportFieldItems.Items(idx)
                ReportFieldItems.Items(idx) = di
                idx += 1
            End If

            For i As Integer = 0 To ReportTableItems.Items.Length - 1
                di = ReportTableItems.Items(i)
                tbl = di.Text
                dt = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For ii As Integer = 0 To dt.Rows.Count - 1
                        dr = dt.Rows(ii)
                        fld = dr("COLUMN_NAME").ToString
                        fldtype = GetFieldDataType(tbl, fld, Session("UserConnString"), Session("UserConnProvider"))
                        fld = tbl & "." & fld
                        di = New DragItem
                        di.Text = fld
                        di.ID = fld
                        di.Tag = fldtype
                        ReDim Preserve ReportFieldItems.Items(idx)
                        ReportFieldItems.Items(idx) = di
                        idx += 1
                    Next
                End If
            Next
            lstFields.DragItems = ReportFieldItems
            Session("ReportFieldItems") = ReportFieldItems
            '**************************Group Items ******************************
            ReDim ReportGroupItems.Items(-1)

            If HasReportGroups(ReportID) Then
                idx = -1
                dt = GetReportGroups(ReportID)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    For i As Integer = 0 To dt.Rows.Count - 1
                        dr = dt.Rows(i)
                        fld = dr("GroupField").ToString
                        If fld <> "Overall" Then
                            If i = 0 OrElse dr("GroupField") <> dt.Rows(i - 1)("GroupField") Then
                                di = New DragItem
                                di.Text = fld
                                di.ID = "Group_" & fld
                                di.Tag = dr("GrpOrder")
                                idx += 1
                                ReDim Preserve ReportGroupItems.Items(idx)
                                ReportGroupItems.Items(idx) = di
                            End If
                            'di = New DragItem
                            'di.Text = fld
                            '    di.ID = fld
                            '    di.Tag = fld
                            '    idx += 1
                            '    ReDim Preserve ReportGroupItems.Items(idx)
                            '    ReportGroupItems.Items(idx) = di
                        End If
                    Next
                End If
            End If
            Session("ReportGroupItems") = ReportGroupItems
            lstGroups.DragItems = ReportGroupItems
        End If
    End Sub
    Private Sub GetFormattedFields()
        Dim ReportFieldItems = CType(Session("ReportFieldItems"), DragListItems)
        Dim htFields As New Hashtable
        Dim htTblCol As New Hashtable
        Dim htDuplicate As New Hashtable
        Dim htFieldType As New Hashtable
        Dim htListId As New Hashtable

        Dim tbl As String = String.Empty
        Dim fld As String = String.Empty
        Dim liID As String = String.Empty
        Dim tblfld As String = String.Empty

        Dim dt As DataTable = GetSQLFields(hdnReportID.Value)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim dr As DataRow
            For i As Integer = 0 To dt.Rows.Count - 1
                dr = dt.Rows(i)
                tbl = dr("Tbl1").ToString.ToLower
                fld = dr("Tbl1Fld1")
                tblfld = tbl & "." & fld
                If htFields(fld) IsNot Nothing Then
                    If htDuplicate(fld) IsNot Nothing Then
                        Dim j As Integer = CInt(htDuplicate(fld))
                        Dim f As String = fld
                        fld = fld & j.ToString
                        j += 1
                        htDuplicate(f) = j.ToString
                    Else
                        htDuplicate.Add(fld, "2")
                        fld = fld & "1"
                    End If
                End If
                htFields.Add(fld, tblfld)
                htTblCol.Add(tblfld, fld)
            Next
        End If
        For i As Integer = 0 To ReportFieldItems.Items.Count - 1
            liID = "lstFields_li" & (i + 1).ToString
            Dim di As DragItem = lstFields.DragItems.Items(i)
            tbl = Piece(di.Text, ".", 1).ToLower
            fld = Piece(di.Text, ".", 2)
            If htTblCol(di.Text) Is Nothing Then
                If htDuplicate(fld) IsNot Nothing Then
                    Dim j As Integer = CInt(htDuplicate(fld))
                    Dim f As String = fld
                    fld = fld & j.ToString
                    j += 1
                    htDuplicate(f) = j.ToString
                Else
                    htDuplicate.Add(fld, "2")
                    fld = fld & "1"
                End If
                htFields.Add(fld, di.Text)
                htTblCol.Add(di.Text, fld)
            Else
                fld = htTblCol(di.Text)
            End If
            htListId.Add(fld, liID)
            htFieldType.Add(fld, di.Tag)

        Next
        Session("htFields") = htFields
        Session("htTblCol") = htTblCol
        Session("htListId") = htListId
        Session("htFieldType") = htFieldType

    End Sub
    Private Sub AddReportItems(RView As ReportView)
        If RView IsNot Nothing AndAlso RView.ReportID <> "" Then
            Dim ri As ReportItem = Nothing
            Dim j As Integer = 0
            Dim dr As DataRow
            Dim underline As Integer = 0
            Dim strikeout As Integer = 0

            Dim bHasGroups As Boolean = HasReportGroups(RView.ReportID)
            Dim bHasGroupReportItems As Boolean = False
            Dim dtGroups As DataTable

            GetFormattedFields()
            If bHasGroups Then
                dtGroups = GetDistinctReportGroups(RView.ReportID)
                For i As Integer = 0 To dtGroups.Rows.Count - 1
                    dr = dtGroups.Rows(i)
                    If dr("GroupField") <> "Overall" Then
                        CreateGroupReportItem(RView.ReportID, dr("GroupField"), dr("GrpOrder"))
                    End If
                Next
            End If
            Dim dt As DataTable = GetReportItems(RView.ReportID)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                bHasGroupReportItems = HasGroupReportItems(RView.ReportID)

                For i As Integer = 0 To dt.Rows.Count - 1
                    dr = dt.Rows(i)
                    ri = New ReportItem
                    ri.ItemID = dr("ItemID")
                    ri.TabularColumnWidth = IIf(IsDBNull(dr("TabularColumnWidth")), "", dr("TabularColumnWidth"))
                    ri.Caption = dr("Caption")
                    ri.CaptionHeight = IIf(IsDBNull(dr("CaptionHeight")), "", dr("CaptionHeight"))
                    ri.CaptionWidth = IIf(IsDBNull(dr("CaptionWidth")), "", dr("CaptionWidth"))
                    ri.CaptionX = IIf(IsDBNull(dr("CaptionX")), "", dr("CaptionX").ToString)
                    ri.CaptionY = IIf(IsDBNull(dr("CaptionY")), "", dr("CaptionY").ToString)
                    ri.CaptionFontName = IIf(IsDBNull(dr("CaptionFontName")), "", dr("CaptionFontName").ToString)
                    ri.CaptionFontSize = IIf(IsDBNull(dr("CaptionFontSize")), "", dr("CaptionFontSize").ToString) 'dr("CaptionFontSize")
                    ri.CaptionForeColor = IIf(IsDBNull(dr("CaptionForeColor")), "", dr("CaptionForeColor").ToString)  'dr("CaptionForecolor")
                    ri.CaptionBackColor = IIf(IsDBNull(dr("CaptionBackColor")), "", dr("CaptionBackColor").ToString)  'dr("CaptionBackcolor")
                    ri.CaptionBorderColor = IIf(IsDBNull(dr("CaptionBorderColor")), "", dr("CaptionBorderColor").ToString) 'dr("CaptionBorderColor")
                    ri.CaptionBorderStyle = dr("CaptionBorderStyle")
                    ri.CaptionBorderWidth = dr("CaptionBorderWidth")
                    ri.CaptionFontStyle = dr("CaptionFontStyle")
                    ri.CaptionUnderline = False
                    ri.CaptionStrikeout = False
                    ri.CaptionTextAlign = dr("CaptionTextAlign")
                    underline = dr("CaptionUnderline")
                    If underline <> 0 Then ri.CaptionUnderline = True
                    strikeout = dr("CaptionStrikeout")
                    If strikeout <> 0 Then ri.CaptionStrikeout = True
                    ri.ReportItemType = dr("ReportItemType")
                    ri.ImagePath = IIf(IsDBNull(dr("ImagePath")), "", dr("ImagePath"))
                    ri.ImageHeight = IIf(IsDBNull(dr("ImageHeight")), "", dr("ImageHeight"))
                    ri.ImageWidth = IIf(IsDBNull(dr("ImageWidth")), "", dr("ImageWidth"))
                    ri.FieldLayout = dr("FieldLayout")
                    ri.SQLDatabase = IIf(IsDBNull(dr("SQLDatabase")), "", dr("SQLDatabase").ToString)     'dr("SQLDatabase")
                    ri.SQLTable = IIf(IsDBNull(dr("SQLTable")), "", dr("SQLTable").ToString)   'dr("SQLTable")
                    ri.SQLField = IIf(IsDBNull(dr("SQLField")), "", dr("SQLField").ToString)   ' dr("SQLField")
                    ri.SQLDataType = IIf(IsDBNull(dr("SQLDatatype")), "", dr("SQLDatatype").ToString)    'dr("SQLDatatype")
                    ri.ItemOrder = dr("ItemOrder")
                    ri.DataHeight = IIf(IsDBNull(dr("DataHeight")), "", dr("DataHeight"))
                    ri.DataWidth = IIf(IsDBNull(dr("DataWidth")), "", dr("DataWidth"))
                    ri.DataX = IIf(IsDBNull(dr("DataX")), "", dr("DataX").ToString)
                    ri.DataY = IIf(IsDBNull(dr("DataY")), "", dr("DataY").ToString)
                    ri.Height = IIf(IsDBNull(dr("Height")), "", dr("Height"))
                    ri.Width = IIf(IsDBNull(dr("Width")), "", dr("Width"))
                    ri.X = IIf(IsDBNull(dr("X")), "", dr("X").ToString)
                    ri.Y = IIf(IsDBNull(dr("Y")), "", dr("Y").ToString)
                    ri.FontName = dr("FontName")
                    ri.FontSize = dr("FontSize")
                    ri.ForeColor = dr("Forecolor")
                    ri.BackColor = dr("Backcolor")
                    ri.BorderColor = dr("BorderColor")
                    ri.BorderStyle = dr("BorderStyle")
                    ri.BorderWidth = dr("BorderWidth")
                    ri.FontStyle = dr("FontStyle")
                    ri.Underline = False
                    ri.Strikeout = False
                    ri.TextAlign = dr("TextAlign")
                    underline = dr("Underline")
                    If underline <> 0 Then ri.Underline = True
                    strikeout = dr("Strikeout")
                    If strikeout <> 0 Then ri.Strikeout = True
                    ri.Section = dr("Section")
                    ReDim Preserve RView.Items(j)
                    RView.Items(j) = ri
                    j += 1
                Next
                Session("ReportView") = RView
            Else
                Dim htFields As Hashtable = CType(Session("htFields"), Hashtable)
                Dim htTblCol As Hashtable = CType(Session("htTblCol"), Hashtable)
                Dim htListId As Hashtable = CType(Session("htListId"), Hashtable)
                Dim htFieldtype As Hashtable = CType(Session("htFieldType"), Hashtable)

                Dim tbl As String = String.Empty
                Dim fld As String = String.Empty
                Dim liID As String = String.Empty

                If htFields.Count > 0 Then
                    dt = GetReportFields(RView.ReportID)
                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        For i As Integer = 0 To dt.Rows.Count - 1
                            dr = dt.Rows(i)
                            Dim field As String = dr("Val")
                            Dim ord As String = String.Empty
                            If htFields(field) IsNot Nothing Then
                                ord = (j + 1).ToString
                                Dim tblfld As String = htFields(field).ToString
                                tbl = Piece(tblfld, ".", 1)
                                fld = Piece(tblfld, ".", 2)
                                ri = New ReportItem
                                If Not IsDBNull(dr("Prop1")) AndAlso dr("Prop1") <> String.Empty Then
                                    ri.Caption = dr("Prop1")
                                Else
                                    ri.Caption = field
                                End If
                                ri.ReportItemType = "DataField"
                                ri.SQLTable = tbl
                                ri.SQLField = fld
                                ri.SQLDatabase = hdnDataBase.Value
                                ri.SQLDataType = htFieldtype(field).ToString
                                ri.ItemID = htListId(field).ToString
                                ri.ItemOrder = ord
                                ri.CaptionFontName = "Tahoma"
                                ri.CaptionFontSize = "12px"
                                ri.CaptionForeColor = "black"
                                ri.CaptionBackColor = "white"
                                ri.CaptionBorderColor = "lightgrey"
                                ri.CaptionBorderStyle = "Solid"
                                ri.CaptionBorderWidth = "1"
                                ri.CaptionFontStyle = "Regular"
                                ri.CaptionUnderline = False
                                ri.CaptionStrikeout = False
                                ri.CaptionTextAlign = "Left"
                                ri.FontName = "Tahoma"
                                ri.FontSize = "12px"
                                ri.ForeColor = "black"
                                ri.BackColor = "white"
                                ri.BorderColor = "lightgrey"
                                ri.BorderStyle = "Solid"
                                ri.BorderWidth = "1"
                                ri.FontStyle = "Regular"
                                ri.Underline = False
                                ri.Strikeout = False
                                ri.TextAlign = "Left"
                                ri.Section = "Details"
                                ri.FieldLayout = "Block"
                                ReDim Preserve RView.Items(j)
                                RView.Items(j) = ri
                                j += 1
                            End If
                        Next
                        Session("ReportView") = RView
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub ReportDesigner_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing AndAlso target <> String.Empty AndAlso data <> String.Empty Then
            If target = "SaveReportView" Then
                Dim ret As String = String.Empty
                Dim action As String = Piece(data, "~", 2)
                Dim RptViewData As String = Piece(data, "~", 1)
                hdnReportView.Value = RptViewData
                btnSubmit_Click(CObj(btnSubmit), EventArgs.Empty)
                ret = Session("ret").ToString
                Session("ret") = Nothing
                If Not ret.StartsWith("ERROR!!") Then
                    Select Case action
                        Case "Show Report"
                            btnShow.Focus()
                        Case "Return To Designer"
                            btnReturn.Focus()
                        Case "Close Designer"
                            btnExit.Focus()
                    End Select

                    'Response.Redirect("ShowReport.aspx?srd=3")
                    'ClearSession()
                    'Response.Redirect("ReportDesigner.aspx")
                    'MessageBox.DefaultButton = Controls_Msgbox.MessageDefaultButton.Other2
                    'MessageBox.OtherButtonText1 = "Show Report"
                    'MessageBox.OtherButtonText2 = "Return to Designer"
                    'MessageBox.Show(ret, "Report Designer", "OK", Controls_Msgbox.Buttons.OtherOtherCancel, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.Other2)
                End If
                Return
            ElseIf target = "MsgBoxAction" Then
                Dim action As String = data
                If action = "Show Report" Then
                    Response.Redirect("ShowReport.aspx?srd=3")
                    Return
                ElseIf action = "Return To Designer" Then
                    ClearSession()
                    Response.Redirect("ReportDesigner.aspx")
                    Return
                ElseIf action = "Close Designer" Then
                    btnClose_Click(btnSubmit, EventArgs.Empty)
                    Return
                End If
            End If
        End If

        Dim obj As New DragListItems
        Dim ifc = New InstalledFontCollection
        Dim ff As System.Drawing.FontFamily
        Dim fd As New FontData()
        Dim fi As FontItem
        Dim j As Integer = 0
        Dim dt As DataTable = Nothing


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

        Dim url As String = "RDLformat.aspx?Report=" & Session("REPORTID")
        Dim msg As String = String.Empty

        If Session("urlback") Is Nothing Or Session("urlback") = String.Empty Then Session("urlback") = url

        hdnReportID.Value = Session("REPORTID")
        hdnReportTitle.Value = Session("REPTITLE")
        hdnDataBase.Value = GetDataBase(Session("UserConnString"))

        If Not IsPostBack Then

            'If Not HasSQLData(Session("REPORTID")) Then
            'msg = "Fields are not defined for '" & Session("REPTITLE") & "'. Fields must be defined and saved in Report Data Definition."
            'MessageBox.Show(msg, "Fields Not Defined", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            'Return
            'End If

            'If Not FileInOURFiles(Session("REPORTID")) Then
            '    msg = "No RDL file has been created for '" & Session("REPTITLE") & "'. Report must be saved or updated."
            '    MessageBox.Show(msg, "RDL Not Defined", "Error", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            '    Return
            'End If

            'load fonts
            ReDim fd.Items(-1)
            For i As Integer = 0 To ifc.Families.Length - 1
                ff = ifc.Families(i)
                fi = New FontItem

                fi.name = ff.Name
                fi.boldAvailable = ff.IsStyleAvailable(Drawing.FontStyle.Bold)
                fi.italicAvailable = ff.IsStyleAvailable(Drawing.FontStyle.Italic)
                fi.regularAvailable = ff.IsStyleAvailable(Drawing.FontStyle.Regular)
                fi.strikeoutAvailable = ff.IsStyleAvailable(Drawing.FontStyle.Strikeout)
                fi.underlineAvailable = ff.IsStyleAvailable(Drawing.FontStyle.Underline)

                ReDim Preserve fd.Items(j)
                fd.Items(j) = fi
                j += 1
            Next
            Session("FontData") = fd
            hdnFontData.Value = JsonConvert.SerializeObject(Session("FontData"))

            'load fields
            SetLists(hdnReportID.Value)

            If lstFields.DragItems Is Nothing OrElse lstFields.DragItems.Items.Count = 0 Then
                msg = "Field list for '" & Session("REPTITLE") & "' is empty. Check Report Data Definition."
                MessageBox.Show(msg, "Field List Is Empty", "Fields Not Defined", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Return
            End If

            Dim ReportTemplate As String = "Tabular"
            Dim Orientation As String = "portrait"
            Dim ReportFieldLayout As String = "Block"
            Dim HeaderHeight As String = "1"
            Dim HeaderBackColor As String = "white"
            Dim HeaderBorderColor As String = "lightgrey"
            Dim HeaderBorderStyle As String = "Solid"
            Dim HeaderBorderWidth As String = "1"
            Dim FooterHeight As String = "1"
            Dim FooterBackColor As String = "white"
            Dim FooterBorderColor As String = "lightgrey"
            Dim FooterBorderStyle As String = "Solid"
            Dim FooterBorderWidth As String = "1"

            Dim DataFontName As String = "Tahoma"
            Dim DataFontSize As String = "12px"
            Dim DataForeColor As String = "black"
            Dim DataBackColor As String = "white"
            Dim DataBorderColor As String = "lightgrey"
            Dim DataBorderStyle As String = "Solid"
            Dim DataBorderWidth As String = "1"
            Dim DataFontStyle As String = "Regular"
            Dim DataUnderline As Boolean = False
            Dim DataStrikeout As Boolean = False
            Dim ReportDetailAlign As String = "Left"
            Dim LabelFontName As String = "Tahoma"
            Dim LabelFontSize As String = "12px"
            Dim LabelForeColor As String = "black"
            Dim LabelBackColor As String = "white"
            Dim LabelBorderColor As String = "lightgrey"
            Dim LabelBorderStyle As String = "Solid"
            Dim LabelBorderWidth As String = "1"
            Dim LabelFontStyle As String = "Regular"
            Dim LabelUnderline As Boolean = False
            Dim LabelStrikeout As Boolean = False
            Dim ReportCaptionAlign As String = "Left"
            Dim MarginBottom As String = "0.5"
            Dim MarginLeft As String = "0.25"
            Dim MarginRight As String = "0.25"
            Dim MarginTop As String = "0.5"

            dt = GetReportInfo(hdnReportID.Value)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                If Not IsDBNull(dt.Rows(0)("Param0type")) AndAlso dt.Rows(0)("Param0type") <> "" Then
                    ReportTemplate = dt.Rows(0)("Param0type").ToString
                End If
                If Not IsDBNull(dt.Rows(0)("Param9type")) AndAlso dt.Rows(0)("Param9type") <> "" Then
                    Orientation = dt.Rows(0)("Param9type").ToString
                End If
            End If
            lblHeader.Text = ReportTemplate & " Report Designer - " & hdnReportTitle.Value
            lblSection.Text = hdnSection.Value

            Dim ReportView As New ReportView()
            ReportView.ReportID = hdnReportID.Value
            ReportView.ReportTemplate = ReportTemplate
            ReportView.Title = hdnReportTitle.Value
            ReportView.Orientation = Orientation

            dt = GetReportView(hdnReportID.Value)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Dim dr As DataRow = dt.Rows(0)
                ReportFieldLayout = IIf(IsDBNull(dr("ReportFieldLayout")), "Block", dr("ReportFieldLayout"))
                HeaderHeight = IIf(IsDBNull(dr("HeaderHeight")), "1", dr("HeaderHeight"))
                HeaderBackColor = IIf(IsDBNull(dr("HeaderBackColor")), "white", dr("HeaderBackColor"))
                HeaderBorderColor = IIf(IsDBNull(dr("HeaderBorderColor")), "lightgrey", dr("HeaderBorderColor"))
                HeaderBorderStyle = IIf(IsDBNull(dr("HeaderBorderStyle")), "Solid", dr("HeaderBorderStyle"))
                HeaderBorderWidth = IIf(IsDBNull(dr("HeaderBorderWidth")), "1", dr("HeaderBorderWidth"))

                FooterHeight = IIf(IsDBNull(dr("FooterHeight")), "1", dr("FooterHeight"))
                FooterBackColor = IIf(IsDBNull(dr("FooterBackColor")), "white", dr("FooterBackColor"))
                FooterBorderColor = IIf(IsDBNull(dr("FooterBorderColor")), "lightgrey", dr("FooterBorderColor"))
                FooterBorderStyle = IIf(IsDBNull(dr("FooterBorderStyle")), "Solid", dr("FooterBorderStyle"))
                FooterBorderWidth = IIf(IsDBNull(dr("FooterBorderWidth")), "1", dr("FooterBorderWidth"))

                DataFontName = IIf(IsDBNull(dr("DataFontName")), "Tahoma", dr("DataFontName"))
                DataFontSize = IIf(IsDBNull(dr("DataFontSize")), "12px", dr("DataFontSize"))
                DataForeColor = IIf(IsDBNull(dr("DataForeColor")), "black", dr("DataForeColor"))
                DataBackColor = IIf(IsDBNull(dr("DataBackColor")), "white", dr("DataBackColor"))
                DataBorderColor = IIf(IsDBNull(dr("DataBorderColor")), "lightgrey", dr("DataBorderColor"))
                DataBorderStyle = IIf(IsDBNull(dr("DataBorderStyle")), "Solid", dr("DataBorderStyle"))
                DataBorderWidth = IIf(IsDBNull(dr("DataBorderWidth")), "1", dr("DataBorderWidth"))
                DataFontStyle = IIf(IsDBNull(dr("DataFontStyle")), "Regular", dr("DataFontStyle"))
                DataUnderline = dr("DataUnderline")
                DataStrikeout = dr("DataStrikeout")
                ReportDetailAlign = IIf(IsDBNull(dr("ReportDetailAlign")), "Left", dr("ReportDetailAlign"))
                LabelFontName = IIf(IsDBNull(dr("LabelFontName")), "Tahoma", dr("LabelFontName"))
                LabelFontSize = IIf(IsDBNull(dr("LabelFontSize")), "12px", dr("LabelFontSize"))
                LabelForeColor = IIf(IsDBNull(dr("LabelForeColor")), "black", dr("LabelForeColor"))
                LabelBackColor = IIf(IsDBNull(dr("LabelBackColor")), "white", dr("LabelBackColor"))
                LabelBorderColor = IIf(IsDBNull(dr("LabelBorderColor")), "lightgrey", dr("LabelBorderColor"))
                LabelBorderStyle = IIf(IsDBNull(dr("LabelBorderStyle")), "Solid", dr("LabelBorderStyle"))
                LabelBorderWidth = IIf(IsDBNull(dr("LabelBorderWidth")), "1", dr("LabelBorderWidth"))
                LabelFontStyle = IIf(IsDBNull(dr("LabelFontStyle")), "Regular", dr("LabelFontStyle"))
                LabelUnderline = dr("LabelUnderline")
                LabelStrikeout = dr("LabelStrikeout")
                ReportCaptionAlign = IIf(IsDBNull(dr("ReportCaptionAlign")), "Left", dr("ReportCaptionAlign"))
                MarginBottom = IIf(IsDBNull(dr("MarginBottom")), "0.5", dr("MarginBottom"))
                MarginLeft = IIf(IsDBNull(dr("MarginLeft")), "0.25", dr("MarginLeft"))
                MarginRight = IIf(IsDBNull(dr("MarginRight")), "0.25", dr("MarginRight"))
                MarginTop = IIf(IsDBNull(dr("MarginTop")), "0.5", dr("MarginTop"))
            End If
            ReportView.ReportFieldLayout = ReportFieldLayout
            ReportView.HeaderHeight = HeaderHeight
            ReportView.HeaderBackColor = HeaderBackColor
            ReportView.HeaderBorderColor = HeaderBorderColor
            ReportView.HeaderBorderStyle = HeaderBorderStyle
            ReportView.HeaderBorderWidth = HeaderBorderWidth

            ReportView.FooterHeight = FooterHeight
            ReportView.FooterBackColor = FooterBackColor
            ReportView.FooterBorderColor = FooterBorderColor
            ReportView.FooterBorderStyle = FooterBorderStyle
            ReportView.FooterBorderWidth = FooterBorderWidth

            ReportView.DataFontName = DataFontName
            ReportView.DataFontSize = DataFontSize
            ReportView.DataForeColor = DataForeColor
            ReportView.DataBackColor = DataBackColor
            ReportView.DataBorderColor = DataBorderColor
            ReportView.DataBorderStyle = DataBorderStyle
            ReportView.DataBorderWidth = DataBorderWidth
            ReportView.DataFontStyle = DataFontStyle
            ReportView.DataUnderline = DataUnderline
            ReportView.DataStrikeout = DataStrikeout
            ReportView.ReportDetailAlign = ReportDetailAlign
            ReportView.LabelFontName = LabelFontName
            ReportView.LabelFontSize = LabelFontSize
            ReportView.LabelForeColor = LabelForeColor
            ReportView.LabelBackColor = LabelBackColor
            ReportView.LabelBorderColor = LabelBorderColor
            ReportView.LabelBorderStyle = LabelBorderStyle
            ReportView.LabelBorderWidth = LabelBorderWidth
            ReportView.LabelFontStyle = LabelFontStyle
            ReportView.LabelUnderline = LabelUnderline
            ReportView.LabelStrikeout = LabelStrikeout
            ReportView.ReportCaptionAlign = ReportCaptionAlign
            ReportView.MarginBottom = MarginBottom
            ReportView.MarginLeft = MarginLeft
            ReportView.MarginRight = MarginRight
            ReportView.MarginTop = MarginTop

            AddReportItems(ReportView)

            If ReportView IsNot Nothing Then
                hdnReportView.Value = JsonConvert.SerializeObject(ReportView)
                btnClose.Focus()
            End If
        End If

    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = "~/" & node.Value
        Session("ReportView") = Nothing
        Session("ReportListItems") = Nothing
        Session("ReportFieldItems") = Nothing
        Session("htFields") = Nothing
        Session("htTblCol") = Nothing
        Session("htListId") = Nothing
        Session("htFieldType") = Nothing
        Session("FontData") = Nothing
        Session("urlback") = Nothing
        Response.Redirect(url)
    End Sub
    Private Sub ClearSession()
        Session("ReportView") = Nothing
        Session("ReportListItems") = Nothing
        Session("ReportFieldItems") = Nothing
        Session("htFields") = Nothing
        Session("htTblCol") = Nothing
        Session("htListId") = Nothing
        Session("htFieldType") = Nothing
        Session("FontData") = Nothing
        Session("urlback") = Nothing

    End Sub
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Dim url As String = Session("urlback")
        ClearSession()
        'Session("ReportView") = Nothing
        'Session("ReportListItems") = Nothing
        'Session("ReportFieldItems") = Nothing
        'Session("htFields") = Nothing
        'Session("htTblCol") = Nothing
        'Session("htListId") = Nothing
        'Session("htFieldType") = Nothing
        'Session("FontData") = Nothing
        'Session("urlback") = Nothing
        If url IsNot Nothing AndAlso url <> String.Empty Then Response.Redirect(url)
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) 'Handles btnSubmit.Click
        Dim val As String = hdnReportView.Value
        If val <> String.Empty Then
            Dim rvSaved As ReportView = JsonConvert.DeserializeObject(Of ReportView)(val)
            Dim rvStart As ReportView = CType(Session("ReportView"), ReportView)
            Dim htFields As Hashtable = CType(Session("htFields"), Hashtable)
            Dim htTblCol As Hashtable = CType(Session("htTblCol"), Hashtable)

            Dim ItemsToDelete As ReportItem()
            'Dim ItemsToAdd As ReportItem()

            ReDim ItemsToDelete(-1)
            'ReDim ItemsToAdd(-1)

            If rvStart Is Nothing Then
                rvStart = New ReportView
                ReDim rvStart.Items(-1)
            End If
            If rvSaved IsNot Nothing Then
                Dim itm As ReportItem
                Dim itmSaved As ReportItem

                'Items to delete
                Dim j As Integer = 0
                For i As Integer = 0 To rvStart.Items.Count - 1
                    itm = rvStart.Items(i)
                    If Not rvSaved.Exists(itm.ItemID, itm.SQLTable, itm.SQLField) Then
                        ReDim Preserve ItemsToDelete(j)
                        ItemsToDelete(j) = itm
                        j += 1
                    ElseIf itm.ReportItemType = "Image" Then      'Delete old image if image source has changed.

                        itmSaved = rvSaved.FindItem(itm.ItemID, "", "")
                        If itmSaved.ImagePath <> itm.ImagePath AndAlso itm.ImagePath.StartsWith("ImageFiles/") Then
                            Dim BaseAppPath As String = System.AppDomain.CurrentDomain.BaseDirectory()
                            Dim FullImagePath As String = (BaseAppPath & itm.ImagePath).Replace("/", "\")

                            If File.Exists(FullImagePath) Then _
                                File.Delete(FullImagePath)

                        End If
                    End If
                Next

                Dim ReportTemplate As String = rvSaved.ReportTemplate
                Dim repid As String = rvSaved.ReportID
                Dim orientation As String = rvSaved.Orientation
                Dim sSql As String = "UPDATE OURReportInfo "
                sSql &= " SET Param0type = '" & ReportTemplate & "',"
                sSql &= "Param9type = '" & orientation & "' "
                sSql &= "WHERE ReportId = '" & repid & "'"

                ExequteSQLquery(sSql)

                Dim ItemID As String
                Dim TabularColumnWidth As String
                Dim Caption As String
                Dim CaptionHeight As String
                Dim CaptionWidth As String
                Dim CaptionX As String
                Dim CaptionY As String
                Dim CaptionFontName As String
                Dim CaptionFontSize As String
                Dim CaptionForeColor As String
                Dim CaptionBackColor As String
                Dim CaptionBorderColor As String
                Dim CaptionBorderStyle As String
                Dim CaptionBorderWidth As String
                Dim CaptionFontStyle As String
                Dim CaptionUnderline As Integer = 0
                Dim CaptionStrikeout As Integer = 0
                Dim CaptionTextAlign As String
                Dim ReportItemType As String
                Dim ImagePath As String
                Dim ImageHeight As String
                Dim ImageWidth As String
                Dim ImageData As String
                Dim FieldLayout As String
                Dim SQLDatabase As String
                Dim SQLTable As String
                Dim SQLField As String
                Dim SQLDataType As String
                Dim ItemOrder As String
                Dim DataHeight As String
                Dim DataWidth As String
                Dim DataX As String
                Dim DataY As String
                Dim Height As String
                Dim Width As String
                Dim X As String
                Dim Y As String
                Dim FontName As String
                Dim FontSize As String
                Dim ForeColor As String
                Dim BackColor As String
                Dim BorderColor As String
                Dim BorderStyle As String
                Dim BorderWidth As String
                Dim FontStyle As String
                Dim Underline As Integer = 0
                Dim Strikeout As Integer = 0
                Dim TextAlign As String
                Dim Section As String
                Dim tblfld As String
                Dim field As String

                For i As Integer = 0 To ItemsToDelete.Count - 1
                    itm = ItemsToDelete(i)
                    ItemID = itm.ItemID
                    If itm.ReportItemType = "DataField" Then
                        If itm.SQLField <> "Expression" AndAlso itm.SQLField <> "Combined" Then
                            tblfld = itm.SQLTable & "." & itm.SQLField
                            field = htTblCol(tblfld).ToString
                            If field = String.Empty Then
                                field = itm.SQLField
                            End If
                            'DeleteReportColumn(repid, field)
                        Else
                            field = itm.SQLTable
                        End If
                        'tblfld = itm.SQLTable & "." & itm.SQLField
                        'field = htTblCol(tblfld).ToString
                        'If field = String.Empty Then
                        '    field = itm.SQLField
                        'End If
                        DeleteReportColumn(repid, field)
                    ElseIf itm.ReportItemType = "Image" Then
                        If itm.ImageData = "" AndAlso itm.ImagePath.StartsWith("ImageFiles/") Then
                            Dim BaseAppPath As String = System.AppDomain.CurrentDomain.BaseDirectory()
                            Dim FullImagePath As String = (BaseAppPath & itm.ImagePath).Replace("/", "\")

                            If File.Exists(FullImagePath) Then _
                                File.Delete(FullImagePath)

                        End If
                    End If
                    DeleteReportItem(repid, ItemID)
                Next

                For i As Integer = 0 To rvSaved.Items.Count - 1
                    itm = rvSaved.Items(i)
                    ItemID = itm.ItemID
                    TabularColumnWidth = itm.TabularColumnWidth
                    Caption = itm.Caption
                    CaptionHeight = itm.CaptionHeight
                    CaptionWidth = itm.CaptionWidth
                    CaptionX = IIf(itm.CaptionX <> "", itm.CaptionX, "0")
                    CaptionY = IIf(itm.CaptionY <> "", itm.CaptionY, "0")
                    CaptionFontName = itm.CaptionFontName
                    CaptionFontSize = itm.CaptionFontSize
                    CaptionForeColor = itm.CaptionForeColor
                    CaptionBackColor = itm.CaptionBackColor
                    CaptionBorderColor = itm.CaptionBorderColor
                    CaptionBorderStyle = itm.CaptionBorderStyle
                    CaptionBorderWidth = itm.CaptionBorderWidth
                    CaptionFontStyle = itm.CaptionFontStyle
                    CaptionUnderline = IIf(itm.CaptionUnderline, 1, 0)
                    CaptionStrikeout = IIf(itm.CaptionStrikeout, 1, 0)
                    CaptionTextAlign = itm.CaptionTextAlign
                    ReportItemType = itm.ReportItemType
                    ImagePath = itm.ImagePath
                    ImageHeight = itm.ImageHeight
                    ImageWidth = itm.ImageWidth
                    ImageData = itm.ImageData
                    FieldLayout = itm.FieldLayout
                    SQLDatabase = itm.SQLDatabase
                    SQLTable = itm.SQLTable
                    SQLField = itm.SQLField
                    SQLDataType = itm.SQLDataType
                    ItemOrder = itm.ItemOrder
                    DataHeight = itm.DataHeight
                    DataWidth = itm.DataWidth
                    DataX = IIf(itm.DataX <> "", itm.DataX, "0")
                    DataY = IIf(itm.DataY <> "", itm.DataY, "0")
                    Height = itm.Height
                    Width = itm.Width
                    X = itm.X
                    Y = itm.Y
                    FontName = itm.FontName
                    FontSize = itm.FontSize
                    ForeColor = itm.ForeColor
                    BackColor = itm.BackColor
                    BorderColor = itm.BorderColor
                    BorderStyle = itm.BorderStyle
                    BorderWidth = itm.BorderWidth
                    FontStyle = itm.FontStyle
                    Underline = IIf(itm.Underline, 1, 0)
                    Strikeout = IIf(itm.Strikeout, 1, 0)
                    TextAlign = itm.TextAlign
                    Section = itm.Section

                    If ReportItemType = "DataField" Then
                        tblfld = itm.SQLTable & "." & itm.SQLField
                        If itm.SQLField <> "Expression" AndAlso itm.SQLField <> "Combined" Then
                            'tblfld = itm.SQLTable & "." & itm.SQLField
                            field = htTblCol(tblfld).ToString
                            If field = String.Empty Then
                                field = itm.SQLField
                            End If
                            If Not SQLFieldExists(repid, tblfld) Then
                                sSql = "INSERT INTO OURReportSQLquery "
                                sSql &= "(ReportId,Doing,Tbl1,Tbl1Fld1) "
                                sSql &= "VALUES ('" & repid & "','SELECT','" & itm.SQLTable & "','" & itm.SQLField & "')"
                                ExequteSQLquery(sSql)
                                GetFormattedFields()
                                htFields = CType(Session("htFields"), Hashtable)
                                htTblCol = CType(Session("htTblCol"), Hashtable)
                                field = htTblCol(tblfld).ToString
                                If field = String.Empty Then
                                    field = itm.SQLField
                                End If
                            End If
                        Else
                            field = itm.SQLTable
                        End If
                        'tblfld = itm.SQLTable & "." & itm.SQLField
                        'field = htTblCol(tblfld).ToString
                        'If field = String.Empty Then
                        '    field = itm.SQLField
                        'End If
                        'If Not SQLFieldExists(repid, tblfld) Then
                        '    sSql = "INSERT INTO OURReportSQLquery "
                        '    sSql &= "(ReportId,Doing,Tbl1,Tbl1Fld1) "
                        '    sSql &= "VALUES ('" & repid & "','SELECT','" & itm.SQLTable & "','" & itm.SQLField & "')"
                        '    ExequteSQLquery(sSql)
                        '    GetFormattedFields()
                        '    htFields = CType(Session("htFields"), Hashtable)
                        '    htTblCol = CType(Session("htTblCol"), Hashtable)
                        '    field = htTblCol(tblfld).ToString
                        '    If field = String.Empty Then
                        '        field = itm.SQLField
                        '    End If
                        'End If
                        If ReportFieldExists(repid, field) Then
                            sSql = "UPDATE OURReportFormat "
                            sSql &= "SET `Order` = '" & ItemOrder & "'"
                            If tblfld <> Caption AndAlso field <> Caption Then
                                sSql &= ",Prop1 = '" & Caption & "' "
                            Else
                                sSql &= " "
                            End If
                            sSql &= "WHERE ReportId = '" & repid & "' And Prop = 'FIELDS' And Val = '" & field & "'"
                        Else
                            sSql = "INSERT INTO OURReportFormat "
                            sSql &= "(ReportId,Prop,Val,`Order`,Prop1) "
                            sSql &= "VALUES ('" & repid & "','FIELDS','" & field & "','" & ItemOrder & "',"
                            If tblfld <> Caption AndAlso field <> Caption Then
                                sSql &= "'" & Caption & "')"
                            Else
                                sSql &= "'')"
                            End If
                        End If
                        ExequteSQLquery(sSql)
                    End If

                    If ReportItemType = "Image" Then
                        If ImageData <> String.Empty AndAlso ImageData.StartsWith("data:image/") Then
                            If ImagePath <> "" AndAlso Not ImagePath.Contains("/") Then
                                Dim FileName As String = repid & "_" & ItemID & "_" & ImagePath
                                Dim FullPath As String = System.AppDomain.CurrentDomain.BaseDirectory() & "\ImageFiles\" & FileName
                                Dim result As String = SaveImageFromDataUrl(ImageData, FullPath)
                                If result <> "" Then
                                    WriteToAccessLog(Session("logon"), "WARNING!! Saving " & FileName & " to ImageFiles has failed with this message: " & result, 3)
                                    ImagePath = ""
                                Else
                                    WriteToAccessLog(Session("logon"), FileName & " saved to ImageFiles folder. " & result, 3)
                                    ImagePath = "ImageFiles/" & FileName
                                End If
                            End If
                        ElseIf ImagePath <> "" AndAlso Not ImagePath.Contains("/") Then
                            ImagePath = String.Empty
                        End If
                    End If

                    If ReportItemExists(repid, SQLTable, SQLField) OrElse ReportItemIDExists(repid, ItemID) Then
                        sSql = "UPDATE OURReportItems "
                        sSql &= "SET TabularColumnWidth = '" & TabularColumnWidth & "',"
                        sSql &= "Caption = '" & Caption & "',"
                        sSql &= "CaptionHeight = '" & CaptionHeight & "',"
                        sSql &= "CaptionWidth = '" & CaptionWidth & "',"
                        sSql &= "CaptionX = '" & CaptionX & "',"
                        sSql &= "CaptionY = '" & CaptionY & "',"
                        sSql &= "CaptionFontName = '" & CaptionFontName & "',"
                        sSql &= "CaptionFontSize = '" & CaptionFontSize & "',"
                        sSql &= "CaptionFontStyle = '" & CaptionFontStyle & "',"
                        sSql &= "CaptionStrikeout = " & CaptionStrikeout.ToString & ","
                        sSql &= "CaptionUnderline = " & CaptionUnderline.ToString & ","
                        sSql &= "CaptionTextAlign = '" & CaptionTextAlign & "',"
                        sSql &= "CaptionForeColor = '" & CaptionForeColor & "',"
                        sSql &= "CaptionBackColor = '" & CaptionBackColor & "',"
                        sSql &= "CaptionBorderColor = '" & CaptionBorderColor & "',"
                        sSql &= "CaptionBorderStyle = '" & CaptionBorderStyle & "',"
                        sSql &= "CaptionBorderWidth = '" & CaptionBorderWidth & "',"
                        sSql &= "FieldLayout = '" & FieldLayout & "',"
                        sSql &= "ImagePath = '" & ImagePath & "',"
                        sSql &= "ImageWidth= '" & ImageWidth & "',"
                        sSql &= "ImageHeight = '" & ImageHeight & "',"
                        sSql &= "ItemOrder = '" & ItemOrder & "',"
                        sSql &= "DataHeight = '" & DataHeight & "',"
                        sSql &= "DataWidth = '" & DataWidth & "',"
                        sSql &= "DataX = '" & DataX & "',"
                        sSql &= "DataY = '" & DataY & "',"
                        sSql &= "Height = '" & Height & "',"
                        sSql &= "Width = '" & Width & "',"
                        sSql &= "X = '" & X & "',"
                        sSql &= "Y = '" & Y & "',"
                        sSql &= "FontName = '" & FontName & "',"
                        sSql &= "FontSize = '" & FontSize & "',"
                        sSql &= "FontStyle = '" & FontStyle & "',"
                        sSql &= "Strikeout = " & Strikeout.ToString & ","
                        sSql &= "Underline = " & Underline.ToString & ","
                        sSql &= "TextAlign = '" & TextAlign & "',"
                        sSql &= "ForeColor = '" & ForeColor & "',"
                        sSql &= "BackColor = '" & BackColor & "',"
                        sSql &= "BorderColor = '" & BorderColor & "',"
                        sSql &= "BorderStyle = '" & BorderStyle & "',"
                        sSql &= "BorderWidth = '" & BorderWidth & "' "

                        sSql &= "WHERE ReportID = '" & repid & "' And ItemID = '" & ItemID & "'"
                    Else
                        sSql = "INSERT INTO OurReportItems "
                        sSql &= "(ReportID, ItemID,TabularColumnWidth,Caption,CaptionHeight,CaptionWidth,CaptionX,CaptionY,CaptionFontName,CaptionFontSize,"
                        sSql &= "CaptionFontStyle,CaptionUnderline,CaptionStrikeout,"
                        sSql &= "CaptionTextAlign,CaptionForeColor,CaptionBackColor,"
                        sSql &= "CaptionBorderColor,CaptionBorderStyle,CaptionBorderWidth,ReportItemType,"
                        sSql &= "ImagePath,ImageWidth,ImageHeight,FieldLayout,SQLDatabase,SQLTable,SQLField,SQLDataType,"
                        sSql &= "ItemOrder,DataHeight,DataWidth,DataX,DataY,Height,Width,X,Y,FontName,FontSize,ForeColor,"
                        sSql &= "FontStyle,Underline,Strikeout,TextAlign,"
                        sSql &= "BackColor,BorderColor,BorderStyle,BorderWidth,[Section]) "
                        sSql &= "VALUES ('" & repid & "','"
                        sSql &= ItemID & "','"
                        sSql &= TabularColumnWidth & "','"
                        sSql &= Caption & "','"
                        sSql &= CaptionHeight & "','"
                        sSql &= CaptionWidth & "','"
                        sSql &= CaptionX & "','"
                        sSql &= CaptionY & "','"
                        sSql &= CaptionFontName & "','"
                        sSql &= CaptionFontSize & "','"
                        sSql &= CaptionFontStyle & "',"
                        sSql &= CaptionUnderline.ToString & ","
                        sSql &= CaptionStrikeout.ToString & ",'"
                        sSql &= CaptionTextAlign & "','"
                        sSql &= CaptionForeColor & "','"
                        sSql &= CaptionBackColor & "','"
                        sSql &= CaptionBorderColor & "','"
                        sSql &= CaptionBorderStyle & "','"
                        sSql &= CaptionBorderWidth & "','"
                        sSql &= ReportItemType & "','"
                        sSql &= ImagePath & "','"
                        sSql &= ImageWidth & "','"
                        sSql &= ImageHeight & "','"
                        sSql &= FieldLayout & "','"
                        sSql &= SQLDatabase & "','"
                        sSql &= SQLTable & "','"
                        sSql &= SQLField & "','"
                        sSql &= SQLDataType & "','"
                        sSql &= ItemOrder & "','"
                        sSql &= DataHeight & "','"
                        sSql &= DataWidth & "','"
                        sSql &= DataX & "','"
                        sSql &= DataY & "','"
                        sSql &= Height & "','"
                        sSql &= Width & "','"
                        sSql &= X & "','"
                        sSql &= Y & "','"
                        sSql &= FontName & "','"
                        sSql &= FontSize & "','"
                        sSql &= ForeColor & "','"
                        sSql &= FontStyle & "',"
                        sSql &= Underline.ToString & ","
                        sSql &= Strikeout.ToString & ",'"
                        sSql &= TextAlign & "','"
                        sSql &= BackColor & "','"
                        sSql &= BorderColor & "','"
                        sSql &= BorderStyle & "','"
                        sSql &= BorderWidth & "','"
                        sSql &= Section & "')"
                    End If
                    Dim re As String = ExequteSQLquery(sSql)
                    If Not re = "Query executed fine." Then
                        re = "ERROR!! " & re
                        'TODO Label or MessageBox?
                    End If
                Next
                Dim ReportDate As String = DateToString(Now)
                If ReportViewExists(rvSaved.ReportID) Then
                    sSql = "UPDATE OURReportView "
                    sSql &= "SET ReportID = '" & rvSaved.ReportID & "',"
                    sSql &= "ReportTemplate = '" & rvSaved.ReportTemplate & "',"
                    sSql &= "ReportTitle = '" & rvSaved.Title & "',"
                    sSql &= "Orientation = '" & rvSaved.Orientation & "',"
                    sSql &= "ReportFieldLayout = '" & rvSaved.ReportFieldLayout & "',"
                    sSql &= "HeaderHeight = '" & rvSaved.HeaderHeight & "',"
                    sSql &= "HeaderBackColor = '" & rvSaved.HeaderBackColor & "',"
                    sSql &= "HeaderBorderColor = '" & rvSaved.HeaderBorderColor & "',"
                    sSql &= "HeaderBorderStyle = '" & rvSaved.HeaderBorderStyle & "',"
                    sSql &= "HeaderBorderWidth ='" & rvSaved.HeaderBorderWidth & "',"
                    sSql &= "FooterHeight = '" & rvSaved.FooterHeight & "',"
                    sSql &= "FooterBackColor = '" & rvSaved.FooterBackColor & "',"
                    sSql &= "FooterBorderColor = '" & rvSaved.FooterBorderColor & "',"
                    sSql &= "FooterBorderStyle = '" & rvSaved.FooterBorderStyle & "',"
                    sSql &= "FooterBorderWidth ='" & rvSaved.FooterBorderWidth & "',"

                    sSql &= "DataFontName = '" & rvSaved.DataFontName & "',"
                    sSql &= "DataFontSize = '" & rvSaved.DataFontSize & "',"
                    sSql &= "DataForeColor = '" & rvSaved.DataForeColor & "',"
                    sSql &= "DataBackColor = '" & rvSaved.DataBackColor & "',"
                    sSql &= "DataBorderColor = '" & rvSaved.DataBorderColor & "',"
                    sSql &= "DataBorderStyle = '" & rvSaved.DataBorderStyle & "',"
                    sSql &= "DataBorderWidth = '" & rvSaved.DataBorderWidth & "',"
                    sSql &= "DataFontStyle = '" & rvSaved.DataFontStyle & "',"
                    sSql &= "DataUnderline = " & IIf(rvSaved.DataUnderline, 1, 0) & ","
                    sSql &= "DataStrikeout = " & IIf(rvSaved.DataStrikeout, 1, 0) & ","
                    sSql &= "ReportDetailAlign = '" & rvSaved.ReportDetailAlign & "',"
                    sSql &= "LabelFontName = '" & rvSaved.LabelFontName & "',"
                    sSql &= "LabelFontSize = '" & rvSaved.LabelFontSize & "',"
                    sSql &= "LabelForeColor = '" & rvSaved.LabelForeColor & "',"
                    sSql &= "LabelBackColor = '" & rvSaved.LabelBackColor & "',"
                    sSql &= "LabelBorderColor = '" & rvSaved.LabelBorderColor & "',"
                    sSql &= "LabelBorderStyle = '" & rvSaved.LabelBorderStyle & "',"
                    sSql &= "LabelBorderWidth = '" & rvSaved.LabelBorderWidth & "',"
                    sSql &= "LabelFontStyle = '" & rvSaved.LabelFontStyle & "',"
                    sSql &= "LabelUnderline = " & IIf(rvSaved.LabelUnderline, 1, 0) & ","
                    sSql &= "LabelStrikeout = " & IIf(rvSaved.LabelStrikeout, 1, 0) & ","
                    sSql &= "ReportCaptionAlign = '" & rvSaved.ReportCaptionAlign & "',"
                    sSql &= "MarginBottom = '" & rvSaved.MarginBottom & "',"
                    sSql &= "MarginLeft = '" & rvSaved.MarginLeft & "',"
                    sSql &= "MarginRight = '" & rvSaved.MarginRight & "',"
                    sSql &= "MarginTop = '" & rvSaved.MarginTop & "',"
                    'sSql &= "CreatedBy = '" & Session("User") & "',"
                    'sSql &= "DateCreated = '" & ReportDate & "',"
                    sSql &= "UpdatedBy = '" & Session("User") & "',"
                    sSql &= "LastUpdate = '" & ReportDate & "' "
                    sSql &= "Where ReportID = '" & rvSaved.ReportID & "'"
                Else
                    sSql = "INSERT INTO OURReportView ("
                    sSql &= "ReportID,"
                    sSql &= "ReportTemplate,"
                    sSql &= "ReportTitle,"
                    sSql &= "Orientation,"
                    sSql &= "ReportFieldLayout,"

                    sSql &= "HeaderHeight,"
                    sSql &= "HeaderBackColor,"
                    sSql &= "HeaderBorderColor,"
                    sSql &= "HeaderBorderStyle,"
                    sSql &= "HeaderBorderWidth,"

                    sSql &= "FooterHeight,"
                    sSql &= "FooterBackColor,"
                    sSql &= "FooterBorderColor,"
                    sSql &= "FooterBorderStyle,"
                    sSql &= "FooterBorderWidth,"

                    sSql &= "DataFontName,"
                    sSql &= "DataFontSize,"
                    sSql &= "DataForeColor,"
                    sSql &= "DataBackColor,"
                    sSql &= "DataBorderColor,"
                    sSql &= "DataBorderStyle,"
                    sSql &= "DataBorderWidth,"
                    sSql &= "DataFontStyle,"
                    sSql &= "DataUnderline,"
                    sSql &= "DataStrikeout,"
                    sSql &= "ReportDetailAlign,"
                    sSql &= "LabelFontName,"
                    sSql &= "LabelFontSize,"
                    sSql &= "LabelForeColor,"
                    sSql &= "LabelBackColor,"
                    sSql &= "LabelBorderColor,"
                    sSql &= "LabelBorderStyle,"
                    sSql &= "LabelBorderWidth,"

                    sSql &= "LabelFontStyle,"
                    sSql &= "LabelUnderline,"
                    sSql &= "LabelStrikeout,"
                    sSql &= "ReportCaptionAlign,"
                    sSql &= "MarginBottom,"
                    sSql &= "MarginLeft,"
                    sSql &= "MarginRight,"
                    sSql &= "MarginTop,"
                    sSql &= "CreatedBy,"
                    sSql &= "DateCreated,"
                    sSql &= "UpdatedBy,"
                    sSql &= "LastUpdate)"
                    sSql &= "VALUES ("

                    sSql &= "'" & rvSaved.ReportID & "',"
                    sSql &= "'" & rvSaved.ReportTemplate & "',"
                    sSql &= "'" & rvSaved.Title & "',"
                    sSql &= "'" & rvSaved.Orientation & "',"
                    sSql &= "'" & rvSaved.ReportFieldLayout & "',"

                    sSql &= "'" & rvSaved.HeaderHeight & "',"
                    sSql &= "'" & rvSaved.HeaderBackColor & "',"
                    sSql &= "'" & rvSaved.HeaderBorderColor & "',"
                    sSql &= "'" & rvSaved.HeaderBorderStyle & "',"
                    sSql &= "'" & rvSaved.HeaderBorderWidth & "',"
                    sSql &= "'" & rvSaved.FooterHeight & "',"
                    sSql &= "'" & rvSaved.FooterBackColor & "',"
                    sSql &= "'" & rvSaved.FooterBorderColor & "',"
                    sSql &= "'" & rvSaved.FooterBorderStyle & "',"
                    sSql &= "'" & rvSaved.FooterBorderWidth & "',"

                    sSql &= "'" & rvSaved.DataFontName & "',"
                    sSql &= "'" & rvSaved.DataFontSize & "',"
                    sSql &= "'" & rvSaved.DataForeColor & "',"
                    sSql &= "'" & rvSaved.DataBackColor & "',"
                    sSql &= "'" & rvSaved.DataBorderColor & "',"
                    sSql &= "'" & rvSaved.DataBorderStyle & "',"
                    sSql &= "'" & rvSaved.DataBorderWidth & "',"
                    sSql &= "'" & rvSaved.DataFontStyle & "',"
                    sSql &= IIf(rvSaved.DataUnderline, 1, 0) & ","
                    sSql &= IIf(rvSaved.DataStrikeout, 1, 0) & ","
                    sSql &= "'" & rvSaved.ReportDetailAlign & "',"
                    sSql &= "'" & rvSaved.LabelFontName & "',"
                    sSql &= "'" & rvSaved.LabelFontSize & "',"
                    sSql &= "'" & rvSaved.LabelForeColor & "',"
                    sSql &= "'" & rvSaved.LabelBackColor & "',"
                    sSql &= "'" & rvSaved.LabelBorderColor & "',"
                    sSql &= "'" & rvSaved.LabelBorderStyle & "',"
                    sSql &= "'" & rvSaved.LabelBorderWidth & "',"
                    sSql &= "'" & rvSaved.LabelFontStyle & "',"
                    sSql &= IIf(rvSaved.LabelUnderline, 1, 0) & ","
                    sSql &= IIf(rvSaved.LabelStrikeout, 1, 0) & ","
                    sSql &= "'" & rvSaved.ReportCaptionAlign & "',"
                    sSql &= "'" & rvSaved.MarginBottom & "',"
                    sSql &= "'" & rvSaved.MarginLeft & "',"
                    sSql &= "'" & rvSaved.MarginRight & "',"
                    sSql &= "'" & rvSaved.MarginTop & "',"
                    sSql &= "'" & Session("User") & "',"
                    sSql &= "'" & ReportDate & "',"
                    sSql &= "'" & Session("User") & "',"
                    sSql &= "'" & ReportDate & "')"
                End If
                Dim ret As String = ExequteSQLquery(sSql)

                Dim dtb As DataTable = GetReportFields(repid) 'sorted by Order
                dtb = CorrectFieldOrder(repid, dtb, "Order", "OURReportFormat", "Val", "Prop", "FIELDS")
                For i = 0 To dtb.Rows.Count - 1
                    sSql = "UPDATE OURReportFormat SET [Order]=" & dtb.Rows(i)("Order") & " WHERE ReportId = '" & repid & "' AND [Val]= '" & dtb.Rows(i)("Val") & "' AND Prop = 'FIELDS'"
                    ExequteSQLquery(sSql)
                Next
                If FileInOURFiles(repid) Then
                    sSql = "UPDATE OURFiles SET Prop2 = 'designer' WHERE ReportId='" & repid & "' AND Type = 'RDL'"
                    ExequteSQLquery(sSql)
                End If
                Dim sqlquerytext As String = MakeSQLQueryFromDB(repid, Session("UserConnString"), Session("UserConnProvider"))
                SaveSQLquery(repid, sqlquerytext)
                ret = UpdateXSDandRDL(repid, Session("UserConnString"), Session("UserConnProvider"))
                CopyFormattedFieldOrder(repid)
                ret = ret & ", " & CreateCleanReportColumnsFieldsItems(repid, Session("dbname"), Session("UserConnString"), Session("UserConnProvider"))
                Session("ret") = ret
                If ret.StartsWith("ERROR!!") Then
                    WriteToAccessLog(Session("logon"), ret, 3)
                    'ret = "Report Format has been updated with errors. "
                    MessageBox.Show(ret, "Report Designer", "Error", Controls_Msgbox.Buttons.OK)
                Else
                    WriteToAccessLog(Session("logon"), ret, 3)
                    ret = "Report Format has been updated. "
                    Session("ret") = ret
                    'Response.Redirect("ShowReport.aspx?srd=3")
                    'MessageBox.DefaultButton = Controls_Msgbox.MessageDefaultButton.Other2
                    'MessageBox.OtherButtonText1 = "Show Report"
                    'MessageBox.OtherButtonText2 = "Return to Designer"
                    ''MessageBox.Show(ret, "Report Designer", "OK", Controls_Msgbox.Buttons.OtherOtherCancel, Controls_Msgbox.MessageIcon.Information)
                    'MessageBox.Show(ret, "Report Designer", "OK", Controls_Msgbox.Buttons.OtherOtherCancel, Controls_Msgbox.MessageIcon.Information, Controls_Msgbox.MessageDefaultButton.Other2)
                End If
                'MessageBox.Show(ret, "Report Designer", "", Controls_Msgbox.Buttons.OK)
            End If

        End If
    End Sub

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Select Case e.Tag
            Case "Error"
                btnClose_Click(btnSubmit, EventArgs.Empty)
            Case "OK"
                If e.Result = Controls_Msgbox.MessageResult.Other1 Then
                    'btnClose_Click(btnSubmit, EventArgs.Empty)
                    Response.Redirect("ShowReport.aspx?srd=3")
                ElseIf e.Result = Controls_Msgbox.MessageResult.Other2 Then
                    ClearSession()
                    Response.Redirect("ReportDesigner.aspx")
                Else
                    btnClose_Click(btnSubmit, EventArgs.Empty)
                End If
            Case "Fields Not Defined"
                Session("urlback") = "SQLquery.aspx?tnq=0"
                btnClose_Click(btnSubmit, EventArgs.Empty)
        End Select

    End Sub


    Private Sub btnUploadImageFile_Click(sender As Object, e As EventArgs) Handles btnUploadImageFile.Click
        Dim imageUploadDir As String = ConfigurationManager.AppSettings("imageupload").ToString.Replace("/", "\")
        Dim filename As String = String.Empty
        Dim fileExt As String = String.Empty
        Dim strFile As String = String.Empty

        If FileRDL.HasFile Then
        Else
            MessageBox.Show("No file selected", "Upload Image File", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        End If

    End Sub
End Class


