Imports System.ComponentModel


Imports System.Data

Partial Class Controls_TicketDlg
    Inherits System.Web.UI.UserControl

#Region "Enums"
    Public Enum TicketDialogResult
        OK = 0
        Cancel = 1
        Yes = 2
        No = 3
        Retry = 4
        Ignore = 5
        Abort = 6
        None = 7
        Other1 = 8
        Other2 = 9
        Other3 = 10
    End Enum
    Public Enum Mode
        Add
        Edit
        None
    End Enum
#End Region

#Region "Classes"
    <Serializable> Public Class TicketData
        Public Property DateTime As String
        Public Property From As String
        Public Property Version As String
        Public Property Deadline As String
        Public Property Description As String
        Public Property Status As String
        Public Property Comments As String
        Public Property [To] As String
        Public Property Attachment As String
        Public Property ID As String
    End Class
    Public Class TicketDlgEventArgs
        Inherits System.EventArgs

        Private mTicketItem As TicketData
        Private mOldTicketItem As TicketData
        Private mResult As TicketDialogResult
        Private mEntryMode As Mode
        Public Sub New(MsgResult As TicketDialogResult, TktItem As TicketData, OldTktItem As TicketData, TicketMode As Mode)
            mResult = MsgResult
            mTicketItem = TktItem
            mOldTicketItem = OldTktItem
            mEntryMode = TicketMode
        End Sub
        Public ReadOnly Property Result As TicketDialogResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property TicketItem As TicketData
            Get
                Return mTicketItem
            End Get
        End Property
        Public ReadOnly Property EntryMode As Mode
            Get
                Return mEntryMode
            End Get
        End Property
    End Class
#End Region

#Region "Properties"
    <Browsable(False)>
    Property User As String
        Get
            If ViewState("User") Is Nothing Then ViewState("User") = String.Empty
            Return ViewState("User").ToString
        End Get
        Set(value As String)
            ViewState("User") = value
        End Set
    End Property
    <Browsable(False)>
    Property UserItems As ListItemCollection
        Get
            Return ddSendTo.Items
        End Get
        Set(value As ListItemCollection)
            'ddFrom.Items.Clear()
            ddSendTo.Items.Clear()

            'ddTo.Items.Clear()
            If Not value Is Nothing AndAlso value.Count > 0 Then
                'ddFrom.Items.Add(New ListItem(value(0).Text, value(0).Value))
                For i As Integer = 0 To value.Count - 1
                    'ddFrom.Items.Add(New ListItem(value(i).Text, value(i).Value))
                    'ddTo.Items.Add(New ListItem(value(i).Text, value(i).Value))
                    ddSendTo.Items.Add(New ListItem(value(i).Text, value(i).Value))
                Next
            End If
        End Set
    End Property
    Public Property FontName As String
        Get
            If ViewState("FontName") Is Nothing Then
                ViewState("FontName") = "Arial"
            End If
            Return ViewState("FontName").ToString
        End Get
        Set(value As String)
            If value = String.Empty Then
                pnlHeader.Font.Name = "Arial"
                lblComments.Font.Name = "Arial"
                lblDateTime.Font.Name = "Arial"
                lblDateTimeVal.Font.Name = "Arial"
                lblDescription.Font.Name = "Arial"
                lblFrom.Font.Name = "Arial"
                lblHeader.Font.Name = "Arial"
                lblStatus.Font.Name = "Arial"
                lblTicketNo.Font.Name = "Arial"
                lblTicketNoVal.Font.Name = "Arial"
                lblVersion.Font.Name = "Arial"
                ddVersion.Font.Name = "Arial"
                ddFrom.Font.Name = "Arial"
                'ddTo.Font.Name = "Arial"
                ddSendTo.FontName = "Arial"
                txtComments.Font.Name = "Arial"
                txtDescription.Font.Name = "Arial"
                txtOtherStatus.Font.Name = "Arial"
                btnOK.Font.Name = "Arial"
                btnCancel.Font.Name = "Arial"
                divInput.Style.Item("font-name") = "Arial"
                ViewState("FontName") = "Arial"
                ckNoEmail.Font.Name = "Arial"
            ElseIf divInput.Style.Item("font-name") <> value Then
                pnlHeader.Font.Name = value
                lblComments.Font.Name = value
                lblDateTime.Font.Name = value
                lblDateTimeVal.Font.Name = value
                lblDescription.Font.Name = value
                lblFrom.Font.Name = value
                lblHeader.Font.Name = value
                lblStatus.Font.Name = value
                lblTicketNo.Font.Name = value
                lblTicketNoVal.Font.Name = value
                lblVersion.Font.Name = value
                ddVersion.Font.Name = value
                ddFrom.Font.Name = value
                'ddTo.Font.Name = value
                ddSendTo.FontName = value
                txtComments.Font.Name = value
                txtDescription.Font.Name = value
                txtOtherStatus.Font.Name = value
                btnOK.Font.Name = value
                btnCancel.Font.Name = value
                divInput.Style.Item("font-name") = value
                ckNoEmail.Font.Name = value
                ViewState("FontName") = value
            End If
        End Set
    End Property
    Public Property FontSize As FontUnit
        Get
            If ViewState("FontSize") Is Nothing Then
                ViewState("FontSize") = lblComments.Font.Size
            End If
            Return CType(ViewState("FontSize"), FontUnit)
        End Get
        Set(value As FontUnit)
            If value <> lblComments.Font.Size Then
                lblComments.Font.Size = value
                lblDateTime.Font.Size = value
                lblDateTimeVal.Font.Size = value
                lblDescription.Font.Size = value
                lblFrom.Font.Size = value
                lblHeader.Font.Size = value
                lblStatus.Font.Size = value
                lblTicketNo.Font.Size = value
                lblTicketNoVal.Font.Size = value
                lblVersion.Font.Size = value
                ddVersion.Font.Size = value
                ddFrom.Font.Size = value
                'ddTo.Font.Size = value
                ddSendTo.FontSize = value
                txtComments.Font.Size = value
                txtDescription.Font.Size = value
                txtOtherStatus.Font.Size = value
                divInput.Style.Item("font-size") = value.ToString
                ckNoEmail.Font.Size = value
                ViewState("FontSize") = value
            End If
        End Set
    End Property
    <Browsable(False)>
    Public Property Data As TicketData
        Get
            If ViewState("Data") Is Nothing Then
                ViewState("Data") = New TicketData()
            End If
            Return CType(ViewState("Data"), TicketData)
        End Get
        Set(value As TicketData)
            ViewState("Data") = value
        End Set
    End Property
    <Browsable(False)>
    Public Property OldData As TicketData
        Get
            If ViewState("OldData") Is Nothing Then
                ViewState("OldData") = New TicketData()
            End If
            Return CType(ViewState("OldData"), TicketData)
        End Get
        Set(value As TicketData)
            ViewState("OldData") = value
        End Set
    End Property
    <Browsable(False)>
    Public Property EntryMode As Mode
        Get
            If ViewState("EntryMode") Is Nothing Then
                ViewState("EntryMode") = Mode.None
            End If
            Return CType(ViewState("EntryMode"), Mode)
        End Get
        Set(value As Mode)
            ViewState("EntryMode") = value
        End Set
    End Property
    Public Property OKCaption As String
        Get
            If ViewState("OKCaption") Is Nothing Then
                ViewState("OKCaption") = btnOK.Text
            End If
            Return ViewState("OKCaption").ToString
        End Get
        Set(value As String)
            If value.ToUpper = "OK" Then
                btnOK.Style("Width") = "80px;"
            Else
                btnOK.Style("Width") = "Auto;"
            End If
            ViewState("OKCaption") = value
            btnOK.Text = value
        End Set
    End Property

    Public Property HeadingText As String
        Get
            If ViewState("HeadingText") Is Nothing Then
                ViewState("HeadingText") = lblHeader.Text
            End If
            Return ViewState("HeadingText").ToString
        End Get
        Set(value As String)
            ViewState("HeadingText") = value
            lblHeader.Text = value
        End Set
    End Property
    Public Property DropShadow As Boolean
        Get
            If ViewState("DropShadow") Is Nothing Then
                ViewState("DropShadow") = popDlg.DropShadow
            End If
            Return Convert.ToBoolean(ViewState("DropShadow"))
        End Get
        Set(value As Boolean)
            popDlg.DropShadow = value
            ViewState("DropShadow") = value
        End Set
    End Property
    <Browsable(True), DefaultValue("550px")>
    Public Property Width As String
        Get
            If ViewState("Width") Is Nothing Then
                ViewState("Width") = pnlBody.Style("Width")
            End If
            Return ViewState("Width").ToString
        End Get
        Set(value As String)
            If value <> String.Empty Then
                pnlBody.Style("width") = value
                ViewState("Width") = pnlBody.Style("Width")
            End If
        End Set
    End Property
    <Browsable(True), DefaultValue(1)>
    Public Property BorderWidth As Integer
        Get
            Dim bordwidth As String = pnlBody.Style("border-width").ToString
            If bordwidth <> String.Empty Then
                Return CInt(Val(bordwidth))
            Else
                Return 1
            End If

        End Get
        Set(value As Integer)
            pnlBody.Style("border-width") = value.ToString & "px"
        End Set
    End Property
    <Browsable(True), DefaultValue(GetType(Drawing.Color), "DarkGray")>
    Public Property BorderColor As Drawing.Color
        Get
            Return Drawing.Color.FromName(pnlBody.Style("border-color"))
        End Get
        Set(value As Drawing.Color)
            If value <> Drawing.Color.Transparent Then
                pnlBody.Style("border-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
                Dim borderwidth As String = pnlBody.Style("border-width")
                Dim borderstyle As String = pnlBody.Style("border-style")
                If borderwidth = String.Empty Then pnlBody.Style("border-width") = "1px"
                If borderstyle = String.Empty Then pnlBody.Style("border-style") = "solid"
            End If
        End Set
    End Property
    Public Property BodyBackColor As System.Drawing.Color
        Get
            Return Drawing.Color.FromName(pnlBody.Style("background-color"))
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                value = System.Drawing.Color.White
            End If
            pnlBody.Style("background-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
        End Set
    End Property
    Public Property HeadingBackColor As System.Drawing.Color
        Get
            Return Drawing.Color.FromName(pnlHeader.Style("background-color"))
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                value = System.Drawing.Color.White
            End If
            pnlHeader.Style("background-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
        End Set
    End Property
    Public Property HeadingFontSize As FontUnit
        Get
            Return pnlHeader.Font.Size
        End Get
        Set(value As FontUnit)
            If value <> pnlHeader.Font.Size Then
                pnlHeader.Font.Size = value
            End If
        End Set
    End Property
    Public Property HeadingForeColor As System.Drawing.Color
        Get
            Return Drawing.Color.FromName(pnlHeader.Style("color"))
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                value = System.Drawing.Color.White
            End If
            pnlHeader.Style("color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
        End Set
    End Property
    'Public Property AttachedFile As String
    '    Get
    '        If ViewState("AttachedFile") Is Nothing Then
    '            ViewState("AttachedFile") = String.Empty
    '        End If
    '        Return ViewState("AttachedFile").ToString
    '    End Get
    '    Set(value As String)
    '        ViewState("AttachedFile") = value
    '        hdnAttached.Value = value
    '    End Set
    'End Property
#End Region

#Region "Event Definitions"
    Public Delegate Sub TicketDlgEventHandler(sender As Object, e As TicketDlgEventArgs)
    Public Event TicketDialogResulted As TicketDlgEventHandler
    Protected Overridable Sub OnTicketDialogResulted(e As TicketDlgEventArgs)
        RaiseEvent TicketDialogResulted(Me, e)
    End Sub
#End Region

#Region "Methods"

    Private Sub SetControlFocus(ctlId As String)
        Dim id As String = Me.ClientID & "_" & ctlId
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Page.ClientScript
        With sb
            .Append("<script language='JavaScript'>")
            .Append("function SetdlgTicketFocus()")
            .Append("{")
            .Append("  var ctl = document.getElementById('" & id & "');")
            .Append("  if (ctl != null)")
            .Append("    ctl.focus();")
            .Append("}")
            .Append("window.onload = SetdlgTicketFocus;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "SetdlgTicketFocus", sb.ToString)
    End Sub
    Private Sub LoadTicketData()
        divLblBreak.Style.Item("display") = ""
        divNoEmail.Style.Item("display") = ""
        lblTicketNoVal.Text = Data.ID
        lblDateTimeVal.Text = Data.DateTime
        Dim li As ListItem = ddSendTo.Items.FindByText(Data.From)
        ddFrom.Text = Data.From
        Dim lis As ListItem = ddVersion.Items.FindByText(Data.Version)
        If lis IsNot Nothing Then
            ddVersion.SelectedIndex = ddVersion.Items.IndexOf(lis)
        Else
            lis = New ListItem(Data.Version, Data.Version)
            ddVersion.Items.Insert(0, lis)
            ddVersion.SelectedIndex = 0
        End If
        txtDescription.Text = cleanTextLight(Data.Description)
        li = ddStatus.Items.FindByText(Data.Status)
        If li IsNot Nothing Then
            ddStatus.SelectedIndex = ddStatus.Items.IndexOf(li)
        Else
            li = ddStatus.Items.FindByText("other")
            ddStatus.SelectedIndex = ddStatus.Items.IndexOf(li)
            divOtherStatus.Style.Item("display") = ""
            txtOtherStatus.Text = Data.Status
        End If
        If Data.Comments <> String.Empty Then
            trPreviousComments.Visible = True
            txtPrevComments.Text = cleanTextLight(Data.Comments).Replace("|", " " & vbCrLf & " ")
        Else
            trPreviousComments.Visible = False
        End If
        Dim sS As String = String.Empty
        Dim sTo As String = String.Empty
        Dim sOthers As String = String.Empty
        For i As Integer = 1 To Pieces(Data.To, ",")
            sS = Piece(Data.To, ",", i)
            If sS <> String.Empty Then
                li = ddSendTo.Items.FindByText(sS)
                If li IsNot Nothing Then
                    If sTo = String.Empty Then
                        sTo = sS
                    Else
                        sTo &= "," & sS
                    End If
                Else
                    If sOthers = String.Empty Then
                        sOthers = sS
                    Else
                        sOthers &= "," & sS
                    End If
                End If
            End If
        Next
        Dim sAll As String = GetAllSelectedString()

        If sTo = sAll Then
            ddSendTo.SelectedItemsString = sAll
        Else
            ddSendTo.SelectedItemsString = sTo
        End If
        If sOthers <> String.Empty Then
            If ddSendTo.SelectedItemsString = String.Empty Then
                ddSendTo.SelectedItemsString = sOthers
            Else
                ddSendTo.SelectedItemsString &= "," & sOthers
            End If
        End If
        txtLookup.Text = ddSendTo.SelectedItemsString
    End Sub
    Private Function GetAllSelectedString() As String
        Dim ret As String = String.Empty
        For i As Integer = 0 To ddSendTo.Items.Count - 1
            If ddSendTo.Items(i).Text <> "All" Then
                If ret = String.Empty Then
                    ret = ddSendTo.Items(i).Text
                Else
                    ret &= "," & ddSendTo.Items(i).Text
                End If
            End If
        Next
        Return ret
    End Function
    Public Sub Show(Caption As String, TickData As TicketData, Optional TickEntryMode As Mode = Mode.Add, Optional OKButtonCaption As String = "OK")
        Dim li As New ListItem("All", "All")
        Dim sS As String = String.Empty
        Dim sTo As String = String.Empty

        lblHeader.Text = Caption
        lblAttached.Text = "No file selected."
        ckNoEmail.Checked = False
        Data = TickData
        User = Data.From
        EntryMode = TickEntryMode
        OKCaption = OKButtonCaption
        divOtherStatus.Style.Item("display") = "none"
        txtOtherStatus.Text = String.Empty
        trPreviousComments.Visible = False
        txtPrevComments.Text = String.Empty

        If ddSendTo.Items.FindByText("All") Is Nothing AndAlso ddSendTo.Items.Count > 0 Then
            ddSendTo.Items.Insert(0, li)
        End If
        lblTicketNoVal.Text = Data.ID

            tdSendto.Style("padding-bottom") = "0px"
            divDDSendTo.Style("display") = "none"
            divLUSendto.Style("display") = ""
        txtLookup.Text = String.Empty
        If EntryMode = Mode.Add Then
            divLblBreak.Style.Item("display") = "none"
            divNoEmail.Style.Item("display") = "none"
            lblDateTimeVal.Text = Format(Now, "M/d/yyyy h:mm:s tt")
            ddFrom.Text = Data.From
            txtDescription.Text = String.Empty
            ddStatus.SelectedIndex = 0
            'ddVersion.Text = Session("Version")
            txtComments.Text = String.Empty
            If Not Data.Comments Is Nothing AndAlso Data.Comments.ToString.Trim.Length > 0 Then
                txtComments.Text = Data.Comments
            End If
            If Not Data.Description Is Nothing AndAlso Data.Description.ToString.Trim.Length > 0 Then
                txtDescription.Text = Data.Description
            End If
            txtLookup.Text = Session("logon")
        Else
            If Not IsPostBack AndAlso Session("attachrep") = "yes" AndAlso Session("myfile").ToString.Trim <> "" Then
                txtComments.Text = txtComments.Text & " | File attached: SAVEDFILES/" & Session("myfile") & " ."
            End If
            LoadTicketData()
        End If
        'SetControlFocus("ddFrom")
        ddFrom.Focus()
        OldData = CType(CopyObject(Data), TicketData)
        popDlg.Show()
    End Sub
#End Region

#Region "Event Handlers"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Private Sub TicketDlg_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        udpTicketDlg.Update()
        If TypeOf Parent Is UpdatePanel Then
            Dim udpContainer As UpdatePanel = CType(Parent, UpdatePanel)
            udpContainer.Update()
        End If
    End Sub

    Private Sub TicketDlg_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim ctlID As String = Me.ClientID & "_"
        btnBrowse.OnClientClick = "clickFileUpload('" & ctlID & "');return false;"
        btnLookup.OnClientClick = "showAll('" & ctlID & "');return false;"
        btnClose.OnClientClick = "btnCloseClicked('" & ctlID & "');return false;"
        btnFind.OnClientClick = "DoSearch('" & ctlID & "','txtSearch');return false;"
        btnSubmit.OnClientClick = "btnSubmitClicked('" & ctlID & "');return false;"

        txtLookup.Attributes.Add("onchange", "DoSearch('" & ctlID & "','txtLookup');")
        txtLookup.Attributes.Add("onkeydown", "txtLookupKeyDown(event,'" & ctlID & "');")
        ddStatus.Attributes.Add("onchange", "handleStatusChange('" & ctlID & "');")
        FileUpload.Attributes.Add("onchange", "getAttachedFile('" & ctlID & "');")
        spnClose.Attributes.Add("onclick", "closePopup('" & ctlID & "');")
        lstItems.Attributes.Add("onclick", "ChecklistIndexChanged(event,'" & ctlID & "');")
        txtSearch.Attributes.Add("onkeydown", "txtSearchKeyDown(event,'" & ctlID & "');")
        ddVersion.Items(0).Text = Session("Version")
        ddVersion.Items(0).Value = Session("Version")
        ddFrom.Text = ""
        ddVersion.Text = Session("Version")
        If Session("FromSite") = "yes" Then
            ddStatus.Text = "problem"
            ddStatus.Enabled = False
        ElseIf Session("tasklist") = "yes" Then
            Dim i As Integer
            Dim er As String = String.Empty
            Dim dv As DataView = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("UnitName").ToString & "' AND [User]='" & Session("logon") & "' ORDER BY Prop1,FldOrder", er)
            If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count < 1 Then
            Else
                'fill out version and status dropdowns
                ddVersion.Items.Clear()
                ddStatus.Items.Clear()
                If Session("Version").Trim <> "" Then ddVersion.Items.Add(Session("Version"))
                ddVersion.Text = Session("Version")
                For i = 0 To dv.Table.Rows.Count - 1
                    If dv.Table.Rows(i)("Prop1") = "version" Then
                        ddVersion.Items.Add(dv.Table.Rows(i)("FldText"))
                    ElseIf dv.Table.Rows(i)("Prop1") = "status" Then
                        ddStatus.Items.Add(dv.Table.Rows(i)("FldText"))
                    End If
                Next
            End If
        End If

    End Sub

    Private Sub btnAttach_Click(sender As Object, e As EventArgs) Handles btnAttach.Click
        Dim dir As String = String.Empty
        Dim FileName As String = String.Empty
        Dim FileNameFix As String = String.Empty
        Dim FixPoint As String = String.Empty
        Dim contentType As String = String.Empty
        Dim contentLength As Integer = 0
        Dim file As String = String.Empty

        Dim PostedFile As HttpPostedFile = Nothing
        If FileUpload.HasFile Then
            PostedFile = FileUpload.PostedFile
            FileName = Now.ToString & "_" & PostedFile.FileName
            FileNameFix = Replace(FileName, " ", "_")
            FileNameFix = Replace(FileNameFix, ",", "_")
            FileNameFix = Replace(FileNameFix, "#", "_")
            FileNameFix = Replace(FileNameFix, ":", "-")
            FileNameFix = Replace(FileNameFix, "/", "-")
            FixPoint = Replace(Left(FileNameFix, Len(FileNameFix) - 5), ".", "_")
            FileNameFix = FixPoint & Right(FileNameFix, 5)
            contentType = PostedFile.ContentType
            contentLength = PostedFile.ContentLength
            dir = Request.PhysicalApplicationPath & "SAVEDFILES/"
            file = dir & FileNameFix
            PostedFile.SaveAs(file)
            Dim msg As String = FileName & " uploaded "
            msg &= "<br/>Content Type: " & contentType
            msg &= "<br/>Content Length: " & contentLength.ToString & "K"

            lblAttached.Text = "No file selected."
            txtComments.Text = txtComments.Text & " | File attached: SAVEDFILES/" & FileNameFix & " ."
            If Data.Attachment = String.Empty Then
                Data.Attachment = file
            Else
                Data.Attachment = Data.Attachment & "," & file
            End If
            MessageBox.Show(msg, "File Upload", "FileUpload", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
        Else
            MessageBox.Show("No file selected.", "File Upload", "NoFile", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
            Return
        End If
    End Sub

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "NoFile" Then
            popDlg.Show()
            btnBrowse.Focus()
        ElseIf e.Tag = "FileUpload" Then
            popDlg.Show()
            btnOK.Focus()
        ElseIf e.Tag = "NoStatus" Then
            popDlg.Show()
            If divOtherStatus.Style.Item("display") = "none" Then
                ddStatus.Focus()
            Else
                txtOtherStatus.Focus()
            End If
        ElseIf e.Tag = "NoDescription" Then
            popDlg.Show()
            txtDescription.Focus()
        ElseIf e.Tag = "NoAssignedUsers" Then
            popDlg.Show()
            ddSendTo.Text = "Please select..."
            ddSendTo.Focus()

        End If
    End Sub
    Private Function DeleteFromList(sList As String, ToDelete As String) As String
        Dim sRet As String = String.Empty
        Dim sS As String = String.Empty
        For i As Integer = 1 To Pieces(sList, ",")
            sS = Piece(sList, ",", i)
            If sS <> String.Empty AndAlso sS <> ToDelete Then
                If sRet = String.Empty Then
                    sRet = sS
                Else
                    sRet &= "," & sS
                End If
            End If
        Next
        Return sRet
    End Function
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Dim status As String = ddStatus.SelectedItem.Text
        If status = "other" Then
            status = txtOtherStatus.Text
            divOtherStatus.Style.Item("display") = ""
            If status = String.Empty Then
                MessageBox.Show("Other status was not entered.", "Ticket Entry Error", "NoStatus", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Return
            End If
        Else
            divOtherStatus.Style.Item("display") = "none"
        End If

        If txtDescription.Text = String.Empty Then
            MessageBox.Show("Description was not entered.", "Ticket Entry Error", "NoDescription", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        End If
        If status = String.Empty Then
            MessageBox.Show("Status was not chosen.", "Ticket Entry Error", "NoStatus", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        End If
            If txtLookup.Text = String.Empty Then
                MessageBox.Show("Assigned User(s) have not been chosen.", "Ticket Entry Error", "NoAssignedUsers", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                Return
            End If
        Dim comment As String = String.Empty

        Data.ID = lblTicketNoVal.Text
        Data.DateTime = lblDateTimeVal.Text
        Data.From = ddFrom.Text
        Data.Version = ddVersion.SelectedItem.Text
        Data.Description = cleanTextLight(txtDescription.Text)
        Data.Status = status
        Data.To = txtLookup.Text

        Data.Deadline = Request("txtDeadline").ToString
        comment = cleanTextLight(txtComments.Text)
        'If EntryMode = Mode.Edit Then
        If comment <> String.Empty Then
            comment = Session("logon") & " (" & Format(Now, "M/d/yyyy HH:mm:ss") & "): " & comment
            If Data.Comments <> String.Empty AndAlso Not comment.Contains(Data.Comments) Then
                comment &= "| " & Data.Comments
            End If
        Else
            comment = Data.Comments
        End If
        If ckNoEmail.Checked Then
            Data.To = DeleteFromList(Data.To, User)
            comment &= "| Delete me from the email list for this ticket."
        End If
        'End If

        Data.Comments = comment
        OnTicketDialogResulted(New TicketDlgEventArgs(TicketDialogResult.OK, Data, OldData, EntryMode))
    End Sub


    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        OnTicketDialogResulted(New TicketDlgEventArgs(TicketDialogResult.Cancel, Data, OldData, EntryMode))
    End Sub


#End Region
End Class
