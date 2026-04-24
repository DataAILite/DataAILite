Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Drawing
'Imports System.Math
Partial Class TaskListTimeLine
    Inherits System.Web.UI.Page
    Dim tmdt As DataTable
    Private Sub TaskListTimeLine_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        Session("WEBOUR") = ConfigurationManager.AppSettings("weboureports").ToString
        HyperLink2.NavigateUrl = Session("WEBOUR")
    End Sub

    Private Sub TaskListTimeLine_Load(sender As Object, e As EventArgs) Handles Me.Load
        'fill out tmdt from mnthdata
        Dim mnthdata As DataTable = Session("Assignments")
        If mnthdata Is Nothing OrElse mnthdata.Rows.Count = 0 Then
            Exit Sub
        End If
        Dim i As Integer
        'Dim minstartdate As String = DateToString(Now.ToShortDateString)
        'minstartdate = minstartdate.Substring(0, minstartdate.IndexOf(" "))
        Dim maxenddate As String = DateToString(Now.ToShortDateString)
        If maxenddate.IndexOf(" ") > 0 Then
            maxenddate = maxenddate.Substring(0, maxenddate.IndexOf(" "))
        End If
        'create columns in tmdt
        For i = 0 To mnthdata.Rows.Count - 1
            'If minstartdate > mnthdata.Rows(i)("Start").ToString Then
            '    minstartdate = mnthdata.Rows(i)("Start").ToString
            'End If
            If maxenddate < mnthdata.Rows(i)("Deadline").ToString Then
                maxenddate = mnthdata.Rows(i)("Deadline").ToString
            End If
        Next
        maxenddate = DateToString(Convert.ToDateTime(maxenddate).AddMonths(1))
        'add colunms from curent month to maxenddate
        Dim tmdt As New DataTable
        Dim col As DataColumn
        col = New DataColumn
        col.DataType = System.Type.GetType("System.Int16")
        col.ColumnName = "Task"
        tmdt.Columns.Add(col)
        i = 0
        Dim j As Integer = 0
        Dim d As DateTime
        d = DateTime.Now
        If DateToString(d) > maxenddate Then
            Exit Sub
        End If
        While DateToString(d) <= maxenddate
            i = i + 1
            col = New DataColumn
            col.DataType = System.Type.GetType("System.String")
            col.ColumnName = DateToString(d).Substring(0, DateToString(d).LastIndexOf("-")) '"Current Month"
            tmdt.Columns.Add(col)
            d = DateTime.Now.AddMonths(i)
        End While

        'add tasks
        For i = 0 To mnthdata.Rows.Count - 1
            If mnthdata.Rows(i)("Deadline").ToString.Trim <> "" AndAlso mnthdata.Rows(i)("Deadline").ToString.Trim > tmdt.Columns(1).Caption Then
                Dim rw As DataRow = tmdt.NewRow()
                rw(0) = mnthdata.Rows(i)("ID")
                tmdt.Rows.Add(rw)
                For j = 1 To tmdt.Columns.Count - 1
                    If mnthdata.Rows(i)("Deadline").ToString.StartsWith(tmdt.Columns(j).Caption) Then
                        rw(j) = mnthdata.Rows(i)("Deadline").ToString
                        Exit For
                    Else
                        rw(j) = "."
                    End If
                Next
            End If
        Next

        GridViewTML.DataSource = tmdt.DefaultView
        GridViewTML.Visible = True
        GridViewTML.DataBind()
        If tmdt Is Nothing OrElse tmdt.Rows.Count = 0 Then
            Exit Sub
        End If
        Dim m As Integer
        GridViewTML.CellPadding = 0
        For i = 0 To GridViewTML.Rows.Count - 1
            GridViewTML.Rows(i).Height = 20
            If GridViewTML.Rows(i).Cells(0).Text = Session("tn") Then
                GridViewTML.Rows(i).Cells(0).BackColor = Color.Yellow
            End If
            GridViewTML.Rows(i).ToolTip = "Task #" & GridViewTML.Rows(i).Cells(0).Text
            Dim colr As String = ColorOfTask(GridViewTML.Rows(i).Cells(0).Text, Session("logon"), GridViewTML.Rows(i).ToolTip)
            For j = 0 To GridViewTML.Rows(0).Cells.Count - 1
                If GridViewTML.Rows(i).Cells(j).Text.Trim.StartsWith(".") Then
                    GridViewTML.Rows(i).Cells(j).BackColor = System.Drawing.ColorTranslator.FromHtml(colr) 'Color.Bisque
                ElseIf GridViewTML.Rows(i).Cells(j).Text.Trim.StartsWith("20") Then
                    GridViewTML.Rows(i).ToolTip = GridViewTML.Rows(i).ToolTip & ", deadline " & GridViewTML.Rows(i).Cells(j).Text
                    'GridViewTML.Rows(i).Cells(j).BackColor = System.Drawing.ColorTranslator.FromHtml(colr) 'Color.Bisque
                    'w = GridViewTML.Rows(i).Cells(j).Width
                    m = CInt(GridViewTML.Rows(i).Cells(j).Text.Substring(GridViewTML.Rows(i).Cells(j).Text.LastIndexOf("-") + 1))
                    Dim tbl As New Table
                    Dim row As New TableRow
                    Dim col1 As New TableCell
                    col1.Width = Math.Round((100 * m / 31)) '.ToString & "%"
                    col1.BackColor = System.Drawing.ColorTranslator.FromHtml(colr)
                    Dim col2 As New TableCell
                    col2.Width = Math.Round(100 * (1 - m / 31)) '.ToString & "%"
                    row.Cells.Add(col1)
                    row.Cells.Add(col2)
                    row.Height = 20
                    tbl.Rows.Add(row)
                    tbl.BorderWidth = 0
                    tbl.BorderColor = Color.FromName("red")
                    GridViewTML.Rows(i).Cells(j).Controls.Add(tbl)

                End If
            Next
            Dim hp As HyperLink = New HyperLink
            hp.Text = GridViewTML.Rows(i).Cells(0).Text
            hp.NavigateUrl = "~/TaskList.aspx?tn=" + hp.Text
            GridViewTML.Rows(i).Cells(0).Controls.Add(hp)
        Next
    End Sub
End Class
