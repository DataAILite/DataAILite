Imports System.Data
Imports System.IO
Partial Class DataAI
    Inherits System.Web.UI.Page
    Dim tblname As String
    Dim dataTable As DataTable
    Dim sFileType As String
    Dim srd As String
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Session("SELECTEDFlds") Is Nothing Then
            Session("SELECTEDFlds") = ""
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Not Session("PAGETTL") Is Nothing AndAlso Session("PAGETTL").ToString.Length > 0 Then
            LabelPageTtl.Text = Session("PAGETTL")
        End If
        If Not Request("TextBoxSearch") Is Nothing AndAlso Request("TextBoxSearch").ToString <> "" Then
            Session("srchval") = Request("TextBoxSearch").ToString
            TextBoxSearch.Text = Session("srchval")
        End If

        dataTable = Session("dataTable")
        Dim i As Integer
        Dim dt As DataTable = dataTable
        Dim fldname As String = String.Empty
        Dim frdname As String = String.Empty
        Dim fldtype As String = String.Empty
        Dim er As String = String.Empty

        If Not IsPostBack Then
            Session("SELECTEDFlds") = ""

            If Session("dataTable") Is Nothing Then
                ButtonUploadResult()
                Session("dataTable") = dataTable
                Session("OriginalDataTable") = dataTable
            Else
                Session("dataTable") = dataTable
            End If
            If Session("OriginalDataTable") Is Nothing Then
                Session("OriginalDataTable") = dataTable
            End If
            If Not dataTable Is Nothing Then
                dt = dataTable
                dt = MakeDTColumnsNamesCLScompliant(dt, Session("UserConnProvider"), er)
                'make search statement to filter dataTable and selecting fields for analytics
                If Not IsPostBack Then
                    'dropdowns
                    DropDownColumns.Items.Clear()
                    DropDownColumns1.Items.Clear()
                    DropDownColumns.Items.Add("")
                    DropDownColumns1.Items.Add("")


                    For i = 0 To dt.Columns.Count - 1
                        fldname = ""
                        frdname = ""
                        fldname = dt.Columns(i).Caption
                        fldtype = dt.Columns(i).DataType.ToString
                        If frdname = "" Then frdname = fldname
                        Dim li As ListItem = New ListItem
                        li.Text = frdname
                        li.Value = fldname
                        If Not Session("srchfld") Is Nothing Then
                            'search from session
                            If li.Text = Session("srchfld").ToString.Trim Then
                                li.Selected = True
                                For j = 0 To DropDownOperator.Items.Count - 1
                                    If DropDownOperator.Items(j).Text = Session("srchoper").ToString.Trim Then
                                        DropDownOperator.Items(j).Selected = True
                                    End If
                                Next
                                TextBoxSearch.Text = Session("srchval").ToString
                            End If
                        End If
                        DropDownColumns.Items.Add(li)
                        li = New ListItem
                        li.Text = fldname
                        li.Value = fldname
                        DropDownColumns1.Items.Add(li)
                    Next
                End If
            End If
            If Session("SELECTEDFlds") = "" OrElse Session("SELECTEDFlds") = "ALL" Then
                btnSelectAllClicked(sender, e)
            End If
            DropDownColumns1.Text = Session("SELECTEDFlds")

        Else  'PostBack
            'fill out grid with raw data to analyze
            dataTable = Session("dataTable")
            If Not dataTable Is Nothing Then
                'make search statement to filter dv3
                If IsPostBack Then
                    'dropdown
                    For i = 0 To dt.Columns.Count - 1
                        If Not Session("srchfld") Is Nothing Then
                            'search from session
                            If DropDownColumns.Items(i).Text = Session("srchfld").ToString.Trim Then
                                DropDownColumns.Items(i).Selected = True
                                For j = 0 To DropDownOperator.Items.Count - 1
                                    If DropDownOperator.Items(j).Text = Session("srchoper").ToString.Trim Then
                                        DropDownOperator.Items(j).Selected = True
                                    End If
                                Next
                                TextBoxSearch.Text = Session("srchval").ToString
                            End If

                        End If
                    Next
                End If
            Else
                LabelRowCount.Text = "Records returned: " & 0.ToString
            End If
        End If

        'Filter dataTable
        Dim srch As String = SearchStatement()
        If srch.Trim <> "" Then
            dataTable = Session("OriginalDataTable")
            dataTable.DefaultView.RowFilter = ""
            dataTable.DefaultView.RowFilter = srch

            If Session("srch") <> srch Then
                Session("srch") = srch
                Session("SortedView") = Nothing
                Session("SortColumn") = ""
            End If
            If Session("sqltoexport").ToUpper.indexof(" WHERE " & srch.ToUpper) < 0 Then
                If Session("sqltoexport").ToUpper.indexof(" WHERE ") > 1 Then
                    Session("sqltoexport") = Session("sqltoexport").Replace(" WHERE ", " WHERE " & srch & " AND ")
                ElseIf Session("sqltoexport").ToUpper.indexof(" ORDER BY ") > 1 Then
                    Session("sqltoexport") = Session("sqltoexport").Replace(" ORDER BY ", " WHERE " & srch & "  ORDER BY  ")
                Else
                    Session("sqltoexport") = Session("sqltoexport") & " WHERE " & srch
                End If
            End If
        ElseIf srch.Trim = "" Then  'AndAlso Session("srch") <> ""
            dataTable = Session("OriginalDataTable")
            dataTable.DefaultView.RowFilter = ""
            Session("dataTable") = dataTable
            Session("SortedView") = Nothing
            Session("srch") = ""
            Session("SortColumn") = ""
        End If

        Dim dtt As New DataTable
        dtt = dataTable.DefaultView.ToTable
        dtt = MakeDTColumnsNamesCLScompliant(dtt, Session("UserConnProvider"), er)
        If Session("SELECTEDFlds").ToString.Trim <> "" AndAlso Session("SELECTEDFlds").ToString.Trim <> "ALL" Then
            Dim flds() As String = Split(Session("SELECTEDFlds").Replace(",,", ","), ",")
            dtt = dtt.DefaultView.ToTable(True, flds)
        End If
        If dtt.Rows Is Nothing OrElse dtt.Rows.Count = 0 Then
            LabelRowCount.Text = "Records returned: " & 0.ToString
        ElseIf dtt.Rows.Count > 0 Then
            LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
        Else
            LabelRowCount.Text = "Records returned: " & 0.ToString
        End If
        Session("dataTable") = dtt
        dataTable = dtt

        If dataTable.Rows.Count > 0 Then   'AndAlso Session("GraphType") <> "matrix"
            ButtonUploadResult()
            GridView1.DataSource = dataTable.DefaultView
            GridView1.Visible = True
            GridView1.DataBind()
        Else
            'GridView1.DataSource = Nothing
            TextboxResult.Text = "no data"
            GridView1.DataSource = dataTable.DefaultView
            GridView1.Visible = True
            'GridView1.DataBind()
        End If

        LabelRowCount.Text = "Records returned: " & Session("dataTable").Rows.Count.ToString

    End Sub


    Protected Sub ButtonUploadResult()
        Dim ret As String = String.Empty
        Dim maptype As String = String.Empty
        sFileType = Request("pg")  'DropDownTypes.Text
        sFileType = IIf(sFileType = "", "charts", sFileType)
        If Not Request("srd") Is Nothing AndAlso Request("srd").ToString <> "" Then
            srd = Request("srd")
        Else
            srd = "0"
        End If
        srd = IIf(srd = "", "0", srd)
        'HyperLinkChatAI.NavigateUrl = "ChatAI.aspx?pg=" & sFileType & "&srd=" & srd
        If Session("GraphType") = "matrix" Then
            sFileType = "ExploreData"
            If Session("dataTable") Is Nothing Then
                LabelAlert.Text = "No data"
                Exit Sub
            End If
            'export dt to text
            Session("writetext") = ExportToCSVtext(Session("dataTable"), Chr(9), Session("QuestionToAI"))

        ElseIf sFileType = "expl" AndAlso srd = "0" Then
            sFileType = "ExploreData"
            If Session("dataTable") Is Nothing Then
                Session("dataTable") = Session("GridView1DataSource")
            End If
            If Session("dataTable") Is Nothing Then
                If Not Session("OriginalDataTable") Is Nothing Then
                    Session("dataTable") = Session("OriginalDataTable")
                Else
                    LabelAlert.Text = "No data"
                    Exit Sub
                End If
            End If
            Session("writetext") = ExportToCSVtext(Session("dataTable"), ",", "Explore data of report:   " & Session("REPTITLE"))

        ElseIf sFileType = "expl" AndAlso srd = "8" Then
            sFileType = "OverallStatistics"
            If Session("GridView2DataSource") Is Nothing Then
                LabelAlert.Text = "No data"
                Exit Sub
            End If
            'export dt to text
            Session("writetext") = ExportToCSVtext(Session("GridView2DataSource"), ",", "Overall Statistics for report:   " & Session("REPTITLE"))

        ElseIf sFileType = "cor" Then
            'export to csv the datatable ddtv
            Dim ddv As DataView = Session("ddtv")
            If ddv Is Nothing OrElse ddv.Table Is Nothing Then
                LabelAlert.Text = "No data"
                Exit Sub
            End If
            Dim cols(2) As String
            cols(0) = "Tbl1Fld1"
            cols(1) = "Tbl2Fld2"
            cols(2) = "Param2"
            Dim delmtr As String = Session("delimiter")
            Dim dt As New DataTable
            dt = ddv.ToTable(True, cols)
            dt.Columns("Tbl1Fld1").ColumnName = "Field1"
            dt.Columns("Tbl2Fld2").ColumnName = "Field2"
            dt.Columns("Param2").ColumnName = "CorCoef"
            Dim writetext As String = "Correlation data for report:   " & Session("REPTITLE")
            writetext = writetext & Chr(10)
            writetext = writetext & "Field1" & ","
            writetext = writetext & "Field2" & ","
            writetext = writetext & "Correlation Coefficient"
            writetext = writetext & Chr(10)
            For i = 0 To dt.Rows.Count - 1
                writetext = writetext & dt.Rows(i)("Field1") & ","
                writetext = writetext & dt.Rows(i)("Field2") & ","
                writetext = writetext & dt.Rows(i)("CorCoef")
                writetext = writetext & Chr(10)
            Next
            Session("writetext") = writetext

        ElseIf sFileType = "maps" Then
            maptype = Request("type")
        End If

        If Session("writetext") Is Nothing OrElse Session("writetext").ToString.Trim = "" Then
            LabelAlert.Text = "No data"
            Exit Sub
        End If
        '----------------------------------------------------------------------------
        'for downloading text result
        'Dim fileuploadDir As String = applpath ' ConfigurationManager.AppSettings("fileupload").ToString
        Dim filename As String = String.Empty
        Dim sDownloadFilename As String = sFileType.Replace(" ", "").Replace("/", "").Replace("\", "").Replace("_", "") & Now.ToShortDateString.Replace("/", "") & Now.ToShortTimeString.Replace(":", "").Replace(" ", "") & ".txt"
        Dim strFile As String = applpath & sDownloadFilename
        'strFile = strFile.Replace("\\", "\")

        hdnFileName.Value = sDownloadFilename
        hdnPath.Value = strFile
        '----------------------------------------------------------------------------

        dataTable = Session("dataTable")
        '============================================================ start =======================================================================
        Dim nl = Environment.NewLine()

        'CHARTS ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        If sFileType = "charts" Then
            ' Process CSV text and populate DataTable
            Dim result As Tuple(Of DataTable, String) = ChartsFromText(Session("writetext"))

            ' Access the DataTable
            If dataTable Is Nothing Then dataTable = result.Item1

            ' Access the first line of the file
            Dim titletext As String = result.Item2

            ' Print the DataTable content
            Dim resultText As String = "" ' "All Records Below:" & nl & nl & PrintDataTablebyColumn(dataTable)

            ' Find Analytics
            Dim analytics As String = ChartsAnalytics(dataTable)

            ' Set the result to the TextBox         
            TextboxResult.Text = titletext + New String(nl, 2) + analytics + resultText



            'MAPS ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ElseIf sFileType = "maps" Then

            ' Process CSV text and populate DataTable
            Dim titletext As String = String.Empty
            If dataTable Is Nothing Then dataTable = ProcessMapText(Session("writetext"), titletext, maptype)

            ' Print the DataTable content
            Dim resultText As String = "" '"All Records Below:" & nl & nl & PrintDataTablebyColumn(dataTable)

            ' Find Analytics
            Dim analytics As String = ComputeMapsAnalytics(dataTable, maptype)

            ' Set the result to the TextBox         
            TextboxResult.Text = titletext + New String(nl, 2) + analytics + resultText


            'MATRIX BALANCING ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ElseIf sFileType = "MatrixBalancing" Then
            'Dim datatable As DataTable
            Dim start_matrix As DataTable
            Dim target_matrix As DataTable
            Dim balancing_matrix As DataTable

            Dim balancing_coeffs As DataTable
            Dim final_text As String = ""
            Dim divider = New String("-", 120)

            Dim csvPaths() As String
            ' Iterate over the array containing CSV paths
            For Each csvFilePath As String In csvPaths
                ' Access the first line of the file
                Dim titletext As String
                Using sr1 As StreamReader = New StreamReader(csvFilePath)
                    titletext = sr1.ReadLine()
                End Using

                ' Identify kind of file and call appropriate function
                If titletext.StartsWith("Starting Matrix") Then
                    ' Process CSV file and populate DataTable
                    start_matrix = ProcessStartingMatrixCsv(csvFilePath)
                    dataTable = start_matrix

                ElseIf titletext.StartsWith("Balancing Matrix") Then
                    ' Process CSV file and populate DataTable
                    balancing_matrix = ProcessBalancingMatrixCsv(csvFilePath)
                    dataTable = balancing_matrix

                ElseIf titletext.StartsWith("Target Matrix") Then
                    ' Process CSV file and populate DataTable
                    target_matrix = ProcessTargetMatrixCsv(csvFilePath)
                    dataTable = target_matrix

                ElseIf titletext.StartsWith("Balancing Coefficients") Then
                    ' Process CSV file and populate DataTable
                    balancing_coeffs = ProcessBalancingCoefficientsCsv(csvFilePath)
                    dataTable = balancing_coeffs
                End If

                ' Print the DataTable content and get analytics
                Dim resultText As String = "All Records Below: " & nl & nl & PrintDataTablebyColumn(dataTable)
                Dim analytics As String = ComputeMatrixBalancingAnalytics(dataTable)

                ' Set the result to the TextBox
                final_text &= "FILE TITLE: " + titletext.ToUpper + New String(nl, 2) +
                                 analytics + New String(nl, 2) +
                                resultText + New String(nl, 2) +
                                divider + nl + divider + New String(nl, 3)

            Next

            Dim differencesString As String = ""
            If start_matrix IsNot Nothing AndAlso balancing_matrix IsNot Nothing Then
                differencesString &= "SPECIAL CROSS MATRIX ANALYTICS - Top 5 max differences between start_matrix and balancing_matrix: " & nl &
                                    GetTop5Differences(start_matrix, balancing_matrix) & nl &
                                    divider + nl + divider + nl + nl
            End If
            If start_matrix IsNot Nothing AndAlso target_matrix IsNot Nothing Then
                differencesString &= "SPECIAL CROSS MATRIX ANALYTICS - Top 5 max differences between start_matrix and target_matrix: " & nl &
                                    GetTop5Differences(start_matrix, target_matrix) & nl &
                                    divider + nl + divider + nl + nl
            End If
            If target_matrix IsNot Nothing AndAlso balancing_matrix IsNot Nothing Then
                differencesString &= "SPECIAL CROSS MATRIX ANALYTICS - Top 5 max differences between balancing_matrix and target matrix: " & nl &
                                    GetTop5Differences(balancing_matrix, target_matrix) & nl &
                                    divider + nl + divider + nl + nl
            End If

            TextboxResult.Text = differencesString & final_text

            'CORRELATION +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ElseIf sFileType = "cor" Then
            ' Process CSV text Correlations and populate DataTable
            Dim result As Tuple(Of DataTable, String) = ProcessCorrelationsText(Session("writetext"))

            ' Access the DataTable
            If dataTable Is Nothing Then dataTable = result.Item1

            ' Access the first line of the file
            Dim titletext As String = result.Item2

            ' Print the DataTable content
            Dim resultText As String = "" '"All Records Below:" & nl & nl & PrintDataTablebyColumn(dataTable)
            Dim analytics As String = ComputeCorrelationsAnalytics(dataTable)

            ' Set the result to the TextBox
            TextboxResult.Text = titletext + New String(nl, 2) + analytics + nl & resultText

            'OVERALL STATISTICS  +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ElseIf sFileType = "OverallStatistics" Then

            ' Process CSV file and populate DataTable
            Dim result As Tuple(Of DataTable, String) = ProcessOverallStatisticsText(Session("writetext"))

            ' Access the DataTable
            If dataTable Is Nothing Then dataTable = result.Item1

            ' Access the first line of the file
            Dim titletext As String = result.Item2

            'Get the DataTable content and analytics
            Dim resultText As String = "" '"All Records Below:" & nl & nl & PrintDataTablebyColumn(dataTable)
            Dim analytics As String = ComputeOverallStatisticsAnalytics(dataTable)

            ' Set the title and result to the TextBox
            TextboxResult.Text = titletext + New String(nl, 2) + analytics + resultText

            'EXPLORE DATA ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        ElseIf sFileType = "ExploreData" Then

            ' Process CSV file and populate DataTable
            Dim result As Tuple(Of DataTable, String) = ProcessExploreDataStatisticsText(Session("writetext"))

            ' Access the DataTable
            If dataTable Is Nothing Then dataTable = result.Item1

            ' Access the first line of the file
            Dim titletext As String = result.Item2

            ' Print the DataTable content
            Dim resultText As String = "" '"All Records Below:" & nl & nl & PrintDataTablebyColumn(dataTable)
            'Get the DataTable content and analytics
            Dim analytics As String = ComputeExploreDataAnalytics(dataTable)

            ' Set the title and result to the TextBox
            TextboxResult.Text = titletext + New String(nl, 2) + analytics + New String(nl, 2) + resultText

            'ANALYTICS/MATRIX ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        ElseIf sFileType = "AnalyticsMatrix" Then

            'GROUPS STATISTICS +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        ElseIf sFileType = "GroupsStatistics" Then


        End If

    End Sub
    Private Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        Session("srchfld") = Request("DropDownColumns")  'DropDownColumns.SelectedValue.ToString.Trim
        Session("srchoper") = Request("DropDownOperator")  'DropDownOperator.SelectedValue.ToString.Trim
        If Not Request("TextBoxSearch") Is Nothing AndAlso Request("TextBoxSearch").ToString <> "" Then
            Session("srchval") = Request("TextBoxSearch").ToString
            TextBoxSearch.Text = Session("srchval")
        Else
            Session("srchval") = TextBoxSearch.Text
        End If

    End Sub
    Private Function SearchStatement() As String
        Dim srch As String = String.Empty
        Dim myprovider = Session("UserConnProvider")
        Dim srchfld As String = String.Empty
        Dim oper As String = String.Empty
        Dim val As String = TextBoxSearch.Text
        Dim HasNot As Boolean = False

        If Request("DropDownColumns") Is Nothing OrElse Request("DropDownOperator") Is Nothing OrElse val = "" Then
            If Session("srchfld") IsNot Nothing AndAlso Session("srchfld").ToString.Trim <> "" AndAlso Session("srchoper") IsNot Nothing AndAlso Session("srchoper").ToString.Trim <> "" AndAlso Session("srchval") IsNot Nothing AndAlso Session("srchval").ToString.Trim <> "" Then
                srchfld = Session("srchfld")
                oper = Session("srchoper")
                val = Session("srchval")
            Else
                Session("srchfld") = Nothing
                Session("srchoper") = Nothing
                Session("srchval") = Nothing
                DropDownColumns.SelectedIndex = 0
                DropDownOperator.SelectedIndex = 0
                TextBoxSearch.Text = String.Empty
                Return srch
            End If

        ElseIf Request("DropDownColumns").ToString.Trim <> "" AndAlso Request("DropDownOperator").ToString.Trim <> "" AndAlso TextBoxSearch.Text <> "" Then
            srchfld = "[" & Request("DropDownColumns").ToString.Trim & "] "
            oper = Request("DropDownOperator").ToString.Trim
            Session("srchfld") = Request("DropDownColumns").ToString.Trim 'srchfld
            Session("srchoper") = oper
            Session("srchval") = val
        Else
            Session("srchfld") = Nothing
            Session("srchoper") = Nothing
            Session("srchval") = Nothing
            DropDownColumns.SelectedIndex = 0
            DropDownOperator.SelectedIndex = 0
            TextBoxSearch.Text = String.Empty
            Return srch
        End If

        HasNot = oper.ToUpper.Contains("NOT")
        If Not TblSQLqueryFieldIsNumeric(Session("REPORTID"), srchfld, Session("UserConnString"), Session("UserConnProvider")) Then
            If oper.ToUpper.Contains("STARTSWITH") Then
                If HasNot Then
                    oper = "Not Like"
                Else
                    oper = "Like"
                End If
                srch = srchfld & oper & " '" & val & "%'"
            ElseIf oper.ToUpper.Contains("ENDSWITH") Then
                If HasNot Then
                    oper = "Not Like"
                Else
                    oper = "Like"
                End If
                srch = srchfld & oper & " '%" & val & "'"
            ElseIf oper.ToUpper.Contains("CONTAINS") Then
                If HasNot Then
                    oper = "Not Like"
                Else
                    oper = "Like"
                End If
                srch = srchfld & oper & " '%" & val & "%'"
            ElseIf oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                srch = srchfld & oper & " ('" & val.Replace("(", "").Replace(")", "").Replace(",", "','") & "')"
            End If
            If srch.Trim = "" Then
                srch = srchfld & oper & " '" & val.Replace("'", "") & "'"
            End If
        End If
        If TblSQLqueryFieldIsNumeric(Session("REPORTID"), srchfld, Session("UserConnString"), Session("UserConnProvider")) Then
            If oper.ToUpper = "IN" OrElse oper.ToUpper = "NOT IN" Then
                srch = srchfld & oper & " (" & val.Replace("(", "").Replace(")", "").Replace("'", "") & ")"
            End If
            If srch.Trim = "" Then
                srch = srchfld & oper & " " & val.Replace("'", "")
            End If
        End If

        Session("Search") = srch
        Return srch
    End Function
    Private Sub BindReportFields()
        Dim ReportDataView As DataView = CType(Session("dataTable").DefaultView, DataView)
        If Session("SortedView") IsNot Nothing Then
            ReportDataView = Session("SortedView")
        End If
        'If ColumnTypeIsNumeric(dv3.Table.Columns(i)) Then
        '    GridView1.Columns(i).ControlStyle = Type. 'ItemStyle =  "{0:P}" '.DefaultCellStyle.Format = "N0"
        'End If
        GridView1.DataSource = ReportDataView
        GridView1.DataBind()
    End Sub
    Private Sub GridView1_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageIndex = e.NewPageIndex
        If Session("SortedView") Is Nothing Then
            BindReportFields()
        Else
            GridView1.DataSource = Session("SortedView")
            GridView1.DataBind()
        End If
        ButtonSearch_Click(sender, e)
    End Sub

    Protected Sub GridView1_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView1.Sorting
        Dim ReportDataView As DataView = CType(Session("dataTable").DefaultView, DataView)
        If ReportDataView.Count < 2 Then Return
        If String.IsNullOrEmpty(Session("SortColumn")) OrElse
           Session("SortColumn") <> e.SortExpression Then
            Session("SortColumn") = e.SortExpression
            Session("SortDirection") = "ASC"
        ElseIf Session("SortDirection") = "ASC" Then
            Session("SortDirection") = "DESC"
        Else
            Session("SortDirection") = "ASC"
        End If
        Dim SortedView As New DataView(ReportDataView.Table)
        SortedView.RowFilter = ReportDataView.RowFilter
        SortedView.Sort = Session("SortColumn") & " " & Session("SortDirection")
        Session("SortedView") = SortedView
        GridView1.DataSource = SortedView
        GridView1.DataBind()
        ButtonSearch_Click(sender, e)
    End Sub


    'CHARTS ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    ' Function to process Charts Text and return DataTable
    Private Function ChartsFromText(ByVal filestr As String) As Tuple(Of DataTable, String)
        Dim i, j As Integer
        ' Create DataTable to store data and firstLine to store title
        Dim dataTable As New DataTable
        Dim firstLine As String
        Dim secondline As String
        ' Read the string and process the data
        Using sr1 As StringReader = New StringReader(filestr)
            firstLine = sr1.ReadLine
            secondline = sr1.ReadLine
        End Using


        Dim lines() As String = secondline.Trim.Split("],")

        Dim cols() As String = lines(0).Replace("[", "").Replace("'", "").Replace("{", "").Replace("}", "").Replace(":", "").Split(",")
        For i = 0 To cols.Length - 1
            If cols(i).Trim <> "" Then
                dataTable.Columns.Add(cols(i).Trim.Replace(" ", "_"))
            End If
        Next
        For j = 1 To lines.Length - 1
            Dim line As String = lines(j).Trim.Replace(",[", "")
            If line.Trim = "" Then
                Continue For
            End If
            Dim columnData() As String = SplitCsv(line)
            Dim newRow As DataRow = dataTable.NewRow()
            For i = 0 To columnData.Length - 1
                newRow(i) = columnData(i).Trim.Replace("[", "").Replace("]", "").Replace("'", "")
                'System.Diagnostics.Debug.WriteLine("Column Data" & columnData(j) & Environment.NewLine)
            Next
            'Add DataRow to DataTable
            dataTable.Rows.Add(newRow)
        Next

        ''firstLine = lines(0)    '.Split(New String() {"---"}, StringSplitOptions.RemoveEmptyEntries)(0)
        '' Find index of column names line and title
        'Dim columnNamesIndex As Integer = Array.FindIndex(lines, Function(line) line.Trim().EndsWith("records) ---"))

        'For i As Integer = columnNamesIndex + 1 To lines.Length - 1
        '    Dim line As String = lines(i).Trim().Replace("],", "")
        '    If i = columnNamesIndex + 1 Then
        '        Dim columnNames() As String = SplitCsv(line)
        '        For Each columnName As String In columnNames
        '            Try
        '                dataTable.Columns.Add(columnName.Replace("'", "").Trim)
        '            Catch ex As Exception

        '            End Try
        '        Next
        '    Else
        '        Dim columnData() As String = SplitCsv(line)
        '        Dim newRow As DataRow = dataTable.NewRow()
        '        For j As Integer = 0 To columnData.Length - 1
        '            newRow(j) = columnData(j).Trim.Replace("]", "").Replace("'", "")
        '            'System.Diagnostics.Debug.WriteLine("Column Data" & columnData(j) & Environment.NewLine)
        '        Next
        '        ' Add DataRow to DataTable
        '        dataTable.Rows.Add(newRow)
        '    End If
        'Next

        Return Tuple.Create(dataTable, firstLine)

    End Function

    ' Function to process Charts file and return DataTable
    Private Function ChartsCsv(csvPath As String) As Tuple(Of DataTable, String)

        ' Create DataTable to store data and firstLine to store title
        Dim dataTable As New DataTable
        Dim firstLine As String

        ' Read the file content and process the data
        Using sr1 As StreamReader = New StreamReader(csvPath)
            ' Read file content
            Dim filestr As String = sr1.ReadToEnd

            Dim lines() As String = filestr.Split("[")
            firstLine = lines(0).Split(New String() {"---"}, StringSplitOptions.RemoveEmptyEntries)(0)

            ' Find index of column names line and title
            Dim columnNamesIndex As Integer = Array.FindIndex(lines, Function(line) line.Trim().EndsWith("records) ---"))

            For i As Integer = columnNamesIndex + 1 To lines.Length - 1
                Dim line As String = lines(i).Trim().Replace("],", "")
                If i = columnNamesIndex + 1 Then
                    Dim columnNames() As String = SplitCsv(line)
                    For Each columnName As String In columnNames
                        Try
                            dataTable.Columns.Add(columnName)
                        Catch ex As Exception

                        End Try

                    Next
                Else
                    Dim columnData() As String = SplitCsv(line)
                    Dim newRow As DataRow = dataTable.NewRow()
                    For j As Integer = 0 To columnData.Length - 1
                        newRow(j) = columnData(j).Replace("]", "")
                        'System.Diagnostics.Debug.WriteLine("Column Data" & columnData(j) & Environment.NewLine)
                    Next
                    ' Add DataRow to DataTable
                    dataTable.Rows.Add(newRow)
                End If
            Next
        End Using

        For Each column As DataColumn In dataTable.Columns
            System.Diagnostics.Debug.WriteLine("Column Name: " & column.ColumnName & Environment.NewLine)
        Next

        Return Tuple.Create(dataTable, firstLine)

    End Function

    ' Function to process Charts datatable and return analytics
    Private Function ChartsAnalytics(DataTable As DataTable) As String
        If DataTable Is Nothing OrElse DataTable.Rows.Count = 0 Then
            Return "No data"
        End If
        Dim nl = Environment.NewLine()

        ' Calculate max and min for all numeric columns
        Dim analytics As String = ""

        ' Analyze all numeric columns
        For Each column As DataColumn In DataTable.Columns
            Dim isNumericColumn As Boolean = True
            For Each row As DataRow In DataTable.Rows
                If Not IsNumeric(row(column.ColumnName)) OrElse column.ColumnName.ToUpper = "ID" OrElse column.ColumnName.ToUpper = "INDX" Then
                    isNumericColumn = False
                    Exit For
                End If
            Next

            If isNumericColumn Then
                'Finding max and min values
                Dim maxValues As New List(Of Double)()
                Dim minValues As New List(Of Double)()

                For Each row As DataRow In DataTable.Rows
                    maxValues.Add(CDbl(row(column.ColumnName)))
                    minValues.Add(CDbl(row(column.ColumnName)))
                Next

                Dim max As Double = maxValues.Max()
                Dim min As Double = minValues.Min()
                Dim maxText As String = "Max Value: " & max.ToString()
                Dim minText As String = "Min Value: " & min.ToString()

                ' Sort the DataTable based on the column you want to analyze
                Dim sortedDataTable As DataTable
                If column.ColumnName.ToLower().Contains("rank") Then
                    sortedDataTable = DataTable.Select().OrderBy(Function(x) Convert.ToInt32(x(column.ColumnName))).CopyToDataTable()
                Else
                    sortedDataTable = DataTable.Select().OrderByDescending(Function(x) Convert.ToDecimal(x(column.ColumnName))).CopyToDataTable()
                End If
                'Dim datavw As New DataView
                'datavw = DataTable.DefaultView
                'datavw.Sort = column.ColumnName & " DESC"
                'sortedDataTable = datavw.ToTable
                ' Calculate the 5% threshold
                Dim threshold As Integer = CInt(Math.Ceiling(DataTable.Rows.Count * 0.05))

                ' Get the top 5% and lowest 5% values
                Dim top5Percent = sortedDataTable.AsEnumerable().Take(threshold).CopyToDataTable()
                Dim lowest5Percent = sortedDataTable.AsEnumerable().Skip(Math.Max(0, sortedDataTable.Rows.Count - threshold)).CopyToDataTable()


                ' Display the top 5% values
                Dim top5PercentText = "Below are the Records in Top 5%:" & nl
                ' Display the top 5% values in tabular form
                Dim top5PercentDataTable As New DataTable()
                For Each col As DataColumn In top5Percent.Columns
                    If col.ColumnName = column.ColumnName Then
                        top5PercentDataTable.Columns.Add("*" & col.ColumnName & "*", col.DataType) ' Add marker to highlight current column
                    Else
                        top5PercentDataTable.Columns.Add(col.ColumnName, col.DataType)
                    End If
                Next

                ' Copy the top 5% rows to the new DataTable
                For Each row As DataRow In top5Percent.Rows
                    Dim newRow As DataRow = top5PercentDataTable.NewRow()
                    For Each col As DataColumn In top5Percent.Columns
                        If col.ColumnName = column.ColumnName Then
                            newRow("*" & col.ColumnName & "*") = row(col.ColumnName) ' Add marker to highlight current column
                        Else
                            newRow(col.ColumnName) = row(col.ColumnName)
                        End If
                    Next
                    top5PercentDataTable.Rows.Add(newRow)
                Next

                ' Print the top 5% DataTable by columns
                Dim top5percent_records As String = PrintDataTablebyColumn(top5PercentDataTable)

                ' Calculate the range for top 5%
                Dim range_start As Double = Double.MaxValue
                Dim range_end As Double = Double.MinValue
                For Each row As DataRow In top5Percent.Rows
                    Dim value As Double = Convert.ToDouble(row(column.ColumnName))
                    range_start = Math.Min(range_start, value)
                    range_end = Math.Max(range_end, value)
                Next

                Dim lowest5PercentText = "Below are the Records in Lowest 5% for " & column.ColumnName & ":" & nl
                ' Display the lowest 5% values in tabular form
                Dim low5PercentDataTable As New DataTable()
                For Each col As DataColumn In lowest5Percent.Columns
                    If col.ColumnName = column.ColumnName Then
                        low5PercentDataTable.Columns.Add("*" & col.ColumnName & "*", col.DataType) ' Add marker to highlight current column
                    Else
                        low5PercentDataTable.Columns.Add(col.ColumnName, col.DataType)
                    End If
                Next

                ' Copy the lowest 5% rows to the new DataTable
                For Each row As DataRow In lowest5Percent.Rows
                    Dim newRow As DataRow = low5PercentDataTable.NewRow()
                    For Each col As DataColumn In lowest5Percent.Columns
                        If col.ColumnName = column.ColumnName Then
                            newRow("*" & col.ColumnName & "*") = row(col.ColumnName) ' Add marker to highlight current column
                        Else
                            newRow(col.ColumnName) = row(col.ColumnName)
                        End If
                    Next
                    low5PercentDataTable.Rows.Add(newRow)
                Next

                ' Print the lowest 5% DataTable by columns
                Dim lowest5percent_records As String = PrintDataTablebyColumn(low5PercentDataTable)

                ' Calculate the range for top 5%
                Dim low_range_start As Double = Double.MaxValue
                Dim low_range_end As Double = Double.MinValue
                For Each row As DataRow In lowest5Percent.Rows
                    Dim value As Double = Convert.ToDouble(row(column.ColumnName))
                    low_range_start = Math.Min(low_range_start, value)
                    low_range_end = Math.Max(low_range_end, value)
                Next

                ' Combine all information for the column
                analytics &= "Analytics for *" & column.ColumnName & "* : " & nl & nl &
                                maxText & nl & minText & nl & nl &
                                "Top 5% Range: ( " & range_start.ToString() & " , " & range_end.ToString() & " )" & nl &
                                top5PercentText & top5percent_records & nl &
                                "Lowest 5% Range: ( " & low_range_start.ToString() & " , " & low_range_end.ToString() & " )" & nl &
                                lowest5PercentText & lowest5percent_records & nl
                Dim divider As String = New String("-", 80)

                analytics &= divider & nl & divider & nl

            End If
        Next

        Return analytics

    End Function

    'MAPS ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    ' Function to process Maps CSV file and return DataTable
    Private Function ProcessMapText(ByVal fileContent As String, ByRef titletext As String, ByVal maptype As String) As DataTable
        Dim n As Integer = 0
        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim k As Integer = 0
        Dim addcolumns(0) As String
        Dim colname As Integer = 0
        Dim dataTable As New DataTable
        Dim baloontext As String = String.Empty

        Dim lines() As String = fileContent.Split("[")
        titletext = lines(0).Split(New String() {"---"}, StringSplitOptions.RemoveEmptyEntries)(0)
        For i = 1 To lines.Length - 1
            Dim line As String = lines(i).Replace("],", "").Replace("]", "").Trim()
            If i = 1 Then
                Dim columnNames() As String = line.Split(","c)
                For Each columnName As String In columnNames
                    dataTable.Columns.Add(columnName.Replace("'", "").Trim())
                Next
                If maptype = "MapChart" Then
                    line = lines(2).Replace("],", "").Replace("]", "").Trim()
                    Dim mapname As String = Request("mapname").ToString
                    Dim dm As DataTable = GetReportMapFields(Session("REPORTID"), mapname)
                    'add columns with names in despription
                    For j = 0 To dm.Rows.Count - 1
                        If dm.Rows(j)("MapField").ToString.Trim = "---" AndAlso dm.Rows(j)("ForMap").ToString.Trim = "PlacemarkDescription" Then
                            ReDim Preserve addcolumns(n)
                            addcolumns(n) = dm.Rows(j)("descrtext").ToString.Trim
                            dataTable.Columns.Add(addcolumns(n))
                            n = n + 1
                        End If
                    Next
                End If

            Else
                Dim columnData() As String = line.Split(","c)
                Dim newRow As DataRow = dataTable.NewRow()
                For k = 0 To columnData.Length - 1
                    newRow(k) = columnData(k).Replace("'", "").Trim()
                Next

                If maptype = "MapChart" Then
                    'add MapChart fields values splitting in loop by names of columns
                    'clean value in the column "Name" from additional fields values 
                    baloontext = newRow("Name")
                    For j = 0 To n - 1
                        newRow(addcolumns(n - 1 - j)) = Piece(baloontext, addcolumns(n - 1 - j), 2).ToString.Trim
                        baloontext = Piece(baloontext, addcolumns(n - 1 - j), 1).ToString.Trim
                    Next
                    newRow("Name") = baloontext
                End If

                dataTable.Rows.Add(newRow)
            End If
        Next

        Return dataTable
    End Function

    ' Function to process Maps datatable and return analytics
    Private Function ComputeMapsAnalytics(dataTable As DataTable, ByVal maptype As String) As String
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            Return "No data"
        End If

        Dim nl As String = Environment.NewLine()


        Dim analytics As String = ""

        Dim latColumnName As String = ""
        Dim longColumnName As String = ""

        ' Find the column names for latitude and longitude
        For Each column As DataColumn In dataTable.Columns
            If column.ColumnName.ToLower().StartsWith("lat") Then
                latColumnName = column.ColumnName
            End If
            If column.ColumnName.ToLower().StartsWith("long") Then
                longColumnName = column.ColumnName
            End If
        Next

        ' Analyze all numeric columns
        For Each column As DataColumn In dataTable.Columns
            Dim isNumericColumn As Boolean = True
            For Each row As DataRow In dataTable.Rows
                If Not IsNumeric(row(column.ColumnName)) Then
                    isNumericColumn = False
                    Exit For
                End If
            Next

            If isNumericColumn AndAlso column.ColumnName.ToLower() <> latColumnName.ToLower() AndAlso column.ColumnName.ToLower() <> longColumnName.ToLower() Then
                ' Sort the table for the current numeric column
                Dim sortedDataTable = dataTable.Clone()
                sortedDataTable.Columns(column.ColumnName).DataType = GetType(Double)
                sortedDataTable = dataTable.AsEnumerable().OrderBy(Function(x) CDbl(x(column.ColumnName))).CopyToDataTable()

                'Finding max and min values after sorting
                Dim max As Double = CDbl(sortedDataTable.Rows(sortedDataTable.Rows.Count - 1)(column.ColumnName))
                Dim min As Double = CDbl(sortedDataTable.Rows(0)(column.ColumnName))

                ' Get rows where max and min values occur
                Dim maxLocationsDataTable As DataTable = sortedDataTable.Clone()
                Dim minLocationsDataTable As DataTable = sortedDataTable.Clone()

                For Each row As DataRow In sortedDataTable.Rows
                    Dim value As Double = CDbl(row(column.ColumnName))
                    If value = max Then
                        maxLocationsDataTable.ImportRow(row)
                    End If
                    If value = min Then
                        minLocationsDataTable.ImportRow(row)
                    End If
                Next

                maxLocationsDataTable.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName & "*"
                minLocationsDataTable.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName & "*"

                ' Calculate average
                Dim sum As Double = 0
                For Each row As DataRow In sortedDataTable.Rows
                    sum += CDbl(row(column.ColumnName))
                Next
                Dim average As Double = sum / sortedDataTable.Rows.Count

                ' Get the top 5% values
                Dim threshold As Integer = CInt(Math.Ceiling(dataTable.Rows.Count * 0.05))
                Dim sortedDataTable2 = dataTable.AsEnumerable().OrderByDescending(Function(x) CDbl(x(column.ColumnName))).CopyToDataTable()
                Dim top5Percent = sortedDataTable2.AsEnumerable().Take(threshold).CopyToDataTable()
                Dim lowest5Percent = sortedDataTable2.AsEnumerable().Skip(Math.Max(0, sortedDataTable2.Rows.Count - threshold)).CopyToDataTable()

                ' Get the range for top 5% values
                Dim top5PercentLongitudes As New List(Of Double)
                Dim top5PercentLatitudes As New List(Of Double)
                For Each top5PercentRow As DataRow In top5Percent.Rows
                    top5PercentLongitudes.Add(CDbl(top5PercentRow(longColumnName)))
                    top5PercentLatitudes.Add(CDbl(top5PercentRow(latColumnName)))
                Next
                Dim top5PercentMinLong As Double = top5PercentLongitudes.Min()
                Dim top5PercentMaxLong As Double = top5PercentLongitudes.Max()
                Dim top5PercentMinLat As Double = top5PercentLatitudes.Min()
                Dim top5PercentMaxLat As Double = top5PercentLatitudes.Max()

                top5Percent.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName & "*"

                ' Get the range for lowest 5% values
                Dim lowest5PercentLongitudes As New List(Of Double)
                Dim lowest5PercentLatitudes As New List(Of Double)
                For Each lowest5PercentRow As DataRow In lowest5Percent.Rows
                    lowest5PercentLongitudes.Add(CDbl(lowest5PercentRow(longColumnName)))
                    lowest5PercentLatitudes.Add(CDbl(lowest5PercentRow(latColumnName)))
                Next
                Dim lowest5PercentMinLong As Double = lowest5PercentLongitudes.Min()
                Dim lowest5PercentMaxLong As Double = lowest5PercentLongitudes.Max()
                Dim lowest5PercentMinLat As Double = lowest5PercentLatitudes.Min()
                Dim lowest5PercentMaxLat As Double = lowest5PercentLatitudes.Max()

                lowest5Percent.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName & "*"


                Dim divider As String = New String("-", 80)
                analytics &= "ANALYTICS FOR *" & column.ColumnName.ToUpper() & "* :" & nl & nl &
                            "Average: " & average.ToString() & nl & nl &
                            "Max Value: " & max.ToString() & " occurs at below locations:" & nl &
                            PrintDataTablebyColumn(maxLocationsDataTable) & nl &
                            "Min Value: " & min.ToString() & " occurs at below locations:" & nl &
                            PrintDataTablebyColumn(minLocationsDataTable) & nl &
                        "Top 5% records are located in the below range of (Longitude, Latitude):" & nl & "(" &
                        top5PercentMinLong & ", " & top5PercentMinLat & "), (" &
                        top5PercentMinLong & ", " & top5PercentMaxLat & "), (" &
                        top5PercentMaxLong & ", " & top5PercentMaxLat & "), (" &
                        top5PercentMaxLong & ", " & top5PercentMinLat & ")" & nl & nl &
                            "Records in Top 5% are shown below:" & nl &
                            PrintDataTablebyColumn(top5Percent) & nl & nl &
                            "Lowest 5% records are located in the below range of (Longitude, Latitude):" & nl & "(" &
                            lowest5PercentMinLong & ", " & lowest5PercentMinLat & "), (" &
                            lowest5PercentMinLong & ", " & lowest5PercentMaxLat & "), (" &
                            lowest5PercentMaxLong & ", " & lowest5PercentMaxLat & "), (" &
                            lowest5PercentMaxLong & ", " & lowest5PercentMinLat & ")" & nl & nl &
                            "Records in Lowest 5% are shown below:" & nl &
                            PrintDataTablebyColumn(lowest5Percent) & nl &
                        nl & divider & nl & divider & nl & nl
            End If
        Next

        Return analytics

    End Function


    'MATRIX BALANCING ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    ' Function to process Balancing Coefficients CSV file and return DataTable
    Private Function ProcessBalancingCoefficientsCsv(csvPath As String) As DataTable
        Dim dataTable As New DataTable()

        Using sr As New StreamReader(csvPath)
            ' Read file content
            Dim fileContent As String = sr.ReadToEnd()
            ' Split file content by lines
            Dim lines() As String = fileContent.Split(Environment.NewLine)

            Dim columnNamesIndex As Integer = -1
            ' Get column names index
            For i As Integer = 0 To lines.Length - 1
                Dim line As String = lines(i).Trim()
                If i = 0 Or Len(line) = 0 Then
                    Continue For
                End If
                If columnNamesIndex = -1 Then
                    columnNamesIndex = i
                    Exit For
                End If
            Next

            If columnNamesIndex >= 0 Then
                ' Extract column names
                Dim columnNames() As String = lines(columnNamesIndex).Split(","c)
                For Each columnName As String In columnNames
                    dataTable.Columns.Add(columnName.Trim())
                Next

                ' Add rows to DataTable
                For i As Integer = columnNamesIndex + 1 To lines.Length - 1
                    Dim line As String = lines(i).Trim()
                    If line.StartsWith("Result:") Or line.StartsWith("Total:") Or Len(line) = 0 Then
                        Exit For
                    End If

                    Dim rowData() As String = line.Split(","c)
                    Dim newRow As DataRow = dataTable.NewRow()

                    For j As Integer = 0 To Math.Min(columnNames.Length - 1, rowData.Length - 1)
                        newRow(j) = rowData(j).Trim()
                    Next

                    ' Add DataRow to DataTable
                    dataTable.Rows.Add(newRow)
                Next
            End If
        End Using

        Return dataTable
    End Function

    ' Function to process Balancing Matrix CSV file and return DataTable
    Private Function ProcessBalancingMatrixCsv(csvPath As String) As DataTable
        Dim dataTable As New DataTable()

        Using sr As New StreamReader(csvPath)
            ' Read file content
            Dim fileContent As String = sr.ReadToEnd()
            ' Split file content by lines
            Dim lines() As String = fileContent.Split(Environment.NewLine)

            Dim columnNamesIndex As Integer = -1
            ' Get column names index
            For i As Integer = 0 To lines.Length - 1
                Dim line As String = lines(i).Trim()
                If i = 0 Or Len(line) = 0 Then
                    Continue For
                End If
                If columnNamesIndex = -1 Then
                    columnNamesIndex = i
                    Exit For
                End If
            Next

            If columnNamesIndex >= 0 Then
                ' Extract column names
                Dim columnNames() As String = lines(columnNamesIndex).Split(","c)
                For Each columnName As String In columnNames
                    dataTable.Columns.Add(columnName.Trim())
                Next

                ' Add rows to DataTable
                For i As Integer = columnNamesIndex + 1 To lines.Length - 1
                    Dim line As String = lines(i).Trim()
                    If line.StartsWith("Result:") Or line.StartsWith("Total:") Or Len(line) = 0 Then
                        Exit For
                    End If

                    Dim rowData() As String = line.Split(","c)
                    Dim newRow As DataRow = dataTable.NewRow()

                    For j As Integer = 0 To Math.Min(columnNames.Length - 1, rowData.Length - 1)
                        newRow(j) = rowData(j).Trim()
                    Next

                    ' Add DataRow to DataTable
                    dataTable.Rows.Add(newRow)
                Next
            End If
        End Using

        Return dataTable
    End Function

    ' Function to process Starting Matrix CSV file and return DataTable
    Private Function ProcessStartingMatrixCsv(csvPath As String) As DataTable
        Dim dataTable As New DataTable()

        Using sr As New StreamReader(csvPath)
            ' Read file content
            Dim fileContent As String = sr.ReadToEnd()
            ' Split file content by lines
            Dim lines() As String = fileContent.Split(Environment.NewLine)

            Dim columnNamesIndex As Integer = -1
            ' Get column names index
            For i As Integer = 0 To lines.Length - 1
                Dim line As String = lines(i).Trim()
                If i = 0 Or Len(line) = 0 Then
                    Continue For
                End If
                If columnNamesIndex = -1 Then
                    columnNamesIndex = i
                    Exit For
                End If
            Next

            If columnNamesIndex >= 0 Then
                ' Extract column names
                Dim columnNames() As String = lines(columnNamesIndex).Split(","c)
                For Each columnName As String In columnNames
                    dataTable.Columns.Add(columnName.Trim())
                Next

                ' Add rows to DataTable
                For i As Integer = columnNamesIndex + 1 To lines.Length - 1
                    Dim line As String = lines(i).Trim()
                    If line.StartsWith("Result:") Or line.StartsWith("Total:") Or Len(line) = 0 Then
                        Exit For
                    End If

                    Dim rowData() As String = line.Split(","c)
                    Dim newRow As DataRow = dataTable.NewRow()

                    For j As Integer = 0 To Math.Min(columnNames.Length - 1, rowData.Length - 1)
                        newRow(j) = rowData(j).Trim()
                    Next

                    ' Add DataRow to DataTable
                    dataTable.Rows.Add(newRow)
                Next
            End If
        End Using

        Return dataTable
    End Function

    ' Function to process Target Matrix CSV file and return DataTable
    Private Function ProcessTargetMatrixCsv(csvPath As String) As DataTable
        Dim dataTable As New DataTable()

        Using sr As New StreamReader(csvPath)
            ' Read file content
            Dim fileContent As String = sr.ReadToEnd()
            ' Split file content by lines
            Dim lines() As String = fileContent.Split(Environment.NewLine)

            Dim columnNamesIndex As Integer = -1
            ' Get column names index
            For i As Integer = 0 To lines.Length - 1
                Dim line As String = lines(i).Trim()
                If i = 0 Or Len(line) = 0 Then
                    Continue For
                End If
                If columnNamesIndex = -1 Then
                    columnNamesIndex = i
                    Exit For
                End If
            Next

            If columnNamesIndex >= 0 Then
                ' Extract column names
                Dim columnNames() As String = lines(columnNamesIndex).Split(","c)
                For Each columnName As String In columnNames
                    dataTable.Columns.Add(columnName.Trim())
                Next

                ' Add rows to DataTable
                For i As Integer = columnNamesIndex + 1 To lines.Length - 1
                    Dim line As String = lines(i).Trim()
                    If line.StartsWith("Result:") Or line.StartsWith("Total:") Or Len(line) = 0 Then
                        Exit For
                    End If

                    Dim rowData() As String = line.Split(","c)
                    Dim newRow As DataRow = dataTable.NewRow()

                    For j As Integer = 0 To Math.Min(columnNames.Length - 1, rowData.Length - 1)
                        newRow(j) = rowData(j).Trim()
                    Next

                    ' Add DataRow to DataTable
                    dataTable.Rows.Add(newRow)
                Next
            End If
        End Using

        Return dataTable
    End Function

    Function GetTop5Differences(start_matrix As DataTable, end_matrix As DataTable) As String

        Dim maxDifferences As New List(Of Tuple(Of String, String, Double))()
        Dim nl = Environment.NewLine()

        For i As Integer = 1 To start_matrix.Rows.Count - 1
            For j As Integer = 1 To start_matrix.Columns.Count - 1
                Dim startValue As Double = Convert.ToDouble(start_matrix.Rows(i)(j))
                Dim balancingValue As Double = Convert.ToDouble(end_matrix.Rows(i)(j))
                Dim difference As Double = Math.Abs(startValue - balancingValue)
                maxDifferences.Add(New Tuple(Of String, String, Double)(start_matrix(i)(0).ToString, start_matrix.Columns(j).ToString, difference))
            Next
        Next

        ' Sort the differences list in descending order
        maxDifferences.Sort(Function(x, y) y.Item3.CompareTo(x.Item3))

        ' Prepare the formatted string
        Dim result As String = ""


        For i As Integer = 0 To Math.Min(4, maxDifferences.Count - 1)
            Dim difference As Tuple(Of String, String, Double) = maxDifferences(i)
            Dim rowIndex As String = difference.Item1
            Dim colIndex As String = difference.Item2
            Dim diffValue As Double = difference.Item3
            result &= "( " & rowIndex & " , " & colIndex & ") :  " & diffValue & nl
        Next

        Return result
    End Function

    ' Function to compute Analytics for Matrix Balancing
    Private Function ComputeMatrixBalancingAnalytics(dataTable As DataTable) As String

        Dim nl = Environment.NewLine()

        Dim analytics As String = ""
        Dim divider As String = New String("-", 80)

        ' Analyze all numeric columns
        For Each column As DataColumn In dataTable.Columns
            Dim isNumericColumn As Boolean = True
            For Each row As DataRow In dataTable.Rows
                If Not IsNumeric(row(column.ColumnName)) Then
                    isNumericColumn = False
                    Exit For
                End If
            Next

            If isNumericColumn Then
                'Finding max and min values
                Dim maxValues As New List(Of Double)()
                Dim minValues As New List(Of Double)()

                For Each row As DataRow In dataTable.Rows
                    maxValues.Add(CDbl(row(column.ColumnName)))
                    minValues.Add(CDbl(row(column.ColumnName)))
                Next

                Dim max As Double = maxValues.Max()
                Dim min As Double = minValues.Min()
                Dim maxText As String = "Max Value: " & max.ToString()
                Dim minText As String = "Min Value: " & min.ToString()

                ' Sort the DataTable based on the column you want to analyze
                Dim sortedDataTable As DataTable
                If column.ColumnName.ToLower().Contains("rank") Then
                    sortedDataTable = dataTable.Select().OrderBy(Function(x) Convert.ToInt32(x(column.ColumnName))).CopyToDataTable()
                Else
                    sortedDataTable = dataTable.Select().OrderByDescending(Function(x) Convert.ToDecimal(x(column.ColumnName))).CopyToDataTable()
                End If

                ' Calculate the 5% threshold
                Dim threshold As Integer = CInt(Math.Ceiling(dataTable.Rows.Count * 0.05))

                ' Get the top 5% and lowest 5% values
                Dim top5Percent = sortedDataTable.AsEnumerable().Take(threshold).CopyToDataTable()
                Dim lowest5Percent = sortedDataTable.AsEnumerable().Skip(Math.Max(0, sortedDataTable.Rows.Count - threshold)).CopyToDataTable()

                ' Display the top 5% values
                Dim top5PercentText = "Below are the Records in Top 5%:" & nl
                ' Display the top 5% values in tabular form
                Dim top5PercentDataTable As New DataTable()
                For Each col As DataColumn In top5Percent.Columns
                    If col.ColumnName = column.ColumnName Then
                        top5PercentDataTable.Columns.Add("*" & col.ColumnName & "*", col.DataType) ' Add marker to highlight current column
                    Else
                        top5PercentDataTable.Columns.Add(col.ColumnName, col.DataType)
                    End If
                Next

                ' Copy the top 5% rows to the new DataTable
                For Each row As DataRow In top5Percent.Rows
                    Dim newRow As DataRow = top5PercentDataTable.NewRow()
                    For Each col As DataColumn In top5Percent.Columns
                        If col.ColumnName = column.ColumnName Then
                            newRow("*" & col.ColumnName & "*") = row(col.ColumnName) ' Add marker to highlight current column
                        Else
                            newRow(col.ColumnName) = row(col.ColumnName)
                        End If
                    Next
                    top5PercentDataTable.Rows.Add(newRow)
                Next

                ' Print the top 5% DataTable by columns
                Dim top5percent_records As String = PrintDataTablebyColumn(top5PercentDataTable)

                ' Calculate the range for top 5%
                Dim range_start As Double = Double.MaxValue
                Dim range_end As Double = Double.MinValue
                For Each row As DataRow In top5Percent.Rows
                    Dim value As Double = Convert.ToDouble(row(column.ColumnName))
                    range_start = Math.Min(range_start, value)
                    range_end = Math.Max(range_end, value)
                Next

                Dim lowest5PercentText = "Below are the Records in Lowest 5% for " & column.ColumnName & ":" & nl
                ' Display the lowest 5% values in tabular form
                Dim low5PercentDataTable As New DataTable()
                For Each col As DataColumn In lowest5Percent.Columns
                    If col.ColumnName = column.ColumnName Then
                        low5PercentDataTable.Columns.Add("*" & col.ColumnName & "*", col.DataType) ' Add marker to highlight current column
                    Else
                        low5PercentDataTable.Columns.Add(col.ColumnName, col.DataType)
                    End If
                Next

                ' Copy the lowest 5% rows to the new DataTable
                For Each row As DataRow In lowest5Percent.Rows
                    Dim newRow As DataRow = low5PercentDataTable.NewRow()
                    For Each col As DataColumn In lowest5Percent.Columns
                        If col.ColumnName = column.ColumnName Then
                            newRow("*" & col.ColumnName & "*") = row(col.ColumnName) ' Add marker to highlight current column
                        Else
                            newRow(col.ColumnName) = row(col.ColumnName)
                        End If
                    Next
                    low5PercentDataTable.Rows.Add(newRow)
                Next

                ' Print the lowest 5% DataTable by columns
                Dim lowest5percent_records As String = PrintDataTablebyColumn(low5PercentDataTable)


                ' Calculate the range for top 5%
                Dim low_range_start As Double = Double.MaxValue
                Dim low_range_end As Double = Double.MinValue
                For Each row As DataRow In lowest5Percent.Rows
                    Dim value As Double = Convert.ToDouble(row(column.ColumnName))
                    low_range_start = Math.Min(low_range_start, value)
                    low_range_end = Math.Max(low_range_end, value)
                Next

                ' Combine all information for the column
                analytics &= "Analytics for *" & column.ColumnName & "* : " & nl & nl &
                                maxText & nl & minText & nl & nl &
                                "Top 5% Range: ( " & range_start.ToString() & " , " & range_end.ToString() & " )" & nl &
                                top5PercentText & top5percent_records & nl &
                                "Lowest 5% Range: ( " & low_range_start.ToString() & " , " & low_range_end.ToString() & " )" & nl &
                                lowest5PercentText & lowest5percent_records & nl

                analytics &= divider & nl & divider & nl

            End If

        Next

        Return analytics

    End Function


    'CORRELATION +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    ' Function to process Correlations CSV file and return DataTable
    Private Function ProcessCorrelationsText(ByVal filestr As String) As Tuple(Of DataTable, String)
        If filestr Is Nothing OrElse filestr.Trim = "" Then
            LabelAlert.Text = "No data"
            Return Nothing
            Exit Function
        End If
        Dim dataTable As New DataTable()
        Dim firstline As String

        ' Read the file line by line
        Using sr As New StringReader(filestr)
            Dim line As String
            ' Ignore first Line
            firstline = sr.ReadLine().Replace(",", "").Trim()
            Dim isColumnLine As Boolean = True ' Flag to indicate if the current line is the column names line
            line = sr.ReadLine()
            Do While line IsNot Nothing
                If Not String.IsNullOrWhiteSpace(line.Replace(",", "").Trim()) Then
                    If isColumnLine Then
                        ' Split the line to get the column names
                        Dim columnNames() As String = line.Split(","c)
                        For Each columnName As String In columnNames
                            dataTable.Columns.Add(columnName.Trim())
                        Next
                        isColumnLine = False
                    Else
                        ' Split the line to get the data
                        Dim rowData() As String = line.Split(","c)
                        If rowData.Length = 3 Then
                            ' Add the row to the DataTable
                            dataTable.Rows.Add(rowData)
                        End If
                    End If
                End If
                line = sr.ReadLine()
            Loop
        End Using

        Return Tuple.Create(dataTable, firstline)

    End Function

    ' Function to process Correlations CSV file and return DataTable
    Private Function ProcessCorrelationsCsv(csvPath As String) As Tuple(Of DataTable, String)

        Dim dataTable As New DataTable()
        Dim firstline As String

        ' Read the file line by line
        Using sr As New StreamReader(csvPath)
            Dim line As String
            ' Ignore first Line
            firstline = sr.ReadLine().Replace(",", "").Trim()
            Dim isColumnLine As Boolean = True ' Flag to indicate if the current line is the column names line
            line = sr.ReadLine()
            Do While line IsNot Nothing
                If Not String.IsNullOrWhiteSpace(line.Replace(",", "").Trim()) Then
                    If isColumnLine Then
                        ' Split the line to get the column names
                        Dim columnNames() As String = line.Split(","c)
                        For Each columnName As String In columnNames
                            dataTable.Columns.Add(columnName.Trim())
                        Next
                        isColumnLine = False
                    Else
                        ' Split the line to get the data
                        Dim rowData() As String = line.Split(","c)
                        If rowData.Length = 3 Then
                            ' Add the row to the DataTable
                            dataTable.Rows.Add(rowData)
                        End If
                    End If
                End If
                line = sr.ReadLine()
            Loop
        End Using

        Return Tuple.Create(dataTable, firstline)

    End Function

    'Function to compute Analytics for correlations
    Private Function ComputeCorrelationsAnalytics(dataTable As DataTable) As String
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            Return "No data"
        End If
        Dim nl = Environment.NewLine()
        Dim analytics As String = ""

        For Each column As DataColumn In dataTable.Columns
            If IsNumeric(dataTable.Rows(0)(column.ColumnName)) Then
                Dim values As New List(Of Double)()
                For Each row As DataRow In dataTable.Rows
                    Dim value As Double
                    If Double.TryParse(row(column.ColumnName).ToString(), value) Then
                        values.Add(value)
                    End If
                Next

                Dim max As Double = values.Max()
                Dim min As Double = values.Min()
                Dim avg As Double = values.Average()

                Dim maxRows As New List(Of DataRow)()
                Dim minRows As New List(Of DataRow)()
                For Each row As DataRow In dataTable.Rows
                    Dim value As Double
                    If Double.TryParse(row(column.ColumnName).ToString(), value) Then
                        If value = max Then
                            maxRows.Add(row)
                        End If
                        If value = min Then
                            minRows.Add(row)
                        End If
                    End If
                Next

                Dim maxRecordsTable As New DataTable()
                Dim minRecordsTable As New DataTable()

                maxRecordsTable = dataTable.Clone()
                minRecordsTable = dataTable.Clone()

                For Each maxRow As DataRow In maxRows
                    Dim newRow As DataRow = maxRecordsTable.NewRow()
                    For Each col As DataColumn In maxRecordsTable.Columns
                        newRow(col.ColumnName) = maxRow(col.ColumnName)
                    Next
                    maxRecordsTable.Rows.Add(newRow)
                Next

                For Each minRow As DataRow In minRows
                    Dim newRow As DataRow = minRecordsTable.NewRow()
                    For Each col As DataColumn In minRecordsTable.Columns
                        newRow(col.ColumnName) = minRow(col.ColumnName)
                    Next
                    minRecordsTable.Rows.Add(newRow)
                Next

                maxRecordsTable.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName.ToUpper & "*"
                minRecordsTable.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName.ToUpper & "*"

                Dim maxTableOutput As String = PrintDataTablebyColumn(maxRecordsTable)
                Dim minTableOutput As String = PrintDataTablebyColumn(minRecordsTable)

                ' Get the top 5% and lowest 5% values
                Dim sortedDataTable As DataTable
                sortedDataTable = dataTable.Select().OrderByDescending(Function(x) Convert.ToDecimal(x(column.ColumnName))).CopyToDataTable()
                Dim threshold As Integer = CInt(Math.Ceiling(dataTable.Rows.Count * 0.05))
                Dim top5Percent = sortedDataTable.AsEnumerable().Take(threshold).CopyToDataTable()
                Dim lowest5Percent = sortedDataTable.AsEnumerable().Skip(Math.Max(0, sortedDataTable.Rows.Count - threshold)).CopyToDataTable()

                ' Display the top 5% values
                Dim top5PercentText = "Below are the Records in Top 5%:" & nl

                ' Display the top 5% values in tabular form
                Dim top5PercentDataTable As New DataTable()
                For Each col As DataColumn In top5Percent.Columns
                    top5PercentDataTable.Columns.Add(col.ColumnName, col.DataType)
                Next

                ' Copy the top 5% rows to the new DataTable
                For Each row As DataRow In top5Percent.Rows
                    Dim newRow As DataRow = top5PercentDataTable.NewRow()
                    For Each col As DataColumn In top5Percent.Columns
                        newRow(col.ColumnName) = row(col.ColumnName)
                    Next
                    top5PercentDataTable.Rows.Add(newRow)
                Next

                ' Print the top 5% DataTable by columns
                top5PercentDataTable.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName.ToUpper & "*"
                Dim top5percent_records As String = PrintDataTablebyColumn(top5PercentDataTable)

                ' Calculate the range for top 5%
                Dim range_start As Double = Double.MaxValue
                Dim range_end As Double = Double.MinValue
                For Each row As DataRow In top5Percent.Rows
                    Dim value As Double = Convert.ToDouble(row(column.ColumnName))
                    range_start = Math.Min(range_start, value)
                    range_end = Math.Max(range_end, value)
                Next

                Dim lowest5PercentText = "Below are the Records in Lowest 5%:" & nl
                ' Display the lowest 5% values in tabular form
                Dim low5PercentDataTable As New DataTable()
                For Each col As DataColumn In lowest5Percent.Columns
                    low5PercentDataTable.Columns.Add(col.ColumnName, col.DataType)
                Next

                ' Copy the lowest 5% rows to the new DataTable
                For Each row As DataRow In lowest5Percent.Rows
                    Dim newRow As DataRow = low5PercentDataTable.NewRow()
                    For Each col As DataColumn In lowest5Percent.Columns
                        newRow(col.ColumnName) = row(col.ColumnName)
                    Next
                    low5PercentDataTable.Rows.Add(newRow)
                Next

                ' Print the lowest 5% DataTable by
                low5PercentDataTable.Columns(column.ColumnName).ColumnName = "*" & column.ColumnName.ToUpper & "*"
                Dim lowest5percent_records As String = PrintDataTablebyColumn(low5PercentDataTable)

                ' Calculate the range for lowest 5%
                Dim low_range_start As Double = Double.MaxValue
                Dim low_range_end As Double = Double.MinValue
                For Each row As DataRow In lowest5Percent.Rows
                    Dim value As Double = Convert.ToDouble(row(column.ColumnName))
                    low_range_start = Math.Min(low_range_start, value)
                    low_range_end = Math.Max(low_range_end, value)
                Next


                Dim divider As String = New String("-", 80)

                analytics &= "Analytics for *" & column.ColumnName.ToUpper & "* : " & nl & nl &
                            "Average Value: " & avg & nl & nl &
                            "Top 5% Range: ( " & range_start.ToString() & " , " & range_end.ToString() & " )" & nl &
                            top5PercentText & top5percent_records & nl &
                            "Max Value: " & max & nl &
                            maxTableOutput & nl &
                            "Lowest 5% Range: ( " & low_range_start.ToString() & " , " & low_range_end.ToString() & " )" & nl &
                            lowest5PercentText & lowest5percent_records & nl &
                            "Min Value: " & min & nl &
                            minTableOutput & nl &
                            "All records sorted by *" & column.ColumnName.ToUpper & "* : " & nl & PrintDataTablebyColumn(sortedDataTable)

                analytics &= divider & nl & divider & nl

            End If
        Next

        Return analytics
    End Function

    'OVERALL STATISTICS  +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    ' Function to process Overall Statistics CSV file and return DataTable
    Private Function ProcessOverallStatisticsText(ByVal filestr As String) As Tuple(Of DataTable, String)
        If filestr Is Nothing OrElse filestr.Trim = "" Then
            LabelAlert.Text = "No data"
            Return Nothing
            Exit Function
        End If

        Dim dataTable As New DataTable()
        Dim firstLine As String = ""

        ' Read the file line by line
        Using sr As New StringReader(filestr)
            Dim line As String
            ' Take first Line
            firstLine = sr.ReadLine().Replace(",", "").Trim()
            System.Diagnostics.Debug.WriteLine(firstLine)
            TextboxResult.Text = firstLine + "\n"
            Dim isColumnLine As Boolean = True ' Flag to indicate if the current line is the column names line
            line = sr.ReadLine()
            Do While line IsNot Nothing
                If Not String.IsNullOrWhiteSpace(line.Replace(",", "").Trim()) Then
                    If isColumnLine Then
                        ' Split the line to get the column names
                        Dim columnNames() As String = line.Split(","c)
                        For Each columnName As String In columnNames
                            dataTable.Columns.Add(columnName.Trim())
                        Next
                        isColumnLine = False
                    Else
                        ' Split the line to get the data
                        Dim rowData() As String = line.Split(","c)
                        If rowData.Length = dataTable.Columns.Count Then
                            ' Add the row to the DataTable
                            dataTable.Rows.Add(rowData)
                        End If
                    End If
                End If
                line = sr.ReadLine()
            Loop
        End Using

        Return Tuple.Create(dataTable, firstLine)

    End Function

    ' Function to process Overall Statistics CSV file and return DataTable
    Private Function ProcessOverallStatisticsCsv(csvPath As String) As Tuple(Of DataTable, String)

        Dim dataTable As New DataTable()
        Dim firstLine As String = ""

        ' Read the file line by line
        Using sr As New StreamReader(csvPath)
            Dim line As String
            ' Take first Line
            firstLine = sr.ReadLine().Replace(",", "").Trim()
            System.Diagnostics.Debug.WriteLine(firstLine)
            TextboxResult.Text = firstLine + "\n"
            Dim isColumnLine As Boolean = True ' Flag to indicate if the current line is the column names line
            line = sr.ReadLine()
            Do While line IsNot Nothing
                If Not String.IsNullOrWhiteSpace(line.Replace(",", "").Trim()) Then
                    If isColumnLine Then
                        ' Split the line to get the column names
                        Dim columnNames() As String = line.Split(","c)
                        For Each columnName As String In columnNames
                            dataTable.Columns.Add(columnName.Trim())
                        Next
                        isColumnLine = False
                    Else
                        ' Split the line to get the data
                        Dim rowData() As String = line.Split(","c)
                        If rowData.Length = dataTable.Columns.Count Then
                            ' Add the row to the DataTable
                            dataTable.Rows.Add(rowData)
                        End If
                    End If
                End If
                line = sr.ReadLine()
            Loop
        End Using

        Return Tuple.Create(dataTable, firstLine)

    End Function

    'Function to compute Analytics for Overall Statistics    
    Private Function ComputeOverallStatisticsAnalytics(dataTable As DataTable) As String
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            Return "No data"
        End If

        Dim nl As String = Environment.NewLine()
        Dim analytics As String = ""

        For Each column As DataColumn In dataTable.Columns
            If column.ColumnName.ToUpper <> "ID" AndAlso column.ColumnName.ToUpper <> "INDX" AndAlso IsNumeric(dataTable.Rows(0)(column.ColumnName)) Then
                Dim columnToAnalyze As String = column.ColumnName

                Dim values As New List(Of Double)()
                For Each row As DataRow In dataTable.Rows
                    Dim value As Double
                    If Double.TryParse(row(columnToAnalyze).ToString(), value) Then
                        values.Add(value)
                    End If
                Next

                Dim max As Double = values.Max()
                Dim min As Double = values.Min()
                Dim avg As Double = values.Average()

                Dim maxValues As String = "Max Value: " & max.ToString()
                Dim minValues As String = "Min Value: " & min.ToString()
                Dim avgValuesText As String = "Average Value: " & avg.ToString()

                ' Creating DataTable for top 5% values
                Dim top5PercentTable As New DataTable()
                For Each col As DataColumn In dataTable.Columns
                    Dim columnName As String = col.ColumnName
                    If columnName = columnToAnalyze Then
                        columnName = "**" & columnName.ToUpper & "**" ' Enclose current column within **
                    End If
                    top5PercentTable.Columns.Add(columnName, col.DataType)
                Next

                Dim threshold As Integer = CInt(Math.Ceiling(dataTable.Rows.Count * 0.05))
                Dim sortedDataTable = dataTable.AsEnumerable() _
    .Where(Function(x) Double.TryParse(x(columnToAnalyze).ToString(), Nothing)) _
    .OrderByDescending(Function(x) Convert.ToDouble(x(columnToAnalyze))) _
    .Take(threshold) _
    .CopyToDataTable()
                ' Copy the top 5% rows to the new DataTable
                For Each row As DataRow In sortedDataTable.Rows
                    Dim newRow As DataRow = top5PercentTable.NewRow()
                    For Each col As DataColumn In sortedDataTable.Columns
                        If col.ColumnName = columnToAnalyze Then
                            col.ColumnName = "**" & col.ColumnName.ToUpper & "**" ' Enclose current column within **
                        End If
                        newRow(col.ColumnName) = row(col.ColumnName)
                    Next
                    top5PercentTable.Rows.Add(newRow)
                Next

                ' Creating DataTable for lowest 5% values
                Dim lowest5PercentTable As New DataTable()
                For Each col As DataColumn In dataTable.Columns
                    Dim columnName As String = col.ColumnName
                    If columnName = columnToAnalyze Then
                        columnName = "**" & columnName.ToUpper & "**" ' Enclose current column within **
                    End If
                    lowest5PercentTable.Columns.Add(columnName, col.DataType)
                Next

                Dim lowestSortedDataTable = dataTable.AsEnumerable() _
                .Where(Function(x) Double.TryParse(x(columnToAnalyze).ToString(), Nothing)) _
                .OrderBy(Function(x) Convert.ToDouble(x(columnToAnalyze))) _
                .Take(threshold) _
                .CopyToDataTable()

                ' Copy the lowest 5% rows to the new DataTable
                For Each row As DataRow In lowestSortedDataTable.Rows
                    Dim newRow As DataRow = lowest5PercentTable.NewRow()
                    For Each col As DataColumn In lowestSortedDataTable.Columns
                        If col.ColumnName = columnToAnalyze Then
                            col.ColumnName = "**" & col.ColumnName.ToUpper & "**" ' Enclose current column within **
                        End If
                        newRow(col.ColumnName) = row(col.ColumnName)
                    Next
                    lowest5PercentTable.Rows.Add(newRow)
                Next



                Dim top5PercentRange As String = "Top 5% Range: (" & values.OrderByDescending(Function(x) x).Take(threshold).LastOrDefault() & " - " & values.OrderByDescending(Function(x) x).Take(threshold).FirstOrDefault() & ")"
                Dim lowest5PercentRange As String = "Lowest 5% Range: (" & values.OrderBy(Function(x) x).Take(threshold).FirstOrDefault() & " - " & values.OrderBy(Function(x) x).Take(threshold).LastOrDefault() & ")"

                Dim top5PercentText As String = top5PercentRange & nl & PrintDataTablebyColumn(top5PercentTable)

                Dim lowest5PercentText As String = lowest5PercentRange & nl & PrintDataTablebyColumn(lowest5PercentTable)

                Dim divider As String = New String("-", 150)
                analytics &= "Analytics for **" & columnToAnalyze.ToUpper & "** :" & nl & nl &
                        maxValues & nl & nl &
                        minValues & nl & nl &
                        avgValuesText & nl & nl &
                        top5PercentText & nl &
                        lowest5PercentText & nl &
                        divider & nl & divider & nl & nl
            End If
        Next

        Return analytics

    End Function


    'EXPLORE DATA ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    '+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    ' Function to process Explore Data CSV file and return DataTable
    Private Function ProcessExploreDataStatisticsText(ByVal filestr As String) As Tuple(Of DataTable, String)
        If filestr Is Nothing OrElse filestr.Trim = "" Then
            LabelAlert.Text = "No data"
            Return Nothing
            Exit Function
        End If
        Dim dataTable As New DataTable()
        Dim firstLine As String = ""

        ' Read the file line by line
        Using sr As New StringReader(filestr)
            Dim line As String
            ' Take first Line
            firstLine = sr.ReadLine().Replace(",", "").Trim()
            'System.Diagnostics.Debug.WriteLine(firstLine)
            TextboxResult.Text = firstLine + "\n"
            Dim isColumnLine As Boolean = True ' Flag to indicate if the current line is the column names line
            line = sr.ReadLine()
            Do While line IsNot Nothing
                If Not String.IsNullOrWhiteSpace(line.Replace(",", "").Trim()) Then
                    If isColumnLine Then
                        ' Split the line to get the column names
                        Dim columnNames() As String = SplitCsv(line)
                        For Each columnName As String In columnNames
                            dataTable.Columns.Add(columnName.Trim())
                        Next
                        isColumnLine = False
                    Else
                        ' Split the line to get the data
                        Dim rowData() As String = SplitCsv(line)
                        If rowData.Length = dataTable.Columns.Count Then
                            ' Add the row to the DataTable
                            dataTable.Rows.Add(rowData)
                        End If
                    End If
                End If
                line = sr.ReadLine()
            Loop
        End Using

        Return Tuple.Create(dataTable, firstLine)

    End Function

    ' Function to process Explore Data CSV file and return DataTable
    Private Function ProcessExploreDataStatisticsCsv(csvPath As String) As Tuple(Of DataTable, String)
        'NOT IN USE?
        Dim dataTable As New DataTable()
        Dim firstLine As String = ""

        ' Read the file line by line
        Using sr As New StreamReader(csvPath)
            Dim line As String
            ' Take first Line
            firstLine = sr.ReadLine().Replace(",", "").Trim()
            System.Diagnostics.Debug.WriteLine(firstLine)
            TextboxResult.Text = firstLine + "\n"
            Dim isColumnLine As Boolean = True ' Flag to indicate if the current line is the column names line
            line = sr.ReadLine()
            Do While line IsNot Nothing
                If Not String.IsNullOrWhiteSpace(line.Replace(",", "").Trim()) Then
                    If isColumnLine Then
                        ' Split the line to get the column names
                        Dim columnNames() As String = SplitCsv(line)
                        For Each columnName As String In columnNames
                            dataTable.Columns.Add(columnName.Trim())
                        Next
                        isColumnLine = False
                    Else
                        ' Split the line to get the data
                        Dim rowData() As String = SplitCsv(line)
                        If rowData.Length = dataTable.Columns.Count Then
                            ' Add the row to the DataTable
                            dataTable.Rows.Add(rowData)
                        End If
                    End If
                End If
                line = sr.ReadLine()
            Loop
        End Using

        Return Tuple.Create(dataTable, firstLine)

    End Function

    'Function to compute Analytics for Overall Statistics    
    Private Function ComputeExploreDataAnalytics(dataTable As DataTable) As String
        If dataTable Is Nothing OrElse dataTable.Rows.Count = 0 Then
            Return "No data"
        End If

        Dim nl As String = Environment.NewLine()
        Dim analytics As String = ""

        For Each column As DataColumn In dataTable.Columns
            If column.ColumnName.ToUpper <> "ID" AndAlso column.ColumnName.ToUpper <> "INDX" AndAlso IsNumeric(dataTable.Rows(0)(column.ColumnName)) Then
                Dim columnToAnalyze As String = column.ColumnName

                Dim values As New List(Of Double)()
                For Each row As DataRow In dataTable.Rows
                    Dim value As Double
                    If Double.TryParse(row(columnToAnalyze).ToString(), value) Then
                        values.Add(value)
                    End If
                Next

                Dim max As Double = values.Max()
                Dim min As Double = values.Min()
                Dim avg As Double = values.Average()

                Dim maxValues As String = "Max Value: " & max.ToString()
                Dim minValues As String = "Min Value: " & min.ToString()
                Dim avgValuesText As String = "Average Value: " & avg.ToString()

                ' Creating DataTable for top 5% values
                Dim top5PercentTable As New DataTable()
                For Each col As DataColumn In dataTable.Columns
                    Dim columnName As String = col.ColumnName
                    If columnName = columnToAnalyze Then
                        columnName = "**" & columnName.ToUpper & "**" ' Enclose current column within **
                    End If
                    top5PercentTable.Columns.Add(columnName, col.DataType)
                Next

                Dim threshold As Integer = CInt(Math.Ceiling(dataTable.Rows.Count * 0.05))
                Dim sortedDataTable = dataTable.AsEnumerable() _
    .Where(Function(x) Double.TryParse(x(columnToAnalyze).ToString(), Nothing)) _
    .OrderByDescending(Function(x) Convert.ToDouble(x(columnToAnalyze))) _
    .Take(threshold) _
    .CopyToDataTable()
                ' Copy the top 5% rows to the new DataTable
                For Each row As DataRow In sortedDataTable.Rows
                    Dim newRow As DataRow = top5PercentTable.NewRow()
                    For Each col As DataColumn In sortedDataTable.Columns
                        If col.ColumnName = columnToAnalyze Then
                            col.ColumnName = "**" & col.ColumnName.ToUpper & "**" ' Enclose current column within **
                        End If
                        newRow(col.ColumnName) = row(col.ColumnName)
                    Next
                    top5PercentTable.Rows.Add(newRow)
                Next

                ' Creating DataTable for lowest 5% values
                Dim lowest5PercentTable As New DataTable()
                For Each col As DataColumn In dataTable.Columns
                    Dim columnName As String = col.ColumnName
                    If columnName = columnToAnalyze Then
                        columnName = "**" & columnName.ToUpper & "**" ' Enclose current column within **
                    End If
                    lowest5PercentTable.Columns.Add(columnName, col.DataType)
                Next

                Dim lowestSortedDataTable = dataTable.AsEnumerable() _
                .Where(Function(x) Double.TryParse(x(columnToAnalyze).ToString(), Nothing)) _
                .OrderBy(Function(x) Convert.ToDouble(x(columnToAnalyze))) _
                .Take(threshold) _
                .CopyToDataTable()

                ' Copy the lowest 5% rows to the new DataTable
                For Each row As DataRow In lowestSortedDataTable.Rows
                    Dim newRow As DataRow = lowest5PercentTable.NewRow()
                    For Each col As DataColumn In lowestSortedDataTable.Columns
                        If col.ColumnName = columnToAnalyze Then
                            col.ColumnName = "**" & col.ColumnName.ToUpper & "**" ' Enclose current column within **
                        End If
                        newRow(col.ColumnName) = row(col.ColumnName)
                    Next
                    lowest5PercentTable.Rows.Add(newRow)
                Next



                Dim top5PercentRange As String = "Top 5% Range: (" & values.OrderByDescending(Function(x) x).Take(threshold).LastOrDefault() & " - " & values.OrderByDescending(Function(x) x).Take(threshold).FirstOrDefault() & ")"
                Dim lowest5PercentRange As String = "Lowest 5% Range: (" & values.OrderBy(Function(x) x).Take(threshold).FirstOrDefault() & " - " & values.OrderBy(Function(x) x).Take(threshold).LastOrDefault() & ")"

                Dim top5PercentText As String = top5PercentRange & nl & PrintDataTablebyColumn(top5PercentTable)

                Dim lowest5PercentText As String = lowest5PercentRange & nl & PrintDataTablebyColumn(lowest5PercentTable)

                Dim divider As String = New String("-", 150)
                analytics &= "Analytics for **" & columnToAnalyze.ToUpper & "** :" & nl & nl &
                        maxValues & nl & nl &
                        minValues & nl & nl &
                        avgValuesText & nl & nl &
                        top5PercentText & nl &
                        lowest5PercentText & nl &
                        divider & nl & divider & nl & nl
            End If
        Next

        Return analytics

    End Function


    'COMMON FUNCTIONS  ==========================================================================================================
    '============================================================================================================================
    'Private Function SplitLineCsv(line As String) As String()
    '    Dim values As New List(Of String)()
    '    Dim inQuotes As Boolean = False
    '    Dim current As New StringBuilder()

    '    For Each c As Char In line
    '        If c = """"c Or c = "'" Then
    '            System.Diagnostics.Debug.WriteLine("Yes")
    '            inQuotes = Not inQuotes
    '        ElseIf c = ","c AndAlso Not inQuotes Then
    '            values.Add(current.ToString())
    '            current.Clear()
    '        Else
    '            current.Append(c)
    '        End If
    '    Next

    '    values.Add(current.ToString())
    '    Return values.ToArray()

    'End Function
    Private Function SplitCsv(line As String) As String()
        Dim values As New List(Of String)()
        Dim inQuotes As Boolean = False
        Dim current As New StringBuilder()

        For Each c As Char In line
            If c = """"c Then
                inQuotes = Not inQuotes
            ElseIf c = ","c AndAlso Not inQuotes Then
                values.Add(current.ToString())
                current.Clear()
            Else
                current.Append(c)
            End If
        Next

        values.Add(current.ToString())
        Return values.ToArray()
    End Function

    Private Function GetRowValues(dataTable As DataTable, row As DataRow, excludedColumn As String) As String
        Dim rowValues As New List(Of String)()
        For Each col As DataColumn In dataTable.Columns
            If col.ColumnName <> excludedColumn Then
                rowValues.Add(row(col).ToString())
            End If
        Next
        Return "(" & String.Join(", ", rowValues) & ")"
    End Function

    ' Function to print DataTable content into Results textbox
    Private Function PrintDataTable(dataTable As DataTable) As String
        Dim resultBuilder As New StringBuilder()

        ' Print DataRow content with record numbers and column names
        For i As Integer = 0 To dataTable.Rows.Count - 1
            ' Print record header
            resultBuilder.AppendLine("--------Record " & (i + 1) & "--------")

            ' Print the DataRow content
            Dim row As DataRow = dataTable.Rows(i)
            For Each col As DataColumn In dataTable.Columns
                resultBuilder.AppendLine(col.ColumnName & ": " & row(col).ToString())
            Next
        Next


        ' Set the result to the TextBox
        'textBox.Text = resultBuilder.ToString()
        Return resultBuilder.ToString()
    End Function

    ' Function to print DataTable content columnwise into Results textbox
    Private Function PrintDataTablebyColumn(dataTable As DataTable) As String
        Dim isFirstRow As Boolean = True
        Dim resultBuilder As New StringBuilder()

        For Each row As DataRow In dataTable.Rows
            ' Print column names only for the first row
            If isFirstRow Then
                For Each col As DataColumn In dataTable.Columns
                    ' Add padding based on the maximum length of column name and data
                    Dim padding = Math.Max(col.ColumnName.Length, dataTable.AsEnumerable().Max(Function(r) r(col).ToString().Length)) + 2
                    resultBuilder.Append(col.ColumnName.PadRight(padding))
                Next
                resultBuilder.AppendLine()

                ' Add a line below the column names
                For Each col As DataColumn In dataTable.Columns
                    Dim padding = Math.Max(col.ColumnName.Length, dataTable.AsEnumerable().Max(Function(r) r(col).ToString().Length)) + 2
                    resultBuilder.Append("".PadRight(padding, "-"))
                Next
                resultBuilder.AppendLine()

                isFirstRow = False
            End If

            ' Print the DataRow content
            For Each col As DataColumn In dataTable.Columns
                ' Add padding based on the maximum length of column name and data
                Dim padding = Math.Max(col.ColumnName.Length, dataTable.AsEnumerable().Max(Function(r) r(col).ToString().Length)) + 2

                resultBuilder.Append(row(col).ToString().PadRight(padding))
            Next
            resultBuilder.AppendLine()
        Next

        ' Set the result to the TextBox
        Return resultBuilder.ToString()

    End Function
    ' Function to print DataTable content into Results textbox
    Private Function PrintDataTablebyRecord(dataTable As DataTable) As String

        Dim resultBuilder As New StringBuilder()

        ' Print DataRow content with record numbers and column names
        For i As Integer = 0 To dataTable.Rows.Count - 1
            ' Print record header
            resultBuilder.AppendLine("--------Record " & (i + 1) & "--------")

            ' Print the DataRow content
            Dim row As DataRow = dataTable.Rows(i)
            For Each col As DataColumn In dataTable.Columns
                resultBuilder.AppendLine(col.ColumnName & ": " & row(col).ToString())
            Next
            resultBuilder.AppendLine(Environment.NewLine)
        Next

        Return resultBuilder.ToString()
    End Function

    Public Function cleanText(ByVal strText As String, Optional bMultiLine As Boolean = False) As String
        'TODO more!!
        If strText Is Nothing OrElse strText = "" Then
            Return ""
        End If
        strText = strText.Replace("http://", "")
        strText = strText.Replace("https://", "")
        strText = strText.Replace("schemas.microsoft.com/", "http://schemas.microsoft.com/")
        strText = strText.Replace("<%", "***")
        strText = strText.Replace("%>", "***")
        strText = strText.Replace("</", "***")
        strText = strText.Replace("/>", "***")
        strText = strText.Replace("'", "***")
        strText = strText.Replace("<>", "!=")
        strText = strText.Replace("<", "***less than***")
        strText = strText.Replace(">", "***more than***")
        Dim i, l As Integer
        Dim letter As String
        l = Len(strText)
        If l > 0 Then
            For i = 1 To l
                letter = Mid(strText, i, 1)
                If bMultiLine AndAlso (letter = Chr(13) OrElse letter = Chr(10)) Then
                    Continue For
                End If
                If (letter = ">") Or (letter = "<") Or (letter = "%") Or (letter = "'") Or (letter = Chr(39)) Or (letter = Chr(10)) Or (letter = Chr(13)) Then
                    If (i = 1) Then
                        strText = " " & Right(strText, l - 1)
                    Else
                        If (i = l) Then
                            strText = Left(strText, l - 1)
                        Else
                            strText = Left(strText, i - 1) & " " & Right(strText, l - i)
                        End If
                    End If
                End If
            Next
        End If
        Return Trim(strText)
    End Function

    Private Sub lnkbtnDownload_Click(sender As Object, e As EventArgs) Handles lnkbtnDownload.Click
        File.WriteAllText(hdnPath.Value, TextboxResult.Text)
        Try
            Response.ContentType = "application/octet-stream"
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & hdnFileName.Value)
            Response.TransmitFile(hdnPath.Value)
            Response.End()
        Catch ex As Exception
            LabelAlert.Text = "ERROR!! " & ex.Message
        End Try
    End Sub

    Private Sub lnkExportGrid1_Click(sender As Object, e As EventArgs) Handles lnkExportGrid1.Click
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Dim res, appldirExcelFiles, myfile As String
        'Dim applpath As String = ConfigurationManager.AppSettings("fileupload").ToString
        Dim datestr, timestr As String
        datestr = DateString()
        timestr = TimeString()
        myfile = "ExploreDataAI" & "_" & Session("logon").ToString & "_" & Mid(datestr, 7, 4) & Mid(datestr, 1, 2) & Mid(datestr, 4, 2) & Mid(timestr, 1, 2) & Mid(timestr, 4, 2) & Mid(timestr, 7, 2) & ".csv"
        appldirExcelFiles = applpath & "Temp\"
        Dim dt As DataTable = Session("GridView1DataSource")
        'header
        Dim hdr As String = "Explore data of report:   " & Session("REPTITLE")
        hdr = hdr + System.Environment.NewLine
        Dim delmtr As String = Session("delimiter")
        res = DataModule.ExportToExcel(dt, appldirExcelFiles, myfile, hdr, "", delmtr)
        Session("FileGridViewdata") = res
    End Sub

    Private Sub lnkGridAI_Click(sender As Object, e As EventArgs) Handles lnkGridAI.Click
        Session("DataToChatAI") = ExportToCSVtext(Session("dataTable"), Chr(9))
        Response.Redirect("~/ChatAI.aspx?pg=" & sFileType & "&srd=" & srd)
    End Sub
    Private Sub lnkTextAI_Click(sender As Object, e As EventArgs) Handles lnkTextAI.Click
        Session("DataToChatAI") = TextboxResult.Text
        Response.Redirect("~/ChatAI.aspx?pg=" & sFileType & "&srd=" & srd)
    End Sub
    Protected Sub btnSelectAllClicked(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        'If Not DropDownColumns1.AllSelected Then
        '    DropDownColumns1.SetAll(True)
        '    btnSelectAll.Enabled = False
        '    btnSelectAll.CssClass = "DataButtonDisabled"
        '    btnUnselectAll.Enabled = True
        '    btnUnselectAll.CssClass = "DataButtonEnabled"
        '    Dim j As Integer
        '    Session("SELECTEDFlds") = ""
        '    For j = 0 To DropDownColumns1.Items.Count - 1   'all fields in drop-down selected
        '        DropDownColumns1.Items(j).Selected = True
        '        Session("SELECTEDFlds") = Session("SELECTEDFlds") & "," & DropDownColumns.Items(j).Text
        '    Next
        '    DropDownColumns1.Text = Session("SELECTEDFlds")
        'End If
        btnSelectAll.Enabled = False
        btnSelectAll.CssClass = "DataButtonDisabled"
        btnUnselectAll.Enabled = True
        btnUnselectAll.CssClass = "DataButtonEnabled"
        Dim j As Integer
        Session("SELECTEDFlds") = ""
        For j = 0 To DropDownColumns1.Items.Count - 1   'all fields in drop-down selected
            DropDownColumns1.Items(j).Selected = True
            Session("SELECTEDFlds") = Session("SELECTEDFlds") & "," & DropDownColumns1.Items(j).Text
        Next
        Session("SELECTEDFlds") = Session("SELECTEDFlds").Replace(",", " ").Trim.Replace(" ", ",")
        DropDownColumns1.Text = Session("SELECTEDFlds")
    End Sub
    Protected Sub btnUnselectAllClicked(sender As Object, e As EventArgs) Handles btnUnselectAll.Click
        'If Not DropDownColumns1.NoneSelected Then
        '    DropDownColumns1.SetAll(False)
        '    'DropDownColumns.Text = "Please select..."
        '    DropDownColumns.Text = ""
        '    btnSelectAll.Enabled = True
        '    btnSelectAll.CssClass = "DataButtonEnabled"
        '    btnUnselectAll.Enabled = False
        '    btnUnselectAll.CssClass = "DataButtonDisabled"
        '    Dim j As Integer
        '    Session("SELECTEDFlds") = ""
        '    For j = 0 To DropDownColumns1.Items.Count - 1   'draw drop-down start
        '        DropDownColumns1.Items(j).Selected = False
        '    Next
        '    DropDownColumns1.Text = Session("SELECTEDFlds")
        'End If
        btnSelectAll.Enabled = True
        btnSelectAll.CssClass = "DataButtonEnabled"
        btnUnselectAll.Enabled = False
        btnUnselectAll.CssClass = "DataButtonDisabled"
        Dim j As Integer
        Session("SELECTEDFlds") = ""
        For j = 0 To DropDownColumns1.Items.Count - 1   'draw drop-down start
            DropDownColumns1.Items(j).Selected = False
        Next
        DropDownColumns1.Text = Session("SELECTEDFlds")
    End Sub

    Private Sub DropDownColumns1_ChecklistChanged(sender As Object, e As Controls_uc1.ChecklistChangedArgs) Handles DropDownColumns1.ChecklistChanged

        If DropDownColumns1.AllSelected Then
            btnSelectAll.Enabled = False
            btnUnselectAll.Enabled = True
        Else
            btnSelectAll.Enabled = True
            btnUnselectAll.Enabled = False
        End If
        Session("SELECTEDFlds") = ""
        For i = 0 To DropDownColumns1.Items.Count - 1
            If DropDownColumns1.Items(i).Selected Then
                Session("SELECTEDFlds") = Session("SELECTEDFlds") & "," & DropDownColumns1.Items(i).Text
            End If
        Next

        Session("SELECTEDFlds") = Session("SELECTEDFlds").Replace(",", " ").Trim.Replace(" ", ",")
        DropDownColumns1.Text = Session("SELECTEDFlds").Replace(",", " ").Trim.Replace(" ", ",")
        If Session("SELECTEDFlds").trim <> "" AndAlso Session("SELECTEDFlds").trim <> "ALL" Then
            Dim flds() As String = Split(Session("SELECTEDFlds"), ",")
            Dim dtt As New DataTable
            dtt = Session("OriginalDataTable")

            Dim srch As String = SearchStatement()
            If srch.Trim <> "" Then
                dtt.DefaultView.RowFilter = srch
                dtt = dtt.DefaultView.ToTable(True, flds)
                Session("dataTable") = dtt
                If dtt.Rows Is Nothing OrElse dtt.Rows.Count = 0 Then
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                ElseIf dtt.Rows.Count > 0 Then
                    LabelRowCount.Text = "Records returned: " & dtt.Rows.Count.ToString
                Else
                    LabelRowCount.Text = "Records returned: " & 0.ToString
                End If
                If Session("srch") <> srch Then
                    Session("srch") = srch
                    Session("SortedView") = Nothing
                    Session("SortColumn") = ""
                End If
            Else
                dtt = dtt.DefaultView.ToTable(True, flds)
            End If

            Session("dataTable") = dtt
            dataTable = Session("dataTable")
        End If

        ButtonUploadResult()
        Session("DataToChatAI") = TextboxResult.Text

        LabelRowCount.Text = "Records returned: " & Session("dataTable").Rows.Count.ToString

    End Sub

End Class
