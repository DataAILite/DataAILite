Imports System.Data
Imports System.Data.SqlClient
Partial Class TaskListSetting
    Inherits System.Web.UI.Page
    Dim unitname As String
    Dim unit As Integer
    Dim dv As DataView
    Dim ds As DataView
    Private Sub TaskListSetting_Init(sender As Object, e As EventArgs) Handles Me.Init

        HyperLinkBack.NavigateUrl = Session("urlback")

        Session("Prop3") = ""

        unitname = Session("Unit")
        Dim du As DataView = mRecords("SELECT * FROM ourunits WHERE [Unit]='" & unitname & "'")
        If du Is Nothing OrElse du.Count = 0 OrElse du.Table.Rows.Count = 0 Then
            unit = 3  '3 for OUReports, 8 for RunReport
        Else
            unit = du.Table.Rows(0)("Indx")
        End If
        'Dim re As String = GetDefaultColors(unit, unitname, Session("logon"))
        Session("unitindx") = unit
        Session("unitname") = unitname
        Dim re As String = GetDefaultColors(Session("unitindx"), Session("Unit"), Session("logon"))
    End Sub

    Private Sub TaskListSetting_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim er As String = String.Empty
        Dim sql As String = String.Empty
        Dim i As Integer = 0

        'dv = GetDefaultColors(unit, unitname, Session("logon"))
        dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "' And Prop1='header1'", er)
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND Prop3='default' And Prop1='header1'", er)
        End If
        Dim ctl As LinkButton
        'header1
        txtHeader1.Text = dv.Table.Rows(0)("FldText")
        tblHeaders.Rows(1).Cells(1).InnerHtml = "<input type='color' name='h1color' value='" & dv.Table.Rows(0)("FldColor") & "' id='clrHeader1'>"
        'tblHeaders.Rows(1).Cells(3).InnerHtml = "<input type='submit' value='update' name='h1submit'>"
        ctl = New LinkButton
        ctl.Text = "save"
        ctl.ID = "header1_save_" & dv.Table.Rows(0)("Indx").ToString
        ctl.ToolTip = "Save header1"
        AddHandler ctl.Click, AddressOf btnSAVE_Click
        tblHeaders.Rows(1).Cells(3).Controls.Clear()
        tblHeaders.Rows(1).Cells(3).Controls.Add(ctl)

        If dv.Table.Rows(0)("Prop3").ToString.Trim = "default" Then
            Session("Prop3") = "default"
        Else
            Session("Prop3") = ""
        End If

        'header2
        dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & " AND [User]='" & Session("logon") & "'" & " And Prop1='header2'", er)
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND Prop3='default' And Prop1='header2'", er)
        End If
        txtHeader2.Text = dv.Table.Rows(0)("FldText")
        tblHeaders.Rows(2).Cells(1).InnerHtml = "<input type='color' name='h2color' value='" & dv.Table.Rows(0)("FldColor") & "' id='clrHeader2'>"
        'tblHeaders.Rows(2).Cells(3).InnerHtml = "<input type='submit' value='update' name='h2submit'>"
        ctl = New LinkButton
        ctl.Text = "save"
        ctl.ID = "header2_save_" & dv.Table.Rows(0)("Indx").ToString
        ctl.ToolTip = "Save header2"
        AddHandler ctl.Click, AddressOf btnSAVE_Click
        tblHeaders.Rows(2).Cells(3).Controls.Clear()
        tblHeaders.Rows(2).Cells(3).Controls.Add(ctl)

        'dropdown Version
        dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & " AND [User]='" & Session("logon") & "'" & " And Prop1='version'  ORDER BY FldOrder", er)
        If dv Is Nothing OrElse dv.Table Is Nothing OrElse dv.Table.Rows.Count = 0 Then
            dv = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND Prop3='default' And Prop1='version'  ORDER BY FldOrder", er)
        End If
        For i = 0 To dv.Table.Rows.Count - 1
            AddRowIntoHTMLtable(dv.Table.Rows(i), tblVersion)
            tblVersion.Rows(i + 2).Cells(0).InnerHtml = "<input type='text' Width='233px' name='vertxt" & dv.Table.Rows(i)("Indx").ToString & "' value='" & dv.Table.Rows(i)("FldText") & "' id='verid" & dv.Table.Rows(i)("Indx").ToString & "'>"
            tblVersion.Rows(i + 2).Cells(1).InnerHtml = "<input type='color' name='verclr" & dv.Table.Rows(i)("Indx").ToString & "' value='" & dv.Table.Rows(i)("FldColor").ToString & "'>"

            'tblVersion.Rows(i + 2).Cells(2).InnerHtml = "<input type='submit' value='update' name='verupdate" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;"
            ctl = New LinkButton
            ctl.Text = "save"
            ctl.ID = "version_save_" & i.ToString
            ctl.ToolTip = "Save this record"
            AddHandler ctl.Click, AddressOf btnSAVE_Click
            tblVersion.Rows(i + 2).Cells(2).Controls.Clear()
            tblVersion.Rows(i + 2).Cells(2).Controls.Add(ctl)

            'tblVersion.Rows(i + 2).Cells(3).InnerHtml = "<input type='submit' value='delete' name='verdelete" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;"
            ctl = New LinkButton
            ctl.Text = "del"
            ctl.ID = "version_del_" & dv.Table.Rows(i)("Indx").ToString
            ctl.ToolTip = "Delete this record"
            AddHandler ctl.Click, AddressOf btnDEL_Click
            tblVersion.Rows(i + 2).Cells(3).Controls.Clear()
            tblVersion.Rows(i + 2).Cells(3).Controls.Add(ctl)
            If i > 0 Then
                'tblVersion.Rows(i + 2).Cells(3).InnerHtml = "<input type='submit' value='up' name='verup" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;"
                ctl = New LinkButton
                ctl.Text = "up"
                ctl.ID = "version_up_" & i.ToString
                ctl.ToolTip = "Move  this record up"
                AddHandler ctl.Click, AddressOf btnUP_Click
                tblVersion.Rows(i + 2).Cells(4).Controls.Clear()
                tblVersion.Rows(i + 2).Cells(4).Controls.Add(ctl)
            Else
                tblVersion.Rows(i + 2).Cells(4).InnerHtml = "&nbsp;&nbsp;"
            End If
            If i < dv.Table.Rows.Count - 1 Then
                'tblVersion.Rows(i + 2).Cells(4).InnerHtml = "<input type='submit' value='down' name='verdown" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                ctl = New LinkButton
                ctl.Text = "down"
                ctl.ID = "version_down_" & i.ToString
                ctl.ToolTip = "Move  this record down"
                AddHandler ctl.Click, AddressOf btnDOWN_Click
                tblVersion.Rows(i + 2).Cells(5).Controls.Clear()
                tblVersion.Rows(i + 2).Cells(5).Controls.Add(ctl)
            Else
                tblVersion.Rows(i + 2).Cells(5).InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
            End If
            tblVersion.Rows(i + 2).Cells(6).InnerHtml = "&nbsp;&nbsp;&nbsp;"
        Next

        'dropdown Status
        ds = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & " AND [User]='" & Session("logon") & "'" & " And Prop1='status'  ORDER BY FldOrder", er)
        If ds Is Nothing OrElse ds.Table Is Nothing OrElse ds.Table.Rows.Count = 0 Then
            ds = mRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND Prop3='default' And Prop1='status'  ORDER BY FldOrder", er)
        End If
        For i = 0 To ds.Table.Rows.Count - 1
            AddRowIntoHTMLtable(ds.Table.Rows(i), tblStatus)
            tblStatus.Rows(i + 2).Cells(0).InnerHtml = "<input type='text' Width='233px' name='sttxt" & ds.Table.Rows(i)("Indx").ToString & "' value='" & ds.Table.Rows(i)("FldText") & "' id='stid" & ds.Table.Rows(i)("Indx").ToString & "'>"
            tblStatus.Rows(i + 2).Cells(1).InnerHtml = "<input type='color' name='stclr" & ds.Table.Rows(i)("Indx").ToString & "' value='" & ds.Table.Rows(i)("FldColor") & "' id='clrStColor" & ds.Table.Rows(i)("Indx").ToString & "'>"

            'tblStatus.Rows(i + 2).Cells(2).InnerHtml = "<input type='submit' value='update' name='stupdate" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;"
            ctl = New LinkButton
            ctl.Text = "save"
            ctl.ID = "status_save_" & i.ToString
            ctl.ToolTip = "Save this record"
            AddHandler ctl.Click, AddressOf btnSAVE_Click
            tblStatus.Rows(i + 2).Cells(2).Controls.Clear()
            tblStatus.Rows(i + 2).Cells(2).Controls.Add(ctl)

            'tblStatus.Rows(i + 2).Cells(5).InnerHtml = "<input type='submit' value='delete' name='stdelete" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;"
            ctl = New LinkButton
            ctl.Text = "del"
            ctl.ID = "status_del_" & i.ToString
            ctl.ToolTip = "Delete this record"
            AddHandler ctl.Click, AddressOf btnDEL_Click
            tblStatus.Rows(i + 2).Cells(3).Controls.Clear()
            tblStatus.Rows(i + 2).Cells(3).Controls.Add(ctl)

            If i > 0 Then
                'tblStatus.Rows(i + 2).Cells(4).InnerHtml = "<input type='submit' value='up' name='stup" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;"
                ctl = New LinkButton
                ctl.Text = "up"
                ctl.ID = "status_up_" & i.ToString
                ctl.ToolTip = "Move  this record up"
                AddHandler ctl.Click, AddressOf btnUP_Click
                tblStatus.Rows(i + 2).Cells(4).Controls.Clear()
                tblStatus.Rows(i + 2).Cells(4).Controls.Add(ctl)
            Else
                tblStatus.Rows(i + 2).Cells(4).InnerHtml = "&nbsp;&nbsp;"
            End If
            If i < ds.Table.Rows.Count - 1 Then
                'tblStatus.Rows(i + 2).Cells(5).InnerHtml = "<input type='submit' value='down' name='stdown" & dv.Table.Rows(i)("Indx").ToString & "'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
                ctl = New LinkButton
                ctl.Text = "down"
                ctl.ID = "status_down_" & i.ToString
                ctl.ToolTip = "Move  this record down"
                AddHandler ctl.Click, AddressOf btnDOWN_Click
                tblStatus.Rows(i + 2).Cells(5).Controls.Clear()
                tblStatus.Rows(i + 2).Cells(5).Controls.Add(ctl)
            Else
                tblStatus.Rows(i + 2).Cells(5).InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
            End If
            tblStatus.Rows(i + 2).Cells(6).InnerHtml = "&nbsp;&nbsp;&nbsp;"
        Next
    End Sub
    Protected Sub lnkbtnAddVersion_Click(sender As Object, e As EventArgs) Handles lnkbtnAddVersion.Click
        If txtNewVersion.Text.Trim <> "" Then
            Dim er As String = String.Empty
            Dim sql As String = String.Empty
            Dim i As Integer = 0
            If Not HasRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "'" & " And Prop1='version' AND FldText='" & txtNewVersion.Text.Trim & "'") Then
                sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3, FldText, FldOrder, FldColor, Unit, UnitName, [User]) VALUES('version','" & Session("Prop3") & "','" & txtNewVersion.Text.Trim & "',0,'" & Request("addclrVersion").ToString & "'," & Session("unitindx").ToString & ",'" & unitname.ToString & "','" & Session("logon") & "')"
                er = ExequteSQLquery(sql)
            End If
            Response.Redirect("TaskListsetting.aspx")
        End If
    End Sub
    Protected Sub lnkbtnAddStatus_Click(sender As Object, e As EventArgs) Handles lnkbtnAddStatus.Click
        If txtNewStatus.Text.Trim <> "" Then
            Dim er As String = String.Empty
            Dim sql As String = String.Empty
            Dim i As Integer = 0
            If Not HasRecords("Select * FROM ourtasklistsetting WHERE UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "'" & " And Prop1='status' AND FldText='" & txtNewStatus.Text.Trim & "'") Then
                sql = "INSERT INTO ourtasklistsetting (Prop1, Prop3, FldText, FldOrder, FldColor, Unit, UnitName, [User]) VALUES('status','" & Session("Prop3") & "','" & txtNewStatus.Text.Trim & "',0,'" & Request("addclrStatus").ToString & "'," & unit.ToString & ",'" & Session("unitname").ToString & "','" & Session("logon") & "')"
                er = ExequteSQLquery(sql)
            End If
            Response.Redirect("TaskListsetting.aspx")
        End If
    End Sub
    Protected Sub h1submit_Click(ByVal rowind As Integer) '(sender As Object, e As EventArgs) 'Handles h1submit.Click
        Dim er As String = String.Empty
        Dim sql As String = String.Empty
        'Dim indx = ds.Table.Rows(rowind)("Indx")
        sql = "UPDATE ourtasklistsetting SET FldText='" & Request("txtHeader1").ToString & "', FldColor='" & Request("h1color").ToString & "' WHERE Indx=" & rowind ' UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "' And Prop1='header1'"
        er = ExequteSQLquery(sql)
        Response.Redirect("TaskListsetting.aspx")
    End Sub
    Protected Sub h2submit_Click(ByVal rowind As Integer) '(sender As Object, e As EventArgs) 'Handles h2submit.Click
        Dim er As String = String.Empty
        Dim sql As String = String.Empty
        Dim indx = ds.Table.Rows(rowind)("Indx")
        sql = "UPDATE ourtasklistsetting SET FldText='" & Request("txtHeader2").ToString & "', FldColor='" & Request("h2color").ToString & "' WHERE Indx=" & rowind ' UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "' And Prop1='header2'"
        er = ExequteSQLquery(sql)
        Response.Redirect("TaskListsetting.aspx")
    End Sub
    Protected Sub SaveStatus(ByVal rowind As Integer)
        Dim er As String = String.Empty
        Dim sql As String = String.Empty
        Dim indx = ds.Table.Rows(rowind)("Indx")
        sql = "UPDATE ourtasklistsetting SET FldText='" & Request("sttxt" & indx.ToString).ToString & "', FldColor='" & Request("stclr" & indx.ToString).ToString & "' WHERE Indx=" & indx ' UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "' And Prop1='status' AND Indx=" & indx
        er = ExequteSQLquery(sql)
        Response.Redirect("TaskListsetting.aspx")
    End Sub
    Protected Sub SaveVersion(ByVal rowind As Integer)
        Dim er As String = String.Empty
        Dim sql As String = String.Empty
        Dim indx = dv.Table.Rows(rowind)("Indx")
        sql = "UPDATE ourtasklistsetting SET FldText='" & Request("vertxt" & indx.ToString).ToString & "', FldColor='" & Request("verclr" & indx.ToString).ToString & "' WHERE Indx=" & indx ' UnitName='" & Session("unitname").ToString & "' AND [User]='" & Session("logon") & "' And Prop1='version' AND Indx=" & indx
        er = ExequteSQLquery(sql)
        Response.Redirect("TaskListsetting.aspx")
    End Sub
    Protected Sub btnUP_Click(sender As Object, e As EventArgs)
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim id1 As String = Piece(sqlindx, "_", 1)
        Dim id2 As String = Piece(sqlindx, "_", 2)
        Dim id3 As String = Piece(sqlindx, "_", 3)
        Dim dt As DataTable
        If id1 = "version" Then
            dt = dv.Table
        ElseIf id1 = "status" Then
            dt = ds.Table
        Else
            dt = Nothing
            Exit Sub
        End If
        If id2 = "up" Then
            dt = UpRowInDataTable(dt, "FldOrder", id3)
            UpdateRecordOrderInDB("ourtasklistsetting", "FldOrder", "Indx", "int", dt)
            Response.Redirect("TaskListsetting.aspx")
        End If
    End Sub
    Protected Sub btnDOWN_Click(sender As Object, e As EventArgs)
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim id1 As String = Piece(sqlindx, "_", 1)
        Dim id2 As String = Piece(sqlindx, "_", 2)
        Dim id3 As String = Piece(sqlindx, "_", 3)
        Dim dt As DataTable
        If id1 = "version" Then
            dt = dv.Table
        ElseIf id1 = "status" Then
            dt = ds.Table
        Else
            dt = Nothing
            Exit Sub
        End If
        If id2 = "down" Then
            dt = DownRowInDataTable(dt, "FldOrder", id3)
            UpdateRecordOrderInDB("ourtasklistsetting", "FldOrder", "Indx", "int", dt)
            Response.Redirect("TaskListsetting.aspx")
        End If
    End Sub
    Protected Sub btnSAVE_Click(sender As Object, e As EventArgs)
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim id1 As String = Piece(sqlindx, "_", 1)
        Dim id2 As String = Piece(sqlindx, "_", 2)
        Dim id3 As String = Piece(sqlindx, "_", 3)
        If id2 <> "save" Then
            Exit Sub
        End If
        If id1 = "header1" Then
            h1submit_Click(id3)
        ElseIf id1 = "header2" Then
            h2submit_Click(id3)
        ElseIf id1 = "version" Then
            SaveVersion(id3)
        ElseIf id1 = "status" Then
            SaveStatus(id3)
        End If
        Response.Redirect("TaskListsetting.aspx")
    End Sub
    Protected Sub btnDEL_Click(sender As Object, e As EventArgs)
        Dim sqlindx As String = CType(sender, LinkButton).ID
        Dim id1 As String = Piece(sqlindx, "_", 1)
        Dim id2 As String = Piece(sqlindx, "_", 2)
        Dim id3 As String = Piece(sqlindx, "_", 3)
        If id2 = "del" Then
            Dim sql, er As String
            sql = "DELETE FROM ourtasklistsetting WHERE Indx=" & id3
            er = ExequteSQLquery(Sql)
        End If
        Response.Redirect("TaskListsetting.aspx")
    End Sub

End Class
