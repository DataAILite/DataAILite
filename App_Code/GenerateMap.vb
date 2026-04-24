Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml
Imports System.Drawing
Imports System.Math
Imports System.Net
Public Module GenerateMap
    Public kmlPath As String
    Public lgn As String
    Public mapkey As String
    'Private psw As String
    Public Function GetDashboardIdentifier(ByVal dash As String, ByVal logon As String) As String
        Dim ret As String = String.Empty
        Dim re As String = String.Empty
        Dim ind As String = String.Empty
        Dim i As Integer
        Dim d As Integer = 0
        Try
            Dim dri As DataTable = Nothing
            Dim sqls = "SELECT * FROM ourdashboards WHERE (UserID='" & logon & "' AND Dashboard='" & dash & "')"
            dri = mRecords(sqls).Table  'from OUR db
            If dri IsNot Nothing And dri.Rows.Count > 0 Then
                For i = 0 To dri.Rows.Count - 1
                    ret = dri.Rows(i)("Prop6").ToString
                    If ret.Trim <> "" Then
                        Exit For
                    End If
                Next
                If ret.Trim = "" Then
                    'assign random identifier
                    Dim r As Random = New Random
                    d = r.Next(1000) 'Max range
                    ret = "d" & Now().ToString.Replace(" ", "").Replace(":", "").Replace("/", "").Replace("M", "") & d.ToString
                End If
                re = ExequteSQLquery("UPDATE ourdashboards SET Prop6='" & ret & "' WHERE UserID='" & logon & "' AND Dashboard='" & dash & "'")
                If re <> "Query executed fine." Then
                    ret = ""
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            Return ""
        End Try
        Return ret
    End Function
    Public Function GetReportIdentifier(ByVal rep As String) As String
        Dim ret As String = String.Empty
        Dim re As String = String.Empty
        Dim ind As String = String.Empty
        Dim m As Integer = 0
        Try
            Dim dri As DataTable = Nothing
            Dim sqls = "SELECT * FROM OURReportInfo WHERE (ReportID='" & rep & "')"
            dri = mRecords(sqls).Table  'from OUR db
            If dri IsNot Nothing And dri.Rows.Count = 1 Then
                ind = dri.Rows(0)("Indx").ToString
                ret = dri.Rows(0)("Param1id").ToString
                If ret.Trim <> "" Then
                    Return ret
                Else
                    'assign random identifier
                    Dim r As Random = New Random
                    m = r.Next(1000) 'Max range
                    ret = "m" & Now().ToString.Replace(" ", "").Replace(":", "").Replace("/", "").Replace("M", "") & m.ToString
                    re = ExequteSQLquery("UPDATE OURReportInfo SET Param1id='" & ret & "' WHERE (ReportID='" & rep & "') AND (Indx=" & ind & ")")
                    If re <> "Query executed fine." Then
                        ret = ""
                    End If
                End If
            End If
        Catch ex As Exception
            ret = ex.Message
            Return ""
        End Try
        Return ret
    End Function
    Public Function GetScheduledReportIdentifier(ByVal rep As String) As String
        Dim ret As String = String.Empty
        Dim m As Integer = 0
        'assign random identifier
        Dim r As Random = New Random
        m = r.Next(1000) 'Max range
        ret = "s" & Now().ToString.Replace(" ", "").Replace(":", "").Replace("/", "").Replace("M", "") & m.ToString
        Return ret
    End Function
    Public Function GetReportInfoWithParam(ByVal lgn As String, ByVal rep As String) As DataTable
        'Param1id has random identifier
        Dim dri As DataTable = Nothing
        Dim sqls = "SELECT * FROM OURReportInfo WHERE (ReportID='" & rep & "') AND (Param1id='" & lgn & "')"
        dri = mRecords(sqls).Table  'from OUR db
        Return dri
    End Function
    Public Function GetMaxMin(ByVal dt As DataTable, ByVal densfld As String, ByRef dtmin As Integer, ByRef dtmax As Integer) As String
        Dim ret As String = String.Empty
        If densfld.Trim = "" Then
            dtmin = 0
            dtmax = 0
            Exit Function
        End If
        Dim i As Integer = 0
        dtmin = dt.Rows(0)(densfld)
        dtmax = dt.Rows(0)(densfld)
        Try
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i)(densfld) < dtmin Then
                    dtmin = dt.Rows(i)(densfld)
                End If
                If dt.Rows(i)(densfld) > dtmax Then
                    dtmax = dt.Rows(i)(densfld)
                End If
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return ret
    End Function
    Public Function GetColorDensity(ByVal dtval As Integer, ByVal dtmin As Integer, ByVal dtmax As Integer) As Integer
        Dim ret As String = String.Empty
        If dtmax = 0 AndAlso dtmin = 0 Then
            Return 100
        End If
        Dim denspercent As Integer = 100
        Try
            denspercent = Round(100 * (dtval - dtmin) / (dtmax - dtmin))
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return denspercent
    End Function
    Public Function GetColor(ByVal colr As String, ByVal denspercent As Integer) As String
        Dim ret As String = String.Empty
        If denspercent > 99 Then
            colr = "7f" & colr.Substring(5, 2) & colr.Substring(3, 2) & colr.Substring(1, 2)
            Return colr
        End If
        Dim colri As String = colr
        Dim rgbColor As RGB = HexToRGB(colri)
        Dim density As Single = 1.0F - (denspercent / 100)
        If rgbColor IsNot Nothing Then
            Dim hslColor As HSL = RGBToHSL(rgbColor)
            Dim l As Single = hslColor.L
            Dim range As Single = 0.75 - l

            If range > 0.0 Then
                l = l + (density * range)
                Dim hsl As New HSL

                hsl.H = hslColor.H
                hsl.S = hslColor.S
                hsl.L = l
                Dim rgb As RGB = HSLToRGB(hsl)
                colri = RGBToHex(rgb)
                colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                Return colri
            Else
                colr = "7f" & colr.Substring(5, 2) & colr.Substring(3, 2) & colr.Substring(1, 2)
                Return colr
            End If
        Else
            ret = "Color was not converted to RGB."
            Return ret
        End If

        Try

        Catch ex As Exception
            ret = ex.Message
            Return ret
        End Try
        Return colri
    End Function
    Public Function GetReportMapFields(ByVal rep As String, ByVal MapName As String) As DataTable
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Try
            Dim selectSQL As String = "SELECT Val AS MapField,Prop1 AS Friendly,Prop2 AS ForMap,Prop3 AS MapName,[Order] as ord,Prop4 as descrtext,Prop5,Prop6,Prop7,Indx FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='MAPS' AND Prop3='" & MapName & "') ORDER BY ord,Indx"
            dt = DataModule.mRecords(selectSQL, ret).Table
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dt
    End Function
    Public Function MapExists(rep As String, MapName As String) As Boolean

        If MapName = String.Empty Then Return False
        Dim msql As String = "SELECT * FROM OURReportFormat WHERE ReportID='" & rep & "' AND Prop3='" & MapName & "'"
        Return HasRecords(msql)
    End Function
    Public Function GetReportKeyFields(ByVal rep As String, ByVal MapName As String) As DataTable
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Try
            Dim selectSQL As String = "SELECT Val AS KeyField,Prop1 AS Friendly,Prop2 AS ForMap,Prop3 AS MapName,[Order] as ord,Prop4 as descrtext, Indx FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='KEYS' AND Prop3='" & MapName & "') ORDER BY Indx"
            dt = DataModule.mRecords(selectSQL, ret).Table
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dt
    End Function
    Public Function GetReportExtrudedFields(ByVal rep As String, ByVal MapName As String) As DataTable
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Try
            Dim selectSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='EXTRUDE' AND Prop3='" & MapName & "') ORDER BY Indx"
            dt = DataModule.mRecords(selectSQL, ret).Table
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dt
    End Function
    Public Function GetReportColorField(ByVal rep As String, ByVal MapName As String) As DataTable
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Try
            Dim selectSQL As String = "SELECT * FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='COLOR' AND Prop3='" & MapName & "')"
            dt = DataModule.mRecords(selectSQL, ret).Table
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dt
    End Function
    Public Function GetMapShowFields(ByVal rep As String, ByVal MapName As String) As DataTable
        Dim ret As String = String.Empty
        Dim dt As DataTable = Nothing
        Try
            Dim selectSQL As String = "SELECT Val,Prop1,Prop2 AS ForMap,Prop3 AS MapName,Indx FROM OURReportFormat WHERE (ReportID='" & rep & "' AND Prop='SHOWS' AND Prop3='" & MapName & "') ORDER BY Indx"
            dt = DataModule.mRecords(selectSQL, ret).Table
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return dt
    End Function
    Public Function DescriptionText(ByRef dt As DataTable, ByVal drf As DataTable, ByVal i As Integer, Optional ByVal showlink As Boolean = False, Optional ByVal gglmap As Boolean = False) As String
        Dim ret As String = String.Empty
        Dim descr As String = String.Empty
        Dim fltr As String = String.Empty
        Try
            descr = "<![CDATA["
            descr = descr & "<br>"
            For j = 0 To drf.Rows.Count - 1
                If drf.Rows(j)("ForMap").ToString = "PlacemarkDescription" Then
                    If drf.Rows(j)("descrtext").ToString <> "" Then
                        descr = descr & " <b> " & drf.Rows(j)("descrtext").ToString & " </b>"
                    Else
                        'convert number from exponential 
                        If IsNumeric(dt.Rows(i)(drf.Rows(j)("MapField").ToString)) Then
                            descr = descr & ExponentToNumber(dt.Rows(i)(drf.Rows(j)("MapField").ToString.ToUpper)).ToString & "<br>"
                        Else
                            descr = descr & dt.Rows(i)(drf.Rows(j)("MapField").ToString).ToString & "<br>"
                        End If

                    End If
                End If
                If showlink AndAlso drf.Rows(j)("ForMap").ToString = "PlacemarkName" Then
                    If fltr.Trim = "" Then
                        fltr = drf.Rows(j)("MapField").ToString & "~"
                    Else
                        fltr = fltr & "^" & drf.Rows(j)("MapField").ToString & "~"

                    End If
                    If ColumnTypeIsNumeric(dt.Columns(drf.Rows(j)("MapField").ToString)) Then
                        fltr = fltr & dt.Rows(i)(drf.Rows(j)("MapField").ToString).ToString
                    Else
                        fltr = fltr & "*" & dt.Rows(i)(drf.Rows(j)("MapField").ToString).ToString & "*"
                    End If
                End If
            Next
            If showlink Then
                descr = descr & "<br>" & "<br>"
                ret = ConfigurationManager.AppSettings("weboureports").ToString & "default.aspx?srd=3&map=yes&rep=" & repid & "&lgn=" & lgn & "&flt=" & fltr
                descr = descr & "Reports, graphics, and statistics: <a href=""" & ret & """ > here</a>"
            End If
            If gglmap Then
                descr = descr & " "
            Else
                descr = descr & "<]]>"
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try
        Return descr
    End Function
    Private Function GetPass(ByVal lgn As String, ByVal rep As String) As String
        Dim ret As String = String.Empty
        Try
            Dim msql As String = "SELECT localpass FROM OURPermits INNER JOIN OURPermissions ON (OURPermissions.NetId=OURPermits.NetId AND OURPermissions.Application=OURPermits.Application)  WHERE (OURPermissions.NetId='" & lgn & "' AND OURPermissions.Application='InteractiveReporting' AND OURPermissions.Param1='" & rep & "')"
            Dim mrec As DataView = mRecords(msql)
            If mrec Is Nothing OrElse mrec.Table Is Nothing OrElse mrec.Table.Rows.Count = 0 Then
                ret = ""
            Else
                ret = mrec.Table.Rows(0)("localpass").ToString
            End If
            Return ret
        Catch ex As Exception
            ret = ex.Message
            Return ""
        End Try
    End Function
    Public Function CoordinatesInRow(ByVal dt As DataTable, ByVal drf As DataTable, ByVal i As Integer, Optional ByVal altit As Integer = 4000, Optional ByVal gglmap As Boolean = False, Optional ByVal georestrictions As String = "", Optional ByVal latlon As String = "") As String
        'dt - data, i - row index in dt, drf - fields with coordinates in dt row, drk - key fields to get other rows
        Dim ret As String = String.Empty
        Dim m As Integer = 0
        Dim n As Integer = 0
        Dim j As Integer = 0
        Dim coords As String = String.Empty
        Dim coor As String = String.Empty
        Dim coordslon(0) As String
        Dim coordslat(0) As String
        Dim p1 As String = String.Empty
        Dim p2 As String = String.Empty
        Try
            'next points
            For j = 0 To drf.Rows.Count - 1
                If drf.Rows(j)("ForMap").ToString = "PlacemarkLongitude" AndAlso drf.Rows(j)("ord") > 0 Then
                    m = drf.Rows(j)("ord")
                    ReDim Preserve coordslon(m)
                    'coordslon(m) = coords & dt.Rows(i)(drf.Rows(j)("MapField").ToString).ToString & ","
                    coordslon(m) = drf.Rows(j)("MapField").ToString.Trim
                End If
                If drf.Rows(j)("ForMap").ToString = "PlacemarkLatitude" AndAlso drf.Rows(j)("ord") > 0 Then
                    ReDim Preserve coordslat(m)
                    m = drf.Rows(j)("ord")
                    'coordslat(m) = dt.Rows(i)(drf.Rows(j)("MapField").ToString).ToString & "," & altit.ToString & " "
                    coordslat(m) = drf.Rows(j)("MapField").ToString.Trim
                End If
            Next
            For j = 1 To m
                If coordslon(j) Is Nothing OrElse coordslat(j) Is Nothing OrElse coordslon(j).ToString.Trim = "" OrElse coordslat(j).ToString.Trim = "" Then
                    Continue For
                End If
                'If coordslon(j) IsNot Nothing AndAlso coordslat(j) IsNot Nothing AndAlso coordslon(j).ToString.Trim <> "" AndAlso coordslat(j).ToString.Trim <> "" Then
                'coords = coords & coordslon(m) & coordslat(m)
                If coordslon(j) = coordslat(j) Then  'address
                    coor = CoordinatesGeocoding(dt.Rows(i)(coordslon(j)).ToString, 0, gglmap, georestrictions)
                    If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                        coords = coords & coor & " "
                    End If
                ElseIf coordslat(j).ToString.ToUpper.Trim = "POINT" Then
                    p1 = Piece(dt.Rows(i)(coordslon(j)).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                    p2 = Piece(dt.Rows(i)(coordslon(j)).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                    If latlon = "True" Then
                        coords = coords & p1 & "," & p2 & "," & altit.ToString & " "
                    Else
                        coords = coords & p2 & "," & p1 & "," & altit.ToString & " "
                    End If
                Else
                    coords = coords & dt.Rows(i)(coordslon(j)).ToString & "," & dt.Rows(i)(coordslat(j)).ToString & "," & altit.ToString & " "
                End If
                'End If
            Next
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return coords
    End Function
    Public Function CoordinatesInColumns(ByVal dt As DataTable, ByVal longitudecol As String, ByVal latitudecol As String, ByVal drk As DataTable, ByVal i As Integer, Optional ByVal altit As Integer = 4000, Optional ByRef grp As String = "", Optional ByVal gglmap As Boolean = False, Optional ByVal georestrictions As String = "", Optional ByVal latlon As String = "") As String
        'dt - data, i - row index in dt, drf - fields with coordinates in dt row, drk - key fields to get other rows
        Dim ret As String = String.Empty
        Dim j As Integer = 0
        Dim lon As String = dt.Rows(i)(longitudecol)
        Dim lat As String = dt.Rows(i)(latitudecol)
        Dim coords As String = String.Empty
        Dim coor As String = String.Empty
        Dim fltr As String = String.Empty
        Dim p1, p2 As String

        Try
            'other points
            If grp = "" Then
                For j = 0 To drk.Rows.Count - 1
                    If drk.Rows(j)("ForMap").ToString = "KeyField" Then
                        If ColumnTypeIsNumeric(dt.Columns(drk.Rows(j)("KeyField").ToString)) Then
                            fltr = fltr & drk.Rows(j)("KeyField").ToString & "=" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString
                        Else
                            fltr = fltr & drk.Rows(j)("KeyField").ToString & "='" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString & "'"
                        End If
                        If j < drk.Rows.Count - 1 Then
                            fltr = fltr & " AND "
                        End If
                    End If
                Next
                grp = fltr
            Else
                fltr = grp
            End If
            dt.DefaultView.RowFilter = fltr
            Dim dtf As DataTable = dt.DefaultView.ToTable
            If latitudecol = "POINT" Then
                For j = 0 To dtf.Rows.Count - 1
                    If IsDBNull(dtf.Rows(j)(longitudecol)) Then Continue For
                    If dtf.Rows(j)(longitudecol).ToString = "0" Then Continue For
                    If dtf.Rows(j)(longitudecol).ToString = lon Then Continue For

                    p1 = Piece(dtf.Rows(j)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                    p2 = Piece(dtf.Rows(j)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                    If latlon = "True" Then
                        coords = coords & p1 & "," & p2 & "," & altit.ToString & " "
                    Else
                        coords = coords & p2 & "," & p1 & "," & altit.ToString & " "
                    End If


                Next
            ElseIf longitudecol = latitudecol Then  'address
                For j = 0 To dtf.Rows.Count - 1
                    If IsDBNull(dtf.Rows(j)(longitudecol)) OrElse IsDBNull(dtf.Rows(j)(latitudecol)) Then Continue For
                    If dtf.Rows(j)(longitudecol).ToString = "0" AndAlso dtf.Rows(j)(latitudecol).ToString = "0" Then Continue For
                    If dtf.Rows(j)(longitudecol).ToString = lon AndAlso dtf.Rows(j)(latitudecol).ToString = lat Then Continue For
                    coor = CoordinatesGeocoding(dtf.Rows(j)(longitudecol).ToString, 0, gglmap, georestrictions)
                    If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                        coords = coords & coor & " "
                    End If
                Next

            Else
                For j = 0 To dtf.Rows.Count - 1
                    If IsDBNull(dtf.Rows(j)(longitudecol)) OrElse IsDBNull(dtf.Rows(j)(latitudecol)) Then Continue For
                    If dtf.Rows(j)(longitudecol).ToString = "0" AndAlso dtf.Rows(j)(latitudecol).ToString = "0" Then Continue For
                    If dtf.Rows(j)(longitudecol).ToString = lon AndAlso dtf.Rows(j)(latitudecol).ToString = lat Then Continue For
                    coords = coords & dtf.Rows(j)(longitudecol).ToString & "," & dtf.Rows(j)(latitudecol).ToString & "," & altit.ToString & " "
                Next
            End If
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return coords
    End Function
    Public Function CoordinatesGeocoding(ByVal addrs As String, Optional ByVal altit As Integer = 4000, Optional ByVal gglmap As Boolean = False, Optional ByVal georestrictions As String = "") As String
        Dim coords As String = String.Empty
        Try
            Dim requestUri As String = String.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(addrs), mapkey)
            If georestrictions.Trim <> "" Then
                requestUri = requestUri & "&components=" & Uri.EscapeDataString(georestrictions.Trim)
            End If
            Dim request As WebRequest = WebRequest.Create(requestUri)
            Dim response As WebResponse = request.GetResponse()
            Dim xdoc As XDocument = XDocument.Load(response.GetResponseStream())
            Dim Result As XElement = xdoc.Element("GeocodeResponse").Element("result")
            If Result Is Nothing Then
                Return coords
            End If
            Dim locationElement As XElement = Result.Element("geometry").Element("location")
            Dim lat As XElement = locationElement.Element("lat")
            Dim lng As XElement = locationElement.Element("lng")

            coords = lng.Value.ToString & "," & lat.Value.ToString & "," & altit.ToString & " "
        Catch ex As Exception
            coords = "ERROR!! " & ex.Message
        End Try
        Return coords
    End Function
    Public Function CoordinatesLatLngGeocoding(ByVal addrs As String, Optional ByVal georestrictions As String = "") As String
        Dim coords As String = String.Empty
        Try
            Dim requestUri As String = String.Format("https://maps.googleapis.com/maps/api/geocode/xml?key={1}&address={0}&sensor=false", Uri.EscapeDataString(addrs), mapkey)
            If georestrictions.Trim <> "" Then
                requestUri = requestUri & "&components=" & Uri.EscapeDataString(georestrictions.Trim)
            End If
            Dim request As WebRequest = WebRequest.Create(requestUri)
            Dim response As WebResponse = request.GetResponse()
            Dim xdoc As XDocument = XDocument.Load(response.GetResponseStream())
            Dim Result As XElement = xdoc.Element("GeocodeResponse").Element("result")
            If Result Is Nothing Then
                Return coords
            End If
            Dim locationElement As XElement = Result.Element("geometry").Element("location")
            Dim lat As XElement = locationElement.Element("lat")
            Dim lng As XElement = locationElement.Element("lng")

            coords = lat.Value.ToString & "," & lng.Value.ToString
        Catch ex As Exception
            coords = "ERROR!! " & ex.Message
        End Try
        Return coords
    End Function
    Public Function CoordinatesCircle(ByVal r As Decimal, ByVal pntlng As String, ByVal pntlat As String, Optional ByVal altit As Integer = 4000, Optional ByRef gglmap As Boolean = False) As String
        'r - radius; pntlng, pntlata - center of circle, altit - altitude
        Dim ret As String = String.Empty
        Dim n As Integer = 90
        Dim s As Decimal = 2 * 3.14 / n
        Dim m As Decimal = r
        Dim coords As String = String.Empty
        Dim fltr As String = String.Empty
        Try
            'polygon points
            For j = 0 To n - 1
                coords = coords & (pntlng + r * Cos(j * s)).ToString & "," & (pntlat + r * Sin(j * s)).ToString & "," & altit.ToString & " "
                'If pntlat >= 0 Then
                '    m = 1.2 * (90 - pntlat) / 90
                'Else
                '    m = 1.2 * (90 + pntlat) / 90
                'End If
                ''If pntlat >= 0 Then
                ''    m = (1.3-0.2*Sin(j * s))*(90 - pntlat) / 90
                ''Else
                ''    m = (1.3+0.2*Sin(j * s))*(90 + pntlat) / 90
                ''End If
                'coords = coords & (pntlng + r * Cos(j * s)).ToString & "," & (pntlat + r * m * Sin(j * s)).ToString & "," & altit.ToString & " "
            Next
            coords = coords & (pntlng + r).ToString & "," & (pntlat).ToString & "," & altit.ToString & " "
        Catch ex As Exception
            ret = ex.Message
        End Try
        Return coords
    End Function
    Public Function GenerateMapReportPlacemarks(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showlinks As Boolean = False, Optional ByVal showcircles As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False, Optional ByVal latlon As String = "") As String
        Dim ret As String = String.Empty
        Dim descr As String = String.Empty
        Dim txtline As String = String.Empty
        Dim i As Integer
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If
        'if pins selected in dropdown for polygons or paths than they are drawn arond the end points 
        If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" Then
            longitudecol = longitudecolend
            latitudecol = latitudecolend
        End If
        'color and density
        Dim dens As String = String.Empty
        Dim colr As String = String.Empty
        Dim dtmin As Integer = 0
        Dim dtmax As Integer = 0
        Dim perc As Integer = 0
        Dim mult As Decimal = 1
        Dim dtval As String = String.Empty
        Dim colri As String = String.Empty
        Dim colrpin As String = "#ffff00"
        Dim coor As String = String.Empty
        Dim dcl As DataTable = GetReportColorField(repid, mapname)
        If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
            colr = "#ffff00"
        Else
            dens = dcl.Rows(0)("Val").ToString
            colr = dcl.Rows(0)("Prop4").ToString
            'calc max and min
            ret = GetMaxMin(dt, dens, dtmin, dtmax)
            If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
                mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
                If mult = 0 Then mult = 1
            End If
        End If

        'Extruded Fields
        Dim extrud As String = String.Empty
        Dim multiplyby As Decimal
        Dim extrudecolorfld As String = String.Empty
        Dim dce As DataTable = GetReportExtrudedFields(repid, mapname)
        If dce IsNot Nothing AndAlso dce.Rows.Count > 0 Then
            extrud = dce.Rows(0)("Val").ToString
            If dce.Rows(0)("Prop4").ToString.Trim = "" Then
                multiplyby = 1
            Else
                multiplyby = Convert.ToDecimal(dce.Rows(0)("Prop4").ToString)
            End If
            If dce.Rows(0)("Prop5").ToString.Trim = "" Then
                extrudecolorfld = ""
            Else
                extrudecolorfld = dce.Rows(0)("Prop5").ToString.Trim
            End If
        End If
        Dim altit As Integer = initaltit
        Dim descript As String = String.Empty

        descript = "Field for color density and circle radius: " & dens & " multiplied by " & mult.ToString & ", "
        descript = descript & "Field for extruded altitude: " & extrud & " multiplied by " & multiplyby.ToString & " and with initial altitude " & initaltit.ToString

        Try
            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
            doc.Load(New StringReader(xmlData))
            Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
            AddElement(docum, "name", mapname & " (" & rep & ")")
            AddElement(docum, "open", "1")
            AddElement(docum, "description", descript)
            Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
            AddElement(folder, "name", "Placemarks")
            Dim placemark As XmlElement
            Dim point As XmlElement
            Dim style As XmlElement
            Dim pointstyle As XmlElement
            Dim p1, p2 As String

            'pins
            For i = 0 To dt.Rows.Count - 1
                If latitudecol = "POINT" Then
                    If IsDBNull(dt.Rows(i)(longitudecol)) OrElse dt.Rows(i)(longitudecol).Replace("POINT", "").Replace(" ", ",").Replace(",,", ",") = "(0,0)" Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                        Continue For
                    End If
                Else
                    If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                        Continue For
                    End If
                End If

                placemark = AddElement(folder, "Placemark", Nothing)
                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                style = AddElement(placemark, "Style", Nothing)
                pointstyle = AddElement(style, "IconStyle", Nothing)
                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                AddElement(placemark, "description", descr)
                If dens.Trim <> "" Then
                    dtval = dt.Rows(i)(dens)
                    perc = GetColorDensity(dtval, dtmin, dtmax)
                Else
                    perc = 100
                End If
                'colri = GetColor(colrpin, perc) 'yellow color for placemarks 
                If extrudecolorfld = "" Then
                    colri = GetColor(colr, perc)
                Else  'no density in this case !!!!!!!!!!!!!
                    colri = dt.Rows(i)(extrudecolorfld)
                    If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                        colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                    ElseIf colri.Length = 6 Then
                        colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                    End If
                End If
                AddElement(pointstyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                If extrud.Trim <> "" Then
                    altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                Else
                    altit = initaltit '(perc * 10000 + initaltit)
                End If
                point = AddElement(placemark, "Point", Nothing)

                If latitudecol = "POINT" Then 'in longitudecol the format is POINT(lon lat)
                    'AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)

                    p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                    p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                    If latlon = "True" Then
                        AddElement(point, "coordinates", p1 & "," & p2 & "," & altit.ToString)

                    Else
                        AddElement(point, "coordinates", p2 & "," & p1 & "," & altit.ToString)

                    End If

                ElseIf longitudecol = latitudecol Then  'address
                    coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, altit, gglmap, rangecol)
                    If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                        AddElement(point, "coordinates", coor)
                    End If
                Else
                    AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString)
                End If

            Next

            Dim grps As String = "|"
            Dim grp As String = String.Empty
            'circles
            If showcircles Then  'Not gglmap AndAlso
                AddElement(folder, "name", "Polygons")
                'Dim placemark As XmlElement
                Dim polygon As XmlElement
                Dim linearring As XmlElement
                Dim outerb As XmlElement
                'Dim style As XmlElement
                'Dim point As XmlElement
                Dim linestyle As XmlElement
                Dim polystyle As XmlElement
                Dim coordinates As String = String.Empty
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse dt.Rows(i)(longitudecol).Replace("POINT", "").Replace(" ", ",").Replace(",,", ",") = "(0,0)" Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                            Continue For
                        End If
                    End If
                    'new circle? for circles if highest density field assign than color takes from it, not from extruded color field which is used for coloring the paths
                    grp = ""
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens).ToString
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    colri = GetColor(colr, perc)
                    If extrudecolorfld <> "" AndAlso dens.Trim = "" Then
                        'no density in this case !!!!!!!!!!!!!
                        colri = dt.Rows(i)(extrudecolorfld)
                        If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                            colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                        ElseIf colri.Length = 6 Then
                            colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                        End If
                    End If
                    If extrud.Trim <> "" Then
                        altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                    Else
                        altit = initaltit '(perc * 10000 + initaltit)
                    End If
                    If latitudecol = "POINT" Then
                        'grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit & "|"


                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & p1 & "," & p2 & "," & "," & altit & "|"

                        Else
                            grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & p2 & "," & p1 & "," & "," & altit & "|"

                        End If

                    Else

                        grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit & "|"
                    End If
                    If grps.Contains("|" & grp & "|") Then Continue For
                    grps = grps & grp & "|"

                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    AddElement(placemark, "styleUrl", "yellowLineGreenPoly")

                    style = AddElement(placemark, "Style", Nothing)
                    linestyle = AddElement(style, "LineStyle", Nothing)

                    AddElement(linestyle, "width", inwd.ToString)
                    polystyle = AddElement(style, "PolyStyle", Nothing)
                    AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    AddElement(linestyle, "color", colri)
                    polygon = AddElement(placemark, "Polygon", Nothing)
                    AddElement(polygon, "extrude", "1")
                    AddElement(polygon, "altitudeMode", "relativeToGround")
                    outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
                    linearring = AddElement(outerb, "LinearRing", Nothing)
                    'circle around point
                    If latitudecol = "POINT" Then
                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        'coordinates = CoordinatesCircle(perc * mult, ln, lt, altit, gglmap)
                        If latlon = "True" Then
                            coordinates = CoordinatesCircle(perc * mult, p1, p2, altit, gglmap)
                        Else
                            coordinates = CoordinatesCircle(perc * mult, p2, p1, altit, gglmap)
                        End If

                    ElseIf longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, altit, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = CoordinatesCircle(perc * mult, Piece(coor, ",", 1), Piece(coor, ",", 2), altit, gglmap)
                        End If
                    Else
                        coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit, gglmap)
                    End If
                    AddElement(linearring, "coordinates", coordinates)
                Next
            End If
            'correct <Document ...
            doc.FirstChild.InnerXml = doc.FirstChild.InnerXml.Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            doc.Save(expfile)

            'correct <Document ...
            'Dim sr() As String = File.ReadAllLines(expfile)
            'If sr(1).Trim.StartsWith("<Document ") Then
            '    sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            'End If
            'File.WriteAllLines(expfile, sr)
            ret = expfile
            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    'Public Function GenerateMapReportPath(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showpins As Boolean = True, Optional ByVal showlinks As Boolean = False, Optional ByVal showcircles As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False) As String
    '    'NOT IN USE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
    '    Dim ret As String = String.Empty
    '    Dim descr As String = String.Empty
    '    Dim txtline As String = String.Empty
    '    Dim i As Integer
    '    If dt Is Nothing Then
    '        Return ret
    '        Exit Function
    '    End If

    '    'color and density
    '    Dim dens As String = String.Empty
    '    Dim colr As String = String.Empty
    '    Dim dtmin As Integer = 0
    '    Dim dtmax As Integer = 0
    '    Dim perc As Integer = 0
    '    Dim mult As Decimal = 1
    '    Dim dtval As String = String.Empty
    '    Dim colri As String = String.Empty
    '    Dim dcl As DataTable = GetReportColorField(repid, mapname)
    '    If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
    '        colr = "#ffff00"
    '    Else
    '        dens = dcl.Rows(0)("Val").ToString
    '        colr = dcl.Rows(0)("Prop4").ToString
    '        ret = GetMaxMin(dt, dens, dtmin, dtmax)
    '        If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
    '            mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
    '            If mult = 0 Then mult = 1
    '        End If
    '    End If
    '    'Extruded Fields
    '    Dim extrud As String = String.Empty
    '    Dim multiplyby As Decimal
    '    Dim extrudecolorfld As String = String.Empty
    '    dcl = GetReportExtrudedFields(repid, mapname)
    '    If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
    '        extrud = dcl.Rows(0)("Val").ToString
    '        If dcl.Rows(0)("Prop4").ToString.Trim = "" Then
    '            multiplyby = 1
    '        Else
    '            multiplyby = Convert.ToDecimal(dcl.Rows(0)("Prop4").ToString)
    '        End If
    '        If dcl.Rows(0)("Prop5").ToString.Trim = "" Then
    '            extrudecolorfld = ""
    '        Else
    '            extrudecolorfld = dcl.Rows(0)("Prop5").ToString.Trim
    '        End If
    '    End If
    '    'WriteToAccessLog(rep, "extrudecolorfld " & extrudecolorfld.ToString, 111)

    '    Dim drk As DataTable = GetReportKeyFields(repid, mapname)
    '    Try
    '        'Start doc
    '        Dim doc As New XmlDocument
    '        Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
    '        doc.Load(New StringReader(xmlData))
    '        Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
    '        AddElement(docum, "name", mapname)
    '        AddElement(docum, "open", "1")
    '        AddElement(docum, "description", rep)
    '        Dim style As XmlElement = AddElement(docum, "Style", Nothing)
    '        Dim attr As XmlAttribute = style.Attributes.Append(doc.CreateAttribute("id"))
    '        attr.Value = "yellowLineGreenPoly"
    '        Dim linestyle As XmlElement
    '        Dim polystyle As XmlElement
    '        'AddElement(polystyle, "color", "7f00ff00")
    '        Dim pointstyle As XmlElement
    '        Dim altit As Integer = initaltit

    '        Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
    '        AddElement(folder, "name", "Placemarks")
    '        Dim placemark As XmlElement
    '        Dim linestring As XmlElement
    '        Dim point As XmlElement
    '        Dim w As String = inwd

    '        'paths
    '        For i = 0 To dt.Rows.Count - 1
    '            If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
    '                Continue For
    '            End If
    '            placemark = AddElement(folder, "Placemark", Nothing)
    '            AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '            descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '            AddElement(placemark, "description", descr)
    '            AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
    '            style = AddElement(placemark, "Style", Nothing)
    '            linestyle = AddElement(style, "LineStyle", Nothing)
    '            If dens.Trim <> "" Then
    '                dtval = dt.Rows(i)(dens)
    '                perc = GetColorDensity(dtval, dtmin, dtmax)
    '            Else
    '                perc = 100
    '            End If
    '            If extrudecolorfld = "" Then
    '                colri = GetColor(colr, perc)
    '            Else  'no density in this case !!!!!!!!!!!!!
    '                colri = dt.Rows(i)(extrudecolorfld)
    '                If colri.StartsWith("#") AndAlso colri.Length = 7 Then
    '                    colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
    '                ElseIf colri.Length = 6 Then
    '                    colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
    '                End If
    '            End If
    '            'w = (2 + dt.Rows(i)("TOR_WIDTH") / 100).ToString
    '            w = inwd.ToString
    '            AddElement(linestyle, "width", w)
    '            polystyle = AddElement(style, "PolyStyle", Nothing)
    '            AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '            AddElement(linestyle, "color", colri)
    '            linestring = AddElement(placemark, "LineString", Nothing)
    '            altit = initaltit
    '            AddElement(linestring, "extrude", "0")
    '            AddElement(linestring, "tessellate", "0")
    '            'AddElement(linestring, "altitudeMode", "absolute")
    '            AddElement(linestring, "altitudeMode", "relativeToGround")
    '            'first point
    '            Dim coordinates As String = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
    '            If drf.Rows.Count > 0 Then
    '                coordinates = coordinates & CoordinatesInRow(dt, drf, i, altit)
    '            End If
    '            If drk.Rows.Count > 0 Then
    '                coordinates = coordinates & CoordinatesInColumns(dt, longitudecol, latitudecol, drk, i, altit)
    '            End If
    '            If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" AndAlso Not IsDBNull(dt.Rows(i)(longitudecolend)) AndAlso Not IsDBNull(dt.Rows(i)(latitudecolend)) AndAlso Not IsDBNull(dt.Rows(i)(longitudecolend)) AndAlso IsNumeric(dt.Rows(i)(longitudecolend)) AndAlso (dt.Rows(i)(longitudecolend).ToString.Trim <> "0" Or dt.Rows(i)(latitudecolend).ToString.Trim <> "0") Then
    '                coordinates = coordinates & dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit.ToString & " "
    '            Else
    '                coordinates = coordinates & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
    '            End If
    '            AddElement(linestring, "coordinates", coordinates)
    '        Next
    '        If showpins Then
    '            'start points
    '            folder = AddElement(docum, "Folder", Nothing)
    '            AddElement(folder, "name", "Placemarks")
    '            For i = 0 To dt.Rows.Count - 1
    '                If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
    '                    Continue For
    '                End If
    '                If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
    '                    Continue For
    '                End If
    '                placemark = AddElement(folder, "Placemark", Nothing)
    '                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '                AddElement(placemark, "description", descr)
    '                point = AddElement(placemark, "Point", Nothing)
    '                AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",0")
    '            Next

    '            'end pointsAndAlso Not IsDBNull(dt.Rows(i)(longitudecol
    '            If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" Then
    '                folder = AddElement(docum, "Folder", Nothing)
    '                AddElement(folder, "name", "Placemarks")
    '                For i = 0 To dt.Rows.Count - 1
    '                    If IsDBNull(dt.Rows(i)(longitudecolend)) OrElse IsDBNull(dt.Rows(i)(latitudecolend)) Then
    '                        Continue For
    '                    End If
    '                    If i > 0 AndAlso dt.Rows(i)(longitudecolend) = dt.Rows(i - 1)(longitudecolend) AndAlso dt.Rows(i)(latitudecolend) = dt.Rows(i - 1)(latitudecolend) Then
    '                        Continue For
    '                    End If
    '                    placemark = AddElement(folder, "Placemark", Nothing)
    '                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '                    AddElement(placemark, "description", descr)
    '                    style = AddElement(placemark, "Style", Nothing)
    '                    pointstyle = AddElement(style, "IconStyle", Nothing)
    '                    If dens.Trim <> "" Then
    '                        dtval = dt.Rows(i)(dens)
    '                        perc = GetColorDensity(dtval, dtmin, dtmax)
    '                    Else
    '                        perc = 100
    '                    End If
    '                    colri = GetColor(colr, perc)
    '                    AddElement(pointstyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '                    point = AddElement(placemark, "Point", Nothing)
    '                    AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0")

    '                    'AddElement(pointstyle, "scale", "1.2")
    '                    'AddElement(pointstyle, "Icon", "<href>https://earth.google.com/earth/rpc/cc/icon?color=d32f2f&amp;id=2000&amp;scale=4</href>")
    '                    ''AddElement(pointstyle, "Icon", "~/Controls/Images/purple-circle.png")
    '                    ''AddElement(pointstyle, "hotSpot", "x=""32"" y=""1"" xunits=""pixels"" yunits=""pixels""")

    '                Next
    '            End If
    '        End If
    '        Dim grps As String = "|"
    '        Dim grp As String = String.Empty
    '        If showcircles Then
    '            altit = initaltit
    '            AddElement(folder, "name", "Polygons")
    '            Dim polygon As XmlElement
    '            Dim linearring As XmlElement
    '            Dim outerb As XmlElement
    '            For i = 0 To dt.Rows.Count - 1
    '                If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
    '                    Continue For
    '                End If
    '                If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
    '                    Continue For
    '                End If
    '                placemark = AddElement(folder, "Placemark", Nothing)
    '                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '                AddElement(placemark, "description", descr)
    '                AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
    '                style = AddElement(placemark, "Style", Nothing)
    '                linestyle = AddElement(style, "LineStyle", Nothing)
    '                AddElement(linestyle, "width", inwd.ToString)
    '                polystyle = AddElement(style, "PolyStyle", Nothing)
    '                'dtval = dt.Rows(i)(dens)
    '                'perc = GetColorDensity(dtval, dtmin, dtmax)
    '                If dens.Trim <> "" Then
    '                    dtval = dt.Rows(i)(dens)
    '                    perc = GetColorDensity(dtval, dtmin, dtmax)
    '                Else
    '                    perc = 100
    '                End If
    '                colri = GetColor(colr, perc)
    '                AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '                AddElement(linestyle, "color", colri)
    '                polygon = AddElement(placemark, "Polygon", Nothing)
    '                AddElement(polygon, "extrude", "1")
    '                AddElement(polygon, "altitudeMode", "relativeToGround")
    '                outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
    '                linearring = AddElement(outerb, "LinearRing", Nothing)
    '                'circle around point
    '                Dim coordinates As String = String.Empty
    '                coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit)
    '                AddElement(linearring, "coordinates", coordinates)
    '            Next
    '        End If

    '        doc.Save(expfile)

    '        'correct <Document ...
    '        Dim sr() As String = File.ReadAllLines(expfile)
    '        If sr(1).Trim.StartsWith("<Document ") Then
    '            sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
    '        End If
    '        'write in file
    '        File.WriteAllLines(expfile, sr)

    '        ret = expfile
    '        Return ret
    '    Catch ex As Exception
    '        ret = ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function GenerateMapReportExtrudedPath(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showpins As Boolean = True, Optional ByVal showlinks As Boolean = False, Optional ByVal showcircles As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False, Optional ByVal latlon As String = "") As String
        Dim ret As String = String.Empty
        Dim descr As String = String.Empty
        Dim txtline As String = String.Empty
        Dim m, i, j As Integer
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If
        Dim coor As String = String.Empty
        'color and density
        Dim dens As String = String.Empty
        Dim colr As String = String.Empty
        Dim dtmin As Integer = 0
        Dim dtmax As Integer = 0
        Dim perc As Integer = 0
        Dim mult As Decimal = 1
        Dim dtval As String = String.Empty
        Dim colri As String = String.Empty
        Dim p1, p2 As String
        Dim dcl As DataTable = GetReportColorField(repid, mapname)
        If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
            'color and field density does not assigned
            colr = "#ffff00"
        Else
            dens = dcl.Rows(0)("Val").ToString
            colr = dcl.Rows(0)("Prop4").ToString
            'calc max and min
            ret = GetMaxMin(dt, dens, dtmin, dtmax)
            If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
                mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
                If mult = 0 Then mult = 1
            End If
        End If

        'Extruded Fields
        Dim extrud As String = String.Empty
        Dim multiplyby As Decimal
        Dim extrudecolorfld As String = String.Empty
        dcl = GetReportExtrudedFields(repid, mapname)
        If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
            extrud = dcl.Rows(0)("Val").ToString
            If dcl.Rows(0)("Prop4").ToString.Trim = "" Then
                multiplyby = 1
            Else
                multiplyby = Convert.ToDecimal(dcl.Rows(0)("Prop4").ToString)
            End If
            If dcl.Rows(0)("Prop5").ToString.Trim = "" Then
                extrudecolorfld = ""
            Else
                extrudecolorfld = dcl.Rows(0)("Prop5").ToString.Trim
            End If
        End If
        Dim descript As String = String.Empty

        descript = "Field for color density and circle radius: " & dens & " multiplied by " & mult.ToString & ", "
        descript = descript & "Field for extruded altitude: " & extrud & " multiplied by " & multiplyby.ToString & " and with initial altitude " & initaltit.ToString

        'group
        Dim drk As DataTable = GetReportKeyFields(repid, mapname)
        Dim grps As String = "|"
        Dim grp As String = String.Empty
        Try
            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
            doc.Load(New StringReader(xmlData))
            Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
            AddElement(docum, "name", mapname & " (" & rep & ")")
            AddElement(docum, "open", "1")
            AddElement(docum, "description", descript)
            Dim style As XmlElement = AddElement(docum, "Style", Nothing)
            Dim attr As XmlAttribute = style.Attributes.Append(doc.CreateAttribute("id"))
            attr.Value = "yellowLineGreenPoly"
            Dim linestyle As XmlElement '= AddElement(style, "LineStyle", Nothing)
            Dim polystyle As XmlElement '= AddElement(style, "PolyStyle", Nothing)
            Dim pointstyle As XmlElement
            Dim altit As Integer = initaltit
            Dim placemark As XmlElement
            Dim linestring As XmlElement
            Dim point As XmlElement
            Dim w As String = inwd
            Dim firstpoint As String = String.Empty
            Dim coordinates As String = String.Empty
            Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
            AddElement(folder, "name", "Paths")

            'extruded paths
            For i = 0 To dt.Rows.Count - 1
                If latitudecol = "POINT" Then
                    If IsDBNull(dt.Rows(i)(longitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                        Continue For
                    End If
                Else
                    If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                        Continue For
                    End If
                End If
                'key filters
                grp = ""
                If drk.Rows.Count > 0 Then
                    For j = 0 To drk.Rows.Count - 1
                        If drk.Rows(j)("ForMap").ToString = "KeyField" Then
                            If ColumnTypeIsNumeric(dt.Columns(drk.Rows(j)("KeyField").ToString)) Then
                                grp = grp & drk.Rows(j)("KeyField").ToString & "=" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString
                            Else
                                grp = grp & drk.Rows(j)("KeyField").ToString & "='" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString & "'"
                            End If
                            If j < drk.Rows.Count - 1 Then
                                grp = grp & " AND "
                            End If
                        End If
                    Next
                    If grp.Trim = "" OrElse grps.Contains("|" & grp & "|") Then Continue For
                    grps = grps & grp & "|"
                End If

                placemark = AddElement(folder, "Placemark", Nothing)
                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                AddElement(placemark, "description", descr)
                AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
                style = AddElement(placemark, "Style", Nothing)
                linestyle = AddElement(style, "LineStyle", Nothing)
                AddElement(linestyle, "width", inwd.ToString)
                polystyle = AddElement(style, "PolyStyle", Nothing)
                If dens.Trim <> "" Then
                    dtval = dt.Rows(i)(dens)
                    perc = GetColorDensity(dtval, dtmin, dtmax)
                Else
                    perc = 100
                End If
                If extrudecolorfld = "" Then
                    colri = GetColor(colr, perc)
                ElseIf extrudecolorfld <> "" Then 'no density in this case, paths have color from extrudecolorfld, and only if it is empty, then from dens !!!!!!!!!!!!!
                    colri = dt.Rows(i)(extrudecolorfld)
                    If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                        colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                    ElseIf colri.Length = 6 Then
                        colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                    End If
                End If
                If extrud.Trim <> "" Then
                    altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                Else
                    altit = initaltit '(perc * 10000 + initaltit)
                End If

                AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                AddElement(linestyle, "color", colri)
                linestring = AddElement(placemark, "LineString", Nothing)
                AddElement(linestring, "extrude", "1")
                AddElement(linestring, "tessellate", "1")
                'AddElement(linestring, "altitudeMode", "absolute")
                AddElement(linestring, "altitudeMode", "relativeToGround")
                'first point
                If longitudecol.Trim <> "" AndAlso latitudecol.Trim <> "" Then
                    'coordinates = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
                    If latitudecol = "POINT" Then
                        'coordinates = dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString & " "
                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString

                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString

                        End If


                    ElseIf longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = coor
                        End If
                    Else
                        coordinates = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
                    End If
                    firstpoint = coordinates
                End If
                'in row
                If drf.Rows.Count > 0 Then
                    coordinates = coordinates & CoordinatesInRow(dt, drf, i, altit, gglmap, rangecol, latlon)
                End If
                'in column
                If drk.Rows.Count > 0 Then
                    coordinates = coordinates & CoordinatesInColumns(dt, longitudecol, latitudecol, drk, i, altit, grp, gglmap, rangecol, latlon)
                End If
                'end point
                If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" AndAlso Not IsDBNull(dt.Rows(i)(longitudecolend)) AndAlso Not IsDBNull(dt.Rows(i)(latitudecolend)) AndAlso IsNumeric(dt.Rows(i)(longitudecolend)) AndAlso (dt.Rows(i)(longitudecolend).ToString.Trim <> "0" Or dt.Rows(i)(latitudecolend).ToString.Trim <> "0") Then
                    'coordinates = coordinates & dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit.ToString & " "
                    If latitudecolend = "POINT" Then
                        'coordinates = dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString & " "
                        p1 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString

                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString

                        End If

                    ElseIf longitudecolend = latitudecolend Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecolend).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = coordinates & coor & " "
                        End If
                    Else
                        coordinates = coordinates & dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit.ToString & " "
                    End If
                ElseIf firstpoint = coordinates Then

                    coordinates = coordinates & firstpoint
                End If
                AddElement(linestring, "coordinates", coordinates)
            Next
            If showpins Then
                'start points
                folder = AddElement(docum, "Folder", Nothing)
                AddElement(folder, "name", "Placemarks")
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                            Continue For
                        End If
                    End If
                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    point = AddElement(placemark, "Point", Nothing)
                    If longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            AddElement(point, "coordinates", coor)
                        End If
                    ElseIf latitudecol = "POINT" Then 'in longitudecol the format is POINT(lon lat)
                        'AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)
                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString
                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString
                        End If
                        AddElement(point, "coordinates", coordinates)

                    Else
                        AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",0")
                    End If
                Next

                'end points
                If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" Then
                    folder = AddElement(docum, "Folder", Nothing)
                    AddElement(folder, "name", "Placemarks")
                    For i = 0 To dt.Rows.Count - 1
                        If Not IsDBNull(dt.Rows(i)(longitudecolend)) AndAlso Not IsDBNull(dt.Rows(i)(latitudecolend)) Then
                            If i > 0 AndAlso dt.Rows(i)(longitudecolend) = dt.Rows(i - 1)(longitudecolend) AndAlso dt.Rows(i)(latitudecolend) = dt.Rows(i - 1)(latitudecolend) Then
                                Continue For
                            End If
                            placemark = AddElement(folder, "Placemark", Nothing)
                            AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                            descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                            AddElement(placemark, "description", descr)

                            style = AddElement(placemark, "Style", Nothing)
                            pointstyle = AddElement(style, "IconStyle", Nothing)
                            If dens.Trim <> "" Then
                                dtval = dt.Rows(i)(dens)
                                perc = GetColorDensity(dtval, dtmin, dtmax)
                            Else
                                perc = 100
                            End If
                            colri = GetColor(colr, perc)
                            AddElement(pointstyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                            'point = AddElement(placemark, "Point", Nothing)
                            'AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0")
                            point = AddElement(placemark, "Point", Nothing)
                            If longitudecolend = latitudecolend Then  'address
                                coor = CoordinatesGeocoding(dt.Rows(i)(longitudecolend).ToString, 0, gglmap, rangecol)
                                If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                                    AddElement(point, "coordinates", coor)
                                End If
                            ElseIf latitudecolend = "POINT" Then 'in longitudecolend the format is POINT(lon lat)
                                'AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)

                                p1 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                                p2 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                                If latlon = "True" Then
                                    coordinates = p1 & "," & p2 & "," & altit.ToString
                                Else
                                    coordinates = p2 & "," & p1 & "," & altit.ToString
                                End If
                                AddElement(point, "coordinates", coordinates)

                            Else
                                AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0")
                            End If

                            'AddElement(pointstyle, "scale", "1.2")
                            'AddElement(pointstyle, "Icon", "<href>https://earth.google.com/earth/rpc/cc/icon?color=d32f2f&amp;id=2000&amp;scale=4</href>")
                            ''AddElement(pointstyle, "Icon", "~/Controls/Images/purple-circle.png")
                            ''AddElement(pointstyle, "hotSpot", "x=""32"" y=""1"" xunits=""pixels"" yunits=""pixels""")

                        End If
                    Next
                End If
            End If

            'if circles checked for polygons or paths than they are drawn arond the end points 
            If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" Then
                longitudecol = longitudecolend
                latitudecol = latitudecolend
            End If
            If showcircles Then
                altit = initaltit
                AddElement(folder, "name", "Circles")
                Dim polygon As XmlElement
                Dim linearring As XmlElement
                Dim outerb As XmlElement
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                            Continue For
                        End If
                    End If
                    'new circle? for circles if highest density field assign than color takes from it, not from extruded color field which is used for coloring the paths
                    grp = ""
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens).ToString
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    colri = GetColor(colr, perc)
                    If extrudecolorfld <> "" AndAlso dens.Trim = "" Then
                        'no density in this case !!!!!!!!!!!!!
                        colri = dt.Rows(i)(extrudecolorfld)
                        If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                            colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                        ElseIf colri.Length = 6 Then
                            colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                        End If
                    End If
                    If extrud.Trim <> "" Then
                        altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                    Else
                        altit = initaltit ' (perc * 10000 + initaltit)
                    End If
                    grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit & "|"
                    If grps.Contains("|" & grp & "|") Then Continue For
                    grps = grps & grp & "|"

                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
                    style = AddElement(placemark, "Style", Nothing)
                    linestyle = AddElement(style, "LineStyle", Nothing)
                    AddElement(linestyle, "width", inwd.ToString)
                    polystyle = AddElement(style, "PolyStyle", Nothing)
                    AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    AddElement(linestyle, "color", colri)
                    polygon = AddElement(placemark, "Polygon", Nothing)
                    AddElement(polygon, "extrude", "1")
                    AddElement(polygon, "altitudeMode", "relativeToGround")
                    outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
                    linearring = AddElement(outerb, "LinearRing", Nothing)
                    'circle around point
                    'coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit)
                    If latitudecol = "POINT" Then

                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)
                        If latlon = "True" Then
                            coordinates = CoordinatesCircle(perc * mult, p1, p2, altit, gglmap)
                        Else
                            coordinates = CoordinatesCircle(perc * mult, p2, p1, altit, gglmap)
                        End If


                    ElseIf longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, altit, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = CoordinatesCircle(perc * mult, Piece(coor, ",", 1), Piece(coor, ",", 2), altit, gglmap)
                        End If
                    Else
                        coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit, gglmap)
                    End If
                    AddElement(linearring, "coordinates", coordinates)
                Next
            End If
            'correct <Document ...
            doc.FirstChild.InnerXml = doc.FirstChild.InnerXml.Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            doc.Save(expfile)


            'Dim sr() As String = File.ReadAllLines(expfile)
            'If sr(1).Trim.StartsWith("<Document ") Then
            '    sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            'End If
            ''write in file
            'File.WriteAllLines(expfile, sr)

            ret = expfile
            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Public Function GenerateMapReportCircles(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showpins As Boolean = True, Optional ByVal showlinks As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False, Optional ByVal latlon As String = "") As String
        Dim ret As String = String.Empty
        Dim descr As String = String.Empty
        Dim txtline As String = String.Empty
        Dim i As Integer = 0
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If
        'if circles selected in dropdown for polygons or paths than they are drawn arond the end points 
        If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" Then
            longitudecol = longitudecolend
            latitudecol = latitudecolend
        End If
        Dim coor As String = String.Empty
        'color and density
        Dim dens As String = String.Empty
        Dim colr As String = String.Empty
        Dim dtmin As Integer = 0
        Dim dtmax As Integer = 0
        Dim perc As Integer = 0
        Dim mult As Decimal = 1
        Dim dtval As String = String.Empty
        Dim colri As String = String.Empty
        Dim colrpin As String = "#ffff00"
        Dim dcl As DataTable = GetReportColorField(repid, mapname)
        If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
            'color and field density does not assigned
            'colr = "7f00ffff"
            colr = "#ffff00"
        Else
            dens = dcl.Rows(0)("Val").ToString
            colr = dcl.Rows(0)("Prop4").ToString
            'colr = "7f" & colr.Substring(5, 2) & colr.Substring(3, 2) & colr.Substring(1, 2)
            'calc max and min
            ret = GetMaxMin(dt, dens, dtmin, dtmax)
            If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
                mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
                If mult = 0 Then mult = 1
            End If
        End If

        'Extruded Fields
        Dim extrud As String = String.Empty
        Dim multiplyby As Decimal
        Dim extrudecolorfld As String = String.Empty
        Dim dce As DataTable = GetReportExtrudedFields(repid, mapname)
        If dce IsNot Nothing AndAlso dce.Rows.Count > 0 Then
            extrud = dce.Rows(0)("Val").ToString
            If dce.Rows(0)("Prop4").ToString.Trim = "" Then
                multiplyby = 1
            Else
                multiplyby = Convert.ToDecimal(dce.Rows(0)("Prop4").ToString)
            End If
            If dce.Rows(0)("Prop5").ToString.Trim = "" Then
                extrudecolorfld = ""
            Else
                extrudecolorfld = dce.Rows(0)("Prop5").ToString.Trim
            End If
        End If
        Dim descript As String = String.Empty

        descript = "Field for color density and circle radius: " & dens & " multiplied by " & mult.ToString & ", "
        descript = descript & "Field for extruded altitude: " & extrud & " multiplied by " & multiplyby.ToString & " and with initial altitude " & initaltit.ToString

        Dim grps As String = "|"
        Dim grp As String = String.Empty
        Try
            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
            doc.Load(New StringReader(xmlData))
            Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
            AddElement(docum, "name", mapname & " (" & rep & ")")
            AddElement(docum, "open", "1")
            AddElement(docum, "description", descript)
            Dim style As XmlElement = AddElement(docum, "Style", Nothing)
            Dim attr As XmlAttribute = style.Attributes.Append(doc.CreateAttribute("id"))
            attr.Value = "yellowLineGreenPoly"
            Dim linestyle As XmlElement = AddElement(style, "LineStyle", Nothing)
            AddElement(linestyle, "color", "7f00ffff")
            AddElement(linestyle, "width", inwd.ToString)
            Dim polystyle As XmlElement '= AddElement(style, "PolyStyle", Nothing)

            Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
            AddElement(folder, "name", "Polygons")
            Dim placemark As XmlElement
            Dim polygon As XmlElement
            Dim linearring As XmlElement
            Dim outerb As XmlElement
            Dim point As XmlElement
            Dim altit As Integer = initaltit
            Dim coordinates As String = String.Empty
            Dim p1, p2 As String

            'circles
            'If Not gglmap Then

            For i = 0 To dt.Rows.Count - 1
                If latitudecol = "POINT" Then
                    If IsDBNull(dt.Rows(i)(longitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                        Continue For
                    End If
                Else
                    If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                        Continue For
                    End If
                End If
                'new circle?
                grp = ""
                If dens.Trim <> "" Then
                    dtval = dt.Rows(i)(dens).ToString
                    perc = GetColorDensity(dtval, dtmin, dtmax)
                Else
                    perc = 100
                End If
                colri = GetColor(colr, perc)
                If extrudecolorfld <> "" AndAlso dens.Trim = "" Then  'no density in this case !!!!!!!!!!!!!
                    colri = dt.Rows(i)(extrudecolorfld)
                    If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                        colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                    ElseIf colri.Length = 6 Then
                        colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                    End If
                End If
                If extrud.Trim <> "" Then
                    altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                Else
                    altit = initaltit ' (perc * 10000 + initaltit)
                End If
                grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit & "|"
                If grps.Contains("|" & grp & "|") Then Continue For
                grps = grps & grp & "|"

                placemark = AddElement(folder, "Placemark", Nothing)
                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                AddElement(placemark, "description", descr)
                AddElement(placemark, "styleUrl", "yellowLineGreenPoly")

                style = AddElement(placemark, "Style", Nothing)
                linestyle = AddElement(style, "LineStyle", Nothing)

                AddElement(linestyle, "width", inwd.ToString)
                polystyle = AddElement(style, "PolyStyle", Nothing)

                If dens.Trim <> "" Then
                    dtval = dt.Rows(i)(dens)
                    perc = GetColorDensity(dtval, dtmin, dtmax)
                Else
                    perc = 100
                End If
                If extrudecolorfld = "" Then
                    colri = GetColor(colr, perc)
                Else  'no density in this case !!!!!!!!!!!!!
                    colri = dt.Rows(i)(extrudecolorfld)
                    If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                        colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                    ElseIf colri.Length = 6 Then
                        colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                    End If
                End If
                If extrud.Trim <> "" Then
                    altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                Else
                    altit = initaltit ' (perc * 10000 + initaltit)
                End If

                AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                AddElement(linestyle, "color", colri)

                polygon = AddElement(placemark, "Polygon", Nothing)
                AddElement(polygon, "extrude", "1")

                AddElement(polygon, "altitudeMode", "relativeToGround")
                outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
                linearring = AddElement(outerb, "LinearRing", Nothing)
                'circle around point
                'coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit)
                If latitudecol = "POINT" Then

                    p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                    p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)
                    If latlon = "True" Then
                        coordinates = CoordinatesCircle(perc * mult, p1, p2, altit, gglmap)
                    Else
                        coordinates = CoordinatesCircle(perc * mult, p2, p1, altit, gglmap)
                    End If


                ElseIf longitudecol = latitudecol Then  'address
                    coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, altit, gglmap, rangecol)
                    If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                        coordinates = CoordinatesCircle(perc * mult, Piece(coor, ",", 1), Piece(coor, ",", 2), altit, gglmap)
                    End If
                Else
                    coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit, gglmap)
                End If
                AddElement(linearring, "coordinates", coordinates)
            Next
            ' End If
            'pins
            If showpins Then
                folder = AddElement(docum, "Folder", Nothing)
                AddElement(folder, "name", "Placemarks")
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) Then
                            Continue For
                        End If
                    End If
                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens)
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    'colri = GetColor(colrpin, perc) 'yellow color for placemarks
                    If extrudecolorfld = "" Then
                        colri = GetColor(colr, perc)
                    Else  'no density in this case !!!!!!!!!!!!!
                        colri = dt.Rows(i)(extrudecolorfld)
                        If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                            colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                        ElseIf colri.Length = 6 Then
                            colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                        End If
                    End If
                    If extrud.Trim <> "" Then
                        altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                    Else
                        altit = initaltit '(perc * 10000 + initaltit)
                    End If
                    point = AddElement(placemark, "Point", Nothing)
                    If longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, altit, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            AddElement(point, "coordinates", coor)
                        End If
                    ElseIf latitudecol = "POINT" Then 'in longitudecol the format is POINT(lon lat)
                        'AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)
                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString
                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString
                        End If
                        AddElement(point, "coordinates", coordinates)


                    Else
                        AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString)
                    End If

                Next
            End If
            'correct <Document ...
            doc.FirstChild.InnerXml = doc.FirstChild.InnerXml.Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            doc.Save(expfile)

            ''correct <Document ...
            'Dim sr() As String = File.ReadAllLines(expfile)
            'If sr(1).Trim.StartsWith("<Document ") Then
            '    sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            'End If
            ''write in file
            'File.WriteAllLines(expfile, sr)

            ret = expfile
            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    'Public Function GenerateMapReportPolygons(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showpins As Boolean = True, Optional ByVal showlinks As Boolean = False, Optional ByVal showcircles As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False) As String
    '    'NOT IN USE !!!!!!!!!!!!!!!!!!!!!!!!!
    '    Dim ret As String = String.Empty
    '    Dim descr As String = String.Empty
    '    Dim txtline As String = String.Empty
    '    Dim i As Integer
    '    If dt Is Nothing Then
    '        Return ret
    '        Exit Function
    '    End If

    '    'color and density
    '    Dim dens As String = String.Empty
    '    Dim colr As String = String.Empty
    '    Dim dtmin As Integer = 0
    '    Dim dtmax As Integer = 0
    '    Dim perc As Integer = 0
    '    Dim mult As Decimal = 1
    '    Dim dtval As String = String.Empty
    '    Dim colri As String = String.Empty
    '    Dim dcl As DataTable = GetReportColorField(repid, mapname)
    '    If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
    '        'color and field density does not assigned
    '        colr = "#ffff00"
    '    Else
    '        dens = dcl.Rows(0)("Val").ToString
    '        colr = dcl.Rows(0)("Prop4").ToString
    '        'calc max and min
    '        ret = GetMaxMin(dt, dens, dtmin, dtmax)
    '        If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
    '            mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
    '            If mult = 0 Then mult = 1
    '        End If
    '    End If

    '    Dim drk As DataTable = GetReportKeyFields(repid, mapname)
    '    Dim grps As String = "|"
    '    Dim grp As String = String.Empty
    '    Try
    '        'Start doc
    '        Dim doc As New XmlDocument
    '        Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
    '        doc.Load(New StringReader(xmlData))
    '        Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
    '        AddElement(docum, "name", mapname)
    '        AddElement(docum, "open", "1")
    '        AddElement(docum, "description", rep)
    '        Dim style As XmlElement = AddElement(docum, "Style", Nothing)
    '        Dim attr As XmlAttribute = style.Attributes.Append(doc.CreateAttribute("id"))
    '        attr.Value = "yellowLineGreenPoly"
    '        Dim linestyle As XmlElement = AddElement(style, "LineStyle", Nothing)
    '        'AddElement(linestyle, "color", "7f00ffff")
    '        AddElement(linestyle, "width", inwd.ToString)
    '        Dim polystyle As XmlElement
    '        Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
    '        AddElement(folder, "name", "Polygons")
    '        Dim placemark As XmlElement
    '        Dim polygon As XmlElement
    '        Dim linearring As XmlElement
    '        Dim outerb As XmlElement
    '        Dim point As XmlElement
    '        Dim altit As Integer = initaltit


    '        'polygons
    '        For i = 0 To dt.Rows.Count - 1
    '            If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
    '                Continue For
    '            End If
    '            'key filters
    '            grp = ""
    '            For j = 0 To drk.Rows.Count - 1
    '                If drk.Rows(j)("ForMap").ToString = "KeyField" Then
    '                    If ColumnTypeIsNumeric(dt.Columns(drk.Rows(j)("KeyField").ToString)) Then
    '                        grp = grp & drk.Rows(j)("KeyField").ToString & "=" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString
    '                    Else
    '                        grp = grp & drk.Rows(j)("KeyField").ToString & "='" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString & "'"
    '                    End If
    '                    If j < drk.Rows.Count - 1 Then
    '                        grp = grp & " AND "
    '                    End If
    '                End If
    '            Next
    '            If grps.Contains("|" & grp & "|") Then Continue For
    '            grps = grps & grp & "|"
    '            placemark = AddElement(folder, "Placemark", Nothing)
    '            AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '            descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '            AddElement(placemark, "description", descr)
    '            AddElement(placemark, "styleUrl", "yellowLineGreenPoly")

    '            style = AddElement(placemark, "Style", Nothing)
    '            linestyle = AddElement(style, "LineStyle", Nothing)
    '            'AddElement(linestyle, "color", "7f00ffff")
    '            AddElement(linestyle, "width", inwd.ToString)
    '            polystyle = AddElement(style, "PolyStyle", Nothing)
    '            'dtval = dt.Rows(i)(dens)
    '            'perc = GetColorDensity(dtval, dtmin, dtmax)
    '            If dens.Trim <> "" Then
    '                dtval = dt.Rows(i)(dens)
    '                perc = GetColorDensity(dtval, dtmin, dtmax)
    '            Else
    '                perc = 100
    '            End If
    '            colri = GetColor(colr, perc)
    '            AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '            AddElement(linestyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '            polygon = AddElement(placemark, "Polygon", Nothing)
    '            AddElement(polygon, "extrude", "1")

    '            'AddElement(polygon, "altitudeMode", "absolute")
    '            AddElement(polygon, "altitudeMode", "relativeToGround")
    '            outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
    '            linearring = AddElement(outerb, "LinearRing", Nothing)

    '            'first point
    '            Dim coordinates As String = String.Empty
    '            If longitudecol.Trim <> "" AndAlso latitudecol.Trim <> "" Then
    '                coordinates = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
    '            End If
    '            If drf.Rows.Count > 0 Then
    '                coordinates = coordinates & CoordinatesInRow(dt, drf, i, altit)
    '            End If
    '            If drk.Rows.Count > 0 Then
    '                coordinates = coordinates & CoordinatesInColumns(dt, longitudecol, latitudecol, drk, i, altit, grp)
    '            End If
    '            If longitudecol.Trim <> "" AndAlso latitudecol.Trim <> "" Then
    '                coordinates = coordinates & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
    '            End If
    '            AddElement(linearring, "coordinates", coordinates)
    '            'If gglmap AndAlso i > 5 Then
    '            '    Exit For
    '            'End If
    '        Next
    '        'pins show end ponts
    '        If longitudecolend.Trim = "" OrElse latitudecolend.Trim = "" Then
    '            showpins = False
    '            showcircles = False
    '        End If
    '        If showpins Then
    '            folder = AddElement(docum, "Folder", Nothing)
    '            AddElement(folder, "name", "Placemarks")
    '            For i = 0 To dt.Rows.Count - 1
    '                If IsDBNull(dt.Rows(i)(longitudecolend)) OrElse IsDBNull(dt.Rows(i)(latitudecolend)) Then
    '                    Continue For
    '                End If
    '                placemark = AddElement(folder, "Placemark", Nothing)
    '                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '                AddElement(placemark, "description", descr)
    '                point = AddElement(placemark, "Point", Nothing)
    '                AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit.ToString)
    '            Next
    '        End If
    '        'circles
    '        If showcircles Then
    '            AddElement(folder, "name", "Circles")
    '            'Dim polygon As XmlElement
    '            'Dim linearring As XmlElement
    '            'Dim outerb As XmlElement
    '            For i = 0 To dt.Rows.Count - 1
    '                If IsDBNull(dt.Rows(i)(longitudecolend)) OrElse IsDBNull(dt.Rows(i)(latitudecolend)) Then
    '                    Continue For
    '                End If
    '                placemark = AddElement(folder, "Placemark", Nothing)
    '                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
    '                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
    '                AddElement(placemark, "description", descr)
    '                AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
    '                style = AddElement(placemark, "Style", Nothing)
    '                linestyle = AddElement(style, "LineStyle", Nothing)
    '                AddElement(linestyle, "color", "7f00ffff")
    '                AddElement(linestyle, "width", inwd.ToString)
    '                polystyle = AddElement(style, "PolyStyle", Nothing)
    '                'dtval = dt.Rows(i)(dens)
    '                'perc = GetColorDensity(dtval, dtmin, dtmax)
    '                If dens.Trim <> "" Then
    '                    dtval = dt.Rows(i)(dens)
    '                    perc = GetColorDensity(dtval, dtmin, dtmax)
    '                Else
    '                    perc = 100
    '                End If
    '                colri = GetColor(colr, perc)
    '                AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    '                'AddElement(linestyle, "color", colri)
    '                polygon = AddElement(placemark, "Polygon", Nothing)
    '                AddElement(polygon, "extrude", "1")
    '                AddElement(polygon, "altitudeMode", "relativeToGround")
    '                outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
    '                linearring = AddElement(outerb, "LinearRing", Nothing)
    '                'circle around end point
    '                Dim coordinates As String = String.Empty
    '                coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecolend).ToString, dt.Rows(i)(latitudecolend).ToString, altit)
    '                AddElement(linearring, "coordinates", coordinates)
    '            Next
    '        End If

    '        doc.Save(expfile)

    '        'correct <Document ...
    '        Dim sr() As String = File.ReadAllLines(expfile)
    '        If sr(1).Trim.StartsWith("<Document ") Then
    '            sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
    '        End If
    '        'write in file
    '        File.WriteAllLines(expfile, sr)

    '        ret = expfile
    '        Return ret
    '    Catch ex As Exception
    '        ret = ex.Message
    '    End Try
    '    Return ret
    'End Function
    Public Function GenerateMapReportExtrudedPolygons(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showpins As Boolean = True, Optional ByVal showlinks As Boolean = False, Optional ByVal showcircles As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False, Optional ByVal latlon As String = "") As String
        Dim ret As String = String.Empty
        Dim descr As String = String.Empty
        Dim txtline As String = String.Empty
        Dim i As Integer = 0
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If
        Dim coor As String = String.Empty
        'color and density
        Dim dens As String = String.Empty
        Dim colr As String = String.Empty
        Dim dtmin As Integer = 0
        Dim dtmax As Integer = 0
        Dim perc As Integer = 0
        Dim mult As Decimal = 1
        Dim dtval As String = String.Empty
        Dim colri As String = String.Empty
        Dim dcl As DataTable = GetReportColorField(repid, mapname)
        If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
            'color and field density does not assigned
            colr = "#ffff00"
        Else
            dens = dcl.Rows(0)("Val").ToString
            colr = dcl.Rows(0)("Prop4").ToString
            'calc max and min
            ret = GetMaxMin(dt, dens, dtmin, dtmax)
            If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
                mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
                If mult = 0 Then mult = 1
            End If
        End If

        'Extruded Fields
        Dim extrud As String = String.Empty
        Dim multiplyby As Decimal
        Dim extrudecolorfld As String = String.Empty
        dcl = GetReportExtrudedFields(repid, mapname)
        If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
            extrud = dcl.Rows(0)("Val").ToString
            If dcl.Rows(0)("Prop4").ToString.Trim = "" Then
                multiplyby = 1
            Else
                multiplyby = Convert.ToDecimal(dcl.Rows(0)("Prop4").ToString)
            End If
            If dcl.Rows(0)("Prop5").ToString.Trim = "" Then
                extrudecolorfld = ""
            Else
                extrudecolorfld = dcl.Rows(0)("Prop5").ToString.Trim
            End If
        End If
        Dim drk As DataTable = GetReportKeyFields(repid, mapname)

        Dim descript As String = String.Empty

        descript = "Field for color density and circle radius: " & dens & " multiplied by " & mult.ToString & ", "
        descript = descript & "Field for extruded altitude: " & extrud & " multiplied by " & multiplyby.ToString & " and with initial altitude " & initaltit.ToString

        Try
            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
            doc.Load(New StringReader(xmlData))
            Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
            AddElement(docum, "name", mapname & " (" & rep & ")")
            AddElement(docum, "open", "1")
            AddElement(docum, "description", descript)
            Dim style As XmlElement = AddElement(docum, "Style", Nothing)
            Dim attr As XmlAttribute = style.Attributes.Append(doc.CreateAttribute("id"))
            attr.Value = "yellowLineGreenPoly"
            Dim linestyle As XmlElement = AddElement(style, "LineStyle", Nothing)
            AddElement(linestyle, "color", "7f00ffff")
            AddElement(linestyle, "width", inwd.ToString)
            Dim polystyle As XmlElement
            Dim multigeometry As XmlElement
            Dim firstpoint As String = String.Empty
            Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
            AddElement(folder, "name", "Polygons")
            Dim placemark As XmlElement
            Dim polygon As XmlElement
            Dim linearring As XmlElement
            Dim outerb As XmlElement
            Dim point As XmlElement
            Dim altit As Integer = initaltit
            Dim grps As String = "|"
            Dim grp As String = String.Empty
            Dim coordinates As String = String.Empty
            Dim p1, p2 As String

            'polygons
            For i = 0 To dt.Rows.Count - 1
                If latitudecol = "POINT" Then
                    If IsDBNull(dt.Rows(i)(longitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                        Continue For
                    End If
                Else
                    If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) Then
                        Continue For
                    End If
                End If
                Try
                    'key filters
                    grp = ""
                    If drk.Rows.Count > 0 Then
                        For j = 0 To drk.Rows.Count - 1
                            If drk.Rows(j)("ForMap").ToString = "KeyField" Then
                                If ColumnTypeIsNumeric(dt.Columns(drk.Rows(j)("KeyField").ToString)) Then
                                    grp = grp & drk.Rows(j)("KeyField").ToString & "=" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString
                                Else
                                    grp = grp & drk.Rows(j)("KeyField").ToString & "='" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString & "'"
                                End If
                                If j < drk.Rows.Count - 1 Then
                                    grp = grp & " AND "
                                End If
                            End If
                        Next
                        If grps.Contains("|" & grp & "|") Then Continue For
                        grps = grps & grp & "|"
                    End If

                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
                    style = AddElement(placemark, "Style", Nothing)
                    linestyle = AddElement(style, "LineStyle", Nothing)
                    AddElement(linestyle, "width", inwd.ToString)
                    polystyle = AddElement(style, "PolyStyle", Nothing)
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens)
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    If extrudecolorfld = "" Then
                        colri = GetColor(colr, perc)
                    Else  'no density in this case, paths have color from extrudecolorfld, and only if it is empty, then from dens !!!!!!!!!!!!!
                        colri = dt.Rows(i)(extrudecolorfld)
                        If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                            colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                        ElseIf colri.Length = 6 Then
                            colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                        End If
                    End If
                    AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    AddElement(linestyle, "color", colri)
                    multigeometry = AddElement(placemark, "MultiGeometry", Nothing)
                    polygon = AddElement(multigeometry, "Polygon", Nothing)
                    AddElement(polygon, "extrude", "1")
                    'AddElement(polygon, "extrude", (perc * 5 + 4000).ToString)
                    AddElement(polygon, "tessellate", "1")
                    AddElement(polygon, "altitudeMode", "relativeToGround")
                    'AddElement(polygon, "altitudeMode", "absolute")
                    outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
                    linearring = AddElement(outerb, "LinearRing", Nothing)
                    If extrud.Trim <> "" Then
                        altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                    Else
                        altit = initaltit '(perc * 10000 + initaltit)
                    End If
                    AddElement(linearring, "extrude", "1")
                    AddElement(linearring, "tessellate", "1")
                    'AddElement(linearring, "altitudeMode", "clampToGround")
                    AddElement(linearring, "altitudeMode", "relativeToGround")
                    'AddElement(linearring, "altitudeMode", "absolute")

                    'first point
                    If longitudecol.Trim <> "" AndAlso latitudecol.Trim <> "" Then
                        'coordinates = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
                        If latitudecol = "POINT" Then
                            'coordinates = dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString & " "
                            p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                            p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                            If latlon = "True" Then
                                coordinates = p1 & "," & p2 & "," & altit.ToString
                            Else
                                coordinates = p2 & "," & p1 & "," & altit.ToString
                            End If
                            'AddElement(point, "coordinates", coordinates)


                        ElseIf longitudecol = latitudecol Then  'address
                            coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, 0, gglmap, rangecol)
                            If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                                coordinates = coor
                            End If
                        Else
                            coordinates = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
                        End If
                        firstpoint = coordinates
                    End If
                    If drf.Rows.Count > 0 Then
                        coordinates = coordinates & CoordinatesInRow(dt, drf, i, altit, gglmap, rangecol)
                    End If
                    If drk.Rows.Count > 0 Then
                        coordinates = coordinates & CoordinatesInColumns(dt, longitudecol, latitudecol, drk, i, altit, grp, gglmap, rangecol)
                    End If
                    If longitudecol.Trim <> "" AndAlso latitudecol.Trim <> "" Then
                        coordinates = coordinates & firstpoint
                    End If

                    AddElement(linearring, "coordinates", coordinates)
                    'WriteToAccessLog(rep, "Record: " & i.ToString & " text so far: " & coordinates, 111)
                Catch ex As Exception
                    ret = ex.Message
                    WriteToAccessLog(rep, "Record: " & i.ToString & " ERROR!! " & ret, 111)
                End Try
            Next

            'pins and circles - show end points, and if they are not assign than first point
            If longitudecolend.Trim = "" OrElse latitudecolend.Trim = "" Then
                latitudecolend = latitudecol
                longitudecolend = longitudecol
            End If
            If showpins Then
                folder = AddElement(docum, "Folder", Nothing)
                AddElement(folder, "name", "Placemarks")
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If IsDBNull(dt.Rows(i)(longitudecolend)) OrElse IsDBNull(dt.Rows(i)(latitudecolend)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecolend) = dt.Rows(i - 1)(longitudecolend) AndAlso dt.Rows(i)(latitudecolend) = dt.Rows(i - 1)(latitudecolend) Then
                            Continue For
                        End If
                    End If
                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    'point = AddElement(placemark, "Point", Nothing)
                    'AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0 ")
                    point = AddElement(placemark, "Point", Nothing)
                    If longitudecolend = latitudecolend Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecolend).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            AddElement(point, "coordinates", coor)
                        End If
                    ElseIf latitudecolend = "POINT" Then 'in longitudecolend the format is POINT(lon lat)
                        'AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)
                        p1 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString
                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString
                        End If
                        AddElement(point, "coordinates", coordinates)

                    Else
                        AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0")
                    End If
                Next
            End If

            'circles
            grps = "|"
            If showcircles Then
                AddElement(folder, "name", "Polygons")
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If IsDBNull(dt.Rows(i)(longitudecolend)) OrElse IsDBNull(dt.Rows(i)(latitudecolend)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecolend) = dt.Rows(i - 1)(longitudecolend) AndAlso dt.Rows(i)(latitudecolend) = dt.Rows(i - 1)(latitudecolend) Then
                            Continue For
                        End If
                    End If
                    'new circle?
                    grp = ""
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens).ToString
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    colri = GetColor(colr, perc)
                    If extrudecolorfld <> "" AndAlso dens.Trim = "" Then
                        'no density in this case !!!!!!!!!!!!!
                        colri = dt.Rows(i)(extrudecolorfld)
                        If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                            colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                        ElseIf colri.Length = 6 Then
                            colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                        End If
                    End If
                    If extrud.Trim <> "" Then
                        altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                    Else
                        altit = initaltit '(perc * 10000 + initaltit)
                    End If
                    grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit & "|"
                    If grps.Contains("|" & grp & "|") Then Continue For
                    grps = grps & grp & "|"

                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
                    style = AddElement(placemark, "Style", Nothing)
                    linestyle = AddElement(style, "LineStyle", Nothing)
                    AddElement(linestyle, "width", inwd.ToString)
                    polystyle = AddElement(style, "PolyStyle", Nothing)
                    AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    AddElement(linestyle, "color", colri)
                    polygon = AddElement(placemark, "Polygon", Nothing)
                    AddElement(polygon, "extrude", "1")
                    AddElement(polygon, "tessellate", "1")
                    AddElement(polygon, "altitudeMode", "relativeToGround")
                    'AddElement(polygon, "altitudeMode", "absolute")
                    outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
                    linearring = AddElement(outerb, "LinearRing", Nothing)
                    AddElement(linearring, "extrude", "1")
                    AddElement(linearring, "tessellate", "1")
                    'AddElement(linearring, "altitudeMode", "clampToGround")
                    AddElement(linearring, "altitudeMode", "relativeToGround")
                    'AddElement(linearring, "altitudeMode", "absolute")
                    'circle around point
                    ' Dim coordinates As String = String.Empty
                    'coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecolend).ToString, dt.Rows(i)(latitudecolend).ToString, altit)
                    If latitudecolend = "POINT" Then
                        'Dim ln, lt As String
                        'ln = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        'lt = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)
                        'coordinates = CoordinatesCircle(perc * mult, ln, lt, altit, gglmap)
                        p1 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = CoordinatesCircle(perc * mult, p1, p2, altit, gglmap)
                        Else
                            coordinates = CoordinatesCircle(perc * mult, p2, p1, altit, gglmap)
                        End If

                    ElseIf longitudecolend = latitudecolend Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecolend).ToString, altit, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = CoordinatesCircle(perc * mult, Piece(coor, ",", 1), Piece(coor, ",", 2), altit, gglmap)
                        End If
                    Else
                        coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecolend).ToString, dt.Rows(i)(latitudecolend).ToString, altit, gglmap)
                    End If
                    AddElement(linearring, "coordinates", coordinates)
                Next
            End If
            'correct <Document ...
            doc.FirstChild.InnerXml = doc.FirstChild.InnerXml.Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            doc.Save(expfile)

            ''correct <Document ...
            'Dim sr() As String = File.ReadAllLines(expfile)
            'If sr(1).Trim.StartsWith("<Document ") Then
            '    sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            'End If
            ''write in file
            'File.WriteAllLines(expfile, sr)
            ''WriteToAccessLog(rep, "File: " & ret, 110)
            'ret = expfile

            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
            WriteToAccessLog(rep, "ERROR!! " & ret, 112)
        End Try
        Return ret
    End Function
    Public Function GenerateMapReportTours(ByVal rep As String, ByVal mapname As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal drf As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String, Optional ByVal longitudecolend As String = "", Optional ByVal latitudecolend As String = "", Optional ByVal showpins As Boolean = True, Optional ByVal showlinks As Boolean = False, Optional ByVal showcircles As Boolean = False, Optional ByVal initaltit As Integer = 4000, Optional ByVal inwd As Integer = 4, Optional ByVal gglmap As Boolean = False, Optional ByVal latlon As String = "") As String
        Dim ret As String = String.Empty
        Dim descr As String = String.Empty
        Dim txtline As String = String.Empty
        Dim m, i, j As Integer
        If dt Is Nothing Then
            Return ret
            Exit Function
        End If
        Dim coor As String = String.Empty
        'color and density
        Dim dens As String = String.Empty
        Dim colr As String = String.Empty
        Dim dtmin As Integer = 0
        Dim dtmax As Integer = 0
        Dim perc As Integer = 100
        Dim mult As Decimal = 1
        Dim dtval As String = String.Empty
        Dim colri As String = String.Empty
        Dim dcl As DataTable = GetReportColorField(repid, mapname)
        If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
            'color and field density does not assigned
            colr = "#ffff00"
        Else
            dens = dcl.Rows(0)("Val").ToString
            colr = dcl.Rows(0)("Prop4").ToString
            'calc max and min
            ret = GetMaxMin(dt, dens, dtmin, dtmax)
            If IsNumeric(dcl.Rows(0)("Prop5").ToString) Then
                mult = Convert.ToDecimal(dcl.Rows(0)("Prop5").ToString)
                If mult = 0 Then mult = 1
            End If
        End If

        'Extruded Fields
        Dim extrud As String = String.Empty
        Dim multiplyby As Decimal
        Dim extrudecolorfld As String = String.Empty
        dcl = GetReportExtrudedFields(repid, mapname)
        If dcl IsNot Nothing AndAlso dcl.Rows.Count > 0 Then
            extrud = dcl.Rows(0)("Val").ToString
            If dcl.Rows(0)("Prop4").ToString.Trim = "" Then
                multiplyby = 1
            Else
                multiplyby = Convert.ToDecimal(dcl.Rows(0)("Prop4").ToString)
            End If
            If dcl.Rows(0)("Prop5").ToString.Trim = "" Then
                extrudecolorfld = ""
            Else
                extrudecolorfld = dcl.Rows(0)("Prop5").ToString.Trim
            End If
        End If
        Dim descript As String = String.Empty

        descript = "Field for color density and circle radius: " & dens & " multiplied by " & mult.ToString & ", "
        descript = descript & "Field for extruded altitude: " & extrud & " multiplied by " & multiplyby.ToString & " and with initial altitude " & initaltit.ToString

        'group
        Dim drk As DataTable = GetReportKeyFields(repid, mapname)
        'Dim grps As String = "|"
        'Dim grp As String = String.Empty
        Try
            'Start doc
            Dim doc As New XmlDocument
            Dim xmlData As String = "<kml xmlns='http://www.opengis.net/kml/2.2'></kml>"
            doc.Load(New StringReader(xmlData))
            Dim docum As XmlElement = AddElement(doc.FirstChild, "Document", Nothing)
            AddElement(docum, "name", mapname & " (" & rep & ")")
            AddElement(docum, "open", "1")
            AddElement(docum, "description", descript)
            Dim style As XmlElement = AddElement(docum, "Style", Nothing)
            Dim attr As XmlAttribute = style.Attributes.Append(doc.CreateAttribute("id"))
            attr.Value = "yellowLineGreenPoly"
            Dim linestyle As XmlElement
            Dim polystyle As XmlElement
            Dim pointstyle As XmlElement
            Dim altit As Integer = initaltit
            Dim coordinates As String = String.Empty
            Dim folder As XmlElement = AddElement(docum, "Folder", Nothing)
            AddElement(folder, "name", "Placemarks")
            Dim placemark As XmlElement
            Dim linestring As XmlElement
            Dim point As XmlElement
            Dim timespan As XmlElement
            Dim timestamp As XmlElement
            Dim multigeometry As XmlElement
            Dim starttm As String = String.Empty
            Dim endtm As String = String.Empty
            Dim grps As String = "|"
            Dim grp As String = String.Empty
            Dim firstpoint As String = String.Empty
            Dim p1, p2 As String

            'paths
            For i = 0 To dt.Rows.Count - 1
                If latitudecol = "POINT" Then
                    If IsDBNull(dt.Rows(i)(longitudecol)) Then
                        Continue For
                    End If
                    If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                        Continue For
                    End If
                Else
                    If longitudecol = "" OrElse latitudecol = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                        Continue For
                    End If
                    If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) OrElse IsDBNull(dt.Rows(i)(starttimecol)) OrElse IsDBNull(dt.Rows(i)(endtimecol)) Then
                        Continue For
                    End If
                End If
                'key filters
                grp = ""
                If drk.Rows.Count > 0 Then
                    For j = 0 To drk.Rows.Count - 1
                        If drk.Rows(j)("ForMap").ToString = "KeyField" Then
                            If ColumnTypeIsNumeric(dt.Columns(drk.Rows(j)("KeyField").ToString)) Then
                                grp = grp & drk.Rows(j)("KeyField").ToString & "=" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString
                            Else
                                grp = grp & drk.Rows(j)("KeyField").ToString & "='" & dt.Rows(i)(drk.Rows(j)("KeyField").ToString).ToString & "'"
                            End If
                            If j < drk.Rows.Count - 1 Then
                                grp = grp & " AND "
                            End If
                        End If
                    Next
                    If grps.Contains("|" & grp & "|") Then Continue For
                    grps = grps & grp & "|"
                End If

                placemark = AddElement(folder, "Placemark", Nothing)
                AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                AddElement(placemark, "description", descr)
                AddElement(placemark, "styleUrl", "yellowLineGreenPoly")

                starttm = dt.Rows(i)(starttimecol).ToString
                endtm = dt.Rows(i)(endtimecol).ToString
                starttm = DateToString(starttm).Replace(" ", "T") & "Z"
                endtm = DateToString(endtm).Replace(" ", "T") & "Z"
                timespan = AddElement(placemark, "TimeSpan", Nothing)
                If starttimecol.Trim <> "" Then AddElement(timespan, "begin", starttm)
                If endtimecol.Trim <> "" Then AddElement(timespan, "end", endtm)

                style = AddElement(placemark, "Style", Nothing)
                linestyle = AddElement(style, "LineStyle", Nothing)
                'AddElement(linestyle, "color", "7f00ffff")
                AddElement(linestyle, "width", inwd.ToString)
                polystyle = AddElement(style, "PolyStyle", Nothing)


                If extrudecolorfld = "" Then
                    colri = GetColor(colr, perc)
                Else  'no density in this case, tours have color from extrudecolorfld, and only if it is empty, then from dens !!!!!!!!!!!!!
                    colri = dt.Rows(i)(extrudecolorfld)
                    If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                        colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                    ElseIf colri.Length = 6 Then
                        colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                    End If
                End If
                AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                AddElement(linestyle, "color", colri)
                multigeometry = AddElement(placemark, "MultiGeometry", Nothing)
                linestring = AddElement(multigeometry, "LineString", Nothing)
                If extrud.Trim <> "" Then
                    altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                Else
                    altit = initaltit '(perc * 10000 + initaltit)
                End If

                AddElement(linestring, "extrude", "1")
                AddElement(linestring, "tessellate", "1")
                'AddElement(linestring, "altitudeMode", "absolute")
                AddElement(linestring, "altitudeMode", "relativeToGround")
                'first point
                If longitudecol.Trim <> "" AndAlso latitudecol.Trim <> "" Then

                    If latitudecol = "POINT" Then
                        'coordinates = dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString & " "
                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString

                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString

                        End If

                    ElseIf longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = coor
                        End If
                    Else
                        coordinates = dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit.ToString & " "
                    End If
                    firstpoint = coordinates
                End If
                'in row
                If drf.Rows.Count > 0 Then
                    coordinates = coordinates & CoordinatesInRow(dt, drf, i, altit, gglmap, rangecol, latlon)
                End If
                'in column
                If drk.Rows.Count > 0 Then
                    coordinates = coordinates & CoordinatesInColumns(dt, longitudecol, latitudecol, drk, i, altit, grp, gglmap, rangecol, latlon)
                End If
                'end point
                If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" AndAlso Not IsDBNull(dt.Rows(i)(longitudecolend)) AndAlso Not IsDBNull(dt.Rows(i)(latitudecolend)) AndAlso IsNumeric(dt.Rows(i)(longitudecolend)) AndAlso (dt.Rows(i)(longitudecolend).ToString.Trim <> "0" Or dt.Rows(i)(latitudecolend).ToString.Trim <> "0") Then
                    'coordinates = coordinates & dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit.ToString & " "
                    If latitudecolend = "POINT" Then
                        'coordinates = dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString & " "
                        p1 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString

                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString

                        End If

                    ElseIf longitudecolend = latitudecolend Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecolend).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = coordinates & coor & " "
                        End If
                    Else
                        coordinates = coordinates & dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & "," & altit.ToString & " "
                    End If
                ElseIf firstpoint = coordinates Then
                    coordinates = coordinates & firstpoint
                End If
                AddElement(linestring, "coordinates", coordinates)
            Next

            If showpins Then
                'start points
                folder = AddElement(docum, "Folder", Nothing)
                AddElement(folder, "name", "Placemarks")
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If longitudecol = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                            Continue For
                        End If
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If longitudecol = "" OrElse latitudecol = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                            Continue For
                        End If
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) OrElse IsDBNull(dt.Rows(i)(starttimecol)) OrElse IsDBNull(dt.Rows(i)(endtimecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) AndAlso dt.Rows(i)(starttimecol) = dt.Rows(i - 1)(starttimecol) AndAlso dt.Rows(i)(endtimecol) = dt.Rows(i - 1)(endtimecol) Then
                            Continue For
                        End If
                    End If
                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)

                    starttm = dt.Rows(i)(starttimecol).ToString
                    starttm = DateToString(starttm).Replace(" ", "T") & "Z"
                    timestamp = AddElement(placemark, "TimeStamp", Nothing)
                    If starttimecol.Trim <> "" Then AddElement(timestamp, "when", starttm)
                    'point = AddElement(placemark, "Point", Nothing)
                    'AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",0")
                    point = AddElement(placemark, "Point", Nothing)
                    If longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            AddElement(point, "coordinates", coor)
                        End If
                    ElseIf latitudecol = "POINT" Then 'in longitudecol the format is POINT(lon lat)
                        'AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)
                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString
                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString
                        End If

                        AddElement(point, "coordinates", coordinates)
                    Else
                        AddElement(point, "coordinates", dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",0")
                    End If
                Next

                'end points
                folder = AddElement(docum, "Folder", Nothing)
                AddElement(folder, "name", "Placemarks")
                For i = 0 To dt.Rows.Count - 1
                    If latitudecolend = "POINT" Then
                        If longitudecolend = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                            Continue For
                        End If
                        If IsDBNull(dt.Rows(i)(longitudecolend)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecolend) = dt.Rows(i - 1)(longitudecolend) Then
                            Continue For
                        End If
                    Else
                        If longitudecolend = "" OrElse latitudecolend = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                            Continue For
                        End If
                        If IsDBNull(dt.Rows(i)(longitudecolend)) OrElse IsDBNull(dt.Rows(i)(latitudecolend)) OrElse IsDBNull(dt.Rows(i)(starttimecol)) OrElse IsDBNull(dt.Rows(i)(endtimecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecolend) = dt.Rows(i - 1)(longitudecolend) AndAlso dt.Rows(i)(latitudecolend) = dt.Rows(i - 1)(latitudecolend) AndAlso dt.Rows(i)(starttimecol) = dt.Rows(i - 1)(starttimecol) AndAlso dt.Rows(i)(endtimecol) = dt.Rows(i - 1)(endtimecol) Then
                            Continue For
                        End If
                    End If
                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)

                    'starttm = dt.Rows(i)(starttimecol).ToString
                    endtm = dt.Rows(i)(endtimecol).ToString
                    'starttm = DateToString(starttm).Replace(" ", "T") & "Z"
                    endtm = DateToString(endtm).Replace(" ", "T") & "Z"
                    timestamp = AddElement(placemark, "TimeStamp", Nothing)
                    'If starttimecol.Trim <> "" Then AddElement(timestamp, "when", starttm)
                    If endtimecol.Trim <> "" Then AddElement(timestamp, "when", endtm)
                    style = AddElement(placemark, "Style", Nothing)
                    pointstyle = AddElement(style, "IconStyle", Nothing)
                    'dtval = dt.Rows(i)(dens)
                    'perc = GetColorDensity(dtval, dtmin, dtmax)
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens)
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    colri = GetColor(colr, perc)
                    AddElement(pointstyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    'point = AddElement(placemark, "Point", Nothing)
                    'AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0")
                    point = AddElement(placemark, "Point", Nothing)
                    If longitudecolend = latitudecolend Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecolend).ToString, 0, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            AddElement(point, "coordinates", coor)
                        End If
                    ElseIf latitudecolend = "POINT" Then 'in longitudecolend the format is POINT(lon lat)
                        'AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ",") & "," & altit.ToString)
                        p1 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecolend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            coordinates = p1 & "," & p2 & "," & altit.ToString

                        Else
                            coordinates = p2 & "," & p1 & "," & altit.ToString

                        End If

                        AddElement(point, "coordinates", coordinates)
                    Else
                        AddElement(point, "coordinates", dt.Rows(i)(longitudecolend).ToString & "," & dt.Rows(i)(latitudecolend).ToString & ",0")
                    End If
                Next
            End If

            'if circles checked for polygons or paths than they are drawn arond the end points 
            If longitudecolend.Trim <> "" AndAlso latitudecolend.Trim <> "" Then
                longitudecol = longitudecolend
                latitudecol = latitudecolend
            End If
            'circles
            grps = "|"
            If showcircles Then
                altit = initaltit
                AddElement(folder, "name", "Polygons")
                Dim polygon As XmlElement
                Dim linearring As XmlElement
                Dim outerb As XmlElement
                For i = 0 To dt.Rows.Count - 1
                    If latitudecol = "POINT" Then
                        If longitudecol = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                            Continue For
                        End If
                        If IsDBNull(dt.Rows(i)(longitudecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) Then
                            Continue For
                        End If
                    Else
                        If longitudecol = "" OrElse latitudecol = "" OrElse starttimecol = "" OrElse endtimecol = "" Then
                            Continue For
                        End If
                        If IsDBNull(dt.Rows(i)(longitudecol)) OrElse IsDBNull(dt.Rows(i)(latitudecol)) OrElse IsDBNull(dt.Rows(i)(starttimecol)) OrElse IsDBNull(dt.Rows(i)(endtimecol)) Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(longitudecol) = dt.Rows(i - 1)(longitudecol) AndAlso dt.Rows(i)(latitudecol) = dt.Rows(i - 1)(latitudecol) AndAlso dt.Rows(i)(starttimecol) = dt.Rows(i - 1)(starttimecol) AndAlso dt.Rows(i)(endtimecol) = dt.Rows(i - 1)(endtimecol) Then
                            Continue For
                        End If
                    End If
                    'new circle? for circles if highest density field assign than color takes from it, not from extruded color field which is used for coloring the paths
                    grp = ""
                    If dens.Trim <> "" Then
                        dtval = dt.Rows(i)(dens).ToString
                        perc = GetColorDensity(dtval, dtmin, dtmax)
                    Else
                        perc = 100
                    End If
                    colri = GetColor(colr, perc)
                    If extrudecolorfld <> "" AndAlso dens.Trim = "" Then
                        'no density in this case !!!!!!!!!!!!!
                        colri = dt.Rows(i)(extrudecolorfld)
                        If colri.StartsWith("#") AndAlso colri.Length = 7 Then
                            colri = "7f" & colri.Substring(5, 2) & colri.Substring(3, 2) & colri.Substring(1, 2)
                        ElseIf colri.Length = 6 Then
                            colri = "7f" & colri.Substring(4, 2) & colri.Substring(2, 2) & colri.Substring(0, 2)
                        End If
                    End If
                    If extrud.Trim <> "" Then
                        altit = (CInt(dt.Rows(i)(extrud).ToString) * multiplyby + initaltit)
                    Else
                        altit = initaltit ' (perc * 10000 + initaltit)
                    End If
                    grp = "|" & perc.ToString & "," & colri.ToString & "," & mult.ToString & "," & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & "," & altit & "|"
                    If grps.Contains("|" & grp & "|") Then Continue For
                    grps = grps & grp & "|"

                    placemark = AddElement(folder, "Placemark", Nothing)
                    AddElement(placemark, "name", dt.Rows(i)(namecol).ToString)
                    descr = DescriptionText(dt, drf, i, showlinks, gglmap)
                    AddElement(placemark, "description", descr)
                    AddElement(placemark, "styleUrl", "yellowLineGreenPoly")
                    style = AddElement(placemark, "Style", Nothing)
                    linestyle = AddElement(style, "LineStyle", Nothing)
                    AddElement(linestyle, "width", inwd.ToString)
                    polystyle = AddElement(style, "PolyStyle", Nothing)

                    starttm = dt.Rows(i)(starttimecol).ToString
                    endtm = dt.Rows(i)(endtimecol).ToString
                    starttm = DateToString(starttm).Replace(" ", "T") & "Z"
                    endtm = DateToString(endtm).Replace(" ", "T") & "Z"
                    timespan = AddElement(placemark, "TimeSpan", Nothing)
                    If starttimecol.Trim <> "" Then AddElement(timespan, "begin", starttm)
                    If endtimecol.Trim <> "" Then AddElement(timespan, "end", endtm)

                    AddElement(polystyle, "color", colri)  '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    AddElement(linestyle, "color", colri)
                    polygon = AddElement(placemark, "Polygon", Nothing)
                    AddElement(polygon, "extrude", "1")
                    AddElement(polygon, "altitudeMode", "relativeToGround")
                    outerb = AddElement(polygon, "outerBoundaryIs", Nothing)
                    linearring = AddElement(outerb, "LinearRing", Nothing)
                    'circle around point
                    'Dim coordinates As String = String.Empty
                    'coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit)
                    If latitudecol = "POINT" Then
                        'Dim ln, lt As String
                        'ln = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        'lt = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)
                        'coordinates = CoordinatesCircle(perc * mult, ln, lt, altit, gglmap)

                        p1 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(longitudecol).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        'coordinates = CoordinatesCircle(perc * mult, ln, lt, altit, gglmap)
                        If latlon = "True" Then
                            coordinates = CoordinatesCircle(perc * mult, p1, p2, altit, gglmap)
                        Else
                            coordinates = CoordinatesCircle(perc * mult, p2, p1, altit, gglmap)
                        End If

                    ElseIf longitudecol = latitudecol Then  'address
                        coor = CoordinatesGeocoding(dt.Rows(i)(longitudecol).ToString, altit, gglmap, rangecol)
                        If coor.Trim <> "" AndAlso Not coor.ToUpper.Trim.StartsWith("ERROR!!") Then
                            coordinates = CoordinatesCircle(perc * mult, Piece(coor, ",", 1), Piece(coor, ",", 2), altit, gglmap)
                        End If
                    Else
                        coordinates = CoordinatesCircle(perc * mult, dt.Rows(i)(longitudecol).ToString, dt.Rows(i)(latitudecol).ToString, altit, gglmap)
                    End If
                    AddElement(linearring, "coordinates", coordinates)
                Next
            End If
            'correct <Document ...
            doc.FirstChild.InnerXml = doc.FirstChild.InnerXml.Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            doc.Save(expfile)

            ''correct <Document ...
            'Dim sr() As String = File.ReadAllLines(expfile)
            'If sr(1).Trim.StartsWith("<Document ") Then
            '    sr(1) = sr(1).Replace(" xmlns=""http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition""", "")
            'End If
            ''write in file
            'File.WriteAllLines(expfile, sr)

            ret = expfile
            Return ret
        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function

    'Public Function GenerateMapReport(ByVal rep As String, ByVal repttl As String, ByVal expfile As String, ByVal maptype As String, ByVal dt As DataTable, ByVal starttimecol As String, ByVal endtimecol As String, ByVal longitudecol As String, ByVal latitudecol As String, ByVal altitudecol As String, ByVal headingcol As String, ByVal tiltcol As String, ByVal rangecol As String, ByVal namecol As String, ByVal descriptioncol As String) As String
    '    'NOT IN USE, keep as sample
    '    Dim ret As String = String.Empty
    '    Dim txtline As String = String.Empty
    '    Dim m, i, j As Integer
    '    If dt Is Nothing Then
    '        Return ret
    '        Exit Function
    '    End If
    '    Dim MyFile As StreamWriter = New StreamWriter(expfile)
    '    Try
    '        'beginning of kml
    '        MyFile.WriteLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
    '        MyFile.WriteLine("<kml xmlns=""http://www.opengis.net/kml/2.2"">")
    '        MyFile.WriteLine("<Document>")
    '        txtline = "<name>" & repttl & "</name>"
    '        MyFile.WriteLine(txtline)
    '        MyFile.WriteLine("<open>1</open>")
    '        txtline = "<description>" & rep & "</description>"
    '        MyFile.WriteLine(txtline)

    '        'txtline = "<Style id = ""downArrowIcon"">"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<IconStyle>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<Icon>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<href>http://maps.google.com/mapfiles/kml/pal4/icon28.png</href>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</Icon>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</IconStyle>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</Style>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<Style id = ""globeIcon"">"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<IconStyle>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<Icon>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<href>http://maps.google.com/mapfiles/kml/pal3/icon19.png</href>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</Icon>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</IconStyle>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<LineStyle>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "<width>2</width>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</LineStyle>"
    '        'MyFile.WriteLine(txtline)
    '        'txtline = "</Style>"
    '        'MyFile.WriteLine(txtline)

    '        txtline = "<Folder>"
    '        MyFile.WriteLine(txtline)
    '        txtline = "<name>Placemarks</name>"
    '        MyFile.WriteLine(txtline)
    '        For i = 0 To dt.Rows.Count - 1
    '            txtline = "<Placemark>"
    '            MyFile.WriteLine(txtline)
    '            If maptype = "Simple placemark" Then

    '                txtline = "<name>" & dt.Rows(i)(namecol).ToString & "</name>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<description>" & dt.Rows(i)(descriptioncol).ToString & "</description>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<Point>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<coordinates>" & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",0</coordinates>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "</Point>"
    '                MyFile.WriteLine(txtline)

    '            ElseIf maptype = "Floating placemark" Then
    '                txtline = "<name>" & dt.Rows(i)(namecol).ToString & "</name>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<visibility>0</visibility>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<description>" & dt.Rows(i)(descriptioncol).ToString & "</description>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<LookAt>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<altitude>0</altitude>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "</LookAt>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<Point>"
    '                MyFile.WriteLine(txtline)

    '                'txtline = "<styleUrl>#downArrowIcon</styleUrl>"
    '                'MyFile.WriteLine(txtline)

    '                txtline = "<altitudeMode>relativeToGround</altitudeMode>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<coordinates>" & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",50</coordinates>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "</Point>"
    '                MyFile.WriteLine(txtline)

    '            ElseIf maptype = "Extruded placemark" Then
    '                txtline = "<name>" & dt.Rows(i)(namecol).ToString & "</name>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<visibility>0</visibility>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<description>" & dt.Rows(i)(descriptioncol).ToString & "</description>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<LookAt>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<altitude>0</altitude>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "</LookAt>"
    '                MyFile.WriteLine(txtline)

    '                'txtline = "<styleUrl>#globeIcon</styleUrl>"
    '                'MyFile.WriteLine(txtline)

    '                txtline = "<Point>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<extrude>1</extrude>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<altitudeMode>relativeToGround</altitudeMode>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "<coordinates>" & dt.Rows(i)(longitudecol).ToString & "," & dt.Rows(i)(latitudecol).ToString & ",50</coordinates>"
    '                MyFile.WriteLine(txtline)

    '                txtline = "</Point>"
    '                MyFile.WriteLine(txtline)

    '            ElseIf maptype = "" Then
    '            End If
    '            txtline = "</Placemark>"
    '            MyFile.WriteLine(txtline)
    '        Next

    '        'end of kml 
    '        MyFile.WriteLine("</Folder>")
    '        MyFile.WriteLine("</Document>")
    '        MyFile.WriteLine("</kml>")
    '        txtline = ""
    '        MyFile.WriteLine(txtline)
    '        ret = expfile
    '    Catch ex As Exception
    '        ret = ret & ", line: " & txtline & " - " & ex.Message
    '    End Try
    '    MyFile.Flush()
    '    MyFile.Close()
    '    MyFile = Nothing
    '    Return ret
    'End Function
    Public Function ConvertGeoToDDformat(ByVal coor As String) As String
        Dim ret As String = String.Empty
        'If coor.Contains(ChrW(&HC2) & ChrW(&HB0)) Then  'has degree character
        If coor.Contains(ChrW(&HB0)) Then  'has degree character
            Try
                Dim sp() As String = coor.Split(ChrW(&HB0))
                Dim part1 As String = sp(0)
                Dim parts() As String = sp(1).Split("'")
                Dim coord As Decimal = CInt(part1) + CInt(parts(0)) / 60
                If parts(1).Trim.ToUpper = "S" OrElse parts(1).Trim.ToUpper = "W" Then
                    coord = -coord
                End If
                ret = coord.ToString
            Catch ex As Exception
                ret = "ERROR!! " & ex.Message
            End Try
        Else
            Dim cor As String = coor.Replace("N", "").Replace("S", "").Replace("W", "").Replace("E", "")
            If IsNumeric(cor) Then
                If coor.ToUpper.EndsWith("S") OrElse coor.ToUpper.EndsWith("W") Then
                    cor = -cor
                    ret = cor.ToString
                End If
            ElseIf cor.Trim.Contains(" ") Then
                Dim parts() As String = cor.Split(" ")
                Dim coord As Decimal = CInt(parts(0)) + CInt(parts(1)) / 60
                If coor.ToUpper.EndsWith("S") OrElse coor.ToUpper.EndsWith("W") Then
                    coord = -coord
                    ret = coord.ToString
                End If
            ElseIf cor.Trim.Contains(":") Then
                Dim parts() As String = cor.Split(" ")
                Dim coord As Decimal = CInt(parts(0)) + CInt(parts(1)) / 60 + CInt(parts(2)) / 60
                If coor.ToUpper.EndsWith("S") OrElse coor.ToUpper.EndsWith("W") Then
                    coord = -coord
                    ret = coord.ToString
                End If
            End If
        End If
        Return ret
    End Function
End Module




