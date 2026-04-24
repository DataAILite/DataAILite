Imports System.ComponentModel
Partial Class Controls_TextboxDlg
    Inherits System.Web.UI.UserControl

#Region "Enums"
    Public Enum TextboxDlgResult
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
    <Serializable> Public Class TextboxData
        Public Property Prompt As String
        Public Property Text As String
        Public Property TextVisible As Boolean
        Public Property TextEnabled As Boolean

        Public Sub New(sPrompt As String, sText As String, bVisible As Boolean, bEnabled As Boolean)
            Prompt = sPrompt
            Text = sText
            TextVisible = bVisible
            TextEnabled = bEnabled
        End Sub
    End Class
    <Serializable> Public Class TextboxDlgData
        Public TextboxData1 As TextboxData
        Public TextboxData2 As TextboxData
        Public TextboxData3 As TextboxData
        Public Tag As String
    End Class
    Public Class TextboxDlgEventArgs
        Inherits System.EventArgs

        Private mDialogData As TextboxDlgData
        Private mResult As TextboxDlgResult
        Private mTag As String
        Public Sub New(MsgResult As TextboxDlgResult, DlgData As TextboxDlgData, Tag As String)
            mResult = MsgResult
            mDialogData = DlgData
            mTag = Tag
        End Sub
        Public ReadOnly Property Result As TextboxDlgResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property DialogData As TextboxDlgData
            Get
                Return mDialogData
            End Get
        End Property
        Public ReadOnly Property Tag As String
            Get
                Return mTag
            End Get
        End Property
    End Class
#End Region

#Region "Properties"
    <Browsable(False)>
    Public Property Data As TextboxDlgData
        Get
            If ViewState("Data") Is Nothing Then
                ViewState("Data") = New TextboxDlgData()
            End If
            Return CType(ViewState("Data"), TextboxDlgData)
        End Get
        Set(value As TextboxDlgData)
            ViewState("Data") = value
        End Set
    End Property
    Public Property ResultTag As String
        Get
            If ViewState("ResultTag") Is Nothing Then
                ViewState("ResultTag") = ""
            End If
            Return Convert.ToString(ViewState("ResultTag"))
        End Get
        Set(value As String)
            ViewState("ResultTag") = value
        End Set
    End Property
    Public Property FontName As String
        Get
            If ViewState("FontName") Is Nothing OrElse ViewState("FontName").ToString = "" Then
                ViewState("FontName") = "Arial"
            End If
            Return ViewState("FontName").ToString
        End Get
        Set(value As String)
            If value = String.Empty Then
                pnlHeader.Font.Name = "Arial"
                lblMain.Font.Name = "Arial"
                lblText1.Font.Name = "Arial"
                txtText1.Font.Name = "Arial"
                lblText2.Font.Name = "Arial"
                txtText2.Font.Name = "Arial"
                lblText3.Font.Name = "Arial"
                txtText3.Font.Name = "Arial"
                btnOK.Font.Name = "Arial"
                btnCancel.Font.Name = "Arial"
                ViewState("FontName") = "Arial"
            ElseIf divInput.Style.Item("font-name") <> value Then
                pnlHeader.Font.Name = value
                lblMain.Font.Name = value
                lblText1.Font.Name = value
                txtText1.Font.Name = value
                lblText2.Font.Name = value
                txtText2.Font.Name = value
                lblText3.Font.Name = value
                txtText3.Font.Name = value
                btnOK.Font.Name = value
                btnCancel.Font.Name = value
                ViewState("FontName") = value
            End If
        End Set
    End Property
    Public Property FontSize As FontUnit
        Get
            If ViewState("FontSize") Is Nothing Then
                ViewState("FontSize") = lblText1.Font.Size
            End If
            Return CType(ViewState("FontSize"), FontUnit)
        End Get
        Set(value As FontUnit)
            If value <> lblText1.Font.Size Then
                ViewState("FontSize") = value
                lblText1.Font.Size = value
                txtText1.Font.Size = value
                lblText2.Font.Size = value
                txtText2.Font.Size = value
                lblText3.Font.Size = value
                txtText3.Font.Size = value
            End If
        End Set
    End Property

    Public Property MainTitleFontSize As FontUnit
        Get
            If ViewState("MainTitleFontSize") Is Nothing Then
                ViewState("MainTitleFontSize") = lblMain.Font.Size
            End If
            Return CType(ViewState("MainTitleFontSize"), FontUnit)
        End Get
        Set(value As FontUnit)
            If value <> lblMain.Font.Size Then
                ViewState("MainTitleFontSize") = value
                lblMain.Font.Size = value
            End If
        End Set
    End Property

    Public Property MainTitleText As String
        Get
            If ViewState("MainTitleText") Is Nothing OrElse ViewState("MainTitleText").ToString = String.Empty Then
                ViewState("MainTitleText") = lblMain.Text
            End If
            Return ViewState("MainTitleText").ToString
        End Get
        Set(value As String)
            If value <> lblMain.Text Then
                ViewState("MainTitleText") = value
                lblMain.Text = value
            End If
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

#Region "Event Definitions"
    Public Delegate Sub TextboxDlgEventHandler(sender As Object, e As TextboxDlgEventArgs)
    Public Event TextboxDlgResulted As TextboxDlgEventHandler
    Protected Overridable Sub OnTextboxDlgResulted(e As TextboxDlgEventArgs)
        RaiseEvent TextboxDlgResulted(Me, e)
    End Sub
#End Region

#Region "Event Handlers"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub TextboxDlg_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        udpTextboxDlg.Update()
        If TypeOf Parent Is UpdatePanel Then
            Dim udpContainer As UpdatePanel = CType(Parent, UpdatePanel)
            udpContainer.Update()
        End If
    End Sub
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Dim msg As String = String.Empty
        If Data.TextboxData1.TextVisible Then
            If txtText1.Text = String.Empty Then
                msg = Data.TextboxData1.Prompt.Replace(":", "")
                msg &= " was not entered."
                MessageBox.Show(msg, "Entry Error", "Error1", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Return
            End If
        End If
        If Data.TextboxData2 IsNot Nothing AndAlso Data.TextboxData2.TextVisible Then
            If txtText2.Text = String.Empty Then
                msg = Data.TextboxData2.Prompt.Replace(":", "")
                msg &= " was not entered."
                MessageBox.Show(msg, "Entry Error", "Error2", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Return
            End If
        End If
        If Data.TextboxData3 IsNot Nothing AndAlso Data.TextboxData3.TextVisible Then
            If txtText3.Text = String.Empty Then
                msg = Data.TextboxData3.Prompt.Replace(":", "")
                msg &= " was not entered."
                MessageBox.Show(msg, "Entry Error", "Error3", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error, Controls_Msgbox.MessageDefaultButton.OK)
                Return
            End If
        End If
        If Data.TextboxData1.TextVisible Then
            Data.TextboxData1.Text = txtText1.Text
        End If
        If Data.TextboxData2 IsNot Nothing AndAlso Data.TextboxData2.TextVisible Then
            Data.TextboxData2.Text = txtText2.Text
        End If
        If Data.TextboxData3 IsNot Nothing AndAlso Data.TextboxData3.TextVisible Then
            Data.TextboxData3.Text = txtText3.Text
        End If
        Dim arg As New TextboxDlgEventArgs(TextboxDlgResult.OK, Data, ResultTag)
        OnTextboxDlgResulted(arg)
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        OnTextboxDlgResulted(New TextboxDlgEventArgs(TextboxDlgResult.Cancel, Nothing, ""))
    End Sub

#End Region

#Region "Methods"
    Private Sub SetControlFocus(ctlId As String)
        Dim id As String = Me.ClientID & "_" & ctlId
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Page.ClientScript
        With sb
            .Append("<script language='JavaScript'>")
            .Append("function SetControlFocus()")
            .Append("{")
            .Append("  var ctl = document.getElementById('" & id & "');")
            .Append("  if (ctl != null)")
            .Append("    ctl.focus();")
            .Append("}")
            .Append("window.onload = SetControlFocus;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "SetControlFocus", sb.ToString)
    End Sub
    Public Sub Show(Caption As String, MainText As String, Tag As String, DlgData As TextboxDlgData, Optional OKButtonCaption As String = "OK")
        lblHeader.Text = Caption
        lblMain.Text = MainText
        ResultTag = Tag
        'If DlgData Is Nothing Then
        '    MessageBox.Show("No data is defined.", "Show Dialog", "", Msgbox.Buttons.OK, Msgbox.MessageIcon.Error, Msgbox.MessageDefaultButton.OK)
        '    Return
        'End If
        Data = DlgData
        If Data.TextboxData1 IsNot Nothing Then
            If Data.TextboxData1.TextVisible Then
                trText1.Visible = True
                lblText1.Text = Data.TextboxData1.Prompt
                txtText1.Text = Data.TextboxData1.Text
                lblText1.Enabled = Data.TextboxData1.TextEnabled
                txtText1.Enabled = Data.TextboxData1.TextEnabled
            Else
                trText1.Visible = False
            End If
        Else
            trText1.Visible = False
        End If
        If Data.TextboxData2 IsNot Nothing Then
            If Data.TextboxData2.TextVisible Then
                trText2.Visible = True
                lblText2.Text = Data.TextboxData2.Prompt
                txtText2.Text = Data.TextboxData2.Text
                lblText2.Enabled = Data.TextboxData2.TextEnabled
                txtText2.Enabled = Data.TextboxData2.TextEnabled
            Else
                trText2.Visible = False
            End If
        Else
            trText2.Visible = False
        End If
        If Data.TextboxData3 IsNot Nothing Then
            If Data.TextboxData3.TextVisible Then
                trText3.Visible = True
                lblText3.Text = Data.TextboxData3.Prompt
                txtText3.Text = Data.TextboxData3.Text
                lblText3.Enabled = Data.TextboxData3.TextEnabled
                txtText3.Enabled = Data.TextboxData3.TextEnabled
            Else
                trText3.Visible = False
            End If
        Else
            trText3.Visible = False
        End If
        OKCaption = OKButtonCaption
        popDlg.Show()
        If txtText1.Enabled Then
            SetControlFocus("txtText1")
        ElseIf txtText2.Enabled Then
            SetControlFocus("txtText2")
        Else
            SetControlFocus("txtText3")
        End If

    End Sub

    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "Error1" Then
            popDlg.Show()
            txtText1.Focus()
        ElseIf e.Tag = "Error2" Then
            popDlg.Show()
            txtText2.Focus()
        ElseIf e.Tag = "Error3" Then
            popDlg.Show()
            txtText3.Focus()
        End If
    End Sub

    'Private Sub txtText_TextChanged(sender As Object, e As EventArgs) Handles txtText1.TextChanged, txtText2.TextChanged, txtText3.TextChanged
    '    Dim tb As TextBox = CType(sender, TextBox)
    '    Dim text As String = tb.Text
    '    If text <> String.Empty Then
    '        btnOK_Click(Nothing, Nothing)
    '    End If
    'End Sub

#End Region

End Class
