Imports System.ComponentModel
Partial Class Controls_CustomizeLogicDlg
    Inherits System.Web.UI.UserControl
#Region "enums"
    Public Enum CustomizeDialogResult
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
    End Enum
#End Region

#Region "Classes"
    Public Class CustomLogicDlgEventArgs
        Inherits System.EventArgs

        Private mJsonData As String
        Private mResult As CustomizeDialogResult
        Private mDeletedData As String

        Public Sub New(MsgResult As CustomizeDialogResult, strJson As String, strDeleted As String)
            mResult = MsgResult
            mJsonData = strJson
            mDeletedData = strDeleted
        End Sub
        Public ReadOnly Property Result As CustomizeDialogResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property JsonData As String
            Get
                Return mJsonData
            End Get
        End Property
        Public ReadOnly Property DeletedData As String
            Get
                Return mDeletedData
            End Get
        End Property
    End Class
#End Region

#Region "Event Definitions"
    Public Delegate Sub CustomLogicDlgHandler(sender As Object, e As CustomLogicDlgEventArgs)
    Public Event CustomLogicDialogResulted As CustomLogicDlgHandler
    Protected Overridable Sub OnCustomLogicDialogResulted(e As CustomLogicDlgEventArgs)
        RaiseEvent CustomLogicDialogResulted(Me, e)
    End Sub
#End Region

#Region "Properties"
    <Browsable(False)>
    Public Property JsonData As String
        Get
            If ViewState("JsonData") Is Nothing Then
                ViewState("JsonData") = String.Empty
            End If
            'Return CType(ViewState("Data"), ConditionData)
            Return ViewState("JsonData").ToString
        End Get
        Set(value As String)
            ViewState("JsonData") = value
            hdnJson.Value = value
        End Set
    End Property

    <Browsable(False)>
    Public Property GroupCount As Integer
        Get
            If ViewState("GroupCount") Is Nothing Then ViewState("GroupCount") = CInt(hdnGroupCount.Value)
            Return CInt(ViewState("GroupCount"))
        End Get
        Set(value As Integer)
            hdnGroupCount.Value = value.ToString
            ViewState("GroupCount") = value
        End Set
    End Property

    <Browsable(False)>
    Public Property ConditionCount As Integer
        Get
            If ViewState("ConditionCount") Is Nothing Then ViewState("ConditionCount") = CInt(hdnConditionCount.Value)
            Return CInt(ViewState("ConditionCount"))
        End Get
        Set(value As Integer)
            hdnConditionCount.Value = value.ToString
            ViewState("ConditionCount") = value
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
#End Region

#Region "Methods"
    Private Sub WindowOnLoadScript(FocusId As String, sJson As String)
        Dim prefix As String = Me.ClientID & "_"
        Dim id As String = prefix & FocusId
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Page.ClientScript
        With sb
            .Append("<script language='JavaScript'>")
            .Append("function LoadTreeView() {")
            .Append("  prefix = '" & prefix & "';")
            .Append("  TreeView1.SetContainer('divTreeView');")
            .Append("  populateTreeView('" & sJson & "');")
            .Append("  adjustButtons();")
            .Append("  ctl = document.getElementById('" & id & "');")
            .Append("  if (ctl != null) {")
            .Append("    window.setTimeout( 'ctl.focus()',1);")
            .Append("  }")
            .Append("}")
            .Append("window.onload = LoadTreeView;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "WindowOnLoad", sb.ToString)
    End Sub
    Public Sub Show(Caption As String, JsonDta As String, Optional OKButtonCaption As String = "OK")
        lblHeader.Text = Caption
        JsonData = JsonDta
        OKCaption = OKButtonCaption
        popDlg.Show()
        btnCancel.Focus()
        'WindowOnLoadScript("btnCancel", JsonData)
    End Sub
#End Region

#Region "Event Handlers"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Private Sub CustomizeLogicDlg_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        udpCustDlg.Update()
        If TypeOf Parent Is UpdatePanel Then
            Dim udpContainer As UpdatePanel = CType(Parent, UpdatePanel)
            udpContainer.Update()
        End If
    End Sub

    Private Sub CustomizeLogicDlg_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim ctlID As String = Me.ClientID & "_"
        btnUp.OnClientClick = "moveUp();return false;"
        btnDown.OnClientClick = "moveDown();return false;"
        btnAddGroup.OnClientClick = "addGroup();return false;"
        btnUngroup.OnClientClick = "deleteGroup();return false;"
        btnOK.OnClientClick = " return getTreeData();"
        btnCancel.OnClientClick = "isLoaded=false;"
        rbAnd.Attributes.Add("onchange", "operatorChanged('rbAnd');")
        rbOr.Attributes.Add("onchange", "operatorChanged('rbOr');")
        btnCancel.Attributes.Add("onfocus", "loadTreeView('" & ctlID & "');")
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        JsonData = hdnJson.Value
        Dim DelData As String = hdnDeletedData.Value
        Dim arg As New CustomLogicDlgEventArgs(CustomizeDialogResult.OK, JsonData, DelData)
        OnCustomLogicDialogResulted(arg)
    End Sub

#End Region
End Class

