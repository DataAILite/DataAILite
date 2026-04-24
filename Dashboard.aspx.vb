Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Math
Partial Class Dashboard
    Inherits System.Web.UI.Page
    Public arrs(11) As String
    Public ttls(11) As String
    Public srts(11) As String
    Public y1 As String
    Public fn As String
    Public x1 As String
    Public x2 As String
    Dim y1s(11) As String
    Dim y2s(11) As String
    Dim fns(11) As String
    Dim x1s(11) As String
    Dim x2s(11) As String
    Dim mx(11) As String
    Dim mf(11) As String
    Dim mv(11) As String
    Dim reps(11) As String
    Public charttypes(11) As String
    Public chartwidth As String
    Public chartheght As String
    Public chartpckgs(11) As String
    Public chartregns(11) As String
    Public chartp1 As String
    Public chartp2 As String
    Public chartp3 As String
    Public chartp4 As String
    Public nrec As Integer
    Dim mapname As String
    Dim mapyesno As String
    Dim mapnames(11) As String
    Dim mapyesnos(11) As String
    Dim arr As String
    Dim ttl As String
    Dim srt As String
    Dim charttype As String
    Public chartpckg As String
    Dim chartregn As String
    Dim i As Integer
    Dim inds(11) As Integer
    Public dashboardname As String
    Public repttls(11) As String
    Public repids(11) As String
    Dim wherestms(11) As String
    Private Sub Dashboard_Init(sender As Object, e As EventArgs) Handles Me.Init
       If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = ""  Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If

        Session("arr") = ""
        Session("ttl") = ""
        Session("y1") = ""
        Session("srt") = ""
        Session("Aggregate") = ""
        Session("dv3") = ""
        If Session("admin") = "user" Then
            LabelShare.Visible = False
            txtShareEmail.Visible = False
            txtShareEmail.Enabled = False
            btnShare.Visible = False
            btnShare.Enabled = False
            lnkbtn_del_0.Enabled = False
            lnkbtn_del_0.Visible = False
            lnkbtn_del_1.Enabled = False
            lnkbtn_del_1.Visible = False
            lnkbtn_del_2.Enabled = False
            lnkbtn_del_2.Visible = False
            lnkbtn_del_3.Enabled = False
            lnkbtn_del_3.Visible = False
            lnkbtn_del_4.Enabled = False
            lnkbtn_del_4.Visible = False
            lnkbtn_del_5.Enabled = False
            lnkbtn_del_5.Visible = False
            lnkbtn_del_6.Enabled = False
            lnkbtn_del_6.Visible = False
            lnkbtn_del_7.Enabled = False
            lnkbtn_del_7.Visible = False
            lnkbtn_del_8.Enabled = False
            lnkbtn_del_8.Visible = False
            lnkbtn_del_9.Enabled = False
            lnkbtn_del_9.Visible = False
            lnkbtn_del_10.Enabled = False
            lnkbtn_del_10.Visible = False
            lnkbtn_del_11.Enabled = False
            lnkbtn_del_11.Visible = False
        ElseIf Session("admin") = "admin" OrElse Session("admin") = "super" OrElse Session("AdvancedUser") = True Then
            LabelShare.Visible = True
            txtShareEmail.Visible = True
            txtShareEmail.Enabled = True
            btnShare.Visible = True
            btnShare.Enabled = True
            lnkbtn_del_0.Enabled = True
            lnkbtn_del_0.Visible = True
            lnkbtn_del_1.Enabled = True
            lnkbtn_del_1.Visible = True
            lnkbtn_del_2.Enabled = True
            lnkbtn_del_2.Visible = True
            lnkbtn_del_3.Enabled = True
            lnkbtn_del_3.Visible = True
            lnkbtn_del_4.Enabled = True
            lnkbtn_del_4.Visible = True
            lnkbtn_del_5.Enabled = True
            lnkbtn_del_5.Visible = True
            lnkbtn_del_6.Enabled = True
            lnkbtn_del_6.Visible = True
            lnkbtn_del_7.Enabled = True
            lnkbtn_del_7.Visible = True
            lnkbtn_del_8.Enabled = True
            lnkbtn_del_8.Visible = True
            lnkbtn_del_9.Enabled = True
            lnkbtn_del_9.Visible = True
            lnkbtn_del_10.Enabled = True
            lnkbtn_del_10.Visible = True
            lnkbtn_del_11.Enabled = True
            lnkbtn_del_11.Visible = True
        End If
    End Sub

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session("newarr") = "yes"
        LabelError.Text = ""
            Dim ret As String = String.Empty
            If Request("Dashboard") Is Nothing OrElse Request("Dashboard").ToString.Trim = "" Then
                LabelError.Text = "No dashboard"
                Exit Sub
            End If
        Dim ddb As DataView = Nothing
        If Request("dash") Is Nothing OrElse Request("dash").ToString.Trim <> "yes" Then
            ddb = mRecords("SELECT * FROM ourdashboards WHERE UserId='" & Session("logon") & "' AND Dashboard='" & Request("Dashboard") & "'", ret)
            dashboardname = Request("Dashboard")
            Session("dash") = ""
        ElseIf Not Request("dash") Is Nothing AndAlso Request("dash").ToString.Trim = "yes" Then
            ddb = mRecords("SELECT * FROM ourdashboards WHERE Prop6='" & Session("logon") & "' AND Dashboard='" & Request("dashboard") & "'", ret)
            dashboardname = Request("dashboard")
            Session("dash") = Request("dash")
        End If
        If ret.Trim <> "" OrElse ddb Is Nothing OrElse ddb.Table.Rows.Count = 0 Then
            LabelError.Text = ret & " - no data"
            Exit Sub
        End If

        LabelWhere.Text = dashboardname
        Session("UserDash") = dashboardname

        If IsPostBack = False Then

            Session("nrec") = ddb.Table.Rows.Count
            nrec = ddb.Table.Rows.Count

            Dim i As Integer
            If ddb.Table.Rows.Count < 12 Then
                For i = ddb.Table.Rows.Count To 11
                    'hide div and btnlinks 
                    'Dim divname As String = "chart_div_" + i.ToString
                    Dim cntrl As HtmlControl = Page.FindControl("chart_div_" + i.ToString)
                    cntrl.Visible = False
                    Dim cntrlnk As System.Web.UI.WebControls.LinkButton = Page.FindControl("lnkbtn_" + i.ToString)
                    cntrlnk.Visible = False
                    cntrlnk.Enabled = False
                    cntrlnk = Page.FindControl("lnkbtn_del_" + i.ToString)
                    cntrlnk.Visible = False
                    cntrlnk.Enabled = False

                    chartpckgs(i) = "corechart"
                    charttypes(i) = "PieChart"
                    repttls(i) = ""
                    repids(i) = ""
                    wherestms(i) = ""
                Next
            End If

            Dim l As Integer
            Dim dashname As String = String.Empty
            Dim repid As String = String.Empty
            Dim x1 As String = String.Empty
            Dim x2 As String = String.Empty
            Dim yM As String = String.Empty
            Dim xM As String = String.Empty
            Dim mfld As String = String.Empty
            Dim vM As String = String.Empty
            Dim dri As DataTable
            Dim sqlq As String = String.Empty
            Dim dt As DataTable = Nothing
            'loop through dashboard reports
            For l = 0 To ddb.Table.Rows.Count - 1
                Try
                    'array  arr, etc...
                    arr = ""
                    ttl = ""
                    y1 = ""
                    x1 = ""
                    x2 = ""
                    fn = ""
                    srt = ""
                    chartregn = "world"
                    mapname = ""
                    mapyesno = ""
                    sqlq = ""
                    mapname = ddb.Table.Rows(l)("MapName").ToString
                    mapyesno = ddb.Table.Rows(l)("MapYesNo").ToString.Trim
                    dashname = ddb.Table.Rows(l)("Dashboard").ToString
                    repid = ddb.Table.Rows(l)("ReportID").ToString
                    charttype = ddb.Table.Rows(l)("ChartType").ToString.Trim
                    inds(l) = ddb.Table.Rows(l)("Indx")
                    reps(l) = repid

                    dri = GetReportInfo(repid)
                    If dri Is Nothing OrElse dri.Rows.Count = 0 Then
                        Continue For
                    End If

                    If dri.Rows(0)("Param7type").ToString.Trim = "standard" Then
                        lnkbtn_del_0.Enabled = False
                        lnkbtn_del_0.Visible = False
                        lnkbtn_del_1.Enabled = False
                        lnkbtn_del_1.Visible = False
                        lnkbtn_del_2.Enabled = False
                        lnkbtn_del_2.Visible = False
                        lnkbtn_del_3.Enabled = False
                        lnkbtn_del_3.Visible = False
                        lnkbtn_del_4.Enabled = False
                        lnkbtn_del_4.Visible = False
                        lnkbtn_del_5.Enabled = False
                        lnkbtn_del_5.Visible = False
                        lnkbtn_del_6.Enabled = False
                        lnkbtn_del_6.Visible = False
                        lnkbtn_del_7.Enabled = False
                        lnkbtn_del_7.Visible = False
                        lnkbtn_del_8.Enabled = False
                        lnkbtn_del_8.Visible = False
                        lnkbtn_del_9.Enabled = False
                        lnkbtn_del_9.Visible = False
                        lnkbtn_del_10.Enabled = False
                        lnkbtn_del_10.Visible = False
                        lnkbtn_del_11.Enabled = False
                        lnkbtn_del_11.Visible = False
                    End If

                    repids(l) = repid
                    repttls(l) = dri.Rows(0)("ReportTtl").ToString
                    wherestms(l) = ddb.Table.Rows(l)("WhereStm").ToString.Trim
                    y1 = ddb.Table.Rows(l)("y1").ToString.Trim
                    x1 = ddb.Table.Rows(l)("x1").ToString.Trim
                    x2 = ddb.Table.Rows(l)("x2").ToString.Trim
                    fn = ddb.Table.Rows(l)("fn1").ToString.Trim

                    yM = ddb.Table.Rows(l)("y2").ToString.Trim
                    xM = ddb.Table.Rows(l)("Prop3").ToString.Trim
                    mfld = ddb.Table.Rows(l)("Prop4").ToString.Trim
                    vM = ddb.Table.Rows(l)("Prop5").ToString.Trim

                    If mapyesno = "yes" Then
                        ttl = mapname
                    Else
                        If yM = "" And xM = "" And vM = "" Then  'not multi
                            If x1 = x2 Then
                                srt = x1
                            Else
                                srt = x1 & ", " & x2
                            End If
                            'chart titles
                            If fn = "Value" Then
                                ttl = "Value of [" & y1 & "] in group by [" & srt & "]"
                            ElseIf fn = "Count" Then
                                ttl = "Count of records in group by [" & srt & "]"
                            ElseIf fn = "CountDistinct" Then
                                ttl = "Distinct Count of [" & y1 & "] in group by [" & srt & "]"
                            ElseIf fn = "Sum" Then
                                ttl = "Sum of [" & y1 & "] in group by [" & srt & "]"
                            ElseIf fn = "Avg" Then
                                ttl = "Avg of [" & y1 & "] in group by [" & srt & "]"
                            ElseIf fn = "StDev" Then
                                ttl = "StDev of [" & y1 & "] in group by [" & srt & "]"
                            ElseIf fn = "Max" Then
                                ttl = "Max of [" & y1 & "] in group by [" & srt & "]"
                            ElseIf fn = "Min" Then
                                ttl = "Min of [" & y1 & "] in group by [" & srt & "]"
                            End If
                        Else  'multi
                            'chart titles
                            If yM = "" Then yM = y1
                            If fn = "Value" Then
                                ttl = "Value of [" & yM & "] in group by [" & xM & "]"
                            ElseIf fn = "Count" Then
                                ttl = "Count of records in group by [" & xM & "]"
                            ElseIf fn = "CountDistinct" Then
                                ttl = "Distinct Count of [" & yM & "] in group by [" & xM & "]"
                            ElseIf fn = "Sum" Then
                                ttl = "Sum of [" & yM & "] in group by [" & xM & "]"
                            ElseIf fn = "Avg" Then
                                ttl = "Avg of [" & yM & "] in group by [" & xM & "]"
                            ElseIf fn = "StDev" Then
                                ttl = "StDev of [" & yM & "] in group by [" & xM & "]"
                            ElseIf fn = "Max" Then
                                ttl = "Max of [" & yM & "] in group by [" & xM & "]"
                            ElseIf fn = "Min" Then
                                ttl = "Min of [" & yM & "] in group by [" & xM & "]"
                            End If
                            If vM <> "" AndAlso mfld <> "" Then
                                ttl = ttl & " for value of " & mfld & "  in " & vM
                            End If
                        End If
                    End If
                    arrs(l) = ""
                    If Not IsDBNull(ddb.Table.Rows(l)("ARR")) AndAlso ddb.Table.Rows(l)("ARR").ToString.Trim <> "" Then
                        arrs(l) = ddb.Table.Rows(l)("ARR").ToString.Replace("^^", "'").Replace("**", "[").Replace("##", "]")
                        chartregn = ddb.Table.Rows(l)("Prop2").ToString
                        If chartregn.Trim = "" Then
                            chartregn = "world"
                        End If
                    Else
                        'update Prop1 in ourdashboards - update start
                        sqlq = "UPDATE ourdashboards SET Prop1='0' WHERE Indx=" & inds(l)
                        ret = ExequteSQLquery(sqlq)
                        Dim er As String = String.Empty
                        arrs(l) = UpdateDashboardARR(dashname, repid, charttype, mapname, mapyesno, x1, x2, y1, yM, fn, "", wherestms(l), "0", xM, mfld, vM, inds(l), chartregn, Session("UserConnString"), Session("UserConnProvider"), er)
                        If arrs(l).Trim <> "" Then
                            'update ARR in ourdashboards - completed
                            sqlq = "UPDATE ourdashboards SET Prop2='" & chartregn & "', ARR='" & arrs(l).Replace("'", "^^").Replace("[", "**").Replace("]", "##") & "' WHERE Indx=" & inds(l)
                            ret = ExequteSQLquery(sqlq)

                            'update Prop1 in ourdashboards - completed
                            sqlq = "UPDATE ourdashboards SET Prop1='1' WHERE Indx=" & inds(l)
                            ret = ExequteSQLquery(sqlq)
                        End If
                    End If

                    If charttype = "MapChart" OrElse charttype = "Map" Then
                        chartpckg = "map"
                    ElseIf charttype = "GeoChart" Then
                        chartpckg = "geochart"
                    ElseIf charttype = "Gauge" Then
                        chartpckg = "gauge"
                    ElseIf charttype = "Sankey" Then
                        chartpckg = "sankey"
                    Else
                        chartpckg = "corechart"
                    End If

                    y1s(l) = y1
                    fns(l) = fn
                    x1s(l) = x1
                    x2s(l) = x2
                    y2s(l) = yM
                    mx(l) = xM
                    mf(l) = mfld
                    mv(l) = vM
                    mapyesnos(l) = mapyesno
                    mapnames(l) = mapname
                    ttls(l) = ttl
                    If wherestms(l).Trim <> "" Then
                        ttls(l) = ttls(l) & " - " & wherestms(l).Replace("^", "").Replace("'", "").Replace("[", "").Replace("]", "")
                    End If
                    srts(l) = srt
                    If charttype = "MapChart" Then
                        charttype = "Map"
                    End If
                    charttypes(l) = charttype
                    chartpckgs(l) = chartpckg
                    chartregns(l) = chartregn
                Catch ex As Exception
                    ret = ex.Message
                    y1s(l) = y1
                    fns(l) = fn
                    x1s(l) = x1
                    x2s(l) = x2
                    mapyesnos(l) = mapyesno
                    mapnames(l) = mapname
                    arrs(l) = ""
                    ttls(l) = ttl
                    srts(l) = srt
                    charttypes(l) = charttype
                    chartpckgs(l) = chartpckg
                    chartregns(l) = chartregn
                End Try
            Next
            LabelWhere.Text = dashname '& " ___     " & Session("WhereStm").ToString.Trim

            lnkbtn_0.ToolTip = "Maximize the chart " & charttypes(0) & " -" & ttls(0) & " - report " & repttls(0) & " (" & repids(0) & ")"
            lnkbtn_del_0.ToolTip = "Delete the chart " & ttls(0) & " from the dashboard"
            lnkbtn_1.ToolTip = "Maximize the chart " & charttypes(1) & " -" & ttls(1) & " - report " & repttls(1) & " (" & repids(1) & ")"
            lnkbtn_del_1.ToolTip = "Delete the chart " & ttls(1) & " from the dashboard"
            lnkbtn_2.ToolTip = "Maximize the chart " & charttypes(2) & " -" & ttls(2) & " - report " & repttls(2) & " (" & repids(2) & ")"
            lnkbtn_del_2.ToolTip = "Delete the chart " & ttls(2) & " from the dashboard"
            lnkbtn_3.ToolTip = "Maximize the chart " & charttypes(3) & " -" & ttls(3) & " - report " & repttls(3) & " (" & repids(3) & ")"
            lnkbtn_del_3.ToolTip = "Delete the chart " & ttls(3) & " from the dashboard"
            lnkbtn_4.ToolTip = "Maximize the chart " & charttypes(4) & " -" & ttls(4) & " - report " & repttls(4) & " (" & repids(4) & ")"
            lnkbtn_del_4.ToolTip = "Delete the chart " & ttls(4) & " from the dashboard"
            lnkbtn_5.ToolTip = "Maximize the chart " & charttypes(5) & " -" & ttls(5) & " - report " & repttls(5) & " (" & repids(5) & ")"
            lnkbtn_del_5.ToolTip = "Delete the chart " & ttls(5) & " from the dashboard"
            lnkbtn_6.ToolTip = "Maximize the chart " & charttypes(6) & " -" & ttls(6) & " - report " & repttls(6) & " (" & repids(6) & ")"
            lnkbtn_del_6.ToolTip = "Delete the chart " & ttls(6) & " from the dashboard"
            lnkbtn_7.ToolTip = "Maximize the chart " & charttypes(7) & " -" & ttls(7) & " - report " & repttls(7) & " (" & repids(7) & ")"
            lnkbtn_del_7.ToolTip = "Delete the chart " & ttls(7) & " from the dashboard"
            lnkbtn_8.ToolTip = "Maximize the chart " & charttypes(8) & " -" & ttls(8) & " - report " & repttls(8) & " (" & repids(8) & ")"
            lnkbtn_del_8.ToolTip = "Delete the chart " & ttls(8) & " from the dashboard"
            lnkbtn_9.ToolTip = "Maximize the chart " & charttypes(9) & " -" & ttls(9) & " - report " & repttls(9) & " (" & repids(9) & ")"
            lnkbtn_del_9.ToolTip = "Delete the chart " & ttls(9) & " from the dashboard"
            lnkbtn_10.ToolTip = "Maximize the chart " & charttypes(10) & " -" & ttls(10) & " - report " & repttls(10) & " (" & repids(10) & ")"
            lnkbtn_del_10.ToolTip = "Delete the chart " & ttls(10) & " from the dashboard"
            lnkbtn_11.ToolTip = "Maximize the chart " & charttypes(11) & " -" & ttls(11) & " - report " & repttls(11) & " (" & repids(11) & ")"
            lnkbtn_del_11.ToolTip = "Delete the chart " & ttls(11) & " from the dashboard"

            'save in Session
            Session("nrec") = nrec
            Session("y1s") = y1s
            Session("y2s") = y2s
            Session("mx") = mx
            Session("mf") = mf
            Session("mv") = mv
            Session("fns") = fns
            Session("x1s") = x1s
            Session("x2s") = x2s
            Session("mapyesnos") = mapyesnos
            Session("mapnames") = mapnames
            Session("arrs") = arrs
            Session("ttls") = ttls
            Session("srts") = srts
            Session("charttypes") = charttypes
            Session("chartpckgs") = chartpckgs
            Session("chartregns") = chartregns
            Session("wherestms") = wherestms
            Session("reps") = reps
            Session("inds") = inds
        Else  'IsPostBack
            'get from Session
            nrec = Session("nrec")
            y1s = Session("y1s")
            y2s = Session("y2s")
            mx = Session("mx")
            mf = Session("mf")
            mv = Session("mv")
            fns = Session("fns")
            x1s = Session("x1s")
            x2s = Session("x2s")
            mapyesnos = Session("mapyesnos")
            mapnames = Session("mapnames")
            arrs = Session("arrs")
            ttls = Session("ttls")
            srts = Session("srts")
            charttypes = Session("charttypes")
            chartpckgs = Session("chartpckgs")
            chartregns = Session("chartregns")
            wherestms = Session("wherestms")
            reps = Session("reps")
            inds = Session("inds")
        End If


    End Sub

    Private Sub LinkButtonBack_Click(sender As Object, e As EventArgs) Handles LinkButtonBack.Click
        'TODO from Analytics, from ListOfReports
        Response.Redirect("ListOfDashboards.aspx")

    End Sub

    Private Sub DropDownChartType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownChartType.SelectedIndexChanged
        ' charttype = DropDownChartType.SelectedItem.Text
    End Sub

    Private Sub lnkbtnAddToDashboard_Click(sender As Object, e As EventArgs) Handles lnkbtnAddToDashboard.Click
        'Dim i As Integer
        'Dim ret As String = String.Empty
        'Dim dashnames(0) As String
        'dashnames(0) = "Dashboard " & Session("logon")
        ''TODO open dialog with dashbords dropdown and txtbox to get array of dashnames
        'For i = 0 To dashnames.Length - 1
        '    ret = AddToDashboard(dashnames(i))
        'Next
        'TODO message box with dashboard names
    End Sub
    'Private Function AddToDashboard(ByVal DashBoardName As String) As String
    '    Dim ret As String = String.Empty
    '    Dim sqld As String = String.Empty
    '    Try
    '        sqld = "Select * FROM ourdashboards WHERE "
    '        sqld = sqld & " UserID ='" & Session("logon") & "' "
    '        sqld = sqld & " AND Dashboard='" & DashBoardName & "' "
    '        sqld = sqld & " AND ReportID='" & Session("REPORTID") & "' "
    '        sqld = sqld & " AND ChartType='" & charttype & "' "
    '        sqld = sqld & " AND MapName='" & mapname & "' "
    '        sqld = sqld & " AND x1='" & x1 & "' "
    '        sqld = sqld & " AND x2='" & x2 & "' "
    '        sqld = sqld & " AND y1='" & y1 & "' "
    '        sqld = sqld & " AND fn1='" & fn & "' "
    '        If Session("WhereStm") IsNot Nothing Then
    '            sqld = sqld & " AND WhereStm='" & Session("WhereStm").ToString.Trim & "' "
    '        End If
    '        sqld = sqld & " AND GraphTitle='" & ttl & "' "
    '        sqld = sqld & " AND MapYesNo='" & mapyesno & "' "
    '        If Not HasRecords(sqld) Then
    '            'insert
    '            sqld = "INSERT INTO ourdashboards SET "
    '            sqld = sqld & " UserID ='" & Session("logon") & "' "
    '            sqld = sqld & " , Dashboard='" & DashBoardName & "' "
    '            sqld = sqld & " , ReportID='" & Session("REPORTID") & "' "
    '            sqld = sqld & " , ChartType='" & charttype & "' "
    '            sqld = sqld & " , MapName='" & mapname & "' "
    '            sqld = sqld & " , x1='" & x1 & "' "
    '            sqld = sqld & " , x2='" & x2 & "' "
    '            sqld = sqld & " , y1='" & y1 & "' "
    '            sqld = sqld & " , fn1='" & fn & "' "
    '            If Session("WhereStm") IsNot Nothing Then
    '                sqld = sqld & " , WhereStm='" & Session("WhereStm").ToString.Trim & "' "
    '            End If
    '            sqld = sqld & " , GraphTitle='" & ttl & "' "
    '            sqld = sqld & " , MapYesNo='" & mapyesno & "' "
    '            ret = ExequteSQLquery(sqld)
    '        End If
    '    Catch ex As Exception
    '        ret = "ERROR!! " & ex.Message
    '    End Try
    '    Return ret
    'End Function
    Private Sub lnkbtn_0_Click(sender As Object, e As EventArgs) Handles lnkbtn_0.Click
        i = 0
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_1_Click(sender As Object, e As EventArgs) Handles lnkbtn_1.Click

        i = 1
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_2_Click(sender As Object, e As EventArgs) Handles lnkbtn_2.Click

        i = 2
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_3_Click(sender As Object, e As EventArgs) Handles lnkbtn_3.Click

        i = 3
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_4_Click(sender As Object, e As EventArgs) Handles lnkbtn_4.Click

        i = 4
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_5_Click(sender As Object, e As EventArgs) Handles lnkbtn_5.Click

        i = 5
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_6_Click(sender As Object, e As EventArgs) Handles lnkbtn_6.Click

        i = 6
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_7_Click(sender As Object, e As EventArgs) Handles lnkbtn_7.Click

        i = 7
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_8_Click(sender As Object, e As EventArgs) Handles lnkbtn_8.Click

        i = 8
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_9_Click(sender As Object, e As EventArgs) Handles lnkbtn_9.Click

        i = 9
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_10_Click(sender As Object, e As EventArgs) Handles lnkbtn_10.Click

        i = 10
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub
    Private Sub lnkbtn_11_Click(sender As Object, e As EventArgs) Handles lnkbtn_11.Click

        i = 11
        Session("arr") = arrs(i)
        Dim url = "ChartGoogleOne.aspx?das=yes&map=" & mapyesnos(i) & "&mapname=" & mapnames(i) & "&chartregn=" & chartregns(i) & "&Report=" & reps(i) & "&y1=" & y1s(i) & "&y2=" & y2s(i) & "&fn=" & fns(i) & "&x1=" & x1s(i) & "&x2=" & x2s(i) & "&mx=" & mx(i) & "&mf=" & mf(i) & "&mv=" & mv(i) & "&charttype=" & charttypes(i) & "&ttl=" & ttls(i) & "&srt=" & srts(i) & "&wherestm=" & wherestms(i)
        Response.Redirect(url)
    End Sub

    Private Sub lnkbtn_del_0_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_0.Click
        Dim ret As String = DeleteDashboard(inds(0))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_1_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_1.Click
        Dim ret As String = DeleteDashboard(inds(1))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_2_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_2.Click
        Dim ret As String = DeleteDashboard(inds(2))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_3_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_3.Click
        Dim ret As String = DeleteDashboard(inds(3))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_4_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_4.Click
        Dim ret As String = DeleteDashboard(inds(4))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_5_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_5.Click
        Dim ret As String = DeleteDashboard(inds(5))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_6_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_6.Click
        Dim ret As String = DeleteDashboard(inds(6))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_7_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_7.Click
        Dim ret As String = DeleteDashboard(inds(7))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_8_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_8.Click
        Dim ret As String = DeleteDashboard(inds(8))
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_9_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_9.Click
        Dim ret As String = DeleteDashboard(inds(9))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_10_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_10.Click
        Dim ret As String = DeleteDashboard(inds(10))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Sub lnkbtn_del_11_Click(sender As Object, e As EventArgs) Handles lnkbtn_del_11.Click
        Dim ret As String = DeleteDashboard(inds(11))
        If ret.StartsWith("ERROR!!") Then
            MessageBox.Show("Error!! Report has not been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Else
            MessageBox.Show("Report has been deleted from the dashboard.", "Dashboards", "Dashboards", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        End If
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
    Private Function DeleteDashboard(ByVal ind As Integer) As String
        Dim sqld As String = "DELETE FROM ourdashboards WHERE Indx=" & ind.ToString
        Dim ret = ExequteSQLquery(sqld)
        If ret <> "Query executed fine." Then
            ret = "ERROR!! " & ret
        Else
            sqld = "Select * From ourdashboards Where UserId='" & Session("logon") & "' AND Dashboard='" & dashboardname & "'"
            Dim dt As DataTable = mRecords(sqld).Table
            If dt.TableName IsNot Nothing AndAlso dt.Rows.Count = 0 Then _
                Response.Redirect("ListOfDashboards.aspx")
        End If
        Return ret
    End Function
    Private Sub btnShare_Click(sender As Object, e As EventArgs) Handles btnShare.Click
        If cleanText(txtShareEmail.Text) <> txtShareEmail.Text.Trim Then
            txtShareEmail.Text = "Illegal character found. Please retype Email."
            Exit Sub
        End If
        Dim lgn As String = GetDashboardIdentifier(dashboardname, Session("logon"))
        Dim lnk As String = ConfigurationManager.AppSettings("weboureports").ToString & "default.aspx?srd=30&dash=yes&lgn=" & lgn
        Dim ret As String = String.Empty
        Dim emailbody As String = String.Empty
        emailbody = "Please visit OUReports.com at " & lnk & " to see the dashboard shared with you. | | Feel free to contact us at ""https://www.oureports.net/OUReports/ContactUs.aspx"" if you have any questions. | OUReports"
        emailbody = emailbody.Replace("|", Chr(10))
        ret = SendHTMLEmail("", "Dashboard is shared with you", emailbody, txtShareEmail.Text, Session("SupportEmail"))
        WriteToAccessLog(Session("logon"), "Sent Email to " & txtShareEmail.Text & " with body " & emailbody & ". Result: " & ret, 1)
        MessageBox.Show(ret, "Dashboard share", "DashboardShared", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub

    Private Sub LinkButtonRefresh_Click(sender As Object, e As EventArgs) Handles LinkButtonRefresh.Click
        Dim n As Integer = 0
        Dim ret As String = String.Empty
        Dim er As String = String.Empty
        ret = UpdateDashboard(dashboardname, n, Session("UserConnString"), Session("UserConnProvider"), er)

        'MessageBox.Show("Dashboard updated with result: " & ret, "Dashboards", "", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Information)
        Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & dashboardname)
    End Sub
End Class
