Imports System.ComponentModel

Partial Class Controls_CheckboxDropDown

    Inherits System.Web.UI.UserControl
#Region "Variables"
    Dim arBorderStyles As String()

#End Region
#Region "Event Definitions"
    Public Event DropDownOpened As EventHandler
    Protected Overridable Sub OnDropDownOpened(e As EventArgs)
        RaiseEvent DropDownOpened(Me, e)
    End Sub
    Public Event DropDownClosed As EventHandler
    Protected Overridable Sub OnDropDownClosed(e As EventArgs)
        RaiseEvent DropDownClosed(Me, e)
    End Sub
#End Region

#Region "Properties"
    Public Property Text As String
        Get
            Return txtValue.Text
        End Get
        Set(value As String)
            txtValue.Text = value
            txtValue.ToolTip = value
        End Set
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
                Dim ChecklistWidth As Integer = Val(value) - 7
                DivChecklist.Style("width") = ChecklistWidth.ToString & "px"
            End If

        End Set
    End Property
    <Browsable(True), DefaultValue(1)>
    Public Property BorderWidth As Integer
        Get
            Dim bordwidth As String = DivChecklist.Style("border-width").ToString
            If bordwidth <> String.Empty Then
                Return Val(bordwidth)
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
                Dim borderwidth As String = DivChecklist.Style("border-width").ToString
                Dim borderstyle As String = DivChecklist.Style("border-style").ToString
                If borderwidth = String.Empty Then DivChecklist.Style("border-width") = "1px"
                If borderstyle = String.Empty Then DivChecklist.Style("border-style") = "solid"
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
            If Not value.Equals(System.Drawing.Color.Transparent) AndAlso txtValue.ForeColor <> value Then
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
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With Me
            'Dim ckID As String = Me.ClientID & "_"
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

        'btnDD.Attributes.Add("onclick", "OpenCheckListBox('" & ctlID & "');")
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

    Private Sub Checklist_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Checklist.SelectedIndexChanged
        Dim cklst As CheckBoxList = CType(sender, CheckBoxList)
        Dim list As ListItem()
        ReDim list(-1)
        Dim n As Integer = 0

        For i As Integer = 0 To cklst.Items.Count - 1
            If cklst.Items(i).Selected Then
                ReDim Preserve list(n)
                list(n) = cklst.Items(i)
                n = n + 1
            End If
        Next
    End Sub

    Private Sub btnDropDown_Click(sender As Object, e As EventArgs) Handles btnDropDown.Click
        If DivChecklist.Visible Then
            DivChecklist.Visible = False
            btnDropDown.CssClass = "ButtonDownStyle"
            'DivAction.Visible = False
            OnDropDownClosed(New EventArgs)
        Else
            DivChecklist.Visible = True
            btnDropDown.CssClass = "ButtonUpStyle"
            'DivAction.Visible = True
            OnDropDownOpened(New EventArgs)
        End If
    End Sub






#End Region
End Class
