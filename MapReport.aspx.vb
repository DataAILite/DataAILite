Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
Imports System.IO.Compression
Partial Class MapReport
    Inherits System.Web.UI.Page
    Private mReportTables As ListItemCollection = Nothing
    Private mReportFields As ListItemCollection = Nothing
    Private mReportNumericFields As ListItemCollection = Nothing
    Private keyfields() As String
    Public kmlPath As String
    Private Sub SetLists(ReportID As String)
        Dim ddt As DataTable = GetReportTablesFromSQLqueryText(ReportID)
        Dim i As Integer = 0
        Dim ii As Integer = 0
        Dim j As Integer = 0
        Dim li As ListItem = Nothing
        Dim tbl As String = String.Empty
        Dim fld As String = String.Empty
        Dim fldtype As String = String.Empty

        mReportFields = New ListItemCollection
        mReportTables = New ListItemCollection
        mReportNumericFields = New ListItemCollection

        If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
            For i = 0 To ddt.Rows.Count - 1
                tbl = ddt.Rows(i)("Tbl1").ToString
                tbl = tbl.Replace("`", "").Replace("[", "").Replace("]", "")
                mReportTables.Add(tbl)
            Next
        End If
        mReportFields.Add(New ListItem(" ", " "))
        mReportNumericFields.Add(New ListItem(" ", " "))
        For i = 0 To mReportTables.Count - 1
            'tbl = mReportTables.Item(i).Text
            tbl = mReportTables.Item(i).Value
            ddt = GetListOfTableColumns(tbl, Session("UserConnString"), Session("UserConnProvider")).Table
            If Not ddt Is Nothing AndAlso ddt.Rows.Count > 0 Then
                For ii = 0 To ddt.Rows.Count - 1
                    fld = ddt.Rows(ii)("COLUMN_NAME").ToString
                    fldtype = GetFieldDataType(tbl, fld, Session("UserConnString"), Session("UserConnProvider"))
                    If fldtype.ToString.Trim = "" Then fldtype = fld
                    li = New ListItem(fld, fldtype)
                    mReportFields.Add(li)
                    If TblFieldIsNumeric(tbl, fld, Session("UserConnString"), Session("UserConnProvider")) Then
                        mReportNumericFields.Add(li)
                    End If

                Next
            ElseIf Not ddt Is Nothing AndAlso ddt.Rows.Count = 0 Then
                For ii = 0 To ddt.Rows.Count - 1
                    fld = ddt.Columns(ii).ToString
                    fldtype = GetFieldDataType(tbl, fld, Session("UserConnString"), Session("UserConnProvider"))
                    If fldtype.ToString.Trim = "" Then fldtype = fld
                    li = New ListItem(fld, fldtype)
                    mReportFields.Add(li)
                    If TblFieldIsNumeric(tbl, fld, Session("UserConnString"), Session("UserConnProvider")) Then
                        mReportNumericFields.Add(li)
                    End If
                Next
            End If
        Next

    End Sub
    Private Sub MapReport_Init(sender As Object, e As EventArgs) Handles Me.Init
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        Session("arr") = ""
        Session("nrec") = ""
        Session("ttl") = ""
        Session("x1") = ""
        Session("x2") = ""
        Session("y1") = ""
        Session("srt") = ""
        Session("MapChart") = ""
        Session("MatrixChart") = ""
        Session("AllUnSelected") = ""
        Session("AllSelected") = ""
        Session("cat1") = ""
        Session("cat2") = ""
        Session("AxisY") = ""
        Session("Aggregate") = ""
        Session("AxisYM") = ""
        Session("AxisXM") = ""
        Session("fnM") = ""
        Session("AggregateM") = ""
        Session("MFld") = " "
        Session("SELECTEDValuesM") = " "
        If Not IsPostBack Then
            SetLists(repid)
            Session("latlon") = ""
        End If
        Dim i As Integer
        repid = Session("REPORTID")
        LabelReportName.Text = Session("REPTITLE")

        Dim li As ListItem = Nothing
        If Not IsPostBack AndAlso Not mReportFields Is Nothing AndAlso mReportFields.Count > 0 Then
            For i = 0 To mReportFields.Count - 1
                'li = New ListItem(mReportFields(i).Text, mReportFields(i).Value)
                li = New ListItem(mReportFields(i).Text, mReportFields(i).Text)
                DropDownMapFields.Items.Add(li)
                DropDownMapFields1.Items.Add(li)
                DropDownMapFields2.Items.Add(li)
                DropDownMapFields3.Items.Add(li)
                DropDownKeyFields.Items.Add(li)
            Next
            For i = 0 To mReportNumericFields.Count - 1
                li = New ListItem(mReportNumericFields(i).Text, mReportNumericFields(i).Text)
                DropDownListColorDens.Items.Add(li)
            Next
            For i = 0 To mReportNumericFields.Count - 1
                li = New ListItem(mReportNumericFields(i).Text, mReportNumericFields(i).Text)
                DropDownListExtruded.Items.Add(li)
            Next
            For i = 0 To mReportFields.Count - 1
                li = New ListItem(mReportFields(i).Text, mReportFields(i).Text)
                DropDownListExtrudedColor.Items.Add(li)
            Next
        End If
        If Session("txtMapName") = "" Then
            Session("txtMapName") = Session("REPTITTLE")
        End If
        If Session("txtMapName") IsNot Nothing Then
            txtMapName.Text = Session("txtMapName").ToString
        ElseIf Not IsPostBack AndAlso Session("txtMapName") Is Nothing Then
            txtMapName.Text = DropDownMapNames.SelectedItem.Text
        End If

        If Session("maptype") IsNot Nothing AndAlso Session("maptype").ToString.Trim <> "" Then
            Dim sel As ListItem = DropDownMapType.Items.FindByText(Session("maptype"))
            If sel IsNot Nothing Then
                Dim indx As Integer = DropDownMapType.Items.IndexOf(sel)
                DropDownMapType.SelectedIndex = indx
                DropDownMapType.Text = Session("maptype")
            End If
        End If

        'chkShowLinks.Attributes.Add("onchange", "changeShowLinks();")
        'chkShowCircles.Attributes.Add("onchange", "changeShowCircles();")
        'chkShowPins.Attributes.Add("onchange", "changeShowPins();")
        'txtInitialAltit.Attributes.Add("onchange", "changeInitAltitude();")
        'txtLineWidth.Attributes.Add("onchange", "changeLineWidth();")
    End Sub
    Private Sub LoadKMLData(sMapName As String)
        Dim dt As DataTable = Nothing
        Dim li As ListItem = Nothing
        Dim idx As Integer = 0

        'clear MapFields data
        If MapFields.Rows.Count > 1 Then
            Dim n As Integer = MapFields.Rows.Count - 1
            For i As Integer = n To 1 Step -1
                MapFields.Rows.RemoveAt(i)
            Next
        End If

        'draw map fields
        dt = GetReportMapFields(repid, sMapName)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim ctlLnk As LinkButton
            For i As Integer = 0 To dt.Rows.Count - 1
                Dim indx As String = dt.Rows(i)("Indx").ToString
                AddRowIntoHTMLtable(dt.Rows(i), MapFields)
                MapFields.Rows(i + 1).Cells(0).InnerHtml = dt.Rows(i)("MapField").ToString
                MapFields.Rows(i + 1).Cells(1).InnerHtml = dt.Rows(i)("ForMap").ToString
                If dt.Rows(i)("Friendly").ToString.Trim <> "" AndAlso dt.Rows(i)("Friendly").ToString.Trim <> dt.Rows(i)("MapField").ToString.Trim Then
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = dt.Rows(i)("Friendly").ToString  'friendly name
                Else
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = " "
                End If
                If dt.Rows(i)("descrtext").ToString.Trim <> "" Then
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = dt.Rows(i)("descrtext").ToString  'description text
                Else
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = " "
                End If
                MapFields.Rows(i + 1).Cells(3).InnerHtml = dt.Rows(i)("ord").ToString
                If dt.Rows(i)("ForMap").ToString = "PlacemarkName" Then
                    If dt.Rows(i)("Prop5").ToString.Trim <> "" AndAlso IsNumeric(dt.Rows(i)("Prop5").ToString.Trim) Then
                        txtInitialAltit.Text = dt.Rows(i)("Prop5").ToString.Trim
                    End If
                    If dt.Rows(i)("Prop6").ToString.Trim <> "" AndAlso IsNumeric(dt.Rows(i)("Prop6").ToString.Trim) Then
                        txtLineWidth.Text = dt.Rows(i)("Prop6").ToString.Trim
                    End If
                    If dt.Rows(i)("descrtext").ToString.Trim <> "" Then
                        txtGeoRestrictions.Text = dt.Rows(i)("descrtext").ToString.Trim
                    End If
                    'chkLatLonOrLonLat
                    If dt.Rows(i)("Prop7").ToString.Trim = "True" Then
                        chkLatLonOrLonLat.Checked = True
                        Session("latlon") = "True"
                    Else
                        chkLatLonOrLonLat.Checked = False
                        Session("latlon") = "False"
                    End If
                End If

                'del
                ID = "DeleteMapField^" & indx
                ctlLnk = New LinkButton
                ctlLnk.Text = "delete"
                ctlLnk.ID = ID
                ctlLnk.ToolTip = "Delete Map Field"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                MapFields.Rows(i + 1).Cells(4).InnerText = String.Empty
                MapFields.Rows(i + 1).Cells(4).Controls.Add(ctlLnk)
            Next
        End If
        'draw existing key fields
        Dim dk As DataTable
        If trKeyFields.Visible = True Then
            'clear tblKeyFields data
            If tblKeyFields.Rows.Count > 1 Then
                Dim n As Integer = tblKeyFields.Rows.Count - 1
                For i As Integer = n To 1 Step -1
                    tblKeyFields.Rows.RemoveAt(i)
                Next
            End If

            dk = GetReportKeyFields(repid, sMapName)
            If dk IsNot Nothing AndAlso dk.Rows.Count > 0 Then
                Dim ctlLnk As LinkButton
                For i = 0 To dk.Rows.Count - 1
                    Dim indx As String = dk.Rows(i)("Indx").ToString
                    AddRowIntoHTMLtable(dk.Rows(i), tblKeyFields)
                    tblKeyFields.Rows(i + 1).Cells(0).InnerHtml = dk.Rows(i)("KeyField").ToString
                    If dk.Rows(i)("Friendly").ToString.Trim <> "" AndAlso dk.Rows(i)("Friendly").ToString.Trim <> dk.Rows(i)("KeyField").ToString.Trim Then
                        tblKeyFields.Rows(i + 1).Cells(1).InnerHtml = dk.Rows(i)("Friendly").ToString  'friendly name
                    Else
                        tblKeyFields.Rows(i + 1).Cells(1).InnerHtml = " "
                    End If
                    'del
                    ID = "DeleteKeyField^" & indx
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "delete"
                    ctlLnk.ID = ID
                    ctlLnk.ToolTip = "Delete Key Field"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    tblKeyFields.Rows(i + 1).Cells(2).InnerText = String.Empty
                    tblKeyFields.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)
                Next
            End If
        End If
        'draw extruded field
        dt = GetReportExtrudedFields(repid, sMapName)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Session("ExtrudedField") = dt.Rows(0)("Val")
            Session("MultiplyBy") = dt.Rows(0)("Prop4")
            Session("ExtrudedColor") = dt.Rows(0)("Prop5")
        Else
            Session("ExtrudedField") = ""
            Session("MultiplyBy") = "10"
            Session("ExtrudedColor") = ""
        End If

        'draw label with selected ColorField and color
        dt = GetReportColorField(repid, sMapName)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Session("ColorField") = dt.Rows(0)("Val")
            Session("color") = dt.Rows(0)("Prop4")
            Session("MultiplyBy0") = dt.Rows(0)("Prop5")
            LabelColorSaved.Visible = True
            LabelColorSaved.ForeColor = ColorTranslator.FromHtml(Session("color"))
        Else
            LabelColorSaved.Visible = False
            Session("MultiplyBy0") = "0.01"
        End If

        If Session("ColorField") IsNot Nothing AndAlso Session("ColorField") <> String.Empty Then
            li = DropDownListColorDens.Items.FindByText(Session("ColorField"))
            idx = DropDownListColorDens.Items.IndexOf(li)
            DropDownListColorDens.SelectedIndex = idx
            DropDownListColorDens.Text = Session("ColorField")
            txtMultiplyBy0.Text = Session("MultiplyBy0").ToString
        End If
        If Session("color") Is Nothing OrElse Session("color") = String.Empty Then
            Session("color") = "#ff0000"
        End If

        If Session("ExtrudedField") IsNot Nothing AndAlso Session("ExtrudedField").ToString.Trim <> "" Then
            li = DropDownListExtruded.Items.FindByText(Session("ExtrudedField"))
            idx = DropDownListExtruded.Items.IndexOf(li)
            DropDownListExtruded.SelectedIndex = idx
            DropDownListExtruded.Text = Session("ExtrudedField")
            txtMultiplyBy.Text = Session("MultiplyBy").ToString
        End If
        If Session("ExtrudedColor") IsNot Nothing AndAlso Session("ExtrudedColor").ToString.Trim <> "" Then
            li = DropDownListExtrudedColor.Items.FindByText(Session("ExtrudedColor"))
            idx = DropDownListExtrudedColor.Items.IndexOf(li)
            DropDownListExtrudedColor.SelectedIndex = idx
            DropDownListExtrudedColor.Text = Session("ExtrudedColor")
        End If
        If Session("Comments") IsNot Nothing AndAlso Session("Comments").ToString.Trim <> "" Then
            txtComments.Text = Session("Comments").ToString
            txtComments.ToolTip = txtComments.Text
        End If
        dt = GetMapShowFields(repid, txtMapName.Text)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i)("Prop1") = "MapType" Then
                    DropDownMapType.Text = dt.Rows(i)("Val").ToString
                End If
                If dt.Rows(i)("Prop1") = "ShowLinks" Then
                    If dt.Rows(i)("Val") = "True" Then
                        chkShowLinks.Checked = True
                    Else
                        chkShowLinks.Checked = False
                    End If
                    hdnShowLinks.Value = chkShowLinks.Checked
                End If
                If dt.Rows(i)("Prop1") = "ShowPins" Then
                    If dt.Rows(i)("Val") = "True" Then
                        chkShowPins.Checked = True
                    Else
                        chkShowPins.Checked = False
                    End If
                    hdnShowPins.Value = chkShowPins.Checked
                End If
                If dt.Rows(i)("Prop1") = "ShowCircles" Then
                    If dt.Rows(i)("Val") = "True" Then
                        chkShowCircles.Checked = True
                    Else
                        chkShowCircles.Checked = False
                    End If
                    hdnShowCircles.Value = chkShowCircles.Checked
                End If
            Next
        End If

    End Sub

    Private Sub MapReport_Load(sender As Object, e As EventArgs) Handles Me.Load
        repid = Session("REPORTID")
        If (Request("latlon") IsNot Nothing AndAlso Request("latlon").ToString = "True") Then
            chkLatLonOrLonLat.Checked = True
            Session("latlon") = "True"
            'Else
            'chkLatLonOrLonLat.Checked = False
            'Session("latlon") = "False"
        End If
        'download kml
        Dim ret As String = String.Empty
        If Not Request("openkml") Is Nothing AndAlso Request("openkml").ToString.Trim = "yes" AndAlso Not Session("kmlfile") Is Nothing Then
            Try
                kmlPath = "~/KMLS/" & Session("kmlfile").ToString.Replace(Session("appldirKMLFiles"), "")
                Try
                    Response.ContentType = "application/vnd.google-earth.kml+xml"
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" & kmlPath)
                    Response.TransmitFile(kmlPath)
                Catch ex As Exception
                    ret = "ERROR!!  " & ex.Message
                End Try
                Response.End()
            Catch ex As Exception
                ret = "ERROR!! " & ex.Message
            End Try
            LabelAlert.Text = ret
        End If
        If Not IsPostBack Then
            DropDownMapNames.Items.Clear()
            Dim kmls() As String = GetKMLfiles(repid)
            Dim li As ListItem = Nothing
            Dim i As Integer
            If Request("showlinks") IsNot Nothing Then
                chkShowLinks.Checked = Request("showlinks")
                hdnShowLinks.Value = Request("showlinks")
            End If
            If Request("showcircles") IsNot Nothing Then
                chkShowCircles.Checked = Request("showcircles")
                hdnShowCircles.Value = Request("showcircles")
            End If
            If Request("showpins") IsNot Nothing Then
                chkShowPins.Checked = Request("showpins")
                hdnShowPins.Value = Request("showpins")
            End If
            If Request("initalt") IsNot Nothing Then
                txtInitialAltit.Text = Request("initalt")
                hdnInitAltitude.Value = Request("initalt")
            End If
            If Request("linewidth") IsNot Nothing Then
                txtLineWidth.Text = Request("linewidth")
                hdnLineWidth.Value = Request("linewidth")
            End If

            If Not kmls Is Nothing AndAlso kmls.Length > 0 Then
                For i = 0 To kmls.Length - 1
                    li = New ListItem(kmls(i), kmls(i))
                    DropDownMapNames.Items.Add(li)
                    If Session("txtMapName") IsNot Nothing AndAlso kmls(i).Trim = Session("txtMapName").ToString.Trim Then
                        DropDownMapNames.Text = kmls(i)
                        DropDownMapNames.SelectedIndex = i
                        txtMapName.Text = kmls(i)
                    End If
                Next
                Label4.Visible = True
                Label5.Visible = True
                DropDownMapNames.Visible = True
                txtMapName.Text = DropDownMapNames.SelectedItem.Text
            Else
                If Session("txtMapName") <> "" Then
                    txtMapName.Text = Session("txtMapName")
                    DropDownMapNames.Items.Add(txtMapName.Text)
                Else
                    Label4.Visible = False
                    Label5.Visible = False
                    DropDownMapNames.Visible = False
                End If
            End If
        End If
        btnPlacemarkLongitude.Text = "Placemark Longitude"
        btnPlacemarkLatitude.Text = "Placemark Latitude"
        btnPlacemarkLongitude2.Text = "Placemark end Longitude"
        btnPlacemarkLatitude2.Text = "Placemark end Latitude"
        If DropDownMapType.Text = "Pins" Or DropDownMapType.Text = "Circles" Or DropDownMapType.Text = "Floating placemark" Or DropDownMapType.Text = "Extruded placemark" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = False
            btnPlacemarkLongitude2.Enabled = False
            btnPlacemarkLatitude2.Visible = False
            btnPlacemarkLatitude2.Enabled = False
            btnPlacemarkGeolocation2.Visible = False
            btnPlacemarkGeolocation2.Enabled = False
            trAddPoints.Visible = False
            trKeyFields.Visible = False
            trExtruded.Visible = True
            tblKeyFields.Visible = False
        ElseIf DropDownMapType.Text = "Paths" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            btnPlacemarkGeolocation2.Visible = True
            btnPlacemarkGeolocation2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Extruded Paths" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            btnPlacemarkGeolocation2.Visible = True
            btnPlacemarkGeolocation2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Tours" Then
            btnPlacemarkStartTime.Visible = True
            btnPlacemarkStartTime.Enabled = True
            btnPlacemarkEndTime.Visible = True
            btnPlacemarkEndTime.Enabled = True
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            btnPlacemarkGeolocation2.Visible = True
            btnPlacemarkGeolocation2.Enabled = True
            trAddPoints.Visible = False
            trKeyFields.Visible = False
            trExtruded.Visible = True
            tblKeyFields.Visible = False
        ElseIf DropDownMapType.Text = "Extruded Polygons" Then

            btnPlacemarkLongitude.Text = "Placemark border Longitude"
            btnPlacemarkLatitude.Text = "Placemark border Latitude"
            btnPlacemarkLongitude2.Text = "Placemark pin Longitude"
            btnPlacemarkLatitude2.Text = "Placemark pin Latitude"

            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            btnPlacemarkGeolocation2.Visible = True
            btnPlacemarkGeolocation2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Polygons" Then
            btnPlacemarkLongitude.Text = "Placemark border Longitude"
            btnPlacemarkLatitude.Text = "Placemark border Latitude"
            btnPlacemarkLongitude2.Text = "Placemark pin Longitude"
            btnPlacemarkLatitude2.Text = "Placemark pin Latitude"
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            btnPlacemarkGeolocation2.Visible = True
            btnPlacemarkGeolocation2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Curve" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = False
            btnPlacemarkLongitude2.Enabled = False
            btnPlacemarkLatitude2.Visible = False
            btnPlacemarkLatitude2.Enabled = False
            btnPlacemarkGeolocation2.Visible = False
            btnPlacemarkGeolocation2.Enabled = False
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        End If
        If Request("del") IsNot Nothing AndAlso Request("del").ToString = "yes" AndAlso Request("indx") IsNot Nothing Then
            Dim indx As String = Request("indx").ToString
            DeleteMapField(repid, indx)
            'Response.Redirect("MapReport.aspx?del=no" & "&showlinks=" & chkShowLinks.Checked)
        End If
        If Request("delkey") IsNot Nothing AndAlso Request("delkey").ToString = "yes" AndAlso Request("indx") IsNot Nothing Then
            Dim indx As String = Request("indx").ToString
            DeleteKeyField(repid, indx)
            'Response.Redirect("MapReport.aspx?delkey=no" & "&showlinks=" & chkShowLinks.Checked)
        End If

        'draw existing map fields
        'TODO get maptype, show pins, show circles, show links

        dt = GetReportMapFields(repid, txtMapName.Text)
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim ctlLnk As LinkButton
            For i = 0 To dt.Rows.Count - 1
                Dim indx As String = dt.Rows(i)("Indx").ToString
                AddRowIntoHTMLtable(dt.Rows(i), MapFields)
                MapFields.Rows(i + 1).Cells(0).InnerHtml = dt.Rows(i)("MapField").ToString
                MapFields.Rows(i + 1).Cells(1).InnerHtml = dt.Rows(i)("ForMap").ToString
                If dt.Rows(i)("Friendly").ToString.Trim <> "" AndAlso dt.Rows(i)("Friendly").ToString.Trim <> dt.Rows(i)("MapField").ToString.Trim Then
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = dt.Rows(i)("Friendly").ToString  'friendly name
                Else
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = " "
                End If
                If dt.Rows(i)("descrtext").ToString.Trim <> "" Then
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = dt.Rows(i)("descrtext").ToString  'description text
                Else
                    MapFields.Rows(i + 1).Cells(2).InnerHtml = " "
                End If
                MapFields.Rows(i + 1).Cells(3).InnerHtml = dt.Rows(i)("ord").ToString
                If Not IsPostBack Then
                    If dt.Rows(i)("ForMap").ToString = "PlacemarkName" Then
                        If dt.Rows(i)("Prop5").ToString.Trim <> "" AndAlso IsNumeric(dt.Rows(i)("Prop5").ToString.Trim) Then
                            txtInitialAltit.Text = dt.Rows(i)("Prop5").ToString.Trim
                        End If
                        If dt.Rows(i)("Prop6").ToString.Trim <> "" AndAlso IsNumeric(dt.Rows(i)("Prop6").ToString.Trim) Then
                            txtLineWidth.Text = dt.Rows(i)("Prop6").ToString.Trim
                        End If
                        If dt.Rows(i)("descrtext").ToString.Trim <> "" Then
                            txtGeoRestrictions.Text = dt.Rows(i)("descrtext").ToString.Trim
                        End If
                        'chkLatLonOrLonLat
                        If dt.Rows(i)("Prop7").ToString.Trim = "True" Then
                            chkLatLonOrLonLat.Checked = True
                            Session("latlon") = "True"
                        Else
                            chkLatLonOrLonLat.Checked = False
                            Session("latlon") = "False"
                        End If
                    End If
                End If
                'del
                ID = "DeleteMapField^" & indx
                ctlLnk = New LinkButton
                ctlLnk.Text = "delete"
                ctlLnk.ID = ID
                ctlLnk.ToolTip = "Delete Map Field"
                AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                MapFields.Rows(i + 1).Cells(4).InnerText = String.Empty
                MapFields.Rows(i + 1).Cells(4).Controls.Add(ctlLnk)
            Next
        End If

        'draw existing key fields
        Dim dk As DataTable
        If trKeyFields.Visible = True Then
            dk = GetReportKeyFields(repid, txtMapName.Text)
            If dk IsNot Nothing AndAlso dk.Rows.Count > 0 Then
                Dim ctlLnk As LinkButton
                For i = 0 To dk.Rows.Count - 1
                    Dim indx As String = dk.Rows(i)("Indx").ToString
                    AddRowIntoHTMLtable(dk.Rows(i), tblKeyFields)
                    tblKeyFields.Rows(i + 1).Cells(0).InnerHtml = dk.Rows(i)("KeyField").ToString
                    If dk.Rows(i)("Friendly").ToString.Trim <> "" AndAlso dk.Rows(i)("Friendly").ToString.Trim <> dk.Rows(i)("KeyField").ToString.Trim Then
                        tblKeyFields.Rows(i + 1).Cells(1).InnerHtml = dk.Rows(i)("Friendly").ToString  'friendly name
                    Else
                        tblKeyFields.Rows(i + 1).Cells(1).InnerHtml = " "
                    End If
                    'del
                    ID = "DeleteKeyField^" & indx
                    ctlLnk = New LinkButton
                    ctlLnk.Text = "delete"
                    ctlLnk.ID = ID
                    ctlLnk.ToolTip = "Delete Key Field"
                    AddHandler ctlLnk.Click, AddressOf ctlLnk_Click
                    tblKeyFields.Rows(i + 1).Cells(2).InnerText = String.Empty
                    tblKeyFields.Rows(i + 1).Cells(2).Controls.Add(ctlLnk)
                Next
            End If
        End If

        If Not IsPostBack Then
            Dim li As ListItem = Nothing
            Dim idx As Integer = 0

            'draw extruded field
            dt = GetReportExtrudedFields(repid, txtMapName.Text)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Session("ExtrudedField") = dt.Rows(0)("Val")
                Session("MultiplyBy") = dt.Rows(0)("Prop4")
                Session("ExtrudedColor") = dt.Rows(0)("Prop5")
            Else
                Session("ExtrudedField") = ""
                Session("MultiplyBy") = "10"
                Session("ExtrudedColor") = ""
            End If

            'draw label with selected ColorField and color
            dt = GetReportColorField(repid, txtMapName.Text)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                Session("ColorField") = dt.Rows(0)("Val")
                Session("color") = dt.Rows(0)("Prop4")
                Session("MultiplyBy0") = dt.Rows(0)("Prop5")
                LabelColorSaved.Visible = True
                LabelColorSaved.ForeColor = ColorTranslator.FromHtml(Session("color"))
            Else
                LabelColorSaved.Visible = False
                Session("MultiplyBy0") = "0.01"
            End If

            If Session("ColorField") IsNot Nothing Then
                li = DropDownListColorDens.Items.FindByText(Session("ColorField"))
                idx = DropDownListColorDens.Items.IndexOf(li)
                DropDownListColorDens.SelectedIndex = idx
                DropDownListColorDens.Text = Session("ColorField")
                txtMultiplyBy0.Text = Session("MultiplyBy0").ToString
            End If
            If Session("color") Is Nothing Then
                Session("color") = "#ff0000"
            End If

            If Session("ExtrudedField") IsNot Nothing AndAlso Session("ExtrudedField").ToString.Trim <> "" Then
                li = DropDownListExtruded.Items.FindByText(Session("ExtrudedField"))
                idx = DropDownListExtruded.Items.IndexOf(li)
                DropDownListExtruded.SelectedIndex = idx
                DropDownListExtruded.Text = Session("ExtrudedField")
                txtMultiplyBy.Text = Session("MultiplyBy").ToString
            End If
            If Session("ExtrudedColor") IsNot Nothing AndAlso Session("ExtrudedColor").ToString.Trim <> "" Then
                li = DropDownListExtrudedColor.Items.FindByText(Session("ExtrudedColor"))
                idx = DropDownListExtrudedColor.Items.IndexOf(li)
                DropDownListExtrudedColor.SelectedIndex = idx
                DropDownListExtrudedColor.Text = Session("ExtrudedColor")
            End If
            If Session("Comments") IsNot Nothing AndAlso Session("Comments").ToString.Trim <> "" Then
                txtComments.Text = Session("Comments").ToString
                txtComments.ToolTip = txtComments.Text
            End If

            dt = GetMapShowFields(repid, txtMapName.Text)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                For i = 0 To dt.Rows.Count - 1
                    If dt.Rows(i)("Prop1") = "MapType" Then
                        DropDownMapType.Text = dt.Rows(i)("Val").ToString
                    End If
                    If dt.Rows(i)("Prop1") = "ShowLinks" Then
                        If dt.Rows(i)("Val") = "True" Then
                            chkShowLinks.Checked = True
                        Else
                            chkShowLinks.Checked = False
                        End If
                        hdnShowLinks.Value = chkShowLinks.Checked
                    End If
                    If dt.Rows(i)("Prop1") = "ShowPins" Then
                        If dt.Rows(i)("Val") = "True" Then
                            chkShowPins.Checked = True
                        Else
                            chkShowPins.Checked = False
                        End If
                        hdnShowPins.Value = chkShowPins.Checked
                    End If
                    If dt.Rows(i)("Prop1") = "ShowCircles" Then
                        If dt.Rows(i)("Val") = "True" Then
                            chkShowCircles.Checked = True
                        Else
                            chkShowCircles.Checked = False
                        End If
                        hdnShowCircles.Value = chkShowCircles.Checked
                    End If
                Next
            End If
        End If
    End Sub
    Protected Sub ctlLnk_Click(sender As Object, e As EventArgs)
        Dim btnLink As LinkButton = CType(sender, LinkButton)
        Dim id As String = btnLink.ID
        Dim tag As String = Piece(id, "^", 1)
        Dim indx As String = Piece(id, "^", 2)
        Dim link As String = String.Empty
        Select Case tag
            Case "DeleteMapField"
                link = "MapReport.aspx?Report=" & repid & "&indx=" & indx & "&del=yes" & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString
            Case "DeleteKeyField"
                link = "MapReport.aspx?Report=" & repid & "&indx=" & indx & "&delkey=yes" & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString
        End Select
        If link <> String.Empty Then _
          Response.Redirect(link)
    End Sub
    Private Function DeleteMapField(ByVal rep As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            Dim updSQL As String = "DELETE FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='MAPS' AND Indx='" & indx & "')"
            ret = ExequteSQLquery(updSQL)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Private Function DeleteKeyField(ByVal rep As String, ByVal indx As String) As String
        Dim ret As String = String.Empty
        Try
            Dim updSQL As String = "DELETE FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='KEYS' AND Indx='" & indx & "')"
            ret = ExequteSQLquery(updSQL)
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Private Function GetKMLfiles(ByVal rep As String) As String()
        Dim kmls() As String = Nothing
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Try
            Dim selectSQL As String = "SELECT DISTINCT Prop3 AS MapName FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS')"
            dt = DataModule.mRecords(selectSQL, ret).Table
            If dt IsNot Nothing Then
                Dim i As Integer
                For i = 0 To dt.Rows.Count - 1
                    ReDim Preserve kmls(i)
                    kmls(i) = dt.Rows(i)("MapName").ToString
                Next
            End If
            'Add from history
            Dim n As Integer = 0
            If kmls IsNot Nothing Then n = kmls.Length
            selectSQL = "SELECT DISTINCT MapName,Saved FROM ourkmlhistory WHERE (ReportID='" & Session("REPORTID") & "')"
            dt = DataModule.mRecords(selectSQL, ret).Table
            If dt IsNot Nothing Then
                Dim i As Integer
                For i = 0 To dt.Rows.Count - 1
                    ReDim Preserve kmls(i + n)
                    kmls(i + n) = dt.Rows(i)("MapName").ToString & " *(" & dt.Rows(i)("Saved").ToString & ")"
                Next
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return kmls
    End Function

    Protected Function AddField(ByVal fieldtext As String, ByVal placemark As String, ByVal k As Integer, Optional ByVal colr As String = "", Optional ByVal colrfld As String = "") As String
        Dim ret As String = String.Empty
        Dim rep As String = String.Empty
        Dim frname As String = String.Empty
        Dim insSQL As String = String.Empty
        If txtMapName.Text.Trim = "" Then
            txtMapName.Text = Session("REPTITLE")
        End If
        Session("txtMapName") = txtMapName.Text
        Try
            frname = GetFriendlyFieldName(repid, fieldtext)
            If placemark = "ColorField" Then
                Dim sSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='COLOR' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "' AND [Order]=" & k.ToString & ")"
                If Not HasRecords(sSQL) Then
                    'insert
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3,Prop4,Prop5) VALUES('" & Session("REPORTID") & "','COLOR','" & fieldtext & "'," & k.ToString & ",'" & frname & "','" & placemark & "','" & txtMapName.Text & "','" & colr & "','" & txtMultiplyBy0.Text & "')"
                    ret = ExequteSQLquery(insSQL)
                Else
                    'update
                    Dim updSQL As String = "UPDATE OURReportFormat SET Val='" & fieldtext & "',Prop1='" & frname & "',Prop4='" & colr & "',Prop5='" & txtMultiplyBy0.Text & "',[Order]=" & k.ToString & " WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='COLOR' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(updSQL)
                End If
            ElseIf placemark = "KeyField" Then
                Dim sSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='KEYS' AND VAL='" & fieldtext & "' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "' AND [Order]=" & k.ToString & ")"
                If Not HasRecords(sSQL) Then
                    'insert
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','KEYS','" & fieldtext & "'," & k.ToString & ",'" & frname & "','" & placemark & "','" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(insSQL)
                End If
            ElseIf placemark = "ExtrudedField" Then
                Dim sSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='EXTRUDE' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "' AND [Order]=" & k.ToString & ")"
                If Not HasRecords(sSQL) Then
                    'insert
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3,Prop4,Prop5) VALUES('" & Session("REPORTID") & "','EXTRUDE','" & fieldtext & "'," & k.ToString & ",'" & frname & "','" & placemark & "','" & txtMapName.Text & "','" & colr & "','" & colrfld & "')"
                    ret = ExequteSQLquery(insSQL)
                Else
                    'update
                    Dim updSQL As String = "UPDATE OURReportFormat SET Val='" & fieldtext & "',Prop1='" & frname & "',Prop4='" & colr & "',Prop5='" & colrfld & "',[Order]=" & k.ToString & " WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='EXTRUDE' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(updSQL)
                End If
            ElseIf placemark = "MapType" OrElse placemark = "ShowPins" OrElse placemark = "ShowCircles" OrElse placemark = "ShowLinks" Then
                Dim sSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='SHOWS' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "')"
                If Not HasRecords(sSQL) Then
                    'insert
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','SHOWS','" & fieldtext & "'," & k.ToString & ",'" & placemark & "','" & placemark & "','" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(insSQL)
                Else
                    'update
                    Dim updSQL As String = "UPDATE OURReportFormat SET Val='" & fieldtext & "',Prop1='" & placemark & "',Prop2='" & placemark & "',[Order]=" & k.ToString & " WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='SHOWS' AND Prop1='" & placemark & "'  AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(updSQL)
                End If
            Else
                Dim sctSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND VAL='" & fieldtext & "' AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "' AND [Order]=" & k.ToString & ")"
                If Not HasRecords(sctSQL) Then
                    'insert
                    insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3) VALUES('" & Session("REPORTID") & "','MAPS','" & fieldtext & "'," & k.ToString & ",'" & frname & "','" & placemark & "','" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(insSQL)
                Else
                    'update
                    Dim updSQL As String = "UPDATE OURReportFormat SET Prop1='" & frname & "',Prop2='" & placemark & "',[Order]=" & k.ToString & " WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND VAL='" & fieldtext & "'  AND Prop2='" & placemark & "' AND Prop3='" & txtMapName.Text & "')"
                    ret = ExequteSQLquery(updSQL)
                End If
            End If
        Catch ex As Exception
            ret = "ERROR!! " & ret & ", " & ex.Message
        End Try
        Return ret
    End Function

    Protected Function AddDescrText(ByVal placemark As String, ByVal ordr As Integer) As String
        Dim ret As String = String.Empty
        Dim rep As String = String.Empty
        Dim frname As String = String.Empty
        Dim insSQL As String = String.Empty
        If txtMapName.Text.Trim = "" Then
            txtMapName.Text = Session("REPTITLE")
        End If
        Session("txtMapName") = txtMapName.Text
        If txtDescr.Text.Trim = "" Then
            Return ""
            Exit Function
        End If
        Try
            insSQL = "INSERT INTO OURReportFormat (ReportID,Prop,Val,[Order],Prop1,Prop2,Prop3,Prop4) VALUES('" & Session("REPORTID") & "','MAPS','  ---  '," & ordr.ToString & ",'','" & placemark & "','" & txtMapName.Text & "','" & txtDescr.Text & "')"
            ret = ExequteSQLquery(insSQL)
        Catch ex As Exception
            ret = ret & " ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    Protected Sub btnPlacemarkName_Click(sender As Object, e As EventArgs) Handles btnPlacemarkName.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkName", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkDescription_Click(sender As Object, e As EventArgs) Handles btnPlacemarkDescription.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkDescription", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkLongitude_Click(sender As Object, e As EventArgs) Handles btnPlacemarkLongitude.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkLongitude", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkLatitude_Click(sender As Object, e As EventArgs) Handles btnPlacemarkLatitude.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkLatitude", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkLongitude2_Click(sender As Object, e As EventArgs) Handles btnPlacemarkLongitude2.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 1
            ret = AddField(DropDownMapFields.Text, "PlacemarkLongitudeEnd", 1)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkLatitude2_Click(sender As Object, e As EventArgs) Handles btnPlacemarkLatitude2.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 1
            ret = AddField(DropDownMapFields.Text, "PlacemarkLatitudeEnd", 1)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkStartTime_Click(sender As Object, e As EventArgs) Handles btnPlacemarkStartTime.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkStartTime", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnPlacemarkEndTime_Click(sender As Object, e As EventArgs) Handles btnPlacemarkEndTime.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkEndTime", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub

    Protected Sub ButtonSubmit_Click(sender As Object, e As EventArgs) Handles ButtonSubmit.Click
        'NOT IN USE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        'Dim ret As String = String.Empty
        ''btnLinkDown.Visible = False
        ''btnLinkDown.Enabled = False
        ''btnOpenGoogleMap.Visible = False
        ''btnOpenGoogleMap.Enabled = False
        'Dim dm As DataTable = GetReportMapFields(repid, txtMapName.Text)
        'If dm Is Nothing Then
        '    ret = "no Map fields found"
        'End If
        'Dim i As Integer
        'Dim lon, lat, nm, des As String
        'Dim lonend As String = String.Empty
        'Dim latend As String = String.Empty
        'Dim starttimecol As String = String.Empty
        'Dim endtimecol As String = String.Empty
        'For i = 0 To dm.Rows.Count - 1
        '    If dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkName" AndAlso dm.Rows(i)("ord") = 0 Then
        '        nm = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkDescription" AndAlso dm.Rows(i)("ord") = 0 Then
        '        des = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitude" AndAlso dm.Rows(i)("ord") = 0 Then
        '        lon = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitude" AndAlso dm.Rows(i)("ord") = 0 Then
        '        lat = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
        '        lonend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
        '        latend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkStartTime" Then
        '        starttimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkEndTime" Then
        '        endtimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
        '    End If
        'Next
        'Dim appldirKMLFiles, myfile As String
        ''Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        'Dim datestr, timestr As String
        'datestr = DateString()
        'timestr = TimeString()
        'Dim mapname As String = txtMapName.Text
        'If mapname.Contains(" *(") Then 'from history
        '    mapname = mapname.Substring(0, mapname.IndexOf(" *(")).Trim
        'End If
        'myfile = Session("REPORTID") & "_" & mapname.Replace(" ", "_") & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".kml"
        'appldirKMLFiles = applpath & "KMLS\"
        'Session("appldirKMLFiles") = appldirKMLFiles
        'Dim expfile As String = appldirKMLFiles & myfile
        'Session("kmlfile") = expfile
        'Dim dv3 As DataView = Session("dv3")
        'If dv3 Is Nothing Then
        '    Dim dri As DataTable = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & Session("REPORTID") & "')").Table
        '    Dim msql = dri.Rows(0)("SQLquerytext")
        '    ret = ""
        '    dv3 = mRecords(msql, ret, Session("UserConnString"), Session("UserConnProvider"))
        '    If dv3 Is Nothing Then
        '        LabelAlert.Text = "no data"
        '        LabelAlert.Visible = True
        '        Exit Sub
        '    End If
        'End If
        'Dim maptype As String = DropDownMapType.Text.Trim
        'Dim sl As Boolean = False
        'If chkShowLinks.Checked Then
        '    sl = chkShowLinks.Checked
        '    GenerateMap.lgn = GetReportIdentifier(Session("REPORTID"))
        'End If
        'Dim inalt As Integer = 4000
        'Dim inwd As Integer = 4
        'If txtInitialAltit.Text.Trim <> "" AndAlso IsNumeric(txtInitialAltit.Text.Trim) Then
        '    inalt = CInt(txtInitialAltit.Text.Trim)
        'End If
        'If txtLineWidth.Text.Trim <> "" AndAlso IsNumeric(txtLineWidth.Text.Trim) Then
        '    inwd = CInt(txtLineWidth.Text.Trim)
        'End If
        'If maptype = "Pins" Then
        '    ret = GenerateMapReportPlacemarks(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", "", nm, des, sl, chkShowCircles.Checked, inalt, inwd)
        'ElseIf maptype = "Circles" Then
        '    chkShowCircles.Checked = True
        '    hdnShowCircles.Value = "true"
        '    Session("showcircles") = "true"
        '    ret = GenerateMapReportCircles(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", "", nm, des, lonend, latend, chkShowPins.Checked, sl, inalt, inwd)
        'ElseIf maptype = "Paths" Then
        '    '    ret = GenerateMapReportPath(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", "", nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd)
        '    'ElseIf maptype = "Extruded Paths" Then
        '    ret = GenerateMapReportExtrudedPath(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", "", nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd)

        'ElseIf maptype = "Polygons" Then
        '    '    ret = GenerateMapReportPolygons(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", "", nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd)

        '    'ElseIf maptype = "Extruded Polygons" Then
        '    ret = GenerateMapReportExtrudedPolygons(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", "", nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd)

        'ElseIf maptype = "Tours" Then
        '    ret = GenerateMapReportTours(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, starttimecol, endtimecol, lon, lat, "", "", "", "", nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd)

        'Else

        'End If
        ''btnLinkDown.Visible = True
        ''btnLinkDown.Enabled = True
        ''btnLinkDown.ToolTip = "Download KML file and then right click and open in Google Earth Pro."
        ''btnOpenGoogleMap.Visible = True
        ''btnOpenGoogleMap.Enabled = True
        ''btnOpenGoogleMap.ToolTip = "Open KML file in Google Earth"
        ''TODO check if file is empty
        'If ret.StartsWith("ERROR!!") OrElse Session("kmlfile") Is Nothing OrElse Session("kmlfile").ToString.Trim = "" Then  'OrElse File.ReadAllText(Session("kmlfile").ToString).Trim.Length > 0 
        '    'file does not exist
        '    MessageBox.Show("Error!! KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        'Else
        '    'file exists
        '    MessageBox.Show("KML file generated", "KML file generated", "KMLgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        'End If
    End Sub

    Private Sub DropDownMapNames_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownMapNames.SelectedIndexChanged
        Dim ret As String = String.Empty
        Dim SelectedName As String = DropDownMapNames.SelectedItem.Text.Trim
        If SelectedName <> "" Then
            txtMapName.Text = SelectedName

            If SelectedName.Contains("*(") AndAlso SelectedName.Contains(")") AndAlso Session("txtMapName") <> txtMapName.Text Then
                Dim ds As DataTable = GetHistoricKMLsetting(Session("REPORTID"), SelectedName)
                Dim savedat As String = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToLongTimeString.Replace(":", "_").Replace(" ", "")
                Dim mapname As String = SelectedName.Substring(0, SelectedName.IndexOf(" *(")).Trim

                txtMapName.Text = mapname & " -#" & savedat
                'add records from ourkmlhistory into OURReportFormat and assign fields in MapReport page
                If ds Is Nothing OrElse ds.Rows.Count = 0 Then
                    Exit Sub
                End If
                txtComments.Text = ds.Rows(0)("Comments").ToString & ". It was saved in the history as " & DropDownMapNames.SelectedItem.Text
                txtComments.ToolTip = txtComments.Text
                txtMapName.ToolTip = "From the history: " & txtComments.Text
                DropDownMapType.Text = ds.Rows(0)("MapType").ToString
                Session("maptype") = ds.Rows(0)("MapType").ToString
                hdnShowPins.Value = ds.Rows(0)("ShowPins")
                hdnShowCircles.Value = ds.Rows(0)("ShowCircles")
                hdnShowLinks.Value = ds.Rows(0)("ShowLinks")
                If ds.Rows(0)("ShowPins") = True Then
                    chkShowPins.Checked = True
                Else
                    chkShowPins.Checked = False
                End If
                If ds.Rows(0)("ShowCircles") = True Then
                    chkShowCircles.Checked = True
                Else
                    chkShowCircles.Checked = False
                End If
                If ds.Rows(0)("ShowLinks") = True Then
                    chkShowLinks.Checked = True
                Else
                    chkShowLinks.Checked = False
                End If
                hdnInitAltitude.Value = ds.Rows(0)("InitAltit").ToString
                txtInitialAltit.Text = hdnInitAltitude.Value
                hdnLineWidth.Value = ds.Rows(0)("LineWidth").ToString
                txtLineWidth.Text = hdnLineWidth.Value
                txtGeoRestrictions.Text = ds.Rows(0)("Prop4").ToString
                If ds.Rows(0)("Prop7").ToString.Trim = "True" Then
                    chkLatLonOrLonLat.Checked = True
                Else
                    chkLatLonOrLonLat.Checked = False
                End If
                Session("Comments") = txtComments.Text
                Session("initalt") = txtInitialAltit.Text
                Session("linewidth") = txtLineWidth.Text
                Session("order") = 0
                txtGeoRestrictions.Text = ds.Rows(0)("Prop4").ToString
                If ds.Rows(0)("PlacemarkName").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkName"), "PlacemarkName", 0)
                If ds.Rows(0)("PlacemarkLon").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLon"), "PlacemarkLongitude", 0)
                If ds.Rows(0)("PlacemarkLat").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLat"), "PlacemarkLatitude", 0)
                If ds.Rows(0)("PlacemarkLonEnd").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLonEnd"), "PlacemarkLongitudeEnd", 1)
                If ds.Rows(0)("PlacemarkLatEnd").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLatEnd"), "PlacemarkLatitudeEnd", 1)
                If ds.Rows(0)("TimeStart").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("TimeStart"), "PlacemarkStartTime", 0)
                If ds.Rows(0)("TimeEnd").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("TimeEnd"), "PlacemarkEndTime", 0)
                'color density
                txtMultiplyBy0.Text = ds.Rows(0)("ColDensMultBy").ToString
                Dim colr As String = ds.Rows(0)("HighDensColor").ToString
                LabelColorSaved.ForeColor = ColorTranslator.FromHtml(colr)
                Session("color") = colr
                Session("ColorField") = ds.Rows(0)("ColorDensField").ToString
                If ds.Rows(0)("ColorDensField").ToString.Trim <> "" Then
                    DropDownListColorDens.Text = ds.Rows(0)("ColorDensField").ToString
                    ret = AddField(DropDownListColorDens.Text, "ColorField", 0, colr)
                End If
                'extruded
                txtMultiplyBy.Text = ds.Rows(0)("ExtrAltMultBy").ToString
                If ds.Rows(0)("ExtrAltField").ToString.Trim <> "" Then
                    DropDownListExtruded.Text = ds.Rows(0)("ExtrAltField").ToString
                    Session("ExtrudedField") = DropDownListExtruded.Text
                Else
                    Session("ExtrudedField") = ""
                End If
                If ds.Rows(0)("ExtrAltColorField").ToString.Trim <> "" Then
                    DropDownListExtrudedColor.Text = ds.Rows(0)("ExtrAltColorField").ToString
                    Session("ExtrudedColor") = DropDownListExtrudedColor.Text
                Else
                    Session("ExtrudedColor") = ""
                End If
                If ds.Rows(0)("ExtrAltField").ToString.Trim <> "" OrElse ds.Rows(0)("ExtrAltColorField").ToString.Trim <> "" Then
                    ret = AddField(DropDownListExtruded.Text, "ExtrudedField", 0, txtMultiplyBy.Text, DropDownListExtrudedColor.Text)
                End If
                'keyfields
                Dim kflds As String = ds.Rows(0)("KeyFields").ToString
                Dim kfs() As String = kflds.Split(",")
                For i = 0 To kfs.Length - 1
                    If kfs(i).Trim <> "" Then
                        ret = AddField(kfs(i).Trim, "KeyField", 0)
                    End If
                Next
                'additional points
                Dim k As Integer = 1
                Dim addpnts As String = ds.Rows(0)("AddPointsFields").ToString
                Dim adpts() As String = addpnts.Split("|")
                For i = 0 To adpts.Length - 1
                    If adpts(i).Trim <> "" AndAlso adpts(i).Contains("]") AndAlso adpts(i).Trim.Length > 4 Then
                        ret = adpts(i).Trim.Substring(4, adpts(i).Trim.IndexOf("]") - 4)
                        If IsNumeric(ret) Then k = CInt(ret)
                        If adpts(i).Trim.StartsWith("lon[") AndAlso adpts(i).Contains("]") Then
                            ret = AddField(adpts(i).Substring(adpts(i).Trim.IndexOf("]") + 1), "PlacemarkLongitude", k)
                        ElseIf adpts(i).Trim.StartsWith("lat[") AndAlso adpts(i).Contains("]") Then
                            ret = AddField(adpts(i).Substring(adpts(i).Trim.IndexOf("]") + 1), "PlacemarkLatitude", k)
                        End If
                    End If
                Next
                'descriptions
                Dim descs As String = ds.Rows(0)("Descriptions").ToString
                Dim des() As String = descs.Split("|")
                For i = 0 To des.Length - 1
                    If des(i).Trim <> "" AndAlso des(i).Contains("*") AndAlso des(i).Trim.Length > 4 Then
                        ret = des(i).Trim.Substring(4, des(i).Trim.IndexOf("*") - 4)
                        If IsNumeric(ret) Then k = CInt(ret)
                        If des(i).Trim.StartsWith("txt#") AndAlso des(i).Contains("*") Then
                            txtDescr.Text = des(i).Substring(des(i).Trim.IndexOf("*") + 1)
                            ret = AddDescrText("PlacemarkDescription", k)
                        ElseIf des(i).Trim.StartsWith("fld#") AndAlso des(i).Contains("*") Then
                            ret = AddField(des(i).Substring(des(i).Trim.IndexOf("*") + 1), "PlacemarkDescription", k)
                        End If
                    End If
                Next
                LoadKMLData(txtMapName.Text)
            Else
                Session("Comments") = ""
                Session("txtMapName") = txtMapName.Text
                txtMapName.ToolTip = "Map Name to update"
                'LoadKMLData(txtMapName.Text)
                txtComments.Text = ""
                txtComments.ToolTip = txtComments.Text
                hdnInitAltitude.Value = "4000"
                hdnLineWidth.Value = "4"
                hdnShowCircles.Value = "false"
                hdnShowLinks.Value = "false"
                hdnShowPins.Value = "false"
                Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
            End If
            Session("txtMapName") = txtMapName.Text
        End If
    End Sub

    Private Sub btnLinkDown_Click(sender As Object, e As EventArgs) Handles btnLinkDown.Click
        Dim ret As String = String.Empty
        Try
            'save definition for georestrictions, altit, linewith
            'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
            'ret = ExequteSQLquery(updSQL)
            ret = SaveMapDefinition(ret)
            If ret.StartsWith("ERROR!!") Then
                MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            End If
            'make KML for Google Earth Pro
            ret = MakeKMLforGoogleMap(False, Session("latlon").ToString)
            If ret.StartsWith("ERROR!!") OrElse Session("kmlfile") Is Nothing OrElse Session("kmlfile").ToString.Trim = "" Then  'OrElse File.ReadAllText(Session("kmlfile").ToString).Trim.Length > 0 
                'file does not exist
                MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            Else
                'file exists
                MessageBox.Show("KML file generated", "KML file generated", "KMLgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            End If

            'open in Google Earth
            Dim myfile As String = Session("kmlfile").ToString.Replace(Session("appldirKMLFiles"), "")
            Dim dirpath As String = Session("appldirKMLFiles") & myfile.Replace(".kml", "") & "\"
            'Directory.Delete(dirpath)
            Directory.CreateDirectory(dirpath)
            File.Copy(Session("kmlfile"), dirpath & myfile)

            myfile = myfile.Replace(".kml", ".kmz")
            Try
                ZipFile.CreateFromDirectory(dirpath, Session("appldirKMLFiles") & myfile)
            Catch ex As Exception
            End Try
            Try
                Response.ContentType = "application/octet-stream"
                Response.AppendHeader("Content-Disposition", "attachment; filename=" & myfile)
                Response.TransmitFile(Session("appldirKMLFiles") & myfile)
            Catch ex As Exception
                ret = "ERROR!!  " & ex.Message
            End Try

            File.Delete(dirpath & myfile.Replace(".kmz", ".kml"))
            Directory.Delete(dirpath)
            Response.End()
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        LabelAlert.Text = ret
    End Sub
    Private Sub TreeView1_SelectedNodeChanged(sender As Object, e As EventArgs) Handles TreeView1.SelectedNodeChanged
        Dim node As WebControls.TreeNode = TreeView1.SelectedNode
        Dim url As String = "~/" & node.Value
        Dim urlBack As String = HttpContext.Current.Request.Url.ToString
        Dim nodeText As String = node.Text

        If nodeText.Contains("Advanced Report Designer") Then
            Session("urlback") = urlBack
        Else
            Session("urlback") = Nothing
        End If
        Response.Redirect(url)
    End Sub
    Protected Sub ButtonAddMapName_Click(sender As Object, e As EventArgs) Handles ButtonAddMapName.Click
        Dim mname As String = txtMapName.Text
        Dim SelectedName As String = String.Empty

        If mname = String.Empty Then
            MessageBox.Show("Map name has not been entered.", "Add Map", "NoMapName", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Exit Sub
        End If

        If DropDownMapNames.SelectedItem IsNot Nothing Then _
            SelectedName = DropDownMapNames.SelectedItem.Text.Trim

        Session("txtMapName") = mname

        If DropDownMapNames.Visible = False Then
            Label4.Visible = True
            DropDownMapNames.Visible = True
            DropDownMapNames.Enabled = True
            Dim sel As ListItem = DropDownMapNames.Items.FindByText(mname)
            If sel IsNot Nothing Then
                Dim indx As Integer = DropDownMapNames.Items.IndexOf(sel)
                DropDownMapNames.SelectedIndex = indx
                DropDownMapNames.Text = mname
                DropDownMapNames_SelectedIndexChanged(DropDownMapNames, Nothing)
            Else
                Dim n As Integer = DropDownMapNames.Items.Count
                DropDownMapNames.Items.Add(mname)
                DropDownMapNames.SelectedIndex = n
                DropDownMapNames.Text = mname
            End If
            Exit Sub
        End If
        Dim i As Integer
        For i = 0 To DropDownMapNames.Items.Count - 1
            If DropDownMapNames.Items(i).Text = mname Then
                DropDownMapNames.SelectedIndex = i
                DropDownMapNames.SelectedItem.Text = mname
                DropDownMapNames.Text = mname
                DropDownMapNames_SelectedIndexChanged(DropDownMapNames, Nothing)
                Exit Sub
            End If
        Next
        If i = DropDownMapNames.Items.Count Then
            DropDownMapNames.Items.Add(mname)
            DropDownMapNames.Text = mname
            DropDownMapNames.SelectedIndex = i
        End If

        If SelectedName.Contains("*(") AndAlso SelectedName.Contains(")") Then
            Dim ds As DataTable = GetHistoricKMLsetting(Session("REPORTID"), SelectedName)
            Dim ret As String = ""

            If ds IsNot Nothing And ds.Rows.Count > 0 Then
                hdnInitAltitude.Value = ds.Rows(0)("InitAltit").ToString
                txtInitialAltit.Text = hdnInitAltitude.Value
                hdnLineWidth.Value = ds.Rows(0)("LineWidth").ToString
                txtLineWidth.Text = hdnLineWidth.Value
                Session("Comments") = txtComments.Text
                Session("initalt") = txtInitialAltit.Text
                Session("linewidth") = txtLineWidth.Text
                Session("order") = 0
                txtGeoRestrictions.Text = ds.Rows(0)("Prop4").ToString

                If ds.Rows(0)("PlacemarkName").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkName"), "PlacemarkName", 0)
                If ds.Rows(0)("PlacemarkLon").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLon"), "PlacemarkLongitude", 0)
                If ds.Rows(0)("PlacemarkLat").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLat"), "PlacemarkLatitude", 0)
                If ds.Rows(0)("PlacemarkLonEnd").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLonEnd"), "PlacemarkLongitudeEnd", 1)
                If ds.Rows(0)("PlacemarkLatEnd").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("PlacemarkLatEnd"), "PlacemarkLatitudeEnd", 1)
                If ds.Rows(0)("TimeStart").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("TimeStart"), "PlacemarkStartTime", 0)
                If ds.Rows(0)("TimeEnd").ToString.Trim <> "" Then ret = AddField(ds.Rows(0)("TimeEnd"), "PlacemarkEndTime", 0)

                Dim colr As String = ds.Rows(0)("HighDensColor").ToString
                If ds.Rows(0)("ColorDensField").ToString.Trim <> "" Then
                    DropDownListColorDens.Text = ds.Rows(0)("ColorDensField").ToString
                    ret = AddField(DropDownListColorDens.Text, "ColorField", 0, colr)
                End If

                'extruded
                txtMultiplyBy.Text = ds.Rows(0)("ExtrAltMultBy").ToString
                If ds.Rows(0)("ExtrAltField").ToString.Trim <> "" Then
                    DropDownListExtruded.Text = ds.Rows(0)("ExtrAltField").ToString
                    Session("ExtrudedField") = DropDownListExtruded.Text
                Else
                    Session("ExtrudedField") = ""
                End If
                If ds.Rows(0)("ExtrAltColorField").ToString.Trim <> "" Then
                    DropDownListExtrudedColor.Text = ds.Rows(0)("ExtrAltColorField").ToString
                    Session("ExtrudedColor") = DropDownListExtrudedColor.Text
                Else
                    Session("ExtrudedColor") = ""
                End If
                If ds.Rows(0)("ExtrAltField").ToString.Trim <> "" OrElse ds.Rows(0)("ExtrAltColorField").ToString.Trim <> "" Then
                    ret = AddField(DropDownListExtruded.Text, "ExtrudedField", 0, txtMultiplyBy.Text, DropDownListExtrudedColor.Text)
                End If


                'keyfields
                Dim kflds As String = ds.Rows(0)("KeyFields").ToString
                Dim kfs() As String = kflds.Split(",")
                For i = 0 To kfs.Length - 1
                    If kfs(i).Trim <> "" Then
                        ret = AddField(kfs(i).Trim, "KeyField", 0)
                    End If
                Next

                'additional points
                Dim k As Integer = 1
                Dim addpnts As String = ds.Rows(0)("AddPointsFields").ToString
                Dim adpts() As String = addpnts.Split("|")
                For i = 0 To adpts.Length - 1
                    If adpts(i).Trim <> "" AndAlso adpts(i).Contains("]") AndAlso adpts(i).Trim.Length > 4 Then
                        ret = adpts(i).Trim.Substring(4, adpts(i).Trim.IndexOf("]") - 4)
                        If IsNumeric(ret) Then k = CInt(ret)
                        If adpts(i).Trim.StartsWith("lon[") AndAlso adpts(i).Contains("]") Then
                            ret = AddField(adpts(i).Substring(adpts(i).Trim.IndexOf("]") + 1), "PlacemarkLongitude", k)
                        ElseIf adpts(i).Trim.StartsWith("lat[") AndAlso adpts(i).Contains("]") Then
                            ret = AddField(adpts(i).Substring(adpts(i).Trim.IndexOf("]") + 1), "PlacemarkLatitude", k)
                        End If
                    End If
                Next
                'descriptions
                Dim descs As String = ds.Rows(0)("Descriptions").ToString
                Dim des() As String = descs.Split("|")
                For i = 0 To des.Length - 1
                    If des(i).Trim <> "" AndAlso des(i).Contains("*") AndAlso des(i).Trim.Length > 4 Then
                        ret = des(i).Trim.Substring(4, des(i).Trim.IndexOf("*") - 4)
                        If IsNumeric(ret) Then k = CInt(ret)
                        If des(i).Trim.StartsWith("txt#") AndAlso des(i).Contains("*") Then
                            txtDescr.Text = des(i).Substring(des(i).Trim.IndexOf("*") + 1)
                            ret = AddDescrText("PlacemarkDescription", k)
                        ElseIf des(i).Trim.StartsWith("fld#") AndAlso des(i).Contains("*") Then
                            ret = AddField(des(i).Substring(des(i).Trim.IndexOf("*") + 1), "PlacemarkDescription", k)
                        End If
                    End If
                Next
                LoadKMLData(txtMapName.Text)
            End If
        End If
    End Sub
    Protected Sub btnOpenGoogleMap_Click(sender As Object, e As EventArgs) Handles btnOpenGoogleMap.Click
        'Dim myProcess As System.Diagnostics.Process = New System.Diagnostics.Process
        'myProcess.StartInfo.UseShellExecute = False
        'myProcess.StartInfo.FileName = "C:\\Program Files\Google\Google Earth Pro\client\googleearth.exe"
        'myProcess.StartInfo.CreateNoWindow = True
        'myProcess.Start()
        'Response.Redirect("MapReport.aspx?openkml=yes")

        'save definition for georestrictions, altit, linewith
        'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        'Dim ret As String = ExequteSQLquery(updSQL)
        Dim ret As String = String.Empty
        ret = SaveMapDefinition(ret)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        End If
        'make KML for Google Map
        ret = MakeKMLforGoogleMap(True, Session("latlon").ToString)
        If ret = "MapType is not supported in Google Maps" Then
            MessageBox.Show("MapType is not supported in Google Maps. Try to open it in Google Earth Pro", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        ElseIf ret.StartsWith("FILE TOO BIG FOR GOOGLE MAP!! ") Then
            MessageBox.Show("KML file is too big for Google Maps. Try to open it in Google Earth Pro", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        ElseIf ret.StartsWith("ERROR!!") OrElse Session("kmlfile") Is Nothing OrElse Session("kmlfile").ToString.Trim = "" Then  'OrElse File.ReadAllText(Session("kmlfile").ToString).Trim.Length > 0 
            'file does not exist
            If ret.StartsWith("ERROR!!") Then
                MessageBox.Show(ret, "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Else
                MessageBox.Show("Error!! KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            End If
            Exit Sub
        Else
            'file exists
            'MessageBox.Show("KML file generated", "KML file generated", "KMLgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        'If txtGeoRestrictions.Text.Trim <> "" Then

        'End If
        'open in Google Maps
        Response.Redirect("MapGoogle.aspx?openkml=yes&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub

    Private Sub DropDownMapType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownMapType.SelectedIndexChanged
        'Session("maptypeindex") = DropDownMapType.SelectedIndex
        Session("maptype") = DropDownMapType.SelectedItem.Text
        btnPlacemarkLongitude.Text = "Placemark Longitude"
        btnPlacemarkLatitude.Text = "Placemark Latitude"
        btnPlacemarkLongitude2.Text = "Placemark end Longitude"
        btnPlacemarkLatitude2.Text = "Placemark end Latitude"
        If DropDownMapType.Text = "Pins" Or DropDownMapType.Text = "Circles" Or DropDownMapType.Text = "Floating placemark" Or DropDownMapType.Text = "Extruded placemark" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = False
            btnPlacemarkLongitude2.Enabled = False
            btnPlacemarkLatitude2.Visible = False
            btnPlacemarkLatitude2.Enabled = False
            trAddPoints.Visible = False
            trKeyFields.Visible = False
            trExtruded.Visible = True
            tblKeyFields.Visible = False
        ElseIf DropDownMapType.Text = "Paths" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Extruded Paths" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Extruded Polygons" Then
            btnPlacemarkLongitude.Text = "Placemark border Longitude"
            btnPlacemarkLatitude.Text = "Placemark border Latitude"
            btnPlacemarkLongitude2.Text = "Placemark pin Longitude"
            btnPlacemarkLatitude2.Text = "Placemark pin Latitude"
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Tours" Then
            btnPlacemarkStartTime.Visible = True
            btnPlacemarkStartTime.Enabled = True
            btnPlacemarkEndTime.Visible = True
            btnPlacemarkEndTime.Enabled = True
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            trAddPoints.Visible = False
            trKeyFields.Visible = False
            tblKeyFields.Visible = False
            trExtruded.Visible = True
        ElseIf DropDownMapType.Text = "Polygons" Then
            btnPlacemarkLongitude.Text = "Placemark border Longitude"
            btnPlacemarkLatitude.Text = "Placemark border Latitude"
            btnPlacemarkLongitude2.Text = "Placemark pin Longitude"
            btnPlacemarkLatitude2.Text = "Placemark pin Latitude"
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = True
            btnPlacemarkLongitude2.Enabled = True
            btnPlacemarkLatitude2.Visible = True
            btnPlacemarkLatitude2.Enabled = True
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trAddDescr.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        ElseIf DropDownMapType.Text = "Curve" Then
            btnPlacemarkStartTime.Visible = False
            btnPlacemarkStartTime.Enabled = False
            btnPlacemarkEndTime.Visible = False
            btnPlacemarkEndTime.Enabled = False
            btnPlacemarkLongitude2.Visible = False
            btnPlacemarkLongitude2.Enabled = False
            btnPlacemarkLatitude2.Visible = False
            btnPlacemarkLatitude2.Enabled = False
            trAddPoints.Visible = True
            trKeyFields.Visible = True
            trAddDescr.Visible = True
            trExtruded.Visible = True
            tblKeyFields.Visible = True
        End If
    End Sub
    Protected Sub DropDownMapFields_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownMapFields.SelectedIndexChanged
        Dim fldindx = DropDownMapFields.SelectedIndex
    End Sub
    Protected Sub btnAddToDescription_Click(sender As Object, e As EventArgs) Handles btnAddToDescription.Click
        Dim n As Integer = 0
        Dim ret As String = String.Empty
        Try
            Dim dtf As DataTable = GetReportMapFields(repid, txtMapName.Text)
            If dtf IsNot Nothing AndAlso dtf.Rows.Count > 0 Then
                n = dtf.Rows(dtf.Rows.Count - 1)("ord") + 1
                Session("Order") = n
                If txtDescr.Text.Trim <> "" Then
                    ret = AddDescrText("PlacemarkDescription", n)
                End If
                If DropDownMapFields1.Text.Trim <> "" Then
                    ret = AddField(DropDownMapFields1.Text, "PlacemarkDescription", n)
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            LabelAlert.Text = ret
            LabelAlert.Visible = True
        End Try
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)

    End Sub

    Protected Sub btnAddPointLong_Click(sender As Object, e As EventArgs) Handles btnAddPointLong.Click
        Dim ret As String = String.Empty
        Dim k As Integer
        Try
            Dim dtf As DataTable = GetReportMapFields(repid, txtMapName.Text)
            If dtf IsNot Nothing AndAlso dtf.Rows.Count > 0 Then
                k = dtf.Rows(dtf.Rows.Count - 1)("ord") + 1
                Session("Order") = k
                If DropDownMapFields2.Text.Trim <> "" Then
                    ret = AddField(DropDownMapFields2.Text, "PlacemarkLongitude", k)
                End If
                'If DropDownMapFields3.Text.Trim <> "" Then
                '    ret = AddField(DropDownMapFields3.Text, "PlacemarkLatitude", k)
                'End If
            End If
        Catch ex As Exception
            ret = ex.Message
            LabelAlert.Text = ret
            LabelAlert.Visible = True
        End Try
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)

    End Sub
    Protected Sub btnAddPointLat_Click(sender As Object, e As EventArgs) Handles btnAddPointLat.Click
        Dim ret As String = String.Empty
        Dim k As Integer
        Try
            Dim dtf As DataTable = GetReportMapFields(repid, txtMapName.Text)
            If dtf IsNot Nothing AndAlso dtf.Rows.Count > 0 Then
                k = dtf.Rows(dtf.Rows.Count - 1)("ord") + 1
                Session("Order") = k
                'If DropDownMapFields2.Text.Trim <> "" Then
                '    ret = AddField(DropDownMapFields2.Text, "PlacemarkLongitude", k)
                'End If
                If DropDownMapFields2.Text.Trim <> "" Then
                    ret = AddField(DropDownMapFields3.Text, "PlacemarkLatitude", k)
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            LabelAlert.Text = ret
            LabelAlert.Visible = True
        End Try
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value)

    End Sub
    Protected Sub btnAddKeyField_Click(sender As Object, e As EventArgs) Handles btnAddKeyField.Click
        Dim ret As String = String.Empty
        If DropDownKeyFields.SelectedItem.Text.Trim <> "" Then
            ret = AddField(DropDownKeyFields.SelectedItem.Text, "KeyField", 0)
        End If
        'save definition for georestrictions, altit, linewith
        'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        'ret = ExequteSQLquery(updSQL)
        ret = SaveMapDefinition(ret)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnColorDensity_Click(sender As Object, e As EventArgs) Handles btnColorDensity.Click
        Dim ret As String = String.Empty
        'If txtMultiplyBy0.Text.Trim = "" OrElse txtMultiplyBy0.Text.Trim = "0" Then
        '    txtMultiplyBy0.Text = "1"
        'End If
        'If Request("colornum") IsNot Nothing AndAlso DropDownListColorDens.SelectedItem.Text.Trim <> "" Then
        '    Dim colr As String = Request("colornum")
        '    LabelColorSaved.ForeColor = ColorTranslator.FromHtml(colr)
        '    Session("color") = colr
        '    Session("ColorField") = DropDownListColorDens.SelectedItem.Text
        '    ret = AddField(DropDownListColorDens.SelectedItem.Text, "ColorField", 0, colr)
        'End If
        'save definition for georestrictions, altit, linewith
        'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        'ret = ExequteSQLquery(updSQL)

        ret = SaveMapDefinition(ret)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Protected Sub btnExtrudedAltitude_Click(sender As Object, e As EventArgs) Handles btnExtrudedAltitude.Click
        Dim ret As String = String.Empty
        'If txtMultiplyBy.Text.Trim = 0 Then
        '    txtMultiplyBy.Text = "1"
        'End If
        'If DropDownListExtruded.SelectedItem.Text.Trim <> "" Or DropDownListExtrudedColor.SelectedItem.Text.Trim <> "" Then
        '    ret = AddField(DropDownListExtruded.SelectedItem.Text, "ExtrudedField", 0, txtMultiplyBy.Text, DropDownListExtrudedColor.Text)
        '    Session("ExtrudedField") = DropDownListExtruded.SelectedItem.Text
        '    Session("ExtrudedColor") = DropDownListExtrudedColor.SelectedItem.Text
        'Else
        '    Session("ExtrudedField") = ""
        '    Session("ExtrudedColor") = ""
        'End If
        ''save definition for georestrictions, altit, linewith
        'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        'ret = ExequteSQLquery(updSQL)
        ret = SaveMapDefinition(ret)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub
    Private Function SaveMapDefinition(ByVal er As String) As String
        Dim ret As String = String.Empty
        If txtMultiplyBy0.Text.Trim = "" OrElse txtMultiplyBy0.Text.Trim = "0" Then
            txtMultiplyBy0.Text = "1"
        End If
        If Request("colornum") IsNot Nothing AndAlso DropDownListColorDens.SelectedItem.Text.Trim <> "" Then
            Dim colr As String = Request("colornum")
            LabelColorSaved.ForeColor = ColorTranslator.FromHtml(colr)
            Session("color") = colr
            Session("ColorField") = DropDownListColorDens.SelectedItem.Text
            ret = AddField(DropDownListColorDens.SelectedItem.Text, "ColorField", 0, colr)
        End If
        If txtMultiplyBy.Text.Trim = 0 Then
            txtMultiplyBy.Text = "1"
        End If
        If DropDownListExtruded.SelectedItem.Text.Trim <> "" Or DropDownListExtrudedColor.SelectedItem.Text.Trim <> "" Then
            ret = AddField(DropDownListExtruded.SelectedItem.Text, "ExtrudedField", 0, txtMultiplyBy.Text, DropDownListExtrudedColor.Text)
            Session("ExtrudedField") = DropDownListExtruded.SelectedItem.Text
            Session("ExtrudedColor") = DropDownListExtrudedColor.SelectedItem.Text
        Else
            Session("ExtrudedField") = ""
            Session("ExtrudedColor") = ""
        End If
        'save maptype, show pins, show circles, show links
        ret = AddField(DropDownMapType.SelectedItem.Text, "MapType", 0)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Adding the field crashed.", "Add Map field", "ERROR!", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Return ret
        End If
        ret = AddField(chkShowPins.Checked.ToString, "ShowPins", 0)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Adding the field crashed.", "Add Map field", "ERROR!", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Return ret
        End If
        ret = AddField(chkShowCircles.Checked.ToString, "ShowCircles", 0)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Adding the field crashed.", "Add Map field", "ERROR!", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Return ret
        End If
        ret = AddField(chkShowLinks.Checked.ToString, "ShowLinks", 0)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Adding the field crashed.", "Add Map field", "ERROR!", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Return ret
        End If
        'save definition for georestrictions, altit, linewith in the row where map name is.
        Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        ret = ExequteSQLquery(updSQL)
        If ret <> "Query executed fine." Then
            ret = "ERROR!! " & ret
        End If
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Updating the map definishion crashed.", "Update Map fields", "ERROR!", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Return ret
        End If
        Return ret
    End Function

    Private Sub DropDownListColorDens_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListColorDens.SelectedIndexChanged
        Session("ColorField") = DropDownListColorDens.SelectedItem.Text
    End Sub

    Private Sub DropDownListExtruded_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListExtruded.SelectedIndexChanged
        Session("ExtrudedField") = DropDownListExtruded.SelectedItem.Text
    End Sub
    Private Sub DropDownListExtrudedColor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListExtrudedColor.SelectedIndexChanged
        Session("ExtrudedColor") = DropDownListExtrudedColor.SelectedItem.Text
    End Sub
    Protected Sub ButtonSaveHistory_Click(sender As Object, e As EventArgs) Handles ButtonSaveHistory.Click
        Dim ret As String = String.Empty
        Dim mapname As String = txtMapName.Text.Trim
        Dim dm As DataTable = GetReportMapFields(repid, mapname)
        Dim dcl As DataTable = GetReportColorField(repid, mapname)
        Dim dce As DataTable = GetReportExtrudedFields(repid, mapname)
        Dim drk As DataTable = GetReportKeyFields(repid, mapname)
        If dm Is Nothing Then
            ret = "no Map fields found"
            MessageBox.Show("Nothing to save", "Nothing to save", "KMLnotsaved", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        End If
        Dim i As Integer
        Dim lon As String = String.Empty
        Dim lat As String = String.Empty
        Dim nm As String = String.Empty
        Dim des As String = String.Empty
        Dim lonend As String = String.Empty
        Dim latend As String = String.Empty
        Dim starttimecol As String = String.Empty
        Dim endtimecol As String = String.Empty
        Dim lonadd As String = String.Empty
        Dim latadd As String = String.Empty
        Dim coordadd As String = String.Empty
        For i = 0 To dm.Rows.Count - 1
            If dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkName" AndAlso dm.Rows(i)("ord") = 0 Then
                nm = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkDescription" Then
                If dm.Rows(i)("descrtext").ToString <> "" Then
                    des = des & "txt#" & dm.Rows(i)("ord") & "*" & dm.Rows(i)("descrtext").ToString & "|"
                Else
                    des = des & "fld#" & dm.Rows(i)("ord") & "*" & dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1) & "|"
                End If
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitude" AndAlso dm.Rows(i)("ord") = 0 Then
                lon = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitude" AndAlso dm.Rows(i)("ord") = 0 Then
                lat = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
                lonend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
                latend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkStartTime" Then
                starttimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkEndTime" Then
                endtimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitude" AndAlso dm.Rows(i)("ord") > 0 Then
                lonadd = coordadd & "lon[" & dm.Rows(i)("ord") & "]" & dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1) & "|"
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitude" AndAlso dm.Rows(i)("ord") > 0 Then
                latadd = coordadd & "lat[" & dm.Rows(i)("ord") & "]" & dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1) & "|"

            End If
        Next
        Dim savedat As String = Session("logon") & Session("logonindx") & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        If mapname.Contains(" *(") Then 'from history
            mapname = mapname.Substring(0, mapname.IndexOf(" *(")).Trim()
        End If
        If mapname.Contains(" -#") Then 'from history
            mapname = mapname.Substring(0, mapname.IndexOf(" -#")).Trim
        End If
        Dim msql As String = ""
        Dim sFields As String = "Saved,"
        sFields &= "ReportId,"
        sFields &= "MapName,"
        sFields &= "MapType,"
        sFields &= "ShowPins,"
        sFields &= "ShowCircles,"
        sFields &= "ShowLinks,"
        sFields &= "PlacemarkName,"
        sFields &= "PlacemarkLon,"
        sFields &= "PlacemarkLat,"
        sFields &= "PlacemarkLonEnd,"
        sFields &= "PlacemarkLatEnd,"
        sFields &= "TimeStart,"
        sFields &= "TimeEnd,"
        sFields &= "InitAltit,"
        sFields &= "LineWidth,"
        sFields &= "Descriptions,"
        sFields &= "AddPointsFields,"
        sFields &= "Comments,"
        sFields &= "Prop4,"  'GeoRestrictions
        sFields &= "Prop7,"  'latlon
        If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
            sFields &= "ColorDensField,"
            sFields &= "ColDensMultBy,"
            sFields &= "HighDensColor,"
        End If
        If dce IsNot Nothing AndAlso dce.Rows.Count > 0 Then
            sFields &= "ExtrAltField,"
            sFields &= "ExtrAltMultBy,"
            sFields &= "ExtrAltColorField,"
        End If
        If drk IsNot Nothing AndAlso drk.Rows.Count > 0 Then
            sFields &= "KeyFields"
        End If

        sFields = sFields.Trim
        If sFields.EndsWith(",") Then _
            sFields = sFields.Substring(0, sFields.Length - 1)

        Dim sValues As String = "'" & savedat & "',"
        sValues &= "'" & Session("REPORTID") & "',"
        sValues &= "'" & mapname & "',"
        sValues &= "'" & DropDownMapType.Text.Trim & "',"
        If chkShowPins.Checked Then
            sValues &= "1,"
        Else
            sValues &= "0,"
        End If
        If chkShowCircles.Checked Then
            sValues &= "1,"
        Else
            sValues &= "0,"
        End If
        If chkShowLinks.Checked Then
            sValues &= "1,"
        Else
            sValues &= "0,"
        End If

        sValues &= "'" & nm & "',"
        sValues &= "'" & lon & "',"
        sValues &= "'" & lat & "',"
        sValues &= "'" & lonend & "',"
        sValues &= "'" & latend & "',"
        sValues &= "'" & starttimecol & "',"
        sValues &= "'" & endtimecol & "',"
        sValues &= "'" & txtInitialAltit.Text.Trim & "',"
        sValues &= "'" & txtLineWidth.Text.Trim & "',"
        sValues &= "'" & des & "',"
        sValues &= "'" & coordadd & "',"
        sValues &= "'" & txtComments.Text.Trim & "',"
        sValues &= "'" & txtGeoRestrictions.Text.Trim & "',"
        sValues &= "'" & chkLatLonOrLonLat.Checked.ToString & "',"
        If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
            sValues &= "'" & dcl.Rows(0)("Val").ToString.Trim & "',"
            sValues &= "'" & dcl.Rows(0)("Prop5").ToString.Trim & "',"
            sValues &= "'" & dcl.Rows(0)("Prop4").ToString.Trim & "',"
        End If
        If dce IsNot Nothing AndAlso dce.Rows.Count > 0 Then
            sValues &= "'" & dce.Rows(0)("Val").ToString.Trim & "',"
            sValues &= "'" & dce.Rows(0)("Prop4").ToString.Trim & "',"
            sValues &= "'" & dce.Rows(0)("Prop5").ToString.Trim & "',"
        End If
        If drk IsNot Nothing AndAlso drk.Rows.Count > 0 Then
            Dim keys As String = String.Empty
            For j = 0 To drk.Rows.Count - 1
                If drk.Rows(j)("ForMap").ToString = "KeyField" Then
                    keys = keys & drk.Rows(j)("KeyField").ToString() & ","
                End If
            Next
            sValues &= "'" & keys & "'"
        End If

        sValues = sValues.Trim
        If sValues.EndsWith(",") Then _
           sValues = sValues.Substring(0, sValues.Length - 1)

        msql = "INSERT INTO ourkmlhistory (" & sFields & ") VALUES (" & sValues & ")"

        ret = ExequteSQLquery(msql)
        If ret = "Query executed fine." Then
            MessageBox.Show("KML saved for future use.", "KML saved for future use", "KMLsavedToHistory", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Saving crashed", "Saving crashed", "KMLnotsaved", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        'Dim msql As String = "INSERT INTO ourkmlhistory SET SAVED='" & savedat & "', "

        'msql = msql & " ReportId='" & Session("REPORTID") & "', "
        'msql = msql & " MapName='" & mapname & "', "
        'msql = msql & " MapType='" & DropDownMapType.Text.Trim & "', "
        'If chkShowPins.Checked Then
        '    msql = msql & " ShowPins=1, "
        'Else
        '    msql = msql & " ShowPins=0, "
        'End If
        'If chkShowCircles.Checked Then
        '    msql = msql & " ShowCircles=1, "
        'Else
        '    msql = msql & " ShowCircles=0, "
        'End If
        'If chkShowLinks.Checked Then
        '    msql = msql & " ShowLinks=1, "
        'Else
        '    msql = msql & " ShowLinks=0, "
        'End If
        'hdnShowPins.Value = chkShowPins.Checked
        'hdnShowCircles.Value = chkShowCircles.Checked
        'hdnShowLinks.Value = chkShowLinks.Checked
        'msql = msql & " PlacemarkName='" & nm & "', "
        'msql = msql & " PlacemarkLon='" & lon & "', "
        'msql = msql & " PlacemarkLat='" & lat & "', "
        'msql = msql & " PlacemarkLonEnd='" & lonend & "', "
        'msql = msql & " PlacemarkLatEnd='" & latend & "', "
        'msql = msql & " TimeStart='" & starttimecol & "', "
        'msql = msql & " TimeEnd='" & endtimecol & "', "
        'msql = msql & " InitAltit=" & txtInitialAltit.Text.Trim & ", "
        'msql = msql & " LineWidth=" & txtLineWidth.Text.Trim & ", "
        'msql = msql & " Descriptions='" & des & "', "
        'msql = msql & " AddPointsFields='" & coordadd & "', "
        'msql = msql & " Comments='" & txtComments.Text.Trim & "', "
        'msql = msql & " Prop4='" & txtGeoRestrictions.Text.Trim & "', "
        'If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
        '    msql = msql & " ColorDensField='" & dcl.Rows(0)("Val").ToString.Trim & "', "
        '    msql = msql & " ColDensMultBy='" & dcl.Rows(0)("Prop5").ToString.Trim & "', "
        '    msql = msql & " HighDensColor='" & dcl.Rows(0)("Prop4").ToString.Trim & "', "
        'End If
        'If dce IsNot Nothing AndAlso dce.Rows.Count > 0 Then
        '    msql = msql & " ExtrAltField='" & dce.Rows(0)("Val").ToString.Trim & "', "
        '    msql = msql & " ExtrAltMultBy='" & dce.Rows(0)("Prop4").ToString.Trim & "', "
        '    msql = msql & " ExtrAltColorField='" & dce.Rows(0)("Prop5").ToString.Trim & "', "
        'End If
        'If drk IsNot Nothing AndAlso drk.Rows.Count > 0 Then
        '    Dim keys As String = String.Empty
        '    For j = 0 To drk.Rows.Count - 1
        '        If drk.Rows(j)("ForMap").ToString = "KeyField" Then
        '            keys = keys & drk.Rows(j)("KeyField").ToString() & ","
        '        End If
        '    Next
        '    msql = msql & " KeyFields='" & keys & "' "
        'End If
        'msql = msql.Trim
        'If msql.EndsWith(",") Then
        '    msql = msql.Substring(0, msql.Length - 1)
        'End If

        'ret = ExequteSQLquery(msql)
        'If ret = "Query executed fine." Then
        '    MessageBox.Show("KML saved for future use.", "KML saved for future use", "KMLsavedToHistory", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        'Else
        '    MessageBox.Show("Saving crashed", "Saving crashed", "KMLnotsaved", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        'End If
    End Sub
    Private Sub DeleteHistoryItem(sMapName As String, sSaved As String)
        'not being used - for development only
        Dim msql As String = "DELETE FROM ourkmlhistory WHERE ReportID='" & Session("REPORTID") & "' AND MapName='" & sMapName & "' AND Saved='" & sSaved & "'"
        Dim ret As String = ExequteSQLquery(msql)
    End Sub
    Private Sub DeleteMap(sMapName As String)
        Dim msql As String = "DELETE FROM OURReportFormat WHERE ReportID='" & Session("REPORTID") & "' AND Prop3='" & sMapName & "'"
        Dim ret As String = ExequteSQLquery(msql)
        If ret = "Query executed fine." Then

            If Not MapExists(Session("REPORTID"), sMapName) Then
                MessageBox.Show(sMapName & " deleted.", "Delete Map", "Mapdeleted", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            Else
                MessageBox.Show(sMapName & " was Not deleted.", "Delete Map", "Mapnotdeleted", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
            End If
        Else
            MessageBox.Show(sMapName & " was Not deleted.", "Delete Map", "Mapnotdeleted", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.OK)
        End If
    End Sub
    Private Sub ButtonDeleteMap_Click(sender As Object, e As EventArgs) Handles ButtonDeleteMap.Click
        '// uncomment this section if you want to delete history item also
        '//
        'Dim SelectedMap As String = DropDownMapNames.SelectedItem.Text.Trim

        'If SelectedMap.Contains(" *(") Then
        '    Dim sMapName As String = Piece(SelectedMap, "*(", 1).Trim
        '    Dim sSaved As String = Piece(SelectedMap, "*(", 2).Trim.Replace(")", "")
        '    DeleteHistoryItem(sMapName, sSaved)
        'End If

        DeleteMap(txtMapName.Text)

    End Sub

    Private Function MakeKMLforGoogleMap(ByVal gglmap As Boolean, Optional ByVal latlon As String = "") As String
        Dim ret As String = String.Empty
        'save definition for georestrictions, altit, linewith
        Dim updSQL As String = "UPDATE OURReportFormat SET Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        ret = ExequteSQLquery(updSQL)

        Dim maptype As String = DropDownMapType.Text.Trim
        If gglmap AndAlso maptype = "Tours" Then
            ret = "MapType is not supported in Google Maps"
            Return ret
        End If
        Dim dm As DataTable = GetReportMapFields(repid, txtMapName.Text)
        If dm Is Nothing Then
            ret = "no Map fields found"
        End If
        Dim i As Integer
        Dim lon As String = String.Empty
        Dim lat As String = String.Empty
        Dim nm As String = String.Empty
        Dim des As String = String.Empty
        Dim lonend As String = String.Empty
        Dim latend As String = String.Empty
        Dim starttimecol As String = String.Empty
        Dim endtimecol As String = String.Empty
        For i = 0 To dm.Rows.Count - 1
            If dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkName" AndAlso dm.Rows(i)("ord") = 0 Then
                nm = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkDescription" AndAlso dm.Rows(i)("ord") = 0 Then
                des = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitude" AndAlso dm.Rows(i)("ord") = 0 Then
                lon = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitude" AndAlso dm.Rows(i)("ord") = 0 Then
                lat = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLongitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
                lonend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkLatitudeEnd" AndAlso dm.Rows(i)("ord") = 1 Then
                latend = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkStartTime" Then
                starttimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            ElseIf dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkEndTime" Then
                endtimecol = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
            End If
        Next
        Dim appldirKMLFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString

        '-----------------------------------------------------------------------------------------------------------------------------------------
        'wwwroot\OUR\KMLS is not writable !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        'Dim fileuploadDir As String = ConfigurationManager.AppSettings("fileupload").ToString
        'applpath = fileuploadDir
        '-----------------------------------------------------------------------------------------------------------------------------------------

        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        Dim mapname As String = txtMapName.Text
        If mapname.Contains(" *(") Then 'from history
            mapname = mapname.Substring(0, mapname.IndexOf(" *(")).Trim
        End If

        'myfile = Session("REPORTID") & "_" & mapname.Replace(" ", "_").Replace("/", "_").Replace("\", "_").Substring(0, 20) & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".kml"

        myfile = Session("REPORTID") & "_" & mapname.Replace(" ", "_").Replace("/", "_").Replace("\", "_")
        If myfile.Length > 50 Then
            myfile = myfile.Substring(0, 50)
        End If
        myfile = myfile & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".kml"

        appldirKMLFiles = applpath & "KMLS\"
        Session("appldirKMLFiles") = appldirKMLFiles
        Dim expfile As String = appldirKMLFiles & myfile
        Session("kmlfile") = expfile
        Dim dv3 As DataView = Session("dv3")
        If dv3 Is Nothing Then
            Dim dri As DataTable = mRecords("SELECT * FROM OURReportInfo WHERE (ReportID='" & Session("REPORTID") & "')").Table
            Dim msql = dri.Rows(0)("SQLquerytext")
            ret = ""
            dv3 = mRecords(msql, ret, Session("UserConnString"), Session("UserConnProvider"))
            If dv3 Is Nothing Then
                LabelAlert.Text = "no data"
                LabelAlert.Visible = True
                Return "no data"
            End If
            Dim dtf As New DataTable
            dtf = dv3.Table
            dtf = MakeDTColumnsNamesCLScompliant(dtf, Session("UserConnProvider"), ret)
            dv3 = dtf.DefaultView
        End If

        Dim sl As Boolean = False
        If chkShowLinks.Checked Then
            sl = chkShowLinks.Checked
            GenerateMap.lgn = GetReportIdentifier(Session("REPORTID"))
        End If
        Dim inalt As Integer = 4000
        Dim inwd As Integer = 4
        If txtInitialAltit.Text.Trim <> "" AndAlso IsNumeric(txtInitialAltit.Text.Trim) Then
            inalt = CInt(txtInitialAltit.Text.Trim)
        End If
        If txtLineWidth.Text.Trim <> "" AndAlso IsNumeric(txtLineWidth.Text.Trim) Then
            inwd = CInt(txtLineWidth.Text.Trim)
        End If
        Dim georest As String = txtGeoRestrictions.Text.Trim
        'map types
        If (maptype = "Pins" Or chkShowPins.Checked = True) AndAlso nm.Trim = "" Then
            ret = "ERROR!! Field for Pacemark Name is not selected"
            Return ret
        End If
        If maptype = "Pins" Then
            ret = GenerateMapReportPlacemarks(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", georest, nm, des, lonend, latend, sl, chkShowCircles.Checked, inalt, inwd, gglmap, latlon)
        ElseIf maptype = "Circles" Then
            chkShowCircles.Checked = True
            hdnShowCircles.Value = "true"
            Session("showcircles") = "true"
            ret = GenerateMapReportCircles(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", georest, nm, des, lonend, latend, chkShowPins.Checked, sl, inalt, inwd, gglmap, latlon)

        ElseIf maptype = "Paths" Then
            ret = GenerateMapReportExtrudedPath(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", georest, nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd, gglmap, latlon)

        ElseIf maptype = "Polygons" Then
            ret = GenerateMapReportExtrudedPolygons(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, "", "", lon, lat, "", "", "", georest, nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd, gglmap)

        ElseIf maptype = "Tours" Then
            ret = GenerateMapReportTours(Session("REPORTID"), mapname, expfile, DropDownMapType.Text, dv3.Table, dm, starttimecol, endtimecol, lon, lat, "", "", "", georest, nm, des, lonend, latend, chkShowPins.Checked, sl, chkShowCircles.Checked, inalt, inwd, gglmap, latlon)
        End If

        If ret.StartsWith("ERROR!!") Then
            Return ret
            'LabelAlert.Text = ret
            'LabelAlert.Visible = True
            'LabelAlert.ForeColor = Color.Red
        Else
            If Not File.Exists(expfile) Then
                Return "ERROR!! " & ret
            End If
            If gglmap Then 'check file size
                Dim st As String = File.ReadAllText(expfile)
                If st.Length > 9000000 Then
                    WriteToAccessLog(Session("REPORTID"), "FILE TOO BIG FOR GOOGLE MAP!! " & ret, 112)
                    ret = "FILE TOO BIG FOR GOOGLE MAP!! " & ret
                End If
            End If
        End If

        Return ret
    End Function
    Private Sub MessageBox_MessageResulted(sender As Object, e As Controls_Msgbox.MsgBoxEventArgs) Handles MessageBox.MessageResulted
        If e.Tag = "Mapdeleted" Then
            Session("txtMapName") = ""
            Session("comments") = ""
            Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&showlinks=false&showcircles=false&showpins=false&initalt=4000&linewidth=4")
        ElseIf e.Tag = "KMLsavedToHistory" Then
            Session("Comments") = txtComments.Text.Trim
            Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&showlinks=false&showcircles=false&showpins=false&initalt=4000&linewidth=4")
        ElseIf e.Tag = "KMLnotgenerated" Then
            Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&showlinks=false&showcircles=false&showpins=false&initalt=4000&linewidth=4")
        ElseIf e.Tag = "NoMapName" Then
            txtMapName.Focus()
        End If
    End Sub

    Private Sub btnOpenGoogleChartMapReport_Click(sender As Object, e As EventArgs) Handles btnOpenGoogleChartMapReport.Click
        'save definition for georestrictions, altit, linewith
        'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        'Dim ret As String = ExequteSQLquery(updSQL)
        Dim ret As String = String.Empty
        ret = SaveMapDefinition(ret)
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show(ret & ". KML file has not been generated.", "KML file  has not been generated", "KMLnotgenerated", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
            Exit Sub
        End If
        Response.Redirect("ChartGoogleOne.aspx?map=yes&Report=" & Session("REPORTID") & "&mapname=" & txtMapName.Text)
    End Sub

    Private Sub btnPlacemarkGeolocation_Click(sender As Object, e As EventArgs) Handles btnPlacemarkGeolocation.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField(DropDownMapFields.Text, "PlacemarkLongitude", 0)
        End If
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 0
            ret = AddField("POINT", "PlacemarkLatitude", 0)
        End If

        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)

    End Sub

    Private Sub btnUpdateGeoRestrictions_Click(sender As Object, e As EventArgs) Handles btnUpdateGeoRestrictions.Click
        'GeoRestrictions, altitude saved in row for PlacemarkName
        Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        Dim ret As String = ExequteSQLquery(updSQL)
        Response.Redirect("ChartGoogleOne.aspx?map=yes&Report=" & Session("REPORTID") & "&mapname=" & txtMapName.Text & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)
    End Sub

    Private Sub btnAddPointGeolocation_Click(sender As Object, e As EventArgs) Handles btnAddPointGeolocation.Click
        Dim ret As String = String.Empty
        If DropDownMapFields2.Text.Trim <> "" Then
            Session("order") = 2
            ret = AddField(DropDownMapFields2.Text, "PlacemarkLongitude", 2)
        End If
        If DropDownMapFields2.Text.Trim <> "" Then
            Session("order") = 2
            ret = AddField("POINT", "PlacemarkLatitude", 2)
        End If

        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)

    End Sub

    Private Sub btnPlacemarkGeolocation2_Click(sender As Object, e As EventArgs) Handles btnPlacemarkGeolocation2.Click
        Dim ret As String = String.Empty
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 1
            ret = AddField(DropDownMapFields.Text, "PlacemarkLongitudeEnd", 0)
        End If
        If DropDownMapFields.Text.Trim <> "" Then
            Session("order") = 1
            ret = AddField("POINT", "PlacemarkLatitudeEnd", 0)
        End If
        Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID") & "&ret=" & ret & "&showlinks=" & hdnShowLinks.Value & "&showcircles=" & hdnShowCircles.Value & "&showpins=" & hdnShowPins.Value & "&initalt=" & hdnInitAltitude.Value & "&linewidth=" & hdnLineWidth.Value & "&latlon=" & chkLatLonOrLonLat.Checked.ToString)

    End Sub

    Private Sub chkLatLonOrLonLat_CheckedChanged(sender As Object, e As EventArgs) Handles chkLatLonOrLonLat.CheckedChanged
        If chkLatLonOrLonLat.Checked = True Then
            Session("latlon") = "True"
        Else
            Session("latlon") = "False"
        End If
        'Dim updSQL As String = "UPDATE OURReportFormat SET Prop7='" & Session("latlon").ToString & "',Prop4='" & txtGeoRestrictions.Text.Trim & "',Prop5='" & txtInitialAltit.Text.Trim & "',Prop6='" & txtLineWidth.Text.Trim & "' WHERE (ReportID='" & Session("REPORTID") & "' AND Prop='MAPS' AND Prop2='PlacemarkName' AND Prop3='" & txtMapName.Text & "')"
        'Dim ret As String = ExequteSQLquery(updSQL)
    End Sub
End Class




