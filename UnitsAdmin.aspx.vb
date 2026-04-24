Imports System.Data
Imports System.Data.SqlClient
Partial Class UnitsAdmin
    Inherits System.Web.UI.Page

    Private Sub UnitsAdmin_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session Is Nothing OrElse Session("admin") Is Nothing OrElse Session("admin").ToString = "" Then
            Response.Redirect("~/Default.aspx?msg=SessionExpired")
        End If
        If Session("admin").ToString <> "super" Then
            Response.Redirect("~/Default.aspx")
        End If
        Dim err As String = String.Empty
        Dim mSql As String = String.Empty
        Dim userconnstrnopass As String = Session("UserConnString")
        userconnstrnopass = userconnstrnopass.Substring(0, userconnstrnopass.IndexOf("Password")).Trim()
        Dim srch As String = SearchText.Text.Trim
        mSql = "SELECT Unit,DistrMode,UnitWeb,OURConnStr,OURConnPrv,UserConnStr,UserConnPrv,Comments,StartDate,EndDate,Indx FROM OURUnits "
        If srch <> "" Then
            mSql = mSql & "  WHERE "
            mSql = mSql & " ((Unit LIKE '%" & srch & "%') OR (DistrMode LIKE '%" & srch & "%') OR (UnitWeb LIKE '%" & srch & "%') OR (OURConnStr LIKE '%" & srch & "%') OR (UserConnStr LIKE '%" & srch & "%') OR (Comments LIKE '%" & srch & "%')) "
        End If
        mSql = mSql & " ORDER BY Unit, UserConnStr"

        Try
            Dim dvu As DataView = mRecords(mSql, err)  'Data for report by SQL statement from the OURdb database
            If err <> "" Then
                Label3.Text = err
                Exit Sub
            End If
            Label3.Text = dvu.Table.Rows.Count.ToString & " units"
            GridViewUnits.DataSource = dvu
            GridViewUnits.Visible = True
            GridViewUnits.DataBind()
        Catch ex As Exception
            Label3.Text = ex.Message
        End Try

    End Sub

    Private Sub btnRegistration_Click(sender As Object, e As EventArgs) Handles btnRegistration.Click
        Response.Redirect("UnitDefinition.aspx")
    End Sub

    Private Sub GridViewUnits_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles GridViewUnits.PageIndexChanging
        GridViewUnits.PageIndex = e.NewPageIndex
        GridViewUnits.DataBind()
    End Sub
End Class
