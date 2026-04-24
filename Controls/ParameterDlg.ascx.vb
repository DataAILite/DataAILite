Imports System.ComponentModel
Partial Class Controls_ParameterDlg
    Inherits System.Web.UI.UserControl
#Region "Enums"
    Public Enum ParamDialogResult
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
    Public Enum Mode
        Add
        Edit
        None
    End Enum
    'Public Enum EntryError
    '    None
    '    NoField
    '    NoLabel
    '    NoParameter
    '    NoType
    '    NoSQL

    'End Enum
#End Region

#Region "Variables"


#End Region

#Region "Classes"
    <Serializable> Public Class ParameterData
        Public Property Field As String
        Public Property Label As String
        Public Property Parameter As String
        Public Property ParameterType As String
        Public Property ParameterSQL As String
        Public Property ParameterComments As String
        'Public Property Err As EntryError = EntryError.None
    End Class
    Public Class ParamDlgBoxEventArgs
        Inherits System.EventArgs

        Private mParameterItem As ParameterData
        Private mOldParameterItem As ParameterData
        Private mResult As ParamDialogResult
        Private mEntryMode As Mode
        Public Sub New(MsgResult As ParamDialogResult, ParamItem As ParameterData, OldParamItem As ParameterData, ParamMode As Mode)
            mResult = MsgResult
            mParameterItem = ParamItem
            mEntryMode = ParamMode
            mOldParameterItem = OldParamItem
        End Sub
        Public ReadOnly Property Result As ParamDialogResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property ParameterItem As ParameterData
            Get
                Return mParameterItem
            End Get
        End Property
        Public ReadOnly Property OldParameterItem As ParameterData
            Get
                Return mOldParameterItem
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
    Property FieldItems As ListItemCollection
        Get
            Return ddField.Items
        End Get
        Set(value As ListItemCollection)
            ddField.Items.Clear()
            If Not value Is Nothing AndAlso value.Count > 0 Then
                For i As Integer = 0 To value.Count - 1
                    ddField.Items.Add(value(i))
                Next
            End If
        End Set
    End Property
    '<Browsable(False)>
    Public Property ReportSQL As String
        Get
            If ViewState("ReportSQL") Is Nothing Then
                ViewState("ReportSQL") = ""
            End If
            Return ViewState("ReportSQL").ToString
        End Get
        Set(value As String)
            ViewState("ReportSQL") = value
            hdnSQL.Value = value
        End Set
    End Property

    <Browsable(False)>
    Public Property TypeItems As ListItemCollection
        Get
            Return ddType.Items
        End Get
        Set(value As ListItemCollection)
            ddType.Items.Clear()
            If Not value Is Nothing AndAlso value.Count > 0 Then
                For i As Integer = 0 To value.Count - 1
                    ddType.Items.Add(value(i))
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
                ddField.Font.Name = "Arial"
                ddType.Font.Name = "Arial"
                txtComments.Font.Name = "Arial"
                txtSQL.Font.Name = "Arial"
                txtLabel.Font.Name = "Arial"
                txtParameter.Font.Name = "Arial"
                btnOK.Font.Name = "Arial"
                btnCancel.Font.Name = "Arial"
                divInput.Style.Item("font-name") = "Arial"
                ViewState("FontName") = "Arial"
            ElseIf divInput.Style.Item("font-name") <> value Then
                pnlHeader.Font.Name = value
                ddField.Font.Name = value
                ddType.Font.Name = value
                txtComments.Font.Name = value
                txtSQL.Font.Name = value
                txtLabel.Font.Name = value
                txtParameter.Font.Name = value
                btnOK.Font.Name = value
                btnCancel.Font.Name = value
                divInput.Style.Item("font-name") = value
                ViewState("FontName") = value

            End If
        End Set
    End Property
    <Browsable(False)>
    Public Property Data As ParameterData
        Get
            If ViewState("Data") Is Nothing Then
                ViewState("Data") = New ParameterData()
            End If
            Return CType(ViewState("Data"), ParameterData)
        End Get
        Set(value As ParameterData)
            ViewState("Data") = value
        End Set
    End Property
    <Browsable(False)>
    Public Property OldData As ParameterData
        Get
            If ViewState("OldData") Is Nothing Then
                ViewState("OldData") = New ParameterData()
            End If
            Return CType(ViewState("OldData"), ParameterData)
        End Get
        Set(value As ParameterData)
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
    <Browsable(False)>
    Public Property IsCacheDB As Boolean
        Get
            If ViewState("IsCacheDB") Is Nothing Then
                ViewState("IsCacheDB") = False
            End If
            Return Convert.ToBoolean(ViewState("IsCacheDB"))
        End Get
        Set(value As Boolean)
            ViewState("IsCacheDB") = value
            If value Then
                hdnIsCacheDB.Value = "1"
            Else
                hdnIsCacheDB.Value = "0"
            End If
        End Set
    End Property
    <Browsable(False)>
    Public Property IsOracleDB As Boolean
        Get
            If ViewState("IsOracleDB") Is Nothing Then
                ViewState("IsOracleDB") = False
            End If
            Return Convert.ToBoolean(ViewState("IsOracleDB"))
        End Get
        Set(value As Boolean)
            ViewState("IsOracleDB") = value
            If value Then
                hdnIsOracleDB.Value = "1"
            Else
                hdnIsOracleDB.Value = "0"
            End If
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
    Public Delegate Sub ParamDlgBoxEventHandler(sender As Object, e As ParamDlgBoxEventArgs)
    Public Event ParamDialogResulted As ParamDlgBoxEventHandler
    Protected Overridable Sub OnParamDialogResulted(e As ParamDlgBoxEventArgs)
        RaiseEvent ParamDialogResulted(Me, e)
    End Sub
#End Region

#Region "Event Handlers"
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ddField.SelectedItem.Text.Trim = "" Then
            MessageBox.Show("No field has been chosen.", "Parameter Entry", "NoField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        Else
            Data.Field = hdnField.Value
        End If

        If txtLabel.Text.Trim = "" Then
            MessageBox.Show("No label has been entered.", "Parameter Entry", "NoLabel", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        Else
            Data.Label = txtLabel.Text.Trim
        End If
        If txtParameter.Text.Trim = "" Then
            MessageBox.Show("No parameter has been entered.", "Parameter Entry", "NoParameter", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        Else
            Data.Parameter = txtParameter.Text.Trim
        End If

        If ddType.Text.Trim = "" Then
            MessageBox.Show("No type has been chosen.", "Parameter Entry", "NoType", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        Else
            Data.ParameterType = ddType.Text.Trim
        End If

        'If txtSQL.Text.Trim = "" Then
        '    MessageBox.Show("No SQL has been entered.", "Parameter Entry", "NoSQL", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
        '    Return
        'Else
        Data.ParameterSQL = txtSQL.Text.Trim
        'End If

        Data.ParameterComments = "manual"
        OnParamDialogResulted(New ParamDlgBoxEventArgs(ParamDialogResult.OK, Data, OldData, EntryMode))
    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        OnParamDialogResulted(New ParamDlgBoxEventArgs(ParamDialogResult.Cancel, Nothing, OldData, Mode.None))
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim x As Integer = 0
        If Session("AdvancedUser") = True Or Session("Attributes") = "sp" Then
            trType.Style.Item("Display") = ""
            trSQL.Style.Item("Display") = ""
        Else
            trType.Style.Item("Display") = "none"
            trSQL.Style.Item("Display") = "none"
        End If
    End Sub
    Protected Sub Controls_ParameterDlg_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim ctlID As String = Me.ClientID & "_"
        ddField.Attributes.Add("onchange", "SetOtherFields('" & ctlID & "');")
    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Select Case e.Tag
            Case "NoField"
                popDlg.Show()
                ddField.Focus()
            Case "NoLabel"
                popDlg.Show()
                txtLabel.Focus()
            Case "NoParameter"
                popDlg.Show()
                txtParameter.Focus()
            Case "NoType"
                popDlg.Show()
                ddType.Focus()
            Case "NoSQL"
                popDlg.Show()
                txtSQL.Focus()
        End Select
    End Sub
    Private Sub ParameterDlg_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        udpParamDlg.Update()
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
            .Append("function SetdlgParamFocus()")
            .Append("{")
            .Append("  var ctl = document.getElementById('" & id & "');")
            .Append("  if (ctl != null)")
            .Append("    ctl.focus();")
            .Append("}")
            .Append("window.onload = SetdlgParamFocus;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "SetdlgParamFocus", sb.ToString)
    End Sub
    Public Sub Show(Caption As String, ParamData As ParameterData, Optional ParamEntryMode As Mode = Mode.Add, Optional OKButtonCaption As String = "OK")
        lblHeader.Text = Caption
        Data = ParamData
        EntryMode = ParamEntryMode
        OKCaption = OKButtonCaption
        If Data.Field = "" Then
            ddField.SelectedIndex = 0

        Else
            Dim li As ListItem = ddField.Items.FindByText(Data.Field)
            If li IsNot Nothing Then
                ddField.SelectedIndex = ddField.Items.IndexOf(li)
                hdnField.Value = Data.Field
            Else
                For i As Integer = 0 To ddField.Items.Count - 1
                    li = ddField.Items(i)
                    If li.Text.StartsWith(Data.Field) Then
                        ddField.SelectedIndex = i
                        Exit For
                    End If
                Next
            End If
        End If
        txtLabel.Text = Data.Label
        txtParameter.Text = Data.Parameter
        If Data.ParameterType = "" Then
            ddType.SelectedIndex = 0
        Else
            ddType.SelectedValue = ddType.Items.FindByText(Data.ParameterType).Value
        End If
        txtSQL.Text = Data.ParameterSQL
        txtComments.Text = Data.ParameterComments
        OldData = Data
        popDlg.Show()
        ddField.Focus()
    End Sub





#End Region

End Class
