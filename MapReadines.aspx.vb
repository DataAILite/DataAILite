Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Globalization
Imports System.Text
Imports System.Web.UI.WebControls

Partial Class MapReadines
    Inherits System.Web.UI.Page

    Private Sub MapReadines_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Session("PAGETTL") IsNot Nothing AndAlso Session("PAGETTL").ToString().Trim() <> "" Then LabelPageTtl.Text = Session("PAGETTL").ToString()
        If Request("Report") IsNot Nothing AndAlso Request("Report").ToString().Trim() <> "" Then Session("REPORTID") = Request("Report").ToString().Trim()
        If Session("REPTITLE") IsNot Nothing AndAlso Session("REPTITLE").ToString().Trim() <> "" Then lblHeader.Text = Session("REPTITLE").ToString() & " - Map Readiness"
        HyperLinkHelp.NavigateUrl = "DataAIHelp.aspx?hilt=Map%20Readiness"
    End Sub

    Private Sub MapReadines_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString() = "" Then Response.Redirect("~/Default.aspx?msg=SessionExpired")
        If Not IsPostBack Then
            LoadReportData()
            FillFieldLists()
            BuildAndBindReadiness()
        ElseIf Session("MapReadinesTable") IsNot Nothing Then
            BuildAutomaticMapSuitability()
            BindReadiness(CType(Session("MapReadinesTable"), DataTable))
        End If
    End Sub

    Private Function LoadReportData() As DataTable
        LabelError.Text = ""
        Dim ret As String = ""
        Dim repid As String = ""
        If Session("REPORTID") IsNot Nothing Then repid = Session("REPORTID").ToString()

        If repid.Trim() = "" Then
            LabelError.Text = "Report is not selected."
            Return Nothing
        End If

        Dim dv As DataView = Nothing
        Try
            dv = RetrieveReportData(repid, "", 1, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret, "")
        Catch ex As Exception
            LabelError.Text = "ERROR!! " & ex.Message
            Return Nothing
        End Try

        If ret.Trim() <> "" Then LabelError.Text = ret
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            LabelError.Text = "No data. Run or import report data first."
            Session("MapReadinesSource") = Nothing
            Return Nothing
        End If
        Session("dv3") = dv
        Session("MapReadinesSource") = dv.Table
        Return dv.Table
    End Function

    Private Function GetSourceTable() As DataTable
        If Session("MapReadinesSource") Is Nothing Then Return LoadReportData()
        Return CType(Session("MapReadinesSource"), DataTable)
    End Function

    Private Sub FillFieldLists()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing Then Exit Sub

        DropDownLatitude.Items.Clear()
        DropDownLongitude.Items.Clear()
        DropDownNameField.Items.Clear()
        DropDownLatitude.Items.Add(New ListItem("(Select)", ""))
        DropDownLongitude.Items.Add(New ListItem("(Select)", ""))
        DropDownNameField.Items.Add(New ListItem("(None)", ""))

        For Each col As DataColumn In source.Columns
            DropDownLatitude.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            DropDownLongitude.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
            DropDownNameField.Items.Add(New ListItem(col.ColumnName, col.ColumnName))
        Next

        SelectFieldByGuess(DropDownLatitude, source, True)
        SelectFieldByGuess(DropDownLongitude, source, False)
        SelectNameField(source)
    End Sub

    Private Sub SelectFieldByGuess(dropDown As DropDownList, source As DataTable, latitude As Boolean)
        For Each col As DataColumn In source.Columns
            Dim name As String = col.ColumnName.ToLowerInvariant()
            Dim isMatch As Boolean
            If latitude Then
                isMatch = (name = "lat" OrElse name.Contains("latitude") OrElse name.Contains(" lat") OrElse name.EndsWith("lat"))
            Else
                isMatch = (name = "lon" OrElse name = "lng" OrElse name.Contains("longitude") OrElse name.Contains(" lon") OrElse name.Contains(" lng") OrElse name.EndsWith("lon") OrElse name.EndsWith("lng"))
            End If
            If isMatch AndAlso dropDown.Items.FindByValue(col.ColumnName) IsNot Nothing Then
                dropDown.SelectedValue = col.ColumnName
                Exit Sub
            End If
        Next
    End Sub

    Private Function FindCoordinateColumn(source As DataTable, latitude As Boolean, excludeColumnName As String) As DataColumn
        If source Is Nothing Then Return Nothing
        For Each col As DataColumn In source.Columns
            If IsIdentifierLikeField(col.ColumnName) Then Continue For
            If String.Equals(col.ColumnName, excludeColumnName, StringComparison.OrdinalIgnoreCase) Then Continue For
            Dim name As String = col.ColumnName.ToLowerInvariant()
            Dim isNameMatch As Boolean
            If latitude Then
                isNameMatch = (name = "lat" OrElse name.Contains("latitude") OrElse name.Contains(" lat") OrElse name.EndsWith("lat"))
            Else
                isNameMatch = (name = "lon" OrElse name = "lng" OrElse name.Contains("longitude") OrElse name.Contains(" lon") OrElse name.Contains(" lng") OrElse name.EndsWith("lon") OrElse name.EndsWith("lng"))
            End If
            If isNameMatch AndAlso CoordinateColumnHasUsableValues(source, col, latitude) Then Return col
        Next

        For Each col As DataColumn In source.Columns
            If IsIdentifierLikeField(col.ColumnName) Then Continue For
            If String.Equals(col.ColumnName, excludeColumnName, StringComparison.OrdinalIgnoreCase) Then Continue For
            If CoordinateColumnHasUsableValues(source, col, latitude) Then Return col
        Next
        Return Nothing
    End Function

    Private Function CoordinateColumnHasUsableValues(source As DataTable, col As DataColumn, latitude As Boolean) As Boolean
        Dim checkedRows As Integer = 0
        Dim validRows As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = FieldText(row(col)).Trim()
            If text = "" Then Continue For
            checkedRows += 1
            Dim value As Double
            If TryParseCoordinate(text, value) Then
                If latitude AndAlso value >= -90 AndAlso value <= 90 Then validRows += 1
                If Not latitude AndAlso value >= -180 AndAlso value <= 180 Then validRows += 1
            End If
            If checkedRows >= 50 Then Exit For
        Next
        Return validRows > 0 AndAlso validRows >= Math.Max(1, CInt(Math.Ceiling(checkedRows * 0.6)))
    End Function

    Private Function SuggestedCoordinateFields(source As DataTable, latitude As Boolean) As String
        If source Is Nothing Then Return ""
        Dim suggestions As New List(Of String)()
        For Each col As DataColumn In source.Columns
            If IsIdentifierLikeField(col.ColumnName) Then Continue For
            Dim score As Integer = CoordinateSuggestionScore(source, col, latitude)
            If score > 0 Then suggestions.Add(col.ColumnName & " (" & score.ToString() & "%)")
        Next
        If suggestions.Count = 0 Then Return ""
        Return String.Join("; ", suggestions.ToArray())
    End Function

    Private Function CoordinateSuggestionScore(source As DataTable, col As DataColumn, latitude As Boolean) As Integer
        If IsIdentifierLikeField(col.ColumnName) Then Return 0
        Dim name As String = col.ColumnName.ToLowerInvariant()
        Dim nameMatch As Boolean
        If latitude Then
            nameMatch = (name = "lat" OrElse name.Contains("latitude") OrElse name.Contains(" lat") OrElse name.EndsWith("lat"))
        Else
            nameMatch = (name = "lon" OrElse name = "lng" OrElse name.Contains("longitude") OrElse name.Contains(" lon") OrElse name.Contains(" lng") OrElse name.EndsWith("lon") OrElse name.EndsWith("lng"))
        End If

        Dim checkedRows As Integer = 0
        Dim validRows As Integer = 0
        For Each row As DataRow In source.Rows
            Dim text As String = FieldText(row(col)).Trim()
            If text = "" Then Continue For
            checkedRows += 1
            Dim value As Double
            If TryParseCoordinate(text, value) Then
                If latitude AndAlso value >= -90 AndAlso value <= 90 Then validRows += 1
                If Not latitude AndAlso value >= -180 AndAlso value <= 180 Then validRows += 1
            End If
            If checkedRows >= 100 Then Exit For
        Next

        If checkedRows = 0 OrElse validRows = 0 Then Return 0
        Dim score As Integer = CInt(Math.Round((validRows / checkedRows) * 100))
        If nameMatch Then score = Math.Min(100, score + 10)
        If score < 60 AndAlso Not nameMatch Then Return 0
        Return score
    End Function

    Private Function IsIdentifierLikeField(columnName As String) As Boolean
        Dim name As String = columnName.Trim().ToLowerInvariant().Replace("_", "").Replace(" ", "")
        If name = "id" OrElse name = "ind" OrElse name = "indx" OrElse name = "inx" OrElse name = "index" Then Return True
        If name.EndsWith("id") OrElse name.EndsWith("ids") OrElse name.EndsWith("index") OrElse name.EndsWith("indx") OrElse name.EndsWith("inx") Then Return True
        If name.Contains("indx") OrElse name.Contains("inx") OrElse name.Contains("index") Then Return True
        If name.Contains("recordid") OrElse name.Contains("rowid") OrElse name.Contains("objectid") Then Return True
        Return False
    End Function

    Private Function FindNameColumn(source As DataTable) As DataColumn
        Dim preferred() As String = {"name", "placemark", "title", "location", "address", "city"}
        For Each key As String In preferred
            For Each col As DataColumn In source.Columns
                If col.ColumnName.ToLowerInvariant().Contains(key) Then Return col
            Next
        Next
        Return Nothing
    End Function

    Private Sub SelectNameField(source As DataTable)
        Dim preferred() As String = {"name", "placemark", "title", "location", "address", "city"}
        For Each key As String In preferred
            For Each col As DataColumn In source.Columns
                If col.ColumnName.ToLowerInvariant().Contains(key) AndAlso DropDownNameField.Items.FindByValue(col.ColumnName) IsNot Nothing Then
                    DropDownNameField.SelectedValue = col.ColumnName
                    Exit Sub
                End If
            Next
        Next
    End Sub

    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As System.Web.UI.WebControls.TreeNode = TreeView1.SelectedNode
        If node IsNot Nothing AndAlso node.Value IsNot Nothing AndAlso node.Value.Trim() <> "" Then Response.Redirect(node.Value)
    End Sub

    Private Sub ButtonBuild_Click(sender As Object, e As EventArgs) Handles ButtonBuild.Click
        BuildAndBindReadiness()
    End Sub

    Private Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        txtSearch.Text = ""
        FillFieldLists()
        BuildAndBindReadiness()
    End Sub

    Private Sub ButtonExportCSV_Click(sender As Object, e As EventArgs) Handles ButtonExportCSV.Click
        ExportReadiness("csv")
    End Sub

    Private Sub ButtonExportExcel_Click(sender As Object, e As EventArgs) Handles ButtonExportExcel.Click
        ExportReadiness("xls")
    End Sub

    Private Sub BuildAndBindReadiness()
        LabelError.Text = ""
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then
            BuildAutomaticMapSuitability()
            Exit Sub
        End If
        BuildAutomaticMapSuitability()

        Dim latField As String = DropDownLatitude.SelectedValue.Trim()
        Dim lonField As String = DropDownLongitude.SelectedValue.Trim()
        Dim nameField As String = DropDownNameField.SelectedValue.Trim()

        Dim results As DataTable = CreateResultTable()
        Session("MapReadinesFilters") = New Dictionary(Of String, String)()
        If latField = "" OrElse lonField = "" Then
            AddFinding(results, "Required Fields", "Fail", "", "", source.Rows.Count, "Select latitude and longitude fields before checking map readiness.", "No", RowsFilter(AllRows(source)))
            Session("MapReadinesTable") = results
            BindReadiness(results)
            Exit Sub
        End If

        Dim missingCoordinates As Integer = 0
        Dim invalidCoordinates As Integer = 0
        Dim validCoordinates As Integer = 0
        Dim missingNames As Integer = 0
        Dim duplicateCoordinates As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Dim duplicateRowsByCoordinate As New Dictionary(Of String, List(Of DataRow))(StringComparer.OrdinalIgnoreCase)
        Dim missingCoordinateRows As New List(Of DataRow)()
        Dim invalidCoordinateRows As New List(Of DataRow)()
        Dim validCoordinateRows As New List(Of DataRow)()
        Dim missingNameRows As New List(Of DataRow)()
        Dim searchText As String = txtSearch.Text.Trim().ToLowerInvariant()

        For Each row As DataRow In source.Rows
            If searchText <> "" AndAlso Not RowMatchesSearch(row, searchText) Then Continue For

            Dim latText As String = FieldText(row(latField)).Trim()
            Dim lonText As String = FieldText(row(lonField)).Trim()
            If latText = "" OrElse lonText = "" Then
                missingCoordinates += 1
                missingCoordinateRows.Add(row)
                Continue For
            End If

            Dim lat As Double
            Dim lon As Double
            If Not TryParseCoordinate(latText, lat) OrElse Not TryParseCoordinate(lonText, lon) OrElse lat < -90 OrElse lat > 90 OrElse lon < -180 OrElse lon > 180 Then
                invalidCoordinates += 1
                invalidCoordinateRows.Add(row)
                Continue For
            End If

            validCoordinates += 1
            validCoordinateRows.Add(row)
            Dim coordinateKey As String = Math.Round(lat, 6).ToString(CultureInfo.InvariantCulture) & "," & Math.Round(lon, 6).ToString(CultureInfo.InvariantCulture)
            If Not duplicateCoordinates.ContainsKey(coordinateKey) Then duplicateCoordinates(coordinateKey) = 0
            duplicateCoordinates(coordinateKey) += 1
            If Not duplicateRowsByCoordinate.ContainsKey(coordinateKey) Then duplicateRowsByCoordinate(coordinateKey) = New List(Of DataRow)()
            duplicateRowsByCoordinate(coordinateKey).Add(row)

            If nameField <> "" AndAlso FieldText(row(nameField)).Trim() = "" Then
                missingNames += 1
                missingNameRows.Add(row)
            End If
        Next

        Dim duplicatePairs As Integer = 0
        Dim duplicateRecords As Integer = 0
        Dim duplicateRows As New List(Of DataRow)()
        For Each item As KeyValuePair(Of String, Integer) In duplicateCoordinates
            If item.Value > 1 Then
                duplicatePairs += 1
                duplicateRecords += item.Value
                If duplicateRowsByCoordinate.ContainsKey(item.Key) Then duplicateRows.AddRange(duplicateRowsByCoordinate(item.Key))
            End If
        Next

        AddFinding(results, "Missing Coordinates", If(missingCoordinates = 0, "Pass", "Warning"), latField & ", " & lonField, "", missingCoordinates, "Rows where latitude or longitude is blank.", If(missingCoordinates = 0, "Yes", "No"), RowsFilter(missingCoordinateRows))
        AddFinding(results, "Invalid Coordinate Range", If(invalidCoordinates = 0, "Pass", "Fail"), latField & ", " & lonField, "Latitude -90 to 90; longitude -180 to 180", invalidCoordinates, "Rows with nonnumeric coordinates or values outside valid map ranges.", If(invalidCoordinates = 0, "Yes", "No"), RowsFilter(invalidCoordinateRows))
        AddFinding(results, "Duplicate Locations", If(duplicatePairs = 0, "Pass", "Warning"), latField & ", " & lonField, duplicatePairs.ToString() & " coordinate pairs", duplicateRecords, "Records sharing the same rounded latitude/longitude pair.", "Review", RowsFilter(duplicateRows))
        AddFinding(results, "Placemark Names", If(nameField = "", "Warning", If(missingNames = 0, "Pass", "Warning")), nameField, "", missingNames, "KML placemarks are clearer when each valid coordinate has a name or label.", If(nameField <> "" AndAlso missingNames = 0, "Yes", "Review"), RowsFilter(missingNameRows))
        AddFinding(results, "KML Ready Data", If(validCoordinates > 0 AndAlso missingCoordinates = 0 AndAlso invalidCoordinates = 0, "Pass", "Warning"), latField & ", " & lonField, validCoordinates.ToString() & " valid records", validCoordinates, "Records with valid latitude and longitude can be used for KML-ready map output.", If(validCoordinates > 0 AndAlso missingCoordinates = 0 AndAlso invalidCoordinates = 0, "Yes", "Partial"), CoordinateRangeFilter(source.Columns(latField), source.Columns(lonField)))

        Session("MapReadinesTable") = results
        BindReadiness(results)
    End Sub

    Private Sub BuildAutomaticMapSuitability()
        Dim source As DataTable = GetSourceTable()
        If source Is Nothing OrElse source.Rows.Count = 0 Then
            litMapSuitability.Text = RenderMapSuitability("Not enough data", "", "", "", "", 0, 0, 0, 0, 0, 0, "No report data is available for map readiness analysis.")
            Exit Sub
        End If

        Dim latCol As DataColumn = FindCoordinateColumn(source, True, "")
        Dim lonCol As DataColumn = FindCoordinateColumn(source, False, If(latCol Is Nothing, "", latCol.ColumnName))
        If latCol Is Nothing OrElse lonCol Is Nothing Then
            litMapSuitability.Text = RenderMapSuitability("Not enough data", If(latCol Is Nothing, "", latCol.ColumnName), If(lonCol Is Nothing, "", lonCol.ColumnName), SuggestedCoordinateFields(source, True), SuggestedCoordinateFields(source, False), source.Rows.Count, 0, 0, 0, 0, 0, "Could not find a usable latitude and longitude field pair in the report data.")
            Exit Sub
        End If

        Dim nameCol As DataColumn = FindNameColumn(source)
        Dim missingCoordinates As Integer = 0
        Dim invalidCoordinates As Integer = 0
        Dim validCoordinates As Integer = 0
        Dim missingNames As Integer = 0
        Dim duplicateCoordinates As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For Each row As DataRow In source.Rows
            Dim latText As String = FieldText(row(latCol)).Trim()
            Dim lonText As String = FieldText(row(lonCol)).Trim()
            If latText = "" OrElse lonText = "" Then
                missingCoordinates += 1
                Continue For
            End If

            Dim lat As Double
            Dim lon As Double
            If Not TryParseCoordinate(latText, lat) OrElse Not TryParseCoordinate(lonText, lon) OrElse lat < -90 OrElse lat > 90 OrElse lon < -180 OrElse lon > 180 Then
                invalidCoordinates += 1
                Continue For
            End If

            validCoordinates += 1
            Dim coordinateKey As String = Math.Round(lat, 6).ToString(CultureInfo.InvariantCulture) & "," & Math.Round(lon, 6).ToString(CultureInfo.InvariantCulture)
            If Not duplicateCoordinates.ContainsKey(coordinateKey) Then duplicateCoordinates(coordinateKey) = 0
            duplicateCoordinates(coordinateKey) += 1
            If nameCol IsNot Nothing AndAlso FieldText(row(nameCol)).Trim() = "" Then missingNames += 1
        Next

        Dim duplicateRecords As Integer = 0
        For Each item As KeyValuePair(Of String, Integer) In duplicateCoordinates
            If item.Value > 1 Then duplicateRecords += item.Value
        Next

        Dim status As String = MapSuitabilityStatus(validCoordinates, missingCoordinates, invalidCoordinates, duplicateRecords)
        Dim recommendation As String = MapSuitabilityRecommendation(validCoordinates, missingCoordinates, invalidCoordinates, duplicateRecords, If(nameCol Is Nothing, "", nameCol.ColumnName), missingNames)
        litMapSuitability.Text = RenderMapSuitability(status, latCol.ColumnName, lonCol.ColumnName, SuggestedCoordinateFields(source, True), SuggestedCoordinateFields(source, False), source.Rows.Count, validCoordinates, missingCoordinates, invalidCoordinates, duplicateRecords, missingNames, recommendation)
    End Sub

    Private Function MapSuitabilityStatus(validCoordinates As Integer, missingCoordinates As Integer, invalidCoordinates As Integer, duplicateRecords As Integer) As String
        If validCoordinates <= 0 Then Return "Not enough data"
        If missingCoordinates > 0 OrElse invalidCoordinates > 0 OrElse duplicateRecords > 0 Then Return "Partial"
        Return "Good"
    End Function

    Private Function MapSuitabilityRecommendation(validCoordinates As Integer, missingCoordinates As Integer, invalidCoordinates As Integer, duplicateRecords As Integer, nameField As String, missingNames As Integer) As String
        If validCoordinates <= 0 Then Return "Not recommended for maps until valid latitude and longitude values are available."
        Dim notes As New List(Of String)()
        If missingCoordinates > 0 Then notes.Add("fill missing coordinates")
        If invalidCoordinates > 0 Then notes.Add("fix invalid coordinate ranges")
        If duplicateRecords > 0 Then notes.Add("review duplicate locations")
        If nameField = "" Then notes.Add("select a name or location label field")
        If nameField <> "" AndAlso missingNames > 0 Then notes.Add("fill missing placemark names")
        If notes.Count = 0 Then Return "Data is ready for map and KML output."
        Return "Data can be mapped, but review: " & String.Join(", ", notes.ToArray()) & "."
    End Function

    Private Function RenderMapSuitability(status As String, latField As String, lonField As String, suggestedLatitudeFields As String, suggestedLongitudeFields As String, totalRows As Integer, validCoordinates As Integer, missingCoordinates As Integer, invalidCoordinates As Integer, duplicateRecords As Integer, missingNames As Integer, recommendation As String) As String
        Dim statusClass As String = If(status = "Good", "statusGood", If(status = "Partial", "statusPartial", "statusMissing"))
        Dim foundFields As String = If(latField.Trim() <> "" AndAlso lonField.Trim() <> "", "Latitude: " & latField & "; Longitude: " & lonField, "")
        Dim missingFields As String = If(foundFields = "", "Latitude field; Longitude field", "")

        Dim sb As New StringBuilder()
        sb.Append("<table class=""readinessTable""><tr><th>Status</th><th>Found Fields</th><th>Suggested Latitude Fields</th><th>Suggested Longitude Fields</th><th>Missing Fields</th><th>Total Records Checked</th><th>KML Ready Records</th><th>Missing Coordinates</th><th>Invalid Coordinates</th><th>Duplicate Location Records</th><th>Missing Names</th><th>Recommendation</th></tr>")
        sb.Append("<tr>")
        sb.Append("<td class=""").Append(statusClass).Append(""">").Append(Server.HtmlEncode(status)).Append("</td>")
        sb.Append("<td>").Append(Server.HtmlEncode(foundFields)).Append("</td>")
        sb.Append("<td>").Append(Server.HtmlEncode(suggestedLatitudeFields)).Append("</td>")
        sb.Append("<td>").Append(Server.HtmlEncode(suggestedLongitudeFields)).Append("</td>")
        sb.Append("<td>").Append(Server.HtmlEncode(missingFields)).Append("</td>")
        sb.Append("<td>").Append(totalRows.ToString()).Append("</td>")
        sb.Append("<td>").Append(validCoordinates.ToString()).Append("</td>")
        sb.Append("<td>").Append(missingCoordinates.ToString()).Append("</td>")
        sb.Append("<td>").Append(invalidCoordinates.ToString()).Append("</td>")
        sb.Append("<td>").Append(duplicateRecords.ToString()).Append("</td>")
        sb.Append("<td>").Append(missingNames.ToString()).Append("</td>")
        sb.Append("<td>").Append(Server.HtmlEncode(recommendation)).Append("</td>")
        sb.Append("</tr></table>")
        Return sb.ToString()
    End Function

    Private Function CountFilteredRows(source As DataTable, searchText As String) As Integer
        If source Is Nothing Then Return 0
        If searchText.Trim() = "" Then Return source.Rows.Count
        Dim count As Integer = 0
        For Each row As DataRow In source.Rows
            If RowMatchesSearch(row, searchText) Then count += 1
        Next
        Return count
    End Function

    Private Function CreateResultTable() As DataTable
        Dim dt As New DataTable()
        dt.Columns.Add("Check", GetType(String))
        dt.Columns.Add("Status", GetType(String))
        dt.Columns.Add("Fields", GetType(String))
        dt.Columns.Add("Value", GetType(String))
        dt.Columns.Add("Records", GetType(Integer))
        dt.Columns.Add("Details", GetType(String))
        dt.Columns.Add("KML Ready", GetType(String))
        dt.Columns.Add("FilterId", GetType(String))
        Return dt
    End Function

    Private Sub AddFinding(dt As DataTable, checkName As String, status As String, fields As String, valueText As String, records As Integer, details As String, kmlReady As String, Optional rowFilter As String = "")
        Dim row As DataRow = dt.NewRow()
        row("Check") = checkName
        row("Status") = status
        row("Fields") = fields
        row("Value") = valueText
        row("Records") = records
        row("Details") = details
        row("KML Ready") = kmlReady
        row("FilterId") = RegisterMapReadinessFilter(rowFilter, records)
        dt.Rows.Add(row)
    End Sub

    Private Sub BindReadiness(dt As DataTable)
        GridViewMapReadiness.DataSource = dt
        GridViewMapReadiness.DataBind()
        If GridViewMapReadiness.HeaderRow IsNot Nothing AndAlso dt IsNot Nothing AndAlso dt.Columns.Contains("FilterId") Then
            Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
            If filterIndex >= 0 AndAlso filterIndex < GridViewMapReadiness.HeaderRow.Cells.Count Then GridViewMapReadiness.HeaderRow.Cells(filterIndex).Visible = False
        End If
        If dt Is Nothing Then LabelInfo.Text = "" Else LabelInfo.Text = "Map readiness checks (" & dt.Rows.Count.ToString() & " rows)"
    End Sub

    Protected Sub GridViewMapReadiness_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType <> DataControlRowType.DataRow Then Exit Sub

        Dim dt As DataTable = TryCast(Session("MapReadinesTable"), DataTable)
        If dt Is Nothing OrElse Not dt.Columns.Contains("FilterId") Then Exit Sub

        Dim recordsIndex As Integer = dt.Columns.IndexOf("Records")
        Dim filterIndex As Integer = dt.Columns.IndexOf("FilterId")
        If filterIndex >= 0 AndAlso filterIndex < e.Row.Cells.Count Then e.Row.Cells(filterIndex).Visible = False
        If recordsIndex < 0 OrElse filterIndex < 0 OrElse recordsIndex >= e.Row.Cells.Count OrElse filterIndex >= e.Row.Cells.Count Then Exit Sub

        Dim recordsText As String = e.Row.Cells(recordsIndex).Text.Replace("&nbsp;", "").Trim()
        Dim recordCount As Integer = 0
        If Not Integer.TryParse(recordsText, recordCount) OrElse recordCount <= 0 Then Exit Sub

        Dim filterId As String = e.Row.Cells(filterIndex).Text.Replace("&nbsp;", "").Trim()
        If filterId = "" Then Exit Sub

        Dim link As New HyperLink()
        link.Text = recordsText
        link.NavigateUrl = "~/ShowReport.aspx?srd=0&mrfilter=" & Server.UrlEncode(filterId)
        link.CssClass = "NodeStyle"
        link.ToolTip = "Open corresponding records in Data Explorer."
        e.Row.Cells(recordsIndex).Controls.Clear()
        e.Row.Cells(recordsIndex).Controls.Add(link)
    End Sub

    Private Function RowMatchesSearch(row As DataRow, searchText As String) As Boolean
        For Each col As DataColumn In row.Table.Columns
            If FieldText(row(col)).ToLowerInvariant().Contains(searchText) Then Return True
        Next
        Return False
    End Function

    Private Function TryParseCoordinate(text As String, ByRef value As Double) As Boolean
        Dim cleanText As String = text.Trim().Replace(",", ".")
        Return Double.TryParse(cleanText, NumberStyles.Float Or NumberStyles.AllowThousands, CultureInfo.InvariantCulture, value)
    End Function

    Private Function RegisterMapReadinessFilter(rowFilter As String, records As Integer) As String
        If records <= 0 OrElse rowFilter.Trim() = "" Then Return ""

        Dim filters As Dictionary(Of String, String) = TryCast(Session("MapReadinesFilters"), Dictionary(Of String, String))
        If filters Is Nothing Then
            filters = New Dictionary(Of String, String)()
            Session("MapReadinesFilters") = filters
        End If

        Dim filterId As String = Guid.NewGuid().ToString("N")
        filters(filterId) = rowFilter
        Return filterId
    End Function

    Private Function AllRows(source As DataTable) As List(Of DataRow)
        Dim rows As New List(Of DataRow)()
        If source Is Nothing Then Return rows
        For Each row As DataRow In source.Rows
            rows.Add(row)
        Next
        Return rows
    End Function

    Private Function RowsFilter(rows As List(Of DataRow)) As String
        If rows Is Nothing OrElse rows.Count = 0 Then Return ""

        Dim filters As New List(Of String)()
        Dim seen As New Dictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
        For Each row As DataRow In rows
            Dim rowFilter As String = SingleRowFilter(row)
            If rowFilter.Trim() <> "" AndAlso Not seen.ContainsKey(rowFilter) Then
                filters.Add("(" & rowFilter & ")")
                seen(rowFilter) = True
            End If
        Next

        Return String.Join(" OR ", filters.ToArray())
    End Function

    Private Function CoordinateRangeFilter(latColumn As DataColumn, lonColumn As DataColumn) As String
        If latColumn Is Nothing OrElse lonColumn Is Nothing Then Return ""
        Return FieldRef(latColumn) & " >= -90 AND " & FieldRef(latColumn) & " <= 90 AND " & FieldRef(lonColumn) & " >= -180 AND " & FieldRef(lonColumn) & " <= 180"
    End Function

    Private Function SingleRowFilter(row As DataRow) As String
        Dim filters As New List(Of String)()
        For Each col As DataColumn In row.Table.Columns
            filters.Add(ValueFilter(col, row(col)))
        Next
        Return String.Join(" AND ", filters.ToArray())
    End Function

    Private Function ValueFilter(col As DataColumn, valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return FieldRef(col) & " IS NULL"

        Dim valueText As String = FieldText(valueObject)
        If valueText.Trim() = "" Then Return FieldRef(col) & " = ''"

        Dim numericValue As Double
        If IsNumericColumn(col) AndAlso Double.TryParse(valueText, numericValue) Then
            Return FieldRef(col) & " = " & numericValue.ToString(CultureInfo.InvariantCulture)
        End If

        If col.DataType Is GetType(DateTime) Then
            Dim dateValue As DateTime
            If DateTime.TryParse(valueText, dateValue) Then Return FieldRef(col) & " = #" & dateValue.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) & "#"
        End If

        Return FieldRef(col) & " = '" & valueText.Replace("'", "''") & "'"
    End Function

    Private Function FieldRef(col As DataColumn) As String
        Return "[" & col.ColumnName.Replace("]", "\]") & "]"
    End Function

    Private Function IsNumericColumn(col As DataColumn) As Boolean
        Return col.DataType Is GetType(Byte) OrElse col.DataType Is GetType(SByte) OrElse col.DataType Is GetType(Int16) OrElse col.DataType Is GetType(Int32) OrElse col.DataType Is GetType(Int64) OrElse col.DataType Is GetType(UInt16) OrElse col.DataType Is GetType(UInt32) OrElse col.DataType Is GetType(UInt64) OrElse col.DataType Is GetType(Single) OrElse col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Decimal)
    End Function

    Private Function FieldText(valueObject As Object) As String
        If valueObject Is Nothing OrElse IsDBNull(valueObject) Then Return ""
        If TypeOf valueObject Is DateTime Then Return CType(valueObject, DateTime).ToShortDateString()
        Return valueObject.ToString()
    End Function

    Private Sub ExportReadiness(formatName As String)
        Dim dt As DataTable = TryCast(Session("MapReadinesTable"), DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            BuildAndBindReadiness()
            dt = TryCast(Session("MapReadinesTable"), DataTable)
        End If

        Dim fileName As String = "MapReadiness_" & DateTime.Now.ToString("yyyyMMddHHmmss")
        Response.Clear()
        If formatName = "csv" Then
            Response.ContentType = "text/csv"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".csv")
            Response.Write(ExportToCSVtext(dt, ",", "Map Readiness", ""))
        Else
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "attachment; filename=" & fileName & ".xls")
            Response.Write(ExportToCSVtext(dt, Chr(9), "Map Readiness", ""))
        End If
        Response.End()
    End Sub
End Class
