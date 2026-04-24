Imports System.ComponentModel
Partial Class Controls_ConditionDlg
    Inherits System.Web.UI.UserControl

#Region "Enums"
    Public Enum ConditionDialogResult
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
#End Region

#Region "Classes"
    <Serializable> Public Class ConditionData
        Public Property IsDate As Boolean
        Public Property Field As String
        Public Property NotOperator As Boolean
        Public Property ConditionOperator As String
        Public Property FieldChosen1 As Boolean
        Public Property TextValue1 As String
        Public Property FieldValue1 As String
        Public Property FieldChosen2 As Boolean
        Public Property TextValue2 As String
        Public Property FieldValue2 As String
        Public Property DateRelative1 As Boolean
        Public Property DateFieldChosen1 As Boolean
        Public Property DateValue1 As String
        Public Property DateFieldValue1 As String
        Public Property DateRelative2 As Boolean
        Public Property DateFieldChosen2 As Boolean
        Public Property DateValue2 As String
        Public Property DateFieldValue2 As String

    End Class

    Public Class CondDlgBoxEventArgs
        Inherits System.EventArgs

        Private mConditionItem As ConditionData
        Private mOldConditionItem As ConditionData
        Private mResult As ConditionDialogResult
        Private mEntryMode As Mode
        Public Sub New(MsgResult As ConditionDialogResult, CondItem As ConditionData, OldCondItem As ConditionData, ConditionMode As Mode)
            mResult = MsgResult
            mConditionItem = CondItem
            mOldConditionItem = OldCondItem
            mEntryMode = ConditionMode
        End Sub
        Public ReadOnly Property Result As ConditionDialogResult
            Get
                Return mResult
            End Get
        End Property
        Public ReadOnly Property ConditionItem As ConditionData
            Get
                Return mConditionItem
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
            ddDateField1.Items.Clear()
            ddDateField2.Items.Clear()
            ddValField1.Items.Clear()
            ddValField2.Items.Clear()

            If Not value Is Nothing AndAlso value.Count > 0 Then
                For i As Integer = 0 To value.Count - 1
                    ddField.Items.Add(New ListItem(value(i).Text, value(i).Value))
                    ddDateField1.Items.Add(New ListItem(value(i).Text, value(i).Value))
                    ddDateField2.Items.Add(New ListItem(value(i).Text, value(i).Value))
                    ddValField1.Items.Add(New ListItem(value(i).Text, value(i).Value))
                    ddValField2.Items.Add(New ListItem(value(i).Text, value(i).Value))
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
                lblField.Font.Name = "Arial"
                ddField.Font.Name = "Arial"
                lblOperator.Font.Name = "Arial"
                ddOperator.Font.Name = "Arial"
                lblValue1.Font.Name = "Arial"
                lblValue2.Font.Name = "Arial"
                txtValue1.Font.Name = "Arial"
                txtValue2.Font.Name = "Arial"
                ddValField1.Font.Name = "Arial"
                ddValField2.Font.Name = "Arial"
                lblAnd1.Font.Name = "Arial"
                divAnd1.Style.Item("font-name") = "Arial"
                lblCal1.Font.Name = "Arial"
                lblCal2.Font.Name = "Arial"
                Date1.Font.Name = "Arial"
                Date2.Font.Name = "Arial"
                ddDateField1.Font.Name = "Arial"
                ddDateField2.Font.Name = "Arial"
                lblAnd2.Font.Name = "Arial"
                'divAnd2.Style.Item("font-name") = "Arial"
                'ckCal1.Font.Name = "Arial"
                'ckCal2.Font.Name = "Arial"
                ckRelative1.Font.Name = "Arial"
                ckRelative2.Font.Name = "Arial"
                'ckTxt1.Font.Name = "Arial"
                'ckTxt2.Font.Name = "Arial"
                btnOK.Font.Name = "Arial"
                btnCancel.Font.Name = "Arial"
                divInput.Style.Item("font-name") = "Arial"
                ViewState("FontName") = "Arial"
            ElseIf divInput.Style.Item("font-name") <> value Then
                pnlHeader.Font.Name = value
                lblField.Font.Name = value
                ddField.Font.Name = value
                lblOperator.Font.Name = value
                ddOperator.Font.Name = value

                lblValue1.Font.Name = value
                lblValue2.Font.Name = value
                txtValue1.Font.Name = value
                txtValue2.Font.Name = value
                ddValField1.Font.Name = value
                ddValField2.Font.Name = value
                lblAnd1.Font.Name = value
                lblCal1.Font.Name = value
                lblCal2.Font.Name = value
                Date1.Font.Name = value
                Date2.Font.Name = value
                ddDateField1.Font.Name = value
                ddDateField2.Font.Name = value
                'divAnd2.Style.Item("font-name") = value
                lblAnd2.Font.Name = value
                'ckCal1.Font.Name = value
                'ckCal2.Font.Name = value
                ckRelative1.Font.Name = value
                ckRelative2.Font.Name = value
                'ckTxt1.Font.Name = value
                'ckTxt2.Font.Name = value

                btnOK.Font.Name = value
                btnCancel.Font.Name = value
                divInput.Style.Item("font-name") = value
                ViewState("FontName") = value
            End If
        End Set
    End Property

    Public Property FontSize As FontUnit
        Get
            If ViewState("FontSize") Is Nothing Then
                ViewState("FontSize") = lblField.Font.Size
            End If
            Return CType(ViewState("FontSize"), FontUnit)
        End Get
        Set(value As FontUnit)
            If value <> lblField.Font.Size Then
                lblField.Font.Size = value
                ddField.Font.Size = value
                lblOperator.Font.Size = value
                ddOperator.Font.Size = value
                lblValue1.Font.Size = value
                lblValue2.Font.Size = value
                txtValue1.Font.Size = value
                txtValue2.Font.Size = value
                ddValField1.Font.Size = value
                ddValField2.Font.Size = value
                lblAnd1.Font.Size = value

                lblCal1.Font.Size = value
                lblCal2.Font.Size = value
                Date1.Font.Size = value
                Date2.Font.Size = value
                ddDateField1.Font.Size = value
                ddDateField2.Font.Size = value
                lblAnd2.Font.Size = value

                'ckCal1.Font.Size = value
                'ckCal2.Font.Size = value
                ckRelative1.Font.Size = value
                ckRelative2.Font.Size = value
                'ckTxt1.Font.Size = value
                'ckTxt2.Font.Size = value

                divInput.Style.Item("font-size") = value.ToString
                ViewState("FontSize") = value
            End If
        End Set
    End Property
    <Browsable(False)>
    Public Property Data As ConditionData
        Get
            If ViewState("Data") Is Nothing Then
                ViewState("Data") = New ConditionData()
            End If
            Return CType(ViewState("Data"), ConditionData)
        End Get
        Set(value As ConditionData)
            ViewState("Data") = value
        End Set
    End Property
    <Browsable(False)>
    Public Property OldData As ConditionData
        Get
            If ViewState("OldData") Is Nothing Then
                ViewState("OldData") = New ConditionData()
            End If
            Return CType(ViewState("OldData"), ConditionData)
        End Get
        Set(value As ConditionData)
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
#End Region

#Region "Event Handlers"
    Private Sub ConditionDlg_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim ctlID As String = Me.ClientID & "_"
        btnText1.OnClientClick = "showTextorFields('" & ctlID & "','btnText1');return false;"
        btnText2.OnClientClick = "showTextorFields('" & ctlID & "','btnText2');return false;"
        btnCal1.OnClientClick = "showTextorFields('" & ctlID & "','btnCal1');return false;"
        btnCal2.OnClientClick = "showTextorFields('" & ctlID & "','btnCal2');return false;"
        ddOperator.Attributes.Add("onchange", "handleOperatorChange('" & ctlID & "');")
        ddField.Attributes.Add("onchange", "handleFieldChange('" & ctlID & "');")
        ddValField1.Attributes.Add("onchange", "handleOtherFieldsChange('" & ctlID & "','ddValField1');")
        ddValField2.Attributes.Add("onchange", "handleOtherFieldsChange('" & ctlID & "','ddValField2');")
        ddDateField1.Attributes.Add("onchange", "handleOtherFieldsChange('" & ctlID & "','ddDateField1');")
        ddDateField2.Attributes.Add("onchange", "handleOtherFieldsChange('" & ctlID & "','ddDateField2');")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        OnCondDialogResulted(New CondDlgBoxEventArgs(ConditionDialogResult.Cancel, Nothing, Nothing, Mode.None))
    End Sub
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Dim oper As String = hdnOperator.Value
        Dim typ As String = String.Empty

        If hdnFieldIndex.Value = 0 Then
            MessageBox.Show("No field has been chosen.", "Condition Entry", "NoField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
            Return
        End If
        Data.Field = ddField.Items(hdnFieldIndex.Value).Text
        Data.ConditionOperator = oper
        Data.NotOperator = ckNot.Checked
        typ = ddField.Items(hdnFieldIndex.Value).Value.ToUpper
        Dim IsDate As Boolean = (typ <> "TIME" AndAlso (typ.Contains("DATE") OrElse typ.Contains("TIME")))
        Data.IsDate = IsDate
        If oper = "between" Then
            If IsDate Then
                Dim btnCalText1 As String = hdnBtnCal1.Value
                Dim btnCalText2 As String = hdnBtnCal2.Value
                If btnCalText1 = "Fields" Then
                    If Date1.Text.Trim = "" Then
                        If btnCalText2 = "Fields" Then
                            hdnNext.Value = "Date"
                        Else
                            hdnNext.Value = "DateField"
                        End If
                        MessageBox.Show("First date has not been entered.", "Condition Entry", "NoFirstDate", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.DateRelative1 = ckRelative1.Checked
                    Data.DateValue1 = Date1.Text
                Else
                    If ddDateField1.SelectedItem.Text.Trim = "" Then
                        If btnCalText2 = "Fields" Then
                            hdnNext.Value = "Date"
                        Else
                            hdnNext.Value = "DateField"
                        End If
                        MessageBox.Show("First date field has not been chosen.", "Condition Entry", "NoFirstDateField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.DateFieldChosen1 = True
                    Data.DateFieldValue1 = hdnDateField1.Value
                End If
                If btnCalText2 = "Fields" Then
                    If Date2.Text.Trim = "" Then
                        If btnCalText1 = "Fields" Then
                            hdnPrev.Value = "Date"
                        Else
                            hdnPrev.Value = "DateField"
                        End If
                        MessageBox.Show("Second date has not been entered.", "Condition Entry", "NoSecondDate", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.DateRelative2 = ckRelative2.Checked
                    Data.DateValue2 = Date2.Text
                Else
                    If ddDateField2.SelectedItem.Text.Trim = "" Then
                        If btnCalText1 = "Fields" Then
                            hdnPrev.Value = "Date"
                        Else
                            hdnPrev.Value = "DateField"
                        End If
                        MessageBox.Show("Second date field has not been chosen.", "Condition Entry", "NoSecondDateField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.DateFieldChosen2 = True
                    Data.DateFieldValue2 = hdnDateField2.Value
                End If
            Else
                Dim btnTxt1 As String = hdnBtnText1.Value
                Dim btnTxt2 As String = hdnBtnText2.Value
                If btnTxt1 = "Fields" Then
                    If txtValue1.Text.Trim = "" Then
                        If btnTxt2 = "Fields" Then
                            hdnNext.Value = "TextVal"
                        Else
                            hdnNext.Value = "FieldVal"
                        End If
                        MessageBox.Show("First text value has not been entered.", "Condition Entry", "NoFirstText", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                            Return
                        End If
                        Data.FieldChosen1 = False
                    Data.TextValue1 = txtValue1.Text.Trim
                Else
                    If ddValField1.SelectedItem.Text.Trim = "" Then
                        If btnTxt2 = "Fields" Then
                            hdnNext.Value = "TextVal"
                        Else
                            hdnNext.Value = "FieldVal"
                        End If
                        MessageBox.Show("First field value has not been chosen.", "Condition Entry", "NoFirstField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.FieldChosen1 = True
                    Data.FieldValue1 = hdnValField1.Value
                End If
                If btnTxt2 = "Fields" Then
                    If txtValue2.Text.Trim = "" Then
                        If btnTxt1 = "Fields" Then
                            hdnPrev.Value = "TextVal"
                        Else
                            hdnPrev.Value = "FieldVal"
                        End If
                        MessageBox.Show("Second text value has not been entered.", "Condition Entry", "NoSecondText", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.FieldChosen2 = False
                    Data.TextValue2 = txtValue2.Text.Trim
                Else
                    If ddValField2.SelectedItem.Text.Trim = "" Then
                        If btnTxt1 = "Fields" Then
                            hdnPrev.Value = "TextVal"
                        Else
                            hdnPrev.Value = "FieldVal"
                        End If
                        MessageBox.Show("Second field value has not been chosen.", "Condition Entry", "NoSecondField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.FieldChosen2 = True
                    Data.FieldValue2 = hdnValField2.Value

                End If
            End If
        ElseIf oper = "IS NULL" Then
            Data.IsDate = False
            Data.DateFieldChosen1 = False
            Data.DateFieldChosen2 = False
            Data.FieldChosen1 = False
            Data.TextValue1 = String.Empty
            Data.FieldValue1 = String.Empty
            Data.FieldChosen2 = False
            Data.TextValue2 = String.Empty
            Data.FieldValue2 = String.Empty
            Data.DateRelative1 = False
            Data.DateValue1 = String.Empty
            Data.DateFieldValue1 = String.Empty
            Data.DateRelative2 = False
            Data.DateValue2 = String.Empty
            Data.DateFieldValue2 = String.Empty
        Else
            If IsDate Then
                Dim btnCalText1 As String = hdnBtnCal1.Value
                If btnCalText1 = "Fields" Then
                    If Date1.Text.Trim = "" Then
                        MessageBox.Show("Date has not been entered.", "Condition Entry", "NoDate", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.DateRelative1 = ckRelative1.Checked
                    Data.DateValue1 = Date1.Text
                    Data.DateRelative2 = False
                    Data.DateValue2 = ""
                    Data.DateFieldChosen1 = False
                    Data.DateFieldChosen2 = False
                Else
                    If ddDateField1.SelectedItem.Text.Trim = "" Then
                        MessageBox.Show("Date field has not been chosen.", "Condition Entry", "NoDateField", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.DateFieldChosen1 = True
                    Data.DateFieldChosen2 = False
                    Data.DateFieldValue1 = hdnDateField1.Value
                    Data.DateRelative1 = False
                    Data.DateValue1 = ""
                    Data.DateRelative2 = False
                    Data.DateValue2 = ""
                    Data.DateFieldValue2 = ""
                End If
            Else
                Dim btnTxt1 As String = hdnBtnText1.Value
                If btnTxt1 = "Fields" Then
                    If txtValue1.Text.Trim = "" Then
                        MessageBox.Show("Text value has not been entered.", "Condition Entry", "NoTextVal", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.FieldChosen1 = False
                    Data.TextValue1 = txtValue1.Text.Trim
                Else
                    If ddValField1.SelectedItem.Text.Trim = "" Then
                        MessageBox.Show("Field value has not been chosen.", "Condition Entry", "NoFieldVal", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Error)
                        Return
                    End If
                    Data.FieldChosen1 = True
                    Data.FieldValue1 = hdnValField1.Value
                End If
            End If
        End If
        OnCondDialogResulted(New CondDlgBoxEventArgs(ConditionDialogResult.OK, Data, OldData, EntryMode))
    End Sub
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        Dim idx As Integer = CInt(hdnFieldIndex.Value)
        Select Case e.Tag
            Case "NoField"
                popDlg.Show()
                ddField.Focus()
            Case "NoDate"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = ""
                trText.Style.Item("display") = "none"
                divCal2.Style.Item("display") = "none"

                divCalendar1.Style.Item("display") = ""
                divDateField1.Style.Item("display") = "none"
                btnCal1.Text = "Fields"
                btnCal1.ToolTip = "Choose field"
                Date1.Focus()
            Case "NoFirstDate"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = ""
                trText.Style.Item("display") = "none"
                divCal2.Style.Item("display") = ""

                divCalendar1.Style.Item("display") = ""
                divDateField1.Style.Item("display") = "none"
                btnCal1.Text = "Fields"
                btnCal1.ToolTip = "Choose field"

                If hdnNext.Value = "Date" Then
                    divCalendar2.Style.Item("display") = ""
                    divDateField2.Style.Item("display") = "none"
                    btnCal2.Text = "Fields"
                    btnCal2.ToolTip = "Choose field"
                Else
                    divCalendar2.Style.Item("display") = "none"
                    divDateField2.Style.Item("display") = ""
                    btnCal2.Text = "Date"
                    btnCal2.ToolTip = "Enter date value"
                End If

                Date1.Focus()
            Case "NoSecondDate"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = ""
                trText.Style.Item("display") = "none"
                divCal2.Style.Item("display") = ""

                divCalendar2.Style.Item("display") = ""
                divDateField2.Style.Item("display") = "none"
                btnCal2.Text = "Fields"
                btnCal2.ToolTip = "Choose field"

                If hdnPrev.Value = "Date" Then
                    divCalendar1.Style.Item("display") = ""
                    divDateField1.Style.Item("display") = "none"
                    btnCal1.Text = "Fields"
                    btnCal1.ToolTip = "Choose field"
                Else
                    divCalendar1.Style.Item("display") = "none"
                    divDateField1.Style.Item("display") = ""
                    btnCal1.Text = "Date"
                    btnCal1.ToolTip = "Enter date value"
                End If
                Date2.Focus()
            Case "NoDateField"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = ""
                trText.Style.Item("display") = "none"
                divCal2.Style.Item("display") = "none"

                divCalendar1.Style.Item("display") = "none"
                divDateField1.Style.Item("display") = ""
                btnCal1.Text = "Date"
                btnCal1.ToolTip = "Enter date value"
                ddDateField1.Focus()
            Case "NoFirstDateField"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = ""
                trText.Style.Item("display") = "none"
                divCal2.Style.Item("display") = ""

                divCalendar1.Style.Item("display") = "none"
                divDateField1.Style.Item("display") = ""
                btnCal1.Text = "Date"
                btnCal1.ToolTip = "Enter date value"

                If hdnNext.Value = "Date" Then
                    divCalendar2.Style.Item("display") = ""
                    divDateField2.Style.Item("display") = "none"
                    btnCal2.Text = "Fields"
                    btnCal2.ToolTip = "Choose field"
                Else
                    divCalendar2.Style.Item("display") = "none"
                    divDateField2.Style.Item("display") = ""
                    btnCal2.Text = "Date"
                    btnCal2.ToolTip = "Enter date value"
                End If

                ddDateField1.Focus()
            Case "NoSecondDateField"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = ""
                trText.Style.Item("display") = "none"
                divCal2.Style.Item("display") = ""

                divCalendar2.Style.Item("display") = "none"
                divDateField2.Style.Item("display") = ""
                btnCal2.Text = "Date"
                btnCal2.ToolTip = "Enter date value"

                If hdnPrev.Value = "Date" Then
                    divCalendar1.Style.Item("display") = ""
                    divDateField1.Style.Item("display") = "none"
                    btnCal1.Text = "Fields"
                    btnCal1.ToolTip = "Choose field"
                Else
                    divCalendar1.Style.Item("display") = "none"
                    divDateField1.Style.Item("display") = ""
                    btnCal1.Text = "Date"
                    btnCal1.ToolTip = "Enter date value"
                End If

                ddDateField2.Focus()
            Case "NoTextVal"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = "none"
                trText.Style.Item("display") = ""
                divValue2.Style.Item("display") = "none"

                txtValue1.Style.Item("display") = ""
                ddValField1.Style.Item("display") = "none"
                btnText1.Text = "Fields"
                btnText1.ToolTip = "Choose field"

                txtValue1.Focus()
            Case "NoFieldVal"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = "none"
                trText.Style.Item("display") = ""
                divValue2.Style.Item("display") = "none"

                txtValue1.Style.Item("display") = "none"
                ddValField1.Style.Item("display") = ""
                btnText1.Text = "Text"
                btnText1.ToolTip = "Enter text value"

                ddValField1.Focus()
            Case "NoFirstText"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = "none"
                trText.Style.Item("display") = ""
                divValue2.Style.Item("display") = ""

                txtValue1.Style.Item("display") = ""
                ddValField1.Style.Item("display") = "none"
                btnText1.Text = "Fields"
                btnText1.ToolTip = "Choose field"

                If hdnNext.Value = "TextVal" Then
                    txtValue2.Style.Item("display") = ""
                    ddValField2.Style.Item("display") = "none"
                    btnText2.Text = "Fields"
                    btnText2.ToolTip = "Choose field"
                Else
                    txtValue2.Style.Item("display") = "none"
                    ddValField2.Style.Item("display") = ""
                    btnText2.Text = "Text"
                    btnText2.ToolTip = "Enter text value"
                End If

                txtValue1.Focus()
            Case "NoSecondText"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = "none"
                trText.Style.Item("display") = ""
                divValue2.Style.Item("display") = ""

                txtValue2.Style.Item("display") = ""
                ddValField2.Style.Item("display") = "none"
                btnText2.Text = "Fields"
                btnText2.ToolTip = "Choose field"

                If hdnPrev.Value = "TextVal" Then
                    txtValue1.Style.Item("display") = ""
                    ddValField1.Style.Item("display") = "none"
                    btnText1.Text = "Fields"
                    btnText1.ToolTip = "Choose field"
                Else
                    txtValue1.Style.Item("display") = "none"
                    ddValField1.Style.Item("display") = ""
                    btnText1.Text = "Text"
                    btnText1.ToolTip = "Enter text value"
                End If

                txtValue2.Focus()
            Case "NoFirstField"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = "none"
                trText.Style.Item("display") = ""
                divValue2.Style.Item("display") = ""

                txtValue1.Style.Item("display") = "none"
                ddValField1.Style.Item("display") = ""
                btnText1.Text = "Text"
                btnText1.ToolTip = "Enter text value"

                If hdnNext.Value = "TextVal" Then
                    txtValue2.Style.Item("display") = ""
                    ddValField2.Style.Item("display") = "none"
                    btnText2.Text = "Fields"
                    btnText2.ToolTip = "Choose field"
                Else
                    txtValue2.Style.Item("display") = "none"
                    ddValField2.Style.Item("display") = ""
                    btnText2.Text = "Text"
                    btnText2.ToolTip = "Enter text value"
                End If

                ddValField1.Focus()
            Case "NoSecondField"
                popDlg.Show()
                ddField.SelectedIndex = idx
                trCalendar.Style.Item("display") = "none"
                trText.Style.Item("display") = ""
                divValue2.Style.Item("display") = ""

                txtValue2.Style.Item("display") = "none"
                ddValField2.Style.Item("display") = ""
                btnText2.Text = "Text"
                btnText2.ToolTip = "Enter text value"

                If hdnPrev.Value = "TextVal" Then
                    txtValue1.Style.Item("display") = ""
                    ddValField1.Style.Item("display") = "none"
                    btnText1.Text = "Fields"
                    btnText1.ToolTip = "Choose field"
                Else
                    txtValue1.Style.Item("display") = "none"
                    ddValField1.Style.Item("display") = ""
                    btnText1.Text = "Text"
                    btnText1.ToolTip = "Enter text value"
                End If

                ddValField2.Focus()
        End Select
    End Sub
    Private Sub ConditionDlg_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        udpCondDlg.Update()
        If TypeOf Parent Is UpdatePanel Then
            Dim udpContainer As UpdatePanel = CType(Parent, UpdatePanel)
            udpContainer.Update()
        End If
    End Sub
#End Region

#Region "Event Definitions"
    Public Delegate Sub CondDlgBoxEventHandler(sender As Object, e As CondDlgBoxEventArgs)
    Public Event CondDialogResulted As CondDlgBoxEventHandler
    Protected Overridable Sub OnCondDialogResulted(e As CondDlgBoxEventArgs)
        RaiseEvent CondDialogResulted(Me, e)
    End Sub
#End Region

#Region "Methods"
    Private Sub SetControlFocus(ctlId As String)
        Dim id As String = Me.ClientID & "_" & ctlId
        Dim sb As New System.Text.StringBuilder("")
        Dim cs As ClientScriptManager = Page.ClientScript
        With sb
            .Append("<script language='JavaScript'>")
            .Append("function SetdlgCondFocus()")
            .Append("{")
            .Append("  var ctl = document.getElementById('" & id & "');")
            .Append("  if (ctl != null)")
            .Append("    ctl.focus();")
            .Append("}")
            .Append("window.onload = SetdlgCondFocus;")
            .Append("</script>")
        End With
        cs.RegisterStartupScript(Me.GetType, "SetdlgCondFocus", sb.ToString)
    End Sub
    Private Sub LoadConditionData()
        Dim li As ListItem = ddField.Items.FindByText(Data.Field)
        ddField.SelectedIndex = ddField.Items.IndexOf(li)
        hdnFieldIndex.Value = ddField.SelectedIndex

        li = ddOperator.Items.FindByText(Data.ConditionOperator)
        ddOperator.SelectedIndex = ddOperator.Items.IndexOf(li)
        ckNot.Checked = Data.NotOperator
        hdnOperator.Value = Data.ConditionOperator
        If Data.ConditionOperator.ToUpper <> "BETWEEN" Then
            If Data.ConditionOperator = "IS NULL" Then
                trText.Style.Item("display") = "none"
                trCalendar.Style.Item("display") = "none"
            ElseIf Data.IsDate Then
                trText.Style.Item("display") = "none"
                    trCalendar.Style.Item("display") = ""
                    lblCal1.Style.Item("display") = ""
                    lblCal2.Style.Item("display") = "none"
                    divCal2.Style.Item("display") = "none"

                    If Data.DateFieldChosen1 Then
                        divCalendar1.Style.Item("display") = "none"
                        divDateField1.Style.Item("display") = ""
                        btnCal1.Text = "Date"
                        btnCal1.ToolTip = "Enter date value"
                        hdnBtnCal1.Value = "Date"
                        li = ddDateField1.Items.FindByText(Data.DateFieldValue1)
                        ddDateField1.SelectedIndex = ddDateField1.Items.IndexOf(li)
                        hdnDateField1.Value = Data.DateFieldValue1
                    Else
                        divCalendar1.Style.Item("display") = ""
                        divDateField1.Style.Item("display") = "none"
                        btnCal1.Text = "Fields"
                        btnCal1.ToolTip = "Choose field"
                        hdnBtnCal1.Value = "Fields"
                        Date1.Text = Data.DateValue1
                        If Data.DateRelative1 Then ckRelative1.Checked = True
                    End If
                Else
                    trText.Style.Item("display") = ""
                    trCalendar.Style.Item("display") = "none"
                    lblValue1.Style.Item("display") = ""
                    lblValue2.Style.Item("display") = "none"
                    divValue2.Style.Item("display") = "none"
                    If Data.FieldChosen1 Then
                        ddValField1.Style.Item("display") = ""
                        txtValue1.Style.Item("display") = "none"
                        btnText1.Text = "Text"
                        btnText1.ToolTip = "Enter text value"
                        hdnBtnText1.Value = "Text"
                        li = ddValField1.Items.FindByText(Data.FieldValue1)
                        ddValField1.SelectedIndex = ddValField1.Items.IndexOf(li)
                        hdnValField1.Value = Data.FieldValue1
                    Else
                        ddValField1.Style.Item("display") = "none"
                        txtValue1.Style.Item("display") = ""
                        btnText1.Text = "Fields"
                        btnText1.ToolTip = "Choose field"
                        hdnBtnText1.Value = "Fields"
                        txtValue1.Text = Data.TextValue1
                    End If
                'End If
            End If
        'If Data.IsDate Then
        '    trText.Style.Item("display") = "none"
        '    trCalendar.Style.Item("display") = ""
        '    lblCal1.Style.Item("display") = ""
        '    lblCal2.Style.Item("display") = "none"
        '    divCal2.Style.Item("display") = "none"

        '    If Data.DateFieldChosen1 Then
        '        divCalendar1.Style.Item("display") = "none"
        '        divDateField1.Style.Item("display") = ""
        '        btnCal1.Text = "Date"
        '        btnCal1.ToolTip = "Enter date value"
        '        hdnBtnCal1.Value = "Date"
        '        li = ddDateField1.Items.FindByText(Data.DateFieldValue1)
        '        ddDateField1.SelectedIndex = ddDateField1.Items.IndexOf(li)
        '        hdnDateField1.Value = Data.DateFieldValue1
        '    Else
        '        divCalendar1.Style.Item("display") = ""
        '        divDateField1.Style.Item("display") = "none"
        '        btnCal1.Text = "Fields"
        '        btnCal1.ToolTip = "Choose field"
        '        hdnBtnCal1.Value = "Fields"
        '        Date1.Text = Data.DateValue1
        '        If Data.DateRelative1 Then ckRelative1.Checked = True
        '    End If
        'Else
        '    trText.Style.Item("display") = ""
        '    trCalendar.Style.Item("display") = "none"
        '    lblValue1.Style.Item("display") = ""
        '    lblValue2.Style.Item("display") = "none"
        '    divValue2.Style.Item("display") = "none"
        '    If Data.FieldChosen1 Then
        '        ddValField1.Style.Item("display") = ""
        '        txtValue1.Style.Item("display") = "none"
        '        btnText1.Text = "Text"
        '        btnText1.ToolTip = "Enter text value"
        '        hdnBtnText1.Value = "Text"
        '        li = ddValField1.Items.FindByText(Data.FieldValue1)
        '        ddValField1.SelectedIndex = ddValField1.Items.IndexOf(li)
        '        hdnValField1.Value = Data.FieldValue1
        '    Else
        '        ddValField1.Style.Item("display") = "none"
        '        txtValue1.Style.Item("display") = ""
        '        btnText1.Text = "Fields"
        '        btnText1.ToolTip = "Choose field"
        '        hdnBtnText1.Value = "Fields"
        '        txtValue1.Text = Data.TextValue1
        '    End If
        'End If
        Else
            If Data.IsDate Then
                trText.Style.Item("display") = "none"
                trCalendar.Style.Item("display") = ""
                lblCal1.Style.Item("display") = "none"
                lblCal2.Style.Item("display") = ""
                divCal2.Style.Item("display") = ""
                If Data.DateFieldChosen1 Then
                    divCalendar1.Style.Item("display") = "none"
                    divDateField1.Style.Item("display") = ""
                    btnCal1.Text = "Date"
                    btnCal1.ToolTip = "Enter date value"
                    hdnBtnCal1.Value = "Date"
                    li = ddDateField1.Items.FindByText(Data.DateFieldValue1)
                    ddDateField1.SelectedIndex = ddDateField1.Items.IndexOf(li)
                    hdnDateField1.Value = Data.DateFieldValue1
                Else
                    divCalendar1.Style.Item("display") = ""
                    divDateField1.Style.Item("display") = "none"
                    btnCal1.Text = "Fields"
                    btnCal1.ToolTip = "Choose field"
                    hdnBtnCal1.Value = "Fields"
                    Date1.Text = Data.DateValue1
                    If Data.DateRelative1 Then ckRelative1.Checked = True
                End If
                If Data.DateFieldChosen2 Then
                    divCalendar2.Style.Item("display") = "none"
                    divDateField2.Style.Item("display") = ""
                    btnCal2.Text = "Date"
                    btnCal2.ToolTip = "Enter date value"
                    hdnBtnCal2.Value = "Date"
                    li = ddDateField2.Items.FindByText(Data.DateFieldValue2)
                    ddDateField2.SelectedIndex = ddDateField2.Items.IndexOf(li)
                    hdnDateField2.Value = Data.DateFieldValue2
                Else
                    divCalendar2.Style.Item("display") = ""
                    divDateField2.Style.Item("display") = "none"
                    btnCal2.Text = "Fields"
                    btnCal2.ToolTip = "Choose field"
                    hdnBtnCal2.Value = "Fields"
                    Date2.Text = Data.DateValue2
                    If Data.DateRelative2 Then ckRelative2.Checked = True
                End If
            Else
                trText.Style.Item("display") = ""
                trCalendar.Style.Item("display") = "none"
                lblValue1.Style.Item("display") = "none"
                lblValue2.Style.Item("display") = ""
                divValue2.Style.Item("display") = ""
                If Data.FieldChosen1 Then
                    ddValField1.Style.Item("display") = ""
                    txtValue1.Style.Item("display") = "none"
                    btnText1.Text = "Text"
                    btnText1.ToolTip = "Enter text value"
                    hdnBtnText1.Value = "Text"
                    li = ddValField1.Items.FindByText(Data.FieldValue1)
                    ddValField1.SelectedIndex = ddValField1.Items.IndexOf(li)
                    hdnValField1.Value = Data.FieldValue1
                Else
                    ddValField1.Style.Item("display") = "none"
                    txtValue1.Style.Item("display") = ""
                    btnText1.Text = "Fields"
                    btnText1.ToolTip = "Choose field"
                    hdnBtnText1.Value = "Fields"
                    txtValue1.Text = Data.TextValue1
                End If
                If Data.FieldChosen2 Then
                    ddValField2.Style.Item("display") = ""
                    txtValue2.Style.Item("display") = "none"
                    btnText2.Text = "Text"
                    btnText2.ToolTip = "Enter text value"
                    hdnBtnText2.Value = "Text"
                    li = ddValField2.Items.FindByText(Data.FieldValue2)
                    ddValField2.SelectedIndex = ddValField2.Items.IndexOf(li)
                    hdnValField2.Value = Data.FieldValue2
                Else
                    ddValField2.Style.Item("display") = "none"
                    txtValue2.Style.Item("display") = ""
                    btnText2.Text = "Fields"
                    btnText2.ToolTip = "Choose field"
                    hdnBtnText2.Value = "Fields"
                    txtValue2.Text = Data.TextValue2
                End If
            End If
        End If
    End Sub
    Public Sub ShowData()
        If EntryMode = Mode.Add Then
            ddField.SelectedIndex = 0
            ddOperator.SelectedIndex = 0
            ddDateField1.SelectedIndex = 0
            ddDateField2.SelectedIndex = 0
            ddValField1.SelectedIndex = 0
            ddValField2.SelectedIndex = 0
            Date1.Text = ""
            hdnDateField1.Value = ""
            hdnDateField2.Value = ""
            hdnValField1.Value = ""
            hdnValField2.Value = ""
            Date1.Text = ""
            Date2.Text = ""
            txtValue1.Text = ""
            txtValue2.Text = ""
            ckNot.Checked = False
            ckRelative1.Checked = False
            ckRelative2.Checked = False
            trCalendar.Style.Item("display") = "none"
            trText.Style.Item("display") = ""
            lblValue1.Style.Item("display") = ""
            lblValue2.Style.Item("display") = "none"
            ddValField1.Style.Item("display") = "none"
            txtValue1.Style.Item("display") = ""
            divValue2.Style.Item("display") = "none"
            btnText1.Text = "Fields"
        Else
            If Data.Field = "" Then
                ddField.SelectedIndex = 0
            Else
                LoadConditionData()
            End If
        End If
    End Sub
    Public Sub Show(Caption As String, CondData As ConditionData, Optional CondEntryMode As Mode = Mode.Add, Optional OKButtonCaption As String = "OK")
        lblHeader.Text = Caption
        Data = CondData
        EntryMode = CondEntryMode
        hdnMode.Value = EntryMode.ToString
        OKCaption = OKButtonCaption

        ShowData()
        ddField.Focus()
        OldData = Data
        popDlg.Show()

    End Sub







#End Region



End Class
