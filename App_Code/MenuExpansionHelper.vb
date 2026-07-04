Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public NotInheritable Class MenuExpansionHelper
    Private Sub New()
    End Sub

    Public Shared Sub Attach(page As Page)
        If page Is Nothing Then Exit Sub
        Dim tree As System.Web.UI.WebControls.TreeView = TryCast(FindControlRecursive(page, "TreeView1"), System.Web.UI.WebControls.TreeView)
        Attach(tree)
    End Sub

    Public Shared Sub Attach(tree As System.Web.UI.WebControls.TreeView)
        If tree Is Nothing Then Exit Sub

        RemoveHandler tree.TreeNodeExpanded, AddressOf TreeNodeExpanded
        RemoveHandler tree.TreeNodeCollapsed, AddressOf TreeNodeCollapsed
        AddHandler tree.TreeNodeExpanded, AddressOf TreeNodeExpanded
        AddHandler tree.TreeNodeCollapsed, AddressOf TreeNodeCollapsed
        ApplySavedState(tree)
    End Sub

    Private Shared Sub TreeNodeExpanded(sender As Object, e As System.Web.UI.WebControls.TreeNodeEventArgs)
        SaveNodeState(e.Node, True)
    End Sub

    Private Shared Sub TreeNodeCollapsed(sender As Object, e As System.Web.UI.WebControls.TreeNodeEventArgs)
        SaveNodeState(e.Node, False)
    End Sub

    Private Shared Sub SaveNodeState(node As System.Web.UI.WebControls.TreeNode, expanded As Boolean)
        Dim key As String = SessionKey(node)
        If key = "" OrElse HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing Then Exit Sub
        HttpContext.Current.Session(key) = If(expanded, "1", "0")
    End Sub

    Public Shared Sub ApplySavedState(tree As System.Web.UI.WebControls.TreeView)
        For Each node As System.Web.UI.WebControls.TreeNode In tree.Nodes
            ApplySavedState(node)
        Next
    End Sub

    Private Shared Sub ApplySavedState(node As System.Web.UI.WebControls.TreeNode)
        Dim key As String = SessionKey(node)
        If key <> "" Then
            Dim savedValue As Object = Nothing
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then savedValue = HttpContext.Current.Session(key)
            If savedValue Is Nothing OrElse savedValue.ToString().Trim() = "" Then
                node.Expanded = False
            Else
                node.Expanded = savedValue.ToString().Trim() = "1"
            End If
        End If

        For Each child As System.Web.UI.WebControls.TreeNode In node.ChildNodes
            ApplySavedState(child)
        Next
    End Sub

    Private Shared Function SessionKey(node As System.Web.UI.WebControls.TreeNode) As String
        If node Is Nothing OrElse node.ChildNodes.Count = 0 Then Return ""
        Dim text As String = CleanText(node.Text)
        If text = "Reports" Then Return "LeftMenuExpanded_Reports"
        If text = "Analytics Dashboard" Then Return "LeftMenuExpanded_AnalyticsDashboard"
        If text = "Data Quality Dashboard" Then Return "LeftMenuExpanded_DataQualityDashboard"
        If text = "Market Dashboard" Then Return "LeftMenuExpanded_MarketDashboard"
        Return ""
    End Function

    Private Shared Function CleanText(value As String) As String
        If value Is Nothing Then Return ""
        Dim text As String = value.Replace("&lt;b&gt;", "").Replace("&lt;/b&gt;", "")
        text = text.Replace("<b>", "").Replace("</b>", "")
        Return text.Trim()
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
