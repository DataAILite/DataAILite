Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Partial Class ChartGoogle
    Inherits System.Web.UI.Page
    Public arr As String
    Public ttl As String
    Public srt As String
    Public y1 As String
    Public x1 As String
    Public x2 As String
    Public arrCount As String
    Public ttlCount As String
    Public arrDistCount As String
    Public ttlDistCount As String
    Public arrValue As String
    Public ttlValue As String
    Public arrSum As String
    Public ttlSum As String
    Public arrAvg As String
    Public ttlAvg As String
    Public arrStDev As String
    Public ttlStDev As String
    Public arrMax As String
    Public ttlMax As String
    Public arrMin As String
    Public ttlMin As String
    Public charttype As String
    Public chartpckg As String
    Public nv As Integer

    Private Sub ChartGoogle_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        LinkButtonBack.OnClientClick = "showSpinner();return true;"
        lnkbtnAvg.OnClientClick = "showSpinner();return true;"
        lnkbtnCount.OnClientClick = "showSpinner();return true;"
        lnkbtnDistCount.OnClientClick = "showSpinner();return true;"
        lnkbtnMax.OnClientClick = "showSpinner();return true;"
        lnkbtnMin.OnClientClick = "showSpinner();return true;"
        lnkbtnStDev.OnClientClick = "showSpinner();return true;"
        lnkbtnSum.OnClientClick = "showSpinner();return true;"
        lnkbtnValue.OnClientClick = "showSpinner();return true;"
        DropDownChartType.Attributes.Add("onchange", "showSpinner();")
        nv = 8
        If Not Session("nv") Is Nothing And IsNumeric(Session("nv")) Then
            nv = Session("nv")
        End If
        Session("StatDash") = "yes"
        charttype = DropDownChartType.Text
        If charttype = "MapChart" OrElse charttype = "Map" Then
            chartpckg = "Map"
        ElseIf charttype = "GeoChart" Then
            chartpckg = "geochart"
        ElseIf charttype = "Gauge" Then
            chartpckg = "gauge"
        ElseIf charttype = "Sankey" Then
            chartpckg = "sankey"
        Else
            chartpckg = "corechart"
        End If
        Session("arr") = ""
        arr = "['Mushrooms', 2],"
        arr = arr & "['Onions', 2],"
        arr = arr & "['Olives', 2],"
        arr = arr & "['Zucchini', 0],"
        arr = arr & "['Pepperoni', 3]"
        ttl = "How Much Pizza Anthony Ate Last Night"
        If Not IsPostBack AndAlso Request("map") IsNot Nothing AndAlso Request("map").ToString.Trim = "yes" Then
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("Map")
            DropDownChartType.Items.Add("GeoChart")
        Else
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("PieChart")
            DropDownChartType.Items.Add("BarChart")
            DropDownChartType.Items.Add("LineChart")
            DropDownChartType.Items.Add("AreaChart")
            DropDownChartType.Items.Add("SteppedAreaChart")
            DropDownChartType.Items.Add("ScatterChart")
            DropDownChartType.Items.Add("ComboChart")
            'DropDownChartType.Items.Add("BubbleChart")
            DropDownChartType.Items.Add("ColumnChart")
            DropDownChartType.Items.Add("Histogram")
            'DropDownChartType.Items.Add("Gauge")
            'DropDownChartType.Items.Add("Sankey")
            'DropDownChartType.Items.Add("CandlestickChart")
            'DropDownChartType.Items.Add("Waterfall")
        End If
        If Request("charttype") IsNot Nothing AndAlso Request("charttype").ToString.Trim <> "" Then
            DropDownChartType.Text = Request("charttype").ToString.Trim
            Session("ChartType") = DropDownChartType.Text
        ElseIf Session("ChartType") IsNot Nothing AndAlso Session("ChartType").ToString.Trim <> "" Then
            DropDownChartType.Text = Session("ChartType")
        End If
        If nv < 8 Then
            Value_chart_div.Visible = False
            Sum_chart_div.Visible = False
            Avg_chart_div.Visible = False
            StDev_chart_div.Visible = False
            Max_chart_div.Visible = False
            Min_chart_div.Visible = False
            lnkbtnValue.Visible = False
            lnkbtnValue.Enabled = False
            lnkbtnSum.Visible = False
            lnkbtnSum.Enabled = False
            lnkbtnAvg.Visible = False
            lnkbtnAvg.Enabled = False
            lnkbtnStDev.Visible = False
            lnkbtnStDev.Enabled = False
            lnkbtnMax.Visible = False
            lnkbtnMax.Enabled = False
            lnkbtnMin.Visible = False
            lnkbtnMin.Enabled = False
        End If
        If nv = 3 Then
            Value_chart_div.Visible = True
            lnkbtnValue.Visible = True
            lnkbtnValue.Enabled = True
        End If
        If Session("WhereStm") Is Nothing Then
            Session("WhereStm") = ""
        End If
    End Sub

    Private Sub ChartGoogle_Load(sender As Object, e As EventArgs) Handles Me.Load
        charttype = DropDownChartType.SelectedItem.Text
        Dim repid As String = String.Empty
        If Request("Report") Is Nothing OrElse Request("Report").ToString.Trim = "" Then
            Exit Sub
        Else
            repid = Request("Report").ToString
        End If
        Session("arr") = ""
        If Not IsPostBack Then
            Session("arrCount") = ""
            Session("arrDistCount") = ""
            Session("arrSum") = ""
            Session("arrAvg") = ""
            Session("arrStDev") = ""
            Session("arrMax") = ""
            Session("arrMin") = ""
            Session("arrValue") = ""
        Else
            arrCount = Session("arrCount")
            arrDistCount = Session("arrDistCount")
            arrSum = Session("arrSum")
            arrAvg = Session("arrAvg")
            arrStDev = Session("arrStDev")
            arrMax = Session("arrMax")
            arrMin = Session("arrMin")
            arrValue = Session("arrValue")
        End If
        Dim ret As String = String.Empty
        Dim dt As DataTable
        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            Exit Sub
        End If
        Try
            Dim sqlq As String = dri.Rows(0)("SQLquerytext").ToString

            LabelWhere.Text = dri.Rows(0)("ReportTtl").ToString & " _____ " & Session("WhereStm").ToString.Trim

            x1 = String.Empty
            x2 = String.Empty
            y1 = String.Empty
            Dim fn As String = String.Empty
            If Request("x1") IsNot Nothing Then
                x1 = Request("x1").ToString
            End If
            If Request("x2") IsNot Nothing Then
                x2 = Request("x2").ToString
            End If
            If Request("y1") IsNot Nothing Then
                y1 = Request("y1").ToString
            End If
            If Request("fn") IsNot Nothing AndAlso Request("fn") <> "Value" Then
                fn = Request("fn").ToString
            End If
            Session("cat1") = x1
            Session("cat2") = x2
            Session("AxisY") = y1
            Session("Aggregate") = fn

            srt = x1 & ", " & x2
            If x1 = x2 Then
                srt = x1
            End If

            Dim er As String = String.Empty
            Dim rt As String = String.Empty
            If x1 <> x2 AndAlso Not IsPostBack Then
                rt = AddGroupBy(Session("REPORTID"), x1, x2, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
            End If

            Dim selflds As String = x1 & "," & x2 & "," & y1
            selflds = FixSelectedFields(repid, selflds, Session("UserConnString"), Session("UserConnProvider"))
            Dim xx1, xx2, yy1, ssrt As String
            xx1 = Piece(selflds, ",", 1)
            xx2 = Piece(selflds, ",", 2)
            yy1 = Piece(selflds, ",", 3)

            ssrt = xx1 & ", " & xx2
            If x1 = x2 Then
                ssrt = xx1
                lnkbtnReverse.Visible = False
                lnkbtnReverse.Enabled = False
            Else
                lnkbtnReverse.Visible = True
                lnkbtnReverse.Enabled = True
            End If

            Dim msql As String = String.Empty
            Dim i As Integer
            Dim grp As String = String.Empty

            arrValue = Session("arrValue")
            ttlValue = "Value of [" & y1 & "] in group by [" & srt & "]"

            arrCount = Session("arrCount")
            ttlCount = "Count of records in group by [" & srt & "]"

            arrDistCount = Session("arrDistCount")
            ttlDistCount = "Distinct Count of [" & y1 & "] in group by [" & srt & "]"

            arrSum = Session("arrSum")
            ttlSum = "Sum of [" & y1 & "] in group by [" & srt & "]"

            arrAvg = Session("arrAvg")
            ttlAvg = "Avg of [" & y1 & "] in group by [" & srt & "]"

            arrStDev = Session("arrStDev")
            ttlStDev = "StDev of [" & y1 & "] in group by [" & srt & "]"

            arrMax = Session("arrMax")
            ttlMax = "Max of [" & y1 & "] in group by [" & srt & "]"

            arrMin = Session("arrMin")
            ttlMin = "Min of [" & y1 & "] in group by [" & srt & "]"

            If Not IsPostBack Then

                Dim dv3 As DataView
                If Session("dv3") Is Nothing Then
                    'retrieve dv3
                    dv3 = RetrieveReportData(repid, Session("WhereStm").ToString, True, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret)
                    If dv3 Is Nothing OrElse dv3.Table.Rows.Count = 0 Then
                        LabelError.Text = "No data"
                        Exit Sub
                    End If
                    Session("dv3") = dv3
                Else
                    dv3 = Session("dv3")
                End If
                If dv3 Is Nothing Then
                    LabelError.Text = ret
                    Exit Sub
                End If

                'calc stats and make arrs
                'Dim dta As DataTable
                For Each fn In {"Count", "DistCount", "Sum", "AVG", "MAX", "MIN", "StDev"}
                    If nv <= 3 AndAlso Not fn.Contains("Count") Then
                        Exit For
                    End If
                    arr = ""
                    Dim dta As New DataTable
                    'dta = ComputeStats(dv3.Table, fn, y1, x1, x2, Session("WhereStm").ToString.Trim, ret, Session("UserConnString"), Session("UserConnProvider"))
                    dta = ComputeStats(dv3.Table, fn, y1, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))

                    If dta Is Nothing Then
                        LabelError.Text = LabelError.Text & " " & ret
                        Continue For
                    End If
                    For i = 0 To dta.Rows.Count - 1
                        If x1 = x2 Then
                            grp = dta.Rows(i)(x1).ToString
                        Else
                            grp = dta.Rows(i)(x1).ToString & "," & dta.Rows(i)(x2).ToString
                        End If
                        arr = arr & "['" & grp & "'," & dta.Rows(i)("ARR").ToString & "]"
                        If i < dta.Rows.Count - 1 Then arr = arr & ","
                    Next
                    If fn = "Count" Then
                        arrCount = arr
                    ElseIf fn = "DistCount" Then
                        arrDistCount = arr
                    ElseIf fn = "Sum" Then
                        arrSum = arr
                    ElseIf fn = "AVG" Then
                        arrAvg = arr
                    ElseIf fn = "MAX" Then
                        arrMax = arr
                    ElseIf fn = "MIN" Then
                        arrMin = arr
                    ElseIf fn = "StDev" Then
                        arrStDev = arr
                    End If
                Next

                dt = dv3.ToTable
                If nv > 2 AndAlso ColumnTypeIsNumeric(dt.Columns(y1)) Then
                    'calc Value and arrValue
                    For i = 0 To dt.Rows.Count - 1
                        If x1 = x2 Then
                            grp = dt.Rows(i)(x1).ToString
                        Else
                            grp = dt.Rows(i)(x1).ToString & "," & dt.Rows(i)(x2).ToString
                        End If
                        arrValue = arrValue & "['" & grp & "'," & dt.Rows(i)(y1).ToString & "]"
                        If i < dt.Rows.Count - 1 Then arrValue = arrValue & ","
                    Next
                End If

                If nv = 2 Then
                    arrSum = ""
                    arrAvg = ""
                    arrStDev = ""
                    arrMax = ""
                    arrMin = ""
                    arrValue = ""
                ElseIf nv = 3 Then
                    arrSum = ""
                    arrAvg = ""
                    arrStDev = ""
                    arrMax = ""
                    arrMin = ""
                End If
                Session("arrCount") = arrCount
                Session("arrDistCount") = arrDistCount
                Session("arrSum") = arrSum
                Session("arrAvg") = arrAvg
                Session("arrStDev") = arrStDev
                Session("arrMax") = arrMax
                Session("arrMin") = arrMin
                Session("arrValue") = arrValue

            End If
        Catch ex As Exception
            ret = ex.Message
            LabelError.Text = ret
            'Session("arrCount") = ""
            'Session("arrDistCount") = ""
            'Session("arrSum") = ""
            'Session("arrAvg") = ""
            'Session("arrStDev") = ""
            'Session("arrMax") = ""
            'Session("arrMin") = ""
            'Session("arrValue") = ""
        End Try


    End Sub

    Private Sub LinkButtonBack_Click(sender As Object, e As EventArgs) Handles LinkButtonBack.Click
        'from Analytics, from ListOfReports
        If Request("frm") = "Analytics" Then
            Response.Redirect("ShowReport.aspx?srd=11&REPORT=" & Session("REPORTID"))
        ElseIf Request("frm") = "ListOfReports" Then
            Response.Redirect("ListOfReports.aspx")

        Else
            Response.Redirect("ReportViews.aspx?Report=" & Session("REPORTID") & "&see=yes")
            'Response.Redirect("ReportViews.aspx?see=yes")
        End If

    End Sub

    Private Sub DropDownChartType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownChartType.SelectedIndexChanged
        charttype = DropDownChartType.Text
        Session("ChartType") = charttype
    End Sub

    Private Sub lnkbtnCount_Click(sender As Object, e As EventArgs) Handles lnkbtnCount.Click
        Session("arr") = arrCount
        Response.Redirect("ChartGoogleOne.aspx?fn=Count&Report=" & Session("REPORTID") & "&ttl=" & ttlCount & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnDistCount_Click(sender As Object, e As EventArgs) Handles lnkbtnDistCount.Click
        Session("arr") = arrDistCount
        Response.Redirect("ChartGoogleOne.aspx?fn=CountDistinct&Report=" & Session("REPORTID") & "&ttl=" & ttlDistCount & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnValue_Click(sender As Object, e As EventArgs) Handles lnkbtnValue.Click
        Session("arr") = arrValue
        Response.Redirect("ChartGoogleOne.aspx?fn=Value&Report=" & Session("REPORTID") & "&ttl=" & ttlValue & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnSum_Click(sender As Object, e As EventArgs) Handles lnkbtnSum.Click
        Session("arr") = arrSum
        Response.Redirect("ChartGoogleOne.aspx?fn=Sum&Report=" & Session("REPORTID") & "&ttl=" & ttlSum & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnAvg_Click(sender As Object, e As EventArgs) Handles lnkbtnAvg.Click
        Session("arr") = arrAvg
        Response.Redirect("ChartGoogleOne.aspx?fn=Avg&Report=" & Session("REPORTID") & "&ttl=" & ttlAvg & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnStDev_Click(sender As Object, e As EventArgs) Handles lnkbtnStDev.Click
        Session("arr") = arrStDev
        Response.Redirect("ChartGoogleOne.aspx?fn=StDev&Report=" & Session("REPORTID") & "&ttl=" & ttlStDev & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnMax_Click(sender As Object, e As EventArgs) Handles lnkbtnMax.Click
        Session("arr") = arrMax
        Response.Redirect("ChartGoogleOne.aspx?fn=Max&Report=" & Session("REPORTID") & "&ttl=" & ttlMax & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnMin_Click(sender As Object, e As EventArgs) Handles lnkbtnMin.Click
        Session("arr") = arrMin
        Response.Redirect("ChartGoogleOne.aspx?fn=Min&Report=" & Session("REPORTID") & "&ttl=" & ttlMin & "&y1=" & y1 & "&srt=" & srt & "&x1=" & x1 & "&x2=" & x2 & "&charttype=" & charttype)
    End Sub

    Private Sub lnkbtnReverse_Click(sender As Object, e As EventArgs) Handles lnkbtnReverse.Click
        Response.Redirect("ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat2") & "&x2=" & Session("cat1") & "&y1=" & Session("AxisY") & "&fn=" & Session("Aggregate") & "&charttype=" & charttype)
    End Sub
End Class
