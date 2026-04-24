Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Math
Partial Class ChartGoogleOne
    Inherits System.Web.UI.Page
    Public dv3 As DataView
    Public arr As String
    Public ttl As String
    Public srt As String
    Public y1 As String
    Public fn As String
    Public x1 As String
    Public x2 As String
    Public charttype As String
    Public charttypeM As String
    Public chartwidth As String
    Public chartheght As String = "850"
    Public chartpckg As String
    Public chartregn As String
    Public chartp1 As String
    Public chartp2 As String
    Public chartp3 As String
    Public chartp4 As String
    Public nrec As Integer
    Public mapname As String
    Public mapyesno As String
    Public wherestm As String
    Public sDashboards As String()
    Private bUseSQLText As Boolean

    Private Sub ChartGoogleOne_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        btnClose.OnClientClick = "closeDialog();return false;"
        btnFind.OnClientClick = "DoSearch('txtSearch');return false;"
        btnSubmit.OnClientClick = "btnSubmitClicked();return false;"
        lnkbtnAddToDashboard.OnClientClick = "addToDashboardClicked();return false;"
        lnkbtnReverse.OnClientClick = "showSpinner();return true;"
        LinkButtonBack.OnClientClick = "showSpinner();return true;"
        spnClose.Attributes.Add("onclick", "closeDialog();")
        txtSearch.Attributes.Add("onkeydown", "txtSearchKeyDown();")
        lstItems.Attributes.Add("onclick", "ChecklistIndexChanged(event);")

        DropDownChartType.Attributes.Add("onchange", "showSpinner();")

        If Session("admin") <> "admin" AndAlso Session("admin") <> "super" Then
            lnkbtnAddToDashboard.Enabled = False
            lnkbtnAddToDashboard.Visible = False
            lnkDownloadARR.Enabled = False
            lnkDownloadARR.Visible = False
        Else
            lnkbtnAddToDashboard.Enabled = True
            lnkbtnAddToDashboard.Visible = True
            lnkDownloadARR.Enabled = True
            lnkDownloadARR.Visible = True
        End If
        'If Not IsPostBack Then
        If Request("map") IsNot Nothing AndAlso Request("map").ToString.Trim = "yes" Then
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("GeoChart")
            DropDownChartType.Items.Add("MapChart")
            lnkbtnReverse.Visible = False
            lnkbtnReverse.Enabled = False

            LabelX.Visible = False
            LabelY.Visible = False
            LabelM.Visible = False
            LabelV.Visible = False
            DropDownColumnsX.Visible = False
            DropDownColumnsY.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            btnShowChart.Enabled = False
            btnShowChart.Visible = False
            LabelA.Visible = False
            DropDownListA.Enabled = False
            DropDownListA.Visible = False

        ElseIf (Not Request("MatrixChart") Is Nothing AndAlso Request("MatrixChart") = "yes") OrElse (Not Session("MatrixChart") Is Nothing AndAlso Session("MatrixChart") = "MatrixChart") Then
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("PieChart")
            DropDownChartType.Items.Add("BarChart")
            DropDownChartType.Items.Add("ColumnChart")
            DropDownChartType.Items.Add("LineChart")
            DropDownChartType.Items.Add("AreaChart")
            DropDownChartType.Items.Add("SteppedAreaChart")
            DropDownChartType.Items.Add("ScatterChart")
            DropDownChartType.Items.Add("ComboChart")
            DropDownChartType.Items.Add("Histogram")
            lnkbtnReverse.Visible = False
            lnkbtnReverse.Enabled = False
            lnkbtnAddToDashboard.Enabled = False
            lnkbtnAddToDashboard.Visible = False
            LinkButtonBack.Enabled = False
            LinkButtonBack.Visible = False
            LinkButtonData.Enabled = False
            LinkButtonData.Visible = False
            LinkButtonRefresh.Enabled = False
            LinkButtonRefresh.Visible = False
        Else
            DropDownChartType.Items.Clear()
            DropDownChartType.Items.Add("PieChart")
            DropDownChartType.Items.Add("BarChart")
            DropDownChartType.Items.Add("ColumnChart")
            DropDownChartType.Items.Add("LineChart")
            DropDownChartType.Items.Add("AreaChart")
            DropDownChartType.Items.Add("SteppedAreaChart")
            DropDownChartType.Items.Add("ScatterChart")
            DropDownChartType.Items.Add("ComboChart")
            DropDownChartType.Items.Add("BubbleChart")
            DropDownChartType.Items.Add("Histogram")
            DropDownChartType.Items.Add("Gauge")
            DropDownChartType.Items.Add("Sankey")
            'DropDownChartType.Items.Add("CandlestickChart")
            'DropDownChartType.Items.Add("Waterfall")
        End If
        'End If
        If Request("charttype") IsNot Nothing AndAlso Request("charttype").ToString.Trim <> "" Then
            DropDownChartType.Text = Request("charttype").ToString.Trim
            Session("ChartType") = DropDownChartType.Text
        ElseIf Session("ChartType") IsNot Nothing AndAlso Session("ChartType").ToString.Trim <> "" Then
            DropDownChartType.Text = Session("ChartType")
        End If
        Session("GraphType") = ""
        chartwidth = 1900
        If Session("WhereStm") Is Nothing Then Session("WhereStm") = ""

        If Not IsPostBack Then
            'Session("AxisXM") = ""
            'Session("AxisYM") = ""
            'Session("fnM") = ""
            'Session("AggregateM") = ""
            'Session("MFld") = " "
            'Session("SELECTEDValuesM") = " "

            'Else
            If Session("AxisXM") = "" Then
                If Session("cat1") = Session("cat2") Then
                    Session("AxisXM") = Session("cat1")
                Else
                    Session("AxisXM") = Session("cat1") & "," & Session("cat2")
                End If
            End If
            If Session("AxisYM") = "" Then
                Session("AxisYM") = Session("AxisY")
            End If
            If Session("fnM") = "" Then
                Session("fnM") = Session("fn")
            End If
            If Session("AggregateM") = "" Then
                Session("AggregateM") = Session("Aggregate")
            ElseIf Session("Aggregate") = "" Then
                Session("Aggregate") = Session("AggregateM")
            End If
            LabelInfo3.Text = ""
            If Request("lbl") IsNot Nothing AndAlso Request("lbl").ToString.Trim <> "" Then
                LabelInfo3.Text = Request("lbl").ToString
                LabelInfo3.Visible = True
                If LabelInfo3.Text = "Average return" Then
                    LabelInfo3.Text = "* Multiple different Values for the same selected categories in XAxis. Average returnes instead."
                    DropDownListA.Text = "Avg"
                    Session("Aggregate") = "Avg"
                    Session("AggregateM") = "Avg"
                End If
            Else
                LabelInfo3.Text = ""
            End If
            If Request("frm") IsNot Nothing AndAlso Request("frm").ToString.Trim <> "" Then
                Session("frm") = Request("frm").ToString.Trim
            ElseIf Session("frm") Is Nothing Then
                Session("frm") = ""
            End If
        End If
        charttypeM = "MultiLineChart"
        srt = ""
        If Session("srt") IsNot Nothing AndAlso Session("srt").ToString.Trim <> "" Then srt = Session("srt").ToString
    End Sub

    Private Sub LoadDashboards()
        Dim li As ListItem = Nothing

        ddDashboards.Items.Clear()

        If sDashboards IsNot Nothing AndAlso sDashboards.Length > 0 Then
            For i As Integer = 0 To sDashboards.Length - 1
                li = New ListItem(sDashboards(i), sDashboards(i))
                ddDashboards.Items.Add(li)
            Next
        End If
    End Sub
    Private Sub ChartGoogleOne_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("admin") <> "admin" AndAlso Session("admin") <> "super" Then
            lnkbtnAddToDashboard.Enabled = False
            lnkbtnAddToDashboard.Visible = False
            lnkDownloadARR.Enabled = False
            lnkDownloadARR.Visible = False
            HyperLinkDataAI.Enabled = False
            HyperLinkDataAI.Visible = False
        Else
            lnkbtnAddToDashboard.Enabled = True
            lnkbtnAddToDashboard.Visible = True
            lnkDownloadARR.Enabled = True
            lnkDownloadARR.Visible = True
            HyperLinkDataAI.Enabled = True
            HyperLinkDataAI.Visible = True
        End If
        If Session("arr") Is Nothing OrElse Session("arr").ToString.Trim = "" Then
            lnkbtnAddToDashboard.Enabled = False
            lnkbtnAddToDashboard.Visible = False
            lnkDownloadARR.Enabled = False
            lnkDownloadARR.Visible = False
            HyperLinkDataAI.Enabled = False
            HyperLinkDataAI.Visible = False
        Else
            HyperLinkDataAI.Enabled = True
            HyperLinkDataAI.Visible = True
        End If
        Dim writetext As String = String.Empty

        If Session("nv") > 3 Then
            chkboxNumeric.Checked = True
            'Else
            '    chkboxNumeric.Checked = False
        End If

        'get array  arr, etc...
        charttype = DropDownChartType.SelectedItem.Text
        If charttype = "BubbleChart" OrElse charttype = "CandlestickChart" OrElse charttype = "Sankey" OrElse charttype = "Gauge" Then
            Session("arr") = ""
            arr = ""
            If IsPostBack = False Then
                Session("newarr") = "yes"
            Else
                Session("newarr") = "no"
            End If
            charttypeM = charttype
        ElseIf charttype = "MatrixChart" Then
            charttypeM = "MatrixChart"
        ElseIf charttype = "MapChart" OrElse charttype = "GeoChart" Then
            charttypeM = charttype
        Else
            charttypeM = "MultiLineChart"
        End If
        If charttype = "MapChart" OrElse charttype = "Map" Then
            chartpckg = "map"
        ElseIf charttype = "GeoChart" Then
            chartpckg = "geochart"
        ElseIf charttype = "Gauge" Then
            chartpckg = "gauge"
        ElseIf charttype = "Sankey" Then
            chartpckg = "sankey"
        ElseIf charttypeM = "MultiLineChart" Then
            chartpckg = "corechart"
        Else
            chartpckg = "corechart"
        End If

        LabelInfo3.Text = ""
        If Not IsPostBack Then
            If Request("lbl") IsNot Nothing AndAlso Request("lbl").ToString.Trim <> "" Then
                LabelInfo3.Text = Request("lbl").ToString
                LabelInfo3.Visible = True
                If LabelInfo3.Text = "Average Return" Then
                    LabelInfo3.Text = "* Multiple different Values For the same selected categories In XAxis. Average returnes instead."
                    DropDownListA.Text = "Avg"
                    Session("Aggregate") = "Avg"
                    Session("AggregateM") = "Avg"
                End If
            Else
                LabelInfo3.Text = ""
            End If

        End If
        Dim ret As String = String.Empty
        Dim repid As String = String.Empty

        hdnDefaultDashboard.Value = "Dashboard " & Session("logon")
        hdnDefaultDashboardAvailable.Value = DashboardAvailable(Session("logon"), "Dashboard " & Session("logon")).ToString.ToLower

        Dim target As String = Request("__EVENTTARGET")
        Dim data As String = Request("__EVENTARGUMENT")

        If target IsNot Nothing AndAlso data IsNot Nothing Then
            If target.StartsWith("AddDashboards") Then
                charttype = Piece(target, "~", 2)
                DoAddToDashboard(data)
            End If
        End If

        Dim DashboardList As ListItemCollection = GetDashBoards(Session("logon"))
        If DashboardList.Count > 0 Then
            ddDashboards.Items = DashboardList
        Else
            ddDashboards.Items.Clear()
        End If

        If Request("map") IsNot Nothing AndAlso Request("map").ToString = "yes" Then
            HyperLinkDataAI.NavigateUrl = "DataAI.aspx?pg=maps&type=" & charttype & "&mapname=" & Request("mapname").ToString
        Else
            HyperLinkDataAI.NavigateUrl = "DataAI.aspx?pg=charts"
        End If

        writetext = ""
        ' ----------------------FROM Matrix Balancing-----------!!!!!!--------------------------------------------------------------------------------------------
        If (Not Request("MatrixChart") Is Nothing AndAlso Request("MatrixChart") = "yes") OrElse (Not IsPostBack AndAlso Session("MatrixChart") = "MatrixChart") Then
            Session("MatrixChart") = "MatrixChart"

            LabelInfo1.Text = ""
            LabelInfo2.Text = ""
            LabelX.Visible = False
            LabelY.Visible = False
            LabelM.Visible = False
            LabelV.Visible = False
            DropDownColumnsX.Visible = False
            DropDownColumnsY.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            btnShowChart.Enabled = False
            btnShowChart.Visible = False
            LabelA.Visible = False
            DropDownListA.Enabled = False
            DropDownListA.Visible = False

            Dim dtkr As DataTable = Session("dtkr")
            Dim grp0 As String = ""
            Dim val As String = ""
            Dim clr As String = ""
            arr = ""
            Dim mcolors() As String = {"blue", "gold", "lightgreen", "red", "yellow", "darkred", "lightblue", "green", "#e5e4e2", "orange", "darkgreen", "pink", "darkblue", "grey", "maroon", "salmon", "darkorange"}
            Dim k As Integer = 0
            Dim lastk As Integer = 0
            Dim n, m, i, j, l As Integer
            Dim itemval As Double = 0
            Dim lbl As String = String.Empty
            If Not Request("MatrixItem") Is Nothing Then
                itemval = Request("MatrixItem")   'item value of starting matrix
                lbl = Request("MatrixItemLabel")
                'initial value
                i = 0
                n = CInt(Request("MatrixRow").ToString)  'row number in starting matrix
                m = CInt(Request("MatrixColumn").ToString) 'column number in starting matrix
                l = 0
                For j = 0 To dtkr.Columns.Count - 1
                    If dtkr.Columns(j).Caption.Trim = "--" Then
                        l = j + 1
                        Exit For
                    End If
                Next
                grp0 = lbl & " - " & dtkr.Columns(n + 1).Caption & " - " & dtkr.Columns(l + m).Caption
                val = itemval.ToString
                If val = "" Then val = "0"
                arr = arr & "['" & grp0 & "'," & val & ","
                writetext = writetext & "['" & grp0.Replace(",", ";") & "'," & val & ","
                'color
                k = lastk Mod mcolors.Length
                clr = mcolors(k)
                lastk = k + 1
                arr = arr & "'" & clr & "'"
                arr = arr & "]"
                writetext = writetext & "'" & clr & "']"

                If i < dtkr.Rows.Count Then
                    arr = arr & ","
                    writetext = writetext & ","
                End If
                If itemval = 0 Then itemval = 0.001
                For i = 0 To dtkr.Rows.Count - 1
                    grp0 = dtkr.Rows(i)(0).ToString & " - " & dtkr.Columns(n + 1).Caption & " - " & dtkr.Columns(l + m).Caption
                    If dtkr.Rows(i)(n + 1) = 0 Then dtkr.Rows(i)(n + 1) = 0.001
                    If dtkr.Rows(i)(l + m) = 0 Then dtkr.Rows(i)(l + m) = 0.001
                    val = itemval * dtkr.Rows(i)(n + 1) * dtkr.Rows(i)(l + m)
                    If val = "" Then val = "0"
                    arr = arr & "['" & grp0 & "'," & val & ","
                    writetext = writetext & "['" & grp0.Replace(",", ";") & "'," & val & ","
                    'color
                    k = lastk Mod mcolors.Length
                    clr = mcolors(k)
                    lastk = k + 1
                    arr = arr & "'" & clr & "'"
                    arr = arr & "]"
                    writetext = writetext & "'" & clr & "']"
                    If i < dtkr.Rows.Count Then
                        arr = arr & ","
                        writetext = writetext & ","
                    End If
                Next
                arr = "['" & srt & "','Balancing value',{ role: 'style' }]," & arr
                writetext = "['" & srt & "','Balancing value',{ role: 'style' }]," & writetext
                'Session("nrec") = dtkr.Columns.Count - 1
                Session("nrec") = dtkr.Columns.Count
                Session("arr") = arr

                ttl = "Starting Matrix with Balancing Values " & " - " & dtkr.Columns(n + 1).Caption & " - " & dtkr.Columns(l + m).Caption & " (" & dtkr.Rows.Count.ToString & " records)"
            ElseIf Not Request("MatrixRow") Is Nothing AndAlso Request("MatrixItem") Is Nothing Then 'row chart
                n = CInt(Request("MatrixRow").ToString)
                For j = 0 To dtkr.Columns.Count - 2
                    grp0 = dtkr.Columns(j + 1).Caption & " - " & dtkr.Rows(n)(0).ToString
                    'dtkr.Columns(j + 1).Caption = GridView4.HeaderRow.Cells(j + 1).ToolTip & "-" & GridView4.HeaderRow.Cells(j + 1).Text
                    val = dtkr.Rows(n)(j + 1).ToString.Trim
                    If val = "" Then val = "0"
                    arr = arr & "['" & grp0 & "'," & val & ","
                    writetext = writetext & "['" & grp0.Replace(",", ";") & "'," & val & ","
                    'color
                    k = lastk Mod mcolors.Length
                    clr = mcolors(k)
                    If dtkr.Columns(j + 1).Caption = "--" Then
                        clr = "black"
                    End If
                    lastk = k + 1
                    arr = arr & "'" & clr & "'"
                    arr = arr & "]"
                    writetext = writetext & "'" & clr & "']"
                    If j < dtkr.Columns.Count - 1 Then
                        arr = arr & ","
                        writetext = writetext & ","
                    End If
                Next
                arr = "['" & srt & "','Balancing coefficient',{ role: 'style' }]," & arr
                writetext = "['" & srt & "','Balancing coefficient',{ role: 'style' }]," & writetext
                Session("nrec") = dtkr.Columns.Count - 1
                Session("arr") = arr

                ttl = "Balancing Coefficients for Step " & " - " & dtkr.Rows(n)(0).ToString & " (" & dtkr.Rows.Count.ToString & " records)"

            ElseIf Not Request("MatrixColumn") Is Nothing AndAlso Request("MatrixItem") Is Nothing Then 'column chart
                m = CInt(Request("MatrixColumn").ToString)
                For i = 0 To dtkr.Rows.Count - 1
                    grp0 = dtkr.Columns(m).Caption & " - " & dtkr.Rows(i)(0).ToString
                    'dtkr.Columns(j + 1).Caption = GridView4.HeaderRow.Cells(j + 1).ToolTip & "-" & GridView4.HeaderRow.Cells(j + 1).Text
                    val = dtkr.Rows(i)(m).ToString.Trim
                    If val = "" Then val = "0"
                    arr = arr & "['" & grp0 & "'," & val & ","
                    writetext = writetext & "['" & grp0.Replace(",", ";") & "'," & val & ","
                    'color
                    k = lastk Mod mcolors.Length
                    clr = mcolors(k)
                    'If dtkr.Columns(j + 1).Caption = "--" Then
                    '    clr = "black"
                    'End If
                    lastk = k + 1
                    arr = arr & "'" & clr & "'"
                    arr = arr & "]"
                    writetext = writetext & "'" & clr & "']"
                    If i < dtkr.Rows.Count Then
                        arr = arr & ","
                        writetext = writetext & ","
                    End If
                Next
                arr = "['" & srt & "','Balancing coefficient',{ role: 'style' }]," & arr
                writetext = "['" & srt & "','Balancing coefficient',{ role: 'style' }]," & writetext
                Session("nrec") = dtkr.Columns.Count - 1
                Session("arr") = arr

                ttl = "Balancing Coefficients for - " & dtkr.Columns(m).Caption & " (" & dtkr.Rows.Count.ToString & " records)"

            Else  'main Chart
                For i = 0 To dtkr.Rows.Count - 1
                    For j = 0 To dtkr.Columns.Count - 2
                        grp0 = dtkr.Rows(i)(0).ToString & "," & dtkr.Columns(j + 1).Caption
                        If dtkr.Columns(j + 1).Caption = "--" Then
                            grp0 = dtkr.Rows(i)(0).ToString & ",-------"
                        End If
                        val = dtkr.Rows(i)(j + 1).ToString.Trim
                        If val = "" Then val = "0"
                        arr = arr & "['" & grp0 & "'," & val & ","
                        writetext = writetext & "['" & grp0.Replace(",", ";") & "'," & val & ","
                        'color
                        k = lastk Mod mcolors.Length
                        clr = mcolors(k)
                        If dtkr.Columns(j + 1).Caption = "--" Then
                            clr = "black"
                        End If
                        lastk = k + 1
                        arr = arr & "'" & clr & "'"
                        arr = arr & "]"
                        writetext = writetext & "'" & clr & "']"
                        'If i < dtkr.Rows.Count OrElse j < dtkr.Columns.Count - 1 Then
                        If j < dtkr.Columns.Count - 1 Then
                            arr = arr & ","
                            writetext = writetext & ","
                        End If
                    Next
                    'add one more item with "--" to separate kjs from kis coefficiets
                    grp0 = dtkr.Rows(i)(0).ToString & ",-------"
                    val = 0
                    clr = "black"
                    arr = arr & "['" & grp0 & "'," & val & "," & "'" & clr & "']"
                    writetext = writetext & "['" & grp0.Replace(",", ";") & "'," & val & "," & "'" & clr & "']"
                    If i < dtkr.Rows.Count Then
                        arr = arr & ","
                        writetext = writetext & ","
                    End If
                Next
                arr = "['" & srt & "','Balancing coefficient',{ role: 'style' }]," & arr
                writetext = "['" & srt & "','Balancing coefficient',{ role: 'style' }]," & writetext
                'Session("nrec") = dtkr.Rows.Count * (dtkr.Columns.Count - 1)
                Session("nrec") = dtkr.Rows.Count * dtkr.Columns.Count
                Session("arr") = arr

                ttl = "Balancing Coefficients" & " (" & dtkr.Rows.Count.ToString & " records)"
            End If

            If Session("arr") IsNot Nothing AndAlso Session("arr").ToString.Trim <> "" Then arr = Session("arr").ToString
            If Session("srt") IsNot Nothing AndAlso Session("srt").ToString.Trim <> "" Then ttl = Session("srt").ToString
            Dim nr As Integer = arr.Split("]").Length
            'chartwidth, chartheght
            chartwidth = 1900
            If charttype = "BarChart" Then
                chartheght = (10 * nr)
            ElseIf charttype = "Sankey" Then
                chartheght = (5 * nr)
            Else
                chartheght = 870
            End If
            If chartheght < 850 Then
                chartheght = 850
            End If
            If charttype = "ColumnChart" Then
                chartwidth = nr * 10
                If chartwidth < 1900 Then chartwidth = 1900
            End If

            Session("arr") = arr

            Session("ttl") = ttl
            LabelWhere.Text = ttl & "      " & Session("WhereStm").ToString.Trim
            writetext = LabelWhere.Text & Chr(10) & writetext
            Session("writetext") = writetext
            Exit Sub

        End If
        '-----------------------------------------End Matrix------------------------------------------------------------------------------------

        'Retrieve Report Data
        If Request("Report") Is Nothing OrElse Request("Report").ToString.Trim = "" Then
            Exit Sub
        Else
            repid = Request("Report").ToString
            Session("REPORTID") = repid
        End If

        If Request("wherestm") IsNot Nothing Then
            wherestm = Request("wherestm").ToString.Trim.Replace("^", "'")
            Session("WhereStm") = wherestm
        End If
        If Session("dv3") Is Nothing OrElse Session("dv3").ToString = String.Empty Then
            'retrieve dv3
            dv3 = RetrieveReportData(repid, Session("WhereStm").ToString, True, -1, Nothing, Nothing, Nothing, Session("UserConnString"), Session("UserConnProvider"), ret)
            If dv3 Is Nothing OrElse dv3.Table Is Nothing OrElse dv3.Table.Rows.Count = 0 Then
                LabelError.Text = "No data"
                Exit Sub
            End If
        Else
            dv3 = Session("dv3")
        End If

        'correct dv3 replacing commas in all rows, column names are already CLS-compliant
        For j = 0 To dv3.Table.Columns.Count - 1
            If dv3.Table.Columns(j).DataType.Name = "String" OrElse dv3.Table.Columns(j).DataType.Name = "Text" Then
                For i = 0 To dv3.Table.Rows.Count - 1
                    If IsDBNull(dv3.Table.Rows(i)(j)) Then
                        dv3.Table.Rows(i)(j) = ""
                    ElseIf dv3.Table.Rows(i)(j).ToString.trim <> "" AndAlso dv3.Table.Rows(i)(j).ToString.IndexOf(",") > 0 Then
                        dv3.Table.Rows(i)(j) = dv3.Table.Rows(i)(j).ToString.Replace(",", ";")
                    End If
                Next
            End If
        Next

        Session("dv3") = dv3
        Dim dt As DataTable = dv3.Table


        If Session("REPTITLE") Is Nothing OrElse Session("REPTITLE").ToString.Trim = "" Then
            Dim dr As DataTable = GetReportInfo(repid)
            Session("REPTITLE") = dr.Rows(0)("ReportTtl").ToString
        End If


        '------------------------------------------  MAP chart or GeoChart---------------------------------------------------------
        If Request("map") IsNot Nothing AndAlso Request("map").ToString.Trim = "yes" Then
            Session("MapChart") = "MapChart"
            Session("chartregn") = "world"
            LabelInfo1.Text = ""
            LabelInfo2.Text = ""
            LabelX.Visible = False
            LabelY.Visible = False
            LabelM.Visible = False
            LabelV.Visible = False
            DropDownColumnsX.Visible = False
            DropDownColumnsY.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            btnShowChart.Enabled = False
            btnShowChart.Visible = False
            LabelA.Visible = False
            DropDownListA.Enabled = False
            DropDownListA.Visible = False
            lnkbtnReverse.Visible = False
            lnkbtnReverse.Enabled = False

            mapname = String.Empty
            If Request("mapname") Is Nothing OrElse Request("mapname").ToString.Trim = "" Then
                LabelError.Text = "no Map found with this name "
                Exit Sub
            Else
                mapname = Request("mapname").ToString
                Session("mapname") = mapname
                mapyesno = "yes"
                Session("mapyesno") = mapyesno
            End If

            If Session("admin") <> "admin" AndAlso Session("admin") <> "super" Then
                lnkbtnAddToDashboard.Enabled = False
                lnkbtnAddToDashboard.Visible = False
                lnkDownloadARR.Enabled = False
                lnkDownloadARR.Visible = False
            Else
                lnkbtnAddToDashboard.Enabled = True
                lnkbtnAddToDashboard.Visible = True
                lnkDownloadARR.Enabled = True
                lnkDownloadARR.Visible = True
            End If

            chartregn = Request("chartregn")
            If chartregn Is Nothing OrElse chartregn.ToString.Trim = "" Then
                chartregn = "world"
            Else
                If chartregn.IndexOf(":") >= 0 Then chartregn = chartregn.Substring(chartregn.IndexOf(":") + 1)
            End If
            Session("chartregn") = chartregn

            'mapname = Request("mapname").ToString
            ttl = mapname & " (" & dt.Rows.Count.ToString & " placemarks)"
            chartheght = 870
            If DropDownChartType.Text = "MapChart" Then
                charttype = "MapChart"
                chartpckg = "map"
            Else
                charttype = "GeoChart"
                chartpckg = "geochart"
            End If

            Dim dm As DataTable = GetReportMapFields(repid, mapname)
            Dim dcl As DataTable = GetReportColorField(repid, mapname)
            Dim dce As DataTable = GetReportExtrudedFields(repid, mapname)
            Dim drk As DataTable = GetReportKeyFields(repid, mapname)
            If dm Is Nothing Then
                ret = "no Map fields found"
                LabelError.Text = ret
                'MessageBox.Show("Nothing to save", "Nothing to save", "KMLnotsaved", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            End If
            Dim i As Integer
            Dim lon As String = String.Empty
            Dim lat As String = String.Empty
            Dim latlon As String = String.Empty
            Dim nm As String = String.Empty
            Dim des As String = String.Empty
            Dim lonend As String = String.Empty
            Dim latend As String = String.Empty
            Dim starttimecol As String = String.Empty
            Dim endtimecol As String = String.Empty
            Dim lonadd As String = String.Empty
            Dim latadd As String = String.Empty
            Dim coordadd As String = String.Empty
            Dim coor As String = String.Empty
            For i = 0 To dm.Rows.Count - 1
                If dm.Rows(i)("ForMap").ToString.Trim = "PlacemarkName" AndAlso dm.Rows(i)("ord") = 0 Then
                    nm = dm.Rows(i)("MapField").ToString.Trim.Substring(dm.Rows(i)("MapField").ToString.Trim.LastIndexOf(".") + 1)
                    If dm.Rows(i)("descrtext").ToString.Trim <> "" Then
                        chartregn = dm.Rows(i)("descrtext").ToString.Trim '.Substring(dm.Rows(i)("descrtext").ToString.Trim.IndexOf(":") + 1)
                        If chartregn.IndexOf(":") > 0 Then
                            chartregn = chartregn.Substring(dm.Rows(i)("descrtext").ToString.Trim.IndexOf(":") + 1)
                        End If
                        Session("chartregn") = chartregn
                    End If
                    If dm.Rows(i)("Prop7").ToString.Trim.ToUpper = "TRUE" Then
                        latlon = "True"
                    Else
                        latlon = "False"
                    End If
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

            If lon.Trim = "" OrElse lat.Trim = "" Then
                ret = "no Map fields found"
                LabelError.Text = ret
                'MessageBox.Show("Nothing to save", "Nothing to save", "KMLnotsaved", Controls_Msgbox.Buttons.OK, Controls_Msgbox.MessageIcon.Warning, Controls_Msgbox.MessageDefaultButton.PostOK)
                Exit Sub
            End If


            'request came from MapReport - use dt
            Dim p1, p2 As String
            arr = ""
            writetext = ""
            coor = ""

            'For MapChart and GeoChart the coordinates are in format latitude, longitude !!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            If DropDownChartType.Text = "MapChart" Then
                'Map chart ----------------------------------------------------------------------------------------------

                arr = "['Long','Lat','Name','Marker'],"

                For i = 0 To dt.Rows.Count - 1
                    'lon,lat
                    coor = ""
                    If lat = "POINT" Then
                        If dt.Rows(i)(lon).ToString.Trim = "" Then
                            Continue For
                        End If
                    Else
                        If i > 0 AndAlso (dt.Rows(i)(lon).ToString.Trim = "" OrElse dt.Rows(i)(lat).ToString.Trim = "") Then
                            Continue For
                        End If
                        If i > 0 AndAlso dt.Rows(i)(lon).ToString = "0" AndAlso dt.Rows(i)(lat).ToString = "0" Then
                            Continue For
                        End If
                    End If

                    arr = arr & "["
                    If lat = "POINT" Then
                        If dt.Rows(i)(lon).ToString.Trim = "" Then
                            Continue For
                        End If
                        'arr = arr & dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",") & ","
                        p1 = Piece(dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                        p2 = Piece(dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                        If latlon = "True" Then
                            arr = arr & p2 & "," & p1 & ","
                        Else
                            arr = arr & p1 & "," & p2 & ","
                        End If


                        'ElseIf i > 0 AndAlso dt.Rows(i)(lon) = dt.Rows(i - 1)(lon) AndAlso dt.Rows(i)(lat) = dt.Rows(i - 1)(lat) Then  '???
                        '    Continue For

                    ElseIf lon <> lat AndAlso IsNumeric(dt.Rows(i)(lon).ToString) AndAlso IsNumeric(dt.Rows(i)(lat).ToString) Then
                        arr = arr & dt.Rows(i)(lat).ToString & "," & dt.Rows(i)(lon).ToString & ","
                    ElseIf lon <> lat AndAlso Not IsNumeric(dt.Rows(i)(lon).ToString) AndAlso Not IsNumeric(dt.Rows(i)(lat).ToString) Then
                        arr = arr & ConvertGeoToDDformat(dt.Rows(i)(lat).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & "," & ConvertGeoToDDformat(dt.Rows(i)(lon).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & ","


                    ElseIf lon = lat AndAlso Not IsNumeric(dt.Rows(i)(lon).ToString) Then
                        coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lon).ToString, chartregn)
                        arr = arr & coor & ","
                        'arr = arr & "'" & dt.Rows(i)(lon).ToString & "'" & ","
                    End If
                    'arr = arr & "'" & dt.Rows(i)(nm).ToString & "'"
                    arr = arr & "'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ").Replace(",", ";")).Trim & "'"
                    arr = arr & ",'pink'"
                    arr = arr & "]"
                    If i < dt.Rows.Count - 1 Then arr = arr & ","

                    If lonend.Trim <> "" AndAlso latend.Trim <> "" Then
                        'lonend,latend
                        If latend = "POINT" Then
                            If dt.Rows(i)(lonend).ToString.Trim = "" Then
                                Continue For
                            End If
                        Else
                            If i > 0 AndAlso (dt.Rows(i)(lonend).ToString.Trim = "" OrElse dt.Rows(i)(latend).ToString.Trim = "") Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lonend) = 0 AndAlso dt.Rows(i)(latend) = 0 Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lonend) = dt.Rows(i - 1)(lonend) AndAlso dt.Rows(i)(latend) = dt.Rows(i - 1)(latend) Then
                                Continue For
                            End If
                        End If

                        arr = arr & "["
                        If latend = "POINT" Then
                            If dt.Rows(i)(lonend).ToString.Trim = "" Then
                                Continue For
                            End If
                            'arr = arr & dt.Rows(i)(lonend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",")
                            p1 = Piece(dt.Rows(i)(lonend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                            p2 = Piece(dt.Rows(i)(lonend).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                            'arr = arr & lt & "," & ln & ","
                            If latlon = "True" Then
                                arr = arr & p2 & "," & p1 & ","
                            Else
                                arr = arr & p1 & "," & p2 & ","
                            End If

                        ElseIf lonend <> latend AndAlso IsNumeric(dt.Rows(i)(lonend).ToString) AndAlso IsNumeric(dt.Rows(i)(latend).ToString) Then
                            arr = arr & dt.Rows(i)(latend).ToString & "," & dt.Rows(i)(lonend).ToString & ","
                        ElseIf lonend <> latend AndAlso Not IsNumeric(dt.Rows(i)(lonend).ToString) AndAlso Not IsNumeric(dt.Rows(i)(latend).ToString) Then
                            arr = arr & ConvertGeoToDDformat(dt.Rows(i)(latend).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & "," & ConvertGeoToDDformat(dt.Rows(i)(lonend).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & ","


                        ElseIf lonend = latend AndAlso Not IsNumeric(dt.Rows(i)(lonend).ToString) Then
                            'arr = arr & "'" & dt.Rows(i)(lonend).ToString & "'" & ","
                            coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lonend).ToString, chartregn)
                            arr = arr & coor & ","
                        End If
                        'arr = arr & "'" & dt.Rows(i)(nm).ToString & "'"
                        arr = arr & "'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ").Replace(",", ";")).Trim & "'"
                        arr = arr & ",'blue'"
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","
                    End If

                    If lonadd.Trim <> "" AndAlso latadd.Trim <> "" Then
                        'lonadd,latedd
                        If latadd = "POINT" Then
                            If dt.Rows(i)(lonadd).ToString.Trim = "" Then
                                Continue For
                            End If
                        Else
                            If i > 0 AndAlso (dt.Rows(i)(lonadd).ToString.Trim = "" OrElse dt.Rows(i)(latadd).ToString.Trim = "") Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lonadd) = 0 AndAlso dt.Rows(i)(latadd) = 0 Then
                                Continue For
                            End If
                            If i > 0 AndAlso dt.Rows(i)(lonadd) = dt.Rows(i - 1)(lonadd) AndAlso dt.Rows(i)(latadd) = dt.Rows(i - 1)(latadd) Then
                                Continue For
                            End If
                        End If

                        arr = arr & "["
                        If latadd = "POINT" Then
                            If dt.Rows(i)(lonadd).ToString.Trim = "" Then
                                Continue For
                            End If
                            'arr = arr & dt.Rows(i)(lonadd).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",")
                            p1 = Piece(dt.Rows(i)(lonadd).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 1)
                            p2 = Piece(dt.Rows(i)(lonadd).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",").Replace(",,,", ",").Replace(",,", ","), ",", 2)

                            'arr = arr & lt & "," & ln & ","
                            If latlon = "True" Then
                                arr = arr & p2 & "," & p1 & ","
                            Else
                                arr = arr & p1 & "," & p2 & ","
                            End If

                        ElseIf lonadd <> latadd AndAlso IsNumeric(dt.Rows(i)(lonadd).ToString) AndAlso IsNumeric(dt.Rows(i)(latadd).ToString) Then
                            arr = arr & dt.Rows(i)(latadd).ToString & "," & dt.Rows(i)(lonadd).ToString & ","
                        ElseIf lonadd <> latadd AndAlso Not IsNumeric(dt.Rows(i)(lonadd).ToString) AndAlso Not IsNumeric(dt.Rows(i)(latadd).ToString) Then
                            arr = arr & ConvertGeoToDDformat(dt.Rows(i)(latadd).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & "," & ConvertGeoToDDformat(dt.Rows(i)(lonadd).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & ","

                        ElseIf lonadd = latadd AndAlso Not IsNumeric(dt.Rows(i)(lonadd).ToString) Then
                            'arr = arr & "'" & dt.Rows(i)(lonadd).ToString & "'" & ","
                            coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lonadd).ToString, chartregn)
                            arr = arr & coor & ","
                        End If
                        'arr = arr & "'" & dt.Rows(i)(nm).ToString & "'"
                        arr = arr & "'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ").Replace(",", ";")).Trim & "'"
                        arr = arr & ",'green'"
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","
                    End If
                Next
                arr = arr.Replace("][", "],[")  'correct if record for end or add were skipped
                charttype = "Map"  'for google.visualization in aspx 
            Else  'GeoChart ---------------------------------------------------------------------------------------------
                Dim fldtoclr As String = String.Empty
                Dim fldtoext As String = String.Empty
                If dcl Is Nothing OrElse dcl.Rows.Count = 0 Then
                Else
                    fldtoclr = dcl.Rows(0)("Val").ToString
                End If
                If dce Is Nothing OrElse dce.Rows.Count = 0 Then
                Else
                    fldtoext = dce.Rows(0)("Val").ToString
                End If
                arr = "['Long','Lat'," ''" & fldtoclr & "','" & fldtoext & "'],"
                If lon <> lat Then
                    If fldtoclr <> "" Then arr = arr & "'" & fldtoclr & "'"
                    If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext Then
                        arr = arr & ",'" & fldtoext & "'"
                    End If
                    'If fldtoclr <> "" Then arr = arr & "'Value1'"
                    'If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext Then
                    '    arr = arr & ",'Value2'"
                    'End If
                    arr = arr & "],"
                Else
                    'arr = "['Address'," ''" & fldtoclr & "','" & fldtoext & "'],"
                    arr = "['Long','Lat'" ''" & fldtoclr & "','" & fldtoext & "'],"

                    If fldtoclr <> "" Then arr = arr & ",'" & fldtoclr & "'"
                    If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext Then
                        arr = arr & ",'" & fldtoext & "'"
                    End If
                    'If fldtoclr <> "" Then arr = arr & "'Value1'"
                    'If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext Then
                    '    arr = arr & ",'Value2'"
                    'End If

                    arr = arr & "],"
                End If
                If fldtoclr = "" Then
                    arr = "['Long','Lat','W']"
                End If

                Dim lnlt As String = String.Empty
                For i = 0 To dt.Rows.Count - 1
                    'lon,lat
                    arr = arr & "["
                    If lat = "POINT" Then
                        If dt.Rows(i)(lon).ToString.Trim = "" Then
                            Continue For
                        End If
                        'arr = arr & dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",") & ","
                        lnlt = dt.Rows(i)(lon).ToString.ToUpper.Replace("POINT", "").Trim.Replace("(", "").Replace(")", "").Replace(" ", ",")
                        lnlt = lnlt.Replace(",,,", ",").Replace(",,", ",")
                        p1 = Piece(lnlt, ",", 1)
                        p2 = Piece(lnlt, ",", 2)

                        'arr = arr & lt & "," & ln & ","
                        If latlon = "True" Then
                            arr = arr & p2 & "," & p1 & ","
                        Else
                            arr = arr & p1 & "," & p2 & ","
                        End If

                    ElseIf lon <> lat AndAlso IsNumeric(dt.Rows(i)(lon).ToString) AndAlso IsNumeric(dt.Rows(i)(lat).ToString) Then
                        arr = arr & dt.Rows(i)(lat).ToString & "," & dt.Rows(i)(lon).ToString & ","

                    ElseIf lon <> lat AndAlso Not IsNumeric(dt.Rows(i)(lon).ToString) AndAlso Not IsNumeric(dt.Rows(i)(lat).ToString) Then
                        arr = arr & ConvertGeoToDDformat(dt.Rows(i)(lat).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & "," & ConvertGeoToDDformat(dt.Rows(i)(lon).ToString.Replace("***", "'").Replace(ChrW(&HFFFD), ChrW(&HB0))) & ","

                    ElseIf lon = lat AndAlso Not IsNumeric(dt.Rows(i)(lon).ToString) Then
                        'arr = arr & "'" & dt.Rows(i)(lon).ToString & "'" & ","
                        coor = CoordinatesLatLngGeocoding(dt.Rows(i)(lon).ToString, chartregn)
                        arr = arr & coor & ","
                    End If
                    If fldtoclr <> "" AndAlso IsNumeric(dt.Rows(i)(fldtoclr).ToString) Then
                        arr = arr & dt.Rows(i)(fldtoclr).ToString
                    ElseIf fldtoclr <> "" AndAlso Not IsNumeric(dt.Rows(i)(fldtoclr).ToString) Then
                        arr = arr & "'" & dt.Rows(i)(fldtoclr).ToString & "'"
                    End If
                    If fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext AndAlso IsNumeric(dt.Rows(i)(fldtoext).ToString) Then
                        arr = arr & "," & dt.Rows(i)(fldtoext).ToString
                    ElseIf fldtoclr <> "" AndAlso fldtoext <> "" AndAlso fldtoclr <> fldtoext AndAlso Not IsNumeric(dt.Rows(i)(fldtoext).ToString) Then
                        arr = arr & ",'" & dt.Rows(i)(fldtoext).ToString & "'"
                    ElseIf fldtoclr = "" Then
                        arr = arr & "4000"
                        'arr = arr.Substring(0, arr.Length - 1)
                    End If
                    'arr = arr & ",'" & (dt.Rows(i)(nm).ToString & DescriptionText(dt, dm, i, False, True).Replace("<![CDATA[", " ").Replace("<br>", " ").Replace("</br>", " ").Replace("<b>", " ").Replace("</b>", " ")).Trim & "'"

                    arr = arr & "]"
                    If i < dt.Rows.Count - 1 Then arr = arr & ","
                Next
            End If

            Session("arr") = arr

            If Session("arr").ToString.Trim = "" Then
                lnkbtnAddToDashboard.Enabled = False
                lnkbtnAddToDashboard.Visible = False
                lnkDownloadARR.Enabled = False
                lnkDownloadARR.Visible = False
                HyperLinkDataAI.Enabled = False
                HyperLinkDataAI.Visible = False
            Else
                HyperLinkDataAI.Enabled = True
                HyperLinkDataAI.Visible = True
            End If

            Session("ttl") = ttl
            LabelWhere.Text = Session("REPTITLE").ToString & ":            --- " & ttl & " ---     " & Session("WhereStm").ToString.Trim
            writetext = LabelWhere.Text
            writetext = writetext & Chr(10) & Session("arr").ToString
            Session("writetext") = writetext
            Exit Sub

        End If
        '----------------------------------  END MAP ------------------------------------------------------

        'From Dashboard
        If Not IsPostBack AndAlso Request("das") IsNot Nothing AndAlso Request("das").ToString.Trim = "yes" Then
            Session("WhereStm") = wherestm.Replace("^", "'")
            Session("AxisXM") = Request("mx")
            Session("AxisYM") = Request("y2")
            Session("AggregateM") = Request("fn")
            Session("MFld") = Request("mf")
            Session("SELECTEDValuesM") = Request("mv")
            DropDownColumnsX.Text = Session("AxisXM")
            DropDownColumnsY.Text = Session("AxisYM")
            DropDownListA.SelectedValue = Session("AggregateM")
            DropDownListM.Text = Session("MFld")
            DropDownColumnsV.Text = Session("SELECTEDValuesM")
            charttype = Request("charttype")
            DropDownChartType.Text = charttype
            chartregn = Request("chartregn")
            arr = Session("arr")
            writetext = Session("writetext")
            Session("newarr") = "no"

        Else
            If wherestm <> "" Then Session("WhereStm") = wherestm.Replace("^", "'")
        End If

        'From btnShowChart
        If Not IsPostBack AndAlso (Not Request("domulti") Is Nothing AndAlso Request("domulti").ToString.Trim = "yes") Then
            If Session("arr") IsNot Nothing AndAlso Session("arr").ToString.Trim <> "" Then arr = Session("arr").ToString
            If Session("ttl") IsNot Nothing AndAlso Session("ttl").ToString.Trim <> "" Then ttl = Session("ttl").ToString
            If Session("srt") IsNot Nothing AndAlso Session("srt").ToString.Trim <> "" Then srt = Session("srt").ToString
            If Session("y1") IsNot Nothing AndAlso Session("y1").ToString.Trim <> "" Then y1 = Session("y1").ToString
            x1 = Session("cat1")
            x2 = Session("cat2")
            y1 = Session("AxisY")
            fn = Session("AggregateM")
            ttl = Session("ttl")

        End If
        If Not IsPostBack AndAlso (((Request("domulti") Is Nothing OrElse Request("domulti").ToString.Trim <> "yes")) OrElse (Session("frm") = "Analytics" AndAlso (Not Request("domulti") Is Nothing AndAlso Request("domulti").ToString.Trim = "yes"))) Then
            arr = ""
            writetext = ""
            ttl = ""
            y1 = ""
            x1 = ""
            x2 = ""
            fn = ""
            mapyesno = "no"
            Session("mapyesno") = ""
            Session("mapname") = ""

            If Request("ttl") IsNot Nothing Then
                ttl = Request("ttl").ToString
                Session("ttl") = ttl
            Else
                ttl = Session("ttl")
            End If
            If Request("srt") IsNot Nothing Then
                srt = Request("srt").ToString
                Session("srt") = srt
            End If
            If Request("y1") IsNot Nothing Then
                y1 = Request("y1").ToString
            Else
                y1 = Session("y1")
            End If
            If Request("x1") IsNot Nothing Then
                x1 = Request("x1").ToString
            Else
                x1 = Session("cat1")
            End If
            If Request("x2") IsNot Nothing Then
                x2 = Request("x2").ToString
            Else
                x2 = Session("cat2")
            End If
            If Request("fn") IsNot Nothing Then
                fn = Request("fn").ToString
            Else
                fn = Session("Aggregate")
            End If

            Session("cat1") = x1
            Session("cat2") = x2
            Session("y1") = y1
            Session("AxisY") = y1
            Session("Aggregate") = fn
            Session("AggregateM") = Session("Aggregate")
            If Session("AxisXM") Is Nothing OrElse Session("AxisXM").ToString.Trim = "" Then
                If x1 = x2 Then
                    Session("AxisXM") = x1
                Else
                    Session("AxisXM") = x1 & "," & x2
                End If
            End If
            If Session("AxisYM") Is Nothing OrElse Session("AxisYM").ToString.Trim = "" Then
                Session("AxisYM") = y1
            End If

        ElseIf IsPostBack AndAlso Session("arr") = "" Then

            Session("AxisXM") = DropDownColumnsX.Text
            Session("AxisYM") = DropDownColumnsY.Text
            Session("AggregateM") = DropDownListA.SelectedValue
            Session("MFld") = DropDownListM.Text
            Session("SELECTEDValuesM") = DropDownColumnsV.Text
            Dim xsels() As String = Split(Session("AxisXM"), ",")
            Dim ysels() As String = Split(Session("AxisYM"), ",")
            Session("AxisY") = ysels(0)
            Session("cat1") = xsels(0)
            If xsels.Length = 1 Then
                Session("cat2") = xsels(0)
            ElseIf xsels.Length > 1 Then
                Session("cat2") = xsels(1)
            Else
                LabelError.Visible = True
                LabelError.Text = "Select chart parameters. "
                Exit Sub
            End If
            Session("Aggregate") = Session("AggregateM")

            If Session("ttl") IsNot Nothing AndAlso Session("ttl").ToString.Trim <> "" Then ttl = Session("ttl").ToString
            If Session("srt") IsNot Nothing AndAlso Session("srt").ToString.Trim <> "" Then srt = Session("srt").ToString
            If Session("y1") IsNot Nothing AndAlso Session("y1").ToString.Trim <> "" Then y1 = Session("y1").ToString
            x1 = Session("cat1")
            x2 = Session("cat2")
            y1 = Session("AxisY")
            fn = Session("Aggregate")
            ttl = Session("ttl")


        ElseIf IsPostBack AndAlso Session("arr") <> "" Then
            If Session("arr") IsNot Nothing AndAlso Session("arr").ToString.Trim <> "" Then arr = Session("arr").ToString

            If Session("ttl") IsNot Nothing AndAlso Session("ttl").ToString.Trim <> "" Then ttl = Session("ttl").ToString
            If Session("srt") IsNot Nothing AndAlso Session("srt").ToString.Trim <> "" Then srt = Session("srt").ToString
            If Session("y1") IsNot Nothing AndAlso Session("y1").ToString.Trim <> "" Then y1 = Session("y1").ToString
            If Session("cat1") IsNot Nothing AndAlso Session("cat1").ToString.Trim <> "" Then x1 = Session("cat1").ToString
            If Session("cat2") IsNot Nothing AndAlso Session("cat2").ToString.Trim <> "" Then x2 = Session("cat2").ToString
            If y1 Is Nothing AndAlso Session("AxisY") IsNot Nothing AndAlso Session("AxisY").ToString.Trim <> "" Then y1 = Session("AxisY").ToString
            If Session("Aggregate") IsNot Nothing AndAlso Session("Aggregate").ToString.Trim <> "" Then fn = Session("Aggregate").ToString

        End If

        'dropdowns
        If charttypeM = "MultiLineChart" Then
            If Not IsPostBack Then
                LabelX.Visible = True
                LabelY.Visible = True
                LabelM.Visible = True
                LabelV.Visible = True
                DropDownColumnsX.Visible = True
                DropDownColumnsY.Visible = True
                DropDownColumnsV.Visible = True
                CheckBoxSelectAll.Visible = True
                CheckBoxUnselectAll.Visible = True
                CheckBoxSelectAll.Enabled = True
                CheckBoxUnselectAll.Enabled = True
                DropDownListM.Enabled = True
                DropDownListM.Visible = True
                btnShowChart.Enabled = True
                btnShowChart.Visible = True
                LabelA.Visible = True
                DropDownListA.Enabled = True
                DropDownListA.Visible = True

                'fill out drop downs
                Dim dv4 As New DataView
                dv4 = Session("dv3")
                'dv4.RowFilter = ""
                Dim ddt As DataTable = dv4.Table
                Dim fldname, frdname, fldtype As String
                Dim xsels() As String = Split(Session("AxisXM"), ",")
                Dim ysels() As String = Split(Session("AxisYM"), ",")
                If charttype = "PieChart" Then
                    Session("AxisYM") = ysels(0)
                End If
                DropDownListM.Items.Clear()
                DropDownListM.Items.Add(" ")
                DropDownListM.Text = " "
                DropDownColumnsX.Items.Clear()
                DropDownColumnsX.Items.Add(" ")
                DropDownColumnsY.Items.Clear()
                DropDownColumnsY.Items.Add(" ")
                For i = 0 To ddt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = ddt.Columns(i).Caption
                    fldtype = ddt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    DropDownListM.Items.Add(li)

                    For j = 0 To xsels.Length - 1
                        li = New ListItem
                        li.Text = frdname
                        li.Value = fldname
                        If fldname = xsels(j) Then
                            li.Selected = True
                            Exit For
                        End If
                    Next
                    DropDownColumnsX.Items.Add(li)

                    For j = 0 To ysels.Length - 1
                        li = New ListItem
                        li.Text = frdname
                        li.Value = fldname
                        If fldname = ysels(j) Then
                            li.Selected = True
                            Exit For
                        End If
                    Next
                    DropDownColumnsY.Items.Add(li)
                Next
                DropDownListA.Items.Clear()
                DropDownListA.Items.Add("Count")
                DropDownListA.Items.Add("CountDistinct")
                Dim nselected As Integer = 0
                Dim bNumeric As Boolean = True
                For j = 0 To DropDownColumnsY.Items.Count - 1
                    If DropDownColumnsY.Items(j).Selected Then
                        nselected = nselected + 1
                        If nselected = 1 Then
                            Session("AxisYM") = DropDownColumnsY.Items(j).Text
                        Else
                            If charttype = "PieChart" Then
                                DropDownColumnsY.Items(j).Selected = False
                            Else
                                Session("AxisYM") = Session("AxisYM") & "," & DropDownColumnsY.Items(j).Text
                            End If

                        End If
                        If Not ColumnTypeIsNumeric(dv3.Table.Columns(DropDownColumnsY.Items(j).Text)) Then
                            bNumeric = False
                        End If
                    End If
                Next
                Session("nYselM") = nselected
                If (bNumeric AndAlso DropDownColumnsY.Text.Trim <> "") OrElse Session("nv") > 3 Then
                    DropDownListA.Items.Add("Sum")
                    DropDownListA.Items.Add("Max")
                    DropDownListA.Items.Add("Min")
                    DropDownListA.Items.Add("Avg")
                    DropDownListA.Items.Add("StDev")
                    DropDownListA.Items.Add("Value")
                    chkboxNumeric.Checked = True
                End If
                Session("nv") = DropDownListA.Items.Count
                If nselected > 1 OrElse charttype = "PieChart" Then
                    LabelM.Visible = False
                    LabelV.Visible = False
                    DropDownColumnsV.Visible = False
                    CheckBoxSelectAll.Visible = False
                    CheckBoxUnselectAll.Visible = False
                    CheckBoxSelectAll.Enabled = False
                    CheckBoxUnselectAll.Enabled = False
                    DropDownListM.Enabled = False
                    DropDownListM.Visible = False
                    Session("MFld") = " "
                    'Session("SELECTEDValuesM") = " "
                Else
                    LabelM.Visible = True
                    LabelV.Visible = True
                    DropDownColumnsV.Visible = True
                    CheckBoxSelectAll.Visible = True
                    CheckBoxUnselectAll.Visible = True
                    CheckBoxSelectAll.Enabled = True
                    CheckBoxUnselectAll.Enabled = True
                    DropDownListM.Enabled = True
                    DropDownListM.Visible = True
                End If

                DropDownColumnsX.Text = Session("AxisXM")
                DropDownColumnsY.Text = Session("AxisYM")
                DropDownListA.Text = Session("AggregateM")
                DropDownListA.SelectedValue = Session("AggregateM")

                If DropDownListM.Visible = True Then
                    If Not Session("MFld") Is Nothing AndAlso Session("MFld").ToString.Trim <> "" Then
                        DropDownListM.Text = Session("MFld")
                        DropDownColumnsV.Items.Clear()
                        DropDownColumnsV.Items.Add(" ")
                        Dim fld As String = DropDownListM.Text
                        If fld.Trim = "" Then
                            Exit Sub
                        End If
                        Session("mfld") = fld
                        dv3 = Session("dv3")
                        dv3.Sort = fld & " ASC"
                        Dim dq As DataTable = dv3.ToTable(True, fld)
                        For i = 0 To dq.Rows.Count - 1
                            If dq.Rows(i)(0).ToString.Trim = "" Then
                                Continue For
                            End If
                            If IsNumeric(dq.Rows(i)(0).ToString) Then
                                DropDownColumnsV.Items.Add(ExponentToNumber(dq.Rows(i)(0).ToString))
                            Else
                                DropDownColumnsV.Items.Add(dq.Rows(i)(0).ToString)
                            End If
                        Next
                    End If
                    If Not Session("SELECTEDValuesM") Is Nothing AndAlso Session("SELECTEDValuesM").ToString.Trim <> "" Then
                        DropDownColumnsV.Text = Session("SELECTEDValuesM")
                        Dim vsels As String = "," & Session("SELECTEDValuesM").ToString.Trim & ","
                        Dim vitem As String = String.Empty
                        For i = 0 To DropDownColumnsV.Items.Count - 1
                            vitem = DropDownColumnsV.Items(i).Text.Trim
                            If vitem = "" Then
                                Continue For
                            End If
                            vitem = "," & vitem & ","
                            If vsels.IndexOf(vitem) >= 0 Then
                                DropDownColumnsV.Items(i).Selected = True
                            End If
                        Next
                    End If
                End If
            End If
            If Not IsPostBack AndAlso Request("frm") = "Analytics" AndAlso Not Request("domulti") Is Nothing AndAlso Request("domulti").ToString.Trim = "yes" Then
                'Request not Session - we need it only once !!
                DropDownColumnsX.Text = Session("AxisXM")
                DropDownColumnsY.Text = Session("AxisYM")
                DropDownListA.Text = Session("AggregateM")
                DropDownListA.SelectedValue = Session("AggregateM")
                'DropDownListM.Text = Session("MFld")
                'DropDownColumnsV.Text = Session("SELECTEDValuesM")
                charttypeM = "MultiLineChart"
                btnShowChart_Click(sender, e)
            End If
            If Not x1 Is Nothing AndAlso Not x2 Is Nothing AndAlso Not y1 Is Nothing AndAlso x1 = x2 AndAlso x1 = y1 Then
                LabelError.Visible = True
                LabelError.Text = "Select chart parameters above and click the Show Chart button:"
                x1 = " "
                x2 = " "
                y1 = " "
                Session("cat1") = x1
                Session("cat2") = x2
                Session("AxisY") = y1
                Session("y1") = y1
                Session("arr") = ""
                arr = ""
                If Session("arr").ToString.Trim = "" Then
                    lnkbtnAddToDashboard.Enabled = False
                    lnkbtnAddToDashboard.Visible = False
                    lnkDownloadARR.Enabled = False
                    lnkDownloadARR.Visible = False
                End If
                If IsPostBack = False Then Session("newarr") = "yes"
                'Response.End()
                Exit Sub
            Else
                LabelError.Text = ""
            End If
        ElseIf charttypeM = "MapChart" OrElse charttypeM = "GeoChart" OrElse charttypeM = "MatrixChart" OrElse charttypeM = "Map" OrElse Session("MapChart") = "MapChart" OrElse Session("MatrixChart") = "MatrixChart" Then
            'no dropdowns
            LabelX.Visible = False
            LabelY.Visible = False
            LabelM.Visible = False
            LabelV.Visible = False
            DropDownColumnsX.Visible = False
            DropDownColumnsY.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            btnShowChart.Enabled = False
            btnShowChart.Visible = False
            LabelA.Visible = False
            DropDownListA.Enabled = False
            DropDownListA.Visible = False
        Else 'charttypeM <> "MultiLineChart" 
            LabelV.Visible = False
            LabelM.Visible = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            If Not IsPostBack Then
                'fill out drop downs
                Dim dv4 As New DataView
                dv4 = Session("dv3")
                'dv4.RowFilter = ""
                Dim ddt As DataTable = dv4.Table
                Dim fldname, frdname, fldtype As String

                DropDownColumnsX.Items.Clear()
                DropDownColumnsX.Items.Add(" ")
                DropDownColumnsY.Items.Clear()
                DropDownColumnsY.Items.Add(" ")
                For i = 0 To ddt.Columns.Count - 1
                    fldname = ""
                    frdname = ""
                    fldname = ddt.Columns(i).Caption
                    fldtype = ddt.Columns(i).DataType.ToString
                    If frdname = "" Then frdname = fldname
                    Dim li As ListItem
                    li = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    If Not Session("cat1") Is Nothing AndAlso fldname = Session("cat1") Then
                        li.Selected = True
                    End If
                    If Not Session("cat2") Is Nothing AndAlso fldname = Session("cat2") Then
                        li.Selected = True
                    End If
                    DropDownColumnsX.Items.Add(li)

                    li = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    If Not Session("AxisY") Is Nothing AndAlso fldname = Session("AxisY") Then
                        li.Selected = True
                    End If
                    DropDownColumnsY.Items.Add(li)
                    DropDownColumnsY.Text = Session("AxisY")
                Next
                DropDownColumnsX.Text = Session("cat1") & "," & Session("cat2")

                DropDownListA.Items.Clear()
                DropDownListA.Items.Add("Count")
                DropDownListA.Items.Add("CountDistinct")
                If (Session("AxisY").ToString.Trim <> "" AndAlso ColumnTypeIsNumeric(dv3.Table.Columns(Session("AxisY")))) OrElse Session("nv") > 3 Then
                    DropDownListA.Items.Add("Sum")
                    DropDownListA.Items.Add("Max")
                    DropDownListA.Items.Add("Min")
                    DropDownListA.Items.Add("Avg")
                    DropDownListA.Items.Add("StDev")
                    DropDownListA.Items.Add("Value")
                    chkboxNumeric.Checked = True
                End If
                DropDownListA.Text = Session("Aggregate")
                Session("nv") = DropDownListA.Items.Count
            End If
        End If

        Dim dri As DataTable = GetReportInfo(repid)
        If dri Is Nothing OrElse dri.Rows.Count = 0 Then
            Exit Sub
        End If
        If dri.Rows(0)("Param7type").ToString.Trim = "standard" Then
            lnkbtnAddToDashboard.Enabled = False
            lnkbtnAddToDashboard.Visible = False
            lnkDownloadARR.Enabled = False
            lnkDownloadARR.Visible = False
        End If

        bUseSQLText = (Not IsDBNull(dri.Rows(0)("Param8type")) AndAlso dri.Rows(0)("Param8type").ToString.ToUpper = "USESQLTEXT")


        If Not IsPostBack AndAlso Session("arr") IsNot Nothing AndAlso Session("arr").ToString.Trim <> "" AndAlso (Not Request("domulti") Is Nothing AndAlso Request("domulti").ToString.Trim = "yes") Then
            arr = Session("arr")
            ttl = Session("ttl")
            srt = Session("srt")
            LabelWhere.Text = Session("REPTITLE").ToString & ":            --- " & ttl & " ---     " & Session("WhereStm").ToString.Trim
            writetext = LabelWhere.Text
            writetext = writetext & Chr(10) & Session("arr").ToString
            Session("writetext") = writetext
            Exit Sub
        End If


        If Not IsPostBack AndAlso Session("arr") IsNot Nothing AndAlso Session("arr").ToString.Trim <> "" AndAlso Not Request("das") Is Nothing AndAlso Request("das").ToString.Trim = "yes" Then
            'chartregn = Request("chartregn")
            'ttl = Request("ttl").ToString

            Dim nr As Integer = arr.Split("]").Length
            'chartwidth, chartheght
            chartwidth = 1900
            If charttype = "BarChart" Then
                chartheght = (10 * nr)
            ElseIf charttype = "Sankey" Then
                chartheght = (5 * nr)
            Else
                chartheght = 870
            End If
            If chartheght < 850 Then
                chartheght = 850
            End If
            If charttype = "ColumnChart" Then
                chartwidth = nr * 10
                If chartwidth < 1900 Then chartwidth = 1900
            End If

            arr = Session("arr")
            ttl = Session("ttl")
            srt = Session("srt")
            LabelWhere.Text = Session("REPTITLE").ToString & ":            --- " & ttl & " ---     " & Session("WhereStm").ToString.Trim.Replace("^", "'")
            writetext = LabelWhere.Text
            writetext = writetext & Chr(10) & Session("arr").ToString
            Session("writetext") = writetext
            If Session("arr").ToString.Trim = "" Then
                lnkbtnAddToDashboard.Enabled = False
                lnkbtnAddToDashboard.Visible = False
                lnkDownloadARR.Enabled = False
                lnkDownloadARR.Visible = False
            End If
            Exit Sub
        End If

        Dim msql As String = String.Empty
        Dim grp As String = String.Empty
        Dim sqlq As String = String.Empty


        ''----------------------------------  NO MAP , NO MATRIX------------------------------------------------------
        'charttype = DropDownChartType.SelectedItem.Text
        LabelWhere.Text = Session("REPTITLE").ToString & ":              --- " & Session("WhereStm").ToString.Trim

        Dim nrec As Integer = 0
        If Session("nrec") IsNot Nothing AndAlso IsNumeric(Session("nrec").ToString) Then
            nrec = CInt(Session("nrec").ToString)
        End If
        Try
            Dim er As String = String.Empty
            Dim rt As String = String.Empty


            '---------------------NOT MULTIPLE ----------------------------------------------------------------------------------------
            If IsPostBack = False OrElse Session("arr").ToString.Trim = "" OrElse Session("newarr") = "yes" Then
                'calc arr for other Charts !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                If x1 = " " OrElse x2 = " " OrElse y1 = " " Then
                    Exit Sub
                End If
                If y1 Is Nothing OrElse y1.Trim = "" Then
                    y1 = DropDownColumnsY.Text
                End If
                If y1.Trim = "" Then
                    Exit Sub
                End If
                Dim selflds As String = x1 & "," & x2 & "," & y1
                selflds = FixSelectedFields(repid, selflds, Session("UserConnString"), Session("UserConnProvider"))
                Dim xx1, xx2, yy1, ssrt As String
                xx1 = Piece(selflds, ",", 1)
                xx2 = Piece(selflds, ",", 2)
                yy1 = Piece(selflds, ",", 3)
                ssrt = xx1 & ", " & xx2
                srt = x1 & ", " & x2
                If x1 = x2 Then
                    srt = x1
                    lnkbtnReverse.Visible = False
                    lnkbtnReverse.Enabled = False
                Else
                    lnkbtnReverse.Visible = True
                    lnkbtnReverse.Enabled = True
                End If

                If x1.Trim <> "" AndAlso x2.Trim <> "" AndAlso x1 <> x2 Then
                    rt = AddGroupBy(Session("REPORTID"), x1, x2, "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
                End If

                'data from sql 
                If UCase(dri.Rows(0)("ReportAttributes").ToString) = "SQL" AndAlso bUseSQLText = False Then
                    'sql
                    sqlq = dri.Rows(0)("SQLquerytext").ToString
                    'If sqlq.ToUpper.IndexOf(" WHERE ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" WHERE "))
                    If sqlq.ToUpper.IndexOf(" GROUP BY ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" GROUP BY "))
                    If sqlq.ToUpper.IndexOf(" ORDER BY ") > 0 Then sqlq = sqlq.Substring(0, sqlq.ToUpper.IndexOf(" ORDER BY "))
                    sqlq = sqlq.Substring(sqlq.ToUpper.IndexOf(" FROM "))
                    'Session("WhereStm") 'parameters and searh
                    'wherestms - conditions
                    Dim wherestms As String = String.Empty
                    If sqlq.ToUpper.IndexOf(" WHERE ") > 0 Then
                        wherestms = sqlq.Substring(sqlq.ToUpper.IndexOf(" WHERE ") + 6)
                    End If
                    If wherestms.Trim <> "" Then
                        If Session("WhereStm").ToString.Trim <> "" Then
                            'TODO other providers if needed
                            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data.") Then
                                sqlq = sqlq & " AND " & Session("WhereStm").ToString.Trim.Replace("^", "'").Replace("`", "").Replace("[", "").Replace("]", "")
                            Else
                                sqlq = sqlq & " AND " & Session("WhereStm").ToString.Trim.Replace("^", "'")
                            End If

                        End If
                    Else
                        If Session("WhereStm").ToString.Trim <> "" Then
                            'TODO other providers if needed
                            If Session("UserConnProvider").ToString.StartsWith("InterSystems.Data.") Then
                                sqlq = sqlq & " WHERE " & Session("WhereStm").ToString.Trim.Replace("^", "'").Replace("`", "").Replace("[", "").Replace("]", "")
                            Else
                                sqlq = sqlq & " WHERE " & Session("WhereStm").ToString.Trim.Replace("^", "'")
                            End If

                        End If
                    End If

                    sqlq = sqlq & " ORDER BY " & ssrt

                    If fn = "Value" Then
                        ttl = "Value of [" & y1 & "] in group by [" & srt & "]"
                        msql = "SELECT DISTINCT " & ssrt & "," & yy1 & " AS ARR "
                    ElseIf fn = "Count" Then
                        ttl = "Count of records in group by [" & srt & "]"
                        msql = "SELECT " & ssrt & ",Count(" & yy1 & ") AS ARR "
                    ElseIf fn = "CountDistinct" Then
                        ttl = "Distinct Count of [" & y1 & "] in group by [" & srt & "]"
                        msql = "SELECT " & ssrt & ",Count(Distinct " & yy1 & ") AS ARR "

                    ElseIf fn = "Sum" Then
                        ttl = "Sum of [" & y1 & "] in group by [" & srt & "]"
                        msql = "SELECT " & ssrt & ",Sum(" & yy1 & ") AS ARR "

                    ElseIf fn = "Avg" Then
                        ttl = "Avg of [" & y1 & "] in group by [" & srt & "]"
                        msql = "SELECT " & ssrt & ", AVG(" & yy1 & ") AS ARR "

                    ElseIf fn = "StDev" Then
                        ttl = "StDev of [" & y1 & "] in group by [" & srt & "]"

                        'TODO other providers
                        Dim std As String = String.Empty
                        If Session("UserConnProvider") = "System.Data.SqlClient" Then 'SQL Server
                            std = "STDEVP"
                        Else 'All others
                            std = "STDDEV"
                        End If

                        msql = "SELECT " & ssrt & ", " & std & "(" & yy1 & ") AS ARR "

                    ElseIf fn = "Max" Then
                        ttl = "Max of [" & y1 & "] in group by [" & srt & "]"
                        msql = "SELECT " & ssrt & ", MAX(" & yy1 & ") AS ARR "

                    ElseIf fn = "Min" Then
                        ttl = "Min of [" & y1 & "] in group by [" & srt & "]"
                        msql = "SELECT " & ssrt & ", MIN(" & yy1 & ") AS ARR "
                    End If
                    Session("ttl") = ttl
                    msql = msql & sqlq   '!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    msql = msql.Replace(" ORDER BY ", " GROUP BY " & ssrt & " ORDER BY ")

                    Try
                        dt = mRecords(msql, ret, Session("UserConnString"), Session("UserConnProvider")).ToTable
                        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                            LabelError.Text = ret
                            Exit Sub
                        End If
                        'ttl = ttl & " (" & dt.Rows.Count.ToString & " records)"

                    Catch ex As Exception
                        LabelError.Text = "ERROR!! " & ret & " - " & ex.Message
                    End Try



                    'Stored procedure or UseSQLtext---------------------------------------------------------------------------------------------
                ElseIf y1.Trim <> "" Then  'SP
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
                    'ttl = ttl & " (" & dt.Rows.Count.ToString & " records)"
                    Session("ttl") = ttl
                    'dv3 retrieved already!!!
                    If fn = "Value" Then
                        Dim n As Integer
                        dt = dv3.ToTable
                        Dim coln As New DataColumn
                        coln.DataType = System.Type.GetType("System.String")
                        coln.ColumnName = "ARR"
                        dt.Columns.Add(coln)
                        For n = 0 To dt.Rows.Count - 1
                            If dt.Rows(n)(y1).ToString.Trim = "" Then
                                dt.Rows(n)("ARR") = "0"
                            Else
                                dt.Rows(n)("ARR") = dt.Rows(n)(y1)
                            End If
                        Next
                    Else
                        dt = ComputeStats(dv3.Table, fn, y1, x1, x2, "", ret, Session("UserConnString"), Session("UserConnProvider"))
                    End If
                Else
                End If

                'we have dt to calc arr  ------------------------------------------------------------------------------------------------
                arr = ""
                Dim mcolors() As String = {"blue", "gold", "lightgreen", "red", "yellow", "darkred", "lightblue", "green", "#e5e4e2", "orange", "darkgreen", "pink", "darkblue", "lightyellow", "maroon", "salmon", "darkorange"}
                Dim k As Integer = 0
                Dim i As Integer = 0
                Dim j As Integer = 0
                Dim lastk As Integer = 0
                Dim maxvalue As Integer = 1
                Dim col As New DataColumn
                col.DataType = System.Type.GetType("System.String")
                col.ColumnName = "x2color"
                Try
                    dt.Columns.Add(col)
                Catch ex As Exception
                End Try

                If charttype = "Gauge" Then
                    chartp1 = 90
                    chartp2 = 100
                    chartp3 = 75
                    chartp4 = 90
                    For i = 0 To dt.Rows.Count - 1
                        If CInt(dt.Rows(i)("ARR").ToString) > maxvalue Then
                            maxvalue = CInt(dt.Rows(i)("ARR").ToString)
                        End If
                    Next

                End If

                For i = 0 To dt.Rows.Count - 1
                    If charttype = "CandlestickChart" Then
                        If i = 0 Then
                            arr = arr & "["
                            arr = arr & "'" & dt.Rows(i)(x1).ToString.Replace(",", ";") & "'"
                            arr = arr & "," & "0" 'dt.Rows(i)("ARR").ToString
                            arr = arr & "," & "0" ' & dt.Rows(i)("ARR").ToString
                        ElseIf i > 0 AndAlso dt.Rows(i)(x1).ToString = dt.Rows(i - 1)(x1).ToString Then
                            'arr = arr & "," & dt.Rows(i)("ARR").ToString
                        ElseIf i > 0 AndAlso dt.Rows(i)(x1).ToString <> dt.Rows(i - 1)(x1).ToString Then
                            arr = arr & "," & dt.Rows(i - 1)("ARR").ToString
                            arr = arr & "," & dt.Rows(i - 1)("ARR").ToString
                            arr = arr & "],["
                            arr = arr & "'" & dt.Rows(i)(x1).ToString.Replace(",", ";") & "'"
                            arr = arr & "," & "0" ' & dt.Rows(i)("ARR").ToString
                            arr = arr & "," & "0" ' & dt.Rows(i)("ARR").ToString
                        End If
                        If i = dt.Rows.Count - 1 Then
                            arr = arr & "," & dt.Rows(i)("ARR").ToString
                            arr = arr & "," & dt.Rows(i)("ARR").ToString
                            arr = arr & "]"
                        End If
                    ElseIf charttype = "Sankey" Then
                        arr = arr & "['" & dt.Rows(i)(x1).ToString.Replace(",", ";") & "','" & dt.Rows(i)(x2).ToString.Replace(",", ";") & "'," & dt.Rows(i)("ARR").ToString & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","

                    ElseIf charttype = "Gauge" Then
                        If x1 = x2 Then
                            grp = dt.Rows(i)(x1).ToString.Replace(",", ";")
                        Else
                            grp = dt.Rows(i)(x1).ToString.Replace(",", ";") & "," & dt.Rows(i)(x2).ToString.Replace(",", ";")
                        End If
                        arr = arr & "['" & grp & ", " & ExponentToNumber(dt.Rows(i)("ARR").ToString).ToString & "',"
                        arr = arr & 100 * Round(CInt(dt.Rows(i)("ARR").ToString) / maxvalue, 5)
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","

                    Else  'other charts
                        If x1 = x2 Then
                            grp = dt.Rows(i)(x1).ToString.Replace(",", ";")
                        Else
                            grp = dt.Rows(i)(x1).ToString.Replace(",", ";") & "," & dt.Rows(i)(x2).ToString.Replace(",", ";")
                        End If
                        arr = arr & "['" & grp & "'," & dt.Rows(i)("ARR").ToString

                        If charttype = "BubbleChart" Then
                            arr = arr & "," & dt.Rows(i)("ARR").ToString & ",'" & grp & "'"

                        Else
                            For j = 0 To i
                                If dt.Rows(j)("x2color").ToString.Trim <> "" AndAlso dt.Rows(j)(x2).ToString = dt.Rows(i)(x2).ToString Then
                                    dt.Rows(i)("x2color") = dt.Rows(j)("x2color")
                                    Exit For
                                End If
                            Next
                            If IsDBNull(dt.Rows(i)("x2color")) OrElse dt.Rows(i)("x2color").ToString.Trim = "" Then
                                k = lastk Mod mcolors.Length
                                dt.Rows(i)("x2color") = mcolors(k)
                                lastk = k + 1
                            End If
                            'arr = arr & ",'" & mcolors(k) & "'"
                            arr = arr & ",'" & dt.Rows(i)("x2color") & "'"
                        End If
                        arr = arr & "]"
                        If i < dt.Rows.Count - 1 Then arr = arr & ","
                    End If

                Next
                If charttype = "BubbleChart" Then
                    arr = "['" & srt & "','" & y1 & "','" & y1 & "','" & srt & "']," & arr
                ElseIf charttype = "Sankey" Then
                    arr = "['From','To','" & y1 & "']," & arr
                ElseIf charttype = "Gauge" Then
                    arr = "['Label','Value']," & arr
                ElseIf charttype = "CandlestickChart" Then
                    Dim m As Integer = 4
                    Dim brr As String = String.Empty
                    Dim arrs() As String = Split(arr, "],[")
                    Dim aris() As String
                    For i = 0 To arrs.Length - 1
                        aris = arrs(i).Split(",")
                        brr = brr & aris(0)
                        For j = aris.Length - m To aris.Length - 1
                            brr = brr & "," & aris(j)
                        Next
                        If i < arrs.Length - 1 Then brr = brr & "],["
                    Next
                    arr = brr
                    chartwidth = arrs.Length * 100
                    If chartwidth < 1900 Then chartwidth = 1900
                Else
                    arr = "['" & srt & "','" & y1 & "',{ role: 'style' }]," & arr
                End If

                nrec = dt.Rows.Count
                Session("nrec") = nrec
                Session("arr") = arr
                If Session("arr").ToString.Trim = "" Then
                    lnkbtnAddToDashboard.Enabled = False
                    lnkbtnAddToDashboard.Visible = False
                    lnkDownloadARR.Enabled = False
                    lnkDownloadARR.Visible = False
                End If
                Session("ttl") = ttl & " (" & nrec.ToString & " records)"
                Session("y1") = y1
                Session("srt") = srt
                Session("AxisY") = y1
            End If

            If charttype = "BubbleChart" OrElse charttype = "CandlestickChart" OrElse charttype = "Sankey" OrElse charttype = "Gauge" Then
                Session("newarr") = "yes"
            Else
                Session("newarr") = "no"
            End If
            'End If
            If charttype = "BarChart" Then
                chartheght = (10 * nrec)
            ElseIf charttype = "Sankey" Then
                chartheght = (5 * nrec)
            Else
                chartheght = 850
            End If
            If chartheght < 850 Then
                chartheght = 850
            End If
            If charttype = "ColumnChart" Then
                chartwidth = nrec * 10
                If chartwidth < 1900 Then chartwidth = 1900
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

            LabelWhere.Text = Session("REPTITLE").ToString & ":             ---" & Session("ttl") & " ---   " & Session("WhereStm").ToString.Trim.Replace("^", "'")

            writetext = LabelWhere.Text
            writetext = writetext & Chr(10) & Session("arr").ToString
            Session("writetext") = writetext
            If Request("map") IsNot Nothing AndAlso Request("map").ToString = "yes" Then
                HyperLinkDataAI.NavigateUrl = "DataAI.aspx?pg=maps"
            Else
                HyperLinkDataAI.NavigateUrl = "DataAI.aspx?pg=charts"
            End If
            If Session("arr").ToString.Trim = "" Then
                lnkbtnAddToDashboard.Enabled = False
                lnkbtnAddToDashboard.Visible = False
                lnkDownloadARR.Enabled = False
                lnkDownloadARR.Visible = False
                HyperLinkDataAI.Enabled = False
                HyperLinkDataAI.Visible = False
            Else
                HyperLinkDataAI.Enabled = True
                HyperLinkDataAI.Visible = True
            End If

        Catch ex As Exception
            ret = ex.Message
        End Try
    End Sub

    Private Sub LinkButtonBack_Click(sender As Object, e As EventArgs) Handles LinkButtonBack.Click
        'from Analytics, from ListOfReports
        If Session("frm") = "Analytics" Then
            Response.Redirect("ShowReport.aspx?srd=11&REPORT=" & Session("REPORTID"))
        ElseIf Session("frm") = "ListOfReports" Then
            Response.Redirect("ListOfReports.aspx")
        ElseIf Not Session("UserDash") Is Nothing AndAlso Session("UserDash").ToString.Trim <> "" Then  'came from user dashboard
            If Not Session("dash") Is Nothing AndAlso Session("dash") = "yes" Then  'came from sharing dashboard
                Response.Redirect("Dashboard.aspx?dash=yes&Prop6=" & Session("logon") & "&dashboard=" & Session("UserDash"))
            Else  'came from list of dashboards
                Response.Redirect("Dashboard.aspx?user=" & Session("logon") & "&dashboard=" & Session("UserDash"))
            End If
        ElseIf Not Session("StatDash") Is Nothing AndAlso Session("StatDash").ToString.Trim = "yes" Then  'came from stats dashboard
            Response.Redirect("ChartGoogle.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat1").ToString & "&x2=" & Session("cat2").ToString & "&y1=" & Session("AxisY").ToString & "&fn=" & Session("Aggregate"))
        ElseIf (charttype = "MapChart" OrElse charttype = "GeoChart") AndAlso (Session("admin") = "admin" OrElse Session("admin") = "super") Then
            Response.Redirect("MapReport.aspx?Report=" & Session("REPORTID"))
        Else
            'Response.Redirect("ReportViews.aspx?Report=" & Session("REPORTID") & "&see=yes")
            Response.Redirect("ReportViews.aspx?see=yes")
        End If

    End Sub

    Private Sub DropDownChartType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownChartType.SelectedIndexChanged
        charttype = DropDownChartType.SelectedItem.Text
        Session("ChartType") = charttype
        LabelError.Text = ""
        If charttype = "BubbleChart" OrElse charttype = "CandlestickChart" OrElse charttype = "Sankey" OrElse charttype = "Gauge" Then
            Session("arr") = ""
            arr = ""
            ttl = ""
            charttypeM = charttype
        ElseIf Session("MatrixChart") = "MatrixChart" Then
            charttypeM = "MatrixChart"
        ElseIf Session("MapChart") = "MapChart" Then
            charttypeM = "MapChart"
        Else
            charttypeM = "MultiLineChart"
        End If
        If charttypeM = "MultiLineChart" Then
            'fill out drop downs
            If IsPostBack Then
                Session("AxisXM") = DropDownColumnsX.Text
                Session("AxisYM") = DropDownColumnsY.Text
                Session("AggregateM") = DropDownListA.Text
            End If
            Dim dv4 As New DataView
            dv4 = Session("dv3")
            'dv4.RowFilter = ""
            'dropdowns
            Dim ddt As DataTable = dv4.Table
            Dim fldname, frdname, fldtype As String
            Dim xsels() As String = Split(Session("AxisXM"), ",")
            Dim ysels() As String = Split(Session("AxisYM"), ",")
            If charttype = "PieChart" Then
                Session("AxisYM") = ysels(0)
            End If
            DropDownListM.Items.Clear()
            DropDownListM.Items.Add(" ")
            DropDownColumnsX.Items.Clear()
            DropDownColumnsX.Items.Add(" ")
            DropDownColumnsY.Items.Clear()
            DropDownColumnsY.Items.Add(" ")
            For i = 0 To ddt.Columns.Count - 1
                fldname = ""
                frdname = ""
                fldname = ddt.Columns(i).Caption
                fldtype = ddt.Columns(i).DataType.ToString
                If frdname = "" Then frdname = fldname
                Dim li As ListItem = New ListItem
                li.Text = frdname
                li.Value = fldname
                DropDownListM.Items.Add(li)

                For j = 0 To xsels.Length - 1
                    li = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    If fldname = xsels(j) Then
                        li.Selected = True
                        Exit For
                    End If
                Next
                DropDownColumnsX.Items.Add(li)

                For j = 0 To ysels.Length - 1
                    li = New ListItem
                    li.Text = frdname
                    li.Value = fldname
                    If fldname = ysels(j) Then
                        li.Selected = True
                        Exit For
                    End If
                Next
                DropDownColumnsY.Items.Add(li)
            Next
            DropDownListA.Items.Clear()
            DropDownListA.Items.Add("Count")
            DropDownListA.Items.Add("CountDistinct")
            Dim nselected As Integer = 0
            Dim bNumeric As Boolean = True
            For j = 0 To DropDownColumnsY.Items.Count - 1
                If DropDownColumnsY.Items(j).Selected Then
                    nselected = nselected + 1
                    If nselected = 1 Then
                        Session("AxisYM") = DropDownColumnsY.Items(j).Text
                    Else
                        If charttype = "PieChart" Then
                            DropDownColumnsY.Items(j).Selected = False
                        Else
                            Session("AxisYM") = Session("AxisYM") & "," & DropDownColumnsY.Items(j).Text
                        End If
                    End If
                    If Not ColumnTypeIsNumeric(dv3.Table.Columns(DropDownColumnsY.Items(j).Text)) Then
                        bNumeric = False
                    End If
                End If
            Next
            Session("nYselM") = nselected
            If (bNumeric AndAlso DropDownColumnsY.Text.Trim <> "") OrElse Session("nv") > 3 Then
                DropDownListA.Items.Add("Sum")
                DropDownListA.Items.Add("Max")
                DropDownListA.Items.Add("Min")
                DropDownListA.Items.Add("Avg")
                DropDownListA.Items.Add("StDev")
                DropDownListA.Items.Add("Value")
                chkboxNumeric.Checked = True
            End If
            Session("nv") = DropDownListA.Items.Count
            If nselected > 1 OrElse charttype = "PieChart" Then
                LabelM.Visible = False
                LabelV.Visible = False
                DropDownColumnsV.Visible = False
                CheckBoxSelectAll.Visible = False
                CheckBoxUnselectAll.Visible = False
                CheckBoxSelectAll.Enabled = False
                CheckBoxUnselectAll.Enabled = False
                DropDownListM.Enabled = False
                DropDownListM.Visible = False
                'Session("MFld") = " "
                'Session("SELECTEDValuesM") = ""
            Else
                LabelM.Visible = True
                LabelV.Visible = True
                DropDownColumnsV.Visible = True
                CheckBoxSelectAll.Visible = True
                CheckBoxUnselectAll.Visible = True
                CheckBoxSelectAll.Enabled = True
                CheckBoxUnselectAll.Enabled = True
                DropDownListM.Enabled = True
                DropDownListM.Visible = True

            End If

            DropDownColumnsX.Text = Session("AxisXM")
            DropDownColumnsY.Text = Session("AxisYM")
            DropDownListA.Text = Session("AggregateM")
            DropDownListA.SelectedValue = Session("AggregateM")

            If Not Session("MFld") Is Nothing AndAlso Session("MFld").ToString.Trim <> "" Then DropDownListM.Text = Session("MFld")
            If Not Session("SELECTEDValuesM") Is Nothing AndAlso Session("SELECTEDValuesM").ToString.Trim <> "" Then DropDownColumnsV.Text = Session("SELECTEDValuesM")

        ElseIf charttypeM = "MatrixChart" OrElse charttypeM = "MapChart" OrElse Session("MapChart") = "MapChart" OrElse Session("MatrixChart") = "MatrixChart" Then
            'no dropdowns
            LabelX.Visible = False
            LabelY.Visible = False
            LabelM.Visible = False
            LabelV.Visible = False
            DropDownColumnsX.Visible = False
            DropDownColumnsY.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            btnShowChart.Enabled = False
            btnShowChart.Visible = False
            LabelA.Visible = False
            DropDownListA.Enabled = False
            DropDownListA.Visible = False
            If charttypeM = "MapChart" OrElse Session("MapChart") = "MapChart" Then
                Response.Redirect("ChartGoogleOne.aspx?map=yes&Report=" & Session("REPORTID") & "&mapname=" & mapname)
            End If

        Else 'If charttypeM <> "MultiLineChart" Then 
                'fill out drop downs
                Dim dv4 As New DataView
            dv4 = Session("dv3")
            'dv4.RowFilter = ""
            'dropdowns
            Dim ddt As DataTable = dv4.Table
            Dim fldname, frdname, fldtype As String

            LabelV.Visible = False
            LabelM.Visible = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False


            DropDownColumnsX.Items.Clear()
            DropDownColumnsX.Items.Add(" ")
            DropDownColumnsY.Items.Clear()
            DropDownColumnsY.Items.Add(" ")
            For i = 0 To ddt.Columns.Count - 1
                fldname = ""
                frdname = ""
                fldname = ddt.Columns(i).Caption
                fldtype = ddt.Columns(i).DataType.ToString
                If frdname = "" Then frdname = fldname
                Dim li As ListItem
                li = New ListItem
                li.Text = frdname
                li.Value = fldname
                If fldname = Session("cat1") Then
                    li.Selected = True
                End If
                If fldname = Session("cat2") Then
                    li.Selected = True
                End If
                DropDownColumnsX.Items.Add(li)
                DropDownColumnsX.Text = Session("cat1") & "," & Session("cat2")

                li = New ListItem
                li.Text = frdname
                li.Value = fldname
                If fldname = Session("AxisY") Then
                    li.Selected = True
                End If
                DropDownColumnsY.Items.Add(li)
                DropDownColumnsY.Text = Session("AxisY")
            Next
            DropDownListA.Items.Clear()
            DropDownListA.Items.Add("Count")
            DropDownListA.Items.Add("CountDistinct")
            If Session("AxisY").ToString.Trim <> "" AndAlso ColumnTypeIsNumeric(dv3.Table.Columns(Session("AxisY"))) Then
                DropDownListA.Items.Add("Sum")
                DropDownListA.Items.Add("Max")
                DropDownListA.Items.Add("Min")
                DropDownListA.Items.Add("Avg")
                DropDownListA.Items.Add("StDev")
                DropDownListA.Items.Add("Value")
                chkboxNumeric.Checked = True
            End If
            Session("nv") = DropDownListA.Items.Count
            DropDownListA.Text = Session("Aggregate")
            Response.Redirect("ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat1") & "&x2=" & Session("cat2") & "&y1=" & Session("AxisY") & "&fn=" & Session("AggregateM") & "&charttype=" & charttype)

        End If

    End Sub

    Private Sub DoAddToDashboard(Dashboards As String)
        Dim dashnames As String()
        Dim ret As String = String.Empty
        If Dashboards <> String.Empty Then
            dashnames = Dashboards.Split(","c)
        Else
            ReDim dashnames(0)
            dashnames(0) = "Dashboard " & Session("logon")
        End If
        For i As Integer = 0 To dashnames.Length - 1
            ret = AddToDashboard(dashnames(i))
        Next
        Response.Redirect("ListOfDashboards.aspx")
    End Sub
    Private Sub lnkbtnAddToDashboard_Click(sender As Object, e As EventArgs) Handles lnkbtnAddToDashboard.Click
        'No longer used. See DoAddToDashboards
        Dim i As Integer
        Dim ret As String = String.Empty
        Dim dashnames(0) As String
        dashnames(0) = "Dashboard " & Session("logon")
        'open dialog with dashbords dropdown and txtbox to get array of dashnames
        For i = 0 To dashnames.Length - 1
            ret = AddToDashboard(dashnames(i))
        Next
        Response.Redirect("ListOfDashboards.aspx")
    End Sub

    Private Sub lnkbtnReverse_Click(sender As Object, e As EventArgs) Handles lnkbtnReverse.Click
        Response.Redirect("ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat2") & "&x2=" & Session("cat1") & "&y1=" & Session("AxisY") & "&fn=" & Session("Aggregate") & "&charttype=" & charttype)
    End Sub

    Private Sub LinkButtonData_Click(sender As Object, e As EventArgs) Handles LinkButtonData.Click
        Response.Redirect("ReportViews.aspx?see=yes")
    End Sub

    Private Function AddToDashboard(ByVal DashBoardName As String) As String
        Dim ret As String = String.Empty
        Dim sqld As String = String.Empty
        Try
            sqld = "SELECT * FROM ourdashboards WHERE "
            sqld = sqld & " UserID ='" & Session("logon") & "' "
            sqld = sqld & " AND Dashboard='" & DashBoardName & "' "
            sqld = sqld & " AND ReportID='" & Session("REPORTID") & "' "
            sqld = sqld & " AND ChartType='" & charttype & "' "
            sqld = sqld & " AND MapName='" & Session("mapname") & "' "
            sqld = sqld & " AND x1='" & Session("cat1") & "' "
            sqld = sqld & " AND x2='" & Session("cat2") & "' "
            sqld = sqld & " AND y1='" & Session("AxisY") & "' "
            sqld = sqld & " AND fn1='" & Session("Aggregate") & "' "
            sqld = sqld & " AND WhereStm='" & Session("WhereStm").ToString.Trim.Replace("'", "^") & "' "
            sqld = sqld & " AND GraphTitle='" & Session("ttl") & "' "
            sqld = sqld & " AND MapYesNo='" & Session("mapyesno") & "' "

            If Not HasRecords(sqld) Then
                'insert
                Dim sFieldList As String = "UserID,Dashboard,ReportID,ChartType,MapName,x1,x2,y1,fn1,WhereStm,GraphTitle,MapYesNo"
                Dim sValues As String = "'" & Session("logon") & "',"
                sValues &= "'" & DashBoardName & "',"
                sValues &= "'" & Session("REPORTID") & "',"
                sValues &= "'" & charttype & "',"
                sValues &= "'" & Session("mapname") & "',"
                sValues &= "'" & Session("cat1") & "',"
                sValues &= "'" & Session("cat2") & "',"
                sValues &= "'" & Session("AxisY") & "',"
                sValues &= "'" & Session("Aggregate") & "',"
                sValues &= "'" & Session("WhereStm").ToString.Trim.Replace("'", "^") & "',"
                If Session("ttl").ToString.Length > 200 Then
                    sValues &= "'" & Session("ttl").ToString.Substring(0, 200) & "...',"
                Else
                    sValues &= "'" & Session("ttl").ToString & "',"
                End If
                sValues &= "'" & Session("mapyesno") & "'"

                If Session("mapyesno").ToString.Trim = "yes" AndAlso Session("arr").ToString.Trim <> "" Then
                    sFieldList = sFieldList & ",ARR,Prop2"
                    sValues &= ",""" & Session("arr").ToString.Replace("'", "^^").Replace("[", "**").Replace("]", "##") & """,""" & Session("chartregn").ToString.Trim & """"
                End If
                If Session("mapyesno").ToString.Trim <> "yes" Then
                    Dim xsels() As String = Split(Session("AxisXM"), ",")
                    Dim ysels() As String = Split(Session("AxisYM"), ",")
                    'If xsels.Length > 2 Then
                    sFieldList = sFieldList & ",Prop3"
                    sValues &= ",'" & Session("AxisXM") & "'"
                    'End If
                    'If ysels.Length > 1 Then
                    sFieldList = sFieldList & ",y2"
                    sValues &= ",'" & Session("AxisYM") & "'"
                    'End If

                    If ysels.Length = 1 AndAlso Not Session("SELECTEDValuesM") Is Nothing AndAlso Session("SELECTEDValuesM").ToString.Trim <> "" AndAlso Not Session("MFld") Is Nothing AndAlso Session("MFld").ToString.Trim <> "" Then
                        sFieldList = sFieldList & ",Prop4,Prop5"
                        If Session("SELECTEDValuesM").ToString.Length > 200 Then
                            sValues &= ",'" & Session("MFld").ToString.Trim & "','" & Session("SELECTEDValuesM").ToString.Trim.Substring(0, 200) & "...'"
                        Else
                            sValues &= ",'" & Session("MFld").ToString.Trim & "','" & Session("SELECTEDValuesM").ToString.Trim & "'"
                        End If

                    End If
                End If
                sqld = "INSERT INTO ourdashboards (" & sFieldList & ") VALUES (" & sValues & ")"

                ret = ExequteSQLquery(sqld)

            End If
            Dim lgn As String = GetDashboardIdentifier(DashBoardName, Session("logon"))

        Catch ex As Exception
            ret = "ERROR!! " & ex.Message
        End Try
        Return ret
    End Function
    Protected Sub CheckBoxSelectAllFields_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSelectAll.CheckedChanged
        Dim j As Integer
        Session("SELECTEDValuesM") = ""
        If CheckBoxSelectAll.Checked Then
            CheckBoxUnselectAll.Checked = False
            For j = 0 To DropDownColumnsV.Items.Count - 1   'all fields in drop-down selected
                If DropDownColumnsV.Items(j).Text.ToString.Trim = "" Then
                    Continue For
                End If
                DropDownColumnsV.Items(j).Selected = True
                If Session("SELECTEDValuesM") = "" Then
                    Session("SELECTEDValuesM") = DropDownColumnsV.Items(j).Text
                Else
                    Session("SELECTEDValuesM") = Session("SELECTEDValuesM") & "," & DropDownColumnsV.Items(j).Text
                End If
            Next
            Session("AllSelected") = "yes"
            Session("AllUnSelected") = ""
        End If
        DropDownColumnsV.Text = Session("SELECTEDValuesM")
    End Sub
    Protected Sub CheckBoxUnselectAllFields_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxUnselectAll.CheckedChanged
        Dim j As Integer
        Session("SELECTEDValuesM") = ""
        If CheckBoxUnselectAll.Checked Then
            CheckBoxSelectAll.Checked = False
            Session("AllSelected") = ""
            Session("AllUnSelected") = "yes"
            For j = 0 To DropDownColumnsV.Items.Count - 1   'draw drop-down start
                DropDownColumnsV.Items(j).Selected = False
            Next
        End If
        DropDownColumnsV.Text = Session("SELECTEDValuesM")
    End Sub

    Private Sub DropDownListM_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListM.SelectedIndexChanged
        Session("MFld") = DropDownListM.Text
        DropDownColumnsV.Items.Clear()
        DropDownColumnsV.Items.Add(" ")
        Dim fld As String = DropDownListM.Text
        If fld.Trim = "" Then
            Exit Sub
        End If
        Session("mfld") = fld
        dv3 = Session("dv3")
        dv3.Sort = fld & " ASC"
        Dim dt As DataTable = dv3.ToTable(True, fld)
        For i = 0 To dt.Rows.Count - 1
            If dt.Rows(i)(0).ToString.Trim = "" Then
                Continue For
            End If
            If IsNumeric(dt.Rows(i)(0).ToString) Then
                DropDownColumnsV.Items.Add(ExponentToNumber(dt.Rows(i)(0).ToString))
            Else
                DropDownColumnsV.Items.Add(dt.Rows(i)(0).ToString.Replace(",", ";"))
                'replace comma in dv3 column
                Try
                    If dv3.Table.Columns(fld).DataType.Name = "DateTime" Then
                        dv3.Table.Rows(i)(fld) = DateToString(dv3.Table.Rows(i)(fld))
                    Else
                        dv3.Table.Rows(i)(fld) = dv3.Table.Rows(i)(fld).Replace(",", ";")
                    End If
                Catch ex As Exception

                End Try
            End If
        Next
        Session("SELECTEDValuesM") = ""
        DropDownColumnsV.Text = " "
        CheckBoxSelectAll.Checked = False
        CheckBoxUnselectAll.Checked = False
    End Sub

    Private Sub DropDownColumnsY_ChecklistChanged(sender As Object, e As Controls_uc1.ChecklistChangedArgs) Handles DropDownColumnsY.ChecklistChanged
        'depended of column type
        DropDownListA.Items.Clear()
        DropDownListA.Items.Add("Count")
        DropDownListA.Items.Add("CountDistinct")
        Dim nselected As Integer = 0
        Dim bNumeric As Boolean = True
        For i = 0 To DropDownColumnsY.Items.Count - 1
            If DropDownColumnsY.Items(i).Selected Then
                nselected = nselected + 1
                If nselected = 1 Then
                    Session("AxisYM") = DropDownColumnsY.Items(i).Text
                    Session("AxisY") = DropDownColumnsY.Items(i).Text
                    Session("y1") = DropDownColumnsY.Items(i).Text
                Else 'nselected > 1
                    If charttype = "PieChart" OrElse charttype = "BubbleChart" OrElse charttype = "Gauge" OrElse charttype = "Sankey" Then
                        Session("AxisYM") = Session("y1")
                        DropDownColumnsY.Items(i).Selected = False
                        nselected = 1
                    Else
                        Session("AxisYM") = Session("AxisYM") & "," & DropDownColumnsY.Items(i).Text
                    End If
                End If
                If Not ColumnTypeIsNumeric(dv3.Table.Columns(DropDownColumnsY.Items(i).Text)) Then
                    bNumeric = False
                    'Exit For
                End If
            End If
        Next
        Session("nYselM") = nselected
        If bNumeric AndAlso DropDownColumnsY.Text.Trim <> "" Then
            DropDownListA.Items.Add("Sum")
            DropDownListA.Items.Add("Max")
            DropDownListA.Items.Add("Min")
            DropDownListA.Items.Add("Avg")
            DropDownListA.Items.Add("StDev")
            DropDownListA.Items.Add("Value")
            chkboxNumeric.Checked = True
        Else
            Session("AggregateM") = "Count"
        End If
        Session("nv") = DropDownListA.Items.Count
        DropDownListA.Text = Session("AggregateM")
        DropDownListA.SelectedValue = Session("AggregateM")
        DropDownColumnsY.Text = Session("AxisYM")
        If nselected > 1 Or charttype = "PieChart" Then
            LabelM.Visible = False
            LabelV.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            Session("SELECTEDValuesM") = ""
            Session("MFld") = ""
            DropDownListM.Text = " "
            DropDownColumnsV.Text = " "
        Else
            LabelM.Visible = True
            LabelV.Visible = True
            DropDownColumnsV.Visible = True
            CheckBoxSelectAll.Visible = True
            CheckBoxUnselectAll.Visible = True
            CheckBoxSelectAll.Enabled = True
            CheckBoxUnselectAll.Enabled = True
            DropDownListM.Enabled = True
            DropDownListM.Visible = True
            Session("MFld") = ""
            Session("SELECTEDValuesM") = ""
            If Not Session("MFld") Is Nothing AndAlso Session("MFld").ToString.Trim <> "" Then DropDownListM.Text = Session("MFld")
            If Not Session("SELECTEDValuesM") Is Nothing AndAlso Session("SELECTEDValuesM").ToString.Trim <> "" Then DropDownColumnsV.Text = Session("SELECTEDValuesM")

        End If
        If charttypeM <> "MultiLineChart" Then
            LabelV.Visible = False
            LabelM.Visible = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
        End If
    End Sub

    Private Sub DropDownListA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListA.SelectedIndexChanged
        Session("Aggregate") = DropDownListA.SelectedValue
        Session("AggregateM") = DropDownListA.SelectedValue
    End Sub

    Private Sub btnShowChart_Click(sender As Object, e As EventArgs) Handles btnShowChart.Click
        Session("AxisXM") = DropDownColumnsX.Text
        Session("AxisYM") = DropDownColumnsY.Text
        Session("AggregateM") = DropDownListA.SelectedValue
        Session("MFld") = DropDownListM.Text
        Session("SELECTEDValuesM") = DropDownColumnsV.Text
        Dim xsels() As String = Split(Session("AxisXM"), ",")
        Dim ysels() As String = Split(Session("AxisYM"), ",")
        Session("AxisY") = ysels(0)
        Session("cat1") = xsels(0)
        If xsels.Length = 1 Then
            Session("cat2") = xsels(0)
        ElseIf xsels.Length > 1 Then
            Session("cat2") = xsels(1)
        Else
            LabelError.Visible = True
            LabelError.Text = "Select chart parameters. "
            Exit Sub
        End If
        Session("Aggregate") = Session("AggregateM")
        If charttype = "BubbleChart" OrElse charttype = "CandlestickChart" OrElse charttype = "Sankey" OrElse charttype = "Gauge" Then
            Session("arr") = ""
            arr = ""
            ttl = ""
            Response.Redirect("ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat1") & "&x2=" & Session("cat2") & "&y1=" & Session("AxisY") & "&fn=" & Session("AggregateM") & "&charttype=" & charttype)
            Exit Sub
        End If
        'calc arr for MultiLineChart !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        Dim xselected As String = Session("AxisXM") ' FixSelectedFields(repid, Session("AxisXM"), Session("UserConnString"), Session("UserConnProvider"))
        Dim yselected As String = Session("AxisYM") ' FixSelectedFields(repid, Session("AxisYM"), Session("UserConnString"), Session("UserConnProvider"))
        Dim nyselected As Integer = Session("nYselM")
        Dim fnM As String = Session("AggregateM")
        Dim mfld As String = Session("MFld")
        Dim valsM As String = Session("SELECTEDValuesM")
        Dim srtm As String = Session("AxisXM")
        Dim ssrtm As String = xselected
        Dim rt As String = String.Empty
        Dim er As String = String.Empty
        Dim flt As String = String.Empty
        'chart titles
        If fnM = "Value" Then
            ttl = "Value of [" & yselected & "] in group by [" & xselected & "]"
        ElseIf fnM = "Count" Then
            ttl = "Count of records in group by [" & xselected & "]"
        ElseIf fnM = "CountDistinct" Then
            ttl = "Distinct Count of [" & yselected & "] in group by [" & xselected & "]"
        ElseIf fnM = "Sum" Then
            ttl = "Sum of [" & yselected & "] in group by [" & xselected & "]"
        ElseIf fnM = "Avg" Then
            ttl = "Avg of [" & yselected & "] in group by [" & xselected & "]"
        ElseIf fnM = "StDev" Then
            ttl = "StDev of [" & yselected & "] in group by [" & xselected & "]"
        ElseIf fnM = "Max" Then
            ttl = "Max of [" & yselected & "] in group by [" & xselected & "]"
        ElseIf fnM = "Min" Then
            ttl = "Min of [" & yselected & "] in group by [" & xselected & "]"
        End If
        If Session("SELECTEDValuesM").ToString.Trim <> "" Then
            ttl = ttl & " for value of " & Session("MFld").ToString.Trim & "  in " & Session("SELECTEDValuesM").ToString.Trim
        End If

        Dim n, j, i As Integer

        'add custom groups
        For i = 0 To xsels.Length - 1
            For j = 0 To xsels.Length - 1
                If i <> j AndAlso xsels(i).Trim <> "" AndAlso xsels(j).Trim <> "" Then
                    rt = AddGroupBy(Session("REPORTID"), xsels(i), xsels(i), "custom", Session("UserConnString").ToString, Session("UserConnProvider").ToString, er)
                End If
            Next
        Next
        'dv3 retrieved already!!!
        dt = dv3.ToTable

        Session("ttl") = ttl & " (" & dt.Rows.Count.ToString & " records)"

        If nyselected = 1 AndAlso Session("SELECTEDValuesM").ToString.Trim <> "" Then
            If valsM.Trim = "" Then
                LabelError.Text = "Select field and values."
                Exit Sub
            End If
            'multiple lines for each selected value in valsM of the field mfld
            Dim vsels() As String = Split(valsM, ",")
            rt = ""
            dt = ComputeStatsV(dv3.Table, fnM, ysels, xsels, mfld, vsels, "", rt, Session("UserConnString"), Session("UserConnProvider"))
            dt.DefaultView.Sort = DropDownColumnsX.Text
            dt = dt.DefaultView.ToTable
            If rt = "Average return" Then
                ttl = "Avg of [" & yselected & "] in group by [" & xselected & "]"
            End If


            Dim arr = ""
            Dim grp As String = String.Empty
            Dim m As Integer
            arr = "['" & xselected & "','" & valsM.Replace(",", "','") & "'],"
            For i = 0 To dt.Rows.Count - 1
                grp = ""
                For m = 0 To xsels.Length - 1
                    grp = grp & dt.Rows(i)(xsels(m)).ToString
                    If m < xsels.Length - 1 Then
                        grp = grp & ","
                    End If
                Next
                arr = arr & "['" & grp & "',"
                '& dt.Rows(i)("ARR").ToString
                For n = 0 To vsels.Length - 1
                    arr = arr & dt.Rows(i)("ARR" & vsels(n)).ToString
                    If n < vsels.Length - 1 Then
                        arr = arr & ","
                    End If
                Next
                arr = arr & "]"
                If i < dt.Rows.Count - 1 Then arr = arr & ","
            Next
            nrec = dt.Rows.Count
            Session("nrec") = nrec
            Session("arr") = arr
            Session("ttl") = ttl & " (" & nrec.ToString & " records)"
            Session("y1") = y1
            Session("srt") = srt
            Session("AxisY") = y1
            If Session("arr").ToString.Trim = "" Then
                lnkbtnAddToDashboard.Enabled = False
                lnkbtnAddToDashboard.Visible = False
                lnkDownloadARR.Enabled = False
                lnkDownloadARR.Visible = False
            End If
        Else
            rt = ""
            'multiple lines for each selected field from yselected
            If fnM = "Value" Then
                For i = 0 To ysels.Length - 1
                    Dim coln As New DataColumn
                    coln.DataType = System.Type.GetType("System.String")
                    coln.ColumnName = "ARR" & ysels(i)
                    dt.Columns.Add(coln)
                    For n = 0 To dt.Rows.Count - 1
                        If dt.Rows(n)(ysels(i)).ToString.Trim = "" Then
                            dt.Rows(n)(coln.ColumnName) = "0"
                        Else
                            dt.Rows(n)(coln.ColumnName) = dt.Rows(n)(ysels(i))
                        End If
                    Next
                Next
            Else
                dt = ComputeStatsM(dv3.Table, fnM, ysels, xsels, "", rt, Session("UserConnString"), Session("UserConnProvider"))
                dt.DefaultView.Sort = DropDownColumnsX.Text
                dt = dt.DefaultView.ToTable
            End If

            Dim arr = ""
            Dim grp As String = String.Empty
            Dim m As Integer
            arr = "['" & xselected & "','" & yselected.Replace(",", "','") & "'],"
            For i = 0 To dt.Rows.Count - 1
                grp = ""
                For m = 0 To xsels.Length - 1
                    grp = grp & dt.Rows(i)(xsels(m)).ToString
                    If m < xsels.Length - 1 Then
                        grp = grp & ","
                    End If
                Next
                arr = arr & "['" & grp & "',"
                '& dt.Rows(i)("ARR").ToString
                For n = 0 To ysels.Length - 1
                    arr = arr & dt.Rows(i)("ARR" & ysels(n)).ToString
                    If n < ysels.Length - 1 Then
                        arr = arr & ","
                    End If
                Next
                arr = arr & "]"
                If i < dt.Rows.Count - 1 Then arr = arr & ","
            Next
            nrec = dt.Rows.Count
            Session("nrec") = nrec
            Session("arr") = arr
            Session("ttl") = ttl & " (" & nrec.ToString & " records)"
            Session("y1") = y1
            Session("srt") = srt
            Session("AxisY") = y1
            If Session("arr").ToString.Trim = "" Then
                lnkbtnAddToDashboard.Enabled = False
                lnkbtnAddToDashboard.Visible = False
                lnkDownloadARR.Enabled = False
                lnkDownloadARR.Visible = False
            End If
        End If

        Response.Redirect("ChartGoogleOne.aspx?Report=" & Session("REPORTID") & "&x1=" & Session("cat1") & "&x2=" & Session("cat2") & "&y1=" & Session("AxisY") & "&fn=" & Session("AggregateM") & "&charttype=" & charttype & "&domulti=yes&lbl=" & rt)

    End Sub

    Private Sub DropDownColumnsX_ChecklistChanged(sender As Object, e As Controls_uc1.ChecklistChangedArgs) Handles DropDownColumnsX.ChecklistChanged
        Session("AxisXM") = DropDownColumnsX.Text
        If charttypeM <> "MultiLineChart" OrElse Session("nYselM") > 1 OrElse charttype = "PieChart" Then
            LabelV.Visible = False
            LabelM.Visible = False
            DropDownListM.Enabled = False
            DropDownListM.Visible = False
            DropDownColumnsV.Visible = False
            CheckBoxSelectAll.Visible = False
            CheckBoxUnselectAll.Visible = False
            CheckBoxSelectAll.Enabled = False
            CheckBoxUnselectAll.Enabled = False
            Dim xsels() As String = Split(Session("AxisXM"), ",")
            Session("cat1") = xsels(0)
            If xsels.Length = 1 Then
                Session("cat2") = xsels(0)
            ElseIf xsels.Length > 1 Then
                Session("cat2") = xsels(1)
            Else
                LabelError.Visible = True
                LabelError.Text = "Select chart parameters. "
                Exit Sub
            End If
        Else
            LabelM.Visible = True
            LabelV.Visible = True
            DropDownColumnsV.Visible = True
            CheckBoxSelectAll.Visible = True
            CheckBoxUnselectAll.Visible = True
            CheckBoxSelectAll.Enabled = True
            CheckBoxUnselectAll.Enabled = True
            DropDownListM.Enabled = True
            DropDownListM.Visible = True
            If Not Session("MFld") Is Nothing AndAlso Session("MFld").ToString.Trim <> "" Then DropDownListM.Text = Session("MFld")
            If Not Session("SELECTEDValuesM") Is Nothing AndAlso Session("SELECTEDValuesM").ToString.Trim <> "" Then DropDownColumnsV.Text = Session("SELECTEDValuesM")

        End If
    End Sub

    Private Sub lnkDownloadARR_Click(sender As Object, e As EventArgs) Handles lnkDownloadARR.Click
        'Dim client As New Net.WebClient
        'client.Encoding = Encoding.UTF8
        'Dim str As String = client.DownloadString(url)

        'download Session("arr")
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString & "TEMP\"
        Dim filename As String = Session("REPORTID")
        ' This text is added only once to the file.
        If File.Exists(filename) = True Then
            filename = filename & "_" & Now.ToShortDateString.Replace("/", "_") & "_" & Now.ToShortTimeString.Replace(":", "_").Replace(" ", "")
        End If
        filename = filename & "ARR.txt"
        ' Create a file to write to.
        Dim writetext As String = LabelWhere.Text
        writetext = writetext & Chr(10) & Session("arr").ToString
        Session("writetext") = writetext
        File.WriteAllText(applpath & "TEMP\" & filename, writetext)
        Dim ret As String = String.Empty
        Try
            Response.ContentType = "application/octet-stream"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & filename)
            Response.TransmitFile(applpath & "TEMP\" & filename)
        Catch ex As Exception
            ret = "ERROR!!  " & ex.Message
        End Try
        Response.End()

    End Sub

    Private Sub lnkDataAI_Click(sender As Object, e As EventArgs) Handles lnkDataAI.Click
        'NOT IN USE
        Dim writetext As String = LabelWhere.Text
        writetext = writetext & Chr(10) & Session("arr").ToString
        Session("writetext") = writetext
        If Request("map") IsNot Nothing AndAlso Request("map").ToString = "yes" Then
            Response.Redirect("DataAI.aspx?pg=maps")
        Else
            Response.Redirect("DataAI.aspx?pg=charts")
        End If

    End Sub

    Private Sub chkboxNumeric_CheckedChanged(sender As Object, e As EventArgs) Handles chkboxNumeric.CheckedChanged
        DropDownListA.Items.Clear()
        DropDownListA.Items.Add("Count")
        DropDownListA.Items.Add("CountDistinct")
        If chkboxNumeric.Checked Then
            DropDownListA.Items.Add("Sum")
            DropDownListA.Items.Add("Max")
            DropDownListA.Items.Add("Min")
            DropDownListA.Items.Add("Avg")
            DropDownListA.Items.Add("StDev")
            DropDownListA.Items.Add("Value")
            Dim er As String = String.Empty
            dv3 = CorrectColumnsAsNumeric(dv3.Table, DropDownColumnsY.Text, er, True).DefaultView
            Session("dv3") = dv3
        End If
        Session("nv") = DropDownListA.Items.Count
        If Session("Aggregate") IsNot Nothing AndAlso Session("Aggregate").ToString.Trim <> "" Then
            DropDownListA.Text = Session("Aggregate")
            DropDownListA.SelectedValue = Session("Aggregate")
        End If
    End Sub
End Class
