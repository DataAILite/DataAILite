Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public NotInheritable Class UrlInputHelper
    Private Sub New()
    End Sub

    Public Shared Function HasParam(page As Page, paramName As String) As Boolean
        Return page IsNot Nothing AndAlso page.Request IsNot Nothing AndAlso page.Request(paramName) IsNot Nothing
    End Function

    Public Shared Function Param(page As Page, paramName As String) As String
        If Not HasParam(page, paramName) Then Return ""
        Return page.Request(paramName).ToString().Trim()
    End Function

    Public Shared Sub ApplySession(page As Page, paramName As String, sessionName As String)
        If page Is Nothing OrElse page.Session Is Nothing OrElse Not HasParam(page, paramName) Then Exit Sub
        page.Session(sessionName) = Param(page, paramName)
    End Sub

    Public Shared Sub ApplyReportSession(page As Page)
        If page Is Nothing OrElse page.Session Is Nothing OrElse page.Request Is Nothing Then Exit Sub
        Dim reportKeys() As String = {"Report", "REPORT", "ReportID", "REPORTID", "repid"}
        For Each key As String In reportKeys
            If HasParam(page, key) AndAlso Param(page, key) <> "" Then
                page.Session("REPORTID") = Param(page, key)
                Exit Sub
            End If
        Next
    End Sub

    Public Shared Sub ApplyDropDown(page As Page, controlId As String, paramName As String)
        If Not HasParam(page, paramName) Then Exit Sub
        Dim ddl As DropDownList = TryCast(FindControlRecursive(page, controlId), DropDownList)
        If ddl Is Nothing Then Exit Sub
        SelectDropDownValue(ddl, Param(page, paramName))
    End Sub

    Public Shared Sub ApplyTextBox(page As Page, controlId As String, paramName As String)
        If Not HasParam(page, paramName) Then Exit Sub
        Dim txt As TextBox = TryCast(FindControlRecursive(page, controlId), TextBox)
        If txt Is Nothing Then Exit Sub
        txt.Text = Param(page, paramName)
    End Sub

    Public Shared Sub ApplyCheckBox(page As Page, controlId As String, paramName As String)
        If Not HasParam(page, paramName) Then Exit Sub
        Dim chk As CheckBox = TryCast(FindControlRecursive(page, controlId), CheckBox)
        If chk Is Nothing Then Exit Sub
        chk.Checked = IsTrueValue(Param(page, paramName))
    End Sub

    Public Shared Sub ApplyListBox(page As Page, controlId As String, paramName As String)
        If Not HasParam(page, paramName) Then Exit Sub
        Dim list As ListBox = TryCast(FindControlRecursive(page, controlId), ListBox)
        If list Is Nothing Then Exit Sub
        SelectListValues(list, Param(page, paramName))
    End Sub

    Public Shared Sub ApplyMultiSelectControl(page As Page, controlId As String, paramName As String)
        If Not HasParam(page, paramName) Then Exit Sub
        Dim ctl As Control = FindControlRecursive(page, controlId)
        If ctl Is Nothing Then Exit Sub
        Dim prop = ctl.GetType().GetProperty("SelectedItemsString")
        If prop IsNot Nothing AndAlso prop.CanWrite Then prop.SetValue(ctl, Param(page, paramName), Nothing)
    End Sub

    Public Shared Sub SelectDropDownValue(ddl As DropDownList, valueText As String)
        If ddl Is Nothing OrElse valueText Is Nothing Then Exit Sub
        valueText = valueText.Trim()
        Dim item As ListItem = ddl.Items.FindByValue(valueText)
        If item Is Nothing Then item = ddl.Items.FindByText(valueText)
        If item Is Nothing Then Exit Sub
        ddl.ClearSelection()
        item.Selected = True
    End Sub

    Private Shared Sub SelectListValues(list As ListBox, valuesText As String)
        If list Is Nothing OrElse valuesText Is Nothing Then Exit Sub
        Dim values() As String = valuesText.Split(","c)
        For Each item As ListItem In list.Items
            item.Selected = False
        Next
        For Each rawValue As String In values
            Dim value As String = rawValue.Trim()
            If value = "" Then Continue For
            Dim item As ListItem = list.Items.FindByValue(value)
            If item Is Nothing Then item = list.Items.FindByText(value)
            If item IsNot Nothing Then item.Selected = True
        Next
    End Sub

    Private Shared Function IsTrueValue(value As String) As Boolean
        value = If(value, "").Trim().ToLowerInvariant()
        Return value = "1" OrElse value = "true" OrElse value = "yes" OrElse value = "on"
    End Function

    Private Shared Function FindControlRecursive(root As Control, id As String) As Control
        If root Is Nothing Then Return Nothing
        If String.Equals(root.ID, id, StringComparison.OrdinalIgnoreCase) Then Return root
        For Each child As Control In root.Controls
            Dim found As Control = FindControlRecursive(child, id)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function
End Class
