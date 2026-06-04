Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.SessionState
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Module AnalysisExportSnapshot
    Private Const SnapshotSessionKey As String = "AnalysisExportSnapshots"
    Private Const SnapshotFolderSessionKey As String = "AnalysisExportSnapshotFolder"
    Private Const PendingSnapshotItemKey As String = "AnalysisExportPendingSnapshot"
    Private Const PendingSnapshotHandlerKey As String = "AnalysisExportPendingSnapshotHandler"

    Private Class PendingSnapshot
        Public Property ItemKey As String
        Public Property ItemName As String
        Public Property LabelText As String
        Public Property Controls As DataTable
        Public Property GridTable As DataTable
    End Class

    Public Sub Save(page As Page, itemKey As String, itemName As String, infoLabel As Label, grid As GridView, sourceTable As DataTable)
        If page Is Nothing OrElse page.Session Is Nothing Then Return
        If sourceTable Is Nothing Then Return
        If Not IsBuildPostback(page) Then Return

        Dim labelText As String = ""
        If infoLabel IsNot Nothing Then labelText = FieldText(infoLabel.Text)
        Dim controls As DataTable = ControlValues(page)
        Dim gridTable As DataTable = SnapshotGridTable(grid, sourceTable)

        QueueSnapshot(page, itemKey, itemName, labelText, controls, gridTable)
    End Sub

    Private Sub QueueSnapshot(page As Page, itemKey As String, itemName As String, labelText As String, controls As DataTable, gridTable As DataTable)
        If page Is Nothing Then Return
        Dim pending As New PendingSnapshot()
        pending.ItemKey = itemKey
        pending.ItemName = itemName
        pending.LabelText = labelText
        pending.Controls = controls
        pending.GridTable = gridTable
        page.Items(PendingSnapshotItemKey) = pending

        If page.Items(PendingSnapshotHandlerKey) Is Nothing Then
            page.Items(PendingSnapshotHandlerKey) = True
            AddHandler page.PreRenderComplete, AddressOf WritePendingSnapshot
        End If
    End Sub

    Private Sub WritePendingSnapshot(sender As Object, e As EventArgs)
        Dim page As Page = TryCast(sender, Page)
        If page Is Nothing OrElse page.Session Is Nothing Then Return
        Dim pending As PendingSnapshot = TryCast(page.Items(PendingSnapshotItemKey), PendingSnapshot)
        If pending Is Nothing Then Return
        If pending.GridTable Is Nothing Then Return

        Dim folderPath As String = SessionFolder(page.Session)
        If folderPath.Trim() = "" Then Return
        Directory.CreateDirectory(folderPath)

        Dim snapshotStamp As String = DateTime.Now.ToString("yyyyMMddHHmmssfff")
        Dim safeKey As String = SafeFilePart(pending.ItemKey)
        Dim fileName As String = safeKey & "_" & snapshotStamp & ".xls"
        Dim filePath As String = Path.Combine(folderPath, fileName)

        Dim signature As String = SnapshotSignature(page, pending.ItemKey, pending.LabelText, pending.Controls, pending.GridTable)
        If SnapshotExists(page.Session, signature) Then Return

        WriteSnapshotFile(page, filePath, pending.ItemName, pending.LabelText, pending.Controls, pending.GridTable)
        RegisterSnapshot(page.Session, pending.ItemKey & "_" & snapshotStamp, pending.ItemName & " - " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pending.LabelText, fileName, filePath, signature)
        page.Items.Remove(PendingSnapshotItemKey)
    End Sub

    Private Function IsBuildPostback(page As Page) As Boolean
        If page Is Nothing OrElse page.Request Is Nothing Then Return False
        If Not page.IsPostBack Then Return False
        Dim buildButton As Button = TryCast(FindControlRecursive(page, "ButtonBuild"), Button)
        If buildButton Is Nothing Then Return False
        If page.Request.Form(buildButton.UniqueID) IsNot Nothing Then Return True
        If page.Request.Form("__EVENTTARGET") IsNot Nothing AndAlso page.Request.Form("__EVENTTARGET").ToString() = buildButton.UniqueID Then Return True
        Return False
    End Function

    Private Function FindControlRecursive(control As Control, id As String) As Control
        If control Is Nothing Then Return Nothing
        If control.ID = id Then Return control
        For Each child As Control In control.Controls
            Dim found As Control = FindControlRecursive(child, id)
            If found IsNot Nothing Then Return found
        Next
        Return Nothing
    End Function

    Public Sub Cleanup(session As HttpSessionState)
        If session Is Nothing Then Return
        Dim folderPath As String = ""
        If session(SnapshotFolderSessionKey) IsNot Nothing Then folderPath = session(SnapshotFolderSessionKey).ToString()
        If folderPath.Trim() <> "" AndAlso Directory.Exists(folderPath) Then
            Try
                Directory.Delete(folderPath, True)
            Catch ex As Exception
            End Try
        End If
        session(SnapshotSessionKey) = Nothing
        session(SnapshotFolderSessionKey) = Nothing
    End Sub

    Public Sub CleanupOldFolders()
        Dim tempPath As String = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory(), "Temp")
        If Not Directory.Exists(tempPath) Then Return
        For Each folderPath As String In Directory.GetDirectories(tempPath, "AnalysisSnapshots_*")
            Try
                Dim info As New DirectoryInfo(folderPath)
                If info.LastWriteTime < DateTime.Now.AddHours(-2) Then info.Delete(True)
            Catch ex As Exception
            End Try
        Next
    End Sub

    Public Function SnapshotTable(session As HttpSessionState) As DataTable
        If session Is Nothing Then Return EmptySnapshotTable()
        Dim dt As DataTable = TryCast(session(SnapshotSessionKey), DataTable)
        If dt Is Nothing Then
            dt = EmptySnapshotTable()
            session(SnapshotSessionKey) = dt
        End If
        EnsureSnapshotColumns(dt)
        Return dt
    End Function

    Private Function EmptySnapshotTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Key", GetType(String))
        dt.Columns.Add("Included", GetType(Boolean))
        dt.Columns.Add("Package Item", GetType(String))
        dt.Columns.Add("Label Above Grid", GetType(String))
        dt.Columns.Add("File", GetType(String))
        dt.Columns.Add("Description", GetType(String))
        dt.Columns.Add("FullPath", GetType(String))
        dt.Columns.Add("Signature", GetType(String))
        Return dt
    End Function

    Private Sub EnsureSnapshotColumns(dt As DataTable)
        If dt Is Nothing Then Return
        If Not dt.Columns.Contains("Signature") Then dt.Columns.Add("Signature", GetType(String))
    End Sub

    Private Function SessionFolder(session As HttpSessionState) As String
        If session(SnapshotFolderSessionKey) IsNot Nothing AndAlso session(SnapshotFolderSessionKey).ToString().Trim() <> "" Then
            Return session(SnapshotFolderSessionKey).ToString()
        End If

        Dim basePath As String = System.AppDomain.CurrentDomain.BaseDirectory()
        Dim tempPath As String = Path.Combine(basePath, "Temp")
        CleanupOldFolders()
        Dim sessionName As String = "AnalysisSnapshots_" & SafeFilePart(session.SessionID)
        Dim logonText As String = ""
        If session("logon") IsNot Nothing Then logonText = SafeFilePart(session("logon").ToString())
        If logonText.Trim() <> "" Then sessionName &= "_" & logonText
        Dim folderPath As String = Path.Combine(tempPath, sessionName)
        session(SnapshotFolderSessionKey) = folderPath
        Return folderPath
    End Function

    Private Function SnapshotExists(session As HttpSessionState, signature As String) As Boolean
        Dim dt As DataTable = SnapshotTable(session)
        EnsureSnapshotColumns(dt)
        For Each existingRow As DataRow In dt.Rows
            If String.Equals(existingRow("Signature").ToString(), signature, StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function

    Private Sub RegisterSnapshot(session As HttpSessionState, itemKey As String, itemName As String, labelText As String, fileName As String, filePath As String, signature As String)
        Dim dt As DataTable = SnapshotTable(session)
        EnsureSnapshotColumns(dt)
        For Each existingRow As DataRow In dt.Rows
            If String.Equals(existingRow("FullPath").ToString(), filePath, StringComparison.OrdinalIgnoreCase) Then Return
            If String.Equals(existingRow("Signature").ToString(), signature, StringComparison.OrdinalIgnoreCase) Then Return
        Next
        Dim row As DataRow = dt.NewRow()
        row("Key") = itemKey
        row("Included") = True
        row("Package Item") = itemName
        row("Label Above Grid") = labelText
        row("File") = fileName
        row("Description") = "Excel snapshot created by the Build button from the page controls, label above grid, and result grid."
        row("FullPath") = filePath
        row("Signature") = signature
        dt.Rows.Add(row)
    End Sub

    Private Sub WriteSnapshotFile(page As Page, filePath As String, itemName As String, labelText As String, controls As DataTable, gridTable As DataTable)
        Dim sb As New StringBuilder()
        sb.AppendLine("<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" /></head><body style=""font-family:Arial"">")
        sb.AppendLine("<h2>" & HtmlEncode(itemName) & "</h2>")
        sb.AppendLine("<table border=""1"" cellspacing=""0"" cellpadding=""4"">")
        AppendInfoRow(sb, "Created", DateTime.Now.ToString())
        AppendInfoRow(sb, "Report", SessionText(page, "REPORTID"))
        AppendInfoRow(sb, "Report Title", SessionText(page, "REPTITLE"))
        If labelText.Trim() <> "" Then AppendInfoRow(sb, "Label Above Grid", labelText)
        sb.AppendLine("</table>")

        If controls.Rows.Count > 0 Then
            sb.AppendLine("<h3>Controls</h3>")
            AppendDataTableHtml(sb, controls)
        End If

        sb.AppendLine("<h3>Grid</h3>")
        AppendDataTableHtml(sb, gridTable)
        sb.AppendLine("</body></html>")

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
    End Sub

    Private Function SnapshotSignature(page As Page, itemKey As String, labelText As String, controls As DataTable, gridTable As DataTable) As String
        Dim sb As New StringBuilder()
        sb.AppendLine(itemKey)
        sb.AppendLine(SessionText(page, "REPORTID"))
        sb.AppendLine(labelText)
        AppendTableSignature(sb, controls)
        AppendTableSignature(sb, gridTable)
        Using sha As SHA256 = SHA256.Create()
            Dim bytes() As Byte = Encoding.UTF8.GetBytes(sb.ToString())
            Dim hash() As Byte = sha.ComputeHash(bytes)
            Return BitConverter.ToString(hash).Replace("-", "")
        End Using
    End Function

    Private Sub AppendTableSignature(sb As StringBuilder, dt As DataTable)
        If dt Is Nothing Then Return
        For Each col As DataColumn In dt.Columns
            sb.Append(col.ColumnName).Append(ChrW(30))
        Next
        sb.AppendLine()
        For Each row As DataRow In dt.Rows
            For Each col As DataColumn In dt.Columns
                sb.Append(FieldText(row(col))).Append(ChrW(30))
            Next
            sb.AppendLine()
        Next
    End Sub

    Private Sub AppendInfoRow(sb As StringBuilder, labelText As String, valueText As String)
        sb.AppendLine("<tr><th align=""left"">" & HtmlEncode(labelText) & "</th><td>" & HtmlEncode(valueText) & "</td></tr>")
    End Sub

    Private Function ControlValues(page As Page) As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Control", GetType(String))
        dt.Columns.Add("Value", GetType(String))
        Dim values As New List(Of Tuple(Of String, String))()
        CollectControlValues(page, values)
        For Each item As Tuple(Of String, String) In values
            If item.Item2.Trim() = "" Then Continue For
            Dim row As DataRow = dt.NewRow()
            row("Control") = item.Item1
            row("Value") = item.Item2
            dt.Rows.Add(row)
        Next
        Return dt
    End Function

    Private Sub CollectControlValues(control As Control, values As List(Of Tuple(Of String, String)))
        If control Is Nothing Then Return
        Dim webControl As WebControl = TryCast(control, WebControl)
        If webControl IsNot Nothing AndAlso Not webControl.Visible Then Return
        If TypeOf control Is System.Web.UI.WebControls.GridView OrElse TypeOf control Is System.Web.UI.WebControls.TreeView Then Return

        Dim controlName As String = If(control.ID, control.GetType().Name)
        If TypeOf control Is TextBox Then
            values.Add(Tuple.Create(controlName, CType(control, TextBox).Text))
        ElseIf TypeOf control Is DropDownList Then
            Dim ddl As DropDownList = CType(control, DropDownList)
            values.Add(Tuple.Create(controlName, SelectedItemText(ddl)))
        ElseIf TypeOf control Is ListBox Then
            Dim lb As ListBox = CType(control, ListBox)
            Dim selected As New List(Of String)()
            For Each item As ListItem In lb.Items
                If item.Selected Then selected.Add(item.Text)
            Next
            values.Add(Tuple.Create(controlName, String.Join(", ", selected.ToArray())))
        ElseIf TypeOf control Is CheckBox Then
            Dim chk As CheckBox = CType(control, CheckBox)
            values.Add(Tuple.Create(controlName, If(chk.Checked, "Checked", "Not checked")))
        ElseIf TypeOf control Is RadioButtonList Then
            Dim rbl As RadioButtonList = CType(control, RadioButtonList)
            values.Add(Tuple.Create(controlName, SelectedItemText(rbl)))
        ElseIf TypeOf control Is FileUpload Then
            Dim upload As FileUpload = CType(control, FileUpload)
            values.Add(Tuple.Create(controlName, upload.FileName))
        End If

        For Each child As Control In control.Controls
            CollectControlValues(child, values)
        Next
    End Sub

    Private Function SelectedItemText(listControl As ListControl) As String
        If listControl Is Nothing OrElse listControl.SelectedItem Is Nothing Then Return ""
        Return listControl.SelectedItem.Text
    End Function

    Private Function SnapshotGridTable(grid As GridView, sourceTable As DataTable) As DataTable
        Dim dt As DataTable = Nothing
        If sourceTable IsNot Nothing Then dt = sourceTable.Copy()
        If dt Is Nothing Then dt = New DataTable()

        For Each hiddenName As String In New String() {"FilterId", "BaseFilterId", "CompareFilterId"}
            If dt.Columns.Contains(hiddenName) Then dt.Columns.Remove(hiddenName)
        Next
        Return dt
    End Function

    Private Sub AppendDataTableHtml(sb As StringBuilder, dt As DataTable)
        sb.AppendLine("<table border=""1"" cellspacing=""0"" cellpadding=""4"">")
        sb.AppendLine("<tr>")
        For Each col As DataColumn In dt.Columns
            sb.AppendLine("<th>" & HtmlEncode(col.ColumnName) & "</th>")
        Next
        sb.AppendLine("</tr>")
        For Each row As DataRow In dt.Rows
            sb.AppendLine("<tr>")
            For Each col As DataColumn In dt.Columns
                sb.AppendLine("<td>" & HtmlEncode(FieldText(row(col))) & "</td>")
            Next
            sb.AppendLine("</tr>")
        Next
        sb.AppendLine("</table>")
    End Sub

    Private Function SessionText(page As Page, key As String) As String
        If page Is Nothing OrElse page.Session Is Nothing OrElse page.Session(key) Is Nothing Then Return ""
        Return page.Session(key).ToString()
    End Function

    Private Function SafeFilePart(valueText As String) As String
        If valueText Is Nothing OrElse valueText.Trim() = "" Then Return "Snapshot"
        Dim safeText As String = valueText.Trim()
        For Each invalidChar As Char In Path.GetInvalidFileNameChars()
            safeText = safeText.Replace(invalidChar, "_"c)
        Next
        safeText = safeText.Replace(" "c, "_"c)
        Return safeText
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        Return valueObject.ToString()
    End Function

    Private Function HtmlEncode(valueText As String) As String
        Return HttpUtility.HtmlEncode(FieldText(valueText))
    End Function
End Module
