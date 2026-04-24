Imports System.ComponentModel
Partial Class Controls_CheckboxDialog
    Inherits System.Web.UI.UserControl
#Region "Enums"
    Public Enum DialogResult
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
    Public Class DlgBoxEventArgs
        Inherits System.EventArgs

        Private mSelectedItems As ListItemCollection = New ListItemCollection()
        Private mResult As DialogResult = DialogResult.None

        Public Sub New(MsgResult As DialogResult, SelItems As ListItemCollection)
            mResult = MsgResult
            mSelectedItems = SelItems
        End Sub
        Public ReadOnly Property Result As DialogResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property SelectedItems As ListItemCollection
            Get
                Return mSelectedItems
            End Get
        End Property

    End Class
#End Region

#Region "Properties"
    <Browsable(False)>
    Property Items As ListItemCollection
        Get
            Return Checklist.Items
        End Get
        Set(value As ListItemCollection)
            Checklist.Items.Clear()
            If Not value Is Nothing AndAlso value.Count > 0 Then
                For i As Integer = 0 To value.Count - 1
                    Checklist.Items.Add(value(i))
                Next
            End If
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property SelectedItems As ListItemCollection
        Get
            Dim Selected As New ListItemCollection
            If Not Checklist.Items Is Nothing And Checklist.Items.Count > 0 Then
                For i As Integer = 0 To Checklist.Items.Count - 1
                    If Checklist.Items(i).Selected Then
                        Selected.Add(Checklist.Items(i))
                    End If
                Next
            End If
            Return Selected
        End Get
    End Property

    Public Property FontName As String
        Get
            If ViewState("FontName") Is Nothing Then
                ViewState("FontName") = "Tahoma"
            End If
            Return ViewState("FontName").ToString
        End Get
        Set(value As String)
            If value = String.Empty Then
                pnlHeader.Font.Name = "Tahoma"
                Checklist.Font.Name = "Tahoma"
                lblPrompt.Font.Name = "Tahoma"
                btnOK.Font.Name = "Tahoma"
                btnCancel.Font.Name = "Tahoma"
            ElseIf Checklist.Font.Name <> value Then
                Checklist.Font.Name = value
                pnlHeader.Font.Name = value
                lblPrompt.Font.Name = value
                btnOK.Font.Name = value
                btnCancel.Font.Name = value
            End If
            ViewState("FontName") = Checklist.Font.Name
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
                DivChecklist.Style("width") = value.ToString & "px"
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
    Public Property PromptForeColor As System.Drawing.Color
        Get
            Return lblPrompt.ForeColor
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                lblPrompt.ForeColor = System.Drawing.Color.Black
            End If
            lblPrompt.ForeColor = value
        End Set
    End Property
    Public Property PromptFontSize As FontUnit
        Get
            Return lblPrompt.Font.Size
        End Get
        Set(value As FontUnit)
            If value <> lblPrompt.Font.Size Then
                lblPrompt.Font.Size = value
            End If
        End Set
    End Property
    Public Property PromptText As String
        Get
            If ViewState("PromptText") Is Nothing Then
                ViewState("PromptText") = lblPrompt.Text
            End If
            Return ViewState("PromptText").ToString
        End Get
        Set(value As String)
            lblPrompt.Text = value
            ViewState("PromptText") = value
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
    <Browsable(True), DefaultValue(GetType(Drawing.Color), "White")>
    Public Property ChecklistBackColor As System.Drawing.Color
        Get
            Return Drawing.Color.FromName(DivChecklist.Style("background-color"))
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                value = System.Drawing.Color.White
            End If
            DivChecklist.Style("background-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
        End Set
    End Property
    <Browsable(True), DefaultValue(GetType(Drawing.Color), "Black")>
    Public Property ChecklistForeColor As System.Drawing.Color
        Get
            Return Drawing.Color.FromName(DivChecklist.Style("color"))
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                value = System.Drawing.Color.White
            End If
            DivChecklist.Style("color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
        End Set
    End Property
    <Browsable(True), DefaultValue(GetType(Drawing.Color), "DarkGray")>
    Public Property ChecklistBorderColor As Drawing.Color
        Get
            Return Drawing.Color.FromName(DivChecklist.Style("border-color"))
        End Get
        Set(value As Drawing.Color)
            If value <> Drawing.Color.Transparent Then
                DivChecklist.Style("border-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
                Dim borderwidth As String = DivChecklist.Style("border-width")
                Dim borderstyle As String = DivChecklist.Style("border-style")
                If borderwidth = String.Empty Then DivChecklist.Style("border-width") = "1px"
                If borderstyle = String.Empty Then DivChecklist.Style("border-style") = "solid"
            End If
        End Set
    End Property
    <Browsable(True), DefaultValue(1)>
    Public Property ChecklistBorderWidth As Integer
        Get
            Dim bordwidth As String = DivChecklist.Style("border-width").ToString
            If bordwidth <> String.Empty Then
                Return CInt(Val(bordwidth))
            Else
                Return 1
            End If

        End Get
        Set(value As Integer)
            DivChecklist.Style("border-width") = value.ToString & "px"
        End Set
    End Property
#End Region

#Region "Event Definitions"
    Public Delegate Sub DlgBoxEventHandler(sender As Object, e As DlgBoxEventArgs)
    Public Event DialogResulted As DlgBoxEventHandler
    Protected Overridable Sub OnDialogResulted(e As DlgBoxEventArgs)
        RaiseEvent DialogResulted(Me, e)
    End Sub
#End Region

#Region "Event Handlers"
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles Msgbox.MessageResulted
        If e.Tag = "NoItems" Then
            popDlg.Show()
            SetControlFocus("Checklist")
        End If
    End Sub
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If SelectedItems Is Nothing OrElse SelectedItems.Count = 0 Then
            Msgbox.Show("No Items have been selected.", "Warning", "NoItems", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning)
        Else
            OnDialogResulted(New DlgBoxEventArgs(DialogResult.OK, SelectedItems))
        End If
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        OnDialogResulted(New DlgBoxEventArgs(DialogResult.Cancel, Nothing))
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub CheckboxDlg_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        udpCheckboxDlg.Update()
        If TypeOf Parent Is UpdatePanel Then
            Dim udpContainer As UpdatePanel = CType(Parent, UpdatePanel)
            udpContainer.Update()
        End If

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
    Public Sub Show(Caption As String, Prompt As String, Optional OKButtonCaption As String = "OK")
        lblHeader.Text = Caption
        lblPrompt.Text = Prompt
        OKCaption = OKButtonCaption
        popDlg.Show()
        SetControlFocus("Checklist")
    End Sub
#End Region

End Class
