Partial Class Controls_Msgbox
    Inherits System.Web.UI.UserControl
    Private Messages As New List(Of Message)

#Region "enums"
    Public Enum MessageDefaultButton
        OK = 0
        PostOK = 1
        Cancel = 2
        Yes = 3
        No = 4
        Retry = 5
        Abort = 6
        Ignore = 7
        Other1 = 8
        Other2 = 9
        None = 10
    End Enum
    Public Enum MessageIcon
        [Error] = 0
        Warning = 1
        Information = 2
        None = 3
        Question = 4
        [Stop] = 5
    End Enum
    Public Enum Buttons
        OK = 0
        OKCancel = 1
        YesNo = 2
        RetryCancel = 3
        YesNoCancel = 4
        AbortRetryIgnore = 5
        Other = 6
        OtherCancel = 7
        OtherOther = 8
        OtherOtherCancel = 9
    End Enum
    Public Enum MessageResult
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


#Region "Properties"
    Public Property MessageTag As String
        Get
            If ViewState("MessageTag") Is Nothing Then
                ViewState("MessageTag") = ""
            End If
            Return Convert.ToString(ViewState("MessageTag"))
        End Get
        Set(value As String)
            ViewState("MessageTag") = value
        End Set
    End Property

    Public Property OtherButtonText1 As String
        Get
            If ViewState("OtherButtonText1") Is Nothing Then
                ViewState("OtherButtonText1") = ""
            End If
            Return Convert.ToString(ViewState("OtherButtonText1"))
        End Get
        Set(value As String)
            ViewState("OtherButtonText1") = value
            btnPostOther1.Text = value
        End Set
    End Property
    Public Property OtherButtonText2 As String
        Get
            If ViewState("OtherButtonText2") Is Nothing Then
                ViewState("OtherButtonText2") = ""
            End If
            Return Convert.ToString(ViewState("OtherButtonText2"))
        End Get
        Set(value As String)
            ViewState("OtherButtonText2") = value
            btnPostOther2.Text = value
        End Set
    End Property
    Public Property DefaultButton As MessageDefaultButton
        Get
            If ViewState("DefaultButton") Is Nothing Then
                ViewState("DefaultButton") = MessageDefaultButton.None
            End If
            Return CType(ViewState("DefaultButton"), MessageDefaultButton)

        End Get
        Set(value As MessageDefaultButton)
            ViewState("DefaultButton") = value
        End Set
    End Property
    Protected ReadOnly Property MessageCount As Integer
        Get
            Return Messages.Count
        End Get
    End Property
#End Region

#Region "Classes"
    Public Class MsgBoxEventArgs
        Inherits System.EventArgs

        Private mTag As String = ""
        Private mResult As MessageResult = MessageResult.None

        Public Sub New(MsgResult As MessageResult, MsgTag As String)
            mResult = MsgResult
            mTag = MsgTag
        End Sub
        Public ReadOnly Property Result As MessageResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property Tag As String
            Get
                Return mTag
            End Get
        End Property

    End Class
    Public Class Message
        Private mMessageText As String = ""
        Private mIcon As MessageIcon = MessageIcon.None

        Public Sub New(msg As String, msgIcon As MessageIcon)
            mMessageText = msg
            mIcon = msgIcon
        End Sub
        Public Property Icon As MessageIcon
            Get
                Return mIcon
            End Get
            Set(value As MessageIcon)
                mIcon = value
            End Set
        End Property

        Public Property MessageText As String
            Get
                Return mMessageText
            End Get
            Set(value As String)
                mMessageText = value
            End Set
        End Property

    End Class

#End Region

#Region "Methods"
    Private Sub SetDefaultFocus(ctlId As String)
        Dim id As String = Me.ClientID & "_" & ctlId
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Page.ClientScript
        With sb
            .Append("<script language='JavaScript'>")
            .Append("function SetDefaultFocus()")
            .Append("{")
            .Append("  var btn = document.getElementById('" & id & "');")
            .Append(" if (btn!=null)")
            .Append("    btn.focus();")
            .Append("}")
            .Append("window.onload = SetDefaultFocus;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "SetDefaultFocus", sb.ToString)
    End Sub
    Private Sub MakeButtonVisible(Button As Buttons, Optional Postback As Boolean = True)

        btnOK.Visible = False
        btnPostOK.Visible = False
        btnPostCancel.Visible = False
        btnPostYes.Visible = False
        btnPostNo.Visible = False
        btnPostRetry.Visible = False
        btnPostAbort.Visible = False
        btnPostIgnore.Visible = False
        btnPostOther1.Visible = False
        btnPostOther2.Visible = False

        Select Case Button
            Case Buttons.OK
                If Postback Then
                    btnPostOK.Visible = True
                Else
                    btnOK.Visible = True
                End If
            Case Buttons.OKCancel
                btnPostOK.Visible = True
                btnPostCancel.Visible = True
            Case Buttons.YesNo
                btnPostYes.Visible = True
                btnPostNo.Visible = True
            Case Buttons.YesNoCancel
                btnPostYes.Visible = True
                btnPostNo.Visible = True
                btnPostCancel.Visible = True
            Case Buttons.RetryCancel
                btnPostRetry.Visible = True
                btnPostCancel.Visible = True
            Case Buttons.AbortRetryIgnore
                btnPostAbort.Visible = True
                btnPostRetry.Visible = True
                btnPostIgnore.Visible = True
            Case Buttons.Other
                btnPostOther1.Visible = True
            Case Buttons.OtherCancel
                btnPostOther1.Visible = True
                btnPostCancel.Visible = True
            Case Buttons.OtherOther
                btnPostOther1.Visible = True
                btnPostOther2.Visible = True
            Case Buttons.OtherOtherCancel
                btnPostOther1.Visible = True
                btnPostOther2.Visible = True
                btnPostCancel.Visible = True
        End Select
    End Sub
    Public Sub SetDefaultButtonFocus(btnDefault As MessageDefaultButton)
        Select Case btnDefault
            Case MessageDefaultButton.OK
                'SetDefaultFocus("btnOK")
                btnOK.Focus()
            Case MessageDefaultButton.PostOK
                btnPostOK.Focus()
                'SetDefaultFocus("btnPostOK")
            Case MessageDefaultButton.Cancel
                btnPostCancel.Focus()
                'SetDefaultFocus("btnPostCancel")
            Case MessageDefaultButton.Yes
                btnPostYes.Focus()
                'SetDefaultFocus("btnPostYes")
            Case MessageDefaultButton.No
                btnPostNo.Focus()
                'SetDefaultFocus("btnPostNo")
            Case MessageDefaultButton.Retry
                btnPostRetry.Focus()
                'SetDefaultFocus("btnPostRetry")
            Case MessageDefaultButton.Abort
                btnPostRetry.Focus()
                'SetDefaultFocus("btnPostAbort")
            Case MessageDefaultButton.Ignore
                btnPostIgnore.Focus()
                'SetDefaultFocus("btnPostIgnore")
            Case MessageDefaultButton.Other1
                btnPostOther1.Focus()
                'SetDefaultFocus("btnPostOther1")
            Case MessageDefaultButton.Other2
                btnPostOther2.Focus()
                'SetDefaultFocus("btnPostOther2")
        End Select
    End Sub
    Public Sub Show(msg As String)
        Messages.Clear()
        Messages.Add(New Message(msg, MessageIcon.None))
        MakeButtonVisible(Buttons.OK, False)
        SetDefaultButtonFocus(MessageDefaultButton.OK)
    End Sub
    Public Sub Show(msg As String, caption As String)
        lblHeader.Text = caption
        Messages.Clear()
        Messages.Add(New Message(msg, MessageIcon.None))
        MakeButtonVisible(Buttons.OK, False)
        SetDefaultButtonFocus(MessageDefaultButton.OK)
    End Sub
    Public Sub Show(msg As String, caption As String, Tag As String, Buttons As Buttons)
        lblHeader.Text = caption
        Messages.Clear()
        Messages.Add(New Message(msg, MessageIcon.None))
        MakeButtonVisible(Buttons)
        MessageTag = Tag
        SetDefaultButtonFocus(DefaultButton)
    End Sub
    Public Sub Show(msg As String, caption As String, Tag As String, Buttons As Buttons, Icon As MessageIcon)
        lblHeader.Text = caption
        Messages.Clear()
        Messages.Add(New Message(msg, Icon))
        MakeButtonVisible(Buttons)
        MessageTag = Tag
        SetDefaultButtonFocus(DefaultButton)
    End Sub
    Public Sub Show(msg As String, caption As String, Tag As String, Buttons As Buttons, Icon As MessageIcon, DefaultBtn As MessageDefaultButton)
        lblHeader.Text = caption
        Messages.Clear()
        Messages.Add(New Message(msg, Icon))
        MakeButtonVisible(Buttons)
        MessageTag = Tag
        'DefaultButton = DefaultBtn
        SetDefaultButtonFocus(DefaultBtn)
    End Sub
#End Region

#Region "Event Definitions"
    Public Delegate Sub MsgBoxEventHandler(sender As Object, e As MsgBoxEventArgs)
    Public Event MessageResulted As MsgBoxEventHandler
    Protected Overridable Sub OnMessageResulted(e As MsgBoxEventArgs)
        RaiseEvent MessageResulted(Me, e)
    End Sub
#End Region

#Region "Event Handlers"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Protected Sub btnPostOK_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.OK, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostCancel_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Cancel, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostYes_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Yes, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostNo_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.No, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostRetry_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Retry, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostAbort_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Abort, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostIgnore_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Ignore, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostOther1_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Other1, MessageTag))
        MessageTag = ""
    End Sub
    Protected Sub btnPostOther2_Click(sender As Object, e As EventArgs)
        OnMessageResulted(New MsgBoxEventArgs(MessageResult.Other2, MessageTag))
        MessageTag = ""
    End Sub
    Private Sub Msgbox_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If btnOK.Visible Then
            popMsg.OkControlID = "btnOK"
        Else
            popMsg.OkControlID = "btnD2"
        End If
        If Messages.Count > 0 Then
            grdMsg.DataSource = Messages
            grdMsg.DataBind()
            popMsg.Show()
            udpMsgbox.Update()
        Else
            grdMsg.DataBind()
            udpMsgbox.Update()
        End If
        If TypeOf Parent Is UpdatePanel Then
            Dim udpContainer As UpdatePanel = CType(Parent, UpdatePanel)
            udpContainer.Update()
        End If
    End Sub


#End Region




End Class
