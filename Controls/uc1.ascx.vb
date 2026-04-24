Imports System.ComponentModel


Partial Class Controls_uc1
    Inherits System.Web.UI.UserControl


#Region "Variables"
    Dim arBorderStyles As String()

#End Region
#Region "Enums"
    Public Enum PostBackMode
        Auto = 0
        OnClose = 1
        None = 2
    End Enum
#End Region

#Region "Classes"
    Public Class ChecklistChangedArgs
        Inherits System.EventArgs

        Private mItemDesc As String
        Private mItemChecked As Boolean
        Private mAllChecked As Boolean
        Private mNoneChecked As Boolean

        Public Sub New(ItmDesc As String, ItmChecked As Boolean, AllChecked As Boolean, NoneChecked As Boolean)
            mItemDesc = ItmDesc
            mItemChecked = ItmChecked
            mAllChecked = AllChecked
            mNoneChecked = NoneChecked
        End Sub

        Public ReadOnly Property ItemDescription As String
            Get
                Return mItemDesc
            End Get
        End Property

        Public ReadOnly Property ItemChecked As Boolean
            Get
                Return mItemChecked
            End Get
        End Property

        Public ReadOnly Property AllChecked As Boolean
            Get
                Return mAllChecked
            End Get
        End Property

        Public ReadOnly Property NoneChecked As Boolean
            Get
                Return mNoneChecked
            End Get
        End Property

    End Class

#End Region

#Region "Event Definitions"
    Public Delegate Sub ChecklistChangedHandler(sender As Object, e As ChecklistChangedArgs)
    Public Event ChecklistChanged As ChecklistChangedHandler
    Protected Overridable Sub OnChecklistChanged(e As ChecklistChangedArgs)
        RaiseEvent ChecklistChanged(Me, e)
    End Sub
#End Region

#Region "Properties"
    Public Property Text As String
        Get
            Return txtValue.Text
        End Get
        Set(value As String)
            txtValue.Text = value
            If value <> "Please select..." AndAlso value.ToUpper <> "ALL" Then
                hdnText.Value = value
            txtValue.ToolTip = value
            End If

        End Set
    End Property
    <Browsable(False)>
    Public ReadOnly Property CheckBoxList As CheckBoxList
        Get
            Return Checklist
        End Get
    End Property
    'Public Property Width As Double
    '    Get
    '        Return pnlDropDownCheckList.Width.Value
    '    End Get
    '    Set(value As Double)
    '        If value <> Nothing AndAlso value > 0 Then
    '            Dim UnitValue As New Unit(value)
    '            pnlDropDownCheckList.Width = UnitValue
    '            Dim CheckBoxValue As String = (value - 7).ToString & "px"
    '            DivChecklist.Style.Remove("width")
    '            DivChecklist.Style.Add("width", CheckBoxValue)
    '        End If
    '    End Set
    'End Property

    '<Editor(GetType(Web.UI.Design.WebControls.ListItemsCollectionEditor), GetType(System.Drawing.Design.UITypeEditor))>
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
    <Browsable(True), DefaultValue("210px")>
    Public Property DropDownHeight As String
        Get
            Return DivChecklist.Style.Item("max-height")
        End Get
        Set(value As String)

            If value <> String.Empty Then
                DivChecklist.Style.Remove("max-height")
                DivChecklist.Style.Add("max-height", value)
                DivChecklist.Style("height") = value
            End If
        End Set
    End Property
    <Browsable(True), DefaultValue("200px")>
    Public Property Width As String
        Get
            Return pnlDropDownCheckList.Style("width")
        End Get
        Set(value As String)
            If value <> String.Empty Then
                pnlDropDownCheckList.Style("width") = value
                Dim ChecklistWidth As Double = Val(value) - 7
                DivChecklist.Style("width") = ChecklistWidth.ToString & "px"
            End If

        End Set
    End Property
    <Browsable(True), DefaultValue("20px")>
    Public Property TextBoxHeight As String
        Get
            If txtValue.Style("height") = "" Then
                txtValue.Style("height") = "20px"
            End If
            Return txtValue.Style("height")
        End Get
        Set(value As String)
            If value <> String.Empty Then
                Dim HeightVal As Double = Val(value)
                If HeightVal > 0 Then
                    Dim ht As String = HeightVal.ToString & "px"
                    txtValue.Style("height") = ht
                End If
            End If

        End Set
    End Property
    <Browsable(True), DefaultValue("20px")>
    Public Property DropDownButtonHeight As String
        Get
            If btnDropDown.Style("height") = "" Then
                btnDropDown.Style("height") = "20px"
            End If
            Return btnDropDown.Style("height")
        End Get
        Set(value As String)
            If value <> String.Empty Then
                Dim HeightVal As Double = Val(value)
                If HeightVal > 0 Then
                    Dim ht As String = HeightVal.ToString & "px"
                    btnDropDown.Style("height") = ht
                End If
            End If

        End Set
    End Property
    <Browsable(True), DefaultValue(1)>
    Public Property BorderWidth As Integer
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
    <Browsable(True), DefaultValue(GetType(Drawing.Color), "DarkGray")>
    Public Property BorderColor As Drawing.Color
        Get
            Return Drawing.Color.FromName(DivChecklist.Style("border-color"))
        End Get
        Set(value As Drawing.Color)
            If value <> Drawing.Color.Transparent Then
                DivChecklist.Style("border-color") = "rgba(" & value.R.ToString & "," & value.G.ToString & "," & value.B.ToString & "," & value.A.ToString & ")"
                'Dim borderwidth As String = String.Empty
                If DivChecklist.Style("border-width") Is Nothing Then
                    DivChecklist.Style("border-width") = "1px"
                    DivChecklist.Style("border-style") = "solid"
                End If
            End If
        End Set
    End Property
    Public Property AutoPostBack As Boolean
        Get
            Return Checklist.AutoPostBack
        End Get
        Set(value As Boolean)
            If value <> Checklist.AutoPostBack Then
                'Checklist.AutoPostBack = value
                If value Then
                    PostBackType = PostBackMode.Auto
                    'hdnAutoPostBack.Value = "1"
                    'hdnPostBackType.Value = "0"

                Else
                    'hdnAutoPostBack.Value = "0"
                    PostBackType = PostBackMode.OnClose
                End If
            End If
        End Set
    End Property
    Public Property PostBackType As PostBackMode
        Get
            Dim pbType As String = hdnPostBackType.Value
            Select Case pbType
                Case "0"
                    Return PostBackMode.Auto
                Case "1"
                    Return PostBackMode.OnClose
                Case Else
                    Return PostBackMode.None
            End Select
        End Get
        Set(value As PostBackMode)
            Select Case value
                Case PostBackMode.Auto
                    hdnPostBackType.Value = "0"
                    hdnAutoPostBack.Value = "1"
                    Checklist.AutoPostBack = True
                Case PostBackMode.OnClose
                    hdnPostBackType.Value = "1"
                    hdnAutoPostBack.Value = "0"
                    Checklist.AutoPostBack = False
                Case PostBackMode.None
                    hdnPostBackType.Value = "2"
                    hdnAutoPostBack.Value = "0"
                    Checklist.AutoPostBack = False
            End Select
        End Set
    End Property
    <Browsable(False)>
    Public Property SelectedItemsString As String
        Get
            Return hdnText.Value
        End Get
        Set(value As String)
            hdnText.Value = value
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
    <Browsable(False)>
    Public ReadOnly Property AllSelected As Boolean
        Get
            Dim all As Boolean = True
            If Not Checklist.Items Is Nothing And Checklist.Items.Count > 0 Then
                For i As Integer = 0 To Checklist.Items.Count - 1
                    If Not Checklist.Items(i).Selected Then
                        all = False
                        Exit For
                    End If
                Next
            Else
                all = False
            End If

            Return all
        End Get
    End Property

    <Browsable(False)>
    Public ReadOnly Property NoneSelected As Boolean
        Get
            Dim none As Boolean = True
            If Not Checklist.Items Is Nothing And Checklist.Items.Count > 0 Then
                For i As Integer = 0 To Checklist.Items.Count - 1
                    If Checklist.Items(i).Selected Then
                        none = False
                        Exit For
                    End If
                Next
            End If

            Return none
        End Get
    End Property

    Public Property CssClass As String
        Get
            Return txtValue.CssClass
        End Get
        Set(value As String)
            If value <> String.Empty Then
                If txtValue.CssClass <> value Then
                    txtValue.CssClass = value
                    Checklist.CssClass = value
                End If
            End If
        End Set
    End Property

    Public Property FontName As String
        Get
            Return txtValue.Font.Name
        End Get
        Set(value As String)
            If value = String.Empty Then
                txtValue.Font.Name = "Trebuchet MS"
                Checklist.Font.Name = "Trebuchet MS"
            ElseIf txtValue.Font.Name <> value Then
                txtValue.Font.Name = value
                Checklist.Font.Name = value
            End If
        End Set
    End Property
    Public Property FontBold As Boolean
        Get
            Return txtValue.Font.Bold
        End Get
        Set(value As Boolean)
            If value <> txtValue.Font.Bold Then
                txtValue.Font.Bold = value
                Checklist.Font.Bold = value
            End If
        End Set
    End Property
    Public Property FontSize As FontUnit
        Get
            Return txtValue.Font.Size
        End Get
        Set(value As FontUnit)
            If value <> txtValue.Font.Size Then
                txtValue.Font.Size = value
                Checklist.Font.Size = value
            End If
        End Set
    End Property

    Public Property ForeColor As System.Drawing.Color
        Get
            Return txtValue.ForeColor
        End Get
        Set(value As System.Drawing.Color)
            If Not value.Equals(System.Drawing.Color.Transparent) AndAlso
               txtValue.ForeColor <> value Then
                txtValue.ForeColor = value
                Checklist.ForeColor = value
            End If
        End Set
    End Property
    Public Property TextBoxBackColor As System.Drawing.Color
        Get
            Return txtValue.BackColor
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(System.Drawing.Color.Transparent) Then
                txtValue.BackColor = Drawing.Color.White
                pnlDropDownCheckList.BackColor = txtValue.BackColor
            ElseIf value <> txtValue.BackColor Then
                txtValue.BackColor = value
                pnlDropDownCheckList.BackColor = value

            End If
        End Set
    End Property
    Public Property DropDownBackColor As System.Drawing.Color
        Get
            Return Checklist.BackColor
        End Get
        Set(value As System.Drawing.Color)
            If value.Equals(Drawing.Color.Transparent) Then
                DivChecklist.Style("background-color") = Drawing.Color.White.Name
                Checklist.BackColor = Drawing.Color.White
            ElseIf value <> Checklist.BackColor Then
                DivChecklist.Style("background-color") = value.Name
                Checklist.BackColor = value
            End If

        End Set
    End Property
#End Region
#Region "Public Methods"
    Public Sub SetAll(Check As Boolean)
        If Not Checklist.Items Is Nothing And Checklist.Items.Count > 0 Then
            Dim sSelected As String = ""
            For i As Integer = 0 To Checklist.Items.Count - 1
                If Check Then
                    Checklist.Items(i).Selected = True
                    sSelected = CType(IIf(sSelected = "", Checklist.Items(i).Text, sSelected & "," & Checklist.Items(i).Text), String)
                Else
                    Checklist.Items(i).Selected = False
                End If
            Next
            SelectedItemsString = sSelected
            If Check Then
                txtValue.Text = "ALL"
            Else
                txtValue.Text = "Please select..."
            End If
            txtValue.ToolTip = sSelected

            Dim cls As String = DivChecklist.Attributes.Item("class")
            If cls IsNot Nothing AndAlso cls = "DropDownOpened" AndAlso hdnDropDown.Value = "closed" Then
                DivChecklist.Attributes.Add("class", "DropDownClosed")
                btnDropDown.CssClass = "ButtonDownStyle"
                hdnDropDown.Value = "closed"
            End If

            Dim arg As ChecklistChangedArgs

            If Check Then
                arg = New ChecklistChangedArgs("ALL", True, True, False)
            Else
                arg = New ChecklistChangedArgs("ALL", False, False, True)
            End If

            OnChecklistChanged(arg)
        End If
    End Sub

#End Region
    Private Sub WindowOnLoadScript()
        Dim sb As New System.Text.StringBuilder("")

        With sb
            .Append("<script language='JavaScript'>")
            .Append("window.onload = AdjustScroll('" & ClientID & "_');")
            .Append("</script>")
        End With

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), ClientID, sb.ToString, False)


    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With Me
            WindowOnLoadScript()
            'Dim ckID As String = Me.ClientID & "_"

            'Dim target As String = Request("__EVENTTARGET")
            'Dim data As String = Request("__EVENTARGUMENT")
            'If target IsNot Nothing AndAlso data IsNot Nothing Then
            '    If target = ckID & "CheckItemChanged" Then

            '    End If
            'End If
            '
            'If Request("__EVENTARGUMENT") = "opened" AndAlso
            '    Request("__EVENTTARGET") = ckID Then
            '    DropDownChanged("opened")
            'End If
            'DivChecklist.Style.Remove("max-Height")
            'DivChecklist.Style.Add("max-height", "400px")
            'Dim ss As String = DivChecklist.Style.Item("max-height")
            If Not IsPostBack Then

            End If
        End With
    End Sub
    Protected Sub PageInit() Handles Me.Init
        Dim ctlID As String = Me.ClientID & "_"

        btnDropDown.OnClientClick = "OpenCheckListBox('" & ctlID & "');return false;"
        txtValue.Attributes.Add("onclick", "OpenCheckListBox('" & ctlID & "');")
        Checklist.Attributes.Add("onclick", "CheckedIndexChanged(event,'" & ctlID & "');")
    End Sub
#Region "Private Events"
    'Private Sub DropDownChanged(param As String)
    '    With Me
    '        If param = "opened" Then
    '            OnDropDownOpened(New EventArgs)
    '        ElseIf param = "closed" Then
    '            OnDropDownClosed(New EventArgs)
    '        End If
    '    End With
    'End Sub

    Protected Sub Checklist_SelectedIndexChanged(sender As Object, e As EventArgs) 'Handles Checklist.SelectedIndexChanged
        Dim cklst As CheckBoxList = CType(sender, CheckBoxList)
        Dim n As Integer = 0

        Dim SelectedData As String = hdnSelectedData.Value

        If AutoPostBack Then
            'keep drop down open
            DivChecklist.Attributes.Add("class", "DropDownOpened")
            btnDropDown.CssClass = "ButtonUpStyle"
            hdnDropDown.Value = "open"
        End If

        'set selected items in drop down textbox
        Text = Me.SelectedItemsString

        'call ChecklistChanged event
        Dim data() As String = Split(SelectedData, "~")
        Dim itmdesc As String = data(0)
        Dim itmchecked As Boolean = CBool(data(1))
        Dim allchecked As Boolean = CBool(data(2))
        Dim nonechecked As Boolean = CBool(data(3))

        If allchecked Then
            Text = "ALL"
        ElseIf nonechecked Then
            Text = "Please select..."
        End If
        'call ChecklistChanged event
        Dim arg As New ChecklistChangedArgs(itmdesc, itmchecked, allchecked, nonechecked)

        OnChecklistChanged(arg)

    End Sub










#End Region
End Class

